using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneCi.Entity;
using INPS.Pensioni.LiquidazioneCi.Service.Contracts.DataContracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace INPS.Pensioni.LiquidazioneCi.Service
{
    [INPS.DNA.Exceptions.Services.ExceptionShielding]
    public class ServizioLiquidazioneCi : INPS.DNA.Services.ServiceBase, IServizioLiquidazioneCi
    {
        #region Culture
        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("it-IT");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("it-IT");
        }
        #endregion Culture

        #region Get Dati Pensione

        public long GetIdPensioneByNumeroDomanda(long numeroDomanda, byte? progStorico)
        {
            long IdPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(numeroDomanda, progStorico, out IdPensione);
            return IdPensione;
        }

        public GestionePensione.DatiPensione GetDatiPensioneByNumeroDomanda(long numeroDomanda, byte? progStorico)
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);
            return datiPensione;
        }

        #endregion Get Dati Pensione

        #region Versioni
        public AreaEsito GetListaVersioniCI(out AreaVersioni areaVersioni)
        {
            SetCulture();

            areaVersioni = new AreaVersioni();
            areaVersioni.ListaVersioni = new Dictionary<string, string>();
            AreaEsito esito = new AreaEsito();

            try
            {
                List<GestioneVersioni.DatiVersioni> elencoVersioni = null;
                GestioneVersioni.GetVersioni(out elencoVersioni);

                Utility.GetListaVersioni(ref elencoVersioni, Utility.ChiaviVersioni.WCFCI, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision);

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

        #region DatiContributivi
        public AreaEsito GetDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
            out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

            AreaEsito Esito = new AreaEsito();
            bool IsDataProRataFromDB = false;
            areaDatiContributivi = new AreaDatiContributivi();
            string messaggioVideo = "";

            #region GetDatiPECO
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiContributivi);

            List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiRetributivi);

            GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec = null;
            // se non è presente alcun dato contr e retr sul db invoco il service
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                ((ldatiContributivi == null && ldatiRetributivi == null) || listaPrestazioniEstere == null || listaPrestazioniEstere.Count == 0))
            {
                string errori = string.Empty;
                GestioneAggiornamentoPECO.GetDatiTotali(datiPensione, out datiAggPec, out errori);
                if (!String.IsNullOrEmpty(errori))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = errori;
                    return Esito;
                }
            }
            #endregion GetDatiPECO

            #region ProRata
            List<GestioneContrib.StatoEstero> elencoStatiEsteri = null;
            string cittadinanzaTitolare = string.Empty;
            GestioneContrib.GetStatiEsteri(datiPensione, listaPrestazioniEstere, datiAggPec, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out elencoStatiEsteri, out cittadinanzaTitolare,
                out IsDataProRataFromDB, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            if (elencoStatiEsteri != null)
            {
                areaDatiContributivi.ProRata = new GestioneContrib.ProRata();
                areaDatiContributivi.ProRata.ElencoStatiEsteri = new List<GestioneContrib.StatoEstero>();
                areaDatiContributivi.ProRata.ElencoStatiEsteri = elencoStatiEsteri;
                areaDatiContributivi.ProRata.IsDataFromDB = IsDataProRataFromDB;
            }
            #endregion ProRata

            #region Decodifica
            List<GestioneContrib.CodiceConvenzione> codiceConvenzione = null;
            GestioneContrib.GetListaCodiceConvenzione(out codiceConvenzione);
            if (codiceConvenzione != null && codiceConvenzione.Count > 0)
            {
                if (areaDatiContributivi == null)
                    areaDatiContributivi = new AreaDatiContributivi();
                //if (areaDatiContributivi.ProRata == null)
                //    areaDatiContributivi.ProRata = new GestioneDatiContributivi.ProRata();
                //areaDatiContributivi.ProRata.ElencoCodiceConvenzione = new List<GestioneDatiContributivi.CodiceConvenzione>();
                //areaDatiContributivi.ProRata.ElencoCodiceConvenzione = codiceConvenzione;
                areaDatiContributivi.ElencoCodiceConvenzione = new List<GestioneContrib.CodiceConvenzione>();
                areaDatiContributivi.ElencoCodiceConvenzione = codiceConvenzione;

            }

            List<GestioneContrib.CodiceVirtuale> codiceVirtuale = null;
            GestioneContrib.GetListaCodiceVirtuale(out codiceVirtuale);
            if (codiceVirtuale != null && codiceVirtuale.Count > 0)
            {
                if (areaDatiContributivi == null)
                    areaDatiContributivi = new AreaDatiContributivi();
                //if (areaDatiContributivi.ProRata == null)
                //    areaDatiContributivi.ProRata = new GestioneDatiContributivi.ProRata();
                //areaDatiContributivi.ProRata.ElencoCodiceVirtuale = new List<GestioneDatiContributivi.CodiceVirtuale>();
                //areaDatiContributivi.ProRata.ElencoCodiceVirtuale = codiceVirtuale;
                areaDatiContributivi.ElencoCodiceVirtuale = new List<GestioneContrib.CodiceVirtuale>();
                areaDatiContributivi.ElencoCodiceVirtuale = codiceVirtuale;
            }

            List<GestioneContrib.RegimeLiquidazione> regimeLiquidazione = null;
            GestioneContrib.GetListaRegimeLiquidazione(out regimeLiquidazione);
            if (regimeLiquidazione != null && regimeLiquidazione.Count > 0)
            {
                if (areaDatiContributivi == null)
                    areaDatiContributivi = new AreaDatiContributivi();
                //if (areaDatiContributivi.ProRata == null)
                //    areaDatiContributivi.ProRata = new GestioneDatiContributivi.ProRata();
                //areaDatiContributivi.ProRata.ElencoRegimeLiquidazione = new List<GestioneDatiContributivi.RegimeLiquidazione>();
                //areaDatiContributivi.ProRata.ElencoRegimeLiquidazione = regimeLiquidazione;
                areaDatiContributivi.ElencoRegimeLiquidazione = new List<GestioneContrib.RegimeLiquidazione>();
                areaDatiContributivi.ElencoRegimeLiquidazione = regimeLiquidazione;

            }

            GetListeDecodifica(datiPensione, ref areaDatiContributivi);

            #endregion Decodifica

            #region DatiCalcolo
            GestioneContrib.DatiCalcolo datiCalcolo = null;
            GestioneContrib.GetDatiCalcoloByDatiPensione(datiPensione, ldatiContributivi, ldatiRetributivi, datiAggPec, out datiCalcolo, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            if (datiCalcolo != null)
            {
                areaDatiContributivi.DatiCalcolo = new GestioneContrib.DatiCalcolo();
                areaDatiContributivi.DatiCalcolo = datiCalcolo;
            }

            #endregion DatiCalcolo

            #region ImportiEsteri

            List<GestioneContrib.PensioniCiImportiValuta> datiImportiEsteri = null;
            GestioneContrib.GetDatiImportiEsteriByIdPensione(datiPensione.Id, out datiImportiEsteri, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            if (datiImportiEsteri != null)
            {
                areaDatiContributivi.LimportiEsteriValuta = new List<GestioneContrib.PensioniCiImportiValuta>();
                areaDatiContributivi.LimportiEsteriValuta = datiImportiEsteri;
            }

            #endregion ImportiEsteri

            #region MaternitaAcna

            List<GestioneContrib.MaternitaAcna> LdatiMaternitaAcna = null;
            GestioneContrib.GetDatiMaternitaAcnaByIdPensione(datiPensione.Id, out LdatiMaternitaAcna);

            if (LdatiMaternitaAcna != null)
            {
                areaDatiContributivi.LMaternitaAcna = new List<GestioneContrib.MaternitaAcna>();
                areaDatiContributivi.LMaternitaAcna = LdatiMaternitaAcna;
            }

            #endregion MaternitaAcna

            #region DatiPostDecOriginaria
            List<GestioneContrib.DatiPostDecOriginaria> datiPostDecOriginaria = null;
            GestioneContrib.GetDatiPostDecOriginariaByIdPensione(datiPensione.Id, out datiPostDecOriginaria);
            if (datiPostDecOriginaria != null)
            {
                areaDatiContributivi.LDatiPostDecOriginaria = new List<GestioneContrib.DatiPostDecOriginaria>();
                areaDatiContributivi.LDatiPostDecOriginaria = datiPostDecOriginaria;
            }
            #endregion DatiPostDecOriginaria

            #region LavoratoriAutonomi

            GestioneContrib.LavoratoriAutonomi datiLavoratoriAutonomi = null;
            GestioneContrib.GetDatiLavoratoriAutonomiByIdPensione(datiPensione.Id, datiIstruttoriaCommon, out datiLavoratoriAutonomi, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            if (datiLavoratoriAutonomi != null)
            {
                areaDatiContributivi.LavoratoriAutonomi = new GestioneContrib.LavoratoriAutonomi();
                areaDatiContributivi.LavoratoriAutonomi = datiLavoratoriAutonomi;
            }

            #endregion LavoratoriAutonomi

            #region RedditiPerInegrazioneVirtuale

            List<GestioneContrib.RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtuale = null;
            GestioneContrib.GetDatiRedditiPerIntegrazioneVirtualeByIdPensione(datiPensione, out listaReddPerIntegrazioneVirtuale, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            areaDatiContributivi.LRedditiPerIntegrazioneVirtuale = new List<GestioneContrib.RedditiPerIntegrazioneVirtuale>();
            if (listaReddPerIntegrazioneVirtuale != null)
            {
                areaDatiContributivi.LRedditiPerIntegrazioneVirtuale = listaReddPerIntegrazioneVirtuale;
            }

            #endregion RedditiPerInegrazioneVirtuale

            areaDatiContributivi.IsFineAssicurazionePost2012 = GestioneContrib.IsFineAssicurazionePost2012(datiPensione.FineAssicurazione);
            areaDatiContributivi.IsInizioAssicurazionePost1995 = GestioneContrib.IsInizioAssicurazionePost1995(datiPensione.InizioAssicurazione);
            areaDatiContributivi.IsSettimane707Visible = GestioneContrib.IsSettimane707Visible(datiPensione, datiCalcolo.LDatiRetributivi, datiCalcolo.LDatiContributivi, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null);
            areaDatiContributivi.IsPensioneTipoContributivo = Utility.IsDomandaTipoContributivo(datiPensione, null, false);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        public AreaEsito StoreDatiContributivi(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);

            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiProRataPrivate(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, ref datiIstruttoriaCommon, ref areaDatiContributivi, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiCalcoloPrivate(datiPensione, ref datiIstruttoriaCommon, datiNuoveLiquidate, ref datiMaggiorazioniBeneficiCommon, areaDatiContributivi, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiImportiEsteri(datiPensione, areaDatiContributivi, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiMaternitaAcna(datiPensione, areaDatiContributivi, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito = StoreDatiPostDecOriginaria(datiPensione, areaDatiContributivi, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            //StoreDatiLavoratoriAutonomi(numeroDomanda, areaDatiContributivi, false);

            Esito = StoreRedditiPerIntegrazioneVirtualePrivate(datiPensione, ref areaDatiContributivi);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        private void GetListeDecodifica(GestionePensione.DatiPensione datiPensione, ref AreaDatiContributivi areaDatiContributivi)
        {

            if (areaDatiContributivi == null)
                areaDatiContributivi = new AreaDatiContributivi();

            List<DecodificaGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivo = null;
            GestioneContrib.GetListaDecodificaGestioneCalcoloRetributivo(datiPensione, out listaDecodificaGestioneCalcoloRetributivo);
            if (listaDecodificaGestioneCalcoloRetributivo != null)
            {
                areaDatiContributivi.ListaDecodificaGestioneCalcoloRetributivo = listaDecodificaGestioneCalcoloRetributivo;
            }

            List<DecodificaGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivo = null;
            GestioneContrib.GetListaDecodificaGestioneCalcoloContributivo(datiPensione, out listaDecodificaGestioneCalcoloContributivo);
            if (listaDecodificaGestioneCalcoloContributivo != null)
            {
                areaDatiContributivi.ListaDecodificaGestioneCalcoloContributivo = listaDecodificaGestioneCalcoloContributivo;
            }


            List<DecodificaCodeGestione> listaDecodificaCodeGestione = null;
            GestioneContrib.GetListaDecodificaCodeGestione(datiPensione, out listaDecodificaCodeGestione);
            if (listaDecodificaCodeGestione != null)
            {
                areaDatiContributivi.ListaDecodificaCodeGestione = listaDecodificaCodeGestione;
            }

        }

        #region Dati ProRata
        public AreaEsito StoreDatiProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiProRataPrivate(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, ref datiIstruttoriaCommon, ref areaDatiContributivi, true);
            return Esito;
        }

        private AreaEsito StoreDatiProRataPrivate(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
            ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, ref AreaDatiContributivi areaDatiContributivi, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            if (areaDatiContributivi.ProRata == null || areaDatiContributivi.ProRata.ElencoStatiEsteri == null)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Nessuno Stato Estero Presente. Non è possibile procedere con il salvataggio";
                return Esito;
            }

            GestioneContrib.StoreStatiEsteri(datiPensione, ref datiIstruttoriaCommon, areaDatiContributivi.ProRata.ElencoStatiEsteri, areaDatiContributivi.DatiCalcolo, singleTab, areaDatiContributivi.LRedditiPerIntegrazioneVirtuale, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            #region GetData
            List<GestioneContrib.StatoEstero> elencoStatiEsteri = null;
            string cittadinanzaTitolare = string.Empty;
            bool IsDataProRataFromDB = false;
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);
            GestioneContrib.GetStatiEsteri(datiPensione, listaPrestazioniEstere, null, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out elencoStatiEsteri, out cittadinanzaTitolare,
                out IsDataProRataFromDB, out messaggioControllo);
            if (!string.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            if (elencoStatiEsteri != null)
            {
                areaDatiContributivi.ProRata = new GestioneContrib.ProRata();
                areaDatiContributivi.ProRata.ElencoStatiEsteri = new List<GestioneContrib.StatoEstero>();
                areaDatiContributivi.ProRata.ElencoStatiEsteri = elencoStatiEsteri;
                areaDatiContributivi.ProRata.IsDataFromDB = IsDataProRataFromDB;
            }
            #endregion GetData

            return Esito;
        }

        public AreaEsito CancelProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.EliminaStatiEsteri(datiPensione, datiIstruttoriaCommon, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        #endregion Dati ProRata

        #region Dati Calcolo

        public AreaEsito StoreDatiCalcolo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);

            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiCalcoloPrivate(datiPensione, ref datiIstruttoriaCommon, datiNuoveLiquidate, ref datiMaggiorazioniBeneficiCommon, areaDatiContributivi, true);

            return Esito;
        }

        private AreaEsito StoreDatiCalcoloPrivate(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, AreaDatiContributivi areaDatiContributivi, bool singleTab)
        {
            string messaggioControllo = string.Empty;
            AreaEsito Esito = new AreaEsito();
            GestioneContrib.StoreDatiCalcoloByDatiPensione(datiPensione, ref datiIstruttoriaCommon, datiNuoveLiquidate, ref datiMaggiorazioniBeneficiCommon, areaDatiContributivi.DatiCalcolo, areaDatiContributivi.LMaternitaAcna,
                areaDatiContributivi.ProRata, singleTab, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        public AreaEsito CancelDatiCalcolo(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            string messaggioVideo = string.Empty;

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.DeleteDatiCalcoloByDatiPensione(datiPensione, out messaggioVideo);
                Esito.Messaggio = string.Empty;

                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    Esito.Messaggio = messaggioVideo;
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    return Esito;
                }
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        #endregion Dati Calcolo

        #region Importi Esteri

        public AreaEsito StoreDatiImportiEsteri(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiImportiEsteri(datiPensione, areaDatiContributivi, true);

            return Esito;
        }

        public AreaEsito CancelDatiImportiEsteri(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.CancelDatiImportiEsteri(datiPensione);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        private AreaEsito StoreDatiImportiEsteri(GestionePensione.DatiPensione datiPensione, AreaDatiContributivi areaDatiContributivi, bool IsSingleTabSaved)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiImportiEsteri(datiPensione, areaDatiContributivi.LimportiEsteriValuta, areaDatiContributivi.ProRata, IsSingleTabSaved, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        #endregion Importi Esteri

        #region StatiEsteri
        public AreaEsito GetStatiEsteri(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
            out List<GestioneContrib.StatoEstero> elencoStatiEsteri, out string cittadinanzaTitolare)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            elencoStatiEsteri = null;
            cittadinanzaTitolare = string.Empty;
            string messaggioVideo = "";

            GestioneContrib.GetStatiEsteriFromService(numeroDomanda, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore,
                out elencoStatiEsteri, out cittadinanzaTitolare, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }
        //ENG - RIC/TRF: aggiunta la gestione per il recupero degli stati(se presenti e diversi da quelli provenienti da prelievo) dal servizio Naci o AllegatiConvenzioni
        public AreaEsito GetStatiEsteriRicTrf(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
           out List<GestioneContrib.StatoEstero> elencoStatiEsteri, out string cittadinanzaTitolare)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            elencoStatiEsteri = null;
            cittadinanzaTitolare = string.Empty;
            string messaggioVideo = "";
        
            GestioneContrib.GetStatiEsteriFromServiceRicTrf(numeroDomanda, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore,
                out elencoStatiEsteri, out cittadinanzaTitolare, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }
        #endregion StatiEsteri

        #region Maternita/Acna

        public AreaEsito StoreDatiMaternitaAcna(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiMaternitaAcna(datiPensione, areaDatiContributivi, true);

            return Esito;
        }

        public AreaEsito CancelDatiMaternitaAcna(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.CancelDatiMaternitaAcna(datiPensione);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        private AreaEsito StoreDatiMaternitaAcna(GestionePensione.DatiPensione datiPensione, AreaDatiContributivi areaDatiContributivi, bool IsSingleTabSaved)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiMaternitaAcna(datiPensione, areaDatiContributivi.LMaternitaAcna, IsSingleTabSaved, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }

        #endregion Maternita/Acna

        #region DatiPostDecOriginaria
        public AreaEsito StoreDatiPostDecOriginaria(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreDatiPostDecOriginaria(datiPensione, areaDatiContributivi, true);

            return Esito;
        }

        public AreaEsito CancelDatiPostDecOriginaria(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.CancelDatiPostDecOriginaria(datiPensione);

                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        private AreaEsito StoreDatiPostDecOriginaria(GestionePensione.DatiPensione datiPensione, AreaDatiContributivi areaDatiContributivi, bool IsSingleTabSaved)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiPostDecOriginaria(datiPensione, areaDatiContributivi.LDatiPostDecOriginaria, IsSingleTabSaved, out messaggioControllo);
            if (!string.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }
        #endregion DatiPostDecOriginaria

        #region Lavoratori Autonomi

        public AreaEsito StoreDatiLavoratoriAutonomi(long numeroDomanda, AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiLavoratoriAutonomi(datiPensione, ref datiIstruttoriaCommon, areaDatiContributivi, true);
            return Esito;
        }

        public AreaEsito CancelDatiLavoratoriAutonomi(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);

            AreaEsito Esito = new AreaEsito();
            try
            {
                GestioneContrib.CancelDatiLavoratoriAutonomi(datiPensione, ref datiIstruttoriaCommon);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        private AreaEsito StoreDatiLavoratoriAutonomi(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, AreaDatiContributivi areaDatiContributivi, bool IsSingleTabSaved)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiLavoratoriAutonomi(datiPensione, areaDatiContributivi.LavoratoriAutonomi, ref datiIstruttoriaCommon, IsSingleTabSaved, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }

            return Esito;
        }
        #endregion Lavoratori Autonomi

        #region RedditiPerIntegrazioneVirtuale

        public AreaEsito StoreRedditiPerIntegrazioneVirtuale(long numeroDomanda, ref AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();

            Esito = StoreRedditiPerIntegrazioneVirtualePrivate(datiPensione, ref areaDatiContributivi);

            return Esito;
        }

        public AreaEsito CancelRedditiPerIntegrazioneVirtuale(long numeroDomanda, out AreaDatiContributivi areaDatiContributivi)
        {
            SetCulture();

            string messaggioVideo = "";

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            List<GestioneContrib.RedditiPerIntegrazioneVirtuale> listaRedditiPerIntegrazioneVirtuale = null;

            AreaEsito Esito = new AreaEsito();
            areaDatiContributivi = new AreaDatiContributivi();
            try
            {
                GestioneContrib.CancelDatiRedditiPerIntegrazioneVirtuale(datiPensione);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
                GestioneContrib.GetDatiRedditiPerIntegrazioneVirtualeByIdPensione(datiPensione, out listaRedditiPerIntegrazioneVirtuale, out messaggioVideo);

                areaDatiContributivi.LRedditiPerIntegrazioneVirtuale = new List<GestioneContrib.RedditiPerIntegrazioneVirtuale>();
                if (listaRedditiPerIntegrazioneVirtuale != null)
                {
                    areaDatiContributivi.LRedditiPerIntegrazioneVirtuale = listaRedditiPerIntegrazioneVirtuale;
                }
            }
            catch (Exception)
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Attenzione non è stato possibile completare la cancellazione.";
            }
            return Esito;
        }

        private AreaEsito StoreRedditiPerIntegrazioneVirtualePrivate(GestionePensione.DatiPensione datiPensione, ref AreaDatiContributivi areaDatiContributivi)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestioneContrib.StoreDatiRedditiPerIntegrazioneVirtuale(datiPensione, areaDatiContributivi.LRedditiPerIntegrazioneVirtuale, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
            }
            else
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        #endregion RedditiPerIntegrazioneVirtuale

        #endregion DatiContributivi

        #region Calcolo
        public AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out string statoPensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            statoPensione = string.Empty;
            listaConsultazioniANF = null;
            AreaEsito Esito = new AreaEsito();
            try
            {
                string messaggioVideo;
                bool esito = false;

                if (!GestioneCalcoloDomanda.ControlsDatiCalcolaDomanda(datiPensione, matricolaOperatore, isConsultazioniANFVerificate, out listaConsultazioniANF, out messaggioVideo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioVideo;
                    return Esito;
                }
                DateTime dataSistema = Utility.DataSistemaCi;
                GestioneControlliDinamici.ControlloDinamico controlloDinamicoData = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioNuovoTracciato", out controlloDinamicoData);
                DateTime dataInizioNuovoTracciato = Utility.DataFromString(controlloDinamicoData.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

                if (datiPensione.IsRicRinnovata.GetValueOrDefault() || Utility.DataSuccessivaA(dataSistema, dataInizioNuovoTracciato))
                    GestioneCalcoloDomanda.CalcolaDomandaNew(datiPensione, numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out statoPensione, out esito, out messaggioVideo);
                else 
                    GestioneCalcoloDomanda.CalcolaDomanda(datiPensione, numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out statoPensione, out esito, out messaggioVideo);

                if (esito)
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                else
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioVideo;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);

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

            string messaggioVideo = "";
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
                messaggioVideo = "Errore tecnico durante il prelievo dei dati della pensione per le convenzioni internazionali";
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

        #region LiquidazionePensione

        public AreaEsito GetLiquidazionePensioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

            AreaEsito Esito = new AreaEsito();
            areaLiquidazionePensione = null;
            Entity.DatiGenerici datiGenerici = null;
            Entity.DatiAssicurativi datiAssicurativi = null;
            Entity.DatiIstruttoria datiIstruttoria = null;
            Entity.DatiOpzione datiOpzione = null;
            Entity.DatiProvenienza datiProvenienza = null;
            List<Entity.DatiInail> listaDatiInail = null;

            GestioneLiquidazionePensione.GetLiquidazionePensione(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiGenerici, out datiAssicurativi, out datiIstruttoria, out datiOpzione, out datiProvenienza, out listaDatiInail);

            if (datiGenerici != null)
            {
                areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiGenerici = datiGenerici;
            }

            if (datiAssicurativi != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiAssicurativi = datiAssicurativi;
            }

            if (datiIstruttoria != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiIstruttoria = datiIstruttoria;
            }

            if (datiOpzione != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiOpzione = datiOpzione;
            }

            if (datiProvenienza != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiProvenienza = datiProvenienza;
            }

            //ENG - Reversibilità: campi Inail
            if (listaDatiInail != null && listaDatiInail.Count > 0)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.DatiInail = listaDatiInail;
            }

            GetListeDecodifica(datiPensione, ref areaLiquidazionePensione);
            GetCrossProperties(datiPensione, datiMaggiorazioniBeneficiCommon, datiAnagrafici, datiDetrazioni, isRiaperturaDomanda, datiStoricoGP, ref areaLiquidazionePensione);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito StoreDatiLiquidazionePensione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            Entity.DatiExCombattente datiExCombattente = null;
            Entity.DatiBenefici datiBenefici = null;
            Entity.DatiMaggiorazioni datiMaggiorazioni = null;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoria, out datiMaggiorazioniBeneficiCommon, true, true);
            ValorizzaDatiForMaggiorazioniBenefici(datiPensione.Id, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            DateTime dataSistema = Utility.DataSistemaCi;

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiGenericiPrivate(datiPensione, ref datiIstruttoria, ref datiMaggiorazioniBeneficiCommon, datiEliminazione, ref datiPensioniDatiGenerici, datiBeneficioVittimeTerrorismo,
                areaLiquidazionePensione, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, false, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiAssicurativiPrivate(datiPensione, ref datiIstruttoria, ref datiMaggiorazioniBeneficiCommon, ref datiPensioniDatiGenerici, areaLiquidazionePensione, dataSistema, false, isRiaperturaDomanda,
                out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiIstruttoriaPrivate(datiPensione, ref datiIstruttoria, ref datiPensioniDatiGenerici, datiBeneficioVittimeTerrorismo, areaLiquidazionePensione, false, isRiaperturaDomanda,
                out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiOpzionePrivate(datiPensione, ref datiIstruttoria, ref datiPensioniDatiGenerici, areaLiquidazionePensione, false, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiProvenienzaPrivate(datiPensione, ref datiIstruttoria, areaLiquidazionePensione, false, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            StoreDatiInailPrivate(datiPensione, areaLiquidazionePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        #region Dati Generici

        public AreaEsito StoreDatiGenerici(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            Entity.DatiExCombattente datiExCombattente = null;
            Entity.DatiBenefici datiBenefici = null;
            Entity.DatiMaggiorazioni datiMaggiorazioni = null;
            GestionePensione.DatiPensione datiPensione = null;

            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);
            ValorizzaDatiForMaggiorazioniBenefici(datiPensione.Id, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            DateTime dataSistema = Utility.DataSistemaCi;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiGenericiPrivate(datiPensione, ref datiIstruttoriaCommon, ref datiMaggiorazioniBeneficiCommon, datiEliminazione, ref datiPensioniDatiGenerici, datiBeneficioVittimeTerrorismo,
                areaLiquidazionePensione, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, true, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiGenerici(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            Entity.DatiExCombattente datiExCombattente = null;
            Entity.DatiBenefici datiBenefici = null;
            Entity.DatiMaggiorazioni datiMaggiorazioni = null;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoria, out datiMaggiorazioniBeneficiCommon, true, true);
            ValorizzaDatiForMaggiorazioniBenefici(datiPensione.Id, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

            DateTime dataSistema = Utility.DataSistemaCi;

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;
            Entity.DatiGenerici datiGenerici = null;
            GestioneLiquidazionePensione.EliminaDatiGenerici(datiPensione, datiIstruttoria, datiMaggiorazioniBeneficiCommon, ref datiPensioniDatiGenerici, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, out msgVideo);

            GestioneLiquidazionePensione.GetDatiGenerici(datiPensione, datiIstruttoria, isRiaperturaDomanda, out datiGenerici);

            areaLiquidazionePensione = new AreaLiquidazionePensione();
            areaLiquidazionePensione.DatiGenerici = datiGenerici;

            GetListeDecodifica(datiPensione, ref areaLiquidazionePensione);
            GetCrossProperties(datiPensione, datiMaggiorazioniBeneficiCommon, datiAnagrafici, datiDetrazioni, isRiaperturaDomanda, datiStoricoGP, ref areaLiquidazionePensione);

            Esito.Messaggio = "";

            if (!String.IsNullOrEmpty(msgVideo))
                Esito.Messaggio = msgVideo;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            return Esito;
        }

        private void StoreDatiGenericiPrivate(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, GestionePensione.DatiEliminazione datiEliminazione,
            ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            AreaLiquidazionePensione areaLiquidazionePensione, Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici,
            Entity.DatiMaggiorazioni datiMaggiorazioni, DateTime dataSistema, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiGenerici(datiPensione, datiIstruttoriaCommon, datiMaggiorazioniBeneficiCommon, datiEliminazione, datiBeneficioVittimeTerrorismo,
                areaLiquidazionePensione.DatiGenerici, areaLiquidazionePensione.DatiAssicurativi, areaLiquidazionePensione.DatiOpzione, areaLiquidazionePensione.DatiIstruttoria,
                areaLiquidazionePensione.DatiProvenienza, datiExCombattente, datiBenefici, datiMaggiorazioni, IsSingleTab, isRiaperturaDomanda, out messaggioControllo);

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiGenerici(datiPensione, ref datiMaggiorazioniBeneficiCommon, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, areaLiquidazionePensione.DatiGenerici, datiExCombattente,
                datiBenefici, datiMaggiorazioni, dataSistema, false);
        }

        #endregion Dati Generici

        #region Dati Assicurativi

        public AreaEsito StoreDatiAssicurativi(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            DateTime dataSistema = Utility.DataSistemaCi;
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiAssicurativiPrivate(datiPensione, ref datiIstruttoriaCommon, ref datiMaggiorazioniBeneficiCommon, ref datiPensioniDatiGenerici, areaLiquidazionePensione, dataSistema, true,
                isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiAssicurativi(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

            AreaEsito Esito = new AreaEsito();
            string msgVideo = string.Empty;
            Entity.DatiAssicurativi datiAssicurativi = null;
            GestioneLiquidazionePensione.EliminaDatiAssicurativi(datiPensione, datiIstruttoriaCommon, ref datiPensioniDatiGenerici, out msgVideo);
            GestioneLiquidazionePensione.GetDatiAssicurativi(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiAssicurativi);

            areaLiquidazionePensione = new AreaLiquidazionePensione();
            areaLiquidazionePensione.DatiAssicurativi = datiAssicurativi;

            GetCrossProperties(datiPensione, datiMaggiorazioniBeneficiCommon, datiAnagrafici, datiDetrazioni, isRiaperturaDomanda, datiStoricoGP, ref areaLiquidazionePensione);

            Esito.Messaggio = "";
            if (!String.IsNullOrEmpty(msgVideo))
                Esito.Messaggio = msgVideo;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;

            return Esito;
        }

        private void StoreDatiAssicurativiPrivate(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            AreaLiquidazionePensione areaLiquidazionePensione, DateTime dataSistema, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiAssicurativi(datiPensione, datiIstruttoriaCommon, datiMaggiorazioniBeneficiCommon, areaLiquidazionePensione.DatiAssicurativi,
                areaLiquidazionePensione.DatiGenerici, areaLiquidazionePensione.DatiOpzione, areaLiquidazionePensione.DatiProvenienza, dataSistema, IsSingleTab, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiAssicurativi(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, areaLiquidazionePensione.DatiAssicurativi, false);
        }

        #endregion Dati Assicurativi

        #region Dati Istruttoria

        public AreaEsito StoreDatiIstruttoria(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiIstruttoriaPrivate(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, datiBeneficioVittimeTerrorismo, areaLiquidazionePensione, true, isRiaperturaDomanda,
                out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiIstruttoria(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

            AreaEsito Esito = new AreaEsito();
            Entity.DatiIstruttoria datiIstruttoria = null;

            GestioneLiquidazionePensione.EliminaDatiIstruttoria(datiPensione, datiIstruttoriaCommon, ref datiPensioniDatiGenerici);
            GestioneLiquidazionePensione.GetDatiIstruttoria(datiPensione, datiIstruttoriaCommon, out datiIstruttoria);

            areaLiquidazionePensione = new AreaLiquidazionePensione();
            areaLiquidazionePensione.DatiIstruttoria = datiIstruttoria;

            GetListeDecodifica(datiPensione, ref areaLiquidazionePensione);
            GetCrossProperties(datiPensione, datiMaggiorazioniBeneficiCommon, datiAnagrafici, datiDetrazioni, isRiaperturaDomanda, datiStoricoGP, ref areaLiquidazionePensione);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        private void StoreDatiIstruttoriaPrivate(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            AreaLiquidazionePensione areaLiquidazionePensione, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiIstruttoria(datiPensione, datiIstruttoriaCommon, datiBeneficioVittimeTerrorismo, areaLiquidazionePensione.DatiIstruttoria, areaLiquidazionePensione.DatiGenerici, IsSingleTab,
                isRiaperturaDomanda, out messaggioControllo);

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiIstruttoria(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, areaLiquidazionePensione.DatiIstruttoria, false);
        }

        #endregion Dati Istruttoria

        #region Dati Opzione

        public AreaEsito StoreDatiOpzione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiOpzionePrivate(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, areaLiquidazionePensione, true, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiOpzione(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiOpzione(datiPensione, datiIstruttoriaCommon, ref datiPensioniDatiGenerici);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        private void StoreDatiOpzionePrivate(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            AreaLiquidazionePensione areaLiquidazionePensione, bool IsSingleTab, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiOpzione(datiPensione, datiIstruttoriaCommon, areaLiquidazionePensione.DatiOpzione, areaLiquidazionePensione.DatiGenerici, areaLiquidazionePensione.DatiAssicurativi, IsSingleTab, out messaggioControllo);

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiOpzione(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, areaLiquidazionePensione.DatiOpzione, false);
        }

        #endregion Dati Opzione

        #region Dati Provenienza

        public AreaEsito StoreDatiProvenienza(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            StoreDatiProvenienzaPrivate(datiPensione, ref datiIstruttoriaCommon, areaLiquidazionePensione, true, isRiaperturaDomanda, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";
            return Esito;
        }

        public AreaEsito CancelDatiProvenienza(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, false);

            AreaEsito Esito = new AreaEsito();
            GestioneLiquidazionePensione.EliminaDatiProvenienza(datiPensione, datiIstruttoriaCommon);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = "";

            return Esito;
        }

        private void StoreDatiProvenienzaPrivate(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, AreaLiquidazionePensione areaLiquidazionePensione, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            GestioneLiquidazionePensione.ControlDatiProvenienza(datiPensione, datiIstruttoriaCommon, areaLiquidazionePensione.DatiGenerici, areaLiquidazionePensione.DatiProvenienza, areaLiquidazionePensione.DatiAssicurativi, IsSingleTab, isRiaperturaDomanda, out messaggioControllo);

            if (!String.IsNullOrEmpty(messaggioControllo))
                return;
            GestioneLiquidazionePensione.StoreDatiProvenienza(datiPensione, ref datiIstruttoriaCommon, areaLiquidazionePensione.DatiProvenienza, false);
        }

        #endregion Dati Provenienza

        #region Dati Inail
        public AreaEsito StoreDatiInail(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            SetCulture();

            DateTime dataSistema = Utility.DataSistemaCi;
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, false, false);

            StoreDatiInailPrivate(datiPensione, areaLiquidazionePensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private void StoreDatiInailPrivate(GestionePensione.DatiPensione datiPensione, AreaLiquidazionePensione areaLiquidazionePensione, out string messaggioControllo)
        {
            messaggioControllo = String.Empty; //al momento non sono previsti controlli
            GestioneLiquidazionePensione.StoreDatiInail(datiPensione, areaLiquidazionePensione.DatiInail);
        }

        public AreaEsito CancelDatiInail(long numeroDomanda)
        {
            SetCulture();

            AreaEsito Esito = new AreaEsito();
            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, false, false);
            GestioneLiquidazionePensione.EliminaDatiInail(datiPensione);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }
        #endregion Dati Inail

        private void GetListeDecodifica(GestionePensione.DatiPensione datiPensione, ref AreaLiquidazionePensione areaLiquidazionePensione)
        {
            //List<CDCMMR> listaCDCMMR = null;
            //GestioneLiquidazionePensione.GetListaCDCMMR(out listaCDCMMR);
            //if (listaCDCMMR != null)
            //{
            //    if (areaLiquidazionePensione == null)
            //        areaLiquidazionePensione = new AreaLiquidazionePensione();
            //    areaLiquidazionePensione.listaCDCMMR = listaCDCMMR;
            //}

            List<CodiceParticolare> listaCodiceParticolare = null;
            GestioneLiquidazionePensione.GetListaCodiceParticolare(datiPensione, out listaCodiceParticolare);
            if (listaCodiceParticolare != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaCodiceParticolare = listaCodiceParticolare;
            }

            List<DecodificaLegge44997> listaDecodificaLegge44997 = null;
            GestioneLiquidazionePensione.GetListaCodiceLegge44997(datiPensione, out listaDecodificaLegge44997);
            if (listaDecodificaLegge44997 != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaDecodificaLegge44997 = listaDecodificaLegge44997;
            }

            List<DomandaRicorso> listaDomandaRicorso = null;
            GestioneLiquidazionePensione.GetListaCodiciDomandaRicorso(out listaDomandaRicorso);
            if (listaDomandaRicorso != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaDomandaRicorso = listaDomandaRicorso;
            }

            List<Mobilita> listaMobilita = null;
            GestioneLiquidazionePensione.GetListaCodiciMobilita(out listaMobilita);
            if (listaMobilita != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaMobilita = listaMobilita;
            }

            List<CodiciNatura> listaCodiciNatura_AGO = null;
            GestioneLiquidazionePensione.GetListaCodicNatura(datiPensione, out listaCodiciNatura_AGO);
            if (listaCodiciNatura_AGO != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaCodiciNatura = listaCodiciNatura_AGO;
            }

            List<DecModalitaLiquidazione> listaDecModalitaLiquidazione = null;
            GestioneLiquidazionePensione.GetListaCodiceModalitaLiquidazione(out listaDecModalitaLiquidazione);
            if (listaDecModalitaLiquidazione != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaModalitaLiquidazione = listaDecModalitaLiquidazione;
            }

            List<OpzioneRiliquidazione> listaOpzioneRiliquidazione = null;
            GestioneLiquidazionePensione.GetListaOpzioneRiliquidazione(out listaOpzioneRiliquidazione);
            if (listaOpzioneRiliquidazione != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.lOpzioneRiliquidazione = listaOpzioneRiliquidazione;
            }

            List<CodiceVirtuale> listaCodiceVirtuale = null;
            GestioneLiquidazionePensione.GetListaCodiceVirtuale(out listaCodiceVirtuale);
            if (listaCodiceVirtuale != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.lCodiceVirtuale = listaCodiceVirtuale;
            }

            List<CodiceCi21> listaCodiceCi21 = null;
            GestioneLiquidazionePensione.GetListaCodiceCi21(out listaCodiceCi21);
            if (listaCodiceCi21 != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.lCodiceCi21 = listaCodiceCi21;
            }

            List<CodiceCi28> listaCodiceCi28 = null;
            GestioneLiquidazionePensione.GetListaCodiceCi28(out listaCodiceCi28);
            if (listaCodiceCi28 != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.lCodiceCi28 = listaCodiceCi28;
            }

            List<DecodificaRiconoscimentiInvalidita> listaDecodificaRiconoscimentiInvalidita = null;
            GestioneLiquidazionePensione.GetListaRiconoscimentiInvalidita(out listaDecodificaRiconoscimentiInvalidita);
            if (listaDecodificaRiconoscimentiInvalidita != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaRiconoscimentiInvalidita = listaDecodificaRiconoscimentiInvalidita;
            }

            List<CodiceRequisitiParticolari> listaCodiceRequisitiParticolari = null;
            GestioneLiquidazionePensione.GetListaCodiceRequisitiParticolari(out listaCodiceRequisitiParticolari);
            if (listaCodiceRequisitiParticolari != null)
            {
                if (areaLiquidazionePensione == null)
                    areaLiquidazionePensione = new AreaLiquidazionePensione();
                areaLiquidazionePensione.listaCodiceRequisitiParticolari = listaCodiceRequisitiParticolari;
            }
        }

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon,
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici, GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, bool isRiaperturaDomanda, GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP, ref AreaLiquidazionePensione areaLiquidazionePensione)
        {
            GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia = null;
            DateTime? DataPrelievoDomanda = null;
            Dictionary<string, bool?> lCrossProperties = GestioneLiquidazionePensione.GetCrossProperties(datiPensione, datiAnagrafici, datiMaggiorazioniBeneficiCommon, datiDetrazioni, isRiaperturaDomanda, datiStoricoGP, out TipologiaSalvaguardia, out DataPrelievoDomanda);

            if (areaLiquidazionePensione == null)
                areaLiquidazionePensione = new AreaLiquidazionePensione();

            areaLiquidazionePensione.IsEsenzioneFiscaleEstero = lCrossProperties["IsEsenzioneFiscaleEstero"];
            areaLiquidazionePensione.IsEsenzioneFiscaleVittima = lCrossProperties["IsEsenzioneFiscaleVittima"];
            areaLiquidazionePensione.IsRiduzioneRetribVisible = lCrossProperties["IsRiduzioneRetributiva"];
            areaLiquidazionePensione.IsUsuranti = lCrossProperties["Usuranti"];
            areaLiquidazionePensione.TipologiaSalvaguardia = TipologiaSalvaguardia;
            areaLiquidazionePensione.IsGestioneNormale = lCrossProperties["IsGestioneNormale"];
            areaLiquidazionePensione.IsVecchiaiaInvaliditaSupplementare = lCrossProperties["IsVecchiaiaInvaliditaSupplementare"];
            areaLiquidazionePensione.IsImportoIVSVisible = lCrossProperties["IsImportoIVSVisible"];
            areaLiquidazionePensione.IsRipristino = lCrossProperties["IsRipristino"];
            areaLiquidazionePensione.IsRiduzioneRetributivaEnabled = lCrossProperties["IsRiduzioneRetributivaEnabled"];
            areaLiquidazionePensione.IsTrasformazioneInvalidita = lCrossProperties["IsTrasformazioneInvalidita"];
            areaLiquidazionePensione.IsBeneficioArt24Comma15BisFromFELPE = lCrossProperties["IsBeneficioArt24Comma15BisFromFELPE"];
            areaLiquidazionePensione.IsPensioneTipoContributivo = lCrossProperties["IsPensioneTipoContributivo"];
            areaLiquidazionePensione.IsPensioneTipoContributivoConOpzione = lCrossProperties["IsPensioneTipoContributivoConOpzione"];
            areaLiquidazionePensione.IsSperimentaleDonna = lCrossProperties["IsSperimentaleDonna"];
            areaLiquidazionePensione.IsBeneficioApePrecociFromFELPE = lCrossProperties["IsBeneficioApePrecociFromFELPE"];
            areaLiquidazionePensione.IsPensioneVecchiaiaOrRicostituzione = lCrossProperties["IsPensioneVecchiaiaOrRicostituzione"];
            areaLiquidazionePensione.IsPensioneAnzianitaOrRicostituzione = lCrossProperties["IsPensioneAnzianitaOrRicostituzione"];
            areaLiquidazionePensione.IsEsenzioneFiscaleEsteroFromDetrazioni = lCrossProperties["IsEsenzioneFiscaleEsteroFromDetrazioni"];
            areaLiquidazionePensione.IsRichiestaBonusBookingAbilitata = lCrossProperties["IsRichiestaBonusBookingAbilitata"];
            areaLiquidazionePensione.IsBeneficioNonVedente = lCrossProperties["IsBeneficioNonVedente"];
            areaLiquidazionePensione.IsDataRinunciaTrattenutaInpdapStorico = lCrossProperties["IsDataRinunciaTrattenutaInpdapStorico"];
            areaLiquidazionePensione.IsBeneficioNonVedenteFromStorico = lCrossProperties["IsBeneficioNonVedenteFromStorico"];
            areaLiquidazionePensione.IsRichiestaBonus154Abilitata = lCrossProperties["IsRichiestaBonus154Abilitata"];
            areaLiquidazionePensione.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = lCrossProperties["IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione"];
            areaLiquidazionePensione.IsPensioneTipoContributivoAnzianitàVecchiaia = lCrossProperties["IsPensioneTipoContributivoAnzianitàVecchiaia"];
            areaLiquidazionePensione.IsAnte96 = lCrossProperties["IsAnte96"];


            //ENG - Aggiornamento Memo86
            areaLiquidazionePensione.IsPresenteTrattenutaFondoCreditoDaPrelievo = lCrossProperties["IsPresenteTrattenutaFondoCreditoDaPrelievo"];
            areaLiquidazionePensione.DataPrelievoDomanda = DataPrelievoDomanda;
        }

        private void GetDatiDBCommon(long numeroDomanda, byte? progStorico, out GestionePensione.DatiPensione datiPensione, out GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            out Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, bool datiIstruttoriaRequired, bool datiMaggBenRequired)
        {
            datiIstruttoriaCommon = null;
            datiMaggiorazioniBeneficiCommon = null;

            datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, progStorico);

            if (datiIstruttoriaRequired)
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoriaCommon);

            if (datiMaggBenRequired)
                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBeneficiCommon);
        }

        private void ValorizzaDatiForMaggiorazioniBenefici(long idPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon,
            out Entity.DatiExCombattente datiExCombattente, out Entity.DatiBenefici datiBenefici, out Entity.DatiMaggiorazioni datiMaggiorazioni)
        {
            datiExCombattente = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiExCombattente(datiMaggiorazioniBeneficiCommon, out datiExCombattente);

            datiBenefici = null;
            GestioneMaggiorazioniBenefici.GetDatiBeneficiByIdPensione(idPensione, datiMaggiorazioniBeneficiCommon, out datiBenefici);

            datiMaggiorazioni = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiMaggiorazioni(datiMaggiorazioniBeneficiCommon, out datiMaggiorazioni);
        }

        #endregion LiquidazionePensione

        #region AreaMaggiorazioneBenefici

        public AreaEsito GetMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            Entity.DatiExCombattente datiExCombattente = null;
            Entity.DatiBenefici datiBenefici = null;
            Entity.DatiMaggiorazioni datiMaggiorazioni = null;

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;

            GetDatiDBCommon(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico, out datiPensione, out datiIstruttoria, out datiMaggiorazioniBeneficiCommon, false, true);
            ValorizzaDatiForMaggiorazioniBenefici(datiPensione.Id, datiMaggiorazioniBeneficiCommon, out datiExCombattente, out datiBenefici, out datiMaggiorazioni);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);

            AreaEsito Esito = new AreaEsito();
            areaMaggiorazioniBenefici = null;

            #region Ex Combattente
            if (datiExCombattente != null)
            {
                areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiExCombattente = datiExCombattente;
            }

            #endregion Ex Combattente

            #region Benefici
            if (datiBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiBenefici = datiBenefici;
            }

            #endregion Benefici

            #region Maggiorazioni
            if (datiMaggiorazioni != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.DatiMaggiorazioni = datiMaggiorazioni;
            }

            #endregion Maggiorazioni

            #region Decodifiche
            List<Entity.CodiceCieco> listaCodiceCieco = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceCieco(out listaCodiceCieco);
            if (listaCodiceCieco != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceCieco = listaCodiceCieco;
            }

            List<Entity.TipoBenefici> listaTipoBenefici = null;
            GestioneMaggiorazioniBenefici.GetListaTipoBenefici(datiPensione, out listaTipoBenefici);
            if (listaTipoBenefici != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipoBenefici = listaTipoBenefici;
            }

            List<Entity.CodiceMaggiorazioneExCombattente> listaCodiceMaggiorazioneExCombattente = null;
            GestioneMaggiorazioniBenefici.GetListaCodiceMaggiorazioneExCombattente(out listaCodiceMaggiorazioneExCombattente);
            if (listaCodiceMaggiorazioneExCombattente != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceMaggiorazioneExCombattente = listaCodiceMaggiorazioneExCombattente;
            }

            List<Entity.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392 = null;
            GestioneMaggiorazioniBenefici.GetListaRequisitiLegge50392(out listaCodiceRequisitiLegge50392);
            if (listaCodiceRequisitiLegge50392 != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaCodiceRequisitiLegge50392 = listaCodiceRequisitiLegge50392;
            }

            List<Entity.SoggettoBeneficiario> listaSoggettoBeneficiario = null;
            GestioneMaggiorazioniBenefici.GetListaSoggettoBeneficiario(datiPensione, out listaSoggettoBeneficiario);
            if (listaSoggettoBeneficiario != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaSoggettoBeneficiario = listaSoggettoBeneficiario;
            }

            List<Entity.TipologiaPrestazione> listaTipologiaPrestazione = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaPrestazione(out listaTipologiaPrestazione);
            if (listaTipologiaPrestazione != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipologiaPrestazione = listaTipologiaPrestazione;
            }

            List<Entity.TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaBeneficioTerrorismo(out listaTipologiaBeneficioTerrorismo);
            if (listaTipologiaBeneficioTerrorismo != null)
            {
                if (areaMaggiorazioniBenefici == null)
                    areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
                areaMaggiorazioniBenefici.ListaTipologiaBeneficioTerrorismo = listaTipologiaBeneficioTerrorismo;
            }

            #endregion Decodifiche

            DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetDatiBeneficioVittimeTerrorismo(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            GetCrossProperties(datiPensione, datiMaggiorazioniBeneficiCommon, datiBeneficioVittimeTerrorismo, ref areaMaggiorazioniBenefici, datiGenerici);

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito StoreMaggiorazioniBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            char? derogaTraduzioneSuGP = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);

            if (datiIstruttoriaCommon != null && datiIstruttoriaCommon.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = null;
                GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareSoggettoDerogato);
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoriaCommon.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficaTitolare);
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = null;
            GestioneCalcoloVittimeTerrorismo.GetCalcoloVittimeTerrorismoByIdPensione(datiPensione.Id, out listaDatiCalcoloVittimeTerrorismo);

            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out datiCalcoloContributivo);

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            AreaEsito Esito = new AreaEsito();

            #region Ex Combattente

            Esito = StoreDatiExCombattentePrivate(datiPensione, ref datiMaggiorazioniBeneficiCommon, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Ex Combattente

            #region Benefici

            Esito = StoreDatiBeneficiPrivate(datiPensione, ref datiMaggiorazioniBeneficiCommon, datiAnagraficaTitolare, datiIstruttoriaCommon, datiPensioniDatiGenerici, areaMaggiorazioniBenefici, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Benefici

            #region Maggiorazioni

            Esito = StoreDatiMaggiorazioniPrivate(datiPensione, ref datiMaggiorazioniBeneficiCommon, areaMaggiorazioniBenefici, false, datiAnagraficaTitolare.DataNascita);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Maggiorazioni

            #region Benefici Vittime Terrorismo

            Esito = StoreDatiBeneficioVittimeTerrorismoPrivate(datiPensione, areaMaggiorazioniBenefici, listaDatiCalcoloVittimeTerrorismo, datiCalcoloContributivo, datiBeneficioVittimeTerrorismo, false);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion Benefici Vittime Terrorismo

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #region DatiBenefici

        public AreaEsito StoreDatiBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, true, true);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficaTitolare);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiBeneficiPrivate(datiPensione, ref datiMaggiorazioniBeneficiCommon, datiAnagraficaTitolare, datiIstruttoriaCommon, datiPensioniDatiGenerici, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        private AreaEsito StoreDatiBeneficiPrivate(GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, null, true, null, null))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlDatiBenefici(datiPensione, areaMaggiorazioniBenefici.DatiBenefici, datiAnagraficaTitolare, datiIstruttoria, datiPensioniDatiGenerici, false,
                    out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneMaggiorazioniBenefici.StoreDatiBenefici(datiPensione, ref datiMaggiorazioniBeneficiCommon, areaMaggiorazioniBenefici.DatiBenefici);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        public AreaEsito CancelDatiBenefici(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;
            List<GestioneOneri.DatiOneri> listaDatiOneri = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, false, true);
            GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaDatiOneri);

            AreaEsito Esito = new AreaEsito();

            GestioneMaggiorazioniBenefici.EliminaDatiBenefici(datiPensione, datiMaggiorazioniBeneficiCommon, listaDatiOneri);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiBenefici

        #region DatiExCombattente

        public AreaEsito StoreDatiExCombattente(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, false, true);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiExCombattentePrivate(datiPensione, ref datiMaggiorazioniBeneficiCommon, areaMaggiorazioniBenefici, true);
            return Esito;
        }

        public AreaEsito CancelDatiExCombattente(long numeroDomanda)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, false, true);


            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiExCombattente(datiPensione, datiMaggiorazioniBeneficiCommon);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        private AreaEsito StoreDatiExCombattentePrivate(GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;


            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, true, null, null, null))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlDatiExCombattente(datiPensione, areaMaggiorazioniBenefici.DatiExCombattente, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }
                GestioneMaggiorazioniBenefici.StoreDatiExCombattente(datiPensione, ref datiMaggiorazioniBeneficiCommon, areaMaggiorazioniBenefici.DatiExCombattente);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        #endregion DatiExCombattente

        #region DatiMaggiorazioni

        public AreaEsito StoreDatiMaggiorazioni(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoriaCommon, out datiMaggiorazioniBeneficiCommon, false, true);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficaTitolare);

            AreaEsito Esito = new AreaEsito();
            Esito = StoreDatiMaggiorazioniPrivate(datiPensione, ref datiMaggiorazioniBeneficiCommon, areaMaggiorazioniBenefici, true, datiAnagraficaTitolare.DataNascita);
            return Esito;
        }

        private AreaEsito StoreDatiMaggiorazioniPrivate(GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon,
            AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, bool singleTab, DateTime? dataNascitaTitolare)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;


            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, null, null, true, null))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                GestioneMaggiorazioniBenefici.ControlDatiMaggiorazioni(datiPensione, areaMaggiorazioniBenefici.DatiMaggiorazioni, false, dataNascitaTitolare, out messaggioControllo);
                if (!String.IsNullOrEmpty(messaggioControllo))
                {
                    Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = messaggioControllo;
                    return Esito;
                }

                GestioneMaggiorazioniBenefici.StoreDatiMaggiorazioni(datiPensione, ref datiMaggiorazioniBeneficiCommon, areaMaggiorazioniBenefici.DatiMaggiorazioni);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            return Esito;
        }

        public AreaEsito CancelDatiMaggiorazioni(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;

            GetDatiDBCommon(numeroDomanda, null, out datiPensione, out datiIstruttoria, out datiMaggiorazioniBeneficiCommon, false, true);

            areaMaggiorazioniBenefici = null;
            AreaEsito Esito = new AreaEsito();
            GestioneMaggiorazioniBenefici.EliminaDatiMaggiorazioni(datiPensione, datiMaggiorazioniBeneficiCommon);

            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            Entity.DatiMaggiorazioni datiMaggiorazioni = null;
            GestioneMaggiorazioniBenefici.ValorizzaDatiMaggiorazioni(datiMaggiorazioniBeneficiCommon, out datiMaggiorazioni);

            if (datiMaggiorazioni != null)
                areaMaggiorazioniBenefici.DatiMaggiorazioni = datiMaggiorazioni;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion DatiMaggiorazioni

        private void GetCrossProperties(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, ref AreaMaggiorazioniBenefici areaMaggiorazioniBenefici, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici)
        {
            int? settimane = null;
            Dictionary<string, bool?> lCrossProperties = GestioneMaggiorazioniBenefici.GetCrossProperties(datiPensione, datiMaggiorazioniBenefici, datiBeneficioVittimeTerrorismo, datiGenerici, out settimane);

            if (areaMaggiorazioniBenefici == null)
                areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();

            areaMaggiorazioniBenefici.IsBeneficioExArt80 = lCrossProperties["IsBeneficioExArt80"];
            areaMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE = lCrossProperties["IsBeneficioArt24Comma15BisFromFELPE"];
            areaMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE = lCrossProperties["IsBeneficioApePrecociFromFELPE"];
            areaMaggiorazioniBenefici.IsDomandaPensioneInabilita = lCrossProperties["IsDomandaPensioneInabilita"];
            areaMaggiorazioniBenefici.IsBeneficioVittimeTerrorismo = lCrossProperties["IsBeneficioVittimeTerrorismo"];
            areaMaggiorazioniBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015 = lCrossProperties["IsBeneficioMaggiorazioneAmiantoLegge208_2015"];
            areaMaggiorazioniBenefici.Settimane = settimane;
        }

        #endregion AreaMaggiorazioneBenefici

        #region DatiBeneficioVittimeTerrorismo

        public AreaEsito StoreDatiBeneficioVittimeTerrorismo(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = null;
            GestioneCalcoloVittimeTerrorismo.GetCalcoloVittimeTerrorismoByIdPensione(datiPensione.Id, out listaDatiCalcoloVittimeTerrorismo);

            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out datiCalcoloContributivo);

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            AreaEsito Esito = new AreaEsito();
            try
            {
                Esito = StoreDatiBeneficioVittimeTerrorismoPrivate(datiPensione, areaMaggiorazioniBenefici, listaDatiCalcoloVittimeTerrorismo, datiCalcoloContributivo, datiBeneficioVittimeTerrorismo, true);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati Vittime. Riprovare più tardi";
            }

            return Esito;
        }

        private AreaEsito StoreDatiBeneficioVittimeTerrorismoPrivate(GestionePensione.DatiPensione datiPensione, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo, List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, bool singleTab)
        {
            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;
            bool isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null);
            if (areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo != null)
            {
                isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione) ||
                                               Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione);
            }
            if (!singleTab && !GestioneMaggiorazioniBenefici.ControlsVisibleTabs(datiPensione, null, null, null, isBeneficioVittimeTerrorismo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            else
            {
                try
                {
                    GestioneMaggiorazioniBenefici.ControlDatiBeneficioVittimeTerrorismo(datiPensione, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo, listaDatiCalcoloVittimeTerrorismo,
                        datiCalcoloContributivo, datiBeneficioVittimeTerrorismo, out messaggioControllo);
                    if (!String.IsNullOrEmpty(messaggioControllo))
                    {
                        Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                        Esito.Messaggio = messaggioControllo;
                        return Esito;
                    }

                    GestioneMaggiorazioniBenefici.StoreDatiBeneficioVittimeTerrorismo(datiPensione, areaMaggiorazioniBenefici.DatiBeneficioVittimeTerrorismo, datiCalcoloContributivo);
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

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            try
            {
                GestioneMaggiorazioniBenefici.EliminaDatiBeneficioVittimeTerrorismo(datiPensione);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
                Esito.Messaggio = string.Empty;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = "Errore tecnico nel salvataggio dei dati calcolo. Riprovare più tardi";
            }

            List<Entity.SoggettoBeneficiario> listaSoggettoBeneficiario = null;
            GestioneMaggiorazioniBenefici.GetListaSoggettoBeneficiario(datiPensione, out listaSoggettoBeneficiario);
            if (listaSoggettoBeneficiario != null)
                areaMaggiorazioniBenefici.ListaSoggettoBeneficiario = listaSoggettoBeneficiario;

            List<Entity.TipologiaPrestazione> listaTipologiaPrestazione = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaPrestazione(out listaTipologiaPrestazione);
            if (listaTipologiaPrestazione != null)
                areaMaggiorazioniBenefici.ListaTipologiaPrestazione = listaTipologiaPrestazione;

            List<Entity.TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismo = null;
            GestioneMaggiorazioniBenefici.GetListaTipologiaBeneficioTerrorismo(out listaTipologiaBeneficioTerrorismo);
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

        #region AreaBititolarita

        public AreaEsito GetBititolaritaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            long idPensione = GetIdPensioneByNumeroDomanda(areaRichiestaDomanda.NumeroDomanda, areaRichiestaDomanda.ProgStorico);

            AreaEsito Esito = new AreaEsito();
            areaBititolarita = null;

            #region AltrePensioni

            List<Entity.AltraPensione> LdatiAltraPensione = null;
            GestioneBititolarita.GetDatiAltraPensioneByIdPensione(idPensione, out LdatiAltraPensione);
            if (LdatiAltraPensione != null && LdatiAltraPensione.Count > 0)
            {
                areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoAltraPensione = LdatiAltraPensione;
            }

            #endregion AltrePensioni

            #region Liste

            List<GestioneBititolarita.DecodificaEnte> ElencoEnte = null;
            GestioneBititolarita.GetListeDecodificaEnte(out ElencoEnte);
            if (ElencoEnte != null && ElencoEnte.Count > 0)
            {
                if (areaBititolarita == null)
                    areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoDecodificaEnte = ElencoEnte;
            }

            List<GestioneBititolarita.DecCatEnte> ElencoCatEnte = null;
            GestioneBititolarita.GetListeDecCatEnte(out ElencoCatEnte);
            if (ElencoCatEnte != null && ElencoCatEnte.Count > 0)
            {
                if (areaBititolarita == null)
                    areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoCatEnte = ElencoCatEnte;
            }
            #endregion Liste

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        public AreaEsito StoreBititolarita(long numeroDomanda, AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            #region AltraPensione

            Esito = StoreAltraPensionePrivate(datiPensione, areaBititolarita, out messaggioControllo);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            #endregion AltraPensione

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;
        }

        #region AltraPensione

        public AreaEsito StoreAltraPensione(long numeroDomanda, AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            AreaEsito Esito = new AreaEsito();
            string messaggioControllo = string.Empty;

            Esito = StoreAltraPensionePrivate(datiPensione, areaBititolarita, out messaggioControllo);
            if (Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                return Esito;

            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;

            return Esito;

        }

        private AreaEsito StoreAltraPensionePrivate(GestionePensione.DatiPensione datiPensione, AreaDatiBititolarita areaBititolarita, out string messaggioControllo)
        {
            messaggioControllo = string.Empty;
            AreaEsito Esito = new AreaEsito();

            GestioneBititolarita.ControlsDatiAltraPensione(datiPensione, areaBititolarita.ElencoAltraPensione, out messaggioControllo);
            if (!String.IsNullOrEmpty(messaggioControllo))
            {
                Esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                Esito.Messaggio = messaggioControllo;
                return Esito;
            }
            GestioneBititolarita.StoreDatiAltraPensione(datiPensione, areaBititolarita.ElencoAltraPensione);
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        public AreaEsito CancelAltraPensione(long numeroDomanda, out AreaDatiBititolarita areaBititolarita)
        {
            SetCulture();

            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);

            areaBititolarita = null;
            AreaEsito Esito = new AreaEsito();
            GestioneBititolarita.DeleteDatiAltraPensione(datiPensione);

            #region Liste

            List<GestioneBititolarita.DecodificaEnte> ElencoEnte = null;
            GestioneBititolarita.GetListeDecodificaEnte(out ElencoEnte);
            if (ElencoEnte != null && ElencoEnte.Count > 0)
            {
                areaBititolarita = new AreaDatiBititolarita();
                areaBititolarita.ElencoDecodificaEnte = ElencoEnte;
            }
            #endregion Liste
            Esito.RisultatoOperazione = AreaEsito.TipoEsito.OK;
            Esito.Messaggio = string.Empty;
            return Esito;
        }

        #endregion AltraPensione

        #endregion AreaBititolarita
    }
}
