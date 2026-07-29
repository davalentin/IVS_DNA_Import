using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneFs.ServiceReferences.AMGFelpe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneAMGFelpe
    {
        public static bool GetDatiPECO_AMG(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            if (!GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errore))
                return false;

            return true;
        }

        private static bool GetDatiPECO_AMGbyNDomus(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_Fondi_AMG dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_Fondi_AMG();
                dati.A_Funzione = "L";
                dati.A_Numdomus = Convert.ToString(datiPensione.NDomus);
                Aggiornamento_PECO_Fondi_AMG(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC_AMG"], ref dati, out errore);
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
                errore = ex.Message;
                return false;
            }
        }

        private static void Aggiornamento_PECO_Fondi_AMG(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_Fondi_AMG dati, out string errori)
        {
            errori = string.Empty;
            GestionePecoServiceClient proxy = new GestionePecoServiceClient();
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAMGFelpe, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG, Utility.SOAPLogDirection.IN, dati.A_Numdomus, guid);

                    proxy.Aggiornamento_PECO_Fondi_AMG(ProgrChiamante, AppChiamante, ref dati);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = exception.Message;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
                {
                    errori = "Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC_FS, method Aggiornamento_PECO_FS";
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = "Puntamento errato al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = "Errore di comunicazione con il servizio AGG_PEC_FS, method Aggiornamento_PECO_FS";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = "Errore nella chiamata al servizio AGG_PEC_FS, method Aggiornamento_PECO_FS: " + Ex.Message;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAMGFelpe, Utility.MetodoServizio.Aggiornamento_PECO_Fondi_AMG, Utility.SOAPLogDirection.OUT, dati.A_Numdomus, guid);

                    Utility.CloseClient(proxy);
                }
            }
        }

        internal static void RecuperaDatiTotaliAMGFelpe(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione,
            out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtile, out GestioneAggiornamentoPECO.DatiContributivi datiContributivi)
        {
            RecuperaDatiTotaliINPDAP(dati, datiPensione, out listaDatiServizioUtile, out datiContributivi);
        }

        internal static void RecuperaDatiTotaliINPDAP(csAggiornamentoPECO_Fondi_AMG dati, GestionePensione.DatiPensione datiPensione,
            out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtile, out GestioneAggiornamentoPECO.DatiContributivi datiContributivi)
        {
            datiContributivi = null;
            listaDatiServizioUtile = null;

            if (dati.aRETRIBUTIVE_AMG != null && dati.aRETRIBUTIVE_AMG.Length > 0)
            {
                foreach (A_RETRIBUTIVE retr in dati.aRETRIBUTIVE_AMG)
                {
                    GestioneDatiServizioUtileINPDAP.ServizioUtile servizioUtile = null;

                    if (!string.IsNullOrEmpty(retr.A_CodQuota.Trim()) && !string.IsNullOrEmpty(retr.A_CodGest_Retr.Trim()))
                    {
                        listaDatiServizioUtile = new List<GestioneDatiServizioUtileINPDAP.ServizioUtile>();

                        // Quota A - Dati al 31/12/92
                        if (new List<string>() { "I", "K", "J", "Y", "L" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "A")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                string strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "A";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string>() { "I1", "K1", "J1", "Y1", "L1" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                string strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile.Retribuzione = Convert.ToDecimal(retr.A_Rms);
                            servizioUtile.Quota = "B1";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string>() { "I2", "K2", "J2", "Y2", "L2" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                string strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile.Quota = "B2";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string>() { "I3", "K3", "J3", "Y3", "L3" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                string strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile.Quota = "B3";
                        }
                        // Quota B - Dati al 31/12/94
                        else if (new List<string>() { "I4", "K4", "J4", "Y4", "L4" }.Contains(retr.A_CodGest_Retr.Trim()) && retr.A_CodQuota.Trim() == "B")
                        {
                            servizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();

                            if (retr.A_Anzconr != 0)
                            {
                                string strApp = retr.A_Anzconr.ToString().PadLeft(6, '0');
                                servizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(strApp.Substring(0, 2));
                                servizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableByte(strApp.Substring(2, 2));
                                servizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableByte(strApp.Substring(4, 2));
                            }

                            servizioUtile.Quota = "B4";
                        }

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
                        if (!Utility.IsDoubleEquals(contr.A_Contrib, 0.0) || !Utility.IsDoubleEquals(contr.A_Montante, 0.0) || contr.A_Anzconc != 0)
                        {
                            IsContribNull = false;
                            if (datiContributivi == null)
                                datiContributivi = new GestioneAggiornamentoPECO.DatiContributivi();
                            string strApp = string.Empty;
                            switch (contr.A_CodQuota_Contr.Trim().ToUpperInvariant())
                            {
                                case "C":
                                    datiContributivi.ImportoContributivoTotale = Convert.ToDecimal(contr.A_Contrib);
                                    datiContributivi.Montante = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiContributivi.Settimane = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() / 6.923));
                                    break;
                                case "D":
                                    datiContributivi.ImportoContribTotaleQuotaDL214 = Convert.ToDecimal(contr.A_Contrib);
                                    datiContributivi.MontanteQuotaDL214 = Convert.ToDecimal(contr.A_Montante);
                                    strApp = contr.A_Anzconc.ToString().PadLeft(6, '0');
                                    datiContributivi.NSettimaneQuotaDL214 = (int)Math.Ceiling((Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() * 52) +
                                        (Utility.StringToNullableInt(strApp.Substring(2, 2)).GetValueOrDefault() * 4.333) +
                                        (Utility.StringToNullableInt(strApp.Substring(0, 2)).GetValueOrDefault() / 6.923));
                                    break;
                            }
                        }

                    }
                }

                if (IsContribNull)
                    datiContributivi = null;
            }
        }
    }
}
