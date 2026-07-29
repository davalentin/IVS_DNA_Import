using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using INPS.Pensioni.LiquidazioneAgo.ServiceReferences.AggPec;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Configuration;
using System.Reflection;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneAggiornamentoPECO
    {
        #region public method

        internal static bool GetDatiPECObyNumeroDomanda(GestionePensione.DatiPensione datiPensione, ref csAggiornamentoPECO_AGO dati, out string errore)
        {
            errore = string.Empty;
            try
            {
                dati = new csAggiornamentoPECO_AGO();
                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneFunzioneAVESO92PerUNICARPE", out ctrl);
                if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    dati.PL_Funzione = "G";
                else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
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
                AggiornamentoPECO(ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC"], ConfigurationManager.AppSettings["ChiaveApplicazioneAGGPEC"], ref dati, datiPensione.NDomus.ToString(),
                    out errore);
                if (!String.IsNullOrEmpty(errore) || dati.PL_Return_Code != 0)
                {
                    if (String.IsNullOrEmpty(errore))
                        errore = "Codice di ritorno della posizione richiesta (diverso da 0): " + dati.PL_Return_Code.ToString();
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

        internal static char? GetModalitaLiquidazioneValue(int PL_CODPROV)
        {
            if (PL_CODPROV == 0)
                return null;
            else
                return Convert.ToChar("1");
        }

        internal static bool? GetFlagContributivaValue(string PL_SoloCPNTR)
        {
            return null;
        }

        internal static byte? GetDerogaValue(int CI_Deroga)
        {
            return null;
        }

        internal static void GetDatiTotali(GestionePensione.DatiPensione datiPensione, out DatiTotaliAggPec datiAggPec, out string errori)
        {
            datiAggPec = null;
            errori = string.Empty;
            csAggiornamentoPECO_AGO dati = null;

            GetDatiPECObyNumeroDomanda(datiPensione, ref dati, out errori);

            if (dati != null)
            {
                datiAggPec = new DatiTotaliAggPec();

                if (dati.PL_ContrSolSett != 0 || !Utility.IsDoubleEquals(dati.PL_ContrSolImp, 0))
                    datiAggPec.DatiInpdai = new DatiINPDAI
                    {
                        Anz95 = dati.PL_ContrSolSett,
                        Quota95 = dati.PL_ContrSolImp
                    };

                if (!IsDatiFlatNull(dati))
                {
                    datiAggPec.DatiFlat = new DatiFlat
                    {
                        ImportoLordo = (decimal)dati.PL_Importor,
                        PL_Coeftrasf = (decimal)dati.PL_Coeftrasf
                    };
                }


                if ((Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria)) &&
                    (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)))
                {
                    GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
                    GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

                    if (datiPensioniDatiGenerici == null)
                        datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                    if (!Utility.IsDoubleEquals(dati.PL_Coeftrasf, 0))
                        datiPensioniDatiGenerici.PL_Coeftrasf = (decimal)dati.PL_Coeftrasf;
                    GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioniDatiGenerici);
                }

                List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoGestioneQuotaFondoINPGI = new List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI>();
                GestioneDecodifica.GetCodeGestioneQuotaFondoINPGI(out elencoGestioneQuotaFondoINPGI);

                if (dati.aCONTRIBUTIVA != null)
                {
                    datiAggPec.lContribuzione = new List<DatiContributivi>();
                    datiAggPec.lQuotaFondoIntegrativo = new List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo>();
                    datiAggPec.lDatiContributiviINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI>();

                    for (int i = 0; i < dati.aCONTRIBUTIVA.Count(); i++)
                    {
                        if (!IsContributivaNull(dati.aCONTRIBUTIVA[i]))
                        {
                            string codGestione = dati.aCONTRIBUTIVA[i].PL_Codgest_contr;
                            if (dati.aCONTRIBUTIVA[i].PL_Codgest_contr == "GI")
                            {
                                if (dati.aCONTRIBUTIVA[i].PL_CodQuota_Contr == "F")
                                {
                                    GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI datiContributivoINPGI = new GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI();

                                    //funziona solo se le contributive hanno una sola dec , se dovessero aumentare è da rivedere
                                    datiContributivoINPGI.CodiceGestione = elencoGestioneQuotaFondoINPGI.Where(x => x.TipoQuota == "C").FirstOrDefault().Id;
                                    datiContributivoINPGI.Montante = (decimal?)dati.aCONTRIBUTIVA[i].PL_Montante;
                                    datiContributivoINPGI.Quota = (decimal?)dati.aCONTRIBUTIVA[i].PL_Quotac;
                                    datiContributivoINPGI.Settimane = dati.aCONTRIBUTIVA[i].PL_Settda0196;

                                    datiAggPec.lDatiContributiviINPGI.Add(datiContributivoINPGI);
                                }

                            }
                            else if (!string.IsNullOrEmpty(codGestione) && codGestione.ToUpperInvariant() == "ES")
                            {
                                GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo datiQuotaFondoIntegrativo = new GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo();
                                datiQuotaFondoIntegrativo.CodiceGestione = 1;
                                datiQuotaFondoIntegrativo.Quota = dati.aCONTRIBUTIVA[i].PL_CodQuota_Contr.Trim() != string.Empty ? Convert.ToChar(dati.aCONTRIBUTIVA[i].PL_CodQuota_Contr.Trim().ToUpperInvariant()) : (char?)null;
                                if (datiQuotaFondoIntegrativo.Quota.HasValue && datiQuotaFondoIntegrativo.Quota.Value.ToString().ToUpperInvariant() == "C")
                                {
                                    datiQuotaFondoIntegrativo.ImportoContributivoTotale = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Contrib, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Contrib) : (decimal?)null;
                                    datiQuotaFondoIntegrativo.Montante = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Montante, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Montante) : (decimal?)null;
                                    datiQuotaFondoIntegrativo.NSettimane = dati.aCONTRIBUTIVA[i].PL_Settda0196 != 0 ? dati.aCONTRIBUTIVA[i].PL_Settda0196 : (int?)null;
                                }
                                else if (datiQuotaFondoIntegrativo.Quota.HasValue && datiQuotaFondoIntegrativo.Quota.Value.ToString().ToUpperInvariant() == "D")
                                {
                                    datiQuotaFondoIntegrativo.ImportoContribTotaleQuotaD = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Contrib, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Contrib) : (decimal?)null;
                                    datiQuotaFondoIntegrativo.MontanteQuotaD = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Montante, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Montante) : (decimal?)null;
                                    datiQuotaFondoIntegrativo.NSettimaneQuotaD = dati.aCONTRIBUTIVA[i].PL_Settda0196 != 0 ? dati.aCONTRIBUTIVA[i].PL_Settda0196 : (int?)null;
                                }
                                datiQuotaFondoIntegrativo.PL_Quotac = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Quotac, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Quotac) : (decimal?)null;
                                datiAggPec.lQuotaFondoIntegrativo.Add(datiQuotaFondoIntegrativo);
                            }
                            else
                            {
                                DatiContributivi datiContrib = new DatiContributivi();

                                List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                                GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);
                                if (elencoCodeGestioneCalcoloContributivo != null && elencoCodeGestioneCalcoloContributivo.Count > 0)
                                {
                                    GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP == dati.aCONTRIBUTIVA[i].PL_Codgest_contr && !x.IsFondo);
                                    if (codeGestioneCalcoloContributivo != null)
                                        datiContrib.CodGestione = codeGestioneCalcoloContributivo.Id;
                                }

                                datiContrib.Quota = dati.aCONTRIBUTIVA[i].PL_CodQuota_Contr.Trim() != string.Empty ? Convert.ToChar(dati.aCONTRIBUTIVA[i].PL_CodQuota_Contr.Trim().ToUpperInvariant()) : (char?)null;

                                if (datiContrib.Quota.HasValue && datiContrib.Quota.Value.ToString().ToUpperInvariant() == "C")
                                {
                                    datiContrib.ImportoContributivo = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Contrib, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Contrib) : (decimal?)null;
                                    datiContrib.MontanteContributivo = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Montante, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Montante) : (decimal?)null;
                                    datiContrib.Settimane = dati.aCONTRIBUTIVA[i].PL_Settda0196 != 0 ? dati.aCONTRIBUTIVA[i].PL_Settda0196 : (int?)null;
                                }
                                else if (datiContrib.Quota.HasValue && datiContrib.Quota.Value.ToString().ToUpperInvariant() == "D")
                                {
                                    datiContrib.ImportoContributivoQuotaD = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Contrib, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Contrib) : (decimal?)null;
                                    datiContrib.MontanteContributivoQuotaD = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Montante, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Montante) : (decimal?)null;
                                    datiContrib.SettimaneQuotaD = dati.aCONTRIBUTIVA[i].PL_Settda0196 != 0 ? dati.aCONTRIBUTIVA[i].PL_Settda0196 : (int?)null;
                                }

                                datiContrib.PL_Quotac = !Utility.IsDoubleEquals(dati.aCONTRIBUTIVA[i].PL_Quotac, 0.0) ? Convert.ToDecimal(dati.aCONTRIBUTIVA[i].PL_Quotac) : (decimal?)null;
                                datiAggPec.lContribuzione.Add(datiContrib);
                            }
                        }
                    }
                    if (datiAggPec.lContribuzione.Count == 0)
                        datiAggPec.lContribuzione = null;
                    if (datiAggPec.lQuotaFondoIntegrativo.Count == 0)
                        datiAggPec.lQuotaFondoIntegrativo = null;
                    if (datiAggPec.lDatiContributiviINPGI.Count == 0)
                        datiAggPec.lDatiContributiviINPGI = null;
                }
                if (dati.aRETRIBUTIVA != null)
                {
                    datiAggPec.lRetribuzione = new List<DatiRetributivi>();
                    datiAggPec.lDatiRetributiviINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI>();

                    for (int i = 0; i < dati.aRETRIBUTIVA.Count(); i++)
                    {
                        if (!IsRetributivaNull(dati.aRETRIBUTIVA[i]))
                        {
                            DatiRetributivi datiRetrib = new DatiRetributivi();

                            if (dati.aRETRIBUTIVA[i].PL_Codgest_retr == "GI")
                            {
                                GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI datiRetributivoINPGI = new GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI();

                                datiRetributivoINPGI.CodiceGestione = elencoGestioneQuotaFondoINPGI.Where(x => x.TraduzioneSuGP == dati.aRETRIBUTIVA[i].PL_Codquota2).FirstOrDefault() == null ? (long?)null : elencoGestioneQuotaFondoINPGI.Where(x => x.TraduzioneSuGP == dati.aRETRIBUTIVA[i].PL_Codquota2).FirstOrDefault().Id;

                                datiRetributivoINPGI.Settimane = dati.aRETRIBUTIVA[i].PL_Anzcon1;
                                datiRetributivoINPGI.ImportoCalcolato = (decimal?)dati.aRETRIBUTIVA[i].PL_Quotar;
                                datiRetributivoINPGI.RetribuzioneMediaSettimanale = (decimal?)dati.aRETRIBUTIVA[i].PL_Rms;

                                if (datiRetributivoINPGI.CodiceGestione != null)
                                    datiAggPec.lDatiRetributiviINPGI.Add(datiRetributivoINPGI);

                            }
                            else
                            {

                                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                                GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);
                                if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                {
                                    GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == dati.aRETRIBUTIVA[i].PL_Codgest_retr.Trim() && !x.IsFondo);
                                    if (codeGestioneCalcoloRetributivo != null)
                                        datiRetrib.CodGestione = codeGestioneCalcoloRetributivo.Id;
                                }

                                //G.Arru - poichè PL_Codquota2 è presente solo per domande DAI evitiamo di fare la query su CtrlDecorrenzaRetrExINPDAI quando non
                                //è presente il dato e quindi non può essere una DAI.
                                if (dati.aRETRIBUTIVA[i].PL_Codquota2 != null && !string.IsNullOrEmpty(dati.aRETRIBUTIVA[i].PL_Codquota2.Trim()))
                                {
                                    List<CtrlDecorrenzaRetrExINPDAI> lstCtrlDecorrenzaRetrExINPDAI = null;
                                    GestioneCtrlDecorrenzaRetrExINPDAI.GetCtrlDecorrenzaRetrExINPDAI(out lstCtrlDecorrenzaRetrExINPDAI);
                                    if (lstCtrlDecorrenzaRetrExINPDAI != null && lstCtrlDecorrenzaRetrExINPDAI.Count > 0)
                                    {

                                        CtrlDecorrenzaRetrExINPDAI ctrlDecRetrExINPDAI = lstCtrlDecorrenzaRetrExINPDAI.Find(x => x.CodiceDecorrenza.ToString() == dati.aRETRIBUTIVA[i].PL_Codquota2.Trim());
                                        if (ctrlDecRetrExINPDAI != null)
                                            datiRetrib.CodiceTipoQuota = ctrlDecRetrExINPDAI.TipoQuota;
                                    }
                                }

                                datiRetrib.Quota = dati.aRETRIBUTIVA[i].PL_Codquota.Trim() != string.Empty ? Convert.ToChar(dati.aRETRIBUTIVA[i].PL_Codquota.Trim().ToUpperInvariant()) : (char?)null;
                                if (datiRetrib.Quota.HasValue && datiRetrib.Quota.Value.ToString().ToUpperInvariant() == "A")
                                {
                                    datiRetrib.SettimaneA = dati.aRETRIBUTIVA[i].PL_Anzcon1;
                                    datiRetrib.RMSQuotaA = !Utility.IsDoubleEquals(dati.aRETRIBUTIVA[i].PL_Rms, 0.0) ? Convert.ToDecimal(dati.aRETRIBUTIVA[i].PL_Rms) : (decimal?)null;
                                    if (!((datiAggPec.lContribuzione == null || !datiAggPec.lContribuzione.Exists(x => x.Quota.GetValueOrDefault() == 'D')) && dati.PL_SoloCPNTR == "0"))
                                        datiRetrib.NSettimane707 = dati.aRETRIBUTIVA[i].PL_Anzcon707 != 0 ? dati.aRETRIBUTIVA[i].PL_Anzcon707 : (int?)null;
                                }
                                else
                                    if (datiRetrib.Quota.HasValue && datiRetrib.Quota.Value.ToString().ToUpperInvariant() == "B")
                                {
                                    datiRetrib.SettimaneB = dati.aRETRIBUTIVA[i].PL_Anzcon1;
                                    datiRetrib.RMSQuotaB = !Utility.IsDoubleEquals(dati.aRETRIBUTIVA[i].PL_Rms, 0.0) ? Convert.ToDecimal(dati.aRETRIBUTIVA[i].PL_Rms) : (decimal?)null;
                                    if (!((datiAggPec.lContribuzione == null || !datiAggPec.lContribuzione.Exists(x => x.Quota.GetValueOrDefault() == 'D')) && dati.PL_SoloCPNTR == "0"))
                                        datiRetrib.NSettimane707 = dati.aRETRIBUTIVA[i].PL_Anzcon707 != 0 ? dati.aRETRIBUTIVA[i].PL_Anzcon707 : (int?)null;

                                }

                                if (datiRetrib.SettimaneA.GetValueOrDefault() == 1 && (datiRetrib.RMSQuotaA.GetValueOrDefault() == 0.01M || datiRetrib.RMSQuotaA.GetValueOrDefault() == 1M))
                                    datiRetrib.RMSQuotaA = 0.004M;

                                datiRetrib.PL_Quotar = !Utility.IsDoubleEquals(dati.aRETRIBUTIVA[i].PL_Quotar, 0.0) ? Convert.ToDecimal(dati.aRETRIBUTIVA[i].PL_Quotar) : (decimal?)null;
                                datiRetrib.PL_Quotar707 = !Utility.IsDoubleEquals(dati.aRETRIBUTIVA[i].PL_Quotar707, 0.0) ? Convert.ToDecimal(dati.aRETRIBUTIVA[i].PL_Quotar707) : (decimal?)null; ;

                                datiAggPec.lRetribuzione.Add(datiRetrib);
                            }
                        }
                    }
                    if (datiAggPec.lRetribuzione.Count == 0)
                        datiAggPec.lRetribuzione = null;
                    if (datiAggPec.lDatiRetributiviINPGI.Count == 0)
                        datiAggPec.lDatiRetributiviINPGI = null;
                }
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

        private static void AggiornamentoPECO(string ProgrChiamante, string AppChiamante, ref csAggiornamentoPECO_AGO dati, string numDomanda, out string errori)
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AGG_PEC, method Aggiornamento_PECO | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AGG_PEC, method Aggiornamento_PECO | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AGG_PEC, method Aggiornamento_PECO | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AGG_PEC, method Aggiornamento_PECO: {0}", Utility.GetMessageFromException(Ex));
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
                    GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAggPec, Utility.MetodoServizio.Aggiornamento_PECO_AGO, Utility.SOAPLogDirection.OUT, numDomanda, guid, dati.PL_Funzione);
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static bool IsContributivaNull(CONTRIBUTIVA contrib)
        {
            if (contrib.PL_Codgest_contr.Trim() == string.Empty && Utility.IsDoubleEquals(contrib.PL_Contrib, 0.0) &&
                Utility.IsDoubleEquals(contrib.PL_Montante, 0.0) && contrib.PL_Settda0196 == 0)
                return true;
            else
                return false;
        }

        private static bool IsRetributivaNull(RETRIBUTIVA retrib)
        {
            if (retrib.PL_Anzcon1 == 0 && retrib.PL_Codgest_retr.Trim() == string.Empty &&
                retrib.PL_Codquota.Trim() == string.Empty && Utility.IsDoubleEquals(retrib.PL_Rms, 0.0))
                return true;
            else
                return false;
        }

        private static bool IsDatiFlatNull(csAggiornamentoPECO_AGO dati)
        {
            if (dati != null && (!Utility.IsDoubleEquals(dati.PL_Importor, 0) || !Utility.IsDoubleEquals(dati.PL_Coeftrasf, 0)))
                return false;

            return true;
        }

        #endregion private method

        #region nested class

        public class DatiTotaliAggPec
        {
            #region private properties

            private List<DatiContributivi> _lContribuzione;
            private List<DatiRetributivi> _lRetribuzione;
            private DatiControllo _DatiControllo;
            private DatiINPDAI _DatiInpdai;
            private DatiFlat _DatiFlat;
            private List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> _lQuotaFondoIntegrativo;
            private List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> _lDatiRetributiviINPGI;
            private List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> _lDatiContributiviINPGI;

            #endregion private properties

            #region public properties
            public List<DatiContributivi> lContribuzione { get { return _lContribuzione; } set { _lContribuzione = value; } }
            public List<DatiRetributivi> lRetribuzione { get { return _lRetribuzione; } set { _lRetribuzione = value; } }
            public DatiControllo DatiControllo { get { return _DatiControllo; } set { _DatiControllo = value; } }
            public DatiINPDAI DatiInpdai { get { return _DatiInpdai; } set { _DatiInpdai = value; } }
            public DatiFlat DatiFlat { get { return _DatiFlat; } set { _DatiFlat = value; } }
            public List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> lQuotaFondoIntegrativo { get { return _lQuotaFondoIntegrativo; } set { _lQuotaFondoIntegrativo = value; } }
            public List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> lDatiRetributiviINPGI { get { return _lDatiRetributiviINPGI; } set { _lDatiRetributiviINPGI = value; } }
            public List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> lDatiContributiviINPGI { get { return _lDatiContributiviINPGI; } set { _lDatiContributiviINPGI = value; } }

            #endregion public properties

            public bool IsNull()
            {
                if ((lContribuzione == null || lContribuzione.Count == 0) && (lRetribuzione == null || lRetribuzione.Count == 0) && (DatiControllo == null)
                    && (_DatiInpdai == null) && (_DatiFlat == null) && (lQuotaFondoIntegrativo == null || lQuotaFondoIntegrativo.Count == 0) 
                    && (lDatiContributiviINPGI == null || lDatiContributiviINPGI.Count == 0) && (lDatiRetributiviINPGI == null || lDatiRetributiviINPGI.Count == 0))
                    return true;
                return false;
            }
        }

        public class DatiContributivi
        {
            #region private properties

            private char? _Quota;
            private long? _CodGestione;
            private decimal? _MontanteContributivo;
            private decimal? _ImportoContributivo;
            private int? _Settimane;
            private int? _SettimaneQuotaD;
            private decimal? _MontanteContributivoQuotaD;
            private decimal? _ImportoContributivoQuotaD;
            private decimal? _PL_Quotac;
            private DateTime? _DecorrenzaCalcoloContibutivo;
            #endregion private properties

            #region public properties
            public char? Quota { get { return _Quota; } set { _Quota = value; } }
            public long? CodGestione { get { return _CodGestione; } set { _CodGestione = value; } }
            public decimal? MontanteContributivo { get { return _MontanteContributivo; } set { _MontanteContributivo = value; } }
            public decimal? ImportoContributivo { get { return _ImportoContributivo; } set { _ImportoContributivo = value; } }
            public int? Settimane { get { return _Settimane; } set { _Settimane = value; } }
            public int? SettimaneQuotaD { get { return _SettimaneQuotaD; } set { _SettimaneQuotaD = value; } }
            public decimal? MontanteContributivoQuotaD { get { return _MontanteContributivoQuotaD; } set { _MontanteContributivoQuotaD = value; } }
            public decimal? ImportoContributivoQuotaD { get { return _ImportoContributivoQuotaD; } set { _ImportoContributivoQuotaD = value; } }
            public decimal? PL_Quotac { get { return _PL_Quotac; } set { _PL_Quotac = value; } }
            public DateTime? DecorrenzaCalcoloContibutivo { get { return _DecorrenzaCalcoloContibutivo; } set { _DecorrenzaCalcoloContibutivo = value; } }

            #endregion public properties

            public bool IsQuotaDL214Presente()
            {
                if (this._SettimaneQuotaD.HasValue || this._MontanteContributivoQuotaD.HasValue || this._ImportoContributivoQuotaD.HasValue)
                    return true;

                return false;
            }
        }

        public class DatiRetributivi
        {
            #region private properties
            private char? _Quota;
            private long? _CodGestione;
            private int? _SettimaneA;
            private int? _SettimaneB;
            private decimal? _RMSQuotaA;
            private decimal? _RMSQuotaB;
            private DateTime? _Decorrenza;
            private int? _NSettimane707;

            private string _CodiceTipoQuota;
            private decimal? _PL_Quotar;
            private decimal? _PL_Quotar707;
            private decimal? _RMS;
            #endregion private properties

            #region public properties
            public char? Quota { get { return _Quota; } set { _Quota = value; } }
            public long? CodGestione { get { return _CodGestione; } set { _CodGestione = value; } }
            public int? SettimaneA { get { return _SettimaneA; } set { _SettimaneA = value; } }
            public int? SettimaneB { get { return _SettimaneB; } set { _SettimaneB = value; } }
            public decimal? RMSQuotaA { get { return _RMSQuotaA; } set { _RMSQuotaA = value; } }
            public decimal? RMSQuotaB { get { return _RMSQuotaB; } set { _RMSQuotaB = value; } }
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public string CodiceTipoQuota { get { return _CodiceTipoQuota; } set { _CodiceTipoQuota = value; } }
            public int? NSettimane707 { get { return _NSettimane707; } set { _NSettimane707 = value; } }
            public decimal? PL_Quotar { get { return _PL_Quotar; } set { _PL_Quotar = value; } }
            public decimal? PL_Quotar707 { get { return _PL_Quotar707; } set { _PL_Quotar707 = value; } }
            public decimal? RMS { get { return _RMS; } set { _RMS = value; } }
            public decimal? RMSExCombattente { get; set; }
            public int? NSettAnzianitaVV { get; set; }
            public int? NSettimaneExCombattente { get; set; }
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

        public class DatiINPDAI
        {
            public int Anz95 { get; set; }
            public double Quota95 { get; set; }
        }

        public class DatiFlat
        {
            public decimal? ImportoLordo { get; set; }
            public decimal? PL_Coeftrasf { get; set; }
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
