using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaTitolare
    {
        #region public members
        public static bool GetAreaTitolareByDatiPensione(GestionePensione.DatiPensione datiPensione, out BLCommon.Entity.AreaTitolare areaTitolareBL, out Entity.Anagrafica anagrafica, out string errori)
        {
            errori = "";
            areaTitolareBL = null;
            anagrafica = null;
            try
            {
                BLCommon.GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolareBL);
                if (areaTitolareBL == null)
                    return true;

                if (areaTitolareBL.Anagrafica != null)
                {
                    List<Entity.Anagrafica> anag = null;
                    if (!GestioneAreaRiepilogo.ValorizzaAnagraficaFromDB(areaTitolareBL.Anagrafica, out anag, out errori))
                        throw new INPS.DNA.DnaValidationException(errori);
                    anagrafica = anag[0];
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }

            return true;
        }

        public static void AggiornaSemaforiDopoARCA(GestionePensione.DatiPensione datiPensione, bool isRiapertura, Dictionary<Utility.TabAggArca, byte?> listaTabAggARCA)
        {
            if (listaTabAggARCA != null && listaTabAggARCA.Count > 0)
            {
                foreach (KeyValuePair<Utility.TabAggArca, byte?> tabAggARCA in listaTabAggARCA)
                {
                    GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
                    GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazione = null;
                    GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni = null;
                    GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = null;
                    GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = null;

                    switch (tabAggARCA.Key)
                    {
                        case Utility.TabAggArca.Anagrafica:
                            GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);

                            datiQuadroTitolare.TabAnagrafica = tabAggARCA.Value;

                            GestioneQuadri.SalvaQuadroTitolare(datiPensione.Id, datiQuadroTitolare);
                            break;

                        case Utility.TabAggArca.ResidenzaEstero:
                            GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);

                            datiQuadroTitolare.TabResidenzeEstero = tabAggARCA.Value;

                            GestioneQuadri.SalvaQuadroTitolare(datiPensione.Id, datiQuadroTitolare);
                            break;

                        case Utility.TabAggArca.DatiGenerici:
                            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazione);

                            datiQuadroLiquidazione.TabDatiGenerici = tabAggARCA.Value;

                            GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazione);
                            break;

                        case Utility.TabAggArca.Detrazioni:
                            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                                break;

                            GestioneQuadri.GetQuadroDetrazioniByDatiPensione(datiPensione, out datiQuadroDetrazioni);

                            datiQuadroDetrazioni.TabDetrazioni = tabAggARCA.Value;
                            if (datiQuadroDetrazioni.Tipo != 2)
                                datiQuadroDetrazioni.Tipo = 2;

                            GestioneQuadri.SalvaQuadroDetrazioni(datiPensione.Id, datiQuadroDetrazioni);
                            break;

                        case Utility.TabAggArca.Redditi:
                            if (Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiapertura))
                                break;

                            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out datiQuadroRedditi);

                            if (datiQuadroRedditi.TabRedditi == 2)
                            {
                                datiQuadroRedditi.TabRedditi = tabAggARCA.Value;
                                switch (tabAggARCA.Value)
                                {
                                    case 1:
                                        datiQuadroRedditi.Tipo = 1;
                                        break;
                                    case 0:
                                        datiQuadroRedditi.Tipo = 2;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                            break;

                        case Utility.TabAggArca.Eliminazione:
                            GestioneQuadri.GetQuadroEliminazioneByDatiPensione(datiPensione, out datiQuadroEliminazione);
                            GestionePensione.DatiEliminazione datiEliminazione = null;
                            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);
                            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
                            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

                            // Se Flag Provvisoria è true i dati Eliminazione non sono inseribili, quindi il quadro non va visualizzato
                            if (!(datiNuoveLiquidate != null && datiNuoveLiquidate.FlagProvvisoria.HasValue && datiNuoveLiquidate.FlagProvvisoria.Value))
                            {
                                if (tabAggARCA.Value == 0)
                                {
                                    if (datiEliminazione == null || datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
                                    {
                                        datiQuadroEliminazione.Tipo = 2;
                                        datiQuadroEliminazione.TabEliminazione = 0;
                                    }
                                }
                                else if (tabAggARCA.Value == 1)
                                {
                                    if (datiEliminazione == null || datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
                                    {
                                        datiQuadroEliminazione.Tipo = 1;
                                        datiQuadroEliminazione.TabEliminazione = 1;
                                    }
                                }
                            }

                            GestioneQuadri.SalvaQuadroEliminazione(datiPensione.Id, datiQuadroEliminazione);
                            break;
                    }
                }
            }
        }

        public static void EliminaEsenzioneFiscale(long idPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            datiIstruttoria.CodiceComunicazioneCampo4 = null;
            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
        }

        #endregion public members

        #region Anagrafica

        public static bool SalvaAnagrafica(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, BLCommon.Entity.AreaTitolare areaTitolareBL, bool IsSingleTabSaved,
            out bool isTabAnagraficaSaved, bool isRiaperturaDomanda, DateTime dataSistema, out bool isWarning, out string errori)
        {
            errori = string.Empty;
            string erroriWarning = string.Empty;
            isTabAnagraficaSaved = false;
            isWarning = false;
            byte? scaltaLavoratriceMadreSalvatiDB = null;

            try
            {
                if (datiPensione == null)
                    return false;

                List<GestioneOneri.DatiOneri> lstDatiOneri = null;
                GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out lstDatiOneri);

                List<GestioneDecodifica.GruppoOneri> decGruppoOnere = null;
                List<GestioneDecodifica.SottoGruppoOneri> decSottoGruppoOneri = null;

                GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
                GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out datiQuadroOneri);

                GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
                GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);

                GestioneFondo.DatiFondo datiFondo = null;
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);

                GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
                GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

                List<GestioneFamiliari.Familiare> datiFamiliari = null;
                List<GestioneAnagrafica.DatiAnagrafici> datiAnagraficiFamiliari = null;
                GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out datiFamiliari, out datiAnagraficiFamiliari);

                List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
                GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaCodMaggFamiliari);

                BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                GestioneDanteCausa.GetDanteCausaByIdPensione(datiPensione.Id, out datiDanteCausa);

                BLCommon.GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

                List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = null;
                GestioneCalcoloVittimeTerrorismo.GetCalcoloVittimeTerrorismoByIdPensione(datiPensione.Id, out listaDatiCalcoloVittimeTerrorismo);

                GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
                GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);

                GestioneLavorazione.DatiLavorazione datiLavorazione = null;
                GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

                GestionePensione.DatiEliminazione datiEliminazione = null;
                GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);

                GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
                GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out ctrl);

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                #region Data for Ante Armonizzazione

                List<GestioneDatiServizioUtile.ServizioUtile> lstDatiServizioUtile = null;
                GestioneDL407.DatiDL407 datiDl407 = null;
                GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
                GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;
                Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcoloById(datiPensione.TipoCalcolo, datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione).GetValueOrDefault());

                #endregion Data for Ante Armonizzazione

                if (Utility.IsDomandaVOPGI_AGI(datiPensione))
                {
                    ControlsDatiAnagraficaVOPGI_AGI(datiPensione, areaTitolareBL, out errori);
                }
                //VOPGI con Diritto Autonomo: disabilitare i controlli anagrafici attualmente previsti nel pannello ‘Anagrafica’ (memo 68/2022)
                else
                {
                    ControlsDatiAnagrafica(datiPensione, areaTitolareBL.Anagrafica, areaTitolareBL, datiFondo, tipoDomanda, IsSingleTabSaved, datiFamiliari, datiDanteCausa, datiGenericiAgoCi,
                        listaDatiCalcoloVittimeTerrorismo, datiLavorazione, datiEliminazione, tipoCalcolo, isRiaperturaDomanda, datiMaggiorazioniBenefici, listaCodMaggFamiliari, out isWarning, out errori, out erroriWarning);
                }

                if (!string.IsNullOrEmpty(errori))
                {
                    if (datiQuadroTitolare.TabAnagrafica != 2 && Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.FS)
                        GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.Titolare_Anagrafica_FS));
                }
                else
                {
                    GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref datiContributivi,
                        ref datiRetributivi, ref datiDl407, ref lstDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out errori);

                    isTabAnagraficaSaved = true;

                    //long? soggettoBeneficiario = datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.SoggettoBeneficiario : null;
                    //long? tipologiaPrestazione = datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaPrestazione : null;
                    //long? tipologiaBeneficio = datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaBeneficio : null;

                    //bool isDatiCalcoloVittimeRosso =
                    //    Utility.IsDatiRetributiviVittimeVisible(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.TipoCalcolo) != Utility.IsDatiRetributiviVittimeVisible(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.TipoCalcolo) ||
                    //    Utility.IsDatiContributiviVittimeVisible(datiPensione, datiPensione.TipoCalcolo) != Utility.IsDatiContributiviVittimeVisible(datiPensione, datiPensione.TipoCalcolo) ||
                    //    Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiario, tipologiaPrestazione, tipologiaBeneficio) != Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiario, tipologiaPrestazione, tipologiaBeneficio);

                    //bool isDatiCalcoloVittimeNonVisibile = !Utility.IsDatiRetributiviVittimeVisible(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.TipoCalcolo) &&
                    //                                       !Utility.IsDatiContributiviVittimeVisible(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.TipoCalcolo) &&
                    //                                       !Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiario, tipologiaPrestazione, tipologiaBeneficio);

                    // Se è cambiata la visibilità oppure se adesso deve essere visualizzato ma il quadro non è visibile
                    bool anagraficaChangedPerOneri = ((((Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) ^
                                     Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri)) ||
                                     (Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) &&
                                     datiQuadroOneri != null && datiQuadroOneri.TabOneri == null) ||
                                     (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione, dataPerfezionamentoRequisiti: datiPensione.DataPerfezionamentoRequisiti) !=
                                     Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione, dataPerfezionamentoRequisiti: areaTitolareBL.Pensione.DataPerfezionamentoRequisiti)
                                     || Utility.IsDomandaRiliquidazioneAnzianitaAnticipata(datiPensione) || Utility.IsDomandaAnticipataConOpzionePL(datiPensione) || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false))))
                                     && !Utility.IsDomandaTipoContributivo(datiPensione, null, null)) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione));
                    bool recordOneriChanged = false;
                    if (anagraficaChangedPerOneri)
                    {
                        GestioneDecodifica.GetGruppoOneri(out decGruppoOnere);
                        GestioneDecodifica.GetSottoGruppoOneri(out decSottoGruppoOneri);
                    }

                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    {
                        areaTitolareBL.Pensione.DecorrenzaOriginaria = datiPensione.DecorrenzaOriginaria;
                        if (datiPensione.DataPerfezionamentoRequisitiUnicarpe.HasValue)
                            areaTitolareBL.Pensione.DataPerfezionamentoRequisiti = datiPensione.DataPerfezionamentoRequisiti;
                        areaTitolareBL.Pensione.LavoratorePubblico = datiPensione.LavoratorePubblico;
                        areaTitolareBL.Pensione.NumeroFigli = datiPensione.NumeroFigli;
                        areaTitolareBL.Pensione.SceltaLavMadri = datiPensione.SceltaLavMadri;
                    }
                    datiPensione.DecorrenzaOriginaria = areaTitolareBL.Pensione.DecorrenzaOriginaria;
                    datiPensione.DataPerfezionamentoRequisiti = areaTitolareBL.Pensione.DataPerfezionamentoRequisiti;
                    if (!(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.CI && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa)))
                        datiPensione.DecorrenzaOriginariaPrima = areaTitolareBL.Pensione.DecorrenzaOriginariaPrima;
                    datiPensione.LavoratorePubblico = areaTitolareBL.Pensione.LavoratorePubblico;

                    scaltaLavoratriceMadreSalvatiDB = datiPensione.SceltaLavMadri;

                    datiPensione.NumeroFigli = areaTitolareBL.Pensione.NumeroFigli;
                    datiPensione.SceltaLavMadri = areaTitolareBL.Pensione.SceltaLavMadri;
                    datiPensione.DataCondizioniPerComputo = areaTitolareBL.Pensione.DataCondizioniPerComputo;
                    //datiPensione.DataOpzione = areaTitolareBL.Pensione.DataOpzione;
                    //datiPensione.DataRaggiungimentoOpzione = areaTitolareBL.Pensione.DataRaggiungimentoOpzione;

                    //in caso di variazione della sede di destinazione, se i redditi risultano già acquisiti (semaforo verde)
                    //è necessario settarlo a rosso
                    GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = null;
                    bool aggiornaQuadroRedditi = ControlsAggiornamentoQuadroRedditiByIdPensione(datiPensione, areaTitolareBL.Pensione.CodiceSedeDestinazione, datiPensione.CodiceSedeDestinazione,
                        out datiQuadroRedditi);

                    //GESTIONE AGGIORMANENTO SEMAFORO TAB OPZIONE IN LIQUIDAZIONE PENSIONE. PER LE RICOSTITUZIONI, LA TAB SARA' SEMPRE NON VISIBILE (IMPOSTATO IN FASE DI INIZIALIZZAZIONE DEL QUADRO)
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazione = null;
                    if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                    {
                        if (tipoAppartenenza.HasValue)
                        {
                            switch (tipoAppartenenza.Value)
                            {
                                case Utility.TipoAppartenenza.AGO:
                                    ControlsAggiornamentoQuadroLiquidazione(datiPensione, scaltaLavoratriceMadreSalvatiDB, out datiQuadroLiquidazione);
                                    break;
                            }
                        }
                    }

                    //Memo 28_2024
                    GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
                    if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                    {
                        if (((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") ||
                            (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0045" && datiPensione.CodiceTipoRichiesta == "AV")) &&
                            ((datiPensione.TipoCalcolo.HasValue && (datiPensione.TipoCalcolo == (byte)Utility.TipoCalcolo.Contributivo || datiPensione.TipoCalcolo == 19)) ||
                            Utility.IsDomandaTipoContributivo(datiPensione, true, false)) &&
                            datiPensione.DecorrenzaOriginaria.HasValue)
                        {
                            ControlsAggiornamentoQuadroLiquidazione_memo28_2024(datiPensione, out datiQuadroLiquidazione);
                            GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazione);
                        }
                    }

                    datiPensione.CodiceSedeDestinazione = areaTitolareBL.Pensione.CodiceSedeDestinazione;

                    if ((Utility.IsDomandaPSO(datiPensione.SiglaCategoria) || Utility.IsDomandaPMO(datiPensione.SiglaCategoria)) && (datiPensione.NCertificato == null || datiPensione.NCertificato == 0))
                    {
                        datiPensione.NCertificato = areaTitolareBL.Pensione.NCertificato;
                    }
                    if (Utility.IsDomandaVOPGI_AGI(datiPensione) && tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                    {
                        datiPensione.DirittoAutonomo = "DA";
                    }

                    GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
                    GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);
                    GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = null;
                    GestioneQuadri.GetQuadroEliminazioneByDatiPensione(datiPensione, out datiQuadroEliminazione);
                    GestioneIstruttoria.DatiIstruttoria datiIstruttoria;
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                    bool isEliminazioneRossoPerConfermaInvalidita = !GestioneCrossControls.AGO_CI_ControlsEliminazioneConfermaInvalidita(datiPensione,
                        datiEliminazione != null ? datiEliminazione.DataEvento : null, datiIstruttoria != null ? datiIstruttoria.NRiconoscimentiInvalidita : null, dataSistema, isRiaperturaDomanda,
                        out errori);
                    errori = string.Empty;

                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        GestioneAnagrafica.AggiornaAnagrafica(areaTitolareBL.Anagrafica);
                        GestionePensione.SalvaPensione(datiPensione);

                        if (areaTitolareBL.Sindacato != null && !Utility.ConfrontaOggetti(areaTitolareBL.Sindacato, new GestionePensione.DatiSindacato()))
                        {
                            if (Utility.IsSindacatoPresente(areaTitolareBL.Sindacato.CodiceSindacato))
                                areaTitolareBL.Sindacato.DecorrenzaSindacato = datiPensione.DecorrenzaOriginaria;
                            else
                                areaTitolareBL.Sindacato.DecorrenzaSindacato = null;
                            GestionePensione.SalvaSindacato(datiPensione.Id, areaTitolareBL.Sindacato);
                        }
                        else
                            GestionePensione.EliminaSindacati(datiPensione.Id);

                        GestioneQuadri.GestioneSemaforoQuadroTitolare(datiPensione, false, false, false, false, true, areaTitolareBL, 0, null, ref datiQuadroTitolare);

                        if (!Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda))
                        {
                            if (aggiornaQuadroRedditi)
                            {
                                switch (datiQuadroRedditi.Tipo.Value)
                                {
                                    case 1:
                                        datiQuadroRedditi.Tipo = 2;
                                        datiQuadroRedditi.TabRedditi = 0;
                                        break;
                                    case 2:
                                        datiQuadroRedditi.TabRedditi = 0;
                                        break;
                                    default:
                                        break;
                                }
                                GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                            }
                        }

                        //GESTIONE AGGIORMANENTO SEMAFORO TAB OPZIONE IN LIQUIDAZIONE PENSIONE. PER LE RICOSTITUZIONI, LA TAB SARA' SEMPRE NON VISIBILE (IMPOSTATO IN FASE DI INIZIALIZZAZIONE DEL QUADRO)
                        if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                        {
                            switch (tipoAppartenenza.Value)
                            {
                                case Utility.TipoAppartenenza.AGO:
                                    GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazione);
                                    break;
                            }
                        }

                        #region Gestione Quadro Dati Contributivi
                        if (tipoAppartenenza == Utility.TipoAppartenenza.FS)
                        {
                            //ENG - MEMO 50/2023
                            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
                            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);
                            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
                            switch (tipoFondo)
                            {
                                case Utility.TipoFondo.ES:
                                case Utility.TipoFondo.GAS:
                                    if (!(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione))
                                    {
                                        if (quadroDatiContributivi.TabDatiAgo != 2)
                                        {
                                            if (Utility.IsDomandaReversibilita(datiPensione))
                                            {
                                                if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue)
                                                {
                                                    if (Utility.DataStrettamenteSuccessivaA(datiDanteCausa.DecorrenzaPensione.Value, new DateTime(1998, 02, 01)))
                                                        quadroDatiContributivi.TabDatiAgo = 0;
                                                    else
                                                        quadroDatiContributivi.TabDatiAgo = 1;
                                                }
                                            }
                                            else
                                            {
                                                if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1998, 02, 01)))
                                                    quadroDatiContributivi.TabDatiAgo = 0;
                                                else
                                                    quadroDatiContributivi.TabDatiAgo = 1;
                                            }

                                            GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                                        }
                                    }
                                    break;
                                case Utility.TipoFondo.ET:
                                    //Gestione per Ante Armonizzazione
                                    if (Utility.IsVisibleTabAltraPensioneDatiAgo(datiPensione, datiDanteCausa, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione))
                                        quadroDatiContributivi.TabDatiAgo = 0;
                                    else
                                        quadroDatiContributivi.TabDatiAgo = null;
                                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                                    break;
                            }
                        }
                        else if (tipoAppartenenza == Utility.TipoAppartenenza.AGO)
                        {
                            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
                            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
                            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);
                            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);
                            if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) && listaDatiCalcoloContributivo == null && listaDatiCalcoloRetributivo == null)
                            {
                                quadroDatiContributivi = null;
                                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                            }
                        }

                        //if (isDatiCalcoloVittimeRosso && !isDatiCalcoloVittimeNonVisibile)
                        //{
                        //    quadroDatiContributivi.TabVittime = 0;
                        //    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                        //}
                        //else if (isDatiCalcoloVittimeNonVisibile)
                        //{
                        //    quadroDatiContributivi.TabVittime = null;
                        //    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                        //}
                        #endregion Gestione Quadro Dati Contributivi

                        #region Gestione Semaforo Eliminazione

                        // Per le ricostituzioni il semaforo non deve variare
                        if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                        {
                            // Se Flag Provvisoria è true i dati Eliminazione non sono inseribili, quindi il quadro non va visualizzato
                            if (!(datiNuoveLiquidate != null && datiNuoveLiquidate.FlagProvvisoria.HasValue && datiNuoveLiquidate.FlagProvvisoria.Value))
                            {
                                if (tipoAppartenenza != null && (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI))
                                {
                                    if (datiEliminazione == null || datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
                                    {
                                        if ((areaTitolareBL.Anagrafica.DataMorte.HasValue && Utility.DataSuccessivaA(areaTitolareBL.Anagrafica.DataMorte.Value, datiPensione.DecorrenzaOriginaria.Value))
                                            || (isEliminazioneRossoPerConfermaInvalidita) ||
                                            (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) && datiGenericiAgoCi != null && datiGenericiAgoCi.ScadenzaAssegno.HasValue &&
                                                Utility.DataSuccessivaA(Utility.FirstDayOfMonth(dataSistema), Utility.FirstDayOfMonth(datiGenericiAgoCi.ScadenzaAssegno.Value)))
                                                || Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
                                        {

                                            datiQuadroEliminazione.Tipo = 2;
                                            datiQuadroEliminazione.TabEliminazione = 0;
                                            GestioneQuadri.SalvaQuadroEliminazione(datiPensione.Id, datiQuadroEliminazione);

                                        }
                                        else
                                        {

                                            datiQuadroEliminazione.Tipo = 1;
                                            datiQuadroEliminazione.TabEliminazione = 1;
                                            GestioneQuadri.SalvaQuadroEliminazione(datiPensione.Id, datiQuadroEliminazione);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion Gestione Semaforo Eliminazione

                        if (!Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda) && !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione))))
                        {
                            // Eseguo le operazioni sul semaforo degli oneri solo se cambia la visibilità
                            if (anagraficaChangedPerOneri)
                            {
                                GestioneOneri.DatiOneri datiOneriSperDonna = lstDatiOneri != null ? lstDatiOneri.Where(x => x.IdCodeGruppo == decGruppoOnere.Find(y => y.Code == "4700").Id && (x.IdCodeSottoGruppo == (Utility.IsDomandaINPDAP(datiPensione.Gestione) ? decSottoGruppoOneri.Find(y => y.Code == "4702").Id : decSottoGruppoOneri.Find(y => y.Code == "4701").Id))).FirstOrDefault() : null;
                                if (Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) && datiOneriSperDonna == null)
                                {
                                    GestioneOneri.DatiOneri newOneri = new GestioneOneri.DatiOneri { IdCodeGruppo = decGruppoOnere.Find(y => y.Code == "4700").Id, IdCodeSottoGruppo = Utility.IsDomandaINPDAP(datiPensione.Gestione) ? decSottoGruppoOneri.Find(y => y.Code == "4702").Id : decSottoGruppoOneri.Find(y => y.Code == "4701").Id, Decorrenza = areaTitolareBL.Pensione.DecorrenzaOriginaria, IdPensione = datiPensione.Id };
                                    GestioneOneri.SalvaOneriOnere(newOneri);
                                    recordOneriChanged = true;
                                }
                                else if (!Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) && datiOneriSperDonna != null)
                                {
                                    GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                                    lstDatiOneri.Where(x => x.IdCodeGruppo != decGruppoOnere.Find(y => y.Code == "4700").Id && x.IdCodeSottoGruppo != (Utility.IsDomandaINPDAP(datiPensione.Gestione) ? decSottoGruppoOneri.Find(y => y.Code == "4702").Id : decSottoGruppoOneri.Find(y => y.Code == "4701").Id))
                                        .ToList()
                                        .ForEach(x => GestioneOneri.SalvaOneriOnere(x));
                                    recordOneriChanged = true;
                                }
                                GestioneOneri.DatiOneri datiOneriAnticipata2019 = lstDatiOneri != null ? lstDatiOneri.Where(x => x.IdCodeGruppo == decGruppoOnere.Find(y => y.Code == "5400").Id && (x.IdCodeSottoGruppo == decSottoGruppoOneri.Find(y => y.Code == "5401").Id || x.IdCodeSottoGruppo == decSottoGruppoOneri.Find(y => y.Code == "5402").Id)).FirstOrDefault() : null;
                                if (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione, dataPerfezionamentoRequisiti: areaTitolareBL.Pensione.DataPerfezionamentoRequisiti) && datiOneriAnticipata2019 == null)
                                    recordOneriChanged = true;
                                else if (!Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione, dataPerfezionamentoRequisiti: areaTitolareBL.Pensione.DataPerfezionamentoRequisiti) && datiOneriAnticipata2019 != null
                                    && !(Utility.IsDomandaAnticipataConOpzionePL(datiPensione) && Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria))) //Intervento a seguito della segnalazione del rinnovo 2024 di A. Polinari legata alla segnalazione 14728
                                {
                                    GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                                    lstDatiOneri.Where(x => x.IdCodeGruppo != decGruppoOnere.Find(y => y.Code == "5400").Id && x.IdCodeSottoGruppo != decSottoGruppoOneri.Find(y => y.Code == "5401").Id && x.IdCodeSottoGruppo != decSottoGruppoOneri.Find(y => y.Code == "5402").Id)
                                        .ToList()
                                        .ForEach(x => GestioneOneri.SalvaOneriOnere(x));
                                    recordOneriChanged = true;
                                }

                                #region Gestione Semaforo Oneri
                                if ((Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                                    Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                                    Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                                    Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione))
                                    || Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale)
                                    //si passano i datiBeneficioTerrorismo a null in quanto i controlli effettuati sono per domande != da ricostituzione o riapertura
                                    || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null)
                                    || Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri)
                                    || Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione)
                                    || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione)
                                    || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione)
                                    || (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) && !Utility.IsDomandaMIN(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                                    || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione)
                                    || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false) && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                                    || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true)
                                    || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                                    || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                                    || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) ||
                                    (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                                    || (Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                                    || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                                {
                                    if (datiQuadroOneri.TabOneri == null || (datiQuadroOneri.TabOneri == 2 && recordOneriChanged))
                                        datiQuadroOneri.TabOneri = 0; //rosso
                                }
                                else
                                    datiQuadroOneri.TabOneri = null;

                                if (//condizioni visibilità oneri
                                   (Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                                    Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                                    Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione)) ||
                                    Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione)
                                   || Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale)
                                    //si passano i datiBeneficioTerrorismo a null in quanto i controlli effettuati sono per domande != da ricostituzione o riapertura
                                   || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null)
                                   || Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri)
                                   || Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione)
                                   || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione)
                                   || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione)
                                   || (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) && !Utility.IsDomandaMIN(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                                   || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione)
                                   || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false) && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                                   || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                                   || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                                   || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                                   || (Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                                   || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)
                                   ||
                                    //condizioni visibilità prepensionamento
                                   (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione) && !Utility.IsDomandaSalvaguardia122(datiPensione)
                                       && tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti && tipoDomanda != Utility.TipoDomanda.Ripristino && !isRiaperturaDomanda)
                                    || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione))
                                    datiQuadroOneri.Tipo = 2;//visibile 
                                else
                                    datiQuadroOneri.Tipo = 0;

                                GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                                #endregion Gestione Semaforo Oneri
                            }
                        }

                        if ((Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                            && datiPensione.SiglaCategoria.StartsWith("V")
                            && datiMaggiorazioniBenefici != null && (datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "11" || (datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "12" && !(Utility.IsDomandaVOAUT(datiPensione.SiglaCategoria) && datiPensione.IdTipoPLPerRIC.HasValue && datiPensione.IdTipoPLPerRIC.Value == 21 &&
                            datiGenericiAgoCi != null && datiGenericiAgoCi.DataAssunzioneCarico.HasValue && !Utility.DataStrettamenteSuccessivaA(datiGenericiAgoCi.DataAssunzioneCarico.Value, new DateTime(2024, 04, 01))))
                            || datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "13" || datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "14" || datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "18" || datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "19" || datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "24"))
                        {
                            GestioneQuadri.DatiQuadroMaggiorazioniBenefici quadroMaggiorazioniBeneficiBL = null;
                            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out quadroMaggiorazioniBeneficiBL);
                            if (quadroMaggiorazioniBeneficiBL == null) quadroMaggiorazioniBeneficiBL = new GestioneQuadri.DatiQuadroMaggiorazioniBenefici();
                            quadroMaggiorazioniBeneficiBL.TabBenefici = 2;
                            GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, quadroMaggiorazioniBeneficiBL);
                        }

                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            //Eng - Il messaggio non deve essere bloccante Segnalazione 31811
            if (!String.IsNullOrEmpty(erroriWarning))
            {
                if (string.IsNullOrEmpty(errori))
                    errori = erroriWarning;
            }
            return true;
        }

        public static bool ControlsDatiAnagrafica(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, BLCommon.Entity.AreaTitolare areaTitolareBL,
            GestioneFondo.DatiFondo datiFondo, Utility.TipoDomanda tipoDomanda, bool IsSingleTabSaved, List<GestioneFamiliari.Familiare> datiFamiliari,
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo, GestioneLavorazione.DatiLavorazione datiLavorazione,
            GestionePensione.DatiEliminazione datiEliminazione, Utility.TipoCalcolo tipoCalcolo, bool isRiaperturaDomanda, GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari, out bool isWarning, out string errori, out string erroriWarning)
        {
            errori = string.Empty;
            erroriWarning = string.Empty;
            isWarning = false;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione));

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            List<GestioneBancheFideiussione.DecBancaFideiussione> listaDecBancaFideiussione = null;
            GestioneDecodificaAzienda.DecAzienda codiceBancaEsodati = null;

            if (Utility.IsDomandaVESO92(datiPensione.SiglaCategoria))
                GestioneBancheFideiussione.GetDecodificaBancaFideiussione(out listaDecBancaFideiussione);

            if (datiPensione.CodiceBancaEsodati.HasValue)
            {
                List<GestioneDecodificaAzienda.DecAzienda> listaDecAzienda = null;
                GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(datiPensione.SiglaCategoria, datiPensione.Tipo, out listaDecAzienda);
                if (listaDecAzienda != null && listaDecAzienda.Count > 0)
                    codiceBancaEsodati = listaDecAzienda.Find(x => x.Id == datiPensione.CodiceBancaEsodati.Value);
            }

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondo != null && datiFondo.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondo.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            char? derogaTraduzioneSuGP = null;
            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = null;
                GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolareSoggettoDerogato);
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            if (tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
            {
                GestioneCalcolo.GetCalcoloRetributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);
                GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);
            }
            else
            {
                GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);
                GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);
            }

            List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile = null;
            GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listaServizioUtile);

            GestioneEnpals.DatiEnpals datiEnpals = null;
            GestioneEnpals.GetDatiEnpalsByIdPensione(datiPensione.Id, out datiEnpals);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPensioniCiPrestazioniEE);

            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(datiPensione.Id, out listaRecordDatiFondoINPDAP);

            object objectFondoXX = null;
            GestioneFondo.GetFondoXXByDatiPensione(datiPensione, out objectFondoXX);

            if (!VerificaLeggeBilancio2024(datiPensione, areaTitolareBL, out errori))
            {
                return false;
            }

            if (datiPensione.DataPresentazioneDomanda == DateTime.MinValue)
            {
                errori = "La Data Presentazione Domanda è obbligatoria.";
                return false;
            }

            if (!areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue)
            {
                errori = "La decorrenza pensione è obbligatoria";
                return false;
            }

            //ENG - Aggiornamento Memo 90/2016
            if (tipoAppartenenza == Utility.TipoAppartenenza.AGO &&
                ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0192") ||
                 (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0011" && datiPensione.Tipo == "0045") ||
                 (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0045") ||
                 (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022" && datiPensione.Tipo == "0045")))
            {
                if (!areaTitolareBL.Pensione.DataCondizioniPerComputo.HasValue)
                {
                    errori = "Data condizioni per il computo: campo obbligatorio";
                    return false;
                }
            }

            var isDomandaAnte96Before = Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda);
            var isDomandaAnte96After = Utility.IsDomandaAnte96(datiPensione, areaTitolareBL.Pensione, datiDanteCausa, isRiaperturaDomanda);
            if (isDomandaAnte96Before != isDomandaAnte96After)
            {
                if (datiPensione.TipoCalcolo != null || listaDatiCalcoloRetributivo != null || listaDatiCalcoloContributivo != null)
                {
                    errori = "Attenzione, cambiando la decorrenza cambiano anche i dati di calcolo. Occorre eliminarli prima di procedere.";
                    return false;
                }
            }

            DateTime? dataValiditaInferiore = null;
            bool? isDecorrenzaInferioreValida = Utility.ControllaDataDecorrenzaInferiore(datiPensione, Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione), areaTitolareBL.Pensione.DecorrenzaOriginaria, out dataValiditaInferiore);
            if ((!isDecorrenzaInferioreValida.HasValue || !isDecorrenzaInferioreValida.Value) && !(Utility.IsDomandaVOST(datiPensione.SiglaCategoria) || Utility.IsRenditaFacoltativa(datiPensione) || Utility.IsRenditaCasalinghe(datiPensione) || Utility.IsDomandaSPED(datiPensione.SiglaCategoria) || Utility.IsDomandaBancari(datiPensione.SiglaCategoria)))
            {
                errori = "La decorrenza pensione non può essere inferiore al " +
                    (dataValiditaInferiore.HasValue ? dataValiditaInferiore.Value.Month.ToString() + "/" + dataValiditaInferiore.Value.Year.ToString() : "limite minimo");
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensione(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                return false;
            // Memo 79 2025
            if (!GestioneCrossControls.ALL_VerificaDecorPensione_DomandeOrganizzazioniInternazionali(datiPensione, datiDanteCausa, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;
            GestioneControlliDinamici.ControlloDinamico sediDaControllare = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SediBancAgo9195", out sediDaControllare);
            if (!(sediDaControllare != null && !string.IsNullOrEmpty(sediDaControllare.ValoreControllo) &&
                sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == datiPensione.CodiceSede.ToString().PadLeft(4, '0')) &&
                (Utility.IsDomandaBanc_91_95(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null) && Utility.IsDomandaPL(datiPensione))) &&
                ((Utility.IsDomandaBanc_91_95(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null) || Utility.IsDomandaAnte96(datiPensione, areaTitolareBL.Pensione, datiDanteCausa, isRiaperturaDomanda) != null)
                && Utility.IsDomandaPL(datiPensione) && Utility.DisabilitaSalvaAnagrafica() && !GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Titolare_Anagrafica_AGO.BYPASS_ANTE96_BANC_AGO)))
            {
                errori = "Pensione non liquidabile, inviare segnalazione alla casella istituzionale supporto.ivs@inps.it";
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaSperimentaleDonna(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;


            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaWithDataMorteTitolare(areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Anagrafica.DataMorte, out errori))
                return false;

            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
            {
                DateTime? decorrenzaOrig = areaTitolareBL.Pensione.DecorrenzaOriginaria;
                string codiceFiscale = areaTitolareBL.Anagrafica.CodiceFiscale;
                DateTime? dataNascita = areaTitolareBL.Anagrafica.DataNascita;
                if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaPerIndirette(decorrenzaOrig, codiceFiscale, dataNascita, (datiDanteCausa != null) ? datiDanteCausa.DataMorte : null, datiFamiliari, datiPensione, datiDanteCausa, listaCodMaggFamiliari, out errori))
                    return false;
            }

            if (!GestioneCrossControls.ALL_ControlsPerfezionamentoRequisitiSperimentaleDonna(datiPensione, datiAnagraficiTitolare, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, tipoAppartenenza, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePerfRequisitiSperimentaleDonna(datiPensione, tipoAppartenenza, areaTitolareBL.Pensione.DecorrenzaOriginaria,
                    areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Anagrafica.DataNascita, out errori))
                return false;

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, tipoAppartenenza, out errori))
                return false;
            //Eng - Il messaggio non deve essere bloccante Segnalazione 31811
            else if (!String.IsNullOrEmpty(errori))
            {
                erroriWarning = errori;
            }

            //ENG - memo 13 - opzionedonna2023 ddlFigli valorizzabile da view
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Pensione.NumeroFigli, datiAnagraficiTitolare, out errori))
                return false;

            DateTime? dataValiditaSuperiore = null;

            if (!GestioneCrossControls.ALL_VerificaResidenzaEsteroTitolare(areaTitolareBL.Anagrafica.ResidenzaEstero, areaTitolareBL.Anagrafica.CodiceComuneResidenza,
                areaTitolareBL.Anagrafica.FrazioneResidenza, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaResidenzaContributiveFiltroERI(datiPensione, areaTitolareBL.Anagrafica.Cittadinanza, areaTitolareBL.Anagrafica.CodiceComuneResidenza, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaProvinciaTitolare(areaTitolareBL.Anagrafica.ProvinciaResidenza, out errori))
                return false;

            bool? isDecorrenzaSuperioreValida = Utility.ControllaDataDecorrenzaSuperiore(areaTitolareBL.Pensione.DecorrenzaOriginaria, tipoAppartenenza, out dataValiditaSuperiore);
            if (!isDecorrenzaSuperioreValida.HasValue || !isDecorrenzaSuperioreValida.Value)
            {
                bool eseguiControllo = true;
                if ((Utility.IsDomandaINPDAP(datiPensione.Gestione) || tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && dataValiditaSuperiore.HasValue)
                {
                    dataValiditaSuperiore = new DateTime(dataValiditaSuperiore.Value.Year, 12, 31);
                    if (DateTime.Compare(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Date,
                    dataValiditaSuperiore.Value.Date) <= 0)
                    {
                        eseguiControllo = false;
                    }
                }
                if (eseguiControllo)
                {
                    errori = "La decorrenza pensione non può essere superiore al " + (dataValiditaSuperiore.HasValue ? dataValiditaSuperiore.Value.Day.ToString() + "/" +
                        dataValiditaSuperiore.Value.Month.ToString() + "/" + dataValiditaSuperiore.Value.Year.ToString() : " limite massimo");
                    return false;
                }
            }


            //string dataValiditaInferiore = null;
            //if (!Utility.ControllaDataDecorrenzaInferiore(areaTitolareBL.Pensione.DecorrenzaOriginaria, tipoAppartenenza, datiPensione.SiglaCategoria, out dataValiditaInferiore))
            //{
            //    errori = "La decorrenza pensione non può essere inferiore al limite minimo: " + dataValiditaInferiore;
            //    return false;
            //}

            //controllo salvaguardia
            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia214(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia135(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensionePerfReqSalvaguardia122(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria,
                areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia228(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia124(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia124Art11Bis(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia147(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia147_2014(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia178_2020(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            //controllo usuranti
            if (!GestioneCrossControls.ALL_VerificaDecorPensioneUsuranti(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneEsuberiPA(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorPensioneSalvaguardia208_2015(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            //20151028 G.Arru - Per il fondo GAS non valgono le regole ordinarie per la dataPerfRequisiti. I controlli in questo caso sono implementati
            //nel metodo FS_GAS_ControlliPerfezionamentoRequisiti(...)
            if (tipoFondo != Utility.TipoFondo.GAS)
            {
                if ((datiPensione.Gruppo == "0002" && !Utility.IsDomandaENPALS(datiPensione.Gestione) && !(Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione)))
                    || datiPensione.Gruppo == "0003" || (Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.TipoLetturaUnicarpe == 'L' && (datiPensione.SiglaCategoria.Substring(0, 2).ToUpperInvariant() == "SO" || datiPensione.SiglaCategoria.Substring(0, 2).ToUpperInvariant() == "IO")))
                {
                    if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue)
                    {
                        errori = "La data di perfezionamento dei requisiti non è prevista";
                        return false;
                    }
                }
            }

            if (!GestioneCrossControls.FS_VerificaFinestraMobileVecchiaiaVEL_VET_VTT(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP, derogaTraduzioneSuGP, out errori))
                return false;

            if (areaTitolareBL.Pensione.SceltaLavMadri == null && Utility.IsMaggiorazioniSceltaLavMadri(datiMaggiorazioniBenefici) &&
                !(Utility.IsDomandaVOAUT(datiPensione.SiglaCategoria) && datiPensione.IdTipoPLPerRIC.HasValue && datiPensione.IdTipoPLPerRIC.Value == 21 &&
                  datiGenericiAgoCi != null && datiGenericiAgoCi.DataAssunzioneCarico.HasValue && !Utility.DataStrettamenteSuccessivaA(datiGenericiAgoCi.DataAssunzioneCarico.Value, new DateTime(2024, 04, 01))))
            {
                errori = "Eliminare i dati Benefici di Maggiorazione / Benefici relativi alle lavoratrici madri prima di procedere.";
                return false;
            }

            if (areaTitolareBL.Pensione.SceltaLavMadri != null && datiMaggiorazioniBenefici != null && !Utility.CheckAbbinamentoMaggiorazioniSceltaLavMadri(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, areaTitolareBL.Pensione.SceltaLavMadri))
            {
                errori = "Eliminare i dati Benefici di Maggiorazione / Benefici relativi alle lavoratrici madri non congruenti prima di procedere.";
                return false;
            }

            if (Utility.IsGestioneLavoratriciMadri(datiPensione) && areaTitolareBL.Anagrafica.Sesso.GetValueOrDefault() == 'F')
            {
                if ((areaTitolareBL.Pensione.SceltaLavMadri.HasValue && areaTitolareBL.Pensione.SceltaLavMadri > 0 && areaTitolareBL.Pensione.NumeroFigli.GetValueOrDefault() == 0) ||
                (areaTitolareBL.Pensione.NumeroFigli.HasValue && areaTitolareBL.Pensione.NumeroFigli > 0 && areaTitolareBL.Pensione.SceltaLavMadri.GetValueOrDefault() == 0))
                {
                    //ENG - memo 28/2024
                    if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                    {
                        char codNat1 = ' ';
                        char codNat2 = ' ';
                        char codNat3 = ' ';
                        Utility.GetCodiciNatura(datiPensione.NaturaPensione, out codNat1, out codNat2, out codNat3);
                        Utility.TipoFondo? tipofondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                        if (((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") ||
                            (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0045" && datiPensione.CodiceTipoRichiesta == "AV") ||
                            (datiPensione.IdTipoPLPerRIC.HasValue && ((datiPensione.IdTipoPLPerRIC == 7 && ((codNat1 == '1' || codNat1 == '2') || (datiFondo != null &&
                            ((tipofondo == Utility.TipoFondo.PT && datiFondo.CodiceSpecifico == 41) ||
                            (tipofondo == Utility.TipoFondo.FS && datiFondo.CodiceSpecifico == 47) ||
                            (tipofondo == Utility.TipoFondo.TT && datiFondo.CodiceSpecifico == 14) ||
                            (tipofondo == Utility.TipoFondo.ET && datiFondo.CodiceSpecifico == 22) ||
                            (Utility.IsDomandaINPDAP(datiPensione.Gestione) && (datiFondo.CodiceSpecifico == 181 || datiFondo.CodiceSpecifico == 182)))
                            ))) || datiPensione.IdTipoPLPerRIC == 26))) &&
                            (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2023, 12, 31))))
                        {
                            if (areaTitolareBL.Pensione.NumeroFigli.GetValueOrDefault() == 0)
                            {
                                errori = "Se il campo 'Numero figli' non è presente, il campo 'Lavoratrici madri' non deve essere presente";
                                return false;
                            }
                        }
                        else
                        {
                            errori = "I campi 'Lavoratrici madri' e 'Numero figli', se acquisiti, sono entrambi obbligatori.";
                            return false;
                        }
                    }
                    else
                    {
                        errori = "I campi 'Lavoratrici madri' e 'Numero figli', se acquisiti, sono entrambi obbligatori.";
                        return false;
                    }
                }
                if (areaTitolareBL.Pensione.NumeroFigli.GetValueOrDefault() == 4 &&
                    Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2024, 12, 31)) &&
                    GestioneCrossControls.checkMemo228_2025(datiPensione)) // Memo 228_2025
                {
                    //if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && !Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2024, 12, 31)))
                    //{
                    //    errori = "La data perfezionamento requisiti non può essere precedente il 1° gennaio 2025.";
                    //    return false;
                    //}
                    //else
                    //{
                    DateTime? dataLimiteInf = null;
                    int anno = 2025;
                    if (tipoAppartenenza == Utility.TipoAppartenenza.FS && (Utility.IsDomandaINPDAP(datiPensione.Gestione) || (tipoFondo.HasValue && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))))
                        dataLimiteInf = new DateTime(anno, 01, 02);
                    else
                        dataLimiteInf = new DateTime(anno, 02, 01);

                    if (dataLimiteInf.HasValue && areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue)
                    {
                        if (!Utility.DataSuccessivaA(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value, dataLimiteInf.Value))
                        {
                            errori = "La decorrenza pensione non può essere minore della data " + String.Format("{0:dd/MM/yyyy}", dataLimiteInf);
                            return false;
                        }
                    }
                    //}
                }

            }

            if (!Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                //if (!((datiPensione.Gruppo == "0002" && (datiPensione.Prodotto == "0011" || datiPensione.Prodotto == "0012")) || datiPensione.Gruppo == "0031"))
                //per le RIC la dataperfezionamento requisiti non è mai prevista
                DateTime dataControllo = new DateTime(2010, 12, 31);
                if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
                    dataControllo = new DateTime(2011, 1, 31);
                //ENG - MEMO 166/2023
                if ((Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione) ||
                     (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                     (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) ||
                     (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                {
                    dataControllo = new DateTime(2010, 12, 31);
                    if (areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue && !areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue)
                    {
                        errori = "La data di perfezionamento dei requisiti è obbligatoria per le pensioni di tipo contributivo";
                        return false;
                    }
                }

                //ENG - Per le Ric AGO tranne le AUT e Superstiti il controllo deve essere effettuato in caso la DPR non è presente e la decorrenza pensione è successiva al 31/12/2011
                // non deve essere effettuato più in caso la DPR non è presente e la decorrenza pensione è successiva al 01/2011
                if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsDomandaAUT(datiPensione) && !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && !datiPensione.SiglaCategoria.StartsWith("I"))
                {
                    if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaINPGI(datiPensione) && datiPensione.GP1AV91B == "2") &&
                        areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value, new DateTime(2011, 12, 31)) && !areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue)
                    {
                        errori = "La data di perfezionamento dei requisiti è obbligatoria per le pensioni con decorrenza successiva al 31/12/2011";
                        return false;
                    }
                }
                else
                {
                    if (!(datiPensione.Gruppo == "0002" || datiPensione.Gruppo == "0003") &&
                        datiPensione.SiglaCategoria.StartsWith("V") && !Utility.IsRenditaCasalinghe(datiPensione) && !Utility.IsRenditaFacoltativa(datiPensione) &&
                        !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaVOAUT(datiPensione.SiglaCategoria)) &&
                        !(tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS && (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && !(tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT || Utility.IsDomandaINPDAP(datiPensione.Gestione))))
                    {
                        if (!(tipoAppartenenza.Value == Utility.TipoAppartenenza.FS && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_MotiviContributivi(datiPensione))))
                        {
                            if (areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue && areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.CompareTo(dataControllo) > 0 &&
                                !areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue)
                            {
                                errori = "La data di perfezionamento dei requisiti è obbligatoria per le pensioni con decorrenza > 01/2011";
                                return false;
                            }
                        }
                    }
                }

                if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.CompareTo(dataControllo) > 0)
                {
                    if (!datiPensione.SiglaCategoria.StartsWith("I") && !datiPensione.SiglaCategoria.StartsWith("S"))
                    {
                        if (!GestioneCrossControls.ALL_VerificaPerfezRequisitiDecPensioneAnzianitaVecchiaia(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria,
                            areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, derogaTraduzioneSuGP, isRiaperturaDomanda, out errori))
                            return false;
                    }
                    else
                    {
                        DateTime? dataPerfReq = null;
                        if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
                        {
                            if (tipoFondo.HasValue)
                            {
                                switch (tipoFondo.Value)
                                {
                                    case Utility.TipoFondo.FS:
                                    case Utility.TipoFondo.PT:
                                        dataPerfReq = areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value;
                                        break;
                                    default:
                                        dataPerfReq = Utility.DataFromInt(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Year, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Month, 1);
                                        break;
                                }
                            }
                            else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                            {
                                dataPerfReq = areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value;
                            }
                        }
                        else
                        {
                            dataPerfReq = Utility.DataFromInt(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Year, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Month, 1);
                        }

                        //if (DateTime.Compare(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Date, new DateTime(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Year, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Month, 1).Date) <= 0)
                        if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.SiglaCategoria.StartsWith("S")) && DateTime.Compare(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Date, dataPerfReq.Value) <= 0)
                        {
                            errori = "La decorrenza pensione non può essere inferiore alla data di perfezionamento dei requisiti";
                            return false;
                        }
                    }
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCiviliSuperstiti(datiPensione, datiFamiliari, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAPEPrecoce(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiAnagraficiTitolare, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaPerfezionamentoRequisitiQuota100(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneQuota100(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Pensione.LavoratorePubblico, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneQuota102(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Pensione.LavoratorePubblico, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePrecoci(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAnticipataFlessibile(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Pensione.LavoratorePubblico, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneIOPGI(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                return false;

            //ENG - Memo 28
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaTipo0017_0045Pav(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                return false;

            //ENG - Memo 123/2024
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAnticipataFlessibileLeggeDiBilancio2024(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Pensione.LavoratorePubblico, out errori))
                return false;

            if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue &&
                ((Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) &&
                (Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2017, 12, 31)) && Utility.DataSuccessivaA(new DateTime(2018, 03, 31), datiPensione.DataPresentazioneDomanda)))
                ||
                (Utility.IsDomandaAPEPrecoci(datiPensione) &&
                (Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2017, 12, 31)) && Utility.DataSuccessivaA(new DateTime(2018, 03, 01), datiPensione.DataPresentazioneDomanda)))))
            {
                if (areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value, new DateTime(2018, 02, 01)))
                {
                    errori = "La decorrenza pensione non può essere antecedente al 02/2018";
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota100(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out errori))
                return false;
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota102(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out errori))
                return false;
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaDomandaRequisitoAnticipatoArt1(datiPensione, areaTitolareBL.Pensione, datiAnagraficiTitolare.DataNascita, out errori))
                return false;
            if (!GestioneCrossControls.AGO_VerificaRequisitoEtaDomandaVMP(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiAnagraficiTitolare.DataNascita, out errori))
                return false;
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibile(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out errori))
                return false;

            if (!GestioneCrossControls.ALL_VerificaFinestraDomandaAnzianitaPerLeggeBilancio2019(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria,
                areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, isRiaperturaDomanda, tipoAppartenenza, out errori))
                return false;

            if (!GestioneCrossControls.AGO_VerificaRequisitoEtaPrepensionamentoEBA(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out errori))
                return false;

            //ENG - Memo 123/2024
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibileLeggeDiBilancio2024(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiAnagraficiTitolare.DataNascita, out errori))
                return false;

            #region Controls Stato Civile

            BLCommon.Entity.AreaTitolare areaTitolareDB = null;
            BLCommon.GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolareDB);

            //Reimposto nuovamente la decorrenza pensione nel formato MM/AAAA (per FS e PT la decPensione è nel formato GG/MM/AAAA).
            DateTime? decPensione = Utility.DataFromInt(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Year, areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Month, 1);

            if (IsSingleTabSaved)
            {
                if (areaTitolareDB != null)
                {
                    if (areaTitolareDB.ElencoStatiCivili.Count > 0)
                    {
                        areaTitolareDB.ElencoStatiCivili.Sort(delegate(GestioneAnagrafica.DatiStatoCivile c1, GestioneAnagrafica.DatiStatoCivile c2) { return c1.Decorrenza.Value.CompareTo(c2.Decorrenza); });

                        if (areaTitolareDB.ElencoStatiCivili.First().Decorrenza.HasValue && decPensione.Value.CompareTo(areaTitolareDB.ElencoStatiCivili.First().Decorrenza.Value) != 0)
                        {
                            errori = "La decorrenza più remota dello stato civile non coincide con la decorrenza della pensione";
                            return false;
                        }
                    }
                    if (Utility.IsResidenteEstero(areaTitolareDB.Anagrafica.CodiceComuneResidenza) && areaTitolareDB.ElencoResidenzeEstere.Count > 0)
                    {
                        areaTitolareDB.ElencoResidenzeEstere.Sort(delegate(GestioneAnagrafica.DatiResidenzaEstero c1, GestioneAnagrafica.DatiResidenzaEstero c2) { return c1.Decorrenza.Value.CompareTo(c2.Decorrenza); });
                        if (areaTitolareDB.ElencoResidenzeEstere.First().Decorrenza.HasValue && decPensione.Value.CompareTo(areaTitolareDB.ElencoResidenzeEstere.First().Decorrenza.Value) != 0)
                        {
                            errori = "La decorrenza più remota della residenza estera non coincide con la decorrenza della pensione";
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (areaTitolareDB != null)
                {
                    if (areaTitolareBL.ElencoStatiCivili.Count > 0)
                    {
                        if (areaTitolareBL.ElencoStatiCivili.First().Decorrenza.HasValue)
                        {
                            if (areaTitolareBL.ElencoStatiCivili.First().Decorrenza.HasValue && decPensione.Value.CompareTo(areaTitolareBL.ElencoStatiCivili.First().Decorrenza.Value) != 0)
                            {
                                errori = "La decorrenza più remota dello stato civile non coincide con la decorrenza della pensione";
                                return false;
                            }
                        }
                    }

                    if (Utility.IsResidenteEstero(areaTitolareDB.Anagrafica.CodiceComuneResidenza) && areaTitolareBL.ElencoResidenzeEstere.Count > 0)
                    {
                        if (areaTitolareBL.ElencoResidenzeEstere.First().Decorrenza.HasValue && decPensione.Value.CompareTo(areaTitolareBL.ElencoResidenzeEstere.First().Decorrenza.Value) != 0)
                        {
                            errori = "La decorrenza più remota della residenza estera non coincide con la decorrenza della pensione";
                            return false;
                        }
                    }
                }
            }

            #endregion Controls Stato Civile

            #region Sindacato Attivo

            if (areaTitolareBL.Sindacato != null && areaTitolareBL.Sindacato.Stato != null && areaTitolareBL.Sindacato.Stato != Utility.StatoSindacato.Attivo)
            {
                errori = "Sindacato non Attivo. Selezionare un nuovo Sindacato";
                return false;
            }
            #endregion Sindacato Attivo

            GestionePensione.DatiPensione pensioneApp = new GestionePensione.DatiPensione();
            Utility.ValorizzaOggetti(datiPensione, pensioneApp);
            pensioneApp.DataPerfezionamentoRequisiti = areaTitolareBL.Pensione.DataPerfezionamentoRequisiti;

            switch (tipoAppartenenza.Value)
            {
                #region FS

                case Utility.TipoAppartenenza.FS:

                    //Utility.TipoFondo? tipoFondo = Utility.GeTipoFondoByCategoria(datiPensione.SiglaCategoria);

                    if (!areaTitolareBL.Pensione.CodiceSedeDestinazione.HasValue)
                    {
                        errori = "Il campo 'Sede' è obbligatorio";
                        return false;
                    }

                    if (!Utility.ExistSedeProvinciale(areaTitolareBL.Pensione.CodiceSedeDestinazione.Value))
                    {
                        errori = "La 'Sede' inserita non esiste";
                        return false;
                    }

                    if (Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL(datiPensione))
                    {
                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteSenzaPerditaTitoloAbilitante(areaTitolareBL.Pensione.DecorrenzaOriginaria,
                            areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                            return false;

                        if (!GestioneCrossControls.FS_VerificaCompatibilitaPerfezionamentoPersonaleViaggianteSenzaPerditaTitoloAbilitante(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti,
                            areaTitolareBL.Anagrafica.DataNascita, out errori))
                            return false;

                        if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Titolare_Anagrafica_FS.NATI_29FEBBRAIO)
                            && !GestioneCrossControls.FS_VerificaEtaTitolarePersonaleViaggianteSenzaPerditaTitoloAbilitante(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti,
                                areaTitolareBL.Anagrafica.DataNascita, areaTitolareBL.Anagrafica.Sesso, out errori))
                            return false;
                    }

                    if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL(datiPensione))
                    {
                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteConPerditaTitoloAbilitante(areaTitolareBL.Pensione.DecorrenzaOriginaria,
                            areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiPensione, out errori))
                            return false;

                        if (!GestioneCrossControls.FS_VerificaEtaTitolarePersonaleViaggianteConPerditaTitoloAbilitante(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti,
                            areaTitolareBL.Anagrafica.DataNascita, areaTitolareBL.Anagrafica.Sesso, out errori))
                            return false;
                    }



                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:
                            case Utility.TipoFondo.TT:
                                if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcoloDecPensione(areaTitolareBL.Pensione.DecorrenzaOriginaria, Utility.GetTipoCalcolo(datiPensione), out errori))
                                    return false;

                                if (datiFondo != null && !GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(pensioneApp, datiFondo.RiduzioneRetributiva, tipoCalcolo))
                                {
                                    errori = "La data perfezionamento requisiti è incompatibile con la riduzione retributiva; eliminare i dati calcolo prima di proseguire.";
                                    return false;
                                }

                                if (!CrossControlsWithDanteCausa(datiPensione.Id, tipoDomanda, areaTitolareBL, out errori))
                                    return false;

                                if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolareBL, derogaTraduzioneSuGP, out errori))
                                    return false;

                                break;
                            case Utility.TipoFondo.ET:

                                if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcoloDecPensione(areaTitolareBL.Pensione.DecorrenzaOriginaria, Utility.GetTipoCalcolo(datiPensione), out errori))
                                    return false;

                                if (datiFondo != null && !GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(pensioneApp, datiFondo.RiduzioneRetributiva, tipoCalcolo))
                                {
                                    errori = "La data perfezionamento requisiti è incompatibile con la riduzione retributiva; eliminare i dati calcolo prima di proseguire.";
                                    return false;
                                }

                                if (!CrossControlsWithDanteCausa(datiPensione.Id, tipoDomanda, areaTitolareBL, out errori))
                                    return false;

                                if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolareBL, derogaTraduzioneSuGP, out errori))
                                    return false;

                                if (Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione))
                                {
                                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteSenzaPerditaTitoloAbilitante(areaTitolareBL.Pensione.DecorrenzaOriginaria,
                                        areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                                        return false;

                                    if (!GestioneCrossControls.FS_VerificaCompatibilitaPerfezionamentoPersonaleViaggianteSenzaPerditaTitoloAbilitante(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti,
                                        areaTitolareBL.Anagrafica.DataNascita, out errori))
                                        return false;

                                    if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Titolare_Anagrafica_FS.NATI_29FEBBRAIO)
                                        && !GestioneCrossControls.FS_VerificaEtaTitolarePersonaleViaggianteSenzaPerditaTitoloAbilitante(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti,
                                            areaTitolareBL.Anagrafica.DataNascita, areaTitolareBL.Anagrafica.Sesso, out errori))
                                        return false;
                                }

                                if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione))
                                {
                                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteConPerditaTitoloAbilitante(areaTitolareBL.Pensione.DecorrenzaOriginaria,
                                        areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiPensione, out errori))
                                        return false;

                                    if (!GestioneCrossControls.FS_VerificaEtaTitolarePersonaleViaggianteConPerditaTitoloAbilitante(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti,
                                        areaTitolareBL.Anagrafica.DataNascita, areaTitolareBL.Anagrafica.Sesso, out errori))
                                        return false;
                                }

                                break;

                            case Utility.TipoFondo.VL:

                                if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcoloDecPensione(areaTitolareBL.Pensione.DecorrenzaOriginaria, Utility.GetTipoCalcolo(datiPensione), out errori))
                                    return false;

                                if (!datiPensione.SiglaCategoria.StartsWith("I")) // no invalidità 
                                {
                                    //  vecchiaia
                                    if (datiPensione.Prodotto.Trim() == "0002" && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2012, 01, 01)) >= 0)
                                    {
                                        //mail 03-04-2013: bypass controlli per L214 e usuranti per il solo prodotto 0002
                                        //mail 28-11-2013: bypass controlli per L.228 RE: Reeng Pensioni - Salvaguardia L.228 - Punti aperti
                                        //mail 16-07-2014: bypass controlli per L.124 art.11 bis RE: ReEng Pensioni - Salvaguardia L.124/2013 art.11
                                        if (!Utility.IsDomandaSalvaguardia214(datiPensione) && !Utility.IsDomandaSalvaguardia135(datiPensione) && !Utility.IsDomandaSalvaguardia228(datiPensione) &&
                                            !Utility.IsDomandaSalvaguardia124(datiPensione) && !Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) && !Utility.IsDomandaSalvaguardia147(datiPensione) &&
                                            !Utility.IsDomandaUsuranti(datiPensione) && !Utility.IsDomandaEsuberiPA(datiPensione) && !Utility.IsDomandaSalvaguardia147_2014(datiPensione) &&
                                            !Utility.IsDomandaSalvaguardia208_2015(datiPensione) && !Utility.IsDomandaAPEPrecoci(datiPensione))
                                        {
                                            if (!GestioneCrossControls.FS_VerificaEtaTitolareDataPerfRequisitiPostFeb2012(tipoFondo, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Anagrafica.CodiceFiscale,
                                                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out errori))
                                                return false;
                                        }
                                    }
                                }
                                else
                                {
                                    GestioneFondo.DatiFondoVL datiFondoVL = null;
                                    GestioneFondo.GetFondoVLByIdPensione(datiPensione.Id, out datiFondoVL);

                                    if (datiFondoVL != null && datiFondoVL.DataInvalidita.HasValue && datiFondoVL.CodiceArt22.HasValue &&
                                        !GestioneCrossControls.FS_VerificaDecPensWithDataInvaliditaCodeArt22FondoVL(datiPensione, datiDanteCausa, datiPensione.FineAssicurazione, datiFondoVL.DataInvalidita,
                                            datiFondoVL.CodiceArt22, areaTitolareBL.Pensione.DecorrenzaOriginaria, isRiaperturaDomanda, out errori))
                                        return false;
                                }

                                if (datiFondo != null && !GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(pensioneApp, datiFondo.RiduzioneRetributiva, tipoCalcolo))
                                {
                                    errori = "La data perfezionamento requisiti è incompatibile con la riduzione retributiva; eliminare i dati calcolo prima di proseguire.";
                                    return false;
                                }

                                if (!CrossControlsWithDanteCausa(datiPensione.Id, tipoDomanda, areaTitolareBL, out errori))
                                    return false;

                                if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolareBL, derogaTraduzioneSuGP, out errori))
                                    return false;

                                if (Utility.IsDomandaVecchPerditaTitolo(datiPensione))
                                {
                                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteConPerditaTitoloAbilitante(areaTitolareBL.Pensione.DecorrenzaOriginaria,
                                        areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiPensione, out errori))
                                        return false;
                                }

                                break;
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:

                                if (datiFondo != null && !GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(pensioneApp, datiFondo.RiduzioneRetributiva, tipoCalcolo))
                                {
                                    errori = "La data perfezionamento requisiti è incompatibile con la riduzione retributiva; eliminare i dati calcolo prima di proseguire.";
                                    return false;
                                }

                                if (!CrossControlsWithDanteCausa(datiPensione.Id, tipoDomanda, areaTitolareBL, out errori))
                                    return false;

                                if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolareBL, derogaTraduzioneSuGP, out errori))
                                    return false;

                                break;
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                                break;
                            case Utility.TipoFondo.GAS:
                                if (!GestioneCrossControls.FS_GAS_ControlliPerfezionamentoRequisiti(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                                    return false;
                                break;
                            case Utility.TipoFondo.ES:
                                if (pensioneApp.DataPerfezionamentoRequisiti.HasValue && !Utility.DataStrettamenteSuccessivaA(pensioneApp.DataPerfezionamentoRequisiti.Value, new DateTime(2010, 12, 31)))
                                {
                                    errori = "Attenzione non sono soddisfatti i 15 anni di requisiti di età sul fondo.";
                                    return false;
                                }
                                break;
                        }
                    }

                    //per fondo VOLO il controllo tra DecPensione e dataPresentazione per le anzianità è specifico
                    if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001")
                    {
                        DateTime DataLimite = datiPensione.DataPresentazioneDomanda.Date.AddYears(-2);
                        if (areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value, DataLimite))
                        {
                            errori = "La decorrenza pensione non può essere antecedente al " + (DataLimite.Day == 1 ? DataLimite.ToString("MM-yyyy") : DataLimite.AddMonths(1).ToString("MM-yyyy"));
                            return false;
                        }
                    }
                    else if (!(Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault() ||
                        Utility.IsDomandaINPDAP(datiPensione.Gestione) ||
                        (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.EL && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001") ||
                        (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002") || (datiPensione.Gruppo == "0031") ||
                        (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.VL || tipoFondo.Value == Utility.TipoFondo.ET) && datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0011") ||
                        (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT) ||
                        (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS) ||
                        (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PM && datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0010") ||
                        (datiPensione.Gruppo == "0003") ||
                        (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && GestioneCrossControls.BypassVerificaDecorPensionePerAPESocialeEPrecoci(datiPensione, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti))
                        ))
                    {
                        DateTime DataPresentazioneDomanda = datiPensione.DataPresentazioneDomanda.AddDays(1 - datiPensione.DataPresentazioneDomanda.Day);
                        if (areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue &&
                            (Utility.IsDomandaInabilitaAmianto(datiPensione) ? areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.CompareTo(DataPresentazioneDomanda) <= 0 :
                            areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.CompareTo(DataPresentazioneDomanda) < 0))
                        {
                            errori = "La decorrenza pensione non può essere antecedente alla data di presentazione della stessa.";
                            return false;
                        }
                    }

                    if (!CrossControlsFSWithRequisiti247_243ByIdPensione(datiPensione, tipoFondo, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, derogaTraduzioneSuGP, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, areaTitolareBL.Pensione, isRiaperturaDomanda, datiFondo != null ? datiFondo.RiduzioneRetributiva : false, datiFondo != null ? datiFondo.RiduzioneRetributivaPercentuale : null, out errori))
                    {
                        errori += " Eliminare i Dati Calcolo.";
                        return false;
                    }

                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                    {
                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                        {
                            if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolareBL, derogaTraduzioneSuGP, out errori))
                                return false;
                        }
                        if (!GestioneCrossControls.FS_ControlsTipoBeneficioArt24Comma15Bis(datiPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                           datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione,
                           areaTitolareBL.Anagrafica.Sesso, areaTitolareBL.Anagrafica.DataNascita, codiceSpecificoTraduzioneSuGP,
                           listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0 ? listaDatiCalcoloRetributivo.First() : null,
                           listaDatiCalcoloContributivo, listaServizioUtile, listaRecordDatiFondoINPDAP, objectFondoXX, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out errori))
                            return false;
                    }

                    if (!GestioneCrossControls.ALL_ControlsRequisitoEta(datiPensione, tipoAppartenenza, isRiaperturaDomanda, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                        areaTitolareBL.Anagrafica.Sesso, null, null, derogaTraduzioneSuGP, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, null, null, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerTipoContributivo(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                        areaTitolareBL.Anagrafica.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out isWarning, out errori))
                        return false;
                    break;

                #endregion FS

                #region AGO

                case Utility.TipoAppartenenza.AGO:
                    if (string.IsNullOrEmpty(areaTitolareBL.Anagrafica.Cittadinanza) && !datiPensione.TipoAutomazione.HasValue)
                    {
                        errori = "La cittadinanza è un dato obbligatorio";
                        return false;
                    }

                    if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione))
                    {
                        if (!areaTitolareBL.Pensione.CodiceSedeDestinazione.HasValue)
                        {
                            errori = "Il campo 'Sede' è obbligatorio";
                            return false;
                        }
                    }

                    if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneInabilitaAmianto(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                        return false;
                    if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneEsattoriali(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                        return false;
                    if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensione(areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiPensione,
                        datiAnagraficiTitolare, datiDanteCausa, datiLavorazione, datiEliminazione, datiGenericiAgoCi, dataSistema, isRiaperturaDomanda, areaTitolareBL.Pensione.DataCondizioniPerComputo, areaTitolareBL.Pensione, out errori))
                        return false;

                    if (!Utility.IsDomandaENPALS(datiPensione.Gestione))
                    {
                        if (!GestioneCrossControls.AGO_VerificaPerfRequisiti(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, areaTitolareBL.Pensione.DecorrenzaOriginaria,
                            datiGenericiAgoCi != null ? datiGenericiAgoCi.ScadenzaAssegno : null, datiPensione, datiIstruttoria, areaTitolareBL.Pensione.DataCondizioniPerComputo, out errori))
                            return false;

                        if (!GestioneCrossControls.AGO_VerificaCodiceSindacato(areaTitolareBL.Sindacato, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione, out errori))
                            return false;

                        if (datiGenericiAgoCi != null && !GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(pensioneApp, datiGenericiAgoCi.RiduzioneRetributiva, tipoCalcolo))
                        {
                            errori = "La data perfezionamento requisiti è incompatibile con la riduzione retributiva; eliminare i dati istruttoria prima di proseguire.";
                            return false;
                        }

                        if (!CrossControlsWithDanteCausa(datiPensione.Id, tipoDomanda, areaTitolareBL, out errori))
                            return false;

                    }

                    if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolareBL, derogaTraduzioneSuGP, out errori))
                        return false;

                    ////LOGICA PER CONTROLLARE LA PRESENZA DEI DATI DELLA TAB OPZIONI PRIMA DI PROCEDERE CON IL SALVATAGGIO DEL TITOLARE
                    if (!CrossControlsDecPensioneWithDatiOpzioneByIdPensione(datiPensione, isRiaperturaDomanda, datiDanteCausa, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, areaTitolareBL.Pensione, isRiaperturaDomanda, datiGenericiAgoCi != null ? datiGenericiAgoCi.RiduzioneRetributiva : false, datiGenericiAgoCi != null ? datiGenericiAgoCi.RiduzioneRetributivaPercentuale : null, out errori))
                    {
                        errori += " Eliminare i Dati Istruttoria.";
                        return false;
                    }

                    if (!GestioneCrossControls.AGO_VerificaPerfezionamentoRequisiti_Decorrenza_ScadenzaAssegno(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria,
                        areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, datiGenericiAgoCi != null ? datiGenericiAgoCi.ScadenzaAssegno : null, out errori))
                        return false;

                    if (!GestioneCrossControls.AGO_VerificaPerfezionamentoRequisiti_Decorrenza_InvaliditaOver80(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria,
                        areaTitolareBL.Pensione.DataPerfezionamentoRequisiti, out errori))
                        return false;

                    if (!GestioneCrossControls.VerificaDecorrenzaOriginariaVESO92(datiPensione, datiPensione.DecorrenzaOriginaria, codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null,
                        datiGenericiAgoCi != null ? datiGenericiAgoCi.AnnoBancaFideiussoria : null, datiGenericiAgoCi != null ? datiGenericiAgoCi.ProgressivoBancaFideiussoria : null,
                        areaTitolareBL.Anagrafica.Cognome, areaTitolareBL.Anagrafica.CodiceFiscale, listaDecBancaFideiussione, out errori))
                        return false;

                    if (!GestioneCrossControls.AGO_ControlsRiduzioneRetributivaVOCRED(datiPensione, areaTitolareBL.Pensione.DecorrenzaOriginaria, areaTitolareBL.Anagrafica.DataNascita,
                        datiGenericiAgoCi != null ? datiGenericiAgoCi.ScadenzaAssegno : null, datiGenericiAgoCi != null ? datiGenericiAgoCi.RiduzioneRetributiva : false,
                        datiGenericiAgoCi != null ? datiGenericiAgoCi.RiduzioneRetributivaPercentuale : null, isRiaperturaDomanda, out errori))
                        return false;

                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                        if (!GestioneCrossControls.AGO_ControlsTipoBeneficioArt24Comma15Bis(datiPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, areaTitolareBL.Anagrafica.Sesso,
                            areaTitolareBL.Anagrafica.DataNascita, datiEnpals != null ? datiEnpals.AADiritto : null, datiEnpals != null ? datiEnpals.MMDiritto : null, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null,
                            datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, out errori))
                            return false;

                    //ENG - Vecchiaia in Computo
                    if (!GestioneCrossControls.VerificaPerfezionamentoRequisitiVecchiaiaInComputo(datiPensione, areaTitolareBL.Pensione, tipoAppartenenza, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_ControlsRequisitoEta(datiPensione, tipoAppartenenza, isRiaperturaDomanda, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                        areaTitolareBL.Anagrafica.Sesso, null, null, derogaTraduzioneSuGP, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, null, datiGenericiAgoCi != null ? datiGenericiAgoCi.DataAssunzioneCarico : null, out errori))
                        return false;

                    if (!GestioneCrossControls.AGO_ControlsRequisitoEtaInvaliditaOver80(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita, areaTitolareBL.Anagrafica.Sesso, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerTipoContributivo(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                        areaTitolareBL.Anagrafica.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out isWarning, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaSPED(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerVOAUT(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                        areaTitolareBL.Anagrafica.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiGenericiAgoCi != null ? datiGenericiAgoCi.DataAssunzioneCarico : null, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerAnticipoVOAUT(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                        areaTitolareBL.Anagrafica.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, null, null, true, datiGenericiAgoCi != null ? datiGenericiAgoCi.DataAssunzioneCarico : null, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaINDCOM(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica, out errori))
                        return false;

                    //if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerVOMIN(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                    //    areaTitolareBL.Anagrafica.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out errori))
                    //    return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaVecchiaiaPerPescatori(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita, areaTitolareBL.Anagrafica.Sesso,
                        datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiGenericiAgoCi != null ? datiGenericiAgoCi.DataAssunzioneCarico : null, out errori))
                        return false;

                    break;

                #endregion AGO

                #region CI
                case Utility.TipoAppartenenza.CI:

                    if (string.IsNullOrEmpty(areaTitolareBL.Anagrafica.Cittadinanza))
                    {
                        errori = "La cittadinanza è un dato obbligatorio";
                        return false;
                    }


                    // PCIPL35 - Se l'anno della DEC. ORIGINARIA PENSIONE è inferiore al 1930 o superiore all’anno corrente e DEC. ORIGINARIA PENSIONE non è uguale 
                    // al mese successivo della data corrente mostra l'errore "DECORRENZA ILLOGICA O MANCANTE".
                    if (areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Year < 1930 || areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Year > dataSistema.Year)
                    {
                        DateTime meseSuc = (new DateTime(dataSistema.Year, dataSistema.Month, 01)).AddMonths(1);
                        if (DateTime.Compare(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Date, meseSuc) != 0)
                        {
                            errori = "Decorrenza illogica o mancante";
                            return false;
                        }
                    }

                    if (!GestioneCrossControls.CI_VerificaDecorrenzaPensione(datiPensione, isRiaperturaDomanda, areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori))
                        return false;
                    // PCIPL35 - Controlla:
                    // se la sigla categoria inizia con "V" e la decorrenza pensione è successiva a 2011/01/01 allora
                    //// se la data perf. req. è antecedente al 1980 segnala data perf. req. illogica
                    //// se l'anno della data perf. req.  è <  1990 segnala errore data  perf. Req. errata; 
                    if (datiPensione.SiglaCategoria.StartsWith("V") && Utility.DataSuccessivaA(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value, new DateTime(2011, 01, 01)))
                    {
                        if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Year < 1980)
                        {
                            errori = "Decorrenza Perfezionamento Requisiti illogica";
                            return false;
                        }
                        if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value.Year < 1990)
                        {
                            errori = "Decorrenza Perfezionamento Requisiti errata";
                            return false;
                        }
                    }
                    // se data perf. req. ha valore segnala errore  che la data perfezionamento req. non va acquisita.   
                    else
                    {
                        if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue)
                        {
                            errori = "La Decorrenza Perfezionamento Requisiti non va acquisita";
                            return false;
                        }
                    }

                    if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, areaTitolareBL.Pensione, isRiaperturaDomanda, datiGenericiAgoCi != null ? datiGenericiAgoCi.RiduzioneRetributiva : false, datiGenericiAgoCi != null ? datiGenericiAgoCi.RiduzioneRetributivaPercentuale : null, out errori))
                    {
                        errori += " Eliminare i Dati Istruttoria.";
                        return false;
                    }

                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                        if (!GestioneCrossControls.CI_ControlsTipoBeneficioArt24Comma15Bis(datiPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                            datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, areaTitolareBL.Pensione.DecorrenzaOriginaria, datiPensione.NaturaPensione,
                            areaTitolareBL.Anagrafica.Sesso, areaTitolareBL.Anagrafica.DataNascita, datiIstruttoria, datiGenericiAgoCi, listaPensioniCiPrestazioniEE, out errori))
                            return false;

                    if (!GestioneCrossControls.ALL_ControlsRequisitoEta(datiPensione, tipoAppartenenza, isRiaperturaDomanda, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita, areaTitolareBL.Anagrafica.Sesso,
                        datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiIstruttoria != null ? datiIstruttoria.CodiceParticolareSoggettoDerogato : null, derogaTraduzioneSuGP,
                        datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, null, null, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerTipoContributivo(datiPensione, areaTitolareBL.Pensione, areaTitolareBL.Anagrafica.DataNascita,
                        areaTitolareBL.Anagrafica.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out isWarning, out errori))
                        return false;

                    if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolareBL, derogaTraduzioneSuGP, out errori))
                        return false;

                    if ((Utility.IsDomandaTipoContributivo(datiPensione, null, true) && !Utility.IsDomandaAutomatica(datiPensione) && !(Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                        (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) ||
                        (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)))
                        && (datiPensione.DataRaggiungimentoOpzione.HasValue && !Utility.DataSuccessivaA(datiPensione.DataRaggiungimentoOpzione.Value, new DateTime(2012, 01, 01))
                        && datiPensione.DataPerfezionamentoRequisiti.HasValue && !Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2012, 01, 01))))
                    {
                        errori = "Domanda non lavorabile in quanto la data raggiungimento dell’opzione e la data raggiungimento requisiti pensione sono precedenti al 01/01/2012. Inviare segnalazione al Supporto IVS";
                        return false;
                    }

                    if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002") && datiPensione.Tipo == "0017" && !Utility.IsDomandaAutomatica(datiPensione) && areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && !Utility.DataSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2012, 01, 01)))
                    {
                        errori = "Domanda non lavorabile in quanto la data raggiungimento requisiti pensione è precedente al 01/01/2012. Inviare segnalazione al Supporto IVS";
                        return false;
                    }
                    break;
                #endregion CI
            }

            if ((Utility.IsDomandaPSO(datiPensione.SiglaCategoria) || Utility.IsDomandaPMO(datiPensione.SiglaCategoria)))
            {
                if (areaTitolareBL.Pensione.NCertificato != null)
                {
                    if (!ControlsCertificato((int)areaTitolareBL.Pensione.NCertificato,
                        datiPensione.SiglaCategoria, datiPensione.Gruppo, out errori))
                    {
                        return false;
                    }
                }

                if (areaTitolareBL.Pensione.DecorrenzaOriginaria != null)
                {
                    if (!VerificaDecorrenzaPensionePSO_PMO(datiPensione, (DateTime)areaTitolareBL.Pensione.DecorrenzaOriginaria, out errori, isRiaperturaDomanda))
                    {
                        return false;
                    }
                }
            }

            //ENG - Memo 48_2023
            //Segnalazione 33631 spostare il controllo dal pannello "Titolare" al pannello "Liquidazione Pensione" dati generici alla selezione "Esenzione Fiscale Residente Estero".
            //if (!GestioneCrossControls.VerificaResidenzaCittadinanzaTitolareBulgaria(datiPensione, areaTitolareBL.Anagrafica, out errori))
            //    return false;

            return true;
        }

        public static bool ControlsDatiAnagraficaVOPGI_AGI(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.AreaTitolare areaTitolareBL, out string errori)
        {
            errori = string.Empty;

            if (!VerificaLeggeBilancio2024(datiPensione, areaTitolareBL, out errori))
            {
                return false;
            }

            if (!areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue)
            {
                errori = "Il campo 'Perfezionamento Requisiti' è obbligatorio";
                return false;
            }

            if (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2022, 06, 30)))
            {
                errori = "Pensione con diritto autonomo INPGI avente data perfezionamento requisiti successiva al 30 Giugno 2022 non ammissibile. In assenza di diritto autonomo, rimuovere dalla domanda la tipologia ‘AGI’ tramite procedura Webdom e riprelevare in IVS";
                return false;
            }

            //ENG - Aggiornamento Memo INPGI
            GestioneControlliDinamici.ControlloDinamico ctrlAggiornamentoMemo_INPGI_20240307 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20240307", out ctrlAggiornamentoMemo_INPGI_20240307);
            if (ctrlAggiornamentoMemo_INPGI_20240307 != null && !String.IsNullOrEmpty(ctrlAggiornamentoMemo_INPGI_20240307.ValoreControllo)
                && ctrlAggiornamentoMemo_INPGI_20240307.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if (datiPensione != null && datiPensione.DecorrenzaOriginaria.HasValue && areaTitolareBL != null && areaTitolareBL.Pensione != null && areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue)
                {
                    if (datiPensione.DecorrenzaOriginaria.Value != areaTitolareBL.Pensione.DecorrenzaOriginaria.Value)
                    {
                        if (datiPensione.Id > 0)
                        {
                            List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> listaDatiContributivi = null;
                            GestioneQuotaFondoINPGI.GetCalcoloContributivoINPGIByIdPensione(datiPensione.Id, out listaDatiContributivi);

                            List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> listaDatiRetributivi = null;
                            GestioneQuotaFondoINPGI.GetCalcoloRetributivoINPGIByIdPensione(datiPensione.Id, out listaDatiRetributivi);

                            if ((listaDatiRetributivi != null && listaDatiRetributivi.Count() > 0) || (listaDatiContributivi != null && listaDatiContributivi.Count() > 0))
                            {
                                errori = "Cancellare i Dati Calcolo Quota Fondo INPGI";
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static bool ControlsCertificato(int NCertificato, string Categoria, string gruppo, out string errori)
        {

            errori = string.Empty;
            string certificatoString = Convert.ToString(NCertificato).PadLeft(8, '0');

            if (Utility.IsDomandaPSO(Categoria) || Utility.IsDomandaPMO(Categoria))
            {
                if (NCertificato.ToString().Length > 8)
                {
                    errori = "Il numero certificato non può superare le 8 cifre";
                    return false;
                }

                if (Utility.IsDomandaPSO(Categoria))
                {
                    string certificatoSubstring = certificatoString.Substring(0, 3);
                    if (certificatoSubstring == "092" || certificatoSubstring == "098" || certificatoSubstring == "095")
                    {
                        errori = "Attenzione, il certificato inserito non permette di associare l’ente";
                        return false;
                    }

                    if (NCertificato <= 9100000)
                    {
                        errori = "Il numero certificato di una domanda PSO deve essere strettamente maggiore 09100000";
                        return false;
                    }
                }

                if (Utility.IsDomandaPMO(Categoria))
                {

                    if (gruppo == "0003")
                    {
                        if (certificatoString[2] != '3' && certificatoString[2] != '6')
                        {
                            errori = "Una pensione superstiti deve avere il terzo numero del certificato uguale a 3 o 6";
                            return false;
                        }
                    }

                    if (gruppo == "0002")
                    {
                        if (certificatoString[2] != '2' && certificatoString[2] != '5')
                        {
                            errori = "Una pensione di invalidità deve avere il terzo numero del certificato uguale a 3 o 6";
                            return false;
                        }
                    }

                    if (NCertificato < 100001 || NCertificato > 699999)
                    {
                        errori = "Il numero certificato di una domanda PMO deve essere un numero compreso tra 00100001 e 00699999";
                        return false;
                    }
                }

            }
            return true;
        }

        private static bool VerificaDecorrenzaPensionePSO_PMO(GestionePensione.DatiPensione datiPensione, DateTime DecorrenzaPensione, out string errori, bool isRiaperturaDomanda)
        {
            errori = string.Empty;
            DateTime PSOMin = new DateTime(1976, 1, 1, 0, 0, 0);
            DateTime PMOMax = new DateTime(1965, 2, 1, 0, 0, 0);

            if (Utility.IsDomandaPSO(datiPensione.SiglaCategoria) && !Utility.DataSuccessivaA(DecorrenzaPensione, PSOMin))
            {
                errori = "La decorrenza pensione deve essere maggiore o uguale al 01/1976";
                return false;
            }

            if (Utility.IsDomandaPMO(datiPensione.SiglaCategoria) && Utility.DataSuccessivaA(DecorrenzaPensione, PMOMax) && !(datiPensione.Gruppo == "0003" || Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)))
            {
                errori = "La decorrenza pensione deve essere strettamente inferiore al 02/1965";
                return false;
            }
            return true;
        }

        private static bool CrossControlsWithDanteCausa(long idPensione, Utility.TipoDomanda tipoDomanda, BLCommon.Entity.AreaTitolare areaTitolareBL, out string errori)
        {
            errori = string.Empty;
            if (tipoDomanda == Utility.TipoDomanda.Superstiti && areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue)
            {
                BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(idPensione, out datiDanteCausa);
                if (datiDanteCausa != null)
                {
                    if (datiDanteCausa.DecorrenzaPensione.HasValue)
                    {
                        if (DateTime.Compare(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Date, datiDanteCausa.DecorrenzaPensione.Value.Date) <= 0)
                        {
                            errori = "La decorrenza della pensione deve essere maggiore della decorrenza pensione del dante causa";
                            return false;
                        }
                    }
                    if (datiDanteCausa.DataMorte.HasValue)
                    {
                        if (DateTime.Compare(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Date, datiDanteCausa.DataMorte.Value.Date) <= 0)
                        {
                            errori = "La decorrenza della pensione deve essere maggiore della data di morte del dante causa";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static bool ControlsAggiornamentoQuadroRedditiByIdPensione(GestionePensione.DatiPensione datiPensione, short? sedeDestinazione, short? sedeDestinazioneActual,
            out GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi)
        {
            datiQuadroRedditi = null;
            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out datiQuadroRedditi);
            //in caso di redditi già acquisiti e sede destinazione discordante tra app e DB
            if (datiQuadroRedditi != null && datiQuadroRedditi.TabRedditi.HasValue && datiQuadroRedditi.TabRedditi.Value == 2 &&
                ((sedeDestinazioneActual.HasValue && sedeDestinazione.HasValue &&
                sedeDestinazioneActual.Value != sedeDestinazione.Value) ||
                (sedeDestinazioneActual.HasValue && !sedeDestinazione.HasValue) ||
                (!sedeDestinazioneActual.HasValue && sedeDestinazione.HasValue)))
                return true;

            return false;
        }

        private static void ControlsAggiornamentoQuadroLiquidazione(GestionePensione.DatiPensione datiPensione, byte? scaltaLavoratriceMadreSalvatiDB, out GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazione)
        {
            datiQuadroLiquidazione = null;
            DateTime dataCompare = new DateTime(1980, 01, 01);

            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazione);
            if (datiQuadroLiquidazione != null)
            {
                if (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value >= dataCompare)
                    datiQuadroLiquidazione.TabOpzione = null;
                else
                    datiQuadroLiquidazione.TabOpzione = 1;

                //ENG - Per le domande 0001/0002/0017 della linea AGO il quadro Liquidazione Pensione - Tab Generici deve diventare rosso se elimini/inserisci la scelta lavoratrice madre
                if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0017")
                {
                    if (datiQuadroLiquidazione != null)
                    {
                        if ((scaltaLavoratriceMadreSalvatiDB.HasValue && !datiPensione.SceltaLavMadri.HasValue) || (!scaltaLavoratriceMadreSalvatiDB.HasValue && datiPensione.SceltaLavMadri.HasValue))
                        {
                            datiQuadroLiquidazione.TabDatiGenerici = 0;
                        }
                    }
                }
            }
        }

        private static void ControlsAggiornamentoQuadroLiquidazione_memo28_2024(GestionePensione.DatiPensione datiPensione, out GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazione)
        {
            datiQuadroLiquidazione = null;

            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiquidazione);
            if (datiQuadroLiquidazione != null)
            {
                datiQuadroLiquidazione.TabDatiGenerici = 0;
            }
        }

        private static bool CrossControlsDecPensioneWithDatiOpzioneByIdPensione(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string errori)
        {
            errori = string.Empty;

            if ((Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                (Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaESPA(datiPensione.SiglaCategoria))) || Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) != null)
                return true;

            DateTime dataCompare = new DateTime(1980, 01, 01);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            if (datiIstruttoria != null && (datiIstruttoria.DataDomandaOpzione != null || datiIstruttoria.DecorrenzaOpzione != null) && (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value >= dataCompare))
            {
                errori = "Eliminare i 'Dati Opzione' in 'Liquidazione Pensione' prima di procedere con il salvataggio";
                return false;
            }
            return true;
        }

        private static bool CrossControlsFSWithRequisiti247_243ByIdPensione(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, DateTime? DataPerfRequisiti, char? derogaTraduzioneSuGP, out string msgVideo)
        {
            msgVideo = string.Empty;
            bool? Requisiti247_243 = null;
            byte? NumeroTriSemRequisiti = null;
            short? AnnoRequisiti = null;
            int? AnzianitaAnni = null;

            if (!DataPerfRequisiti.HasValue || !Utility.DataSuccessivaA(DataPerfRequisiti.Value, new DateTime(2011, 01, 01)))
                return true;

            // Per le domande di salvaguardia L.122 dei fondi EL, TT, ET, GAS, DZ, ES, PM, PI, VL devono essere presenti i campi Numero Trimestre e Anzianità Anni 
            if (Utility.IsDomandaSalvaguardia122_FS_2011_2012(datiPensione, derogaTraduzioneSuGP))
                return true;

            switch (tipoFondo)
            {
                case Utility.TipoFondo.EL:
                    GestioneFondo.DatiFondoEL datiFondoEL = null;
                    GestioneFondo.GetFondoELByIdPensione(datiPensione.Id, out datiFondoEL);

                    Requisiti247_243 = datiFondoEL != null ? datiFondoEL.Requisiti247_243 : null;
                    NumeroTriSemRequisiti = datiFondoEL != null ? datiFondoEL.NumeroTriSemRequisiti : null;
                    AnnoRequisiti = datiFondoEL != null ? datiFondoEL.AnnoRequisiti : null;
                    AnzianitaAnni = datiFondoEL != null ? datiFondoEL.AnzianitaAnni : null;
                    break;
                case Utility.TipoFondo.TT:
                    GestioneFondo.DatiFondoTT datiFondoTT = null;
                    GestioneFondo.GetFondoTTByIdPensione(datiPensione.Id, out datiFondoTT);

                    Requisiti247_243 = datiFondoTT != null ? datiFondoTT.Requisiti247_243 : null;
                    NumeroTriSemRequisiti = datiFondoTT != null ? datiFondoTT.NumeroTriSemRequisiti : null;
                    AnnoRequisiti = datiFondoTT != null ? datiFondoTT.AnnoRequisiti : null;
                    AnzianitaAnni = datiFondoTT != null ? datiFondoTT.AnzianitaAnni : null;
                    break;
                case Utility.TipoFondo.ET:
                    GestioneFondo.DatiFondoET datiFondoET = null;
                    GestioneFondo.GetFondoETByIdPensione(datiPensione.Id, out datiFondoET);

                    Requisiti247_243 = datiFondoET != null ? datiFondoET.Requisiti247_243 : null;
                    NumeroTriSemRequisiti = datiFondoET != null ? datiFondoET.NumeroTriSemRequisiti : null;
                    AnnoRequisiti = datiFondoET != null ? datiFondoET.AnnoRequisiti : null;
                    AnzianitaAnni = datiFondoET != null ? datiFondoET.AnzianitaAnni : null;
                    break;
                case Utility.TipoFondo.VL:
                    GestioneFondo.DatiFondoVL datiFondoVL = null;
                    GestioneFondo.GetFondoVLByIdPensione(datiPensione.Id, out datiFondoVL);

                    Requisiti247_243 = datiFondoVL != null ? datiFondoVL.Requisiti247_243 : null;
                    NumeroTriSemRequisiti = datiFondoVL != null ? datiFondoVL.NumeroTriSemRequisiti : null;
                    AnnoRequisiti = datiFondoVL != null ? datiFondoVL.AnnoRequisiti : null;
                    AnzianitaAnni = datiFondoVL != null ? datiFondoVL.AnzianitaAnni : null;
                    break;
                case Utility.TipoFondo.PT:
                    GestioneFondo.DatiFondoPT datiFondoPT = null;
                    GestioneFondo.GetFondoPTByIdPensione(datiPensione.Id, out datiFondoPT);

                    Requisiti247_243 = datiFondoPT != null ? datiFondoPT.RequisitiAnte247 : null;
                    NumeroTriSemRequisiti = datiFondoPT != null ? datiFondoPT.TrimesteRequisiti : null;
                    AnnoRequisiti = datiFondoPT != null ? datiFondoPT.AnnoRequisiti : null;
                    AnzianitaAnni = datiFondoPT != null ? datiFondoPT.AnzianitaAnni : null;
                    break;
                case Utility.TipoFondo.FS:
                    GestioneFondo.DatiFondoFST datiFondoFS = null;
                    GestioneFondo.GetFondoFSTByIdPensione(datiPensione.Id, out datiFondoFS);

                    Requisiti247_243 = datiFondoFS != null ? datiFondoFS.RequisitiAnte247 : null;
                    NumeroTriSemRequisiti = datiFondoFS != null ? datiFondoFS.TrimesteRequisiti : null;
                    AnnoRequisiti = datiFondoFS != null ? datiFondoFS.AnnoRequisiti : null;
                    AnzianitaAnni = datiFondoFS != null ? datiFondoFS.AnzianitaAnni : null;
                    break;
                case Utility.TipoFondo.PI:
                case Utility.TipoFondo.PL:
                    List<GestioneFondo.DatiFondoPI> datiFondoPI = null;
                    GestioneFondo.GetFondoPIRecordFondoByIdPensione(datiPensione.Id, out datiFondoPI);

                    Requisiti247_243 = datiFondoPI != null ? datiFondoPI.FirstOrDefault().Requisiti247_243 : null;
                    NumeroTriSemRequisiti = datiFondoPI != null ? datiFondoPI.FirstOrDefault().NumeroTriSemRequisiti : null;
                    AnnoRequisiti = datiFondoPI != null ? datiFondoPI.FirstOrDefault().AnnoRequisiti : null;
                    AnzianitaAnni = datiFondoPI != null ? datiFondoPI.FirstOrDefault().AnzianitaAnni : null;
                    break;
            }

            if (Requisiti247_243.HasValue || NumeroTriSemRequisiti.HasValue || AnnoRequisiti.HasValue || AnzianitaAnni.HasValue)
            {
                msgVideo = "'Data Perfezionamento Requisiti' incompatibile con i requisiti 'L.247' o 'L.243'; Eliminare i dati Generici prima di proseguire.";
                return false;
            }

            return true;
        }

        public static void GetElencoSindacatiForCategoria_Codice(GestionePensione.DatiPensione datiPensione, string CodiceSindacato, out List<Liquidazione.BLCommon.Entity.Sindacato> ElencoSindacati, out string errori)
        {
            errori = string.Empty;
            string IdCategoria = string.Empty;
            ElencoSindacati = null;
            string SiglaCategoria = datiPensione != null ? datiPensione.SiglaCategoria : string.Empty;
            IdCategoria = BLCommon.GestioneSindacati.GetIdCategoriaForSindacato(SiglaCategoria, out errori);
            if (!String.IsNullOrEmpty(errori))
            {
                errori = "Ricerca Sindacati, errore" + errori;
                ElencoSindacati = null;
                return;
            }

            // Per la categoria APE e SPED non sono presenti sindacati, quindi non richiamo il servizio
            if (IdCategoria == "143" || IdCategoria == "010" || IdCategoria == "011" || IdCategoria == "012" || Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) ||
                Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) || Utility.IsRenditaFacoltativa(datiPensione) || Utility.IsRenditaCasalinghe(datiPensione) ||
                Utility.IsDomandaVOST(SiglaCategoria))
                return;

            GestioneDelegheSindacali.GetElencoSindacatiPerCategoria(IdCategoria, out ElencoSindacati, out errori);  // tutti i sindacati dal ws

            //Per le categorie ESOTEL, ESOAMB e ESPA se non sono presenti sindacati non va mostrato l'errore
            if ((IdCategoria == "196" || IdCategoria == "197" || IdCategoria == "200") && (ElencoSindacati == null || ElencoSindacati.Count == 0))
            {
                errori = String.Empty;
                return;
            }

            if (!String.IsNullOrEmpty(errori))
            {
                errori = "Ricerca Sindacati, errore" + errori;
                ElencoSindacati = null;
                return;
            }

            if (ElencoSindacati.Count == 0)
            {
                errori = "Ricerca Sindacati: Per la categoria '" + SiglaCategoria.Trim() + "' non sono presenti Sindacati";
                ElencoSindacati = null;
                return;
            }

            ElencoSindacati = BLCommon.GestioneSindacati.GetElencoSindacatiAttivi(ElencoSindacati, out errori); // solo i sidacati attivi

            //Per le categorie ESOTEL, ESOAMB e ESPA se non sono presenti sindacati attivi non va mostrato l'errore
            if ((IdCategoria == "196" || IdCategoria == "197" || IdCategoria == "200") && ElencoSindacati.Count == 0)
            {
                errori = String.Empty;
                return;
            }

            if (!String.IsNullOrEmpty(errori))
            {
                errori = "Ricerca Sindacati, errore" + errori;
                ElencoSindacati = null;
                return;
            }

            if (!String.IsNullOrEmpty(CodiceSindacato) && Utility.IsSindacatoPresente(CodiceSindacato)) // Sindacato salvato a DB
            {
                int index = ElencoSindacati.FindIndex(x => x.Id == CodiceSindacato.Trim());
                if (index < 0)                          // non è piu attivo il sindacato a db
                {
                    BLCommon.Entity.Sindacato sindacato = null;
                    GestioneDelegheSindacali.DecodificaCodiceSindacato(CodiceSindacato.Trim(), out sindacato, out errori);
                    if (!String.IsNullOrEmpty(errori))  //gestione del sindacato non attivo in presenza di errori
                    {
                        errori = "Il Sindacato (codice " + CodiceSindacato.Trim() + ") non è più attivo; " + errori.ToUpperInvariant();
                        sindacato = new Liquidazione.BLCommon.Entity.Sindacato();
                        sindacato.Id = CodiceSindacato.Trim();
                        sindacato.Descrizione = "NON ATTIVO";
                    }
                    sindacato.Stato = Utility.StatoSindacato.Cessato;

                    ElencoSindacati.Add(sindacato);
                }
            }
        }

        public static void ControlsDatiAnagraficaDopoAggiornaARCA(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, Utility.TipoAppartenenza? tipoApp, string codResidenzaARCA,
            string codResidenzaDB, bool? ResidenzaEsteroARCA, bool? ResidenzaEsteroDB, DateTime? dataMorteTitolare, bool isRiaperturaDomanda, DateTime dataSistema,
            out Dictionary<Utility.TabAggArca, byte?> semafori)
        {
            semafori = new Dictionary<Utility.TabAggArca, byte?>();
            bool isEsenzioneFiscaleEsteroDB;
            bool isEsenzioneFiscaleEsteroARCA;
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                isEsenzioneFiscaleEsteroDB = Utility.IsEsenzioneFiscaleEsteroINPDAP(codResidenzaDB);
                isEsenzioneFiscaleEsteroARCA = Utility.IsEsenzioneFiscaleEsteroINPDAP(codResidenzaARCA);
            }
            else
            {
                isEsenzioneFiscaleEsteroDB = Utility.IsEsenzioneFiscaleEstero(datiPensione, codResidenzaDB, datiDetrazioni, isRiaperturaDomanda);
                isEsenzioneFiscaleEsteroARCA = Utility.IsEsenzioneFiscaleEstero(datiPensione, codResidenzaARCA, datiDetrazioni, isRiaperturaDomanda);
            }

            if (ResidenzaEsteroARCA != ResidenzaEsteroDB)
                semafori.Add(Utility.TabAggArca.Anagrafica, 0);

            // Per le ricostituzioni il semaforo non deve variare
            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
            {
                if (tipoApp.HasValue && (tipoApp.Value == Utility.TipoAppartenenza.AGO || tipoApp.Value == Utility.TipoAppartenenza.CI))
                {
                    if (dataMorteTitolare.HasValue && datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(dataMorteTitolare.Value, datiPensione.DecorrenzaOriginaria.Value) ||
                        (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) && datiPensioniDatiGenerici != null && datiPensioniDatiGenerici.ScadenzaAssegno.HasValue &&
                               Utility.DataSuccessivaA(Utility.FirstDayOfMonth(dataSistema), Utility.FirstDayOfMonth(datiPensioniDatiGenerici.ScadenzaAssegno.Value))))
                        semafori.Add(Utility.TabAggArca.Eliminazione, 0);
                    else
                        semafori.Add(Utility.TabAggArca.Eliminazione, 1);
                }
            }

            if (!string.IsNullOrEmpty(codResidenzaDB) && !string.IsNullOrEmpty(codResidenzaARCA))
            {
                if (codResidenzaARCA != codResidenzaDB && (codResidenzaARCA.StartsWith("Z") || codResidenzaDB.StartsWith("Z")))
                {
                    if (!semafori.ContainsKey(Utility.TabAggArca.Anagrafica))
                        semafori.Add(Utility.TabAggArca.Anagrafica, 0);
                }

                //Stato precedente: Italia; Stato nuovo: Italia. --> Nessuna modifica
                if (!Utility.IsResidenteEstero(codResidenzaDB) && !Utility.IsResidenteEstero(codResidenzaARCA))
                    return;

                //Stato precedente uguale allo Stato nuovo --> Nessuna modifica
                if (codResidenzaARCA == codResidenzaDB)
                    return;

                //Stato precedente: Estero con la possibilità di avere l’esenzione; Stato nuovo: Estero con la possibilità di avere l’esenzione.
                //Stato precedente: Estero senza la possibilità di avere l’esenzione; Stato nuovo: Estero senza la possibilità di avere l’esenzione.
                //Stato precedente: Italia; Stato nuovo: Estero con la possibilità di avere l’esenzione.
                //Stato precedente: Italia; Stato nuovo: Estero senza la possibilità di avere l’esenzione.
                if ((Utility.IsResidenteEstero(codResidenzaDB) && Utility.IsResidenteEstero(codResidenzaARCA)) ||
                    (!Utility.IsResidenteEstero(codResidenzaDB) && Utility.IsResidenteEstero(codResidenzaARCA)))
                {
                    semafori.Add(Utility.TabAggArca.ResidenzaEstero, 0);

                    //20150626 - Per VESO33 e VESO92 non ci sta il quadro reddito quindi non si deve impostare a rosso.
                    if (!Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || !Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                        semafori.Add(Utility.TabAggArca.Redditi, 0);

                    //Stato precedente: Estero con la possibilità di avere l’esenzione; Stato nuovo: Estero senza la possibilità di avere l’esenzione.
                    if (isEsenzioneFiscaleEsteroDB && !isEsenzioneFiscaleEsteroARCA)
                        GestioneSemaforiEsenzioneFiscale(tipoApp, datiPensione, isRiaperturaDomanda, datiIstruttoria, ref semafori);
                }

                //Stato precedente: Estero con la possibilità di avere l’esenzione; Stato nuovo: Italia.
                //Stato precedente: Estero senza la possibilità di avere l’esenzione; Stato nuovo: Italia.
                if (Utility.IsResidenteEstero(codResidenzaDB) && !Utility.IsResidenteEstero(codResidenzaARCA))
                {
                    //20150626 - Per VESO33 e VESO92 non ci sta il quadro reddito quindi non si deve impostare a rosso.
                    if (!Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) && !Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                        semafori.Add(Utility.TabAggArca.Redditi, 0);

                    List<GestioneAnagrafica.DatiResidenzaEstero> elencoResidenzaEstero = null;
                    GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out elencoResidenzaEstero);

                    if (tipoApp.HasValue)
                    {
                        switch (tipoApp.Value)
                        {
                            case Utility.TipoAppartenenza.FS:
                                semafori.Add(Utility.TabAggArca.ResidenzaEstero, null);
                                break;
                            case Utility.TipoAppartenenza.AGO:
                            case Utility.TipoAppartenenza.CI:
                                if (elencoResidenzaEstero != null && elencoResidenzaEstero.Count > 0)
                                    semafori.Add(Utility.TabAggArca.ResidenzaEstero, 0);
                                else
                                    semafori.Add(Utility.TabAggArca.ResidenzaEstero, 1);
                                break;
                        }
                    }

                    if (isEsenzioneFiscaleEsteroDB)
                        GestioneSemaforiEsenzioneFiscale(tipoApp, datiPensione, isRiaperturaDomanda, datiIstruttoria, ref semafori);
                }
            }
            else
                if (codResidenzaARCA != codResidenzaDB)
                    if (!semafori.ContainsKey(Utility.TabAggArca.Anagrafica))
                        semafori.Add(Utility.TabAggArca.Anagrafica, 0);
        }

        private static void GestioneSemaforiEsenzioneFiscale(Utility.TipoAppartenenza? tipoApp, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref Dictionary<Utility.TabAggArca, byte?> semafori)
        {
            //id 2: esenzione fiscale estera
            if (datiIstruttoria != null && datiIstruttoria.CodiceComunicazioneCampo4.GetValueOrDefault() == 2)
            {
                semafori.Add(Utility.TabAggArca.EsenzioneFiscale, null);
                semafori.Add(Utility.TabAggArca.DatiGenerici, 0);
                if (!((tipoApp == Utility.TipoAppartenenza.FS || tipoApp == Utility.TipoAppartenenza.AGO) &&
                    (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda ||
                    //si passano i datiBeneficioTerrorismo a null in quanto viene già verificata la condizione di essere una ricostituzione
                    Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null) ||
                    Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria))))
                    semafori.Add(Utility.TabAggArca.Detrazioni, 0);
            }
            //id 1: esenzione fiscale vittima terrorismo. Nessuna azione richiesta
        }

        //ENG - Blocco per Legge Bilancio 2024
        private static bool VerificaLeggeBilancio2024(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.AreaTitolare areaTitolareBL, out string errore)
        {
            errore = string.Empty;
            GestioneControlliDinamici.ControlloDinamico ctrlLeggeBilancio2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("BLOCCO_DPR_MAGG_122023_LEGGEBILANCIO_2024", out ctrlLeggeBilancio2024);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (ctrlLeggeBilancio2024 != null && !String.IsNullOrEmpty(ctrlLeggeBilancio2024.ValoreControllo) && !String.IsNullOrEmpty(ctrlLeggeBilancio2024.ValoreControllo.Trim())
                && ctrlLeggeBilancio2024.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017")
                    || (tipoAppartenenza == Utility.TipoAppartenenza.AGO && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0045" && !String.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta) && datiPensione.CodiceTipoRichiesta.Trim().ToUpperInvariant() == "AV"))
                {
                    if (areaTitolareBL != null && areaTitolareBL.Pensione != null && (areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.HasValue
                        && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2023, 12, 31))) ||
                        (areaTitolareBL.Pensione.DecorrenzaOriginaria.HasValue && Utility.DataStrettamenteSuccessivaA(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value, new DateTime(2024, 01, 01))))
                    {
                        errore = "La liquidazione di tale tipologia di prestazione, nelle more dell’implementazione delle novità previste dalla legge di bilancio 2024, è momentaneamente inibita.";
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion Anagrafica

        #region StatoCivile

        public static bool SalvaStatoCivile(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.AreaTitolare areaTitolareBL, bool IsSingleTabSaved, DateTime dataSistema, bool isRiaperturaDomanda,
            out string errori)
        {
            errori = string.Empty;
            try
            {
                GestioneAnagrafica.DatiAnagrafici Anagrafica;
                GestioneAnagrafica.GetAnagraficaByCodiceFiscale(areaTitolareBL.Anagrafica.CodiceFiscale, out Anagrafica);

                GestioneAnagrafica.DatiAnagrafici anagraficaDC = null;
                BLCommon.GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out anagraficaDC);

                if (datiPensione == null)
                    return false;

                GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
                GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);

                GestioneAnagrafica.DatiStatoCivile statoCivileLatest;
                GestioneAnagrafica.GetLatestStatoCivileById(Anagrafica.Id, datiPensione.Id, out statoCivileLatest);

                GestioneQuadri.DatiQuadroFamiliari datiQuadroFamiliari = null;
                GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out datiQuadroFamiliari);

                List<GestioneFamiliari.Familiare> listaFamiliari = null;
                List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
                GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagrafiche);

                List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
                GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaCodMaggFamiliari);

                GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

                //ENG - Spacchettate SOPGI
                BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);


                ControlsStatoCivile(areaTitolareBL, datiPensione, listaFamiliari, listaAnagrafiche, listaCodMaggFamiliari, Anagrafica.DataNascita, anagraficaDC != null ? anagraficaDC.DataMatrimonio : null,
       Anagrafica.Sesso, anagraficaDC != null ? anagraficaDC.Sesso : null, IsSingleTabSaved, Anagrafica.CodiceFiscale, dataSistema, out errori);
                bool IsFamiliariValid = true;
                try
                {
                    if (listaFamiliari != null && listaFamiliari.Count > 0)
                    {
                        string messaggioInfo = string.Empty;
                        GestioneFamiliari.ValidateFamiliari(datiPensione, isRiaperturaDomanda, string.Empty, listaFamiliari, listaCodMaggFamiliari, areaTitolareBL, null, controlloDinamicoSpacchettate024, out messaggioInfo);
                    }
                }
                catch (INPS.DNA.DnaValidationException)
                {
                    IsFamiliariValid = false;
                }
                if (String.IsNullOrEmpty(errori))
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        GestioneAnagrafica.EliminaStatiCivili(Anagrafica.Id, datiPensione.Id);

                        if (areaTitolareBL.ElencoStatiCivili != null && areaTitolareBL.ElencoStatiCivili.Count > 0)
                        {
                            foreach (GestioneAnagrafica.DatiStatoCivile statoCivile in areaTitolareBL.ElencoStatiCivili)
                            {
                                GestioneAnagrafica.SalvaStatoCivile(Anagrafica.Id, datiPensione.Id, statoCivile);
                                if (statoCivileLatest == null || !statoCivileLatest.Decorrenza.HasValue || statoCivile.Decorrenza.Value.CompareTo(statoCivileLatest.Decorrenza) > 0)
                                    statoCivileLatest = statoCivile;
                            }
                            Anagrafica.CodiceStatoCivile = statoCivileLatest.Codice;
                            Anagrafica.DecorrenzaStatoCivile = statoCivileLatest.Decorrenza;
                            GestioneAnagrafica.SalvaAnagrafica(Anagrafica);
                        }

                        GestioneQuadri.GestioneSemaforoQuadroTitolare(datiPensione, true, false, false, false, false, areaTitolareBL, 0, null, ref datiQuadroTitolare);

                        if (!Utility.IsDomandaSpacchettamentoENPALS(datiPensione) && !Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                            && !(controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda))
                            && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa) && !Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda)
                            && !Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                            AggiornaLegameStatoCivileFamiliari(datiPensione.Id, ref areaTitolareBL, ref datiQuadroFamiliari, listaFamiliari, IsFamiliariValid,
                                Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione),
                                   Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria), danteCausa, isRiaperturaDomanda);

                        string siglaCategoria = string.Empty;
                        string certificato = string.Empty;
                        siglaCategoria = datiPensione.SiglaCategoria.Trim().ToUpperInvariant();
                        certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "00000000";

                        if (siglaCategoria == "PSO" && ((Utility.IsDomandaPL(datiPensione) && (datiPensione.Gruppo == "0005" && datiPensione.Prodotto == "0043" && (datiPensione.Tipo == "0014" || datiPensione.Tipo == "0015"))) || (Utility.IsRicostituzione(datiPensione.Gruppo) && (certificato.Substring(2, 1) == "3" || certificato.Substring(2, 1) == "6"))))
                        {

                            if (datiQuadroFamiliari != null)
                            {
                                datiQuadroFamiliari.Tipo = null;
                                datiQuadroFamiliari.TabFamiliari = null;
                                GestioneQuadri.SalvaQuadroFamiliari(datiPensione.Id, datiQuadroFamiliari);

                            }
                        }

                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool DeleteStatoCivileByDatiPensione(GestionePensione.DatiPensione datiPensione, out string errori)
        {
            errori = string.Empty;
            try
            {
                BLCommon.Entity.AreaTitolare areaTitolareBL = null;
                GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolareBL);

                long idAnagrafica = 0;
                GestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(areaTitolareBL.Anagrafica.CodiceFiscale, out idAnagrafica);

                GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
                Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    GestioneAnagrafica.EliminaStatiCivili(idAnagrafica, datiPensione.Id);

                    GestioneQuadri.GestioneSemaforoQuadroTitolare(datiPensione, false, true, false, false, false, areaTitolareBL, 0, null, ref datiQuadroTitolare);

                    transactionScope.Complete();
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool ControlsStatoCivile(BLCommon.Entity.AreaTitolare areaTitolareBL, GestionePensione.DatiPensione datiPensione, List<GestioneFamiliari.Familiare> listaFamiliari,
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheFamiliari, List<GestioneFamiliari.CodMaggFamiliari> elencoCodMaggFamiliari, DateTime? dataNascita, DateTime? dataMatrimonioDC,
            char? sessoTitolare, char? sessoDanteCausa, bool IsSingleTabSaved, string codiceFiscaleTitolare, DateTime dataSistema, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            List<GestioneDecodifica.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392 = null;
            GestioneDecodifica.GetCodiceRequisitiLegge50392(out listaCodiceRequisitiLegge50392);

            char? codiceRequisitiLegge50392TraduzioneSuGP = null;
            if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.HasValue && listaCodiceRequisitiLegge50392 != null && listaCodiceRequisitiLegge50392.Count > 0)
            {
                GestioneDecodifica.CodiceRequisitiLegge50392 appCodiceRequisitiLegge50392 = listaCodiceRequisitiLegge50392.Find(x => x.Id == datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2.ToString());
                codiceRequisitiLegge50392TraduzioneSuGP = appCodiceRequisitiLegge50392 != null ? appCodiceRequisitiLegge50392.TraduzioneSuGP : null;
            }

            //Reimposto la decorrenza pensione nel formato MM/AAAA (per i fondi FS e PT la decorrenza pensione è nel formato GG/MM/AAAA). I controlli tra decorrenza pensione e stato civile rimangono invariati
            DateTime? decPensione = null;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (IsSingleTabSaved)
                decPensione = datiPensione.DecorrenzaOriginaria.HasValue ? new DateTime(datiPensione.DecorrenzaOriginaria.Value.Year, datiPensione.DecorrenzaOriginaria.Value.Month, 1) : (DateTime?)null;
            else
                decPensione = Utility.DataFromInt(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Year, areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Month, 1);

            if (!GestioneCrossControls.ALL_VerificaStatiCivili(decPensione, datiPensione, areaTitolareBL.ElencoStatiCivili, listaFamiliari, listaAnagraficheFamiliari, elencoCodMaggFamiliari, dataNascita,
                dataMatrimonioDC, sessoTitolare, sessoDanteCausa, codiceFiscaleTitolare, dataSistema, out messaggioVideo))
                return false;

            if (tipoAppartenenza == Utility.TipoAppartenenza.CI)
            {
                if (!GestioneCrossControls.CI_VerificaCodiceRequisitiLegge50392WithStatoCivile(codiceRequisitiLegge50392TraduzioneSuGP, areaTitolareBL.ElencoStatiCivili, out messaggioVideo))
                    return false;
            }

            //controlli sulle unioni civili
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCivili(areaTitolareBL.ElencoStatiCivili, datiPensione, out messaggioVideo))
                return false;

            return true;
        }

        #endregion StatoCivile

        #region ResidenzeEstere

        public static bool SalvaResidenzeEstereByDatiPensione(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.AreaTitolare areaTitolareBL, bool IsSingleTabSaved, out string errori)
        {
            errori = string.Empty;
            try
            {
                long idAnagrafica = 0;
                GestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(areaTitolareBL.Anagrafica.CodiceFiscale, out idAnagrafica);

                if (datiPensione == null)
                    return false;

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
                GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);

                if (datiQuadroTitolare.TabResidenzeEstero != null)
                {
                    bool isErroreObbligatorio = false;
                    ControlsResidenzeEstere(areaTitolareBL, datiPensione, IsSingleTabSaved, out isErroreObbligatorio, out errori);
                    if (String.IsNullOrEmpty(errori))
                    {
                        using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                        {
                            GestioneAnagrafica.EliminaResidenzeEstero(idAnagrafica, datiPensione.Id);
                            if (areaTitolareBL.ElencoResidenzeEstere != null && areaTitolareBL.ElencoResidenzeEstere.Count > 0)
                            {
                                foreach (GestioneAnagrafica.DatiResidenzaEstero residenzaEstero in areaTitolareBL.ElencoResidenzeEstere)
                                    GestioneAnagrafica.SalvaResidenzaEstero(idAnagrafica, datiPensione.Id, residenzaEstero);
                            }
                            GestioneQuadri.GestioneSemaforoQuadroTitolare(datiPensione, false, false, true, false, false, areaTitolareBL, idAnagrafica, tipoAppartenenza, ref datiQuadroTitolare);
                            transactionScope.Complete();
                        }
                    }
                    else if (isErroreObbligatorio)
                    {
                        using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                        {
                            datiQuadroTitolare.TabResidenzeEstero = 0;
                            GestioneQuadri.SalvaQuadroTitolare(datiPensione.Id, datiQuadroTitolare);
                            transactionScope.Complete();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool DeleteResidenzeEstereByDatiPensione(GestionePensione.DatiPensione datiPensione, out string errori)
        {
            errori = string.Empty;
            try
            {
                BLCommon.Entity.AreaTitolare areaTitolareBL = null;
                GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolareBL);

                long idAnagrafica = 0;
                GestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(areaTitolareBL.Anagrafica.CodiceFiscale, out idAnagrafica);

                GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
                Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    GestioneAnagrafica.EliminaResidenzeEstero(idAnagrafica, datiPensione.Id);

                    GestioneQuadri.GestioneSemaforoQuadroTitolare(datiPensione, false, false, false, true, false, areaTitolareBL, idAnagrafica, null, ref datiQuadroTitolare);

                    transactionScope.Complete();
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool ControlsResidenzeEstere(BLCommon.Entity.AreaTitolare areaTitolareBL, GestionePensione.DatiPensione datiPensione, bool IsSingleTabSaved, out bool isErroreObbligatorio, out string messaggioVideo)
        {
            isErroreObbligatorio = false;
            messaggioVideo = string.Empty;
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione));

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);


            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (areaTitolareBL.ElencoResidenzeEstere != null && areaTitolareBL.ElencoResidenzeEstere.Count > 0)
            {
                areaTitolareBL.ElencoResidenzeEstere.Sort(delegate(GestioneAnagrafica.DatiResidenzaEstero c1, GestioneAnagrafica.DatiResidenzaEstero c2) { return c1.Decorrenza.Value.CompareTo(c2.Decorrenza); });

                //Reimposto la decorrenza pensione nel formato MM/AAAA (per i fondi FS e PT la decorrenza pensione è nel formato GG/MM/AAAA). I controlli tra decorrenza pensione e stato residenze estere rimangono invariati
                DateTime? DecPensione = null;
                if (IsSingleTabSaved)
                    DecPensione = datiPensione.DecorrenzaOriginaria.HasValue ? new DateTime(datiPensione.DecorrenzaOriginaria.Value.Year, datiPensione.DecorrenzaOriginaria.Value.Month, 1) : (DateTime?)null;
                else
                    DecPensione = Utility.DataFromInt(areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Year, areaTitolareBL.Pensione.DecorrenzaOriginaria.Value.Month, 1);

                if (!DecPensione.HasValue)
                {
                    messaggioVideo = "Salvare la Decorrenza Pensione prima di procedere con il salvataggio delle Residenze Estere";
                    return false;
                }

                // decorrenza pensione maggiore della data odierna
                if (DecPensione.Value.CompareTo(new DateTime(dataSistema.Year, dataSistema.Month, 1)) >= 0)
                {
                    if (areaTitolareBL.ElencoResidenzeEstere.Count > 1)
                    {
                        messaggioVideo = "Con Decorrenza Pensione successiva alla data odierna (MM/AAAA) è possibile inserire un'unica residenza estera.";
                        return false;
                    }

                    if (String.IsNullOrEmpty(areaTitolareBL.ElencoResidenzeEstere[0].CodCatastaleStatoEE) || !areaTitolareBL.ElencoResidenzeEstere[0].Decorrenza.HasValue)
                    {
                        messaggioVideo = "Decorrenza e/o Residenza Estera mancanti.";
                        return false;
                    }

                    if (areaTitolareBL.ElencoResidenzeEstere.First().Decorrenza.Value.CompareTo(DecPensione.Value) != 0)
                    {
                        string strDecorrenza = "decorrenza della pensione";
                        if (Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) ||
                            Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) ||
                            Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria))
                            strDecorrenza = "decorrenza dell'assegno";

                        messaggioVideo = string.Format("La decorrenza della residenza estera inserita non coincide con la decorrenza della pensione.", strDecorrenza);
                        return false;
                    }

                    if (!GestioneCrossControls.ALL_VerificaResidenzeEstereWithAnagrafica(areaTitolareBL.Anagrafica, areaTitolareBL.ElencoResidenzeEstere, out messaggioVideo))
                    {
                        if (Utility.IsRicostituzione(datiPensione.Gruppo))
                            isErroreObbligatorio = true;
                        return false;
                    }

                    if (areaTitolareBL.ElencoResidenzeEstere.Last().CodCatastaleStatoEE.ToUpperInvariant().Trim() == "Z000") //Italia
                    {
                        //messaggioVideo = "Lo Stato Estero di residenza corrispondente alla decorrenza più recente non coincide con quello presente nell'anagrafica.";
                        messaggioVideo = "Impossibile salvare come unico stato estero di residenza 'Italia'";
                        return false;
                    }
                }
                else // decorrenza pensione minore uguale della data odierna
                {
                    if (!areaTitolareBL.ElencoResidenzeEstere.First().Decorrenza.HasValue || areaTitolareBL.ElencoResidenzeEstere.First().Decorrenza.Value.CompareTo(DecPensione.Value) != 0)
                    {
                        messaggioVideo = "La decorrenza più remota delle residenze estere inserite non coincide con la decorrenza della pensione.";
                        return false;
                    }

                    int index = 0;
                    foreach (GestioneAnagrafica.DatiResidenzaEstero residenzaEstera in areaTitolareBL.ElencoResidenzeEstere)
                    {
                        if (!residenzaEstera.Decorrenza.HasValue || String.IsNullOrEmpty(residenzaEstera.CodCatastaleStatoEE))
                        {
                            messaggioVideo = "Decorrenza e Residenza Estera obbligatorie.";
                            return false;
                        }

                        if (IsDecorrenzaDuplicataRE(areaTitolareBL.ElencoResidenzeEstere) || residenzaEstera.Decorrenza.Value.CompareTo(DecPensione.Value) < 0)
                        {
                            messaggioVideo = "Le decorrenza delle residenze esteri devono essere sempre successive.";
                            return false;
                        }

                        if (((index + 1) != areaTitolareBL.ElencoResidenzeEstere.Count) && residenzaEstera.CodCatastaleStatoEE.Trim() == areaTitolareBL.ElencoResidenzeEstere[index + 1].CodCatastaleStatoEE.Trim())
                        {
                            messaggioVideo = "Residenza Estera successive non possono essere uguali.";
                            return false;
                        }
                        index++;
                    }

                    if (IsDecorrenzaFuturaRE(areaTitolareBL.ElencoResidenzeEstere, dataSistema))
                    {
                        messaggioVideo = "Non possono essere presenti decorrenze successive alla data odierna(MM/AAAA).";
                        return false;
                    }

                    if (String.IsNullOrEmpty(areaTitolareBL.ElencoResidenzeEstere.Last().CodCatastaleStatoEE))
                    {
                        messaggioVideo = "L'ultimo Stato Estero di residenza corrispondente alla decorrenza più recente è obbligatorio.";
                        return false;
                    }

                    if (!GestioneCrossControls.ALL_VerificaResidenzeEstereWithAnagrafica(areaTitolareBL.Anagrafica, areaTitolareBL.ElencoResidenzeEstere, out messaggioVideo))
                    {
                        if (Utility.IsRicostituzione(datiPensione.Gruppo))
                            isErroreObbligatorio = true;
                        return false;
                    }

                    if (areaTitolareBL.ElencoResidenzeEstere.Last().CodCatastaleStatoEE.ToUpperInvariant().Trim() == "Z000")
                    {
                        //messaggioVideo = "Lo Stato Estero di residenza corrispondente alla decorrenza più recente non coincide con quello presente nell'anagrafica.";
                        if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && areaTitolareBL.ElencoResidenzeEstere.Count == 1)
                        {
                            messaggioVideo = "Impossibile salvare come unico stato estero di residenza 'Italia'";
                            return false;
                        }
                    }
                }

                if (tipoAppartenenza.HasValue && tipoAppartenenza == Utility.TipoAppartenenza.CI)
                {
                    if (!GestioneCrossControls.CI_VerificaResidenzaWithCodOpzione(datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null, areaTitolareBL.ElencoResidenzeEstere.First().CodCatastaleStatoEE))
                    {
                        messaggioVideo = "Residenza alla Decorrenza Originaria deve essere Italia se Codice Opzione è uguale a 7";
                        return false;
                    }
                }

            }
            return true;
        }

        private static Boolean IsDecorrenzaDuplicataRE(List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere)
        {
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere2 = new List<GestioneAnagrafica.DatiResidenzaEstero>();
            listaResidenzeEstere2 = listaResidenzeEstere.FindAll(delegate(GestioneAnagrafica.DatiResidenzaEstero re1)
            {
                return listaResidenzeEstere.FindAll(delegate(GestioneAnagrafica.DatiResidenzaEstero re2)
                { return re1.Decorrenza == re2.Decorrenza; }).Count > 1;
            }).Distinct().ToList();

            if (listaResidenzeEstere2.Count > 0)
                return true;
            else
                return false;
        }

        private static bool IsDecorrenzaFuturaRE(List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere, DateTime dataSistema)
        {
            DateTime now = new DateTime(dataSistema.Year, dataSistema.Month, 1);

            int index = listaResidenzeEstere.FindIndex(delegate(GestioneAnagrafica.DatiResidenzaEstero re)
            {
                return (DateTime.Compare(re.Decorrenza.Value, now) > 0);
            });

            if (index >= 0)
                return true;

            return false;
        }

        #endregion ResidenzeEstere

        #region private members
        private static void AggiornaLegameStatoCivileFamiliari(long idPensione, ref BLCommon.Entity.AreaTitolare areaTitolareBL,
            ref GestioneQuadri.DatiQuadroFamiliari datiQuadroFamiliari, List<GestioneFamiliari.Familiare> listaFamiliari, bool IsFamiliariValid, Utility.TipoDomanda tipoDomanda, GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoFondo? tipoFondo, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa, bool isRiaperturaDomanda)
        {
            if (areaTitolareBL.ElencoStatiCivili != null && areaTitolareBL.ElencoStatiCivili.Count > 0)
            {
                //areaTitolareBL.ElencoStatiCivili = areaTitolareBL.ElencoStatiCivili.OrderBy(x => x.Decorrenza).ToList<GestioneAnagrafica.DatiStatoCivile>();
                //if (areaTitolareBL.ElencoStatiCivili[areaTitolareBL.ElencoStatiCivili.Count - 1].Codice == 2)
                if ((areaTitolareBL.ElencoStatiCivili.FindAll(x => x.Codice == '2' || x.Codice == '7').Count != 0)
                    || (tipoDomanda == Utility.TipoDomanda.Superstiti && areaTitolareBL.ElencoStatiCivili.Count == 1 && areaTitolareBL.ElencoStatiCivili.FindAll(x => x.Codice == '3' || x.Codice == 'C').Count != 0))
                {
                    if (Utility.IsDomandaAnte96(datiPensione, areaTitolareBL.Pensione, danteCausa, isRiaperturaDomanda) == null)
                    {


                        datiQuadroFamiliari.Tipo = 2;
                        if (datiQuadroFamiliari.TabFamiliari == 1)
                            datiQuadroFamiliari.TabFamiliari = 0;
                        else if (datiQuadroFamiliari.TabFamiliari == 2)
                        {
                            bool presenzaConiuge = false;
                            foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                            {
                                if (fam.Confermato && fam.IsConiugeOrUnitoCivile())
                                {
                                    presenzaConiuge = true;
                                    break;
                                }
                            }
                            if (!presenzaConiuge)
                                datiQuadroFamiliari.TabFamiliari = 0;
                        }
                        GestioneQuadri.SalvaQuadroFamiliari(idPensione, datiQuadroFamiliari);
                    }
                }
                else
                {
                    if (tipoDomanda != Utility.TipoDomanda.Superstiti && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti)
                    {
                        datiQuadroFamiliari.Tipo = 1;
                        if (datiQuadroFamiliari.TabFamiliari == 0)
                        {
                            if (tipoDomanda != Utility.TipoDomanda.Ripristino)
                            {
                                if (listaFamiliari != null && listaFamiliari.Count > 0)
                                {
                                    if (!IsFamiliariValid || listaFamiliari.FindIndex(x => !x.Confermato) > -1)
                                    {
                                        datiQuadroFamiliari.Tipo = 2;
                                        datiQuadroFamiliari.TabFamiliari = 0;
                                    }
                                    else
                                        datiQuadroFamiliari.TabFamiliari = 2;
                                }
                                else
                                    datiQuadroFamiliari.TabFamiliari = 1;
                            }
                            else
                            {
                                if (listaFamiliari != null && listaFamiliari.Count > 0 && !(listaFamiliari.Count == 1 && listaFamiliari.FirstOrDefault().FlagTitolare == true))
                                {
                                    if (!IsFamiliariValid || listaFamiliari.FindIndex(x => !x.Confermato) > -1)
                                    {
                                        datiQuadroFamiliari.Tipo = 2;
                                        datiQuadroFamiliari.TabFamiliari = 0;
                                    }
                                    else
                                        datiQuadroFamiliari.TabFamiliari = 2;
                                }
                                else
                                    datiQuadroFamiliari.TabFamiliari = 1;
                            }

                        }
                        //nel caso di familiari prepopolati
                        else if (datiQuadroFamiliari.TabFamiliari == 1)
                        {
                            if (listaFamiliari != null && listaFamiliari.Count > 0 && !(listaFamiliari.Count == 1 && listaFamiliari.FirstOrDefault().FlagTitolare == true))
                            {
                                datiQuadroFamiliari.Tipo = 2;
                                datiQuadroFamiliari.TabFamiliari = 0;
                            }
                        }

                        GestioneQuadri.SalvaQuadroFamiliari(idPensione, datiQuadroFamiliari);

                    }
                    else
                    {
                        if (listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.FindIndex(x => !x.Confermato) == -1)
                        {
                            datiQuadroFamiliari.Tipo = 2;
                            datiQuadroFamiliari.TabFamiliari = 2;
                        }
                        else
                        {
                            datiQuadroFamiliari.Tipo = 2;
                            datiQuadroFamiliari.TabFamiliari = 0;
                        }
                        GestioneQuadri.SalvaQuadroFamiliari(idPensione, datiQuadroFamiliari);
                    }
                }
            }
            else
            {
                if (datiQuadroFamiliari.Tipo != 1)
                {
                    datiQuadroFamiliari.Tipo = 1;
                    if (datiQuadroFamiliari.TabFamiliari == 0)
                    {
                        if (listaFamiliari != null && listaFamiliari.Count > 0)
                        {
                            if (!IsFamiliariValid || listaFamiliari.FindIndex(x => !x.Confermato) > -1)
                            {
                                datiQuadroFamiliari.Tipo = 2;
                                datiQuadroFamiliari.TabFamiliari = 0;
                            }
                            else
                                datiQuadroFamiliari.TabFamiliari = 2;
                        }
                        else
                            datiQuadroFamiliari.TabFamiliari = 1;
                    }
                    GestioneQuadri.SalvaQuadroFamiliari(idPensione, datiQuadroFamiliari);
                }
                //nel caso di familiari prepopolati
                else
                {
                    if (datiQuadroFamiliari.TabFamiliari == 1)
                    {
                        if (listaFamiliari != null && listaFamiliari.Count > 0)
                        {
                            datiQuadroFamiliari.Tipo = 2;
                            datiQuadroFamiliari.TabFamiliari = 0;
                        }
                    }
                    GestioneQuadri.SalvaQuadroFamiliari(idPensione, datiQuadroFamiliari);
                }
            }
        }
        #endregion  private members

        #region Cross Properties
        public static Dictionary<string, bool> GetCrossProperties(GestionePensione.DatiPensione datiPensione)
        {
            bool IsEnteIstruttoreFondoExINPDAP;
            bool IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024 = false;

            Dictionary<string, bool> lReturn = new Dictionary<string, bool>();
            IsEnteIstruttoreFondoExINPDAP = CheckEnteIstruttoreFondoExINPDAP(datiPensione);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            if (Utility.IsDomandaVOAUT(datiPensione.SiglaCategoria) && datiPensione.IdTipoPLPerRIC.HasValue && datiPensione.IdTipoPLPerRIC.Value == 21 &&
                datiPensioniDatiGenerici != null && datiPensioniDatiGenerici.DataAssunzioneCarico.HasValue && !Utility.DataStrettamenteSuccessivaA(datiPensioniDatiGenerici.DataAssunzioneCarico.Value, new DateTime(2024, 04, 01)) &&
                datiPensione.NumeroFigli.HasValue && datiPensione.SceltaLavMadri.HasValue)
                IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024 = true;

            lReturn.Add("IsEnteIstruttoreFondoExINPDAP", IsEnteIstruttoreFondoExINPDAP);
            lReturn.Add("IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024", IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024);

            return lReturn;
        }

        private static bool CheckEnteIstruttoreFondoExINPDAP(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);
                if (datiGenerici != null && datiGenerici.EnteIstruttoreExInpdap.GetValueOrDefault())
                    return true;
            }

            return false;
        }


        #endregion Cross Properties
    }
}

