using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts;
using INPS.Pensioni.LiquidazioneFs.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Threading;
using INPS.Pensioni.LiquidazioneFs.ServiceReferences.AggPec;
using System.Reflection;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneFs.Service
{
    [DNA.Exceptions.Services.ExceptionShielding]
    public class ServizioLiquidazioneFs : DNA.Services.ServiceBase, IServizioLiquidazioneFs
    {
        #region Culture
        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("it-IT");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("it-IT");
        }
        #endregion Culture

        #region Versioni
        public AreaEsito GetListaVersioniFS(out AreaVersioni areaVersioni)
        {
            SetCulture();

            areaVersioni = new AreaVersioni();
            areaVersioni.ListaVersioni = new Dictionary<string, string>();
            AreaEsito esito = new AreaEsito();

            try
            {
                List<GestioneVersioni.DatiVersioni> elencoVersioni = null;
                GestioneVersioni.GetVersioni(out elencoVersioni);

                Utility.GetListaVersioni(ref elencoVersioni, Utility.ChiaviVersioni.WCFFS, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision);

                areaVersioni.ListaVersioni = Utility.FormattaVersioni(elencoVersioni);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                esito.Messaggio = "Errore nel recupero delle versioni di rilascio. Riprovare più tardi";
                return esito;
            }

            esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            esito.Messaggio = string.Empty;

            return esito;
        }
        #endregion Versioni

        #region Get Dati Pensione

        public long GetIdPensioneByNumeroDomanda(long numeroDomanda, byte? progStorico)
        {
            SetCulture();

            long IdPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(numeroDomanda, progStorico, out IdPensione);
            return IdPensione;
        }

        public GestionePensione.DatiPensione GetDatiPensioneByNumeroDomanda(long numeroDomanda, byte? progStorico)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);
            return datiPensione;
        }

        #endregion Get Dati Pensione

        #region LiquidazionePensione

        public AreaEsito GetLiquidazionePensioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object datiFondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);

            GetDatiDBFondi(ref contenitore, out datiFondoXX);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = contenitore.DatiAnagraficiTitolare;
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = contenitore.DatiDetrazioni;
            GestioneDatiControlloFelpe.ControlloFelpe controlloFelpe = contenitore.DatiControlloFelpe;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = contenitore.ListaDatiPensioneINPDAP;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;

            AreaEsito Esito = new AreaEsito();
            areaLiquidazionePensione = null;


            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                DatiGenericiINPDAP datiGenericiINPDAP = null;
                GestioneLiquidazionePensione.GetDatiGenericiINPDAP(ref contenitore, datiPensione, datiIstruttoriaCommon, datiFondoCommon, listaDatiPensioneINPDAP != null ? listaDatiPensioneINPDAP.FirstOrDefault() : null,
                    controlloFelpe, datiEliminazione, out datiGenericiINPDAP);
                if (datiGenericiINPDAP != null)
                {
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.DatiGenericiINPDAP = datiGenericiINPDAP;
                }
            }
            else
            {
                DatiGenerici datiGenerici = null;
                GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, datiPensione, datiIstruttoriaCommon, datiFondoCommon, controlloFelpe, out datiGenerici);
                if (datiGenerici != null)
                {
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.DatiGenerici = datiGenerici;
                }
            }

            GetDatiAssicurativi(ref contenitore, datiPensione, datiFondoCommon, listaDatiPensioneINPDAP != null ? listaDatiPensioneINPDAP.FirstOrDefault() : null, isRiaperturaDomanda, ref areaLiquidazionePensione);

            DatiPrecedentePensione datiPrecedentePensione = null;
            GestioneLiquidazionePensione.ValorizzaDatiPrecedentePensione(datiIstruttoriaCommon, out datiPrecedentePensione);
            if (datiPrecedentePensione != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiPrecedentePensione = datiPrecedentePensione;
            }

            DatiBititolaritaInail datiBititolaritaInail = null;
            GestioneLiquidazionePensione.GetDatiBititolaritaInailByIdPensione(ref contenitore, out datiBititolaritaInail);
            if (datiBititolaritaInail != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiBititolaritaInail = datiBititolaritaInail;
            }

            DatiLegge460 datiLegge460 = null;
            GestioneLiquidazionePensione.GetDatiLegge460ByIdPensione(ref contenitore, out datiLegge460);
            if (datiLegge460 != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiLegge460 = datiLegge460;
            }

            DatiLiquidazionePensioneStorico datiLiquidazionePensioneStorico = null;
            GestioneLiquidazionePensione.GetDatiLiquidazionePensioneStorico(ref contenitore, out datiLiquidazionePensioneStorico);
            if (datiLiquidazionePensioneStorico != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiLiquidazionePensioneStorico = datiLiquidazionePensioneStorico;
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                DatiIstruttoriaINPDAP datiIstruttoriaINPDAP = null;
                GestioneLiquidazionePensione.GetDatiIsruttoriaINPDAPByIdPensione(datiFondoCommon, datiIstruttoriaCommon, out datiIstruttoriaINPDAP);

                if (datiIstruttoriaINPDAP != null)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.DatiIstruttoriaINPDAP = datiIstruttoriaINPDAP;
                }
            }

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaLiquidazionePensione);
            GetCrossProperties(ref contenitore, ref contenitoreDecodifica, datiPensione, datiAnagrafici, datiFondoCommon, datiIstruttoriaCommon, datiFondoXX, datiMaggiorazioniBeneficiCommon, datiDetrazioni, isRiaperturaDomanda, datiDanteCausa,
                tipoFondo, datiStoricoGP, ref areaLiquidazionePensione);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        public AreaEsito StoreLiquidazionePensione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiDL407 datiDL407 = null;
            DatiPrivilegiate datiPrivilegiate = null;
            DatiArticolo2 datiArticolo2 = null;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);
            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, datiPensione.Id, datiPensione.SiglaCategoria, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiDL407, out datiPrivilegiate, out datiArticolo2);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = contenitore.DatiAnagraficiTitolare;
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = contenitore.DatiDetrazioni;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = contenitore.ListaDatiPensioneINPDAP;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = contenitore.DatiControlloFelpe;
            GestionePagamento.DatiPagamento datiPagamento = contenitore.DatiPagamento;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            AreaEsito Esito = new AreaEsito();

            string messaggioControllo = "";

            #region DatiGenerici
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneLiquidazionePensione.ControlDatiGenericiINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione, false, areaLiquidazionePensione.DatiGenericiINPDAP, areaLiquidazionePensione.DatiAssicurativiINPDAP,
                    areaLiquidazionePensione.ListaRipartizioneINPDAP, datiFondoCommon, listaDatiPensioneINPDAP, datiIstruttoriaCommon, datiExCombattente, datiBenefici,
                    datiAnagrafici, datiDetrazioni, datiEliminazione, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneLiquidazionePensione.StoreDatiGenericiINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione, areaLiquidazionePensione.DatiGenericiINPDAP, areaLiquidazionePensione.DatiAssicurativiINPDAP, ref datiIstruttoriaCommon,
                    ref datiFondoCommon, ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione, ref datiEliminazione, ref datiPagamento, isRiaperturaDomanda, false, false);
            }
            else
            {
                GestioneLiquidazionePensione.ControlDatiGenerici(ref contenitore, ref contenitoreDecodifica, datiPensione, datiAnagrafici, datiIstruttoriaCommon, datiDetrazioni, areaLiquidazionePensione.DatiGenerici, areaLiquidazionePensione.DatiAssicurativi, areaLiquidazionePensione.ListaRecordFondo,
                    datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, datiEliminazione, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneLiquidazionePensione.StoreDatiGenerici(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiIstruttoriaCommon, ref datiFondoCommon, areaLiquidazionePensione.DatiGenerici, false,
                    datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, areaLiquidazionePensione.DatiAssicurativi, areaLiquidazionePensione.ListaRecordFondo, false, ref datiPagamento);
            }
            #endregion DatiGenerici

            #region DatiAssicurativi
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneLiquidazionePensione.ControlDatiAssicurativiINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione, areaLiquidazionePensione.DatiAssicurativiINPDAP, areaLiquidazionePensione.DatiGenericiINPDAP,
                    datiMaggiorazioniBeneficiCommon, datiFondoCommon, datiIstruttoriaCommon, listaDatiPensioneINPDAP, datiControlloFelpe, datiEliminazione, false, isRiaperturaDomanda, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneLiquidazionePensione.StoreDatiAssicurativiINPDAP(ref contenitore, datiPensione, areaLiquidazionePensione.DatiAssicurativiINPDAP, areaLiquidazionePensione.ListaRipartizioneINPDAP,
                    ref datiFondoCommon, ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione, false);
            }
            else
            {
                GestioneLiquidazionePensione.ControlDatiAssicurativi(ref contenitore, ref contenitoreDecodifica, datiPensione, datiIstruttoriaCommon, datiMaggiorazioniBeneficiCommon, datiFondoCommon, areaLiquidazionePensione.DatiGenerici, areaLiquidazionePensione.DatiAssicurativi,
                areaLiquidazionePensione.ListaRecordFondo, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
                    if (datiQuadroLiquidazionePensione.TabDatiAssicurativi == 0)
                        GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS));

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                try
                {
                    GestioneLiquidazionePensione.StoreDatiAssicurativi(ref contenitore, datiPensione, ref datiFondoCommon, areaLiquidazionePensione.DatiAssicurativi, areaLiquidazionePensione.ListaRecordFondo, false);
                }
                catch (Exception)
                {
                    datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
                    if (datiQuadroLiquidazionePensione.TabDatiAssicurativi == 0)
                        GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS));
                    throw;
                }
            }

            #endregion DatiAssicurativi

            #region DatiPrecedentePensione
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                GestioneLiquidazionePensione.ControlDatiPrecedentePensioneINPDAP(areaLiquidazionePensione.DatiGenericiINPDAP, areaLiquidazionePensione.DatiPrecedentePensione, out messaggioControllo);
            else
                GestioneLiquidazionePensione.ControlDatiPrecedentePensione(areaLiquidazionePensione.DatiGenerici, areaLiquidazionePensione.DatiPrecedentePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneLiquidazionePensione.StoreDatiPrecedentePensione(ref contenitore, datiPensione, ref datiIstruttoriaCommon, areaLiquidazionePensione.DatiPrecedentePensione);

            #endregion DatiPrecedentePensione

            #region DatiBititolaritaInail
            GestioneLiquidazionePensione.ControlDatiBititolaritaInail(areaLiquidazionePensione.DatiBititolaritaInail, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneLiquidazionePensione.StoreDatiBititolaritaInail(ref contenitore, datiPensione, areaLiquidazionePensione.DatiBititolaritaInail);
            #endregion DatiBititolaritaInail

            #region Dati Legge4/60

            GestioneLiquidazionePensione.ControlDatiLegge460(areaLiquidazionePensione.DatiLegge460, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneLiquidazionePensione.StoreDatiLegge460(ref contenitore, datiPensione, ref datiFondoCommon, areaLiquidazionePensione.DatiLegge460);

            #endregion Dati Legge4/60

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        #region Dati Generici
        public AreaEsito StoreDatiGenerici(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiDL407 datiDL407 = null;
            DatiPrivilegiate datiPrivilegiate = null;
            DatiArticolo2 datiArticolo2 = null;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);
            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, datiPensione.Id, datiPensione.SiglaCategoria, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiDL407, out datiPrivilegiate, out datiArticolo2);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = contenitore.DatiAnagraficiTitolare;
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = contenitore.DatiDetrazioni;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = contenitore.ListaDatiPensioneINPDAP;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            GestionePagamento.DatiPagamento datiPagamento = contenitore.DatiPagamento;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            AreaEsito Esito = new AreaEsito();

            string messaggioControllo = string.Empty;
            if (areaLiquidazionePensione.ListaRecordFondo != null && areaLiquidazionePensione.ListaRecordFondo.Count > 0)
                areaLiquidazionePensione.ListaRecordFondo = areaLiquidazionePensione.ListaRecordFondo.OrderBy(rF => rF.DecorrenzaValiditaDati).ToList();
            DatiAssicurativi datiAssicurativi = null;
            List<RecordFondo> listaRecordFondo = null;

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneLiquidazionePensione.ControlDatiGenericiINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione, true, areaLiquidazionePensione.DatiGenericiINPDAP, areaLiquidazionePensione.DatiAssicurativiINPDAP,
                    areaLiquidazionePensione.ListaRipartizioneINPDAP, datiFondoCommon, listaDatiPensioneINPDAP, datiIstruttoriaCommon, datiExCombattente, datiBenefici,
                    datiAnagrafici, datiDetrazioni, datiEliminazione, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }


                GestioneLiquidazionePensione.StoreDatiGenericiINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione, areaLiquidazionePensione.DatiGenericiINPDAP, null, ref datiIstruttoriaCommon, ref datiFondoCommon,
                    ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione, ref datiEliminazione, ref datiPagamento, isRiaperturaDomanda, false, true);
            }
            else
            {
                GestioneLiquidazionePensione.GetDatiAssicurativi(ref contenitore, datiPensione, datiFondoCommon, isRiaperturaDomanda, out datiAssicurativi, out listaRecordFondo);

                GestioneLiquidazionePensione.ControlDatiGenerici(ref contenitore, ref contenitoreDecodifica, datiPensione, datiAnagrafici, datiIstruttoriaCommon, datiDetrazioni, areaLiquidazionePensione.DatiGenerici, datiAssicurativi, listaRecordFondo,
                    datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, datiEliminazione, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneLiquidazionePensione.StoreDatiGenerici(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiIstruttoriaCommon, ref datiFondoCommon, areaLiquidazionePensione.DatiGenerici, false,
                    datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, null, null, true, ref datiPagamento);

            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        public AreaEsito CancelDatiGenerici(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            areaLiquidazionePensione = null;
            AreaEsito Esito = new AreaEsito();
            string errore = string.Empty;
            object datiFondoXX = null;

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiDL407 datiDL407 = null;
            DatiPrivilegiate datiPrivilegiate = null;
            DatiArticolo2 datiArticolo2 = null;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);

            GetDatiDBFondi(ref contenitore, out datiFondoXX);

            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, datiPensione.Id, datiPensione.SiglaCategoria, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiDL407, out datiPrivilegiate, out datiArticolo2);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = contenitore.DatiAnagraficiTitolare;
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = contenitore.DatiDetrazioni;
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = contenitore.DatiControlloFelpe;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = contenitore.ListaDatiPensioneINPDAP;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;

            GestioneLiquidazionePensione.EliminaDatiGenerici(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiIstruttoriaCommon, ref datiFondoCommon, ref listaDatiPensioneINPDAP, ref datiEliminazione, datiDanteCausa, datiDL407,
                datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, out errore);
            if (!String.IsNullOrEmpty(errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }
            areaLiquidazionePensione = new AreaLiquidazionePensione();

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                DatiGenericiINPDAP datiGenericiINPDAP = null;
                GestioneLiquidazionePensione.GetDatiGenericiINPDAP(ref contenitore, datiPensione, datiIstruttoriaCommon, datiFondoCommon, listaDatiPensioneINPDAP != null ? listaDatiPensioneINPDAP.FirstOrDefault() : null,
                    datiControlloFelpe, datiEliminazione, out datiGenericiINPDAP);

                if (datiGenericiINPDAP != null)
                    areaLiquidazionePensione.DatiGenericiINPDAP = datiGenericiINPDAP;
            }
            else
            {
                DatiGenerici datiGenerici = null;
                GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, datiPensione, datiIstruttoriaCommon, datiFondoCommon, datiControlloFelpe, out datiGenerici);

                if (datiGenerici != null)
                    areaLiquidazionePensione.DatiGenerici = datiGenerici;
            }

            DatiLiquidazionePensioneStorico datiLiquidazionePensioneStorico = null;
            GestioneLiquidazionePensione.GetDatiLiquidazionePensioneStorico(ref contenitore, out datiLiquidazionePensioneStorico);
            if (datiLiquidazionePensioneStorico != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiLiquidazionePensioneStorico = datiLiquidazionePensioneStorico;
            }

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaLiquidazionePensione);
            GetCrossProperties(ref contenitore, ref contenitoreDecodifica, datiPensione, datiAnagrafici, datiFondoCommon, datiIstruttoriaCommon, datiFondoXX, datiMaggiorazioniBeneficiCommon, datiDetrazioni, isRiaperturaDomanda, datiDanteCausa,
                tipoFondo, datiStoricoGP, ref areaLiquidazionePensione);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }
        #endregion Dati Generici

        #region Dati Assicurativi
        public AreaEsito StoreDatiAssicurativi(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);

            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = contenitore.DatiControlloFelpe;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = contenitore.ListaDatiPensioneINPDAP;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            AreaEsito Esito = new AreaEsito();

            string messaggioControllo = string.Empty;
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneLiquidazionePensione.ControlDatiAssicurativiINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione, areaLiquidazionePensione.DatiAssicurativiINPDAP, areaLiquidazionePensione.DatiGenericiINPDAP,
                    datiMaggiorazioniBeneficiCommon, datiFondoCommon, datiIstruttoriaCommon, listaDatiPensioneINPDAP, datiControlloFelpe, datiEliminazione, true, isRiaperturaDomanda, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneLiquidazionePensione.StoreDatiAssicurativiINPDAP(ref contenitore, datiPensione, areaLiquidazionePensione.DatiAssicurativiINPDAP, areaLiquidazionePensione.ListaRipartizioneINPDAP,
                    ref datiFondoCommon, ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione, false);
            }
            else
            {
                DatiGenerici datiGenerici = null;
                GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, datiPensione, datiIstruttoriaCommon, datiFondoCommon, datiControlloFelpe, out datiGenerici);
                GestioneLiquidazionePensione.ControlDatiAssicurativi(ref contenitore, ref contenitoreDecodifica, datiPensione, datiIstruttoriaCommon, datiMaggiorazioniBeneficiCommon, datiFondoCommon, datiGenerici,
                    areaLiquidazionePensione.DatiAssicurativi, areaLiquidazionePensione.ListaRecordFondo, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    datiQuadroLiquidazionePensione = null;
                    GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);
                    if (datiQuadroLiquidazionePensione.TabDatiAssicurativi == 0)
                        GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS));

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                try
                {
                    GestioneLiquidazionePensione.StoreDatiAssicurativi(ref contenitore, datiPensione, ref datiFondoCommon, areaLiquidazionePensione.DatiAssicurativi, areaLiquidazionePensione.ListaRecordFondo, false);
                }
                catch (Exception)
                {
                    datiQuadroLiquidazionePensione = null;
                    GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);
                    if (datiQuadroLiquidazionePensione.TabDatiAssicurativi == 0)
                        GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS));
                    throw;
                }
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        public AreaEsito CancelDatiAssicurativi(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object datiFondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);
            GetDatiDBFondi(ref contenitore, out datiFondoXX);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = contenitore.DatiAnagraficiTitolare;
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = contenitore.DatiDetrazioni;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            List<DatiRecordNoCalcolo> listaDatiRecordNoCalcolo = null;
            GestioneAreaNoCalcolo.GetRecordNoCalcolo(datiPensione, out listaDatiRecordNoCalcolo);

            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = contenitore.ListaDatiPensioneINPDAP;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;
            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            areaLiquidazionePensione = null;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneLiquidazionePensione.ControlDatiAssicurativiINPDAPForCancel(out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneLiquidazionePensione.EliminaDatiAssicurativiINPDAP(ref contenitore, datiPensione, ref datiFondoCommon, ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
            }
            else
            {
                GestioneLiquidazionePensione.ControlDatiAssicurativiForCancel(ref contenitore, datiPensione, listaDatiRecordNoCalcolo, datiDanteCausa, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneLiquidazionePensione.EliminaDatiAssicurativi(ref contenitore, datiPensione, ref datiFondoCommon, out messaggioControllo);
                if (!string.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
            }

            areaLiquidazionePensione = new AreaLiquidazionePensione();
            GetDatiAssicurativi(ref contenitore, datiPensione, datiFondoCommon, listaDatiPensioneINPDAP != null ? listaDatiPensioneINPDAP.FirstOrDefault() : null, isRiaperturaDomanda, ref areaLiquidazionePensione);

            GetListeDecodifica(ref contenitore, ref contenitoreDecodifica, ref areaLiquidazionePensione);
            GetCrossProperties(ref contenitore, ref contenitoreDecodifica, datiPensione, datiAnagrafici, datiFondoCommon, datiIstruttoriaCommon, datiFondoXX, datiMaggiorazioniBeneficiCommon, datiDetrazioni, isRiaperturaDomanda, datiDanteCausa,
                tipoFondo, datiStoricoGP, ref areaLiquidazionePensione);

            GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS));

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        private void GetDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP,
            bool isRiaperturaDomanda, ref AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            DatiAssicurativi datiAssicurativi = null;
            DatiAssicurativiINPDAP datiAssicurativiINPDAP = null;
            List<RipartizioneINPDAP> listaRipartizioneINPDAP = null;
            List<RecordFondo> recordFondo = null;

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                GestioneLiquidazionePensione.GetDatiAssicurativiINPDAP(ref contenitore, datiPensione, datiFondo, datiPensioneINPDAP, out datiAssicurativiINPDAP, out listaRipartizioneINPDAP);
            else
                GestioneLiquidazionePensione.GetDatiAssicurativi(ref contenitore, datiPensione, datiFondo, isRiaperturaDomanda, out datiAssicurativi, out recordFondo);
            if (datiAssicurativi != null || (recordFondo != null && recordFondo.Count > 0) || datiAssicurativiINPDAP != null || (listaRipartizioneINPDAP != null && listaRipartizioneINPDAP.Count > 0))
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();

                areaLiquidazionePensione.DatiAssicurativi = datiAssicurativi;
                areaLiquidazionePensione.DatiAssicurativiINPDAP = datiAssicurativiINPDAP;
                areaLiquidazionePensione.ListaRecordFondo = recordFondo;
                areaLiquidazionePensione.ListaRipartizioneINPDAP = listaRipartizioneINPDAP;
            }
        }
        #endregion Dati Assicurativi

        #region Dati PrecedentePensione
        public AreaEsito StoreDatiPrecedentePensione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, false, true);

            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = contenitore.DatiControlloFelpe;
            AreaEsito Esito = new AreaEsito();

            string messaggioControllo = "";
            DatiGenerici datiGenerici = null;
            GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, datiPensione, datiIstruttoriaCommon, datiFondoCommon, datiControlloFelpe, out datiGenerici);
            GestioneLiquidazionePensione.ControlDatiPrecedentePensione(datiGenerici, areaLiquidazionePensione.DatiPrecedentePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneLiquidazionePensione.StoreDatiPrecedentePensione(ref contenitore, datiPensione, ref datiIstruttoriaCommon, areaLiquidazionePensione.DatiPrecedentePensione);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        public AreaEsito CancelDatiPrecedentePensione(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, false, false);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiPrecedentePensione(ref contenitore, datiPensione, datiIstruttoriaCommon);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        #endregion Dati PrecedentePensione

        #region DatiBititolaritaInail

        public AreaEsito StoreDatiBititolaritaInail(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneLiquidazionePensione.ControlDatiBititolaritaInail(areaLiquidazionePensione.DatiBititolaritaInail, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneLiquidazionePensione.StoreDatiBititolaritaInail(ref contenitore, contenitore.DatiPensione, areaLiquidazionePensione.DatiBititolaritaInail);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito CancelDatiBititolaritaInail(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiBititolaritaInail(ref contenitore);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiBititolaritaInail

        #region Dati Legge 4/60

        public AreaEsito StoreDatiLegge460(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneLiquidazionePensione.ControlDatiLegge460(areaLiquidazionePensione.DatiLegge460, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            GestioneLiquidazionePensione.StoreDatiLegge460(ref contenitore, datiPensione, ref datiFondoCommon, areaLiquidazionePensione.DatiLegge460);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito CancelDatiLegge460(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiLegge460(ref contenitore, datiPensione, datiFondoCommon);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion Dati Legge 4/60

        #region Dati Istruttoria

        public AreaEsito StoreDatiIstruttoria(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, false, true);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneLiquidazionePensione.ControlDatiIstruttoriaINPDAP(areaLiquidazionePensione.DatiIstruttoriaINPDAP, datiPensione, isRiaperturaDomanda, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneLiquidazionePensione.StoreDatiIstruttoriaINPDAP(datiPensione, ref datiFondoCommon, ref datiIstruttoriaCommon, ref datiQuadroLiquidazionePensione,
                    areaLiquidazionePensione.DatiIstruttoriaINPDAP);
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito CancelDatiIstruttoria(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            areaLiquidazionePensione = new AreaLiquidazionePensione();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, false, true);

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            AreaEsito Esito = new AreaEsito();

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneLiquidazionePensione.EliminaDatiIstruttoriaINPDAP(datiPensione, ref datiFondoCommon, ref datiIstruttoriaCommon, ref datiQuadroLiquidazionePensione);

                DatiIstruttoriaINPDAP datiIstruttoriaINPDAP = null;
                GestioneLiquidazionePensione.GetDatiIsruttoriaINPDAPByIdPensione(datiFondoCommon, datiIstruttoriaCommon, out datiIstruttoriaINPDAP);

                areaLiquidazionePensione.DatiIstruttoriaINPDAP = datiIstruttoriaINPDAP;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }


        #endregion Dati Istruttoria

        #region Get Dati DB Common & Liste Decodifica & Cross Properties

        private void GetListeDecodifica(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, ref AreaLiquidazionePensione areaLiquidazionePensione)
        {
            List<DatiAttivitaSvolta> attivitaSvolte = null;
            GestioneLiquidazionePensione.GetAttivitaSvolte(ref contenitoreDecodifica, contenitore.DatiPensione, out attivitaSvolte);
            if (attivitaSvolte != null && attivitaSvolte.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaAttivitaSvolte = attivitaSvolte;
            }

            List<CodiceRequisito1> codiceRequisito1 = null;
            GestioneLiquidazionePensione.GetListaCodiceRequisito1(ref contenitoreDecodifica, out codiceRequisito1);
            if (codiceRequisito1 != null && codiceRequisito1.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceRequisito1 = codiceRequisito1;
            }

            List<CodiceRequisito2> codiceRequisito2 = null;
            GestioneLiquidazionePensione.GetListaCodiceRequisito2(ref contenitoreDecodifica, out codiceRequisito2);
            if (codiceRequisito2 != null && codiceRequisito2.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceRequisito2 = codiceRequisito2;
            }

            List<CodiceSpecifico> codiceSpecifico = null;
            GestioneLiquidazionePensione.GetListaCodiceSpecifico(ref contenitore, ref contenitoreDecodifica, contenitore.DatiPensione, out codiceSpecifico);
            if (codiceSpecifico != null && codiceSpecifico.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceSpecifico = codiceSpecifico;
            }

            List<CodiceConvenzioneInternazionale> codiceConvenzioneInternazionale = null;
            GestioneLiquidazionePensione.GetListaCodiceConvenzioneInternazionale(ref contenitoreDecodifica, out codiceConvenzioneInternazionale);
            if (codiceConvenzioneInternazionale != null && codiceConvenzioneInternazionale.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceConvenzioneInternazionale = codiceConvenzioneInternazionale;
            }

            List<CodiceArt22> lCodiceArt22 = null;
            GestioneLiquidazionePensione.GetListaCodiceArt22(ref contenitoreDecodifica, out lCodiceArt22);
            if (lCodiceArt22 != null && lCodiceArt22.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceArt22 = lCodiceArt22;
            }

            List<CodiceCapitalizzazione> lCodiceCapitalizzazione = null;
            GestioneLiquidazionePensione.GetListaCodiceCapitalizzazione(ref contenitoreDecodifica, out lCodiceCapitalizzazione);
            if (lCodiceCapitalizzazione != null && lCodiceCapitalizzazione.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceCapitalizzazione = lCodiceCapitalizzazione;
            }

            List<CodiceEsodo> lCodiceEsodo = null;
            GestioneLiquidazionePensione.GetListaCodiceEsodo(ref contenitoreDecodifica, out lCodiceEsodo);
            if (lCodiceEsodo != null && lCodiceEsodo.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceEsodo = lCodiceEsodo;
            }

            List<CodicePartTime> lCodicePartTime = null;
            GestioneLiquidazionePensione.GetListaCodicePartTime(ref contenitoreDecodifica, out lCodicePartTime);
            if (lCodicePartTime != null && lCodicePartTime.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodicePartTime = lCodicePartTime;
            }

            List<CausaCessazione> lCausaCessazione = null;
            GestioneLiquidazionePensione.GetListaCausaCessazione(ref contenitoreDecodifica, contenitore.DatiPensione, out lCausaCessazione);
            if (lCausaCessazione != null && lCausaCessazione.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCausaCessazione = lCausaCessazione;
            }

            List<TipoCalcolo> lTipoCalcolo = null;
            GestioneLiquidazionePensione.GetListaTipoCalcolo(ref contenitoreDecodifica, contenitore.DatiPensione, out lTipoCalcolo);
            if (lTipoCalcolo != null && lTipoCalcolo.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaTipoCalcolo = lTipoCalcolo;
            }

            List<CodiceEliminazione> lCodiceEliminazione = null;
            GestioneLiquidazionePensione.GetListaCodiceEliminazione(ref contenitoreDecodifica, contenitore.DatiPensione, contenitore.DatiEliminazione, out lCodiceEliminazione);
            if (lCodiceEliminazione != null && lCodiceEliminazione.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceEliminazione = lCodiceEliminazione;
            }

            List<CodiceParticolare> listaCodiceParticolare = null;
            GestioneLiquidazionePensione.GetListaCodiceParticolare(ref contenitoreDecodifica, contenitore.DatiPensione, out listaCodiceParticolare);
            if (listaCodiceParticolare != null && listaCodiceParticolare.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceParticolare = listaCodiceParticolare;
            }

            List<TipoLiquidazionePM> listaTipoLiquidazionePM = null;
            GestioneLiquidazionePensione.GetListaTipoLiquidazionePM(ref contenitoreDecodifica, out listaTipoLiquidazionePM);
            if (listaTipoLiquidazionePM != null && listaTipoLiquidazionePM.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaTipoLiquidazionePM = listaTipoLiquidazionePM;
            }

            List<CodiceTipoLiquidazionePM> listaCodiceTipoLiquidazionePM = null;
            GestioneLiquidazionePensione.GetListaCodiceTipoLiquidazionePM(ref contenitoreDecodifica, out listaCodiceTipoLiquidazionePM);
            if (listaCodiceTipoLiquidazionePM != null && listaCodiceTipoLiquidazionePM.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceTipoLiquidazionePM = listaCodiceTipoLiquidazionePM;
            }

            List<CodiceLegge413> listaCodiceLegge413 = null;
            GestioneLiquidazionePensione.GetListaCodiceLegge413(ref contenitoreDecodifica, out listaCodiceLegge413);
            if (listaCodiceLegge413 != null && listaCodiceLegge413.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiceLegge413 = listaCodiceLegge413;
            }

            List<AttivitaSvolta2> listaAttivitaSvolta2 = null;
            GestioneLiquidazionePensione.GetListaAttivitaSvolta2(ref contenitoreDecodifica, out listaAttivitaSvolta2);
            if (listaAttivitaSvolta2 != null && listaAttivitaSvolta2.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaAttivitaSvolta2 = listaAttivitaSvolta2;
            }

            List<TipoLiquidazione> listaTipoLiquidazione = null;
            GestioneLiquidazionePensione.GetListaTipoLiquidazione(ref contenitoreDecodifica, out listaTipoLiquidazione);
            if (listaTipoLiquidazione != null && listaTipoLiquidazione.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaTipoLiquidazione = listaTipoLiquidazione;
            }

            List<CodiciNatura> listaCodiciNatura = null;
            GestioneLiquidazionePensione.GetListaCodiciNatura(ref contenitoreDecodifica, contenitore.DatiPensione, out listaCodiciNatura);
            if (listaCodiciNatura != null && listaCodiciNatura.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaCodiciNatura = listaCodiciNatura;
            }

            // DPR Armonizzazione
            List<PersonaleViaggiante> listaPersonaleViaggiante = null;
            GestioneLiquidazionePensione.GetListaPersonaleViaggiante(ref contenitoreDecodifica, out listaPersonaleViaggiante);
            if (listaPersonaleViaggiante != null && listaPersonaleViaggiante.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.ListaPersonaleViaggiante = listaPersonaleViaggiante;
            }
            // ----------------------

            if (Utility.IsDomandaINPDAP(contenitore.DatiPensione.Gestione))
            {
                List<DecodificaEnteRipartizioneINPDAP> listaEnteRipartizioneINPDAP = null;
                GestioneLiquidazionePensione.GetListaDecEnteRipartizioneINPDAP(ref contenitoreDecodifica, out listaEnteRipartizioneINPDAP);
                if (listaEnteRipartizioneINPDAP != null && listaEnteRipartizioneINPDAP.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaDecEnteRipartizioneINPDAP = listaEnteRipartizioneINPDAP;
                }

                List<MicroqualificaINPDAP> listaMicroqualificaINPDAP = null;
                GestioneLiquidazionePensione.GetListaDecMicroqualificaINPDAP(ref contenitoreDecodifica, areaLiquidazionePensione.DatiAssicurativiINPDAP != null ? areaLiquidazionePensione.DatiAssicurativiINPDAP.Microqualifica : null, out listaMicroqualificaINPDAP);
                if (listaMicroqualificaINPDAP != null && listaMicroqualificaINPDAP.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaMicroqualificaINPDAP = listaMicroqualificaINPDAP;
                }

                List<CtrlCompartoSettoreRuolo> listaCtrlCompartoSettoreRuolo = null;
                GestioneLiquidazionePensione.GetListaCtrlCompartoSettoreRuolo(contenitore.DatiPensione.SiglaCategoria, out listaCtrlCompartoSettoreRuolo);
                if (listaCtrlCompartoSettoreRuolo != null && listaCtrlCompartoSettoreRuolo.Count > 0)
                {
                    if (areaLiquidazionePensione == null)
                        areaLiquidazionePensione = new AreaLiquidazionePensione();
                    areaLiquidazionePensione.ListaCtrlCompartoSettoreRuolo = listaCtrlCompartoSettoreRuolo.OrderBy(x => x.CodiceComparto).ThenBy(x => x.CodiceSettore).ThenBy(x => x.CodiceRuolo).ToList();
                }
            }
        }

        private void GetCrossProperties(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, GestioneFondo.DatiFondo datiFondo,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, object datiFondoXX, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, bool isRiaperturaDomanda, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoFondo? tipoFondo, GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP,
            ref AreaLiquidazionePensione areaLiquidazionePensione)
        {
            Utility.CategoriaFondoPI? CategoriaFondoPI = null;
            GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia = null;
            DateTime? DecorrenzaPensioneDirettaDC = null;
            Dictionary<string, char?> TipoPensione = null;
            DateTime? DataPrelievoDomanda = null;
            char? TipoReversibilita = null;
            Dictionary<string, bool?> lCrossProperties = GestioneLiquidazionePensione.GetCrossProperties(ref contenitore, ref contenitoreDecodifica, datiPensione, datiAnagrafici, datiFondo, datiIstruttoria, datiFondoXX, datiMaggiorazioniBenefici,
                datiDetrazioni, isRiaperturaDomanda, datiDanteCausa, tipoFondo, datiStoricoGP, out CategoriaFondoPI, out TipologiaSalvaguardia, out DecorrenzaPensioneDirettaDC, out TipoPensione, out DataPrelievoDomanda, out TipoReversibilita);

            if (areaLiquidazionePensione == null)
                areaLiquidazionePensione = new AreaLiquidazionePensione();

            areaLiquidazionePensione.IsEsenzioneFiscaleEstero = lCrossProperties["IsEsenzioneFiscaleEstero"];
            areaLiquidazionePensione.IsResidenteEstero = lCrossProperties["IsResidenteEstero"];
            areaLiquidazionePensione.IsEsenzioneFiscaleVittima = lCrossProperties["IsEsenzioneFiscaleVittima"];
            areaLiquidazionePensione.IsRequisitiL247_L243Enable = lCrossProperties["RequisitiL247_L243Enable"];
            areaLiquidazionePensione.IsCodiceSpecificoVisible = lCrossProperties["CodiceSpecificoVisible"];
            areaLiquidazionePensione.CategoriaFondoPI = CategoriaFondoPI;
            areaLiquidazionePensione.IsVisibleArt2 = lCrossProperties["Articolo2"];
            areaLiquidazionePensione.IsDecPensAnteAgosto95 = lCrossProperties["DecPensAnteAgosto95"];
            areaLiquidazionePensione.TipologiaSalvaguardia = TipologiaSalvaguardia;
            areaLiquidazionePensione.IsCodiceNatura2Enabled = lCrossProperties["IsCodiceNatura2Enabled"];
            areaLiquidazionePensione.IsUsuranti = lCrossProperties["Usuranti"];
            areaLiquidazionePensione.IsVecchPerditaTitolo = lCrossProperties["VecchPerditaTitolo"];
            areaLiquidazionePensione.IsCodiceSpecificoEnabled = lCrossProperties["CodiceSpecificoEnabled"];
            areaLiquidazionePensione.IsCodiceArt22Enabled = lCrossProperties["CodiceArt22Enabled"];
            areaLiquidazionePensione.IsDomandaTrasformazioneAOI = lCrossProperties["DomandaTrasformazioneAOI"];
            areaLiquidazionePensione.DecorrenzaPensioneDirettaDC = DecorrenzaPensioneDirettaDC;
            areaLiquidazionePensione.IsCodDirittoQuoteFisseVisible = lCrossProperties["IsCodDirittoQuoteFisseVisible"];
            areaLiquidazionePensione.IsIndennitaAggiuntivaVisible = lCrossProperties["IsIndennitaAggiuntivaVisible"];
            areaLiquidazionePensione.TipoPensione = TipoPensione;
            areaLiquidazionePensione.IsDecorrenzaSuccSett1989 = lCrossProperties["IsDecorrenzaSuccSett1989"];
            areaLiquidazionePensione.IsCodiceComunicazione3Visible = lCrossProperties["IsCodiceComunicazione3Visible"];
            areaLiquidazionePensione.IsProvvisoriaVisible = lCrossProperties["IsProvvisoriaVisible"];
            areaLiquidazionePensione.IsCodiceNatura2DisabledPerSperDonna = lCrossProperties["IsCodiceNatura2DisabledPerSperDonna"];
            areaLiquidazionePensione.IsDomandaConNuovaGestioneDatiFondoFSPT = lCrossProperties["IsDomandaConNuovaGestioneDatiFondoFSPT"];
            // DPR Armonizzazione
            areaLiquidazionePensione.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante = lCrossProperties["IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante"];
            areaLiquidazionePensione.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL = lCrossProperties["IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL"];
            areaLiquidazionePensione.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante = lCrossProperties["IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante"];
            areaLiquidazionePensione.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL = lCrossProperties["IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL"];
            //-----------------------
            areaLiquidazionePensione.IsDomandaAnteArmonizzazione = lCrossProperties["IsDomandaAnteArmonizzazione"];
            areaLiquidazionePensione.IsCapitalizzazioneVisible = lCrossProperties["IsCapitalizzazioneVisible"];
            areaLiquidazionePensione.IsTrimestreAnzianitaRequisitiNoInvaliditaVisible = lCrossProperties["IsTrimestreAnzianitaRequisitiNoInvaliditaVisible"];
            areaLiquidazionePensione.IsBeneficioArt24Comma15BisFromFELPE = lCrossProperties["IsBeneficioArt24Comma15BisFromFELPE"];
            // ------------------
            areaLiquidazionePensione.IsPensioneTipoContributivo = lCrossProperties["IsPensioneTipoContributivo"];
            areaLiquidazionePensione.IsPensioneTipoContributivoConOpzione = lCrossProperties["IsPensioneTipoContributivoConOpzione"];
            areaLiquidazionePensione.IsSperimentaleDonna = lCrossProperties["IsSperimentaleDonna"];
            areaLiquidazionePensione.IsRiduzioneRetribVisible = lCrossProperties["IsRiduzioneRetributiva"];
            areaLiquidazionePensione.IsRiduzioneRetributivaEnabled = lCrossProperties["IsRiduzioneRetributivaEnabled"];
            areaLiquidazionePensione.IsBeneficioApePrecociFromFELPE = lCrossProperties["IsBeneficioApePrecociFromFELPE"];
            areaLiquidazionePensione.IsEsenzioneFiscaleEsteroFromDetrazioni = lCrossProperties["IsEsenzioneFiscaleEsteroFromDetrazioni"];
            areaLiquidazionePensione.IsReversibilitaOrRicostituzione = lCrossProperties["IsReversibilitaOrRicostituzione"];
            areaLiquidazionePensione.IsRicostituzioneForMemo72 = lCrossProperties["IsRicostituzioneForMemo72"];
            areaLiquidazionePensione.IsRichiestaBonusBookingAbilitata = lCrossProperties["IsRichiestaBonusBookingAbilitata"];
            areaLiquidazionePensione.IsPrimoVersamentoNonObbligatorio = lCrossProperties["IsPrimoVersamentoNonObbligatorio"];
            areaLiquidazionePensione.IsBeneficioNonVedente = lCrossProperties["IsBeneficioNonVedente"];
            areaLiquidazionePensione.IsDataRinunciaTrattenutaInpdapStorico = lCrossProperties["IsDataRinunciaTrattenutaInpdapStorico"];
            areaLiquidazionePensione.IsBeneficioNonVedenteFromStorico = lCrossProperties["IsBeneficioNonVedenteFromStorico"];
            areaLiquidazionePensione.IsRichiestaBonus154Abilitata = lCrossProperties["IsRichiestaBonus154Abilitata"];
            areaLiquidazionePensione.IsCodComunicazioniEsenzioneFiscaleVittimaVisibile = lCrossProperties["IsCodComunicazioniEsenzioneFiscaleVittimaVisibile"];
            areaLiquidazionePensione.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = lCrossProperties["IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione"];
            areaLiquidazionePensione.isSenzaLegge33670 = lCrossProperties["isSenzaLegge33670"];
            //ENG - Aggiornamento Memo86
            areaLiquidazionePensione.IsPresenteTrattenutaFondoCreditoDaPrelievo = lCrossProperties["IsPresenteTrattenutaFondoCreditoDaPrelievo"];
            areaLiquidazionePensione.DataPrelievoDomanda = DataPrelievoDomanda;
            areaLiquidazionePensione.TipoReversibilita = TipoReversibilita;

            areaLiquidazionePensione.IsMiglioramentiContrattualiAutomatici = lCrossProperties["IsMiglioramentiContrattualiAutomatici"];
        }

        private void GetDatiDBCommon(ref EntityBLCommon.ContenitoreObject contenitore, out GestionePensione.DatiPensione datiPensione, out GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            out Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, out GestioneFondo.DatiFondo datiFondoCommon,
            bool datiIstruttoriaRequired, bool datiMaggBenRequired, bool datiFondoRequired)
        {
            datiPensione = null;
            datiIstruttoriaCommon = null;
            datiMaggiorazioniBeneficiCommon = null;
            datiFondoCommon = null;

            if (contenitore.DatiPensione == null)
                return;

            datiPensione = contenitore.DatiPensione;

            if (datiIstruttoriaRequired)
            {
                datiIstruttoriaCommon = contenitore.DatiIstruttoria;
                //if (datiIstruttoriaCommon == null)
                //    datiIstruttoriaCommon = new GestioneIstruttoria.DatiIstruttoria();
            }

            if (datiMaggBenRequired)
            {
                datiMaggiorazioniBeneficiCommon = contenitore.DatiMaggiorazioniBenefici;
                //if (datiMaggiorazioniBeneficiCommon == null)
                //    datiMaggiorazioniBeneficiCommon = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
            }

            if (datiFondoRequired)
            {
                datiFondoCommon = contenitore.DatiFondo;
                //if (datiFondoCommon == null)
                //    datiFondoCommon = new GestioneFondo.DatiFondo();
            }
        }

        private void ValorizzaDatiForMaggiorazioniBenefici(ref EntityBLCommon.ContenitoreObject contenitore, long idPensione, string siglaCategoria, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon,
            out DatiExCombattente datiExCombattente, out DatiBenefici datiBenefici, out DatiDL407 datiDL407, out DatiPrivilegiate datiPrivilegiate, out DatiArticolo2 datiArticolo2)
        {
            datiExCombattente = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiExCombattente(datiMaggiorazioniBeneficiCommon, out datiExCombattente);

            datiBenefici = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiByIdPensione(ref contenitore, datiMaggiorazioniBeneficiCommon, out datiBenefici);

            datiDL407 = null;
            GestioneMaggiorazioniBenefici.GetDatiDL407ByIdPensione(ref contenitore, out datiDL407);

            datiPrivilegiate = null;
            GestioneMaggiorazioniBenefici.GetDatiPrivilegiateByIdPensione(ref contenitore, siglaCategoria, out datiPrivilegiate);

            datiArticolo2 = null;
            GestioneMaggiorazioniBenefici.GetDatiArticolo2ByIdPensione(ref contenitore, out datiArticolo2);
        }

        #endregion Get Dati DB Common & Liste Decodifica & Cross Properties

        #endregion LiquidazionePensione

        #region AreaDatiContributivi

        public AreaEsito GetDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            return GetDatiContributiviByDatiPensionePrivate(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, contenitore.IdFondoPensione,out areaDatiContributivi);
        }

        public AreaEsito StoreDatiContributiviByDomanda(Int64 numeroDomanda, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            object fondoXX = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            DateTime dataSistema = Utility.DataSistemaFs;

            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            AreaEsito Esito = new AreaEsito();
            //string messaggioVideo = "";

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            #region Dati Fondo
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.GAS:
                        Esito = StoreDatiFondoByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi);
                        if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            return Esito;
                        break;


                }
            }
            #endregion Dati Fondo
            #region Dati Calcolo

            Esito = StoreDatiCalcoloByDatiPensionePrivate(datiPensione, datiDanteCausa, datiAnagraficiTitolare, codiceSpecificoTraduzioneSuGP, isRiaperturaDomanda, ref datiMaggiorazioniBeneficiCommon,
                ref datiFondoCommon, ref fondoXX, areaDatiContributivi, dataSistema, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Dati Calcolo
            #region Dati Calcolo 707

            Esito = StoreDatiCalcolo707ByDatiPensionePrivate(datiPensione, datiDanteCausa, ref datiFondoCommon, ref fondoXX, areaDatiContributivi);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Dati Calcolo 707

            #region Art 11 e 14
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.GAS:
                        Esito = StoreDatiArt14e11ByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, false);
                        if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            return Esito;
                        break;
                }
            }
            #endregion Art 11 e 14
            #region Ante67
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.ES:
                        Esito = StoreAnte67ByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, false);
                        if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            return Esito;
                        break;


                }
            }
            #endregion Ante67
            #region SL 336
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.ES:
                        Esito = StoreSL336ByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, false);
                        if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            return Esito;
                        break;
                }
            }
            #endregion SL 336
            #region Altra Pensione - Dati AGO
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.ET:
                        if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC) &&
                            Utility.IsVisibleTabAltraPensioneDatiAgo(datiPensione, datiDanteCausa, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione))
                        {
                            Esito = StoreAltraPensioneDatiAgoPrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, true);
                            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                                return Esito;
                        }
                        break;
                }
            }
            #endregion Altra Pensione - Dati AGO

            #region dati ago fondo PI
            if (tipoFondo.HasValue && tipoFondo == Utility.TipoFondo.PI)
            {
                Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                if (categoriaFondoPI.HasValue &&
                    (categoriaFondoPI.Value == Utility.CategoriaFondoPI.A || categoriaFondoPI.Value == Utility.CategoriaFondoPI.B)
                    && areaDatiContributivi.ElencoDatiAgo != null && areaDatiContributivi.ElencoDatiAgo.Count > 0)
                {
                    foreach (var datiago in areaDatiContributivi.ElencoDatiAgo)
                    {
                        AreaDatiAgoFondoPI datiAgoFondoPI = new AreaDatiAgoFondoPI();
                        GetDatiAgoFondoPIById(datiago.Id, out datiAgoFondoPI);

                        //futuri controlli 

                        StoreDatiAgoFondoPIByIdPrivate(datiAgoFondoPI);
                    }

                }

            }
            #endregion

            GetCrossProperties(datiPensione, isRiaperturaDomanda, datiFondoCommon, fondoXX, ref areaDatiContributivi, areaDatiContributivi.DatiCalcolo, datiDanteCausa, codiceSpecificoTraduzioneSuGP, tipoFondo);
            GetListeDecodifica(codiceSpecificoTraduzioneSuGP, ref areaDatiContributivi);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        #region Dati Calcolo

        public AreaEsito StoreDatiCalcoloByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            object fondoXX = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            DateTime dataSistema = Utility.DataSistemaFs;

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            AreaEsito Esito = StoreDatiCalcoloByDatiPensionePrivate(datiPensione, datiDanteCausa, datiAnagraficiTitolare, codiceSpecificoTraduzioneSuGP, isRiaperturaDomanda,
                ref datiMaggiorazioniBeneficiCommon, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, dataSistema, true);
            return Esito;
        }

        public AreaEsito CancelDatiCalcoloByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            areaDatiContributivi = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiCalcoloByDatiPensione(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }

            GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS));

            return GetDatiContributiviByDatiPensioneForCancel(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, out areaDatiContributivi);

        }

        private AreaEsito StoreDatiCalcoloByDatiPensionePrivate(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, char? codiceSpecificoTraduzioneSuGP, bool isRiaperturaDomanda,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX,
            AreaDatiContributivi areaDatiContributivi, DateTime dataSistema, bool isSingleTab)
        {
            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();
            GestioneContrib.DatiCalcolo datiCalcoloBL = null;
            datiCalcoloBL = areaDatiContributivi.DatiCalcolo;
            try
            {
                GestioneContrib.StoreDatiCalcoloByDomandaFelpe(datiPensione, datiCalcoloBL, datiDanteCausa, datiAnagraficiTitolare, codiceSpecificoTraduzioneSuGP, isRiaperturaDomanda,
                    ref datiMaggiorazioniBenefici, ref datiFondo, ref fondoXX, areaDatiContributivi.DatiArt11e14, dataSistema, isSingleTab, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
                    GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);
                    if (datiQuadroDatiContributivi.TabDatiCalcolo == 0 && datiQuadroDatiContributivi.TabDatiAgo == 0)
                        GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS));

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
            }
            catch (Exception ex)
            {
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);
                if (datiQuadroDatiContributivi.TabDatiCalcolo == 0 && datiQuadroDatiContributivi.TabDatiAgo == 0)
                    GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS));
                throw;
            }

            areaDatiContributivi.DatiCalcolo = datiCalcoloBL;
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito StoreDatiCalcolo707ByDatiPensionePrivate(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, AreaDatiContributivi areaDatiContributivi)
        {
            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();
            DatiCalcolo707 datiCalcolo707BL = null;
            datiCalcolo707BL = areaDatiContributivi.DatiCalcolo707;
            try
            {
                GestioneContrib.StoreDatiCalcolo707ByDomandaFelpe(datiPensione, datiCalcolo707BL, datiDanteCausa, ref datiFondo, ref fondoXX, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
            }
            catch (Exception)
            {
                throw;
            }

            areaDatiContributivi.DatiCalcolo707 = datiCalcolo707BL;
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito GetDatiContributiviByDatiPensionePrivate(GestionePensione.DatiPensione datiPensione,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, GestioneFondo.DatiFondo datiFondoCommon, object datiFondoXX,
            bool isRiaperturaDomanda, long? idFondo, out AreaDatiContributivi areaDatiContributivi)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = "";
            object datiFelpe = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            areaDatiContributivi = new AreaDatiContributivi();
            GestioneContrib.DatiCalcolo datiCalcoloBL = null; //dati ago
            DatiCalcolo707 datiCalcolo707BL = null;
            GestioneContrib.DatiCalcolo datiCalcoloStorico = null;
            GestioneContrib.EntityDatiFondo entityDatiFondoGAS = null;
            GestioneContrib.DatiArt11e14 entityDatiArt11e14 = null;
            GestioneContrib.DatiAnte67 entityDatiAnte67 = null;
            GestioneContrib.DatiSL33670 entitySL33670 = null;
            GestioneContrib.DatiAgoAltraPensione entityDatiAgoAltraPensione = null;
            List<GestioneFondo.PretabellaDatiAgoFondoPI> elencoDatiAgoPi = null;
            List<GestioneFondo.PretabellaPensioneFondoPI> elencoDatiPensioneFondoPi = null;

            GestioneContrib.GetDatiCalcoloByDomandaFelpe(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, isRiaperturaDomanda, out datiCalcoloBL, out datiFelpe, out messaggioVideo);

            GestioneContrib.GetDatiCalcolo707ByDomandaFelpe(datiPensione, datiFelpe, false, out datiCalcolo707BL, out messaggioVideo);

            GestioneContrib.GetDatiFondoAndDatiArt14e11(datiPensione, out entityDatiFondoGAS, out entityDatiArt11e14);

            GestioneContrib.GetDatiAnte67AndSL336(datiPensione, out entityDatiAnte67, out entitySL33670);

            GestioneContrib.GetAltraPensioneDatiAGO_ET(datiPensione, datiFondoXX, out entityDatiAgoAltraPensione);

            GestioneContrib.GetStoricoGP(datiPensione, out datiCalcoloStorico);

            GestioneContrib.GetElencoDatiAgoPi(datiPensione, out elencoDatiAgoPi);
            GestioneContrib.GetElencoDatiFondoPi(datiPensione, out elencoDatiPensioneFondoPi);

            //set entity
            areaDatiContributivi.DatiCalcolo = datiCalcoloBL;
            areaDatiContributivi.DatiCalcolo707 = datiCalcolo707BL;
            areaDatiContributivi.DatiCalcoloStorico = datiCalcoloStorico;
            areaDatiContributivi.DatiFondo = entityDatiFondoGAS;
            areaDatiContributivi.DatiArt11e14 = entityDatiArt11e14;
            areaDatiContributivi.DatiAnte67 = entityDatiAnte67;
            areaDatiContributivi.DatiSL336 = entitySL33670;
            areaDatiContributivi.DatiAgoAltraPensione = entityDatiAgoAltraPensione;
            areaDatiContributivi.ElencoDatiAgo = elencoDatiAgoPi;
            areaDatiContributivi.ElencoDatiPensioneFondoPI = elencoDatiPensioneFondoPi;
            areaDatiContributivi.IdFondo = idFondo;

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            //areaDatiContributivi.DatiCalcolo.fondoPI.ElencoDatiAgo

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            GetCrossProperties(datiPensione, isRiaperturaDomanda, datiFondoCommon, datiFondoXX, ref areaDatiContributivi, datiCalcoloBL, datiDanteCausa, codiceSpecificoTraduzioneSuGP, tipoFondo);

            GetListeDecodifica(codiceSpecificoTraduzioneSuGP, ref areaDatiContributivi);

            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito GetDatiContributiviByDatiPensioneForCancel(GestionePensione.DatiPensione datiPensione,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, GestioneFondo.DatiFondo datiFondoCommon, object datiFondoXX,
            bool isRiaperturaDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            AreaEsito Esito = new AreaEsito();
            areaDatiContributivi = null;
            long? idFondo = null;
            try
            {
                Esito = GetDatiContributiviByDatiPensionePrivate(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, datiFondoXX, isRiaperturaDomanda, idFondo, out areaDatiContributivi);
                if (!String.IsNullOrEmpty(Esito.Messaggio))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    return Esito;
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }
            return Esito;
        }

        public AreaEsito StoreDatiFondoByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            AreaEsito Esito = StoreDatiFondoByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi);
            return Esito;
        }

        public AreaEsito CancelDatiFondoByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            areaDatiContributivi = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiFondoByDatiPensione(datiPensione, ref datiFondoCommon, ref fondoXX);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }

            return GetDatiContributiviByDatiPensioneForCancel(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, out areaDatiContributivi);

        }

        private AreaEsito StoreDatiFondoByDatiPensionePrivate(GestionePensione.DatiPensione datiPensione,
            ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, AreaDatiContributivi areaDatiContributivi)
        {
            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();

            GestioneContrib.EntityDatiFondo datiFondoGAS = areaDatiContributivi.DatiFondo;

            GestioneContrib.ControlsDatiFondo(datiFondoGAS, datiPensione, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            GestioneContrib.StoreDatiFondo(datiPensione, datiFondoGAS, ref datiFondo, ref fondoXX, out messaggioVideo);
            areaDatiContributivi.DatiFondo = datiFondoGAS;
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito StoreDatiArt14e11ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            AreaEsito Esito = StoreDatiArt14e11ByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, true);
            return Esito;
        }

        public AreaEsito StoreAnte67ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            AreaEsito Esito = StoreAnte67ByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, true);
            return Esito;
        }

        public AreaEsito StoreSL336ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            AreaEsito Esito = StoreSL336ByDatiPensionePrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, true);
            return Esito;
        }

        public AreaEsito CancelDatiArt14e11ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            areaDatiContributivi = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiArt14e11ByDatiPensione(datiPensione, ref datiFondoCommon);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }

            return GetDatiContributiviByDatiPensioneForCancel(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, out areaDatiContributivi);

        }

        private AreaEsito StoreDatiArt14e11ByDatiPensionePrivate(GestionePensione.DatiPensione datiPensione,
            ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, AreaDatiContributivi areaDatiContributivi, bool isSingleTab)
        {
            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();
            GestioneContrib.DatiArt11e14 datiArt11e14 = null;
            datiArt11e14 = areaDatiContributivi.DatiArt11e14;
            GestioneContrib.StoreDatiArt14e11(datiPensione, datiArt11e14, ref datiFondo, ref fondoXX, areaDatiContributivi.DatiCalcolo, isSingleTab, out messaggioVideo);
            areaDatiContributivi.DatiArt11e14 = datiArt11e14;
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito StoreAnte67ByDatiPensionePrivate(GestionePensione.DatiPensione datiPensione,
           ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, AreaDatiContributivi areaDatiContributivi, bool isSingleTab)
        {
            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();
            GestioneContrib.DatiAnte67 datiAnte67 = null;
            datiAnte67 = areaDatiContributivi.DatiAnte67;
            if (datiAnte67 == null || datiAnte67.IsNull())
            {
                return Esito;
            }
            GestioneContrib.StoreDatiAnte67(datiPensione, datiAnte67, ref datiFondo, ref fondoXX, areaDatiContributivi.DatiCalcolo, isSingleTab, out messaggioVideo);
            areaDatiContributivi.DatiAnte67 = datiAnte67;
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito StoreSL336ByDatiPensionePrivate(GestionePensione.DatiPensione datiPensione,
          ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, AreaDatiContributivi areaDatiContributivi, bool isSingleTab)
        {
            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();
            GestioneContrib.DatiSL33670 datiSL336 = null;
            datiSL336 = areaDatiContributivi.DatiSL336;
            if (datiSL336 == null || datiSL336.IsNull())
            {
                return Esito;
            }
            GestioneContrib.StoreDatiSL336(datiPensione, datiSL336, ref datiFondo, ref fondoXX, areaDatiContributivi.DatiCalcolo, isSingleTab, out messaggioVideo);
            areaDatiContributivi.DatiSL336 = datiSL336;
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiAnte67ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            areaDatiContributivi = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiAnte67ByDatiPensione(datiPensione, ref datiFondoCommon);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }

            return GetDatiContributiviByDatiPensioneForCancel(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, out areaDatiContributivi);

        }

        public AreaEsito CancelSL336ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            areaDatiContributivi = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiSL336ByDatiPensione(datiPensione, ref datiFondoCommon);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }

            return GetDatiContributiviByDatiPensioneForCancel(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, out areaDatiContributivi);

        }

        public AreaEsito StoreAltraPensioneDatiAgoByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            AreaEsito Esito = StoreAltraPensioneDatiAgoPrivate(datiPensione, ref datiFondoCommon, ref fondoXX, areaDatiContributivi, true);
            return Esito;
        }

        private AreaEsito StoreAltraPensioneDatiAgoPrivate(GestionePensione.DatiPensione datiPensione,
            ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, AreaDatiContributivi areaDatiContributivi, bool isSingleTab)
        {
            string messaggioVideo = "";
            AreaEsito Esito = new AreaEsito();

            GestioneContrib.ControlsDatiAgoAltraPensione(areaDatiContributivi.DatiAgoAltraPensione, datiPensione, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            GestioneContrib.StoreDatiAgoAltraPensione(datiPensione, areaDatiContributivi.DatiAgoAltraPensione, ref datiFondo, ref fondoXX, areaDatiContributivi.DatiCalcolo, isSingleTab, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiAgoAltraPensioneByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            areaDatiContributivi = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiAgoAltraPensioneByDatiPensione(datiPensione, ref datiFondoCommon);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }

            return GetDatiContributiviByDatiPensioneForCancel(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, out areaDatiContributivi);

        }

        #endregion Dati Calcolo

        #region Dati Calcolo 707

        public AreaEsito StoreDatiCalcolo707ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            AreaEsito Esito = StoreDatiCalcolo707ByDatiPensionePrivate(datiPensione, datiDanteCausa, ref datiFondoCommon, ref fondoXX, areaDatiContributivi);
            return Esito;
        }

        public AreaEsito CancelDatiCalcolo707ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            GetDatiDBFondi(ref contenitore, out fondoXX);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            areaDatiContributivi = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiCalcolo707ByDatiPensione(datiPensione, datiFondoCommon);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                return Esito;
            }

            return GetDatiContributiviByDatiPensioneForCancel(datiPensione, datiMaggiorazioniBeneficiCommon, datiFondoCommon, fondoXX, isRiaperturaDomanda, out areaDatiContributivi);

        }

        #endregion Dati Calcolo 707

        #region Liste Decodifica & Cross Properties

        public static void GetListeDecodifica(char? codiceSpecificoTraduzioneSuGP, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            List<TipoLiquidazioneGAS> listaTipoLiquidazioneGAS = null;
            GestioneContrib.GetListaTipoLiquidazioneGAS(out listaTipoLiquidazioneGAS);
            if (listaTipoLiquidazioneGAS != null && listaTipoLiquidazioneGAS.Count > 0)
            {
                if (areaDatiContributivi == null)
                    areaDatiContributivi = new AreaDatiContributivi();
                areaDatiContributivi.ListaTipoLiquidazioneGAS = listaTipoLiquidazioneGAS;
            }

            List<TipoLiquidazionePI> listaTipoLiquidazionePI = null;
            GestioneContrib.GetListaTipoLiquidazionePI(out listaTipoLiquidazionePI);
            if (listaTipoLiquidazionePI != null && listaTipoLiquidazionePI.Count > 0)
            {
                if (areaDatiContributivi == null)
                    areaDatiContributivi = new AreaDatiContributivi();
                areaDatiContributivi.ListaTipoLiquidazionePI = listaTipoLiquidazionePI;
            }

            List<AttCon> listaAttCon = null;
            GestioneContrib.GetListaAttCon(codiceSpecificoTraduzioneSuGP, out listaAttCon);
            if (listaAttCon != null && listaAttCon.Count > 0)
            {
                if (areaDatiContributivi == null)
                    areaDatiContributivi = new AreaDatiContributivi();
                areaDatiContributivi.ListaAttCon = listaAttCon;
            }
        }

        private static void GetCrossProperties(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, GestioneFondo.DatiFondo datiFondoCommon, object datiFondoXX, ref AreaDatiContributivi areaDatiContributivi,
            GestioneContrib.DatiCalcolo datiCalcoloBL, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, char? codiceSpecificoTraduzioneSuGP, Utility.TipoFondo? tipoFondo)
        {
            GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia = null;
            Dictionary<string, char?> TipoPensione = null;
            Utility.CategoriaFondoPI? categoriaFondoPI = null;
            Dictionary<string, bool?> lCrossProperties = GestioneContrib.GetCrossProperties(datiPensione, isRiaperturaDomanda, datiDanteCausa, datiCalcoloBL, datiFondoCommon, datiFondoXX, codiceSpecificoTraduzioneSuGP, tipoFondo,
                out TipologiaSalvaguardia, out TipoPensione, out categoriaFondoPI);

            areaDatiContributivi.IsRiduzioneRetribVisible = lCrossProperties["RiduzioneRetribVisible"];
            areaDatiContributivi.IsContribL214Visible = lCrossProperties["ContribL214Visible"];
            areaDatiContributivi.IsAnzianita = lCrossProperties["Anzianita"];
            areaDatiContributivi.IsVecchiaiaSpecifica = lCrossProperties["VecchiaiaSpecifica"];
            areaDatiContributivi.IsInvaliditaSpecifica = lCrossProperties["InvaliditaSpecifica"];
            areaDatiContributivi.TipologiaSalvaguardia = TipologiaSalvaguardia;
            areaDatiContributivi.IsUsuranti = lCrossProperties["Usuranti"];
            areaDatiContributivi.TipoPensione = TipoPensione;
            areaDatiContributivi.IsAltraPensioneVisible = lCrossProperties["IsAltraPensioneVisible"];
            areaDatiContributivi.IsDecorrenzaSuccSett1989 = lCrossProperties["IsDecorrenzaSuccSett1989"];
            areaDatiContributivi.IsRiduzioneRetributivaEnabled = lCrossProperties["IsRiduzioneRetributivaEnabled"];
            areaDatiContributivi.IsSettimane707Visible = lCrossProperties["IsSettimane707Visible"];
            areaDatiContributivi.IsAnteArmonizzazione = lCrossProperties["IsAnteArmonizzazione"];
            areaDatiContributivi.CategoriaFondoPI = categoriaFondoPI;
            areaDatiContributivi.IsContribuzioneL335NonObbligatoria = lCrossProperties["IsContribuzioneL335NonObbligatoria"];
            areaDatiContributivi.IsPIAPIBAnte99 = lCrossProperties["IsPIAPIBAnte99"];
        }

        #endregion Liste Decodifica & Cross Properties

        #region dati Ago Fondo PI

        public AreaEsito GetDatiAgoFondoPIById(long idDatiAgoFondoPI, out AreaDatiAgoFondoPI datiAgoFondoPI)
        {
            SetCulture();
            datiAgoFondoPI = null;
            return GetDatiAgoFondoPIByIdPrivate(idDatiAgoFondoPI, out datiAgoFondoPI);
        }

        public AreaEsito StoreDatiAgoFondoPIById(AreaDatiAgoFondoPI areaDatiAgoFondoPI)
        {
            SetCulture();
            AreaEsito Esito = new AreaEsito();

            return StoreDatiAgoFondoPIByIdPrivate(areaDatiAgoFondoPI);
        }

        public AreaEsito GetDatiAgoFondoPIByIdPrivate(long idDatiAgoFondoPI, out AreaDatiAgoFondoPI datiAgoFondoPI)
        {
            AreaEsito Esito = new AreaEsito();
            datiAgoFondoPI = new AreaDatiAgoFondoPI()
            {
                DettaglioDatiAgoFondoPi = new GestioneFondo.DatiAgoPI()
            };


            try
            {
                GestioneFondo.DatiAgoPI datiDB = new GestioneFondo.DatiAgoPI();
                GestioneFondo.GetDatiAgoPIById(idDatiAgoFondoPI, out datiDB);
                datiAgoFondoPI.DettaglioDatiAgoFondoPi = datiDB;

            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel recupero dei dati Ago";
            }


            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito StoreDatiAgoFondoPIByIdPrivate(AreaDatiAgoFondoPI areaDatiAgoFondoPI)
        {
            AreaEsito Esito = new AreaEsito();

            if (areaDatiAgoFondoPI == null || areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi == null)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Dettaglio non trovato";
                return Esito;
            }
            try
            {
                //semaforo a verde 
                areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi.SemaforoRecord = 2;
                GestioneFondo.InsertOrUpdateDatiAgoPI(areaDatiAgoFondoPI.Id, areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi);

            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati Ago";
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;

        }

        public AreaEsito CancelDatiAgoPensioneFondoPI(long idDatiAgoPI)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestioneFondo.EliminaDatiAgoPISingolo(idDatiAgoPI);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }
        #endregion

        #region dati pensione fondo PI

        public AreaEsito GetDatiPensioneFondoPIById(long idRecord, out AreaDatiPensioneFondoPI datiPensioneFondoPI)
        {
            SetCulture();
            datiPensioneFondoPI = null;
            return GetDatiPensioneFondoPIByIdPrivate(idRecord, out datiPensioneFondoPI);
        }

        public AreaEsito StoreDatiPensioneFondoPIByIdRecord(AreaDatiPensioneFondoPI areaDatiPensioneFondoPI)
        {
            SetCulture();
            AreaEsito Esito = new AreaEsito();

            return StoreDatiPensioneFondoPIByIdRecordPrivate(areaDatiPensioneFondoPI);
        }

        public AreaEsito CancelDatiFondoPensioneFondoPI(long idRecordFondo) 
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestioneFondo.EliminaFondoPIRecordFondo(idRecordFondo);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private AreaEsito GetDatiPensioneFondoPIByIdPrivate(long idRecord, out AreaDatiPensioneFondoPI datiPensioneFondoPI)
        {
            AreaEsito Esito = new AreaEsito();
            datiPensioneFondoPI = new AreaDatiPensioneFondoPI()
            {
                DettaglioDatiPensioneFondoPi = new GestioneFondo.DatiFondoPI()
            };


            try
            {
                GestioneFondo.DatiFondoPI datiDBPensioneFondoPI = new GestioneFondo.DatiFondoPI();
                GestioneFondo.GetDettaglioPensioneFondoPIByIdRecord(idRecord, out datiDBPensioneFondoPI);
                datiPensioneFondoPI.DettaglioDatiPensioneFondoPi = datiDBPensioneFondoPI;
                
                GestioneRecordFondo.DatiRecordFondo datiDBRecordFondo = new GestioneRecordFondo.DatiRecordFondo();
                GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecord, out datiDBRecordFondo);
                datiPensioneFondoPI.DatiRecordFondo = datiDBRecordFondo;

            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel recupero dei dati Ago";
            }


            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito StoreDatiPensioneFondoPIByIdRecordPrivate(AreaDatiPensioneFondoPI areaDatiPensioneFondoPI)
        {
            AreaEsito Esito = new AreaEsito();

            if (areaDatiPensioneFondoPI == null || areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi == null)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Dettaglio non trovato";
                return Esito;
            }
            try
            {
                SetCulture();

                EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaDatiPensioneFondoPI.NumDomanda, null);

                GestionePensione.DatiPensione datiPensione = null;
                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
                GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
                GestioneFondo.DatiFondo datiFondoCommon = null;
                GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                object fondoXX = null;

                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
                GetDatiDBFondi(ref contenitore, out fondoXX);

                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);


                string msg = "";
                GestioneContrib.ControlsDatiPensioneFondoPI(areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi, datiPensione, datiDanteCausa, areaDatiPensioneFondoPI.ControCodiceRetribuzione, out msg);
                
                if (string.IsNullOrEmpty(msg))
                {
                    GestioneRecordFondo.SalvaSingoloRecordFondo(datiPensione.Id, areaDatiPensioneFondoPI.DatiRecordFondo);
                    
                    areaDatiPensioneFondoPI.IdRecordFondo = areaDatiPensioneFondoPI.DatiRecordFondo.Id;
                    //semaforo a verde 
                    areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi.SemaforoRecord = 2;
                    
                    GestioneFondo.SalvaFondoPIRecordFondo(areaDatiPensioneFondoPI.IdFondo, areaDatiPensioneFondoPI.IdRecordFondo, areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi);
                }
                else 
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = msg;
                    return Esito;
                }

            }
            catch (Exception Ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati Ago";
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        #endregion

        #endregion AreaDatiContributivi

        #region Calcolo
        public AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, bool isReingegnerizzato, bool? isNuovoCalcolo, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out string statoPensione, out int certificato)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;

            statoPensione = string.Empty;
            certificato = 0;
            listaConsultazioniANF = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo;
                bool esito = false;

                if (!GestioneCalcoloDomanda.ControlsDatiCalcolaDomanda(ref contenitore, ref contenitoreDecodifica, datiPensione, matricolaOperatore, isConsultazioniANFVerificate, out listaConsultazioniANF, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }

                DateTime dataSistema = Utility.DataSistemaFs;
                GestioneControlliDinamici.ControlloDinamico controlloDinamicoData = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioNuovoTracciato", out controlloDinamicoData);
                DateTime dataInizioNuovoTracciato = Utility.DataFromString(controlloDinamicoData.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

                if (datiPensione.IsRicRinnovata.GetValueOrDefault() || Utility.DataSuccessivaA(dataSistema, dataInizioNuovoTracciato))
                    GestioneCalcoloDomanda.CalcolaDomandaNew(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isReingegnerizzato, isNuovoCalcolo, out statoPensione, out certificato, out esito, out messaggioVideo);
                else
                    GestioneCalcoloDomanda.CalcolaDomanda(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isReingegnerizzato, out statoPensione, out certificato, out esito, out messaggioVideo);

                if (esito)
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                else
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            catch (Exception Ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Utility.GetMessageFromException(Ex), null, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel calcolo della domanda. Riprovare più tardi";
            }

            return Esito;
        }
        #endregion Calcolo

        #region Prelievo
        public AreaEsito PrelevaDomanda(ref AreaPrelievo areaPrelievo)
        {
            SetCulture();

            string messaggioVideo = string.Empty;
            AreaEsito Esito = new AreaEsito();
            GestionePrelievo.RispostaPrelievo risposta = null;
            try
            {
                GestionePrelievo.PrelevaDomanda(areaPrelievo.Richiesta, out risposta, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }

                areaPrelievo.Risposta = risposta;
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                messaggioVideo = "Errore tecnico durante il prelievo dei dati della pensione";
                string parametri = string.Format("Numero domanda: {0}; Sede: {1}; Categoria: {2}; Certificato: {3}: Sede operatore: {4}; Centro operativo operatore: {5}; Tipo domanda: {6}",
                    areaPrelievo.Richiesta.NumDomanda, areaPrelievo.Richiesta.Sede, areaPrelievo.Richiesta.Categoria, areaPrelievo.Richiesta.Certificato, areaPrelievo.Richiesta.SedeOperatore,
                    areaPrelievo.Richiesta.CentroOperativoOperatore, areaPrelievo.Richiesta.TipoDomanda.ToString());
                long numeroDomanda = 0;
                long.TryParse(areaPrelievo.Richiesta.NumDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            return Esito;
        }

        public AreaEsito EseguiSprenotazione(AreaPrelievo areaPrelievo)
        {
            SetCulture();

            string messaggioVideo = string.Empty;
            AreaEsito Esito = new AreaEsito();
            try
            {
                GestionePrelievo.EseguiSprenotazione(areaPrelievo.Richiesta, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                messaggioVideo = "Errore tecnico durante la sprenotazione dei dati della pensione per i fondi speciali";
                string parametri = string.Format("Numero domanda: {0}; Sede: {1}; Categoria: {2}; Certificato: {3}: Sede operatore: {4}; Centro operativo operatore: {5}; Tipo domanda: {6}",
                    areaPrelievo.Richiesta.NumDomanda, areaPrelievo.Richiesta.Sede, areaPrelievo.Richiesta.Categoria, areaPrelievo.Richiesta.Certificato, areaPrelievo.Richiesta.SedeOperatore,
                    areaPrelievo.Richiesta.CentroOperativoOperatore, areaPrelievo.Richiesta.TipoDomanda.ToString());
                long numeroDomanda = 0;
                long.TryParse(areaPrelievo.Richiesta.NumDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            return Esito;
        }
        #endregion Prelievo

        #region AreaMaggiorazioniBenefici

        public AreaEsito GetMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            DatiExCombattente datiExCombattente = null;
            DatiBenefici datiBenefici = null;
            DatiDL407 datiDL407 = null;
            DatiPrivilegiate datiPrivilegiate = null;
            DatiArticolo2 datiArticolo2 = null;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            char? codiceSpecificoTraduzioneSuGP = null;
            byte? codiceSpecifico = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);
            datiDanteCausa = contenitore.DatiDanteCausa;

            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> listaCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (listaCodiceSpecifico != null && listaCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = listaCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico);
                    if (codice != null)
                    {
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                        codiceSpecifico = codice.Id;
                    }
                }
            }

            ValorizzaDatiForMaggiorazioniBenefici(ref contenitore, datiPensione.Id, datiPensione.SiglaCategoria, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiDL407, out datiPrivilegiate, out datiArticolo2);

            AreaEsito Esito = new AreaEsito();
            areaMaggiorazioniBenefici = null;

            if (datiExCombattente != null)
            {
                areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiExCombattente = datiExCombattente;
            }

            if (datiBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiBenefici = datiBenefici;
            }

            if (datiDL407 != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiDL407 = datiDL407;
            }

            if (areaMaggiorazioniBenefici == null)
                areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();

            GestioneMaggiorazioniBenefici.PrevalorizzaArticolo2(datiIstruttoriaCommon, ref datiArticolo2);
            areaMaggiorazioniBenefici.DatiArticolo2 = datiArticolo2;

            if (datiPrivilegiate != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiPrivilegiate = datiPrivilegiate;
            }

            List<CodiceCieco> listaCodiceCieco = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceCieco(ref contenitoreDecodifica, out listaCodiceCieco);
            if (listaCodiceCieco != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceCieco = listaCodiceCieco;
            }

            List<TipoBenefici> listaTipoBenefici = null;
            GestioneMaggiorazioniBenefici.GetListaTipoBenefici(ref contenitoreDecodifica, datiPensione, codiceSpecifico, out listaTipoBenefici);
            if (listaTipoBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipoBenefici = listaTipoBenefici;
            }

            List<CodiceMaggiorazioneExCombattente> listaCodiceMaggiorazioneExCombattente = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceMaggiorazioneExCombattente(ref contenitoreDecodifica, out listaCodiceMaggiorazioneExCombattente);
            if (listaCodiceMaggiorazioneExCombattente != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceMaggiorazioneExCombattente = listaCodiceMaggiorazioneExCombattente;
            }

            List<CodicePensioniPrivilegiate> listaCodicePensioniPrivilegiate = null;
            GestioneMaggiorazioniBenefici.GetListaCodicePensioniPrivilegiate(out listaCodicePensioniPrivilegiate);
            if (listaCodicePensioniPrivilegiate != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodicePensioniPrivilegiate = listaCodicePensioniPrivilegiate;
            }

            #region Beneficio Vittime Terrorismo

            DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetDatiBeneficioVittimeTerrorismo(ref contenitore, out datiBeneficioVittimeTerrorismo);
            if (datiBeneficioVittimeTerrorismo != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo = datiBeneficioVittimeTerrorismo;
            }

            #region Decodifiche

            List<SoggettoBeneficiario> listaSoggettoBeneficiario = null;
            GestioneMaggiorazioniBenefici.GetListaSoggettoBeneficiario(ref contenitore, ref contenitoreDecodifica, out listaSoggettoBeneficiario);
            if (listaSoggettoBeneficiario != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaSoggettoBeneficiario = listaSoggettoBeneficiario;
            }

            List<TipologiaPrestazione> listaTipologiaPrestazione = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaPrestazione(ref contenitoreDecodifica, out listaTipologiaPrestazione);
            if (listaTipologiaPrestazione != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipologiaPrestazione = listaTipologiaPrestazione;
            }

            List<TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaBeneficioTerrorismo(ref contenitoreDecodifica, out listaTipologiaBeneficioTerrorismo);
            if (listaTipologiaBeneficioTerrorismo != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipologiaBeneficioTerrorismo = listaTipologiaBeneficioTerrorismo;
            }
            #endregion Decodifiche

            #endregion Beneficio Vittime Terrorismo

            Utility.TipoFondo? tipoFondo = null;
            if (datiPensione != null)
                tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            Object objectFondoXX = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.FS:
                        objectFondoXX = (object)contenitore.ListaDatiFondoFST;
                        break;
                    case Utility.TipoFondo.PT:
                        objectFondoXX = (object)contenitore.ListaDatiFondoPT;
                        break;
                    case Utility.TipoFondo.DZ:
                        contenitore.DatiRetributivi = null;
                        break;
                }
            }

            GetCrossProperties(datiPensione, datiDanteCausa, datiMaggiorazioniBeneficiCommon, datiBeneficioVittimeTerrorismo, codiceSpecificoTraduzioneSuGP, contenitore.DatiFondo != null ? contenitore.DatiFondo.SettimaneUtiliDiritto : null, ref areaMaggiorazioniBenefici, contenitore.DatiRetributivi, contenitore.ListaDatiCalcoloContributivoRecordFondo, contenitore.ListaDatiServizioUtile, contenitore.ListaRecordDatiFondoINPDAP, objectFondoXX);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;


        }

        public AreaEsito StoreMaggiorazioniBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            char? derogaTraduzioneSuGP = null;
            char? codiceSpecifico = null;
            object datiFondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);
            GetDatiDBFondi(ref contenitore, out datiFondoXX);
            listaRecordFondo = contenitore.ListaDatiRecordFondo;

            if (datiIstruttoriaCommon != null && datiIstruttoriaCommon.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = contenitoreDecodifica.ElencoCodiceParticolare;
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoriaCommon.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }

            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> listaCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (listaCodiceSpecifico != null && listaCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = listaCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico);
                    if (codice != null)
                        codiceSpecifico = codice.TraduzioneGp;
                }
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = contenitore.DatiAnagraficiTitolare;
            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo = contenitore.ListaDatiContributivi;
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = contenitore.ListaDatiCalcoloVittimeTerrorismo;
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(datiPensione);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiExCombattentePrivate(ref contenitore, datiPensione, ref datiMaggiorazioniBeneficiCommon, datiFondoCommon, listaRecordFondo, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiBeneficiPrivate(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiMaggiorazioniBeneficiCommon, datiFondoCommon, datiAnagraficaTitolare, areaMaggiorazioniBenefici, datiFondoXX, codiceSpecifico, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiDL407Private(ref contenitore, datiPensione, datiFondoCommon, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiArticolo2Private(ref contenitore, datiPensione, datiFondoCommon, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiPrivilegiatePrivate(ref contenitore, ref contenitoreDecodifica, datiPensione, datiFondoCommon, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiBeneficioVittimeTerrorismoPrivate(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici, listaDatiCalcoloVittimeTerrorismo, datiCalcoloContributivo, tipoCalcolo, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #region DatiBenefici

        public AreaEsito StoreDatiBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            char? codiceSpecifico = null;
            object datiFondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);

            GetDatiDBFondi(ref contenitore, out datiFondoXX);

            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> listaCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (listaCodiceSpecifico != null && listaCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = listaCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico);
                    if (codice != null)
                        codiceSpecifico = codice.TraduzioneGp;
                }
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = contenitore.DatiAnagraficiTitolare;

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiBeneficiPrivate(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiMaggiorazioniBeneficiCommon, datiFondoCommon, datiAnagraficaTitolare, areaMaggiorazioniBenefici, datiFondoXX, codiceSpecifico, true);

            return Esito;
        }

        private AreaEsito StoreDatiBeneficiPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneFondo.DatiFondo datiFondo, GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, object datiFondoXX, char? codiceSpecifico,
            bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, datiFondo, null, true, null, null, null, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlDatiBenefici(ref contenitore, ref contenitoreDecodifica, datiPensione, areaMaggiorazioniBenefici.DatiBenefici, datiAnagraficaTitolare, datiMaggiorazioniBenefici, datiFondoXX, datiFondo, codiceSpecifico,
                    false, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
                    if (datiQuadroMaggiorazioniBenefici.TabBenefici == 0)
                        GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Benefici_FS));

                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                try
                {
                    GestioneMaggiorazioniBenefici.StoreDatiBenefici(ref contenitore, datiPensione, ref datiMaggiorazioniBenefici, areaMaggiorazioniBenefici.DatiBenefici);
                }
                catch (Exception)
                {
                    GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
                    if (datiQuadroMaggiorazioniBenefici.TabBenefici == 0)
                        GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Benefici_FS));
                    throw;
                }

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        public AreaEsito CancelDatiBenefici(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object datiFondoXX = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, true, true, true);
            GetDatiDBFondi(ref contenitore, out datiFondoXX);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = contenitore.DatiAnagraficiTitolare;
            List<GestioneOneri.DatiOneri> listaDatiOneri = contenitore.ListaDatiOneri;

            areaMaggiorazioniBenefici = null;
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            char? codiceSpecifico = null;
            byte? datiFondoCodSpec = null;

            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> listaCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (listaCodiceSpecifico != null && listaCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = listaCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico);
                    if (codice != null)
                    {
                        codiceSpecifico = codice.TraduzioneGp;
                        datiFondoCodSpec = codice.Id;
                    }
                }
            }

            GestioneMaggiorazioniBenefici.ControlDatiBeneficiForCancel(ref contenitore, ref contenitoreDecodifica, datiPensione, datiAnagraficaTitolare, datiMaggiorazioniBeneficiCommon, datiFondoXX, datiFondoCommon, codiceSpecifico, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }
            GestioneMaggiorazioniBenefici.EliminaDatiBenefici(ref contenitore, datiPensione, datiMaggiorazioniBeneficiCommon, listaDatiOneri);

            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();

            DatiBenefici datiBenefici = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiByIdPensione(ref contenitore, datiMaggiorazioniBeneficiCommon, out datiBenefici);
            if (datiBenefici != null)
                areaMaggiorazioniBenefici.DatiBenefici = datiBenefici;

            List<TipoBenefici> listaTipoBenefici = null;
            GestioneMaggiorazioniBenefici.GetListaTipoBenefici(ref contenitoreDecodifica, datiPensione, datiFondoCodSpec, out listaTipoBenefici);
            if (listaTipoBenefici != null)
                areaMaggiorazioniBenefici.ListaTipoBenefici = listaTipoBenefici;

            GestioneBypassControllo.SetUnlock(numeroDomanda, typeof(GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Benefici_FS));

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        #endregion DatiBenefici

        #region DatiExCombattente

        public AreaEsito StoreDatiExCombattente(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);
            listaRecordFondo = contenitore.ListaDatiRecordFondo;

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiExCombattentePrivate(ref contenitore, datiPensione, ref datiMaggiorazioniBeneficiCommon, datiFondoCommon, listaRecordFondo, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiExCombattente(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, false);

            areaMaggiorazioniBenefici = null;
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiExCombattente(ref contenitore, datiPensione, datiMaggiorazioniBeneficiCommon);

            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();

            DatiExCombattente datiExCombattente = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiExCombattente(datiMaggiorazioniBeneficiCommon, out datiExCombattente);
            if (datiExCombattente != null)
                areaMaggiorazioniBenefici.DatiExCombattente = datiExCombattente;

            List<CodiceCieco> listaCodiceCieco = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceCieco(ref contenitoreDecodifica, out listaCodiceCieco);
            if (listaCodiceCieco != null)
                areaMaggiorazioniBenefici.ListaCodiceCieco = listaCodiceCieco;

            List<CodiceMaggiorazioneExCombattente> listaCodiceMaggiorazioneExCombattente = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceMaggiorazioneExCombattente(ref contenitoreDecodifica, out listaCodiceMaggiorazioneExCombattente);
            if (listaCodiceMaggiorazioneExCombattente != null)
                areaMaggiorazioniBenefici.ListaCodiceMaggiorazioneExCombattente = listaCodiceMaggiorazioneExCombattente;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private AreaEsito StoreDatiExCombattentePrivate(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, datiFondo, true, null, null, null, null, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlDatiExCombattente(datiPensione.SiglaCategoria, listaRecordFondo, areaMaggiorazioniBenefici.DatiExCombattente, datiPensione.DecorrenzaOriginaria, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneMaggiorazioniBenefici.StoreDatiExCombattente(ref contenitore, datiPensione, ref datiMaggiorazioniBenefici, areaMaggiorazioniBenefici.DatiExCombattente);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        #endregion DatiExCombattente

        #region DatiDL407

        public AreaEsito StoreDatiDL407(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiDL407Private(ref contenitore, datiPensione, datiFondoCommon, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiDL407(long numeroDomanda)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDL407(ref contenitore);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private AreaEsito StoreDatiDL407Private(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, datiFondo, null, null, true, null, null, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlsDatiDL407(ref contenitore, datiPensione, areaMaggiorazioniBenefici.DatiDL407, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneMaggiorazioniBenefici.StoreDatiDL407(ref contenitore, datiPensione, datiFondo, areaMaggiorazioniBenefici.DatiDL407);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        #endregion DatiDL407

        #region DatiArticolo2

        public AreaEsito StoreDatiArticolo2(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiArticolo2Private(ref contenitore, datiPensione, datiFondoCommon, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiArticolo2(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            areaMaggiorazioniBenefici = null;
            AreaEsito Esito = new AreaEsito();
            string errore = string.Empty;
            GestioneMaggiorazioniBenefici.EliminaDatiArticolo2(ref contenitore);
            if (!String.IsNullOrEmpty(errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }
            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            DatiArticolo2 datiArticolo2 = null;
            GestioneMaggiorazioniBenefici.GetDatiArticolo2ByIdPensione(ref contenitore, out datiArticolo2);
            if (datiArticolo2 != null)
                areaMaggiorazioniBenefici.DatiArticolo2 = datiArticolo2;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito StoreDatiArticolo2Private(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, datiFondo, null, null, null, true, null, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlsDatiArticolo2(areaMaggiorazioniBenefici.DatiArticolo2, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneMaggiorazioniBenefici.StoreDatiArticolo2(ref contenitore, datiPensione, areaMaggiorazioniBenefici.DatiArticolo2);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        #endregion DatiArticolo2

        #region DatiPrivilegiate

        public AreaEsito StoreDatiPrivilegiate(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;

            GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiPrivilegiatePrivate(ref contenitore, ref contenitoreDecodifica, datiPensione, datiFondoCommon, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiPrivilegiate(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            areaMaggiorazioniBenefici = null;
            AreaEsito Esito = new AreaEsito();
            string errore = string.Empty;
            GestioneMaggiorazioniBenefici.EliminaDatiPrivilegiate(ref contenitore);
            if (!String.IsNullOrEmpty(errore))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = errore;
                return Esito;
            }

            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();

            DatiPrivilegiate datiPrivilegiate = null;
            GestioneMaggiorazioniBenefici.GetDatiPrivilegiateByIdPensione(ref contenitore, contenitore.DatiPensione.SiglaCategoria, out datiPrivilegiate);
            if (datiPrivilegiate != null)
                areaMaggiorazioniBenefici.DatiPrivilegiate = datiPrivilegiate;

            List<CodicePensioniPrivilegiate> listaCodicePensioniPrivilegiate = null;
            GestioneMaggiorazioniBenefici.GetListaCodicePensioniPrivilegiate(out listaCodicePensioniPrivilegiate);
            if (listaCodicePensioniPrivilegiate != null)
                areaMaggiorazioniBenefici.ListaCodicePensioniPrivilegiate = listaCodicePensioniPrivilegiate;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        private AreaEsito StoreDatiPrivilegiatePrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, datiFondo, null, null, null, null, true, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlsDatiPrivilegiate(ref contenitoreDecodifica, datiPensione.SiglaCategoria, areaMaggiorazioniBenefici.DatiPrivilegiate, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneMaggiorazioniBenefici.StoreDatiPrivilegiate(ref contenitore, datiPensione, areaMaggiorazioniBenefici.DatiPrivilegiate);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        #endregion DatiPrivilegiate

        #region Cross Properties
        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            char? codiceSpecificoTraduzioneSuGP, int? settimaneUtiliDiritto, ref AreaMaggiorazioniBenefici areaMaggBenefici, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, object objectFondoXX)
        {
            int? settimane = null;
            Dictionary<string, bool?> lCrossProperties = GestioneMaggiorazioniBenefici.GetCrossProperties(datiPensione, datiDanteCausa, datiMaggiorazioniBenefici, datiBeneficioVittimeTerrorismo, codiceSpecificoTraduzioneSuGP, settimaneUtiliDiritto, datiRetributivi, listaDatiContributivi, listaServizioUtile, listaRecordDatiFondoINPDAP, objectFondoXX, out settimane);

            if (areaMaggBenefici == null)
                areaMaggBenefici = new AreaMaggiorazioniBenefici();

            areaMaggBenefici.IsNuovaGestioneDL407ForAnteArm = lCrossProperties["IsNuovaGestioneDL407ForAnteArm"];
            areaMaggBenefici.IsBeneficioArt24Comma15BisFromFELPE = lCrossProperties["IsBeneficioArt24Comma15BisFromFELPE"];
            areaMaggBenefici.IsBeneficioApePrecociFromFELPE = lCrossProperties["IsBeneficioApePrecociFromFELPE"];
            areaMaggBenefici.IsDomandaPensioneInabilita = lCrossProperties["IsDomandaPensioneInabilita"];
            areaMaggBenefici.IsBeneficioVittimeTerrorismo = lCrossProperties["IsBeneficioVittimeTerrorismo"];
            areaMaggBenefici.IsMaggiorazioniForMemo72 = lCrossProperties["IsMaggiorazioniForMemo72"];
            areaMaggBenefici.Settimane = settimane;
        }
        #endregion Cross Properties

        #endregion AreaMaggiorazioniBenefici

        #region DatiBeneficioVittimeTerrorismo

        public AreaEsito StoreDatiBeneficioVittimeTerrorismo(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = contenitore.ListaDatiCalcoloVittimeTerrorismo;
            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo = contenitore.ListaDatiContributivi;
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            AreaEsito Esito = new AreaEsito();
            try
            {
                Esito = StoreDatiBeneficioVittimeTerrorismoPrivate(ref contenitore, ref contenitoreDecodifica, areaMaggiorazioniBenefici, listaDatiCalcoloVittimeTerrorismo, datiCalcoloContributivo, tipoCalcolo, true);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati Vittime. Riprovare più tardi";
            }

            return Esito;
        }

        private AreaEsito StoreDatiBeneficioVittimeTerrorismoPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo, List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo,
            Utility.TipoCalcolo tipoCalcolo, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, datiBeneficioVittimeTerrorismo);

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(contenitore.DatiPensione, null, null, null, null, null, null, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                try
                {
                    GestioneMaggiorazioniBenefici.ControlDatiBeneficioVittimeTerrorismo(ref contenitoreDecodifica, contenitore.DatiPensione, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo, listaDatiCalcoloVittimeTerrorismo,
                        datiCalcoloContributivo, datiBeneficioVittimeTerrorismo, tipoCalcolo, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }

                    GestioneMaggiorazioniBenefici.StoreDatiBeneficioVittimeTerrorismo(ref contenitore, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo, datiCalcoloContributivo);
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                    Esito.Messaggio = string.Empty;
                }
                catch (Exception Ex)
                {
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = "Errore tecnico nel salvataggio dei dati Vittime. Riprovare più tardi";
                }
            }
            return Esito;
        }

        public AreaEsito CancelDatiBeneficioVittimeTerrorismo(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            AreaEsito Esito = new AreaEsito();
            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            try
            {
                GestioneMaggiorazioniBenefici.EliminaDatiBeneficioVittimeTerrorismo(ref contenitore);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo. Riprovare più tardi";
            }

            List<SoggettoBeneficiario> listaSoggettoBeneficiario = null;
            GestioneMaggiorazioniBenefici.GetListaSoggettoBeneficiario(ref contenitore, ref contenitoreDecodifica, out listaSoggettoBeneficiario);
            if (listaSoggettoBeneficiario != null)
                areaMaggiorazioniBenefici.ListaSoggettoBeneficiario = listaSoggettoBeneficiario;

            List<TipologiaPrestazione> listaTipologiaPrestazione = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaPrestazione(ref contenitoreDecodifica, out listaTipologiaPrestazione);
            if (listaTipologiaPrestazione != null)
                areaMaggiorazioniBenefici.ListaTipologiaPrestazione = listaTipologiaPrestazione;

            List<TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaBeneficioTerrorismo(ref contenitoreDecodifica, out listaTipologiaBeneficioTerrorismo);
            if (listaTipologiaBeneficioTerrorismo != null)
                areaMaggiorazioniBenefici.ListaTipologiaBeneficioTerrorismo = listaTipologiaBeneficioTerrorismo;

            return Esito;
        }

        //public AreaEsito StoreDatiVittimeTerrorismo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        //{
        //    GestionePensione.DatiPensione datiPensione = null;
        //    Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
        //    GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
        //    GestionePrepensionamento.DatiPrepensionamento datiPrepensionamentoCommon = null;
        //    GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;

        //    GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiPrepensionamentoCommon, false, false, false);

        //    GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

        //    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcoloRetributivo = null;
        //    GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out lstDecGestioneCalcoloRetributivo);

        //    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> lstDecGestioneCalcoloContributivo = null;
        //    GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out lstDecGestioneCalcoloContributivo);

        //    AreaEsito Esito = new AreaEsito();
        //    string messaggioControllo = string.Empty;

        //    try
        //    {
        //        StoreDatiVittimeTerrorismoPrivate(datiPensione, areaDatiContributivi, datiBeneficioVittimeTerrorismo, lstDecGestioneCalcoloRetributivo, lstDecGestioneCalcoloContributivo, true,
        //            out messaggioControllo);
        //        if (!String.IsNullOrEmpty(messaggioControllo))
        //        {
        //            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
        //            Esito.Messaggio = messaggioControllo;
        //            return Esito;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        INPS.DNA.Logging.Logger.LogException(Ex);
        //        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
        //        Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo Vittime. Riprovare più tardi";
        //        return Esito;
        //    }

        //    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
        //    Esito.Messaggio = string.Empty;
        //    return Esito;
        //}

        //    private void StoreDatiVittimeTerrorismoPrivate(GestionePensione.DatiPensione datiPensione, AreaDatiContributivi areaDatiContributivi,
        //GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcoloRetributivo,
        //List<GestioneDecodifica.CodeGestioneCalcoloContributivo> lstDecGestioneCalcoloContributivo, bool isSingleTab, out string messaggioControllo)
        //    {
        //        messaggioControllo = string.Empty;

        //        GestioneContrib.StoreDatiCalcoloVittimeTerrorismoByDatiPensione(datiPensione, areaDatiContributivi.DatiCalcoloVittimeTerrorismo, areaDatiContributivi.DatiCalcolo,
        //            datiBeneficioVittimeTerrorismo, lstDecGestioneCalcoloRetributivo, lstDecGestioneCalcoloContributivo, isSingleTab, out messaggioControllo);
        //    }

        #endregion DatiBeneficioVittimeTerrorismo

        #region Get Dati DB

        private void GetDatiDBFondi(ref EntityBLCommon.ContenitoreObject contenitore, out object fondoXX)
        {
            fondoXX = null;

            if (contenitore.DatiPensione == null)
                return;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, contenitore.DatiPensione.SiglaCategoria);

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        GestioneFondo.DatiFondoEL fondoEL = contenitore.DatiFondoEL;
                        fondoXX = (object)fondoEL;
                        break;
                    case Utility.TipoFondo.ET:
                        GestioneFondo.DatiFondoET fondoET = contenitore.DatiFondoET;
                        fondoXX = (object)fondoET;
                        break;
                    case Utility.TipoFondo.TT:
                        GestioneFondo.DatiFondoTT fondoTT = contenitore.DatiFondoTT;
                        fondoXX = (object)fondoTT;
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneFondo.DatiFondoVL fondoVL = contenitore.DatiFondoVL;
                        fondoXX = (object)fondoVL;
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.DatiFondoFST fondoFS = contenitore.DatiFondoFS;
                        fondoXX = (object)fondoFS;
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.DatiFondoPT fondoPT = contenitore.DatiFondoPT;
                        fondoXX = (object)fondoPT;
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.DatiFondoPI fondoPI = contenitore.DatiFondoPI;
                        fondoXX = (object)fondoPI;
                        break;
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.DatiFondoGAS fondoGAS = contenitore.DatiFondoGAS;
                        fondoXX = (object)fondoGAS;
                        break;
                    case Utility.TipoFondo.CL:
                        GestioneFondo.DatiFondoCL fondoCL = contenitore.DatiFondoCL;
                        fondoXX = (object)fondoCL;
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.DatiFondoDZ fondoDZ = contenitore.DatiFondoDZ;
                        fondoXX = (object)fondoDZ;
                        break;
                    case Utility.TipoFondo.ES:
                        GestioneFondo.DatiFondoES fondoES = contenitore.DatiFondoES;
                        fondoXX = (object)fondoES;
                        break;
                    case Utility.TipoFondo.PM:
                        GestioneFondo.DatiFondoPM fondoPM = contenitore.DatiFondoPM;
                        fondoXX = (object)fondoPM;
                        break;
                }
            }
        }

        private void GetDatiDBFondiByIdRecordFondo(Utility.TipoFondo? tipoFondo, long idRecordFondo, out object fondoXX)
        {
            fondoXX = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.FS:
                        GestioneFondo.DatiFondoFST fondoFS = null;
                        GestioneFondo.GetFondoFSTByIdRecordFondo(idRecordFondo, out fondoFS);
                        fondoXX = fondoFS;
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.DatiFondoPT fondoPT = null;
                        GestioneFondo.GetFondoPTByIdRecordFondo(idRecordFondo, out fondoPT);
                        fondoXX = fondoPT;
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.DatiFondoDZ fondoDZ = null;
                        GestioneFondo.GetFondoDZByIdRecordFondo(idRecordFondo, out fondoDZ);
                        fondoXX = fondoDZ;
                        break;
                }
            }
        }

        #endregion Get Dati DB

        #region AreaDatiFondo

        public AreaEsito GetQuadroDatiFondoByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            areaDatiFondo = new AreaDatiFondo();

            GestionePensione.DatiPensione datiPensione = null;
            DatiRegistrazioneFondo entityDatiRecordFondo = null;
            try
            {
                datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

                GestioneDatiFondo.GetDatiRecordFondoByIdPensione(datiPensione, out entityDatiRecordFondo);
                areaDatiFondo.DatiRegistrazioniFondo = entityDatiRecordFondo;


            }

            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito AddRegistrazioneFondoByDomanda(long numeroDomanda, out AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            areaDatiFondo = new AreaDatiFondo();
            GestionePensione.DatiPensione datiPensione = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            Utility.TipoFondo? tipoFondo = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            long idRecordFondo;
            List<GestioneDatiServizioUtile.ServizioUtile> lstServizioUtile = null;
            List<GestioneCalcolo.ServizioUtile707> lstServizioUtile707 = null;
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lstServizioUtileINPDAP = null;
            List<GestioneCalcolo.ServizioUtileINPDAP707> lstServizioUtileINPDAP707 = null;
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = null;

            try
            {
                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    //get
                    GestioneDatiFondo.AddRecordFondoINPDAP(datiPensione, ref recordDatiFondoINPDAP, ref lstServizioUtileINPDAP, out datiQuadroDatiRecordFondo, out idRecordFondo);
                    GetRegistrazioneFondoForAddOperationINPDAP(idRecordFondo, datiPensione, ref lstServizioUtileINPDAP, ref lstServizioUtileINPDAP707, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, ref datiControlloFelpe,
                        out areaDatiFondo, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                    {
                        throw new DNA.DnaValidationException(messaggioVideo);
                    }
                }
                else
                {
                    //get
                    tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GestioneDatiFondo.AddRecordFondo(datiPensione, tipoFondo, ref datiFondoCommon, ref fondoXX, ref lstServizioUtile, out datiQuadroDatiRecordFondo, out idRecordFondo);
                    GetRegistrazioneFondoForAddOperation(idRecordFondo, tipoFondo, datiPensione, ref fondoXX, ref lstServizioUtile, ref lstServizioUtile707, ref datiQuadroDatiRecordFondo, ref datiFondoCommon, out areaDatiFondo, contenitore.DatiDanteCausa, contenitore.DatiLavorazione, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                    {
                        throw new DNA.DnaValidationException(messaggioVideo);
                    }
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelRegistrazioneFondoByIdRecordFondo(Int64 numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            DatiRegistrazioneFondo datiRegistrazioneFondo = null;
            try
            {
                //parameter
                long idRecordFondo = areaDatiFondo.IdRecordFondo;
                //get
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    //delete
                    GestioneDatiFondo.EliminaDatiRecordFondoINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo);
                }
                else
                {
                    //delete
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GestioneDatiFondo.EliminaDatiRecordFondoByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo);
                }

                //get area
                GestioneDatiFondo.GetDatiRecordFondoByIdPensione(datiPensione, out datiRegistrazioneFondo);

                areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiRegistrazioniFondo = datiRegistrazioneFondo;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }


            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelRegistrazioniFondoByDomanda(Int64 numeroDomanda, out AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            areaDatiFondo = null;
            GestionePensione.DatiPensione datiPensione = null;
            try
            {
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    GestioneDatiFondo.EliminaDatiRecordFondoINPDAPByDatiPensione(datiPensione);
                else
                    GestioneDatiFondo.EliminaDatiRecordFondoByDatiPensione(datiPensione);
            }
            catch (Exception ex)
            {
                AreaEsito Esito = new AreaEsito();
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            return GetQuadroDatiFondoByDomanda(new AreaRichiestaDomanda { NumeroDomanda = numeroDomanda, ProgStorico = null }, out areaDatiFondo);
        }

        public AreaEsito GetRegistrazioneFondoByIdRecordFondo(AreaRichiestaDomanda areaRichiestaDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();
            string errori = string.Empty;
            long idRecordFondo = areaDatiFondo.IdRecordFondo;
            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione;
            try
            {
                datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    GetRegistrazioneFondoINPDAPByIdRecordFondoPrivate(idRecordFondo, datiPensione, ref areaDatiFondo, out errori);
                else
                    GetRegistrazioneFondoByIdRecordFondoPrivate(idRecordFondo, datiPensione, ref areaDatiFondo, out errori);
                if (!String.IsNullOrEmpty(errori))
                {
                    throw new DNA.DnaValidationException(errori);
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito StoreQuadroDatiFondoByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            char? codiceSpecificoTraduzioneSuGP = null;

            try
            {
                //paramethers
                long idRecordFondo = areaDatiFondo.IdRecordFondo;

                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);

                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

                if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico.Value);
                        if (codice != null)
                            codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                    }
                }
                ValorizzaSemaforiTabDatiFondoByIdRecordFondo(idRecordFondo, ref datiQuadroDatiRecordFondo, ref areaDatiFondo, datiPensione, codiceSpecificoTraduzioneSuGP, tipoFondo, contenitore.DatiDanteCausa, contenitore.DatiLavorazione);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    //Dati Fondo
                    if (!GestioneDatiFondo.ControlsDatiFondoINPDAP(datiPensione, idRecordFondo, areaDatiFondo.DatiFondo, areaDatiFondo.DatiArticolo2, ref listaRecordFondo, recordDatiFondoINPDAP, false,
                        out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiFondoINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiFondo, false);
                    //Dati Calcolo
                    if (!GestioneDatiFondo.ControlsDatiCalcoloINPDAP(datiPensione, areaDatiFondo.DatiCalcolo, areaDatiFondo.DatiArticolo2, recordDatiFondoINPDAP, datiMaggiorazioniBeneficiCommon, datiDanteCausa, out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiCalcoloINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiCalcolo);
                    //Dati Calcolo 707
                    if (datiQuadroDatiRecordFondo.TabDatiCalcolo707.HasValue)
                    {
                        // non sono presenti controlli per questa tab
                        //store
                        GestioneDatiFondo.StoreDatiCalcoloINPDAP707ByidRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiCalcolo707);
                    }

                    //Dati Miglioramenti Contrattuali
                    if (datiQuadroDatiRecordFondo.TabMiglioramentiContrattualiFS.HasValue)
                    {
                        // non sono presenti controlli per questa tab
                        //store
                        //GestioneDatiFondo.StoreDatiQuoteMiglioramentiContrattualiByDomanda(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiCalcolo707);
                    }

                    //Dati Privilegiate
                    if (datiQuadroDatiRecordFondo.TabPrivilegiate.HasValue)
                    {
                        GestioneDatiFondo.ControlsDatiPrivilegiateINPDAP(areaDatiFondo.DatiPrivilegiate, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                        {
                            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                            Esito.Messaggio = messaggioVideo;
                            return Esito;
                        }
                        GestioneDatiFondo.StoreDatiPrivilegiateINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiPrivilegiate);
                    }
                    //Dati Articolo 2
                    if (datiQuadroDatiRecordFondo.TabArticolo2.HasValue)
                    {
                        GestioneDatiFondo.ControlsDatiArticolo2INPDAP(areaDatiFondo.DatiArticolo2, (listaRecordFondo != null && listaRecordFondo.Count > 0 ? listaRecordFondo.Find(x => x.Id == idRecordFondo) : null), recordDatiFondoINPDAP, datiPensione, codiceSpecificoTraduzioneSuGP,
                            out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                        {
                            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                            Esito.Messaggio = messaggioVideo;
                            return Esito;
                        }
                        GestioneDatiFondo.StoreDatiArticolo2INPDAPByIdRecordFondo(idRecordFondo, datiPensione, areaDatiFondo.DatiArticolo2, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, codiceSpecificoTraduzioneSuGP);
                    }
                    //Dati Legge 4/60
                    if (datiQuadroDatiRecordFondo.TabLegge460.HasValue)
                    {
                        //controls
                        GestioneDatiFondo.ControlDatiLegge460(areaDatiFondo.DatiLegge460, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                        {
                            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                            Esito.Messaggio = messaggioVideo;
                            return Esito;
                        }
                        //store
                        GestioneDatiFondo.StoreDatiLegge460INPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiLegge460);
                    }
                }
                else
                {
                    //get
                    GetDatiDBFondiByIdRecordFondo(tipoFondo, idRecordFondo, out fondoXX);

                    //Dati Fondo
                    if (!GestioneDatiFondo.ControlsDatiFondo(datiPensione, isRiaperturaDomanda, tipoFondo, idRecordFondo, areaDatiFondo.DatiFondo, areaDatiFondo.DatiArticolo2, fondoXX, ref listaRecordFondo, false, out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref fondoXX, ref lstDatiQuadroDatiRecordFondo, areaDatiFondo.DatiFondo);
                    //Dati Calcolo
                    if (!GestioneDatiFondo.ControlsDatiCalcolo(datiPensione, areaDatiFondo.DatiCalcolo, areaDatiFondo.DatiArticolo2, fondoXX, codiceSpecificoTraduzioneSuGP,
                        datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio : null, datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.MaggiorazioneAmianto : null,
                        datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.MaggiorazioneInv74 : null, datiMaggiorazioniBeneficiCommon, areaDatiFondo.DatiCalcoloDZ, out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiCalcoloByidRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref lstDatiQuadroDatiRecordFondo, ref fondoXX, areaDatiFondo.DatiCalcolo, areaDatiFondo.DatiCalcoloDZ);
                    //Dati Calolo 707
                    if (datiQuadroDatiRecordFondo.TabDatiCalcolo707.HasValue)
                    {
                        // non sono presenti controlli per questa tab
                        //store
                        GestioneDatiFondo.StoreDatiCalcolo707ByidRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref lstDatiQuadroDatiRecordFondo, ref fondoXX, areaDatiFondo.DatiCalcolo707);
                    }
                    //Dati Legge 4/60
                    if (datiQuadroDatiRecordFondo.TabLegge460.HasValue)
                    {
                        GestioneDatiFondo.ControlDatiLegge460(areaDatiFondo.DatiLegge460, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                        {
                            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                            Esito.Messaggio = messaggioVideo;
                            return Esito;
                        }
                        GestioneDatiFondo.StoreDatiLegge460ByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref datiFondoCommon, ref fondoXX, areaDatiFondo.DatiLegge460);
                    }
                    //Dati Privilegiate
                    if (datiQuadroDatiRecordFondo.TabPrivilegiate.HasValue)
                    {
                        GestioneDatiFondo.ControlsDatiPrivilegiate(datiPensione.SiglaCategoria, areaDatiFondo.DatiPrivilegiate, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                        {
                            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                            Esito.Messaggio = messaggioVideo;
                            return Esito;
                        }
                        GestioneDatiFondo.StoreDatiPrivilegiateByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref fondoXX, ref lstDatiQuadroDatiRecordFondo, areaDatiFondo.DatiPrivilegiate);
                    }
                    //Dati Articolo 2
                    if (datiQuadroDatiRecordFondo.TabArticolo2.HasValue)
                    {
                        GestioneDatiFondo.ControlsDatiArticolo2(tipoFondo, areaDatiFondo.DatiArticolo2, fondoXX,
                            (listaRecordFondo != null && listaRecordFondo.Count > 0 ? listaRecordFondo.Find(x => x.Id == idRecordFondo) : null), datiPensione, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                        {
                            Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                            Esito.Messaggio = messaggioVideo;
                            return Esito;
                        }
                        GestioneDatiFondo.StoreDatiArticolo2ByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, areaDatiFondo.DatiArticolo2, ref datiFondoCommon, ref fondoXX, ref lstDatiQuadroDatiRecordFondo);
                    }
                }

                GetDecodifiche(ref areaDatiFondo);
                GetCrossProperties(datiPensione, datiFondoCommon, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        #region DatiFondo

        public AreaEsito StoreDatiFondoByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            GestionePensione.DatiPensione datiPensione;
            long idRecordFondo = areaDatiFondo.IdRecordFondo;
            object fondoXX = null;

            GestioneFondo.DatiFondo datiFondoCommon = null;

            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;

            try
            {
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);
                    if (!GestioneDatiFondo.ControlsDatiFondoINPDAP(datiPensione, idRecordFondo, areaDatiFondo.DatiFondo, areaDatiFondo.DatiArticolo2, ref listaRecordFondo, recordDatiFondoINPDAP, true,
                        out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiFondoINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiFondo, false);
                }
                else
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GetDatiDBFondiByIdRecordFondo(tipoFondo, idRecordFondo, out fondoXX);
                    if (!GestioneDatiFondo.ControlsDatiFondo(datiPensione, isRiaperturaDomanda, tipoFondo, idRecordFondo, areaDatiFondo.DatiFondo, areaDatiFondo.DatiArticolo2, fondoXX, ref listaRecordFondo, true,
                        out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref fondoXX, ref lstDatiQuadroDatiRecordFondo, areaDatiFondo.DatiFondo);
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelDatiFondoByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione;
            long idRecordFondo = areaDatiFondo.IdRecordFondo;
            DatiFondo datiFondo = null;
            object fondoXX = null;
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            GestioneRecordFondo.DatiRecordFondo datiRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = null;

            try
            {
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    GestioneDatiFondo.StoreDatiFondoINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, new DatiFondo(), true);
                    GestioneDatiFondo.GetDatiFondoINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref datiRecordFondo, ref recordDatiFondoINPDAP, ref datiControlloFelpe,
                        out datiFondo);
                }
                else
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GestioneDatiFondo.StoreDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref fondoXX, ref lstDatiQuadroDatiRecordFondo, new DatiFondo());
                    GestioneDatiFondo.GetDatiFondoByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, ref datiRecordFondo, ref datiFondoCommon, out datiFondo);
                }
                areaDatiFondo.DatiFondo = datiFondo;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        #endregion  DatiFondo

        #region DatiCalcolo

        public AreaEsito StoreDatiCalcoloByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            long idRecordFondo = areaDatiFondo.IdRecordFondo;

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            char? codiceSpecificoTraduzioneSuGP = null;

            try
            {
                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    //get
                    GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

                    if (!GestioneDatiFondo.ControlsDatiCalcoloINPDAP(datiPensione, areaDatiFondo.DatiCalcolo, null, recordDatiFondoINPDAP, datiMaggiorazioniBeneficiCommon, datiDanteCausa, out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    //store
                    GestioneDatiFondo.StoreDatiCalcoloINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiCalcolo);
                }
                else
                {
                    //get
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GetDatiDBFondiByIdRecordFondo(tipoFondo, idRecordFondo, out fondoXX);

                    if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
                    {
                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico.Value);
                            if (codice != null)
                                codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                        }
                    }

                    if (!GestioneDatiFondo.ControlsDatiCalcolo(datiPensione, areaDatiFondo.DatiCalcolo, null, fondoXX, codiceSpecificoTraduzioneSuGP,
                        datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio : null, datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.MaggiorazioneAmianto : null,
                        datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.MaggiorazioneInv74 : null, datiMaggiorazioniBeneficiCommon, areaDatiFondo.DatiCalcoloDZ, out messaggioVideo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioVideo;
                        return Esito;
                    }
                    //store
                    GestioneDatiFondo.StoreDatiCalcoloByidRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref lstDatiQuadroDatiRecordFondo, ref fondoXX, areaDatiFondo.DatiCalcolo, areaDatiFondo.DatiCalcoloDZ);
                }

                GetDecodifiche(ref areaDatiFondo);
                GetCrossProperties(datiPensione, datiFondoCommon, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelDatiCalcoloByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            DatiCalcolo datiCalcolo;
            long idRecordFondo = areaDatiFondo.IdRecordFondo;
            GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = null;
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = null;
            List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon = null;
            List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lServizioUtileINPDAP = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;

            try
            {
                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
                csAggiornamentoPECO_Fondi_AMG dati = null;
                csAggiornamentoPECO_Fondi_AMG_INPDAP datiINPDAP = null;

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    GestioneDatiFondo.DeleteDatiCalcoloINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, ref datiQuadroDatiFondo);
                    lServizioUtileINPDAP = new List<GestioneDatiServizioUtileINPDAP.ServizioUtile>();
                    GestioneDatiFondo.GetDatiCalcoloINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref lServizioUtileINPDAP, ref recordDatiFondoINPDAP, out datiCalcolo, out datiINPDAP, out dati);
                }
                else
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GestioneDatiFondo.DeleteDatiCalcoloByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref datiQuadroDatiRecordFondo, ref fondoXX, ref datiQuadroDatiFondo);
                    lServizioUtileCommon = new List<GestioneDatiServizioUtile.ServizioUtile>();
                    GestioneDatiFondo.GetDatiCalcoloByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, ref lServizioUtileCommon, out datiCalcolo, out dati);
                }
                areaDatiFondo.DatiCalcolo = datiCalcolo;

                GetCrossProperties(datiPensione, datiFondoCommon, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        #endregion DatiCalcolo

        #region DatiCalcolo707

        public AreaEsito StoreDatiCalcolo707ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            long idRecordFondo = areaDatiFondo.IdRecordFondo;

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;

            try
            {
                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, true, true);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    //get
                    GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

                    //store
                    GestioneDatiFondo.StoreDatiCalcoloINPDAP707ByidRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiCalcolo707);
                }
                else
                {
                    //get
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GetDatiDBFondiByIdRecordFondo(tipoFondo, idRecordFondo, out fondoXX);

                    //store
                    GestioneDatiFondo.StoreDatiCalcolo707ByidRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref lstDatiQuadroDatiRecordFondo, ref fondoXX, areaDatiFondo.DatiCalcolo707);
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelDatiCalcolo707ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            DatiCalcolo707 datiCalcolo707 = null;
            long idRecordFondo = areaDatiFondo.IdRecordFondo;
            GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = null;
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = null;
            List<GestioneCalcolo.ServizioUtile707> lServizioUtile707 = null;
            List<GestioneCalcolo.ServizioUtileINPDAP707> lServizioUtileINPDAP = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;

            try
            {
                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

                char? codiceSpecificoTraduzioneSuGP = null;
                if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico.Value);
                        if (codice != null)
                            codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                    }
                }

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    csAggiornamentoPECO_Fondi_AMG_INPDAP datiINPDAP = null;
                    csAggiornamentoPECO_Fondi_AMG dati = null;
                    GestioneDatiFondo.DeleteDatiCalcoloINPDAP707ByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, ref datiQuadroDatiFondo);
                    lServizioUtileINPDAP = new List<GestioneCalcolo.ServizioUtileINPDAP707>();

                    GestioneDatiFondo.GetDatiCalcoloINPDAP707ByIdRecordFondo(idRecordFondo, datiPensione, datiINPDAP, dati, codiceSpecificoTraduzioneSuGP, ref datiQuadroDatiRecordFondo, ref lServizioUtileINPDAP,
                        ref recordDatiFondoINPDAP, out datiCalcolo707, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                    {
                        throw new DNA.DnaValidationException(messaggioVideo);
                    }
                }
                else
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    csAggiornamentoPECO_Fondi_AMG dati = null;
                    GestioneDatiFondo.DeleteDatiCalcolo707ByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref datiQuadroDatiRecordFondo, ref fondoXX, ref datiQuadroDatiFondo);
                    lServizioUtile707 = new List<GestioneCalcolo.ServizioUtile707>();
                    GestioneDatiFondo.GetDatiCalcolo707ByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, dati, codiceSpecificoTraduzioneSuGP, true, ref datiQuadroDatiRecordFondo, ref fondoXX,
                        ref lServizioUtile707, out datiCalcolo707, contenitore.DatiDanteCausa, contenitore.DatiLavorazione, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                    {
                        throw new DNA.DnaValidationException(messaggioVideo);
                    }
                }
                areaDatiFondo.DatiCalcolo707 = datiCalcolo707;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }
        #endregion DatiCalcolo707

        #region Dati Legge 4/60

        public AreaEsito StoreDatiLegge460ForDatiFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;

            long idRecordFondo = areaDatiFondo.IdRecordFondo;
            object datiFondoPT = null;
            try
            {
                //get common
                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    //controls
                    GestioneDatiFondo.ControlDatiLegge460(areaDatiFondo.DatiLegge460, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }
                    //get
                    GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
                    GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);
                    //store
                    GestioneDatiFondo.StoreDatiLegge460INPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiLegge460);
                }
                else
                {
                    //get
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    //controls
                    GestioneDatiFondo.ControlDatiLegge460(areaDatiFondo.DatiLegge460, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }
                    //store
                    GestioneDatiFondo.StoreDatiLegge460ByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref datiFondoCommon, ref datiFondoPT, areaDatiFondo.DatiLegge460);
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito CancelDatiLegge460ForDatiFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = null;
            object datiFondoPT = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            DatiLegge460 datiLegge460 = null;

            try
            {
                //parameters
                long idRecordFondo = areaDatiFondo.IdRecordFondo;
                //get common
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    //get
                    GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
                    GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);
                    //delete
                    GestioneDatiFondo.EliminaDatiLegge460INPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref recordDatiFondoINPDAP, ref lstDatiQuadroDatiRecordFondo);
                    GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);
                    GestioneDatiFondo.GetDatiLegge460INPDAPByIdRecordFondo(idRecordFondo, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, out datiLegge460);
                }
                else
                {
                    //get
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    //delete
                    GestioneDatiFondo.EliminaDatiLegge460ByIdRecordFondo(idRecordFondo, datiPensione, ref datiFondoPT, ref lstDatiQuadroDatiRecordFondo);
                    GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);
                    GestioneDatiFondo.GetDatiLegge460ByIdRecordFondo(idRecordFondo, tipoFondo, ref datiQuadroDatiRecordFondo, ref datiFondoPT, out datiLegge460);
                }
                areaDatiFondo.DatiLegge460 = datiLegge460;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        #endregion Dati Legge 4/60

        #region DatiPrivilegiate

        public AreaEsito StoreDatiPrivilegiateByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;

            try
            {
                long idRecordFondo = areaDatiFondo.IdRecordFondo;

                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    ////store
                    GestioneDatiFondo.ControlsDatiPrivilegiateINPDAP(areaDatiFondo.DatiPrivilegiate, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiPrivilegiateINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, areaDatiFondo.DatiPrivilegiate);
                }
                else
                {
                    //get
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    //store
                    GestioneDatiFondo.ControlsDatiPrivilegiate(datiPensione.SiglaCategoria, areaDatiFondo.DatiPrivilegiate, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiPrivilegiateByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref datiFondoCommon, ref fondoXX, ref lstDatiQuadroDatiRecordFondo, areaDatiFondo.DatiPrivilegiate);
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelDatiPrivilegiateByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            object fondoXX = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;

            DatiPrivilegiate datiPrivilegiate = null;
            try
            {
                long idRecordFondo = areaDatiFondo.IdRecordFondo;
                GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    GestioneDatiFondo.EliminaDatiPrivilegiateINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP);
                    GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);
                    GestioneDatiFondo.GetDatiPrivilegiateINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, out datiPrivilegiate);
                }
                else
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GestioneDatiFondo.EliminaDatiPrivilegiateByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref fondoXX, ref lstDatiQuadroDatiRecordFondo);

                    GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);
                    GestioneDatiFondo.GetDatiPrivilegiateByIdRecordFondo(idRecordFondo, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, out datiPrivilegiate);
                }

                areaDatiFondo.DatiPrivilegiate = datiPrivilegiate;
                GetDecodifiche(ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiPrivilegiate

        #region DatiArticolo2

        public AreaEsito StoreDatiArticolo2ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneFondo.DatiFondo datiFondoCommon = null;
            object fondoXX = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            GestioneRecordFondo.DatiRecordFondo datiRecordFondo = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            char? codiceSpecificoTraduzioneSuGP = null;

            try
            {
                //parameters
                long idRecordFondo = areaDatiFondo.IdRecordFondo;
                //get
                GetDatiDBCommon(ref contenitore, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, out datiFondoCommon, false, false, true);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out datiRecordFondo);

                    if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
                    {
                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico.Value);
                            if (codice != null)
                                codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                        }
                    }
                    //store
                    if (!GestioneDatiFondo.ControlsDatiArticolo2INPDAP(areaDatiFondo.DatiArticolo2, datiRecordFondo, recordDatiFondoINPDAP, datiPensione, codiceSpecificoTraduzioneSuGP, out messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiArticolo2INPDAPByIdRecordFondo(idRecordFondo, datiPensione, areaDatiFondo.DatiArticolo2, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, codiceSpecificoTraduzioneSuGP);
                }
                else
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GetDatiDBFondiByIdRecordFondo(tipoFondo, idRecordFondo, out fondoXX);
                    GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out datiRecordFondo);

                    //store
                    if (!GestioneDatiFondo.ControlsDatiArticolo2(tipoFondo, areaDatiFondo.DatiArticolo2, fondoXX, datiRecordFondo, datiPensione, out messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }
                    GestioneDatiFondo.StoreDatiArticolo2ByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, areaDatiFondo.DatiArticolo2, ref datiFondoCommon, ref fondoXX, ref lstDatiQuadroDatiRecordFondo);
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelDatiArticolo2ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            object fondoXX = null;
            List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo = null;
            DatiArticolo2ForDatiFondo datiArticolo2 = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;

            try
            {
                long idRecordFondo = areaDatiFondo.IdRecordFondo;
                GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    GestioneDatiFondo.EliminaDatiArticolo2INPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref lstDatiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP);
                    GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);
                    GestioneDatiFondo.GetDatiArticolo2INPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, out datiArticolo2);
                }
                else
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    GestioneDatiFondo.EliminaDatiArticolo2ByIdRecordFondo(idRecordFondo, tipoFondo, datiPensione, ref fondoXX, ref lstDatiQuadroDatiRecordFondo);
                    GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);
                    GestioneDatiFondo.GetDatiArticolo2ByIdRecordFondo(idRecordFondo, tipoFondo, ref fondoXX, ref datiQuadroDatiRecordFondo, out datiArticolo2);
                }

                areaDatiFondo.DatiArticolo2 = datiArticolo2;
                GetDecodifiche(ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiArticolo2

        #region Private Methods

        private void GetRegistrazioneFondoByIdRecordFondoPrivate(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref AreaDatiFondo areaDatiFondo, out string errori)
        {
            object fondoXX = null;
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = null;
            GestioneRecordFondo.DatiRecordFondo datiRecordFondo = null;
            List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon = null;
            List<GestioneCalcolo.ServizioUtile707> lServizioUtile707 = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            GestioneFondo.DatiFondo datiFondoGenerici = null;
            GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoGenerici);

            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondoGenerici != null && datiFondoGenerici.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoGenerici.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            DatiFondo datiFondo = null;
            GestioneDatiFondo.GetDatiFondoByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, ref datiRecordFondo, ref datiFondoGenerici, out datiFondo);
            if (datiFondo != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiFondo = datiFondo;
            }
            DatiCalcolo datiCalcolo = null;
            csAggiornamentoPECO_Fondi_AMG dati;
            GestioneDatiFondo.GetDatiCalcoloByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, ref lServizioUtileCommon, out datiCalcolo, out dati);
            if (datiCalcolo != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiCalcolo = datiCalcolo;
            }

            GestioneContrib.DatiCalcolo datiCalcoloDZ = null;
            csAggiornamentoPECO_Fondi_AMG datidz;
            GestioneDatiFondo.GetDatiCalcoloDZByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, datiFondoGenerici, ref datiQuadroDatiRecordFondo, ref fondoXX, ref lServizioUtileCommon, out datiCalcoloDZ, out datidz);
            if (datiCalcoloDZ != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiCalcoloDZ = datiCalcoloDZ;
            }

            DatiCalcolo707 datiCalcolo707 = null;
            GestioneDatiFondo.GetDatiCalcolo707ByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, dati, codiceSpecificoTraduzioneSuGP, false, ref datiQuadroDatiRecordFondo, ref fondoXX,
                ref lServizioUtile707, out datiCalcolo707, danteCausa, datiLavorazione, out errori);
            if (datiCalcolo707 != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiCalcolo707 = datiCalcolo707;
            }
            DatiLegge460 datiLegge460 = null;
            GestioneDatiFondo.GetDatiLegge460ByIdRecordFondo(idRecordFondo, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, out datiLegge460);
            if (datiLegge460 != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiLegge460 = datiLegge460;
            }
            DatiPrivilegiate datiPrivilegiate = null;
            GestioneDatiFondo.GetDatiPrivilegiateByIdRecordFondo(idRecordFondo, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, out datiPrivilegiate);
            if (datiPrivilegiate != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiPrivilegiate = datiPrivilegiate;
            }
            DatiArticolo2ForDatiFondo datiArticolo2 = null;
            GestioneDatiFondo.GetDatiArticolo2ByIdRecordFondo(idRecordFondo, tipoFondo, ref fondoXX, ref datiQuadroDatiRecordFondo, out datiArticolo2);
            if (datiArticolo2 != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiArticolo2 = datiArticolo2;
            }

            if (tipoFondo == Utility.TipoFondo.DZ)
            {
                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
                AreaDatiContributivi areaDatiContrib = new AreaDatiContributivi();
                GetCrossProperties(datiPensione, isRiaperturaDomanda, datiFondoGenerici, fondoXX, ref areaDatiContrib, areaDatiFondo.DatiCalcoloDZ, danteCausa, null, tipoFondo);
                areaDatiFondo.CrossDataDZ = areaDatiContrib;
                areaDatiFondo.TipoPensione = Utility.GetTipoPensione(datiPensione);
            }
            else
            {
                GetCrossProperties(datiPensione, datiFondoGenerici, ref areaDatiFondo);
            }
            GetDecodifiche(ref areaDatiFondo);
        }

        private void GetRegistrazioneFondoINPDAPByIdRecordFondoPrivate(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref AreaDatiFondo areaDatiFondo, out string errori)
        {
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = null;
            GestioneRecordFondo.DatiRecordFondo datiRecordFondo = null;
            List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lServizioUtileCommon = null;
            List<GestioneCalcolo.ServizioUtileINPDAP707> lServizioUtile707 = null;
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = null;
            GestioneFondo.DatiFondo datiFondoGenerici = null;

            GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoGenerici);

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondoGenerici != null && datiFondoGenerici.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoGenerici.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            DatiFondo datiFondo = new DatiFondo();
            GestioneDatiFondo.GetDatiFondoINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref datiRecordFondo, ref recordDatiFondoINPDAP, ref datiControlloFelpe, out datiFondo);
            if (datiFondo != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiFondo = datiFondo;
            }
            DatiCalcolo datiCalcolo = new DatiCalcolo();
            csAggiornamentoPECO_Fondi_AMG_INPDAP datiINPDAP = null;
            csAggiornamentoPECO_Fondi_AMG dati = null;
            GestioneDatiFondo.GetDatiCalcoloINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref lServizioUtileCommon, ref recordDatiFondoINPDAP, out datiCalcolo, out datiINPDAP, out dati);
            if (datiCalcolo != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiCalcolo = datiCalcolo;
            }
            DatiCalcolo707 datiCalcolo707 = new DatiCalcolo707();
            GestioneDatiFondo.GetDatiCalcoloINPDAP707ByIdRecordFondo(idRecordFondo, datiPensione, datiINPDAP, dati, codiceSpecificoTraduzioneSuGP, ref datiQuadroDatiRecordFondo, ref lServizioUtile707,
                ref recordDatiFondoINPDAP, out datiCalcolo707, out errori);
            if (datiCalcolo707 != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiCalcolo707 = datiCalcolo707;
            }

            DatiPrivilegiate datiPrivilegiate = new DatiPrivilegiate();
            GestioneDatiFondo.GetDatiPrivilegiateINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, out datiPrivilegiate);
            if (datiPrivilegiate != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiPrivilegiate = datiPrivilegiate;
            }

            DatiArticolo2ForDatiFondo datiArticolo2 = new DatiArticolo2ForDatiFondo();
            GestioneDatiFondo.GetDatiArticolo2INPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, out datiArticolo2);
            if (datiArticolo2 != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiArticolo2 = datiArticolo2;
            }

            DatiLegge460 datiLegge460 = new DatiLegge460();
            GestioneDatiFondo.GetDatiLegge460INPDAPByIdRecordFondo(idRecordFondo, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, out datiLegge460);
            if (datiLegge460 != null)
            {
                if (areaDatiFondo == null)
                    areaDatiFondo = new AreaDatiFondo();
                areaDatiFondo.DatiLegge460 = datiLegge460;
            }

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
            {
                List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> datiQuoteMiglioramentiContrattuali = null;
                GestioneMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiByIdPensione(datiPensione.Id, out datiQuoteMiglioramentiContrattuali);
                areaDatiFondo.QuoteMiglioramentiContrattuali = new Entity.CrossEntity.DatiMiglioramentiContrattuali();
                areaDatiFondo.QuoteMiglioramentiContrattuali.LDatiQuoteMiglioramentiContrattuali = new List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali>();
                if (datiQuoteMiglioramentiContrattuali != null && datiQuoteMiglioramentiContrattuali.Count > 0) //per il momento visibile solo se già presenti dati.
                {
                    if (!areaDatiFondo.QuoteMiglioramentiContrattuali.Semaforo.HasValue) areaDatiFondo.QuoteMiglioramentiContrattuali.Semaforo = 1; //in caso di abilitazione modifica del quadro, andrebbe mostrato solo sulla prima riga

                    foreach (var quota in datiQuoteMiglioramentiContrattuali)
                    {
                        areaDatiFondo.QuoteMiglioramentiContrattuali.LDatiQuoteMiglioramentiContrattuali.Add(
                            new GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali
                            {
                                Codice = quota.Codice,
                                DataDecorrenza = quota.DataDecorrenza,
                                Id = quota.Id,
                                IdPensione = quota.IdPensione,
                                IsStorico = quota.IsStorico,
                                Quota = quota.Quota
                            });
                    }
                }
            }
            GetCrossProperties(datiPensione, datiFondoGenerici, ref areaDatiFondo);
            GetDecodifiche(ref areaDatiFondo);
        }

        private void GetRegistrazioneFondoForAddOperation(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref object fondoXX, ref List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon,
            ref List<GestioneCalcolo.ServizioUtile707> lServizioUtile707, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref GestioneFondo.DatiFondo datiFondoGenerici, out AreaDatiFondo areaDatiFondo, GestioneDanteCausa.DatiDanteCausa danteCausa,
            GestioneLavorazione.DatiLavorazione datiLavorazione, out string errori)
        {
            csAggiornamentoPECO_Fondi_AMG dati = null;
            areaDatiFondo = new AreaDatiFondo();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondoGenerici != null && datiFondoGenerici.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    byte codiceSpecifico = datiFondoGenerici.CodiceSpecifico.Value;
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == codiceSpecifico);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            GestioneRecordFondo.DatiRecordFondo datiRecordFondo = new GestioneRecordFondo.DatiRecordFondo();
            DatiFondo datiFondo = new DatiFondo();
            GestioneDatiFondo.GetDatiFondoByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, ref datiRecordFondo, ref datiFondoGenerici, out datiFondo);
            areaDatiFondo.DatiFondo = datiFondo;

            GestioneContrib.DatiCalcolo datiCalcoloDZ = null;
            GestioneDatiFondo.GetDatiCalcoloDZByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, datiFondoGenerici, ref datiQuadroDatiRecordFondo, ref fondoXX, ref lServizioUtileCommon, out datiCalcoloDZ, out dati);
            areaDatiFondo.DatiCalcoloDZ = datiCalcoloDZ;

            DatiCalcolo datiCalcolo = null;
            GestioneDatiFondo.GetDatiCalcoloByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, ref lServizioUtileCommon, out datiCalcolo, out dati);
            areaDatiFondo.DatiCalcolo = datiCalcolo;

            DatiCalcolo707 datiCalcolo707 = null;
            GestioneDatiFondo.GetDatiCalcolo707ByIdRecordFondo(idRecordFondo, datiPensione, tipoFondo, dati, codiceSpecificoTraduzioneSuGP, false, ref datiQuadroDatiRecordFondo, ref fondoXX, ref lServizioUtile707, out datiCalcolo707, danteCausa, datiLavorazione, out errori);
            areaDatiFondo.DatiCalcolo707 = datiCalcolo707;

            DatiLegge460 datiLegge460 = new DatiLegge460();
            datiLegge460.Semaforo = datiQuadroDatiRecordFondo.TabLegge460;
            areaDatiFondo.DatiLegge460 = datiLegge460;

            DatiPrivilegiate datiPrivilegiate = null;
            GestioneDatiFondo.GetDatiPrivilegiateByIdRecordFondo(idRecordFondo, tipoFondo, ref datiQuadroDatiRecordFondo, ref fondoXX, out datiPrivilegiate);
            areaDatiFondo.DatiPrivilegiate = datiPrivilegiate;

            DatiArticolo2ForDatiFondo datiArticolo2 = new DatiArticolo2ForDatiFondo();
            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
            areaDatiFondo.DatiArticolo2 = datiArticolo2;

            areaDatiFondo.IdRecordFondo = idRecordFondo;
            GetDecodifiche(ref areaDatiFondo);
            GetCrossProperties(datiPensione, datiFondoGenerici, ref areaDatiFondo);

            // Se aggiungo un record significa che non è il primo
            areaDatiFondo.IsPrimoRecord = false;
        }

        private void GetRegistrazioneFondoForAddOperationINPDAP(long idRecordFondo, GestionePensione.DatiPensione datiPensione,
            ref List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lServizioUtileCommon, ref List<GestioneCalcolo.ServizioUtileINPDAP707> lServizioUtileINPDAP707,
            ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            ref GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe, out AreaDatiFondo areaDatiFondo, out string errori)
        {
            csAggiornamentoPECO_Fondi_AMG_INPDAP datiINPDAP = null;
            csAggiornamentoPECO_Fondi_AMG dati = null;
            areaDatiFondo = new AreaDatiFondo();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            GestioneFondo.DatiFondo datiFondoGenerici = null;

            GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoGenerici);

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondoGenerici != null && datiFondoGenerici.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoGenerici.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            GestioneRecordFondo.DatiRecordFondo datiRecordFondo = new GestioneRecordFondo.DatiRecordFondo();
            DatiFondo datiFondo = new DatiFondo();
            GestioneDatiFondo.GetDatiFondoINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref datiRecordFondo, ref recordDatiFondoINPDAP, ref datiControlloFelpe, out datiFondo);
            areaDatiFondo.DatiFondo = datiFondo;

            DatiCalcolo datiCalcolo = null;
            GestioneDatiFondo.GetDatiCalcoloINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref lServizioUtileCommon, ref recordDatiFondoINPDAP, out datiCalcolo, out datiINPDAP, out dati);
            areaDatiFondo.DatiCalcolo = datiCalcolo;

            DatiCalcolo707 datiCalcolo707 = null;
            GestioneDatiFondo.GetDatiCalcoloINPDAP707ByIdRecordFondo(idRecordFondo, datiPensione, datiINPDAP, dati, codiceSpecificoTraduzioneSuGP, ref datiQuadroDatiRecordFondo, ref lServizioUtileINPDAP707,
                ref recordDatiFondoINPDAP, out datiCalcolo707, out errori);
            areaDatiFondo.DatiCalcolo707 = datiCalcolo707;

            DatiPrivilegiate datiPrivilegiate = null;
            GestioneDatiFondo.GetDatiPrivilegiateINPDAPByIdRecordFondo(idRecordFondo, datiPensione, ref datiQuadroDatiRecordFondo, ref recordDatiFondoINPDAP, out datiPrivilegiate);
            areaDatiFondo.DatiPrivilegiate = datiPrivilegiate;

            DatiArticolo2ForDatiFondo datiArticolo2 = new DatiArticolo2ForDatiFondo();
            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
            areaDatiFondo.DatiArticolo2 = datiArticolo2;

            DatiLegge460 datiLegge460 = new DatiLegge460();
            datiLegge460.Semaforo = datiQuadroDatiRecordFondo.TabLegge460;
            areaDatiFondo.DatiLegge460 = datiLegge460;

            areaDatiFondo.IdRecordFondo = idRecordFondo;
            GetCrossProperties(datiPensione, datiFondoGenerici, ref areaDatiFondo);

            // Se aggiungo un record significa che non è il primo
            areaDatiFondo.IsPrimoRecord = false;
        }

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, ref AreaDatiFondo areaDatiFondo)
        {
            DateTime? decorrenzaPensioneDirettaDC = null;

            Dictionary<string, bool?> getCrossProperties = GestioneDatiFondo.GetCrossProperties(datiPensione, datiFondo, out decorrenzaPensioneDirettaDC);

            areaDatiFondo.IsDecPensAnteAgosto95 = getCrossProperties["IsDecPensAnteAgosto95"];
            areaDatiFondo.DecorrenzaPensioneDirettaDC = decorrenzaPensioneDirettaDC;
            areaDatiFondo.IsContribL214Visible = getCrossProperties["ContribL214Visible"];
            areaDatiFondo.IsDomandaSperimentaleDonna = getCrossProperties["IsDomandaSperimentaleDonna"];
            areaDatiFondo.IsPensioneTipoContributivo = getCrossProperties["IsPensioneTipoContributivo"];
            areaDatiFondo.IsPensioneTipoContributivoConOpzione = getCrossProperties["IsPensioneTipoContributivoConOpzione"];

            areaDatiFondo.CrossDataDZ = new AreaDatiContributivi();
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible = getCrossProperties["IsRiduzioneRetributiva"];
            areaDatiFondo.CrossDataDZ.IsContribL214Visible = getCrossProperties["ContribL214Visible"];

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
            areaDatiFondo.TipoReversibilita = datiLavorazione != null ? datiLavorazione.TipoReversibilita : null;
            areaDatiFondo.FineAssicurazione = datiPensione.FineAssicurazione;
            //valorizzo 
        }

        private void ValorizzaSemaforiTabDatiFondoByIdRecordFondo(long idRecordFondo, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref AreaDatiFondo areaDatiFondo,
            GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, Utility.TipoFondo? tipoFondo, GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneLavorazione.DatiLavorazione datiLavorazione)
        {
            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (areaDatiFondo == null)
                areaDatiFondo = new AreaDatiFondo();

            if (areaDatiFondo.DatiFondo == null)
                areaDatiFondo.DatiFondo = new DatiFondo();
            areaDatiFondo.DatiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiFondo;

            if (areaDatiFondo.DatiCalcolo == null)
                areaDatiFondo.DatiCalcolo = new DatiCalcolo();
            areaDatiFondo.DatiCalcolo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo;

            if (areaDatiFondo.DatiLegge460 == null)
                areaDatiFondo.DatiLegge460 = new DatiLegge460();
            areaDatiFondo.DatiLegge460.Semaforo = datiQuadroDatiRecordFondo.TabLegge460;

            if (areaDatiFondo.DatiPrivilegiate == null)
                areaDatiFondo.DatiPrivilegiate = new DatiPrivilegiate();
            areaDatiFondo.DatiPrivilegiate.Semaforo = datiQuadroDatiRecordFondo.TabPrivilegiate;

            if (areaDatiFondo.DatiArticolo2 == null)
                areaDatiFondo.DatiArticolo2 = new DatiArticolo2ForDatiFondo();
            areaDatiFondo.DatiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;

            if (areaDatiFondo.DatiCalcolo707 == null)
                areaDatiFondo.DatiCalcolo707 = new DatiCalcolo707();
            if (!datiQuadroDatiRecordFondo.TabDatiCalcolo707.HasValue &&
                GestioneContrib.IsSettimane707Visible(datiPensione, codiceSpecificoTraduzioneSuGP, areaDatiFondo.DatiCalcolo != null ? areaDatiFondo.DatiCalcolo.IsQuotaDL214Presente() : false) &&
                ((tipoFondo.HasValue && new List<Utility.TipoFondo> { Utility.TipoFondo.FS, Utility.TipoFondo.PT }.Contains(tipoFondo.Value) &&
                (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione)) || Utility.IsDomandaINPDAP(datiPensione.Gestione)))
                areaDatiFondo.DatiCalcolo707.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo707 = 0;
            else
                areaDatiFondo.DatiCalcolo707.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo707;
        }

        private void GetDecodifiche(ref AreaDatiFondo areaDatiFondo)
        {
            List<CodicePensioniPrivilegiate> listaCodicePensioniPrivilegiate = null;
            GestioneMaggiorazioniBenefici.GetListaCodicePensioniPrivilegiate(out listaCodicePensioniPrivilegiate);
            if (listaCodicePensioniPrivilegiate != null)
            {
                areaDatiFondo.ListaCodicePensioniPrivilegiate = listaCodicePensioniPrivilegiate;
            }
        }

        #endregion Private Methods

        #endregion AreaDatiFondo

        #region AreaDatiNoCalcolo

        public AreaEsito GetQuadroDatiRecordNoCalcoloByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaNoCalcolo areaNoCalcolo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            areaNoCalcolo = new AreaNoCalcolo();

            GestionePensione.DatiPensione datiPensione = null;
            List<DatiRecordNoCalcolo> lstDatiRecordNoCalcolo = null;
            DatiNoCalcolo entityDatiNoCalcolo;
            long? idRecord;
            try
            {
                datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
                GestioneAreaNoCalcolo.GetRecordNoCalcolo(datiPensione, out lstDatiRecordNoCalcolo);
                areaNoCalcolo.LstRecordNoCalcolo = lstDatiRecordNoCalcolo;
                //se non ci sono record inseriti ne viene aggiunto uno e viene immediatamente mostrato.
                if (lstDatiRecordNoCalcolo == null || lstDatiRecordNoCalcolo.Count == 0)
                {
                    GestioneAreaNoCalcolo.AddRecordNoCalcolo(datiPensione, out idRecord, out entityDatiNoCalcolo);
                    areaNoCalcolo.IdRecordNoCalcolo = idRecord;
                    areaNoCalcolo.DatiNoCalcolo = entityDatiNoCalcolo;
                }
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito AddRecordNoCalcoloByDomanda(long numeroDomanda, out AreaNoCalcolo areaDatiNoCalcolo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            areaDatiNoCalcolo = new AreaNoCalcolo();
            GestionePensione.DatiPensione datiPensione = null;
            DatiNoCalcolo recordNoCalcolo = null;
            long? idRecordNoCalcolo = -1;
            List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstRecordNoCalcolo = null;
            try
            {
                //get
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                GestioneDatiNoCalcolo.GetRecordNoCalcoloByIdPensione(datiPensione.Id, out lstRecordNoCalcolo);
                //crossproperties
                GetCrossProperties(datiPensione, ref areaDatiNoCalcolo);
                //controls
                if (!GestioneAreaNoCalcolo.ControlAddRecordNoCalcolo(lstRecordNoCalcolo, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                //operation
                GestioneAreaNoCalcolo.AddRecordNoCalcolo(datiPensione, out idRecordNoCalcolo, out recordNoCalcolo);
                areaDatiNoCalcolo.IdRecordNoCalcolo = idRecordNoCalcolo;
                areaDatiNoCalcolo.DatiNoCalcolo = recordNoCalcolo;

            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito GetDatiNoCalcoloByIdRecord(AreaRichiestaDomanda areaRichiestaDomanda, long idRecord, out AreaNoCalcolo areaDatiNoCalcolo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            areaDatiNoCalcolo = new AreaNoCalcolo();
            GestionePensione.DatiPensione datiPensione = null;
            DatiNoCalcolo recordNoCalcolo = null;

            try
            {
                //get
                datiPensione = GetDatiPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);
                //crossproperties
                GetCrossProperties(datiPensione, ref areaDatiNoCalcolo);
                //operation
                GestioneAreaNoCalcolo.GetDatiNoCalcolo(datiPensione, idRecord, out recordNoCalcolo);
                areaDatiNoCalcolo.DatiNoCalcolo = recordNoCalcolo;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito StoreDatiNoCalcolo(long numeroDomanda, long idRecord, ref AreaNoCalcolo areaDatiNoCalcolo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            string messaggioVideo = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            DatiNoCalcolo datiNoCalcolo = areaDatiNoCalcolo.DatiNoCalcolo;
            List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstRecordNoCalcolo = null;
            List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo = null;
            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
            try
            {
                //get
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                GestioneDatiNoCalcolo.GetRecordNoCalcoloByIdPensione(datiPensione.Id, out lstRecordNoCalcolo);
                GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstRecordFondo);
                //crossproperties
                GetCrossProperties(datiPensione, ref areaDatiNoCalcolo);
                //controls
                List<string> lstCodFiscSelected = null;
                if (areaDatiNoCalcolo.DatiNoCalcolo.ListaComponentiFamiliari != null && areaDatiNoCalcolo.DatiNoCalcolo.ListaComponentiFamiliari.Count > 0)
                    lstCodFiscSelected = areaDatiNoCalcolo.DatiNoCalcolo.ListaComponentiFamiliari.Select(x => x.CodiceFiscale).ToList();

                if (!GestioneAreaNoCalcolo.ControlsDatiNoCalcolo(idRecord, areaDatiNoCalcolo.DatiNoCalcolo, datiPensione, lstRecordNoCalcolo, lstRecordFondo, out messaggioVideo)
                    || !GestioneControlli.ControlsFamiliari(datiPensione, areaDatiNoCalcolo.DatiNoCalcolo.Decorrenza, lstCodFiscSelected, ref listaFamiliari, ref listaAnagrafiche, ref listaCodMaggFamiliari, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                GestioneAreaNoCalcolo.StoreDatiNoCalcolo(datiPensione, idRecord, ref datiNoCalcolo);
                areaDatiNoCalcolo.DatiNoCalcolo = datiNoCalcolo;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito DeleteDatiNoCalcolo(long numeroDomanda, long idRecord, out AreaNoCalcolo areaDatiNoCalcolo)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = null;
            DatiNoCalcolo datiNoCalcolo = null;
            areaDatiNoCalcolo = new AreaNoCalcolo();

            try
            {
                //get
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                //crossproperties
                GetCrossProperties(datiPensione, ref areaDatiNoCalcolo);
                //operation
                GestioneAreaNoCalcolo.DeleteDatiNoCalcolo(datiPensione, idRecord, out datiNoCalcolo);
                GestioneAreaNoCalcolo.ValorizzaEntityComponentiFamiliari(datiPensione, idRecord, ref datiNoCalcolo);
                areaDatiNoCalcolo.DatiNoCalcolo = datiNoCalcolo;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelRecordDatiNoCalcolo(long numeroDomanda, long idRecord, out AreaNoCalcolo areaDatiNoCalcolo)
        {
            SetCulture();

            areaDatiNoCalcolo = new AreaNoCalcolo();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = null;
            List<DatiRecordNoCalcolo> lstDatiRecordNoCalcolo = null;
            try
            {
                //get
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                GestioneAreaNoCalcolo.DeleteRecordNoCalcolo(datiPensione, idRecord);
                //operation
                GestioneAreaNoCalcolo.GetRecordNoCalcolo(datiPensione, out lstDatiRecordNoCalcolo);
                areaDatiNoCalcolo.LstRecordNoCalcolo = lstDatiRecordNoCalcolo;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        public AreaEsito CancelAllRecordDatiNoCalcolo(long numeroDomanda, out AreaNoCalcolo areaDatiNoCalcolo)
        {
            SetCulture();

            areaDatiNoCalcolo = new AreaNoCalcolo();
            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = null;
            List<DatiRecordNoCalcolo> lstDatiRecordNoCalcolo = null;
            try
            {
                //get
                datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
                GestioneAreaNoCalcolo.DeleteAllRecordNoCalcolo(datiPensione);
                GestioneAreaNoCalcolo.GetRecordNoCalcolo(datiPensione, out lstDatiRecordNoCalcolo);
                areaDatiNoCalcolo.LstRecordNoCalcolo = lstDatiRecordNoCalcolo;
            }
            catch (Exception ex)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            return Esito;
        }

        #region Cross Properties
        public void GetCrossProperties(GestionePensione.DatiPensione datiPensione, ref AreaNoCalcolo area)
        {
            if (area == null)
                area = new AreaNoCalcolo();

            area.CategoriaPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

        }
        #endregion Cross Properties


        #endregion AreaDatiNoCalcolo

    }
}
