using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneAgo.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneLiquidazionePensione
    {
        public static void GetLiquidazionePensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            bool isRiaperturaDomanda, out DatiGenerici datiTabGenerici, out DatiAssicurativi datiTabAssicurativi, out DatiIstruttoria datiTabIstruttoria, out DatiOpzione datiTabOpzione,
            out DatiProvenienza datiTabProvenienza, out DatiInail datiTabInail, out EntityBLCommon.DatiContribuzioneEnpals datiContribuzioneEnpals, out DatiSentenzaArt4 datiSentenzaArt4,
            out DatiLiquidazionePensioneStorico datiLiquidazionePensioneStorico, out DatiSentenze datiSentenze, out List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, out List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi)
        {
            datiTabGenerici = null;
            string messaggioVideo = string.Empty;

            GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiTabGenerici, out messaggioVideo);

            datiTabAssicurativi = null;
            GetDatiAssicurativi(ref contenitore, out datiTabAssicurativi, out messaggioVideo);

            datiTabIstruttoria = null;
            GetDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, out datiTabIstruttoria);

            datiTabOpzione = null;
            ValorizzaDatiOpzione(ref contenitore, out datiTabOpzione);
            datiTabProvenienza = null;
            ValorizzaDatiProvenienza(contenitore.DatiIstruttoria, out datiTabProvenienza);

            datiTabInail = null;
            GetDatiInailByIdPensione(ref contenitore, out datiTabInail);

            datiContribuzioneEnpals = null;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                GestioneContribuzioneEnpals.GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(contenitore.DatiPensione.Id, TipologiaContribuzioneEnpals.SAI, out datiContribuzioneEnpals);

            datiLiquidazionePensioneStorico = null;
            GetDatiLiquidazionePensioneStorico(ref contenitore, out datiLiquidazionePensioneStorico);

            datiSentenzaArt4 = null;
            GetDatiSentenzaArt4(ref contenitore, out datiSentenzaArt4);

            datiSentenze = null;
            GetDatiSentenze(ref contenitore, out datiSentenze);

            listaDatiContributivi = null;
            GetDatiCalcoloContributivo(ref contenitore, out listaDatiContributivi);

            listaDatiRetributivi = null;
            GetDatiCalcoloRetributivo(ref contenitore, out listaDatiRetributivi);
        }

        #region dati Generici
        public static bool ControlDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, bool IsSingleTab,
            DatiGenerici datiGenerici, DatiAssicurativi datiAssicurativi, DatiProvenienza datiProvenienza, DatiIstruttoria datiIstruttoria, DatiExCombattente datiExCombattente,
            DatiBenefici datiBenefici, DatiMaggiorazioni datiMaggiorazioni, DateTime dataSistema, int annoCompetenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneDanteCausa.DatiDanteCausa datiDA = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                datiDA = contenitore.DatiDanteCausa;

            if (IsSingleTab)
            {
                GetDatiAssicurativi(ref contenitore, out datiAssicurativi, out messaggioVideo);
                if (!string.IsNullOrEmpty(messaggioVideo))
                    return false;
                ValorizzaDatiProvenienza(contenitore.DatiIstruttoria, out datiProvenienza);
                GetDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, out datiIstruttoria);
            }
            else
            {
                if (datiIstruttoria != null && datiIstruttoria.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoria.IsDatiIstruttoriaPensioneNull() &&
                    datiIstruttoria.IsDatiIstruttoriaDatiGenericiNull() && datiIstruttoria.IsDatiIstruttoriaMaggiorazioneBeneficiNull())
                    datiIstruttoria = null;
            }

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);

            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            if (datiGenerici == null)
                return true;

            if (Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria))
            {
                if (datiGenerici.DataCompletezza.HasValue)
                {
                    messaggioVideo = "Campo 'Data Completezza' non deve essere valorizzato";
                    return false;
                }
            }
            else if (!Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria)
                && !Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria)
                && !Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria)
                && !Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault()
                && !Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria)
                && !datiGenerici.DataCompletezza.HasValue)
            {
                messaggioVideo = "Campo 'Data Completezza' obbligatorio";
                return false;
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!datiGenerici.Contributivo.HasValue)
                {
                    messaggioVideo = "Campo 'Contributivo' obbligatorio";
                    return false;
                }
            }
            else if (!Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) && !Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) && !Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) &&
                !Utility.IsDomandaSPED(contenitore.DatiPensione) &&
                     !(Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") &&
                     !(Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") &&
                     !Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) &&
                     !Utility.IsIsoPensioneRicWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null)
                     && !Utility.IsDomandaINDCOM(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria)
                     && !(Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione)) &&
                     !Utility.IsDomandaESPA_L26(contenitore.DatiPensione) && !Utility.IsRenditaCasalinghe(contenitore.DatiPensione) && !Utility.IsRenditaFacoltativa(contenitore.DatiPensione) &&
                     !Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) && !Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria) &&
                     !Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa) && !(Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) != Utility.TipoUnicarpe.Automatica && Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.ListaDatiContributivi == null && contenitore.ListaDatiRetributivi == null)
                     && !Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione))
            {
                if (!datiGenerici.TipoCalcolo.HasValue)
                {
                    messaggioVideo = "Campo 'Tipo Calcolo' obbligatorio";
                    return false;
                }
            }

            if (!GestioneControlli.ControlsCodiciNatura(contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiGenerici.NaturaPensione, datiGenerici.CodiceDomandaRicorso,
                datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : null, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : null,
                datiDA, datiGenerici.TipoCalcolo, datiGenerici.Contributivo, contenitore.DatiPensione, contenitore.DatiLavorazione, contenitore.DatiPensioniDatiGenerici,
                datiAssicurativi != null ? datiAssicurativi.NSettimaneOBG : null, isRiaperturaDomanda, contenitore.ListaDatiSupplementi,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.TipoPensioneExInpdai : null, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceArretrati(datiGenerici.CodiceArretrati, contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.CodiceMotivo : null, contenitore.DatiPensione,
                isRiaperturaDomanda, datiGenerici.TipoCumulo, datiGenerici.CumuloEsterno, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDecorrenzaArretrati(datiGenerici.DecorrenzaCalcoloArretrati, datiGenerici.DataInizioCalcolo, contenitore.DatiPensione, datiGenerici.CausaCarico, annoCompetenza,
            isRiaperturaDomanda, datiIstruttoria != null ? datiIstruttoria.CodiceBancaEsodati : (short?)null, contenitore.DatiDanteCausa, out messaggioVideo))
                return false;

            if (!Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDataCompletezza(datiGenerici.DataCompletezza, contenitore.DatiPensione, dataSistema, isRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsDataInteressiLegali(datiGenerici.DataInteressiLegali, datiGenerici.DataCompletezza,
                    datiGenerici.CausaCarico, datiGenerici.CodiceDomandaRicorso, datiGenerici.DataInizioCalcolo, contenitore.DatiPensione, isRiaperturaDomanda, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.ControlsCausaCarico(contenitore.DatiPensione, datiGenerici.CausaCarico, datiGenerici.CodiceDomandaRicorso, datiGenerici.DataInizioCalcolo, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDataRipristino(datiGenerici.DataInizioCalcolo, datiGenerici.CausaCarico, contenitore.DatiPensione, dataSistema, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceLiquidazione(datiGenerici.CodiceLiquidazione,
                datiDA != null ? datiDA.DecorrenzaPensione : null, contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceMobilita(datiGenerici.CodiceMobilita, datiGenerici.NaturaPensione,
                datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, contenitore.DatiPensione, out messaggioVideo))
                return false;

            if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) &&
                !(Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") &&
                !(Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") &&
                !Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) && !Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) &&
                !Utility.IsDomandaSPED(contenitore.DatiPensione) &&
                !Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) &&
                !Utility.IsIsoPensioneRicWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null) &&
                !Utility.IsDomandaINDCOM(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) &&
                !(Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione)) &&
                !Utility.IsDomandaESPA_L26(contenitore.DatiPensione) && !Utility.IsRenditaCasalinghe(contenitore.DatiPensione) && !Utility.IsRenditaFacoltativa(contenitore.DatiPensione) &&
                !Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) && !Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria) &&
                (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria))))
                if (!GestioneControlli.ControlsTipoCalcolo(contenitore.DatiPensione, contenitore.DatiStoricoGP, contenitore.DatiDanteCausa, datiGenerici.TipoCalcolo, isRiaperturaDomanda, out messaggioVideo))
                    return false;

            if (!GestioneControlli.ControlsScadenzaRevSan(datiGenerici.ScadenzaRevisioneSanitaria, datiAssicurativi != null ? datiAssicurativi.InizioAssicurazione : null,
                contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.DecorrenzaEliminazione : null, contenitore.DatiPensione,
                contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Sesso : null, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.DataNascita : null,
                dataSistema, isRiaperturaDomanda, contenitoreDecodifica.ElencoCtrlScadenzaIndennizzoINDCOM, contenitore.DatiEliminazione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsConfermaInvalidita(datiGenerici.NRiconoscimentiInvalidita, datiGenerici.NaturaPensione, contenitore.DatiPensione, dataSistema, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsTrattenutaINPDAP(datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap,
                contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.DecorrenzaEliminazione : null, contenitore.DatiPensione,
                contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsProvvisoriaWithDataMorteTitolare(contenitore.DatiPensione, datiGenerici.FlagProvvisoria, contenitore.DatiAnagraficiTitolare.DataMorte, out messaggioVideo))
                return false;

            DateTime? inizioBonus = Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) ? (datiAssicurativi != null ? datiAssicurativi.InizioBonus : null) : (contenitore.DatiControlloFelpe != null ? contenitore.DatiControlloFelpe.InizioBonus : null);
            if (!GestioneControlli.ControlsFlagProvvisoria(datiGenerici.FlagProvvisoria, contenitore.DatiPensione, annoCompetenza, isRiaperturaDomanda, inizioBonus, out messaggioVideo))
                return false;

            if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiGenerici.ExCombattente, datiGenerici.Benefici, datiGenerici.Maggiorazioni, false,
                datiExCombattente, datiBenefici, datiMaggiorazioni, out messaggioVideo))
                return false;

            //if (!ControlsDatiGenericiForPensioneProvenienza(numeroDomanda, datiGenerici.TrasformazioneAOI, false))
            //{
            //    messaggioVideo = "Eliminare i dati Pensione di Provenienza prima di procedere con il salvataggio dei dati Generici";
            //    return false;
            //}

            if (!ControlsDatiGenericiForBititolaritaAltraPensioneByIdPensione(contenitore.ListaAltraPensione, datiGenerici.NaturaPensione, false))
            {
                messaggioVideo = "Eliminare i dati 'Altra Pensione' nel quadro 'Bititolarità' prima di procedere";
                return false;
            }

            if (!ControlsDatiGenericiForIstruttoria(datiGenerici.NaturaPensione, contenitore.DatiPensione.SiglaCategoria, datiIstruttoria != null ? datiIstruttoria.CodiceBancaEsodati : null,
                datiIstruttoria != null ? datiIstruttoria.Attivitausuranti : null, false, contenitore.DatiPensione, out messaggioVideo))
                return false;

            DateTime? fineAssicurazione = datiAssicurativi != null && datiAssicurativi.FineAssicurazione.HasValue ? datiAssicurativi.FineAssicurazione.Value : (DateTime?)null;

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) == null)
                if (!GestioneCrossControls.AGO_ControlsTipoCalcoloForDatiContributivi(datiGenerici.TipoCalcolo, fineAssicurazione, contenitore.DatiPensione, contenitore.ListaDatiContributivi,
                    contenitore.ListaDatiRetributivi, Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione), false))
                {
                    messaggioVideo = "I dati calcolo salvati sono incongruenti con il 'Tipo Calcolo'; cancellare i dati calcolo prima di proseguire.";
                    return false;
                }


            //if (datiGenerici.TipoCalcolo.HasValue && datiIstruttoria != null)
            //{
            //    /////// IMPORTANTE: Per ENPALS bypassiamo il controllo perchè manca ancora l'analisi della tab istruttoria
            //    if (!Utility.IsDomandaENPALS(datiPensione.Gestione))
            //        if (!GestioneCrossControls.AGO_CI_ControlsRiduzioneRetributiva(datiGenerici.TipoCalcolo, datiIstruttoria.RiduzioneRetributiva, datiIstruttoria.RiduzioneRetributivaPercentuale, datiPensione, out messaggioVideo))
            //            return false;
            //}

            #region Gestione visibilità tabs MaggiorazioneBenefici

            if (!ControlsDatiGenericiAssicurativiForDatiPrepensionamento(contenitore.DatiPensione, contenitore.DatiMaggiorazioniBenefici, contenitore.DatiPrepensionamento,
                datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : null, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : null, datiGenerici.NaturaPensione, out messaggioVideo))
                return false;

            #endregion Gestione visibilità tabs MaggiorazioneBenefici

            if (!GestioneCrossControls.ALL_VerificaIncongruenzaEsenzioneFiscaleToDB(contenitore.DatiPensione,
                contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza : string.Empty, contenitore.DatiDetrazioni, isRiaperturaDomanda,
                datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsEsenzioneFiscaleVittimaTerrorismo(contenitore.DatiPensione, isRiaperturaDomanda, datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsTipoBeneficioWithCodNatura(datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : string.Empty, datiGenerici.NaturaPensione, false, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsConfermaInvalidita(contenitore.DatiPensione, contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.DataEvento : null,
                datiGenerici.NRiconoscimentiInvalidita, dataSistema, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsIndiretteInabilita(contenitore.DatiPensione, contenitore.DatiDanteCausa, datiGenerici.NaturaPensione, datiGenerici.Benefici, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsTipoBeneficiForPensioneInabilitaIndiretta(datiGenerici.GetCodNatura1(), datiGenerici.GetCodNatura3(), contenitore.DatiPensione, isRiaperturaDomanda,
                datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsTipoBeneficiForCumulo(datiGenerici.GetCodNatura1(), contenitore.DatiPensione, isRiaperturaDomanda,
                datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsTipoBeneficiForPensioneAUT(datiGenerici.GetCodNatura1(), contenitore.DatiPensione, isRiaperturaDomanda,
               datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                return false;

            if (contenitore.DatiEliminazione != null && !GestioneControlli.ControlsDecorArretratiWithDecorEliminazione(datiGenerici.DecorrenzaCalcoloArretrati,
                contenitore.DatiEliminazione.DecorrenzaEliminazione, out messaggioVideo))
                return false;


            #region Cumulo L.228/2012

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDatiGenericiObbligatoriPerCumulo(datiGenerici.EnteCassa, datiGenerici.EnteIstruttoreExInpdap, out messaggioVideo))
                    return false;
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsEnteCassaPerCumulo(contenitore.DatiPensione, datiGenerici.EnteCassa, datiGenerici.EnteIstruttoreExInpdap, datiGenerici.TipoCumulo, out messaggioVideo))
                    return false;
                if (!GestioneControlli.ControlsDecorrenzaPensioneEnteIstruttorePerCumulo(contenitore.DatiPensione, datiGenerici.EnteIstruttoreExInpdap, isRiaperturaDomanda, out messaggioVideo))
                    return false;
            }

            #endregion Cumulo L.228/2012

            if (Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsEnteCassaTOT(contenitore.DatiPensione, datiGenerici.EnteCassa, out messaggioVideo, contenitoreDecodifica != null ? contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale : null))
                    return false;
            }

            if (!GestioneControlli.ControlsCodiceRequisitiRidotti(datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiGenerici.CodiceMobilita, datiGenerici.NaturaPensione, contenitore.DatiPensione,
                contenitore.TipoCalcolo, contenitore.DatiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!((Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo))
                && Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)))
                if (!GestioneControlli.VerificaCoerenzaTipoCalcoloConTerrorismo(datiGenerici.TipoCalcolo, contenitore.ListaDatiCalcoloVittimeTerrorismo, out messaggioVideo))
                    return false;

            if (!GestioneControlli.ControlsNaturaPensionePerIDAI(contenitore.DatiPensione, contenitore.ListaDatiRetributivi, contenitore.ListaDatiContributivi, datiGenerici.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaArretratiConDataAssunzioneCaricoDAI(contenitore.DatiPensione, isRiaperturaDomanda, datiGenerici.DecorrenzaCalcoloArretrati,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCumuloEsterno(contenitore.DatiPensione, datiGenerici.CumuloEsterno, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCoerenzaResidenteEstero(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, datiGenerici.CodiceComunicazioneCampo4, contenitore.DatiDetrazioniStorico,
                isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsEsenzioneResidenzaEstera(contenitore.DatiPensione, datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(contenitore.DatiPensione, datiGenerici.NaturaPensione, datiAssicurativi != null ? datiAssicurativi.InizioAssicurazione : null,
                out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaBeneficiPerOpzioneTipoContributivo(contenitore.DatiPensione, datiGenerici.Benefici, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_CI_ControlsEsenzioneFiscaleDoppiaImposizione(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza : string.Empty,
                isRiaperturaDomanda, datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerVOMIN(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.DataNascita : null,
                        contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Sesso : null, datiGenerici.NaturaPensione, out messaggioVideo))
                return false;

            if (Utility.IsBonusBooking(contenitore.DatiPensione) && contenitore.DatiPensione.Tipo != "0167" && datiGenerici.IsRichiestaBonus.GetValueOrDefault())
            {
                if (!GestioneCrossControls.ALL_VerificaAnnoRichiestaBonus154(contenitore.DatiPensione, datiGenerici.AnnoDecorrenzaBonus, out messaggioVideo))
                    return false;
            }

            //ENG - Memo 48_2023
            //Segnalazione 33631 spostare il controllo dal pannello "Titolare" al pannello "Liquidazione Pensione" dati generici alla selezione "Esenzione Fiscale Residente Estero".
            if (datiGenerici.CodiceComunicazioneCampo4 != null && datiGenerici.CodiceComunicazioneCampo4 == 2)
            {
                if (!GestioneCrossControls.VerificaResidenzaCittadinanzaTitolareBulgaria(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Cittadinanza : null, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza : null, out messaggioVideo))
                    return false;
            }

            //ENG - VOPGI NO AGI
            if (!GestioneControlli.ControllaInizioAssicurazioneTipoCalcoloVOPGINOAGI(contenitore.DatiPensione, datiGenerici != null ? datiGenerici.TipoCalcolo : null, datiAssicurativi != null ? datiAssicurativi.InizioAssicurazione : null, out messaggioVideo))
                return false;

            return true;
        }

        public static void StoreDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiGenerici datiGenerici,
            DatiAssicurativi datiAssicurativi, DatiExCombattente datiExCombattente, DatiBenefici datiBenefici, DatiMaggiorazioni datiMaggiorazioni, DatiIstruttoria datiIstruttoria, DateTime dataSistema,
            Utility.TipoCalcolo tipoCalcoloDB, Utility.TipoCalcolo tipoCalcoloView, bool isSingleTab, bool IsCancelOperation)
        {
            string msg;

            if (datiGenerici == null)
                datiGenerici = new DatiGenerici();

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneEnpals.DatiEnpals datiEnpals = null;
            if (Utility.IsDomandaENPALS(datiPensione.Gestione))
                datiEnpals = contenitore.DatiEnpals;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = contenitore.DatiIstruttoria;
            GestionePagamento.DatiPagamento datiPagamento = contenitore.DatiPagamento;
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = contenitore.DatiNuoveLiquidate;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = contenitore.DatiPensioniDatiGenerici;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = contenitore.DatiQuadroEliminazione;
            GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni = contenitore.DatiQuadroDetrazioni;
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = contenitore.DatiQuadroSupplementi;
            GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = contenitore.DatiQuadroRedditi;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            GestioneQuadri.DatiQuadroFamiliari datiQuadroFamiliari = contenitore.DatiQuadroFamiliari;
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
            GestioneQuadri.DatiQuadroBititolarita datiQuadroBititolarita = null;
            GestioneQuadri.DatiQuadroRichiestaBonus datiQuadroRichiestaBonus = contenitore.DatiQuadroRichiestaBonus;
            if (!((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && (datiPensione.Tipo == "0009" || datiPensione.Tipo == "0192")) ||
                (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0009")))
                datiQuadroBititolarita = contenitore.DatiQuadroBititolarita;
            GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = contenitore.DatiQuadroTitolare;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = contenitore.DatiAnagraficiTitolare;
            List<GestioneOneri.DatiOneri> lstDatiOneri = contenitore.ListaDatiOneri;
            GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = contenitore.DatiPrepensionamento;
            List<EntityBLCommon.DatiSupplementi> listaSupplementi = contenitore.ListaDatiSupplementi;
            EntityBLCommon.SupplementiBase datiSupplBase = contenitore.DatiSupplementiBase;
            GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntArt11 = contenitore.DatiIntegrazioneArt11;
            List<GestioneFamiliari.Familiare> listaFamiliari = contenitore.ListaFamiliari;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = contenitore.ListaAnagraficaFamiliari;
            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = contenitore.ListaCodMaggFamiliari;
            //ENG - Integrazione Modifiche Accenture
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = contenitore.DatiMaggiorazioniBenefici;
            //----------------------------------------------------------------
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;


            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            long? soggettoBeneficiario = datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.SoggettoBeneficiario : null;
            long? tipologiaPrestazione = datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaPrestazione : null;
            long? tipologiaBeneficio = datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaBeneficio : null;

            bool isEliminazioneRossoPerConfermaInvalidita = !GestioneCrossControls.AGO_CI_ControlsEliminazioneConfermaInvalidita(datiPensione,
                (datiEliminazione != null) ? datiEliminazione.DataEvento : null, datiGenerici.NRiconoscimentiInvalidita, dataSistema, isRiaperturaDomanda, out msg);

            bool isDatiCalcoloVittimeRosso =
                Utility.IsDatiRetributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcoloDB) != Utility.IsDatiRetributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcoloView) ||
                Utility.IsDatiContributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcoloDB, contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Exists(x => x.IsQuotaDL214Presente())) != Utility.IsDatiContributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcoloView, contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Exists(x => x.IsQuotaDL214Presente()));

            bool isDatiCalcoloVittimeNonVisibile = !Utility.IsDatiRetributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcoloView) &&
                                                    !Utility.IsDatiContributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcoloView, contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Exists(x => x.IsQuotaDL214Presente())) &&
                                                    !Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiario, tipologiaPrestazione, tipologiaBeneficio);

            // Se è cambiata la visibilità oppure se adesso deve essere visualizzato ma il quadro non è visibile
            bool genericiChangedPerOneri = Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) ^
                                     Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri) ||
                                     (Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri) &&
                                     contenitore.DatiQuadroOneri != null && (contenitore.DatiQuadroOneri.TabOneri == null || contenitore.DatiQuadroOneri.Tipo == 0)) || Utility.IsDomandaVecchiaiaENAV(datiPensione);
            bool recordOneriChanged = false;

            // Per il cumulo se passo da contributivo SI a contributivo NO o viceversa, devo aggiornare il semaforo dei redditi
            // Se passo da o passo al TipoCalcolo contributivo devo aggiornare il semaforo dei redditi
            // Quindi se il vecchio è diverso dal nuovo e uno dei due è Contributivo (valore 1)
            bool aggiornaRedditi = Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) ? datiGenerici.Contributivo != datiPensione.Contributivo :
                                    datiGenerici.TipoCalcolo != datiPensione.TipoCalcolo && (datiGenerici.TipoCalcolo == 1 || datiPensione.TipoCalcolo == 1);

            bool isVariaDetrazioni = Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsEsenzioneFiscaleEsteroFromDetrazioni(datiPensione, contenitore.DatiDetrazioni, isRiaperturaDomanda).GetValueOrDefault() &&
                !Utility.IsEsenzioneFiscaleEsteroAutonomi(datiPensione, datiAnagraficiTitolare != null ? datiAnagraficiTitolare.CodiceComuneResidenza : null);

            bool isVittimeTerrorismoUnderOver80 = Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo);

            if (isSingleTab)
            {
                GetDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, out datiIstruttoria);
            }

            List<GestioneDecodifica.GruppoOneri> decGruppoOnere = null;
            List<GestioneDecodifica.SottoGruppoOneri> decSottoGruppoOneri = null;
            if (genericiChangedPerOneri)
            {
                decGruppoOnere = contenitoreDecodifica.ElencoDecCodeGruppoOnere;
                decSottoGruppoOneri = contenitoreDecodifica.ElencoDecCodeSottoGruppoOnere;
            }

            bool? EnteIstruttoreExInpdap = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteIstruttoreExInpdap.GetValueOrDefault() : (bool?)null;
            bool? TipoCumulo = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.TipoCumulo : (bool?)null;

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out ctrl);
            //ENG - MEMO 50/2023
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);
            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);
            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            //ENG - MEMO 50/2023
            bool aggiornaSupplementi = false;
            if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.Tipo == "0001" &&
                !Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
            {
                aggiornaSupplementi = datiPensione.TipoCalcolo != datiGenerici.TipoCalcolo;
                if (datiIstruttoria == null)
                    datiIstruttoria = new DatiIstruttoria();
                if (datiPensione.TipoCalcolo.HasValue)
                    datiIstruttoria.TipoCalcoloPrecedente = datiPensione.TipoCalcolo;
            }

            if (Utility.IsDomandaIOCUM(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiGenerici.NaturaPensione) && Utility.ValorizzaTipologiaCumuloPerInabilita())
            {
                if ((Utility.IsDomandaInabilitaOrdinaria(datiPensione) && (datiGenerici.NaturaPensione.StartsWith("3") || datiGenerici.NaturaPensione.StartsWith("4"))) || datiPensione.IdTipoPLPerRIC == Utility.TipoPLPerRIC.RicInabilitaOrdinariaInCumulo.GetHashCode())
                    datiPensioniDatiGenerici.TipologiaCumulo = 'C';
                if ((Utility.IsDomandaInabilitaLegge335(datiPensione) && (datiGenerici.NaturaPensione.StartsWith("3") || datiGenerici.NaturaPensione.StartsWith("4"))) || datiPensione.IdTipoPLPerRIC == Utility.TipoPLPerRIC.RicInabilitaArt2Comma12Legge3351995InCumulo.GetHashCode())
                    datiPensioniDatiGenerici.TipologiaCumulo = 'D';
                if ((Utility.IsDomandaInabilitaProficuoLavoro(datiPensione) && (datiGenerici.NaturaPensione.StartsWith("3") || datiGenerici.NaturaPensione.StartsWith("4"))) || datiPensione.IdTipoPLPerRIC == Utility.TipoPLPerRIC.RicInabilitaAProficuoLavoroMensioniInCumulo.GetHashCode())
                    datiPensioniDatiGenerici.TipologiaCumulo = 'E';
            }

            //ENG - memo 28_2024
            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if (((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0045" && datiPensione.CodiceTipoRichiesta == "AV")) &&
                    ((datiGenerici.TipoCalcolo.HasValue && datiGenerici.TipoCalcolo == (byte)Utility.TipoCalcolo.Contributivo) || Utility.IsDomandaCumulo(datiPensione.SiglaCategoria)) &&
                    datiGenerici != null)
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
                    else if (contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.HasValue)
                        datiGenerici.ScadenzaRevisioneSanitaria = contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria;
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiGenericiPerPensione(datiGenerici, datiAssicurativi, datiPensione, datiEnpals, isRiaperturaDomanda);
                StoreDatiGenericiPerIstruttoria(datiPensione, datiGenerici, ref datiIstruttoriaCommon);
                StoreDatiGenericiPerDatiPagamento(datiPensione, datiGenerici, ref datiPagamento);
                StoreDatiGenericiPerNuoveLiquidate(datiPensione.Id, datiGenerici, ref datiNuoveLiquidate);
                StoreDatiGenericiPerPensioniDatiGenerici(datiPensione, datiGenerici, ref datiPensioniDatiGenerici);

                if ((datiGenerici.IsDatiGenericiPensioneNull() && datiGenerici.IsDatiGenericiIstruttoriaNull() &&
                    datiGenerici.IsDatiGenericiPagamentoNull() && datiGenerici.IsDatiGenericiNuoveLiquidateNull() &&
                    datiGenerici.IsDatiGenericiPensioniDatiGenericiNull()) || IsCancelOperation)
                    datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                else
                    datiQuadroLiquidazionePensione.TabDatiGenerici = 2;

                if (datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value)
                {
                    if (datiQuadroLiquidazionePensione.TabPrecedentePensione == 1 && !Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 0;
                }
                else
                {
                    bool? isDomandaTrasformazioneAOI = Utility.IsDomandaTrasformazioneAOI(datiPensione);
                    if (isDomandaTrasformazioneAOI.HasValue && isDomandaTrasformazioneAOI.Value && !Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 0;
                    else
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 1;
                }

                #region Gestione visibilità menu Bititolarità

                if (!((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && (datiPensione.Tipo == "0009" || datiPensione.Tipo == "0192")) ||
                    (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0009")))
                {
                    if (Utility.IsBititolaritaVisible(datiGenerici.NaturaPensione))
                    {
                        if (Utility.IsRicostituzione_Reddituale(datiPensione) || (Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) != null && datiQuadroBititolarita.Tipo == 1))
                        {
                            datiQuadroBititolarita.Tipo = 1;
                            datiQuadroBititolarita.TabAltrePensioni = 1;
                        }
                        else if (datiQuadroBititolarita.Tipo == 0)
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
                }
                #endregion Gestione visibilità menu Bititolarità

                #region Gestione visibilità tabs MaggiorazioneBenefici

                //ENG - Integrazione Modifiche Accenture
                if (Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault() && (!Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione) &&
                 !Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale) &&
                 (datiMaggiorazioniBeneficiCommon == null || (!datiMaggiorazioniBeneficiCommon.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() && !datiMaggiorazioniBeneficiCommon.IsBeneficioApePrecociFromFELPE.GetValueOrDefault() &&
                 !Utility.IsDomandaVecchiaiaENAV(datiPensione) && datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio != "01"))))
                    GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);

                if (datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value)
                {
                    if (!(Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) != null && datiQuadroMaggiorazioniBenefici.TabExCombattente == 1))
                    {
                        if ((datiExCombattente == null || datiExCombattente.IsDatiExCombattenteNull()) || (datiQuadroMaggiorazioniBenefici.TabExCombattente != 2))
                            datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                        else
                            datiQuadroMaggiorazioniBenefici.TabExCombattente = 2;
                    }
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = null;

                if (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value)
                    if (!(Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) != null && datiQuadroMaggiorazioniBenefici.TabBenefici == 1))
                    {
                        if ((Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaCOOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaCRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria))) || Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                            datiQuadroMaggiorazioniBenefici.TabBenefici = 1;
                        else if ((datiBenefici == null || datiBenefici.IsDatiBeneficiNull()) || (datiQuadroMaggiorazioniBenefici.TabBenefici != 2))
                            datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                        else
                            datiQuadroMaggiorazioniBenefici.TabBenefici = 2;
                    }
                    else
                        datiQuadroMaggiorazioniBenefici.TabBenefici = null;

                //ENG - memo 28_2024
                if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                {
                    if (((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") ||
                        (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0045" && datiPensione.CodiceTipoRichiesta == "AV")) &&
                        datiGenerici.Benefici.HasValue && !datiGenerici.Benefici.Value)
                    {
                        datiQuadroMaggiorazioniBenefici.TabBenefici = null;
                    }
                }

                if (datiGenerici.Maggiorazioni.HasValue && datiGenerici.Maggiorazioni.Value)
                {
                    if (!(Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) != null && datiQuadroMaggiorazioniBenefici.TabMaggiorazioni == 1))
                    {
                        if ((datiMaggiorazioni == null || datiMaggiorazioni.IsDatiMaggiorazioniNull()) || (datiQuadroMaggiorazioniBenefici.TabMaggiorazioni != 2))
                            datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = 0;
                        else
                            datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = 2;
                    }
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = null;


                //ENG - Risolta problematica per check benefici
                if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0017")
                {
                    if (!datiGenerici.Benefici.HasValue || !datiGenerici.Benefici.Value)
                        datiQuadroMaggiorazioniBenefici.TabBenefici = null;
                }


                if ((datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value && datiQuadroMaggiorazioniBenefici.TabExCombattente == 2) ||
                    (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value && datiQuadroMaggiorazioniBenefici.TabBenefici == 2) ||
                    (datiGenerici.Maggiorazioni.HasValue && datiGenerici.Maggiorazioni.Value && datiQuadroMaggiorazioniBenefici.TabMaggiorazioni == 2) ||
                    ((Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo)) && datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                if (datiQuadroMaggiorazioniBenefici.TabExCombattente == 0 || datiQuadroMaggiorazioniBenefici.TabBenefici == 0 || datiQuadroMaggiorazioniBenefici.TabMaggiorazioni == 0 || datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo == 0)
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;
                if (!datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && !datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && !datiQuadroMaggiorazioniBenefici.TabMaggiorazioni.HasValue && !datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.HasValue)
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

                #region Gestione Semaforo Eliminazione
                // Per le ricostituzioni il semaforo non deve variare
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                {
                    if (datiEliminazione == null || datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
                    {
                        if ((Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) && datiIstruttoria != null && datiIstruttoria.ScadenzaAssegno.HasValue &&
                            Utility.DataSuccessivaA(Utility.FirstDayOfMonth(dataSistema), Utility.FirstDayOfMonth(datiIstruttoria.ScadenzaAssegno.Value))) ||
                            ((datiAnagraficiTitolare != null && datiAnagraficiTitolare.DataMorte.HasValue && Utility.DataSuccessivaA(datiAnagraficiTitolare.DataMorte.Value, datiPensione.DecorrenzaOriginaria.Value)) ||
                            (isEliminazioneRossoPerConfermaInvalidita && !IsCancelOperation)) || Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
                        {
                            datiQuadroEliminazione.Tipo = 2;
                            datiQuadroEliminazione.TabEliminazione = 0;
                        }
                        else
                        {
                            datiQuadroEliminazione.Tipo = 1;
                            datiQuadroEliminazione.TabEliminazione = 1;
                        }
                    }

                    GestioneQuadri.SalvaQuadroEliminazione(datiPensione.Id, datiQuadroEliminazione);
                }
                #endregion Gestione Semaforo Eliminazione


                #region Gestione Semaforo Detrazioni
                if (isVariaDetrazioni || !Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda))
                {
                    // Le detrazioni non devono essere presenti nel caso in cui venga salvata l'esenzione fiscale
                    Utility.ManageSemaforoDetrazioniPerEsenzioneFiscale(datiPensione, datiQuadroDetrazioni, datiGenerici.CodiceComunicazioneCampo4, isRiaperturaDomanda, isVariaDetrazioni, isVittimeTerrorismoUnderOver80);
                }
                #endregion Gestione Semaforo Detrazioni

                if (!Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaBancari(datiPensione.SiglaCategoria))
                {
                    #region Gestione Semaforo Istruttoria
                    if (!Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && !Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) && !Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) &&
                        !Utility.IsDomandaMIN(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && !Utility.IsDomandaBancari(datiPensione.SiglaCategoria) &&
                        !Utility.IsRenditaCasalinghe(datiPensione) && !Utility.IsRenditaFacoltativa(datiPensione) && !Utility.IsDomandaVOST(datiPensione.SiglaCategoria))
                    {
                        if (((datiIstruttoria == null || !datiIstruttoria.CodiceBancaEsodati.HasValue) && IsIstruttoriaAziendaVisible(datiGenerici.NaturaPensione, datiPensione.SiglaCategoria, datiPensione)) ||
                            (datiQuadroLiquidazionePensione.TabIstruttoria == 1 && datiEnpals != null && !datiEnpals.IsIstruttoriaNull()) ||
                            ((datiIstruttoria == null || !datiIstruttoria.CodiceAziendaEditoria.HasValue) && Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione)) ||
                            ((datiIstruttoria == null || !datiIstruttoria.CodiceAziendaEditoriaLetteraB.HasValue) && Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione)) ||
                            ((datiIstruttoria == null || !datiIstruttoria.CodiceAziendaEditoriaPerTipo0171.HasValue) && Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione)) ||
                            ((datiIstruttoria == null || !datiIstruttoria.CodiceAziendaEditoriaPerTipo0179.HasValue) && Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione)))
                            datiQuadroLiquidazionePensione.TabIstruttoria = 0;
                        else if (!(Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) ||
                            Utility.IsDomandaSalvaguardia135(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) ||
                            Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                            Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                            Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione))
                            &&
                            !IsIstruttoriaAziendaVisible(datiGenerici.NaturaPensione, datiPensione.SiglaCategoria, datiPensione) &&
                            (datiIstruttoria == null || (datiIstruttoria.IsDatiIstruttoriaDatiGenericiNull() && datiIstruttoria.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoria.IsDatiIstruttoriaMaggiorazioneBeneficiNull() && datiIstruttoria.IsDatiIstruttoriaPensioneNull()))
                            && !(Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) ||
                                 Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) ||
                                 Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria) || Utility.IsDomandaManualeInvaliditaOver80_L80(datiPensione))//per VESO33 e VESO92 VOCRED CRED27 VOCOOP COOP28 tabIstruttoria sempre rosso
                            && (datiEnpals == null || datiEnpals.IsIstruttoriaNull())
                            )
                            datiQuadroLiquidazionePensione.TabIstruttoria = 1;
                    }
                    #endregion Gestione Semaforo Istruttoria

                    #region Gestione Semaforo Oneri

                    if (genericiChangedPerOneri)
                    {
                        GestioneOneri.DatiOneri datiOneriSperDonna = lstDatiOneri != null ? lstDatiOneri.Where(x => x.IdCodeGruppo == decGruppoOnere.Find(y => y.Code == "4700").Id && (x.IdCodeSottoGruppo == decSottoGruppoOneri.Find(y => y.Code == "4701").Id || x.IdCodeSottoGruppo == decSottoGruppoOneri.Find(y => y.Code == "4702").Id)).FirstOrDefault() : null;
                        if (Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri) && datiOneriSperDonna == null)
                        {
                            GestioneOneri.DatiOneri newOneri = new GestioneOneri.DatiOneri { IdCodeGruppo = decGruppoOnere.Find(y => y.Code == "4700").Id, IdCodeSottoGruppo = Utility.IsDomandaINPDAP(datiPensione.Gestione) ? decSottoGruppoOneri.Find(y => y.Code == "4702").Id : decSottoGruppoOneri.Find(y => y.Code == "4701").Id, Decorrenza = datiPensione.DecorrenzaOriginaria, IdPensione = datiPensione.Id };
                            GestioneOneri.SalvaOneriOnere(newOneri);
                            recordOneriChanged = true;
                        }
                        else if (!Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) && datiOneriSperDonna != null)
                        {
                            GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                            lstDatiOneri.Where(x => x.IdCodeGruppo != decGruppoOnere.Find(y => y.Code == "4700").Id && x.IdCodeSottoGruppo != decSottoGruppoOneri.Find(y => y.Code == "4701").Id && x.IdCodeSottoGruppo != decSottoGruppoOneri.Find(y => y.Code == "4702").Id)
                                .ToList()
                                .ForEach(x => GestioneOneri.SalvaOneriOnere(x));
                            recordOneriChanged = true;
                        }

                        if (Utility.IsDomandaVecchiaiaENAV(datiPensione))
                        {
                            GestioneOneri.DatiOneri datiOneriEnav = lstDatiOneri != null ? lstDatiOneri.Where(x => x.IdCodeGruppo == decGruppoOnere.FirstOrDefault(y => y.Code == "5600").Id && x.IdCodeSottoGruppo == decSottoGruppoOneri.FirstOrDefault(y => y.Code == "5601").Id).FirstOrDefault() : null;
                            if (datiOneriEnav == null)
                            {
                                GestioneOneri.DatiOneri newOneri = new GestioneOneri.DatiOneri { IdCodeGruppo = decGruppoOnere.FirstOrDefault(y => y.Code == "5600").Id, IdCodeSottoGruppo = decSottoGruppoOneri.FirstOrDefault(y => y.Code == "5601").Id, Decorrenza = datiPensione.DecorrenzaOriginaria, IdPensione = datiPensione.Id };
                                GestioneOneri.SalvaOneriOnere(newOneri);
                            }
                        }
                    }

                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

                    // Oneri
                    if (tipoDomanda != Utility.TipoDomanda.Ripristino)
                    {
                        if ((Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                            Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                            Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                            Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) ||
                            Utility.IsDomandaAPEPrecoci(datiPensione))
                            || Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : 0, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : 0)
                            || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo)
                            || Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri)
                            || Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione)
                            || Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione)
                            || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione)
                            || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione)
                            || (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) && !Utility.IsDomandaMIN(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                            || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione)
                            || Utility.IsDomandaVecchiaiaENAV(datiPensione) || (datiBenefici != null && datiBenefici.TipoSettimaneBeneficio == "01")
                            || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                            || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione)
                            || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                            || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                            || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                            || (Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                            || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                        {
                            if (((datiQuadroOneri.TabOneri == null) || (datiQuadroOneri.TabOneri == 2 && recordOneriChanged)) && !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione))))
                                datiQuadroOneri.TabOneri = 0; //rosso
                        }
                        else
                            datiQuadroOneri.TabOneri = null;
                    }
                    // Prepensionamento
                    if (!(Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                       Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) ||
                       Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                       Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                       Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione))
                        && tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti &&
                                tipoDomanda != Utility.TipoDomanda.Ripristino && !isRiaperturaDomanda)
                    {
                        if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : 0, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : 0, datiGenerici.NaturaPensione))
                            if ((datiPrepensionamento == null || datiPrepensionamento.IsDatiPrepensionamentoNull()) || (datiQuadroOneri.TabPrepensionamento != 2))
                                datiQuadroOneri.TabPrepensionamento = 0;
                            else
                                datiQuadroOneri.TabPrepensionamento = 2;
                        else
                            datiQuadroOneri.TabPrepensionamento = null;
                    }
                    else
                        datiQuadroOneri.TabPrepensionamento = null;

                    if (tipoDomanda != Utility.TipoDomanda.Ripristino)
                    {
                        if (//condizione visibilità oneri
                        (Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                        Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                        Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                        Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) ||
                        Utility.IsDomandaAPEPrecoci(datiPensione)
                        || Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : 0, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : 0)
                        || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo)
                        || Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri)
                        || Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione)
                        || Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione)
                        || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione)
                        || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) && !Utility.IsDomandaMIN(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                        || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione)
                        || Utility.IsDomandaVecchiaiaENAV(datiPensione)) || (datiBenefici != null && datiBenefici.TipoSettimaneBeneficio == "01")
                        || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                        || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                        || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                        || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                        || (Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                        || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)
                        ||
                            //condizione visibilità prepensionamento
                       (Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : 0, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : 0, datiGenerici.NaturaPensione) &&
                        !Utility.IsDomandaSalvaguardia122(datiPensione) && tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti &&
                         tipoDomanda != Utility.TipoDomanda.Ripristino && !isRiaperturaDomanda)
                        || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione))
                        {
                            if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione))))
                                datiQuadroOneri.Tipo = 2;
                        }
                        else
                            datiQuadroOneri.Tipo = 0;
                    }

                    GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                    #endregion Gestione Semaforo Oneri

                    #region Gestione Semafori Supplementi

                    //Per ENAPALS il semaforo dei supplementi non deve variare a causa della gestione che dipende dai dati che arrivano dal SAS/SAI.
                    if (!Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && !Utility.IsDomandaESPA(datiPensione.SiglaCategoria) && !Utility.IsPannelloSupplementiAnte96(datiPensione, datiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) && Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) == null)
                    {
                        if (tipoDomanda != Utility.TipoDomanda.Superstiti && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti)
                        {
                            GestioneDanteCausa.DatiDanteCausa datiDA = contenitore.DatiDanteCausa;
                            Utility.TipoQuadro? tipoQuadro = Utility.GetVisibilitaQuadroSupplementi(datiPensione, datiGenerici.NaturaPensione, isRiaperturaDomanda, datiDA);
                            if (tipoQuadro == Utility.TipoQuadro.Obbligatorio)
                            {
                                if (datiQuadroSupplementi.TabSupplementi == 1)//facoltativo
                                    datiQuadroSupplementi.TabSupplementi = 0;
                                datiQuadroSupplementi.Tipo = 2;
                            }
                            else if (tipoQuadro == Utility.TipoQuadro.Facoltativo)
                            {
                                if ((listaSupplementi == null || listaSupplementi.Count == 0) &&
                                    !(Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                    (datiSupplBase == null || datiSupplBase.IsSupplementiBaseNull()) &&
                                    datiIntArt11 == null)
                                {
                                    if (datiQuadroSupplementi.TabSupplementi == 0) //obbligatorio
                                        datiQuadroSupplementi.TabSupplementi = 1;
                                    datiQuadroSupplementi.Tipo = 1;
                                }
                            }

                            GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                        }
                    }
                    #endregion Gestione Semafori Supplementi

                    #region Gestione Semaforo Redditi
                    if (datiPensioniDatiGenerici != null && datiPensioniDatiGenerici.CumuloEsterno == 'M')
                    {
                        GestioneRedditi.EliminaAllRedditiDRedd(datiPensione.Id);
                        datiQuadroRedditi.Tipo = 0;
                        datiQuadroRedditi.TabRedditi = null;

                        GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                    }
                    else if (!Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) && !Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) &&
                             !Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) && !Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                    {
                        if (aggiornaRedditi)
                        {
                            //Eng - se è una domanda di ricostituzione documentale, tipo 0001 e (categoria "VOTOT", prodotto 0108) oppure (categoria "IOTOT", prodotto 0308) oppure (categoria "SOTOT", prodotto 0408), non effettuo alcuna modifica al quadro redditi
                            if (!(tipoDomanda == Utility.TipoDomanda.Ricostituzione && datiPensione.Tipo == "0001" && ((datiPensione.SiglaCategoria.Trim() == "VOTOT" && datiPensione.Prodotto == "0108") || (datiPensione.SiglaCategoria.Trim() == "IOTOT" && datiPensione.Prodotto == "0308") || (datiPensione.SiglaCategoria.Trim() == "SOTOT" && datiPensione.Prodotto == "0408"))))
                            {
                                // Se la domanda è una quota 100, 102 e il quadro non è già obbligatorio non effettuo alcuna modifica
                                if (!((Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || (Utility.IsDomandaAUT(datiPensione) && datiPensione.GetFiltro().ToUpperInvariant().Equals("ERI")) || Utility.IsDomandaAGOTipoContributivoFiltroERI(datiPensione) ||
                                    (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                                    ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))) ||
                                    Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)) && datiQuadroRedditi.Tipo != 2))
                                {
                                    if (datiQuadroRedditi.Tipo != 2)
                                        datiQuadroRedditi.Tipo = 2;

                                    datiQuadroRedditi.TabRedditi = 0;

                                    GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                                }
                            }
                        }
                    }
                    #endregion Gestione Semaforo Redditi
                }

                #region Gestione Semafori Dati Calcolo
                if (isDatiCalcoloVittimeRosso && !isDatiCalcoloVittimeNonVisibile)
                {
                    datiQuadroDatiContributivi.TabVittime = 0;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                }
                else if (isDatiCalcoloVittimeNonVisibile)
                {
                    datiQuadroDatiContributivi.TabVittime = null;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                }

                if (Utility.IsDomandaSOCUM(datiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(datiPensione) && !datiGenerici.TipoCumulo.GetValueOrDefault())
                {
                    if (datiGenerici.TipoCumulo != TipoCumulo)
                    {
                        datiQuadroDatiContributivi.TabQuotePensione = 0;
                        GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                    }
                }

                //ENG - VOPGI           
                if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione))
                {
                    if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0017") ||
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0017") ||
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0001") ||
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0001"))
                    {
                        if (tipoCalcoloDB != tipoCalcoloView)
                        {
                            datiQuadroDatiContributivi.TabDatiCalcolo = 1;
                            datiQuadroDatiContributivi.TabQuotaFondoINPGI = 0;
                            //dobbiamo eliminare i dati calcolo perchè il tab Dati Calcolo diventa giallo e le griglie contributive/retributive si vedono in base al tipo calcolo o in base all'inizio assicurazione. Necessario
                            //per evitare che dei dati precedentemente inseriti rimangano sul database e non siano corretti
                            GestioneCalcolo.EliminaCalcoloContributivoByIdPensione(datiPensione.Id, false);
                            GestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(datiPensione.Id, false);
                            GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                        }
                    }
                }
                #endregion Gestione Semafori Dati Calcolo

                #region Gestione Semafori Familiari

                if (datiPensioniDatiGenerici != null && datiPensioniDatiGenerici.CumuloEsterno == 'M')
                {
                    if (listaFamiliari != null && listaFamiliari.Count > 0 && listaCodMaggFamiliari != null && listaCodMaggFamiliari.Count > 0)
                    {
                        bool semaforoFamiliari = false;
                        listaFamiliari.ForEach(x =>
                        {
                            if (listaCodMaggFamiliari.Exists(y => y.IdAnagrafica == x.IdAnagrafica && y.CodiceMaggiorazione == 2))
                            {
                                x.Confermato = false;
                                GestioneFamiliari.SalvaFamiliare(x, listaCodMaggFamiliari.FindAll(y => y.IdAnagrafica == x.IdAnagrafica),
                                    listaAnagraficaFamiliari.FirstOrDefault(y => y.Id == x.IdAnagrafica), null, datiPensione.Id, datiPensione.SiglaCategoria);
                                semaforoFamiliari = true;
                            }
                        });

                        if (semaforoFamiliari)
                        {
                            datiQuadroFamiliari.TabFamiliari = 0;
                            datiQuadroFamiliari.Tipo = 2;

                            GestioneQuadri.SalvaQuadroFamiliari(datiPensione.Id, datiQuadroFamiliari);
                        }
                    }
                }

                #endregion Gestione Semafori Familiari

                #region Gestione Semaforo Titolare
                if (Utility.IsDomandaIOCUM(datiPensione.SiglaCategoria) && Utility.IsDomandaPensioneInabilita(datiPensione) && datiPensione.Tipo != "0001" && datiPensione.Tipo != "0052"
                    && !Utility.IsPensioneInabilitaProficuoLavoroCumulo(datiPensione))
                {
                    //ogni volta che EnteIstruttoreExInpdap cambia oppure è la prima volta che viene inserito SI
                    if ((datiGenerici.EnteIstruttoreExInpdap != EnteIstruttoreExInpdap && EnteIstruttoreExInpdap != null)
                        || (EnteIstruttoreExInpdap == null && datiGenerici.EnteIstruttoreExInpdap.GetValueOrDefault()))
                    {
                        datiQuadroTitolare.TabAnagrafica = 0;
                        GestioneQuadri.SalvaQuadroTitolare(datiPensione.Id, datiQuadroTitolare);
                    }
                }
                #endregion

                #region Gestione Semafori Supplementi
                //ENG - MEMO 50/2023
                if (aggiornaSupplementi && !Utility.IsPannelloSupplementiAnte96(datiPensione, datiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) && Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) == null)
                {
                    datiQuadroSupplementi.TabSupplementi = 0;
                    datiQuadroSupplementi.Tipo = 2;

                    GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                }
                //if (Utility.IsRicostituzione(datiPensione.Gruppo) && (Utility.IsDomandaVOTOT(datiPensione.SiglaCategoria) || Utility.IsDomandaSOTOT(datiPensione.SiglaCategoria)))
                //{
                //    datiQuadroSupplementi.Tipo = 0;
                //    GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                //}
                #endregion Gestione Semafori Supplementi

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiIstruttoria = datiIstruttoriaCommon;
            contenitore.DatiPagamento = datiPagamento;
            contenitore.DatiNuoveLiquidate = datiNuoveLiquidate;
            contenitore.DatiPensioniDatiGenerici = datiPensioniDatiGenerici;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            contenitore.DatiQuadroBititolarita = datiQuadroBititolarita;
            contenitore.DatiQuadroEliminazione = datiQuadroEliminazione;
            contenitore.DatiQuadroOneri = datiQuadroOneri;
            contenitore.DatiQuadroSupplementi = datiQuadroSupplementi;
            contenitore.DatiQuadroRedditi = datiQuadroRedditi;
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            contenitore.DatiQuadroFamiliari = datiQuadroFamiliari;
            contenitore.DatiQuadroTitolare = datiQuadroTitolare;
            contenitore.DatiQuadroRichiestaBonus = datiQuadroRichiestaBonus;
            //--------------------------------------------------------------------
        }

        public static void GetDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            bool isRiaperturaDomanda, out DatiGenerici datiGenerici, out string errori)
        {
            datiGenerici = null;
            errori = string.Empty;

            if (contenitore.DatiPensione == null && contenitore.DatiIstruttoria == null && contenitore.DatiPagamento == null)
                return;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            datiGenerici = new DatiGenerici();

            Utility.ValorizzaOggetti(contenitore.DatiPensione, datiGenerici);
            Utility.ValorizzaOggetti(contenitore.DatiIstruttoria, datiGenerici);
            Utility.ValorizzaOggetti(contenitore.DatiPagamento, datiGenerici);
            Utility.ValorizzaOggetti(contenitore.DatiNuoveLiquidate, datiGenerici);
            Utility.ValorizzaOggetti(contenitore.DatiPensioniDatiGenerici, datiGenerici);

            if (datiGenerici.IsDatiGenericiIstruttoriaNull() && datiGenerici.IsDatiGenericiPensioneNull() && datiGenerici.IsDatiGenericiPagamentoNull() &&
                datiGenerici.IsDatiGenericiNuoveLiquidateNull() && datiGenerici.IsDatiGenericiPensioniDatiGenericiNull())
            {
                if (contenitore.DatiPensione.IsCumuloAutomatica.GetValueOrDefault())
                {
                    ServiceReferences.TotalIvs.clsDatiCumulo risposta = null;
                    if ((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCO_RIC_CUMULO_AUTOMATICHE, Utility.DataSistemaAgo))
                        || (Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id) && GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCO_TRF_CUMULO_AUTOMATICHE, Utility.DataSistemaAgo)))
                    {
                        if (!GestioneTotalIvs.GetDatiCumulRicostituzioneIVS(contenitore.DatiPensione.NDomus, out risposta, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                    }
                    else
                    {
                        if (!GestioneTotalIvs.GetDatiCumulIVS(contenitore.DatiPensione.NDomus, out risposta, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                    }

                    if (risposta.objDomanda != null)
                    {
                        if (risposta.objDomanda.SistemaCalcolo == "1")
                            datiGenerici.Contributivo = '8';
                        else if (risposta.objDomanda.SistemaCalcolo == "2")
                            datiGenerici.Contributivo = '2';
                    }

                    if (!string.IsNullOrEmpty(risposta.objDomanda.EnteIstruttoria) && !string.IsNullOrEmpty(risposta.objDomanda.EnteIstruttoria.Trim()))
                    {
                        if (contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale != null && contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Count > 0)
                        {
                            GestioneDecodifica.DecodificaEnteCassaProfessionale decodifica = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Find(x => x.TraduzioneSuGP == risposta.objDomanda.EnteIstruttoria.PadLeft(4, '0'));
                            if (decodifica != null)
                                datiGenerici.EnteCassa = decodifica.Id;
                        }
                    }
                }
                if (contenitore.DatiPensione.IsTotAutomatica.GetValueOrDefault())
                {
                    ServiceReferences.TotalIvs.clsDati risposta = null;

                    if (!GestioneTotalIvs.GetDatiTotalIVS(contenitore.DatiPensione.NDomus, out risposta, out errori))
                        throw new INPS.DNA.DnaValidationException(errori);


                    if (risposta.objDomanda != null)
                    {
                        if (risposta.objDomanda.SistemaCalcolo == "1")
                            datiGenerici.Contributivo = '8';
                        else if (risposta.objDomanda.SistemaCalcolo == "2")
                            datiGenerici.Contributivo = '2';
                    }

                    if (!string.IsNullOrEmpty(risposta.objDomanda.EnteIstruttoria) && !string.IsNullOrEmpty(risposta.objDomanda.EnteIstruttoria.Trim()))
                    {
                        if (contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale != null && contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Count > 0)
                        {
                            GestioneDecodifica.DecodificaEnteCassaProfessionale decodifica = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Find(x => x.TraduzioneSuGP == risposta.objDomanda.EnteIstruttoria.PadLeft(4, '0'));
                            if (decodifica != null)
                                datiGenerici.EnteCassa = decodifica.Id;
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(contenitore.DatiPensione.CodiceTipoRichiesta) && String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                datiGenerici.NaturaPensione = GestioneCrossControls.GetCodiceNaturaFromCodiceTipoRichiesta(contenitore.DatiPensione.CodiceTipoRichiesta, null, Utility.TipoAppartenenza.AGO);
            if (Utility.IsDomandaSperimentaleDonna(contenitore.DatiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(contenitore.DatiPensione) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione, true, true))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " O ";
                if (!datiGenerici.TipoCalcolo.HasValue || datiGenerici.TipoCalcolo.Value == 0)
                    datiGenerici.TipoCalcolo = 1; //contributivo
            }
            if (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "  O";
            }
            if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "1 O";
            }
            if (Utility.IsRimpatriatiAlbania(contenitore.DatiPensione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " H ";
            }
            if (Utility.IsDomandaUsuranti(contenitore.DatiPensione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "  Z";
                else if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) == " ")
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "Z";
            }
            if (Utility.IsTelematica(contenitore.DatiPensione.CodiceProcedura) && !Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!datiGenerici.DataCompletezza.HasValue)
                    datiGenerici.DataCompletezza = contenitore.DatiPensione.DataPresentazioneDomanda;
            }
            if (Utility.IsDomandaTrasformazioneInvalidita(contenitore.DatiPensione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "  H";
                else if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) == " ")
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "H";
            }
            if (Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria))
            {
                if (string.IsNullOrEmpty(datiGenerici.NaturaPensione))
                {
                    if (Utility.IsDomandaVecchiaiaVESO29(contenitore.DatiPensione))
                        datiGenerici.NaturaPensione = "   ";
                    else if (Utility.IsDomandaAnticipataVESO29(contenitore.DatiPensione))
                        datiGenerici.NaturaPensione = "1  ";
                }
                else
                {
                    if (Utility.IsDomandaVecchiaiaVESO29(contenitore.DatiPensione))
                    {
                        if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 1) != " ")
                            datiGenerici.NaturaPensione = " " + datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(1, 2);
                    }
                    else if (Utility.IsDomandaAnticipataVESO29(contenitore.DatiPensione))
                    {
                        if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 1) != "1")
                            datiGenerici.NaturaPensione = "1" + datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(1, 2);
                    }
                }
            }
            if ((Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria)) && !Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) &&
                Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.TipoCertificazioneFelpe == "CII")
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "  V";
                else if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) != "V")
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "V";
            }
            if ((Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria)) &&
                !Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) &&
                Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.TipoCertificazioneFelpe == "AUT")
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = "  B";
                else if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) != "B")
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "B";
            }
            if (Utility.IsDomandaInvaliditaSpecifica(contenitore.DatiPensione) && Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " 1 ";
                else
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.Substring(0, 1) + "1" + datiGenerici.NaturaPensione.Substring(2, 1);
            }
            if (Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, true) || Utility.IsDomandaTipoContributivoCumulo(contenitore.DatiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione) ||
                (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione)) ||
                (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione)))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " J ";
                else
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.Substring(0, 1) + "J" + datiGenerici.NaturaPensione.Substring(2, 1);
            }

            if (Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria))
            {
                if (string.IsNullOrEmpty(datiGenerici.NaturaPensione))
                {
                    if (Utility.IsDomandaVecchiaiaESOTEL(contenitore.DatiPensione) || Utility.IsDomandaVecchiaiaESOAMB(contenitore.DatiPensione))
                        datiGenerici.NaturaPensione = "   ";
                    else if (Utility.IsDomandaAnticipataESOTEL(contenitore.DatiPensione) || Utility.IsDomandaAnticipataESOAMB(contenitore.DatiPensione))
                        datiGenerici.NaturaPensione = "1  ";
                }
                else
                {
                    if (Utility.IsDomandaVecchiaiaESOTEL(contenitore.DatiPensione) || Utility.IsDomandaVecchiaiaESOAMB(contenitore.DatiPensione))
                    {
                        if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 1) != " ")
                            datiGenerici.NaturaPensione = " " + datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(1, 2);
                    }
                    else if (Utility.IsDomandaAnticipataESOTEL(contenitore.DatiPensione) || Utility.IsDomandaAnticipataESOAMB(contenitore.DatiPensione))
                    {
                        if (datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(0, 1) != "1")
                            datiGenerici.NaturaPensione = "1" + datiGenerici.NaturaPensione.PadLeft(3, ' ').Substring(1, 2);
                    }
                }
            }

            if (datiGenerici.IsDatiGenericiIstruttoriaNull() && datiGenerici.IsDatiGenericiPensioneNull() && datiGenerici.IsDatiGenericiPagamentoNull() &&
                datiGenerici.IsDatiGenericiNuoveLiquidateNull() && datiGenerici.IsDatiGenericiPensioniDatiGenericiNull())
                datiGenerici = null;

            if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda))
            {
                byte? causaCarico = GetCausaCaricoFromTipoDomanda(contenitore.DatiPensione);
                if (causaCarico.HasValue)
                {
                    if (datiGenerici == null)
                        datiGenerici = new DatiGenerici();
                    datiGenerici.CausaCarico = causaCarico;
                }
            }

            if (Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) ||
                (Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") ||
                Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) || Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null) ||
                Utility.IsDomandaESPA_L26(contenitore.DatiPensione) || Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione))
            {
                if (datiGenerici == null)
                    datiGenerici = new DatiGenerici();
                datiGenerici.TipoCalcolo = null;
            }
        }

        public static void EliminaDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiExCombattente datiExCombattente,
            DatiBenefici datiBenefici, DatiMaggiorazioni datiMaggiorazioni, DateTime dataSistema, Utility.TipoCalcolo tipoCalcolo, out string msgVideo)
        {
            msgVideo = string.Empty;

            DatiAssicurativi datiAssicurativi = null;
            GetDatiAssicurativi(ref contenitore, out datiAssicurativi, out msgVideo);
            if (!string.IsNullOrEmpty(msgVideo))
                return;

            bool? attivitaUsuranti = contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.Attivitausuranti : null;

            //if (!ControlsDatiGenericiForPensioneProvenienza(numeroDomanda, datiPensione.TrasformazioneAOI, true))
            //{
            //    msgVideo = "Eliminare i dati della Pensione di Provenienza prima di procedere con la cancellazione dei dati Generici";
            //    return;
            //}

            if (!ControlsDatiGenericiForBititolaritaAltraPensioneByIdPensione(contenitore.ListaAltraPensione, string.Empty, true))
            {
                msgVideo = "Eliminare i dati 'Altra Pensione' nel quadro 'Bititolarità' prima di procedere con la cancellazione";
                return;
            }

            if (!ControlsDatiGenericiForIstruttoria(string.Empty, contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.CodiceBancaEsodati, attivitaUsuranti, true, contenitore.DatiPensione,
                out msgVideo))
                return;

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione)) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                if (!GestioneCrossControls.AGO_ControlsTipoCalcoloForDatiContributivi(contenitore.DatiPensione.TipoCalcolo, contenitore.DatiPensione.FineAssicurazione, contenitore.DatiPensione,
                    contenitore.ListaDatiContributivi, contenitore.ListaDatiRetributivi, Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione), true))
                {
                    msgVideo = "Eliminare i dati Calcolo prima di procedere con la cancellazione.";
                    return;
                }

            if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(contenitore.DatiPensione.ExCombattente, contenitore.DatiPensione.Benefici, contenitore.DatiPensione.Maggiorazioni, true,
                datiExCombattente, datiBenefici, datiMaggiorazioni, out msgVideo))
                return;

            if (!ControlsDatiGenericiAssicurativiForDatiPrepensionamento(contenitore.DatiPensione, contenitore.DatiMaggiorazioniBenefici, contenitore.DatiPrepensionamento,
                datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : null, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : null, null, out msgVideo))
                return;

            if (datiAssicurativi != null && Utility.IsTabPrepensionamentoVisible(contenitore.DatiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, null))
            {
                msgVideo = "Eliminare i Dati Assicurativi prima di procedere con la cancellazione.";
                return;
            }

            if (!GestioneControlli.VerificaCoerenzaTipoCalcoloConTerrorismo(0, contenitore.ListaDatiCalcoloVittimeTerrorismo, out msgVideo))
            {
                msgVideo = "Eliminare i dati Calcolo Vittime prima di procedere con la cancellazione.";
                return;
            }

            StoreDatiGenerici(ref contenitore, ref contenitoreDecodifica, null, datiAssicurativi, datiExCombattente, datiBenefici, datiMaggiorazioni, null, dataSistema, tipoCalcolo,
                Utility.TipoCalcolo.NonValido, true, true);
        }

        private static void StoreDatiGenericiPerPensione(Entity.DatiGenerici datiGenerici, Entity.DatiAssicurativi datiAssicurativi, GestionePensione.DatiPensione datiPensione,
            GestioneEnpals.DatiEnpals datiENPALS, bool isRiaperturaDomanda)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ||
                (Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsEnpalsManualePL(true, Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda), datiPensione.IsDatiENPALSRecuperati)))
                datiGenerici.TipoCalcolo = datiPensione.TipoCalcolo;

            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012")
                datiGenerici.Benefici = datiPensione.Benefici;

            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsEnpalsManualePL(true, Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda), datiPensione.IsDatiENPALSRecuperati))
            {
                if (datiENPALS != null && datiENPALS.NumeroContributiNLNonVedenti.HasValue)
                    datiGenerici.ExCombattente = datiPensione.ExCombattente;
                if (datiENPALS != null && datiENPALS.IndicatoreInvalidita80.HasValue)
                    datiGenerici.Benefici = datiPensione.Benefici;
            }

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && !(Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria)))
            {
                datiGenerici.CodiceArretrati = datiPensione.CodiceArretrati;
                datiGenerici.TrasformazioneAOI = datiPensione.TrasformazioneAOI;
            }

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
                {
                    datiGenerici.ExCombattente = datiPensione.ExCombattente;
                    datiGenerici.Benefici = datiPensione.Benefici;
                    datiGenerici.Maggiorazioni = datiPensione.Maggiorazioni;
                }
            }

            if ((datiAssicurativi != null && Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, null)))
                datiGenerici.Benefici = true;

            if (datiPensione.IsCumuloAutomatica.GetValueOrDefault())
            {
                datiGenerici.Contributivo = datiPensione.Contributivo;
            }

            if (Utility.IsBonusBooking(datiPensione) && datiPensione.Tipo == "0167")
            {
                datiGenerici.AnnoDecorrenzaBonus = datiPensione.AnnoDecorrenzaBonus;
            }

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
            //if (datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value && datiPensione.TipoLetturaUnicarpe.HasValue && datiPensione.TipoLetturaUnicarpe.Value == 'L') 
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                //datiGenerici.ScadenzaRevisioneSanitaria = datiIstruttoria.ScadenzaRevisioneSanitaria;
                datiGenerici.CodiceMobilita = datiIstruttoria.CodiceMobilita;
                datiGenerici.ModalitaLiquidazione = datiIstruttoria.ModalitaLiquidazione;
            }
            if (!Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione))
                datiGenerici.CodiceDomandaRicorso = datiIstruttoria.CodiceDomandaRicorso;

            Utility.ValorizzaOggetti(datiGenerici, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
            {
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(datiPensione.Id);
                datiIstruttoria = null;
            }
            else
                GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);
        }

        private static void StoreDatiGenericiPerDatiPagamento(GestionePensione.DatiPensione datiPensione, Entity.DatiGenerici datiGenerici, ref GestionePagamento.DatiPagamento datiPagamento)
        {
            if (datiPagamento == null)
            {
                if (datiGenerici.IsDatiGenericiPagamentoNull())
                    return;
                else
                    datiPagamento = new GestionePagamento.DatiPagamento();
            }

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
                {
                    datiGenerici.TrattenutaInpdap = datiPagamento.TrattenutaInpdap;
                    datiGenerici.DataRinunciaTrattenutaInpdap = datiPagamento.DataRinunciaTrattenutaInpdap;
                }
            }

            Utility.ValorizzaOggetti(datiGenerici, datiPagamento);

            if (datiPagamento.Equals(new GestionePagamento.DatiPagamento()))
            {
                GestionePagamento.EliminaPagamentoByIdPensione(datiPensione.Id);
                datiPagamento = null;
            }
            else
                GestionePagamento.SalvaPagamento(datiPensione.Id, datiPagamento);
        }

        private static void StoreDatiGenericiPerNuoveLiquidate(long idPensione, Entity.DatiGenerici datiGenerici, ref GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate)
        {
            if (datiNuoveLiquidate == null)
            {
                if (datiGenerici.IsDatiGenericiNuoveLiquidateNull())
                    return;
                else
                {
                    datiNuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();
                    datiNuoveLiquidate.IdPensione = idPensione;
                }
            }

            Utility.ValorizzaOggetti(datiGenerici, datiNuoveLiquidate);

            if (datiNuoveLiquidate.Equals(new GestioneNuoveLiquidate.NuoveLiquidate()))
            {
                GestioneNuoveLiquidate.EliminaNuoveLiquidateByIdPensione(idPensione);
                datiNuoveLiquidate = null;
            }
            else
                GestioneNuoveLiquidate.SalvaNuoveLiquidate(datiNuoveLiquidate);
        }

        private static void StoreDatiGenericiPerPensioniDatiGenerici(GestionePensione.DatiPensione datiPensione, Entity.DatiGenerici datiGenerici,
            ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            if (datiPensioniDatiGenerici == null)
            {
                if (datiGenerici.IsDatiGenericiPensioniDatiGenericiNull())
                    return;
                else
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            }

            if (datiPensione.IsCumuloAutomatica.GetValueOrDefault())
            {
                datiGenerici.TipoCumulo = datiPensioniDatiGenerici.TipoCumulo;
                datiGenerici.CumuloEsterno = datiPensioniDatiGenerici.CumuloEsterno;
                datiGenerici.EnteCassa = datiPensioniDatiGenerici.EnteCassa;
            }

            Utility.ValorizzaOggetti(datiGenerici, datiPensioniDatiGenerici);

            if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
            {
                GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                datiPensioniDatiGenerici = null;
            }
            else
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioniDatiGenerici);
        }

        private static bool ControlsDatiGenericiForPensioneProvenienza(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool? TrasformazioneAOI, bool IsDeleteOperation)
        {
            if (IsDeleteOperation || !TrasformazioneAOI.HasValue || !TrasformazioneAOI.Value)
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

        private static bool ControlsDatiGenericiForBititolaritaAltraPensioneByIdPensione(List<GestioneAltrePensioni.AltraPensione> elencoAltraPensione, string naturaPensione, bool isDeleteOperation)
        {
            if (elencoAltraPensione != null && elencoAltraPensione.Count > 0 && (isDeleteOperation || !Utility.IsBititolaritaVisible(naturaPensione)))
                return false;

            return true;
        }

        private static bool ControlsDatiGenericiForMaggBeneficiByIdPensione(bool? exCombattente, bool? benefici, bool? maggiorazioni, bool IsDeleteOperation,
            Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici, Entity.DatiMaggiorazioni datiMaggiorazioni, out string errore)
        {
            errore = string.Empty;

            if (exCombattente.HasValue && !exCombattente.Value || (IsDeleteOperation && exCombattente.HasValue && exCombattente.Value))
            {
                if (datiExCombattente != null && !datiExCombattente.IsDatiExCombattenteNull())
                {
                    errore = "Eliminare i dati Ex Combattente di Maggiorazione / Benefici prima di procedere.";
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

        private static bool ControlsDatiGenericiForIstruttoria(string NaturaPensione, string siglaCategoria, short? codiceBancaEsodati, bool? attivitaUsuranti, bool IsDeleteOperation, GestionePensione.DatiPensione datiPensione, out string errore)
        {
            errore = string.Empty;

            //if ((datiIstruttoria == null || !codiceBancaEsodati.HasValue) && IsIstruttoriaAziendaVisible(NaturaPensione))
            //{
            //    errore = "In presenza del terzo codice natura 'O' è obbligatorio inserire l'Azienda sulla tab Istruttoria";
            //    return false;
            //}

            if (!Utility.IsDomandaVESO33(siglaCategoria) && !Utility.IsDomandaVESO92(siglaCategoria) && !Utility.IsDomandaVOCRED_CRED27(siglaCategoria) && !Utility.IsDomandaVOCOOP_COOP28(siglaCategoria) &&
                !Utility.IsDomandaVESO29(siglaCategoria) && !Utility.IsDomandaVOESO(siglaCategoria) && !Utility.IsDomandaESOTEL(siglaCategoria) && !Utility.IsDomandaESOAMB(siglaCategoria) &&
                !Utility.IsDomandaESPA(siglaCategoria) && !Utility.IsDomandaBancari(siglaCategoria) && codiceBancaEsodati.HasValue && (!IsIstruttoriaAziendaVisible(NaturaPensione, siglaCategoria, datiPensione) || IsDeleteOperation) &&
                !Utility.IsRenditaCasalinghe(datiPensione) && !Utility.IsRenditaFacoltativa(datiPensione))
            {
                errore = "Eliminare i dati 'Istruttoria' prima di procedere con il salvataggio/eliminazione dei 'Dati Generici'";
                return false;
            }

            //if ((datiIstruttoria == null || !attivitaUsuranti.HasValue) && IsIstruttoriaAttivitaUsurantiVisible(NaturaPensione))
            //{
            //    errore = "In presenza del primo codice natura pari a '0', '6', '8', '9' è obbligatorio inserire l'Attività Usuranti sulla tab Istruttoria";
            //    return false;
            //}

            if (attivitaUsuranti.HasValue && (!IsIstruttoriaAttivitaUsurantiVisible(NaturaPensione, siglaCategoria) || IsDeleteOperation))
            {
                errore = "Eliminare i dati 'Istruttoria' prima di procedere con il salvataggio/eliminazione dei 'Dati Generici'";
                return false;
            }

            return true;
        }

        public static byte? GetCausaCaricoFromTipoDomanda(GestionePensione.DatiPensione datiPensione)
        {
            if (String.IsNullOrEmpty(datiPensione.Gruppo) || String.IsNullOrEmpty(datiPensione.Prodotto))
                return null;
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti)
                return 9;
            else
                return 1;
        }

        private static bool IsIstruttoriaAziendaVisible(string naturaPensione, string siglaCategoria, GestionePensione.DatiPensione datiPensione)
        {
            if (String.IsNullOrEmpty(naturaPensione))
                return false;

            return (naturaPensione.Substring(2, 1).Equals("O") && siglaCategoria.Trim().Equals("VO") && !Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) &&
                    !Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) && !Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione));
        }

        private static bool IsIstruttoriaAttivitaUsurantiVisible(string naturaPensione, string siglaCategoria)
        {
            if (String.IsNullOrEmpty(naturaPensione))
                return false;

            return (naturaPensione.Substring(0, 1).Equals("0") || naturaPensione.Substring(0, 1).Equals("6") || naturaPensione.Substring(0, 1).Equals("8") || naturaPensione.Substring(0, 1).Equals("9")) &&
                (siglaCategoria.Trim().Equals("VO") || siglaCategoria.Trim().Equals("VOBANC") || siglaCategoria.Trim().Equals("VOP") || siglaCategoria.Trim().Equals("VR") || siglaCategoria.Trim().Equals("VOART") ||
                siglaCategoria.Trim().Equals("VOCOM") || siglaCategoria.Trim().Equals("VDAI"));
        }

        #endregion dati Generici

        #region dati Assicurativi

        public static void GetDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, out DatiAssicurativi datiAssicurativi, out string errori)
        {
            datiAssicurativi = null;
            errori = string.Empty;
            datiAssicurativi = new DatiAssicurativi();

            Utility.ValorizzaOggetti(contenitore.DatiPensione, datiAssicurativi);
            Utility.ValorizzaOggetti(contenitore.DatiIstruttoria, datiAssicurativi);
            Utility.ValorizzaOggetti(contenitore.DatiControlloFelpe, datiAssicurativi);
            Utility.ValorizzaOggetti(contenitore.DatiPensioniDatiGenerici, datiAssicurativi);

            if (datiAssicurativi.IsDatiAssicurativiIstruttoriaNull() && datiAssicurativi.IsDatiAssicurativiPensioneNull() &&
                datiAssicurativi.IsDatiAssicurativiControlloFelpeNull() && datiAssicurativi.IsDatiAssicurativiPensioneDatiGenericiNull() &&
                (datiAssicurativi.DatiENPALS == null || datiAssicurativi.DatiENPALS.IsDatiAssicurativiEnpalsNull()))
            {
                if (contenitore.DatiPensione.IsCumuloAutomatica.GetValueOrDefault())
                {
                    ServiceReferences.TotalIvs.clsDatiCumulo risposta = null;
                    if ((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && contenitore.DatiPensione.IsCumuloAutomatica.GetValueOrDefault() &&
                    GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCO_RIC_CUMULO_AUTOMATICHE, Utility.DataSistemaAgo))
                        || (Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id) && GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCO_TRF_CUMULO_AUTOMATICHE, Utility.DataSistemaAgo)))
                    {
                        if (!GestioneTotalIvs.GetDatiCumulRicostituzioneIVS(contenitore.DatiPensione.NDomus, out risposta, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                    }
                    else
                    {
                        if (!GestioneTotalIvs.GetDatiCumulIVS(contenitore.DatiPensione.NDomus, out risposta, out errori))
                            throw new INPS.DNA.DnaValidationException(errori);
                    }

                    if (risposta.objDomanda != null)
                    {
                        if (risposta.objDomanda.DataIniAss != DateTime.MinValue)
                            datiAssicurativi.InizioAssicurazione = risposta.objDomanda.DataIniAss;

                        if (risposta.objDomanda.DataFineAss != DateTime.MinValue)
                            datiAssicurativi.FineAssicurazione = risposta.objDomanda.DataFineAss;

                        if (risposta.objDomanda.TotSettDiritto != 0)
                            datiAssicurativi.NSettimaneOBG = risposta.objDomanda.TotSettDiritto;
                    }
                }
                if (contenitore.DatiPensione.IsTotAutomatica.GetValueOrDefault())
                {
                    ServiceReferences.TotalIvs.clsDati risposta = null;

                    if (!GestioneTotalIvs.GetDatiTotalIVS(contenitore.DatiPensione.NDomus, out risposta, out errori))
                        throw new INPS.DNA.DnaValidationException(errori);

                    if (risposta.objDomanda != null)
                    {
                        if (!string.IsNullOrEmpty(risposta.objDomanda.DataIniAss))
                            datiAssicurativi.InizioAssicurazione = Utility.DataFromString(risposta.objDomanda.DataIniAss, Utility.FormatoData.GGmmAAAA);

                        if (!string.IsNullOrEmpty(risposta.objDomanda.DataFineAss))
                            datiAssicurativi.FineAssicurazione = Utility.DataFromString(risposta.objDomanda.DataFineAss, Utility.FormatoData.GGmmAAAA);

                        if (!string.IsNullOrEmpty(risposta.objDomanda.TotSettDiritto))
                            datiAssicurativi.NSettimaneOBG = Utility.StringToNullableInt(risposta.objDomanda.TotSettDiritto);
                    }
                }
            }
            //in caso di gestione Com i campi devono essere precompilati con 61 e 172 (Per gestione com si intendono le categorie: VOCOM, IOCOM, SOCOM)
            if (!datiAssicurativi.AttivitaEconomica.HasValue && !datiAssicurativi.ProfessioneIndividuale.HasValue)
            {
                if (GestioneCOM(contenitore.DatiPensione))
                {
                    datiAssicurativi.AttivitaEconomica = 61;
                    datiAssicurativi.ProfessioneIndividuale = 172;
                }
                //in caso di usuranti i campi devono essere precompilati con 67 e 011
                if (Utility.IsDomandaUsuranti(contenitore.DatiPensione))
                {
                    datiAssicurativi.AttivitaEconomica = 67;
                    datiAssicurativi.ProfessioneIndividuale = 011;
                }
            }

            if (Utility.IsDomandaESOPMI(contenitore.DatiPensione.SiglaCategoria))
            {
                datiAssicurativi.AttivitaEconomica = 10;
                datiAssicurativi.ProfessioneIndividuale = 144;
            }

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (contenitore.DatiEnpals != null)
                {
                    datiAssicurativi.DatiENPALS = new DatiAssicurativi.ENPALS();
                    Utility.ValorizzaOggetti(contenitore.DatiEnpals, datiAssicurativi.DatiENPALS);
                }
            }

            if (datiAssicurativi.IsDatiAssicurativiIstruttoriaNull() && datiAssicurativi.IsDatiAssicurativiPensioneNull() &&
                datiAssicurativi.IsDatiAssicurativiControlloFelpeNull() && datiAssicurativi.IsDatiAssicurativiPensioneDatiGenericiNull() &&
                (datiAssicurativi.DatiENPALS == null || datiAssicurativi.DatiENPALS.IsDatiAssicurativiEnpalsNull()))
                datiAssicurativi = null;
        }

        public static void StoreDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiAssicurativi datiAssicurativi,
            DatiGenerici datiGenerici, DatiBenefici datiBenefici, bool isRiapertura, bool isSingleTab, bool IsCancelOperation, DateTime? dataInizioAssicurazioneDB, DateTime? dataInizioAssicurazioneView)
        {
            if (datiAssicurativi == null)
                datiAssicurativi = new Entity.DatiAssicurativi();

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            List<GestioneDecodifica.GruppoOneri> decGruppoOnere = null;
            List<GestioneDecodifica.SottoGruppoOneri> decSottoGruppoOneri = null;
            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = contenitore.DatiIstruttoria;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioneDatiGenerici = contenitore.DatiPensioniDatiGenerici;
            GestioneEnpals.DatiEnpals datiEnpals = null;
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = null;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                datiEnpals = contenitore.DatiEnpals;
                datiControlloFelpe = contenitore.DatiControlloFelpe;
            }
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = contenitore.DatiMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            List<GestioneOneri.DatiOneri> lstDatiOneri = contenitore.ListaDatiOneri;
            GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = null;
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;

            bool assicurativiChangedPerOneri = (Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale) ^
                                     Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale));
            bool recordOneriChanged = false;
            if (assicurativiChangedPerOneri)
            {
                decGruppoOnere = contenitoreDecodifica.ElencoDecCodeGruppoOnere;
                decSottoGruppoOneri = contenitoreDecodifica.ElencoDecCodeSottoGruppoOnere;
            }

            if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, datiGenerici != null ? datiGenerici.NaturaPensione : "   "))
                datiPrepensionamento = contenitore.DatiPrepensionamento;
            //----------------------------------------------------------------

            bool isBeneficiNonVisibile = ((datiBenefici == null || datiBenefici.IsDatiBeneficiNull()) &&
                                            (
                                                Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, null) ||
                                                Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale)
                                            )) && !Utility.IsDomandaPensioneInabilita(datiPensione);

            bool isAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiapertura) != null;

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out ctrl);

            //ENG - Prepensionamento Editoria EBA: Gestione Quadro Maggiorazione Benefici
            bool isQuadroBeneficiRosso = false;
            if (Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) && (!Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
            {
                int? numeroSettimaneOBG_DB = contenitore != null && contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null;
                if (numeroSettimaneOBG_DB != datiAssicurativi.NSettimaneOBG)
                    isQuadroBeneficiRosso = true;
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiAssicurativiPerPensione(datiAssicurativi, datiPensione, datiStoricoGP, datiBenefici, isRiapertura, isSingleTab, IsCancelOperation);
                StoreDatiAssicurativiPerIstruttoria(datiPensione, datiAssicurativi, ref datiIstruttoria);
                StoreDatiAssicurativiPerPensioneDatiGenerici(datiPensione, datiAssicurativi, ref datiPensioneDatiGenerici);
                StoreDatiAssicurativiPerEnpals(datiPensione, datiAssicurativi, ref datiEnpals);
                StoreDatiAssicurativiPerControlloFelpe(datiPensione, datiAssicurativi, ref datiControlloFelpe);

                if (!Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione) &&
                    !Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale) &&
                    (datiMaggiorazioniBeneficiCommon == null ||
                    (!datiMaggiorazioniBeneficiCommon.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() && !datiMaggiorazioniBeneficiCommon.IsBeneficioApePrecociFromFELPE.GetValueOrDefault() &&
                    !Utility.IsDomandaVecchiaiaENAV(datiPensione) && datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio != "01"))
                    && !Utility.IsDomandaAnticipataFlessibile(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione)
                    && !((!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                    (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
                    && !Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)
                    && (datiMaggiorazioniBeneficiCommon != null && !Utility.IsRiaperturaRicTRF_Benefici16_17(datiPensione, datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio)))
                    GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);

                if ((IsCancelOperation || (datiAssicurativi.IsDatiAssicurativiPensioneNull() && datiAssicurativi.IsDatiAssicurativiIstruttoriaNull() && !Utility.IsDomandaENPALS(datiPensione.Gestione) && !isAnte96)) && !Utility.IsDomandaPSO(datiPensione.SiglaCategoria))
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                else if (Utility.IsDomandaPSO(datiPensione.SiglaCategoria))
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = null;
                else
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;

                if (!Utility.IsRicostituzioneOrRiaperturaAGOAutomaticaAbilitata(datiPensione, isRiapertura))
                {
                    ////////////////////////////////////////////////////////////////
                    if (isSingleTab)
                    {
                        if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, null)
                            || Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale))
                        {
                            if ((datiBenefici == null || datiBenefici.IsDatiBeneficiNull()) || (datiQuadroMaggiorazioniBenefici.TabBenefici != 2))
                                datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                            else
                                datiQuadroMaggiorazioniBenefici.TabBenefici = 2;
                        }
                        else
                        {
                            if (isBeneficiNonVisibile)
                            {
                                datiQuadroMaggiorazioniBenefici.TabBenefici = null;
                            }
                        }
                    }

                    if (IsCancelOperation)
                    {
                        if (datiPensione.Benefici.GetValueOrDefault())
                            if ((datiBenefici == null || datiBenefici.IsDatiBeneficiNull()) || (datiQuadroMaggiorazioniBenefici.TabBenefici != 2))
                                datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                            else
                                datiQuadroMaggiorazioniBenefici.TabBenefici = 2;
                        else
                            datiQuadroMaggiorazioniBenefici.TabBenefici = null;
                    }

                    if (datiGenerici != null && ((datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value && datiQuadroMaggiorazioniBenefici.TabExCombattente == 2) ||
                        (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value && datiQuadroMaggiorazioniBenefici.TabBenefici == 2) ||
                        (datiGenerici.Maggiorazioni.HasValue && datiGenerici.Maggiorazioni.Value && datiQuadroMaggiorazioniBenefici.TabMaggiorazioni == 2)) ||

                        ((Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null)) && datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo == 2))
                        datiQuadroMaggiorazioniBenefici.Tipo = 2;
                    if (datiQuadroMaggiorazioniBenefici.TabExCombattente == 0 || datiQuadroMaggiorazioniBenefici.TabBenefici == 0 || datiQuadroMaggiorazioniBenefici.TabMaggiorazioni == 0 || datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo == 0)
                        datiQuadroMaggiorazioniBenefici.Tipo = 1;
                    if (!datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && !datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && !datiQuadroMaggiorazioniBenefici.TabMaggiorazioni.HasValue && !datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.HasValue)
                        datiQuadroMaggiorazioniBenefici.Tipo = 0;

                    //ENG - Prepensionamento Editoria EBA: Gestione Quadro Maggiorazione Benefici
                    if (isQuadroBeneficiRosso)
                    {
                        datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                    }

                    GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                    /////////////////////////////////////////////////////////////////
                }

                if (!Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiapertura) && !Utility.IsDomandaBancari(datiPensione.SiglaCategoria))
                {
                    #region Gestione Semaforo Oneri

                    if (assicurativiChangedPerOneri)
                    {
                        GestioneOneri.DatiOneri datiOneriAmianto181 = lstDatiOneri != null ? lstDatiOneri.Where(x => x.IdCodeGruppo == decGruppoOnere.Find(y => y.Code == "2000").Id && x.IdCodeSottoGruppo == decSottoGruppoOneri.Find(y => y.Code == "2010").Id).FirstOrDefault() : null;
                        if (Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale) && datiOneriAmianto181 == null)
                        {
                            GestioneOneri.DatiOneri newOneri = new GestioneOneri.DatiOneri { IdCodeGruppo = decGruppoOnere.Find(y => y.Code == "2000").Id, IdCodeSottoGruppo = decSottoGruppoOneri.Find(y => y.Code == "2010").Id, Decorrenza = datiPensione.DecorrenzaOriginaria, IdPensione = datiPensione.Id };
                            GestioneOneri.SalvaOneriOnere(newOneri);
                            recordOneriChanged = true;
                        }
                        else if (!Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale) && datiOneriAmianto181 != null)
                        {
                            GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                            lstDatiOneri.Where(x => x.IdCodeGruppo != decGruppoOnere.Find(y => y.Code == "2000").Id && x.IdCodeSottoGruppo != decSottoGruppoOneri.Find(y => y.Code == "2010").Id)
                                .ToList()
                                .ForEach(x => GestioneOneri.SalvaOneriOnere(x));
                            recordOneriChanged = true;
                        }
                    }

                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

                    //visibilità tab prepensionamento
                    #region Gestione visibilità tab prepensionamento
                    if (!(Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                        Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) ||
                        Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                        Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                        Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione))
                        && tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti && tipoDomanda != Utility.TipoDomanda.Ripristino && !isRiapertura)
                    {
                        if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, datiGenerici != null ? datiGenerici.NaturaPensione : "   "))
                            if ((datiPrepensionamento == null || datiPrepensionamento.IsDatiPrepensionamentoNull()) || (datiQuadroOneri.TabPrepensionamento != 2))
                                datiQuadroOneri.TabPrepensionamento = 0;
                            else
                            {

                                datiQuadroOneri.TabPrepensionamento = 2;
                            }
                        else
                        {
                            datiQuadroOneri.TabPrepensionamento = null;
                        }
                    }
                    else
                    {
                        datiQuadroOneri.TabPrepensionamento = null;
                    }
                    #endregion Gestione visibilità tab prepensionamento

                    if (tipoDomanda != Utility.TipoDomanda.Ripristino)
                    {
                        if ((Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                            Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                            Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                            Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) ||
                            Utility.IsDomandaAPEPrecoci(datiPensione))
                           || Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale)
                           || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null)
                           || Utility.IsOneriSperDonnaVisibili(datiPensione, isRiapertura, datiPensione.DecorrenzaOriginaria, datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, lstDatiOneri)
                           || Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione)
                           || Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione)
                           || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione)
                           || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) && !Utility.IsDomandaMIN(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                           || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione)
                           || Utility.IsDomandaVecchiaiaENAV(datiPensione) || (datiBenefici != null && datiBenefici.TipoSettimaneBeneficio == "01")
                           || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                           || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione)
                           || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                           || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                           || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                           || (Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                           || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                        {
                            if (((datiQuadroOneri.TabOneri == null) || (datiQuadroOneri.TabOneri == 2 && recordOneriChanged)) && !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione))))
                                datiQuadroOneri.TabOneri = 0; //rosso
                        }
                        else
                            datiQuadroOneri.TabOneri = null;

                        if (//condizioni visibilità oneri
                           (Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                            Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                            Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                            Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) ||
                            Utility.IsDomandaAPEPrecoci(datiPensione))
                           || Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale)
                           || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null)
                           || Utility.IsOneriSperDonnaVisibili(datiPensione, isRiapertura, datiPensione.DecorrenzaOriginaria, datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, lstDatiOneri)
                           || Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione)
                           || Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione)
                           || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione)
                           || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) && !Utility.IsDomandaMIN(datiPensione.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                           || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione)
                           || Utility.IsDomandaVecchiaiaENAV(datiPensione) || (datiBenefici != null && datiBenefici.TipoSettimaneBeneficio == "01") || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                           || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                           || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                           || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                           || (Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) && datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2018, 12, 31)))
                           || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)
                           ||
                            //condizioni visibilità prepensionamento
                           (Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, (datiGenerici != null ? datiGenerici.NaturaPensione : null)) && !Utility.IsDomandaSalvaguardia122(datiPensione)
                               && tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti && tipoDomanda != Utility.TipoDomanda.Ripristino && !isRiapertura)
                           || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione))
                        {
                            if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione))))
                                datiQuadroOneri.Tipo = 2;//visibile 
                        }
                        else
                            datiQuadroOneri.Tipo = 0;

                        GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                    }
                    #endregion Gestione Semaforo Oneri
                }

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);

                //ENG - VOPGI           
                if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione))
                {
                    if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0017") ||
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0017") ||
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0001") ||
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0001"))
                    {
                        if (dataInizioAssicurazioneDB != dataInizioAssicurazioneView)
                        {
                            datiQuadroDatiContributivi.TabDatiCalcolo = 1;
                            datiQuadroDatiContributivi.TabQuotaFondoINPGI = 0;
                            //dobbiamo eliminare i dati calcolo perchè il tab Dati Calcolo diventa giallo e le griglie contributive/retributive si vedono in base al tipo calcolo o in base all'inizio assicurazione. Necessario
                            //per evitare che dei dati precedentemente inseriti rimangano sul database e non siano corretti
                            GestioneCalcolo.EliminaCalcoloContributivoByIdPensione(datiPensione.Id, false);
                            GestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(datiPensione.Id, false);
                        }
                    }
                }

                if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) &&
                    !((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2") &&
                    datiAssicurativi.FineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(datiAssicurativi.FineAssicurazione.Value, new DateTime(2022, 06, 30)))
                {
                    datiQuadroDatiContributivi.TabDatiCalcolo = 0;
                    datiQuadroDatiContributivi.Tipo = 2;
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiIstruttoria = datiIstruttoria;
            contenitore.DatiPensioniDatiGenerici = datiPensioneDatiGenerici;
            contenitore.DatiEnpals = datiEnpals;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            contenitore.DatiQuadroOneri = datiQuadroOneri;
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            //--------------------------------------------------------------------
        }

        private static void StoreDatiAssicurativiPerPensioneDatiGenerici(GestionePensione.DatiPensione datiPensione, DatiAssicurativi datiAssicurativi, ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioneDatiGenerici)
        {
            if (datiPensioneDatiGenerici == null)
            {
                if (datiAssicurativi.IsDatiAssicurativiPensioneDatiGenericiNull() && !(Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) ||
                    Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(datiPensione)))
                    return;
                else
                    datiPensioneDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            }

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
            {
                datiAssicurativi.ImportoUltimaRetribuzione = datiPensioneDatiGenerici.ImportoUltimaRetribuzione;
                datiAssicurativi.InizioUltimoLavoro = datiPensioneDatiGenerici.InizioUltimoLavoro;
                datiAssicurativi.FineUltimoLavoro = datiPensioneDatiGenerici.FineUltimoLavoro;
            }

            if (Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) ||
                Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(datiPensione))
            {
                datiPensioneDatiGenerici.CodiceConvenzioneAgo = datiAssicurativi.CodiceConvenzioneAgo;
            }

            Utility.ValorizzaOggetti(datiAssicurativi, datiPensioneDatiGenerici);
            if (datiPensioneDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
            else
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioneDatiGenerici);
        }

        private static void StoreDatiAssicurativiPerPensione(DatiAssicurativi datiAssicurativi, GestionePensione.DatiPensione datiPensione, GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP,
            DatiBenefici datiBenefici, bool isRiaperturaDomanda, bool isSingleTab, bool isCancelOperation)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiAssicurativi.InizioAssicurazione = datiPensione.InizioAssicurazione;
                datiAssicurativi.FineAssicurazione = datiPensione.FineAssicurazione;

                datiAssicurativi.AttivitaEconomicaFELPE = datiPensione.AttivitaEconomicaFELPE;
                datiAssicurativi.ProfessioneIndividualeFELPE = datiPensione.ProfessioneIndividualeFELPE;
            }

            if (Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                if (!Utility.IsDomandaReversibilita(datiPensione) &&
                    !(Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione && (!Utility.IsRicostituzione_MotiviContributivi(datiPensione) ||
                    Utility.IsRicostituzioneContributivaPerEsecuzioneSentenza(datiPensione))) &&
                    !(Utility.IsDomandaPensioneIndiretta(datiPensione) && (datiStoricoGP != null && (!datiStoricoGP.InizioAssicurazione.HasValue || !datiStoricoGP.FineAssicurazione.HasValue))) &&
                    !Utility.IsEnpalsManualePL(true, Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda), datiPensione.IsDatiENPALSRecuperati))
                {
                    datiAssicurativi.InizioAssicurazione = datiPensione.InizioAssicurazione;
                    datiAssicurativi.FineAssicurazione = datiPensione.FineAssicurazione;
                }
            }

            if (datiPensione.Amianto181Unicarpe == true)
            {
                datiAssicurativi.AttivitaEconomica = datiPensione.AttivitaEconomica;
                datiAssicurativi.ProfessioneIndividuale = datiPensione.ProfessioneIndividuale;
            }

            if (datiPensione.IsCumuloAutomatica.GetValueOrDefault())
            {
                datiAssicurativi.InizioAssicurazione = datiPensione.InizioAssicurazione;
                datiAssicurativi.FineAssicurazione = datiPensione.FineAssicurazione;
            }

            if (isSingleTab)
            {
                if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, null)
                    || Utility.IsDomandaConBeneficioAmianto181(datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale))
                {
                    datiPensione.Benefici = true;
                }
                else
                {
                    if ((datiBenefici == null || datiBenefici.IsDatiBeneficiNull()) &&
                        (
                            Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, null) ||
                            Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale)
                        ) && !Utility.IsDomandaPensioneInabilita(datiPensione))
                    {
                        datiPensione.Benefici = null;
                    }
                }
            }

            if (isCancelOperation &&
              (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, null) ||
              Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale)) && !Utility.IsDomandaPensioneInabilita(datiPensione))
            {
                if (datiPensione.Benefici.HasValue && datiPensione.Benefici.Value)
                {
                    if (datiBenefici == null || datiBenefici.IsDatiBeneficiNull())
                    {
                        datiPensione.Benefici = null;
                    }
                }
            }

            Utility.ValorizzaOggetti(datiAssicurativi, datiPensione);
            GestionePensione.SalvaPensione(datiPensione);
        }

        private static void StoreDatiAssicurativiPerIstruttoria(GestionePensione.DatiPensione datiPensione, Entity.DatiAssicurativi datiAssicurativi, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiAssicurativi.IsDatiAssicurativiIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }


            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiAssicurativi.NContributiVolontari = datiIstruttoria.NContributiVolontari;
                datiAssicurativi.NSettimaneOBG = datiIstruttoria.NSettimaneOBG;
                datiAssicurativi.NSettimaneOI = datiIstruttoria.NSettimaneOI;
                datiAssicurativi.NContributiVVAnzianita = datiIstruttoria.NContributiVVAnzianita;
            }

            if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOAUT" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOAUT")
            {
                datiAssicurativi.NContributiVolontari = datiIstruttoria.NContributiVolontari;
                datiAssicurativi.NContributiVVAnzianita = datiIstruttoria.NContributiVVAnzianita;
            }

            if (datiPensione.IsCumuloAutomatica.GetValueOrDefault())
            {
                datiAssicurativi.NSettimaneOBG = datiIstruttoria.NSettimaneOBG;
                datiAssicurativi.NSettimaneOI = datiIstruttoria.NSettimaneOI;
            }

            Utility.ValorizzaOggetti(datiAssicurativi, datiIstruttoria);
            datiIstruttoria.NContributiUtiliLavoratoriAutonomi = datiAssicurativi.NSettimaneOBG + datiAssicurativi.NContributiVolontari;

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(datiPensione.Id);
            else
                GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);
        }

        private static void StoreDatiAssicurativiPerEnpals(GestionePensione.DatiPensione datiPensione, Entity.DatiAssicurativi datiAssicurativi, ref GestioneEnpals.DatiEnpals datiEnpals)
        {
            if (datiEnpals == null)
            {
                if (datiAssicurativi.DatiENPALS == null || datiAssicurativi.DatiENPALS.IsDatiAssicurativiEnpalsNull())
                    return;
                else
                    datiEnpals = new GestioneEnpals.DatiEnpals();
            }

            Utility.ValorizzaOggetti(datiAssicurativi.DatiENPALS, datiEnpals);

            if (datiEnpals.Equals(new GestioneEnpals.DatiEnpals()))
                GestioneEnpals.EliminaDatiEnpalsByIdPensione(datiPensione.Id);
            else
            {
                datiEnpals.IdPensione = datiPensione.Id;
                GestioneEnpals.SalvaDatiEnpalsEnpals(datiEnpals);
            }
        }

        private static void StoreDatiAssicurativiPerControlloFelpe(GestionePensione.DatiPensione datiPensione, Entity.DatiAssicurativi datiAssicurativi, ref GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe)
        {
            if (!Utility.IsDomandaENPALS(datiPensione.Gestione))
                return;
            if (datiControlloFelpe == null)
            {
                if (datiAssicurativi.IsDatiAssicurativiControlloFelpeNull())
                    return;
                else
                    datiControlloFelpe = new GestioneDatiControlloFelpe.ControlloFelpe();
            }
            Utility.ValorizzaOggetti(datiAssicurativi, datiControlloFelpe);
            GestioneDatiControlloFelpe.SalvaDatiControlloFelpe(datiPensione.Id, datiControlloFelpe);
        }

        public static bool ControlDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            bool IsSingleTab, bool isRiaperturaDomanda, DateTime dataSistema, DatiAssicurativi datiAssicurativi, DatiGenerici datiGenerici, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<GestioneCalcolo.QuotePensione> lQuotePensione = null;
            GestioneDanteCausa.DatiDanteCausa datiDA = null;
            GestioneEnpals.DatiEnpals datiEnpals = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                datiDA = contenitore.DatiDanteCausa;
            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
                lQuotePensione = contenitore.ListaQuotePensione;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                datiEnpals = contenitore.DatiEnpals;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);

            if (datiAssicurativi == null)
                return true;

            DateTime? inizioBonus = Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) ? datiAssicurativi.InizioBonus : (contenitore.DatiControlloFelpe != null ? contenitore.DatiControlloFelpe.InizioBonus : null);

            if (!(Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
                && !(Utility.IsDomandaSOBANC(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaIOBANC(contenitore.DatiPensione.SiglaCategoria))
                && Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) == null)
            {
                if (!ControlDatiAssicurativiCommon(contenitore.DatiPensione, datiAssicurativi, datiGenerici, contenitore.DatiMaggiorazioniBenefici, contenitore.DatiPrepensionamento, datiDA, tipoDomanda,
                    contenitore.DatiAnagraficiTitolare, dataSistema, isRiaperturaDomanda, contenitoreDecodifica != null ? contenitoreDecodifica.ElencoCtrlEnteCassaCodiceGestione : null, contenitoreDecodifica != null ? contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsInizioFineAssicurazione(datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione,
                    datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty,
                    datiDA != null ? datiDA.ProvenienzaPensione : null,
                    datiDA != null ? datiDA.DecorrenzaPensione : null,
                    datiGenerici != null ? datiGenerici.CodiceLiquidazione : null, contenitore.DatiPensione, contenitore.DatiPensioniDatiGenerici,
                    lQuotePensione, contenitore.TipoCalcolo, isRiaperturaDomanda, false, inizioBonus, contenitore.ListaDatiContributivi, ref contenitoreDecodifica,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.FacoltaComputo : null, datiAssicurativi.NSettimaneOBG, contenitore.DatiAnagraficiTitolare.DataNascita, out messaggioVideo))
                    return false;
            }

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (datiAssicurativi.NContributiVVAnzianita.GetValueOrDefault() > 3000)
                {
                    messaggioVideo = "Il numero contributi volontari per anzianità non può essere superiore a 3000";
                    return false;
                }
            }
            else
            {
                byte? tipoCalcolo = datiGenerici != null ? datiGenerici.TipoCalcolo : null;

                string codiceBancaEsodatiTraduzioneSuGP = "0";
                if (contenitore.DatiPensione != null && Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (contenitore.DatiPensione.CodiceBancaEsodati.HasValue)
                    {
                        if (contenitoreDecodifica.ElencoDecAzienda != null && contenitoreDecodifica.ElencoDecAzienda.Count > 0)
                        {
                            short codiceBancaEsodati = contenitore.DatiPensione.CodiceBancaEsodati.Value;
                            GestioneDecodificaAzienda.DecAzienda decAzienda = contenitoreDecodifica.ElencoDecAzienda.Find(x => x.Id == codiceBancaEsodati);
                            if (decAzienda != null)
                                codiceBancaEsodatiTraduzioneSuGP = decAzienda.TraduzioneSuGP;
                        }
                    }
                }


                if (!GestioneControlli.ControlsNSettimaneOBG(datiAssicurativi.NSettimaneOBG.HasValue ? datiAssicurativi.NSettimaneOBG.Value : 0,
                        datiAssicurativi.NContributiVolontari.HasValue ? datiAssicurativi.NContributiVolontari.Value : 0,
                        datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty,
                        datiGenerici != null ? datiGenerici.CodiceLiquidazione : null, tipoCalcolo, datiAssicurativi.InizioAssicurazione, contenitore.DatiPensione, contenitore.DatiDanteCausa,
                        contenitore.DatiAnagraficiTitolare, contenitore.DatiBeneficioVittimeTerrorismo, contenitore.DatiPensioniDatiGenerici, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceAziendaEditoria : null,
                        contenitore.ListaDatiBeneficiParticolari, contenitoreDecodifica.ElencoDecodAnagraficaAccordi, contenitoreDecodifica != null ? contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale : null,
                        datiGenerici != null ? datiGenerici.Contributivo : null, isRiaperturaDomanda, contenitore.DatiMaggiorazioniBenefici, out messaggioVideo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null,
                        codiceBancaEsodatiTraduzioneSuGP, datiAssicurativi.NSettimaneOI.HasValue ? datiAssicurativi.NSettimaneOI.Value : 0))
                    return false;

                if (!GestioneControlli.ControlsNContributiVolontari(datiAssicurativi.NContributiVolontari.HasValue ? datiAssicurativi.NContributiVolontari.Value : 0,
                        datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, contenitore.DatiPensione, isRiaperturaDomanda, codiceBancaEsodatiTraduzioneSuGP, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null,
                        datiAssicurativi.NContributiVVAnzianita.HasValue ? datiAssicurativi.NContributiVVAnzianita.Value : 0, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsNContributiVVAnzianita(datiAssicurativi.NContributiVVAnzianita.HasValue ? datiAssicurativi.NContributiVVAnzianita.Value : 0,
                        datiAssicurativi.NContributiVolontari.HasValue ? datiAssicurativi.NContributiVolontari.Value : 0,
                        datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, contenitore.DatiPensione, isRiaperturaDomanda, codiceBancaEsodatiTraduzioneSuGP, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null, out messaggioVideo))
                    return false;


                if (!GestioneControlli.ControlsInizioFineUltimoLavoro(datiAssicurativi.InizioUltimoLavoro, datiAssicurativi.FineUltimoLavoro,
                    datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                    if (!GestioneControlli.ControlsFineAssicurazioneForDatiContributivi(tipoCalcolo, datiAssicurativi.FineAssicurazione, contenitore.ListaDatiContributivi,contenitore.ListaDatiRetributivi, Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione), Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true), null, false) 
                        && !Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                    {
                        messaggioVideo = "I dati calcolo salvati sono incongruenti con la data 'Fine Assicurazione'; cancellare i dati calcolo prima di proseguire";
                        return false;
                    }
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
            {
                int? NSettimane = datiAssicurativi.NSettimaneOBG;
                if (Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                    NSettimane += datiAssicurativi.NSettimaneOI;

                if (!GestioneControlli.ControlsDatiAssicurativiPerCumulo(contenitore.DatiPensione, NSettimane, datiAssicurativi.NContributiVolontari,
                    contenitore.DatiAnagraficiTitolare.DataNascita, contenitore.DatiMaggiorazioniBenefici, out messaggioVideo))
                    return false;
            }

            if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!GestioneControlli.ControlsDatiAssicurativiPerVAPE(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, contenitore.DatiPensioniDatiGenerici, datiAssicurativi.NSettimaneOBG,
                    datiAssicurativi.NContributiVolontari, out messaggioVideo))
                    return false;
            }

            if (Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione))
            {
                int numSettimaneTipoContibutivo = 0;

                if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                {
                    //ENPALS = ANNI * 52 + MESI * 4,333
                    double numSettimaneDaAnni = datiEnpals != null ? datiEnpals.AADiritto.GetValueOrDefault() * 52 : 0;
                    double numSettimaneDaMesi = datiEnpals != null ? datiEnpals.MMDiritto.GetValueOrDefault() * 4.333 : 0;

                    numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaAnni);
                    numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaMesi);
                }
                else
                    numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + datiAssicurativi.NSettimaneOBG.GetValueOrDefault(); //SETTIMANE OBG DIRITTO

                if (!GestioneControlli.ControlsDatiAssicurativiPerAPEPrecoci(contenitore.DatiPensione, datiAssicurativi.InizioAssicurazione, numSettimaneTipoContibutivo, datiAssicurativi.NContributiVolontari,
                    datiEnpals, out messaggioVideo))
                    return false;
            }

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(contenitore.DatiPensione, datiGenerici != null ? datiGenerici.NaturaPensione : null, datiAssicurativi.InizioAssicurazione,
                out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsRequisitoEta_VOAUT(contenitore.DatiPensione, contenitore.DatiMaggiorazioniBenefici, contenitore.DatiAnagraficiTitolare.DataNascita, contenitore.DatiAnagraficiTitolare.Sesso,
                datiAssicurativi.NSettimaneOBG, datiGenerici != null ? datiGenerici.NaturaPensione : null, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null, out messaggioVideo))
                return false;

            //ENG - VOPGI NO AGI
            if (!GestioneControlli.ControllaInizioAssicurazioneTipoCalcoloVOPGINOAGI(contenitore.DatiPensione, datiGenerici != null ? datiGenerici.TipoCalcolo : null, datiAssicurativi != null ? datiAssicurativi.InizioAssicurazione : null, out messaggioVideo))
                return false;

            //ENG - Memo74_2023: bloccare il salvataggio del tab Assicurativi se non è valorrizzato il Codice Convenzione.
            GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo74_2023", out ctrlMemo74_2023);
            if ((ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI" && Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria)) ||
                //ENG - Memo 116/2025
                Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione) ||
                Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione))
            {
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioniEstere);

                if ((!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && (contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "V" || contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "Z")) ||
                    (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0))
                {
                    if (!datiAssicurativi.CodiceConvenzioneAgo.HasValue)
                    {
                        messaggioVideo = "Codice Convenzione Obbligatorio";
                        return false;
                    }

                    if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0)
                    {
                        List<GestioneContrib.StatoEsteroCumulo> listaStatiEsteri = null;
                        GestioneContrib.GetStatiEEfromDBByIdPensione(contenitore.DatiPensione.Id, listaPrestazioniEstere, out listaStatiEsteri);
                        if (listaStatiEsteri != null && listaStatiEsteri.Count() > 0)
                        {
                            if (!GestioneControlli.VerificaCodiceConvenzioneWithStatoEstero(contenitore.DatiPensione.DecorrenzaOriginaria, listaStatiEsteri[0].PrestazioneEsteraCumulo.CodiceStato, datiAssicurativi.CodiceConvenzioneAgo))
                            {
                                messaggioVideo = "Codice Convenzione errato o incompatibile con Stato " + listaStatiEsteri[0].PrestazioneEsteraCumulo.NomeStato;
                                return false;
                            }
                        }
                    }
                }
            }

            //ENG- Memo 68/2022 aggiornato al 12/03/2025
            if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) &&
                datiAssicurativi.FineAssicurazione.HasValue && !Utility.DataStrettamenteSuccessivaA(datiAssicurativi.FineAssicurazione.Value, new DateTime(2022, 06, 30)))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneModificheMemoINPGI_20250312 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20250312 ", out ctrlAbilitazioneModificheMemoINPGI_20250312);
                if (ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI")
                {
                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContr = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.ToList();
                    if (contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count() > 0)
                    {
                        foreach (GestioneCalcolo.DatiCalcoloContributivo datiContributivi in contenitore.ListaDatiContributivi)
                        {
                            foreach (GestioneDecodifica.CodeGestioneCalcoloContributivo decCodeGestione in elencoCodeGestioneCalcoloContr)
                            {
                                if (decCodeGestione.Id == datiContributivi.CodiceGestione && decCodeGestione.TraduzioneSuGP == "FB")
                                {
                                    messaggioVideo = "Eliminare i dati calcolo prima dell’inserimento della data fine assicurazione <= 30/06/2022.";
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static bool ControlDatiAssicurativiCommon(GestionePensione.DatiPensione datiPensione, DatiAssicurativi datiAssicurativi, DatiGenerici datiGenerici,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggBen, GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento,
            GestioneDanteCausa.DatiDanteCausa datiDA, Utility.TipoDomanda tipoDomanda, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, DateTime dataSistema,
            bool isRiaperturaDomanda, List<GestioneDecodifica.CtrlEnteCassaCodiceGestione> ElencoCtrlEnteCassaCodiceGestione,
            List<GestioneDecodifica.DecodificaEnteCassaProfessionale> ElencoDecodificaEnteCassaProfessionale, out string messaggioVideo)
        {
            if (!(Utility.IsDomandaSPED(datiPensione) && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)) &&
                !(Utility.IsDomandaENPALS(datiPensione.Gestione) && datiDA != null && datiDA.DecorrenzaPensione.HasValue && !Utility.DataSuccessivaA(datiDA.DecorrenzaPensione.Value, new DateTime(1995, 1, 1))) &&
                !((Utility.IsDomandaVOST(datiPensione.SiglaCategoria) || Utility.IsRenditaCasalinghe(datiPensione) || Utility.IsRenditaFacoltativa(datiPensione)) && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)) && !Utility.IsDomandaPSO(datiPensione.SiglaCategoria))
            {
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
            }

            if (datiAssicurativi.InizioAssicurazione.HasValue && datiAssicurativi.FineAssicurazione.HasValue)
            {
                if (Utility.IsDomandaSupplementare(datiPensione) && Utility.IsDomandaENPALS(datiPensione.Gestione))
                {
                    if (!Utility.DataSuccessivaA(datiAssicurativi.FineAssicurazione.Value, datiAssicurativi.InizioAssicurazione.Value))
                    {
                        messaggioVideo = "La data di Inizio Assicurazione deve essere antecedente alla data di Fine Assicurazione";
                        return false;
                    }
                }
                else
                {
                    if (!Utility.DataStrettamenteSuccessivaA(datiAssicurativi.FineAssicurazione.Value, datiAssicurativi.InizioAssicurazione.Value) &&
                        !(Utility.IsDomandaVOAUT(datiPensione.SiglaCategoria) && Utility.IsDomandaAutomatica(datiPensione)))
                    {
                        messaggioVideo = "La data di Inizio Assicurazione deve essere antecedente alla data di Fine Assicurazione";
                        return false;
                    }
                }
            }
            //if (datiAssicurativi.InizioAssicurazione.Value.CompareTo(datiAssicurativi.FineAssicurazione.Value) > 0)
            //{
            //    messaggioVideo = "La data di Inizio Assicurazione deve essere antecedente alla data di Fine Assicurazione";
            //    return false;
            //}


            //20141126 - Per ENPALS non esistono i campi AttivitaEconomica e ProfessioneIndivisuale
            if (!Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                if (!GestioneControlli.ControlsAttivitaEconomica(datiAssicurativi.AttivitaEconomica,
                    datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty,
                    datiAssicurativi.ProfessioneIndividuale,
                    datiGenerici != null ? datiGenerici.CodiceLiquidazione : null, datiPensione, isRiaperturaDomanda, datiDA, out messaggioVideo))
                    return false;

                var idEnteCassa = datiGenerici != null ? datiGenerici.EnteCassa : null;
                var enteCassa = ElencoDecodificaEnteCassaProfessionale != null ? ElencoDecodificaEnteCassaProfessionale.Where(x => x.Id == idEnteCassa).Select(x => x.TraduzioneSuGP).FirstOrDefault() : null;
                enteCassa = !string.IsNullOrEmpty(enteCassa) ? enteCassa.ToString().PadLeft(4, '0') : enteCassa;
                if (!GestioneControlli.ControlsProfessioneIndividuale(datiAssicurativi.ProfessioneIndividuale, datiAssicurativi.AttivitaEconomica,
                    datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty,
                    datiGenerici != null ? datiGenerici.CodiceLiquidazione : null,
                    datiPensione, ElencoCtrlEnteCassaCodiceGestione, enteCassa, isRiaperturaDomanda, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.ControlsRequisiti(datiAssicurativi.RequisitiVecchiaiaAl1294, datiAssicurativi.RequisitiAl1294, datiAssicurativi.RequisitiAl996,
                    datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, datiPensione, isRiaperturaDomanda, out messaggioVideo))
                return false;

            //controllo attivita economica e professione individuae con beneficio
            if (datiMaggBen != null && !string.IsNullOrEmpty(datiMaggBen.TipoSettimaneBeneficio))
            {
                if (!GestioneControlli.ControlsBeneficioWithAttEconomicaProfessioneInd(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale,
                    datiMaggBen.TipoSettimaneBeneficio, isRiaperturaDomanda, out messaggioVideo))
                    return false;
            }

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (!GestioneCrossControls.ALL_VerificaFineAssicurazioneForReversibilita(tipoDomanda, datiAssicurativi.FineAssicurazione, datiPensione.DecorrenzaOriginaria, datiDA != null ? datiDA.DecorrenzaPensione : null, tipoAppartenenza, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!ControlsDatiGenericiAssicurativiForDatiPrepensionamento(datiPensione, datiMaggBen, datiPrepensionamento, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, (datiGenerici != null) ? datiGenerici.NaturaPensione : null, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsInabilitaWithAttivitaEconomicaAndProfessioneIndividuale(datiPensione, datiAssicurativi.AttivitaEconomica, datiAssicurativi.ProfessioneIndividuale, (datiGenerici != null) ? datiGenerici.EnteCassa : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_ControlsInizioAssicurazioneSperimentaleDonna(datiPensione, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                return false;

            int? NSettimaneOBG = (datiAssicurativi.NSettimaneOBG.HasValue ? datiAssicurativi.NSettimaneOBG.Value : 0);
            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                NSettimaneOBG += (datiAssicurativi.NSettimaneOI.HasValue ? datiAssicurativi.NSettimaneOI.Value : 0);
            }
            if (!GestioneControlli.VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, NSettimaneOBG, datiAssicurativi.NContributiVolontari, datiAssicurativi.DatiENPALS, datiAnagraficiTitolare, dataSistema, out messaggioVideo))
                return false;

            //if (!GestioneControlli.VerificaDataPerfezionamentoPerTrasfAOI(datiPensione, datiAssicurativi.NSettimaneOBG, datiAssicurativi.NContributiVolontari, datiAssicurativi.DatiENPALS, datiAnagraficiTitolare, dataSistema, out messaggioVideo))
            //    return false;

            return true;
        }

        public static void EliminaDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiBenefici datiBenefici,
            bool isRiapertura, out string msgVideo)
        {
            msgVideo = string.Empty;
            DatiGenerici datiGenerici = null;

            GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiapertura, out datiGenerici, out msgVideo);
            if (!string.IsNullOrEmpty(msgVideo))
                return;

            if ((contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count > 0) ||
                (contenitore.ListaDatiRetributivi != null && contenitore.ListaDatiRetributivi.Count > 0))
            {
                msgVideo = "Eliminare i 'Dati Calcolo' prima di continuare.";
                return;
            }

            if (!ControlsDatiGenericiAssicurativiForDatiPrepensionamento(contenitore.DatiPensione, contenitore.DatiMaggiorazioniBenefici, contenitore.DatiPrepensionamento, null, null,
                datiGenerici != null ? datiGenerici.NaturaPensione : null, out msgVideo))
                return;

            StoreDatiAssicurativi(ref contenitore, ref contenitoreDecodifica, new DatiAssicurativi(), datiGenerici, datiBenefici, isRiapertura, true, true, contenitore.DatiPensione != null && contenitore.DatiPensione.InizioAssicurazione.HasValue ? contenitore.DatiPensione.InizioAssicurazione : null, null);
        }

        private static bool ControlsDatiGenericiAssicurativiForDatiPrepensionamento(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon,
            GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento, int? attivitaEconomica, int? professioneIndividuale, string naturaPensione, out string msgVideo)
        {
            msgVideo = string.Empty;

            int codiceLeggeDb = 0;
            string tipoBeneficioDb = string.Empty;
            Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione, out codiceLeggeDb, out tipoBeneficioDb);

            string tipoBeneficioEntity = string.Empty;
            int codiceLeggeEntity = 0;
            Utility.IsTabPrepensionamentoVisible(datiPensione, attivitaEconomica, professioneIndividuale, naturaPensione, out codiceLeggeEntity, out tipoBeneficioEntity);
            if (string.IsNullOrEmpty(tipoBeneficioEntity))
            {
                if (Utility.IsDomandaConBeneficioAmianto181(attivitaEconomica, professioneIndividuale))
                {
                    tipoBeneficioEntity = "04";
                }
            }

            if (!string.IsNullOrEmpty(tipoBeneficioEntity))
            {
                if ((datiMaggiorazioniBeneficiCommon != null && !string.IsNullOrEmpty(datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio)) &&
                    (datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio != tipoBeneficioEntity))
                {
                    msgVideo = "Eliminare i dati 'Benefici' in 'Maggiorazione Benefici' prima di continuare";
                    return false;
                }
            }

            if ((datiPrepensionamento != null) && (codiceLeggeDb != codiceLeggeEntity))
            {
                msgVideo = "Eliminare i dati 'Prepensionamento' in 'Oneri' prima di continuare";
                return false;
            }

            return true;
        }

        #endregion dati Assicurativi

        #region dati Istruttoria

        public static void GetDatiIstruttoria(ref EntityBLCommon.ContenitoreObject contenitore, ref ContenitoreDecodifica contenitoreDecodifica, out DatiIstruttoria datiIstruttoriaEntity)
        {
            datiIstruttoriaEntity = null;

            if (contenitore.DatiPensione == null)
                return;

            if (contenitore.DatiIstruttoria == null && contenitore.DatiPensioniDatiGenerici == null && contenitore.DatiMaggiorazioniBenefici == null &&
                (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) || contenitore.DatiEnpals == null))
                return;

            datiIstruttoriaEntity = new DatiIstruttoria();

            Utility.ValorizzaOggetti(contenitore.DatiPensione, datiIstruttoriaEntity);
            Utility.ValorizzaOggetti(contenitore.DatiIstruttoria, datiIstruttoriaEntity);
            string codiceFiscale = contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale;
            if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione) && datiIstruttoriaEntity.CodiceAziendaEditoriaPerTipo0179 == null)
            {
                CtrlCodiciFiscaliAbilitatiPerTipo0179 ctrlCodiciFiscaliAbilitatiPerTipo0179 = null;
                GestioneCtrlCodiciFiscaliAbilitatiPerTipo0179.GetAbilitazionePerTipo0179byCodiceFiscale(contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale,
                    out ctrlCodiciFiscaliAbilitatiPerTipo0179);

                List<Entity.DecAnagraficaAccordiPerTipo0179> listaAccordiPerTipo0179 = null;
                GestioneLiquidazionePensione.GetAnagraficaAccordi(ref contenitoreDecodifica, out listaAccordiPerTipo0179);

                if (listaAccordiPerTipo0179 != null && listaAccordiPerTipo0179.Count() > 0)
                {
                    DecAnagraficaAccordiPerTipo0179 accordoPerTipo179 = listaAccordiPerTipo0179.
                        Where(x => ctrlCodiciFiscaliAbilitatiPerTipo0179 != null && x.Id == ctrlCodiciFiscaliAbilitatiPerTipo0179.IdDecAnagraficaAccordi0179).FirstOrDefault();

                    if (accordoPerTipo179 != null)
                        datiIstruttoriaEntity.CodiceAziendaEditoriaPerTipo0179 = accordoPerTipo179.Codice;
                }
            }
            Utility.ValorizzaOggetti(contenitore.DatiPensioniDatiGenerici, datiIstruttoriaEntity);
            Utility.ValorizzaOggetti(contenitore.DatiMaggiorazioniBenefici, datiIstruttoriaEntity);

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                datiIstruttoriaEntity.DatiENPALS = new DatiIstruttoria.ENPALS();
                Utility.ValorizzaOggetti(contenitore.DatiEnpals, datiIstruttoriaEntity.DatiENPALS);
            }

            if (contenitore.DatiPensione != null && Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria))
            {
                string codiceBancaEsodatiTraduzioneSuGP = string.Empty;
                if (contenitore.DatiPensione.CodiceBancaEsodati.HasValue)
                {
                    if (contenitoreDecodifica.ElencoDecAzienda != null && contenitoreDecodifica.ElencoDecAzienda.Count > 0)
                    {
                        short codiceBancaEsodati = contenitore.DatiPensione.CodiceBancaEsodati.Value;
                        GestioneDecodificaAzienda.DecAzienda decAzienda = contenitoreDecodifica.ElencoDecAzienda.Find(x => x.Id == codiceBancaEsodati);
                        if (decAzienda != null)
                            datiIstruttoriaEntity.CodiceBancaEsodatiTraduzioneSuGP = decAzienda.TraduzioneSuGP;
                    }
                }
            }

            if (datiIstruttoriaEntity.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoriaEntity.IsDatiIstruttoriaPensioneNull() && datiIstruttoriaEntity.IsDatiIstruttoriaDatiGenericiNull() &&
                datiIstruttoriaEntity.IsDatiIstruttoriaMaggiorazioneBeneficiNull() && (datiIstruttoriaEntity.DatiENPALS == null || datiIstruttoriaEntity.DatiENPALS.IsDatiIstruttoriaEnpalsNull()))
                datiIstruttoriaEntity = null;
        }

        public static bool IsDatiIstruttoriaPresenti(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            if (contenitore.DatiIstruttoria == null && contenitore.DatiPensioniDatiGenerici == null && contenitore.DatiMaggiorazioniBenefici == null &&
                (!Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) || contenitore.DatiEnpals == null))
                return false;

            return true;
        }

        public static void StoreDatiIstruttoria(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiIstruttoria datiIstruttoriaEntity,
            DatiGenerici datiGenericiEntity, DateTime dataSistema, bool isSingleTab, bool IsCancelOperation, bool isRiaperturaDomanda)
        {
            string msg = string.Empty;

            if (datiIstruttoriaEntity == null)
                datiIstruttoriaEntity = new DatiIstruttoria();

            if (datiIstruttoriaEntity.DatiENPALS == null)
                datiIstruttoriaEntity.DatiENPALS = new DatiIstruttoria.ENPALS();

            if (isSingleTab)
                GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiGenericiEntity, out msg);

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon = contenitore.DatiIstruttoria;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = contenitore.DatiPensioniDatiGenerici;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = contenitore.DatiMaggiorazioniBenefici;
            GestioneEnpals.DatiEnpals datiENPALS = null;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                datiENPALS = contenitore.DatiEnpals;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = null;
            if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
            {
                datiAnagraficiTitolare = contenitore.DatiAnagraficiTitolare;
                datiQuadroEliminazione = contenitore.DatiQuadroEliminazione;
            }
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
            //----------------------------------------------------------------

            bool bloccoDeroga = false;
            if (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione)
                || Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione)
                || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione)
                || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione)
                || Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione)
                || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione))
                bloccoDeroga = true;

            bool isEliminazioneRossoPerConfermaInvalidita = !GestioneCrossControls.AGO_CI_ControlsEliminazioneConfermaInvalidita(datiPensione,
                (datiEliminazione != null) ? datiEliminazione.DataEvento : null, datiGenericiEntity.NRiconoscimentiInvalidita, dataSistema, isRiaperturaDomanda, out msg);

            bool isQuadroOneriRosso = datiIstruttoriaEntity != null && datiPensioniDatiGenerici != null && datiIstruttoriaEntity.ScadenzaAssegno.GetValueOrDefault() != datiPensioniDatiGenerici.ScadenzaAssegno.GetValueOrDefault();

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiIstruttoriaPerPensioni(datiIstruttoriaEntity, datiPensione);
                StoreDatiIstruttoriaPerIstruttoria(datiPensione.Id, datiPensione.FlagUnicarpe, datiPensione.TipoLetturaUnicarpe, datiIstruttoriaEntity, ref datiIstruttoriaCommon, bloccoDeroga);
                StoreDatiIstruttoriaPerDatiGenerici(datiPensione, datiIstruttoriaEntity, ref datiPensioniDatiGenerici);
                StoreDatiIstruttoriaPerMaggiorazioneBenefici(datiPensione.Id, datiIstruttoriaEntity, ref datiMaggiorazioniBeneficiCommon);
                StoreDatiIstruttoriaPerENPALS(datiPensione, datiIstruttoriaEntity, ref datiENPALS, isRiaperturaDomanda);

                if ((datiIstruttoriaEntity.IsDatiIstruttoriaPensioneNull() && datiIstruttoriaEntity.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoriaEntity.IsDatiIstruttoriaDatiGenericiNull() && datiIstruttoriaEntity.IsDatiIstruttoriaMaggiorazioneBeneficiNull() &&
                    datiIstruttoriaEntity.IsDatiIstruttoriaENPALSNull()) || IsCancelOperation)
                {
                    if ((datiENPALS != null && !datiENPALS.IsIstruttoriaNull()) ||
                        (Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria)) || //per VESO92 e VESO33 il tab istruttoria deve essere sempre rosso
                        (Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria)) ||
                        Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia122(datiPensione) ||
                        Utility.IsDomandaSalvaguardia135(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) ||
                        Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                        Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                        Utility.IsDomandaSalvaguardia178_2020(datiPensione) ||
                        (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiIstruttoriaEntity != null &&
                        (datiIstruttoriaEntity.RiduzioneRetributiva || (datiIstruttoriaEntity.Legge44997.HasValue && datiIstruttoriaEntity.Legge44997.Value != 0))) ||
                        (Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) && !datiIstruttoriaEntity.CodiceAziendaEditoria.HasValue && !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaSO(datiPensione.SiglaCategoria))) ||
                        (Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) && !datiIstruttoriaEntity.CodiceAziendaEditoriaLetteraB.HasValue) ||
                        (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) && !datiIstruttoriaEntity.CodiceAziendaEditoriaPerTipo0171.HasValue) ||
                        (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) && !datiIstruttoriaEntity.CodiceAziendaEditoriaPerTipo0179.HasValue) ||
                        Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) ||
                        Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) ||
                        Utility.IsDomandaESPA(datiPensione.SiglaCategoria) || Utility.IsDomandaManualeInvaliditaOver80_L80(datiPensione) || (Utility.IsDomandaBancari(datiPensione.SiglaCategoria) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)))
                        datiQuadroLiquidazionePensione.TabIstruttoria = 0;
                    else
                        datiQuadroLiquidazionePensione.TabIstruttoria = 1;
                }
                else
                    datiQuadroLiquidazionePensione.TabIstruttoria = 2;

                if ((datiIstruttoriaEntity == null || !datiIstruttoriaEntity.CodiceBancaEsodati.HasValue) && IsIstruttoriaAziendaVisible(datiPensione.NaturaPensione, datiPensione.SiglaCategoria, datiPensione))
                    datiQuadroLiquidazionePensione.TabIstruttoria = 0;

                if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) ||
                    Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) ||
                    Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) ||
                    Utility.IsDomandaMIN(datiPensione.SiglaCategoria) ||
                    Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) ||
                    Utility.IsRenditaCasalinghe(datiPensione) ||
                    Utility.IsRenditaFacoltativa(datiPensione) ||
                    Utility.IsDomandaVOST(datiPensione.SiglaCategoria))
                    datiQuadroLiquidazionePensione.TabIstruttoria = null;

                if (Utility.IsDomandaPescatori(datiPensione.SiglaCategoria))
                    datiQuadroLiquidazionePensione.TabIstruttoria = 2;

                // Qualora la data scadenza indennità indicata nella scheda “Istruttoria” sia inferiore o uguale al mese in corso, 
                // allora il quadro “Eliminazione” e la scheda “Eliminazione” dovranno essere resi obbligatori 
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                {
                    if (datiEliminazione == null || datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
                    {
                        if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
                        {
                            if ((datiIstruttoriaEntity != null && datiIstruttoriaEntity.ScadenzaAssegno.HasValue &&
                               Utility.DataSuccessivaA(Utility.FirstDayOfMonth(dataSistema), Utility.FirstDayOfMonth(datiIstruttoriaEntity.ScadenzaAssegno.Value))) ||
                               ((datiAnagraficiTitolare != null && datiAnagraficiTitolare.DataMorte.HasValue && Utility.DataSuccessivaA(datiAnagraficiTitolare.DataMorte.Value, datiPensione.DecorrenzaOriginaria.Value)) ||
                               (isEliminazioneRossoPerConfermaInvalidita && !IsCancelOperation)))
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
                }

                if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) && Utility.IsRicostituzione(datiPensione.Gruppo) && isQuadroOneriRosso)
                {
                    datiQuadroOneri.Tipo = 2;
                    datiQuadroOneri.TabOneri = 0;

                    GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                }

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiIstruttoria = datiIstruttoriaCommon;
            contenitore.DatiPensioniDatiGenerici = datiPensioniDatiGenerici;
            contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBeneficiCommon;
            contenitore.DatiEnpals = datiENPALS;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.DatiQuadroEliminazione = datiQuadroEliminazione;
            //--------------------------------------------------------------------
        }

        private static void StoreDatiIstruttoriaPerPensioni(Entity.DatiIstruttoria datiIstruttoriaEntity, GestionePensione.DatiPensione datiPensione)
        {
            Utility.ValorizzaOggetti(datiIstruttoriaEntity, datiPensione);
            GestionePensione.SalvaPensione(datiPensione);
        }

        private static void StoreDatiIstruttoriaPerIstruttoria(long idPensione, bool? flagUnicarpe, char? TipoLetturaUnicarpe,
            Entity.DatiIstruttoria datiIstruttoriaEntity, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool bloccoDeroga)
        {
            if (datiIstruttoria == null)
            {
                if (datiIstruttoriaEntity.IsDatiIstruttoriaIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }

            if (Utility.IsDomandaUnicarpe(flagUnicarpe, TipoLetturaUnicarpe, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiIstruttoriaEntity.Legge44997 = datiIstruttoria.Legge44997;
                datiIstruttoriaEntity.CodiceParticolareSoggettoDerogato = datiIstruttoria.CodiceParticolareSoggettoDerogato;
            }
            else if (bloccoDeroga)
                datiIstruttoriaEntity.CodiceParticolareSoggettoDerogato = datiIstruttoria.CodiceParticolareSoggettoDerogato;

            Utility.ValorizzaOggetti(datiIstruttoriaEntity, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
            {
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
                datiIstruttoria = null;
            }
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
        }

        private static void StoreDatiIstruttoriaPerDatiGenerici(GestionePensione.DatiPensione datiPensione, Entity.DatiIstruttoria datiIstruttoriaEntity,
            ref GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici)
        {
            if (datiGenerici == null)
            {
                if (datiIstruttoriaEntity.IsDatiIstruttoriaDatiGenericiNull())
                    return;
                else
                    datiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            }
            if (Utility.IsDomandaUnicarpe(datiPensione.FlagUnicarpe, datiPensione.TipoLetturaUnicarpe, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiIstruttoriaEntity.RiduzioneRetributiva = datiGenerici.RiduzioneRetributiva;
                datiIstruttoriaEntity.RiduzioneRetributivaPercentuale = datiGenerici.RiduzioneRetributivaPercentuale;

                if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
                    datiIstruttoriaEntity.ScadenzaAssegno = datiGenerici.ScadenzaAssegno;
            }
            Utility.ValorizzaOggetti(datiIstruttoriaEntity, datiGenerici);

            if (datiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
            {
                GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                datiGenerici = null;
            }
            else
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);
        }

        private static void StoreDatiIstruttoriaPerMaggiorazioneBenefici(long idPensione, Entity.DatiIstruttoria datiIstruttoriaEntity,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                if (datiIstruttoriaEntity.IsDatiIstruttoriaMaggiorazioneBeneficiNull())
                    return;
                else
                {
                    datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                    datiMaggiorazioniBenefici.IdPensione = idPensione;
                }
            }

            Utility.ValorizzaOggetti(datiIstruttoriaEntity, datiMaggiorazioniBenefici);

            if (datiMaggiorazioniBenefici.Equals(new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici()))
            {
                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(idPensione);
                datiMaggiorazioniBenefici = null;
            }
            else
                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        private static void StoreDatiIstruttoriaPerENPALS(GestionePensione.DatiPensione datiPensione, Entity.DatiIstruttoria datiIstruttoriaEntity, ref GestioneEnpals.DatiEnpals datiENPALS,
            bool isRiaperturaDomanda)
        {
            if (datiENPALS == null)
            {
                if (datiIstruttoriaEntity.IsDatiIstruttoriaENPALSNull())
                    return;
                else
                {
                    datiENPALS = new GestioneEnpals.DatiEnpals();
                    datiENPALS.IdPensione = datiPensione.Id;
                }
            }

            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsEnpalsManualePL(true, Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda), datiPensione.IsDatiENPALSRecuperati))
            {
                datiIstruttoriaEntity.DatiENPALS.CodiceDeroga1 = datiENPALS.CodiceDeroga1;
                datiIstruttoriaEntity.DatiENPALS.CodiceDeroga2 = datiENPALS.CodiceDeroga2;
                datiIstruttoriaEntity.DatiENPALS.CodiceDeroga3 = datiENPALS.CodiceDeroga3;
                datiIstruttoriaEntity.DatiENPALS.CodiceDeroga4 = datiENPALS.CodiceDeroga4;
            }

            Utility.ValorizzaOggetti(datiIstruttoriaEntity.DatiENPALS, datiENPALS);

            if (datiENPALS.Equals(new GestioneEnpals.DatiEnpals()))
            {
                GestioneEnpals.EliminaDatiEnpalsByIdPensione(datiPensione.Id);
                datiENPALS = null;
            }
            else
                GestioneEnpals.SalvaDatiEnpalsEnpals(datiENPALS);
        }

        public static bool ControlDatiIstruttoria(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            DatiIstruttoria datiIstruttoriaEntity, DatiGenerici datiGenerici, DatiAssicurativi datiAssicurativi, bool IsSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region GetData
            List<GestioneBancheFideiussione.DecBancaFideiussione> listaDecBancaFideiussione = null;
            if (Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria))
                listaDecBancaFideiussione = contenitoreDecodifica.ElencoDecBancaFideiussione;

            if (IsSingleTab)
                GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiGenerici, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return false;

            byte? tipoCalcolo = datiGenerici != null ? datiGenerici.TipoCalcolo : null;

            GestioneDecodificaAzienda.DecAzienda codiceBancaEsodati = null;
            GestioneAziendeVESO33.DecAziendeVESO33 azVESO33 = null;
            GestioneAziendeCredito.DecAziendeCredito azCredito = null;
            GestioneAziendeVESO29.DecAziendeVESO29 azVESO29 = null;
            GestioneAziendeVOESO.DecAziendeVOESO azVOESO = null;
            GestioneAziendeESOTEL.DecAziendeESOTEL azESOTEL = null;
            GestioneAziendeESOAMB.DecAziendeESOAMB azESOAMB = null;

            if (datiIstruttoriaEntity.CodiceBancaEsodati.HasValue)
            {
                List<GestioneDecodificaAzienda.DecAzienda> listaDecAzienda = null;

                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)) && Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria))
                    GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(contenitore.DatiPensione.SiglaCategoria, null, out listaDecAzienda);
                else
                    GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Tipo, out listaDecAzienda);

                if (listaDecAzienda != null && listaDecAzienda.Count > 0)
                    codiceBancaEsodati = listaDecAzienda.Find(x => x.Id == datiIstruttoriaEntity.CodiceBancaEsodati.Value);

                if (Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeVESO33.GetDecodificaAziendaVESO33ByIdCodiceAzienda(codiceBancaEsodati.Id, out azVESO33);
                }
                else if (Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeCredito.GetDecodificaAziendaCreditoByIdCodiceAzienda(codiceBancaEsodati.Id, out azCredito);
                }
                else if (Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeVESO29.GetDecodificaAziendaVESO29ByIdCodiceAzienda(codiceBancaEsodati.Id, out azVESO29);
                }
                else if (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeVOESO.GetDecodificaAziendaVOESOByIdCodiceAzienda(codiceBancaEsodati.Id, out azVOESO);
                }
                else if (Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeESOTEL.GetDecodificaAziendaESOTELByIdCodiceAzienda(codiceBancaEsodati.Id, out azESOTEL);
                }
                else if (Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (codiceBancaEsodati != null)
                        GestioneAziendeESOAMB.GetDecodificaAziendaESOAMBByIdCodiceAzienda(codiceBancaEsodati.Id, out azESOAMB);
                }
            }

            char? derogaTraduzioneSuGP = null;
            if (Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria))
            {
                if (datiIstruttoriaEntity != null && datiIstruttoriaEntity.CodiceParticolareSoggettoDerogato.HasValue)
                {
                    if (contenitoreDecodifica.ElencoCodiceParticolare != null && contenitoreDecodifica.ElencoCodiceParticolare.Count > 0)
                    {
                        long valueToCompare = datiIstruttoriaEntity.CodiceParticolareSoggettoDerogato.Value;
                        GestioneDecodifica.CodiceParticolare codiceParticolare = contenitoreDecodifica.ElencoCodiceParticolare.Find(x => x.Id == valueToCompare);
                        if (codiceParticolare != null)
                            derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                    }
                }
            }

            List<GestioneAziendeVOCRED_DAP.DecAziendeVOCRED_DAP> listaAziendeVOCRED_DAPAmmesse = null;
            if (Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione))
                GestioneAziendeVOCRED_DAP.GetDecodificaAziendeVOCRED_DAP(out listaAziendeVOCRED_DAPAmmesse);

            List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> listaAziendeScadenzaAssegnoGGmmAAAA = null;
            if (Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null))
                GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out listaAziendeScadenzaAssegnoGGmmAAAA);
            #endregion GetData

            if (!GestioneControlli.ControlsAziendaPerEditoriaWithCodNatura3(contenitore.DatiPensione.SiglaCategoria,
                datiIstruttoriaEntity.CodiceBancaEsodati.HasValue ? datiIstruttoriaEntity.CodiceBancaEsodati.Value : (short?)null,
                datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceRequisitiRidotti(datiIstruttoriaEntity.Legge44997, datiGenerici != null ? datiGenerici.CodiceMobilita : null,
                datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, contenitore.DatiPensione, contenitore.TipoCalcolo, contenitore.DatiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsAttivitaUsurantiWithCodNatura1(datiIstruttoriaEntity.Attivitausuranti.HasValue ? datiIstruttoriaEntity.Attivitausuranti.Value : (bool?)null,
                datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsAliquotaTFResodati(datiIstruttoriaEntity.AliquotaTFREsodati, contenitore.DatiPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsAttivitaUsuranti(datiIstruttoriaEntity.Attivitausuranti, datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty,
                contenitore.DatiPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsAziendaPerEditoria(datiIstruttoriaEntity.CodiceBancaEsodati, datiGenerici != null ? datiGenerici.NaturaPensione : string.Empty,
                contenitore.DatiPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsAziendaPerEsodati(codiceBancaEsodati, contenitore.DatiPensione, listaAziendeVOCRED_DAPAmmesse, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsBanchePerSede(datiIstruttoriaEntity.CodiceBancaEsodati, contenitore.DatiPensione, out messaggioVideo))
                return false;

            if (tipoCalcolo.HasValue)
                if (!GestioneCrossControls.AGO_ControlsRiduzioneRetributiva(tipoCalcolo, datiIstruttoriaEntity.RiduzioneRetributiva, datiIstruttoriaEntity.RiduzioneRetributivaPercentuale,
                    isRiaperturaDomanda, contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null, codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null, out messaggioVideo))
                    return false;

            if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, datiIstruttoriaEntity.RiduzioneRetributiva,
                datiIstruttoriaEntity.RiduzioneRetributivaPercentuale, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVESO33(contenitore.DatiPensione.DecorrenzaOriginaria, azVESO33, codiceBancaEsodati, contenitore.DatiPensione.SiglaCategoria,
                out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVOCRED_CRED27(contenitore.DatiPensione.DecorrenzaOriginaria, azCredito, codiceBancaEsodati,
                contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVOCOOP_COOP28(contenitore.DatiPensione.DecorrenzaOriginaria, azCredito, codiceBancaEsodati, contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVESO29(contenitore.DatiPensione.DecorrenzaOriginaria, azVESO29, codiceBancaEsodati, contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreVOESO(contenitore.DatiPensione.DecorrenzaOriginaria, azVOESO, codiceBancaEsodati, contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreESOTEL(contenitore.DatiPensione.DecorrenzaOriginaria, azESOTEL, codiceBancaEsodati, contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_VerificaDecorrenzaPensioneSuperioreESOAMB(contenitore.DatiPensione.DecorrenzaOriginaria, azESOAMB, codiceBancaEsodati, contenitore.DatiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsBancaFideiussione(contenitore.DatiPensione, datiIstruttoriaEntity.AnnoBancaFideiussoria, datiIstruttoriaEntity.ProgressivoBancaFideiussoria, codiceBancaEsodati, listaAziendeScadenzaAssegnoGGmmAAAA,
                out messaggioVideo))
                return false;

            if (!GestioneCrossControls.VerificaDecorrenzaOriginariaVESO92(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria,
                codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null,
                datiIstruttoriaEntity.AnnoBancaFideiussoria, datiIstruttoriaEntity.ProgressivoBancaFideiussoria, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Cognome : null,
                contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.CodiceFiscale : null, listaDecBancaFideiussione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_ControlsRiduzioneRetributivaVOCRED(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiAnagraficiTitolare.DataNascita,
                datiIstruttoriaEntity.ScadenzaAssegno, datiIstruttoriaEntity.RiduzioneRetributiva, datiIstruttoriaEntity.RiduzioneRetributivaPercentuale, isRiaperturaDomanda, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCoerenzaRiduzioneAssegno_RiduzioneRetributivaVOCRED(contenitore.DatiPensione, datiIstruttoriaEntity.RiduzioneAssegno, datiIstruttoriaEntity.RiduzioneRetributiva,
                datiIstruttoriaEntity.RiduzioneRetributivaPercentuale, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.AGO_ControlsScadenzaAssegno(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria, datiIstruttoriaEntity.ScadenzaAssegno,
                codiceBancaEsodati != null ? codiceBancaEsodati.TraduzioneSuGP : null, datiIstruttoriaEntity.AnnoBancaFideiussoria, datiIstruttoriaEntity.ProgressivoBancaFideiussoria,
                contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale, isRiaperturaDomanda, contenitore.DatiAnagraficiTitolare.DataNascita, derogaTraduzioneSuGP,
                contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.ScadenzaAssegno : null, contenitore.DatiEliminazione != null ? contenitore.DatiEliminazione.CodiceMotivo : null,
                contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.IsScadenzaAssegnoConGiorno : null, contenitore.DatiQuadroLiquidazionePensione, out messaggioVideo))
            {

                return false;
            }

            if (!GestioneCrossControls.AGO_VerificaPerfezionamentoRequisiti_Decorrenza_ScadenzaAssegno(contenitore.DatiPensione, contenitore.DatiPensione.DecorrenzaOriginaria,
                contenitore.DatiPensione.DataPerfezionamentoRequisiti, datiIstruttoriaEntity.ScadenzaAssegno, out messaggioVideo))
                return false;

            if (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaCodiceAnagraficaAccordi(contenitore.DatiPensione, datiIstruttoriaEntity.CodiceAziendaEditoria, ref contenitoreDecodifica, out messaggioVideo))
                    return false;
            }
            else if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaLetteraBCodiceAnagraficaAccordi(contenitore.DatiPensione, datiIstruttoriaEntity.CodiceAziendaEditoriaLetteraB, ref contenitoreDecodifica, out messaggioVideo))
                    return false;
            }
            else if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaPerTipo0171CodiceAnagraficaAccordi(ref contenitore, ref contenitoreDecodifica, datiIstruttoriaEntity.CodiceAziendaEditoriaPerTipo0171,
                    out messaggioVideo))
                    return false;
            }
            else if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione))
            {
                if (!GestioneControlli.ControlsPrepensionamentoEditoriaPerTipo0179CodiceAnagraficaAccordi(ref contenitore, ref contenitoreDecodifica, datiIstruttoriaEntity.CodiceAziendaEditoriaPerTipo0179,
                    out messaggioVideo))
                    return false;
            }
            if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
            {
                if (!GestioneControlli.VerificaScadenzaIndennitaPerApeSociale(ref contenitore, datiIstruttoriaEntity, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.IsRiduzioneAssegnoAmmissibile(ref contenitore, ref contenitoreDecodifica, datiIstruttoriaEntity, out messaggioVideo))
                return false;

            return true;
        }

        public static void EliminaDatiIstruttoria(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DateTime dataSistema, bool isRiaperturaDomanda)
        {
            StoreDatiIstruttoria(ref contenitore, ref contenitoreDecodifica, new DatiIstruttoria(), null, dataSistema, true, true, isRiaperturaDomanda);
        }

        #endregion dati Istruttoria

        #region dati Sentenza Art.4

        public static void GetDatiSentenzaArt4(ref EntityBLCommon.ContenitoreObject contenitore, out DatiSentenzaArt4 datiSentenzaArt4Entity)
        {
            datiSentenzaArt4Entity = null;

            if (contenitore.DatiPensione == null)
                return;

            datiSentenzaArt4Entity = new DatiSentenzaArt4();
            if (contenitore.ListaDatiSentenzaArt4 != null && contenitore.ListaDatiSentenzaArt4.Count > 0)
            {
                datiSentenzaArt4Entity.lDatiSentenzaArt4 = new List<DatiSentenzaArt4.SentenzaArt4>();

                foreach (GestioneSentenzaArt4.DatiSentenzaArt4 datoSentenza in contenitore.ListaDatiSentenzaArt4)
                {
                    DatiSentenzaArt4.SentenzaArt4 datiSentenzaArt4 = new DatiSentenzaArt4.SentenzaArt4();
                    Utility.ValorizzaOggetti(datoSentenza, datiSentenzaArt4);
                    datiSentenzaArt4Entity.lDatiSentenzaArt4.Add(datiSentenzaArt4);
                }
            }
        }

        public static void StoreDatiSentenzaArt4(ref EntityBLCommon.ContenitoreObject contenitore, DatiSentenzaArt4 datiSentenzaArt4Entity, out string msgVideo)
        {
            msgVideo = string.Empty;

            if (datiSentenzaArt4Entity == null || datiSentenzaArt4Entity.lDatiSentenzaArt4 == null || datiSentenzaArt4Entity.lDatiSentenzaArt4.Count == 0)
                return;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            List<GestioneSentenzaArt4.DatiSentenzaArt4> lDatiSentenzaArt4 = null;
            if (datiSentenzaArt4Entity.lDatiSentenzaArt4 != null)
            {
                lDatiSentenzaArt4 = new List<GestioneSentenzaArt4.DatiSentenzaArt4>();
                foreach (DatiSentenzaArt4.SentenzaArt4 datoSentenza in datiSentenzaArt4Entity.lDatiSentenzaArt4)
                {
                    GestioneSentenzaArt4.DatiSentenzaArt4 datiSentenzaArt4 = new GestioneSentenzaArt4.DatiSentenzaArt4();
                    Utility.ValorizzaOggetti(datoSentenza, datiSentenzaArt4);
                    lDatiSentenzaArt4.Add(datiSentenzaArt4);
                }
            }

            if (!GestioneControlli.ControlsRicostituzionePerEsecuzioneSentenza(datiPensione, contenitore.DatiDanteCausa, lDatiSentenzaArt4, out msgVideo))
                return;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneSentenzaArt4.EliminaDatiSentenzaArt4ByIdPensione(datiPensione.Id);

                if (lDatiSentenzaArt4 != null)
                {
                    foreach (GestioneSentenzaArt4.DatiSentenzaArt4 datoSentArt4 in lDatiSentenzaArt4)
                    {
                        GestioneSentenzaArt4.SalvaSentenzaArt4(datiPensione.Id, datoSentArt4);
                    }
                }

                datiQuadroLiquidazionePensione.TabSentenzaArt4 = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.ListaDatiSentenzaArt4 = lDatiSentenzaArt4;
            //--------------------------------------------------------------------
        }

        public static void EliminaDatiSentenzaArt4(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneSentenzaArt4.EliminaDatiSentenzaArt4NoGPByIdPensione(datiPensione.Id);

                datiQuadroLiquidazionePensione.TabSentenzaArt4 = 0;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.ListaDatiSentenzaArt4_GetEffettuata = false; // l'eliminazione non cancella tutti i dati, quindi imposto il bool a false in modo che se servono i dati, questi vengono recuperati nuovamente da DB
            //--------------------------------------------------------------------
        }
        #endregion dati Sentenza Art.4

        #region dati Sentenze

        public static void GetDatiSentenze(ref EntityBLCommon.ContenitoreObject contenitore, out DatiSentenze datiSentenzeEntity)
        {
            datiSentenzeEntity = null;

            if (contenitore.DatiPensione == null)
                return;

            datiSentenzeEntity = new DatiSentenze();
            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.CodRicalcoloSentenza.HasValue)
            {
                switch (contenitore.DatiPensioniDatiGenerici.CodRicalcoloSentenza)
                {
                    case 9: //Se GP1AXE1 = 9 
                        datiSentenzeEntity.IsSentenza49593 = true; //SI Sentenza 495/93,      
                        datiSentenzeEntity.IsSentenza2401994 = false; //NO Sentenza 240/1994, 
                        datiSentenzeEntity.IsSentenze49593_2401994 = false; //NO Sentenze 495/1993 e 240/1994
                        break;
                    case 8: //Se GP1AXE1 = 8
                        datiSentenzeEntity.IsSentenza49593 = false; //NO Sentenza 495/93, 
                        datiSentenzeEntity.IsSentenza2401994 = true; //SI Sentenza 240/1994, 
                        datiSentenzeEntity.IsSentenze49593_2401994 = false; //NO Sentenze 495/1993 e 240/1994
                        break;
                    case 7: //Se GP1AXE1 = 7
                        datiSentenzeEntity.IsSentenza49593 = false; //NO Sentenza 495/93,
                        datiSentenzeEntity.IsSentenza2401994 = false; //NO Sentenza 240/1994, 
                        datiSentenzeEntity.IsSentenze49593_2401994 = true; //SI Sentenze 495/1993 e 240/1994
                        break;
                    default: //Se ALTRO:
                        datiSentenzeEntity.IsSentenza49593 = false; //NO Sentenza 495/93, 
                        datiSentenzeEntity.IsSentenza2401994 = false; //NO Sentenza 240/1994,
                        datiSentenzeEntity.IsSentenze49593_2401994 = false; //NO Sentenze 495/1993 e 240/1994
                        break;
                }
            }
            if (contenitore.ListaDatiSentenze != null && contenitore.ListaDatiSentenze.Count > 0)
            {
                datiSentenzeEntity.lDatiSentenze = new List<DatiSentenze.Sentenze>();

                foreach (GestioneSentenze.DatiSentenze ds in contenitore.ListaDatiSentenze)
                {
                    DatiSentenze.Sentenze datiSentenze = new DatiSentenze.Sentenze();
                    Utility.ValorizzaOggetti(ds, datiSentenze);
                    datiSentenzeEntity.lDatiSentenze.Add(datiSentenze);
                }
            }
        }

        public static void StoreDatiSentenze(ref EntityBLCommon.ContenitoreObject contenitore, DatiSentenze datiSentenzeEntity, out string msgVideo)
        {
            msgVideo = string.Empty;

            if (datiSentenzeEntity == null || datiSentenzeEntity.IsNull())
                return;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                datiQuadroLiquidazionePensione.TabSentenze = 2;
                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            //--------------------------------------------------------------------
        }

        public static void EliminaDatiSentenze(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                datiQuadroLiquidazionePensione.TabSentenze = 1;
                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            //--------------------------------------------------------------------
        }

        public static bool ControlDatiSentenze(ref EntityBLCommon.ContenitoreObject contenitore, DatiSentenze datiSentenzeEntity, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            //Predisposto metodo di controllo per i dati sentenze
            //attualmente è vuoto in quanto tali dati vengono presi da host e non sono editabili
            return true;
        }
        #endregion dati Sentenze

        #region dati Opzione
        public static void ValorizzaDatiOpzione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiOpzione datiOpzione)
        {
            datiOpzione = null;

            if (contenitore.DatiIstruttoria == null)
                return;

            datiOpzione = new DatiOpzione();
            Utility.ValorizzaOggetti(contenitore.DatiIstruttoria, datiOpzione);
            if (datiOpzione.IsDatiOpzioneNull())
                datiOpzione = null;
        }

        public static void StoreDatiOpzione(ref EntityBLCommon.ContenitoreObject contenitore, DatiOpzione datiOpzione, bool IsCancelOperation)
        {
            if (datiOpzione == null)
                datiOpzione = new DatiOpzione();

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = contenitore.DatiIstruttoria;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            if (Utility.IsDomandaSPED(datiPensione) || Utility.IsRenditaCasalinghe(datiPensione) || Utility.IsRenditaFacoltativa(datiPensione) || Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) || Utility.IsDomandaPSO(datiPensione.SiglaCategoria) || Utility.IsDomandaVOST(datiPensione.SiglaCategoria)
                || Utility.IsDomandaAnte96(datiPensione, datiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null)
                return;

            DateTime dataCompare = new DateTime(1980, 01, 01);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiOpzionePerIstruttoria(datiPensione, datiOpzione, ref datiIstruttoria);

                if ((datiOpzione.IsDatiOpzioneNull() || IsCancelOperation) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                {
                    if (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value >= dataCompare)
                        datiQuadroLiquidazionePensione.TabOpzione = null;
                    else
                        datiQuadroLiquidazionePensione.TabOpzione = 1;
                }
                else
                    datiQuadroLiquidazionePensione.TabOpzione = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiIstruttoria = datiIstruttoria;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            //--------------------------------------------------------------------
        }

        private static void StoreDatiOpzionePerIstruttoria(GestionePensione.DatiPensione datiPensione, Entity.DatiOpzione datiOpzione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiOpzione.IsDatiOpzioneNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }

            // i dati provenienti da felpe sono non modificabili e non cancellabili
            //if (datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value && datiPensione.TipoLetturaUnicarpe.HasValue && datiPensione.TipoLetturaUnicarpe.Value == 'L') 
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                datiOpzione.DecorrenzaOpzione = datiIstruttoria.DecorrenzaOpzione;

            Utility.ValorizzaOggetti(datiOpzione, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
            {
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(datiPensione.Id);
                datiIstruttoria = null;
            }
            else
                GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);
        }

        public static bool ControlDatiOpzione(Entity.DatiOpzione datiOpzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            return true;
        }

        public static void EliminaDatiOpzione(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            StoreDatiOpzione(ref contenitore, new DatiOpzione(), true);
        }

        #endregion dati Opzione

        #region dati Provenienza
        public static void ValorizzaDatiProvenienza(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out DatiProvenienza datiProvenienza)
        {
            datiProvenienza = null;

            if (datiIstruttoria == null)
                return;

            datiProvenienza = new DatiProvenienza();
            Utility.ValorizzaOggetti(datiIstruttoria, datiProvenienza);
            if (datiProvenienza.IsDatiProvenienzaIstruttoriaNull())
                datiProvenienza = null;
        }

        public static bool IsDatiProvenienzaPresenti(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            if (contenitore.DatiIstruttoria == null)
                return false;

            if (!contenitore.DatiIstruttoria.CodiceP18PrecedentePensione.HasValue && !contenitore.DatiIstruttoria.SedePrecedentePensione.HasValue &&
                !contenitore.DatiIstruttoria.CertificatoPrecedentePensione.HasValue && !contenitore.DatiIstruttoria.DecorrenzaOriginariaAltraPensione.HasValue &&
                !contenitore.DatiIstruttoria.DecorrenzaCaricoPrecedentePensione.HasValue)
                return false;

            return true;
        }

        public static void StoreDatiProvenienza(ref EntityBLCommon.ContenitoreObject contenitore, DatiProvenienza datiProvenienza, bool IsCancelOperation)
        {
            if (datiProvenienza == null)
                datiProvenienza = new DatiProvenienza();

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = contenitore.DatiIstruttoria;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiProvenienzaPerIstruttoria(datiPensione.Id, datiProvenienza, ref datiIstruttoria);

                if (datiProvenienza.IsDatiProvenienzaIstruttoriaNull() || IsCancelOperation)
                {
                    if (datiPensione.TrasformazioneAOI.HasValue &&
                        datiPensione.TrasformazioneAOI.Value && !Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 0;
                    else
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 1;
                }
                else
                    datiQuadroLiquidazionePensione.TabPrecedentePensione = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiIstruttoria = datiIstruttoria;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            //--------------------------------------------------------------------
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

        public static bool ControlDatiProvenienza(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiGenerici datiGenerici,
            DatiProvenienza datiProvenienza, bool IsSingleTab, bool isRiaperturaDomanda, DatiAssicurativi datiAssicurativi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (IsSingleTab)
                GetDatiGenerici(ref contenitore, ref contenitoreDecodifica, isRiaperturaDomanda, out datiGenerici, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return false;

            if (datiGenerici != null)
            {
                if (datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value)
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

                    if (!GestioneControlli.ControlsProvenienza(datiProvenienza.CodiceP18PrecedentePensione, datiProvenienza.CertificatoPrecedentePensione, datiProvenienza.DecorrenzaOriginariaAltraPensione, datiProvenienza.SedePrecedentePensione, datiGenerici.DataInteressiLegali,
                        contenitore.DatiPensione, contenitore.DatiDanteCausa, datiAssicurativi != null ? datiAssicurativi.AttivitaEconomica : null, datiAssicurativi != null ? datiAssicurativi.ProfessioneIndividuale : null,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceDomandaRicorso : null, isRiaperturaDomanda, out messaggioVideo))
                        return false;
                }
                bool? isTrasformazioneAOI = Utility.IsDomandaTrasformazioneAOI(contenitore.DatiPensione);
                if (!Utility.IsDomandaRiliquidazione(contenitore.DatiPensione).GetValueOrDefault() && !Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault() && (!isTrasformazioneAOI.HasValue || !isTrasformazioneAOI.Value) &&
                    (!datiProvenienza.IsDatiProvenienzaIstruttoriaNull()) && (!datiGenerici.TrasformazioneAOI.HasValue || !datiGenerici.TrasformazioneAOI.Value) && !Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria))
                {
                    messaggioVideo = "Salvare i dati Generici prima di procedere con il salvataggio dei dati della Pensione di Provenienza";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "E' necessario salvare prima i dati generici";
                return false;
            }
            return true;
        }

        public static void EliminaDatiProvenienza(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            StoreDatiProvenienza(ref contenitore, new DatiProvenienza(), true);
        }
        #endregion dati Provenienza

        #region dati Inail

        public static void GetDatiInailByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiInail datiInail)
        {
            datiInail = null;

            if (contenitore.DatiInabilita != null || (contenitore.ListaDatiPensioniINAIL != null && contenitore.ListaDatiPensioniINAIL.Count > 0))
            {
                datiInail = new DatiInail();

                if (contenitore.ListaDatiPensioniINAIL != null && contenitore.ListaDatiPensioniINAIL.Count > 0)
                {
                    datiInail.LpensioniInail = new List<DatiInail.PensioniInail>();
                    foreach (GestionePensioneInailInabilita.DatiPensioniINAIL pi in contenitore.ListaDatiPensioniINAIL)
                    {
                        DatiInail.PensioniInail pensioniInail = new DatiInail.PensioniInail();
                        Utility.ValorizzaOggetti(pi, pensioniInail);
                        datiInail.LpensioniInail.Add(pensioniInail);
                    }
                }
                if (contenitore.DatiInabilita != null)
                    Utility.ValorizzaOggetti(contenitore.DatiInabilita, datiInail);
            }
        }

        public static void StoreDatiInail(ref EntityBLCommon.ContenitoreObject contenitore, DatiInail datiInail)
        {
            if (datiInail == null)
                return;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            GestionePensioneInailInabilita.DatiInabilita datiInabilita = new GestionePensioneInailInabilita.DatiInabilita();

            Utility.ValorizzaOggetti(datiInail, datiInabilita);
            datiInabilita.IdPensione = datiPensione.Id;

            List<GestionePensioneInailInabilita.DatiPensioniINAIL> LDatiPensioniINAIL = null;

            if (datiInail.LpensioniInail != null && datiInail.LpensioniInail.Count > 0)
            {
                LDatiPensioniINAIL = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();
                foreach (DatiInail.PensioniInail pi in datiInail.LpensioniInail)
                {
                    GestionePensioneInailInabilita.DatiPensioniINAIL pensioniInail = new GestionePensioneInailInabilita.DatiPensioniINAIL();
                    pi.IdPensione = datiPensione.Id;
                    Utility.ValorizzaOggetti(pi, pensioniInail);
                    LDatiPensioniINAIL.Add(pensioniInail);
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensioneInailInabilita.SalvaInabilita(datiInabilita);
                GestionePensioneInailInabilita.EliminaPensioniINAILByIdPensione(datiPensione.Id);
                if (LDatiPensioniINAIL != null && LDatiPensioniINAIL.Count > 0)
                {
                    foreach (GestionePensioneInailInabilita.DatiPensioniINAIL datiPensioniINAIL in LDatiPensioniINAIL)
                        GestionePensioneInailInabilita.SalvaPensioniINAIL(datiPensioniINAIL);
                }

                if (LDatiPensioniINAIL == null && !datiInabilita.CessazioneDirittoIntegrazioneMinimo.HasValue && !datiInabilita.DecorrenzaDirittoIntegrazioneMinimo.HasValue &&
                                                  !datiInabilita.SospensionePensioneInvalidita.HasValue && !datiInabilita.ImportoMensile.HasValue && !datiInabilita.RipristinoPensioneInvalidita.HasValue &&
                                                  !datiInabilita.DecorrenzaAssegnoAccompangamento.HasValue && !datiInabilita.DirittoAssegnoAccompagnamento.HasValue)
                {
                    if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.SiglaCategoria.Trim()) &&
                        (datiPensione.SiglaCategoria.StartsWith("I") || datiPensione.SiglaCategoria.StartsWith("S")))
                        datiQuadroLiquidazionePensione.TabInail = 1;
                    else
                        datiQuadroLiquidazionePensione.TabInail = null;
                }
                else
                    datiQuadroLiquidazionePensione.TabInail = 2;

                if (Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria))
                    datiQuadroLiquidazionePensione.TabInail = null;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiInabilita = datiInabilita;
            contenitore.ListaDatiPensioniINAIL = LDatiPensioniINAIL;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            //--------------------------------------------------------------------
        }

        public static void EliminaDatiInail(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensioneInailInabilita.EliminaInabilita(datiPensione.Id);
                GestionePensioneInailInabilita.EliminaPensioniINAILByIdPensione(datiPensione.Id);
                if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.SiglaCategoria.Trim()) &&
                    (datiPensione.SiglaCategoria.StartsWith("I") || datiPensione.SiglaCategoria.StartsWith("S")))
                    datiQuadroLiquidazionePensione.TabInail = 1;
                else
                    datiQuadroLiquidazionePensione.TabInail = null;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiInabilita = null;
            contenitore.ListaDatiPensioniINAIL = null;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            //--------------------------------------------------------------------
        }

        public static bool ControlDatiInail(DateTime? decorrenzaOriginaria, Entity.DatiInail datiInail, DateTime dataSistema, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            DateTime dataCompare = new DateTime(dataSistema.AddMonths(1).Year, dataSistema.AddMonths(1).Month, 01);

            if (datiInail != null)
            {
                if (!datiInail.DirittoAssegnoAccompagnamento.HasValue && datiInail.DecorrenzaAssegnoAccompangamento.HasValue ||
                                                      datiInail.DirittoAssegnoAccompagnamento.HasValue && !datiInail.DecorrenzaAssegnoAccompangamento.HasValue)
                {
                    messaggioVideo = "Il Diritto all'assegno d'accompagnamento prevede la presenza della decorrenza e viceversa";
                    return false;
                }

                if (decorrenzaOriginaria.HasValue && datiInail.DecorrenzaAssegnoAccompangamento.HasValue &&
                    (datiInail.DecorrenzaAssegnoAccompangamento.Value < decorrenzaOriginaria.Value || datiInail.DecorrenzaAssegnoAccompangamento.Value > dataCompare))
                {
                    messaggioVideo = "La Decorrenza all'assegno d'accompagnamento deve essere compresa tra la Decorrenza Pensione e la Data Odierna più un mese";
                    return false;
                }
            }
            return true;
        }

        #endregion dati Inail

        #region Dati Calcolo
        public static void GetDatiCalcoloContributivo(ref EntityBLCommon.ContenitoreObject contenitore, out List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi)
        {
            listaDatiContributivi = contenitore.ListaDatiContributivi;

            //if (contenitore.ListaDatiContributivi != null)
            //{
            //    listaDatiContributivi = new List<GestioneCalcolo.DatiCalcoloContributivo>();
            //    Utility.ValorizzaOggetti(contenitore.ListaDatiContributivi, listaDatiContributivi);
            //}
        }

        public static void GetDatiCalcoloRetributivo(ref EntityBLCommon.ContenitoreObject contenitore, out List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi)
        {
            listaDatiRetributivi = contenitore.ListaDatiRetributivi;

            //if (contenitore.ListaDatiRetributivi != null)
            //{
            //    listaDatiRetributivi = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
            //    Utility.ValorizzaOggetti(contenitore.ListaDatiRetributivi, listaDatiRetributivi);
            //}
        }
        #endregion Dati Calcolo

        #region Dati Storico
        public static void GetDatiLiquidazionePensioneStorico(ref EntityBLCommon.ContenitoreObject contenitore, out DatiLiquidazionePensioneStorico datiLiquidazionePensioneStorico)
        {
            datiLiquidazionePensioneStorico = null;

            if (contenitore.DatiStoricoGP != null)
            {
                datiLiquidazionePensioneStorico = new DatiLiquidazionePensioneStorico();
                Utility.ValorizzaOggetti(contenitore.DatiStoricoGP, datiLiquidazionePensioneStorico);
            }
        }
        #endregion Dati Storico

        #region decodifica

        public static void GetListaCodiciMobilita(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Mobilita> listaCodiciMobilita)
        {
            listaCodiciMobilita = new List<Mobilita>();
            List<GestioneDecodifica.Mobilita> elencoCodiceMobilitaDB = contenitoreDecodifica.ElencoCodiceMobilita;
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

        public static void GetListaCodiciDomandaRicorso(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DomandaRicorso> listaCodiciDomandaRicorso)
        {
            listaCodiciDomandaRicorso = new List<DomandaRicorso>();
            List<GestioneDecodifica.DomandaRicorso> elencoDomandaRicorsoDB = contenitoreDecodifica.ElencoDomandeRicorso;
            if (elencoDomandaRicorsoDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.DomandaRicorso DomandaRicorsoDB in elencoDomandaRicorsoDB)
                {
                    DomandaRicorso domandaRicorso = new DomandaRicorso();
                    domandaRicorso.Id = DomandaRicorsoDB.Id;
                    domandaRicorso.Descrizione = DomandaRicorsoDB.Descrizione;
                    listaCodiciDomandaRicorso.Add(domandaRicorso);
                }
            }
        }

        public static void GetListaCDCMMR(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<CDCMMR> listaCDCMMR)
        {
            listaCDCMMR = new List<CDCMMR>();
            List<GestioneDecodifica.CDCMMR> elencoCDCMMRdB = contenitoreDecodifica.ElencoCDCMMR;
            if (elencoCDCMMRdB != null)
            {
                foreach (GestioneDecodifica.CDCMMR CDCMMRdB in elencoCDCMMRdB)
                {
                    CDCMMR CDCMMR = new CDCMMR();
                    CDCMMR.Id = CDCMMRdB.Id;
                    CDCMMR.Descrizione = CDCMMRdB.Descrizione;
                    listaCDCMMR.Add(CDCMMR);
                }
            }
        }

        public static void GetListaCodiceParticolare(GestionePensione.DatiPensione datiPensione, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<CodiceParticolare> listaCodiceParticolare)
        {
            listaCodiceParticolare = new List<CodiceParticolare>();
            List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareDB = contenitoreDecodifica.ElencoCodiceParticolare;

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
            if (listaCodiceParticolare.Count > 0)
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

        public static void GetListaCodiceLegge44997(GestionePensione.DatiPensione datiPensione, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecodificaLegge44997> listaCodiceLegge44997)
        {
            listaCodiceLegge44997 = new List<DecodificaLegge44997>();
            List<GestioneDecodifica.DecodificaLegge44997> elencoCodiceLegge44997DB = contenitoreDecodifica.ElencoLegge44997;
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

        public static void GetListaCodicNatura(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<CodiciNatura> elencoCodiciNatura_AGO)
        {
            elencoCodiciNatura_AGO = null;
            List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_AGO = contenitoreDecodifica.ElencoCodiceNaturaAGO_CI;

            if (elencoCodiciNaturaCommon_AGO != null)
            {
                GetCodiciNaturaCustom(ref contenitore, ref elencoCodiciNaturaCommon_AGO);
                elencoCodiciNatura_AGO = new List<CodiciNatura>();

                foreach (GestioneDecodifica.CodiciNatura CodiciNaturaCommon_AGO in elencoCodiciNaturaCommon_AGO)
                {
                    CodiciNatura codeNatura = new CodiciNatura();
                    codeNatura.Fondo = CodiciNaturaCommon_AGO.Fondo;
                    codeNatura.Descrizione = CodiciNaturaCommon_AGO.Descrizione;
                    codeNatura.Posizione = CodiciNaturaCommon_AGO.Posizione;
                    codeNatura.Tipologia = CodiciNaturaCommon_AGO.Tipologia;
                    codeNatura.TraduzioneSuGP = CodiciNaturaCommon_AGO.TraduzioneSuGP;
                    elencoCodiciNatura_AGO.Add(codeNatura);
                }
            }
        }

        private static void GetCodiciNaturaCustom(ref EntityBLCommon.ContenitoreObject contenitore, ref List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_AGO)
        {
            if (contenitore.DatiPensione != null)
            {
                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                //ENG - Per le Vocum Ape Precoci se viene inserito il bypass COMPARTO_SCUOLA per il controllo sul pannello titolare: “La decorrenza pensione deve essere di almeno 3 mesi successiva alla data di perfezionamento dei requisiti”,
                // bisogna valorizzare il secondo byte codice natura = “S” e renderlo non editabile
                bool IsBypassCompartoScuolaAttivo = false;
                if (Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione))
                {
                    List<GestioneBypassControllo.DatiBypassControllo> listaBypassApplicatiPerNDomus = null;
                    GestioneBypassControllo.GetBypassApplicatiPerNDomus(contenitore.DatiPensione.NDomus, out listaBypassApplicatiPerNDomus);
                    if (listaBypassApplicatiPerNDomus != null && listaBypassApplicatiPerNDomus.Count() > 0 && listaBypassApplicatiPerNDomus.Exists(x => x.IdDecBypassControllo == 30164))
                        IsBypassCompartoScuolaAttivo = true;
                }

                if (elencoCodiciNaturaCommon_AGO != null && elencoCodiciNaturaCommon_AGO.Count > 0)
                {
                    List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_AGOApp = elencoCodiciNaturaCommon_AGO.ToList();
                    string codCat = contenitore.DatiPensione.GetCodCategoria();
                    bool IsCodiceGestioneSupplementi9Presente = contenitore.ListaDatiSupplementi != null ? contenitore.ListaDatiSupplementi.Any(x => x.CodGestioneSupplemento == "9") : false;
                    if ((Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault())
                        && !Utility.IsDomandaIOCUM_SOCUM_IOTOT_SOTOT(contenitore.DatiPensione.SiglaCategoria))
                    {
                        char nat1;
                        char nat2;
                        char nat3;

                        Utility.GetCodiciNatura(contenitore.DatiPensione.NaturaPensione, out nat1, out nat2, out nat3);

                        if (contenitore.DatiPensione.SiglaCategoria.Trim() == "VO" && contenitore.DatiLavorazione.CodFase == "0036" &&
                            !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.Sentenza_Bonus_Y.SENTENZA_BONUS_Y))
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 2 || (x.Posizione.Value == 2 && x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == 'V'));
                        else
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 2 || (x.Posizione.Value == 2 && x.TraduzioneSuGP == nat2));
                        if (Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSOMIN(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOMIN(contenitore.DatiPensione.SiglaCategoria)))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 3 || (x.Posizione.Value == 3 && (x.TraduzioneSuGP == nat3 || x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == 'R' || x.TraduzioneSuGP == 'V')));
                        }
                        else if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa) && IsCodiceGestioneSupplementi9Presente)
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 3 || (x.Posizione.Value == 3 && (x.TraduzioneSuGP == nat3 || x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == 'R' || x.TraduzioneSuGP == 'X' || x.TraduzioneSuGP == 'Y' || x.TraduzioneSuGP == 'K' || x.TraduzioneSuGP == 'J')));
                        }
                        else if (Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 3 || (x.Posizione.Value == 3 && (x.TraduzioneSuGP == nat3 || x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == 'R' || x.TraduzioneSuGP == 'V')));
                        }
                        else
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 3 || (x.Posizione.Value == 3 && (x.TraduzioneSuGP == nat3 || x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == 'R')));

                        }
                    }
                    else if (!contenitore.IsRiaperturaDomanda || Utility.IsDomandaIOCUM_SOCUM_IOTOT_SOTOT(contenitore.DatiPensione.SiglaCategoria))
                    {
                        foreach (GestioneDecodifica.CodiciNatura codiceNatura in elencoCodiciNaturaCommon_AGOApp)
                        {
                            if (codiceNatura.Posizione.GetValueOrDefault() == 2)
                            {
                                if (Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) ||
                                    Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) ||
                                    Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria))
                                    elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                else
                                {
                                    switch (codiceNatura.TraduzioneSuGP)
                                    {
                                        case '1':
                                            if (!(Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && (Utility.IsDomandaInvaliditaSpecifica(contenitore.DatiPensione) ||
                                                (Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsPensioneInvaliditaInabilitaENPALSOrCasellario(contenitore.DatiDanteCausa)))))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'A':
                                        case 'G':
                                        case 'L':
                                        case 'R':
                                        case 'T':
                                        case 'Z':
                                            if (codCat != "0072" && codCat != "0071" && codCat != "0013" && codCat != "0079")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if (codCat == "0071" && codiceNatura.TraduzioneSuGP != 'G')
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if (codCat == "0013")
                                            {
                                                if (Utility.IsPensioneAnzianitaOrRicostituzione(contenitore.DatiPensione, null) && codiceNatura.TraduzioneSuGP != 'G')
                                                    elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                                if (Utility.IsPensioneVecchiaiaOrRicostituzione(contenitore.DatiPensione, null) && codiceNatura.TraduzioneSuGP != 'G' && codiceNatura.TraduzioneSuGP != 'A')
                                                    elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            }
                                            break;
                                        case 'B':
                                        case 'E':
                                            if (!(contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0011" && contenitore.DatiPensione.Tipo == "0001" && codCat == "0083"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'C':
                                            if (!(contenitore.DatiPensione.Gruppo == "0003") || codCat == "0172" || codCat == "0072")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'D':
                                            if (!(codCat == "0082" || codCat == "0084"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'F':
                                            if (!(contenitore.DatiPensione.Gestione == "002" || contenitore.DatiPensione.Gestione == "003" || contenitore.DatiPensione.Gestione == "004" || codCat == "0072" || codCat == "0079"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            else if (Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, false) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(contenitore.DatiPensione)) //ENG - MEMO 166/2023
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'H':
                                            if (!(contenitore.DatiPensione.GetFiltro() == "RAL" || contenitore.DatiPensione.GetFiltro() == "R44" || contenitore.DatiPensione.GetFiltro() == "R45"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'I':
                                            if (!(contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0017" &&
                                                contenitore.DatiPensione.CodiceTipoRichiesta == "92") && codCat != "0072" && codCat != "0071" && codCat != "0079")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'J':
                                            if (!(Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, true) || Utility.IsDomandaTipoContributivoCumulo(contenitore.DatiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione) ||
                                                 (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione)) ||
                                                 (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione))))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'M':
                                        case 'N':
                                            if (!(codCat == "0070" || codCat == "0072"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'O':
                                            if (!Utility.IsDomandaSperimentaleDonnaOrRicostituzione(contenitore.DatiPensione) && !Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(contenitore.DatiPensione) &&
                                                !Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione, true, true) && !Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione, true, true) &&
                                                !Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione, true, true))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'P':
                                        case 'Q':
                                            if (!Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || codCat == "0170")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'S':
                                            if (!Utility.IsDomandaAUTAnticipataInComputo(contenitore.DatiPensione, true) && !IsBypassCompartoScuolaAttivo)
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'U':
                                            if (!(contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0025" && contenitore.DatiPensione.Tipo == "0001"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'V':
                                            if (!(codCat == "0001" && contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.CodiceDomandaRicorso.GetValueOrDefault() == 5))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'X':
                                            if (!(contenitore.DatiPensione.CodiceTipoRichiesta == "49" || contenitore.DatiPensione.CodiceTipoRichiesta == "53" || contenitore.DatiPensione.CodiceTipoRichiesta == "65"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        case 'Y':
                                            if (!(contenitore.DatiPensione.CodiceTipoRichiesta == "51" || contenitore.DatiPensione.CodiceTipoRichiesta == "52" || contenitore.DatiPensione.CodiceTipoRichiesta == "55" ||
                                                contenitore.DatiPensione.CodiceTipoRichiesta == "56" || contenitore.DatiPensione.CodiceTipoRichiesta == "B0" || contenitore.DatiPensione.CodiceTipoRichiesta == "B1" ||
                                                contenitore.DatiPensione.CodiceTipoRichiesta == "B2" || contenitore.DatiPensione.CodiceTipoRichiesta == "B3" || contenitore.DatiPensione.CodiceTipoRichiesta == "B4" ||
                                                contenitore.DatiPensione.CodiceTipoRichiesta == "B5" || contenitore.DatiPensione.CodiceTipoRichiesta == "B6" || contenitore.DatiPensione.CodiceTipoRichiesta == "B7" ||
                                                contenitore.DatiPensione.CodiceTipoRichiesta == "C0" || contenitore.DatiPensione.CodiceTipoRichiesta == "C1" || contenitore.DatiPensione.CodiceTipoRichiesta == "C2" ||
                                                contenitore.DatiPensione.CodiceTipoRichiesta == "C3" || contenitore.DatiPensione.CodiceTipoRichiesta == "C4" || contenitore.DatiPensione.CodiceTipoRichiesta == "C5" ||
                                                contenitore.DatiPensione.CodiceTipoRichiesta == "C6" || contenitore.DatiPensione.CodiceTipoRichiesta == "C7"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                        //ENG - Memo 116/2025
                                        case '9':
                                            if (!(Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione) || Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione)))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            break;
                                    }
                                }
                            }

                            if (codiceNatura.Posizione.GetValueOrDefault() == 3)
                            {
                                switch (codiceNatura.TraduzioneSuGP)
                                {
                                    case 'A':
                                    case 'E':
                                    case 'G':
                                    case 'I':
                                    case 'M':
                                        if (!Utility.IsDomandaRiliquidazioneVecchiaiaOAnzianita(contenitore.DatiPensione))
                                        {
                                            if (codCat == "0082" || codCat == "0083" || codCat == "0084" || codCat == "0198" || codCat == "0199" || codCat == "0027" || codCat == "0127" || codCat == "0028" ||
                                                codCat == "0128" || codCat == "0029" || codCat == "0129" || codCat == "0196" || codCat == "0197" || codCat == "0073" || codCat == "0074" || codCat == "0075" || codCat == "0200")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if (codiceNatura.TraduzioneSuGP != 'G' && codCat == "0171")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if ((codiceNatura.TraduzioneSuGP == 'A' || codiceNatura.TraduzioneSuGP == 'M') && (codCat == "0072" || codCat == "0071" || codCat == "0070"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if ((codiceNatura.TraduzioneSuGP == 'I') && (codCat == "0071" || codCat == "0070"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if ((codiceNatura.TraduzioneSuGP == 'E') && codCat == "0172")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if ((codCat == "0013" || codCat == "0014") && codiceNatura.TraduzioneSuGP != 'G' && codiceNatura.TraduzioneSuGP != 'I')
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        }
                                        break;
                                    case 'D':
                                        if (!Utility.IsDomandaRiliquidazioneVecchiaiaOAnzianita(contenitore.DatiPensione))
                                        {
                                            if (codCat == "0082" || codCat == "0083" || codCat == "0084" || codCat == "0198" || codCat == "0199" || codCat == "0027" || codCat == "0127" || codCat == "0028" ||
                                            codCat == "0128" || codCat == "0029" || codCat == "0129" || codCat == "0196" || codCat == "0197" || codCat == "0171" || codCat == "0172" || codCat == "0200")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        }
                                        break;
                                    case 'S':
                                        if (!Utility.IsDomandaRiliquidazioneVecchiaiaOAnzianita(contenitore.DatiPensione))
                                        {
                                            if (codCat == "0082" || codCat == "0083" || codCat == "0084" || codCat == "0198" || codCat == "0199" || codCat == "0027" || codCat == "0127" || codCat == "0028" ||
                                            codCat == "0128" || codCat == "0029" || codCat == "0129" || codCat == "0196" || codCat == "0197" || codCat == "0073" || codCat == "0074" || codCat == "0075" || codCat == "0171" || codCat == "0072" || codCat == "0071" || codCat == "0070" ||
                                            codCat == "0200" || codCat == "0013" || codCat == "0014" || (contenitore.DatiPensione.IsCumuloAutomatica.GetValueOrDefault() && (string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ||
                                            contenitore.DatiPensione.NaturaPensione.Substring(2, 1) != "S")))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        }
                                        break;
                                    case 'B':
                                        if (!(codCat == "0027" || codCat == "0127" || codCat == "0028" || codCat == "0128" || codCat == "0029" || codCat == "0129" || codCat == "0198" || codCat == "0199" ||
                                              codCat == "0196" || codCat == "0197" || codCat == "0200"))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                    case 'C':
                                        if (!(codCat == "0044"))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                    case 'F':
                                    case 'T':
                                        elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                    case 'H':
                                        if (!(contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" &&
                                            (contenitore.DatiPensione.Tipo == "0002" || contenitore.DatiPensione.Tipo == "0003")))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                    case 'J':
                                    case 'K':
                                    case 'X':
                                    case 'Y':
                                    case 'W':
                                        if (!(contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0003" &&
                                        (contenitore.DatiPensione.Tipo == "0012" || contenitore.DatiPensione.Tipo == "0018")) && !(codCat == "0013" || codCat == "0014"))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        if ((codCat == "0013" || codCat == "0014") && codiceNatura.TraduzioneSuGP != 'X')
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;

                                    case 'L':
                                        if (!(contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0003" &&
                                            (contenitore.DatiPensione.Tipo == "0106" || contenitore.DatiPensione.Tipo == "0107" || contenitore.DatiPensione.Tipo == "0113" || contenitore.DatiPensione.Tipo == "0114")))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                    case 'N':
                                    case 'P':
                                    case 'R':
                                    case 'U':
                                        if (!Utility.IsDomandaRiliquidazioneVecchiaiaOAnzianita(contenitore.DatiPensione))
                                        {
                                            //In attesa di chiarimenti. Attualmente il codice natura è visibile tranne per le VESO33, VESO92, VOCOOP, VOESO, VOCRED, ESPA
                                            if (codCat == "0198" || codCat == "0199" || codCat == "0027" || codCat == "0127" || codCat == "0028" || codCat == "0128" || codCat == "0029" ||
                                            codCat == "0129" || codCat == "0196" || codCat == "0197" || codCat == "0073" || codCat == "0074" || codCat == "0075" || codCat == "0200")
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                            if ((codiceNatura.TraduzioneSuGP == 'P' || codiceNatura.TraduzioneSuGP == 'U') && (codCat == "0171" || codCat == "0072" || codCat == "0071" || codCat == "0070" || codCat == "0013" || codCat == "0014"))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        }
                                        break;
                                    case 'Q':
                                        if (!Utility.IsDomandaRiliquidazioneVecchiaiaOAnzianita(contenitore.DatiPensione))
                                        {
                                            //In attesa di chiarimenti. Attualmente il codice natura è visibile tranne per le VESO92, VOCOOP, VOESO, VOCRED, ESPA
                                            if ((codCat == "0027" || codCat == "0028" || codCat == "0029" || codCat == "0073" || codCat == "0074" || codCat == "0075" ||
                                             codCat == "0127" || codCat == "0128" || codCat == "0129" || codCat == "0196" || codCat == "0197" || codCat == "0171" || codCat == "0072" || codCat == "0071" || codCat == "0070" || codCat == "0200" || codCat == "0013" || codCat == "0014") &&
                                            !Utility.IsDomandaQuota100VESO33(contenitore.DatiPensione) && !Utility.IsDomandaQuota100ESOTEL(contenitore.DatiPensione) &&
                                            !Utility.IsDomandaQuota100ESOAMB(contenitore.DatiPensione) && !Utility.IsDomandaQuota100VESO29(contenitore.DatiPensione) &&
                                            !Utility.IsDomandaQuota100CRED27(contenitore.DatiPensione) && !Utility.IsDomandaQuota100COOP28(contenitore.DatiPensione))
                                                elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        }
                                        break;
                                    case 'O':
                                        // Il codice natura è visibile solo per ENPALS
                                        if (!(codCat == "0201" || codCat == "0202" || codCat == "0203" || codCat == "0204" || codCat == "0205" || codCat == "0206" ||
                                            codCat == "0207" || codCat == "0208" || codCat == "0209" || codCat == "0210" || codCat == "0211" || codCat == "0212" ||
                                            Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione) ||
                                            Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione)))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                    case 'V':
                                        //ENG - Spacchettate SOPGI
                                        //Eng - V aggiunta per gruppo = 0001 prodotto = 0001 tipo = 0161 di categoria = VO
                                        if (!(codCat == "0198" || codCat == "0199" || codCat == "0082" || codCat == "0083" || codCat == "0084" || codCat == "0027" || codCat == "0127" ||
                                            codCat == "0028" || codCat == "0171" || codCat == "0172" || codCat == "0128" || codCat == "0029" || codCat == "0129" || codCat == "0170" ||
                                            codCat == "0196" || codCat == "0197" || codCat == "0073" || codCat == "0074" || codCat == "0075" || codCat == "0072" || codCat == "0071" ||
                                            codCat == "0070" || codCat == "0200" || codCat == "0007" || codCat == "0008" || codCat == "0009" || codCat == "0032" || codCat == "0033" || codCat == "0034" ||
                                            (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.TipoCertificazioneFelpe == "CII") ||
                                            Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) || Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) ||
                                            (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione)) ||
                                            Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) ||
                                            (codCat == "0001" && contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0161") || (Utility.IsDomandaSOMIN(contenitore.DatiPensione.SiglaCategoria)
                                            || Utility.IsDomandaVOMIN(contenitore.DatiPensione.SiglaCategoria))) && !Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                    case 'Z':
                                        if (!(Utility.IsDomandaUsuranti(contenitore.DatiPensione)))
                                            elencoCodiciNaturaCommon_AGO.Remove(codiceNatura);
                                        break;
                                }
                            }
                        }
                    }

                    if (Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault())
                    {
                        if ((contenitore.DatiPensione.SiglaCategoria.StartsWith("V") || contenitore.DatiPensione.SiglaCategoria.StartsWith("I")) && (contenitore.DatiPensione.NaturaPensione.StartsWith("6") || contenitore.DatiPensione.NaturaPensione.StartsWith(" ")))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6')));
                        }
                        else if (contenitore.DatiPensione.SiglaCategoria.StartsWith("V") && (contenitore.DatiPensione.NaturaPensione.StartsWith("1") || contenitore.DatiPensione.NaturaPensione.StartsWith("2")))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '1' || x.TraduzioneSuGP == '2')));
                        }
                        else if (contenitore.DatiPensione.SiglaCategoria.StartsWith("I") && (contenitore.DatiPensione.NaturaPensione.StartsWith("3") || contenitore.DatiPensione.NaturaPensione.StartsWith("4")))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4')));
                        }
                        else if (contenitore.DatiPensione.SiglaCategoria.StartsWith("S") && (contenitore.DatiPensione.NaturaPensione.StartsWith("3") || contenitore.DatiPensione.NaturaPensione.StartsWith("4") || contenitore.DatiPensione.NaturaPensione.StartsWith("6") || contenitore.DatiPensione.NaturaPensione.StartsWith(" ")))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6' || x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4')));
                        }
                        else if (contenitore.DatiPensione.NaturaPensione.StartsWith("5"))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '5')));
                        }
                    }
                    else if (Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) ||
                        Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '1')));
                    }
                    else if (Utility.IsDomandaAnticipataCumuloL232(contenitore.DatiPensione) || Utility.IsDomandaAnticipataVESO29(contenitore.DatiPensione) ||
                             Utility.IsDomandaAnticipataESOTEL(contenitore.DatiPensione) || Utility.IsDomandaAnticipataESOAMB(contenitore.DatiPensione))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '1' || x.TraduzioneSuGP == '2')));
                    }
                    else if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVecchiaiaVESO29(contenitore.DatiPensione) ||
                             Utility.IsDomandaVecchiaiaESOTEL(contenitore.DatiPensione) || Utility.IsDomandaVecchiaiaESOAMB(contenitore.DatiPensione))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6')));
                    }
                    else if (Utility.IsDomandaVOMIN(contenitore.DatiPensione.SiglaCategoria))
                    {
                        if (Utility.IsPensioneAnzianitaOrRicostituzione(contenitore.DatiPensione, null))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2'))));
                        }
                        else if (Utility.IsPensioneVecchiaiaOrRicostituzione(contenitore.DatiPensione, null))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value != '1' && x.TraduzioneSuGP.Value != '2' && x.TraduzioneSuGP.Value != '5'))));
                        }
                        //elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == '0' || x.TraduzioneSuGP.Value == '8' || x.TraduzioneSuGP.Value == '9'))));
                    }
                    else if (Utility.IsDomandaSOMIN(contenitore.DatiPensione.SiglaCategoria))
                    {
                        if (Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) && Utility.IsDomandaPL(contenitore.DatiPensione))
                        {
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == '0' || x.TraduzioneSuGP.Value == '8' || x.TraduzioneSuGP.Value == '9' || x.TraduzioneSuGP.Value == ' '))));
                        }
                        else
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2' || x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == '0' || x.TraduzioneSuGP.Value == '8' || x.TraduzioneSuGP.Value == '9' || x.TraduzioneSuGP.Value == ' '))));
                    }
                    //else if (Utility.IsRenditaFacoltativa(contenitore.DatiPensione))
                    //{
                    //    if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
                    //    {
                    //        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6' || x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4' || x.TraduzioneSuGP == '5')));
                    //    }
                    //    else
                    //    {
                    //        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6')));
                    //    }
                    //}
                    //else if (Utility.IsRenditaCasalinghe(contenitore.DatiPensione))
                    //{
                    //    if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
                    //    {
                    //        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6' || x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4' || x.TraduzioneSuGP == '5')));
                    //    }
                    //    else
                    //    {
                    //        if (!string.IsNullOrEmpty(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.SiglaCategoria.Trim() == "VMP")
                    //            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6')));
                    //        else if (!string.IsNullOrEmpty(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.SiglaCategoria.Trim() == "IMP")
                    //        {
                    //            if (Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1997, 1, 1)))
                    //                elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4')));
                    //            else
                    //                elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6')));
                    //        }
                    //    }
                    //}
                    else if (Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == ' '))));
                    }
                    else if (Utility.IsDomandaPMO(contenitore.DatiPensione.SiglaCategoria))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2' || x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == '0' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == ' '))));
                    }
                    else if (Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == ' '))));
                    }
                    else if (Utility.IsDomandaPMO(contenitore.DatiPensione.SiglaCategoria))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2' || x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == '0' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == ' '))));
                    }
                    else if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                    {
                        char siglaCatChar1 = contenitore.DatiPensione.SiglaCategoria.Trim()[0];
                        char nat1;
                        char nat2;
                        char nat3;
                        if (contenitore.DatiPensione.NaturaPensione != null)
                        {
                            Utility.GetCodiciNatura(contenitore.DatiPensione.NaturaPensione, out nat1, out nat2, out nat3);

                            if (contenitore.DatiPensione.SiglaCategoria.Trim() == "VOBIS" || contenitore.DatiPensione.SiglaCategoria.Trim() == "VMP" || contenitore.DatiPensione.SiglaCategoria.Trim() == "IOBIS" || Utility.IsDomandaIMP(contenitore.DatiPensione))
                            {
                                if (nat1 == '5')
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '5')));
                                else if (siglaCatChar1 == 'V')
                                {
                                    if (nat1 == ' ' || nat1 == '6')
                                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6')));
                                    else if (nat1 == '1' || nat1 == '2')
                                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '1' || x.TraduzioneSuGP == '2')));
                                    else if (nat1 == '3' || nat1 == '4')
                                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4')));
                                }
                                else if (siglaCatChar1 == 'I')
                                {
                                    if (nat1 == '1' || nat1 == '2')
                                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '1' || x.TraduzioneSuGP == '2')));
                                    else if (nat1 == '3' || nat1 == '4')
                                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4')));

                                }
                            }
                            else
                            {
                                if (nat1 == ' ' || nat1 == '6')
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == ' ' || x.TraduzioneSuGP == '6')));
                                else if (nat1 == '1' || nat1 == '2')
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '1' || x.TraduzioneSuGP == '2')));
                                else if (nat1 == '3' || nat1 == '4')
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4')));
                                else if (nat1 == '5')
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '5')));
                                else if (nat1 == '8' || nat1 == '9')
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '8' || x.TraduzioneSuGP == '9')));

                            }
                        }
                        //se pensione == null -> ritorno tutti i codici natura 1.

                        //Per domande di Esodo sulla posizione 3 i codici natura attivi sono solo B, Q e V
                        if (Utility.IsDomandaEsodo(contenitore.DatiPensione))
                            elencoCodiciNaturaCommon_AGO.RemoveAll(x => x.Posizione.GetValueOrDefault() == 3 &&
                            x.TraduzioneSuGP.GetValueOrDefault() != 'B' && x.TraduzioneSuGP.GetValueOrDefault() != 'Q' &&
                            x.TraduzioneSuGP.GetValueOrDefault() != 'V');
                    }
                    else if (contenitore.DatiPensione.SiglaCategoria.Trim() == "VOBIS" || contenitore.DatiPensione.SiglaCategoria.Trim() == "VMP" || contenitore.DatiPensione.SiglaCategoria.Trim() == "IOBIS")
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == ' ')));
                    }
                    else if (Utility.IsDomandaIMP(contenitore.DatiPensione))
                    {
                        DateTime date = new DateTime(1997, 01, 01);
                        if (contenitore.DatiPensione.DecorrenzaOriginaria.HasValue && Utility.DataStrettamenteSuccessivaSenzaGiorno(contenitore.DatiPensione.DecorrenzaOriginaria.Value, date))
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4')));
                        else
                            elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == ' ')));
                    }
                    else if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
                    {
                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == ' '))) ||
                            (x.Posizione.Value == 3 && x.TraduzioneSuGP.HasValue && x.TraduzioneSuGP.Value == 'O'));
                    }
                    else
                    {
                        // Le AUT supplementari con codice domanda ricorso pari a 8 o 9 mostrano tutti i codici al primo byte del codice natura
                        if (!(Utility.IsDomandaAUT(contenitore.DatiPensione) && (contenitore.DatiIstruttoria.CodiceDomandaRicorso.GetValueOrDefault() == 8 || contenitore.DatiIstruttoria.CodiceDomandaRicorso.GetValueOrDefault() == 9) &&
                            (contenitore.DatiPensione.Tipo == "0009" || contenitore.DatiPensione.Tipo == "0192")))
                        {
                            if (Utility.IsDomandaSPED(contenitore.DatiPensione))
                            {
                                elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 3 || (x.Posizione.Value == 3 && (new List<char>() { ' ', 'G', 'S' }.Contains((char)x.TraduzioneSuGP))));
                            }

                            if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && (contenitore.DatiPensione.Tipo == "0009" || contenitore.DatiPensione.Tipo == "0192")) ||
                                (contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0013" && contenitore.DatiPensione.Tipo == "0009") ||
                                (contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0021" && contenitore.DatiPensione.Tipo == "0009") ||
                                (contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0022" && contenitore.DatiPensione.Tipo == "0009")) // pensione supplementare
                                elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '5')))));
                            else if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002") ||
                                (contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0011"))  // pensione vecchiaia
                                elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '6')))));
                            else if (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001") // pensione anzianita
                                elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2')))));
                            else if (contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0012")
                                elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4')))));
                            else if (contenitore.DatiPensione.Gruppo == "0003")
                            {
                                if (contenitore.DatiPensione.Prodotto == "0021" && Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) &&
                                    Utility.IsPensioneInvaliditaInabilitaENPALSOrCasellario(contenitore.DatiDanteCausa) && contenitore.DatiPensione.NaturaPensione != null)
                                {
                                    char nat1;
                                    char nat2;
                                    char nat3;
                                    Utility.GetCodiciNatura(contenitore.DatiPensione.NaturaPensione, out nat1, out nat2, out nat3);
                                    if (nat1 == '3' || nat1 == '4')
                                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP == '3' || x.TraduzioneSuGP == '4')));
                                    else
                                        elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '6')))));
                                }
                                else if (Utility.IsDomandaIndennitaUnaTantum_AGO(contenitore.DatiPensione))
                                {
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '6')))));
                                }
                                else if (Utility.IsDomandaSOTOT(contenitore.DatiPensione.SiglaCategoria))
                                {
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2' || x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '6')))));
                                }
                                else
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '3' || x.TraduzioneSuGP.Value == '4' || x.TraduzioneSuGP.Value == '6')))));
                            }
                            else if (Utility.IsDomandaRiliquidazione(contenitore.DatiPensione).GetValueOrDefault())
                            {
                                if (contenitore.DatiPensione.Tipo == "0021")
                                {
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == '1' || x.TraduzioneSuGP.Value == '2' || x.TraduzioneSuGP.Value == '5')))));
                                }
                                else if (contenitore.DatiPensione.Tipo == "0022")
                                {
                                    elencoCodiciNaturaCommon_AGO = elencoCodiciNaturaCommon_AGO.FindAll(x => (x.Posizione.Value != 1 || (x.Posizione.Value == 1 && (x.TraduzioneSuGP.HasValue && (x.TraduzioneSuGP.Value == ' ' || x.TraduzioneSuGP.Value == '6' || x.TraduzioneSuGP.Value == '5')))));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void GetListaCodiceModalitaLiquidazione(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecModalitaLiquidazione> listaCodiceModalitaLiquidazione)
        {
            listaCodiceModalitaLiquidazione = new List<DecModalitaLiquidazione>();
            List<GestioneDecodifica.DecModalitaLiquidazione> elencoModalitaLiquidazioneDB = contenitoreDecodifica.ElencoDecModalitaLiquidazione;
            if (elencoModalitaLiquidazioneDB != null)
            {
                foreach (GestioneDecodifica.DecModalitaLiquidazione CodiceModalitaLiquidazioneDB in elencoModalitaLiquidazioneDB)
                {
                    DecModalitaLiquidazione modalitaLiquidazione = new DecModalitaLiquidazione();
                    Utility.ValorizzaOggetti(CodiceModalitaLiquidazioneDB, modalitaLiquidazione);

                    //DecModalitaLiquidazione modalitaLiquidazione = new DecModalitaLiquidazione();
                    //modalitaLiquidazione.TraduzioneGp = CodiceModalitaLiquidazioneDB.TraduzioneGp;
                    //modalitaLiquidazione.Descrizione = CodiceModalitaLiquidazioneDB.Descrizione;
                    listaCodiceModalitaLiquidazione.Add(modalitaLiquidazione);
                }
            }
        }

        public static void GetListaAziendaEditoria(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecodificaAzienda> listaDecodificaAziendaEditoria)
        {
            listaDecodificaAziendaEditoria = new List<DecodificaAzienda>();
            List<GestioneDecodificaAzienda.DecAzienda> elencoAziendaEditoriaDB = contenitoreDecodifica.ElencoDecAzienda;
            //Casi speciali per le aziende
            //15/11/2021 I seguenti controlli sono commentati perchè effettuati nel metodo chiamato dalla property del contenitore decodifica
            //if (siglaCategoria.Trim() == "CRED27")
            //    siglaCategoria = "VOCRED";
            //else if (siglaCategoria.Trim() == "COOP28")
            //    siglaCategoria = "VOCOOP";

            if (elencoAziendaEditoriaDB != null)
            {
                foreach (GestioneDecodificaAzienda.DecAzienda AziendaEditoriaDB in elencoAziendaEditoriaDB)
                {
                    DecodificaAzienda aziendaEditoria = new DecodificaAzienda();
                    Utility.ValorizzaOggetti(AziendaEditoriaDB, aziendaEditoria);
                    listaDecodificaAziendaEditoria.Add(aziendaEditoria);
                }
            }
        }

        public static void GetListaRiconoscimentiInvalidita(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecodificaRiconoscimentiInvalidita> listaDecodificaRiconoscimentiInvalidita)
        {
            listaDecodificaRiconoscimentiInvalidita = new List<DecodificaRiconoscimentiInvalidita>();
            List<GestioneDecodifica.DecRiconoscimentiInvalidita> elencoRiconoscimentiInvaliditaDB = contenitoreDecodifica.ElencoRiconoscimentoInvalidita;
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

        public static void GetListaDecodificaDerogaENPALS(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecodificaDerogaENPALS> listaDecodificaDerogaENPALS)
        {
            listaDecodificaDerogaENPALS = new List<DecodificaDerogaENPALS>();
            List<GestioneDecodifica.DerogaENPALS> elencoDecodificaDerogaENPALSDB = contenitoreDecodifica.ElencoDerogaENPALS;
            if (elencoDecodificaDerogaENPALSDB != null)
            {
                foreach (GestioneDecodifica.DerogaENPALS decodificaDerogaENPALSDB in elencoDecodificaDerogaENPALSDB)
                {
                    DecodificaDerogaENPALS decodificaDerogaENPALS = new DecodificaDerogaENPALS();
                    Utility.ValorizzaOggetti(decodificaDerogaENPALSDB, decodificaDerogaENPALS);
                    listaDecodificaDerogaENPALS.Add(decodificaDerogaENPALS);
                }
            }
        }

        public static void GetListaDecodificaEnteCassaProfessionale(GestionePensione.DatiPensione datiPensione, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecodificaEnteCassaProfessionale> listaDecodificaEnteCassaProfessionale)
        {
            listaDecodificaEnteCassaProfessionale = new List<DecodificaEnteCassaProfessionale>();
            List<GestioneDecodifica.DecodificaEnteCassaProfessionale> elencoDecodificaEnteCassaProfessionaleDB = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale;

            if (elencoDecodificaEnteCassaProfessionaleDB != null && elencoDecodificaEnteCassaProfessionaleDB.Count > 0)
            {
                GetListaDecodificaEnteCassaProfessionaleCustom(datiPensione, ref elencoDecodificaEnteCassaProfessionaleDB);

                foreach (GestioneDecodifica.DecodificaEnteCassaProfessionale decodificaEnteCassaProfessionaleDB in elencoDecodificaEnteCassaProfessionaleDB)
                {
                    DecodificaEnteCassaProfessionale decodificaEnteCassaProfessionale = new DecodificaEnteCassaProfessionale();
                    Utility.ValorizzaOggetti(decodificaEnteCassaProfessionaleDB, decodificaEnteCassaProfessionale);
                    listaDecodificaEnteCassaProfessionale.Add(decodificaEnteCassaProfessionale);
                }
            }
        }

        private static void GetListaDecodificaEnteCassaProfessionaleCustom(GestionePensione.DatiPensione datiPensione,
            ref List<GestioneDecodifica.DecodificaEnteCassaProfessionale> elencoDecodificaEnteCassaProfessionaleCustom)
        {
            if (datiPensione != null)
            {
                if (elencoDecodificaEnteCassaProfessionaleCustom != null && elencoDecodificaEnteCassaProfessionaleCustom.Count > 0)
                {
                    if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                    {
                        elencoDecodificaEnteCassaProfessionaleCustom = elencoDecodificaEnteCassaProfessionaleCustom.FindAll(x => x.TraduzioneSuGP != "0802" && x.TraduzioneSuGP != "0803" && x.TraduzioneSuGP != "0804" && x.TraduzioneSuGP != "0812");
                    }
                }
            }
        }

        public static void GetListaDecodificaBancaFideiussione(GestionePensione.DatiPensione datiPensione, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecBancaFideiussione> listaDecodificaBancaFideiussione)
        {
            listaDecodificaBancaFideiussione = new List<DecBancaFideiussione>();
            List<GestioneBancheFideiussione.DecBancaFideiussione> elencoDecodificaBancaFideiussioneDB = contenitoreDecodifica.ElencoDecBancaFideiussione;

            if (elencoDecodificaBancaFideiussioneDB != null && elencoDecodificaBancaFideiussioneDB.Count > 0)
            {
                GetListaDecodificaBancaFideiussioneCustom(datiPensione, ref elencoDecodificaBancaFideiussioneDB);

                foreach (GestioneBancheFideiussione.DecBancaFideiussione decodificaBancaFideiussioneDB in elencoDecodificaBancaFideiussioneDB)
                {
                    DecBancaFideiussione decodificaBancaFideiussione = new DecBancaFideiussione();
                    Utility.ValorizzaOggetti(decodificaBancaFideiussioneDB, decodificaBancaFideiussione);
                    listaDecodificaBancaFideiussione.Add(decodificaBancaFideiussione);
                }
            }
        }

        private static void GetListaDecodificaBancaFideiussioneCustom(GestionePensione.DatiPensione datiPensione, ref List<GestioneBancheFideiussione.DecBancaFideiussione> listaDecodificaBancaFideiussione)
        {
            if (Utility.IsDomandaVESO92_AGO(datiPensione))
                listaDecodificaBancaFideiussione.RemoveAll(x => x.Progressivo == 99);
        }

        public static void GetListaDecodificaBancaFideiussioneESPA(GestionePensione.DatiPensione datiPensione, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecBancaFideiussione> listaDecodificaBancaFideiussione)
        {
            listaDecodificaBancaFideiussione = new List<DecBancaFideiussione>();
            List<GestioneBancheFideiussioneESPA.DecBancaFideiussione> elencoDecodificaBancaFideiussioneDB = contenitoreDecodifica.ElencoDecBancaFideiussioneESPA;

            if (elencoDecodificaBancaFideiussioneDB != null && elencoDecodificaBancaFideiussioneDB.Count > 0)
            {
                foreach (GestioneBancheFideiussioneESPA.DecBancaFideiussione decodificaBancaFideiussioneDB in elencoDecodificaBancaFideiussioneDB)
                {
                    DecBancaFideiussione decodificaBancaFideiussione = new DecBancaFideiussione();
                    Utility.ValorizzaOggetti(decodificaBancaFideiussioneDB, decodificaBancaFideiussione);
                    listaDecodificaBancaFideiussione.Add(decodificaBancaFideiussione);
                }
            }
        }

        public static void GetListaAziendeScadenzaAssegnoGGMMAAAA(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.DecAziendeScadenzaAssegnoGGmmAAAA> listaAziendeScadenzaAssegnoGGMMAAAA)
        {
            listaAziendeScadenzaAssegnoGGMMAAAA = new List<DecAziendeScadenzaAssegnoGGmmAAAA>();
            List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> listaAziendeScadenzaAssegnoGGMMAAAA_BL = contenitoreDecodifica.ElencoDecAziendeScadenzaAssegnoGGmmAAAA;
            if (listaAziendeScadenzaAssegnoGGMMAAAA_BL != null && listaAziendeScadenzaAssegnoGGMMAAAA_BL.Count > 0)
            {
                foreach (var dec in listaAziendeScadenzaAssegnoGGMMAAAA_BL)
                {
                    DecAziendeScadenzaAssegnoGGmmAAAA decAzienda = new DecAziendeScadenzaAssegnoGGmmAAAA();
                    Utility.ValorizzaOggetti(dec, decAzienda);
                    listaAziendeScadenzaAssegnoGGMMAAAA.Add(decAzienda);
                }
            }
        }

        public static void GetListaDecodificaBanchePerSede(ref ContenitoreDecodifica contenitoreDecodifica, out List<DecodificaBanchePerSede> listaDecodificaBanca)
        {
            listaDecodificaBanca = new List<DecodificaBanchePerSede>();
            List<GestioneDecodifica.DecodificaBanchePerSede> elencoDecodificaBancaDB = contenitoreDecodifica.ElencoDecodificaBanchePerSede;

            if (elencoDecodificaBancaDB != null && elencoDecodificaBancaDB.Count > 0)
            {
                foreach (GestioneDecodifica.DecodificaBanchePerSede decodificaBancaDB in elencoDecodificaBancaDB)
                {
                    DecodificaBanchePerSede decodificaBanca = new DecodificaBanchePerSede();
                    Utility.ValorizzaOggetti(decodificaBancaDB, decodificaBanca);
                    listaDecodificaBanca.Add(decodificaBanca);
                }
            }
        }

        public static void GetAnagraficaAccordi(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.DecAnagraficaAccordi> listaAnagraficaAccordi)
        {
            listaAnagraficaAccordi = new List<DecAnagraficaAccordi>();
            List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> listaAnagraficaAccordi_BL = contenitoreDecodifica.ElencoDecodAnagraficaAccordi;
            if (listaAnagraficaAccordi_BL != null && listaAnagraficaAccordi_BL.Count > 0)
            {
                foreach (var dec in listaAnagraficaAccordi_BL)
                {
                    DecAnagraficaAccordi decAnagraficaAccordi = new DecAnagraficaAccordi();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAccordi);
                    listaAnagraficaAccordi.Add(decAnagraficaAccordi);
                }
            }
        }

        public static void GetAnagraficaAziende(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.DecAnagraficaAziende> listaAnagraficaAziende)
        {
            listaAnagraficaAziende = new List<DecAnagraficaAziende>();
            List<GestioneAnagraficaAziende.DecodAnagraficaAziende> listaAnagraficaAziende_BL = contenitoreDecodifica.ElencoAnagraficaAziende;
            if (listaAnagraficaAziende_BL != null && listaAnagraficaAziende_BL.Count > 0)
            {
                foreach (var dec in listaAnagraficaAziende_BL)
                {
                    DecAnagraficaAziende decAnagraficaAziende = new DecAnagraficaAziende();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAziende);
                    listaAnagraficaAziende.Add(decAnagraficaAziende);
                }
            }
        }

        public static void GetAnagraficaAccordi(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecAnagraficaAccordiPerTipo0171> listaAnagraficaAccordi)
        {
            listaAnagraficaAccordi = new List<DecAnagraficaAccordiPerTipo0171>();
            if (contenitoreDecodifica.ElencoDecodAnagraficaAccordiPerTipo0171 != null && contenitoreDecodifica.ElencoDecodAnagraficaAccordiPerTipo0171.Count > 0)
            {
                foreach (var dec in contenitoreDecodifica.ElencoDecodAnagraficaAccordiPerTipo0171)
                {
                    DecAnagraficaAccordiPerTipo0171 decAnagraficaAccordi = new DecAnagraficaAccordiPerTipo0171();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAccordi);
                    listaAnagraficaAccordi.Add(decAnagraficaAccordi);
                }
            }
        }

        public static void GetAnagraficaAziende(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecAnagraficaAziendePerTipo0171> listaAnagraficaAziende)
        {
            listaAnagraficaAziende = new List<DecAnagraficaAziendePerTipo0171>();
            if (contenitoreDecodifica.ElencoDecodAnagraficaAziendePerTipo0171 != null && contenitoreDecodifica.ElencoDecodAnagraficaAziendePerTipo0171.Count > 0)
            {
                foreach (var dec in contenitoreDecodifica.ElencoDecodAnagraficaAziendePerTipo0171)
                {
                    DecAnagraficaAziendePerTipo0171 decAnagraficaAziende = new DecAnagraficaAziendePerTipo0171();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAziende);
                    listaAnagraficaAziende.Add(decAnagraficaAziende);
                }
            }
        }
        public static void GetAnagraficaAccordi(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecAnagraficaAccordiPerTipo0179> listaAnagraficaAccordi)
        {
            listaAnagraficaAccordi = new List<DecAnagraficaAccordiPerTipo0179>();
            if (contenitoreDecodifica.ElencoDecodAnagraficaAccordiPerTipo0179 != null && contenitoreDecodifica.ElencoDecodAnagraficaAccordiPerTipo0179.Count > 0)
            {
                foreach (var dec in contenitoreDecodifica.ElencoDecodAnagraficaAccordiPerTipo0179)
                {
                    DecAnagraficaAccordiPerTipo0179 decAnagraficaAccordi = new DecAnagraficaAccordiPerTipo0179();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAccordi);
                    listaAnagraficaAccordi.Add(decAnagraficaAccordi);
                }
            }
        }

        public static void GetAnagraficaAziende(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecAnagraficaAziendePerTipo0179> listaAnagraficaAziende)
        {
            listaAnagraficaAziende = new List<DecAnagraficaAziendePerTipo0179>();
            if (contenitoreDecodifica.ElencoDecodAnagraficaAziendePerTipo0179 != null && contenitoreDecodifica.ElencoDecodAnagraficaAziendePerTipo0179.Count > 0)
            {
                foreach (var dec in contenitoreDecodifica.ElencoDecodAnagraficaAziendePerTipo0179)
                {
                    DecAnagraficaAziendePerTipo0179 decAnagraficaAziende = new DecAnagraficaAziendePerTipo0179();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAziende);
                    listaAnagraficaAziende.Add(decAnagraficaAziende);
                }
            }
        }

        public static void GetAnagraficaAccordi(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecAnagraficaAccordiLetteraB> listaAnagraficaAccordi)
        {
            listaAnagraficaAccordi = new List<DecAnagraficaAccordiLetteraB>();
            if (contenitoreDecodifica.ElencoDecodAnagraficaAccordiLetteraB != null && contenitoreDecodifica.ElencoDecodAnagraficaAccordiLetteraB.Count > 0)
            {
                foreach (var dec in contenitoreDecodifica.ElencoDecodAnagraficaAccordiLetteraB)
                {
                    DecAnagraficaAccordiLetteraB decAnagraficaAccordi = new DecAnagraficaAccordiLetteraB();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAccordi);
                    listaAnagraficaAccordi.Add(decAnagraficaAccordi);
                }
            }
        }

        public static void GetAnagraficaAziende(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecAnagraficaAziendeLetteraB> listaAnagraficaAziende)
        {
            listaAnagraficaAziende = new List<DecAnagraficaAziendeLetteraB>();
            if (contenitoreDecodifica.ElencoDecodAnagraficaAziendeLetteraB != null && contenitoreDecodifica.ElencoDecodAnagraficaAziendeLetteraB.Count > 0)
            {
                foreach (var dec in contenitoreDecodifica.ElencoDecodAnagraficaAziendeLetteraB)
                {
                    DecAnagraficaAziendeLetteraB decAnagraficaAziende = new DecAnagraficaAziendeLetteraB();
                    Utility.ValorizzaOggetti(dec, decAnagraficaAziende);
                    listaAnagraficaAziende.Add(decAnagraficaAziende);
                }
            }
        }

        public static void GetListaCtrlScadenzaIndennizzoINDCOM(ref ContenitoreDecodifica contenitoreDecodifica, out List<CtrlScadenzaIndennizzoINDCOM> listaCtrlScadenzaIndennizzoINDCOM)
        {
            listaCtrlScadenzaIndennizzoINDCOM = new List<CtrlScadenzaIndennizzoINDCOM>();
            List<GestioneDecodifica.CtrlScadenzaIndennizzoINDCOM> elencoCtrlScadenzaIndennizzoINDCOMDB = contenitoreDecodifica.ElencoCtrlScadenzaIndennizzoINDCOM;

            if (elencoCtrlScadenzaIndennizzoINDCOMDB != null && elencoCtrlScadenzaIndennizzoINDCOMDB.Count > 0)
            {
                foreach (GestioneDecodifica.CtrlScadenzaIndennizzoINDCOM CtrlScadenzaIndennizzoINDCOMDB in elencoCtrlScadenzaIndennizzoINDCOMDB)
                {
                    CtrlScadenzaIndennizzoINDCOM CtrlScadenzaIndennizzoINDCOM = new CtrlScadenzaIndennizzoINDCOM();
                    Utility.ValorizzaOggetti(CtrlScadenzaIndennizzoINDCOMDB, CtrlScadenzaIndennizzoINDCOM);
                    listaCtrlScadenzaIndennizzoINDCOM.Add(CtrlScadenzaIndennizzoINDCOM);
                }
            }
        }
        #endregion decodifica

        #region Cross Properties

        public static Dictionary<string, bool?> GetCrossProperties(ref EntityBLCommon.ContenitoreObject contenitore, bool isRiaperturaDomanda, out TipoSalvaguardia? TipologiaSalvaguardia,
            out string codiceAziendaFromPatronato, out DateTime? DecorrenzaPensioneDirettaDC, out Dictionary<string, byte?> TipoPensione, out DateTime? dataAssunzioneCarico, out DateTime? dataPrelievoDomanda, out string tipoSettimaneBeneficio)
        {
            bool? IsEsenzioneFiscaleEstero = null;
            bool? isAliquotaTfrEsodati = null;
            bool? isRiduzioneRetributiva = null;
            bool? isGestioneCOM = null;
            bool? IsCodiceNatura2Enabled = null;
            bool? IsSperimentaleDonna = null;
            TipologiaSalvaguardia = null;
            bool? IsUsuranti = null;
            bool? IsRimpatriatiAlbania = null;
            bool? IsVecchiaiaInvaliditaSupplementare = null;
            bool? IsDatiExCombattenteENPALSPresenti = null;
            bool? IsDatiBeneficiENPALSPresenti = null;
            bool? IsTabPrepensionamentoVisible = null;
            bool? IsFlagProvvisoriaCheckedAndEnabled = null;
            bool? IsRipristino = null;
            bool? isRiduzioneRetributivaEnabled = null;
            bool? isDomandaTrasformazioneInvalidita = null;
            bool? isDomandaAmianto181FromUnicarpe = null;
            bool? isDatiBeneficiSalvati = null;
            bool? isDomandaVESO92WithFiltroL92 = null;
            codiceAziendaFromPatronato = null;
            bool? isDatiCalcoloDAIAltraGestionePresent = null;
            bool isContribuzioneEnpalsRetributivaVisible = false;
            bool isContribuzioneEnpalsContributivaVisible = false;
            bool isEsenzioneFiscaleVittima = false;
            bool? isRequisitiL247_L243Enable = null;
            bool? isCodiceComunicazione3Visible = null;
            bool? isProvvisoriaVisible = null;
            DecorrenzaPensioneDirettaDC = null;
            TipoPensione = null;
            bool? isDecPensAnteAgosto95 = null;
            bool? isBeneficioArt24Comma15BisFromFELPE = null;
            bool? isPensioneTipoContributivo = null;
            bool? isPensioneTipoContributivoConOpzione = null;
            bool? isPrepensionamentoEditoriaFiltroEAA = null;
            bool? isPrepensionamentoEditoriaArt1c154L205_2017 = null;
            bool? isPrepensionamentoEditoriaArt1c500L160_2019 = null;
            bool? isBeneficioApePrecociFromFELPE = null;
            bool? isDomandaCasellario = null;
            bool? isEsenzioneFiscaleEsteroFromDetrazioni = null;
            bool? isDomandaInabilitaSpecificaENPALS = null;
            bool? isPensioneInvaliditaInabilitaENPALSOrCasellario = null;
            bool? isBeneficioInabilitaByPrimoCodiceNatura = null;
            bool? isRichiestaBonusBookingAbilitata = null;
            bool? IsRiaperturaPerCausaPersa = null;
            bool? isScadenzaStoricoValorizzata = null;
            bool? isRicEnpalsMotiviContributivi = null;
            bool? isBeneficioNonVedente = null;
            bool? isDataRinunciaTrattenutaInpdapStorico = null;
            bool? isBeneficioNonVedenteFromStorico = null;
            bool? isRichiestaBonus154Abilitata = null;
            bool? isDomandaESPAFiltroL26 = null;
            bool? isDomandaVESO33FiltroDAP = null;
            bool? isDomandaRicTrfCred27GestioneL = null;
            bool? isEliminataPerCauseVarie = null;
            bool? isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = null;
            bool? IsPrepensionamentoEditoriaFiltroEBA = null;
            bool? isRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11 = null;
            dataAssunzioneCarico = null;
            bool? IsMemo74_2023Abilitato = null;

            //ENG - Aggiornamento Memo86
            bool? isPresenteTrattenutaFondoCreditoDaPrelievo = null;
            dataPrelievoDomanda = null;

            //ENG - RIC REVERSIBILITA
            tipoSettimaneBeneficio = null;

            //ENG - RIC/TRF (NO ENPALS): rendere non obbligatori i campi "Attivita Economica" e "Professione Individuale" se dal prelievo arrivano vuoti
            bool? IsAttivitaEconomicaDaPrelievo = null;
            bool? IsProfessioneIndividualeDaPrelievo = null;

            //ENG - Memo 108_2024
            bool? IsFlagProvvisoriaFromCumulo = null;

            bool? IsBypassCompartoScuolaAttivo = null;
            //ENG - Memo 91/2026 
            bool? isDomandaCOOP28FiltroDAP = null;

            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);

            IsEsenzioneFiscaleEstero = Utility.IsEsenzioneFiscaleEstero(contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza, contenitore.DatiDetrazioni, isRiaperturaDomanda);   // generici
            isAliquotaTfrEsodati = IsAliquotaTfrEsodati(contenitore.DatiPensione);             // istruttoria
            isRiduzioneRetributiva = GestioneRiduzioneRetributiva(contenitore.DatiPensione, isRiaperturaDomanda, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null);   // istruttoria
            isGestioneCOM = GestioneCOM(contenitore.DatiPensione);                       // assicurativi
            IsCodiceNatura2Enabled = GestioneCrossControls.IsCodiceNatura2Enabled(contenitore.DatiPensione);  // generici
            IsSperimentaleDonna = Utility.IsDomandaSperimentaleDonnaOrRicostituzione(contenitore.DatiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(contenitore.DatiPensione);
            IsUsuranti = Utility.IsDomandaUsuranti(contenitore.DatiPensione);
            TipologiaSalvaguardia = GetTipoSalvaguardia(contenitore.DatiPensione); // generici
            IsRimpatriatiAlbania = Utility.IsRimpatriatiAlbania(contenitore.DatiPensione);
            IsVecchiaiaInvaliditaSupplementare = Utility.IsVecchiaiaInvaliditaSupplementare(contenitore.DatiPensione);
            IsTabPrepensionamentoVisible = Utility.IsTabPrepensionamentoVisible(contenitore.DatiPensione, contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale,
                contenitore.DatiPensione.NaturaPensione);
            IsRipristino = tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti;
            isRiduzioneRetributivaEnabled = Utility.GestioneRiduzioneRetributivaEnabled(contenitore.DatiPensione, isRiaperturaDomanda, null, null);
            isDomandaTrasformazioneInvalidita = Utility.IsDomandaTrasformazioneInvalidita(contenitore.DatiPensione);
            isDomandaAmianto181FromUnicarpe = contenitore.DatiPensione.Amianto181Unicarpe;
            isDatiBeneficiSalvati = contenitore.DatiMaggiorazioniBenefici != null ? !contenitore.DatiMaggiorazioniBenefici.IsBeneficiAGONull() : (bool?)null;
            isDomandaVESO92WithFiltroL92 = Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null);
            codiceAziendaFromPatronato = GetCodiceAziendaFromPatronato(contenitore.DatiPensione);
            isDatiCalcoloDAIAltraGestionePresent = IsDatiCalcoloDAIAltraGestionePresent(contenitore.DatiPensione, contenitore.ListaDatiRetributivi, contenitore.ListaDatiContributivi);
            isEsenzioneFiscaleVittima = Utility.IsEsenzioneFiscaleVittima(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo, contenitore.DatiDetrazioni, isRiaperturaDomanda);
            isRequisitiL247_L243Enable = GetIsRequisitiL247_L243Enable(contenitore.DatiPensione);
            isCodiceComunicazione3Visible = IsCodiceComunicazione3Visible(contenitore.DatiPensione, contenitore.DatiIstruttoria);
            isProvvisoriaVisible = IsProvvisoriaVisible(contenitore.DatiPensione, contenitore.DatiIstruttoria);
            DecorrenzaPensioneDirettaDC = GetDecorrenzaPensioneDirettaDC(contenitore.DatiPensione);
            TipoPensione = GetTipoPensione(contenitore.DatiPensione);
            isDecPensAnteAgosto95 = GetIsDecPensAnteAgosto95(contenitore.DatiPensione);      // assicurativi
            isBeneficioArt24Comma15BisFromFELPE = contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE : null;
            isPensioneTipoContributivo = Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, null);
            isPensioneTipoContributivoConOpzione = Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, true);
            isPrepensionamentoEditoriaFiltroEAA = Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione);
            isPrepensionamentoEditoriaArt1c154L205_2017 = Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione);
            isPrepensionamentoEditoriaArt1c500L160_2019 = Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione);
            isBeneficioApePrecociFromFELPE = contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE : null;
            isEsenzioneFiscaleEsteroFromDetrazioni = Utility.IsEsenzioneFiscaleEsteroFromDetrazioni(contenitore.DatiPensione, contenitore.DatiDetrazioni, isRiaperturaDomanda);
            isDomandaESPAFiltroL26 = Utility.IsDomandaESPA_L26(contenitore.DatiPensione);
            isDomandaVESO33FiltroDAP = Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione);
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                IsDatiBeneficiENPALSPresenti = VerifyDatiBeneficiENPALSPresenti(contenitore.DatiPensione, contenitore.DatiEnpals);
                IsDatiExCombattenteENPALSPresenti = VerifyDatiExCombattenteENPALSPresenti(contenitore.DatiPensione, contenitore.DatiEnpals);
                isContribuzioneEnpalsRetributivaVisible = contenitore.DatiCalcoloRetributivoENPALS != null && !contenitore.DatiCalcoloRetributivoENPALS.IsDatiCalcoloRetributivoEnpalsNull();
                isContribuzioneEnpalsContributivaVisible = contenitore.DatiCalcoloContributivoENPALS != null && !contenitore.DatiCalcoloContributivoENPALS.IsDatiCalcoloContributivoEnpalsNull();
                IsFlagProvvisoriaCheckedAndEnabled = GetIsFlagProvvisoriaCheckedAndEnabled(contenitore.DatiPensione, contenitore.DatiEnpals, isRiaperturaDomanda);
                if (Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione) && contenitore.DatiCalcoloRetributivoENPALS != null
                    && contenitore.DatiCalcoloRetributivoENPALS.ImportoProRataTemporis != null && contenitore.DatiCalcoloRetributivoENPALS.ImportoProRataTemporis > 0M)
                    isRicEnpalsMotiviContributivi = true;
            }
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
            {
                isDomandaCasellario = contenitore.DatiDanteCausa != null ? Utility.IsDomandaCasellario(contenitore.DatiDanteCausa.SiglaCategoria, contenitore.DatiDanteCausa.Sede) : (bool?)null;
            }
            isDomandaInabilitaSpecificaENPALS = Utility.IsDomandaInvaliditaSpecifica(contenitore.DatiPensione) && Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione);
            isPensioneInvaliditaInabilitaENPALSOrCasellario = IsPensioneInvaliditaInabilitaENPALSOrCasellario(ref contenitore);
            isBeneficioInabilitaByPrimoCodiceNatura = Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) &&
                (contenitore.DatiPensione.SiglaCategoria.StartsWith("I") || Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria)) &&
                !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && (contenitore.DatiPensione.NaturaPensione.StartsWith("3") || contenitore.DatiPensione.NaturaPensione.StartsWith("4")) && !Utility.IsDomandaIMP(contenitore.DatiPensione);

            GestioneControlliDinamici.ControlloDinamico ctrl = null;

            short sede = Utility.GetCodiceSedeLavorazione(contenitore.DatiPensione, isRiaperturaDomanda);
            if (Utility.IsBonusBooking(contenitore.DatiPensione))
            {
                GestioneControlliDinamici.ControlloDinamico sediDaControllare = null;

                if (contenitore.DatiPensione.Tipo == "0167") //BONUS 14°
                {
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingAGO", out ctrl);
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingSediAGO", out sediDaControllare);

                    if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                        (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                         sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == sede.ToString().PadLeft(4, '0')))))
                    {
                        isRichiestaBonusBookingAbilitata = true;
                    }
                }
                else //BONUS 154
                {
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154AGO", out ctrl);
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154SediAGO", out sediDaControllare);

                    if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                        (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                         sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == sede.ToString().PadLeft(4, '0')))))
                    {
                        isRichiestaBonus154Abilitata = true;
                    }
                }
            }


            IsRiaperturaPerCausaPersa = contenitore.DatiLavorazione.CodFase == "0036";

            if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && contenitore.DatiStoricoGP != null)
            {
                isScadenzaStoricoValorizzata = contenitore.DatiStoricoGP.ScadenzaAssegno.HasValue;
            }

            if (contenitore.DatiMaggiorazioniBenefici != null && contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01")
                isBeneficioNonVedente = true;

            if (contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.DataRinunciaTrattenutaInpdap.HasValue)
                isDataRinunciaTrattenutaInpdapStorico = true;

            if (contenitore.DatiStoricoGP != null && !string.IsNullOrEmpty(contenitore.DatiStoricoGP.TipoSettimaneBeneficio) && contenitore.DatiStoricoGP.TipoSettimaneBeneficio == "01")
                isBeneficioNonVedenteFromStorico = true;

            if (Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count > 0)
            {
                GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneL = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null ? (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "L") ? contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "L") : null) : null;
                bool esisteGestioneL = contenitore.ListaDatiContributivi.Exists(x => x.CodiceGestione == decGestioneL.Id);
                // Verifico che sia presente solo la Gestione L (che non ha quota) e non sia presente alcuna quota C
                bool esisteQuotaC = GestioneControlli.VerificaPresenzaQuotaC_Cred27(ref contenitore, ref contenitoreDecodifica, contenitore.ListaDatiContributivi);
                if (esisteGestioneL && !esisteQuotaC)
                    isDomandaRicTrfCred27GestioneL = true;
            }

            isEliminataPerCauseVarie = contenitore.DatiEliminazione != null && contenitore.DatiEliminazione.CodiceMotivo == 3 ? true : false;

            isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione, true, true) ||
                                                                         Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione);

            IsPrepensionamentoEditoriaFiltroEBA = Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione);

            if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && contenitore.DatiPensione.NaturaPensione.Substring(2, 1).Equals("Z") && contenitore.DatiPensione.AttivitaEconomica.GetValueOrDefault() == 67 && contenitore.DatiPensione.ProfessioneIndividuale.GetValueOrDefault() == 011 &&
                contenitore.DatiMaggiorazioniBenefici == null)
                isRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11 = true;

            dataAssunzioneCarico = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null;

            //ENG - Aggiornamento Memo86
            if (contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.TrattenutaFondoCredito.HasValue)
                isPresenteTrattenutaFondoCreditoDaPrelievo = contenitore.DatiStoricoGP.TrattenutaFondoCredito.Value;
            else
                isPresenteTrattenutaFondoCreditoDaPrelievo = null;

            GestioneLogSoap.GetTimestampMinimo(contenitore.DatiPensione.NDomus, out dataPrelievoDomanda);

            //ENG - REVERSIBILITA
            if (contenitore != null && contenitore.DatiStoricoGP != null && !String.IsNullOrEmpty(contenitore.DatiStoricoGP.TipoSettimaneBeneficio))
                tipoSettimaneBeneficio = contenitore.DatiStoricoGP.TipoSettimaneBeneficio.Trim();

            //ENG - RIC/TRF (NO ENPALS): rendere non obbligatori i campi "Attivita Economica" e "Professione Individuale" se dal prelievo arrivano vuoti
            if (contenitore.DatiStoricoGP != null)
            {
                if (contenitore.DatiStoricoGP.AttivitaEconomica.HasValue && contenitore.DatiStoricoGP.AttivitaEconomica.Value != 0)
                    IsAttivitaEconomicaDaPrelievo = true;
                else
                    IsAttivitaEconomicaDaPrelievo = null;

                if (contenitore.DatiStoricoGP.ProfessioneIndividuale.HasValue && contenitore.DatiStoricoGP.ProfessioneIndividuale.Value != 0)
                    IsProfessioneIndividualeDaPrelievo = true;
                else
                    IsProfessioneIndividualeDaPrelievo = null;
            }

            //ENG - MEMO 74_2023
            if (Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo74_2023", out ctrlMemo74_2023);
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioniEstere);
                if (ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI")
                {
                    if ((!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "V") ||
                        (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0))
                        IsMemo74_2023Abilitato = true;
                }
            }

            //ENG - Memo 108_2024
            if (Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlMemo108_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo108_2024", out ctrlMemo108_2024);

                if (ctrlMemo108_2024 != null && !String.IsNullOrEmpty(ctrlMemo108_2024.ValoreControllo) && ctrlMemo108_2024.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    if (contenitore.DatiNuoveLiquidate != null && contenitore.DatiNuoveLiquidate.FlagProvvisoria.HasValue && contenitore.DatiNuoveLiquidate.FlagProvvisoria.Value
                        && contenitore.DatiNuoveLiquidate.IsFlagProvvisoriaFromCumulo.HasValue && contenitore.DatiNuoveLiquidate.IsFlagProvvisoriaFromCumulo.Value)
                    {
                        IsFlagProvvisoriaFromCumulo = true;
                    }
                }
            }

            //ENG - Per le Vocum Ape Precoci se viene inserito il bypass COMPARTO_SCUOLA per il controllo sul pannello titolare: “La decorrenza pensione deve essere di almeno 3 mesi successiva alla data di perfezionamento dei requisiti”,
            // bisogna valorizzare il secondo byte codice natura = “S” e renderlo non editabile
            if (Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione))
            {
                List<GestioneBypassControllo.DatiBypassControllo> listaBypassApplicatiPerNDomus = null;
                GestioneBypassControllo.GetBypassApplicatiPerNDomus(contenitore.DatiPensione.NDomus, out listaBypassApplicatiPerNDomus);
                if (listaBypassApplicatiPerNDomus != null && listaBypassApplicatiPerNDomus.Count() > 0 && listaBypassApplicatiPerNDomus.Exists(x => x.IdDecBypassControllo == 30164))
                    IsBypassCompartoScuolaAttivo = true;
            }

            //ENG - Memo 91/2026 
            isDomandaCOOP28FiltroDAP = Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione);

            lReturn.Add("IsEsenzioneFiscaleEstero", IsEsenzioneFiscaleEstero);
            lReturn.Add("IsAliquotaTfrEsodati", isAliquotaTfrEsodati);
            lReturn.Add("IsRiduzioneRetributiva", isRiduzioneRetributiva);
            lReturn.Add("isGestioneCOM", isGestioneCOM);
            lReturn.Add("IsCodiceNatura2Enabled", IsCodiceNatura2Enabled);
            lReturn.Add("IsSperimentaleDonna", IsSperimentaleDonna);
            lReturn.Add("Usuranti", IsUsuranti);
            lReturn.Add("IsRimpatriatiAlbania", IsRimpatriatiAlbania);
            lReturn.Add("IsVecchiaiaInvaliditaSupplementare", IsVecchiaiaInvaliditaSupplementare);
            lReturn.Add("IsDatiExCombattenteENPALSPresenti", IsDatiExCombattenteENPALSPresenti);
            lReturn.Add("IsDatiBeneficiENPALSPresenti", IsDatiBeneficiENPALSPresenti);
            lReturn.Add("IsTabPrepensionamentoVisible", IsTabPrepensionamentoVisible);
            lReturn.Add("IsFlagProvvisoriaCheckedAndEnabled", IsFlagProvvisoriaCheckedAndEnabled);
            lReturn.Add("IsRipristino", IsRipristino);
            lReturn.Add("IsRiduzioneRetributivaEnabled", isRiduzioneRetributivaEnabled);
            lReturn.Add("IsDomandaTrasformazioneInvalidita", isDomandaTrasformazioneInvalidita);
            lReturn.Add("IsDomandaAmianto181FromUnicarpe", isDomandaAmianto181FromUnicarpe);
            lReturn.Add("IsDatiBeneficiSalvati", isDatiBeneficiSalvati);
            lReturn.Add("IsDomandaVESO92WithFiltroL92", isDomandaVESO92WithFiltroL92);
            lReturn.Add("IsDatiCalcoloDAIAltraGestionePresent", isDatiCalcoloDAIAltraGestionePresent);
            lReturn.Add("IsContribuzioneEnpalsRetributivaVisible", isContribuzioneEnpalsRetributivaVisible);
            lReturn.Add("IsContribuzioneEnpalsContributivaVisible", isContribuzioneEnpalsContributivaVisible);
            lReturn.Add("IsEsenzioneFiscaleVittima", isEsenzioneFiscaleVittima);
            lReturn.Add("IsRequisitiL247_L243Enable", isRequisitiL247_L243Enable);
            lReturn.Add("IsCodiceComunicazione3Visible", isCodiceComunicazione3Visible);
            lReturn.Add("IsProvvisoriaVisible", isProvvisoriaVisible);
            lReturn.Add("DecPensAnteAgosto95", isDecPensAnteAgosto95);
            lReturn.Add("IsBeneficioArt24Comma15BisFromFELPE", isBeneficioArt24Comma15BisFromFELPE);
            lReturn.Add("IsPensioneTipoContributivo", isPensioneTipoContributivo);
            lReturn.Add("IsPensioneTipoContributivoConOpzione", isPensioneTipoContributivoConOpzione);
            lReturn.Add("IsPrepensionamentoEditoriaFiltroEAA", isPrepensionamentoEditoriaFiltroEAA);
            lReturn.Add("IsPrepensionamentoEditoriaArt1c154L205_2017", isPrepensionamentoEditoriaArt1c154L205_2017);
            lReturn.Add("IsPrepensionamentoEditoriaArt1c500L160_2019", isPrepensionamentoEditoriaArt1c500L160_2019);
            lReturn.Add("IsBeneficioApePrecociFromFELPE", isBeneficioApePrecociFromFELPE);
            lReturn.Add("IsDomandaCasellario", isDomandaCasellario);
            lReturn.Add("IsEsenzioneFiscaleEsteroFromDetrazioni", isEsenzioneFiscaleEsteroFromDetrazioni);
            lReturn.Add("IsDomandaInabilitaSpecificaENPALS", isDomandaInabilitaSpecificaENPALS);
            lReturn.Add("IsPensioneInvaliditaInabilitaENPALSOrCasellario", isPensioneInvaliditaInabilitaENPALSOrCasellario);
            lReturn.Add("IsBeneficioInabilitaByPrimoCodiceNatura", isBeneficioInabilitaByPrimoCodiceNatura);
            lReturn.Add("IsRichiestaBonusBookingAbilitata", isRichiestaBonusBookingAbilitata);
            lReturn.Add("IsRiaperturaPerCausaPersa", IsRiaperturaPerCausaPersa);
            lReturn.Add("IsScadenzaStoricoValorizzata", isScadenzaStoricoValorizzata);
            lReturn.Add("IsRicEnpalsMotiviContributivi", isRicEnpalsMotiviContributivi);
            lReturn.Add("IsBeneficioNonVedente", isBeneficioNonVedente);
            lReturn.Add("IsDataRinunciaTrattenutaInpdapStorico", isDataRinunciaTrattenutaInpdapStorico);
            lReturn.Add("IsBeneficioNonVedenteFromStorico", isBeneficioNonVedenteFromStorico);
            lReturn.Add("IsRichiestaBonus154Abilitata", isRichiestaBonus154Abilitata);
            lReturn.Add("IsDomandaESPAFiltroL26", isDomandaESPAFiltroL26);
            lReturn.Add("IsDomandaVESO33FiltroDAP", isDomandaVESO33FiltroDAP);
            lReturn.Add("IsDomandaRicTrfCred27GestioneL", isDomandaRicTrfCred27GestioneL);
            lReturn.Add("IsEliminataPerCauseVarie", isEliminataPerCauseVarie);
            lReturn.Add("IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione", isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione);
            lReturn.Add("IsPrepensionamentoEditoriaFiltroEBA", IsPrepensionamentoEditoriaFiltroEBA);
            lReturn.Add("IsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11", isRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11);

            //ENG - Aggiornamento Memo86
            lReturn.Add("IsPresenteTrattenutaFondoCreditoDaPrelievo", isPresenteTrattenutaFondoCreditoDaPrelievo);

            //ENG - RIC/TRF (NO ENPALS): rendere non obbligatori i campi "Attivita Economica" e "Professione Individuale" se dal prelievo arrivano vuoti
            lReturn.Add("IsAttivitaEconomicaDaPrelievo", IsAttivitaEconomicaDaPrelievo);
            lReturn.Add("IsProfessioneIndividualeDaPrelievo", IsProfessioneIndividualeDaPrelievo);

            lReturn.Add("IsMemo74_2023Abilitato", IsMemo74_2023Abilitato);

            lReturn.Add("IsFlagProvvisoriaFromCumulo", IsFlagProvvisoriaFromCumulo);
            lReturn.Add("IsBypassCompartoScuolaAttivo", IsBypassCompartoScuolaAttivo);
            lReturn.Add("IsDomandaCOOP28FiltroDAP", isDomandaCOOP28FiltroDAP);

            return lReturn;
        }

        private static string GetCodiceAziendaFromPatronato(GestionePensione.DatiPensione datiPensione)
        {
            string traduzioneSuGpAzienda = null;
            if (Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
            {
                GestionePensione.DatiPatronato datiPatronato = null;
                GestionePensione.GetPatronatoByIdPensione(datiPensione.Id, out datiPatronato);
                if (datiPatronato != null && datiPatronato.isAzienda())
                {
                    traduzioneSuGpAzienda = datiPatronato.CodiceUfficio;
                }
            }
            return traduzioneSuGpAzienda;
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

        private static bool? IsAliquotaTfrEsodati(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            //Aliquota TFR esodati è da visualizzare solo per le categorie 027 (VOCRED), 028 (VOCOOP), 029 (VOESO) e quindi deve essere nascosta per le altre
            if (!Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) && !Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria))
                return false;

            return true;
        }

        private static bool? GestioneRiduzioneRetributiva(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, string GP2BB05)
        {
            if (datiPensione == null)
                return false;

            if (Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria))
                return true;

            if (Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione))
                return false;

            ////ENG - RIC NUOVA OPZIONE DONNA
            if (Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione))
                return true;

            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
            {
                if (Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) && datiPensione.CodiceBancaEsodati.HasValue)
                {
                    GestioneDecodificaAzienda.DecAzienda codiceBancaEsodati;
                    List<GestioneDecodificaAzienda.DecAzienda> listaDecAzienda = null;
                    GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(datiPensione.SiglaCategoria, null, out listaDecAzienda);
                    if (listaDecAzienda != null && listaDecAzienda.Count > 0)
                    {
                        codiceBancaEsodati = listaDecAzienda.Find(x => x.Id == datiPensione.CodiceBancaEsodati.Value);
                        if (codiceBancaEsodati != null)
                        {
                            if (Utility.IsDomandaVOESOFerrovieDelloStatoRicConFiltro(datiPensione, isRiaperturaDomanda, GP2BB05, codiceBancaEsodati.TraduzioneSuGP))
                                return true;
                        }
                    }
                }
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
                if (Utility.IsAssegnoStraordinarioFerrovieDelloStato(datiPensione) && datiPensione.CodiceTipoRichiesta == "74")
                    return true;
                if (Utility.IsDomandaPSO(datiPensione.SiglaCategoria) || Utility.IsDomandaPMO(datiPensione.SiglaCategoria))
                    return true;
                //riduzione retributiva non è visibile per pensioni diverse da anzianità (gruppo 0001 e prodotto 0001)
                if (string.IsNullOrEmpty(datiPensione.Gruppo) || datiPensione.Gruppo != "0001")
                    return false;
                if (string.IsNullOrEmpty(datiPensione.Prodotto) || datiPensione.Prodotto != "0001")
                    return false;
                if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione))
                    return false;
                if (!datiPensione.DataPerfezionamentoRequisiti.HasValue || (datiPensione.DataPerfezionamentoRequisiti.HasValue && DateTime.Compare(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 12, 31).Date) <= 0))
                    return false;
            }
            return true;
        }

        private static bool GestioneCOM(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            //True solo per le categorie VOCOM, IOCOM, SOCOM e quindi deve essere nascosta per le altre
            if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() != "VOCOM" && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() != "IOCOM" && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() != "SOCOM")
                return false;

            return true;
        }

        private static bool? VerifyDatiExCombattenteENPALSPresenti(GestionePensione.DatiPensione datiPensione, GestioneEnpals.DatiEnpals datiENPALS)
        {
            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiENPALS != null)
            {
                if (datiENPALS.NumeroContributiNLNonVedenti.HasValue)
                    return true;
                else
                    return false;
            }

            return null;
        }

        private static bool? VerifyDatiBeneficiENPALSPresenti(GestionePensione.DatiPensione datiPensione, GestioneEnpals.DatiEnpals datiENPALS)
        {
            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiENPALS != null)
            {
                if (datiENPALS.IndicatoreInvalidita80.HasValue || datiENPALS.NumeroContributiNLNonVedenti.HasValue)
                    return true;
                else
                    return false;
            }

            return null;
        }

        private static bool? GetIsFlagProvvisoriaCheckedAndEnabled(GestionePensione.DatiPensione datiPensione, GestioneEnpals.DatiEnpals datiENPALS, bool isRiaperturaDomanda)
        {
            if (Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                if (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || isRiaperturaDomanda)
                    return false;

                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                {
                    if (datiENPALS != null && !string.IsNullOrEmpty(datiENPALS.TipoLiquidazione) && datiENPALS.TipoLiquidazione == "0")
                        return false;
                    else if (datiENPALS != null && !string.IsNullOrEmpty(datiENPALS.TipoLiquidazioneProvvisoria) && datiENPALS.TipoLiquidazioneProvvisoria == "0")
                        return true;
                }

                if (!Utility.IsDomandaReversibilita(datiPensione) && datiENPALS != null && !string.IsNullOrEmpty(datiENPALS.TipoLiquidazione) &&
                    datiENPALS.TipoLiquidazione == "8" && !(datiENPALS.TipoLiquidazioneProvvisoria == "3" || datiENPALS.TipoLiquidazioneProvvisoria == "4"))
                    return true;
            }

            return false;
        }

        internal static bool? IsDatiCalcoloDAIAltraGestionePresent(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloRetributivo> datiCalcoloRetributivo,
            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo)
        {
            if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) && datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0011" && datiPensione.Tipo == "0001" &&
                ((datiCalcoloRetributivo != null && datiCalcoloRetributivo.Count > 0) || (datiCalcoloContributivo != null && datiCalcoloContributivo.Count > 0)))
            {
                if ((datiCalcoloRetributivo != null && datiCalcoloRetributivo.Exists(x => x.CodiceGestione.GetValueOrDefault() == 2 || x.CodiceGestione.GetValueOrDefault() == 3 || x.CodiceGestione.GetValueOrDefault() == 4)) ||
                    (datiCalcoloContributivo != null && datiCalcoloContributivo.Exists(x => x.CodiceGestione.GetValueOrDefault() == 2 || x.CodiceGestione.GetValueOrDefault() == 3 || x.CodiceGestione.GetValueOrDefault() == 4)))
                    return true;
                else
                    return false;
            }

            return null;
        }

        private static bool? GetIsRequisitiL247_L243Enable(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.DataPerfezionamentoRequisiti.HasValue && Liquidazione.BLCommon.Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 01, 01)))
                return false;

            return true;
        }

        private static bool IsCodiceComunicazione3Visible(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            switch (tipoDomanda)
            {
                case Utility.TipoDomanda.Ricostituzione:
                case Utility.TipoDomanda.Ripristino:
                case Utility.TipoDomanda.RipristinoSuperstiti:
                    if (datiIstruttoria == null || (!(datiIstruttoria.Provvisoria.HasValue && datiIstruttoria.Provvisoria.Value) &&
                        (!datiIstruttoria.CodiceComunicazioneCampo3.HasValue || datiIstruttoria.CodiceComunicazioneCampo3.Value == ' ' || datiIstruttoria.CodiceComunicazioneCampo3.Value == 'Q')))
                        return false;
                    break;
            }

            return true;
        }

        private static bool IsProvvisoriaVisible(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            switch (tipoDomanda)
            {
                case Utility.TipoDomanda.Ricostituzione:
                case Utility.TipoDomanda.Ripristino:
                case Utility.TipoDomanda.RipristinoSuperstiti:
                    if (datiIstruttoria == null || !datiIstruttoria.Provvisoria.HasValue || !datiIstruttoria.Provvisoria.Value)
                        return false;
                    break;
            }

            return true;
        }

        private static bool IsPensioneInvaliditaInabilitaENPALSOrCasellario(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            if (Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) &&
                Utility.IsPensioneInvaliditaInabilitaENPALSOrCasellario(contenitore.DatiDanteCausa) &&
                (contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue && Utility.DataSuccessivaA(contenitore.DatiDanteCausa.DecorrenzaPensione.Value, new DateTime(1984, 8, 1)) &&
                (string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) || (contenitore.DatiPensione.NaturaPensione.Substring(0, 1) != "3" && contenitore.DatiPensione.NaturaPensione.Substring(0, 1) != "4" && (contenitore.DatiDanteCausa.SiglaCategoria == "IOSPORT" || contenitore.DatiPensione.NaturaPensione.Substring(1, 1) != "1")))))
                return true;

            return false;
        }

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

        private static Dictionary<string, byte?> GetTipoPensione(GestionePensione.DatiPensione datiPensione)
        {
            Dictionary<string, byte?> tipoPensione = new Dictionary<string, byte?>();

            if (datiPensione.SiglaCategoria.StartsWith("V"))
            {
                tipoPensione.Add("VECCHIAIA", 1);
                return tipoPensione;
            }
            if (datiPensione.SiglaCategoria.StartsWith("I"))
            {
                tipoPensione.Add("INVALIDITA'", 2);
                return tipoPensione;
            }
            if (datiPensione.SiglaCategoria.StartsWith("S"))
            {
                GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);
                if (danteCausa != null && !string.IsNullOrEmpty(danteCausa.SiglaCategoria))
                {
                    if (danteCausa.SiglaCategoria.StartsWith("V"))
                    {
                        tipoPensione.Add("VECCHIAIA", 1);
                        return tipoPensione;
                    }
                    if (danteCausa.SiglaCategoria.StartsWith("I"))
                    {
                        tipoPensione.Add("INVALIDITA'", 2);
                        return tipoPensione;
                    }
                }
                else
                    tipoPensione.Add("INDIRETTA", 3);
                return tipoPensione;
            }

            return null;
        }

        private static bool? GetIsDecPensAnteAgosto95(GestionePensione.DatiPensione datiPensione)
        {
            DateTime dataCompare = new DateTime(1995, 8, 17);
            if (datiPensione.DecorrenzaOriginaria.HasValue && !Liquidazione.BLCommon.Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataCompare))
                return true;
            else return false;
        }

        #endregion Cross Properties



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
