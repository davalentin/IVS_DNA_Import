using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneCi.Data;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;
using System.Reflection;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneCalcoloDomanda
    {
        #region public members
        public static void CalcolaDomanda(GestionePensione.DatiPensione datiPensione, long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = string.Empty;
            messaggioVideo = string.Empty;

            if (datiPensione == null)
                throw new INPS.DNA.DnaApplicationException("Nessuna pensione associata al numero di domanda: " + numeroDomanda);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            object AreaCalcolo = null;
            ValorizzaAreaCalcolo(matricolaOperatore, sedeOperatore, centroOperativoOperatore, datiPensione, tipoDomanda, isRiaperturaDomanda, out AreaCalcolo);
            // Il salvataggio del LogSOAP viene fatto dentro EseguiCalcolo
            EseguiCalcolo(AreaCalcolo, tipoDomanda, isRiaperturaDomanda, datiPensione.NDomus);
            ControllaEsitoCalcolo(datiPensione.NDomus, datiPensione.ProgStorico, AreaCalcolo, tipoDomanda, isRiaperturaDomanda, out statoPensione, out esito, out messaggioVideo);
        }

        public static bool ControlsDatiCalcolaDomanda(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, bool isConsultazioniANFVerificate, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            listaConsultazioniANF = null;
            DateTime dataSistema = Utility.DataSistemaCi;

            #region GetData

            GestioneAnagrafica.DatiAnagrafici datiDelegato = null;
            GestioneDelegatoTutore.GetDelegatoByIdPensione(datiPensione.Id, out datiDelegato);

            GestioneAnagrafica.DatiAnagrafici datiTutore = null;
            GestioneDelegatoTutore.GetTutoreByIdPensione(datiPensione.Id, out datiTutore);

            GestionePensione.DatiSindacato sindacato = null;
            GestionePensione.GetSindacatoByIdPensione(datiPensione.Id, out sindacato);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiCi);

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);

            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            List<GestioneContrib.StatoEstero> listaStatiEsteri = null;
            GestioneContrib.GetStatiEEfromDBByIdPensione(datiPensione.Id, listaPrestazioniEstere, out listaStatiEsteri);

            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out listaResidenzeEstere);

            List<GestioneDanteCausa.DatiRedditoSentenza495_93> listaRedditoSentenza495_93 = null;
            GestioneDanteCausa.GetRedditiSentenza495_93ByIdPensione(datiPensione.Id, out listaRedditoSentenza495_93);

            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
            GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCtrlCodiceConvenzionePrestazioniEE(out listaCodiciConvenzione);

            int codicePrimoStatoEE = 0;
            string codiceIstituzione = string.Empty;
            string nomeStato = string.Empty;
            byte? codiceConvenzione = null;
            if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            {
                string codiceStato = listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE;
                codiceIstituzione = listaStatiEsteri[0].PrestazioneEstera.CodiceIstituzione;
                nomeStato = listaStatiEsteri[0].PrestazioneEstera.NomeStato;
                codiceConvenzione = listaStatiEsteri[0].PrestazioneEstera.CodiceConvenzione;

                int.TryParse(codiceStato, out codicePrimoStatoEE);
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareDB = null;
            GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareDB);

            GestioneDecodifica.CodiceParticolare codiceParticolareSoggettoDerogato = null;
            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                codiceParticolareSoggettoDerogato = elencoCodiceParticolareDB.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);

            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficheFamiliari);

            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
            GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaCodMaggFamiliari);

            AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            GestionePagamento.DatiPagamento datiPagamento = null;
            GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            List<DatiSupplementi> listaDatiSupplementi = null;
            GestioneSupplementi.GetSupplementiByIdPensione(datiPensione.Id, out listaDatiSupplementi);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out datiAnagraficiDC);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
            GestioneDatiContributiviCi.GetImportiEsteriByIdPensione(datiPensione.Id, out listaImportiEsteri);

            List<GestioneDanteCausa.PensioniEstereDcBL> LpensioniEstereDcBL = null;
            GestioneDanteCausa.GetPensioniEstereDCByIdPensione(datiPensione.Id, out LpensioniEstereDcBL);

            List<GestioneDatiContributiviCi.PensioniCiImportiValuta> listaImportiValuta = null;
            GestioneDatiContributiviCi.GetImportiEsteriValutaByIdPensione(datiPensione.Id, out listaImportiValuta);

            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

            List<GestioneCalcolo.DatiCalcoloContributivoEstero> listaDatiCalcoloContributivoEstero = null;
            GestioneCalcolo.GetCalcoloContributivoEsteroCIbyIdPensione(datiPensione.Id, out listaDatiCalcoloContributivoEstero);

            GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11 = null;
            GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(datiPensione.Id, out integrazioneArt11);

            List<Entity.AltraPensione> listaAltrePensioni = null;
            GestioneBititolarita.GetDatiAltraPensioneByIdPensione(datiPensione.Id, out listaAltrePensioni);

            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> listaMaternitaAcna = null;
            GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(datiPensione.Id, out listaMaternitaAcna);

            GestioneAnagrafica.DatiStatoCivile ultimoStatoCivile = null;
            if (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0)
                ultimoStatoCivile = areaTitolare.ElencoStatiCivili.Last();

            List<GestioneDecodifica.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392 = null;
            GestioneDecodifica.GetCodiceRequisitiLegge50392(out listaCodiceRequisitiLegge50392);

            List<GestioneDecodifica.CodeGestione> listaCodiciGestione = null;
            GestioneDecodifica.GetCodiceGestione(out listaCodiciGestione);

            List<GestioneRedditi.RedditoDRedd> lstRedditi = null;
            GestioneRedditi.GetRedditiDReddByIdPensione(datiPensione.Id, out lstRedditi);

            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaDatiOneri);

            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetributivo = null;
            GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out listaCodeGestioneCalcoloRetributivo);

            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo = null;
            GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out listaCodeGestioneCalcoloContributivo);

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            List<GestioneDecodifica.GruppoOneri> elencoDecCodeGruppoOneri = null;
            GestioneDecodifica.GetGruppoOneri(out elencoDecCodeGruppoOneri);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

            int? settimaneRetributiveQuotaACodGestione1 = null;
            int? settimaneRetributiveQuotaACodGestione2 = null;
            int? settimaneRetributiveQuotaACodGestione3 = null;
            int? settimaneRetributiveQuotaACodGestione4 = null;
            decimal? rmsQuotaACodGestione1 = null;
            decimal? rmsQuotaACodGestione2 = null;
            decimal? rmsQuotaACodGestione3 = null;
            decimal? rmsQuotaACodGestione4 = null;
            int? settimaneRetributiveQuotaBCodGestione1 = null;
            int? settimaneRetributiveQuotaBCodGestione2 = null;
            int? settimaneRetributiveQuotaBCodGestione3 = null;
            int? settimaneRetributiveQuotaBCodGestione4 = null;
            decimal? rmsQuotaBCodGestione1 = null;
            decimal? rmsQuotaBCodGestione2 = null;
            decimal? rmsQuotaBCodGestione3 = null;
            decimal? rmsQuotaBCodGestione4 = null;
            int? settimaneContributiveCodGestione1 = null;
            int? settimaneContributiveCodGestione2 = null;
            int? settimaneContributiveCodGestione3 = null;
            int? settimaneContributiveCodGestione4 = null;
            int? settimaneContributiveDL214CodGestione1 = null;
            int? settimaneContributiveDL214CodGestione2 = null;
            int? settimaneContributiveDL214CodGestione3 = null;
            int? settimaneContributiveDL214CodGestione4 = null;
            decimal? montanteCodGestione1 = null;
            decimal? montanteCodGestione2 = null;
            decimal? montanteCodGestione3 = null;
            decimal? montanteCodGestione4 = null;
            decimal? importoContributivoTotaleCodGestione1 = null;
            decimal? importoContributivoTotaleCodGestione2 = null;
            decimal? importoContributivoTotaleCodGestione3 = null;
            decimal? importoContributivoTotaleCodGestione4 = null;
            decimal? montanteContributivoQuotaDCodGestione1 = null;
            decimal? montanteContributivoQuotaDCodGestione2 = null;
            decimal? montanteContributivoQuotaDCodGestione3 = null;
            decimal? montanteContributivoQuotaDCodGestione4 = null;
            decimal? importoContributivoTotaleQuotaDCodGestione1 = null;
            decimal? importoContributivoTotaleQuotaDCodGestione2 = null;
            decimal? importoContributivoTotaleQuotaDCodGestione3 = null;
            decimal? importoContributivoTotaleQuotaDCodGestione4 = null;
            int? settimane707QuotaBCodGestione1 = null;
            int? settimane707QuotaBCodGestione2 = null;
            int? settimane707QuotaBCodGestione3 = null;
            int? settimane707QuotaBCodGestione4 = null;

            int? sommaSettimaneContributi = 0;

            if (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo retr in listaDatiCalcoloRetributivo)
                {
                    if (retr.CodiceGestione == 1)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            settimaneRetributiveQuotaACodGestione1 = retr.NSettimaneQuotaA;
                            rmsQuotaACodGestione1 = retr.RMSQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione1.GetValueOrDefault();
                        }
                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            settimaneRetributiveQuotaBCodGestione1 = retr.NSettimaneQuotaB;
                            rmsQuotaBCodGestione1 = retr.RMSQuotaB;
                            settimane707QuotaBCodGestione1 = retr.NSettimane707;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault();
                        }
                    }

                    if (retr.CodiceGestione == 2)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione2 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione2 = retr.NSettimaneQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione2.GetValueOrDefault();
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione2 = retr.RMSQuotaB;
                            settimaneRetributiveQuotaBCodGestione2 = retr.NSettimaneQuotaB;
                            settimane707QuotaBCodGestione2 = retr.NSettimane707;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione2.GetValueOrDefault();
                        }
                    }

                    if (retr.CodiceGestione == 3)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione3 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione3 = retr.NSettimaneQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione3.GetValueOrDefault();
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione3 = retr.RMSQuotaB;
                            settimaneRetributiveQuotaBCodGestione3 = retr.NSettimaneQuotaB;
                            settimane707QuotaBCodGestione3 = retr.NSettimane707;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione3.GetValueOrDefault();
                        }
                    }

                    if (retr.CodiceGestione == 4)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione4 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione4 = retr.NSettimaneQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione4.GetValueOrDefault();
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione4 = retr.RMSQuotaB;
                            settimaneRetributiveQuotaBCodGestione4 = retr.NSettimaneQuotaB;
                            settimane707QuotaBCodGestione4 = retr.NSettimane707;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault();
                        }
                    }
                }
            }

            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo contr in listaDatiCalcoloContributivo)
                {
                    if (contr.CodiceGestione == 1)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione1 = contr.NSettimane;
                            importoContributivoTotaleCodGestione1 = contr.ImportoContributivoTotale;
                            montanteCodGestione1 = contr.Montante;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione1.GetValueOrDefault();
                        }
                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione1 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione1 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione1 = contr.MontanteQuotaDL214;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione1.GetValueOrDefault();
                        }
                    }

                    if (contr.CodiceGestione == 2)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione2 = contr.NSettimane;
                            montanteCodGestione2 = contr.Montante;
                            importoContributivoTotaleCodGestione2 = contr.ImportoContributivoTotale;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione2.GetValueOrDefault();
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione2 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione2 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione2 = contr.MontanteQuotaDL214;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione2.GetValueOrDefault();
                        }
                    }

                    if (contr.CodiceGestione == 3)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione3 = contr.NSettimane;
                            montanteCodGestione3 = contr.Montante;
                            importoContributivoTotaleCodGestione3 = contr.ImportoContributivoTotale;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione3.GetValueOrDefault();
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione3 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione3 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione3 = contr.MontanteQuotaDL214;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione3.GetValueOrDefault();
                        }
                    }

                    if (contr.CodiceGestione == 4)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione4 = contr.NSettimane;
                            montanteCodGestione4 = contr.Montante;
                            importoContributivoTotaleCodGestione4 = contr.ImportoContributivoTotale;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione4.GetValueOrDefault();
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione4 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione4 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione4 = contr.MontanteQuotaDL214;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione4.GetValueOrDefault();
                        }
                    }
                }
            }

            decimal rmsQuotaATotale = rmsQuotaACodGestione1.GetValueOrDefault() + rmsQuotaACodGestione2.GetValueOrDefault() + rmsQuotaACodGestione3.GetValueOrDefault() +
                                      rmsQuotaACodGestione4.GetValueOrDefault();

            decimal rmsQuotaBTotale = rmsQuotaBCodGestione1.GetValueOrDefault() + rmsQuotaBCodGestione2.GetValueOrDefault() + rmsQuotaBCodGestione3.GetValueOrDefault() +
                                      rmsQuotaBCodGestione4.GetValueOrDefault();

            int settimaneQuotaATotale = settimaneRetributiveQuotaACodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione2.GetValueOrDefault() +
                                        settimaneRetributiveQuotaACodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione4.GetValueOrDefault();

            int settimaneQuotaBTotale = settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione2.GetValueOrDefault() +
                                        settimaneRetributiveQuotaBCodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault();

            int settimaneQuotaCTotale = settimaneContributiveCodGestione1.GetValueOrDefault() + settimaneContributiveCodGestione2.GetValueOrDefault() +
                                        settimaneContributiveCodGestione3.GetValueOrDefault() + settimaneContributiveCodGestione4.GetValueOrDefault();

            int settimaneQuotaDTotale = settimaneContributiveDL214CodGestione1.GetValueOrDefault() + settimaneContributiveDL214CodGestione2.GetValueOrDefault() +
                                        settimaneContributiveDL214CodGestione3.GetValueOrDefault() + settimaneContributiveDL214CodGestione4.GetValueOrDefault();

            int? settimane707QuotaBTotali = settimane707QuotaBCodGestione1.GetValueOrDefault() + settimane707QuotaBCodGestione2.GetValueOrDefault() + settimane707QuotaBCodGestione3.GetValueOrDefault() + settimane707QuotaBCodGestione4.GetValueOrDefault();


            int? sommaSettimaneDirittoEstere = null;

            foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
            {
                sommaSettimaneDirittoEstere = sommaSettimaneDirittoEstere.GetValueOrDefault() + prestEE.ContributiEEDiritto.GetValueOrDefault();
            }

            //////////////////////////////// settiamo il numero di settimane in base alla categoria////////////////////
            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);
            int? settimane = GestioneControlli.NumeroSettimane(datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null,
                datiIstruttoria != null ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi : null);
            if (categoria > 0 && categoria < 7)
            {
                settimane = settimane.GetValueOrDefault() + (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0);
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////Routine CTR Minimi/////////////////////////////////////////////////////////
            int ctrMinimi = GestioneControlli.CTR_Minimi(codiceConvenzione, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria,
                datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.Gruppo, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return false;
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            decimal? montante = null;
            int settimaneItalianeMisura = datiGenericiCi != null && datiGenericiCi.SettimaneItalianeMisura.HasValue ? datiGenericiCi.SettimaneItalianeMisura.Value :
                GestioneLiquidazionePensione.GetNumeroSettimaneItalianeMisura(listaDatiCalcoloContributivo, listaDatiCalcoloRetributivo);

            long? codiceGestioneContributiEsteri = null;
            if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
                codiceGestioneContributiEsteri = listaDatiCalcoloContributivoEstero[0].CodiceGestione;
            short? primoCodiceGestioneTraduzioneSuGP = 0;
            if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
            {
                GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == codiceGestioneContributiEsteri);
                if (codeGestione != null)
                    primoCodiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
            }

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            char? sesso;
            if (datiAnagraficiDC != null && datiAnagraficiDC.Sesso.HasValue)
                sesso = datiAnagraficiDC.Sesso;
            else
                sesso = datiAnagraficiTitolare.Sesso;

            int? settimaneAl1292Maternita = null;
            int? settimaneDL50392Maternita = null;

            if (listaMaternitaAcna != null && listaMaternitaAcna.Count > 0)
            {
                foreach (GestioneDatiContributiviCi.PensioniCiMaternitaAcna maternitaAcna in listaMaternitaAcna)
                {
                    if (maternitaAcna.Tipo == 'M')
                    {
                        settimaneAl1292Maternita = maternitaAcna.SettimaneAl1292;
                        settimaneDL50392Maternita = maternitaAcna.SettimaneDL50392;
                    }
                }
            }

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.TipoAppartenenza.CI;

            int annoCompetenza;
            GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);

            bool presenzaConiugato = areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0 && areaTitolare.ElencoStatiCivili.FindIndex(x => x.Codice == 2) > -1;
            bool presenzaConiuge = listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.FindIndex(x => x.IsConiugeOrUnitoCivile() && x.Confermato) > -1;
            bool presenzaTitolare = listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.FindIndex(x => x.IdAnagrafica == datiAnagraficiTitolare.Id && x.Confermato) > -1;
            List<char> listaSigleFamiliari = new List<char> { 'I', 'M', 'S', 'U', 'N', 'J', 'Z', 'W', 'K' };
            bool presenzaOrfano = listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.FindIndex(x => x.Confermato && listaSigleFamiliari.Contains(x.SiglaFamiliare.Value)) > -1;

            DateTime? ultimaDecorrenzaResidenzaItaliana = GestioneControlli.GetUltimaDecorrenzaResidenzaItaliana(datiAnagraficiTitolare.CodiceComuneResidenza, listaResidenzeEstere, codiceConvenzione);

            char? codiceRequisitiLegge50392TraduzioneSuGP = null;
            if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.HasValue && listaCodiceRequisitiLegge50392 != null && listaCodiceRequisitiLegge50392.Count > 0)
            {
                GestioneDecodifica.CodiceRequisitiLegge50392 appCodiceRequisitiLegge50392 = listaCodiceRequisitiLegge50392.Find(x => x.Id == datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.ToString());
                codiceRequisitiLegge50392TraduzioneSuGP = appCodiceRequisitiLegge50392 != null ? appCodiceRequisitiLegge50392.TraduzioneSuGP : null;
            }

            int? settimaneRicalcoloMisura = 0;
            DateTime?[] primaDecorrenzaImportiEsteri = new DateTime?[6];
            int? settimaneEstereWithCodiceArt48 = 0;
            bool set_Rical = false;
            char? codiceArt48PrimoStato = null;
            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                int index = 0;
                foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
                {
                    List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> LImportiEsteri = null;
                    if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                        LImportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestEE.Id);
                    settimaneRicalcoloMisura = GestioneControlli.GetNumeroSettimaneRicalcoloMisura(settimaneRicalcoloMisura, prestEE.CodiceArt48, LImportiEsteri != null && LImportiEsteri.Count > 0 ? LImportiEsteri[0].DecorrenzaPrestazioneEE : null, prestEE.ContributiEERicalcolo, prestEE.ContributiEEDecorrenzaOriginaria, ref set_Rical);
                    primaDecorrenzaImportiEsteri[index] = LImportiEsteri != null && LImportiEsteri.Count > 0 ? LImportiEsteri[0].DecorrenzaPrestazioneEE : null;
                    settimaneEstereWithCodiceArt48 = GestioneControlli.GetNumeroSettimaneEstereWithCodiceArt48(settimaneEstereWithCodiceArt48, prestEE.CodiceArt48, prestEE.ContributiEERicalcolo, prestEE.ContributiEEDecorrenzaOriginaria);
                    if (index == 0)
                        codiceArt48PrimoStato = prestEE.CodiceArt48;
                    index++;
                }
            }

            int?[] numeroSettimaneEstere = null;
            int? sommaSettimaneContributiItalianiEdEsteri = 0;
            bool isDecorrenzaContributiItalianiEdEsteriDuplicata = false;
            int? sommaSettimaneCodiceGestioneX4 = 0;
            bool isCodiceGestione0XPresenteContributiItalianiEdEsteri = false;
            bool isCodiceGestione6XPresenteContributiItalianiEdEsteri = false;
            int?[] sommaGEST_EST_61 = null;
            int sommaSettimaneDecUgualePrimaDec = 0;
            int sommaSettimaneCodGestione1_61CTRItalianiEdEsteri = 0;

            sommaSettimaneCodiceGestioneX4 = settimaneRetributiveQuotaACodGestione4.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault() + settimaneContributiveCodGestione4.GetValueOrDefault();
            if (categoria == 92 || categoria == 93)
                sommaSettimaneCodiceGestioneX4 = sommaSettimaneCodiceGestioneX4.GetValueOrDefault() + (datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : 0);
            if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
            {
                numeroSettimaneEstere = new int?[listaDatiCalcoloContributivoEstero.Count];
                sommaGEST_EST_61 = new int?[listaDatiCalcoloContributivoEstero.Count];
                int indexCalcoloContributivoEstero = 0;
                foreach (GestioneCalcolo.DatiCalcoloContributivoEstero datiContributiEsteri in listaDatiCalcoloContributivoEstero)
                {
                    if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
                    {
                        int index = 0;
                        foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
                        {
                            numeroSettimaneEstere[indexCalcoloContributivoEstero] = GestioneControlli.GetNumeroSettimaneEstereWithDecorrenzaContributiItalianiEdEsteri(numeroSettimaneEstere[indexCalcoloContributivoEstero], datiContributiEsteri.Decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, prestEE.CodiceArt48, prestEE.ContributiEEDecorrenzaOriginaria, primaDecorrenzaImportiEsteri[index], prestEE.ContributiEERicalcolo);
                            sommaGEST_EST_61[indexCalcoloContributivoEstero] = GestioneControlli.GEST_EST_61(sommaGEST_EST_61[indexCalcoloContributivoEstero], datiContributiEsteri.Decorrenza,
                                datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiGenericiCi != null ? datiGenericiCi.DecorrenzaBonus : null,
                                prestEE.CodiceArt48, primaDecorrenzaImportiEsteri, prestEE.ContributiEEDecorrenzaOriginaria, prestEE.ContributiEERicalcolo, index);

                            index++;
                        }
                    }

                    short? codiceGestioneTraduzioneSuGP = 0;
                    if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                    {
                        GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContributiEsteri.CodiceGestione.Value);
                        if (codeGestione != null)
                            codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                    }

                    sommaSettimaneContributiItalianiEdEsteri = sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() + datiContributiEsteri.Settimane.GetValueOrDefault();

                    if (listaDatiCalcoloContributivoEstero.FindAll(x => x.Decorrenza == datiContributiEsteri.Decorrenza).Count > 1)
                        isDecorrenzaContributiItalianiEdEsteriDuplicata = true;

                    sommaSettimaneCodiceGestioneX4 = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteriCodGestioneX4(sommaSettimaneCodiceGestioneX4, codiceGestioneTraduzioneSuGP, datiContributiEsteri.Settimane);

                    if (codiceGestioneTraduzioneSuGP / 10 == 0)
                        isCodiceGestione0XPresenteContributiItalianiEdEsteri = true;
                    if (codiceGestioneTraduzioneSuGP / 10 == 6)
                        isCodiceGestione6XPresenteContributiItalianiEdEsteri = true;

                    if (codiceGestioneTraduzioneSuGP == 1 || codiceGestioneTraduzioneSuGP == 61)
                        sommaSettimaneCodGestione1_61CTRItalianiEdEsteri += datiContributiEsteri.Settimane.GetValueOrDefault();

                    indexCalcoloContributivoEstero++;
                }
            }

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            #endregion GetData

            #region Controlli preliminari
            if (GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.CI.BLOCCOCALCOLO_ESTERO, dataSistema) &&
                GestioneCrossControls.ALL_VerificaBloccoCalcoloEstero(datiAnagraficiTitolare.CodiceComuneResidenza, datiPagamento))
            {
                messaggioVideo = "Invio al calcolo temporaneamente non disponibile per domande con titolare residente all'estero e/o avente modalità di pagamento estera.";
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaBloccoCalcoloAnticipata2019(datiPensione, tipoAppartenenza, dataSistema, out messaggioVideo))
                return false;

            //RINNOVO RIC/TRF
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.ControlloDinamico ctrlValorizzaAnnoCompetenzaPrelievoCI = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoCI", out ctrlValorizzaAnnoCompetenzaPrelievoCI);

            // se è una RIC o TRF e ci troviamo in fase di interregno, isRicRinnovata deve essere true se no scatta il controllo
            if (ctrlValorizzaAnnoCompetenzaPrelievoCI != null && ctrlValorizzaAnnoCompetenzaPrelievoCI.ValoreControllo == "SI")
            {
                if ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno) && !datiPensione.IsRicRinnovata.HasValue)
                {
                    messaggioVideo = "Pensione non rinnovata cancellare e riprelevare la domanda.";
                    return false;
                }
            }
            #endregion Controlli preliminari

            #region Anagrafica

            DateTime? dataValiditaInferiore = null;

            if (!GestioneCrossControls.ALL_VerificaBloccoDecorrenzaPensione(datiPensione, isRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaWithDataMorteTitolare(datiPensione.DecorrenzaOriginaria, datiAnagraficiTitolare.DataMorte, out messaggioVideo))
                return false;

            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
            {
                //Pensioni ai superstiti o sue ricostituzioni
                if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaPerIndirette(datiPensione.DecorrenzaOriginaria, datiAnagraficiTitolare.CodiceFiscale, datiAnagraficiTitolare.DataNascita, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, listaFamiliari, datiPensione, datiDanteCausa, listaCodMaggFamiliari, out messaggioVideo))
                    return false;
            }
            if (!GestioneCrossControls.ALL_VerificaResidenzaEsteroTitolare(datiAnagraficiTitolare.ResidenzaEstero, datiAnagraficiTitolare.CodiceComuneResidenza, datiAnagraficiTitolare.FrazioneResidenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaProvinciaTitolare(datiAnagraficiTitolare.ProvinciaResidenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            bool? isDecorrenzaValida = Utility.ControllaDataDecorrenzaInferiore(datiPensione, Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa), datiPensione.DecorrenzaOriginaria, out dataValiditaInferiore);
            if (!isDecorrenzaValida.HasValue || !isDecorrenzaValida.Value)
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>Decorrenza Pensione antecedente il " +
                    (dataValiditaInferiore.HasValue ? dataValiditaInferiore.Value.Month.ToString() + "/" + dataValiditaInferiore.Value.Year.ToString() : "limite minimo");
                return false;
            }

            if (sindacato != null && Utility.IsSindacatoPresente(sindacato.CodiceSindacato) && !GestioneControlli.VerificaSindacatoAttivo(sindacato, datiPensione.SiglaCategoria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia214(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia135(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensionePerfReqSalvaguardia122(datiPensione, datiPensione.DecorrenzaOriginaria,
                datiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia228(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia124(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneUsuranti(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneEsuberiPA(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia147_2014(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia208_2015(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia178_2020(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlsDecorrenza(datiPensione.NaturaPensione, datiPensione.DataPerfezionamentoRequisiti, datiPensione.SiglaCategoria,
                datiAnagraficiTitolare.DataNascita, datiAnagraficiTitolare.Sesso, datiIstruttoria != null ? datiIstruttoria.Legge44997 : null,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, codiceParticolareSoggettoDerogato != null ? codiceParticolareSoggettoDerogato.TraduzioneSuGp : null, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaSperimentaleDonna(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsPerfezionamentoRequisitiSperimentaleDonna(datiPensione, datiAnagraficiTitolare, datiPensione.DataPerfezionamentoRequisiti, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            //ENG - memo 13 - opzionedonna2023 ddlFigli valorizzabile da view
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiPensione.NumeroFigli, datiAnagraficiTitolare, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePerfRequisitiSperimentaleDonna(datiPensione, tipoAppartenenza, datiPensione.DecorrenzaOriginaria,
                datiPensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensione(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCiviliSuperstiti(datiPensione, listaFamiliari, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsRequisitoEta(datiPensione, tipoAppartenenza, isRiaperturaDomanda, datiPensione, datiAnagraficiTitolare.DataNascita, datiAnagraficiTitolare.Sesso,
                datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiIstruttoria != null ? datiIstruttoria.CodiceParticolareSoggettoDerogato : null, codiceParticolareSoggettoDerogato != null ? codiceParticolareSoggettoDerogato.TraduzioneSuGp : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, null, null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            bool isWarning = false;
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerTipoContributivo(datiPensione, datiPensione, datiAnagraficiTitolare.DataNascita,
                       datiAnagraficiTitolare.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out isWarning, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAPEPrecoce(datiPensione, datiPensione.DecorrenzaOriginaria, datiAnagraficiTitolare, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaPerfezionamentoRequisitiQuota100(datiPensione, datiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneQuota100(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, datiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota100(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePrecoci(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneQuota102(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, datiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota102(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibile(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            //ENG - Memo 123/2024
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibileLeggeDiBilancio2024(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAnticipataFlessibile(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, datiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            //ENG - Memo 123/2024
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAnticipataFlessibileLeggeDiBilancio2024(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, datiPensione.LavoratorePubblico, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Anagrafica

            #region Stati Civili
            if (!GestioneCrossControls.ALL_VerificaStatiCivili(datiPensione.DecorrenzaOriginaria, datiPensione, areaTitolare.ElencoStatiCivili, listaFamiliari, listaAnagraficheFamiliari, listaCodMaggFamiliari,
                datiAnagraficiTitolare.DataNascita, datiAnagraficiDC != null ? datiAnagraficiDC.DataMatrimonio : null, datiAnagraficiTitolare.Sesso, datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : null,
                datiAnagraficiTitolare.CodiceFiscale, dataSistema, out messaggioVideo))
                return false;

            //controlli unioni civili
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCivili(areaTitolare.ElencoStatiCivili, datiPensione, out messaggioVideo))
                return false;
            #endregion Stati Civili

            #region Residenze Estere
            if (listaResidenzeEstere != null && listaResidenzeEstere.Count > 0)
            {
                if (!GestioneCrossControls.CI_VerificaResidenzaWithCodOpzione(datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null, listaResidenzeEstere.First().CodCatastaleStatoEE))
                {
                    messaggioVideo = "Residenza alla Decorrenza Originaria deve essere Italia se Codice Opzione è uguale a 7";
                    return false;
                }

                if (Utility.IsRicostituzione(datiPensione.Gruppo) &&
                    !GestioneCrossControls.ALL_VerificaResidenzeEstereWithAnagrafica(areaTitolare.Anagrafica, listaResidenzeEstere, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
            }
            #endregion Residenze Estere

            #region Dante Causa
            if (datiDanteCausa != null)
            {
                #region AnagraficaDC
                if (datiAnagraficiDC != null)
                {
                    if (!GestioneCrossControls.CI_ControlsAnagraficaDanteCausa(datiAnagraficiDC.CodiceFiscale, datiAnagraficiDC.Cognome, datiAnagraficiDC.Nome, datiAnagraficiDC.DataNascita,
                        datiDanteCausa.DataMorte, datiDanteCausa.DecorrenzaResidenza, datiAnagraficiDC.Cittadinanza, datiPensione.CausaCarico, datiDanteCausa.StatoEEResidenza,
                        datiDanteCausa.DecorrenzaPensione, datiDanteCausa.ParentelaDC, datiAnagraficiDC.Sesso, datiAnagraficiTitolare.Sesso, datiAnagraficiTitolare.CodiceStatoCivile, datiAnagraficiTitolare.CognomeAcquisito,
                        datiAnagraficiDC.DataMatrimonio, datiPensione.DecorrenzaOriginaria, listaFamiliari, listaAnagraficheFamiliari, datiPensione.SiglaCategoria, out messaggioVideo))
                        return false;

                    if (!GestioneCrossControls.AGO_CI_ControlsDecorrenzaResidenzaDanteCausa(datiDanteCausa != null ? datiDanteCausa.StatoEEResidenza : string.Empty, datiDanteCausa != null ? datiDanteCausa.DecorrenzaResidenza : null,
                        datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli incrociati - Dati Dante Causa Anagrafica: " + messaggioVideo;
                        return false;
                    }

                    if (!GestioneCrossControls.ALL_VerificaDataMatrimonioDC(datiPensione, isRiaperturaDomanda, datiAnagraficiDC.DataMatrimonio, listaFamiliari, tipoAppartenenza, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli incrociati - Dati Dante Causa Anagrafica: " + messaggioVideo;
                        return false;
                    }
                }

                if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) &&
                        tipoAppartenenza.HasValue && (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI))
                {
                    if (!GestioneCrossControls.AGO_CI_ControlsProvenienzaPensione(datiDanteCausa.ProvenienzaPensione, out messaggioVideo))
                        return false;
                }
                #endregion AnagraficaDC

                #region Pensione Diretta DC

                if (!(!String.IsNullOrEmpty(datiAnagraficiDC.CodiceFiscale) && datiAnagraficiDC.CodiceFiscale.Contains("DANTEC_") && GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_CI.NESSUN_DANTE_CAUSA)))
                {
                    if (!GestioneCrossControls.CI_ControlsPensioneDirettaDanteCausa(datiDanteCausa.Certificato, datiDanteCausa.SiglaCategoria, datiDanteCausa.Sede, datiDanteCausa.DecorrenzaPensione, datiDanteCausa.Maggiorazione781Contributi,
                        datiDanteCausa.NaturaPensione, datiAnagraficiDC.DataNascita, datiDanteCausa.DataMorte, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione,
                        datiPensione.CausaCarico, out messaggioVideo))
                        return false;

                    if (!GestioneCrossControls.CI_VerificaCodNaturaWithCategoriaDC(datiPensione.NaturaPensione, datiDanteCausa.SiglaCategoria))
                    {
                        messaggioVideo = "Natura pensione 'O' (reg.sperimentale donne) incompatibile con reversibilità da assicurato";
                        return false;
                    }

                    if (!GestioneCrossControls.CI_VerificaCodNaturaTitolareWithDC(datiDanteCausa.NaturaPensione, datiPensione.NaturaPensione))
                    {
                        messaggioVideo = "Natura Pensione errata";
                        return false;
                    }

                    if (!GestioneCrossControls.CI_VerificaCompatibilitaCategoriaDirettaWithCodNatura(datiPensione.NaturaPensione, datiDanteCausa.SiglaCategoria, datiDanteCausa.DecorrenzaPensione))
                    {
                        messaggioVideo = "Categoria Diretta incompatibile con Natura Pensione";
                        return false;
                    }

                    if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                    {
                        if (!GestioneCrossControls.CI_ControlsCodiceVirtualeWithCertificatoDiretta(datiGenericiCi != null ? datiGenericiCi.CodiceVirtuale : null, datiDanteCausa.Certificato, codiceConvenzione, datiPensione.CausaCarico, out messaggioVideo))
                            return false;
                    }
                }

                //valorizzo la variabile importoArt345 che servirà per il controllo CI_VerificaImportoArt345
                decimal? importoArt345 = 0;
                if (datiMaggiorazioniBenefici != null)
                {
                    if (datiMaggiorazioniBenefici.ImportoComplessivoArt3.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt3.Value != 0M)
                        importoArt345 = datiMaggiorazioniBenefici.ImportoComplessivoArt3.Value;
                    else if (datiMaggiorazioniBenefici.ImportoComplessivoArt4.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt4.Value != 0M)
                        importoArt345 = datiMaggiorazioniBenefici.ImportoComplessivoArt4.Value;
                    else if (datiMaggiorazioniBenefici.ImportoComplessivoArt5.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt5.Value != 0M)
                        importoArt345 = datiMaggiorazioniBenefici.ImportoComplessivoArt5.Value;
                    else if (datiMaggiorazioniBenefici.ImportoComplessivoArt1.HasValue && datiMaggiorazioniBenefici.ImportoComplessivoArt1.Value != 0M)
                        importoArt345 = datiMaggiorazioniBenefici.ImportoComplessivoArt1.Value;
                }

                if (LpensioniEstereDcBL != null && LpensioniEstereDcBL.Count > 0)
                {
                    foreach (GestioneDanteCausa.PensioniEstereDcBL pensioneEsteraDC in LpensioniEstereDcBL)
                    {
                        //questi codici non sono contemplati nel controllo. Vengono settati staticamente nella web e corrispondono alla sezione Dati Pensione del Dante Causa 10/2013  Decorrenza "SO" e alla sezione Articolo 6 della tab Pensione CI
                        //Vengono salvati nella stessa tabella e al servizio arriva una lista di 3 record, di cui due contengono il CodiceVari 6 e 10
                        if (pensioneEsteraDC.CodiciVari.HasValue && pensioneEsteraDC.CodiciVari.Value != 6 && pensioneEsteraDC.CodiciVari.Value != 10)
                        {
                            if (!GestioneCrossControls.CI_VerificaCodiceArt4Legge140(datiDanteCausa.DecorrenzaPensione, datiDanteCausa.Maggiorazione781Contributi, pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaCodiceDCPM(datiDanteCausa.DecorrenzaPensione, datiDanteCausa.Maggiorazione781Contributi, pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaCodiceArt41(datiPensione.DecorrenzaOriginaria, datiDanteCausa.Maggiorazione781Contributi, pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaLegge140WithCategoria(datiPensione.SiglaCategoria, pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaImportoArt345(datiPensione.DecorrenzaOriginaria, importoArt345, pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaImportoArt345WithCodiciVari(importoArt345, pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaRangeImportoArt345(importoArt345, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaCodiciVariWithDecorrenzaPensione(datiPensione.DecorrenzaOriginaria, pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.CI_VerificaCodiciVariWithEccedenzaArt5(pensioneEsteraDC.CodiciVari, datiDanteCausa.EccedenzaArt5, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }
                        }
                    }
                }

                if (!GestioneCrossControls.CI_VerificaDecorrenzaPensioneWithEccedenzaArt5(datiPensione.DecorrenzaOriginaria, datiDanteCausa.EccedenzaArt5, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                /////// Per maggiori informazioni sul commento fare riferimento al documento L1-PCIPL29.docx sotto tfs alla cartella Documentazione\ControlliCI
                //if (!GestioneCrossControls.CI_VerificaSiglaCategoria(datiDanteCausa.SiglaCategoria, out messaggioVideo))
                //{
                //    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                //    return false;
                //}

                /////// Per maggiori informazioni sul commento fare riferimento al documento L1-PCIPL29.docx sotto tfs alla cartella Documentazione\ControlliCI
                //if (!GestioneCrossControls.CI_VerificaDecorrenzaDiretta(datiDanteCausa.DecorrenzaPensione, out messaggioVideo))
                //{
                //    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                //    return false;
                //}

                if (!GestioneCrossControls.CI_VerificaRequisitoParticolareDirittoWithDanteCausa(datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, categoria, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.SiglaCategoria : null, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                #endregion Pensione Diretta DC

                #region Altra Pensione DC

                if (!GestioneCrossControls.CI_ControlsAltraPensioneWithPensioneDiretta(datiDanteCausa.CategoriaAltraPensione, datiDanteCausa.NaturaPensione, datiDanteCausa.SiglaCategoria, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_ControlsAltraPensioneDanteCausa(datiDanteCausa.CodiceUCAltraPensione, datiDanteCausa.DecorrenzaAltraPensione, datiAnagraficiDC.DataNascita, datiDanteCausa.DataMorte, datiDanteCausa.CessazioneAltraPensione, datiDanteCausa.CategoriaAltraPensione, out messaggioVideo))
                    return false;

                #endregion Altra Pensione DC

                #region Pensione Estere DC

                if (LpensioniEstereDcBL != null && LpensioniEstereDcBL.Count > 0)
                {
                    foreach (GestioneDanteCausa.PensioniEstereDcBL pensioneEsteraDC in LpensioniEstereDcBL)
                    {
                        //questi codici non sono contemplati nel controllo. Vengono settati staticamente nella web e corrispondono alla sezione Dati Pensione del Dante Causa 10/2013  Decorrenza "SO" e alla sezione Articolo 6 della tab Pensione CI
                        //Vengono salvati nella stessa tabella e al servizio arriva una lista di 3 record, di cui due contengono il CodiceVari 6 e 10
                        if (pensioneEsteraDC.CodiciVari.HasValue && pensioneEsteraDC.CodiciVari.Value != 6 && pensioneEsteraDC.CodiciVari.Value != 10)
                        {
                            if (!GestioneCrossControls.CI_VerificaCodiciVari(pensioneEsteraDC.CodiciVari, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                                return false;
                            }
                        }
                    }
                }

                #endregion Pensione Estere DC

                #region Pensione CI

                if (!GestioneCrossControls.CI_VerificaLegge5991WithDecorrenzaPensione(datiPensione.DecorrenzaOriginaria, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.AumentoMensileLegge5991Comma9 : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaLegge5991WithDecorrenze(datiPensione.DecorrenzaOriginaria, datiDanteCausa.DecorrenzaPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.AumentoMensileLegge5991Comma9 : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSentenza7290Art2WithCategoria(datiPensione.SiglaCategoria, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.Aumento7290 : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.AumentoMensileLegge161289Art2 : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSentenza7290WithRms8888(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.Aumento7290 : null, datiGenericiCi != null ? datiGenericiCi.RMS8888 : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaAumentoLeggeArt2WithRms9090(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.AumentoMensileLegge161289Art2 : null, datiGenericiCi != null ? datiGenericiCi.RMS9090 : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }

                #endregion Pensione CI

                #region Sentenza 495/93
                if (listaRedditoSentenza495_93 != null && listaRedditoSentenza495_93.Count > 0)
                {
                    if (!GestioneCrossControls.AGO_CI_ControlsRedditiSentenza495_93(listaRedditoSentenza495_93, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.ProvenienzaPensione : null, datiPensione, listaFamiliari, tipoAppartenenza, datiDanteCausa, Utility.IsRiaperturaDomanda(datiPensione.Id), out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                        return false;
                    }
                }
                #endregion Sentenza 495/93
            }
            #endregion Dante Causa

            #region Familiari
            if (!GestioneCrossControls.ALL_ControlsFamiliariWithStatiCivili(listaFamiliari, listaCodMaggFamiliari, areaTitolare.ElencoStatiCivili, tipoAppartenenza, datiAnagraficiTitolare.DataMorte,
                out messaggioVideo))
                return false;

            #region PCIPL11
            if (tipoDomanda == Utility.TipoDomanda.Superstiti ||
                (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0 && areaTitolare.ElencoStatiCivili.FindIndex(x => x.Codice == '2') > -1) ||
                (listaFamiliari != null && listaFamiliari.Count > 0))
            {
                if (!GestioneCrossControls.CI_VerificaReversObbligatorietaFamiliare(listaFamiliari != null && listaFamiliari.Count > 0, tipoDomanda, out messaggioVideo))
                    return false;

                DateTime? appDecorrenzaCarico = DateTime.MinValue;
                DateTime? appCessazioneCarico = DateTime.MinValue;
                DateTime? decorrenzaCaricoCompare = DateTime.MinValue;
                DateTime? cessazioneCaricoCompare = DateTime.MinValue;

                if (listaFamiliari != null && listaFamiliari.Count > 0)
                {
                    int indexFam = 0;

                    foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                    {
                        if (fam.Confermato)
                        {
                            GestioneAnagrafica.DatiAnagrafici datiAnagraficiFamiliare = null;
                            GestioneAnagrafica.GetAnagraficaByIdAnagrafica(fam.IdAnagrafica, out datiAnagraficiFamiliare);

                            List<GestioneFamiliari.CodMaggFamiliari> LcodMaggFam = listaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica);

                            int index = 0;
                            foreach (GestioneFamiliari.CodMaggFamiliari codice in LcodMaggFam)
                            {
                                if (codice.CodiceMaggiorazione > 2)
                                {
                                    messaggioVideo = "Codice Maggiorazione errato per il familiare con CF " + fam.CodiceFiscale + " ('SI' / 'NO' / '  ')";
                                    return false;
                                }

                                if (!GestioneCrossControls.CI_VerificaNoReversCodeMaggiorazioneConiuge(fam.SiglaFamiliare, datiPensione.Gruppo, datiPensione.Prodotto, codice.CodiceMaggiorazione))
                                {
                                    messaggioVideo = "Codice Maggiorazione errato o mancante ('SI' / 'NO')";
                                    return false;
                                }

                                if (!GestioneCrossControls.CI_VerificaNoReversCodeMaggiorazioneNoConiuge(fam.SiglaFamiliare, datiPensione.Gruppo, datiPensione.Prodotto, codice.CodiceMaggiorazione, datiPensione))
                                {
                                    messaggioVideo = "Codice Maggiorazione non deve essere acquisito";
                                    return false;
                                }

                                if (!GestioneCrossControls.CI_VerificaReversCodeMaggiorazioneNoConiuge(fam.SiglaFamiliare, datiPensione.Gruppo, datiPensione.Prodotto, codice.CodiceMaggiorazione))
                                {
                                    messaggioVideo = "Codice Maggiorazione errato o mancante (SI / NO)";
                                    return false;
                                }

                                if (!GestioneCrossControls.CI_VerificaCodiceMaggiorazioneWithPeriodo(tipoDomanda, fam, codice.CodiceMaggiorazione, codice.Decorrenza, codice.Cessazione, ref appDecorrenzaCarico, ref appCessazioneCarico,
                                    ref decorrenzaCaricoCompare, ref cessazioneCaricoCompare, out messaggioVideo))
                                    return false;

                                if (index == 0)
                                {
                                    if (!GestioneCrossControls.CI_VerificaDateFamiliari(tipoDomanda, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAnagraficiFamiliare.DataNascita, codice.Decorrenza, datiPensione.DecorrenzaOriginaria, fam.SiglaFamiliare, out messaggioVideo))
                                        return false;
                                }

                                if (!GestioneCrossControls.CI_VerificaCessazioneCarico(fam.SiglaFamiliare, datiAnagraficiFamiliare.DataNascita, annoCompetenza, datiPensione.CausaCarico, codice.Cessazione, codice.Decorrenza, out messaggioVideo))
                                    return false;

                                if (!GestioneCrossControls.CI_VerificaSiglaFamiliareWithDC(tipoDomanda, codice.Cessazione, datiAnagraficiTitolare.CodiceFiscale, datiAnagraficiFamiliare.CodiceFiscale, fam,
                                    datiDanteCausa != null ? datiDanteCausa.ParentelaDC : null, ultimoStatoCivile != null ? ultimoStatoCivile.Codice : '0', annoCompetenza, codice.CodiceMaggiorazione, out messaggioVideo))
                                    return false;

                                index++;
                            }

                            if (!GestioneCrossControls.CI_VerificaCognomeAcquisitoWithSesso(datiAnagraficiFamiliare.CognomeAcquisito, datiAnagraficiFamiliare.Sesso, listaCodMaggFamiliari, out messaggioVideo))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaSiglaFamiliareWithDataNascita(fam, datiAnagraficiFamiliare.DataNascita, out messaggioVideo))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaScadenzaRevisioneSanitariaWithDatiGenerici(fam.ScadenzaRevisioneSanitaria, fam.SiglaFamiliare, datiPensione.CausaCarico, out messaggioVideo))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaScadenzaRevisioneSanitaria(tipoDomanda, datiPensione.DecorrenzaOriginaria, datiAnagraficiFamiliare.DataNascita,
                                indexFam, LcodMaggFam, fam, LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam[0].CodiceMaggiorazione : (byte?)null,
                                out messaggioVideo))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaSiglaFamiliare(fam, tipoDomanda, categoria, LcodMaggFam, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null,
                                datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null, datiIstruttoria != null ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi : null,
                                datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null,
                                datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, codiceConvenzione, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria,
                                codicePrimoStatoEE, datiDanteCausa != null ? datiDanteCausa.SiglaCategoria : string.Empty, out messaggioVideo))
                                return false;

                            indexFam++;
                        }
                    }
                }

                if (!GestioneCrossControls.CI_VerificaCoerenzaPeriodi(listaFamiliari, listaCodMaggFamiliari, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_VerificaDataMatrimonioDC(tipoDomanda, datiPensione.DecorrenzaOriginaria, presenzaConiugato, presenzaConiuge, datiAnagraficiDC != null ? datiAnagraficiDC.DataMatrimonio : null, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_VerificaObbligatorietaContitolare(tipoDomanda, datiEliminazione != null ? datiEliminazione.CodiceMotivo : null, presenzaTitolare,
                    ultimoStatoCivile != null ? ultimoStatoCivile.Codice : '0', presenzaConiuge, categoria, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_VerificaSiglaFamiliareAscendente(listaFamiliari, out messaggioVideo))
                    return false;
            }
            #endregion PCIPL11

            //Per ciascun familiare presente verificare che il codice fiscale del Titolare  sia diverso dal codice fiscale del familiare 
            if (!GestioneCrossControls.ALL_VerificaFamiliariTitolare(listaFamiliari, areaTitolare, datiPensione, tipoAppartenenza, isRiaperturaDomanda, datiDanteCausa))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>Il titolare pensione non può essere presente nell'elenco dei familiari.";
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaFamiliariMorti(listaFamiliari, listaCodMaggFamiliari, datiPensione.DecorrenzaOriginaria, tipoAppartenenza, out messaggioVideo, datiPensione, datiEliminazione))
                return false;

            if (!GestioneCrossControls.ALL_VerificaCessazioneCodMagg(listaFamiliari, listaCodMaggFamiliari, dataSistema, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    List<GestioneFamiliari.CodMaggFamiliari> LcodMaggFam = listaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica);
                    if (LcodMaggFam != null && LcodMaggFam.Count > 0 &&
                        LcodMaggFam.Exists(x => x.Decorrenza.HasValue && x.Cessazione.HasValue && !Utility.DataStrettamenteSuccessivaA(x.Cessazione.Value, x.Decorrenza.Value)))
                    {
                        messaggioVideo = "Per il familiare " + fam.CodiceFiscale + " la data fine carico non può essere inferiore alla data decorrenza carico";
                        return false;
                    }
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaCarico(listaFamiliari, listaCodMaggFamiliari, datiPensione, datiDanteCausa, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaFamiliariConiugiTitolareConiugato(datiPensione, areaTitolare, listaFamiliari, true, datiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaSovrapposizioneCodMaggFamiliariConiugi(listaFamiliari, listaCodMaggFamiliari, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_VerificaFamiliariConiugiRicostituzioneOrRiapertura(datiPensione, datiEliminazione, listaFamiliari, listaCodMaggFamiliari, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) &&
                !GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggFamiliariNipoti(listaFamiliari, listaCodMaggFamiliari, dataSistema))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari: Non è possibile inserire per i nipoti una data fine carico successiva a Gennaio " + (dataSistema.Year + 1).ToString();
                return false;
            }
            if (!GestioneCrossControls.CI_ControlsQuotaContitolaritaNipote(datiPensione, tipoAppartenenza, listaFamiliari, listaCodMaggFamiliari, annoCompetenza, out messaggioVideo))
                return false;

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ConsultazioneANFAttivaCI", out ctrl);
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (!isConsultazioniANFVerificate)
                {
                    if (!ControlsConsultazioneANF(datiPensione, listaFamiliari, listaCodMaggFamiliari, dataSistema, matricolaOperatore, out listaConsultazioniANF, out messaggioVideo))
                        return false;
                    if (listaConsultazioniANF != null && listaConsultazioniANF.Count > 0)
                        return true;
                }
            }

            if (!ControlsDatiFamiliari(datiPensione, dataSistema, annoCompetenza, isRiaperturaDomanda, listaFamiliari, tipoAppartenenza, listaCodMaggFamiliari, out messaggioVideo))
            {
                if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    GestioneFamiliari.SbloccaFamiliari(datiPensione, listaFamiliari);
                return false;
            }


            #endregion Familiari

            #region LiquidazionePensione

            string ufficioPagatoreArretratiEE = datiGenericiCi != null ? Utility.GetUfficioPagatoreFromId(datiGenericiCi.UfficioPagatoreArretratiEE) : string.Empty;
            if (!GestioneLiquidazionePensione.ControlsUfficioPagatoreArretratiEsteri(ufficioPagatoreArretratiEE, listaPrestazioniEstere, datiPensione.CodiceArretrati,
                datiGenericiCi != null ? datiGenericiCi.CodiceBloccoArretratiEE : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Assicurativi:<br/>" + messaggioVideo;
                return false;
            }

            if (datiPensione.DataInizioCalcolo.HasValue)
            {
                if (datiPensione.DataInizioCalcolo.Value.CompareTo(datiPensione.DecorrenzaOriginaria) < 0)
                {
                    messaggioVideo = "Decorrenza Calcolo anteriore a Decorrenza Originaria";
                    return false;
                }

                if (datiPensione.DataInizioCalcolo.Value.CompareTo(dataSistema.AddMonths(1).AddDays(-dataSistema.Day + 1)) > 0)
                {
                    messaggioVideo = "Decorrenza Calcolo posteriore a data del giorno";
                    return false;
                }

                if (datiPensione.DataInizioCalcolo.Value.CompareTo(datiPensione.DecorrenzaOriginaria) != 0 && datiPensione.CausaCarico != 3 && datiPensione.CausaCarico != 9 && datiPensione.CausaCarico != 2)
                {
                    messaggioVideo = "Se prima liquidata: Decorrenza Calcolo deve essere uguale a Decorrenza Pensione";
                    return false;
                }
            }

            if (datiIstruttoria != null && codiceParticolareSoggettoDerogato != null)
            {
                if (datiPensione.NaturaPensione.Substring(2, 1) == "Z" && datiPensione.DataPresentazioneDomanda.CompareTo(new DateTime(2001, 08, 16)) > 0 &&
                   datiPensione.CausaCarico == 1 && codiceParticolareSoggettoDerogato.TraduzioneSuGp != '3')
                {
                    messaggioVideo = "3° codice Natura Pensione ('Z') incompatibile con Data Domanda";
                    return false;
                }
            }

            if (datiGenericiCi != null)
            {
                if (datiGenericiCi.DecorrenzaBonus.HasValue)
                {
                    if (datiGenericiCi.DecorrenzaBonus.Value.CompareTo(new DateTime(2001, 03, 01)) < 0)
                    {
                        messaggioVideo = "Decorrenza Bonus illogica";
                        return false;
                    }

                    if (datiPensione.NaturaPensione.Substring(1, 1) != "X" && datiPensione.NaturaPensione.Substring(1, 1) != "Y")
                    {
                        messaggioVideo = "Decorrenza Bonus incompatibile con natura pensione";
                        return false;
                    }

                    if (datiIstruttoria != null && codiceParticolareSoggettoDerogato != null)
                    {
                        if (datiPensione.CausaCarico != 2 && codiceParticolareSoggettoDerogato.TraduzioneSuGp > 3)
                        {
                            messaggioVideo = "Codice Soggetto Derogato errato";
                            return false;
                        }
                    }
                }
            }

            if (!GestioneControlli.VerificaDecorrenzaArretratiWithDataPresentazione(datiPensione.DecorrenzaCalcoloArretrati, datiPensione.CausaCarico, datiPensione.DataPresentazioneDomanda))
            {
                messaggioVideo = "Decorrenza Arretrati incompatibile con la Data Domanda";
                return false;
            }

            if (!GestioneControlli.ControlsCodNaturaForDatiGenerici(datiPensione, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.SiglaCategoria, datiPensione.CodiceArretrati, datiAnagraficiTitolare.CodiceComuneResidenza, datiPensione.CausaCarico, datiPensione.DataPresentazioneDomanda, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaOriginariaWithCodNaturaAndDataPresentazione(datiPensione, datiPensione.CausaCarico, datiPensione.NaturaPensione, datiPensione.AttivitaEconomica,
                datiPensione.ProfessioneIndividuale, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDelibera12688WithCodNatura(datiGenericiCi != null ? datiGenericiCi.DeliberaCee126 : null, datiPensione.NaturaPensione, datiPensione.Gruppo))
            {
                messaggioVideo = "Delibera 126/88 incompatibile con Natura Pensione";
                return false;
            }

            if (!GestioneControlli.VerificaObbligatorietaAttivitaEconomicaWithCausaCarico(datiPensione.CausaCarico, datiPensione.AttivitaEconomica))
            {
                messaggioVideo = "Codice Attività Economica mancante";
                return false;
            }

            if (!GestioneControlli.VerificaObbligatorietaProfessioneIndividualeWithCausaCarico(datiPensione.CausaCarico, datiPensione.ProfessioneIndividuale))
            {
                messaggioVideo = "Codice Professione Individuale mancante";
                return false;
            }

            if (!GestioneControlli.ControlsNaturaPensioneWithEtaPensionabile(datiPensione.Gruppo, datiPensione.SiglaCategoria, datiAnagraficiTitolare.Sesso, datiAnagraficiTitolare.DataNascita,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, codicePrimoStatoEE, codiceConvenzione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceConvenzioneWithStatoEstero(datiPensione, listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE : string.Empty, codiceConvenzione, datiPensione.Gruppo))
            {
                messaggioVideo = "Codice Convenzione errato o incompatibile con Stato " + nomeStato;
                return false;
            }

            string maxDate = GestioneControlli.VerificaDecorrenzaCodiceConvenzioneWithStatoEstero(datiPensione, decorrenza, listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE : string.Empty, codiceConvenzione);
            if (maxDate != null)
            {
                messaggioVideo = "Nel caso di codice convenzione " + codiceConvenzione + " la decorrenza non può essere precedente a " + maxDate;
                return false;
            }


            if (!GestioneControlli.VerificaAnniDifferimento(datiGenericiCi != null ? datiGenericiCi.AnniDifferimento : null, datiPensione.Gruppo))
            {
                messaggioVideo = "Anni di differimento incompatibili con la categoria della pensione";
                return false;
            }

            if (!GestioneControlli.VerificaAnniDifferimentoWithVOS(datiGenericiCi != null ? datiGenericiCi.AnniDifferimento : null, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Anni di differimento incompatibile con categoria VOS post 08/1976";
                return false;
            }

            if (!GestioneControlli.ControlsAnniDifferimentoWithEtaPensionabile(datiGenericiCi != null ? datiGenericiCi.AnniDifferimento : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiPensione.SiglaCategoria, datiAnagraficiTitolare.Sesso, datiAnagraficiTitolare.DataNascita, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null, codicePrimoStatoEE, codiceConvenzione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaPresenzaTrattenutaINPDAP(datiPagamento.TrattenutaInpdap, datiPagamento.DataRinunciaTrattenutaInpdap, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaTrattenutaINPDAPWithCausaCarico(datiPagamento.TrattenutaInpdap, datiPagamento.DataRinunciaTrattenutaInpdap, datiPensione.CausaCarico, datiPensione))
            {
                messaggioVideo = "Codice trattenuta Fondo Credito errato (SI/SPAZIO)";
                return false;
            }

            if (!GestioneControlli.VerificaCoerenzaTrattenutaINPDAP(datiPagamento.TrattenutaInpdap, datiPagamento.DataRinunciaTrattenutaInpdap))
            {
                messaggioVideo = "Trattenuta Fondo Credito: Decorrenza incompatibile con codice";
                return false;
            }

            if (!GestioneControlli.VerificaTrattenutaINPDAPWithCategoria(datiPagamento.TrattenutaInpdap, datiPensione.Gruppo, datiPensione))
            {
                messaggioVideo = "Trattenuta Fondo Credito incompatibile con Categoria Pensione";
                return false;
            }

            if (!GestioneControlli.VerificaTrattenutaINPDAPWithDecorrenzaPensione(datiPagamento.TrattenutaInpdap, datiPagamento.DataRinunciaTrattenutaInpdap, datiPensione.DecorrenzaOriginaria, datiPensione, datiStoricoGP != null ? datiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaTrattenutaINPDAP(datiPensione, datiPagamento.TrattenutaInpdap, datiPagamento.DataRinunciaTrattenutaInpdap,
                datiStoricoGP != null ? datiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsRequisitoRidotto(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceContrattoEquiparato(decorrenza, datiPensione.Gruppo, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.CodiceContrattoEquiparato : null, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceLivelloEquiparato(decorrenza, datiPensione.Gruppo, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.CodiceLivelloEquip : null, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceMobilita(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.CodiceMobilita : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceMobilitaWithRequisitoRidotto(decorrenza, datiPensione.Gruppo, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.CodiceMobilita : null, datiPensione.SiglaCategoria, datiIstruttoria != null ? datiIstruttoria.Legge44997 : null))
            {
                messaggioVideo = "Codice Mobilità incompatibile con il Requisito Ridotto";
                return false;
            }

            if (!GestioneControlli.VerificaEsenzioneFiscaleTerrorismo(datiIstruttoria != null ? datiIstruttoria.CodiceComunicazioneCampo4 : null, datiDetrazioni != null ? datiDetrazioni.DetrazioniReddito : null))
            {
                messaggioVideo = "Esenzione fiscale 'Vittime Terrorismo' deve essere 'NO'";
                return false;
            }

            if (!GestioneControlli.ControlsEsenzioneFiscaleEstero(datiIstruttoria != null ? datiIstruttoria.CodiceComunicazioneCampo4 : null, datiDetrazioni != null ? datiDetrazioni.DetrazioniReddito : null, datiAnagraficiTitolare.ProvinciaResidenza, datiAnagraficiTitolare.CodiceComuneResidenza, out messaggioVideo))
                return false;

            if (!datiPensione.CodiceArretrati.HasValue)
            {
                messaggioVideo = "Codice Arretrati errato o mancante (1 / 8)";
                return false;
            }

            if (!GestioneControlli.ControlsCodNaturaCrossTab(datiPensione.NaturaPensione, datiPensione.Gruppo, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.DecorrenzaOriginaria, datiPensione.CausaCarico, datiPensione.CodiceTipoRichiesta, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceOpzioneRiliquidazione(datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null, datiAnagraficiTitolare.Cittadinanza,
                listaResidenzeEstere, listaPrestazioniEstere, datiPensione.Gruppo, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.Legge44997 : null,
                datiPensione.DecorrenzaOriginaria, datiAnagraficiTitolare.DataNascita, datiAnagraficiTitolare.Sesso, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaOpzione(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null))
            {
                messaggioVideo = "Decorrenza Opzione illogica";
                return false;
            }

            bool isRocOrRevCI = (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || Utility.IsRicostituzione(datiPensione.Gruppo)) && tipoAppartenenza == Utility.TipoAppartenenza.CI;
            if (!GestioneControlli.ControlsDecorrenzaOpzione(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, codiceConvenzione, listaPrestazioniEstere[0].CodiceStatoEE, datiPensione.SiglaCategoria, datiPensione.NaturaPensione, isRocOrRevCI, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDecorrenzaDPCM(datiGenericiCi != null ? datiGenericiCi.DecorrenzaArt2Dpcm : null, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsImportoCristallizzazione(datiGenericiCi != null ? datiGenericiCi.ImportoCristallizzazione3481 : null, datiPensione.CausaCarico, datiPensione.SiglaCategoria, datiGenericiCi != null ? datiGenericiCi.CodiceVirtuale : null, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.CI_VerificaDecorrenzaArt2DPCMWithDanteCausa(datiGenericiCi != null ? datiGenericiCi.DecorrenzaArt2Dpcm : null, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Decorrenza D.P.C.M. (Liquidazione Pensione/Dati Opzione) incompatibile con Decorrenza della Pensione";
                return false;
            }

            if (datiDanteCausa != null)
            {
                if (!GestioneControlli.ControlsDecorrenzaOpzioneWithDanteCausa(decorrenza, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiDanteCausa.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria, datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, datiDanteCausa.SiglaCategoria, datiPensione.SiglaCategoria, datiPensione.NaturaPensione, codiceConvenzione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRequisitoRidottoWithDanteCausa(decorrenza, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiPensione.SiglaCategoria, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCodiceMobilitaWithDanteCausa(decorrenza, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.CodiceMobilita : null, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_VerificaAnniDifferimentoWithDanteCausa(datiGenericiCi.AnniDifferimento, datiDanteCausa.SiglaCategoria, datiDanteCausa.DecorrenzaPensione))
                {
                    messaggioVideo = "Anni Differimento incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaImportoIVS(datiPensione.SiglaCategoria, datiGenericiCi != null ? datiGenericiCi.ImportoIVS : null, datiDanteCausa.Certificato, datiDanteCausa.DataMorte, datiDanteCausa.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                    return false;
            }

            #region PCIPL39 Categoria >= 7
            if (categoria >= 7)
            {
                if (!GestioneControlli.VerificaSettimaneItalianeDiritto(settimane, codiceConvenzione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneItalianeWithCodiceConvenzione(datiPensione.Gruppo, codiceConvenzione, decorrenza, datiPensione.DecorrenzaOriginaria,
                    datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null,
                    settimane.Value, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, ctrMinimi, datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null,
                    datiAnagraficiTitolare != null ? datiAnagraficiTitolare.CodiceComuneResidenza : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneItaliane(codiceConvenzione, datiPensione.NaturaPensione, settimane, settimaneItalianeMisura, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneEffettive(datiGenericiCi != null ? datiGenericiCi.NContributiItalia : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
                {
                    foreach (GestioneCalcolo.DatiCalcoloContributivo datiCalcolo in listaDatiCalcoloContributivo)
                    {
                        if (datiCalcolo.Montante.HasValue && datiCalcolo.MontanteQuotaDL214.HasValue)
                            montante += datiCalcolo.Montante.Value + datiCalcolo.MontanteQuotaDL214.Value;
                    }
                }

                if (!GestioneControlli.ControlsSettimaneFittizieWithEtaPensionabileAndDecorrenza(datiPensione.Gruppo, datiPensione.NaturaPensione, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione.DecorrenzaOriginaria, decorrenza,
                    datiAnagraficiTitolare != null ? datiAnagraficiTitolare.DataNascita : null, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, montante, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null,
                    datiPensione, sommaSettimaneDirittoEstere, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaImportoIVS(datiGenericiCi != null ? datiGenericiCi.ImportoIVS : null, decorrenza, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 1, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 2, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione3, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 3, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione4, settimaneRetributiveQuotaBCodGestione4, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 4, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneAndImportoAndMontanteQuotaD(settimaneContributiveDL214CodGestione2, importoContributivoTotaleQuotaDCodGestione2, montanteContributivoQuotaDCodGestione2, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneAndImportoAndMontanteQuotaD(settimaneContributiveDL214CodGestione3, importoContributivoTotaleQuotaDCodGestione3, montanteContributivoQuotaDCodGestione3, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneAndImportoAndMontanteQuotaD(settimaneContributiveDL214CodGestione4, importoContributivoTotaleQuotaDCodGestione4, montanteContributivoQuotaDCodGestione4, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;

                //ENG - memo 28_2024 saltare controllo per le 0001 0001 0017
                GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
                if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                {
                    if (!((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") &&
                    Utility.IsDomandaTipoContributivo(datiPensione, true, false)))
                    {
                        if (!GestioneControlli.VerificaSettimaneWithNaturaPensione(datiPensione.Gruppo, sommaSettimaneDirittoEstere, (datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto.GetValueOrDefault() : 0), (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0), datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione.NaturaPensione, out messaggioVideo))
                            return false;
                    }
                }
                else
                {
                    if (!GestioneControlli.VerificaSettimaneWithNaturaPensione(datiPensione.Gruppo, sommaSettimaneDirittoEstere, (datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto.GetValueOrDefault() : 0), (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0), datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione.NaturaPensione, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.VerificaCmsmWithSettimaneFittizie(datiGenericiCi != null ? datiGenericiCi.CMSM : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCmsmWithSettimaneFittizieAndImportiContribTot(datiGenericiCi != null ? datiGenericiCi.CMSM : null, decorrenza, datiGenericiCi.NSettFittiziePrepensionamento, datiPensione.FineAssicurazione, importoContributivoTotaleCodGestione1,
                    importoContributivoTotaleCodGestione2, importoContributivoTotaleCodGestione3, importoContributivoTotaleCodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettWithCodReqPartAndNaturaPensione(datiGenericiCi != null ? datiGenericiCi.DeliberaCee126 : null,
                    datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, settimane, datiPensione.NaturaPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettGodimentoAssegnoWithCodReqParticolari(datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, datiPensione.Gruppo, tipoDomanda, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettimaneWithCodReqParticolari(datiPensione.Gruppo, sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, settimane, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettimaneWithCodReqParticolariAndTipoDomanda(tipoDomanda, sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, settimane, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMaggiori6(datiGenericiCi != null ? datiGenericiCi.NContributiItalia : null, settimane,
                    datiPensione.DataInizioCalcolo, tipoDomanda, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }
            }
            #endregion PCIPL39 Categoria >= 7

            if (!GestioneControlli.VerificaDecorrenzaOpzioneWithCodiceStato(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, codicePrimoStatoEE, codiceConvenzione, settimane,
                out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaSettimaneFittizieWithCodNatura(datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaSettinaneEffettiveNSettimaneOBG(datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null, datiGenericiCi != null ? datiGenericiCi.NContributiItalia : null))
            {
                messaggioVideo = "Settimane Effettive mancanti.";
                return false;
            }

            #region Categorie minori o uguali a 6
            if (categoria > 0 && categoria <= 6)
            {
                if (!GestioneControlli.ControlsFineAssicurazione(datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsInizioAssicurazione(datiPensione.InizioAssicurazione, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, datiAnagraficiTitolare.DataNascita, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsDecorrenzaBonusWithFineAssicurazione(datiPensione.FineAssicurazione, datiGenericiCi != null ? datiGenericiCi.DecorrenzaBonus : null, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCodNaturaWithEtaPensionabile(datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiAnagraficiTitolare.DataNascita, datiAnagraficiTitolare.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaSettimaneOBG(settimane.GetValueOrDefault() + (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0),
                    codiceConvenzione, codicePrimoStatoEE, settimaneRetributiveQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, datiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi - " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneOBG(settimaneRetributiveQuotaACodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, codicePrimoStatoEE, settimane,
                    rmsQuotaACodGestione1, datiPensione.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaSettimaneVV(codiceConvenzione, settimaneRetributiveQuotaACodGestione1, settimane, datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null,
                    datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, integrazioneArt11 != null ? integrazioneArt11.ImportoIVS : null,
                    datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneVV(datiPensione, listaDatiCalcoloContributivo, datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, decorrenza, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, integrazioneArt11 != null ? integrazioneArt11.ImportoIVS : null, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, settimaneContributiveCodGestione1, rmsQuotaACodGestione1, true, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportoIVSPost1976(datiGenericiCi != null ? datiGenericiCi.ImportoIVS : null, categoria, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, decorrenza, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi - " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimanePerCalcoloContributivoWithImportoIVS(datiGenericiCi != null ? datiGenericiCi.SettimanePerCalcoloContributivo : null, datiGenericiCi != null ? datiGenericiCi.ImportoIVS : null, decorrenza, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi - " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneWithMinCTRAndConvenzione(codiceConvenzione, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria,
                    decorrenza, codicePrimoStatoEE, settimane, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null,
                    datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null, datiPensione.Gruppo, datiPensione.NaturaPensione, settimaneRetributiveQuotaACodGestione1,
                    settimaneRetributiveQuotaBCodGestione1, settimaneContributiveCodGestione1, settimaneContributiveDL214CodGestione1, datiAnagraficiTitolare.CodiceComuneResidenza,
                    (listaDatiSupplementi != null && listaDatiSupplementi.Count > 0) ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, datiDanteCausa != null ? datiDanteCausa.SiglaCategoria : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaCapienzaSettimaneWithAssicurazione(datiPensione, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimane, datiPensione.ProfessioneIndividuale,
                    datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaImportoIVS(categoria, rmsQuotaACodGestione1, datiGenericiCi != null ? datiGenericiCi.ImportoIVS : null, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaObbligatorietaImportoIVSWithDecorrenze(datiGenericiCi != null ? datiGenericiCi.ImportoIVS : null, decorrenza, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaRMS8888WithRMSQuotaA(datiGenericiCi != null ? datiGenericiCi.RMS8888 : null, rmsQuotaACodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS8888WithDecorrenza(datiGenericiCi != null ? datiGenericiCi.RMS8888 : null, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS9090WithRMSQuotaA(datiGenericiCi != null ? datiGenericiCi.RMS9090 : null, rmsQuotaACodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS9090WithDecorrenze(datiGenericiCi != null ? datiGenericiCi.RMS9090 : null, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, decorrenza, datiGenericiCi != null ? datiGenericiCi.DecorrenzaArt2Dpcm : null, (listaDatiSupplementi != null && listaDatiSupplementi.Count > 0) ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCapienzaSettimaneVV(datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsSettimaneFittizie(datiPensione.NaturaPensione, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione.Gruppo, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, datiAnagraficiTitolare.DataNascita, datiGenericiCi != null ? datiGenericiCi.CMSM : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null,
                    settimaneRetributiveQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, sommaSettimaneDirittoEstere, decorrenza, rmsQuotaBCodGestione1, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null, datiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Assicurativi: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaImportoIVSArt11(integrazioneArt11 != null ? integrazioneArt11.ImportoIVS : null, rmsQuotaACodGestione1, decorrenza, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, settimaneRetributiveQuotaACodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettWithCodReqPartAndNaturaPensione(datiGenericiCi != null ? datiGenericiCi.DeliberaCee126 : null,
                    datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, settimane, datiPensione.NaturaPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettimaneWithDecPensioneAndCodRequisitiParticolari(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.SiglaCategoria, settimane,
                    datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null,
                        sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettGodimentoAssegnoAndCodReqParticolari(tipoDomanda, datiPensione.Gruppo, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, datiPensione, out messaggioVideo))
                    return false;

                //ENG - memo 28_2024 saltare controllo per le 0001 0001 0017
                GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
                if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                {
                    if (!((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") &&
                    Utility.IsDomandaTipoContributivo(datiPensione, true, false)))
                    {
                        if (!GestioneControlli.VerificaSettimane(tipoDomanda, datiPensione.Gruppo, datiPensione.SiglaCategoria, datiPensione.NaturaPensione,
                            datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, settimane, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null,
                            sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null, out messaggioVideo))
                            return false;
                    }
                }
                else
                {
                    if (!GestioneControlli.VerificaSettimane(tipoDomanda, datiPensione.Gruppo, datiPensione.SiglaCategoria, datiPensione.NaturaPensione,
                        datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, settimane, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null,
                        sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.ControlsSettimaneWithCodiceSedeAndCertificato(datiPensione, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, settimane,
                    datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null,
                    out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCodReqParticolareAndProfIndivAndAttEconAndNumContribVolontari(datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiPensione.AttivitaEconomica, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, datiPensione.ProfessioneIndividuale, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMinori7(datiGenericiCi != null ? datiGenericiCi.NContributiItalia : null, settimane,
                    datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, datiPensione.DataInizioCalcolo, tipoDomanda,
                    listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, codicePrimoStatoEE,
                    datiDanteCausa != null ? datiDanteCausa.Certificato : null, out messaggioVideo))
                    return false;
            }
            #endregion Categorie minori o uguali a 6

            if (tipoDomanda == Utility.TipoDomanda.Superstiti ||
                (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0 && areaTitolare.ElencoStatiCivili.FindIndex(x => x.Codice == '2') > -1) ||
                (listaFamiliari != null && listaFamiliari.Count > 0))
            {
                if (!GestioneControlli.VerificaContributiWithOrfano(settimane, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null,
                    datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, tipoDomanda, datiDanteCausa != null ? datiDanteCausa.Certificato : null, codiceConvenzione,
                    datiPensione.DecorrenzaOriginaria, presenzaOrfano, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaCodRiduzioneWithCodNatura(datiGenericiCi != null ? datiGenericiCi.RiduzioneRetributiva : false, datiPensione.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodRiduzioneWithEtaTitolare(datiGenericiCi != null ? datiGenericiCi.RiduzioneRetributiva : false, datiAnagraficiTitolare.DataNascita, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodRiduzioneWithPercentualeRiduzione(datiGenericiCi != null ? datiGenericiCi.RiduzioneRetributiva : false, datiGenericiCi != null ? datiGenericiCi.RiduzioneRetributivaPercentuale : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDataDomandaOpzione(datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, codiceConvenzione, datiPensione.DecorrenzaOriginaria, codicePrimoStatoEE, categoria, datiPensione.Gruppo, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaOpzioneWithDataDomandaOpzione(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, codiceConvenzione, codicePrimoStatoEE, datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null, datiPensione.DecorrenzaOriginaria, tipoDomanda, datiAnagraficiTitolare.Cittadinanza, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaNRicoscimentiInvaliditaWithDecorrenza(datiIstruttoria != null ? datiIstruttoria.NRiconoscimentiInvalidita : null, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.NaturaPensione, datiPensione.SiglaCategoria, datiPensione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.CI_VerificaDataDomandaOpzioneWithDanteCausa(datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, decorrenza, datiDanteCausa != null ? datiDanteCausa.SiglaCategoria : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaFineAssicurazioneWithDataDomandaOpzione(datiPensione.FineAssicurazione, decorrenza, datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaRMS8888WithOpzione(datiGenericiCi != null ? datiGenericiCi.RMS8888 : null, decorrenza, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceRequisitiParticolari(datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, categoria, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceRequisitiParticolariWithDatiGenerici(datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, tipoDomanda, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, datiPensione.Gruppo, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaCodiceVirtuale(datiPensione.CausaCarico, datiGenericiCi != null ? datiGenericiCi.DecorrenzaCodiceVirtuale : null, datiGenericiCi != null ? datiGenericiCi.CodiceVirtuale : null, datiPensione.DecorrenzaOriginaria, codiceConvenzione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaFineAssicurazioneForReversibilita(tipoDomanda, datiPensione.FineAssicurazione, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, tipoAppartenenza, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaIncongruenzaEsenzioneFiscaleToDB(datiPensione, datiAnagraficiTitolare != null ? datiAnagraficiTitolare.CodiceComuneResidenza : string.Empty, datiDetrazioni, isRiaperturaDomanda, datiIstruttoria != null ? datiIstruttoria.CodiceComunicazioneCampo4 : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsSettimaneOBGSettimaneDiritto(datiPensione, tipoDomanda, (datiIstruttoria != null) ? (datiIstruttoria.NSettimaneOBG) : null, (datiIstruttoria != null) ? (datiIstruttoria.NContributiVolontari) : null, sommaSettimaneDirittoEstere, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeMisura : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, datiAnagraficiTitolare, dataSistema, out messaggioVideo))
                return false;

            #region Dati Generici
            if (!GestioneCrossControls.AGO_CI_ControlsTipoBeneficioWithCodNatura(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty,
                datiPensione.NaturaPensione, true, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDataInizioCalcolo(datiPensione.DataInizioCalcolo, datiPensione.DataInteressiLegali, datiIstruttoria != null ? datiIstruttoria.CodiceDomandaRicorso : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsConfermaInvalidita(datiPensione, datiEliminazione != null ? datiEliminazione.DataEvento : null,
                datiIstruttoria != null ? datiIstruttoria.NRiconoscimentiInvalidita : null, dataSistema, isRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.AGO_CI_ControlsEliminazioneConfermaInvalidita(datiPensione, datiEliminazione != null ? datiEliminazione.DataEvento : null,
                            datiIstruttoria != null ? datiIstruttoria.NRiconoscimentiInvalidita : null, dataSistema, isRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaCodiceArretratiWithEliminazione(datiEliminazione != null ? datiEliminazione.CodiceMotivo : null, datiPensione.CodiceArretrati, datiPensione, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }

            if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiPL(datiPensione.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, datiPensione, datiPensione.DataInizioCalcolo, dataSistema, out messaggioVideo))
                    return false;
            }
            else
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiRIC(datiPensione.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, datiPensione.CausaCarico, datiPensione.DataInizioCalcolo, out messaggioVideo))
                    return false;
            }

            if (!GestioneCrossControls.AGO_CI_ControlsEsenzioneFiscaleDoppiaImposizione(datiPensione, datiAnagraficiTitolare != null ? datiAnagraficiTitolare.CodiceComuneResidenza : null, isRiaperturaDomanda, datiIstruttoria != null ? datiIstruttoria.CodiceComunicazioneCampo4 : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Dati Generici

            #region Dati Assicurativi
            if (!GestioneCrossControls.ALL_ControlsInizioAssicurazioneSperimentaleDonna(datiPensione, datiPensione.InizioAssicurazione, out messaggioVideo))
                return false;

            /* ENG - 05/11/2024 Deprecata
            if (!GestioneControlli.ControlsLimiteSettimaneReversibilitaSloveniaCroazia(datiPensione, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null,
                datiIstruttoria != null ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi : null, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, codiceConvenzione, codicePrimoStatoEE, out messaggioVideo))
                return false;
            */

            if (!GestioneControlli.ControlsNSettimanePerRequisitoAnticipatoArt1(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerQuota100(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerQuota102(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerSperimentaleDonna_DL_4_2019(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerAnzianitaPerLeggeBilancio2019(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, datiAnagraficiTitolare.Sesso, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceVirtuale(datiPensione, isRiaperturaDomanda, codiceConvenzione, datiGenericiCi != null ? datiGenericiCi.CodiceVirtuale : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimanePerAnticipateFlessibili(datiPensione, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, out messaggioVideo))
                return false;

            #endregion Dati Assicurativi

            #region Dati Provenienza
            if ((datiPensione.TrasformazioneAOI.HasValue && datiPensione.TrasformazioneAOI.Value) || datiPensione.CausaCarico == 3 || datiPensione.CausaCarico == 9)
            {
                if (!GestioneControlli.VerificaDatiPrecedentePensione(datiPensione.CausaCarico, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, datiPensione.NaturaPensione, datiIstruttoria != null ? datiIstruttoria.CodiceP18PrecedentePensione : null, datiIstruttoria != null ? datiIstruttoria.CertificatoPrecedentePensione : null, datiIstruttoria != null ? datiIstruttoria.SedePrecedentePensione : null,
                    categoria, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOriginariaAltraPensione : null, datiPensione != null ? datiPensione.TrasformazioneAOI : null, out messaggioVideo))
                    return false;
            }
            #endregion Dati Provenienza

            if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiGenericiCi != null ? datiGenericiCi.RiduzioneRetributiva : false, datiGenericiCi != null ? datiGenericiCi.RiduzioneRetributivaPercentuale : null, out messaggioVideo))
                return false;

            //if (!GestioneControlli.ControlsNSettimanePerAPEPrecoci(datiPensione, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null,
            //    datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, listaPrestazioniEstere, out messaggioVideo))
            //    return false;

            if (!GestioneControlli.VerificaSettimaneDirittoConvenzioneCanada(codiceConvenzione, listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE : string.Empty, settimane, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaSettimaneDirittoConvenzioneRegnoUnito(codiceConvenzione, listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE : string.Empty, settimane, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(datiPensione, datiPensione.NaturaPensione, datiPensione.InizioAssicurazione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaBeneficiPerOpzioneTipoContributivo(datiPensione, datiPensione.Benefici, out messaggioVideo))
                return false;

            #endregion LiquidazionePensione

            #region DatiContributivi
            #region Prestazioni Estere
            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                int index = 0;
                foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE stato in listaPrestazioniEstere)
                {
                    List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> LimportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == stato.Id);

                    bool isDecorrenzaResidenzaItalianaOK = GestioneControlli.IsDecorrenzaResidenzaItalianaOK(ultimaDecorrenzaResidenzaItaliana, LimportiEsteri);

                    bool dec_Opz = false;
                    bool dec2000 = false;

                    if (!GestioneControlli.VerificaStatoEsteroInConvenzione(int.Parse(stato.CodiceStatoEE)))
                    {
                        messaggioVideo = "Stato estero non in convenzione";
                        return false;
                    }

                    if (!GestioneControlli.VerificaIstituzioneLussemburgo(datiPensione.CausaCarico, int.Parse(stato.CodiceStatoEE), int.Parse(stato.CodiceIstituzione)))
                    {
                        messaggioVideo = "Istituzione Lussemburgo errata: diversa da 0001, 0002, 0003, 0004, 0005, 0501, 0502, 0503";
                        return false;
                    }

                    if (!GestioneControlli.ControlliTurchia(listaCodiciConvenzione, codiceConvenzione, datiPensione.DecorrenzaOriginaria, datiAnagraficiTitolare.Cittadinanza, index, int.Parse(stato.CodiceStatoEE), out messaggioVideo))
                        return false;

                    if (index == 0 && !GestioneControlli.VerificaSloveniaWithDecPensione(int.Parse(stato.CodiceStatoEE), datiPensione.DecorrenzaOriginaria, codiceConvenzione))
                    {
                        messaggioVideo = "Convenzione " + codiceConvenzione + " incompatibile con Stato SLOVENIA";
                        return false;
                    }

                    if (!GestioneControlli.VerificaSloveniaWithCittadinanza(codiceConvenzione, datiAnagraficiTitolare.Cittadinanza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null))
                    {
                        messaggioVideo = "Convenzione Slovenia incompatibile con la cittadinanza";
                        return false;
                    }

                    if (!GestioneControlli.VerificaCroaziaWithCittadinanza(codiceConvenzione, datiAnagraficiTitolare.Cittadinanza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null))
                    {
                        messaggioVideo = "Convenzione Croata incompatibile con la cittadinanza";
                        return false;
                    }

                    if (index == 0)
                    {
                        DateTime? data = GestioneContrib.GetDecorrenzaRiferimentoWithConvenzione(codiceConvenzione, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, int.Parse(stato.CodiceStatoEE), int.Parse(stato.CodiceIstituzione));
                        if (data != null)
                        {
                            if (codiceConvenzione == 4)
                                messaggioVideo = "Decorrenza convenzione '04' non compresa tra 04/1953 e 03/1973";
                            else
                                messaggioVideo = "Decorrenza anteriore alla convenzione (" + codiceConvenzione + "--> " + String.Format("{0:MM/yyyy}", data) + ")";
                            return false;
                        }

                        if (datiDanteCausa != null)
                        {
                            if (!GestioneControlli.VerificaConvenzioneWithDecorrenzaDiretta(codiceConvenzione, decorrenza, int.Parse(stato.CodiceStatoEE)))
                            {
                                messaggioVideo = "Codice Convenzione incompatibile con Decorrenza Pensione";
                                return false;
                            }
                        }

                        if (!GestioneControlli.ControlliSvizzera(settimane, datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null,
                            !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, listaPrestazioniEstere.Count > 1 ? int.Parse(listaPrestazioniEstere[1].CodiceStatoEE) : 0,
                            datiPensione.DecorrenzaOriginaria, categoria, datiPensione.Gruppo, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null,
                            LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, datiAnagraficiTitolare.Sesso, datiAnagraficiTitolare.DataNascita, listaResidenzeEstere,
                            datiEliminazione != null ? datiEliminazione.DecorrenzaEliminazione : null, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaSettimaneSvizzere(codiceConvenzione, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, stato.ContributiEEDecorrenzaOriginaria, datiAnagraficiTitolare.Cittadinanza, out messaggioVideo))
                            return false;
                    }

                    if (datiDanteCausa != null)
                    {
                        if (!GestioneControlli.VerificaStatiEsteriWithDanteCausa(decorrenza, codiceConvenzione, int.Parse(stato.CodiceStatoEE), stato.ContributiEEDecorrenzaOriginaria, LimportiEsteri.Count > 0 ? LimportiEsteri[0].ImportoPrestazioneEE : null))
                        {
                            messaggioVideo = "Settimane esteri mancanti (stato CEE)";
                            return false;
                        }
                    }

                    if (!GestioneControlli.VerificaDataPrecedenteLiquidazioneWithCausaCarico(stato.DecorrenzaLiquidazioneStatoEE, datiPensione.CausaCarico, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaDataPrecedenteLiquidazione(stato.DecorrenzaLiquidazioneStatoEE, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaDataPrecedenteLiquidazioneWithDecImportiEsteri(stato.DecorrenzaLiquidazioneStatoEE, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaDataRicalcolo(stato.DecorrenzaRicalcolo, datiPensione.Gruppo, stato.ContributiEEDecorrenzaOriginaria, stato.ContributiEERicalcolo, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaSettimaneARicalcolo(stato.ContributiEERicalcolo, codiceConvenzione, codicePrimoStatoEE, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, datiPensione.DecorrenzaOriginaria, stato.ContributiEEDecorrenzaOriginaria, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaContributiTurchia(listaCodiciConvenzione, codiceConvenzione, int.Parse(stato.CodiceStatoEE), stato.ContributiEEDiritto, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaContributiDanimarca(codiceConvenzione, datiPensione.DecorrenzaOriginaria, stato.CodiceStatoEE, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.ContributiEEDiritto, datiAnagraficiTitolare.Cittadinanza))
                    {
                        messaggioVideo = "Contributi o quota Danesi incompatibili con cittadinanza extraUE.";
                        return false;
                    }

                    if (!GestioneControlli.VerificaContributiDanimarcaDanteCausa(codiceConvenzione, datiPensione.DecorrenzaOriginaria, stato.CodiceStatoEE,
                        LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.ContributiEEDiritto,
                        datiAnagraficiDC != null ? datiAnagraficiDC.Cittadinanza : string.Empty, datiPensione.Gruppo, datiPensione.Prodotto))
                    {
                        messaggioVideo = "Ctr. o Quota DANESI incompatibili con cittad.extraUE dante causa.";
                        return false;
                    }

                    if (!GestioneControlli.VerificaSospensioneEstero(stato.SospensioneCautelativaIntegrazione, tipoDomanda, stato.CodiceArt48, stato.EtaSospensione, datiAnagraficiTitolare.Sesso, datiPensione.CausaCarico, datiAnagraficiTitolare.DataNascita, codiceConvenzione, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaArticolo48(datiPensione, stato.CodiceArt48, codiceConvenzione, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, stato.DecorrenzaArt48, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.ContributiEEDecorrenzaOriginaria, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, stato.ContributiEEDiritto, listaPrestazioniEstere.Exists(x => x.CodiceStatoEE.Trim() == "11"), listaPrestazioniEstere.Exists(x => x.CodiceStatoEE.Trim() == "20"), listaPrestazioniEstere.FindIndex(x => x.CodiceStatoEE.Trim() == "17") > -1 ? true : false, datiAnagraficiTitolare.Cittadinanza, datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaSettimaneEstere(tipoDomanda, codiceConvenzione, datiPensione.DecorrenzaOriginaria, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, stato.ContributiEEDecorrenzaOriginaria, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].ImportoPrestazioneEE : null, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, index, stato.ContributiEEDiritto, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, !string.IsNullOrEmpty(stato.CodiceIstituzione) ? int.Parse(stato.CodiceIstituzione) : 0, out messaggioVideo))
                        return false;

                    if (GestioneControlli.GetCodiceConvenzioneByCodiceStatoEE(!string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, datiPensione.DecorrenzaOriginaria) == 0)
                    {
                        messaggioVideo = "Stato/Convenzione errato o mancante";
                        return false;
                    }

                    if (!GestioneControlli.VerificaConvenzioneVaticano(codiceConvenzione, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, stato.ContributiEEDecorrenzaOriginaria, datiAnagraficiTitolare.Cittadinanza, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaNuovaCaledonia(!string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, !string.IsNullOrEmpty(stato.CodiceIstituzione) ? int.Parse(stato.CodiceIstituzione) : 0, stato.ContributiEEDecorrenzaOriginaria, stato.ContributiEERicalcolo, stato.ContributiEEDiritto, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.SospensioneCautelativaIntegrazione, out messaggioVideo))
                        return false;

                    if (LimportiEsteri != null && LimportiEsteri.Count > 0)
                    {
                        int indexImportiEsteri = 0;
                        GestioneDatiContributiviCi.PensioniCiImportiEsteri appImportoEstero = null;
                        foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importiEsteri in LimportiEsteri)
                        {
                            if (!GestioneControlli.VerificaPresenzaMatricola(datiPensione.CausaCarico, datiEliminazione != null ? datiEliminazione.CodiceMotivo : null, !string.IsNullOrEmpty(stato.MatricolaIstituzioneEE) ? stato.MatricolaIstituzioneEE : null, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, !string.IsNullOrEmpty(stato.CodiceIstituzione) ? int.Parse(stato.CodiceIstituzione) : 0, importiEsteri.DecorrenzaPrestazioneEE, out messaggioVideo))
                                return false;

                            if (appImportoEstero != null)
                            {
                                if (!Utility.DataStrettamenteSuccessivaA(importiEsteri.DecorrenzaPrestazioneEE.Value, appImportoEstero.DecorrenzaPrestazioneEE.Value))
                                {
                                    messaggioVideo = "Decorrenza Prestazione Estera non in sequenza";
                                    return false;
                                }

                                if (appImportoEstero.CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(importiEsteri.DecorrenzaPrestazioneEE.Value, appImportoEstero.CessazionePrestazioneEE.Value))
                                {
                                    messaggioVideo = "Decorrenza Prestazione Estera non posteriore a Cessazione precedente";
                                    return false;
                                }
                            }

                            if (!GestioneControlli.VerificaDecorrenzaImportiEsteriPosterioreADataOdierna(importiEsteri.DecorrenzaPrestazioneEE, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithDecorrenzaOriginaria(importiEsteri.DecorrenzaPrestazioneEE, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                                return false;

                            if (indexImportiEsteri > 0)
                            {
                                if (!GestioneControlli.VerificaMeseDecorrenzaImportiEsteriPerLussemburgo(importiEsteri.DecorrenzaPrestazioneEE, tipoDomanda, appImportoEstero.CessazionePrestazioneEE, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, LimportiEsteri[0].DecorrenzaPrestazioneEE, out messaggioVideo))
                                    return false;
                            }

                            if (!GestioneControlli.VerificaCompatibilitaImportoWithDecorrenza(importiEsteri.ImportoPrestazioneEE, importiEsteri.DecorrenzaPrestazioneEE, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaCoerenzaDecorrenzaCessazione(importiEsteri.DecorrenzaPrestazioneEE, importiEsteri.CessazionePrestazioneEE, out messaggioVideo))
                                return false;

                            GestioneControlli.GetDecOpz(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, LimportiEsteri, indexImportiEsteri, ref dec_Opz);
                            dec2000 = GestioneControlli.GetDec2000(LimportiEsteri);

                            appImportoEstero = importiEsteri;
                            indexImportiEsteri++;
                        }

                        if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithCodiceVirtuale(LimportiEsteri.Last().DecorrenzaPrestazioneEE, LimportiEsteri.Last().CessazionePrestazioneEE, tipoDomanda, datiGenericiCi != null ? datiGenericiCi.CodiceVirtuale : null, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithCodiceOpzione(datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null, LimportiEsteri.First().DecorrenzaPrestazioneEE, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                            return false;
                    }

                    if (!GestioneControlli.VerificaDataRicalcolo(stato.DecorrenzaRicalcolo, LimportiEsteri, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaObbligatorietaDecorrenzaImportiEsteri(LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null,
                        codiceConvenzione, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0,
                        settimane.GetValueOrDefault() - (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0), stato.DecorrenzaLiquidazioneStatoEE,
                        LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri.Last().CessazionePrestazioneEE : null, datiPensione.CausaCarico,
                        datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, ultimaDecorrenzaResidenzaItaliana, isDecorrenzaResidenzaItalianaOK, codicePrimoStatoEE, dec_Opz,
                        datiAnagraficiTitolare.CodiceComuneResidenza, dec2000, datiPensione.DecorrenzaOriginaria, stato.ContributiEEDecorrenzaOriginaria, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlliVenezuela(tipoDomanda, !string.IsNullOrEmpty(stato.CodiceStatoEE) ? int.Parse(stato.CodiceStatoEE) : 0, stato.SospensioneCautelativaIntegrazione, stato.ContributiEEDiritto, stato.ContributiEEDecorrenzaOriginaria, LimportiEsteri, datiPensione, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlliYugoslavia(datiPensione.CausaCarico, codiceConvenzione, datiAnagraficiTitolare.CodiceComuneResidenza, int.Parse(stato.CodiceStatoEE), LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].DecorrenzaPrestazioneEE : null, datiPensione.DecorrenzaOriginaria, stato.DecorrenzaIntegrazione, stato.QuotaIntegrazioneEEeArgentinaResidentiItalia, stato.DecorrenzaLiquidazioneStatoEE, LimportiEsteri != null && LimportiEsteri.Count > 0 ? LimportiEsteri[0].ImportoPrestazioneEE : null, index, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaSospensioneCautelativaIntegrazioneObbligatoria(stato.SospensioneCautelativaIntegrazione, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo \\ Istituzioni Estere:<br/>" + messaggioVideo;
                        return false;
                    }

                    index++;
                }

                if (!GestioneControlli.VerificaCompatibilitaTraStati(codiceConvenzione, listaPrestazioniEstere, datiPensione.DecorrenzaOriginaria, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaPresenzaDecorrenza01_XXper335(listaPrestazioniEstere, listaImportiEsteri, codiceConvenzione, datiAnagraficiTitolare.CodiceComuneResidenza, listaResidenzeEstere, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportiEsteriWithCodNatura(listaPrestazioniEstere, listaImportiEsteri, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsDomandeInabilità(datiPensione, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null, sommaSettimaneDirittoEstere, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null,
                    datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, datiGenericiCi != null ? datiGenericiCi.SettimaneItalianeDiritto : null, out messaggioVideo))
                    return false;
            }
            #endregion Prestazioni Estere

            #region Dati Calcolo
            bool IsDomandaStandard = false;
            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && (datiPensione.SiglaCategoria.ToUpperInvariant().Trim() == "VOS" || datiPensione.SiglaCategoria.ToUpperInvariant().Trim() == "IOS" || datiPensione.SiglaCategoria.ToUpperInvariant().Trim() == "SOS"))
                IsDomandaStandard = true;

            if (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0)
            {
                if (IsDomandaStandard)
                {
                    if (listaDatiCalcoloRetributivo.Count > 2)
                    {
                        messaggioVideo = "Dati Retributivi: è possibile acquisire al più due record, per una domanda di categoria 'VOS, IOS o SOS'.";
                        return false;
                    }
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo Retrb in listaDatiCalcoloRetributivo)
                    {
                        if (!Retrb.CodiceGestione.HasValue || Retrb.CodiceGestione.Value == 0)
                        {
                            messaggioVideo = "Dati Retributivi: Codice gestione obbligatorio.";
                            return false;
                        }
                        if (Retrb.CodiceGestione.HasValue && Retrb.CodiceGestione.Value != 1)
                        {
                            messaggioVideo = "Dati Retributivi: Codice gestione deve essere obligatoriamente: 'gestione OBG'.";
                            return false;
                        }
                    }
                }

                List<GestioneCalcolo.DatiCalcoloRetributivo> listApp = listaDatiCalcoloRetributivo.FindAll(delegate (GestioneCalcolo.DatiCalcoloRetributivo retr1)
                {
                    return listaDatiCalcoloRetributivo.FindAll(delegate (GestioneCalcolo.DatiCalcoloRetributivo retr2)
                    {
                        return (retr1.CodiceGestione == retr2.CodiceGestione && retr1.QuotePrimeLiquidate == retr2.QuotePrimeLiquidate);
                    }).Count > 1;
                }).ToList();

                if (listApp.Count > 1)
                {
                    messaggioVideo = "Dati Retributivi: non può essere presente più di una occorrenza con lo stesso codice gestione e la stessa quota.";
                    return false;
                }

            }

            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                if (IsDomandaStandard)
                {
                    if (listaDatiCalcoloContributivo.Count > 2)
                    {
                        messaggioVideo = "Dati Calcolo: è possibile acquisire al più due record di tipo dati calcolo, per una domanda di categoria 'VOS, IOS o SOS'.";
                        return false;
                    }

                    foreach (GestioneCalcolo.DatiCalcoloContributivo Contr in listaDatiCalcoloContributivo)
                    {
                        if (!Contr.CodiceGestione.HasValue || Contr.CodiceGestione.Value == 0)
                        {
                            messaggioVideo = "Dati Calcolo: Codice gestione obbligatorio.";
                            return false;
                        }
                        if (Contr.CodiceGestione.HasValue && Contr.CodiceGestione.Value != 1)
                        {
                            messaggioVideo = "Dati Calcolo: Codice gestione deve essere obligatoriamente: 'gestione FPLD'.";
                            return false;
                        }
                    }
                }

                List<GestioneCalcolo.DatiCalcoloContributivo> listApp = listaDatiCalcoloContributivo.FindAll(delegate (GestioneCalcolo.DatiCalcoloContributivo contr1)
                {
                    return listaDatiCalcoloContributivo.FindAll(delegate (GestioneCalcolo.DatiCalcoloContributivo contr2)
                    {
                        return (contr1.CodiceGestione == contr2.CodiceGestione &&
                            ((contr1.NSettimane.HasValue && contr2.NSettimane.HasValue && contr1.MontanteContributivo.HasValue && contr2.MontanteContributivo.HasValue && contr1.ImportoContributivoTotale.HasValue && contr2.ImportoContributivoTotale.HasValue) ||
                            (contr1.NSettimaneQuotaDL214.HasValue && contr2.NSettimaneQuotaDL214.HasValue && contr1.MontanteQuotaDL214.HasValue && contr2.MontanteQuotaDL214.HasValue && contr1.ImportoContribTotaleQuotaDL214.HasValue && contr2.ImportoContribTotaleQuotaDL214.HasValue)));
                    }).Count > 1;
                }).ToList();

                if (listApp.Count > 1)
                {
                    messaggioVideo = "Dati Calcolo: non può essere presente più di una occorrenza con lo stesso codice gestione e la stessa quota.";
                    return false;
                }

            }

            if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 1)
            {
                int index = 0;
                GestioneCalcolo.DatiCalcoloContributivoEstero contrEsteroApp = null;
                foreach (GestioneCalcolo.DatiCalcoloContributivoEstero contrEstero in listaDatiCalcoloContributivoEstero)
                {
                    if (index == 0)
                        contrEsteroApp = contrEstero;
                    else
                    {
                        if (!Utility.DataSuccessivaA(contrEstero.Decorrenza.Value, contrEsteroApp.Decorrenza.Value))
                        {
                            messaggioVideo = "Decorrenza Contributi Esteri non in sequenza";
                            return false;
                        }
                    }

                    if ((contrEstero.CodiceGestione.HasValue || contrEstero.Decorrenza.HasValue) && (!contrEstero.CodiceGestione.HasValue || !contrEstero.Decorrenza.HasValue))
                    {
                        messaggioVideo = "Registrazioni Contributi Esteri incomplete";
                        return false;
                    }

                    int indexDupl = 0;
                    foreach (GestioneCalcolo.DatiCalcoloContributivoEstero contrEsteroDupl in listaDatiCalcoloContributivoEstero)
                    {
                        if (index < indexDupl)
                        {
                            if (contrEstero.CodiceGestione == contrEsteroDupl.CodiceGestione && contrEstero.Decorrenza == contrEsteroDupl.Decorrenza)
                            {
                                messaggioVideo = "Stesso Codice Gestione e Decorrenza ripetuto in piu' registrazioni";
                                return false;
                            }
                        }

                        indexDupl++;
                    }

                    index++;
                }
            }

            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> LmaternitaAcna = null;
            GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(datiPensione.Id, out LmaternitaAcna);

            if (LmaternitaAcna != null && (!datiGenericiCi.MaternitaAcna.HasValue || (datiGenericiCi.MaternitaAcna.HasValue && !datiGenericiCi.MaternitaAcna.Value)))
            {
                messaggioVideo = "Cancellare i dati della tab 'Maternità/Acna' prima di procedere con il salvataggio dei dati della tab 'Dati Calcolo'";
                return false;
            }

            int? nSettimane = null;
            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo app = listaDatiCalcoloContributivo.FindAll(x => x.NSettimane.HasValue).FirstOrDefault();
                if (app != null)
                    nSettimane = app.NSettimane;
            }
            if (!GestioneControlli.VerificaSettVVMisuraWithDecOrigWithDecOpzioneWithNContribVolWithNsett(datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, datiPensione.DecorrenzaOriginaria, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null,
                datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiGenericiCi != null ? datiGenericiCi.ImportoIVS : null, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, nSettimane))
            {
                messaggioVideo = "Settimane VV per Misura mancanti o incompatibili con VV diritto.";
                return false;
            }

            if (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in listaDatiCalcoloRetributivo)
                {
                    if (!GestioneControlli.VerificaRMSWithDecOriginaria(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiRetributivi.RMSQuotaA))
                    {
                        messaggioVideo = "R.M.S. errata per decorrenza ante 05/1968.";
                        return false;
                    }

                    if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiRetributivi.RMSQuotaA,
                        datiPensione.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                        datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null,
                        datiPensione.NaturaPensione, datiPensione.Gruppo, datiPensione.Prodotto))
                    {
                        messaggioVideo = "R.M.S. mancante.";
                        return false;
                    }

                    #region Categorie minori o uguali a 6
                    if (categoria > 0 && categoria <= 6)
                    {
                        if (!GestioneControlli.VerificaRMSQuotaAWithDecorrenze(decorrenza, datiRetributivi.RMSQuotaA, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaNSettimaneQuotaAWithInizioAssicurazione(datiRetributivi.NSettimaneQuotaA, datiPensione.InizioAssicurazione, out messaggioVideo))
                            return false;
                    }
                    #endregion Categorie minori o uguali a 6
                }
            }

            #region PCIPL39 categoria >= 7
            if (categoria >= 7 || ((listaDatiCalcoloRetributivo == null || listaDatiCalcoloRetributivo.Count == 0 || !listaDatiCalcoloRetributivo.Exists(x => x.QuotePrimeLiquidate == 'A')) && !Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) && !Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione)))
            {
                if (!GestioneControlli.VerificaSettimanePost1993WithNSettimaneIncrementoPercentuale(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, settimaneRetributiveQuotaBCodGestione1, null, null, out messaggioVideo))
                    return false;
               
                    if (!GestioneControlli.VerificaSettimaneDatiCalcolo(settimaneItalianeMisura, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                    {
                        messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                        return false;
                    }
                
                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione1, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione2, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione3, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione4, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsRMSQuotaBWithDecorrenzaAndFineAssicurazione(categoria, datiPensione.NaturaPensione, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, rmsQuotaBCodGestione2, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsRMSQuotaBWithDecorrenzaAndFineAssicurazione(categoria, datiPensione.NaturaPensione, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, rmsQuotaBCodGestione3, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsRMSQuotaBWithDecorrenzaAndFineAssicurazione(categoria, datiPensione.NaturaPensione, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, rmsQuotaBCodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                //84: categoria minima per la comparazione all'interno del metodo. 87: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, 84, 87, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                //87: categoria minima per la comparazione all'interno del metodo. 91: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, 87, 91, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                //90: categoria minima per la comparazione all'interno del metodo. 94: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, 90, 94, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaContributiviWithDecorrenza(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                datiPensione.NaturaPensione, montanteCodGestione1, settimaneContributiveCodGestione1, importoContributivoTotaleCodGestione1, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaContributiviWithDecorrenza(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, montanteCodGestione2, settimaneContributiveCodGestione2, importoContributivoTotaleCodGestione2, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione1, importoContributivoTotaleCodGestione1, settimaneContributiveCodGestione1, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione2, importoContributivoTotaleCodGestione2, settimaneContributiveCodGestione2, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione3, importoContributivoTotaleCodGestione3, settimaneContributiveCodGestione3, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione4, importoContributivoTotaleCodGestione4, settimaneContributiveCodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsQuotaAWithSettimaneQuotaDAndCategoria(categoria, settimaneRetributiveQuotaACodGestione2, settimaneRetributiveQuotaBCodGestione2, settimaneContributiveCodGestione2, settimaneContributiveDL214CodGestione2, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, 2, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsQuotaAWithSettimaneQuotaDAndCategoria(categoria, settimaneRetributiveQuotaACodGestione3, settimaneRetributiveQuotaBCodGestione3, settimaneContributiveCodGestione3, settimaneContributiveDL214CodGestione3, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, 3, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsQuotaAWithSettimaneQuotaDAndCategoria(categoria, settimaneRetributiveQuotaACodGestione4, settimaneRetributiveQuotaBCodGestione4, settimaneContributiveCodGestione4, settimaneContributiveDL214CodGestione4, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, 4, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsContributiviWithDecorrenzaWithSettQuotaD(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, montanteCodGestione3, settimaneContributiveCodGestione3, importoContributivoTotaleCodGestione3, settimaneContributiveDL214CodGestione3, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsContributiviWithDecorrenzaWithSettQuotaD(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, montanteCodGestione4, settimaneContributiveCodGestione4, importoContributivoTotaleCodGestione4, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneQuotaBWithRsmQuotaB(decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione,
                    settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, categoria, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaFineAssicurazioneWithSettimaneQuotaD(datiPensione.InizioAssicurazione, settimaneContributiveDL214CodGestione1, settimaneContributiveDL214CodGestione2, settimaneContributiveDL214CodGestione3, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaRmsQuotaBAndSettimaneWithFineAssicurazione(datiPensione, datiPensione.FineAssicurazione, rmsQuotaBCodGestione1, rmsQuotaBCodGestione2, rmsQuotaBCodGestione3, rmsQuotaBCodGestione4,
                    settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, settimaneContributiveDL214CodGestione1, settimaneContributiveDL214CodGestione2,
                    settimaneContributiveDL214CodGestione3, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaInizioAndFineAssicurazioneWithSettimaneTotaliQuotaB(decorrenza, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, settimaneQuotaBTotale, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneFittizieWithCmsmAndRMS(decorrenza, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento.GetValueOrDefault() : 0, datiGenericiCi != null ? datiGenericiCi.CMSM : null, categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, 2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCmsmAndRMS(decorrenza, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento.GetValueOrDefault() : 0, datiGenericiCi != null ? datiGenericiCi.CMSM : null, categoria, rmsQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione3, 3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCmsmAndRMS(decorrenza, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento.GetValueOrDefault() : 0, datiGenericiCi != null ? datiGenericiCi.CMSM : null, categoria, rmsQuotaBCodGestione4, settimaneRetributiveQuotaBCodGestione4, 4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione2, out messaggioVideo))
                {
                    messaggioVideo = "Contributi CD/CM dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione3, out messaggioVideo))
                {
                    messaggioVideo = "Contributi ART dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Contributi COM dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneQuotaBWithPeriodoAssicurativo(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione1, datiPensione.DataInizioCalcolo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCmsmWithDecorrenza(decorrenza, datiGenericiCi != null ? datiGenericiCi.CMSM : null, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithLegge335(datiPensione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneContributiveCodGestione1, montanteCodGestione1, importoContributivoTotaleCodGestione1, settimaneContributiveCodGestione2, montanteCodGestione2, importoContributivoTotaleCodGestione2, settimaneContributiveCodGestione3, montanteCodGestione3, importoContributivoTotaleCodGestione3, settimaneContributiveCodGestione4, montanteCodGestione4, importoContributivoTotaleCodGestione4, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione2, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione3, rmsQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione4, rmsQuotaBCodGestione4, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, isCodiceGestione0XPresenteContributiItalianiEdEsteri, isCodiceGestione6XPresenteContributiItalianiEdEsteri, primoCodiceGestioneTraduzioneSuGP, codiceArt48PrimoStato, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneContributive(categoria, 2, datiPensione.NaturaPensione, settimaneContributiveCodGestione2, settimaneContributiveDL214CodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneContributive(categoria, 3, datiPensione.NaturaPensione, settimaneContributiveCodGestione3, settimaneContributiveDL214CodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneContributive(categoria, 4, datiPensione.NaturaPensione, settimaneContributiveCodGestione4, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(1, datiPensione.NaturaPensione, rmsQuotaACodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(2, datiPensione.NaturaPensione, rmsQuotaACodGestione2, rmsQuotaBCodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(3, datiPensione.NaturaPensione, rmsQuotaACodGestione3, rmsQuotaBCodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(4, datiPensione.NaturaPensione, rmsQuotaACodGestione4, rmsQuotaBCodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(1, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione1, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(2, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione2, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(3, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione3, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(4, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione4, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
                {
                    int index = 0;
                    foreach (GestioneCalcolo.DatiCalcoloContributivoEstero datiContributiEsteri in listaDatiCalcoloContributivoEstero)
                    {
                        int? settimaneToCompare = 0;
                        short? codiceGestioneTraduzioneSuGP = 0;
                        if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                        {
                            GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContributiEsteri.CodiceGestione.Value);
                            if (codeGestione != null)
                                codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                        }

                        if (!GestioneControlli.VerificaContributiItalianiEdEsteri(codiceGestioneTraduzioneSuGP, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, settimaneRicalcoloMisura, out messaggioVideo))
                        {
                            messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                            return false;
                        }

                        if (!GestioneControlli.VerificaRMSWithContributiItalianiEdEsteri(codiceGestioneTraduzioneSuGP, montanteCodGestione1, montanteContributivoQuotaDCodGestione1, montanteCodGestione2, montanteContributivoQuotaDCodGestione2, montanteCodGestione3, montanteContributivoQuotaDCodGestione3, montanteCodGestione4, montanteContributivoQuotaDCodGestione4, rmsQuotaBCodGestione1, rmsQuotaBCodGestione2, rmsQuotaBCodGestione3, rmsQuotaBCodGestione4, rmsQuotaACodGestione1, rmsQuotaACodGestione2, rmsQuotaACodGestione3, rmsQuotaACodGestione4, out messaggioVideo))
                        {
                            messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                            return false;
                        }

                        if (!GestioneControlli.VerificaDecorrenzaContributiItalianiEdEsteri(datiContributiEsteri.Decorrenza, decorrenza, datiGenericiCi != null ? datiGenericiCi.DecorrenzaBonus : null, codiceGestioneTraduzioneSuGP, primaDecorrenzaImportiEsteri, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, out messaggioVideo))
                        {
                            messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                            return false;
                        }

                        if (!GestioneControlli.VerificaSettimaneEstereWithContributiItalianiEdEsteri(datiContributiEsteri.Decorrenza, numeroSettimaneEstere[index], sommaSettimaneContributiItalianiEdEsteri, decorrenza, datiPensione.InizioAssicurazione, sommaSettimaneContributi, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaCapienzaSettimaneContributiItalianiEdEsteri(codiceGestioneTraduzioneSuGP, decorrenza, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, datiContributiEsteri.Decorrenza, settimaneRetributiveQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione4, datiContributiEsteri.Settimane, categoria, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaSettimaneWithContributiItalianiEdEsteri(sommaSettimaneContributiItalianiEdEsteri, settimaneEstereWithCodiceArt48, sommaSettimaneContributi, out messaggioVideo))
                            return false;

                        settimaneToCompare = settimaneRetributiveQuotaBCodGestione2;
                        foreach (GestioneCalcolo.DatiCalcoloContributivoEstero appDatiContributiEsteri in listaDatiCalcoloContributivoEstero)
                            settimaneToCompare = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteri9395(settimaneToCompare, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, appDatiContributiEsteri.Settimane, 62);

                        if (!GestioneControlli.VerificaSettimaneNelPeriodo9395(datiPensione.FineAssicurazione, datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, decorrenza, settimaneToCompare, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, "CD-CM", out messaggioVideo))
                            return false;

                        settimaneToCompare = settimaneRetributiveQuotaBCodGestione3;
                        foreach (GestioneCalcolo.DatiCalcoloContributivoEstero appDatiContributiEsteri in listaDatiCalcoloContributivoEstero)
                            settimaneToCompare = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteri9395(settimaneToCompare, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, appDatiContributiEsteri.Settimane, 63);

                        if (!GestioneControlli.VerificaSettimaneNelPeriodo9395(datiPensione.FineAssicurazione, datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, decorrenza, settimaneToCompare, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, "ART", out messaggioVideo))
                            return false;

                        settimaneToCompare = settimaneRetributiveQuotaBCodGestione4;
                        foreach (GestioneCalcolo.DatiCalcoloContributivoEstero appDatiContributiEsteri in listaDatiCalcoloContributivoEstero)
                            settimaneToCompare = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteri9395(settimaneToCompare, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, appDatiContributiEsteri.Settimane, 64);

                        if (!GestioneControlli.VerificaSettimaneNelPeriodo9395(datiPensione.FineAssicurazione, datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, decorrenza, settimaneToCompare, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, "COM", out messaggioVideo))
                            return false;

                        index++;
                    }

                    if (categoria >= 7)
                    {
                        if (!GestioneControlli.VerificaContributiItalianiEdEsteriWithSettimaneProRata(sommaSettimaneContributiItalianiEdEsteri, settimaneRicalcoloMisura, set_Rical, isDecorrenzaContributiItalianiEdEsteriDuplicata, datiPensione.DecorrenzaOriginaria, listaDatiCalcoloContributivoEstero[0].Decorrenza, sommaSettimaneCodiceGestioneX4.GetValueOrDefault() == 2080, out messaggioVideo))
                            return false;
                    }
                }
                else
                {
                    if (!GestioneControlli.VerificaObbligatorietaContributiItalianiEdEsteri(null, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, settimaneRicalcoloMisura, out messaggioVideo))
                    {
                        messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                        return false;
                    }
                }

                if (!GestioneControlli.VerificaRMSQuotaBWithDecorrenzaAndUltimoContributo(rmsQuotaBCodGestione1, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, datiPensione.DataInizioCalcolo, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneQuotaBWithDecorrenzaAndUltimoContributo(settimaneRetributiveQuotaBCodGestione1, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, datiPensione.DataInizioCalcolo, datiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }
            }
            #endregion PCIPL39 categoria >= 7

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione1, importoContributivoTotaleCodGestione1, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione2, importoContributivoTotaleCodGestione2, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione3, importoContributivoTotaleCodGestione3, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione4, importoContributivoTotaleCodGestione4, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaCompletezzaDatiContributiviQuotaD(settimaneContributiveDL214CodGestione1, importoContributivoTotaleQuotaDCodGestione1, montanteContributivoQuotaDCodGestione1,
                datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                return false;
            }

            #region Categorie minori o uguali a 6
            if ((categoria > 0 && categoria <= 6) || categoria == 88 || categoria == 91 || categoria == 85)
            {
                if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295(datiPensione, datiPensione.DecorrenzaOriginaria, datiAnagraficiTitolare != null ? datiAnagraficiTitolare.DataNascita : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                                                                            datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiPensione.DataPerfezionamentoRequisiti, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null,
                                                                            datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimane707QuotaBTotali, rmsQuotaBTotale, rmsQuotaATotale, settimaneQuotaATotale,
                                                                            settimaneQuotaBTotale, settimaneQuotaCTotale, settimaneQuotaDTotale, datiDanteCausa != null ? datiDanteCausa.Certificato : null, categoria, datiPensione.NaturaPensione, datiPensione.Gruppo, out messaggioVideo))
                    return false;
            }

            if (categoria > 0 && categoria <= 6)
            {
                if (!GestioneControlli.ControlsSettimaneWithAnzianitaAndAustralia(codiceConvenzione, datiPensione.NaturaPensione, settimane,
                    datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, settimaneRetributiveQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimanePost1993WithNSettimaneIncrementoPercentuale(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, settimaneRetributiveQuotaBCodGestione1, settimaneContributiveCodGestione1, settimaneContributiveDL214CodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMSQuotaAWithOpzioneAndDanteCausa(categoria, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, rmsQuotaACodGestione1, datiPensione.InizioAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.NaturaPensione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaObbligatorietaRMSQuotaAWithDecorrenze(categoria, rmsQuotaACodGestione1, decorrenza, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaRMSQuotaBWithSettimane(datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneQuotaCTotale, settimaneQuotaDTotale, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiGenericiCi != null ? datiGenericiCi.CMSM : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsRMSWithDecorrenzaAndAssicurazioneAndCodNatura(decorrenza, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, rmsQuotaACodGestione1, settimaneRetributiveQuotaACodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione.Gruppo, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : null, datiAnagraficiTitolare.DataNascita, datiAnagraficiTitolare.Sesso, datiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaContributiWithFineAssicurazione(datiPensione.FineAssicurazione, datiPensione.NaturaPensione, settimaneRetributiveQuotaBCodGestione1, settimaneContributiveCodGestione1, settimaneContributiveDL214CodGestione1, codiceConvenzione, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, rmsQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaContributiWithDecorrenza(decorrenza, datiPensione.DecorrenzaOriginaria, settimaneContributiveCodGestione1, importoContributivoTotaleCodGestione1, montanteCodGestione1, settimaneContributiveDL214CodGestione1, importoContributivoTotaleQuotaDCodGestione1, montanteContributivoQuotaDCodGestione1, datiPensione.NaturaPensione, datiPensione.FineAssicurazione, rmsQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.CMSM : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, datiDanteCausa != null ? datiDanteCausa.Certificato : null, settimaneRetributiveQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaRegistrazioneADecorrenza(datiPensione.InizioAssicurazione, primoCodiceGestioneTraduzioneSuGP, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.VerificaCMSM(decorrenza, datiGenericiCi != null ? datiGenericiCi.CMSM : null, datiGenericiCi != null ? datiGenericiCi.NSettFittiziePrepensionamento : null, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo - " + messaggioVideo;
                    return false;
                }

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                // ENG - Bypass MONTANTE_335_INCOMPATIBILE con data ultimo contributo
                if (!Utility.IsDomandaTipoContributivo(datiPensione, null, true) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) &&
                    !((!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                       ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))))
                {
                    if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_CI.MONTANTE_335_INCOMPATIBILE))
                    {
                        if (!GestioneControlli.VerificaMontante335(decorrenza, datiPensione.FineAssicurazione, montanteCodGestione1, datiPensione.NaturaPensione, datiGenericiCi != null ? datiGenericiCi.CMSM : null, out messaggioVideo))
                        {
                            messaggioVideo = "Dati Calcolo - " + messaggioVideo;
                            return false;
                        }
                    }
                }

                //if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295(datiPensione.InizioAssicurazione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, datiPensione.NaturaPensione, settimaneContributiveCodGestione1, importoContributivoTotaleCodGestione1, montanteCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, rmsQuotaBCodGestione1, datiPensione.Gruppo, isCodiceGestione0XPresenteContributiItalianiEdEsteri, primoCodiceGestioneTraduzioneSuGP, isCodiceGestione6XPresenteContributiItalianiEdEsteri, codiceArt48PrimoStato, out messaggioVideo))
                //    return false;

                int sommaSettimaneContrEE = 0;
                int sommaSettimaneContrEE1993_1995 = 0;


                if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
                {
                    int index = 0;
                    foreach (GestioneCalcolo.DatiCalcoloContributivoEstero datiContribEsteri in listaDatiCalcoloContributivoEstero)
                    {
                        short? codiceGestioneTraduzioneSuGP = 0;
                        if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                        {
                            GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContribEsteri.CodiceGestione.Value);
                            if (codeGestione != null)
                                codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                        }

                        sommaSettimaneContrEE = GestioneControlli.CalcolaSettimaneContrEE(sommaSettimaneContrEE, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, datiContribEsteri.Decorrenza, datiContribEsteri.Settimane, datiPensione.DecorrenzaOriginaria);

                        sommaSettimaneContrEE1993_1995 = GestioneControlli.CalcolaSettimane(datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneRetributiveQuotaBCodGestione1,
                            codiceGestioneTraduzioneSuGP, datiContribEsteri.Settimane, sommaSettimaneContrEE1993_1995);

                        if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1993, 01, 01)) && datiPensione.InizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1993, 01, 01)))
                        {
                            if (index == 0)
                            {
                                sommaSettimaneDecUgualePrimaDec = listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0 ? listaDatiCalcoloContributivoEstero[0].Settimane.GetValueOrDefault() : 0;
                            }
                            else
                            {
                                if (listaDatiCalcoloContributivoEstero[0].Equals(datiContribEsteri.Decorrenza))
                                {
                                    sommaSettimaneDecUgualePrimaDec += datiContribEsteri.Settimane.GetValueOrDefault();
                                }
                            }
                        }
                        index++;
                    }
                }

                if (!GestioneControlli.VerificaSettimane1993_1995(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.DecorrenzaOriginaria, sommaSettimaneContrEE1993_1995, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimane23390_50392_33595(sommaSettimaneCodGestione1_61CTRItalianiEdEsteri, settimaneRicalcoloMisura, out messaggioVideo))
                    return false;
            }
            #endregion Categorie minori o uguali a 6

            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                int index = 0;
                foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioniEE in listaPrestazioniEstere)
                {
                    if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
                    {
                        int indexContributi = 0;
                        GestioneDatiContributiviCi.PensioniCiImportiEsteri LImportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == listaPrestazioniEstere[index].Id).FirstOrDefault();
                        foreach (GestioneCalcolo.DatiCalcoloContributivoEstero datiContribEsteri in listaDatiCalcoloContributivoEstero)
                        {
                            short? codiceGestioneTraduzioneSuGP = 0;
                            if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                            {
                                GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContribEsteri.CodiceGestione.Value);
                                if (codeGestione != null)
                                    codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                            }

                            if (index > 0)
                            {
                                if (!GestioneControlli.ControlsSettimaneEstereWithDecorrenzaRicalcolo(datiPensione.InizioAssicurazione, datiContribEsteri.Decorrenza, prestazioniEE.ContributiEERicalcolo,
                                     LImportiEsteri != null ? LImportiEsteri.DecorrenzaPrestazioneEE : null, out messaggioVideo))
                                    return false;
                            }

                            if (categoria > 0 && categoria <= 6)
                            {
                                if (!GestioneControlli.ControlsContributiEsteri(indexContributi, codiceGestioneTraduzioneSuGP, datiContribEsteri.Decorrenza, datiContribEsteri.Settimane, datiPensione.InizioAssicurazione,
                                    rmsQuotaBCodGestione1, importoContributivoTotaleCodGestione1, importoContributivoTotaleQuotaDCodGestione1, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria,
                                    datiPensione.NaturaPensione, montanteCodGestione1, montanteContributivoQuotaDCodGestione1, datiGenericiCi != null ? datiGenericiCi.DecorrenzaBonus : null, primaDecorrenzaImportiEsteri,
                                    datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null,
                                    sommaGEST_EST_61[indexContributi], sommaSettimaneContributiItalianiEdEsteri, sommaSettimaneDecUgualePrimaDec, datiPensione, out messaggioVideo))
                                    return false;
                            }
                            indexContributi++;
                        }
                    }
                    index++;
                }
            }

            if (!GestioneControlli.VerificaOpzioneContributiva(datiPensione, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null,
                datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneQuotaATotale, settimaneQuotaBTotale, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo - " + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaCapienzaSettimaneDL50392WithAssicurazione(datiPensione, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, decorrenza, settimaneRetributiveQuotaBCodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, datiPensione.AttivitaEconomica, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsSettimane707(datiPensione, listaDatiCalcoloRetributivo, listaDatiCalcoloContributivo, listaCodeGestioneCalcoloRetributivo, listaCodeGestioneCalcoloContributivo,
                datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo - " + messaggioVideo;
                return false;
            }

            if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                !Utility.GestioneRiduzioneRetributivaEnabled(datiPensione, isRiaperturaDomanda, listaDatiCalcoloContributivo, listaDatiCalcoloRetributivo))
            {
                if (datiGenericiCi.RiduzioneRetributiva || datiGenericiCi.RiduzioneRetributivaPercentuale.HasValue)
                {
                    messaggioVideo = "Dati Calcolo - La Riduzione Retributiva non può essere acquisita. E' necessario eliminare i Dati Istruttoria.";
                    return false;
                }
            }
            if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295WithQuotaC(datiPensione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, settimaneQuotaCTotale, out messaggioVideo))
                return false;

            //ENG - Rimosso controllo anche per le TRF
            //if (!GestioneControlli.ControlsFineAssicurazioneWithQuotaD(datiPensione, settimaneQuotaDTotale, out messaggioVideo))
            //    return false;

            if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295PerAPEPrecoci(datiPensione, datiGenericiCi != null ? datiGenericiCi.ContributiItalianiEdEsteriAl1295 : null, out messaggioVideo))
                return false;
            #endregion Dati Calcolo

            #region Importi Esteri
            if (listaImportiValuta != null && listaImportiValuta.Count > 0)
            {
                if (!GestioneControlli.VerificaImportiEsteriWithCausaCarico(datiPensione.CausaCarico, listaImportiValuta.First().ImportoPrestazioneEE, listaImportiValuta.First().DecorrenzaPrestazioneEE))
                {
                    messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                    return false;
                }

                if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
                {
                    bool flag = false;
                    foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEstera in listaPrestazioniEstere)
                    {
                        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                        if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                            listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEstera.Id);

                        flag = GestioneControlli.VerificaImportiEsteriWithPrestazioniEE((listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0) ? listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE : null, (listaImportiValuta != null && listaImportiValuta.Count > 0) ? listaImportiValuta.First().ImportoPrestazioneEE : null, (listaImportiValuta != null && listaImportiValuta.Count > 0) ? listaImportiValuta.First().DecorrenzaPrestazioneEE : null);

                        if (flag)
                            break;
                    }
                    if (!flag)
                    {
                        messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                        return false;
                    }

                    flag = false;
                    foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEstera in listaPrestazioniEstere)
                    {
                        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                        if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                            listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEstera.Id);

                        flag = GestioneControlli.VerificaImportiEsteriWithDecPrecLiquidata(datiPensione.CausaCarico, prestazioneEstera.DecorrenzaLiquidazioneStatoEE, listaImportiValuta.First().ImportoPrestazioneEE, listaImportiValuta.First().DecorrenzaPrestazioneEE);

                        if (flag)
                            break;
                    }
                    if (!flag)
                    {
                        messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                        return false;
                    }

                    if (!GestioneControlli.VerificaImportiEsteriWithConvenzione(datiPensione.CausaCarico, codiceConvenzione, listaResidenzeEstere, listaImportiValuta.First().ImportoPrestazioneEE, listaImportiValuta.First().DecorrenzaPrestazioneEE))
                    {
                        messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                        return false;
                    }
                }

                int index = 0;
                foreach (GestioneDatiContributiviCi.PensioniCiImportiValuta importoEstero in listaImportiValuta)
                {
                    if (!GestioneControlli.VerificaObbligatorietaImportiEsteri(importoEstero.ImportoPrestazioneEE, importoEstero.DecorrenzaPrestazioneEE))
                    {
                        messaggioVideo = "Importi Esteri: Decorrenza e/o Importo mancanti";
                        return false;
                    }

                    if (!GestioneControlli.VerificaDataDecorrenzaImportiEsteri(importoEstero.DecorrenzaPrestazioneEE))
                    {
                        messaggioVideo = "Importi Esteri: Decorrenza posteriore al 12/1992";
                        return false;
                    }

                    if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
                    {
                        if (index == 0)
                        {
                            bool flag = false;
                            foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEstera in listaPrestazioniEstere)
                            {
                                List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                                if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                                    listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEstera.Id);

                                flag = GestioneControlli.VerificaDecorrenzaImportiEsteriWithDecorrenzaPrestazioniEE(importoEstero.DecorrenzaPrestazioneEE, (listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0) ? listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE : null);

                                if (flag)
                                    break;
                            }
                            if (!flag)
                            {
                                messaggioVideo = "Decorrenza diversa da Decorrenza Estero";
                                return false;
                            }

                            foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEstera in listaPrestazioniEstere)
                            {
                                List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                                if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                                    listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEstera.Id);

                                if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithPrestazioniEE(importoEstero.DecorrenzaPrestazioneEE, (listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0) ? listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE : null, prestazioneEstera.DecorrenzaLiquidazioneStatoEE))
                                {
                                    messaggioVideo = "Decorrenza maggiore di Decorrenza Stato " + prestazioneEstera.CodiceStatoEE + " / " + prestazioneEstera.CodiceIstituzione;
                                    return false;
                                }
                            }
                        }
                    }

                    index++;
                }

                if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
                {
                    bool decIsGreaterThan91 = false;
                    bool decIsGreaterThan90 = false;
                    index = 0;
                    DateTime? dataMin = new DateTime(9999, 01, 01);

                    foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEstera in listaPrestazioniEstere)
                    {
                        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                        if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                            listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEstera.Id);

                        if (listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0)
                        {
                            if (!prestazioneEstera.DecorrenzaLiquidazioneStatoEE.HasValue)
                                if (Utility.DataSuccessivaA(listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE.Value, dataMin.Value))
                                    dataMin = listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE;

                            if (!decIsGreaterThan91)
                                decIsGreaterThan91 = Utility.DataSuccessivaA(listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE.Value, new DateTime(1991, 01, 01));
                            if (!decIsGreaterThan90)
                                decIsGreaterThan90 = Utility.DataSuccessivaA(listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE.Value, new DateTime(1990, 01, 01));
                        }
                    }

                    foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEstera in listaPrestazioniEstere)
                    {
                        if (prestazioneEstera.DecorrenzaLiquidazioneStatoEE.HasValue)
                        {
                            List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                            if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                                listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEstera.Id);

                            List<GestioneContrib.PensioniCiImportiValuta> listaImportiValutaGestioneContrib = new List<GestioneContrib.PensioniCiImportiValuta>();
                            foreach (GestioneDatiContributiviCi.PensioniCiImportiValuta importoValuta in listaImportiValuta)
                            {
                                GestioneContrib.PensioniCiImportiValuta importo = new GestioneContrib.PensioniCiImportiValuta();
                                importo.DecorrenzaPrestazioneEE = importoValuta.DecorrenzaPrestazioneEE;
                                importo.IdPensione = importoValuta.IdPensione;
                                importo.ImportoPrestazioneEE = importoValuta.ImportoPrestazioneEE;

                                listaImportiValutaGestioneContrib.Add(importo);
                            }

                            if (!GestioneControlli.ControlsDecorrenzaImportiEsteri(listaImportiValutaGestioneContrib, listaImportiEsteriPrestazione, dataMin, decIsGreaterThan90, decIsGreaterThan91, listaPrestazioniEstere.Count, index, out messaggioVideo))
                            {
                                messaggioVideo = "Importi Esteri: " + messaggioVideo;
                                return false;
                            }
                        }

                        index++;
                    }
                }
            }
            #endregion Importi Esteri

            #region Maternità/Acna
            if (listaMaternitaAcna != null && listaMaternitaAcna.Count > 0)
            {
                foreach (GestioneDatiContributiviCi.PensioniCiMaternitaAcna maternitaAcna in listaMaternitaAcna)
                {
                    if (maternitaAcna.Tipo == 'M') // Maternità
                    {
                        if (maternitaAcna.SettimaneAl1292.GetValueOrDefault() > 0 || maternitaAcna.SettimaneDL50392.GetValueOrDefault() > 0 || maternitaAcna.ImportoIVS.GetValueOrDefault() > 0)
                        {
                            if (!GestioneControlli.ControlsMaternita(maternitaAcna.SettimaneAl1292, maternitaAcna.SettimaneDL50392, sesso, decorrenza, maternitaAcna.ImportoIVS, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaMaternitaWithSettimane(maternitaAcna.SettimaneAl1292, maternitaAcna.SettimaneDL50392, settimaneRetributiveQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, settimaneContributiveCodGestione1, datiGenericiCi != null ? datiGenericiCi.VVMisuraAl1292 : null, datiGenericiCi != null ? datiGenericiCi.VVMisuraDL50392 : null, listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaMaternitaWithDatiCalcolo(maternitaAcna.SettimaneAl1292, settimaneRetributiveQuotaACodGestione1, rmsQuotaACodGestione1, maternitaAcna.SettimaneDL50392, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                                return false;
                        }
                    }

                    if (maternitaAcna.Tipo == 'A') // Acna
                    {
                        if (maternitaAcna.SettimaneAl1292.GetValueOrDefault() > 0 || maternitaAcna.SettimaneDL50392.GetValueOrDefault() > 0 || maternitaAcna.ImportoIVS.GetValueOrDefault() > 0)
                        {
                            if (!GestioneControlli.ControlsAcna(maternitaAcna.SettimaneAl1292, maternitaAcna.SettimaneDL50392, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, maternitaAcna.ImportoIVS, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaAcnaWithDatiAssicurativi(maternitaAcna.SettimaneAl1292, maternitaAcna.SettimaneDL50392, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaAcnaWithDatiCalcolo(maternitaAcna.SettimaneAl1292, maternitaAcna.SettimaneDL50392, settimaneRetributiveQuotaACodGestione1, rmsQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                                return false;
                        }
                    }
                }
            }
            #endregion Maternità/Acna
            #endregion DatiContributivi

            #region Maggiorazioni e Benefici
            #region Dati Benefici
            if (!GestioneControlli.ControlsNSettimaneIncremento1Percento(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimaneIncremento05Percento(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, datiAnagraficiTitolare.Sesso, out messaggioVideo))
                return false;

            if (datiDanteCausa != null)
            {
                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento1PercentoWithDanteCausa(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, datiDanteCausa.SiglaCategoria, datiDanteCausa.DecorrenzaPensione))
                {
                    messaggioVideo = "Settimane Incremento 1% incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithDecorrenzaDiretta(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, datiDanteCausa.SiglaCategoria, datiDanteCausa.DecorrenzaPensione))
                {
                    messaggioVideo = "Settimane Incremento 0.5% incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithSessoDanteCausa(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, datiAnagraficiDC.Sesso))
                {
                    messaggioVideo = "Settimane Incremento 0.5% incompatibili con Sesso del Titolare Dante Causa";
                    return false;
                }
            }

            #region Categorie minori o uguali a 6
            if (categoria > 0 && categoria <= 6)
            {
                if (!GestioneControlli.VerificaCapienzaNSettimaneIncrementoPercentuale(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, datiPensione.FineAssicurazione, out messaggioVideo))
                {
                    messaggioVideo = "Dati Benefici: " + messaggioVideo;
                    return false;
                }
            }
            #endregion Categorie minori o uguali a 6

            if (!GestioneCrossControls.CI_ControlsTipoBeneficioArt24Comma15Bis(datiPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, datiAnagraficiTitolare.Sesso,
                datiAnagraficiTitolare.DataNascita, datiIstruttoria, datiGenericiCi, listaPrestazioniEstere, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlsBeneficioPrecoci(datiPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_ControlsLavoratoriNonVedenti(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                 datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.SettAnzContribPost311295 : null, datiPensione, datiDanteCausa,
                         out messaggioVideo))
                return false;

            #endregion Dati Benefici

            #region Dati Maggiorazioni
            if (datiMaggiorazioniBenefici != null)
            {
                if (datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
                {
                    if (Utility.DataStrettamenteSuccessivaA(new DateTime(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value.Year, datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value.Month, 1),
                        new DateTime(dataSistema.AddMonths(1).Year, dataSistema.AddMonths(1).Month, 1)))
                    {
                        messaggioVideo = "La decorrenza maggiorazione sociale non può essere superiore di 1 mese dalla data odierna tenendo conto solo del mese ed anno";
                        return false;
                    }

                    if (!GestioneControlli.VerificaDecorrenzaMaggiorazioneSocialeWithDecorrenzaOriginaria(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, datiPensione.DecorrenzaOriginaria))
                    {
                        messaggioVideo = "Decorrenza L.544/1 anteriore a Decorrenza Originaria, o 07/1988";
                        return false;
                    }

                    if (datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue)
                    {
                        if (Utility.DataStrettamenteSuccessivaA(datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value, dataSistema.AddMonths(1)))
                        {
                            messaggioVideo = "Cessazione L.544/1 illogica o posteriore data odierna";
                            return false;
                        }

                        if (!Utility.DataSuccessivaA(datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value, datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value))
                        {
                            messaggioVideo = "Cessazione L.544/1 anteriore a Decorrenza L.544/1";
                            return false;
                        }
                    }

                    if (!GestioneControlli.ControlsMaggiorazioniWithEtaPensionabile(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, datiAnagraficiTitolare.DataNascita, datiPensione.Gruppo, datiPensione.CausaCarico, datiPensione, out messaggioVideo))
                        return false;
                }
                else
                {
                    if (datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue)
                    {
                        messaggioVideo = "Cessazione L.544/1 illogica (Decorrenza mancante)";
                        return false;
                    }
                }

                if (!GestioneControlli.VerificaDecorrenzaMaggiorazioneLegge140(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneLegge140, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaDecorrenzaMaggiorazioneLegg140WithEtaPensionabile(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneLegge140, tipoDomanda, datiPensione.CausaCarico, datiAnagraficiTitolare.DataNascita, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaAnniRiduzioneBeneficiArt38Legge02(datiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02, tipoDomanda, datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCodiceRequisitiLegge50392(codiceRequisitiLegge50392TraduzioneSuGP, datiPensione.DecorrenzaOriginaria, tipoDomanda, categoria, datiAnagraficiTitolare.DataNascita, datiAnagraficiTitolare.Sesso, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCodiceRequisitiLegge50392WithInvalidita(codiceRequisitiLegge50392TraduzioneSuGP, datiPensione.Gruppo, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (datiMaggiorazioniBenefici != null && !GestioneCrossControls.ALL_ControlsDecorrenzaMaggiorazioneWithDataPresentazione(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, datiPensione, datiAnagraficiTitolare != null ? datiAnagraficiTitolare.DataNascita : null,
                    datiStoricoGP != null ? datiStoricoGP.DecorrenzaMaggiorazioneSociale.HasValue : false, datiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Maggiorazione Benefici/Maggiorazione:<br/>" + messaggioVideo;
                    return false;
                }
            }
            #endregion Dati Maggiorazioni

            #region Dati Cieco/Ex Combattente
            if (!GestioneControlli.ControlsDecorrenzaMaggiorazioneArt6(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDecorrenzaOriginaria(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Decorrenza art.6 L.140/544 antecedente a Decorrenza Diretta o 01/85";
                return false;
            }

            if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDataMorteAndDecorrenzaOriginaria(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Decorrenza art.6 L.140/544 incompatibile con Data Morte/Decorrenza Diretta";
                return false;
            }

            if (!GestioneControlli.VerificaCodiceCiecoArt6(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Maggiorazioni Benefici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaArt6(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Maggiorazioni Benefici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaCodiceCiecoWithDecorrenza(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.CodiceCieco : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Maggiorazioni Benefici:<br/>" + messaggioVideo;
                return false;
            }

            if (datiMaggiorazioniBenefici != null && !GestioneCrossControls.ALL_ControlsDecorrenzaExCombattenteWithDataPresentazione(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6, datiPensione, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Maggiorazione Benefici/Cieco ExCombattente:<br/>" + messaggioVideo;
                return false;
            }


            #endregion Dati Cieco/Ex Combattente
            #endregion Maggiorazioni e Benefici

            #region Supplementi
            if (listaDatiSupplementi != null && listaDatiSupplementi.Count > 0)
            {
                if (!GestioneCrossControls.CI_VerificaSupplementi(listaDatiSupplementi, datiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Supplementi:<br/>" + messaggioVideo;
                    return false;
                }
            }
            #endregion Supplementi

            #region Deleghe/Tutele
            if (!GestioneCrossControls.ALL_VerificaDelegheTuteleByIdPensione(datiPensione, datiDelegato != null ? datiDelegato.CodiceFiscale : string.Empty,
                datiTutore != null ? datiTutore.CodiceFiscale : string.Empty,
                datiTutore != null ? datiTutore.CodiceTutore : (char?)null,
                datiTutore != null ? datiTutore.CessValAmmSost : (DateTime?)null, datiAnagraficiTitolare.CodiceFiscale, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (Utility.DataStrettamenteSuccessivaA(datiAnagraficiTitolare.DataNascita.Value.AddYears(18), dataSistema) && (datiTutore == null || !datiTutore.CodiceTutore.HasValue))
            {
                messaggioVideo = "Titolare minorenne: inserire codice Tutore / Amminis. di sostegno";
                return false;
            }
            #endregion Deleghe/Tutele

            #region Oneri
            if (!GestioneCrossControls.ALL_VerificaOneri(datiPensione, listaDatiOneri, codiceParticolareSoggettoDerogato != null ? codiceParticolareSoggettoDerogato.TraduzioneSuGp : null, isRiaperturaDomanda, datiAnagraficiTitolare, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Oneri - Oneri:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaPresenzaOneriObbligatori(datiPensione, isRiaperturaDomanda, listaDatiOneri, elencoDecCodeGruppoOneri, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Oneri / Oneri:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Oneri

            #region Usuranti
            if (!GestioneCrossControls.ALL_VerificaCodNaturaUsuranti(datiPensione, out messaggioVideo))
                return false;
            #endregion Usuranti

            #region Bititolarita
            List<string> listaCodiciNatura = new List<string> { "2", "4", "5", "6", "9" };
            if (!listaCodiciNatura.Contains(datiPensione.NaturaPensione.Substring(0, 1)))
            {
                if (listaAltrePensioni != null && listaAltrePensioni.Count > 0)
                {
                    messaggioVideo = "Non è possibile acquisire le bititolarità se il primo codice natura è pari a '" + (!string.IsNullOrEmpty(datiPensione.NaturaPensione) ? datiPensione.NaturaPensione.Substring(0, 1) : " ") + "'";
                    return false;
                }
            }
            else
            {
                //if (listaAltrePensioni != null && listaAltrePensioni.Count > 0)
                //{
                //    foreach (Entity.AltraPensione altraPensione in listaAltrePensioni)
                //    {
                //        if (!GestioneControlli.CI_ControlsCategoriaWithCodiceEnteAltraPensione(altraPensione.Categoria, altraPensione.Ente, out messaggioVideo))
                //            return false;

                //        if (!GestioneControlli.CI_ControlsCategoriaWithCodiceUCAltraPensione(altraPensione.Categoria, altraPensione.CodiceUC, out messaggioVideo))
                //            return false;

                //        if (!GestioneControlli.CI_ControlsCategoriaWithCodiceImportoAltraPensione(altraPensione.Categoria, altraPensione.CodiceImporto, out messaggioVideo))
                //            return false;
                //    }
                //}

                if (!GestioneControlli.VerificaAltraPensioneWithCategoriaPensione(listaAltrePensioni, categoriaNumerica, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaAltraPensioneWithNaturaPensione(listaAltrePensioni, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;
            }

            #endregion Bititolarita

            #region Eliminazione
            if (datiEliminazione != null)
            {
                if (!GestioneCrossControls.AGO_CI_ControlsDatiEliminazione(datiPensione, datiEliminazione.CodiceMotivo, datiEliminazione.DecorrenzaEliminazione, datiEliminazione.DataEvento,
                    datiEliminazione.DataFineCalcoloArretrati, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagProvvisoria : null,
                    datiPagamento != null ? datiPagamento.DataRinunciaTrattenutaInpdap : null, datiIstruttoria != null ? datiIstruttoria.ScadenzaRevisioneSanitaria : null,
                    datiPensione.DecorrenzaCalcoloArretrati, isRiaperturaDomanda, datiDanteCausa, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Eliminazione:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.ALL_VerificaDecorrenzaEliminazioneWithRedditi(lstRedditi, datiEliminazione.DataEvento, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Eliminazione:<br/>" + messaggioVideo;
                    return false;
                }
            }
            #endregion Eliminazione

            #region Modalità Pagamento
            if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (!GestioneCrossControls.ALL_ControlsBancaItalia(datiPensione, datiPagamento.ABI, datiPagamento.CAB, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Modalità Pagamento:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneCrossControls.ALL_ControlsDatiPagamento(datiPagamento.TipoPagamento, datiPagamento.ModalitaPagamento, datiPagamento.IBAN, datiPagamento.Libretto, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Modalità Pagamento:<br/>" + messaggioVideo;
                    return false;
                }
            }
            #endregion Modalità Pagamento

            return true;
        }

        private static bool ControlsDatiFamiliari(GestionePensione.DatiPensione datiPensione, DateTime dataSistema, int annoCompetenza, bool isRiaperturaDomanda, List<GestioneFamiliari.Familiare> listaFamiliari, Utility.TipoAppartenenza? tipoAppartenenza, List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (listaFamiliari != null && listaFamiliari.Count > 0 && listaCodMaggFamiliari != null && listaCodMaggFamiliari.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    List<GestioneFamiliari.CodMaggFamiliari> LcodMaggFam = listaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica);
                    if (!GestioneCrossControls.ALL_VerificaDecorrenzaCessazioneFamiliari(datiPensione, tipoAppartenenza, fam, LcodMaggFam))
                    {
                        messaggioVideo = "Non è consentito l'inserimento del 'SI' diritto da 03/2022 a nessun familiare che abbia sigla U, S, M, L. Cambiare codice maggiorazione o data inizio/fine carico";
                        return false;
                    }
                }
            }

            //ENG - Memo 22/2024
            if (!GestioneCrossControls.ALL_VerificaPlurimeRegistrazioniConiugeUnitoCivile(datiPensione, tipoAppartenenza, listaFamiliari, listaCodMaggFamiliari, out messaggioVideo))
            {
                return false;
            }

            return true;
        }

        public static void CalcolaDomandaNew(GestionePensione.DatiPensione datiPensione, long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = string.Empty;
            messaggioVideo = string.Empty;

            if (datiPensione == null)
                throw new INPS.DNA.DnaApplicationException("Nessuna pensione associata al numero di domanda: " + numeroDomanda);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            object AreaCalcolo = null;
            ValorizzaAreaCalcoloNew(matricolaOperatore, sedeOperatore, centroOperativoOperatore, datiPensione, tipoDomanda, isRiaperturaDomanda, out AreaCalcolo);
            // Il salvataggio del LogSOAP viene fatto dentro EseguiCalcolo
            EseguiCalcoloNew(AreaCalcolo, tipoDomanda, isRiaperturaDomanda, datiPensione.NDomus);
            ControllaEsitoCalcoloNew(datiPensione.NDomus, datiPensione.ProgStorico, AreaCalcolo, tipoDomanda, isRiaperturaDomanda, out statoPensione, out esito, out messaggioVideo);
        }

        #endregion public members

        #region private members
        private static void ValorizzaAreaCalcolo(string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, GestionePensione.DatiPensione datiPensione,
            Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda, out object AreaCalcolo)
        {
            AreaCalcolo = null;
            Data.HostRequest.CI01_CI02Request richiesta = null;

            MappingVersoHost.ValorizzaRichiesta(matricolaOperatore, sedeOperatore, datiPensione, out richiesta);

            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(sedeOperatore.ToString().PadLeft(4, '0') + centroOperativoOperatore.ToString().PadLeft(2, '0'));

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                AreaCalcolo = new INPS.Pensioni.LiquidazioneCi.Data.CI02(richiesta.Gruppo1, richiesta.Gruppo2, richiesta.Gruppo3, richiesta.Gruppo4);
            else
                AreaCalcolo = new INPS.Pensioni.LiquidazioneCi.Data.CI01(richiesta.Gruppo1, richiesta.Gruppo2, richiesta.Gruppo3, richiesta.Gruppo4);
        }

        private static void EseguiCalcolo(object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda, long numeroDomanda)
        {
            Guid guid = Guid.NewGuid();

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
            {
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI02, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);

                ((Data.CI02)AreaCalcolo).Invoke();

                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI02, Utility.SOAPLogDirection.OUT, numeroDomanda.ToString(), guid);
            }
            else
            {
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI01, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);

                ((Data.CI01)AreaCalcolo).Invoke();

                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI01, Utility.SOAPLogDirection.OUT, numeroDomanda.ToString(), guid);
            }
        }

        private static void ControllaEsitoCalcolo(long numeroDomanda, byte? progStorico, object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda, out string statoPensione, out bool esito,
            out string messaggioVideo)
        {
            esito = false;
            statoPensione = null;
            DateTime dataSistema = Utility.DataSistemaCi;
            string messaggioDaLoggare = null;
            char? flag5000 = null;

            //// Questa Get viene eseguita per evitare di avere dati sporchi modificati dalla valorizzazione area calcolo
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                messaggioDaLoggare = ((Data.CI02)AreaCalcolo).MessaggioDaLoggare;
            else
                messaggioDaLoggare = ((Data.CI01)AreaCalcolo).MessaggioDaLoggare;
            if (!string.IsNullOrEmpty(messaggioDaLoggare))
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggioDaLoggare, null, null);

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                messaggioVideo = ((Data.CI02)AreaCalcolo).Messaggio;
            else
                messaggioVideo = ((Data.CI01)AreaCalcolo).Messaggio;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoCalcolo.DatiEsitoCalcolo datiEsitoCalcolo = new GestioneEsitoCalcolo.DatiEsitoCalcolo();

                if (((Data.CI02)AreaCalcolo).Response.RecordZeroCentro != null && ((Data.CI02)AreaCalcolo).Response.RecordZeroCentro.FLAG_INDEB != null && ((Data.CI02)AreaCalcolo).Response.RecordZeroCentro.FLAG_INDEB.Trim() != "0")
                    datiPensione.FlagIndebito = ((Data.CI02)AreaCalcolo).Response.RecordZeroCentro.FLAG_INDEB;

                if (AreaCalcolo != null && (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda) ? ((Data.CI02)AreaCalcolo).Response != null : ((Data.CI01)AreaCalcolo).Response != null)
                {
                    string codErrore = "";
                    if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        codErrore = ((Data.CI02)AreaCalcolo).Response.RecordZeroCentro != null ? ((Data.CI02)AreaCalcolo).Response.RecordZeroCentro.RIS_COD_ER1 : "";
                    else
                        codErrore = ((Data.CI01)AreaCalcolo).Response.RecordZeroCentro != null ? ((Data.CI01)AreaCalcolo).Response.RecordZeroCentro.RIS_COD_ER1 : "";

                    GestioneDecodifica.ErroreCalcoloCi erroreCalcoloCi = null;
                    GestioneDecodifica.GetErroreCalcoloCiByCode(codErrore, out erroreCalcoloCi);
                    if (erroreCalcoloCi == null && codErrore != "" && codErrore != "\0\0\0")
                    {
                        int res = 0;
                        if (int.TryParse(codErrore.Trim(), out res))
                        {
                            if (res == 669 || res == 668 || res == 681 || res == 682)
                                messaggioVideo = codErrore + GetErrore669(AreaCalcolo, tipoDomanda, isRiaperturaDomanda);
                            else if (res > 600 && res < 700)
                                messaggioVideo = codErrore + " Errore Pgm calcolo";
                            else if (res > 703 && res < 707)
                                messaggioVideo = codErrore + " Incompatibilita' dati coniuge";
                            else
                                messaggioVideo = codErrore + ScompattaErroreSuControlli(AreaCalcolo, tipoDomanda, isRiaperturaDomanda);
                        }
                        else
                            messaggioVideo = codErrore + ScompattaErroreSuControlli(AreaCalcolo, tipoDomanda, isRiaperturaDomanda);
                    }
                    else if (erroreCalcoloCi != null)
                    {
                        messaggioVideo = erroreCalcoloCi.Codice + " " + erroreCalcoloCi.Descrizione;

                        int res = 0;
                        if (int.TryParse(codErrore.Trim(), out res))
                        {
                            if (res == 837)
                            {
                                flag5000 = '2';
                            }
                            else if (res == 867)
                            {
                                flag5000 = '1';
                            }
                        }
                    }
                    else
                        messaggioVideo = "Calcolo eseguito correttamente";

                    datiEsitoCalcolo.DettaglioEsito = messaggioVideo;

                    switch (codErrore)
                    {
                        case "":
                        case "\0\0\0":
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //CALCOLATA
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoStazLavoro;
                            else
                                //CALCOLO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcoloVerify;
                            datiPensione.DataElaborazione = dataSistema;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = true;
                            break;
                        default:
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                            //SCARTO DA CALCOLO
                            {
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoDaCalcolo;
                                datiPensione.Flag5000 = flag5000;
                            }
                            else
                                //SCARTO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoVerify;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = false;
                            break;
                    }
                }
                if (esito)
                {
                    datiEsitoCalcolo.Esito = "OK";
                    if (datiPensione.StatoPensione == (int)Utility.StatoPensione.CalcolataNoStazLavoro)
                        GestioneLogGenerico.EliminaLogGenerico(numeroDomanda);
                }
                else
                    datiEsitoCalcolo.Esito = "KO";

                GestioneEsitoCalcolo.SalvaEsitoCalcolo(datiPensione.Id, datiEsitoCalcolo);
                transactionScope.Complete();
            }

            GestioneDecodifica.GetStatoPensioneById(datiPensione.StatoPensione.Value, out statoPensione);
        }

        private static string GetErrore669(object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda)
        {
            string errore = string.Empty;
            List<Data.HostResponse.AreaRecordZeroCentro.Risposta669> ListaErrori = null;
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                ListaErrori = ((Data.CI02)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;
            else
                ListaErrori = ((Data.CI01)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;

            if (ListaErrori != null && ListaErrori.Count > 0)
            {
                errore = " ";
                foreach (Data.HostResponse.AreaRecordZeroCentro.Risposta669 err in ListaErrori)
                {
                    int anno = 0;
                    int.TryParse(err.RIS_669_ANN.Trim(), out anno);
                    switch (err.RIS_669_COD)
                    {
                        case "2":
                            errore = string.Concat(errore, anno >= 1980 ? " Anno:" + anno.ToString() + " " : string.Empty, "Redd.pens. ", err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, " : dati attualizzati.");
                            break;
                        case "3":
                            errore = string.Concat(errore, anno >= 1980 ? " Anno:" + anno.ToString() + " " : string.Empty, "Redd.pens. ", err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, " : scorporo fiscale mancato.");
                            break;
                        case "9":
                            errore = string.Concat(errore, anno >= 1980 ? " Anno:" + anno.ToString() + " " : string.Empty, "Redd.pens. ", err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, " : dati attualizzati.");
                            break;
                    }
                }
            }
            return errore;
        }

        private static string GetErrore669New(object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda)
        {
            string errore = string.Empty;
            List<Data.HostResponse.AreaRecordZeroCentro.Risposta669> ListaErrori = null;
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                ListaErrori = ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;
            else
                ListaErrori = ((Data.CI01New)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;

            if (ListaErrori != null && ListaErrori.Count > 0)
            {
                errore = " ";
                foreach (Data.HostResponse.AreaRecordZeroCentro.Risposta669 err in ListaErrori)
                {
                    int anno = 0;
                    int.TryParse(err.RIS_669_ANN.Trim(), out anno);
                    switch (err.RIS_669_COD)
                    {
                        case "2":
                            errore = string.Concat(errore, anno >= 1980 ? " Anno:" + anno.ToString() + " " : string.Empty, "Redd.pens. ", err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, " : dati attualizzati.");
                            break;
                        case "3":
                            errore = string.Concat(errore, anno >= 1980 ? " Anno:" + anno.ToString() + " " : string.Empty, "Redd.pens. ", err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, " : scorporo fiscale mancato.");
                            break;
                        case "9":
                            errore = string.Concat(errore, anno >= 1980 ? " Anno:" + anno.ToString() + " " : string.Empty, "Redd.pens. ", err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, " : dati attualizzati.");
                            break;
                    }
                }
            }
            return errore;
        }

        private static string ScompattaErroreSuControlli(object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda)
        {
            string errore = string.Empty;

            List<Data.HostResponse.AreaRecordZeroCentro.Risposta669> ListaErrori = null;
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                ListaErrori = ((Data.CI02)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;
            else
                ListaErrori = ((Data.CI01)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;

            if (ListaErrori != null && ListaErrori.Count > 0)
            {
                errore = " ";
                foreach (Data.HostResponse.AreaRecordZeroCentro.Risposta669 err in ListaErrori)
                    errore = string.Concat(errore, err.RIS_669_ANN, err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, err.RIS_669_COD);
            }

            if (string.IsNullOrEmpty(errore))
                errore = " Errore nel calcolo non codificato";
            return errore;
        }

        private static string ScompattaErroreSuControlliNew(object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda)
        {
            string errore = string.Empty;

            List<Data.HostResponse.AreaRecordZeroCentro.Risposta669> ListaErrori = null;
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                ListaErrori = ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;
            else
                ListaErrori = ((Data.CI01New)AreaCalcolo).Response.RecordZeroCentro.RISPOSTE669;

            if (ListaErrori != null && ListaErrori.Count > 0)
            {
                errore = " ";
                foreach (Data.HostResponse.AreaRecordZeroCentro.Risposta669 err in ListaErrori)
                    errore = string.Concat(errore, err.RIS_669_ANN, err.RIS_669_KEY_C, err.RIS_669_KEY_S, err.RIS_669_KEY_N, err.RIS_669_COD);
            }

            if (string.IsNullOrEmpty(errore))
                errore = " Errore nel calcolo non codificato";
            return errore;
        }


        private static bool ControlsConsultazioneANF(GestionePensione.DatiPensione datiPensione, List<GestioneFamiliari.Familiare> listaFamiliari,
            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggiorazione, DateTime dataSistema, string matricolaOperatore,
            out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioni, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            listaConsultazioni = null;
            List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> listaRichieste = null;
            GestioneFamiliari.GetRichiesteRicercaDomandeANFByIdPensione(datiPensione.Id, out listaRichieste);
            if (listaRichieste != null && listaRichieste.Count > 0 && listaFamiliari != null && listaFamiliari.Count > 0)
            {
                listaConsultazioni = new List<GestioneFamiliari.ConsultazioneUnificataANF>();
                foreach (GestioneFamiliari.DatiRichiestaRicercaDomandeANF richiesta in listaRichieste)
                {
                    string codiceFiscale = listaFamiliari.FirstOrDefault(x => x.IdAnagrafica == richiesta.IdAnagrafica).CodiceFiscale;
                    string rispostaConsultazione = string.Empty;
                    GestioneFamiliari.ConsultazioneUnificataANF consultazioneANF = null;
                    if (!GestioneANF.RichiediRispostaById(datiPensione.NDomus.ToString(), codiceFiscale, richiesta.Guid, matricolaOperatore, out rispostaConsultazione, out messaggioVideo))
                    {
                        listaConsultazioni = null;
                        return false;
                    }
                    if (!GestioneFamiliari.ControllaRispostaANF(rispostaConsultazione, out consultazioneANF, out messaggioVideo))
                    {
                        listaConsultazioni = null;
                        return false;
                    }
                    if (consultazioneANF != null)
                        listaConsultazioni.Add(consultazioneANF);
                }
            }
            return true;
        }

        private static void ValorizzaAreaCalcoloNew(string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, GestionePensione.DatiPensione datiPensione,
            Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda, out object AreaCalcolo)
        {
            AreaCalcolo = null;
            Data.HostRequest.CI01_CI02RequestNew richiesta = null;

            MappingVersoHostNew.ValorizzaRichiesta(matricolaOperatore, sedeOperatore, datiPensione, out richiesta);

            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(sedeOperatore.ToString().PadLeft(4, '0') + centroOperativoOperatore.ToString().PadLeft(2, '0'));

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                AreaCalcolo = new INPS.Pensioni.LiquidazioneCi.Data.CI02New(richiesta.Gruppo1, richiesta.Gruppo2, richiesta.Gruppo3, richiesta.Gruppo4, richiesta.Gruppo5);
            else
                AreaCalcolo = new INPS.Pensioni.LiquidazioneCi.Data.CI01New(richiesta.Gruppo1, richiesta.Gruppo2, richiesta.Gruppo3, richiesta.Gruppo4, richiesta.Gruppo5);
        }

        private static void EseguiCalcoloNew(object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda, long numeroDomanda)
        {
            Guid guid = Guid.NewGuid();

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
            {
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI02, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);

                ((Data.CI02New)AreaCalcolo).Invoke();

                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI02, Utility.SOAPLogDirection.OUT, numeroDomanda.ToString(), guid);
            }
            else
            {
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI01, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);

                ((Data.CI01New)AreaCalcolo).Invoke();

                GestioneLogSoap.SalvaLogSoap(AreaCalcolo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI01, Utility.SOAPLogDirection.OUT, numeroDomanda.ToString(), guid);
            }
        }

        private static void ControllaEsitoCalcoloNew(long numeroDomanda, byte? progStorico, object AreaCalcolo, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda, out string statoPensione, out bool esito,
            out string messaggioVideo)
        {
            esito = false;
            statoPensione = null;
            DateTime dataSistema = Utility.DataSistemaCi;
            string messaggioDaLoggare = null;
            char? flag5000 = null;

            //// Questa Get viene eseguita per evitare di avere dati sporchi modificati dalla valorizzazione area calcolo
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                messaggioDaLoggare = ((Data.CI02New)AreaCalcolo).MessaggioDaLoggare;
            else
                messaggioDaLoggare = ((Data.CI01New)AreaCalcolo).MessaggioDaLoggare;
            if (!string.IsNullOrEmpty(messaggioDaLoggare))
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggioDaLoggare, null, null);

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                messaggioVideo = ((Data.CI02New)AreaCalcolo).Messaggio;
            else
                messaggioVideo = ((Data.CI01New)AreaCalcolo).Messaggio;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoCalcolo.DatiEsitoCalcolo datiEsitoCalcolo = new GestioneEsitoCalcolo.DatiEsitoCalcolo();

                if (AreaCalcolo != null && (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda) ? ((Data.CI02New)AreaCalcolo).Response != null : ((Data.CI01New)AreaCalcolo).Response != null)
                {
                    string codErrore = "";
                    if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        codErrore = ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro != null ? ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro.RIS_COD_ER1 : "";
                    else
                        codErrore = ((Data.CI01New)AreaCalcolo).Response.RecordZeroCentro != null ? ((Data.CI01New)AreaCalcolo).Response.RecordZeroCentro.RIS_COD_ER1 : "";

                    if ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda) && ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro != null && ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro.FLAG_INDEB != null && ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro.FLAG_INDEB.Trim() != "0")
                        datiPensione.FlagIndebito = ((Data.CI02New)AreaCalcolo).Response.RecordZeroCentro.FLAG_INDEB;

                    GestioneDecodifica.ErroreCalcoloCi erroreCalcoloCi = null;
                    GestioneDecodifica.GetErroreCalcoloCiByCode(codErrore, out erroreCalcoloCi);
                    if (erroreCalcoloCi == null && codErrore != "" && codErrore != "\0\0\0")
                    {
                        int res = 0;
                        if (int.TryParse(codErrore.Trim(), out res))
                        {
                            if (res == 669 || res == 668 || res == 681 || res == 682)
                                messaggioVideo = codErrore + GetErrore669New(AreaCalcolo, tipoDomanda, isRiaperturaDomanda);
                            else if (res > 600 && res < 700)
                                messaggioVideo = codErrore + " Errore Pgm calcolo";
                            else if (res > 703 && res < 707)
                                messaggioVideo = codErrore + " Incompatibilita' dati coniuge";
                            else
                                messaggioVideo = codErrore + ScompattaErroreSuControlliNew(AreaCalcolo, tipoDomanda, isRiaperturaDomanda);
                        }
                        else
                            messaggioVideo = codErrore + ScompattaErroreSuControlliNew(AreaCalcolo, tipoDomanda, isRiaperturaDomanda);
                    }
                    else if (erroreCalcoloCi != null)
                    {
                        messaggioVideo = erroreCalcoloCi.Codice + " " + erroreCalcoloCi.Descrizione;

                        int res = 0;
                        if (int.TryParse(codErrore.Trim(), out res))
                        {
                            if (res == 837)
                            {
                                flag5000 = '2';
                            }
                            else if (res == 867)
                            {
                                flag5000 = '1';
                            }
                        }
                    }
                    else
                        messaggioVideo = "Calcolo eseguito correttamente";

                    datiEsitoCalcolo.DettaglioEsito = messaggioVideo;

                    switch (codErrore)
                    {
                        case "":
                        case "\0\0\0":
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //CALCOLATA
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoStazLavoro;
                            else
                                //CALCOLO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcoloVerify;
                            datiPensione.DataElaborazione = dataSistema;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = true;
                            break;
                        default:
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                            //SCARTO DA CALCOLO
                            {
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoDaCalcolo;
                                datiPensione.Flag5000 = flag5000;
                            }
                            else
                                //SCARTO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoVerify;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = false;
                            break;
                    }
                }
                if (esito)
                {
                    datiEsitoCalcolo.Esito = "OK";
                    if (datiPensione.StatoPensione == (int)Utility.StatoPensione.CalcolataNoStazLavoro)
                        GestioneLogGenerico.EliminaLogGenerico(numeroDomanda);
                }
                else
                    datiEsitoCalcolo.Esito = "KO";

                GestioneEsitoCalcolo.SalvaEsitoCalcolo(datiPensione.Id, datiEsitoCalcolo);
                transactionScope.Complete();
            }

            GestioneDecodifica.GetStatoPensioneById(datiPensione.StatoPensione.Value, out statoPensione);
        }
        #endregion private members
    }
}
