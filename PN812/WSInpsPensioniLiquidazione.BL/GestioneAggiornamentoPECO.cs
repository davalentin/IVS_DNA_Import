using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.AggPec;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.ServiceModel;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAggiornamentoPECO
    {
        #region public method
        public static bool AggiornaFelpe(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, string matricolaOperatore, short sedeOperatore, out string statoPensione,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            bool isCodiceEsito9 = false;

            if (!ControllaStatoPensionePerAggiornamento(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento Felpe";
                return false;
            }

            if (!AggiornaPECO(datiPensione, matricolaOperatore, sedeOperatore, out messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoFelpe);
                return false;
            }

            if (!GestioneOneriPrepensionamento.AggiornaOneri(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoOneri;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoOneri);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Oneri. " + messaggioVideo;
                return false;
            }

            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiPensione.IsDatiENPALSRecuperati.GetValueOrDefault())
            {
                if (!GestioneSAI.AggiornaSAI(datiPensione, datiDanteCausa, GestioneSAI.GetTipoRichiestaPAG(datiPensione), out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSAI;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSAI);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SAI. " + messaggioVideo;
                    return false;
                }
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                if (!GestioneINPDAP.AggiornaINPDAP(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSIN;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSIN);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SIN. " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Note di debito. " + messaggioVideo;
                    return false;
                }

                if (!GestioneINPDAP.AggiornaPianiDiPagamento(datiPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNo6Scatti;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNo6Scatti);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }

                if (!GestioneINPDAP.AggiornaEquoIndennizzo(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoEquoInd;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoEquoInd);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }

                if (!GestioneINPDAP.AggiornaIndennitaSpeciale(datiPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }
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
            if (!GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Note di debito. " + messaggioVideo;
                return false;
            }

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        internal static bool AggiornaPECO(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, out string errore)
        {
            errore = string.Empty;
            try
            {
                //if (datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value)               
                if (Utility.IsDomandaUnicarpe(datiPensione, false) == Utility.TipoUnicarpe.Yes)
                {
                    Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    switch (tipoAppartenenza.Value)
                    {
                        case Utility.TipoAppartenenza.FS:
                            if (Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                                Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) ||
                                Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                                Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) ||
                                Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) ||
                                tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                            {
                                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                                List<Utility.TipoFondo> listaTipoFondo_PECO_Fondi_AMG = Utility.GetListaTipoFondo_PECO_Fondi_AMG();
                                bool isGestioneAMG_Fondi_FSPT = false;
                                if (ConfigurationManager.AppSettings["GestioneAMG_Fondi_FSPT"] != null &&
                                    ConfigurationManager.AppSettings["GestioneAMG_Fondi_FSPT"] == "SI")
                                    isGestioneAMG_Fondi_FSPT = true;

                                if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI))
                                {
                                    csAggiornamentoPECO_Fondi_AMG_INPDAP datiFs = null;
                                    ValorizzaInputAggiornamentoPECO_Fondi_AMG_INPDAP(datiPensione, matricolaOperatore, tipoUnicarpe, out datiFs);
                                    Aggiornamento_PECO_Fondi_AMG_INPDAP(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ref datiFs,
                                        datiPensione.NDomus.ToString(), out errore);
                                    if (!String.IsNullOrEmpty(errore))
                                        return false;
                                    else if (datiFs != null && datiFs.A_Return_Code != 0)
                                    {
                                        errore = string.Format("Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto. Codice: {0}", datiFs.A_Return_Code.ToString());
                                        return false;
                                    }
                                }
                                else if ((isGestioneAMG_Fondi_FSPT && tipoFondo.HasValue && listaTipoFondo_PECO_Fondi_AMG.Contains(tipoFondo.Value)) ||
                                    (Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.AMG))
                                {
                                    csAggiornamentoPECO_Fondi_AMG datiFs = null;
                                    ValorizzaInputAggiornamentoPECO_Fondi_AMG(datiPensione, matricolaOperatore, tipoUnicarpe, out datiFs);
                                    Aggiornamento_PECO_Fondi_AMG(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ref datiFs,
                                        datiPensione.NDomus.ToString(), out errore);
                                    if (!String.IsNullOrEmpty(errore))
                                        return false;
                                    else if (datiFs != null && datiFs.A_Return_Code != 0)
                                    {
                                        errore = string.Format("Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto. Codice: {0}", datiFs.A_Return_Code.ToString());
                                        return false;
                                    }
                                }
                                else
                                {
                                    csAggiornamentoPECO_Fondi_Speciali datiFs = null;
                                    ValorizzaInputAggiornamentoPECO_FS(datiPensione, matricolaOperatore, tipoUnicarpe, out datiFs);
                                    AggiornamentoPECO_FS(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ref datiFs,
                                        datiPensione.NDomus.ToString(), out errore);
                                    if (!String.IsNullOrEmpty(errore))
                                        return false;
                                    else if (datiFs != null && datiFs.F_Return_Code != 0)
                                    {
                                        errore = string.Format("Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto. Codice: {0}", datiFs.F_Return_Code.ToString());
                                        return false;
                                    }
                                }
                            }
                            break;
                        case Utility.TipoAppartenenza.AGO:
                            csAggiornamentoPECO_AGO datiAgo = null;
                            if (Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                                Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) ||
                                Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                                Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) ||
                                Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || (Utility.IsDomandaAPEPrecoci(datiPensione) && !(Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria) && Utility.IsRiaperturaDomanda(datiPensione.Id))) ||
                                tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                            {
                                ValorizzaInputAggiornamentoPECO_AGO(datiPensione, matricolaOperatore, tipoUnicarpe, out datiAgo);
                                AggiornamentoPECO_AGO(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AGO"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AGO"], ref datiAgo,
                                    datiPensione.NDomus.ToString(), out errore);
                                if (!String.IsNullOrEmpty(errore))
                                    return false;
                                else if (datiAgo != null && datiAgo.PL_Return_Code != 0)
                                {
                                    errore = string.Format("Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto. Codice: {0}", datiAgo.PL_Return_Code.ToString());
                                    return false;
                                }
                            }
                            break;
                        case Utility.TipoAppartenenza.CI:
                            csAggiornamentoPECO_Convenzioni_Internazionali datiCi = null;
                            if (Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                                Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) ||
                                Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                                Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) ||
                                Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) ||
                                tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                            {
                                ValorizzaInputAggiornamentoPECO_CI(datiPensione, matricolaOperatore, tipoUnicarpe, out datiCi);
                                AggiornamentoPECO_CI(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_CI"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_CI"], ref datiCi,
                                    datiPensione.NDomus.ToString(), out errore);
                                if (!String.IsNullOrEmpty(errore))
                                    return false;
                                else if (datiCi != null && datiCi.CI_Return_Code != 0)
                                {
                                    errore = string.Format("Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto.Codice: {0}", datiCi.CI_Return_Code.ToString());
                                    return false;
                                }
                            }
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        internal static string GetModalitaLiquidazioneValue(int PL_CODPROV)
        {
            string sCODPROV = Convert.ToString(PL_CODPROV);
            List<GestioneDecodifica.DecModalitaLiquidazione> lModalitaLiquidazione = null;
            GestioneDecodifica.GetElencoDecModalitaLiquidazione(out lModalitaLiquidazione);
            if (lModalitaLiquidazione.Exists(x => x.ValoreAggPeco.Trim() == sCODPROV))
                return sCODPROV;
            else
                return null;
        }

        internal static string GetModalitaLiquidazioneValue(string pl_CodProv)
        {
            char cCodProv = Convert.ToChar(pl_CodProv.Trim());
            List<GestioneDecodifica.DecModalitaLiquidazione> lModalitaLiquidazione = null;
            GestioneDecodifica.GetElencoDecModalitaLiquidazione(out lModalitaLiquidazione);
            if (lModalitaLiquidazione.Exists(x => x.TraduzioneGp == cCodProv))
                return lModalitaLiquidazione.Find(x => x.TraduzioneGp == cCodProv).ValoreAggPeco;
            else
                return null;
        }

        internal static bool? GetFlagContributivaValue(string PL_SoloCPNTR)
        {
            return null;
        }

        internal static bool GetDatiPECO_FS(GestionePensione.DatiPensione datiPensione, string codFisc, Utility.TipoSalvaguardia TipoSalvaguardia, bool isRiapertura, ref csAggiornamentoPECO_Fondi_Speciali dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_FSbyNDomus(datiPensione, isRiapertura, ref dati, out errore))
                return false;

            else if (dati == null && TipoSalvaguardia != Utility.TipoSalvaguardia.Nessuna)
            {
                if (!GetDatiPECO_FSbyCodFisc(codFisc, datiPensione.NDomus.ToString(), ref dati, out errore))
                    return false;
            }
            return true;
        }

        internal static bool GetDatiPECO_AGO(GestionePensione.DatiPensione datiPensione, string codFisc, Utility.TipoSalvaguardia TipoSalvaguardia, bool isRiapertura, ref csAggiornamentoPECO_AGO dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_AGObyNDomus(datiPensione, isRiapertura, ref dati, out errore))
                return false;

            else if (dati == null && TipoSalvaguardia != Utility.TipoSalvaguardia.Nessuna)
            {
                if (!GetDatiPECO_AGObyCodFisc(codFisc, datiPensione.NDomus.ToString(), ref dati, out errore))
                    return false;
            }

            return true;
        }

        internal static bool GetDatiPECO_CI(GestionePensione.DatiPensione datiPensione, string codFisc, Utility.TipoSalvaguardia TipoSalvaguardia, bool isRiapertura, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_CIbyNDomus(datiPensione, isRiapertura, ref dati, out errore))
                return false;

            else if (dati == null && TipoSalvaguardia != Utility.TipoSalvaguardia.Nessuna && !(isRiapertura && Utility.IsDomandaTipoContributivo(datiPensione, null, true)))
            {
                if (!GetDatiPECO_CIbyCodFisc(codFisc, datiPensione.NDomus.ToString(), ref dati, out errore))
                    return false;
            }

            return true;
        }

        internal static bool GetCertificazionePerAutomatica(Utility.TipoAppartenenza? tipoAppartenenza, string codiceFiscale, string numDomanda, ref object datiAGGPECO, ref string errori)
        {
            if (tipoAppartenenza.HasValue)
            {
                string funzione = string.Empty;
                switch (tipoAppartenenza.Value)
                {
                    case Utility.TipoAppartenenza.FS:
                        funzione = ((csAggiornamentoPECO_Fondi_Speciali)datiAGGPECO).F_Funzione;
                        if (datiAGGPECO != null && (funzione == "L" || funzione == "H" || funzione == "G"))
                        {
                            csAggiornamentoPECO_Fondi_Speciali datiFS = (csAggiornamentoPECO_Fondi_Speciali)datiAGGPECO;
                            if (!GetDatiPECO_FSbyCodFisc(codiceFiscale, numDomanda, ref datiFS, out errori))
                                return false;
                            datiAGGPECO = datiFS;
                            return true;
                        }
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        funzione = ((csAggiornamentoPECO_AGO)datiAGGPECO).PL_Funzione;
                        if (datiAGGPECO != null && (funzione == "L" || funzione == "H" || funzione == "G"))
                        {
                            csAggiornamentoPECO_AGO datiAGO = (csAggiornamentoPECO_AGO)datiAGGPECO;
                            if (!GetDatiPECO_AGObyCodFisc(codiceFiscale, numDomanda, ref datiAGO, out errori))
                                return false;
                            datiAGGPECO = datiAGO;
                            return true;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        funzione = ((csAggiornamentoPECO_Convenzioni_Internazionali)datiAGGPECO).CI_Funzione;
                        if (datiAGGPECO != null && (funzione == "L" || funzione == "H" || funzione == "G"))
                        {
                            csAggiornamentoPECO_Convenzioni_Internazionali datiCI = (csAggiornamentoPECO_Convenzioni_Internazionali)datiAGGPECO;
                            if (!GetDatiPECO_CIbyCodFisc(codiceFiscale, numDomanda, ref datiCI, out errori))
                                return false;
                            datiAGGPECO = datiCI;
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        internal static bool GetCertificazione_AGO(Utility.TipoAppartenenza? tipoAppartenenza, string codiceFiscale, string numDomanda, ref csAggiornamentoPECO_AGO datiAGGPECO, ref string errori)
        {
            if (!GetDatiPECO_AGObyCodFisc(codiceFiscale, numDomanda, ref datiAGGPECO, out errori))
                return false;

            return true;
        }

        internal static bool GetCertificazione_FS(Utility.TipoAppartenenza? tipoAppartenenza, string codiceFiscale, string numDomanda, ref csAggiornamentoPECO_Fondi_Speciali datiAGGPECO, ref string errori)
        {
            if (!GetDatiPECO_FSbyCodFisc(codiceFiscale, numDomanda, ref datiAGGPECO, out errori))
                return false;

            return true;
        }

        internal static bool GetCertificazione_AMG(Utility.TipoAppartenenza? tipoAppartenenza, string codiceFiscale, string numDomanda, ref csAggiornamentoPECO_Fondi_AMG datiAGGPECO, ref string errori)
        {
            if (!GetDatiPECO_AMGbyCodFisc(codiceFiscale, numDomanda, ref datiAGGPECO, out errori))
                return false;

            return true;
        }

        internal static bool GetCertificazione_CI(Utility.TipoAppartenenza? tipoAppartenenza, string codiceFiscale, string numDomanda, ref csAggiornamentoPECO_Convenzioni_Internazionali datiAGGPECO,
            ref string errori)
        {
            if (!GetDatiPECO_CIbyCodFisc(codiceFiscale, numDomanda, ref datiAGGPECO, out errori))
                return false;

            return true;
        }

        public static bool GetDatiPECO_AMG(GestionePensione.DatiPensione datiPensione, string codFisc, Utility.TipoSalvaguardia TipoSalvaguardia, bool isRiapertura, ref csAggiornamentoPECO_Fondi_AMG dati,
            out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_AMGbyNDomus(datiPensione, isRiapertura, ref dati, out errore))
                return false;
            else if (dati == null && TipoSalvaguardia != Utility.TipoSalvaguardia.Nessuna)
            {
                if (!GetDatiPECO_AMGbyCodFisc(codFisc, datiPensione.NDomus.ToString(), datiPensione, ref dati, out errore))
                    return false;
            }

            return true;
        }

        public static bool GetDatiPECO_AMG_INPDAP(GestionePensione.DatiPensione datiPensione, string codFisc, Utility.TipoSalvaguardia TipoSalvaguardia,
            bool isRiapertura, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_AMG_INPDAPbyNDomus(datiPensione, isRiapertura, ref dati, out errore))
                return false;

            else if (dati == null && TipoSalvaguardia != Utility.TipoSalvaguardia.Nessuna)
            {
                if (!GetDatiPECO_AMG_INPDAPbyCodFisc(codFisc, datiPensione.NDomus.ToString(), datiPensione, ref dati, out errore))
                    return false;
            }

            return true;
        }

        public static bool GetDatiPECO_AGO_FunzioneC(string nDomus, string codFisc, ref string Caratterizzazione, out string errore)
        {
            errore = string.Empty;
            bool retVal = false;
            csAggiornamentoPECO_AGO dati = null;
            try
            {
                GestioneControlliDinamici.ControlloDinamico AbilitazioneMemo_74_88_2025_AGO = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo_74_88_2025_AGO", out AbilitazioneMemo_74_88_2025_AGO);
                if (AbilitazioneMemo_74_88_2025_AGO != null && !String.IsNullOrEmpty(AbilitazioneMemo_74_88_2025_AGO.ValoreControllo) && AbilitazioneMemo_74_88_2025_AGO.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    retVal = GetDatiPECO_AGO_FunzioneC(nDomus, codFisc, ref dati, out errore);
                    if (dati != null)
                    {
                        if (dati.PL_TipCert.Trim() == "POS" || dati.PL_TipoSpec.Trim() == "E")
                        {
                            long numeroDomanda = Convert.ToInt64(nDomus);
                            bool salvataggio = SalvaTipoSpecECaretterizzazionePensione(numeroDomanda, Utility.TipoOperazione.INSERIMENTO, ref Caratterizzazione);
                        }
                        else { retVal = false; }
                    }
                    else
                    {
                        retVal = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }

        public static bool GetDatiPECO_AGO_FunzioneC(GestionePensione.DatiPensione datiPensione, string codFisc, ref csAggiornamentoPECO_AGO dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_AGObyCodFisc(codFisc, datiPensione.NDomus.ToString(), ref dati, out errore))
                return false;
            return true;
        }

        private static bool GetDatiPECO_AGO_FunzioneC(string nDomus, string codFisc, ref csAggiornamentoPECO_AGO dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_AGObyCodFisc(codFisc, nDomus, ref dati, out errore))
                return false;
            return true;
        }

        public static bool GetDatiPECO_FS_FunzioneC(string nDomus, string codFisc, ref string Caratterizzazione, out string errore)
        {
            errore = string.Empty;
            bool retVal = false;
            csAggiornamentoPECO_Fondi_Speciali dati = null;

            try
            {
                GestioneControlliDinamici.ControlloDinamico AbilitazioneMemo_74_88_2025_FS = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo_74_88_2025_FS", out AbilitazioneMemo_74_88_2025_FS);
                if (AbilitazioneMemo_74_88_2025_FS != null && !String.IsNullOrEmpty(AbilitazioneMemo_74_88_2025_FS.ValoreControllo) && AbilitazioneMemo_74_88_2025_FS.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    retVal = GetDatiPECO_FS_FunzioneC(nDomus, codFisc, ref dati, out errore);
                    if (dati != null)
                    {
                        if (dati.F_TipCert.Trim() == "POS" || dati.F_TipoSpec.Trim() == "E")
                        {
                            long numeroDomanda = Convert.ToInt64(nDomus);
                            bool salvataggio = SalvaTipoSpecECaretterizzazionePensione(numeroDomanda, Utility.TipoOperazione.INSERIMENTO, ref Caratterizzazione);
                        }
                        else { retVal = false; }
                    }
                    else { retVal = false; }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }

        public static bool GetDatiPECO_FS_FunzioneC(string nDomus, string codFisc, ref csAggiornamentoPECO_Fondi_Speciali dati, out string errore)
        {
            return GetDatiPECO_FSbyCodFisc(codFisc, nDomus, ref dati, out errore);
        }

        public static bool GetDatiPECO_CI_FunzioneC(string nDomus, string codFisc, ref string Caratterizzazione, out string errore)
        {
            errore = string.Empty;
            bool retVal = false;
            csAggiornamentoPECO_Convenzioni_Internazionali dati = null;
            try
            {
                GestioneControlliDinamici.ControlloDinamico AbilitazioneMemo_74_88_2025_CI = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo_74_88_2025_CI", out AbilitazioneMemo_74_88_2025_CI);
                if (AbilitazioneMemo_74_88_2025_CI != null && !String.IsNullOrEmpty(AbilitazioneMemo_74_88_2025_CI.ValoreControllo) && AbilitazioneMemo_74_88_2025_CI.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    retVal = GetDatiPECO_CI_FunzioneC(nDomus, codFisc, ref dati, out errore);
                    if (dati != null)
                    {
                        if (dati.CI_TipCert.Trim() == "POS" || dati.CI_TipoSpec.Trim() == "E")
                        {
                            long numeroDomanda = Convert.ToInt64(nDomus);
                            bool salvataggio = SalvaTipoSpecECaretterizzazionePensione(numeroDomanda, Utility.TipoOperazione.INSERIMENTO, ref Caratterizzazione);
                        }
                        else { retVal = false; }
                    }
                    else { retVal = false; }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }

        public static bool GetDatiPECO_CI_FunzioneC(GestionePensione.DatiPensione datiPensione, string codFisc, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_CIbyCodFisc(codFisc, datiPensione.NDomus.ToString(), ref dati, out errore))
                return false;
            return true;
        }

        public static bool GetDatiPECO_CI_FunzioneC(string nDomus, string codFisc, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_CIbyCodFisc(codFisc, nDomus, ref dati, out errore))
                return false;
            return true;
        }

        public static bool GetDatiPECO_AMG_INPDAP_FunzioneC(string nDomus, string codFisc, ref string Caratterizzazione, out string errore)
        {
            errore = string.Empty;
            bool retVal = false;
            csAggiornamentoPECO_Fondi_AMG_INPDAP dati = null;
            try
            {
                GestioneControlliDinamici.ControlloDinamico AbilitazioneMemo_74_88_2025_GDP = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo_74_88_2025_GDP", out AbilitazioneMemo_74_88_2025_GDP);
                if (AbilitazioneMemo_74_88_2025_GDP != null && !String.IsNullOrEmpty(AbilitazioneMemo_74_88_2025_GDP.ValoreControllo) && AbilitazioneMemo_74_88_2025_GDP.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    retVal = GetDatiPECO_AMG_INPDAP_FunzioneC(nDomus, codFisc, ref dati, out errore);
                    if (dati.A_TipCert.Trim() == "POS") // || (dati.A_TipoSpec.Trim() == "C" || dati.A_TipoSpec == "D"))
                    {
                        long numeroDomanda = Convert.ToInt64(nDomus);
                        bool salvataggio = SalvaTipoSpecECaretterizzazionePensione(numeroDomanda, Utility.TipoOperazione.INSERIMENTO, ref Caratterizzazione);
                    }
                    else { retVal = false; }
                }
                else { retVal = false; }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }

        public static bool GetDatiPECO_AMG_INPDAP_FunzioneC(string nDomus, string codFisc, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out string errore)
        {
            return GetDatiPECO_AMG_INPDAPbyCodFisc(codFisc, nDomus, ref dati, out errore);
        }

        public static bool GetDatiPECO_AMG_FunzioneC(string nDomus, string codFisc, ref string Caratterizzazione, out string errore)
        {
            errore = string.Empty;
            bool retVal = false;
            csAggiornamentoPECO_Fondi_AMG dati = null;
            try
            {
                GestioneControlliDinamici.ControlloDinamico AbilitazioneMemo_74_88_2025_GDP = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo_74_88_2025_GDP", out AbilitazioneMemo_74_88_2025_GDP);
                if (AbilitazioneMemo_74_88_2025_GDP != null && !String.IsNullOrEmpty(AbilitazioneMemo_74_88_2025_GDP.ValoreControllo) && AbilitazioneMemo_74_88_2025_GDP.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    retVal = GestioneAggiornamentoPECO.GetDatiPECO_AMG_FunzioneC(nDomus, codFisc, ref dati, out errore);
                    if (dati != null)
                    {
                        if (dati.A_TipCert.Trim() == "POS" || dati.A_TipoSpec.Trim() == "E")
                        {
                            long numeroDomanda = Convert.ToInt64(nDomus);
                            bool salvataggio = SalvaTipoSpecECaretterizzazionePensione(numeroDomanda, Utility.TipoOperazione.INSERIMENTO, ref Caratterizzazione);
                        }
                        else { retVal = false; }
                    }
                    else { retVal = false; }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }

        public static bool GetDatiPECO_AMG_FunzioneC(string nDomus, string codFisc, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            return GetDatiPECO_AMGbyCodFisc(codFisc, nDomus, ref dati, out errore);
        }

        private static bool SalvaTipoSpecECaretterizzazionePensione(long numeroDomanda, Utility.TipoOperazione tipoOperazione, ref string Caratterizzazione)
        {
            GestionePensione.DatiPensione datiPensione = GetDatiPensioneByNumeroDomanda(numeroDomanda, null);
            bool retVal = false;
            try
            {
                string messaggioVideo = string.Empty;
                string CaratterizzazioneMerged = string.Empty;
                if (tipoOperazione == Utility.TipoOperazione.INSERIMENTO)
                {
                    CaratterizzazioneMerged = Utility.InserisciValoreCaratterizzazione(datiPensione.Caratterizzazione, '1', 3);
                }
                else if (tipoOperazione == Utility.TipoOperazione.CANCELLAZIONE)
                {
                    CaratterizzazioneMerged = Utility.EliminaValoreCaratterizzazione(datiPensione.Caratterizzazione, 3);
                }
                Caratterizzazione = CaratterizzazioneMerged;
                datiPensione.Caratterizzazione = CaratterizzazioneMerged;
                GestionePensione.SalvaPensione(datiPensione);
                retVal = true;
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            return retVal;
        }

        public static bool CleanTipoSpecECaratterizzazione(string nDomus, ref string Caratterizzazione)
        {
            bool retVal = false;
            try
            {
                long numeroDomanda = Convert.ToInt64(nDomus);
                retVal = SalvaTipoSpecECaretterizzazionePensione(numeroDomanda, Utility.TipoOperazione.CANCELLAZIONE, ref Caratterizzazione);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
            }

            return retVal;
        }

        public static GestionePensione.DatiPensione GetDatiPensioneByNumeroDomanda(Int64 numeroDomanda, byte? progStorico)
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);
            return datiPensione;
        }

        #endregion public method

        #region private method
        private static bool ControllaStatoPensionePerAggiornamento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoFelpe)
                return true;
            else
                return false;
        }

        #region FS
        private static bool GetDatiPECO_FSbyNDomus(GestionePensione.DatiPensione datiPensione, bool isRiapertura, ref csAggiornamentoPECO_Fondi_Speciali dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_Speciali();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.F_Funzione = "G";
                else if (isRiapertura)
                    dati.F_Funzione = "H";
                else
                    dati.F_Funzione = "L";
                dati.F_Numdomus = Convert.ToString(datiPensione.NDomus);
                AggiornamentoPECO_FS(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ref dati, datiPensione.NDomus.ToString(), out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                //ENG - Per le nuove OpzioniDonna automatiche KWA, KYA, KUA deve essere presente una valorizzaziuone Unicarpe
                if (dati.F_Return_Code != 0)
                {
                    dati = null;
                    if (datiPensione.GetFiltro() == "KWA" || datiPensione.GetFiltro() == "KYA" || datiPensione.GetFiltro() == "KUA")
                    {
                        errore = "ATTENZIONE per la tipologia di domanda selezionata è necessario effettuare la verifica del diritto sulla procedura Unicarpe. Se la domanda è da liquidare in modalità manuale occorre modificare la tipologia su Webdom";
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static bool GetDatiPECO_FSbyCodFisc(string codFisc, string numDomanda, ref csAggiornamentoPECO_Fondi_Speciali dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_Speciali();
                dati.F_Funzione = "C";
                dati.F_Codfisc = codFisc;
                AggiornamentoPECO_FS(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ref dati, numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                if (dati.F_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static void AggiornamentoPECO_FS(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_Speciali dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;

            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_Speciali, Utility.SOAPLogDirection.IN, numDomanda, guid, dati.F_Funzione);
                    proxy.Aggiornamento_PECO_Fondi_Speciali(ProgrChiamante, AppChiamante, ref dati);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, null, stackTrace);
                        errori = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                    }
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_Speciali, Utility.SOAPLogDirection.OUT, numDomanda, guid, dati.F_Funzione);
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void ValorizzaInputAggiornamentoPECO_FS(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, Utility.TipoUnicarpe tipoUnicarpe, out csAggiornamentoPECO_Fondi_Speciali datiFs)
        {
            datiFs = new csAggiornamentoPECO_Fondi_Speciali();

            if (datiPensione == null)
                return;

            switch (tipoUnicarpe)
            {
                case Utility.TipoUnicarpe.Automatica:
                default:
                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                        datiFs.F_Funzione = "V";
                    else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                        datiFs.F_Funzione = "P";
                    else
                        datiFs.F_Funzione = "R";
                    break;
                case Utility.TipoUnicarpe.Manuale:
                    datiFs.F_Funzione = "I";
                    break;
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            if (datiAnagrafici != null)
                datiFs.F_Codfisc = datiAnagrafici.CodiceFiscale;

            datiFs.F_Codicesede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') +
                Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
            datiFs.F_Numdomus = datiPensione.NDomus.ToString().PadLeft(13, '0');
            datiFs.F_Matricola = matricolaOperatore;
            GestioneIstruttoria.DatiIstruttoria istruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out istruttoria);

            List<GestioneDecodifica.ComunicazioneCampo3> elencoDecodificaComunicazioneCampo3 = null;
            GestioneDecodifica.GetComunicazioneCampo3(out elencoDecodificaComunicazioneCampo3);
            if (istruttoria != null && istruttoria.CodiceComunicazioneCampo3.HasValue && elencoDecodificaComunicazioneCampo3 != null && elencoDecodificaComunicazioneCampo3.Count > 0
                && elencoDecodificaComunicazioneCampo3.Exists(x => x.Id.Trim().ToUpperInvariant() == istruttoria.CodiceComunicazioneCampo3.ToString().ToUpperInvariant()))
                datiFs.F_Tipoliquid = "9";
            else
                datiFs.F_Tipoliquid = "4";
            datiFs.F_Data_Calc = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.Year.ToString().PadLeft(4, '0') +
                datiPensione.DataElaborazione.Value.Month.ToString().PadLeft(2, '0') +
                datiPensione.DataElaborazione.Value.Day.ToString().PadLeft(2, '0') : string.Empty;

            string categoria = datiPensione.GetCodCategoria();
            datiFs.F_Categoria = categoria.PadLeft(4, '0').Substring(1, 3);
            datiFs.F_Certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : string.Empty;
            datiFs.F_CF_Dantecausa = string.Empty;
            datiFs.F_Campo36 = datiPensione.GetFiltro();
        }

        #endregion FS

        #region AGO
        private static bool GetDatiPECO_AGObyNDomus(GestionePensione.DatiPensione datiPensione, bool isRiapertura, ref csAggiornamentoPECO_AGO dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_AGO();
                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneFunzioneAVESO92PerUNICARPE", out ctrl);
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.PL_Funzione = "G";
                else if (isRiapertura)
                {
                    if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || 
                        Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || 
                        Utility.IsDomandaESPA(datiPensione.SiglaCategoria) ||
                        Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria))
                        dati.PL_Funzione = "D";
                    else
                        dati.PL_Funzione = "H";
                }
                else if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || 
                    (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaVESO92(datiPensione.SiglaCategoria)) || 
                    Utility.IsDomandaESPA(datiPensione.SiglaCategoria) ||
                    Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria))
                    dati.PL_Funzione = "A";
                else
                    dati.PL_Funzione = "L";
                dati.PL_NumDomus = Convert.ToString(datiPensione.NDomus);
                AggiornamentoPECO_AGO(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AGO"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AGO"], ref dati,
                    datiPensione.NDomus.ToString(), out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                if (dati.PL_Return_Code != 0)
                {
                    dati = null;
                    //ENG - Per le nuove OpzioniDonna automatiche KWA, KYA, KUA deve essere presente una valorizzaziuone Unicarpe
                    if (datiPensione.GetFiltro() == "KWA" || datiPensione.GetFiltro() == "KYA" || datiPensione.GetFiltro() == "KUA")
                    {
                        errore = "ATTENZIONE per la tipologia di domanda selezionata è necessario effettuare la verifica del diritto sulla procedura Unicarpe. Se la domanda è da liquidare in modalità manuale occorre modificare la tipologia su Webdom";
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static bool GetDatiPECO_AGObyCodFisc(string codFisc, string numDomanda, ref csAggiornamentoPECO_AGO dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_AGO();
                dati.PL_Funzione = "C";
                dati.PL_Codicefiscale = codFisc;
                AggiornamentoPECO_AGO(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AGO"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AGO"], ref dati, numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                if (dati.PL_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static void AggiornamentoPECO_AGO(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_AGO dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            GestionePecoServiceClient proxy = new GestionePecoServiceClient();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_AGO, Utility.SOAPLogDirection.IN, numDomanda, guid, dati.PL_Funzione);
                    proxy.Aggiornamento_PECO_AGO(ProgrChiamante, AppChiamante, ref dati);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_AGO, method Aggiornamento_PECO_AGO | {0}", Utility.GetMessageFromException(ex));
                    stackTrace = ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC_AGO, method Aggiornamento_PECO_AGO | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_AGO, method Aggiornamento_PECO_AGO | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_AGO, method Aggiornamento_PECO_AGO: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, null, stackTrace);
                        errori = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                    }
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_AGO, Utility.SOAPLogDirection.OUT, numDomanda, guid, dati.PL_Funzione);
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void ValorizzaInputAggiornamentoPECO_AGO(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, Utility.TipoUnicarpe tipoUnicarpe, out csAggiornamentoPECO_AGO datiAgo)
        {
            datiAgo = new csAggiornamentoPECO_AGO();

            if (datiPensione == null)
                return;

            switch (tipoUnicarpe)
            {
                case Utility.TipoUnicarpe.Automatica:
                default:
                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                        datiAgo.PL_Funzione = "V";
                    else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                    {
                        if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                            datiAgo.PL_Funzione = "Q";
                        else
                            datiAgo.PL_Funzione = "P";
                    }
                    else if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                        datiAgo.PL_Funzione = "Z";
                    else
                        datiAgo.PL_Funzione = "R";
                    break;
                case Utility.TipoUnicarpe.Manuale:
                    datiAgo.PL_Funzione = "I";
                    break;
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            if (datiAnagrafici != null)
                datiAgo.PL_Codicefiscale = datiAnagrafici.CodiceFiscale;

            datiAgo.PL_Codicesede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0');
            datiAgo.PL_NumDomus = datiPensione.NDomus.ToString().PadLeft(13, '0');
            datiAgo.PL_Matricola = matricolaOperatore;

            datiAgo.PL_Tipoliquidazione = "0";
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
            if (datiIstruttoria != null && !string.IsNullOrEmpty(datiIstruttoria.ModalitaLiquidazione))
            {
                List<GestioneDecodifica.DecModalitaLiquidazione> lModalitaLiquidazione = null;
                GestioneDecodifica.GetElencoDecModalitaLiquidazione(out lModalitaLiquidazione);
                if (lModalitaLiquidazione != null && lModalitaLiquidazione.Count > 0)
                {
                    GestioneDecodifica.DecModalitaLiquidazione decLiq = lModalitaLiquidazione.Find(x => x.ValoreAggPeco == datiIstruttoria.ModalitaLiquidazione);
                    if (decLiq != null)
                    {
                        int tipoLiq = 0;
                        int.TryParse(decLiq.TraduzioneGp.ToString(), out tipoLiq);
                        if (decLiq.TraduzioneGp != '0' && tipoLiq == 0)
                            datiAgo.PL_Tipoliquidazione = "9";
                        else
                            datiAgo.PL_Tipoliquidazione = tipoLiq.ToString();
                    }
                }
            }

            datiAgo.PL_Data_Calc = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.Year.ToString().PadLeft(4, '0') +
                datiPensione.DataElaborazione.Value.Month.ToString().PadLeft(2, '0') +
                datiPensione.DataElaborazione.Value.Day.ToString().PadLeft(2, '0') : "00000000";

            string categoria = datiPensione.GetCodCategoria();
            datiAgo.PL_Categoria = categoria.PadLeft(4, '0').Substring(1, 3);
            datiAgo.PL_Certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : string.Empty;

            if (datiPensione.SiglaCategoria.StartsWith("S"))
            {
                GestioneAnagrafica.DatiAnagrafici anagraficaDC = null;
                BLCommon.GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out anagraficaDC);
                if (anagraficaDC != null)
                    datiAgo.PL_CF_dantecausa = anagraficaDC.CodiceFiscale;
            }
            else
                datiAgo.PL_CF_dantecausa = string.Empty;

            if ((Utility.IsDomandaENPALS(datiPensione.Gestione) || Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria)) && Utility.IsDomandaAPEPrecoci(datiPensione))
                datiAgo.PL_Campo36 = Utility.GetFiltroByCodTipoRichiesta("S2");
            else
                datiAgo.PL_Campo36 = datiPensione.GetFiltro();
        }

        #endregion AGO

        #region CI
        private static bool GetDatiPECO_CIbyNDomus(GestionePensione.DatiPensione datiPensione, bool isRiapertura, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Convenzioni_Internazionali();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.CI_Funzione = "G";
                else if (isRiapertura)
                    dati.CI_Funzione = "H";
                else
                    dati.CI_Funzione = "L";
                dati.CI_Numdomus = Convert.ToString(datiPensione.NDomus);
                AggiornamentoPECO_CI(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_CI"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_CI"], ref dati, datiPensione.NDomus.ToString(),
                    out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                //ENG - Per le nuove OpzioniDonna automatiche KWA, KYA, KUA deve essere presente una valorizzaziuone Unicarpe
                if (dati.CI_Return_Code != 0)
                {
                    dati = null;
                    if (datiPensione.GetFiltro() == "KWA" || datiPensione.GetFiltro() == "KYA" || datiPensione.GetFiltro() == "KUA")
                    {
                        errore = "ATTENZIONE per la tipologia di domanda selezionata è necessario effettuare la verifica del diritto sulla procedura Unicarpe. Se la domanda è da liquidare in modalità manuale occorre modificare la tipologia su Webdom";
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static bool GetDatiPECO_CIbyCodFisc(string codFisc, string numDomanda, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Convenzioni_Internazionali();
                dati.CI_Funzione = "C";
                dati.CI_Codfisc = codFisc;
                AggiornamentoPECO_CI(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_CI"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_CI"], ref dati, numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                if (dati.CI_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static void AggiornamentoPECO_CI(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            GestionePecoServiceClient proxy = new GestionePecoServiceClient();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Convenzioni_Internazionali, Utility.SOAPLogDirection.IN, numDomanda, guid, dati.CI_Funzione);
                    proxy.Aggiornamento_PECO_Convenzioni_Internazionali(ProgrChiamante, AppChiamante, ref dati);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_CI, method Aggiornamento_PECO_CI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC, method Aggiornamento_PECO_CI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_CI method Aggiornamento_PECO_CI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_CI method Aggiornamento_PECO_CI: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, null, stackTrace);
                        errori = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                    }
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Convenzioni_Internazionali, Utility.SOAPLogDirection.OUT, numDomanda, guid, dati.CI_Funzione);
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void ValorizzaInputAggiornamentoPECO_CI(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, Utility.TipoUnicarpe tipoUnicarpe, out csAggiornamentoPECO_Convenzioni_Internazionali datiCi)
        {
            datiCi = new csAggiornamentoPECO_Convenzioni_Internazionali();

            if (datiPensione == null)
                return;


            switch (tipoUnicarpe)
            {
                case Utility.TipoUnicarpe.Automatica:
                default:
                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                        datiCi.CI_Funzione = "V";
                    else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                        datiCi.CI_Funzione = "P";
                    else
                        datiCi.CI_Funzione = "R";
                    break;
                case Utility.TipoUnicarpe.Manuale:
                    datiCi.CI_Funzione = "I";
                    break;
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            if (datiAnagrafici != null)
                datiCi.CI_Codfisc = datiAnagrafici.CodiceFiscale;

            datiCi.CI_Codicesede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') +
                Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
            datiCi.CI_Numdomus = datiPensione.NDomus.ToString().PadLeft(13, '0');
            datiCi.CI_Matricola = matricolaOperatore;
            datiCi.CI_Tipoliquid = "4";

            datiCi.CI_Data_Calc = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.Year.ToString().PadLeft(4, '0') +
                datiPensione.DataElaborazione.Value.Month.ToString().PadLeft(2, '0') +
                datiPensione.DataElaborazione.Value.Day.ToString().PadLeft(2, '0') : "00000000";

            string categoria = datiPensione.GetCodCategoria();
            datiCi.CI_Categoria = categoria.PadLeft(4, '0').Substring(1, 3);
            datiCi.CI_Certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : string.Empty;
            datiCi.CI_CF_Dantecausa = string.Empty;
            datiCi.CI_Campo36 = datiPensione.GetFiltro();
        }

        #endregion CI

        #region AMG
        private static bool GetDatiPECO_AMGbyNDomus(GestionePensione.DatiPensione datiPensione, bool isRiapertura, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.A_Funzione = "G";
                else if (isRiapertura)
                    dati.A_Funzione = "H";
                else
                    dati.A_Funzione = "L";
                dati.A_Numdomus = Convert.ToString(datiPensione.NDomus);
                Aggiornamento_PECO_Fondi_AMG(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati,
                    datiPensione.NDomus.ToString(), out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                //ENG - Per le nuove OpzioniDonna automatiche KWA, KYA, KUA deve essere presente una valorizzaziuone Unicarpe
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione || (Utility.IsRicostituzione_MotiviContributivi(datiPensione) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione)))
                    if (dati != null && dati.A_Return_Code != 0)
                    {
                        if (datiPensione.GetFiltro() == "KWA" || datiPensione.GetFiltro() == "KYA" || datiPensione.GetFiltro() == "KUA")
                        {
                            errore = "ATTENZIONE per la tipologia di domanda selezionata è necessario effettuare la verifica del diritto sulla procedura Unicarpe. Se la domanda è da liquidare in modalità manuale occorre modificare la tipologia su Webdom";
                            return false;
                        }
                    }
                if (dati.A_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static bool GetDatiPECO_AMGbyCodFisc(string codFisc, string numDomanda, GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG();
                dati.A_Funzione = "C";
                dati.A_Codfisc = codFisc;
                Aggiornamento_PECO_Fondi_AMG(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati,
                    numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                //ENG - Per le nuove OpzioniDonna automatiche KWA, KYA, KUA deve essere presente una valorizzaziuone Unicarpe
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione || (Utility.IsRicostituzione_MotiviContributivi(datiPensione) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione)))
                    if (dati != null && dati.A_Return_Code != 0)
                    {
                        if (datiPensione.GetFiltro() == "KWA" || datiPensione.GetFiltro() == "KYA" || datiPensione.GetFiltro() == "KUA")
                        {
                            errore = "ATTENZIONE per la tipologia di domanda selezionata è necessario effettuare la verifica del diritto sulla procedura Unicarpe. Se la domanda è da liquidare in modalità manuale occorre modificare la tipologia su Webdom";
                            return false;
                        }
                    }
                if (dati.A_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static bool GetDatiPECO_AMGbyCodFisc(string codFisc, string numDomanda, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG();
                dati.A_Funzione = "C";
                dati.A_Codfisc = codFisc;
                Aggiornamento_PECO_Fondi_AMG(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati,
                    numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }

                if (dati.A_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static void Aggiornamento_PECO_Fondi_AMG(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_AMG dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG, Utility.SOAPLogDirection.IN, numDomanda, guid, dati.A_Funzione);
                    proxy.Aggiornamento_PECO_Fondi_AMG(ProgrChiamante, AppChiamante, ref dati);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, null, stackTrace);
                        errori = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                    }
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG, Utility.SOAPLogDirection.OUT, numDomanda, guid, dati.A_Funzione);
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void ValorizzaInputAggiornamentoPECO_Fondi_AMG(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, Utility.TipoUnicarpe tipoUnicarpe,
            out csAggiornamentoPECO_Fondi_AMG dati)
        {
            dati = new csAggiornamentoPECO_Fondi_AMG();

            if (datiPensione == null)
                return;

            switch (tipoUnicarpe)
            {
                case Utility.TipoUnicarpe.Automatica:
                default:
                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                        dati.A_Funzione = "V";
                    else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                        dati.A_Funzione = "P";
                    else
                        dati.A_Funzione = "R";
                    break;
                case Utility.TipoUnicarpe.Manuale:
                    dati.A_Funzione = "I";
                    break;
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            if (datiAnagrafici != null)
                dati.A_Codfisc = datiAnagrafici.CodiceFiscale;

            dati.A_Codicesede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') +
                Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
            dati.A_Numdomus = datiPensione.NDomus.ToString().PadLeft(13, '0');
            dati.A_Matricola = matricolaOperatore;
            GestioneIstruttoria.DatiIstruttoria istruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out istruttoria);

            List<GestioneDecodifica.ComunicazioneCampo3> elencoDecodificaComunicazioneCampo3 = null;
            GestioneDecodifica.GetComunicazioneCampo3(out elencoDecodificaComunicazioneCampo3);
            if (istruttoria != null && istruttoria.CodiceComunicazioneCampo3.HasValue && elencoDecodificaComunicazioneCampo3 != null && elencoDecodificaComunicazioneCampo3.Count > 0
                && elencoDecodificaComunicazioneCampo3.Exists(x => x.Id.Trim().ToUpperInvariant() == istruttoria.CodiceComunicazioneCampo3.ToString().ToUpperInvariant()))
                dati.A_Tipoliquid = "9";
            else
                dati.A_Tipoliquid = "4";
            dati.A_Data_Calc = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.Year.ToString().PadLeft(4, '0') +
                datiPensione.DataElaborazione.Value.Month.ToString().PadLeft(2, '0') +
                datiPensione.DataElaborazione.Value.Day.ToString().PadLeft(2, '0') : string.Empty;

            string categoria = datiPensione.GetCodCategoria();
            dati.A_Categoria = categoria.PadLeft(4, '0').Substring(1, 3);
            dati.A_Certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : string.Empty;
            dati.A_CA_Dantecausa = string.Empty;
            dati.A_Campo36 = datiPensione.GetFiltro();
        }

        private static bool GetDatiPECO_AMG_INPDAPbyNDomus(GestionePensione.DatiPensione datiPensione, bool isRiapertura, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG_INPDAP();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.A_Funzione = "G";
                else if (isRiapertura)
                    dati.A_Funzione = "H";
                else
                    dati.A_Funzione = "L";
                dati.A_Numdomus = Convert.ToString(datiPensione.NDomus);
                Aggiornamento_PECO_Fondi_AMG_INPDAP(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati,
                    datiPensione.NDomus.ToString(), out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                //ENG - Per le nuove OpzioniDonna automatiche KWA, KYA, KUA deve essere presente una valorizzaziuone Unicarpe
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                    if (dati != null && dati.A_Return_Code != 0)
                    {
                        if (datiPensione.GetFiltro() == "KWA" || datiPensione.GetFiltro() == "KYA" || datiPensione.GetFiltro() == "KUA")
                        {
                            errore = "ATTENZIONE per la tipologia di domanda selezionata è necessario effettuare la verifica del diritto sulla procedura Unicarpe. Se la domanda è da liquidare in modalità manuale occorre modificare la tipologia su Webdom";
                            return false;
                        }
                    }
                if (dati.A_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static bool GetDatiPECO_AMG_INPDAPbyCodFisc(string codFisc, string numDomanda, GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG_INPDAP();
                dati.A_Funzione = "C";
                dati.A_Codfisc = codFisc;
                Aggiornamento_PECO_Fondi_AMG_INPDAP(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati,
                    numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }
                //ENG - Per le nuove OpzioniDonna automatiche KWA, KYA, KUA deve essere presente una valorizzaziuone Unicarpe
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                    if (dati != null && dati.A_Return_Code != 0)
                    {
                        if (datiPensione.GetFiltro() == "KWA" || datiPensione.GetFiltro() == "KYA" || datiPensione.GetFiltro() == "KUA")
                        {
                            errore = "ATTENZIONE per la tipologia di domanda selezionata è necessario effettuare la verifica del diritto sulla procedura Unicarpe. Se la domanda è da liquidare in modalità manuale occorre modificare la tipologia su Webdom";
                            return false;
                        }
                    }
                if (dati.A_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static bool GetDatiPECO_AMG_INPDAPbyCodFisc(string codFisc, string numDomanda, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG_INPDAP();
                dati.A_Funzione = "C";
                dati.A_Codfisc = codFisc;
                Aggiornamento_PECO_Fondi_AMG_INPDAP(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati,
                    numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore))
                {
                    dati = null;
                    return false;
                }

                if (dati.A_Return_Code != 0)
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errore = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                string messaggio = Utility.GetMessageFromException(ex);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        private static void Aggiornamento_PECO_Fondi_AMG_INPDAP(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG_INPDAP, Utility.SOAPLogDirection.IN, numDomanda, guid, dati.A_Funzione);
                    proxy.Aggiornamento_PECO_Fondi_AMG_INPDAP(ProgrChiamante, AppChiamante, ref dati);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG_INPDAP | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG_INPDAP | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG_INPDAP | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_AMG, method Aggiornamento_PECO_Fondi_AMG_INPDAP: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, null, stackTrace);
                        errori = "Errore tecnico durante il recupero delle informazioni relative alla misura e al diritto";
                    }
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG_INPDAP, Utility.SOAPLogDirection.OUT, numDomanda, guid, dati.A_Funzione);
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void ValorizzaInputAggiornamentoPECO_Fondi_AMG_INPDAP(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, Utility.TipoUnicarpe tipoUnicarpe,
            out csAggiornamentoPECO_Fondi_AMG_INPDAP dati)
        {
            dati = new csAggiornamentoPECO_Fondi_AMG_INPDAP();

            if (datiPensione == null)
                return;

            switch (tipoUnicarpe)
            {
                case Utility.TipoUnicarpe.Automatica:
                default:
                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                        dati.A_Funzione = "V";
                    else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                        dati.A_Funzione = "P";
                    else
                        dati.A_Funzione = "R";
                    break;
                case Utility.TipoUnicarpe.Manuale:
                    dati.A_Funzione = "I";
                    break;
            }

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            if (datiAnagrafici != null)
                dati.A_Codfisc = datiAnagrafici.CodiceFiscale;

            dati.A_Codicesede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') +
                Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
            dati.A_Numdomus = datiPensione.NDomus.ToString().PadLeft(13, '0');
            dati.A_Matricola = matricolaOperatore;
            GestioneIstruttoria.DatiIstruttoria istruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out istruttoria);

            List<GestioneDecodifica.ComunicazioneCampo3> elencoDecodificaComunicazioneCampo3 = null;
            GestioneDecodifica.GetComunicazioneCampo3(out elencoDecodificaComunicazioneCampo3);

            if (istruttoria != null && istruttoria.CodiceComunicazioneCampo3.HasValue && elencoDecodificaComunicazioneCampo3 != null && elencoDecodificaComunicazioneCampo3.Count > 0
                && elencoDecodificaComunicazioneCampo3.Exists(x => x.Id.Trim().ToUpperInvariant() == istruttoria.CodiceComunicazioneCampo3.ToString().ToUpperInvariant()))
                dati.A_Tipoliquid = "9";
            else
                dati.A_Tipoliquid = "4";
            dati.A_Data_Calc = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.Year.ToString().PadLeft(4, '0') +
                datiPensione.DataElaborazione.Value.Month.ToString().PadLeft(2, '0') +
                datiPensione.DataElaborazione.Value.Day.ToString().PadLeft(2, '0') : string.Empty;

            string categoria = datiPensione.GetCodCategoria();
            dati.A_Categoria = categoria.PadLeft(4, '0').Substring(1, 3);
            dati.A_Certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : string.Empty;
            dati.A_CA_Dantecausa = string.Empty;
            dati.A_Campo36 = datiPensione.GetFiltro();
        }
        #endregion AMG

        #endregion private method

    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          