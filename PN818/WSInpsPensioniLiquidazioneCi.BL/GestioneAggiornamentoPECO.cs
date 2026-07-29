using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using INPS.Pensioni.LiquidazioneCi.ServiceReferences.AggPec;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Configuration;
using System.Reflection;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneAggiornamentoPECO
    {
        #region public method

        internal static bool GetDatiPECO_CIbyNumeroDomanda(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Convenzioni_Internazionali();
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.CI_Funzione = "G";
                else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                    dati.CI_Funzione = "H";
                else
                    dati.CI_Funzione = "L";
                dati.CI_Numdomus = Convert.ToString(datiPensione.NDomus);
                AggiornamentoPECO_CI(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC"], ref dati, datiPensione.NDomus.ToString(),
                    out errore);
                if (!String.IsNullOrEmpty(errore) || dati.CI_Return_Code != 0)
                {
                    if (String.IsNullOrEmpty(errore))
                        errore = "Codice di ritorno della posizione richiesta (diverso da 0): " + dati.CI_Return_Code.ToString();
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

        public static void GetDatiTotali(GestionePensione.DatiPensione datiPensione, out DatiTotaliAggPec datiAggPec, out string errori)
        {
            datiAggPec = null;
            char? cNull = null;
            int? intNull = null;
            decimal? dNull = null;
            errori = string.Empty;

            csAggiornamentoPECO_Convenzioni_Internazionali dati = null;
            GetDatiPECO_CIbyNumeroDomanda(datiPensione, ref dati, out errori);

            if (dati != null)
            {
                datiAggPec = new DatiTotaliAggPec();
                if (dati.aCONTRIBUTIVA_CI != null)
                {
                    datiAggPec.lContribuzione = new List<DatiContributivi>();
                    for (int i = 0; i < dati.aCONTRIBUTIVA_CI.Count(); i++)
                    {
                        if (!IsContributivaCINull(dati.aCONTRIBUTIVA_CI[i]))
                        {
                            DatiContributivi datiContrib = new DatiContributivi();

                            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                            GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);
                            if (elencoCodeGestioneCalcoloContributivo != null && elencoCodeGestioneCalcoloContributivo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo.Find(x => !string.IsNullOrEmpty(x.TraduzioneSuGP) &&
                                    !string.IsNullOrEmpty(dati.aCONTRIBUTIVA_CI[i].CI_Codgest_Contr) &&
                                    x.TraduzioneSuGP.Trim() == dati.aCONTRIBUTIVA_CI[i].CI_Codgest_Contr.Trim() && !x.IsFondo);
                                if (codeGestioneCalcoloContributivo != null)
                                    datiContrib.CodiceGestione = codeGestioneCalcoloContributivo.Id;
                            }

                            datiContrib.Quota = dati.aCONTRIBUTIVA_CI[i].CI_CodQuota_Contr.Trim() != string.Empty ? Convert.ToChar(dati.aCONTRIBUTIVA_CI[i].CI_CodQuota_Contr.Trim().ToUpperInvariant()) : (char?)null;

                            if (datiContrib.Quota.HasValue && datiContrib.Quota.Value.ToString().ToUpperInvariant() == "C")
                            {
                                datiContrib.ImportoContributivoTotale = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA_CI[i].CI_Contrib, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA_CI[i].CI_Contrib) : dNull;
                                datiContrib.MontanteContributivo = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA_CI[i].CI_Montante, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA_CI[i].CI_Montante) : dNull;
                                datiContrib.Nsettimane = dati.aCONTRIBUTIVA_CI[i].CI_Anzconc != 0 ? dati.aCONTRIBUTIVA_CI[i].CI_Anzconc : intNull;

                            }
                            else if (datiContrib.Quota.HasValue && datiContrib.Quota.Value.ToString().ToUpperInvariant() == "D")
                            {
                                datiContrib.ImportoContributivoQuotaD = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA_CI[i].CI_Contrib, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA_CI[i].CI_Contrib) : (decimal?)null;
                                datiContrib.MontanteContributivoQuotaD = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA_CI[i].CI_Montante, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA_CI[i].CI_Montante) : (decimal?)null;
                                datiContrib.SettimaneQuotaD = dati.aCONTRIBUTIVA_CI[i].CI_Anzconc != 0 ? dati.aCONTRIBUTIVA_CI[i].CI_Anzconc : (int?)null;
                            }

                            datiAggPec.lContribuzione.Add(datiContrib);
                        }
                    }
                    if (datiAggPec.lContribuzione.Count == 0)
                        datiAggPec.lContribuzione = null;
                }
                if (dati.aRETRIBUTIVA_CI != null)
                {
                    datiAggPec.lRetribuzione = new List<DatiRetributivi>();

                    for (int i = 0; i < dati.aRETRIBUTIVA_CI.Count(); i++)
                    {
                        if (!IsRetributivaCINull(dati.aRETRIBUTIVA_CI[i]))
                        {
                            DatiRetributivi datiRetrib = new DatiRetributivi();

                            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                            GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);
                            if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == dati.aRETRIBUTIVA_CI[i].CI_Codgest_Retr.Trim() && !x.IsFondo);
                                if (codeGestioneCalcoloRetributivo != null)
                                    datiRetrib.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                            }

                            datiRetrib.QuotePrimeLiquidate = dati.aRETRIBUTIVA_CI[i].CI_Codquota.Trim() != string.Empty ? Convert.ToChar(dati.aRETRIBUTIVA_CI[i].CI_Codquota.Trim().ToUpperInvariant()) : cNull;
                            if (datiRetrib.QuotePrimeLiquidate.HasValue && datiRetrib.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                            {
                                datiRetrib.NSettimaneQuotaA = dati.aRETRIBUTIVA_CI[i].CI_Anzconr;
                                datiRetrib.RMSQuotaA = !Utility.IsDoubleEquals(dati.aRETRIBUTIVA_CI[i].CI_Rms, 0.0) ? Convert.ToDecimal(dati.aRETRIBUTIVA_CI[i].CI_Rms) : dNull;
                                datiRetrib.Nsettimane707 = dati.aRETRIBUTIVA_CI[i].CI_Anzcon707 != 0 ? dati.aRETRIBUTIVA_CI[i].CI_Anzcon707 : (int?)null;
                            }
                            else
                                if (datiRetrib.QuotePrimeLiquidate.HasValue && datiRetrib.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                                {
                                    datiRetrib.NSettimaneQuotaB = dati.aRETRIBUTIVA_CI[i].CI_Anzconr;
                                    datiRetrib.RMSQuotaB = !Utility.IsDoubleEquals(dati.aRETRIBUTIVA_CI[i].CI_Rms, 0.0) ? Convert.ToDecimal(dati.aRETRIBUTIVA_CI[i].CI_Rms) : dNull;
                                    datiRetrib.Nsettimane707 = dati.aRETRIBUTIVA_CI[i].CI_Anzcon707 != 0 ? dati.aRETRIBUTIVA_CI[i].CI_Anzcon707 : (int?)null;
                                }
                            datiAggPec.lRetribuzione.Add(datiRetrib);
                        }
                    }
                    if (datiAggPec.lRetribuzione.Count == 0)
                        datiAggPec.lRetribuzione = null;
                }
                if (dati.aISTITUZIONI_CI != null && dati.aISTITUZIONI_CI.Count() > 0)
                {
                    datiAggPec.lIstituzioniEstere = new List<DatiIstituzioniEstere>();
                    foreach (CI_ISTITUZIONI istituzionePECO in dati.aISTITUZIONI_CI)
                    {
                        if (!IsIstituzioneEsteraCINull(istituzionePECO))
                        {
                            DatiIstituzioniEstere istituzione = new DatiIstituzioniEstere();
                            istituzione.CodiceStatoEE = istituzionePECO.CI_Stato;
                            istituzione.CodiceIstituzione = istituzionePECO.CI_Istit;
                            if (!Utility.IsDoubleEquals(istituzionePECO.CI_Misest, 0.0))
                                istituzione.ContributiEEDecorrenzaOriginaria = (int)istituzionePECO.CI_Misest;
                            if (!Utility.IsDoubleEquals(istituzionePECO.CI_Direst, 0.0))
                                istituzione.ContributiEEDiritto = (int)istituzionePECO.CI_Direst;

                            datiAggPec.lIstituzioniEstere.Add(istituzione);
                        }
                    }

                    if (datiAggPec.lIstituzioniEstere.Count == 0)
                        datiAggPec.lIstituzioniEstere = null;
                }

                if (dati.CI_Est95 >= 0)
                    datiAggPec.ContributiItalianiEdEsteriAl1295 = dati.CI_Est95;
            }
        }

        internal static void ImpostaDatiControllo(DatiTotaliAggPec datiAggPec, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiAggPec.lContribuzione != null && datiAggPec.lRetribuzione == null)
            {
                datiAggPec.DatiControllo = new DatiControllo();
                datiAggPec.DatiControllo.TipoCalcolo = TipoCalcolo.Contributivo;
                datiAggPec.DatiControllo.IsCalcoloValido = true;
            }
            else if (datiAggPec.lContribuzione == null && datiAggPec.lRetribuzione != null)
            {
                datiAggPec.DatiControllo = new DatiControllo();
                datiAggPec.DatiControllo.TipoCalcolo = TipoCalcolo.Retributivo;
                datiAggPec.DatiControllo.IsCalcoloValido = true;
            }
            else
            {
                datiAggPec.DatiControllo = new DatiControllo();
                datiAggPec.DatiControllo.TipoCalcolo = TipoCalcolo.Misto;
                datiAggPec.DatiControllo.IsCalcoloValido = true;
            }
        }

        #endregion public method

        #region private method

        private static void AggiornamentoPECO_CI(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Convenzioni_Internazionali dati, string numDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            Guid guid = Guid.NewGuid();

            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Convenzioni_Internazionali, Utility.SOAPLogDirection.IN, numDomanda, guid,
                        dati.CI_Funzione);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC, method Aggiornamento_PECO_CI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC, method Aggiornamento_PECO_CI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC method Aggiornamento_PECO_CI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC method Aggiornamento_PECO_CI: {0}", Utility.GetMessageFromException(Ex));
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
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_Convenzioni_Internazionali, Utility.SOAPLogDirection.OUT, numDomanda, guid,
                        dati.CI_Funzione);
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static bool IsContributivaCINull(CI_CONTRIBUTIVA contrib)
        {
            if (contrib.CI_Anzconc == 0 && contrib.CI_Codgest_Contr.Trim() == string.Empty &&
                Utility.IsDoubleEquals(contrib.CI_Contrib, 0.0) && Utility.IsDoubleEquals(contrib.CI_Montante, 0.0))
                return true;
            else
                return false;
        }

        private static bool IsRetributivaCINull(CI_RETRIBUTIVA retrib)
        {
            if (retrib.CI_Anzconr == 0 && retrib.CI_Codgest_Retr.Trim() == string.Empty &&
                retrib.CI_Codquota.Trim() == string.Empty && Utility.IsDoubleEquals(retrib.CI_Rms, 0.0))
                return true;
            else
                return false;
        }

        private static bool IsIstituzioneEsteraCINull(CI_ISTITUZIONI istituzione)
        {
            if ((string.IsNullOrEmpty(istituzione.CI_Stato) || string.IsNullOrEmpty(istituzione.CI_Stato.Trim())) &&
                (string.IsNullOrEmpty(istituzione.CI_Istit) || string.IsNullOrEmpty(istituzione.CI_Istit.Trim())) &&
                Utility.IsDoubleEquals(istituzione.CI_Direst, 0.0) && Utility.IsDoubleEquals(istituzione.CI_Misest, 0.0))
                return true;

            return false;
        }

        #endregion private method

        #region nested class

        public class DatiTotaliAggPec
        {
            #region private properties

            private List<DatiContributivi> _lContribuzione;
            private List<DatiRetributivi> _lRetribuzione;
            private List<DatiContributiEsteri> _lContribuzioneEE;
            private List<DatiIstituzioniEstere> _lIstituzioniEstere;
            private DatiControllo _DatiControllo;
            private int? _ContributiItalianiEdEsteriAl1295;
            #endregion private properties

            #region public properties
            public List<DatiContributivi> lContribuzione { get { return _lContribuzione; } set { _lContribuzione = value; } }
            public List<DatiRetributivi> lRetribuzione { get { return _lRetribuzione; } set { _lRetribuzione = value; } }
            public List<DatiContributiEsteri> lContribuzioneEE { get { return _lContribuzioneEE; } set { _lContribuzioneEE = value; } }
            public List<DatiIstituzioniEstere> lIstituzioniEstere { get { return _lIstituzioniEstere; } set { _lIstituzioniEstere = value; } }
            public DatiControllo DatiControllo { get { return _DatiControllo; } set { _DatiControllo = value; } }
            public int? ContributiItalianiEdEsteriAl1295 { get { return _ContributiItalianiEdEsteriAl1295; } set { _ContributiItalianiEdEsteriAl1295 = value; } }
            #endregion public properties

            public bool IsNull()
            {
                if ((lContribuzione == null || lContribuzione.Count == 0) &&
                    (lRetribuzione == null || lRetribuzione.Count == 0) &&
                    (lContribuzioneEE == null || lContribuzioneEE.Count == 0) &&
                    (lIstituzioniEstere == null || lIstituzioniEstere.Count == 0) &&
                    DatiControllo == null &&
                    !ContributiItalianiEdEsteriAl1295.HasValue)
                    return true;
                return false;
            }
        }

        public class DatiContributivi
        {
            #region private properties

            private long? _CodiceGestione;
            private decimal? _MontanteContributivo;
            private decimal? _ImportoContributivoTotale;
            private int? _Nsettimane;
            private char? _Quota;
            private int? _SettimaneQuotaD;
            private decimal? _MontanteContributivoQuotaD;
            private decimal? _ImportoContributivoQuotaD;

            #endregion private properties

            #region public properties

            public long? CodiceGestione { get { return _CodiceGestione; } set { _CodiceGestione = value; } }
            public decimal? MontanteContributivo { get { return _MontanteContributivo; } set { _MontanteContributivo = value; } }
            public decimal? ImportoContributivoTotale { get { return _ImportoContributivoTotale; } set { _ImportoContributivoTotale = value; } }
            public int? Nsettimane { get { return _Nsettimane; } set { _Nsettimane = value; } }
            public char? Quota { get { return _Quota; } set { _Quota = value; } }
            public int? SettimaneQuotaD { get { return _SettimaneQuotaD; } set { _SettimaneQuotaD = value; } }
            public decimal? MontanteContributivoQuotaD { get { return _MontanteContributivoQuotaD; } set { _MontanteContributivoQuotaD = value; } }
            public decimal? ImportoContributivoQuotaD { get { return _ImportoContributivoQuotaD; } set { _ImportoContributivoQuotaD = value; } }

            #endregion public properties
        }

        public class DatiRetributivi
        {
            #region private properties

            private DateTime? _DecorrenzaOriginariaPensione;
            private long? _CodiceGestione;
            private char? _QuotePrimeLiquidate;
            private int? _NSettimaneQuotaA;
            private int? _NSettimaneQuotaB;
            private decimal? _RMSQuotaA;
            private decimal? _RMSQuotaB;
            private int? _NSettimane707;

            #endregion private properties

            #region public properties
            public DateTime? DecorrenzaOriginariaPensione { get { return _DecorrenzaOriginariaPensione; } set { _DecorrenzaOriginariaPensione = value; } }
            public long? CodiceGestione { get { return _CodiceGestione; } set { _CodiceGestione = value; } }
            public char? QuotePrimeLiquidate { get { return _QuotePrimeLiquidate; } set { _QuotePrimeLiquidate = value; } }
            public int? NSettimaneQuotaA { get { return _NSettimaneQuotaA; } set { _NSettimaneQuotaA = value; } }
            public int? NSettimaneQuotaB { get { return _NSettimaneQuotaB; } set { _NSettimaneQuotaB = value; } }
            public decimal? RMSQuotaA { get { return _RMSQuotaA; } set { _RMSQuotaA = value; } }
            public decimal? RMSQuotaB { get { return _RMSQuotaB; } set { _RMSQuotaB = value; } }
            public int? Nsettimane707 { get { return _NSettimane707; } set { _NSettimane707 = value; } }

            #endregion public properties
        }

        public class DatiContributiEsteri
        {
            #region private properties
            public long? Id { get { return _Id; } set { _Id = value; } }
            public long? CodiceGestione { get { return _CodiceGestione; } set { _CodiceGestione = value; } }
            public int? Settimane { get { return _Settimane; } set { _Settimane = value; } }
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }

            #endregion private properties

            #region private properties
            private long? _Id;
            private long? _CodiceGestione;
            private int? _Settimane;
            private DateTime? _Decorrenza;
            #endregion private properties
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

        public class DatiIstituzioniEstere
        {
            #region public properties
            public string CodiceStatoEE { get; set; }
            public string CodiceIstituzione { get; set; }
            public int ContributiEEDecorrenzaOriginaria { get; set; }
            public int ContributiEEDiritto { get; set; }
            #endregion public properties
        }
        #endregion nested class

        public enum TipoCalcolo
        {
            Contributivo,
            Retributivo,
            Misto,
            NonValido
        };

    }
}
