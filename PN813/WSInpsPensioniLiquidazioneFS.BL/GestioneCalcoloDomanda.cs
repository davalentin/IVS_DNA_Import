using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Configuration;
using System.Reflection;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneCalcoloDomanda
    {
        #region public members
        public static void CalcolaDomanda(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isReingegnerizzato, out string statoPensione,
            out int certificato, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = "";
            certificato = 0;
            messaggioVideo = "";
            Data.FSPL_FSRC AreaCalcolo = null;
            Guid guid = Guid.NewGuid();
            ValorizzaAreaCalcolo(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, true, isReingegnerizzato, out AreaCalcolo);

            Utility.MetodoServizio? metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaCalcolo.TransactionName);
            GestioneLogSoap.SalvaLogSoap(AreaCalcolo.AreaInputVariabile, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

            EseguiCalcolo(AreaCalcolo);

            if (AreaCalcolo.Response != null && AreaCalcolo.Response.Dati != null && AreaCalcolo.Response.Dati.Stampa != null
                && AreaCalcolo.Response.Dati.Stampa.Anagrafica != null && AreaCalcolo.Response.Dati.Stampa.Anagrafica.FLAG_INDEB != null && AreaCalcolo.Response.Dati.Stampa.Anagrafica.FLAG_INDEB.Trim() != "0")
                datiPensione.FlagIndebito = AreaCalcolo.Response.Dati.Stampa.Anagrafica.FLAG_INDEB;

            if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaCalcolo.MessaggioDaLoggare, null, null);

            if (AreaCalcolo.HasError)
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Messaggio, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
            else
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Response, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);

            if (AreaCalcolo.Request != null && AreaCalcolo.Request.AR_TIPO == "ELI")
            {
                AreaCalcolo = null;
                ValorizzaAreaCalcolo(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, false, isReingegnerizzato, out AreaCalcolo);

                metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaCalcolo.TransactionName);
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo.AreaInputVariabile, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

                EseguiCalcolo(AreaCalcolo);
                if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                    GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaCalcolo.MessaggioDaLoggare, null, null);

                if (AreaCalcolo.HasError)
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Messaggio, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Response, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
            }
            ControllaEsitoCalcolo(datiPensione.NDomus, AreaCalcolo, datiPensione, out statoPensione, out certificato, out esito, out messaggioVideo);
        }

        public static bool ControlsDatiCalcolaDomanda(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, string matricolaOperatore, bool isConsultazioniANFVerificate, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            listaConsultazioniANF = null;

            if (datiPensione == null)
            {
                messaggioVideo = "Errore nel recupero delle informazioni.";
                return false;
            }

            #region GetData

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(datiPensione);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.TipoAppartenenza.FS;
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            Utility.TipoFondo? tipoFondo = null;
            if (datiPensione != null)
                tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            List<GestioneFamiliari.Familiare> Lfamiliare = null;
            List<GestioneAnagrafica.DatiAnagrafici> LAnagraficheFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out Lfamiliare, out LAnagraficheFamiliari);
            List<GestioneFamiliari.Familiare> LfamiliariCompleta = null;
            List<GestioneAnagrafica.DatiAnagrafici> LAnagraficheFamiliariCompleta = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out LfamiliariCompleta, out LAnagraficheFamiliariCompleta);
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if (Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)))
            {
                string codiceFiscaleTitolare = areaTitolare.Anagrafica.CodiceFiscale;
                Lfamiliare = Lfamiliare.FindAll(x => x.CodiceFiscale != codiceFiscaleTitolare);
            }

            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
            GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaCodMaggFamiliari);

            List<Liquidazione.BLCommon.Entity.DatiSupplementi> listDatiSupplementi = null;
            GestioneSupplementi.GetSupplementiByIdPensione(datiPensione.Id, out listDatiSupplementi);

            GestioneFondo.DatiFondo datiFondo = null;
            GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);

            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out listaRecordFondo);

            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi = null;
            GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiContributivi);

            GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi = null;
            if (tipoFondo == Utility.TipoFondo.DZ)
                GestioneCalcolo.GetCalcoloRetributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiRetributivi);
            else
                GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiRetributivi);

            object datiFelpe = null;
            GestioneContrib.DatiCalcolo datiCalcolo = null;
            GestioneContrib.GetDatiCalcoloByDomandaFelpe(datiPensione, datiMaggiorazioniBenefici, datiFondo, isRiaperturaDomanda, out datiCalcolo, out datiFelpe, out messaggioVideo);

            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);

            List<GestioneRedditi.RedditoDRedd> lstRedditi = null;
            GestioneRedditi.GetRedditiDReddByIdPensione(datiPensione.Id, out lstRedditi);

            //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            //bypassare controllo per i fondi non aventi il tipo calcolo
            if (tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PI && tipoFondo.Value != Utility.TipoFondo.PL)
            {
                if (!string.IsNullOrEmpty(messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }
            }

            else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                if (!string.IsNullOrEmpty(messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }
            }



            GestioneAnagrafica.DatiAnagrafici datiDelegato = null;
            GestioneDelegatoTutore.GetDelegatoByIdPensione(datiPensione.Id, out datiDelegato);

            GestioneAnagrafica.DatiAnagrafici datiTutore = null;
            GestioneDelegatoTutore.GetTutoreByIdPensione(datiPensione.Id, out datiTutore);

            GestionePensione.DatiSindacato sindacato = null;
            GestionePensione.GetSindacatoByIdPensione(datiPensione.Id, out sindacato);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficaTitolare);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out datiAnagraficiDC);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
            GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listaDatiServizioUtile);

            GestioneDL407.DatiDL407 datiDl407 = null;
            GestioneDL407.GetDL407ByIdPensione(datiPensione.Id, out datiDl407);

            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaDatiOneri);

            GestionePagamento.DatiPagamento datiPagamento = null;
            GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);

            List<GestioneDetrazioniContitolare.DatiDetrazioniContitolare> listaDetrazioniContitolare = null;
            GestioneDetrazioniContitolare.GetDetrazioniByIdPensione(datiPensione.Id, out listaDetrazioniContitolare);

            List<GestioneDecodifica.GruppoOneri> elencoDecCodeGruppoOneri = null;
            GestioneDecodifica.GetGruppoOneri(out elencoDecCodeGruppoOneri);

            GestioneFondo.DatiFondoEL datiFondoEL = null;
            GestioneFondo.DatiFondoTT datiFondoTT = null;
            GestioneFondo.DatiFondoET datiFondoET = null;
            GestioneFondo.DatiFondoVL datiFondoVL = null;
            List<GestioneFondo.DatiFondoFST> listaDatiFondoFS = null;
            List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = null;
            List<GestioneFondo.DatiFondoPI> listaDatiFondoPI = null;
            GestioneFondo.DatiFondoCL datiFondoCL = null;
            GestioneFondo.DatiFondoES datiFondoES = null;
            List<GestioneFondo.DatiFondoDZ> listaDatiFondoDZ = null;
            GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            GestioneFondo.DatiFondoPM datiFondoPM = null;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiFondoINPDAP = null;

            Object objectFondoXX = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                        GestioneFondo.GetFondoELByIdPensione(datiPensione.Id, out datiFondoEL);
                        objectFondoXX = datiFondoEL;
                        break;
                    case Utility.TipoFondo.TT:
                        GestioneFondo.GetFondoTTByIdPensione(datiPensione.Id, out datiFondoTT);
                        objectFondoXX = datiFondoTT;
                        break;
                    case Utility.TipoFondo.ET:
                        GestioneFondo.GetFondoETByIdPensione(datiPensione.Id, out datiFondoET);
                        objectFondoXX = datiFondoET;
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneFondo.GetFondoVLByIdPensione(datiPensione.Id, out datiFondoVL);
                        objectFondoXX = datiFondoVL;
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.GetFondoFSRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoFS);
                        objectFondoXX = listaDatiFondoFS;
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.GetFondoPTRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoPT);
                        objectFondoXX = listaDatiFondoPT;
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.GetFondoPIRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoPI);
                        objectFondoXX = listaDatiFondoPI;
                        break;
                    case Utility.TipoFondo.CL:
                        GestioneFondo.GetFondoCLByIdPensione(datiPensione.Id, out datiFondoCL);
                        objectFondoXX = datiFondoCL;
                        break;
                    case Utility.TipoFondo.ES:
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        objectFondoXX = datiFondoES;
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.GetFondoDZRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoDZ);
                        objectFondoXX = listaDatiFondoDZ;
                        break;
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.GetFondoGASByIdPensione(datiPensione.Id, out datiFondoGAS);
                        objectFondoXX = datiFondoGAS;
                        break;
                    case Utility.TipoFondo.PM:
                        GestioneFondo.GetFondoPMByIdPensione(datiPensione.Id, out datiFondoPM);
                        objectFondoXX = datiFondoPM;
                        break;
                }
            }
            else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestionePensioneINPDAP.GetPensioneINPDAPRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoINPDAP);
                objectFondoXX = listaDatiFondoINPDAP;
            }

            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(datiPensione.Id, out listaRecordDatiFondoINPDAP);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
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

            //// Questa gestione è stata inserita perchè per i fondi FS e PT i dati retributivi vengono salvati nella tabella DatiServizioUtile.
            //// Normalmente questo problema viene risolto aggiungendo un record retributivo vuoto a DB in fase di salvataggio dei Dati Calcolo.
            //// Nel caso di ricostituzioni è possibile che non sia visibile il quadro dei Dati Calcolo, quindi viene creato direttamente qui il record vuoto.
            //// Il record retributivo vuoto serve per bypassare i controlli.
            if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.FS || tipoFondo.Value == Utility.TipoFondo.PT))
            {
                if (datiRetributivi == null && datiCalcolo != null &&
                    ((datiCalcolo.fondoFST != null && datiCalcolo.fondoFST.lDatiServizioUtile != null && datiCalcolo.fondoFST.lDatiServizioUtile.Count > 0) ||
                    (datiCalcolo.fondoPT != null && datiCalcolo.fondoPT.lDatiServizioUtile != null && datiCalcolo.fondoPT.lDatiServizioUtile.Count > 0)))
                {
                    datiRetributivi = new GestioneCalcolo.DatiCalcoloRetributivo();
                }
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

            Entity.DatiAssicurativi datiAssicurativiEntity = null;
            List<Entity.RecordFondo> listaRecordFondoEntity = null;
            GestioneLiquidazionePensione.GetDatiAssicurativi(ref contenitore, datiPensione, datiFondo, isRiaperturaDomanda, out datiAssicurativiEntity, out listaRecordFondoEntity);

            string attivitaSvoltaTraduzioneSuGP = string.Empty;
            if (datiFondo != null)
            {
                GestioneDecodifica.AttivitaSvolta attivitaSvolta = null;
                GestioneDecodifica.GetAttivitaSvoltaById(datiFondo.AttivitaSvolta, out attivitaSvolta);
                if (attivitaSvolta != null)
                    attivitaSvoltaTraduzioneSuGP = attivitaSvolta.TraduzioneSuGp;
            }

            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            #endregion GetData

            #region Controlli preliminari
            if (GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.FS.BLOCCOCALCOLO_ESTERO, dataSistema) &&
                GestioneCrossControls.ALL_VerificaBloccoCalcoloEstero(datiAnagraficaTitolare.CodiceComuneResidenza, datiPagamento))
            {
                messaggioVideo = "Invio al calcolo temporaneamente non disponibile per domande con titolare residente all'estero e/o avente modalità di pagamento estera.";
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaBloccoCalcoloAnticipata2019(datiPensione, Utility.TipoAppartenenza.FS, dataSistema, out messaggioVideo))
                return false;

            //RINNOVO RIC/TRF
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.ControlloDinamico ctrlValorizzaAnnoCompetenzaPrelievoFS = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoFS", out ctrlValorizzaAnnoCompetenzaPrelievoFS);

            // se è una RIC o TRF e ci troviamo in fase di interregno, isRicRinnovata deve essere true se no scatta il controllo
            if (ctrlValorizzaAnnoCompetenzaPrelievoFS != null && ctrlValorizzaAnnoCompetenzaPrelievoFS.ValoreControllo == "SI")
            {
                if ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno) && !datiPensione.IsRicRinnovata.HasValue)
                {
                    messaggioVideo = "Pensione non rinnovata cancellare e riprelevare la domanda.";
                    return false;
                }
            }
            #endregion Controlli preliminari

            #region Obbligatorietà

            if (datiPensione == null || !datiPensione.DecorrenzaOriginaria.HasValue)
            {
                messaggioVideo = "Controlli Incrociati - Dati Pensione:<br/>Dati obbligatori.";
                return false;
            }

            if (areaTitolare == null || areaTitolare.Anagrafica == null)
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>Dati obbligatori.";
                return false;
            }

            if (datiFondo == null)
            {
                if (!Utility.isDomandaGiornalistiDipendentiConSistemaPrivato(datiPensione))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Dati Fondo Generici obbligatori.";
                    return false;
                }
                else
                {
                    //senza inizializzazione scoppia più avanti
                    datiFondo = new GestioneFondo.DatiFondo();
                }
            }

            if (listaRecordFondo == null || listaRecordFondo.Count == 0)
            {
                messaggioVideo = "Controlli Incrociati - Dati Assicurativi:<br/>Record Fondo dati obbligatori.";
                return false;
            }

            if (tipoFondo.HasValue)
            {
                if (!datiPensione.SiglaCategoria.StartsWith("I") && !datiPensione.SiglaCategoria.StartsWith("S") &&
               (!datiPensione.DataPerfezionamentoRequisiti.HasValue || !Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 01, 01))) &&
                objectFondoXX == null)
                {
                    messaggioVideo = "Controlli Incrociati - Dati Generici:<br/>Dati Fondo " + tipoFondo.Value.ToString() + " obbligatori.";
                    return false;
                }

                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.CL:
                        if (!GestioneControlli.VerificaDatiAssicurativiObbligatori(listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0 ? listaDatiServizioUtile.First().ServizioUtileAA : null,
                            listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0 ? listaDatiServizioUtile.First().ServizioUtileMM : null,
                            datiFondoCL.DataPerfezionamentoRequisiti, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                            return false;
                        }
                        break;
                    case Utility.TipoFondo.ET:
                        if (!datiFondoET.Stipendio.HasValue)
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Stipendio obbligatorio.";
                            return false;
                        }

                        if (!datiFondoET.Importo13ma.HasValue)
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Tredicesima obbligatoria.";
                            return false;
                        }
                        break;
                }
            }

            if (!GestioneControlli.ControlsObbligatorietaDetrazioni(datiPensione, datiAnagraficaTitolare, Lfamiliare, listaDetrazioniContitolare, datiDanteCausa, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Detrazioni:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlReversibilitaConCodiceSpecificoP(datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Obbligatorietà

            #region Titolare

            if (!GestioneCrossControls.ALL_VerificaBloccoDecorrenzaPensione(datiPensione, isRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaWithDataMorteTitolare(datiPensione.DecorrenzaOriginaria, datiAnagraficaTitolare.DataMorte,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                return false;
            }

            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
            {
                //Pensioni ai superstiti o sue ricostituzioni
                if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaPerIndirette(datiPensione.DecorrenzaOriginaria, datiAnagraficaTitolare.CodiceFiscale,
                    datiAnagraficaTitolare.DataNascita, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, Lfamiliare, datiPensione, datiDanteCausa, listaCodMaggFamiliari, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Titolare:<br />" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaResidenzaEsteroTitolare(datiAnagraficaTitolare.ResidenzaEstero, datiAnagraficaTitolare.CodiceComuneResidenza, datiAnagraficaTitolare.FrazioneResidenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaProvinciaTitolare(datiAnagraficaTitolare.ProvinciaResidenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (areaTitolare.Anagrafica.CodiceFiscale == string.Empty || areaTitolare.Anagrafica.CodiceFiscale.Trim().Length != 16)
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>Codice Fiscale Titolare non corretto.";
                return false;
            }
            DateTime? dataValiditaInferiore = null;
            bool? isDecorrenzaValida = Utility.ControllaDataDecorrenzaInferiore(datiPensione, Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione), areaTitolare.Pensione.DecorrenzaOriginaria, out dataValiditaInferiore);
            if (!isDecorrenzaValida.HasValue || !isDecorrenzaValida.Value)
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>Decorrenza Pensione antecedente il " +
                    (dataValiditaInferiore.HasValue ? dataValiditaInferiore.Value.Month.ToString() + "/" + dataValiditaInferiore.Value.Year.ToString() : "limite minimo");
                return false;
            }
            DateTime? dataValiditaSuperiore = null;
            isDecorrenzaValida = Utility.ControllaDataDecorrenzaSuperiore(areaTitolare.Pensione.DecorrenzaOriginaria, tipoAppartenenza, out dataValiditaSuperiore);
            if (!isDecorrenzaValida.HasValue || !isDecorrenzaValida.Value)
            {
                bool eseguiControllo = true;
                if ((Utility.IsDomandaINPDAP(datiPensione.Gestione) || tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && dataValiditaSuperiore.HasValue)
                {
                    dataValiditaSuperiore = new DateTime(dataValiditaSuperiore.Value.Year, 12, 31);
                    if (DateTime.Compare(areaTitolare.Pensione.DecorrenzaOriginaria.Value.Date,
                    dataValiditaSuperiore.Value.Date) <= 0)
                    {
                        eseguiControllo = false;
                    }
                }
                if (eseguiControllo)
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>Decorrenza Pensione successiva al " +
                    (dataValiditaSuperiore.HasValue ? dataValiditaSuperiore.Value.Day.ToString() + "/" +
                    dataValiditaSuperiore.Value.Month.ToString() + "/" + dataValiditaSuperiore.Value.Year.ToString() : "limite massimo");
                    return false;
                }
            }

            if (!datiPensione.SiglaCategoria.StartsWith("I"))
            {
                if (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value.CompareTo(new DateTime(2010, 12, 31)) > 0 &&
                    datiPensione.DataPerfezionamentoRequisiti.HasValue && datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2010, 12, 31)) > 0)
                {
                    if (!GestioneCrossControls.ALL_VerificaPerfezRequisitiDecPensioneAnzianitaVecchiaia(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti,
                        derogaTraduzioneSuGP, isRiaperturaDomanda, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
                {
                    if (datiPensione.Prodotto.Trim() == "0002" && datiPensione.DataPerfezionamentoRequisiti.HasValue && datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2012, 01, 01)) >= 0)
                    {
                        //mail 29-11-2013: bypass controlli per Salvaguardie FW: Reeng Pensioni FS - Segnalazione Produzione  per salvaguardia 135
                        if (!Utility.IsDomandaSalvaguardia214(datiPensione) && !Utility.IsDomandaSalvaguardia135(datiPensione) && !Utility.IsDomandaSalvaguardia228(datiPensione) &&
                            !Utility.IsDomandaSalvaguardia124(datiPensione) && !Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) && !Utility.IsDomandaSalvaguardia147(datiPensione) &&
                            !Utility.IsDomandaUsuranti(datiPensione) && !Utility.IsDomandaEsuberiPA(datiPensione) && !Utility.IsDomandaSalvaguardia147_2014(datiPensione) &&
                            !Utility.IsDomandaSalvaguardia208_2015(datiPensione) && !Utility.IsDomandaAPEPrecoci(datiPensione))
                        {
                            if (!GestioneCrossControls.FS_VerificaEtaTitolareDataPerfRequisitiPostFeb2012(tipoFondo, datiPensione.DataPerfezionamentoRequisiti, areaTitolare.Anagrafica.CodiceFiscale,
                                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                                return false;
                            }
                        }
                    }
                }
            }

            if (tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PI && tipoFondo.Value != Utility.TipoFondo.PL)
            {
                if (!GestioneCrossControls.ALL_VerificaSperimentaleDonnaTitolare(datiPensione, areaTitolare, derogaTraduzioneSuGP, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                if (!GestioneControlli.VerificaDecPensioneInvioPreCalcolo(datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
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

            if (!GestioneCrossControls.ALL_VerificaStatiCivili(datiPensione.DecorrenzaOriginaria, datiPensione, areaTitolare.ElencoStatiCivili, LfamiliariCompleta, LAnagraficheFamiliari, listaCodMaggFamiliari,
                datiAnagraficaTitolare.DataNascita, datiAnagraficiDC != null ? datiAnagraficiDC.DataMatrimonio : null, datiAnagraficaTitolare.Sesso, datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : null,
                datiAnagraficaTitolare.CodiceFiscale, dataSistema, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaOriginariaSperimentaleDonna(datiPensione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsPerfezionamentoRequisitiSperimentaleDonna(datiPensione, datiAnagraficaTitolare, datiPensione.DataPerfezionamentoRequisiti, tipoAppartenenza, out messaggioVideo))
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
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiPensione.NumeroFigli, datiAnagraficaTitolare, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.ET:
                        if (Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione))
                        {
                            if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione.DecorrenzaOriginaria,
                                datiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                                return false;
                            }
                            if (!GestioneCrossControls.FS_VerificaCompatibilitaPerfezionamentoPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione.DataPerfezionamentoRequisiti,
                                datiAnagraficaTitolare.DataNascita, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                                return false;
                            }
                            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Titolare_Anagrafica_FS.NATI_29FEBBRAIO) &&
                                !GestioneCrossControls.FS_VerificaEtaTitolarePersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione.DataPerfezionamentoRequisiti,
                                datiAnagraficaTitolare.DataNascita, datiAnagraficaTitolare.Sesso, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                                return false;
                            }
                        }

                        if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione))
                        {
                            if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione.DecorrenzaOriginaria,
                                datiPensione.DataPerfezionamentoRequisiti, datiPensione, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                                return false;
                            }

                            if (!GestioneCrossControls.FS_VerificaEtaTitolarePersonaleViaggianteConPerditaTitoloAbilitante(datiPensione.DataPerfezionamentoRequisiti,
                                datiAnagraficaTitolare.DataNascita, datiAnagraficaTitolare.Sesso, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                                return false;
                            }
                        }

                        if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
                        {
                            if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione) || Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione))
                            {
                                if (!datiFondoET.PersonaleViaggiante.HasValue)
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Personale Viaggiante obbligatorio.";
                                    return false;
                                }

                                if (datiFondoET.PersonaleViaggiante.HasValue)
                                {
                                    if (datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2014, 1, 1)))
                                    {
                                        List<Entity.PersonaleViaggiante> listaPersonaleViaggiante = null;
                                        GestioneLiquidazionePensione.GetListaPersonaleViaggiante(ref contenitoreDecodifica, out listaPersonaleViaggiante);
                                        Entity.PersonaleViaggiante personaleViaggiante = listaPersonaleViaggiante.Find(x => x.Id == datiFondoET.PersonaleViaggiante.Value);

                                        if (Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione))
                                        {
                                            if (personaleViaggiante.TraduzioneSuGP != 1)
                                            {
                                                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Personale Viaggiante incongruente con prodotto WebDom.";
                                                return false;
                                            }
                                        }
                                        else if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione))
                                        {
                                            if (personaleViaggiante.TraduzioneSuGP != 2)
                                            {
                                                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Personale Viaggiante incongruente con prodotto WebDom.";
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Utility.TipoFondo.VL:
                        if (Utility.IsDomandaVecchPerditaTitolo(datiPensione))
                        {
                            if (!GestioneCrossControls.FS_ControlsDecorrenzaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione.DecorrenzaOriginaria,
                                datiPensione.DataPerfezionamentoRequisiti, datiPensione, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                                return false;
                            }
                        }
                        break;
                    case Utility.TipoFondo.GAS:
                        if (!GestioneCrossControls.FS_GAS_ControlliPerfezionamentoRequisiti(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                            return false;
                        }
                        break;
                }
            }

            if (!GestioneCrossControls.ALL_ControlsRequisitoEta(datiPensione, tipoAppartenenza, isRiaperturaDomanda, datiPensione, datiAnagraficaTitolare.DataNascita, datiAnagraficaTitolare.Sesso,
                datiIstruttoria != null ? datiIstruttoria.Legge44997 : null, datiIstruttoria != null ? datiIstruttoria.CodiceParticolareSoggettoDerogato : null, derogaTraduzioneSuGP, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, null, null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            bool isWarning = false;
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaPerTipoContributivo(datiPensione, datiPensione, datiAnagraficaTitolare.DataNascita,
                datiAnagraficaTitolare.Sesso, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, out isWarning, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePerfRequisitiSperimentaleDonna(datiPensione, tipoAppartenenza, datiPensione.DecorrenzaOriginaria,
                datiPensione.DataPerfezionamentoRequisiti, datiAnagraficaTitolare.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorPensione(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.FS_VerificaFinestraMobileVecchiaiaVEL_VET_VTT(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP, derogaTraduzioneSuGP, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCiviliSuperstiti(datiPensione, Lfamiliare, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneAPEPrecoce(datiPensione, datiPensione.DecorrenzaOriginaria, datiAnagraficaTitolare, out messaggioVideo))
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
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione))))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota100(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficaTitolare.DataNascita, out messaggioVideo))
            {
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione))))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensionePrecoci(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaQuota102(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficaTitolare.DataNascita, out messaggioVideo))
            {
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione))))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaPensioneQuota102(datiPensione, datiPensione.DecorrenzaOriginaria, datiPensione.DataPerfezionamentoRequisiti, datiPensione.LavoratorePubblico, out messaggioVideo))
            {
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione))))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibile(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficaTitolare.DataNascita, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            //ENG - Memo 123/2024
            if (!GestioneCrossControls.ALL_VerificaRequisitoEtaAnticipataFlessibileLeggeDiBilancio2024(datiPensione, datiPensione.DataPerfezionamentoRequisiti, datiAnagraficaTitolare.DataNascita, out messaggioVideo))
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

            #endregion Titolare

            #region Stati Civili
            //controlli unioni civili

            //// Controllo temporaneo per bloccare l'inserimento delle unioni civili
            //if (areaTitolare.ElencoStatiCivili.Exists(x => x.Codice == '7' || x.Codice == '8' || x.Codice == 'C'))
            //{
            //    messaggioVideo = "Non è possibile inserire gli stati civili UNITO/A CIVILMENTE, SCIOLTO/A DALL'UNIONE e VEDOVO/A DA UNIONE CIVILE.";
            //    return false;
            //}

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaUnioniCivili(areaTitolare.ElencoStatiCivili, datiPensione, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }

            #endregion Stati Civili

            #region Residenze Estere
            if (Utility.IsRicostituzione(datiPensione.Gruppo) &&
                !GestioneCrossControls.ALL_VerificaResidenzeEstereWithAnagrafica(areaTitolare.Anagrafica, areaTitolare.ElencoResidenzeEstere, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Residenze Estere

            #region DanteCausa
            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Superstiti)
            {
                if (!GestioneCrossControls.AGO_FS_ControlsDataMatrimonioWithGradoParentelaAndDataMorte(datiPensione, datiAnagraficiDC != null ? datiAnagraficiDC.DataMatrimonio : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, Lfamiliare, LAnagraficheFamiliari,
                    datiAnagraficaTitolare, Utility.TipoAppartenenza.FS, isRiaperturaDomanda, datiDanteCausa, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDataMatrimonioDC(datiPensione, isRiaperturaDomanda, datiAnagraficiDC != null ? datiAnagraficiDC.DataMatrimonio : null, Lfamiliare, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Dante Causa:<br/>" + messaggioVideo;
                return false;
            }

            #endregion DanteCausa

            #region LiquidazionePensione

            //bypass PRIMO_VERSAMENTO
            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.PRIMO_VERSAMENTO))
            {
                if (!GestioneControlli.VerificaPrimoVersamento(datiPensione, datiFondo, datiPensione.InizioAssicurazione, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : datiAnagraficaTitolare.DataNascita,
                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PI && tipoFondo.Value != Utility.TipoFondo.PT && tipoFondo.Value != Utility.TipoFondo.PL)
            {
                if (!GestioneControlli.VerificaPensioneInvaliditaWithoutBonus(datiPensione, datiFondo.AttribuzioneBonus))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Le pensioni di invalidità non possono avere bonus.";
                    return false;
                }
            }

            if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) &&
                !GestioneControlli.VerificaListDecRecordFondoDecPensione(Utility.IsDomandaReversibilita(datiPensione) ? datiDanteCausa.DecorrenzaPensione : datiPensione.DecorrenzaOriginaria, listaRecordFondo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>La data decorrenza del primo Record Fondo è diversa dalla Decorrenza della Pensione.";
                return false;
            }

            if (!GestioneControlli.VerificaListDecorDataSospDecorValDatiRecordFondo(listaRecordFondo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>La data di sospensione del record fondo presente in dati assicurativi deve essere successiva alla decorrenza del record stesso.";
                return false;
            }

            if (!GestioneControlli.VerificaDataSospRecordFondoDecPensione(listaRecordFondo, datiPensione, datiDanteCausa))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>La data di sospensione dei record fondo presente in dati assicurativi deve essere successiva alla decorrenza della pensione.";
                return false;
            }

            if (!GestioneControlli.VerificaDataSospUltimoRecordFondo(listaRecordFondo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>L'ultimo record fondo presente in dati assicurativi non può avere una data di sospensione.";
                return false;
            }

            if (objectFondoXX != null)
            {
                if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, objectFondoXX, datiPensione, datiFondo.CodiceRequisiti1,
                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP, derogaTraduzioneSuGP, datiAnagraficaTitolare.DataNascita,
                    datiAnagraficaTitolare.Sesso, true, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }
            }


            // qui il controllo objectFondoXX != null non serve perchè viene verificato internamente
            if (tipoFondo.HasValue)
            {
                if (!GestioneControlli.VerificaEtaTitolareVecchiaia(areaTitolare.Anagrafica, datiPensione, tipoFondo, objectFondoXX, datiFondo.CodiceRequisiti1,
                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PI && tipoFondo.Value != Utility.TipoFondo.FS && tipoFondo.Value != Utility.TipoFondo.PT && tipoFondo.Value != Utility.TipoFondo.CL &&
                tipoFondo.Value != Utility.TipoFondo.ES && tipoFondo.Value != Utility.TipoFondo.PL)
            {
                if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcolo(datiPensione.DecorrenzaOriginaria, datiPensione.FineAssicurazione, Utility.GetTipoCalcolo(datiPensione), datiPensione.Gruppo, datiPensione.Prodotto, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }

                #region Gestione Presenza DatiCalcolo per Competenza 2013 (legge 214)

                if (!GestioneControlli.VerificaDataUltimoVersamentoWithDatiCalcolo(datiPensione, datiCalcolo, tipoCalcolo, tipoFondo.Value, codiceSpecificoTraduzioneSuGP))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Data Ultimo Versamento incompatibile con i Dati Calcolo";
                    return false;
                }
                #endregion Gestione Presenza DatiCalcolo per Competenza 2013 (legge 214)
            }

            if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.EL || tipoFondo.Value == Utility.TipoFondo.TT || tipoFondo.Value == Utility.TipoFondo.ET || tipoFondo.Value == Utility.TipoFondo.VL))
            {
                if (!GestioneControlli.ControlsNaturaPensioneWithTrasformazioneAOI(datiPensione, datiPensione.NaturaPensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }
            }

            #region Fondo FS
            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS)
            {
                foreach (GestioneFondo.DatiFondoFST datiFondoFS in listaDatiFondoFS)
                {
                    int? anni = null;
                    DateTime? data = null;
                    if (!GestioneControlli.VerificaEtaTitolareWithQualificaProfessionale(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo, datiPensione.DataPerfezionamentoRequisiti,
                        datiFondo.AttivitaSvolta, datiFondoFS != null ? datiFondoFS.RequisitiAnte247.HasValue : false, datiAnagraficaTitolare.DataNascita, datiPensione.DecorrenzaOriginaria,
                        datiFondoFS != null ? datiFondoFS.TrimesteRequisiti : null, datiFondoFS != null ? datiFondoFS.AnnoRequisiti : null, out anni, out data))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Titolare:<br/>Il Titolare non ha compiuto " + anni + " anni alla data Perfezionamento Requisiti (" + String.Format("{0:dd/MM/yyyy}", data) + ")";
                        return false;
                    }

                    if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                    {
                        if (datiFondoFS != null && !datiFondoFS.DecorrenzaEconomica.HasValue)
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Campo 'Decorrenza Economica' obbligatorio";
                            return false;
                        }

                        if (datiFondoFS != null && datiFondoFS.DecorrenzaEconomica.Value != datiPensione.DecorrenzaOriginaria.Value)
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>La 'Decorrenza Economica' deve essere uguale alla 'Decorrenza Giuridica'";
                            return false;
                        }
                    }
                    if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.DECOR_REGISTR_CALCOLO_FSPT))
                    {
                        //ENG - saltare controllo nel caso di prime liquidate di reversibilità (quindi gruppo= 0003 e prodotto= 0021)
                        //ENG - saltare controllo anche per le RIC di reversibilità
                        if (!Utility.IsRicostituzioneOrRiaperturaFSPTPerequata(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria) &&
                            datiFondoFS != null && !datiFondo.InizioBonus.HasValue && datiFondoFS.DecorrenzaCalcolo.HasValue && datiFondoFS.DecorrenzaCalcolo.Value != datiPensione.DecorrenzaOriginaria.Value
                            //&& !(datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021")
                            && !Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) && !(Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.SiglaCategoria.StartsWith("S")))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>La Decorrenza Calcolo deve essere uguale alla Decorrenza Pensione";
                            return false;
                        }
                    }

                    if (datiFondoFS != null && datiFondo.InizioBonus.HasValue && datiFondoFS.DecorrenzaCalcolo.HasValue && datiFondoFS.DecorrenzaCalcolo.Value != datiFondo.InizioBonus.Value)
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>La Decorrenza Calcolo deve essere uguale alla Data Inizio Bonus.";
                        return false;
                    }
                }
            }
            #endregion Fondo FS

            if (!GestioneCrossControls.ALL_VerificaFineAssicurazioneForReversibilita(tipoDomanda, datiPensione.FineAssicurazione, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, tipoAppartenenza, datiPensione.SiglaCategoria, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                int i = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    if (!GestioneControlli.VerificaCodiceNonCalcoloRecordFondo(datiPensione, tipoFondo, recordFondo.CodiceNonCalcolo, categoriaFondoPI, i == listaRecordFondo.Count - 1, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                    i++;
                }
            }

            if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.FS || tipoFondo.Value == Utility.TipoFondo.PT))
            {
                if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                {
                    if (!GestioneControlli.ControlsNaturaForPrivilegiateFS_PT(datiFondo != null ? datiFondo.Privilegiate : false, datiPensione.NaturaPensione, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }

                    if ((datiAssicurativiEntity.fondoPT != null && !datiAssicurativiEntity.fondoPT.TrediciMensilita.HasValue) ||
                        (datiAssicurativiEntity.fondoFST != null && !datiAssicurativiEntity.fondoFST.TrediciMensilita.HasValue))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Tredicesima Mensilità: campo obbligatorio";
                        return false;
                    }
                }
                else
                {
                    if (tipoFondo.Value == Utility.TipoFondo.FS)
                    {
                        foreach (GestioneFondo.DatiFondoFST datiFondoFST in listaDatiFondoFS)
                        {
                            Entity.DatiPrivilegiate datiPrivilegiate = new Entity.DatiPrivilegiate();
                            Utility.ValorizzaOggetti(datiFondoFST, datiPrivilegiate);
                            if (!datiPrivilegiate.IsDatiPrivilegiateNull())
                            {
                                if (string.IsNullOrEmpty(datiPensione.NaturaPensione) || (!datiPensione.NaturaPensione.Substring(0, 1).Equals("1") && !datiPensione.NaturaPensione.Substring(0, 1).Equals("2")))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Per le pensioni privilegiate il primo codice natura deve essere uguale a 1 o 2";
                                    return false;
                                }
                            }

                            if (!datiFondoFST.TrediciMensilita.HasValue)
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Tredicesima Mensilità: campo obbligatorio";
                                return false;
                            }
                        }
                    }

                    if (tipoFondo.Value == Utility.TipoFondo.PT)
                    {
                        foreach (GestioneFondo.DatiFondoPT datiFondoPT in listaDatiFondoPT)
                        {
                            Entity.DatiPrivilegiate datiPrivilegiate = new Entity.DatiPrivilegiate();
                            Utility.ValorizzaOggetti(datiFondoPT, datiPrivilegiate);
                            if (!datiPrivilegiate.IsDatiPrivilegiateNull())
                            {
                                if (string.IsNullOrEmpty(datiPensione.NaturaPensione) || (!datiPensione.NaturaPensione.Substring(0, 1).Equals("1") && !datiPensione.NaturaPensione.Substring(0, 1).Equals("2")))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Per le pensioni privilegiate il primo codice natura deve essere uguale a 1 o 2";
                                    return false;
                                }
                            }

                            if (!datiFondoPT.TrediciMensilita.HasValue)
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Tredicesima Mensilità: campo obbligatorio";
                                return false;
                            }
                        }
                    }
                }
            }

            if (!GestioneCrossControls.ALL_VerificaIncongruenzaEsenzioneFiscaleToDB(datiPensione, datiAnagraficaTitolare != null ? datiAnagraficaTitolare.CodiceComuneResidenza : string.Empty, datiDetrazioni, isRiaperturaDomanda, datiIstruttoria != null ? datiIstruttoria.CodiceComunicazioneCampo4 : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlsProvvisoriaPerRiapertura(ref contenitoreDecodifica, isRiaperturaDomanda, datiIstruttoria != null ? datiIstruttoria.CodiceComunicazioneCampo3 : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.GAS:
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                        if (!GestioneControlli.VerificaCodiceRequisitiOrSperimentaleDonna(datiAssicurativiEntity.CodiceRequisiti2, datiPensione, tipoFondo, isRiaperturaDomanda,
                            datiDanteCausa, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                            return false;
                        }
                        break;
                }

                if (tipoFondo.Value == Utility.TipoFondo.TT)
                {
                    if (!GestioneControlli.VerificaRetribuzioneMensileINAILPerTT(datiAssicurativiEntity != null && datiAssicurativiEntity.fondoTT != null ?
                        datiAssicurativiEntity.fondoTT.RetribuzioneMensileInail : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (tipoFondo.Value == Utility.TipoFondo.CL)
                {
                    if (!GestioneControlli.VerificaCodiceRequisiti2CL(datiAssicurativiEntity != null ? datiAssicurativiEntity.CodiceRequisiti2 : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                    if (!GestioneControlli.ControlsCapienzaServizioUtile_CL(listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0 ? listaDatiServizioUtile.First().ServizioUtileAA : null,
                        listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0 ? listaDatiServizioUtile.First().ServizioUtileMM : null, datiPensione.InizioAssicurazione,
                        datiPensione.FineAssicurazione, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                    if (datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "VCL")
                        if (!GestioneControlli.ControlsServizioUtileAAMM_CL(datiAssicurativiEntity.fondoCL.ServizioUtileAA, datiAssicurativiEntity.fondoCL.ServizioUtileMM, datiAssicurativiEntity.fondoCL.CodicePensioneSenzaRequisiti, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                            return false;
                        }
                }

                if (tipoFondo.Value != Utility.TipoFondo.FS && tipoFondo.Value != Utility.TipoFondo.PT && tipoFondo.Value != Utility.TipoFondo.PI && tipoFondo.Value != Utility.TipoFondo.PL)
                {
                    if (!GestioneControlli.VerificaCodiceRequisiti1(datiAssicurativiEntity != null ? datiAssicurativiEntity.CodiceRequisiti1 : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                #region Fondo ET
                if (tipoFondo == Utility.TipoFondo.ET)
                {
                    if (!GestioneControlli.VerificaImporto13maImporto14maPerET(datiAssicurativiEntity, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }

                    if (!GestioneControlli.VerificaDecorenzaTeoricaContributivoPerET(datiPensione, datiAssicurativiEntity, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }

                    if (datiAssicurativiEntity != null && datiAssicurativiEntity.fondoET != null
                        && !GestioneControlli.ET_ObbligatorietaElementiAccessori(datiAssicurativiEntity.fondoET.ElementiAccessori, out messaggioVideo))
                    {
                        messaggioVideo = string.Concat("Controlli Incrociati - Dati Liquidazione Pensione:<br/>", messaggioVideo);
                        return false;
                    }

                    if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
                    {
                        if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione))
                        {
                            if (string.IsNullOrEmpty(datiPensione.NaturaPensione) || datiPensione.NaturaPensione.Substring(1, 1) != "W")
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Il secondo codice natura deve essere W.";
                                return false;
                            }
                        }
                        else if (Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione))
                        {
                            if (string.IsNullOrEmpty(datiPensione.NaturaPensione) || datiPensione.NaturaPensione.Substring(1, 1) != "K")
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Il secondo codice natura deve essere K.";
                                return false;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && (datiPensione.NaturaPensione.Substring(1, 1) == "K" || datiPensione.NaturaPensione.Substring(1, 1) == "W"))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Il secondo codice natura non può essere K o W.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && (datiPensione.NaturaPensione.Substring(1, 1) == "K" || datiPensione.NaturaPensione.Substring(1, 1) == "W"))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Il secondo codice natura non può essere K o W.";
                            return false;
                        }
                    }

                    if (!GestioneControlli.ControlsServizioMilitareFondoET(datiPensione, datiDanteCausa, datiFondoET != null ? datiFondoET.CodiceServizioMilitare : null,
                        datiFondoET != null ? datiFondoET.NSettimaneLeva : null, datiFondoET != null ? datiFondoET.NSettimaneRichiamato : null,
                        datiFondoET != null ? datiFondoET.ContributiAgoLegge40245 : null, datiFondoET != null ? datiFondoET.ContributiAgoLegge140830 : null, tipoFondo, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                }
                #endregion Fondo ET

                #region Fondo VL
                if (tipoFondo == Utility.TipoFondo.VL)
                {
                    if (!GestioneControlli.VerificaAttivitaSvolta_VL(datiPensione, tipoFondo, attivitaSvoltaTraduzioneSuGP, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                    if (!GestioneControlli.ControlsCodiceCapitalizzazione(datiPensione, isRiaperturaDomanda, datiAssicurativiEntity.fondoVL.CodiceCapitalizzazione,
                            datiAssicurativiEntity.fondoVL.ImportoPercentualeCapitalizzazione, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                        return false;
                    }
                }
                #endregion Fondo VL
            }

            if (datiEliminazione != null)
            {
                if (!GestioneCrossControls.ALL_VerificaDecorrenzaEliminazioneWithRedditi(lstRedditi, datiEliminazione.DataEvento, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }
                if (!GestioneCrossControls.ALL_VerificaCodiceArretratiWithEliminazione(datiEliminazione.CodiceMotivo, datiPensione.CodiceArretrati, datiPensione, out messaggioVideo))
                {
                    messaggioVideo = "Controlli incrociati - Dati Generici:<br/>" + messaggioVideo;
                    return false;
                }
            }
            if (!GestioneCrossControls.ALL_ControlsInizioAssicurazioneSperimentaleDonna(datiPensione, datiPensione.InizioAssicurazione, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }
            if (!GestioneControlli.ControlsCodNaturaSperDonna(datiPensione, datiPensione.NaturaPensione, datiPensione.TipoCalcolo, datiAnagraficaTitolare.Sesso,
                datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaTipoCalcoloConRecordFondo_PIU(datiPensione, listaRecordFondoEntity, categoriaFondoPI, datiPensione.TipoCalcolo, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaExCombattentePerPIU(listaRecordFondo.Exists(x => x.CodiceNonCalcolo == 'N'), datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.ExCombattente : null,
                        datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA : null, categoriaFondoPI, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlsCodiceSpecificoAnteArmonizzazione(datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaSettimane707PresentiMaNonVisibili(datiPensione, codiceSpecificoTraduzioneSuGP, datiCalcolo.IsComma707Null(),
                datiCalcolo != null ? !datiCalcolo.IsContribL214Null() : false, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile,
                listaRecordDatiFondoINPDAP, tipoFondo, objectFondoXX, datiAnagraficaTitolare, dataSistema, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlsPrimoVersamentoPerAPEPrecoci(datiPensione, datiPensione.InizioAssicurazione, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiRIC(datiPensione.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, datiPensione.CausaCarico, datiPensione.DataInizioCalcolo, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }
            }
            else
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiPL(datiPensione.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(datiPensione, datiPensione.NaturaPensione, datiPensione.InizioAssicurazione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaBeneficiPerOpzioneTipoContributivo(datiPensione, datiPensione.Benefici, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaFineAssicurazioneINPDAP(datiPensione, datiFondo, datiPensione != null ? datiPensione.FineAssicurazione : null, out messaggioVideo))
                return false;

            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Generici_FS.CONFERMA_ESENZIONE_VITTIME) &&
                datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value && Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
            {
                messaggioVideo = "Ricostituzione da validare a cura del responsabile. Inserire il bypass CONFERMA_ESENZIONE_VITTIME tramite l’apposita sezione in procedura Nuova IVS.";
                return false;
            }

            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione)) // Memo 79
            {
                if (!GestioneCrossControls.ALL_CodNaturOrganizzazioniInternazionali(datiPensione, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                    {
                        long idRecordFondo = recordFondo.Id;
                        GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoINPDAP = listaRecordDatiFondoINPDAP.Find(x => x.IdRecordFondo == idRecordFondo);

                        short Anni, Mesi, Giorni, AnniOI, MesiOI, GiorniOI;
                        Anni = datiRecordFondoINPDAP.ServizioUtileDirittoAA.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoAA.Value : (short)0;
                        Mesi = datiRecordFondoINPDAP.ServizioUtileDirittoMM.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoMM.Value : (short)0;
                        Giorni = datiRecordFondoINPDAP.ServizioUtileDirittoGG.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoGG.Value : (short)0;
                        AnniOI = datiRecordFondoINPDAP.ServizioUtileDirittoOIAA.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoOIAA.Value : (short)0;
                        MesiOI = datiRecordFondoINPDAP.ServizioUtileDirittoOIMM.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoOIMM.Value : (short)0;
                        GiorniOI = datiRecordFondoINPDAP.ServizioUtileDirittoOIGG.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoOIGG.Value : (short)0;

                        if (!GestioneCrossControls.ALL_NSettimane_OrganizzazioniInternazionali_INPDAP(datiPensione, datiAnagraficaTitolare, Anni, Mesi,Giorni, AnniOI, MesiOI, GiorniOI, out messaggioVideo))
                            return false;
                    }

                }
                else
                {
                    if (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS)
                    {
                        if (!GestioneCrossControls.ALL_NSettimane_OrganizzazioniInternazionali_FS_PT(datiPensione, datiAnagraficaTitolare, tipoFondo, objectFondoXX, out messaggioVideo))
                            return false;
                    }
                    else
                    {
                        if (!GestioneCrossControls.ALL_NSettimane_OrganizzazioniInternazionali(datiPensione, datiAnagraficaTitolare, datiFondo, out messaggioVideo))
                            return false;
                    }
                }
                
            }


            #endregion LiquidazionePensione

            #region Familiari

            if (!ControlsDatiFamiliari(datiPensione, dataSistema, isRiaperturaDomanda, tipoAppartenenza, Lfamiliare, LAnagraficheFamiliari, listaCodMaggFamiliari, areaTitolare, datiAnagraficaTitolare, datiEliminazione, datiDanteCausa, matricolaOperatore, out messaggioVideo))
            {
                if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    GestioneFamiliari.SbloccaFamiliari(datiPensione, Lfamiliare);
                return false;
            }

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ConsultazioneANFAttivaFS", out ctrl);
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (!isConsultazioniANFVerificate)
                {
                    if (!ControlsConsultazioneANF(datiPensione, Lfamiliare, listaCodMaggFamiliari, dataSistema, matricolaOperatore, out listaConsultazioniANF, out messaggioVideo))
                        return false;
                    if (listaConsultazioniANF != null && listaConsultazioniANF.Count > 0)
                        return true;
                }
            }

            #endregion Familiari

            #region Dati Fondo
            if (isDomandaConNuovaGestioneDatiFondoFSPT && tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.FS || tipoFondo.Value == Utility.TipoFondo.PT))
            {
                DateTime? appDecorrenzaValiditaDati = null;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    if (appDecorrenzaValiditaDati.HasValue)
                    {
                        if (Utility.DataSuccessivaA(appDecorrenzaValiditaDati.Value, recordFondo.DecorrenzaValiditaDati.Value))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Fondo:<br/>Le decorrenze registrazione devono essere sequenziali.";
                            return false;
                        }
                    }
                    appDecorrenzaValiditaDati = recordFondo.DecorrenzaValiditaDati;

                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            GestioneFondo.DatiFondoPT datiFondoPT = null;
                            GestioneFondo.GetFondoPTByIdRecordFondo(recordFondo.Id, out datiFondoPT);
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
                                    if (!GestioneControlli.ControlPALBeneficiPAL(datiFondoPT.PALConBenefici, pensioneAnnuaLorda, out messaggioVideo))
                                    {
                                        messaggioVideo = "Controlli Incrociati - Dati Fondo:<br/>" + messaggioVideo;
                                        return false;
                                    }
                                }

                                if (!GestioneControlli.ControlScadenzaBeneficiWithDecorrenzaFondo(datiFondoPT.ScadenzaBenefici, recordFondo.DecorrenzaValiditaDati, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Fondo:<br/>" + messaggioVideo;
                                    return false;
                                }
                            }
                            break;
                        case Utility.TipoFondo.FS:
                            GestioneFondo.DatiFondoFST datiFondoFS = null;
                            GestioneFondo.GetFondoFSTByIdRecordFondo(recordFondo.Id, out datiFondoFS);
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
                                    if (!GestioneControlli.ControlPALBeneficiPAL(datiFondoFS.PALConBenefici, pensioneAnnuaLorda, out messaggioVideo))
                                    {
                                        messaggioVideo = "Controlli Incrociati - Dati Fondo:<br/>" + messaggioVideo;
                                        return false;
                                    }
                                }
                                if (!GestioneControlli.ControlScadenzaBeneficiWithDecorrenzaFondo(datiFondoFS.ScadenzaBenefici, recordFondo.DecorrenzaValiditaDati, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Fondo:<br/>" + messaggioVideo;
                                    return false;
                                }
                            }
                            break;
                    }
                }
            }
            #endregion Dati Fondo

            #region Periodi
            //domande gdp
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodi;

                List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto;
                List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheAventiDiritto;
                GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(datiPensione.Id, out listaAventiDiritto, out listaAnagraficheAventiDiritto);
                long? idAventeDirittoTitolare = listaAventiDiritto.Where(x => x.IdAnagrafica == datiAnagraficaTitolare.Id).Select(x => x.Id).FirstOrDefault();

                if (idAventeDirittoTitolare != null)
                {
                    GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(datiPensione.Id, idAventeDirittoTitolare, out listaPeriodi);

                    if (listaPeriodi != null && !GestioneCrossControls.OrdinamentoPeriodiAventiDiritto(listaPeriodi, out messaggioVideo))
                        return false;
                }

            }
            #endregion

            if ((tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PI && tipoFondo.Value != Utility.TipoFondo.CL && tipoFondo.Value != Utility.TipoFondo.ES && tipoFondo.Value != Utility.TipoFondo.PL) || Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                #region Dati Calcolo

                if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                {
                    if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
                    {
                        foreach (var datiContributivi in listaDatiContributivi)
                            if (!GestioneControlli.VerificaDatiContributiviTipoCalcolo(tipoCalcolo, datiContributivi, datiRetributivi, listaDatiServizioUtile, contenitore.ListaDatiRetributivi))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>Tipo Calcolo non congruo con i dati calcolo inseriti";
                                return false;
                            }
                    }
                    else
                    {
                        if (!GestioneControlli.VerificaDatiContributiviTipoCalcolo(tipoCalcolo, null, datiRetributivi, listaDatiServizioUtile, contenitore.ListaDatiRetributivi))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>Tipo Calcolo non congruo con i dati calcolo inseriti";
                            return false;
                        }
                    }
                }

                //TODO da eliminare nel momento del cambiamento del tracciato di calcolo per EL ed ET
                if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.EL || tipoFondo.Value == Utility.TipoFondo.ET))
                {
                    if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
                    {
                        foreach (var datiContributivi in listaDatiContributivi)
                            if (!GestioneControlli.VerificaDimensioneImportoContributivo(datiContributivi, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                return false;
                            }
                    }
                    else
                    {
                        if (!GestioneControlli.VerificaDimensioneImportoContributivo(null, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                            return false;
                        }
                    }
                }

                if (!GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(datiPensione, datiFondo.RiduzioneRetributiva, tipoCalcolo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo / Dati Titolare:<br/>La riduzione retributiva è incompatibile con la data perfezionamento requisiti.";
                    return false;
                }
                if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.FS || tipoFondo.Value == Utility.TipoFondo.PT))
                {
                    if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                    {
                        if (!GestioneControlli.ControlsDatiCalcoloFS_PT(datiCalcolo, tipoFondo, datiPensione, tipoCalcolo, codiceSpecificoTraduzioneSuGP,
                            datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null,
                            datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                            return false;
                        }
                    }
                    else
                    {
                        if (tipoFondo.Value == Utility.TipoFondo.FS)
                        {
                            foreach (GestioneFondo.DatiFondoFST datiFondoFST in listaDatiFondoFS)
                            {
                                decimal? pensioneAnnuaLorda = null;
                                if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                    !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                    pensioneAnnuaLorda = datiFondoFST.PensioneAnnuaLorda214;
                                else
                                    pensioneAnnuaLorda = datiFondoFST.PensioneAnnuaLorda;

                                List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileApp = listaDatiServizioUtile.FindAll(x => x.IdRecordFondo == datiFondoFST.IdRecordFondo);
                                if (!GestioneControlli.ControlsDatiCalcoloFS_PTRecordFondo(datiPensione, isRiaperturaDomanda, pensioneAnnuaLorda, datiFondoFST.ServizioUtileDirittoAA,
                                    datiFondoFST.ServizioUtileDirittoMM, datiFondoFST.ServizioUtileDirittoGG, datiFondoFST.ServizioUtileDirittoOIAA, datiFondoFST.ServizioUtileDirittoOIMM, datiFondoFST.ServizioUtileDirittoOIGG,
                                    lServizioUtileApp, tipoFondo, tipoCalcolo, codiceSpecificoTraduzioneSuGP,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null, datiCalcolo.ImportoContributivoTotale, datiCalcolo.Montante, datiCalcolo.MontanteContributivo, datiCalcolo.NSettimane, datiCalcolo.MontanteQuotaDL214, datiCalcolo.ImportoContribTotaleQuotaDL214, datiCalcolo.NSettimaneQuotaDL214, datiCalcolo.QuotaContributivaAnnua, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtile(lServizioUtileApp, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtileWithFineAssicurazione(lServizioUtileApp, datiPensione.FineAssicurazione, tipoFondo, datiPensione, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsAnniServizioUtiliDiritto(datiPensione, tipoFondo, datiMaggiorazioniBenefici, datiFondoFST.ServizioUtileDirittoAA,
                                    datiFondoFST.ServizioUtileDirittoMM, datiFondoFST.ServizioUtileDirittoGG, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }
                            }
                        }
                        if (tipoFondo.Value == Utility.TipoFondo.PT)
                        {
                            foreach (GestioneFondo.DatiFondoPT datiFondoPT in listaDatiFondoPT)
                            {
                                decimal? pensioneAnnuaLorda = null;
                                if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                    !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                    pensioneAnnuaLorda = datiFondoPT.PensioneAnnuaLorda214;
                                else
                                    pensioneAnnuaLorda = datiFondoPT.PensioneAnnuaLorda;

                                List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileApp = listaDatiServizioUtile.FindAll(x => x.IdRecordFondo == datiFondoPT.IdRecordFondo);
                                if (!GestioneControlli.ControlsDatiCalcoloFS_PTRecordFondo(datiPensione, isRiaperturaDomanda, pensioneAnnuaLorda, datiFondoPT.ServizioUtileDirittoAA, datiFondoPT.ServizioUtileDirittoMM,
                                    datiFondoPT.ServizioUtileDirittoGG, datiFondoPT.ServizioUtileDirittoOIAA, datiFondoPT.ServizioUtileDirittoOIMM, datiFondoPT.ServizioUtileDirittoOIGG, lServizioUtileApp, tipoFondo, tipoCalcolo, codiceSpecificoTraduzioneSuGP,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null, datiCalcolo.ImportoContributivoTotale, datiCalcolo.Montante, datiCalcolo.MontanteContributivo, datiCalcolo.NSettimane, datiCalcolo.MontanteQuotaDL214, datiCalcolo.ImportoContribTotaleQuotaDL214, datiCalcolo.NSettimaneQuotaDL214, datiCalcolo.QuotaContributivaAnnua, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtile(lServizioUtileApp, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtileWithFineAssicurazione(lServizioUtileApp, datiPensione.FineAssicurazione, tipoFondo, datiPensione, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }
                                if (!GestioneControlli.ControlsAnniServizioUtiliDiritto(datiPensione, tipoFondo, datiMaggiorazioniBenefici, datiFondoPT.ServizioUtileDirittoAA,
                                    datiFondoPT.ServizioUtileDirittoMM, datiFondoPT.ServizioUtileDirittoGG, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }
                            }
                        }
                    }
                }
                if (tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PI && tipoFondo.Value != Utility.TipoFondo.FS && tipoFondo.Value != Utility.TipoFondo.PT && tipoFondo.Value != Utility.TipoFondo.PM && tipoFondo.Value != Utility.TipoFondo.PL)
                {
                    if (!GestioneControlli.ControlsRiduzioneRetributiva(tipoCalcolo, datiFondo, datiPensione, objectFondoXX, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>La riduzione retributiva è non corretta.";
                        return false;
                    }

                    if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiFondo != null ? datiFondo.RiduzioneRetributiva : false, datiFondo != null ? datiFondo.RiduzioneRetributivaPercentuale : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.ET:
                            GestioneDatiServizioUtile.ServizioUtile servizioUtileQuotaA = null;
                            if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                                servizioUtileQuotaA = listaDatiServizioUtile.Find(x => x.Quota == "A");
                            decimal? retribuzionePensionabileQuotaA = null;
                            if (servizioUtileQuotaA != null)
                                retribuzionePensionabileQuotaA = servizioUtileQuotaA.RetribuzionePensionabile;

                            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                            {
                                if (!GestioneControlli.ControlsETDatiCalcoloAnteArmonizzazione(listaDatiServizioUtile, datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null, tipoFondo, true, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtile(listaDatiServizioUtile, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
                                {
                                    foreach (var datiContributivi in listaDatiContributivi)
                                    {
                                        GestioneCalcolo.DatiCalcoloContributivo dati = datiContributivi;
                                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref dati, ref datiRetributivi,
                                            ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                        {
                                            messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
                                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref datiContributivi, ref datiRetributivi,
                                            ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                    {
                                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!GestioneControlli.ControlsDatiCalcoloET(datiCalcolo, tipoFondo, datiPensione, datiMaggiorazioniBenefici, true, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }
                            }

                            if (!(Utility.IsDomandaReversibilita(datiPensione) && !Utility.DataStrettamenteSuccessivaA(decorrenzaPensioneOrDecorrenzaPensioneDC.GetValueOrDefault(), new DateTime(1974, 12, 31))))
                            {
                                if (!GestioneControlli.VerificaRetribuzionePensionabileQuotaA_ET(datiPensione, datiPensione.TipoCalcolo, retribuzionePensionabileQuotaA,
                                    datiFondoET != null ? datiFondoET.Stipendio : null, datiFondoET != null ? datiFondoET.Importo13ma : null, datiFondoET != null ? datiFondoET.Importo14ma : null, datiFondoET != null ? datiFondoET.ElementiAccessori : null, datiFondoET != null ? datiFondoET.Competenze40Percento : null,
                                    out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }
                            }
                            break;
                        case Utility.TipoFondo.GAS:
                            if (!GestioneControlli.VerificaDecorrenzaTeorica(datiCalcolo != null && datiCalcolo.fondoGAS != null ? datiCalcolo.fondoGAS.DecorrenzaTeorica : null,
                                datiFondo != null ? datiFondo.InizioBonus : null, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Ago:<br/>" + messaggioVideo;
                                return false;
                            }
                            break;
                        case Utility.TipoFondo.EL:
                            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondo: datiFondo, datiCalcoloRetributivo: datiRetributivi,
                                datiServizioUtile: listaDatiServizioUtile))
                            {
                                if (!GestioneControlli.ControlsELDatiCalcoloAnteArmonizzazione(listaDatiServizioUtile, datiFondo, datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null, tipoFondo, true, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtile(listaDatiServizioUtile, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
                                {
                                    foreach (var datiContributivi in listaDatiContributivi)
                                    {
                                        GestioneCalcolo.DatiCalcoloContributivo dati = datiContributivi;
                                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref dati, ref datiRetributivi,
                                            ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                        {
                                            messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    GestioneCalcolo.DatiCalcoloContributivo dati = null;
                                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref dati, ref datiRetributivi,
                                            ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                    {
                                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                        return false;
                                    }
                                }
                            }
                            break;
                        case Utility.TipoFondo.VL:
                            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                            {
                                if (!GestioneControlli.ControlsVLDatiCalcoloAnteArmonizzazione(listaDatiServizioUtile, datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null, tipoFondo, true, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtile(listaDatiServizioUtile, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
                                {
                                    foreach (var datiContributivi in listaDatiContributivi)
                                    {
                                        GestioneCalcolo.DatiCalcoloContributivo dati = datiContributivi;
                                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref dati, ref datiRetributivi,
                                            ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                        {
                                            messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    GestioneCalcolo.DatiCalcoloContributivo dati = null;
                                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref dati, ref datiRetributivi,
                                        ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                    {
                                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                        return false;
                                    }
                                }
                            }
                            break;
                        case Utility.TipoFondo.TT:
                            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondoXX: objectFondoXX))
                            {
                                if (!GestioneControlli.ControlsTTDatiCalcoloAnteArmonizzazione(listaDatiServizioUtile, datiPensione, datiDanteCausa, objectFondoXX, null, true, codiceSpecificoTraduzioneSuGP,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null,
                                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null, tipoFondo, out messaggioVideo))
                                {
                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (!GestioneControlli.ControlsDatiServizioUtile(listaDatiServizioUtile, out messaggioVideo))
                                {
                                    if (messaggioVideo.Contains("(Quota A)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota A)", "(Quota Ante 01/01/93)");
                                    else if (messaggioVideo.Contains("(Quota A2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota A2)", "(Quota Ante 01/01/93 Ridotto)");
                                    else if (messaggioVideo.Contains("(Quota B)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota B)", "(Quota Post 31/12/92)");
                                    else if (messaggioVideo.Contains("(Quota B2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota B2)", "(Quota Post 31/12/92 Ridotto)");
                                    else if (messaggioVideo.Contains("(Quota C)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota C)", "(Quota Post 31/12/94)");
                                    else if (messaggioVideo.Contains("(Quota C2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota C2)", "(Quota Post 31/12/94 Ridotto)");
                                    else if (messaggioVideo.Contains("(Quota D)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota D)", "(Quota Post 31/12/96)");
                                    else if (messaggioVideo.Contains("(Quota D2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota D2)", "(Quota Post 31/12/96 Ridotto)");

                                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                    return false;
                                }

                                if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
                                {
                                    foreach (var datiContributivi in listaDatiContributivi)
                                    {
                                        GestioneCalcolo.DatiCalcoloContributivo dati = datiContributivi;
                                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref dati, ref datiRetributivi,
                                            ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                        {
                                            messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    GestioneCalcolo.DatiCalcoloContributivo dati = null;
                                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref dati, ref datiRetributivi,
                                        ref datiDl407, ref listaDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo))
                                    {
                                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                        return false;
                                    }
                                }
                            }
                            break;
                    }
                }

                GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneDoppioCalcolo707", out controlloDinamico);
                if ((tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.VL || tipoFondo.Value == Utility.TipoFondo.TT || tipoFondo.Value == Utility.TipoFondo.EL || tipoFondo.Value == Utility.TipoFondo.ES))
                    || (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI"))
                {
                    if (!GestioneControlli.ControlsDatiComma707(datiPensione, tipoFondo, datiCalcolo, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.NUM_SETT_APEPRECOCI) &&
                    !GestioneControlli.ControlsNSettimanePerAPEPrecoci(datiPensione, datiCalcolo, listaRecordDatiFondoINPDAP, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsNSettimanePerRequisitoAnticipatoArt1(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP,
                    tipoFondo, objectFondoXX, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }
                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.NUM_SETT_QUOTA100) &&
                    !GestioneControlli.ControlsNSettimanePerQuota100(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, tipoFondo, objectFondoXX,
                    datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }

                if (!(Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                {
                    if (!GestioneControlli.ControlsNSettimanePerSperimentaleDonna_DL_4_2019(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, tipoFondo,
                    objectFondoXX, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        return false;
                    }

                    if (!GestioneControlli.ControlsNSettimanePerOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, tipoFondo,
                        objectFondoXX, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (!Utility.IsDomandaCTPS(datiPensione.SiglaCategoria))
                {
                    if (!GestioneControlli.ControlsNSettimanePerAnzianitaPerLeggeBilancio2019(datiPensione, datiFondo, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, datiAnagraficaTitolare.Sesso,
                        listaRecordDatiFondoINPDAP, tipoFondo, objectFondoXX, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                if (!GestioneControlli.ControlsNSettimanePerQuota102(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, tipoFondo, objectFondoXX,
                    datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }


                if (!(Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaAutomatica(datiPensione)) &&
                    !GestioneControlli.ControlsNSettimanePerAnticipateFlessibili(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, tipoFondo, objectFondoXX,
                   datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }

                #endregion Dati Calcolo

                #region Supplementi

                //Controllare che Il supplemento non abbia una data antecedente alla decorrenza della pensione (Supplementi.DecorrenzaSupplemento >= Pensione.DecorrenzaPensione)
                if (tipoFondo.HasValue && !(tipoFondo.Value == Utility.TipoFondo.ET && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)))
                {
                    if (!GestioneCrossControls.FS_VerificaDecorrenzaSupplementoDecorrenzaPensione(listDatiSupplementi, Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null)))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Supplementi:<br/>La data di decorrenza del supplemento non può essere antecedente alla decorrenza della pensione";
                        return false;
                    }

                    if (!GestioneCrossControls.FS_VerificaSupplementiWithBonus(listDatiSupplementi, listaRecordFondo, datiFondo.AttribuzioneBonus, codiceSpecificoTraduzioneSuGP, tipoFondo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Supplementi:<br/>Non è possibile inserire Supplementi in mancanza del Bonus.";
                        return false;
                    }
                }

                if (!GestioneCrossControls.FS_VerificaSupplementiCodiceSpecificoCodiceGestione(datiPensione, listDatiSupplementi, datiFondo.AttribuzioneBonus, codiceSpecificoTraduzioneSuGP, tipoFondo, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Supplementi:<br/>" + messaggioVideo;
                    return false;
                }

                #endregion Supplementi

                #region Deleghe/Tutele
                if (!GestioneCrossControls.ALL_VerificaDelegheTuteleByIdPensione(datiPensione, datiDelegato != null ? datiDelegato.CodiceFiscale : string.Empty,
                    datiTutore != null ? datiTutore.CodiceFiscale : string.Empty,
                    datiTutore != null ? datiTutore.CodiceTutore : (char?)null,
                    datiTutore != null ? datiTutore.CessValAmmSost : (DateTime?)null, datiAnagraficaTitolare.CodiceFiscale, isRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Deleghe / Tutele:<br/>" + messaggioVideo;
                    return false;
                }

                //ENG - Reversibilita 024
                if (Utility.IsDomandaReversibilita(datiPensione) && !isRiaperturaDomanda && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                {
                    if (!GestioneControlli.IsPresenteDelegatoTutoreTitolareMinorenne(datiPensione, datiAnagraficaTitolare, datiTutore, out messaggioVideo))
                    {
                        GestioneDelegatoTutore.ImpostaTabTuteleObbligatorio(datiPensione);
                        messaggioVideo = "Controlli Incrociati - Deleghe / Tutele:<br/>" + messaggioVideo;
                        return false;
                    }
                }

                #endregion Deleghe/Tutele
            }

            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PI)
            {
                if (categoriaFondoPI.HasValue)
                {
                    switch (categoriaFondoPI.Value)
                    {
                        case Utility.CategoriaFondoPI.U:
                            if (!GestioneControlli.ControlsPensComplRiv195PIU(datiCalcolo.fondoPI != null ? datiCalcolo.fondoPI.PensComplRiv1_95 : null, decorrenzaPensioneOrDecorrenzaPensioneDC, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                return false;
                            }
                            break;
                        case Utility.CategoriaFondoPI.V:
                            if (!GestioneControlli.ControlsCapienzaSettimanePIV(datiCalcolo.fondoPI != null ? datiCalcolo.fondoPI.NSettimaneQuotaA : null, datiCalcolo.fondoPI != null ? datiCalcolo.fondoPI.NSettimaneQuotaB : null,
                                datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, out messaggioVideo))
                            {
                                messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                                return false;
                            }
                            break;
                    }
                }

                if (categoriaFondoPI == Utility.CategoriaFondoPI.V || categoriaFondoPI == Utility.CategoriaFondoPI.U)
                {
                    List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstDatiNoCalcolo;
                    GestioneDatiNoCalcolo.GetRecordNoCalcoloByIdPensione(datiPensione.Id, out lstDatiNoCalcolo);

                    List<GestioneComponenteFamiliare.ComponenteFamiliare> lstComponenteFamiliare;
                    GestioneComponenteFamiliare.GetComponenteFamiliareByIdPensione(datiPensione.Id, out lstComponenteFamiliare);

                    foreach (var record in lstDatiNoCalcolo)
                    {
                        List<string> codiceFicaliForRecord = null;
                        if (lstComponenteFamiliare != null && lstComponenteFamiliare.Count > 0)
                            codiceFicaliForRecord = lstComponenteFamiliare.Where(x => x.IdRecordDatiNoCalcolo == record.Id).Select(x => x.CodiceFiscale).ToList();

                        if (!GestioneControlli.ControlsDecNoCalcoloWithRecordFondo(datiPensione, listaRecordFondo, record.Decorrenza, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati No Calcolo:<br/>" + messaggioVideo;
                            return false;
                        }

                        if (!GestioneControlli.ControlsDecPensioneWithDecNoCalcolo(datiPensione, record.Decorrenza, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati No Calcolo:<br/>" + messaggioVideo;
                            return false;
                        }

                        if (!GestioneControlli.ControlsFamiliari(datiPensione, record.Decorrenza, codiceFicaliForRecord, ref Lfamiliare, ref LAnagraficheFamiliari, ref listaCodMaggFamiliari, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati No Calcolo:<br/>" + messaggioVideo;
                            return false;
                        }
                    }
                }
            }

            switch (tipoFondo)
            {
                case Utility.TipoFondo.EL:
                case Utility.TipoFondo.ET:
                case Utility.TipoFondo.TT:
                case Utility.TipoFondo.VL:
                    if (!GestioneContrib.ControlsSettimaneUtiliDirittoFondi(datiCalcolo, datiPensione, tipoFondo, ref datiMaggiorazioniBenefici, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        return false;
                    }
                    break;
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                foreach (GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiFondoInpdap in listaRecordDatiFondoINPDAP)
                {
                    if (!GestioneControlli.ControlsAnniServizioUtiliDiritto(datiPensione, tipoFondo, datiMaggiorazioniBenefici, datiFondoInpdap.ServizioUtileDirittoAA, datiFondoInpdap.ServizioUtileDirittoMM, datiFondoInpdap.ServizioUtileDirittoGG, out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                        return false;
                    }
                }
            }

            //Controllo sperimentale donna
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
            {
                if (!GestioneControlli.ControlsNSettimanePerSperimentaleDonna_DL_4_2019(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, tipoFondo,
                    objectFondoXX, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }

                if (!GestioneControlli.ControlsNSettimanePerOpzioneDonna_Legge197_2022_Art1_Comma292(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, tipoFondo,
                    objectFondoXX, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Dati Calcolo:<br/>" + messaggioVideo;
                    return false;
                }
            }

            #region MaggiorazioniBenefici
            #region Benefici
            if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
            {
                foreach (var datiContributivi in listaDatiContributivi)
                {
                    if (!GestioneControlli.ControlsSettimaneBeneficioNonVedenteWithDatiCalcolo(datiPensione, datiRetributivi, datiContributivi, listaDatiServizioUtile,
                        datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP,
                        out messaggioVideo))
                    {
                        messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                        return false;
                    }
                }
            }
            else
            {
                if (!GestioneControlli.ControlsSettimaneBeneficioNonVedenteWithDatiCalcolo(datiPensione, datiRetributivi, null, listaDatiServizioUtile,
                            datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP,
                            out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneCrossControls.FS_ControlsTipoBeneficioArt24Comma15Bis(datiPensione, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, datiAnagraficaTitolare.Sesso,
                datiAnagraficaTitolare.DataNascita, codiceSpecificoTraduzioneSuGP, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, listaRecordDatiFondoINPDAP, objectFondoXX,
                datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneControlli.ControlsBeneficioPrecoci(datiPensione, objectFondoXX, datiFondo, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                decorrenzaPensioneOrDecorrenzaPensioneDC, tipoFondo, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                return false;
            }

            if (tipoFondo != Utility.TipoFondo.CL &&
                !GestioneCrossControls.ALL_ControlsLavoratoriNonVedenti(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneBeneficio : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.SettAnzContribPost311295 : null, datiPensione, datiDanteCausa,
                out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                return false;
            }

            if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
            {
                GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
                GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

                if (!GestioneControlli.ControlsDecMaggiorazioneSociale(ref contenitore, ref contenitoreDecodifica, datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, datiPensione, datiAnagraficaTitolare, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                    return false;
                }
                if (!GestioneCrossControls.ALL_ControlsDecorrenzaMaggiorazioneWithDataPresentazione(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale, datiPensione, datiAnagraficaTitolare != null ? datiAnagraficaTitolare.DataNascita : null,
                    datiStoricoGP != null ? datiStoricoGP.DecorrenzaMaggiorazioneSociale.HasValue : false, datiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Controlli Incrociati - Maggiorazioni Benefici / Benefici:<br/>" + messaggioVideo;
                    return false;
                }
            }

            #endregion Benefici
            #endregion MaggiorazioniBenefici

            #region Oneri
            if (!GestioneCrossControls.AGO_FS_ControlsOneriSperDonna(datiPensione, isRiaperturaDomanda, listaDatiOneri, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Oneri / Oneri:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaOneri(datiPensione, listaDatiOneri, derogaTraduzioneSuGP, isRiaperturaDomanda, datiAnagraficaTitolare, tipoAppartenenza, out messaggioVideo))
            {
                messaggioVideo = "Controlli incrociati - Dati Maggiorazioni / Benefici - Oneri:<br/>" + messaggioVideo;
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
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>" + messaggioVideo;
                return false;
            }
            #endregion Usuranti

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

        public static void CalcolaDomandaNew(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isReingegnerizzato, bool? isNuovoCalcolo, out string statoPensione,
          out int certificato, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = "";
            certificato = 0;
            messaggioVideo = "";
            Data.FSPL_FSRCNew AreaCalcolo = null;
            Guid guid = Guid.NewGuid();
            ValorizzaAreaCalcoloNew(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, true, isReingegnerizzato, out AreaCalcolo);

            //Nuovo calcolo
            string transactionId = "";
            string jsonStringRequest = string.Empty;
            string erroriNuovo = string.Empty;
            string codiciErrore = string.Empty;
            string jsonStringResponse = string.Empty;
            string eccezioni = string.Empty;

            if (isNuovoCalcolo.GetValueOrDefault())
            {
                DateTimeOffset dataNuovo = new DateTimeOffset();
                dataNuovo = DateTimeOffset.Now;
                DateTimeOffset startTime = DateTimeOffset.UtcNow;
                transactionId = AreaCalcolo.CallMiddleware(datiPensione, out jsonStringRequest, out erroriNuovo, out codiciErrore, out eccezioni, out jsonStringResponse);

                DateTimeOffset endTime = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(transactionId))
                {
                    esito = false;
                    if (!string.IsNullOrEmpty(erroriNuovo))
                        messaggioVideo = erroriNuovo;
                    else if (!string.IsNullOrEmpty(eccezioni))
                        messaggioVideo = "Si è verificato un errore imprevisto, contattare il supporto tecnico";
                    else
                        messaggioVideo = "Non è stato possibile effettuare la richiesta di calcolo";

                    GestioneLogSoap.SalvaLogSoap(messaggioVideo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.IvsInvocation, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);

                    //Log Abaco
                    try
                    {
                        List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrata;
                        GestioneNuovoCalcolo.FlowConf confFiltrata;
                        Utility.IsPerimetroNuovoCalcoloConfDinamica(datiPensione, out lstConfFiltrata, datiPensione.FlagVerify.GetValueOrDefault());
                        var flagVerify = datiPensione.FlagVerify.HasValue ? datiPensione.FlagVerify.Value ? "1" : "0" : "1";
                        confFiltrata = lstConfFiltrata != null ? lstConfFiltrata.Find(x => x.TipoRichiesta == flagVerify && x.SistemiInvocati == "NEW") : null;
                        AreaCalcolo.CallAbaco(datiPensione, transactionId, jsonStringRequest, "", "", startTime, endTime, dataNuovo, esito, erroriNuovo, codiciErrore, confFiltrata, eccezioni, jsonStringResponse);
                    }
                    catch
                    {
                        //ignora
                    }
                }
                else
                    esito = true;
            }
            else
            {
                Utility.MetodoServizio? metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaCalcolo.TransactionName);
                GestioneLogSoap.SalvaLogSoap(AreaCalcolo.AreaInputVariabile, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

                bool doppiaChiamata = false;
                DateTimeOffset dataNuovo = new DateTimeOffset();
                GestioneNuovoCalcolo.FlowConf confFiltrata;
                if (Utility.IsDoppiaChiamataConfDinamica(datiPensione, datiPensione.FlagVerify.GetValueOrDefault(), out confFiltrata))
                {
                    dataNuovo = DateTimeOffset.Now;
                    doppiaChiamata = true;
                    transactionId = AreaCalcolo.CallMiddleware(datiPensione, out jsonStringRequest, out erroriNuovo, out codiciErrore, out eccezioni, out jsonStringResponse);
                }

                DateTimeOffset startTime = DateTimeOffset.UtcNow;
                EseguiCalcoloNew(AreaCalcolo);
                DateTimeOffset endTime = DateTimeOffset.UtcNow;

            if (AreaCalcolo.Response != null && AreaCalcolo.Response.Dati != null && AreaCalcolo.Response.Dati.Stampa != null
                && AreaCalcolo.Response.Dati.Stampa.Anagrafica != null && AreaCalcolo.Response.Dati.Stampa.Anagrafica.FLAG_INDEB != null && AreaCalcolo.Response.Dati.Stampa.Anagrafica.FLAG_INDEB.Trim() != "0")
                datiPensione.FlagIndebito = AreaCalcolo.Response.Dati.Stampa.Anagrafica.FLAG_INDEB;

                if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                    GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaCalcolo.MessaggioDaLoggare, null, null);

                if (AreaCalcolo.HasError)
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Messaggio, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Response, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);

                if (AreaCalcolo.Request != null && AreaCalcolo.Request.AR_TIPO == "ELI")
                {
                    AreaCalcolo = null;
                    ValorizzaAreaCalcoloNew(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, false, isReingegnerizzato, out AreaCalcolo);

                    metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaCalcolo.TransactionName);
                    GestioneLogSoap.SalvaLogSoap(AreaCalcolo.AreaInputVariabile, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

                    EseguiCalcoloNew(AreaCalcolo);
                    if (!string.IsNullOrEmpty(AreaCalcolo.MessaggioDaLoggare))
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaCalcolo.MessaggioDaLoggare, null, null);

                    if (AreaCalcolo.HasError)
                        GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Messaggio, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                    else
                        GestioneLogSoap.SalvaLogSoap(AreaCalcolo.Response, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                }
                ControllaEsitoCalcoloNew(datiPensione.NDomus, AreaCalcolo, datiPensione, out statoPensione, out certificato, out esito, out messaggioVideo);

                //monitoraggio nuovo calcolo
                if (doppiaChiamata)
                {
                    string codErrore = "";
                    string descrError = "";
                    if (!esito)
                    {
                        codErrore = "KO";
                        descrError = messaggioVideo;
                    }
                  
                    AreaCalcolo.CallMainframe(datiPensione, transactionId, jsonStringRequest, codErrore, descrError, startTime, endTime, dataNuovo, esito, confFiltrata);
                    if (!string.IsNullOrEmpty(erroriNuovo))
                    {
                        AreaCalcolo.CallAbaco(datiPensione, transactionId, jsonStringRequest, codErrore, descrError, startTime, endTime, dataNuovo, esito, erroriNuovo, codiciErrore, confFiltrata, eccezioni, jsonStringResponse);
                    }
                }

            }
        }

        #endregion public members

        #region private members
        private static void ValorizzaAreaCalcolo(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isFirst, bool isReingegnerizzato, out Data.FSPL_FSRC AreaCalcolo)
        {
            if (datiPensione == null)
                throw new INPS.DNA.DnaApplicationException("Errore durante il recupero delle informazioni.");

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            string transazione = "FSPL";
            string sottoTipo = isReingegnerizzato ? "U" : "A";
            if (datiPensione.FlagVerify.HasValue && datiPensione.FlagVerify.Value)
                sottoTipo = isReingegnerizzato ? "W" : "V";
            string tipoOperazione = isFirst && (sottoTipo == "A" || sottoTipo == "U") && datiLavorazione != null && !string.IsNullOrEmpty(datiLavorazione.TipoLiquidazione) &&
                (datiLavorazione.TipoLiquidazione.ToUpperInvariant().StartsWith("A2") || datiLavorazione.TipoLiquidazione.ToUpperInvariant().StartsWith("A5")) ? "ELI" : "NEW";

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || Utility.IsRiaperturaDomanda(datiLavorazione != null ? datiLavorazione.CodFase : string.Empty))
            {
                transazione = "FSRC";
                tipoOperazione = "RIC";
            }
            string fase = ""; //TODO da impostare: R se LK-RESTA = SI; N se LK-RESTA = NO. LK-RESTA ???
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(sedeOperatore.ToString().PadLeft(4, '0') + centroOperativoOperatore.ToString().PadLeft(2, '0'));

            int dataCompetenza = 0;
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.FS);
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoFS", out ctrl);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno di competenza
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo 01012004
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                int annoCompetenza = 0;
                int anno = 0;
                GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out anno);
                int annoCompetenzaRinnovo = dataSistema.Year + 1;

                if (datiPensione.IsRicRinnovata.GetValueOrDefault())
                    annoCompetenza = annoCompetenzaRinnovo;
                else
                    annoCompetenza = anno;

                string annoDataCompetenza = annoCompetenza.ToString() + "0101";
                dataCompetenza = int.Parse(annoDataCompetenza);
            }
            else
                dataCompetenza = 01012004;

            AreaCalcolo = new INPS.Pensioni.LiquidazioneFs.Data.FSPL_FSRC(transazione, tipoOperazione, sottoTipo, fase, dataCompetenza);

            AreaCalcolo.AreaInputVariabile = new Data.CMSGTRA.AreaVariabile();
            AreaCalcolo.Request.LISTBLOCCO = new List<Data.HostRequest.FSPL_FSRCRequest.BLOCCO>();
            AreaCalcolo.UtilizzaNuovoTracciato = GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.FS.UTILIZZANUOVOTRACCIATO_FSPT, dataSistema);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            #region Anagrafica
            GestionePagamento.DatiPagamento datiPagamento = null;
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            GestionePensione.DatiPatronato datiPatronato = null;
            List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneFondo.DatiFondo datiFondo = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            Object objectFondoXX = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDelegato = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTutore = null;

            MappingVersoHost.ValorizzaAnagrafica(matricolaOperatore, datiPensione, tipoFondo, datiLavorazione, ref AreaCalcolo, out datiPagamento,
                out listaRecordFondo, out datiPatronato, out listaStatiCivili, out datiIstruttoria, out datiFondo, out datiDanteCausa, out objectFondoXX,
                out datiAnagraficiTitolare, out datiAnagraficiDelegato, out datiAnagraficiTutore);
            #endregion Anagrafica

            #region Delegato
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            if (tipoOperazione != "ELI")
            {
                MappingVersoHost.ValorizzaDelegato(datiPensione, datiPagamento, datiPatronato, tipoFondo, datiLavorazione, datiFondo, out datiMaggiorazioniBenefici, ref AreaCalcolo);
            }
            #endregion Delegato

            #region Familiare
            Dictionary<string, char> componentiFamiliari = null;
            if (tipoOperazione != "ELI")
            {
                MappingVersoHost.ValorizzaFamiliareByIdPensione(datiPensione, out componentiFamiliari, ref AreaCalcolo);
            }
            #endregion Familiare

            #region DanteCausa
            MappingVersoHost.ValorizzaDanteCausaByIdPensione(datiPensione, datiDanteCausa, out datiAnagraficiDanteCausa, ref AreaCalcolo);
            #endregion DanteCausa

            if (tipoOperazione == "ELI")
            {
                if (AreaCalcolo != null && AreaCalcolo.AreaInputVariabile != null &&
                    AreaCalcolo.AreaInputVariabile.ListaAnagrafica != null && AreaCalcolo.AreaInputVariabile.ListaAnagrafica.Count > 0)
                    AreaCalcolo.AreaInputVariabile.ListaAnagrafica[0].TRACEDUT = 0;
                return;
            }

            #region Supplementi
            MappingVersoHost.ValorizzaSupplementi(datiPensione, ref AreaCalcolo);
            #endregion Supplementi

            #region TrattamentiFamiglia
            MappingVersoHost.ValorizzaTrattamentiFamiglia(listaStatiCivili, ref AreaCalcolo);
            #endregion TrattamentiFamiglia

            #region Minimo_PensInv
            Entity.DatiBititolaritaInail datiBititolaritaInail = null;
            MappingVersoHost.ValorizzaMinimo_PensInv(datiPensione.Id, out datiBititolaritaInail, ref AreaCalcolo);
            #endregion Minimo_PensInv

            #region Residenza
            MappingVersoHost.ValorizzaResidenza(datiPensione, datiMaggiorazioniBenefici, tipoFondo, datiIstruttoria, ref AreaCalcolo);
            #endregion Residenza

            #region MaggiorazioneLegge
            GestioneDL407.DatiDL407 datiDL407 = null;
            MappingVersoHost.ValorizzaMaggiorazioneLegge(datiPensione, out datiDL407, ref AreaCalcolo);
            #endregion MaggiorazioneLegge


            #region Deleghe e Tutele
            MappingVersoHost.ValorizzaDelegatoTutore(datiAnagraficiDelegato, datiAnagraficiTutore, ref AreaCalcolo);
            #endregion Deleghe e Tutele

            #region RenditaINAIL
            MappingVersoHost.ValorizzaRenditaINAIL(datiBititolaritaInail, ref AreaCalcolo);
            #endregion RenditaINAIL

            #region TrattenuteLavAutonomi
            MappingVersoHost.ValorizzaTrattenuteLavAutonomi(ref AreaCalcolo);
            #endregion TrattenuteLavAutonomi

            #region AgoTeorico
            MappingVersoHost.ValorizzaAgoTeorico(ref AreaCalcolo);
            #endregion AgoTeorico

            #region MaggiorazioneSociale
            MappingVersoHost.ValorizzaMaggiorazioneSociale(datiMaggiorazioniBenefici, ref AreaCalcolo);
            #endregion MaggiorazioneSociale

            #region Redditi
            MappingVersoHost.ValorizzaRecordR(datiPensione, datiAnagraficiTitolare, ref AreaCalcolo);
            #endregion Redditi

            #region Fondo_Ago
            if (tipoFondo.HasValue)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        MappingVersoHost.ValorizzaFondoEL(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoEL(datiPensione, datiFondo, listaRecordFondo, datiDL407, datiDanteCausa, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.TT:
                        MappingVersoHost.ValorizzaFondoTT(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoTT(datiPensione, datiFondo, listaRecordFondo, datiDanteCausa, objectFondoXX, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.ET:
                        MappingVersoHost.ValorizzaFondoET(datiPensione, objectFondoXX, datiFondo, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoET(datiPensione, datiFondo, objectFondoXX, listaRecordFondo, datiDanteCausa, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.VL:
                        MappingVersoHost.ValorizzaFondoVL(objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiPensione, datiFondo, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoVL(datiPensione, datiFondo, objectFondoXX, listaRecordFondo, datiDanteCausa, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.PT:
                        MappingVersoHost.ValorizzaFondoPT(datiPensione, objectFondoXX, datiFondo, listaRecordFondo, isDomandaConNuovaGestioneDatiFondoFSPT, out listaDatiServizioUtile, ref AreaCalcolo);
                        if (AreaCalcolo.UtilizzaNuovoTracciato)
                            MappingVersoHost.ValorizzaAgoPT(datiPensione, datiFondo, datiIstruttoria, listaRecordFondo, listaDatiServizioUtile, objectFondoXX, isDomandaConNuovaGestioneDatiFondoFSPT, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.FS:
                        MappingVersoHost.ValorizzaFondoFS(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, isDomandaConNuovaGestioneDatiFondoFSPT,
                            out listaDatiServizioUtile, ref AreaCalcolo);
                        if (AreaCalcolo.UtilizzaNuovoTracciato)
                            MappingVersoHost.ValorizzaAgoFS(datiPensione, datiFondo, datiIstruttoria, listaRecordFondo, listaDatiServizioUtile, objectFondoXX, isDomandaConNuovaGestioneDatiFondoFSPT, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        MappingVersoHost.ValorizzaFondoPI(listaRecordFondo, datiMaggiorazioniBenefici, datiPensione, datiFondo, objectFondoXX, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.GAS:
                        MappingVersoHost.ValorizzaFondoGAS(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiFondo, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoGAS(datiPensione, datiFondo, objectFondoXX, listaRecordFondo, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.DZ:
                        MappingVersoHost.ValorizzaFondoDZ(datiPensione, datiFondo, listaRecordFondo, datiMaggiorazioniBenefici, objectFondoXX, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoDZ(datiPensione, datiFondo, listaRecordFondo, objectFondoXX, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.CL:
                        MappingVersoHost.ValorizzaFondoCL(datiPensione, objectFondoXX, datiFondo, listaRecordFondo, datiMaggiorazioniBenefici, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.ES:
                        MappingVersoHost.ValorizzaFondoES(datiPensione, listaRecordFondo, datiMaggiorazioniBenefici, datiFondo, objectFondoXX, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoES(datiPensione, datiFondo, listaRecordFondo, objectFondoXX, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.PM:
                        MappingVersoHost.ValorizzaFondoPM(datiPensione, listaRecordFondo, datiMaggiorazioniBenefici, datiFondo, objectFondoXX, ref AreaCalcolo);
                        MappingVersoHost.ValorizzaAgoPM(datiPensione, datiFondo, listaRecordFondo, ref AreaCalcolo);
                        break;
                    default:
                        break;
                }
            }
            else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP = null;
                MappingVersoHost.ValorizzaFondoINPDAP(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, out listaDatiServizioUtileINPDAP, ref AreaCalcolo);
                MappingVersoHost.ValorizzaAgoINPDAP(datiPensione, datiFondo, datiIstruttoria, listaRecordFondo, listaDatiServizioUtileINPDAP, objectFondoXX, ref AreaCalcolo);
            }
            #endregion Fondo_Ago

            #region DatiNoCalcolo
            MappingVersoHost.ValorizzaDatiNonCalcolo(datiPensione, componentiFamiliari, ref AreaCalcolo);
            #endregion DatiNoCalcolo

            #region Gp4Inpdap
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                MappingVersoHost.ValorizzaGp4Inpdap(datiPensione, datiAnagraficiTitolare, datiDanteCausa, datiAnagraficiDanteCausa, ref AreaCalcolo);
            #endregion

            if (Utility.IsDomandaSpacchettamento024(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI")
                MappingVersoHost.ValorizzaGp4Ipost(datiPensione, datiAnagraficiTitolare, datiDanteCausa, datiAnagraficiDanteCausa, ref AreaCalcolo);
        }

        private static void EseguiCalcolo(Data.FSPL_FSRC AreaCalcolo)
        {
            AreaCalcolo.Invoke();
        }

        private static void ControllaEsitoCalcolo(long numeroDomanda, Data.FSPL_FSRC AreaCalcolo, GestionePensione.DatiPensione datiPensioneOld, out string statoPensione,
            out int certificato, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = null;
            certificato = 0;
            messaggioVideo = AreaCalcolo.Messaggio;

            //// Questa Get viene eseguita per evitare di avere dati sporchi modificati dalla valorizzazione area calcolo
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            //Questa assegnazione serve per leggere e salvare correttamente il flagIndebito che altrimenti verrebbe perso con la Get di sopra
            datiPensione.FlagIndebito = datiPensioneOld.FlagIndebito;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoCalcolo.DatiEsitoCalcolo datiEsitoCalcolo = new GestioneEsitoCalcolo.DatiEsitoCalcolo();
                datiEsitoCalcolo.DettaglioEsito = messaggioVideo;

                if (AreaCalcolo.Response != null && AreaCalcolo.Response.Dati != null)
                {
                    switch (AreaCalcolo.Response.Dati.RZ_ESITO)
                    {
                        case 0:
                        case 1:
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                            {
                                //CALCOLATA (calcolo definitivo)
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
                                datiPensione.NCertificatoProvvisorio = null;
                                if (AreaCalcolo.Response.Dati.Stampa != null && AreaCalcolo.Response.Dati.Stampa.Intestazione != null)
                                {

                                    if (!Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                        datiPensione.NCertificato = AreaCalcolo.Response.Dati.Stampa.Intestazione.CERT;
                                    certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
                                    if (AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC != 0)
                                    {
                                        string dataCalcolo = AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC.ToString().PadLeft(8, '0');
                                        datiPensione.DataElaborazione = Utility.DataFromString(dataCalcolo, Utility.FormatoData.AAAAmmGG);
                                    }
                                }
                            }
                            else
                            {
                                //CALCOLO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcoloVerify;
                                if (AreaCalcolo.Response.Dati.Stampa != null && AreaCalcolo.Response.Dati.Stampa.Intestazione != null)
                                {
                                    if (!Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                        datiPensione.NCertificatoProvvisorio = AreaCalcolo.Response.Dati.Stampa.Intestazione.CERT;
                                    if (AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC != 0)
                                    {
                                        string dataCalcolo = AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC.ToString().PadLeft(8, '0');
                                        datiPensione.DataElaborazione = Utility.DataFromString(dataCalcolo, Utility.FormatoData.AAAAmmGG);
                                    }
                                }
                            }
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = true;
                            break;
                        case 2:
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //SCARTO DA CALCOLO
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoDaCalcolo;
                            else
                                //SCARTO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoVerify;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = false;
                            break;
                        default:
                            esito = false;
                            break;
                    }
                }
                if (esito)
                {
                    datiEsitoCalcolo.Esito = "OK";
                    if (datiPensione.StatoPensione == (int)Utility.StatoPensione.CalcolataNoWebDom)
                        GestioneLogGenerico.EliminaLogGenerico(numeroDomanda);
                }
                else
                    datiEsitoCalcolo.Esito = "KO";

                GestioneEsitoCalcolo.SalvaEsitoCalcolo(datiPensione.Id, datiEsitoCalcolo);
                transactionScope.Complete();
            }

            GestioneDecodifica.GetStatoPensioneById(datiPensione.StatoPensione.Value, out statoPensione);
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

        private static bool ControlsDatiFamiliari(GestionePensione.DatiPensione datiPensione, DateTime dataSistema, bool isRiaperturaDomanda, Utility.TipoAppartenenza? tipoAppartenenza,
            List<GestioneFamiliari.Familiare> Lfamiliare, List<GestioneAnagrafica.DatiAnagrafici> LAnagraficheFamiliari, List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari,
            Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare, GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare, GestionePensione.DatiEliminazione datiEliminazione,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, string matricolaOperatore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((Lfamiliare != null && Lfamiliare.Count > 0) && (LAnagraficheFamiliari == null || LAnagraficheFamiliari.Count == 0))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>Dati anagrafici obbligatori";
                return false;
            }

            if (!GestioneControlli.VerificaAnagraficaFamiliari(LAnagraficheFamiliari, Lfamiliare))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari: Codice Fiscale Familiare non corretto.";
                return false;
            }

            //La prima decorrenza del codice maggiorazione (CodMaggiorazioneFamiliari.Decorrenza) deve essere >= alla decorrenza della pensione (Pensione.DecorrenzaOriginaria)
            if (Lfamiliare != null && Lfamiliare.Count > 0)
            {
                if (listaCodMaggFamiliari == null || listaCodMaggFamiliari.Count == 0)
                {
                    messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>Dati Codici Maggiorazione obbligatori";
                    return false;
                }
            }
            long idAnagrafica = 0;
            if (!GestioneControlli.VerificaDecorrenzaListCodMaggDecorrenzaPensione(Lfamiliare, listaCodMaggFamiliari, datiPensione.DecorrenzaOriginaria, out idAnagrafica))
            {
                GestioneAnagrafica.DatiAnagrafici datiAnagApp = new GestioneAnagrafica.DatiAnagrafici();
                datiAnagApp = LAnagraficheFamiliari.Find(x => x.Id == idAnagrafica);
                string nome = datiAnagApp != null ? datiAnagApp.Nome : string.Empty;
                string cognome = datiAnagApp != null ? datiAnagApp.Cognome : string.Empty;
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>Decorrenza maggiorazione errata per il familiare " + nome + " " + cognome;
                return false;
            }

            //Verificare nella tabella Familiare, che per la stessa domanda da calcolare, non ci siano C.F. uguali
            if (!GestioneCrossControls.ALL_VerificaFamiliariDuplicati(Lfamiliare, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            //Per ciascun familiare presente verificare che il codice fiscale del Titolare  sia diverso dal codice fiscale del familiare 
            if (!GestioneCrossControls.ALL_VerificaFamiliariTitolare(Lfamiliare, areaTitolare, datiPensione, tipoAppartenenza, isRiaperturaDomanda, datiDanteCausa))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>Il titolare pensione non può essere presente nell'elenco dei familiari.";
                return false;
            }

            //Non siano presenti più di due record con SiglaFamiliare = G
            if (!GestioneCrossControls.ALL_VerificaFamiliariGenitori(Lfamiliare, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            //Se esistono due coniugi (Familiare.SiglaFamiliare = C) devono avere decorrenze diverse (CodMaggiorazioneFamiliari.Decorrenza)
            if (!GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggFamiliariConiugi(Lfamiliare, listaCodMaggFamiliari, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            //Se Anagrafica.CodiceStatoCivile = 2 del titolare, allora  deve esistere nella tabella Familiare il coniuge (SiglaFamiliare = C) 

            //Nota: il prefisso "Controlli Incrociati" è già presente all'interno del metodo
            if (!GestioneCrossControls.ALL_VerificaFamiliariConiugiTitolareConiugato(datiPensione, areaTitolare, Lfamiliare, true, datiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_ControlsFamiliariWithStatiCivili(Lfamiliare, listaCodMaggFamiliari, areaTitolare.ElencoStatiCivili, tipoAppartenenza, datiAnagraficaTitolare.DataMorte, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaFamiliariMorti(Lfamiliare, listaCodMaggFamiliari, datiPensione.DecorrenzaOriginaria, tipoAppartenenza, out messaggioVideo, datiPensione, datiEliminazione))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaCessazioneCodMagg(Lfamiliare, listaCodMaggFamiliari, dataSistema, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaSovrapposizioneCodMaggFamiliariConiugi(Lfamiliare, listaCodMaggFamiliari, out messaggioVideo))
                return false;

            if (Lfamiliare != null && Lfamiliare.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare fam in Lfamiliare)
                {
                    List<GestioneFamiliari.CodMaggFamiliari> LcodMaggFam = listaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica);
                    if (LcodMaggFam != null && LcodMaggFam.Count > 0 &&
                        LcodMaggFam.Exists(x => x.Decorrenza.HasValue && x.Cessazione.HasValue && !Utility.DataStrettamenteSuccessivaA(x.Cessazione.Value, x.Decorrenza.Value)))
                    {
                        messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>Per il familiare " + fam.CodiceFiscale + " la data fine carico non può essere inferiore alla data decorrenza carico";
                        return false;
                    }

                    if (fam.SiglaFamiliare == 'N' || fam.SiglaFamiliare == 'J')
                    {
                        if (!GestioneCrossControls.ALL_VerificaMaggiorazioneFamiliariNeJ(datiPensione, tipoAppartenenza, fam, LcodMaggFam, out messaggioVideo))
                        {
                            messaggioVideo = "Controlli Incrociati - Dati Familiari:" + messaggioVideo;
                            return false;
                        }
                    }
                }
            }

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaCarico(Lfamiliare, listaCodMaggFamiliari, datiPensione, datiDanteCausa, out messaggioVideo))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari:<br/>" + messaggioVideo;
                return false;
            }

            if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) &&
                !GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggFamiliariNipoti(Lfamiliare, listaCodMaggFamiliari, dataSistema))
            {
                messaggioVideo = "Controlli Incrociati - Dati Familiari: Non è possibile inserire per i nipoti una data fine carico successiva a Gennaio " + (dataSistema.Year + 1).ToString();
                return false;
            }

            if (!GestioneCrossControls.VerificaScadenzaContitolareNipoteNeJ(datiPensione, Lfamiliare, tipoAppartenenza, listaCodMaggFamiliari, out messaggioVideo))
                return false;

            if (Lfamiliare != null && Lfamiliare.Count > 0 && listaCodMaggFamiliari != null && listaCodMaggFamiliari.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare fam in Lfamiliare)
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
            if (!GestioneCrossControls.ALL_VerificaPlurimeRegistrazioniConiugeUnitoCivile(datiPensione, tipoAppartenenza, Lfamiliare, listaCodMaggFamiliari, out messaggioVideo))
            {
                return false;
            }

            return true;
        }

        private static void ValorizzaAreaCalcoloNew(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isFirst, bool isReingegnerizzato, out Data.FSPL_FSRCNew AreaCalcolo)
        {
            if (datiPensione == null)
                throw new INPS.DNA.DnaApplicationException("Errore durante il recupero delle informazioni.");

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            string transazione = "FSPL";
            string sottoTipo = isReingegnerizzato ? "U" : "A";
            if (datiPensione.FlagVerify.HasValue && datiPensione.FlagVerify.Value)
                sottoTipo = isReingegnerizzato ? "W" : "V";
            string tipoOperazione = isFirst && (sottoTipo == "A" || sottoTipo == "U") && datiLavorazione != null && !string.IsNullOrEmpty(datiLavorazione.TipoLiquidazione) &&
                (datiLavorazione.TipoLiquidazione.ToUpperInvariant().StartsWith("A2") || datiLavorazione.TipoLiquidazione.ToUpperInvariant().StartsWith("A5")) ? "ELI" : "NEW";

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || Utility.IsRiaperturaDomanda(datiLavorazione != null ? datiLavorazione.CodFase : string.Empty))
            {
                transazione = "FSRC";
                tipoOperazione = "RIC";
            }
            string fase = ""; //TODO da impostare: R se LK-RESTA = SI; N se LK-RESTA = NO. LK-RESTA ???
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(sedeOperatore.ToString().PadLeft(4, '0') + centroOperativoOperatore.ToString().PadLeft(2, '0'));

            int dataCompetenza = 0;
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.FS);
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoFS", out ctrl);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno di competenza
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo 01012004
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                int annoCompetenza = 0;
                int anno = 0;
                GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out anno);
                int annoCompetenzaRinnovo = dataSistema.Year + 1;

                if (datiPensione.IsRicRinnovata.GetValueOrDefault())
                    annoCompetenza = annoCompetenzaRinnovo;
                else
                    annoCompetenza = anno;

                string annoDataCompetenza = annoCompetenza.ToString() + "0101";
                dataCompetenza = int.Parse(annoDataCompetenza);
            }
            else
                dataCompetenza = 01012004;

            AreaCalcolo = new INPS.Pensioni.LiquidazioneFs.Data.FSPL_FSRCNew(transazione, tipoOperazione, sottoTipo, fase, dataCompetenza);

            AreaCalcolo.AreaInputVariabile = new Data.CMSGTRA.AreaVariabile();
            AreaCalcolo.Request.LISTBLOCCO = new List<Data.HostRequest.FSPL_FSRCRequest.BLOCCO>();
            AreaCalcolo.UtilizzaNuovoTracciato = GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.FS.UTILIZZANUOVOTRACCIATO_FSPT, dataSistema);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            #region Anagrafica
            GestionePagamento.DatiPagamento datiPagamento = null;
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            GestionePensione.DatiPatronato datiPatronato = null;
            List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneFondo.DatiFondo datiFondo = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            Object objectFondoXX = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDelegato = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTutore = null;
            List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> datiQuoteMiglioramentiContrattuali = null;

            MappingVersoHostNew.ValorizzaAnagrafica(matricolaOperatore, datiPensione, tipoFondo, datiLavorazione, ref AreaCalcolo, out datiPagamento,
                out listaRecordFondo, out datiPatronato, out listaStatiCivili, out datiIstruttoria, out datiFondo, out datiDanteCausa, out objectFondoXX,
                out datiAnagraficiTitolare, out datiAnagraficiDelegato, out datiAnagraficiTutore, out datiQuoteMiglioramentiContrattuali);
            #endregion Anagrafica

            #region Delegato
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            if (tipoOperazione != "ELI")
            {
                MappingVersoHostNew.ValorizzaDelegato(datiPensione, datiPagamento, datiPatronato, tipoFondo, datiLavorazione, datiFondo, out datiMaggiorazioniBenefici, ref AreaCalcolo);
            }
            #endregion Delegato

            #region Familiare
            Dictionary<string, char> componentiFamiliari = null;
            if (tipoOperazione != "ELI")
            {
                MappingVersoHostNew.ValorizzaFamiliareByIdPensione(datiPensione, out componentiFamiliari, ref AreaCalcolo);
            }
            #endregion Familiare

            #region DanteCausa
            MappingVersoHostNew.ValorizzaDanteCausaByIdPensione(datiPensione, datiDanteCausa, out datiAnagraficiDanteCausa, ref AreaCalcolo);
            #endregion DanteCausa

            if (tipoOperazione == "ELI")
            {
                if (AreaCalcolo != null && AreaCalcolo.AreaInputVariabile != null &&
                    AreaCalcolo.AreaInputVariabile.ListaAnagrafica != null && AreaCalcolo.AreaInputVariabile.ListaAnagrafica.Count > 0)
                    AreaCalcolo.AreaInputVariabile.ListaAnagrafica[0].TRACEDUT = 0;
                return;
            }

            #region Supplementi
            MappingVersoHostNew.ValorizzaSupplementi(datiPensione, ref AreaCalcolo);
            #endregion Supplementi

            #region TrattamentiFamiglia
            MappingVersoHostNew.ValorizzaTrattamentiFamiglia(listaStatiCivili, ref AreaCalcolo);
            #endregion TrattamentiFamiglia

            #region Minimo_PensInv
            Entity.DatiBititolaritaInail datiBititolaritaInail = null;
            MappingVersoHostNew.ValorizzaMinimo_PensInv(datiPensione.Id, out datiBititolaritaInail, ref AreaCalcolo);
            #endregion Minimo_PensInv

            #region Residenza
            MappingVersoHostNew.ValorizzaResidenza(datiPensione, datiMaggiorazioniBenefici, tipoFondo, datiIstruttoria, ref AreaCalcolo);
            #endregion Residenza

            #region MaggiorazioneLegge
            GestioneDL407.DatiDL407 datiDL407 = null;
            MappingVersoHostNew.ValorizzaMaggiorazioneLegge(datiPensione, out datiDL407, ref AreaCalcolo);
            #endregion MaggiorazioneLegge


            #region Deleghe e Tutele
            MappingVersoHostNew.ValorizzaDelegatoTutore(datiAnagraficiDelegato, datiAnagraficiTutore, ref AreaCalcolo);
            #endregion Deleghe e Tutele

            #region RenditaINAIL
            MappingVersoHostNew.ValorizzaRenditaINAIL(datiBititolaritaInail, ref AreaCalcolo);
            #endregion RenditaINAIL

            #region TrattenuteLavAutonomi
            MappingVersoHostNew.ValorizzaTrattenuteLavAutonomi(ref AreaCalcolo);
            #endregion TrattenuteLavAutonomi

            #region AgoTeorico
            MappingVersoHostNew.ValorizzaAgoTeorico(ref AreaCalcolo);
            #endregion AgoTeorico

            #region MaggiorazioneSociale
            MappingVersoHostNew.ValorizzaMaggiorazioneSociale(datiMaggiorazioniBenefici, ref AreaCalcolo);
            #endregion MaggiorazioneSociale

            #region Redditi

            MappingVersoHostNew.ValorizzaRecordR(datiPensione, datiAnagraficiTitolare, tipoFondo, datiFondo, objectFondoXX, listaRecordFondo, ref AreaCalcolo);
            #endregion Redditi

            #region Fondo_Ago
            if (tipoFondo.HasValue)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        MappingVersoHostNew.ValorizzaFondoEL(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoEL(datiPensione, datiFondo, listaRecordFondo, datiDL407, datiDanteCausa, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.TT:
                        MappingVersoHostNew.ValorizzaFondoTT(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoTT(datiPensione, datiFondo, listaRecordFondo, datiDanteCausa, objectFondoXX, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.ET:
                        MappingVersoHostNew.ValorizzaFondoET(datiPensione, objectFondoXX, datiFondo, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoET(datiPensione, datiFondo, objectFondoXX, listaRecordFondo, datiDanteCausa, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.VL:
                        MappingVersoHostNew.ValorizzaFondoVL(objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiPensione, datiFondo, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoVL(datiPensione, datiFondo, objectFondoXX, listaRecordFondo, datiDanteCausa, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.PT:
                        MappingVersoHostNew.ValorizzaFondoPT(datiPensione, objectFondoXX, datiFondo, listaRecordFondo, isDomandaConNuovaGestioneDatiFondoFSPT, datiDanteCausa, datiLavorazione, out listaDatiServizioUtile, ref AreaCalcolo);
                        if (AreaCalcolo.UtilizzaNuovoTracciato)
                            MappingVersoHostNew.ValorizzaAgoPT(datiPensione, datiFondo, datiIstruttoria, listaRecordFondo, listaDatiServizioUtile, objectFondoXX, isDomandaConNuovaGestioneDatiFondoFSPT, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.FS:
                        MappingVersoHostNew.ValorizzaFondoFS(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, isDomandaConNuovaGestioneDatiFondoFSPT, datiDanteCausa, datiLavorazione,
                            out listaDatiServizioUtile, ref AreaCalcolo);
                        if (AreaCalcolo.UtilizzaNuovoTracciato)
                            MappingVersoHostNew.ValorizzaAgoFS(datiPensione, datiFondo, datiIstruttoria, listaRecordFondo, listaDatiServizioUtile, objectFondoXX, isDomandaConNuovaGestioneDatiFondoFSPT, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        MappingVersoHostNew.ValorizzaFondoPI(listaRecordFondo, datiMaggiorazioniBenefici, datiPensione, datiFondo, objectFondoXX, datiDanteCausa, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoPI(datiPensione, datiFondo, listaRecordFondo, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoTeoricoPI(datiPensione, datiFondo, listaRecordFondo, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.GAS:
                        MappingVersoHostNew.ValorizzaFondoGAS(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiFondo, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoGAS(datiPensione, datiFondo, objectFondoXX, listaRecordFondo, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.DZ:
                        MappingVersoHostNew.ValorizzaFondoDZ(datiPensione, datiFondo, listaRecordFondo, datiMaggiorazioniBenefici, objectFondoXX, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoDZ(datiPensione, datiFondo, listaRecordFondo, objectFondoXX, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.CL:
                        MappingVersoHostNew.ValorizzaFondoCL(datiPensione, objectFondoXX, datiFondo, listaRecordFondo, datiMaggiorazioniBenefici, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.ES:
                        MappingVersoHostNew.ValorizzaFondoES(datiPensione, listaRecordFondo, datiMaggiorazioniBenefici, datiFondo, objectFondoXX, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoES(datiPensione, datiFondo, listaRecordFondo, objectFondoXX, ref AreaCalcolo);
                        break;
                    case Utility.TipoFondo.PM:
                        MappingVersoHostNew.ValorizzaFondoPM(datiPensione, listaRecordFondo, datiMaggiorazioniBenefici, datiFondo, objectFondoXX, ref AreaCalcolo);
                        MappingVersoHostNew.ValorizzaAgoPM(datiPensione, datiFondo, listaRecordFondo, ref AreaCalcolo);
                        break;
                    default:
                        break;
                }
            }
            else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                MappingVersoHostNew.ValorizzaMiglioramentiContrattuali(datiQuoteMiglioramentiContrattuali, ref AreaCalcolo);
                List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP = null;
                MappingVersoHostNew.ValorizzaFondoINPDAP(datiPensione, objectFondoXX, listaRecordFondo, datiMaggiorazioniBenefici, datiIstruttoria, datiFondo, out listaDatiServizioUtileINPDAP, ref AreaCalcolo);
                MappingVersoHostNew.ValorizzaAgoINPDAP(datiPensione, datiFondo, datiIstruttoria, listaRecordFondo, listaDatiServizioUtileINPDAP, objectFondoXX, ref AreaCalcolo);
            }
            #endregion Fondo_Ago

            #region DatiNoCalcolo
            MappingVersoHostNew.ValorizzaDatiNonCalcolo(datiPensione, componentiFamiliari, ref AreaCalcolo);
            #endregion DatiNoCalcolo

            #region Gp4Inpdap
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                MappingVersoHostNew.ValorizzaGp4Inpdap(datiPensione, datiAnagraficiTitolare, datiDanteCausa, datiAnagraficiDanteCausa, ref AreaCalcolo);
            #endregion

            if (Utility.IsDomandaSpacchettamento024(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI")
                MappingVersoHostNew.ValorizzaGp4Ipost(datiPensione, datiAnagraficiTitolare, datiDanteCausa, datiAnagraficiDanteCausa, ref AreaCalcolo);
        }

        private static void EseguiCalcoloNew(Data.FSPL_FSRCNew AreaCalcolo)
        {
            AreaCalcolo.Invoke();
        }

        private static void ControllaEsitoCalcoloNew(long numeroDomanda, Data.FSPL_FSRCNew AreaCalcolo, GestionePensione.DatiPensione datiPensioneOld, out string statoPensione,
            out int certificato, out bool esito, out string messaggioVideo)
        {
            esito = false;
            statoPensione = null;
            certificato = 0;
            messaggioVideo = AreaCalcolo.Messaggio;

            //// Questa Get viene eseguita per evitare di avere dati sporchi modificati dalla valorizzazione area calcolo
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            //Questa assegnazione serve per leggere e salvare correttamente il flagIndebito che altrimenti verrebbe perso con la Get di sopra
            datiPensione.FlagIndebito = datiPensioneOld.FlagIndebito;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoCalcolo.DatiEsitoCalcolo datiEsitoCalcolo = new GestioneEsitoCalcolo.DatiEsitoCalcolo();
                datiEsitoCalcolo.DettaglioEsito = messaggioVideo;

                if (AreaCalcolo.Response != null && AreaCalcolo.Response.Dati != null)
                {
                    switch (AreaCalcolo.Response.Dati.RZ_ESITO)
                    {
                        case 0:
                        case 1:
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                            {
                                //CALCOLATA (calcolo definitivo)
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
                                datiPensione.NCertificatoProvvisorio = null;
                                if (AreaCalcolo.Response.Dati.Stampa != null && AreaCalcolo.Response.Dati.Stampa.Intestazione != null)
                                {

                                    if (!Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                        datiPensione.NCertificato = AreaCalcolo.Response.Dati.Stampa.Intestazione.CERT;
                                    certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
                                    if (AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC != 0)
                                    {
                                        string dataCalcolo = AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC.ToString().PadLeft(8, '0');
                                        datiPensione.DataElaborazione = Utility.DataFromString(dataCalcolo, Utility.FormatoData.AAAAmmGG);
                                    }
                                }
                            }
                            else
                            {
                                //CALCOLO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcoloVerify;
                                if (AreaCalcolo.Response.Dati.Stampa != null && AreaCalcolo.Response.Dati.Stampa.Intestazione != null)
                                {
                                    if (!Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                        datiPensione.NCertificatoProvvisorio = AreaCalcolo.Response.Dati.Stampa.Intestazione.CERT;
                                    if (AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC != 0)
                                    {
                                        string dataCalcolo = AreaCalcolo.Response.Dati.Stampa.Intestazione.DATA_CALC.ToString().PadLeft(8, '0');
                                        datiPensione.DataElaborazione = Utility.DataFromString(dataCalcolo, Utility.FormatoData.AAAAmmGG);
                                    }
                                }
                            }
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = true;
                            break;
                        case 2:
                            if (datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
                                //SCARTO DA CALCOLO
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoDaCalcolo;
                            else
                                //SCARTO VERIFY
                                datiPensione.StatoPensione = (int)Utility.StatoPensione.ScartoVerify;
                            GestionePensione.SalvaPensione(datiPensione);
                            esito = false;
                            break;
                        default:
                            esito = false;
                            break;
                    }
                }
                if (esito)
                {
                    datiEsitoCalcolo.Esito = "OK";
                    if (datiPensione.StatoPensione == (int)Utility.StatoPensione.CalcolataNoWebDom)
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