using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.ServiceReferences.DatiPensioni;
using INPS.DNA.Logging;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneDatiPensioni
    {
        #region public methods
        public static bool GetDatiTGP1ByChiavePensione(ref GestionePensione.DatiPensione datiPensione, string chiavePensione, ref GestioneEnpals.DatiEnpals datiENPALS, out string gp1af03, out string errori)
        {
            errori = string.Empty;
            gp1af03 = string.Empty;
            DatiTGP1Response risposta = null;
            string gp1af02 = string.Empty;

            try
            {
                DatiTGP1Request input = new DatiTGP1Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP1(datiPensione.NDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP1ToDB(risposta, ref datiPensione, ref datiENPALS, out gp1af03, out gp1af02);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP1ByChiavePensioneForLavorazioneManuale(GestionePensione.DatiPensione datiPensione, object prelievo, Utility.TipoAppartenenza? tipoAppartenenza, long numDomanda, short sedeChiavePensione, DateTime? DecorrenzaOriginaria, string codiceTipoRichiesta, List<GestioneLavorazioneManualeAutomatiche.TipologiaAutomaticaUnicarpe> elencoTipologiaAutomaticaUnicarpe, out bool isRicPlAutomaticaUnicarpe, out string errori)
        {
            errori = string.Empty;
            isRicPlAutomaticaUnicarpe = false;

            string sede = string.Empty;
            string categoria = datiPensione.GetCodCategoria().Substring(1, 3);
            string certificato = datiPensione.NCertificato.ToString().PadLeft(8, '0');
            string chiavePensione = string.Empty;

            try
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.AGO:
                        if (Utility.IsGestioneENPALSConSedeDestinazione(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
                        {
                            ServiceReferences.LiquidazioneAgo.AreaPrelievo prelievoAgo = prelievo as ServiceReferences.LiquidazioneAgo.AreaPrelievo;
                            if (prelievoAgo != null && prelievoAgo.Risposta != null && prelievoAgo.Risposta.DatiPensione != null && prelievoAgo.Risposta.DatiPensione.CodiceSedeDestinazione != null &&
                                prelievoAgo.Risposta.DatiPensione.CodiceSedeDestinazione > 0)
                                sede = prelievoAgo.Risposta.DatiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0');
                            else
                                sede = (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO)) ? sedeChiavePensione.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                        }
                        else
                            sede = (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO)) ? sedeChiavePensione.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                        break;
                    case Utility.TipoAppartenenza.FS:
                        ServiceReferences.LiquidazioneFs.AreaPrelievo prelievoFS = prelievo as ServiceReferences.LiquidazioneFs.AreaPrelievo;
                        if (prelievoFS != null && prelievoFS.Risposta != null && prelievoFS.Risposta.DatiPensione != null && prelievoFS.Risposta.DatiPensione.CodiceSedeDestinazione != null &&
                            prelievoFS.Risposta.DatiPensione.CodiceSedeDestinazione > 0)
                            sede = prelievoFS.Risposta.DatiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0');
                        else
                            sede = (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.FS)) ? sedeChiavePensione.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                        break;
                    case Utility.TipoAppartenenza.CI:
                        ServiceReferences.LiquidazioneCi.AreaPrelievo prelievoCI = prelievo as ServiceReferences.LiquidazioneCi.AreaPrelievo;
                        if (prelievoCI != null && prelievoCI.Risposta != null && prelievoCI.Risposta.DatiPensione != null && prelievoCI.Risposta.DatiPensione.CodiceSedeDestinazione != null &&
                            prelievoCI.Risposta.DatiPensione.CodiceSedeDestinazione > 0)
                            sede = prelievoCI.Risposta.DatiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0');
                        else
                            sede = (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.CI)) ? sedeChiavePensione.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                        break;
                }
                chiavePensione = categoria + sede + certificato;

                DatiTGP1Response risposta = null;
                DatiTGP1Request input = new DatiTGP1Request();
                input.ChiavePensione = chiavePensione;
                GetDatiTGP1(numDomanda.ToString(), input, out risposta, out errori);

                if (!String.IsNullOrEmpty(errori))
                    return false;
                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0)
                {
                    foreach (GestioneLavorazioneManualeAutomatiche.TipologiaAutomaticaUnicarpe lst in elencoTipologiaAutomaticaUnicarpe)
                    {
                        if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == lst.SiglaCategoria.Trim().ToUpperInvariant() &&
                            Utility.DataStrettamenteSuccessivaA((DateTime)DecorrenzaOriginaria, (DateTime)lst.DecorrenzaMinima) &&
                            risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == lst.Gruppo && x.GP1DPRD.Valore.Codice == lst.Prodotto &&
                      x.GP1DTIP.Valore.Codice == lst.Tipo && x.GP1DTIPOL.Valore.Codice.Trim().ToUpperInvariant() == lst.CodiceTipoRichiesta.Trim().ToUpperInvariant()).Count() > 0)
                        {
                            isRicPlAutomaticaUnicarpe = true;
                            break;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP1ByChiavePensione(long numDomanda, string chiavePensione, string codTipoRichiesta, out string gp1Freq1, out bool? isPLInvalidita, out bool? isPLPrepensionamentoEditoriaTipo0162, out bool? isPLESPAFiltroL26, out bool? isPLVESO33FiltroDAP,
            out bool? isPLOpzioneDonnaFiltroKWA, out bool? isPLOpzioneDonnaFiltroKXM, out bool? isPLOpzioneDonnaFiltroKYA, out bool? isPLOpzioneDonnaFiltroKZM, out bool? isPLOpzioneDonnaFiltroKUA, out bool? isPLOpzioneDonnaFiltroKVM, out bool? isPLAnticipateComputoSenzaFiltroPAV, out bool? isPLAnticipateComputoConFiltroPAV, out bool? isPLVecchiaiaComputo, out bool? isPlVecchiaiaOrdinario, out string gp1Af03, out string gp1Tipo, out string gp1Prodotto, out string gp1Gruppo, out bool isPLAnticipata0017, out bool? isPLLavoratoriFaticosiEPesanti,
            out bool? isVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE, out bool? isVOAUTAnticipataTipoContributivoFiltroGSE, out bool? isVOAUTVecchiaiaTipoContributivoFiltroGSE,
            out bool? isPLOIVecchiaiaInvaliditaFiltroC9A, out bool? isPLOISuperstitiFiltroC9A, out bool? isPLOIAnticipateFiltroC9A, out bool? isPLCOOP28FiltroDAP, bool isDomandaCOOP28, out bool? isRIcVopgiFiltroL80, out string errori)
        {
            errori = string.Empty;
            gp1Freq1 = string.Empty;
            gp1Tipo = string.Empty;
            gp1Prodotto = string.Empty;
            gp1Gruppo = string.Empty;
            gp1Af03 = string.Empty;
            isPLInvalidita = false;
            isPLPrepensionamentoEditoriaTipo0162 = false;
            isPLESPAFiltroL26 = false;
            isPLVESO33FiltroDAP = false;
            isPLOpzioneDonnaFiltroKWA = false;
            isPLOpzioneDonnaFiltroKXM = false;
            isPLOpzioneDonnaFiltroKYA = false;
            isPLOpzioneDonnaFiltroKZM = false;
            isPLOpzioneDonnaFiltroKUA = false;
            isPLOpzioneDonnaFiltroKVM = false;
            //ENG - Gestione RIC Anticipate Computo Senza Filtro PAV
            isPLAnticipateComputoSenzaFiltroPAV = false;
            //ENG - RIC/TRF Anticipate Computo Con Filtro PAV
            isPLAnticipateComputoConFiltroPAV = false;
            DatiTGP1Response risposta = null;
            //ENG - Gestione RIC Vecchiaia Computo
            isPLVecchiaiaComputo = false;
            isPlVecchiaiaOrdinario = false;

            isPLAnticipata0017 = false;
            //ENG - RIC Lavoratori Faticosi e Pesanti
            isPLLavoratoriFaticosiEPesanti = false;

            //ENG - Memo 116/2025
            isVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE = false;
            isVOAUTAnticipataTipoContributivoFiltroGSE = false;
            isVOAUTVecchiaiaTipoContributivoFiltroGSE = false;
            //ENG - Memo 97/2025
            isPLOIVecchiaiaInvaliditaFiltroC9A = false;
            isPLOISuperstitiFiltroC9A = false;
            isPLOIAnticipateFiltroC9A = false;
            //ENG - Memo 91/2026 
            isPLCOOP28FiltroDAP = false;
            //ENG - RIC VOPGI filtro L80
            isRIcVopgiFiltroL80 = false;

            try
            {
                DatiTGP1Request input = new DatiTGP1Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP1(numDomanda.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null)
                {
                    gp1Freq1 = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1FREQ1);
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0002" && x.GP1DPRD.Valore.Codice == "0011").Count() > 0)
                {
                    isPLInvalidita = true;
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIP.Valore.Codice == "0162" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "EAA").Count() > 0)
                {
                    isPLPrepensionamentoEditoriaTipo0162 = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "L26").Count() > 0)
                {
                    isPLESPAFiltroL26 = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "DAP").Count() > 0)
                {
                    //ENG - Memo 91/2026 
                    if (isDomandaCOOP28)
                        isPLCOOP28FiltroDAP = true;
                    else
                        isPLVESO33FiltroDAP = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                      risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIP.Valore.Codice == "0190" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "KWA").Count() > 0)
                {
                    isPLOpzioneDonnaFiltroKWA = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                     risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIP.Valore.Codice == "0190" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "KXM").Count() > 0)
                {
                    isPLOpzioneDonnaFiltroKXM = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                     risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIP.Valore.Codice == "0190" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "KYA").Count() > 0)
                {
                    isPLOpzioneDonnaFiltroKYA = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                     risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIP.Valore.Codice == "0190" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "KZM").Count() > 0)
                {
                    isPLOpzioneDonnaFiltroKZM = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                     risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIP.Valore.Codice == "0190" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "KUA").Count() > 0)
                {
                    isPLOpzioneDonnaFiltroKUA = true;
                }
                else if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                     risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIP.Valore.Codice == "0190" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "KVM").Count() > 0)
                {
                    isPLOpzioneDonnaFiltroKVM = true;
                }

                //ENG - Gestione RIC Anticipate Computo Senza Filtro PAV
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0001" &&
                        x.GP1DTIP.Valore.Codice == "0045" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() != "PAV").Count() > 0)
                {
                    isPLAnticipateComputoSenzaFiltroPAV = true;
                }

                //ENG - RIC/TRF Anticipate Computo Con Filtro PAV
                //ENG - TRF Computo con Filtro PAV: inserito flusso per il recupero del codice tipo richiesta da webdom in caso non corrisponde a quello della PL
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                  risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0001" &&
                      x.GP1DTIP.Valore.Codice == "0045" && ((x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "PAV") || (!string.IsNullOrEmpty(codTipoRichiesta) && codTipoRichiesta == "AV"))).Count() > 0)
                {
                    isPLAnticipateComputoConFiltroPAV = true;

                }

                //ENG - Gestione RIC Vecchiaia Computo
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                  risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0002" && x.GP1DTIP.Valore.Codice == "0045").Count() > 0)
                {
                    isPLVecchiaiaComputo = true;
                }

                //ENG - Gestione RIC Vecchiaia Ordinario
                if (risposta != null 
                    && risposta.ElementoDatiTGP1 != null 
                    && risposta.ElementoDatiTGP1.GP1T11 != null 
                    && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 
                    && risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0002" && x.GP1DTIP.Valore.Codice == "0001").Count() > 0
                    && risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "C9A").Count() == 0)
                {
                    isPlVecchiaiaOrdinario = true;
                }

                //ENG - memo 28
                if (risposta != null && risposta.ElementoDatiTGP1.GP1AF03 != null)
                {
                    gp1Af03 = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AF03);
                }
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => !string.IsNullOrEmpty(x.GP1DPRD.Valore.Codice) && GetValueDatoGP(x.GP1CMPNTIP) == "B").Count() > 0)
                    gp1Prodotto = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1T11.Where(x => !string.IsNullOrEmpty(x.GP1DPRD.Valore.Codice) && GetValueDatoGP(x.GP1CMPNTIP) == "B").First().GP1DPRD);

                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => !string.IsNullOrEmpty(x.GP1DTIP.Valore.Codice) && GetValueDatoGP(x.GP1CMPNTIP) == "B").Count() > 0)
                    gp1Tipo = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1T11.Where(x => !string.IsNullOrEmpty(x.GP1DTIP.Valore.Codice) && GetValueDatoGP(x.GP1CMPNTIP) == "B").First().GP1DTIP);

                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => !string.IsNullOrEmpty(x.GP1DGRP.Valore.Codice) && GetValueDatoGP(x.GP1CMPNTIP) == "B").Count() > 0)
                    gp1Gruppo = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1T11.Where(x => !string.IsNullOrEmpty(x.GP1DGRP.Valore.Codice) && GetValueDatoGP(x.GP1CMPNTIP) == "B").First().GP1DGRP);

                //ENG - 0001/0001/0017
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                  risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0001" && x.GP1DTIP.Valore.Codice == "0017").Count() > 0)
                {
                    isPLAnticipata0017 = true;
                }

                //ENG - RIC Lavoratori Faticosi e Pesanti
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                  risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0001" && x.GP1DTIP.Valore.Codice == "0140").Count() > 0)
                {
                    isPLLavoratoriFaticosiEPesanti = true;
                }

                //ENG - Memo 116/2025
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                   risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0001" &&
                       x.GP1DTIP.Valore.Codice == "0201" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "GSE").Count() > 0)
                {
                    isVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE = true;
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                   risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0001" &&
                       x.GP1DTIP.Valore.Codice == "0017" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "GSE").Count() > 0)
                {
                    isVOAUTAnticipataTipoContributivoFiltroGSE = true;
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                   risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DGRP.Valore.Codice == "0001" && x.GP1DPRD.Valore.Codice == "0002" &&
                       x.GP1DTIP.Valore.Codice == "0017" && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "GSE").Count() > 0)
                {
                    isVOAUTVecchiaiaTipoContributivoFiltroGSE = true;
                }
                //ENG - Memo 79 2025
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                   risposta.ElementoDatiTGP1.GP1T11.Where(x => Utility.IsDomanda_Vecchiaia_Invialidita(x.GP1DGRP.Valore.Codice, x.GP1DPRD.Valore.Codice, x.GP1DTIP.Valore.Codice)
                       && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "C9A").Count() > 0)
                {
                    isPLOIVecchiaiaInvaliditaFiltroC9A = true;
                }
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                   risposta.ElementoDatiTGP1.GP1T11.Where(x => Utility.IsDomanda_Superstiti(x.GP1DGRP.Valore.Codice, x.GP1DPRD.Valore.Codice, x.GP1DTIP.Valore.Codice)
                       && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "C9A").Count() > 0)
                {
                    isPLOISuperstitiFiltroC9A = true;
                }
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                   risposta.ElementoDatiTGP1.GP1T11.Where(x => Utility.IsDomanda_Anticipate(x.GP1DGRP.Valore.Codice, x.GP1DPRD.Valore.Codice, x.GP1DTIP.Valore.Codice)
                       && x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "C9A").Count() > 0)
                {
                    isPLOIAnticipateFiltroC9A = true;
                }

                //ENG - RIC VOPGI filtro L80
                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0 &&
                    risposta.ElementoDatiTGP1.GP1T11.Where(x => x.GP1DTIPOL.Valore.Codice != null && x.GP1DTIPOL.Valore.Codice.ToUpperInvariant() == "L80").Count() > 0)
                {
                    isRIcVopgiFiltroL80 = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP1ByChiavePensione_AOI_AUT(long numDomanda, string chiavePensione, out string gp1aj10, out string gp1af02, out string gp1af03, out string errori)
        {
            errori = string.Empty;
            gp1aj10 = string.Empty;
            gp1af02 = string.Empty;
            gp1af03 = string.Empty;
            DatiTGP1Response risposta = null;

            try
            {
                DatiTGP1Request input = new DatiTGP1Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP1(numDomanda.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null)
                {
                    gp1aj10 = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AJ10);
                    gp1af02 = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AF02);
                    gp1af03 = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AF03);
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP5ByChiavePensione(long numDomanda, string chiavePensione, out ElementoDatiTGP5[] listaDatiTGP5, out string errori)
        {
            errori = string.Empty;
            listaDatiTGP5 = null;

            DatiTGP5Response risposta = null;
            try
            {
                DatiTGP5Request request = new DatiTGP5Request();

                request.ChiavePensione = chiavePensione;

                GetDatiTGP5(numDomanda.ToString(), request, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }
                else if (risposta != null && risposta.ListaDatiTGP5 != null && risposta.ListaDatiTGP5.Count() > 0)
                {
                    listaDatiTGP5 = risposta.ListaDatiTGP5;
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }
        public static bool GetDatiTGP6ByChiavePensione(long numDomanda, string chiavePensione, out ElementoDatiTGP6[] listaDatiTGP6, out string errori)
        {
            errori = string.Empty;
            listaDatiTGP6 = null;

            DatiTGP6Response response = null;
            try
            {
                DatiTGP6Request requestDatiTGP6 = new DatiTGP6Request();

                requestDatiTGP6.ChiavePensione = chiavePensione;

                GetDatiTGP6(numDomanda.ToString(), requestDatiTGP6, out response, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (response != null && response.Esito != null && response.Esito.Risultato != "OK")
                {
                    errori = response.Esito.Descrizione;
                    return false;
                }

                else if (response != null && response.ListaDatiTGP6 != null && response.ListaDatiTGP6.Count() > 0)
                {
                    listaDatiTGP6 = response.ListaDatiTGP6;
                }
                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP2ByCodiceFascicolo(long nDomus, string codiceFascicolo, GestionePensione.DatiPensione datiPensione, out GestioneCalcolo.DatiCalcoloRetributivoENPAL calcoloRetributivoENPALS,
            out GestioneCalcolo.DatiCalcoloContributivoENPAL calcoloContributivoENPALS, out List<BLCommon.Entity.DatiSupplementiENPALS> listaSupplementi, ref GestioneEnpals.DatiEnpals datiENPALS,
            out List<BLCommon.Entity.DatiSuppRecordENPALS> listaSuppRecordENPALS, out string errori)
        {
            errori = string.Empty;
            DatiTGP2Response risposta = null;
            calcoloRetributivoENPALS = null;
            calcoloContributivoENPALS = null;
            listaSupplementi = null;
            listaSuppRecordENPALS = null;

            try
            {
                DatiTGP2Request input = new DatiTGP2Request();

                input.ChiavePensione = codiceFascicolo;

                GetDatiTGP2(nDomus.ToString(), input, Utility.MetodoServizio.GetDatiTGP2ByCodiceFascicolo, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP2ToDB(risposta, datiPensione, out calcoloRetributivoENPALS, out calcoloContributivoENPALS, out listaSupplementi, ref datiENPALS, out listaSuppRecordENPALS);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fascicolo: {0}", codiceFascicolo);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP2ByCodiceFascicolo(long nDomus, string codiceFascicolo, out List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordFondoINPDAP,
            out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP, out string errori)
        {
            errori = string.Empty;
            listaRecordFondoINPDAP = null;
            listaDatiServizioUtileINPDAP = null;

            try
            {
                DatiTGP2Request input = new DatiTGP2Request();

                input.ChiavePensione = codiceFascicolo;

                DatiTGP2Response risposta;
                GetDatiTGP2(nDomus.ToString(), input, Utility.MetodoServizio.GetDatiTGP2ByCodiceFascicolo, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP2ToDB(risposta, out listaRecordFondoINPDAP, out listaDatiServizioUtileINPDAP);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fascicolo: {0}", codiceFascicolo);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP2ByChiavePensione(string chiavePensione, ref GestionePensione.DatiPensione datiPensione, out string errori)
        {
            errori = string.Empty;
            DatiTGP2Response risposta = null;

            try
            {
                DatiTGP2Request input = new DatiTGP2Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP2(datiPensione.NDomus.ToString(), input, Utility.MetodoServizio.GetDatiTGP2ByChiavePensione, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                //NormalizzaDatiGP2ToDB
                DatiTGP2Response datiTGP2Response = risposta;
                if (datiTGP2Response != null && datiTGP2Response.ElementoTGP2 != null)
                {
                    //Recupero Data Inizio e Fine Assicurazione
                    if (datiTGP2Response.ElementoTGP2.GP2BM00 != null)
                    {
                        if (datiTGP2Response.ElementoTGP2.GP2BM00.GP2BM01Z != null)
                            datiPensione.InizioAssicurazione = Utility.DataFromString(GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP2BM00.GP2BM01Z), Utility.FormatoData.GGmmAAAA);
                        if (datiTGP2Response.ElementoTGP2.GP2BM00.GP2BM02Z != null)
                            datiPensione.FineAssicurazione = Utility.DataFromString(GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP2BM00.GP2BM02Z), Utility.FormatoData.AAAAmmGG);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP4ByCodiceFascicolo(long nDomus, string codiceFascicolo, string codiceFiscaleTitolare, Entity.ParametriARCA parametriARCA,
            out List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDiritto, out List<GestioneAnagrafica.DatiAnagrafici> listaAngraficaAventiDiritto, out string errori)
        {
            errori = string.Empty;
            DatiTGP4Response risposta = null;
            listaDatiAventiDiritto = null;
            listaAngraficaAventiDiritto = null;

            try
            {
                DatiTGP4Request input = new DatiTGP4Request();

                input.ChiaveFascicolo = codiceFascicolo;

                GetDatiTGP4(nDomus.ToString(), input, Utility.MetodoServizio.GetDatiTGP4ByCodiceFascicolo, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP4ToDB(parametriARCA, risposta, codiceFiscaleTitolare, nDomus.ToString(), out listaDatiAventiDiritto, out listaAngraficaAventiDiritto);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fascicolo: {0}", codiceFascicolo);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP7ByCodiceFascicolo(long nDomus, string codiceFascicolo, GestionePensione.DatiPensione datiPensione, bool isRiapertura, out List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditoSentenza495_93,
            out BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string errori)
        {
            errori = string.Empty;
            DatiTGP7Response risposta = null;
            listaDatiRedditoSentenza495_93 = null;
            datiDanteCausa = null;

            try
            {
                DatiTGP7Request input = new DatiTGP7Request();

                input.ChiavePensione = codiceFascicolo;

                GetDatiTGP7(nDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP7ToDB(risposta, datiPensione, isRiapertura, out listaDatiRedditoSentenza495_93, out datiDanteCausa);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fascicolo: {0}", codiceFascicolo);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetCodiceFascicoloByChiavePensione(long nDomusPerLogSoap, string chiavePensione, out short? categoriaFascicolo, out short? sedeFascicolo, out int? numeroFascicolo, out string errori)
        {
            categoriaFascicolo = null;
            sedeFascicolo = null;
            numeroFascicolo = null;
            errori = string.Empty;
            DatiTGP4Response risposta = null;

            try
            {
                DatiTGP4Request input = new DatiTGP4Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP4(nDomusPerLogSoap.ToString(), input, Utility.MetodoServizio.GetDatiTGP4ByChiavePensione, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                {
                    errori = "Errore tecnico durante il recupero dei dati della pensione: " + errori;
                    return false;
                }

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = "Errore tecnico durante il recupero dei dati della pensione: " + risposta.Esito.Descrizione;
                    return false;
                }

                if (risposta != null && risposta.ElementoDatiTGP4 != null)
                {
                    if (risposta.ElementoDatiTGP4.GP4DAA1 != null)
                        categoriaFascicolo = Utility.StringToNullableShort(GetValueDatoGP(risposta.ElementoDatiTGP4.GP4DAA1));
                    if (risposta.ElementoDatiTGP4.GP4DAA2 != null)
                    {
                        sedeFascicolo = Utility.StringToNullableShort(GetValueDatoGP(risposta.ElementoDatiTGP4.GP4DAA2).Substring(0, 4));
                        numeroFascicolo = Utility.StringToNullableInt(GetValueDatoGP(risposta.ElementoDatiTGP4.GP4DAA2).Substring(4));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(nDomusPerLogSoap, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool IsDomandaConPensioneLiquidata(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, out string errori)
        {
            errori = string.Empty;
            DatiTGP1Response risposta = null;
            short codCategoria = 0;

            try
            {
                DatiTGP1Request input = new DatiTGP1Request();
                string codCategoriaStr = datiPensione.GetCodCategoria();
                short.TryParse(codCategoriaStr, out codCategoria);

                input.ChiavePensione = codCategoria.ToString().PadLeft(3, '0') + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0') :
                    datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + datiPensione.NCertificato.ToString().PadLeft(8, '0');

                GetDatiTGP1(datiPensione.NDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    if (risposta.Esito.Codice == "-0019") // Chiave pensione non trovata
                        return false;

                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1T11 != null && risposta.ElementoDatiTGP1.GP1T11.Count() > 0)
                {
                    string gp1cmpntip = Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) ? "DA" : "B";
                    List<ElementoGP1T11> listaElementi = risposta.ElementoDatiTGP1.GP1T11.Where(x => GetValueDatoGP(x.GP1NDOMUS).Trim() == datiPensione.NDomus.ToString() &&
                        GetValueDatoGP(x.GP1FMPNTIP).Trim() == "1" && GetValueDatoGP(x.GP1CMPNTIP).Trim() == gp1cmpntip && GetValueDatoGP(x.GP1CPRD).Trim() == "IVSNEW").ToList();

                    if (listaElementi != null && listaElementi.Count > 0)
                    {
                        string matricola = string.Empty;
                        if (!listaElementi.Exists(x =>
                            {
                                matricola = GetValueDatoGP(x.GP1CDIP).Trim();
                                return matricola == datiPensione.MatricolaUtenteAcquisizione.Trim();
                            }))
                        {
                            errori = string.Format("La Matricola a cui è in carico la domanda ({0}) è diversa rispetto a quella che l'ha liquidata ({1}).", datiPensione.MatricolaUtenteAcquisizione.Trim(), matricola);
                            return false;
                        }

                        DateTime? dataLiquidazione = null;
                        if (datiPensione.DataTentativoCalcoloDefinitivo.HasValue && !listaElementi.Exists(x =>
                            {
                                dataLiquidazione = Utility.DataFromString(GetValueDatoGP(x.GP1DMPN), Utility.FormatoData.AAAAmmGG);
                                return dataLiquidazione == datiPensione.DataTentativoCalcoloDefinitivo.Value;
                            }))
                        {
                            errori = string.Format("La data di elaborazione ({0:dd/MM/yyyy}) risuta diversa rispetto alla data di liquidazione ({1:dd/MM/yyyy}).", datiPensione.DataTentativoCalcoloDefinitivo, dataLiquidazione);
                            return false;
                        }

                        if (listaElementi.FirstOrDefault(x => GetValueDatoGP(x.GP1CDIP).Trim() == datiPensione.MatricolaUtenteAcquisizione.Trim() &&
                            (!datiPensione.DataTentativoCalcoloDefinitivo.HasValue || Utility.DataFromString(GetValueDatoGP(x.GP1DMPN), Utility.FormatoData.AAAAmmGG) == datiPensione.DataTentativoCalcoloDefinitivo.Value)) != null)
                            return true;
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", codCategoria.ToString().PadLeft(3, '0') +
                    (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0')) +
                    datiPensione.NCertificato.ToString().PadLeft(8, '0'));
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }

            return false;
        }

        public static bool GetDatiTGP1ByChiavePensione(string numDomanda, string chiavePensione, out DatiTGP1Response risposta, out string errori)
        {
            risposta = null;
            try
            {
                DatiTGP1Request input = new DatiTGP1Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP1(numDomanda, input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }


        public static bool GetDatiECodiciVari(GestionePensione.DatiPensione datiPensione, string codiceFiscale, bool isRiapertura, out int countElementi, out string errori)
        {
            errori = string.Empty;
            DatiECodiciVariResponse risposta = null;
            countElementi = 0;
            try
            {
                DatiECodiciVariRequest input = new DatiECodiciVariRequest();

                input.CodiceFiscale = codiceFiscale;

                GetDatiECodiciVari(datiPensione.NDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK" && risposta.Esito.Codice != "-0027")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                if (risposta != null && risposta.ElementoDatiECodiciVari != null)
                {
                    var bloccaBititolareAutomatizzata = risposta.ElementoDatiECodiciVari.Where(x => x.Relazioni.Where(y => y.Codice == "T" || y.Codice == "C").Any() && x.DataEventoEliminazione == null).ToList();

                    if (isRiapertura && bloccaBititolareAutomatizzata != null && datiPensione.SiglaCategoria != null && datiPensione.NCertificato != null)
                    {
                        bloccaBititolareAutomatizzata = bloccaBititolareAutomatizzata.Except(bloccaBititolareAutomatizzata.Where(x => x.SiglaCategoria.Trim() == datiPensione.SiglaCategoria.Trim()
                        && x.CodiceSede == datiPensione.CodiceSede.ToString().Substring(0, 2) && x.CodiceZona == datiPensione.CodiceSede.ToString().Substring(2, 2) && x.NumeroCertificato == datiPensione.NCertificato.ToString()).ToList()).ToList();
                    }

                    Utility.TipoAutomazione? tipoAutomazione = null;
                    string messaggio = null;

                    Utility.IsDomandaDaAutomatizzare(datiPensione, isRiapertura, out tipoAutomazione, out messaggio);

                    if (datiPensione.TipoAutomazione == (byte)Utility.TipoAutomazione.Vecchiaia || tipoAutomazione == Utility.TipoAutomazione.Vecchiaia)
                        bloccaBititolareAutomatizzata = bloccaBititolareAutomatizzata.Except(bloccaBititolareAutomatizzata.Where(x => int.Parse(x.CodiceCategoria) >= 700 && int.Parse(x.CodiceCategoria) <= 799).ToList()).ToList();

                    countElementi = bloccaBititolareAutomatizzata != null ? bloccaBititolareAutomatizzata.Count() : 0;
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fiscale: {0}", codiceFiscale);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }


        //ENG - Spacchettate SOPGI
        public static bool GetDatiTGP1ByChiavePensione(ref GestionePensione.DatiPensione datiPensione, string chiavePensione, bool isRiapertura, out string gp1af03, out int gp1ag03M, out int gp1ad01m, out string errori)
        {
            errori = string.Empty;
            gp1af03 = string.Empty;
            gp1ag03M = 0;
            gp1ad01m = 0;
            DatiTGP1Response risposta = null;

            try
            {
                DatiTGP1Request input = new DatiTGP1Request();

                input.ChiavePensione = chiavePensione;

                GetDatiTGP1(datiPensione.NDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                if (risposta != null && risposta.ElementoDatiTGP1 != null)
                {
                    gp1af03 = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AF03);
                    if (datiPensione != null)
                        datiPensione.NaturaPensione = GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AF02);

                    //ENG - Spacchettate AGO
                    if (Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiapertura) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiapertura))
                    {
                        if (!String.IsNullOrEmpty(GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AG03Z)) && GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AG03Z).Trim().Length >= 6)
                        {
                            int.TryParse(GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AG03Z).Trim().Substring(4, 2), out gp1ag03M);
                        }

                        if (!String.IsNullOrEmpty(GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AD01Z)) && GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AD01Z).Trim().Length >= 6)
                        {
                            int.TryParse(GetValueDatoGP(risposta.ElementoDatiTGP1.GP1AD01Z).Trim().Substring(4, 2), out gp1ad01m);
                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Chiave pensione: {0}", chiavePensione);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        //ENG - Spacchettate SOPGI
        public static bool GetDatiTGP2ByCodiceFascicolo(long nDomus, string codiceFascicolo, GestionePensione.DatiPensione datiPensione, out List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivoSOPGI, out List<GestioneCalcolo.DatiCalcoloRetributivo> datiCalcoloRetributivoSOPGI, out List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> datiCalcoloContributivoQuotaFondoSOPGI, out List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> datiCalcoloRetributivoQuotaFondoSOPGI, out List<BLCommon.Entity.DatiSupplementi> datiSupplementi, out decimal? coefficienteTrasformazione, out string errori)
        {
            errori = string.Empty;
            DatiTGP2Response risposta = null;
            datiCalcoloContributivoSOPGI = null;
            datiCalcoloRetributivoSOPGI = null;
            datiCalcoloContributivoQuotaFondoSOPGI = null;
            datiCalcoloRetributivoQuotaFondoSOPGI = null;
            datiSupplementi = null;
            coefficienteTrasformazione = null;

            try
            {
                DatiTGP2Request input = new DatiTGP2Request();

                input.ChiavePensione = codiceFascicolo;

                GetDatiTGP2(nDomus.ToString(), input, Utility.MetodoServizio.GetDatiTGP2ByCodiceFascicolo, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP2ToDB(risposta, datiPensione, out datiCalcoloContributivoSOPGI, out datiCalcoloRetributivoSOPGI, out datiCalcoloContributivoQuotaFondoSOPGI, out datiCalcoloRetributivoQuotaFondoSOPGI, out datiSupplementi, out coefficienteTrasformazione);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fascicolo: {0}", codiceFascicolo);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiTGP8(long nDomus, string codiceFascicolo, out string errori, out string dataInizioTrattenuta, out string rataMensile)
        {
            errori = string.Empty;
            dataInizioTrattenuta = string.Empty;
            rataMensile = string.Empty;
            DatiTGP8Response risposta = null;

            try
            {
                DatiTGP8Request input = new DatiTGP8Request();

                input.ChiavePensione = codiceFascicolo;

                GetDatiTGP8(nDomus.ToString(), input, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP8ToDB(risposta, out dataInizioTrattenuta, out rataMensile);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fascicolo: {0}", codiceFascicolo);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }


        #endregion public methods

        #region private methods
        private static bool GetDatiTGP1(string numDomanda, DatiTGP1Request datiTGP1Request, out DatiTGP1Response datiTGP1Response, out string errori)
        {
            errori = string.Empty;
            datiTGP1Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP1Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP1, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP1Response = proxy.GetDatiTGP1(datiTGP1Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Chiave pensione: {0}", datiTGP1Request != null ? datiTGP1Request.ChiavePensione : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP1Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP1, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP2(string numDomanda, DatiTGP2Request datiTGP2Request, Utility.MetodoServizio metodoServizio, out DatiTGP2Response datiTGP2Response, out string errori)
        {
            errori = string.Empty;
            datiTGP2Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP2Request, Utility.Servizio.SrvDatiPensioni, metodoServizio, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP2Response = proxy.GetDatiTGP2(datiTGP2Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Chiave pensione / Codice fascicolo: {0}", datiTGP2Request != null ? datiTGP2Request.ChiavePensione : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP2Response, Utility.Servizio.SrvDatiPensioni, metodoServizio, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP4(string numDomanda, DatiTGP4Request datiTGP4Request, Utility.MetodoServizio metodoServizio, out DatiTGP4Response datiTGP4Response, out string errori)
        {
            errori = string.Empty;
            datiTGP4Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP4Request, Utility.Servizio.SrvDatiPensioni, metodoServizio, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP4Response = proxy.GetDatiTGP4(datiTGP4Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Chiave pensione: {0}; Codice fascicolo: {1}",
                            datiTGP4Request != null ? datiTGP4Request.ChiavePensione : null,
                            datiTGP4Request != null ? datiTGP4Request.ChiaveFascicolo : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP4Response, Utility.Servizio.SrvDatiPensioni, metodoServizio, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP7(string numDomanda, DatiTGP7Request datiTGP7Request, out DatiTGP7Response datiTGP7Response, out string errori)
        {
            errori = string.Empty;
            datiTGP7Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP7Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP7, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP7Response = proxy.GetDatiTGP7(datiTGP7Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Codice fascicolo: {0}", datiTGP7Request != null ? datiTGP7Request.ChiavePensione : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP7Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP7, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP8(string numDomanda, DatiTGP8Request datiTGP8Request, out DatiTGP8Response datiTGP8Response, out string errori)
        {
            errori = string.Empty;
            datiTGP8Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP8Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP8, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP8Response = proxy.GetDatiTGP8(datiTGP8Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Codice fascicolo: {0}", datiTGP8Request != null ? datiTGP8Request.ChiavePensione : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP8Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP8, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP5(string numDomanda, DatiTGP5Request datiTGP5Request, out DatiTGP5Response datiTGP5Response, out string errori)
        {
            errori = string.Empty;
            datiTGP5Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP5Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP5, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP5Response = proxy.GetDatiTGP5(datiTGP5Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Codice fascicolo: {0}", datiTGP5Request != null ? datiTGP5Request.ChiavePensione : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP5Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP5, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetDatiTGP6(string numDomanda, DatiTGP6Request datiTGP6Request, out DatiTGP6Response datiTGP6Response, out string errori)
        {
            errori = string.Empty;
            datiTGP6Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP6Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP6, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP6Response = proxy.GetDatiTGP6(datiTGP6Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Codice fascicolo: {0}", datiTGP6Request != null ? datiTGP6Request.ChiavePensione : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP6Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiTGP6, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool NormalizzaDatiGP1ToDB(DatiTGP1Response datiTGP1Response, ref GestionePensione.DatiPensione datiPensione, ref GestioneEnpals.DatiEnpals datiEnpals, out string gp1af03, out string gp1af02)
        {
            gp1af02 = string.Empty;
            gp1af03 = string.Empty;

            if (datiTGP1Response != null && datiTGP1Response.ElementoDatiTGP1 != null)
            {
                if (datiEnpals == null)
                    datiEnpals = new GestioneEnpals.DatiEnpals();

                datiEnpals.TipoLiquidazione = GetValueDatoGP(datiTGP1Response.ElementoDatiTGP1.GP1AZ11E);
                datiEnpals.TipoLiquidazioneProvvisoria = GetValueDatoGP(datiTGP1Response.ElementoDatiTGP1.GP1AZ11F);
                gp1af03 = GetValueDatoGP(datiTGP1Response.ElementoDatiTGP1.GP1AF03);
                if (datiPensione != null)
                    datiPensione.NaturaPensione = GetValueDatoGP(datiTGP1Response.ElementoDatiTGP1.GP1AF02);
            }

            return true;
        }

        private static void NormalizzaDatiGP2ToDB(DatiTGP2Response datiTGP2Response, GestionePensione.DatiPensione datiPensione, out GestioneCalcolo.DatiCalcoloRetributivoENPAL calcoloRetributivoENPALS,
            out GestioneCalcolo.DatiCalcoloContributivoENPAL calcoloContributivoENPALS, out List<BLCommon.Entity.DatiSupplementiENPALS> listaSupplementi,
            ref GestioneEnpals.DatiEnpals datiENPALS, out List<BLCommon.Entity.DatiSuppRecordENPALS> listaSuppRecordENPALS)
        {
            calcoloContributivoENPALS = null;
            calcoloRetributivoENPALS = null;
            listaSupplementi = null;
            listaSuppRecordENPALS = null;

            List<string> categorieENPALS = new List<string> { "0201", "0202", "0203", "0204", "0205", "0206", "0207", "0208", "0209", "0210", "0211", "0212" };

            if (datiTGP2Response != null && datiTGP2Response.ElementoTGP2 != null)
            {
                if (datiTGP2Response.ElementoTGP2.GP2BC00 != null && datiTGP2Response.ElementoTGP2.GP2BC00.Count() > 0)
                {
                    foreach (var retr in datiTGP2Response.ElementoTGP2.GP2BC00.ToList().FindAll(x => !(!string.IsNullOrEmpty(GetValueDatoGP(x.GP2BC09)) && GetValueDatoGP(x.GP2BC09).Length > 1 &&
                        new List<string> { "X", "Y", "W", "Z" }.Contains(GetValueDatoGP(x.GP2BC09).Substring(1, 1)))))
                    {
                        if (GetValueDatoGP(retr.GP2BC02) != "0" || GetValueDatoGP(retr.GP2BC03E) != "0" || !string.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC09)))
                        {
                            if (categorieENPALS.Contains(GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP1AB01).PadLeft(4, '0')))
                            {
                                char? quota = Utility.StringToNullableChar(GetValueDatoGP(retr.GP2BC0B));
                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString().Trim()))
                                {
                                    short mese = 0;
                                    if (GetValueDatoGP(retr.GP2BC01Z).Trim().Length >= 6)
                                    {
                                        short.TryParse(GetValueDatoGP(retr.GP2BC01Z).Substring(4, 2), out mese);
                                        GetQuotaByDecorrRetr(mese, out quota);
                                    }
                                }

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    if (calcoloRetributivoENPALS == null)
                                        calcoloRetributivoENPALS = new GestioneCalcolo.DatiCalcoloRetributivoENPAL();

                                    calcoloRetributivoENPALS.PeriodiQuotaA = Utility.StringToNullableShort(GetValueDatoGP(retr.GP2BC02));
                                    calcoloRetributivoENPALS.RMQuotaA = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E));
                                    if (GetValueDatoGP(retr.GP2BC10) != "0")
                                        calcoloRetributivoENPALS.GiorniQuotaA707 = Utility.StringToNullableShort(GetValueDatoGP(retr.GP2BC10));
                                    if (GetValueDatoGP(retr.GP2BC01Z) != "0")
                                    {
                                        DateTime? app = Utility.DataFromString(GetValueDatoGP(retr.GP2BC01Z) + "01", Utility.FormatoData.AAAAmmGG);
                                        if (app.HasValue)
                                            calcoloRetributivoENPALS.DecorrenzaQuotaA = app.Value.ToString("dd/MM/yyyy");
                                    }

                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    if (calcoloRetributivoENPALS == null)
                                        calcoloRetributivoENPALS = new GestioneCalcolo.DatiCalcoloRetributivoENPAL();

                                    calcoloRetributivoENPALS.PeriodiQuotaB = Utility.StringToNullableShort(GetValueDatoGP(retr.GP2BC02));
                                    calcoloRetributivoENPALS.RMQuotaB = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E));
                                    if (GetValueDatoGP(retr.GP2BC10) != "0")
                                        calcoloRetributivoENPALS.GiorniQuotaB707 = Utility.StringToNullableShort(GetValueDatoGP(retr.GP2BC10));
                                    if (GetValueDatoGP(retr.GP2BC01Z) != "0")
                                    {
                                        DateTime? app = Utility.DataFromString(GetValueDatoGP(retr.GP2BC01Z) + "01", Utility.FormatoData.AAAAmmGG);
                                        if (app.HasValue)
                                            calcoloRetributivoENPALS.DecorrenzaQuotaB = app.Value.ToString("dd/MM/yyyy");
                                    }
                                }
                            }
                        }
                    }
                }

                if (datiTGP2Response.ElementoTGP2.GP2BB00 != null && datiTGP2Response.ElementoTGP2.GP2BB00.Count() > 0)
                {
                    foreach (var contr in datiTGP2Response.ElementoTGP2.GP2BB00.ToList().FindAll(x => !(!string.IsNullOrEmpty(GetValueDatoGP(x.GP2BB05N)) && GetValueDatoGP(x.GP2BB05N).Length > 1 &&
                        new List<string> { "X", "Y", "W", "Z" }.Contains(GetValueDatoGP(x.GP2BB05N).Substring(1, 1)))))
                    {
                        if (GetValueDatoGP(contr.GP2BB06E) != "0" || GetValueDatoGP(contr.GP2BB07E) != "0" || GetValueDatoGP(contr.GP2BB08) != "0" || !string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)) || GetValueDatoGP(contr.GP2BB09E) != "0")
                        {
                            if (categorieENPALS.Contains(GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP1AB01).PadLeft(4, '0')))
                            {
                                if (GetValueDatoGP(contr.GP2BB05N) == "M0")
                                {
                                    if (datiENPALS == null)
                                        datiENPALS = new GestioneEnpals.DatiEnpals();
                                    datiENPALS.ImportoPensione = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E));
                                    if (GetValueDatoGP(contr.GP2BB04Z) != "0")
                                    {
                                        DateTime? app = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);
                                        if (app.HasValue)
                                            datiENPALS.DecorrenzaImportoPensione = app.Value.ToString("dd/MM/yyyy");
                                    }
                                }
                                else if (GetValueDatoGP(contr.GP2BB05N) == "M2")
                                {
                                    if (datiENPALS == null)
                                        datiENPALS = new GestioneEnpals.DatiEnpals();
                                    datiENPALS.ImportoPensione707 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E));
                                    if (GetValueDatoGP(contr.GP2BB04Z) != "0")
                                    {
                                        DateTime? app = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);
                                        if (app.HasValue)
                                            datiENPALS.DecorrenzaImportoPensione707 = app.Value.ToString("dd/MM/yyyy");
                                    }
                                }
                                else if (GetValueDatoGP(contr.GP2BB05N) == "M1" && !(datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0402" && datiPensione.Tipo == "0001"))
                                {
                                    if (listaSuppRecordENPALS == null)
                                        listaSuppRecordENPALS = new List<Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS>();

                                    Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS datiSuppRecordENPALS = new Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS();

                                    if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB04Z).Trim()) && GetValueDatoGP(contr.GP2BB04Z) != "0")
                                        datiSuppRecordENPALS.Decorrenza = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);
                                    if (GetValueDatoGP(contr.GP2BB09E) != "0")
                                        datiSuppRecordENPALS.Importo = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB09E));
                                    datiSuppRecordENPALS.IsFromGP = true;

                                    if (!listaSuppRecordENPALS.Exists(x => x.Decorrenza == datiSuppRecordENPALS.Decorrenza && x.Importo == datiSuppRecordENPALS.Importo))
                                        listaSuppRecordENPALS.Add(datiSuppRecordENPALS);
                                }
                                else if (GetValueDatoGP(contr.GP2BB05N) == "I1")
                                {
                                    if (datiENPALS == null)
                                        datiENPALS = new GestioneEnpals.DatiEnpals();
                                    datiENPALS.ImportoIIS = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB09E));
                                    if (contr.GP2BB04Z != null && GetValueDatoGP(contr.GP2BB04Z) != "0")
                                    {
                                        datiENPALS.DecorrenzaImportoIIS = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);
                                    }
                                }
                                else if (GetValueDatoGP(contr.GP2BB05N) != "S1")// i dati con gestione S1 vengono recuperati nel metodo ValorizzaSentenzaArt4 poichè non fanno parte dei dati calcolo contributivo
                                {
                                    if (!Utility.IsNullOrWhiteSpace(GetValueDatoGP(contr.GP2BB05N)) && GetValueDatoGP(contr.GP2BB05N).Trim() == "1" &&
                                        (string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB06E)) || GetValueDatoGP(contr.GP2BB06E) == "0") && (string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB07E)) || GetValueDatoGP(contr.GP2BB07E) == "0"))
                                        continue;

                                    if (calcoloContributivoENPALS == null)
                                        calcoloContributivoENPALS = new GestioneCalcolo.DatiCalcoloContributivoENPAL();

                                    calcoloContributivoENPALS.Montante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E));
                                    calcoloContributivoENPALS.ImportoContributivoTotale = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E));
                                    if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0B).Trim()))
                                        calcoloContributivoENPALS.Quota = Utility.StringToNullableChar(GetValueDatoGP(contr.GP2BB0B).Trim());
                                    else if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0A).Trim()))
                                    {
                                        if (GetValueDatoGP(contr.GP2BB0A) == "3")
                                            calcoloContributivoENPALS.Quota = 'C';
                                        else if (GetValueDatoGP(contr.GP2BB0A) == "4")
                                            calcoloContributivoENPALS.Quota = 'D';
                                    }
                                    else
                                        calcoloContributivoENPALS.Quota = 'C';

                                    if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB08)) && GetValueDatoGP(contr.GP2BB08) != "0")
                                        calcoloContributivoENPALS.NumeroContributiTotale = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));

                                    if (GetValueDatoGP(contr.GP2BB04Z) != "0")
                                    {
                                        DateTime? app = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);
                                        if (app.HasValue)
                                            calcoloContributivoENPALS.Decorrenza = app.Value.ToString("dd/MM/yyyy");
                                    }
                                }
                            }
                        }
                    }
                }

                if (datiTGP2Response.ElementoTGP2.GP2BE00 != null && datiTGP2Response.ElementoTGP2.GP2BE00.Count() > 0 && !(datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0402" && datiPensione.Tipo == "0001")) //ENG - Per le Ric Supplemento ENPALS P=0402 i supplementi vanno presi dal GETSAS
                {
                    foreach (var supp in datiTGP2Response.ElementoTGP2.GP2BE00)
                    {
                        if (categorieENPALS.Contains(GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP1AB01)))
                        {
                            Liquidazione.BLCommon.Entity.DatiSupplementiENPALS datiSuppEnpals = new Liquidazione.BLCommon.Entity.DatiSupplementiENPALS();
                            if (GetValueDatoGP(supp.GP2BE03E) != "0" || GetValueDatoGP(supp.GP2BE04E) != "0")
                                datiSuppEnpals.TipoSupplemento = 'C';
                            if (GetValueDatoGP(supp.GP2BE03E) != "0")
                                datiSuppEnpals.Montante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE03E));
                            if (GetValueDatoGP(supp.GP2BE04E) != "0")
                                datiSuppEnpals.ImportoContributivoTotale = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE04E));
                            if (!string.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE0B)) && !string.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE0B).Trim()))
                                datiSuppEnpals.Quota = Utility.StringToNullableChar(GetValueDatoGP(supp.GP2BE0B).Trim());
                            if (GetValueDatoGP(supp.GP2BE05E) != "0")
                            {
                                datiSuppEnpals.TipoSupplemento = 'R';
                                datiSuppEnpals.RM = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE05E));
                            }
                            if (GetValueDatoGP(supp.GP2BE06) != "0" && datiSuppEnpals.TipoSupplemento == 'R')
                                datiSuppEnpals.Periodi = Utility.StringToNullableShort(GetValueDatoGP(supp.GP2BE06));
                            if (!string.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE01Z)) && GetValueDatoGP(supp.GP2BE01Z) != "0")
                            {
                                datiSuppEnpals.Decorrenza = Utility.DataFromString(GetValueDatoGP(supp.GP2BE01Z) + "01", Utility.FormatoData.AAAAmmGG);
                                //TODO: Da verificare
                                //NOTA: a breve dovrebbe cambiare la logica sulla decorrenza che dovrebbe diventare una stringa
                                //if (!datiSuppEnpals.Decorrenza.HasValue)
                                //{
                                //    Data.CAREPET.Supplementi.T_GP2BE00 tempSupp = datiTGP2Response.ElementoTGP2.GP2BE00.FirstOrDefault(x => x.T_GP2BE01A == supp.T_GP2BE01A);
                                //    if (tempSupp != null)
                                //        datiSuppEnpals.Decorrenza = Utility.DataFromShort(tempSupp.T_GP2BE01A, tempSupp.T_GP2BE01M, 1);
                                //}
                            }
                            if (listaSuppRecordENPALS != null && listaSuppRecordENPALS.Count > 0)
                            {
                                Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS datiSuppRecordENPALS = null;
                                if (datiSuppEnpals.Quota == 'B')
                                    datiSuppRecordENPALS = listaSuppRecordENPALS.Find(x => x.Decorrenza.Value.Year.ToString() == GetValueDatoGP(supp.GP2BE01Z).Substring(0, 4));
                                else
                                    datiSuppRecordENPALS = listaSuppRecordENPALS.Find(x => x.Decorrenza == Utility.DataFromString(GetValueDatoGP(supp.GP2BE01Z) + "01", Utility.FormatoData.AAAAmmGG));

                                if (datiSuppRecordENPALS != null)
                                {
                                    // Manca il corrispondente campo
                                    //if (supp.GP2BE11RZA != 0 && supp.T_GP2BE11RZM != 0 && supp.T_GP2BE11RZG != 0)
                                    //    datiSuppRecordENPALS.InizioSupplemento = Utility.DataFromShort(supp.T_GP2BE11RZA, supp.T_GP2BE11RZM, supp.T_GP2BE11RZG);
                                    //if (datiSuppRecordENPALS.InizioSupplemento == DateTime.MinValue)
                                    //    datiSuppRecordENPALS.InizioSupplemento = null;
                                    //if (supp.GP2BE12RZA != 0 && supp.T_GP2BE12RZM != 0 && supp.T_GP2BE12RZG != 0)
                                    //    datiSuppRecordENPALS.FineSupplemento = Utility.DataFromShort(supp.T_GP2BE12RZA, supp.T_GP2BE12RZM, supp.T_GP2BE12RZG);
                                    //if (datiSuppRecordENPALS.FineSupplemento == DateTime.MinValue)
                                    //    datiSuppRecordENPALS.FineSupplemento = null;
                                }
                            }
                            listaSupplementi.Add(datiSuppEnpals);
                        }
                    }
                }
            }
        }

        private static void NormalizzaDatiGP2ToDB(DatiTGP2Response datiTGP2Response, out List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordFondoINPDAP,
            out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP)
        {
            listaRecordFondoINPDAP = new List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP>();
            listaDatiServizioUtileINPDAP = new List<GestioneDatiServizioUtileINPDAP.ServizioUtile>();

            if (datiTGP2Response != null && datiTGP2Response.ElementoTGP2 != null)
            {
                if (datiTGP2Response.ElementoTGP2.datiFondoSpeciale != null)
                {
                    if (datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NO20 != null && datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NO20.Length > 0)
                    {
                        int progressivo = 1;
                        foreach (var recordDatiFondoGP in datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NO20)
                        {
                            if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(recordDatiFondoGP.GP2NO30E)).GetValueOrDefault() != 0)
                            {
                                GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                                if (recordDatiFondoINPDAP == null)
                                {
                                    recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                    recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                    listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                                }

                                recordDatiFondoINPDAP.PensioneAnnuaLorda = Utility.StringToNullableDecimalPoint(GetValueDatoGP(recordDatiFondoGP.GP2NO30E));
                                recordDatiFondoINPDAP.PALConBenefici = Utility.StringToNullableDecimalPoint(GetValueDatoGP(recordDatiFondoGP.GP2NO30E));
                            }

                            if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(recordDatiFondoGP.GP2NO34E)).GetValueOrDefault() != 0)
                            {
                                GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "B1");
                                if (datiServizioUtile == null)
                                {
                                    datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                    datiServizioUtile.IdRecordFondo = progressivo;
                                    datiServizioUtile.Quota = "B1";
                                    listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                                }
                                datiServizioUtile.Retribuzione = Utility.StringToNullableDecimalPoint(GetValueDatoGP(recordDatiFondoGP.GP2NO34E));
                            }

                            if (Utility.StringToNullableBool(GetValueDatoGP(recordDatiFondoGP.GP2NO24)).GetValueOrDefault())
                            {
                                GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                                if (recordDatiFondoINPDAP == null)
                                {
                                    recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                    recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                    listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                                }
                                recordDatiFondoINPDAP.IndennitaIntegrativaSpecialeConglobata = Utility.StringToNullableBool(GetValueDatoGP(recordDatiFondoGP.GP2NO24));
                            }

                            if (Utility.StringToNullableBool(GetValueDatoGP(recordDatiFondoGP.GP2NO33)).GetValueOrDefault())
                            {
                                GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                                if (recordDatiFondoINPDAP == null)
                                {
                                    recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                    recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                    listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                                }
                                recordDatiFondoINPDAP.TrediciMensilita = Utility.StringToNullableBool(GetValueDatoGP(recordDatiFondoGP.GP2NO33));
                            }

                            progressivo++;
                        }
                    }

                    if (datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NE40 != null && datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NE40.Length > 0)
                    {
                        int progressivo = 1;
                        foreach (var recordDatiFondoGP in datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NE40)
                        {
                            if (Utility.StringToNullableShort(GetValueDatoGP(recordDatiFondoGP.GP2NE45E)).GetValueOrDefault() != 0)
                            {
                                GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                                if (recordDatiFondoINPDAP == null)
                                {
                                    recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                    recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                    listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                                }

                                recordDatiFondoINPDAP.ServizioUtileDirittoAA = Utility.StringToNullableShort(GetValueDatoGP(recordDatiFondoGP.GP2NE45E));
                            }

                            if (GetValueDatoGP(recordDatiFondoGP.GP2NE46) != "0")
                            {
                                GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "A");
                                if (datiServizioUtile == null)
                                {
                                    datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                    datiServizioUtile.IdRecordFondo = progressivo;
                                    datiServizioUtile.Quota = "A";
                                    listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                                }

                                int anni = 0;
                                byte mesi = 0;
                                byte giorni = 0;
                                SplitGiorni(GetValueDatoGP(recordDatiFondoGP.GP2NE46), out anni, out mesi, out giorni);

                                datiServizioUtile.ServizioUtileAA = (short)anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }

                            if (GetValueDatoGP(recordDatiFondoGP.GP2NE72) != "0")
                            {
                                GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "B1");
                                if (datiServizioUtile == null)
                                {
                                    datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                    datiServizioUtile.IdRecordFondo = progressivo;
                                    datiServizioUtile.Quota = "B1";
                                    listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                                }

                                int anni = 0;
                                byte mesi = 0;
                                byte giorni = 0;
                                SplitGiorni(GetValueDatoGP(recordDatiFondoGP.GP2NE72), out anni, out mesi, out giorni);

                                datiServizioUtile.ServizioUtileAA = (short)anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }

                            if (GetValueDatoGP(recordDatiFondoGP.GP2NE73E) != "0")
                            {
                                GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "B2");
                                if (datiServizioUtile == null)
                                {
                                    datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                    datiServizioUtile.IdRecordFondo = progressivo;
                                    datiServizioUtile.Quota = "B2";
                                    listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                                }

                                int anni = 0;
                                byte mesi = 0;
                                byte giorni = 0;
                                SplitGiorni(GetValueDatoGP(recordDatiFondoGP.GP2NE73E), out anni, out mesi, out giorni);

                                datiServizioUtile.ServizioUtileAA = (short)anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }

                            if (GetValueDatoGP(recordDatiFondoGP.GP2NE74E) != "0")
                            {
                                GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "B3");
                                if (datiServizioUtile == null)
                                {
                                    datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                    datiServizioUtile.IdRecordFondo = progressivo;
                                    datiServizioUtile.Quota = "B3";
                                    listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                                }

                                int anni = 0;
                                byte mesi = 0;
                                byte giorni = 0;
                                SplitGiorni(GetValueDatoGP(recordDatiFondoGP.GP2NE74E), out anni, out mesi, out giorni);

                                datiServizioUtile.ServizioUtileAA = (short)anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }

                            if (GetValueDatoGP(recordDatiFondoGP.GP2NE76) != "0")
                            {
                                GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "B4");
                                if (datiServizioUtile == null)
                                {
                                    datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                    datiServizioUtile.IdRecordFondo = progressivo;
                                    datiServizioUtile.Quota = "B4";
                                    listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                                }

                                int anni = 0;
                                byte mesi = 0;
                                byte giorni = 0;
                                SplitGiorni(GetValueDatoGP(recordDatiFondoGP.GP2NE76), out anni, out mesi, out giorni);

                                datiServizioUtile.ServizioUtileAA = (short)anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }

                            progressivo++;
                        }
                    }

                    if (datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NF10 != null && datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NF10.Length > 0)
                    {
                        int progressivo = 1;
                        foreach (var datiGP in datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NF10)
                        {
                            if (Utility.StringToNullableShort(GetValueDatoGP(datiGP.GP2NF19E)).GetValueOrDefault() != 0)
                            {
                                GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "A");
                                if (datiServizioUtile == null)
                                {
                                    datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                    datiServizioUtile.IdRecordFondo = progressivo;
                                    datiServizioUtile.Quota = "A";
                                    listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                                }

                                datiServizioUtile.Retribuzione = Utility.StringToNullableShort(GetValueDatoGP(datiGP.GP2NF19E));
                            }

                            if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(datiGP.GP2NF24E)).GetValueOrDefault() != 0)
                            {
                                GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                                if (recordDatiFondoINPDAP == null)
                                {
                                    recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                    recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                    listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                                }

                                recordDatiFondoINPDAP.RMSSenzaLegge33670QA = Utility.StringToNullableDecimalPoint(GetValueDatoGP(datiGP.GP2NF24E));
                            }

                            progressivo++;
                        }
                    }

                    if (datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NE00 != null && datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NE00.Length > 0)
                    {
                        int progressivo = 1;
                        foreach (var recordDatiFondoGP in datiTGP2Response.ElementoTGP2.datiFondoSpeciale.GP2NE00)
                        {
                            if (Utility.DataFromString(GetValueDatoGP(recordDatiFondoGP.GP2NE12Z), Utility.FormatoData.AAAAmmGG).GetValueOrDefault() != DateTime.MinValue)
                            {
                                GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                                if (recordDatiFondoINPDAP == null)
                                {
                                    recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                    recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                    listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                                }

                                recordDatiFondoINPDAP.DecorrenzaCalcolo = Utility.DataFromString(GetValueDatoGP(recordDatiFondoGP.GP2NE12Z), Utility.FormatoData.AAAAmmGG);
                            }
                        }
                    }
                }

                if (datiTGP2Response.ElementoTGP2.GP2BB00 != null && datiTGP2Response.ElementoTGP2.GP2BB00.Length > 0)
                {
                    int progressivo = 1;
                    foreach (var recordDatiFondoGP in datiTGP2Response.ElementoTGP2.GP2BB00)
                    {
                        if (Utility.StringToNullableByte(GetValueDatoGP(recordDatiFondoGP.GP2BH01E)).GetValueOrDefault() != 0)
                        {
                            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                            if (recordDatiFondoINPDAP == null)
                            {
                                recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                            }

                            recordDatiFondoINPDAP.Divisore = Utility.StringToNullableByte(GetValueDatoGP(recordDatiFondoGP.GP2BH01E));
                        }

                        progressivo++;
                    }
                }

                if (datiTGP2Response.ElementoTGP2.GP2BC00 != null && datiTGP2Response.ElementoTGP2.GP2BC00.Length > 0)
                {
                    int progressivo = 1;
                    foreach (var datiServizioUtileGP in datiTGP2Response.ElementoTGP2.GP2BC00)
                    {
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(datiServizioUtileGP.GP2BC0D)).GetValueOrDefault() != 0)
                        {
                            GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo && record.Quota == "A");
                            if (datiServizioUtile == null)
                            {
                                datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                                datiServizioUtile.IdRecordFondo = progressivo;
                                datiServizioUtile.Quota = "A";
                                listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                            }

                            datiServizioUtile.QuoteArt14 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(datiServizioUtileGP.GP2BC0D));
                        }

                        progressivo++;
                    }
                }

                if (datiTGP2Response.ElementoTGP2.GP2PB00 != null && datiTGP2Response.ElementoTGP2.GP2PB00.Length > 0)
                {
                    int progressivo = 1;
                    foreach (var datiGP in datiTGP2Response.ElementoTGP2.GP2PB00)
                    {
                        if (Utility.DataFromString(GetValueDatoGP(datiGP.GP2PBCES), Utility.FormatoData.AAAAmmGG).GetValueOrDefault() != DateTime.MinValue)
                        {
                            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordFondoINPDAP.FirstOrDefault(record => record.IdRecordFondo == progressivo);
                            if (recordDatiFondoINPDAP == null)
                            {
                                recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                                recordDatiFondoINPDAP.IdRecordFondo = progressivo;
                                listaRecordFondoINPDAP.Add(recordDatiFondoINPDAP);
                            }

                            recordDatiFondoINPDAP.ScadenzaBenefici = Utility.DataFromString(GetValueDatoGP(datiGP.GP2PBCES), Utility.FormatoData.AAAAmmGG);
                        }

                        progressivo++;
                    }
                }

                if (datiTGP2Response.ElementoTGP2.GP2PUB != null)
                {
                    if (!string.IsNullOrEmpty(GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP2PUB.GP2PUBCAP).Trim()))
                    {
                        listaRecordFondoINPDAP.ForEach(record =>
                        {
                            record.Capitolo = GetValueDatoGP(datiTGP2Response.ElementoTGP2.GP2PUB.GP2PUBCAP).Trim();
                        });
                    }
                }
            }
        }

        private static void NormalizzaDatiGP4ToDB(Entity.ParametriARCA parametriArca, DatiTGP4Response datiTGP4Response, string codiceFiscaleTitolare, string numDomanda,
            out List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDiritto, out List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaAventiDiritto)
        {
            listaDatiAventiDiritto = null;
            listaAnagraficaAventiDiritto = null;

            string codiceNucleoTitolare = string.Empty;

            if (datiTGP4Response != null && datiTGP4Response.ElementoDatiTGP4 != null && datiTGP4Response.ElementoDatiTGP4.GP4DB00 != null && datiTGP4Response.ElementoDatiTGP4.GP4DB00.Count() > 0)
            {
                listaDatiAventiDiritto = new List<GestioneAventiDiritto.AventeDirittoRecuperato>();
                listaAnagraficaAventiDiritto = new List<GestioneAnagrafica.DatiAnagrafici>();
                var listaAventiDiritto = datiTGP4Response.ElementoDatiTGP4.GP4DB00.ToList();

                GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                richiestaArca.Applicazione = parametriArca.Applicazione;
                richiestaArca.Matricola = parametriArca.Matricola;
                richiestaArca.Provenienza = parametriArca.Provenienza;
                richiestaArca.Ruolo = parametriArca.Ruolo;

                foreach (var aventeDirittoHost in listaAventiDiritto)
                {
                    GestioneAventiDiritto.AventeDirittoRecuperato aventeDiritto = new GestioneAventiDiritto.AventeDirittoRecuperato();
                    if (!string.IsNullOrEmpty(GetValueDatoGP(aventeDirittoHost.GP4DB13)) && GetValueDatoGP(aventeDirittoHost.GP4DB13) != "0")
                        aventeDiritto.CSog = Utility.StringToNullableInt(GetValueDatoGP(aventeDirittoHost.GP4DB13));
                    else
                        // Se non è presente il codice soggetto, significa che l'elemento è vuoto e quindi passo al prossimo
                        continue;

                    richiestaArca.CSog = aventeDiritto.CSog;
                    string errori = string.Empty;
                    Entity.Anagrafica anagrafica = null;
                    GestioneARCA.GetAnagraficaArcaByCodiceSoggetto(richiestaArca, numDomanda, out anagrafica, out errori);
                    if (!string.IsNullOrEmpty(errori))
                        throw new INPS.DNA.DnaValidationException(errori);
                    else if (anagrafica == null)
                        throw new INPS.DNA.DnaValidationException(string.Format("Errore nel recupero dell'anagrafica per il codice soggetto {0}", aventeDiritto.CSog));

                    GestioneAnagrafica.DatiAnagrafici datiAnagrafici = new GestioneAnagrafica.DatiAnagrafici();
                    Utility.ValorizzaOggetti(anagrafica, datiAnagrafici);
                    GestioneAnagrafica.SalvaAnagrafica(datiAnagrafici);
                    listaAnagraficaAventiDiritto.Add(datiAnagrafici);
                    aventeDiritto.CodiceFiscale = datiAnagrafici.CodiceFiscale;

                    if (!string.IsNullOrEmpty(GetValueDatoGP(aventeDirittoHost.GP4KA01)))
                        aventeDiritto.CategoriaPensione = GetValueDatoGP(aventeDirittoHost.GP4KA01);
                    if (!string.IsNullOrEmpty(GetValueDatoGP(aventeDirittoHost.GP4KA02)) && !string.IsNullOrEmpty(GetValueDatoGP(aventeDirittoHost.GP4KA03)))
                    {
                        short sede = 0;
                        short.TryParse(GetValueDatoGP(aventeDirittoHost.GP4KA02) + GetValueDatoGP(aventeDirittoHost.GP4KA03), out sede);
                        if (sede != 0)
                            aventeDiritto.SedePensione = sede;
                    }
                    if (!string.IsNullOrEmpty(GetValueDatoGP(aventeDirittoHost.GP4KA04)))
                    {
                        int certificato = 0;
                        int.TryParse(GetValueDatoGP(aventeDirittoHost.GP4KA04), out certificato);
                        if (certificato != 0)
                            aventeDiritto.CertificatoPensione = certificato;
                    }
                    if (!string.IsNullOrEmpty(GetValueDatoGP(aventeDirittoHost.GP4DB14Z)) && GetValueDatoGP(aventeDirittoHost.GP4DB14Z) != "0001-01-01")
                        aventeDiritto.DataMatrimonio = Utility.DataFromString(GetValueDatoGP(aventeDirittoHost.GP4DB14Z), Utility.FormatoData.AAAAmmGG);
                    if (!string.IsNullOrEmpty(GetValueDatoGP(aventeDirittoHost.GP4DB15)))
                    {
                        aventeDiritto.CodiceNucleoFromGP = GetValueDatoGP(aventeDirittoHost.GP4DB15);

                        if (aventeDiritto.CodiceFiscale == codiceFiscaleTitolare)
                            codiceNucleoTitolare = aventeDiritto.CodiceNucleoFromGP;
                    }

                    //TODO: Da dove va recuperata la scadenza revisione sanitaria?????
                    // Per il titolare recupero la scadenza revisione sanitaria
                    //if (aventeDiritto.CodiceFiscale == codiceFiscaleTitolare)
                    //    if (AreaPrelievo.Response != null && AreaPrelievo.Response.Familiari != null && AreaPrelievo.Response.Familiari.LISTT_GP3 != null && AreaPrelievo.Response.Familiari.LISTT_GP3.Count > 0)
                    //    {
                    //        Data.CAREPET.Familiari.T_GP3 familiare = AreaPrelievo.Response.Familiari.LISTT_GP3.Find(x => x.T_GP3CB08 == codiceFiscaleTitolare);
                    //        if (familiare != null)
                    //            aventeDiritto.ScadenzaRevisioneSanitaria = Utility.DataFromShort(familiare.T_GP3CK20A, familiare.T_GP3CK20M, 1);
                    //    }

                    // Per il titolare setto IsTitolare a true
                    if (aventeDiritto.CodiceFiscale == codiceFiscaleTitolare)
                        aventeDiritto.IsTitolare = true;

                    aventeDiritto.PresenzaGP = true;

                    if (aventeDirittoHost.GP4DC00 != null && aventeDirittoHost.GP4DC00.Count() > 0)
                    {
                        aventeDiritto.ListaPeriodi = new List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto>();

                        foreach (var periodoHost in aventeDirittoHost.GP4DC00.Select((value, index) => new { value, index }))
                        {
                            GestionePeriodiAventiDiritto.PeriodoAventiDiritto periodo = new GestionePeriodiAventiDiritto.PeriodoAventiDiritto();
                            periodo.IsFromGP = true;
                            if (!string.IsNullOrEmpty(GetValueDatoGP(periodoHost.value.GP4DC01)) && GetValueDatoGP(periodoHost.value.GP4DC01) != "0")
                                periodo.PercSpettante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(periodoHost.value.GP4DC01));
                            if (!string.IsNullOrEmpty(GetValueDatoGP(periodoHost.value.GP4DC02Z)) && GetValueDatoGP(periodoHost.value.GP4DC02Z) != "0")
                                periodo.DecorrenzaPeriodo = Utility.DataFromString(GetValueDatoGP(periodoHost.value.GP4DC02Z) + "01", Utility.FormatoData.AAAAmmGG);
                            if (!string.IsNullOrEmpty(GetValueDatoGP(periodoHost.value.GP4DC03Z)) && GetValueDatoGP(periodoHost.value.GP4DC03Z) != "0")
                                periodo.CessazionePeriodo = Utility.DataFromString(GetValueDatoGP(periodoHost.value.GP4DC03Z) + "01", Utility.FormatoData.AAAAmmGG);
                            if (!string.IsNullOrEmpty(GetValueDatoGP(periodoHost.value.GP4DC04)))
                            {
                                periodo.GradoParentela = Utility.StringToNullableChar(GetValueDatoGP(periodoHost.value.GP4DC04));

                                if (periodoHost.index == 0)
                                    //Aggiungo anche in decParentelaDa
                                    aventeDiritto.DecParentelaDA = Utility.StringToNullableChar(GetValueDatoGP(periodoHost.value.GP4DC04));

                                if (GetValueDatoGP(periodoHost.value.GP4DC04) == "CU")
                                {
                                    periodo.TipoUnione = "U";
                                    if (periodoHost.index == 0)
                                        aventeDiritto.TipoUnione = "U";
                                }
                                else if (periodo.GradoParentela == 'C')
                                {
                                    periodo.TipoUnione = "M";
                                    if (periodoHost.index == 0)
                                        aventeDiritto.TipoUnione = "M";
                                }
                            }
                            if (!string.IsNullOrEmpty(GetValueDatoGP(periodoHost.value.GP4DC05)) && GetValueDatoGP(periodoHost.value.GP4DC05) != "0")
                                periodo.CoeffRiduzione = Utility.StringToNullableDecimalPoint(GetValueDatoGP(periodoHost.value.GP4DC05));
                            if (!string.IsNullOrEmpty(GetValueDatoGP(periodoHost.value.GP4DC07)))
                            {
                                periodo.PercGiudice = Utility.StringToNullableDecimalPoint(GetValueDatoGP(periodoHost.value.GP4DC07));
                                if (periodo.PercGiudice == 0M)
                                    periodo.PercGiudice = null;
                            }

                            aventeDiritto.ListaPeriodi.Add(periodo);
                        }
                    }

                    listaDatiAventiDiritto.Add(aventeDiritto);
                }

                if (listaDatiAventiDiritto != null && listaDatiAventiDiritto.Count > 0)
                {
                    listaDatiAventiDiritto.FindAll(x => x.CodiceNucleoFromGP == codiceNucleoTitolare).ForEach(x => x.NucleoTitolare = true);
                    listaDatiAventiDiritto.FindAll(x => x.CodiceNucleoFromGP != codiceNucleoTitolare).ForEach(x => x.NucleoTitolare = false);
                }
                else
                    listaDatiAventiDiritto = null;
            }
        }


        private static void NormalizzaDatiGP7ToDB(DatiTGP7Response datiTGP7Response, GestionePensione.DatiPensione datiPensione, bool isRiapertura, out List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditoSentenza495_93,
            out BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            listaDatiRedditoSentenza495_93 = null;
            datiDanteCausa = null;

            if (datiTGP7Response != null && datiTGP7Response.ElementoDatiTGP7 != null)
            {
                if (datiTGP7Response.ElementoDatiTGP7.GP7LKE != null && datiTGP7Response.ElementoDatiTGP7.GP7LKE.Count() > 0)
                {
                    listaDatiRedditoSentenza495_93 = new List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93>();
                    var listaRedditoSentenza495_93Host = datiTGP7Response.ElementoDatiTGP7.GP7LKE.ToList();

                    foreach (GP7LKEType redditoSentenza495_93Host in listaRedditoSentenza495_93Host)
                    {
                        BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93 redditoSentenza495_93 = new BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93();
                        if (!string.IsNullOrEmpty(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE1Z)) && short.Parse(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE1Z)) != 0)
                        {
                            redditoSentenza495_93.AnnoReddito = short.Parse(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE1Z));
                            if (short.Parse(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE1Z)) < 2009)
                            {
                                if (!string.IsNullOrEmpty(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE2E)))
                                    redditoSentenza495_93.RedditoTitolare = Utility.StringToNullableDecimalPoint(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE2E));

                                if (!string.IsNullOrEmpty(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE3E)))
                                    redditoSentenza495_93.RedditoConiuge = Utility.StringToNullableDecimalPoint(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE3E));
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE2D)))
                                    redditoSentenza495_93.RedditoTitolare = Utility.StringToNullableDecimalPoint(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE2D));

                                if (!string.IsNullOrEmpty(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE2P)))
                                    redditoSentenza495_93.RedditoDaPensioneDC = Utility.StringToNullableDecimalPoint(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE2P));

                                if (!string.IsNullOrEmpty(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE3D)))
                                    redditoSentenza495_93.RedditoConiuge = Utility.StringToNullableDecimalPoint(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE3D));

                                if (!string.IsNullOrEmpty(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE3P)))
                                    redditoSentenza495_93.RedditoDaPensioneConiuge = Utility.StringToNullableDecimalPoint(GetValueDatoGP(redditoSentenza495_93Host.GP7LKE3P));
                            }

                            if (redditoSentenza495_93.RedditoConiuge.HasValue || redditoSentenza495_93.RedditoDaPensioneConiuge.HasValue || redditoSentenza495_93.RedditoDaPensioneDC.HasValue || redditoSentenza495_93.RedditoTitolare.HasValue)
                                listaDatiRedditoSentenza495_93.Add(redditoSentenza495_93);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LB01)) || !string.IsNullOrEmpty(GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LB02)) ||
                    !string.IsNullOrEmpty(GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LB03)) || !string.IsNullOrEmpty(GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LC02Z)))
                {
                    datiDanteCausa = new BLCommon.GestioneDanteCausa.DatiDanteCausa();
                    datiDanteCausa.SiglaCategoria = GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LB01);
                    datiDanteCausa.Sede = GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LB02);
                    datiDanteCausa.Certificato = Utility.StringToNullableInt(GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LB03));
                    datiDanteCausa.DecorrenzaPensione = Utility.DataFromString(GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LC02Z) + "01", Utility.FormatoData.AAAAmmGG);
                }

                //ENG - Spacchettate AGO
                if (Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiapertura) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiapertura))
                {
                    if (!string.IsNullOrEmpty(GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LC04)))
                        datiDanteCausa.ProvenienzaPensione = Utility.StringToNullableByte((GetValueDatoGP(datiTGP7Response.ElementoDatiTGP7.GP7LC04)));
                }

            }
        }

        private static void NormalizzaDatiGP8ToDB(DatiTGP8Response datiTGP8Response, out string dataInizioTrattenuta, out string rataMensile)
        {
            dataInizioTrattenuta = string.Empty;
            rataMensile = string.Empty;
            if (datiTGP8Response != null && datiTGP8Response.ListaElementoDatiTGP8 != null && datiTGP8Response.ListaElementoDatiTGP8.Count() > 0)
            {
                if (datiTGP8Response.ListaElementoDatiTGP8.First() != null && datiTGP8Response.ListaElementoDatiTGP8.First().GP8MC00 != null && datiTGP8Response.ListaElementoDatiTGP8.First().GP8MC00.GP8MC03Z != null)
                {
                    var GP8MC03Z = datiTGP8Response.ListaElementoDatiTGP8.First().GP8MC00.GP8MC03Z;
                    dataInizioTrattenuta = GP8MC03Z.Valore.Codice;
                }
                if (datiTGP8Response.ListaElementoDatiTGP8.First() != null && datiTGP8Response.ListaElementoDatiTGP8.First().GP8MD00 != null && datiTGP8Response.ListaElementoDatiTGP8.First().GP8MD00.First() != null && datiTGP8Response.ListaElementoDatiTGP8.First().GP8MD00.First().GP8MD13E != null)
                {
                    var GP8MD13E = datiTGP8Response.ListaElementoDatiTGP8.First().GP8MD00.First().GP8MD13E;
                    rataMensile = GP8MD13E.Valore.Codice;
                }
            }
        }

        private static void GetQuotaByDecorrRetr(short meseDecRetr, out char? quota)
        {
            quota = null;

            if (meseDecRetr == 61 || meseDecRetr == 62 || meseDecRetr == 63 || meseDecRetr == 64 || meseDecRetr == 16 ||
                meseDecRetr == 21 || meseDecRetr == 31 || meseDecRetr == 41 || meseDecRetr == 51 || meseDecRetr == 91 ||
                meseDecRetr == 92 || meseDecRetr == 93 || meseDecRetr == 94)
                quota = 'B';
            else
                quota = 'A';
        }

        public static string GetValueDatoGP(DatoGP datoGP)
        {
            if (datoGP != null && datoGP.Valore != null && !string.IsNullOrEmpty(datoGP.Valore.Codice))
                return datoGP.Valore.Codice;

            return string.Empty;
        }

        private static void SplitGiorni(string totaleGiorniStr, out int anni, out byte mesi, out byte giorni)
        {
            int? totaleGiorni = Utility.StringToNullableInt(totaleGiorniStr);

            anni = totaleGiorni.GetValueOrDefault() / 12;
            int app = totaleGiorni.GetValueOrDefault() % 12;
            mesi = (byte)(app / 30);
            giorni = (byte)(app % 30);
        }


        private static bool GetDatiECodiciVari(string numDomanda, DatiECodiciVariRequest datiTGP1Request, out DatiECodiciVariResponse datiTGP1Response, out string errori)
        {
            errori = string.Empty;
            datiTGP1Response = null;

            DatiPensioniClient proxy = new DatiPensioniClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(datiTGP1Request, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiECodiciVari, Utility.SOAPLogDirection.IN, numDomanda, guid);
                    datiTGP1Response = proxy.GetDatiECodiciVari(datiTGP1Request);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio DatiPensioni | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio DatiPensioni: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori))
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei dati della pensione";
                        string parametri = string.Format("Codice fiscale: {0}", datiTGP1Request != null ? datiTGP1Request.CodiceFiscale : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTGP1Response, Utility.Servizio.SrvDatiPensioni, Utility.MetodoServizio.GetDatiECodiciVari, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        //ENG - Spacchettate SOPGI
        private static void NormalizzaDatiGP2ToDB(DatiTGP2Response risposta, GestionePensione.DatiPensione datiPensione, out List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivoSOPGI, out List<GestioneCalcolo.DatiCalcoloRetributivo> datiCalcoloRetributivoSOPGI, out List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> datiCalcoloContributivoQuotaFondoSOPGI, out List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> datiCalcoloRetributivoQuotaFondoSOPGI, out List<BLCommon.Entity.DatiSupplementi> datiSupplementi, out decimal? coefficienteTrasformazione)
        {
            datiCalcoloContributivoSOPGI = null;
            datiCalcoloRetributivoSOPGI = null;
            datiCalcoloContributivoQuotaFondoSOPGI = null;
            datiCalcoloRetributivoQuotaFondoSOPGI = null;
            datiSupplementi = null;
            coefficienteTrasformazione = null;

            if (risposta != null && risposta.ElementoTGP2 != null)
            {
                List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGI = null;
                GestioneDecodifica.GetCodeGestioneQuotaFondoINPGI(out elencoCodeGestioneQuotaFondoINPGI);

                List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);

                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);

                if (risposta.ElementoTGP2.GP2BB00 != null && risposta.ElementoTGP2.GP2BB00.Count() > 0)
                {
                    datiCalcoloContributivoQuotaFondoSOPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI>();
                    datiCalcoloContributivoSOPGI = new List<GestioneCalcolo.DatiCalcoloContributivo>();

                    foreach (var contr in risposta.ElementoTGP2.GP2BB00)
                    {
                        if ((!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB06E)) && Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)) != 0M)
                            || (!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB07E)) && Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)) != 0M)
                            || (!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB08)) && Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)) != 0)
                            || !string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)))
                        {
                            if (elencoCodeGestioneQuotaFondoINPGI.Exists(x => x.TraduzioneSuGP == GetValueDatoGP(contr.GP2BB05N) && x.TipoQuota == "C"))
                            {
                                GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI datiQuotaFondoContrINPGI = new GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI();

                                datiQuotaFondoContrINPGI.Settimane = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));
                                datiQuotaFondoContrINPGI.Montante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E));
                                datiQuotaFondoContrINPGI.Quota = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB0D));
                                datiQuotaFondoContrINPGI.CodiceGestione = elencoCodeGestioneQuotaFondoINPGI.Find(x => x.TraduzioneSuGP == GetValueDatoGP(contr.GP2BB05N) && x.TipoQuota == "C").Id;

                                datiCalcoloContributivoQuotaFondoSOPGI.Add(datiQuotaFondoContrINPGI);
                            }
                            else
                            {
                                GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();
                                if ((!(string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0B))) && GetValueDatoGP(contr.GP2BB0B) == "D") || (!(string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0A))) && GetValueDatoGP(contr.GP2BB0A) == "4"))
                                {
                                    datiContr.NSettimaneQuotaDL214 = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));
                                    datiContr.MontanteQuotaDL214 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E));
                                    datiContr.ImportoContribTotaleQuotaDL214 = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E));
                                }
                                else if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)) && GetValueDatoGP(contr.GP2BB05N) == "K")
                                {
                                    if (Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)).GetValueOrDefault() > 0)
                                        datiContr.NSettimane = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));
                                    if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)).GetValueOrDefault() > 0)
                                        datiContr.Montante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E));
                                    if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)).GetValueOrDefault() > 0)
                                        datiContr.ImportoContributivoTotale = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E));
                                }
                                else
                                {
                                    datiContr.NSettimane = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));
                                    datiContr.Montante = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E));
                                    datiContr.ImportoContributivoTotale = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E));
                                }
                                if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)))
                                {
                                    if (elencoCodeGestioneCalcoloContributivo != null && elencoCodeGestioneCalcoloContributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim() == GetValueDatoGP(contr.GP2BB05N).Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloContributivo != null)
                                            datiContr.CodiceGestione = codeGestioneCalcoloContributivo.Id;
                                    }
                                }

                                datiContr.PL_Quotac = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB0D));

                                if (!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB04Z)))
                                {
                                    DateTime? app = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);
                                    if (app.HasValue)
                                        datiContr.DecorrenzaCalcoloContibutivo = app;
                                }

                                datiCalcoloContributivoSOPGI.Add(datiContr);
                            }
                        }
                    }

                    var contrTemp = risposta.ElementoTGP2.GP2BB00.FirstOrDefault();
                    if (contrTemp != null)
                    {
                        if ((!String.IsNullOrEmpty(GetValueDatoGP(contrTemp.GP2BB10)) && Utility.StringToNullableDecimalPoint(GetValueDatoGP(contrTemp.GP2BB10)) != 0M))
                        {
                            if (!coefficienteTrasformazione.HasValue)
                                coefficienteTrasformazione = Utility.StringToNullableDecimalPoint(GetValueDatoGP(contrTemp.GP2BB10));
                        }
                    }
                }

                if (risposta.ElementoTGP2.GP2BC00 != null && risposta.ElementoTGP2.GP2BC00.Count() > 0)
                {
                    datiCalcoloRetributivoQuotaFondoSOPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI>();
                    datiCalcoloRetributivoSOPGI = new List<GestioneCalcolo.DatiCalcoloRetributivo>();

                    int meseDecorrenza = 0;
                    foreach (var gp2Temp in risposta.ElementoTGP2.GP2BC00)
                    {
                        if (!String.IsNullOrEmpty(GetValueDatoGP(gp2Temp.GP2BC01Z)) && GetValueDatoGP(gp2Temp.GP2BC01Z).Trim().Length >= 6)
                        {
                            short meseTemp = 0;
                            short.TryParse(GetValueDatoGP(gp2Temp.GP2BC01Z).Trim().Substring(4, 2), out meseTemp);
                            if (meseTemp < 13)
                            {
                                meseDecorrenza = meseTemp;
                                break;
                            }
                        }
                    }

                    foreach (var retr in risposta.ElementoTGP2.GP2BC00)
                    {
                        if ((!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC02)) && Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02)) != 0) ||
                            (!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC03E)) && Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E)) != 0M)
                            || !string.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC09)))
                        {
                            if (elencoCodeGestioneQuotaFondoINPGI.Exists(x => x.TraduzioneSuGP == GetValueDatoGP(retr.GP2BC09) && x.TipoQuota == "R"))
                            {
                                GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI datiQuotaFondoRetrINPGI = new GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI();

                                datiQuotaFondoRetrINPGI.Settimane = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02));
                                datiQuotaFondoRetrINPGI.RetribuzioneMediaSettimanale = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E));
                                datiQuotaFondoRetrINPGI.ImportoCalcolato = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC0D));
                                datiQuotaFondoRetrINPGI.CodiceGestione = elencoCodeGestioneQuotaFondoINPGI.Find(x => x.TraduzioneSuGP == GetValueDatoGP(retr.GP2BC09) && x.TipoQuota == "R").Id;

                                datiCalcoloRetributivoQuotaFondoSOPGI.Add(datiQuotaFondoRetrINPGI);
                            }
                            else
                            {
                                GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                                char? quota = Utility.StringToNullableChar(GetValueDatoGP(retr.GP2BC0B));
                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString()))
                                {
                                    short mese = 0;
                                    if (GetValueDatoGP(retr.GP2BC01Z).Trim().Length >= 6)
                                    {
                                        short.TryParse(GetValueDatoGP(retr.GP2BC01Z).Substring(4, 2), out mese);
                                        GetQuotaByDecorrRetr(mese, out quota);
                                    }
                                }
                                datiRetr.QuotePrimeLiquidate = quota;

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    datiRetr.NSettimaneQuotaA = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02));
                                    datiRetr.RMSQuotaA = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E));
                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    datiRetr.NSettimaneQuotaB = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02));
                                    datiRetr.RMSQuotaB = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E));
                                }

                                if (!string.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC09)))
                                {
                                    if (retr.GP2BC09 != null && retr.GP2BC09.Valore != null && retr.GP2BC09.Valore.Codice != null)
                                        retr.GP2BC09.Valore.Codice = GetValueDatoGP(retr.GP2BC09).Replace("0", " ");

                                    if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == GetValueDatoGP(retr.GP2BC09).Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloRetributivo != null)
                                            datiRetr.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                                    }
                                }

                                if (!datiRetr.CodiceGestione.HasValue)
                                {
                                    datiRetr.CodiceGestione = GetGestioneFromQuotaDecorrenza("0245", meseDecorrenza, quota, elencoCodeGestioneCalcoloRetributivo);
                                }

                                if (Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC10)) != 0)
                                    datiRetr.NSettimane707 = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC10));

                                if (!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC01Z)) && GetValueDatoGP(retr.GP2BC01Z).Trim().Length >= 6)
                                {
                                    short annoRetr = 0;
                                    short.TryParse(GetValueDatoGP(retr.GP2BC01Z).Trim().Substring(0, 4), out annoRetr);
                                    if (annoRetr != 0)
                                        datiRetr.DecorrenzaOriginariaPensione = Utility.DataFromInt(annoRetr, meseDecorrenza, 1);
                                }

                                datiRetr.PL_Quotar = Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC0D));

                                datiCalcoloRetributivoSOPGI.Add(datiRetr);

                            }
                        }
                    }
                }

                if (risposta.ElementoTGP2.GP2BE00 != null && risposta.ElementoTGP2.GP2BE00.Count() > 0)
                {
                    foreach (var supp in risposta.ElementoTGP2.GP2BE00)
                    {
                        INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupp = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE05E)).GetValueOrDefault() != 0M)
                        {
                            datiSupp.RMSSupplemento = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE05E));
                            datiSupp.TipoSupplemento = 'R';
                        }
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE03E)).GetValueOrDefault() != 0M)
                            datiSupp.MontanteSupplemento = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE03E));
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE04E)).GetValueOrDefault() != 0M)
                            datiSupp.AmmontareContributivo = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE04E));
                        if (datiSupp.MontanteSupplemento.HasValue && datiSupp.AmmontareContributivo.HasValue)
                        {
                            datiSupp.TipoSupplemento = 'C';
                            if (Utility.StringToNullableByte(GetValueDatoGP(supp.GP2BE07)).GetValueOrDefault() != 0)
                                datiSupp.CodiceLiquidazione = Utility.StringToNullableByte(GetValueDatoGP(supp.GP2BE07));
                        }

                        short mese = 0;
                        short anno = 0;
                        if (!String.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE01Z)) && GetValueDatoGP(supp.GP2BE01Z).Trim().Length >= 6)
                        {
                            short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(0, 4), out anno);
                            short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(4, 2), out mese);
                            datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(anno, mese, 1);
                        }

                        if (!datiSupp.DecorrenzaSupplemento.HasValue)
                        {
                            foreach (var supplem in risposta.ElementoTGP2.GP2BE00)
                            {
                                short meseTemp = 0;
                                short annoTemp = 0;
                                if (!String.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE01Z)) && GetValueDatoGP(supp.GP2BE01Z).Trim().Length >= 6)
                                {
                                    short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(0, 4), out annoTemp);
                                    short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(4, 2), out meseTemp);
                                    if (annoTemp == anno && meseTemp == mese && meseTemp < 13)
                                    {
                                        datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(annoTemp, meseTemp, 1);
                                        break;
                                    }
                                }
                            }

                            if (datiSupp.DecorrenzaSupplemento.HasValue && datiSupp.TipoSupplemento == 'R')
                                datiSupp.QuotaSupplemento = 'B';
                        }
                        else if (datiSupp.TipoSupplemento == 'R')
                        {
                            datiSupp.QuotaSupplemento = 'A';
                        }

                        if (!string.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE02N)))
                            datiSupp.CodGestioneSupplemento = GetValueDatoGP(supp.GP2BE02N);
                        if (Utility.StringToNullableInt(GetValueDatoGP(supp.GP2BE06)).GetValueOrDefault() != 0)
                            datiSupp.NSettimaneSupplemento = Utility.StringToNullableInt(GetValueDatoGP(supp.GP2BE06));
                        if (datiSupp.TipoSupplemento != null)
                            datiSupp.IsFromPrelievo = true;

                        datiSupplementi.Add(datiSupp);
                    }
                }

            }
        }

        //ENG - Spacchettate SOPGI        
        private static long? GetGestioneFromQuotaDecorrenza(string codiceCategoria, int mese, char? quota,
            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo)
        {
            long? codiceGestione = null;
            string gestioneApp = null;

            if (quota == 'A')
            {
                if (mese == 99)
                    gestioneApp = "7";
                else if (mese == 75)
                    gestioneApp = "H";
                else
                    gestioneApp = (mese - 70).ToString();
            }
            else
            {
                if (mese == 98)
                    gestioneApp = "7";
                else if (mese == 65)
                    gestioneApp = "H";
                else
                    gestioneApp = (mese - 60).ToString();
            }


            if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
            {
                GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == gestioneApp && !x.IsFondo);
                if (codeGestioneCalcoloRetributivo != null)
                    codiceGestione = codeGestioneCalcoloRetributivo.Id;
            }

            return codiceGestione;
        }

        //ENG - Spacchettate AGO        
        private static long? GetGestioneFromQuotaDecorrenzaSpacchettateAGO(string codiceCategoria, int mese, int anno, char? quota,
            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo, int gp1ag03m, int gp1ad01m, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausaGP7)
        {
            long? codiceGestione = null;
            string gestioneApp = null;
            short? meseDecorrenzaOpzione = null;
            short? meseDecorrenzaOriginaria = null;
            byte? provenienzaPensione = null;
            short? meseDecorrenzaPensioneDA = null;

            meseDecorrenzaOpzione = (short?)gp1ag03m;
            meseDecorrenzaOriginaria = (short?)gp1ad01m;
            provenienzaPensione = danteCausaGP7 != null ? danteCausaGP7.ProvenienzaPensione : null;
            meseDecorrenzaPensioneDA = (danteCausaGP7 != null && danteCausaGP7.DecorrenzaPensione.HasValue) ? (short?)danteCausaGP7.DecorrenzaPensione.Value.Month : null;


            if (codiceCategoria.Trim().ToUpperInvariant() == "SOCOM" || codiceCategoria.Trim().ToUpperInvariant() == "SR" || codiceCategoria.Trim().ToUpperInvariant() == "SOART")
            {
                if (quota == 'A')
                {
                    if (mese < 13)
                        gestioneApp = "S";
                    else
                        gestioneApp = (mese - 70).ToString();
                }
                else
                {
                    gestioneApp = (mese - 60).ToString();
                }
            }
            else if (codiceCategoria.Trim().ToUpperInvariant() == "SO")
            {
                if (quota == 'A')
                {
                    if (mese == 99)
                        gestioneApp = "7";
                    if (mese == meseDecorrenzaOpzione ||
                        (mese == meseDecorrenzaOriginaria && (!provenienzaPensione.HasValue || provenienzaPensione.Value == 0)) ||
                        (mese == meseDecorrenzaPensioneDA && (provenienzaPensione.HasValue && (provenienzaPensione.Value == 1 || provenienzaPensione.Value == 2))))
                        gestioneApp = "1";
                    else if (anno < 1996) //paracadute ante96 -> mettere 1 al posto di S in quanto non arrivano domande da bonus e le quote possono comunque riportare date diverse tra loro
                        gestioneApp = "1";
                    else
                        gestioneApp = "S";
                }
                else
                {
                    if (mese == 98)
                        gestioneApp = "7";
                    else
                        gestioneApp = (mese - 60).ToString();
                }
            }


            if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
            {
                GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == gestioneApp && !x.IsFondo);
                if (codeGestioneCalcoloRetributivo != null)
                    codiceGestione = codeGestioneCalcoloRetributivo.Id;
            }

            return codiceGestione;
        }
        #endregion private methods

        //ENG - Spacchettate AGO
        internal static bool GetDatiTGP2ByCodiceFascicolo(long numDomanda, string codiceFascicolo, GestionePensione.DatiPensione datiPensione, out List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivoSpacchettateAGO, out List<GestioneCalcolo.DatiCalcoloRetributivo> datiCalcoloRetributivoSpacchettateAGO, out List<BLCommon.Entity.DatiSupplementi> datiSupplementiSpacchettateAGO, int gp1ag03m,
            int gp1ad01m, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaGP7, out string errori)
        {
            errori = string.Empty;
            DatiTGP2Response risposta = null;
            datiCalcoloContributivoSpacchettateAGO = null;
            datiCalcoloRetributivoSpacchettateAGO = null;
            datiSupplementiSpacchettateAGO = null;

            try
            {
                DatiTGP2Request input = new DatiTGP2Request();

                input.ChiavePensione = codiceFascicolo;

                GetDatiTGP2(numDomanda.ToString(), input, Utility.MetodoServizio.GetDatiTGP2ByCodiceFascicolo, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.Esito != null && risposta.Esito.Risultato != "OK")
                {
                    errori = risposta.Esito.Descrizione;
                    return false;
                }

                NormalizzaDatiGP2ToDB(risposta, datiPensione, gp1ag03m, gp1ad01m, datiDanteCausaGP7, out datiCalcoloContributivoSpacchettateAGO, out datiCalcoloRetributivoSpacchettateAGO, out datiSupplementiSpacchettateAGO);

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                errori = "Errore tecnico durante il recupero dei dati della pensione";
                string parametri = string.Format("Codice fascicolo: {0}", codiceFascicolo);
                GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        //ENG - Spacchettate AGO
        private static void NormalizzaDatiGP2ToDB(DatiTGP2Response risposta, GestionePensione.DatiPensione datiPensione, int gp1ag03m, int gp1ad01m, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaGP7, out List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivoSpacchettateAGO, out List<GestioneCalcolo.DatiCalcoloRetributivo> datiCalcoloRetributivoSpacchettateAGO, out List<BLCommon.Entity.DatiSupplementi> datiSupplementiSpacchettateAGO)
        {
            datiCalcoloContributivoSpacchettateAGO = null;
            datiCalcoloRetributivoSpacchettateAGO = null;
            datiSupplementiSpacchettateAGO = null;

            if (risposta != null && risposta.ElementoTGP2 != null)
            {
                //dati contributivi
                if (risposta.ElementoTGP2.GP2BB00 != null && risposta.ElementoTGP2.GP2BB00.Count() > 0)
                {
                    datiCalcoloContributivoSpacchettateAGO = new List<GestioneCalcolo.DatiCalcoloContributivo>();

                    List<GP2BB00Type> listaDatiContributiviGP2 = risposta.ElementoTGP2.GP2BB00.ToList().FindAll(x => !(!string.IsNullOrEmpty(GetValueDatoGP(x.GP2BB05N)) && GetValueDatoGP(x.GP2BB05N).Length == 2 && new List<string> { "X", "Y", "W", "Z" }.Contains(GetValueDatoGP(x.GP2BB05N).Substring(1, 1))));

                    if (listaDatiContributiviGP2 != null && listaDatiContributiviGP2.Count() > 0)
                    {
                        foreach (var contr in listaDatiContributiviGP2)
                        {
                            if ((!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB06E)) && Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB06E)) != 0M)
                                || (!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB07E)) && Utility.StringToNullableDecimalPoint(GetValueDatoGP(contr.GP2BB07E)) != 0M)
                                || (!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB08)) && Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)) != 0)
                                || !string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)))
                            {
                                GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();

                                if ((!(string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0B))) && GetValueDatoGP(contr.GP2BB0B) == "D") || (!(string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB0A))) && GetValueDatoGP(contr.GP2BB0A) == "4"))
                                {
                                    datiContr.NSettimaneQuotaDL214 = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));
                                    datiContr.MontanteQuotaDL214 = Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB06E));
                                    datiContr.ImportoContribTotaleQuotaDL214 = Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB07E));
                                }
                                else if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)) && GetValueDatoGP(contr.GP2BB05N) == "K")
                                {
                                    if (Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08)).GetValueOrDefault() > 0)
                                        datiContr.NSettimane = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));
                                    if (Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB06E)).GetValueOrDefault() > 0)
                                        datiContr.Montante = Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB06E));
                                    if (Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB07E)).GetValueOrDefault() > 0)
                                        datiContr.ImportoContributivoTotale = Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB07E));
                                }
                                else
                                {
                                    datiContr.NSettimane = Utility.StringToNullableInt(GetValueDatoGP(contr.GP2BB08));
                                    datiContr.Montante = Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB06E));
                                    datiContr.ImportoContributivoTotale = Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB07E));
                                }
                                if (!string.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB05N)))
                                {
                                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                                    GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);

                                    if (elencoCodeGestioneCalcoloContributivo != null && elencoCodeGestioneCalcoloContributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim() == GetValueDatoGP(contr.GP2BB05N).Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloContributivo != null)
                                            datiContr.CodiceGestione = codeGestioneCalcoloContributivo.Id;
                                    }
                                }

                                datiContr.PL_Quotac = Utility.StringToNullableDecimal(GetValueDatoGP(contr.GP2BB0D));

                                if (!String.IsNullOrEmpty(GetValueDatoGP(contr.GP2BB04Z)))
                                {
                                    DateTime? app = Utility.DataFromString(GetValueDatoGP(contr.GP2BB04Z) + "01", Utility.FormatoData.AAAAmmGG);
                                    if (app.HasValue)
                                        datiContr.DecorrenzaCalcoloContibutivo = app;
                                }

                                datiCalcoloContributivoSpacchettateAGO.Add(datiContr);
                            }

                        }
                    }
                }

                //dati retributivi
                if (risposta.ElementoTGP2.GP2BC00 != null && risposta.ElementoTGP2.GP2BC00.Count() > 0)
                {
                    datiCalcoloRetributivoSpacchettateAGO = new List<GestioneCalcolo.DatiCalcoloRetributivo>();

                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                    GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);

                    int meseDecorrenza = 0;

                    GP2BC00Type objMeseDecorrenza = risposta.ElementoTGP2.GP2BC00.ToList().Find(x => !String.IsNullOrEmpty(GetValueDatoGP(x.GP2BC01Z)) && GetValueDatoGP(x.GP2BC01Z).Trim().Length >= 6
                        && Utility.StringToNullableInt(GetValueDatoGP(x.GP2BC01Z).Trim().Substring(4, 2)) < 13);
                    if (objMeseDecorrenza != null && !String.IsNullOrEmpty(GetValueDatoGP(objMeseDecorrenza.GP2BC01Z)) && Utility.StringToNullableInt(GetValueDatoGP(objMeseDecorrenza.GP2BC01Z).Trim().Substring(4, 2)) != 0)
                    {
                        meseDecorrenza = Utility.StringToNullableInt(GetValueDatoGP(objMeseDecorrenza.GP2BC01Z).Trim().Substring(4, 2)).GetValueOrDefault();
                    }
                    else
                        meseDecorrenza = 1;

                    List<GP2BC00Type> listaDatiRetributiviGP2 = risposta.ElementoTGP2.GP2BC00.ToList().FindAll(x => !(!string.IsNullOrEmpty(GetValueDatoGP(x.GP2BC09)) && GetValueDatoGP(x.GP2BC09).Length > 1 && new List<string> { "X", "Y", "W", "Z" }.Contains(GetValueDatoGP(x.GP2BC09).Substring(1, 1))));

                    if (listaDatiRetributiviGP2 != null && listaDatiRetributiviGP2.Count() > 0)
                    {
                        foreach (var retr in listaDatiRetributiviGP2)
                        {
                            if ((!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC02)) && Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02)) != 0) ||
                                  (!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC03E)) && Utility.StringToNullableDecimalPoint(GetValueDatoGP(retr.GP2BC03E)) != 0M)
                                  || !string.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC09)))
                            {
                                GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                                char? quota = Utility.StringToNullableChar(GetValueDatoGP(retr.GP2BC0B));

                                int annoDecorrenza = 0;
                                if (!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC01Z)) && GetValueDatoGP(retr.GP2BC01Z).Trim().Length >= 6)
                                    annoDecorrenza = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC01Z).Trim().Substring(0, 4)).GetValueOrDefault();

                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString()))
                                {
                                    short mese = 0;
                                    if (!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC01Z)) && GetValueDatoGP(retr.GP2BC01Z).Trim().Length >= 6)
                                    {
                                        short.TryParse(GetValueDatoGP(retr.GP2BC01Z).Substring(4, 2), out mese);
                                        GetQuotaByDecorrRetr(mese, out quota);
                                    }
                                }

                                datiRetr.QuotePrimeLiquidate = quota;

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    datiRetr.NSettimaneQuotaA = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02));
                                    datiRetr.RMSQuotaA = Utility.StringToNullableDecimal(GetValueDatoGP(retr.GP2BC03E));
                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    datiRetr.NSettimaneQuotaB = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC02));
                                    datiRetr.RMSQuotaB = Utility.StringToNullableDecimal(GetValueDatoGP(retr.GP2BC03E));
                                }

                                if (!string.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC09)))
                                {

                                    string temp = GetValueDatoGP(retr.GP2BC09);
                                    string codiceGestione = temp.Replace("0", " ");

                                    if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == codiceGestione.Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloRetributivo != null)
                                            datiRetr.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                                    }
                                }

                                if (!datiRetr.CodiceGestione.HasValue)
                                {
                                    datiRetr.CodiceGestione = GetGestioneFromQuotaDecorrenzaSpacchettateAGO(datiPensione.SiglaCategoria, meseDecorrenza, annoDecorrenza, quota, elencoCodeGestioneCalcoloRetributivo, gp1ag03m, gp1ad01m, datiDanteCausaGP7);
                                }

                                if (Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC10)).GetValueOrDefault() != 0)
                                    datiRetr.NSettimane707 = Utility.StringToNullableInt(GetValueDatoGP(retr.GP2BC10));

                                if (!String.IsNullOrEmpty(GetValueDatoGP(retr.GP2BC01Z)) && GetValueDatoGP(retr.GP2BC01Z).Trim().Length >= 6)
                                {

                                    int annoTemp = 0;
                                    int meseTemp = 0;
                                    int.TryParse(GetValueDatoGP(retr.GP2BC01Z).Trim().Substring(4, 2), out meseTemp);
                                    int.TryParse(GetValueDatoGP(retr.GP2BC01Z).Trim().Substring(0, 4), out annoTemp);

                                    if (annoTemp < 1996 && meseTemp < 13)
                                        datiRetr.DecorrenzaOriginariaPensione = Utility.DataFromInt(annoTemp, meseTemp, 1);
                                    else
                                        datiRetr.DecorrenzaOriginariaPensione = Utility.DataFromInt(annoTemp, meseDecorrenza, 1);

                                    if (meseTemp == 88 || meseTemp == 90)
                                        datiRetr.DecorrenzaOriginariaPensione = datiRetr.DecorrenzaOriginariaPensione.Value.AddSeconds(meseTemp);
                                }

                                datiRetr.PL_Quotar = Utility.StringToNullableDecimal(GetValueDatoGP(retr.GP2BC0D));

                                datiCalcoloRetributivoSpacchettateAGO.Add(datiRetr);

                            }
                        }
                    }
                }

                //dati supplemento
                if (risposta.ElementoTGP2.GP2BE00 != null && risposta.ElementoTGP2.GP2BE00.Count() > 0)
                {
                    foreach (var supp in risposta.ElementoTGP2.GP2BE00)
                    {
                        INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupp = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE05E)).GetValueOrDefault() != 0M)
                        {
                            datiSupp.RMSSupplemento = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE05E));
                            datiSupp.TipoSupplemento = 'R';
                        }
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE03E)).GetValueOrDefault() != 0M)
                            datiSupp.MontanteSupplemento = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE03E));
                        if (Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE04E)).GetValueOrDefault() != 0M)
                            datiSupp.AmmontareContributivo = Utility.StringToNullableDecimalPoint(GetValueDatoGP(supp.GP2BE04E));
                        if (datiSupp.MontanteSupplemento.HasValue && datiSupp.AmmontareContributivo.HasValue)
                        {
                            datiSupp.TipoSupplemento = 'C';
                            if (Utility.StringToNullableByte(GetValueDatoGP(supp.GP2BE07)).GetValueOrDefault() != 0)
                                datiSupp.CodiceLiquidazione = Utility.StringToNullableByte(GetValueDatoGP(supp.GP2BE07));
                        }

                        short mese = 0;
                        short anno = 0;
                        if (!String.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE01Z)) && GetValueDatoGP(supp.GP2BE01Z).Trim().Length >= 6)
                        {
                            short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(0, 4), out anno);
                            short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(4, 2), out mese);
                            datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(anno, mese, 1);
                        }

                        if (!datiSupp.DecorrenzaSupplemento.HasValue)
                        {
                            foreach (var supplem in risposta.ElementoTGP2.GP2BE00)
                            {
                                short meseTemp = 0;
                                short annoTemp = 0;
                                if (!String.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE01Z)) && GetValueDatoGP(supp.GP2BE01Z).Trim().Length >= 6)
                                {
                                    short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(0, 4), out annoTemp);
                                    short.TryParse(GetValueDatoGP(supp.GP2BE01Z).Trim().Substring(4, 2), out meseTemp);
                                    if (annoTemp == anno && meseTemp == mese && meseTemp < 13)
                                    {
                                        datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(annoTemp, meseTemp, 1);
                                        break;
                                    }
                                }
                            }

                            if (datiSupp.DecorrenzaSupplemento.HasValue && datiSupp.TipoSupplemento == 'R')
                                datiSupp.QuotaSupplemento = 'B';
                        }
                        else if (datiSupp.TipoSupplemento == 'R')
                        {
                            datiSupp.QuotaSupplemento = 'A';
                        }

                        if (!string.IsNullOrEmpty(GetValueDatoGP(supp.GP2BE02N)))
                            datiSupp.CodGestioneSupplemento = GetValueDatoGP(supp.GP2BE02N);
                        if (Utility.StringToNullableInt(GetValueDatoGP(supp.GP2BE06)).GetValueOrDefault() != 0)
                            datiSupp.NSettimaneSupplemento = Utility.StringToNullableInt(GetValueDatoGP(supp.GP2BE06));
                        if (datiSupp.TipoSupplemento != null)
                            datiSupp.IsFromPrelievo = true;

                        datiSupplementiSpacchettateAGO.Add(datiSupp);
                    }
                }

            }
        }
    }
}
