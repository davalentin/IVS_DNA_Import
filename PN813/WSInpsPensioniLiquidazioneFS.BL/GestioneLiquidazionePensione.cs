using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneFs.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneLiquidazionePensione
    {
        #region public members

        #region Dati Generici
        public static bool ControlDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, Entity.DatiGenerici datiGenerici, Entity.DatiAssicurativi datiAssicurativi, List<Entity.RecordFondo> listaRecordFondo,
            Entity.DatiDL407 datiDL407, Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici, Entity.DatiPrivilegiate datiPrivilegiate,
            Entity.DatiArticolo2 datiArticolo2, GestionePensione.DatiEliminazione datiEliminazione, out string messaggioVideo)
        {
            messaggioVideo = "";
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            // Se la get di questi dati viene effettuata più volte in questo flusso, andrà considerata l'opzione di recuperarli sul svc
            #region Get Data
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = contenitore.DatiAnagraficiDanteCausa;
            GestioneFondo.DatiFondo datiFondo = contenitore.DatiFondo;
            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiAssicurativi != null && datiAssicurativi.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiAssicurativi.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            char? derogaTraduzioneSuGP = null;
            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = contenitoreDecodifica.ElencoCodiceParticolare;
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }
            #endregion Get Data

            #region Controlli obbligatorietà o primitivi

            if (datiGenerici == null)
                return true;

            if (datiPensione == null)
            {
                messaggioVideo = "Dati pensione non presenti";
                return false;
            }

            if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
            {
                messaggioVideo = "Campo 'Codici Natura' obbligatorio";
                return false;
            }

            if (!datiGenerici.CodiceArretrati.HasValue)
            {
                messaggioVideo = "Campo 'Codice Arretrati' obbligatorio";
                return false;
            }

            if (datiGenerici.DataCompletezza == null)
            {
                messaggioVideo = "Campo 'Data Completezza' obbligatorio";
                return false;
            }

            if (datiGenerici.DataCompletezza.Value.Date > Utility.DataSistemaFs.Date)
            {
                messaggioVideo = "Il campo 'Data Completezza' non deve superare la data odierna.";
                return false;
            }

            if ((Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !isRiapertura) &&
                datiGenerici.DataCompletezza.Value.Date < datiPensione.DataPresentazioneDomanda)
            {
                messaggioVideo = "Il campo 'Data Completezza' deve superare la data di presentazione della domanda.";
                return false;
            }

            if (datiGenerici.DataInteressiLegali.HasValue && datiGenerici.DataInteressiLegali.Value.Date < datiGenerici.DataCompletezza.Value.Date.AddDays(121))
            {
                messaggioVideo = "Il campo 'Data Interessi Legali' deve superare la 'Data Completezza' di almeno 120 giorni";
                return false;
            }

            if (!GestioneControlli.ControlsProvvisoriaPerRiapertura(ref contenitoreDecodifica, isRiapertura, datiGenerici.CodiceComunicazioneCampo3, out messaggioVideo))
                return false;

            #endregion Controlli obbligatorietà o primitivi

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.GAS:
                    case Utility.TipoFondo.DZ:
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.PM:

                        if (datiGenerici.TipoCalcolo == null)
                        {
                            messaggioVideo = "Campo 'Tipo Calcolo' obbligatorio";
                            return false;
                        }

                        if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiGenerici.ExCombattente, datiGenerici.Benefici, datiGenerici.ChkDL407, datiGenerici.Privilegiate, datiGenerici.Articolo2,
                            false, tipoFondo, datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, isDomandaConNuovaGestioneDatiFondoFSPT, out messaggioVideo))
                            return false;

                        if ((!(tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)) &&
                            codiceSpecificoTraduzioneSuGP.HasValue && !GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, datiGenerici.NaturaPensione)) &&
                            (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || Utility.IsRicostituzione_Supplemento(datiPensione)))
                        {
                            messaggioVideo = "Eliminare i dati Supplementi prima di procedere con il salvataggio";
                            return false;
                        }

                        if (!ControlsDatiGenericiForDatiContributivi(ref contenitore, ref contenitoreDecodifica, datiGenerici.TipoCalcolo, false, tipoFondo))
                        {
                            messaggioVideo = "I dati calcolo salvati differiscono dal tipo di calcolo selezionato; effettuare una nuova scelta o cancellare i dati calcolo.";
                            return false;
                        }

                        if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcolo(datiPensione.DecorrenzaOriginaria, datiAssicurativi.FineAssicurazione, Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS), datiPensione.Gruppo, datiPensione.Prodotto, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.ControlsNaturaPensioneWithTrasformazioneAOI(datiPensione, datiGenerici.NaturaPensione, out messaggioVideo))
                            return false;

                        break;
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:

                        if (datiGenerici.TipoCalcolo == null)
                        {
                            messaggioVideo = "Campo 'Tipo Calcolo' obbligatorio";
                            return false;
                        }

                        if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiGenerici.ExCombattente, datiGenerici.Benefici, datiGenerici.ChkDL407, datiGenerici.Privilegiate, datiGenerici.Articolo2,
                            false, tipoFondo, datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, isDomandaConNuovaGestioneDatiFondoFSPT, out messaggioVideo))
                            return false;

                        if (codiceSpecificoTraduzioneSuGP.HasValue && !GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, datiGenerici.NaturaPensione))
                        {
                            messaggioVideo = "Eliminare i dati Supplementi prima di procedere con il salvataggio";
                            return false;
                        }

                        if (!ControlsDatiGenericiForDatiContributiviFS_PT(ref contenitore, ref contenitoreDecodifica, datiGenerici.TipoCalcolo, false) &&
                            !(Utility.IsRicostituzione_MotiviContributivi(datiPensione) || (Utility.IsRicostituzione_Reddituale(datiPensione) && datiPensione.Tipo == "0101") ||
                             Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione)))
                        {
                            messaggioVideo = "I dati calcolo salvati differiscono dal tipo di calcolo selezionato; effettuare una nuova scelta o cancellare i dati calcolo.";
                            return false;
                        }

                        break;

                    case Utility.TipoFondo.CL:
                        if (datiGenerici.TipoCalcolo == null)
                        {
                            messaggioVideo = "Campo 'Tipo Calcolo' obbligatorio";
                            return false;
                        }
                        break;
                    case Utility.TipoFondo.PI:
                        Utility.CategoriaFondoPI? categoriaPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                        if (categoriaPI == Utility.CategoriaFondoPI.U || categoriaPI == Utility.CategoriaFondoPI.V)
                        {
                            if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiGenerici.ExCombattente, datiGenerici.Benefici, datiGenerici.ChkDL407, datiGenerici.Privilegiate, datiGenerici.Articolo2,
                                false, tipoFondo, datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, isDomandaConNuovaGestioneDatiFondoFSPT, out messaggioVideo))
                                return false;
                        }
                        break;
                }
            }

            if (!isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                if (listaRecordFondo != null && listaRecordFondo.Count > 0)
                {
                    string messaggioApp = "record fondo avente decorrenza uguale alla decorrenza originaria,";
                    Entity.RecordFondo recordFondo = null;
                    //if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && listaRecordFondo.Exists(x => x.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria))
                    //{
                    //    recordFondo = listaRecordFondo.FirstOrDefault(x => x.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria);
                    //    //ENG - Linea FS (no GDP, FS, PT) Se nella lista dei record fondo E’ PRESENTE un record con decorrenza uguale a quella della pensione, il controllo viene effettuato sull’ultimo record fondo
                    //    if (!Utility.IsDomandaINPDAP(datiPensione.Gestione) && tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.FS && tipoFondo.Value != Utility.TipoFondo.PT)
                    //    {
                    //        messaggioApp = "record fondo ultimo,";
                    //        recordFondo = listaRecordFondo.LastOrDefault();
                    //    }
                    //}
                    //else if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && !listaRecordFondo.Exists(x => x.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria))
                    //{
                    //    messaggioApp = "record fondo ultimo,";
                    //    recordFondo = listaRecordFondo.LastOrDefault();
                    //}
                    //else
                    //{
                    //    messaggioApp = "primo record fondo,";
                    //    recordFondo = listaRecordFondo[0];
                    //}

                    // ACC segnalazione Controllo di coerenza del Codice Natura (Fondi Speciali), documento "SF_IVS_FS - Interventi Puntuali@20250912_v7.5.docx", paragrafo 8.42
                    messaggioApp = "record fondo ultimo,";
                    recordFondo = listaRecordFondo.LastOrDefault();

                    if (recordFondo.CodiceNatura1.HasValue || recordFondo.CodiceNatura2.HasValue || recordFondo.CodiceNatura3.HasValue)
                    {
                        if ((recordFondo.CodiceNatura1.HasValue && recordFondo.CodiceNatura1.Value != char.Parse(datiGenerici.NaturaPensione.Substring(0, 1))) ||
                            (recordFondo.CodiceNatura2.HasValue && recordFondo.CodiceNatura2.Value != char.Parse(datiGenerici.NaturaPensione.Substring(1, 1))) ||
                            (recordFondo.CodiceNatura3.HasValue && recordFondo.CodiceNatura3.Value != char.Parse(datiGenerici.NaturaPensione.Substring(2, 1))))
                        {
                            messaggioVideo = string.Format("I Codici Natura devono coincidere con quelli del {0} indicato nel tab 'Dati Assicurativi'", messaggioApp);
                            return false;
                        }
                    }
                }
            }

            if (!GestioneCrossControls.ALL_VerificaIncongruenzaEsenzioneFiscaleToDB(datiPensione, datiAnagrafici.CodiceComuneResidenza, datiDetrazioni, isRiapertura, datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodNaturaSperDonna(datiPensione, datiGenerici.NaturaPensione, datiGenerici.TipoCalcolo, datiAnagrafici.Sesso, datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : null,
                out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodNaturaContrib(datiPensione, datiGenerici.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaCodiceArretratiWithEliminazione(datiGenerici.CodiceMotivo, datiGenerici.CodiceArretrati, datiPensione, out messaggioVideo))
                return false;

            if (!ControlDatiGenericiWithFondi(ref contenitore, tipoFondo, datiPensione, datiIstruttoria, datiFondo, datiGenerici, datiAssicurativi, datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP,
                isDomandaConNuovaGestioneDatiFondoFSPT, derogaTraduzioneSuGP, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(datiPensione, datiGenerici.NaturaPensione, datiAssicurativi != null ? datiAssicurativi.InizioAssicurazione : null, out messaggioVideo))
                return false;

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiRIC(datiGenerici.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, datiGenerici.CausaCarico, datiPensione.DataInizioCalcolo, out messaggioVideo))
                    return false;
            }
            else
            {

                if (Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione))
                {
                    if (Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault() && !datiGenerici.DecorrenzaCalcoloArretrati.HasValue)
                    {
                        messaggioVideo = "La data 'Decorrenza Arretrati' è obbligatoria";
                        return false;
                    }

                    if (!GestioneControlli.ControlsDecorrenzaArretratiStorico(datiGenerici.DecorrenzaCalcoloArretrati, datiStoricoGP != null ? datiStoricoGP.DataEliminazioneContabile : null, out messaggioVideo))
                        return false;

                }
                else
                {
                    if (!GestioneControlli.ControlsDecorrenzaArretratiPL(datiGenerici.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                        return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaBeneficiPerOpzioneTipoContributivo(datiPensione, datiGenerici.Benefici, out messaggioVideo))
                return false;

            if (Utility.IsBonusBooking(datiPensione) && datiPensione.Tipo != "0167" && datiGenerici.IsRichiestaBonus.GetValueOrDefault())
            {
                if (!GestioneCrossControls.ALL_VerificaAnnoRichiestaBonus154(datiPensione, datiGenerici.AnnoDecorrenzaBonus, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.ControlsTrattenutaINPDAP(datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap, datiEliminazione != null ? datiEliminazione.DecorrenzaEliminazione : null, datiPensione,
               datiStoricoGP != null ? datiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                if (!GestioneCrossControls.ALL_CodNaturOrganizzazioniInternazionali(datiPensione, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;
            }

            return true;
        }

        public static bool ControlDatiGenericiINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, bool IsSingleTab, Entity.DatiGenericiINPDAP datiGenerici, DatiAssicurativiINPDAP datiAssicurativi,
            List<RipartizioneINPDAP> listaRipartizioneINPDAP, GestioneFondo.DatiFondo datiFondo, List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, DatiExCombattente datiExCombattente, DatiBenefici datiBenefici, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, GestionePensione.DatiEliminazione datiEliminazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;

            // Se la get di questi dati viene effettuata più volte in questo flusso, andrà considerata l'opzione di recuperarli sul svc
            #region Get Data

            if (IsSingleTab)
            {
                GetDatiAssicurativiINPDAP(ref contenitore, datiPensione, datiFondo, listaDatiPensioneINPDAP != null ? listaDatiPensioneINPDAP.FirstOrDefault() : null, out datiAssicurativi, out listaRipartizioneINPDAP);
            }
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = contenitore.DatiAnagraficiDanteCausa;

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiAssicurativi != null && datiAssicurativi.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiAssicurativi.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            char? derogaTraduzioneSuGP = null;
            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = contenitoreDecodifica.ElencoCodiceParticolare;
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }


            #endregion Get Data

            #region Controlli obbligatorietà o primitivi

            if (datiGenerici == null)
                return true;

            if (datiPensione == null)
            {
                messaggioVideo = "Dati pensione non presenti";
                return false;
            }

            if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
            {
                messaggioVideo = "Campo 'Codici Natura' obbligatorio";
                return false;
            }

            if (!datiGenerici.CodiceArretrati.HasValue)
            {
                messaggioVideo = "Campo 'Codice Arretrati' obbligatorio";
                return false;
            }

            if (datiGenerici.DataCompletezza == null)
            {
                messaggioVideo = "Campo 'Data Completezza' obbligatorio";
                return false;
            }

            if (datiGenerici.DataCompletezza.Value.Date > Utility.DataSistemaFs.Date)
            {
                messaggioVideo = "Il campo 'Data Completezza' non deve superare la data odierna.";
                return false;
            }

            if ((Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !isRiapertura) &&
                datiGenerici.DataCompletezza.Value.Date < datiPensione.DataPresentazioneDomanda)
            {
                messaggioVideo = "Il campo 'Data Completezza' deve superare la data di presentazione della domanda.";
                return false;
            }

            if (datiGenerici.DataInteressiLegali.HasValue && datiGenerici.DataInteressiLegali.Value.Date < datiGenerici.DataCompletezza.Value.Date.AddDays(31))
            {
                messaggioVideo = "Il campo 'Data Interessi Legali' deve superare la 'Data Completezza' di almeno 30 giorni";
                return false;
            }

            if (!GestioneControlli.ControlsProvvisoriaPerRiapertura(ref contenitoreDecodifica, isRiapertura, datiGenerici.CodiceComunicazioneCampo3, out messaggioVideo))
                return false;

            #endregion Controlli obbligatorietà o primitivi

            if (datiGenerici.TipoCalcolo == null)
            {
                messaggioVideo = "Campo 'Tipo Calcolo' obbligatorio";
                return false;
            }

            if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiGenerici.ExCombattente, datiGenerici.Benefici, null, null, null, false, Utility.TipoFondo.FS, null, datiExCombattente,
                datiBenefici, null, null, true, out messaggioVideo))
                return false;

            if (codiceSpecificoTraduzioneSuGP.HasValue && !GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, datiGenerici.NaturaPensione))
            {
                messaggioVideo = "Eliminare i dati Supplementi prima di procedere con il salvataggio";
                return false;
            }

            if (!ControlsDatiGenericiForDatiContributiviINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione.Id, datiGenerici.TipoCalcolo, false) && !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
            {
                messaggioVideo = "I dati calcolo salvati differiscono dal tipo di calcolo selezionato; effettuare una nuova scelta o cancellare i dati calcolo.";
                return false;
            }

            if (!GestioneCrossControls.ALL_VerificaIncongruenzaEsenzioneFiscaleToDB(datiPensione, datiAnagraficiTitolare.CodiceComuneResidenza, datiDetrazioni, isRiapertura,
                datiGenerici.CodiceComunicazioneCampo4, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodNaturaSperDonna(datiPensione, datiGenerici.NaturaPensione, datiGenerici.TipoCalcolo, datiAnagraficiTitolare.Sesso,
                datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : null, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaCodiceArretratiWithEliminazione(datiGenerici.CodiceMotivo, datiGenerici.CodiceArretrati, datiPensione, out messaggioVideo))
                return false;

            if (!ControlsDatiGenericiBonusINPDAP(datiGenerici, datiPensione, out messaggioVideo))
                return false;

            if (!ControlsDatiGenericiEliminazioneContestualeINPDAP(datiGenerici, datiPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDecorrenzaArretratiINPDAP(datiGenerici, datiPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsTrattenutaINPDAP(datiGenerici.TrattenutaInpdap, datiGenerici.DataRinunciaTrattenutaInpdap, datiEliminazione != null ? datiEliminazione.DecorrenzaEliminazione : null, datiPensione,
                 datiStoricoGP != null ? datiStoricoGP.DataRinunciaTrattenutaInpdap : null, out messaggioVideo))
                return false;

            List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = new List<GestioneFondo.DatiFondoFST>();
            GestioneFondo.DatiFondoFST datiFondoFST = new GestioneFondo.DatiFondoFST();
            Utility.ValorizzaOggetti(datiGenerici, datiFondoFST);
            listaDatiFondoFST.Add(datiFondoFST);
            object datiFondoXX = listaDatiFondoFST;

            if (!GestioneControlli.VerificaRequisitiNoInvalidita(Utility.TipoFondo.FS, listaDatiFondoFST, datiPensione, null, datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null,
                codiceSpecificoTraduzioneSuGP, derogaTraduzioneSuGP, datiAnagraficiTitolare.DataNascita, datiAnagraficiTitolare.Sesso, false, out messaggioVideo))
                return false;

            GestioneCrossControls.TipoDecPensione? tipoDecPensione = GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo);
            if (tipoDecPensione.HasValue)
            {
                if (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia)
                {
                    //mail 03-04-2013: bypass controlli per L214 e usuranti per il solo prodotto 0002
                    //mail 28-11-2013: bypass controlli per L.228 RE: Reeng Pensioni - Salvaguardia L.228 - Punti aperti
                    //mail 16-07-2014: bypass controlli per L.124 art.11 bis RE: ReEng Pensioni - Salvaguardia L.124/2013 art.11
                    if ((Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                        Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) ||
                        Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) ||
                        Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione)) &&
                        datiPensione.Prodotto == "0002")
                        return true;

                    //mail 24-02-2014: bypass controlli per domande di ricostituzione diverse da Variazione Per Decorrenza
                    if (datiPensione.Gruppo == "0031" && !Utility.IsRicostituzione_VariazionePerDecorrenza(datiPensione))
                        return true;

                    bool? bReturn = GestioneControlli.VerificaEtaTitolareFromAnte247(datiPensione, datiAnagraficiTitolare, Utility.TipoFondo.FS, datiFondoXX, null,
                        datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, codiceSpecificoTraduzioneSuGP, out messaggioVideo);
                    if (bReturn.HasValue)
                    {
                        if (!bReturn.Value)
                            return false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(messaggioVideo))
                            messaggioVideo = "Dati obbligatori mancanti";
                        return false;
                    }

                }
            }

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
            {
                if (!GestioneControlli.ControlsDecorrenzaArretratiRIC(datiGenerici.DecorrenzaCalcoloArretrati, datiPensione.DecorrenzaOriginaria, datiGenerici.CausaCarico, datiPensione.DataInizioCalcolo, out messaggioVideo))
                    return false;
            }

            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                if (!GestioneCrossControls.ALL_CodNaturOrganizzazioniInternazionali(datiPensione, datiGenerici.NaturaPensione, out messaggioVideo))
                    return false;
            }

            return true;
        }

        public static void GetDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneFondo.DatiFondo datiFondo,
            GestioneDatiControlloFelpe.ControlloFelpe controlloFelpe, out Entity.DatiGenerici datiGenerici)
        {
            datiGenerici = null;
            if (datiPensione == null)
                return;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            datiGenerici = new Entity.DatiGenerici();
            Utility.ValorizzaOggetti(datiPensione, datiGenerici);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (!String.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta) && String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                datiGenerici.NaturaPensione = GestioneCrossControls.GetCodiceNaturaFromCodiceTipoRichiesta(datiPensione.CodiceTipoRichiesta, tipoFondo, Utility.TipoAppartenenza.FS);
            if (Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                (!Utility.IsRicostituzioneOrRiapertura(datiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsRicostituzioneOrRiapertura(datiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
            {
                if (string.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " J ";
                else
                    datiGenerici.NaturaPensione = datiGenerici.NaturaPensione.Substring(0, 1) + "J" + datiGenerici.NaturaPensione.Substring(2, 1);
            }

            if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione, true, true))
            {
                if (String.IsNullOrEmpty(datiGenerici.NaturaPensione))
                    datiGenerici.NaturaPensione = " O ";
                if (!datiGenerici.TipoCalcolo.HasValue || datiGenerici.TipoCalcolo.Value == 0)
                    datiGenerici.TipoCalcolo = 19; //contributivo
            }

            if (datiIstruttoria != null)
                Utility.ValorizzaOggetti(datiIstruttoria, datiGenerici);

            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            if (datiEliminazione != null)
                Utility.ValorizzaOggetti(datiEliminazione, datiGenerici);

            GestionePagamento.DatiPagamento datiPagamento = contenitore.DatiPagamento;
            if (datiPagamento != null)
                Utility.ValorizzaOggetti(datiPagamento, datiGenerici);

            if (datiFondo != null)
                Utility.ValorizzaOggetti(datiFondo, datiGenerici);

            GetDatiGenericiWithFondiByIdPensione(ref contenitore, tipoFondo, ref datiGenerici);

            if (controlloFelpe != null)
            {
                if (datiGenerici == null)
                    datiGenerici = new Entity.DatiGenerici();
                datiGenerici.IsProvvisoria = controlloFelpe.IsProvvisoria;
                if (datiGenerici.IsProvvisoria.HasValue && datiGenerici.IsProvvisoria.Value && !datiGenerici.CodiceComunicazioneCampo3.HasValue)
                    datiGenerici.CodiceComunicazioneCampo3 = 'P';
            }

        }

        public static void GetDatiGenericiINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneFondo.DatiFondo datiFondo,
            GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP, GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe, GestionePensione.DatiEliminazione datiEliminazione,
            out DatiGenericiINPDAP datiGenericiINPDAP)
        {
            datiGenericiINPDAP = null;

            if (datiPensione == null)
                return;

            datiGenericiINPDAP = new DatiGenericiINPDAP();

            Utility.ValorizzaOggetti(datiPensione, datiGenericiINPDAP);

            if (!String.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta) && String.IsNullOrEmpty(datiGenericiINPDAP.NaturaPensione))
                datiGenericiINPDAP.NaturaPensione = GestioneCrossControls.GetCodiceNaturaFromCodiceTipoRichiesta(datiPensione.CodiceTipoRichiesta, null, Utility.TipoAppartenenza.FS);
            if (Utility.IsDomandaSperimentaleDonna(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
            {
                if (String.IsNullOrEmpty(datiGenericiINPDAP.NaturaPensione))
                    datiGenericiINPDAP.NaturaPensione = " O ";
                else if (datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(1, 1) == " ")
                    datiGenericiINPDAP.NaturaPensione = datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(0, 1) + "O" + datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(2, 1);
                if (!datiGenericiINPDAP.TipoCalcolo.HasValue || datiGenericiINPDAP.TipoCalcolo.Value == 0)
                    datiGenericiINPDAP.TipoCalcolo = 19; //contributivo

            }
            if (Utility.IsRimpatriatiAlbania(datiPensione))
            {
                if (String.IsNullOrEmpty(datiGenericiINPDAP.NaturaPensione))
                    datiGenericiINPDAP.NaturaPensione = " H ";
                else if (datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(1, 1) == " ")
                    datiGenericiINPDAP.NaturaPensione = datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(0, 1) + "H" + datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(2, 1);

            }
            if (Utility.IsDomandaUsuranti(datiPensione))
            {
                if (String.IsNullOrEmpty(datiGenericiINPDAP.NaturaPensione))
                    datiGenericiINPDAP.NaturaPensione = "  Z";
                else if (datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) == " ")
                    datiGenericiINPDAP.NaturaPensione = datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "Z";
            }
            if (Utility.IsTelematica(datiPensione.CodiceProcedura))
            {
                if (!datiGenericiINPDAP.DataCompletezza.HasValue)
                    datiGenericiINPDAP.DataCompletezza = datiPensione.DataPresentazioneDomanda;
            }
            if (Utility.IsDomandaTrasformazioneInvalidita(datiPensione))
            {
                if (String.IsNullOrEmpty(datiGenericiINPDAP.NaturaPensione))
                    datiGenericiINPDAP.NaturaPensione = "  H";
                else if (datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(2, 1) == " ")
                    datiGenericiINPDAP.NaturaPensione = datiGenericiINPDAP.NaturaPensione.PadLeft(3, ' ').Substring(0, 2) + "H";
            }

            GestionePagamento.DatiPagamento datiPagamento = contenitore.DatiPagamento;

            Utility.ValorizzaOggetti(datiIstruttoria, datiGenericiINPDAP);
            Utility.ValorizzaOggetti(datiFondo, datiGenericiINPDAP);
            Utility.ValorizzaOggetti(datiPensioneINPDAP, datiGenericiINPDAP);
            Utility.ValorizzaOggetti(datiControlloFelpe, datiGenericiINPDAP);
            Utility.ValorizzaOggetti(datiEliminazione, datiGenericiINPDAP);
            Utility.ValorizzaOggetti(datiPagamento, datiGenericiINPDAP);


            if (datiGenericiINPDAP.IsDatiGenericiINPDAPIstruttoriaNull() && datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneNull() && datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneINPDAPNull() &&
                datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneFondoDatiGenericiNull() && datiGenericiINPDAP.IsDatiGenericiINPDAPControlloFelpeNull() &&
                datiGenericiINPDAP.IsDatiGenericiINPDAPEliminazioneNull())
                datiGenericiINPDAP = null;

            byte? causaCarico = GetCausaCaricoFromTipoDomanda(datiPensione);
            if (causaCarico.HasValue)
            {
                if (datiGenericiINPDAP == null)
                    datiGenericiINPDAP = new DatiGenericiINPDAP();
                datiGenericiINPDAP.CausaCarico = causaCarico;
            }

            if (datiGenericiINPDAP != null && datiGenericiINPDAP.IsProvvisoria.HasValue && datiGenericiINPDAP.IsProvvisoria.Value && !datiGenericiINPDAP.CodiceComunicazioneCampo3.HasValue)
                datiGenericiINPDAP.CodiceComunicazioneCampo3 = 'P';

        }

        public static void StoreDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneFondo.DatiFondo datiFondo,
            Entity.DatiGenerici datiGenerici, bool IsCancelOperation, Entity.DatiDL407 datiDL407, Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici,
            Entity.DatiPrivilegiate datiPrivilegiate, Entity.DatiArticolo2 datiArticolo2, DatiAssicurativi datiAssicurativi, List<RecordFondo> listaRecordFondo, bool isSingleTab, ref GestionePagamento.DatiPagamento datiPagamento)
        {
            if (datiGenerici == null)
                datiGenerici = new INPS.Pensioni.LiquidazioneFs.Entity.DatiGenerici();

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            if (isSingleTab)
            {
                GetDatiAssicurativi(ref contenitore, datiPensione, datiFondo, isRiaperturaDomanda, out datiAssicurativi, out listaRecordFondo);
            }

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);
            bool isAnteArmonizzazione = Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC,
                datiAssicurativi != null && datiAssicurativi.fondoTT != null ? datiAssicurativi.fondoTT.DimissioniAnte97 : null,
                codiceRequisiti2: (datiAssicurativi != null ? datiAssicurativi.CodiceRequisiti2 : null));

            List<GestioneOneri.DatiOneri> lstDatiOneri = contenitore.ListaDatiOneri;
            List<GestioneDecodifica.GruppoOneri> decGruppoOnere = null;
            List<GestioneDecodifica.SottoGruppoOneri> decSottoGruppoOneri = null;
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
            GestionePensione.DatiEliminazione datiEliminazione = contenitore.DatiEliminazione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestioneQuadri.DatiQuadroRichiestaBonus datiQuadroRichiestaBonus = contenitore.DatiQuadroRichiestaBonus;
            List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo = null;
            //ENG - memo 28_2024
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = contenitore.DatiAnagraficiTitolare;
            //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
            GestioneLavorazione.DatiLavorazione datiLavorazione = contenitore.DatiLavorazione;

            GestioneCtrlRic.ControlTabRic controlTabRic = null;
            if (isRiaperturaDomanda)
                GestioneCtrlRic.GetCtrlTabRic("0107", Utility.TipoAppartenenza.FS, out controlTabRic);
            else
                GestioneCtrlRic.GetCtrlTabRic(datiPensione.Prodotto, Utility.TipoAppartenenza.FS, out controlTabRic);

            #region Gestione visibilità tabs MaggiorazioneBenefici

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            #endregion Gestione visibilità tabs MaggiorazioneBenefici

            #region gestione TipoFondo

            GestioneFondo.DatiFondoEL datiFondoEL = null;
            GestioneFondo.DatiFondoTT datiFondoTT = null;
            GestioneFondo.DatiFondoET datiFondoET = null;
            GestioneFondo.DatiFondoVL datiFondoVL = null;
            List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = null;
            List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = null;
            GestioneFondo.DatiFondoPI datiFondoPI = null;
            GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            GestioneFondo.DatiFondoDZ datiFondoDZ = null;
            GestioneFondo.DatiFondoES datiFondoES = null;
            GestioneFondo.DatiFondoPM datiFondoPM = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        datiFondoEL = contenitore.DatiFondoEL;
                        break;
                    case Utility.TipoFondo.TT:
                        datiFondoTT = contenitore.DatiFondoTT;
                        break;
                    case Utility.TipoFondo.ET:
                        datiFondoET = contenitore.DatiFondoET;
                        break;
                    case Utility.TipoFondo.VL:
                        datiFondoVL = contenitore.DatiFondoVL;
                        break;
                    case Utility.TipoFondo.PT:
                        listaDatiFondoPT = contenitore.ListaDatiFondoPT;
                        break;
                    case Utility.TipoFondo.FS:
                        listaDatiFondoFST = contenitore.ListaDatiFondoFST;
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        datiFondoPI = contenitore.DatiFondoPI;
                        StoreDatiGenericiPI(ref contenitore, datiFondoPI, datiPensione, datiGenerici, ref datiFondo, ref datiIstruttoria, datiEliminazione, datiQuadroLiquidazionePensione, datiQuadroMaggiorazioniBenefici,
                            IsCancelOperation, controlTabRic, isRiaperturaDomanda, ref datiPagamento);
                        return;
                    case Utility.TipoFondo.GAS:
                        datiFondoGAS = contenitore.DatiFondoGAS;
                        break;
                    case Utility.TipoFondo.DZ:
                        datiFondoDZ = contenitore.DatiFondoDZ;
                        break;
                    case Utility.TipoFondo.ES:
                        datiFondoES = contenitore.DatiFondoES;
                        break;
                    case Utility.TipoFondo.PM:
                        datiFondoPM = contenitore.DatiFondoPM;
                        break;
                }
            }
            #endregion gestione TipoFondo

            #region Gestione visibilità Supplementi

            //bool IsSupplementiVisible = GestioneSupplementi.IsSupplementiVisible(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.Gruppo, datiPensione.Prodotto, datiGenerici.NaturaPensione);
            bool IsSupplementiVisible = Utility.GetVisibilitaQuadroSupplementi(datiPensione, datiGenerici.NaturaPensione, isRiaperturaDomanda, null) != Utility.TipoQuadro.NonVisibile;
            bool isSupplementiPerRIC = false;
            bool isSupplementiPerRev = false;

            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            if (IsSupplementiVisible)//se quadro è visibile
                datiQuadroSupplementi = contenitore.DatiQuadroSupplementi;
            else if (isRiaperturaDomanda || (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || Utility.IsRicostituzione_Supplemento(datiPensione) ||
                tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
            {
                List<EntityBLCommon.DatiSupplementi> LdatiSupplementi = contenitore.ListaDatiSupplementi;
                if (LdatiSupplementi != null && LdatiSupplementi.Count > 0)
                    isSupplementiPerRIC = true;
            }
            else if (Utility.IsDomandaReversibilita(datiPensione))
            {
                List<EntityBLCommon.DatiSupplementi> LdatiSupplementi = contenitore.ListaDatiSupplementi;
                if (LdatiSupplementi != null && LdatiSupplementi.Count > 0)
                    isSupplementiPerRev = true;
            }
            #endregion Gestione visibilità Supplementi

            #region Gestione Semaforica Quadro Redditi
            List<GestioneRedditi.RedditoDRedd> lstRedditi = contenitore.ListaRedditoDRedd;
            GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = contenitore.DatiQuadroRedditi;
            #endregion Gestione Semaforica Quadro Redditi

            #region Gestione Semaforica Quadro Dati Calcolo
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiCalcolo = contenitore.DatiQuadroDatiContributivi;
            #endregion Gestione Semaforica Quadro Dati Calcolo

            // Se è cambiata la visibilità oppure se adesso deve essere visualizzato ma il quadro non è visibile
            bool genericiChangedPerOneri = (Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) ^
                                            Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri)) ||
                                            (Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri) &&
                                            datiQuadroOneri != null && (datiQuadroOneri.TabOneri == null || datiQuadroOneri.Tipo == 0));

            bool recordOneriChanged = false;
            if (genericiChangedPerOneri)
            {
                decGruppoOnere = contenitoreDecodifica.ElencoDecCodeGruppoOnere;
                decSottoGruppoOneri = contenitoreDecodifica.ElencoDecCodeSottoGruppoOnere;
            }

            bool bloccoDeroga = false;
            if (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) ||
                Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) ||
                Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) ||
                Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) ||
                Utility.IsDomandaVecchPerditaTitolo(datiPensione))
                bloccoDeroga = true;

            GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni = contenitore.DatiQuadroDetrazioni;

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiAssicurativi.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiAssicurativi.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = contenitore.ListaDatiServizioUtile;
            bool isDatiServizioUtilePresenti = listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0;

            if (isDomandaConNuovaGestioneDatiFondoFSPT && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                GestioneAreaRecordFondo.GetListaRecordFondoByIdPensione(ref contenitore, out listaRecordFondo);

            if (isDomandaConNuovaGestioneDatiFondoFSPT && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && listaRecordFondo != null &&
                listaRecordFondo.Count(x => x.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria || !x.DecorrenzaValiditaDati.HasValue) > 0)
            {
                var firstRecordFondo = listaRecordFondo.First(x => x.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria || !x.DecorrenzaValiditaDati.HasValue);
                if (!string.IsNullOrEmpty(datiGenerici.NaturaPensione))
                {
                    firstRecordFondo.CodiceNatura1 = datiGenerici.NaturaPensione[0];
                    firstRecordFondo.CodiceNatura2 = datiGenerici.NaturaPensione[1];
                    firstRecordFondo.CodiceNatura3 = datiGenerici.NaturaPensione[2];
                }
                else
                    firstRecordFondo.CodiceNatura1 = firstRecordFondo.CodiceNatura2 = firstRecordFondo.CodiceNatura3 = null;
            }

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;

            bool isBeneficioVittimeUnderOver80 = Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo);
            //ENG - MEMO 50/2023
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);

            //ENG - MEMO 50/2023
            bool aggiornaSupplementi = false;
            if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.Tipo == "0001" && !Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                aggiornaSupplementi = datiPensione.TipoCalcolo != datiGenerici.TipoCalcolo;
                if (datiIstruttoria == null)
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                if (datiPensione.TipoCalcolo.HasValue)
                    datiIstruttoria.TipoCalcoloPrecedente = datiPensione.TipoCalcolo;
            }

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            //ENG - memo 28_2024
            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") &&
                    datiGenerici.TipoCalcolo.HasValue && datiGenerici.TipoCalcolo == 19 && datiGenerici != null)
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
                StoreDatiGenericiPerPensione(datiGenerici, datiPensione);
                StoreDatiGenericiPerIstruttoria(datiPensione.Id, datiGenerici, ref datiIstruttoria, datiPensione, bloccoDeroga);
                StoreDatiGenericiPerEliminazione(datiPensione.Id, datiGenerici, ref datiEliminazione);
                bool eliminaFondoDatiGenerici = false;
                StoreDatiGenericiPerFondoDatiGenerici(datiPensione.Id, datiGenerici, ref datiFondo, isDomandaConNuovaGestioneDatiFondoFSPT, out eliminaFondoDatiGenerici);
                if (isDomandaConNuovaGestioneDatiFondoFSPT || isDatiServizioUtilePresenti)
                    eliminaFondoDatiGenerici = false;

                StoreDatiGenericiPerDatiPagamento(datiPensione, datiGenerici, ref datiPagamento);

                long idFondo = 0;
                if (datiFondo != null)
                    idFondo = datiFondo.Id;

                #region gestione TipoFondo
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.EL:
                            StoreDatiGenericiPerFondoEL(datiPensione.Id, idFondo, datiGenerici, ref datiFondoEL, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.TT:
                            StoreDatiGenericiPerFondoTT(datiPensione.Id, idFondo, datiGenerici, ref datiFondoTT, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.ET:
                            StoreDatiGenericiPerFondoET(datiPensione.Id, idFondo, datiGenerici, ref datiFondoET, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.VL:
                            StoreDatiGenericiPerFondoVL(datiPensione.Id, idFondo, datiGenerici, ref datiFondoVL, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.PT:
                            StoreDatiGenericiPerFondoPT(datiPensione.Id, idFondo, datiGenerici, ref listaDatiFondoPT, isDomandaConNuovaGestioneDatiFondoFSPT, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.FS:
                            StoreDatiGenericiPerFondoFST(datiPensione.Id, idFondo, datiGenerici, ref listaDatiFondoFST, isDomandaConNuovaGestioneDatiFondoFSPT, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.GAS:
                            StoreDatiGenericiPerFondoGAS(datiPensione.Id, idFondo, datiGenerici, ref datiFondoGAS, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.DZ:
                            StoreDatiGenericiPerFondoDZ(datiPensione.Id, idFondo, datiGenerici, ref datiFondoDZ, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.ES:
                            StoreDatiGenericiPerFondoES(datiPensione.Id, idFondo, datiGenerici, ref datiFondoES, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                        case Utility.TipoFondo.PM:
                            StoreDatiGenericiPerFondoPM(datiPensione.Id, idFondo, datiGenerici, ref datiFondoPM, eliminaFondoDatiGenerici, ref datiFondo);
                            break;
                    }
                }
                #endregion gestione TipoFondo

                if (isDomandaConNuovaGestioneDatiFondoFSPT && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                    GestioneAreaRecordFondo.SalvaRecordFondo(datiPensione.Id, listaRecordFondo, out listaDatiRecordFondo);

                if (IsCancelOperation)
                    datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                else
                {
                    if (!datiGenerici.IsPensioneNull() || !datiGenerici.IsIstruttoriaNull() || !datiGenerici.IsEliminazioneNull() || !datiGenerici.IsFondoDatiGenericiNull())
                        datiQuadroLiquidazionePensione.TabDatiGenerici = 2;
                    else
                        datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                }

                #region Gestione visibilità PrecedentePensione
                if (datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value)
                {
                    if (datiQuadroLiquidazionePensione.TabPrecedentePensione == 1 && (!Utility.IsRicostituzione(datiPensione.Gruppo) || !Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione)))
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 0;
                }
                else
                {
                    StoreDatiPrecedentePensionePerIstruttoria(datiPensione.Id, new INPS.Pensioni.LiquidazioneFs.Entity.DatiPrecedentePensione(), ref datiIstruttoria);
                    datiQuadroLiquidazionePensione.TabPrecedentePensione = 1;
                }
                #endregion Gestione visibilità PrecedentePensione

                #region Gestione visibilità Supplementi
                if (tipoFondo != Utility.TipoFondo.CL && !(((ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)) || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione)) && !Utility.IsDomandaINPDAP(datiPensione.Gestione)))
                {
                    /*Natura pensione dentro datiPensione è aggiornata con i nuovi valori.*/
                    if (!IsSupplementiVisible)
                    {
                        GestioneQuadri.InizializzaQuadroSupplementi(datiPensione, Utility.TipoAppartenenza.FS, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), isSupplementiPerRIC, false, false, isSupplementiPerRev, false, controlTabRic, isRiaperturaDomanda, false, null);
                    }
                    else
                    {
                        if (datiQuadroSupplementi.Tipo == 0)
                        {
                            datiQuadroSupplementi = new GestioneQuadri.DatiQuadroSupplementi();
                            GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                        }
                    }
                }

                //ENG - MEMO 50/2023
                if (aggiornaSupplementi)
                {
                    datiQuadroSupplementi = new GestioneQuadri.DatiQuadroSupplementi();
                    datiQuadroSupplementi.TabSupplementi = 0;
                    datiQuadroSupplementi.Tipo = 2;

                    GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                }
                #endregion Gestione visibilità Supplementi

                #region Gestione visibilità tabs MaggiorazioneBenefici

                if (datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabExCombattente == null)
                        datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = null;

                if (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabBenefici == null)
                        datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabBenefici = null;

                if (datiGenerici.ChkDL407.HasValue && datiGenerici.ChkDL407.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabLegge407 == null)
                        datiQuadroMaggiorazioniBenefici.TabLegge407 = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabLegge407 = null;

                if (datiGenerici.Articolo2.HasValue && datiGenerici.Articolo2.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabArticolo2 == null)
                        datiQuadroMaggiorazioniBenefici.TabArticolo2 = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabArticolo2 = null;

                if (datiGenerici.Privilegiate.HasValue && datiGenerici.Privilegiate.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabPrivilegiate == null)
                        datiQuadroMaggiorazioniBenefici.TabPrivilegiate = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabPrivilegiate = null;

                if ((datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value && datiQuadroMaggiorazioniBenefici.TabExCombattente == 2) ||
                    (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value && datiQuadroMaggiorazioniBenefici.TabBenefici == 2) ||
                    (datiGenerici.ChkDL407.HasValue && datiGenerici.ChkDL407.Value && datiQuadroMaggiorazioniBenefici.TabLegge407 == 2) ||
                    (datiGenerici.Articolo2.HasValue && datiGenerici.Articolo2.Value && datiQuadroMaggiorazioniBenefici.TabArticolo2 == 2) ||
                    (datiGenerici.Privilegiate.HasValue && datiGenerici.Privilegiate.Value && datiQuadroMaggiorazioniBenefici.TabPrivilegiate == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;

                if (datiQuadroMaggiorazioniBenefici.TabExCombattente == 0 || datiQuadroMaggiorazioniBenefici.TabBenefici == 0 ||
                    datiQuadroMaggiorazioniBenefici.TabLegge407 == 0 || datiQuadroMaggiorazioniBenefici.TabArticolo2 == 0 ||
                    datiQuadroMaggiorazioniBenefici.TabPrivilegiate == 0)
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                if (!datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && !datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && !datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue &&
                    !datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && !datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue)
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

                #region Gestione Semaforo Detrazioni
                // Le detrazioni non devono essere presenti nel caso in cui venga salvata l'esenzione fiscale
                Utility.ManageSemaforoDetrazioniPerEsenzioneFiscale(datiPensione, datiQuadroDetrazioni, datiGenerici.CodiceComunicazioneCampo4, isRiaperturaDomanda, false, isBeneficioVittimeUnderOver80);
                #endregion Gestione Semaforo Detrazioni

                #region Gestione Semaforo Redditi
                //20150107 - E' stato richiesto di rendere rosso il semaforo dei redditi nel caso in cui non venga passato questo controllo
                string msg;
                if (!GestioneCrossControls.ALL_VerificaDecorrenzaEliminazioneWithRedditi(lstRedditi, (datiGenerici != null) ? (datiGenerici.DataEvento) : (null), out msg))
                {
                    datiQuadroRedditi.TabRedditi = 0;
                    GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                }
                #endregion Gestione Semaforo Redditi

                #region Gestione Semaforo DatiCalcolo
                if (tipoFondo == Utility.TipoFondo.ET && isAnteArmonizzazione && !(((ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(contenitore.DatiPensione)) || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(contenitore.DatiPensione)) && !Utility.IsDomandaINPDAP(contenitore.DatiPensione.Gestione)))
                {
                    if (Utility.IsVisibleTabAltraPensioneDatiAgo(datiPensione, datiDanteCausa, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione))
                    {
                        if (datiQuadroDatiCalcolo.TabDatiAgo == null)
                            datiQuadroDatiCalcolo.TabDatiAgo = 0;
                    }
                    else
                    {
                        datiQuadroDatiCalcolo.TabDatiAgo = null;
                    }
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiCalcolo);
                }
                #endregion Gestione Semaforo DatiCalcolo

                #region Gestione Semaforo Oneri

                if (genericiChangedPerOneri)
                {
                    GestioneOneri.DatiOneri datiOneriSperDonna = lstDatiOneri != null ? lstDatiOneri.Where(x => x.IdCodeGruppo == decGruppoOnere.Find(y => y.Code == "4700").Id && x.IdCodeSottoGruppo == decSottoGruppoOneri.Find(y => y.Code == "4701").Id).FirstOrDefault() : null;
                    if (Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri) && datiOneriSperDonna == null)
                    {
                        GestioneOneri.DatiOneri newOneri = new GestioneOneri.DatiOneri { IdCodeGruppo = decGruppoOnere.Find(y => y.Code == "4700").Id, IdCodeSottoGruppo = decSottoGruppoOneri.Find(y => y.Code == "4701").Id, Decorrenza = datiPensione.DecorrenzaOriginaria, IdPensione = datiPensione.Id };
                        GestioneOneri.SalvaOneriOnere(newOneri);
                        recordOneriChanged = true;
                    }
                    else if (!Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, lstDatiOneri) && datiOneriSperDonna != null)
                    {
                        GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                        lstDatiOneri.Where(x => x.IdCodeGruppo != decGruppoOnere.Find(y => y.Code == "4700").Id && x.IdCodeSottoGruppo != decSottoGruppoOneri.Find(y => y.Code == "4701").Id)
                            .ToList()
                            .ForEach(x => GestioneOneri.SalvaOneriOnere(x));
                        recordOneriChanged = true;
                    }

                    // Oneri
                    if ((Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                        Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                        Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                        Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                        Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione)) ||
                        Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri) ||
                        Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) ||
                        Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione)
                        || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) ||
                        Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione) ||
                        (datiBenefici != null && datiBenefici.TipoSettimaneBeneficio == "01")
                        || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) ||
                        Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                        || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                        || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
                    {
                        if (((datiQuadroOneri.TabOneri == null) || (datiQuadroOneri.TabOneri == 2 && recordOneriChanged)) && !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione))))
                            datiQuadroOneri.TabOneri = 0; //rosso
                    }
                    else
                        datiQuadroOneri.TabOneri = null;

                    if (//condizione visibilità oneri
                        Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                        Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                        Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                        Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                        Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) ||
                        Utility.IsOneriSperDonnaVisibili(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, lstDatiOneri) ||
                        Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) ||
                        Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione)
                        || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) ||
                        Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione) ||
                        (datiBenefici != null && datiBenefici.TipoSettimaneBeneficio == "01")
                        || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) ||
                        Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)
                        || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))
                        || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
                        if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione))))
                            datiQuadroOneri.Tipo = 2;
                        else
                            datiQuadroOneri.Tipo = 0;

                    GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                }
                #endregion Gestione Semaforo Oneri

                if ((Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda) && controlTabRic != null && !controlTabRic.TabGenerici)
                    datiQuadroLiquidazionePensione.TabDatiGenerici = null;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);

                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE ---*/
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiIstruttoria = datiIstruttoria;
            contenitore.DatiEliminazione = datiEliminazione;
            contenitore.DatiFondo = datiFondo;
            contenitore.DatiPagamento = datiPagamento;
            contenitore.DatiFondoEL = datiFondoEL;
            contenitore.DatiFondoTT = datiFondoTT;
            contenitore.DatiFondoET = datiFondoET;
            contenitore.DatiFondoVL = datiFondoVL;
            contenitore.ListaDatiFondoPT = listaDatiFondoPT;
            contenitore.DatiFondoPT = (listaDatiFondoPT != null && listaDatiFondoPT.Count > 0) ? listaDatiFondoPT.First() : null;
            contenitore.ListaDatiFondoFST = listaDatiFondoFST;
            contenitore.DatiFondoFS = (listaDatiFondoFST != null && listaDatiFondoFST.Count > 0) ? listaDatiFondoFST.First() : null;
            contenitore.DatiFondoGAS = datiFondoGAS;
            contenitore.DatiFondoDZ = datiFondoDZ;
            contenitore.DatiFondoES = datiFondoES;
            contenitore.DatiFondoPM = datiFondoPM;
            contenitore.ListaDatiRecordFondo = listaDatiRecordFondo;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.DatiQuadroSupplementi = datiQuadroSupplementi;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            contenitore.DatiQuadroRichiestaBonus = datiQuadroRichiestaBonus;
            contenitore.DatiQuadroDetrazioni = datiQuadroDetrazioni;
            contenitore.DatiQuadroRedditi = datiQuadroRedditi;
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiCalcolo;
            contenitore.DatiQuadroOneri = datiQuadroOneri;

        }

        public static void StoreDatiGenericiINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, DatiGenericiINPDAP datiGenericiINPDAP, DatiAssicurativiINPDAP datiAssicurativiINPDAP,
            ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneFondo.DatiFondo datiFondo, ref List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP,
            ref GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione, ref GestionePensione.DatiEliminazione datiEliminazione, ref GestionePagamento.DatiPagamento datiPagamento, bool isRiaperturaDomanda,
            bool isCancelOperation, bool isSingleTab)
        {
            if (datiGenericiINPDAP == null)
                datiGenericiINPDAP = new DatiGenericiINPDAP();

            List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo = null;
            //ENG - memo 28_2024
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = contenitore.DatiAnagraficiTitolare;

            if (isSingleTab)
            {
                List<RipartizioneINPDAP> listaRipartizioneINPDAP = null;
                GetDatiAssicurativiINPDAP(ref contenitore, datiPensione, datiFondo, listaDatiPensioneINPDAP != null ? listaDatiPensioneINPDAP.FirstOrDefault() : null, out datiAssicurativiINPDAP, out listaRipartizioneINPDAP);
            }

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiAssicurativiINPDAP != null && datiAssicurativiINPDAP.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiAssicurativiINPDAP.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            List<RecordFondo> listaRecordFondo;
            GestioneAreaRecordFondo.GetListaRecordFondoByIdPensione(ref contenitore, out listaRecordFondo);

            if (listaRecordFondo != null &&
                listaRecordFondo.Count(x => x.DecorrenzaValiditaDati <= datiPensione.DecorrenzaOriginaria || !x.DecorrenzaValiditaDati.HasValue) > 0)
            {
                var firstRecordFondo = listaRecordFondo.OrderBy(x => x.DecorrenzaValiditaDati).First(x => x.DecorrenzaValiditaDati <= datiPensione.DecorrenzaOriginaria || !x.DecorrenzaValiditaDati.HasValue);
                if (!string.IsNullOrEmpty(datiGenericiINPDAP.NaturaPensione))
                {
                    firstRecordFondo.CodiceNatura1 = datiGenericiINPDAP.NaturaPensione[0];
                    firstRecordFondo.CodiceNatura2 = datiGenericiINPDAP.NaturaPensione[1];
                    firstRecordFondo.CodiceNatura3 = datiGenericiINPDAP.NaturaPensione[2];
                }
                else
                    firstRecordFondo.CodiceNatura1 = firstRecordFondo.CodiceNatura2 = firstRecordFondo.CodiceNatura3 = null;


            }

            //ENG - memo 28_2024
            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") &&
                    datiGenericiINPDAP != null && datiGenericiINPDAP.TipoCalcolo.HasValue && datiGenericiINPDAP.TipoCalcolo == 19)
                {
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                    {
                        if (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 01, 01)))
                        {
                            DateTime? cessazioneIncumulabilita = Utility.CalcolaCessazioneIncumulabilita_memo_28(datiPensione, datiAnagraficiTitolare, datiPensione.DataPerfezionamentoRequisiti);
                            if (cessazioneIncumulabilita.HasValue)
                            {
                                datiGenericiINPDAP.ScadenzaRevisioneSanitaria = cessazioneIncumulabilita;
                            }
                        }
                        else
                        {
                            datiGenericiINPDAP.ScadenzaRevisioneSanitaria = null;
                        }
                    }
                    else if (contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.HasValue)
                        datiGenericiINPDAP.ScadenzaRevisioneSanitaria = contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria;
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiGenericiINPDAPPerPensione(datiGenericiINPDAP, datiPensione, isRiaperturaDomanda);
                StoreDatiGenericiINPDAPPerIstruttoria(datiPensione, datiGenericiINPDAP, ref datiIstruttoria);
                StoreDatiGenericiINPDAPPerEliminazione(datiPensione.Id, datiGenericiINPDAP, ref datiEliminazione);
                StoreDatiGenericiINPDAPPerPensioneFondoDatiGenerici(datiPensione.Id, datiGenericiINPDAP, ref datiFondo);
                StoreDatiGenericiINPDAPPerPensioneINPDAP(datiPensione.Id, datiGenericiINPDAP, ref listaDatiPensioneINPDAP);
                StoreDatiGenericiINPDAPPerDatiPagamento(datiPensione, datiGenericiINPDAP, ref datiPagamento);
                if (isCancelOperation)
                    datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                else
                {
                    if (!datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneNull() || !datiGenericiINPDAP.IsDatiGenericiINPDAPIstruttoriaNull() ||
                        !datiGenericiINPDAP.IsDatiGenericiINPDAPEliminazioneNull() || !datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneFondoDatiGenericiNull() ||
                        !datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneINPDAPNull())
                        datiQuadroLiquidazionePensione.TabDatiGenerici = 2;
                    else
                        datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                }

                #region Gestione visibilità tabs MaggiorazioneBenefici

                if (datiGenericiINPDAP.ExCombattente.HasValue && datiGenericiINPDAP.ExCombattente.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabExCombattente == null)
                        datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = null;

                if (datiGenericiINPDAP.Benefici.HasValue && datiGenericiINPDAP.Benefici.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabBenefici == null)
                        datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabBenefici = null;

                if ((datiGenericiINPDAP.ExCombattente.HasValue && datiGenericiINPDAP.ExCombattente.Value && datiQuadroMaggiorazioniBenefici.TabExCombattente == 2) ||
                    (datiGenericiINPDAP.Benefici.HasValue && datiGenericiINPDAP.Benefici.Value && datiQuadroMaggiorazioniBenefici.TabBenefici == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;

                if (datiQuadroMaggiorazioniBenefici.TabExCombattente == 0 || datiQuadroMaggiorazioniBenefici.TabBenefici == 0 ||
                    datiQuadroMaggiorazioniBenefici.TabLegge407 == 0 || datiQuadroMaggiorazioniBenefici.TabArticolo2 == 0 ||
                    datiQuadroMaggiorazioniBenefici.TabPrivilegiate == 0)
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                if (!datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && !datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && !datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue &&
                    !datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && !datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && !datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.HasValue)
                    datiQuadroMaggiorazioniBenefici.Tipo = 0;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                #endregion Gestione visibilità tabs MaggiorazioneBenefici

                GestioneAreaRecordFondo.SalvaRecordFondo(datiPensione.Id, listaRecordFondo, out listaDatiRecordFondo);
                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);

                transactionScope.Complete();

            }

            /* --- AGGIORNAMENTO DATI SUL CONTENITORE --- */
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiIstruttoria = datiIstruttoria;
            contenitore.DatiEliminazione = datiEliminazione;
            contenitore.DatiFondo = datiFondo;
            contenitore.ListaDatiPensioneINPDAP = listaDatiPensioneINPDAP;
            contenitore.DatiPagamento = datiPagamento;
            contenitore.ListaDatiRecordFondo = listaDatiRecordFondo;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
        }

        private static void StoreDatiGenericiINPDAPPerDatiPagamento(GestionePensione.DatiPensione datiPensione, Entity.DatiGenericiINPDAP datiGenerici, ref GestionePagamento.DatiPagamento datiPagamento)
        {
            if (datiPagamento == null)
            {
                if (datiGenerici.IsDatiGenericiINPDAPPagamentoNull())
                    return;
                else
                    datiPagamento = new GestionePagamento.DatiPagamento();
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

        public static void EliminaDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneFondo.DatiFondo datiFondo,
            ref List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP, ref GestionePensione.DatiEliminazione datiEliminazione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            Entity.DatiDL407 datiDL407, Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici, Entity.DatiPrivilegiate datiPrivilegiate,
            Entity.DatiArticolo2 datiArticolo2, out string errore)
        {
            errore = string.Empty;

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestionePagamento.DatiPagamento datiPagamento = contenitore.DatiPagamento;

            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondo != null && datiFondo.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    byte codiceSpecifico = datiFondo.CodiceSpecifico.Value;
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == codiceSpecifico);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.GAS:
                    case Utility.TipoFondo.DZ:
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.PM:
                        {

                            if (!GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, null))
                            {
                                errore = "Eliminare i dati Supplementi prima di procedere con la cancellazione";
                                return;
                            }

                            if (!ControlsDatiGenericiForPrecedentePensione(datiIstruttoria))
                            {
                                errore = "Eliminare i 'Dati Precedente Pensione' prima di procedere con la cancellazione";
                                return;
                            }

                            if (!ControlsDatiGenericiForDatiContributivi(ref contenitore, ref contenitoreDecodifica, datiPensione.TipoCalcolo, true, tipoFondo))
                            {
                                errore = "Eliminare i 'Dati Calcolo' prima di procedere con la cancellazione";
                                return;
                            }

                            bool? bChkDL407 = null;
                            bool? bChkPrivilegiate = null;
                            bool? bChkArticolo2 = null;

                            if (datiFondo != null)
                            {
                                bChkDL407 = datiFondo.ChkDL407;
                                bChkPrivilegiate = datiFondo.Privilegiate;
                                bChkArticolo2 = datiFondo.Articolo2;
                            }
                            //Modificato: aggiunti controlli per le nuove tab di MaggBen. Controllo valido per TUTTI i fondi
                            if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiPensione.ExCombattente, datiPensione.Benefici, bChkDL407, bChkPrivilegiate, bChkArticolo2, true, tipoFondo,
                                datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, isDomandaConNuovaGestioneDatiFondoFSPT, out errore))
                                return;
                        }
                        if (tipoFondo == Utility.TipoFondo.ET)
                        {
                            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                            {
                                GestioneFondo.DatiFondoET datiETApp;
                                GestioneFondo.GetFondoETByIdPensione(datiPensione.Id, out datiETApp);
                                if (datiETApp != null && !GestioneControlli.ControlET_AltraPensDatiAgo(datiPensione.DecorrenzaOriginaria, "   ", datiETApp, out errore))
                                {
                                    return;
                                }
                            }
                        }
                        break;
                    case Utility.TipoFondo.PI:
                        Utility.CategoriaFondoPI? categoriaPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                        if (categoriaPI == Utility.CategoriaFondoPI.U || categoriaPI == Utility.CategoriaFondoPI.V)
                        {
                            bool? bChkDL407 = null;
                            bool? bChkPrivilegiate = null;
                            bool? bChkArticolo2 = null;
                            if (datiFondo != null)
                            {
                                bChkDL407 = datiFondo.ChkDL407;
                                bChkPrivilegiate = datiFondo.Privilegiate;
                                bChkArticolo2 = datiFondo.Articolo2;
                            }
                            if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiPensione.ExCombattente, datiPensione.Benefici, bChkDL407, bChkPrivilegiate, bChkArticolo2, true, tipoFondo,
                            datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, isDomandaConNuovaGestioneDatiFondoFSPT, out errore))
                                return;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                        {

                            if (!GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, null))
                            {
                                errore = "Eliminare i dati Supplementi prima di procedere con la cancellazione";
                                return;
                            }

                            if (!ControlsDatiGenericiForPrecedentePensione(datiIstruttoria))
                            {
                                errore = "Eliminare i 'Dati Precedente Pensione' prima di procedere con la cancellazione";
                                return;
                            }

                            if (!ControlsDatiGenericiForDatiContributiviFS_PT(ref contenitore, ref contenitoreDecodifica, datiPensione.TipoCalcolo, true))
                            {
                                errore = "Eliminare i 'Dati Calcolo' prima di procedere con la cancellazione";
                                return;
                            }

                            bool? bChkDL407 = null;
                            bool? bChkPrivilegiate = null;
                            bool? bChkArticolo2 = null;

                            if (datiFondo != null)
                            {
                                bChkDL407 = datiFondo.ChkDL407;
                                bChkPrivilegiate = datiFondo.Privilegiate;
                                bChkArticolo2 = datiFondo.Articolo2;
                            }
                            //Modificato: aggiunti controlli per le nuove tab di MaggBen. Controllo valido per TUTTI i fondi
                            if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiPensione.ExCombattente, datiPensione.Benefici, bChkDL407, bChkPrivilegiate, bChkArticolo2, true, tipoFondo,
                                datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, isDomandaConNuovaGestioneDatiFondoFSPT, out errore))
                                return;
                        }
                        break;
                }
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                if (!GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, null))
                {
                    errore = "Eliminare i dati Supplementi prima di procedere con la cancellazione";
                    return;
                }

                if (!ControlsDatiGenericiForPrecedentePensione(datiIstruttoria))
                {
                    errore = "Eliminare i 'Dati Precedente Pensione' prima di procedere con la cancellazione";
                    return;
                }

                if (!ControlsDatiGenericiForDatiContributivi(ref contenitore, ref contenitoreDecodifica, datiPensione.TipoCalcolo, true, tipoFondo))
                {
                    errore = "Eliminare i 'Dati Calcolo' prima di procedere con la cancellazione";
                    return;
                }

                //Modificato: aggiunti controlli per le nuove tab di MaggBen. Controllo valido per TUTTI i fondi
                if (!ControlsDatiGenericiForMaggBeneficiByIdPensione(datiPensione.ExCombattente, datiPensione.Benefici, null, null, null, true, Utility.TipoFondo.FS,
                    null, datiExCombattente, datiBenefici, null, null, true, out errore))
                    return;
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                EliminaDatiGenericiINPDAPPrivate(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiIstruttoria, ref datiFondo, ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione, ref datiEliminazione, ref datiPagamento, isRiaperturaDomanda);
            else
                EliminaDatiGenericiPrivate(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiIstruttoria, ref datiFondo, datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, ref datiPagamento);
        }

        public static void GetListaTipoCalcolo(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, out List<Entity.TipoCalcolo> listaTipoCalcolo)
        {
            listaTipoCalcolo = new List<Entity.TipoCalcolo>();
            List<GestioneDecodifica.TipoCalcolo> listaTipoCalcoloDB = contenitoreDecodifica.ElencoTipoCalcolo;
            if (listaTipoCalcoloDB != null && listaTipoCalcoloDB.Count > 0)
            {
                listaTipoCalcoloDB = listaTipoCalcoloDB.FindAll(x => x.Tipologia == "FS");

                if (listaTipoCalcoloDB != null && listaTipoCalcoloDB.Count > 0)
                {
                    if (datiPensione != null && (!datiPensione.TipoLetturaUnicarpe.HasValue || datiPensione.TipoLetturaUnicarpe.Value != 'L')) //Se non si tratta di una prima liquidata automatica
                    {
                        GestioneDecodifica.TipoCalcolo tipoCalcoloRemove = listaTipoCalcoloDB.Find(x => x.Id == "25"); // retributivo monti;
                        Utility.TipoFondo? TipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                        if (TipoFondo.HasValue)
                        {
                            switch (TipoFondo.Value)
                            {
                                case Utility.TipoFondo.FS:
                                case Utility.TipoFondo.PT:
                                    if ((Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && !datiPensione.IsPLUnicarpe.GetValueOrDefault()) ||
                                        (!datiPensione.DecorrenzaOriginaria.HasValue || !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 2, 1))))
                                    {
                                        //ENG - Reversibilità 024: il tipo calcolo 25 non deve essere rimosso
                                        GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                                        GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);
                                        GestioneLavorazione.DatiLavorazione datiLavorazione = null;
                                        GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
                                        if (!Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione))
                                        {
                                            listaTipoCalcoloDB.Remove(tipoCalcoloRemove);
                                        }
                                    }
                                    break;
                                case Utility.TipoFondo.PI:
                                case Utility.TipoFondo.PL:
                                    listaTipoCalcoloDB.Remove(tipoCalcoloRemove);
                                    break;
                                default:// vale per tutti i fondi tranne FS e PT
                                    if (!datiPensione.DecorrenzaOriginaria.HasValue || !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 2, 1)))
                                        listaTipoCalcoloDB.Remove(tipoCalcoloRemove);
                                    break;
                            }
                        }

                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione) &&
                            (!datiPensione.DecorrenzaOriginaria.HasValue || !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 2, 1))))
                            listaTipoCalcoloDB.Remove(tipoCalcoloRemove);
                    }

                    //ENG - Aggiornamento Memo 19/2023
                    if (Utility.IsDomandaAnticipataFlessibile(datiPensione))
                    {
                        listaTipoCalcoloDB.RemoveAll(x => x.TraduzioneSuGP == 4);
                    }
                    if (Utility.IsDomanda_Anticipate_NoContributivo(datiPensione)) // Memo 79
                    {
                        var contributivo = listaTipoCalcoloDB.Find(x =>
                            string.Equals(x.Descrizione, "Contributivo", StringComparison.OrdinalIgnoreCase));

                        if (contributivo != null)
                            listaTipoCalcoloDB.Remove(contributivo);
                    }
                }
            }
            if (listaTipoCalcoloDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.TipoCalcolo decodificaTipoCalcoloDB in listaTipoCalcoloDB)
                {
                    LiquidazioneFs.Entity.TipoCalcolo tipoCalcolo = new LiquidazioneFs.Entity.TipoCalcolo();
                    Utility.ValorizzaOggetti(decodificaTipoCalcoloDB, tipoCalcolo);
                    listaTipoCalcolo.Add(tipoCalcolo);
                }
            }
        }

        public static void GetListaTipoLiquidazione(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.TipoLiquidazione> listaTipoLiquidazione)
        {
            listaTipoLiquidazione = new List<INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazione>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazione> listaTipoLiquidazioneDB = contenitoreDecodifica.ElencoTipoLiquidazione;
            if (listaTipoLiquidazioneDB != null && listaTipoLiquidazioneDB.Count > 0)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazione tipoLiquidazioneDB in listaTipoLiquidazioneDB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazione tipoLiquidazione = new INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazione();
                    Utility.ValorizzaOggetti(tipoLiquidazioneDB, tipoLiquidazione);
                    listaTipoLiquidazione.Add(tipoLiquidazione);
                }
            }
        }

        public static void GetListaCodiceTipoLiquidazionePM(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceTipoLiquidazionePM> listaCodiceTipoLiquidazionePM)
        {
            listaCodiceTipoLiquidazionePM = new List<INPS.Pensioni.LiquidazioneFs.Entity.CodiceTipoLiquidazionePM>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceTipoLiquidazionePM> listaCodiceTipoLiquidazioneDB = contenitoreDecodifica.ElencoCodiceTipoLiquidazionePM;
            if (listaCodiceTipoLiquidazioneDB != null && listaCodiceTipoLiquidazioneDB.Count > 0)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceTipoLiquidazionePM codiceTipoLiquidazioneDB in listaCodiceTipoLiquidazioneDB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.CodiceTipoLiquidazionePM codiceTipoLiquidazionePM = new INPS.Pensioni.LiquidazioneFs.Entity.CodiceTipoLiquidazionePM();
                    Utility.ValorizzaOggetti(codiceTipoLiquidazioneDB, codiceTipoLiquidazionePM);
                    listaCodiceTipoLiquidazionePM.Add(codiceTipoLiquidazionePM);
                }
            }
        }

        public static void GetListaCodiciNatura(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, out List<Entity.CodiciNatura> listaCodiciNatura)
        {
            listaCodiciNatura = null;
            List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_FS = contenitoreDecodifica.ElencoCodiciNaturaFS;
            if (elencoCodiciNaturaCommon_FS != null)
            {
                GetCodiciNaturaCustom(datiPensione, ref elencoCodiciNaturaCommon_FS);
                listaCodiciNatura = new List<Entity.CodiciNatura>();

                foreach (GestioneDecodifica.CodiciNatura CodiciNaturaCommon_FS in elencoCodiciNaturaCommon_FS)
                {
                    Entity.CodiciNatura codeNatura = new Entity.CodiciNatura();
                    codeNatura.Fondo = CodiciNaturaCommon_FS.Fondo;
                    codeNatura.Descrizione = CodiciNaturaCommon_FS.Descrizione;
                    codeNatura.Posizione = CodiciNaturaCommon_FS.Posizione;
                    codeNatura.Tipologia = CodiciNaturaCommon_FS.Tipologia;
                    codeNatura.TraduzioneSuGP = CodiciNaturaCommon_FS.TraduzioneSuGP;
                    listaCodiciNatura.Add(codeNatura);
                }
            }
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

        #endregion Dati Generici

        #region Dati Assicurativi

        public static bool ControlDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneFondo.DatiFondo datiFondo, Entity.DatiGenerici datiGenerici,
            Entity.DatiAssicurativi datiAssicurativi, List<Entity.RecordFondo> listaRecordFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            bool? IsCodiceSpecificoObbligatorio = null;

            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            object datiFelpe = null;
            GestioneContrib.DatiCalcolo datiCalcolo = null;
            GestioneContrib.GetDatiCalcoloByDomandaFelpe(datiPensione, datiMaggiorazioniBenefici, datiFondo, isRiaperturaDomanda, out datiCalcolo, out datiFelpe, out messaggioVideo);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = contenitore.DatiAnagraficiTitolare;
            GestioneAnagrafica.DatiAnagrafici anagraficaDC = contenitore.DatiAnagraficiDanteCausa;
            List<DatiRecordNoCalcolo> listaDatiRecordNoCalcolo = null;
            GestioneAreaNoCalcolo.GetRecordNoCalcolo(datiPensione, out listaDatiRecordNoCalcolo);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            if (datiAssicurativi == null)
                return true;

            if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
            {
                return true;
            }

            string attivitaSvoltaTraduzioneSuGP = string.Empty;
            if (!string.IsNullOrEmpty(datiAssicurativi.AttivitaSvolta))
            {
                GestioneDecodifica.AttivitaSvolta attivitaSvolta = null;
                GestioneDecodifica.GetAttivitaSvoltaById(datiAssicurativi.AttivitaSvolta, out attivitaSvolta);
                if (attivitaSvolta != null)
                    attivitaSvoltaTraduzioneSuGP = attivitaSvolta.TraduzioneSuGp;
            }

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiAssicurativi.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiAssicurativi.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }
            //ENG - PL Reversibilita 024
            //ENG - RIC Reversibilita 024 
            if (!datiAssicurativi.InizioAssicurazione.HasValue)
            {
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione))) &&
                    !(tipoFondo == Utility.TipoFondo.FS && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.PRIMO_VERSAMENTO_FONDO_FS))
                    && !(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                    && !(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                {
                    messaggioVideo = "La data di Primo Versamento è obbligatoria";
                    return false;
                }
            }

            if (!datiAssicurativi.FineAssicurazione.HasValue)
            {
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione)))
                    && !(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                {
                    messaggioVideo = "La data di Ultimo Versamento è obbligatoria";
                    return false;
                }
            }

            //bool? IsCodiceSpecificoObbligatorio = GetIsCodiceSpecificoObbligatorio(datiPensione);
            //if ((!IsCodiceSpecificoObbligatorio.HasValue || IsCodiceSpecificoObbligatorio.Value) && !datiAssicurativi.CodiceSpecifico.HasValue)
            //{
            //    messaggioVideo = "Il codice specifico è obbligatorio";
            //    return false;
            //}

            if (datiAssicurativi.InizioAssicurazione.HasValue && datiAssicurativi.FineAssicurazione.HasValue && datiAssicurativi.InizioAssicurazione.Value.Date > datiAssicurativi.FineAssicurazione.Value.Date)
            {
                messaggioVideo = "La data di Ultimo Versamento deve superare quella di Primo Versamento";
                return false;
            }

            if (datiGenerici == null)
                datiGenerici = new Entity.DatiGenerici();

            if (!isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                #region Gestione Records fondo

                //controlli record fondo
                if (listaRecordFondo == null || listaRecordFondo.Count == 0)
                {
                    messaggioVideo = "E' obbligatorio inserire almeno un record fondo";
                    return false;
                }

                if (listaDatiRecordNoCalcolo != null && listaDatiRecordNoCalcolo.Count > 0 && !listaRecordFondo.Exists(x => x.CodiceNonCalcolo == 'S'))
                {
                    messaggioVideo = "Il codice non calcolo deve essere a SI in presenza dei Dati No Calcolo.";
                    return false;
                }

                bool isCodiceNoCalcoloNOPresente = false;
                for (int i = 0; i < listaRecordFondo.Count; i++)
                {
                    Entity.RecordFondo primoRecordFondo = listaRecordFondo[0];
                    Entity.RecordFondo iRecordFondo = listaRecordFondo[i];
                    isCodiceNoCalcoloNOPresente = iRecordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'N' ? true : isCodiceNoCalcoloNOPresente;

                    if (isCodiceNoCalcoloNOPresente && iRecordFondo.CodiceNonCalcolo == 'S')
                    {
                        messaggioVideo = "Non è possibile passare da una gestione con calcolo a una no calcolo";
                        return false;
                    }

                    if (!GestioneControlli.VerificaExCombattentePerPIU(isCodiceNoCalcoloNOPresente, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.ExCombattente : null,
                        datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA : null, categoriaFondoPI, out messaggioVideo))
                        return false;

                    if ((!iRecordFondo.CodiceNatura1.HasValue /*|| char.IsWhiteSpace(iRecordFondo.CodiceNatura1.Value)*/) &&
                        (!iRecordFondo.CodiceNatura2.HasValue || char.IsWhiteSpace(iRecordFondo.CodiceNatura2.Value)) &&
                        (!iRecordFondo.CodiceNatura3.HasValue || char.IsWhiteSpace(iRecordFondo.CodiceNatura3.Value)))
                    {
                        messaggioVideo = "E' obbligatorio indicare almeno un codice natura per il " + (i + 1) + "° record fondo";
                        return false;
                    }

                    if (!iRecordFondo.DecorrenzaValiditaDati.HasValue)
                    {
                        messaggioVideo = "Decorrenza del " + (i + 1) + "° record fondo obbligatoria";
                        return false;
                    }

                    if (!GestioneControlli.VerificaCodiceNonCalcoloRecordFondo(datiPensione, tipoFondo, iRecordFondo.CodiceNonCalcolo, categoriaFondoPI, i == listaRecordFondo.Count - 1, out messaggioVideo))
                        return false;


                    DateTime? decorrenza = null;
                    string msgForDecorrenza = string.Empty;
                    if (datiPensione.Gruppo.Equals("0003") && datiPensione.Prodotto.Equals("0021"))
                    {
                        decorrenza = datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null;
                        msgForDecorrenza = "Decorrenza della pensione diretta";
                    }
                    else
                    {
                        decorrenza = datiPensione.DecorrenzaOriginaria;
                        msgForDecorrenza = "Decorrenza della pensione";
                    }

                    if (iRecordFondo == listaRecordFondo.LastOrDefault())
                    {
                        if ((Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && listaRecordFondo.Count == 1) || !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                        {
                            if (!String.IsNullOrEmpty(datiGenerici.NaturaPensione) /* && datiGenerici.NaturaPensione.Trim() != ""*/)
                            {
                                if (iRecordFondo.CodiceNatura1.HasValue || iRecordFondo.CodiceNatura2.HasValue || iRecordFondo.CodiceNatura3.HasValue)
                                {
                                    if ((iRecordFondo.CodiceNatura1.HasValue && iRecordFondo.CodiceNatura1 != char.Parse(datiGenerici.NaturaPensione.Substring(0, 1))) ||
                                        (iRecordFondo.CodiceNatura2.HasValue && iRecordFondo.CodiceNatura2 != char.Parse(datiGenerici.NaturaPensione.Substring(1, 1))) ||
                                        (iRecordFondo.CodiceNatura3.HasValue && iRecordFondo.CodiceNatura3 != char.Parse(datiGenerici.NaturaPensione.Substring(2, 1))))
                                    {
                                        messaggioVideo = "I Codici Natura devono coincidere con quelli del record fondo più recente, indicato nel tab 'Dati Assicurativi'";
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    if (i == 0)
                    {
                       
                        //CROSS-CONTROLS  
                        //if (iRecordFondo.DecorrenzaValiditaDati.Date != datiPensione.DecorrenzaOriginaria.Value.Date)
                        if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) &&
                            GestioneControlli.VerificaDecRecordFondoDecPensione(decorrenza, iRecordFondo.DecorrenzaValiditaDati))
                        {
                            messaggioVideo = "Il " + (i + 1) + "° record fondo deve avere una data di Decorrenza uguale alla data di " + msgForDecorrenza;
                            return false;
                        }
                    }

                    if (i > 0)
                    {
                        if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                        {
                            bool isDecPensioneDiversaDaDecFondo = false;
                            bool isDecPensioneUgualeADecFondoUltimoRecord = false;
                            if (!listaRecordFondo.Exists(x => x.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria) && i == listaRecordFondo.Count - 1)
                                isDecPensioneDiversaDaDecFondo = true;

                            //ENG - Linea FS (no GDP, FS, PT) Se nella lista dei record fondo E’ PRESENTE un record con decorrenza uguale a quella della pensione, il controllo viene effettuato sull’ultimo record fondo
                            if (listaRecordFondo.Exists(x => x.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria) && i == listaRecordFondo.Count - 1)
                                isDecPensioneUgualeADecFondoUltimoRecord = true;

                            if (primoRecordFondo.CodiceNatura1.HasValue && primoRecordFondo.CodiceNatura1 == '1' && iRecordFondo.CodiceNatura1.HasValue &&
                                !new List<char> { '1', '2' }.Contains(iRecordFondo.CodiceNatura1.GetValueOrDefault()))
                            {
                                messaggioVideo = "Il " + (i + 1) + "° record fondo deve avere il primo codice natura pari a '1' o '2'";
                                return false;
                            }

                            if (!String.IsNullOrEmpty(datiGenerici.NaturaPensione) && (iRecordFondo.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria || isDecPensioneDiversaDaDecFondo || isDecPensioneUgualeADecFondoUltimoRecord) /* && datiGenerici.NaturaPensione.Trim() != ""*/)
                            {
                                if (iRecordFondo.CodiceNatura1.HasValue || iRecordFondo.CodiceNatura2.HasValue || iRecordFondo.CodiceNatura3.HasValue)
                                {
                                    if ((iRecordFondo.CodiceNatura1.HasValue && iRecordFondo.CodiceNatura1 != char.Parse(datiGenerici.NaturaPensione.Substring(0, 1))) ||
                                        (iRecordFondo.CodiceNatura2.HasValue && iRecordFondo.CodiceNatura2 != char.Parse(datiGenerici.NaturaPensione.Substring(1, 1))) ||
                                        (iRecordFondo.CodiceNatura3.HasValue && iRecordFondo.CodiceNatura3 != char.Parse(datiGenerici.NaturaPensione.Substring(2, 1))))
                                    {
                                        if (isDecPensioneDiversaDaDecFondo)
                                        {
                                            messaggioVideo = "I Codici Natura devono coincidere con quelli dell'ultimo record fondo, indicato nel tab 'Dati Assicurativi'";
                                            return false;
                                        }
                                        else if (isDecPensioneUgualeADecFondoUltimoRecord && !Utility.IsDomandaINPDAP(datiPensione.Gestione) && tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.FS && tipoFondo.Value != Utility.TipoFondo.PT)
                                        {
                                            messaggioVideo = "I Codici Natura devono coincidere con quelli dell'ultimo record fondo, indicato nel tab 'Dati Assicurativi'";
                                            return false;
                                        }
                                        else if (iRecordFondo.DecorrenzaValiditaDati == datiPensione.DecorrenzaOriginaria && !(!Utility.IsDomandaINPDAP(datiPensione.Gestione) && tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.FS && tipoFondo.Value != Utility.TipoFondo.PT))
                                        {
                                            messaggioVideo = "I Codici Natura devono coincidere con quelli del record fondo avente decorrenza uguale alla decorrenza originaria, indicato nel tab 'Dati Assicurativi'";
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                        if (iRecordFondo.DecorrenzaValiditaDati.Value.Date <= primoRecordFondo.DecorrenzaValiditaDati.Value.Date)
                        {
                            messaggioVideo = "La decorrenza del " + (i + 1) + "° record fondo deve essere successiva alla decorrenza del 1° record fondo";
                            return false;
                        }

                        if (iRecordFondo.DecorrenzaValiditaDati.Value.Date <= listaRecordFondo[i - 1].DecorrenzaValiditaDati.Value.Date)
                        {
                            messaggioVideo = "La Decorrenza del " + (i + 1) + "° record fondo deve superare la Decorrenza del " + i + "° record fondo";
                            return false;
                        }
                    }

                    //CROSS-CONTROLS
                    //if (iRecordFondo.DataSospensione.HasValue && iRecordFondo.DataSospensione.Value.Date <= iRecordFondo.DecorrenzaValiditaDati.Date)
                    if (GestioneControlli.VerificaDecorDataSospDecorValDatiRecordFondo(iRecordFondo.DataSospensione, iRecordFondo.DecorrenzaValiditaDati))
                    {
                        messaggioVideo = "La data di sospensione deve superare la Decorrenza del " + (i + 1) + "° record fondo";
                        return false;
                    }

                    for (int j = i + 1; j < listaRecordFondo.Count; j++)
                    {
                        if (iRecordFondo.Equals(listaRecordFondo[j])) // || iRecordFondo.EqualsExceptDecorrenza(listaRecordFondo[j]))
                        {
                            messaggioVideo = "Il " + (j + 1) + "° record fondo è uguale al " + (i + 1) + "° record fondo. Non è possibile inserire più record fondo uguali.";
                            return false;
                        }
                        else if (iRecordFondo.EqualsExceptDecorrenzaNonCalcolo(listaRecordFondo[j]) &&
                                 iRecordFondo.CodiceNonCalcolo != listaRecordFondo[j].CodiceNonCalcolo)
                        {
                            if (iRecordFondo.CodiceNonCalcolo.HasValue && listaRecordFondo[j].CodiceNonCalcolo.HasValue && iRecordFondo.CodiceNonCalcolo.Equals('N') && !listaRecordFondo[j].CodiceNonCalcolo.Equals('N'))
                            {
                                messaggioVideo = "Il " + (j + 1) + "° record fondo è incompatibile al " + (i + 1) + "° record fondo. E' presente un'incongruenza sul CodiceNonCalcolo.";
                                return false;
                            }
                        }
                    }
                }

                if (!GestioneControlli.VerificaTipoCalcoloConRecordFondo_PIU(datiPensione, listaRecordFondo, categoriaFondoPI, datiGenerici.TipoCalcolo, out messaggioVideo))
                    return false;

                #endregion Gestione Records fondo
            }

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.GAS:
                        IsCodiceSpecificoObbligatorio = GetIsCodiceSpecificoObbligatorio(datiPensione, datiDanteCausa);
                        if ((!IsCodiceSpecificoObbligatorio.HasValue || IsCodiceSpecificoObbligatorio.Value) && !datiAssicurativi.CodiceSpecifico.HasValue)
                        {
                            messaggioVideo = "Il codice specifico è obbligatorio";
                            return false;
                        }

                        #region Gestione Presenza Supplementi

                        if (!(tipoFondo.Value == Utility.TipoFondo.ET && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)) &&
                            !GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, datiGenerici.NaturaPensione))
                        {
                            messaggioVideo = "Cancellare i dati della tab 'Supplementi' in Supplementi prima di procedere con il salvataggio dei dati della tab 'Dati Assicurativi'.";
                            return false;
                        }

                        #endregion Gestione Presenza Supplementi

                        #region Gestione Cross Assicurativi - Dati Contrib

                        //Gestione Presenza DatiCalcolo per Competenza 2013 (legge 214)
                        if (!GestioneControlli.VerificaDataUltimoVersamentoWithDatiCalcolo(datiAssicurativi.FineAssicurazione, datiPensione.FineAssicurazione, datiCalcolo))
                        {
                            messaggioVideo = "Data Ultimo Versamento incompatibile con i Dati Calcolo; cancellare i Dati Calcolo prima di procedere con il salvataggio.";
                            return false;
                        }

                        #endregion Gestione Cross Assicurativi - Dati Contrib

                        #region Gestione Cross Assicurativi - CodiceRequisiti - sperimentale donna

                        if (!GestioneControlli.VerificaCodiceRequisitiOrSperimentaleDonna(datiAssicurativi.CodiceRequisiti2, datiPensione, tipoFondo, isRiaperturaDomanda,
                            datiDanteCausa, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaCodiceRequisiti1(datiAssicurativi.CodiceRequisiti1, out messaggioVideo))
                            return false;

                        #endregion Gestione Cross Assicurativi - CodiceRequisiti - sperimentale donna

                        #region CoerenzaTipoCalcolo

                        if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcolo(datiPensione.DecorrenzaOriginaria, datiAssicurativi.FineAssicurazione, Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS), datiPensione.Gruppo, datiPensione.Prodotto, out messaggioVideo))
                            return false;

                        //mail 14/04/2015 - Controllo per rendere obbligatorio 'Elementi Accessori' per ET
                        if (tipoFondo == Utility.TipoFondo.ET)
                        {
                            if (datiAssicurativi != null && datiAssicurativi.fondoET != null &&
                                !GestioneControlli.ET_ObbligatorietaElementiAccessori(datiAssicurativi.fondoET.ElementiAccessori, out messaggioVideo))
                            {
                                return false;
                            }
                        }

                        #endregion CoerenzaTipoCalcolo

                        break;
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                        IsCodiceSpecificoObbligatorio = GetIsCodiceSpecificoObbligatorio(datiPensione, datiDanteCausa);
                        if ((!IsCodiceSpecificoObbligatorio.HasValue || IsCodiceSpecificoObbligatorio.Value) && !datiAssicurativi.CodiceSpecifico.HasValue)
                        {
                            messaggioVideo = "Il codice specifico è obbligatorio";
                            return false;
                        }

                        if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                        {
                            if ((datiAssicurativi.fondoPT != null && !datiAssicurativi.fondoPT.TrediciMensilita.HasValue) ||
                                (datiAssicurativi.fondoFST != null && !datiAssicurativi.fondoFST.TrediciMensilita.HasValue))
                            {
                                messaggioVideo = "Tredicesima Mensilità: campo obbligatorio";
                                return false;
                            }
                        }

                        #region Gestione Presenza Supplementi

                        if (!GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, datiGenerici.NaturaPensione))
                        {
                            messaggioVideo = "Cancellare i dati della tab 'Supplementi' in Supplementi prima di procedere con il salvataggio dei dati della tab 'Dati Assicurativi'.";
                            return false;
                        }

                        #endregion Gestione Presenza Supplementi

                        #region Gestione Cross Assicurativi - Dati Contrib

                        Utility.TipoCalcolo? TipoCalcolo = Utility.GetTipoCalcolo(datiPensione);

                        if (!Utility.IsRicostituzioneOrRiaperturaFSPTPerequata(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria) &&
                            TipoCalcolo.HasValue && (TipoCalcolo.Value == Utility.TipoCalcolo.Retributivo || TipoCalcolo.Value == Utility.TipoCalcolo.Misto))
                        {
                            List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon = contenitore.ListaDatiServizioUtile;
                            if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0 && lServizioUtileCommon.FindIndex(x => !x.IsNull()) > -1)
                            {
                                if (datiPensione.FineAssicurazione.HasValue && datiAssicurativi.FineAssicurazione.HasValue && datiAssicurativi.FineAssicurazione.Value.CompareTo(datiPensione.FineAssicurazione) != 0)
                                {
                                    messaggioVideo = "Cancellare i 'Dati Calcolo' prima di procedere con il salvataggio dei 'Dati Assicurativi'";
                                    return false;
                                }
                            }
                        }

                        #endregion Gestione Cross Assicurativi - Dati Contrib

                        #region Gestione Cross Assicurativi - CodiceRequisiti

                        if (!GestioneControlli.VerificaCodiceRequisitiOrSperimentaleDonna(datiAssicurativi.CodiceRequisiti2, datiPensione, tipoFondo, isRiaperturaDomanda,
                            datiDanteCausa, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaCodiceRequisiti1(datiAssicurativi.CodiceRequisiti1, out messaggioVideo))
                            return false;

                        #endregion Gestione Cross Assicurativi - CodiceRequisiti


                        break;
                    case Utility.TipoFondo.DZ:
                        IsCodiceSpecificoObbligatorio = GetIsCodiceSpecificoObbligatorio(datiPensione, datiDanteCausa);
                        if ((!IsCodiceSpecificoObbligatorio.HasValue || IsCodiceSpecificoObbligatorio.Value) && !datiAssicurativi.CodiceSpecifico.HasValue)
                        {
                            messaggioVideo = "Il codice specifico è obbligatorio";
                            return false;
                        }

                        if (!GestioneControlli.VerificaCodiceRequisiti1(datiAssicurativi.CodiceRequisiti1, out messaggioVideo))
                            return false;

                        if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcolo(datiPensione.DecorrenzaOriginaria, datiAssicurativi.FineAssicurazione, Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS), datiPensione.Gruppo, datiPensione.Prodotto, out messaggioVideo))
                            return false;

                        if (datiAssicurativi.fondoDZ.CodiceBenefici.HasValue)
                        {
                            switch (datiAssicurativi.fondoDZ.CodiceBenefici.Value)
                            {
                                case 2:
                                    if (datiAssicurativi.fondoDZ.DataCessazioneServizio.HasValue && !Utility.DataSuccessivaA(datiAssicurativi.FineAssicurazione.Value, datiAssicurativi.fondoDZ.DataCessazioneServizio.Value))
                                    {
                                        messaggioVideo = "Con 'Codice Benefici' 2, la data 'Cessazione Servizio' deve essere minore della data 'Ultimo Versamento'";
                                        return false;
                                    }
                                    break;
                                case 4:
                                    if (datiAssicurativi.fondoDZ.MaggiorazionePensionePrivilegiataAA.GetValueOrDefault() < 15)
                                    {
                                        messaggioVideo = "Con 'Codice Benefici' 4, il campo 'Maggiorazione Pensione Privilegiata AA' deve essere maggiore o uguale a 15";
                                        return false;
                                    }
                                    break;
                            }
                        }

                        if (datiAssicurativi.fondoDZ.RaggiuntoRequisiti311297.HasValue && datiAssicurativi.fondoDZ.RaggiuntoRequisiti311297.Value &&
                            codiceSpecificoTraduzioneSuGP.HasValue && codiceSpecificoTraduzioneSuGP == 'B')
                        {
                            messaggioVideo = "Con 'Codice Specifico' B, il campo “Raggiunto requisiti al 31/12/1997” non può essere valorizzato con SI";
                            return false;
                        }
                        break;
                    case Utility.TipoFondo.ES:
                        IsCodiceSpecificoObbligatorio = GetIsCodiceSpecificoObbligatorio(datiPensione, datiDanteCausa);
                        if ((!IsCodiceSpecificoObbligatorio.HasValue || IsCodiceSpecificoObbligatorio.Value) && !datiAssicurativi.CodiceSpecifico.HasValue)
                        {
                            messaggioVideo = "Il codice specifico è obbligatorio";
                            return false;
                        }

                        if (!GestioneControlli.VerificaCodiceRequisiti1(datiAssicurativi.CodiceRequisiti1, out messaggioVideo))
                            return false;

                        #region Gestione Presenza Supplementi

                        if (!GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, datiGenerici.NaturaPensione))
                        {
                            messaggioVideo = "Cancellare i dati della tab 'Supplementi' in Supplementi prima di procedere con il salvataggio dei dati della tab 'Dati Assicurativi'.";
                            return false;
                        }

                        #endregion Gestione Presenza Supplementi

                        #region Gestione Cross Assicurativi - Dati Contrib

                        //Gestione Presenza DatiCalcolo per Competenza 2013 (legge 214)
                        if (!GestioneControlli.VerificaDataUltimoVersamentoWithDatiCalcolo(datiAssicurativi.FineAssicurazione, datiPensione.FineAssicurazione, datiCalcolo))
                        {
                            messaggioVideo = "Data Ultimo Versamento incompatibile con i Dati Calcolo; cancellare i Dati Calcolo prima di procedere con il salvataggio.";
                            return false;
                        }

                        #endregion Gestione Cross Assicurativi - Dati Contrib

                        #region CoerenzaTipoCalcolo

                        if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcolo(datiPensione.DecorrenzaOriginaria, datiAssicurativi.FineAssicurazione, Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS), datiPensione.Gruppo, datiPensione.Prodotto, out messaggioVideo))
                            return false;

                        #endregion CoerenzaTipoCalcolo

                        break;
                    case Utility.TipoFondo.CL:
                        #region Gestione Cross Assicurativi - CodiceRequisiti - sperimentale donna

                        if (!GestioneControlli.VerificaCodiceRequisiti1(datiAssicurativi.CodiceRequisiti1, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaCodiceRequisiti2CL(datiAssicurativi.CodiceRequisiti2, out messaggioVideo))
                            return false;

                        #endregion Gestione Cross Assicurativi - CodiceRequisiti - sperimentale donna
                        break;
                    case Utility.TipoFondo.PM:
                        IsCodiceSpecificoObbligatorio = GetIsCodiceSpecificoObbligatorio(datiPensione, datiDanteCausa);
                        if ((!IsCodiceSpecificoObbligatorio.HasValue || IsCodiceSpecificoObbligatorio.Value) && !datiAssicurativi.CodiceSpecifico.HasValue)
                        {
                            messaggioVideo = "Il codice specifico è obbligatorio";
                            return false;
                        }

                        if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                            if (!GestioneControlli.VerificaCodiceRequisiti1(datiAssicurativi.CodiceRequisiti1, out messaggioVideo))
                                return false;

                        #region Gestione Cross Assicurativi - Dati Contrib

                        //Gestione Presenza DatiCalcolo per Competenza 2013 (legge 214)
                        if (!GestioneControlli.VerificaDataUltimoVersamentoWithDatiCalcolo(datiAssicurativi.FineAssicurazione, datiPensione.FineAssicurazione, datiCalcolo))
                        {
                            messaggioVideo = "Data Ultimo Versamento incompatibile con i Dati Calcolo; cancellare i Dati Calcolo prima di procedere con il salvataggio.";
                            return false;
                        }

                        #endregion Gestione Cross Assicurativi - Dati Contrib

                        #region CoerenzaTipoCalcolo

                        if (!GestioneCrossControls.FS_VerificaCoerenzaTipoCalcolo(datiPensione.DecorrenzaOriginaria, datiAssicurativi.FineAssicurazione, Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS), datiPensione.Gruppo, datiPensione.Prodotto, out messaggioVideo))
                            return false;

                        #endregion CoerenzaTipoCalcolo
                        break;
                }
            }

            if (!ControlDatiAssicurativiWithFondi(ref contenitore, ref contenitoreDecodifica, tipoFondo, datiPensione, datiIstruttoria, datiFondo, datiAssicurativi, datiGenerici, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                isDomandaConNuovaGestioneDatiFondoFSPT, attivitaSvoltaTraduzioneSuGP, codiceSpecificoTraduzioneSuGP, isRiaperturaDomanda, listaRecordFondo, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaFineAssicurazioneForReversibilita(tipoDomanda, datiAssicurativi.FineAssicurazione, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, tipoAppartenenza, datiPensione.SiglaCategoria,  out messaggioVideo))
                return false;


            //bypass PRIMO_VERSAMENTO
            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.PRIMO_VERSAMENTO))
            {
                // Il controllo va effettuato sulla data nascita del Dante Causa se presente, altrimenti sulla data nascita del Titolare
                if (!GestioneControlli.VerificaPrimoVersamento(datiPensione, datiFondo, datiAssicurativi.InizioAssicurazione, anagraficaDC != null ? anagraficaDC.DataNascita : datiAnagraficiTitolare.DataNascita,
                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty, out messaggioVideo))
                    return false;
            }

            if (!GestioneCrossControls.ALL_ControlsInizioAssicurazioneSperimentaleDonna(datiPensione, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceSpecificoAnteArmonizzazione(datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaSettimane707PresentiMaNonVisibili(datiPensione, codiceSpecificoTraduzioneSuGP, datiCalcolo.IsComma707Null(),
                    datiCalcolo != null ? !datiCalcolo.IsContribL214Null() : false, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsPrimoVersamentoPerAPEPrecoci(datiPensione, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_VerificaNaturaPensioneEAssicurazione_PensioneOpzioneContributivo(datiPensione, datiGenerici != null ? datiGenerici.NaturaPensione : null, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlDatiAssicurativiINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, DatiAssicurativiINPDAP datiAssicurativi, DatiGenericiINPDAP datiGenerici,
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneFondo.DatiFondo datiFondo,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP,
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe, GestionePensione.DatiEliminazione datiEliminazione, bool isSingleTab, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            bool? IsCodiceSpecificoObbligatorio = null;

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = contenitore.DatiAnagraficiTitolare;
            GestioneAnagrafica.DatiAnagrafici anagraficaDC = contenitore.DatiAnagraficiDanteCausa;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (datiAssicurativi == null)
                return true;

            if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione))
            {
                return true;
            }

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiAssicurativi.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiAssicurativi.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            if (!Utility.isDomandaGiornalistiDipendentiConSistemaPrivato(datiPensione))
            {
                if (!datiAssicurativi.InizioAssicurazione.HasValue)
                {
                    messaggioVideo = "La data di Primo Versamento è obbligatoria";
                    return false;
                }

                if (!datiAssicurativi.FineAssicurazione.HasValue)
                {
                    messaggioVideo = "La data di Ultimo Versamento è obbligatoria";
                    return false;
                }

                if (datiAssicurativi.InizioAssicurazione.Value.Date > datiAssicurativi.FineAssicurazione.Value.Date)
                {
                    messaggioVideo = "La data di Ultimo Versamento deve superare quella di Primo Versamento";
                    return false;
                }

                IsCodiceSpecificoObbligatorio = GetIsCodiceSpecificoObbligatorio(datiPensione, datiDanteCausa);
                if ((!IsCodiceSpecificoObbligatorio.HasValue || IsCodiceSpecificoObbligatorio.Value) && !datiAssicurativi.CodiceSpecifico.HasValue)
                {
                    messaggioVideo = "Il codice specifico è obbligatorio";
                    return false;
                }
            }

            if (isSingleTab)
            {
                GetDatiGenericiINPDAP(ref contenitore, datiPensione, datiIstruttoria, datiFondo, listaDatiPensioneINPDAP != null ? listaDatiPensioneINPDAP.FirstOrDefault() : null, datiControlloFelpe, datiEliminazione,
                    out datiGenerici);
            }

            #region Gestione Presenza Supplementi

            if (!GestioneControlli.VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref contenitore, datiPensione, codiceSpecificoTraduzioneSuGP, datiGenerici.NaturaPensione))
            {
                messaggioVideo = "Cancellare i dati della tab 'Supplementi' in Supplementi prima di procedere con il salvataggio dei dati della tab 'Dati Assicurativi'.";
                return false;
            }

            #endregion Gestione Presenza Supplementi

            #region Gestione Cross Assicurativi - Dati Contrib

            Utility.TipoCalcolo? TipoCalcolo = Utility.GetTipoCalcolo(datiPensione);

            if (TipoCalcolo.HasValue && (TipoCalcolo.Value == Utility.TipoCalcolo.Retributivo || TipoCalcolo.Value == Utility.TipoCalcolo.Misto))
            {
                List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lServizioUtileCommon = contenitore.ListaDatiServizioUtileINPDAP;
                if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0 && lServizioUtileCommon.FindIndex(x => !x.IsNull()) > -1)
                {
                    if (datiAssicurativi.FineAssicurazione.Value.CompareTo(datiPensione.FineAssicurazione) != 0)
                    {
                        messaggioVideo = "Cancellare i 'Dati Calcolo' prima di procedere con il salvataggio dei 'Dati Assicurativi'";
                        return false;
                    }
                }
            }

            #endregion Gestione Cross Assicurativi - Dati Contrib

            GestioneCrossControls.TipoDecPensione? tipoDecPensione = GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(datiPensione.DecorrenzaOriginaria,
                datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo);

            object datiFondoXX = null;
            if (tipoDecPensione.HasValue &&
               ((tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia)) &&
               (datiGenerici == null || datiGenerici.IsDatiGenericiINPDAPPensioneINPDAPNull()))
            {
                messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                return false;
            }
            else
            {
                if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia ||
                    tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                {
                    List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = new List<GestioneFondo.DatiFondoFST>();
                    GestioneFondo.DatiFondoFST datiFondoFST = new GestioneFondo.DatiFondoFST();
                    Utility.ValorizzaOggetti(datiGenerici, datiFondoFST);
                    listaDatiFondoFST.Add(datiFondoFST);
                    datiFondoXX = listaDatiFondoFST;
                }
            }

            if (!datiAssicurativi.Microqualifica.HasValue)
            {
                messaggioVideo = "La Microqualifica è obbligatoria";
                return false;
            }

            List<MicroqualificaINPDAP> listaMicroqualificaINPDAP = null;
            GestioneLiquidazionePensione.GetListaDecMicroqualificaINPDAP(ref contenitoreDecodifica, null, out listaMicroqualificaINPDAP);
            if (listaMicroqualificaINPDAP != null && listaMicroqualificaINPDAP.Count > 0)
            {
                if (listaMicroqualificaINPDAP.Find(x => x.Id == datiAssicurativi.Microqualifica.Value) == null)
                {
                    messaggioVideo = "La Microqualifica non è compatibile con la categoria";
                    return false;
                }
            }

            if (!GestioneCrossControls.ALL_VerificaFineAssicurazioneForReversibilita(tipoDomanda, datiAssicurativi.FineAssicurazione, datiPensione.DecorrenzaOriginaria,
                datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria, out messaggioVideo))
                return false;

            //bypass PRIMO_VERSAMENTO
            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.PRIMO_VERSAMENTO))
            {
                // Il controllo va effettuato sulla data nascita del Dante Causa se presente, altrimenti sulla data nascita del Titolare
                if (!GestioneControlli.VerificaPrimoVersamento(datiPensione, datiFondo, datiAssicurativi.InizioAssicurazione,
                    anagraficaDC != null ? anagraficaDC.DataNascita : datiAnagraficiTitolare.DataNascita,
                    datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty, out messaggioVideo))
                    return false;
            }

            if (!GestioneCrossControls.ALL_ControlsInizioAssicurazioneSperimentaleDonna(datiPensione, datiAssicurativi.InizioAssicurazione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCodiceSpecificoAnteArmonizzazione(datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaFineAssicurazioneINPDAP(datiPensione, datiFondo, datiAssicurativi != null ? datiAssicurativi.FineAssicurazione : null, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlDatiAssicurativiForCancel(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, List<DatiRecordNoCalcolo> listaDatiRecordNoCalcolo,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;
            GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
            List<GestioneDatiServizioUtile.ServizioUtile> lstDatiServizioUtile = null;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            List<GestioneCalcolo.DatiCalcoloContributivo> ldaticalcolocontributivo = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.DZ:
                        if (datiPensione.Id != 0)
                        {
                            datiContributivi = contenitore.DatiContributivi;

                            if ((contenitore.ListaDatiRetributivi != null && contenitore.ListaDatiRetributivi.Count() > 0) || datiContributivi != null)
                            {
                                messaggioVideo = "Eliminare i dati calcolo prima di procedere con la cancellazione";
                                return false;
                            }
                        }
                        break;
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.PM:
                        if (datiPensione.Id != 0)
                        {
                            datiRetributivi = contenitore.DatiRetributivi;
                            datiContributivi = contenitore.DatiContributivi;

                            if (datiRetributivi != null || datiContributivi != null)
                            {
                                messaggioVideo = "Eliminare i dati calcolo prima di procedere con la cancellazione";
                                return false;
                            }
                        }
                        break;
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                        if (datiPensione.Id != 0)
                        {
                            ldaticalcolocontributivo = contenitore.ListaDatiCalcoloContributivoRecordFondo;
                            lstDatiServizioUtile = contenitore.ListaDatiServizioUtile;

                            if ((ldaticalcolocontributivo != null && ldaticalcolocontributivo.Count > 0) || (lstDatiServizioUtile != null && lstDatiServizioUtile.Count > 0))
                            {
                                messaggioVideo = "Eliminare i dati calcolo prima di procedere con la cancellazione";
                                return false;
                            }
                        }
                        break;
                    case Utility.TipoFondo.GAS:
                        if (datiPensione.Id != 0)
                        {
                            datiRetributivi = contenitore.DatiRetributivi;
                            datiContributivi = contenitore.DatiContributivi;
                            lstDatiServizioUtile = contenitore.ListaDatiServizioUtile;

                            if (datiRetributivi != null || datiContributivi != null)
                            {
                                messaggioVideo = "Eliminare i dati calcolo prima di procedere con la cancellazione";
                                return false;
                            }

                            if (lstDatiServizioUtile != null && lstDatiServizioUtile.Count > 0)
                            {
                                messaggioVideo = "Eliminare i dati fondo prima di procedere con la cancellazione";
                                return false;
                            }
                        }
                        break;
                    case Utility.TipoFondo.PI:
                        if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
                        {
                            if (datiPensione.Id != 0)
                            {
                                GestioneFondo.DatiFondoPI datiFondoPI = contenitore.DatiFondoPI;
                                if (datiFondoPI != null && datiFondoPI.AttCon.HasValue && datiFondoPI.AttCon.Value == '2')
                                {
                                    messaggioVideo = "Eliminare i dati calcolo prima di procedere con la cancellazione";
                                    return false;
                                }
                            }
                        }

                        if (listaDatiRecordNoCalcolo != null && listaDatiRecordNoCalcolo.Count > 0)
                        {
                            messaggioVideo = "Eliminare i dati no calcolo prima di procedere con la cancellazione";
                            return false;
                        }
                        break;
                }

                if (tipoFondo.Value == Utility.TipoFondo.TT)
                {
                    GestioneDL407.DatiDL407 datiDL407 = null;
                    GestioneFondo.DatiFondo datiFondo = null;
                    Utility.TipoCalcolo tipoCalcolo = Utility.TipoCalcolo.NonValido;
                    if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref datiContributivi, ref datiRetributivi, ref datiDL407,
                        ref lstDatiServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo, false, null))
                        return false;
                }
            }
            return true;
        }

        public static bool ControlDatiAssicurativiINPDAPForCancel(out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            return true;
        }

        public static void GetDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, bool isRiaperturaDomanda, out Entity.DatiAssicurativi datiAssicurativi, out List<Entity.RecordFondo> listaRecordFondo)
        {
            datiAssicurativi = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi();
            listaRecordFondo = null;

            if (datiPensione == null)
                return;

            Utility.ValorizzaOggetti(datiPensione, datiAssicurativi);

            if (datiFondo != null)
                Utility.ValorizzaOggetti(datiFondo, datiAssicurativi);

            if (!datiAssicurativi.TipoPensione.HasValue)
            {
                try
                {
                    datiAssicurativi.TipoPensione = GetTipoPensione(datiPensione).First().Value;
                }
                catch (Exception)
                {
                    datiAssicurativi.TipoPensione = Utility.GeTipoPensioneByCodeProdotto(datiPensione.Prodotto);
                }
            }
            if (!datiAssicurativi.Decorrenza.HasValue)
                datiAssicurativi.Decorrenza = datiPensione.DecorrenzaOriginaria;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            GetDatiAssicurativiWithFondiByIdPensione(ref contenitore, tipoFondo, ref datiAssicurativi);

            bool disableCodSpecifico = false;
            bool disableCodArt22 = false;
            PrevalorizzazioneDatiAssicurativi(ref contenitore, tipoFondo, datiPensione, isRiaperturaDomanda, ref datiAssicurativi, out disableCodSpecifico, out disableCodArt22);

            GestioneAreaRecordFondo.GetListaRecordFondoByIdPensione(ref contenitore, out listaRecordFondo);
        }

        public static void GetDatiAssicurativiINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP,
            out Entity.DatiAssicurativiINPDAP datiAssicurativiINPDAP, out List<Entity.RipartizioneINPDAP> listaRipartizioneINPDAP)
        {
            datiAssicurativiINPDAP = null;
            listaRipartizioneINPDAP = new List<RipartizioneINPDAP>();

            if (datiPensione == null)
                return;

            datiAssicurativiINPDAP = new DatiAssicurativiINPDAP();

            Utility.ValorizzaOggetti(datiPensione, datiAssicurativiINPDAP);
            Utility.ValorizzaOggetti(datiFondo, datiAssicurativiINPDAP);
            Utility.ValorizzaOggetti(datiPensioneINPDAP, datiAssicurativiINPDAP);

            if (datiAssicurativiINPDAP.IsDatiAssicurativiINPDAPPensioneNull() && datiAssicurativiINPDAP.IsDatiAssicurativiINPDAPPensioniFondoDatiGenericiNull() &&
                datiAssicurativiINPDAP.IsDatiAssicurativiINPDAPPensioneINPDAPNull())
                datiAssicurativiINPDAP = null;

            List<GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP> listaRipartizioneINPDAPDB = contenitore.ListaDatiRipartizioneINPDAP;
            if (listaRipartizioneINPDAPDB != null && listaRipartizioneINPDAPDB.Count > 0)
            {
                foreach (var ripartizioneDB in listaRipartizioneINPDAPDB)
                {
                    Entity.RipartizioneINPDAP ripartizioneINPDAP = new RipartizioneINPDAP();
                    Utility.ValorizzaOggetti(ripartizioneDB, ripartizioneINPDAP);
                    listaRipartizioneINPDAP.Add(ripartizioneINPDAP);
                }
            }
            else
            {
                // Prevalorizzo i record con tutti gli enti
                listaRipartizioneINPDAP.Add(new RipartizioneINPDAP { CodiceEnte = 1 });
                listaRipartizioneINPDAP.Add(new RipartizioneINPDAP { CodiceEnte = 2 });
                listaRipartizioneINPDAP.Add(new RipartizioneINPDAP { CodiceEnte = 3 });
            }
        }

        public static void StoreDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo, DatiAssicurativi datiAssicurativi,
            List<RecordFondo> listaRecordFondo, bool isCancelOperation)
        {
            if (datiAssicurativi == null)
                datiAssicurativi = new DatiAssicurativi();
            if (listaRecordFondo == null)
                listaRecordFondo = new List<RecordFondo>();

            Liquidazione.BLCommon.Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            #region gestione TipoFondo
            GestioneFondo.DatiFondoEL datiFondoEL = null;
            GestioneFondo.DatiFondoTT datiFondoTT = null;
            GestioneFondo.DatiFondoET datiFondoET = null;
            GestioneFondo.DatiFondoVL datiFondoVL = null;
            List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = null;
            List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = null;
            GestioneFondo.DatiFondoPI datiFondoPI = null;
            GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            GestioneFondo.DatiFondoCL datiFondoCL = null;
            GestioneFondo.DatiFondoDZ datiFondoDZ = null;
            GestioneFondo.DatiFondoES datiFondoES = null;
            GestioneFondo.DatiFondoPM datiFondoPM = null;
            List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        datiFondoEL = contenitore.DatiFondoEL;
                        break;
                    case Utility.TipoFondo.TT:
                        datiFondoTT = contenitore.DatiFondoTT;
                        break;
                    case Utility.TipoFondo.ET:
                        datiFondoET = contenitore.DatiFondoET;
                        break;
                    case Utility.TipoFondo.VL:
                        datiFondoVL = contenitore.DatiFondoVL;
                        break;
                    case Utility.TipoFondo.PT:
                        listaDatiFondoPT = contenitore.ListaDatiFondoPT;
                        break;
                    case Utility.TipoFondo.FS:
                        listaDatiFondoFST = contenitore.ListaDatiFondoFST;
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        datiFondoPI = contenitore.DatiFondoPI;
                        break;
                    case Utility.TipoFondo.GAS:
                        datiFondoGAS = contenitore.DatiFondoGAS;
                        break;
                    case Utility.TipoFondo.CL:
                        datiFondoCL = contenitore.DatiFondoCL;
                        break;
                    case Utility.TipoFondo.DZ:
                        datiFondoDZ = contenitore.DatiFondoDZ;
                        break;
                    case Utility.TipoFondo.ES:
                        datiFondoES = contenitore.DatiFondoES;
                        break;
                    case Utility.TipoFondo.PM:
                        datiFondoPM = contenitore.DatiFondoPM;
                        break;
                }
            }
            #endregion gestione TipoFondo

            //if (datiPensione != null && datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value && datiPensione.TipoLetturaUnicarpe.HasValue && datiPensione.TipoLetturaUnicarpe.Value == 'L')
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (datiPensione.InizioAssicurazione.HasValue)
                    datiAssicurativi.InizioAssicurazione = datiPensione.InizioAssicurazione;
                if (datiPensione.FineAssicurazione.HasValue)
                    datiAssicurativi.FineAssicurazione = datiPensione.FineAssicurazione;

                switch (tipoFondo)
                {
                    case Utility.TipoFondo.VL:
                        if (datiFondoVL != null)
                        {
                            if (datiAssicurativi.fondoVL == null)
                                datiAssicurativi.fondoVL = new Entity.DatiAssicurativi.FondoVL();
                            datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaA = datiFondoVL.RetribuzioneSettimanaleAgoQuotaA;
                            datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaB = datiFondoVL.RetribuzioneSettimanaleAgoQuotaB;
                        }
                        break;
                    case Utility.TipoFondo.ET:
                        if (datiFondoET != null)
                        {
                            if (datiAssicurativi.fondoET == null)
                                datiAssicurativi.fondoET = new DatiAssicurativi.FondoET();

                            datiAssicurativi.fondoET.PersonaleViaggiante = datiFondoET.PersonaleViaggiante;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        if (datiPensione.AttivitaEconomica.HasValue)
                            datiAssicurativi.AttivitaEconomica = datiPensione.AttivitaEconomica;
                        if (datiPensione.ProfessioneIndividuale.HasValue)
                            datiAssicurativi.ProfessioneIndividuale = datiPensione.ProfessioneIndividuale;
                        if (listaDatiFondoFST != null && listaDatiFondoFST.Count > 0)
                        {
                            if (datiAssicurativi.fondoFST == null)
                                datiAssicurativi.fondoFST = new DatiAssicurativi.FondoFST();
                            datiAssicurativi.fondoFST.VVUtiliDiritto = listaDatiFondoFST.First().VVUtiliDiritto;
                            datiAssicurativi.fondoFST.VVUtiliMisura = listaDatiFondoFST.First().VVUtiliMisura;
                        }
                        break;
                    case Utility.TipoFondo.PT:
                        if (datiPensione.AttivitaEconomica.HasValue)
                            datiAssicurativi.AttivitaEconomica = datiPensione.AttivitaEconomica;
                        if (datiPensione.ProfessioneIndividuale.HasValue)
                            datiAssicurativi.ProfessioneIndividuale = datiPensione.ProfessioneIndividuale;
                        if (listaDatiFondoPT != null && listaDatiFondoPT.Count > 0)
                        {
                            if (datiAssicurativi.fondoPT == null)
                                datiAssicurativi.fondoPT = new DatiAssicurativi.FondoPT();
                            datiAssicurativi.fondoPT.VVUtiliDiritto = listaDatiFondoPT.First().VVUtiliDiritto;
                            datiAssicurativi.fondoPT.VVUtiliMisura = listaDatiFondoPT.First().VVUtiliMisura;
                        }
                        break;
                }
            }

            Utility.ValorizzaOggetti(datiAssicurativi, datiPensione);

            long idFondo = contenitore.IdFondoPensione;
            bool eliminaFondoDatiGenerici = false;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
            GestioneQuadri.DatiQuadroDatiNoCalcolo datiQuadroNoCalcolo = contenitore.DatiQuadroNoCalcolo;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            GestioneCtrlRic.ControlTabRic controlTabRic = null;
            if (isRiaperturaDomanda)
                GestioneCtrlRic.GetCtrlTabRic("0107", Utility.TipoAppartenenza.FS, out controlTabRic);
            else
                GestioneCtrlRic.GetCtrlTabRic(datiPensione.Prodotto, Utility.TipoAppartenenza.FS, out controlTabRic);

            //Serve per gestire la scomparsa di CheckBoxExCombattente quando si inserisce un record fondo con codiceNoCalcolo = NO 
            bool exCombattenteChanged = false;
            if (categoriaFondoPI == Utility.CategoriaFondoPI.U)
            {
                if (datiPensione.ExCombattente == true && !IsExCombattenteVisibleForPIU(datiPensione, listaRecordFondo))
                {
                    datiPensione.ExCombattente = false;
                    exCombattenteChanged = true; // mi dice se aggiornare il semaforo di MaggBenef
                }
            }

            bool isDatiServizioUtilePresenti = false;
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = contenitore.ListaDatiServizioUtile;
            if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                isDatiServizioUtilePresenti = true;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensione.SalvaPensione(datiPensione);

                StoreDatiAssicurativiPerFondoDatiGenerici(datiPensione.Id, datiAssicurativi, ref datiFondo, isDomandaConNuovaGestioneDatiFondoFSPT, isDatiServizioUtilePresenti,
                    out eliminaFondoDatiGenerici);

                if (datiFondo != null)
                    idFondo = datiFondo.Id;

                if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                    GestioneAreaRecordFondo.SalvaRecordFondo(datiPensione.Id, listaRecordFondo, out listaDatiRecordFondo);


                //cancellazione
                if (isCancelOperation || (datiAssicurativi.IsFondoDatiGenericiNull() && (listaRecordFondo == null || listaRecordFondo.Count == 0)))
                {
                    #region Gestione TipoFondo
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:

                                if (isCancelOperation || datiAssicurativi.fondoEL == null || datiAssicurativi.fondoEL.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoEL(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoEL, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;

                            case Utility.TipoFondo.TT:

                                if (isCancelOperation || datiAssicurativi.fondoTT == null || datiAssicurativi.fondoTT.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoTT(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoTT, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.ET:

                                if (isCancelOperation || datiAssicurativi.fondoET == null || datiAssicurativi.fondoET.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoET(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoET, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.VL:

                                StoreDatiAssicurativiPerFondoVL(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoVL, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                break;
                            case Utility.TipoFondo.PT:

                                if (isCancelOperation || datiAssicurativi.fondoPT == null || datiAssicurativi.fondoPT.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoPT(datiPensione.Id, idFondo, datiAssicurativi, ref listaDatiFondoPT, isDomandaConNuovaGestioneDatiFondoFSPT, eliminaFondoDatiGenerici, ref datiFondo, ref contenitore);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.FS:

                                if (isCancelOperation || datiAssicurativi.fondoFST == null || datiAssicurativi.fondoFST.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoFST(datiPensione.Id, idFondo, datiAssicurativi, ref listaDatiFondoFST, isDomandaConNuovaGestioneDatiFondoFSPT, eliminaFondoDatiGenerici, ref datiFondo, ref contenitore);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                                if (isCancelOperation || datiAssicurativi.fondoPI == null || datiAssicurativi.fondoPI.IsFondoNull())
                                {                                 
                                    if (categoriaFondoPI.HasValue && (categoriaFondoPI.Value == Utility.CategoriaFondoPI.U || categoriaFondoPI.Value == Utility.CategoriaFondoPI.V))
                                    {                                        
                                        // Nell'Entity dei Dati Calcolo sono presenti alcuni campi, con nome identico, che non sono visibili su Assicurativi
                                        GestioneContrib.FondoPI fondoPI = new GestioneContrib.FondoPI();
                                        Utility.ValorizzaOggetti(datiFondoPI, fondoPI);
                                        Utility.ValorizzaOggetti(fondoPI, datiAssicurativi.fondoPI);
                                    }

                                    StoreDatiAssicurativiPerFondoPI(datiPensione.Id, idFondo, datiAssicurativi, listaDatiRecordFondo.Select(x => x.Id).ToList(), ref datiFondoPI, eliminaFondoDatiGenerici, ref datiFondo, ref listaDatiServizioUtile);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;

                                }
                                break;
                            case Utility.TipoFondo.GAS:

                                if (isCancelOperation || datiAssicurativi.fondoGAS == null || datiAssicurativi.fondoGAS.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoGAS(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoGAS, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.CL:

                                if (isCancelOperation || datiAssicurativi.fondoCL == null || datiAssicurativi.fondoCL.IsFondoNull())
                                {
                                    GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                    listaDatiServizioUtile = null;
                                    StoreDatiAssicurativiPerFondoCL(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoCL, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.DZ:

                                if (isCancelOperation || datiAssicurativi.fondoDZ == null || datiAssicurativi.fondoDZ.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoDZ(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoDZ, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.ES:

                                if (isCancelOperation || datiAssicurativi.fondoES == null || datiAssicurativi.fondoES.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoES(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoES, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                            case Utility.TipoFondo.PM:

                                if (isCancelOperation || datiAssicurativi.fondoPM == null || datiAssicurativi.fondoPM.IsFondoNull())
                                {
                                    StoreDatiAssicurativiPerFondoPM(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoPM, eliminaFondoDatiGenerici, ref datiFondo);
                                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                                }
                                break;
                        }
                    }
                    #endregion Gestione TipoFondo
                }
                else
                {
                    #region Gestione TipoFondo
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:
                                StoreDatiAssicurativiPerFondoEL(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoEL, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.TT:
                                StoreDatiAssicurativiPerFondoTT(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoTT, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.ET:
                                StoreDatiAssicurativiPerFondoET(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoET, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.VL:
                                StoreDatiAssicurativiPerFondoVL(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoVL, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.PT:
                                StoreDatiAssicurativiPerFondoPT(datiPensione.Id, idFondo, datiAssicurativi, ref listaDatiFondoPT, isDomandaConNuovaGestioneDatiFondoFSPT, eliminaFondoDatiGenerici, ref datiFondo, ref contenitore);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.FS:
                                StoreDatiAssicurativiPerFondoFST(datiPensione.Id, idFondo, datiAssicurativi, ref listaDatiFondoFST, isDomandaConNuovaGestioneDatiFondoFSPT, eliminaFondoDatiGenerici, ref datiFondo, ref contenitore);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                                if (categoriaFondoPI.HasValue && (categoriaFondoPI.Value == Utility.CategoriaFondoPI.U || categoriaFondoPI.Value == Utility.CategoriaFondoPI.V))
                                {                                  
                                    // Nell'Entity dei Dati Calcolo sono presenti alcuni campi, con nome identico, che non sono visibili su Assicurativi
                                    GestioneContrib.FondoPI fondoPI = new GestioneContrib.FondoPI();
                                    Utility.ValorizzaOggetti(datiFondoPI, fondoPI);
                                    Utility.ValorizzaOggetti(fondoPI, datiAssicurativi.fondoPI);
                                }
                                StoreDatiAssicurativiPerFondoPI(datiPensione.Id, idFondo, datiAssicurativi, listaDatiRecordFondo.Select(x => x.Id).ToList(), ref datiFondoPI, eliminaFondoDatiGenerici, ref datiFondo, ref listaDatiServizioUtile);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                
                                break;
                            case Utility.TipoFondo.GAS:
                                StoreDatiAssicurativiPerFondoGAS(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoGAS, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.CL:
                                StoreDatiAssicurativiPerFondoCL(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoCL, eliminaFondoDatiGenerici, ref datiFondo);
                                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                                Utility.ValorizzaOggetti(datiAssicurativi.fondoCL, datiServizioUtile);
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                GestioneDatiServizioUtile.SalvaDatiServizioUtile(idFondo, datiServizioUtile);
                                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                                listaDatiServizioUtile.Add(datiServizioUtile);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.DZ:
                                StoreDatiAssicurativiPerFondoDZ(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoDZ, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.ES:
                                StoreDatiAssicurativiPerFondoES(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoES, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                            case Utility.TipoFondo.PM:
                                StoreDatiAssicurativiPerFondoPM(datiPensione.Id, idFondo, datiAssicurativi, ref datiFondoPM, eliminaFondoDatiGenerici, ref datiFondo);
                                datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                                break;
                        }
                    }
                    #endregion Gestione TipoFondo
                }

                if ((Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda))
                {
                    //Eng - Il tab Assicurativi è visibile per le Ricostituzioni non FS/PT/INPDAP con Prodotto "0109" e Tipo "0130"
                    if ((controlTabRic != null && !controlTabRic.TabAssicurativi) || (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
                        tipoFondo != Utility.TipoFondo.FS && tipoFondo != Utility.TipoFondo.PT && !(datiPensione.Prodotto == "0109" && datiPensione.Tipo == "0130" && !Utility.IsDomandaINPDAP(datiPensione.Gestione)))
                        || ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione))
                        || (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                        datiQuadroLiquidazionePensione.TabDatiAssicurativi = null;
                }

                if (categoriaFondoPI.HasValue && (categoriaFondoPI.Value == Utility.CategoriaFondoPI.U || categoriaFondoPI.Value == Utility.CategoriaFondoPI.V || categoriaFondoPI.Value == Utility.CategoriaFondoPI.A) &&
                    listaRecordFondo != null && listaRecordFondo.Count > 0 && listaRecordFondo.Exists(x => x.CodiceNonCalcolo == 'S'))
                {
                    if (datiQuadroNoCalcolo.TabRegistrazioniNoCalcolo != 2)
                    {
                        datiQuadroNoCalcolo.Tipo = 2;
                        datiQuadroNoCalcolo.TabRegistrazioniNoCalcolo = 0;
                    }
                }
                else
                {
                    datiQuadroNoCalcolo.Tipo = 0;
                    datiQuadroNoCalcolo.TabRegistrazioniNoCalcolo = null;
                }

                GestioneQuadri.SalvaQuadroDatiNoCalcolo(datiPensione.Id, datiQuadroNoCalcolo);

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);

                if (exCombattenteChanged)
                {
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = null;
                    if ((datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue) || (datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue) ||
                    (datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue) || (datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue) ||
                    (datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue))
                        datiQuadroMaggiorazioniBenefici.Tipo = 2;

                    if (datiQuadroMaggiorazioniBenefici.TabExCombattente == 0 || datiQuadroMaggiorazioniBenefici.TabBenefici == 0 ||
                        datiQuadroMaggiorazioniBenefici.TabLegge407 == 0 || datiQuadroMaggiorazioniBenefici.TabArticolo2 == 0 ||
                        datiQuadroMaggiorazioniBenefici.TabPrivilegiate == 0)
                        datiQuadroMaggiorazioniBenefici.Tipo = 1;

                    if (!datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && !datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && !datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue &&
                        !datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && !datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue)
                        datiQuadroMaggiorazioniBenefici.Tipo = 0;

                    GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                }

                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiFondo = datiFondo;
            contenitore.ListaDatiRecordFondo = listaDatiRecordFondo;
            contenitore.DatiFondoEL = datiFondoEL;
            contenitore.DatiFondoTT = datiFondoTT;
            contenitore.DatiFondoET = datiFondoET;
            contenitore.DatiFondoVL = datiFondoVL;
            contenitore.ListaDatiFondoPT = listaDatiFondoPT;
            contenitore.DatiFondoPT = (listaDatiFondoPT != null && listaDatiFondoPT.Count > 0) ? listaDatiFondoPT.First() : null;
            contenitore.ListaDatiFondoFST = listaDatiFondoFST;
            contenitore.DatiFondoFS = (listaDatiFondoFST != null && listaDatiFondoFST.Count > 0) ? listaDatiFondoFST.First() : null;
            contenitore.DatiFondoPI = datiFondoPI;
            contenitore.DatiFondoGAS = datiFondoGAS;
            contenitore.DatiFondoCL = datiFondoCL;
            contenitore.DatiFondoDZ = datiFondoDZ;
            contenitore.DatiFondoES = datiFondoES;
            contenitore.DatiFondoPM = datiFondoPM;
            contenitore.ListaDatiServizioUtile = listaDatiServizioUtile;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.DatiQuadroNoCalcolo = datiQuadroNoCalcolo;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        public static void StoreDatiAssicurativiINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, DatiAssicurativiINPDAP datiAssicurativiINPDAP, List<RipartizioneINPDAP> listaDatiRipartizioneINPDAP,
            ref GestioneFondo.DatiFondo datiFondo, ref List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP,
            ref GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione, bool isCancelOperation)
        {
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            GestioneCtrlRic.ControlTabRic controlTabRic = null;
            if (isRiaperturaDomanda)
                GestioneCtrlRic.GetCtrlTabRic("0107", Utility.TipoAppartenenza.FS, out controlTabRic);
            else
                GestioneCtrlRic.GetCtrlTabRic(datiPensione.Prodotto, Utility.TipoAppartenenza.FS, out controlTabRic);

            List<GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP> datiRipartizioneINPDAP = contenitore.ListaDatiRipartizioneINPDAP;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiAssicurativiINPDAPPerPensione(datiAssicurativiINPDAP, datiPensione);
                StoreDatiAssicurativiINPDAPPerPensioneFondoDatiGenerici(datiPensione, datiAssicurativiINPDAP, ref datiFondo);
                StoreDatiAssicurativiINPDAPPerPensioneINPDAP(datiPensione, datiAssicurativiINPDAP, ref listaDatiPensioneINPDAP);
                StoreDatiRipartizioneINPDAP(datiPensione, listaDatiRipartizioneINPDAP, ref datiRipartizioneINPDAP);

                if (isCancelOperation)
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                else
                {
                    if (datiAssicurativiINPDAP.IsDatiAssicurativiINPDAPPensioneNull() && datiAssicurativiINPDAP.IsDatiAssicurativiINPDAPPensioniFondoDatiGenericiNull() &&
                        datiAssicurativiINPDAP.IsDatiAssicurativiINPDAPPensioneINPDAPNull())
                        datiQuadroLiquidazionePensione.TabDatiAssicurativi = 0;
                    else
                        datiQuadroLiquidazionePensione.TabDatiAssicurativi = 2;
                }

                if ((Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda) && controlTabRic != null &&
                    !controlTabRic.TabAssicurativi)
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = null;

                //ENG - GDP - RIC CONCESSIONE ALTRA PENSIONE
                if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione))
                {
                    datiQuadroLiquidazionePensione.TabDatiAssicurativi = null;
                }

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);

                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiFondo = datiFondo;
            contenitore.ListaDatiPensioneINPDAP = listaDatiPensioneINPDAP;
            contenitore.ListaDatiRipartizioneINPDAP = datiRipartizioneINPDAP;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
        }

        private static bool IsExCombattenteVisibleForPIU(GestionePensione.DatiPensione datiPensione, List<RecordFondo> listaRecordFondo)
        {
            bool ret = true;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            Utility.CategoriaFondoPI? categoriaPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            switch (tipoFondo)
            {
                case Utility.TipoFondo.PI:
                    if ((categoriaPI == Utility.CategoriaFondoPI.U)
                        && (listaRecordFondo != null && listaRecordFondo.Exists(x => x.CodiceNonCalcolo == 'N')))
                    { ret = false; }
                    break;
            }
            return ret;
        }

        public static void EliminaDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo, out string errore)
        {
            errore = string.Empty;

            List<GestioneRecordFondo.DatiRecordFondo> datiRecordFondo = contenitore.ListaDatiRecordFondo;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            Entity.DatiAssicurativi datiAssicurativi = new Entity.DatiAssicurativi();
            List<Entity.RecordFondo> listaRecordFondo = new List<Entity.RecordFondo>();

            #region gestione TipoFondo

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        datiAssicurativi.fondoEL = new Entity.DatiAssicurativi.FondoEL();
                        break;
                    case Utility.TipoFondo.TT:
                        datiAssicurativi.fondoTT = new Entity.DatiAssicurativi.FondoTT();
                        break;
                    case Utility.TipoFondo.ET:
                        datiAssicurativi.fondoET = new Entity.DatiAssicurativi.FondoET();
                        break;
                    case Utility.TipoFondo.VL:
                        datiAssicurativi.fondoVL = new Entity.DatiAssicurativi.FondoVL();
                        break;
                    case Utility.TipoFondo.PT:
                        datiAssicurativi.fondoPT = new Entity.DatiAssicurativi.FondoPT();
                        break;
                    case Utility.TipoFondo.FS:
                        datiAssicurativi.fondoFST = new Entity.DatiAssicurativi.FondoFST();
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        datiAssicurativi.fondoPI = new Entity.DatiAssicurativi.FondoPI();
                        break;
                    case Utility.TipoFondo.GAS:
                        datiAssicurativi.fondoGAS = new Entity.DatiAssicurativi.FondoGAS();
                        break;
                    case Utility.TipoFondo.CL:
                        datiAssicurativi.fondoCL = new Entity.DatiAssicurativi.FondoCL();
                        break;
                    case Utility.TipoFondo.DZ:
                        datiAssicurativi.fondoDZ = new Entity.DatiAssicurativi.FondoDZ();
                        break;
                    case Utility.TipoFondo.ES:
                        datiAssicurativi.fondoES = new Entity.DatiAssicurativi.FondoES();
                        break;
                    case Utility.TipoFondo.PM:
                        datiAssicurativi.fondoPM = new Entity.DatiAssicurativi.FondoPM();
                        break;
                }
            }
            #endregion gestione TipoFondo

            StoreDatiAssicurativi(ref contenitore, datiPensione, ref datiFondo, datiAssicurativi, listaRecordFondo, true);
        }

        public static void EliminaDatiAssicurativiINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo,
            ref List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP, ref GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione, out string errore)
        {
            errore = string.Empty;

            List<GestioneQuadri.DatiQuadroDatiRecordFondo> listaDatiQuadroDatiRecordFondo = contenitore.ListaDatiQuadroDatiRecordFondo;
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = contenitore.ListaDatiCalcoloContributivoRecordFondo;
            List<KeyValuePair<long?, bool>> listaIsQuotaDPresente = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                listaIsQuotaDPresente = new List<KeyValuePair<long?, bool>>();
                foreach (GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo in listaDatiCalcoloContributivo)
                    listaIsQuotaDPresente.Add(new KeyValuePair<long?, bool>(datiCalcoloContributivo.IdRecordFondo, datiCalcoloContributivo.IsQuotaDL214Presente()));
            }

            try
            {
                if (listaDatiCalcoloContributivo == null || listaDatiCalcoloContributivo.Count == 0)
                    GestioneAggiornamentoPECO.IsQuotaDPresenteFromFelpeAMG_INPDAP(datiPensione, tipoFondo, out listaIsQuotaDPresente);
            }
            catch (Exception e)
            {
                errore = e.Message;
                return;
            }

            DatiAssicurativiINPDAP datiAssicurativiINPDAP = new DatiAssicurativiINPDAP();
            List<RipartizioneINPDAP> listaRipartizioneINPDAP = new List<RipartizioneINPDAP>();

            StoreDatiAssicurativiINPDAP(ref contenitore, datiPensione, datiAssicurativiINPDAP, listaRipartizioneINPDAP, ref datiFondo, ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione, true);
        }

        public static void GetAttivitaSvolte(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, out List<Entity.DatiAttivitaSvolta> ListaAttivitaSvolte)
        {
            ListaAttivitaSvolte = null;
            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria))
            {
                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = contenitoreDecodifica.ElencoAttivitaSvolte;
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    ListaAttivitaSvolte = new List<INPS.Pensioni.LiquidazioneFs.Entity.DatiAttivitaSvolta>();
                    foreach (GestioneDecodifica.AttivitaSvolta attSvolta in elencoAttivitaSvolte)
                        ListaAttivitaSvolte.Add(new INPS.Pensioni.LiquidazioneFs.Entity.DatiAttivitaSvolta(attSvolta));
                }
            }
        }

        public static void GetListaCodiceRequisito1(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceRequisito1> listaCodiceRequisito1)
        {
            listaCodiceRequisito1 = new List<INPS.Pensioni.LiquidazioneFs.Entity.CodiceRequisito1>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisito1> listaCodiceRequisito1DB = contenitoreDecodifica.ElencoCodiceRequisito1;
            if (listaCodiceRequisito1DB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisito1 codiceRequisito1DB in listaCodiceRequisito1DB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.CodiceRequisito1 codiceRequisito1 = new INPS.Pensioni.LiquidazioneFs.Entity.CodiceRequisito1();
                    codiceRequisito1.Id = codiceRequisito1DB.Id;
                    codiceRequisito1.Descrizione = codiceRequisito1DB.Descrizione;
                    listaCodiceRequisito1.Add(codiceRequisito1);
                }
            }
        }

        public static void GetListaCodiceRequisito2(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceRequisito2> listaCodiceRequisito2)
        {
            listaCodiceRequisito2 = new List<CodiceRequisito2>();
            List<GestioneDecodifica.CodiceRequisito2> listaCodiceRequisito2DB = contenitoreDecodifica.ElencoCodiceRequisito2;
            if (listaCodiceRequisito2DB != null)
            {
                foreach (GestioneDecodifica.CodiceRequisito2 codiceRequisito2DB in listaCodiceRequisito2DB)
                {
                    CodiceRequisito2 codiceRequisito2 = new CodiceRequisito2();
                    codiceRequisito2.Id = codiceRequisito2DB.Id;
                    codiceRequisito2.Descrizione = codiceRequisito2DB.Descrizione;
                    listaCodiceRequisito2.Add(codiceRequisito2);
                }
            }
        }

        public static void GetListaCodiceSpecifico(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, out List<Entity.CodiceSpecifico> listaCodiceSpecifico)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            listaCodiceSpecifico = new List<CodiceSpecifico>();
            List<GestioneDecodifica.CodiceSpecifico> listaCodiceSpecificoDB = contenitoreDecodifica.ElencoCodiceSpecifico;
            if (listaCodiceSpecificoDB != null)
            {
                bool isFondoVL = false;
                bool isFondoDZ = false;

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.EL:
                            listaCodiceSpecificoDB.RemoveAll(x => Utility.DataStrettamenteSuccessivaA(datiPensione.DataPresentazioneDomanda, new DateTime(1996, 11, 15)) && x.Id == 4);
                            break;
                        case Utility.TipoFondo.TT:
                            listaCodiceSpecificoDB.RemoveAll(x => Utility.DataStrettamenteSuccessivaA(datiPensione.DataPresentazioneDomanda, new DateTime(1997, 11, 8)) && (x.Id == 12 || x.Id == 13));
                            listaCodiceSpecificoDB.RemoveAll(x => !Utility.DataStrettamenteSuccessivaA(datiPensione.DataPresentazioneDomanda, new DateTime(1997, 11, 8)) && x.Id == 69);
                            break;
                        case Utility.TipoFondo.ET:
                            listaCodiceSpecificoDB.RemoveAll(x => Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1996, 01, 31)) && (x.Id == 59 || x.Id == 65));
                            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                                listaCodiceSpecificoDB.RemoveAll(x => (x.TraduzioneGp == 'B' || x.TraduzioneGp == 'C'));
                            //Per le PL con Gruppo 0001 e Prodotto pari a 0001 o  0002 eliminati i codici specifici W e Z
                            if (!Utility.IsRiaperturaDomanda(datiPensione.Id) && datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002"))
                                listaCodiceSpecificoDB.RemoveAll(x => x.TraduzioneGp == 'W' || x.TraduzioneGp == 'Z');
                            break;
                        case Utility.TipoFondo.VL:
                            isFondoVL = true;
                            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                                listaCodiceSpecificoDB.RemoveAll(x => x.TraduzioneGp == 'A' || x.TraduzioneGp == 'B' || x.TraduzioneGp == 'C' || x.TraduzioneGp == 'D' || x.TraduzioneGp == 'E');
                            break;
                        case Utility.TipoFondo.DZ:
                            isFondoDZ = true;
                            break;
                    }
                }

                //RIMUOVERE per il fondo VL e DZ - Filtro del codice specifico per domande di anzianità/vecchiaia
                if (!isFondoVL && !isFondoDZ)
                {
                    if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")
                        listaCodiceSpecificoDB = listaCodiceSpecificoDB.FindAll(x => x.Descrizione.ToLowerInvariant().Contains("vecchiaia"));
                    if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001")
                        listaCodiceSpecificoDB = listaCodiceSpecificoDB.FindAll(x => x.Descrizione.ToLowerInvariant().Contains("anzianit"));
                }

                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceSpecifico codiceSpecificoDB in listaCodiceSpecificoDB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.CodiceSpecifico codiceSpecifico = new INPS.Pensioni.LiquidazioneFs.Entity.CodiceSpecifico();
                    Utility.ValorizzaOggetti(codiceSpecificoDB, codiceSpecifico);
                    listaCodiceSpecifico.Add(codiceSpecifico);
                }
            }
        }

        public static void GetListaCodiceConvenzioneInternazionale(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceConvenzioneInternazionale> listaCodiceConvenzioneInternazionale)
        {
            listaCodiceConvenzioneInternazionale = new List<INPS.Pensioni.LiquidazioneFs.Entity.CodiceConvenzioneInternazionale>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzioneInternazionale> listaCodiceConvenzioneInternazionaleDB = contenitoreDecodifica.ElencoCodiceConvenzioneInternazionale;
            if (listaCodiceConvenzioneInternazionaleDB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzioneInternazionale codiceConvenzioneInternazionaleDB in listaCodiceConvenzioneInternazionaleDB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.CodiceConvenzioneInternazionale codiceConvenzioneInternazionale = new INPS.Pensioni.LiquidazioneFs.Entity.CodiceConvenzioneInternazionale();
                    codiceConvenzioneInternazionale.Id = codiceConvenzioneInternazionaleDB.Id;
                    codiceConvenzioneInternazionale.Descrizione = codiceConvenzioneInternazionaleDB.Descrizione;
                    listaCodiceConvenzioneInternazionale.Add(codiceConvenzioneInternazionale);
                }
            }
        }

        public static void GetListaCodiceEsodo(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceEsodo> listaCodiceEsodo)
        {
            listaCodiceEsodo = new List<INPS.Pensioni.LiquidazioneFs.Entity.CodiceEsodo>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaCodeEsodo> listaCodiceEsodoDB = contenitoreDecodifica.ElencoCodiceEsodo;
            if (listaCodiceEsodoDB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaCodeEsodo decodificaCodeEsodoDB in listaCodiceEsodoDB)
                {
                    if (!decodificaCodeEsodoDB.Codice)
                        continue;

                    INPS.Pensioni.LiquidazioneFs.Entity.CodiceEsodo codiceEsodo = new INPS.Pensioni.LiquidazioneFs.Entity.CodiceEsodo();
                    Utility.ValorizzaOggetti(decodificaCodeEsodoDB, codiceEsodo);
                    listaCodiceEsodo.Add(codiceEsodo);
                }
            }
        }

        public static void GetListaCodicePartTime(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodicePartTime> listaCodicePartTime)
        {
            listaCodicePartTime = new List<INPS.Pensioni.LiquidazioneFs.Entity.CodicePartTime>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaPartTime> listaCodiceDecodificaPartTimeDB = contenitoreDecodifica.ElencoCodiceDecodificaPartTime;
            if (listaCodiceDecodificaPartTimeDB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaPartTime decodificaCodeEsodoDB in listaCodiceDecodificaPartTimeDB)
                {
                    if (!decodificaCodeEsodoDB.Codice)
                        continue;

                    INPS.Pensioni.LiquidazioneFs.Entity.CodicePartTime codicePartTime = new INPS.Pensioni.LiquidazioneFs.Entity.CodicePartTime();
                    Utility.ValorizzaOggetti(decodificaCodeEsodoDB, codicePartTime);
                    listaCodicePartTime.Add(codicePartTime);
                }
            }
        }

        public static void GetListaCodiceArt22(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceArt22> listaCodiceArt22)
        {
            listaCodiceArt22 = new List<Entity.CodiceArt22>();
            List<Liquidazione.BLCommon.GestioneDecodifica.DecodificaCodiceArt22> listaCodiceDecodificaArt22DB = contenitoreDecodifica.ElencoCodiceDecodificaArt22;
            if (listaCodiceDecodificaArt22DB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.DecodificaCodiceArt22 decodificaCodeArt22DB in listaCodiceDecodificaArt22DB)
                {
                    LiquidazioneFs.Entity.CodiceArt22 codiceArt22 = new LiquidazioneFs.Entity.CodiceArt22();
                    Utility.ValorizzaOggetti(decodificaCodeArt22DB, codiceArt22);
                    listaCodiceArt22.Add(codiceArt22);
                }
            }
        }

        public static void GetListaCodiceCapitalizzazione(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<CodiceCapitalizzazione> listaCodiceCapitalizzazione)
        {
            listaCodiceCapitalizzazione = new List<CodiceCapitalizzazione>();
            List<GestioneDecodifica.DecodificaCodiceCapitalizzazione> listaCodiceDecodificaCodeCapitalizzazioneDB = contenitoreDecodifica.ElencoCodiceDecodificaCapitalizzazione;
            if (listaCodiceDecodificaCodeCapitalizzazioneDB != null)
            {
                foreach (GestioneDecodifica.DecodificaCodiceCapitalizzazione decodificaCodeCapitalizzazioneDB in listaCodiceDecodificaCodeCapitalizzazioneDB)
                {
                    CodiceCapitalizzazione codiceCapitalizzazion = new CodiceCapitalizzazione();
                    Utility.ValorizzaOggetti(decodificaCodeCapitalizzazioneDB, codiceCapitalizzazion);
                    listaCodiceCapitalizzazione.Add(codiceCapitalizzazion);
                }
            }
        }

        public static void GetListaCausaCessazione(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, out List<CausaCessazione> listaCausaCessazione)
        {
            char codNatura1, codNatura2, codNatura3;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            Utility.GetCodiciNatura(datiPensione.NaturaPensione, out codNatura1, out codNatura2, out codNatura3);

            listaCausaCessazione = new List<CausaCessazione>();
            List<GestioneDecodifica.DecodificaCausaCessazione> listaCodiceCausaCessazioneDB = contenitoreDecodifica.ElencoCodiceCausaCessazione;
            if (listaCodiceCausaCessazioneDB != null)
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.FS:
                            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001")    //Anzianità. Ritorniamo alla web solo i record con TipoPensione A
                                listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == tipoFondo.Value.ToString() && x.TipoPensione == 'A');
                            else if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")    //Vecchiaia. Ritorniamo alla web solo i record con TipoPensione V
                                listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == tipoFondo.Value.ToString() && x.TipoPensione == 'V');
                            else if (datiPensione.Gruppo == "0003")    //Reversibilità. Ritorniamo alla web solo i record con TipoPensione S
                                listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == tipoFondo.Value.ToString() && x.TipoPensione == 'S');
                            else if (datiPensione.Gruppo == "0002")    //Invalidità. Ritorniamo alla web solo i record con TipoPensione I
                                listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == tipoFondo.Value.ToString() && x.TipoPensione == 'I');
                            else
                                listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == "FS");
                            break;
                        case Utility.TipoFondo.PT:
                            listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == tipoFondo.Value.ToString());
                            break;
                    }
                }
                else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    if (
                        (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001") ||  //Anzianità. Ritorniamo alla web solo i record con TipoPensione A 
                        (datiPensione.Gruppo == "0031" && datiPensione.SiglaCategoria.StartsWith("V") && (codNatura1 == '1' || codNatura1 == '2'))
                       )
                        listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == "DAP" && x.TipoPensione == 'A');
                    else if (
                        (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002") ||   //Vecchiaia. Ritorniamo alla web solo i record con TipoPensione V
                        (datiPensione.Gruppo == "0031" && datiPensione.SiglaCategoria.StartsWith("V") && (codNatura1 == ' ' || codNatura1 == '6'))
                            )
                        listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == "DAP" && x.TipoPensione == 'V');
                    else if (
                        (datiPensione.Gruppo == "0003") ||   //Reversibilità. Ritorniamo alla web solo i record con TipoPensione S
                        (datiPensione.Gruppo == "0031" && datiPensione.SiglaCategoria.StartsWith("S"))
                            )
                        listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == "DAP" && x.TipoPensione == 'S');
                    else if (
                        (datiPensione.Gruppo == "0002") ||   //Invalidità. Ritorniamo alla web solo i record con TipoPensione I
                        (datiPensione.Gruppo == "0031" && datiPensione.SiglaCategoria.StartsWith("I"))
                            )
                        listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == "DAP" && x.TipoPensione == 'I');
                    else
                        listaCodiceCausaCessazioneDB = listaCodiceCausaCessazioneDB.FindAll(x => x.Fondo == "DAP");
                }


                foreach (GestioneDecodifica.DecodificaCausaCessazione decodificaCausaCessazioneDB in listaCodiceCausaCessazioneDB)
                {
                    CausaCessazione causaCessazione = new CausaCessazione();
                    Utility.ValorizzaOggetti(decodificaCausaCessazioneDB, causaCessazione);
                    listaCausaCessazione.Add(causaCessazione);
                }
            }
        }

        public static void GetListaCodiceEliminazione(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, GestionePensione.DatiEliminazione datiEliminazione, out List<CodiceEliminazione> listaCodiceEliminazione)
        {
            listaCodiceEliminazione = new List<CodiceEliminazione>();
            List<GestioneDecodifica.CodiceEliminazione> elencoCodiceEliminazioneDB = contenitoreDecodifica.ElencoCodiceEliminazione;
            if (elencoCodiceEliminazioneDB != null)
            {
                foreach (GestioneDecodifica.CodiceEliminazione codiceEliminazioneDB in elencoCodiceEliminazioneDB)
                {
                    if (Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria) == Utility.TipoFondo.PT ||
                        Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria) == Utility.TipoFondo.FS ||
                        Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    {
                        if (codiceEliminazioneDB.TraduzioneSuGP == '1' || codiceEliminazioneDB.TraduzioneSuGP == '3')
                        {
                            CodiceEliminazione codiceEliminazione = new CodiceEliminazione();
                            Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                            listaCodiceEliminazione.Add(codiceEliminazione);
                        }
                    }
                    else
                        if (codiceEliminazioneDB.TraduzioneSuGP == '2' || codiceEliminazioneDB.TraduzioneSuGP == '3' || (datiEliminazione != null && datiEliminazione.CodiceMotivo.HasValue && datiEliminazione.CodiceMotivo.ToString() == codiceEliminazioneDB.Id))
                        {
                            CodiceEliminazione codiceEliminazione = new CodiceEliminazione();
                            Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                            listaCodiceEliminazione.Add(codiceEliminazione);
                        }
                }
            }
        }

        public static void GetListaCodiceParticolare(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, out List<Entity.CodiceParticolare> listaCodiceParticolare)
        {
            listaCodiceParticolare = new List<Entity.CodiceParticolare>();
            List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareDB = contenitoreDecodifica.ElencoCodiceParticolare;
            if (elencoCodiceParticolareDB != null)
            {
                foreach (GestioneDecodifica.CodiceParticolare CodiceParticolareDB in elencoCodiceParticolareDB)
                {
                    Entity.CodiceParticolare codiceParticolare = new Entity.CodiceParticolare();
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
                    foreach (Entity.CodiceParticolare cP in listaCodiceParticolare)
                    {
                        if (cP.TraduzioneSuGp.HasValue && cP.TraduzioneSuGp.Value == 3 &&
                            !string.IsNullOrEmpty(cP.Descrizione) && cP.Descrizione.Contains('|'))
                            cP.Descrizione = cP.Descrizione.Substring(0, cP.Descrizione.IndexOf('|') - 1).Trim();
                    }
                }
                else if (Utility.IsDomandaSalvaguardia122(datiPensione))
                {
                    foreach (Entity.CodiceParticolare cP in listaCodiceParticolare)
                    {
                        if (cP.TraduzioneSuGp.HasValue && cP.TraduzioneSuGp.Value == 3 &&
                            !string.IsNullOrEmpty(cP.Descrizione) && cP.Descrizione.Contains('|'))
                            cP.Descrizione = cP.Descrizione.Substring(cP.Descrizione.IndexOf('|') + 1).Trim();
                    }
                }
                //nel caso di perdita titolo o salvaguardia 214 essendo il valore uguale e pari a 5, altero la descrizione
                //al fine di mostrare a video il corretto messaggio
                else if (Utility.IsDomandaSalvaguardia214(datiPensione))
                {
                    foreach (Entity.CodiceParticolare cP in listaCodiceParticolare)
                    {
                        if (cP.TraduzioneSuGp.HasValue && cP.TraduzioneSuGp.Value == 5 &&
                            !string.IsNullOrEmpty(cP.Descrizione) && cP.Descrizione.Contains('|'))
                            cP.Descrizione = cP.Descrizione.Substring(0, cP.Descrizione.IndexOf('|') - 1).Trim();
                    }
                }
                else if (Utility.IsDomandaVecchPerditaTitolo(datiPensione))
                {
                    foreach (Entity.CodiceParticolare cP in listaCodiceParticolare)
                    {
                        if (cP.TraduzioneSuGp.HasValue && cP.TraduzioneSuGp.Value == 5 &&
                            !string.IsNullOrEmpty(cP.Descrizione) && cP.Descrizione.Contains('|'))
                            cP.Descrizione = cP.Descrizione.Substring(cP.Descrizione.IndexOf('|') + 1).Trim();
                    }
                }
            }
        }

        public static void GetListaTipoLiquidazionePM(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.TipoLiquidazionePM> listaTipoLiquidazionePM)
        {
            listaTipoLiquidazionePM = new List<INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazionePM>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazionePM> listaTipoLiquidazionePMDB = contenitoreDecodifica.ElencoTipoLiquidazionePM;
            if (listaTipoLiquidazionePMDB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazionePM tipoLiquidazionePMDB in listaTipoLiquidazionePMDB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazionePM tipoLiquidazionePM = new INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazionePM();
                    Utility.ValorizzaOggetti(tipoLiquidazionePMDB, tipoLiquidazionePM);
                    listaTipoLiquidazionePM.Add(tipoLiquidazionePM);
                }
            }
        }

        public static void GetListaCodiceLegge413(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceLegge413> listaCodiceLegge413)
        {
            listaCodiceLegge413 = new List<INPS.Pensioni.LiquidazioneFs.Entity.CodiceLegge413>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaLegge413> listaCodiceLegge413DB = contenitoreDecodifica.ElencoCodiceLegge413;
            if (listaCodiceLegge413DB != null && listaCodiceLegge413DB.Count > 0)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaLegge413 codiceLegge413DB in listaCodiceLegge413DB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.CodiceLegge413 codiceLegge413 = new INPS.Pensioni.LiquidazioneFs.Entity.CodiceLegge413();
                    Utility.ValorizzaOggetti(codiceLegge413DB, codiceLegge413);
                    listaCodiceLegge413.Add(codiceLegge413);
                }
            }
        }

        public static void GetListaAttivitaSvolta2(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.AttivitaSvolta2> listaAttivitaSvolta2)
        {
            listaAttivitaSvolta2 = new List<INPS.Pensioni.LiquidazioneFs.Entity.AttivitaSvolta2>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaAttivitaSvolta2> listaAttivitaSvolta2DB = contenitoreDecodifica.ElencoAttivitaSvolta2;
            if (listaAttivitaSvolta2DB != null && listaAttivitaSvolta2DB.Count > 0)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaAttivitaSvolta2 attivitaSvolta2DB in listaAttivitaSvolta2DB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.AttivitaSvolta2 attivitaSvolta2 = new INPS.Pensioni.LiquidazioneFs.Entity.AttivitaSvolta2();
                    Utility.ValorizzaOggetti(attivitaSvolta2DB, attivitaSvolta2);
                    listaAttivitaSvolta2.Add(attivitaSvolta2);
                }
            }
        }

        public static void GetListaPersonaleViaggiante(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.PersonaleViaggiante> listaPersonaleViaggiante)
        {
            listaPersonaleViaggiante = new List<INPS.Pensioni.LiquidazioneFs.Entity.PersonaleViaggiante>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecPersonaleViaggiante> listaPersonaleViaggianteDB = contenitoreDecodifica.ElencoPersonaleViaggiante;
            if (listaPersonaleViaggianteDB != null && listaPersonaleViaggianteDB.Count > 0)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecPersonaleViaggiante personaleViaggianteDB in listaPersonaleViaggianteDB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.PersonaleViaggiante personaleViaggiante = new INPS.Pensioni.LiquidazioneFs.Entity.PersonaleViaggiante();
                    Utility.ValorizzaOggetti(personaleViaggianteDB, personaleViaggiante);
                    listaPersonaleViaggiante.Add(personaleViaggiante);
                }
            }
        }

        public static void GetListaDecEnteRipartizioneINPDAP(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.DecodificaEnteRipartizioneINPDAP> listaEnteRipartizioneINPDAP)
        {
            listaEnteRipartizioneINPDAP = new List<DecodificaEnteRipartizioneINPDAP>();
            List<GestioneDecodifica.DecodificaEnteRipartizioneINPDAP> listaDecodificaEnteRipartizioneINPDAP = contenitoreDecodifica.ElencoDecodificaEnteRipartizioneINPDAP;
            if (listaDecodificaEnteRipartizioneINPDAP != null && listaDecodificaEnteRipartizioneINPDAP.Count > 0)
            {
                foreach (var dec in listaDecodificaEnteRipartizioneINPDAP)
                {
                    DecodificaEnteRipartizioneINPDAP enteRipartizione = new DecodificaEnteRipartizioneINPDAP();
                    Utility.ValorizzaOggetti(dec, enteRipartizione);
                    listaEnteRipartizioneINPDAP.Add(enteRipartizione);
                }
            }
        }

        public static void GetListaDecMicroqualificaINPDAP(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, long? microqualificaDB, out List<MicroqualificaINPDAP> listaMicroqualificaINPDAP)
        {
            bool addMicroqualificaDB = true;
            listaMicroqualificaINPDAP = new List<MicroqualificaINPDAP>();
            List<GestioneDecodifica.DecMicroqualificaINPDAP> listaDecMicroqualificaNPDAP = contenitoreDecodifica.ElencoDecMicroqualificaNPDAP;
            if (listaDecMicroqualificaNPDAP != null && listaDecMicroqualificaNPDAP.Count > 0)
            {
                foreach (var dec in listaDecMicroqualificaNPDAP)
                {
                    MicroqualificaINPDAP microqualifica = new MicroqualificaINPDAP();
                    Utility.ValorizzaOggetti(dec, microqualifica);
                    listaMicroqualificaINPDAP.Add(microqualifica);
                    if (microqualificaDB == dec.Id)
                        addMicroqualificaDB = false;
                }
            }
            if (addMicroqualificaDB && microqualificaDB.HasValue)
            {
                GestioneDecodifica.DecMicroqualificaINPDAP decMicroqualifica = new GestioneDecodifica.DecMicroqualificaINPDAP();
                MicroqualificaINPDAP microqualifica = new MicroqualificaINPDAP();
                GestioneDecodifica.GetMicroqualificaById(microqualificaDB.Value, out decMicroqualifica);
                if (decMicroqualifica != null)
                {
                    Utility.ValorizzaOggetti(decMicroqualifica, microqualifica);
                    listaMicroqualificaINPDAP.Add(microqualifica);
                }
            }
        }

        public static void GetListaCtrlCompartoSettoreRuolo(string siglaCategoria, out List<Entity.CtrlCompartoSettoreRuolo> listaCompartoSettoreRuolo)
        {
            listaCompartoSettoreRuolo = new List<CtrlCompartoSettoreRuolo>();
            List<GestioneDecodifica.CtrlCompartoSettoreRuolo> listaCtrlCompartoSettoreRuolo = null;
            GestioneDecodifica.GetCtrlCompartoSettoreRuoloByCat(siglaCategoria, out listaCtrlCompartoSettoreRuolo);
            if (listaCtrlCompartoSettoreRuolo != null && listaCtrlCompartoSettoreRuolo.Count > 0)
            {
                foreach (var dec in listaCtrlCompartoSettoreRuolo)
                {
                    CtrlCompartoSettoreRuolo entity = new CtrlCompartoSettoreRuolo();
                    Utility.ValorizzaOggetti(dec, entity);
                    listaCompartoSettoreRuolo.Add(entity);
                }
            }
        }

        #endregion Dati Assicurativi

        #region Dati PrecedentePensione
        public static bool ControlDatiPrecedentePensione(Entity.DatiGenerici datiGenerici, Entity.DatiPrecedentePensione datiPrecedentePensione, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (datiPrecedentePensione == null)
                return true;
            if (datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value)
            {
                if (!datiPrecedentePensione.CodiceP18PrecedentePensione.HasValue)
                {
                    messaggioVideo = "Il campo 'Codice Categoria' è obbligatorio";
                    return false;
                }

                if (!datiPrecedentePensione.SedePrecedentePensione.HasValue)
                {
                    messaggioVideo = "Il campo 'Sede' è obbligatorio";
                    return false;
                }

                if (!Utility.ExistSedeProvinciale(datiPrecedentePensione.SedePrecedentePensione.Value))
                {
                    messaggioVideo = "La 'Sede' inserita non esiste";
                    return false;
                }

                if (!datiPrecedentePensione.CertificatoPrecedentePensione.HasValue)
                {
                    messaggioVideo = "Il campo 'Certificato' è obbligatorio";
                    return false;
                }
            }
            else if (datiPrecedentePensione.CodiceP18PrecedentePensione.HasValue ||
                datiPrecedentePensione.SedePrecedentePensione.HasValue ||
                datiPrecedentePensione.CertificatoPrecedentePensione.HasValue)
            {
                messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di procedere con il salvataggio dei dati della tab 'Precedente Pensione'";
                return false;
            }
            return true;
        }

        public static bool ControlDatiPrecedentePensioneINPDAP(Entity.DatiGenericiINPDAP datiGenerici, Entity.DatiPrecedentePensione datiPrecedentePensione, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (datiPrecedentePensione == null)
                return true;
            if (datiGenerici.TrasformazioneAOI.HasValue && datiGenerici.TrasformazioneAOI.Value)
            {
                if (!datiPrecedentePensione.CodiceP18PrecedentePensione.HasValue)
                {
                    messaggioVideo = "Il campo 'Codice Categoria' è obbligatorio";
                    return false;
                }

                if (!datiPrecedentePensione.SedePrecedentePensione.HasValue)
                {
                    messaggioVideo = "Il campo 'Sede' è obbligatorio";
                    return false;
                }

                if (!Utility.ExistSedeProvinciale(datiPrecedentePensione.SedePrecedentePensione.Value))
                {
                    messaggioVideo = "La 'Sede' inserita non esiste";
                    return false;
                }

                if (!datiPrecedentePensione.CertificatoPrecedentePensione.HasValue)
                {
                    messaggioVideo = "Il campo 'Certificato' è obbligatorio";
                    return false;
                }
            }
            else if (datiPrecedentePensione.CodiceP18PrecedentePensione.HasValue ||
                datiPrecedentePensione.SedePrecedentePensione.HasValue ||
                datiPrecedentePensione.CertificatoPrecedentePensione.HasValue)
            {
                messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di procedere con il salvataggio dei dati della tab 'Precedente Pensione'";
                return false;
            }
            return true;
        }

        public static void ValorizzaDatiPrecedentePensione(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out Entity.DatiPrecedentePensione datiPrecedentePensione)
        {
            datiPrecedentePensione = new INPS.Pensioni.LiquidazioneFs.Entity.DatiPrecedentePensione();

            if (datiIstruttoria != null)
                Utility.ValorizzaOggetti(datiIstruttoria, datiPrecedentePensione);

            if (datiPrecedentePensione.IsIstruttoriaNull())
                datiPrecedentePensione = null;
        }

        public static void StoreDatiPrecedentePensione(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Entity.DatiPrecedentePensione datiPrecedentePensione)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.EL:
                case Utility.TipoFondo.TT:
                case Utility.TipoFondo.ET:
                case Utility.TipoFondo.VL:
                case Utility.TipoFondo.FS:
                case Utility.TipoFondo.PT:
                case Utility.TipoFondo.GAS:
                    StoreDatiPrecedentePensionePrivate(ref contenitore, datiPensione, ref datiIstruttoria, datiPrecedentePensione);
                    break;
                case Utility.TipoFondo.PI:
                case Utility.TipoFondo.PL:
                    break;

            }
        }

        private static void StoreDatiPrecedentePensionePrivate(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Entity.DatiPrecedentePensione datiPrecedentePensione)
        {
            if (datiPrecedentePensione == null)
                datiPrecedentePensione = new INPS.Pensioni.LiquidazioneFs.Entity.DatiPrecedentePensione();

            long idFondo = contenitore.IdFondoPensione;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
             new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiPrecedentePensionePerIstruttoria(datiPensione.Id, datiPrecedentePensione, ref datiIstruttoria);
                if (datiPrecedentePensione.IsIstruttoriaNull())
                {
                    if (datiPensione.TrasformazioneAOI.GetValueOrDefault())
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 0;
                    else
                        datiQuadroLiquidazionePensione.TabPrecedentePensione = 1;
                }
                else
                    datiQuadroLiquidazionePensione.TabPrecedentePensione = 2;
                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiIstruttoria = datiIstruttoria;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;

        }

        public static void EliminaDatiPrecedentePensione(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            StoreDatiPrecedentePensionePrivate(ref contenitore, datiPensione, ref datiIstruttoria, new Entity.DatiPrecedentePensione());
        }
        #endregion Dati PrecedentePensione

        #region DatiBititolaritaInail

        public static void GetDatiBititolaritaInailByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out Entity.DatiBititolaritaInail DatiBititolaritaInail)
        {
            DatiBititolaritaInail = null;

            GestionePensioneInailInabilita.DatiInabilita datiInabilita = contenitore.DatiInabilita;
            List<GestionePensioneInailInabilita.DatiPensioniINAIL> LdatiInabilita = contenitore.ListaDatiPensioniINAIL;

            if (datiInabilita != null || (LdatiInabilita != null && LdatiInabilita.Count > 0))
            {
                DatiBititolaritaInail = new Entity.DatiBititolaritaInail();

                if (LdatiInabilita != null && LdatiInabilita.Count > 0)
                {
                    DatiBititolaritaInail.LpensioniInail = new List<Entity.DatiBititolaritaInail.PensioniInail>();
                    foreach (GestionePensioneInailInabilita.DatiPensioniINAIL pi in LdatiInabilita)
                    {
                        Entity.DatiBititolaritaInail.PensioniInail pensioniInail = new Entity.DatiBititolaritaInail.PensioniInail();
                        Utility.ValorizzaOggetti(pi, pensioniInail);
                        DatiBititolaritaInail.LpensioniInail.Add(pensioniInail);
                    }
                }
                if (datiInabilita != null)
                    Utility.ValorizzaOggetti(datiInabilita, DatiBititolaritaInail);
            }
        }

        public static void StoreDatiBititolaritaInail(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, Entity.DatiBititolaritaInail datiDatiBititolaritaInail)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                    case Utility.TipoFondo.GAS:
                    case Utility.TipoFondo.PM:
                        StoreDatiBititolaritaInailPrivate(ref contenitore, datiPensione, datiDatiBititolaritaInail);
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        break;
                }
            }
            else
            {
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    StoreDatiBititolaritaInailPrivate(ref contenitore, datiPensione, datiDatiBititolaritaInail);
            }
        }

        private static void StoreDatiBititolaritaInailPrivate(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, Entity.DatiBititolaritaInail datiDatiBititolaritaInail)
        {
            if (datiDatiBititolaritaInail != null)
            {
                GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;
                GestionePensioneInailInabilita.DatiInabilita datiInabilita = new GestionePensioneInailInabilita.DatiInabilita();

                Utility.ValorizzaOggetti(datiDatiBititolaritaInail, datiInabilita);
                datiInabilita.IdPensione = datiPensione.Id;

                List<GestionePensioneInailInabilita.DatiPensioniINAIL> LDatiPensioniINAIL = null;

                if (datiDatiBititolaritaInail.LpensioniInail != null && datiDatiBititolaritaInail.LpensioniInail.Count > 0)
                {
                    LDatiPensioniINAIL = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();
                    foreach (Entity.DatiBititolaritaInail.PensioniInail pi in datiDatiBititolaritaInail.LpensioniInail)
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
                        datiQuadroLiquidazionePensione.TabInail = 1;
                    else
                        datiQuadroLiquidazionePensione.TabInail = 2;

                    GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                    transactionScope.Complete();
                }

                /* --- AGGIORNO I DATI SUL CONTENITORE --- */
                contenitore.DatiInabilita = datiInabilita;
                contenitore.ListaDatiPensioniINAIL = LDatiPensioniINAIL;
                contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            }
        }

        public static void EliminaDatiBititolaritaInail(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensioneInailInabilita.EliminaInabilita(contenitore.DatiPensione.Id);
                GestionePensioneInailInabilita.EliminaPensioniINAILByIdPensione(contenitore.DatiPensione.Id);
                datiQuadroLiquidazionePensione.TabInail = 1;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(contenitore.DatiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiInabilita = null;
            contenitore.ListaDatiPensioniINAIL = null;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
        }

        public static bool ControlDatiBititolaritaInail(Entity.DatiBititolaritaInail datiBititolaritaInail, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiBititolaritaInail != null && (!datiBititolaritaInail.DecorrenzaDirittoIntegrazioneMinimo.HasValue && datiBititolaritaInail.CessazioneDirittoIntegrazioneMinimo.HasValue))
            {
                messaggioVideo = "La data Decorrenza Cessazione prevede la presenza della data Decorrenza Diritto";
                return false;
            }

            if (datiBititolaritaInail != null && (datiBititolaritaInail.DecorrenzaDirittoIntegrazioneMinimo.HasValue && datiBititolaritaInail.CessazioneDirittoIntegrazioneMinimo.HasValue))
            {
                if (datiBititolaritaInail.CessazioneDirittoIntegrazioneMinimo.Value < datiBititolaritaInail.DecorrenzaDirittoIntegrazioneMinimo.Value)
                {
                    messaggioVideo = "La data Decorrenza Cessazione deve essere maggiore o uguale alla data Decorrenza Diritto";
                    return false;
                }
            }

            if (datiBititolaritaInail != null && (!datiBititolaritaInail.SospensionePensioneInvalidita.HasValue && datiBititolaritaInail.RipristinoPensioneInvalidita.HasValue))
            {
                messaggioVideo = "La data di Ripristino prevede la presenza della data di Sospensione";
                return false;
            }

            if (datiBititolaritaInail != null && (!datiBititolaritaInail.DirittoAssegnoAccompagnamento.HasValue && datiBititolaritaInail.DecorrenzaAssegnoAccompangamento.HasValue ||
                                                  datiBititolaritaInail.DirittoAssegnoAccompagnamento.HasValue && !datiBititolaritaInail.DecorrenzaAssegnoAccompangamento.HasValue))
            {
                messaggioVideo = "Il Diritto all'assegno d'accompagnamento prevede la presenza della decorrenza e viceversa";
                return false;
            }
            return true;
        }

        #endregion DatiBititolaritaInail

        #region Dati Legge 4/60

        public static void GetDatiLegge460ByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out Entity.DatiLegge460 datiLegge460)
        {
            datiLegge460 = null;

            GestioneFondo.DatiFondoPT datiFondoPT = contenitore.DatiFondoPT;
            if (datiFondoPT != null)
            {
                datiLegge460 = new Entity.DatiLegge460();
                Utility.ValorizzaOggetti(datiFondoPT, datiLegge460);
                datiLegge460.NCertificato = datiFondoPT.Ncertificato != 0 ? datiFondoPT.Ncertificato.ToString() : string.Empty;
            }
        }

        public static void StoreDatiLegge460(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo, Entity.DatiLegge460 datiLegge460)
        {
            if (datiLegge460 == null)
                datiLegge460 = new Entity.DatiLegge460();

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool isDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione);

            #region gestione TipoFondo
            List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.PT:
                        listaDatiFondoPT = contenitore.ListaDatiFondoPT;
                        break;
                }
            }
            #endregion gestione TipoFondo

            long idFondo = 0;

            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiLegge460PerFondoDatiGenerici(datiPensione.Id, ref datiFondo);
                if (datiFondo != null)
                    idFondo = datiFondo.Id;

                StoreDatiLegge460Private(datiPensione.Id, idFondo, datiLegge460, isDomandaConNuovaGestioneDatiFondoFSPT, ref datiFondo, ref listaDatiFondoPT);

                if (!datiLegge460.IsDatiLegge460Null())
                    datiQuadroLiquidazionePensione.TabDatiLegge460 = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiFondo = datiFondo;
            contenitore.ListaDatiFondoPT = listaDatiFondoPT;
            contenitore.DatiFondoPT = (listaDatiFondoPT != null && listaDatiFondoPT.Count > 0) ? listaDatiFondoPT.First() : null;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
        }

        public static void EliminaDatiLegge460(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo)
        {
            List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = contenitore.ListaDatiFondoPT;
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = contenitore.DatiQuadroLiquidazionePensione;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (listaDatiFondoPT != null && listaDatiFondoPT.Count() > 0)
                {
                    Entity.DatiLegge460 datiLegge460 = new Entity.DatiLegge460();
                    Utility.ValorizzaOggetti(datiLegge460, listaDatiFondoPT.First());
                    listaDatiFondoPT.First().Ncertificato = null;

                    if (listaDatiFondoPT.First().Equals(new GestioneFondo.DatiFondoPT()))
                    {
                        GestioneFondo.EliminaFondoPT(datiPensione.Id);
                        listaDatiFondoPT = null;
                    }
                    else
                    {
                        GestioneFondo.SalvaFondoPT(listaDatiFondoPT.First().IdFondo, listaDatiFondoPT.First());
                    }
                }

                if (datiFondo != null)
                {
                    if (datiFondo.IsFondoNull())
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        datiFondo = null;
                    }
                }

                datiQuadroLiquidazionePensione.TabDatiLegge460 = 1;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.ListaDatiFondoPT = listaDatiFondoPT;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.DatiFondo = datiFondo;
        }

        public static bool ControlDatiLegge460(Entity.DatiLegge460 datiLegge460, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiLegge460 != null)
            {
                if (!((datiLegge460.SiglaCategoria.HasValue && datiLegge460.CodiceSede.HasValue && !string.IsNullOrEmpty(datiLegge460.NCertificato) &&
                       datiLegge460.DecorrenzaSecondaria.HasValue && datiLegge460.NMesiRiscattati.HasValue && datiLegge460.NMesiTotali.HasValue) ||
                      (!datiLegge460.SiglaCategoria.HasValue && !datiLegge460.CodiceSede.HasValue && string.IsNullOrEmpty(datiLegge460.NCertificato) &&
                       !datiLegge460.DecorrenzaSecondaria.HasValue && !datiLegge460.NMesiRiscattati.HasValue && !datiLegge460.NMesiTotali.HasValue)))
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

        #endregion Dati Legge 4/60

        #region Dati Storico
        public static void GetDatiLiquidazionePensioneStorico(ref EntityBLCommon.ContenitoreObject contenitore, out Entity.DatiLiquidazionePensioneStorico datiLiquidazionePensioneStorico)
        {
            datiLiquidazionePensioneStorico = null;

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;

            if (datiStoricoGP != null)
            {
                datiLiquidazionePensioneStorico = new DatiLiquidazionePensioneStorico();
                Utility.ValorizzaOggetti(datiStoricoGP, datiLiquidazionePensioneStorico);
            }
        }
        #endregion Dati Storico

        #region Dati Istruttoria

        public static void GetDatiIsruttoriaINPDAPByIdPensione(GestioneFondo.DatiFondo datiFondo, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, out DatiIstruttoriaINPDAP datiIstruttoriaINPDAP)
        {
            datiIstruttoriaINPDAP = new DatiIstruttoriaINPDAP();

            Utility.ValorizzaOggetti(datiFondo, datiIstruttoriaINPDAP);
            Utility.ValorizzaOggetti(datiIstruttoriaCommon, datiIstruttoriaINPDAP);

            if (datiIstruttoriaINPDAP.IsDatiIstruttoriaIstruttoriaNull() && datiIstruttoriaINPDAP.IsDatiIstruttoriaPensioneFondoDatiGenericiNull())
                datiIstruttoriaINPDAP = null;
        }

        public static void StoreDatiIstruttoriaINPDAP(GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            ref GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione, DatiIstruttoriaINPDAP datiIstruttoriaINPDAP)
        {
            if (datiIstruttoriaINPDAP == null)
                datiIstruttoriaINPDAP = new DatiIstruttoriaINPDAP();

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiIstruttoriaINPDAPPerPensioneFondoDatiGenerici(datiPensione.Id, datiIstruttoriaINPDAP, ref datiFondo);
                StoreDatiIstruttoriaINPDAPPerIstruttoria(datiPensione.Id, datiIstruttoriaINPDAP, ref datiIstruttoria);

                datiQuadroLiquidazionePensione.TabIstruttoria = 2;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiIstruttoriaINPDAP(GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            ref GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiIstruttoriaINPDAPPerPensioneFondoDatiGenerici(datiPensione.Id, new DatiIstruttoriaINPDAP(), ref datiFondo);
                StoreDatiIstruttoriaINPDAPPerIstruttoria(datiPensione.Id, new DatiIstruttoriaINPDAP(), ref datiIstruttoria);

                datiQuadroLiquidazionePensione.TabIstruttoria = 1;

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        public static bool ControlDatiIstruttoriaINPDAP(DatiIstruttoriaINPDAP datiIstruttoria, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiIstruttoria.RiduzioneRetributiva,
                datiIstruttoria.RiduzioneRetributivaPercentuale, out messaggioVideo))
                return false;

            return true;
        }

        #endregion Dati Istruttoria

        public static Dictionary<string, bool?> GetCrossProperties(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, GestioneFondo.DatiFondo datiFondo,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, object datiFondoXX, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, bool isRiaperturaDomanda, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoFondo? tipoFondo, GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP,
            out Utility.CategoriaFondoPI? catFondoPI, out TipoSalvaguardia? TipologiaSalvaguardia, out DateTime? DecorrenzaPensioneDirettaDC,
            out Dictionary<string, char?> TipoPensione, out DateTime? dataPrelievoDomanda, out char? tipoReversibilita)
        {
            bool? IsEsenzioneFiscaleEstero = null;
            bool? IsResidenteEstero = null;
            bool? IsEsenzioneFiscaleVittima = null;
            bool? IsCodiceSpecificoVisible = null;
            bool? IsCodiceSpecificoEnabled = null;
            bool? IsCodiceArt22Enabled = null;
            bool? IsRequisitiL247_L243Enable = null;
            TipologiaSalvaguardia = null;
            bool? IsVisibleArt2 = null;
            bool? IsDecPensAnteAgosto95 = null;
            bool? IsCodiceNatura2Enabled = null;
            bool? IsUsuranti = null;
            bool? IsVecchPerditaTitolo = null;
            catFondoPI = null;
            bool? IsDomandaTrasformazioneAOI = null;
            DecorrenzaPensioneDirettaDC = null;
            bool? IsCodDirittoQuoteFisseVisible = null;
            bool? IsIndennitaAggiuntivaVisible = null;
            TipoPensione = null;
            bool? isDecorrenzaSuccSett1989;
            bool? isCodiceComunicazione3Visible = null;
            bool? isProvvisoriaVisible = null;
            bool? isCodiceNatura2DisabledPerSperDonna = null;
            bool? IsDomandaConNuovaGestioneDatiFondoFSPT = null;
            // DPR Armonizzazione
            bool? IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante = null;
            bool? IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL = null;
            bool? IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante = null;
            bool? IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL = null;
            //-----------------------
            bool? isDomandaAnteArmonizzazione = null;
            bool? isCapitalizzazioneVisible = null;
            bool? isTrimestreAnzianitaRequisitiNoInvaliditaVisible = null;
            bool? isBeneficioArt24Comma15BisFromFELPE = null;
            bool? isPensioneTipoContributivo = null;
            bool? isPensioneTipoContributivoConOpzione = null;
            bool? isSperimentaleDonna = null;
            bool? isRiduzioneRetributiva = null;
            bool? isRiduzioneRetributivaEnabled = null;
            bool? isBeneficioApePrecociFromFELPE = null;
            bool? isEsenzioneFiscaleEsteroFromDetrazioni = null;
            bool? isReversibilitaOrRicostituzione = null;
            bool? isRicostituzioneForMemo72 = null;
            bool? isRichiestaBonusBookingAbilitata = null;
            bool? isPrimoVersamentoNonObbligatorio = null;
            bool? isBeneficioNonVedente = null;
            bool? isDataRinunciaTrattenutaInpdapStorico = null;
            bool? isBeneficioNonVedenteFromStorico = null;
            bool? isRichiestaBonus154Abilitata = null;
            bool? isCodComunicazioniEsenzioneFiscaleVittimaVisibile = null;
            bool? isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = null;
            bool? isSenzaLegge33670 = null;
            //ENG - Aggiornamento Memo86
            bool? isPresenteTrattenutaFondoCreditoDaPrelievo = null;
            dataPrelievoDomanda = null;
            tipoReversibilita = null;
            bool? isMiglioramentiContrattualiAutomatici = null;
            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();

            char? derogaTraduzioneSuGP = null;
            if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
            {
                List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolareSoggettoDerogato = contenitoreDecodifica.ElencoCodiceParticolare;
                if (elencoCodiceParticolareSoggettoDerogato != null && elencoCodiceParticolareSoggettoDerogato.Count > 0)
                {
                    GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiceParticolareSoggettoDerogato.Find(x => x.Id == datiIstruttoria.CodiceParticolareSoggettoDerogato.Value);
                    if (codiceParticolare != null)
                        derogaTraduzioneSuGP = codiceParticolare.TraduzioneSuGp;
                }
            }
            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                IsEsenzioneFiscaleEstero = Utility.IsEsenzioneFiscaleEsteroINPDAP(datiAnagrafici.CodiceComuneResidenza);
            }
            else
            {
                IsEsenzioneFiscaleEstero = Utility.IsEsenzioneFiscaleEstero(datiPensione, datiAnagrafici.CodiceComuneResidenza, datiDetrazioni, isRiaperturaDomanda);   // generici
            }
            IsResidenteEstero = Utility.IsResidenteEstero(datiAnagrafici.CodiceComuneResidenza);
            IsEsenzioneFiscaleVittima = Utility.IsEsenzioneFiscaleVittima(datiPensione, null, datiDetrazioni, isRiaperturaDomanda);
            IsCodiceSpecificoVisible = GetIsCodiceSpecificoVisible(datiPensione);    // assicurativi
            IsRequisitiL247_L243Enable = GetIsRequisitiL247_L243Enable(datiPensione); // generici
            TipologiaSalvaguardia = GetTipoSalvaguardia(datiPensione); // generici
            IsVisibleArt2 = GetIsVisibleArt2(datiPensione);              // generici
            IsDecPensAnteAgosto95 = GetIsDecPensAnteAgosto95(datiPensione, datiDanteCausa);      // assicurativi
            catFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);           // generici, assicurativi
            IsCodiceNatura2Enabled = ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "Y") ? false : GestioneCrossControls.IsCodiceNatura2Enabled(datiPensione);  // generici
            IsUsuranti = Utility.IsDomandaUsuranti(datiPensione);
            IsVecchPerditaTitolo = Utility.IsDomandaVecchPerditaTitolo(datiPensione);
            IsCodiceSpecificoEnabled = GetIsCodiceSpecificoEnable(ref contenitore, datiPensione, isRiaperturaDomanda);    // assicurativi
            IsCodiceArt22Enabled = GetIsCodiceArt22Enable(ref contenitore, datiPensione, isRiaperturaDomanda);    // assicurativi
            IsDomandaTrasformazioneAOI = Utility.IsDomandaINPDAP(datiPensione.Gestione) ? false : Utility.IsDomandaTrasformazioneAOI(datiPensione); // generici
            DecorrenzaPensioneDirettaDC = GetDecorrenzaPensioneDirettaDC(datiPensione, datiDanteCausa, contenitore.DatiLavorazione);
            IsCodDirittoQuoteFisseVisible = GetIsCodDirittoQuoteFisseVisible(datiPensione, DecorrenzaPensioneDirettaDC); // assicurativi
            IsIndennitaAggiuntivaVisible = GetIsIndennitaAggiuntivaVisible(datiPensione, DecorrenzaPensioneDirettaDC); // assicurativi
            TipoPensione = GetTipoPensione(datiPensione);
            
            isCodiceComunicazione3Visible = IsCodiceComunicazione3Visible(datiPensione, datiIstruttoria);
            isProvvisoriaVisible = IsProvvisoriaVisible(datiPensione, datiIstruttoria);
            isCodiceNatura2DisabledPerSperDonna = IsCodiceNatura2DisabledPerSperDonna(datiPensione);
            IsDomandaConNuovaGestioneDatiFondoFSPT = Utility.IsDomandaConNuovaGestioneDatiFondoFSPT(datiPensione); //utile solo per FS e PT
            // DPR Armonizzazione
            IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante = Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione);
            IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL = Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL(datiPensione);
            IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante = Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione);
            IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL = Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL(datiPensione);
            //----------------------
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, DecorrenzaPensioneDirettaDC != null ? DecorrenzaPensioneDirettaDC : null);
            isDomandaAnteArmonizzazione = Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondoXX: datiFondoXX, datiFondo: datiFondo);
            isCapitalizzazioneVisible = IsCapitalizzazioneVisible(datiPensione);
            isTrimestreAnzianitaRequisitiNoInvaliditaVisible = Utility.IsDomandaSalvaguardia122_FS_2011_2012(datiPensione, derogaTraduzioneSuGP);
            isBeneficioArt24Comma15BisFromFELPE = datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE : null;
            //----------------------
            isPensioneTipoContributivo = Utility.IsDomandaTipoContributivo(datiPensione, null, null);
            isPensioneTipoContributivoConOpzione = Utility.IsDomandaTipoContributivo(datiPensione, null, true);
            isSperimentaleDonna = Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione);
            isRiduzioneRetributiva = GestioneRiduzioneRetributiva(datiPensione, isRiaperturaDomanda);   // istruttoria
            isRiduzioneRetributivaEnabled = Utility.GestioneRiduzioneRetributivaEnabled(datiPensione, isRiaperturaDomanda, null, null); // istruttoria
            isBeneficioApePrecociFromFELPE = datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE : null;
            isEsenzioneFiscaleEsteroFromDetrazioni = Utility.IsEsenzioneFiscaleEsteroFromDetrazioni(datiPensione, datiDetrazioni, isRiaperturaDomanda);
            isReversibilitaOrRicostituzione = Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, contenitore.DatiLavorazione);
            isDecorrenzaSuccSett1989 = decorrenzaPensioneOrDecorrenzaPensioneDC.HasValue && Utility.DataSuccessivaA(decorrenzaPensioneOrDecorrenzaPensioneDC.Value, new DateTime(1989, 10, 1)) ? true : false;

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneControlliMemo72", out ctrl);
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                char? codiceSpecificoTraduzioneSuGP = null;
                if (datiFondo != null && datiFondo.CodiceSpecifico.HasValue)
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = contenitoreDecodifica.ElencoCodiceSpecifico;
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondo.CodiceSpecifico.Value);
                        if (codice != null)
                            codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                    }
                }
                isRicostituzioneForMemo72 = (codiceSpecificoTraduzioneSuGP != null && (((tipoFondo == Utility.TipoFondo.VL && codiceSpecificoTraduzioneSuGP == 'E') || (tipoFondo == Utility.TipoFondo.ET && (codiceSpecificoTraduzioneSuGP == 'Q' || codiceSpecificoTraduzioneSuGP == 'I')) || ((tipoFondo == Utility.TipoFondo.TT || tipoFondo == Utility.TipoFondo.EL) && codiceSpecificoTraduzioneSuGP == 'Q')) ||
                                             ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && codiceSpecificoTraduzioneSuGP == 'F')) &&
                                             ((Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.NCertificato.HasValue && (datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "2" || datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "5"))) &&
                                             Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1984, 7, 31)) && datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue && Utility.DataStrettamenteSuccessivaA(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value, new DateTime(2020, 7, 31)) &&
                                             datiAnagrafici != null && !Utility.DataStrettamenteSuccessivaA(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value, datiAnagrafici.DataNascita.Value.AddYears(60)));

            }

            ctrl = null;
            if (Utility.IsBonusBooking(contenitore.DatiPensione))
            {
                GestioneControlliDinamici.ControlloDinamico sediDaControllare = null;

                if (contenitore.DatiPensione.Tipo == "0167") //BONUS 14° 
                {
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingFS", out ctrl);
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingSediFS", out sediDaControllare);

                    if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                        (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                         sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == Utility.GetCodiceSedeLavorazione(datiPensione, isRiaperturaDomanda).ToString().PadLeft(4, '0')))))
                    {
                        isRichiestaBonusBookingAbilitata = true;
                    }
                }
                else //BONUS 154
                {
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154FS", out ctrl);
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154SediFS", out sediDaControllare);

                    if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                        (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                         sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == Utility.GetCodiceSedeLavorazione(datiPensione, isRiaperturaDomanda).ToString().PadLeft(4, '0')))))
                    {
                        isRichiestaBonus154Abilitata = true;
                    }
                }
            }

            isPrimoVersamentoNonObbligatorio = Utility.IsRicostituzione_MotiviContributivi(datiPensione) && GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.PRIMO_VERSAMENTO_FONDO_FS);

            if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01")
                isBeneficioNonVedente = true;

            if (datiStoricoGP != null && datiStoricoGP.DataRinunciaTrattenutaInpdap.HasValue)
                isDataRinunciaTrattenutaInpdapStorico = true;

            if (datiStoricoGP != null && !string.IsNullOrEmpty(datiStoricoGP.TipoSettimaneBeneficio) && datiStoricoGP.TipoSettimaneBeneficio == "01")
                isBeneficioNonVedenteFromStorico = true;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Abilita_Cod_Comunicazioni.ABILITA_COD_COMUNICAZIONI)
                || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || (datiIstruttoria != null && datiIstruttoria.CodiceComunicazioneCampo4 == 1 ))
                isCodComunicazioniEsenzioneFiscaleVittimaVisibile = true;

            isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) ||
                Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione);

            if (datiMaggiorazioniBenefici != null && (datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue || datiMaggiorazioniBenefici.RMSSenzaLegge33670QB.HasValue || datiMaggiorazioniBenefici.PercentualeMaggiorazioneSenzaLegge33670.HasValue))
            {
                isSenzaLegge33670 = true;
            }

            //ENG - Aggiornamento Memo86
            if (contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.TrattenutaFondoCredito.HasValue)
                isPresenteTrattenutaFondoCreditoDaPrelievo = contenitore.DatiStoricoGP.TrattenutaFondoCredito.Value;
            else
                isPresenteTrattenutaFondoCreditoDaPrelievo = null;

            GestioneLogSoap.GetTimestampMinimo(contenitore.DatiPensione.NDomus, out dataPrelievoDomanda);

            tipoReversibilita = contenitore.DatiLavorazione != null ? contenitore.DatiLavorazione.TipoReversibilita : null;

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> datiQuoteMiglioramentiContrattuali = null;
                GestioneMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiByIdPensione(datiPensione.Id, out datiQuoteMiglioramentiContrattuali);
                if(datiQuoteMiglioramentiContrattuali != null && datiQuoteMiglioramentiContrattuali.Count > 0)
                    isMiglioramentiContrattualiAutomatici = true;
            }

            lReturn.Add("IsEsenzioneFiscaleEstero", IsEsenzioneFiscaleEstero);
            lReturn.Add("IsResidenteEstero", IsResidenteEstero);
            lReturn.Add("IsEsenzioneFiscaleVittima", IsEsenzioneFiscaleVittima);
            lReturn.Add("CodiceSpecificoVisible", IsCodiceSpecificoVisible);
            lReturn.Add("RequisitiL247_L243Enable", IsRequisitiL247_L243Enable);
            lReturn.Add("Articolo2", IsVisibleArt2);
            lReturn.Add("DecPensAnteAgosto95", IsDecPensAnteAgosto95);
            lReturn.Add("IsCodiceNatura2Enabled", IsCodiceNatura2Enabled);
            lReturn.Add("Usuranti", IsUsuranti);
            lReturn.Add("VecchPerditaTitolo", IsVecchPerditaTitolo);
            lReturn.Add("CodiceSpecificoEnabled", IsCodiceSpecificoEnabled);
            lReturn.Add("CodiceArt22Enabled", IsCodiceArt22Enabled);
            lReturn.Add("DomandaTrasformazioneAOI", IsDomandaTrasformazioneAOI);
            lReturn.Add("IsCodDirittoQuoteFisseVisible", IsCodDirittoQuoteFisseVisible);
            lReturn.Add("IsIndennitaAggiuntivaVisible", IsIndennitaAggiuntivaVisible);
            lReturn.Add("IsDecorrenzaSuccSett1989", isDecorrenzaSuccSett1989);
            lReturn.Add("IsCodiceComunicazione3Visible", isCodiceComunicazione3Visible);
            lReturn.Add("IsProvvisoriaVisible", isProvvisoriaVisible);
            lReturn.Add("IsCodiceNatura2DisabledPerSperDonna", isCodiceNatura2DisabledPerSperDonna);
            lReturn.Add("IsDomandaConNuovaGestioneDatiFondoFSPT", IsDomandaConNuovaGestioneDatiFondoFSPT);
            //DPR Armonizzazione
            lReturn.Add("IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante", IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante);
            lReturn.Add("IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL", IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL);
            lReturn.Add("IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante", IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante);
            lReturn.Add("IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL", IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL);
            // ------------------
            lReturn.Add("IsDomandaAnteArmonizzazione", isDomandaAnteArmonizzazione);
            lReturn.Add("IsCapitalizzazioneVisible", isCapitalizzazioneVisible);
            lReturn.Add("IsTrimestreAnzianitaRequisitiNoInvaliditaVisible", isTrimestreAnzianitaRequisitiNoInvaliditaVisible);
            lReturn.Add("IsBeneficioArt24Comma15BisFromFELPE", isBeneficioArt24Comma15BisFromFELPE);
            // ------------------
            lReturn.Add("IsPensioneTipoContributivo", isPensioneTipoContributivo);
            lReturn.Add("IsPensioneTipoContributivoConOpzione", isPensioneTipoContributivoConOpzione);
            lReturn.Add("IsSperimentaleDonna", isSperimentaleDonna);
            lReturn.Add("IsRiduzioneRetributiva", isRiduzioneRetributiva);
            lReturn.Add("IsRiduzioneRetributivaEnabled", isRiduzioneRetributivaEnabled);
            lReturn.Add("IsBeneficioApePrecociFromFELPE", isBeneficioApePrecociFromFELPE);
            lReturn.Add("IsEsenzioneFiscaleEsteroFromDetrazioni", isEsenzioneFiscaleEsteroFromDetrazioni);
            lReturn.Add("IsReversibilitaOrRicostituzione", isReversibilitaOrRicostituzione);
            lReturn.Add("IsRicostituzioneForMemo72", isRicostituzioneForMemo72);
            lReturn.Add("IsRichiestaBonusBookingAbilitata", isRichiestaBonusBookingAbilitata);
            lReturn.Add("IsPrimoVersamentoNonObbligatorio", isPrimoVersamentoNonObbligatorio);
            lReturn.Add("IsBeneficioNonVedente", isBeneficioNonVedente);
            lReturn.Add("IsDataRinunciaTrattenutaInpdapStorico", isDataRinunciaTrattenutaInpdapStorico);
            lReturn.Add("IsBeneficioNonVedenteFromStorico", isBeneficioNonVedenteFromStorico);
            lReturn.Add("IsRichiestaBonus154Abilitata", isRichiestaBonus154Abilitata);
            lReturn.Add("IsCodComunicazioniEsenzioneFiscaleVittimaVisibile", isCodComunicazioniEsenzioneFiscaleVittimaVisibile);
            lReturn.Add("IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione", isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione);
            lReturn.Add("isSenzaLegge33670", isSenzaLegge33670);
            //ENG - Aggiornamento Memo86
            lReturn.Add("IsPresenteTrattenutaFondoCreditoDaPrelievo", isPresenteTrattenutaFondoCreditoDaPrelievo);
            lReturn.Add("IsMiglioramentiContrattualiAutomatici", isMiglioramentiContrattualiAutomatici); 
            return lReturn;
        }

        public static Dictionary<string, char?> GetTipoPensione(GestionePensione.DatiPensione datiPensione)
        {
            Dictionary<string, char?> tipoPensione = new Dictionary<string, char?>();
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (datiPensione.SiglaCategoria.StartsWith("V"))
            {
                tipoPensione.Add("VECCHIAIA", '1');
                return tipoPensione;
            }
            if (datiPensione.SiglaCategoria.StartsWith("I"))
            {
                tipoPensione.Add("INVALIDITA'", '2');
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
                        tipoPensione.Add("VECCHIAIA", '1');
                        return tipoPensione;
                    }
                    if (danteCausa.SiglaCategoria.StartsWith("I"))
                    {
                        tipoPensione.Add("INVALIDITA'", '2');
                        return tipoPensione;
                    }
                }
                else
                {
                    GestioneLavorazione.DatiLavorazione datiLavorazione = null;
                    GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
                    if (Utility.IsRicostituzione(datiPensione.Gruppo) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)
                        && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione))
                    {
                        short? tipologiaPensione = null;
                        if (tipoFondo == Utility.TipoFondo.FS)
                        {
                            List<GestioneFondo.DatiFondoFST> listaPensioneFondoFS = null;
                            GestioneFondo.GetFondoFSRecordFondoByIdPensione(datiPensione.Id, out listaPensioneFondoFS);
                            if (listaPensioneFondoFS != null && listaPensioneFondoFS.Count > 0)
                                tipologiaPensione = listaPensioneFondoFS[0].TipologiaPensione;
                        }
                        else if (tipoFondo == Utility.TipoFondo.PT)
                        {
                            List<GestioneFondo.DatiFondoPT> listaPensioneFondoPT = null;
                            GestioneFondo.GetFondoPTRecordFondoByIdPensione(datiPensione.Id, out listaPensioneFondoPT);
                            if (listaPensioneFondoPT != null && listaPensioneFondoPT.Count > 0)
                                tipologiaPensione = listaPensioneFondoPT[0].TipologiaPensione;
                        }

                        if (tipologiaPensione.HasValue)
                        {
                            if (datiPensione.Isola != null && datiPensione.Isola == 1)// Isola utilizzato per taggare le migrate con XFSCSPEC = A. Impostiamo 1 per la gestione del TipoSpecifico
                                tipoPensione.Add("REVERSIBILITA'", '3');
                            else
                            {
                                char tipoPens = ' ';
                                Char.TryParse(tipologiaPensione.ToString(), out tipoPens);
                                tipoPensione.Add("REVERSIBILITA'", tipoPens);
                            }
                        }
                        else
                            tipoPensione.Add("REVERSIBILITA'", ' ');
                    }
                    else
                        tipoPensione.Add("INDIRETTA", '3');
                }
                return tipoPensione;
            }

            return null;
        }

        private static bool? GetIsCodiceSpecificoEnable(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            Entity.DatiAssicurativi datiAssicurativi = null;
            bool disableCodSpecifico = false;
            bool disableCodArt22 = false;
            PrevalorizzazioneDatiAssicurativi(ref contenitore, Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria), datiPensione, isRiaperturaDomanda, ref datiAssicurativi,
                out disableCodSpecifico, out disableCodArt22);
            if (disableCodSpecifico)
                return false;

            return true;
        }

        private static bool? GetIsCodiceArt22Enable(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            Entity.DatiAssicurativi datiAssicurativi = null;
            bool disableCodSpecifico = false;
            bool disableCodArt22 = false;
            PrevalorizzazioneDatiAssicurativi(ref contenitore, Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria), datiPensione, isRiaperturaDomanda, ref datiAssicurativi,
                out disableCodSpecifico, out disableCodArt22);
            if (disableCodArt22)
                return false;

            return true;
        }

        internal static TipoSalvaguardia? GetTipoSalvaguardia(GestionePensione.DatiPensione datiPensione)
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

        private static bool? GetIsVisibleArt2(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0002") //invalidità e inabilità
                return true;
            else return false;
        }

        private static bool? GetIsDecPensAnteAgosto95(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            DateTime dataCompare = new DateTime(1995, 8, 17);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if ((datiPensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataCompare))
                || (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue &&
                !Utility.DataSuccessivaA(datiDanteCausa.DecorrenzaPensione.Value, dataCompare)))
                return true;
            else
                return false;
        }

        private static bool? GetIsCodiceSpecificoObbligatorio(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            bool isObbligatorio = true;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.VL:
                        if (!(datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001") && !(datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0001"))
                            isObbligatorio = false;
                        break;
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.DZ: //Inserimento casistica Dazio
                        if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                            isObbligatorio = false;
                        break;
                    default:
                        break;
                }
            }
            return isObbligatorio;
        }

        private static bool? GetIsCodiceSpecificoVisible(GestionePensione.DatiPensione datiPensione)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL && ((datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0011") ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")))
                return false;
            return true;
        }

        private static bool? GetIsRequisitiL247_L243Enable(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 01, 01)))
                return false;

            return true;
        }


        private static DateTime? GetDecorrenzaPensioneDirettaDC(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, GestioneLavorazione.DatiLavorazione datiLavorazione)
        {
            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione))
                return datiDanteCausa.DecorrenzaPensione;

            return null;
        }

        private static bool? GetIsCodDirittoQuoteFisseVisible(GestionePensione.DatiPensione datiPensione, DateTime? decorrenzaPensioneDC)
        {
            DateTime? decorrenza = null;
            if (decorrenzaPensioneDC.HasValue)
                decorrenza = decorrenzaPensioneDC;
            else
                decorrenza = datiPensione.DecorrenzaOriginaria;

            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1984, 01, 01)))
                return true;
            return false;
        }

        private static bool? GetIsIndennitaAggiuntivaVisible(GestionePensione.DatiPensione datiPensione, DateTime? decorrenzaPensioneDC)
        {
            DateTime? decorrenza = null;
            if (decorrenzaPensioneDC.HasValue)
                decorrenza = decorrenzaPensioneDC;
            else
                decorrenza = datiPensione.DecorrenzaOriginaria;

            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1987, 04, 01)))
                return true;
            return false;
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
                        (!datiIstruttoria.CodiceComunicazioneCampo3.HasValue || datiIstruttoria.CodiceComunicazioneCampo3.Value == ' ' || datiIstruttoria.CodiceComunicazioneCampo3.Value == 'Y')))
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

        private static bool IsCodiceNatura2DisabledPerSperDonna(GestionePensione.DatiPensione datiPensione)
        {
            if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "O")
                return true;

            return false;
        }

        private static bool IsCapitalizzazioneVisible(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (tipoFondo == Utility.TipoFondo.VL && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2004, 1, 1)))
                return false;

            return true;
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

        #endregion public members

        #region private members

        #region dati Generici
        private static void StoreDatiGenericiPerPensione(Entity.DatiGenerici datiGenerici, GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                datiGenerici.TipoCalcolo = datiPensione.TipoCalcolo;

            Utility.ValorizzaOggetti(datiGenerici, datiPensione);
            GestionePensione.SalvaPensione(datiPensione);
            return;
        }

        private static void StoreDatiGenericiINPDAPPerPensione(DatiGenericiINPDAP datiGenericiINPDAP, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiGenericiINPDAP.TipoCalcolo = datiPensione.TipoCalcolo;
                //datiGenericiINPDAP.NaturaPensione = datiPensione.NaturaPensione;
                //datiGenericiINPDAP.ExCombattente = datiPensione.ExCombattente;
                //datiGenericiINPDAP.Benefici = datiPensione.Benefici;
            }

            if (Utility.IsDomandaInabilitaAmianto(datiPensione))
                datiGenericiINPDAP.Benefici = datiPensione.Benefici;

            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                datiGenericiINPDAP.CodiceArretrati = datiPensione.CodiceArretrati;

            Utility.ValorizzaOggetti(datiGenericiINPDAP, datiPensione);
            GestionePensione.SalvaPensione(datiPensione);
        }

        private static void StoreDatiGenericiPerIstruttoria(long idPensione, Entity.DatiGenerici datiGenerici,
            ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestionePensione.DatiPensione datiPensione, bool bloccoDeroga)
        {
            if (datiIstruttoria == null)
            {
                if (datiGenerici.IsIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica || bloccoDeroga)
            {
                datiGenerici.CodiceParticolareSoggettoDerogato = datiIstruttoria.CodiceParticolareSoggettoDerogato;
            }
            Utility.ValorizzaOggetti(datiGenerici, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
            {
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
                datiIstruttoria = null;
            }
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
            return;
        }

        private static void StoreDatiGenericiINPDAPPerIstruttoria(GestionePensione.DatiPensione datiPensione, Entity.DatiGenericiINPDAP datiGenericiINPDAP,
            ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiGenericiINPDAP.IsDatiGenericiINPDAPIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiGenericiINPDAP.CodiceComunicazioneCampo1 = datiIstruttoria.CodiceComunicazioneCampo1;
                datiGenericiINPDAP.CodiceComunicazioneCampo2 = datiIstruttoria.CodiceComunicazioneCampo2;
                datiGenericiINPDAP.CodiceComunicazioneCampo3 = datiIstruttoria.CodiceComunicazioneCampo3;
                //datiGenericiINPDAP.CodiceComunicazioneCampo4 = datiIstruttoria.CodiceComunicazioneCampo4;
            }

            Utility.ValorizzaOggetti(datiGenericiINPDAP, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
            {
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(datiPensione.Id);
                datiIstruttoria = null;
            }
            else
                GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);
            return;
        }

        private static void StoreDatiGenericiPerEliminazione(long idPensione, Entity.DatiGenerici datiGenerici, ref GestionePensione.DatiEliminazione datiEliminazione)
        {
            if (datiEliminazione == null)
            {
                if (datiGenerici.IsEliminazioneNull())
                    return;
                else
                    datiEliminazione = new GestionePensione.DatiEliminazione();
            }
            Utility.ValorizzaOggetti(datiGenerici, datiEliminazione);
            if (datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
            {
                GestionePensione.EliminaEliminazione(idPensione);
                datiEliminazione = null;
            }
            else
                GestionePensione.SalvaEliminazione(idPensione, datiEliminazione);
            return;
        }

        private static void StoreDatiGenericiINPDAPPerEliminazione(long idPensione, DatiGenericiINPDAP datiGenericiINPDAP, ref GestionePensione.DatiEliminazione datiEliminazione)
        {
            if (datiEliminazione == null)
            {
                if (datiGenericiINPDAP.IsDatiGenericiINPDAPEliminazioneNull())
                    return;
                else
                    datiEliminazione = new GestionePensione.DatiEliminazione();
            }
            Utility.ValorizzaOggetti(datiGenericiINPDAP, datiEliminazione);
            if (datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
            {
                GestionePensione.EliminaEliminazione(idPensione);
                datiEliminazione = null;
            }
            else
                GestionePensione.SalvaEliminazione(idPensione, datiEliminazione);
        }

        private static void StoreDatiGenericiPerFondoDatiGenerici(long idPensione, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondo datiFondo,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, out bool eliminaFondoDatiGenerici)
        {
            eliminaFondoDatiGenerici = false;
            if (datiFondo == null)
            {
                if (datiGenerici.IsFondoDatiGenericiNull())
                    return;
                else
                    datiFondo = new GestioneFondo.DatiFondo();
            }
            Utility.ValorizzaOggetti(datiGenerici, datiFondo);

            if (datiFondo.IsFondoNull())
            {
                if (isDomandaConNuovaGestioneDatiFondoFSPT)
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
                else
                    eliminaFondoDatiGenerici = true;
            }
            else
                GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
            return;
        }

        private static void StoreDatiGenericiINPDAPPerPensioneFondoDatiGenerici(long idPensione, DatiGenericiINPDAP datiGenericiINPDAP,
            ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondo == null)
            {

                if (datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneFondoDatiGenericiNull())
                    return;
                else
                    datiFondo = new GestioneFondo.DatiFondo();
            }
            Utility.ValorizzaOggetti(datiGenericiINPDAP, datiFondo);

            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
        }

        private static void StoreDatiGenericiINPDAPPerPensioneINPDAP(long idPensione, Entity.DatiGenericiINPDAP datiGenericiINPDAP,
            ref List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP)
        {
            if (listaDatiPensioneINPDAP == null || listaDatiPensioneINPDAP.Count == 0)
            {
                if (datiGenericiINPDAP == null || datiGenericiINPDAP.IsDatiGenericiINPDAPPensioneINPDAPNull())
                    return;
                else
                {
                    listaDatiPensioneINPDAP = new List<GestionePensioneINPDAP.DatiPensioneINPDAP>();
                    GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP = new GestionePensioneINPDAP.DatiPensioneINPDAP();
                    listaDatiPensioneINPDAP.Add(datiPensioneINPDAP);
                }
            }

            foreach (GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP in listaDatiPensioneINPDAP)
            {
                Utility.ValorizzaOggetti(datiGenericiINPDAP, datiPensioneINPDAP);

                GestionePensioneINPDAP.SalvaPensioneINPDAPRecordFondo(idPensione, datiPensioneINPDAP.IdRecordFondo.GetValueOrDefault(), datiPensioneINPDAP);
            }
        }

        private static void StoreDatiGenericiPerFondoEL(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoEL datiFondoEL, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoEL == null)
            {
                if (datiGenerici.fondoEL == null || datiGenerici.fondoEL.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoEL = new GestioneFondo.DatiFondoEL();
            }

            Utility.ValorizzaOggetti(datiGenerici.fondoEL, datiFondoEL);
            if (datiFondoEL.Equals(new GestioneFondo.DatiFondoEL()))
            {
                GestioneFondo.EliminaFondoEL(idPensione);
                datiFondoEL = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoEL(idFondo, datiFondoEL);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoTT(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoTT datiFondoTT, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoTT == null)
            {
                if (datiGenerici.fondoTT == null || datiGenerici.fondoTT.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }

                    return;
                }
                else
                    datiFondoTT = new GestioneFondo.DatiFondoTT();
            }
            Utility.ValorizzaOggetti(datiGenerici.fondoTT, datiFondoTT);
            if (datiFondoTT.Equals(new GestioneFondo.DatiFondoTT()))
            {
                GestioneFondo.EliminaFondoTT(idPensione);
                datiFondoTT = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoTT(idFondo, datiFondoTT);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoET(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoET datiFondoET, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoET == null)
            {
                if (datiGenerici.fondoET == null || datiGenerici.fondoET.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoET = new GestioneFondo.DatiFondoET();
            }
            Utility.ValorizzaOggetti(datiGenerici.fondoET, datiFondoET);
            if (datiFondoET.Equals(new GestioneFondo.DatiFondoET()))
            {
                GestioneFondo.EliminaFondoET(idPensione);
                datiFondoET = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoET(idFondo, datiFondoET);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoVL(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoVL datiFondoVL, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoVL == null)
            {
                if (datiGenerici.fondoVL == null || datiGenerici.fondoVL.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoVL = new GestioneFondo.DatiFondoVL();
            }

            Utility.ValorizzaOggetti(datiGenerici.fondoVL, datiFondoVL);
            if (datiFondoVL.Equals(new GestioneFondo.DatiFondoVL()))
            {
                GestioneFondo.EliminaFondoVL(idPensione);
                datiFondoVL = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoVL(idFondo, datiFondoVL);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoPT(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref List<GestioneFondo.DatiFondoPT> listaDatiFondoPT,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (listaDatiFondoPT == null || listaDatiFondoPT.Count == 0)
            {
                if (datiGenerici.fondoPT == null || datiGenerici.fondoPT.IsFondoNull())
                {
                    if (!isDomandaConNuovaGestioneDatiFondoFSPT && eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                {
                    listaDatiFondoPT = new List<GestioneFondo.DatiFondoPT>();
                    GestioneFondo.DatiFondoPT datiFondoPT = new GestioneFondo.DatiFondoPT();
                    listaDatiFondoPT.Add(datiFondoPT);
                }
            }

            if (isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                foreach (GestioneFondo.DatiFondoPT datiFondoPT in listaDatiFondoPT)
                {
                    datiFondoPT.FinestraMobile = datiGenerici.fondoPT.FinestraMobile;
                    datiFondoPT.RequisitiAnte247 = datiGenerici.fondoPT.RequisitiAnte247;
                    datiFondoPT.TrimesteRequisiti = datiGenerici.fondoPT.TrimesteRequisiti;
                    datiFondoPT.AnzianitaAnni = datiGenerici.fondoPT.AnzianitaAnni;
                    datiFondoPT.AnnoRequisiti = datiGenerici.fondoPT.AnnoRequisiti;

                    if (!datiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                    {
                        if (idFondo == 0 || eliminaFondoDatiGenerici)
                        {
                            GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                            idFondo = datiFondoNew.Id;
                            datiFondo = datiFondoNew;
                        }
                        GestioneFondo.SalvaFondoPTRecordFondo(idFondo, datiFondoPT.IdRecordFondo.Value, datiFondoPT);
                    }
                }
            }
            else
            {
                foreach (GestioneFondo.DatiFondoPT datiFondoPT in listaDatiFondoPT)
                    Utility.ValorizzaOggetti(datiGenerici.fondoPT, datiFondoPT);

                if (listaDatiFondoPT.First().Equals(new GestioneFondo.DatiFondoPT()))
                {
                    GestioneFondo.EliminaFondoPT(idPensione);
                    listaDatiFondoPT = null;
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                }
                else
                {
                    if (idFondo == 0 || eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                        idFondo = datiFondoNew.Id;
                        datiFondo = datiFondoNew;
                    }
                    GestioneFondo.SalvaFondoPT(idFondo, listaDatiFondoPT.First());
                }
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoFST(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref List<GestioneFondo.DatiFondoFST> listaDatiFondoFST,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (listaDatiFondoFST == null || listaDatiFondoFST.Count == 0)
            {
                if (datiGenerici.fondoFST == null || datiGenerici.fondoFST.IsFondoNull())
                {
                    if (!isDomandaConNuovaGestioneDatiFondoFSPT && eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                {
                    listaDatiFondoFST = new List<GestioneFondo.DatiFondoFST>();
                    GestioneFondo.DatiFondoFST datiFondoFST = new GestioneFondo.DatiFondoFST();
                    listaDatiFondoFST.Add(datiFondoFST);
                }
            }

            if (isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                foreach (GestioneFondo.DatiFondoFST datiFondoFST in listaDatiFondoFST)
                {
                    datiFondoFST.RequisitiAnte247 = datiGenerici.fondoFST.RequisitiAnte247;
                    datiFondoFST.TrimesteRequisiti = datiGenerici.fondoFST.TrimesteRequisiti;
                    datiFondoFST.AnzianitaAnni = datiGenerici.fondoFST.AnzianitaAnni;
                    datiFondoFST.AnnoRequisiti = datiGenerici.fondoFST.AnnoRequisiti;

                    if (!datiFondoFST.Equals(new GestioneFondo.DatiFondoFST()))
                    {
                        if (idFondo == 0 || eliminaFondoDatiGenerici)
                        {
                            GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                            idFondo = datiFondoNew.Id;
                            datiFondo = datiFondoNew;
                        }
                        GestioneFondo.SalvaFondoFSTRecordFondo(idFondo, datiFondoFST.IdRecordFondo.Value, datiFondoFST);
                    }
                }
            }
            else
            {
                foreach (GestioneFondo.DatiFondoFST datiFondoFST in listaDatiFondoFST)
                    Utility.ValorizzaOggetti(datiGenerici.fondoFST, datiFondoFST);

                if (listaDatiFondoFST.First().Equals(new GestioneFondo.DatiFondoFST()))
                {
                    GestioneFondo.EliminaFondoFST(idPensione);
                    listaDatiFondoFST = null;
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                }
                else
                {
                    if (idFondo == 0 || eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                        idFondo = datiFondoNew.Id;
                        datiFondo = datiFondoNew;
                    }
                    GestioneFondo.SalvaFondoFST(idFondo, listaDatiFondoFST.First());
                }
            }
            return;
        }

        private static void StoreDatiGenericiPI(ref EntityBLCommon.ContenitoreObject contenitore, GestioneFondo.DatiFondoPI datiFondoPI, GestionePensione.DatiPensione datiPensione,
            Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondo datiFondo, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestionePensione.DatiEliminazione datiEliminazione, GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione,
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici, bool IsCancelOperation, GestioneCtrlRic.ControlTabRic controlTabRic, bool isRiaperturaDomanda, ref GestionePagamento.DatiPagamento datiPagamento)
        {
            bool bloccoDeroga = false;
            if (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) ||
                Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) ||
                Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) ||
                Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) ||
                Utility.IsDomandaVecchPerditaTitolo(datiPensione))
                bloccoDeroga = true;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiGenericiPerPensione(datiGenerici, datiPensione);
                StoreDatiGenericiPerIstruttoria(datiPensione.Id, datiGenerici, ref datiIstruttoria, datiPensione, bloccoDeroga);
                StoreDatiGenericiPerEliminazione(datiPensione.Id, datiGenerici, ref datiEliminazione);
                bool eliminaFondoDatiGenerici = false;
                StoreDatiGenericiPerFondoDatiGenerici(datiPensione.Id, datiGenerici, ref datiFondo, false, out eliminaFondoDatiGenerici);

                long idFondo = 0;
                if (datiFondo != null)
                    idFondo = datiFondo.Id;

                if (IsCancelOperation)
                    datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                else
                {
                    if (!datiGenerici.IsPensioneNull() || !datiGenerici.IsIstruttoriaNull() || !datiGenerici.IsEliminazioneNull() || !datiGenerici.IsFondoDatiGenericiNull())
                        datiQuadroLiquidazionePensione.TabDatiGenerici = 2;
                    else
                        datiQuadroLiquidazionePensione.TabDatiGenerici = 0;
                }

                StoreDatiGenericiPerFondoPI(datiPensione.Id, idFondo, datiGenerici, ref datiFondoPI, eliminaFondoDatiGenerici, ref datiFondo);
                StoreDatiGenericiPerDatiPagamento(datiPensione, datiGenerici, ref datiPagamento);

                if ((Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda) && controlTabRic != null && !controlTabRic.TabGenerici)
                    datiQuadroLiquidazionePensione.TabDatiGenerici = null;

                #region Gestione visibilità tabs MaggiorazioneBenefici

                if (datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabExCombattente == null)
                        datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = null;

                if (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabBenefici == null)
                        datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabBenefici = null;

                if (datiGenerici.ChkDL407.HasValue && datiGenerici.ChkDL407.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabLegge407 == null)
                        datiQuadroMaggiorazioniBenefici.TabLegge407 = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabLegge407 = null;

                if (datiGenerici.Articolo2.HasValue && datiGenerici.Articolo2.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabArticolo2 == null)
                        datiQuadroMaggiorazioniBenefici.TabArticolo2 = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabArticolo2 = null;

                if (datiGenerici.Privilegiate.HasValue && datiGenerici.Privilegiate.Value)
                {
                    if (datiQuadroMaggiorazioniBenefici.TabPrivilegiate == null)
                        datiQuadroMaggiorazioniBenefici.TabPrivilegiate = 0;
                }
                else
                    datiQuadroMaggiorazioniBenefici.TabPrivilegiate = null;

                if ((datiGenerici.ExCombattente.HasValue && datiGenerici.ExCombattente.Value && datiQuadroMaggiorazioniBenefici.TabExCombattente == 2) ||
                    (datiGenerici.Benefici.HasValue && datiGenerici.Benefici.Value && datiQuadroMaggiorazioniBenefici.TabBenefici == 2) ||
                    (datiGenerici.ChkDL407.HasValue && datiGenerici.ChkDL407.Value && datiQuadroMaggiorazioniBenefici.TabLegge407 == 2) ||
                    (datiGenerici.Articolo2.HasValue && datiGenerici.Articolo2.Value && datiQuadroMaggiorazioniBenefici.TabArticolo2 == 2) ||
                    (datiGenerici.Privilegiate.HasValue && datiGenerici.Privilegiate.Value && datiQuadroMaggiorazioniBenefici.TabPrivilegiate == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;

                if (datiQuadroMaggiorazioniBenefici.TabExCombattente == 0 || datiQuadroMaggiorazioniBenefici.TabBenefici == 0 ||
                    datiQuadroMaggiorazioniBenefici.TabLegge407 == 0 || datiQuadroMaggiorazioniBenefici.TabArticolo2 == 0 ||
                    datiQuadroMaggiorazioniBenefici.TabPrivilegiate == 0)
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                if (!datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && !datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && !datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue &&
                    !datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && !datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue)
                    datiQuadroMaggiorazioniBenefici.Tipo = 0;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                #endregion Gestione visibilità tabs MaggiorazioneBenefici

                GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiquidazionePensione);
                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE ---*/
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiIstruttoria = datiIstruttoria;
            contenitore.DatiEliminazione = datiEliminazione;
            contenitore.DatiFondo = datiFondo;
            contenitore.DatiFondoPI = datiFondoPI;
            contenitore.DatiPagamento = datiPagamento;
            contenitore.DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        private static void StoreDatiGenericiPerFondoPI(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoPI datiFondoPI, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoPI == null)
            {
                if (datiGenerici.fondoPI == null || datiGenerici.fondoPI.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoPI = new GestioneFondo.DatiFondoPI();
            }

            Utility.ValorizzaOggetti(datiGenerici.fondoPI, datiFondoPI);
            if (datiFondoPI.Equals(new GestioneFondo.DatiFondoPI()))
            {
                GestioneFondo.EliminaFondoPI(idPensione);
                datiFondoPI = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoPIRecordFondo(idFondo, datiFondoPI.IdRecordFondo, datiFondoPI);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoGAS(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoGAS datiFondoGAS, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoGAS == null)
            {
                if (datiGenerici.fondoGAS == null || datiGenerici.fondoGAS.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoGAS = new GestioneFondo.DatiFondoGAS();
            }

            Utility.ValorizzaOggetti(datiGenerici.fondoGAS, datiFondoGAS);
            if (datiFondoGAS.Equals(new GestioneFondo.DatiFondoGAS()))
            {
                GestioneFondo.EliminaFondoGAS(idPensione);
                datiFondoGAS = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoGAS(idFondo, datiFondoGAS);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoDZ(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoDZ datiFondoDZ, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoDZ == null)
            {
                if (datiGenerici.fondoDZ == null || datiGenerici.fondoDZ.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoDZ = new GestioneFondo.DatiFondoDZ();
            }

            Utility.ValorizzaOggetti(datiGenerici.fondoDZ, datiFondoDZ);
            if (datiFondoDZ.Equals(new GestioneFondo.DatiFondoDZ()))
            {
                GestioneFondo.EliminaFondoDZ(idPensione);
                datiFondoDZ = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoDZ(idFondo, datiFondoDZ);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoES(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoES datiFondoES, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoES == null)
            {
                if (datiGenerici.fondoES == null || datiGenerici.fondoES.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoES = new GestioneFondo.DatiFondoES();
            }

            Utility.ValorizzaOggetti(datiGenerici.fondoES, datiFondoES);
            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
            {
                GestioneFondo.EliminaFondoES(idPensione);
                datiFondoES = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoES(idFondo, datiFondoES);
            }
            return;
        }

        private static void StoreDatiGenericiPerFondoPM(long idPensione, long idFondo, Entity.DatiGenerici datiGenerici, ref GestioneFondo.DatiFondoPM datiFondoPM, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoPM == null)
            {
                if (datiGenerici.fondoPM == null || datiGenerici.fondoPM.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoPM = new GestioneFondo.DatiFondoPM();
            }

            Utility.ValorizzaOggetti(datiGenerici.fondoPM, datiFondoPM);
            if (datiFondoPM.Equals(new GestioneFondo.DatiFondoPM()))
            {
                GestioneFondo.EliminaFondoPM(idPensione);
                datiFondoPM = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoPM(idFondo, datiFondoPM);
            }
            return;
        }

        private static bool ControlDatiGenericiWithFondi(ref EntityBLCommon.ContenitoreObject contenitore, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneFondo.DatiFondo datiFondo, Entity.DatiGenerici datiGenerici, Entity.DatiAssicurativi datiAssicurativi, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, bool isDomandaConNuovaGestioneDatiFondoFSPT,
            char? derogaTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            object datiFondoXX = null;

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = contenitore.DatiAnagraficiTitolare;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare = contenitore.DatiAreaTitolare;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    #region FondoEL
                    case Utility.TipoFondo.EL:
                        GestioneFondo.DatiFondoEL datiFondoEL = new GestioneFondo.DatiFondoEL();
                        Utility.ValorizzaOggetti(datiGenerici.fondoEL, datiFondoEL);
                        datiFondoXX = datiFondoEL;

                        if (!ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(datiIstruttoria, datiGenerici, out messaggioVideo) && !(datiPensione.Gruppo.Trim() == "0051" && datiPensione.Gestione.Trim() == "007"))
                            return false;

                        if (!ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoEL, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        if (datiGenerici.InizioBonus.HasValue)
                        {
                            //GestioneFondo.DatiFondoEL datiFondoELApp = null;
                            //GestioneFondo.GetFondoELByNumeroDomanda(numeroDomanda, out datiFondoELApp);
                            if (datiAssicurativi != null && datiAssicurativi.fondoEL != null && datiAssicurativi.fondoEL.DecorrenzaTeorica.HasValue && datiGenerici.InizioBonus.Value.Date != datiAssicurativi.fondoEL.DecorrenzaTeorica)
                            {
                                messaggioVideo = "La data di inizio bonus deve coincidere con la data di decorrenza teorica indicata nel tab 'Dati Assicurativi'";
                                return false;
                            }
                        }

                        break;
                    #endregion FondoEL

                    #region FondoTT
                    case Utility.TipoFondo.TT:
                        GestioneFondo.DatiFondoTT datiFondoTT = new GestioneFondo.DatiFondoTT();
                        Utility.ValorizzaOggetti(datiGenerici.fondoTT, datiFondoTT);
                        datiFondoXX = datiFondoTT;


                        if (!ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(datiIstruttoria, datiGenerici, out messaggioVideo) && !(datiPensione.Gruppo.Trim() == "0051" && datiPensione.Gestione.Trim() == "007"))
                            return false;

                        if (!ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoTT, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        if (datiGenerici.InizioBonus.HasValue)
                        {
                            //GestioneFondo.DatiFondoTT datiFondoTTApp = null;
                            //GestioneFondo.GetFondoTTByNumeroDomanda(numeroDomanda, out datiFondoTTApp);
                            if (datiAssicurativi != null && datiAssicurativi.fondoTT != null && datiAssicurativi.fondoTT.DecorrenzaTeorica.HasValue && datiGenerici.InizioBonus.Value.Date != datiAssicurativi.fondoTT.DecorrenzaTeorica)
                            {
                                messaggioVideo = "La data di inizio bonus deve coincidere con la data di decorrenza teorica indicata nel tab 'Dati Assicurativi'";
                                return false;
                            }
                        }

                        GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
                        GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;
                        GestioneDL407.DatiDL407 datiDL407 = null;
                        List<GestioneDatiServizioUtile.ServizioUtile> lstServizioUtile = null;
                        Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione).GetValueOrDefault());
                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref datiContributivi, ref datiRetributivi, ref datiDL407,
                            ref lstServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo,
                            datiAssicurativi != null && datiAssicurativi.fondoTT != null ? datiAssicurativi.fondoTT.DimissioniAnte97 : null,
                            datiAssicurativi != null ? datiAssicurativi.CodiceRequisiti2 : null))
                            return false;

                        break;
                    #endregion FondoTT

                    #region FondoET
                    case Utility.TipoFondo.ET:
                        GestioneFondo.DatiFondoET datiFondoET = new GestioneFondo.DatiFondoET();
                        Utility.ValorizzaOggetti(datiGenerici.fondoET, datiFondoET);
                        datiFondoXX = datiFondoET;

                        if (!ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(datiIstruttoria, datiGenerici, out messaggioVideo) && !(datiPensione.Gruppo.Trim() == "0051" && datiPensione.Gestione.Trim() == "007"))
                            return false;

                        if (!ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoET, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        if (datiGenerici.InizioBonus.HasValue)
                        {
                            //GestioneFondo.DatiFondoET datiFondoETApp = null;
                            //GestioneFondo.GetFondoETByNumeroDomanda(numeroDomanda, out datiFondoETApp);
                            if (datiAssicurativi != null && datiAssicurativi.fondoET != null && datiAssicurativi.fondoET.DecorrenzaTeorica.HasValue && datiGenerici.InizioBonus.Value.Date != datiAssicurativi.fondoET.DecorrenzaTeorica)
                            {
                                messaggioVideo = "La data di inizio bonus deve coincidere con la data di decorrenza teorica indicata nel tab 'Dati Assicurativi'";
                                return false;
                            }
                        }


                        if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
                        {
                            if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione) || Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL(datiPensione))
                            {
                                if (string.IsNullOrEmpty(datiGenerici.NaturaPensione) || datiGenerici.NaturaPensione.Substring(1, 1) != "W")
                                {
                                    messaggioVideo = "Il secondo codice natura deve essere W.";
                                    return false;
                                }
                            }
                            else if (Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione) || Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL(datiPensione))
                            {
                                if (string.IsNullOrEmpty(datiGenerici.NaturaPensione) || datiGenerici.NaturaPensione.Substring(1, 1) != "K")
                                {
                                    messaggioVideo = "Il secondo codice natura deve essere K.";
                                    return false;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(datiGenerici.NaturaPensione) && (datiGenerici.NaturaPensione.Substring(1, 1) == "K" || datiGenerici.NaturaPensione.Substring(1, 1) == "W"))
                                {
                                    messaggioVideo = "Il secondo codice natura non può essere K o W.";
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(datiGenerici.NaturaPensione) && (datiGenerici.NaturaPensione.Substring(1, 1) == "K" || datiGenerici.NaturaPensione.Substring(1, 1) == "W"))
                            {
                                messaggioVideo = "Il secondo codice natura non può essere K o W.";
                                return false;
                            }
                        }

                        if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                        {
                            GestioneFondo.DatiFondoET datiETApp;
                            datiETApp = contenitore.DatiFondoET;
                            if (datiETApp != null && !GestioneControlli.ControlET_AltraPensDatiAgo(datiPensione.DecorrenzaOriginaria, datiGenerici.NaturaPensione, datiETApp, out messaggioVideo))
                            {
                                return false;
                            }
                        }
                        break;
                    #endregion FondoET

                    #region FondoVL
                    case Utility.TipoFondo.VL:
                        GestioneFondo.DatiFondoVL datiFondoVL = new GestioneFondo.DatiFondoVL();
                        Utility.ValorizzaOggetti(datiGenerici.fondoVL, datiFondoVL);
                        datiFondoXX = datiFondoVL;

                        if (!ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(datiIstruttoria, datiGenerici, out messaggioVideo) && !(datiPensione.Gruppo.Trim() == "0051" && datiPensione.Gestione.Trim() == "007"))
                            return false;

                        if (!ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoVL, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        if (datiAssicurativi != null && datiAssicurativi.fondoVL != null)
                        {
                            if (!(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) && Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC)) &&
                                !GestioneCrossControls.FS_VerificaCoerenzaRetrAGOAnnua(datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaA,
                                datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaB, (datiGenerici != null ?
                                Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS) : Utility.TipoCalcolo.NonValido), out messaggioVideo))
                                return false;
                        }
                        break;
                    #endregion FondoVL

                    #region FondoPT
                    case Utility.TipoFondo.PT:
                        List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = new List<GestioneFondo.DatiFondoPT>();
                        GestioneFondo.DatiFondoPT datiFondoPT = new GestioneFondo.DatiFondoPT();
                        Utility.ValorizzaOggetti(datiGenerici.fondoPT, datiFondoPT);
                        listaDatiFondoPT.Add(datiFondoPT);
                        datiFondoXX = listaDatiFondoPT;

                        if (!ControlsDatiGenericiPT(datiGenerici, datiPensione, isDomandaConNuovaGestioneDatiFondoFSPT, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, listaDatiFondoPT, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        break;
                    #endregion FondoPT

                    #region FondoFS
                    case Utility.TipoFondo.FS:
                        List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = new List<GestioneFondo.DatiFondoFST>();
                        GestioneFondo.DatiFondoFST datiFondoFST = new GestioneFondo.DatiFondoFST();
                        Utility.ValorizzaOggetti(datiGenerici.fondoFST, datiFondoFST);
                        listaDatiFondoFST.Add(datiFondoFST);
                        datiFondoXX = listaDatiFondoFST;

                        if (!ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiFS(datiGenerici, datiPensione, isDomandaConNuovaGestioneDatiFondoFSPT, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, listaDatiFondoFST, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio,
                            codiceSpecificoTraduzioneSuGP, derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;
                        break;
                    #endregion FondoFS

                    #region FondoPI
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.DatiFondoPI datiFondoPI = new GestioneFondo.DatiFondoPI();
                        Utility.ValorizzaOggetti(datiGenerici.fondoPI, datiFondoPI);
                        datiFondoXX = datiFondoPI;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoPI, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        break;
                    #endregion FondoPI

                    #region FondoGAS
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.DatiFondoGAS datiFondoGAS = new GestioneFondo.DatiFondoGAS();
                        Utility.ValorizzaOggetti(datiGenerici.fondoGAS, datiFondoGAS);
                        datiFondoXX = datiFondoGAS;

                        if (!ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(datiIstruttoria, datiGenerici, out messaggioVideo) && !(datiPensione.Gruppo.Trim() == "0051" && datiPensione.Gestione.Trim() == "007"))
                            return false;

                        if (!ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoGAS, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        if (datiGenerici.InizioBonus.HasValue)
                        {
                            if (datiFondoGAS != null && datiFondoGAS.DecorrenzaTeorica.HasValue && datiGenerici.InizioBonus.Value.Date != datiFondoGAS.DecorrenzaTeorica)
                            {
                                messaggioVideo = "La data di inizio bonus deve coincidere con la data di decorrenza teorica indicata nel tab 'Dati Ago'";
                                return false;
                            }
                        }

                        break;
                    #endregion FondoGAS

                    #region FondoCL
                    case Utility.TipoFondo.CL:
                        //Commentato in attesa di sapere se la tab PrecedentePensione è visibile per fondo CL
                        //if (!ControlsDatiGenericiEL_TT_ET_VL_GAS(datiIstruttoria, datiGenerici, out messaggioVideo))
                        //    return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        break;
                    #endregion FondoCL

                    #region FondoDZ
                    case Utility.TipoFondo.DZ:
                        List<GestioneFondo.DatiFondoDZ> listaDatiFondoDZ = new List<GestioneFondo.DatiFondoDZ>();
                        GestioneFondo.DatiFondoDZ datiFondoDZ = new GestioneFondo.DatiFondoDZ();
                        Utility.ValorizzaOggetti(datiGenerici.fondoDZ, datiFondoDZ);
                        listaDatiFondoDZ.Add(datiFondoDZ);
                        datiFondoXX = listaDatiFondoDZ;

                        if (!ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(datiIstruttoria, datiGenerici, out messaggioVideo) && !(datiPensione.Gruppo.Trim() == "0051" && datiPensione.Gestione.Trim() == "007"))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, listaDatiFondoDZ, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;

                        break;
                    #endregion FondoDZ

                    #region FondoES
                    case Utility.TipoFondo.ES:
                        GestioneFondo.DatiFondoES datiFondoES = new GestioneFondo.DatiFondoES();
                        Utility.ValorizzaOggetti(datiGenerici.fondoES, datiFondoES);
                        datiFondoXX = datiFondoES;

                        if (!ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(datiIstruttoria, datiGenerici, out messaggioVideo) && !(datiPensione.Gruppo.Trim() == "0051" && datiPensione.Gestione.Trim() == "007"))
                            return false;

                        if (!ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsDatiGenericiEliminazioneContestuale(datiGenerici, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoES, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;
                        break;
                    #endregion FondoES

                    #region FondoPM
                    case Utility.TipoFondo.PM:
                        GestioneFondo.DatiFondoPM datiFondoPM = new GestioneFondo.DatiFondoPM();
                        Utility.ValorizzaOggetti(datiGenerici.fondoPM, datiFondoPM);
                        datiFondoXX = datiFondoPM;

                        if (!GestioneControlli.VerificaRequisitiNoInvalidita(tipoFondo, datiFondoPM, datiPensione, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                            derogaTraduzioneSuGP, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, false, out messaggioVideo))
                            return false;
                        break;
                    #endregion FondoES
                }
            }

            #region CrossControls for EL_TT_ET_VL_FS_PT_PI_GAS_CL_DZ_ES_PM fondi: Verifica Eta Titolare Vecchiaia

            GestioneCrossControls.TipoDecPensione? tipoDecPensione = GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo);
            if (tipoDecPensione.HasValue)
            {
                if (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia)
                {
                    //mail 03-04-2013: bypass controlli per L214 e usuranti per il solo prodotto 0002
                    //mail 28-11-2013: bypass controlli per L.228 RE: Reeng Pensioni - Salvaguardia L.228 - Punti aperti
                    //mail 16-07-2014: bypass controlli per L.124 art.11 bis RE: ReEng Pensioni - Salvaguardia L.124/2013 art.11
                    if ((Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                        Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) ||
                        Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) ||
                        Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione)) &&
                        datiPensione.Prodotto == "0002")
                        return true;

                    //mail 24-02-2014: bypass controlli per domande di ricostituzione diverse da Variazione Per Decorrenza
                    if (datiPensione.Gruppo == "0031" && !Utility.IsRicostituzione_VariazionePerDecorrenza(datiPensione))
                        return true;

                    bool? bReturn = GestioneControlli.VerificaEtaTitolareFromAnte247(datiPensione, datiAnagrafici, tipoFondo, datiFondoXX, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP,
                        out messaggioVideo);
                    if (bReturn.HasValue)
                    {
                        if (!bReturn.Value)
                            return false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(messaggioVideo))
                            messaggioVideo = "Dati obbligatori mancanti";
                        return false;
                    }

                }
            }
            #endregion CrossControls for EL_TT_ET_VL_FS_PT_PI_GAS_CL_DZ_ES_PM fondi: Verifica Eta Titolare Vecchiaia

            return true;
        }

        private static void GetDatiGenericiWithFondiByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, Utility.TipoFondo? tipoFondo, ref Entity.DatiGenerici datiGenerici)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    #region FondoEL
                    case Utility.TipoFondo.EL:

                        GestioneFondo.DatiFondoEL datiFondoEL = contenitore.DatiFondoEL;
                        if (datiFondoEL != null)
                        {
                            datiGenerici.fondoEL = new Entity.DatiGenerici.FondoEL();
                            Utility.ValorizzaOggetti(datiFondoEL, datiGenerici.fondoEL);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoEL == null || datiGenerici.fondoEL.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoEL

                    #region FondoTT
                    case Utility.TipoFondo.TT:

                        GestioneFondo.DatiFondoTT datiFondoTT = contenitore.DatiFondoTT;
                        if (datiFondoTT != null)
                        {
                            datiGenerici.fondoTT = new Entity.DatiGenerici.FondoTT();
                            Utility.ValorizzaOggetti(datiFondoTT, datiGenerici.fondoTT);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoTT == null || datiGenerici.fondoTT.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoTT

                    #region FondoET
                    case Utility.TipoFondo.ET:

                        GestioneFondo.DatiFondoET datiFondoET = contenitore.DatiFondoET;
                        if (datiFondoET != null)
                        {
                            datiGenerici.fondoET = new Entity.DatiGenerici.FondoET();
                            Utility.ValorizzaOggetti(datiFondoET, datiGenerici.fondoET);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoET == null || datiGenerici.fondoET.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoET

                    #region FondoVL
                    case Utility.TipoFondo.VL:

                        GestioneFondo.DatiFondoVL datiFondoVL = contenitore.DatiFondoVL;
                        if (datiFondoVL != null)
                        {
                            datiGenerici.fondoVL = new Entity.DatiGenerici.FondoVL();
                            Utility.ValorizzaOggetti(datiFondoVL, datiGenerici.fondoVL);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoVL == null || datiGenerici.fondoVL.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoVL

                    #region FondoPT
                    case Utility.TipoFondo.PT:

                        List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = contenitore.ListaDatiFondoPT;
                        if (listaDatiFondoPT != null && listaDatiFondoPT.Count > 0)
                        {
                            datiGenerici.fondoPT = new Entity.DatiGenerici.FondoPT();
                            Utility.ValorizzaOggetti(listaDatiFondoPT.First(), datiGenerici.fondoPT);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoPT == null || datiGenerici.fondoPT.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoPT

                    #region FondoFS
                    case Utility.TipoFondo.FS:

                        List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = contenitore.ListaDatiFondoFST;
                        if (listaDatiFondoFST != null && listaDatiFondoFST.Count > 0)
                        {
                            datiGenerici.fondoFST = new Entity.DatiGenerici.FondoFST();
                            Utility.ValorizzaOggetti(listaDatiFondoFST.First(), datiGenerici.fondoFST);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoFST == null || datiGenerici.fondoFST.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoFS

                    #region FondoPI
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:

                        GestioneFondo.DatiFondoPI datiFondoPI = contenitore.DatiFondoPI;
                        if (datiFondoPI != null)
                        {
                            datiGenerici.fondoPI = new Entity.DatiGenerici.FondoPI();
                            Utility.ValorizzaOggetti(datiFondoPI, datiGenerici.fondoPI);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoPI == null || datiGenerici.fondoPI.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoPI

                    #region FondoGAS
                    case Utility.TipoFondo.GAS:

                        GestioneFondo.DatiFondoGAS datiFondoGAS = contenitore.DatiFondoGAS;
                        if (datiFondoGAS != null)
                        {
                            datiGenerici.fondoGAS = new Entity.DatiGenerici.FondoGAS();
                            Utility.ValorizzaOggetti(datiFondoGAS, datiGenerici.fondoGAS);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoGAS == null || datiGenerici.fondoGAS.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoGAS

                    #region FondoDZ
                    case Utility.TipoFondo.DZ:

                        GestioneFondo.DatiFondoDZ datiFondoDZ = contenitore.DatiFondoDZ;
                        if (datiFondoDZ != null)
                        {
                            datiGenerici.fondoDZ = new Entity.DatiGenerici.FondoDZ();
                            Utility.ValorizzaOggetti(datiFondoDZ, datiGenerici.fondoDZ);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoDZ == null || datiGenerici.fondoDZ.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoDZ

                    #region FondoES
                    case Utility.TipoFondo.ES:
                        GestioneFondo.DatiFondoES datiFondoES = contenitore.DatiFondoES;
                        if (datiFondoES != null)
                        {
                            datiGenerici.fondoES = new Entity.DatiGenerici.FondoES();
                            Utility.ValorizzaOggetti(datiFondoES, datiGenerici.fondoES);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoES == null || datiGenerici.fondoES.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoES

                    #region FondoPM
                    case Utility.TipoFondo.PM:
                        GestioneFondo.DatiFondoPM datiFondoPM = contenitore.DatiFondoPM;
                        if (datiFondoPM != null)
                        {
                            datiGenerici.fondoPM = new Entity.DatiGenerici.FondoPM();
                            Utility.ValorizzaOggetti(datiFondoPM, datiGenerici.fondoPM);
                        }

                        if (datiGenerici.IsPensioneNull() && datiGenerici.IsIstruttoriaNull() && datiGenerici.IsEliminazioneNull() && datiGenerici.IsFondoDatiGenericiNull())
                        {
                            if (datiGenerici.fondoPM == null || datiGenerici.fondoPM.IsFondoNull())
                                datiGenerici = null;
                        }
                        break;
                    #endregion FondoPM
                }
            }
        }

        private static bool ControlsDatiGenericiEL_TT_ET_VL_GAS_DZ_ES(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Entity.DatiGenerici datiGenerici, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!ControlsDatiGenericiForPrecedentePensione(datiIstruttoria, datiGenerici))
            {
                messaggioVideo = "Eliminare i 'Dati Precedente Pensione' prima di procedere con il salvataggio";
                return false;
            }

            return true;
        }

        private static bool ControlsDatiGenericiBonusEL_TT_ET_VL_FS_GAS_ES(Entity.DatiGenerici datiGenerici, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiGenerici.NaturaPensione.Substring(1, 1).ToUpperInvariant() == "Y")
            {
                if (!GestioneControlli.VerificaPensioneInvaliditaWithoutBonus(datiPensione, datiGenerici.AttribuzioneBonus))
                {
                    messaggioVideo = "Le pensioni di invalidità non possono avere bonus.";
                    return false;
                }

                if ((!datiGenerici.AttribuzioneBonus.HasValue) || (datiGenerici.AttribuzioneBonus.Value && (!datiGenerici.InizioBonus.HasValue || !datiGenerici.FineBonus.HasValue)) ||
                                                                 (!datiGenerici.AttribuzioneBonus.Value && (datiGenerici.InizioBonus.HasValue || datiGenerici.FineBonus.HasValue)))
                {
                    messaggioVideo = "I campi 'Attribuzione Bonus' , 'Data Inizio Bonus' e 'Data Fine Bonus' sono obbligatori in presenza del 2° Codice Natura pari a Y";
                    return false;
                }

                if (datiGenerici.InizioBonus.HasValue && datiGenerici.InizioBonus.Value.CompareTo(new DateTime(2004, 11, 01)) < 0)
                {
                    messaggioVideo = "la data 'Inizio Bonus' non può essere precedente al 11/2004";
                    return false;
                }

                if (datiGenerici.FineBonus.HasValue && datiGenerici.FineBonus.Value.CompareTo(new DateTime(2007, 12, 01)) > 0)
                {
                    messaggioVideo = "la data 'Fine Bonus' non può essere successiva al 12/2007";
                    return false;
                }
            }

            //Controllo presente per i seguenti fondi: EL-TT-ET-VL-FS (non presente per il fondo PT)
            if (datiGenerici.NaturaPensione.Substring(1, 1).ToUpperInvariant() != "Y" && (datiGenerici.AttribuzioneBonus.HasValue || datiGenerici.InizioBonus.HasValue || datiGenerici.FineBonus.HasValue))
            {
                messaggioVideo = "I campi 'Attribuzione Bonus' , 'Data Inizio Bonus' e 'Data Fine Bonus' non devono essere inseriti in presenza del 2° Codice Natura diverso da Y";
                return false;
            }

            return true;
        }

        private static bool ControlsDatiGenericiBonusINPDAP(Entity.DatiGenericiINPDAP datiGenerici, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiGenerici.NaturaPensione.Substring(1, 1).ToUpperInvariant() == "Y")
            {
                if (!GestioneControlli.VerificaPensioneInvaliditaWithoutBonus(datiPensione, datiGenerici.AttribuzioneBonus))
                {
                    messaggioVideo = "Le pensioni di invalidità non possono avere bonus.";
                    return false;
                }

                if ((!datiGenerici.AttribuzioneBonus.HasValue) || (datiGenerici.AttribuzioneBonus.Value && (!datiGenerici.InizioBonus.HasValue || !datiGenerici.FineBonus.HasValue)) ||
                                                                 (!datiGenerici.AttribuzioneBonus.Value && (datiGenerici.InizioBonus.HasValue || datiGenerici.FineBonus.HasValue)))
                {
                    messaggioVideo = "I campi 'Attribuzione Bonus' , 'Data Inizio Bonus' e 'Data Fine Bonus' sono obbligatori in presenza del 2° Codice Natura pari a Y";
                    return false;
                }

                if (datiGenerici.InizioBonus.HasValue && datiGenerici.InizioBonus.Value.CompareTo(new DateTime(2004, 11, 01)) < 0)
                {
                    messaggioVideo = "la data 'Inizio Bonus' non può essere precedente al 11/2004";
                    return false;
                }

                if (datiGenerici.FineBonus.HasValue && datiGenerici.FineBonus.Value.CompareTo(new DateTime(2007, 12, 01)) > 0)
                {
                    messaggioVideo = "la data 'Fine Bonus' non può essere successiva al 12/2007";
                    return false;
                }
            }

            //Controllo presente per i seguenti fondi: EL-TT-ET-VL-FS (non presente per il fondo PT)
            if (datiGenerici.NaturaPensione.Substring(1, 1).ToUpperInvariant() != "Y" && (datiGenerici.AttribuzioneBonus.HasValue || datiGenerici.InizioBonus.HasValue || datiGenerici.FineBonus.HasValue))
            {
                messaggioVideo = "I campi 'Attribuzione Bonus' , 'Data Inizio Bonus' e 'Data Fine Bonus' non devono essere inseriti in presenza del 2° Codice Natura diverso da Y";
                return false;
            }

            return true;
        }

        private static bool ControlsDatiGenericiEliminazioneContestuale(Entity.DatiGenerici datiGenerici, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
            if (!(Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiLavorazione.CodFase)))
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                //Controllo presente per i seguenti fondi: EL-TT-ET-VL-FS-PI-GAS-CL (non presente per il fondo PT)
                if ((datiGenerici.CodiceMotivo.HasValue && !datiGenerici.DecorrenzaEliminazione.HasValue) ||
                    (!datiGenerici.CodiceMotivo.HasValue && datiGenerici.DecorrenzaEliminazione.HasValue))
                {
                    messaggioVideo = "Il campo 'Decorrenza Eliminazione Contestuale' è obbligatorio in presenza del 'Codice Eliminazione Contestuale' e viceversa";
                    return false;
                }

                if (((!datiGenerici.DataEvento.HasValue && datiGenerici.DecorrenzaEliminazione.HasValue) || (datiGenerici.DataEvento.HasValue && !datiGenerici.DecorrenzaEliminazione.HasValue)) &&
                    !(Utility.IsRicostituzione(datiPensione.Gruppo) && tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PT && tipoFondo.Value != Utility.TipoFondo.FS &&
                    (datiPensione.Prodotto == "0101" || datiPensione.Prodotto == "0102" || datiPensione.Prodotto == "0104" || datiPensione.Prodotto == "0108" || datiPensione.Prodotto == "0111" || datiPensione.Prodotto == "0112" ||
                    datiPensione.Prodotto == "0120" || datiPensione.Prodotto == "0301" || datiPensione.Prodotto == "0302" || datiPensione.Prodotto == "0304" || datiPensione.Prodotto == "0308" || datiPensione.Prodotto == "0311" ||
                    datiPensione.Prodotto == "0312" || datiPensione.Prodotto == "0320" || datiPensione.Prodotto == "0401" || datiPensione.Prodotto == "0402" || datiPensione.Prodotto == "0404" || datiPensione.Prodotto == "0408" ||
                    datiPensione.Prodotto == "0411" || datiPensione.Prodotto == "0412" || datiPensione.Prodotto == "0420")))
                {
                    messaggioVideo = "Se presente il campo 'Decorrenza Eliminazione Contestuale' deve essere presente il campo 'Data Evento Eliminazione' e viceversa";
                    return false;
                }
                else if (datiGenerici.DataEvento.HasValue && datiGenerici.DecorrenzaEliminazione.HasValue && datiGenerici.DataEvento.Value.Date > datiGenerici.DecorrenzaEliminazione.Value.Date)
                {
                    messaggioVideo = "Il campo 'Data Evento Eliminazione' non deve superare la data di Decorrenza Eliminazione Contestuale";
                    return false;
                }

                //Controllo presente per i seguenti fondi: EL-TT-ET-VL-PI-GAS-CL (non presente per il fondo PT ed FS)
                if (datiGenerici.DecorrenzaEliminazione.HasValue && datiPensione.DecorrenzaOriginaria.HasValue)
                {
                    if (tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.PT && tipoFondo.Value != Utility.TipoFondo.FS)
                    {

                        DateTime decorrenzaOriginariaControl = datiPensione.DecorrenzaOriginaria.Value.AddMonths(1);
                        int result = DateTime.Compare(datiGenerici.DecorrenzaEliminazione.Value, decorrenzaOriginariaControl);
                        if (result < 0)
                        {
                            messaggioVideo = "Il campo 'Decorrenza Eliminazione Contestuale' non deve essere antecedente alla data 'Decorrenza Pensione + 1 mese'";
                            return false;
                        }
                    }
                }

                //solo per FS???
                //if (datiGenerici.DecorrenzaEliminazione.HasValue && datiGenerici.DecorrenzaEliminazione.Value < datiPensione.DecorrenzaOriginaria.Value)
                //{
                //    messaggioVideo = "La 'Decorrenza Eliminazione Contestuale' deve essere maggiore o uguale alla 'Decorrenza Pensione'";
                //    return false;
                //}

                //Controllo presente per i seguenti fondi: EL_TT_ET_VL_FS-PI-GAS-CL (non presente per il fondo PT)
                if (datiGenerici.DataEvento.HasValue && datiPensione.DecorrenzaOriginaria.HasValue && datiGenerici.DataEvento < datiPensione.DecorrenzaOriginaria)
                {
                    messaggioVideo = "Il campo 'Data Evento Eliminazione Contestuale' deve essere successiva alla data 'Decorrenza Pensione'";
                    return false;
                }
            }
            
            return true;
        }

        private static bool ControlsDatiGenericiEliminazioneContestualeINPDAP(Entity.DatiGenericiINPDAP datiGenerici, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
            if (!(Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiLavorazione.CodFase)))
            {
                //Controllo presente per i seguenti fondi: EL-TT-ET-VL-FS-PI-GAS-CL (non presente per il fondo PT)
                if ((datiGenerici.CodiceMotivo.HasValue && !datiGenerici.DecorrenzaEliminazione.HasValue) ||
                    (!datiGenerici.CodiceMotivo.HasValue && datiGenerici.DecorrenzaEliminazione.HasValue))
                {
                    messaggioVideo = "Il campo 'Decorrenza Eliminazione Contestuale' è obbligatorio in presenza del 'Codice Eliminazione Contestuale' e viceversa";
                    return false;
                }

                if ((!datiGenerici.DataEvento.HasValue && datiGenerici.DecorrenzaEliminazione.HasValue) || (datiGenerici.DataEvento.HasValue && !datiGenerici.DecorrenzaEliminazione.HasValue))
                {
                    messaggioVideo = "Se presente il campo 'Decorrenza Eliminazione Contestuale' deve essere presente il campo 'Data Evento Eliminazione' e viceversa";
                    return false;
                }
                else if (datiGenerici.DataEvento.HasValue && datiGenerici.DecorrenzaEliminazione.HasValue && datiGenerici.DataEvento.Value.Date > datiGenerici.DecorrenzaEliminazione.Value.Date)
                {
                    messaggioVideo = "Il campo 'Data Evento Eliminazione' non deve superare la data di Decorrenza Eliminazione Contestuale";
                    return false;
                }

                //Controllo presente per i seguenti fondi: EL_TT_ET_VL_FS-PI-GAS-CL (non presente per il fondo PT)
                if (datiGenerici.DataEvento.HasValue && datiPensione.DecorrenzaOriginaria.HasValue && datiGenerici.DataEvento < datiPensione.DecorrenzaOriginaria)
                {
                    messaggioVideo = "Il campo 'Data Evento Eliminazione Contestuale' deve essere successiva alla data 'Decorrenza Pensione'";
                    return false;
                }

                DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione));
                if (datiGenerici.DataEvento.HasValue && Utility.DataStrettamenteSuccessivaA(datiGenerici.DataEvento.Value, dataSistema))
                {
                    messaggioVideo = "La Data Evento non può essere successiva alla data odierna";
                    return false;
                }
            }
            return true;
        }

        private static bool ControlsDatiGenericiPT(Entity.DatiGenerici datiGenerici, GestionePensione.DatiPensione datiPensione, bool isDomandaConNuovaGestioneDatiFondoFSPT, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                if (!datiGenerici.fondoPT.DecorrenzaEconomica.HasValue)
                {
                    messaggioVideo = "Campo 'Decorrenza Economica' obbligatorio";
                    return false;
                }

                if (datiGenerici.fondoPT.DecorrenzaEconomica.Value < datiPensione.DecorrenzaOriginaria.Value)
                {
                    messaggioVideo = "La 'Decorrenza Economica' deve essere maggiore o uguale alla 'Decorrenza Giuridica'";
                    return false;
                }
            }

            if (datiGenerici.DecorrenzaCalcoloArretrati.HasValue && datiGenerici.DecorrenzaCalcoloArretrati.Value < Utility.FirstDayOfMonth(datiPensione.DecorrenzaOriginaria.Value))
            {
                messaggioVideo = "La 'Decorrenza Arretrati' deve essere maggiore o uguale alla 'Decorrenza Pensione'";
                return false;
            }

            return true;
        }

        private static bool ControlsDatiGenericiFS(Entity.DatiGenerici datiGenerici, GestionePensione.DatiPensione datiPensione, bool isDomandaConNuovaGestioneDatiFondoFSPT, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                if (!datiGenerici.fondoFST.DecorrenzaEconomica.HasValue)
                {
                    messaggioVideo = "Campo 'Decorrenza Economica' obbligatorio";
                    return false;
                }

                if (datiGenerici.fondoFST.DecorrenzaEconomica.Value != datiPensione.DecorrenzaOriginaria.Value)
                {
                    messaggioVideo = "La 'Decorrenza Economica' deve essere uguale alla 'Decorrenza Giuridica'";
                    return false;
                }
            }

            if (datiGenerici.DecorrenzaCalcoloArretrati.HasValue && datiGenerici.DecorrenzaCalcoloArretrati.Value < Utility.FirstDayOfMonth(datiPensione.DecorrenzaOriginaria.Value))
            {
                messaggioVideo = "La 'Decorrenza Arretrati' deve essere maggiore o uguale alla 'Decorrenza Pensione'";
                return false;
            }

            return true;
        }

        private static void EliminaDatiGenericiPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneFondo.DatiFondo datiFondo,
            Entity.DatiDL407 datiDL407, Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici, Entity.DatiPrivilegiate datiPrivilegiate, Entity.DatiArticolo2 datiArticolo2, ref GestionePagamento.DatiPagamento datiPagamento)
        {
            Entity.DatiGenerici datiGenerici = new Entity.DatiGenerici();

            #region gestione TipoFondo

            Liquidazione.BLCommon.Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        datiGenerici.fondoEL = new Entity.DatiGenerici.FondoEL();
                        break;
                    case Utility.TipoFondo.TT:
                        datiGenerici.fondoTT = new Entity.DatiGenerici.FondoTT();
                        break;
                    case Utility.TipoFondo.ET:
                        datiGenerici.fondoET = new Entity.DatiGenerici.FondoET();
                        break;
                    case Utility.TipoFondo.VL:
                        datiGenerici.fondoVL = new Entity.DatiGenerici.FondoVL();
                        break;
                    case Utility.TipoFondo.PT:
                        datiGenerici.fondoPT = new Entity.DatiGenerici.FondoPT();
                        break;
                    case Utility.TipoFondo.FS:
                        datiGenerici.fondoFST = new Entity.DatiGenerici.FondoFST();
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        datiGenerici.fondoPI = new Entity.DatiGenerici.FondoPI();
                        break;
                    case Utility.TipoFondo.GAS:
                        datiGenerici.fondoGAS = new Entity.DatiGenerici.FondoGAS();
                        break;
                    case Utility.TipoFondo.DZ:
                        datiGenerici.fondoDZ = new Entity.DatiGenerici.FondoDZ();
                        break;
                    case Utility.TipoFondo.ES:
                        datiGenerici.fondoES = new Entity.DatiGenerici.FondoES();
                        break;
                    case Utility.TipoFondo.PM:
                        datiGenerici.fondoPM = new Entity.DatiGenerici.FondoPM();
                        break;
                }
            }
            #endregion gestione TipoFondo

            #region Gestione interazione Precedente Pensione

            Entity.DatiPrecedentePensione datiPrecedentePensione = null;
            GestioneLiquidazionePensione.ValorizzaDatiPrecedentePensione(datiIstruttoria, out datiPrecedentePensione);
            if (datiPrecedentePensione != null && !datiPrecedentePensione.IsDatiPrecedentePensioneNull())
                datiGenerici.TrasformazioneAOI = true;

            #endregion Gestione interazione Precedente Pensione

            #region Gestione Sperimentale Donna
            if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "O")
                datiGenerici.NaturaPensione = " O ";
            #endregion Gestione Sperimentale Donna

            StoreDatiGenerici(ref contenitore, ref contenitoreDecodifica, datiPensione, ref datiIstruttoria, ref datiFondo, datiGenerici, true, datiDL407, datiExCombattente, datiBenefici, datiPrivilegiate, datiArticolo2, null, null, true, ref datiPagamento);
        }

        private static void EliminaDatiGenericiINPDAPPrivate(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref GestioneFondo.DatiFondo datiFondo,
            ref List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP, ref GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione,
            ref GestionePensione.DatiEliminazione datiEliminazione, ref GestionePagamento.DatiPagamento datiPagamento, bool isRiaperturaDomanda)
        {
            Entity.DatiGenericiINPDAP datiGenericiINPDAP = new Entity.DatiGenericiINPDAP();

            #region Gestione Sperimentale Donna
            if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "O")
                datiGenericiINPDAP.NaturaPensione = " O ";
            #endregion Gestione Sperimentale Donna

            StoreDatiGenericiINPDAP(ref contenitore, ref contenitoreDecodifica, datiPensione, datiGenericiINPDAP, null, ref datiIstruttoria, ref datiFondo, ref listaDatiPensioneINPDAP, ref datiQuadroLiquidazionePensione,
                ref datiEliminazione, ref datiPagamento, isRiaperturaDomanda, true, true);
        }

        private static bool ControlsDatiGenericiForPrecedentePensione(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Entity.DatiGenerici datiGenerici)
        {
            if (!datiGenerici.TrasformazioneAOI.HasValue || !datiGenerici.TrasformazioneAOI.Value)
                return ControlsDatiGenericiForPrecedentePensione(datiIstruttoria);
            return true;
        }

        private static bool ControlsDatiGenericiForPrecedentePensione(GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            Entity.DatiPrecedentePensione datiPrecedentePensione = null;
            GestioneLiquidazionePensione.ValorizzaDatiPrecedentePensione(datiIstruttoria, out datiPrecedentePensione);
            if (datiPrecedentePensione != null)
                return false;
            return true;
        }

        private static bool ControlsDatiGenericiForMaggBeneficiByIdPensione(bool? exCombattente, bool? benefici, bool? dl407,
            bool? privilegiata, bool? articolo2, bool IsDeleteOperation, Utility.TipoFondo? tipoFondo,
            Entity.DatiDL407 datiDL407, Entity.DatiExCombattente datiExCombattente, Entity.DatiBenefici datiBenefici,
            Entity.DatiPrivilegiate datiPrivilegiate, Entity.DatiArticolo2 datiArticolo2, bool isDomandaConNuovaGestioneDatiFondoFSPT, out string errore)
        {
            errore = string.Empty;

            if ((dl407.HasValue && !dl407.Value) || (IsDeleteOperation && dl407.HasValue && dl407.Value))
            {
                //Entity.DatiDL407 datiDL407 = null;
                //GestioneMaggiorazioniBenefici.GetDatiDL407ByIdPensione(idPensione, out datiDL407);
                if (datiDL407 != null && !datiDL407.IsDL407Null())
                {
                    errore = "Eliminare i dati DL407 di Maggiorazione / Benefici prima di procedere.";
                    return false;
                }

            }
            if (exCombattente.HasValue && !exCombattente.Value || (IsDeleteOperation && exCombattente.HasValue && exCombattente.Value))
            {
                //Entity.DatiExCombattente datiExCombattente = null;
                //GestioneMaggiorazioniBenefici.ValorizzaDatiExCombattente(idPensione, out datiExCombattente);
                if (tipoFondo == Utility.TipoFondo.FS)
                {
                    if (datiExCombattente != null && !datiExCombattente.IsDatiExCombattenteNull())
                    {
                        errore = "Eliminare i dati Ex Combattente di Maggiorazione / Benefici prima di procedere.";
                        return false;
                    }
                }
                else
                {
                    if (datiExCombattente != null && (!datiExCombattente.IsDatiExCombattenteNull() || datiExCombattente.RMSSenzaLegge33670QA.HasValue))
                    {
                        errore = "Eliminare i dati Ex Combattente di Maggiorazione / Benefici prima di procedere.";
                        return false;
                    }
                }
            }
            if (benefici.HasValue && !benefici.Value || (IsDeleteOperation && benefici.HasValue && benefici.Value))
            {
                //Entity.DatiBenefici datiBenefici = null;
                //GestioneMaggiorazioniBenefici.ValorizzaDatiBeneficiByIdPensione(idPensione, out datiBenefici);
                if (datiBenefici != null && !datiBenefici.IsDatiBeneficiNull())
                {
                    errore = "Eliminare i dati Benefici di Maggiorazione / Benefici prima di procedere.";
                    return false;
                }
            }
            if (!isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                if (privilegiata.HasValue && !privilegiata.Value || (IsDeleteOperation && privilegiata.HasValue && privilegiata.Value))
                {
                    //Entity.DatiPrivilegiate datiPrivilegiate = null;
                    //GestioneMaggiorazioniBenefici.GetDatiPrivilegiateByIdPensione(idPensione, siglaCategoria, out datiPrivilegiate);
                    if (datiPrivilegiate != null && !datiPrivilegiate.IsDatiPrivilegiateNull())
                    {
                        errore = "Eliminare i dati Pensione Privilegiate di Maggiorazione / Benefici prima di procedere.";
                        return false;
                    }
                }
                if (articolo2.HasValue && !articolo2.Value || (IsDeleteOperation && articolo2.HasValue && articolo2.Value))
                {
                    //Entity.DatiArticolo2 datiArticolo2 = null;
                    //GestioneMaggiorazioniBenefici.GetDatiArticolo2ByIdPensione(idPensione, out datiArticolo2);
                    if (datiArticolo2 != null && !datiArticolo2.IsDatiArticolo2Null())
                    {
                        errore = "Eliminare i dati Articolo 2 di Maggiorazione / Benefici prima di procedere.";
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool ControlsDatiGenericiForDatiContributivi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, byte? TipoCalcolo, bool IsDeleteOperation, Utility.TipoFondo? tipoFondo)
        {
            if (TipoCalcolo.HasValue)
            {
                List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
                List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
                GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.DZ)
                {
                    listaDatiCalcoloRetributivo = contenitore.ListaDatiRetributivi;
                    listaDatiCalcoloContributivo = contenitore.ListaDatiContributivi;
                }
                else
                {
                    datiCalcoloContributivo = contenitore.DatiContributivi;
                    datiCalcoloRetributivo = contenitore.DatiRetributivi;
                }

                GestioneFondo.DatiFondo datiFondoGenerici = contenitore.DatiFondo;
                DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : null);

                //Per il fondo ET e le domanda ante armonizzazione EL e TT vengono presi in considerazione sia i DatiServizioUtile che i dati retributivi
                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET || (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.EL || tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.TT) &&
                    Utility.IsDomandaAnteArmonizzazione(contenitore.DatiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondoXX: contenitore.DatiFondoTT, datiFondo: datiFondoGenerici)))
                {
                    List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = contenitore.ListaDatiServizioUtile;
                    if (datiCalcoloContributivo != null || (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0) || datiCalcoloRetributivo != null)
                    {
                        if (!IsDeleteOperation)
                        {
                            List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = contenitoreDecodifica.ElencoTipoCalcolo;
                            GestioneDecodifica.TipoCalcolo tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == TipoCalcolo.Value.ToString());
                            if (tipoCalcolo != null)
                            {
                                if ((tipoCalcolo.TraduzioneSuGP == 1 && datiCalcoloContributivo != null && TipoCalcolo.Value != 25) ||
                                    (tipoCalcolo.TraduzioneSuGP == 4 && ((listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0) || datiCalcoloRetributivo != null)) ||
                                    (tipoCalcolo.TraduzioneSuGP == 3 && (datiCalcoloContributivo == null || (!(listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0) && datiCalcoloRetributivo == null))) ||
                                    (tipoCalcolo.TraduzioneSuGP == 1 && (datiCalcoloContributivo == null || (!(listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0) && datiCalcoloRetributivo == null) || datiCalcoloContributivo.IsQuotaL335Presente()) && TipoCalcolo.Value == 25))
                                    return false;
                            }
                            //Se non è calcolo retributivo monti e sono salvati i dati comma 707 allora non è possibile cambiare tipo calcolo
                            if (TipoCalcolo.Value != 25 && datiFondoGenerici != null && !datiFondoGenerici.IsDatiComma707Null())
                                return false;
                        }
                        else
                            return false;
                    }
                }
                else if (datiCalcoloContributivo != null || datiCalcoloRetributivo != null)
                {
                    if (!IsDeleteOperation)
                    {
                        List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = contenitoreDecodifica.ElencoTipoCalcolo;
                        GestioneDecodifica.TipoCalcolo tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == TipoCalcolo.Value.ToString());
                        if (tipoCalcolo != null)
                        {
                            if ((tipoCalcolo.TraduzioneSuGP == 1 && datiCalcoloContributivo != null && TipoCalcolo.Value != 25) ||
                                (tipoCalcolo.TraduzioneSuGP == 4 && datiCalcoloRetributivo != null) ||
                                (tipoCalcolo.TraduzioneSuGP == 3 && (datiCalcoloContributivo == null || datiCalcoloRetributivo == null)) ||
                                (tipoCalcolo.TraduzioneSuGP == 1 && (datiCalcoloContributivo == null || datiCalcoloRetributivo == null || datiCalcoloContributivo.IsQuotaL335Presente()) && TipoCalcolo.Value == 25))
                                return false;
                        }
                        //Se non è calcolo retributivo monti e sono salvati i dati comma 707 allora non è possibile cambiare tipo calcolo
                        if (TipoCalcolo.Value != 25 && datiFondoGenerici != null && !datiFondoGenerici.IsDatiComma707Null())
                            return false;
                    }
                    else
                        return false;
                }
                else if ((listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count() > 0) || (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count() > 0))
                {
                    if (!IsDeleteOperation)
                    {
                        List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = contenitoreDecodifica.ElencoTipoCalcolo;
                        GestioneDecodifica.TipoCalcolo tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == TipoCalcolo.Value.ToString());
                        if (tipoCalcolo != null)
                        {
                            if ((tipoCalcolo.TraduzioneSuGP == 1 && listaDatiCalcoloContributivo != null && TipoCalcolo.Value != 25) ||
                               (tipoCalcolo.TraduzioneSuGP == 4 && listaDatiCalcoloRetributivo != null) ||
                               (tipoCalcolo.TraduzioneSuGP == 3 && (listaDatiCalcoloContributivo == null || listaDatiCalcoloRetributivo == null)) ||
                               (tipoCalcolo.TraduzioneSuGP == 1 && (listaDatiCalcoloContributivo == null || listaDatiCalcoloRetributivo == null) && TipoCalcolo.Value == 25))
                                return false;
                        }
                        //Se non è calcolo retributivo monti e sono salvati i dati comma 707 allora non è possibile cambiare tipo calcolo
                        if (TipoCalcolo.Value != 25 && datiFondoGenerici != null && !datiFondoGenerici.IsDatiComma707Null())
                            return false;
                    }
                    else
                        return false;
                }
            }
            return true;
        }

        private static bool ControlsDatiGenericiForDatiContributiviFS_PT(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, byte? TipoCalcolo, bool IsDeleteOperation)
        {
            if (TipoCalcolo.HasValue)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
                List<GestioneCalcolo.DatiCalcoloContributivo> ldaticalcolocontributivo = contenitore.ListaDatiCalcoloContributivoRecordFondo;
                List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = contenitore.ListaDatiServizioUtile;
                List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707 = contenitore.ListaDatiServizioUtile707;

                if ((ldaticalcolocontributivo != null && ldaticalcolocontributivo.Count > 0) || (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0))
                {
                    if (!IsDeleteOperation)
                    {
                        List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = contenitoreDecodifica.ElencoTipoCalcolo;
                        GestioneDecodifica.TipoCalcolo tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == TipoCalcolo.Value.ToString());

                        if (ldaticalcolocontributivo != null && ldaticalcolocontributivo.Count > 0)
                        {
                            foreach (GestioneCalcolo.DatiCalcoloContributivo contr in ldaticalcolocontributivo)
                            {
                                if (tipoCalcolo != null)
                                {
                                    if ((tipoCalcolo.TraduzioneSuGP == 1 && contr != null && TipoCalcolo.Value != 25) ||
                                        (tipoCalcolo.TraduzioneSuGP == 4 && (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)) ||
                                        (tipoCalcolo.TraduzioneSuGP == 3 && (contr == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0))) ||
                                        (tipoCalcolo.TraduzioneSuGP == 1 && (contr == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0) || contr.IsQuotaL335Presente()) && TipoCalcolo.Value == 25))
                                        return false;
                                }
                                //Se non è calcolo retributivo monti e sono salvati i dati comma 707 allora non è possibile cambiare tipo calcolo
                                if (TipoCalcolo.Value != 25 && listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
                                    return false;
                            }
                        }
                        else
                        {
                            if (tipoCalcolo != null)
                            {
                                if ((tipoCalcolo.TraduzioneSuGP == 1 && datiCalcoloContributivo != null && TipoCalcolo.Value != 25) ||
                                    (tipoCalcolo.TraduzioneSuGP == 4 && (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)) ||
                                    (tipoCalcolo.TraduzioneSuGP == 3 && (datiCalcoloContributivo == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0))) ||
                                    (tipoCalcolo.TraduzioneSuGP == 1 && (datiCalcoloContributivo == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0) || datiCalcoloContributivo.IsQuotaL335Presente()) && TipoCalcolo.Value == 25))
                                    return false;
                            }
                            //Se non è calcolo retributivo monti e sono salvati i dati comma 707 allora non è possibile cambiare tipo calcolo
                            if (TipoCalcolo.Value != 25 && listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
                                return false;
                        }
                    }
                    else
                        return false;
                }
            }
            return true;
        }

        private static bool ControlsDatiGenericiForDatiContributiviINPDAP(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, long idPensione, byte? TipoCalcolo, bool IsDeleteOperation)
        {
            if (TipoCalcolo.HasValue)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
                List<GestioneCalcolo.DatiCalcoloContributivo> ldaticalcolocontributivo = contenitore.ListaDatiCalcoloContributivoRecordFondo;
                List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtile = contenitore.ListaDatiServizioUtileINPDAP;
                List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtile707 = contenitore.ListaDatiServizioUtile707INPDAP;

                if ((ldaticalcolocontributivo != null && ldaticalcolocontributivo.Count > 0) || (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0))
                {
                    if (!IsDeleteOperation)
                    {
                        List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = contenitoreDecodifica.ElencoTipoCalcolo;
                        GestioneDecodifica.TipoCalcolo tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == TipoCalcolo.Value.ToString());

                        if (ldaticalcolocontributivo != null && ldaticalcolocontributivo.Count > 0)
                        {
                            foreach (GestioneCalcolo.DatiCalcoloContributivo contr in ldaticalcolocontributivo)
                            {
                                if (tipoCalcolo != null)
                                {
                                    if ((tipoCalcolo.TraduzioneSuGP == 1 && contr != null && TipoCalcolo.Value != 25) ||
                                        (tipoCalcolo.TraduzioneSuGP == 4 && (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)) ||
                                        (tipoCalcolo.TraduzioneSuGP == 3 && (contr == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0))) ||
                                        (tipoCalcolo.TraduzioneSuGP == 1 && (contr == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0) || contr.IsQuotaL335Presente()) && TipoCalcolo.Value == 25))
                                        return false;
                                }
                                //Se non è calcolo retributivo monti e sono salvati i dati comma 707 allora non è possibile cambiare tipo calcolo
                                if (TipoCalcolo.Value != 25 && listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
                                    return false;
                            }
                        }
                        else
                        {
                            if (tipoCalcolo != null)
                            {
                                if ((tipoCalcolo.TraduzioneSuGP == 1 && datiCalcoloContributivo != null && TipoCalcolo.Value != 25) ||
                                    (tipoCalcolo.TraduzioneSuGP == 4 && (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)) ||
                                    (tipoCalcolo.TraduzioneSuGP == 3 && (datiCalcoloContributivo == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0))) ||
                                    (tipoCalcolo.TraduzioneSuGP == 1 && (datiCalcoloContributivo == null || (listaDatiServizioUtile == null || listaDatiServizioUtile.Count == 0) || datiCalcoloContributivo.IsQuotaL335Presente()) && TipoCalcolo.Value == 25))
                                    return false;
                            }
                            //Se non è calcolo retributivo monti e sono salvati i dati comma 707 allora non è possibile cambiare tipo calcolo
                            if (TipoCalcolo.Value != 25 && listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
                                return false;
                        }
                    }
                    else
                        return false;
                }
            }
            return true;
        }

        private static void GetCodiciNaturaCustom(GestionePensione.DatiPensione datiPensione, ref List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_FS)
        {
            if (datiPensione != null)
            {
                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
                if (elencoCodiciNaturaCommon_FS != null && elencoCodiciNaturaCommon_FS.Count > 0)
                {
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                    if (tipoFondo.HasValue)
                    {
                        if (tipoFondo == Utility.TipoFondo.PL) tipoFondo = Utility.TipoFondo.PI;
                        elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => x.Fondo == tipoFondo.ToString());
                    }                        
                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                        elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => x.Fondo == "FS");

                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

                    List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaCommon_FSApp = elencoCodiciNaturaCommon_FS.ToList();

                    foreach (GestioneDecodifica.CodiciNatura codiceNatura in elencoCodiciNaturaCommon_FSApp)
                    {
                        if (codiceNatura.Posizione.GetValueOrDefault() == 1)
                        {
                            switch (codiceNatura.TraduzioneSuGP)
                            {
                                // Il codice 0 è visibile solo se è già presente sui dati (si verifica solo nel caso in cui il valore sia prelevato da Host)
                                case '0':
                                    if (string.IsNullOrEmpty(datiPensione.NaturaPensione) || datiPensione.NaturaPensione.Substring(0, 1) != "0")
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                                case '1':
                                    if (tipoFondo.HasValue)
                                    {
                                        switch (tipoFondo.Value)
                                        {
                                            case Utility.TipoFondo.ES:
                                                if (datiPensione.Gruppo == "0002")
                                                    elencoCodiciNaturaCommon_FS.SingleOrDefault(x => x == codiceNatura).Descrizione = "Invalidità privilegiata";
                                                else if (datiPensione.Gruppo == "0003")
                                                    elencoCodiciNaturaCommon_FS.SingleOrDefault(x => x == codiceNatura).Descrizione = "Superstiti privilegiata";
                                                break;
                                            case Utility.TipoFondo.PM:
                                                if (datiPensione.Gruppo == "0002")
                                                    elencoCodiciNaturaCommon_FS.SingleOrDefault(x => x == codiceNatura).Descrizione = "Invalidità privilegiata (PM-PMS)";
                                                else if (datiPensione.Gruppo == "0003")
                                                    elencoCodiciNaturaCommon_FS.SingleOrDefault(x => x == codiceNatura).Descrizione = "Superstiti privilegiata (PM-PMS)";
                                                break;
                                        }
                                    }
                                    //Per inpdap G/P 0001/0002 primo byte codice natura ammessi solo blank e 6
                                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                                case '2':
                                    if (tipoFondo.HasValue)
                                    {
                                        switch (tipoFondo.Value)
                                        {
                                            case Utility.TipoFondo.ES:
                                            case Utility.TipoFondo.PM:
                                                if (datiPensione.Gruppo == "0002")
                                                    elencoCodiciNaturaCommon_FS.SingleOrDefault(x => x == codiceNatura).Descrizione = "Invalidità privilegiata tit. altra pensione";
                                                break;
                                        }
                                    }
                                    //Per inpdap G/P 0001/0002 primo byte codice natura ammessi solo blank e 6
                                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                                case '7':
                                    if (!Utility.IsCTPSPrivilegio(datiPensione) && !(Utility.IsDomandaINPDAP(datiPensione.Gestione) && (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaReversibilita(datiPensione))))
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                                default:
                                    //Per inpdap G/P 0001/0001 primo byte codice natura ammessi solo 1 e 2
                                    //Per inpdap G/P 0001/0002 primo byte codice natura ammessi solo blank e 6
                                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.Gruppo == "0001")
                                    {
                                        if (datiPensione.Prodotto == "0001")
                                            elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                        else if (datiPensione.Prodotto == "0002" && codiceNatura.TraduzioneSuGP != ' ' && codiceNatura.TraduzioneSuGP != '6')
                                            elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    }
                                    break;
                            }
                        }

                        if (codiceNatura.Posizione.GetValueOrDefault() == 2)
                        {
                            switch (codiceNatura.TraduzioneSuGP)
                            {
                                case 'O':
                                    if (!(Utility.IsDomandaSperimentaleDonna(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) ||
                                        (tipoDomanda == Utility.TipoDomanda.Ricostituzione && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1).Equals("O")) ||
                                        (Utility.IsDomandaReversibilita(datiPensione) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1).Equals("O")) ||
                                        Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                                        Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true)))
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                                case 'J':
                                    if (!(Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                                        (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                                        (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;

                                case 'K':
                                    if (!Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL(datiPensione))
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;

                                case 'W':
                                    if (!Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL(datiPensione))
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                            }
                        }

                        if (codiceNatura.Posizione.GetValueOrDefault() == 3)
                        {
                            switch (codiceNatura.TraduzioneSuGP)
                            {
                                case 'V':
                                    if ((!Utility.IsDomandaINPDAP(datiPensione.Gestione) || datiPensione.NaturaPensione == null || !datiPensione.NaturaPensione.Substring(2, 1).Equals("V")) && !Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                                case 'Z':
                                    if (!(Utility.IsDomandaINPDAP(datiPensione.Gestione) && (isRiaperturaDomanda || Utility.IsRicostituzione(datiPensione.Gruppo)) && datiPensione.NaturaPensione != null && datiPensione.NaturaPensione.Substring(2, 1).Equals("Z")) && tipoFondo.HasValue && tipoFondo != Utility.TipoFondo.ET)
                                        elencoCodiciNaturaCommon_FS.Remove(codiceNatura);
                                    break;
                            }
                        }
                    }

                    var primoByte = datiPensione.NaturaPensione != null ? datiPensione.NaturaPensione.Substring(0, 1) : null;
                    
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.PM:
                                if (tipoDomanda == Utility.TipoDomanda.Superstiti || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti)
                                    elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => (x.Posizione.Value != 2 || (x.Posizione == 2 && x.TraduzioneSuGP == 'P')));
                                break;
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                                if (categoriaFondoPI.HasValue)
                                {
                                    switch (categoriaFondoPI.Value)
                                    {
                                        case Utility.CategoriaFondoPI.U:
                                        case Utility.CategoriaFondoPI.V:
                                            elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => (x.Posizione.Value != 3 || (x.Posizione == 3 && (x.TraduzioneSuGP == 'P' || x.TraduzioneSuGP == 'S'))));
                                            break;
                                    }
                                    
                                }
                                if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                                {
                                    char primoByteChar = (!string.IsNullOrEmpty(primoByte) && primoByte.Length == 1) ? primoByte[0] : ' ';

                                    if (elencoCodiciNaturaCommon_FS.Find(x => x.Posizione.Value == 1 && x.TraduzioneSuGP == primoByteChar) == null)
                                    {
                                        elencoCodiciNaturaCommon_FS.Add(
                                            new GestioneDecodifica.CodiciNatura(
                                                new Liquidazione.DataCommon.DecodificaCodiciNatura
                                                {
                                                    Posizione = 1,
                                                    TraduzioneSuGP = primoByteChar,
                                                    Fondo = "PI"
                                                }));
                                    }
                                }
                                break;
                        }
                    }

                   
                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaReversibilita(datiPensione)))
                    {
                        if (datiPensione.SiglaCategoria.StartsWith("V") && datiPensione.NaturaPensione != null && !Utility.IsDomandaReversibilita(datiPensione)
                            && !Utility.IsDomandaRicPensioneOrdinariaCambioPrivilegio(datiPensione))
                        {
                            if (primoByte == "1" || primoByte == "2")
                                elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => x.Posizione.Value != 1 || (x.Posizione == 1 && (x.TraduzioneSuGP == '1' || x.TraduzioneSuGP == '2')));
                            else if (primoByte == "6" || primoByte == " ")
                                elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => x.Posizione.Value != 1 || (x.Posizione == 1 && (x.TraduzioneSuGP == '6' || x.TraduzioneSuGP == ' ')));
                        }

                        if (primoByte == "7" || (Utility.IsDomandaRicPensioneInabilitaCambioPrivilegio(datiPensione) || Utility.IsDomandaRicPensioneIndirettaOrdinariaCambioPrivilegio(datiPensione) ||
                            Utility.IsDomandaRicPensioneOrdinariaCambioPrivilegio(datiPensione)))
                            elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => x.Posizione.Value != 1 || (x.Posizione == 1 && (x.TraduzioneSuGP == '7')));
                        else
                            elencoCodiciNaturaCommon_FS = elencoCodiciNaturaCommon_FS.FindAll(x => !(x.Posizione == 1 && (x.TraduzioneSuGP == '7')));

                    }
                }
            }
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

            Utility.ValorizzaOggetti(datiGenerici, datiPagamento);

            if (datiPagamento.Equals(new GestionePagamento.DatiPagamento()))
            {
                GestionePagamento.EliminaPagamentoByIdPensione(datiPensione.Id);
                datiPagamento = null;
            }
            else
                GestionePagamento.SalvaPagamento(datiPensione.Id, datiPagamento);
        }

        #endregion dati Generici

        #region Dati Assicurativi

        private static void StoreDatiAssicurativiPerFondoDatiGenerici(long idPensione, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondo datiFondo,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, bool isDatiServizioUtilePresenti, out bool eliminaFondoDatiGenerici)
        {
            eliminaFondoDatiGenerici = false;
            if (datiFondo == null)
            {
                if (datiAssicurativi.IsFondoDatiGenericiNull())
                    return;
                else
                    datiFondo = new GestioneFondo.DatiFondo();
            }
            Utility.ValorizzaOggetti(datiAssicurativi, datiFondo);

            if (datiFondo.IsFondoNull())
            {
                if (isDomandaConNuovaGestioneDatiFondoFSPT || isDatiServizioUtilePresenti)
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
                else
                    eliminaFondoDatiGenerici = true;
            }
            else
                GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
            return;
        }

        private static void StoreDatiAssicurativiINPDAPPerPensione(Entity.DatiAssicurativiINPDAP datiAssicurativi, GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                datiAssicurativi.InizioAssicurazione = datiPensione.InizioAssicurazione;
                datiAssicurativi.FineAssicurazione = datiPensione.FineAssicurazione;
                //**Revisione Campi INPDAP**
                //datiAssicurativi.AttivitaEconomica = datiPensione.AttivitaEconomica;
                //datiAssicurativi.ProfessioneIndividuale = datiPensione.ProfessioneIndividuale;
            }

            Utility.ValorizzaOggetti(datiAssicurativi, datiPensione);
            GestionePensione.SalvaPensione(datiPensione);
        }

        private static void StoreDatiAssicurativiINPDAPPerPensioneFondoDatiGenerici(GestionePensione.DatiPensione datiPensione, DatiAssicurativiINPDAP datiAssicurativi,
            ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondo == null)
            {
                if (datiAssicurativi.IsDatiAssicurativiINPDAPPensioniFondoDatiGenericiNull())
                    return;
                else
                    datiFondo = new GestioneFondo.DatiFondo();
            }

            Utility.ValorizzaOggetti(datiAssicurativi, datiFondo);
            if (datiFondo.Equals(new GestioneFondo.DatiFondo()))
            {
                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                datiFondo = null;
            }
            else
                GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondo);
        }

        private static void StoreDatiAssicurativiINPDAPPerPensioneINPDAP(GestionePensione.DatiPensione datiPensione, Entity.DatiAssicurativiINPDAP datiAssicurativiINPDAP,
           ref List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP)
        {
            if (listaDatiPensioneINPDAP == null || listaDatiPensioneINPDAP.Count == 0)
            {
                if (datiAssicurativiINPDAP == null || datiAssicurativiINPDAP.IsDatiAssicurativiINPDAPPensioneINPDAPNull())
                    return;
                else
                {
                    listaDatiPensioneINPDAP = new List<GestionePensioneINPDAP.DatiPensioneINPDAP>();
                    GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP = new GestionePensioneINPDAP.DatiPensioneINPDAP();
                    listaDatiPensioneINPDAP.Add(datiPensioneINPDAP);
                }
            }

            foreach (GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP in listaDatiPensioneINPDAP)
            {
                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                {
                    datiAssicurativiINPDAP.VVUtiliDirittoAA = datiPensioneINPDAP.VVUtiliDirittoAA;
                    datiAssicurativiINPDAP.VVUtiliDirittoMM = datiPensioneINPDAP.VVUtiliDirittoMM;
                    datiAssicurativiINPDAP.VVUtiliDirittoGG = datiPensioneINPDAP.VVUtiliDirittoGG;
                    datiAssicurativiINPDAP.VVUtiliMisuraAA = datiPensioneINPDAP.VVUtiliMisuraAA;
                    datiAssicurativiINPDAP.VVUtiliMisuraMM = datiPensioneINPDAP.VVUtiliMisuraMM;
                    datiAssicurativiINPDAP.VVUtiliMisuraGG = datiPensioneINPDAP.VVUtiliMisuraGG;
                    if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN)
                    {
                        datiAssicurativiINPDAP.Microqualifica = datiPensioneINPDAP.Microqualifica;
                        //**Revisione Campi INPDAP**
                        //datiAssicurativiINPDAP.AnniMax = datiPensioneINPDAP.AnniMax;
                        //datiAssicurativiINPDAP.AnniUtili = datiPensioneINPDAP.AnniUtili;
                    }

                }
                datiAssicurativiINPDAP.CausaCessazione = datiAssicurativiINPDAP.CausaCessazione.HasValue ? datiAssicurativiINPDAP.CausaCessazione : null;

                Utility.ValorizzaOggetti(datiAssicurativiINPDAP, datiPensioneINPDAP);

                GestionePensioneINPDAP.SalvaPensioneINPDAPRecordFondo(datiPensione.Id, datiPensioneINPDAP.IdRecordFondo.GetValueOrDefault(), datiPensioneINPDAP);
            }
        }

        private static void StoreDatiRipartizioneINPDAP(GestionePensione.DatiPensione datiPensione, List<Entity.RipartizioneINPDAP> listaRipartizioneINPDAP, ref List<GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP> datiRipartizioneINPDAP)
        {

            // Se i dati arrivano da Felpe non vanno modificati
            if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
            {
                GestioneRipartizioneINPDAP.EliminaRipartizioneINPDAPByIdPensione(datiPensione.Id);
                datiRipartizioneINPDAP = null;

                if (listaRipartizioneINPDAP != null && listaRipartizioneINPDAP.Count > 0)
                {
                    datiRipartizioneINPDAP = new List<GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP>();
                    foreach (var ripartizioneINPDAP in listaRipartizioneINPDAP)
                    {
                        GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP ripartizione = new GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP();
                        Utility.ValorizzaOggetti(ripartizioneINPDAP, ripartizione);
                        ripartizione.IdPensione = datiPensione.Id;
                        GestioneRipartizioneINPDAP.SalvaRipartizioneINPDAP(ripartizione);
                        datiRipartizioneINPDAP.Add(ripartizione);
                    }
                }
            }
        }

        private static void StoreDatiAssicurativiPerFondoEL(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoEL datiFondoEL, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoEL == null)
            {
                if (datiAssicurativi.fondoEL == null || datiAssicurativi.fondoEL.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoEL = new GestioneFondo.DatiFondoEL();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoEL, datiFondoEL);
            if (datiFondoEL.Equals(new GestioneFondo.DatiFondoEL()))
            {
                GestioneFondo.EliminaFondoEL(idPensione);
                datiFondoEL = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoEL(idFondo, datiFondoEL);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoET(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoET datiFondoET, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoET == null)
            {
                if (datiAssicurativi.fondoET == null || datiAssicurativi.fondoET.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoET = new GestioneFondo.DatiFondoET();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoET, datiFondoET);
            if (datiFondoET.Equals(new GestioneFondo.DatiFondoET()))
            {
                GestioneFondo.EliminaFondoET(idPensione);
                datiFondoET = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoET(idFondo, datiFondoET);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoTT(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoTT datiFondoTT, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoTT == null)
            {
                if (datiAssicurativi.fondoTT == null || datiAssicurativi.fondoTT.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoTT = new GestioneFondo.DatiFondoTT();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoTT, datiFondoTT);
            if (datiFondoTT.Equals(new GestioneFondo.DatiFondoTT()))
            {
                GestioneFondo.EliminaFondoTT(idPensione);
                datiFondoTT = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoTT(idFondo, datiFondoTT);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoVL(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoVL datiFondoVL, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoVL == null)
            {
                if (datiAssicurativi.fondoVL == null || datiAssicurativi.fondoVL.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoVL = new GestioneFondo.DatiFondoVL();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoVL, datiFondoVL);
            if (datiFondoVL.Equals(new GestioneFondo.DatiFondoVL()))
            {
                GestioneFondo.EliminaFondoVL(idPensione);
                datiFondoVL = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoVL(idFondo, datiFondoVL);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoPT(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref List<GestioneFondo.DatiFondoPT> listaDatiFondoPT,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo, ref EntityBLCommon.ContenitoreObject contenitore)
        {
            if (listaDatiFondoPT == null || listaDatiFondoPT.Count == 0)
            {
                if (datiAssicurativi.fondoPT == null || datiAssicurativi.fondoPT.IsFondoNull())
                {
                    if (!isDomandaConNuovaGestioneDatiFondoFSPT && eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                {
                    listaDatiFondoPT = new List<GestioneFondo.DatiFondoPT>();
                    GestioneFondo.DatiFondoPT datiFondoPT = new GestioneFondo.DatiFondoPT();
                    listaDatiFondoPT.Add(datiFondoPT);
                }
            }

            if (isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                foreach (GestioneFondo.DatiFondoPT datiFondoPT in listaDatiFondoPT)
                {
                    datiFondoPT.CausaCessazione = datiAssicurativi.fondoPT.CausaCessazione;
                    datiFondoPT.IndennitaIntegrativaSpecialeConglobata = datiAssicurativi.fondoPT.IndennitaIntegrativaSpecialeConglobata;
                    if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.DatiLavorazione)))
                        datiFondoPT.DirittoIndennitaIntegrativaSpeciale = datiAssicurativi.fondoPT.DirittoIndennitaIntegrativaSpeciale;
                    datiFondoPT.RiduzioneL537 = datiAssicurativi.fondoPT.RiduzioneL537;
                    datiFondoPT.IISAbbattimentoAnni = datiAssicurativi.fondoPT.IISAbbattimentoAnni;
                    datiFondoPT.OnereMEF = datiAssicurativi.fondoPT.OnereMEF;
                    datiFondoPT.RipartizioneInpdap = datiAssicurativi.fondoPT.RipartizioneInpdap;
                }
            }
            else
            {
                foreach (GestioneFondo.DatiFondoPT datiFondoPT in listaDatiFondoPT)
                    Utility.ValorizzaOggetti(datiAssicurativi.fondoPT, datiFondoPT);
            }

            if (isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                foreach (GestioneFondo.DatiFondoPT datiFondoPT in listaDatiFondoPT)
                {
                    //NOTA: il record fondo verrà sempre elimitato da gestioneFondo e il primo record sarà 
                    //presente dall'acquiszione della domanda e non verrà mai elimitato per PensioneFondoPT.

                    //if (!datiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                    //{
                    if (idFondo == 0 || eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                        idFondo = datiFondoNew.Id;
                        datiFondo = datiFondoNew;
                    }
                    GestioneFondo.SalvaFondoPTRecordFondo(idFondo, datiFondoPT.IdRecordFondo.Value, datiFondoPT);

                    //}
                }
            }
            else
            {
                if (listaDatiFondoPT.First().Equals(new GestioneFondo.DatiFondoPT()))
                {
                    GestioneFondo.EliminaFondoPT(idPensione);
                    listaDatiFondoPT = null;
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                }
                else
                {
                    if (idFondo == 0 || eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                        idFondo = datiFondoNew.Id;
                        datiFondo = datiFondoNew;
                    }
                    GestioneFondo.SalvaFondoPT(idFondo, listaDatiFondoPT.First());
                }
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoFST(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref List<GestioneFondo.DatiFondoFST> listaDatiFondoFST,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo, ref EntityBLCommon.ContenitoreObject contenitore)
        {
            if (listaDatiFondoFST == null || listaDatiFondoFST.Count == 0)
            {
                if (datiAssicurativi.fondoFST == null || datiAssicurativi.fondoFST.IsFondoNull())
                {
                    if (!isDomandaConNuovaGestioneDatiFondoFSPT && eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                {
                    listaDatiFondoFST = new List<GestioneFondo.DatiFondoFST>();
                    GestioneFondo.DatiFondoFST datiFondoFST = new GestioneFondo.DatiFondoFST();
                    listaDatiFondoFST.Add(datiFondoFST);
                }
            }

            if (isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                foreach (GestioneFondo.DatiFondoFST datiFondoFST in listaDatiFondoFST)
                {
                    datiFondoFST.CausaCessazione = datiAssicurativi.fondoFST.CausaCessazione;
                    datiFondoFST.IndennitaIntegrativaSpecialeConglobata = datiAssicurativi.fondoFST.IndennitaIntegrativaSpecialeConglobata;
                    datiFondoFST.TitolareAltraPensione = datiAssicurativi.fondoFST.TitolareAltraPensione;
                    if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.DatiLavorazione)))
                        datiFondoFST.DirittoIndennitaIntegrativaSpeciale = datiAssicurativi.fondoFST.DirittoIndennitaIntegrativaSpeciale;
                    datiFondoFST.RiduzioneL537 = datiAssicurativi.fondoFST.RiduzioneL537;
                    datiFondoFST.IISAbbattimentoAnni = datiAssicurativi.fondoFST.IISAbbattimentoAnni;

                    if (!datiFondoFST.Equals(new GestioneFondo.DatiFondoFST()))
                    {
                        if (idFondo == 0 || eliminaFondoDatiGenerici)
                        {
                            GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                            idFondo = datiFondoNew.Id;
                            datiFondo = datiFondoNew;
                        }
                        GestioneFondo.SalvaFondoFSTRecordFondo(idFondo, datiFondoFST.IdRecordFondo.Value, datiFondoFST);

                    }
                }
            }
            else
            {
                foreach (GestioneFondo.DatiFondoFST datiFondoFST in listaDatiFondoFST)
                    Utility.ValorizzaOggetti(datiAssicurativi.fondoFST, datiFondoFST);

                if (listaDatiFondoFST.First().Equals(new GestioneFondo.DatiFondoFST()))
                {
                    GestioneFondo.EliminaFondoFST(idPensione);
                    listaDatiFondoFST = null;
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                }
                else
                {
                    if (idFondo == 0 || eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                        idFondo = datiFondoNew.Id;
                        datiFondo = datiFondoNew;
                    }
                    GestioneFondo.SalvaFondoFST(idFondo, listaDatiFondoFST.First());
                }
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoPI(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, List<long> listaDatiRecordFondo, ref GestioneFondo.DatiFondoPI datiFondoPI, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo, ref List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile)
        {
            if (datiFondoPI == null)
            {
                if (datiAssicurativi.fondoPI == null || datiAssicurativi.fondoPI.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoPI = new GestioneFondo.DatiFondoPI();
            }

            //Gestione Dati Servizio Utile
            if (datiAssicurativi.fondoPI.ServizioUtile == null || datiAssicurativi.fondoPI.ServizioUtile.IsDatiServizioUtileNull())
            {
                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(idPensione);
                listaDatiServizioUtile = null;
            }

            Utility.ValorizzaOggetti(datiAssicurativi.fondoPI, datiFondoPI);
            if (datiFondoPI.Equals(new GestioneFondo.DatiFondoPI()))
            {
                GestioneFondo.EliminaFondoPI(idPensione);
                datiFondoPI = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoPIRecordFondo(idFondo, datiFondoPI.IdRecordFondo, datiFondoPI);
                
            }

            if (datiAssicurativi.fondoPI.ServizioUtile != null && !datiAssicurativi.fondoPI.ServizioUtile.IsDatiServizioUtileNull())
            {
                if (listaDatiServizioUtile == null)
                    listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();

                GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon = new GestioneDatiServizioUtile.ServizioUtile();
                Utility.ValorizzaOggetti(datiAssicurativi.fondoPI.ServizioUtile, servizioUtileCommon);
                GestioneDatiServizioUtile.SalvaDatiServizioUtile(idFondo, servizioUtileCommon);
                listaDatiServizioUtile.Add(servizioUtileCommon);
            }

            GestioneFondo.SalvaFondoPIEmpty(idFondo, listaDatiRecordFondo);

            return;
        }

        private static void StoreDatiAssicurativiPerFondoGAS(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoGAS datiFondoGAS, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoGAS == null)
            {
                if (datiAssicurativi.fondoGAS == null || datiAssicurativi.fondoGAS.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoGAS = new GestioneFondo.DatiFondoGAS();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoGAS, datiFondoGAS);
            if (datiFondoGAS.Equals(new GestioneFondo.DatiFondoGAS()))
            {
                GestioneFondo.EliminaFondoGAS(idPensione);
                datiFondoGAS = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoGAS(idFondo, datiFondoGAS);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoCL(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoCL datiFondoCL, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoCL == null)
            {
                if (datiAssicurativi.fondoCL == null || datiAssicurativi.fondoCL.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoCL = new GestioneFondo.DatiFondoCL();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoCL, datiFondoCL);
            if (datiFondoCL.Equals(new GestioneFondo.DatiFondoCL()))
            {
                GestioneFondo.EliminaFondoCL(idPensione);
                datiFondoCL = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoCL(idFondo, datiFondoCL);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoDZ(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoDZ datiFondoDZ, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoDZ == null)
            {
                if (datiAssicurativi.fondoDZ == null || datiAssicurativi.fondoDZ.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoDZ = new GestioneFondo.DatiFondoDZ();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoDZ, datiFondoDZ);
            if (datiFondoDZ.Equals(new GestioneFondo.DatiFondoDZ()))
            {
                GestioneFondo.EliminaFondoDZ(idPensione);
                datiFondoDZ = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoDZ(idFondo, datiFondoDZ);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoES(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoES datiFondoES, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoES == null)
            {
                if (datiAssicurativi.fondoES == null || datiAssicurativi.fondoES.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoES = new GestioneFondo.DatiFondoES();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoES, datiFondoES);
            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
            {
                GestioneFondo.EliminaFondoES(idPensione);
                datiFondoES = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoES(idFondo, datiFondoES);
            }
            return;
        }

        private static void StoreDatiAssicurativiPerFondoPM(long idPensione, long idFondo, Entity.DatiAssicurativi datiAssicurativi, ref GestioneFondo.DatiFondoPM datiFondoPM, bool eliminaFondoDatiGenerici, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondoPM == null)
            {
                if (datiAssicurativi.fondoPM == null || datiAssicurativi.fondoPM.IsFondoNull())
                {
                    if (eliminaFondoDatiGenerici)
                    {
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                        datiFondo = null;
                    }
                    return;
                }
                else
                    datiFondoPM = new GestioneFondo.DatiFondoPM();
            }
            Utility.ValorizzaOggetti(datiAssicurativi.fondoPM, datiFondoPM);
            if (datiFondoPM.Equals(new GestioneFondo.DatiFondoPM()))
            {
                GestioneFondo.EliminaFondoPM(idPensione);
                datiFondoPM = null;
                if (eliminaFondoDatiGenerici)
                {
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    datiFondo = null;
                }

            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                GestioneFondo.SalvaFondoPM(idFondo, datiFondoPM);
            }
            return;
        }

        private static void GetDatiAssicurativiWithFondiByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, Utility.TipoFondo? tipoFondo, ref Entity.DatiAssicurativi datiAssicurativi)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    #region FondoEL
                    case Utility.TipoFondo.EL:
                        GestioneFondo.DatiFondoEL datiFondoEL = contenitore.DatiFondoEL;
                        if (datiFondoEL != null)
                        {
                            datiAssicurativi.fondoEL = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi.FondoEL();
                            Utility.ValorizzaOggetti(datiFondoEL, datiAssicurativi.fondoEL);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoEL == null || datiAssicurativi.fondoEL.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoEL

                    #region FondoTT
                    case Utility.TipoFondo.TT:
                        GestioneFondo.DatiFondoTT datiFondoTT = contenitore.DatiFondoTT;
                        if (datiFondoTT != null)
                        {
                            datiAssicurativi.fondoTT = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi.FondoTT();
                            Utility.ValorizzaOggetti(datiFondoTT, datiAssicurativi.fondoTT);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoTT == null || datiAssicurativi.fondoTT.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoTT

                    #region FondoET
                    case Utility.TipoFondo.ET:
                        GestioneFondo.DatiFondoET datiFondoET = contenitore.DatiFondoET;
                        if (datiFondoET != null)
                        {
                            datiAssicurativi.fondoET = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi.FondoET();
                            Utility.ValorizzaOggetti(datiFondoET, datiAssicurativi.fondoET);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoET == null || datiAssicurativi.fondoET.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoET

                    #region FondoVL
                    case Utility.TipoFondo.VL:
                        GestioneFondo.DatiFondoVL datiFondoVL = contenitore.DatiFondoVL;
                        if (datiFondoVL != null)
                        {
                            datiAssicurativi.fondoVL = new LiquidazioneFs.Entity.DatiAssicurativi.FondoVL();
                            Utility.ValorizzaOggetti(datiFondoVL, datiAssicurativi.fondoVL);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoVL == null || datiAssicurativi.fondoVL.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoVL

                    #region FondoPT
                    case Utility.TipoFondo.PT:
                        List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = contenitore.ListaDatiFondoPT;
                        if (listaDatiFondoPT != null && listaDatiFondoPT.Count() > 0)
                        {
                            datiAssicurativi.fondoPT = new Entity.DatiAssicurativi.FondoPT();
                            Utility.ValorizzaOggetti(listaDatiFondoPT.First(), datiAssicurativi.fondoPT);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoPT == null || datiAssicurativi.fondoPT.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoPT

                    #region FondoFST
                    case Utility.TipoFondo.FS:
                        List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = contenitore.ListaDatiFondoFST;
                        if (listaDatiFondoFST != null && listaDatiFondoFST.Count > 0)
                        {
                            datiAssicurativi.fondoFST = new Entity.DatiAssicurativi.FondoFST();
                            Utility.ValorizzaOggetti(listaDatiFondoFST.First(), datiAssicurativi.fondoFST);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoFST == null || datiAssicurativi.fondoFST.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoFST

                    #region FondoPI
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.DatiFondoPI datiFondoPI = contenitore.DatiFondoPI;
                        List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtile = contenitore.ListaDatiServizioUtile;

                        if (datiFondoPI != null)
                        {
                            datiAssicurativi.fondoPI = new Entity.DatiAssicurativi.FondoPI();
                            Utility.ValorizzaOggetti(datiFondoPI, datiAssicurativi.fondoPI);
                        }

                        if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
                        {
                            if (datiAssicurativi.fondoPI == null)
                                datiAssicurativi.fondoPI = new Entity.DatiAssicurativi.FondoPI();

                            datiAssicurativi.fondoPI.ServizioUtile = new Entity.DatiAssicurativi.DatiServizioUtile();
                            foreach (GestioneDatiServizioUtile.ServizioUtile servUtile in lDatiServizioUtile)
                            {
                                Utility.ValorizzaOggetti(servUtile, datiAssicurativi.fondoPI.ServizioUtile);
                            }
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoPI == null || datiAssicurativi.fondoPI.IsFondoNull())
                                datiAssicurativi = null;

                        break;
                    #endregion FondoPI

                    #region FondoGAS
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.DatiFondoGAS datiFondoGAS = contenitore.DatiFondoGAS;
                        if (datiFondoGAS != null)
                        {
                            datiAssicurativi.fondoGAS = new Entity.DatiAssicurativi.FondoGAS();
                            Utility.ValorizzaOggetti(datiFondoGAS, datiAssicurativi.fondoGAS);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoGAS == null || datiAssicurativi.fondoGAS.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoGAS

                    #region FondoCL
                    case Utility.TipoFondo.CL:
                        GestioneFondo.DatiFondoCL datiFondoCL = contenitore.DatiFondoCL;
                        List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = contenitore.ListaDatiServizioUtile;
                        if (datiFondoCL != null)
                        {
                            datiAssicurativi.fondoCL = new Entity.DatiAssicurativi.FondoCL();
                            Utility.ValorizzaOggetti(datiFondoCL, datiAssicurativi.fondoCL);
                        }

                        if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                        {
                            if (datiAssicurativi.fondoCL == null)
                                datiAssicurativi.fondoCL = new Entity.DatiAssicurativi.FondoCL();

                            //prendiamo direttamente la posizione 0 perchè possiamo avere al max un solo record
                            Utility.ValorizzaOggetti(listaDatiServizioUtile[0], datiAssicurativi.fondoCL);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoCL == null || datiAssicurativi.fondoCL.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoCL

                    #region FondoDZ
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.DatiFondoDZ datiFondoDZ = contenitore.DatiFondoDZ;
                        if (datiFondoDZ != null)
                        {
                            datiAssicurativi.fondoDZ = new Entity.DatiAssicurativi.FondoDZ();
                            Utility.ValorizzaOggetti(datiFondoDZ, datiAssicurativi.fondoDZ);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoDZ == null || datiAssicurativi.fondoDZ.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoDZ

                    #region FondoES
                    case Utility.TipoFondo.ES:
                        GestioneFondo.DatiFondoES datiFondoES = contenitore.DatiFondoES;
                        if (datiFondoES != null)
                        {
                            datiAssicurativi.fondoES = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi.FondoES();
                            Utility.ValorizzaOggetti(datiFondoES, datiAssicurativi.fondoES);
                        }

                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                            if (datiAssicurativi.fondoES == null || datiAssicurativi.fondoES.IsFondoNull())
                                datiAssicurativi = null;
                        break;
                    #endregion FondoES

                    #region FondoPM
                    case Utility.TipoFondo.PM:
                        GestioneFondo.DatiFondoPM datiFondoPM = contenitore.DatiFondoPM;
                        if (datiFondoPM != null)
                        {
                            datiAssicurativi.fondoPM = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi.FondoPM();
                            Utility.ValorizzaOggetti(datiFondoPM, datiAssicurativi.fondoPM);
                        }
                        if (datiAssicurativi.IsFondoDatiGenericiNull())
                        {
                            if (datiAssicurativi.fondoPM == null || datiAssicurativi.fondoPM.IsFondoNull())
                                datiAssicurativi = null;
                        }
                        break;
                    #endregion FondoPM
                }
            }
        }

        private static void PrevalorizzazioneDatiAssicurativi(ref EntityBLCommon.ContenitoreObject contenitore, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, ref Entity.DatiAssicurativi datiAssicurativi,
            out bool disableCodSpecifico, out bool disableCodArt22)
        {
            disableCodSpecifico = false;
            disableCodArt22 = false;
            byte? codSpecifico = null;
            byte? codArt22 = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.VL:
                        codSpecifico = Utility.CalcolaCodiceSpecificoForVolo(datiPensione);
                        codArt22 = Utility.CalcolaArticolo22ForVolo(datiPensione);

                        if (codSpecifico.HasValue)
                        {
                            disableCodSpecifico = true;
                            if (datiAssicurativi == null)
                                datiAssicurativi = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi();
                            datiAssicurativi.CodiceSpecifico = codSpecifico;
                        }

                        if (codArt22.HasValue)
                        {
                            disableCodArt22 = true;
                            if (datiAssicurativi == null)
                                datiAssicurativi = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi();
                            if (datiAssicurativi.fondoVL == null)
                                datiAssicurativi.fondoVL = new INPS.Pensioni.LiquidazioneFs.Entity.DatiAssicurativi.FondoVL();
                            datiAssicurativi.fondoVL.CodiceArt22 = codArt22;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                        byte? codiceSpecifico = null;
                        
                        if(contenitore.DatiFondo != null)
                            codiceSpecifico = contenitore.DatiFondo.CodiceSpecifico;

                        bool isRicostituzioneOrRiapertura = Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda);

                        if (isRicostituzioneOrRiapertura)

                        {
                            if (codiceSpecifico.HasValue && ((codiceSpecifico.Value == 188 || codiceSpecifico.Value == 189)
                                || (codiceSpecifico.Value == 142 || codiceSpecifico.Value == 187)))
                            {
                                disableCodSpecifico = true;
                            }
                            else if (!Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                            {
                                GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;
                                if (datiStoricoGP != null && datiStoricoGP.CodiceSpecifico.HasValue)
                                {
                                    disableCodSpecifico = true;
                                }
                            }
                        }
                        else if (Utility.IsDomandaInabilitaLegge335(datiPensione))
                        {
                            disableCodSpecifico = true;
                        }
                        break;
                }
            }
        }

        private static bool ControlDatiAssicurativiWithFondi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneFondo.DatiFondo datiFondo, Entity.DatiAssicurativi datiAssicurativi, Entity.DatiGenerici datiGenerici, string tipoSettimaneBeneficio, bool isDomandaConNuovaGestioneDatiFondoFSPT, string attivitaSvoltaTraduzioneSuGP,
            char? codiceSpecificoTraduzioneSuGP, bool isRiaperturaDomanda, List<Entity.RecordFondo> listaRecordFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = contenitore.DatiAnagraficiTitolare;
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = contenitore.DatiAnagraficiDanteCausa;
            GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = contenitore.DatiControlloFelpe;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;

            if (datiGenerici == null)
                GestioneLiquidazionePensione.GetDatiGenerici(ref contenitore, datiPensione, datiIstruttoria, datiFondo, datiControlloFelpe, out datiGenerici);

            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            char? tipoPensione = datiAssicurativi.TipoPensione;
            if (!tipoPensione.HasValue)
            {
                try
                {
                    tipoPensione = GestioneLiquidazionePensione.GetTipoPensione(datiPensione).First().Value;
                }
                catch (Exception)
                {
                    tipoPensione = Utility.GeTipoPensioneByCodeProdotto(datiPensione.Prodotto);
                }
            }

            object datiFondoXX = null;
            GestioneCrossControls.TipoDecPensione? tipoDecPensione;
            if (tipoFondo.HasValue)
            {
                tipoDecPensione = GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo);

                switch (tipoFondo)
                {
                    #region FondoEL
                    case Utility.TipoFondo.EL:

                        if (tipoDecPensione.HasValue &&
                           (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia) &&
                            (datiGenerici == null || datiGenerici.fondoEL == null || datiGenerici.fondoEL.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                GestioneFondo.DatiFondoEL datiFondoEL = new GestioneFondo.DatiFondoEL();
                                Utility.ValorizzaOggetti(datiGenerici.fondoEL, datiFondoEL);
                                datiFondoXX = datiFondoEL;
                            }
                        }
                        if (datiAssicurativi.fondoEL.DecorrenzaTeorica.HasValue)
                        {
                            if (datiGenerici != null && datiGenerici.InizioBonus.HasValue &&
                                datiAssicurativi.fondoEL.DecorrenzaTeorica.Value.Date != datiGenerici.InizioBonus.Value.Date)
                            {
                                messaggioVideo = "La data di decorrenza teorica deve coincidere con la data di inizio bonus indicata nel tab 'Dati Generici'";
                                return false;
                            }
                        }

                        if (datiAssicurativi.fondoEL.DecorrenzaTeorica > datiPensione.DecorrenzaOriginaria)
                        {
                            messaggioVideo = "La data 'Decorrenza Teorica' non deve superare la data 'Decorrenza Pensione'";
                            return false;
                        }

                        if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                        {
                            messaggioVideo = "L'attività svolta è obbligatoria";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.AnnoRiscatti.HasValue && datiAssicurativi.fondoEL.AnnoRiscatti.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Riscatti' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.MeseRiscatti.HasValue && datiAssicurativi.fondoEL.MeseRiscatti.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Riscatti' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.AnnoAnzianitaPregressa.HasValue && datiAssicurativi.fondoEL.AnnoAnzianitaPregressa.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Anzianità Pregressa' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.MeseAnzianitaPregressa.HasValue && datiAssicurativi.fondoEL.MeseAnzianitaPregressa.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Anzianità Pregressa' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.AnnoServizioMilitare.HasValue && datiAssicurativi.fondoEL.AnnoServizioMilitare.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Servizio Militare' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.MeseServizioMilitare.HasValue && datiAssicurativi.fondoEL.MeseServizioMilitare.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Servizio Militare' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.AnnoArt3Legge107971.HasValue && datiAssicurativi.fondoEL.AnnoArt3Legge107971.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Articolo 3 Legge 1079' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoEL.MeseArt3Legge107971.HasValue && datiAssicurativi.fondoEL.MeseArt3Legge107971.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Articolo 3 Legge 1079' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }

                        break;
                    #endregion FondoEL

                    #region FondoTT
                    case Utility.TipoFondo.TT:
                        GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneAnteArmonizzazioneTT", out controlloDinamico);

                        if (!GestioneControlli.VerificaRetribuzioneMensileINAILPerTT(datiAssicurativi.fondoTT.RetribuzioneMensileInail, out messaggioVideo))
                            return false;

                        if (controlloDinamico == null || controlloDinamico.ValoreControllo == "NO")
                        {
                            if (datiAssicurativi.fondoTT != null && !datiAssicurativi.fondoTT.IsFondoNull() && datiAssicurativi.fondoTT.DimissioniAnte97.HasValue && datiAssicurativi.fondoTT.DimissioniAnte97.Value)
                            {
                                messaggioVideo = "Liquidazione non abilitata per le Domande ante 01/07/1997";
                                return false;
                            }
                        }

                        if (tipoDecPensione.HasValue &&
                           (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia) &&
                           (datiGenerici == null || datiGenerici.fondoTT == null || datiGenerici.fondoTT.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                GestioneFondo.DatiFondoTT datiFondoTT = new GestioneFondo.DatiFondoTT();
                                Utility.ValorizzaOggetti(datiGenerici.fondoTT, datiFondoTT);
                                datiFondoXX = datiFondoTT;
                            }
                        }

                        if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                        {
                            messaggioVideo = "L'attività svolta è obbligatoria";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.DecorrenzaTeorica.HasValue)
                        {
                            if (datiGenerici != null && datiGenerici.InizioBonus.HasValue &&
                                datiAssicurativi.fondoTT.DecorrenzaTeorica.Value.Date != datiGenerici.InizioBonus.Value.Date)
                            {
                                messaggioVideo = "La data di decorrenza teorica deve coincidere con la data di inizio bonus indicata nel tab 'Dati Generici'";
                                return false;
                            }
                        }

                        if (datiAssicurativi.fondoTT.DecorrenzaTeorica > datiPensione.DecorrenzaOriginaria)
                        {
                            messaggioVideo = "La data 'Decorrenza Teorica' non deve superare la data 'Decorrenza Pensione'";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.PeriodiFigurativiAnni.HasValue && datiAssicurativi.fondoTT.PeriodiFigurativiAnni.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Periodi figurativi Anni' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.PeriodiFigurativiMesi.HasValue && datiAssicurativi.fondoTT.PeriodiFigurativiMesi.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Periodi figurativi Mese' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.PeriodiFigurativiGiorni.HasValue && datiAssicurativi.fondoTT.PeriodiFigurativiGiorni.Value > 29)
                        {
                            messaggioVideo = "Il giorno del campo 'Periodi figurativi Giorno' può assumere un valore compreso tra 0 e 29";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.RiscattiContributiFissiAnni.HasValue && datiAssicurativi.fondoTT.RiscattiContributiFissiAnni.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Riscatti Contributi Fissi Anni' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.RiscattiContributiFissiMesi.HasValue && datiAssicurativi.fondoTT.RiscattiContributiFissiMesi.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Riscatti Contributi Fissi Mesi' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.RiscattiContributiFissiGiorni.HasValue && datiAssicurativi.fondoTT.RiscattiContributiFissiGiorni.Value > 29)
                        {
                            messaggioVideo = "Il giorno del campo 'Riscatti Contributi Fissi Giorno' può assumere un valore compreso tra 0 e 29";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.RiscattiRiservaMatematicaAnni.HasValue && datiAssicurativi.fondoTT.RiscattiRiservaMatematicaAnni.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Riscatti Riserva matematica Anni' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.RiscattiRiservaMatematicaMesi.HasValue && datiAssicurativi.fondoTT.RiscattiRiservaMatematicaMesi.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Riscatti Riserva matematica Mese' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoTT.RiscattiRiservaMatematicaGiorni.HasValue && datiAssicurativi.fondoTT.RiscattiRiservaMatematicaGiorni.Value > 29)
                        {
                            messaggioVideo = "Il giorno del campo 'Riscatti Riserva matematica Giorni' può assumere un valore compreso tra 0 e 29";
                            return false;
                        }

                        GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
                        GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;
                        GestioneDL407.DatiDL407 datiDL407 = null;
                        List<GestioneDatiServizioUtile.ServizioUtile> lstServizioUtile = null;
                        Utility.TipoCalcolo tipoCalcolo = datiGenerici != null ? Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione).GetValueOrDefault()) : Utility.TipoCalcolo.NonValido;
                        if (!GestioneCrossControls.FS_ControlsDecorrenzaPostAnteArmonizzazione(datiPensione, datiDanteCausa, ref datiContributivi, ref datiRetributivi, ref datiDL407,
                            ref lstServizioUtile, ref datiFondo, ref tipoCalcolo, out messaggioVideo, datiAssicurativi.fondoTT.DimissioniAnte97))
                            return false;

                        break;
                    #endregion FondoTT

                    #region FondoET
                    case Utility.TipoFondo.ET:

                        if (!GestioneControlli.VerificaImporto13maImporto14maPerET(datiAssicurativi, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaDecorenzaTeoricaContributivoPerET(datiPensione, datiAssicurativi, out messaggioVideo))
                            return false;

                        if (tipoDecPensione.HasValue &&
                           (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia) &&
                           datiPensione.DataPerfezionamentoRequisiti.HasValue && datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2011, 01, 01)) < 0 &&
                           (datiGenerici == null || datiGenerici.fondoET == null || datiGenerici.fondoET.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                GestioneFondo.DatiFondoET datiFondoET = new GestioneFondo.DatiFondoET();
                                Utility.ValorizzaOggetti(datiGenerici.fondoET, datiFondoET);
                                datiFondoXX = datiFondoET;
                            }
                        }

                        if ((!datiPensione.Gruppo.Equals("0002") || !datiPensione.Prodotto.Equals("0011") || !datiPensione.Tipo.Equals("0001")) && !datiAssicurativi.fondoET.DataEsonero.HasValue)
                        {
                            messaggioVideo = "La data 'Cessazione Iscrizione' è obbligatoria";
                            return false;
                        }

                        if (!datiAssicurativi.fondoET.DecorrenzaTeorica.HasValue)
                        {
                            messaggioVideo = "La data 'Decorrenza Teorica' è obbligatoria";
                            return false;
                        }

                        //ENG - Memo 123/2024
                        GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

                        //ENG - Memo 123/2024
                        GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                        if (!Utility.IsDomandaTipoContributivo(datiPensione, null, null) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) && !Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione) &&
                            !((!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                              ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))) //ENG - Memo 166/2023
                        {
                            if (datiAssicurativi.fondoET.DecorrenzaTeorica.HasValue)
                            {
                                if (datiGenerici != null && datiGenerici.InizioBonus.HasValue &&
                                    datiAssicurativi.fondoET.DecorrenzaTeorica.Value.Date != datiGenerici.InizioBonus.Value.Date)
                                {
                                    messaggioVideo = "La data 'Decorrenza Teorica' deve coincidere con la data di 'Inizio Bonus' indicata nel tab 'Dati Generici'";
                                    return false;
                                }
                            }

                            if (datiAssicurativi.fondoET.DecorrenzaTeorica > datiPensione.DecorrenzaOriginaria)
                            {
                                messaggioVideo = "La data 'Decorrenza Teorica' non deve superare la data 'Decorrenza Pensione'";
                                return false;
                            }
                        }

                        if (datiAssicurativi.fondoET.GradoInvalidita.HasValue && datiAssicurativi.fondoET.GradoInvalidita.Value > 0)
                        {
                            if (tipoPensione != '2')
                            {
                                messaggioVideo = "Grado di invalidità incompatibile con il tipo pensione";
                                return false;
                            }
                            else
                            {
                                char? codNat1 = null;
                                if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                                    codNat1 = listaRecordFondo != null && listaRecordFondo.Count > 0 ? listaRecordFondo[0].CodiceNatura1 : null;
                                else
                                    codNat1 = datiGenerici != null && !string.IsNullOrEmpty(datiGenerici.NaturaPensione) ? datiGenerici.NaturaPensione[0] : (char?)null;

                                if (codNat1 == null || (codNat1 != '1' && codNat1 != '2'))
                                {
                                    messaggioVideo = "Grado di invalidità incompatibile con il codice natura";
                                    return false;
                                }
                            }
                        }

                        if (datiAssicurativi.fondoET.GradoInvalidita.HasValue && datiAssicurativi.fondoET.GradoInvalidita.Value > 79)
                        {
                            if (datiAssicurativi.fondoET.ImportoRenditaInail.HasValue || datiAssicurativi.fondoET.RetribuzioneEffettiva.HasValue)
                            {
                                messaggioVideo = "Rendità Inail annua e/o Retribuzione effettiva incompatibili con il Grado di invalidità";
                                return false;
                            }
                        }

                        if (!datiAssicurativi.fondoET.CodAzienda.HasValue && !(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
                        {
                            messaggioVideo = "Azienda: si prega di inserire correttamente il codice";
                            return false;
                        }

                        if (!datiAssicurativi.fondoET.Stipendio.HasValue)
                        {
                            messaggioVideo = "Stipendio obbligatorio.";
                            return false;
                        }

                        if (!datiAssicurativi.fondoET.Importo13ma.HasValue)
                        {
                            messaggioVideo = "Tredicesima obbligatoria.";
                            return false;
                        }

                        if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
                        {
                            if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione) || Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione))
                            {
                                if (!datiAssicurativi.fondoET.PersonaleViaggiante.HasValue)
                                {
                                    messaggioVideo = "Personale Viaggiante obbligatorio.";
                                    return false;
                                }

                                if (datiAssicurativi.fondoET.PersonaleViaggiante.HasValue)
                                {
                                    if (datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2014, 1, 1)))
                                    {
                                        List<Entity.PersonaleViaggiante> listaPersonaleViaggiante = null;
                                        GetListaPersonaleViaggiante(ref contenitoreDecodifica, out listaPersonaleViaggiante);
                                        Entity.PersonaleViaggiante personaleViaggiante = listaPersonaleViaggiante.Find(x => x.Id == datiAssicurativi.fondoET.PersonaleViaggiante.Value);

                                        if (Utility.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(datiPensione))
                                        {
                                            if (personaleViaggiante.TraduzioneSuGP != 1)
                                            {
                                                messaggioVideo = "Personale Viaggiante incongruente con prodotto WebDom.";
                                                return false;
                                            }
                                        }
                                        else if (Utility.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(datiPensione))
                                        {
                                            if (personaleViaggiante.TraduzioneSuGP != 2)
                                            {
                                                messaggioVideo = "Personale Viaggiante incongruente con prodotto WebDom.";
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!GestioneControlli.ControlsServizioMilitareFondoET(datiPensione, datiDanteCausa, datiAssicurativi.fondoET.CodiceServizioMilitare, datiAssicurativi.fondoET.NSettimaneLeva,
                            datiAssicurativi.fondoET.NSettimaneRichiamato, datiAssicurativi.fondoET.ContributiAgoLegge40245, datiAssicurativi.fondoET.ContributiAgoLegge140830, tipoFondo, out messaggioVideo))
                            return false;
                        break;

                    #endregion FondoET

                    #region FondoVL
                    case Utility.TipoFondo.VL:
                        if (tipoDecPensione.HasValue &&
                           ((tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia)) &&
                           (datiGenerici == null || datiGenerici.fondoVL == null || datiGenerici.fondoVL.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                GestioneFondo.DatiFondoVL datiFondoVL = new GestioneFondo.DatiFondoVL();
                                Utility.ValorizzaOggetti(datiGenerici.fondoVL, datiFondoVL);
                                datiFondoXX = datiFondoVL;
                            }
                        }

                        if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                        {
                            messaggioVideo = "L'attività svolta è obbligatoria";
                            return false;
                        }

                        if (!datiAssicurativi.fondoVL.CodiceArt22.HasValue)
                        {
                            messaggioVideo = "Il Codice Art.22 è obbligatorio";
                            return false;
                        }

                        if (datiPensione.SiglaCategoria.Substring(0, 1) == "I" && !datiAssicurativi.fondoVL.DataInvalidita.HasValue)
                        {
                            messaggioVideo = "La Data Invalidità è obbligatoria";
                            return false;
                        }

                        if (!(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) && Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC)) &&
                            !GestioneCrossControls.FS_VerificaCoerenzaRetrAGOAnnua(datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaA,
                                datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaB, (datiGenerici != null ?
                                Utility.GetTipoCalcoloById(datiGenerici.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS) : Utility.TipoCalcolo.NonValido), out messaggioVideo))
                            return false;

                        if (datiAssicurativi.fondoVL.ProsecuzioneVolontariaAA.HasValue && datiAssicurativi.fondoVL.ProsecuzioneVolontariaAA.Value.ToString().Length > 2)
                        {
                            messaggioVideo = "Prosecuzione Volontaria AA deve essere compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoVL.ProsecuzioneVolontariaMM.HasValue && (datiAssicurativi.fondoVL.ProsecuzioneVolontariaMM.Value.ToString().Length > 2 || datiAssicurativi.fondoVL.ProsecuzioneVolontariaMM.Value > 11))
                        {
                            messaggioVideo = "Prosecuzione Volontaria MM deve essere compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoVL.ProsecuzioneVolontariaGG.HasValue && (datiAssicurativi.fondoVL.ProsecuzioneVolontariaGG.Value.ToString().Length > 2 || datiAssicurativi.fondoVL.ProsecuzioneVolontariaGG.Value > 31))
                        {
                            messaggioVideo = "Prosecuzione Volontaria GG deve essere compreso tra 0 e 30";
                            return false;
                        }

                        if (datiAssicurativi.fondoVL.RiscattiRicongiunzioniAA.HasValue && datiAssicurativi.fondoVL.RiscattiRicongiunzioniAA.Value.ToString().Length > 2)
                        {
                            messaggioVideo = "Riscatti Ricongiunzioni AA deve essere compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoVL.RiscattiRicongiunzioniMM.HasValue && (datiAssicurativi.fondoVL.RiscattiRicongiunzioniMM.Value.ToString().Length > 2 || datiAssicurativi.fondoVL.RiscattiRicongiunzioniMM.Value > 11))
                        {
                            messaggioVideo = "Riscatti Ricongiunzioni MM deve essere compreso tra 0 e 11";
                            return false;
                        }

                        if (datiAssicurativi.fondoVL.RiscattiRicongiunzioniGG.HasValue && (datiAssicurativi.fondoVL.RiscattiRicongiunzioniGG.Value.ToString().Length > 2 || datiAssicurativi.fondoVL.RiscattiRicongiunzioniGG.Value > 31))
                        {
                            messaggioVideo = "Riscatti Ricongiunzioni GG deve essere compreso tra 0 e 30";
                            return false;
                        }

                        if (!GestioneCrossControls.FS_VerificaDecPensWithDataInvaliditaCodeArt22FondoVL(datiPensione, datiDanteCausa, datiAssicurativi.FineAssicurazione, datiAssicurativi.fondoVL.DataInvalidita,
                                datiAssicurativi.fondoVL.CodiceArt22, datiPensione.DecorrenzaOriginaria, isRiaperturaDomanda, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaAttivitaSvolta_VL(datiPensione, tipoFondo, attivitaSvoltaTraduzioneSuGP, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.ControlsCodiceCapitalizzazione(datiPensione, isRiaperturaDomanda, datiAssicurativi.fondoVL.CodiceCapitalizzazione,
                            datiAssicurativi.fondoVL.ImportoPercentualeCapitalizzazione, out messaggioVideo))
                            return false;

                        break;

                    #endregion FondoVL

                    #region FondoPT
                    case Utility.TipoFondo.PT:
                        if (tipoDecPensione.HasValue &&
                           ((tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia)) &&
                           (datiGenerici == null || datiGenerici.fondoPT == null || datiGenerici.fondoPT.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = new List<GestioneFondo.DatiFondoPT>();
                                GestioneFondo.DatiFondoPT datiFondoPT = new GestioneFondo.DatiFondoPT();
                                Utility.ValorizzaOggetti(datiGenerici.fondoPT, datiFondoPT);
                                listaDatiFondoPT.Add(datiFondoPT);
                                datiFondoXX = listaDatiFondoPT;
                            }
                        }

                        if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                        {
                            if (!(Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione)
                                || (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, contenitore != null ? contenitore.DatiLavorazione : null) && !isRiaperturaDomanda)
                                || (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))))
                            {
                                messaggioVideo = "La qualifica professionale è obbligatoria";
                                return false;
                            }
                        }

                        if (!ControlsDatiAssicurativiFondoPT(datiAssicurativi, datiPensione, isDomandaConNuovaGestioneDatiFondoFSPT, datiGenerici, out messaggioVideo))
                            return false;
                        break;

                    #endregion FondoPT

                    #region FondoFST
                    case Utility.TipoFondo.FS:
                        DateTime? decorrenzaCalcoloCompareFS = new DateTime(1995, 10, 1);

                        if (tipoDecPensione.HasValue &&
                           ((tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia)) &&
                           (datiGenerici == null || datiGenerici.fondoFST == null || datiGenerici.fondoFST.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = new List<GestioneFondo.DatiFondoFST>();
                                GestioneFondo.DatiFondoFST datiFondoFST = new GestioneFondo.DatiFondoFST();
                                Utility.ValorizzaOggetti(datiGenerici.fondoFST, datiFondoFST);
                                listaDatiFondoFST.Add(datiFondoFST);
                                datiFondoXX = listaDatiFondoFST;
                            }
                        }

                        if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                        {
                            if (!(Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione)
                                || (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, contenitore != null ? contenitore.DatiLavorazione : null) && !isRiaperturaDomanda)
                                || (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))))
                            {
                                messaggioVideo = "La qualifica professionale è obbligatoria";
                                return false;
                            }
                        }

                        if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                        {
                            if (!datiAssicurativi.fondoFST.DecorrenzaCalcolo.HasValue)
                            {
                                messaggioVideo = "La Decorrenza Calcolo è obbligatoria";
                                return false;
                            }

                            //ENG - saltare controllo nel caso di prime liquidate di reversibilità (quindi gruppo= 0003 e prodotto= 0021)
                            //ENG - saltare controllo anche per le RIC di reversibilità
                            if (!Utility.IsRicostituzioneOrRiaperturaFSPTPerequata(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria) &&
                                !datiGenerici.InizioBonus.HasValue && datiAssicurativi.fondoFST.DecorrenzaCalcolo.Value != datiPensione.DecorrenzaOriginaria.Value
                                //&& !(datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021")
                                && !Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, contenitore.DatiLavorazione))
                            {
                                messaggioVideo = "La Decorrenza Calcolo deve essere uguale alla Decorrenza Pensione";
                                return false;
                            }

                            if (datiGenerici.InizioBonus.HasValue && datiAssicurativi.fondoFST.DecorrenzaCalcolo.Value != datiGenerici.InizioBonus.Value)
                            {
                                messaggioVideo = "La Decorrenza Calcolo deve essere uguale alla Data Inizio Bonus.";
                                return false;
                            }
                            if (datiAssicurativi.fondoFST.DecorrenzaCalcolo.Value <= datiAssicurativi.FineAssicurazione.Value)
                            {
                                messaggioVideo = "La Decorrenza Calcolo deve essere maggiore della Data Ultimo Versamento";
                                return false;
                            }

                            if (!VerificaDecCalcoloWithDecPensione(datiAssicurativi.fondoFST.DecorrenzaCalcolo.Value, decorrenzaCalcoloCompareFS, datiPensione, datiGenerici != null ? datiGenerici.InizioBonus : null, out messaggioVideo))
                                return false;
                        }

                        int? anni = null;
                        DateTime? data = null;
                        if (!GestioneControlli.VerificaEtaTitolareWithQualificaProfessionale(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo, datiPensione.DataPerfezionamentoRequisiti, datiAssicurativi.AttivitaSvolta,
                            datiGenerici != null ? datiGenerici.fondoFST != null ? datiGenerici.fondoFST.RequisitiAnte247.HasValue : false : false, datiAnagrafici.DataNascita, datiPensione.DecorrenzaOriginaria, datiGenerici != null ? datiGenerici.fondoFST != null ? datiGenerici.fondoFST.TrimesteRequisiti : null : null, datiGenerici != null ? datiGenerici.fondoFST != null ? datiGenerici.fondoFST.AnnoRequisiti : null : null, out anni, out data))
                        {
                            messaggioVideo = "Il Titolare non ha compiuto " + anni + " anni alla data Perfezionamento Requisiti (" + String.Format("{0:dd/MM/yyyy}", data) + ")";
                            return false;
                        }

                        break;

                    #endregion FondoFST

                    #region FondoPI
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        if (tipoDecPensione.HasValue &&
                           ((tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia)) &&
                           (datiGenerici == null || datiGenerici.fondoPI == null || datiGenerici.fondoPI.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                GestioneFondo.DatiFondoPI datiFondoPI = new GestioneFondo.DatiFondoPI();
                                Utility.ValorizzaOggetti(datiGenerici.fondoPI, datiFondoPI);
                                datiFondoXX = datiFondoPI;
                            }
                        }

                        if (categoriaFondoPI != Utility.CategoriaFondoPI.V)
                        {
                            if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                            {
                                messaggioVideo = "L'attività svolta è obbligatoria";
                                return false;
                            }
                        }
             
                        if (!ControlsDatiAssicurativiFondoPI(datiAssicurativi, datiPensione, codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP,
                            datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : datiAnagrafici.DataNascita, datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : datiAnagrafici.Sesso,
                            datiAnagraficiDC != null, out messaggioVideo))
                            return false;
                        break;

                    #endregion FondoPI

                    #region FondoGAS
                    case Utility.TipoFondo.GAS:

                        if (tipoDecPensione.HasValue &&
                           (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia) &&
                            (datiGenerici == null || datiGenerici.fondoGAS == null || datiGenerici.fondoGAS.IsFondoNull()))
                        {
                            messaggioVideo = "Salvare i dati della tab 'Dati Generici' prima di salvare i dati della tab 'Dati Assicurativi'";
                            return false;
                        }
                        else
                        {
                            if (tipoDecPensione.HasValue && (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia))
                            {
                                GestioneFondo.DatiFondoGAS datiFondoGAS = new GestioneFondo.DatiFondoGAS();
                                Utility.ValorizzaOggetti(datiGenerici.fondoGAS, datiFondoGAS);
                                datiFondoXX = datiFondoGAS;
                            }
                        }

                        if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                        {
                            messaggioVideo = "L'attività svolta è obbligatoria";
                            return false;
                        }

                        //// Commentato perchè serve una maggiore analisi
                        //if (!GestioneControlli.VerificaRiscattiUtiliFondoGAS(datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiAssicurativi.fondoGAS.MesiUtiliIndennitaAggiuntiva, out messaggioVideo))
                        //    return false;

                        if ((datiAssicurativi.fondoGAS.ServizioUtileIndennitaAggiuntiva.HasValue || datiAssicurativi.fondoGAS.Retribuzione.HasValue) &&
                            (!datiAssicurativi.fondoGAS.ServizioUtileIndennitaAggiuntiva.HasValue || !datiAssicurativi.fondoGAS.Retribuzione.HasValue))
                        {
                            messaggioVideo = "Se è presente Servizio Utile Indennità Aggiuntiva allora deve essere presente anche Retribuzione Indennità Aggiuntiva e viceversa";
                            return false;
                        }

                        break;
                    #endregion FondoGAS

                    #region FondoCL
                    case Utility.TipoFondo.CL:
                        if (!GestioneControlli.VerificaDatiAssicurativiObbligatori(datiAssicurativi.fondoCL.ServizioUtileAA, datiAssicurativi.fondoCL.ServizioUtileMM,
                            datiAssicurativi.fondoCL.DataPerfezionamentoRequisiti, out messaggioVideo))
                            return false;
                        if (!GestioneControlli.ControlsCapienzaServizioUtile_CL(datiAssicurativi.fondoCL.ServizioUtileAA, datiAssicurativi.fondoCL.ServizioUtileMM, datiAssicurativi.InizioAssicurazione,
                            datiAssicurativi.FineAssicurazione, out messaggioVideo))
                            return false;
                        if (datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "VCL")
                            if (!GestioneControlli.ControlsServizioUtileAAMM_CL(datiAssicurativi.fondoCL.ServizioUtileAA, datiAssicurativi.fondoCL.ServizioUtileMM, datiAssicurativi.fondoCL.CodicePensioneSenzaRequisiti, out messaggioVideo))
                                return false;
                        break;

                    #endregion FondoCL

                    #region FondoDZ
                    case Utility.TipoFondo.DZ:
                        if (datiAssicurativi.fondoDZ.ClasseAnte50.GetValueOrDefault() < 0 || datiAssicurativi.fondoDZ.ClasseAnte50.GetValueOrDefault() > 14)
                        {
                            messaggioVideo = "Il valore Classe Ante 50 deve essere compreso tra 0 e 14";
                            return false;
                        }
                        break;
                    #endregion FondoDZ

                    #region FondoES
                    case Utility.TipoFondo.ES:
                        if (datiAssicurativi.AttivitaSvolta == null || datiAssicurativi.AttivitaSvolta.Trim() == "")
                        {
                            messaggioVideo = "L'attività svolta è obbligatoria";
                            return false;
                        }

                        if (datiAssicurativi.fondoES.AnniRiscatti.HasValue && datiAssicurativi.fondoES.AnniRiscatti.Value > 99)
                        {
                            messaggioVideo = "L'anno del campo 'Riscatti' può assumere un valore compreso tra 0 e 99";
                            return false;
                        }

                        if (datiAssicurativi.fondoES.MesiRiscatti.HasValue && datiAssicurativi.fondoES.MesiRiscatti.Value > 11)
                        {
                            messaggioVideo = "Il mese del campo 'Riscatti' può assumere un valore compreso tra 0 e 11";
                            return false;
                        }
                        break;
                    #endregion FondoES
                }
            }

            if (!GestioneControlli.VerificaEtaTitolareVecchiaia(datiAnagrafici, datiPensione, tipoFondo, datiFondoXX, datiAssicurativi.CodiceRequisiti1, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP,
                out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsDatiAssicurativiFondoPI(Entity.DatiAssicurativi datiAssicurativi, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP,
            string attivitaSvoltaTraduzioneSuGP, DateTime? dataNascita, char? sesso, bool isDanteCausaPresente, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (datiAssicurativi.fondoPI == null)
            {
                messaggioVideo = "Dati fondo PI obbligatori";
                return false;
            }

            if (datiAssicurativi.fondoPI.ServizioUtile != null)
            {
                if (!datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.HasValue)
                {
                    messaggioVideo = "Servizio Utile AA obbligatorio";
                    return false;
                }

                if (!datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.HasValue)
                {
                    messaggioVideo = "Servizio Utile MM obbligatorio";
                    return false;
                }

                if (!datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.HasValue)
                {
                    messaggioVideo = "Servizio Utile GG obbligatorio";
                    return false;
                }
            }

            //if (string.IsNullOrEmpty(datiAssicurativi.fondoPI.Qualifica))
            //{
            //    messaggioVideo = "Qualifica obbligatoria";
            //    return false;
            //}

            if (categoriaFondoPI != Utility.CategoriaFondoPI.U && categoriaFondoPI != Utility.CategoriaFondoPI.V)
            {
                //if (!datiAssicurativi.fondoPI.ControCodiceRetribuzione.HasValue)
                //{
                //    messaggioVideo = "Controcodice Retribuzione obbligatorio";
                //    return false;
                //}

                //if (!datiAssicurativi.fondoPI.StipendioAnnuo.HasValue)
                //{
                //    messaggioVideo = "Stipendio Annuo obbligatorio";
                //    return false;
                //}

                //Per il momento la rendiamo non obbligatoria
                //if (string.IsNullOrEmpty(datiAssicurativi.fondoPI.NumeroMatricola))
                //{
                //    messaggioVideo = "Matricola obbligatoria";
                //    return false;
                //}

                //if (datiAssicurativi.fondoPI.NumeroMatricola.Length != 8)
                //{
                //    messaggioVideo = "Matricola deve avere lunghezza 8";
                //    return false;
                //}

                //if (!CheckImportoPensione(datiAssicurativi, datiPensione, out messaggioVideo))
                //    return false;
            }
            else
            {
                //if (!string.IsNullOrEmpty(datiAssicurativi.fondoPI.Qualifica) && !(new List<string> { "10", "20", "30" }).Contains(datiAssicurativi.fondoPI.Qualifica.Trim()))
                //{
                //    messaggioVideo = "Inquadramento Professionale errato. (Valori ammessi: 10 - non quadri, 20 - quadri, 30 - dirigenti)";
                //    return false;
                //}

                if (!GestioneControlli.VerificaRequisitiEtaPIU_PIV(codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP, datiPensione.DataPerfezionamentoRequisiti, dataNascita,
                    datiAssicurativi.fondoPI.ServizioUtile, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiPensione.DecorrenzaOriginaria, sesso, isDanteCausaPresente,
                    datiPensione.SiglaCategoria.Trim().ToUpper(), out messaggioVideo))
                    return false;
            }

            if (datiAssicurativi.fondoPI.ServizioUtile != null)
            {
                if (!GestioneControlli.IsValoreAAMMGGValido(datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA, null, null))
                {
                    messaggioVideo = "Servizio Utile AA deve essere compreso tra 0 e 99";
                    return false;
                }

                if (!GestioneControlli.IsValoreAAMMGGValido(null, datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM, null))
                {
                    messaggioVideo = "Servizio Utile MM deve essere compreso tra 0 e 11";
                    return false;
                }

                if (!GestioneControlli.IsValoreAAMMGGValido(null, null, datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG))
                {
                    messaggioVideo = "Servizio Utile GG deve essere compreso tra 0 e 29";
                    return false;
                }
            }
            //if (!GestioneControlli.IsValoreAAMMGGValido(datiAssicurativi.fondoPI.RiscattiAA, null, null))
            //{
            //    messaggioVideo = "Riscatti AA deve essere compreso tra 0 e 99";
            //    return false;
            //}

            //if (!GestioneControlli.IsValoreAAMMGGValido(null, datiAssicurativi.fondoPI.RiscattiMM, null))
            //{
            //    messaggioVideo = "Riscatti MM deve essere compreso tra 0 e 11";
            //    return false;
            //}

            //if (!GestioneControlli.IsValoreAAMMGGValido(null, null, datiAssicurativi.fondoPI.RiscattiGG))
            //{
            //    messaggioVideo = "Riscatti GG deve essere compreso tra 0 e 29";
            //    return false;
            //}

            return true;
        }

        private static bool ControlsDatiAssicurativiFondoPT(Entity.DatiAssicurativi datiAssicurativi, GestionePensione.DatiPensione datiPensione, bool isDomandaConNuovaGestioneDatiFondoFSPT,
            Entity.DatiGenerici datiGenerici, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaCalcoloComparePT = new DateTime(1996, 10, 1);

            if (datiAssicurativi.fondoPT == null)
            {
                messaggioVideo = "Dati fondo PT obbligatori";
                return false;
            }

            if (!isDomandaConNuovaGestioneDatiFondoFSPT)
            {
                if (!datiAssicurativi.fondoPT.DecorrenzaCalcolo.HasValue)
                {
                    messaggioVideo = "La Decorrenza Calcolo è obbligatoria";
                    return false;
                }

                if (datiAssicurativi.fondoPT.DecorrenzaCalcolo.Value < datiPensione.DecorrenzaOriginaria.Value)
                {
                    messaggioVideo = "La Decorrenza Calcolo deve essere maggiore o uguale alla Decorrenza Pensione";
                    return false;
                }

                if (datiAssicurativi.fondoPT.DecorrenzaCalcolo.Value <= datiAssicurativi.FineAssicurazione.Value)
                {
                    messaggioVideo = "La Decorrenza Calcolo deve essere maggiore della Data Ultimo Versamento";
                    return false;
                }

                if (!VerificaDecCalcoloWithDecPensione(datiAssicurativi.fondoPT.DecorrenzaCalcolo.Value, decorrenzaCalcoloComparePT, datiPensione, datiGenerici != null ? datiGenerici.InizioBonus : null, out messaggioVideo))
                    return false;
            }

            if (!datiAssicurativi.fondoPT.OnereMEF.HasValue)
            {
                messaggioVideo = "Il campo Onere MEF è obbligatorio";
                return false;
            }

            return true;
        }

        private static bool CheckImportoPensione(Entity.DatiAssicurativi datiAssicurativi, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!GestioneControlli.CheckImportoWithControCodice(datiAssicurativi.fondoPI.StipendioAnnuo.HasValue ? datiAssicurativi.fondoPI.StipendioAnnuo.Value : (decimal?)null,
                    datiAssicurativi.fondoPI.ControCodiceRetribuzione.HasValue ? datiAssicurativi.fondoPI.ControCodiceRetribuzione.Value : (int?)null, datiPensione, out messaggioVideo))
                return false;

            //int? result = datiAssicurativi.fondoPI != null && datiAssicurativi.fondoPI.StipendioAnnuo.HasValue ? Convert.ToInt32(datiAssicurativi.fondoPI.StipendioAnnuo.Value % 999): (int?)null;

            //if (datiAssicurativi.fondoPI.StipendioAnnuo.HasValue && datiAssicurativi.fondoPI.StipendioAnnuo.Value < 1000)
            //{
            //    if (!datiAssicurativi.fondoPI.ControCodiceRetribuzione.Value.Equals((int)datiAssicurativi.fondoPI.StipendioAnnuo.Value))
            //    {
            //        messaggioVideo = "Il campo 'Controcodice Retribuzione' deve coincidere con il campo 'Stipendio Annuo' per valori inferiori a 1000";
            //        return false;
            //    }
            //}
            //else
            //{
            //    if (!datiAssicurativi.fondoPI.ControCodiceRetribuzione.Value.Equals(result.Value))
            //    {
            //        messaggioVideo = "Il campo 'Controcodice Retribuzione' deve essere uguale alla parte decimale della divisione tra 'Stipendio Annuo' e 999 per valori superiori a 1000";
            //        return false;
            //    }
            //}

            return true;
        }

        private static bool VerificaDecCalcoloWithDecPensione(DateTime? decorrenzaCalcolo, DateTime? decorrenzaCalcoloFondoCompare, GestionePensione.DatiPensione datiPensione, DateTime? dataInizioBonus, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            //mail 26-05-2015 
            //Nel caso di domande da bonus (secondo codice natura = Y) occorrerà valorizzare la data calcolo con la data di inizio bonus (assumendo il primo giorno del mese)
            if (dataInizioBonus.HasValue)
            {
                DateTime primoMeseDataInizioBonus = new DateTime(dataInizioBonus.Value.Year, dataInizioBonus.Value.Month, 1);
                if (primoMeseDataInizioBonus != decorrenzaCalcolo)
                {
                    messaggioVideo = "Nel caso di domande da bonus occorrerà valorizzare la 'Decorrenza calcolo' con il primo del mese della data di inizio bonus.";
                    return false;
                }
            }
            else
            {
                DateTime? decorrenzaPensioneCompare = new DateTime(1988, 1, 1);
                DateTime? decorrenzaCalcoloCompare = new DateTime(1992, 1, 1);

                switch (datiPensione.Gruppo)
                {
                    case "0001":
                    case "0002":
                        if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaPensioneCompare.Value) && (decorrenzaCalcolo.Value != decorrenzaCalcoloFondoCompare.Value))
                        {
                            messaggioVideo = string.Format("Per 'Decorrenza Pensione' fino al 01/01/1988, la 'Decorrenza Calcolo' deve essere uguale al {0:dd/MM/yyyy}", decorrenzaCalcoloFondoCompare.Value);
                            return false;
                        }
                        if (Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaPensioneCompare.Value) && !Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaCalcoloCompare.Value) && (decorrenzaCalcoloCompare.Value != decorrenzaCalcolo.Value))
                        {
                            messaggioVideo = "Per 'Decorrenza Pensione' compresa tra 01/01/1988, e 01/01/1992, la 'Decorrenza Calcolo' deve essere uguale al 01/01/1992";
                            return false;
                        }
                        if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaCalcoloCompare.Value) && (decorrenzaCalcolo.Value != datiPensione.DecorrenzaOriginaria.Value))
                        {
                            messaggioVideo = string.Format("Per 'Decorrenza Pensione' maggiore del 01/01/1992, la 'Decorrenza Calcolo' deve essere uguale alla 'Decorrenza Pensione' ({0:dd/MM/yyyy})", datiPensione.DecorrenzaOriginaria.Value);
                            return false;
                        }
                        break;
                    case "0003":
                        if (!Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaCalcoloFondoCompare.Value) && (decorrenzaCalcolo.Value != decorrenzaCalcoloFondoCompare.Value))
                        {
                            messaggioVideo = string.Format("Per 'Decorrenza Pensione' fino al {0:dd/MM/yyyy}, la 'Decorrenza Calcolo' deve essere uguale al {1:dd/MM/yyyy}", decorrenzaCalcoloFondoCompare.Value, decorrenzaCalcoloFondoCompare.Value);
                            return false;
                        }
                        if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaCalcoloFondoCompare.Value) && (decorrenzaCalcolo.Value != datiPensione.DecorrenzaOriginaria.Value))
                        {
                            messaggioVideo = string.Format("Per 'Decorrenza Pensione' fino al {0:dd/MM/yyyy}, la 'Decorrenza Calcolo' deve essere uguale alla 'Decorrenza Pensione' ({1:dd/MM/yyyy})", decorrenzaCalcoloFondoCompare.Value, datiPensione.DecorrenzaOriginaria.Value);
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        #endregion Dati Assicurativi

        #region Dati PrecedentePensione
        private static void StoreDatiPrecedentePensionePerIstruttoria(long idPensione, Entity.DatiPrecedentePensione datiPrecedentePensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiPrecedentePensione.IsIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }
            Utility.ValorizzaOggetti(datiPrecedentePensione, datiIstruttoria);
            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
            {
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
                datiIstruttoria = null;
            }
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
            return;
        }
        #endregion Dati PrecedentePensione

        #region Dati Legge 4/60

        private static void StoreDatiLegge460PerFondoDatiGenerici(long idPensione, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondo == null)
            {
                datiFondo = new GestioneFondo.DatiFondo();
            }

            if (datiFondo.Equals(new GestioneFondo.DatiFondo()))
            {
                GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
            }
        }

        private static void StoreDatiLegge460Private(long idPensione, long idFondo, Entity.DatiLegge460 datiLegge460, bool isDomandaConNuovaGestioneDatiFondoFSPT, ref GestioneFondo.DatiFondo datiFondo, ref List<GestioneFondo.DatiFondoPT> listaDatiFondoPT)
        {
            if (listaDatiFondoPT == null || listaDatiFondoPT.Count == 0)
            {
                listaDatiFondoPT = new List<GestioneFondo.DatiFondoPT>();
                GestioneFondo.DatiFondoPT datiFondoPT = new GestioneFondo.DatiFondoPT();
                listaDatiFondoPT.Add(datiFondoPT);
            }

            Utility.ValorizzaOggetti(datiLegge460, listaDatiFondoPT.First());
            listaDatiFondoPT.First().Ncertificato = !string.IsNullOrEmpty(datiLegge460.NCertificato) ? int.Parse(datiLegge460.NCertificato) : (int?)null;

            if (listaDatiFondoPT.First().Equals(new GestioneFondo.DatiFondoPT()))
            {
                GestioneFondo.EliminaFondoPT(idPensione);
                listaDatiFondoPT = null;
            }
            else
            {
                if (idFondo == 0)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                    datiFondo = datiFondoNew;
                }
                if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                {
                    GestioneFondo.SalvaFondoPT(idFondo, listaDatiFondoPT.First());
                }
                else
                {
                    if (listaDatiFondoPT.First().IdRecordFondo != null)
                    {
                        GestioneFondo.SalvaFondoPTRecordFondo(idFondo, listaDatiFondoPT.First().IdRecordFondo.GetValueOrDefault(), listaDatiFondoPT.First());
                    }
                }
            }
            return;
        }

        #endregion Dati Legge 4/60

        #region Dati Istruttoria

        private static void StoreDatiIstruttoriaINPDAPPerPensioneFondoDatiGenerici(long idPensione, DatiIstruttoriaINPDAP datiIstruttoriaINPDAP, ref GestioneFondo.DatiFondo datiFondo)
        {
            if (datiFondo == null)
            {
                if (datiIstruttoriaINPDAP.IsDatiIstruttoriaPensioneFondoDatiGenericiNull())
                    return;
                else
                    datiFondo = new GestioneFondo.DatiFondo();
            }

            Utility.ValorizzaOggetti(datiIstruttoriaINPDAP, datiFondo);

            if (datiFondo.Equals(new GestioneFondo.DatiFondo()))
                GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            else
                GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
        }

        private static void StoreDatiIstruttoriaINPDAPPerIstruttoria(long idPensione, DatiIstruttoriaINPDAP datiIstruttoriaINPDAP, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria == null)
            {
                if (datiIstruttoriaINPDAP.IsDatiIstruttoriaIstruttoriaNull())
                    return;
                else
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            }

            Utility.ValorizzaOggetti(datiIstruttoriaINPDAP, datiIstruttoria);

            if (datiIstruttoria.Equals(new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
            else
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
        }

        #endregion Dati Istruttoria

        #endregion private members

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

