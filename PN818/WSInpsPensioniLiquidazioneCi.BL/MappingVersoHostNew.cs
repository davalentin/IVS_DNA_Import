using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class MappingVersoHostNew
    {
        #region public members
        public static void ValorizzaRichiesta(string matricolaOperatore, short sedeOperatore, GestionePensione.DatiPensione datiPensione, out Data.HostRequest.CI01_CI02RequestNew richiesta)
        {
            richiesta = new INPS.Pensioni.LiquidazioneCi.Data.HostRequest.CI01_CI02RequestNew();
            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datigenericiCi = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare = null;
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo = null;
            GestionePensione.DatiEliminazione datiEliminazione = null;
            List<GestioneAltrePensioni.AltraPensione> listaAltraPensione = null;
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            List<GestioneDanteCausa.DatiRedditoSentenza495_93> lDatiSentenza495_93 = null;
            GestioneDanteCausa.GetRedditiSentenza495_93ByIdPensione(datiPensione.Id, out lDatiSentenza495_93);

            ValorizzaGruppo1(datiPensione, isRiapertura, out danteCausa, out datigenericiCi, out datiIstruttoria, out areaTitolare,
                out listaPrestazioniEE, out datiMaggiorazioniBenefici, out listaCalcoloContributivo, out datiEliminazione, out listaAltraPensione, ref richiesta);

            ValorizzaGruppo2(datiPensione, areaTitolare.Anagrafica.CodiceFiscale, ref richiesta);

            ValorizzaGruppo3(matricolaOperatore, datiPensione, danteCausa, datigenericiCi, datiIstruttoria, datiMaggiorazioniBenefici, areaTitolare,
                listaCalcoloContributivo, datiEliminazione, listaAltraPensione, listaPrestazioniEE, lDatiSentenza495_93, ref richiesta);

            ValorizzaGruppo4(datiPensione, listaPrestazioniEE, datiMaggiorazioniBenefici, danteCausa, ref richiesta);
        }
        #endregion public members

        #region private methods
        private static void GetCodiceProvinciaNascita(string provinciaNascita, out short codProvNascita)
        {
            codProvNascita = 0;
            string query = (from s in INPS.DNA.Context.OfficeList.Offices
                            where (s.Value.ExtendedProperties != null ? s.Value.ExtendedProperties["PR"].Trim() : s.Value.Province.Trim()) == provinciaNascita.Trim()
                            select s.Value.SSCode).FirstOrDefault<string>();
            short.TryParse(query, out codProvNascita);
        }

        #region Gruppo1
        private static void ValorizzaGruppo1(GestionePensione.DatiPensione datiPensione, bool isRiapertura,
            out GestioneDanteCausa.DatiDanteCausa danteCausa, out GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            out GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare,
            out List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE,
            out Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            out List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo,
            out GestionePensione.DatiEliminazione datiEliminazione,
            out List<GestioneAltrePensioni.AltraPensione> listaAltraPensione,
            ref Data.HostRequest.CI01_CI02RequestNew richiesta)
        {
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            Data.PCIINPU7.AreaTP11 areaTP11 = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            areaTitolare = null;
            ValorizzaAreaTP11(datiPensione, out datiAnagrafici, out areaTitolare, out areaTP11);
            richiesta.Gruppo1.AreaTP11 = areaTP11;

            Data.PCIINPU7.AreaTP12 areaTP12 = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            danteCausa = null;
            datiIstruttoria = null;
            datiEliminazione = null;
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            ValorizzaAreaTP12(datiPensione, datiAnagrafici, out datiAnagraficiDC, out danteCausa, out datiIstruttoria, out datiDetrazioni, out datiEliminazione, out areaTP12);
            richiesta.Gruppo1.AreaTP12 = areaTP12;

            Data.PCIINPU7.AreaDelegato areaDelegato = null;

            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiapertura)
            {
                ValorizzaAreaDelegato(datiPensione, out areaDelegato);
                richiesta.Gruppo1.AreaDelegato = areaDelegato;
            }

            Data.PCIINPU7.AreaTutore areaTutore = null;
            ValorizzaAreaTutore(datiPensione, out areaTutore);
            richiesta.Gruppo1.AreaTutore = areaTutore;

            Data.PCIINPU7.AreaDati areaDati = null;
            GestionePagamento.DatiPagamento datiPagamento = null;
            datiMaggiorazioniBenefici = null;

            ValorizzaAreaDati(datiPensione, datiIstruttoria, out datiPagamento, out datiMaggiorazioniBenefici, datiEliminazione, isRiapertura, out areaDati);
            richiesta.Gruppo1.AreaDati = areaDati;

            Data.PCIINPU7.AreaW1L areaW1L = null;
            datiGenericiCi = null;
            listaPrestazioniEE = null;
            ValorizzaAreaW1L(datiPensione, datiAnagraficiDC, danteCausa, datiIstruttoria, areaTitolare, datiMaggiorazioniBenefici, datiEliminazione, out datiGenericiCi,
                out listaPrestazioniEE, isRiapertura, out areaW1L);
            richiesta.Gruppo1.AreaW1L = areaW1L;

            Data.PCIINPU7.AreaW2CL areaW2CL = null;

            ValorizzaAreaW2CL(datiPensione, datiAnagrafici, datiGenericiCi, listaPrestazioniEE, out areaW2CL);
            richiesta.Gruppo1.AreaW2CL = areaW2CL;

            Data.PCIINPU7.AreaW2 areaW2 = null;
            ValorizzaAreaW2(datiPensione, out areaW2);
            richiesta.Gruppo1.AreaW2 = areaW2;

            Data.PCIINPU7.AreaVarie areaVarie = null;
            listaCalcoloContributivo = null;

            ValorizzaAreaVarie(datiPensione, datiPagamento, areaTitolare, datiAnagrafici, datiAnagraficiDC, danteCausa, datiGenericiCi, datiIstruttoria, datiMaggiorazioniBenefici, datiDetrazioni, listaPrestazioniEE, isRiapertura, out listaCalcoloContributivo, out areaVarie);
            richiesta.Gruppo1.AreaVarie = areaVarie;

            Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati = null;
            listaAltraPensione = null;
            ValorizzaAreaUlterioriDati(datiPensione, danteCausa, datiIstruttoria, datiEliminazione, datiMaggiorazioniBenefici, out listaAltraPensione, out areaUlterioriDati);
        }

        private static void ValorizzaAreaTP11(GestionePensione.DatiPensione datiPensione, out GestioneAnagrafica.DatiAnagrafici datiAnagrafici, out Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare, out Data.PCIINPU7.AreaTP11 areaTP11)
        {
            areaTP11 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaTP11();
            datiAnagrafici = null;
            areaTitolare = null;
            DateTime dataSistema = Utility.DataSistemaCi;

            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            if (datiAnagrafici != null)
            {
                areaTP11.TP1CAPRS = datiAnagrafici.CAP;

                if (!string.IsNullOrEmpty(datiAnagrafici.Cittadinanza))
                {
                    //ENG - Memo 48_2023
                    if (Utility.IsTitolareResidente_Cittadino_Bulgaria(datiPensione, datiAnagrafici))
                        areaTP11.TP1CITT1 = "BG";
                    else
                    {
                        List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
                        GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);
                        if (listaStatiEsteri != null)
                        {
                            string citt = datiAnagrafici.Cittadinanza;
                            GestioneDecodifica.StatoEstero statoEstero = listaStatiEsteri.Find(x => x.CodCatastale == citt);
                            if (statoEstero != null)
                            {
                                areaTP11.TP1CITT1 = !string.IsNullOrEmpty(statoEstero.Sigla) ? statoEstero.Sigla.Trim() == "ITA" ? "I" : statoEstero.Sigla.Trim() : string.Empty;
                            }
                        }
                    }
                }

                areaTP11.TP1CIVICO = datiAnagrafici.NCivico;

                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagrafici.CodiceComuneNascita, Utility.TipoAppartenenza.CI.ToString(), 0, false, out codiceInpsComune);
                areaTP11.TP1CO = codiceInpsComune;

                areaTP11.TP1COAC = datiAnagrafici.CognomeAcquisito;
                areaTP11.TP1COG1 = datiAnagrafici.Cognome;
                areaTP11.TP1COMUN = datiAnagrafici.ComuneResidenza;
                if (datiAnagrafici.ResidenzaEstero.HasValue && datiAnagrafici.ResidenzaEstero.Value)
                    areaTP11.TP1ESTITA = "9";
                else if (datiAnagrafici.ResidenzaEstero.HasValue && !datiAnagrafici.ResidenzaEstero.Value)
                    areaTP11.TP1ESTITA = "1";
                else
                    areaTP11.TP1ESTITA = "";
                areaTP11.TP1FRAZIO = datiAnagrafici.FrazioneResidenza;
                areaTP11.TP1NOM1 = datiAnagrafici.Nome;

                //short codProvNascita = 0;
                //GetCodiceProvinciaNascita(datiAnagrafici.ProvinciaNascita, out codProvNascita);
                //areaTP11.TP1PR_EX = codProvNascita;

                areaTP11.TP1PROV = datiAnagrafici.ProvinciaResidenza;

                if (Utility.IsDomandaAPEPrecoci(datiPensione))
                    areaTP11.TP1SEDE = datiPensione.CodiceSede;
                else
                    areaTP11.TP1SEDE = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value : datiPensione.CodiceSede;

                GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);
                if (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0)
                    areaTP11.TP1STACIV = areaTitolare.ElencoStatiCivili[areaTitolare.ElencoStatiCivili.Count - 1].Codice.ToString();
                else if (datiAnagrafici.CodiceStatoCivile.HasValue)
                    areaTP11.TP1STACIV = datiAnagrafici.CodiceStatoCivile.Value.ToString();

                if (datiAnagrafici.Indirizzo.Trim().Length > 52)
                {
                    areaTP11.TP1VIA1 = datiAnagrafici.Indirizzo.Trim().Substring(0, 52);
                    if (datiAnagrafici.Indirizzo.Trim().Length > 104)
                    {
                        areaTP11.TP1VIA2 = datiAnagrafici.Indirizzo.Trim().Substring(52, 52);
                        if (datiAnagrafici.Indirizzo.Trim().Length > 156)
                        {
                            areaTP11.TP1VIA3 = datiAnagrafici.Indirizzo.Trim().Substring(104, 52);
                            if (datiAnagrafici.Indirizzo.Trim().Length > 208)
                                areaTP11.TP1VIA4 = datiAnagrafici.Indirizzo.Trim().Substring(156, 52);
                            else
                                areaTP11.TP1VIA4 = datiAnagrafici.Indirizzo.Trim().Substring(156);
                        }
                        else
                            areaTP11.TP1VIA3 = datiAnagrafici.Indirizzo.Trim().Substring(104);
                    }
                    else
                        areaTP11.TP1VIA2 = datiAnagrafici.Indirizzo.Trim().Substring(52);
                }
                else
                    areaTP11.TP1VIA1 = datiAnagrafici.Indirizzo.Trim();
            }

            areaTP11.TP1ELABA = (short)dataSistema.Year;
            areaTP11.TP1ELABM = (short)dataSistema.Month;
            areaTP11.TP1ELABG = (short)dataSistema.Day;

            bool presenzaFamiliari = false;
            GestioneFamiliari.CheckFamiliariByIdPensione(datiPensione.Id, out presenzaFamiliari);
            areaTP11.TP1NFAM = presenzaFamiliari ? (short)1 : (short)0;
        }

        private static void ValorizzaAreaTP12(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici,
            out GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, out GestioneDanteCausa.DatiDanteCausa danteCausa,
            out GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni,
            out GestionePensione.DatiEliminazione datiEliminazione, out Data.PCIINPU7.AreaTP12 areaTP12)
        {
            areaTP12 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaTP12();

            areaTP12.TP1ATEC = datiPensione.AttivitaEconomica.HasValue ? datiPensione.AttivitaEconomica.Value : 0;
            areaTP12.TP1PRIN = datiPensione.ProfessioneIndividuale.HasValue ? datiPensione.ProfessioneIndividuale.Value : 0;
            areaTP12.TP1ACC = datiPensione.CodiceArretrati.HasValue ? datiPensione.CodiceArretrati.Value : (short)0;
            areaTP12.TP1ILEGA = datiPensione.DataInteressiLegali.HasValue ? (short)datiPensione.DataInteressiLegali.Value.Year : (short)0;
            areaTP12.TP1ILEGM = datiPensione.DataInteressiLegali.HasValue ? (short)datiPensione.DataInteressiLegali.Value.Month : (short)0;
            areaTP12.TP1ILEGG = datiPensione.DataInteressiLegali.HasValue ? (short)datiPensione.DataInteressiLegali.Value.Day : (short)0;

            datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);
            if (datiDetrazioni != null)
            {
                areaTP12.CO1N = datiDetrazioni.DetrazioniReddito.HasValue ? (short)datiDetrazioni.DetrazioniReddito.Value : (short)0;
                areaTP12.CO2N = datiDetrazioni.AgevolazionePensionati.HasValue ? (short)datiDetrazioni.AgevolazionePensionati.Value : (short)0;
                areaTP12.CO3N = datiDetrazioni.ConiugeOFiglio.HasValue ? (short)datiDetrazioni.ConiugeOFiglio.Value : (short)0;
                areaTP12.CO4N = datiDetrazioni.FigliMinori3AnniNoHandicap100.HasValue ? (short)datiDetrazioni.FigliMinori3AnniNoHandicap100.Value : (short)0;
                areaTP12.CO5N = datiDetrazioni.FigliMinori3AnniNoHandicap50.HasValue ? (short)datiDetrazioni.FigliMinori3AnniNoHandicap50.Value : (short)0;
                areaTP12.CO6N = datiDetrazioni.FigliMinori3AnniHandicap100.HasValue ? (short)datiDetrazioni.FigliMinori3AnniHandicap100.Value : (short)0;
                areaTP12.CO7N = datiDetrazioni.FigliMinori3AnniHandicap50.HasValue ? (short)datiDetrazioni.FigliMinori3AnniHandicap50.Value : (short)0;
                areaTP12.CO8N = datiDetrazioni.FigliMaggiori3AnniNoHandicap100.HasValue ? (short)datiDetrazioni.FigliMaggiori3AnniNoHandicap100.Value : (short)0;
                areaTP12.CO9N = datiDetrazioni.FigliMaggiori3AnniNoHandicap50.HasValue ? (short)datiDetrazioni.FigliMaggiori3AnniNoHandicap50.Value : (short)0;
                areaTP12.CO10N = datiDetrazioni.FigliMaggiori3AnniHandicap100.HasValue ? (short)datiDetrazioni.FigliMaggiori3AnniHandicap100.Value : (short)0;
                areaTP12.CO11N = datiDetrazioni.FigliMaggiori3AnniHandicap50.HasValue ? (short)datiDetrazioni.FigliMaggiori3AnniHandicap50.Value : (short)0;
                areaTP12.CO12N = datiDetrazioni.AltriFamiliari100.HasValue ? (short)datiDetrazioni.AltriFamiliari100.Value : (short)0;
                areaTP12.CO13N = datiDetrazioni.AltriFamiliari50.HasValue ? (short)datiDetrazioni.AltriFamiliari50.Value : (short)0;
                areaTP12.CO14N = datiDetrazioni.AddizionaleLombardiaVeneto.HasValue ? (short)datiDetrazioni.AddizionaleLombardiaVeneto.Value : (short)0;
            }

            //ENG - Memo 48_2023
            if (Utility.IsTitolareResidente_Cittadino_Bulgaria(datiPensione, datiAnagrafici))
            {
                areaTP12.CO1N = 2;
            }

            // ENG - Memo 49_2023
            if (Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
            {
                areaTP12.CO1N = 3;
            }

            if (datiAnagrafici != null)
            {
                areaTP12.TP1COFI = datiAnagrafici.CodiceFiscale;
            }

            datiAnagraficiDC = null;
            danteCausa = null;
            GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out datiAnagraficiDC);
            if (datiAnagraficiDC != null)
            {
                areaTP12.TP1COGDC = datiAnagraficiDC.Cognome;
                areaTP12.TP1NOMDC = datiAnagraficiDC.Nome;
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagraficiDC.CodiceComuneNascita, Utility.TipoAppartenenza.CI.ToString(), 0, false, out codiceInpsComune);
                areaTP12.TP1COMDC = codiceInpsComune;

                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);
                if (danteCausa != null)
                {
                    areaTP12.TP1CERTD = danteCausa.Certificato.HasValue ? danteCausa.Certificato.Value : 0;
                    short resShort = 0;
                    if (!string.IsNullOrEmpty(danteCausa.SiglaCategoria))
                    {
                        string codCategoria = "";
                        GestioneDecodifica.GetCodCategoriaBySiglaCategoria(danteCausa.SiglaCategoria, out codCategoria);
                        if (!string.IsNullOrEmpty(codCategoria))
                            short.TryParse(codCategoria.Trim(), out resShort);
                    }
                    areaTP12.TP1CATD = resShort;
                    resShort = 0;
                    if (!string.IsNullOrEmpty(danteCausa.Sede))
                        short.TryParse(danteCausa.Sede.Trim(), out resShort);
                    areaTP12.TP1SEDED = resShort;
                    GestioneDecodifica.StatoEstero statoEstero = null;
                    GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(danteCausa.StatoEEResidenza, out statoEstero);
                    if (statoEstero != null)
                        areaTP12.TP1RESDC = statoEstero.Sigla;
                }
            }

            datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
            if (datiIstruttoria != null)
            {
                areaTP12.TP1REVA = datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? (short)datiIstruttoria.ScadenzaRevisioneSanitaria.Value.Year : (short)0;
                areaTP12.TP1REVM = datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? (short)datiIstruttoria.ScadenzaRevisioneSanitaria.Value.Month : (short)0;
                areaTP12.TP1CORIC = datiIstruttoria.CodiceDomandaRicorso.HasValue ? datiIstruttoria.CodiceDomandaRicorso.Value : (short)0;
                if (datiIstruttoria.CodiceCdCmMr.HasValue)
                {
                    List<GestioneDecodifica.CDCMMR> elencoCDCMMR = null;
                    GestioneDecodifica.GetCodiciCDCMMR(out elencoCDCMMR);
                    if (elencoCDCMMR != null && elencoCDCMMR.Count > 0)
                    {
                        byte? CdCmMr = datiIstruttoria.CodiceCdCmMr;
                        GestioneDecodifica.CDCMMR codiceCDCMMR = elencoCDCMMR.Find(x => x.Id == CdCmMr);
                        if (codiceCDCMMR != null)
                            areaTP12.TP1CDCM = codiceCDCMMR.Descrizione;
                    }
                }

                areaTP12.TP1CLIV1 = datiIstruttoria.ClasseInvalidita1Codice.HasValue ? datiIstruttoria.ClasseInvalidita1Codice.Value : (short)0;
                areaTP12.TP1CLIV2 = datiIstruttoria.ClasseInvalidita2Codice.HasValue ? datiIstruttoria.ClasseInvalidita2Codice.Value : (short)0;
            }

            datiEliminazione = null;
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);
            if (datiEliminazione != null)
            {
                areaTP12.TP1ELIMA = datiEliminazione.DecorrenzaEliminazione.HasValue ? (short)datiEliminazione.DecorrenzaEliminazione.Value.Year : (short)0;
                areaTP12.TP1ELIMM = datiEliminazione.DecorrenzaEliminazione.HasValue ? (short)datiEliminazione.DecorrenzaEliminazione.Value.Month : (short)0;
            }

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            //Per i precoci la valorizzazione dl campo GP1AF06 avviene mediante la Scadenza Beneficio dell'onere acquisito
            if (Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
            {
                List<GestioneOneri.DatiOneri> listaOneri = null;
                GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaOneri);
                List<GestioneDecodifica.GruppoOneri> listaGruppoOneri = null;
                GestioneDecodifica.GetGruppoOneri(out listaGruppoOneri);
                if (listaOneri != null && listaOneri.Count > 0 && listaGruppoOneri != null && listaGruppoOneri.Count > 0)
                {
                    string codeGruppoOneri = string.Empty;
                    if (Utility.IsDomandaAPEPrecoci(datiPensione))
                        codeGruppoOneri = "5000";
                    else if (Utility.IsDomandaQuota100(datiPensione))
                        codeGruppoOneri = "5300";
                    else if (Utility.IsDomandaQuota102(datiPensione))
                        codeGruppoOneri = "5800";
                    else if (Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione))
                        codeGruppoOneri = "6000";
                    else if (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))
                        codeGruppoOneri = "6100";
                    GestioneDecodifica.GruppoOneri gruppoOneri = listaGruppoOneri.FirstOrDefault(x => x.Code == codeGruppoOneri);
                    if (gruppoOneri != null)
                    {
                        GestioneOneri.DatiOneri onere = listaOneri.FirstOrDefault(x => x.IdCodeGruppo == gruppoOneri.Id);
                        if (onere != null)
                        {
                            areaTP12.TP1REVA = onere.ScadenzaBeneficio.HasValue ? (short)onere.ScadenzaBeneficio.Value.Year : (short)0;
                            areaTP12.TP1REVM = onere.ScadenzaBeneficio.HasValue ? (short)onere.ScadenzaBeneficio.Value.Month : (short)0;
                        }
                    }
                }
            }
        }

        private static void ValorizzaAreaDelegato(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaDelegato areaDelegato)
        {
            areaDelegato = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaDelegato();
            GestioneAnagrafica.DatiAnagrafici datiDelegato = null;
            GestioneDelegatoTutore.GetDelegatoByIdPensione(datiPensione.Id, out datiDelegato);
            if (datiDelegato != null)
            {
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiDelegato.CodiceComuneNascita, Utility.TipoAppartenenza.CI.ToString(), 0, false, out codiceInpsComune);
                areaDelegato.D_TP1AP23 = codiceInpsComune;
                areaDelegato.D_TP1AP24 = datiDelegato.ComuneNascita;
                areaDelegato.D_TP1CAPRS = datiDelegato.CAP;
                areaDelegato.D_TP1CIVICO = datiDelegato.NCivico;
                areaDelegato.D_TP1COMUNE = datiDelegato.ComuneResidenza;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiDelegato.CodiceComuneResidenza, Utility.TipoAppartenenza.CI.ToString(), 0, false, out codiceInpsComune);
                //areaDelegato.D_TP1DTCCOM = codiceInpsComune.ToString();
                areaDelegato.D_TP1DTCOD = datiDelegato.CodiceDelegato.HasValue ? datiDelegato.CodiceDelegato.Value.ToString() : "";
                areaDelegato.D_TP1DTCOG = datiDelegato.Cognome;
                areaDelegato.D_TP1DTFISC = datiDelegato.CodiceFiscale;
                areaDelegato.D_TP1DTNASC = datiDelegato.DataNascita.HasValue ?
                    int.Parse(datiDelegato.DataNascita.Value.Day.ToString().PadLeft(2, '0') +
                    datiDelegato.DataNascita.Value.Month.ToString().PadLeft(2, '0') +
                    datiDelegato.DataNascita.Value.Year.ToString().PadLeft(4, '0')) : 0;
                areaDelegato.D_TP1DTNOM = datiDelegato.Nome;
                areaDelegato.D_TP1DTPROR = datiDelegato.ProvinciaResidenza;
                areaDelegato.D_TP1DTSES = datiDelegato.Sesso.HasValue ? datiDelegato.Sesso.Value.ToString() : "";
                if (datiDelegato.ResidenzaEstero.HasValue && datiDelegato.ResidenzaEstero.Value)
                {
                    areaDelegato.D_TP1ESTITA = "9";
                    areaDelegato.D_TP1GP1DRESIDOM = "2";

                }
                else if (datiDelegato.ResidenzaEstero.HasValue && !datiDelegato.ResidenzaEstero.Value)
                {
                    areaDelegato.D_TP1ESTITA = "1";
                    areaDelegato.D_TP1GP1DRESIDOM = "1";
                }
                else
                {
                    areaDelegato.D_TP1ESTITA = "";
                    areaDelegato.D_TP1GP1DRESIDOM = "";
                }
                areaDelegato.D_TP1FRAZIO = datiDelegato.FrazioneResidenza;
                areaDelegato.D_TP1GP1AP25 = datiDelegato.ProvinciaNascita;

                if (datiDelegato.Indirizzo.Trim().Length > 52)
                {
                    areaDelegato.D_TP1VIA1 = datiDelegato.Indirizzo.Trim().Substring(0, 52);
                    if (datiDelegato.Indirizzo.Trim().Length > 104)
                    {
                        areaDelegato.D_TP1VIA2 = datiDelegato.Indirizzo.Trim().Substring(52, 52);
                        if (datiDelegato.Indirizzo.Trim().Length > 156)
                        {
                            areaDelegato.D_TP1VIA3 = datiDelegato.Indirizzo.Trim().Substring(104, 52);
                            if (datiDelegato.Indirizzo.Trim().Length > 208)
                                areaDelegato.D_TP1VIA4 = datiDelegato.Indirizzo.Trim().Substring(156, 52);
                            else
                                areaDelegato.D_TP1VIA4 = datiDelegato.Indirizzo.Trim().Substring(156);
                        }
                        else
                            areaDelegato.D_TP1VIA3 = datiDelegato.Indirizzo.Trim().Substring(104);
                    }
                    else
                        areaDelegato.D_TP1VIA2 = datiDelegato.Indirizzo.Trim().Substring(52);
                }
                else
                    areaDelegato.D_TP1VIA1 = datiDelegato.Indirizzo.Trim();
            }

        }

        private static void ValorizzaAreaTutore(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaTutore areaTutore)
        {
            areaTutore = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaTutore();
            GestioneAnagrafica.DatiAnagrafici datiTutore = null;
            GestioneDelegatoTutore.GetTutoreByIdPensione(datiPensione.Id, out datiTutore);
            if (datiTutore != null)
            {
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiTutore.CodiceComuneNascita, Utility.TipoAppartenenza.CI.ToString(), 0, false, out codiceInpsComune);
                areaTutore.T_TP1AP23 = codiceInpsComune;
                areaTutore.T_TP1AP24 = datiTutore.ComuneNascita;
                areaTutore.T_TP1CAPRS = datiTutore.CAP;
                areaTutore.T_TP1CIVICO = datiTutore.NCivico;
                areaTutore.T_TP1COMUNE = datiTutore.ComuneResidenza;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiTutore.CodiceComuneResidenza, Utility.TipoAppartenenza.CI.ToString(), 0, false, out codiceInpsComune);
                //areaTutore.T_TP1DTCCOM = codiceInpsComune.ToString();
                areaTutore.T_TP1DTCOD = datiTutore.CodiceTutore.HasValue ? datiTutore.CodiceTutore.Value.ToString() : "";
                areaTutore.T_TP1DTCOG = datiTutore.Cognome;
                areaTutore.T_TP1DTFISC = datiTutore.CodiceFiscale;
                areaTutore.T_TP1DTNASC = datiTutore.DataNascita.HasValue ?
                    int.Parse(datiTutore.DataNascita.Value.Day.ToString().PadLeft(2, '0') +
                    datiTutore.DataNascita.Value.Month.ToString().PadLeft(2, '0') +
                    datiTutore.DataNascita.Value.Year.ToString().PadLeft(4, '0')) : 0;
                areaTutore.T_TP1DTNOM = datiTutore.Nome;
                areaTutore.T_TP1DTPROR = datiTutore.ProvinciaResidenza;
                areaTutore.T_TP1DTSES = datiTutore.Sesso.HasValue ? datiTutore.Sesso.Value.ToString() : "";
                if (datiTutore.ResidenzaEstero.HasValue && datiTutore.ResidenzaEstero.Value)
                {
                    areaTutore.T_TP1ESTITA = "9";
                    areaTutore.T_TP1GP1DRESIDOM = "2";
                }
                else if (datiTutore.ResidenzaEstero.HasValue && !datiTutore.ResidenzaEstero.Value)
                {
                    areaTutore.T_TP1ESTITA = "1";
                    areaTutore.T_TP1GP1DRESIDOM = "1";
                }
                else
                {
                    areaTutore.T_TP1ESTITA = "";
                    areaTutore.T_TP1GP1DRESIDOM = "";
                }
                areaTutore.T_TP1FRAZIO = datiTutore.FrazioneResidenza;
                areaTutore.T_TP1GP1AP25 = datiTutore.ProvinciaNascita;

                if (datiTutore.Indirizzo.Trim().Length > 52)
                {
                    areaTutore.T_TP1VIA1 = datiTutore.Indirizzo.Trim().Substring(0, 52);
                    if (datiTutore.Indirizzo.Trim().Length > 104)
                    {
                        areaTutore.T_TP1VIA2 = datiTutore.Indirizzo.Trim().Substring(52, 52);
                        if (datiTutore.Indirizzo.Trim().Length > 156)
                        {
                            areaTutore.T_TP1VIA3 = datiTutore.Indirizzo.Trim().Substring(104, 52);
                            if (datiTutore.Indirizzo.Trim().Length > 208)
                                areaTutore.T_TP1VIA4 = datiTutore.Indirizzo.Trim().Substring(156, 52);
                            else
                                areaTutore.T_TP1VIA4 = datiTutore.Indirizzo.Trim().Substring(156);
                        }
                        else
                            areaTutore.T_TP1VIA3 = datiTutore.Indirizzo.Trim().Substring(104);
                    }
                    else
                        areaTutore.T_TP1VIA2 = datiTutore.Indirizzo.Trim().Substring(52);
                }
                else
                    areaTutore.T_TP1VIA1 = datiTutore.Indirizzo.Trim();
            }

        }

        private static void ValorizzaAreaDati(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            out GestionePagamento.DatiPagamento datiPagamento, out Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioneBenefici,
            GestionePensione.DatiEliminazione datiEliminazione, bool isRiapertura, out Data.PCIINPU7.AreaDati areaDati)
        {
            areaDati = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaDati();

            areaDati.DAT4218 = datiPensione.DataRicezionePrenotazioneCentrale.HasValue ? int.Parse(datiPensione.DataRicezionePrenotazioneCentrale.Value.Day.ToString().PadLeft(2, '0') +
                datiPensione.DataRicezionePrenotazioneCentrale.Value.Month.ToString().PadLeft(2, '0') +
                datiPensione.DataRicezionePrenotazioneCentrale.Value.Year.ToString().PadLeft(4, '0').Substring(2, 2)) : 0;

            datiPagamento = null;
            GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (datiPagamento != null)
            {
                areaDati.TP1ABI = datiPagamento.ABI.HasValue ? datiPagamento.ABI.Value : 0;

                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiapertura)
                {
                    string cab = "";
                    if (datiPagamento.TipoPagamento.HasValue && datiPagamento.TipoPagamento != 'P' && datiPagamento.CAB.HasValue && datiPagamento.CAB.Value != 0)
                    {
                        if (datiPagamento.TipoPagamento == 'E')
                            cab = datiPagamento.CAB.Value.ToString();
                        else
                            cab = datiPagamento.CAB.Value.ToString().PadLeft(7, '0');
                    }
                    else if (datiPagamento.TipoPagamento.HasValue && datiPagamento.TipoPagamento == 'P')
                    {
                        if (datiPagamento.ABI.GetValueOrDefault() == 07601 && datiPagamento.Frazionario.HasValue && datiPagamento.Frazionario.Value != 0)
                            cab = datiPagamento.Frazionario.Value.ToString().PadLeft(7, '0');
                        else if (datiPagamento.CAB.HasValue && datiPagamento.CAB.Value != 0)
                            cab = datiPagamento.CAB.Value.ToString().PadLeft(7, '0');
                    }

                    if (!string.IsNullOrEmpty(cab))
                    {
                        areaDati.TP1LIRE_EURO = cab.Substring(0, 1);
                        areaDati.TP1SEDEUP = cab.Substring(1, 4);
                        areaDati.TP1CIN = cab.Substring(5, 1);
                        areaDati.TP1COSTA = cab.Substring(6, 1);
                    }

                    areaDati.TP1MODPAG = datiPagamento.ModalitaPagamento.HasValue ? datiPagamento.ModalitaPagamento.Value.ToString() : "";
                }

                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                {
                    string cab = "";
                    if (datiPagamento.CAB.HasValue)
                        cab = datiPagamento.CAB.Value.ToString().PadLeft(7, '0');
                    else if (datiPagamento.Frazionario.HasValue)
                        cab = datiPagamento.Frazionario.Value.ToString().PadLeft(7, '0');

                    areaDati.TP1LIRE_EURO = cab.Substring(0, 1);
                    areaDati.TP1SEDEUP = cab.Substring(1, 4);
                    areaDati.TP1CIN = cab.Substring(5, 1);
                    areaDati.TP1COSTA = cab.Substring(6, 1);
                }
            }

            areaDati.TP1NDOM_SIAL_S = short.Parse(datiPensione.NDomus.ToString().PadLeft(13, '0').Substring(0, 4));
            areaDati.TP1NDOM_SIAL_G = short.Parse(datiPensione.NDomus.ToString().PadLeft(13, '0').Substring(4, 4));
            areaDati.TP1NDOM_SIAL_P = int.Parse(datiPensione.NDomus.ToString().PadLeft(13, '0').Substring(8, 5));

            if (datiIstruttoria != null)
            {
                areaDati.TP1REQRID = datiIstruttoria.Legge44997.HasValue ? datiIstruttoria.Legge44997.Value : (short)0;
                areaDati.TP1CONTRATTO = datiIstruttoria.CodiceContrattoEquiparato.HasValue ? datiIstruttoria.CodiceContrattoEquiparato.Value : (short)0;
                areaDati.TP1LIVELLO = datiIstruttoria.CodiceLivelloEquip.HasValue ? datiIstruttoria.CodiceLivelloEquip.Value : (short)0;
                areaDati.TP1MOBILITA = datiIstruttoria.CodiceMobilita.HasValue ? datiIstruttoria.CodiceMobilita.Value : (short)0;
                areaDati.IW1CODOPZ = datiIstruttoria.CodiceOpzioneRiliquidazione.HasValue ? datiIstruttoria.CodiceOpzioneRiliquidazione.Value : (short)0;
                areaDati.IW1OPZAN = datiIstruttoria.DataDomandaOpzione.HasValue ? (short)datiIstruttoria.DataDomandaOpzione.Value.Year : (short)0;
                areaDati.IW1OPZMM = datiIstruttoria.DataDomandaOpzione.HasValue ? (short)datiIstruttoria.DataDomandaOpzione.Value.Month : (short)0;
                areaDati.IW1OPZGG = datiIstruttoria.DataDomandaOpzione.HasValue ? (short)datiIstruttoria.DataDomandaOpzione.Value.Day : (short)0;
            }

            if (datiEliminazione != null)
            {
                List<GestioneDecodifica.CodiceEliminazione> lstDecCodElim;
                GestioneDecodifica.GetCodiceEliminazioneByTipologia(out lstDecCodElim, Utility.TipoAppartenenza.CI);

                areaDati.TP1CODELIM = datiEliminazione.CodiceMotivo.HasValue ? lstDecCodElim.Find(x => x.Id == datiEliminazione.CodiceMotivo.ToString()).TraduzioneSuGP.ToString() : "0";
            }
            else
                areaDati.TP1CODELIM = "0";

            datiMaggiorazioneBenefici = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioneBenefici);
            if (datiMaggiorazioneBenefici != null)
            {
                areaDati.TP1USURA = datiMaggiorazioneBenefici.Attivitausuranti.HasValue ? datiMaggiorazioneBenefici.Attivitausuranti.Value ? (short)1 : (short)0 : (short)0;
            }
        }

        private static void ValorizzaAreaW1L(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC,
            GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioneBenefici,
            GestionePensione.DatiEliminazione datiEliminazione, out GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            out List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, bool isRiapertura, out Data.PCIINPU7.AreaW1L areaW1L)
        {
            areaW1L = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW1L();

            if (Utility.IsDomandaAPEPrecoci(datiPensione))
                areaW1L.IW1SESEZ = datiPensione.CodiceSede;
            else
                areaW1L.IW1SESEZ = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value : datiPensione.CodiceSede;

            listaPrestazioniEE = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEE);
            if (datiPensione.FlagVerify.HasValue)
            {
                if (datiPensione.Gruppo == "0031" || isRiapertura)
                    areaW1L.IW1TIPEL = datiPensione.FlagVerify.Value ? "V" : "R";
                else
                {
                    //casistica O solo per conv. 13/14/26 per verify
                    if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0 && datiPensione.FlagVerify.Value)
                    {
                        if (listaPrestazioniEE[0].CodiceConvenzione.HasValue &&
                            (listaPrestazioniEE[0].CodiceConvenzione.Value == 13 || listaPrestazioniEE[0].CodiceConvenzione.Value == 14 ||
                            listaPrestazioniEE[0].CodiceConvenzione.Value == 26))
                            areaW1L.IW1TIPEL = "O";
                        else
                            areaW1L.IW1TIPEL = "V";
                    }
                    else
                        areaW1L.IW1TIPEL = datiPensione.FlagVerify.Value ? "V" : "L";
                }
            }
            areaW1L.IW1CARIC = datiPensione.CausaCarico.HasValue ? datiPensione.CausaCarico.Value : (short)0;
            areaW1L.IW1DEOSEC = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
            areaW1L.IW1DEOAA = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0').Substring(2, 2)) : (short)0;
            areaW1L.IW1DEORM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
            areaW1L.IW1DA1A = datiPensione.DataInizioCalcolo.HasValue ? (short)datiPensione.DataInizioCalcolo.Value.Year : (short)0;
            areaW1L.IW1DA1M = datiPensione.DataInizioCalcolo.HasValue ? (short)datiPensione.DataInizioCalcolo.Value.Month : (short)0;
            areaW1L.IW1CAT8 = datiPensione.SiglaCategoria;
            string codCategoria = datiPensione.GetCodCategoria();
            short resShort = 0;
            short.TryParse(codCategoria, out resShort);
            areaW1L.IW1CATPEN = resShort;
            areaW1L.IW1CERT = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
            if (datiPensione.SiglaCategoria.Length >= 1 && datiPensione.SiglaCategoria.Substring(0, 1) == "I" && datiPensione.DecorrenzaOriginaria.HasValue &&
                Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1984, 7, 1)))
            {
                if (datiPensione.NaturaPensione.Length >= 1 &&
                    (datiPensione.NaturaPensione.Substring(0, 1) == "3" || datiPensione.NaturaPensione.Substring(0, 1) == "4"))
                    areaW1L.IW1TIPINV = 2;
                else
                    areaW1L.IW1TIPINV = 1;
            }

            if (datiAnagraficiDC != null)
            {
                areaW1L.IW1DSES = datiAnagraficiDC.Sesso.HasValue ? datiAnagraficiDC.Sesso.Value.ToString() : "";
                areaW1L.IW1DNASA = datiAnagraficiDC.DataNascita.HasValue ? (short)datiAnagraficiDC.DataNascita.Value.Year : (short)0;
                areaW1L.IW1DNASM = datiAnagraficiDC.DataNascita.HasValue ? (short)datiAnagraficiDC.DataNascita.Value.Month : (short)0;
                areaW1L.IW1DNASG = datiAnagraficiDC.DataNascita.HasValue ? (short)datiAnagraficiDC.DataNascita.Value.Day : (short)0;

                if (danteCausa != null)
                {
                    areaW1L.IW1DEDIRA = danteCausa.DecorrenzaPensione.HasValue ? (short)danteCausa.DecorrenzaPensione.Value.Year : (short)0;
                    areaW1L.IW1DEDIRM = danteCausa.DecorrenzaPensione.HasValue ? (short)danteCausa.DecorrenzaPensione.Value.Month : (short)0;
                    areaW1L.IW1DMORA = danteCausa.DataMorte.HasValue ? (short)danteCausa.DataMorte.Value.Year : (short)0;
                    areaW1L.IW1DMORM = danteCausa.DataMorte.HasValue ? (short)danteCausa.DataMorte.Value.Month : (short)0;
                    areaW1L.IW1DMORG = danteCausa.DataMorte.HasValue ? (short)danteCausa.DataMorte.Value.Day : (short)0;
                    areaW1L.IW1780CD = danteCausa.Maggiorazione781Contributi.HasValue ? danteCausa.Maggiorazione781Contributi.Value : (short)0;
                    areaW1L.IW1DPROV = danteCausa.ProvenienzaPensione.HasValue ? danteCausa.ProvenienzaPensione.Value : (short)0;
                    //ENG - Implementata la gestione mancante per le Reversibilità
                    if (Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, danteCausa) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa))
                    {
                        if (danteCausa.CodiceTipoPerequazione.HasValue)
                            areaW1L.IW1CRIRIL = danteCausa.CodiceTipoPerequazione.Value;
                    }
                }
            }

            datiGenericiCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiCi);
            if (datiGenericiCi != null)
            {
                areaW1L.IW1NOAF86 = datiGenericiCi.ConiugeSuperstite.HasValue ? (short)datiGenericiCi.ConiugeSuperstite.Value : (short)0;
            }

            if (datiIstruttoria != null)
            {
                areaW1L.IW1DEOPA = datiIstruttoria.DecorrenzaOpzione.HasValue ? (short)datiIstruttoria.DecorrenzaOpzione.Value.Year : (short)0;
                areaW1L.IW1DEOPM = datiIstruttoria.DecorrenzaOpzione.HasValue ? (short)datiIstruttoria.DecorrenzaOpzione.Value.Month : (short)0;

                if (!(Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, danteCausa) && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa)))
                {
                    areaW1L.IW1CRIRIL = datiIstruttoria.RiliquidazionePostCristallizzazione.HasValue ? (short)char.GetNumericValue(datiIstruttoria.RiliquidazionePostCristallizzazione.Value) : (short)0;
                }
            }

            if (areaTitolare != null && areaTitolare.Anagrafica != null)
            {
                areaW1L.IW1SECAN = areaTitolare.Anagrafica.DataNascita.HasValue ? (short)areaTitolare.Anagrafica.DataNascita.Value.Year : (short)0;
                areaW1L.IW1NATITM = areaTitolare.Anagrafica.DataNascita.HasValue ? (short)areaTitolare.Anagrafica.DataNascita.Value.Month : (short)0;
                areaW1L.IW1NATITG = areaTitolare.Anagrafica.DataNascita.HasValue ? (short)areaTitolare.Anagrafica.DataNascita.Value.Day : (short)0;
                areaW1L.IW1SESTIT = areaTitolare.Anagrafica.Sesso.HasValue ? areaTitolare.Anagrafica.Sesso.Value.ToString() : "";
            }

            if (datiEliminazione != null)
            {
                areaW1L.IW1DA2A = datiEliminazione.DataFineCalcoloArretrati.HasValue ? (short)datiEliminazione.DataFineCalcoloArretrati.Value.Year : (short)0;
                areaW1L.IW1DA2M = datiEliminazione.DataFineCalcoloArretrati.HasValue ? (short)datiEliminazione.DataFineCalcoloArretrati.Value.Month : (short)0;
            }

            if (datiMaggiorazioneBenefici != null)
            {
                areaW1L.IW1DEC544A = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.HasValue ? (short)datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.Value.Year : (short)0;
                areaW1L.IW1DEC544M = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.HasValue ? (short)datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.Value.Month : (short)0;
                areaW1L.IW1DECEXA = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneArt6.HasValue ? (short)datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneArt6.Value.Year : (short)0;
                areaW1L.IW1DECEXM = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneArt6.HasValue ? (short)datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneArt6.Value.Month : (short)0;
                areaW1L.IW1DECMS1A = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneLegge140.HasValue ? (short)datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneLegge140.Value.Year : (short)0;
                areaW1L.IW1DECMS1M = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneLegge140.HasValue ? (short)datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneLegge140.Value.Month : (short)0;
                areaW1L.IW1TM59B = datiMaggiorazioneBenefici.AumentoMensileLegge5991Comma9.HasValue ? datiMaggiorazioneBenefici.AumentoMensileLegge5991Comma9.Value : 0M;
                //IW1CODEX
                areaW1L.IW1CODEX = datiMaggiorazioneBenefici.Articolo6140.HasValue ? (short)datiMaggiorazioneBenefici.Articolo6140.Value : (short)0;
            }
        }

        private static void ValorizzaAreaW2CL(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, out Data.PCIINPU7.AreaW2CL areaW2CL)
        {
            areaW2CL = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW2CL();

            areaW2CL.ICI2DADASEC = short.Parse(datiPensione.DataPresentazioneDomanda.Year.ToString().PadLeft(4, '0').Substring(0, 2));
            areaW2CL.ICI2DADAA = short.Parse(datiPensione.DataPresentazioneDomanda.Year.ToString().PadLeft(4, '0').Substring(2, 2));
            areaW2CL.ICI2DAMM = (short)datiPensione.DataPresentazioneDomanda.Month;
            areaW2CL.ICI2DAGG = (short)datiPensione.DataPresentazioneDomanda.Day;

            if (datiAnagrafici != null)
            {
                if (datiAnagrafici.CodiceComuneResidenza != null && datiAnagrafici.CodiceComuneResidenza.StartsWith("Z"))
                    areaW2CL.ICI2RESEST = datiAnagrafici.ProvinciaResidenza;
                else
                    areaW2CL.ICI2RESEST = "I";
            }
            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
            {
                areaW2CL.ICI2CONV = listaPrestazioniEE[0].CodiceConvenzione.HasValue ? (short)listaPrestazioniEE[0].CodiceConvenzione.Value : (short)0;
            }

            if (datiGenericiCi != null)
            {
                areaW2CL.ICI2REGLIQ = datiGenericiCi.RegimeLiquidazione.HasValue ? datiGenericiCi.RegimeLiquidazione.Value.ToString() : "";
            }
        }

        private static void ValorizzaAreaW2(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaW2 areaW2)
        {
            areaW2 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW2();

            areaW2.IABCONA2 = datiPensione.NaturaPensione.PadRight(3, ' ').Substring(0, 1);
            areaW2.IABCONA3 = datiPensione.NaturaPensione.PadRight(3, ' ').Substring(1, 1);
            areaW2.IABCONA4 = datiPensione.NaturaPensione.PadRight(3, ' ').Substring(2, 1);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);
            if (datiStoricoGP != null)
            {
                areaW2.IABTIPEN = datiStoricoGP.IABTIPEN;
            }

            if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione))
                areaW2.IABTIPEN = "8";

            GestionePensione.DatiSindacato datiSindacato = null;
            GestionePensione.GetSindacatoByIdPensione(datiPensione.Id, out datiSindacato);
            if (datiSindacato != null && Utility.IsSindacatoPresente(datiSindacato.CodiceSindacato))
            {
                areaW2.IABCOSIND = datiSindacato.CodiceSindacato;
            }
            else
                areaW2.IABCOSIND = "00";
        }

        private static void ValorizzaAreaVarie(GestionePensione.DatiPensione datiPensione, GestionePagamento.DatiPagamento datiPagamento,
            Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC,
            GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, bool isRiapertura,
            out List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo,
            out Data.PCIINPU7.AreaVarie areaVarie)
        {
            areaVarie = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie();
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            DateTime dataSistema = Utility.DataSistemaCi;

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            GestioneControlliDinamici.ControlloDinamico ctrl06_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo06_2024", out ctrl06_2024);

            GestioneControlliDinamici.ControlloDinamico ctrl_SbloccaMetaProcesso = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrl_SbloccaMetaProcesso);

            short resShort = 0;

            areaVarie.DECPERFREQ_A = datiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)datiPensione.DataPerfezionamentoRequisiti.Value.Year : (short)0;
            areaVarie.DECPERFREQ_M = datiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)datiPensione.DataPerfezionamentoRequisiti.Value.Month : (short)0;
            areaVarie.DECPERFREQ_G = datiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)datiPensione.DataPerfezionamentoRequisiti.Value.Day : (short)0;
            areaVarie.IREQVE1294 = datiPensione.RequisitiVecchiaiaAl1294.HasValue ? datiPensione.RequisitiVecchiaiaAl1294.Value ? "2" : "1" : "0";
            areaVarie.IREQ300996 = datiPensione.RequisitiAl996.HasValue ? datiPensione.RequisitiAl996.Value ? "2" : "1" : "0";

            areaVarie.IPRIMADAAA = datiPensione.DataPrimaDomanda.HasValue ? (short)datiPensione.DataPrimaDomanda.Value.Year : (short)0;
            areaVarie.IPRIMADAMM = datiPensione.DataPrimaDomanda.HasValue ? (short)datiPensione.DataPrimaDomanda.Value.Month : (short)0;
            areaVarie.IPRIMADAGG = datiPensione.DataPrimaDomanda.HasValue ? (short)datiPensione.DataPrimaDomanda.Value.Day : (short)0;

            if (datiPagamento != null)
            {
                if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiapertura))
                {
                    areaVarie.BIC = datiPagamento.BIC;
                    areaVarie.IBAN = !string.IsNullOrEmpty(datiPagamento.IBAN) ? datiPagamento.IBAN.ToUpperInvariant() : string.Empty;
                    if (datiPagamento.TipoPagamento.GetValueOrDefault() == 'P' &&
                        datiPagamento.ModalitaPagamento.GetValueOrDefault() == 'L' && string.IsNullOrEmpty(areaVarie.IBAN))
                        areaVarie.IBAN = !string.IsNullOrEmpty(datiPagamento.Libretto) ? datiPagamento.Libretto.ToUpperInvariant() : string.Empty;

                    areaVarie.PAESEPAG = datiPagamento.TipoPagamento.HasValue ? datiPagamento.TipoPagamento.Value == 'E' ? "S" : "N" : "N";
                }

                areaVarie.AN87A = datiPagamento.TrattenutaInpdap.HasValue ? datiPagamento.TrattenutaInpdap.Value ? "SI" : "NO" : "";
                if (datiPagamento.DataRinunciaTrattenutaInpdap.HasValue && string.IsNullOrEmpty(areaVarie.AN87A))
                    areaVarie.AN87A = "NO";

                areaVarie.AN87DATAA = datiPagamento.DataRinunciaTrattenutaInpdap.HasValue ? (short)datiPagamento.DataRinunciaTrattenutaInpdap.Value.Year : (short)0;
                areaVarie.AN87DATAM = datiPagamento.DataRinunciaTrattenutaInpdap.HasValue ? (short)datiPagamento.DataRinunciaTrattenutaInpdap.Value.Month : (short)0;
            }

            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out listaResidenzeEstere);
            if (listaResidenzeEstere != null && listaResidenzeEstere.Count > 0)
            {
                areaVarie.DATIRESIDENZA = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.DatiResidenza>();
                foreach (GestioneAnagrafica.DatiResidenzaEstero res in listaResidenzeEstere)
                {
                    Data.PCIINPU7.AreaVarie.DatiResidenza residenza = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.DatiResidenza();
                    residenza.ICODRES = "";
                    residenza.IDECRESAA = res.Decorrenza.HasValue ? res.Decorrenza.Value.Year : 0;
                    residenza.IDECRESMM = res.Decorrenza.HasValue ? (short)res.Decorrenza.Value.Month : (short)0;
                    if (res.CodCatastaleStatoEE == "Z000")
                        residenza.ICODRES = "I";
                    else
                    {
                        GestioneDecodifica.StatoEstero statoEstero = null;
                        GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(res.CodCatastaleStatoEE, out statoEstero);
                        if (statoEstero != null)
                            residenza.ICODRES = statoEstero.Sigla;
                    }
                    areaVarie.DATIRESIDENZA.Add(residenza);
                }
            }
            else
            {
                areaVarie.DATIRESIDENZA = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.DatiResidenza>();
                Data.PCIINPU7.AreaVarie.DatiResidenza residenza = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.DatiResidenza();
                residenza.ICODRES = "";
                residenza.IDECRESAA = datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria.Value.Year : 0;
                residenza.IDECRESMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                if (datiAnagrafici.ResidenzaEstero.HasValue && datiAnagrafici.ResidenzaEstero.Value)
                    residenza.ICODRES = datiAnagrafici.ProvinciaResidenza;
                else
                    residenza.ICODRES = "I";
                areaVarie.DATIRESIDENZA.Add(residenza);
            }

            areaVarie.N_DOMUS_13 = datiPensione.NDomus.ToString().PadLeft(13, '0');
            areaVarie.N_DOMUS_02 = "00";

            if (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0)
            {
                areaVarie.VARSTATICIVILI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.VarStatiCivili>();
                foreach (GestioneAnagrafica.DatiStatoCivile stCiv in areaTitolare.ElencoStatiCivili)
                {
                    INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.VarStatiCivili statoCivile = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.VarStatiCivili();
                    statoCivile.CODSCIV = stCiv.Codice.ToString();
                    statoCivile.DECSCIVA = stCiv.Decorrenza.HasValue ? (short)stCiv.Decorrenza.Value.Year : (short)0;
                    statoCivile.DECSCIVM = stCiv.Decorrenza.HasValue ? (short)stCiv.Decorrenza.Value.Month : (short)0;
                    areaVarie.VARSTATICIVILI.Add(statoCivile);
                }
            }

            List<GestioneDatiContributiviCi.PensioniCiImportiValuta> listaImportiValuta = null;
            GestioneDatiContributiviCi.GetImportiEsteriValutaByIdPensione(datiPensione.Id, out listaImportiValuta);
            if (listaImportiValuta != null && listaImportiValuta.Count > 0)
            {
                areaVarie.IMPORTIESTERI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.ImportiEsteri>();
                foreach (GestioneDatiContributiviCi.PensioniCiImportiValuta importoValuta in listaImportiValuta)
                {
                    Data.PCIINPU7.AreaVarie.ImportiEsteri importoEstero = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaVarie.ImportiEsteri();
                    importoEstero.DECESTLA = importoValuta.DecorrenzaPrestazioneEE.HasValue ? (short)importoValuta.DecorrenzaPrestazioneEE.Value.Year : (short)0;
                    importoEstero.DECESTLM = importoValuta.DecorrenzaPrestazioneEE.HasValue ? (short)importoValuta.DecorrenzaPrestazioneEE.Value.Month : (short)0;
                    importoEstero.IMPESTL = importoValuta.ImportoPrestazioneEE.HasValue ? importoValuta.ImportoPrestazioneEE.Value : 0M;
                    areaVarie.IMPORTIESTERI.Add(importoEstero);
                }
            }

            if (datiAnagraficiDC != null)
            {
                if (!string.IsNullOrEmpty(datiAnagraficiDC.Cittadinanza))
                {
                    List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
                    GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);
                    if (listaStatiEsteri != null)
                    {
                        string citt = datiAnagraficiDC.Cittadinanza;
                        GestioneDecodifica.StatoEstero statoEstero = listaStatiEsteri.Find(x => x.CodCatastale == citt);
                        if (statoEstero != null)
                        {
                            areaVarie.CITTDC = !string.IsNullOrEmpty(statoEstero.Sigla) ? statoEstero.Sigla.Trim() == "ITA" ? "I" : statoEstero.Sigla.Trim() : string.Empty;
                        }
                    }
                }

                areaVarie.TP1COFI_DC = datiAnagraficiDC.CodiceFiscale;
                areaVarie.DC_ARCA_UNO = datiAnagraficiDC.Codice1Arca;
                int resInt = 0;
                if (!string.IsNullOrEmpty(datiAnagraficiDC.Codice2Arca))
                    int.TryParse(datiAnagraficiDC.Codice2Arca.Trim(), out resInt);
                areaVarie.DC_ARCA_DUE = resInt;

                areaVarie.DATA_MATRIM_A = datiAnagraficiDC.DataMatrimonio.HasValue ? (short)datiAnagraficiDC.DataMatrimonio.Value.Year : (short)0;
                areaVarie.DATA_MATRIM_M = datiAnagraficiDC.DataMatrimonio.HasValue ? (short)datiAnagraficiDC.DataMatrimonio.Value.Month : (short)0;
                areaVarie.DATA_MATRIM_G = datiAnagraficiDC.DataMatrimonio.HasValue ? (short)datiAnagraficiDC.DataMatrimonio.Value.Day : (short)0;

                if (danteCausa != null)
                {
                    areaVarie.DECRESDCA = danteCausa.DecorrenzaResidenza.HasValue ? (short)danteCausa.DecorrenzaResidenza.Value.Year : (short)0;
                    areaVarie.DECRESDCM = danteCausa.DecorrenzaResidenza.HasValue ? (short)danteCausa.DecorrenzaResidenza.Value.Month : (short)0;
                    if (!string.IsNullOrEmpty(danteCausa.NaturaPensione))
                    {
                        areaVarie.IW8NAT1_DC = danteCausa.NaturaPensione.PadRight(3, ' ').Substring(0, 1);
                        areaVarie.IW8NAT2_DC = danteCausa.NaturaPensione.PadRight(3, ' ').Substring(1, 1);
                        areaVarie.IW8NAT3_DC = danteCausa.NaturaPensione.PadRight(3, ' ').Substring(2, 1);
                    }
                    else
                    {
                        areaVarie.IW8NAT1_DC = " ";
                        areaVarie.IW8NAT2_DC = " ";
                        areaVarie.IW8NAT3_DC = " ";
                    }
                    if (!string.IsNullOrEmpty(danteCausa.CategoriaAltraPensione))
                    {
                        resShort = 0;
                        short.TryParse(danteCausa.CategoriaAltraPensione, out resShort);
                        areaVarie.IAPCATEG_DC = resShort != 0 ? resShort.ToString().PadLeft(3, '0') : danteCausa.CategoriaAltraPensione.PadLeft(3, ' ');
                    }
                    areaVarie.IAPCESSAA_DC = danteCausa.CessazioneAltraPensione.HasValue ? (short)danteCausa.CessazioneAltraPensione.Value.Year : (short)0;
                    areaVarie.IAPCESSAM_DC = danteCausa.CessazioneAltraPensione.HasValue ? (short)danteCausa.CessazioneAltraPensione.Value.Month : (short)0;
                    if (danteCausa.CodiceImportoAltraPensione.HasValue)
                    {
                        resShort = 0;
                        short.TryParse(danteCausa.CodiceImportoAltraPensione.Value.ToString(), out resShort);
                        areaVarie.IAPCODIMP_DC = resShort;
                    }
                    areaVarie.IAPDECORA_DC = danteCausa.DecorrenzaAltraPensione.HasValue ? (short)danteCausa.DecorrenzaAltraPensione.Value.Year : (short)0;
                    areaVarie.IAPDECORM_DC = danteCausa.DecorrenzaAltraPensione.HasValue ? (short)danteCausa.DecorrenzaAltraPensione.Value.Month : (short)0;
                    areaVarie.IAPUNIC_DC = danteCausa.CodiceUCAltraPensione.HasValue ? danteCausa.CodiceUCAltraPensione.Value.ToString() : "";
                }
            }

            if (datiGenericiCi != null)
            {
                areaVarie.IW1DEBONA = datiGenericiCi.DecorrenzaBonus.HasValue ? (short)datiGenericiCi.DecorrenzaBonus.Value.Year : (short)0;
                areaVarie.IW1DEBONM = datiGenericiCi.DecorrenzaBonus.HasValue ? (short)datiGenericiCi.DecorrenzaBonus.Value.Month : (short)0;
                areaVarie.ICODVIRT = datiGenericiCi.CodiceVirtuale.HasValue ? datiGenericiCi.CodiceVirtuale.Value.ToString() : "";
                areaVarie.IDEL126 = datiGenericiCi.DeliberaCee126.HasValue ? datiGenericiCi.DeliberaCee126.Value ? "S" : "N" : "N";
                areaVarie.IIMPASSEST = datiGenericiCi.ImportoPensioneEEInvalido.HasValue ? datiGenericiCi.ImportoPensioneEEInvalido.Value : 0M;
                areaVarie.IW1C495 = datiGenericiCi.ApplicazioneSentenza49593.HasValue ? datiGenericiCi.ApplicazioneSentenza49593.ToString() : null;
                areaVarie.COD_RIDUZIONE = datiGenericiCi.RiduzioneRetributiva ? "S" : "N";
                areaVarie.PER_RIDUZIONE = datiGenericiCi.RiduzioneRetributivaPercentuale.HasValue ? datiGenericiCi.RiduzioneRetributivaPercentuale.Value : 0M;
            }

            if (datiIstruttoria != null)
            {
                areaVarie.IREQPARD = datiIstruttoria.CodiceRequisitiParticolari.HasValue ? datiIstruttoria.CodiceRequisitiParticolari.Value : (short)0;
                areaVarie.IADASS = datiIstruttoria.ImportoAdeguataAoi.HasValue ? datiIstruttoria.ImportoAdeguataAoi.Value : 0M;
                areaVarie.IDECASSA = datiIstruttoria.DecorrenzaOriginariaAltraPensione.HasValue ? (short)datiIstruttoria.DecorrenzaOriginariaAltraPensione.Value.Year : (short)0;
                areaVarie.IDECASSM = datiIstruttoria.DecorrenzaOriginariaAltraPensione.HasValue ? (short)datiIstruttoria.DecorrenzaOriginariaAltraPensione.Value.Month : (short)0;
                areaVarie.IIMPASS = datiIstruttoria.ImportoPagamentoAoi.HasValue ? datiIstruttoria.ImportoPagamentoAoi.Value : 0M;
                if (Utility.IsDomandaAPEPrecoci(datiPensione))
                    areaVarie.COD_C_OPERATIVO = datiPensione.CentroOperativo.GetValueOrDefault();
                else if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.CI) && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                {
                    if (Utility.isRicostituzioneOrRiaperturaPolarizzata(datiPensione, isRiapertura))
                        areaVarie.COD_C_OPERATIVO = datiPensione.CentroOperativoGP1ALZ6.GetValueOrDefault();
                    else
                        areaVarie.COD_C_OPERATIVO = datiPensione.CentroOperativo.GetValueOrDefault();
                }
                else if (datiPensione.CentroOperativoDestinazione.HasValue)
                    areaVarie.COD_C_OPERATIVO = datiPensione.CentroOperativoDestinazione.Value;
                else
                    areaVarie.COD_C_OPERATIVO = datiPensione.CentroOperativo.HasValue ? (short)datiPensione.CentroOperativo.GetValueOrDefault() : (short)0;
                areaVarie.ESEFIS_TERR = datiIstruttoria.CodiceComunicazioneCampo4.HasValue ? datiIstruttoria.CodiceComunicazioneCampo4.Value == 1 ? "SI" : "NO" : "NO";
                if (areaVarie.ESEFIS_TERR == "NO" && datiDetrazioni != null && datiDetrazioni.DetrazioniReddito.HasValue && datiDetrazioni.DetrazioniReddito.Value == 3)
                    areaVarie.ESEFIS_TERR = "SI";
                areaVarie.ESEFIS_EST = datiIstruttoria.CodiceComunicazioneCampo4.HasValue ? datiIstruttoria.CodiceComunicazioneCampo4.Value == 2 ? "SI" : "NO" : "NO";
                if (areaVarie.ESEFIS_EST == "NO" && datiDetrazioni != null && datiDetrazioni.DetrazioniReddito.HasValue && datiDetrazioni.DetrazioniReddito.Value == 2)
                    areaVarie.ESEFIS_EST = "SI";

                GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
                GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

                if (isRiapertura)
                    areaVarie.PEN_PROVV = "Z";
                else if (datiNuoveLiquidate.FlagProvvisoria.HasValue && datiNuoveLiquidate.FlagProvvisoria.Value || (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                    !string.IsNullOrEmpty(datiIstruttoria.ModalitaLiquidazione)))
                    areaVarie.PEN_PROVV = "X";

                List<GestioneDecodifica.DecModalitaLiquidazione> elencoDecModalitaLiquidazione = null;
                GestioneDecodifica.GetElencoDecModalitaLiquidazione(out elencoDecModalitaLiquidazione);

                if (!string.IsNullOrEmpty(datiIstruttoria.ModalitaLiquidazione))
                {
                    if (elencoDecModalitaLiquidazione != null && elencoDecModalitaLiquidazione.Count > 0)
                    {
                        string modalitaLiquidazione = datiIstruttoria.ModalitaLiquidazione;
                        GestioneDecodifica.DecModalitaLiquidazione decModalitaLiquidazione = elencoDecModalitaLiquidazione.Find(x => x.ValoreAggPeco.Trim() == modalitaLiquidazione.Trim());
                        if (decModalitaLiquidazione != null)
                            areaVarie.DAFELPE_CPROV = decModalitaLiquidazione.TraduzioneGp.ToString();
                    }
                }

                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    areaVarie.DAFELPE_DATA = int.Parse(dataSistema.Year.ToString().PadLeft(4, '0') + dataSistema.Month.ToString().PadLeft(2, '0') + dataSistema.Day.ToString().PadLeft(2, '0'));

            }
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.TipoAppartenenza.CI;

            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);
            int annoCompetenzaRinnovo = dataSistema.Year + 1;

            if (datiPensione.IsRicRinnovata.GetValueOrDefault())
                areaVarie.ANNO_COMPETENZA = (short)annoCompetenzaRinnovo;
            else
                areaVarie.ANNO_COMPETENZA = (short)annoCompetenza;

            GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);
            if (nuoveLiquidate != null)
            {
                //ENG Pensione Ovunque: bisogna ripristinare il flusso inziale e quindi inviare il Codice Processo di WebDom
                areaVarie.COD_PROCESSO = nuoveLiquidate.CodiceProcesso.HasValue ? nuoveLiquidate.CodiceProcesso.Value : (short)0;
            }

            areaVarie.COD_PROCEDURA = "R";

            areaVarie.IDECARPENA = datiPensione.DecorrenzaCalcoloArretrati.HasValue ? (short)datiPensione.DecorrenzaCalcoloArretrati.Value.Year : (short)0;
            areaVarie.IDECARPENM = datiPensione.DecorrenzaCalcoloArretrati.HasValue ? (short)datiPensione.DecorrenzaCalcoloArretrati.Value.Month : (short)0;

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("VersioneCalcoloCI", out ctrl);

            GestioneControlliDinamici.ControlloDinamico ctrlVersioneCalcoloCIInterregno = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("VersioneCalcoloCIInterregno ", out ctrlVersioneCalcoloCIInterregno);
            //per le RIC /TRF in fase di rinnovo bisogna passare nel campo IVERSIONE la lettera contenuta nel controllo dinamico VersioneCalcoloCIInterregno
            if (datiPensione.IsRicRinnovata.GetValueOrDefault())
            {
                areaVarie.IVERSIONE = ctrlVersioneCalcoloCIInterregno.ValoreControllo;
            }
            else if (ctrl != null)
            {
                areaVarie.IVERSIONE = ctrl.ValoreControllo;
            }

            if (datiAnagrafici != null)
            {
                areaVarie.T_ARCA_UNO = datiAnagrafici.Codice1Arca;
                int arca2 = 0;
                int.TryParse(datiAnagrafici.Codice2Arca, out arca2);
                areaVarie.T_ARCA_DUE = arca2;
            }

            if (datiMaggiorazioniBenefici != null)
            {
                areaVarie.IW1CES544A = datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue ? (short)datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value.Year : (short)0;
                areaVarie.IW1CES544M = datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue ? (short)datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value.Month : (short)0;
            }

            GestionePensione.DatiPatronato datiPatronato = null;
            GestionePensione.GetPatronatoByIdPensione(datiPensione.Id, out datiPatronato);
            if (datiPatronato != null)
            {
                short codEnte = 0;
                short.TryParse(datiPatronato.CodiceEnte, out codEnte);
                areaVarie.RICPCOD = codEnte;
                if (!string.IsNullOrEmpty(datiPatronato.NPratica) && datiPatronato.NPratica.ToString().Length <= 8)
                {
                    int nPratica = 0;
                    int.TryParse(datiPatronato.NPratica, out nPratica);
                    areaVarie.RICPNUM = nPratica;
                }
                if (!string.IsNullOrEmpty(datiPatronato.TipoUfficio))
                {
                    short tipoUfficio = 0;
                    short.TryParse(datiPatronato.TipoUfficio.Trim(), out tipoUfficio);
                    areaVarie.RICPTUFF = tipoUfficio == (short)2 ? (short)1 : tipoUfficio == (short)23 ? (short)2 : tipoUfficio;
                }
                areaVarie.RICPZON = datiPatronato.CodiceUfficio;
            }

            listaCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaCalcoloContributivo);
            if (listaCalcoloContributivo != null && listaCalcoloContributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo calcoloContributivo in listaCalcoloContributivo)
                {
                    if (calcoloContributivo.CodiceGestione.HasValue &&
                        (calcoloContributivo.NSettimaneQuotaDL214.HasValue || calcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue || calcoloContributivo.MontanteQuotaDL214.HasValue))
                    {
                        switch (calcoloContributivo.CodiceGestione.Value)
                        {
                            case 1: //AGO - OBG
                                areaVarie.ICISTOBG012 = calcoloContributivo.NSettimaneQuotaDL214.HasValue ? calcoloContributivo.NSettimaneQuotaDL214.Value : 0;
                                areaVarie.ICICONOBG012 = calcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? calcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                                areaVarie.ICIRETOBG012 = calcoloContributivo.MontanteQuotaDL214.HasValue ? calcoloContributivo.MontanteQuotaDL214.Value : 0M;
                                break;
                            case 2://CDCM
                                areaVarie.ICISTCDM012 = calcoloContributivo.NSettimaneQuotaDL214.HasValue ? calcoloContributivo.NSettimaneQuotaDL214.Value : 0;
                                areaVarie.ICICONCDM012 = calcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? calcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                                areaVarie.ICIRETCDM012 = calcoloContributivo.MontanteQuotaDL214.HasValue ? calcoloContributivo.MontanteQuotaDL214.Value : 0M;
                                break;
                            case 3://ART
                                areaVarie.ICISTART012 = calcoloContributivo.NSettimaneQuotaDL214.HasValue ? calcoloContributivo.NSettimaneQuotaDL214.Value : 0;
                                areaVarie.ICICONART012 = calcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? calcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                                areaVarie.ICIRETART012 = calcoloContributivo.MontanteQuotaDL214.HasValue ? calcoloContributivo.MontanteQuotaDL214.Value : 0M;
                                break;
                            case 4://COM
                                areaVarie.ICISTCOM012 = calcoloContributivo.NSettimaneQuotaDL214.HasValue ? calcoloContributivo.NSettimaneQuotaDL214.Value : 0;
                                areaVarie.ICICONCOM012 = calcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? calcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                                areaVarie.ICIRETCOM012 = calcoloContributivo.MontanteQuotaDL214.HasValue ? calcoloContributivo.MontanteQuotaDL214.Value : 0M;
                                break;
                        }
                    }
                }
            }

            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
            {
                areaVarie.T_E211 = new List<Data.PCIINPU7.AreaVarie.E211>();
                foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEE in listaPrestazioniEE)
                {
                    Data.PCIINPU7.AreaVarie.E211 e211 = new Data.PCIINPU7.AreaVarie.E211();
                    e211.C_PE_E211 = prestazioneEE.CodicePi.HasValue ? prestazioneEE.CodicePi.ToString() : string.Empty;
                    e211.CODICE_ISTITUZ_E211 = prestazioneEE.CodiceIstituzione.PadLeft(4, '0').Substring(1, 3);
                    e211.CODICE_STATO_E211 = prestazioneEE.CodiceStatoEE;
                    areaVarie.T_E211.Add(e211);
                }
            }

            //ENG - Gestione Pensione Estera e redditi Sentenza 495 valore ImportoMensilePensioneEstera da areaWK2R.IABML1Q a areaVarie.IAPIMPO_DC
            //ENG - Aggiornamento valori campi IW8RED_DC e IW8REDCON_DC
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
            {
                List<GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditiSentenza495 = null;
                GestioneDanteCausa.GetRedditiSentenza495_93ByIdPensione(datiPensione.Id, out listaDatiRedditiSentenza495);
                if (listaDatiRedditiSentenza495 != null && listaDatiRedditiSentenza495.Count(x => !x.IsPre2009.GetValueOrDefault() && x.FlagSentenza == null) > 0)
                {
                    GestioneDanteCausa.DatiRedditoSentenza495_93 redditiSentenza495 = listaDatiRedditiSentenza495.First(x => !x.IsPre2009.GetValueOrDefault() && x.FlagSentenza == null);
                    areaVarie.IW8DEC_DC = redditiSentenza495.AnnoReddito.GetValueOrDefault();
                    areaVarie.IW8RED_DC = redditiSentenza495.RedditoDaPensioneDC.GetValueOrDefault() + redditiSentenza495.RedditoTitolare.GetValueOrDefault();
                    areaVarie.IW8REDCON_DC = redditiSentenza495.RedditoDaPensioneConiuge.GetValueOrDefault() + redditiSentenza495.RedditoConiuge.GetValueOrDefault();
                    if (datiGenericiCi != null && datiGenericiCi.ImportoMensilePensioneEstera.HasValue)
                        areaVarie.IAPIMPO_DC = datiGenericiCi.ImportoMensilePensioneEstera.GetValueOrDefault();
                }
            }

            //ENG - Implementata la gestione mancante per le Reversibilità
            if (Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, danteCausa) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa))
            {
                GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
                GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

                if (datiStoricoGP != null)
                {
                    if (datiStoricoGP.CodiceTipoPerequazione.HasValue)
                        areaVarie.I_CRIRIL = datiStoricoGP.CodiceTipoPerequazione.Value;

                    areaVarie.I_VINTERA = datiStoricoGP.VirtualePura.HasValue ? datiStoricoGP.VirtualePura.Value : 0M;
                    areaVarie.I_VIRT = datiStoricoGP.VirtualeIntegrata.HasValue ? datiStoricoGP.VirtualeIntegrata.Value : 0M;
                    areaVarie.I_ADEG = datiStoricoGP.Adeguata.HasValue ? datiStoricoGP.Adeguata.Value : 0M;

                    if (datiStoricoGP.DecorrenzaOriginariaPrima.HasValue && datiPensione != null && datiPensione.DecorrenzaOriginariaPrima != null &&
                        datiStoricoGP.DecorrenzaOriginariaPrima.Value != datiPensione.DecorrenzaOriginariaPrima.Value)
                        areaVarie.I_AGGANCIO = "0";
                    else
                        areaVarie.I_AGGANCIO = Convert.ToString(datiIstruttoria.I_AGGANCIO);

                    if (datiIstruttoria.I_AGGANCIO == '1')
                    {
                        if (datiStoricoGP.CodiceTipoPerequazione.GetValueOrDefault() < 1 || datiStoricoGP.VirtualePura.GetValueOrDefault() == 0 || datiStoricoGP.VirtualeIntegrata.GetValueOrDefault() == 0)
                            areaVarie.I_AGGANCIO = "0";
                    }
                }
                if (datiIstruttoria.I_AGGANCIO == '1')
                {
                    if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
                    {
                        int? sett1 = listaPrestazioniEE[0].ContributiEEDecorrenzaOriginaria.HasValue ? listaPrestazioniEE[0].ContributiEEDecorrenzaOriginaria.Value : 0;
                        int? sett2 = 0;
                        int? sett3 = 0;
                        int? sett4 = 0;
                        int? sett5 = 0;
                        int? sett6 = 0;

                        if (listaPrestazioniEE.Count > 1)
                            sett2 = listaPrestazioniEE[1].ContributiEEDecorrenzaOriginaria.HasValue ? listaPrestazioniEE[1].ContributiEEDecorrenzaOriginaria.Value : 0;
                        if (listaPrestazioniEE.Count > 2)
                            sett3 = listaPrestazioniEE[2].ContributiEEDecorrenzaOriginaria.HasValue ? listaPrestazioniEE[2].ContributiEEDecorrenzaOriginaria.Value : 0;
                        if (listaPrestazioniEE.Count > 3)
                            sett4 = listaPrestazioniEE[3].ContributiEEDecorrenzaOriginaria.HasValue ? listaPrestazioniEE[3].ContributiEEDecorrenzaOriginaria.Value : 0;
                        if (listaPrestazioniEE.Count > 4)
                            sett5 = listaPrestazioniEE[4].ContributiEEDecorrenzaOriginaria.HasValue ? listaPrestazioniEE[4].ContributiEEDecorrenzaOriginaria.Value : 0;
                        if (listaPrestazioniEE.Count > 5)
                            sett6 = listaPrestazioniEE[5].ContributiEEDecorrenzaOriginaria.HasValue ? listaPrestazioniEE[5].ContributiEEDecorrenzaOriginaria.Value : 0;

                        int? totaleSettimane = sett1 + sett2 + sett3 + sett4 + sett5 + sett6;

                        if (datiIstruttoria.I_SETTEST == 0 && listaPrestazioniEE[0].CodiceConvenzione == 12 && totaleSettimane < 52)
                            areaVarie.I_SETTEST = (short)totaleSettimane;
                        else
                            areaVarie.I_SETTEST = (short)datiIstruttoria.I_SETTEST;
                    }
                }
            }
            //ENG - Memo 28_2024 0001-0001-0017 con decorrenza > 01.01.2024 e tipo di calcolo "contributivo" GP1TPCLC con secondo byte uguale a 1
            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if (!String.IsNullOrEmpty(datiPensione.Caratterizzazione))
                {
                    areaVarie.GP1TPCLC = datiPensione.Caratterizzazione;
                }
                else if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") &&
                    (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaTipoContributivoCumulo(datiPensione, null, null)) && datiPensione.DecorrenzaOriginaria.HasValue &&
                    Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 01, 01)))
                {
                    areaVarie.GP1TPCLC = " 1      ";
                }
            }

            //ENG - Memo 06_2024
            if (ctrl06_2024 != null && !String.IsNullOrEmpty(ctrl06_2024.ValoreControllo) && ctrl06_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if (datiPensione.CodProPE.HasValue && datiPensione.CodProPE == 8)
                {
                    if (!String.IsNullOrEmpty(areaVarie.GP1TPCLC))
                    {
                        areaVarie.GP1TPCLC = "1" + areaVarie.GP1TPCLC.Substring(1);
                    }
                    else
                    {
                        areaVarie.GP1TPCLC = "1       ";
                    }
                }
            }

            //ENG - Implementazione Meta Processo
            if (ctrl_SbloccaMetaProcesso != null && !String.IsNullOrEmpty(ctrl_SbloccaMetaProcesso.ValoreControllo) && ctrl_SbloccaMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if (Utility.IsRicostituzione(datiPensione.Gruppo) || isRiapertura)
                {
                    if (datiPensione.CodiceSedeLavorazione.HasValue && datiPensione.CodiceSedeLavorazione.Value > 0)
                    {
                        string sedeDomanda = null;
                        areaVarie.IW1_SEDE_LAVO_METAPRO = sedeDomanda = datiPensione.CodiceSedeLavorazione.ToString().PadLeft(4, '0') + (datiPensione.CentroOperativo.HasValue ? datiPensione.CentroOperativo.GetValueOrDefault().ToString().PadLeft(2, '0') : "00");

                        //ENG - Valorizzazione nuovo campo "IW1_DES_SEDE_DOMANDA" per la sede della domanda
                        KeyValuePair<string, DNA.Office> sede = DNA.Context.OfficeList.Offices.FirstOrDefault(x => x.Value.AspnCode == sedeDomanda);
                        if (!sede.Equals(default(KeyValuePair<string, DNA.Office>)))
                            areaVarie.IW1_DES_SEDE_DOMANDA = sede.Value.ExtendedProperties != null ? sede.Value.ExtendedProperties["SEDE"].Trim() : sede.Value.Name.Trim();
                        if (!String.IsNullOrEmpty(areaVarie.IW1_DES_SEDE_DOMANDA) && areaVarie.IW1_DES_SEDE_DOMANDA.Length > 22)
                            areaVarie.IW1_DES_SEDE_DOMANDA = areaVarie.IW1_DES_SEDE_DOMANDA.Substring(0, 22);
                    }

                }

            }

        }
        #endregion Gruppo1

        #region Gruppo2
        private static void ValorizzaGruppo2(GestionePensione.DatiPensione datiPensione, string codiceFiscaleTitolare, ref Data.HostRequest.CI01_CI02RequestNew richiesta)
        {
            Data.PCIINPU7.AreaW3 areaW3 = null;
            ValorizzaAreaW3(datiPensione, out areaW3);
            richiesta.Gruppo2.AreaW3 = areaW3;

            Data.PCIINPU7.AreaW4 areaW4 = null;
            ValorizzaAreaW4(datiPensione, codiceFiscaleTitolare, out areaW4);
            richiesta.Gruppo2.AreaW4 = areaW4;
        }

        private static void ValorizzaAreaW3(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaW3 areaW3)
        {
            areaW3 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW3();

            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaDatiSupplementi = null;
            GestioneSupplementi.GetSupplementiByIdPensione(datiPensione.Id, out listaDatiSupplementi);
            if (listaDatiSupplementi != null && listaDatiSupplementi.Count > 0)
            {
                areaW3.SUPPLEMENTI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW3.Supplemento>();
                foreach (INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi supp in listaDatiSupplementi)
                {
                    Data.PCIINPU7.AreaW3.Supplemento supplemento = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW3.Supplemento();
                    supplemento.IW3DESUPA = supp.DecorrenzaSupplemento.HasValue ? (short)supp.DecorrenzaSupplemento.Value.Year : (short)0;
                    supplemento.IW3DESUPM = supp.DecorrenzaSupplemento.HasValue ? (short)supp.DecorrenzaSupplemento.Value.Month : (short)0;
                    supplemento.IW3COGEST = supp.CodGestioneSupplemento;
                    supplemento.IW3IVS = supp.MontanteSupplemento.HasValue ? supp.MontanteSupplemento.Value : 0M;
                    supplemento.IW3RETSET = supp.RMSSupplemento.HasValue ? Math.Round(supp.RMSSupplemento.Value, 6) : 0M;
                    supplemento.IW3SETANZ = supp.NSettimaneSupplemento.HasValue ? supp.NSettimaneSupplemento.Value : 0;
                    supplemento.IW3IVSSOS = supp.AmmontareContributivo.HasValue ? supp.AmmontareContributivo.Value : 0M;

                    //ENG - Modifica Supplementi CI Memo 177/2012
                    if (supp.QuotaSupplemento.HasValue)
                    {
                        if (supp.QuotaSupplemento == 'A')
                            supplemento.IW3TIPSUP = "0";
                        else if (supp.QuotaSupplemento == 'B')
                            supplemento.IW3TIPSUP = "1";
                        else if (supp.QuotaSupplemento == 'C')
                            supplemento.IW3TIPSUP = "3";
                        else if (supp.QuotaSupplemento == 'D')
                            supplemento.IW3TIPSUP = "4";
                    }

                    areaW3.SUPPLEMENTI.Add(supplemento);
                }
            }
        }

        private static void ValorizzaAreaW4(GestionePensione.DatiPensione datiPensione, string codiceFiscaleTitolare, out Data.PCIINPU7.AreaW4 areaW4)
        {
            areaW4 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW4();

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagrafiche);
            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
            GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaCodMaggFamiliari);

            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                areaW4.CODICIFISCALIFAMILIARI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW4.CodiciFiscaliFamiliari>();
                areaW4.DATIFAMILIARI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW4.DatiFamiliari>();


                if (datiPensione.Gruppo == "0003")
                {
                    var isTitolare = listaFamiliari.FindIndex(x => x.CodiceFiscale == codiceFiscaleTitolare);
                    if (isTitolare > 0)
                    {
                        var elemento = listaFamiliari[isTitolare];
                        listaFamiliari.RemoveAt(isTitolare);
                        listaFamiliari.Insert(0, elemento);
                    }

                }

                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    Data.PCIINPU7.AreaW4.CodiciFiscaliFamiliari codiceFiscale = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW4.CodiciFiscaliFamiliari();
                    Data.PCIINPU7.AreaW4.DatiFamiliari anagrafica = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW4.DatiFamiliari();

                    GestioneAnagrafica.DatiAnagrafici datiAnagFam = listaAnagrafiche.Find(x => x.CodiceFiscale == fam.CodiceFiscale);

                    codiceFiscale.IW4COFI = fam.CodiceFiscale;
                    areaW4.CODICIFISCALIFAMILIARI.Add(codiceFiscale);

                    anagrafica.TP1COGNF = datiAnagFam.Cognome;
                    anagrafica.TP1COACF = datiAnagFam.CognomeAcquisito;
                    anagrafica.TP1NOMEF = datiAnagFam.Nome;
                    anagrafica.IW4SES = datiAnagFam.Sesso.HasValue ? datiAnagFam.Sesso.Value.ToString() : "";
                    anagrafica.IW4NASCA = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Year : (short)0;
                    anagrafica.IW4NASCM = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Month : (short)0;
                    anagrafica.IW4NASCG = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Day : (short)0;
                    anagrafica.TP1PRF = datiAnagFam.ProvinciaNascita.Trim();

                    int codiceInpsComune = 0;
                    GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagFam.CodiceComuneNascita, Utility.TipoAppartenenza.CI.ToString(), 0, false, out codiceInpsComune);
                    anagrafica.TP1COF = codiceInpsComune;
                    anagrafica.LIST_GP3CK = new List<Data.PCIINPU7.AreaW4.GP3CK>();
                    Data.PCIINPU7.AreaW4.GP3CK cM = new Data.PCIINPU7.AreaW4.GP3CK();

                    cM.GP3CH01B = !string.IsNullOrEmpty(fam.TipoUnione) && fam.TipoUnione == "U" ? fam.TipoUnione : null;

                    if (listaCodMaggFamiliari != null)
                    {
                        List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliariParziali =
                            listaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica && x.IdPensione == fam.IdPensione);
                        if (listaCodMaggFamiliariParziali != null && listaCodMaggFamiliariParziali.Count > 0)
                        {
                            for (int i = 0; i < listaCodMaggFamiliariParziali.Count; i++)
                            {
                                if (i != 0)
                                    cM = new Data.PCIINPU7.AreaW4.GP3CK();

                                if (listaCodMaggFamiliariParziali[i].Decorrenza.HasValue || listaCodMaggFamiliariParziali[i].Cessazione.HasValue)
                                {
                                    cM.IW4SIG = listaCodMaggFamiliariParziali[i].SiglaFamiliare.HasValue ? listaCodMaggFamiliariParziali[i].SiglaFamiliare.Value.ToString() : "";
                                    cM.IW4ACQA = listaCodMaggFamiliariParziali[i].Decorrenza.HasValue ? (short)listaCodMaggFamiliariParziali[i].Decorrenza.Value.Year : (short)0;
                                    cM.IW4ACQM = listaCodMaggFamiliariParziali[i].Decorrenza.HasValue ? (short)listaCodMaggFamiliariParziali[i].Decorrenza.Value.Month : (short)0;
                                    cM.IW4CESA = listaCodMaggFamiliariParziali[i].Cessazione.HasValue ? (short)listaCodMaggFamiliariParziali[i].Cessazione.Value.Year : (short)0;
                                    cM.IW4CESM = listaCodMaggFamiliariParziali[i].Cessazione.HasValue ? (short)listaCodMaggFamiliariParziali[i].Cessazione.Value.Month : (short)0;
                                    cM.IW4CMAG = listaCodMaggFamiliariParziali[i].CodiceMaggiorazione.HasValue ? listaCodMaggFamiliariParziali[i].CodiceMaggiorazione.Value : (short)0;
                                    //Eng - se IW4SIG (sigla familiare sul pannello contitolari) = "C" impostare IW4CMAG = 2
                                    if (cM.IW4SIG == "C" && cM.IW4CMAG == 0)
                                        cM.IW4CMAG = (short)2;
                                    anagrafica.LIST_GP3CK.Add(cM);
                                }
                            }
                        }
                    }
                    if (anagrafica.LIST_GP3CK.Count == 0)
                        anagrafica.LIST_GP3CK.Add(cM);

                    areaW4.DATIFAMILIARI.Add(anagrafica);
                }
            }
        }

        private static void ValorizzaAreaWKAUT(out Data.PCIINPU7.AreaWKAUT areaWKAUT)
        {
            areaWKAUT = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaWKAUT();
        }
        #endregion Gruppo2

        #region Gruppo3
        private static void ValorizzaGruppo3(string matricolaOperatore, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa danteCausa,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo,
            GestionePensione.DatiEliminazione datiEliminazione,
            List<GestioneAltrePensioni.AltraPensione> listaAltraPensione,
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, List<GestioneDanteCausa.DatiRedditoSentenza495_93> lDatiSentenza495_93,
            ref Data.HostRequest.CI01_CI02RequestNew richiesta)
        {
            Data.PCIINPU7.AreaW8 areaW8 = null;
            ValorizzaAreaW8(datiPensione, out areaW8);
            richiesta.Gruppo3.AreaW8 = areaW8;

            Data.PCIINPU7.AreaEX_W240 areaEX_W240 = null;
            ValorizzaAreaEX_W240(datiIstruttoria, out areaEX_W240);
            richiesta.Gruppo3.AreaEX_W240 = areaEX_W240;

            Data.PCIINPU7.AreaWK1R areaWK1R = null;
            GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11 = null;
            ValorizzaAreaWK1R(datiPensione, datiGenericiCi, datiMaggiorazioniBenefici, danteCausa, out integrazioneArt11, out areaWK1R);
            richiesta.Gruppo3.AreaWK1R = areaWK1R;

            Data.PCIINPU7.AreaW2CIR areaW2CIR = null;
            ValorizzaAreaW2CIR(datiGenericiCi, datiPensione, danteCausa, out areaW2CIR);
            richiesta.Gruppo3.AreaW2CIR = areaW2CIR;

            Data.PCIINPU7.AreaWK2R areaWK2R = null;
            ValorizzaAreaWK2R(out areaWK2R, datiPensione, datiGenericiCi, danteCausa, datiMaggiorazioniBenefici);
            richiesta.Gruppo3.AreaWK2R = areaWK2R;

            Data.PCIINPU7.AreaAltriCampi areaAltriCampi = null;
            ValorizzaAreaAltriCampi(datiPensione, datiGenericiCi, datiIstruttoria, out areaAltriCampi);
            richiesta.Gruppo3.AreaAltriCampi = areaAltriCampi;

            Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati = null;
            ValorizzaAreaUlterioriDati(datiPensione, danteCausa, datiIstruttoria, datiEliminazione, datiMaggiorazioniBenefici, out listaAltraPensione, out areaUlterioriDati);
            richiesta.Gruppo3.AreaUlterioriDati = areaUlterioriDati;

            Data.PCIINPU7.AreaAssegnoAccompagnamento areaAssegnoAccompagnamento = null;
            ValorizzaAreaAssegnoAccompagnamento(out areaAssegnoAccompagnamento);
            richiesta.Gruppo3.AreaAssegnoAccompagnamento = areaAssegnoAccompagnamento;

            Data.PCIINPU7.AreaAssegnoAltroEnte areaAssegnoAltroEnte = null;
            ValorizzaAreaAssegnoAltroEnte(out areaAssegnoAltroEnte);
            richiesta.Gruppo3.AreaAssegnoAltroEnte = areaAssegnoAltroEnte;

            Data.PCIINPU7.AreaSentenze areaSentenze = null;
            ValorizzaAreaSentenze(datiPensione, listaPrestazioniEE, lDatiSentenza495_93, out areaSentenze);
            richiesta.Gruppo3.AreaSentenze = areaSentenze;

            Data.PCIINPU7.AreaLavEsteroPrePens areaLavEsteroPrePens = null;
            ValorizzaAreaLavEsteroPrePens(out areaLavEsteroPrePens);
            richiesta.Gruppo3.AreaLavEsteroPrePens = areaLavEsteroPrePens;

            Data.PCIINPU7.AreaContributi areaContributi = null;
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo = null;

            ValorizzaAreaContributi(datiPensione, datiGenericiCi, datiIstruttoria, integrazioneArt11, out listaCalcoloRetributivo,
                listaCalcoloContributivo, out areaContributi);
            richiesta.Gruppo3.AreaContributi = areaContributi;

            Data.PCIINPU7.AreaContributi233 areaContributi233 = null;
            ValorizzaAreaContributi233(datiPensione, listaCalcoloRetributivo, out areaContributi233);
            richiesta.Gruppo3.AreaContributi233 = areaContributi233;

            Data.PCIINPU7.AreaSettimaneEst areaSettimaneEst = null;
            ValorizzaAreaSettimaneEst(datiPensione, out areaSettimaneEst);
            richiesta.Gruppo3.AreaSettimaneEst = areaSettimaneEst;

            Data.PCIINPU7.AreaContributi503 areaContributi503 = null;
            ValorizzaAreaContributi503(datiPensione, datiGenericiCi, listaCalcoloRetributivo, out areaContributi503);
            richiesta.Gruppo3.AreaContributi503 = areaContributi503;

            Data.PCIINPU7.AreaContributi335 areaContributi335 = null;
            ValorizzaAreaContributi335(datiGenericiCi, datiMaggiorazioniBenefici, listaCalcoloContributivo, out areaContributi335);
            richiesta.Gruppo3.AreaContributi335 = areaContributi335;

            Data.PCIINPU7.AreaContributiPostDec areaContributiPostDec = null;
            ValorizzaAreaContributiPostDec(datiPensione, out areaContributiPostDec);
            richiesta.Gruppo3.AreaContributiPostDec = areaContributiPostDec;

            Data.PCIINPU7.AreaSpazio areaSpazio = null;
            ValorizzaAreaSpazio(datiPensione, datiGenericiCi, datiIstruttoria, areaTitolare, datiMaggiorazioniBenefici, out areaSpazio);
            richiesta.Gruppo3.AreaSpazio = areaSpazio;

            Data.PCIINPU7.AreaSicurezza areaSicurezza = null;
            ValorizzaAreaSicurezza(matricolaOperatore, out areaSicurezza);
            richiesta.Gruppo3.AreaSicurezza = areaSicurezza;

            Data.PCIINPU7.AreaCodiciStampa areaCodiciStampa = null;
            ValorizzaAreaCodiciStampa(datiGenericiCi, out areaCodiciStampa);
            richiesta.Gruppo3.AreaCodiciStampa = areaCodiciStampa;
        }

        private static void ValorizzaAreaW8(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaW8 areaW8)
        {
            areaW8 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW8();

            List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale> listaReddIntegrazVirtuale = null;
            GestioneDatiContributiviCi.GetRedditiPerIntegrazioneVirtuale(datiPensione.Id, out listaReddIntegrazVirtuale);

            if (listaReddIntegrazVirtuale != null && listaReddIntegrazVirtuale.Count > 0)
            {
                areaW8.REDDITIINTEGRAZIONEVIRTUALE = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW8.RedditiIntegrazioneVirtuale>();

                List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale> lReddIVTitolare = new List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale>();
                List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale> lReddIVConiuge = new List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale>();

                lReddIVTitolare = listaReddIntegrazVirtuale.FindAll(x => x.IsTitolare);
                lReddIVConiuge = listaReddIntegrazVirtuale.FindAll(x => !x.IsTitolare);

                for (int i = 0; i < 5; i++)
                {
                    Data.PCIINPU7.AreaW8.RedditiIntegrazioneVirtuale reddIV = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW8.RedditiIntegrazioneVirtuale();

                    reddIV.IW8DEC = i < lReddIVTitolare.Count() && lReddIVTitolare[i].Anno > 0 ? short.Parse(lReddIVTitolare[i].Anno.ToString()) : (short)0;
                    reddIV.IW8RED = i < lReddIVTitolare.Count() && lReddIVTitolare[i].Reddito.HasValue ? lReddIVTitolare[i].Reddito.Value : 0;
                    areaW8.REDDITIINTEGRAZIONEVIRTUALE.Add(reddIV);
                }

                for (int i = 0; i < 5; i++)
                {
                    Data.PCIINPU7.AreaW8.RedditiIntegrazioneVirtuale reddIV = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW8.RedditiIntegrazioneVirtuale();

                    reddIV.IW8DEC = i < lReddIVConiuge.Count() && lReddIVConiuge[i].Anno > 0 ? short.Parse(lReddIVConiuge[i].Anno.ToString()) : (short)0;
                    reddIV.IW8RED = i < lReddIVConiuge.Count() && lReddIVConiuge[i].Reddito.HasValue ? lReddIVConiuge[i].Reddito.Value : 0;
                    areaW8.REDDITIINTEGRAZIONEVIRTUALE.Add(reddIV);
                }
            }
        }

        private static void ValorizzaAreaEX_W240(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out Data.PCIINPU7.AreaEX_W240 areaEX_W240)
        {
            areaEX_W240 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaEX_W240();
            areaEX_W240.IGP1AJ11 = "0";

            if (datiIstruttoria != null)
            {
                if (datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                {
                    List<GestioneDecodifica.CodiceParticolare> elencoCodiciParticolari = null;
                    GestioneDecodifica.GetCodiciParticolari(out elencoCodiciParticolari);
                    if (elencoCodiciParticolari != null && elencoCodiciParticolari.Count > 0)
                    {
                        GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiciParticolari.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                        if (codiceParticolare != null)
                            areaEX_W240.IGP1AJ11 = codiceParticolare.TraduzioneSuGp.HasValue ? codiceParticolare.TraduzioneSuGp.Value.ToString() : "0";
                    }
                }
            }
        }

        private static void ValorizzaAreaWK1R(GestionePensione.DatiPensione datiPensione, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            out GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11, out Data.PCIINPU7.AreaWK1R areaWK1R)
        {
            areaWK1R = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaWK1R();

            if (datiGenericiCi != null)
            {
                areaWK1R.IW1RMSAR2 = datiGenericiCi.RMS9090.HasValue ? datiGenericiCi.RMS9090.Value : 0M;
                areaWK1R.IW1RMSS72 = datiGenericiCi.RMS8888.HasValue ? datiGenericiCi.RMS8888.Value : 0M;
                areaWK1R.IW1DDPCMA = datiGenericiCi.DecorrenzaArt2Dpcm.HasValue ? (short)datiGenericiCi.DecorrenzaArt2Dpcm.Value.Year : (short)0;
                areaWK1R.IW1DDPCMM = datiGenericiCi.DecorrenzaArt2Dpcm.HasValue ? (short)datiGenericiCi.DecorrenzaArt2Dpcm.Value.Month : (short)0;
            }

            integrazioneArt11 = null;
            GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(datiPensione.Id, out integrazioneArt11);
            if (integrazioneArt11 != null)
            {
                areaWK1R.IW1A11S72 = integrazioneArt11.ImportoIVS.HasValue ? integrazioneArt11.ImportoIVS.Value : 0M;
            }

            if (datiMaggiorazioniBenefici != null)
            {
                areaWK1R.IW1ADPCM = datiMaggiorazioniBenefici.AumentoMensileLegge161289Art2.HasValue ? datiMaggiorazioniBenefici.AumentoMensileLegge161289Art2.Value : 0M;
                areaWK1R.IW1AS72A = datiMaggiorazioniBenefici.Aumento7290.HasValue ? datiMaggiorazioniBenefici.Aumento7290.Value : 0M;
                if (datiMaggiorazioniBenefici.ImportoComplessivoArt3.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt3.Value != 0M)
                    areaWK1R.IW1TM345 = datiMaggiorazioniBenefici.ImportoComplessivoArt3.Value;
                else if (datiMaggiorazioniBenefici.ImportoComplessivoArt4.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt4.Value != 0M)
                    areaWK1R.IW1TM345 = datiMaggiorazioniBenefici.ImportoComplessivoArt4.Value;
                else if (datiMaggiorazioniBenefici.ImportoComplessivoArt5.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt5.Value != 0M)
                    areaWK1R.IW1TM345 = datiMaggiorazioniBenefici.ImportoComplessivoArt5.Value;
                else if (datiMaggiorazioniBenefici.ImportoComplessivoArt1.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt1.Value != 0M)
                    areaWK1R.IW1TM345 = datiMaggiorazioniBenefici.ImportoComplessivoArt1.Value;
                areaWK1R.IW1TM409 = datiMaggiorazioniBenefici.AumentoMensileLegge5991Comma2.HasValue ? datiMaggiorazioniBenefici.AumentoMensileLegge5991Comma2.Value : 0M;

                //ENG - Implementata la gestione mancante per le Reversibilità
                if (Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                {
                    if (datiMaggiorazioniBenefici.Articolo1Legge5991.HasValue)
                        areaWK1R.IW1CM409 = (short)(datiMaggiorazioniBenefici.Articolo1Legge5991.Value ? 1 : 0);
                }
            }

            if (datiDanteCausa != null)
            {
                areaWK1R.IW1DART5 = datiDanteCausa.EccedenzaArt5.HasValue ? datiDanteCausa.EccedenzaArt5.Value : 0M;
            }

            //ENG - Implementata la gestione mancante per le Reversibilità
            if (Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
            {
                List<GestioneDanteCausa.PensioniEstereDcBL> LpensioniEstereDcBL = null;
                GestioneDanteCausa.GetPensioniEstereDCByIdPensione(datiPensione.Id, out LpensioniEstereDcBL);
                if (LpensioniEstereDcBL != null && LpensioniEstereDcBL.Count() > 0)
                {
                    if (LpensioniEstereDcBL.LastOrDefault().CodiciVari.HasValue)
                        areaWK1R.IW1CM345 = (short)LpensioniEstereDcBL.LastOrDefault().CodiciVari;
                }
            }
        }

        private static void ValorizzaAreaW2CIR(GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out Data.PCIINPU7.AreaW2CIR areaW2CIR)
        {
            areaW2CIR = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaW2CIR();

            if (datiGenericiCi != null)
            {
                areaW2CIR.ICI2IMPCRIS34 = datiGenericiCi.ImportoCristallizzazione3481.HasValue ? datiGenericiCi.ImportoCristallizzazione3481.Value : 0M;
            }

            //ENG - Implementata la gestione mancante per le Reversibilità
            if (Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
            {
                areaW2CIR.ICI2DAPLIQA = datiPensione.DecorrenzaOriginariaPrima.HasValue ? (short)datiPensione.DecorrenzaOriginariaPrima.Value.Year : (short)0;
                areaW2CIR.ICI2DAPLIQM = datiPensione.DecorrenzaOriginariaPrima.HasValue ? (short)datiPensione.DecorrenzaOriginariaPrima.Value.Month : (short)0;

                if (datiDanteCausa != null)
                {
                    areaW2CIR.ICI2VINTERA = datiDanteCausa.VirtualePura.HasValue ? datiDanteCausa.VirtualePura.Value : 0M;
                    areaW2CIR.ICI2VIRT = datiDanteCausa.VirtualeIntegrata.HasValue ? datiDanteCausa.VirtualeIntegrata.Value : 0M;
                }

                List<GestioneDanteCausa.PensioniEstereDcBL> LpensioniEstereDcBL = null;
                GestioneDanteCausa.GetPensioniEstereDCByIdPensione(datiPensione.Id, out LpensioniEstereDcBL);
                if (LpensioniEstereDcBL != null && LpensioniEstereDcBL.Count() > 0)
                {
                    if (LpensioniEstereDcBL.FirstOrDefault().Importo.HasValue)
                        areaW2CIR.ICI2SUP = Convert.ToDecimal(LpensioniEstereDcBL.FirstOrDefault().Importo);
                }
            }
        }

        //test
        private static void ValorizzaAreaWK2R(out Data.PCIINPU7.AreaWK2R areaWK2R, GestionePensione.DatiPensione datiPensione, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi, GestioneDanteCausa.DatiDanteCausa danteCausa,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            areaWK2R = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaWK2R();

            //ENG - Implementata la gestione mancante per le Reversibilità
            //Aggiunta gestione ImportoPagamentoDataMorte49593
            if (Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, danteCausa) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa))
            {
                if (danteCausa != null)
                {
                    areaWK2R.IABTQFI = danteCausa.TotaleQuoteFisse.HasValue ? danteCausa.TotaleQuoteFisse.Value : 0M;
                    areaWK2R.IABMCP = danteCausa.Adeguata.HasValue ? danteCausa.Adeguata.Value : 0M;
                    areaWK2R.IABMM409 = datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.MensileLegge5991.HasValue ? datiMaggiorazioniBenefici.MensileLegge5991.Value : 0M;
                    areaWK2R.IABML1Q = danteCausa.ImportoPagamentoDataMorte49593.HasValue ? danteCausa.ImportoPagamentoDataMorte49593.Value : 0M;
                }

                List<GestioneDanteCausa.PensioniEstereDcBL> LpensioniEstereDcBL = null;
                GestioneDanteCausa.GetPensioniEstereDCByIdPensione(datiPensione.Id, out LpensioniEstereDcBL);
                if (LpensioniEstereDcBL != null && LpensioniEstereDcBL.Count() > 0)
                {

                    if (LpensioniEstereDcBL.Count > 1 && LpensioniEstereDcBL[1].Importo.HasValue)
                        areaWK2R.IABMMEX6 = Convert.ToDecimal(LpensioniEstereDcBL[1].Importo);

                    //metto il >= perchè non si sa mai, ma dovrebbero essere sempre esattamente 3
                    if (LpensioniEstereDcBL.Count >= 3 && LpensioniEstereDcBL[2].Importo.HasValue)
                        areaWK2R.IABMM345 = Convert.ToDecimal(LpensioniEstereDcBL[2].Importo);

                }
            }
        }

        private static void ValorizzaAreaAltriCampi(GestionePensione.DatiPensione datiPensione, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            out Data.PCIINPU7.AreaAltriCampi areaAltriCampi)
        {
            areaAltriCampi = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaAltriCampi();

            areaAltriCampi.INIASSA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
            areaAltriCampi.INIASSM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
            areaAltriCampi.INIASSG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
            areaAltriCampi.FINASSA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
            areaAltriCampi.FINASSM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
            areaAltriCampi.FINASSG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

            if (datiGenericiCi != null)
            {
                areaAltriCampi.IDECNAT3A = datiGenericiCi.DecorrenzaCodiceVirtuale.HasValue ? (short)datiGenericiCi.DecorrenzaCodiceVirtuale.Value.Year : (short)0;
                areaAltriCampi.IDECNAT3M = datiGenericiCi.DecorrenzaCodiceVirtuale.HasValue ? (short)datiGenericiCi.DecorrenzaCodiceVirtuale.Value.Month : (short)0;
                areaAltriCampi.TRESTERO = (datiGenericiCi.CodiceBloccoArretratiEE.HasValue && datiGenericiCi.CodiceBloccoArretratiEE.Value) ? 1 : 0;
                areaAltriCampi.TRESTEROUP = Utility.GetUfficioPagatoreFromId(datiGenericiCi.UfficioPagatoreArretratiEE);
                //areaAltriCampi.TRESTEROUP = datiGenericiCi.UfficioPagatoreArretratiEE;
            }

            if (datiIstruttoria != null)
            {
                areaAltriCampi.NRICONOSC = datiIstruttoria.NRiconoscimentiInvalidita.HasValue ? (short)datiIstruttoria.NRiconoscimentiInvalidita.Value : (short)0;
            }
        }

        private static void ValorizzaAreaUlterioriDati(GestionePensione.DatiPensione datiPensione,
            GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestionePensione.DatiEliminazione datiEliminazione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            out List<GestioneAltrePensioni.AltraPensione> listaAltraPensione,
            out Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati)
        {
            listaAltraPensione = null;
            areaUlterioriDati = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaUlterioriDati();

            areaUlterioriDati.IREQ311294 = datiPensione.RequisitiAl1294.HasValue ? datiPensione.RequisitiAl1294.Value ? "2" : "1" : "0";

            if (danteCausa != null)
            {
                List<GestioneDecodifica.ParentelaDC> listaParentelaDC = null;
                GestioneDecodifica.GetParentelaDC(out listaParentelaDC);
                if (listaParentelaDC != null && listaParentelaDC.Count > 0)
                {
                    if (danteCausa.ParentelaDC.HasValue)
                    {
                        GestioneDecodifica.ParentelaDC parentelaDC = listaParentelaDC.Find(x => x.Id == danteCausa.ParentelaDC.Value.ToString());
                        if (parentelaDC != null)
                        {
                            if (parentelaDC.Descrizione == "Unito civilmente")
                                parentelaDC.Descrizione = "Unito";
                        }
                        areaUlterioriDati.IRELPAR = parentelaDC.Descrizione;
                    }
                }
            }

            if (datiIstruttoria != null)
            {
                areaUlterioriDati.PRECSEDE = datiIstruttoria.SedePrecedentePensione.HasValue ? datiIstruttoria.SedePrecedentePensione.Value.ToString().PadLeft(4, '0') : "0000";
                areaUlterioriDati.PRECCAT = datiIstruttoria.CodiceP18PrecedentePensione.HasValue ? datiIstruttoria.CodiceP18PrecedentePensione.Value.ToString().PadLeft(3, '0') : "000";
                areaUlterioriDati.PRECCER = datiIstruttoria.CertificatoPrecedentePensione.HasValue ? datiIstruttoria.CertificatoPrecedentePensione.Value.ToString().PadLeft(8, '0') : "00000000";

            }

            if (datiEliminazione != null)
            {
                areaUlterioriDati.DECELIMA = datiEliminazione.DataEvento.HasValue ? (short)datiEliminazione.DataEvento.Value.Year : (short)0;
                areaUlterioriDati.DECELIMM = datiEliminazione.DataEvento.HasValue ? (short)datiEliminazione.DataEvento.Value.Month : (short)0;
                areaUlterioriDati.DECELIMG = datiEliminazione.DataEvento.HasValue ? (short)datiEliminazione.DataEvento.Value.Day : (short)0;
            }

            GestioneAltrePensioni.GetAltraPensioneByIdPensione(datiPensione.Id, out listaAltraPensione);
            if (listaAltraPensione != null && listaAltraPensione.Count > 0)
            {
                areaUlterioriDati.ALTRAPENSIONE = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaUlterioriDati.AltraPensione>();
                for (int i = 0; i < listaAltraPensione.Count; i++)
                {
                    INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaUlterioriDati.AltraPensione altraPensione = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaUlterioriDati.AltraPensione();
                    short codImporto = 0;

                    altraPensione.IAPCATEG = !string.IsNullOrEmpty(listaAltraPensione[i].Categoria) ? listaAltraPensione[i].Categoria.PadLeft(3, ' ') : string.Empty;
                    altraPensione.IAPNUMP = listaAltraPensione[i].Certificato.HasValue ? listaAltraPensione[i].Certificato.Value : 0;
                    altraPensione.IAPENTE = listaAltraPensione[i].Ente.HasValue ? listaAltraPensione[i].Ente.ToString() : string.Empty;
                    altraPensione.IAPDECORA = listaAltraPensione[i].Decorrenza.HasValue ? (short)listaAltraPensione[i].Decorrenza.Value.Year : (short)0;
                    altraPensione.IAPDECORM = listaAltraPensione[i].Decorrenza.HasValue ? (short)listaAltraPensione[i].Decorrenza.Value.Month : (short)0;
                    altraPensione.IAPUNIC = listaAltraPensione[i].CodiceUC.HasValue ? listaAltraPensione[i].CodiceUC.ToString() : string.Empty;
                    short.TryParse(listaAltraPensione[i].CodiceImporto.HasValue ? listaAltraPensione[i].CodiceImporto.ToString() : string.Empty, out codImporto);
                    altraPensione.IAPCODIMP = codImporto;
                    altraPensione.IAPCESSAA = listaAltraPensione[i].Cessazione.HasValue ? (short)listaAltraPensione[i].Cessazione.Value.Year : (short)0;
                    altraPensione.IAPCESSAM = listaAltraPensione[i].Cessazione.HasValue ? (short)listaAltraPensione[i].Cessazione.Value.Month : (short)0;

                    areaUlterioriDati.ALTRAPENSIONE.Add(altraPensione);
                }
            }

            //ENG - Implementata la gestione mancante per le Reversibilità
            if (Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, danteCausa) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa))
            {
                if (datiMaggiorazioniBenefici != null)
                    areaUlterioriDati.IW1AS72B = datiMaggiorazioniBenefici.Aumento7290DC.HasValue ? datiMaggiorazioniBenefici.Aumento7290DC.Value : 0M;
            }
        }

        private static void ValorizzaAreaAssegnoAccompagnamento(out Data.PCIINPU7.AreaAssegnoAccompagnamento areaAssegnoAccompagnamento)
        {
            areaAssegnoAccompagnamento = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaAssegnoAccompagnamento();
        }

        private static void ValorizzaAreaAssegnoAltroEnte(out Data.PCIINPU7.AreaAssegnoAltroEnte areaAssegnoAltroEnte)
        {
            areaAssegnoAltroEnte = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaAssegnoAltroEnte();
        }

        private static void ValorizzaAreaSentenze(GestionePensione.DatiPensione datiPensione, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, List<GestioneDanteCausa.DatiRedditoSentenza495_93> lDatiSentenza495_93, out Data.PCIINPU7.AreaSentenze areaSentenze)
        {
            areaSentenze = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSentenze();
            areaSentenze.SENTENZE = new List<Data.PCIINPU7.AreaSentenze.Sentenza>();
            Data.PCIINPU7.AreaSentenze.Sentenza sentenza = new Data.PCIINPU7.AreaSentenze.Sentenza();

            byte? codiceConvenzione = null;
            string codicePrimoStato = string.Empty;
            int codicePrimoStatoEE = 0;
            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
            {
                codiceConvenzione = listaPrestazioniEE[0].CodiceConvenzione;
                codicePrimoStato = listaPrestazioniEE[0].CodiceStatoEE;

                int.TryParse(codicePrimoStato, out codicePrimoStatoEE);
            }
            //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
            if ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
            {
                if (lDatiSentenza495_93 != null && lDatiSentenza495_93.Count() > 0)
                {
                    foreach (GestioneDanteCausa.DatiRedditoSentenza495_93 reddSent495 in lDatiSentenza495_93.FindAll(x => x.FlagSentenza != null))
                    {
                        sentenza.ICISEN2 = reddSent495.CodiceSentenza.HasValue ? reddSent495.CodiceSentenza.Value : (short)0;
                        sentenza.ICISEN3A = reddSent495.AnnoSentenza.HasValue ? reddSent495.AnnoSentenza.Value : (short)0;
                        sentenza.ICISEN3M = reddSent495.MeseSentenza.HasValue ? reddSent495.MeseSentenza.Value : (short)0;

                        areaSentenze.SENTENZE.Add(sentenza);
                    }
                }
            }
        }

        private static void ValorizzaAreaLavEsteroPrePens(out Data.PCIINPU7.AreaLavEsteroPrePens areaLavEsteroPrePens)
        {
            areaLavEsteroPrePens = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaLavEsteroPrePens();
        }

        private static void ValorizzaAreaContributi(GestionePensione.DatiPensione datiPensione, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11,
            out List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo,
            out Data.PCIINPU7.AreaContributi areaContributi)
        {
            areaContributi = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributi();

            listaCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaCalcoloRetributivo);

            if (datiGenericiCi != null)
            {
                areaContributi.IW1VVMISURA = datiGenericiCi.VVMisuraAl1292.HasValue ? datiGenericiCi.VVMisuraAl1292.Value : 0;
                areaContributi.I1SETIVS = datiGenericiCi.SettimanePerCalcoloContributivo.HasValue ? (short)datiGenericiCi.SettimanePerCalcoloContributivo.Value : (short)0;
                areaContributi.IW1IVSTOT = datiGenericiCi.ImportoIVS.HasValue ? datiGenericiCi.ImportoIVS.Value : 0M;
                areaContributi.IW1FFAA = datiGenericiCi.NContributiItalia.HasValue ? datiGenericiCi.NContributiItalia.Value : 0;
                areaContributi.ICI2SETFIT = datiGenericiCi.NSettFittiziePrepensionamento.HasValue ? (short)datiGenericiCi.NSettFittiziePrepensionamento.Value : (short)0;
                areaContributi.IW1NSAUT = datiGenericiCi.SettimaneItalianeMisura.HasValue ? datiGenericiCi.SettimaneItalianeMisura.Value :
                    GestioneLiquidazionePensione.GetNumeroSettimaneItalianeMisura(listaCalcoloContributivo, listaCalcoloRetributivo);
                areaContributi.TP1NUA = datiGenericiCi.SettimaneItalianeDiritto.HasValue ? datiGenericiCi.SettimaneItalianeDiritto.Value : 0;
                areaContributi.TP1DIFN = datiGenericiCi.AnniDifferimento.HasValue ? (short)datiGenericiCi.AnniDifferimento.Value : (short)0;
            }

            if (datiIstruttoria != null)
            {
                if (areaContributi.TP1NUA == 0)
                    areaContributi.TP1NUA = datiIstruttoria.NSettimaneOBG.HasValue ? datiIstruttoria.NSettimaneOBG.Value : 0;
                if (areaContributi.TP1NUA == 0)
                    areaContributi.TP1NUA = datiIstruttoria.NContributiUtiliLavoratoriAutonomi.HasValue ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi.Value : 0;
                areaContributi.TP1NUB = datiIstruttoria.NContributiVolontari.HasValue ? datiIstruttoria.NContributiVolontari.Value : 0;
                areaContributi.IABNSASS = datiIstruttoria.NSettGodimentoAssegno.HasValue ? datiIstruttoria.NSettGodimentoAssegno.Value : 0;
            }

            if (integrazioneArt11 != null)
            {
                areaContributi.IABAR11VV = integrazioneArt11.ImportoIVS.HasValue ? integrazioneArt11.ImportoIVS.Value : 0M;
            }

            if (listaCalcoloRetributivo != null && listaCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo calcoloRetributivo in listaCalcoloRetributivo)
                {
                    if (calcoloRetributivo.CodiceGestione.HasValue)
                    {
                        switch (calcoloRetributivo.CodiceGestione.Value)
                        {
                            case 1: //AGO - OBG 
                                string codCategoria = datiPensione.GetCodCategoria();
                                if (codCategoria.Trim() == "0004" || codCategoria.Trim() == "0005" || codCategoria.Trim() == "0006")
                                {
                                    if (areaContributi.IABREMSVV == 0M)
                                        areaContributi.IABREMSVV = calcoloRetributivo.RMSQuotaA.HasValue ? calcoloRetributivo.RMSQuotaA.Value : 0M;
                                    if (areaContributi.IW1NSOBG == 0)
                                        areaContributi.IW1NSOBG = calcoloRetributivo.NSettimaneQuotaA.HasValue ? calcoloRetributivo.NSettimaneQuotaA.Value : 0;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void ValorizzaAreaContributi233(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo,
            out Data.PCIINPU7.AreaContributi233 areaContributi233)
        {
            areaContributi233 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributi233();

            if (listaCalcoloRetributivo != null && listaCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo calcoloRetributivo in listaCalcoloRetributivo)
                {
                    if (calcoloRetributivo.CodiceGestione.HasValue)
                    {
                        switch (calcoloRetributivo.CodiceGestione.Value)
                        {
                            case 1: //AGO - OBG 
                                string codCategoria = datiPensione.GetCodCategoria();
                                if (codCategoria.Trim() != "0004" && codCategoria.Trim() != "0005" && codCategoria.Trim() != "0006")
                                {
                                    if (areaContributi233.IW1RMSOBG == 0M)
                                        areaContributi233.IW1RMSOBG = calcoloRetributivo.RMSQuotaA.HasValue ? calcoloRetributivo.RMSQuotaA.Value : 0M;
                                    if (areaContributi233.IW1SAOBG == 0)
                                        areaContributi233.IW1SAOBG = calcoloRetributivo.NSettimaneQuotaA.HasValue ? calcoloRetributivo.NSettimaneQuotaA.Value : 0;
                                }
                                break;
                            case 2: //CDCM
                                if (areaContributi233.IW1RMSCDM == 0M)
                                    areaContributi233.IW1RMSCDM = calcoloRetributivo.RMSQuotaA.HasValue ? calcoloRetributivo.RMSQuotaA.Value : 0M;
                                if (areaContributi233.IW1SACDM == 0)
                                    areaContributi233.IW1SACDM = calcoloRetributivo.NSettimaneQuotaA.HasValue ? calcoloRetributivo.NSettimaneQuotaA.Value : 0;
                                break;
                            case 3: //ART
                                if (areaContributi233.IW1RMSART == 0M)
                                    areaContributi233.IW1RMSART = calcoloRetributivo.RMSQuotaA.HasValue ? calcoloRetributivo.RMSQuotaA.Value : 0M;
                                if (areaContributi233.IW1SAART == 0)
                                    areaContributi233.IW1SAART = calcoloRetributivo.NSettimaneQuotaA.HasValue ? calcoloRetributivo.NSettimaneQuotaA.Value : 0;
                                break;
                            case 4: //COM
                                if (areaContributi233.IW1RMSCOM == 0M)
                                    areaContributi233.IW1RMSCOM = calcoloRetributivo.RMSQuotaA.HasValue ? calcoloRetributivo.RMSQuotaA.Value : 0M;
                                if (areaContributi233.IW1SACOM == 0)
                                    areaContributi233.IW1SACOM = calcoloRetributivo.NSettimaneQuotaA.HasValue ? calcoloRetributivo.NSettimaneQuotaA.Value : 0;
                                break;
                        }
                    }
                }
            }
        }

        private static void ValorizzaAreaSettimaneEst(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaSettimaneEst areaSettimaneEst)
        {
            areaSettimaneEst = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSettimaneEst();

            List<GestioneCalcolo.DatiCalcoloContributivoEstero> listaCalcoloContrEstero = null;
            GestioneCalcolo.GetCalcoloContributivoEsteroCIbyIdPensione(datiPensione.Id, out listaCalcoloContrEstero);
            if (listaCalcoloContrEstero != null && listaCalcoloContrEstero.Count > 0)
            {
                areaSettimaneEst.SETTIMANEESTERE = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSettimaneEst.SettimaneEstere>();
                foreach (GestioneCalcolo.DatiCalcoloContributivoEstero calcoloContrEstero in listaCalcoloContrEstero)
                {
                    Data.PCIINPU7.AreaSettimaneEst.SettimaneEstere settEstera = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSettimaneEst.SettimaneEstere();
                    settEstera.DEC233A = calcoloContrEstero.Decorrenza.HasValue ? (short)calcoloContrEstero.Decorrenza.Value.Year : (short)0;
                    settEstera.DEC233M = calcoloContrEstero.Decorrenza.HasValue ? (short)calcoloContrEstero.Decorrenza.Value.Month : (short)0;
                    if (calcoloContrEstero.CodiceGestione.HasValue)
                    {
                        List<GestioneDecodifica.CodeGestione> listaCodiciGestione = null;
                        GestioneDecodifica.GetCodiceGestione(out listaCodiciGestione);
                        if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                        {
                            GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == calcoloContrEstero.CodiceGestione.Value);
                            if (codeGestione != null)
                                settEstera.GEST233 = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                        }
                    }
                    settEstera.SETRI233 = calcoloContrEstero.Settimane.HasValue ? calcoloContrEstero.Settimane.Value : 0;
                    areaSettimaneEst.SETTIMANEESTERE.Add(settEstera);
                }
            }
        }

        private static void ValorizzaAreaContributi503(GestionePensione.DatiPensione datiPensione, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo, out Data.PCIINPU7.AreaContributi503 areaContributi503)
        {
            areaContributi503 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributi503();

            if (datiGenericiCi != null)
            {
                //ENG - In fase di invio al calcolo per le domande della linea CI ad esclusione delle categorie VOS, IOS, SOS si deve passare il campo ICI1VVOBG = 0
                string codCategoria = datiPensione.GetCodCategoria();
                if (codCategoria.Trim() == "0004" || codCategoria.Trim() == "0005" || codCategoria.Trim() == "0006")
                    areaContributi503.ICI1VVOBG = datiGenericiCi.VVMisuraDL50392.HasValue ? datiGenericiCi.VVMisuraDL50392.Value : 0;
                else
                    areaContributi503.ICI1VVOBG = 0;
            }

            if (listaCalcoloRetributivo != null && listaCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo calcoloRetributivo in listaCalcoloRetributivo)
                {
                    if (calcoloRetributivo.CodiceGestione.HasValue)
                    {
                        switch (calcoloRetributivo.CodiceGestione.Value)
                        {
                            case 1: //AGO - OBG
                                if (areaContributi503.IW1RETOBG == 0M)
                                    areaContributi503.IW1RETOBG = calcoloRetributivo.RMSQuotaB.HasValue ? calcoloRetributivo.RMSQuotaB.Value : 0M;
                                if (areaContributi503.IW1STOBG == 0)
                                    areaContributi503.IW1STOBG = calcoloRetributivo.NSettimaneQuotaB.HasValue ? calcoloRetributivo.NSettimaneQuotaB.Value : 0;
                                break;
                            case 2: //CDCM
                                if (areaContributi503.IW1RETCDM == 0M)
                                    areaContributi503.IW1RETCDM = calcoloRetributivo.RMSQuotaB.HasValue ? calcoloRetributivo.RMSQuotaB.Value : 0M;
                                if (areaContributi503.IW1STCDM == 0)
                                    areaContributi503.IW1STCDM = calcoloRetributivo.NSettimaneQuotaB.HasValue ? calcoloRetributivo.NSettimaneQuotaB.Value : 0;
                                break;
                            case 3: //ART
                                if (areaContributi503.IW1RETART == 0M)
                                    areaContributi503.IW1RETART = calcoloRetributivo.RMSQuotaB.HasValue ? calcoloRetributivo.RMSQuotaB.Value : 0M;
                                if (areaContributi503.IW1START == 0)
                                    areaContributi503.IW1START = calcoloRetributivo.NSettimaneQuotaB.HasValue ? calcoloRetributivo.NSettimaneQuotaB.Value : 0;
                                break;
                            case 4: //COM
                                if (areaContributi503.IW1RETCOM == 0M)
                                    areaContributi503.IW1RETCOM = calcoloRetributivo.RMSQuotaB.HasValue ? calcoloRetributivo.RMSQuotaB.Value : 0M;
                                if (areaContributi503.IW1STCOM == 0)
                                    areaContributi503.IW1STCOM = calcoloRetributivo.NSettimaneQuotaB.HasValue ? calcoloRetributivo.NSettimaneQuotaB.Value : 0;
                                break;
                        }
                    }
                }
            }
        }

        private static void ValorizzaAreaContributi335(GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo, out Data.PCIINPU7.AreaContributi335 areaContributi335)
        {
            areaContributi335 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributi335();


            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo calcoloContributivo in listaDatiCalcoloContributivo)
                {
                    if (calcoloContributivo.CodiceGestione.HasValue &&
                        (calcoloContributivo.NSettimane.HasValue || calcoloContributivo.ImportoContributivoTotale.HasValue || calcoloContributivo.Montante.HasValue))
                    {
                        switch (calcoloContributivo.CodiceGestione.Value)
                        {
                            case 1: //AGO - OBG
                                areaContributi335.ICISTOBG335 = calcoloContributivo.NSettimane.HasValue ? calcoloContributivo.NSettimane.Value : 0;
                                areaContributi335.ICICONOBG335 = calcoloContributivo.ImportoContributivoTotale.HasValue ? calcoloContributivo.ImportoContributivoTotale.Value : 0M;
                                areaContributi335.ICIRETOBG335 = calcoloContributivo.Montante.HasValue ? calcoloContributivo.Montante.Value : 0M;
                                break;
                            case 2://CDCM
                                areaContributi335.ICISTCDM335 = calcoloContributivo.NSettimane.HasValue ? calcoloContributivo.NSettimane.Value : 0;
                                areaContributi335.ICICONCDM335 = calcoloContributivo.ImportoContributivoTotale.HasValue ? calcoloContributivo.ImportoContributivoTotale.Value : 0M;
                                areaContributi335.ICIRETCDM335 = calcoloContributivo.Montante.HasValue ? calcoloContributivo.Montante.Value : 0M;
                                break;
                            case 3://ART
                                areaContributi335.ICISTART335 = calcoloContributivo.NSettimane.HasValue ? calcoloContributivo.NSettimane.Value : 0;
                                areaContributi335.ICICONART335 = calcoloContributivo.ImportoContributivoTotale.HasValue ? calcoloContributivo.ImportoContributivoTotale.Value : 0M;
                                areaContributi335.ICIRETART335 = calcoloContributivo.Montante.HasValue ? calcoloContributivo.Montante.Value : 0M;
                                break;
                            case 4://COM
                                areaContributi335.ICISTCOM335 = calcoloContributivo.NSettimane.HasValue ? calcoloContributivo.NSettimane.Value : 0;
                                areaContributi335.ICICONCOM335 = calcoloContributivo.ImportoContributivoTotale.HasValue ? calcoloContributivo.ImportoContributivoTotale.Value : 0M;
                                areaContributi335.ICIRETCOM335 = calcoloContributivo.Montante.HasValue ? calcoloContributivo.Montante.Value : 0M;
                                break;
                        }
                    }
                }
            }

            if (datiGenericiCi != null)
            {
                areaContributi335.ICIMMF = datiGenericiCi.CMSM.HasValue ? datiGenericiCi.CMSM.Value : 0M;
            }

            if (datiMaggiorazioniBenefici != null)
            {
                areaContributi335.ICISET1X100 = datiMaggiorazioniBenefici.NSettimaneIncremento1Percento.HasValue ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento.Value : 0;
                areaContributi335.ICISET05X100 = datiMaggiorazioniBenefici.NSettimaneIncremento05Percento.HasValue ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento.Value : 0;
            }
        }

        private static void ValorizzaAreaContributiPostDec(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaContributiPostDec areaContributiPostDec)
        {
            areaContributiPostDec = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributiPostDec();

            List<GestioneDatiContributiviCi.DatiPostDecOriginaria> listaDatiPostDecOriginaria = null;
            GestioneDatiContributiviCi.GetDatiPostDecOriginariaByIdPensione(datiPensione.Id, out listaDatiPostDecOriginaria);

            if (listaDatiPostDecOriginaria != null && listaDatiPostDecOriginaria.Count > 0)
            {
                areaContributiPostDec.CONTRIBUTI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributiPostDec.Contributo>();
                foreach (GestioneDatiContributiviCi.DatiPostDecOriginaria datiPostDecOriginaria in listaDatiPostDecOriginaria)
                {
                    INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributiPostDec.Contributo contributo = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaContributiPostDec.Contributo();
                    contributo.IDECRICA = datiPostDecOriginaria.Decorrenza.HasValue ? (short)datiPostDecOriginaria.Decorrenza.Value.Year : (short)0;
                    contributo.IDECRICM = datiPostDecOriginaria.Decorrenza.HasValue ? (short)datiPostDecOriginaria.Decorrenza.Value.Month : (short)0;
                    contributo.INSIVSRIC = datiPostDecOriginaria.CTR.HasValue ? (short)datiPostDecOriginaria.CTR.Value : (short)0;
                    contributo.IIVSRIC = datiPostDecOriginaria.IVS.HasValue ? datiPostDecOriginaria.IVS.Value : (decimal)0;
                    contributo.INSOBGRIC = datiPostDecOriginaria.SettimaneRetributive.HasValue ? (short)datiPostDecOriginaria.SettimaneRetributive.Value : (short)0;
                    contributo.INSVVRIC = datiPostDecOriginaria.SettimaneVV.HasValue ? (short)datiPostDecOriginaria.SettimaneVV.Value : (short)0;
                    contributo.IRMSRIC = datiPostDecOriginaria.RMS.HasValue ? datiPostDecOriginaria.RMS.Value : (decimal)0;

                    areaContributiPostDec.CONTRIBUTI.Add(contributo);
                }
            }
        }

        private static void ValorizzaAreaSpazio(GestionePensione.DatiPensione datiPensione, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            out Data.PCIINPU7.AreaSpazio areaSpazio)
        {
            areaSpazio = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSpazio();
            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagrafiche);

            areaSpazio.TP1COMPA = datiPensione.DataCompletezza.HasValue ? (short)datiPensione.DataCompletezza.Value.Year : (short)0;
            areaSpazio.TP1COMPM = datiPensione.DataCompletezza.HasValue ? (short)datiPensione.DataCompletezza.Value.Month : (short)0;
            areaSpazio.TP1COMPG = datiPensione.DataCompletezza.HasValue ? (short)datiPensione.DataCompletezza.Value.Day : (short)0;

            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> listaMaternitaAcna = null;
            GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(datiPensione.Id, out listaMaternitaAcna);
            if (listaMaternitaAcna != null && listaMaternitaAcna.Count > 0)
            {
                foreach (GestioneDatiContributiviCi.PensioniCiMaternitaAcna maternitAcna in listaMaternitaAcna)
                {
                    if (maternitAcna.Tipo.HasValue)
                    {
                        switch (maternitAcna.Tipo.Value)
                        {
                            case 'M':
                                areaSpazio.ISETMAT1 = maternitAcna.SettimaneAl1292.HasValue ? (short)maternitAcna.SettimaneAl1292.Value : (short)0;
                                areaSpazio.ISETMAT2 = maternitAcna.SettimaneDL50392.HasValue ? (short)maternitAcna.SettimaneDL50392.Value : (short)0;
                                areaSpazio.IIVSMAT1 = maternitAcna.ImportoIVS.HasValue ? maternitAcna.ImportoIVS.Value : 0M;
                                break;
                            case 'A':
                                areaSpazio.ISETCEN1 = maternitAcna.SettimaneAl1292.HasValue ? (short)maternitAcna.SettimaneAl1292.Value : (short)0;
                                areaSpazio.ISETCEN2 = maternitAcna.SettimaneDL50392.HasValue ? (short)maternitAcna.SettimaneDL50392.Value : (short)0;
                                areaSpazio.IIVSCEN1 = maternitAcna.ImportoIVS.HasValue ? maternitAcna.ImportoIVS.Value : 0M;
                                break;
                        }
                    }
                }
            }

            if (datiGenericiCi != null)
            {
                areaSpazio.ITOT_EST_95 = datiGenericiCi.ContributiItalianiEdEsteriAl1295.HasValue ? (short)datiGenericiCi.ContributiItalianiEdEsteriAl1295.Value : (short)0;
            }

            if (datiIstruttoria != null)
            {
                areaSpazio.ISETAUTVV_D = datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi.HasValue ? (short)datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi.Value : (short)0;
                areaSpazio.ISETAUTVV_M = datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi.HasValue ? (short)datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi.Value : (short)0;
            }

            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);
            if (datiNuoveLiquidate != null)
            {
                areaSpazio.OPZIONE_CONTRIBUTIVA = datiNuoveLiquidate.FlagContributiva.HasValue ? datiNuoveLiquidate.FlagContributiva.Value ? "S" : "N" : "N";
            }

            if (areaTitolare != null && areaTitolare.Anagrafica != null)
            {
                areaSpazio.CODCOMUNE_R = areaTitolare.Anagrafica.CodiceComuneResidenza;
            }

            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                areaSpazio.REVISIONISANITARIE = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSpazio.RevisioneSanitaria>();
                for (int i = 0; i < listaFamiliari.Count; i++)
                {
                    INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSpazio.RevisioneSanitaria revisioneSanitaria = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSpazio.RevisioneSanitaria();
                    if (listaFamiliari[i].ScadenzaRevisioneSanitaria.HasValue)
                    {
                        revisioneSanitaria.TP1REVFA = (short)listaFamiliari[i].ScadenzaRevisioneSanitaria.Value.Year;
                        revisioneSanitaria.TP1REVFM = (short)listaFamiliari[i].ScadenzaRevisioneSanitaria.Value.Month;
                    }
                    areaSpazio.REVISIONISANITARIE.Add(revisioneSanitaria);
                }
            }

            if (datiMaggiorazioniBenefici != null)
            {
                areaSpazio.ANNI_ANTICIPO_544 = datiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.HasValue ? datiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.Value : (short)0;

                if (datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.HasValue)
                {
                    List<GestioneDecodifica.CodiceRequisitiLegge50392> listaCodiciRequisitiLegge50392 = null;
                    GestioneDecodifica.GetCodiceRequisitiLegge50392(out listaCodiciRequisitiLegge50392);
                    if (listaCodiciRequisitiLegge50392 != null && listaCodiciRequisitiLegge50392.Count > 0)
                    {
                        GestioneDecodifica.CodiceRequisitiLegge50392 codiceRequisitiLegge50392 = listaCodiciRequisitiLegge50392.Find(x => x.Id == datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.ToString());
                        areaSpazio.IREQA2C3_385 = codiceRequisitiLegge50392.TraduzioneSuGP.ToString();
                    }
                }
            }
        }

        private static void ValorizzaAreaSicurezza(string matricolaOperatore, out Data.PCIINPU7.AreaSicurezza areaSicurezza)
        {
            DateTime dataSistema = Utility.DataSistemaCi;

            areaSicurezza = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaSicurezza();

            areaSicurezza.DATA_OPER_A = (short)dataSistema.Year;
            areaSicurezza.DATA_OPER_M = (short)dataSistema.Month;
            areaSicurezza.DATA_OPER_G = (short)dataSistema.Day;
            try
            {
                areaSicurezza.MATRICOLA_OPER = int.Parse(matricolaOperatore);
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }
        }

        private static void ValorizzaAreaINAIL(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa danteCausa, out Data.PCIINPU7.AreaINAIL areaINAIL)
        {
            areaINAIL = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaINAIL();
            string siglaCategoria = datiPensione.SiglaCategoria.Trim().ToUpperInvariant();

            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa) || Utility.IsDomandaPensioneInabilita(datiPensione) ||
                Utility.IsAssegnoInvalidita(datiPensione) || (Utility.IsRicostituzione(datiPensione.Gruppo) && (siglaCategoria == "IOS" || siglaCategoria == "IRS" || siglaCategoria == "IOCOMS" ||
                siglaCategoria == "IOARTS")))
            {
                List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaPensioniInail = null;
                GestionePensioneInailInabilita.GetPensioniINAILByIdPensione(datiPensione.Id, out listaPensioniInail);

                if (listaPensioniInail != null && listaPensioniInail.Count > 0)
                {
                    List<Data.PCIINPU7.AreaINAIL.RenditaINAIL> listaInail = new List<Data.PCIINPU7.AreaINAIL.RenditaINAIL>();

                    foreach (GestionePensioneInailInabilita.DatiPensioniINAIL pensioniInail in listaPensioniInail)
                    {
                        Data.PCIINPU7.AreaINAIL.RenditaINAIL inail = new Data.PCIINPU7.AreaINAIL.RenditaINAIL();
                        inail.N_IDECINAA = pensioniInail.DecorrenzaRenditaInail.HasValue ? (short)pensioniInail.DecorrenzaRenditaInail.Value.Year : (short)0;
                        inail.N_IDECINAM = pensioniInail.DecorrenzaRenditaInail.HasValue ? (short)pensioniInail.DecorrenzaRenditaInail.Value.Month : (short)0;
                        inail.N_IIMPINAIL = pensioniInail.ImportoMensileInail.HasValue ? pensioniInail.ImportoMensileInail.Value : 0;
                        if (pensioniInail.Evento.HasValue)
                        {
                            inail.N_ICODINAIL = pensioniInail.Evento.Value ? "1" : "0";
                        }

                        listaInail.Add(inail);
                    }

                    areaINAIL.RENDITAINAIL = listaInail;
                }
            }
        }

        private static void ValorizzaAreaCodiciStampa(GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi, out Data.PCIINPU7.AreaCodiciStampa areaCodiciStampa)
        {
            areaCodiciStampa = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCodiciStampa();

            if (datiGenericiCi != null)
            {
                //ENG - Gestione Nuovo Codice CI28
                if (datiGenericiCi.CodiceCI28.HasValue)
                    areaCodiciStampa.CI281 = datiGenericiCi.CodiceCI28.Value.ToString();
                else
                    areaCodiciStampa.CI281 = datiGenericiCi.CodiciMotivazioniCi281;

                areaCodiciStampa.CI21 = datiGenericiCi.CodiciCi21.HasValue ? datiGenericiCi.CodiciCi21.Value.ToString() : "";
            }

        }
        #endregion Gruppo3

        #region Gruppo4
        private static void ValorizzaGruppo4(GestionePensione.DatiPensione datiPensione, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneDanteCausa.DatiDanteCausa danteCausa,
            ref Data.HostRequest.CI01_CI02RequestNew richiesta)
        {
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaCalcoloRetributivo);

            Data.PCIINPU7.AreaCampi2004 areaCampi2004 = null;
            ValorizzaAreaCampi2004(datiPensione, listaPrestazioniEE, out areaCampi2004);
            richiesta.Gruppo4.AreaCampi2004 = areaCampi2004;

            Data.PCIINPU7.AreaCampiVar areaCampiVar = null;
            ValorizzaAreaCampiVar(listaCalcoloRetributivo, datiPensione, listaPrestazioniEE, out areaCampiVar);
            richiesta.Gruppo4.AreaCampiVar = areaCampiVar;

            Data.PCIINPU7.AreaCampi2017 areaCampi2017 = null;
            ValorizzaAreaCampi2017(datiPensione, out areaCampi2017);
            richiesta.Gruppo4.AreaCampi2017 = areaCampi2017;

            Data.PCIINPU7.AreaCampi2018 areaCampi2018 = null;
            ValorizzaAreaCampi2018(datiPensione, datiMaggiorazioniBenefici, danteCausa, out areaCampi2018);
            richiesta.Gruppo4.AreaCampi2018 = areaCampi2018;

            Data.PCIINPU7.AreaFlags areaFlags = null;
            Data.PCIINPU7.AreaPostFlags areaPostFlags = null;
            ValorizzaAreaFlags(out areaFlags, out areaPostFlags);
            richiesta.Gruppo4.AreaFlags = areaFlags;
            richiesta.Gruppo5.AreaPostFlags = areaPostFlags;
        }

        private static void ValorizzaAreaCampi2004(GestionePensione.DatiPensione datiPensione, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE,
            out Data.PCIINPU7.AreaCampi2004 areaCampi2004)
        {
            areaCampi2004 = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCampi2004();

            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
            {
                List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
                GestioneDatiContributiviCi.GetImportiEsteriByIdPensione(datiPensione.Id, out listaImportiEsteri);

                areaCampi2004.STATIESTERI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCampi2004.StatoEstero>();
                foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEE)
                {
                    Data.PCIINPU7.AreaCampi2004.StatoEstero statoEstero = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCampi2004.StatoEstero();
                    statoEstero.ART48 = prestEE.CodiceArt48.HasValue ? prestEE.CodiceArt48.Value.ToString() : "N";
                    statoEstero.COD_SOSP_ESTERO = prestEE.SospensioneCautelativaIntegrazione.HasValue ? prestEE.SospensioneCautelativaIntegrazione.Value.ToString() : "N";
                    statoEstero.DECART48A = prestEE.DecorrenzaArt48.HasValue ? (short)prestEE.DecorrenzaArt48.Value.Year : (prestEE.CodiceArt48.HasValue && prestEE.CodiceArt48.Value.ToString() == "S" ? (short)9999 : (short)0);
                    statoEstero.DECART48M = prestEE.DecorrenzaArt48.HasValue ? (short)prestEE.DecorrenzaArt48.Value.Month : (prestEE.CodiceArt48.HasValue && prestEE.CodiceArt48.Value.ToString() == "S" ? (short)99 : (short)0);
                    statoEstero.ETA_SOSP_ESTERO = prestEE.EtaSospensione.HasValue ? prestEE.EtaSospensione.Value : (short)0;
                    statoEstero.IDAPLIQA = prestEE.DecorrenzaLiquidazioneStatoEE.HasValue ? (short)prestEE.DecorrenzaLiquidazioneStatoEE.Value.Year : (short)0;
                    statoEstero.IDAPLIQM = prestEE.DecorrenzaLiquidazioneStatoEE.HasValue ? (short)prestEE.DecorrenzaLiquidazioneStatoEE.Value.Month : (short)0;
                    short resShort = 0;
                    short.TryParse(prestEE.CodiceIstituzione, out resShort);
                    statoEstero.ISTIT = resShort;
                    statoEstero.MATRIC = prestEE.MatricolaIstituzioneEE;
                    statoEstero.RICALSTATOA = prestEE.DecorrenzaRicalcolo.HasValue ? (short)prestEE.DecorrenzaRicalcolo.Value.Year : (short)0;
                    statoEstero.RICALSTATOM = prestEE.DecorrenzaRicalcolo.HasValue ? (short)prestEE.DecorrenzaRicalcolo.Value.Month : (short)0;
                    statoEstero.SETT1 = prestEE.ContributiEEDecorrenzaOriginaria.HasValue ? prestEE.ContributiEEDecorrenzaOriginaria.Value : 0;
                    statoEstero.SETT2 = prestEE.ContributiEERicalcolo.HasValue ? prestEE.ContributiEERicalcolo.Value : 0;
                    statoEstero.SETTDIR = prestEE.ContributiEEDiritto.HasValue ? prestEE.ContributiEEDiritto.Value : 0;
                    resShort = 0;
                    short.TryParse(prestEE.CodiceStatoEE, out resShort);
                    statoEstero.STATO = resShort;
                    if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                    {
                        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPerStatoEstero = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestEE.Id);
                        if (listaImportiEsteriPerStatoEstero != null && listaImportiEsteriPerStatoEstero.Count > 0)
                        {
                            statoEstero.IMPORTI = new List<INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCampi2004.StatoEstero.Importo>();
                            foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importoEstero in listaImportiEsteriPerStatoEstero)
                            {
                                Data.PCIINPU7.AreaCampi2004.StatoEstero.Importo importo = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCampi2004.StatoEstero.Importo();
                                importo.CESAA = importoEstero.CessazionePrestazioneEE.HasValue ? (short)importoEstero.CessazionePrestazioneEE.Value.Year : (short)0;
                                importo.CESMM = importoEstero.CessazionePrestazioneEE.HasValue ? (short)importoEstero.CessazionePrestazioneEE.Value.Month : (short)0;
                                importo.DECAA = importoEstero.DecorrenzaPrestazioneEE.HasValue ? (short)importoEstero.DecorrenzaPrestazioneEE.Value.Year : (short)0;
                                importo.DECMM = importoEstero.DecorrenzaPrestazioneEE.HasValue ? (short)importoEstero.DecorrenzaPrestazioneEE.Value.Month : (short)0;
                                importo.IMPEST = importoEstero.ImportoPrestazioneEE.HasValue ? importoEstero.ImportoPrestazioneEE.Value : 0M;
                                importo.PERIODIC = "M";
                                statoEstero.IMPORTI.Add(importo);
                            }
                        }
                    }
                    areaCampi2004.STATIESTERI.Add(statoEstero);
                }
            }
        }

        private static void ValorizzaAreaCampiVar(List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo, GestionePensione.DatiPensione datiPensione, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE,
            out Data.PCIINPU7.AreaCampiVar areaCampiVar)
        {
            areaCampiVar = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCampiVar();

            areaCampiVar.Dati_2016 = new Data.PCIINPU7.AreaCampiVar.Dati2016();

            if (listaCalcoloRetributivo != null)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo calcoloRetributivo in listaCalcoloRetributivo)
                {
                    if (calcoloRetributivo.CodiceGestione.HasValue && calcoloRetributivo.NSettimane707.HasValue)
                    {
                        switch (calcoloRetributivo.CodiceGestione.Value)
                        {
                            case 1:
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'A')
                                    areaCampiVar.Dati_2016.GP2BC10OBGA = calcoloRetributivo.NSettimane707.Value;
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'B')
                                    areaCampiVar.Dati_2016.GP2BC10OBGB = calcoloRetributivo.NSettimane707.Value;
                                break;
                            case 2:
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'A')
                                    areaCampiVar.Dati_2016.GP2BC10CDMA = calcoloRetributivo.NSettimane707.Value;
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'B')
                                    areaCampiVar.Dati_2016.GP2BC10CDMB = calcoloRetributivo.NSettimane707.Value;
                                break;
                            case 3:
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'A')
                                    areaCampiVar.Dati_2016.GP2BC10ARTA = calcoloRetributivo.NSettimane707.Value;
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'B')
                                    areaCampiVar.Dati_2016.GP2BC10ARTB = calcoloRetributivo.NSettimane707.Value;
                                break;
                            case 4:
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'A')
                                    areaCampiVar.Dati_2016.GP2BC10COMA = calcoloRetributivo.NSettimane707.Value;
                                if (calcoloRetributivo.QuotePrimeLiquidate == 'B')
                                    areaCampiVar.Dati_2016.GP2BC10COMB = calcoloRetributivo.NSettimane707.Value;
                                break;
                        }

                        areaCampiVar.Dati_2016.FLAG2016 = "16";
                    }
                }
            }

            Data.PCIINPU7.AreaCampiVar.ModelloStampa modelloStampa = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaCampiVar.ModelloStampa();
            areaCampiVar.MODELLISTAMPA = new List<Data.PCIINPU7.AreaCampiVar.ModelloStampa>();

            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out listaResidenzeEstere);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            GestioneDecodifica.StatoEstero statoEstero = null;
            bool isResidenteInItalia = false;

            List<GestioneAnagrafica.DatiResidenzaEstero> listaDecorrenzeResidenza = listaResidenzeEstere.OrderBy(x => x.Decorrenza).ToList();
            if (listaDecorrenzeResidenza != null && listaDecorrenzeResidenza.Count > 0)
            {
                GestioneAnagrafica.DatiResidenzaEstero ultimaResidenza = listaDecorrenzeResidenza.Last();
                GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(ultimaResidenza.CodCatastaleStatoEE, out statoEstero);
            }

            if ((statoEstero != null && statoEstero.Sigla == "I") || !(datiAnagrafici.ResidenzaEstero.HasValue && datiAnagrafici.ResidenzaEstero.Value))
                isResidenteInItalia = true;

            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
            {
                if (listaPrestazioniEE[0].CodiceConvenzione.GetValueOrDefault() == 12 && !isResidenteInItalia)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (i < listaPrestazioniEE.Count() && (listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'A' || listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'R'))
                        {
                            modelloStampa.STAMOD_X = "1";
                            areaCampiVar.MODELLISTAMPA.Add(modelloStampa);
                        }
                    }
                }
                else if (listaPrestazioniEE[0].CodiceConvenzione.GetValueOrDefault() == 12 && isResidenteInItalia)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (i < listaPrestazioniEE.Count() && (listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'A' || listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'R'))
                        {
                            modelloStampa.STAMOD_X = "2";
                            areaCampiVar.MODELLISTAMPA.Add(modelloStampa);
                        }
                    }
                }
                else if (listaPrestazioniEE[0].CodiceConvenzione.GetValueOrDefault() != 12 && !isResidenteInItalia)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (i < listaPrestazioniEE.Count() && (listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'A' || listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'R'))
                        {
                            modelloStampa.STAMOD_X = "3";
                            areaCampiVar.MODELLISTAMPA.Add(modelloStampa);
                        }
                    }
                }
                else if (listaPrestazioniEE[0].CodiceConvenzione.GetValueOrDefault() != 12 && isResidenteInItalia)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (i < listaPrestazioniEE.Count() && (listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'A' || listaPrestazioniEE[i].CodicePi.GetValueOrDefault() == 'R'))
                        {
                            modelloStampa.STAMOD_X = "4";
                            areaCampiVar.MODELLISTAMPA.Add(modelloStampa);
                        }
                    }
                }
            }
        }

        private static void ValorizzaAreaCampi2017(GestionePensione.DatiPensione datiPensione, out Data.PCIINPU7.AreaCampi2017 areaCampi2017)
        {
            areaCampi2017 = new Data.PCIINPU7.AreaCampi2017();

            areaCampi2017.GP1DGRP = datiPensione.Gruppo;
            areaCampi2017.GP1DPRD = datiPensione.Prodotto;
            areaCampi2017.GP1DTIP = datiPensione.Tipo;
            areaCampi2017.GP1DTIPOL = datiPensione.GetFiltro();

            Data.PCIINPU7.AreaWKAUT areaWKAUT = null;
            ValorizzaAreaWKAUT(out areaWKAUT);
            areaCampi2017.AreaWKAUT = areaWKAUT;
        }

        private static void ValorizzaAreaCampi2018(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneDanteCausa.DatiDanteCausa danteCausa, out Data.PCIINPU7.AreaCampi2018 areaCampi2018)
        {
            areaCampi2018 = new Data.PCIINPU7.AreaCampi2018();

            List<GestioneOneri.DatiOneri> listaOneri = null;
            GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaOneri);
            if (listaOneri != null && listaOneri.Count > 0)
            {
                areaCampi2018.FELPE_ONERI = new List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri>();
                foreach (GestioneOneri.DatiOneri o in listaOneri)
                {
                    Data.PCIINPU7.AreaCampi2018.Felpe_Oneri onere = new Data.PCIINPU7.AreaCampi2018.Felpe_Oneri();

                    onere.FELPE_DECONERE = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0')) : string.Empty;
                    if (datiPensione.DecorrenzaOriginaria.HasValue && o.Scadenza.HasValue &&
                        Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, o.Scadenza.Value))
                    {
                        onere.FELPE_SCADENZA = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0')) : string.Empty;
                    }
                    else
                    {
                        onere.FELPE_SCADENZA = o.Scadenza.HasValue ? (o.Scadenza.Value.Year.ToString().PadLeft(4, '0') +
                        o.Scadenza.Value.Month.ToString().PadLeft(2, '0') +
                        o.Scadenza.Value.Day.ToString().PadLeft(2, '0')) : string.Empty;
                    }
                    List<GestioneDecodifica.GruppoOneri> listaGruppoOneri = null;
                    GestioneDecodifica.GetGruppoOneri(out listaGruppoOneri);
                    if (listaGruppoOneri != null && listaGruppoOneri.Count > 0)
                    {
                        GestioneDecodifica.GruppoOneri gruppoOneri = listaGruppoOneri.Find(x => x.Id == (o.IdCodeGruppo.HasValue ? o.IdCodeGruppo.Value : (long)0));
                        if (gruppoOneri != null)
                        {
                            onere.FELPE_CODGRUP = gruppoOneri.Code;
                        }
                    }
                    List<GestioneDecodifica.SottoGruppoOneri> listaSottoGruppoOneri = null;
                    GestioneDecodifica.GetSottoGruppoOneri(out listaSottoGruppoOneri);
                    if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                    {
                        GestioneDecodifica.SottoGruppoOneri sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Id == (o.IdCodeSottoGruppo.HasValue ? o.IdCodeSottoGruppo.Value : (long)0));
                        if (sottoGruppoOneri != null)
                        {
                            onere.FELPE_CODSGRUP = sottoGruppoOneri.Code;
                        }
                    }
                    onere.FELPE_ONERE = o.Onere.GetValueOrDefault();
                    onere.FELPE_ANZCON = o.Settimane.GetValueOrDefault();

                    if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                        Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
                    {
                        onere.FELPE_GP2PBNFGL = datiPensione.NumeroFigli.HasValue ? datiPensione.NumeroFigli.Value.ToString() : string.Empty;

                        if (!string.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta))
                        {
                            if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true))
                            {
                                if (datiPensione.CodiceTipoRichiesta == "KW" || datiPensione.CodiceTipoRichiesta == "KX")
                                    onere.FELPE_CODBENEF = "20";
                            }
                            else if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true))
                            {
                                if (datiPensione.CodiceTipoRichiesta == "KY" || datiPensione.CodiceTipoRichiesta == "KZ")
                                    onere.FELPE_CODBENEF = "21";
                            }
                            else if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
                            {
                                if (datiPensione.CodiceTipoRichiesta == "KU" || datiPensione.CodiceTipoRichiesta == "KV")
                                    onere.FELPE_CODBENEF = "22";
                            }
                        }
                    }

                    if (Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) ||
                        Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione))
                    {
                        onere.FELPE_GP2PBNFGL = datiPensione.NumeroFigli.HasValue ? datiPensione.NumeroFigli.Value.ToString() : string.Empty;
                    }

                    areaCampi2018.FELPE_ONERI.Add(onere);
                }
            }

            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaBeneficiParticolari = null;
            GestioneBeneficiParticolari.GetBeneficiParticolariByIdPensione(datiPensione.Id, datiPensione, out listaBeneficiParticolari);
            if (listaBeneficiParticolari != null && listaBeneficiParticolari.Count > 0)
            {
                if (areaCampi2018.FELPE_ONERI == null)
                    areaCampi2018.FELPE_ONERI = new List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri>();

                for (int i = 0; i < listaBeneficiParticolari.Count; i++)
                {
                    //rimossi 12 e 15 perchè vengono gestiti più sotto (segnalazione duplicazione area)
                    if (listaBeneficiParticolari[i].CodiceBenefici != "12" && listaBeneficiParticolari[i].CodiceBenefici != "15")
                    {
                        if (areaCampi2018.FELPE_ONERI.Count > i)
                        {
                            areaCampi2018.FELPE_ONERI[i].FELPE_CODBENEF = listaBeneficiParticolari[i].CodiceBenefici;
                            areaCampi2018.FELPE_ONERI[i].FELPE_ANZBENEF = listaBeneficiParticolari[i].Settimane.GetValueOrDefault();
                        }
                        else
                        {
                            Data.PCIINPU7.AreaCampi2018.Felpe_Oneri onere = new Data.PCIINPU7.AreaCampi2018.Felpe_Oneri();
                            onere.FELPE_CODBENEF = listaBeneficiParticolari[i].CodiceBenefici;
                            onere.FELPE_ANZBENEF = listaBeneficiParticolari[i].Settimane.GetValueOrDefault();
                            areaCampi2018.FELPE_ONERI.Add(onere);
                        }
                    }
                }
            }

            // Mapping del tipo beneficio 10 - BENEFICI PREVISTI PER EX ART 24 COMMA 15 BIS
            if (datiMaggiorazioniBenefici != null && (new List<string> { "10", "11", "14", "18", "19", "24" }).Contains(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
            {
                if (areaCampi2018.FELPE_ONERI == null)
                    areaCampi2018.FELPE_ONERI = new List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri>();

                Data.PCIINPU7.AreaCampi2018.Felpe_Oneri onere = null;
                if (areaCampi2018.FELPE_ONERI != null && areaCampi2018.FELPE_ONERI.Count > 0)
                {
                    onere = areaCampi2018.FELPE_ONERI.Find(x => string.IsNullOrEmpty(x.FELPE_CODBENEF) && x.FELPE_ANZBENEF == 0);
                    if (onere != null)
                        onere.FELPE_CODBENEF = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                }

                if (onere == null)
                {
                    onere = new Data.PCIINPU7.AreaCampi2018.Felpe_Oneri();
                    onere.FELPE_CODBENEF = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                    onere.FELPE_DECONERE = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0')) : string.Empty;

                    areaCampi2018.FELPE_ONERI.Add(onere);
                }
            }

            Data.PCIINPU7.AreaINAIL areaInail = null;
            ValorizzaAreaINAIL(datiPensione, danteCausa, out areaInail);
            areaCampi2018.AreaINAIL = areaInail;

            if (datiPensione.Flag5000.HasValue && datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                areaCampi2018.FLAG_5000 = Convert.ToString(datiPensione.Flag5000.Value);

            if (!(Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                 Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)))
            {
                if (datiPensione.NumeroFigli.HasValue)
                {
                    if (areaCampi2018.FELPE_ONERI == null)
                        areaCampi2018.FELPE_ONERI = new List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri>();

                    if (datiMaggiorazioniBenefici != null && (new List<string> { "15", "12" }).Contains(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                    {
                        Data.PCIINPU7.AreaCampi2018.Felpe_Oneri onere = null;
                        if (areaCampi2018.FELPE_ONERI != null && areaCampi2018.FELPE_ONERI.Count > 0)
                        {
                            onere = areaCampi2018.FELPE_ONERI.Find(x => string.IsNullOrEmpty(x.FELPE_CODBENEF) && x.FELPE_ANZBENEF == 0);
                            if (onere != null)
                            {
                                onere.FELPE_CODBENEF = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                                onere.FELPE_GP2PBNFGL = datiPensione.NumeroFigli.ToString();
                            }
                        }

                        if (onere == null)
                        {
                            onere = new Data.PCIINPU7.AreaCampi2018.Felpe_Oneri();
                            onere.FELPE_CODBENEF = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                            onere.FELPE_DECONERE = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                                datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0')) : string.Empty;
                            onere.FELPE_GP2PBNFGL = datiPensione.NumeroFigli.ToString();
                            areaCampi2018.FELPE_ONERI.Add(onere);
                        }
                    }
                    else
                    {
                        if (areaCampi2018.FELPE_ONERI.Count > 0)
                            areaCampi2018.FELPE_ONERI.FirstOrDefault().FELPE_GP2PBNFGL = datiPensione.NumeroFigli.ToString();
                        else
                        {
                            Data.PCIINPU7.AreaCampi2018.Felpe_Oneri onere;
                            onere = new Data.PCIINPU7.AreaCampi2018.Felpe_Oneri();
                            onere.FELPE_GP2PBNFGL = datiPensione.NumeroFigli.ToString();

                            //ENG - Memo 28_2024 - Figli senza benefici
                            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
                            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
                            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                            {
                                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") ||
                                    (datiPensione.IdTipoPLPerRIC == 7 && !String.IsNullOrEmpty(datiPensione.NaturaPensione) && (datiPensione.NaturaPensione.Substring(0, 1) == "1") || (datiPensione.NaturaPensione.Substring(0, 1) == "2")))
                                {
                                    if (datiPensione.DecorrenzaOriginaria.HasValue &&
                                        Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 1, 1)))
                                    {
                                        if (datiMaggiorazioniBenefici == null || String.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                                        {
                                            if (datiPensione.NumeroFigli.HasValue && datiPensione.NumeroFigli.Value > 0)
                                            {
                                                onere.FELPE_DECONERE = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                                                                                                     datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                                                                                                                     datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0')) : string.Empty;
                                            }
                                        }
                                    }
                                }
                            }

                            areaCampi2018.FELPE_ONERI.Add(onere);
                        }
                    }
                }
            }

            if (datiPensione.GP1AV91A.HasValue)
            {
                if (areaInail.SENTENZA_IGP1AV91A == null)
                    areaInail.SENTENZA_IGP1AV91A = new Data.PCIINPU7.AreaINAIL.Sentenza_IGP1AV91A();
                areaInail.SENTENZA_IGP1AV91A.IGP1AV91A = datiPensione.GP1AV91A.Value;
            }
        }

        private static void ValorizzaAreaFlags(out Data.PCIINPU7.AreaFlags areaFlags, out Data.PCIINPU7.AreaPostFlags areaPostFlags)
        {
            areaFlags = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaFlags();
            areaPostFlags = new INPS.Pensioni.LiquidazioneCi.Data.PCIINPU7.AreaPostFlags();

            areaFlags.W_CAMPO36 = "";
            areaFlags.FILLER = "";

            areaPostFlags.FILLER_FINE_ZERI = "0000000";
            areaPostFlags.FINE_REC_100K = "FINE";
        }
        #endregion Gruppo4
        #endregion private methods
    }
}
