using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneCi.Entity;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneLiquidazionePensione
    {

        public static void GetLiquidazionePensione(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, bool isRiaperturaDomanda, out Entity.DatiGenerici datiTabGenerici, out Entity.DatiAssicurativi datiTabAssicurativi, out Entity.DatiIstruttoria datiTabIstruttoria, out Entity.DatiOpzione datiTabOpzione, out Entity.DatiProvenienza datiTabProvenienza, out List<Entity.DatiInail> listaDatiTabInail)
        {
            datiTabGenerici = null;
            GetDatiGenerici(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiTabGenerici);
            datiTabAssicurativi = null;
            GetDatiAssicurativi(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiTabAssicurativi);
            datiTabIstruttoria = null;
            GetDatiIstruttoria(datiPensione, datiIstruttoriaCommon, out datiTabIstruttoria);
            datiTabOpzione = null;
            GetDatiOpzione(datiPensione.Id, datiIstruttoriaCommon, out datiTabOpzione);
            datiTabProvenienza = null;
            GetDatiProvenienza(datiPensione.Id, datiIstruttoriaCommon, out datiTabProvenienza);
            //ENG - Reversibilità: campi Inail
            listaDatiTabInail = null;
            GetDatiInail(datiPensione.Id, out listaDatiTabInail);
        }

        //ENG - Reversibilità: campi Inail
        public static void GetDatiInail(long idPensione, out List<Entity.DatiInail> listaDatiTabInail)
        {
            listaDatiTabInail = null;
            List<GestionePensioneInailInabilita.DatiPensioniINAIL> datiPensioneInailDB = null;
            GestionePensioneInailInabilita.GetPensioniINAILByIdPensione(idPensione, out datiPensioneInailDB);

            if (datiPensioneInailDB != null && datiPensioneInailDB.Count > 0)
            {
                listaDatiTabInail = new List<Entity.DatiInail>();

                foreach (GestionePensioneInailInabilita.DatiPensioniINAIL inailTemp in datiPensioneInailDB)
                {
                    Entity.DatiInail inailEntity = new Entity.DatiInail();
                    Utility.ValorizzaOggetti(inailTemp, inailEntity);
                    listaDatiTabInail.Add(inailEntity);
                }
            }

        }

        #region dati Generici

        public static bool ControlDatiGenerici(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, GestionePensione.DatiEliminazione datiEliminazione,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, Entity.DatiGenerici datiGenerici, Entity.DatiAssicurativi datiAssicurativi,
            Entity.DatiOpzione datiOpzione, Entity.DatiIstruttoria datiIstruttoria, Entity.DatiProvenienza datiProvenienza, Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici,
            Entity.DatiMaggiorazioni datiMaggiorazioni, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;

            List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareDB = null;
            GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareDB);

            GestioneDecodifica.CodiceParticolare codiceParticolareSoggettoDerogato = null;

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

            if (IsSingleTab)
            {
                GetDatiAssicurativi(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiAssicurativi);
                GetDatiOpzione(datiPensione.Id, datiIstruttoriaCommon, out datiOpzione);
                GetDatiIstruttoria(datiPensione, datiIstruttoriaCommon, out datiIstruttoria);
            }
            else
            {
                if (datiIstruttoria != null && datiIstruttoria.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoria.IsDatiIstruttoriaDatiGenericiNull())
                    datiIstruttoria = null;

                if (datiOpzione != null && datiOpzione.IsDatiOpzioneIstruttoriaNull() && datiOpzione.IsDatiOpzionePensioniCiDatiGenericiNull())
                    datiOpzione = null;

                if (datiAssicurativi != null && datiAssicurativi.IsDatiAssicurativiIntegrazioneArt11Null() && datiAssicurativi.IsDatiAssicurativiIstruttoriaNull() &&
                    datiAssicurativi.IsDatiAssicurativiPensioneCiGenericiNull() && datiAssicurativi.IsDatiAssicurativiPensioneNull())
                    datiAssicurativi = null;
            }

            if (!ControlsDatiGenericiForMaggBenefici(datiExCombattente, datiBenefici, datiMaggiorazioni, datiGenerici.ExCombattente, datiGenerici.Benefici, datiGenerici.Maggiorazioni, false, out messaggioVideo))
                return false;

            if (!ControlsDatiGenericiForPensioneProvenienza(datiIstruttoriaCommon, datiGenerici.TrasformazioneAOI, datiGenerici.CausaCarico, datiPensione, false))
            {
                messaggioVideo = "Eliminare i dati Pensione di Provenienza prima di procedere con il salvataggio dei dati Generici";
                return false;
            }

            if (!ControlsDatiGenericiForBititolaritaAltraPensioneByIdPensione(datiPensione.Id, datiGenerici.NaturaPensione, false))
            {
                messaggioVideo = "Eliminare i dati 'Altra Pensione' nel quadro 'Bititolarità' prima di procedere";
                return false;
            }

            if (!ControlsCrossDatiGenerici(datiGenerici, datiAssicurativi, datiOpzione, datiPensione, datiIstruttoriaCommon, datiMaggiorazioniBeneficiCommon, datiIstruttoria, datiProvenienza, dataSistema,
                IsSingleTab, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (datiGenerici.DataInizioCalcolo.HasValue)
            {
                if (datiGenerici.DataInizioCalcolo.Value.CompareTo(datiPensione.DecorrenzaOriginaria) < 0)
                {
                    messaggioVideo = "Data Ripristino anteriore a Decorrenza Originaria";
                    return false;
                }

                if (datiGenerici.DataInizioCalcolo.Value.CompareTo(dataSistema.AddMonths(1).AddDays(-dataSistema.Day + 1)) > 0)
                {
                    messaggioVideo = "Decorrenza Calcolo posteriore a data del giorno";
                    return false;
                }

                if (datiGenerici.DataInizioCalcolo.Value.CompareTo(datiPensione.DecorrenzaOriginaria) != 0 && datiGenerici.CausaCarico != 3 && datiGenerici.CausaCarico != 9 && datiGenerici.CausaCarico != 2)
                {
                    messaggioVideo = "Se prima liquidata: Data Ripristino deve essere uguale a Decorrenza Pensione";
                    return false;
                }
            }

            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                codiceParticolareSoggettoDerogato = elencoCodiceParticolareDB.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);

                if (datiGenerici.NaturaPensione.Substring(2, 1) == "Z" && datiPensione.DataPresentazioneDomanda.CompareTo(new DateTime(2001, 08, 16)) > 0 &&
                        datiGenerici.CausaCarico == 1 && codiceParticolareSoggettoDerogato.TraduzioneSuGp != '3')
                {
                    messaggioVideo = "3° codice Natura Pensione ('Z') incompatibile con Data Domanda";
                    return false;
                }
            }

            if (datiGenerici.DecorrenzaBonus.HasValue)
            {
                if (datiGenerici.DecorrenzaBonus.Value.CompareTo(new DateTime(2001, 03, 01)) < 0)
                {
                    messaggioVideo = "Decorrenza Bonus illogica";
                    return false;
                }

                if (datiGenerici.NaturaPensione.Substring(1, 1) != "X" && datiGenerici.NaturaPensione.Substring(1, 1) != "Y")
                {
                    messaggioVideo = "Decorrenza Bonus incompatibile con natura pensione";
                    return false;
                }

                if (datiIstruttoria != null)
                {
                    if (datiGenerici.CausaCarico != 2 && codiceParticolareSoggettoDerogato != null && codiceParticolareSoggettoDerogato.TraduzioneSuGp > 3)
                    {
                        messaggioVideo = "Codice Soggetto Derogato errato";
                        return false;
                    }
                }
            }

            if (!GestioneControlli.ControlsCodNaturaForDatiGenerici(datiPensione, datiGenerici.NaturaPensione, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.SiglaCategoria, datiGenerici.CodiceArretrati, datiAnagrafici.CodiceComuneResidenza, datiGenerici.CausaCarico, datiPensione.DataPresentazioneDomanda, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaPresenzaTrattenutaINPDAP(datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaTrattenutaINPDAPWithCausaCarico(datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap, datiGenerici.CausaCarico, datiPensione))
            {
                messaggioVideo = "Codice trattenuta Fondo Credito errato (SI/SPAZIO)";
                return false;
            }

            if (!GestioneControlli.VerificaCoerenzaTrattenutaINPDAP(datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap))
            {
                messaggioVideo = "Trattenuta Fondo Credito: Decorrenza incompatibile con codice";
                return false;
            }

            if (!GestioneControlli.VerificaTrattenutaINPDAPWithCategoria(datiGenerici.TrattenutaInpdap, datiPensione.Gruppo, datiPensione))
            {
                messaggioVideo = "Trattenuta Fondo Credito incompatibile con Categoria Pensione";
                return false;
            }

            if (!GestioneControlli.VerificaTrattenutaINPDAPWithDecorrenzaPensione(datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap, datiPensione.DecorrenzaOriginaria, datiPensione, datiStoricoGP != null ? datiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaTrattenutaINPDAP(datiPensione, datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap,
                datiStoricoGP != null ? datiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceMobilita(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiGenerici.CodiceMobilita, out messaggioVideo))
                return false;

            if (datiDanteCausa != null)
            {
                if (!GestioneControlli.ControlsCodiceMobilitaWithDanteCausa(decorrenza, datiPensione.NaturaPensione, datiGenerici.CodiceMobilita, out messaggioVideo))
                    return false;
            }

            if (!datiGenerici.CodiceArretrati.HasValue)
            {
                messaggioVideo = "Codice Arretrati errato o mancante (1 / 8)";
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaIncongruenzaEsenzioneFiscaleToDB(datiPensione, datiAnagrafici != null ? datiAnagrafici.CodiceComuneResidenza : string.Empty, datiDetrazioni, isRiaperturaDomanda, datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsEsenzioneFiscaleVittimaTerrorismo(datiPensione, isRiaperturaDomanda, datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsTipoBeneficioWithCodNatura(datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : string.Empty, datiGenerici.NaturaPensione, false, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDataInizioCalcolo(datiGenerici.DataInizioCalcolo, datiGenerici.DataInteressiLegali, datiGenerici.CodiceDomandaRicorso, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaCodiceArretratiWithEliminazione(datiEliminazione != null ? datiEliminazione.CodiceMotivo : null, datiGenerici.CodiceArretrati, datiPensione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaBeneficiPerOpzioneTipoContributivo(datiPensione, datiGenerici.Benefici, out messaggioVideo))
                return false;

            if (Utility.IsBonusBooking(datiPensione) && datiPensione.Tipo != "0167" && datiGenerici.IsRichiestaBonus.GetValueOrDefault())
            {
                if (!GestioneCrossControls.ALL_VerificaAnnoRichiestaBonus154(datiPensione, datiGenerici.AnnoDecorrenzaBonus, out messaggioVideo))
                    return false;
            }

            return true;
        }

        private static bool ControlsCrossDatiGenerici(Entity.DatiGenerici datiGenerici, Entity.DatiAssicurativi datiAssicurativi, Entity.DatiOpzione datiOpzione, GestionePensione.DatiPensione datiPensione,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            Entity.DatiIstruttoria datiIstruttoria, Entity.DatiProvenienza datiProvenienza, DateTime dataSistema, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<Liquidazione.BLCommon.GestioneCalcolo.DatiCalcoloRetributivo> lDatiCalcoloRetrib = null;
            Liquidazione.BLCommon.GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out lDatiCalcoloRetrib);

            List<Liquidazione.BLCommon.GestioneCalcolo.DatiCalcoloContributivo> lDatiCalcoloContrib = null;
            Liquidazione.BLCommon.GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out lDatiCalcoloContrib);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiCIGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiCIGenerici);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
            GestioneDatiContributiviCi.GetImportiEsteriByIdPensione(datiPensione.Id, out listaImportiEsteri);

            List<GestioneDecodifica.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392 = null;
            GestioneDecodifica.GetCodiceRequisitiLegge50392(out listaCodiceRequisitiLegge50392);

            List<Entity.AltraPensione> listaAltrePensioni = null;
            GestioneBititolarita.GetDatiAltraPensioneByIdPensione(datiPensione.Id, out listaAltrePensioni);

            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheFamiliari = null;
            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficheFamiliari);

            AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            List<GestioneContrib.StatoEstero> listaStatiEsteri = null;
            GestioneContrib.GetStatiEEfromDBByIdPensione(datiPensione.Id, listaPrestazioniEstere, out listaStatiEsteri);

            List<GestioneCalcolo.DatiCalcoloContributivoEstero> listaDatiCalcoloContributivoEstero = null;
            GestioneCalcolo.GetCalcoloContributivoEsteroCIbyIdPensione(datiPensione.Id, out listaDatiCalcoloContributivoEstero);

            List<GestioneDecodifica.CodeGestione> listaCodiciGestione = null;
            GestioneDecodifica.GetCodiceGestione(out listaCodiciGestione);

            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaDatiSupplementi = null;
            GestioneSupplementi.GetSupplementiByIdPensione(datiPensione.Id, out listaDatiSupplementi);

            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            int? settimaneRetributiveQuotaACodGestione1 = null;
            decimal? rmsQuotaACodGestione1 = null;
            decimal? rmsQuotaACodGestione2 = null;
            decimal? rmsQuotaACodGestione3 = null;
            decimal? rmsQuotaACodGestione4 = null;
            int? settimaneRetributiveQuotaBCodGestione1 = null;
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

            if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo retr in lDatiCalcoloRetrib)
                {
                    if (retr.CodiceGestione == 1)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            settimaneRetributiveQuotaACodGestione1 = retr.NSettimaneQuotaA;
                            rmsQuotaACodGestione1 = retr.RMSQuotaA;
                        }
                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            settimaneRetributiveQuotaBCodGestione1 = retr.NSettimaneQuotaB;
                            rmsQuotaBCodGestione1 = retr.RMSQuotaB;
                        }
                    }

                    if (retr.CodiceGestione == 2)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                            rmsQuotaACodGestione2 = retr.RMSQuotaA;

                        if (retr.QuotePrimeLiquidate == 'B')
                            rmsQuotaBCodGestione2 = retr.RMSQuotaB;
                    }

                    if (retr.CodiceGestione == 3)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                            rmsQuotaACodGestione3 = retr.RMSQuotaA;

                        if (retr.QuotePrimeLiquidate == 'B')
                            rmsQuotaBCodGestione3 = retr.RMSQuotaB;
                    }

                    if (retr.CodiceGestione == 4)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                            rmsQuotaACodGestione4 = retr.RMSQuotaA;

                        if (retr.QuotePrimeLiquidate == 'B')
                            rmsQuotaBCodGestione4 = retr.RMSQuotaB;
                    }
                }
            }
            if (lDatiCalcoloContrib != null && lDatiCalcoloContrib.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo contr in lDatiCalcoloContrib)
                {
                    if (contr.CodiceGestione == 1)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione1 = contr.NSettimane;
                            importoContributivoTotaleCodGestione1 = contr.ImportoContributivoTotale;
                        }
                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione1 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione1 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione1 = contr.MontanteQuotaDL214;
                        }
                    }

                    if (contr.CodiceGestione == 2)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione2 = contr.NSettimane;
                            importoContributivoTotaleCodGestione2 = contr.ImportoContributivoTotale;
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione2 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione2 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione2 = contr.MontanteQuotaDL214;
                        }
                    }

                    if (contr.CodiceGestione == 3)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione3 = contr.NSettimane;
                            importoContributivoTotaleCodGestione3 = contr.ImportoContributivoTotale;
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione3 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione3 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione3 = contr.MontanteQuotaDL214;
                        }
                    }

                    if (contr.CodiceGestione == 4)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione4 = contr.NSettimane;
                            importoContributivoTotaleCodGestione4 = contr.ImportoContributivoTotale;
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione4 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione4 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione4 = contr.MontanteQuotaDL214;
                        }
                    }
                }
            }

            int? sommaSettimaneEstere = null;
            int? sommaSettimaneDirittoEstere = null;

            foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
            {
                sommaSettimaneEstere = sommaSettimaneEstere.GetValueOrDefault() + prestEE.ContributiEEDecorrenzaOriginaria.GetValueOrDefault();
                sommaSettimaneDirittoEstere = sommaSettimaneDirittoEstere.GetValueOrDefault() + prestEE.ContributiEEDiritto.GetValueOrDefault();
            }

            if (datiCIGenerici == null)
                datiCIGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

            //////////////////////////////// settiamo il numero di settimane in base alla categoria////////////////////
            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);
            int? settimane = null;
            if (IsSingleTab)
                settimane = GestioneControlli.NumeroSettimane(datiCIGenerici != null ? datiCIGenerici.SettimaneItalianeDiritto : null, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.NSettimaneOBG : null, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.NContributiUtiliLavoratoriAutonomi : null);
            else
                settimane = GestioneControlli.NumeroSettimane(datiAssicurativi != null ? datiAssicurativi.SettimaneItalianeDiritto : null, datiAssicurativi != null ? datiAssicurativi.NSettimaneOBG : null, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.NContributiUtiliLavoratoriAutonomi : null);
            if (categoria > 0 && categoria < 7)
            {
                settimane = settimane.GetValueOrDefault() + datiAssicurativi.NContributiVolontari.GetValueOrDefault();
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            char? codiceRequisitiLegge50392TraduzioneSuGP = null;
            if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.HasValue && listaCodiceRequisitiLegge50392 != null && listaCodiceRequisitiLegge50392.Count > 0)
            {
                GestioneDecodifica.CodiceRequisitiLegge50392 appCodiceRequisitiLegge50392 = listaCodiceRequisitiLegge50392.Find(x => x.Id == datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.ToString());
                codiceRequisitiLegge50392TraduzioneSuGP = appCodiceRequisitiLegge50392 != null ? appCodiceRequisitiLegge50392.TraduzioneSuGP : null;
            }

            int codiceStatoEE = 0;
            string codiceIstituzione = string.Empty;
            string nomeStato = string.Empty;
            byte? codiceConvenzione = null;
            if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            {
                string codiceStato = listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE;
                codiceIstituzione = listaStatiEsteri[0].PrestazioneEstera.CodiceIstituzione;
                nomeStato = listaStatiEsteri[0].PrestazioneEstera.NomeStato;
                codiceConvenzione = listaStatiEsteri[0].PrestazioneEstera.CodiceConvenzione;

                int.TryParse(codiceStato, out codiceStatoEE);
            }

            if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivoEstero contrEstero in listaDatiCalcoloContributivoEstero)
                {
                    short? codiceGestioneTraduzioneSuGP = 0;
                    if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                    {
                        GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == contrEstero.CodiceGestione.Value);
                        if (codeGestione != null)
                            codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                    }
                }
            }

            long? codiceGestioneContributiEsteri = null;
            if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
                codiceGestioneContributiEsteri = listaDatiCalcoloContributivoEstero[0].CodiceGestione;

            if (IsSingleTab)
            {
                GetDatiAssicurativi(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiAssicurativi);
            }

            #region Controlli Decorrenza Arretrati
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.TipoAppartenenza.CI;

            int annoCompetenza = 0;
            Liquidazione.BLCommon.GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);

            if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiPL(datiGenerici.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, datiPensione, datiGenerici.DataInizioCalcolo, dataSistema, out messaggioVideo))
                    return false;
            }
            else
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiRIC(datiGenerici.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, datiGenerici.CausaCarico, datiGenerici.DataInizioCalcolo, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaArretrati(datiGenerici.DecorrenzaCalcoloArretrati) && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                messaggioVideo = "Decorrenza Arretrati illogica o mancante.";
                return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaArretratiWithDataInizioCalcolo(datiGenerici.DecorrenzaCalcoloArretrati, datiGenerici.DataInizioCalcolo))
            {
                messaggioVideo = "Decorrenza Arretrati anteriore a Data Ripristino.";
                return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaArretratiWithGennaio1983(datiGenerici.DecorrenzaCalcoloArretrati, datiGenerici.CausaCarico) && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                messaggioVideo = "Decorrenza Arretrati errata.";
                return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaArretratiWithDataPresentazione(datiGenerici.DecorrenzaCalcoloArretrati, datiGenerici.CausaCarico, datiPensione.DataPresentazioneDomanda))
            {
                messaggioVideo = "Decorrenza Arretrati incompatibile con la Data Domanda";
                return false;
            }

            #endregion Controlli Decorrenza Arretrati

            #region Controlli Causa Carico

            //Attualmente utilizziamo il Gruppo per verificare che non si tratti di una Ricostituzione. Per le specifice, vedere il summary del metodo.
            if (!GestioneControlli.VerificaCausaCarico(datiGenerici.CausaCarico, datiPensione.Gruppo, datiPensione.Prodotto, isRiaperturaDomanda, out messaggioVideo))
                return false;

            #endregion Controlli Causa Carico

            #region Controlli OBG Misura 503 o Contributi 335

            ////COMMENTATO. NON RICHIAMARE FINO A NUOVE SPECIFICHE
            //List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
            //GestioneCalcolo.GetCalcoloContributivoCI_AGOByPensione(datiPensione.Id, out ldatiContributivi);

            //List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
            //GestioneCalcolo.GetCalcoloRetributivoCI_AGOByPensione(datiPensione.Id, out ldatiRetributivi);

            //GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            //GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);

            //int? nSettimane = null;
            //if (ldatiContributivi != null && ldatiContributivi.Count > 0)
            //    nSettimane = ldatiContributivi[0].NSettimane;

            //if (ldatiRetributivi != null && ldatiRetributivi.Count > 0)
            //    foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in ldatiRetributivi)
            //        if (!GestioneControlli.VerificaOBGMisura335Contributi335(datiAssicurativi.FineAssicurazione, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione,
            //            datiRetributivi.NSettimaneQuotaB, nSettimane, datiAssicurativi.CodiceConvenzione, datiIstruttoria.NContributiVolontari))
            //        {
            //            messaggioVideo = "OBG Misura 503/92 o Contributi 335/95 mancanti.";
            //            return false;
            //        }

            #endregion Controlli OBG Misura 503 o Contributi 335

            if (!GestioneControlli.VerificaEsenzioneFiscaleTerrorismo(datiGenerici.CodiceComunicazioneCampo4, datiDetrazioni != null ? datiDetrazioni.DetrazioniReddito : null))
            {
                messaggioVideo = "Esenzione fiscale 'Vittime Terrorismo' deve essere 'NO'";
                return false;
            }

            if (!GestioneControlli.ControlsEsenzioneFiscaleEstero(datiGenerici.CodiceComunicazioneCampo4, datiDetrazioni != null ? datiDetrazioni.DetrazioniReddito : null, datiAnagrafici.ProvinciaResidenza, datiAnagrafici.CodiceComuneResidenza, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsEsenzioneFiscaleDoppiaImposizione(datiPensione, datiAnagrafici != null ? datiAnagrafici.CodiceComuneResidenza : null, isRiaperturaDomanda, datiGenerici != null ? datiGenerici.CodiceComunicazioneCampo4 : null, out messaggioVideo))
                return false;

            if (datiDanteCausa != null)
            {
                if (!GestioneCrossControls.CI_VerificaCodNaturaWithCategoriaDC(datiGenerici.NaturaPensione, datiDanteCausa.SiglaCategoria))
                {
                    messaggioVideo = "Natura pensione 'O' (reg.sperimentale donne) incompatibile con reversibilità da assicurato";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaCodNaturaTitolareWithDC(datiDanteCausa.NaturaPensione, datiGenerici.NaturaPensione))
                {
                    messaggioVideo = "Natura Pensione errata";
                    return false;
                }
            }

            #region Categorie minori o uguali a 6
            if (categoria > 0 && categoria <= 6)
            {
                if (!GestioneControlli.VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMinori7(datiAssicurativi != null ? datiAssicurativi.NContributiItalia : null, settimane,
                    datiAssicurativi != null ? datiAssicurativi.VVMisuraAl1292 : null, datiGenerici.DataInizioCalcolo, tipoDomanda,
                    listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, codiceStatoEE,
                    datiDanteCausa != null ? datiDanteCausa.Certificato : null, out messaggioVideo))
                    return false;
            }
            #endregion Categorie minori o uguali a 6

            #region PCIPL39 categoria >= 7
            if (categoria >= 7)
            {
                if (!GestioneControlli.VerificaCmsmWithDecorrenza(decorrenza, datiCIGenerici != null ? datiCIGenerici.CMSM : null, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(1, datiGenerici.NaturaPensione, rmsQuotaACodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(2, datiGenerici.NaturaPensione, rmsQuotaACodGestione2, rmsQuotaBCodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(3, datiGenerici.NaturaPensione, rmsQuotaACodGestione3, rmsQuotaBCodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(4, datiGenerici.NaturaPensione, rmsQuotaACodGestione4, rmsQuotaBCodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMaggiori6(datiAssicurativi != null ? datiAssicurativi.NContributiItalia : null, settimane,
                    datiGenerici.DataInizioCalcolo, tipoDomanda, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, out messaggioVideo))
                    return false;
            }
            #endregion PCIPL39 categoria >= 7

            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
                {
                    if (!GestioneControlli.VerificaDataPrecedenteLiquidazioneWithCausaCarico(prestEE.DecorrenzaLiquidazioneStatoEE, datiGenerici.CausaCarico, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.VerificaImportiEsteriWithCodNatura(listaPrestazioniEstere, listaImportiEsteri, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaMaggiorazioneLegg140WithEtaPensionabile(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneLegge140 : null, tipoDomanda, datiGenerici.CausaCarico, datiAnagrafici.DataNascita, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceRequisitiLegge50392WithInvalidita(codiceRequisitiLegge50392TraduzioneSuGP, datiPensione.Gruppo, datiGenerici.NaturaPensione, out messaggioVideo))
                return false;

            if (datiGenerici.NaturaPensione.Substring(0, 1).Equals("2") || datiGenerici.NaturaPensione.Substring(0, 1).Equals("4") || datiGenerici.NaturaPensione.Substring(0, 1).Equals("5") ||
                datiGenerici.NaturaPensione.Substring(0, 1).Equals("6") || datiGenerici.NaturaPensione.Substring(0, 1).Equals("9"))
            {
                if (!GestioneControlli.VerificaAltraPensioneWithNaturaPensione(listaAltrePensioni, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaNRicoscimentiInvaliditaWithDecorrenza(datiGenerici.NRiconoscimentiInvalidita, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiPensione.SiglaCategoria, datiPensione, out messaggioVideo))
                return false;

            #region PCIPL11
            if (tipoDomanda == Utility.TipoDomanda.Superstiti || (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0 && areaTitolare.ElencoStatiCivili.FindIndex(x => x.Codice == 2) > -1) || (listaFamiliari != null && listaFamiliari.Count > 0))
            {
                if (listaFamiliari != null && listaFamiliari.Count > 0)
                {
                    foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                    {
                        if (!GestioneCrossControls.CI_VerificaScadenzaRevisioneSanitariaWithDatiGenerici(fam.ScadenzaRevisioneSanitaria, fam.SiglaFamiliare, datiGenerici.CausaCarico, out messaggioVideo))
                            return false;
                    }
                }
            }
            #endregion PCIPL11

            if (!GestioneControlli.VerificaCodiceRequisitiParticolariWithDatiGenerici(datiAssicurativi.CodiceRequisitiParticolari, tipoDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, datiPensione.Gruppo, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaCodiceVirtuale(datiGenerici.CausaCarico, datiAssicurativi != null ? datiAssicurativi.DecorrenzaCodiceVirtuale : null, datiAssicurativi != null ? datiAssicurativi.CodiceVirtuale : null, datiPensione.DecorrenzaOriginaria, codiceConvenzione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsConfermaInvalidita(datiPensione, datiEliminazione != null ? datiEliminazione.DataEvento : null, datiGenerici.NRiconoscimentiInvalidita,
                dataSistema, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(datiPensione, datiGenerici.NaturaPensione, datiAssicurativi != null ? datiAssicurativi.InizioAssicurazione : null, out messaggioVideo))
                return false;

            if (IsSingleTab)
            {
                GestioneQuadri.DatiQuadroLiquidazionePensione quadroLiquidazionePensione = null;
                GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out quadroLiquidazionePensione);
                bool isAssicurativiAcquisito = quadroLiquidazionePensione.TabDatiAssicurativi.HasValue && quadroLiquidazionePensione.TabDatiAssicurativi.Value == 2;

                if (!GestioneControlli.VerificaCodiceVirtualeWithCausaCarico(datiCIGenerici.CodiceVirtuale, datiGenerici.CausaCarico))
                {
                    messaggioVideo = "Codice Virtuale 6 ammesso solo in Ricostituzione o Causa Carico 9.";
                    return false;
                }

                ////NON RICHIAMARE TALE CONTROLLO!!!!
                ////Da verificare!!!!!!!!!!!!! Leggere commento a lato del documento di specifiche - Anomalia tra codiceVirtuale != 6 e codiceVirtuale = 6

                //List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
                //GestioneDatiContributiviCi.GetPrestazioniEEByNumeroDomanda(numeroDomanda, out listaPrestazioniEE);

                //byte? codiceConvenzione = null;
                //if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
                //    codiceConvenzione = listaPrestazioniEE[0].CodiceConvenzione;

                //if (!GestioneControlli.VerificaCodiceConvenzioneWithCodiceVirtualeReversibilita(codiceConvenzione, datiCIGenerici.CodiceVirtuale, datiGenerici.CausaCarico, datiPensione.Gruppo, out messaggioVideo))
                //    return false;

                if (!GestioneControlli.VerificaDecorrenzaOriginariaWithCodNaturaAndDataPresentazione(datiPensione, datiGenerici.CausaCarico, datiGenerici.NaturaPensione, datiPensione.AttivitaEconomica,
                    datiPensione.ProfessioneIndividuale, out messaggioVideo))
                    return false;

                if (!VerificaRequisitiAnzianita9496Vecchiaia94(datiPensione.RequisitiAl1294, datiPensione.RequisitiVecchiaiaAl1294, datiPensione.RequisitiAl996, datiGenerici.NaturaPensione,
                    datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, isAssicurativiAcquisito, isRiaperturaDomanda, datiPensione, out messaggioVideo))
                    return false;

                #region Controlli R.M.S.

                if (datiOpzione == null)
                {
                    datiOpzione = new DatiOpzione();
                    GestioneLiquidazionePensione.GetDatiOpzione(datiPensione.Id, datiIstruttoriaCommon, out datiOpzione);
                }

                if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in lDatiCalcoloRetrib)
                    {
                        if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiRetributivi.RMSQuotaA,
                            datiPensione.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                            datiOpzione != null ? datiOpzione.DecorrenzaOpzione : null, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione, datiPensione.Gruppo, datiPensione.Prodotto))
                        {
                            messaggioVideo = "R.M.S. mancante.";
                            return false;
                        }
                    }

                #endregion Controlli R.M.S.

                #region Controlli Ufficio Pagatore Istituzione Estera

                if (datiGenerici.CodiceArretrati.HasValue && datiGenerici.CodiceArretrati.Value == 8)
                {
                    if (datiCIGenerici.CodiceBloccoArretratiEE.HasValue && datiCIGenerici.CodiceBloccoArretratiEE.Value && datiCIGenerici.UfficioPagatoreArretratiEE.HasValue)
                    {
                        messaggioVideo = "Accantonamento Arretrati incompatibile con la sezione 'Blocco Arretrati Estero' presenti nella tab Dati Assicurativi.";
                        return false;
                    }
                }

                #endregion Controlli Ufficio Pagatore Istituzione Estera

                if (!GestioneControlli.VerificaDelibera12688WithCodNatura(datiCIGenerici.DeliberaCee126, datiGenerici.NaturaPensione, datiPensione.Gruppo))
                {
                    messaggioVideo = "Delibera 126/88 incompatibile con Natura Pensione";
                    return false;
                }

                if (!GestioneControlli.VerificaCodiceMobilitaWithRequisitoRidotto(decorrenza, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiGenerici.CodiceMobilita, datiPensione.SiglaCategoria, datiIstruttoria != null ? datiIstruttoria.Legge44997 : null))
                {
                    messaggioVideo = "Codice Mobilità incompatibile con il Requisito Ridotto";
                    return false;
                }

                if (!GestioneControlli.ControlsCodNaturaCrossTab(datiGenerici.NaturaPensione, datiPensione.Gruppo, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.DecorrenzaOriginaria, datiGenerici.CausaCarico, datiPensione.CodiceTipoRichiesta, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsImportoCristallizzazione(datiCIGenerici != null ? datiCIGenerici.ImportoCristallizzazione3481 : null, datiGenerici.CausaCarico, datiPensione.SiglaCategoria, datiCIGenerici != null ? datiCIGenerici.CodiceVirtuale : null, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.CodiceRequisitiParticolari : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCodNatura(datiCIGenerici.NSettFittiziePrepensionamento, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;

                #region Categorie minori o uguali a 6
                if (categoria > 0 && categoria <= 6)
                {
                    if (!GestioneControlli.ControlsDecorrenzaBonusWithFineAssicurazione(datiPensione.FineAssicurazione, datiGenerici.DecorrenzaBonus, datiGenerici.NaturaPensione, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaCapienzaSettimaneWithAssicurazione(datiPensione, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimane, datiPensione.ProfessioneIndividuale,
                        datiGenerici.NaturaPensione, out messaggioVideo))
                        return false;
                }
                #endregion Categorie minori o uguali a 6

                if (!GestioneControlli.VerificaCodRiduzioneWithCodNatura(datiCIGenerici != null ? datiCIGenerici.RiduzioneRetributiva : false, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCapienzaSettimaneDL50392WithAssicurazione(datiPensione, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.ProfessioneIndividuale, datiGenerici.NaturaPensione, datiPensione.DecorrenzaOriginaria, decorrenza, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, datiPensione.AttivitaEconomica, out messaggioVideo))
                    return false;

                if ((datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value) || datiGenerici.CausaCarico == 3 || datiGenerici.CausaCarico == 9)
                {
                    if (!GestioneControlli.VerificaDatiPrecedentePensione(datiGenerici.CausaCarico, datiAssicurativi != null ? datiAssicurativi.CodiceRequisitiParticolari : null, datiGenerici.NaturaPensione, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.CodiceP18PrecedentePensione : null, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.CertificatoPrecedentePensione : null, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.SedePrecedentePensione : null,
                        categoria, datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : null, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiIstruttoriaCommon != null ? datiIstruttoriaCommon.DecorrenzaOriginariaAltraPensione : null, datiGenerici != null ? datiGenerici.TrasformazioneAOI : null, out messaggioVideo))
                        return false;
                }
            }
            else
            {
                if (!GestioneControlli.VerificaCodiceVirtualeWithCausaCarico(datiAssicurativi.CodiceVirtuale, datiGenerici.CausaCarico))
                {
                    messaggioVideo = "Codice Virtuale 6 ammesso solo in Ricostituzione o Causa Carico 9.";
                    return false;
                }

                ////NON RICHIAMARE TALE CONTROLLO!!!!
                ////Da verificare!!!!!!!!!!!!! Leggere commento a lato del documento di specifiche - Anomalia tra codiceVirtuale != 6 e codiceVirtuale = 6
                //if (!GestioneControlli.VerificaCodiceConvenzioneWithCodiceVirtualeReversibilita(datiAssicurativi.CodiceConvenzione, datiAssicurativi.CodiceVirtuale, datiGenerici.CausaCarico, datiPensione.Gruppo, out messaggioVideo))
                //    return false;

                if (!VerificaRequisitiAnzianita9496Vecchiaia94(datiAssicurativi.RequisitiAl1294, datiAssicurativi.RequisitiVecchiaiaAl1294, datiAssicurativi.RequisitiAl996, datiGenerici.NaturaPensione,
                    datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, null, isRiaperturaDomanda, datiPensione, out messaggioVideo))
                    return false;

                if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in lDatiCalcoloRetrib)
                    {
                        if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiRetributivi.RMSQuotaA,
                            datiAssicurativi.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                            datiOpzione != null ? datiOpzione.DecorrenzaOpzione : (DateTime?)null, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione, datiPensione.Gruppo, datiPensione.Prodotto))
                        {
                            messaggioVideo = "R.M.S. mancante.";
                            return false;
                        }
                    }

                #region Controlli Ufficio Pagatore Istituzione Estera

                if (datiGenerici.CodiceArretrati.HasValue && datiGenerici.CodiceArretrati.Value == 8)
                {
                    if (datiAssicurativi.CodiceBloccoArretratiEE.HasValue && datiAssicurativi.CodiceBloccoArretratiEE.Value && !string.IsNullOrEmpty(datiAssicurativi.UfficioPagatoreArretratiEsteri))
                    {
                        messaggioVideo = "Accantonamento Arretrati incompatibile con la sezione 'Blocco Arretrati Estero' presenti nella tab Dati Assicurativi.";
                        return false;
                    }
                }

                #endregion Controlli Ufficio Pagatore Istituzione Estera
                if (datiAssicurativi != null)
                {
                    if (!GestioneControlli.VerificaDecorrenzaOriginariaWithCodNaturaAndDataPresentazione(datiPensione, datiGenerici.CausaCarico, datiGenerici.NaturaPensione,
                        datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaDelibera12688WithCodNatura(datiAssicurativi.DeliberaCee126, datiGenerici.NaturaPensione, datiPensione.Gruppo))
                    {
                        messaggioVideo = "Delibera 126/88 incompatibile con Natura Pensione";
                        return false;
                    }
                }

                if (datiIstruttoria != null && !GestioneControlli.VerificaCodiceMobilitaWithRequisitoRidotto(decorrenza, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiGenerici.CodiceMobilita, datiPensione.SiglaCategoria, datiIstruttoria.Legge44997))
                {
                    messaggioVideo = "Codice Mobilità incompatibile con il Requisito Ridotto";
                    return false;
                }

                if (datiAssicurativi != null && !GestioneControlli.ControlsCodNaturaCrossTab(datiGenerici.NaturaPensione, datiPensione.Gruppo, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, datiPensione.DecorrenzaOriginaria, datiGenerici.CausaCarico, datiPensione.CodiceTipoRichiesta, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsImportoCristallizzazione(datiAssicurativi != null ? datiAssicurativi.ImportoCristallizzazione3481 : null, datiGenerici.CausaCarico, datiPensione.SiglaCategoria, datiAssicurativi != null ? datiAssicurativi.CodiceVirtuale : null, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiAssicurativi != null ? datiAssicurativi.CodiceRequisitiParticolari : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCodNatura(datiAssicurativi != null ? datiAssicurativi.NSettFittiziePrepensionamento : null, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;

                #region Categorie minori o uguali a 6
                if (categoria > 0 && categoria <= 6)
                {
                    if (!GestioneControlli.ControlsDecorrenzaBonusWithFineAssicurazione(datiAssicurativi != null ? datiAssicurativi.FineAssicurazione : null, datiGenerici.DecorrenzaBonus, datiGenerici.NaturaPensione, out messaggioVideo))
                        return false;

                    if (datiAssicurativi != null && !GestioneControlli.VerificaCapienzaSettimaneWithAssicurazione(datiPensione, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, settimane,
                        datiAssicurativi.ProfessioneIndividuale, datiGenerici.NaturaPensione, out messaggioVideo))
                        return false;
                }
                #endregion Categorie minori o uguali a 6

                if (!GestioneControlli.VerificaCodRiduzioneWithCodNatura(datiIstruttoria != null ? datiIstruttoria.RiduzioneRetributiva : false, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCapienzaSettimaneDL50392WithAssicurazione(datiPensione, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiAssicurativi.ProfessioneIndividuale, datiGenerici.NaturaPensione, datiPensione.DecorrenzaOriginaria, decorrenza, settimaneRetributiveQuotaBCodGestione1, datiAssicurativi.VVMisuraDL50392, datiAssicurativi.AttivitaEconomica, out messaggioVideo))
                    return false;

                if ((datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value) || datiGenerici.CausaCarico == 3 || datiGenerici.CausaCarico == 9)
                {
                    if (!GestioneControlli.VerificaDatiPrecedentePensione(datiGenerici.CausaCarico, datiAssicurativi != null ? datiAssicurativi.CodiceRequisitiParticolari : null, datiGenerici.NaturaPensione, datiProvenienza.CodiceP18PrecedentePensione, datiProvenienza.CertificatoPrecedentePensione, datiProvenienza.SedePrecedentePensione,
                        categoria, datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : null, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiProvenienza.DecorrenzaOriginariaAltraPensione, datiGenerici != null ? datiGenerici.TrasformazioneAOI : null, out messaggioVideo))
                        return false;
                }
            }
            return true;
        }

        public static void StoreDatiGenerici(GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, Entity.DatiGenerici datiGenerici, Entity.DatiExCombattente datiExCombattente,
            Entity.DatiBenefici datiBenefici, Entity.DatiMaggiorazioni datiMaggiorazioni, DateTime dataSistema, bool IsCancelOperation)
        {
            if (datiGenerici == null)
                datiGenerici = new Entity.DatiGenerici();

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

            GestionePagamento.DatiPagamento datiPagamento = null;
            GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);

            GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);

            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);

            GestionePensione.DatiTitolare datiTitolare = null;
            GestionePensione.GetTitolareByIdPensione(datiPensione.Id, out datiTitolare);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiTitolare.IdAnagrafica, out datiAnagraficiTitolare);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            #region Gestione visibilità tabs MaggiorazioneBenefici

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);

            #endregion Gestione visibilità tabs MaggiorazioneBenefici

            GestioneQuadri.DatiQuadroBititolarita datiQuadroBititolarita = null;
            GestioneQuadri.GetQuadroBititolaritaByDatiPensione(datiPensione, out datiQuadroBititolarita);

            GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = null;
            GestioneQuadri.GetQuadroEliminazioneByDatiPensione(datiPensione, out datiQuadroEliminazione);

            GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni = null;
            GestioneQuadri.GetQuadroDetrazioniByDatiPensione(datiPensione, out datiQuadroDetrazioni);

            GestioneQuadri.DatiQuadroRichiestaBonus datiQuadroRichiestaBonus = null;
            GestioneQuadri.GetQuadroRichiestaBonusByDatiPensione(datiPensione, out datiQuadroRichiestaBonus);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            bool isVariaDetrazioni = Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsEsenzioneFiscaleEsteroFromDetrazioni(datiPensione, datiDetrazioni, isRiaperturaDomanda).GetValueOrDefault() &&
                !Utility.IsEsenzioneFiscaleEsteroAutonomi(datiPensione, datiAnagraficiTitolare != null ? datiAnagraficiTitolare.CodiceComuneResidenza : null);

            bool isBeneficioVittimeUnderOver80 = Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo);

            //ENG - memo 28_2024
            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") &&
                Utility.IsDomandaTipoContributivo(datiPensione, true, false) && datiGenerici != null)
                {
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                    {
                        if (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 01, 01)))
                        {
                            DateTime? cessazioneIncumulabilita = Utility.CalcolaCessazioneIncumulabilita_memo_28(datiPensione, datiAnagraficiTitolare, datiPensione.DataPerfezionamentoRequisiti);
                            if (cessazioneIncumulabilita.HasValue)
                            {
                                datiGenerici.ScadenzaRevisioneSanitaria = cessazioneIncumulabilita;
                            }
                        }
                        else
                        {
                            datiGenerici.ScadenzaRevisioneSanitaria = null;
                        }
                    }
                    else if (datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue)
                        datiGenerici.ScadenzaRevisioneSanitaria = datiIstruttoria.ScadenzaRevisioneSanitaria;
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiGenericiPerPensione(datiGenerici, datiPensione);
                StoreDatiGenericiPerIstruttoria(datiPensione, datiGenerici, ref datiIstruttoria);
                StoreDatiGenericiPerDatiGenericiCI(datiPensione.Id, datiPensione.FlagUnicarpe, datiPensione.TipoLetturaUnicarpe, datiGenerici, ref datiPensioniDatiGenerici);
                StoreDatiGenericiPerDatiPagamento(datiPensione.Id, datiGenerici, datiPagamento);
                StoreDatiGenericiPerDatiNuoveLiquidate(datiPensione.Id, datiGenerici, nuoveLiquidate);

                if ((datiGenerici.IsDatiGenericiPensioneNull() && datiGenerici.IsDatiGenericiIstruttoriaNull() &&
                     datiGenerici.IsDatiGenericiNuoveLiquidateNull() && datiGenerici.IsDatiGenericiPagamentoNull() &&
                     datiGenerici.IsDatiGenericiPensioneCiDatiDenericiNull()) || IsCancelOperation)
                    datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                else
                {
                    datiQuadroLiquidazionePensione.TabDatiGenerici = 2;
                }

                if ((datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value) || datiGenerici.CausaCarico == 3 || datiGenerici.CausaCarico == 9)
                {
                    if (datiQuadroLiquidazionePensione.TabPrecedentePensione == 1)
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 0;
                }
                else
                    datiQuadroLiquidazionePensione.TabPrecedentePensione = 1;

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                bool isRipristino = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ripristino || Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.RipristinoSuperstiti;
                if (tipoAppartenenza == Utility.TipoAppartenenza.CI && isRipristino)
                {
                    datiQuadroLiquidazionePensione.TabPrecedentePensione = 1;
                }

                #region Gestione visibilità tabs MaggiorazioneBenefici

                if (datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabExCombattente != 2)
                        datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = null;

                if (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabBenefici != 2)
                        datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabBenefici = null;

                if (datiGenerici.Maggiorazioni.HasValue && datiGenerici.Maggiorazioni.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabMaggiorazioni != 2)
                        datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = null;

                if ((datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value && datiQuadroMaggiorazioniBenefici.TabExCombattente == 2) ||
                    (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value && datiQuadroMaggiorazioniBenefici.TabBenefici == 2) ||
                    (datiGenerici.Maggiorazioni.HasValue && datiGenerici.Maggiorazioni.Value && datiQuadroMaggiorazioniBenefici.TabMaggiorazioni == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                if (datiQuadroMaggiorazioniBenefici.TabExCombattente == 0 || datiQuadroMaggiorazioniBenefici.TabBenefici == 0 || datiQuadroMaggiorazioniBenefici.TabMaggiorazioni == 0)
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;
                if (!datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && !datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && !datiQuadroMaggiorazioniBenefici.TabMaggiorazioni.HasValue)
                    datiQuadroMaggiorazioniBenefici.Tipo = 0;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                #endregion Gestione visibilità tabs MaggiorazioneBenefici

                #region Gestione RichiestaBonus

                if (datiGenerici.IsRichiestaBonus.HasValue)
                {
                    if (datiGenerici.IsRichiestaBonus.Value)
                    {
                        datiQuadroRichiestaBonus.Tipo = 2;
                        datiQuadroRichiestaBonus.TabRichiestaBonus = 0;
                        GestioneQuadri.SalvaQuadroRichiestaBonus(datiPensione.Id, datiQuadroRichiestaBonus);
                    }
                    else
                    {
                        GestioneAnniRichiestaBonus.EliminaAnniRichiestaBonusByIdPensione(datiPensione.Id);
                        datiQuadroRichiestaBonus.Tipo = 0;
                        datiQuadroRichiestaBonus.TabRichiestaBonus = null;
                        GestioneQuadri.SalvaQuadroRichiestaBonus(datiPensione.Id, datiQuadroRichiestaBonus);
                    }
                }

                #endregion Gestione RichiestaBonus

                #region Gestione visibilità menu Bititolarità

                if (Utility.IsBititolaritaVisible(datiGenerici.NaturaPensione))
                {
                    if (datiQuadroBititolarita.Tipo == 0)
                    {
                        datiQuadroBititolarita.Tipo = 2;
                        datiQuadroBititolarita.TabAltrePensioni = 0;
                    }
                }
                else
                {
                    // update quadro ai valori iniziali; a monte è stato effettuato un cross- control sulla presenza o meno della bititolarità
                    datiQuadroBititolarita.Tipo = 0;
                    datiQuadroBititolarita.TabAltrePensioni = null;
                }

                GestioneQuadri.SalvaQuadroBititolarita(datiPensione.Id, datiQuadroBititolarita);
                #endregion Gestione visibilità menu Bititolarità

                #region Gestione Semaforo Detrazioni
                // Le detrazioni non devono essere presenti nel caso in cui venga salvata l'esenzione fiscale
                Utility.ManageSemaforoDetrazioniPerEsenzioneFiscale(datiPensione, datiQuadroDetrazioni, datiGenerici.CodiceComunicazioneCampo4, isRiaperturaDomanda, isVariaDetrazioni, isBeneficioVittimeUnderOver80);
                #endregion Gestione Semaforo Detrazioni

                #region Gestione Semaforo Eliminazione
                // Per le ricostituzioni il semaforo non deve variare
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                {
                    if (datiEliminazione == null || datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
                    {
                        string messaggioVideo = string.Empty;

                        if (
                            (datiTitolare != null && datiTitolare.DataMorte.HasValue && Utility.DataSuccessivaA(datiTitolare.DataMorte.Value, datiPensione.DecorrenzaOriginaria.Value))
                            ||
                            (!IsCancelOperation && !GestioneCrossControls.AGO_CI_ControlsEliminazioneConfermaInvalidita(datiPensione,
                            datiEliminazione != null ? datiEliminazione.DataEvento : null, datiIstruttoria != null ? datiIstruttoria.NRiconoscimentiInvalidita : null,
                            dataSistema, isRiaperturaDomanda, out messaggioVideo))
                            )
                        {
                            datiQuadroEliminazione.Tipo = 2;
                            datiQuadroEliminazione.TabEliminazione = 0;
                        }
                        else
                        {
                            datiQuadroEliminazione.Tipo = 1;
                            datiQuadroEliminazione.TabEliminazione = 1;
                        }

                        GestioneQuadri.SalvaQuadroEliminazione(datiPensione.Id, datiQuadroEliminazione);
                    }
                }
                #endregion Gestione Semaforo Eliminazione

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        public static void GetDatiGenerici(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool isRiaperturaDomanda, out Entity.DatiGenerici datiGenerici)
        {
            datiGenerici = null;

            if (datiPensione == null)
                return;

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiCIGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiCIGenerici);

            GestionePagamento.DatiPagamento datiPagamento = null;
            GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);

            GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (datiIstruttoria == null && datiCIGenerici == null && datiPagamento == null && nuoveLiquidate == null)
                return;

            datiGenerici = new Entity.DatiGenerici();
            Utility.ValorizzaOggetti(datiPensione, datiGenerici);
            if (Utility.IsDomandaSperimentaleDonna(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " O ";
            }
            if (Utility.IsDomandaUsuranti(datiPensione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "  Z";
                else if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) == string.Empty)
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "Z";
            }
            if (Utility.IsDomandaTrasformazioneInvalidita(datiPensione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "  H";
                else if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) == " ")
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "H";
            }
            if (Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " J ";
                else
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.Substring(0, 1) + "J" + datiGenerici.NaturaPensione.Substring(2, 1);
            }

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                DateTime dataSistema = Utility.DataSistemaCi;
                datiGenerici.DataRicezionePrenotazioneCentrale = dataSistema;
            }

            Utility.ValorizzaOggetti(datiIstruttoria, datiGenerici);
            Utility.ValorizzaOggetti(datiCIGenerici, datiGenerici);
            Utility.ValorizzaOggetti(datiPagamento, datiGenerici);
            Utility.ValorizzaOggetti(nuoveLiquidate, datiGenerici);

            if (datiGenerici.IsDatiGenericiIstruttoriaNull() && datiGenerici.IsDatiGenericiNuoveLiquidateNull() && datiGenerici.IsDatiGenericiPagamentoNull() &&
               datiGenerici.IsDatiGenericiPensioneCiDatiDenericiNull() && datiGenerici.IsDatiGenericiPensioneNull() /*&& datiGenerici.IsDatiGenericiVittimeTerrorismoNull()*/)
                datiGenerici = null;
        }

        public static void EliminaDatiGenerici(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici, Entity.DatiMaggiorazioni datiMaggiorazioni, DateTime dataSistema, out string msgVideo)
        {
            msgVideo = string.Empty;

            if (datiPensione == null)
                datiPensione = new GestionePensione.DatiPensione();

            if (datiPensioniDatiGenerici == null)
                datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

            if (!ControlsDatiGenericiForPensioneProvenienza(datiIstruttoria, datiPensione.TrasformazioneAOI, datiPensione.CausaCarico, datiPensione, true))
            {
                msgVideo = "Eliminare i dati della Pensione di Provenienza prima di procedere con la cancellazione dei Dati Generici";
                return;
            }

            if (datiPensioniDatiGenerici.UfficioPagatoreArretratiEE.HasValue || datiPensioniDatiGenerici.CodiceBloccoArretratiEE.HasValue)
            {
                msgVideo = "Eliminare i Dati Assicurativi prima di procedere con la cancellazione dei Dati Generici";
                return;
            }

            if (!ControlsDatiGenericiForBititolaritaAltraPensioneByIdPensione(datiPensione.Id, string.Empty, true))
            {
                msgVideo = "Eliminare i dati 'Altra Pensione' nel quadro 'Bititolarità' prima di procedere con la cancellazione";
                return;
            }


            //GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            //GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);
            //if (datiIstruttoria != null)
            //{
            //    datiProvenienza = new DatiProvenienza();
            //    Utility.ValorizzaOggetti(datiIstruttoria, datiProvenienza);

            //    if (!datiProvenienza.IsDatiProvenienzaIstruttoriaNull())
            //    {
            //        msgVideo = "Eliminare i dati della Pensione di Provenienza prima di procedere con la cancellazione dei dati Generici";
            //        return;
            //    }
            //}

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);

            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

            if (!ControlsDatiGenericiForMaggBenefici(datiExCombattente, datiBenefici, datiMaggiorazioni, datiPensione.ExCombattente, datiPensione.Benefici, datiPensione.Maggiorazioni, true, out msgVideo))
                return;

            StoreDatiGenerici(datiPensione, ref datiMaggiorazioniBenefici, ref datiIstruttoria, ref datiPensioniDatiGenerici, null, datiExCombattente, datiBenefici, datiMaggiorazioni, dataSistema, true);
        }

        private static void StoreDatiGenericiPerPensione(Entity.DatiGenerici datiGenerici, GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012")
                datiGenerici.Benefici = datiPensione.Benefici;

            Utility.ValorizzaOggetti(datiGenerici, datiPensione);

            GestionePensione.SalvaPensione(datiPensione);
        }

        private static void StoreDatiGenericiPerIstruttoria(GestionePensione.DatiPensione datiPensione, Entity.DatiGenerici datiGenerici, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiGenerici.IsDatiGenericiIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }

            // i dati provenienti da felpe sono non modificabili e non cancellabili
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                datiGenerici.CodiceMobilita = datiIstruttoria.CodiceMobilita;
            if (!Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                datiGenerici.CodiceDomandaRicorso = datiIstruttoria.CodiceDomandaRicorso;
            //il valore CodiceDomandaRicorso non deve mai essere cancellato

            Utility.ValorizzaOggetti(datiGenerici, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(datiPensione.Id);
            else
                GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);
        }

        private static void StoreDatiGenericiPerDatiGenericiCI(long idPensione, bool? FlagUnicarpe, char? TipoLetturaUnicarpe, Entity.DatiGenerici datiGenerici, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            if (datiPensioniDatiGenerici == null)
            {
                if (datiGenerici.IsDatiGenericiPensioneCiDatiDenericiNull())
                    return;
                else
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            }
            if (Utility.IsDomandaUnicarpe(FlagUnicarpe, TipoLetturaUnicarpe, true) == Utility.TipoUnicarpe.Automatica)
                datiPensioniDatiGenerici.DecorrenzaBonus = datiGenerici.DecorrenzaBonus;

            Utility.ValorizzaOggetti(datiGenerici, datiPensioniDatiGenerici);

            if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(idPensione);
            else
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiPensioniDatiGenerici);
        }

        private static void StoreDatiGenericiPerDatiPagamento(long idPensione, Entity.DatiGenerici datiGenerici, GestionePagamento.DatiPagamento datiPagamento)
        {
            if (datiPagamento == null)
            {
                if (datiGenerici.IsDatiGenericiPagamentoNull())
                    return;
                else
                    datiPagamento = new GestionePagamento.DatiPagamento();
            }

            Utility.ValorizzaOggetti(datiGenerici, datiPagamento);

            if (datiPagamento.Equals(new GestionePagamento.DatiPagamento()))
                GestionePagamento.EliminaPagamentoByIdPensione(idPensione);
            else
                GestionePagamento.SalvaPagamento(idPensione, datiPagamento);
        }

        private static void StoreDatiGenericiPerDatiNuoveLiquidate(long idPensione, Entity.DatiGenerici datiGenerici, GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate)
        {
            if (nuoveLiquidate == null)
            {
                if (datiGenerici.IsDatiGenericiNuoveLiquidateNull())
                    return;
                else
                    nuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();
            }

            Utility.ValorizzaOggetti(datiGenerici, nuoveLiquidate);

            if (nuoveLiquidate.Equals(new GestioneNuoveLiquidate.NuoveLiquidate()))
                GestioneNuoveLiquidate.EliminaNuoveLiquidateByIdPensione(idPensione);
            else
            {
                nuoveLiquidate.IdPensione = idPensione;
                GestioneNuoveLiquidate.SalvaNuoveLiquidate(nuoveLiquidate);
            }
        }

        private static bool ControlsDatiGenericiForPensioneProvenienza(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool? TrasformazioneAOI, byte? causaCarico, GestionePensione.DatiPensione datiPensione, bool IsDeleteOperation)
        {
            //if (IsDeleteOperation || ((!TrasformazioneAOI.HasValue || !TrasformazioneAOI.Value) && TrasformazioneAOI.HasValue && TrasformazioneAOI.Value))
            if ((IsDeleteOperation || ((!TrasformazioneAOI.HasValue || !TrasformazioneAOI.Value) && (causaCarico != 3 && causaCarico != 9))) && !Utility.IsDomandaRiliquidazioneAOI(datiPensione))
            {
                if (datiIstruttoria != null)
                {
                    Entity.DatiProvenienza datiProvenienza = new DatiProvenienza();
                    Utility.ValorizzaOggetti(datiIstruttoria, datiProvenienza);
                    if (!datiProvenienza.IsDatiProvenienzaIstruttoriaNull())
                        return false;
                }
            }

            return true;
        }

        private static bool ControlsDatiGenericiForBititolaritaAltraPensioneByIdPensione(long idPensione, string NaturaPensione, bool IsDeleteOperation)
        {
            List<Entity.AltraPensione> ElencoAltraPensione = null;
            GestioneBititolarita.GetDatiAltraPensioneByIdPensione(idPensione, out ElencoAltraPensione);

            if (ElencoAltraPensione != null && ElencoAltraPensione.Count > 0 && (IsDeleteOperation || !Utility.IsBititolaritaVisible(NaturaPensione)))
                return false;

            return true;
        }

        private static bool ControlsDatiGenericiForMaggBenefici(Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici, Entity.DatiMaggiorazioni datiMaggiorazioni, bool? exCombattente, bool? benefici, bool? maggiorazioni, bool IsDeleteOperation, out string errore)
        {
            errore = string.Empty;

            if (exCombattente.HasValue && !exCombattente.Value || (IsDeleteOperation && exCombattente.HasValue && exCombattente.Value))
            {
                if (datiExCombattente != null && !datiExCombattente.IsDatiExCombattenteNull())
                {
                    errore = "Eliminare i dati Ex Combattente di Maggiorazione / Benefici prima di procedere";
                    return false;
                }
            }
            if (benefici.HasValue && !benefici.Value || (IsDeleteOperation && benefici.HasValue && benefici.Value))
            {
                if (datiBenefici != null && !datiBenefici.IsDatiBeneficiNull())
                {
                    errore = "Eliminare i dati Benefici di Maggiorazione / Benefici prima di procedere.";
                    return false;
                }
            }

            if (maggiorazioni.HasValue && !maggiorazioni.Value || (IsDeleteOperation && maggiorazioni.HasValue && maggiorazioni.Value))
            {
                if (datiMaggiorazioni != null && !datiMaggiorazioni.IsDatiMaggiorazioniNull())
                {
                    errore = "Eliminare i dati Maggiorazioni di Maggiorazione / Benefici prima di procedere.";
                    return false;
                }
            }

            return true;
        }

        #endregion dati Generici

        #region dati Assicurativi
        public static void GetDatiAssicurativi(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool isRiaperturaDomanda, out Entity.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi = null;

            if (datiPensione == null)
                return;

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiCIGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiCIGenerici);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEE);

            GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11 = null;
            GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(datiPensione.Id, out integrazioneArt11);

            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);

            string codiceStato = string.Empty;
            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
                codiceStato = listaPrestazioniEE[0].CodiceStatoEE;

            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
            GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceConvenzionePerStato(codiceStato, datiPensione.DecorrenzaOriginaria, out listaCodiciConvenzione);

            if (datiCIGenerici == null && datiIstruttoria == null && integrazioneArt11 == null)
                return;

            datiAssicurativi = new Entity.DatiAssicurativi();

            Utility.ValorizzaOggetti(datiPensione, datiAssicurativi);
            //in caso di usuranti i campi devono essere precompilati con 67 e 011
            if (Utility.IsDomandaUsuranti(datiPensione))
            {
                datiAssicurativi.AttivitaEconomica = 67;
                datiAssicurativi.ProfessioneIndividuale = 011;
            }

            Utility.ValorizzaOggetti(datiCIGenerici, datiAssicurativi);
            Utility.ValorizzaOggetti(datiIstruttoria, datiAssicurativi);

            if (integrazioneArt11 != null)
                datiAssicurativi.ImportoIVS_Art11 = integrazioneArt11.ImportoIVS;

            //valore derivato dalla somme del numero settimane presenti nei dati contributivi e retributivi
            if (!datiAssicurativi.SettimaneItalianeMisura.HasValue)
                datiAssicurativi.SettimaneItalianeMisura = GetNumeroSettimaneItalianeMisura(listaDatiCalcoloContributivo, listaDatiCalcoloRetributivo);

            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0 && listaPrestazioniEE[0].CodiceConvenzione != null && listaPrestazioniEE[0].CodiceConvenzione > 0)
                datiAssicurativi.CodiceConvenzione = listaPrestazioniEE[0].CodiceConvenzione;
            else if (listaCodiciConvenzione != null && listaCodiciConvenzione.Count > 0)
                datiAssicurativi.CodiceConvenzione = listaCodiciConvenzione[0].CodiceConvenzione;

            if (datiCIGenerici != null)
                datiAssicurativi.UfficioPagatoreArretratiEsteri = Utility.GetUfficioPagatoreFromId(datiCIGenerici.UfficioPagatoreArretratiEE);
            //else
            //    datiAssicurativi.UfficioPagatoreArretratiEsteri = GetUfficioPagatoreArretratiEEFromListaPrestazioniEE(listaPrestazioniEE);

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (listaDatiCalcoloRetributivo != null)
                {
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo retrib in listaDatiCalcoloRetributivo)
                    {
                        if (retrib.QuotePrimeLiquidate == 'A' && retrib.CodiceGestione == 1)
                            datiAssicurativi.SettimaneOBGMisura12_92 = retrib.NSettimaneQuotaA;
                        if (retrib.QuotePrimeLiquidate == 'B' && retrib.CodiceGestione == 1)
                            datiAssicurativi.SettimaneOBGMisuraDL503_92 = retrib.NSettimaneQuotaB;
                    }
                }
            }

            if (datiAssicurativi.IsDatiAssicurativiPensioneCiGenericiNull() && datiAssicurativi.IsDatiAssicurativiPensioneNull() &&
                datiAssicurativi.IsDatiAssicurativiIstruttoriaNull() && datiAssicurativi.IsDatiAssicurativiIntegrazioneArt11Null() && !datiAssicurativi.CodiceConvenzione.HasValue)
                datiAssicurativi = null;
        }

        public static void StoreDatiAssicurativi(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            Entity.DatiAssicurativi datiAssicurativi, bool IsCancelOperation)
        {
            if (datiAssicurativi == null)
                datiAssicurativi = new Entity.DatiAssicurativi();

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11 = null;
            GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(datiPensione.Id, out integrazioneArt11);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            #region Unicarpe

            // i dati provenienti da felpe sono non modificabili e non cancellabili
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (datiPensioniDatiGenerici != null)
                {
                    datiAssicurativi.ImportoIVS = datiPensioniDatiGenerici.ImportoIVS;
                    datiAssicurativi.NContributiItalia = datiPensioniDatiGenerici.NContributiItalia;
                    datiAssicurativi.NSettFittiziePrepensionamento = datiPensioniDatiGenerici.NSettFittiziePrepensionamento;
                    datiAssicurativi.VVMisuraAl1292 = datiPensioniDatiGenerici.VVMisuraAl1292;
                    datiAssicurativi.VVMisuraDL50392 = datiPensioniDatiGenerici.VVMisuraDL50392;
                }

                if (datiIstruttoria != null)
                {
                    datiAssicurativi.NSettimaneOBG = datiIstruttoria.NSettimaneOBG;
                    datiAssicurativi.NContributiVolontari = datiIstruttoria.NContributiVolontari;
                    datiAssicurativi.NSettGodimentoAssegno = datiIstruttoria.NSettGodimentoAssegno;

                }
            }
            #endregion Unicarpe

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiAssicurativiPerPensione(datiAssicurativi, datiPensione);
                StoreDatiAssicurativiPerPensioneCiGenerici(datiPensione.Id, datiAssicurativi, ref datiPensioniDatiGenerici);
                StoreDatiAssicurativiPerIstruttoria(datiPensione.Id, datiAssicurativi, ref datiIstruttoria);
                StoreDatiAssicurativiPerIntegrazioneArt11(datiPensione.Id, datiAssicurativi, integrazioneArt11);
                StoreDatiAssicurativiPerPrimoStatoEstero(datiPensione.Id, datiAssicurativi, listaPrestazioniEstere, IsCancelOperation);

                if ((datiAssicurativi.IsDatiAssicurativiPensioneNull() && datiAssicurativi.IsDatiAssicurativiPensioneCiGenericiNull() &&
                    datiAssicurativi.IsDatiAssicurativiIstruttoriaNull() && datiAssicurativi.IsDatiAssicurativiIntegrazioneArt11Null()) || IsCancelOperation)
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                else
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        private static void StoreDatiAssicurativiPerPensione(Entity.DatiAssicurativi datiAssicurativi, GestionePensione.DatiPensione datiPensione)
        {
            // i dati provenienti da felpe sono non modificabili e non cancellabili
            //if (datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value && datiPensione.TipoLetturaUnicarpe.HasValue && datiPensione.TipoLetturaUnicarpe.Value == 'L')
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (datiPensione.InizioAssicurazione.HasValue)  // se il servizio non me lo ha prepopolato prendo il valore della pagina
                    datiAssicurativi.InizioAssicurazione = datiPensione.InizioAssicurazione;
                if (datiPensione.FineAssicurazione.HasValue)   // se il servizio non me lo ha prepopolato prendo il valore della pagina
                    datiAssicurativi.FineAssicurazione = datiPensione.FineAssicurazione;
            }
            Utility.ValorizzaOggetti(datiAssicurativi, datiPensione);

            GestionePensione.SalvaPensione(datiPensione);
        }

        private static void StoreDatiAssicurativiPerPensioneCiGenerici(long idPensione, Entity.DatiAssicurativi datiAssicurativi,
            ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            if (datiPensioniDatiGenerici == null)
            {
                if (datiAssicurativi.IsDatiAssicurativiPensioneCiGenericiNull())
                    return;
                else
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            }

            datiAssicurativi.SettimaneItalianeMisura = datiPensioniDatiGenerici.SettimaneItalianeMisura;

            Utility.ValorizzaOggetti(datiAssicurativi, datiPensioniDatiGenerici);

            //Valorizzazione ufficiopagatorearretratiEE a partire dalla descrizione
            if (!string.IsNullOrEmpty(datiAssicurativi.UfficioPagatoreArretratiEsteri))
                datiPensioniDatiGenerici.UfficioPagatoreArretratiEE = Utility.GetIdFromUfficioPagatore(datiAssicurativi.UfficioPagatoreArretratiEsteri.ToUpperInvariant());
            else datiPensioniDatiGenerici.UfficioPagatoreArretratiEE = null;

            if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(idPensione);
            else
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiPensioniDatiGenerici);
        }

        private static void StoreDatiAssicurativiPerIstruttoria(long idPensione, Entity.DatiAssicurativi datiAssicurativi, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiAssicurativi.IsDatiAssicurativiIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }

            Utility.ValorizzaOggetti(datiAssicurativi, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
        }

        private static void StoreDatiAssicurativiPerIntegrazioneArt11(long idPensione, DatiAssicurativi datiAssicurativi, GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11)
        {
            if (integrazioneArt11 == null)
            {
                if (datiAssicurativi.IsDatiAssicurativiIntegrazioneArt11Null())
                    return;
                else
                    integrazioneArt11 = new GestioneIntegrazioneArt11.IntegrazioneArt11();
            }

            integrazioneArt11.ImportoIVS = datiAssicurativi.ImportoIVS_Art11;

            if (integrazioneArt11.Equals(new GestioneIntegrazioneArt11.IntegrazioneArt11()))
                GestioneIntegrazioneArt11.EliminaIntegrazioneArt11ByIdPensione(idPensione);
            else
                GestioneIntegrazioneArt11.SalvaIntegrazioneArt11(idPensione, integrazioneArt11);
        }

        private static void StoreDatiAssicurativiPerPrimoStatoEstero(long idPensione, DatiAssicurativi datiAssicurativi, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere,
            bool isCancelOperation)
        {
            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 && listaPrestazioniEstere.First() != null && !isCancelOperation)
            {
                listaPrestazioniEstere.First().CodiceConvenzione = datiAssicurativi.CodiceConvenzione;
                listaPrestazioniEstere.First().IdPensione = idPensione;
                GestioneDatiContributiviCi.SalvaPrestazioneEstera(listaPrestazioniEstere.First());
            }
        }

        public static bool ControlDatiAssicurativi(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, Entity.DatiAssicurativi datiAssicurativi, Entity.DatiGenerici datiGenerici,
            Entity.DatiOpzione datiOpzione, Entity.DatiProvenienza datiProvenienza, DateTime dataSistema, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiAssicurativi == null)
                return true;

            if (!datiAssicurativi.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "Il campo 'Inizio Assicurazione' è obbligatorio";
                return false;
            }

            if (!datiAssicurativi.FineAssicurazione.HasValue)
            {
                messaggioVideo = "Il campo 'Fine Assicurazione' è obbligatorio";
                return false;
            }

            if (!ControlsCrossDatiAssicurativi(datiPensione, datiIstruttoriaCommon, datiMaggiorazioniBeneficiCommon, datiAssicurativi, datiGenerici, datiOpzione, datiProvenienza, dataSistema, IsSingleTab,
                isRiaperturaDomanda, out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsCrossDatiAssicurativi(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, Entity.DatiAssicurativi datiAssicurativi, Entity.DatiGenerici datiGenerici,
            Entity.DatiOpzione datiOpzione, Entity.DatiProvenienza datiProvenienza, DateTime dataSistema, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausa != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausa.IdAnagrafica, out datiAnagraficiDC);

            List<GestioneCalcolo.DatiCalcoloRetributivo> lDatiCalcoloRetrib = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out lDatiCalcoloRetrib);

            List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiContributivi);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            List<GestioneContrib.StatoEstero> listaStatiEsteri = null;
            GestioneContrib.GetStatiEEfromDBByIdPensione(datiPensione.Id, listaPrestazioniEstere, out listaStatiEsteri);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaDatiSupplementi = null;
            GestioneSupplementi.GetSupplementiByIdPensione(datiPensione.Id, out listaDatiSupplementi);

            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> listaMaternitaAcna = null;
            GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(datiPensione.Id, out listaMaternitaAcna);

            AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);

            List<GestioneCalcolo.DatiCalcoloContributivoEstero> listaDatiCalcoloContributivoEstero = null;
            GestioneCalcolo.GetCalcoloContributivoEsteroCIbyIdPensione(datiPensione.Id, out listaDatiCalcoloContributivoEstero);

            List<GestioneDecodifica.CodeGestione> listaCodiciGestione = null;
            GestioneDecodifica.GetCodiceGestione(out listaCodiciGestione);

            string codicePrimoStato = string.Empty;
            string nomeStato = string.Empty;
            byte? codiceConvenzione = datiAssicurativi.CodiceConvenzione;
            int codicePrimoStatoEE = 0;
            if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            {
                codicePrimoStato = listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE;
                nomeStato = listaStatiEsteri[0].PrestazioneEstera.NomeStato;
                int.TryParse(codicePrimoStato, out codicePrimoStatoEE);
            }

            int? nSettimane = null;
            if (ldatiContributivi != null && ldatiContributivi.Count > 0)
                nSettimane = ldatiContributivi[0].NSettimane;

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
            decimal? importoContributivoTotaleCodGestione1 = null;
            decimal? importoContributivoTotaleCodGestione2 = null;
            decimal? importoContributivoTotaleCodGestione3 = null;
            decimal? importoContributivoTotaleCodGestione4 = null;
            decimal? montanteContributivoQuotaDCodGestione1 = null;
            decimal? importoContributivoTotaleQuotaDCodGestione1 = null;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo retr in lDatiCalcoloRetrib)
                {
                    if (retr.CodiceGestione == 1)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            settimaneRetributiveQuotaACodGestione1 = retr.NSettimaneQuotaA;
                            rmsQuotaACodGestione1 = retr.RMSQuotaA;
                        }
                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            settimaneRetributiveQuotaBCodGestione1 = retr.NSettimaneQuotaB;
                            rmsQuotaBCodGestione1 = retr.RMSQuotaB;
                        }
                    }

                    if (retr.CodiceGestione == 2)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione2 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione2 = retr.NSettimaneQuotaA;
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione2 = retr.RMSQuotaB;
                            settimaneRetributiveQuotaBCodGestione2 = retr.NSettimaneQuotaB;
                        }
                    }

                    if (retr.CodiceGestione == 3)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione3 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione3 = retr.NSettimaneQuotaA;
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione3 = retr.RMSQuotaB;
                            settimaneRetributiveQuotaBCodGestione3 = retr.NSettimaneQuotaB;
                        }
                    }

                    if (retr.CodiceGestione == 4)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione4 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione4 = retr.NSettimaneQuotaA;
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione4 = retr.RMSQuotaB;
                            settimaneRetributiveQuotaBCodGestione4 = retr.NSettimaneQuotaB;
                        }
                    }
                }
            }
            //int settimaneQuotaATotale = settimaneRetributiveQuotaACodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione2.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione4.GetValueOrDefault();
            //int settimaneQuotaBTotale = settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione2.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault();

            if (ldatiContributivi != null && ldatiContributivi.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo contr in ldatiContributivi)
                {
                    if (contr.CodiceGestione == 1)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione1 = contr.NSettimane;
                            importoContributivoTotaleCodGestione1 = contr.ImportoContributivoTotale;
                            montanteCodGestione1 = contr.Montante;
                        }
                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                        {
                            settimaneContributiveDL214CodGestione1 = contr.NSettimaneQuotaDL214;
                            importoContributivoTotaleQuotaDCodGestione1 = contr.ImportoContribTotaleQuotaDL214;
                            montanteContributivoQuotaDCodGestione1 = contr.MontanteQuotaDL214;
                        }
                    }
                    if (contr.CodiceGestione == 2)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione2 = contr.NSettimane;
                            importoContributivoTotaleCodGestione2 = contr.ImportoContributivoTotale;
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                            settimaneContributiveDL214CodGestione2 = contr.NSettimaneQuotaDL214;
                    }

                    if (contr.CodiceGestione == 3)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione3 = contr.NSettimane;
                            importoContributivoTotaleCodGestione3 = contr.ImportoContributivoTotale;
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                            settimaneContributiveDL214CodGestione3 = contr.NSettimaneQuotaDL214;
                    }

                    if (contr.CodiceGestione == 4)
                    {
                        if (contr.NSettimane.HasValue || contr.ImportoContributivoTotale.HasValue || contr.Montante.HasValue)
                        {
                            settimaneContributiveCodGestione4 = contr.NSettimane;
                            importoContributivoTotaleCodGestione4 = contr.ImportoContributivoTotale;
                        }

                        if (contr.NSettimaneQuotaDL214.HasValue || contr.ImportoContribTotaleQuotaDL214.HasValue || contr.MontanteQuotaDL214.HasValue)
                            settimaneContributiveDL214CodGestione4 = contr.NSettimaneQuotaDL214;
                    }
                }
            }
            int settimaneQuotaATotale = settimaneRetributiveQuotaACodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione2.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione4.GetValueOrDefault();
            int settimaneQuotaBTotale = settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione2.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault();
            int settimaneQuotaCTotale = settimaneContributiveCodGestione1.GetValueOrDefault() + settimaneContributiveCodGestione2.GetValueOrDefault() + settimaneContributiveCodGestione3.GetValueOrDefault() + settimaneContributiveCodGestione4.GetValueOrDefault();
            int settimaneQuotaDTotale = settimaneContributiveDL214CodGestione1.GetValueOrDefault() + settimaneContributiveDL214CodGestione2.GetValueOrDefault() + settimaneContributiveDL214CodGestione3.GetValueOrDefault() + settimaneContributiveDL214CodGestione4.GetValueOrDefault();

            int? sommaSettimaneDirittoEstere = null;
            foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
            {
                sommaSettimaneDirittoEstere = sommaSettimaneDirittoEstere.GetValueOrDefault() + prestEE.ContributiEEDiritto.GetValueOrDefault();
            }

            //////////////////////////////// settiamo il numero di settimane in base alla categoria////////////////////
            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);
            int? settimane = GestioneControlli.NumeroSettimane(datiAssicurativi.SettimaneItalianeDiritto, datiAssicurativi.NSettimaneOBG, datiIstruttoria != null ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi : null);
            if (categoria > 0 && categoria < 7)
            {
                settimane = settimane.GetValueOrDefault() + datiAssicurativi.NContributiVolontari.GetValueOrDefault();
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            List<char> listaSigleFamiliari = new List<char> { 'I', 'M', 'S', 'U', 'N', 'J', 'Z', 'W', 'K' };

            bool presenzaOrfano = listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.FindIndex(x => listaSigleFamiliari.Contains(x.SiglaFamiliare.Value)) > -1;

            if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivoEstero contrEstero in listaDatiCalcoloContributivoEstero)
                {
                    short? codiceGestioneTraduzioneSuGP = 0;
                    if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                    {
                        GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == contrEstero.CodiceGestione.Value);
                        if (codeGestione != null)
                            codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                    }
                }
            }

            long? codiceGestioneContributiEsteri = null;
            if (listaDatiCalcoloContributivoEstero != null && listaDatiCalcoloContributivoEstero.Count > 0)
                codiceGestioneContributiEsteri = listaDatiCalcoloContributivoEstero[0].CodiceGestione;

            if (datiAnagraficiTitolare == null)
                datiAnagraficiTitolare = new GestioneAnagrafica.DatiAnagrafici();

            if (datiGenericiAgoCi == null)
                datiGenericiAgoCi = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

            if (datiPensione == null)
            {
                messaggioVideo = "Dati Pensione obbligatori.";
                return false;
            }

            if (IsSingleTab)
            {
                GetDatiGenerici(datiPensione, datiIstruttoria, isRiaperturaDomanda, out datiGenerici);
                GetDatiOpzione(datiPensione.Id, datiIstruttoria, out datiOpzione);
                GetDatiProvenienza(datiPensione.Id, datiIstruttoria, out datiProvenienza);
            }

            if (!GestioneCrossControls.ALL_ControlsInizioAssicurazioneSperimentaleDonna(datiPensione, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                return false;

            //if (!GestioneControlli.VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiAssicurativi.NSettimaneOBG, datiAssicurativi.NContributiVolontari, datiAssicurativi.SettimaneItalianeDiritto, listaPrestazioniEstere,
            //    datiAnagraficiTitolare, dataSistema, out messaggioVideo))
            //    return false;

            #region Controlli Codice Virtuale

            if (!GestioneControlli.ControlsCodiceVirtuale(datiPensione, isRiaperturaDomanda, codiceConvenzione, datiAssicurativi.CodiceVirtuale, out messaggioVideo))
                return false;

            ////COMMENTATO. NON RICHIAMARE FINO A NUOVE SPECIFICHE
            //if (!GestioneControlli.VerificaCodiceConvenzioneWithCittadinanza(datiAssicurativi.CodiceConvenzione, datiAnagrafici.Cittadinanza, datiPensione.Gruppo))
            //{
            //    messaggioVideo = "Convenzione Svizzera incompatibile con cittadinanza - Attenzione attualemente non gestiamo la cittadinanza!!!";
            //    return false;
            //}

            #endregion Controlli Codice Virtuale

            #region Data Inizio Assicurazione

            if (!GestioneControlli.VerificaInizioAssicurazione(datiPensione, datiAssicurativi.InizioAssicurazione, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaInizioAssicurazioneWithDataNascitaTitolare(datiAssicurativi.InizioAssicurazione, datiAnagraficiTitolare.DataNascita))
            {
                messaggioVideo = "Data Inizio Assicurazione incompatibile con Data di Nascita.";
                return false;
            }

            if (!GestioneControlli.VerificaInizioAssicurazioneWithDecorrenzaOriginaria(datiAssicurativi.InizioAssicurazione, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Data Inizio Assicurazione posteriore a Decorrenza.";
                return false;
            }

            #endregion Data Inizio Assicurazione

            #region Data Fine Assicurazione

            if (!GestioneControlli.VerificaFineAssicurazione(datiAssicurativi.FineAssicurazione))
            {
                messaggioVideo = "Data Ultimo Contributo illogica o mancante.";
                return false;
            }

            if (!GestioneControlli.VerificaInizioAssicurazioneWithFineAssicurazione(datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione))
            {
                messaggioVideo = "Data Ultimo Contributo anteriore a Data Inizio Assicurazione.";
                return false;
            }

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (!GestioneControlli.VerificaFineAssicurazioneWithDecorrenzaOriginaria(datiAssicurativi.FineAssicurazione, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Data Ultimo Contributo posteriori a Decorrenza.";
                return false;
            }

            #endregion Data Fine Assicurazione

            #region Controlli Settimane Effettive

            if (!GestioneControlli.VerificaSettimaneEffettiveCodiceStatoEE(datiAssicurativi.NContributiItalia, codicePrimoStatoEE.ToString()))
            {
                messaggioVideo = "Settimane Effettive mancanti.";
                return false;
            }

            if (!GestioneControlli.VerificaSettinaneEffettiveNSettimaneOBG(datiAssicurativi.NSettimaneOBG, datiAssicurativi.NContributiItalia))
            {
                messaggioVideo = "Settimane Effettive mancanti.";
                return false;
            }

            #endregion Controlli Settimane Effettive

            #region Controlli VV Misura Al 192

            if (datiOpzione == null)
                datiOpzione = new Entity.DatiOpzione();

            if (!GestioneControlli.VerificaSettVVMisuraWithDecOriginariaWithDecOpzione(datiAssicurativi.VVMisuraAl1292, datiPensione.DecorrenzaOriginaria, datiOpzione.DecorrenzaOpzione))
            {
                messaggioVideo = "Settimane VV per Misura incompatibili con Decorrenza ante 07/72.";
                return false;
            }

            if (!GestioneControlli.VerificaSettimaneVVMisuraWithDecorrenzaOriginaria(datiAssicurativi.VVMisuraAl1292, datiOpzione.DecorrenzaOpzione))
            {
                messaggioVideo = "Settimane VV per Misura incompatibili con Decorrenza ante 07/72.";
                return false;
            }

            if (!GestioneControlli.VerificaSettVVMisuraWithNContribVolontariWithImportoIVS(datiAssicurativi.VVMisuraAl1292, datiAssicurativi.NContributiVolontari, datiAssicurativi.ImportoIVS))
            {
                messaggioVideo = "Settimane VV per Misura incompatibili con Art. 11/488";
                return false;
            }

            if (!GestioneControlli.VerificaSettimaneVVMisuraWithNContributiVolontari(datiAssicurativi.VVMisuraAl1292, datiAssicurativi.NContributiVolontari))
            {
                messaggioVideo = "Settimane VV per Misura incompatibili con VV per diritto.";
                return false;
            }

            if (!GestioneControlli.VerificaSettVVMisuraWithDecOrigWithDecOpzioneWithNContribVolWithNsett(datiAssicurativi.VVMisuraAl1292, datiPensione.DecorrenzaOriginaria, datiOpzione.DecorrenzaOpzione,
                datiAssicurativi.NContributiVolontari, datiAssicurativi.ImportoIVS, datiAssicurativi.VVMisuraDL50392, nSettimane))
            {
                messaggioVideo = "Settimane VV per Misura mancanti o incompatibili con VV diritto.";
                return false;
            }

            #endregion Controlli VV Misura Al 192

            if (!GestioneControlli.VerificaCodiceConvenzioneWithStatoEstero(datiPensione, listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE : string.Empty,
                datiAssicurativi.CodiceConvenzione, datiPensione.Gruppo))
            {
                messaggioVideo = "Codice Convenzione errato o incompatibile con Stato " + nomeStato;
                return false;
            }

            if (!GestioneControlli.VerificaSettimaneDirittoConvenzioneCanada(codiceConvenzione, listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE : string.Empty, settimane, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaSettimaneDirittoConvenzioneRegnoUnito(codiceConvenzione, listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE : string.Empty, settimane, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceConvenzioneUruguayArgentina(datiPensione, datiAssicurativi.CodiceConvenzione, codicePrimoStatoEE, out messaggioVideo))
                return false;

            int settimaneItalianeMisura = datiAssicurativi.SettimaneItalianeMisura.HasValue ? datiAssicurativi.SettimaneItalianeMisura.Value :
                GestioneLiquidazionePensione.GetNumeroSettimaneItalianeMisura(ldatiContributivi, lDatiCalcoloRetrib);

            #region PCIPL39 Categoria >= 7
            if (categoria >= 7)
            {
                if (!GestioneControlli.VerificaSettimaneItalianeDiritto(settimane, datiAssicurativi.CodiceConvenzione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane(datiAssicurativi.CodiceConvenzione, datiGenerici.NaturaPensione, settimane, settimaneItalianeMisura, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneEffettive(datiAssicurativi.NContributiItalia, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportoIVS(datiAssicurativi.ImportoIVS, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiAssicurativi.NSettFittiziePrepensionamento, decorrenza,
                    datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                    datiAssicurativi.FineAssicurazione, datiGenerici.NaturaPensione, 1, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, datiAssicurativi.NSettFittiziePrepensionamento, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAssicurativi.FineAssicurazione, datiGenerici.NaturaPensione, 2, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione3, datiAssicurativi.NSettFittiziePrepensionamento, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAssicurativi.FineAssicurazione, datiGenerici.NaturaPensione, 3, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione4, settimaneRetributiveQuotaBCodGestione4, datiAssicurativi.NSettFittiziePrepensionamento, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAssicurativi.FineAssicurazione, datiGenerici.NaturaPensione, 4, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                //84: categoria minima per la comparazione all'interno del metodo. 87: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAssicurativi.FineAssicurazione,
                    datiPensione.NaturaPensione, datiAssicurativi.NSettFittiziePrepensionamento, 84, 87, out messaggioVideo))
                    return false;

                //87: categoria minima per la comparazione all'interno del metodo. 91: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAssicurativi.FineAssicurazione,
                    datiPensione.NaturaPensione, datiAssicurativi.NSettFittiziePrepensionamento, 87, 91, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCmsmWithSettimaneFittizie(datiGenericiAgoCi != null ? datiGenericiAgoCi.CMSM : null, datiAssicurativi.NSettFittiziePrepensionamento, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCmsmWithSettimaneFittizieAndImportiContribTot(datiGenericiAgoCi != null ? datiGenericiAgoCi.CMSM : null, decorrenza, datiAssicurativi.NSettFittiziePrepensionamento, datiAssicurativi.FineAssicurazione, importoContributivoTotaleCodGestione1,
                    importoContributivoTotaleCodGestione2, importoContributivoTotaleCodGestione3, importoContributivoTotaleCodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiGenericiAgoCi != null ? datiGenericiAgoCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione2, out messaggioVideo))
                {
                    messaggioVideo = "Contributi CD/CM dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiGenericiAgoCi != null ? datiGenericiAgoCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione3, out messaggioVideo))
                {
                    messaggioVideo = "Contributi ART dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiGenericiAgoCi != null ? datiGenericiAgoCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Contributi COM dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneQuotaBWithPeriodoAssicurativo(datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione,
                    datiGenericiAgoCi != null ? datiGenericiAgoCi.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione1,
                    datiGenerici != null ? datiGenerici.DataInizioCalcolo : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(1, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, settimaneContributiveCodGestione1, datiGenerici.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(2, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, settimaneContributiveCodGestione2, datiGenerici.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(3, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, settimaneContributiveCodGestione3, datiGenerici.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(4, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, settimaneContributiveCodGestione4, datiGenerici.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettWithCodReqPartAndNaturaPensione(datiAssicurativi.DeliberaCee126, datiAssicurativi.CodiceRequisitiParticolari, settimane,
                    datiGenerici.NaturaPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettGodimentoAssegnoWithCodReqParticolari(datiAssicurativi.NSettGodimentoAssegno, datiAssicurativi.CodiceRequisitiParticolari, datiPensione.Gruppo, tipoDomanda, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettimaneWithCodReqParticolari(datiPensione.Gruppo, sommaSettimaneDirittoEstere, datiAssicurativi.NSettGodimentoAssegno, settimane, datiAssicurativi.CodiceRequisitiParticolari, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMaggiori6(datiAssicurativi.NContributiItalia, settimane,
                    datiGenerici != null ? datiGenerici.DataInizioCalcolo : null, tipoDomanda, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, out messaggioVideo))
                    return false;
            }

            #endregion PCIPL39 Categoria >= 7

            #region Anni Differimento
            if (!GestioneControlli.VerificaAnniDifferimento(datiAssicurativi.AnniDifferimento, datiPensione.Gruppo))
            {
                messaggioVideo = "Anni di differimento incompatibili con la categoria della pensione";
                return false;
            }

            if (!GestioneControlli.VerificaAnniDifferimentoWithVOS(datiAssicurativi.AnniDifferimento, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Anni di differimento incompatibile con categoria VOS post 08/1976";
                return false;
            }
            #endregion Anni Differimento

            if (tipoDomanda == Utility.TipoDomanda.Superstiti)
            {
                if (!GestioneCrossControls.CI_ControlsCodiceVirtualeWithCertificatoDiretta(datiAssicurativi.CodiceVirtuale, datiDanteCausa.Certificato, listaPrestazioniEstere.Count() > 0? listaPrestazioniEstere[0].CodiceConvenzione : null, datiPensione.CausaCarico, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_VerificaAnniDifferimentoWithDanteCausa(datiAssicurativi.AnniDifferimento, datiDanteCausa.SiglaCategoria, datiDanteCausa.DecorrenzaPensione))
                {
                    messaggioVideo = "Anni Differimento incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaImportoIVS(datiPensione.SiglaCategoria, datiAssicurativi.ImportoIVS, datiDanteCausa.Certificato, datiDanteCausa.DataMorte, datiDanteCausa.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_VerificaRequisitoParticolareDirittoWithDanteCausa(datiAssicurativi.CodiceRequisitiParticolari, categoria, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.SiglaCategoria : null, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                    return false;
            }

            if (datiMaggiorazioniBenefici != null)
            {
                if (!GestioneCrossControls.CI_VerificaSentenza7290WithRms8888(datiMaggiorazioniBenefici.Aumento7290, datiAssicurativi.RMS8888, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.CI_VerificaAumentoLeggeArt2WithRms9090(datiMaggiorazioniBenefici.AumentoMensileLegge161289Art2, datiAssicurativi.RMS9090, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaOpzioneWithCodiceStato(datiOpzione.DecorrenzaOpzione, codicePrimoStatoEE, datiAssicurativi.CodiceConvenzione, settimane, out messaggioVideo))
                return false;

            #region Categorie minori o uguali a 6
            if (categoria > 0 && categoria <= 6)
            {
                if (!GestioneControlli.ControlsInizioAssicurazione(datiAssicurativi.InizioAssicurazione, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, datiAnagraficiTitolare.DataNascita, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsFineAssicurazione(datiAssicurativi.FineAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaSettimaneOBG(settimane.GetValueOrDefault() - datiAssicurativi.NContributiVolontari.GetValueOrDefault(), datiAssicurativi.CodiceConvenzione,
                    codicePrimoStatoEE, settimaneRetributiveQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneOBG(settimaneRetributiveQuotaACodGestione1, datiAssicurativi.VVMisuraAl1292, codicePrimoStatoEE, settimane, rmsQuotaACodGestione1, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaSettimaneVV(codiceConvenzione, settimaneRetributiveQuotaACodGestione1, settimane, datiAssicurativi.VVMisuraAl1292,
                    datiAssicurativi.VVMisuraDL50392, datiAssicurativi.ImportoIVS_Art11, datiAssicurativi.NContributiVolontari, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneVV(datiPensione, ldatiContributivi, datiAssicurativi.VVMisuraAl1292, decorrenza, datiOpzione.DecorrenzaOpzione, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiAssicurativi.NContributiVolontari, datiAssicurativi.ImportoIVS_Art11, datiAssicurativi.VVMisuraDL50392, settimaneContributiveCodGestione1, rmsQuotaACodGestione1, false, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportoIVSPost1976(datiAssicurativi.ImportoIVS, categoria, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimanePerCalcoloContributivoWithImportoIVS(datiAssicurativi.SettimanePerCalcoloContributivo, datiAssicurativi.ImportoIVS, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaImportoIVS(categoria, rmsQuotaACodGestione1, datiAssicurativi.ImportoIVS, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaImportoIVSWithDecorrenze(datiAssicurativi.ImportoIVS, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS8888WithRMSQuotaA(datiAssicurativi.RMS8888, rmsQuotaACodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS8888WithDecorrenza(datiAssicurativi.RMS8888, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS9090WithRMSQuotaA(datiAssicurativi.RMS9090, rmsQuotaACodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCapienzaSettimaneVV(datiAssicurativi.VVMisuraAl1292, datiAssicurativi.VVMisuraDL50392, datiAssicurativi.NContributiVolontari, out messaggioVideo))
                    return false;

                if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
                {
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo retr in lDatiCalcoloRetrib)
                    {
                        if (!GestioneControlli.VerificaNSettimaneQuotaAWithInizioAssicurazione(retr.NSettimaneQuotaA, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                            return false;
                    }
                }

                if (!GestioneControlli.VerificaRMSQuotaBWithSettimane(datiGenericiAgoCi != null ? datiGenericiAgoCi.ContributiItalianiEdEsteriAl1295 : null, settimaneQuotaCTotale, settimaneQuotaDTotale, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiAssicurativi.VVMisuraDL50392, datiAssicurativi.NSettFittiziePrepensionamento, datiGenericiAgoCi != null ? datiGenericiAgoCi.CMSM : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportoIVSArt11(datiAssicurativi.ImportoIVS_Art11, rmsQuotaACodGestione1, decorrenza, datiAssicurativi.NContributiVolontari, settimaneRetributiveQuotaACodGestione1, datiAssicurativi.VVMisuraAl1292, out messaggioVideo))
                    return false;

                //if (!GestioneControlli.VerificaCMSM(decorrenza, datiGenericiAgoCi != null ? datiGenericiAgoCi.CMSM : null, datiAssicurativi.NSettFittiziePrepensionamento, out messaggioVideo))
                //    return false;

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
                        if (!GestioneControlli.VerificaMontante335(decorrenza, datiAssicurativi.FineAssicurazione, montanteCodGestione1, datiGenerici.NaturaPensione, datiGenericiAgoCi != null ? datiGenericiAgoCi.CMSM : null, out messaggioVideo))
                            return false;
                    }
                }

                if (!GestioneControlli.VerificaSettGodimentoAssegnoAndCodReqParticolari(tipoDomanda, datiPensione.Gruppo, datiAssicurativi.CodiceRequisitiParticolari, datiAssicurativi.NSettGodimentoAssegno, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettimaneWithCodiceSedeAndCertificato(datiPensione, datiAssicurativi.CodiceRequisitiParticolari, settimane, datiAssicurativi.NContributiVolontari,
                    sommaSettimaneDirittoEstere, datiAssicurativi.NSettGodimentoAssegno, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCodReqParticolareAndProfIndivAndAttEconAndNumContribVolontari(datiAssicurativi.NContributiVolontari, datiAssicurativi.AttivitaEconomica, datiAssicurativi.CodiceRequisitiParticolari, datiAssicurativi.ProfessioneIndividuale, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMinori7(datiAssicurativi.NContributiItalia, settimane, datiAssicurativi.VVMisuraAl1292,
                    datiGenerici != null ? datiGenerici.DataInizioCalcolo : null, tipoDomanda,
                    listaDatiSupplementi != null && listaDatiSupplementi.Count > 0 ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, codicePrimoStatoEE,
                    datiDanteCausa != null ? datiDanteCausa.Certificato : null, out messaggioVideo))
                    return false;
            }
            #endregion Categorie minori o uguali a 6

            if (!GestioneControlli.VerificaOpzioneContributiva(datiPensione, datiGenerici != null ? datiGenerici.FlagContributiva : null, datiGenericiAgoCi != null ? datiGenericiAgoCi.ContributiItalianiEdEsteriAl1295 : null, settimaneQuotaATotale, settimaneQuotaBTotale, out messaggioVideo))
                return false;

            if (listaMaternitaAcna != null && listaMaternitaAcna.Count > 0)
            {
                foreach (GestioneDatiContributiviCi.PensioniCiMaternitaAcna maternitaAcna in listaMaternitaAcna)
                {
                    if (maternitaAcna.Tipo == 'A') // Acna
                    {
                        if (maternitaAcna.SettimaneAl1292.GetValueOrDefault() > 0 || maternitaAcna.SettimaneDL50392.GetValueOrDefault() > 0 || maternitaAcna.ImportoIVS.GetValueOrDefault() > 0)
                        {
                            if (!GestioneControlli.VerificaAcnaWithDatiAssicurativi(maternitaAcna.SettimaneAl1292, maternitaAcna.SettimaneDL50392, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, out messaggioVideo))
                                return false;
                        }
                    }
                }
            }

            if (tipoDomanda == Utility.TipoDomanda.Superstiti ||
                (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0 && areaTitolare.ElencoStatiCivili.FindIndex(x => x.Codice == '2') > -1) ||
                (listaFamiliari != null && listaFamiliari.Count > 0))
            {
                if (!GestioneControlli.VerificaContributiWithOrfano(settimane, datiAssicurativi.NSettFittiziePrepensionamento, datiAssicurativi.NSettGodimentoAssegno, tipoDomanda,
                    datiDanteCausa != null ? datiDanteCausa.Certificato : null, codiceConvenzione, datiPensione.DecorrenzaOriginaria, presenzaOrfano, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaFineAssicurazioneWithDataDomandaOpzione(datiAssicurativi.FineAssicurazione, decorrenza, datiOpzione.DataDomandaOpzione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaRMS8888WithOpzione(datiAssicurativi.RMS8888, decorrenza, datiOpzione.DecorrenzaOpzione, datiOpzione.DataDomandaOpzione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCapienzaSettimaneDL50392WithAssicurazione(datiPensione, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiAssicurativi.ProfessioneIndividuale, datiGenerici.NaturaPensione, datiPensione.DecorrenzaOriginaria, decorrenza, settimaneRetributiveQuotaBCodGestione1, datiAssicurativi.VVMisuraDL50392, datiAssicurativi.AttivitaEconomica, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceRequisitiParticolari(datiAssicurativi.CodiceRequisitiParticolari, categoria, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceRequisitiParticolariWithDatiGenerici(datiAssicurativi.CodiceRequisitiParticolari, tipoDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, datiPensione.Gruppo, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, out messaggioVideo))
                return false;

            if ((datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value) || datiGenerici.CausaCarico == 3 || datiGenerici.CausaCarico == 9)
            {
                if (!GestioneControlli.VerificaDatiPrecedentePensione(datiGenerici != null ? datiGenerici.CausaCarico : null, datiAssicurativi.CodiceRequisitiParticolari, datiGenerici != null ? datiGenerici.NaturaPensione : null, datiProvenienza != null ? datiProvenienza.CodiceP18PrecedentePensione : null, datiProvenienza != null ? datiProvenienza.CertificatoPrecedentePensione : null, datiProvenienza != null ? datiProvenienza.SedePrecedentePensione : null,
                    categoria, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiProvenienza != null ? datiProvenienza.DecorrenzaOriginariaAltraPensione : null, datiGenerici != null ? datiGenerici.TrasformazioneAOI : null, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaDecorrenzaCodiceVirtuale(datiGenerici != null ? datiGenerici.CausaCarico : null, datiAssicurativi.DecorrenzaCodiceVirtuale, datiAssicurativi.CodiceVirtuale, datiPensione.DecorrenzaOriginaria, codiceConvenzione, out messaggioVideo))
                return false;

            if (IsSingleTab)
            {
                if (!GestioneControlli.VerificaCodiceVirtualeWithCausaCarico(datiAssicurativi.CodiceVirtuale, datiPensione.CausaCarico))
                {
                    messaggioVideo = "Codice Virtuale 6 ammesso solo in Ricostituzione o Causa Carico 9.";
                    return false;
                }

                //Da verificare!!!!!!!!!!!!! Leggere commento a lato del documento di specifiche
                ////COMMENTATO. NON RICHIAMARE FINO A NUOVE SPECIFICHE
                //if (!GestioneControlli.VerificaCodiceConvenzioneWithCodiceVirtualeReversibilita(datiAssicurativi.CodiceConvenzione, datiAssicurativi.CodiceVirtuale, datiPensione.CausaCarico, datiPensione.Gruppo, out messaggioVideo))
                //    return false;

                if (!GestioneControlli.VerificaDecorrenzaOriginariaWithCodNaturaAndDataPresentazione(datiPensione, datiPensione.CausaCarico, datiPensione.NaturaPensione, datiAssicurativi.AttivitaEconomica,
                    datiAssicurativi.ProfessioneIndividuale, out messaggioVideo))
                    return false;

                if (!VerificaRequisitiAnzianita9496Vecchiaia94(datiAssicurativi.RequisitiAl1294, datiAssicurativi.RequisitiVecchiaiaAl1294, datiAssicurativi.RequisitiAl996, datiPensione.NaturaPensione,
                    datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, null, isRiaperturaDomanda, datiPensione, out messaggioVideo))
                    return false;

                #region Controlli R.M.S.

                if (datiOpzione == null)
                    GestioneLiquidazionePensione.GetDatiOpzione(datiPensione.Id, datiIstruttoria, out datiOpzione);

                if (datiGenerici == null)
                    GestioneLiquidazionePensione.GetDatiGenerici(datiPensione, datiIstruttoria, isRiaperturaDomanda, out datiGenerici);

                if (datiOpzione == null)
                    datiOpzione = new DatiOpzione();

                if (datiGenerici == null)
                    datiGenerici = new DatiGenerici();

                if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in lDatiCalcoloRetrib)
                    {
                        if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiRetributivi.RMSQuotaA,
                            datiAssicurativi.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                            datiOpzione != null ? datiOpzione.DecorrenzaOpzione : null, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione, datiPensione.Gruppo, datiPensione.Prodotto))
                        {
                            messaggioVideo = "R.M.S. mancante.";
                            return false;
                        }
                    }

                #endregion Controlli R.M.S.

                #region Controlli OBG Misura 503 o Contributi 335

                ////COMMENTATO. NON RICHIAMARE FINO A NUOVE SPECIFICHE
                //List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
                //GestioneCalcolo.GetCalcoloContributivoCI_AGOByPensione(datiPensione.Id, out ldatiContributivi);

                //List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
                //GestioneCalcolo.GetCalcoloRetributivoCI_AGOByPensione(datiPensione.Id, out ldatiRetributivi);

                //GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                //GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);

                //if (datiGenerici == null)
                //    GestioneLiquidazionePensione.GetDatiGenerici(numeroDomanda, out datiGenerici);

                //if (datiGenerici == null)
                //    datiGenerici = new DatiGenerici();

                //int? nSettimane = null;
                //if (ldatiContributivi != null && ldatiContributivi.Count > 0)
                //    nSettimane = ldatiContributivi[0].NSettimane;

                //if (ldatiRetributivi != null && ldatiRetributivi.Count > 0)
                //    foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in ldatiRetributivi)
                //        if (!GestioneControlli.VerificaOBGMisura335Contributi335(datiAssicurativi.FineAssicurazione, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione,
                //            datiRetributivi.NSettimaneQuotaB, nSettimane, datiAssicurativi.CodiceConvenzione, datiIstruttoria.NContributiVolontari))
                //        {
                //            messaggioVideo = "OBG Misura 503/92 o Contributi 335/95 mancanti.";
                //            return false;
                //        }

                #endregion Controlli OBG Misura 503 o Contributi 335

                #region Controlli Ufficio Pagatore Arretrati Esteri

                if (!ControlsUfficioPagatoreArretratiEsteri(datiAssicurativi.UfficioPagatoreArretratiEsteri, listaPrestazioniEstere, datiPensione.CodiceArretrati, datiAssicurativi.CodiceBloccoArretratiEE, out messaggioVideo))
                    return false;

                #endregion Controlli Ufficio Pagatore Arretrati Esteri

                if (!GestioneControlli.VerificaDelibera12688WithCodNatura(datiAssicurativi.DeliberaCee126, datiPensione.NaturaPensione, datiPensione.Gruppo))
                {
                    messaggioVideo = "Delibera 126/88 incompatibile con Natura Pensione";
                    return false;
                }

                if (!GestioneControlli.VerificaObbligatorietaAttivitaEconomicaWithCausaCarico(datiPensione.CausaCarico, datiAssicurativi.AttivitaEconomica))
                {
                    messaggioVideo = "Codice Attività Economica mancante";
                    return false;
                }

                if (!GestioneControlli.VerificaObbligatorietaProfessioneIndividualeWithCausaCarico(datiPensione.CausaCarico, datiAssicurativi.ProfessioneIndividuale))
                {
                    messaggioVideo = "Codice Professione Individuale mancante";
                    return false;
                }

                if (!GestioneControlli.ControlsCodNaturaCrossTab(datiPensione.NaturaPensione, datiPensione.Gruppo, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, datiPensione.DecorrenzaOriginaria, datiPensione.CausaCarico, datiPensione.CodiceTipoRichiesta, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsImportoCristallizzazione(datiAssicurativi.ImportoCristallizzazione3481, datiPensione.CausaCarico, datiPensione.SiglaCategoria, datiAssicurativi.CodiceVirtuale, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiAssicurativi.CodiceRequisitiParticolari, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCodNatura(datiAssicurativi.NSettFittiziePrepensionamento, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                #region Categorie minori o uguali a 6
                if (categoria > 0 && categoria <= 6)
                {
                    if (!GestioneControlli.ControlsDecorrenzaBonusWithFineAssicurazione(datiAssicurativi.FineAssicurazione, datiGenericiAgoCi != null ? datiGenericiAgoCi.DecorrenzaBonus : null, datiPensione.NaturaPensione, out messaggioVideo))
                        return false;


                    if (!GestioneControlli.VerificaCapienzaSettimaneWithAssicurazione(datiPensione, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, settimane,
                        datiAssicurativi.ProfessioneIndividuale, datiPensione.NaturaPensione, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaRMS9090WithDecorrenze(datiAssicurativi.RMS9090, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, decorrenza, datiGenericiAgoCi != null ? datiGenericiAgoCi.DecorrenzaArt2Dpcm : null, (listaDatiSupplementi != null && listaDatiSupplementi.Count > 0) ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, out messaggioVideo))
                        return false;
                }
                #endregion Categorie minori o uguali a 6
            }
            else
            {
                if (!GestioneControlli.VerificaCodiceVirtualeWithCausaCarico(datiAssicurativi.CodiceVirtuale, datiGenerici.CausaCarico))
                {
                    messaggioVideo = "Codice Virtuale 6 ammesso solo in Ricostituzione o Causa Carico 9.";
                    return false;
                }

                if (!GestioneControlli.VerificaCodiceConvenzioneWithCodiceVirtualeReversibilita(datiAssicurativi.CodiceConvenzione, datiAssicurativi.CodiceVirtuale, datiGenerici.CausaCarico,
                    datiPensione.Gruppo, datiPensione.Prodotto, out messaggioVideo))
                    return false;

                if (!VerificaRequisitiAnzianita9496Vecchiaia94(datiAssicurativi.RequisitiAl1294, datiAssicurativi.RequisitiVecchiaiaAl1294, datiAssicurativi.RequisitiAl996, datiGenerici.NaturaPensione,
                    datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, null, isRiaperturaDomanda, datiPensione, out messaggioVideo))
                    return false;

                #region Controlli R.M.S.

                if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in lDatiCalcoloRetrib)
                    {
                        if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiRetributivi.RMSQuotaA,
                            datiAssicurativi.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                            datiOpzione != null ? datiOpzione.DecorrenzaOpzione : null, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione, datiPensione.Gruppo, datiPensione.Prodotto))
                        {
                            messaggioVideo = "R.M.S. mancante.";
                            return false;
                        }
                    }

                #endregion Controlli R.M.S.

                #region Controlli OBG Misura 503 o Contributi 335

                ////COMMENTATO. NON RICHIAMARE FINO A NUOVE SPECIFICHE
                //List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
                //GestioneCalcolo.GetCalcoloContributivoCI_AGOByPensione(datiPensione.Id, out ldatiContributivi);

                //List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
                //GestioneCalcolo.GetCalcoloRetributivoCI_AGOByPensione(datiPensione.Id, out ldatiRetributivi);

                //GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                //GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);

                //int? nSettimane = null;
                //if (ldatiContributivi != null && ldatiContributivi.Count > 0)
                //    nSettimane = ldatiContributivi[0].NSettimane;

                //if (ldatiRetributivi != null && ldatiRetributivi.Count > 0)
                //    foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in ldatiRetributivi)
                //        if (!GestioneControlli.VerificaOBGMisura335Contributi335(datiAssicurativi.FineAssicurazione, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione,
                //            datiRetributivi.NSettimaneQuotaB, nSettimane, datiAssicurativi.CodiceConvenzione, datiIstruttoria.NContributiVolontari))
                //        {
                //            messaggioVideo = "OBG Misura 503/92 o Contributi 335/95 mancanti.";
                //            return false;
                //        }

                #endregion Controlli OBG Misura 503 o Contributi 335

                #region Controlli Ufficio Pagatore Arretrati Esteri

                if (datiGenerici != null && !ControlsUfficioPagatoreArretratiEsteri(datiAssicurativi.UfficioPagatoreArretratiEsteri, listaPrestazioniEstere, datiGenerici.CodiceArretrati, datiAssicurativi.CodiceBloccoArretratiEE, out messaggioVideo))
                    return false;

                #endregion Controlli Ufficio Pagatore Arretrati Esteri

                if (datiGenerici != null)
                {
                    if (!GestioneControlli.VerificaDecorrenzaOriginariaWithCodNaturaAndDataPresentazione(datiPensione, datiGenerici.CausaCarico, datiGenerici.NaturaPensione, datiAssicurativi.AttivitaEconomica,
                        datiAssicurativi.ProfessioneIndividuale, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaDelibera12688WithCodNatura(datiAssicurativi.DeliberaCee126, datiGenerici.NaturaPensione, datiPensione.Gruppo))
                    {
                        messaggioVideo = "Delibera 126/88 incompatibile con Natura Pensione";
                        return false;
                    }

                    if (!GestioneControlli.VerificaObbligatorietaAttivitaEconomicaWithCausaCarico(datiGenerici.CausaCarico, datiAssicurativi.AttivitaEconomica))
                    {
                        messaggioVideo = "Codice Attività Economica mancante";
                        return false;
                    }

                    if (!GestioneControlli.VerificaObbligatorietaProfessioneIndividualeWithCausaCarico(datiGenerici.CausaCarico, datiAssicurativi.ProfessioneIndividuale))
                    {
                        messaggioVideo = "Codice Professione Individuale mancante";
                        return false;
                    }

                    if (!GestioneControlli.ControlsCodNaturaCrossTab(datiGenerici.NaturaPensione, datiPensione.Gruppo, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, datiPensione.DecorrenzaOriginaria, datiGenerici.CausaCarico, datiPensione.CodiceTipoRichiesta, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.ControlsImportoCristallizzazione(datiAssicurativi.ImportoCristallizzazione3481, datiGenerici != null ? datiGenerici.CausaCarico : null, datiPensione.SiglaCategoria, datiAssicurativi.CodiceVirtuale, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiAssicurativi.CodiceRequisitiParticolari, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCodNatura(datiAssicurativi.NSettFittiziePrepensionamento, datiGenerici != null ? datiGenerici.NaturaPensione : null, out messaggioVideo))
                    return false;

                #region Categorie minori o uguali a 6
                if (categoria > 0 && categoria <= 6)
                {
                    if (!GestioneControlli.ControlsDecorrenzaBonusWithFineAssicurazione(datiAssicurativi.FineAssicurazione, datiGenerici != null ? datiGenerici.DecorrenzaBonus : null, datiGenerici != null ? datiGenerici.NaturaPensione : null, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaCapienzaSettimaneWithAssicurazione(datiPensione, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, settimane,
                        datiAssicurativi.ProfessioneIndividuale, datiGenerici != null ? datiGenerici.NaturaPensione : null, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.VerificaRMS9090WithDecorrenze(datiAssicurativi.RMS9090, datiOpzione != null ? datiOpzione.DecorrenzaOpzione : null, decorrenza, datiOpzione != null ? datiOpzione.DecorrenzaArt2Dpcm : null, (listaDatiSupplementi != null && listaDatiSupplementi.Count > 0) ? listaDatiSupplementi[0].DecorrenzaSupplemento : null, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlsSettWithCodReqPartAndNaturaPensione(datiAssicurativi.DeliberaCee126, datiAssicurativi.CodiceRequisitiParticolari, settimane,
                        datiGenerici.NaturaPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                        return false;
                }
                #endregion Categorie minori o uguali a 6
            }

            if (!GestioneCrossControls.ALL_VerificaFineAssicurazioneForReversibilita(tipoDomanda, datiAssicurativi.FineAssicurazione, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, tipoAppartenenza, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            /* ENG - 05/11/2024 Deprecata
            if (!GestioneControlli.ControlsLimiteSettimaneReversibilitaSloveniaCroazia(datiPensione, datiAssicurativi.SettimaneItalianeDiritto, datiAssicurativi.NSettimaneOBG,
                datiIstruttoria != null ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi : null, datiAssicurativi.NContributiVolontari, datiAssicurativi.CodiceConvenzione, codicePrimoStatoEE, out messaggioVideo))
                return false;
            */
            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(datiPensione, datiGenerici != null ? datiGenerici.NaturaPensione : null, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                return false;

            //ENG - Gestione Nuovo Codice CI28
            //if (!GestioneControlli.VerificaCodiceCI28(datiPensione, datiAssicurativi.CodiceConvenzione, datiAssicurativi.CodiceCI28, out messaggioVideo))
            //    return false;

            return true;
        }

        public static void EliminaDatiAssicurativi(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            out string msgVideo)
        {
            msgVideo = string.Empty;

            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);

            if ((listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0) || (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0))
            {
                msgVideo = "Eliminare i 'Dati Calcolo' prima di continuare.";
                return;
            }

            StoreDatiAssicurativi(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, new Entity.DatiAssicurativi(), true);
        }

        private static bool VerificaRequisitiAnzianita9496Vecchiaia94(bool? bReqAnz94, bool? bReqVec94, bool? bReqAnz96, string CodeNatura, string Gruppo, string prodotto, string Categoria,
            DateTime? Decorrenza, bool? isAssicurativiAcquisito, bool isRiaperturaDomanda, GestionePensione.DatiPensione datiPensione, out string msg)
        {
            msg = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_CI.BYPASS_REQ_ANZ9496_VECCH94))
                return true;

            if (!GestioneControlli.VerificaReqAnz96CategDecorrenza(bReqAnz96, CodeNatura, Gruppo, prodotto, Decorrenza, isRiaperturaDomanda))
            {
                msg = "Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz96_CatVOS_9596(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 09/96 incompatibile con categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz96_CatMix_9597(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 09/96 incompatibile con categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqVec94CategDecorrenza(bReqVec94, CodeNatura, Gruppo, prodotto, Decorrenza, isRiaperturaDomanda))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqVec94_CatVOS_9709(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqVec94_CatVOS_97(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqVec94_CatMix_9709(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqVec94_CatMix_97(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqVec94_CodNatura_9509(bReqVec94, CodeNatura, Gruppo, prodotto, Decorrenza, isRiaperturaDomanda))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94CategDecorrenza(bReqAnz94, CodeNatura, Gruppo, prodotto, Decorrenza, isRiaperturaDomanda))
            {
                msg = "Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94_CatVOS_9596(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94_CatVOS_9596(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94Anz96_CatVOS_9698(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94Anz96_CatVOS_9698(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94Anz96_CatVOS_9698(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/96 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatVOS_9709(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatVOS_9709(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatVOS_97(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatVOS_97(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94_CatMix_9597(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94_CatMix_9597(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94Anz96_CatMix_9698(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94Anz96_CatMix_9698(bReqVec94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito vecchiaia al 12/94 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnzVec94Anz96_CatMix_9698(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 09/96 mancante (S/N)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatMix_9709(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatMix_9709(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatMix_97(bReqAnz94, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CatMix_97(bReqAnz96, CodeNatura, Categoria, Decorrenza))
            {
                msg = "Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza";
                return false;
            }


            if (!GestioneControlli.VerificaReqAnz94Anz96_CodNatura_9509(bReqAnz94, CodeNatura, Gruppo, prodotto, Decorrenza, isRiaperturaDomanda))
            {
                msg = "Requisito anzianità al 12/94 errato (non deve essere acquisito)";
                return false;
            }

            if (!GestioneControlli.VerificaReqAnz94Anz96_CodNatura_9509(bReqAnz96, CodeNatura, Gruppo, prodotto, Decorrenza, isRiaperturaDomanda))
            {
                msg = "Requisito anzianità al 09/96 errato (non deve essere acquisito)";
                return false;
            }

            bool bFalse1 = true;
            bool bFalse2 = true;
            if (!GestioneControlli.VerificaReqAnzVec94Anz96_Fin95(bReqAnz94, CodeNatura, Gruppo, prodotto, Categoria, Decorrenza, isAssicurativiAcquisito, out bFalse1, out bFalse2))
            {
                if (!bFalse1)
                {
                    msg = "Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza";
                    return false;
                }
                if (!bFalse2)
                {
                    msg = "Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza (S)";
                    return false;
                }
            }

            bFalse1 = true;
            bFalse2 = true;
            //ENG - Funzionamento come ex-eap (Per le condizioni seguenti il controllo non deve essere effettuato)
            if (!((Utility.DataStrettamenteSuccessivaA(Decorrenza.GetValueOrDefault(), new DateTime(1997, 12, 1)) && !Utility.DataSuccessivaA(Decorrenza.GetValueOrDefault(), new DateTime(2009, 1, 1)))
                || (Utility.DataStrettamenteSuccessivaA(Decorrenza.GetValueOrDefault(), new DateTime(1997, 12, 1)) && !String.IsNullOrEmpty(CodeNatura) && (CodeNatura.Substring(2, 1) == "U" || CodeNatura.Substring(2, 1) == "I" || CodeNatura.Substring(2, 1) == "L"))))
            {
                if (!GestioneControlli.VerificaReqAnzVec94Anz96_Fin95(bReqVec94, CodeNatura, Gruppo, prodotto, Categoria, Decorrenza, isAssicurativiAcquisito, out bFalse1, out bFalse2))
                {
                    if (!bFalse1)
                    {
                        msg = "Requisito vecchiaia al 12/94 incompatibile con Categoria o Decorrenza";
                        return false;
                    }
                    if (!bFalse2)
                    {
                        msg = "Requisito vecchiaia al 12/94 incompatibile con Categoria o Decorrenza (S)";
                        return false;
                    }
                }
            }

            bFalse1 = true;
            bFalse2 = true;
            if (!GestioneControlli.VerificaReqAnzVec94Anz96_Fin95(bReqAnz96, CodeNatura, Gruppo, prodotto, Categoria, Decorrenza, isAssicurativiAcquisito, out bFalse1, out bFalse2))
            {
                if (!bFalse1)
                {
                    msg = "Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza";
                    return false;
                }
                //if (!bFalse2)
                //{
                //    msg = "Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza (S)";
                //    return false;
                //}
            }
            return true;
        }

        #endregion dati Assicurativi

        #region dati Istruttoria
        public static void GetDatiIstruttoria(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out Entity.DatiIstruttoria datiIstruttoriaEntity)
        {
            datiIstruttoriaEntity = null;

            if (datiPensione == null)
                return;

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);

            if (datiIstruttoria == null && datiGenerici == null)
                return;

            datiIstruttoriaEntity = new Entity.DatiIstruttoria();

            Utility.ValorizzaOggetti(datiPensione, datiIstruttoriaEntity);
            Utility.ValorizzaOggetti(datiIstruttoria, datiIstruttoriaEntity);
            Utility.ValorizzaOggetti(datiGenerici, datiIstruttoriaEntity);

            if (datiIstruttoriaEntity.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoriaEntity.IsDatiIstruttoriaDatiGenericiNull())
                datiIstruttoriaEntity = null;
        }

        public static void StoreDatiIstruttoria(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            Entity.DatiIstruttoria datiIstruttoriaEntity, bool IsCancelOperation)
        {
            if (datiIstruttoriaEntity == null)
                datiIstruttoriaEntity = new Entity.DatiIstruttoria();

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            bool bloccoDeroga = false;
            if (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione)
                || Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione)
                || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione)
                || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione)
                || Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione)
                || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione))
                bloccoDeroga = true;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiIstruttoriaPerPensioni(datiIstruttoriaEntity, datiPensione);
                StoreDatiIstruttoriaPerIstruttoria(datiPensione, datiIstruttoriaEntity, ref datiIstruttoria, bloccoDeroga);
                StoreDatiIstruttoriaPerDatiGenerici(datiPensione.Id, datiPensione.FlagUnicarpe, datiPensione.TipoLetturaUnicarpe, datiIstruttoriaEntity, ref datiPensioniDatiGenerici);

                if ((datiIstruttoriaEntity.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoriaEntity.IsDatiIstruttoriaDatiGenericiNull()) || IsCancelOperation)
                {
                    if (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione)
                        || Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione)
                        || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione)
                        || Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione)
                        || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione)
                        || (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiIstruttoriaEntity != null &&
                                (datiIstruttoriaEntity.RiduzioneRetributiva ||
                                 (datiIstruttoriaEntity.Legge44997.HasValue && datiIstruttoriaEntity.Legge44997.Value != 0))
                            )
                        )
                        datiQuadroLiquidazionePensione.TabIstruttoria = 0;
                    else
                        datiQuadroLiquidazionePensione.TabIstruttoria = 1;
                }
                else
                    datiQuadroLiquidazionePensione.TabIstruttoria = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        private static void StoreDatiIstruttoriaPerPensioni(Entity.DatiIstruttoria datiIstruttoriaEntity, GestionePensione.DatiPensione datiPensione)
        {
            Utility.ValorizzaOggetti(datiIstruttoriaEntity, datiPensione);
            GestionePensione.SalvaPensione(datiPensione);
        }

        private static void StoreDatiIstruttoriaPerIstruttoria(GestionePensione.DatiPensione datiPensione, Entity.DatiIstruttoria datiIstruttoriaEntity,
            ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool bloccoDeroga)
        {
            if (datiIstruttoria == null)
            {
                if (datiIstruttoriaEntity.IsDatiIstruttoriaIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }

            // i dati provenienti da felpe sono non modificabili e non cancellabili
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiIstruttoriaEntity.Legge44997 = datiIstruttoria.Legge44997;
                datiIstruttoriaEntity.CodiceParticolareSoggettoDerogato = datiIstruttoria.CodiceParticolareSoggettoDerogato;
            }
            else if (bloccoDeroga)
                datiIstruttoriaEntity.CodiceParticolareSoggettoDerogato = datiIstruttoria.CodiceParticolareSoggettoDerogato;

            Utility.ValorizzaOggetti(datiIstruttoriaEntity, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(datiPensione.Id);
            else
                GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);
        }

        private static void StoreDatiIstruttoriaPerDatiGenerici(long idPensione, bool? flagUnicarpe, char? TipoLetturaUnicarpe, Entity.DatiIstruttoria datiIstruttoriaEntity,
            ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            if (datiPensioniDatiGenerici == null)
            {
                if (datiIstruttoriaEntity.IsDatiIstruttoriaDatiGenericiNull())
                    return;
                else
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            }
            if (Utility.IsDomandaUnicarpe(flagUnicarpe, TipoLetturaUnicarpe, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiIstruttoriaEntity.RiduzioneRetributiva = datiPensioniDatiGenerici.RiduzioneRetributiva;
                datiIstruttoriaEntity.RiduzioneRetributivaPercentuale = datiPensioniDatiGenerici.RiduzioneRetributivaPercentuale;
            }
            Utility.ValorizzaOggetti(datiIstruttoriaEntity, datiPensioniDatiGenerici);

            if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(idPensione);
            else
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiPensioniDatiGenerici);
        }

        public static bool ControlDatiIstruttoria(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, Entity.DatiIstruttoria datiIstruttoria,
            Entity.DatiGenerici datiGenerici, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (IsSingleTab)
                GetDatiGenerici(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiGenerici);

            if (datiGenerici != null && datiIstruttoria != null)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareDB = null;
                GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareDB);

                if (datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolareSoggettoDerogato = elencoCodiceParticolareDB.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                    if (datiGenerici.DecorrenzaBonus.HasValue)
                    {
                        if (datiGenerici.CausaCarico != 2 && codiceParticolareSoggettoDerogato.TraduzioneSuGp > 3)
                        {
                            messaggioVideo = "Codice Soggetto Derogato errato";
                            return false;
                        }
                    }

                    if (datiGenerici.NaturaPensione.Substring(2, 1) == "Z" && datiPensione.DataPresentazioneDomanda.CompareTo(new DateTime(2001, 08, 16)) > 0 &&
                        datiGenerici.CausaCarico == 1 && codiceParticolareSoggettoDerogato.TraduzioneSuGp != '3')
                    {
                        messaggioVideo = "3° codice Natura Pensione ('Z') incompatibile con Data Domanda";
                        return false;
                    }
                }

                if (!GestioneControlli.ControlsRequisitoRidotto(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiIstruttoria.Legge44997, datiPensione.SiglaCategoria, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCodiceContrattoEquiparato(decorrenza, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiIstruttoria.CodiceContrattoEquiparato, datiPensione.SiglaCategoria, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCodiceLivelloEquiparato(decorrenza, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiIstruttoria.CodiceLivelloEquip, datiPensione.SiglaCategoria, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCodiceMobilitaWithRequisitoRidotto(decorrenza, datiPensione.Gruppo, datiGenerici.NaturaPensione, datiGenerici.CodiceMobilita, datiPensione.SiglaCategoria, datiIstruttoria.Legge44997))
                {
                    messaggioVideo = "Codice Mobilità incompatibile con il Requisito Ridotto";
                    return false;
                }
            }

            if (datiDanteCausa != null)
            {
                if (!GestioneControlli.ControlsRequisitoRidottoWithDanteCausa(decorrenza, datiPensione.DecorrenzaOriginaria, datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, datiIstruttoria.Legge44997, datiPensione.SiglaCategoria, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.VerificaCodRiduzioneWithCodNatura(datiIstruttoria.RiduzioneRetributiva, datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodRiduzioneWithEtaTitolare(datiIstruttoria.RiduzioneRetributiva, datiAnagrafici.DataNascita, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodRiduzioneWithPercentualeRiduzione(datiIstruttoria.RiduzioneRetributiva, datiIstruttoria.RiduzioneRetributivaPercentuale, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiIstruttoria.RiduzioneRetributiva,
                datiIstruttoria.RiduzioneRetributivaPercentuale, out messaggioVideo))
                return false;

            return true;
        }

        public static void EliminaDatiIstruttoria(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            StoreDatiIstruttoria(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, new Entity.DatiIstruttoria(), true);
        }

        #endregion dati Istruttoria

        #region dati Opzione
        public static void GetDatiOpzione(long idPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out Entity.DatiOpzione datiOpzione)
        {
            datiOpzione = null;

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiCIGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(idPensione, out datiCIGenerici);

            if (datiIstruttoria == null)
                return;

            datiOpzione = new Entity.DatiOpzione();
            Utility.ValorizzaOggetti(datiIstruttoria, datiOpzione);
            Utility.ValorizzaOggetti(datiCIGenerici, datiOpzione);
            if (datiOpzione.IsDatiOpzioneIstruttoriaNull() && datiOpzione.IsDatiOpzionePensioniCiDatiGenericiNull())
                datiOpzione = null;
        }

        public static void StoreDatiOpzione(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            Entity.DatiOpzione datiOpzione, bool IsCancelOperation)
        {
            if (datiOpzione == null)
                datiOpzione = new Entity.DatiOpzione();

            DateTime dataCompare = new DateTime(1980, 01, 01);

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiOpzionePerIstruttoria(datiPensione.Id, datiOpzione, ref datiIstruttoria);
                StoreDatiOpzionePerPensioniCiDatiGenerici(datiPensione.Id, datiOpzione, ref datiPensioniDatiGenerici);

                if ((datiOpzione.IsDatiOpzioneIstruttoriaNull() && datiOpzione.IsDatiOpzionePensioniCiDatiGenericiNull()) || IsCancelOperation)
                {
                    if (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value >= dataCompare)
                    {
                        if (!(IsCancelOperation && Utility.IsRicostituzione(datiPensione.Gruppo)))
                        {

                            datiQuadroLiquidazionePensione.TabOpzione = null;
                        }
                        else
                        {
                            datiQuadroLiquidazionePensione.TabOpzione = 1;
                        }

                    }

                    else
                        datiQuadroLiquidazionePensione.TabOpzione = 1;
                }
                else
                    datiQuadroLiquidazionePensione.TabOpzione = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        private static void StoreDatiOpzionePerIstruttoria(long idPensione, Entity.DatiOpzione datiOpzione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiOpzione.IsDatiOpzioneIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }
            Utility.ValorizzaOggetti(datiOpzione, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
        }

        private static void StoreDatiOpzionePerPensioniCiDatiGenerici(long idPensione, Entity.DatiOpzione datiOpzione, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            if (datiPensioniDatiGenerici == null)
            {
                if (datiOpzione.IsDatiOpzioneIstruttoriaNull())
                    return;
                else
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            }

            Utility.ValorizzaOggetti(datiOpzione, datiPensioniDatiGenerici);

            if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(idPensione);
            else
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiPensioniDatiGenerici);
        }

        public static bool ControlDatiOpzione(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Entity.DatiOpzione datiOpzione, Entity.DatiGenerici datiGenerici, Entity.DatiAssicurativi datiAssicurativi, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiContributivi);

            List<GestioneCalcolo.DatiCalcoloRetributivo> lDatiCalcoloRetrib = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out lDatiCalcoloRetrib);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out listaResidenzeEstere);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiCi);

            List<GestioneContrib.StatoEstero> listaStatiEsteri = null;
            GestioneContrib.GetStatiEEfromDBByIdPensione(datiPensione.Id, listaPrestazioniEstere, out listaStatiEsteri);

            int codiceStatoEE = 0;
            string codiceIstituzione = string.Empty;
            string nomeStato = string.Empty;
            byte? codiceConvenzione = null;
            if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            {
                string codiceStato = listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE;
                codiceIstituzione = listaStatiEsteri[0].PrestazioneEstera.CodiceIstituzione;
                nomeStato = listaStatiEsteri[0].PrestazioneEstera.NomeStato;
                codiceConvenzione = listaStatiEsteri[0].PrestazioneEstera.CodiceConvenzione;

                int.TryParse(codiceStato, out codiceStatoEE);
            }

            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (datiPensione == null)
            {
                messaggioVideo = "Dati Pensione obbligatori.";
                return false;
            }

            if (datiDanteCausa == null)
                datiDanteCausa = new GestioneDanteCausa.DatiDanteCausa();

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            #region Controlli VV Misura Al 192

            int? nSettimane = null;
            if (ldatiContributivi != null && ldatiContributivi.Count > 0)
                nSettimane = ldatiContributivi[0].NSettimane;

            if (isSingleTab)
            {
                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiCIGenerici = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiCIGenerici);

                if (datiCIGenerici == null)
                    datiCIGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                if (!GestioneControlli.VerificaSettVVMisuraWithDecOrigWithDecOpzioneWithNContribVolWithNsett(datiCIGenerici.VVMisuraAl1292, datiPensione.DecorrenzaOriginaria, datiOpzione.DecorrenzaOpzione,
                    datiIstruttoria.NContributiVolontari, datiCIGenerici.ImportoIVS, datiCIGenerici.VVMisuraDL50392, nSettimane))
                {
                    messaggioVideo = "Settimane VV per Misura mancanti o incompatibili con VV diritto.";
                    return false;
                }

                if (!GestioneControlli.VerificaSettVVMisuraWithDecOriginariaWithDecOpzione(datiCIGenerici.VVMisuraAl1292, datiPensione.DecorrenzaOriginaria, datiOpzione.DecorrenzaOpzione))
                {
                    messaggioVideo = "Settimane VV per Misura incompatibili con Decorrenza ante 07/72.";
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneVVMisuraWithDecorrenzaOriginaria(datiCIGenerici.VVMisuraAl1292, datiOpzione.DecorrenzaOpzione))
                {
                    messaggioVideo = "Settimane VV per Misura incompatibili con Decorrenza ante 07/72.";
                    return false;
                }
            }
            else
            {
                if (!GestioneControlli.VerificaSettVVMisuraWithDecOrigWithDecOpzioneWithNContribVolWithNsett(datiAssicurativi.VVMisuraAl1292, datiPensione.DecorrenzaOriginaria, datiOpzione.DecorrenzaOpzione,
                    datiIstruttoria.NContributiVolontari, datiAssicurativi.ImportoIVS, datiAssicurativi.VVMisuraDL50392, nSettimane))
                {
                    messaggioVideo = "Settimane VV per Misura mancanti o incompatibili con VV diritto.";
                    return false;
                }

                if (!GestioneControlli.VerificaSettVVMisuraWithDecOriginariaWithDecOpzione(datiAssicurativi.VVMisuraAl1292, datiPensione.DecorrenzaOriginaria, datiOpzione.DecorrenzaOpzione))
                {
                    messaggioVideo = "Settimane VV per Misura incompatibili con Decorrenza ante 07/72.";
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneVVMisuraWithDecorrenzaOriginaria(datiAssicurativi.VVMisuraAl1292, datiOpzione.DecorrenzaOpzione))
                {
                    messaggioVideo = "Settimane VV per Misura incompatibili con Decorrenza ante 07/72.";
                    return false;
                }
            }

            #endregion Controlli VV Misura Al 192

            #region Controlli R.M.S.

            if (lDatiCalcoloRetrib != null && lDatiCalcoloRetrib.Count > 0)
                foreach (GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi in lDatiCalcoloRetrib)
                {
                    if (!GestioneControlli.VerificaRMSWithDecOriginaria(datiOpzione.DecorrenzaOpzione, datiPensione.DecorrenzaOriginaria, datiDanteCausa.Certificato, datiRetributivi.RMSQuotaA))
                    {
                        messaggioVideo = "R.M.S. errata per decorrenza ante 05/1968.";
                        return false;
                    }

                    if (isSingleTab)
                    {
                        if (datiGenerici == null)
                            datiGenerici = new DatiGenerici();

                        if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa.Certificato, datiDanteCausa.DecorrenzaPensione, datiRetributivi.RMSQuotaA,
                            datiPensione.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa.DataMorte, datiOpzione.DecorrenzaOpzione, datiGenerici.FlagContributiva,
                            datiPensione.NaturaPensione, datiPensione.Gruppo, datiPensione.Prodotto))
                        {
                            messaggioVideo = "R.M.S. mancante.";
                            return false;
                        }
                    }
                    else
                    {
                        if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa.Certificato, datiDanteCausa.DecorrenzaPensione, datiRetributivi.RMSQuotaA,
                            datiAssicurativi.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa.DataMorte, datiOpzione.DecorrenzaOpzione, datiGenerici.FlagContributiva,
                            datiGenerici.NaturaPensione, datiPensione.Gruppo, datiPensione.Prodotto))
                        {
                            messaggioVideo = "R.M.S. mancante.";
                            return false;
                        }
                    }
                }

            #endregion Controlli R.M.S.

            if (!GestioneControlli.VerificaDecorrenzaOpzione(datiIstruttoria.DecorrenzaOpzione))
            {
                messaggioVideo = "Decorrenza Opzione illogica";
                return false;
            }

            if (!GestioneControlli.ControlsDecorrenzaDPCM(datiOpzione.DecorrenzaArt2Dpcm, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.CI_VerificaDecorrenzaArt2DPCMWithDanteCausa(datiOpzione.DecorrenzaArt2Dpcm, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Decorrenza D.P.C.M. incompatibile con Decorrenza della Pensione";
                return false;
            }

            if (listaResidenzeEstere != null && listaResidenzeEstere.Count > 0)
            {
                if (!GestioneCrossControls.CI_VerificaResidenzaWithCodOpzione(datiOpzione.CodiceOpzioneRiliquidazione, listaResidenzeEstere.First().CodCatastaleStatoEE))
                {
                    messaggioVideo = "Residenza alla Decorrenza Originaria deve essere Italia se Codice Opzione è uguale a 7";
                    return false;
                }
            }

            if (!GestioneControlli.VerificaDataDomandaOpzione(datiOpzione.DataDomandaOpzione, codiceConvenzione, datiPensione.DecorrenzaOriginaria, codiceStatoEE, categoria, datiPensione.Gruppo, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaOpzioneWithDataDomandaOpzione(datiOpzione.DecorrenzaOpzione, datiOpzione.DataDomandaOpzione, codiceConvenzione, codiceStatoEE, datiOpzione.CodiceOpzioneRiliquidazione, datiPensione.DecorrenzaOriginaria, tipoDomanda, datiAnagrafici.Cittadinanza, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.CI_VerificaDataDomandaOpzioneWithDanteCausa(datiOpzione.DataDomandaOpzione, decorrenza, datiDanteCausa != null ? datiDanteCausa.SiglaCategoria : string.Empty, out messaggioVideo))
                return false;

            if (isSingleTab)
            {
                if (!GestioneControlli.ControlsCodiceOpzioneRiliquidazione(datiOpzione.CodiceOpzioneRiliquidazione, datiAnagrafici.Cittadinanza, listaResidenzeEstere,
                    listaPrestazioniEstere, datiPensione.Gruppo, datiPensione.NaturaPensione, datiIstruttoria.Legge44997, datiPensione.DecorrenzaOriginaria,
                    datiAnagrafici.DataNascita, datiAnagrafici.Sesso, datiPensione.SiglaCategoria, out messaggioVideo))
                    return false;

                bool isRocOrRevCI = (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || Utility.IsRicostituzione(datiPensione.Gruppo)) && (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione)) == Utility.TipoAppartenenza.CI;
                if (!GestioneControlli.ControlsDecorrenzaOpzione(datiOpzione.DecorrenzaOpzione, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria,
                    datiOpzione.DataDomandaOpzione, listaPrestazioniEstere[0].CodiceConvenzione, listaPrestazioniEstere[0].CodiceStatoEE, datiPensione.SiglaCategoria,
                    datiPensione.NaturaPensione, isRocOrRevCI, out messaggioVideo))
                    return false;

                if (datiDanteCausa != null)
                {
                    if (!GestioneControlli.ControlsDecorrenzaOpzioneWithDanteCausa(decorrenza, datiOpzione.DecorrenzaOpzione, datiDanteCausa.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria, datiOpzione.DataDomandaOpzione, datiDanteCausa.SiglaCategoria, datiPensione.SiglaCategoria, datiPensione.NaturaPensione, listaPrestazioniEstere[0].CodiceConvenzione, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.VerificaFineAssicurazioneWithDataDomandaOpzione(datiPensione.FineAssicurazione, decorrenza, datiOpzione.DataDomandaOpzione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS8888WithOpzione(datiGenericiCi != null ? datiGenericiCi.RMS8888 : null, decorrenza, datiOpzione.DecorrenzaOpzione, datiOpzione.DataDomandaOpzione, out messaggioVideo))
                    return false;
            }
            else
            {
                if (!GestioneControlli.ControlsCodiceOpzioneRiliquidazione(datiOpzione.CodiceOpzioneRiliquidazione, datiAnagrafici.Cittadinanza,
                    listaResidenzeEstere, listaPrestazioniEstere, datiPensione.Gruppo, datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty,
                    datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiPensione.DecorrenzaOriginaria, datiAnagrafici.DataNascita, datiAnagrafici.Sesso,
                    datiPensione.SiglaCategoria, out messaggioVideo))
                    return false;

                bool isRocOrRevCI = (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || Utility.IsRicostituzione(datiPensione.Gruppo)) && (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione)) == Utility.TipoAppartenenza.CI;
                if (!GestioneControlli.ControlsDecorrenzaOpzione(datiOpzione.DecorrenzaOpzione, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria,
                    datiOpzione.DataDomandaOpzione, listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 ? listaPrestazioniEstere[0].CodiceConvenzione : 0, listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 ? listaPrestazioniEstere[0].CodiceStatoEE : string.Empty, datiPensione.SiglaCategoria,
                    datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, isRocOrRevCI, out messaggioVideo))
                    return false;

                if (datiDanteCausa != null)
                {
                    if (!GestioneControlli.ControlsDecorrenzaOpzioneWithDanteCausa(decorrenza, datiOpzione.DecorrenzaOpzione, datiDanteCausa.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria, datiOpzione.DataDomandaOpzione, datiDanteCausa.SiglaCategoria, datiPensione.SiglaCategoria, datiPensione.NaturaPensione, listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 ? listaPrestazioniEstere[0].CodiceConvenzione : 0, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.VerificaFineAssicurazioneWithDataDomandaOpzione(datiAssicurativi != null ? datiAssicurativi.FineAssicurazione : null, decorrenza, datiOpzione.DataDomandaOpzione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS8888WithOpzione(datiAssicurativi != null ? datiAssicurativi.RMS8888 : null, decorrenza, datiOpzione.DecorrenzaOpzione, datiOpzione.DataDomandaOpzione, out messaggioVideo))
                    return false;
            }

            return true;
        }

        public static void EliminaDatiOpzione(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            StoreDatiOpzione(datiPensione, ref datiIstruttoriaCommon, ref datiPensioniDatiGenerici, new Entity.DatiOpzione(), true);
        }

        #endregion dati Opzione

        #region dati Provenienza
        public static void GetDatiProvenienza(long idPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out Entity.DatiProvenienza datiProvenienza)
        {
            datiProvenienza = null;

            if (datiIstruttoria == null)
                return;

            datiProvenienza = new Entity.DatiProvenienza();
            Utility.ValorizzaOggetti(datiIstruttoria, datiProvenienza);
            if (datiProvenienza.IsDatiProvenienzaIstruttoriaNull())
                datiProvenienza = null;
        }

        public static void StoreDatiProvenienza(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Entity.DatiProvenienza datiProvenienza, bool IsCancelOperation)
        {
            if (datiPensione == null)
                return;

            if (datiProvenienza == null)
                datiProvenienza = new Entity.DatiProvenienza();

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiProvenienzaPerIstruttoria(datiPensione.Id, datiProvenienza, ref datiIstruttoria);

                if (datiProvenienza.IsDatiProvenienzaIstruttoriaNull() || IsCancelOperation)
                {
                    if ((datiPensione.TrasformazioneAOI.HasValue &&
                        datiPensione.TrasformazioneAOI.Value) || (datiPensione.CausaCarico == 3 || datiPensione.CausaCarico == 9))
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 0;
                    else
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 1;
                }
                else
                    datiQuadroLiquidazionePensione.TabPrecedentePensione = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        private static void StoreDatiProvenienzaPerIstruttoria(long idPensione, Entity.DatiProvenienza datiProvenienza, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiProvenienza.IsDatiProvenienzaIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }
            Utility.ValorizzaOggetti(datiProvenienza, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
        }

        public static bool ControlDatiProvenienza(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, Entity.DatiGenerici datiGenerici, Entity.DatiProvenienza datiProvenienza, Entity.DatiAssicurativi datiAssicurativi, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiPensione != null)
            {
                if (datiPensione.TrasformazioneAOI.HasValue && datiPensione.TrasformazioneAOI.Value)
                {
                    if (!datiProvenienza.SedePrecedentePensione.HasValue)
                    {
                        messaggioVideo = "Il campo 'Sede' è obbligatorio";
                        return false;
                    }

                    if (!Utility.ExistSedeProvinciale(datiProvenienza.SedePrecedentePensione.Value))
                    {
                        messaggioVideo = "La 'Sede' inserita non esiste";
                        return false;
                    }

                    if (!datiProvenienza.CodiceP18PrecedentePensione.HasValue)
                    {
                        messaggioVideo = "Il campo 'Categoria' è obbligatorio";
                        return false;
                    }

                    if (!datiProvenienza.CertificatoPrecedentePensione.HasValue)
                    {
                        messaggioVideo = "Il campo 'Certificato' è obbligatorio";
                        return false;
                    }

                    if (!ControlsCrossDatiProvenienza(datiPensione, datiIstruttoriaCommon, datiGenerici, datiProvenienza, datiAssicurativi, IsSingleTab, isRiaperturaDomanda, out messaggioVideo))
                        return false;
                }
                if (!Utility.IsDomandaRiliquidazione(datiPensione).GetValueOrDefault() && !Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault() &&
                    !datiProvenienza.IsDatiProvenienzaIstruttoriaNull() && ((!datiPensione.TrasformazioneAOI.HasValue || !datiPensione.TrasformazioneAOI.Value) && (datiPensione.CausaCarico != 3 && datiPensione.CausaCarico != 9)))
                {
                    messaggioVideo = "Salvare i dati Generici prima di procedere con il salvataggio dei dati della Pensione di Provenienza";
                    return false;
                }

                //if (IsSingleTab)
                //{
                //    if (!datiProvenienza.SedePrecedentePensione.HasValue)
                //    {
                //        messaggioVideo = "Il campo 'Sede' è obbligatorio";
                //        return false;
                //    }

                //    if (!Utility.ExistSedeProvinciale(datiProvenienza.SedePrecedentePensione.Value))
                //    {
                //        messaggioVideo = "La 'Sede' inserita non esiste";
                //        return false;
                //    }

                //    if (!datiProvenienza.CodiceP18PrecedentePensione.HasValue)
                //    {
                //        messaggioVideo = "Il campo 'Categoria' è obbligatorio";
                //        return false;
                //    }

                //    if (!datiProvenienza.CertificatoPrecedentePensione.HasValue)
                //    {
                //        messaggioVideo = "Il campo 'Certificato' è obbligatorio";
                //        return false;
                //    }
                //}
                //else
                //{
                //    if (datiPensione.TrasformazioneAOI.HasValue && datiPensione.TrasformazioneAOI.Value)
                //    {
                //        if (!datiProvenienza.SedePrecedentePensione.HasValue)
                //        {
                //            messaggioVideo = "Il campo 'Sede' è obbligatorio";
                //            return false;
                //        }

                //        if (!Utility.ExistSedeProvinciale(datiProvenienza.SedePrecedentePensione.Value))
                //        {
                //            messaggioVideo = "La 'Sede' inserita non esiste";
                //            return false;
                //        }

                //        if (!datiProvenienza.CodiceP18PrecedentePensione.HasValue)
                //        {
                //            messaggioVideo = "Il campo 'Categoria' è obbligatorio";
                //            return false;
                //        }

                //        if (!datiProvenienza.CertificatoPrecedentePensione.HasValue)
                //        {
                //            messaggioVideo = "Il campo 'Certificato' è obbligatorio";
                //            return false;
                //        }
                //    }
                //    if (!datiProvenienza.IsDatiProvenienzaIstruttoriaNull() && (!datiPensione.TrasformazioneAOI.HasValue || !datiPensione.TrasformazioneAOI.Value))
                //    {
                //        messaggioVideo = "Salvare i dati Generici prima di procedere con il salvataggio dei dati della Pensione di Provenienza";
                //        return false;
                //    }
                //}
            }
            else
            {
                messaggioVideo = "Dati obbligatori non presenti nel DB.";
                return false;
            }
            return true;
        }

        public static bool ControlsCrossDatiProvenienza(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, Entity.DatiGenerici datiGenerici, Entity.DatiProvenienza datiProvenienza, Entity.DatiAssicurativi datiAssicurativi, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region GetData

            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);

            if (IsSingleTab)
            {
                GetDatiGenerici(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiGenerici);
                GetDatiAssicurativi(datiPensione, datiIstruttoriaCommon, isRiaperturaDomanda, out datiAssicurativi);
            }

            #endregion GetData

            if (!GestioneControlli.VerificaDatiPrecedentePensione(datiGenerici != null ? datiGenerici.CausaCarico : null, datiAssicurativi != null ? datiAssicurativi.CodiceRequisitiParticolari : null, datiGenerici != null ? datiGenerici.NaturaPensione : null, datiProvenienza.CodiceP18PrecedentePensione, datiProvenienza.CertificatoPrecedentePensione, datiProvenienza.SedePrecedentePensione,
                categoria, datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : null, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : null, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiProvenienza.DecorrenzaOriginariaAltraPensione, datiGenerici != null ? datiGenerici.TrasformazioneAOI : null, out messaggioVideo))
                return false;

            return true;
        }

        public static void EliminaDatiProvenienza(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon)
        {
            StoreDatiProvenienza(datiPensione, ref datiIstruttoriaCommon, new Entity.DatiProvenienza(), true);
        }
        #endregion dati Provenienza

        #region Dati Inail
        public static void StoreDatiInail(GestionePensione.DatiPensione datiPensione, List<Entity.DatiInail> listDatiInail)
        {
            if (listDatiInail == null || listDatiInail.Count() == 0)
                return;

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            List<GestionePensioneInailInabilita.DatiPensioniINAIL> LDatiPensioniINAIL = null;

            if (listDatiInail != null && listDatiInail.Count > 0)
            {
                LDatiPensioniINAIL = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();
                foreach (Entity.DatiInail temp in listDatiInail)
                {
                    GestionePensioneInailInabilita.DatiPensioniINAIL pensioniInail = new GestionePensioneInailInabilita.DatiPensioniINAIL();
                    temp.IdPensione = datiPensione.Id;
                    Utility.ValorizzaOggetti(temp, pensioniInail);
                    LDatiPensioniINAIL.Add(pensioniInail);
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensioneInailInabilita.EliminaPensioniINAILByIdPensione(datiPensione.Id);
                if (LDatiPensioniINAIL != null && LDatiPensioniINAIL.Count > 0)
                {
                    foreach (GestionePensioneInailInabilita.DatiPensioniINAIL datiPensioniINAIL in LDatiPensioniINAIL)
                        GestionePensioneInailInabilita.SalvaPensioniINAIL(datiPensioniINAIL);
                }

                datiQuadroLiquidazionePensione.TabInail = 2;
                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiInail(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazionePensione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensioneInailInabilita.EliminaPensioniINAILByIdPensione(datiPensione.Id);
                datiQuadroLiquidazionePensione.TabInail = 1;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }
        #endregion DatiInail

        #region decodifica

        public static void GetListaCodiciMobilita(out List<Mobilita> listaCodiciMobilita)
        {
            listaCodiciMobilita = new List<Mobilita>();
            List<GestioneDecodifica.Mobilita> elencoCodiceMobilitaDB = null;
            GestioneDecodifica.GetCodiceMobilita(out elencoCodiceMobilitaDB);
            if (elencoCodiceMobilitaDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.Mobilita MobilitaDB in elencoCodiceMobilitaDB)
                {
                    Mobilita mobilita = new Mobilita();
                    mobilita.Id = MobilitaDB.Id;
                    mobilita.Descrizione = MobilitaDB.Descrizione;
                    listaCodiciMobilita.Add(mobilita);
                }
            }
        }

        public static void GetListaCodiceParticolare(GestionePensione.DatiPensione datiPensione, out List<CodiceParticolare> listaCodiceParticolare)
        {
            listaCodiceParticolare = new List<CodiceParticolare>();
            List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareDB = null;
            GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareDB);

            if (elencoCodiceParticolareDB != null)
            {
                foreach (GestioneDecodifica.CodiceParticolare CodiceParticolareDB in elencoCodiceParticolareDB)
                {
                    CodiceParticolare codiceParticolare = new CodiceParticolare();
                    Utility.ValorizzaOggetti(CodiceParticolareDB, codiceParticolare);
                    listaCodiceParticolare.Add(codiceParticolare);
                }
            }

            string catNum = datiPensione.GetCodCategoria();
            if (listaCodiceParticolare.Count() > 0)
                listaCodiceParticolare = listaCodiceParticolare.FindAll(x => x.CodCategoria == catNum);
            if (listaCodiceParticolare.Count > 0)
            {
                //nel caso di usurante o salvaguardia 122 essendo il valore uguale e pari a 3, altero la descrizione
                //al fine di mostrare a video il corretto messaggio
                if (Utility.IsDomandaUsuranti(datiPensione))
                {
                    foreach (CodiceParticolare cP in listaCodiceParticolare)
                    {
                        if (cP.TraduzioneSuGp.HasValue && cP.TraduzioneSuGp.Value == 3 &&
                            !string.IsNullOrEmpty(cP.Descrizione) && cP.Descrizione.Contains('|'))
                            cP.Descrizione = cP.Descrizione.Substring(0, cP.Descrizione.IndexOf('|') - 1).Trim();
                    }
                }
                else if (Utility.IsDomandaSalvaguardia122(datiPensione))
                {
                    foreach (CodiceParticolare cP in listaCodiceParticolare)
                    {
                        if (cP.TraduzioneSuGp.HasValue && cP.TraduzioneSuGp.Value == 3 &&
                            !string.IsNullOrEmpty(cP.Descrizione) && cP.Descrizione.Contains('|'))
                            cP.Descrizione = cP.Descrizione.Substring(cP.Descrizione.IndexOf('|') + 1).Trim();
                    }
                }
            }
        }

        public static void GetListaCodiceLegge44997(GestionePensione.DatiPensione datiPensione, out List<DecodificaLegge44997> listaCodiceLegge44997)
        {
            listaCodiceLegge44997 = new List<DecodificaLegge44997>();
            List<GestioneDecodifica.DecodificaLegge44997> elencoCodiceLegge44997DB = null;
            GestioneDecodifica.GetElencoDecodificaLegge44997(out elencoCodiceLegge44997DB);
            if (elencoCodiceLegge44997DB != null)
            {
                bool isContrPuro = false;
                if (Utility.IsDomandaTipoContributivo(datiPensione, null, false) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione)) //ENG - Memo 166/2023
                    isContrPuro = true;
                foreach (GestioneDecodifica.DecodificaLegge44997 CodiceLegge44997DB in elencoCodiceLegge44997DB)
                {
                    //bypass INVALIDO ALL’80% per contributive pure
                    if (isContrPuro && CodiceLegge44997DB.Id == 6)
                        continue;
                    DecodificaLegge44997 codeLegge44997 = new DecodificaLegge44997();
                    codeLegge44997.Id = CodiceLegge44997DB.Id;
                    codeLegge44997.Descrizione = CodiceLegge44997DB.Descrizione;
                    listaCodiceLegge44997.Add(codeLegge44997);
                }
            }
        }

        public static void GetListaCodicNatura(GestionePensione.DatiPensione datiPensione, out List<CodiciNatura> elencoCodiciNatura_CI)
        {
            elencoCodiciNatura_CI = null;
            List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_CI = null;
            GestioneDecodifica.GetCodiciNatura_AGO_CI(out elencoCodiciNaturaCommon_CI);

            if (elencoCodiciNaturaCommon_CI != null)
            {
                GetCodiciNaturaCustom(datiPensione, ref elencoCodiciNaturaCommon_CI);
                elencoCodiciNatura_CI = new List<CodiciNatura>();
                foreach (GestioneDecodifica.CodiciNatura CodiciNaturaCommon_CI in elencoCodiciNaturaCommon_CI)
                {
                    CodiciNatura codeNatura = new CodiciNatura();
                    codeNatura.Fondo = CodiciNaturaCommon_CI.Fondo;
                    codeNatura.Descrizione = CodiciNaturaCommon_CI.Descrizione;
                    codeNatura.Posizione = CodiciNaturaCommon_CI.Posizione;
                    codeNatura.Tipologia = CodiciNaturaCommon_CI.Tipologia;
                    codeNatura.TraduzioneSuGP = CodiciNaturaCommon_CI.TraduzioneSuGP;
                    elencoCodiciNatura_CI.Add(codeNatura);
                }
            }
        }

        private static void GetCodiciNaturaCustom(GestionePensione.DatiPensione datiPensione, ref List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_CI)
        {
            if (datiPensione != null)
            {
                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                if (elencoCodiciNaturaCommon_CI != null && elencoCodiciNaturaCommon_CI.Count > 0)
                {
                    List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_CIApp = elencoCodiciNaturaCommon_CI.ToList();
                    foreach (GestioneDecodifica.CodiciNatura codiceNatura in elencoCodiciNaturaCommon_CIApp)
                    {
                        if (codiceNatura.Posizione.GetValueOrDefault() == 2)
                        {
                            switch (codiceNatura.TraduzioneSuGP)
                            {
                                case '1':
                                case 'S':
                                    elencoCodiciNaturaCommon_CI.Remove(codiceNatura);
                                    break;
                                case 'F':
                                    if (Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                                        (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                                        (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                                        elencoCodiciNaturaCommon_CI.Remove(codiceNatura);
                                    break;
                                case 'J':
                                    if (!(Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                                        (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                                        (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                                        elencoCodiciNaturaCommon_CI.Remove(codiceNatura);
                                    break;
                                case 'O':
                                    if (!Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) && !Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione) &&
                                        !Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) && !Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) &&
                                        !Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
                                        elencoCodiciNaturaCommon_CI.Remove(codiceNatura);
                                    break;
                            }
                        }
                    }
                }

                if (elencoCodiciNaturaCommon_CI != null)
                {
                    if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0009") || (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0009") || (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022" && datiPensione.Tipo == "0009")) // pensione vecchiaia supplementare
                        elencoCodiciNaturaCommon_CI = elencoCodiciNaturaCommon_CI.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '5')))));
                    else if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")  // pensione vecchiaia
                        elencoCodiciNaturaCommon_CI = elencoCodiciNaturaCommon_CI.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '6')))));
                    else if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001") // pensione anzianita
                        elencoCodiciNaturaCommon_CI = elencoCodiciNaturaCommon_CI.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2')))));
                    else if (datiPensione.Gruppo == "0003")
                        elencoCodiciNaturaCommon_CI = elencoCodiciNaturaCommon_CI.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2' || x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '6')))));
                    else if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0001" &&
                            (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOS" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IRS" ||
                            datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOCOMS" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOARTS"))
                        elencoCodiciNaturaCommon_CI = elencoCodiciNaturaCommon_CI.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4')))));
                }
            }
        }

        public static void GetListaCodiceModalitaLiquidazione(out List<DecModalitaLiquidazione> listaCodiceModalitaLiquidazione)
        {
            listaCodiceModalitaLiquidazione = new List<DecModalitaLiquidazione>();
            List<GestioneDecodifica.DecModalitaLiquidazione> elencoModalitaLiquidazioneDB = null;
            GestioneDecodifica.GetElencoDecModalitaLiquidazione(out elencoModalitaLiquidazioneDB);
            if (elencoModalitaLiquidazioneDB != null)
            {
                foreach (GestioneDecodifica.DecModalitaLiquidazione CodiceModalitaLiquidazioneDB in elencoModalitaLiquidazioneDB)
                {
                    DecModalitaLiquidazione modalitaLiquidazione = new DecModalitaLiquidazione();
                    Utility.ValorizzaOggetti(CodiceModalitaLiquidazioneDB, modalitaLiquidazione);
                    listaCodiceModalitaLiquidazione.Add(modalitaLiquidazione);
                }
            }
        }

        public static void GetListaOpzioneRiliquidazione(out List<OpzioneRiliquidazione> listaOpzioneRiliquidazione)
        {
            listaOpzioneRiliquidazione = new List<OpzioneRiliquidazione>();
            List<GestioneDecodifica.DecOpzioneRiliquidazione> elencoOpzioneRiliquidazioneDB = null;
            GestioneDecodifica.GetElencoDecOpzioneRiliquidazione(out elencoOpzioneRiliquidazioneDB);
            if (elencoOpzioneRiliquidazioneDB != null)
            {
                foreach (GestioneDecodifica.DecOpzioneRiliquidazione OpzioneRiliquidazioneDB in elencoOpzioneRiliquidazioneDB)
                {
                    OpzioneRiliquidazione opzioneRiliquidazione = new OpzioneRiliquidazione();
                    Utility.ValorizzaOggetti(OpzioneRiliquidazioneDB, opzioneRiliquidazione);
                    listaOpzioneRiliquidazione.Add(opzioneRiliquidazione);
                }
            }
        }

        public static void GetListaCodiceVirtuale(out List<CodiceVirtuale> listaCodiceVirtuale)
        {
            listaCodiceVirtuale = new List<CodiceVirtuale>();
            List<GestioneDecodifica.CodiceVirtuale> elencoCodiceVirtualeDB = null;
            GestioneDecodifica.GetCodiceVirtuale(out elencoCodiceVirtualeDB);
            if (elencoCodiceVirtualeDB != null)
            {
                foreach (GestioneDecodifica.CodiceVirtuale codiceVirtualeDB in elencoCodiceVirtualeDB)
                {
                    CodiceVirtuale codiceVirtuale = new CodiceVirtuale();
                    Utility.ValorizzaOggetti(codiceVirtualeDB, codiceVirtuale);
                    listaCodiceVirtuale.Add(codiceVirtuale);
                }
            }
        }

        public static void GetListaCodiceCi21(out List<CodiceCi21> listaCodiceCi21)
        {
            listaCodiceCi21 = new List<CodiceCi21>();
            List<GestioneDecodifica.DecCodiceCi21> elencoCodiceCi21DB = null;
            GestioneDecodifica.GetElencoDecCodiceCi21(out elencoCodiceCi21DB);
            if (elencoCodiceCi21DB != null)
            {
                foreach (GestioneDecodifica.DecCodiceCi21 codiceCi21DB in elencoCodiceCi21DB)
                {
                    CodiceCi21 codiceCi21 = new CodiceCi21();
                    Utility.ValorizzaOggetti(codiceCi21DB, codiceCi21);
                    listaCodiceCi21.Add(codiceCi21);
                }
            }
        }

        public static void GetListaCodiceCi28(out List<CodiceCi28> listaCodiceCi28)
        {
            listaCodiceCi28 = new List<CodiceCi28>();
            List<GestioneDecodifica.DecCodiceCi28> elencoCodiceCi28DB = null;
            GestioneDecodifica.GetElencoDecCodiceCi28(out elencoCodiceCi28DB);
            if (elencoCodiceCi28DB != null)
            {
                foreach (GestioneDecodifica.DecCodiceCi28 codiceCi28DB in elencoCodiceCi28DB)
                {
                    CodiceCi28 codiceCi28 = new CodiceCi28();
                    Utility.ValorizzaOggetti(codiceCi28DB, codiceCi28);
                    listaCodiceCi28.Add(codiceCi28);
                }
            }
        }

        public static void GetListaCodiciDomandaRicorso(out List<DomandaRicorso> listaCodiciDomandaRicorso)
        {
            listaCodiciDomandaRicorso = new List<DomandaRicorso>();
            List<GestioneDecodifica.DomandaRicorso> listaCodiciDomandaRicorsoDB = null;
            GestioneDecodifica.GetElencoDomandaRicorso(out listaCodiciDomandaRicorsoDB);
            if (listaCodiciDomandaRicorsoDB != null)
            {
                foreach (GestioneDecodifica.DomandaRicorso domandaRicorsoDB in listaCodiciDomandaRicorsoDB)
                {
                    DomandaRicorso domandaRicorso = new DomandaRicorso();
                    Utility.ValorizzaOggetti(domandaRicorsoDB, domandaRicorso);
                    listaCodiciDomandaRicorso.Add(domandaRicorso);
                }
            }
        }

        public static void GetListaRiconoscimentiInvalidita(out List<DecodificaRiconoscimentiInvalidita> listaDecodificaRiconoscimentiInvalidita)
        {
            listaDecodificaRiconoscimentiInvalidita = new List<DecodificaRiconoscimentiInvalidita>();
            List<GestioneDecodifica.DecRiconoscimentiInvalidita> elencoRiconoscimentiInvaliditaDB = null;
            GestioneDecodifica.GetElencoRiconoscimentiInvalidita(out elencoRiconoscimentiInvaliditaDB);
            if (elencoRiconoscimentiInvaliditaDB != null)
            {
                foreach (GestioneDecodifica.DecRiconoscimentiInvalidita RiconoscimentiInvaliditaDB in elencoRiconoscimentiInvaliditaDB)
                {
                    DecodificaRiconoscimentiInvalidita riconoscimentiInvalidita = new DecodificaRiconoscimentiInvalidita();
                    Utility.ValorizzaOggetti(RiconoscimentiInvaliditaDB, riconoscimentiInvalidita);
                    listaDecodificaRiconoscimentiInvalidita.Add(riconoscimentiInvalidita);
                }
            }
        }

        public static void GetListaCodiceRequisitiParticolari(out List<CodiceRequisitiParticolari> listaCodiceRequisitiParticolari)
        {
            listaCodiceRequisitiParticolari = new List<CodiceRequisitiParticolari>();
            List<GestioneDecodifica.CodiceRequisitoParticolare> elencoCodiceRequisitoParticolareDB = null;
            GestioneDecodifica.GetCodiciRequisitiParticolari(out elencoCodiceRequisitoParticolareDB);
            if (elencoCodiceRequisitoParticolareDB != null)
            {
                foreach (GestioneDecodifica.CodiceRequisitoParticolare codiceRequisitoParticolareDB in elencoCodiceRequisitoParticolareDB)
                {
                    CodiceRequisitiParticolari codiceRequisitiParticolari = new CodiceRequisitiParticolari();
                    Utility.ValorizzaOggetti(codiceRequisitoParticolareDB, codiceRequisitiParticolari);
                    listaCodiceRequisitiParticolari.Add(codiceRequisitiParticolari);
                }
            }
        }

        #endregion decodifica

        #region Cross Properties

        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni,
            bool isRiaperturaDomanda, GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP, out TipoSalvaguardia? TipologiaSalvaguardia, out DateTime? dataPrelievoDomanda)
        {
            bool? IsEsenzioneFiscaleEstero = null;
            bool? IsEsenzioneFiscaleVittima = null;
            bool? isRiduzioneRetributiva = null;
            TipologiaSalvaguardia = null;
            bool? IsUsuranti = null;
            bool? isGestioneNormale = null;
            bool? isVecchiaiaInvaliditaSupplementare = null;
            bool? isImportoIVSVisible = null;
            bool? isRipristino = null;
            bool? isRiduzioneRetributivaEnabled = null;
            bool? isTrasformazioneInvalidita = null;
            bool? isBeneficioArt24Comma15BisFromFELPE = null;
            bool? isPensioneTipoContributivo = null;
            bool? isPensioneTipoContributivoConOpzione = null;
            bool? isSperimentaleDonna = null;
            bool? isBeneficioApePrecociFromFELPE = null;
            bool? isPensioneVecchiaiaOrRicostituzione = null;
            bool? isPensioneAnzianitaOrRicostituzione = null;
            bool? isEsenzioneFiscaleEsteroFromDetrazioni = null;
            bool? isRichiestaBonusBookingAbilitata = null;
            bool? isBeneficioNonVedente = null;
            bool? isDataRinunciaTrattenutaInpdapStorico = null;
            bool? isBeneficioNonVedenteFromStorico = null;
            bool? isRichiestaBonus154Abilitata = null;
            bool? isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = null;
            //ENG - Aggiornamento Memo86
            bool? isPresenteTrattenutaFondoCreditoDaPrelievo = null;
            bool? isPensioneTipoContributivoAnzianitàVecchiaia = null;
            bool? isAnte96 = null;
            dataPrelievoDomanda = null;

            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
            List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiContributivi);
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiRetributivi);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficaTitolare);

            IsEsenzioneFiscaleEstero = Utility.IsEsenzioneFiscaleEstero(datiPensione, datiAnagrafici.CodiceComuneResidenza, datiDetrazioni, isRiaperturaDomanda);                 // generici
            IsEsenzioneFiscaleVittima = Utility.IsEsenzioneFiscaleVittima(datiPensione, null, datiDetrazioni, isRiaperturaDomanda);
            isRiduzioneRetributiva = GestioneRiduzioneRetributiva(datiPensione);    // istruttoria
            IsUsuranti = Utility.IsDomandaUsuranti(datiPensione);
            TipologiaSalvaguardia = GetTipoSalvaguardia(datiPensione);              // generici
            isGestioneNormale = IsGestioneNormale(datiPensione);                    // assicurativi
            isVecchiaiaInvaliditaSupplementare = Utility.IsVecchiaiaInvaliditaSupplementare(datiPensione);
            isImportoIVSVisible = IsImportoIVSVisible(datiPensione, datiDanteCausa);
            isRipristino = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ripristino || Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.RipristinoSuperstiti;
            isRiduzioneRetributivaEnabled = Utility.GestioneRiduzioneRetributivaEnabled(datiPensione, isRiaperturaDomanda, ldatiContributivi, ldatiRetributivi);
            isTrasformazioneInvalidita = Utility.IsDomandaTrasformazioneInvalidita(datiPensione);
            isBeneficioArt24Comma15BisFromFELPE = datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.IsBeneficioArt24Comma15BisFromFELPE : null;
            isPensioneTipoContributivo = Utility.IsDomandaTipoContributivo(datiPensione, null, null);
            isPensioneTipoContributivoConOpzione = Utility.IsDomandaTipoContributivo(datiPensione, null, true);
            isSperimentaleDonna = Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione);
            isBeneficioApePrecociFromFELPE = datiMaggiorazioniBeneficiCommon != null ? datiMaggiorazioniBeneficiCommon.IsBeneficioApePrecociFromFELPE : null;
            isPensioneVecchiaiaOrRicostituzione = Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensione, null);
            isPensioneAnzianitaOrRicostituzione = Utility.IsPensioneAnzianitaOrRicostituzione(datiPensione, null);
            isEsenzioneFiscaleEsteroFromDetrazioni = Utility.IsEsenzioneFiscaleEsteroFromDetrazioni(datiPensione, datiDetrazioni, isRiaperturaDomanda);
            isPensioneTipoContributivoAnzianitàVecchiaia = Utility.IsDomandaTipoContributivo(datiPensione, null, false);
            isAnte96 = Utility.IsDomandaAnte96Generica(datiPensione, datiDanteCausa, isRiaperturaDomanda);

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            if (Utility.IsBonusBooking(datiPensione))
            {
                GestioneControlliDinamici.ControlloDinamico sediDaControllare = null;

                if (datiPensione.Tipo == "0167") //BONUS 14°
                {
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingCI", out ctrl);
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingSediCI", out sediDaControllare);

                    if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                        (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                         sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == Utility.GetCodiceSedeLavorazione(datiPensione, isRiaperturaDomanda).ToString().PadLeft(4, '0')))))
                    {
                        isRichiestaBonusBookingAbilitata = true;
                    }
                }
                else //BONUS 154
                {
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154CI", out ctrl);
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154SediCI", out sediDaControllare);

                    if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                        (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                         sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == Utility.GetCodiceSedeLavorazione(datiPensione, isRiaperturaDomanda).ToString().PadLeft(4, '0')))))
                    {
                        isRichiestaBonus154Abilitata = true;
                    }
                }
            }

            if (datiMaggiorazioniBeneficiCommon != null && datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01")
                isBeneficioNonVedente = true;

            if (datiStoricoGP != null && datiStoricoGP.DataRinunciaTrattenutaInpdap.HasValue)
                isDataRinunciaTrattenutaInpdapStorico = true;

            if (datiStoricoGP != null && !string.IsNullOrEmpty(datiStoricoGP.TipoSettimaneBeneficio) && datiStoricoGP.TipoSettimaneBeneficio == "01")
                isBeneficioNonVedenteFromStorico = true;

            isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) ||
                Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione);

            //ENG - Aggiornamento Memo86
            if (datiStoricoGP != null && datiStoricoGP.TrattenutaFondoCredito.HasValue)
                isPresenteTrattenutaFondoCreditoDaPrelievo = datiStoricoGP.TrattenutaFondoCredito.Value;
            else
                isPresenteTrattenutaFondoCreditoDaPrelievo = null;

            GestioneLogSoap.GetTimestampMinimo(datiPensione.NDomus, out dataPrelievoDomanda);

            lReturn.Add("IsEsenzioneFiscaleEstero", IsEsenzioneFiscaleEstero);
            lReturn.Add("IsEsenzioneFiscaleVittima", IsEsenzioneFiscaleVittima);
            lReturn.Add("IsRiduzioneRetributiva", isRiduzioneRetributiva);
            lReturn.Add("Usuranti", IsUsuranti);
            lReturn.Add("IsGestioneNormale", isGestioneNormale);
            lReturn.Add("IsVecchiaiaInvaliditaSupplementare", isVecchiaiaInvaliditaSupplementare);
            lReturn.Add("IsImportoIVSVisible", isImportoIVSVisible);
            lReturn.Add("IsRipristino", isRipristino);
            lReturn.Add("IsRiduzioneRetributivaEnabled", isRiduzioneRetributivaEnabled);
            lReturn.Add("IsTrasformazioneInvalidita", isTrasformazioneInvalidita);
            lReturn.Add("IsBeneficioArt24Comma15BisFromFELPE", isBeneficioArt24Comma15BisFromFELPE);
            lReturn.Add("IsPensioneTipoContributivo", isPensioneTipoContributivo);
            lReturn.Add("IsPensioneTipoContributivoConOpzione", isPensioneTipoContributivoConOpzione);
            lReturn.Add("IsSperimentaleDonna", isSperimentaleDonna);
            lReturn.Add("IsBeneficioApePrecociFromFELPE", isBeneficioApePrecociFromFELPE);
            lReturn.Add("IsPensioneVecchiaiaOrRicostituzione", isPensioneVecchiaiaOrRicostituzione);
            lReturn.Add("IsPensioneAnzianitaOrRicostituzione", isPensioneAnzianitaOrRicostituzione);
            lReturn.Add("IsEsenzioneFiscaleEsteroFromDetrazioni", isEsenzioneFiscaleEsteroFromDetrazioni);
            lReturn.Add("IsRichiestaBonusBookingAbilitata", isRichiestaBonusBookingAbilitata);
            lReturn.Add("IsBeneficioNonVedente", isBeneficioNonVedente);
            lReturn.Add("IsDataRinunciaTrattenutaInpdapStorico", isDataRinunciaTrattenutaInpdapStorico);
            lReturn.Add("IsBeneficioNonVedenteFromStorico", isBeneficioNonVedenteFromStorico);
            lReturn.Add("IsRichiestaBonus154Abilitata", isRichiestaBonus154Abilitata);
            lReturn.Add("IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione", isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione);
            //ENG - Aggiornamento Memo86
            lReturn.Add("IsPresenteTrattenutaFondoCreditoDaPrelievo", isPresenteTrattenutaFondoCreditoDaPrelievo);
            lReturn.Add("IsPensioneTipoContributivoAnzianitàVecchiaia", isPensioneTipoContributivoAnzianitàVecchiaia);
            lReturn.Add("IsAnte96", isAnte96);

            return lReturn;
        }

        //private static bool? IsEsenzioneFiscale(long idPensione)
        //{
        //    GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
        //    GestioneAnagrafica.GetAnagraficaByIdPensione(idPensione, out datiAnagrafici);

        //    if (datiAnagrafici != null && datiAnagrafici.ResidenzaEstero.HasValue && datiAnagrafici.ResidenzaEstero.Value && !string.IsNullOrEmpty(datiAnagrafici.CodiceComuneResidenza) && datiAnagrafici.CodiceComuneResidenza != "Z110")
        //        return true;
        //    else
        //        return false;
        //}

        private static bool? GestioneRiduzioneRetributiva(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (string.IsNullOrEmpty(datiPensione.Gruppo) || datiPensione.Gruppo != "0001")
                return false;
            if (string.IsNullOrEmpty(datiPensione.Prodotto) || datiPensione.Prodotto != "0001")
                return false;
            if (!datiPensione.DataPerfezionamentoRequisiti.HasValue || (datiPensione.DataPerfezionamentoRequisiti.HasValue && DateTime.Compare(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 12, 31).Date) <= 0))
                return false;

            return true;
        }

        private static TipoSalvaguardia? GetTipoSalvaguardia(GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaSalvaguardia214(datiPensione))
                return TipoSalvaguardia.L214;
            else if (Utility.IsDomandaSalvaguardia122(datiPensione))
                return TipoSalvaguardia.L122;
            else if (Utility.IsDomandaSalvaguardia135(datiPensione))
                return TipoSalvaguardia.L135;
            else if (Utility.IsDomandaSalvaguardia228(datiPensione))
                return TipoSalvaguardia.L228;
            else if (Utility.IsDomandaSalvaguardia124(datiPensione))
                return TipoSalvaguardia.L124;
            else if (Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione))
                return TipoSalvaguardia.L124Art11Bis;
            else if (Utility.IsDomandaSalvaguardia147(datiPensione))
                return TipoSalvaguardia.L147;
            else if (Utility.IsDomandaEsuberiPA(datiPensione))
                return TipoSalvaguardia.EsuberiPA;
            else if (Utility.IsDomandaSalvaguardia147_2014(datiPensione))
                return TipoSalvaguardia.L147_2014;
            else if (Utility.IsDomandaSalvaguardia208_2015(datiPensione))
                return TipoSalvaguardia.L208_2015;
            else if (Utility.IsDomandaSalvaguardia232_2016(datiPensione))
                return TipoSalvaguardia.L232_2016;
            else if (Utility.IsDomandaAPEPrecoci(datiPensione))
                return TipoSalvaguardia.APE_Precoci;
            else if (Utility.IsDomandaSalvaguardia178_2020(datiPensione))
                return TipoSalvaguardia.L178_2020;
            else
                return null;
        }

        private static bool IsGestioneNormale(GestionePensione.DatiPensione datiPensione)
        {
            bool isNormale = false;

            if (datiPensione != null && (datiPensione.SiglaCategoria.Trim() == "VOS" || datiPensione.SiglaCategoria.Trim() == "IOS" || datiPensione.SiglaCategoria.Trim() == "SOS"))
                isNormale = true;

            return isNormale;
        }

        private static bool IsImportoIVSVisible(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            DateTime? dataCompare = null;
            if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue)
                dataCompare = datiDanteCausa.DecorrenzaPensione;
            else
                if (datiPensione != null && datiPensione.DecorrenzaOriginaria.HasValue)
                dataCompare = datiPensione.DecorrenzaOriginaria;
            else
                dataCompare = null;

            if (dataCompare != null)
                if (!Utility.DataSuccessivaA(dataCompare.Value, new DateTime(1976, 08, 01)))
                    return true;

            return false;
        }

        #endregion Cross Properties

        public static int GetNumeroSettimaneItalianeMisura(List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo, List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo)
        {
            int settimaneItalianeMisura = 0;

            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo calcoloContrib in listaDatiCalcoloContributivo)
                {
                    if (calcoloContrib.NSettimane.HasValue)
                        settimaneItalianeMisura = settimaneItalianeMisura + (int)calcoloContrib.NSettimane;
                    if (calcoloContrib.NSettimaneQuotaDL214.HasValue)
                        settimaneItalianeMisura = settimaneItalianeMisura + (int)calcoloContrib.NSettimaneQuotaDL214;
                }
            }

            if (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo calcoloRetrib in listaDatiCalcoloRetributivo)
                {
                    if (calcoloRetrib.QuotePrimeLiquidate.ToString() == "A")
                        settimaneItalianeMisura = settimaneItalianeMisura + (int)calcoloRetrib.NSettimaneQuotaA;

                    if (calcoloRetrib.QuotePrimeLiquidate.ToString() == "B")
                        settimaneItalianeMisura = settimaneItalianeMisura + (int)calcoloRetrib.NSettimaneQuotaB;
                }
            }

            return settimaneItalianeMisura;
        }

        public static bool ControlsUfficioPagatoreArretratiEsteri(string ufficioPagatore, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, byte? codiceArretrati, bool? codiceBloccoArretratiEE, out string messaggioVideo)
        {
            if (codiceArretrati.HasValue && codiceArretrati.Value == 1 && (codiceBloccoArretratiEE.HasValue && codiceBloccoArretratiEE.Value) &&
                    string.IsNullOrEmpty(ufficioPagatore))
            {
                messaggioVideo = "Ufficio Pagatore Istituzione Estera mancante.";
                return false;
            }

            if ((!codiceArretrati.HasValue) || (codiceArretrati.Value == 8 && codiceBloccoArretratiEE.HasValue && codiceBloccoArretratiEE.Value))
            {
                messaggioVideo = "Accantonamento Arretrati incompatibile con il Codice Ufficio Pagatore Istituzione Estera";
                return false;
            }

            if (!string.IsNullOrEmpty(ufficioPagatore) && !codiceBloccoArretratiEE.HasValue)
            {
                messaggioVideo = "I dati 'Blocco Arretrati Estero' devono essere presenti contemporaneamente";
                return false;
            }

            if (!ControlsUfficioPagatore(ufficioPagatore, listaPrestazioniEE, codiceBloccoArretratiEE, out messaggioVideo))
                return false;

            if (!string.IsNullOrEmpty(ufficioPagatore) && codiceBloccoArretratiEE.HasValue && !codiceBloccoArretratiEE.Value)
            {
                messaggioVideo = "In presenza del Codice Ufficio Pagatore Istituzione Estera, il Codice Blocco Arretrati deve essere valorizzato a 'SI'";
                return false;
            }

            return true;
        }

        private static bool ControlsUfficioPagatore(string ufficioPagatore, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE, bool? codiceBloccoArretratiEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceBloccoArretratiEE.HasValue && codiceBloccoArretratiEE.Value)
            {
                List<GestioneDecodifica.UfficiPagatoriEsteri> elencoUfficiPagatori = null;
                GestioneDecodifica.GetUfficiPagatoriEsteri(out elencoUfficiPagatori);
                GestioneDecodifica.UfficiPagatoriEsteri ufficio = null;

                if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
                {
                    foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEE in listaPrestazioniEE)
                    {
                        ufficio = elencoUfficiPagatori.Find(delegate (GestioneDecodifica.UfficiPagatoriEsteri code)
                        {
                            return (code.CodiceStato == int.Parse(prestazioneEE.CodiceStatoEE) && code.CodiceIstituzione == int.Parse(prestazioneEE.CodiceIstituzione));
                        });

                        //Germania - per codice istituzione maggiore di 9, l'ufficio pagatore deve contenere LVE
                        if (int.Parse(prestazioneEE.CodiceStatoEE) == 10 && int.Parse(prestazioneEE.CodiceIstituzione) > 9 && ufficioPagatore.ToUpperInvariant() != "LVE")
                        {
                            messaggioVideo = "Il 'Codice Ufficio Pagatore Istituzione Estera' deve essere valorizzato con LVE per Codice Istituzione maggiore di 9";
                            return false;
                        }
                    }
                }

                if (ufficio != null && string.IsNullOrEmpty(ufficioPagatore))
                {
                    messaggioVideo = "Il Codice Ufficio Pagatore Istituzione Estera deve essere valorizzato";
                    return false;
                }

                if ((ufficio != null && !string.IsNullOrEmpty(ufficioPagatore.ToUpperInvariant())) && !ufficio.Descrizione.Equals(ufficioPagatore.ToUpperInvariant()))
                {
                    messaggioVideo = "Codice Ufficio Pagatore Istituzione Estera non corrispondente con il Codice Stato e Codice Istituzione.";
                    return false;
                }
            }

            return true;
        }

        public enum TipoSalvaguardia
        {
            L214,
            L122,
            L135,
            L228,
            L124,
            L124Art11Bis,
            L147,
            EsuberiPA,
            L147_2014,
            L208_2015,
            L232_2016,
            APE_Precoci,
            L178_2020
        }
    }
}
