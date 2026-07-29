using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazioneFs.ServiceReferences.AggPec;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Configuration;
using System.Reflection;
using System.Globalization;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneAggiornamentoPECO
    {
        #region GetDatiPECO

        internal static bool GetDatiPECO_FSbyNumeroDomanda(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_Speciali dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_Speciali();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.F_Funzione = "G";
                else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                    dati.F_Funzione = "H";
                else
                    dati.F_Funzione = "L";
                dati.F_Numdomus = Convert.ToString(datiPensione.NDomus);
                AggiornamentoPECO_FS(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ref dati,
                    datiPensione.NDomus.ToString(), out errore);
                if (!String.IsNullOrEmpty(errore) || dati.F_Return_Code != 0)
                {
                    if (String.IsNullOrEmpty(errore))
                        errore = "Codice di ritorno della posizione richiesta (diverso da 0): " + dati.F_Return_Code.ToString();
                    dati = null;
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

        internal static bool GetDatiCertificazioniPECO_FSbyCodFisc(string codFisc, string numDomanda, ref csAggiornamentoPECO_Fondi_Speciali dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_Speciali();
                dati.F_Funzione = "C";
                dati.F_Codfisc = codFisc;
                AggiornamentoPECO_FS(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_FS"], ref dati,
                    numDomanda, out errore);
                if (!String.IsNullOrEmpty(errore) || dati.F_Return_Code != 0)
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

        internal static bool GetDatiPECO_AMGbyNDomus(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.A_Funzione = "G";
                else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
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

        internal static bool GetDatiPECO_AMG_INPDAPbyNDomus(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG_INPDAP();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.A_Funzione = "G";
                else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
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

        #endregion GetDatiPECO

        #region Dati Contrib

        public static void IsQuotaDPresenteFromFelpeAMG(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, out List<KeyValuePair<long?, bool>> listaIsQuotaDPresente)
        {
            listaIsQuotaDPresente = null;

            if ((tipoFondo.HasValue && new List<Utility.TipoFondo> { Utility.TipoFondo.FS, Utility.TipoFondo.PT }.Contains(tipoFondo.Value)) &&
                Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                string errori = string.Empty;
                ServiceReferences.AggPec.csAggiornamentoPECO_Fondi_AMG dati = null;
                GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errori);
                if (!String.IsNullOrEmpty(errori))
                    throw new INPS.DNA.DnaValidationException(errori);

                List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
                GestioneAggiornamentoPECO.DatiContributivi datiCalcoloContributivo = null;
                Entity.DatiCalcolo datiCalcoloForDatiFondo = null;
                GestioneContrib.CrossDataRecipient crossDataRecipient;
                GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out listaDatiServizioUtile, out datiCalcoloContributivo, out datiCalcoloForDatiFondo, out crossDataRecipient);
                if (datiCalcoloForDatiFondo != null)
                {
                    listaIsQuotaDPresente = new List<KeyValuePair<long?, bool>>();
                    listaIsQuotaDPresente.Add(new KeyValuePair<long?, bool>(null, datiCalcoloForDatiFondo.IsQuotaDL214Presente()));
                }
            }
        }

        public static void IsQuotaDPresenteFromFelpeAMG_INPDAP(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, out List<KeyValuePair<long?, bool>> listaIsQuotaDPresente)
        {
            listaIsQuotaDPresente = null;

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                string errori = string.Empty;
                List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtile = null;
                GestioneAggiornamentoPECO.DatiContributivi datiCalcoloContributivo = null;
                Entity.DatiCalcolo datiCalcoloForDatiFondo = null;
                GestioneContrib.CrossDataRecipient crossDataRecipient;
                if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
                {
                    csAggiornamentoPECO_Fondi_AMG_INPDAP dati = null;
                    GestioneAggiornamentoPECO.GetDatiPECO_AMG_INPDAPbyNDomus(datiPensione, ref dati, out errori);
                    if (!String.IsNullOrEmpty(errori))
                        throw new INPS.DNA.DnaValidationException(errori);
                    GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out listaDatiServizioUtile, out datiCalcoloContributivo, out datiCalcoloForDatiFondo, out crossDataRecipient);
                }
                else
                {
                    csAggiornamentoPECO_Fondi_AMG dati = null;
                    GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errori);
                    if (!String.IsNullOrEmpty(errori))
                        throw new INPS.DNA.DnaValidationException(errori);
                    GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out listaDatiServizioUtile, out datiCalcoloContributivo, out datiCalcoloForDatiFondo, out crossDataRecipient);
                }


                if (datiCalcoloForDatiFondo != null)
                {
                    listaIsQuotaDPresente = new List<KeyValuePair<long?, bool>>();
                    listaIsQuotaDPresente.Add(new KeyValuePair<long?, bool>(null, datiCalcoloForDatiFondo.IsQuotaDL214Presente()));
                }
            }
        }

        internal static void GetDatiTotali(GestionePensione.DatiPensione datiPensione, out csAggiornamentoPECO_Fondi_Speciali dati, out string messaggioVideo)
        {
            messaggioVideo = "";
            dati = null;
            GetDatiPECO_FSbyNumeroDomanda(datiPensione, ref dati, out messaggioVideo);
            if (dati == null || !String.IsNullOrEmpty(messaggioVideo))
                throw new INPS.DNA.DnaApplicationException(messaggioVideo);
            return;
        }

        internal static void GetDatiTotaliAMG(GestionePensione.DatiPensione datiPensione, out csAggiornamentoPECO_Fondi_AMG dati, out string messaggioVideo)
        {
            messaggioVideo = "";
            dati = null;
            GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out messaggioVideo);
            if (dati == null || !String.IsNullOrEmpty(messaggioVideo))
                throw new INPS.DNA.DnaApplicationException(messaggioVideo);
            return;
        }

        internal static void GetDatiTotaliAMG_INPDAP(GestionePensione.DatiPensione datiPensione, out csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out string messaggioVideo)
        {
            messaggioVideo = "";
            dati = null;
            GetDatiPECO_AMG_INPDAPbyNDomus(datiPensione, ref dati, out messaggioVideo);
            if (dati == null || !String.IsNullOrEmpty(messaggioVideo))
                throw new INPS.DNA.DnaApplicationException(messaggioVideo);
            return;
        }

        internal static void RecuperaDatiTotaliAggPeco(csAggiornamentoPECO_Fondi_Speciali dati, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoFondo? tipoFondo, GestioneFondo.DatiFondo datiFondo,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, bool isRiaperturaDomanda, out DatiTotaliAggPeco datiAggPeco,
            ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            datiAggPeco = new DatiTotaliAggPeco();
            messaggioVideo = string.Empty;
            DatiParzialiAggPeco datiParziali = null;
            string errori = string.Empty;
            List<GestioneContrib.DatiServizioUtile> listaDatiServizioUtile = null;

            RecuperaDatiParzialiAggPeco(datiPensione, tipoFondo, dati, out datiParziali, out errori);
            if (!String.IsNullOrEmpty(errori))
            {
                datiAggPeco = null;
                return;
            }
            datiAggPeco.DatiParziali = datiParziali;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPecoEL_TT(dati, datiPensione, ref datiAggPeco, out messaggioVideo);
                        break;
                    case Utility.TipoFondo.ET:
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPecoET(dati, datiPensione, ref datiAggPeco, ref crossDataRecipient, out messaggioVideo);
                        if (crossDataRecipient != null)
                            listaDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPecoVL(dati, datiPensione, ref datiAggPeco, out messaggioVideo);
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPecoDZ(dati, ref datiAggPeco, ref crossDataRecipient, out messaggioVideo);
                        if (crossDataRecipient != null)
                            listaDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                        break;
                }
            }
            ImpostaDatiControllo(tipoFondo, datiAggPeco, datiPensione, datiDanteCausa, datiMaggiorazioniBenefici, listaDatiServizioUtile, null, datiFondo, isRiaperturaDomanda, out messaggioVideo);
        }

        internal static void RecuperaDatiTotaliAMG(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoFondo? tipoFondo, GestioneFondo.DatiFondo datiFondo,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, bool isRiaperturaDomanda, out DatiTotaliAggPeco datiAggPeco,
            ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            datiAggPeco = new DatiTotaliAggPeco();
            messaggioVideo = string.Empty;
            DatiParzialiAggPeco datiParziali = null;
            string errori = string.Empty;
            List<GestioneContrib.DatiServizioUtile> listaDatiServizioUtile = null;
            List<Entity.DatiCalcolo707.DatiServizioUtile707> listaDatiServizioUtile707 = null;

            RecuperaDatiParzialiAggPeco(dati, out datiParziali, out errori);
            if (!String.IsNullOrEmpty(errori))
            {
                datiAggPeco = null;
                return;
            }
            datiAggPeco.DatiParziali = datiParziali;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPecoFS_PT(dati, datiPensione, ref datiAggPeco, ref crossDataRecipient, out messaggioVideo);
                        if (crossDataRecipient != null)
                        {
                            listaDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                            crossDataRecipient.IdPensione = datiPensione.Id;
                        }

                        break;
                }
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPecoINPDAP_AMG(dati, datiPensione, ref datiAggPeco, ref crossDataRecipient, out messaggioVideo);
                if (crossDataRecipient != null)
                {
                    listaDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                    crossDataRecipient.IdPensione = datiPensione.Id;
                }
            }

            ImpostaDatiControllo(tipoFondo, datiAggPeco, datiPensione, datiDanteCausa, datiMaggiorazioniBenefici, listaDatiServizioUtile, listaDatiServizioUtile707, datiFondo, isRiaperturaDomanda, out messaggioVideo);
        }

        internal static void RecuperaDatiTotaliAMG_INPDAP(csAggiornamentoPECO_Fondi_AMG_INPDAP dati, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoFondo? tipoFondo, GestioneFondo.DatiFondo datiFondo,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, bool isRiaperturaDomanda, out DatiTotaliAggPeco datiAggPeco,
            ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            datiAggPeco = new DatiTotaliAggPeco();
            messaggioVideo = string.Empty;
            DatiParzialiAggPeco datiParziali = null;
            string errori = string.Empty;
            List<GestioneContrib.DatiServizioUtile> listaDatiServizioUtile = null;
            List<Entity.DatiCalcolo707.DatiServizioUtile707> listaDatiServizioUtile707 = null;

            RecuperaDatiParzialiAggPecoINPDAP(dati, out datiParziali, out errori);
            if (!String.IsNullOrEmpty(errori))
            {
                datiAggPeco = null;
                return;
            }
            datiAggPeco.DatiParziali = datiParziali;

            GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPecoINPDAP(dati, datiPensione, ref datiAggPeco, ref crossDataRecipient, out messaggioVideo);
            if (crossDataRecipient != null)
            {
                listaDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                crossDataRecipient.IdPensione = datiPensione.Id;
            }

            ImpostaDatiControllo(tipoFondo, datiAggPeco, datiPensione, datiDanteCausa, datiMaggiorazioniBenefici, listaDatiServizioUtile, listaDatiServizioUtile707, datiFondo, isRiaperturaDomanda, out messaggioVideo);
        }

        internal static void RecuperaDatiTotaliAggPecoEL_TT(csAggiornamentoPECO_Fondi_Speciali dati, GestionePensione.DatiPensione datiPensione, ref DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (dati.aRETRIBUTIVE_FS != null && dati.aRETRIBUTIVE_FS.Length > 0)
            {
                decimal rmsQuotaC = 0M;
                foreach (F_RETRIBUTIVE retr in dati.aRETRIBUTIVE_FS)
                {
                    if (!string.IsNullOrEmpty(retr.F_CodQuota.Trim()))
                    {
                        if (datiAggPeco.Retribuzione == null)
                            datiAggPeco.Retribuzione = new DatiRetributivi();

                        switch (retr.F_CodQuota.Trim().ToUpperInvariant())
                        {
                            case "A":
                                datiAggPeco.Retribuzione.RmsQuotaA = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneA = Convert.ToInt32(retr.F_Anzconr);
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneA = null;
                                datiAggPeco.Retribuzione.QuotaA707 = retr.F_Anzcon707 != 0 ? Convert.ToInt16(retr.F_Anzcon707) : (short?)null;
                                break;
                            case "B":
                                datiAggPeco.Retribuzione.RmsQuotaB = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneB = Convert.ToInt32(retr.F_Anzconr);
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneB = null;
                                datiAggPeco.Retribuzione.QuotaB707 = retr.F_Anzcon707 != 0 ? Convert.ToInt16(retr.F_Anzcon707) : (short?)null;
                                break;
                            case "C":
                                rmsQuotaC = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneC = Convert.ToInt32(retr.F_Anzconr);
                                datiAggPeco.Retribuzione.QuotaC707 = retr.F_Anzcon707 != 0 ? Convert.ToInt16(retr.F_Anzcon707) : (short?)null;
                                break;
                            case "D":
                                datiAggPeco.Retribuzione.RmsQuotaD = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneD = Convert.ToInt32(retr.F_Anzconr);
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneD = null;
                                datiAggPeco.Retribuzione.QuotaD707 = retr.F_Anzcon707 != 0 ? Convert.ToInt16(retr.F_Anzcon707) : (short?)null;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (datiAggPeco.Retribuzione != null && datiAggPeco.Retribuzione.RmsQuotaB == 0M && rmsQuotaC != 0M)
                    datiAggPeco.Retribuzione.RmsQuotaB = rmsQuotaC;

                if (datiAggPeco.Retribuzione != null && datiAggPeco.Retribuzione.RmsQuotaB == 0 && datiAggPeco.Retribuzione.SettimaneC.GetValueOrDefault() == 0)
                    datiAggPeco.Retribuzione.SettimaneC = null;

                if (!Utility.IsDoubleEquals(dati.F_Retrpond, 0) || !Utility.IsDoubleEquals(dati.F_Retrbiennio, 0) || !Utility.IsDoubleEquals(dati.F_Retrultanno, 0))
                {
                    if (datiAggPeco.Retribuzione != null)
                    {
                        datiAggPeco.Retribuzione.RetribuzionePonderataAnnua = Convert.ToDecimal(dati.F_Retrpond);
                        datiAggPeco.Retribuzione.RetribuzioneBiennio = Convert.ToDecimal(dati.F_Retrbiennio);
                        datiAggPeco.Retribuzione.RetribuzioneUltimoAnnoQuotaA = Convert.ToDecimal(dati.F_Retrultanno);

                    }
                    else // non possono essere presenti questi dati senza i dati retributivi
                        return;
                }
                if (dati.F_Retrpond707 > 0)
                    datiAggPeco.Retribuzione.RetribuzionePonderataAGO707 = Convert.ToDecimal(dati.F_Retrpond707);
            }

            if (dati.aCONTRIBUTIVE_FS != null && dati.aCONTRIBUTIVE_FS.Length > 0)
            {
                bool IsContribNull = true;
                foreach (F_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_FS)
                {
                    if (!Utility.IsDoubleEquals(contr.F_Contrib, 0) || !Utility.IsDoubleEquals(contr.F_Montante, 0) || contr.F_SetteSCL != 0)
                    {
                        IsContribNull = false;
                        if (datiAggPeco.Contribuzione == null)
                            datiAggPeco.Contribuzione = new DatiContributivi();
                        switch (contr.F_CodQuota_Contr.Trim().ToUpperInvariant())
                        {
                            case "C":
                                datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.F_Contrib);
                                datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.F_Montante);
                                datiAggPeco.Contribuzione.Settimane = contr.F_Anzconc;
                                break;
                            case "D":
                                datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.F_Contrib);
                                datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.F_Montante);
                                datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = contr.F_Anzconc;
                                break;
                        }
                    }
                }

                if (IsContribNull)
                    datiAggPeco.Contribuzione = null;

            }
        }

        internal static void RecuperaDatiTotaliAggPecoET(csAggiornamentoPECO_Fondi_Speciali dati, GestionePensione.DatiPensione datiPensione, ref DatiTotaliAggPeco datiAggPeco, ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (dati.aRETRIBUTIVE_FS != null && dati.aRETRIBUTIVE_FS.Length > 0)
            {
                GestioneContrib.DatiServizioUtile ServizioUtile = null;
                foreach (F_RETRIBUTIVE retr in dati.aRETRIBUTIVE_FS)
                {
                    if (!string.IsNullOrEmpty(retr.F_CodQuota.Trim()))
                    {
                        if (datiAggPeco.Retribuzione == null)
                            datiAggPeco.Retribuzione = new DatiRetributivi();

                        switch (retr.F_CodQuota.Trim().ToUpperInvariant())
                        {
                            case "AA":
                                datiAggPeco.Retribuzione.RmsQuotaA = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneA = Convert.ToInt32(retr.F_Anzconr);
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneA = null;
                                if (!string.IsNullOrEmpty(retr.F_CodGest_Retr) && retr.F_CodGest_Retr.Trim() == "8" && retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaA707 = (short)retr.F_Anzcon707;
                                break;
                            case "BA":
                                datiAggPeco.Retribuzione.RmsQuotaB = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneB = Convert.ToInt32(retr.F_Anzconr);
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneB = null;
                                if (!string.IsNullOrEmpty(retr.F_CodGest_Retr) && retr.F_CodGest_Retr.Trim() == "8" && retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaB707 = (short)retr.F_Anzcon707;
                                break;
                            case "A":
                                if (!Utility.IsDoubleEquals(retr.F_Rms, 0.0) || retr.F_Anzconr != 0)
                                {
                                    if (crossDataRecipient == null)
                                        crossDataRecipient = new GestioneContrib.CrossDataRecipient();
                                    if (crossDataRecipient.lDatiServizioUtile == null)
                                        crossDataRecipient.lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();

                                    ServizioUtile = new GestioneContrib.DatiServizioUtile();
                                    ServizioUtile.ServizioUtileAA = Convert.ToInt16(retr.F_Anzconr);
                                    ServizioUtile.RetribuzionePensionabile = Convert.ToDecimal(retr.F_Rms);
                                    ServizioUtile.Quota = retr.F_CodQuota.Trim().ToUpperInvariant();

                                    crossDataRecipient.lDatiServizioUtile.Add(ServizioUtile);
                                }

                                if (!string.IsNullOrEmpty(retr.F_CodGest_Retr) && retr.F_CodGest_Retr.Trim() == "8" && retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaA707AA = (byte)retr.F_Anzcon707;
                                break;
                            case "B":
                                if (!Utility.IsDoubleEquals(retr.F_Rms, 0.0) || retr.F_Anzconr != 0)
                                {
                                    if (crossDataRecipient == null)
                                        crossDataRecipient = new GestioneContrib.CrossDataRecipient();
                                    if (crossDataRecipient.lDatiServizioUtile == null)
                                        crossDataRecipient.lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();

                                    ServizioUtile = new GestioneContrib.DatiServizioUtile();
                                    ServizioUtile.ServizioUtileAA = Convert.ToInt16(retr.F_Anzconr);
                                    ServizioUtile.RetribuzionePensionabile = Convert.ToDecimal(retr.F_Rms);
                                    ServizioUtile.Quota = retr.F_CodQuota.Trim().ToUpperInvariant();

                                    crossDataRecipient.lDatiServizioUtile.Add(ServizioUtile);
                                }

                                if (!string.IsNullOrEmpty(retr.F_CodGest_Retr) && retr.F_CodGest_Retr.Trim() == "8" && retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaB707AA = (byte)retr.F_Anzcon707;
                                break;
                            case "C":
                                if (!Utility.IsDoubleEquals(retr.F_Rms, 0.0) || retr.F_Anzconr != 0)
                                {
                                    if (crossDataRecipient == null)
                                        crossDataRecipient = new GestioneContrib.CrossDataRecipient();
                                    if (crossDataRecipient.lDatiServizioUtile == null)
                                        crossDataRecipient.lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();

                                    ServizioUtile = new GestioneContrib.DatiServizioUtile();
                                    ServizioUtile.ServizioUtileAA = Convert.ToInt16(retr.F_Anzconr);
                                    ServizioUtile.RetribuzionePensionabile = Convert.ToDecimal(retr.F_Rms);
                                    ServizioUtile.Quota = retr.F_CodQuota.Trim().ToUpperInvariant();

                                    crossDataRecipient.lDatiServizioUtile.Add(ServizioUtile);
                                }

                                if (!string.IsNullOrEmpty(retr.F_CodGest_Retr) && retr.F_CodGest_Retr.Trim() == "8" && retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaC707AA = (byte)retr.F_Anzcon707;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (crossDataRecipient.lDatiServizioUtile == null || crossDataRecipient.lDatiServizioUtile.Count == 0)
                    crossDataRecipient.lDatiServizioUtile = null;
                else
                {
                    GestioneContrib.DatiServizioUtile elementoC = crossDataRecipient.lDatiServizioUtile.FirstOrDefault(x => x.Quota == "C" &&
                        x.RetribuzionePensionabile.HasValue && x.RetribuzionePensionabile.Value != 0M);
                    if (elementoC != null)
                    {
                        GestioneContrib.DatiServizioUtile elementoB = crossDataRecipient.lDatiServizioUtile.FirstOrDefault(x => x.Quota == "B" &&
                            (!x.RetribuzionePensionabile.HasValue || x.RetribuzionePensionabile.Value == 0M));
                        if (elementoB != null)
                            elementoB.RetribuzionePensionabile = elementoC.RetribuzionePensionabile;
                        else if (crossDataRecipient.lDatiServizioUtile.FirstOrDefault(x => x.Quota == "B") == null)
                        {
                            ServizioUtile = new GestioneContrib.DatiServizioUtile();
                            ServizioUtile.RetribuzionePensionabile = elementoC.RetribuzionePensionabile;
                            ServizioUtile.Quota = "B";
                            crossDataRecipient.lDatiServizioUtile.Add(ServizioUtile);
                        }
                    }
                }

                if (!Utility.IsDoubleEquals(dati.F_Retrpond, 0) || !Utility.IsDoubleEquals(dati.F_Retrbiennio, 0) || !Utility.IsDoubleEquals(dati.F_Retrultanno, 0))
                {
                    if (datiAggPeco.Retribuzione != null)
                    {
                        datiAggPeco.Retribuzione.RetribuzionePonderataAnnua = Convert.ToDecimal(dati.F_Retrpond);
                        datiAggPeco.Retribuzione.RetribuzioneBiennio = Convert.ToDecimal(dati.F_Retrbiennio);
                        datiAggPeco.Retribuzione.RetribuzioneUltimoAnnoQuotaA = Convert.ToDecimal(dati.F_Retrultanno);
                    }
                    else // non possono essere presenti questi dati senza i dati retributivi
                        return;
                }

                if (dati.F_Retrpond707 > 0)
                    datiAggPeco.Retribuzione.RetribuzionePonderataAGO707 = Convert.ToDecimal(dati.F_Retrpond707);
            }

            if (dati.aCONTRIBUTIVE_FS != null && dati.aCONTRIBUTIVE_FS.Length > 0)
            {
                bool IsContribNull = true;
                foreach (F_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_FS)
                {
                    if (!Utility.IsDoubleEquals(contr.F_Contrib, 0.0) || Utility.IsDoubleEquals(contr.F_Montante, 0.0) || contr.F_SetteSCL != 0)
                    {
                        IsContribNull = false;
                        if (datiAggPeco.Contribuzione == null)
                            datiAggPeco.Contribuzione = new DatiContributivi();
                        switch (contr.F_CodQuota_Contr.Trim().ToUpperInvariant())
                        {
                            case "C":
                                datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.F_Contrib);
                                datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.F_Montante);
                                datiAggPeco.Contribuzione.Settimane = contr.F_Anzconc;
                                break;
                            case "D":
                                datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.F_Contrib);
                                datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.F_Montante);
                                datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = contr.F_Anzconc;
                                break;
                        }
                    }
                }

                if (IsContribNull)
                    datiAggPeco.Contribuzione = null;

            }
        }

        internal static void RecuperaDatiTotaliAggPecoVL(csAggiornamentoPECO_Fondi_Speciali dati, GestionePensione.DatiPensione datiPensione, ref DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            datiAggPeco.Retribuzione = null;
            if (dati.aRETRIBUTIVE_FS != null && dati.aRETRIBUTIVE_FS.Length > 0)
            {
                decimal rmsQuotaA2 = 0M;
                decimal rmsQuotaB2 = 0M;
                decimal rmsQuotaB3 = 0M;

                foreach (F_RETRIBUTIVE retr in dati.aRETRIBUTIVE_FS)
                {
                    if (!string.IsNullOrEmpty(retr.F_CodQuota.Trim()))
                    {
                        if (datiAggPeco.Retribuzione == null)
                            datiAggPeco.Retribuzione = new DatiRetributivi();

                        switch (retr.F_CodQuota.Trim().ToUpperInvariant())
                        {
                            case "A1":
                                datiAggPeco.Retribuzione.RmsQuotaA = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneA = retr.F_Anzconr;
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneA = null;
                                if (retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaA707 = Convert.ToInt16(retr.F_Anzcon707);
                                break;
                            case "A2":
                                rmsQuotaA2 = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneA2 = retr.F_Anzconr;
                                if (retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaA2707 = Convert.ToInt16(retr.F_Anzcon707);
                                break;
                            case "B1":
                                datiAggPeco.Retribuzione.RmsQuotaB = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneB = retr.F_Anzconr;
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneB = null;
                                if (retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaB707 = Convert.ToInt16(retr.F_Anzcon707);
                                break;
                            case "B2":
                                rmsQuotaB2 = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneC = retr.F_Anzconr;
                                if (retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaC707 = Convert.ToInt16(retr.F_Anzcon707);
                                break;
                            case "B3":
                                rmsQuotaB3 = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneC2 = retr.F_Anzconr;
                                if (retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaC2707 = Convert.ToInt16(retr.F_Anzcon707);
                                break;
                            case "C":
                                datiAggPeco.Retribuzione.RmsQuotaD = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneD = retr.F_Anzconr;
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneD = null;
                                if (retr.F_Anzcon707 != 0)
                                    datiAggPeco.Retribuzione.QuotaD707 = Convert.ToInt16(retr.F_Anzcon707);
                                break;
                            default:
                                break;
                        }
                    }
                }



                if (datiAggPeco.Retribuzione != null && datiAggPeco.Retribuzione.RmsQuotaA == 0M && rmsQuotaA2 != 0M)
                    datiAggPeco.Retribuzione.RmsQuotaA = rmsQuotaA2;

                if (datiAggPeco.Retribuzione != null && datiAggPeco.Retribuzione.RmsQuotaB == 0M)
                {
                    if (rmsQuotaB2 != 0M)
                        datiAggPeco.Retribuzione.RmsQuotaB = rmsQuotaB2;
                    else if (rmsQuotaB3 != 0M)
                        datiAggPeco.Retribuzione.RmsQuotaB = rmsQuotaB3;
                }

                if (datiAggPeco.Retribuzione != null && datiAggPeco.Retribuzione.RmsQuotaB == 0M && datiAggPeco.Retribuzione.SettimaneC.GetValueOrDefault() == 0)
                    datiAggPeco.Retribuzione.SettimaneC = null;

                if (dati.F_Retrpond707 > 0)
                    datiAggPeco.Retribuzione.RetribuzionePonderataAGO707 = Convert.ToDecimal(dati.F_Retrpond707);
            }

            datiAggPeco.Contribuzione = null;
            if (dati.aCONTRIBUTIVE_FS != null && dati.aCONTRIBUTIVE_FS.Length > 0)  // contributivo puro
            {
                if (string.IsNullOrEmpty(dati.aCONTRIBUTIVE_FS[0].F_CodQuota_Contr.Trim()))
                {
                    if (!Utility.IsDoubleEquals(dati.aCONTRIBUTIVE_FS[0].F_Contrib, 0.0) || !Utility.IsDoubleEquals(dati.aCONTRIBUTIVE_FS[0].F_Montante, 0.0))
                    {
                        if (datiAggPeco.Contribuzione == null)
                            datiAggPeco.Contribuzione = new DatiContributivi();

                        datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(dati.aCONTRIBUTIVE_FS[0].F_Contrib);
                        datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(dati.aCONTRIBUTIVE_FS[0].F_Montante);
                        datiAggPeco.Contribuzione.Settimane = dati.aCONTRIBUTIVE_FS[0].F_Anzconc;
                    }
                }
                else  // contributivo misto
                {

                    decimal importoContrTotA = 0;
                    decimal importoContrTotX = 0;
                    foreach (F_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_FS)
                    {
                        if (!string.IsNullOrEmpty(contr.F_CodQuota_Contr.Trim()))
                        {
                            if (datiAggPeco.Contribuzione == null)
                                datiAggPeco.Contribuzione = new DatiContributivi();

                            switch (contr.F_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "A":
                                    importoContrTotA = Convert.ToDecimal(contr.F_Contrib);
                                    datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.F_Montante);
                                    datiAggPeco.Contribuzione.AnzianitaPost0697AA = Convert.ToInt16(contr.F_AnzAA);
                                    datiAggPeco.Contribuzione.AnzianitaPost0697MM = Convert.ToInt16(contr.F_AnzMM);
                                    datiAggPeco.Contribuzione.AnzianitaPost0697GG = Convert.ToInt16(contr.F_AnzGG);
                                    break;

                                case "X":
                                    //datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.F_Contrib);
                                    importoContrTotX = Convert.ToDecimal(contr.F_Contrib);
                                    datiAggPeco.Contribuzione.MontanteAnte0697 = Convert.ToDecimal(contr.F_Montante);
                                    datiAggPeco.Contribuzione.AnzianitaAnte0697AA = Convert.ToInt16(contr.F_AnzAA);
                                    datiAggPeco.Contribuzione.AnzianitaAnte0697MM = Convert.ToInt16(contr.F_AnzMM);
                                    datiAggPeco.Contribuzione.AnzianitaAnte0697GG = Convert.ToInt16(contr.F_AnzGG);
                                    break;
                                case "D":
                                    datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.F_Contrib);
                                    datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.F_Montante);
                                    datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = contr.F_Anzconc;
                                    break;
                                case "C":
                                    datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.F_Contrib);
                                    datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.F_Montante);
                                    break;
                            }
                        }
                    }
                    if (importoContrTotA != 0 || importoContrTotX != 0)
                        datiAggPeco.Contribuzione.ImportoContributivoTotale = importoContrTotA + importoContrTotX;
                }
            }
        }

        internal static void RecuperaDatiTotaliAggPecoFS_PT(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione, ref DatiTotaliAggPeco datiAggPeco,
            ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (crossDataRecipient == null)
                crossDataRecipient = new GestioneContrib.CrossDataRecipient();

            crossDataRecipient.PensioneAnnuaLorda = Convert.ToDecimal(dati.A_Pal);
            string strApp = dati.A_Anzcont.ToString().PadLeft(6, '0');
            crossDataRecipient.ServizioUtileDirittoAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
            crossDataRecipient.ServizioUtileDirittoMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
            crossDataRecipient.ServizioUtileDirittoGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
            crossDataRecipient.CoefficienteTrasformazione = Convert.ToDecimal(dati.A_CoefTrasf);

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                GestioneContrib.DatiServizioUtile servizioUtile = null;

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        if (crossDataRecipient.lDatiServizioUtile == null)
                            crossDataRecipient.lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();

                        // Quota A - al 92
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "A";
                        }
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "A";
                        }
                        // Quota B1 - 1993 -1994
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1"))
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B1";
                        }
                        // Quota B2 - 1995
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2"))
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Quota = "B2";
                        }
                        // Quota B3 - 1996 - 1997
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3"))
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Quota = "B3";
                        }
                        // Quota B4 - post 1997
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4"))
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Quota = "B4";
                        }

                        if (servizioUtile != null)
                            crossDataRecipient.lDatiServizioUtile.Add(servizioUtile);
                    }
                }
            }

            if (crossDataRecipient.lDatiServizioUtile == null || crossDataRecipient.lDatiServizioUtile.Count == 0)
                crossDataRecipient.lDatiServizioUtile = null;

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                bool IsContribNull = true;
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {
                            IsContribNull = false;
                            if (datiAggPeco.Contribuzione == null)
                                datiAggPeco.Contribuzione = new DatiContributivi();
                            strApp = string.Empty;
                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "C":
                                    datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiAggPeco.Contribuzione.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    datiAggPeco.Contribuzione.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                                case "D":
                                    datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    datiAggPeco.Contribuzione.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                            }
                        }
                    }
                }

                if (IsContribNull)
                    datiAggPeco.Contribuzione = null;
            }
        }

        internal static void RecuperaDatiTotaliAggPecoINPDAP(csAggiornamentoPECO_Fondi_AMG_INPDAP dati, GestionePensione.DatiPensione datiPensione, ref DatiTotaliAggPeco datiAggPeco,
            ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (crossDataRecipient == null)
                crossDataRecipient = new GestioneContrib.CrossDataRecipient();


            crossDataRecipient.PensioneAnnuaLorda = Convert.ToDecimal(dati.A_Pal);
            string strApp = dati.A_Anzcont.ToString().PadLeft(6, '0');
            crossDataRecipient.ServizioUtileDirittoAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
            crossDataRecipient.ServizioUtileDirittoMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
            crossDataRecipient.ServizioUtileDirittoGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
            //crossDataRecipient.Divisore = Utility.StringToNullableByte(dati.A_Divisore);
            //crossDataRecipient.Capitolo = dati.A_Capitolo.Trim();
            crossDataRecipient.CoefficienteTrasformazione = !string.IsNullOrEmpty(dati.A_CoefTrasf) ? (decimal?)decimal.Parse(dati.A_CoefTrasf, new NumberFormatInfo() { NumberDecimalSeparator = "." }) : null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                if (crossDataRecipient.lDatiServizioUtile == null)
                    crossDataRecipient.lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneContrib.DatiServizioUtile servizioUtile = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodQuota2.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - Dati al 31/12/92
                        if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A" && retr.A_CodQuota2.Trim() == "22")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "A";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "23")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B1";
                        }
                        // Quota B - Dati al 31/12/95
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "26")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B2";
                        }
                        // Quota B - Dati al 31/12/97
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "24")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B3";
                        }
                        // Quota B - Dati dal 01/01/98
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "25")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B5";
                        }
                        // Quota B - Dati cessazione
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "27")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B4";
                        }

                        if (servizioUtile != null)
                            crossDataRecipient.lDatiServizioUtile.Add(servizioUtile);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                bool IsContribNull = true;
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {
                            IsContribNull = false;
                            if (datiAggPeco.Contribuzione == null)
                                datiAggPeco.Contribuzione = new DatiContributivi();
                            strApp = string.Empty;
                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "C":
                                    datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.A_Montante);
                                    //datiContributivi.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiAggPeco.Contribuzione.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));

                                    //DL335FONDO
                                    datiAggPeco.Contribuzione.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                                case "D":
                                    //DatiContribDL214
                                    datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    //datiContributivi.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);

                                    //DatiContribDL214FONDO
                                    datiAggPeco.Contribuzione.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                            }
                        }
                    }
                }

                if (IsContribNull)
                    datiAggPeco.Contribuzione = null;
            }
        }

        internal static void RecuperaDatiTotaliAggPecoINPDAP_AMG(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione, ref DatiTotaliAggPeco datiAggPeco,
           ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (crossDataRecipient == null)
                crossDataRecipient = new GestioneContrib.CrossDataRecipient();

            crossDataRecipient.PensioneAnnuaLorda = Convert.ToDecimal(dati.A_Pal);
            string strApp = dati.A_Anzcont.ToString().PadLeft(6, '0');
            crossDataRecipient.ServizioUtileDirittoAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
            crossDataRecipient.ServizioUtileDirittoMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
            crossDataRecipient.ServizioUtileDirittoGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
            crossDataRecipient.CoefficienteTrasformazione = Convert.ToDecimal(dati.A_CoefTrasf);
            crossDataRecipient.RMSSenzaLegge33670QA = !Utility.IsDoubleEquals(dati.A_Beneficio336, 0.0) ? Convert.ToDecimal(dati.A_Beneficio336) : (decimal?)null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                if (crossDataRecipient.lDatiServizioUtile == null)
                    crossDataRecipient.lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneContrib.DatiServizioUtile servizioUtile = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - Dati al 31/12/92
                        if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "A";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B1";
                        }
                        // Quota B - Dati al 31/12/95
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B2";
                        }
                        // Quota B - Dati al 31/12/97
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B3";
                        }
                        // Quota B - Dati dal 01/01/98
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4")
                        {
                            servizioUtile = new GestioneContrib.DatiServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B4";
                        }
                        // Quota B - Dati cessazione manca in questo caso

                        if (servizioUtile != null)
                            crossDataRecipient.lDatiServizioUtile.Add(servizioUtile);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                bool IsContribNull = true;
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {
                            IsContribNull = false;
                            if (datiAggPeco.Contribuzione == null)
                                datiAggPeco.Contribuzione = new DatiContributivi();
                            strApp = string.Empty;

                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "C":
                                    datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.A_Montante);
                                    //datiContributivi.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiAggPeco.Contribuzione.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));

                                    //DL335FONDO
                                    datiAggPeco.Contribuzione.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.Montante = Convert.ToDecimal(contr.A_Montante);
                                    datiAggPeco.Contribuzione.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    datiAggPeco.Contribuzione.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    break;
                                case "D":
                                    //DatiContribDL214
                                    datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    //datiContributivi.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);

                                    //DatiContribDL214FONDO
                                    datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    datiAggPeco.Contribuzione.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                            }
                        }
                    }
                }

                if (IsContribNull)
                    datiAggPeco.Contribuzione = null;
            }

        }

        internal static void RecuperaDatiTotaliAggPecoDZ(csAggiornamentoPECO_Fondi_Speciali dati, ref DatiTotaliAggPeco datiAggPeco, ref GestioneContrib.CrossDataRecipient crossDataRecipient, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (dati.aRETRIBUTIVE_FS != null && dati.aRETRIBUTIVE_FS.Length > 0)
            {
                GestioneContrib.DatiServizioUtile ServizioUtile = null;
                foreach (F_RETRIBUTIVE retr in dati.aRETRIBUTIVE_FS)
                {
                    if (!string.IsNullOrEmpty(retr.F_CodQuota.Trim()))
                    {
                        if (datiAggPeco.Retribuzione == null)
                            datiAggPeco.Retribuzione = new DatiRetributivi();

                        switch (retr.F_CodQuota.Trim().ToUpperInvariant())
                        {
                            case "AA":
                                datiAggPeco.Retribuzione.RmsQuotaA = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneA = Convert.ToInt32(retr.F_Anzconr);
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneA = null;
                                break;
                            case "BA":
                                datiAggPeco.Retribuzione.RmsQuotaB = Convert.ToDecimal(retr.F_Rms);
                                datiAggPeco.Retribuzione.SettimaneB = Convert.ToInt32(retr.F_Anzconr);
                                if (Utility.IsDoubleEquals(retr.F_Rms, 0) && retr.F_Anzconr == 0)
                                    datiAggPeco.Retribuzione.SettimaneB = null;
                                break;
                            case "A":
                            case "B":
                                if (!Utility.IsDoubleEquals(retr.F_Rms, 0.0) || retr.F_Anzconr != 0)
                                {
                                    if (crossDataRecipient == null)
                                        crossDataRecipient = new GestioneContrib.CrossDataRecipient();
                                    if (crossDataRecipient.lDatiServizioUtile == null)
                                        crossDataRecipient.lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();

                                    ServizioUtile = new GestioneContrib.DatiServizioUtile();
                                    ServizioUtile.ServizioUtileAA = Convert.ToInt16(retr.F_Anzconr);
                                    ServizioUtile.RetribuzionePensionabile = Convert.ToDecimal(retr.F_Rms);
                                    ServizioUtile.Quota = retr.F_CodQuota.Trim().ToUpperInvariant();

                                    crossDataRecipient.lDatiServizioUtile.Add(ServizioUtile);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_FS != null && dati.aCONTRIBUTIVE_FS.Length > 0)
            {
                bool IsContribNull = true;
                foreach (F_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_FS)
                {
                    if (!Utility.IsDoubleEquals(contr.F_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.F_Montante, 0.0) || contr.F_SetteSCL != 0)
                    {
                        IsContribNull = false;
                        if (datiAggPeco.Contribuzione == null)
                            datiAggPeco.Contribuzione = new DatiContributivi();
                        switch (contr.F_CodQuota_Contr.Trim().ToUpperInvariant())
                        {
                            case "D":
                                datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.F_Contrib);
                                datiAggPeco.Contribuzione.MontanteQuotaDL214 = Convert.ToDecimal(contr.F_Montante);
                                datiAggPeco.Contribuzione.NSettimaneQuotaDL214 = contr.F_Anzconc;
                                break;
                        }
                    }
                }

                if (IsContribNull)
                    datiAggPeco.Contribuzione = null;

            }
        }

        internal static void RecuperaDatiTotaliAMGFelpe<T>(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione,
            out List<T> listaDatiServizioUtile, out DatiContributivi datiContributivi, out Entity.DatiCalcolo datiCalcoloForDatiFondo, out GestioneContrib.CrossDataRecipient crossDataRecipient)
        {
            listaDatiServizioUtile = null;
            datiContributivi = null;
            datiCalcoloForDatiFondo = null;
            crossDataRecipient = null;

            if (typeof(T) == typeof(GestioneDatiServizioUtile.ServizioUtile))
            {
                List<GestioneDatiServizioUtile.ServizioUtile> lista = null;
                RecuperaDatiTotaliAMG_FS_PT(dati, datiPensione, out lista, out datiContributivi, out datiCalcoloForDatiFondo);
                listaDatiServizioUtile = lista as List<T>;
            }
            else if (typeof(T) == typeof(GestioneCalcolo.ServizioUtile707))
            {
                List<GestioneCalcolo.ServizioUtile707> lista707 = null;
                RecuperaDatiTotaliAMG_FS_PT707(dati, datiPensione, out lista707, out crossDataRecipient);
                listaDatiServizioUtile = lista707 as List<T>;
            }
            //casi INPDAP AMG
            else if (typeof(T) == typeof(GestioneDatiServizioUtileINPDAP.ServizioUtile))
            {
                List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lista = null;
                RecuperaDatiTotaliINPDAP_AMG(dati, datiPensione, out lista, out datiContributivi, out datiCalcoloForDatiFondo);
                listaDatiServizioUtile = lista as List<T>;
            }
            else if (typeof(T) == typeof(GestioneCalcolo.ServizioUtileINPDAP707))
            {
                List<GestioneCalcolo.ServizioUtileINPDAP707> lista707 = null;
                RecuperaDatiTotaliINPDAP707_AMG(dati, datiPensione, out lista707, out crossDataRecipient);
                listaDatiServizioUtile = lista707 as List<T>;
            }

        }

        internal static void RecuperaDatiTotaliAMGFelpe<T>(csAggiornamentoPECO_Fondi_AMG_INPDAP dati, GestionePensione.DatiPensione datiPensione,
            out List<T> listaDatiServizioUtile, out DatiContributivi datiContributivi, out Entity.DatiCalcolo datiCalcoloForDatiFondo, out GestioneContrib.CrossDataRecipient crossDataRecipient)
        {
            listaDatiServizioUtile = null;
            datiContributivi = null;
            datiCalcoloForDatiFondo = null;
            crossDataRecipient = null;

            if (typeof(T) == typeof(GestioneDatiServizioUtileINPDAP.ServizioUtile))
            {
                List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lista = null;
                RecuperaDatiTotaliINPDAP(dati, datiPensione, out lista, out datiContributivi, out datiCalcoloForDatiFondo);
                listaDatiServizioUtile = lista as List<T>;
            }
            else if (typeof(T) == typeof(GestioneCalcolo.ServizioUtileINPDAP707))
            {
                List<GestioneCalcolo.ServizioUtileINPDAP707> lista707 = null;
                RecuperaDatiTotaliINPDAP707(dati, datiPensione, out lista707, out crossDataRecipient);
                listaDatiServizioUtile = lista707 as List<T>;
            }
        }

        internal static void RecuperaDatiTotaliINPDAP(csAggiornamentoPECO_Fondi_AMG_INPDAP dati, GestionePensione.DatiPensione datiPensione,
            out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtile, out DatiContributivi datiContributivi, out Entity.DatiCalcolo datiCalcoloForDatiFondo)
        {
            datiContributivi = null;
            listaDatiServizioUtile = null;

            datiCalcoloForDatiFondo = new Entity.DatiCalcolo();
            datiCalcoloForDatiFondo.PensioneAnnuaLorda = Convert.ToDecimal(dati.A_Pal);
            string strApp = dati.A_Anzcont.ToString().PadLeft(6, '0');
            datiCalcoloForDatiFondo.ServizioUtileDirittoAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
            datiCalcoloForDatiFondo.ServizioUtileDirittoMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
            datiCalcoloForDatiFondo.ServizioUtileDirittoGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
            datiCalcoloForDatiFondo.Divisore = Utility.StringToNullableByte(dati.A_Divisore);
            datiCalcoloForDatiFondo.Capitolo = dati.A_Capitolo.Trim();
            datiCalcoloForDatiFondo.CoefficienteTrasformazione = !string.IsNullOrEmpty(dati.A_CoefTrasf) ? (decimal?)decimal.Parse(dati.A_CoefTrasf, new NumberFormatInfo() { NumberDecimalSeparator = "." }) : null;
            datiCalcoloForDatiFondo.RMSSenzaLegge33670QA = !Utility.IsDoubleEquals(dati.A_Beneficio336, 0.0) ? Convert.ToDecimal(dati.A_Beneficio336) : (decimal?)null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtileINPDAP.ServizioUtile>();

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneDatiServizioUtileINPDAP.ServizioUtile servizioUtile = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodQuota2.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - Dati al 31/12/92
                        if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A" && retr.A_CodQuota2.Trim() == "22")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "A";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "23")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B1";
                        }
                        // Quota B - Dati al 31/12/95
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "26")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B2";
                        }
                        // Quota B - Dati al 31/12/97
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "24")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B3";
                        }
                        // Quota B - Dati dal 01/01/98
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "25")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B5";
                        }
                        // Quota B - Dati cessazione
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "27")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B4";
                        }

                        if (servizioUtile != null)
                            listaDatiServizioUtile.Add(servizioUtile);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                bool IsContribNull = true;
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {
                            IsContribNull = false;
                            if (datiContributivi == null)
                                datiContributivi = new GestioneAggiornamentoPECO.DatiContributivi();
                            if (datiCalcoloForDatiFondo == null)
                                datiCalcoloForDatiFondo = new Entity.DatiCalcolo();
                            strApp = string.Empty;
                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "C":
                                    datiContributivi.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiContributivi.Montante = Convert.ToDecimal(contr.A_Montante);
                                    //datiContributivi.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiContributivi.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));

                                    //DL335FONDO
                                    datiCalcoloForDatiFondo.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiCalcoloForDatiFondo.Montante = Convert.ToDecimal(contr.A_Montante);
                                    datiCalcoloForDatiFondo.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    datiCalcoloForDatiFondo.NSettimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    break;
                                case "D":
                                    //DatiContribDL214
                                    datiContributivi.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiContributivi.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiContributivi.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    //datiContributivi.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);

                                    //DatiContribDL214FONDO
                                    datiCalcoloForDatiFondo.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiCalcoloForDatiFondo.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    datiCalcoloForDatiFondo.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    datiCalcoloForDatiFondo.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                            }
                        }
                    }
                }

                if (IsContribNull)
                    datiContributivi = null;
            }
        }

        internal static void RecuperaDatiTotaliINPDAP_AMG(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione,
        out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtile, out DatiContributivi datiContributivi, out Entity.DatiCalcolo datiCalcoloForDatiFondo)
        {
            datiContributivi = null;
            listaDatiServizioUtile = null;

            datiCalcoloForDatiFondo = new Entity.DatiCalcolo();
            datiCalcoloForDatiFondo.PensioneAnnuaLorda = Convert.ToDecimal(dati.A_Pal);
            string strApp = dati.A_Anzcont.ToString().PadLeft(6, '0');
            datiCalcoloForDatiFondo.ServizioUtileDirittoAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
            datiCalcoloForDatiFondo.ServizioUtileDirittoMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
            datiCalcoloForDatiFondo.ServizioUtileDirittoGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
            datiCalcoloForDatiFondo.CoefficienteTrasformazione = Convert.ToDecimal(dati.A_CoefTrasf);
            datiCalcoloForDatiFondo.RMSSenzaLegge33670QA = !Utility.IsDoubleEquals(dati.A_Beneficio336, 0.0) ? Convert.ToDecimal(dati.A_Beneficio336) : (decimal?)null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtileINPDAP.ServizioUtile>();

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneDatiServizioUtileINPDAP.ServizioUtile servizioUtile = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - Dati al 31/12/92
                        if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "A";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B1";
                        }
                        // Quota B - Dati al 31/12/95
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B2";
                        }
                        // Quota B - Dati al 31/12/97
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B3";
                        }
                        // Quota B - Dati dal 01/01/98
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }
                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B4";
                        }
                        // Quota B - Dati cessazione manca in questo caso

                        if (servizioUtile != null)
                            listaDatiServizioUtile.Add(servizioUtile);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                bool IsContribNull = true;
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {
                            IsContribNull = false;
                            if (datiContributivi == null)
                                datiContributivi = new GestioneAggiornamentoPECO.DatiContributivi();
                            if (datiCalcoloForDatiFondo == null)
                                datiCalcoloForDatiFondo = new Entity.DatiCalcolo();
                            strApp = string.Empty;
                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "C":
                                    datiContributivi.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiContributivi.Montante = Convert.ToDecimal(contr.A_Montante);
                                    //datiContributivi.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiContributivi.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));

                                    //DL335FONDO
                                    datiCalcoloForDatiFondo.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiCalcoloForDatiFondo.Montante = Convert.ToDecimal(contr.A_Montante);
                                    datiCalcoloForDatiFondo.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    datiCalcoloForDatiFondo.NSettimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    break;
                                case "D":
                                    //DatiContribDL214
                                    datiContributivi.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiContributivi.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiContributivi.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    //datiContributivi.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);

                                    //DatiContribDL214FONDO
                                    datiCalcoloForDatiFondo.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiCalcoloForDatiFondo.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    datiCalcoloForDatiFondo.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    datiCalcoloForDatiFondo.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                            }
                        }
                    }
                }

                if (IsContribNull)
                    datiContributivi = null;
            }
        }

        internal static void RecuperaDatiTotaliINPDAP707_AMG(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione,
          out List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtileINPDAP707, out GestioneContrib.CrossDataRecipient crossDataRecipient)
        {
            listaDatiServizioUtileINPDAP707 = null;
            crossDataRecipient = null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                listaDatiServizioUtileINPDAP707 = new List<GestioneCalcolo.ServizioUtileINPDAP707>();

                if (!Utility.IsDoubleEquals(dati.A_Pal707, 0.0))
                {
                    crossDataRecipient = new GestioneContrib.CrossDataRecipient();
                    crossDataRecipient.PensioneAnnuaLorda707 = Convert.ToDecimal(dati.A_Pal707);
                }

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneCalcolo.ServizioUtileINPDAP707 servizioUtile707 = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - Dati al 31/12/92
                        if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "A";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B1";
                        }
                        // Quota B - Dati al 31/12/95
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B2";
                        }
                        // Quota B - Dati al 31/12/97
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B3";
                        }
                        // Quota B - Dati dal 01/01/98
                        else if (new List<string> { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B4";
                        }
                        // Quota B - Dati cessazione manca in questo caso


                        if (servizioUtile707 != null)
                            listaDatiServizioUtileINPDAP707.Add(servizioUtile707);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {

                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "D":
                                    if (crossDataRecipient == null)
                                        crossDataRecipient = new GestioneContrib.CrossDataRecipient();

                                    crossDataRecipient.IsQuotaDPresente = true;
                                    break;
                            }
                        }

                    }
                }
            }
        }

        internal static void RecuperaDatiTotaliAMG_FS_PT(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione,
            out List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile, out DatiContributivi datiContributivi, out Entity.DatiCalcolo datiCalcoloForDatiFondo)
        {
            datiContributivi = null;
            listaDatiServizioUtile = null;
            datiCalcoloForDatiFondo = new Entity.DatiCalcolo();

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            datiCalcoloForDatiFondo.PensioneAnnuaLorda = Convert.ToDecimal(dati.A_Pal);
            string strApp = dati.A_Anzcont.ToString().PadLeft(6, '0');
            datiCalcoloForDatiFondo.ServizioUtileDirittoAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
            datiCalcoloForDatiFondo.ServizioUtileDirittoMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
            datiCalcoloForDatiFondo.ServizioUtileDirittoGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
            datiCalcoloForDatiFondo.CoefficienteTrasformazione = Convert.ToDecimal(dati.A_CoefTrasf);
            datiCalcoloForDatiFondo.RMSSenzaLegge33670QA = !Utility.IsDoubleEquals(dati.A_Beneficio336, 0.0) ? Convert.ToDecimal(dati.A_Beneficio336) : (decimal?)null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneDatiServizioUtile.ServizioUtile servizioUtile = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - al 92
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.ImportoIndennitaIntegrativaSpeciale = Convert.ToDecimal(dati.A_Iissultgior);
                            servizioUtile.Quota = "A";
                        }
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.ImportoIndennitaIntegrativaSpeciale = Convert.ToDecimal(dati.A_Iissultgior);
                            servizioUtile.Quota = "A";
                        }
                        // Quota B1 - 1993 -1994
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1"))
                        {
                            servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B1";
                        }
                        // Quota B2 - 1995
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2"))
                        {
                            servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Quota = "B2";
                        }
                        // Quota B3 - 1996 - 1997
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3"))
                        {
                            servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Quota = "B3";
                        }
                        // Quota B4 - post 1997
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4"))
                        {
                            servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                                servizioUtile.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar);
                            }

                            servizioUtile.Quota = "B4";
                        }

                        if (servizioUtile != null)
                            listaDatiServizioUtile.Add(servizioUtile);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                bool IsContribNull = true;
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {
                            IsContribNull = false;
                            if (datiContributivi == null)
                                datiContributivi = new GestioneAggiornamentoPECO.DatiContributivi();
                            strApp = string.Empty;
                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "C":
                                    //datiContributivi.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    //datiContributivi.Montante = Convert.ToDecimal(contr.A_Montante);

                                    //datiContributivi.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                    //    (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                    //    (Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() / 6.923));
                                    //datiContributivi.MontanteContributivo = datiContributivi.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);

                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiCalcoloForDatiFondo.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiCalcoloForDatiFondo.Montante = Convert.ToDecimal(contr.A_Montante);
                                    datiCalcoloForDatiFondo.MontanteContributivo = Convert.ToDecimal(contr.A_Quotac);
                                    datiCalcoloForDatiFondo.NSettimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    break;
                                case "D":
                                    //datiContributivi.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    //datiContributivi.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    ////datiContributivi.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);

                                    //datiContributivi.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                    //    (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                    //    (Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() / 6.923));

                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiCalcoloForDatiFondo.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiCalcoloForDatiFondo.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    datiCalcoloForDatiFondo.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(4, 2)).GetValueOrDefault() / 6.923));
                                    datiCalcoloForDatiFondo.QuotaContributivaAnnua = Convert.ToDecimal(contr.A_Quotac);
                                    break;
                            }
                        }
                    }
                }

                if (IsContribNull)
                    datiContributivi = null;
            }
        }

        internal static void RecuperaDatiTotaliINPDAP707(csAggiornamentoPECO_Fondi_AMG_INPDAP dati, GestionePensione.DatiPensione datiPensione,
            out List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtileINPDAP707, out GestioneContrib.CrossDataRecipient crossDataRecipient)
        {
            listaDatiServizioUtileINPDAP707 = null;
            crossDataRecipient = null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                listaDatiServizioUtileINPDAP707 = new List<GestioneCalcolo.ServizioUtileINPDAP707>();

                if (!Utility.IsDoubleEquals(dati.A_Pal707, 0.0))
                {
                    crossDataRecipient = new GestioneContrib.CrossDataRecipient();
                    crossDataRecipient.PensioneAnnuaLorda707 = Convert.ToDecimal(dati.A_Pal707);
                }

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneCalcolo.ServizioUtileINPDAP707 servizioUtile707 = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - Dati al 31/12/92
                        if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A" && retr.A_CodQuota2.Trim() == "22")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "A";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "23")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B1";
                        }
                        // Quota B - Dati al 31/12/95
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "26")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B2";
                        }
                        // Quota B - Dati al 31/12/97
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "24")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B3";
                        }
                        // Quota B - Dati dal 01/01/98
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "25")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B5";
                        }
                        // Quota B - Dati cessazione
                        else if (new List<string> { "T", "D", "P", "S", "U" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B" && retr.A_CodQuota2.Trim() == "27")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtileINPDAP707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B4";
                        }

                        if (servizioUtile707 != null)
                            listaDatiServizioUtileINPDAP707.Add(servizioUtile707);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {

                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "D":
                                    if (crossDataRecipient == null)
                                        crossDataRecipient = new GestioneContrib.CrossDataRecipient();

                                    crossDataRecipient.IsQuotaDPresente = true;
                                    break;
                            }
                        }

                    }
                }
            }
        }

        internal static void RecuperaDatiTotaliAMG_FS_PT707(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione,
            out List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707, out GestioneContrib.CrossDataRecipient crossDataRecipient)
        {
            listaDatiServizioUtile707 = null;
            crossDataRecipient = null;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                listaDatiServizioUtile707 = new List<GestioneCalcolo.ServizioUtile707>();

                if (!Utility.IsDoubleEquals(dati.A_Pal707, 0.0))
                {
                    crossDataRecipient = new GestioneContrib.CrossDataRecipient();
                    crossDataRecipient.PensioneAnnuaLorda707 = Convert.ToDecimal(dati.A_Pal707);
                }

                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneCalcolo.ServizioUtile707 servizioUtile707 = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        // Quota A - al 92
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtile707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "A";
                        }
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtile707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "A";
                        }
                        // Quota B1 - 1993 -1994
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B1"))
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtile707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B1";
                        }
                        // Quota B2 - 1995
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B2"))
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtile707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B2";
                        }
                        // Quota B3 - 1996 - 1997
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B3"))
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtile707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B3";
                        }
                        // Quota B4 - post 1997
                        else if ((tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && new List<string> { "D" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4") ||
                            (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PT && new List<string> { "G" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B4"))
                        {
                            servizioUtile707 = new GestioneCalcolo.ServizioUtile707();

                            if (retr.A_Anzcon707 != 0)
                            {
                                string strApp = retr.A_Anzcon707.ToString().PadLeft(6, '0');
                                servizioUtile707.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile707.ServizioUtileCessazioneMM = Utility.StringToNullableShort(strApp.Substring(2, 2));
                                servizioUtile707.ServizioUtileCessazioneGG = Utility.StringToNullableShort(strApp.Substring(4, 2));
                            }

                            servizioUtile707.QuotaPensioneRetributivaAnnua = Convert.ToDecimal(retr.A_Quotar707);
                            servizioUtile707.Quota = "B4";
                        }

                        if (servizioUtile707 != null)
                            listaDatiServizioUtile707.Add(servizioUtile707);
                    }
                }
            }

            if (dati.aCONTRIBUTIVE_AMG != null && dati.aCONTRIBUTIVE_AMG.Count() > 0)
            {
                foreach (A_CONTRIBUTIVE contr in dati.aCONTRIBUTIVE_AMG)
                {
                    if (!string.IsNullOrEmpty(contr.A_CodQuota_Contr.Trim()) && !string.IsNullOrEmpty(contr.A_CodGest_Contr.Trim()))
                    {
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0 || !Utility.IsDoubleEquals(contr.A_Quotac, 0.0))
                        {

                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "D":
                                    if (crossDataRecipient == null)
                                        crossDataRecipient = new GestioneContrib.CrossDataRecipient();

                                    crossDataRecipient.IsQuotaDPresente = true;
                                    break;
                            }
                        }

                    }
                }
            }
        }

        internal static void ImpostaDatiControlloToSave(Utility.TipoFondo? tipoFondo, DatiTotaliAggPeco datiAggPeco,
            GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            List<GestioneContrib.DatiServizioUtile> listaDatiServizioUtile, List<Entity.DatiCalcolo707.DatiServizioUtile707> listaDatiServizioUtile707, GestioneFondo.DatiFondo datiFondo, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiAggPeco.DatiControllo == null)
                datiAggPeco.DatiControllo = new DatiControllo();

            datiAggPeco.DatiControllo.IsCalcoloValido = true;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                        if (datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null && ((datiPensione.TipoCalcolo.HasValue && datiPensione.TipoCalcolo.Value == 25))) //controllo calcolo retributivo monti
                        {
                            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                            {
                                if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
                                {
                                    messaggioVideo = "Montante L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                                if (datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 == 0.0M)
                                {
                                    messaggioVideo = "Importo contributivo totale L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                                if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
                                {
                                    messaggioVideo = "N settimane L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //controllo calcolo contributivo //controllo calcolo misto
                            if ((datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione == null) || (datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null))
                            {
                                if (datiAggPeco.Contribuzione.ImportoContributivoTotale == 0.0M)
                                {
                                    //ENG - PL CONTRIBUZIONE POST 2011
                                    if (!(!Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) &&
                                        Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(2011, 12, 31))))
                                    {
                                        messaggioVideo = "Importo contributivo totale L.335 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                }

                                if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                                {
                                    if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
                                    {
                                        messaggioVideo = "Montante L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                    if (datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 == 0.0M)
                                    {
                                        messaggioVideo = "Importo contributivo totale L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                    if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
                                    {
                                        messaggioVideo = "N settimane L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case Utility.TipoFondo.VL:
                        if (datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null && ((datiPensione.TipoCalcolo.HasValue && datiPensione.TipoCalcolo.Value == 25))) //controllo calcolo retributivo monti
                        {
                            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                            {
                                if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
                                {
                                    messaggioVideo = "Montante L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                                if (datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 == 0.0M)
                                {
                                    messaggioVideo = "Importo contributivo totale L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                                if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
                                {
                                    messaggioVideo = "N settimane L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //controllo calcolo contributivo //controllo calcolo misto
                            if ((datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione == null) || (datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null))
                            {
                                //solo per misto 335 con Monti salto controlli obbligatorietà legge 335
                                if (!((datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null) &&
                                    (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)))
                                {
                                    //ENG - PL CONTRIBUZIONE POST 2011
                                    if (!(!Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) &&
                                          datiPensione.InizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(2011, 12, 31))))
                                    {
                                        if (datiAggPeco.Contribuzione.ImportoContributivoTotale == 0.0M)
                                        {
                                            messaggioVideo = "Importo contributivo totale L.335 obbligatorio";
                                            datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                            return;
                                        }

                                        if (datiAggPeco.Retribuzione == null && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                                        {
                                            if (datiAggPeco.Contribuzione.Montante == 0.0M)
                                            {
                                                messaggioVideo = "Montante L.335 obbligatorio";
                                                datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (datiAggPeco.Contribuzione.Montante == 0.0M && datiAggPeco.Contribuzione.MontanteAnte0697 == 0.0M)
                                            {
                                                messaggioVideo = "Montante da 01/96 a 06/97 o Montante dal 07/97 L.335 obbligatori";
                                                datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                                return;
                                            }
                                        }
                                    }
                                }
                                if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                                {
                                    if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
                                    {
                                        messaggioVideo = "Montante L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                    if (datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 == 0.0M)
                                    {
                                        messaggioVideo = "Importo contributivo totale L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                    if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
                                    {
                                        messaggioVideo = "N settimane L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case Utility.TipoFondo.GAS:
                        if (datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null && ((datiPensione.TipoCalcolo.HasValue && datiPensione.TipoCalcolo.Value == 25))) //controllo calcolo retributivo monti
                        {
                            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                            {
                                if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
                                {
                                    messaggioVideo = "Montante L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                                if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
                                {
                                    messaggioVideo = "N settimane L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //controllo calcolo contributivo //controllo calcolo misto
                            if ((datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione == null) || (datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null))
                            {
                                if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                                {
                                    if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
                                    {
                                        messaggioVideo = "Montante L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                    if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
                                    {
                                        messaggioVideo = "N settimane L.214 obbligatorio";
                                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case Utility.TipoFondo.DZ:
                    case Utility.TipoFondo.ES:
                        if (datiAggPeco.Contribuzione != null && datiAggPeco.Retribuzione != null && ((datiPensione.TipoCalcolo.HasValue && datiPensione.TipoCalcolo.Value == 25))) //controllo calcolo retributivo monti
                        {
                            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                            {
                                if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
                                {
                                    messaggioVideo = "Montante L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                                if (datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 == 0.0M)
                                {
                                    messaggioVideo = "Importo contributivo totale L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                                if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
                                {
                                    messaggioVideo = "N settimane L.214 obbligatorio";
                                    datiAggPeco.DatiControllo.IsCalcoloValido = false;
                                    return;
                                }
                            }
                        }
                        break;
                }
            }
            ImpostaDatiControllo(tipoFondo, datiAggPeco, datiPensione, datiDanteCausa, datiMaggiorazioniBenefici, listaDatiServizioUtile, listaDatiServizioUtile707, datiFondo, isRiaperturaDomanda, out messaggioVideo);
        }

        internal static void ImpostaDatiControllo(Utility.TipoFondo? tipoFondo, DatiTotaliAggPeco datiAggPeco,
            GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            List<GestioneContrib.DatiServizioUtile> listaDatiServizioUtile, List<Entity.DatiCalcolo707.DatiServizioUtile707> listaDatiServizioUtile707, GestioneFondo.DatiFondo datiFondo, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = "";
            char? codiceSpecificoTraduzioneSuGP = null;
            string tipoSettimaneBeneficio = null;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            DateTime? decorrenzaAntePostArmonizzazione = (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione != null) ? datiDanteCausa.DecorrenzaPensione : datiPensione.DecorrenzaOriginaria;

            if (datiAggPeco == null)
                return;
            if (datiAggPeco.DatiControllo == null)
                datiAggPeco.DatiControllo = new DatiControllo();

            if (datiFondo != null && datiFondo.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> listaCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out listaCodiceSpecifico);

                if (listaCodiceSpecifico != null && listaCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = listaCodiceSpecifico.Find(x => x.Id == datiFondo.CodiceSpecifico);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtile = null;
            if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
            {
                lDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (GestioneContrib.DatiServizioUtile sUtile in listaDatiServizioUtile)
                {
                    GestioneDatiServizioUtile.ServizioUtile servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    Utility.ValorizzaOggetti(sUtile, servizioUtile);
                    lDatiServizioUtile.Add(servizioUtile);
                }
            }

            List<Entity.DatiCalcolo707.DatiServizioUtile707> lDatiServizioUtile707 = null;
            if (listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
            {
                lDatiServizioUtile707 = new List<Entity.DatiCalcolo707.DatiServizioUtile707>();
                foreach (Entity.DatiCalcolo707.DatiServizioUtile707 sUtile707 in listaDatiServizioUtile707)
                {
                    Entity.DatiCalcolo707.DatiServizioUtile707 servizioUtile707 = new Entity.DatiCalcolo707.DatiServizioUtile707();
                    Utility.ValorizzaOggetti(sUtile707, servizioUtile707);
                    lDatiServizioUtile707.Add(servizioUtile707);
                }
            }

            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            try
            {
                if (datiAggPeco.Contribuzione != null && (datiAggPeco.Retribuzione == null && (lDatiServizioUtile == null || lDatiServizioUtile.Count == 0)))  //controllo calcolo contributivo
                {
                    datiAggPeco.DatiControllo.TipoCalcolo = TipoCalcolo.Contributivo;

                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:
                            case Utility.TipoFondo.TT:
                            case Utility.TipoFondo.ET:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloContributivo(datiAggPeco, datiPensione, codiceSpecificoTraduzioneSuGP,
                                    tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.VL:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloContributivoFondoVL(datiAggPeco, datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio,
                                    maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloContributivoFondoFS_PT(datiAggPeco, datiPensione.DecorrenzaOriginaria, datiPensione,
                                    isRiaperturaDomanda, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.GAS:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloContributivoFondoGAS(datiAggPeco, datiPensione, out messaggioVideo);
                                break;
                        }
                    }

                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && Utility.IsDomandaPL(datiPensione))
                    {
                        datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloContributivoFondoFS_PT(datiAggPeco, datiPensione.DecorrenzaOriginaria, datiPensione,
                               isRiaperturaDomanda, out messaggioVideo);

                    }
                }
                else if (datiAggPeco.Contribuzione == null && (datiAggPeco.Retribuzione != null || (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)))  //controllo calcolo retributivo
                {
                    datiAggPeco.DatiControllo.TipoCalcolo = TipoCalcolo.Retributivo;

                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:
                            case Utility.TipoFondo.TT:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivo(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, datiDanteCausa,
                                    codiceSpecificoTraduzioneSuGP, tipoFondo, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.ET:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoFondoET(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, datiDanteCausa,
                                    codiceSpecificoTraduzioneSuGP, lDatiServizioUtile, tipoFondo, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.VL:
                                // Se è ante armonizzazione non effettuo i controlli sui dati calcolo
                                datiAggPeco.DatiControllo.IsCalcoloValido = Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaAntePostArmonizzazione) ||
                                    GestioneControlli.ControlsCalcoloRetributivoFondoVL(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, codiceSpecificoTraduzioneSuGP, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.DZ:
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:
                                datiAggPeco.DatiControllo.IsCalcoloValido = true;
                                break;
                            case Utility.TipoFondo.GAS:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoFondoGAS(datiAggPeco, out messaggioVideo);
                                break;
                        }
                    }
                }
                //controllo calcolo retributivo Monti
                else if (datiAggPeco.Contribuzione != null && (datiAggPeco.Retribuzione != null || (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)) &&
                    ((datiPensione.TipoCalcolo.HasValue && datiPensione.TipoCalcolo.Value == 25) || datiAggPeco.DatiParziali.FlagCalcolo == "1") &&
                    ((datiAggPeco.DatiParziali.DecorrenzaPensione.HasValue && DateTime.Compare(datiAggPeco.DatiParziali.DecorrenzaPensione.Value.Date, new DateTime(2012, 01, 01).Date) >= 0) ||
                    (datiPensione.DecorrenzaOriginaria.HasValue && DateTime.Compare(datiPensione.DecorrenzaOriginaria.Value.Date, new DateTime(2012, 01, 01).Date) >= 0)))
                {
                    datiAggPeco.DatiControllo.TipoCalcolo = TipoCalcolo.RetributivoMonti;

                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:
                            case Utility.TipoFondo.TT:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoMonti(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, codiceSpecificoTraduzioneSuGP,
                                    out messaggioVideo);
                                break;
                            case Utility.TipoFondo.ET:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoMontiFondoET(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, datiDanteCausa,
                                        lDatiServizioUtile, codiceSpecificoTraduzioneSuGP, tipoFondo, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.VL:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoMontiFondoVL(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, codiceSpecificoTraduzioneSuGP,
                                    out messaggioVideo);
                                break;
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:
                                //datiAggPeco.DatiControllo.IsCalcoloValido = true;
                                //creo un metodo ControlsCalcoloRetributivoMontiFSPT dove richiamo ControlsCalcoloContributivoL214
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoMontiFondoFSPT(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, codiceSpecificoTraduzioneSuGP, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.GAS:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoMontiFondoGAS(datiAggPeco, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.DZ:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloRetributivoMontiFondoDZ(datiAggPeco, out messaggioVideo);
                                break;
                        }
                    }
                }
                else if (datiAggPeco.Contribuzione != null && (datiAggPeco.Retribuzione != null || (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0))) //controllo calcolo misto
                {
                    datiAggPeco.DatiControllo.TipoCalcolo = TipoCalcolo.Misto;

                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:
                            case Utility.TipoFondo.TT:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloMisto(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, codiceSpecificoTraduzioneSuGP, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.ET:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloMistoFondoET(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, datiDanteCausa, lDatiServizioUtile,
                                    codiceSpecificoTraduzioneSuGP, tipoFondo, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.VL:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloMistoFondoVL(datiAggPeco, datiMaggiorazioniBenefici, datiPensione, codiceSpecificoTraduzioneSuGP, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloContributivoFondoFS_PT(datiAggPeco, datiPensione.DecorrenzaOriginaria, datiPensione,
                                    isRiaperturaDomanda, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.GAS:
                                datiAggPeco.DatiControllo.IsCalcoloValido = GestioneControlli.ControlsCalcoloMistoFondoGAS(datiAggPeco, datiPensione, out messaggioVideo);
                                break;
                        }
                    }
                }
                else if (datiAggPeco.Contribuzione == null && (datiAggPeco.Retribuzione == null && (lDatiServizioUtile == null || lDatiServizioUtile.Count == 0)))
                {
                    if (!datiAggPeco.DatiParziali.TipoCalcolo.HasValue || datiAggPeco.DatiParziali.TipoCalcolo.Value == TipoCalcolo.NonValido)
                    {
                        datiAggPeco.DatiControllo.TipoCalcolo = TipoCalcolo.NonValido;
                        datiAggPeco.DatiControllo.IsCalcoloValido = false;
                        messaggioVideo = "Dati calcolo mancanti o tipo calcolo non selezionato in Liquidazione Pensione";
                    }
                    else
                    {
                        datiAggPeco.DatiControllo.TipoCalcolo = datiAggPeco.DatiParziali.TipoCalcolo.Value;
                        datiAggPeco.DatiControllo.IsCalcoloValido = true;
                    }
                }
            }
            catch (Exception)
            {
                datiAggPeco.DatiControllo = new DatiControllo();
                datiAggPeco.DatiControllo.TipoCalcolo = TipoCalcolo.NonValido;
                datiAggPeco.DatiControllo.IsCalcoloValido = false;
                messaggioVideo = "Controllo validità calcolo non riuscito. Controllare inserimento delle date di inizio e fine assicurazione";
            }
        }

        private static void RecuperaDatiParzialiAggPeco(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, csAggiornamentoPECO_Fondi_Speciali dati, out DatiParzialiAggPeco datiParziali, out string errori)
        {
            datiParziali = new DatiParzialiAggPeco();
            errori = "";

            if (dati == null)
            {
                errori = "Errore nel recupero dei dati da FELPE: dati non presenti.";
                datiParziali = null;
                return;
            }

            if (dati.F_Return_Code != 0)
            {
                errori = string.Format("Errore nel recupero dei dati da FELPE: {0}", dati.F_Return_Code);
                datiParziali = null;
                return;
            }

            if (dati.F_Decorre.Trim().Length == 8)
                datiParziali.DecorrenzaPensione = new DateTime(int.Parse(dati.F_Decorre.Substring(0, 4)),
                                                                                        int.Parse(dati.F_Decorre.Substring(4, 2)),
                                                                                        int.Parse(dati.F_Decorre.Substring(6, 2)));
            else
                datiParziali.DecorrenzaPensione = new DateTime(int.Parse(dati.F_Decorre.Substring(0, 4)),
                                                                                        int.Parse(dati.F_Decorre.Substring(4, 2)), 1);

            if (dati.F_Iniass.ToString().Trim().Length == 8)
                datiParziali.InizioAssicurazione = dati.F_Iniass != 0 ? new DateTime(int.Parse(dati.F_Iniass.ToString().Substring(0, 4)),
                                                                                      int.Parse(dati.F_Iniass.ToString().Substring(4, 2)),
                                                                                      int.Parse(dati.F_Iniass.ToString().Substring(6, 2))) : (DateTime?)null;
            else
                datiParziali.InizioAssicurazione = new DateTime(int.Parse(dati.F_Iniass.ToString().Substring(0, 4)),
                                                                                      int.Parse(dati.F_Iniass.ToString().Substring(4, 2)), 1);
            if (dati.F_Finass.ToString().Trim().Length == 8)
                datiParziali.FineAssicurazione = new DateTime(int.Parse(dati.F_Finass.ToString().Substring(0, 4)),
                                                                                    int.Parse(dati.F_Finass.ToString().Substring(4, 2)),
                                                                                    int.Parse(dati.F_Finass.ToString().Substring(6, 2)));
            else
                datiParziali.FineAssicurazione = new DateTime(int.Parse(dati.F_Finass.ToString().Substring(0, 4)),
                                                                                    int.Parse(dati.F_Finass.ToString().Substring(4, 2)), 1);
            datiParziali.FlagCalcolo = dati.F_Sistcal;

            if (dati.F_Anzcont != 0)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.VL:
                        datiParziali.SettimaneUtiliDiritto = dati.F_Anzcont;
                        break;
                }
            }


            return;
        }

        private static void RecuperaDatiParzialiAggPeco(csAggiornamentoPECO_Fondi_AMG dati, out DatiParzialiAggPeco datiParziali, out string errori)
        {
            datiParziali = new DatiParzialiAggPeco();
            errori = "";

            if (dati == null)
            {
                errori = "Errore nel recupero dei dati da FELPE: dati non presenti.";
                datiParziali = null;
                return;
            }

            if (dati.A_Return_Code != 0)
            {
                errori = string.Format("Errore nel recupero dei dati da FELPE: {0}", dati.A_Return_Code);
                datiParziali = null;
                return;
            }

            if (dati.A_Decorre.Trim().Length == 8)
                datiParziali.DecorrenzaPensione = new DateTime(int.Parse(dati.A_Decorre.Substring(0, 4)),
                                                                                        int.Parse(dati.A_Decorre.Substring(4, 2)),
                                                                                        int.Parse(dati.A_Decorre.Substring(6, 2)));
            else
                datiParziali.DecorrenzaPensione = new DateTime(int.Parse(dati.A_Decorre.Substring(0, 4)),
                                                                                        int.Parse(dati.A_Decorre.Substring(4, 2)), 1);

            if (dati.A_Iniass.ToString().Trim().Length == 8)
                datiParziali.InizioAssicurazione = dati.A_Iniass != 0 ? new DateTime(int.Parse(dati.A_Iniass.ToString().Substring(0, 4)),
                                                                                      int.Parse(dati.A_Iniass.ToString().Substring(4, 2)),
                                                                                      int.Parse(dati.A_Iniass.ToString().Substring(6, 2))) : (DateTime?)null;
            else
                datiParziali.InizioAssicurazione = new DateTime(int.Parse(dati.A_Iniass.ToString().Substring(0, 4)),
                                                                                      int.Parse(dati.A_Iniass.ToString().Substring(4, 2)), 1);
            if (dati.A_Finass.ToString().Trim().Length == 8)
                datiParziali.FineAssicurazione = new DateTime(int.Parse(dati.A_Finass.ToString().Substring(0, 4)),
                                                                                    int.Parse(dati.A_Finass.ToString().Substring(4, 2)),
                                                                                    int.Parse(dati.A_Finass.ToString().Substring(6, 2)));
            else
                datiParziali.FineAssicurazione = new DateTime(int.Parse(dati.A_Finass.ToString().Substring(0, 4)),
                                                                                    int.Parse(dati.A_Finass.ToString().Substring(4, 2)), 1);
            datiParziali.FlagCalcolo = dati.A_Sistcal;

            return;
        }

        private static void RecuperaDatiParzialiAggPecoINPDAP(csAggiornamentoPECO_Fondi_AMG_INPDAP dati, out DatiParzialiAggPeco datiParziali, out string errori)
        {
            datiParziali = new DatiParzialiAggPeco();
            errori = "";

            if (dati == null)
            {
                errori = "Errore nel recupero dei dati da FELPE: dati non presenti.";
                datiParziali = null;
                return;
            }

            if (dati.A_Return_Code != 0)
            {
                errori = string.Format("Errore nel recupero dei dati da FELPE: {0}", dati.A_Return_Code);
                datiParziali = null;
                return;
            }

            if (dati.A_Decorre.Trim().Length == 8)
                datiParziali.DecorrenzaPensione = new DateTime(int.Parse(dati.A_Decorre.Substring(0, 4)),
                                                                                        int.Parse(dati.A_Decorre.Substring(4, 2)),
                                                                                        int.Parse(dati.A_Decorre.Substring(6, 2)));
            else
                datiParziali.DecorrenzaPensione = new DateTime(int.Parse(dati.A_Decorre.Substring(0, 4)),
                                                                                        int.Parse(dati.A_Decorre.Substring(4, 2)), 1);

            if (dati.A_Iniass.ToString().Trim().Length == 8)
                datiParziali.InizioAssicurazione = dati.A_Iniass != 0 ? new DateTime(int.Parse(dati.A_Iniass.ToString().Substring(0, 4)),
                                                                                      int.Parse(dati.A_Iniass.ToString().Substring(4, 2)),
                                                                                      int.Parse(dati.A_Iniass.ToString().Substring(6, 2))) : (DateTime?)null;
            else
                datiParziali.InizioAssicurazione = new DateTime(int.Parse(dati.A_Iniass.ToString().Substring(0, 4)),
                                                                                      int.Parse(dati.A_Iniass.ToString().Substring(4, 2)), 1);
            if (dati.A_Finass.ToString().Trim().Length == 8)
                datiParziali.FineAssicurazione = new DateTime(int.Parse(dati.A_Finass.ToString().Substring(0, 4)),
                                                                                    int.Parse(dati.A_Finass.ToString().Substring(4, 2)),
                                                                                    int.Parse(dati.A_Finass.ToString().Substring(6, 2)));
            else
                datiParziali.FineAssicurazione = new DateTime(int.Parse(dati.A_Finass.ToString().Substring(0, 4)),
                                                                                    int.Parse(dati.A_Finass.ToString().Substring(4, 2)), 1);
            datiParziali.FlagCalcolo = dati.A_Sistcal;

            return;
        }

        #endregion Dati Contrib

        #region private method

        private static void AggiornamentoPECO_FS(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_Speciali dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            GestionePecoServiceClient proxy = new GestionePecoServiceClient();

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
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
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

        private static void Aggiornamento_PECO_Fondi_AMG(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_AMG dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_FS, method Aggiornamento_PECO_FS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
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

        private static void Aggiornamento_PECO_Fondi_AMG_INPDAP(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_AMG_INPDAP dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_FS, method Aggiornamento_PECO_Fondi_AMG_INPDAP | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC_FS, method Aggiornamento_PECO_Fondi_AMG_INPDAP | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC_FS, method Aggiornamento_PECO_Fondi_AMG_INPDAP | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC_FS, method Aggiornamento_PECO_Fondi_AMG_INPDAP: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
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

        #endregion private method

        #region nested class

        public class DatiTotaliAggPeco
        {
            public DatiTotaliAggPeco()
            {
            }
            #region private properties
            private DatiContributivi _Contribuzione;
            private DatiRetributivi _Retribuzione;
            private DatiParzialiAggPeco _DatiParziali;
            private DatiControllo _DatiControllo;
            #endregion private properties

            #region public properties
            public DatiContributivi Contribuzione { get { return _Contribuzione; } set { _Contribuzione = value; } }
            public DatiRetributivi Retribuzione { get { return _Retribuzione; } set { _Retribuzione = value; } }
            public DatiParzialiAggPeco DatiParziali { get { return _DatiParziali; } set { _DatiParziali = value; } }
            public DatiControllo DatiControllo { get { return _DatiControllo; } set { _DatiControllo = value; } }
            #endregion public properties

        }

        public class DatiParzialiAggPeco
        {
            #region private properties
            private DateTime? _DecorrenzaPensione;
            private DateTime? _InizioAssicurazione;
            private DateTime? _FineAssicurazione;
            private TipoCalcolo? _TipoCalcolo;
            private string _FlagCalcolo;
            private int? _SettimaneUtiliDiritto;
            private int? _SettimaneUtiliDirittoOI;
            #endregion private properties

            #region public properties
            public DateTime? DecorrenzaPensione { get { return _DecorrenzaPensione; } set { _DecorrenzaPensione = value; } }
            public DateTime? InizioAssicurazione { get { return _InizioAssicurazione; } set { _InizioAssicurazione = value; } }
            public DateTime? FineAssicurazione { get { return _FineAssicurazione; } set { _FineAssicurazione = value; } }
            public TipoCalcolo? TipoCalcolo { get { return _TipoCalcolo; } set { _TipoCalcolo = value; } }
            public string FlagCalcolo { get { return _FlagCalcolo; } set { _FlagCalcolo = value; } }
            public int? SettimaneUtiliDiritto { get { return _SettimaneUtiliDiritto; } set { _SettimaneUtiliDiritto = value; } }
            public int? SettimaneUtiliDirittoOI { get { return _SettimaneUtiliDirittoOI; } set { _SettimaneUtiliDirittoOI = value; } }
            #endregion public properties

        }

        public class DatiContributivi
        {
            #region private properties
            private decimal _Montante;

            private decimal _ImportoContributivoTotale;
            private int _Settimane;

            private decimal _MontanteAnte0697;
            private short _AnzianitaAnte0697AA;
            private short _AnzianitaAnte0697MM;
            private short _AnzianitaAnte0697GG;
            private short _AnzianitaPost0697AA;
            private short _AnzianitaPost0697MM;
            private short _AnzianitaPost0697GG;
            private decimal _MontanteContributivo;
            private decimal _MontanteQuotaDL214;
            private decimal _ImportoContribTotaleQuotaDL214;
            private int _NSettimaneQuotaDL214;
            private decimal _QuotaContributivaAnnua;

            //Aggiunte in seguito allo sviluppo del fondo GAS. Da rimuovere se non utilizzate ad AggPECO
            private decimal _MontanteEsclusivo;
            private decimal _MontanteEsclusivoQuotaDL214;
            #endregion private properties

            #region public properties
            public decimal Montante { get { return _Montante; } set { _Montante = value; } }
            public decimal ImportoContributivoTotale { get { return _ImportoContributivoTotale; } set { _ImportoContributivoTotale = value; } }
            public int Settimane { get { return _Settimane; } set { _Settimane = value; } }
            public decimal MontanteContributivo { get { return _MontanteContributivo; } set { _MontanteContributivo = value; } }

            public decimal MontanteAnte0697 { get { return _MontanteAnte0697; } set { _MontanteAnte0697 = value; } }
            public short AnzianitaAnte0697AA { get { return _AnzianitaAnte0697AA; } set { _AnzianitaAnte0697AA = value; } }
            public short AnzianitaAnte0697MM { get { return _AnzianitaAnte0697MM; } set { _AnzianitaAnte0697MM = value; } }
            public short AnzianitaAnte0697GG { get { return _AnzianitaAnte0697GG; } set { _AnzianitaAnte0697GG = value; } }
            public short AnzianitaPost0697AA { get { return _AnzianitaPost0697AA; } set { _AnzianitaPost0697AA = value; } }
            public short AnzianitaPost0697MM { get { return _AnzianitaPost0697MM; } set { _AnzianitaPost0697MM = value; } }
            public short AnzianitaPost0697GG { get { return _AnzianitaPost0697GG; } set { _AnzianitaPost0697GG = value; } }


            public decimal MontanteQuotaDL214 { get { return _MontanteQuotaDL214; } set { _MontanteQuotaDL214 = value; } }
            public decimal ImportoContribTotaleQuotaDL214 { get { return _ImportoContribTotaleQuotaDL214; } set { _ImportoContribTotaleQuotaDL214 = value; } }
            public int NSettimaneQuotaDL214 { get { return _NSettimaneQuotaDL214; } set { _NSettimaneQuotaDL214 = value; } }
            public decimal QuotaContributivaAnnua { get { return _QuotaContributivaAnnua; } set { _QuotaContributivaAnnua = value; } }

            //Aggiunte in seguito allo sviluppo del fondo GAS. Da rimuovere se non utilizzate ad AggPECO
            public decimal MontanteEsclusivo { get { return _MontanteEsclusivo; } set { _MontanteEsclusivo = value; } }
            public decimal MontanteEsclusivoQuotaDL214 { get { return _MontanteEsclusivoQuotaDL214; } set { _MontanteEsclusivoQuotaDL214 = value; } }


            #endregion public properties
        }

        public class DatiRetributivi
        {
            #region private properties
            private decimal _RmsQuotaA;
            private decimal _RmsQuotaB;
            private decimal _RmsQuotaD;
            private int? _SettimaneA;
            private int _SettimaneA2;
            private int? _SettimaneB;
            private int? _SettimaneC;
            private int _SettimaneC2;
            private int? _SettimaneD;
            private decimal _RetribuzionePonderataAnnua;
            private decimal _RetribuzioneUltimoAnnoQuotaA;
            private decimal _RetribuzioneBiennio;
            //Aggiunte in seguito allo sviluppo del fondo GAS. Da rimuovere se non utilizzate ad AggPECO
            private int _NSettimaneEsclusiveQuotaA;
            private int _NSettimaneEsclusiveQuotaB;
            //comma 707
            private short? _QuotaA707;
            private short? _QuotaA2707;
            private short? _QuotaB707;
            private short? _QuotaC707;
            private short? _QuotaC2707;
            private short? _QuotaD707;
            private byte? _QuotaA707AA;
            private byte? _QuotaA707MM;
            private byte? _QuotaA707GG;
            private byte? _QuotaB707AA;
            private byte? _QuotaB707MM;
            private byte? _QuotaB707GG;
            private byte? _QuotaC707AA;
            private byte? _QuotaC707MM;
            private byte? _QuotaC707GG;
            private decimal? _RetribuzionePonderataAGO707;
            private short? _QuotaAES707;
            private short? _QuotaBES707;
            #endregion private properties

            #region public properties
            public decimal RmsQuotaA { get { return _RmsQuotaA; } set { _RmsQuotaA = value; } }
            public decimal RmsQuotaB { get { return _RmsQuotaB; } set { _RmsQuotaB = value; } }
            public decimal RmsQuotaD { get { return _RmsQuotaD; } set { _RmsQuotaD = value; } }
            public int? SettimaneA { get { return _SettimaneA; } set { _SettimaneA = value; } }
            public int SettimaneA2 { get { return _SettimaneA2; } set { _SettimaneA2 = value; } }
            public int? SettimaneB { get { return _SettimaneB; } set { _SettimaneB = value; } }
            public int? SettimaneC { get { return _SettimaneC; } set { _SettimaneC = value; } }
            public int SettimaneC2 { get { return _SettimaneC2; } set { _SettimaneC2 = value; } }
            public int? SettimaneD { get { return _SettimaneD; } set { _SettimaneD = value; } }
            public decimal RetribuzionePonderataAnnua { get { return _RetribuzionePonderataAnnua; } set { _RetribuzionePonderataAnnua = value; } }
            public decimal RetribuzioneUltimoAnnoQuotaA { get { return _RetribuzioneUltimoAnnoQuotaA; } set { _RetribuzioneUltimoAnnoQuotaA = value; } }
            public decimal RetribuzioneBiennio { get { return _RetribuzioneBiennio; } set { _RetribuzioneBiennio = value; } }

            //Aggiunte in seguito allo sviluppo del fondo GAS. Da rimuovere se non utilizzate ad AggPECO
            public int NSettimaneEsclusiveQuotaA { get { return _NSettimaneEsclusiveQuotaA; } set { _NSettimaneEsclusiveQuotaA = value; } }
            public int NSettimaneEsclusiveQuotaB { get { return _NSettimaneEsclusiveQuotaB; } set { _NSettimaneEsclusiveQuotaB = value; } }

            public int? NSettAnzianitaVV { get; set; }


            public short? QuotaA707 { get { return _QuotaA707; } set { _QuotaA707 = value; } }
            public short? QuotaA2707 { get { return _QuotaA2707; } set { _QuotaA2707 = value; } }
            public short? QuotaB707 { get { return _QuotaB707; } set { _QuotaB707 = value; } }
            public short? QuotaC707 { get { return _QuotaC707; } set { _QuotaC707 = value; } }
            public short? QuotaC2707 { get { return _QuotaC2707; } set { _QuotaC2707 = value; } }
            public short? QuotaD707 { get { return _QuotaD707; } set { _QuotaD707 = value; } }
            public byte? QuotaA707AA { get { return _QuotaA707AA; } set { _QuotaA707AA = value; } }
            public byte? QuotaA707MM { get { return _QuotaA707MM; } set { _QuotaA707MM = value; } }
            public byte? QuotaA707GG { get { return _QuotaA707GG; } set { _QuotaA707GG = value; } }
            public byte? QuotaB707AA { get { return _QuotaB707AA; } set { _QuotaB707AA = value; } }
            public byte? QuotaB707MM { get { return _QuotaB707MM; } set { _QuotaB707MM = value; } }
            public byte? QuotaB707GG { get { return _QuotaB707GG; } set { _QuotaB707GG = value; } }
            public byte? QuotaC707AA { get { return _QuotaC707AA; } set { _QuotaC707AA = value; } }
            public byte? QuotaC707MM { get { return _QuotaC707MM; } set { _QuotaC707MM = value; } }
            public byte? QuotaC707GG { get { return _QuotaC707GG; } set { _QuotaC707GG = value; } }
            public decimal? RetribuzionePonderataAGO707 { get { return _RetribuzionePonderataAGO707; } set { _RetribuzionePonderataAGO707 = value; } }
            public short? QuotaAES707 { get { return _QuotaAES707; } set { _QuotaAES707 = value; } }
            public short? QuotaBES707 { get { return _QuotaBES707; } set { _QuotaBES707 = value; } }

            #endregion public properties
        }

        public class DatiControllo
        {
            #region private properties
            private TipoCalcolo _TipoCalcolo;
            private bool _IsCalcoloValido;
            #endregion private properties

            #region public properties
            public TipoCalcolo TipoCalcolo { get { return _TipoCalcolo; } set { _TipoCalcolo = value; } }
            public bool IsCalcoloValido { get { return _IsCalcoloValido; } set { _IsCalcoloValido = value; } }
            #endregion public properties
        }
        #endregion nested class

        public enum TipoCalcolo
        {
            NonValido,
            Contributivo,
            Retributivo,
            Misto,
            RetributivoMonti
        };

    }
}
