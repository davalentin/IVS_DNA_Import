using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneControlli
    {
        #region DatiContributivi

        private static void ControlsTotaleNumeroSettimaneRetrib(int? settimaneA, int? settimaneB, int? settimaneC, int? settimaneD, out string messaggio, out bool IsCalcoloValid)
        {
            int totale = 0;
            messaggio = string.Empty;
            IsCalcoloValid = true;

            totale = settimaneA.GetValueOrDefault() + settimaneB.GetValueOrDefault() + settimaneC.GetValueOrDefault() + settimaneD.GetValueOrDefault();
            if (totale > 2080)
            {
                IsCalcoloValid = false;
                messaggio = "La somma delle quote delle settimane non deve essere superiore a 2080";
            }
            else
            {
                IsCalcoloValid = true;
                messaggio = string.Empty;
            }
        }

        private static void ControlsNumeroSingoleSettimaneRetr(GestionePensione.DatiPensione datiPensione, int? NsettimanaA, int? NsettimanaB, int? NsettimanaC, int? NsettimanaD, DateTime? InizioAssicurazione,
            DateTime? FineAssicurazione, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
            {
                //Quota A: il numero massimo di settimane ammesso è pari alla differenza dal 31/12/92 al primo versamento;
                DateTime dataLimite = new DateTime(1992, 12, 31);
                //aggiunta settimana di tolleranza come indicato da mail del 07-09-12
                //int settimaneAmmesse = (int)Math.Ceiling(((dataLimite.Year - InizioAssicurazione.Value.Year) * 12 +
                //        (dataLimite.Month - InizioAssicurazione.Value.Month) + 1) * 4.33333) + 1;
                int settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite, InizioAssicurazione.Value) + 1;

                if (settimaneAmmesse > 0)
                {
                    if (NsettimanaA.GetValueOrDefault() <= settimaneAmmesse)
                    {
                        //Quota B: il numero massimo di settimane ammesso è pari a 104;
                        if (NsettimanaB.GetValueOrDefault() <= 104)
                        {
                            //Quota C: il numero massimo di settimane ammesso è pari a 104;
                            if (NsettimanaC.GetValueOrDefault() <= 104)
                            {
                                //Quota D: il numero massimo di settimane ammesso è pari alla differenza fra l’ultimo versamento e il 01/01/97.
                                dataLimite = new DateTime(1997, 01, 01);
                                //settimaneAmmesse = (int)Math.Ceiling(((FineAssicurazione.Value.Year - dataLimite.Year) * 12 +
                                //    (FineAssicurazione.Value.Month - dataLimite.Month) + 1) * 4.33333);
                                settimaneAmmesse = Utility.NSettimaneBetweenDate(FineAssicurazione.Value, dataLimite);

                                //controllo su quota D va effettuato solo per Prodotto != 0012 quindi se pari a 0012 il calcolo è valido
                                if (settimaneAmmesse > 0)
                                {
                                    if (datiPensione.Prodotto == "0012" || NsettimanaD.GetValueOrDefault() <= settimaneAmmesse)
                                        IsCalcoloValid = true;
                                    else
                                    {
                                        IsCalcoloValid = false;
                                        //if (settimaneAmmesse < 0)
                                        //    messaggio = "La data 'Ultimo versamento' inserita nella tab ‘Dati Assicurativi’ del menu 'Liquidazione Pensione' non è compatibile con la 'Quota D'";
                                        //else
                                        messaggio = "Settimane quota D superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                                    }
                                }
                            }
                            else
                            {
                                IsCalcoloValid = false;
                                messaggio = "Settimane quota C superiori a 104";
                            }
                        }
                        else
                        {
                            IsCalcoloValid = false;
                            messaggio = "Settimane quota B superiori a 104";
                        }
                    }
                    else
                    {
                        IsCalcoloValid = false;
                        //if (settimaneAmmesse < 0)
                        //    messaggio = "La data 'Primo versamento' inserita nella tab 'Dati Assicurativi' del menu 'Liquidazione Pensione' non è compatibile con i 'Dati Calcolo' inseriti";
                        //else
                        messaggio = "Settimane quota A superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                    }
                }
            }
        }

        private static void ControlsNumeroSingoleSettimaneRetrMonti(GestionePensione.DatiPensione datiPensione, int? NsettimanaA, int? NsettimanaB, int? NsettimanaC, int? NsettimanaD, DateTime? InizioAssicurazione,
           char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            if (BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                return;

            //Quota A: il numero massimo di settimane ammesso è pari alla differenza dal 31/12/92 al primo versamento;
            DateTime dataLimite = new DateTime(1992, 12, 31);
            //aggiunta settimana di tolleranza come indicato da mail del 07-09-12
            //int settimaneAmmesse = (int)Math.Ceiling(((dataLimite.Year - InizioAssicurazione.Value.Year) * 12 +
            //        (dataLimite.Month - InizioAssicurazione.Value.Month) + 1) * 4.33333) + 1;
            int settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite, InizioAssicurazione.Value) + 1;

            if (settimaneAmmesse > 0)
            {
                if (NsettimanaA.GetValueOrDefault() <= settimaneAmmesse)
                {
                    //Quota B: il numero massimo di settimane ammesso è pari a 104;
                    if (NsettimanaB.GetValueOrDefault() <= 104)
                    {
                        //Quota C: il numero massimo di settimane ammesso è pari a 104;
                        if (NsettimanaC.GetValueOrDefault() <= 104)
                        {
                            //Quota D: il numero massimo di settimane ammesso è pari alla differenza fra il 31/12/2011 e il 01/01/97.
                            DateTime dataLimite1 = new DateTime(1997, 01, 01);
                            DateTime dataLimite2 = new DateTime(2011, 12, 31);
                            //settimaneAmmesse = (int)Math.Ceiling(((dataLimite2.Year - dataLimite1.Year) * 12 +
                            //    (dataLimite2.Month - dataLimite1.Month) + 1) * 4.33333);
                            settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite2, dataLimite1);

                            //controllo su quota D va effettuato solo per Prodotto != 0012 quindi se pari a 0012 il calcolo è valido
                            if (settimaneAmmesse > 0)
                            {
                                if (datiPensione.Prodotto == "0012" || NsettimanaD.GetValueOrDefault() <= settimaneAmmesse)
                                    IsCalcoloValid = true;
                                else
                                {
                                    IsCalcoloValid = false;
                                    //if (settimaneAmmesse < 0)
                                    //    messaggio = "La data 'Ultimo Versamento' inserita nella tab 'Dati Assicurativi'  del menu 'Liquidazione Pensione' non è compatibile con la 'Quota D'";
                                    //else
                                    messaggio = "Settimane quota D superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                                }
                            }
                        }
                        else
                        {
                            IsCalcoloValid = false;
                            messaggio = "Settimane quota C superiori a 104";
                        }
                    }
                    else
                    {
                        IsCalcoloValid = false;
                        messaggio = "Settimane quota B superiori a 104";
                    }
                }
                else
                {
                    IsCalcoloValid = false;
                    //if (settimaneAmmesse < 0)
                    //    messaggio = "La data 'Primo Versamento' inserita nella tab 'Dati Assicurativi' del menu 'Liquidazione Pensione' non è compatibile con i 'Dati Calcolo' inseriti";
                    //else
                    messaggio = "Settimane quota A superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                }
            }
        }

        private static void ControlsQuotaDMonti(decimal? RmsD, int? settimaneD, out string messaggioVideo, out bool IsCalcoloValid)
        {
            messaggioVideo = string.Empty;
            IsCalcoloValid = true;

            if (!RmsD.HasValue || RmsD == 0 || !settimaneD.HasValue || settimaneD == 0)
            {
                IsCalcoloValid = false;
                messaggioVideo = "Quota D mancante";
            }
        }

        public static void ControlsNumeroSingoleSettimaneDL407(int NsettimanaA, int NsettimanaB, int NsettimanaC, int NsettimanaD, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;
            int settimaneAmmesse = 260;
            if (NsettimanaA <= settimaneAmmesse)
            {
                if (NsettimanaB <= 104)
                {
                    if (NsettimanaC <= 104)
                    {
                        if (NsettimanaD <= settimaneAmmesse)
                            IsCalcoloValid = true;
                        else
                        {
                            IsCalcoloValid = false;
                            messaggio = "Settimane quota D superiori a 260";
                        }
                    }
                    else
                    {
                        IsCalcoloValid = false;
                        messaggio = "Settimane quota C superiori a 104";
                    }
                }
                else
                {
                    IsCalcoloValid = false;
                    messaggio = "Settimane quota B superiori a 104";
                }
            }
            else
            {
                IsCalcoloValid = false;
                messaggio = "Settimane quota A superiori a 260";
            }
        }

        #region Controls Obbligatorietà - Limite Settimane for Retributivo - Retributivo Monti - Misto - Misto Monti

        //contiene i controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti - Misto - MistoMonti
        private static void ControlsNSettimaneAllRetributivoAllMisto(GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, int? NsettimaneA1, int? NsettimaneA2, int? NsettimaneB, int? NsettimaneC1,
            DateTime? InizioAssicurazione, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            int settimaneLimiteA2 = 212;
            int settimaneLimiteB = 104;
            int settimaneLimiteC1 = 131;

            //Settimane A1: il numero massimo di settimane ammesso è pari alla differenza dal 27/11/88 al primo versamento;
            DateTime dataLimite1 = new DateTime(1988, 11, 27);
            //inserire una tolleranza sui controlli di capienza per le settimane A1, consentendo di inserire al massimo numero di settimane ammesse + 2. 
            // mail del 02/04/2014 - ERRORE TRASMISSIONE /UNICARPE
            int settimaneAmmesse1 = Utility.NSettimaneBetweenDate(dataLimite1, InizioAssicurazione.Value) + 2;


            if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio.Equals("02"))
            {
                //TipoBeneficio = AMIANTO
                settimaneLimiteA2 = 318;
                settimaneLimiteB = 156;
                settimaneLimiteC1 = 197;

                //10-12-2014 il numero settimane ammesse per la quaota A1 in presenza di beneficio amianto  sarà minore o uguale al 
                //numero di settimane che intercorrono tra la data inizio assicurazione e il 27/11/1988 moltiplicato per 1,5 e arrotondato per eccesso + 2
                settimaneAmmesse1 = ((int)Math.Ceiling(Utility.NSettimaneBetweenDate(dataLimite1, InizioAssicurazione.Value) * 1.5)) + 2;
            }

            if (NsettimaneA1.HasValue && NsettimaneA1.Value != 0 && Utility.DataStrettamenteSuccessivaA(InizioAssicurazione.Value, dataLimite1))
            {
                messaggio = "Settimane A1 non compatibili con la data di primo versamento";
                IsCalcoloValid = false;
                return;
            }

            if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
            {
                if (NsettimaneA1.HasValue && NsettimaneA1.Value != 0 && NsettimaneA1.Value > settimaneAmmesse1)
                {
                    messaggio = "Settimane quota A1 superiori al numero di settimane ammesse (" + settimaneAmmesse1 + ")";
                    IsCalcoloValid = false;
                    return;
                }
                if (NsettimaneA2.GetValueOrDefault() > settimaneLimiteA2)
                {
                    messaggio = "Settimane quota A2 superiori al numero di settimane ammesse (" + settimaneLimiteA2 + ")";
                    IsCalcoloValid = false;
                    return;
                }
                if (NsettimaneB.GetValueOrDefault() > settimaneLimiteB)
                {
                    messaggio = "Settimane quota B superiori al numero di settimane ammesse (" + settimaneLimiteB + ")";
                    IsCalcoloValid = false;
                    return;
                }
                if (NsettimaneC1.GetValueOrDefault() > settimaneLimiteC1)
                {
                    messaggio = "Settimane quota C1 superiori al numero di settimane ammesse (" + settimaneLimiteC1 + ")";
                    IsCalcoloValid = false;
                    return;
                }
            }
        }

        //contiene i controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti
        private static void ControlsNSettimaneAllRetributivo(GestionePensione.DatiPensione datiPensione, int? NsettimaneC2, DateTime? FineAssicurazione, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            if (BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                return;

            //Settimane D: il numero massimo di settimane ammesso è pari alla differenza dal 01/01/98 al primo versamento; (Retributivo)
            DateTime dataLimite2 = new DateTime(1998, 01, 01);
            //int settimaneAmmesse2 = (int)Math.Ceiling(((dataLimite2.Year - FineAssicurazione.Value.Year) * 12 +
            //        (dataLimite2.Month - FineAssicurazione.Value.Month) + 1) * 4.33333);

            int settimaneLimiteC2 = 26;

            if (tipoSettimaneBeneficio.Equals("02"))
                settimaneLimiteC2 = 39;

            if (NsettimaneC2 > settimaneLimiteC2)
            {
                messaggio = "Settimane quota C2 superiori al numero di settimane ammesse (" + settimaneLimiteC2 + ")";
                IsCalcoloValid = false;
            }
        }

        //contiene i controlli relativi al numero di settimane limite per: Retributivo
        private static void ControlsNSettimaneRetributivo(GestionePensione.DatiPensione datiPensione, int? NsettimaneD, DateTime? FineAssicurazione, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            if (BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                return;

            //Settimane D: il numero massimo di settimane ammesso è pari alla differenza dal 01/01/98 al primo versamento; (Retributivo)
            DateTime dataLimite2 = new DateTime(1998, 01, 01);
            //int settimaneAmmesse2 = (int)Math.Ceiling(((FineAssicurazione.Value.Year - dataLimite2.Year) * 12 +
            //        (FineAssicurazione.Value.Month - dataLimite2.Month) + 1) * 4.33333);
            int settimaneAmmesse2 = Utility.NSettimaneBetweenDate(FineAssicurazione.Value, dataLimite2);

            // Per beneficio amianto abbiamo un incremento del 50%
            if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio.Equals("02"))
                settimaneAmmesse2 = (int)Math.Ceiling(Utility.NSettimaneBetweenDate(FineAssicurazione.Value, dataLimite2) * 1.5);

            if (NsettimaneD > settimaneAmmesse2)
            {
                messaggio = "Settimane quota D superiori al numero di settimane ammesse (" + settimaneAmmesse2 + ")";
                IsCalcoloValid = false;
            }
        }

        //contiene i controlli relativi al numero di settimane limite per: RetributivoMonti
        private static void ControlsNSettimaneRetributivoMonti(GestionePensione.DatiPensione datiPensione, int? NsettimaneD, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            if (BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                return;

            //Settimane D: il numero massimo di settimane ammesso è pari alla differenza dal 01/01/98 al 31/12/2011; (RetributivoMonti)
            DateTime dataLimite1 = new DateTime(1998, 01, 01);
            DateTime dataLimite2 = new DateTime(2011, 12, 31);
            //int settimaneAmmesse = (int)Math.Ceiling(((dataLimite2.Year - dataLimite1.Year) * 12 +
            //        (dataLimite2.Month - dataLimite1.Month) + 1) * 4.33333);
            int settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite2, dataLimite1);

            // Per beneficio amianto abbiamo un incremento del 50%
            if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio.Equals("02"))
                settimaneAmmesse = (int)Math.Ceiling(Utility.NSettimaneBetweenDate(dataLimite2, dataLimite1) * 1.5);

            if (NsettimaneD > settimaneAmmesse)
            {
                messaggio = "Settimane quota D superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                IsCalcoloValid = false;
            }
        }

        //contiene i controlli relativi all'obbligatorietà per: Retributivo - RetributivoMonti - Misto - MistoMonti
        private static bool ControlsCalcoloAllRetributivoMonti_MistoMontiFondoVL(GestionePensione.DatiPensione datiPensione, GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, char? codiceSpecificoTraduzioneSuGP,
            string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            decimal rmsQuotaA = 0;
            decimal rmsQuotaB = 0;
            int? settimaneA = null;
            int settimaneA2 = 0;
            int? settimaneB = null;
            int? settimaneC = null;
            int settimaneC2 = 0;

            if (datiAggPeco != null && datiAggPeco.Retribuzione != null)
            {
                rmsQuotaA = datiAggPeco.Retribuzione.RmsQuotaA;
                rmsQuotaB = datiAggPeco.Retribuzione.RmsQuotaB;
                settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                settimaneA2 = datiAggPeco.Retribuzione.SettimaneA2;
                settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                settimaneC2 = datiAggPeco.Retribuzione.SettimaneC2;
            }

            // Per Inizio Assicurazione antecedente al 01/01/1993 è obbligatoria la quota A
            if (datiPensione.InizioAssicurazione.HasValue && !Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1993, 1, 1)) && rmsQuotaA == 0.0M)
            {
                messaggioVideo = "Retribuzione Media Settimanale Quota A obbligatorio";
                return false;
            }
            if (rmsQuotaA != 0.0M && (settimaneA.GetValueOrDefault() == 0 && settimaneA2 == 0))
            {
                messaggioVideo = "Settimane A1 o Settimane A2 obbligatori in presenza di Retribuzione Media Settimanale Quota A";
                return false;
            }
            // Per Inizio Assicurazione successiva al 01/01/1993 e Inizio Assicurazione antecedente al 01/01/1996 è obbligatoria la quota B
            if (datiPensione.InizioAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1993, 1, 1)) &&
                !Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1996, 1, 1)) &&
                rmsQuotaB == 0.0M)
            {
                messaggioVideo = "Retribuzione Media Settimanale Quota B obbligatorio";
                return false;
            }
            if (rmsQuotaB != 0.0M && (settimaneB.GetValueOrDefault() == 0 && settimaneC.GetValueOrDefault() == 0 && settimaneC2 == 0))
            {
                messaggioVideo = "Settimane B o Settimane C1 o Settimane C2 obbligatori in presenza di Retribuzione Media Settimanale Quota B";
                return false;
            }
            if (rmsQuotaB == 0.0M && (settimaneB.GetValueOrDefault() != 0 || settimaneC.GetValueOrDefault() != 0 || settimaneC2 != 0))
            {
                messaggioVideo = "Retribuzione Media Settimanale Quota B obbligatoria in presenza di Settimane B o Settimane C1 o Settimane C2";
                return false;
            }
            // Per Inizio Assicurazione successiva al 01/01/1995 e Inizio Assicurazione antecedente al 01/01/1996 è obbligatorio Settimane C1
            if (datiPensione.InizioAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1995, 1, 1)) &&
                !Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1996, 1, 1)) &&
                !settimaneC.HasValue)
            {
                messaggioVideo = "Settimane C1 obbligatorie.";
                return false;
            }

            if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
            {
                // Per Inizio Assicurazione successiva al 01/01/1995 e Inizio Assicurazione antecedente al 01/01/1996 è Settimane C1 non può essere maggiore di 52
                if (datiPensione.InizioAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1995, 1, 1)) &&
                    !Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1996, 1, 1)) &&
                    settimaneC.GetValueOrDefault() > 52M)
                {
                    messaggioVideo = "Settimane C1 non può essere maggiore di 52.";
                    return false;
                }
            }

            return true;
        }

        //contiene i controlli relativi all'obbligatorietà per: Retributivo - RetributivoMonti
        private static bool ControlsCalcoloAllRetributivoFondoVL(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int? settimaneD = null;
            decimal rmsQuotaD = 0;
            if (datiAggPeco != null && datiAggPeco.Retribuzione != null)
            {
                settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                rmsQuotaD = datiAggPeco.Retribuzione.RmsQuotaD;
            }

            if (rmsQuotaD != 0.0M && settimaneD.GetValueOrDefault() == 0 ||
                rmsQuotaD == 0.0M && settimaneD.GetValueOrDefault() != 0)
            {
                messaggioVideo = "Settimane D obbligatorio in presenza di Retribuzione Media Settimanale Quota D o viceversa.";
                return false;
            }
            return true;
        }

        #endregion Controls Obbligatorietà - Limite Settimane for Retributivo - Retributivo Monti - Misto - Misto Monti

        #region Limite Settimane Retributivo - Retributivo Monti for Fondo ET

        private static void ControlsNumeroSingoleSettimaneRetrFondoET(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtile, int? NsettimanaB, DateTime? InizioAssicurazione, DateTime? FineAssicurazione, char? codiceSpecificoTraduzioneSuGP,
            string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, Utility.TipoFondo? tipoFondo, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                return;

            if (BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                return;

            if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
            {
                foreach (GestioneDatiServizioUtile.ServizioUtile sUtile in lDatiServizioUtile)
                {
                    DateTime dataLimiteSup;
                    DateTime dataLimiteInf;
                    switch (sUtile.Quota)
                    {
                        case "A":
                            dataLimiteInf = InizioAssicurazione.Value;
                            dataLimiteSup = new DateTime(1992, 12, 31);
                            break;
                        case "B":
                            dataLimiteInf = new DateTime(1993, 1, 1);
                            dataLimiteSup = new DateTime(1994, 12, 31);
                            break;
                        case "C":
                            dataLimiteInf = new DateTime(1995, 1, 1);
                            dataLimiteSup = new DateTime(1995, 12, 31);
                            break;
                        default:
                            dataLimiteInf = DateTime.MinValue;
                            dataLimiteSup = DateTime.MinValue;
                            break;
                    }

                    Utility.DifferenzaDateTime diff = Utility.DifferenzaBetweenDate(dataLimiteSup.AddDays(1), dataLimiteInf, Utility.TipoAppartenenza.FS);
                    if (diff >= new Utility.DifferenzaDateTime(diff.Year, 6, 1))
                        diff = new Utility.DifferenzaDateTime(diff.Year + 1, 0, 0);

                    Utility.DifferenzaDateTime data = new Utility.DifferenzaDateTime(sUtile.ServizioUtileAA.GetValueOrDefault(), sUtile.ServizioUtileMM.GetValueOrDefault(), sUtile.ServizioUtileGG.GetValueOrDefault());
                    if (diff < data)
                    {
                        IsCalcoloValid = false;
                        messaggio = "Superata la capienza per la quota " + sUtile.Quota + " (" + diff.Year + " AA, " + diff.Month + " MM, " + diff.Day + " GG)";
                        return;
                    }
                }
            }

            DateTime dataLimite = new DateTime(2011, 12, 31);
            if (Utility.DataStrettamenteSuccessivaA(dataLimite, FineAssicurazione.Value))
                dataLimite = FineAssicurazione.Value;

            int settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite, new DateTime(1993, 01, 01));

            if (settimaneAmmesse > 0 && NsettimanaB.GetValueOrDefault() > settimaneAmmesse)
            {
                IsCalcoloValid = false;
                messaggio = "Settimane quota B superiori a " + settimaneAmmesse;
            }
        }

        private static void ControlsNumeroSingoleSettimaneRetrMontiFondoET(GestionePensione.DatiPensione datiPensione, int? NsettimaneB, List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile,
            char? codiceSpecifico, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            if (BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                return;

            int settimaneAmmesse = GetSettimaneMontiETAmmesse(datiPensione, listaDatiServizioUtile, codiceSpecifico);
            if (NsettimaneB.GetValueOrDefault() > settimaneAmmesse)
            {
                IsCalcoloValid = false;
                messaggio = "Settimane quota B superiori a " + settimaneAmmesse.ToString();
            }
        }

        #endregion Limite Settimane Retributivo - Retributivo Monti for Fondo ET

        public static bool CheckImportoWithControCodice(decimal? importo, int? controCodice, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoFondo? tipoFondo = null;
            Utility.CategoriaFondoPI? categoriaFondoPI = null;
            if (datiPensione != null)
            {
                tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            }

            string msg = string.Empty;
            switch (tipoFondo)
            {
                case Utility.TipoFondo.PI:
                case Utility.TipoFondo.PL:
                    msg = "Stipendio Annuo";
                    if (categoriaFondoPI.HasValue)
                    {
                        switch (categoriaFondoPI.Value)
                        {
                            case Utility.CategoriaFondoPI.U:
                                msg = "Elemento Retributivo + Stipendio Base + Pens.Compl.Riv.1/95";
                                break;
                        }
                    }
                    break;
                case Utility.TipoFondo.EL:
                case Utility.TipoFondo.ET:
                case Utility.TipoFondo.ES:
                case Utility.TipoFondo.GAS:
                case Utility.TipoFondo.TT:
                    msg = "Retribuzione Pensionabile";
                    break;
                case Utility.TipoFondo.DZ:
                    msg = "Retribuzione";
                    break;
                case Utility.TipoFondo.VL:
                    msg = "Retrib. Pens. Annua";
                    break;
            }

            int? result = importo.HasValue ? Convert.ToInt32(Math.Floor(importo.Value) % 999) : (int?)null;

            if (importo.HasValue && importo.Value < 1000)
            {
                if (controCodice.HasValue && !controCodice.Value.Equals((int)importo.Value))
                {
                    messaggioVideo = "Il campo 'Controcodice Retribuzione' deve coincidere con il campo '" + msg + "' per valori inferiori a 1000 (" + importo + ")";
                    return false;
                }
            }
            else
            {
                if (controCodice.HasValue && result.HasValue && !controCodice.Value.Equals(result.Value))
                {
                    messaggioVideo = "Il campo 'Controcodice Retribuzione' deve essere uguale alle prime tre cifre decimali del risultato della divisione tra '" + msg + "' e 999";/* per valori superiori a 1000 (" + result + ")" Commentato in seguito richiesta Sabina 26-12-2015*/
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica che, in base al tipo Calcolo, siano valorizzati correttamente i dati
        /// </summary>
        /// <param name="tipoCalcolo">Tipo Calcolo</param>
        /// <param name="datiContributivi">Dati Cotnributivi</param>
        /// <param name="datiRetributivi">Dati Retributivi</param>
        /// <returns>true se le condizioni sono verificate, false altrimenti</returns>
        public static bool VerificaDatiContributiviTipoCalcolo(Utility.TipoCalcolo tipoCalcolo, GestioneCalcolo.DatiCalcoloContributivo datiContributivi,
                                                               GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, List<GestioneDatiServizioUtile.ServizioUtile> lstServizioUtile,
                                                               List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi)
        {
            switch (tipoCalcolo)
            {
                case Utility.TipoCalcolo.Retributivo: //retributivo
                    if ((datiRetributivi != null || (lstServizioUtile != null && lstServizioUtile.Count > 0) || (listaDatiRetributivi != null && listaDatiRetributivi.Count > 0)) && (datiContributivi == null || datiContributivi.IsDatiCalcoloContributivoNull()))
                        return true;
                    break;
                case Utility.TipoCalcolo.Contributivo: //contributivo
                    if (datiContributivi != null && (datiRetributivi == null || datiRetributivi.IsDatiCalcoloRetributivoNull() || lstServizioUtile == null || lstServizioUtile.Count == 0 || listaDatiRetributivi == null || listaDatiRetributivi.Count == 0))
                        return true;
                    break;
                case Utility.TipoCalcolo.Misto: //misto
                case Utility.TipoCalcolo.RetributivoMonti: //retributivo Monti
                    if ((datiRetributivi != null || (lstServizioUtile != null && lstServizioUtile.Count > 0) || (listaDatiRetributivi != null && listaDatiRetributivi.Count > 0)) && datiContributivi != null)
                        return true;
                    break;
                default:
                    return true;
            }

            return false;
        }

        public static bool VerificaDimensioneImportoContributivo(GestioneCalcolo.DatiCalcoloContributivo datiContributivi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiContributivi != null)
            {
                if ((datiContributivi.ImportoContributivoTotale.HasValue && datiContributivi.ImportoContributivoTotale.Value > 9999999.9999M) ||
                    (datiContributivi.ImportoContribTotaleQuotaDL214.HasValue && datiContributivi.ImportoContribTotaleQuotaDL214.Value > 9999999.9999M))
                {
                    messaggioVideo = "Il campo 'Importo Contributivo Totale' in 'Dati Contributivi' è maggiore di 999999,9999";
                    return false;
                }
            }
            return true;
        }

        #region calcolo contributivo

        public static bool ControlsCalcoloContributivo(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, GestionePensione.DatiPensione datiPensione, char? codiceSpecifico, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // contributivo classico
            if (!datiPensione.FineAssicurazione.HasValue || DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) <= 0)
            {
                if (!ControlsCalcoloContributivoL335(datiAggPeco, datiPensione, true, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                    return false;
            }
            else // contributivo monti
            {
                if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                {
                    if (!ControlsCalcoloContributivoL335(datiAggPeco, datiPensione, false, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                        return false;

                    if (!ControlsCalcoloContributivoL214(datiAggPeco, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                        return false;
                }
            }
            return true;
        }

        public static bool ControlsSettimaneContributiveL214(GestionePensione.DatiPensione datiPensione, DateTime? FineAssicurazione, int? NSettimaneQuotaDL214, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dtMaxL214 = new DateTime(2011, 12, 31);

            if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
            {
                if (FineAssicurazione.HasValue && DateTime.Compare(FineAssicurazione.Value, dtMaxL214) > 0)
                {
                    //int NsettimaneAmmesseL214 = (int)Math.Ceiling(((FineAssicurazione.Value.Year - dtMaxL214.Year) * 12 + (FineAssicurazione.Value.Month - dtMaxL214.Month) + 1) * 4.33333);
                    int NsettimaneAmmesseL214 = Utility.NSettimaneBetweenDate(FineAssicurazione.Value, dtMaxL214);
                    if (NSettimaneQuotaDL214 > NsettimaneAmmesseL214)
                    {
                        messaggioVideo = "Numero settimane contributive L.214 superiore al numero di settimane ammesse (" + NsettimaneAmmesseL214 + ")";
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool ControlsCalcoloContributivo(GestioneCalcolo.DatiCalcoloContributivo datiContributivi, GestionePensione.DatiPensione datiPensione, char? codiceSpecifico, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // contributivo classico
            if (!datiPensione.FineAssicurazione.HasValue || DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) <= 0)
            {
                if (datiContributivi != null && datiContributivi.Montante.HasValue && datiContributivi.ImportoContributivoTotale.HasValue)
                {
                    if (!ControlsCalcoloContributivoL335(datiContributivi, datiPensione, true, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                        return false;
                }
            }
            else // contributivo monti
            {
                if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                {
                    if (!ControlsCalcoloContributivoL214(datiContributivi, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                        return false;
                }
            }
            return true;
        }

        private static bool ControlsCalcoloContributivoL214(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, GestionePensione.DatiPensione datiPensione, char? codiceSpecifico, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
            {
                // Con decorrenza >= 01/2015 non dovrà essere verificato il controllo sotto. 
                // Rif. mail del 23/02/2015 con oggetto "LIQPENS - attività"
                if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
                {
                    if (datiAggPeco.Contribuzione.MontanteQuotaDL214 < datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214)
                    {
                        messaggioVideo = "Montante L.214 inferiore all'importo contributivo totale";
                        return false;
                    }
                }

                if (!ControlsSettimaneContributiveL214(datiPensione, datiAggPeco.DatiParziali.FineAssicurazione.Value, datiAggPeco.Contribuzione.NSettimaneQuotaDL214, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto,
                    maggiorazioneInv74, out messaggioVideo))
                    return false;
            }
            return true;
        }

        private static bool ControlsCalcoloContributivoL214(GestioneCalcolo.DatiCalcoloContributivo datiContributivi, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
            {
                if (datiContributivi.MontanteQuotaDL214 < datiContributivi.ImportoContribTotaleQuotaDL214)
                {
                    messaggioVideo = "Montante L.214 inferiore all'importo contributivo totale";
                    return false;
                }

                if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                    if (!ControlsSettimaneContributiveL214(datiPensione, datiPensione.FineAssicurazione, datiContributivi.NSettimaneQuotaDL214, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto,
                        maggiorazioneInv74, out messaggioVideo))
                        return false;
            }
            return true;
        }

        private static bool ControlsCalcoloContributivoL335(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, GestionePensione.DatiPensione datiPensione, bool isContribPuro, char? codiceSpecifico, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            // Con decorrenza >= 01/2015 non dovrà essere verificato il controllo sotto. 
            // Rif. mail del 23/02/2015 con oggetto "LIQPENS - attività"
            if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
            {
                //reso minore per segnalazione 07-09-2012
                if (datiAggPeco.Contribuzione.Montante < datiAggPeco.Contribuzione.ImportoContributivoTotale)
                {
                    messaggioVideo = "Montante inferiore all'importo contributivo totale";
                    return false;
                }
            }
            if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
            {
                int settimaneAmmesse = GetSettimaneAmmesseForContributivo(datiPensione, isContribPuro, datiAggPeco.DatiParziali.InizioAssicurazione.GetValueOrDefault(), datiAggPeco.DatiParziali.FineAssicurazione.GetValueOrDefault());
                if (datiAggPeco.Contribuzione.Settimane > settimaneAmmesse)
                {
                    messaggioVideo = "Numero settimane contributive L.335 superiore al numero di settimane ammesse (" + settimaneAmmesse + ")";
                    return false;
                }
            }
            return true;
        }

        private static bool ControlsCalcoloContributivoL335(GestioneCalcolo.DatiCalcoloContributivo datiContributivi, GestionePensione.DatiPensione datiPensione, bool isContribPuro, char? codiceSpecifico, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            // Con decorrenza >= 01/2015 non dovrà essere verificato il controllo sotto. 
            // Rif. mail del 23/02/2015 con oggetto "LIQPENS - attività"
            if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
            {
                //reso minore per segnalazione 07-09-2012
                if (datiContributivi.Montante < datiContributivi.ImportoContributivoTotale)
                {
                    messaggioVideo = "Montante inferiore all'importo contributivo totale";
                    return false;
                }
            }

            if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
            {
                int settimaneAmmesse = GetSettimaneAmmesseForContributivo(datiPensione, isContribPuro, datiPensione.InizioAssicurazione.GetValueOrDefault(), datiPensione.FineAssicurazione.GetValueOrDefault());

                if (datiContributivi.NSettimane > settimaneAmmesse)
                {
                    messaggioVideo = "Numero settimane contributive L.335 superiore al numero di settimane ammesse (" + settimaneAmmesse + ")";
                    return false;
                }
            }
            return true;
        }

        public static bool ControlsCalcoloContributivoFondoVL(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, GestionePensione.DatiPensione datiPensione, char? codiceSpecifico, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // contributivo classico
            if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && !ControlsCalcoloContributivoL335FondoVL(datiAggPeco, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
            {
                if (!ControlsCalcoloContributivoL214(datiAggPeco, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                    return false;
            }

            return true;
        }

        private static int GetSettimaneAmmesseForContributivo(GestionePensione.DatiPensione datiPensione, bool isContribPuro, DateTime? inizioAssicurazione, DateTime? fineAssicurazione)
        {
            int settimaneAmmesse = 0;
            if (isContribPuro)
            {
                if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione))
                    settimaneAmmesse = Utility.NSettimaneBetweenDate(fineAssicurazione.Value, inizioAssicurazione.Value);
                else
                    settimaneAmmesse = Utility.NSettimaneBetweenDate(fineAssicurazione.Value, new DateTime(1995, 12, 31));
            }
            else
            {
                if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione))
                    settimaneAmmesse = Utility.NSettimaneBetweenDate(new DateTime(2011, 12, 31), inizioAssicurazione.Value);
                else
                    settimaneAmmesse = Utility.NSettimaneBetweenDate(new DateTime(2011, 12, 31), new DateTime(1995, 12, 31));
            }
            return settimaneAmmesse;
        }

        private static bool ControlsCalcoloContributivoFondoVLForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloContributivo datiContributivi, GestionePensione.DatiPensione datiPensione,
            char? codiceSpecifico, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // contributivo classico
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && !ControlsCalcoloContributivoL335FondoVLForMaggiorazioneBenefici(datiContributivi, out messaggioVideo))
                return false;

            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
            {
                if (!ControlsCalcoloContributivoL214(datiContributivi, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                    return false;
            }
            return true;
        }

        private static bool ControlsCalcoloContributivoL335FondoVL(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            decimal? montante = datiAggPeco.Contribuzione.Montante + datiAggPeco.Contribuzione.MontanteAnte0697;

            // Con decorrenza >= 01/2015 non dovrà essere verificato il controllo sotto. 
            // Rif. mail del 23/02/2015 con oggetto "LIQPENS - attività"
            if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
            {
                if (montante.HasValue && montante.Value < datiAggPeco.Contribuzione.ImportoContributivoTotale)
                {
                    messaggioVideo = "Totale Montante L.335 inferiore all'importo contributivo totale";
                    return false;
                }
            }

            if (datiAggPeco.Contribuzione.MontanteAnte0697 != 0.0M && datiAggPeco.Contribuzione.AnzianitaAnte0697AA == 0 &&
                datiAggPeco.Contribuzione.AnzianitaAnte0697MM == 0 && datiAggPeco.Contribuzione.AnzianitaAnte0697GG == 0)
            {
                messaggioVideo = "Anzianità 01/96 a 06/97 L.335 obbligatoria in presenza del Montante da 01/96 a 06/97";
                return false;
            }

            if (datiAggPeco.Contribuzione.Montante != 0.0M && datiAggPeco.Contribuzione.AnzianitaPost0697AA == 0 &&
                datiAggPeco.Contribuzione.AnzianitaPost0697MM == 0 && datiAggPeco.Contribuzione.AnzianitaPost0697GG == 0)
            {
                messaggioVideo = "Anzianità dal 07/97 L.335 obbligatoria in presenza del Montante dal 07/97";
                return false;
            }

            return true;
        }

        private static bool ControlsCalcoloContributivoL335FondoVLForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloContributivo datiContributivi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            decimal? montante = datiContributivi.Montante + datiContributivi.MontanteAnte0697;

            if (montante.HasValue && montante.Value < datiContributivi.ImportoContributivoTotale)
            {
                messaggioVideo = "Totale Montante L.335 inferiore all'importo contributivo totale";
                return false;
            }

            if (datiContributivi.MontanteAnte0697.HasValue && !datiContributivi.AnzianitaAnte0697AA.HasValue &&
                !datiContributivi.AnzianitaAnte0697MM.HasValue && !datiContributivi.AnzianitaAnte0697GG.HasValue)
            {
                messaggioVideo = "Anzianità 01/96 a 06/97 L.335 obbligatoria in presenza del Montante da 01/96 a 06/97";
                return false;
            }

            if (datiContributivi.Montante.HasValue && !datiContributivi.AnzianitaPost0697AA.HasValue &&
                !datiContributivi.AnzianitaPost0697MM.HasValue && !datiContributivi.AnzianitaPost0697GG.HasValue)
            {
                messaggioVideo = "Anzianità dal 07/97 L.335 obbligatoria in presenza del Montante dal 07/97";
                return false;
            }

            return true;
        }

        private static bool ControlsCalcoloContributivoL214FondoGAS(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
            {
                messaggioVideo = "Montante L.214 obbligatorio";
                return false;
            }
            if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
            {
                messaggioVideo = "N settimane L.214 obbligatorio";
                return false;
            }

            return true;
        }

        private static bool ControlsCalcoloContributivoL214FondoDZ(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 == 0)
            {
                messaggioVideo = "Importo Contributivo Totale L.214 obbligatorio";
                return false;
            }
            if (datiAggPeco.Contribuzione.MontanteQuotaDL214 == 0.0M)
            {
                messaggioVideo = "Montante L.214 obbligatorio";
                return false;
            }
            if (datiAggPeco.Contribuzione.NSettimaneQuotaDL214 == 0)
            {
                messaggioVideo = "N settimane L.214 obbligatorio";
                return false;
            }

            return true;
        }

        public static bool ControlsCalcoloContributivoFondoFS_PT(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, DateTime? decorrenzaOriginaria, GestionePensione.DatiPensione datiPensione,
            bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            if (!(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && !Utility.IsRiaperturaDomanda(datiPensione.Id)))
            {
                if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    if (datiAggPeco.Contribuzione.Montante == 0.0M)
                    {
                        messaggioVideo = "Montante obbligatorio";
                        return false;
                    }

                if (datiAggPeco.Contribuzione.ImportoContributivoTotale == 0.0M)
                {
                    messaggioVideo = "Importo Contributivo Totale obbligatorio";
                    return false;
                }
            }

            // Con decorrenza >= 01/2015 non dovrà essere verificato il controllo sotto. 
            // Rif. mail del 23/02/2015 con oggetto "LIQPENS - attività"
            if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2015, 1, 1)) && !(Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica))
            {
                if (datiAggPeco.Contribuzione.Montante < datiAggPeco.Contribuzione.ImportoContributivoTotale)
                {
                    messaggioVideo = "Montante inferiore all'importo contributivo totale";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsCalcoloContributivoFondoGAS(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiAggPeco.Contribuzione.Montante == 0.0M)
            {
                messaggioVideo = "Montante totale obbligatorio";
                return false;
            }

            if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
            {
                if (!ControlsCalcoloContributivoL214FondoGAS(datiAggPeco, out messaggioVideo))
                    return false;
            }

            return true;
        }

        #endregion calcolo contributivo

        #region calcolo retributivo

        #region Fondo EL - TT

        public static bool ControlsCalcoloRetributivo(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, char? codiceSpecifico, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            int? settimaneA = null;
            int? settimaneB = null;
            int? settimaneC = null;
            int? settimaneD = null;
            DateTime? inizioAssicurazione = null;
            DateTime? fineAssicurazione = null;
            decimal retribuzionePonderataAnnua = 0;
            if (datiAggPeco != null)
            {
                if (datiAggPeco.Retribuzione != null)
                {
                    settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                    settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                    settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                    settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                    retribuzionePonderataAnnua = datiAggPeco.Retribuzione.RetribuzionePonderataAnnua;
                }

                if (datiAggPeco.DatiParziali != null)
                {
                    inizioAssicurazione = datiAggPeco.DatiParziali.InizioAssicurazione;
                    fineAssicurazione = datiAggPeco.DatiParziali.FineAssicurazione;
                }
            }

            if ((!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03"))) ||
                (codiceSpecifico == 'Q'))
                GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(settimaneA, settimaneB, settimaneC, settimaneD, out messaggioVideo, out IsCalcoloValid);
            else
                GestioneControlli.ControlsNumeroSingoleSettimaneRetr(datiPensione, settimaneA, settimaneB, settimaneC, settimaneD, inizioAssicurazione,
                                                                     fineAssicurazione, codiceSpecifico, tipoSettimaneBeneficio,
                                                                     maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            //aggiunto controllo su RetribuzionePonderataAnnua
            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC) &&
                !ControlsRetrPonderataAnnuaObbligatoria(retribuzionePonderataAnnua, datiPensione, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsCalcoloRetributivoForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, Entity.DatiBenefici datiBenefici,
            GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = string.Empty;
            if (datiBenefici != null)
                tipoSettimaneBeneficio = datiBenefici.TipoSettimaneBeneficio;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Benefici_FS.DOPPIO_BENEFICIO_CON_QUOTA100) ||
                (datiBenefici != null && (!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03")))))
            {
                if (datiRetributivi != null)
                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                        datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                        datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                        datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0,
                        out messaggioVideo, out IsCalcoloValid);
            }
            else
            {
                if (datiRetributivi != null)
                    GestioneControlli.ControlsNumeroSingoleSettimaneRetr(datiPensione,
                        datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                        datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                        datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                        datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0,
                        datiPensione.InizioAssicurazione,
                        datiPensione.FineAssicurazione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74,
                        out messaggioVideo, out IsCalcoloValid);
            }
            if (!IsCalcoloValid)
                return false;

            return true;
        }

        #endregion Fondo EL - TT

        #region Fondo VL

        public static bool ControlsCalcoloRetributivoFondoVL(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }
            int? settimaneA = null;
            int? settimaneA2 = null;
            int? settimaneB = null;
            int? settimaneC = null;
            int settimaneC2 = 0;
            int? settimaneD = null;
            DateTime? inizioAssicurazione = null;
            DateTime? fineAssicurazione = null;
            if (datiAggPeco != null)
            {
                if (datiAggPeco.Retribuzione != null)
                {
                    settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                    settimaneA2 = datiAggPeco.Retribuzione.SettimaneA2;
                    settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                    settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                    settimaneC2 = datiAggPeco.Retribuzione.SettimaneC2;
                    settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                }

                if (datiAggPeco.DatiParziali != null)
                {
                    inizioAssicurazione = datiAggPeco.DatiParziali.InizioAssicurazione;
                    fineAssicurazione = datiAggPeco.DatiParziali.FineAssicurazione;
                }
            }

            //obbligatorietà per: Retributivo - RetributivoMonti - Misto - MistoMonti
            if (!ControlsCalcoloAllRetributivoMonti_MistoMontiFondoVL(datiPensione, datiAggPeco, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            //obbligatorietà per: Retributivo - RetributivoMonti
            if (!ControlsCalcoloAllRetributivoFondoVL(datiAggPeco, out messaggioVideo))
                return false;

            //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti - Misto - MistoMonti
            ControlsNSettimaneAllRetributivoAllMisto(datiPensione, codiceSpecificoTraduzioneSuGP, settimaneA, settimaneA2, settimaneB,
                settimaneC, inizioAssicurazione, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti
            ControlsNSettimaneAllRetributivo(datiPensione, settimaneC2, fineAssicurazione, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto,
                maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            //controlli relativi al numero di settimane limite per: Retributivo
            ControlsNSettimaneRetributivo(datiPensione, settimaneD, fineAssicurazione, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto,
                maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            return true;
        }

        public static bool ControlsCalcoloRetributivoFondoVLForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, Entity.DatiBenefici datiBenefici, GestionePensione.DatiPensione datiPensione,
            char? codiceSpecificoTraduzioneSuGP, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = datiBenefici.TipoSettimaneBeneficio;

            if (datiRetributivi != null)
            {
                //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti - Misto - MistoMonti
                ControlsNSettimaneAllRetributivoAllMisto(datiPensione, codiceSpecificoTraduzioneSuGP,
                    datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                                                         datiRetributivi.NSettimaneQuotaA2.HasValue ? datiRetributivi.NSettimaneQuotaA2.Value : 0,
                                                         datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                                                         datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                                                         datiPensione.InizioAssicurazione, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti
                ControlsNSettimaneAllRetributivo(datiPensione, datiRetributivi.NSettimaneQuotaC2.HasValue ? datiRetributivi.NSettimaneQuotaC2.Value : 0,
                                                         datiPensione.FineAssicurazione, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                //controlli relativi al numero di settimane limite per: Retributivo
                ControlsNSettimaneRetributivo(datiPensione, datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0,
                                                         datiPensione.FineAssicurazione, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;
            }

            return true;
        }

        #endregion Fondo VL

        #region Fondo ET

        public static bool ControlsCalcoloRetributivoFondoET(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            char? codiceSpecifico, List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtile, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;
            bool isDatiCalcoloRetrib = lDatiServizioUtile != null && lDatiServizioUtile.Count > 0 && lDatiServizioUtile.Exists(x => x.Quota == "A" || x.Quota == "B");

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            int? settimaneA = null;
            int? settimaneB = null;
            int? settimaneC = null;
            int? settimaneD = null;
            decimal? retribuzionePonderataAnnua = null;
            DateTime? inizioAssicurazione = null;
            DateTime? fineAssicurazione = null;
            if (datiAggPeco != null)
            {
                if (datiAggPeco.Retribuzione != null)
                {
                    settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                    settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                    settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                    settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                    retribuzionePonderataAnnua = datiAggPeco.Retribuzione.RetribuzionePonderataAnnua;
                }
                if (datiAggPeco.DatiParziali != null)
                {
                    inizioAssicurazione = datiAggPeco.DatiParziali.InizioAssicurazione;
                    fineAssicurazione = datiAggPeco.DatiParziali.FineAssicurazione;
                }
            }

            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if ((!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03"))) ||
                (codiceSpecifico == 'Q'))
                GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(settimaneA, settimaneB, settimaneC, settimaneD, out messaggioVideo, out IsCalcoloValid);
            else
                GestioneControlli.ControlsNumeroSingoleSettimaneRetrFondoET(datiPensione, datiDanteCausa, lDatiServizioUtile, settimaneB, inizioAssicurazione, fineAssicurazione,
                    codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, tipoFondo,
                    out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            //aggiunto controllo su RetribuzionePonderataAnnua
            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC) &&
                isDatiCalcoloRetrib && !ControlsRetrPonderataAnnuaObbligatoria(retribuzionePonderataAnnua, datiPensione, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsCalcoloRetributivoFondoETForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, Entity.DatiBenefici datiBenefici,
            GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtile, char? codiceSpecificoTraduzioneSuGP,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiRetributivi);

            string tipoSettimaneBeneficio = string.Empty;
            if (datiBenefici != null)
                tipoSettimaneBeneficio = datiBenefici.TipoSettimaneBeneficio;

            if (datiBenefici != null && (!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03"))))
            {
                if (datiRetributivi != null)
                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                        datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                        datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                        datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0,
                        out messaggioVideo, out IsCalcoloValid);
            }
            else
            {
                if (datiRetributivi != null)
                    GestioneControlli.ControlsNumeroSingoleSettimaneRetrFondoET(datiPensione, datiDanteCausa, lDatiServizioUtile,
                        datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                        datiPensione.InizioAssicurazione,
                        datiPensione.FineAssicurazione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, tipoFondo,
                        out messaggioVideo, out IsCalcoloValid);
            }
            if (!IsCalcoloValid)
                return false;

            return true;
        }

        #endregion Fondo ET

        #region Fondo GAS
        public static bool ControlsCalcoloRetributivoFondoGAS(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            decimal rmsQuotaA = 0;
            decimal rmsQuotaB = 0;
            int? settimaneA = null;
            int? settimaneB = null;
            if (datiAggPeco != null && datiAggPeco.Retribuzione != null)
            {
                rmsQuotaA = datiAggPeco.Retribuzione.RmsQuotaA;
                rmsQuotaB = datiAggPeco.Retribuzione.RmsQuotaB;
                settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                settimaneB = datiAggPeco.Retribuzione.SettimaneB;
            }

            if (rmsQuotaA == 0.0M && rmsQuotaB == 0.0M)
            {
                messaggioVideo = "RMS obbligatorio";
                return false;
            }

            if (settimaneA.GetValueOrDefault() == 0 && settimaneB.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Settimane totali obbligatorie";
                return false;
            }

            return true;
        }
        #endregion Fondo GAS

        #region Fondo DZ
        public static bool ControlsCalcoloRetributivoFondoDZ(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            decimal rmsQuotaA = 0;
            decimal rmsQuotaB = 0;
            int? settimaneA = null;
            int? settimaneB = null;
            if (datiAggPeco != null && datiAggPeco.Retribuzione != null)
            {
                rmsQuotaA = datiAggPeco.Retribuzione.RmsQuotaA;
                rmsQuotaB = datiAggPeco.Retribuzione.RmsQuotaB;
                settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                settimaneB = datiAggPeco.Retribuzione.SettimaneB;
            }

            if (rmsQuotaA == 0.0M && rmsQuotaB == 0.0M)
            {
                messaggioVideo = "RMS obbligatorio";
                return false;
            }

            if (settimaneA.GetValueOrDefault() == 0 && settimaneB.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Settimane totali obbligatorie";
                return false;
            }

            return true;
        }
        #endregion Fondo DZ

        #endregion calcolo retributivo

        #region calcolo RetributivoMonti

        #region Fondo EL - TT

        public static bool ControlsCalcoloRetributivoMonti(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestionePensione.DatiPensione datiPensione, char? codiceSpecifico, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;
            string tipoSettimaneBeneficio = null;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;

            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            if (datiAggPeco.Retribuzione != null)
            {
                GestioneControlli.ControlsQuotaDMonti(datiAggPeco.Retribuzione.RmsQuotaD, datiAggPeco.Retribuzione.SettimaneD, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                if ((datiMaggiorazioniBenefici != null && (!String.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio) &&
                    (datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("01") || datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("02") || datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("03")))) ||
                    (codiceSpecifico == 'Q'))
                {
                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(datiAggPeco.Retribuzione.SettimaneA, datiAggPeco.Retribuzione.SettimaneB, datiAggPeco.Retribuzione.SettimaneC,
                                                                          datiAggPeco.Retribuzione.SettimaneD, out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }
                else
                {
                    GestioneControlli.ControlsNumeroSingoleSettimaneRetrMonti(datiPensione, datiAggPeco.Retribuzione.SettimaneA, datiAggPeco.Retribuzione.SettimaneB,
                                                                              datiAggPeco.Retribuzione.SettimaneC, datiAggPeco.Retribuzione.SettimaneD,
                                                                              datiAggPeco.DatiParziali.InizioAssicurazione, codiceSpecifico, tipoSettimaneBeneficio,
                                                                              maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }

                //aggiunto controllo su RetribuzionePonderataAnnua
                if (!ControlsRetrPonderataAnnuaObbligatoria(datiAggPeco.Retribuzione.RetribuzionePonderataAnnua, datiPensione, out messaggioVideo))
                    return false;
            }
            if (!ControlsCalcoloContributivoL214(datiAggPeco, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
            {
                IsCalcoloValid = false;
                return false;
            }
            return true;
        }

        public static bool ControlsCalcoloRetributivoMontiForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, GestioneCalcolo.DatiCalcoloContributivo datiContributivi,
            Entity.DatiBenefici datiBenefici, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = string.Empty;
            if (datiBenefici != null)
                tipoSettimaneBeneficio = datiBenefici.TipoSettimaneBeneficio;

            if (datiRetributivi != null)
            {
                GestioneControlli.ControlsQuotaDMonti(datiRetributivi.RMSQuotaD, datiRetributivi.NSettimaneQuotaD, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Benefici_FS.DOPPIO_BENEFICIO_CON_QUOTA100) ||
                    (datiBenefici != null && (!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                    (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03")))))
                {

                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                                                                          datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                                                                          datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                                                                          datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0,
                                                                          out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }
                else
                {
                    GestioneControlli.ControlsNumeroSingoleSettimaneRetr(datiPensione, datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                                                                         datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                                                                         datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                                                                         datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0,
                                                                         datiPensione.InizioAssicurazione,
                                                                         datiPensione.FineAssicurazione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74,
                                                                         out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }
            }

            if (!ControlsCalcoloContributivoL214(datiContributivi, datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
            {
                IsCalcoloValid = false;
                return false;
            }

            return true;
        }

        #endregion Fondo EL - TT

        #region Fondo VL

        public static bool ControlsCalcoloRetributivoMontiFondoVL(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            int? settimaneA = null;
            int settimaneA2 = 0;
            int? settimaneB = null;
            int? settimaneC = null;
            int settimaneC2 = 0;
            int? settimaneD = null;
            DateTime? inizioAssicurazione = null;
            DateTime? fineAssicurazione = null;
            if (datiAggPeco != null)
            {
                if (datiAggPeco.Retribuzione != null)
                {
                    settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                    settimaneA2 = datiAggPeco.Retribuzione.SettimaneA2;
                    settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                    settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                    settimaneC2 = datiAggPeco.Retribuzione.SettimaneC2;
                    settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                }
                if (datiAggPeco.DatiParziali != null)
                {
                    inizioAssicurazione = datiAggPeco.DatiParziali.InizioAssicurazione;
                    fineAssicurazione = datiAggPeco.DatiParziali.FineAssicurazione;
                }
            }

            //obbligatorietà per: Retributivo - RetributivoMonti - Misto - MistoMonti
            if (!ControlsCalcoloAllRetributivoMonti_MistoMontiFondoVL(datiPensione, datiAggPeco, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            //obbligatorietà per: Retributivo - RetributivoMonti
            if (!ControlsCalcoloAllRetributivoFondoVL(datiAggPeco, out messaggioVideo))
                return false;

            //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti - Misto - MistoMonti
            ControlsNSettimaneAllRetributivoAllMisto(datiPensione, codiceSpecificoTraduzioneSuGP, settimaneA, settimaneA2, settimaneB, settimaneC,
                inizioAssicurazione, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti
            ControlsNSettimaneAllRetributivo(datiPensione, settimaneC2, fineAssicurazione, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto,
                maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            //controlli relativi al numero di settimane limite per: RetributivoMonti
            ControlsNSettimaneRetributivoMonti(datiPensione, settimaneD, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto,
                maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            if (!ControlsCalcoloContributivoL214(datiAggPeco, datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
            {
                IsCalcoloValid = false;
                return false;
            }

            return true;
        }

        public static bool ControlsCalcoloRetributivoMontiFondoVLForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, GestioneCalcolo.DatiCalcoloContributivo datiContributivi,
            Entity.DatiBenefici datiBenefici, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = datiBenefici.TipoSettimaneBeneficio;

            if (datiRetributivi != null)
            {
                //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti - Misto - MistoMonti
                ControlsNSettimaneAllRetributivoAllMisto(datiPensione, codiceSpecificoTraduzioneSuGP,
                    datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                                                         datiRetributivi.NSettimaneQuotaA2.HasValue ? datiRetributivi.NSettimaneQuotaA2.Value : 0,
                                                         datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                                                         datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                                                         datiPensione.InizioAssicurazione, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti
                ControlsNSettimaneAllRetributivo(datiPensione, datiRetributivi.NSettimaneQuotaC2.HasValue ? datiRetributivi.NSettimaneQuotaC2.Value : 0,
                                                         datiPensione.FineAssicurazione, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                //controlli relativi al numero di settimane limite per: RetributivoMonti
                ControlsNSettimaneRetributivoMonti(datiPensione, datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                if (!ControlsCalcoloContributivoL214(datiContributivi, datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74,
                    out messaggioVideo))
                {
                    IsCalcoloValid = false;
                    return false;
                }
            }

            return true;
        }

        #endregion Fondo VL

        #region Fondo ET

        public static bool ControlsCalcoloRetributivoMontiFondoET(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile, char? codiceSpecifico, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;
            bool isDatiCalcoloRetrib = listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0 && listaDatiServizioUtile.Exists(x => x.Quota == "A" || x.Quota == "B");

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (datiAggPeco.Retribuzione != null)
            {
                if ((datiMaggiorazioniBenefici != null && (!String.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio) &&
                    (datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("01") || datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("02") || datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("03")))) ||
                    (codiceSpecifico == 'Q'))
                {
                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(datiAggPeco.Retribuzione.SettimaneA, datiAggPeco.Retribuzione.SettimaneB, datiAggPeco.Retribuzione.SettimaneC,
                                                                          datiAggPeco.Retribuzione.SettimaneD, out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }
                else
                {
                    GestioneControlli.ControlsNumeroSingoleSettimaneRetrFondoET(datiPensione, datiDanteCausa, listaDatiServizioUtile, datiAggPeco.Retribuzione.SettimaneB,
                        datiAggPeco.DatiParziali.InizioAssicurazione, datiAggPeco.DatiParziali.FineAssicurazione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74,
                        tipoFondo, out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;

                    GestioneControlli.ControlsNumeroSingoleSettimaneRetrMontiFondoET(datiPensione, datiAggPeco.Retribuzione.SettimaneB, listaDatiServizioUtile, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto,
                        maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }

                //aggiunto controllo su RetribuzionePonderataAnnua
                if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC) &&
                    isDatiCalcoloRetrib && !ControlsRetrPonderataAnnuaObbligatoria(datiAggPeco.Retribuzione.RetribuzionePonderataAnnua, datiPensione, out messaggioVideo))
                    return false;
            }
            if (!ControlsCalcoloContributivoL214(datiAggPeco, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
            {
                IsCalcoloValid = false;
                return false;
            }
            return true;
        }

        public static bool ControlsCalcoloRetributivoMontiFondoETForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            GestioneCalcolo.DatiCalcoloContributivo datiContributivi, Entity.DatiBenefici datiBenefici,
            GestionePensione.DatiPensione datiPensione,
            List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile, char? codiceSpecifico, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = string.Empty;
            if (datiBenefici != null)
                tipoSettimaneBeneficio = datiBenefici.TipoSettimaneBeneficio;

            if (datiRetributivi != null)
            {
                if (datiBenefici != null && (!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                    (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03"))))
                {

                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                                                                          datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                                                                          datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                                                                          datiRetributivi.NSettimaneQuotaD.HasValue ? datiRetributivi.NSettimaneQuotaD.Value : 0,
                                                                          out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }
                else
                {
                    GestioneControlli.ControlsNumeroSingoleSettimaneRetrMontiFondoET(datiPensione, datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                        listaServizioUtile, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }
            }

            if (!ControlsCalcoloContributivoL214(datiContributivi, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
            {
                IsCalcoloValid = false;
                return false;
            }

            return true;
        }

        #endregion Fondo ET

        #region Fondo GAS
        public static bool ControlsCalcoloRetributivoMontiFondoGAS(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!ControlsCalcoloRetributivoFondoGAS(datiAggPeco, out messaggioVideo))
                return false;

            if (!ControlsCalcoloContributivoL214FondoGAS(datiAggPeco, out messaggioVideo))
                return false;

            return true;
        }
        #endregion Fondo GAS

        #region Fondo DZ
        public static bool ControlsCalcoloRetributivoMontiFondoDZ(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!ControlsCalcoloRetributivoFondoDZ(datiAggPeco, out messaggioVideo))
                return false;

            if (!ControlsCalcoloContributivoL214FondoDZ(datiAggPeco, out messaggioVideo))
                return false;

            return true;
        }
        #endregion Fondo DZ

        #region Fondo FS/PT

        public static bool ControlsCalcoloRetributivoMontiFondoFSPT(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            if (!ControlsCalcoloContributivoL214(datiAggPeco, datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            return true;
        }

        #endregion Fondo FS/PT

        #endregion calcolo RetributivoMonti

        #region calcolo Misto

        #region Fondo EL - TT

        public static bool ControlsCalcoloMisto(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestionePensione.DatiPensione datiPensione, char? codiceSpecifico, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            int? settimaneA = null;
            int? settimaneB = null;
            int? settimaneC = null;
            int? settimaneD = null;
            decimal rmsQuotaD = 0;
            decimal retribuzionePonderataAnnua = 0;
            DateTime? inizioAssicurazione = null;
            DateTime? fineAssicurazione = null;
            if (datiAggPeco != null)
            {
                if (datiAggPeco.Retribuzione != null)
                {
                    settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                    settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                    settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                    settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                    rmsQuotaD = datiAggPeco.Retribuzione.RmsQuotaD;
                    retribuzionePonderataAnnua = datiAggPeco.Retribuzione.RetribuzionePonderataAnnua;
                }
                if (datiAggPeco.DatiParziali != null)
                {
                    inizioAssicurazione = datiAggPeco.DatiParziali.InizioAssicurazione;
                    fineAssicurazione = datiAggPeco.DatiParziali.FineAssicurazione;
                }
            }

            if ((!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03"))) ||
                (codiceSpecifico == 'Q'))
            {
                bool IsCalcoloValid = true;
                GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(settimaneA, settimaneB, settimaneC, settimaneD, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;
            }
            else
            {
                if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                {
                    //Quota B: il numero massimo di settimane ammesso è pari a 104
                    //Quota C: il numero massimo di settimane ammesso è pari a 52;
                    //La quota D non è prevista;            
                    if (settimaneB.GetValueOrDefault() > 104)
                    {
                        messaggioVideo = "Settimane quota B superiori a 104.";
                        return false;
                    }
                    if (settimaneC.GetValueOrDefault() > 52)
                    {
                        messaggioVideo = "Settimane quota C superiori a 52. ";
                        return false;
                    }
                    if (settimaneD.GetValueOrDefault() != 0)
                    {
                        messaggioVideo = "Settimane quota D non pari a 0.";
                        return false;
                    }
                    if (rmsQuotaD != 0)
                    {
                        messaggioVideo = "RMS quota D non pari a 0.";
                        return false;
                    }

                    //Quota A: il numero massimo di settimane ammesso è pari alla differenza dal 31/12/92 al primo versamento;
                    DateTime dataLimite = new DateTime(1992, 12, 31);
                    //aggiunta settimana di tolleranza come indicato da mail del 07-09-12
                    //int settimaneAmmesse = (int)Math.Ceiling(((dataLimite.Year - datiAggPeco.DatiParziali.InizioAssicurazione.Value.Year) * 12 +
                    //        (dataLimite.Month - datiAggPeco.DatiParziali.InizioAssicurazione.Value.Month) + 1) * 4.33333) + 1;
                    int settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite, inizioAssicurazione.Value) + 1;

                    //if (settimaneAmmesse < 0)
                    //{
                    //    messaggioVideo = "La data 'Primo Versamento' inserita nella tab 'Dati Assicurativi' del menu 'Liquidazione Pensione' non è compatibile con i 'Dati Calcolo' inseriti";
                    //    return false;
                    //}

                    if (settimaneAmmesse > 0 && settimaneA.GetValueOrDefault() > settimaneAmmesse && settimaneA.GetValueOrDefault() != 0)
                    {
                        messaggioVideo = "Settimane quota A superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                        return false;
                    }
                }
            }

            //aggiunto controllo su RetribuzionePonderataAnnua
            if (!ControlsRetrPonderataAnnuaObbligatoria(retribuzionePonderataAnnua, datiPensione, out messaggioVideo))
                return false;

            if (!ControlsCalcoloContributivo(datiAggPeco, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsCalcoloMistoForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, GestioneCalcolo.DatiCalcoloContributivo datiContributivi,
            Entity.DatiBenefici datiBenefici, GestionePensione.DatiPensione datiPensione, char? codiceSpecifico, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Benefici_FS.DOPPIO_BENEFICIO_CON_QUOTA100) ||
                ((datiBenefici != null && (!String.IsNullOrEmpty(datiBenefici.TipoSettimaneBeneficio) &&
                (datiBenefici.TipoSettimaneBeneficio.Equals("01") || datiBenefici.TipoSettimaneBeneficio.Equals("02") || datiBenefici.TipoSettimaneBeneficio.Equals("03"))))))
            {
                if (datiRetributivi != null)
                {
                    bool IsCalcoloValid = true;
                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib((datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0),
                        (datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0),
                        (datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0), 0, out messaggioVideo, out IsCalcoloValid);

                    if (!IsCalcoloValid)
                        return false;
                }
            }
            else
            {
                if (datiRetributivi != null)
                {
                    //Quota B: il numero massimo di settimane ammesso è pari a 104
                    //Quota C: il numero massimo di settimane ammesso è pari a 52;
                    //La quota D non è prevista;
                    if (datiRetributivi.NSettimaneQuotaB.HasValue && datiRetributivi.NSettimaneQuotaB.Value > 104)
                    {
                        messaggioVideo = "Settimane quota B superiori a 104.";
                        return false;
                    }
                    if (datiRetributivi.NSettimaneQuotaC.HasValue && datiRetributivi.NSettimaneQuotaC.Value > 52)
                    {
                        messaggioVideo = "Settimane quota C superiori a 52. ";
                        return false;
                    }
                    if (datiRetributivi.NSettimaneQuotaD.HasValue && datiRetributivi.NSettimaneQuotaD.Value != 0)
                    {
                        messaggioVideo = "Settimane quota D non pari a 0.";
                        return false;
                    }
                    if (datiRetributivi.RMSQuotaD.HasValue && datiRetributivi.RMSQuotaD.Value != 0)
                    {
                        messaggioVideo = "RMS quota D non pari a 0.";
                        return false;
                    }
                    if (datiPensione.InizioAssicurazione.HasValue && datiPensione.FineAssicurazione.HasValue)
                    {
                        //Quota A: il numero massimo di settimane ammesso è pari alla differenza dal 31/12/92 al primo versamento;
                        DateTime dataLimite = new DateTime(1992, 12, 31);
                        //aggiunta settimana di tolleranza come indicato da mail del 07-09-12
                        //int settimaneAmmesse = (int)Math.Ceiling(((dataLimite.Year - datiPensione.InizioAssicurazione.Value.Year) * 12 +
                        //        (dataLimite.Month - datiPensione.InizioAssicurazione.Value.Month) + 1) * 4.33333) + 1;
                        int settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite, datiPensione.InizioAssicurazione.Value) + 1;

                        if ((datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0) > settimaneAmmesse &&
                            (datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0) != 0)
                        {
                            messaggioVideo = "Settimane quota A superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                            return false;
                        }


                    }
                }
            }
            if (datiContributivi != null)
            {
                if (!ControlsCalcoloContributivo(datiContributivi, datiPensione, codiceSpecifico, datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                    return false;
            }

            return true;
        }

        #endregion Fondo EL - TT

        #region Fondo VL

        public static bool ControlsCalcoloMistoFondoVL(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            int? settimaneA = null;
            int settimaneA2 = 0;
            int? settimaneB = null;
            int? settimaneC = null;
            int? settimaneD = null;
            DateTime? inizioAssicurazione = null;
            if (datiAggPeco != null)
            {
                if (datiAggPeco.Retribuzione != null)
                {
                    settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                    settimaneA2 = datiAggPeco.Retribuzione.SettimaneA2;
                    settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                    settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                    settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                }
                if (datiAggPeco.DatiParziali != null)
                {
                    inizioAssicurazione = datiAggPeco.DatiParziali.InizioAssicurazione;
                }
            }

            //obbligatorietà per: Retributivo - RetributivoMonti - Misto - MistoMonti
            if (!ControlsCalcoloAllRetributivoMonti_MistoMontiFondoVL(datiPensione, datiAggPeco, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti - Misto - MistoMonti
            ControlsNSettimaneAllRetributivoAllMisto(datiPensione, codiceSpecificoTraduzioneSuGP, settimaneA, settimaneA2, settimaneB, settimaneC,
                inizioAssicurazione, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            if (!ControlsCalcoloContributivoFondoVL(datiAggPeco, datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsCalcoloMistoFondoVLForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, GestioneCalcolo.DatiCalcoloContributivo datiContributivi,
            char? codiceSpecificoTraduzioneSuGP, Entity.DatiBenefici datiBenefici, GestionePensione.DatiPensione datiPensione, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool IsCalcoloValid = true;

            string tipoSettimaneBeneficio = datiBenefici.TipoSettimaneBeneficio;

            //controlli relativi al numero di settimane limite per: Retributivo - RetributivoMonti - Misto - MistoMonti
            ControlsNSettimaneAllRetributivoAllMisto(datiPensione, codiceSpecificoTraduzioneSuGP,
                datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0,
                                                     datiRetributivi.NSettimaneQuotaA2.HasValue ? datiRetributivi.NSettimaneQuotaA2.Value : 0,
                                                     datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0,
                                                     datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0,
                                                     datiPensione.InizioAssicurazione, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
            if (!IsCalcoloValid)
                return false;

            if (!ControlsCalcoloContributivoFondoVLForMaggiorazioneBenefici(datiContributivi, datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            return true;
        }

        #endregion Fondo VL

        #region Fondo ET

        public static bool ControlsCalcoloMistoFondoET(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile, char? codiceSpecifico, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool isDatiCalcoloRetrib = listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0 && listaDatiServizioUtile.Exists(x => x.Quota == "A" || x.Quota == "B");

            string tipoSettimaneBeneficio = string.Empty;
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                tipoSettimaneBeneficio = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            int? settimaneA = null;
            int? settimaneB = null;
            int? settimaneC = null;
            int? settimaneD = null;
            decimal retribuzionePonderataAnnua = 0;
            DateTime? inizioAssicurazione = null;
            DateTime? fineAssicurazione = null;
            if (datiAggPeco != null)
            {
                if (datiAggPeco.Retribuzione != null)
                {
                    settimaneA = datiAggPeco.Retribuzione.SettimaneA;
                    settimaneB = datiAggPeco.Retribuzione.SettimaneB;
                    settimaneC = datiAggPeco.Retribuzione.SettimaneC;
                    settimaneD = datiAggPeco.Retribuzione.SettimaneD;
                    retribuzionePonderataAnnua = datiAggPeco.Retribuzione.RetribuzionePonderataAnnua;
                }
                if (datiAggPeco.DatiParziali != null)
                {
                    inizioAssicurazione = datiAggPeco.DatiParziali.InizioAssicurazione;
                    fineAssicurazione = datiAggPeco.DatiParziali.FineAssicurazione;
                }
            }
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if ((!String.IsNullOrEmpty(tipoSettimaneBeneficio) &&
                (tipoSettimaneBeneficio.Equals("01") || tipoSettimaneBeneficio.Equals("02") || tipoSettimaneBeneficio.Equals("03"))) ||
                (codiceSpecifico == 'Q'))
            {
                bool IsCalcoloValid = true;
                GestioneControlli.ControlsTotaleNumeroSettimaneRetrib(settimaneA, settimaneB, settimaneC, settimaneD, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;
            }
            else
            {
                bool IsCalcoloValid = true;
                GestioneControlli.ControlsNumeroSingoleSettimaneRetrFondoET(datiPensione, datiDanteCausa, listaDatiServizioUtile, settimaneB, inizioAssicurazione,
                        fineAssicurazione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, tipoFondo, out messaggioVideo, out IsCalcoloValid);
                if (!IsCalcoloValid)
                    return false;

                if (datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01)))
                {
                    GestioneControlli.ControlsNumeroSingoleSettimaneRetrMontiFondoET(datiPensione, settimaneB, listaDatiServizioUtile, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto,
                        maggiorazioneInv74, out messaggioVideo, out IsCalcoloValid);
                    if (!IsCalcoloValid)
                        return false;
                }

                //int settimaneAmmesse = 0;
                //DateTime dataLimite = new DateTime(1996, 01, 01);
                //DateTime dataLimiteLeggeMonti = new DateTime(2012, 1, 1);
                //if (!Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, dataLimiteLeggeMonti))   //misto puro
                //{
                //    settimaneAmmesse = Utility.NSettimaneBetweenDate(datiPensione.FineAssicurazione.Value, dataLimite);
                //}
                //else //misto monti
                //{
                //    settimaneAmmesse = GetSettimaneMontiETAmmesse(datiPensione, listaDatiServizioUtile, codiceSpecifico);
                //}

                //if (settimaneAmmesse > 0 && datiAggPeco.Retribuzione.SettimaneB > settimaneAmmesse)
                //{
                //    messaggioVideo = "Settimane quota B superiori a " + settimaneAmmesse + ".";
                //    return false;
                //}
            }

            //aggiunto controllo su RetribuzionePonderataAnnua
            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC) &&
                isDatiCalcoloRetrib && !ControlsRetrPonderataAnnuaObbligatoria(retribuzionePonderataAnnua, datiPensione, out messaggioVideo))
                return false;

            if (!ControlsCalcoloContributivo(datiAggPeco, datiPensione, codiceSpecifico, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsCalcoloMistoFondoETForMaggiorazioneBenefici(GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            GestioneCalcolo.DatiCalcoloContributivo datiContributivi, Entity.DatiBenefici datiBenefici,
            GestionePensione.DatiPensione datiPensione,
            List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile, char? codiceSpecifico, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((datiBenefici != null && (!String.IsNullOrEmpty(datiBenefici.TipoSettimaneBeneficio) &&
                (datiBenefici.TipoSettimaneBeneficio.Equals("01") || datiBenefici.TipoSettimaneBeneficio.Equals("02") || datiBenefici.TipoSettimaneBeneficio.Equals("03")))) ||
                (codiceSpecifico.GetValueOrDefault() == 'Q'))
            {
                if (datiRetributivi != null)
                {
                    bool IsCalcoloValid = true;
                    GestioneControlli.ControlsTotaleNumeroSettimaneRetrib((datiRetributivi.NSettimaneQuotaA.HasValue ? datiRetributivi.NSettimaneQuotaA.Value : 0),
                        (datiRetributivi.NSettimaneQuotaB.HasValue ? datiRetributivi.NSettimaneQuotaB.Value : 0),
                        (datiRetributivi.NSettimaneQuotaC.HasValue ? datiRetributivi.NSettimaneQuotaC.Value : 0), 0, out messaggioVideo, out IsCalcoloValid);

                    if (!IsCalcoloValid)
                        return false;
                }
            }
            else
            {
                if (datiRetributivi != null)
                {
                    int settimaneAmmesse = 0;
                    DateTime dataLimite = new DateTime(1996, 01, 01);
                    DateTime dataLimiteLeggeMonti = new DateTime(2012, 1, 1);
                    if (!Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, dataLimiteLeggeMonti))   //misto puro
                    {

                        settimaneAmmesse = Utility.NSettimaneBetweenDate(datiPensione.FineAssicurazione.Value, dataLimite);
                    }
                    else //misto monti
                    {
                        settimaneAmmesse = GetSettimaneMontiETAmmesse(datiPensione, listaServizioUtile, codiceSpecifico);
                    }

                    if (datiRetributivi.NSettimaneQuotaB.HasValue && datiRetributivi.NSettimaneQuotaB.Value > settimaneAmmesse)
                    {
                        messaggioVideo = "Settimane quota B superiori a " + settimaneAmmesse + ".";
                        return false;
                    }
                }
            }
            if (datiContributivi != null)
            {
                if (!ControlsCalcoloContributivo(datiContributivi, datiPensione, codiceSpecifico, datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                    return false;
            }

            return true;
        }

        public static bool ControlsServizioUtileForMaggiorazioneBenefici(GestionePensione.DatiPensione datiPensione, Entity.DatiBenefici datiBenefici,
            List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtile, char? codiceSpecificoTraduzioneSuGP, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            foreach (GestioneDatiServizioUtile.ServizioUtile datiServizioUtile in lDatiServizioUtile)
            {
                string periodo = string.Empty;
                int maxRangeAA = 0;
                int maxRangeMM = 0;
                int maxRangeGG = 0;
                DateTime dataLimite = new DateTime(1993, 1, 1);
                switch (datiServizioUtile.Quota)
                {
                    case "A":
                        CalcolaMaxRangeAAMMGG(datiPensione.InizioAssicurazione.Value, dataLimite, out maxRangeAA, out maxRangeMM, out maxRangeGG);
                        periodo = "al 01/01/93 (Quota A)";
                        break;
                    case "B":
                        maxRangeAA = 2;
                        maxRangeMM = 0;
                        maxRangeGG = 0;
                        periodo = "al 31/12/92 (Quota B)";
                        break;
                    case "C":
                        maxRangeAA = 1;
                        maxRangeMM = 0;
                        maxRangeGG = 0;
                        periodo = "al 31/12/94 (Quota C)";
                        break;
                }
                if (datiBenefici != null && !string.IsNullOrEmpty(datiBenefici.TipoSettimaneBeneficio) && (datiBenefici.TipoSettimaneBeneficio.Equals("01") ||
                    datiBenefici.TipoSettimaneBeneficio.Equals("02") || datiBenefici.TipoSettimaneBeneficio.Equals("03")))
                {
                    if (!VerificaDatiServizioUtileObbligatori(datiPensione, datiServizioUtile.ServizioUtileAA, datiServizioUtile.ServizioUtileMM, periodo, string.Empty, out messaggioVideo))
                        return false;
                }
                else
                {
                    if (!VerificaPeriodoMaxConsentito(datiPensione, codiceSpecificoTraduzioneSuGP, datiBenefici != null ? datiBenefici.TipoSettimaneBeneficio : null, maggiorazioneAmianto, maggiorazioneInv74,
                        datiServizioUtile.ServizioUtileAA, datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, periodo, maxRangeAA, maxRangeMM, maxRangeGG, string.Empty, out messaggioVideo))
                        return false;
                }
            }
            return true;
        }
        #endregion Fondo ET

        #region Fondo GAS
        public static bool ControlsCalcoloMistoFondoGAS(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!ControlsCalcoloRetributivoFondoGAS(datiAggPeco, out messaggioVideo))
                return false;

            if (!ControlsCalcoloContributivoFondoGAS(datiAggPeco, datiPensione, out messaggioVideo))
                return false;

            return true;
        }
        #endregion Fondo GAS

        #endregion calcolo Misto

        public static bool ControlsRiduzioneRetributiva(Utility.TipoCalcolo tipoCalcolo, Liquidazione.BLCommon.GestioneFondo.DatiFondo datiFondo,
            Liquidazione.BLCommon.GestionePensione.DatiPensione datiPensione, object datiFondoXX, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaReversibilita(datiPensione))
                return true;

            if (GestioneContrib.GestioneRiduzioneRetributiva(datiPensione, datiFondo, datiFondoXX).GetValueOrDefault())
            {
                if (datiFondo.RiduzioneRetributiva && !datiFondo.RiduzioneRetributivaPercentuale.HasValue)
                {
                    messaggioVideo = "Il campo Riduzione Retributiva Percentuale è obbligatorio con Riduzione Retributiva 'SI'";
                    return false;
                }
                if (!datiFondo.RiduzioneRetributiva && datiFondo.RiduzioneRetributivaPercentuale.HasValue)
                {
                    messaggioVideo = "Il campo Riduzione Retributiva Percentuale non deve essere valorizzato con Riduzione Retributiva 'NO'";
                    return false;
                }
            }
            else if (datiFondo.RiduzioneRetributiva || datiFondo.RiduzioneRetributivaPercentuale.HasValue)
            {
                messaggioVideo = "I campi Riduzione Retributiva e Riduzione Retributiva Percentuale non deve essere valorizzati";
                return false;
            }
            return true;
        }

        /// <summary>
        /// le specifiche di questa logica sono espresse negli attach della mail: "FW: Controlli Dati Calcolo - punti aperti" di venerdì 14/12/2012 16:35
        /// </summary>
        /// <param name="datiCalcolo"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="datiPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDatiCalcoloFS_PT(INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiCalcolo datiCalcolo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, Utility.TipoCalcolo tipocalcolo,
            char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            decimal? pensioneAnnuaLorda = null;
            short? anniServizioUtiliDiritto = null;
            short? mesiServizioUtiliDiritto = null;
            short? giorniServizioUtiliDiritto = null;
            List<INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiServizioUtile> lDatiServizioUtile = null;

            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.FS:
                    pensioneAnnuaLorda = datiCalcolo.fondoFST.PensioneAnnuaLorda;
                    anniServizioUtiliDiritto = datiCalcolo.fondoFST.ServizioUtileDirittoAA;
                    mesiServizioUtiliDiritto = datiCalcolo.fondoFST.ServizioUtileDirittoMM;
                    giorniServizioUtiliDiritto = datiCalcolo.fondoFST.ServizioUtileDirittoGG;
                    lDatiServizioUtile = datiCalcolo.fondoFST.lDatiServizioUtile;
                    break;
                case Utility.TipoFondo.PT:
                    pensioneAnnuaLorda = datiCalcolo.fondoPT.PensioneAnnuaLorda;
                    anniServizioUtiliDiritto = datiCalcolo.fondoPT.ServizioUtileDirittoAA;
                    mesiServizioUtiliDiritto = datiCalcolo.fondoPT.ServizioUtileDirittoMM;
                    giorniServizioUtiliDiritto = datiCalcolo.fondoPT.ServizioUtileDirittoGG;
                    lDatiServizioUtile = datiCalcolo.fondoPT.lDatiServizioUtile;
                    break;
            }

            if (!pensioneAnnuaLorda.HasValue)
            {
                messaggioVideo = "Campo 'Pensione Annua Lorda' obbligatorio.";
                return false;
            }
            if (!anniServizioUtiliDiritto.HasValue && !mesiServizioUtiliDiritto.HasValue && !giorniServizioUtiliDiritto.HasValue)
            {
                messaggioVideo = "Campo 'Anni Servizio Utile Diritto' obbligatorio.";
                return false;
            }

            switch (tipocalcolo)
            {
                case Utility.TipoCalcolo.Retributivo:
                case Utility.TipoCalcolo.Misto:

                    if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
                    {
                        List<KeyValuePair<string, string>> lQuotaUltimoVers = null;
                        List<KeyValuePair<string, string>> lQuotaUltimoVersOverFineAss = null;

                        List<KeyValuePair<string, string>> lQuota = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("B3", "1997"),
                                                                                                 new KeyValuePair<string, string>("B2", "1995"),
                                                                                                 new KeyValuePair<string, string>("B1", "1994"),
                                                                                                 new KeyValuePair<string, string>("A",  "1992")};

                        lQuotaUltimoVers = lQuota.FindAll((x => (Convert.ToInt32(x.Value) <= datiPensione.FineAssicurazione.Value.Year)));

                        lQuotaUltimoVersOverFineAss = lQuota.FindAll((x => (Convert.ToInt32(x.Value) > datiPensione.FineAssicurazione.Value.Year)));
                        if (lQuotaUltimoVersOverFineAss != null && lQuotaUltimoVersOverFineAss.Count > 0)
                        {
                            if (lQuotaUltimoVers.Count == 0)
                                lQuotaUltimoVers.Add(lQuotaUltimoVersOverFineAss.Last());
                            else
                            {
                                if (datiPensione.FineAssicurazione.Value.Year > Convert.ToInt32(lQuotaUltimoVers.First().Value))
                                    lQuotaUltimoVers.Add(lQuotaUltimoVersOverFineAss.Last());
                            }

                            lQuotaUltimoVers = lQuotaUltimoVers.OrderBy(x => x.Key).ToList();
                        }

                        GestioneContrib.DatiServizioUtile ServizioUtileApp = null;
                        foreach (KeyValuePair<string, string> quota in lQuotaUltimoVers)
                        {
                            if (lDatiServizioUtile.Find(x => x.Quota == quota.Key) != null)
                                ServizioUtileApp = lDatiServizioUtile.Find(x => x.Quota == quota.Key);
                        }

                        GestioneContrib.DatiServizioUtile ServizioUtileCessazione = lDatiServizioUtile.Find(x => x.Quota == "B4");

                        if (ServizioUtileApp == null)
                        {
                            if (datiPensione.FineAssicurazione.Value.Year > Convert.ToInt32(lQuotaUltimoVers.First().Value))
                            {
                                if (ServizioUtileCessazione == null)
                                {
                                    messaggioVideo = "Dati Servizio Utile tra il 31/12/" + lQuotaUltimoVers.Last().Value + " e Servizio Utile Cessazione mancanti.";
                                    return false;
                                }
                            }
                            else
                            {
                                if (lQuotaUltimoVers.Count == 1)
                                    messaggioVideo = "Dati Servizio Utile al 31/12/" + lQuotaUltimoVers.Last().Value + " mancanti.";
                                else
                                    messaggioVideo = "Dati Servizio Utile tra il 31/12/" + lQuotaUltimoVers.Last().Value + " e il 31/12/" + lQuotaUltimoVers.First().Value + " mancanti.";
                                return false;
                            }
                        }

                        List<KeyValuePair<string, string>> lQuoteInserite = new List<KeyValuePair<string, string>>();
                        foreach (GestioneContrib.DatiServizioUtile servUtile in lDatiServizioUtile)
                        {
                            if (servUtile.Quota != "B4")
                            {
                                KeyValuePair<string, string> q = lQuota.Find((x => (x.Key == servUtile.Quota)));
                                lQuoteInserite.Add(q);
                            }
                        }

                        short maxRangeAA = 0; short maxRangeMM = 0; short maxRangeGG = 0;

                        if (!VerificaPeriodoMaxConsentitoAll(datiPensione, lDatiServizioUtile, lQuoteInserite, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74,
                            out maxRangeAA, out maxRangeMM, out maxRangeGG, tipoFondo, danteCausa, datiLavorazione, out messaggioVideo))
                            return false;

                        if (ServizioUtileCessazione != null)
                        {
                            if (lQuotaUltimoVers.Count > 0)  // se data ultimo versamento < 1992 la lista sarà vuota --> bypass controllo (vale solo AA < 99, MM<11, GG<30)
                            {
                                if (!VerificaPeriodoMaxConsentitoCessazione(datiPensione, ServizioUtileCessazione, datiPensione.FineAssicurazione, codiceSpecificoTraduzioneSuGP,
                                    tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                                    return false;
                            }

                            maxRangeAA += ServizioUtileCessazione.ServizioUtileCessazioneAA.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneAA.Value : (short)0;
                            maxRangeMM += ServizioUtileCessazione.ServizioUtileCessazioneMM.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneMM.Value : (short)0;
                            maxRangeGG += ServizioUtileCessazione.ServizioUtileCessazioneGG.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneGG.Value : (short)0;
                        }

                        if (!VerificaNumeroAnniServizioUtileDiritto(maxRangeAA, maxRangeMM, maxRangeGG, anniServizioUtiliDiritto, mesiServizioUtiliDiritto, giorniServizioUtiliDiritto,
                            datiPensione.DecorrenzaOriginaria, datiPensione, out messaggioVideo))
                            return false;
                    }
                    else
                    {
                        if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && datiPensione.FineAssicurazione.HasValue &&
                            !Utility.DataStrettamenteSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(1992, 12, 31)))
                            return true;

                        messaggioVideo = "Dati Servizio Utile mancanti";
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// le specifiche di questa logica sono espresse negli attach della mail: "FW: Controlli Dati Calcolo - punti aperti" di venerdì 14/12/2012 16:35
        /// </summary>
        /// <param name="datiCalcolo"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="datiPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDatiCalcoloFS_PTRecordFondo(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, decimal? pensioneAnnuaLorda, short? anniServizioUtileDiritto, short? mesiServizioUtileDiritto,
            short? giorniServizioUtileDiritto, short? anniServizioUtileDirittoOI, short? mesiServizioUtileDirittoOI, short? giorniServizioUtileDirittoOI, List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtile, Utility.TipoFondo? tipoFondo, Utility.TipoCalcolo tipocalcolo, char? codiceSpecificoTraduzioneSuGP,
            string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, decimal? ImportoContributivoTotale, decimal? Montante, decimal? MontanteContributivo, int? NSettimane, decimal? MontanteQuotaDL214, decimal? ImportoContribTotaleQuotaDL214, int? NSettimaneQuotaDL214, decimal? QuotaContributivaAnnua, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<GestioneContrib.DatiServizioUtile> lDatiServizioUtile = null;

            if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione)))
                return true;

            if (lServizioUtile != null && lServizioUtile.Count > 0)
            {
                lDatiServizioUtile = new List<GestioneContrib.DatiServizioUtile>();
                foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtile in lServizioUtile)
                {
                    GestioneContrib.DatiServizioUtile datiServizioUtile = new GestioneContrib.DatiServizioUtile();
                    Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                    lDatiServizioUtile.Add(datiServizioUtile);
                }
            }

            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            if (!pensioneAnnuaLorda.HasValue)
            {
                messaggioVideo = "Campo 'Pensione Annua Lorda' obbligatorio.";
                return false;
            }
            if (!anniServizioUtileDiritto.HasValue && !mesiServizioUtileDiritto.HasValue && !giorniServizioUtileDiritto.HasValue)
            {
                messaggioVideo = "Campo 'Anni Servizio Utile Diritto' obbligatorio.";
                return false;
            }

            if (!IsValoreAAMMGGValido(anniServizioUtileDiritto, null, null))
            {
                messaggioVideo = "Anni Servizio Utili Diritto AA deve essere compreso tra 0 e 99";
                return false;
            }

            if (!IsValoreAAMMGGValido(null, mesiServizioUtileDiritto, null))
            {
                messaggioVideo = "Anni Servizio Utili Diritto MM deve essere compreso tra 0 e 11";
                return false;
            }

            if (!IsValoreAAMMGGValido(null, null, giorniServizioUtileDiritto))
            {
                messaggioVideo = "Anni Servizio Utili Diritto GG deve essere compreso tra 0 e 29";
                return false;
            }

            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {

                if (!IsValoreAAMMGGValido(anniServizioUtileDirittoOI, null, null))
                {
                    messaggioVideo = "Anni Servizio Utili Diritto OI AA deve essere compreso tra 0 e 99";
                    return false;
                }

                if (!IsValoreAAMMGGValido(null, mesiServizioUtileDirittoOI, null))
                {
                    messaggioVideo = "Anni Servizio Utili Diritto OI MM deve essere compreso tra 0 e 11";
                    return false;
                }

                if (!IsValoreAAMMGGValido(null, null, giorniServizioUtileDirittoOI))
                {
                    messaggioVideo = "Anni Servizio Utili Diritto OI GG deve essere compreso tra 0 e 29";
                    return false;
                }

                double nSettimaneIT = Utility.CalcolaSettimane(anniServizioUtileDiritto.Value, mesiServizioUtileDiritto.Value, giorniServizioUtileDiritto.Value);
                //double nSettimaneOI = Utility.CalcolaSettimane(anniServizioUtileDirittoOI.Value, mesiServizioUtileDirittoOI.Value, giorniServizioUtileDirittoOI.Value);
                if (Math.Abs(nSettimaneIT) < 52)
                {
                    messaggioVideo = "La differenza tra il numero di settimane totali utile al diritto e il numero settimane OI deve essere maggiore o ugule a 52.";
                    return false;
                }
            }


            if (tipocalcolo == Utility.TipoCalcolo.Misto && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
            {
                if (pensioneAnnuaLorda.HasValue && pensioneAnnuaLorda.Value == 0)
                {
                    messaggioVideo = "Pensione annua lorda:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (giorniServizioUtileDiritto.Value == 0 && mesiServizioUtileDiritto.Value == 0 && anniServizioUtileDiritto.Value == 0)
                {
                    messaggioVideo = "Servizio Utile Diritto:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
            }
            //ENG - GDP/024 - RIC CONCESSIONE ALTRA PENSIONE
            if (!datiPensione.FineAssicurazione.HasValue && !((Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) || (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && !isRiaperturaDomanda)) && (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS)))
            {
                messaggioVideo = "Fine assicurazione non valorizzata.";
                return false;
            }

            if (datiPensione.FineAssicurazione.HasValue)
            {
                switch (tipocalcolo)
                {
                    case Utility.TipoCalcolo.Retributivo:
                    case Utility.TipoCalcolo.Misto:

                        if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
                        {
                            List<KeyValuePair<string, string>> lQuotaUltimoVers = null;
                            List<KeyValuePair<string, string>> lQuotaUltimoVersOverFineAss = null;

                            List<KeyValuePair<string, string>> lQuota = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("B3", "1997"),
                                                                                                 new KeyValuePair<string, string>("B2", "1995"),
                                                                                                 new KeyValuePair<string, string>("B1", "1994"),
                                                                                                 new KeyValuePair<string, string>("A",  "1992")};

                            lQuotaUltimoVers = lQuota.FindAll((x => (Convert.ToInt32(x.Value) <= datiPensione.FineAssicurazione.Value.Year)));

                            lQuotaUltimoVersOverFineAss = lQuota.FindAll((x => (Convert.ToInt32(x.Value) > datiPensione.FineAssicurazione.Value.Year)));

                            if (lQuotaUltimoVersOverFineAss != null && lQuotaUltimoVersOverFineAss.Count > 0)
                            {
                                if (lQuotaUltimoVers.Count == 0)
                                    lQuotaUltimoVers.Add(lQuotaUltimoVersOverFineAss.Last());
                                else
                                {
                                    if (datiPensione.FineAssicurazione.Value.Year > Convert.ToInt32(lQuotaUltimoVers.First().Value))
                                        lQuotaUltimoVers.Add(lQuotaUltimoVersOverFineAss.Last());
                                }

                                lQuotaUltimoVers = lQuotaUltimoVers.OrderBy(x => x.Key).ToList();
                            }

                            GestioneContrib.DatiServizioUtile ServizioUtileApp = null;
                            foreach (KeyValuePair<string, string> quota in lQuotaUltimoVers)
                            {
                                if (lDatiServizioUtile.Find(x => x.Quota == quota.Key) != null)
                                    ServizioUtileApp = lDatiServizioUtile.Find(x => x.Quota == quota.Key);

                                if (tipocalcolo == Utility.TipoCalcolo.Misto && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                                   Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)
                                    && !Utility.IsDomandaReversibilita(datiPensione))
                                {
                                    if (ServizioUtileApp != null)
                                    {
                                        if (ServizioUtileApp.Quota == "A" && ServizioUtileApp.ServizioUtileAA == 0 && ServizioUtileApp.ServizioUtileMM == 0 && ServizioUtileApp.ServizioUtileGG == 0)
                                        {
                                            messaggioVideo = "Servizio Utile quota A dati al 31/12/92:è obbligatorio inserire un valore maggiore di 0";
                                            return false;
                                        }
                                        if (ServizioUtileApp.Quota == "B1" && ServizioUtileApp.ServizioUtileAA == 0 && ServizioUtileApp.ServizioUtileMM == 0 && ServizioUtileApp.ServizioUtileGG == 0)
                                        {
                                            messaggioVideo = "Servizio Utile quota B dati al 31/12/94:è obbligatorio inserire un valore maggiore di 0";
                                            return false;
                                        }
                                        if (ServizioUtileApp.Quota == "B2" && ServizioUtileApp.ServizioUtileAA == 0 && ServizioUtileApp.ServizioUtileMM == 0 && ServizioUtileApp.ServizioUtileGG == 0)
                                        {
                                            messaggioVideo = "Servizio Utile quota B dati al 31/12/95:è obbligatorio inserire un valore maggiore di 0";
                                            return false;
                                        }
                                        if (ServizioUtileApp.Quota == "B3" && ServizioUtileApp.ServizioUtileAA == 0 && ServizioUtileApp.ServizioUtileMM == 0 && ServizioUtileApp.ServizioUtileGG == 0)
                                        {
                                            messaggioVideo = "Servizio Utile quota B dati al 31/12/97:è obbligatorio inserire un valore maggiore di 0";
                                            return false;
                                        }
                                        if (ServizioUtileApp.Quota == "A" && ServizioUtileApp.Retribuzione == 0)
                                        {
                                            messaggioVideo = "Retribuzione ultimo mese:è obbligatorio inserire un valore maggiore di 0";
                                            return false;
                                        }
                                        if (ServizioUtileApp.Quota == "B1" && ServizioUtileApp.Retribuzione == 0)
                                        {
                                            messaggioVideo = "Retribuzione Media:è obbligatorio inserire un valore maggiore di 0";
                                            return false;
                                        }
                                    }
                                }
                            }

                            GestioneContrib.DatiServizioUtile ServizioUtileCessazione = lDatiServizioUtile.Find(x => x.Quota == "B4");

                            if (ServizioUtileApp == null)
                            {
                                if (!(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && !Utility.IsRiaperturaDomanda(datiPensione.Id)))
                                {
                                    if (datiPensione.FineAssicurazione.Value.Year > Convert.ToInt32(lQuotaUltimoVers.First().Value))
                                    {
                                        if (ServizioUtileCessazione == null)
                                        {
                                            messaggioVideo = "Dati Servizio Utile tra il 31/12/" + lQuotaUltimoVers.Last().Value + " e Servizio Utile Cessazione mancanti.";
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        if (lQuotaUltimoVers.Count == 1)
                                            messaggioVideo = "Dati Servizio Utile al 31/12/" + lQuotaUltimoVers.Last().Value + " mancanti.";
                                        else
                                            messaggioVideo = "Dati Servizio Utile tra il 31/12/" + lQuotaUltimoVers.Last().Value + " e il 31/12/" + lQuotaUltimoVers.First().Value + " mancanti.";
                                        return false;
                                    }
                                }
                            }

                            if (tipocalcolo == Utility.TipoCalcolo.Misto && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                                Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)
                                && !Utility.IsDomandaReversibilita(datiPensione))
                            {
                                if (ServizioUtileCessazione != null && ServizioUtileCessazione.ServizioUtileCessazioneAA == 0 && ServizioUtileCessazione.ServizioUtileCessazioneGG == 0 && ServizioUtileCessazione.ServizioUtileCessazioneMM == 0)
                                {
                                    messaggioVideo = "Servizio Utile Cessazione:è obbligatorio inserire un valore maggiore di 0";
                                    return false;
                                }
                            }

                            List<KeyValuePair<string, string>> lQuoteInserite = new List<KeyValuePair<string, string>>();
                            foreach (GestioneContrib.DatiServizioUtile servUtile in lDatiServizioUtile)
                            {
                                if (servUtile.Quota != "B4")
                                {
                                    KeyValuePair<string, string> q = lQuota.Find((x => (x.Key == servUtile.Quota)));
                                    lQuoteInserite.Add(q);
                                }
                            }

                            short maxRangeAA = 0; short maxRangeMM = 0; short maxRangeGG = 0;

                            if (!VerificaPeriodoMaxConsentitoAll(datiPensione, lDatiServizioUtile, lQuoteInserite, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74,
                                out maxRangeAA, out maxRangeMM, out maxRangeGG, tipoFondo, danteCausa, datiLavorazione, out messaggioVideo))
                                return false;

                            if (ServizioUtileCessazione != null)
                            {
                                if (lQuotaUltimoVers.Count > 0)  // se data ultimo versamento < 1992 la lista sarà vuota --> bypass controllo (vale solo AA < 99, MM<11, GG<30)
                                {
                                    if (!VerificaPeriodoMaxConsentitoCessazione(datiPensione, ServizioUtileCessazione, datiPensione.FineAssicurazione, codiceSpecificoTraduzioneSuGP,
                                        tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo))
                                        return false;
                                }

                                maxRangeAA += ServizioUtileCessazione.ServizioUtileCessazioneAA.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneAA.Value : (short)0;
                                maxRangeMM += ServizioUtileCessazione.ServizioUtileCessazioneMM.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneMM.Value : (short)0;
                                maxRangeGG += ServizioUtileCessazione.ServizioUtileCessazioneGG.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneGG.Value : (short)0;
                            }

                            if (!VerificaNumeroAnniServizioUtileDiritto(maxRangeAA, maxRangeMM, maxRangeGG, anniServizioUtileDiritto, mesiServizioUtileDiritto, giorniServizioUtileDiritto,
                                datiPensione.DecorrenzaOriginaria, datiPensione, out messaggioVideo))
                                return false;
                        }
                        else
                        {
                            if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && datiPensione.FineAssicurazione.HasValue &&
                                !Utility.DataStrettamenteSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(1992, 12, 31)))
                                return true;

                            //ENG - REVERSIBILITA 024: saltare il controllo
                            if (!(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && !Utility.IsRiaperturaDomanda(datiPensione.Id))
                                && !(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                            {
                                messaggioVideo = "Dati Servizio Utile mancanti";
                                return false;
                            }
                        }
                        break;
                }
            }
            if (tipocalcolo == Utility.TipoCalcolo.Misto && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)
                && !Utility.IsDomandaReversibilita(datiPensione))
            {
                if (ImportoContributivoTotale.HasValue && ImportoContributivoTotale.Value == 0)
                {
                    messaggioVideo = "Importo Contributivo Totale Legge 335:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (Montante.HasValue && Montante.Value == 0)
                {
                    messaggioVideo = "Montante Legge 335:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (MontanteContributivo.HasValue && MontanteContributivo.Value == 0)
                {
                    messaggioVideo = "Importo Quota C:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (NSettimane.HasValue && NSettimane.Value == 0)
                {
                    messaggioVideo = "Settimane Legge 335:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (MontanteQuotaDL214.HasValue && MontanteQuotaDL214.Value == 0)
                {
                    messaggioVideo = "Montante L. 214:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (ImportoContribTotaleQuotaDL214.HasValue && ImportoContribTotaleQuotaDL214.Value == 0)
                {
                    messaggioVideo = "Importo contributivo totale L. 214:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (NSettimaneQuotaDL214.HasValue && NSettimaneQuotaDL214.Value == 0)
                {
                    messaggioVideo = "Settimane L. 214:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
                if (QuotaContributivaAnnua.HasValue && QuotaContributivaAnnua.Value == 0)
                {
                    messaggioVideo = "Quota pensione contributiva annua:è obbligatorio inserire un valore maggiore di 0";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// le specifiche di questa logica sono espresse negli attach della mail: "FW: Controlli Dati Calcolo - punti aperti" di venerdì 14/12/2012 16:35
        /// </summary>
        /// <param name="datiCalcolo"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="datiPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDatiCalcoloRecordFondo(GestionePensione.DatiPensione datiPensione, decimal? pensioneAnnuaLorda, short? anniServizioUtiliDiritto, short? mesiServizioUtiliDiritto,
            short? giorniServizioUtiliDiritto, short? anniServizioUtileDirittoOI, short? mesiServizioUtileDirittoOI, short? giorniServizioUtileDirittoOI, List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lServizioUtile, GestioneContrib.TipoCalcolo tipocalcolo, byte? divisore, string capitolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!pensioneAnnuaLorda.HasValue)
            {
                messaggioVideo = "Campo 'Pensione Annua Lorda' obbligatorio.";
                return false;
            }
            if (!anniServizioUtiliDiritto.HasValue && !mesiServizioUtiliDiritto.HasValue && !giorniServizioUtiliDiritto.HasValue)
            {
                messaggioVideo = "Campo 'Anni Servizio Utile Diritto' obbligatorio.";
                return false;
            }


            if (pensioneAnnuaLorda.HasValue && pensioneAnnuaLorda.Value == 0)
            {
                messaggioVideo = "Pensione annua lorda:è obbligatorio inserire un valore maggiore di 0";
                return false;
            }
            if (giorniServizioUtiliDiritto.Value == 0 && mesiServizioUtiliDiritto.Value == 0 && anniServizioUtiliDiritto.Value == 0)
            {
                messaggioVideo = "Servizio Utile Diritto:è obbligatorio inserire un valore maggiore di 0";
                return false;
            }


            if (!IsValoreAAMMGGValido(anniServizioUtiliDiritto, null, null))
            {
                messaggioVideo = "Anni Servizio Utili Diritto AA deve essere compreso tra 0 e 99";
                return false;
            }

            if (!IsValoreAAMMGGValido(null, mesiServizioUtiliDiritto, null))
            {
                messaggioVideo = "Anni Servizio Utili Diritto MM deve essere compreso tra 0 e 11";
                return false;
            }

            if (!IsValoreAAMMGGValido(null, null, giorniServizioUtiliDiritto))
            {
                messaggioVideo = "Anni Servizio Utili Diritto GG deve essere compreso tra 0 e 29";
                return false;
            }

            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {

                if (!IsValoreAAMMGGValido(anniServizioUtileDirittoOI, null, null))
                {
                    messaggioVideo = "Anni Servizio Utili Diritto OI AA deve essere compreso tra 0 e 99";
                    return false;
                }

                if (!IsValoreAAMMGGValido(null, mesiServizioUtileDirittoOI, null))
                {
                    messaggioVideo = "Anni Servizio Utili Diritto OI MM deve essere compreso tra 0 e 11";
                    return false;
                }

                if (!IsValoreAAMMGGValido(null, null, giorniServizioUtileDirittoOI))
                {
                    messaggioVideo = "Anni Servizio Utili Diritto OI GG deve essere compreso tra 0 e 29";
                    return false;
                }

                double nSettimaneIT = Utility.CalcolaSettimane(anniServizioUtiliDiritto.Value, mesiServizioUtiliDiritto.Value, giorniServizioUtiliDiritto.Value);
                //double nSettimaneOI = Utility.CalcolaSettimane(anniServizioUtileDirittoOI.Value, mesiServizioUtileDirittoOI.Value, giorniServizioUtileDirittoOI.Value);
                if (Math.Abs(nSettimaneIT) < 52)
                {
                    messaggioVideo = "La differenza tra il numero di settimane totali utile al diritto e il numero settimane OI deve essere maggiore o ugule a 52.";
                    return false;
                }
            }

            if (!datiPensione.FineAssicurazione.HasValue)
            {
                messaggioVideo = "'Ultimo Versamento' non presente.";
                return false;
            }

            if (!divisore.HasValue || string.IsNullOrEmpty(capitolo) || capitolo == " ")
            {
                messaggioVideo = "I campi Divisore e Capitolo sono obbligatori";
                return false;
            }

            switch (tipocalcolo)
            {
                case GestioneContrib.TipoCalcolo.Retributivo:
                case GestioneContrib.TipoCalcolo.Misto:

                    if (lServizioUtile != null && lServizioUtile.Count > 0)
                    {
                        List<KeyValuePair<string, string>> lQuotaUltimoVers = null;
                        List<KeyValuePair<string, string>> lQuotaUltimoVersOverFineAss = null;

                        List<KeyValuePair<string, string>> lQuota = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("B3", "1997"),
                                                                                                 new KeyValuePair<string, string>("B2", "1995"),
                                                                                                 new KeyValuePair<string, string>("B1", "1994"),
                                                                                                 new KeyValuePair<string, string>("A",  "1992")};

                        lQuotaUltimoVers = lQuota.FindAll((x => (Convert.ToInt32(x.Value) <= datiPensione.FineAssicurazione.Value.Year)));

                        lQuotaUltimoVersOverFineAss = lQuota.FindAll((x => (Convert.ToInt32(x.Value) > datiPensione.FineAssicurazione.Value.Year)));
                        if (lQuotaUltimoVersOverFineAss != null && lQuotaUltimoVersOverFineAss.Count > 0)
                        {
                            if (lQuotaUltimoVers.Count == 0)
                                lQuotaUltimoVers.Add(lQuotaUltimoVersOverFineAss.Last());
                            else
                            {
                                if (datiPensione.FineAssicurazione.Value.Year > Convert.ToInt32(lQuotaUltimoVers.First().Value))
                                    lQuotaUltimoVers.Add(lQuotaUltimoVersOverFineAss.Last());
                            }

                            lQuotaUltimoVers.Sort((x, y) => string.Compare(y.Key, x.Key, false, System.Globalization.CultureInfo.CurrentUICulture));
                        }

                        GestioneDatiServizioUtileINPDAP.ServizioUtile ServizioUtileApp = null;
                        foreach (KeyValuePair<string, string> quota in lQuotaUltimoVers)
                        {
                            if (lServizioUtile.Find(x => x.Quota == quota.Key) != null)
                                ServizioUtileApp = lServizioUtile.Find(x => x.Quota == quota.Key);
                        }

                        GestioneDatiServizioUtileINPDAP.ServizioUtile ServizioUtileCessazione = lServizioUtile.Find(x => x.Quota == "B4");

                        if (ServizioUtileApp == null)
                        {
                            if (datiPensione.FineAssicurazione.Value.Year > Convert.ToInt32(lQuotaUltimoVers.First().Value))
                            {
                                if (ServizioUtileCessazione == null)
                                {
                                    messaggioVideo = "Dati Servizio Utile tra il 31/12/" + lQuotaUltimoVers.Last().Value + " e Servizio Utile Cessazione mancanti.";
                                    return false;
                                }
                            }
                            else
                            {
                                if (lQuotaUltimoVers.Count == 1)
                                    messaggioVideo = "Dati Servizio Utile al 31/12/" + lQuotaUltimoVers.Last().Value + " mancanti.";
                                else
                                    messaggioVideo = "Dati Servizio Utile tra il 31/12/" + lQuotaUltimoVers.Last().Value + " e il 31/12/" + lQuotaUltimoVers.First().Value + " mancanti.";
                                return false;
                            }
                        }

                        List<KeyValuePair<string, string>> lQuoteInserite = new List<KeyValuePair<string, string>>();
                        foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile servUtile in lServizioUtile)
                        {
                            if (servUtile.Quota != "B4")
                            {
                                KeyValuePair<string, string> q = lQuota.Find((x => (x.Key == servUtile.Quota)));
                                lQuoteInserite.Add(q);
                            }
                        }

                        short maxRangeAA = 0; short maxRangeMM = 0; short maxRangeGG = 0;

                        if (!VerificaPeriodoMaxConsentitoAll(datiPensione, lServizioUtile, lQuoteInserite, out maxRangeAA, out maxRangeMM, out maxRangeGG, out messaggioVideo))
                            return false;

                        if (ServizioUtileCessazione != null)
                        {
                            if (lQuotaUltimoVers.Count > 0)  // se data ultimo versamento < 1992 la lista sarà vuota --> bypass controllo (vale solo AA < 99, MM<11, GG<30)
                            {
                                if (!VerificaPeriodoMaxConsentitoCessazione(datiPensione, ServizioUtileCessazione, datiPensione.FineAssicurazione, out messaggioVideo))
                                    return false;
                            }

                            maxRangeAA += ServizioUtileCessazione.ServizioUtileCessazioneAA.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneAA.Value : (short)0;
                            maxRangeMM += ServizioUtileCessazione.ServizioUtileCessazioneMM.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneMM.Value : (short)0;
                            maxRangeGG += ServizioUtileCessazione.ServizioUtileCessazioneGG.HasValue ? ServizioUtileCessazione.ServizioUtileCessazioneGG.Value : (short)0;
                        }

                        if (!VerificaNumeroAnniServizioUtileDiritto(maxRangeAA, maxRangeMM, maxRangeGG, anniServizioUtiliDiritto, mesiServizioUtiliDiritto, giorniServizioUtiliDiritto,
                            datiPensione.DecorrenzaOriginaria, datiPensione, out messaggioVideo))
                            return false;
                    }
                    else
                    {
                        messaggioVideo = "Dati Servizio Utile mancanti";
                        return false;
                    }
                    break;
            }

            return true;
        }

        private static bool VerificaPeriodoMaxConsentitoAll(GestionePensione.DatiPensione datiPensione, List<INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiServizioUtile> lDatiServizioUtile,
            List<KeyValuePair<string, string>> lQuotaUltimoVers, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out short maxRangeAA,
            out short maxRangeMM, out short maxRangeGG, Utility.TipoFondo? tipoFondo, GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneLavorazione.DatiLavorazione datiLavorazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            maxRangeAA = 0; maxRangeMM = 0; maxRangeGG = 0;

            INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiServizioUtile ServizioUtileApp = null;
            //ENG - PL Reversibilita 024
            if (!datiPensione.InizioAssicurazione.HasValue)
            {
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione))) &&
                    !(tipoFondo == Utility.TipoFondo.FS && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.PRIMO_VERSAMENTO_FONDO_FS))
                    && !(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                    && !(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS)))
                {
                    messaggioVideo = "Data di Primo Versamento assente.";
                    return false;
                }
            }

            int aa9394_9697;
            int mm9394_9697;
            int gg9394_9697;
            int aa95;
            int mm95;
            int gg95;

            switch (tipoFondo)
            {
                case Utility.TipoFondo.PT:
                    aa9394_9697 = 2;
                    mm9394_9697 = 2;
                    gg9394_9697 = 0;
                    aa95 = 1;
                    mm95 = 1;
                    gg95 = 0;
                    break;
                case Utility.TipoFondo.FS:
                    aa9394_9697 = 2;
                    mm9394_9697 = 2;
                    gg9394_9697 = 12;
                    aa95 = 1;
                    mm95 = 1;
                    gg95 = 6;
                    break;
                default:
                    aa9394_9697 = 0;
                    mm9394_9697 = 0;
                    gg9394_9697 = 0;
                    aa95 = 0;
                    mm95 = 0;
                    gg95 = 0;
                    break;
            }

            foreach (KeyValuePair<string, string> quota in lQuotaUltimoVers)
            {
                ServizioUtileApp = lDatiServizioUtile.Find(x => x.Quota == quota.Key);

                if (lDatiServizioUtile.Find(x => x.Quota == quota.Key) != null)
                {
                    switch (quota.Key)
                    {
                        case "B3":
                        case "B1":
                            //commentato in funzione della mail del 10-07-2013
                            //if (!ServizioUtileApp.Retribuzione.HasValue && (ServizioUtileApp.ServizioUtileAA.HasValue || ServizioUtileApp.ServizioUtileMM.HasValue || ServizioUtileApp.ServizioUtileGG.HasValue))
                            //{
                            //    messaggioVideo = "Retribuzione Quota B1 obbligatoria in presenza dei 'Dati Servizio Utile al 31/12/" + quota.Value + "'";
                            //    return false;
                            //}

                            if (!VerificaPeriodoMaxConsentito(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, ServizioUtileApp.ServizioUtileAA,
                                ServizioUtileApp.ServizioUtileMM, ServizioUtileApp.ServizioUtileGG, "al 31/12/" + quota.Value, aa9394_9697, mm9394_9697, gg9394_9697, quota.Key, out messaggioVideo))
                                return false;

                            maxRangeAA += ServizioUtileApp.ServizioUtileAA.HasValue ? ServizioUtileApp.ServizioUtileAA.Value : (short)0;
                            maxRangeMM += ServizioUtileApp.ServizioUtileMM.HasValue ? ServizioUtileApp.ServizioUtileMM.Value : (short)0;
                            maxRangeGG += ServizioUtileApp.ServizioUtileGG.HasValue ? ServizioUtileApp.ServizioUtileGG.Value : (short)0;

                            break;
                        case "B2":
                            if (!VerificaPeriodoMaxConsentito(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, ServizioUtileApp.ServizioUtileAA,
                                ServizioUtileApp.ServizioUtileMM, ServizioUtileApp.ServizioUtileGG, "al 31/12/" + quota.Value, aa95, mm95, gg95, quota.Key, out messaggioVideo))
                                return false;

                            maxRangeAA += ServizioUtileApp.ServizioUtileAA.HasValue ? ServizioUtileApp.ServizioUtileAA.Value : (short)0;
                            maxRangeMM += ServizioUtileApp.ServizioUtileMM.HasValue ? ServizioUtileApp.ServizioUtileMM.Value : (short)0;
                            maxRangeGG += ServizioUtileApp.ServizioUtileGG.HasValue ? ServizioUtileApp.ServizioUtileGG.Value : (short)0;

                            break;
                        case "A":
                            //commentato in funzione della mail del 10-07-2013
                            //if (!ServizioUtileApp.Retribuzione.HasValue && (ServizioUtileApp.ServizioUtileAA.HasValue || ServizioUtileApp.ServizioUtileMM.HasValue || ServizioUtileApp.ServizioUtileGG.HasValue))
                            //{
                            //    messaggioVideo = "Retribuzione Quota A obbligatoria in presenza dei 'Dati Servizio Utile al 31/12/" + quota.Value + "'";
                            //    return false;
                            //}
                            //if (!ServizioUtileApp.ImportoIndennitaIntegrativaSpeciale.HasValue && (ServizioUtileApp.ServizioUtileAA.HasValue || ServizioUtileApp.ServizioUtileMM.HasValue || ServizioUtileApp.ServizioUtileGG.HasValue))
                            //{
                            //    messaggioVideo = "Importo Indennità Integrativa Speciale Quota A obbligatoria in presenza dei 'Dati Servizio Utile al 31/12/" + quota.Value + "'";
                            //    return false;
                            //}

                            Utility.DifferenzaDateTime dataCompare = null;

                            if (datiPensione.InizioAssicurazione.HasValue && datiPensione.InizioAssicurazione.Value.CompareTo(new DateTime(1992, 12, 31)) <= 0)
                            {
                                if (datiPensione.FineAssicurazione.Value.CompareTo(new DateTime(1992, 12, 31)) >= 0)
                                    dataCompare = Utility.DifferenzaBetweenDate(new DateTime(1993, 01, 01), datiPensione.InizioAssicurazione, Utility.TipoAppartenenza.FS);
                                else
                                    dataCompare = Utility.DifferenzaBetweenDate(datiPensione.FineAssicurazione.Value.AddDays(1), datiPensione.InizioAssicurazione, Utility.TipoAppartenenza.FS);
                            }

                            int aaMax = dataCompare != null ? dataCompare.Year : 0;
                            int mmMax = dataCompare != null ? dataCompare.Month : 0;
                            int ggMax = dataCompare != null ? dataCompare.Day : 0;

                            CalcolaPeriodoWithMaggiorazioneFS_PT(new DateTime(1992, 12, 31), datiPensione.InizioAssicurazione, ref aaMax, ref mmMax, ref ggMax);

                            if (!VerificaPeriodoMaxConsentito(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, ServizioUtileApp.ServizioUtileAA,
                                ServizioUtileApp.ServizioUtileMM, ServizioUtileApp.ServizioUtileGG, "al 31/12/" + quota.Value, aaMax, mmMax, ggMax, quota.Key, out messaggioVideo))
                                return false;

                            maxRangeAA += ServizioUtileApp.ServizioUtileAA.HasValue ? ServizioUtileApp.ServizioUtileAA.Value : (short)0;
                            maxRangeMM += ServizioUtileApp.ServizioUtileMM.HasValue ? ServizioUtileApp.ServizioUtileMM.Value : (short)0;
                            maxRangeGG += ServizioUtileApp.ServizioUtileGG.HasValue ? ServizioUtileApp.ServizioUtileGG.Value : (short)0;

                            break;
                    }
                }
            }

            return true;
        }

        private static bool VerificaPeriodoMaxConsentitoAll(GestionePensione.DatiPensione datiPensione, List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lDatiServizioUtile,
            List<KeyValuePair<string, string>> lQuotaUltimoVers, out short maxRangeAA, out short maxRangeMM, out short maxRangeGG, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            maxRangeAA = 0; maxRangeMM = 0; maxRangeGG = 0;

            GestioneDatiServizioUtileINPDAP.ServizioUtile ServizioUtileApp = null;

            if (!datiPensione.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "'Primo Versamento' non presente.";
                return false;
            }

            int aa9394_9697 = 2;
            int mm9394_9697 = 2;
            int gg9394_9697 = 12;
            int aa95 = 1;
            int mm95 = 1;
            int gg95 = 6;

            foreach (KeyValuePair<string, string> quota in lQuotaUltimoVers)
            {
                ServizioUtileApp = lDatiServizioUtile.Find(x => x.Quota == quota.Key);

                if (lDatiServizioUtile.Find(x => x.Quota == quota.Key) != null)
                {
                    switch (quota.Key)
                    {
                        case "B3":
                        case "B1":
                            //commentato in funzione della mail del 10-07-2013
                            //if (!ServizioUtileApp.Retribuzione.HasValue && (ServizioUtileApp.ServizioUtileAA.HasValue || ServizioUtileApp.ServizioUtileMM.HasValue || ServizioUtileApp.ServizioUtileGG.HasValue))
                            //{
                            //    messaggioVideo = "Retribuzione Quota B1 obbligatoria in presenza dei 'Dati Servizio Utile al 31/12/" + quota.Value + "'";
                            //    return false;
                            //}

                            if (!VerificaPeriodoMaxConsentito(datiPensione, ServizioUtileApp.ServizioUtileAA, ServizioUtileApp.ServizioUtileMM, ServizioUtileApp.ServizioUtileGG, "al 31/12/" + quota.Value, aa9394_9697, mm9394_9697, gg9394_9697, quota.Key, out messaggioVideo))
                                return false;

                            maxRangeAA += ServizioUtileApp.ServizioUtileAA.HasValue ? ServizioUtileApp.ServizioUtileAA.Value : (short)0;
                            maxRangeMM += ServizioUtileApp.ServizioUtileMM.HasValue ? ServizioUtileApp.ServizioUtileMM.Value : (short)0;
                            maxRangeGG += ServizioUtileApp.ServizioUtileGG.HasValue ? ServizioUtileApp.ServizioUtileGG.Value : (short)0;

                            break;
                        case "B2":
                            if (!VerificaPeriodoMaxConsentito(datiPensione, ServizioUtileApp.ServizioUtileAA, ServizioUtileApp.ServizioUtileMM, ServizioUtileApp.ServizioUtileGG, "al 31/12/" + quota.Value, aa95, mm95, gg95, quota.Key, out messaggioVideo))
                                return false;

                            maxRangeAA += ServizioUtileApp.ServizioUtileAA.HasValue ? ServizioUtileApp.ServizioUtileAA.Value : (short)0;
                            maxRangeMM += ServizioUtileApp.ServizioUtileMM.HasValue ? ServizioUtileApp.ServizioUtileMM.Value : (short)0;
                            maxRangeGG += ServizioUtileApp.ServizioUtileGG.HasValue ? ServizioUtileApp.ServizioUtileGG.Value : (short)0;

                            break;
                        case "A":
                            //commentato in funzione della mail del 10-07-2013
                            //if (!ServizioUtileApp.Retribuzione.HasValue && (ServizioUtileApp.ServizioUtileAA.HasValue || ServizioUtileApp.ServizioUtileMM.HasValue || ServizioUtileApp.ServizioUtileGG.HasValue))
                            //{
                            //    messaggioVideo = "Retribuzione Quota A obbligatoria in presenza dei 'Dati Servizio Utile al 31/12/" + quota.Value + "'";
                            //    return false;
                            //}
                            //if (!ServizioUtileApp.ImportoIndennitaIntegrativaSpeciale.HasValue && (ServizioUtileApp.ServizioUtileAA.HasValue || ServizioUtileApp.ServizioUtileMM.HasValue || ServizioUtileApp.ServizioUtileGG.HasValue))
                            //{
                            //    messaggioVideo = "Importo Indennità Integrativa Speciale Quota A obbligatoria in presenza dei 'Dati Servizio Utile al 31/12/" + quota.Value + "'";
                            //    return false;
                            //}

                            Utility.DifferenzaDateTime dataCompare = null;

                            if (datiPensione.InizioAssicurazione.Value.CompareTo(new DateTime(1992, 12, 31)) <= 0)
                            {
                                if (datiPensione.FineAssicurazione.Value.CompareTo(new DateTime(1992, 12, 31)) >= 0)
                                    dataCompare = Utility.DifferenzaBetweenDate(new DateTime(1993, 01, 01), datiPensione.InizioAssicurazione, Utility.TipoAppartenenza.FS);
                                else
                                    dataCompare = Utility.DifferenzaBetweenDate(datiPensione.FineAssicurazione.Value.AddDays(1), datiPensione.InizioAssicurazione, Utility.TipoAppartenenza.FS);
                            }

                            int aaMax = dataCompare != null ? dataCompare.Year : 0;
                            int mmMax = dataCompare != null ? dataCompare.Month : 0;
                            int ggMax = dataCompare != null ? dataCompare.Day : 0;

                            CalcolaPeriodoRecordFondoWithMaggiorazione(new DateTime(1992, 12, 31), datiPensione.InizioAssicurazione, ref aaMax, ref mmMax, ref ggMax);

                            if (!VerificaPeriodoMaxConsentito(datiPensione, ServizioUtileApp.ServizioUtileAA, ServizioUtileApp.ServizioUtileMM, ServizioUtileApp.ServizioUtileGG, "al 31/12/" + quota.Value, aaMax, mmMax, ggMax, quota.Key, out messaggioVideo))
                                return false;

                            maxRangeAA += ServizioUtileApp.ServizioUtileAA.HasValue ? ServizioUtileApp.ServizioUtileAA.Value : (short)0;
                            maxRangeMM += ServizioUtileApp.ServizioUtileMM.HasValue ? ServizioUtileApp.ServizioUtileMM.Value : (short)0;
                            maxRangeGG += ServizioUtileApp.ServizioUtileGG.HasValue ? ServizioUtileApp.ServizioUtileGG.Value : (short)0;

                            break;
                    }
                }
            }

            return true;
        }

        private static bool VerificaPeriodoMaxConsentitoCessazione(GestionePensione.DatiPensione datiPensione, INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiServizioUtile ServizioUtile, DateTime? UltimoVersamento,
            char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataUltimoServUtile = new DateTime(1998, 01, 01);

            if (!UltimoVersamento.HasValue)
            {
                messaggioVideo = "'Data Ultimo Versamento' mancanti";
                return false;
            }

            if (UltimoVersamento.Value.CompareTo(dataUltimoServUtile) <= 0)
            {
                messaggioVideo = "Dati Cessazione non compatibili con la data di ultimo versamento (" + String.Format("{0:dd/MM/yyy}", UltimoVersamento) + ")";
                return false;
            }

            int aaMax = 0;
            int mmMax = 0;
            int ggMax = 0;
            DateTime UltimoVersamentoApp;

            if (UltimoVersamento.Value.Day == dataUltimoServUtile.Day && UltimoVersamento.Value.Month == dataUltimoServUtile.Month)  // gg, mm di U.V. = gg, mm  dataUltimoServUtile
            {
                aaMax = UltimoVersamento.Value.Year - dataUltimoServUtile.Year;
                mmMax = 0;
                ggMax = 0;
            }
            else
            {
                if (UltimoVersamento.Value.Day != dataUltimoServUtile.Day && UltimoVersamento.Value.Month == dataUltimoServUtile.Month) // mm di U.V. = mm  dataUltimoServUtile
                {
                    UltimoVersamentoApp = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day);
                    aaMax = UltimoVersamentoApp.Year - dataUltimoServUtile.Year;
                    mmMax = UltimoVersamentoApp.Month;
                    ggMax = UltimoVersamentoApp.Day;
                }
                else // ultimo giorno del mese
                {
                    DateTime dtMeseSucc = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day).AddDays(1);
                    if (dtMeseSucc.Month > UltimoVersamento.Value.Month)
                    {
                        UltimoVersamentoApp = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day);
                        aaMax = UltimoVersamentoApp.Year - dataUltimoServUtile.Year;
                        mmMax = UltimoVersamento.Value.Month;
                        ggMax = 0;
                    }
                    else // generico
                    {
                        UltimoVersamentoApp = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day);
                        aaMax = UltimoVersamentoApp.Year - dataUltimoServUtile.Year;
                        mmMax = UltimoVersamento.Value.Month;
                        ggMax = UltimoVersamento.Value.Day;
                    }
                }
            }

            CalcolaPeriodoWithMaggiorazioneFS_PT(UltimoVersamento, dataUltimoServUtile.AddDays(-1), ref aaMax, ref mmMax, ref ggMax);

            if (!VerificaPeriodoMaxConsentito(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74, ServizioUtile.ServizioUtileCessazioneAA,
                ServizioUtile.ServizioUtileCessazioneMM, ServizioUtile.ServizioUtileCessazioneGG, "Cessazione", aaMax, mmMax, ggMax, string.Empty, out messaggioVideo))
                return false;

            return true;
        }

        private static bool VerificaPeriodoMaxConsentitoCessazione(GestionePensione.DatiPensione datiPensione, GestioneDatiServizioUtileINPDAP.ServizioUtile ServizioUtile, DateTime? UltimoVersamento,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataUltimoServUtile = new DateTime(1998, 01, 01);

            if (!UltimoVersamento.HasValue)
            {
                messaggioVideo = "'Data Ultimo Versamento' mancanti";
                return false;
            }

            if (UltimoVersamento.Value.CompareTo(dataUltimoServUtile) <= 0)
            {
                messaggioVideo = "Dati Cessazione non compatibili con la data di ultimo versamento (" + String.Format("{0:dd/MM/yyy}", UltimoVersamento) + ")";
                return false;
            }

            int aaMax = 0;
            int mmMax = 0;
            int ggMax = 0;
            DateTime UltimoVersamentoApp;

            if (UltimoVersamento.Value.Day == dataUltimoServUtile.Day && UltimoVersamento.Value.Month == dataUltimoServUtile.Month)  // gg, mm di U.V. = gg, mm  dataUltimoServUtile
            {
                aaMax = UltimoVersamento.Value.Year - dataUltimoServUtile.Year;
                mmMax = 0;
                ggMax = 0;
            }
            else
            {
                if (UltimoVersamento.Value.Day != dataUltimoServUtile.Day && UltimoVersamento.Value.Month == dataUltimoServUtile.Month) // mm di U.V. = mm  dataUltimoServUtile
                {
                    UltimoVersamentoApp = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day);
                    aaMax = UltimoVersamentoApp.Year - dataUltimoServUtile.Year;
                    mmMax = UltimoVersamentoApp.Month;
                    ggMax = UltimoVersamentoApp.Day;
                }
                else // ultimo giorno del mese
                {
                    DateTime dtMeseSucc = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day).AddDays(1);
                    if (dtMeseSucc.Month > UltimoVersamento.Value.Month)
                    {
                        UltimoVersamentoApp = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day);
                        aaMax = UltimoVersamentoApp.Year - dataUltimoServUtile.Year;
                        mmMax = UltimoVersamento.Value.Month;
                        ggMax = 0;
                    }
                    else // generico
                    {
                        UltimoVersamentoApp = new DateTime(UltimoVersamento.Value.Year, UltimoVersamento.Value.Month, UltimoVersamento.Value.Day);
                        aaMax = UltimoVersamentoApp.Year - dataUltimoServUtile.Year;
                        mmMax = UltimoVersamento.Value.Month;
                        ggMax = UltimoVersamento.Value.Day;
                    }
                }
            }

            CalcolaPeriodoRecordFondoWithMaggiorazione(UltimoVersamento, dataUltimoServUtile.AddDays(-1), ref aaMax, ref mmMax, ref ggMax);

            if (!VerificaPeriodoMaxConsentito(datiPensione, ServizioUtile.ServizioUtileCessazioneAA, ServizioUtile.ServizioUtileCessazioneMM, ServizioUtile.ServizioUtileCessazioneGG, "Cessazione", aaMax, mmMax, ggMax, string.Empty, out messaggioVideo))
                return false;

            return true;
        }

        private static bool VerificaNumeroAnniServizioUtileDiritto(short maxRangeAA, short maxRangeMM, short maxRangeGG, short? anniServizioUtiliDiritto, short? mesiServizioUtiliDiritto,
            short? giorniServizioUtiliDiritto, DateTime? decorrenzaOriginaria, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoFondo? fondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            //ENG - RIC Concessione Altra Pensione 024
            if ((fondo == Utility.TipoFondo.FS || fondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione)
                || Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione)))
                return true;

            short mm = 0;
            short aa = 0;
            short gg = 0;

            mm = (short)(maxRangeMM + (Math.Floor(maxRangeGG / 30M)));
            gg = (short)(maxRangeGG - ((Math.Floor(maxRangeGG / 30M) * 30M)));
            // Rif. memo 9/2016
            if (decorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2015, 5, 1)))
                if (gg > 15)
                    mm++;

            aa = (short)(maxRangeAA + (Math.Floor(mm / 12M)));
            mm = (short)(mm - ((Math.Floor(mm / 12M) * 12M)));
            if (mm > 11)
                aa++;

            if (anniServizioUtiliDiritto < aa)
            {
                messaggioVideo = "Anni Servizio Utili Diritto deve essere maggiore o uguale alla somma degli anni dei Dati Servizio Utile inseriti";
                return false;
            }
            else if (anniServizioUtiliDiritto == aa)
            {
                if (mesiServizioUtiliDiritto < mm)
                {
                    messaggioVideo = "Anni Servizio Utili Diritto deve essere maggiore o uguale alla somma degli anni dei Dati Servizio Utile inseriti";
                    return false;
                }
                else if (mesiServizioUtiliDiritto == mm)
                {
                    if (giorniServizioUtiliDiritto < gg)
                    {
                        messaggioVideo = "Anni Servizio Utili Diritto deve essere maggiore o uguale alla somma degli anni dei Dati Servizio Utile inseriti";
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool VerificaPeriodoMaxConsentito(GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74,
            short? ServizioUtileAA, short? ServizioUtileMM, short? ServizioUtileGG, string quotaRiferimento, int maxRangeAA, int maxRangeMM, int maxRangeGG, string quota, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!VerificaDatiServizioUtileObbligatori(datiPensione, ServizioUtileAA, ServizioUtileMM, quotaRiferimento, quota, out messaggioVideo))
                return false;

            if (BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                return true;

            if (ServizioUtileAA.GetValueOrDefault() > maxRangeAA)
            {
                if (maxRangeAA == 0)
                    messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " AA deve essere 0";
                else
                    messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " AA deve essere compreso tra 0 e " + maxRangeAA;
                return false;
            }
            else
            {
                if (ServizioUtileAA.GetValueOrDefault() == maxRangeAA)
                {
                    if (ServizioUtileMM.GetValueOrDefault() > maxRangeMM)
                    {
                        if (maxRangeMM == 0)
                            messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " MM deve essere essere 0";
                        else
                            messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " MM deve essere compreso tra 0 e " + maxRangeMM;
                        return false;
                    }
                    else if (ServizioUtileMM.GetValueOrDefault() == maxRangeMM && ServizioUtileGG.GetValueOrDefault() > maxRangeGG)
                    {
                        messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " GG deve essere " + maxRangeGG;
                        return false;
                    }
                    else
                    {
                        if (ServizioUtileGG.GetValueOrDefault() > 30)
                        {
                            messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " GG deve essere compreso tra 0 e 30";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool VerificaPeriodoMaxConsentito(GestionePensione.DatiPensione datiPensione, short? ServizioUtileAA, short? ServizioUtileMM, short? ServizioUtileGG, string quotaRiferimento, int maxRangeAA, int maxRangeMM,
            int maxRangeGG, string quota, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!VerificaDatiServizioUtileObbligatori(datiPensione, ServizioUtileAA, ServizioUtileMM, quotaRiferimento, quota, out messaggioVideo))
                return false;

            if (ServizioUtileAA.GetValueOrDefault() > maxRangeAA)
            {
                if (maxRangeAA == 0)
                    messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " AA deve essere 0";
                else
                    messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " AA deve essere compreso tra 0 e " + maxRangeAA;
                return false;
            }
            else
            {
                if (ServizioUtileAA.GetValueOrDefault() == maxRangeAA)
                {
                    if (ServizioUtileMM.GetValueOrDefault() > maxRangeMM)
                    {
                        if (maxRangeMM == 0)
                            messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " MM deve essere essere 0";
                        else
                            messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " MM deve essere compreso tra 0 e " + maxRangeMM;
                        return false;
                    }
                    else if (ServizioUtileMM.GetValueOrDefault() == maxRangeMM && ServizioUtileGG.GetValueOrDefault() > maxRangeGG)
                    {
                        messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " GG deve essere " + maxRangeGG;
                        return false;
                    }
                    else
                    {
                        if (ServizioUtileGG.GetValueOrDefault() > 30)
                        {
                            messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " GG deve essere compreso tra 0 e 30";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool VerificaDatiServizioUtileObbligatori(GestionePensione.DatiPensione datiPensione, short? ServizioUtileAA, short? ServizioUtileMM, string quotaRiferimento, string quota, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? dataCompare = null;

            if (datiPensione == null)
            {
                messaggioVideo = "Errore nel recupero delle informazioni.";
                return false;
            }

            Utility.TipoFondo? fondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (fondo.HasValue && ((fondo.Value == Utility.TipoFondo.ET) || ((fondo.Value == Utility.TipoFondo.FS || fondo.Value == Utility.TipoFondo.PT) &&
                (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione)))))
                return true;

            if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) && (Utility.IsDomandaINPDAP(datiPensione.Gestione) || fondo == Utility.TipoFondo.FS || fondo == Utility.TipoFondo.PT))
            {
                return true;
            }

            switch (quota)
            {
                case "A":
                    dataCompare = new DateTime(1992, 12, 31);
                    break;
                case "B1":
                    dataCompare = new DateTime(1994, 12, 31);
                    break;
                case "B2":
                    dataCompare = new DateTime(1995, 12, 31);
                    break;
                case "B3":
                    dataCompare = new DateTime(1997, 12, 31);
                    break;
                default:
                    dataCompare = new DateTime(9999, 12, 31);
                    break;
            }

            if (datiPensione.InizioAssicurazione.HasValue &&
                !Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.Value, dataCompare.Value) && !ServizioUtileAA.HasValue && !ServizioUtileMM.HasValue)
            {
                messaggioVideo = "Dati Servizio Utile " + quotaRiferimento + " AA e/o MM obbligatori";
                return false;
            }

            return true;
        }

        public static bool ControlsDatiCalcoloET(INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiCalcolo datiCalcolo, Utility.TipoFondo? tipoFondo,
            GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, bool isInvioAlCalcolo, char? codiceSpecificoTraduzioneSuGP,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int maxRangeAA = 0;
            int maxRangeMM = 0;
            int maxRangeGG = 0;
            string periodo = string.Empty;
            string quota = string.Empty;

            if (datiPensione == null)
            {
                messaggioVideo = "Dati Pensione mancanti";
                return false;
            }

            if (!datiPensione.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Inizio Assicurazione è obbligatoria";
                return false;
            }

            DateTime dataLimite = new DateTime(1996, 1, 1);
            if (datiCalcolo.TipoCalcolo != GestioneContrib.TipoCalcolo.Contributivo)
            {
                if (!Utility.DataSuccessivaA(dataLimite, datiPensione.InizioAssicurazione.Value))
                {
                    messaggioVideo = "La data di Inizio Assicurazione non è compatibile con i Dati Servizio Utile";
                    return false;
                }
            }

            if (datiCalcolo != null)
            {
                if (datiCalcolo.fondoET != null && datiCalcolo.fondoET.lDatiServizioUtile != null && datiCalcolo.fondoET.lDatiServizioUtile.Count > 0)
                {
                    dataLimite = new DateTime(1993, 1, 1);
                    foreach (INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiServizioUtile datiServizioUtile in datiCalcolo.fondoET.lDatiServizioUtile)
                    {

                        switch (datiServizioUtile.Quota)
                        {
                            case "A":
                                CalcolaMaxRangeAAMMGG(datiPensione.InizioAssicurazione.Value, dataLimite, out maxRangeAA, out maxRangeMM, out maxRangeGG);
                                periodo = "al 01/01/93";
                                quota = "(Quota A)";
                                if (!ControlsServizioUtileWithRetribPensionabile(datiServizioUtile.ServizioUtileAA, datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG,
                                    datiServizioUtile.RetribuzionePensionabile, datiServizioUtile.ControCodiceRetributivo, periodo, quota, datiPensione, isInvioAlCalcolo, out messaggioVideo))
                                    return false;
                                break;
                            case "B":
                                maxRangeAA = 2;
                                maxRangeMM = 0;
                                maxRangeGG = 0;
                                periodo = "al 31/12/92";
                                quota = "(Quota B)";
                                string periodoB = "al 31/12/92";
                                string quotaB = "(Quota B)";
                                string periodoC = "al 31/12/94";
                                string quotaC = "(Quota C)";
                                INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiServizioUtile elementoC = datiCalcolo.fondoET.lDatiServizioUtile.FirstOrDefault(x => x.Quota == "C");
                                if (!ControlsServizioUtileWithRetribPensionabile(datiServizioUtile.ServizioUtileAA, datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG,
                                    datiServizioUtile.RetribuzionePensionabile, datiServizioUtile.ControCodiceRetributivo, periodoB, quotaB, datiPensione,
                                    elementoC != null ? elementoC.ServizioUtileAA : null,
                                    elementoC != null ? elementoC.ServizioUtileCessazioneMM : null,
                                    elementoC != null ? elementoC.ServizioUtileGG : null, periodoC, quotaC, isInvioAlCalcolo, out messaggioVideo))
                                    return false;
                                break;
                            case "C":
                                maxRangeAA = 1;
                                maxRangeMM = 0;
                                maxRangeGG = 0;
                                periodo = "al 31/12/94";
                                quota = "(Quota C)";
                                break;
                        }
                        if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio) && (datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("01") ||
                            datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("02") || datiMaggiorazioniBenefici.TipoSettimaneBeneficio.Equals("03")))
                        {
                            if (!VerificaDatiServizioUtileObbligatori(datiPensione, datiServizioUtile.ServizioUtileAA, datiServizioUtile.ServizioUtileMM, periodo, string.Empty, out messaggioVideo))
                                return false;
                        }
                        else
                        {
                            if (!VerificaPeriodoMaxConsentito(datiPensione, codiceSpecificoTraduzioneSuGP, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneAmianto : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.MaggiorazioneInv74 : null,
                                datiServizioUtile.ServizioUtileAA, datiServizioUtile.ServizioUtileMM, datiServizioUtile.ServizioUtileGG, periodo, maxRangeAA, maxRangeMM, maxRangeGG, string.Empty, out messaggioVideo))
                                return false;
                        }
                    }
                }

                if ((datiCalcolo.RMSQuotaB.HasValue && !datiCalcolo.NSettimaneQuotaB.HasValue) || (!datiCalcolo.RMSQuotaB.HasValue && datiCalcolo.NSettimaneQuotaB.HasValue))
                {
                    messaggioVideo = "Numero Settimane B obbligatorie in presenza della Retribuzione Media Settimanale B e viceversa";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsELDatiCalcoloAnteArmonizzazione(List<GestioneDatiServizioUtile.ServizioUtile> lstDatiServizioUtile, GestioneFondo.DatiFondo datiFondo,
            GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, Utility.TipoFondo? tipoFondo, bool isInvioAlCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria,
                datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondo: datiFondo, datiServizioUtile: lstDatiServizioUtile))
                return true;

            if (!datiPensione.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Inizio Assicurazione è obbligatoria";
                return false;
            }
            if (!datiPensione.FineAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Fine Assicurazione è obbligatoria";
                return false;
            }

            DateTime endQuotaA = new DateTime(1992, 12, 31);
            DateTime startQuotaB = new DateTime(1993, 1, 1);
            DateTime endQuotaB = new DateTime(1994, 12, 31);
            DateTime startQuotaC = new DateTime(1995, 1, 1);
            DateTime inizioAssicurazione = datiPensione.InizioAssicurazione.Value;
            DateTime fineAssicurazione = datiPensione.FineAssicurazione.Value;
            DateTime dataLimite = new DateTime(1996, 1, 1);

            if (!Utility.DataSuccessivaA(dataLimite, datiPensione.InizioAssicurazione.Value))
            {
                messaggioVideo = "La data di Inizio Assicurazione non è compatibile con i Dati Servizio Utile";
                return false;
            }
            //La quota A è obbligatoria
            if (lstDatiServizioUtile == null || !lstDatiServizioUtile.Exists(x => x.Quota == "A"))
            {
                // ENG - Bypass ASSENZA_QUOTA_A_B_C_D_OBBLIGATORIA in fase di invio al calcolo
                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.ASSENZA_QUOTA_A_B_C_D_OBBLIGAT))
                {
                    messaggioVideo = "La quota A dei Dati Servizio Utile è obbligatoria.";
                    return false;
                }
            }
            if (lstDatiServizioUtile != null && lstDatiServizioUtile.Count > 0)
            {
                //quota A
                Utility.DifferenzaDateTime periodoMaxQuota;
                GestioneDatiServizioUtile.ServizioUtile quotaA = lstDatiServizioUtile.Find(x => x.Quota == "A");
                if (quotaA == null || (quotaA.ServizioUtileAA == 0 && quotaA.ServizioUtileMM == 0))
                {
                    // ENG - Bypass ASSENZA_QUOTA_A_B_C_D_OBBLIGATORIA in fase di invio al calcolo
                    if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.ASSENZA_QUOTA_A_B_C_D_OBBLIGAT))
                    {
                        messaggioVideo = "La quota A dei Dati Servizio Utile è obbligatoria.";
                        return false;
                    }
                }
                if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                {
                    periodoMaxQuota = Utility.DifferenzaBetweenDate(endQuotaA, inizioAssicurazione, Utility.TipoAppartenenza.FS);
                    Utility.DifferenzaDateTime periodoQuota = new Utility.DifferenzaDateTime(quotaA.ServizioUtileAA.GetValueOrDefault(), quotaA.ServizioUtileMM.GetValueOrDefault(), 0);
                    if (periodoQuota > periodoMaxQuota)
                    {
                        messaggioVideo = string.Format("Il massimo periodo inseribile per la quota A è {0} AA e {1} MM .", periodoMaxQuota.Year, periodoMaxQuota.Month);
                        return false;
                    }
                }
                if (quotaA != null)
                {
                    if (!quotaA.ServizioUtileAA.HasValue || !quotaA.ServizioUtileMM.HasValue)
                    {
                        messaggioVideo = "Per quota A deve essere valorizzato AA e MM.";
                        return true;
                    }
                    if (!isInvioAlCalcolo && (!quotaA.RetribuzionePensionabile.HasValue || !quotaA.ControCodiceRetributivo.HasValue))
                    {
                        messaggioVideo = "Per quota A Retribuzione Pensionabile e il ControCodice sono dati obbligatori.";
                        return false;
                    }
                    //controllo controcodice
                    if (!isInvioAlCalcolo && !CheckImportoWithControCodice(quotaA.RetribuzionePensionabile, quotaA.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                        return false;
                }

                //quota B
                GestioneDatiServizioUtile.ServizioUtile quotaB = lstDatiServizioUtile.Find(x => x.Quota == "B");
                if (datiPensione.FineAssicurazione >= startQuotaB)
                {
                    if (!(Utility.IsDomandaReversibilita(datiPensione) && datiFondo != null && datiFondo.CodiceRequisiti2 != 0) &&
                        (quotaB == null || (quotaB.ServizioUtileAA == 0 && quotaB.ServizioUtileMM == 0)))
                    {
                        // ENG - Bypass ASSENZA_QUOTA_A_B_C_D_OBBLIGATORIA in fase di invio al calcolo
                        if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.ASSENZA_QUOTA_A_B_C_D_OBBLIGAT))
                        {
                            messaggioVideo = "La quota B dei Dati Servizio Utile è obbligatoria.";
                            return false;
                        }
                    }
                }

                if (quotaB != null)
                {
                    if (!quotaB.ServizioUtileAA.HasValue || !quotaB.ServizioUtileMM.HasValue)
                    {
                        messaggioVideo = "Per quota B deve essere valorizzato  AA e MM.";
                        return true;
                    }
                    if (!isInvioAlCalcolo && (!quotaB.RetribuzionePensionabile.HasValue || !quotaB.ControCodiceRetributivo.HasValue))
                    {
                        messaggioVideo = "Per quota B Retribuzione Pensionabile e il ControCodice sono dati obbligatori.";
                        return true;
                    }
                    if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                    {
                        //Controllo capienza
                        DateTime dataFineQuotaB = fineAssicurazione > endQuotaB ? endQuotaB : fineAssicurazione;
                        periodoMaxQuota = Utility.DifferenzaBetweenDate(dataFineQuotaB.AddDays(1), startQuotaB, Utility.TipoAppartenenza.FS);
                        Utility.DifferenzaDateTime periodoQuota = new Utility.DifferenzaDateTime(quotaB.ServizioUtileAA.GetValueOrDefault(), quotaB.ServizioUtileMM.GetValueOrDefault(), 0);
                        if (periodoQuota > periodoMaxQuota)
                        {
                            messaggioVideo = string.Format("Il massimo periodo inseribile per la quota B è {0} AA e {1} MM .", periodoMaxQuota.Year, periodoMaxQuota.Month);
                            return false;
                        }
                    }
                    //controllo controcodice
                    if (!isInvioAlCalcolo && !CheckImportoWithControCodice(quotaB.RetribuzionePensionabile, quotaB.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                        return false;
                }
                //quota C
                GestioneDatiServizioUtile.ServizioUtile quotaC = lstDatiServizioUtile.Find(x => x.Quota == "C");
                if (datiPensione.FineAssicurazione >= startQuotaC)
                {
                    if ((!(Utility.IsDomandaReversibilita(datiPensione) && datiFondo != null && datiFondo.CodiceRequisiti2 != 0) &&
                        (quotaC == null || (quotaC.ServizioUtileAA == 0 && quotaC.ServizioUtileMM == 0))) && !(Utility.IsRicostituzione(datiPensione.Gruppo) &&
                        Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) && quotaC == null && datiFondo != null && datiFondo.CodiceRequisiti2 == '6'))
                    {
                        // ENG - Bypass ASSENZA_QUOTA_A_B_C_D_OBBLIGATORIA in fase di invio al calcolo
                        if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.ASSENZA_QUOTA_A_B_C_D_OBBLIGAT))
                        {
                            messaggioVideo = "La quota C dei Dati Servizio Utile è obbligatoria.";
                            return false;
                        }
                    }
                }
                if (quotaC != null)
                {
                    if (!quotaC.ServizioUtileAA.HasValue || !quotaC.ServizioUtileMM.HasValue)
                    {
                        messaggioVideo = "Per quota C deve essere valorizzato  AA e MM.";
                        return true;
                    }
                    if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                    {
                        periodoMaxQuota = Utility.DifferenzaBetweenDate(fineAssicurazione.AddDays(1), startQuotaC, Utility.TipoAppartenenza.FS);
                        Utility.DifferenzaDateTime periodoQuota = new Utility.DifferenzaDateTime(quotaC.ServizioUtileAA.GetValueOrDefault(), quotaC.ServizioUtileMM.GetValueOrDefault(), 0);
                        if (periodoQuota > periodoMaxQuota)
                        {
                            messaggioVideo = string.Format("Il massimo periodo inseribile per la quota C è {0} AA e {1} MM .", periodoMaxQuota.Year, periodoMaxQuota.Month);
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        public static bool ControlsDatiLegge407AnteArmonizzazione(List<GestioneMaggiorazioniBenefici.DatiServizioUtileDL407> lstDatiServizioUtile,
            GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (lstDatiServizioUtile != null && lstDatiServizioUtile.Count > 0)
            {
                //quota A
                GestioneMaggiorazioniBenefici.DatiServizioUtileDL407 quotaA = lstDatiServizioUtile.Find(x => x.Quota == "A");
                if (quotaA != null)
                {
                    //ENG - Bypassato controllo per tutte le RIC NON CONTRIBUTIVE COME PER LA QUOTA B
                    if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                        (!quotaA.RetribuzionePensionabile.HasValue || !quotaA.ControCodiceRetributivo.HasValue || !quotaA.ServizioUtileAA.HasValue))
                    {
                        messaggioVideo = "Per quota A Retribuzione Pensionabile, il ControCodice e Servizio Utile AA sono dati obbligatori.";
                        return false;
                    }

                    if (quotaA.ServizioUtileAA > 5)
                    {
                        messaggioVideo = "Per quota A Servizio Utile AA non può essere maggiore di 5.";
                        return false;
                    }
                    //controllo controcodice
                    if (!CheckImportoWithControCodice(quotaA.RetribuzionePensionabile, quotaA.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                        return false;
                }
                //quota B
                GestioneMaggiorazioniBenefici.DatiServizioUtileDL407 quotaB = lstDatiServizioUtile.Find(x => x.Quota == "B");
                if (quotaB != null)
                {
                    if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                        (!quotaB.RetribuzionePensionabile.HasValue || !quotaB.ControCodiceRetributivo.HasValue || !quotaB.ServizioUtileAA.HasValue))
                    {
                        messaggioVideo = "Per quota B Retribuzione Pensionabile, il ControCodice e Servizio Utile AA sono dati obbligatori.";
                        return false;
                    }
                    if (quotaB.ServizioUtileAA > 2)
                    {
                        messaggioVideo = "Per quota B Servizio Utile AA non può essere maggiore di 2.";
                        return false;
                    }
                    //controllo controcodice
                    if (!CheckImportoWithControCodice(quotaB.RetribuzionePensionabile, quotaB.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                        return false;
                }
                //quota C
                GestioneMaggiorazioniBenefici.DatiServizioUtileDL407 quotaC = lstDatiServizioUtile.Find(x => x.Quota == "C");
                if (quotaC != null)
                {
                    if (quotaC.ServizioUtileAA > 2)
                    {
                        messaggioVideo = "Per quota C Servizio Utile AA non può essere maggiore di 2.";
                        return false;
                    }
                }
            }
            else
            {
                messaggioVideo = "Inserire almeno un dato nel tab MaggiorazioniBenefici\\DL407.";
                return false;
            }

            return true;
        }

        public static void CalcolaMaxRangeAAMMGG(DateTime? inizioAssicurazione, DateTime dataLimite, out int maxRangeAA, out int maxRangeMM, out int maxRangeGG)
        {
            maxRangeAA = dataLimite.Year - (inizioAssicurazione.Value.Year + 1);
            maxRangeMM = 0;
            maxRangeGG = 31 - inizioAssicurazione.Value.Day;

            if (dataLimite.Month == inizioAssicurazione.Value.Month)
            {
                maxRangeAA++;
                //maxRangeMM = 0;
                //maxRangeGG = (short)(31 - inizioAssicurazione.Value.Day);
            }
            else
            {
                maxRangeMM = 12 - inizioAssicurazione.Value.Month;
                //maxRangeGG = (short)(31 - inizioAssicurazione.Value.Day);
            }

            // Questa gestione è fatta per incrementare il mese nel caso in cui il maxRangeGG superi il numero massimo di giorni del mese successivo
            if (DateTime.DaysInMonth(inizioAssicurazione.Value.Year, maxRangeMM + 1) <= maxRangeGG)
            {
                maxRangeMM++;
                maxRangeGG = maxRangeGG - DateTime.DaysInMonth(inizioAssicurazione.Value.Year, maxRangeMM);
            }

            // Mesi devono essere da 30 gg
            if (maxRangeGG > 29)
            {
                maxRangeGG = 0;
                maxRangeMM++;
                if (maxRangeMM > 11)
                {
                    maxRangeMM = 0;
                    maxRangeAA++;
                }
            }

            if (maxRangeMM > 6 || (maxRangeMM > 5 && maxRangeGG > 0))
            {
                maxRangeAA++;
                maxRangeMM = 0;
                maxRangeGG = 0;
            }
        }

        private static bool ControlsServizioUtileWithRetribPensionabile(short? maxRangeAA, short? maxRangeMM, short? maxRangeGG, decimal? retribuzionePensionabile, int? controcodice, string periodo, string quota,
            GestionePensione.DatiPensione datiPensione, bool isInvioAlCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((maxRangeAA.HasValue || maxRangeMM.HasValue || maxRangeGG.HasValue) && !retribuzionePensionabile.HasValue)
            {
                messaggioVideo = "La 'Retribuzione Pensionabile' è obbligatoria in presenza dei 'Dati Servizio Utile " + periodo + " " + quota + "'";
                return false;
            }

            if ((!maxRangeAA.HasValue && !maxRangeMM.HasValue && !maxRangeGG.HasValue) && retribuzionePensionabile.HasValue)
            {
                messaggioVideo = "I 'Dati Servizio Utile " + periodo + " " + quota + "' sono obbligatori in presenza della 'Retribuzione Pensionabile'";
                return false;
            }

            // Il controllo non va eseguito all'invio al calcolo e per le domande automatiche e solo in fase di invio al calcolo se è una ricostituzione non contributiva
            if (!isInvioAlCalcolo &&
                !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica) &&
                !(Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
            {
                if (retribuzionePensionabile.HasValue && !controcodice.HasValue)
                {
                    messaggioVideo = "Il Controcodice Retributivo è obbligatorio in presenza della 'Retribuzione Pensionabile' " + quota;
                    return false;
                }
            }


            return true;
        }

        private static bool ControlsServizioUtileWithRetribPensionabile(short? AAquotaB, short? MMquotaB, short? GGquotaB, decimal? retribuzionePensionabile, int? controcodice, string periodoB, string quotaB,
            GestionePensione.DatiPensione datiPensione, short? AAquotaC, short? MMquotaC, short? GGquotaC, string periodoC, string quotaC, bool isInvioAlCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (((AAquotaB.HasValue || MMquotaB.HasValue || GGquotaB.HasValue) || (AAquotaC.HasValue || MMquotaC.HasValue || GGquotaC.HasValue))
                && !retribuzionePensionabile.HasValue)
            {
                messaggioVideo = "La 'Retribuzione Pensionabile' è obbligatoria in presenza dei 'Dati Servizio Utile " + periodoB + " " + quotaB + " o " +
                    periodoC + " " + quotaC + "'";
                return false;
            }

            if (((!AAquotaB.HasValue && !MMquotaB.HasValue && !GGquotaB.HasValue) && (!AAquotaC.HasValue && !MMquotaC.HasValue && !GGquotaC.HasValue)) && retribuzionePensionabile.HasValue)
            {
                messaggioVideo = "I 'Dati Servizio Utile " + periodoB + " " + quotaB + " o " +
                    periodoC + " " + quotaC + "' sono obbligatori in presenza della 'Retribuzione Pensionabile'";
                return false;
            }

            // Il controllo non va eseguito sull'invio al calcolo e per le domande automatiche e solo in fase di invio al calcolo se è una ricostituzione non contributiva
            if (!isInvioAlCalcolo &&
                !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica) &&
                !(Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
            {
                if (retribuzionePensionabile.HasValue && !controcodice.HasValue)
                {
                    messaggioVideo = "Il Controcodice Retributivo è obbligatorio in presenza della 'Retribuzione Pensionabile' " + quotaB;
                    return false;
                }
            }

            return true;
        }

        private static bool ControlsRetrPonderataAnnuaObbligatoria(decimal? retribuzionePonderataAnnua, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (!retribuzionePonderataAnnua.HasValue || retribuzionePonderataAnnua.Value == 0))
            {
                messaggioVideo = "La retribuzione AGO annua è obbligatoria e deve essere maggiore di 0";
                return false;
            }
            return true;
        }

        private static int GetSettimaneMontiETAmmesse(GestionePensione.DatiPensione datiPensione,
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile, char? codiceSpecifico)
        {
            int settimaneAmmesse = 835;
            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0001") ||
            (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0011") ||
            Utility.IsDomandaPensioneInabilitaOrRicostituzioneFS(datiPensione, codiceSpecifico))
            {
                settimaneAmmesse = 992;
                if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                {
                    foreach (GestioneDatiServizioUtile.ServizioUtile sU in listaDatiServizioUtile)
                    {
                        if (sU.Quota == "B" || sU.Quota == "C")
                        {
                            int nSettimane = (int)(((sU.ServizioUtileAA.GetValueOrDefault() * 360) +
                                (sU.ServizioUtileMM.GetValueOrDefault() * 30) +
                                sU.ServizioUtileGG.GetValueOrDefault()) / 6.923M);
                            settimaneAmmesse = settimaneAmmesse - nSettimane;
                        }
                    }
                }
            }
            return settimaneAmmesse;
        }

        public static bool ControlsObbligatorietaForCodiceTipoLiquidazione(byte? codiceTipoLiquidazione, decimal? rmsQuotaA, decimal? rmsQuotaB, int? nSettimaneQuotaA, int? nSettimaneQuotaB, decimal? contributiTotaliSupplementoDPR143271, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceTipoLiquidazione.HasValue)
            {
                switch (codiceTipoLiquidazione)
                {
                    case 0:
                    case 4:
                    case 7:
                    case 9:
                        if (nSettimaneQuotaB.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Inserire le Settimane totali per la quota B";
                            return false;
                        }
                        break;
                    case 1:
                        if (rmsQuotaA.GetValueOrDefault() == 0 || rmsQuotaB.GetValueOrDefault() == 0 || nSettimaneQuotaA.GetValueOrDefault() == 0 || nSettimaneQuotaB.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Inserire RMS e Settimane totali per le quote A e B";
                            return false;
                        }
                        break;
                    case 2:
                        if (nSettimaneQuotaB.GetValueOrDefault() == 0 || contributiTotaliSupplementoDPR143271.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Inserire le Settimane totali per la quota B e i Contrib. Totali";
                            return false;
                        }
                        break;
                    case 3:
                    case 5:
                    case 6:
                    case 8:
                        messaggioVideo = "Tipo Liquidazione non valido";
                        return false;
                }
            }

            return true;
        }

        private static void CalcolaPeriodoWithMaggiorazioneFS_PT(DateTime? dataMaggiore, DateTime? dataMinore, ref int aaMax, ref int mmMax, ref int ggMax)
        {
            int app;
            int giorni1;
            int giorni2;
            if (dataMaggiore.HasValue && dataMinore.HasValue)
            {
                giorni1 = dataMaggiore.Value.Year * 372 + dataMaggiore.Value.Month * 31 + dataMaggiore.Value.Day;
                giorni2 = dataMinore.Value.Year * 372 + dataMinore.Value.Month * 31 + dataMinore.Value.Day;
                app = giorni1 - giorni2 + 1;
                app = ((app / 372) * 37);
                aaMax += app / 372;
                app = app % 372;
                mmMax += app / 31;
                ggMax += app % 31;

                while (ggMax > 30)
                {
                    mmMax += 1;
                    ggMax -= 31;
                }
                while (mmMax > 11)
                {
                    aaMax += 1;
                    mmMax -= 12;
                }
            }
        }

        public static bool VerificaDecorrenzaTeorica(DateTime? decorrenzaTeorica, DateTime? inizioBonus, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaTeorica.HasValue)
            {
                if (inizioBonus.HasValue && decorrenzaTeorica.Value != inizioBonus.Value)
                {
                    messaggioVideo = "La data di decorrenza teorica deve coincidere con la data di inizio bonus indicata nel tab 'Dati Generici'";
                    return false;
                }

                if (decorrenzaTeorica > decorrenzaOriginaria)
                {
                    messaggioVideo = "La data 'Decorrenza Teorica' non deve superare la data 'Decorrenza Pensione'";
                    return false;
                }
            }

            return true;
        }

        public static bool VerificaRetribuzionePensionabileQuotaA_ET(GestionePensione.DatiPensione datiPensione, byte? idTipoCalcolo, decimal? retribuzionePensionabileQuotaA, decimal? stipendio,
            decimal? importo13ma, decimal? importo14ma, decimal? elementiAccessori, decimal? Competenze40Percento, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            decimal? valoreMinimo = new List<decimal?> { elementiAccessori, Competenze40Percento }.Min();

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcoloById(idTipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS);
            switch (tipoCalcolo)
            {
                case Utility.TipoCalcolo.Retributivo:
                case Utility.TipoCalcolo.RetributivoMonti:
                case Utility.TipoCalcolo.Misto:
                    decimal? sommaApp = stipendio.GetValueOrDefault() + importo13ma.GetValueOrDefault() + importo14ma.GetValueOrDefault() + valoreMinimo.GetValueOrDefault();
                    if (retribuzionePensionabileQuotaA.GetValueOrDefault() != sommaApp.GetValueOrDefault() &&
                        Math.Abs(retribuzionePensionabileQuotaA.GetValueOrDefault() - sommaApp.GetValueOrDefault()) > 0.0001m)
                    {
                        messaggioVideo = "La Retribuzione Pensionabile Quota 'A' deve essere uguale alla somma dei campi Stipendio, Tredicesima, Quattordicesima e valore minore tra Elementi Accessori e 40% delle Competenze(" +
                            (stipendio.GetValueOrDefault() + importo13ma.GetValueOrDefault() + importo14ma.GetValueOrDefault() + valoreMinimo.GetValueOrDefault()).ToString(System.Globalization.CultureInfo.CurrentUICulture) + ").";
                        return false;
                    }
                    break;
            }

            return true;
        }

        public static bool ControlsDatiServizioUtile(List<GestioneContrib.DatiServizioUtile> lDatiServizioUtile, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtileApp = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (GestioneContrib.DatiServizioUtile servizioUtile in lDatiServizioUtile)
                {
                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                    lDatiServizioUtileApp.Add(datiServizioUtile);
                }

                if (!GestioneControlli.ControlsDatiServizioUtile(lDatiServizioUtileApp, out messaggioVideo))
                    return false;
            }
            return true;
        }

        public static bool ControlsDatiServizioUtileWithFineAssicurazione(List<GestioneContrib.DatiServizioUtile> lDatiServizioUtileApp, DateTime? fineAssicurazione,
            Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? periodo = null;

            if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && fineAssicurazione.HasValue &&
                !Utility.DataStrettamenteSuccessivaA(fineAssicurazione.Value, new DateTime(1992, 12, 31)))
                return true;

            //ENG - RIC NON CONTRIBUTIVE 024
            if (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                return true;


            if (lDatiServizioUtileApp != null)
            {
                foreach (GestioneContrib.DatiServizioUtile datiServizioUtile in lDatiServizioUtileApp)
                {
                    switch (datiServizioUtile.Quota)
                    {
                        case "A":
                            continue;
                        case "B1":
                            periodo = new DateTime(1992, 12, 31);
                            break;
                        case "B2":
                            periodo = new DateTime(1994, 12, 31);
                            break;
                        case "B3":
                            periodo = new DateTime(1995, 12, 31);
                            break;
                        case "B4":
                            periodo = new DateTime(1997, 12, 31);
                            break;
                    }

                    if (periodo > fineAssicurazione)
                    {
                        messaggioVideo = "Dati Servizio Utile al " + String.Format("{0:dd/MM/yyyy}", periodo) + " non compatibili con la data di fine assicurazione (" + String.Format("{0:dd/MM/yyyy}", fineAssicurazione) + ")";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsDatiServizioUtile(List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtileApp, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (lDatiServizioUtileApp != null && lDatiServizioUtileApp.Count > 0)
            {
                foreach (GestioneDatiServizioUtile.ServizioUtile datiServizioUtile in lDatiServizioUtileApp)
                {
                    if (!IsValoreAAMMGGValido(datiServizioUtile.ServizioUtileAA, null, null))//datiServizioUtile.ServizioUtileAA.HasValue && datiServizioUtile.ServizioUtileAA.Value.ToString().Length > 2)
                    {
                        messaggioVideo = "Servizio Utile AA (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 99";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, datiServizioUtile.ServizioUtileMM, null))//datiServizioUtile.ServizioUtileMM.HasValue && (datiServizioUtile.ServizioUtileMM.Value.ToString().Length > 2 || datiServizioUtile.ServizioUtileMM.Value > 11))
                    {
                        messaggioVideo = "Servizio Utile MM (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 11";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, null, datiServizioUtile.ServizioUtileGG))//datiServizioUtile.ServizioUtileGG.HasValue && (datiServizioUtile.ServizioUtileGG.Value.ToString().Length > 2 || datiServizioUtile.ServizioUtileGG.Value > 31))
                    {
                        messaggioVideo = "Servizio Utile GG (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 29";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(datiServizioUtile.ServizioUtileCessazioneAA, null, null))
                    {
                        messaggioVideo = "Servizio Utile Cessazione AA deve essere compreso tra 0 e 99";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, datiServizioUtile.ServizioUtileCessazioneMM, null))
                    {
                        messaggioVideo = "Servizio Utile Cessazione MM deve essere compreso tra 0 e 11";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, null, datiServizioUtile.ServizioUtileCessazioneGG))
                    {
                        messaggioVideo = "Servizio Utile Cessazione GG deve essere compreso tra 0 e 29";
                        return false;
                    }

                    if (datiServizioUtile.ControCodiceRetributivo.HasValue && datiServizioUtile.ControCodiceRetributivo.Value.ToString().Length > 3)
                    {
                        messaggioVideo = "Controcodice Retr (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 999";
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ControlsDatiServizioUtile(List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lDatiServizioUtileApp, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (lDatiServizioUtileApp != null && lDatiServizioUtileApp.Count > 0)
            {
                foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile in lDatiServizioUtileApp)
                {
                    if (!IsValoreAAMMGGValido(datiServizioUtile.ServizioUtileAA, null, null))//datiServizioUtile.ServizioUtileAA.HasValue && datiServizioUtile.ServizioUtileAA.Value.ToString().Length > 2)
                    {
                        messaggioVideo = "Servizio Utile AA (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 99";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, datiServizioUtile.ServizioUtileMM, null))//datiServizioUtile.ServizioUtileMM.HasValue && (datiServizioUtile.ServizioUtileMM.Value.ToString().Length > 2 || datiServizioUtile.ServizioUtileMM.Value > 11))
                    {
                        messaggioVideo = "Servizio Utile MM (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 11";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, null, datiServizioUtile.ServizioUtileGG))//datiServizioUtile.ServizioUtileGG.HasValue && (datiServizioUtile.ServizioUtileGG.Value.ToString().Length > 2 || datiServizioUtile.ServizioUtileGG.Value > 31))
                    {
                        messaggioVideo = "Servizio Utile GG (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 29";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(datiServizioUtile.ServizioUtileCessazioneAA, null, null))
                    {
                        messaggioVideo = "Servizio Utile Cessazione AA deve essere compreso tra 0 e 99";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, datiServizioUtile.ServizioUtileCessazioneMM, null))
                    {
                        messaggioVideo = "Servizio Utile Cessazione MM deve essere compreso tra 0 e 11";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, null, datiServizioUtile.ServizioUtileCessazioneGG))
                    {
                        messaggioVideo = "Servizio Utile Cessazione GG deve essere compreso tra 0 e 29";
                        return false;
                    }

                    if (datiServizioUtile.ControCodiceRetributivo.HasValue && datiServizioUtile.ControCodiceRetributivo.Value.ToString().Length > 3)
                    {
                        messaggioVideo = "Controcodice Retr (Quota " + datiServizioUtile.Quota + ") deve essere compreso tra 0 e 999";
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ControlsDatiServizioUtileWithFineAssicurazione(List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtileApp, DateTime? fineAssicurazione,
            Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? periodo = null;

            if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && fineAssicurazione.HasValue &&
                !Utility.DataStrettamenteSuccessivaA(fineAssicurazione.Value, new DateTime(1992, 12, 31)))
                return true;

            //ENG - RIC NON CONTRIBUTIVE 024
            if (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                return true;


            if (lDatiServizioUtileApp != null)
            {
                foreach (GestioneDatiServizioUtile.ServizioUtile datiServizioUtile in lDatiServizioUtileApp)
                {
                    switch (datiServizioUtile.Quota)
                    {
                        case "A":
                            continue;
                        case "B1":
                            periodo = new DateTime(1992, 12, 31);
                            break;
                        case "B2":
                            periodo = new DateTime(1994, 12, 31);
                            break;
                        case "B3":
                            periodo = new DateTime(1995, 12, 31);
                            break;
                        case "B4":
                            periodo = new DateTime(1997, 12, 31);
                            break;
                    }

                    if (periodo > fineAssicurazione)
                    {
                        messaggioVideo = "Dati Servizio Utile al " + String.Format("{0:dd/MM/yyyy}", periodo) + " non compatibili con la data di fine assicurazione (" + String.Format("{0:dd/MM/yyyy}", fineAssicurazione) + ")";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsDatiServizioUtileWithFineAssicurazione(List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lDatiServizioUtileApp, DateTime? fineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? periodo = null;

            if (lDatiServizioUtileApp != null)
            {
                foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile in lDatiServizioUtileApp)
                {
                    switch (datiServizioUtile.Quota)
                    {
                        case "A":
                            continue;
                        case "B1":
                            periodo = new DateTime(1992, 12, 31);
                            break;
                        case "B2":
                            periodo = new DateTime(1994, 12, 31);
                            break;
                        case "B3":
                            periodo = new DateTime(1995, 12, 31);
                            break;
                        case "B4":
                            periodo = new DateTime(1997, 12, 31);
                            break;
                    }

                    if (periodo > fineAssicurazione)
                    {
                        messaggioVideo = "Dati Servizio Utile al " + String.Format("{0:dd/MM/yyyy}", periodo) + " non compatibili con la data di fine assicurazione (" + String.Format("{0:dd/MM/yyyy}", fineAssicurazione) + ")";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsAnniServizioUtiliDiritto(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            short? anniServizioUtileDiritto, short? mesiServizioUtileDiritto, short? giorniServizioUtileDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            decimal coefficienteAnni = 52;
            decimal coefficienteMesi = 4.333M;
            decimal coefficienteGiorni = 6.923M;

            decimal sommaSettimaneUtiliAlDirittoApp = 0;

            if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio) && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01" &&
                Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 01)))
            {
                if (anniServizioUtileDiritto.HasValue || mesiServizioUtileDiritto.HasValue || giorniServizioUtileDiritto.HasValue)
                {
                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    {
                        sommaSettimaneUtiliAlDirittoApp += Convert.ToDecimal(anniServizioUtileDiritto * coefficienteAnni + mesiServizioUtileDiritto * coefficienteMesi + giorniServizioUtileDiritto / coefficienteGiorni);
                    }
                    else if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.FS:
                                sommaSettimaneUtiliAlDirittoApp += Convert.ToDecimal(anniServizioUtileDiritto * coefficienteAnni + mesiServizioUtileDiritto * coefficienteMesi + giorniServizioUtileDiritto / coefficienteGiorni);
                                break;
                            case Utility.TipoFondo.PT:
                                sommaSettimaneUtiliAlDirittoApp += Convert.ToDecimal(anniServizioUtileDiritto * coefficienteAnni + mesiServizioUtileDiritto * coefficienteMesi + giorniServizioUtileDiritto / coefficienteGiorni);
                                break;
                        }
                    }
                }
                int sommaSettimaneUtiliAlDiritto = (int)Math.Ceiling(sommaSettimaneUtiliAlDirittoApp);

                if (sommaSettimaneUtiliAlDiritto < 520)
                {
                    messaggioVideo = "Anni Servizio Utili Diritto deve essere maggiore o uguale a 520 settimane";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Esegue i controlli riguardanti i dati calcolo del Comma 707
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="datiCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDatiComma707(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, INPS.Pensioni.LiquidazioneFs.GestioneContrib.DatiCalcolo datiCalcolo,
            char? codiceSpecificoTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!GestioneContrib.IsSettimane707Visible(datiPensione, codiceSpecificoTraduzioneSuGP, datiCalcolo != null ? !datiCalcolo.IsContribL214Null() : false))
            {
                if (datiCalcolo != null && (datiCalcolo.QuotaA2707.HasValue || datiCalcolo.QuotaA707.HasValue || datiCalcolo.QuotaB707.HasValue || datiCalcolo.QuotaC2707.HasValue ||
                    datiCalcolo.QuotaC707.HasValue || datiCalcolo.QuotaD707.HasValue))
                {
                    messaggioVideo = "Non è possibile acquisire i dati 'Calcolo ex Comma 707'.";
                    return false;
                }
                else
                    return true;
            }

            switch (tipoFondo)
            {
                case Utility.TipoFondo.TT:
                case Utility.TipoFondo.EL:
                    if (datiCalcolo == null)
                    {
                        messaggioVideo = "Dati calcolo non presenti.";
                        return false;
                    }
                    if (datiCalcolo.QuotaA707.HasValue != datiCalcolo.NSettimaneQuotaA.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota A' deve essere valorizzato anche il campo 'Quota A' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }
                    if (datiCalcolo.QuotaB707.HasValue != datiCalcolo.NSettimaneQuotaB.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota B' deve essere valorizzato anche il campo 'Quota B' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }
                    if (datiCalcolo.QuotaC707.HasValue != datiCalcolo.NSettimaneQuotaC.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota C' deve essere valorizzato anche il campo 'Quota C' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }
                    if (datiCalcolo.QuotaD707.HasValue != datiCalcolo.NSettimaneQuotaD.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota D' deve essere valorizzato anche il campo 'Quota D' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaA.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota A' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane A.";
                        return false;
                    }

                    if (datiCalcolo.QuotaB707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaB.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota B' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane B.";
                        return false;
                    }

                    if (datiCalcolo.QuotaC707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaC.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota C' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane C.";
                        return false;
                    }

                    if (datiCalcolo.QuotaD707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaD.GetValueOrDefault() + datiCalcolo.NSettimaneQuotaDL214.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota D' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alla somma tra le Settimane D e L.214.";
                        return false;
                    }

                    if (!datiCalcolo.RetribuzionePonderataAGO707.HasValue)
                    {
                        messaggioVideo = "La Retribuzione ponderata AGO per calcolo limite è obbligatoria.";
                        return false;
                    }
                    break;
                case Utility.TipoFondo.VL:
                    if (datiCalcolo == null)
                    {
                        messaggioVideo = "Dati calcolo non presenti.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA707.HasValue ^ datiCalcolo.NSettimaneQuotaA.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota A1' deve essere valorizzato anche il campo 'Quota A1' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA2707.HasValue ^ datiCalcolo.NSettimaneQuotaA2.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota A2' deve essere valorizzato anche il campo 'Quota A2' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaB707.HasValue ^ datiCalcolo.NSettimaneQuotaB.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota B' deve essere valorizzato anche il campo 'Quota B' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaC707.HasValue ^ datiCalcolo.NSettimaneQuotaC.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota C1' deve essere valorizzato anche il campo 'Quota C1' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaC2707.HasValue ^ datiCalcolo.NSettimaneQuotaC2.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota C2' deve essere valorizzato anche il campo 'Quota C2' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaD707.HasValue ^ datiCalcolo.NSettimaneQuotaD.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota D' deve essere valorizzato anche il campo 'Quota D' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaA.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota A1' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane A1.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA2707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaA2.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota A2' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane A2.";
                        return false;
                    }

                    if (datiCalcolo.QuotaB707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaB.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota B' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane B.";
                        return false;
                    }

                    if (datiCalcolo.QuotaC707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaC.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota C1' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane C1.";
                        return false;
                    }

                    if (datiCalcolo.QuotaC2707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaC2.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota C2' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane C2.";
                        return false;
                    }

                    if (datiCalcolo.QuotaD707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaD.GetValueOrDefault() + datiCalcolo.NSettimaneQuotaDL214.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota D' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alla somma tra le Settimane D e L.214.";
                        return false;
                    }
                    break;
                case Utility.TipoFondo.DZ:
                case Utility.TipoFondo.PM:
                case Utility.TipoFondo.ES:
                    if (datiCalcolo == null)
                    {
                        messaggioVideo = "Dati calcolo non presenti.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA707.HasValue ^ datiCalcolo.NSettimaneQuotaA.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota A' deve essere valorizzato anche il campo 'Quota A' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaB707.HasValue ^ datiCalcolo.NSettimaneQuotaB.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota B' deve essere valorizzato anche il campo 'Quota B' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaA.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota A' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane A.";
                        return false;
                    }

                    if (datiCalcolo.QuotaB707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaB.GetValueOrDefault() + datiCalcolo.NSettimaneQuotaDL214.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota B' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alla somma tra le Settimane B e L.214.";
                        return false;
                    }
                    break;
                case Utility.TipoFondo.GAS:
                    if (datiCalcolo == null)
                    {
                        messaggioVideo = "Dati calcolo non presenti.";
                        return false;
                    }

                    if (datiCalcolo.QuotaA707.HasValue ^ datiCalcolo.NSettimaneQuotaA.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota A' deve essere valorizzato anche il campo 'Quota A' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }

                    if (datiCalcolo.QuotaB707.HasValue ^ datiCalcolo.NSettimaneQuotaB.HasValue)
                    {
                        messaggioVideo = "In presenza di 'Settimane Quota B' deve essere valorizzato anche il campo 'Quota B' nella sezione 'Calcolo ex Comma 707' e viceversa.";
                        return false;
                    }
                    if (datiCalcolo.QuotaA707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaA.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota A' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane A.";
                        return false;
                    }

                    if (datiCalcolo.QuotaB707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaB.GetValueOrDefault() + datiCalcolo.NSettimaneQuotaDL214.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Quota B' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alla somma tra le Settimane B e L.214.";
                        return false;
                    }
                    if (datiCalcolo.QuotaAES707.GetValueOrDefault() < datiCalcolo.NSettimaneEsclusiveQuotaA.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane esclusiva della 'Quota A' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane esclusive della 'Quota A'.";
                        return false;
                    }
                    if (datiCalcolo.QuotaBES707.GetValueOrDefault() < datiCalcolo.NSettimaneEsclusiveQuotaB.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane esclusiva della 'Quota B' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alle Settimane esclusive della 'Quota B'.";
                        return false;
                    }
                    break;
                case Utility.TipoFondo.ET:
                    if (datiCalcolo == null)
                    {
                        messaggioVideo = "Dati calcolo non presenti.";
                        return false;
                    }

                    if (datiCalcolo.NSettimaneQuotaA.GetValueOrDefault() == 0 && datiCalcolo.QuotaA707.GetValueOrDefault() != 0)
                    {
                        messaggioVideo = "Se 'Settimane A' è pari a 0 allora 'Settimane quota A AGO' non può essere diverso da 0.";
                        return false;
                    }

                    if (datiCalcolo.NSettimaneQuotaB.GetValueOrDefault() == 0 && datiCalcolo.QuotaB707.GetValueOrDefault() != 0)
                    {
                        messaggioVideo = "Se 'Settimane B' è pari a 0 allora 'Settimane quota B AGO' non può essere diverso da 0.";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(datiCalcolo.QuotaA707AA, null, null))
                    {
                        messaggioVideo = "Quota A Fondo AA deve essere compreso tra 0 e 99";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, datiCalcolo.QuotaA707MM, null))
                    {
                        messaggioVideo = "Quota A Fondo MM deve essere compreso tra 0 e 11";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, null, datiCalcolo.QuotaA707GG))
                    {
                        messaggioVideo = "Quota A Fondo GG deve essere compreso tra 0 e 29";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(datiCalcolo.QuotaB707AA, null, null))
                    {
                        messaggioVideo = "Quota B Fondo AA deve essere compreso tra 0 e 99";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, datiCalcolo.QuotaB707MM, null))
                    {
                        messaggioVideo = "Quota B Fondo MM deve essere compreso tra 0 e 11";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, null, datiCalcolo.QuotaB707GG))
                    {
                        messaggioVideo = "Quota B Fondo GG deve essere compreso tra 0 e 29";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(datiCalcolo.QuotaC707AA, null, null))
                    {
                        messaggioVideo = "Quota C Fondo AA deve essere compreso tra 0 e 99";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, datiCalcolo.QuotaC707MM, null))
                    {
                        messaggioVideo = "Quota C Fondo MM deve essere compreso tra 0 e 11";
                        return false;
                    }

                    if (!IsValoreAAMMGGValido(null, null, datiCalcolo.QuotaC707GG))
                    {
                        messaggioVideo = "Quota C Fondo GG deve essere compreso tra 0 e 29";
                        return false;
                    }

                    if (datiCalcolo.fondoET != null && datiCalcolo.fondoET.lDatiServizioUtile != null && datiCalcolo.fondoET.lDatiServizioUtile.Count > 0)
                    {
                        foreach (GestioneContrib.DatiServizioUtile servizioUtile in datiCalcolo.fondoET.lDatiServizioUtile)
                        {
                            switch (servizioUtile.Quota)
                            {
                                case "A":
                                    Utility.DifferenzaDateTime servizioUtileQuotaA = new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.GetValueOrDefault(), servizioUtile.ServizioUtileMM.GetValueOrDefault(), servizioUtile.ServizioUtileGG.GetValueOrDefault());
                                    Utility.DifferenzaDateTime comma707QuotaA = new Utility.DifferenzaDateTime(datiCalcolo.QuotaA707AA.GetValueOrDefault(), datiCalcolo.QuotaA707MM.GetValueOrDefault(), datiCalcolo.QuotaA707GG.GetValueOrDefault());

                                    if (comma707QuotaA < servizioUtileQuotaA)
                                    {
                                        messaggioVideo = "Il periodo della 'Quota A Fondo' della sezione 'Calcolo ex Comma 707' non può essere inferiore al periodo dei 'Dati Ante 01/01/93 (Quota A)'";
                                        return false;
                                    }
                                    break;
                                case "B":
                                    Utility.DifferenzaDateTime servizioUtileQuotaB = new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.GetValueOrDefault(), servizioUtile.ServizioUtileMM.GetValueOrDefault(), servizioUtile.ServizioUtileGG.GetValueOrDefault());
                                    Utility.DifferenzaDateTime comma707QuotaB = new Utility.DifferenzaDateTime(datiCalcolo.QuotaB707AA.GetValueOrDefault(), datiCalcolo.QuotaB707MM.GetValueOrDefault(), datiCalcolo.QuotaB707GG.GetValueOrDefault());

                                    if (comma707QuotaB < servizioUtileQuotaB)
                                    {
                                        messaggioVideo = "Il periodo della 'Quota B Fondo' della sezione 'Calcolo ex Comma 707' non può essere inferiore al periodo dei 'Dati Post 31/12/92 (Quota B)'";
                                        return false;
                                    }
                                    break;
                                case "C":
                                    Utility.DifferenzaDateTime servizioUtileQuotaC = new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.GetValueOrDefault(), servizioUtile.ServizioUtileMM.GetValueOrDefault(), servizioUtile.ServizioUtileGG.GetValueOrDefault());
                                    Utility.DifferenzaDateTime comma707QuotaC = new Utility.DifferenzaDateTime(datiCalcolo.QuotaC707AA.GetValueOrDefault(), datiCalcolo.QuotaC707MM.GetValueOrDefault(), datiCalcolo.QuotaC707GG.GetValueOrDefault());

                                    if (comma707QuotaC < servizioUtileQuotaC)
                                    {
                                        messaggioVideo = "Il periodo della 'Quota C Fondo' della sezione 'Calcolo ex Comma 707' non può essere inferiore al periodo dei 'Dati Post 31/12/94 (Quota C)'";
                                        return false;
                                    }
                                    break;
                            }
                        }
                    }

                    if (datiCalcolo.QuotaB707.GetValueOrDefault() < datiCalcolo.NSettimaneQuotaB.GetValueOrDefault() + datiCalcolo.NSettimaneQuotaDL214.GetValueOrDefault())
                    {
                        messaggioVideo = "Le settimane della 'Settimane quota B AGO' della sezione 'Calcolo ex Comma 707' non possono essere inferiori alla somma tra le Settimane B e L.214.";
                        return false;
                    }

                    if (!datiCalcolo.RetribuzionePonderataAGO707.HasValue)
                    {
                        messaggioVideo = "La Retribuzione ponderata AGO per calcolo limite è obbligatoria.";
                        return false;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// Verifica il valore del contro codice retributivo per le pensioni con categoria *PIU
        /// Il contro codice retributivo deve essere uguale ai primi tre decimali del resto risultante dalla divisione tra (El. Retr.+ stipendio base + Pens.Compl.Riv.1/95) / 999
        /// </summary>
        /// <param name="stipendioAnnuo"></param>
        /// <param name="stipendioBase"></param>
        /// <param name="pensComplRiv1_95"></param>
        /// <param name="controCodiceRetributivo"></param>
        /// <param name="datiPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsControCodiceRetributivoPIU(decimal? stipendioAnnuo, decimal? stipendioBase, decimal? pensComplRiv1_95, short? controCodiceRetributivo,
            GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            decimal somma = (stipendioAnnuo.GetValueOrDefault() + stipendioBase.GetValueOrDefault() + pensComplRiv1_95.GetValueOrDefault());

            if (!GestioneControlli.CheckImportoWithControCodice(somma, controCodiceRetributivo, datiPensione, out messaggioVideo))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica che il campo Pens. Compl. Riv. 1/95 non sia valorizzato per pensioni con decorrenza successiva o uguale al 01/1995
        /// </summary>
        /// <param name="pensComplRiv1_95"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsPensComplRiv195PIU(decimal? pensComplRiv1_95, DateTime? decorrenzaPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaPensione.HasValue && Utility.DataSuccessivaA(decorrenzaPensione.Value, new DateTime(1995, 1, 1)) && pensComplRiv1_95.HasValue)
            {
                messaggioVideo = "Il campo Pens. Compl. Riv. 1/95 non deve essere valorizzato per pensioni con decorrenza successiva al 01/1995.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Controlla che i valori delle settimane rientrino nelle capienze
        /// Settimane quota A: calcolato con la differenza tra la data inizio assicurazione e  il 31/12/92
        /// Settimane quota B: calcolato con la differenza tra la data fine assicurazione e  il 01/01/93
        /// </summary>
        /// <param name="nSettimaneQuotaA"></param>
        /// <param name="nSettimaneQuotaB"></param>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsCapienzaSettimanePIV(short? nSettimaneQuotaA, short? nSettimaneQuotaB, DateTime? inizioAssicurazione, DateTime? fineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (nSettimaneQuotaA.HasValue && !inizioAssicurazione.HasValue)
            {
                messaggioVideo = "Inizio assicurazione mancante.";
                return false;
            }

            if (nSettimaneQuotaB.HasValue && !fineAssicurazione.HasValue)
            {
                messaggioVideo = "Fine assicurazione mancante.";
                return false;
            }

            int capienzaQuotaA = inizioAssicurazione.HasValue ? Utility.NSettimaneBetweenDate(new DateTime(1992, 12, 31), inizioAssicurazione.Value) : 0;
            int capienzaQuotaB = fineAssicurazione.HasValue ? Utility.NSettimaneBetweenDate(fineAssicurazione.Value, new DateTime(1993, 01, 01)) : 0;

            if (nSettimaneQuotaA.GetValueOrDefault() > capienzaQuotaA)
            {
                messaggioVideo = "Settimane A maggiori della capienza (" + capienzaQuotaA + ")";
                return false;
            }

            if (nSettimaneQuotaB.GetValueOrDefault() > capienzaQuotaB)
            {
                messaggioVideo = "Settimane B maggiori della capienza (" + capienzaQuotaB + ")";
                return false;
            }

            return true;
        }

        public static bool ControlsVLDatiCalcoloAnteArmonizzazione(List<GestioneDatiServizioUtile.ServizioUtile> lstDatiServizioUtile, GestionePensione.DatiPensione datiPensione,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, Utility.TipoFondo? tipoFondo,
            bool isInvioAlCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria,
                datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                return true;

            if (!datiPensione.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Inizio Assicurazione è obbligatoria";
                return false;
            }
            if (!datiPensione.FineAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Fine Assicurazione è obbligatoria";
                return false;
            }

            Dictionary<DateTime, DateTime> listaPeriodi = new Dictionary<DateTime, DateTime>();
            listaPeriodi.Add(datiPensione.InizioAssicurazione.Value, new DateTime(1988, 11, 27));
            listaPeriodi.Add(new DateTime(1988, 11, 28), new DateTime(1992, 12, 31));
            listaPeriodi.Add(new DateTime(1993, 1, 1), new DateTime(1994, 12, 31));
            listaPeriodi.Add(new DateTime(1995, 1, 1), datiPensione.FineAssicurazione.Value);
            List<string> listaQuote = new List<string> { "A", "A2", "B", "C" };

            if (!Utility.DataSuccessivaA(new DateTime(1996, 1, 1), datiPensione.InizioAssicurazione.Value))
            {
                messaggioVideo = "La data di Inizio Assicurazione non è compatibile con i Dati Servizio Utile";
                return false;
            }
            //La quota A e A2 sono obbligatorie
            if (lstDatiServizioUtile == null || !lstDatiServizioUtile.Exists(x => x.Quota == "A" || x.Quota == "A2"))
            {
                messaggioVideo = "La quota A dei Dati Servizio Utile è obbligatoria.";
                return false;
            }

            if (lstDatiServizioUtile != null && lstDatiServizioUtile.Count > 0)
            {
                #region Controlli Capienza
                int index = 0;
                foreach (KeyValuePair<DateTime, DateTime> periodo in listaPeriodi)
                {
                    GestioneDatiServizioUtile.ServizioUtile servizioUtile = lstDatiServizioUtile.Find(x => x.Quota == listaQuote[index]);

                    DateTime dataInferiore;
                    DateTime dataSuperiore;
                    string msg = string.Empty;

                    switch (listaQuote[index])
                    {
                        case "A":
                            msg = "ante 27/11/88";
                            break;
                        case "A2":
                            msg = "ante '93";
                            break;
                        case "B":
                            msg = "post '92";
                            break;
                        case "C":
                            msg = "post '94";
                            break;
                    }

                    if (datiPensione.InizioAssicurazione.Value > periodo.Key)
                        dataInferiore = datiPensione.InizioAssicurazione.Value;
                    else
                        dataInferiore = periodo.Key;

                    if (datiPensione.FineAssicurazione.Value < periodo.Value)
                        dataSuperiore = datiPensione.FineAssicurazione.Value;
                    else
                        dataSuperiore = periodo.Value;

                    if (dataInferiore < dataSuperiore)
                    {
                        Utility.DifferenzaDateTime periodoMaxQuota = Utility.DifferenzaBetweenDate(dataSuperiore.AddDays(1), dataInferiore, Utility.TipoAppartenenza.FS);
                        Utility.DifferenzaDateTime periodoQuota = servizioUtile != null ?
                            new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.GetValueOrDefault(), servizioUtile.ServizioUtileMM.GetValueOrDefault(), servizioUtile.ServizioUtileGG.GetValueOrDefault()) :
                            new Utility.DifferenzaDateTime();

                        if (servizioUtile == null && periodoMaxQuota != new Utility.DifferenzaDateTime())
                        {
                            messaggioVideo = string.Format("Il Servizio utile {0} è obbligatorio per inizio assicurazione {1:dd/MM/yyyy} e fine assicurazione {2:dd/MM/yyyy}.",
                                msg, datiPensione.InizioAssicurazione.Value, datiPensione.FineAssicurazione.Value);
                            return false;
                        }

                        if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                        {
                            if (periodoQuota > periodoMaxQuota)
                            {
                                messaggioVideo = string.Format("Il massimo periodo inseribile per il Servizio utile {0} è {1} AA, {2} MM, {3} GG.", msg, periodoMaxQuota.Year, periodoMaxQuota.Month, periodoMaxQuota.Day);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!Utility.IsDomandaReversibilita(datiPensione))
                        {
                            if (servizioUtile != null &&
                                (servizioUtile.ServizioUtileAA.GetValueOrDefault() != 0 || servizioUtile.ServizioUtileMM.GetValueOrDefault() != 0 || servizioUtile.ServizioUtileGG.GetValueOrDefault() != 0 ||
                                servizioUtile.RetribuzionePensionabile.GetValueOrDefault() != 0M || servizioUtile.Retribuzione.GetValueOrDefault() != 0M ||
                                servizioUtile.ControCodiceRetributivo.GetValueOrDefault() != 0))
                            {
                                messaggioVideo = string.Format("Il Servizio utile {0} non deve essere inserito per inizio assicurazione {1:dd/MM/yyyy} e fine assicurazione {2:dd/MM/yyyy}.",
                                    msg, datiPensione.InizioAssicurazione.Value, datiPensione.FineAssicurazione.Value);
                                return false;
                            }
                        }
                    }

                    index++;
                }
                #endregion Controlli Capienza

                // Servizio Utile ante 27/11/88 e ante '93
                #region Quota A e A2
                GestioneDatiServizioUtile.ServizioUtile quotaA = lstDatiServizioUtile.Find(x => x.Quota == "A");
                GestioneDatiServizioUtile.ServizioUtile quotaA2 = lstDatiServizioUtile.Find(x => x.Quota == "A2");

                if (// Se è presente il servizio utile ma non la retribuzione
                    (((quotaA != null && (quotaA.ServizioUtileAA.GetValueOrDefault() > 0 || quotaA.ServizioUtileMM.GetValueOrDefault() > 0 || quotaA.ServizioUtileGG.GetValueOrDefault() > 0)) ||
                    (quotaA2 != null && (quotaA2.ServizioUtileAA.GetValueOrDefault() > 0 || quotaA2.ServizioUtileMM.GetValueOrDefault() > 0 || quotaA2.ServizioUtileGG.GetValueOrDefault() > 0))) &&
                    (quotaA == null || !quotaA.RetribuzionePensionabile.HasValue))
                    ||
                    // Se è presente la retribuzione ma non il servizio utile
                    (quotaA != null && (quotaA.RetribuzionePensionabile.HasValue) &&
                    !((quotaA != null && (quotaA.ServizioUtileAA.GetValueOrDefault() > 0 || quotaA.ServizioUtileMM.GetValueOrDefault() > 0 || quotaA.ServizioUtileGG.GetValueOrDefault() > 0)) ||
                    (quotaA2 != null && (quotaA2.ServizioUtileAA.GetValueOrDefault() > 0 || quotaA2.ServizioUtileMM.GetValueOrDefault() > 0 || quotaA2.ServizioUtileGG.GetValueOrDefault() > 0))))
                    )
                {
                    messaggioVideo = "Per quota A per Retribuzione Pensionabile e il ControCodice presenti, deve essere presente anche il servizio utile e viceversa.";
                    return false;
                }
                if (!isInvioAlCalcolo)
                {
                    if (quotaA != null && quotaA.ControCodiceRetributivo.HasValue && !quotaA.RetribuzionePensionabile.HasValue)
                    {
                        messaggioVideo = "Per la quota A non è possibile inserire il controcodice senza la retribuzione.";
                        return false;
                    }

                    if (quotaA != null && quotaA.RetribuzionePensionabile.HasValue && quotaA.ControCodiceRetributivo.HasValue)
                    {
                        //controllo controcodice
                        if (!CheckImportoWithControCodice(quotaA.RetribuzionePensionabile, quotaA.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                            return false;
                    }
                }
                #endregion Quota A e A2

                // Servizio Utile post '92 (ante '95)
                #region Quota B
                GestioneDatiServizioUtile.ServizioUtile quotaB = lstDatiServizioUtile.Find(x => x.Quota == "B");
                if (// Se è presente il servizio utile ma non la retribuzione
                    ((quotaB != null && (quotaB.ServizioUtileAA.GetValueOrDefault() > 0 || quotaB.ServizioUtileMM.GetValueOrDefault() > 0 || quotaB.ServizioUtileGG.GetValueOrDefault() > 0)) &&
                    (!quotaB.RetribuzionePensionabile.HasValue))
                    ||
                    // Se è presente la retribuzione ma non il servizio utile
                    (quotaB != null && (quotaB.RetribuzionePensionabile.HasValue) &&
                    !(quotaB != null && (quotaB.ServizioUtileAA.GetValueOrDefault() > 0 || quotaB.ServizioUtileMM.GetValueOrDefault() > 0 || quotaB.ServizioUtileGG.GetValueOrDefault() > 0)))
                    )
                {
                    messaggioVideo = "Per quota B per Retribuzione Pensionabile e il ControCodice presenti, deve essere presente anche il servizio utile e viceversa.";
                    return false;
                }
                if (!isInvioAlCalcolo)
                {
                    if (quotaB != null && quotaB.ControCodiceRetributivo.HasValue && !quotaB.RetribuzionePensionabile.HasValue)
                    {
                        messaggioVideo = "Per la quota B non è possibile inserire il controcodice senza la retribuzione.";
                        return false;
                    }

                    if (quotaB != null && quotaB.RetribuzionePensionabile.HasValue && quotaB.ControCodiceRetributivo.HasValue)
                    {
                        //controllo controcodice
                        if (!CheckImportoWithControCodice(quotaB.RetribuzionePensionabile, quotaB.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                            return false;
                    }
                }
                #endregion Quota B
            }
            return true;
        }

        public static bool ControlsETDatiCalcoloAnteArmonizzazione(List<GestioneDatiServizioUtile.ServizioUtile> lstDatiServizioUtile, GestionePensione.DatiPensione datiPensione,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74, Utility.TipoFondo? tipoFondo,
            bool isInvioAlCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria,
                datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                return true;

            if (!datiPensione.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Inizio Assicurazione è obbligatoria";
                return false;
            }
            if (!datiPensione.FineAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Fine Assicurazione è obbligatoria";
                return false;
            }

            Dictionary<DateTime, DateTime> listaPeriodi = new Dictionary<DateTime, DateTime>();
            listaPeriodi.Add(datiPensione.InizioAssicurazione.Value, new DateTime(1992, 12, 31));
            listaPeriodi.Add(new DateTime(1993, 1, 1), new DateTime(1994, 12, 31));
            listaPeriodi.Add(new DateTime(1995, 1, 1), new DateTime(1996, 6, 30));
            List<string> listaQuote = new List<string> { "A", "B", "C" };

            if (!Utility.DataSuccessivaA(new DateTime(1996, 6, 1), datiPensione.InizioAssicurazione.Value))
            {
                messaggioVideo = "La data di Inizio Assicurazione non è compatibile con i Dati Servizio Utile";
                return false;
            }
            //La quota A è obbligatoria
            if (lstDatiServizioUtile == null || !lstDatiServizioUtile.Exists(x => x.Quota == "A"))
            {
                messaggioVideo = "La quota A dei Dati Servizio Utile è obbligatoria.";
                return false;
            }

            if (lstDatiServizioUtile != null && lstDatiServizioUtile.Count > 0)
            {
                #region Controlli Capienza
                int index = 0;
                foreach (KeyValuePair<DateTime, DateTime> periodo in listaPeriodi)
                {
                    GestioneDatiServizioUtile.ServizioUtile servizioUtile = lstDatiServizioUtile.Find(x => x.Quota == listaQuote[index]);

                    DateTime dataInferiore;
                    DateTime dataSuperiore;
                    string msg = string.Empty;
                    GestioneControlliDinamici.ControlloDinamico ctrl = null;

                    switch (listaQuote[index])
                    {
                        case "A":
                            msg = "ante 01/01/93";
                            break;
                        case "B":
                            msg = "post 31/12/92";
                            //Inserito bypass per Reversibilità fondo ET con quota B
                            if (Utility.IsDomandaReversibilita(datiPensione))
                                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SERVIZIO_UTILE_POST_92", out ctrl);
                            break;
                        case "C":
                            msg = "post 31/12/94";
                            break;
                    }

                    if (datiPensione.InizioAssicurazione.Value > periodo.Key)
                        dataInferiore = datiPensione.InizioAssicurazione.Value;
                    else
                        dataInferiore = periodo.Key;

                    if (datiPensione.FineAssicurazione.Value < periodo.Value)
                        dataSuperiore = datiPensione.FineAssicurazione.Value;
                    else
                        dataSuperiore = periodo.Value;

                    if (dataInferiore < dataSuperiore)
                    {
                        Utility.DifferenzaDateTime periodoMaxQuota = Utility.DifferenzaBetweenDate(dataSuperiore.AddDays(1), dataInferiore, Utility.TipoAppartenenza.FS);
                        Utility.DifferenzaDateTime periodoQuota = servizioUtile != null ?
                            new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.GetValueOrDefault(), servizioUtile.ServizioUtileMM.GetValueOrDefault(), servizioUtile.ServizioUtileGG.GetValueOrDefault()) :
                            new Utility.DifferenzaDateTime();

                        //if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                        //    (ctrl == null || (ctrl != null && ctrl.ValoreControllo == "SI")) && servizioUtile == null && periodoMaxQuota != new Utility.DifferenzaDateTime())
                        //{
                        //    messaggioVideo = string.Format("Il Servizio utile {0} è obbligatorio per inizio assicurazione {1:dd/MM/yyyy} e fine assicurazione {2:dd/MM/yyyy}.",
                        //        msg, datiPensione.InizioAssicurazione.Value, datiPensione.FineAssicurazione.Value);
                        //    return false;
                        //}

                        if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                        {
                            if (periodoQuota > periodoMaxQuota)
                            {
                                messaggioVideo = string.Format("Il massimo periodo inseribile per il Servizio utile {0} è {1} AA, {2} MM, {3} GG.", msg, periodoMaxQuota.Year, periodoMaxQuota.Month, periodoMaxQuota.Day);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!Utility.IsDomandaReversibilita(datiPensione))
                        {
                            if (servizioUtile != null &&
                                (servizioUtile.ServizioUtileAA.GetValueOrDefault() != 0 || servizioUtile.ServizioUtileMM.GetValueOrDefault() != 0 || servizioUtile.ServizioUtileGG.GetValueOrDefault() != 0 ||
                                servizioUtile.RetribuzionePensionabile.GetValueOrDefault() != 0M || servizioUtile.Retribuzione.GetValueOrDefault() != 0M ||
                                servizioUtile.ControCodiceRetributivo.GetValueOrDefault() != 0))
                            {
                                messaggioVideo = string.Format("Il Servizio utile {0} non deve essere inserito per inizio assicurazione {1:dd/MM/yyyy} e fine assicurazione {2:dd/MM/yyyy}.",
                                    msg, datiPensione.InizioAssicurazione.Value, datiPensione.FineAssicurazione.Value);
                                return false;
                            }
                        }
                    }

                    index++;
                }
                #endregion Controlli Capienza

                // Servizio Utile ante 01/01/93
                #region Quota A
                GestioneDatiServizioUtile.ServizioUtile quotaA = lstDatiServizioUtile.Find(x => x.Quota == "A");

                if (// Se è presente il servizio utile ma non la retribuzione
                    ((quotaA != null && (quotaA.ServizioUtileAA.GetValueOrDefault() > 0 || quotaA.ServizioUtileMM.GetValueOrDefault() > 0 || quotaA.ServizioUtileGG.GetValueOrDefault() > 0)) &&
                    (!quotaA.RetribuzionePensionabile.HasValue))
                    ||
                    // Se è presente la retribuzione ma non il servizio utile
                    (quotaA != null && (quotaA.RetribuzionePensionabile.HasValue) &&
                    !(quotaA != null && (quotaA.ServizioUtileAA.GetValueOrDefault() > 0 || quotaA.ServizioUtileMM.GetValueOrDefault() > 0 || quotaA.ServizioUtileGG.GetValueOrDefault() > 0)))
                    )
                {
                    messaggioVideo = "Per quota A per Retribuzione Pensionabile e il ControCodice presenti, deve essere presente anche il servizio utile e viceversa.";
                    return false;
                }
                if (!isInvioAlCalcolo)
                {
                    if (quotaA != null && quotaA.ControCodiceRetributivo.HasValue && !quotaA.RetribuzionePensionabile.HasValue)
                    {
                        messaggioVideo = "Per la quota A non è possibile inserire il controcodice senza la retribuzione.";
                        return false;
                    }

                    if (quotaA != null && quotaA.RetribuzionePensionabile.HasValue && quotaA.ControCodiceRetributivo.HasValue)
                    {
                        //controllo controcodice
                        if (!CheckImportoWithControCodice(quotaA.RetribuzionePensionabile, quotaA.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                            return false;
                    }
                }
                #endregion Quota A

                // Servizio Utile post '92 (ante '95)
                #region Quota B
                GestioneDatiServizioUtile.ServizioUtile quotaB = lstDatiServizioUtile.Find(x => x.Quota == "B");
                if (// Se è presente il servizio utile ma non la retribuzione
                    ((quotaB != null && (quotaB.ServizioUtileAA.GetValueOrDefault() > 0 || quotaB.ServizioUtileMM.GetValueOrDefault() > 0 || quotaB.ServizioUtileGG.GetValueOrDefault() > 0)) &&
                    (!quotaB.RetribuzionePensionabile.HasValue))
                    ||
                    // Se è presente la retribuzione ma non il servizio utile
                    (quotaB != null && (quotaB.RetribuzionePensionabile.HasValue) &&
                    !(quotaB != null && (quotaB.ServizioUtileAA.GetValueOrDefault() > 0 || quotaB.ServizioUtileMM.GetValueOrDefault() > 0 || quotaB.ServizioUtileGG.GetValueOrDefault() > 0)))
                    )
                {
                    messaggioVideo = "Per quota B per Retribuzione Pensionabile e il ControCodice presenti, deve essere presente anche il servizio utile e viceversa.";
                    return false;
                }

                if (!isInvioAlCalcolo)
                {
                    if (quotaB != null && quotaB.ControCodiceRetributivo.HasValue && !quotaB.RetribuzionePensionabile.HasValue)
                    {
                        messaggioVideo = "Per la quota B non è possibile inserire il controcodice senza la retribuzione.";
                        return false;
                    }

                    if (quotaB != null && quotaB.RetribuzionePensionabile.HasValue && quotaB.ControCodiceRetributivo.HasValue)
                    {
                        //controllo controcodice
                        if (!CheckImportoWithControCodice(quotaB.RetribuzionePensionabile, quotaB.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                            return false;
                    }
                }
                #endregion Quota B
            }
            return true;
        }

        public static bool ControlsTTDatiCalcoloAnteArmonizzazione(List<GestioneDatiServizioUtile.ServizioUtile> lstDatiServizioUtile, GestionePensione.DatiPensione datiPensione,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, object datiFondoXX, int? controCodiceRetrQtaA, bool isInvioAlCalcolo, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria,
                datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondoXX: datiFondoXX))
                return true;

            if (!datiPensione.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Inizio Assicurazione è obbligatoria";
                return false;
            }
            if (!datiPensione.FineAssicurazione.HasValue)
            {
                messaggioVideo = "La Data Fine Assicurazione è obbligatoria";
                return false;
            }

            List<KeyValuePair<DateTime, DateTime>> listaPeriodi = new List<KeyValuePair<DateTime, DateTime>>();
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(datiPensione.InizioAssicurazione.Value, new DateTime(1992, 12, 31))); // Quota A
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(datiPensione.InizioAssicurazione.Value, new DateTime(1992, 12, 31))); // Quota A2
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(new DateTime(1993, 1, 1), new DateTime(1994, 12, 31))); // Quota B
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(new DateTime(1993, 1, 1), new DateTime(1994, 12, 31))); // Quota B2
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(new DateTime(1995, 1, 1), new DateTime(1996, 12, 31))); // Quota C
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(new DateTime(1995, 1, 1), new DateTime(1996, 12, 31))); // Quota C2
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(new DateTime(1997, 1, 1), datiPensione.FineAssicurazione.Value)); // Quota D
            listaPeriodi.Add(new KeyValuePair<DateTime, DateTime>(new DateTime(1997, 1, 1), datiPensione.FineAssicurazione.Value)); // Quota D2
            List<string> listaQuote = new List<string> { "A", "A2", "B", "B2", "C", "C2", "D", "D2" };

            if (!Utility.DataSuccessivaA(new DateTime(1996, 6, 1), datiPensione.InizioAssicurazione.Value))
            {
                messaggioVideo = "La data di Inizio Assicurazione non è compatibile con i Dati Servizio Utile";
                return false;
            }
            //La quota A è obbligatoria
            if (lstDatiServizioUtile == null || !lstDatiServizioUtile.Exists(x => x.Quota == "A" || x.Quota == "A2"))
            {
                messaggioVideo = "La quota A dei Dati Servizio Utile è obbligatoria.";
                return false;
            }

            if (lstDatiServizioUtile != null && lstDatiServizioUtile.Count > 0)
            {
                #region Controlli Capienza
                int index = 0;
                foreach (KeyValuePair<DateTime, DateTime> periodo in listaPeriodi)
                {
                    GestioneDatiServizioUtile.ServizioUtile servizioUtile = lstDatiServizioUtile.Find(x => x.Quota == listaQuote[index]);

                    DateTime dataInferiore;
                    DateTime dataSuperiore;
                    string msg = string.Empty;

                    switch (listaQuote[index])
                    {
                        case "A":
                            msg = "ante 01/01/93";
                            break;
                        case "A2":
                            msg = " ridotto ante 01/01/93";
                            break;
                        case "B":
                            msg = "post 31/12/92";
                            break;
                        case "B2":
                            msg = "ridotto post 31/12/92";
                            break;
                        case "C":
                            msg = "post 31/12/94";
                            break;
                        case "C2":
                            msg = "ridotto post 31/12/94";
                            break;
                        case "D":
                            msg = "post 31/12/96";
                            break;
                        case "D2":
                            msg = "ridotto post 31/12/96";
                            break;
                    }

                    if (datiPensione.InizioAssicurazione.Value > periodo.Key)
                        dataInferiore = datiPensione.InizioAssicurazione.Value;
                    else
                        dataInferiore = periodo.Key;

                    if (datiPensione.FineAssicurazione.Value < periodo.Value)
                        dataSuperiore = datiPensione.FineAssicurazione.Value;
                    else
                        dataSuperiore = periodo.Value;

                    if (dataInferiore < dataSuperiore)
                    {
                        Utility.DifferenzaDateTime periodoMaxQuota = Utility.DifferenzaBetweenDate(dataSuperiore.AddDays(1), dataInferiore, Utility.TipoAppartenenza.FS);
                        Utility.DifferenzaDateTime periodoQuota = servizioUtile != null ?
                            new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.GetValueOrDefault(), servizioUtile.ServizioUtileMM.GetValueOrDefault(), servizioUtile.ServizioUtileGG.GetValueOrDefault()) :
                            new Utility.DifferenzaDateTime();

                        if (!listaQuote[index].EndsWith("2") && listaQuote[index] != "D" && servizioUtile == null && periodoMaxQuota != new Utility.DifferenzaDateTime())
                        {
                            messaggioVideo = string.Format("Il Servizio utile {0} è obbligatorio per inizio assicurazione {1:dd/MM/yyyy} e fine assicurazione {2:dd/MM/yyyy}.",
                                msg, datiPensione.InizioAssicurazione.Value, datiPensione.FineAssicurazione.Value);
                            return false;
                        }

                        if (!BypassControlloCapienzaSettimanePerBeneficiOrInabilita(datiPensione, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto, maggiorazioneInv74))
                        {
                            if (periodoQuota > periodoMaxQuota)
                            {
                                messaggioVideo = string.Format("Il massimo periodo inseribile per il Servizio utile {0} è {1} AA, {2} MM.", msg, periodoMaxQuota.Year, periodoMaxQuota.Month);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!Utility.IsDomandaReversibilita(datiPensione))
                        {
                            if (servizioUtile != null &&
                                (servizioUtile.ServizioUtileAA.GetValueOrDefault() != 0 || servizioUtile.ServizioUtileMM.GetValueOrDefault() != 0 || servizioUtile.ServizioUtileGG.GetValueOrDefault() != 0 ||
                                servizioUtile.RetribuzionePensionabile.GetValueOrDefault() != 0M || servizioUtile.Retribuzione.GetValueOrDefault() != 0M ||
                                servizioUtile.ControCodiceRetributivo.GetValueOrDefault() != 0))
                            {
                                messaggioVideo = string.Format("Il Servizio utile {0} non deve essere inserito per inizio assicurazione {1:dd/MM/yyyy} e fine assicurazione {2:dd/MM/yyyy}.",
                                    msg, datiPensione.InizioAssicurazione.Value, datiPensione.FineAssicurazione.Value);
                                return false;
                            }
                        }
                    }

                    index++;
                }
                #endregion Controlli Capienza

                // Servizio Utile post '92 (ante '95)
                #region Quota B
                GestioneDatiServizioUtile.ServizioUtile quotaB = lstDatiServizioUtile.Find(x => x.Quota == "B");
                if (// Se è presente il servizio utile ma non la retribuzione
                    ((quotaB != null && (quotaB.ServizioUtileAA.GetValueOrDefault() > 0 || quotaB.ServizioUtileMM.GetValueOrDefault() > 0 || quotaB.ServizioUtileGG.GetValueOrDefault() > 0)) &&
                    (!quotaB.RetribuzionePensionabile.HasValue))
                    ||
                    // Se è presente la retribuzione ma non il servizio utile
                    (quotaB != null && (quotaB.RetribuzionePensionabile.HasValue) &&
                    !(quotaB != null && (quotaB.ServizioUtileAA.GetValueOrDefault() > 0 || quotaB.ServizioUtileMM.GetValueOrDefault() > 0 || quotaB.ServizioUtileGG.GetValueOrDefault() > 0)))
                    )
                {
                    messaggioVideo = "Per quota B per Retribuzione Pensionabile e il ControCodice presenti, deve essere presente anche il servizio utile e viceversa.";
                    return false;
                }
                if (!isInvioAlCalcolo)
                {
                    if (quotaB != null && quotaB.ControCodiceRetributivo.HasValue && !quotaB.RetribuzionePensionabile.HasValue)
                    {
                        messaggioVideo = "Per la quota B non è possibile inserire il controcodice senza la retribuzione.";
                        return false;
                    }

                    if (quotaB != null && quotaB.RetribuzionePensionabile.HasValue && quotaB.ControCodiceRetributivo.HasValue)
                    {
                        //controllo controcodice
                        if (!CheckImportoWithControCodice(quotaB.RetribuzionePensionabile, quotaB.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                            return false;
                    }
                }
                #endregion Quota B

                // Servizio Utile post '96 
                #region Quota D
                GestioneDatiServizioUtile.ServizioUtile quotaD = lstDatiServizioUtile.Find(x => x.Quota == "D");
                if (// Se è presente il servizio utile ma non la retribuzione
                    ((quotaD != null && (quotaD.ServizioUtileAA.GetValueOrDefault() > 0 || quotaD.ServizioUtileMM.GetValueOrDefault() > 0 || quotaD.ServizioUtileGG.GetValueOrDefault() > 0)) &&
                    (!quotaD.RetribuzionePensionabile.HasValue))
                    ||
                    // Se è presente la retribuzione ma non il servizio utile
                    (quotaD != null && (quotaD.RetribuzionePensionabile.HasValue) &&
                    !(quotaD != null && (quotaD.ServizioUtileAA.GetValueOrDefault() > 0 || quotaD.ServizioUtileMM.GetValueOrDefault() > 0 || quotaD.ServizioUtileGG.GetValueOrDefault() > 0)))
                    )
                {
                    messaggioVideo = "Per quota D per Retribuzione Pensionabile presente, deve essere presente anche il servizio utile e viceversa.";
                    return false;
                }
                #endregion Quota D
            }

            #region Controlli dati Flat

            if (datiFondoXX != null && (datiFondoXX as GestioneFondo.DatiFondoTT) != null)
            {
                GestioneFondo.DatiFondoTT datiFondoTT = (GestioneFondo.DatiFondoTT)datiFondoXX;
                if (!Utility.IsDomandaReversibilita(datiPensione))
                {
                    if (!datiFondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue)
                    {
                        messaggioVideo = "Retribuzione Ultimo Anno obbligatoria.";
                        return false;
                    }

                    if (!datiFondoTT.RetribuzioneBiennio.HasValue)
                    {
                        messaggioVideo = "Retribuzione Biennio obbligatoria.";
                        return false;
                    }
                }

                if (!isInvioAlCalcolo)
                {
                    //controllo controcodice
                    if (!CheckImportoWithControCodice(datiFondoTT.RetribuzioneUltimoAnnoQuotaA.GetValueOrDefault() + datiFondoTT.RetribuzioneBiennio.GetValueOrDefault(),
                        controCodiceRetrQtaA.GetValueOrDefault(), datiPensione, out messaggioVideo))
                    {
                        messaggioVideo = messaggioVideo.Replace("Retribuzione Pensionabile", "Retribuzione Ultimo Anno + Retribuzione Biennio");
                        return false;
                    }
                }
            }

            #endregion Controlli dati Flat

            return true;
        }

        public static bool VerificaSettimane707PresentiMaNonVisibili(GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, bool isDatiComma707Null,
            bool isQuotaDPresente, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!GestioneContrib.IsSettimane707Visible(datiPensione, codiceSpecificoTraduzioneSuGP, isQuotaDPresente) && !isDatiComma707Null)
            {
                messaggioVideo = "Codice Specifico incompatibile con i dati 'Calcolo ex Comma 707'.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se bisogna effettuare il bypass sui controlli di capienza delle settimane dei dati di calcolo
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="codiceSpecificoTraduzioneSuGP"></param>
        /// <param name="tipoSettimaneBeneficio"></param>
        /// <param name="maggiorazioneAmianto"></param>
        /// <param name="maggiorazioneInv74"></param>
        /// <returns>True se deve essere effettuato il bypass</returns>
        public static bool BypassControlloCapienzaSettimanePerBeneficiOrInabilita(GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto,
            int? maggiorazioneInv74)
        {
            // Il bypass tramite Utility si applica soltanto al tipo Beneficio 05 - BENEFICI PREVISTI PER EX ART 24 COMMA 15 BIS 
            if (tipoSettimaneBeneficio == "05" && !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.BENEF_ULT_ART24COMMA15BIS))
            {
                if (!Utility.IsDomandaPensioneInabilitaOrRicostituzioneFS(datiPensione, codiceSpecificoTraduzioneSuGP) && tipoSettimaneBeneficio != "02" && tipoSettimaneBeneficio != "03" && // pensione diversa da inabilità  e beneficio diverso da AMIANTO e ART.80 L.388/2000 - INV.74%
                        !(tipoSettimaneBeneficio == "05" && (maggiorazioneAmianto.GetValueOrDefault() == 1 || maggiorazioneInv74.GetValueOrDefault() == 1)))
                    return false;
            }

            return true;
        }

        private static void CalcolaPeriodoRecordFondoWithMaggiorazione(DateTime? dataMaggiore, DateTime? dataMinore, ref int aaMax, ref int mmMax, ref int ggMax)
        {
            int app;
            int giorni1;
            int giorni2;

            giorni1 = dataMaggiore.Value.Year * 360 + dataMaggiore.Value.Month * 30 + dataMaggiore.Value.Day;
            giorni2 = dataMinore.Value.Year * 360 + dataMinore.Value.Month * 30 + dataMinore.Value.Day;
            app = giorni1 - giorni2 + 1;
            app = ((app / 360) * 30);
            aaMax += app / 360;
            app = app % 360;
            mmMax += app / 30;
            ggMax += app % 30;

            while (ggMax > 29)
            {
                mmMax += 1;
                ggMax -= 30;
            }
            while (mmMax > 11)
            {
                aaMax += 1;
                mmMax -= 12;
            }
        }

        #endregion DatiContributivi

        #region Familiari
        /// <summary>
        /// Verifica che il codice fiscale presente in Anagrafica, sia valido per ogni familiare
        /// </summary>
        /// <param name="LdatiAnagrafici">Lista Dati Anagrafici</param>
        /// <param name="Lfamiliari">Lista Familiari</param>
        /// <returns>true se il codice fiscale è valido, false altrimenti</returns>
        public static bool VerificaAnagraficaFamiliari(List<GestioneAnagrafica.DatiAnagrafici> LdatiAnagrafici, List<GestioneFamiliari.Familiare> Lfamiliari)
        {
            if (Lfamiliari == null || Lfamiliari.Count == 0)
                return true;

            GestioneAnagrafica.DatiAnagrafici datiAnag = new GestioneAnagrafica.DatiAnagrafici();
            foreach (GestioneFamiliari.Familiare fam in Lfamiliari)
            {
                if (fam.Confermato)
                {
                    datiAnag = LdatiAnagrafici.Find(x => x.Id == fam.IdAnagrafica);
                    if (datiAnag.CodiceFiscale == string.Empty || datiAnag.CodiceFiscale.Trim().Length != 16)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se la tutte le decorrenze del codice maggiorazione siano maggiori o uguale alla decorrenza della pensione
        /// </summary>
        /// <param name="LdatiAnagrafici">Lista dei dati anagrafici</param>
        /// <param name="listaCodMaggFamiliari">Lista dei codici maggiorazione dei familiari</param>
        /// <param name="DecorrenzaPensione">Data decorrenza pensione</param>
        /// <param name="Nominativo">Nominativo</param>
        /// <returns>false se la decorrenza pensione è maggiore della prima decorrenza del codice maggiorazione benefici, true altrimenti</returns>
        public static bool VerificaDecorrenzaListCodMaggDecorrenzaPensione(List<GestioneFamiliari.Familiare> Lfamiliari, List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari, DateTime? DecorrenzaPensione, out long idAnagrafica)
        {
            idAnagrafica = 0;

            if (Lfamiliari == null || Lfamiliari.Count == 0)
                return true;

            if (listaCodMaggFamiliari == null || listaCodMaggFamiliari.Count == 0)
                return false;

            foreach (GestioneFamiliari.Familiare datifam in Lfamiliari)
            {
                if (datifam.Confermato)
                {
                    if (GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggDecorrenzaPensione(datifam.IdAnagrafica, listaCodMaggFamiliari, DecorrenzaPensione))
                    {
                        idAnagrafica = datifam.IdAnagrafica;
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Se Anagrafica.CodiceStatoCivile = 2 del titolare, allora  deve esistere nella tabella Familiare il coniuge (SiglaFamiliare = C) 
        /// </summary>
        /// <param name="LdatiAnagrafici">Lista dei dati anagrafici</param>
        /// <param name="Lfamiliare">Lista dei familiari</param>
        /// <returns>true se è presente almeno un coniuge nella tabella familiari con CodiceStatoCivile = 2, false altrimenti</returns>
        //public static bool VerificaConiugeInFamiliari(AreaTitolare areaTitolare, List<GestioneFamiliari.Familiare> Lfamiliare)
        //{     
        //    if (Lfamiliare == null || Lfamiliare.Count == 0)
        //        return true;

        //    int numConiugi = Lfamiliare.Count(x => x.SiglaFamiliare.HasValue && x.SiglaFamiliare.Value.ToString().ToUpper() == "C");

        //    return (((numConiugi == 1) && (areaTitolare.ElencoStatiCivili.OrderBy(x => x.Decorrenza).Last().Codice == 2)) 
        //        || ((numConiugi == 0) && (areaTitolare.ElencoStatiCivili.OrderBy(x => x.Decorrenza).Last().Codice != 2)));
        //}
        #endregion Familiari

        #region LiquidazionePensione
        #region Dati Generici
        internal static void GetCodiciNatura(string naturaPensione, out char codNat1, out char codNat2, out char codNat3)
        {
            codNat1 = ' ';
            codNat2 = ' ';
            codNat3 = ' ';
            if (naturaPensione != null)
            {
                naturaPensione = naturaPensione.PadRight(3, ' ');
                codNat1 = char.Parse(naturaPensione.Substring(0, 1).ToUpperInvariant());
                codNat2 = char.Parse(naturaPensione.Substring(1, 1).ToUpperInvariant());
                codNat3 = char.Parse(naturaPensione.Substring(2, 1).ToUpperInvariant());
            }
        }

        /// <summary>
        /// Verifica che la domanda di invalidità non abbia bonus (Se Pensione.SiglaCategoria = IEL o Pensione.SiglaCategoria = ITT => PensioneFondoDatiGenerici.AttribuzioneBonus = null)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="datiFondo"></param>
        public static bool VerificaPensioneInvaliditaWithoutBonus(GestionePensione.DatiPensione datiPensione, bool? AttribuzioneBonus)
        {
            if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("I"))
            {
                //if (datiPensione.SiglaCategoria.Trim().ToUpper() == "IEL" || datiPensione.SiglaCategoria.Trim().ToUpper() == "ITT")
                //{
                if (AttribuzioneBonus.HasValue && AttribuzioneBonus.Value)
                    return false;
                //}
            }
            return true;
        }

        /// <summary>
        /// Verifica una serie di valori relativi ai dati fondo (EL, TT, ecc.) 
        /// </summary>
        /// <param name="tipoFondo"></param>
        /// <param name="DatiFondoXX" object che contiene i dati del fondo in questione (EL, TT, ecc.)></param>
        /// <param name="datiPensione"></param>
        /// <param name="areaTitolare"></param>
        /// <param name="IsInvioCalcolo" true se tale metodo viene richiamato dalla procedura di invio al calcolo></param>
        /// <param name="msgVideo"></param>
        /// <returns></returns>
        public static bool VerificaRequisitiNoInvalidita(Liquidazione.BLCommon.Utility.TipoFondo? tipoFondo, Object DatiFondoXX, GestionePensione.DatiPensione datiPensione,
            char? codiceRequisito, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, char? derogaTraduzioneSuGP, DateTime? dataNascitaTitolare, char? sessoTitolare,
            bool IsInvioCalcolo, out string msgVideo)
        {
            msgVideo = string.Empty;
            bool? Requisiti247_243 = null;
            byte? NumeroTriSemRequisiti = null;
            short? AnnoRequisiti = null;
            int? AnzianitaAnni = null;

            GestioneCrossControls.TipoDecPensione? tipoDec = GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo,
                datiPensione.Prodotto, datiPensione.Tipo);

            //mail 24-02-2014: bypass controlli per domande di ricostituzione diverse da Variazione Per Decorrenza
            if ((datiPensione.Gruppo == "0031" && !Utility.IsRicostituzione_VariazionePerDecorrenza(datiPensione)) || Utility.IsRiaperturaDomanda(datiPensione.Id))
                return true;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (!datiPensione.SiglaCategoria.StartsWith("I") && !datiPensione.SiglaCategoria.StartsWith("S"))   // no invalidità e no superstiti
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.EL:
                            Requisiti247_243 = ((GestioneFondo.DatiFondoEL)DatiFondoXX).Requisiti247_243;
                            NumeroTriSemRequisiti = ((GestioneFondo.DatiFondoEL)DatiFondoXX).NumeroTriSemRequisiti;
                            AnnoRequisiti = ((GestioneFondo.DatiFondoEL)DatiFondoXX).AnnoRequisiti;
                            AnzianitaAnni = ((GestioneFondo.DatiFondoEL)DatiFondoXX).AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.TT:
                            Requisiti247_243 = ((GestioneFondo.DatiFondoTT)DatiFondoXX).Requisiti247_243;
                            NumeroTriSemRequisiti = ((GestioneFondo.DatiFondoTT)DatiFondoXX).NumeroTriSemRequisiti;
                            AnnoRequisiti = ((GestioneFondo.DatiFondoTT)DatiFondoXX).AnnoRequisiti;
                            AnzianitaAnni = ((GestioneFondo.DatiFondoTT)DatiFondoXX).AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.ET:
                            Requisiti247_243 = ((GestioneFondo.DatiFondoET)DatiFondoXX).Requisiti247_243;
                            NumeroTriSemRequisiti = ((GestioneFondo.DatiFondoET)DatiFondoXX).NumeroTriSemRequisiti;
                            AnnoRequisiti = ((GestioneFondo.DatiFondoET)DatiFondoXX).AnnoRequisiti;
                            AnzianitaAnni = ((GestioneFondo.DatiFondoET)DatiFondoXX).AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.VL:
                            Requisiti247_243 = ((GestioneFondo.DatiFondoVL)DatiFondoXX).Requisiti247_243;
                            NumeroTriSemRequisiti = ((GestioneFondo.DatiFondoVL)DatiFondoXX).NumeroTriSemRequisiti;
                            AnnoRequisiti = ((GestioneFondo.DatiFondoVL)DatiFondoXX).AnnoRequisiti;
                            AnzianitaAnni = ((GestioneFondo.DatiFondoVL)DatiFondoXX).AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.PT:
                            Requisiti247_243 = ((List<GestioneFondo.DatiFondoPT>)DatiFondoXX).First().RequisitiAnte247;
                            NumeroTriSemRequisiti = ((List<GestioneFondo.DatiFondoPT>)DatiFondoXX).First().TrimesteRequisiti;
                            AnnoRequisiti = ((List<GestioneFondo.DatiFondoPT>)DatiFondoXX).First().AnnoRequisiti;
                            AnzianitaAnni = ((List<GestioneFondo.DatiFondoPT>)DatiFondoXX).First().AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.FS:
                            Requisiti247_243 = ((List<GestioneFondo.DatiFondoFST>)DatiFondoXX).First().RequisitiAnte247;
                            NumeroTriSemRequisiti = ((List<GestioneFondo.DatiFondoFST>)DatiFondoXX).First().TrimesteRequisiti;
                            AnnoRequisiti = ((List<GestioneFondo.DatiFondoFST>)DatiFondoXX).First().AnnoRequisiti;
                            AnzianitaAnni = ((List<GestioneFondo.DatiFondoFST>)DatiFondoXX).First().AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.PI:
                        case Utility.TipoFondo.PL:
                            if (DatiFondoXX is List<GestioneFondo.DatiFondoPI>)
                            {
                                List<GestioneFondo.DatiFondoPI> lista =
                                    (List<GestioneFondo.DatiFondoPI>)DatiFondoXX;

                                if (lista != null && lista.Count > 0)
                                {
                                    Requisiti247_243 = ((List<GestioneFondo.DatiFondoPI>)DatiFondoXX).First().Requisiti247_243;
                                    NumeroTriSemRequisiti = ((List<GestioneFondo.DatiFondoPI>)DatiFondoXX).First().NumeroTriSemRequisiti;
                                    AnnoRequisiti = ((List<GestioneFondo.DatiFondoPI>)DatiFondoXX).First().AnnoRequisiti;
                                    AnzianitaAnni = ((List<GestioneFondo.DatiFondoPI>)DatiFondoXX).First().AnzianitaAnni;
                                }
                            }
                            else if (DatiFondoXX is GestioneFondo.DatiFondoPI)
                            {
                                GestioneFondo.DatiFondoPI dati =
                                    (GestioneFondo.DatiFondoPI)DatiFondoXX;

                                Requisiti247_243 = dati.Requisiti247_243;
                                NumeroTriSemRequisiti = dati.NumeroTriSemRequisiti;
                                AnnoRequisiti = dati.AnnoRequisiti;
                                AnzianitaAnni = dati.AnzianitaAnni;
                            }                      
                            break;
                        case Utility.TipoFondo.GAS:
                            Requisiti247_243 = ((GestioneFondo.DatiFondoGAS)DatiFondoXX).Requisiti247_243;
                            NumeroTriSemRequisiti = ((GestioneFondo.DatiFondoGAS)DatiFondoXX).NumeroTriSemRequisiti;
                            AnnoRequisiti = ((GestioneFondo.DatiFondoGAS)DatiFondoXX).AnnoRequisiti;
                            AnzianitaAnni = ((GestioneFondo.DatiFondoGAS)DatiFondoXX).AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.ES:
                            Requisiti247_243 = ((GestioneFondo.DatiFondoES)DatiFondoXX).Requisiti247_243;
                            NumeroTriSemRequisiti = ((GestioneFondo.DatiFondoES)DatiFondoXX).NumeroTriSemRequisiti;
                            AnnoRequisiti = ((GestioneFondo.DatiFondoES)DatiFondoXX).AnnoRequisiti;
                            AnzianitaAnni = ((GestioneFondo.DatiFondoES)DatiFondoXX).AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.DZ:
                            Requisiti247_243 = ((List<GestioneFondo.DatiFondoDZ>)DatiFondoXX).FirstOrDefault().Requisiti247_243;
                            NumeroTriSemRequisiti = ((List<GestioneFondo.DatiFondoDZ>)DatiFondoXX).FirstOrDefault().NumeroTriSemRequisiti;
                            AnnoRequisiti = ((List<GestioneFondo.DatiFondoDZ>)DatiFondoXX).FirstOrDefault().AnnoRequisiti;
                            AnzianitaAnni = ((List<GestioneFondo.DatiFondoDZ>)DatiFondoXX).FirstOrDefault().AnzianitaAnni;
                            break;
                        case Utility.TipoFondo.PM:
                            NumeroTriSemRequisiti = ((GestioneFondo.DatiFondoPM)DatiFondoXX).NumeroTriSemRequisiti;
                            AnnoRequisiti = ((GestioneFondo.DatiFondoPM)DatiFondoXX).AnnoRequisiti;
                            AnzianitaAnni = ((GestioneFondo.DatiFondoPM)DatiFondoXX).AnzianitaAnni;
                            break;
                    }
                }
                else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaApp = DatiFondoXX as List<GestionePensioneINPDAP.DatiPensioneINPDAP>;
                    GestionePensioneINPDAP.DatiPensioneINPDAP app = null;
                    if (listaApp != null && listaApp.Count > 0)
                    {
                        app = listaApp.FirstOrDefault();
                        Requisiti247_243 = app.RequisitiAnte247;
                        NumeroTriSemRequisiti = app.TrimesteRequisiti;
                        AnnoRequisiti = app.AnnoRequisiti;
                        AnzianitaAnni = app.AnzianitaAnni;
                    }
                }


                //ENG - Memo 166/2023: vanno effettuati i controlli previsti per il contributivo e per la vecchiaia
                if (Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione))
                {
                    if (!VerificaAnnoTrimestreRequisiti_TipoContributivo(datiPensione, Requisiti247_243, NumeroTriSemRequisiti, AnnoRequisiti, AnzianitaAnni, datiPensione.DataPerfezionamentoRequisiti, out msgVideo))
                        return false;

                    if ((tipoDec == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDec == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia) &&
                          (datiPensione.DataPerfezionamentoRequisiti.HasValue && datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2011, 01, 01)) < 0))
                    {
                        if (!VerificaAnnoTrimestreRequisiti(tipoFondo, NumeroTriSemRequisiti, AnnoRequisiti, AnzianitaAnni, datiPensione, dataNascitaTitolare.Value,
                            sessoTitolare, codiceRequisito, tipoDec, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, IsInvioCalcolo, out msgVideo))
                            return false;
                    }
                }
                else
                {
                    if (Utility.IsDomandaSalvaguardia122_FS_2011_2012(datiPensione, derogaTraduzioneSuGP))
                    {
                        if (!VerificaAnnoTrimestreRequisitiSalvaguardiaL122(datiPensione.DecorrenzaOriginaria, dataNascitaTitolare, NumeroTriSemRequisiti, AnnoRequisiti, AnzianitaAnni, out msgVideo))
                            return false;
                    }
                    else if (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                            (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                            ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
                    {
                        if (!VerificaAnnoTrimestreRequisiti_TipoContributivo(datiPensione, Requisiti247_243, NumeroTriSemRequisiti, AnnoRequisiti, AnzianitaAnni, datiPensione.DataPerfezionamentoRequisiti, out msgVideo))
                            return false;
                    }
                    else if (datiPensione.Prodotto.Trim() == "0001") //Anzianità
                    {
                        if (Utility.IsDomandaSperimentaleDonna(datiPensione)) // sperimentale donna
                        {
                            if (!datiPensione.DataPerfezionamentoRequisiti.HasValue || datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2011, 01, 01)) < 0)
                            {
                                if (!VerificaAnnoSemestreRequisiti(tipoFondo, Requisiti247_243, NumeroTriSemRequisiti, AnnoRequisiti, AnzianitaAnni, datiPensione, dataNascitaTitolare, tipoDec,
                                    out msgVideo))
                                    return false;
                            }
                        }
                        else  //Anzianità pura
                        {
                            if (!datiPensione.DataPerfezionamentoRequisiti.HasValue || datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2011, 01, 01)) < 0)
                            {
                                if (!Requisiti247_243.HasValue)
                                {
                                    msgVideo = "'Requisiti Ante 247' dato obbligatorio";
                                    return false;
                                }

                                if (!Requisiti247_243.Value)
                                    if (!VerificaAnnoTrimestreRequisiti(tipoFondo, NumeroTriSemRequisiti, AnnoRequisiti, AnzianitaAnni, datiPensione, dataNascitaTitolare.Value,
                                        sessoTitolare.Value, codiceRequisito, tipoDec, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, IsInvioCalcolo, out msgVideo))
                                        return false;
                            }
                        }
                    }
                    //Vecchiaia  
                    else if ((tipoDec == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipoDec == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia) &&
                              (datiPensione.DataPerfezionamentoRequisiti.HasValue && datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2011, 01, 01)) < 0))
                    {
                        if (!VerificaAnnoTrimestreRequisiti(tipoFondo, NumeroTriSemRequisiti, AnnoRequisiti, AnzianitaAnni, datiPensione, dataNascitaTitolare.Value,
                            sessoTitolare, codiceRequisito, tipoDec, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, IsInvioCalcolo, out msgVideo))
                            return false;
                    }
                }
            }
            return true;
        }

        private static bool VerificaRequisitiAnte247(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.Utility.TipoFondo? tipoFondo, byte? NumeroTriSemRequisiti, short? AnnoRequisiti,
            DateTime? dataNascitaTitolare, char? sessoTitolare, char? codiceRequisito, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, out string msgVideo)
        {
            bool isVecchiaia = false;

            if (!dataNascitaTitolare.HasValue)
            {
                msgVideo = "Data Nascita del titolare assente.";
                return false;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Apportata modifica in seguito alla mail del 11-09-2013 RE: Requisito 247 Pensione Vecchiaia
            int anniTitolare = 0;

            if (!string.IsNullOrEmpty(datiPensione.Prodotto.Trim()) && datiPensione.Prodotto.Trim() == "0001") //anzianità
            {
                anniTitolare = 57;

                if (tipoFondo.HasValue && tipoFondo.Value == Liquidazione.BLCommon.Utility.TipoFondo.VL)
                    anniTitolare = 52;
            }
            else //vecchiaia
            {
                isVecchiaia = true;
                anniTitolare = CalcolaEtaTitolareLimite(Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensione, codiceSpecificoTraduzioneSuGP), tipoFondo, codiceRequisito, sessoTitolare, tipoSettimaneBeneficio);
            }

            if (anniTitolare == 0)
            {
                msgVideo = "Non è stato possibile determinare l'età del Titolare";
                return false;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            dataNascitaTitolare = dataNascitaTitolare.Value.AddYears(anniTitolare);

            msgVideo = string.Empty;
            DateTime dataFine = isVecchiaia ? new DateTime(AnnoRequisiti.Value, 03, 31) : new DateTime(AnnoRequisiti.Value, 06, 30);
            DateTime annoMeseTrimestreRequisiti = new DateTime(AnnoRequisiti.Value, 07, 01);

            if (NumeroTriSemRequisiti == 1)
            {
                if (dataNascitaTitolare < dataFine)
                {
                    if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                    {
                        msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                        return false;
                    }
                }
                else
                {
                    msgVideo = "Il titolare non ha compiuto " + anniTitolare + " anni al " + String.Format("{0:dd/MM}", dataFine) + "/" + AnnoRequisiti.Value;
                    return false;
                }
            }

            dataFine = isVecchiaia ? new DateTime(AnnoRequisiti.Value, 06, 30) : new DateTime(AnnoRequisiti.Value, 09, 30);
            annoMeseTrimestreRequisiti = new DateTime(AnnoRequisiti.Value, 10, 01);
            if (NumeroTriSemRequisiti == 2)
            {
                if (dataNascitaTitolare < dataFine)
                {
                    if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                    {
                        msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                        return false;
                    }
                }
                else
                {
                    msgVideo = "Il titolare non ha compiuto " + anniTitolare + " anni al " + String.Format("{0:dd/MM}", dataFine) + "/" + AnnoRequisiti.Value;
                    return false;
                }
            }

            if (isVecchiaia)
                dataFine = new DateTime(AnnoRequisiti.Value, 09, 30);
            annoMeseTrimestreRequisiti = new DateTime(AnnoRequisiti.Value + 1, 01, 01);
            if (NumeroTriSemRequisiti == 3)
            {
                if (isVecchiaia)
                {
                    if (dataNascitaTitolare >= dataFine)
                    {
                        msgVideo = "Il titolare non ha compiuto " + anniTitolare + " anni al 30/09/" + AnnoRequisiti.Value;
                        return false;
                    }
                }

                if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                {
                    msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                    return false;
                }
            }

            if (isVecchiaia)
                dataFine = new DateTime(AnnoRequisiti.Value, 12, 31);
            annoMeseTrimestreRequisiti = new DateTime(AnnoRequisiti.Value + 1, 04, 01);
            if (NumeroTriSemRequisiti == 4)
            {
                if (isVecchiaia)
                {
                    if (dataNascitaTitolare >= dataFine)
                    {
                        msgVideo = "Il titolare non ha compiuto " + anniTitolare + " anni al 31/12/" + AnnoRequisiti.Value;
                        return false;
                    }
                }

                if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                {
                    msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Requisiti247_243"></param>
        /// <param name="NumeroSemestreRequisiti"></param>
        /// <param name="AnnoSemestreRequisiti"></param>
        /// <param name="AnzianitaAnni"></param>
        /// <param name="datiPensione"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="tipo"></param>
        /// <param name="IsInvioCalcolo"></param>
        /// <param name="msgVideo"></param>
        /// <returns></returns>
        private static bool VerificaAnnoSemestreRequisiti(Liquidazione.BLCommon.Utility.TipoFondo? tipoFondo, bool? Requisiti247_243, byte? NumeroSemestreRequisiti, short? AnnoSemestreRequisiti, int? AnzianitaAnni, GestionePensione.DatiPensione datiPensione, DateTime? dataNascitaTitolare, GestioneCrossControls.TipoDecPensione? tipoDecPensione, out string msgVideo)
        {
            msgVideo = string.Empty;
            int anzianitaMin = 35;
            int anzianitaMax = 40;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.ES:

                        //controlli validi per tutte e due le decorrenze per sperimentale donna
                        if (!datiPensione.DecorrenzaOriginaria.HasValue)
                        {
                            msgVideo = "'Decorrenza Pensione' dato obbligatorio";
                            return false;
                        }

                        if (!tipoDecPensione.HasValue)
                        {
                            msgVideo = "dati obbligatori mancanti (gruppo, prodotto, tipo, dec pensione)";
                            return false;
                        }

                        if (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810AnzSpDonna || tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso1115AnzSpDonna)
                        {
                            if (!Requisiti247_243.HasValue)
                            {
                                msgVideo = "'Legge 243' dato obbligatorio";
                                return false;
                            }

                            if (!AnzianitaAnni.HasValue)    //Controllo valido per tutte e due le decorrenze di Sperimentale donna
                            {
                                msgVideo = "'Anzianita Anni' dato obbligatorio";
                                return false;
                            }

                            if (AnzianitaAnni.Value < anzianitaMin || AnzianitaAnni.Value > anzianitaMax)
                            {
                                msgVideo = "'Anzianita Anni' deve essere compreso tra " + anzianitaMin + " e " + anzianitaMax;
                                return false;
                            }
                        }

                        if (tipoDecPensione.Value == GestioneCrossControls.TipoDecPensione.Compreso0810AnzSpDonna)
                        {
                            if (!NumeroSemestreRequisiti.HasValue)
                            {
                                msgVideo = "'Numero Semestre Requisiti' dato obbligatorio";
                                return false;
                            }
                            if (!AnnoSemestreRequisiti.HasValue)
                            {
                                msgVideo = "'Anno Semestre Requisiti' dato obbligatorio";
                                return false;
                            }

                            //controllo età titolare per sperimentale donna
                            if (!VerificaEtaTitolareCompreso20082011SperDonna(NumeroSemestreRequisiti, AnnoSemestreRequisiti, dataNascitaTitolare, out msgVideo))
                                return false;

                            //controllo perfezionamento requisiti per sperimentale donna
                            if (!VerificaPerfRequisitiDecPensioneSperDonna(NumeroSemestreRequisiti, AnnoSemestreRequisiti, datiPensione, out msgVideo))
                                return false;
                        }
                        break;
                }
            }
            return true;
        }

        private static bool VerificaAnnoTrimestreRequisiti(Liquidazione.BLCommon.Utility.TipoFondo? tipoFondo, byte? NumeroTrimestreRequisiti, short? AnnoTrimestreRequisiti, int? AnzianitaAnni,
            GestionePensione.DatiPensione datiPensione, DateTime? dataNascitaTitolare, char? sessoTitolare, char? codiceRequisito, GestioneCrossControls.TipoDecPensione? tipo, string tipoSettimaneBeneficio,
            char? codiceSpecificoTraduzioneSuGP, bool IsInvioCalcolo, out string msgVideo)
        {
            msgVideo = string.Empty;
            int anzianitaMin = 35;
            int anzianitaMax = 40;

            if (tipoFondo.HasValue && tipoFondo.Value == Liquidazione.BLCommon.Utility.TipoFondo.VL)
            {
                anzianitaMin = 30;
                anzianitaMax = 35;
            }

            if (!NumeroTrimestreRequisiti.HasValue)
            {
                msgVideo = "'Numero Trimestre Requisiti' dato obbligatorio";
                if (IsInvioCalcolo)
                    msgVideo = "Se il campo requisiti ante 247 = No, " + msgVideo;
                return false;
            }
            if (!AnnoTrimestreRequisiti.HasValue)
            {
                msgVideo = "'Anno Trimestre Requisiti' dato obbligatorio";
                if (IsInvioCalcolo)
                    msgVideo = "Se il campo requisiti ante 247 = No, " + msgVideo;
                return false;
            }
            if (!AnzianitaAnni.HasValue)
            {
                msgVideo = "'Anzianita Anni' dato obbligatorio";
                if (IsInvioCalcolo)
                    msgVideo = "Se il campo requisiti ante 247 = No, " + msgVideo;
                return false;
            }

            DateTime annoMeseTrimestreRequisiti;
            if (datiPensione.Prodotto.Trim() == "0001") // anzianità
            {
                if (AnzianitaAnni.Value < anzianitaMin || AnzianitaAnni.Value > anzianitaMax)
                {
                    msgVideo = "'Anzianita Anni' deve essere compreso tra " + anzianitaMin + " e " + anzianitaMax;
                    if (IsInvioCalcolo)
                        msgVideo = "Se il campo requisiti ante 247 = No, " + msgVideo;
                    return false;
                }

                if (AnzianitaAnni.Value == anzianitaMax)
                {
                    if (!VerificaRequisitiAnte247(datiPensione, tipoFondo, NumeroTrimestreRequisiti, AnnoTrimestreRequisiti, dataNascitaTitolare, sessoTitolare, codiceRequisito, tipoSettimaneBeneficio,
                        codiceSpecificoTraduzioneSuGP, out msgVideo))
                        return false;
                }
                else
                {
                    if (AnzianitaAnni.Value >= anzianitaMin && AnzianitaAnni.Value < anzianitaMax)
                    {
                        if (NumeroTrimestreRequisiti == 1)
                        {
                            annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 07, 01);
                            if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                            {
                                msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                                return false;
                            }
                        }
                        if (NumeroTrimestreRequisiti == 2)
                        {
                            annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 10, 01);
                            if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                            {
                                msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                                return false;
                            }
                        }
                        if (NumeroTrimestreRequisiti == 3)
                        {
                            annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value + 1, 01, 01);
                            if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                            {
                                msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                                return false;
                            }
                        }
                        if (NumeroTrimestreRequisiti == 4)
                        {
                            annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value + 1, 04, 01);
                            if (datiPensione.DecorrenzaOriginaria < annoMeseTrimestreRequisiti)
                            {
                                msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                                return false;
                            }
                        }
                    }
                }
            }
            else // vecchiaia
            {
                if (AnzianitaAnni.Value > anzianitaMax)
                {
                    msgVideo = "'Anzianita Anni' deve essere minore o uguale a " + anzianitaMax;
                    if (IsInvioCalcolo)
                        msgVideo = "Se il campo requisiti ante 247 = No, " + msgVideo;
                    return false;
                }
                if (tipo.HasValue)
                {
                    if (tipo == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia || tipo == GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia) // vecchiaia compresa tra 2008 e 2011
                    {
                        if (!VerificaRequisitiAnte247(datiPensione, tipoFondo, NumeroTrimestreRequisiti, AnnoTrimestreRequisiti, dataNascitaTitolare, sessoTitolare, codiceRequisito, tipoSettimaneBeneficio,
                            codiceSpecificoTraduzioneSuGP, out msgVideo))
                            return false;
                    }
                }
                else
                {
                    msgVideo = "Dati obbligatori mancanti.";
                    return false;
                }
            }
            return true;
        }

        private static bool VerificaAnnoTrimestreRequisiti_TipoContributivo(GestionePensione.DatiPensione datiPensione, bool? Requisiti247_243, byte? NumeroTrimestreRequisiti, short? AnnoTrimestreRequisiti, int? AnzianitaAnni, DateTime? DataPerfezionamentoRequisiti,
            out string msgVideo)
        {
            msgVideo = string.Empty;

            if (Requisiti247_243.GetValueOrDefault())
            {
                if (NumeroTrimestreRequisiti.HasValue)
                {
                    msgVideo = "'Numero Trimestre Requisiti' non acquisibile";
                    return false;
                }
                if (AnnoTrimestreRequisiti.HasValue)
                {
                    msgVideo = "'Anno Trimestre Requisiti' non acquisibile";
                    return false;
                }
                if (AnzianitaAnni.HasValue)
                {
                    msgVideo = "'Anzianita Anni' non acquisibile";
                    return false;
                }
            }
            else if (Requisiti247_243.HasValue)
            {
                if (!NumeroTrimestreRequisiti.HasValue)
                {
                    msgVideo = "'Numero Trimestre Requisiti' dato obbligatorio";
                    return false;
                }
                if (!AnnoTrimestreRequisiti.HasValue)
                {
                    msgVideo = "'Anno Trimestre Requisiti' dato obbligatorio";
                    return false;
                }
                else if ((Utility.IsDomandaTipoContributivo(datiPensione, false, null) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione)) && (AnnoTrimestreRequisiti.Value < 2008 || AnnoTrimestreRequisiti.Value > 2010))
                {
                    msgVideo = "Attenzione i dati 247 non sono coerenti.";
                    return false;
                }
                if (!AnzianitaAnni.HasValue)
                {
                    msgVideo = "'Anzianita Anni' dato obbligatorio";
                    return false;
                }
                else
                {
                    if (!Requisiti247_243.Value && AnzianitaAnni < 5)
                    {
                        msgVideo = "'Anzianita Anni' deve essere almeno 5";
                        return false;
                    }
                }

                if (DataPerfezionamentoRequisiti.HasValue)
                {
                    var trimestre = (DataPerfezionamentoRequisiti.Value.Month - 1) / 3 + 1;
                    var anno = DataPerfezionamentoRequisiti.Value.Year;
                    if (NumeroTrimestreRequisiti != trimestre || anno != AnnoTrimestreRequisiti)
                    {
                        msgVideo = "Trimestre requisiti non coerente con Data Perfezionamento Requisiti";
                        return false;
                    }
                }
            }
            return true;
        }
        private static bool VerificaAnnoTrimestreRequisitiSalvaguardiaL122(DateTime? decorrenzaOriginaria, DateTime? dataNascitaTitolare, byte? NumeroTrimestreRequisiti, short? AnnoTrimestreRequisiti, int? AnzianitaAnni,
            out string msgVideo)
        {
            msgVideo = string.Empty;
            int anzianitaMin = 35;
            int anzianitaMax = 40;

            if (!NumeroTrimestreRequisiti.HasValue)
            {
                msgVideo = "'Numero Trimestre Requisiti' dato obbligatorio";
                return false;
            }
            if (!AnnoTrimestreRequisiti.HasValue)
            {
                msgVideo = "'Anno Trimestre Requisiti' dato obbligatorio";
                return false;
            }
            if (!AnzianitaAnni.HasValue)
            {
                msgVideo = "'Anzianita Anni' dato obbligatorio";
                return false;
            }

            if (AnnoTrimestreRequisiti.GetValueOrDefault() != 2011 && AnnoTrimestreRequisiti.GetValueOrDefault() != 2012)
            {
                msgVideo = "'Anno Trimestre Requisiti' deve essere 2011 o 2012.";
                return false;
            }

            if (AnzianitaAnni.Value < anzianitaMin || AnzianitaAnni.Value > anzianitaMax)
            {
                msgVideo = "'Anzianita Anni' deve essere compreso tra " + anzianitaMin + " e " + anzianitaMax;
                return false;
            }

            if (AnzianitaAnni.Value == anzianitaMax)
            {
                DateTime dataFine = new DateTime(AnnoTrimestreRequisiti.Value, 06, 30);
                DateTime annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 7, 01);
                if (NumeroTrimestreRequisiti == 1)
                {
                    if (!Utility.DataSuccessivaA(dataNascitaTitolare.Value.AddYears(57), dataFine))
                    {
                        if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, annoMeseTrimestreRequisiti))
                        {
                            msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                            return false;
                        }
                    }
                    else
                    {
                        msgVideo = "Il titolare non ha compiuto 57 anni al " + String.Format("{0:dd/MM}", dataFine) + "/" + AnnoTrimestreRequisiti.Value;
                        return false;
                    }
                }

                dataFine = new DateTime(AnnoTrimestreRequisiti.Value, 09, 30);
                annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 10, 01);
                if (NumeroTrimestreRequisiti == 2)
                {
                    if (!Utility.DataSuccessivaA(dataNascitaTitolare.Value.AddYears(57), dataFine))
                    {
                        if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, annoMeseTrimestreRequisiti))
                        {
                            msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                            return false;
                        }
                    }
                    else
                    {
                        msgVideo = "Il titolare non ha compiuto 57 anni al " + String.Format("{0:dd/MM}", dataFine) + "/" + AnnoTrimestreRequisiti.Value;
                        return false;
                    }
                }

                annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value + 1, 1, 1);
                if (NumeroTrimestreRequisiti == 3)
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, annoMeseTrimestreRequisiti))
                    {
                        msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                        return false;
                    }
                }

                annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value + 1, 04, 01);
                if (NumeroTrimestreRequisiti == 4)
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, annoMeseTrimestreRequisiti))
                    {
                        msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                        return false;
                    }
                }
            }
            else if (AnzianitaAnni.Value >= anzianitaMin && AnzianitaAnni.Value < anzianitaMax)
            {
                if (NumeroTrimestreRequisiti == 1 || NumeroTrimestreRequisiti == 2)
                {
                    DateTime annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value + 1, 01, 01);
                    if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, annoMeseTrimestreRequisiti))
                    {
                        msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                        return false;
                    }
                }
                if (NumeroTrimestreRequisiti == 3)
                {
                    DateTime annoMeseTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value + 1, 07, 01);
                    if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, annoMeseTrimestreRequisiti))
                    {
                        msgVideo = "Decorrenza pensione non compatibile con il trimestre di raggiungimento dei requisiti";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Verifca l'eta della titolare per una pensione di anzianità Sperimentale Donna
        /// </summary>
        /// <param name="NumeroSemestreRequisiti"></param>
        /// <param name="AnnoSemestreRequisiti"></param>
        /// <param name="AnzianitaAnni"></param>
        /// <param name="datiPensione"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="tipo"></param>
        /// <param name="IsInvioCalcolo"></param>
        /// <param name="msgVideo"></param>
        /// <returns></returns>
        private static bool VerificaEtaTitolareCompreso20082011SperDonna(byte? NumeroSemestreRequisiti, short? AnnoSemestreRequisiti, DateTime? dataNascitaTitolare, out string msgVideo)
        {
            msgVideo = string.Empty;

            DateTime? dataSemestreRequisiti = null;

            if (!NumeroSemestreRequisiti.HasValue || !AnnoSemestreRequisiti.HasValue || !dataNascitaTitolare.HasValue)
            {
                msgVideo = "Anno, Semestre requisiti e data nascita titolare obbligatori per una pensione di anzianità sperimentale donna la cui decorrenza è compresa tra il 2008 e il 2010";
                return false;
            }

            switch (NumeroSemestreRequisiti)
            {
                case 1:
                    dataSemestreRequisiti = new DateTime(AnnoSemestreRequisiti.Value, 6, 30);
                    break;
                case 2:
                    dataSemestreRequisiti = new DateTime(AnnoSemestreRequisiti.Value, 12, 31);
                    break;
            }

            //var etaTitolare = (dataSemestreRequisiti.Value - dataNascitaTitolare.Value).TotalDays / 365.2425;
            //if ((int)etaTitolare < 57)
            if (dataSemestreRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(dataNascitaTitolare.Value.AddYears(57), dataSemestreRequisiti.Value))
            {
                msgVideo = string.Format("Età del titolare inferiore a 57 anni al {0:dd/MM/yyyy}", dataSemestreRequisiti.Value);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica la data perferzionamento requisiti con la decorrenza per una pensione di anzianità Sperimentale Donna
        /// </summary>
        /// <param name="NumeroSemestreRequisiti"></param>
        /// <param name="AnnoSemestreRequisiti"></param>
        /// <param name="datiPensione"></param>
        /// <param name="msgVideo"></param>
        /// <returns></returns>
        private static bool VerificaPerfRequisitiDecPensioneSperDonna(byte? NumeroSemestreRequisiti, short? AnnoSemestreRequisiti, GestionePensione.DatiPensione datiPensione, out string msgVideo)
        {
            msgVideo = string.Empty;
            DateTime annoMeseSemestreRequisiti;

            if (!NumeroSemestreRequisiti.HasValue || !AnnoSemestreRequisiti.HasValue || datiPensione == null || !datiPensione.DecorrenzaOriginaria.HasValue)
            {
                msgVideo = "Decorrenza pensione, Numero e Anno Semestre Requisiti obbligatori";
                return false;
            }

            if (NumeroSemestreRequisiti == 1)
            {
                annoMeseSemestreRequisiti = new DateTime(AnnoSemestreRequisiti.Value + 1, 01, 01);
                if (datiPensione.DecorrenzaOriginaria < annoMeseSemestreRequisiti)
                {
                    msgVideo = string.Format("Decorrenza pensione {0:dd/MM/yyyy} non compatibile con il semestre di raggiungimento dei requisiti {1}/{2}",
                        datiPensione.DecorrenzaOriginaria.Value, NumeroSemestreRequisiti.Value, AnnoSemestreRequisiti.Value);
                    return false;
                }
            }
            else
            {
                if (NumeroSemestreRequisiti == 2)
                {
                    annoMeseSemestreRequisiti = new DateTime(AnnoSemestreRequisiti.Value + 1, 07, 01);
                    if (datiPensione.DecorrenzaOriginaria < annoMeseSemestreRequisiti)
                    {
                        msgVideo = string.Format("Decorrenza pensione {0:dd/MM/yyyy} non compatibile con il semestre di raggiungimento dei requisiti {1}/{2}",
                            datiPensione.DecorrenzaOriginaria.Value, NumeroSemestreRequisiti.Value, AnnoSemestreRequisiti.Value);
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool VerificaDatiGenericiAssicurativiWithSupplementiPresent(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, string NaturaPensione)
        {
            //28-05-12: menu supplementi non più visibile per assegno di invalidità
            if ((!String.IsNullOrEmpty(NaturaPensione) && NaturaPensione.Substring(1, 1).ToUpperInvariant() == "Y") ||
                // TODO: realizzare un Utility per la casistica di cui sotto
                (datiPensione.Gruppo == "0031" && (datiPensione.Prodotto == "0102" || datiPensione.Prodotto == "0302" || datiPensione.Prodotto == "0402")))
            {
                List<DatiSupplementi> LdatiSupplementi = contenitore.ListaDatiSupplementi;
                if (LdatiSupplementi == null || LdatiSupplementi.Count >= 0)
                    return true;
            }
            else if (codiceSpecificoTraduzioneSuGP != 'D' && codiceSpecificoTraduzioneSuGP != 'P')
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                if (tipoFondo == Utility.TipoFondo.VL || tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.ES)
                    return true;

                List<DatiSupplementi> LdatiSupplementi = contenitore.ListaDatiSupplementi;
                if (LdatiSupplementi != null && LdatiSupplementi.Count > 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Verifica l'età del titolare
        /// </summary>
        /// <param name="areaTitolare"></param>
        /// <param name="datiPensione"></param>
        /// <param name="codiceRequisito"></param>
        /// <returns>false se il titolare ha un'età inferiore al parametro richiesto, true altrimenti</returns>
        private static bool? VerificaEtaTitolareFromDecPensione(GestioneAnagrafica.DatiAnagrafici anagrTitolare, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, char? codiceRequisito,
            string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, out string msg)
        {
            msg = string.Empty;
            if (!anagrTitolare.Sesso.HasValue)
                return null;

            if (!anagrTitolare.DataNascita.HasValue)
                return null;

            if (!datiPensione.DecorrenzaOriginaria.HasValue)
                return null;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            int eta = CalcolaEtaTitolareLimite(Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensione, codiceSpecificoTraduzioneSuGP), tipoFondo, codiceRequisito, anagrTitolare.Sesso, tipoSettimaneBeneficio);

            if (eta == 0)
            {
                msg = "Non è stato possibile determinare l'età del Titolare";
                return false;
            }

            if (!Utility.IsDomandaTipoContributivo(datiPensione, null, null) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) && !Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione) &&
                !((!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                  ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))) //ENG - MEMO 166/2023
            {
                if (Utility.DataStrettamenteSuccessivaA(anagrTitolare.DataNascita.Value.AddYears(eta), datiPensione.DecorrenzaOriginaria.Value))
                {
                    msg = "Età del titolare inferiore a " + eta + " anni";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica l'età del titolare
        /// </summary>
        /// <param name="areaTitolare"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="DatiFondoXX"></param>
        /// <param name="codiceRequisito"></param>
        /// <returns>false se il titolare ha un'età inferiore al parametro richiesto, true altrimenti</returns>
        public static bool? VerificaEtaTitolareFromAnte247(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici anagrTitolare, Utility.TipoFondo? tipoFondo, Object DatiFondoXX, char? codiceRequisito,
            string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, out string msg)
        {
            msg = string.Empty;
            byte? NumeroTrimestreRequisiti = null;
            short? AnnoTrimestreRequisiti = null;
            bool? Requisiti247_243 = null;
            DateTime dataTrimestreRequisiti = DateTime.MinValue;

            if (!tipoFondo.HasValue)
                return null;

            int eta = CalcolaEtaTitolareLimite(Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensione, codiceSpecificoTraduzioneSuGP), tipoFondo, codiceRequisito, anagrTitolare.Sesso, tipoSettimaneBeneficio);

            if (eta == 0)
            {
                msg = "Non è stato possibile determinare l'età del Titolare";
                return false;
            }

            switch (tipoFondo)
            {
                case Utility.TipoFondo.EL:
                    NumeroTrimestreRequisiti = ((GestioneFondo.DatiFondoEL)DatiFondoXX).NumeroTriSemRequisiti;
                    AnnoTrimestreRequisiti = ((GestioneFondo.DatiFondoEL)DatiFondoXX).AnnoRequisiti;
                    Requisiti247_243 = ((GestioneFondo.DatiFondoEL)DatiFondoXX).Requisiti247_243;
                    break;
                case Utility.TipoFondo.TT:
                    NumeroTrimestreRequisiti = ((GestioneFondo.DatiFondoTT)DatiFondoXX).NumeroTriSemRequisiti;
                    AnnoTrimestreRequisiti = ((GestioneFondo.DatiFondoTT)DatiFondoXX).AnnoRequisiti;
                    Requisiti247_243 = ((GestioneFondo.DatiFondoTT)DatiFondoXX).Requisiti247_243;
                    break;
                case Utility.TipoFondo.ET:
                    NumeroTrimestreRequisiti = ((GestioneFondo.DatiFondoET)DatiFondoXX).NumeroTriSemRequisiti;
                    AnnoTrimestreRequisiti = ((GestioneFondo.DatiFondoET)DatiFondoXX).AnnoRequisiti;
                    Requisiti247_243 = ((GestioneFondo.DatiFondoET)DatiFondoXX).Requisiti247_243;
                    break;
                case Utility.TipoFondo.VL:
                    NumeroTrimestreRequisiti = ((GestioneFondo.DatiFondoVL)DatiFondoXX).NumeroTriSemRequisiti;
                    AnnoTrimestreRequisiti = ((GestioneFondo.DatiFondoVL)DatiFondoXX).AnnoRequisiti;
                    Requisiti247_243 = ((GestioneFondo.DatiFondoVL)DatiFondoXX).Requisiti247_243;
                    break;
                case Utility.TipoFondo.PT:
                    NumeroTrimestreRequisiti = ((List<GestioneFondo.DatiFondoPT>)DatiFondoXX).First().TrimesteRequisiti;
                    AnnoTrimestreRequisiti = ((List<GestioneFondo.DatiFondoPT>)DatiFondoXX).First().AnnoRequisiti;
                    Requisiti247_243 = ((List<GestioneFondo.DatiFondoPT>)DatiFondoXX).First().RequisitiAnte247;
                    break;
                case Utility.TipoFondo.FS:
                    NumeroTrimestreRequisiti = ((List<GestioneFondo.DatiFondoFST>)DatiFondoXX).First().TrimesteRequisiti;
                    AnnoTrimestreRequisiti = ((List<GestioneFondo.DatiFondoFST>)DatiFondoXX).First().AnnoRequisiti;
                    Requisiti247_243 = ((List<GestioneFondo.DatiFondoFST>)DatiFondoXX).First().RequisitiAnte247;
                    break;

                case Utility.TipoFondo.PI:
                case Utility.TipoFondo.PL:
                    if (DatiFondoXX is List<GestioneFondo.DatiFondoPI>)
                    {
                        List<GestioneFondo.DatiFondoPI> lista =
                            (List<GestioneFondo.DatiFondoPI>)DatiFondoXX;

                        if (lista != null && lista.Count > 0)
                        {
                            NumeroTrimestreRequisiti = ((List<GestioneFondo.DatiFondoPI>)DatiFondoXX).First().NumeroTriSemRequisiti;
                            AnnoTrimestreRequisiti = ((List<GestioneFondo.DatiFondoPI>)DatiFondoXX).First().AnnoRequisiti;
                            Requisiti247_243 = ((List<GestioneFondo.DatiFondoPI>)DatiFondoXX).First().Requisiti247_243;
                        }
                    }
                    else if (DatiFondoXX is GestioneFondo.DatiFondoPI)
                    {
                        GestioneFondo.DatiFondoPI dati =(GestioneFondo.DatiFondoPI)DatiFondoXX;

                        NumeroTrimestreRequisiti = dati.NumeroTriSemRequisiti;
                        AnnoTrimestreRequisiti = dati.AnnoRequisiti;
                        Requisiti247_243 = dati.Requisiti247_243;
                    }
                    break;
                case Utility.TipoFondo.ES:
                    NumeroTrimestreRequisiti = ((GestioneFondo.DatiFondoES)DatiFondoXX).NumeroTriSemRequisiti;
                    AnnoTrimestreRequisiti = ((GestioneFondo.DatiFondoES)DatiFondoXX).AnnoRequisiti;
                    Requisiti247_243 = ((GestioneFondo.DatiFondoES)DatiFondoXX).Requisiti247_243;
                    break;
            }

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            //ENG - MEMO 166/2023
            if (((Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione) ||
                (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                 ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))) && Requisiti247_243.GetValueOrDefault()) ||
                 (datiPensione.DataPerfezionamentoRequisiti.HasValue && datiPensione.DataPerfezionamentoRequisiti.Value.CompareTo(new DateTime(2011, 01, 01)) >= 0))
                return true;
            if (!NumeroTrimestreRequisiti.HasValue || !AnnoTrimestreRequisiti.HasValue)
            {
                msg = "Anno e Trimestre requisiti obbligatori";
                return false;
            }

            if (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione) ||
                (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                 ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))) //ENG - MEMO 166/2023
                return true;

            switch (NumeroTrimestreRequisiti)
            {
                case 1:
                    dataTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 3, 31);
                    break;
                case 2:
                    dataTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 6, 30);
                    break;
                case 3:
                    dataTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 9, 30);
                    break;
                case 4:
                    dataTrimestreRequisiti = new DateTime(AnnoTrimestreRequisiti.Value, 12, 31);
                    break;
            }

            if (Utility.DataStrettamenteSuccessivaA(anagrTitolare.DataNascita.Value.AddYears(eta), dataTrimestreRequisiti))
            {
                msg = "Età del titolare inferiore a " + eta + " anni";
                return false;
            }

            return true;
        }


        /// <summary>
        /// Verifica l'età del titolare
        /// </summary>
        /// <param name="areaTitolare"></param>
        /// <param name="datiPensione"></param>
        /// <param name="codiceRequisito"></param>
        /// <returns>false se il titolare ha un'età inferiore al parametro richiesto, true altrimenti</returns>
        public static bool? VerificaEtaTitolareFromPerfRequisiti(GestioneAnagrafica.DatiAnagrafici anagrTitolare, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione,
            char? codiceRequisito, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, string attivitaSvoltaTraduzioneSuGP, out string msg)
        {
            msg = string.Empty;

            if (!datiPensione.DataPerfezionamentoRequisiti.HasValue)
                return null;

            if (!anagrTitolare.Sesso.HasValue)
                return null;

            if (!anagrTitolare.DataNascita.HasValue)
                return null;

            int eta = CalcolaEtaTitolareLimite(Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensione, codiceSpecificoTraduzioneSuGP), tipoFondo, codiceRequisito, anagrTitolare.Sesso, tipoSettimaneBeneficio);

            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS && attivitaSvoltaTraduzioneSuGP == "5825")
                eta = 58;

            if (eta == 0)
            {
                msg = "Non è stato possibile determinare l'età del Titolare";
                return false;
            }

            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.LIMITE_ETA_TITOLARE))
            {
                if (Utility.DataStrettamenteSuccessivaA(anagrTitolare.DataNascita.Value.AddYears(eta), datiPensione.DataPerfezionamentoRequisiti.Value))
                {
                    msg = "Età del titolare inferiore a " + eta + " anni";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica l'età del titolare di pensione di vecchiaia relativamente alla DecPensione che puo essere ante 2008, compresa o post 2011
        /// </summary>
        /// <param name="anagrTitolare"></param>
        /// <param name="datiPensione"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="datiFondoXX"></param>
        /// <param name="codiceRequisito"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool VerificaEtaTitolareVecchiaia(GestioneAnagrafica.DatiAnagrafici anagrTitolare, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, Object datiFondoXX,
            char? codiceRequisito, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, string attivitaSvoltaTraduzioneSuGP, out string msg)
        {
            msg = string.Empty;

            //mail 03-04-2013: bypass controlli per L214 e usuranti per il solo prodotto 0002
            //mail 28-11-2013: bypass controlli per L.228 RE: Reeng Pensioni - Salvaguardia L.228 - Punti aperti
            //mail 16-07-2014: bypass controlli per L.124 art.11 bis RE: ReEng Pensioni - Salvaguardia L.124/2013 art.11
            if ((Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaUsuranti(datiPensione)
                || Utility.IsDomandaSalvaguardia135(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione)) && datiPensione.Prodotto == "0002")
                return true;

            //mail 24-02-2014: bypass controlli per domande di ricostituzione diverse da Variazione Per Decorrenza
            if (datiPensione.Gruppo == "0031" && !Utility.IsRicostituzione_VariazionePerDecorrenza(datiPensione))
                return true;

            GestioneCrossControls.TipoDecPensione? tipoDecPensione = GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo);
            if (tipoDecPensione.HasValue)
            {
                bool? bReturn = null;
                switch (tipoDecPensione.Value)
                {
                    case GestioneCrossControls.TipoDecPensione.Ante2008Vecchiaia:
                        bReturn = GestioneControlli.VerificaEtaTitolareFromDecPensione(anagrTitolare, datiPensione, tipoFondo, codiceRequisito, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, out msg);
                        if (bReturn.HasValue)
                        {
                            if (!bReturn.Value)
                                return false;
                        }
                        else
                        {
                            msg = "Dati obbligatori mancanti";
                            return false;
                        }
                        return true;
                    case GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia:
                    case GestioneCrossControls.TipoDecPensione.Compreso11Febb12Vecchiaia:

                        if ((datiPensione.DataPerfezionamentoRequisiti.HasValue && Liquidazione.BLCommon.Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 01, 01))))
                            bReturn = GestioneControlli.VerificaEtaTitolareFromPerfRequisiti(anagrTitolare, tipoFondo, datiPensione, codiceRequisito, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP,
                                out msg);
                        else
                            bReturn = GestioneControlli.VerificaEtaTitolareFromAnte247(datiPensione, anagrTitolare, tipoFondo, datiFondoXX, codiceRequisito, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, out msg);

                        if (bReturn.HasValue)
                        {
                            if (!bReturn.Value)
                                return false;
                        }
                        else
                        {
                            msg = "Dati obbligatori mancanti";
                            return false;
                        }

                        return true;
                    case GestioneCrossControls.TipoDecPensione.PostFebb2012Vecchiaia:

                        if ((datiPensione.DataPerfezionamentoRequisiti.HasValue && datiPensione.DataPerfezionamentoRequisiti.Value.Year == 2011))
                            bReturn = GestioneControlli.VerificaEtaTitolareFromPerfRequisiti(anagrTitolare, tipoFondo, datiPensione, codiceRequisito, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP,
                                out msg);
                        else
                        {
                            if ((datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2012, 01, 01))))
                            {
                                if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.EL || tipoFondo.Value == Utility.TipoFondo.TT || tipoFondo.Value == Utility.TipoFondo.ET))
                                    bReturn = VerificaEtaTitolareDataPerfRequisitiFondoEL_ET_TT(tipoFondo, datiPensione.DataPerfezionamentoRequisiti, codiceRequisito, anagrTitolare, datiPensione, tipoSettimaneBeneficio,
                                        codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP, out msg);
                                else
                                    bReturn = true;
                            }
                            else
                                bReturn = true;
                        }

                        if (bReturn.HasValue)
                        {
                            if (!bReturn.Value)
                                return false;
                        }
                        else
                        {
                            msg = "Dati obbligatori mancanti";
                            return false;
                        }

                        return true;
                    default:
                        return true;
                }
            }
            else
            {
                msg = "Dati obbligatori mancanti";
                return false;
            }
        }

        private static bool? VerificaEtaTitolareDataPerfRequisitiFondoEL_ET_TT(Utility.TipoFondo? tipoFondo, DateTime? dataPerfRequisiti, char? codiceRequisito, GestioneAnagrafica.DatiAnagrafici anagrTitolare,
            GestionePensione.DatiPensione datiPensione, string tipoSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP, string attivitaSvoltaTraduzioneSuGP, out string errore)
        {
            errore = string.Empty;
            if (codiceRequisito.HasValue && codiceRequisito.Value == 'A')
                return GestioneControlli.VerificaEtaTitolareFromPerfRequisiti(anagrTitolare, tipoFondo, datiPensione, codiceRequisito, tipoSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP, out errore);
            else
                return GestioneCrossControls.FS_VerificaEtaTitolareDataPerfRequisitiPostFeb2012(tipoFondo, dataPerfRequisiti, anagrTitolare.CodiceFiscale, tipoSettimaneBeneficio, out errore);
        }

        public static bool VerificaProvvisorieta(GestionePensione.DatiPensione datiPensione, bool IsUnicarpe, DateTime? FineAssicurazione, byte? idTipoCalcolo, char? codCom3, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcoloById(idTipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS);
            if (!IsUnicarpe && tipoCalcolo == Utility.TipoCalcolo.Retributivo &&
                FineAssicurazione.HasValue && DateTime.Compare(FineAssicurazione.Value.Date, new DateTime(2012, 1, 1).Date) >= 0)
            {
                if (!codCom3.HasValue || codCom3.Value != 'P')
                {
                    messaggioVideo = "Il campo 'Codice Comunicazioni' deve essere settato al valore 'PROVVISORIA'";
                    return false;
                }
            }

            return true;
        }

        private static int CalcolaEtaTitolareLimite(bool isPensioneVecchiaiaOrRicostituzione, Utility.TipoFondo? tipoFondo, char? codiceRequisito, char? sesso, string tipoSettimaneBeneficio)
        {
            int eta = 0;

            if (!sesso.HasValue)
                return eta;

            if (codiceRequisito.HasValue && codiceRequisito.Value == 'A')
            {
                if (sesso.Value == 'M')
                    eta = 60;
                else
                    eta = 55;

            }
            else
            {
                if (sesso.Value == 'M')
                    eta = 65;
                else
                    eta = 60;
            }

            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
            {
                if (sesso.Value == 'M')
                    eta = 60;
                else
                    eta = 55;
            }

            if (isPensioneVecchiaiaOrRicostituzione && tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.EL || tipoFondo.Value == Utility.TipoFondo.ET || tipoFondo.Value == Utility.TipoFondo.TT))
            {
                if (tipoSettimaneBeneficio == "01") // Non Vedente
                {
                    if (sesso.Value == 'M')
                        eta = 55;
                    else
                        eta = 50;
                }
            }

            return eta;
        }

        /// <summary>
        /// Verifica che il terzo codice natura sia 'H' se e solo se siamo in condizione di trasformazione AOI
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="prodotto"></param>
        /// <param name="tipo"></param>
        /// <param name="codNatura"></param>
        /// <returns>False se non siamo in condizione di trasformazione AOI</returns>
        public static bool ControlsNaturaPensioneWithTrasformazioneAOI(GestionePensione.DatiPensione datiPensione, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codNatura.Substring(2, 1) == "H")
            {
                if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && !Utility.IsRicostituzione(datiPensione.Gruppo) &&
                    !(datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0002"))
                {
                    messaggioVideo = "Il terzo Codice Natura non può essere 'H' se non si è in presenza di una pensione per trasformazione di AOI";
                    return false;
                }
            }
            else
            {
                if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0002")
                {
                    messaggioVideo = "Il terzo Codice Natura deve essere 'H' se non si è in presenza di una pensione per trasformazione di AOI";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Se si è in presenza di pensione privilegiata (PensioneFondoDatiGenerici.Privilegiate=1) il primo  codice natura deve essere valorizzato a 1 o a 2. In caso contrario 
        /// visualizzare il messaggio di errore: “Per le pensioni privilegiate il primo codice natura a deve essere uguale a 1 o 2”).
        /// </summary>
        /// <param name="chkPrivilegiate"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsNaturaForPrivilegiateFS_PT(bool? chkPrivilegiate, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (chkPrivilegiate.GetValueOrDefault())
            {
                if (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("1") && !codNatura.Substring(0, 1).Equals("2")))
                {
                    messaggioVideo = "Per le pensioni privilegiate il primo codice natura deve essere uguale a 1 o 2";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsProvvisoriaPerRiapertura(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, bool isRiapertura, char? codiceComunicazioneCampo3, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (isRiapertura)
            {
                List<GestioneDecodifica.ComunicazioneCampo3> elencoDecodificaComunicazioneCampo3 = contenitoreDecodifica.ElencoDecodificaComunicazioneCampo3;
                if (codiceComunicazioneCampo3.HasValue && elencoDecodificaComunicazioneCampo3.Exists(x => x.Id == codiceComunicazioneCampo3.Value.ToString()))
                {
                    messaggioVideo = "Il codice comunicazione non può essere Provvisoria.";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsCodNaturaSperDonna(GestionePensione.DatiPensione datiPensione, string naturaPensione, byte? tipoCalcolo, char? sessoTitolare, char? sessoDanteCausa,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(1, 1) == "O")
            {
                if (Utility.GetTipoCalcoloById(tipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS) != Utility.TipoCalcolo.Contributivo)
                {
                    messaggioVideo = "Natura pensione non congruente al tipo calcolo.";
                    return false;
                }

                if (sessoDanteCausa.HasValue)
                {
                    if (sessoDanteCausa != 'F')
                    {
                        messaggioVideo = "Secondo codice natura non congruente con il sesso del Dante Causa.";
                        return false;
                    }
                }
                else
                {
                    if (sessoTitolare != 'F')
                    {
                        messaggioVideo = "Secondo codice natura non congruente con il sesso del Titolare.";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsCodNaturaContrib(GestionePensione.DatiPensione datiPensione, string naturaPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(1, 1) == "Y" && (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione) ||
                (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                 ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))) //ENG - Memo 166/2023
            {
                messaggioVideo = "Secondo codice natura 'Y' non selezionabile per pensioni di tipo contributivo";
                return false;
            }

            return true;
        }

        public static bool ControlET_AltraPensDatiAgo(DateTime? decorrenza, string naturaPensione, GestioneFondo.DatiFondoET fondoET, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool ret = true;
            char? codiceNatura1 = naturaPensione.FirstOrDefault();
            if (codiceNatura1 != '6' && fondoET != null && !fondoET.IsNullAltraPensioneDatiAgo())
            {
                messaggioVideo = "Eliminare i dati salvati nel tab DatiCalcolo\\Altra Pensione - Dati AGO.";
                ret = false;
            }
            return ret;
        }

        public static bool ControlsDecorrenzaArretratiINPDAP(Entity.DatiGenericiINPDAP datiGenerici, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiGenerici.DecorrenzaCalcoloArretrati.HasValue && datiGenerici.DecorrenzaCalcoloArretrati.Value < Utility.FirstDayOfMonth(datiPensione.DecorrenzaOriginaria.Value))
            {
                messaggioVideo = "La 'Decorrenza Arretrati' deve essere maggiore o uguale alla 'Decorrenza Pensione'";
                return false;
            }

            return true;
        }

        public static bool ControlsTrattenutaINPDAP(bool? trattenutaINPDAD, DateTime? dataTrattenutaINPDAD, DateTime? decEliminazione,
         GestionePensione.DatiPensione datiPensione, DateTime? dataRinunciaTrattenutaInpdapStorico, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region Recupero Dati
            if (datiPensione == null)
            {
                messaggioVideo = "Dati pensione non disponibili";
                return false;
            }
            DateTime decorrenzaOriginaria = DateTime.MinValue;
            if (datiPensione.DecorrenzaOriginaria.HasValue)
                decorrenzaOriginaria = new DateTime(datiPensione.DecorrenzaOriginaria.Value.Year, datiPensione.DecorrenzaOriginaria.Value.Month, 1);

            #endregion Recupero Dati

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneControlliMemo86", out ctrl);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            //Il Campo Codice Trattenuta INPDAP (GP1AN87A) e la Data Decorrenza Trattenuta INPDAP (GP1AN87D) 
            //non vanno acquisiti per gli assegni ordinari di invalidità e le domande ai Superstiti.
            if (ctrl != null && ctrl.ValoreControllo == "SI" && (trattenutaINPDAD.GetValueOrDefault() || dataTrattenutaINPDAD.HasValue) &&
                (Utility.IsDomandaAssegnoInvaliditaOrdinario(datiPensione) || Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                && !(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
            {
                messaggioVideo = "Il codice trattenuta Fondo Credito e la decorrenza trattenuta Fondo Credito non vanno acquisiti";
                return false;
            }

            //Il Campo Codice Trattenuta INPDAP (GP1AN87A) può contenere “SI” o essere vuoto
            //Il 2 dati devono essere presenti contemporanemente
            if ((trattenutaINPDAD.HasValue && trattenutaINPDAD.Value && !dataTrattenutaINPDAD.HasValue) ||
                (dataTrattenutaINPDAD.HasValue && (!trattenutaINPDAD.HasValue || !trattenutaINPDAD.Value)))
            {
                messaggioVideo = "La decorrenza trattenuta Fondo Credito è necessaria in presenza del codice trattenuta Fondo Credito pari a SI";
                return false;
            }

            if (dataTrattenutaINPDAD.HasValue)
            {
                if (!(ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.DataSuccessivaA(decorrenzaOriginaria, new DateTime(2022, 03, 01))))
                {
                    if (!Utility.IsRicostituzione(datiPensione.Gruppo))
                    {
                        //ENG - TRF con Data Trattenuta valorizzata dal prelievo e decorrenza minore di 03/2022
                        if (Utility.IsRiaperturaDomanda(datiPensione.Id) && dataRinunciaTrattenutaInpdapStorico.HasValue && !Utility.DataSuccessivaA(decorrenzaOriginaria, new DateTime(2022, 03, 01)))
                        {
                            if (!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria))
                            {
                                messaggioVideo = "La decorrenza trattenuta Fondo Credito non deve essere minore della decorrenza originaria";
                                return false;
                            }
                        }
                        else
                        {
                            //Se il Campo GP1AN87D (Decorrenza trattenuta dati INPDAP) è presente e
                            //la Decorrenza Originaria (RAU104) è maggiore di maggio 2008 le due
                            //decorrenze devono essere uguali
                            if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria, new DateTime(2008, 05, 31)))
                            {
                                if (!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria) ||
                                    Utility.DataStrettamenteSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria))
                                {
                                    messaggioVideo = "La decorrenza trattenuta Fondo Credito deve essere pari alla decorrenza originaria";
                                    return false;
                                }
                            }

                            //Se il Campo GP1AN87D (Decorrenza trattenuta dati INPDAP) è presente e
                            //la Decorrenza Originaria (RAU104) è maggiore di novembre 2007 il Campo
                            //GP1AN87D (Decorrenza trattenuta dati INPDAP) deve essere = a giungo
                            //2008 oppure = alla Decorrenza Originaria (RAU104)
                            if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria, new DateTime(2007, 11, 30)))
                            {
                                if ((!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria) ||
                                    Utility.DataStrettamenteSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria)) &&
                                    (!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2008, 6, 1)) ||
                                    Utility.DataStrettamenteSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2008, 6, 1))))
                                {
                                    messaggioVideo = "La decorrenza trattenuta Fondo Credito deve essere pari alla decorrenza originaria o pari a giugno 2008";
                                    return false;
                                }
                            }
                        }

                        //Se il Campo GP1AN87D (Decorrenza trattenuta dati INPDAP) è presente e
                        //la Decorrenza Originaria (RAU104) è inferiore a dicembre 2007 il Campo
                        //GP1AN87D (Decorrenza trattenuta dati INPDAP) deve essere = a novembre
                        //2007 oppure = a giugno 2008
                        if (!Utility.DataSuccessivaA(decorrenzaOriginaria, new DateTime(2007, 12, 1)))
                        {
                            if ((!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2007, 11, 1)) ||
                                Utility.DataStrettamenteSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2007, 11, 1))) &&
                                (!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2008, 6, 1)) ||
                                Utility.DataStrettamenteSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2008, 6, 1))))
                            {
                                messaggioVideo = "La decorrenza trattenuta Fondo Credito deve essere pari a novembre 2007 o pari a giugno 2008";
                                return false;
                            }
                        }

                        //La Data Decorrenza Trattenuta INPDAP (GP1AN87D) non può essere inferiore a novembre 2007
                        if (!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2007, 11, 1)))
                        {
                            messaggioVideo = "La decorrenza trattenuta Fondo Credito non può essere inferiore a novembre 2007";
                            return false;
                        }
                    }
                    else
                    {
                        if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                        {
                            if (!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria))
                            {
                                messaggioVideo = "Non è consentito l’inserimento di una decorrenza trattenuta Fondo Credito precedente alla decorrenza della pensione";
                                return false;
                            }

                            if (Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, new DateTime(2022, 03, 01)))
                            {
                                messaggioVideo = "Non è consentito l'inserimento di una decorrenza trattenuta maggiore di febbraio 2022";
                                return false;
                            }
                        }
                    }
                }
                else if (!dataRinunciaTrattenutaInpdapStorico.HasValue)
                {
                    if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                    {
                        //Aggiornamento Memo86: Per le RIC con decorrenza originaria >= 03/2022, la data trattenuta INPDAP deve essere uguale alla decorrenza originaria
                        if (!Utility.DataSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria) || Utility.DataStrettamenteSuccessivaA(dataTrattenutaINPDAD.Value, decorrenzaOriginaria))
                        {
                            messaggioVideo = "La decorrenza trattenuta Fondo Credito deve essere pari alla decorrenza originaria";
                            return false;
                        }
                    }
                }

                //La Data Decorrenza Trattenuta INPDAP (GP1AN87D) non può essere
                //superiore alla Data Decorrenza Eliminazione (RADECELIM) quando
                //presente
                if (decEliminazione.HasValue)
                {
                    if (Utility.DataStrettamenteSuccessivaA(dataTrattenutaINPDAD.Value, decEliminazione.Value))
                    {
                        messaggioVideo = "La decorrenza trattenuta Fondo Credito non può essere superiore alla decorrenza eliminazione";
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDecorrenzaArretratiPL(DateTime? decorrenzaArretrati, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out annoCompetenza);

            if (decorrenzaArretrati.HasValue)
            {
                if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year == Utility.DataSistemaFs.Year &&
                   (decorrenzaArretrati.Value.Year != decorrenzaOriginaria.Value.Year || decorrenzaArretrati.Value.Month != decorrenzaOriginaria.Value.Month))
                {
                    messaggioVideo = "La 'Decorrenza Arretrati' deve essere uguale alla 'Decorrenza Pensione'";
                    return false;
                }

                if (decorrenzaArretrati.Value.Year > annoCompetenza || decorrenzaArretrati.Value.Year > Utility.DataSistemaFs.Year)
                {
                    messaggioVideo = "La data 'Decorrenza Arretrati' non può essere superiore all'anno di competenza o all'anno solare";
                    return false;
                }

                if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year < annoCompetenza)
                {
                    if (Utility.DataStrettamenteSuccessivaA(decorrenzaArretrati.Value, new DateTime(annoCompetenza, 01, 01)))
                    {
                        messaggioVideo = "La data 'Decorrenza Arretrati' non può essere superiore a gennaio dell'anno di competenza.";
                        return false;
                    }
                }

                if (decorrenzaOriginaria.HasValue && Utility.DataStrettamenteSuccessivaA(Utility.FirstDayOfMonth(decorrenzaOriginaria.Value), decorrenzaArretrati.Value))
                {
                    messaggioVideo = "La data 'Decorrenza Arretrati' non può essere inferiore alla 'Decorrenza Pensione'";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsDecorrenzaArretratiStorico(DateTime? decorrenzaArretrati, DateTime? dataEliminazioneContabile, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!dataEliminazioneContabile.HasValue || !decorrenzaArretrati.HasValue)
                return true;

            if (!Utility.DataStrettamenteSuccessivaA(decorrenzaArretrati.Value, dataEliminazioneContabile.Value))
            {
                messaggioVideo = "La data 'Decorrenza Arretrati' deve essere strettamente maggiore della data eliminazione contabile";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="causaCarico"></param>
        /// <param name="messaggioVideo"></param>
        /// <param name="dataInizioCalcolo"></param>
        /// <returns></returns>
        public static bool ControlsDecorrenzaArretratiRIC(DateTime? decorrenzaArretrati, DateTime? decorrenzaOriginaria, byte? causaCarico, DateTime? dataInizioCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out annoCompetenza);

            if (!decorrenzaArretrati.HasValue)
            {
                messaggioVideo = "La 'Decorrenza Arretrati' è obbligatoria";
                return false;
            }

            if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year == annoCompetenza &&
                 (decorrenzaArretrati.Value.Year != decorrenzaOriginaria.Value.Year || decorrenzaArretrati.Value.Month != decorrenzaOriginaria.Value.Month))
            {
                messaggioVideo = "La 'Decorrenza Arretrati' deve essere uguale alla 'Decorrenza Pensione'";
                return false;
            }

            if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year != annoCompetenza)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaArretrati.Value, new DateTime(annoCompetenza, 01, 01)))
                {
                    messaggioVideo = "La data 'Decorrenza Arretrati' non può essere superiore a gennaio dell'anno di competenza.";
                    return false;
                }

                if (Utility.DataStrettamenteSuccessivaA(Utility.FirstDayOfMonth(decorrenzaOriginaria.Value), decorrenzaArretrati.Value))
                {
                    messaggioVideo = "La data 'Decorrenza Arretrati' non può essere inferiore alla 'Decorrenza Pensione'.";
                    return false;
                }
            }

            if (causaCarico == 3 || causaCarico == 9)
            {
                if (Utility.DataStrettamenteSuccessivaA(dataInizioCalcolo.Value, decorrenzaArretrati.Value))
                {
                    messaggioVideo = "La 'Decorrenza Arretrati' non può essere minore della 'Data di Inizio Calcolo'";
                    return false;
                }
            }

            return true;
        }

        #endregion Dati Generici

        #region Dati Assicurativi

        /// <summary>
        /// Verificare che la prima decorrenza di RecordFondo (RecordFondo.Decorrenza) inserita sia uguale alla decorrenza della pensione (Pensione.DecorrenzaOriginaria) 
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="datiFondo"></param>
        /// <returns></returns>
        public static bool VerificaListDecRecordFondoDecPensione(DateTime? decorrenza, List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo)
        {
            listaDatiRecordFondo.Sort((x, y) => (x.Id.CompareTo(y.Id)));
            return !VerificaDecRecordFondoDecPensione(decorrenza, listaDatiRecordFondo[0].DecorrenzaValiditaDati);
        }

        /// <summary>
        /// Verifica che la decorrenza di RecordFondo (RecordFondo.Decorrenza) inserita sia uguale alla decorrenza della pensione (Pensione.DecorrenzaOriginaria) 
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="datiFondo"></param>
        /// <returns></returns>
        public static bool VerificaDecRecordFondoDecPensione(DateTime? decorrenza, DateTime? DecorrenzaValiditaDati)
        {
            return DecorrenzaValiditaDati.HasValue && DateTime.Compare(decorrenza.Value, DecorrenzaValiditaDati.Value) != 0;
        }

        /// <summary>
        /// Verifica che tutte le RecordFondo.DataSospensione >= alle rispettive RecordFondo.DecorrenzaValiditadati
        /// </summary>
        /// <param name="listaDatiRecordFondo"></param>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool VerificaListDecorDataSospDecorValDatiRecordFondo(List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo)
        {
            foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaDatiRecordFondo)
                if (recordFondo.DataSospensione.HasValue)
                    if (VerificaDecorDataSospDecorValDatiRecordFondo(recordFondo.DataSospensione, recordFondo.DecorrenzaValiditaDati))
                        return false;
            return true;
        }

        /// <summary>
        /// Verifica che la singola RecordFondo.DataSospensione >= alla rispettiva RecordFondo.DecorrenzaValiditadati 
        /// </summary>
        /// <param name="DataSospensione"></param>
        /// <param name="DecorrenzaValiditaDati"></param>
        /// <returns></returns>
        public static bool VerificaDecorDataSospDecorValDatiRecordFondo(DateTime? DataSospensione, DateTime? DecorrenzaValiditaDati)
        {
            return (DataSospensione.HasValue && DecorrenzaValiditaDati.HasValue && (DataSospensione.Value.Date <= DecorrenzaValiditaDati.Value.Date));
        }

        /// <summary>
        /// Verifica che RecordFondo.DataSospensione >= Pensione.DecorrenzaOriginaria
        /// </summary>
        /// <param name="listaDatiRecordFondo"></param>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool VerificaDataSospRecordFondoDecPensione(List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            DateTime? DecorrenzaPensioneOrDecorrenzaDantecausa = datiPensione.SiglaCategoria.Trim() == "SET" || datiPensione.SiglaCategoria.Trim() == "SVL" ? Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null) : datiPensione.DecorrenzaOriginaria;
            foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaDatiRecordFondo)
            {
                if (recordFondo.DataSospensione.HasValue)
                {
                    if (DateTime.Compare(recordFondo.DataSospensione.Value, DecorrenzaPensioneOrDecorrenzaDantecausa.Value) < 0)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica che l'ultimo record fondo inserito per la posizione in esame non abbia la data sospensione (RecordFondo.DataSospensione)
        /// </summary>
        /// <param name="listaDatiRecordFondo"></param>
        /// <returns></returns>
        public static bool VerificaDataSospUltimoRecordFondo(List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo)
        {
            listaDatiRecordFondo = listaDatiRecordFondo.OrderBy(x => x.Id).ToList();
            if (listaDatiRecordFondo[listaDatiRecordFondo.Count - 1].DataSospensione.HasValue)
                return false;
            return true;
        }

        /// <summary>
        /// Verifica la presenza dei DatiCalcolo L214 con DataUltimoVersamento minore del 01/01/2012 
        /// </summary>
        /// <param name="dataUltimoVersamento"></param>
        /// <param name="datiCalcolo"></param>
        /// <returns></returns>
        public static bool VerificaDataUltimoVersamentoWithDatiCalcolo(DateTime? dataUltimoVersamentoWeb, DateTime? dataUltimoVersamentoDB, GestioneContrib.DatiCalcolo datiCalcolo)
        {
            DateTime? dataCompare = new DateTime(2012, 01, 01);

            bool compareWeb = dataUltimoVersamentoWeb.HasValue && (dataUltimoVersamentoWeb.Value < dataCompare);
            bool compareDB = dataUltimoVersamentoDB.HasValue && (dataUltimoVersamentoDB.Value < dataCompare);

            if (compareWeb != compareDB)
            {
                if (!datiCalcolo.IsContribL335Null() || !datiCalcolo.IsContribL214Null())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Verifica la corretta relazione tra dati calcolo, il tipo calcolo e la DataUltimoVersamento
        /// </summary>
        /// <param name="dataUltimoVersamento"></param>
        /// <param name="datiCalcolo"></param>
        /// <param name="tipoCalcolo"></param>
        /// <returns></returns>
        public static bool VerificaDataUltimoVersamentoWithDatiCalcolo(GestionePensione.DatiPensione datiPensione, GestioneContrib.DatiCalcolo datiCalcolo, Utility.TipoCalcolo tipoCalcolo,
            Utility.TipoFondo tipoFondo, char? codiceSpecifico)
        {
            if (datiCalcolo == null)
                return true;

            if (!datiPensione.FineAssicurazione.HasValue)
                return false;

            if (Utility.IsDomandaPensioneInabilitaOrRicostituzioneFS(datiPensione, codiceSpecifico) &&
                !Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01)) && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 02, 01)))
                return true;

            DateTime? dataCompare = new DateTime(2012, 01, 01);
            switch (tipoCalcolo)
            {
                case Utility.TipoCalcolo.Contributivo:
                case Utility.TipoCalcolo.Misto:
                    if (DateTime.Compare(datiPensione.FineAssicurazione.Value, dataCompare.Value) < 0)   // 2011
                    {
                        if (!datiCalcolo.IsContribL214Null() || datiCalcolo.IsContribL335Null())
                            return false;
                    }
                    else //Monti
                    {
                        //per misto Monti e fondo VL nessun controllo su obbligatorietà 335
                        //ENG - PL CONTRIBUZIONE POST 2011
                        if ((tipoFondo == Utility.TipoFondo.VL && tipoCalcolo == Utility.TipoCalcolo.Misto) ||
                            ((tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.TT || tipoFondo == Utility.TipoFondo.VL) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(2011, 12, 31))))
                        {
                            if (datiCalcolo.IsContribL214Null())
                                return false;
                        }
                        else
                        {
                            if (datiCalcolo.IsContribL335Null() || datiCalcolo.IsContribL214Null())
                                return false;
                        }
                    }

                    break;
            }
            return true;
        }

        public static bool VerificaEtaTitolareWithQualificaProfessionale(string gruppo, string prodotto, string tipo, DateTime? dataPerfezionamentoRequisiti, string attivitaSvolta, bool requisitiAnte247, DateTime? dataNascitaTitolare, DateTime? decorrenzaOriginaria, byte? numeroTrimestreRequisiti, short? AnnoTrimestreRequisiti, out int? limiteEta, out DateTime? data)
        {
            limiteEta = null;
            data = null;
            DateTime? dataCompare = null;
            GestioneCrossControls.TipoDecPensione? tipoDec = GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(decorrenzaOriginaria, gruppo, prodotto, tipo);
            if (gruppo.Equals("0001") && prodotto.Equals("0002")) // Vecchiaia
            {
                GestioneDecodifica.AttivitaSvolta attivitaSvoltaDB = null;
                GestioneDecodifica.GetAttivitaSvoltaById(attivitaSvolta, out attivitaSvoltaDB);

                if (attivitaSvolta != null)
                {
                    limiteEta = attivitaSvoltaDB.LimiteEta;
                    dataCompare = dataNascitaTitolare.Value.AddYears(limiteEta.GetValueOrDefault());

                    if (tipoDec.Value == GestioneCrossControls.TipoDecPensione.Compreso0810Vecchiaia)
                    {
                        if (requisitiAnte247)
                        {
                            if (numeroTrimestreRequisiti.HasValue)
                            {
                                switch (numeroTrimestreRequisiti.Value)
                                {
                                    case 1:
                                        data = new DateTime(AnnoTrimestreRequisiti.Value, 03, 31);
                                        if (Utility.DataStrettamenteSuccessivaA(dataCompare.Value, data.Value))
                                            return false;
                                        break;
                                    case 2:
                                        data = new DateTime(AnnoTrimestreRequisiti.Value, 06, 30);
                                        if (Utility.DataStrettamenteSuccessivaA(dataCompare.Value, data.Value))
                                            return false;
                                        break;
                                    case 3:
                                        data = new DateTime(AnnoTrimestreRequisiti.Value, 09, 30);
                                        if (Utility.DataStrettamenteSuccessivaA(dataCompare.Value, data.Value))
                                            return false;
                                        break;
                                    case 4:
                                        data = new DateTime(AnnoTrimestreRequisiti.Value, 12, 31);
                                        if (Utility.DataStrettamenteSuccessivaA(dataCompare.Value, data.Value))
                                            return false;
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dataPerfezionamentoRequisiti.HasValue && !Utility.DataSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2012, 01, 01)))
                        {
                            data = dataPerfezionamentoRequisiti;
                            if (Utility.DataStrettamenteSuccessivaA(dataCompare.Value, dataPerfezionamentoRequisiti.Value))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Sul Codice Non Calcolo non è possibile inserire 'SI'
        /// </summary>
        /// <param name="codiceNonCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodiceNonCalcoloRecordFondo(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, char? codiceNonCalcolo,
            Utility.CategoriaFondoPI? categoriaFondoPI, bool isUltimoRecord, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (categoriaFondoPI.HasValue && (categoriaFondoPI == Utility.CategoriaFondoPI.U || categoriaFondoPI == Utility.CategoriaFondoPI.V))
            {
                if (!codiceNonCalcolo.HasValue || codiceNonCalcolo.Value == ' ')
                {
                    messaggioVideo = "Il codice non calcolo è obbligatorio";
                    return false;
                }
            }
            else if (categoriaFondoPI.HasValue && !(categoriaFondoPI == Utility.CategoriaFondoPI.U || categoriaFondoPI == Utility.CategoriaFondoPI.V))
            {
                //nessun controllo
            }
            else
            {
                //if (!(Utility.IsDomandaReversibilita(datiPensione) && tipoFondo == Utility.TipoFondo.ET && !isUltimoRecord))
                //{
                    if (codiceNonCalcolo.HasValue && codiceNonCalcolo.Value == 'S' && isUltimoRecord)
                    {
                        messaggioVideo = "Codice non calcolo non ammesso: si prega di inviare segnalazione all’indirizzo email istituzionale";
                        return false;
                    }
                //}
            }

            return true;
        }

        /// <summary>
        /// Il metodo non è richiamato perchè serve una maggiore analisi
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="mesiUtiliIndennitaAggiuntiva"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRiscattiUtiliFondoGAS(DateTime? inizioAssicurazione, DateTime? fineAssicurazione, short? mesiUtiliIndennitaAggiuntiva, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (mesiUtiliIndennitaAggiuntiva.HasValue && fineAssicurazione.HasValue && inizioAssicurazione.HasValue)
            {
                int settimane = Utility.NSettimaneBetweenDate(fineAssicurazione.Value, inizioAssicurazione.Value);
                if (mesiUtiliIndennitaAggiuntiva.Value > settimane)
                {
                    messaggioVideo = "Superata la capienza per Riscatti Utili (" + settimane + ")";
                    return false;
                }
            }

            return true;
        }

        public static bool VerificaCodiceRequisitiOrSperimentaleDonna(char? codiceRequisiti2, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, bool isRiaperturaDomanda,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.SPER_DONNA))
            {
                DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
                //controllo per sperimentale donna
                //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
                GestioneLavorazione.DatiLavorazione datiLavorazione = null;
                GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

                if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione))
                {
                    if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione ||
                        isRiaperturaDomanda)))
                    {
                        //ENG - sulla nuova opzione donna (tipo=0190) saltare il controllo 2do campo di Codice Requisiti sia per il fondo FS che per il fondo PT 
                        if (!((Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                            Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) ||
                            Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione)) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                        {
                            if (codiceRequisiti2.HasValue && codiceRequisiti2.Value != '9')
                            {
                                messaggioVideo = "Il 2° campo di 'Codice Requisiti' per sperimentale donna può assumere solo il valore '9'";
                                return false;
                            }
                        }
                    }
                }
                else if (!(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) &&
                           (tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.TT || tipoFondo == Utility.TipoFondo.GAS ||
                            tipoFondo == Utility.TipoFondo.CL || tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)) &&
                            !(Utility.IsRicostituzione(datiPensione.Gruppo) && (tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.ET)))
                {
                    if (codiceRequisiti2.HasValue && codiceRequisiti2.Value != '0')
                    {
                        messaggioVideo = "Il 2° campo di 'Codice Requisiti' può assumere solo il valore '0'";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool VerificaCodiceRequisiti1(char? codiceRequisito1, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceRequisito1.HasValue && !char.IsWhiteSpace(codiceRequisito1.Value)
                && codiceRequisito1.Value != 'A' && codiceRequisito1.Value != '0')
            {
                messaggioVideo = "Il 1° campo di 'Codice Requisiti' può assumere un valore compreso tra ' ', 'A' e '0'";
                return false;
            }

            return true;
        }

        public static bool VerificaImporto13maImporto14maPerET(Entity.DatiAssicurativi datiAssicurativi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiAssicurativi != null && datiAssicurativi.fondoET != null)
            {
                if (datiAssicurativi.fondoET.Importo13ma.GetValueOrDefault() > 9999.9999M)
                {
                    messaggioVideo = "La Tredicesima non può essere superiore a 9999,9999";
                    return false;
                }

                if (datiAssicurativi.fondoET.Importo14ma.GetValueOrDefault() > 9999.9999M)
                {
                    messaggioVideo = "La Quattordicesima non può essere superiore a 9999,9999";
                    return false;
                }
            }

            return true;
        }

        public static bool VerificaDecorenzaTeoricaContributivoPerET(GestionePensione.DatiPensione datiPensione, Entity.DatiAssicurativi datiAssicurativi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione) ||
                (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                 ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))) //ENG - Memo 166/2023
            {
                if (datiAssicurativi.fondoET.DecorrenzaTeorica.HasValue && DateTime.Compare(datiAssicurativi.fondoET.DecorrenzaTeorica.Value, datiPensione.DecorrenzaOriginaria.Value) != 0)
                {
                    messaggioVideo = "La data 'Decorrenza Teorica' deve essere uguale alla 'Decorrenza Pensione'";
                    return false;
                }
            }
            return true;
        }

        public static bool VerificaRetribuzioneMensileINAILPerTT(decimal? retribuzioneMensileINAIL, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (retribuzioneMensileINAIL.GetValueOrDefault() > 9999.9999M)
            {
                messaggioVideo = "La Retribuzione effettiva INAIL non può essere superiore a 9999,9999";
                return false;
            }

            return true;
        }

        public static bool VerificaCodiceRequisiti2CL(char? codiceRequisito2, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!codiceRequisito2.HasValue || (codiceRequisito2.Value != '0' && codiceRequisito2.Value != '1'))
            {
                messaggioVideo = "Codice requisito 2 può essere solo '0' o '1'";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica che l'inizio assicurazione sia successiva al compimento dei 14 anni da parte del titolare o da parte del Dante Causa nel caso di pensione ai superstiti
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="dataNascitaTitolareOrDC"></param>
        /// <param name="tipoBeneficio"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns>Ritorna false se l'inizio assicurazione è antecedente al compimento dei 14 anni</returns>
        public static bool VerificaPrimoVersamento(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, DateTime? inizioAssicurazione, DateTime? dataNascitaTitolareOrDC, string tipoBeneficio, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (inizioAssicurazione.HasValue)
            {
                if (dataNascitaTitolareOrDC.HasValue)
                {
                    if (string.IsNullOrEmpty(tipoBeneficio) || !tipoBeneficio.Equals("02"))
                    {
                        if (!Utility.DataSuccessivaA(inizioAssicurazione.Value, dataNascitaTitolareOrDC.Value.AddYears(14)))
                        {
                            messaggioVideo = "Il primo versamento deve essere posteriore al compimento dei 14 anni di età (" + String.Format("{0:dd/MM/yyyy}", dataNascitaTitolareOrDC.Value.AddYears(14)) + ")";
                            return false;
                        }
                    }
                }

                //FG - Controlli tipo contributivo - data inizio assicurazione
                //ENG - Memo 166/2023
                if (Utility.IsDomandaTipoContributivo(datiPensione, null, false) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione))
                {
                    if (!Utility.DataSuccessivaA(inizioAssicurazione.Value, new DateTime(1996, 01, 01)))
                    {
                        messaggioVideo = "La data di primo versamento non può essere inferiore al 1996";
                        return false;
                    }
                }

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                if (Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                    (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                     ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
                {
                    if (Utility.DataSuccessivaA(inizioAssicurazione.Value, new DateTime(1996, 01, 01)))
                    {
                        messaggioVideo = "La data di inizio assicurazione deve essere inferiore al 1996";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool VerificaDatiAssicurativiObbligatori(short? servizioUtileAA, short? servizioUtileMM, DateTime? dataPerfezionamentoRequisiti, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!servizioUtileAA.HasValue || !servizioUtileMM.HasValue)
            {
                messaggioVideo = "Servizio Utile AA e Servizio Utile MM obbligatori";
                return false;
            }

            if (!dataPerfezionamentoRequisiti.HasValue)
            {
                messaggioVideo = "Data Perf. Requisiti obbligatoria.";
                return false;
            }

            return true;
        }

        public static bool ControlsCapienzaServizioUtile_CL(short? servizioUtileAA, short? servizioUtileMM, DateTime? inizioAssicurazione, DateTime? fineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.DifferenzaDateTime differenza = Utility.DifferenzaBetweenDate(fineAssicurazione.Value.AddDays(1), inizioAssicurazione, Utility.TipoAppartenenza.FS);
            Utility.DifferenzaDateTime servizioUtile = new Utility.DifferenzaDateTime(servizioUtileAA.GetValueOrDefault(), servizioUtileMM.GetValueOrDefault(), 0);

            if (differenza < servizioUtile)
            {
                messaggioVideo = "Il servizio utile eccede la capienza massima consentita (" + differenza.Year + " AA, " + differenza.Month + " MM)";
                return false;
            }

            return true;
        }

        public static bool ControlsServizioUtileAAMM_CL(short? servizioUtileAA, short? servizioUtileMM, bool? codicePensioneSenzaRequisiti, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codicePensioneSenzaRequisiti.HasValue && codicePensioneSenzaRequisiti == true && (servizioUtileAA != 0 || servizioUtileMM != 0))
            {
                messaggioVideo = "Se viene valorizzato il campo Pensione no requ. a SI, anno e mese del servizio utile devono essere valorizzati a 0";
                return false;
            }
            if (codicePensioneSenzaRequisiti.HasValue && codicePensioneSenzaRequisiti == false && (servizioUtileAA < 9 || (servizioUtileAA == 9 && servizioUtileMM <= 6)))
            {
                messaggioVideo = "Se viene valorizzato il campo Pensione no requ. a NO, il servizio utile deve essere maggiore di 9 anni e 6 mesi";
                return false;
            }

            return true;
        }

        public static bool ET_ObbligatorietaElementiAccessori(decimal? elementiAccessori, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool ret = true;
            if (!elementiAccessori.HasValue)
            {
                messaggioVideo = "Elementi Accessori è un dato obbligatorio.";
                ret = false;
            }
            return ret;
        }

        public static bool VerificaAttivitaSvolta_VL(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, string attivitaSvoltaTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
            {
                if (Utility.IsDomandaVecchPerditaTitolo(datiPensione))
                {
                    if (!string.IsNullOrEmpty(attivitaSvoltaTraduzioneSuGP) && attivitaSvoltaTraduzioneSuGP.Trim() != "1" && attivitaSvoltaTraduzioneSuGP.Trim() != "2" &&
                        attivitaSvoltaTraduzioneSuGP.Trim() != "3")
                    {
                        messaggioVideo = "Attività Svolta incompatibile con la tipologia di domanda \"Perdita titolo abilitante\"";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool VerificaDataPerfezionamentoPerPensioneTipoContributivo(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi,
            List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo,
            object objectFondoXX, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, DateTime dataSistema, int? settimaneUtiliDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            //ENG - MEMO 166/2023
            if (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione))
            {
                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.NUM_SETT_PENS))
                {
                    int numSettimaneTipoContibutivo = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP, tipoFondo, settimaneUtiliDiritto, objectFondoXX);
                    if (!GestioneCrossControls.ALL_VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiAnagraficiTitolare, numSettimaneTipoContibutivo, out messaggioVideo))
                        return false;
                }
                else
                {
                    //numSettimaneTipoContibutivo = 2184 per eludere il controllo sul numero settimane, ma controllare solo l'età anagrafica
                    if (!GestioneCrossControls.ALL_VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiAnagraficiTitolare, 2184, out messaggioVideo))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica che se non è presente alcun record fondo con codice non calcolo a SI, allora il tipo calcolo deve essere Retributivo. 
        /// Vale per le domande di categoria *PIU
        /// </summary>
        /// <param name="listaRecordFondo"></param>
        /// <param name="categoriaFondoPI"></param>
        /// <param name="tipoCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaTipoCalcoloConRecordFondo_PIU(GestionePensione.DatiPensione datiPensione, List<Entity.RecordFondo> listaRecordFondo, Utility.CategoriaFondoPI? categoriaFondoPI,
            byte? tipoCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
            {
                if (listaRecordFondo != null && !listaRecordFondo.Exists(x => x.CodiceNonCalcolo == 'S'))
                {
                    Utility.TipoCalcolo tipoCalcoloEnum = Utility.GetTipoCalcoloById(tipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS);
                    if (tipoCalcoloEnum != Utility.TipoCalcolo.Retributivo && tipoCalcoloEnum != Utility.TipoCalcolo.RetributivoMonti)
                    {
                        messaggioVideo = "Il Tipo Calcolo deve essere 'RETRIBUTIVO'. Salvare i Dati Generici.";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica i requisiti di età rispetto alla data perfezionamento requisiti per le domande con categoria *PIU e *PIV
        /// </summary>
        /// <param name="codiceSpecifico"></param>
        /// <param name="dataPerfezionamentoRequisiti"></param>
        /// <param name="dataNascita">Data nascita del Dante Causa se la pensione è ai superstiti, altrimenti Data nascita del Titolare</param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRequisitiEtaPIU_PIV(char? codiceSpecificoTraduzioneSuGP, string attivitaSvoltaTraduzioneSuGP, DateTime? dataPerfezionamentoRequisiti, DateTime? dataNascita,
            Entity.DatiAssicurativi.DatiServizioUtile servizioUtile, DateTime? inizioAssicurazione, DateTime? fineAssicurazione, DateTime? decorrenzaOriginaria, char? sesso, bool isDanteCausa,
            string siglaCategoria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            string titolare_DanteCausa = isDanteCausa ? "Dante Causa" : "Titolare";

            if (codiceSpecificoTraduzioneSuGP.HasValue)
            {
                Utility.DifferenzaDateTime eta = Utility.DifferenzaBetweenDate(dataPerfezionamentoRequisiti, dataNascita, Utility.TipoAppartenenza.FS);
                Utility.DifferenzaDateTime servizioEffettivo = Utility.DifferenzaBetweenDate(fineAssicurazione, inizioAssicurazione, Utility.TipoAppartenenza.FS);

                switch (codiceSpecificoTraduzioneSuGP.Value)
                {
                    #region A
                    case 'A':
                        //if (!siglaCategoria.StartsWith("S") && (!dataPerfezionamentoRequisiti.HasValue || !dataNascita.HasValue))
                        //{
                        //    messaggioVideo = "Data Perferzionamento Requisiti e Data Nascita obbligatori";
                        //    return false;
                        //}

                        if (!string.IsNullOrEmpty(attivitaSvoltaTraduzioneSuGP))
                            switch (attivitaSvoltaTraduzioneSuGP.Trim())
                            {
                                case "1":
                                    if (eta < new Utility.DifferenzaDateTime(60, 0, 0))
                                    {
                                        messaggioVideo = string.Format("Età del {0} inferiore a 60 anni al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                        return false;
                                    }
                                    break;
                                case "2":
                                    if (eta < new Utility.DifferenzaDateTime(62, 0, 0))
                                    {
                                        messaggioVideo = string.Format("Età del {0} inferiore a 62 anni al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                        return false;
                                    }
                                    break;
                                case "3":
                                    if (eta < new Utility.DifferenzaDateTime(65, 0, 0))
                                    {
                                        messaggioVideo = string.Format("Età del {0} inferiore a 65 anni al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                        return false;
                                    }
                                    break;
                            }

                        if (!servizioUtile.IsDatiServizioUtileNull())
                        {
                            Utility.DifferenzaDateTime servizioUtileApp = new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : 0,
                                                                                        servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : 0,
                                                                                        servizioUtile.ServizioUtileGG.HasValue ? servizioUtile.ServizioUtileGG.Value : 0);

                            if (servizioUtileApp < new Utility.DifferenzaDateTime(15, 0, 1))
                            {
                                messaggioVideo = "Servizio Utile inferiore a 15 anni e 1 giorno";
                                return false;
                            }
                        }
                        break;
                    #endregion A
                    #region B
                    case 'B':
                        //if (!siglaCategoria.StartsWith("S") && (!dataPerfezionamentoRequisiti.HasValue || !dataNascita.HasValue))
                        //{
                        //    messaggioVideo = "Data Perferzionamento Requisiti e Data Nascita obbligatori";
                        //    return false;
                        //}

                        if (!string.IsNullOrEmpty(attivitaSvoltaTraduzioneSuGP))
                            switch (attivitaSvoltaTraduzioneSuGP.Trim())
                            {
                                case "1":
                                    if (eta < new Utility.DifferenzaDateTime(55, 0, 0))
                                    {
                                        messaggioVideo = string.Format("Età del {0} inferiore a 55 anni al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                        return false;
                                    }
                                    break;
                                case "2":
                                    if (eta < new Utility.DifferenzaDateTime(57, 0, 0))
                                    {
                                        messaggioVideo = string.Format("Età del {0} inferiore a 57 anni al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                        return false;
                                    }
                                    break;
                                case "3":
                                    if (eta < new Utility.DifferenzaDateTime(60, 0, 0))
                                    {
                                        messaggioVideo = string.Format("Età del {0} inferiore a 60 anni al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                        return false;
                                    }
                                    break;
                            }

                        if (!servizioUtile.IsDatiServizioUtileNull())
                        {
                            Utility.DifferenzaDateTime servizioUtileApp = new Utility.DifferenzaDateTime(servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : 0,
                                                                                        servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : 0,
                                                                                        servizioUtile.ServizioUtileGG.HasValue ? servizioUtile.ServizioUtileGG.Value : 0);

                            if (servizioUtileApp < new Utility.DifferenzaDateTime(15, 0, 1))
                            {
                                messaggioVideo = "Servizio Utile inferiore a 15 anni e 1 giorno";
                                return false;
                            }
                        }
                        break;
                    #endregion B
                    #region C
                    case 'C':
                        //if (!siglaCategoria.StartsWith("S") && (!dataPerfezionamentoRequisiti.HasValue || !dataNascita.HasValue))
                        //{
                        //    messaggioVideo = "Data Perferzionamento Requisiti e Data Nascita obbligatori";
                        //    return false;
                        //}

                        if (!sesso.HasValue)
                        {
                            messaggioVideo = "Sesso del " + titolare_DanteCausa + " obbligatorio.";
                            return false;
                        }

                        // Se il primo accredito contributivo decorre dal 1° gennaio 1996
                        if (inizioAssicurazione.HasValue && Utility.DataSuccessivaA(inizioAssicurazione.Value, new DateTime(1996, 1, 1)))
                        {
                            if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year == 2012)
                            {
                                switch (sesso.Value)
                                {
                                    case 'M':
                                        if (eta < new Utility.DifferenzaDateTime(42, 1, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 42 anni e 1 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                    case 'F':
                                        if (eta < new Utility.DifferenzaDateTime(41, 1, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 41 anni e 1 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                }
                            }
                            else if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year == 2013)
                            {
                                switch (sesso.Value)
                                {
                                    case 'M':
                                        if (eta < new Utility.DifferenzaDateTime(42, 5, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 42 anni e 5 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                    case 'F':
                                        if (eta < new Utility.DifferenzaDateTime(41, 5, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 41 anni e 5 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                }
                            }
                            else if (decorrenzaOriginaria.HasValue && (decorrenzaOriginaria.Value.Year == 2014 || decorrenzaOriginaria.Value.Year == 2015))
                            {
                                switch (sesso.Value)
                                {
                                    case 'M':
                                        if (eta < new Utility.DifferenzaDateTime(42, 6, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 42 anni e 6 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                    case 'F':
                                        if (eta < new Utility.DifferenzaDateTime(41, 6, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 41 anni e 6 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                }
                            }
                            else if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year >= 2016)
                            {
                                switch (sesso.Value)
                                {
                                    case 'M':
                                        if (eta < new Utility.DifferenzaDateTime(42, 10, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 42 anni e 10 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                    case 'F':
                                        if (eta < new Utility.DifferenzaDateTime(41, 10, 0))
                                        {
                                            messaggioVideo = string.Format("Età del {0} inferiore a 41 anni e 10 mese al {1:dd/MM/yyyy}", titolare_DanteCausa, dataPerfezionamentoRequisiti.Value);
                                            return false;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    #endregion C
                    #region D
                    case 'D':
                        if (servizioEffettivo < new Utility.DifferenzaDateTime(20, 0, 0))
                        {
                            messaggioVideo = "Servizio effettivo inferiore a 20 anni.";
                            return false;
                        }
                        break;
                    #endregion D
                    #region E G
                    case 'E':
                    case 'G':
                        if (servizioEffettivo < new Utility.DifferenzaDateTime(5, 0, 0))
                        {
                            messaggioVideo = "Servizio effettivo inferiore a 5 anni.";
                            return false;
                        }
                        break;
                        #endregion E G
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se, in presenza di codice non calcolo a NO, sono presenti i dati Ex combattente
        /// </summary>
        /// <param name="isCodiceNoCalcoloNOPresente"></param>
        /// <param name="datiMaggiorazioniBenefici"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaExCombattentePerPIU(bool isCodiceNoCalcoloNOPresente, long? exCombattente, decimal? rmsSenzaLegge33670QA,
            Utility.CategoriaFondoPI? categoriaFondoPI, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
            {
                if (isCodiceNoCalcoloNOPresente && (exCombattente.HasValue || rmsSenzaLegge33670QA.HasValue))
                {
                    messaggioVideo = "Non è possibile avere una gestione con calcolo se sono presenti i dati Ex Combattente e/o RMS Senza Legge 336/70 Quota A.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica la congruenza del codice specifico con la decorrenza della pensione
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="codiceSpecificoTraduzioneSuGP"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsCodiceSpecificoAnteArmonizzazione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, char? codiceSpecificoTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.VL:
                        if (codiceSpecificoTraduzioneSuGP.HasValue)
                        {
                            switch (codiceSpecificoTraduzioneSuGP.Value)
                            {
                                case 'A':
                                case 'B':
                                case 'C':
                                case 'D':
                                case 'E':
                                    if (!Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC))
                                    {
                                        messaggioVideo = "Codice Specifico " + codiceSpecificoTraduzioneSuGP + " non ammesso per pensioni Post Armonizzazione.";
                                        return false;
                                    }
                                    break;
                            }
                        }
                        break;
                    case Utility.TipoFondo.ET:
                        if (codiceSpecificoTraduzioneSuGP.HasValue)
                        {
                            switch (codiceSpecificoTraduzioneSuGP.Value)
                            {
                                case 'B':
                                case 'C':
                                    if (Utility.DataStrettamenteSuccessivaA(decorrenzaPensioneOrDecorrenzaPensioneDC.GetValueOrDefault(), new DateTime(1996, 7, 1)))
                                    {
                                        messaggioVideo = "Codice Specifico " + codiceSpecificoTraduzioneSuGP + " non ammesso per pensioni Post Armonizzazione.";
                                        return false;
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica la congruenza dei dati servizio militare con la decorrenza della pensione
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="codiceServizioMilitare"></param>
        /// <param name="nSettimaneLeva"></param>
        /// <param name="nSettimaneRichiamato"></param>
        /// <param name="contributiAgoLegge40245"></param>
        /// <param name="contributiAgoLegge140830"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsServizioMilitareFondoET(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool? codiceServizioMilitare,
            short? nSettimaneLeva, short? nSettimaneRichiamato, decimal? contributiAgoLegge40245, decimal? contributiAgoLegge140830, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria,
                datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if ((nSettimaneLeva.HasValue || nSettimaneRichiamato.HasValue) &&
                decorrenzaPensioneOrDecorrenzaPensioneDC.HasValue && Utility.DataSuccessivaA(decorrenzaPensioneOrDecorrenzaPensioneDC.Value, new DateTime(1996, 9, 1)))
            {
                messaggioVideo = "I dati relativi al servizio militare non sono ammessi.</br>Eliminare i dati Assicurativi.";
                return false;
            }

            return true;
        }

        public static bool ControlsCodiceCapitalizzazione(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, byte? codiceCapitalizzazione, decimal? importoPercentualeCapitalizzazione,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL && Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione &&
                !isRiaperturaDomanda)
            {
                if (codiceCapitalizzazione.HasValue)
                {
                    switch (codiceCapitalizzazione.Value)
                    {
                        case 1:
                            if (importoPercentualeCapitalizzazione.GetValueOrDefault() > 5000)
                            {
                                messaggioVideo = "Imp - % Capitalizzazione deve essere minore o uguale a 5000.";
                                return false;
                            }
                            break;
                        case 2:
                            if (importoPercentualeCapitalizzazione.GetValueOrDefault() > 50)
                            {
                                messaggioVideo = "Imp - % Capitalizzazione deve essere minore o uguale a 5000.";
                                return false;
                            }
                            break;
                    }
                }
            }

            return true;
        }

        public static bool ControlsPrimoVersamentoPerAPEPrecoci(GestionePensione.DatiPensione datiPensione, DateTime? inizioAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaAPEPrecoci(datiPensione))
            {
                if (inizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(inizioAssicurazione.Value, new DateTime(1995, 12, 31)))
                {
                    messaggioVideo = "La data primo versamento deve essere inferiore o uguale al 31/12/1995";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerAPEPrecoci(GestionePensione.DatiPensione datiPensione, GestioneContrib.DatiCalcolo datiCalcolo, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int limiteSettimane = 2132;

            if (Utility.IsDomandaAPEPrecoci(datiPensione))
            {
                int numSettimaneTipoContibutivo = 0;
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

                switch (tipoFondo)
                {
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                        numSettimaneTipoContibutivo = datiCalcolo != null ? datiCalcolo.SettimaneUtiliDiritto.GetValueOrDefault() : 0;
                        break;
                    case Utility.TipoFondo.FS:
                        numSettimaneTipoContibutivo += datiCalcolo.fondoFST.ServizioUtileDirittoAA.GetValueOrDefault() * 52;
                        //numSettimaneTipoContibutivo += Convert.ToInt32(datiCalcolo.fondoFST.ServizioUtileDirittoMM.GetValueOrDefault() * 4.333);
                        //numSettimaneTipoContibutivo += Convert.ToInt32(datiCalcolo.fondoFST.ServizioUtileDirittoGG.GetValueOrDefault() / 6.923);
                        break;
                    case Utility.TipoFondo.PT:
                        numSettimaneTipoContibutivo += datiCalcolo.fondoPT.ServizioUtileDirittoAA.GetValueOrDefault() * 52;
                        //numSettimaneTipoContibutivo += Convert.ToInt32(datiCalcolo.fondoPT.ServizioUtileDirittoMM.GetValueOrDefault() * 4.333);
                        //numSettimaneTipoContibutivo += Convert.ToInt32(datiCalcolo.fondoPT.ServizioUtileDirittoGG.GetValueOrDefault() / 6.923);
                        break;
                    default:
                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && listaRecordDatiFondoINPDAP != null && listaRecordDatiFondoINPDAP.Count > 0)
                        {
                            //numSettimaneTipoContibutivo = listaRecordDatiFondoINPDAP.Sum(x => x.ServizioUtileDirittoAA.GetValueOrDefault() * 52 +
                            //Convert.ToInt32(x.ServizioUtileDirittoMM.GetValueOrDefault() * 4.333) +
                            //Convert.ToInt32(x.ServizioUtileDirittoGG.GetValueOrDefault() / 6.923));

                            numSettimaneTipoContibutivo = listaRecordDatiFondoINPDAP.Sum(x => x.ServizioUtileDirittoAA.GetValueOrDefault() * 52);
                        }
                        else
                        {
                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + datiCalcolo.NSettimane.GetValueOrDefault(); //LIQ.PENS DATI CONTRIBUTIVI L.355
                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + datiCalcolo.NSettimaneQuotaDL214.GetValueOrDefault(); //LIQ.PENS DATI CONTRIBUTIVI L.214

                            //TIPO VL = ANNI * 52 + MESI * 4,333 + GIORNI / 6.923
                            double numSettimaneDaAnniAnte0697 = datiCalcolo.AnzianitaAnte0697AA.GetValueOrDefault() * 52;
                            double numSettimaneDaMesiAnte0697 = datiCalcolo.AnzianitaAnte0697MM.GetValueOrDefault() * 4.333;
                            double numSettimaneDaGiorniAnte0697 = datiCalcolo.AnzianitaAnte0697GG.GetValueOrDefault() / 6.923;

                            double numSettimaneDaAnniPost0697 = datiCalcolo.AnzianitaPost0697AA.GetValueOrDefault() * 52;
                            double numSettimaneDaMesiPost0697 = datiCalcolo.AnzianitaPost0697MM.GetValueOrDefault() * 4.333;
                            double numSettimaneDaGiorniPost0697 = datiCalcolo.AnzianitaPost0697GG.GetValueOrDefault() / 6.923;

                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaAnniAnte0697);
                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaMesiAnte0697);
                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaGiorniAnte0697);

                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaAnniPost0697);
                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaMesiPost0697);
                            numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + Convert.ToInt32(numSettimaneDaGiorniPost0697);

                            numSettimaneTipoContibutivo += datiCalcolo.NSettimaneQuotaA.GetValueOrDefault();
                            numSettimaneTipoContibutivo += datiCalcolo.NSettimaneQuotaA2.GetValueOrDefault();
                            numSettimaneTipoContibutivo += datiCalcolo.NSettimaneQuotaB.GetValueOrDefault();
                            numSettimaneTipoContibutivo += datiCalcolo.NSettimaneQuotaC.GetValueOrDefault();
                            numSettimaneTipoContibutivo += datiCalcolo.NSettimaneQuotaC2.GetValueOrDefault();
                            numSettimaneTipoContibutivo += datiCalcolo.NSettimaneQuotaD.GetValueOrDefault();

                            List<GestioneContrib.DatiServizioUtile> listaDatiServizioUtile = null;
                            if (tipoFondo.HasValue)
                            {
                                switch (tipoFondo.Value)
                                {
                                    case Utility.TipoFondo.EL:
                                        listaDatiServizioUtile = datiCalcolo.fondoEL != null ? datiCalcolo.fondoEL.LServizioUtile : null;
                                        break;
                                    case Utility.TipoFondo.TT:
                                        listaDatiServizioUtile = datiCalcolo.fondoTT != null ? datiCalcolo.fondoTT.lDatiServizioUtile : null;
                                        break;
                                    case Utility.TipoFondo.ET:
                                        listaDatiServizioUtile = datiCalcolo.fondoET != null ? datiCalcolo.fondoET.lDatiServizioUtile : null;
                                        break;
                                    case Utility.TipoFondo.VL:
                                        listaDatiServizioUtile = datiCalcolo.fondoVL != null ? datiCalcolo.fondoVL.LServizioUtile : null;
                                        break;
                                    case Utility.TipoFondo.PT:
                                        listaDatiServizioUtile = datiCalcolo.fondoPT != null ? datiCalcolo.fondoPT.lDatiServizioUtile : null;
                                        break;
                                    case Utility.TipoFondo.FS:
                                        listaDatiServizioUtile = datiCalcolo.fondoFST != null ? datiCalcolo.fondoFST.lDatiServizioUtile : null;
                                        break;
                                    case Utility.TipoFondo.DZ:
                                        listaDatiServizioUtile = datiCalcolo.fondoDZ != null ? datiCalcolo.fondoDZ.lDatiServizioUtile : null;
                                        break;
                                }
                            }

                            if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                            {
                                foreach (var servizioUtile in listaDatiServizioUtile)
                                {
                                    numSettimaneTipoContibutivo += servizioUtile.ServizioUtileAA.GetValueOrDefault() * 52;
                                    numSettimaneTipoContibutivo += Convert.ToInt32(servizioUtile.ServizioUtileMM.GetValueOrDefault() * 4.333);
                                    numSettimaneTipoContibutivo += Convert.ToInt32(servizioUtile.ServizioUtileGG.GetValueOrDefault() / 6.923);
                                    numSettimaneTipoContibutivo += servizioUtile.ServizioUtileCessazioneAA.GetValueOrDefault() * 52;
                                    numSettimaneTipoContibutivo += Convert.ToInt32(servizioUtile.ServizioUtileCessazioneMM.GetValueOrDefault() * 4.333);
                                    numSettimaneTipoContibutivo += Convert.ToInt32(servizioUtile.ServizioUtileCessazioneGG.GetValueOrDefault() / 6.923);
                                }
                            }
                        }
                        break;
                }

                if (numSettimaneTipoContibutivo < limiteSettimane && !(datiPensione.SiglaCategoria != null && datiPensione.SiglaCategoria.Trim().EndsWith("CTPS") && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                {
                    messaggioVideo = string.Format("Il numero delle settimane deve essere maggiore o uguale a {0}", limiteSettimane);
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerRequisitoAnticipatoArt1(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile,
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo, object objectFondoXX, int? settimaneUtilidiritto,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) ||
                Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione))
            {
                int nLimiteSettimane = 1560;
                int nSettimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaDatiServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                    tipoFondo, settimaneUtilidiritto, objectFondoXX);

                if (nSettimane < nLimiteSettimane)
                {
                    messaggioVideo = string.Format("Il numero delle settimane deve essere maggiore o uguale a {0}", nLimiteSettimane);
                    return false;
                }
            }
            return true;
        }

        public static bool ControlReversibilitaConCodiceSpecificoP(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa danteCausa, char? codiceSpecificoTraduzioneSuGP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<Utility.TipoFondo> listaFondi = new List<Utility.TipoFondo> { Utility.TipoFondo.EL, Utility.TipoFondo.TT, Utility.TipoFondo.VL, Utility.TipoFondo.ET };
            List<char> listaCodiciSpecifici = new List<char> { 'P' };
            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa) && listaFondi.Any(x => x == Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria)) &&
                danteCausa != null && danteCausa.SiglaCategoria.StartsWith("I") && listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
            {
                messaggioVideo = "Codice Specifico non ammesso";
                return false;
            }
            return true;
        }

        public static bool ControlsNSettimanePerQuota100(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile,
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo, object objectFondoXX,
            int? settimaneUtiliDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaQuota100(datiPensione))
            {
                int nSettimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                    tipoFondo, settimaneUtiliDiritto, objectFondoXX);
                if (nSettimane < 1976)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 1976 (38 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerSperimentaleDonna_DL_4_2019(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile,
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo, object objectFondoXX,
            int? settimaneUtiliDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione))
            {
                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.NUM_SETT_PENS))
                {
                    int nSettimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                        tipoFondo, settimaneUtiliDiritto, objectFondoXX);
                    if (nSettimane < 1820)
                    {
                        messaggioVideo = "Il numero settimane non può essere inferiore a 1820 (35 anni di contribuzione)";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerOpzioneDonna_Legge197_2022_Art1_Comma292(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile,
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo, object objectFondoXX,
            int? settimaneUtiliDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione)
                || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione))
            {
                int nSettimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                        tipoFondo, settimaneUtiliDiritto, objectFondoXX);
                if (nSettimane < 1820)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 1820 (35 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerAnzianitaPerLeggeBilancio2019(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile, char? sessoTitolare,
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo, object objectFondoXX, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) 
                && !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && Utility.IsDomandaINPDAP(datiPensione.Gestione))
                && !Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                // Per le domande del fondo VL non si possono applicare i controlli sul requisito contributivo perchè questi hanno dei requisiti diversi dalla generalità delle pensioni
                // Rif. LiqPens - Anticipate_2019@20190807_v1.7.docx
                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
                    return true;

                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_FS.NUM_SETT_PENS))
                {
                    int nSettimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                        tipoFondo, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, objectFondoXX);
                    switch (sessoTitolare)
                    {
                        case 'M':
                            if (nSettimane < 2227)
                            {
                                messaggioVideo = "Il numero settimane non può essere inferiore a 2227 (42 anni e 10 mesi di contribuzione)";
                                return false;
                            }
                            break;
                        case 'F':
                            if (nSettimane < 2175)
                            {
                                messaggioVideo = "Il numero settimane non può essere inferiore a 2175 (41 anni e 10 mesi di contribuzione)";
                                return false;
                            }
                            break;
                        default:
                            messaggioVideo = "Sesso del titolare non presente nell'anagrafica";
                            return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerQuota102(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
              List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile,
              List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo, object objectFondoXX,
              int? settimaneUtiliDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaQuota102(datiPensione))
            {
                int nSettimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                    tipoFondo, settimaneUtiliDiritto, objectFondoXX);
                if (nSettimane < 1976)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 1976 (38 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerAnticipateFlessibili(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi,
           List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile,
           List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, Utility.TipoFondo? tipoFondo, object objectFondoXX,
           int? settimaneUtiliDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if ((Utility.IsDomandaAnticipataFlessibile(datiPensione) && datiPensione.SiglaCategoria.Trim() != "VOCTPS") || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
               ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
            {
                int nSettimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                    tipoFondo, settimaneUtiliDiritto, objectFondoXX);
                if (nSettimane < 2132)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 2132 (41 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool VerificaDecorrenzaFineAssicurazioneINPDAP(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, DateTime? dataFineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenzaPensioneDirettaDC = null;

            if (!Utility.IsDomandaINPDAP(datiPensione.Gestione))
                return true;

            if (datiPensione.SiglaCategoria != null && datiPensione.SiglaCategoria.Trim().StartsWith("I"))
                return true;

            if (Utility.IsDomandaReversibilita(datiPensione))
            {
                GestioneDatiFondo.GetCrossProperties(datiPensione, datiFondo, out decorrenzaPensioneDirettaDC);
            }

            if (dataFineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(2010, 07, 31), dataFineAssicurazione.Value) && datiPensione.SiglaCategoria.StartsWith("V"))
            {
                if ((Utility.IsDomandaReversibilita(datiPensione) && decorrenzaPensioneDirettaDC.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(2010, 07, 31), decorrenzaPensioneDirettaDC.Value)) ||
                    (!Utility.IsDomandaReversibilita(datiPensione) && datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(2010, 07, 31), datiPensione.DecorrenzaOriginaria.Value)))
                {
                    messaggioVideo = "La data decorrenza deve essere maggiore o uguale al 31/07/2010";
                    return false;
                }
            }

            return true;
        }


        #endregion Dati Assicurativi

        #endregion LiquidazionePensione

        #region Titolare
        /// <summary>
        /// Verifica che la domanda può essere inviata al calcolo a partire dal giorno 10 del mese precedente la decorrenza pensione
        /// </summary>
        /// <param name="DecorrenzaPensione"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool VerificaDecPensioneInvioPreCalcolo(DateTime? DecorrenzaPensione, out string msg)
        {
            msg = string.Empty;

            if (!DecorrenzaPensione.HasValue)
                return false;

            DateTime dataInvio;
            if (DecorrenzaPensione.Value.Month - 1 == 0)
                dataInvio = new DateTime(DecorrenzaPensione.Value.Year - 1, 12, 10);
            else
                dataInvio = new DateTime(DecorrenzaPensione.Value.Year, DecorrenzaPensione.Value.Month - 1, 10);


            if (dataInvio.CompareTo(Utility.DataSistemaFs) > 0)
            {
                msg = string.Format("Le posizioni con decorrenza '{0:MM/yyyy}' possono essere inviate al calcolo a partire dal {1:dd/MM/yyyy}", DecorrenzaPensione.Value, dataInvio);
                return false;
            }

            return true;
        }



        /// <summary>
        /// Verifica se il sindacato passato in input è presente nella lista dei sindacati attivi
        /// </summary>
        /// <param name="sindacato"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool VerificaSindacatoAttivo(GestionePensione.DatiSindacato sindacato, string siglaCategoria, out string msg)
        {
            msg = string.Empty;
            List<Liquidazione.BLCommon.Entity.Sindacato> elencoSindacato = null;
            if (sindacato != null)
            {
                string idCategoria = Liquidazione.BLCommon.GestioneSindacati.GetIdCategoriaForSindacato(siglaCategoria, out msg);
                if (!string.IsNullOrEmpty(msg))
                    return false;

                GestioneDelegheSindacali.GetElencoSindacatiPerCategoria(idCategoria, out elencoSindacato, out msg);
                if (!string.IsNullOrEmpty(msg))
                    return false;

                elencoSindacato = Liquidazione.BLCommon.GestioneSindacati.GetElencoSindacatiAttivi(elencoSindacato, out msg);
                if (!string.IsNullOrEmpty(msg))
                    return false;

                int index = elencoSindacato.FindIndex(x => x.Id == sindacato.CodiceSindacato.Trim());
                if (index < 0)
                {
                    msg = "Il Sindacato attualmente salvato non è più attivo.";
                    return false;
                }
            }
            return true;
        }


        #endregion Titolare

        public static bool IsValoreAAMMGGValido(short? AA, short? MM, short? GG)
        {
            if (GG.HasValue && (GG.Value.ToString().Length > 2 || GG.Value > 29))
                return false;
            if (MM.HasValue && (MM.Value.ToString().Length > 2 || MM.Value > 11))
                return false;
            if (AA.HasValue && AA.Value.ToString().Length > 2)
                return false;

            return true;
        }

        #region DatiFondo
        public static bool ControlPALBeneficiPAL(decimal? palBenefici, decimal? pal, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (palBenefici.HasValue && pal.HasValue && palBenefici < pal)
            {
                messaggioVideo = "Il campo 'PAL con benefici' deve essere maggiore del campo 'Pensione Annua Lorda'.";
                return false;
            }
            return true;
        }

        public static bool ControlScadenzaBeneficiWithDecorrenzaFondo(DateTime? scadenzaBenefici, DateTime? decorrenzaFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (scadenzaBenefici.HasValue && !decorrenzaFondo.HasValue)
            {
                messaggioVideo = "Decorrenza Registrazione Fondo assente. Salvare i Dati Fondo.";
                return false;
            }

            if (scadenzaBenefici.HasValue &&
                !Utility.DataStrettamenteSuccessivaA(Utility.FirstDayOfMonth(scadenzaBenefici.Value), Utility.FirstDayOfMonth(decorrenzaFondo.Value)))
            {
                messaggioVideo = "La Scadenza Benefici deve essere successiva alla Decorrenza Registrazione Fondo.";
                return false;
            }

            return true;
        }
        #endregion DatiFondo

        #region Dati No Calcolo
        public static bool ControlsDecNoCalcoloWithRecordFondo(GestionePensione.DatiPensione datiPensione, List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo, string sDecorrenzaNoCalcolo, out string messageVideo)
        {
            GestioneAreaNoCalcolo.DateCustom currentDecRec = GestioneAreaNoCalcolo.DateCustom.Parse(sDecorrenzaNoCalcolo);
            messageVideo = string.Empty;
            if (lstRecordFondo != null && lstRecordFondo.Count > 0)
            {
                var firstRecordFondo = lstRecordFondo.OrderBy(x => x.DecorrenzaValiditaDati).Where(x => x.CodiceNonCalcolo == 'N').FirstOrDefault();
                if (firstRecordFondo != null && firstRecordFondo.DecorrenzaValiditaDati.HasValue)
                {
                    GestioneAreaNoCalcolo.DateCustom cdRecordFondo = new GestioneAreaNoCalcolo.DateCustom(firstRecordFondo.DecorrenzaValiditaDati.Value);
                    if (currentDecRec >= cdRecordFondo)
                    {
                        messageVideo = string.Format("La decorrenza registrazione ({0}) non può essere maggiore alla data di decorrenza del primo del record fondo con codice no calcolo a N ({1})", currentDecRec, firstRecordFondo.DecorrenzaValiditaDati.Value.ToString("MM/yyyy"));
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ControlsDecPensioneWithDecNoCalcolo(GestionePensione.DatiPensione datiPensione, string decorrenzaNoCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria) != Utility.CategoriaFondoPI.V &&
                Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria) != Utility.CategoriaFondoPI.U)
                return true;

            GestioneAreaNoCalcolo.DateCustom minDateDecRec = new GestioneAreaNoCalcolo.DateCustom(datiPensione.DecorrenzaOriginaria.GetValueOrDefault().AddMonths(1));
            GestioneAreaNoCalcolo.DateCustom currentDecRec = GestioneAreaNoCalcolo.DateCustom.Parse(decorrenzaNoCalcolo);

            //controllo decorrenza pensione - la decorrenza registrazione deve essere superiore al mese successivo alla decorrenzaPensione. 
            if (minDateDecRec > currentDecRec)
            {
                messaggioVideo = string.Format("La decorrenza registrazione ({0}) deve essere maggiore del mese successivo alla decorrenza pensione ({1})", currentDecRec, datiPensione.DecorrenzaOriginaria.Value.ToString("MM/yyyy"));
                return false;
            }
            return true;
        }

        public static bool ControlsFamiliari(GestionePensione.DatiPensione datiPensione, string decorrenzaNoCalcolo, List<string> codFiscFamiliariSelezionati,
          ref List<GestioneFamiliari.Familiare> listaFamiliari, ref List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche, ref List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool ret = true;
            if (codFiscFamiliariSelezionati != null && codFiscFamiliariSelezionati.Count > 0)
            {
                //tutte queste query vengono effettuate solo quando necessarie ai controlli

                if (listaFamiliari == null && listaAnagrafiche == null)
                    GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagrafiche);
                if (listaCodMaggFamiliari == null)
                    GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaCodMaggFamiliari);

                var lstFamiliari = from familiare in listaFamiliari
                                   join codmag in listaCodMaggFamiliari on familiare.IdAnagrafica equals codmag.IdAnagrafica
                                   select new { CodiceFiscale = familiare.CodiceFiscale, InizioCarico = codmag.Decorrenza, FineCarico = codmag.Cessazione };

                GestioneAreaNoCalcolo.DateCustom cdDecorReg = GestioneAreaNoCalcolo.DateCustom.Parse(decorrenzaNoCalcolo);

                foreach (var fam in codFiscFamiliariSelezionati)
                {
                    bool decCompresa = false;
                    var lstFamWithCarico = lstFamiliari.Where(x => x.CodiceFiscale == fam).ToList();
                    foreach (var famWithCarico in lstFamWithCarico)
                    {
                        GestioneAreaNoCalcolo.DateCustom cdInizioCarico = new GestioneAreaNoCalcolo.DateCustom(famWithCarico.InizioCarico.GetValueOrDefault());
                        GestioneAreaNoCalcolo.DateCustom cdFineCarico = new GestioneAreaNoCalcolo.DateCustom(famWithCarico.FineCarico.HasValue ? famWithCarico.FineCarico.Value.AddMonths(1).AddDays(-1) : new DateTime(9999, 12, 31));
                        //decorrenza registrazione può avere la 13-esima mensilità. In questo vanno effettuati i controlli
                        //con dicembre
                        if (cdFineCarico.Month == 12)
                            cdFineCarico.Month = 13;
                        if (cdDecorReg >= cdInizioCarico && cdDecorReg <= cdFineCarico)
                        {
                            decCompresa = true;
                            break;
                        }
                    }
                    if (!decCompresa)
                    {
                        messaggioVideo = string.Format("Il familiare con codice ficale ({0}) non risulta essere a carico alla decorrenza registrazione ({1})", fam, cdDecorReg);
                        return false;
                    }
                }
            }
            return ret;
        }

        #endregion Dati No Calcolo

        #region MaggiorazioniBenefici
        #region Benefici
        public static bool ControlsSettimaneBeneficioNonVedenteWithDatiCalcolo(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivi,
            GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivi, List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile, string tipoSettimaneBeneficio, int? nSettimaneBeneficio, char? codiceSpecificoTraduzioneSuGP,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                        if (Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensione, codiceSpecificoTraduzioneSuGP) && tipoSettimaneBeneficio == "01")
                        {
                            decimal maxValue = 0.0M;
                            if (datiCalcoloRetributivi != null)
                                maxValue += datiCalcoloRetributivi.NSettimaneQuotaA.GetValueOrDefault() + datiCalcoloRetributivi.NSettimaneQuotaB.GetValueOrDefault() + datiCalcoloRetributivi.NSettimaneQuotaC.GetValueOrDefault() + datiCalcoloRetributivi.NSettimaneQuotaD.GetValueOrDefault();

                            if (datiCalcoloContributivi != null)
                                maxValue += datiCalcoloContributivi.NSettimane.GetValueOrDefault() + datiCalcoloContributivi.NSettimaneQuotaDL214.GetValueOrDefault();

                            if (tipoFondo.Value == Utility.TipoFondo.ET && listaServizioUtile != null)
                                foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtile in listaServizioUtile)
                                    maxValue += (((servizioUtile.ServizioUtileAA.GetValueOrDefault() * 360) +
                                                    (servizioUtile.ServizioUtileMM.GetValueOrDefault() * 30) +
                                                    servizioUtile.ServizioUtileGG.GetValueOrDefault()) / 6.923M);

                            maxValue = Math.Ceiling(maxValue);

                            if (nSettimaneBeneficio.GetValueOrDefault() > maxValue)
                            {
                                messaggioVideo = "Il numero settimane del beneficio eccede la capienza massima consentita (" + maxValue + ").";
                                return false;
                            }
                        }
                        break;
                }
            }

            return true;
        }

        public static bool ControlsBeneficioPrecoci(GestionePensione.DatiPensione datiPensione, object datiFondoXX, GestioneFondo.DatiFondo datiFondo, string tipoSettimaneBeneficio,
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC, Utility.TipoFondo? tipoFondo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondoXX: datiFondoXX, datiFondo: datiFondo)
                && tipoSettimaneBeneficio == "11")
            {
                messaggioVideo = "Non è possibile acquisire il beneficio \"LAVORATORE PRECOCE\" per una domanda ante armonizzazione.";
                return false;
            }

            if (!Utility.IsDomandaAPEPrecoci(datiPensione) && tipoSettimaneBeneficio == "11")
            {
                messaggioVideo = "Non è possibile acquisire il beneficio \"LAVORATORE PRECOCE\" per una domanda non di tipologia APE Precoci";
                return false;
            }

            return true;
        }

        public static bool ControlsDecMaggiorazioneSociale(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DateTime? decMaggSociale, GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiPensione == null)
            {
                messaggioVideo = "Dati pensione non disponibili";
                return false;
            }

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            char? codiceSpecificoTraduzioneSuGP = null;
            GestioneFondo.DatiFondo datiFondo = contenitore.DatiFondo;
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


            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneControlliMemo72", out ctrl);
            //fondi VL, ET, TT, EL oppure fondi FS, PT con GP2NB14 CLF = F
            //con decorrenza maggiore di 31/07/1984,
            //Ricostituzioni: terzo byte numero certificato uguale a 2 o 5
            //PL: sigla categoria che inizia con 'I'
            //GP1AV37N uguale a 6 
            if (ctrl != null && ctrl.ValoreControllo == "SI" && (codiceSpecificoTraduzioneSuGP != null && (((tipoFondo == Utility.TipoFondo.VL && codiceSpecificoTraduzioneSuGP == 'E') || (tipoFondo == Utility.TipoFondo.ET && (codiceSpecificoTraduzioneSuGP == 'Q' || codiceSpecificoTraduzioneSuGP == 'I')) || ((tipoFondo == Utility.TipoFondo.TT || tipoFondo == Utility.TipoFondo.EL) && codiceSpecificoTraduzioneSuGP == 'Q')) ||
                ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && codiceSpecificoTraduzioneSuGP == 'F')) &&
                ((Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.NCertificato.HasValue && (datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "2" || datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "5")) ||
                (!Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.SiglaCategoria.StartsWith("I"))) &&
                Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1984, 7, 31)) && datiAnagraficiTitolare != null &&
                !Utility.DataStrettamenteSuccessivaA(decMaggSociale.Value, datiAnagraficiTitolare.DataNascita.Value.AddYears(60))))
            {
                //se decorrenza maggiorazione sociale minore di 1/08/2020
                //va verificata la condizione dei 60 anni
                if (!Utility.DataStrettamenteSuccessivaA(decMaggSociale.Value, new DateTime(2020, 7, 31)))
                {
                    messaggioVideo = "La decorrenza maggiorazione sociale deve essere maggiore del mese di compimento del 60esimo anno di età del titolare.";
                    return false;
                }
            }

            return true;
        }
        #endregion Benefici

        #region Vittime terrorismo
        public static bool ControlsCoerenzaBeneficioVittimeTerrorismo(long? tipologiaPrestazione, long? tipologiaBeneficio, long? soggettoBeneficiario, string soggettoBeneficiarioTraduzioneSuGP,
           out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!string.IsNullOrEmpty(soggettoBeneficiarioTraduzioneSuGP) && soggettoBeneficiarioTraduzioneSuGP.Trim() == "V2")
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2 && tipologiaPrestazione.GetValueOrDefault() != 3)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Vittima con invalidità => 25%\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\" o \"Art. 4 comma 2 bis L. 206/2004\"";
                    return false;
                }

                if (tipologiaPrestazione.GetValueOrDefault() == 2)
                {
                    if (tipologiaBeneficio.GetValueOrDefault() != 1 && tipologiaBeneficio.GetValueOrDefault() != 2)
                    {
                        messaggioVideo = "Per Tipologia di Prestazione \"Art. 2 e 3 L. 206/2004\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° settembre 2004\" o \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                        return false;
                    }
                }

                if (tipologiaPrestazione.GetValueOrDefault() == 3)
                {
                    if (tipologiaBeneficio.GetValueOrDefault() != 5)
                    {
                        messaggioVideo = "Per Soggetto Beneficiario \"Vittima con invalidità < 25%\" e Tipologia di Prestazione \"Art. 4 comma 2 bis L. 206/2004\" la Tipologia di Beneficio può essere solo \"Benefici art. 4 con decorrenza dal 1° gennaio 2007\"";
                        return false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(soggettoBeneficiarioTraduzioneSuGP) && soggettoBeneficiarioTraduzioneSuGP.Trim() == "V1")
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Vittima con invalidità < 25%\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaPrestazione.GetValueOrDefault() == 2)
                {
                    if (tipologiaBeneficio.GetValueOrDefault() != 1 && tipologiaBeneficio.GetValueOrDefault() != 2)
                    {
                        messaggioVideo = "Per Tipologia di Prestazione \"Art. 2 e 3 L. 206/2004\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° settembre 2004\" o \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                        return false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(soggettoBeneficiarioTraduzioneSuGP) && soggettoBeneficiarioTraduzioneSuGP.Trim() == "G")
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Genitore\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaBeneficio.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Genitore\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                    return false;
                }
            }

            if (soggettoBeneficiario.GetValueOrDefault() == 4 || soggettoBeneficiario.GetValueOrDefault() == 7)
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge/Vedovo\" o \"Figlio/orfano\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaBeneficio.GetValueOrDefault() != 2 && tipologiaBeneficio.GetValueOrDefault() != 3)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge/Vedovo\" o \"Figlio/orfano\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\" o \"Benefici art. 2-3 con decorrenza sia dal 1° settembre 2004 che dal 1° gennaio 2007\"";
                    return false;
                }
            }

            if (soggettoBeneficiario.GetValueOrDefault() == 5 || soggettoBeneficiario.GetValueOrDefault() == 8)
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge\" o \"Figlio\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaBeneficio.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge\" o \"Figlio\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsDecorrenzaEventoTerroristico(DateTime? dataEventoTerroristico, DateTime dataPresentazioneDomanda, char? codiceEvento, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (dataEventoTerroristico.HasValue && codiceEvento == 'I' && (!Utility.DataSuccessivaA(dataEventoTerroristico.Value, new DateTime(1961, 1, 1)) ||
                Utility.DataSuccessivaA(dataEventoTerroristico.Value, dataPresentazioneDomanda)))
            {
                messaggioVideo = string.Format("Se il Codice Evento è ITALIA la Data Evento Terroristico deve essere successiva al 01/01/1961 e antecedente alla data di presentazione della domanda ({0:dd/MM/yyyy})", dataPresentazioneDomanda);
                return false;
            }

            return true;
        }

        public static bool ControlsDatiCalcoloVittimeTerrorismoWithVisibility(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloContributivo> lDatiCalcoloContributivo,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo, long? soggettoBeneficiario, long? tipologiaPrestazione, long? tipologiaBeneficio,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, Utility.TipoCalcolo tipoCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (listaDatiCalcoloVittimeTerrorismo != null && listaDatiCalcoloVittimeTerrorismo.Count > 0)
            {
                if (!Utility.IsDatiRetributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcolo) &&
                    listaDatiCalcoloVittimeTerrorismo.Exists(x => x.Tipo == 'R'))
                {
                    messaggioVideo = "Non è possibile acquisire i Dati Retributivi Vittime.";
                    return false;
                }


                if (!Utility.IsDatiContributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo, tipoCalcolo,
                    lDatiCalcoloContributivo != null && lDatiCalcoloContributivo.Exists(x => x.IsQuotaDL214Presente())) && listaDatiCalcoloVittimeTerrorismo.Exists(x => x.Tipo == 'C'))
                {
                    messaggioVideo = "Non è possibile acquisire i Dati Contributivi Vittime.";
                    return false;
                }

                if (!Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiario, tipologiaPrestazione, tipologiaBeneficio) &&
                    listaDatiCalcoloVittimeTerrorismo.Exists(x => x.Tipo == 'I'))
                {
                    messaggioVideo = "Non è possibile acquisire i dati Importo Pensione Vittime se il Soggetto Beneficiario è diverso da Vittima.";
                    return false;
                }
            }

            return true;
        }
        #endregion Vittime terrorismo

        #endregion MaggiorazioniBenefici

        #region Detrazioni

        public static bool ControlsObbligatorietaDetrazioni(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, List<GestioneFamiliari.Familiare> listaFamiliari,
            List<GestioneDetrazioniContitolare.DatiDetrazioniContitolare> listaDetrazioniContitolare, GestioneDanteCausa.DatiDanteCausa danteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != Utility.TipoAppartenenza.FS || !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                || (controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))))
                return true;

            //ENG - REVERSIBILITA FS (NO INPDAP/024)  
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DisabilitaDetrazioniObbligatorieContitolariFS", out controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS);
            int annoCompetenzaFS = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out annoCompetenzaFS);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    if (fam.CodiceFiscale == datiAnagrafici.CodiceFiscale)
                        continue;

                    fam.IsDetrazioniObbligatorieContitolare = true;
                }
            }


            if (controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS != null && !String.IsNullOrEmpty(controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS.ValoreControllo)
                && controlloDinamicoDisabilitaDetrazioniObbligatorieContitolariFS.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.FS && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa)
                    && !Utility.IsDomandaINPDAP(datiPensione.Gestione) && tipoFondo != Utility.TipoFondo.FS && tipoFondo != Utility.TipoFondo.PT)
                {
                    if (listaFamiliari != null && listaFamiliari.Count > 0)
                    {
                        List<GestioneFamiliari.CodMaggFamiliari> listaMaggiorazioniFamiliari = null;
                        GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaMaggiorazioniFamiliari);

                        foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                        {
                            if (fam.CodiceFiscale == datiAnagrafici.CodiceFiscale)
                                continue;

                            if (listaMaggiorazioniFamiliari != null && listaMaggiorazioniFamiliari.Exists(x => x.IdAnagrafica == fam.IdAnagrafica && x.Cessazione.HasValue))
                            {
                                DateTime? dataCessazione = listaMaggiorazioniFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica && x.Cessazione.HasValue).OrderByDescending(x => x.Cessazione).First().Cessazione;
                                if (dataCessazione.HasValue && dataCessazione.Value.Year < annoCompetenzaFS)
                                {
                                    fam.IsDetrazioniObbligatorieContitolare = false;
                                }
                            }
                        }
                    }

                }
            }

            if (listaFamiliari != null && listaFamiliari.Count > 0 &&
                listaFamiliari.Exists(x => x.CodiceFiscale != datiAnagrafici.CodiceFiscale && x.IsDetrazioniObbligatorieContitolare && !listaDetrazioniContitolare.Exists(y => y.IdAnagrafica == x.IdAnagrafica)))
            {
                messaggioVideo = "Acquisire le detrazioni per tutti i soggetti contitolari.";
                return false;
            }

            return true;
        }

        #endregion Detrazioni

        #region DelegheTutele
        public static bool IsPresenteDelegatoTutoreTitolareMinorenne(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, GestioneAnagrafica.DatiAnagrafici datiTutore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiPensione == null || datiAnagrafici == null || !datiAnagrafici.DataNascita.HasValue || !datiPensione.DecorrenzaOriginaria.HasValue)
                return false;

            DateTime dataConfronto = Utility.FirstDayOfMonth(datiAnagrafici.DataNascita.Value).AddYears(18);

            if (datiTutore == null && !Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataConfronto))
            {
                messaggioVideo = "Titolare minorenne manca rappresentante legale o tutore ";
                return false;
            }

            return true;
        }

        #endregion DelegheTutele
    }
}
