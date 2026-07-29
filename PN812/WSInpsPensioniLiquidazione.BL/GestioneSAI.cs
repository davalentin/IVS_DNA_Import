using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.ServiceReferences.SAI;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.ServiceModel;
using System.Web.Services.Protocols;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneSAI
    {
        public static bool GetDatiSAI(long numeroDomanda, string codFisc, TipoRichiesta.GET tipoRic, ref SAI datiSAI, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GetDatiSAI_Private(numeroDomanda, tipoRic, ref datiSAI, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                datiSAI = null;
                return false;
            }
            if (datiSAI.GETSAI_ESITO != '0' && datiSAI.GETSAI_ESITO != '7')
                datiSAI = null;

            return true;
        }


        public static bool SbloccoSAI(long numeroDomanda, TipoRichiesta.SBL? tipoRic, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (tipoRic.HasValue)
                SbloccoSAI_Private(numeroDomanda, tipoRic, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
                return false;

            return true;
        }

        public static bool AggiornaSAI(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string statoPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;

            if (!ControllaStatoPensionePerAggiornamento(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento SAI";
                return false;
            }

            if (!AggiornaSAI(datiPensione, datiDanteCausa, GestioneSAI.GetTipoRichiestaPAG(datiPensione), out messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSAI);
                return false;
            }

            if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.IsCumuloAutomatica.GetValueOrDefault())
            {
                if (!GestioneTotalIvs.AggiornaCumulo(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTotal;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal);
                    messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento TOTAL (Cumulo). " + messaggioVideo;
                }
            }

            if (Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && datiPensione.IsTotAutomatica.GetValueOrDefault())
            {
                if (!GestioneTotalIvs.AggiornaTot(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTot;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTot);
                    messaggioVideo = "Aggiornamento WebDom riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento TOTAL (Totalizzazione). " + messaggioVideo;
                }
            }

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        public static bool AggiornaSAI(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, TipoRichiesta.PAG? tipoRic, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (tipoRic.HasValue)
                AggiornaSAI_Private(datiPensione, datiDanteCausa, tipoRic, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
                return false;
            return true;
        }

        public static TipoRichiesta.PAG? GetTipoRichiestaPAG(GestionePensione.DatiPensione datiPensione)
        {
            TipoRichiesta.PAG? tipoRic = TipoRichiesta.PAG.PAGSAI;
            if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                tipoRic = TipoRichiesta.PAG.PAGSAY;
            else if (Utility.IsRicostituzione_Supplemento(datiPensione))
                tipoRic = TipoRichiesta.PAG.PAGSAS;
            else if (Utility.IsRicostituzione_MotiviContributivi(datiPensione) ||
                Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                tipoRic = TipoRichiesta.PAG.PAGSAR;

            return tipoRic;
        }

        private static void GetDatiSAI_Private(long NumeroDomanda, TipoRichiesta.GET tipoRic, ref SAI datiSAI, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            PNBTW01Service proxy = new PNBTW01Service();
            ProgramInterface input = new ProgramInterface();
            ProgramInterface1 output = null;
            input.getsai = new ProgramInterfaceGetsai();
            input.getsai.nbw1_dati_input = new ProgramInterfaceGetsaiNbw1_dati_input();
            input.getsai.nbw1_dati_input.nbw1i_categoria = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_cod_fisc = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_dt_calcolo = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_nr_certif = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_num_dom_inps = NumeroDomanda.ToString();
            input.getsai.nbw1_dati_input.nbw1i_sede = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_tipo_liq = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_tipo_ric = tipoRic.ToString();

            Utility.MetodoServizio? metodoServizio = Utility.GetValueFromDescription<Utility.MetodoServizio>(tipoRic.ToString());

            using (new MethodExecutionTracer())
            {
                try
                {
                    ////Richiamare il metodo di prelievo delle informazioni dal SAI
                    ////proxy.GetDatiSAI(....)

                    ////richiamo metodo di valorizza del Mocks. A tendere andrà sostituito con il Srv SAI e implementato il metodo di mapping
                    //Mocks.MocksGestioneSAI.GetDatiSAI_Mocks(out datiSAI);

                    if (metodoServizio.HasValue)
                        GestioneLogSoap.SalvaLogSoap(input, Utility.Servizio.SrvSAI, metodoServizio.Value, Utility.SOAPLogDirection.IN, NumeroDomanda.ToString(), guid);

                    proxy.Url = GetUrl();
                    output = proxy.PNBTW01Operation(input);

                    if (metodoServizio.HasValue)
                        GestioneLogSoap.SalvaLogSoap(output, Utility.Servizio.SrvSAI, metodoServizio.Value, Utility.SOAPLogDirection.OUT, NumeroDomanda.ToString(), guid);
                }
                catch (SoapException exception)
                {
                    messaggioVideo = string.Format("{0} | {1}", Utility.GetMessageFromException(exception), exception.Detail != null ? exception.Detail.InnerText : string.Empty);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nella chiamata al servizio SAI: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore nel recupero dei dati dai sistemi ENPALS";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        GestioneLogGenerico.SalvaLogGenerico(NumeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                }
            }

            if (output != null && output.getsao.nbw1_dati_output.nbw1o_esito != "7" &&
                !string.IsNullOrEmpty(output.getsao.nbw1_dati_output.nbw1o_des_errore) && !string.IsNullOrEmpty(output.getsao.nbw1_dati_output.nbw1o_des_errore.Trim()))
            {
                if (output.getsao.nbw1_dati_output.nbw1o_esito == "8")
                {
                    messaggioVideo = "Domanda non trovata sui sistemi ENPALS.";
                    return;
                }
                else
                {
                    messaggioVideo = "Errore dai sistemi ENPALS: " + output.getsao.nbw1_dati_output.nbw1o_des_errore;
                    return;
                }
            }

            ValorizzaDatiFromExternalService(output, out datiSAI);
        }

        public static bool ControlsDatiSAI(GestionePensione.DatiPensione datiPensioneMaster, GestionePensione.DatiPensione datiPensioneDaPrelievo,
            GestioneCalcolo.DatiCalcoloContributivoENPAL datiCalcoloContributivo, GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivo, GestioneEnpals.DatiEnpals datiENPALSMaster,
            GestioneEnpals.DatiEnpals datiENPALSDaPrelievo, BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneENPALSLiq, char tipoCalcolo, string matricolaOperatore, short sedeOperatore, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool? isChiaveFascicoloGenerata, string codiceFiscaleDanteCausa, out bool isNuovoCertificatoGeneratoEnpals, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            isNuovoCertificatoGeneratoEnpals = false;

            if (!ControlsDatiPensione(datiPensioneMaster, datiPensioneDaPrelievo, datiDanteCausa, out messaggioVideo))
                return false;

            if (!ControlsDatiContributivi(out messaggioVideo))
                return false;

            if (!ControlsDatiRetributivi(datiCalcoloRetributivo, tipoCalcolo, datiContribuzioneENPALSLiq, out messaggioVideo))
                return false;

            if (!ControlsDatiENPALS(datiPensioneMaster, datiENPALSMaster, datiENPALSDaPrelievo, out messaggioVideo))
                return false;

            #region Aggiornamento Fondo WebDom
            if (!ControlsAggiornamentoFondoWebDom(datiPensioneMaster, matricolaOperatore, sedeOperatore, tipoCalcolo, isChiaveFascicoloGenerata, datiDanteCausa, codiceFiscaleDanteCausa, out isNuovoCertificatoGeneratoEnpals, out messaggioVideo))
            {
                messaggioVideo = "Incongruenza tra la Sigla Categoria e il Tipo Calcolo. " + messaggioVideo;
                return false;
            }
            #endregion Aggiornamento Fondo WebDom

            return true;
        }

        private static bool ControlsDatiRetributivi(GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivo, char tipoCalcolo, BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneENPALSLiq,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (tipoCalcolo == 'K' || tipoCalcolo == 'J')
            {
                if (datiCalcoloRetributivo == null)
                {
                    messaggioVideo = "Non è possibile lavorare la domanda. Dati calcolo incongruenti con il tipo calcolo.";
                    return false;
                }

                if (datiCalcoloRetributivo.ImportoQuotaB.HasValue && datiCalcoloRetributivo.GiorniQuotaB707.HasValue) // Quota B
                {
                    if (datiContribuzioneENPALSLiq != null && datiContribuzioneENPALSLiq.QuotaB != null)
                    {
                        if (datiCalcoloRetributivo.GiorniQuotaB707.GetValueOrDefault() <
                            datiContribuzioneENPALSLiq.QuotaB.Enpals.GetValueOrDefault() + datiContribuzioneENPALSLiq.QuotaB.Inps.GetValueOrDefault() +
                            datiContribuzioneENPALSLiq.QuotaB.Figurativa.GetValueOrDefault() + datiContribuzioneENPALSLiq.QuotaB.Volontaria.GetValueOrDefault() +
                            datiContribuzioneENPALSLiq.QuotaB.Ufficio.GetValueOrDefault())
                        {
                            messaggioVideo = "Non è possibile lavorare la domanda. Il valore contenuto in “Giorni 707” per la quota B non può essere minore della somma dei valori dei giorni della quota B e dei giorni della quota D relative alla stessa gestione.";
                            return false;
                        }
                    }
                }

                if (datiCalcoloRetributivo.GiorniQuotaA707.HasValue && !datiCalcoloRetributivo.RMQuotaA.HasValue)
                {
                    messaggioVideo = "Non è possibile lavorare la domanda. Se sono presenti i “Giorni 707” della quota A deve essere presente anche la “Retribuzione Media” della quota A.";
                    return false;
                }

                if (datiCalcoloRetributivo.GiorniQuotaB707.HasValue && !datiCalcoloRetributivo.RMQuotaB.HasValue)
                {
                    messaggioVideo = "Non è possibile lavorare la domanda. Se sono presenti i “Giorni 707” della quota B deve essere presente anche la “Retribuzione Media” della quota B.";
                    return false;
                }
            }

            return true;
        }

        private static bool ControlsDatiContributivi(out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            return true;
        }

        private static bool ControlsDatiPensione(GestionePensione.DatiPensione datiPensioneMaster, GestionePensione.DatiPensione datiPensioneDaPrelievo, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string messaggioVideo)
        {
            messaggioVideo = "Non è possibile lavorare la domanda. Mancano alcune informazioni provenienti dal SAI.";

            if (datiPensioneMaster == null && datiPensioneDaPrelievo == null)
                return false;

            if ((datiPensioneMaster == null || !datiPensioneMaster.DecorrenzaOriginaria.HasValue) && (datiPensioneDaPrelievo == null || !datiPensioneDaPrelievo.DecorrenzaOriginaria.HasValue))
                return false;

            if ((datiPensioneMaster == null || !datiPensioneMaster.InizioAssicurazione.HasValue) && (datiPensioneDaPrelievo == null || !datiPensioneDaPrelievo.InizioAssicurazione.HasValue))
                return false;

            if ((datiPensioneMaster == null || !datiPensioneMaster.FineAssicurazione.HasValue) && (datiPensioneDaPrelievo == null || !datiPensioneDaPrelievo.FineAssicurazione.HasValue))
                return false;

            if ((datiPensioneMaster == null || !datiPensioneMaster.TipoCalcolo.HasValue) && (datiPensioneDaPrelievo == null || !datiPensioneDaPrelievo.TipoCalcolo.HasValue))
                return false;

            //ENG - RIC Supplemento ai Superstiti: il controllo non deve essere più effettuato
            if (!(!String.IsNullOrEmpty(datiPensioneMaster.SiglaCategoria) && datiPensioneMaster.SiglaCategoria.Trim().ToUpperInvariant().StartsWith("S") && Utility.IsRicostituzione_Supplemento(datiPensioneMaster) && datiPensioneMaster.Tipo == "0001"))
            {
                if (datiDanteCausa != null && datiDanteCausa.DataMorte.HasValue && datiPensioneMaster != null &&
                    datiPensioneMaster.DecorrenzaOriginaria.Value != Utility.FirstDayOfMonth(datiDanteCausa.DataMorte.Value.AddMonths(1)))
                {
                    messaggioVideo = string.Format("La decorrenza pensione ({0:dd/MM/yyyy}) deve essere pari al mese successivo alla data di morte del dante causa ({1:dd/MM/yyyy}).",
                        datiPensioneMaster.DecorrenzaOriginaria.Value, datiDanteCausa.DataMorte.Value);
                    return false;
                }
            }

            //messaggioVideo = "Non è possibile lavorare la domanda. Dati calcolo incongruenti con il tipo calcolo";
            //if (!Utility.IsRicostituzione_Supplemento(datiPensioneMaster))
            //{
            //    switch (tipoCalcolo)
            //    {
            //        // Contributivo
            //        case 'C':
            //        case 'V':
            //            if (datiPensioneMaster.TipoCalcolo.GetValueOrDefault() != 1 && (datiPensioneDaPrelievo == null || datiPensioneDaPrelievo.TipoCalcolo.GetValueOrDefault() != 1))
            //                return false;
            //            break;
            //        // Retributivo
            //        case 'R':
            //        case 'S':
            //            if (datiPensioneMaster.TipoCalcolo.GetValueOrDefault() != 2 && (datiPensioneDaPrelievo == null || datiPensioneDaPrelievo.TipoCalcolo.GetValueOrDefault() != 2))
            //                return false;
            //            break;
            //        // Misto
            //        case 'M':
            //        case 'T':
            //            if (datiPensioneMaster.TipoCalcolo.GetValueOrDefault() != 21 && (datiPensioneDaPrelievo == null || datiPensioneDaPrelievo.TipoCalcolo.GetValueOrDefault() != 21))
            //                return false;
            //            break;
            //        case 'Z':
            //        case 'H':
            //            if ((datiPensioneMaster.TipoCalcolo.GetValueOrDefault() != 26 && (datiPensioneDaPrelievo == null || datiPensioneDaPrelievo.TipoCalcolo.GetValueOrDefault() != 26)) ||
            //                (datiPensioneMaster.FineAssicurazione.HasValue && !Utility.DataSuccessivaA(datiPensioneMaster.FineAssicurazione.Value, new DateTime(2012, 1, 1))) ||
            //                (!datiPensioneMaster.FineAssicurazione.HasValue && datiPensioneDaPrelievo != null && datiPensioneDaPrelievo.FineAssicurazione.HasValue && 
            //                    !Utility.DataSuccessivaA(datiPensioneMaster.FineAssicurazione.Value, new DateTime(2012, 1, 1))
            //                )
            //               )
            //                return false;
            //            break;
            //case 'K':
            //                    case 'J':
            //                        if ((datiPensioneMaster.TipoCalcolo.GetValueOrDefault() != 27 && (datiPensioneDaPrelievo == null || datiPensioneDaPrelievo.TipoCalcolo.GetValueOrDefault() != 27)) ||
            //                            (datiPensioneMaster.FineAssicurazione.HasValue && !Utility.DataSuccessivaA(datiPensioneMaster.FineAssicurazione.Value, new DateTime(2012, 1, 1))) ||
            //                            (!datiPensioneMaster.FineAssicurazione.HasValue && datiPensioneDaPrelievo != null && datiPensioneDaPrelievo.FineAssicurazione.HasValue &&
            //                                !Utility.DataSuccessivaA(datiPensioneMaster.FineAssicurazione.Value, new DateTime(2012, 1, 1))
            //                            )
            //                           )
            //                            return false;
            //                        break;
            //        default:
            //            return false;
            //    }
            //}


            messaggioVideo = string.Empty;
            return true;
        }

        private static bool ControlsDatiENPALS(GestionePensione.DatiPensione datiPensione, GestioneEnpals.DatiEnpals datiENPALSMaster, GestioneEnpals.DatiEnpals datiENPALSDaPrelievo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiENPALSMaster == null && datiENPALSDaPrelievo == null)
            {
                messaggioVideo = "Non è possibile lavorare la domanda. Mancano alcune informazioni provenienti dal SAI.";
                return false;
            }

            if ((datiENPALSMaster == null || !datiENPALSMaster.ImportoPensione.HasValue) && (datiENPALSDaPrelievo == null || !datiENPALSDaPrelievo.ImportoPensione.HasValue))
            {
                messaggioVideo = "Non è possibile lavorare la domanda. Manca l'Importo Pensione.";
                return false;
            }

            if (Utility.IsDomandaSperimentaleDonna(datiPensione))
            {
                if (datiENPALSMaster != null && datiENPALSMaster.CodiceTipoDomanda != "OD")
                {
                    messaggioVideo = "Per la domanda di regime sperimentale donna il codice tipo domanda SAI deve essere pari a OD.";
                    return false;
                }
            }
            else if (Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione))
            {
                if (datiENPALSMaster != null && datiENPALSMaster.CodiceTipoDomanda != "ON")
                {
                    messaggioVideo = "Per la domanda di regime sperimentale donna (2019) il codice tipo domanda SAI deve essere pari a ON.";
                    return false;
                }
            }

            return true;
        }

        private static bool ControllaStatoPensionePerAggiornamento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoSAI)
                return true;
            else
                return false;
        }

        private static bool ControlsAggiornamentoFondoWebDom(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, char tipoCalcolo, bool? isChiaveFascicoloGenerata, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, string codiceFiscaleDanteCausa, out bool isNuovoCertificatoGeneratoEnpals, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool aggiornamentoFondoEffettuato = false;
            isNuovoCertificatoGeneratoEnpals = false;
            string siglaCategoriaOriginale = datiPensione.SiglaCategoria;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoGeneraFascicoloCertificato = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneGeneraCertificatoFascicoloENPALS", out controlloDinamicoGeneraFascicoloCertificato);

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                return true;

            switch (tipoCalcolo)
            {
                case 'R':
                case 'C':
                case 'M':
                case 'Z':
                case 'K':
                    switch (datiPensione.SiglaCategoria.Trim())
                    {
                        case "VOSPORT":
                        case "IOSPORT":
                        case "SOSPORT":
                            if (!GestioneWebDom.AggiornaFondoWebDom(datiPensione, matricolaOperatore, sedeOperatore, "001", out messaggioVideo))
                                return false;
                            aggiornamentoFondoEffettuato = true;
                            break;
                    }
                    break;
                case 'S':
                case 'V':
                case 'T':
                case 'H':
                case 'J':
                    switch (datiPensione.SiglaCategoria.Trim().ToUpperInvariant())
                    {
                        case "VOSPETT":
                        case "IOSPETT":
                        case "SOSPETT":
                            if (!GestioneWebDom.AggiornaFondoWebDom(datiPensione, matricolaOperatore, sedeOperatore, "002", out messaggioVideo))
                                return false;
                            aggiornamentoFondoEffettuato = true;
                            break;
                    }
                    break;
            }

            // Bisogna aggiornare i dati pensione con i valori nuovi
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            GestioneWebDom.GetDomandaPerDomus(datiPensione.NDomus.ToString(), out datiDomanda, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return false;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Istanza != null && datiDomanda.Dati.Istanza.Count > 0)
            {
                datiPensione.SiglaCategoria = datiDomanda.Dati.Istanza[0].SiglaCatLav;
                datiPensione.Fondo = datiDomanda.Dati.Istanza[0].CodFondo;
            }

            /* Se cambia la categoria, bisogna chiamare il servizio per generare il fascicolo/certificato per poter aprire il fascicolo e/o il certificato
            sulla nuova categoria */
            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && controlloDinamicoGeneraFascicoloCertificato != null && !String.IsNullOrEmpty(controlloDinamicoGeneraFascicoloCertificato.ValoreControllo) && controlloDinamicoGeneraFascicoloCertificato.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if (aggiornamentoFondoEffettuato)
                {
                    if (siglaCategoriaOriginale != datiPensione.SiglaCategoria)
                    {
                        if (Utility.IsDomandaPensioneIndiretta(datiPensione) && isChiaveFascicoloGenerata.GetValueOrDefault())
                        {
                            if (!String.IsNullOrEmpty(codiceFiscaleDanteCausa) && datiDanteCausa != null)
                            {
                                ServiceReferences.GeneraCertificati.FascicoloOutput fascicolo = null;
                                short codiceCategoria = 0;
                                string codCategoria = string.Empty;
                                GestioneDecodifica.GetCodCategoriaBySiglaCategoria(datiPensione.SiglaCategoria, out codCategoria);
                                short.TryParse(codCategoria, out codiceCategoria);
                                GestioneGeneraCertificati.GeneraFascicolo(datiPensione.NDomus.ToString(), codiceCategoria.ToString(), "9990", codiceFiscaleDanteCausa, out fascicolo, out messaggioVideo);

                                if (!String.IsNullOrEmpty(messaggioVideo))
                                    return false;
                                
                                datiDanteCausa.CategoriaFascicolo = codiceCategoria;
                                datiDanteCausa.SedeFascicolo = 9990;
                                datiDanteCausa.NumeroFascicolo = (int)fascicolo.NumeroFascicolo;
                                datiDanteCausa.IsFascicoloGenerato = true;
                            }

                        }

                        if (datiPensione.Gruppo == "0001" || (Utility.IsDomandaPensioneIndiretta(datiPensione) && isChiaveFascicoloGenerata.GetValueOrDefault()))
                        {
                            ServiceReferences.GeneraCertificati.CertificatoInput rich = new ServiceReferences.GeneraCertificati.CertificatoInput();
                            rich.SiglaCategoria = datiPensione.SiglaCategoria.Trim().ToUpperInvariant();
                            if (datiPensione.CodiceSedeDestinazione.HasValue)
                                rich.CodiceSede = datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0');
                            else
                                rich.CodiceSede = datiPensione.CodiceSede.ToString().PadLeft(4, '0');

                            ServiceReferences.GeneraCertificati.CertificatoOutput areaRisposta = null;
                            if (!GestioneGeneraCertificati.GeneraCertificato(datiPensione.NDomus.ToString(), rich, out areaRisposta, out messaggioVideo))
                                return false;

                            if (areaRisposta != null && !string.IsNullOrEmpty(areaRisposta.Certificato))
                            {
                                int res = 0;
                                int.TryParse(areaRisposta.Certificato, out res);
                                datiPensione.NCertificato = res;
                                isNuovoCertificatoGeneratoEnpals = true;
                            }
                            else
                            {
                                messaggioVideo = string.Format("Errore nell'assegnazione del certificato per la categoria {0} su sede {1}", rich.SiglaCategoria, rich.CodiceSede);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        internal static bool GetDatiWsSai(out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            string stackTrace = null;

            PNBTW01Service proxy = new PNBTW01Service();
            ProgramInterface input = new ProgramInterface();
            ProgramInterface1 output = null;
            input.getsai = new ProgramInterfaceGetsai();
            input.getsai.nbw1_dati_input = new ProgramInterfaceGetsaiNbw1_dati_input();
            input.getsai.nbw1_dati_input.nbw1i_categoria = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_cod_fisc = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_dt_calcolo = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_nr_certif = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_num_dom_inps = "2146661500006";
            input.getsai.nbw1_dati_input.nbw1i_sede = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_tipo_liq = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_tipo_ric = TipoRichiesta.GET.GETSAI.ToString();
            try
            {
                proxy.Url = GetUrl();
                output = proxy.PNBTW01Operation(input);
            }
            catch (SoapException exception)
            {
                errori = string.Format("{0} | {1}", Utility.GetMessageFromException(exception), exception.Detail != null ? exception.Detail.InnerText : string.Empty);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return false;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                errori = Utility.GetMessageFromException(exception);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return false;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
            {
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio SAI: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                {
                    string messaggio = errori;
                    errori = "Errore nel recupero dei dati dai sistemi ENPALS";
                    string parametri = null;
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
            }

            return true;
        }

        /// <summary>
        /// Mapping tra i dati ritornati dal servizio estero SAI con il nostro oggetto SAI. 
        /// </summary>
        public static void ValorizzaDatiFromExternalService(ProgramInterface1 datiSAI_Input, out SAI datiSAI_Output)
        {
            datiSAI_Output = new SAI();

            datiSAI_Output.GETSAI_AAMM_TRA_DIR = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_aamm_tra_dir;
            datiSAI_Output.GETSAI_COEFF_TRASF = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_coeff_trasf;
            datiSAI_Output.GETSAI_DES_DEROGA1 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_deroga1;
            datiSAI_Output.GETSAI_DES_DEROGA2 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_deroga2;
            datiSAI_Output.GETSAI_DES_DEROGA3 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_deroga3;
            datiSAI_Output.GETSAI_DES_DEROGA4 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_deroga4;
            datiSAI_Output.GETSAI_DES_ERRORE = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_des_errore;
            datiSAI_Output.GETSAI_DT_DECORRENZA = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_dt_decorrenza.Replace("-", "");
            if (datiSAI_Output.GETSAI_DT_DECORRENZA == "00010101")
                datiSAI_Output.GETSAI_DT_DECORRENZA = null;
            datiSAI_Output.GETSAI_DT_PRI_CTB = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_dt_pri_ctb.Replace("-", "");
            if (datiSAI_Output.GETSAI_DT_PRI_CTB == "00010101")
                datiSAI_Output.GETSAI_DT_PRI_CTB = null;
            datiSAI_Output.GETSAI_DT_RAG_REQ = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_dt_rag_req.Replace("-", ""); // da Mappare a DB - è possibile che sia la Data Perfezionamento Requisiti
            if (datiSAI_Output.GETSAI_DT_RAG_REQ == "00010101")
                datiSAI_Output.GETSAI_DT_RAG_REQ = null;
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_esito))
                datiSAI_Output.GETSAI_ESITO = char.Parse(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_esito);
            datiSAI_Output.GETSAI_ETA_MAT_DIR = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_eta_mat_dir;
            datiSAI_Output.GETSAI_ETA_MAT_MIS = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_eta_mat_mis;
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_gru_dir))
                datiSAI_Output.GETSAI_GRU_DIR = char.Parse(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_gru_dir);
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_gru_prev))
                datiSAI_Output.GETSAI_GRU_PREV = char.Parse(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_gru_prev);
            datiSAI_Output.GETSAI_IMP_CONTR = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_contr;
            datiSAI_Output.GETSAI_IMP_PRT = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_prt;
            datiSAI_Output.GETSAI_IMP_QUA = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_qua;
            datiSAI_Output.GETSAI_IMP_QUB = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_qub;
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_ind_ibt))
                datiSAI_Output.GETSAI_IND_IBT = char.Parse(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_ind_ibt);
            datiSAI_Output.GETSAI_MONT_CMP = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_mont_cmp;
            datiSAI_Output.GETSAI_NR_CTB_ANTE = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_ante;
            datiSAI_Output.GETSAI_NR_CTB_DIR = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_dir;
            datiSAI_Output.GETSAI_NR_CTB_MIS = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_mis;
            datiSAI_Output.GETSAI_NR_CTB_NL155 = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_nl155;
            datiSAI_Output.GETSAI_NR_CTB_NL222 = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_nl222;
            datiSAI_Output.GETSAI_NR_CTB_NVV = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_nvv;
            datiSAI_Output.GETSAI_NR_CTB_POST = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_post;
            datiSAI_Output.GETSAI_NR_CTB_QUOA = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_quoa;
            datiSAI_Output.GETSAI_NR_CTB_QUOB = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_ctb_quob;
            datiSAI_Output.GETSAI_NR_TOT_CTB = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_tot_ctb;
            datiSAI_Output.GETSAI_NR_TOT_CTB_OBG = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_nr_tot_ctb_obg;
            datiSAI_Output.GETSAI_QUAL_PREV = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_qual_prev;
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_rag_prev))
                datiSAI_Output.GETSAI_RAG_PREV = char.Parse(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_rag_prev);
            datiSAI_Output.GETSAI_RTB_MED_540 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_rtb_med_540;
            datiSAI_Output.GETSAI_RTB_MED_POST = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_rtb_med_post;
            datiSAI_Output.GETSAI_TOT_CTB_QUAL = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_tot_ctb_qual;
            datiSAI_Output.GETSAI_TOT_CTB_QUAL_QNQ = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_tot_ctb_qual_qnq;
            datiSAI_Output.GETSAI_TOT_CTB_QUAL_TRI = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_tot_ctb_qual_tri;
            datiSAI_Output.OBM_CM_DT_FINESTRA = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_dt_finestra.Replace("-", "");
            if (datiSAI_Output.OBM_CM_DT_FINESTRA == "00010101")
                datiSAI_Output.OBM_CM_DT_FINESTRA = null;
            datiSAI_Output.OBM_CM_IMP_RTV = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_rtv;
            datiSAI_Output.GETSAI_DT_FIN_ASS = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_dt_fin_ass.Replace("-", "");
            if (datiSAI_Output.GETSAI_DT_FIN_ASS == "00010101")
                datiSAI_Output.GETSAI_DT_FIN_ASS = null;
            datiSAI_Output.GETSAI_DT_FIN_SUP = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_dt_fin_sup.Replace("-", ""); // da Mappare a DB - Fine Supplemento
            if (datiSAI_Output.GETSAI_DT_FIN_SUP == "00010101")
                datiSAI_Output.GETSAI_DT_FIN_SUP = null;
            datiSAI_Output.GETSAI_DT_INIS_SUP = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_dt_inis_sup.Replace("-", ""); // da Mappare a DB - Inizio Supplemento
            if (datiSAI_Output.GETSAI_DT_INIS_SUP == "00010101")
                datiSAI_Output.GETSAI_DT_INIS_SUP = null;
            datiSAI_Output.GETSAI_IMP_PENS = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_pens;
            datiSAI_Output.GETSAI_IMP_SUP = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_sup; // da Mappare a DB - Importo Supplemento
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_sistema_calcolo))
                datiSAI_Output.GETSAI_SISTEMA_CALCOLO = char.Parse(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_sistema_calcolo);
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_tip_liq))
                datiSAI_Output.GETSAI_TIP_LIQ = char.Parse(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_tip_liq); // da Mappare a DB - Flag Provvisoria
            datiSAI_Output.GETSAI_NUM_CTB_ENP_ANTE = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_enp_ante; // Tabella: ContribuzioneENPALS Campo: ENPALS quota A
            datiSAI_Output.GETSAI_NUM_CTB_ENP_POST = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_enp_post; // Tabella: ContribuzioneENPALS Campo: ENPALS quota B
            datiSAI_Output.GETSAI_NUM_CTB_ENP_CONT = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_enp_cont; // Tabella: ContribuzioneENPALS Campo: ENPALS quota C
            datiSAI_Output.GETSAI_NUM_CTB_FIG_ANTE = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_fig_ante; // Tabella: ContribuzioneENPALS Campo: Figurativa quota A
            datiSAI_Output.GETSAI_NUM_CTB_FIG_POST = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_fig_post; // Tabella: ContribuzioneENPALS Campo: Figurativa quota B
            datiSAI_Output.GETSAI_NUM_CTB_FIG_CONT = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_fig_cont; // Tabella: ContribuzioneENPALS Campo: Figurativa quota C
            datiSAI_Output.GETSAI_NUM_CTB_UFF_ANTE = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_uff_ante; // Tabella: ContribuzioneENPALS Campo: Ufficio quota A
            datiSAI_Output.GETSAI_NUM_CTB_UFF_POST = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_uff_post; // Tabella: ContribuzioneENPALS Campo: Ufficio quota B
            datiSAI_Output.GETSAI_NUM_CTB_UFF_CONT = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_uff_cont; // Tabella: ContribuzioneENPALS Campo: Ufficio quota C
            datiSAI_Output.GETSAI_NUM_CTB_INPS_ANTE = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_inps_ante; // Tabella: ContribuzioneENPALS Campo: INPS quota A
            datiSAI_Output.GETSAI_NUM_CTB_INPS_POST = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_inps_post; // Tabella: ContribuzioneENPALS Campo: INPS quota B
            datiSAI_Output.GETSAI_NUM_CTB_INPS_CONT = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_inps_cont; // Tabella: ContribuzioneENPALS Campo: INPS quota C
            datiSAI_Output.GETSAI_NUM_CTB_VV_ANTE = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_vv_ante; // Tabella: ContribuzioneENPALS Campo: Volontaria quota A
            datiSAI_Output.GETSAI_NUM_CTB_VV_POST = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_vv_post; // Tabella: ContribuzioneENPALS Campo: Volontaria quota B
            datiSAI_Output.GETSAI_NUM_CTB_VV_CONT = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_vv_cont; // Tabella: ContribuzioneENPALS Campo: Volontaria quota C
            datiSAI_Output.GETSAI_NUM_CTB_EST_ANTE = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_est_ante; // Tabella: ContribuzioneENPALS Campo: Estera quota A
            datiSAI_Output.GETSAI_NUM_CTB_EST_POST = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_est_post; // Tabella: ContribuzioneENPALS Campo: Estera quota B
            datiSAI_Output.GETSAI_NUM_CTB_EST_CONT = (int)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_est_cont; // Tabella: ContribuzioneENPALS Campo: Estera quota C

            datiSAI_Output.GETSAI_COD_TIP_DOM = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_cod_tip_dom; // Tabella: Enpals Campo: CodiceTipoDomanda
            datiSAI_Output.GETSAI_TIP_PEN = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_tipo_pen; // Tabella: Enpals Campo: TipoPensione

            datiSAI_Output.GETSAI_NR_CTB_POST_707 = (short)datiSAI_Input.getsao.nbw1_dati_output.nbw1o_num_ctb_post_707;
            datiSAI_Output.GETSAI_IMP_QUA_707 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_qua_707;
            datiSAI_Output.GETSAI_IMP_QUB_707 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_qub_707;
            datiSAI_Output.GETSAI_IMP_PENS_707 = datiSAI_Input.getsao.nbw1_dati_output.nbw1o_imp_pens_707;
            if (!string.IsNullOrEmpty(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_filler))
            {
                datiSAI_Output.GETSAI_ANZ_CONTR = (short)Math.Floor((Utility.StringToNullableDecimal(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_filler.Substring(0, 3)).GetValueOrDefault() * 52M) +
                    (Utility.StringToNullableDecimal(datiSAI_Input.getsao.nbw1_dati_output.nbw1o_filler.Substring(3, 2)).GetValueOrDefault() * 4.33M));
            }
        }

        private static void SbloccoSAI_Private(long NumeroDomanda, TipoRichiesta.SBL? sb, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            string stackTrace = null;

            PNBTW01Service proxy = new PNBTW01Service();
            Guid guid = Guid.NewGuid();
            ProgramInterface input = new ProgramInterface();
            ProgramInterface1 output = null;
            input.getsai = new ProgramInterfaceGetsai();
            input.getsai.nbw1_dati_input = new ProgramInterfaceGetsaiNbw1_dati_input();
            input.getsai.nbw1_dati_input.nbw1i_categoria = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_cod_fisc = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_dt_calcolo = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_nr_certif = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_num_dom_inps = NumeroDomanda.ToString();
            input.getsai.nbw1_dati_input.nbw1i_sede = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_tipo_liq = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_tipo_ric = sb.ToString();

            Utility.MetodoServizio? metodoServizio = Utility.GetValueFromDescription<Utility.MetodoServizio>(sb.ToString());

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (metodoServizio.HasValue)
                        GestioneLogSoap.SalvaLogSoap(input, Utility.Servizio.SrvSAI, metodoServizio.Value, Utility.SOAPLogDirection.IN, NumeroDomanda.ToString(), guid);

                    proxy.Url = GetUrl();
                    output = proxy.PNBTW01Operation(input);

                    if (metodoServizio.HasValue)
                        GestioneLogSoap.SalvaLogSoap(output, Utility.Servizio.SrvSAI, metodoServizio.Value, Utility.SOAPLogDirection.OUT, NumeroDomanda.ToString(), guid);
                }
                catch (SoapException exception)
                {
                    messaggioVideo = string.Format("{0} | {1}", Utility.GetMessageFromException(exception), exception.Detail != null ? exception.Detail.InnerText : string.Empty);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nella chiamata al servizio SAI: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore nel processo di sblocco della domanda sui sistemi ENPALS";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        GestioneLogGenerico.SalvaLogGenerico(NumeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                }
            }

            if (output != null && !string.IsNullOrEmpty(output.getsao.nbw1_dati_output.nbw1o_des_errore) && !string.IsNullOrEmpty(output.getsao.nbw1_dati_output.nbw1o_des_errore.Trim()))
            {
                messaggioVideo = "Errore dal servizio SAI: " + output.getsao.nbw1_dati_output.nbw1o_des_errore;
                return;
            }
        }

        private static void AggiornaSAI_Private(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, TipoRichiesta.PAG? tipoRichiesta, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            string stackTrace = null;

            PNBTW01Service proxy = new PNBTW01Service();
            Guid guid = Guid.NewGuid();
            ProgramInterface input = new ProgramInterface();
            ProgramInterface1 output = null;
            input.getsai = new ProgramInterfaceGetsai();
            input.getsai.nbw1_dati_input = new ProgramInterfaceGetsaiNbw1_dati_input();
            if (datiDanteCausa != null && datiDanteCausa.CategoriaFascicolo.HasValue && datiDanteCausa.SedeFascicolo.HasValue && datiDanteCausa.NumeroFascicolo.HasValue)
            {
                input.getsai.nbw1_dati_input.nbw1i_categoria = datiDanteCausa.CategoriaFascicolo.ToString().PadLeft(4, '0').Substring(1);
                input.getsai.nbw1_dati_input.nbw1i_nr_certif = datiDanteCausa.NumeroFascicolo.ToString().PadLeft(8, '0');
                input.getsai.nbw1_dati_input.nbw1i_sede = datiDanteCausa.SedeFascicolo.ToString().PadLeft(4, '0');
            }
            else
            {
                string codCat = datiPensione.GetCodCategoria();
                input.getsai.nbw1_dati_input.nbw1i_categoria = codCat.PadLeft(4, '0').Substring(1);
                input.getsai.nbw1_dati_input.nbw1i_nr_certif = datiPensione.NCertificato.GetValueOrDefault().ToString().PadLeft(8, '0');
                input.getsai.nbw1_dati_input.nbw1i_sede = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0');
            }

            input.getsai.nbw1_dati_input.nbw1i_cod_fisc = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_dt_calcolo = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.ToString("dd.MM.yyyy") : string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_num_dom_inps = datiPensione.NDomus.ToString();
            input.getsai.nbw1_dati_input.nbw1i_tipo_liq = string.Empty;
            input.getsai.nbw1_dati_input.nbw1i_tipo_ric = tipoRichiesta.ToString();

            Utility.MetodoServizio? metodoServizio = Utility.GetValueFromDescription<Utility.MetodoServizio>(tipoRichiesta.ToString());

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (metodoServizio.HasValue)
                        GestioneLogSoap.SalvaLogSoap(input, Utility.Servizio.SrvSAI, metodoServizio.Value, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

                    proxy.Url = GetUrl();
                    output = proxy.PNBTW01Operation(input);

                    if (metodoServizio.HasValue)
                        GestioneLogSoap.SalvaLogSoap(output, Utility.Servizio.SrvSAI, metodoServizio.Value, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                }
                catch (SoapException exception)
                {
                    messaggioVideo = exception.Message + exception.Detail != null ? exception.Detail.InnerText : string.Empty;
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio SAI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nella chiamata al servizio SAI: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore nel processo di sblocco della domanda sui sistemi ENPALS";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                }
            }

            if (output != null && !string.IsNullOrEmpty(output.getsao.nbw1_dati_output.nbw1o_des_errore) && !string.IsNullOrEmpty(output.getsao.nbw1_dati_output.nbw1o_des_errore.Trim()))
            {
                messaggioVideo = "Errore dal servizio SAI: " + output.getsao.nbw1_dati_output.nbw1o_des_errore;
                return;
            }
        }

        private static string GetUrl()
        {
            if (ConfigurationManager.AppSettings["UrlSAI"] != null)
                return ConfigurationManager.AppSettings["UrlSAI"];

            return null;
        }
    }
    public class TipoRichiesta
    {
        /*
         * GET
         */
        public enum GET
        {
            /// <summary>
            /// PL
            /// </summary>
            [Description("GETSAI")]
            GETSAI,
            /// <summary>
            /// Ricostituzione per supplemento
            /// </summary>
            [Description("GETSAS")]
            GETSAS,
            /// <summary>
            /// Riaperture
            /// </summary>
            [Description("GETSAY")]
            GETSAY,
            /// <summary>
            /// Ricostituzione per motivi contributivi
            /// </summary>
            [Description("GETSAR")]
            GETSAR
        }
        /*
         *PAG 
         */
        public enum PAG
        {
            /// <summary>
            /// PL
            /// </summary>
            [Description("PAGSAI")]
            PAGSAI,
            /// <summary>
            /// Riapertura
            /// </summary>
            [Description("PAGSAY")]
            PAGSAY,
            /// <summary>
            /// Ricostituzione per motivi contributivi
            /// </summary>
            [Description("PAGSAR")]
            PAGSAR,
            /// <summary>
            /// Ricostituzione per supplemento
            /// </summary>
            [Description("PAGSAS")]
            PAGSAS
        }
        /*
         * SBL
         */
        public enum SBL
        {
            [Description("SBLSAI")]
            SBLSAI,
            /// <summary>
            /// Riapertura
            /// </summary>
            [Description("SBLSAY")]
            SBLSAY,
            /// <summary>
            /// Ricostituzione per motivi contributivi
            /// </summary>
            [Description("SBLSAR")]
            SBLSAR,
            /// <summary>
            /// Ricostituzione per supplemento
            /// </summary>
            [Description("SBLSAS")]
            SBLSAS
        }
    };
}
