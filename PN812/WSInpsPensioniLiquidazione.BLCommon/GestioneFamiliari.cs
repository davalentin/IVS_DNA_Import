using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneFamiliari
    {
        #region Familiare
        public static void GetFamiliariByIdPensione(long idPensione, out List<Familiare> Lifamiliare, out List<BLCommon.GestioneAnagrafica.DatiAnagrafici> Listanagfam)
        {
            Lifamiliare = new List<Familiare>();
            Listanagfam = new List<GestioneAnagrafica.DatiAnagrafici>();
            List<DataCommon.Familiare> mydblist = new List<INPS.Pensioni.Liquidazione.DataCommon.Familiare>();
            List<DataCommon.Anagrafica> mydblistanagrafica = new List<Anagrafica>();

            DAGestioneFamiliari.GetFamiliariByIdPensione(idPensione, out mydblist, out mydblistanagrafica);
            if (mydblist != null)
            {
                foreach (DataCommon.Familiare f in mydblist)
                {
                    Familiare familiareTemp = new Familiare();
                    BLCommon.GestioneAnagrafica.DatiAnagrafici anagraTemp = new GestioneAnagrafica.DatiAnagrafici();
                    Utility.ValorizzaOggetti(f, familiareTemp);
                    Utility.ValorizzaOggetti(f.Anagrafica, anagraTemp);
                    familiareTemp.CodiceFiscale = anagraTemp.CodiceFiscale;
                    Lifamiliare.Add(familiareTemp);
                    Listanagfam.Add(anagraTemp);
                }
            }
            else
                Listanagfam.Clear();
        }

        public static bool ValidateFamiliari(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, string cfFamiliareAttuale, List<Familiare> familiari, List<CodMaggFamiliari> elencoCodMaggFamiliari,
            AreaTitolare AreaTit, List<string> elencoFamiliariDaRimuovere, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, out string messaggioInfo)
        {
            messaggioInfo = string.Empty;
            string msgVideo = string.Empty;

            //ENG - Corretta Anomalia per la data sistema
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione));

            #region GetData
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            int annoCompetenza;
            GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

            GestioneAnagrafica.DatiStatoCivile ultimoStatoCivile = null;
            if (AreaTit != null && AreaTit.ElencoStatiCivili != null && AreaTit.ElencoStatiCivili.Count > 0)
                ultimoStatoCivile = AreaTit.ElencoStatiCivili.Last();

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa = null;
            if (datiDanteCausa != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausa.IdAnagrafica, out datiAnagraficiDanteCausa);

            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            List<GestioneComponenteFamiliare.ComponenteFamiliare> listaComponentiFamiliari = null;
            GestioneComponenteFamiliare.GetComponenteFamiliareByIdPensione(datiPensione.Id, out listaComponentiFamiliari);

            List<Familiare> familiariApp = familiari;
            if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa)
                || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)) || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda)
                || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                familiariApp = familiari.FindAll(x => x.CodiceFiscale != datiAnagraficiTitolare.CodiceFiscale);

            List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
            if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa) || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda)
                || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                GestioneAventiDiritto.GetAventiDirittoByIdPensione(datiPensione.Id, out listaAventiDiritto);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            char? appSesso;
            if (datiAnagraficiDanteCausa != null && !Utility.IsDomandaSpacchettamentoENPALS(datiPensione) && !Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa)
                && !(controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)) && !Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda)
                && !Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                appSesso = datiAnagraficiDanteCausa.Sesso;
            else
                appSesso = datiAnagraficiTitolare.Sesso;

            bool isDanteCausaPresent = false;
            if (datiAnagraficiDanteCausa != null && !Utility.IsDomandaSpacchettamentoENPALS(datiPensione) && !Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa)
                && !(controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda))
                && !Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                isDanteCausaPresent = true;

            bool presenzaConiugatoOrUnito = AreaTit != null && AreaTit.ElencoStatiCivili != null && AreaTit.ElencoStatiCivili.Count > 0 && AreaTit.ElencoStatiCivili.FindIndex(x => x.Codice == '2' || x.Codice == '7') > -1;
            bool presenzaConiuge = familiariApp != null && familiariApp.Count > 0 && familiariApp.FindIndex(x => x.IsConiugeOrUnitoCivile()) > -1;
            bool presenzaTitolare = familiariApp != null && familiariApp.Count > 0 && familiariApp.FindIndex(x => x.IdAnagrafica == datiAnagraficiTitolare.Id) > -1;

            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);

            byte? codiceConvenzione = null;
            int codicePrimoStatoEE = 0;
            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                codiceConvenzione = listaPrestazioniEstere[0].CodiceConvenzione;
                int.TryParse(listaPrestazioniEstere[0].CodiceStatoEE, out codicePrimoStatoEE);
            }

            GestioneControlliDinamici.ControlloDinamico abilitazioneMemo33 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo33" + tipoAppartenenza, out abilitazioneMemo33);

            Utility.TipoFondo? tipoFondo = null;
            tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            #endregion GetData

            if (AreaTit == null)
                throw new DNA.DnaValidationException("Dati Titolare non presenti");

            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("S"))
            {
                if (familiari != null && familiari.Count > 0)
                {
                    if (familiari[0].CodiceFiscale != AreaTit.Anagrafica.CodiceFiscale)
                        throw new DNA.DnaValidationException("Il primo familiare deve essere il titolare della pensione");
                }
            }
            if (tipoAppartenenza == Utility.TipoAppartenenza.AGO &&
                (Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria)))
            {
                if (familiariApp != null && familiariApp.Count > 0)
                {
                    if (familiariApp.Exists(x => !x.IsConiugeOrUnitoCivile() && x.SiglaFamiliare != null && x.SiglaFamiliare != ' ' && x.Confermato))
                        throw new DNA.DnaValidationException("Può essere acquisito solo il 'coniuge' o 'unito/a civilmente'.");
                }
            }

            if (tipoAppartenenza == Utility.TipoAppartenenza.CI)
            {
                msgVideo = string.Empty;
                if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) ||
                    (AreaTit != null && AreaTit.ElencoStatiCivili != null && AreaTit.ElencoStatiCivili.Count > 0 && AreaTit.ElencoStatiCivili.FindIndex(x => x.Codice == '2' || x.Codice == '7') > -1) ||
                    (familiariApp != null && familiariApp.Count > 0))
                {
                    if (!GestioneCrossControls.CI_VerificaReversObbligatorietaFamiliare(familiariApp != null && familiariApp.Count > 0, tipoDomanda, out msgVideo))
                        throw new DNA.DnaValidationException(msgVideo);
                }
            }

            if (elencoFamiliariDaRimuovere != null && elencoFamiliariDaRimuovere.Count > 0 &&
                listaComponentiFamiliari != null && listaComponentiFamiliari.Count > 0 && listaComponentiFamiliari.Exists(x => elencoFamiliariDaRimuovere.Contains(x.CodiceFiscale)))
                throw new DNA.DnaValidationException("Dati Familiari: Impossibile eliminare i familiari presenti tra i Componenti Familiari del record Dati No Calcolo");

            if (!GestioneCrossControls.ALL_VerificaFamiliariDuplicati(familiariApp, out msgVideo))
                throw new DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.ALL_VerificaFamiliariGenitori(familiariApp, out msgVideo))
                throw new DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.ALL_VerificaFamiliariConiugiTitolareConiugato(datiPensione, AreaTit, familiariApp, false, datiDanteCausa, isRiaperturaDomanda, out msgVideo))
                throw new DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.ALL_VerificaFamiliariTitolare(familiariApp, AreaTit, datiPensione, tipoAppartenenza, isRiaperturaDomanda, datiDanteCausa))
                throw new DNA.DnaValidationException("Dati Familiari: Il titolare pensione non può essere presente nell'elenco dei familiari.");

            if (!GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggFamiliariConiugi(familiariApp, elencoCodMaggFamiliari, out msgVideo))
                throw new DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.ALL_VerificaSovrapposizioneCodMaggFamiliariConiugi(familiariApp, elencoCodMaggFamiliari, out msgVideo))
                throw new DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.ALL_VerificaCessazioneCodMagg(familiariApp, elencoCodMaggFamiliari, dataSistema, out msgVideo))
                throw new DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.AGO_CI_VerificaFamiliariConiugiRicostituzioneOrRiapertura(datiPensione, datiEliminazione, familiariApp, elencoCodMaggFamiliari, isRiaperturaDomanda, out msgVideo))
                throw new DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.CI_ControlsQuotaContitolaritaNipote(datiPensione, tipoAppartenenza, familiariApp, elencoCodMaggFamiliari, annoCompetenza, out msgVideo))
                throw new INPS.DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.VerificaScadenzaContitolareNipoteNeJ(datiPensione, familiariApp, tipoAppartenenza, elencoCodMaggFamiliari, out msgVideo))
                throw new INPS.DNA.DnaValidationException(msgVideo);

            if (!GestioneCrossControls.VerificaContitolareNipoteO(datiPensione, familiariApp, datiDanteCausa, out msgVideo))
                throw new INPS.DNA.DnaValidationException(msgVideo);

            if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
            {
                msgVideo = string.Empty;
                if (!GestioneCrossControls.AGO_VerificaFamiliari(cfFamiliareAttuale, familiariApp, elencoCodMaggFamiliari, datiPensione, AreaTit.ElencoStatiCivili, datiDanteCausa,
                    datiPensioniDatiGenerici != null ? datiPensioniDatiGenerici.CumuloEsterno : null, dataSistema, isRiaperturaDomanda, out msgVideo))
                    throw new INPS.DNA.DnaValidationException(msgVideo);
                messaggioInfo = string.IsNullOrEmpty(msgVideo) ? messaggioInfo : msgVideo;
            }

            if (familiariApp != null && familiariApp.Count > 0)
            {
                int indexFam = 0;
                DateTime? appDecorrenzaCarico = DateTime.MinValue;
                DateTime? appCessazioneCarico = DateTime.MinValue;
                DateTime? decorrenzaCaricoCompare = DateTime.MinValue;
                DateTime? cessazioneCaricoCompare = DateTime.MinValue;
                foreach (Familiare fam in familiariApp)
                {
                    //Se il familiare deve essere salvato a DB, eseguo i controlli
                    if (fam.Confermato)
                    {
                        GestioneAnagrafica.DatiAnagrafici datiAnagraficiFamiliare = null;
                        GestioneAnagrafica.GetAnagraficaByIdAnagrafica(fam.IdAnagrafica, out datiAnagraficiFamiliare);

                        List<GestioneFamiliari.CodMaggFamiliari> LcodMaggFam = elencoCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica);

                        DateTime? app = null;
                        switch (fam.SiglaFamiliare)
                        {
                            case 'M':
                            case 'J':
                                if (datiAnagraficiFamiliare != null)
                                {
                                    if (datiAnagraficiFamiliare.DataNascita.HasValue)
                                    {
                                        app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Max(x => x.Cessazione) : null;
                                        if (app != null)
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).AddMonths(1).Date < app)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                        else
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).AddMonths(1).Date < datiPensione.DecorrenzaOriginaria)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                    }

                                    if (elencoCodMaggFamiliari.Exists(delegate(CodMaggFamiliari code) { return (code.IdAnagrafica == datiAnagraficiFamiliare.Id && !code.Cessazione.HasValue); }))
                                        throw new INPS.DNA.DnaValidationException("Tutte le date di Fine Carico dei Codici Maggiorazione associati al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                " devono essere obbligatorie");


                                    if (!GestioneCrossControls.ALL_VerificaDecorrenzaCessazioneFamiliari(datiPensione, tipoAppartenenza, fam, LcodMaggFam))
                                        throw new INPS.DNA.DnaValidationException("Non è consentito l'inserimento del 'SI' diritto da 03/2022 a nessun familiare che abbia sigla U, S, M, L. Cambiare codice maggiorazione o data inizio/fine carico");
                                }

                                break;
                            case 'S':
                                if (datiAnagraficiFamiliare != null)
                                {
                                    if (datiAnagraficiFamiliare.DataNascita.HasValue)
                                    {
                                        app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Max(x => x.Cessazione) : null;
                                        if (app != null)
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(21).AddMonths(1).Date < app)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                        else
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(21).AddMonths(1).Date < datiPensione.DecorrenzaOriginaria)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                    }

                                    if (elencoCodMaggFamiliari.Exists(delegate(CodMaggFamiliari code) { return (code.IdAnagrafica == datiAnagraficiFamiliare.Id && !code.Cessazione.HasValue); }))
                                        throw new INPS.DNA.DnaValidationException("Tutte le date di Fine Carico dei Codici Maggiorazione associati al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                " devono essere obbligatorie");

                                    if (!GestioneCrossControls.ALL_VerificaDecorrenzaCessazioneFamiliari(datiPensione, tipoAppartenenza, fam, LcodMaggFam))
                                        throw new INPS.DNA.DnaValidationException("Non è consentito l'inserimento del 'SI' diritto da 03/2022 a nessun familiare che abbia sigla U, S, M, L. Cambiare codice maggiorazione o data inizio/fine carico");

                                }

                                break;
                            case 'U':
                                if (datiAnagraficiFamiliare != null)
                                {
                                    if (datiAnagraficiFamiliare.DataNascita.HasValue)
                                    {
                                        app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Max(x => x.Cessazione) : null;
                                        if (app != null)
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(26).AddMonths(1).Date < app)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                        else
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(26).AddMonths(1).Date < datiPensione.DecorrenzaOriginaria)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                    }

                                    if (elencoCodMaggFamiliari.Exists(delegate(CodMaggFamiliari code) { return (code.IdAnagrafica == datiAnagraficiFamiliare.Id && !code.Cessazione.HasValue); }))
                                        throw new INPS.DNA.DnaValidationException("Tutte le date di Fine Carico dei Codici Maggiorazione associati al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                " devono essere obbligatorie");

                                    if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && abilitazioneMemo33 != null && !String.IsNullOrEmpty(abilitazioneMemo33.ValoreControllo) && abilitazioneMemo33.ValoreControllo.ToUpperInvariant() == "SI")
                                    {
                                        if (LcodMaggFam != null && LcodMaggFam.Count > 0 && LcodMaggFam.Exists(x => Utility.DataStrettamenteSuccessivaA(x.Cessazione.GetValueOrDefault(), new DateTime(2022, 03, 01))))
                                        {
                                            throw new INPS.DNA.DnaValidationException("Per la sigla familiare U non è possibile inserire una data fine carico maggiore di 03/2022");
                                        }
                                    }

                                    if (!GestioneCrossControls.ALL_VerificaDecorrenzaCessazioneFamiliari(datiPensione, tipoAppartenenza, fam, LcodMaggFam))
                                        throw new INPS.DNA.DnaValidationException("Non è consentito l'inserimento del 'SI' diritto da 03/2022 a nessun familiare che abbia sigla U, S, M, L. Cambiare codice maggiorazione o data inizio/fine carico");
                                }

                                break;
                            case 'C':
                                //in caso di coniuge- matrimonio, il sesso deve essere diverso da quelle del titolare o Dante Causa
                                if (fam.TipoUnione == "M")
                                {
                                    //ENG - RICOSTITUZIONI (NO Inpdap) bypassare il messaggio bloccante "Non è possibile inserire un Coniuge di sesso uguale al Titolare"
                                    if ((!(tipoAppartenenza == Utility.TipoAppartenenza.FS) || !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA) || !GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA_DINAMICO) || tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsDomandaINPDAP(datiPensione.Gestione)) && !(Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault() && datiPensione != null && datiPensione.SiglaCategoria != null && datiPensione.SiglaCategoria.StartsWith("S")))
                                    {
                                        if (appSesso.HasValue && appSesso.Value == datiAnagraficiFamiliare.Sesso.Value)
                                            throw new INPS.DNA.DnaValidationException(string.Format("Non è possibile inserire un Coniuge di sesso uguale al {0}", isDanteCausaPresent ? "Dante Causa" : "Titolare"));
                                    }

                                    if (datiAnagraficiFamiliare.CodiceFiscale == datiAnagraficiTitolare.CodiceFiscale ||
                                        (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) || Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa)
                                        || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)) || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda)
                                        || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda)))
                                    {
                                        if ((ultimoStatoCivile.Codice == '7' || ultimoStatoCivile.Codice == '8' || ultimoStatoCivile.Codice == 'C') &&
                                            LcodMaggFam != null && LcodMaggFam.Count > 0 && !LcodMaggFam.OrderBy(x => x.Decorrenza).Last().Cessazione.HasValue)
                                            throw new INPS.DNA.DnaValidationException("Non è possibile inserire il grado di parentela 'Coniuge' nel caso degli stati civili UNITO/A CIVILMENTE, SCIOLTO/A DALL'UNIONE o VEDOVO/A DA UNIONE CIVILE");
                                    }
                                }
                                else if (fam.TipoUnione == "U")
                                {
                                    //in caso di unito civile, il sesso deve essere uguale da quelle del titolare o Dante Causa
                                    if (appSesso.Value != datiAnagraficiFamiliare.Sesso.Value)
                                        throw new INPS.DNA.DnaValidationException(string.Format("Non è possibile inserire un 'Unito/a Civilmente' di sesso diverso dal {0}", isDanteCausaPresent ? "Dante Causa" : "Titolare"));

                                    if (datiAnagraficiFamiliare.CodiceFiscale == datiAnagraficiTitolare.CodiceFiscale ||
                                        (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) || Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa)
                                        || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)) || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda)
                                        || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda)))
                                    {
                                        if (!(ultimoStatoCivile.Codice == '7' || ultimoStatoCivile.Codice == '8' || ultimoStatoCivile.Codice == 'C') &&
                                            LcodMaggFam != null && LcodMaggFam.Count > 0 && !LcodMaggFam.OrderBy(x => x.Decorrenza).Last().Cessazione.HasValue)
                                            throw new INPS.DNA.DnaValidationException("Non è possibile inserire il grado di parentela 'Unito/a Civilmente' nel caso di stati civili diversi da UNITO/A CIVILMENTE, SCIOLTO/A DALL'UNIONE e VEDOVO/A DA UNIONE CIVILE");
                                    }

                                    if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Sentenza_Unioni_Civili.SENTENZA_UNIONI_CIVILI))
                                    {
                                        if (!Utility.IsDomandaSpacchettamentoENPALS(datiPensione) && !Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa) && !(controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)) && datiDanteCausa != null && datiDanteCausa.DataMorte.HasValue &&
                                            !Utility.DataSuccessivaA(datiDanteCausa.DataMorte.Value, new DateTime(2016, 6, 1)))
                                            throw new INPS.DNA.DnaValidationException("Non è possibile inserire il grado di parentela 'Unito/a Civilmente' se il Dante Causa è morto prima del 01/06/2016");
                                    }
                                }
                                break;

                            case 'N':
                                if (datiAnagraficiFamiliare != null)
                                {
                                    if (elencoCodMaggFamiliari.Exists(delegate(CodMaggFamiliari code) { return (code.IdAnagrafica == fam.IdAnagrafica && !code.Cessazione.HasValue); }))
                                        throw new INPS.DNA.DnaValidationException("Tutte le date di Fine Carico dei Codici Maggiorazione associati al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                " devono essere obbligatorie");
                                }
                                break;
                            case 'K':
                                if (datiAnagraficiFamiliare != null)
                                {
                                    if (datiAnagraficiFamiliare.DataNascita.HasValue)
                                    {
                                        app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Max(x => x.Cessazione) : null;
                                        if (app != null)
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).Date >= app)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                        else
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).Date >= datiPensione.DecorrenzaOriginaria)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                    }
                                }
                                break;
                            case 'W':
                                if (datiAnagraficiFamiliare != null)
                                {
                                    if (datiAnagraficiFamiliare.DataNascita.HasValue)
                                    {
                                        app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Max(x => x.Cessazione) : null;
                                        if (app != null)
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).Date > app || datiAnagraficiFamiliare.DataNascita.Value.AddYears(26).Date < app)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                        else
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).Date > datiPensione.DecorrenzaOriginaria || datiAnagraficiFamiliare.DataNascita.Value.AddYears(26).Date < datiPensione.DecorrenzaOriginaria)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                    }

                                    if (elencoCodMaggFamiliari.Exists(delegate(CodMaggFamiliari code) { return (code.IdAnagrafica == datiAnagraficiFamiliare.Id && !code.Cessazione.HasValue); }))
                                        throw new INPS.DNA.DnaValidationException("Tutte le date di Fine Carico dei Codici Maggiorazione associati al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                " devono essere obbligatorie");
                                }
                                break;
                            case 'Z':
                                if (datiAnagraficiFamiliare != null)
                                {
                                    if (datiAnagraficiFamiliare.DataNascita.HasValue)
                                    {
                                        app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Max(x => x.Cessazione) : null;
                                        if (app != null)
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).Date > app || datiAnagraficiFamiliare.DataNascita.Value.AddYears(21).Date < app)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                        else
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).Date > datiPensione.DecorrenzaOriginaria || datiAnagraficiFamiliare.DataNascita.Value.AddYears(21).Date < datiPensione.DecorrenzaOriginaria)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                    }
                                    if (elencoCodMaggFamiliari.Exists(delegate(CodMaggFamiliari code) { return (code.IdAnagrafica == datiAnagraficiFamiliare.Id && !code.Cessazione.HasValue); }))
                                        throw new INPS.DNA.DnaValidationException("Tutte le date di Fine Carico dei Codici Maggiorazione associati al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                " devono essere obbligatorie");
                                }
                                break;
                            case 'V':
                                if (abilitazioneMemo33 == null || String.IsNullOrEmpty(abilitazioneMemo33.ValoreControllo) || String.IsNullOrEmpty(abilitazioneMemo33.ValoreControllo.Trim()) || abilitazioneMemo33.ValoreControllo.Trim().ToUpperInvariant() == "NO")
                                {
                                    if (LcodMaggFam.Where(x => x.SiglaFamiliare != 'M' && x.SiglaFamiliare != 'S' && x.SiglaFamiliare != 'U').Count() > 1)
                                    {
                                        throw new INPS.DNA.DnaValidationException("Il Codice Maggiorazione 'V' può essere associato solo ai codici: 'S', 'M', 'U'");
                                    }
                                }
                                app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Max(x => x.Decorrenza) : null;
                                if (app != null)
                                {
                                    if (abilitazioneMemo33 == null || String.IsNullOrEmpty(abilitazioneMemo33.ValoreControllo) || String.IsNullOrEmpty(abilitazioneMemo33.ValoreControllo.Trim()) || abilitazioneMemo33.ValoreControllo.Trim().ToUpperInvariant() == "NO")
                                    {
                                        if (app <= new DateTime(2022, 03, 01))
                                        {
                                            throw new INPS.DNA.DnaValidationException("La decorrenza del Codice Maggiorazione 'V' associato al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                        " deve essere successiva al 03/2022");
                                        }
                                    }
                                    else
                                    {
                                        if (app < new DateTime(2022, 03, 01))
                                        {
                                            throw new INPS.DNA.DnaValidationException("La decorrenza del Codice Maggiorazione 'V' associato al Familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                        " deve essere successiva oppure uguale al 03/2022");
                                        }
                                    }
                                    if (datiAnagraficiFamiliare != null)
                                    {
                                        if (datiAnagraficiFamiliare.DataNascita.HasValue)
                                        {
                                            if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(18).Date >= app)
                                                throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                                    " non compatibile con il grado di parentela inserito");
                                        }
                                    }
                                    app = null;
                                }
                                app = LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam.Select(x => x.Cessazione).Last() : null;
                                if (app != null && datiAnagraficiFamiliare != null && datiAnagraficiFamiliare.DataNascita.HasValue)
                                {
                                    if (datiAnagraficiFamiliare.DataNascita.Value.AddYears(21).AddMonths(1).Date < app)
                                        throw new INPS.DNA.DnaValidationException("Età familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome +
                                            " non compatibile con il grado di parentela inserito");
                                }

                                break;

                            case 'L':
                                if (!GestioneCrossControls.ALL_VerificaDecorrenzaCessazioneFamiliari(datiPensione, tipoAppartenenza, fam, LcodMaggFam))
                                    throw new INPS.DNA.DnaValidationException("Non è consentito l'inserimento del 'SI' diritto da 03/2022 a nessun familiare che abbia sigla U, S, M, L. Cambiare codice maggiorazione o data inizio/fine carico");
                                break;
                        }

                        //per Coniuge e Figlio postumo nato entro 300 giorni dalla morte del Dante Causa il controllo sulla decorrenza carico non va effettuato
                        if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Sentenza_Nati_Dopo_300_Giorni.SENTENZA_NATI_DOPO_300_GIORNI) &&
                            Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && !(fam.SiglaFamiliare == 'C' || ((fam.SiglaFamiliare == 'M' || fam.SiglaFamiliare == 'S' || fam.SiglaFamiliare == 'U' || fam.SiglaFamiliare == 'I') && datiAnagraficiFamiliare != null && datiAnagraficiFamiliare.DataNascita.HasValue &&
                            datiDanteCausa != null && datiDanteCausa.DataMorte.HasValue && Utility.DataStrettamenteSuccessivaA(datiAnagraficiFamiliare.DataNascita.Value, datiDanteCausa.DataMorte.Value) &&
                            !Utility.DataStrettamenteSuccessivaA(datiAnagraficiFamiliare.DataNascita.Value, datiDanteCausa.DataMorte.Value.AddDays(300)))))
                        {                           
                            DateTime? decorrenzaOriginariaDaConfrontare = datiPensione.DecorrenzaOriginaria;                            
                            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)
                                && datiPensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1995, 9, 1)))
                                decorrenzaOriginariaDaConfrontare = new DateTime(datiPensione.DecorrenzaOriginaria.Value.Year, datiPensione.DecorrenzaOriginaria.Value.Month, 1);

                            if (!LcodMaggFam.Exists(x => x.Decorrenza == decorrenzaOriginariaDaConfrontare))
                                throw new INPS.DNA.DnaValidationException("È necessario avere un grado di parentela con data decorrenza carico uguale alla decorrenza della pensione.");
                        }

                        if (GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggDecorrenzaPensione(datiAnagraficiFamiliare.Id, elencoCodMaggFamiliari, AreaTit.Pensione.DecorrenzaOriginaria))
                            throw new INPS.DNA.DnaValidationException(string.Format("Tutte le date di Inizio Carico dei Codici Maggiorazione associati al Familiare {0} {1} devono essere successive alla data di decorrenza della Pensione: {2:dd/MM/yyyy}",
                                datiAnagraficiFamiliare.Cognome, datiAnagraficiFamiliare.Nome, AreaTit.Pensione.DecorrenzaOriginaria.Value));

                        if (!GestioneCrossControls.ALL_VerificaDataMorte(datiPensione.DecorrenzaOriginaria, fam.DataMorte))
                            throw new INPS.DNA.DnaValidationException("Familiare non acquisibile. La data di morte risulta inferiore alla decorrenza originaria");

                        if (!(fam.TipoComponente == 'T' && tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)))
                        {
                            if (!GestioneCrossControls.ALL_VerificaPresenzaCodMaggiorazione(datiPensione, datiAnagraficiFamiliare, elencoCodMaggFamiliari, out msgVideo))
                                throw new INPS.DNA.DnaValidationException(msgVideo);
                        }

                        if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI)
                        {
                            if (LcodMaggFam != null && LcodMaggFam.Count > 0)
                            {
                                int index = 0;
                                foreach (GestioneFamiliari.CodMaggFamiliari codice in LcodMaggFam)
                                {
                                    if ((!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("S")) ||
                                        (AreaTit != null && AreaTit.ElencoStatiCivili != null && AreaTit.ElencoStatiCivili.Count > 0 && AreaTit.ElencoStatiCivili.FindIndex(x => x.Codice == '2' || x.Codice == '7') > -1) ||
                                        (familiariApp != null && familiariApp.Count > 0))
                                    {
                                        if (codice.CodiceMaggiorazione > 2)
                                            throw new INPS.DNA.DnaValidationException("Codice Maggiorazione errato per il familiare " + datiAnagraficiFamiliare.Cognome + " " + datiAnagraficiFamiliare.Nome + " ('SI' / 'NO' / '  ')");

                                        if (!GestioneCrossControls.CI_VerificaCodiceMaggiorazioneWithPeriodo(tipoDomanda, fam, codice.CodiceMaggiorazione, codice.Decorrenza, codice.Cessazione, ref appDecorrenzaCarico, ref appCessazioneCarico, ref decorrenzaCaricoCompare, ref cessazioneCaricoCompare, out msgVideo))
                                            throw new INPS.DNA.DnaValidationException(msgVideo);

                                        if (index == 0)
                                        {
                                            if (!GestioneCrossControls.CI_VerificaDateFamiliari(tipoDomanda, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiAnagraficiFamiliare.DataNascita, codice.Decorrenza, datiPensione.DecorrenzaOriginaria, fam.SiglaFamiliare, out msgVideo))
                                                throw new INPS.DNA.DnaValidationException(msgVideo);
                                        }

                                        if (!GestioneCrossControls.CI_VerificaCessazioneCarico(fam.SiglaFamiliare, datiAnagraficiFamiliare.DataNascita, annoCompetenza, datiPensione.CausaCarico, codice.Cessazione, codice.Decorrenza, out msgVideo))
                                            throw new INPS.DNA.DnaValidationException(msgVideo);

                                        if (!GestioneCrossControls.CI_VerificaSiglaFamiliareWithDC(tipoDomanda, codice.Cessazione, datiAnagraficiTitolare.CodiceFiscale, datiAnagraficiFamiliare.CodiceFiscale, fam,
                                            datiDanteCausa != null ? datiDanteCausa.ParentelaDC : null, ultimoStatoCivile != null ? ultimoStatoCivile.Codice : '0', annoCompetenza, codice.CodiceMaggiorazione, out msgVideo))
                                            throw new INPS.DNA.DnaValidationException(msgVideo);
                                    }

                                    if (!ControlsFamiliare(fam.SiglaFamiliare, datiPensione.Gruppo, datiPensione.Prodotto, codice.CodiceMaggiorazione, datiPensione, out msgVideo))
                                        throw new INPS.DNA.DnaValidationException(msgVideo);

                                    index++;
                                }
                            }

                            #region PCIPL11
                            if ((!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("S")) ||
                                (AreaTit != null && AreaTit.ElencoStatiCivili != null && AreaTit.ElencoStatiCivili.Count > 0 && AreaTit.ElencoStatiCivili.FindIndex(x => x.Codice == '2' || x.Codice == '7') > -1) ||
                                (familiariApp != null && familiariApp.Count > 0))
                            {
                                if (!Utility.VerificaData(datiAnagraficiFamiliare.DataNascita, tipoAppartenenza, out msgVideo))
                                {
                                    msgVideo = "Data Nascita: " + msgVideo;
                                    throw new INPS.DNA.DnaValidationException(msgVideo);
                                }

                                if (datiAnagraficiFamiliare.Sesso != 'F' && datiAnagraficiFamiliare.Sesso != 'M')
                                {
                                    msgVideo = "Codice Sesso errato: 'M' / 'F'";
                                    throw new INPS.DNA.DnaValidationException(msgVideo);
                                }

                                if (!GestioneCrossControls.CI_VerificaCognomeAcquisitoWithSesso(datiAnagraficiFamiliare.CognomeAcquisito, datiAnagraficiFamiliare.Sesso, elencoCodMaggFamiliari, out msgVideo))
                                    throw new INPS.DNA.DnaValidationException(msgVideo);

                                if (!GestioneCrossControls.CI_VerificaSiglaFamiliareWithDataNascita(fam, datiAnagraficiFamiliare.DataNascita, out msgVideo))
                                    throw new INPS.DNA.DnaValidationException(msgVideo);

                                if (!GestioneCrossControls.CI_VerificaCoerenzaPeriodi(familiariApp, elencoCodMaggFamiliari, out msgVideo))
                                    throw new INPS.DNA.DnaValidationException(msgVideo);

                                if (!GestioneCrossControls.CI_VerificaDataMatrimonioDC(tipoDomanda, datiPensione.DecorrenzaOriginaria, presenzaConiugatoOrUnito, presenzaConiuge, datiAnagraficiDanteCausa != null ? datiAnagraficiDanteCausa.DataMatrimonio : null, out msgVideo))
                                    throw new INPS.DNA.DnaValidationException(msgVideo);

                                if (!GestioneCrossControls.CI_VerificaObbligatorietaContitolare(tipoDomanda, datiEliminazione != null ? datiEliminazione.CodiceMotivo : null, presenzaTitolare,
                                    ultimoStatoCivile != null ? ultimoStatoCivile.Codice : '0', presenzaConiuge, categoria, out msgVideo))
                                    throw new INPS.DNA.DnaValidationException(msgVideo);

                                if (!GestioneCrossControls.CI_VerificaScadenzaRevisioneSanitariaWithDatiGenerici(fam.ScadenzaRevisioneSanitaria, fam.SiglaFamiliare, datiPensione.CausaCarico, out msgVideo))
                                    throw new INPS.DNA.DnaValidationException(msgVideo);

                                if (!GestioneCrossControls.CI_VerificaScadenzaRevisioneSanitaria(tipoDomanda, datiPensione.DecorrenzaOriginaria, datiAnagraficiFamiliare.DataNascita, indexFam, LcodMaggFam, fam,
                                    LcodMaggFam != null && LcodMaggFam.Count > 0 ? LcodMaggFam[0].CodiceMaggiorazione : null, out msgVideo))
                                    throw new INPS.DNA.DnaValidationException(msgVideo);
                            }
                            #endregion PCIPL11
                        }

                        if (!GestioneCrossControls.ALL_VerificaNuovoCodMaggConCessazionePrecedente(LcodMaggFam, out msgVideo))
                        {
                            throw new INPS.DNA.DnaValidationException(msgVideo);
                        }

                        if (LcodMaggFam != null && LcodMaggFam.Count > 0 &&
                            LcodMaggFam.Exists(x => x.Decorrenza.HasValue && x.Cessazione.HasValue && !Utility.DataStrettamenteSuccessivaA(x.Cessazione.Value, x.Decorrenza.Value)))
                        {
                            messaggioInfo = "Per il familiare " + fam.CodiceFiscale + " la data fine carico non può essere inferiore alla data decorrenza carico";
                            if (string.IsNullOrEmpty(cfFamiliareAttuale) || cfFamiliareAttuale == fam.CodiceFiscale)
                                throw new INPS.DNA.DnaValidationException(messaggioInfo);
                        }

                        //per FS modificato su familiare corrente e spostato in invio per robustezza (INC000002025710)                        
                        if ((fam.CodiceFiscale == cfFamiliareAttuale || !(tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)) && (fam.SiglaFamiliare == 'N' || fam.SiglaFamiliare == 'J'))
                        {
                            if (!GestioneCrossControls.ALL_VerificaMaggiorazioneFamiliariNeJ(datiPensione, tipoAppartenenza, fam, LcodMaggFam, out msgVideo))
                                throw new INPS.DNA.DnaValidationException(msgVideo);
                        }

                        indexFam++;
                    }

                    if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI)
                    {
                        #region PCIPL11
                        if (tipoDomanda == Utility.TipoDomanda.Superstiti ||
                            (AreaTit != null && AreaTit.ElencoStatiCivili != null && AreaTit.ElencoStatiCivili.Count > 0 && AreaTit.ElencoStatiCivili.FindIndex(x => x.Codice == '2' || x.Codice == '7') > -1) ||
                            (familiariApp != null && familiariApp.Count > 0))
                        {
                            if (!GestioneCrossControls.CI_VerificaSiglaFamiliareAscendente(familiariApp, out msgVideo))
                                throw new INPS.DNA.DnaValidationException(msgVideo);
                        }
                        #endregion PCIPL11
                    }
                }


                msgVideo = string.Empty;
                if (!GestioneCrossControls.ALL_ControlsFamiliariWithStatiCivili(familiariApp, elencoCodMaggFamiliari, AreaTit.ElencoStatiCivili, tipoAppartenenza, datiAnagraficiTitolare.DataMorte, out msgVideo))
                    throw new INPS.DNA.DnaValidationException(msgVideo);

                if (!GestioneCrossControls.ALL_VerificaFamiliariMorti(familiariApp, elencoCodMaggFamiliari, datiPensione.DecorrenzaOriginaria, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gruppo), out msgVideo, datiPensione, datiEliminazione))
                    throw new INPS.DNA.DnaValidationException(msgVideo);

                if (!GestioneCrossControls.AGO_ControlsQuotaContitolaritaNipote(datiPensione, tipoAppartenenza, familiariApp, elencoCodMaggFamiliari, annoCompetenza, out msgVideo))
                    throw new INPS.DNA.DnaValidationException(msgVideo);

                if (!GestioneCrossControls.AGO_ControlsDataCessazioneFamiliari(datiPensione, tipoAppartenenza, familiariApp, elencoCodMaggFamiliari, datiPensione.SiglaCategoria, datiEliminazione, out msgVideo))
                    throw new INPS.DNA.DnaValidationException(msgVideo);

                if (!GestioneCrossControls.AGO_VerificaPresenzaFamiliariInAventiDiritto(cfFamiliareAttuale, datiPensione, tipoAppartenenza, familiariApp, listaAventiDiritto, AreaTit != null && AreaTit.Anagrafica != null ? AreaTit.Anagrafica : null, elencoCodMaggFamiliari, datiDanteCausa, out msgVideo))
                    throw new INPS.DNA.DnaValidationException(msgVideo);

                if (!GestioneCrossControls.ALL_VerificaSiglaFamiliareV(datiPensione, tipoAppartenenza, familiariApp, elencoCodMaggFamiliari, out msgVideo))
                    throw new INPS.DNA.DnaValidationException(msgVideo);

                if (!GestioneCrossControls.ALL_VerificaPlurimeRegistrazioniConiugeUnitoCivile(datiPensione, tipoAppartenenza, familiariApp, elencoCodMaggFamiliari, out msgVideo))
                    throw new INPS.DNA.DnaValidationException(msgVideo);

                messaggioInfo = string.IsNullOrEmpty(msgVideo) ? messaggioInfo : msgVideo;
            }

            return true;
        }

        public static void StoreFamiliari(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, string cfFamiliareAttuale, List<Familiare> familiari, List<CodMaggFamiliari> elencoCodMaggFamiliari,
            List<long> IdAnagraficheDaRimuovere, List<string> elencoFamiliariDaRimuovere, out string messaggioInfo)
        {
            messaggioInfo = string.Empty;
            GestioneQuadri.DatiQuadroFamiliari datiQuadroFamiliari = null;
            GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out datiQuadroFamiliari);
            Entity.AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            //ENG - Spacchettate SOPGI
            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            //ENG - REVERSIBILITA FS (NO GPD/024)
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DisabilitaDetrazioniObbligatorieContitolariFS", out controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS);
            int annoCompetenzaFS = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out annoCompetenzaFS);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);

            bool validate = false;
            validate = ValidateFamiliari(datiPensione, isRiaperturaDomanda, cfFamiliareAttuale, familiari, elencoCodMaggFamiliari, areaTitolare, elencoFamiliariDaRimuovere, controlloDinamicoSpacchettate024, out messaggioInfo);

            if (validate)
            {
                GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = null;
                GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni = null;
                bool aggiornaQuadroRedditi = ControlsAggiornamentoQuadroRedditi(datiPensione, familiari, IdAnagraficheDaRimuovere, out datiQuadroRedditi);
                bool aggiornaQuadroDetrazioni = ControlsAggiornamentoQuadroDetrazioni(datiPensione, familiari, areaTitolare, isRiaperturaDomanda, danteCausa, controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS, annoCompetenzaFS, tipoAppartenenza, tipoFondo, elencoCodMaggFamiliari, out datiQuadroDetrazioni);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    List<CodMaggiorazioneFamiliari> elencoCodMaggFamiliariDB = null;
                    foreach (Familiare f in familiari)
                    {
                        if (f.Confermato)
                        {
                            DataCommon.Familiare DBfamiliare = new DataCommon.Familiare();
                            Utility.ValorizzaOggetti(f, DBfamiliare);
                            DAGestioneFamiliari.SalvaFamiliare(DBfamiliare);

                            if (elencoCodMaggFamiliari != null)
                            {
                                if (elencoCodMaggFamiliariDB == null)
                                    elencoCodMaggFamiliariDB = new List<CodMaggiorazioneFamiliari>();
                                foreach (CodMaggFamiliari codMagg in elencoCodMaggFamiliari)
                                {
                                    if (f.IdAnagrafica == codMagg.IdAnagrafica)
                                    {
                                        CodMaggiorazioneFamiliari codMaggDB = new CodMaggiorazioneFamiliari();
                                        Utility.ValorizzaOggetti(codMagg, codMaggDB);
                                        elencoCodMaggFamiliariDB.Add(codMaggDB);
                                    }
                                }
                            }
                        }
                    }

                    DAGestioneFamiliari.SalvaCodMaggiorazioneFamiliari(datiPensione.Id, elencoCodMaggFamiliariDB);

                    foreach (long idAnagrafica in IdAnagraficheDaRimuovere)
                    {
                        DAGestioneRichiestaDomandeANF.DeleteRichiestaRicercaDomandeANFByIdAnagrafica(datiPensione.Id, idAnagrafica);
                        DAGestioneDetrazioniContitolare.EliminaDetrazioniImpostaContitolareNoStoricoBySoggetto(datiPensione.Id, idAnagrafica);
                        DAGestioneFamiliari.DeleteCodMaggiorazioneFamiliariPerFamiliare(idAnagrafica, datiPensione.Id);
                        DAGestioneFamiliari.CancellaFamiliare(idAnagrafica, datiPensione.Id);
                    }

                    if (familiari.Count > 0)
                    {
                        List<Familiare> familiariApp = familiari;
                        if ((Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa) || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda))
                            || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda)) && areaTitolare != null && areaTitolare.Anagrafica != null)
                            familiariApp = familiari.FindAll(x => x.CodiceFiscale != areaTitolare.Anagrafica.CodiceFiscale);

                        if (familiariApp.FindIndex(x => !x.Confermato) == -1)
                        {
                            //se ultimo stato civile è coniugato, è necessario verificare se è stato inserito un coniuge
                            if (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0)
                            {

                                //areaTitolare.ElencoStatiCivili = areaTitolare.ElencoStatiCivili.OrderBy(x => x.Decorrenza).ToList<GestioneAnagrafica.DatiStatoCivile>();
                                //if (areaTitolare.ElencoStatiCivili[areaTitolare.ElencoStatiCivili.Count - 1].Codice == 2)

                                if (areaTitolare.ElencoStatiCivili.FindAll(x => x.Codice == '2' || x.Codice == '7').Count != 0)
                                {
                                    bool presenzaConiugeOrUnito = false;
                                    foreach (Familiare fam in familiari)
                                    {
                                        if (fam.IsConiugeOrUnitoCivile() && fam.Confermato)
                                        {
                                            presenzaConiugeOrUnito = true;
                                            break;
                                        }
                                    }
                                    if (presenzaConiugeOrUnito)
                                        datiQuadroFamiliari.TabFamiliari = 2;
                                    else
                                        datiQuadroFamiliari.TabFamiliari = 0;
                                }
                                else
                                    datiQuadroFamiliari.TabFamiliari = 2;
                            }
                            else
                                datiQuadroFamiliari.TabFamiliari = 2;
                        }
                        else
                            datiQuadroFamiliari.TabFamiliari = 0;
                    }
                    else
                    {
                        if (datiQuadroFamiliari.Tipo == 1)
                            datiQuadroFamiliari.TabFamiliari = 1;
                        //modifica per prepopolamento familiari
                        else if (datiQuadroFamiliari.Tipo == 2)
                        {
                            if (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0)
                            {
                                //areaTitolare.ElencoStatiCivili = areaTitolare.ElencoStatiCivili.OrderBy(x => x.Decorrenza).ToList<GestioneAnagrafica.DatiStatoCivile>();
                                //if (areaTitolare.ElencoStatiCivili[areaTitolare.ElencoStatiCivili.Count - 1].Codice == 2)
                                if (areaTitolare.ElencoStatiCivili.FindAll(x => x.Codice == '2' || x.Codice == '7').Count != 0 ||
                                    Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Superstiti)
                                    datiQuadroFamiliari.TabFamiliari = 0;
                                else
                                {
                                    datiQuadroFamiliari.Tipo = 1;
                                    datiQuadroFamiliari.TabFamiliari = 1;
                                }
                            }
                            else
                            {
                                datiQuadroFamiliari.Tipo = 1;
                                datiQuadroFamiliari.TabFamiliari = 1;
                            }
                        }
                    }

                    GestioneQuadri.SalvaQuadroFamiliari(datiPensione.Id, datiQuadroFamiliari);

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

                        if (aggiornaQuadroDetrazioni)
                        {
                            datiQuadroDetrazioni.TabDetrazioni = 0;
                            GestioneQuadri.SalvaQuadroDetrazioni(datiPensione.Id, datiQuadroDetrazioni);
                        }
                    }

                    if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.FS &&
                        Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                    {

                        datiQuadroDetrazioni.Tipo = 2;
                        datiQuadroDetrazioni.TabDetrazioni = 0;
                        GestioneQuadri.SalvaQuadroDetrazioni(datiPensione.Id, datiQuadroDetrazioni);

                    }

                    transactionScope.Complete();
                }
            }
        }

        public static void SbloccaFamiliari(GestionePensione.DatiPensione datiPensione, List<Familiare> familiari)
        {
            GestioneQuadri.DatiQuadroFamiliari datiQuadroFamiliari = null;
            GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out datiQuadroFamiliari);
            if (datiQuadroFamiliari != null)
            {
                datiQuadroFamiliari.Tipo = 2;
                datiQuadroFamiliari.TabFamiliari = 0;
            }
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (Familiare fam in familiari)
                {
                    if (fam.TipoComponente != 'T')
                    {
                        fam.Confermato = false;
                        DataCommon.Familiare DBfamiliare = new DataCommon.Familiare();
                        Utility.ValorizzaOggetti(fam, DBfamiliare);
                        DAGestioneFamiliari.SalvaFamiliare(DBfamiliare);
                    }
                }
                GestioneQuadri.SalvaQuadroFamiliari(datiPensione.Id, datiQuadroFamiliari);
                transactionScope.Complete();
            }
        }

        public static void DeleteAllFamiliari(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFamiliari.DeleteAllFamiliariByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void CheckFamiliariByIdPensione(long idPensione, out bool presenzaFamiliari)
        {
            presenzaFamiliari = false;
            DAGestioneFamiliari.CheckFamiliariByIdPensione(idPensione, out presenzaFamiliari);
        }

        public static void SalvaFamiliare(Familiare familiare, List<CodMaggFamiliari> elencoCodMaggFamiliari, GestioneAnagrafica.DatiAnagrafici anagrafica, GestioneFamiliari.DatiRichiestaRicercaDomandeANF richiesta, long idPensione, string siglaCategoria)
        {
            GestioneAnagrafica.SalvaAnagrafica(anagrafica);

            DataCommon.Familiare DBfamiliare = new INPS.Pensioni.Liquidazione.DataCommon.Familiare();
            Utility.ValorizzaOggetti(familiare, DBfamiliare);
            DBfamiliare.IdAnagrafica = familiare.IdAnagrafica = anagrafica.Id;
            DBfamiliare.IdPensione = idPensione;
            DAGestioneFamiliari.SalvaFamiliare(DBfamiliare);

            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, siglaCategoria);
            List<CodMaggiorazioneFamiliari> elencoCodMaggFamiliariDB = null;
            if (elencoCodMaggFamiliari != null)
            {
                elencoCodMaggFamiliariDB = new List<CodMaggiorazioneFamiliari>();
                foreach (CodMaggFamiliari codMagg in elencoCodMaggFamiliari)
                {
                    CodMaggiorazioneFamiliari codMaggDB = new CodMaggiorazioneFamiliari();
                    Utility.ValorizzaOggetti(codMagg, codMaggDB);
                    codMaggDB.IdAnagrafica = anagrafica.Id;
                    codMaggDB.IdPensione = idPensione;
                    if (categoriaFondoPI != null)
                    {
                        codMaggDB.Decorrenza = codMagg.Decorrenza.HasValue ? codMagg.Decorrenza : null;
                        codMaggDB.Cessazione = codMagg.Cessazione.HasValue ? codMagg.Cessazione : null;
                        codMaggDB.DirittoAF = codMagg.DirittoAF != null ? codMagg.DirittoAF : "";
                        codMaggDB.QuotaAF = codMagg.QuotaAF != null ? codMagg.QuotaAF : null;
                        codMaggDB.ContitolaritaAgo = codMagg.ContitolaritaAgo != null ? codMagg.ContitolaritaAgo : null;
                        codMaggDB.ContitolaritaFondo = codMagg.ContitolaritaFondo != null ? codMagg.ContitolaritaFondo : null;
                    }
                    elencoCodMaggFamiliariDB.Add(codMaggDB);
                }
            }
            DAGestioneFamiliari.SalvaCodMaggiorazioneFamiliari(idPensione, elencoCodMaggFamiliariDB);

            if (richiesta != null)
            {
                RichiestaRicercaDomandeANF richiestaDB = new RichiestaRicercaDomandeANF();
                richiestaDB.IdAnagrafica = anagrafica.Id;
                richiestaDB.IdPensione = idPensione;
                richiestaDB.Guid = richiesta.Guid;
                richiestaDB.DataRichiesta = richiesta.DataRichiesta;
                DAGestioneRichiestaDomandeANF.SalvaRichiestaRicercaDomandeANF(richiestaDB);
            }
        }

        private static bool ControlsAggiornamentoQuadroRedditi(GestionePensione.DatiPensione datiPensione, List<Familiare> familiari, List<long> IdAnagraficheDaRimuovere, out GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi)
        {
            datiQuadroRedditi = null;
            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out datiQuadroRedditi);
            //in caso di redditi già acquisiti
            if (datiQuadroRedditi != null && datiQuadroRedditi.TabRedditi.HasValue && datiQuadroRedditi.TabRedditi.Value == 2)
            {
                //in caso di familiari da cancellare ritorna true
                if (IdAnagraficheDaRimuovere != null && IdAnagraficheDaRimuovere.Count > 0)
                    return true;
                List<Familiare> familiariAttualiDB = null;
                List<GestioneAnagrafica.DatiAnagrafici> anagraficheAttualiDB = null;
                GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out familiariAttualiDB, out anagraficheAttualiDB);
                //in caso di familiari discordanti in numero tra app e DB ritorna true
                if (((familiariAttualiDB == null || familiariAttualiDB.Count == 0) && familiari != null && familiari.Count > 0) ||
                    ((familiari == null || familiari.Count == 0) && familiariAttualiDB != null && familiariAttualiDB.Count > 0) ||
                    (familiariAttualiDB != null && familiari != null && familiariAttualiDB.Count != familiari.Count))
                    return true;

                if (familiari != null && familiari.Count > 0 && familiariAttualiDB != null && familiariAttualiDB.Count > 0)
                {
                    //in caso di familiari concordanti in numero tra app e DB ma con parentele e/o soggetti diversi ritorna true
                    foreach (Familiare f in familiari)
                    {
                        if (familiariAttualiDB.Find(x => x.CodiceFiscale.Trim().ToUpperInvariant() == f.CodiceFiscale.Trim().ToUpperInvariant() &&
                            x.SiglaFamiliare.HasValue && f.SiglaFamiliare.HasValue && x.SiglaFamiliare.Value == f.SiglaFamiliare.Value) == null)
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool ControlsAggiornamentoQuadroDetrazioni(GestionePensione.DatiPensione datiPensione, List<Familiare> familiari, AreaTitolare areaTitolare, bool isRiaperturaDomanda,
           GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneControlliDinamici.ControlloDinamico controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS, int annoCompetenzaFS,
            Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoFondo? tipoFondo, List<CodMaggFamiliari> elencoCodMaggFamiliari, out GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni)
        {
            datiQuadroDetrazioni = null;
            GestioneQuadri.GetQuadroDetrazioniByDatiPensione(datiPensione, out datiQuadroDetrazioni);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != Utility.TipoAppartenenza.FS || !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                || (controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)))
                return false;

            //in caso di detrazioni già acquisite
            if (datiQuadroDetrazioni != null && datiQuadroDetrazioni.TabDetrazioni.HasValue && datiQuadroDetrazioni.TabDetrazioni.Value == 2)
            {
                if (familiari != null && familiari.Count > 0)
                {
                    //in caso di nuovi familiari o di familiari per cui non sono ancora state acquisite le detrazioni ritorna true
                    foreach (Familiare f in familiari)
                    {
                        if (f.CodiceFiscale == areaTitolare.Anagrafica.CodiceFiscale)
                            continue;

                        GestioneDetrazioniContitolare.DatiDetrazioniContitolare datiDetrazioniContitolare = null;
                        GestioneDetrazioniContitolare.GetDetrazioniBySoggetto(datiPensione.Id, f.IdAnagrafica, out datiDetrazioniContitolare);

                        //ENG - REVERSIBILITA FS (NO GDP/024)                     
                        bool isDetrazioniObbligatorieContitolare = true;
                        if (controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS != null && !String.IsNullOrEmpty(controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS.ValoreControllo)
                            && controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS.ValoreControllo.ToUpperInvariant() == "SI")
                        {
                            if (tipoAppartenenza == Utility.TipoAppartenenza.FS && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa) && !Utility.IsDomandaINPDAP(datiPensione.Gestione)
                                && tipoFondo != Utility.TipoFondo.FS && tipoFondo != Utility.TipoFondo.PT)
                            {
                                if (f.TipoComponente != 'T' && elencoCodMaggFamiliari != null && elencoCodMaggFamiliari.Exists(x => x.IdAnagrafica == f.IdAnagrafica && x.Cessazione.HasValue))
                                {
                                    DateTime? dataCessazioneContitolare = elencoCodMaggFamiliari.FindAll(x => x.IdAnagrafica == f.IdAnagrafica && x.Cessazione.HasValue).OrderByDescending(x => x.Cessazione).First().Cessazione;
                                    if (dataCessazioneContitolare.HasValue && dataCessazioneContitolare.Value.Year < annoCompetenzaFS)
                                        isDetrazioniObbligatorieContitolare = false;
                                }
                            }
                        }

                        if (datiDetrazioniContitolare == null && isDetrazioniObbligatorieContitolare)
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool ControlsFamiliare(char? codeFamiliare, string gruppo, string prodotto, byte? CodeMagg, GestionePensione.DatiPensione datiPensione, out string msgVideo)
        {
            msgVideo = string.Empty;

            if (!GestioneCrossControls.CI_VerificaNoReversCodeMaggiorazioneConiuge(codeFamiliare, gruppo, prodotto, CodeMagg))
            {
                msgVideo = "Codice Maggiorazione errato o mancante ('SI' / 'NO')";
                return false;
            }

            if (!GestioneCrossControls.CI_VerificaNoReversCodeMaggiorazioneNoConiuge(codeFamiliare, gruppo, prodotto, CodeMagg, datiPensione))
            {
                msgVideo = "Codice Maggiorazione non deve essere acquisito";
                return false;
            }

            if (!GestioneCrossControls.CI_VerificaReversCodeMaggiorazioneConiuge(codeFamiliare, gruppo, prodotto, CodeMagg))
            {
                msgVideo = "Codice Maggiorazione non deve essere acquisito (coniuge sup.)";
                return false;
            }

            if (!GestioneCrossControls.CI_VerificaReversCodeMaggiorazioneNoConiuge(codeFamiliare, gruppo, prodotto, CodeMagg))
            {
                msgVideo = "Codice Maggiorazione errato o mancante (SI / NO)";
                return false;
            }

            return true;
        }
        #endregion Familiare

        #region Richiesta Domande ANF
        public static void DeleteAllRichiestaRicercaDomandeANF(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRichiestaDomandeANF.DeleteAllRichiestaRicercaDomandeANF(idPensione);
                transactionScope.Complete();
            }
        }

        public static void DeleteRichiestaRicercaDomandeANF(long idPensione, long idAnagrafica)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRichiestaDomandeANF.DeleteRichiestaRicercaDomandeANFByIdAnagrafica(idPensione, idAnagrafica);
                transactionScope.Complete();
            }
        }

        public static void SalvaRichiestaRicercaDomandaANF(DatiRichiestaRicercaDomandeANF richiesta)
        {
            RichiestaRicercaDomandeANF richiestaDB = new RichiestaRicercaDomandeANF();
            richiestaDB.IdPensione = richiesta.IdPensione;
            richiestaDB.IdAnagrafica = richiesta.IdAnagrafica;
            richiestaDB.Guid = richiesta.Guid;
            richiestaDB.DataRichiesta = richiesta.DataRichiesta;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRichiestaDomandeANF.SalvaRichiestaRicercaDomandeANF(richiestaDB);
                transactionScope.Complete();
            }
        }

        public static void GetRichiesteRicercaDomandeANFByIdPensione(long idPensione, out List<DatiRichiestaRicercaDomandeANF> listaRichieste)
        {
            listaRichieste = null;
            List<RichiestaRicercaDomandeANF> listaRichiesteDB = null;
            DAGestioneRichiestaDomandeANF.GetRichiesteRicercaDomandeANF(idPensione, out listaRichiesteDB);
            if (listaRichiesteDB != null && listaRichiesteDB.Count > 0)
            {
                listaRichieste = new List<DatiRichiestaRicercaDomandeANF>();
                foreach (RichiestaRicercaDomandeANF richiestaDB in listaRichiesteDB)
                {
                    DatiRichiestaRicercaDomandeANF richiesta = new DatiRichiestaRicercaDomandeANF();
                    richiesta.Id = richiestaDB.Id;
                    richiesta.IdPensione = richiestaDB.IdPensione;
                    richiesta.IdAnagrafica = richiestaDB.IdAnagrafica;
                    richiesta.Guid = richiestaDB.Guid;
                    richiesta.DataRichiesta = richiestaDB.DataRichiesta;
                    listaRichieste.Add(richiesta);
                }
            }
        }

        public static bool ControllaRispostaANF(string risposta, out GestioneFamiliari.ConsultazioneUnificataANF consultazioneAnf, out string errori)
        {
            errori = string.Empty;
            consultazioneAnf = null;

            if (!string.IsNullOrEmpty(risposta) && !risposta.Contains("Errore"))
            {
                var doc = XDocument.Parse(risposta);
                if (doc != null)
                {
                    var root = doc.Root;
                    if (root != null)
                    {
                        //Carico dal database l'elenco delle fonti non ammissibili
                        GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("FontiDatiANFNonAmmissibili", out controlloDinamico);
                        string[] listaFontiNonAmmissibili = (controlloDinamico != null && !String.IsNullOrEmpty(controlloDinamico.ValoreControllo)) ? controlloDinamico.ValoreControllo.Split(';') : null;

                        var listaFontiDatiAnf = (root.Elements("FontiDatiANF").FirstOrDefault() != null) ? root.Elements("FontiDatiANF").FirstOrDefault().Elements("FonteDatiANF") : null;
                        string codiceFiscaleRichiedente = (root.Elements("ParametriRichiesta").FirstOrDefault() != null) ? (root.Elements("ParametriRichiesta").FirstOrDefault().Elements("CodiceFiscale").FirstOrDefault() != null) ? root.Elements("ParametriRichiesta").FirstOrDefault().Elements("CodiceFiscale").FirstOrDefault().Value : null : null;
                        string dataRichiesta = (root.Elements("DataRichiesta").FirstOrDefault() != null) ? root.Elements("DataRichiesta").FirstOrDefault().Value : null;
                        var listaFonti = (root.Elements("DomandeANF").FirstOrDefault() != null) ? root.Elements("DomandeANF").FirstOrDefault().Elements("Fonte") : null;
                        consultazioneAnf = new ConsultazioneUnificataANF();
                        consultazioneAnf.codiceFiscaleRichiedente = codiceFiscaleRichiedente;
                        consultazioneAnf.dataRichiestaRichiedente = dataRichiesta;

                        if (!String.IsNullOrEmpty(codiceFiscaleRichiedente) && !String.IsNullOrEmpty(dataRichiesta))
                        {
                            if (listaFonti != null && listaFonti.Count() > 0)
                            {
                                foreach (var fonte in listaFonti)
                                {
                                    string codiceFonte = "";
                                    if (fonte.Elements("CodiceFonteDatiANF").FirstOrDefault() != null)
                                        codiceFonte = fonte.Elements("CodiceFonteDatiANF").FirstOrDefault().Value;

                                    if (!String.IsNullOrEmpty(codiceFonte) && (listaFontiNonAmmissibili == null || listaFontiNonAmmissibili.Length == 0 || !listaFontiNonAmmissibili.Contains(codiceFonte.Trim())))
                                    {
                                        string descrizioneFonte = String.Empty;
                                        if (listaFontiDatiAnf != null && listaFontiDatiAnf.Count() > 0)
                                        {
                                            foreach (var fontiAnf in listaFontiDatiAnf)
                                            {
                                                if (fontiAnf.Element("CodiceFonteDatiANF") != null && fontiAnf.Element("CodiceFonteDatiANF").Value == codiceFonte.Trim())
                                                {
                                                    descrizioneFonte = (fontiAnf.Element("DescrizioneFonteDatiANF") != null) ? fontiAnf.Element("DescrizioneFonteDatiANF").Value : null;
                                                    break;
                                                }
                                            }
                                        }
                                        var listaDomandeAnfFonte = fonte.Elements("DomandaANF");

                                        if (listaDomandeAnfFonte != null && listaDomandeAnfFonte.Count() > 0)
                                        {
                                            foreach (var domandaAnfFonte in listaDomandeAnfFonte)
                                            {
                                                var listaDatiDomandaAnf = domandaAnfFonte.Elements("DatiDomanda");

                                                if (listaDatiDomandaAnf != null && listaDatiDomandaAnf.Count() > 0)
                                                {
                                                    foreach (var datiDomandaAnf in listaDatiDomandaAnf)
                                                    {
                                                        if (datiDomandaAnf.Elements("IdentificativoDomandaPratica").FirstOrDefault() != null)
                                                        {
                                                            DomandaAnf datiAnf = new DomandaAnf();
                                                            var identificativoDomanda = datiDomandaAnf.Elements("IdentificativoDomandaPratica").FirstOrDefault();
                                                            datiAnf.codiceFonte = codiceFonte;
                                                            datiAnf.descrizioneFonte = descrizioneFonte;
                                                            datiAnf.codicePratica1 = (identificativoDomanda.Elements("CodicePratica1").FirstOrDefault() != null) ? identificativoDomanda.Elements("CodicePratica1").FirstOrDefault().Value : null;
                                                            datiAnf.codicePratica2 = (identificativoDomanda.Elements("CodicePratica2").FirstOrDefault() != null) ? identificativoDomanda.Elements("CodicePratica2").FirstOrDefault().Value : null;
                                                            datiAnf.numeroProtocolloDomanda = (identificativoDomanda.Elements("NumeroProtocolloDomanda").FirstOrDefault() != null) ? identificativoDomanda.Elements("NumeroProtocolloDomanda").FirstOrDefault().Value : null;
                                                            if (datiDomandaAnf.Elements("StatoDomanda").FirstOrDefault() != null && datiDomandaAnf.Elements("StatoDomanda").FirstOrDefault().Elements("StatoDomandaUnico").FirstOrDefault() != null)
                                                            {
                                                                int statoDomanda = 0;
                                                                Int32.TryParse(datiDomandaAnf.Elements("StatoDomanda").FirstOrDefault().Elements("StatoDomandaUnico").FirstOrDefault().Value, out statoDomanda);
                                                                datiAnf.statoDomanda = statoDomanda;
                                                            }
                                                            if (datiDomandaAnf.Elements("PeriodiANF").FirstOrDefault() != null && datiDomandaAnf.Elements("PeriodiANF").FirstOrDefault().Elements("PeriodoANF").FirstOrDefault() != null)
                                                            {
                                                                if (datiDomandaAnf.Elements("PeriodiANF").FirstOrDefault().Elements("PeriodoANF").FirstOrDefault().Elements("DataDa").FirstOrDefault() != null)
                                                                    datiAnf.periodoDataDa = datiDomandaAnf.Elements("PeriodiANF").FirstOrDefault().Elements("PeriodoANF").FirstOrDefault().Elements("DataDa").FirstOrDefault().Value;
                                                                if (datiDomandaAnf.Elements("PeriodiANF").FirstOrDefault().Elements("PeriodoANF").FirstOrDefault().Elements("DataA").FirstOrDefault() != null)
                                                                    datiAnf.periodoDataA = datiDomandaAnf.Elements("PeriodiANF").FirstOrDefault().Elements("PeriodoANF").FirstOrDefault().Elements("DataA").FirstOrDefault().Value;
                                                            }

                                                            var listaBeneficiari = (datiDomandaAnf.Elements("Beneficiari").FirstOrDefault() != null) ? datiDomandaAnf.Elements("Beneficiari").FirstOrDefault().Elements("Beneficiario") : null;
                                                            if (listaBeneficiari != null && listaBeneficiari.Count() > 0)
                                                            {
                                                                foreach (var beneficiarioDomandaAnf in listaBeneficiari)
                                                                {
                                                                    //Il beneficiario di interesse è quello avente il Codice Fiscale uguale a chi ha effettuato la richiesta
                                                                    string codiceFiscaleBeneficiario = "";
                                                                    if (beneficiarioDomandaAnf.Elements("CodiceFiscale").FirstOrDefault() != null)
                                                                        codiceFiscaleBeneficiario = beneficiarioDomandaAnf.Elements("CodiceFiscale").FirstOrDefault().Value;

                                                                    if (!String.IsNullOrEmpty(codiceFiscaleBeneficiario) && codiceFiscaleBeneficiario.Equals(codiceFiscaleRichiedente))
                                                                    {
                                                                        if (beneficiarioDomandaAnf.Elements("Respinto").FirstOrDefault() != null)
                                                                            datiAnf.respinto = beneficiarioDomandaAnf.Elements("Respinto").FirstOrDefault().Value;

                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            consultazioneAnf.listaDatiDomandaAnf.Add(datiAnf);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }
                            }
                        }

                        if (consultazioneAnf != null)
                        {
                            //Le domande devono avere necessariamente il CodicePratica1 e il NumeroProtocolloDomanda
                            List<DomandaAnf> listaDomande = consultazioneAnf.listaDatiDomandaAnf;
                            if (listaDomande != null && listaDomande.Count > 0)
                            {
                                listaDomande.RemoveAll(x => String.IsNullOrEmpty(x.codicePratica1) || String.IsNullOrEmpty(x.numeroProtocolloDomanda));
                                if (listaDomande == null || listaDomande.Count() == 0)
                                    consultazioneAnf = null;
                            }
                            else
                                consultazioneAnf = null;
                        }

                    }
                }
            }

            return true;
        }
        #endregion Richiesta Domande ANF

        #region CodMaggFamiliari
        public static void SalvaCodMaggiorazioneFamiliari(long idPensione, List<CodMaggFamiliari> listaCodMaggFamiliari)
        {
            if (listaCodMaggFamiliari != null && listaCodMaggFamiliari.Count > 0)
            {
                List<DataCommon.CodMaggiorazioneFamiliari> listaCodMaggFamiliariDB = new List<DataCommon.CodMaggiorazioneFamiliari>();

                foreach (CodMaggFamiliari CodMaggFamiliare in listaCodMaggFamiliari)
                {
                    DataCommon.CodMaggiorazioneFamiliari CodMaggFamiliareDB = new DataCommon.CodMaggiorazioneFamiliari();

                    Utility.ValorizzaOggetti(CodMaggFamiliare, CodMaggFamiliareDB);

                    listaCodMaggFamiliariDB.Add(CodMaggFamiliareDB);
                }
                if (listaCodMaggFamiliariDB.Count > 0)
                    DAGestioneFamiliari.SalvaCodMaggiorazioneFamiliari(idPensione, listaCodMaggFamiliariDB);
                else
                    DAGestioneFamiliari.DeleteAllCodMaggiorazioneFamiliariByIdPensione(idPensione);
            }
            else
            {
                DAGestioneFamiliari.DeleteAllCodMaggiorazioneFamiliariByIdPensione(idPensione);
            }
        }

        public static void GetCodMaggiorazioneFamiliariByIdPensione(long idPensione, out List<CodMaggFamiliari> listaCodMaggFamiliari)
        {
            listaCodMaggFamiliari = new List<CodMaggFamiliari>();

            List<DataCommon.CodMaggiorazioneFamiliari> listaCodMaggFamiliariDB;
            DAGestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(idPensione, out listaCodMaggFamiliariDB);
            if (listaCodMaggFamiliariDB != null && listaCodMaggFamiliariDB.Count > 0)
            {
                foreach (DataCommon.CodMaggiorazioneFamiliari CodMaggFamiliariDB in listaCodMaggFamiliariDB)
                {
                    CodMaggFamiliari CodMaggFamiliari = new CodMaggFamiliari();
                    Utility.ValorizzaOggetti(CodMaggFamiliariDB, CodMaggFamiliari);
                    listaCodMaggFamiliari.Add(CodMaggFamiliari);
                }
            }
        }

        public static void EliminaCodMaggiorazioneFamiliari(long idCodMaggiorazioneFamiliari)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFamiliari.DeleteCodMaggiorazioneFamiliari(idCodMaggiorazioneFamiliari);
                transactionScope.Complete();
            }
        }

        public static void EliminaAllCodMaggiorazioneFamiliari(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFamiliari.DeleteAllCodMaggiorazioneFamiliariByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaCodMaggiorazioneFamiliariPerFamiliare(long idAnagrafica, long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFamiliari.DeleteCodMaggiorazioneFamiliariPerFamiliare(idAnagrafica, idPensione);
                transactionScope.Complete();
            }
        }
        #endregion CodMaggFamiliari

        #region nested classes
        public class Familiare
        {

            #region private properties

            private long _IdAnagrafica;

            private long _IdPensione;

            private System.Nullable<char> _TipoComponente;

            private System.Nullable<char> _SiglaFamiliare;

            private System.Nullable<System.DateTime> _ScadenzaRevisioneSanitaria;

            private System.Nullable<long> _CodiceDetrazioni;

            private System.Nullable<char> _ValidazioneCF;

            private System.Nullable<bool> _FlagTitolare;

            private System.String _CodiceFiscale;

            private System.String _numerodomanda;

            private System.Nullable<System.DateTime> _DataMorte;

            private System.Nullable<char> _Provenienza;

            private bool _Confermato;

            private char? _Progressivo;

            private string _TipoUnione;

            #endregion private properties

            #region public properties

            public long IdAnagrafica { get { return _IdAnagrafica; } set { _IdAnagrafica = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public System.Nullable<char> TipoComponente { get { return _TipoComponente; } set { _TipoComponente = value; } }

            public System.Nullable<char> SiglaFamiliare { get { return _SiglaFamiliare; } set { _SiglaFamiliare = value; } }

            public System.Nullable<System.DateTime> ScadenzaRevisioneSanitaria { get { return _ScadenzaRevisioneSanitaria; } set { _ScadenzaRevisioneSanitaria = value; } }

            public System.Nullable<long> CodiceDetrazioni { get { return _CodiceDetrazioni; } set { _CodiceDetrazioni = value; } }

            public System.Nullable<char> ValidazioneCF { get { return _ValidazioneCF; } set { _ValidazioneCF = value; } }

            public System.String CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }

            public System.String numerodomanda { get { return _numerodomanda; } set { _numerodomanda = value; } }

            public System.Nullable<bool> FlagTitolare { get { return _FlagTitolare; } set { _FlagTitolare = value; } }

            public System.Nullable<System.DateTime> DataMorte { get { return _DataMorte; } set { _DataMorte = value; } }

            public System.Nullable<char> Provenienza { get { return _Provenienza; } set { _Provenienza = value; } }

            public bool Confermato { get { return _Confermato; } set { _Confermato = value; } }

            public char? Progressivo { get { return _Progressivo; } set { _Progressivo = value; } }

            public string TipoUnione { get { return this._TipoUnione ?? string.Empty; } set { _TipoUnione = value ?? string.Empty; } }

            public bool IsDetrazioniObbligatorieContitolare { get; set; }
            #endregion public properties

            #region public methods
            public bool IsConiugeOrUnitoCivile()
            {
                if (this._SiglaFamiliare.HasValue && this._SiglaFamiliare.Value == 'C')
                    return true;

                return false;
            }

            public bool IsConiuge()
            {
                if (this._SiglaFamiliare.HasValue && this._SiglaFamiliare.Value == 'C' && this._TipoUnione == "M")
                    return true;

                return false;
            }

            public bool IsUnitoCivile()
            {
                if (this._SiglaFamiliare.HasValue && this._SiglaFamiliare.Value == 'C' && this._TipoUnione == "U")
                    return true;

                return false;
            }

            public bool IsExConiugeOrScioltoDallUnione()
            {
                if (this._SiglaFamiliare.HasValue && this._SiglaFamiliare.Value == 'R')
                    return true;

                return false;
            }

            public bool IsScioltoDallUnione()
            {
                if (this._SiglaFamiliare.HasValue && this._SiglaFamiliare.Value == 'R' && this._TipoUnione == "U")
                    return true;

                return false;
            }

            public bool IsExConiuge()
            {
                if (this._SiglaFamiliare.HasValue && this._SiglaFamiliare.Value == 'R' && string.IsNullOrEmpty(this._TipoUnione))
                    return true;

                return false;
            }

            public bool IsAscendenteOrGenitore()
            {
                if (this._SiglaFamiliare.HasValue && this._SiglaFamiliare.Value == 'A')
                    return true;

                return false;
            }

            #endregion public methods
        }

        public class CodMaggFamiliari
        {
            #region private properties
            private long _Id;

            private long _IdPensione;

            private long _IdAnagrafica;

            private System.Nullable<byte> _CodiceMaggiorazione;

            private System.Nullable<System.DateTime> _Decorrenza;

            private System.Nullable<System.DateTime> _Cessazione;

            private System.Nullable<char> _SiglaFamiliare;

            private string _TipoUnione;

            private string _DirittoAF;

            private string _QuotaAF;

            private string _ContitolaritaFondo;

            private string _ContitolaritaAgo;

            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public long IdAnagrafica { get { return _IdAnagrafica; } set { _IdAnagrafica = value; } }

            public System.Nullable<byte> CodiceMaggiorazione { get { return _CodiceMaggiorazione; } set { _CodiceMaggiorazione = value; } }

            public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }

            public System.Nullable<System.DateTime> Cessazione { get { return _Cessazione; } set { _Cessazione = value; } }

            public System.Nullable<char> SiglaFamiliare { get { return _SiglaFamiliare; } set { _SiglaFamiliare = value; } }

            public string TipoUnione { get { return this._TipoUnione ?? string.Empty; } set { _TipoUnione = value ?? string.Empty; } }

            public string DirittoAF { get { return this._DirittoAF ?? string.Empty; } set { _DirittoAF = value ?? string.Empty; } }

            public string QuotaAF { get { return this._QuotaAF ?? string.Empty; } set { _QuotaAF = value ?? string.Empty; } }

            public string ContitolaritaFondo { get { return this._ContitolaritaFondo ?? string.Empty; } set { _ContitolaritaFondo = value ?? string.Empty; } }

            public string ContitolaritaAgo { get { return this._ContitolaritaAgo ?? string.Empty; } set { _ContitolaritaAgo = value ?? string.Empty; } }
            #endregion public properties
        }

        public class FamiliareRecuperato
        {
            private string _TipoUnione;

            public char? TipoComponente { get; set; }
            public char? SiglaFamiliare { get; set; }
            public string CodiceFiscale { get; set; }
            public string TipoUnione { get { return this._TipoUnione ?? string.Empty; } set { _TipoUnione = value ?? string.Empty; } }

            public FamiliareRecuperato() { }

            public FamiliareRecuperato(string codiceFiscale, char? siglaFamiliare, char? tipoComponente, string tipoUnione)
            {
                CodiceFiscale = codiceFiscale;
                SiglaFamiliare = siglaFamiliare;
                TipoComponente = tipoComponente;
                TipoUnione = tipoUnione;
            }
        }

        public class DatiRichiestaRicercaDomandeANF
        {
            public long Id;
            public long IdPensione;
            public long IdAnagrafica;
            public string Guid;
            public DateTime DataRichiesta;
            public string CodiceFiscale;
        }


        public class DomandaAnf
        {
            public string codiceFonte { get; set; }
            public string descrizioneFonte { get; set; }
            public string codicePratica1 { get; set; }
            public string codicePratica2 { get; set; }
            public string numeroProtocolloDomanda { get; set; }
            public int? statoDomanda { get; set; }
            public string respinto { get; set; }
            public string periodoDataDa { get; set; }
            public string periodoDataA { get; set; }
        }

        public class ConsultazioneUnificataANF
        {
            public ConsultazioneUnificataANF()
            {
                listaDatiDomandaAnf = new List<DomandaAnf>();
            }
            public string codiceFiscaleRichiedente { get; set; }
            public string dataRichiestaRichiedente { get; set; }
            public List<DomandaAnf> listaDatiDomandaAnf { get; set; }
        }


        #endregion nested classes
    }
}
