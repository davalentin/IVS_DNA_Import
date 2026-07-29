using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneAgo.Entity;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;
using INPS.Pensioni.LiquidazioneAgo.Data;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneContrib
    {
        public static void GetTipoCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, out TipoCalcolo tipoCalcolo)
        {
            tipoCalcolo = TipoCalcolo.NonValido;
            if (datiPensione == null || !datiPensione.TipoCalcolo.HasValue)
                return;

            switch (Utility.GetTipoCalcolo(datiPensione))
            {
                case Utility.TipoCalcolo.Contributivo:
                    tipoCalcolo = TipoCalcolo.Contributivo;
                    break;
                case Utility.TipoCalcolo.Retributivo:
                    tipoCalcolo = TipoCalcolo.Retributivo;
                    break;
                case Utility.TipoCalcolo.Misto:
                    tipoCalcolo = TipoCalcolo.Misto;
                    break;
                case Utility.TipoCalcolo.MistoL214:
                    tipoCalcolo = TipoCalcolo.MistoL214;
                    break;
                case Utility.TipoCalcolo.RetributivoComma707:
                    tipoCalcolo = TipoCalcolo.RetributivoComma707;
                    break;
            }
        }

        public static void GetDatiCalcoloByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, bool isRiaperturaDomanda,
            out DatiCalcolo datiCalcolo, out string messaggioVideo, out bool IsDataFromDb)
        {
            datiCalcolo = null;
            messaggioVideo = string.Empty;
            IsDataFromDb = false;

            GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec = new GestioneAggiornamentoPECO.DatiTotaliAggPec();

            //ENG- Memo 68/2022 aggiornato al 12/03/2025
            bool isDomandaVOPGIOrIOPGIAbilitazioneModificheMemoINPGI_20250312 = false;
            if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria)) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneModificheMemoINPGI_20250312 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20250312 ", out ctrlAbilitazioneModificheMemoINPGI_20250312);
                if (ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI")
                    isDomandaVOPGIOrIOPGIAbilitazioneModificheMemoINPGI_20250312 = true;
            }

            if (contenitore.DatiPensione != null)
            {
                //Se non sono presenti i dati al database richiamo il metodo in modo che se fai elimina e rientri, ti ritrovi comunque i dati
                if (Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) != Utility.TipoUnicarpe.Automatica &&
                    Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) && !(isDomandaVOPGIOrIOPGIAbilitazioneModificheMemoINPGI_20250312 == true) &&
                    contenitore.ListaDatiContributivi == null && contenitore.ListaDatiRetributivi == null)
                {
                    String codCategoria = contenitore.DatiPensione.GetCodCategoria();
                    int codCategoriaInt = int.Parse(codCategoria);

                    GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;

                    string chiavePensione = codCategoriaInt.ToString().PadLeft(3, '0') + contenitore.DatiPensione.CodiceSede.ToString().PadLeft(4, '0') + contenitore.DatiPensione.NCertificato.GetValueOrDefault().ToString().PadLeft(8, '0');
                    List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
                    List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
                    GestioneDatiPensioni.GetDatiTGP2ByChiavePensione(contenitore.DatiPensione.NDomus, chiavePensione, contenitore.DatiPensione, out ldatiRetributivi, out ldatiContributivi,
                        ref datiGenericiAgoCi, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                        return;

                    datiAggPec = new GestioneAggiornamentoPECO.DatiTotaliAggPec();
                    datiAggPec.lContribuzione = MappingDatiContributiviFromBLToView(ref contenitoreDecodifica, ref contenitore, ldatiContributivi, contenitore.DatiPensione);
                    datiAggPec.lRetribuzione = MappingDatiRetributiviFromBLToView(ldatiRetributivi);

                    if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                    {
                        if (datiGenericiAgoCi != null)
                        {
                            datiAggPec.DatiInpdai = new GestioneAggiornamentoPECO.DatiINPDAI();
                            datiAggPec.DatiInpdai.Anz95 = datiGenericiAgoCi.AnzAl95.HasValue ? (int)datiGenericiAgoCi.AnzAl95.Value : 0;
                            datiAggPec.DatiInpdai.Quota95 = datiGenericiAgoCi.QuotaAl95.HasValue ? (double)datiGenericiAgoCi.QuotaAl95 : 0;
                        }
                    }
                }
                else if (Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica && contenitore.ListaDatiContributivi == null && contenitore.ListaDatiRetributivi == null) // se non è presente alcun dato contr e retr sul db invoco il service
                {
                    GestioneAggiornamentoPECO.GetDatiTotali(contenitore.DatiPensione, out datiAggPec, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                        return;
                }
                else
                {
                    IsDataFromDb = true;
                    if (contenitore.ListaDatiContributivi != null)
                    {
                        datiAggPec.lContribuzione = new List<GestioneAggiornamentoPECO.DatiContributivi>();
                        GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneK = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null ? (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "K") ? contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "K") : null) : null;
                        GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneL = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null ? (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "L") ? contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "L") : null) : null;
                        foreach (GestioneCalcolo.DatiCalcoloContributivo calContr in contenitore.ListaDatiContributivi)
                        {
                            GestioneAggiornamentoPECO.DatiContributivi datiContr = new GestioneAggiornamentoPECO.DatiContributivi();
                            datiContr.CodGestione = calContr.CodiceGestione;
                            var ctrlSettimane = Utility.IsDomandaAUT(contenitore.DatiPensione) ? (calContr.NSettimane.HasValue && calContr.NSettimane.Value != 0 ? true : false) : calContr.NSettimane.HasValue;

                            if ((Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) &&
                                decGestioneL != null && decGestioneL.Id == calContr.CodiceGestione)
                                || calContr.ImportoContributivoTotale.HasValue || calContr.Montante.HasValue || ctrlSettimane)
                            {
                                if ((decGestioneK == null || calContr.CodiceGestione != decGestioneK.Id) &&
                                    (decGestioneL == null || !Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || !Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) && calContr.CodiceGestione != decGestioneL.Id)))
                                    datiContr.Quota = 'C';
                                datiContr.ImportoContributivo = calContr.ImportoContributivoTotale;
                                datiContr.MontanteContributivo = calContr.Montante;
                                datiContr.Settimane = calContr.NSettimane;
                            }
                            else if (calContr.ImportoContribTotaleQuotaDL214.HasValue || calContr.MontanteQuotaDL214.HasValue || calContr.NSettimaneQuotaDL214.HasValue)
                            {
                                datiContr.Quota = 'D';
                                datiContr.ImportoContributivoQuotaD = calContr.ImportoContribTotaleQuotaDL214;
                                datiContr.MontanteContributivoQuotaD = calContr.MontanteQuotaDL214;
                                datiContr.SettimaneQuotaD = calContr.NSettimaneQuotaDL214;
                            }
                            datiContr.PL_Quotac = calContr.PL_Quotac;
                            if (Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null) datiContr.DecorrenzaCalcoloContibutivo = calContr.DecorrenzaCalcoloContibutivo;
                            datiAggPec.lContribuzione.Add(datiContr);
                        }
                    }
                    if (contenitore.ListaDatiRetributivi != null)
                    {
                        datiAggPec.lRetribuzione = new List<GestioneAggiornamentoPECO.DatiRetributivi>();
                        foreach (GestioneCalcolo.DatiCalcoloRetributivo calcRetr in contenitore.ListaDatiRetributivi)
                        {
                            GestioneAggiornamentoPECO.DatiRetributivi datiRetr = new GestioneAggiornamentoPECO.DatiRetributivi();
                            datiRetr.Quota = calcRetr.QuotePrimeLiquidate;
                            if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                            {
                                datiRetr.SettimaneA = calcRetr.NSettimaneQuotaA;
                                datiRetr.RMSQuotaA = calcRetr.RMSQuotaA;
                            }
                            else if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                            {
                                datiRetr.SettimaneB = calcRetr.NSettimaneQuotaB;
                                datiRetr.RMSQuotaB = calcRetr.RMSQuotaB;
                            }
                            datiRetr.CodGestione = calcRetr.CodiceGestione;
                            datiRetr.Decorrenza = calcRetr.DecorrenzaOriginariaPensione;
                            datiRetr.CodiceTipoQuota = calcRetr.CodiceTipoQuota;
                            datiRetr.NSettimane707 = calcRetr.NSettimane707;
                            datiRetr.PL_Quotar = calcRetr.PL_Quotar;
                            datiRetr.PL_Quotar707 = calcRetr.PL_Quotar707;
                            datiRetr.RMS = calcRetr.RMS;
                            //aggiunti per ripassarli per le ante96
                            datiRetr.NSettAnzianitaVV = calcRetr.NSettAnzianitaVV;
                            datiRetr.NSettimaneExCombattente = calcRetr.NSettimaneExCombattente;
                            datiRetr.RMSExCombattente = calcRetr.RMSExCombattente;
                            datiAggPec.lRetribuzione.Add(datiRetr);
                        }
                    }
                }
                if (datiAggPec != null && !datiAggPec.IsNull())
                {
                    GestioneAggiornamentoPECO.ImpostaDatiControllo(datiAggPec, out messaggioVideo);

                    if (datiAggPec.lRetribuzione != null && datiAggPec.lRetribuzione.Count > 0)
                    {
                        GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                        if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                            datiDanteCausa = contenitore.DatiDanteCausa;

                        List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetrOrdered = null;
                        GestioneControlli.OrdinaDatiRetributivi(contenitore.DatiPensione, datiAggPec.lRetribuzione, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.DecorrenzaOpzione : null,
                            contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, datiDanteCausa, contenitore.DatiControlloFelpe, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI,
                            out datiRetrOrdered);
                        datiAggPec.lRetribuzione = datiRetrOrdered;
                    }

                    if (datiAggPec.lContribuzione != null && datiAggPec.lContribuzione.Count > 0)
                    {
                        List<GestioneAggiornamentoPECO.DatiContributivi> datiContrOrdered = null;
                        GestioneControlli.OrdinaDatiContributivi(contenitore.DatiPensione, ref contenitoreDecodifica, ref contenitore, datiAggPec.lContribuzione, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, contenitore.DatiDanteCausa, out datiContrOrdered);
                        datiAggPec.lContribuzione = datiContrOrdered;
                    }

                    datiCalcolo = new DatiCalcolo(datiAggPec);
                    datiCalcolo.IsUnicarpe = Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica;
                    datiCalcolo.IdPensione = contenitore.DatiPensione.Id;
                    if (contenitore.DatiPensione.TipoCalcolo.HasValue)
                        datiCalcolo.TipoCalcolo = (TipoCalcolo)contenitore.DatiPensione.TipoCalcolo;

                    //ENG - Aggiornamento Memo 68/2022 IOPGI
                    //ENG - Spacchettate SOPGI 
                    if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                        || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                    {
                        if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf != null)
                            datiCalcolo.PL_Coeftrasf = contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf;
                    }

                    if (datiAggPec.lQuotaFondoIntegrativo != null && datiAggPec.lQuotaFondoIntegrativo.Count > 0)
                        contenitore.ListaDatiQuotaFondoIntegrativo = datiAggPec.lQuotaFondoIntegrativo;

                    if (datiAggPec.lDatiContributiviINPGI != null && datiAggPec.lDatiContributiviINPGI.Count > 0)
                        contenitore.ListaDatiContributiviINPGI = datiAggPec.lDatiContributiviINPGI;
                    if (datiAggPec.lDatiRetributiviINPGI != null && datiAggPec.lDatiRetributiviINPGI.Count > 0)
                        contenitore.ListaDatiRetributiviINPGI = datiAggPec.lDatiRetributiviINPGI;
                }
                else
                {
                    datiCalcolo = new DatiCalcolo();
                    datiCalcolo.IsUnicarpe = Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica;
                    datiCalcolo.IdPensione = contenitore.DatiPensione.Id;
                    if (contenitore.DatiPensione.TipoCalcolo.HasValue)
                        datiCalcolo.TipoCalcolo = (TipoCalcolo)contenitore.DatiPensione.TipoCalcolo;
                    else
                        datiCalcolo.TipoCalcolo = TipoCalcolo.NonValido;

                    //ENG - Aggiornamento Memo 68/2022 IOPGI
                    //ENG - Spacchettate SOPGI
                    if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                        || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                    {
                        if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf != null)
                            datiCalcolo.PL_Coeftrasf = contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf;
                    }
                    datiCalcolo.lDatiContributivi = null;
                    datiCalcolo.lDatiRetributivi = null;
                }

                var isAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda);
                if (contenitore.DatiPensione.SbloccaPannelliAnte96.GetValueOrDefault() || (isAnte96 != null && Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                    (datiCalcolo.lDatiRetributivi == null || !datiCalcolo.lDatiRetributivi.Exists(x => x.NSettAnzianitaVV.HasValue && x.NSettAnzianitaVV != 0)) && (datiCalcolo.lDatiContributivi == null || (datiCalcolo.lDatiContributivi != null
                    && (contenitore.DatiIntegrazioneArt11 == null || (!contenitore.DatiIntegrazioneArt11.Decorrenza.HasValue && !contenitore.DatiIntegrazioneArt11.ImportoIVS.HasValue))))))
                {
                    datiCalcolo.SbloccaPannelliAnte96 = true;
                    if (contenitore.DatiPensione.SbloccaPannelliAnte96 == null)
                    {
                        contenitore.DatiPensione.SbloccaPannelliAnte96 = true;
                        GestionePensione.SalvaPensione(contenitore.DatiPensione);
                    }
                }
            }

            //ENG - Memo 116/2025        
            if (contenitore.DatiPensioniDatiGenerici != null)
            {
                if (datiCalcolo != null && !datiCalcolo.ContributiItalianiEdEsteriAl1295.HasValue && contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295.HasValue)
                    datiCalcolo.ContributiItalianiEdEsteriAl1295 = contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295;
            }
        }

        public static void GetDatiCalcoloStoricoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            out DatiCalcolo datiCalcoloStorico, out string messaggioVideo)
        {
            datiCalcoloStorico = null;
            messaggioVideo = string.Empty;

            GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec = new GestioneAggiornamentoPECO.DatiTotaliAggPec();

            if (contenitore.DatiPensione != null)
            {
                if (contenitore.ListaDatiContributiviStorico != null)
                {
                    datiAggPec.lContribuzione = new List<GestioneAggiornamentoPECO.DatiContributivi>();
                    foreach (GestioneCalcolo.DatiCalcoloContributivo calContr in contenitore.ListaDatiContributiviStorico)
                    {
                        GestioneAggiornamentoPECO.DatiContributivi datiContr = new GestioneAggiornamentoPECO.DatiContributivi();
                        datiContr.CodGestione = calContr.CodiceGestione;
                        var ctrlSettimane = Utility.IsDomandaAUT(contenitore.DatiPensione) ? (calContr.NSettimane.HasValue && calContr.NSettimane.Value != 0 ? true : false) : calContr.NSettimane.HasValue;
                        if (calContr.ImportoContributivoTotale.HasValue || calContr.Montante.HasValue || ctrlSettimane)
                        {
                            datiContr.Quota = 'C';
                            datiContr.ImportoContributivo = calContr.ImportoContributivoTotale;
                            datiContr.MontanteContributivo = calContr.Montante;
                            datiContr.Settimane = calContr.NSettimane;
                        }
                        else if (calContr.ImportoContribTotaleQuotaDL214.HasValue || calContr.MontanteQuotaDL214.HasValue || calContr.NSettimaneQuotaDL214.HasValue)
                        {
                            datiContr.Quota = 'D';
                            datiContr.ImportoContributivoQuotaD = calContr.ImportoContribTotaleQuotaDL214;
                            datiContr.MontanteContributivoQuotaD = calContr.MontanteQuotaDL214;
                            datiContr.SettimaneQuotaD = calContr.NSettimaneQuotaDL214;
                        }
                        datiContr.PL_Quotac = calContr.PL_Quotac;
                        datiAggPec.lContribuzione.Add(datiContr);
                    }
                }
                if (contenitore.ListaDatiRetributiviStorico != null)
                {
                    datiAggPec.lRetribuzione = new List<GestioneAggiornamentoPECO.DatiRetributivi>();
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo calcRetr in contenitore.ListaDatiRetributiviStorico)
                    {
                        GestioneAggiornamentoPECO.DatiRetributivi datiRetr = new GestioneAggiornamentoPECO.DatiRetributivi();
                        datiRetr.Quota = calcRetr.QuotePrimeLiquidate;
                        if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                        {
                            datiRetr.SettimaneA = calcRetr.NSettimaneQuotaA;
                            datiRetr.RMSQuotaA = calcRetr.RMSQuotaA;
                        }
                        else if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                        {
                            datiRetr.SettimaneB = calcRetr.NSettimaneQuotaB;
                            datiRetr.RMSQuotaB = calcRetr.RMSQuotaB;
                        }
                        datiRetr.CodGestione = calcRetr.CodiceGestione;
                        datiRetr.Decorrenza = calcRetr.DecorrenzaOriginariaPensione;
                        datiRetr.CodiceTipoQuota = calcRetr.CodiceTipoQuota;
                        datiRetr.NSettimane707 = calcRetr.NSettimane707;
                        datiRetr.PL_Quotar = calcRetr.PL_Quotar;
                        datiRetr.PL_Quotar707 = calcRetr.PL_Quotar707;
                        datiAggPec.lRetribuzione.Add(datiRetr);
                    }
                }

                if (!datiAggPec.IsNull())
                {
                    GestioneAggiornamentoPECO.ImpostaDatiControllo(datiAggPec, out messaggioVideo);

                    if (datiAggPec.lRetribuzione != null && datiAggPec.lRetribuzione.Count > 0)
                    {
                        GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                        if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                            datiDanteCausa = contenitore.DatiDanteCausa;

                        List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetrOrdered = null;
                        GestioneControlli.OrdinaDatiRetributivi(contenitore.DatiPensione, datiAggPec.lRetribuzione, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.DecorrenzaOpzione : null,
                            contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, datiDanteCausa, contenitore.DatiControlloFelpe, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI,
                            out datiRetrOrdered);
                        datiAggPec.lRetribuzione = datiRetrOrdered;
                    }

                    if (datiAggPec.lContribuzione != null && datiAggPec.lContribuzione.Count > 0)
                    {
                        List<GestioneAggiornamentoPECO.DatiContributivi> datiContrOrdered = null;
                        GestioneControlli.OrdinaDatiContributivi(contenitore.DatiPensione, ref contenitoreDecodifica, ref contenitore, datiAggPec.lContribuzione, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, contenitore.DatiDanteCausa, out datiContrOrdered);
                        datiAggPec.lContribuzione = datiContrOrdered;
                    }

                    datiCalcoloStorico = new DatiCalcolo(datiAggPec);
                    datiCalcoloStorico.IsUnicarpe = Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica;
                    datiCalcoloStorico.IdPensione = contenitore.DatiPensione.Id;
                    if (contenitore.DatiPensione.TipoCalcolo.HasValue)
                        datiCalcoloStorico.TipoCalcolo = (TipoCalcolo)contenitore.DatiPensione.TipoCalcolo;
                }
                else
                {
                    datiCalcoloStorico = new DatiCalcolo();
                    datiCalcoloStorico.IsUnicarpe = Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica;
                    datiCalcoloStorico.IdPensione = contenitore.DatiPensione.Id;
                    if (contenitore.DatiPensione.TipoCalcolo.HasValue)
                        datiCalcoloStorico.TipoCalcolo = (TipoCalcolo)contenitore.DatiPensione.TipoCalcolo;
                    else
                        datiCalcoloStorico.TipoCalcolo = TipoCalcolo.NonValido;
                    datiCalcoloStorico.lDatiContributivi = null;
                    datiCalcoloStorico.lDatiRetributivi = null;
                }

                //ENG - TRF AUTOMATICHE VESO92/ESPA 
                if (Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id) && Utility.IsDomandaAutomatica(contenitore.DatiPensione) && (Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria)))
                    datiCalcoloStorico.ImportoLordoAllaDecorrenza = (contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.GP2BB06.HasValue) ? contenitore.DatiStoricoGP.GP2BB06 : null;
            }
        }

        public static void GetDatiCalcoloENPALSByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiCalcoloENPALS datiCalcoloENPALS)
        {
            datiCalcoloENPALS = new DatiCalcoloENPALS();

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                datiDanteCausa = contenitore.DatiDanteCausa;

            datiCalcoloENPALS.LDatiContributivi = new List<DatiContributiviENPALS>();
            DatiContributiviENPALS datiContributivi = null;
            if (contenitore.DatiCalcoloContributivoENPALS != null)
            {
                datiContributivi = new DatiContributiviENPALS();
                Utility.ValorizzaOggetti(contenitore.DatiCalcoloContributivoENPALS, datiContributivi);
                datiCalcoloENPALS.LDatiContributivi.Add(datiContributivi);
            }

            datiCalcoloENPALS.LDatiRetributivi = new List<DatiRetributiviENPALS>();
            DatiRetributiviENPALS datiRetributivi = null;
            if (contenitore.DatiCalcoloRetributivoENPALS != null)
            {
                if (contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaA.HasValue || contenitore.DatiCalcoloRetributivoENPALS.NTotaleContributiCalcoloQuotaA.HasValue ||
                    contenitore.DatiCalcoloRetributivoENPALS.RMQuotaA.HasValue || contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaA.HasValue)
                {
                    datiRetributivi = new DatiRetributiviENPALS();
                    datiRetributivi.Quota = 'A';
                    datiRetributivi.Periodi = contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaA;
                    datiRetributivi.NTotaleContributiCalcolo = contenitore.DatiCalcoloRetributivoENPALS.NTotaleContributiCalcoloQuotaA;
                    datiRetributivi.RM = contenitore.DatiCalcoloRetributivoENPALS.RMQuotaA;
                    datiRetributivi.Importo = contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaA;
                    datiRetributivi.Giorni707 = contenitore.DatiCalcoloRetributivoENPALS.GiorniQuotaA707;
                    datiRetributivi.Importo707 = contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaA707;
                    datiRetributivi.Decorrenza = contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaA;
                    datiCalcoloENPALS.LDatiRetributivi.Add(datiRetributivi);
                }
                datiRetributivi = null;
                if (contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaB.HasValue || contenitore.DatiCalcoloRetributivoENPALS.NTotaleContributiCalcoloQuotaB.HasValue ||
                    contenitore.DatiCalcoloRetributivoENPALS.RMQuotaB.HasValue || contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaB.HasValue)
                {
                    datiRetributivi = new DatiRetributiviENPALS();
                    datiRetributivi.Quota = 'B';
                    datiRetributivi.Periodi = contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaB;
                    datiRetributivi.NTotaleContributiCalcolo = contenitore.DatiCalcoloRetributivoENPALS.NTotaleContributiCalcoloQuotaB;
                    datiRetributivi.RM = contenitore.DatiCalcoloRetributivoENPALS.RMQuotaB;
                    datiRetributivi.Importo = contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaB;
                    datiRetributivi.Giorni707 = contenitore.DatiCalcoloRetributivoENPALS.GiorniQuotaB707;
                    datiRetributivi.Importo707 = contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaB707;
                    datiRetributivi.Decorrenza = contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaB;
                    datiCalcoloENPALS.LDatiRetributivi.Add(datiRetributivi);
                }

                datiCalcoloENPALS.ImportoProRataTemporis = contenitore.DatiCalcoloRetributivoENPALS.ImportoProRataTemporis;
                datiCalcoloENPALS.ImportoQuotaRetributivaInMisto = contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaRetributivaInMisto;
            }

            if (contenitore.DatiEnpals != null)
            {
                datiCalcoloENPALS.ImportoPensione = contenitore.DatiEnpals.ImportoPensione;
                datiCalcoloENPALS.ImportoPensione707 = contenitore.DatiEnpals.ImportoPensione707;
                datiCalcoloENPALS.ImportoIIS = contenitore.DatiEnpals.ImportoIIS;

                string decorrenza = string.Empty;

                if (!string.IsNullOrEmpty(contenitore.DatiEnpals.DecorrenzaImportoPensione) && contenitore.DatiEnpals.DecorrenzaImportoPensione.Contains('/'))
                    decorrenza = contenitore.DatiEnpals.DecorrenzaImportoPensione;
                else if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue)
                    decorrenza = string.Format("01/{0:00}/{1:0000}", datiDanteCausa.DecorrenzaPensione.Value.Month, datiDanteCausa.DecorrenzaPensione.Value.Year);
                else if (contenitore.DatiPensione.DecorrenzaOriginaria.HasValue)
                    decorrenza = string.Format("01/{0:00}/{1:0000}", contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month, contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year);

                datiCalcoloENPALS.DecorrenzaImportoPensione = decorrenza;
                datiCalcoloENPALS.DecorrenzaImportoIIS = contenitore.DatiEnpals.DecorrenzaImportoIIS;
            }
        }

        #region Ex-INPDAI

        public static void GetDatiCalcoloExInpdaiByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiCalcolo datiCalcolo,
            out DatiExINPDAI datiExINPDAI)
        {
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                datiDanteCausa = contenitore.DatiDanteCausa;

            datiExINPDAI = new DatiExINPDAI();

            if (contenitoreDecodifica.ElencoDecodificaTipoQuota != null && contenitoreDecodifica.ElencoDecodificaTipoQuota.Count > 0)
            {
                datiExINPDAI.DecodificaTipoQuota = contenitoreDecodifica.ElencoDecodificaTipoQuota.Select(x =>
                {
                    var r = new DecodificaTipoQuota { Codice = x.Codice, Decodifica = x.Decodifica };
                    return r;
                }).ToList();
            }
            if (contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI != null && contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI.Count > 0)
            {
                datiExINPDAI.CtrlDecorrenzaRetrExINPDAI = contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI.Select(x =>
                {
                    var r = new Entity.CtrlDecorrenzaRetrExINPDAI();
                    Utility.ValorizzaOggetti(x, r);
                    return r;
                }).ToList();
            }

            datiExINPDAI.IsDataAnzianitaAl95Bloccato = IsAnz95BloccatoForExInpdai(contenitore.DatiPensione);

            datiExINPDAI.IsPrimoRecordRetrGestioneS = IsPrimoRecordRetrGestioneSForExInpdai(contenitore.DatiPensione);

            Utility.DifferenzaDateTime decorrenzaDatiRetributivi = null;

            if (datiCalcolo != null && datiCalcolo.lDatiRetributivi != null && datiCalcolo.lDatiRetributivi.Count > 0)
            {
                foreach (GestioneAggiornamentoPECO.DatiRetributivi datiRetr in datiCalcolo.lDatiRetributivi)
                {
                    if (datiRetr.Decorrenza != null)
                    {
                        decorrenzaDatiRetributivi = new Utility.DifferenzaDateTime(datiRetr.Decorrenza.Value);
                    }
                }
            }

            datiExINPDAI.DecorrenzaCalcoloRetr = GetDecorrenzaCalcoloRetrExInpdai(datiDanteCausa, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiControlloFelpe, contenitore.DatiPensione,
                decorrenzaDatiRetributivi);

            datiExINPDAI.IsContribSolidarietaVisible = !Utility.IsDomandaIDAI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo)));

            if (datiCalcolo != null && (datiCalcolo.Anz95.HasValue || datiCalcolo.Quota95.HasValue))
            {
                //arrivano i dati da UNICARPE
                datiExINPDAI.AnzAl95 = datiCalcolo.Anz95;
                datiExINPDAI.QuotaAl95 = datiCalcolo.Quota95;
            }
            else if (contenitore.DatiPensioniDatiGenerici != null)
            {
                datiExINPDAI.AnzAl95 = contenitore.DatiPensioniDatiGenerici.AnzAl95;
                datiExINPDAI.QuotaAl95 = contenitore.DatiPensioniDatiGenerici.QuotaAl95;
                datiExINPDAI.ImportoAl200312 = contenitore.DatiPensioniDatiGenerici.ImportoAl200312;
            }
        }

        public static void GetDatiCalcoloExInpdaiStoricoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiCalcolo datiCalcolo,
            out DatiExINPDAI datiExINPDAIStorico)
        {
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                datiDanteCausa = contenitore.DatiDanteCausa;

            datiExINPDAIStorico = new DatiExINPDAI();

            if (contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI != null && contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI.Count > 0)
                datiExINPDAIStorico.CtrlDecorrenzaRetrExINPDAI = contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI.Select(x =>
                {
                    var r = new Entity.CtrlDecorrenzaRetrExINPDAI();
                    Utility.ValorizzaOggetti(x, r);
                    return r;
                }).ToList();

            Utility.DifferenzaDateTime decorrenzaDatiRetributivi = null;
            if (datiCalcolo.lDatiRetributivi != null && datiCalcolo.lDatiRetributivi.Count > 0)
            {
                foreach (GestioneAggiornamentoPECO.DatiRetributivi datiRetr in datiCalcolo.lDatiRetributivi)
                {
                    if (datiRetr.Decorrenza.HasValue)
                    {
                        decorrenzaDatiRetributivi = new Utility.DifferenzaDateTime(datiRetr.Decorrenza.Value);
                    }
                }
            }

            datiExINPDAIStorico.DecorrenzaCalcoloRetr = GetDecorrenzaCalcoloRetrExInpdai(datiDanteCausa, contenitore.DatiPensione.DecorrenzaOriginaria, contenitore.DatiControlloFelpe, contenitore.DatiPensione,
                decorrenzaDatiRetributivi);

            datiExINPDAIStorico.IsContribSolidarietaVisible = !Utility.IsDomandaIDAI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo)));

            if (contenitore.DatiStoricoGP != null)
            {
                datiExINPDAIStorico.AnzAl95 = contenitore.DatiStoricoGP.AnzAl95;
                datiExINPDAIStorico.QuotaAl95 = contenitore.DatiStoricoGP.QuotaAl95;
            }
        }

        #region decodifica

        public static void GetListaTipoCalcoloVincenteDAI(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.TipoCalcoloVincenteDAI> listaTipoCalcoloVincenteDAI)
        {
            listaTipoCalcoloVincenteDAI = new List<TipoCalcoloVincenteDAI>();
            List<GestioneDecodifica.DecTipoCalcoloVincenteDAI> listaDecodificaTipoCalcoloVincente = contenitoreDecodifica.ElencoTipoCalcoloVincenteDAI;

            if (listaDecodificaTipoCalcoloVincente != null && listaDecodificaTipoCalcoloVincente.Count > 0)
            {
                foreach (var dec in listaDecodificaTipoCalcoloVincente)
                {
                    TipoCalcoloVincenteDAI tipoCalcoloVincente = new TipoCalcoloVincenteDAI();
                    Utility.ValorizzaOggetti(dec, tipoCalcoloVincente);
                    listaTipoCalcoloVincenteDAI.Add(tipoCalcoloVincente);
                }
            }
        }
        #endregion decodifica

        #endregion Ex-INPDAI

        public static void GetDatiCalcoloVittimeByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, List<GestioneAggiornamentoPECO.DatiRetributivi> lRetributivi,
            List<GestioneAggiornamentoPECO.DatiContributivi> lContributivi, out DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo)
        {
            datiCalcoloVittimeTerrorismo = new DatiCalcoloVittimeTerrorismo();

            if (contenitore.ListaDatiCalcoloVittimeTerrorismo != null && contenitore.ListaDatiCalcoloVittimeTerrorismo.Count > 0)
            {
                if (contenitore.ListaDatiCalcoloVittimeTerrorismo.Exists(x => x.Tipo == 'R'))
                {
                    datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo = new List<DatiRetributiviVittimeTerrorismo>();
                    foreach (GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo calcoloRetributivo in contenitore.ListaDatiCalcoloVittimeTerrorismo.FindAll(x => x.Tipo == 'R'))
                    {
                        DatiRetributiviVittimeTerrorismo datiRetributivi = new DatiRetributiviVittimeTerrorismo();
                        Utility.ValorizzaOggetti(calcoloRetributivo, datiRetributivi);
                        datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo.Add(datiRetributivi);
                    }
                }

                if (contenitore.ListaDatiCalcoloVittimeTerrorismo.Exists(x => x.Tipo == 'C'))
                {
                    datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo = new List<DatiContributiviVittimeTerrorismo>();
                    foreach (GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo calcoloContributivo in contenitore.ListaDatiCalcoloVittimeTerrorismo.FindAll(x => x.Tipo == 'C'))
                    {
                        DatiContributiviVittimeTerrorismo datiContributivi = new DatiContributiviVittimeTerrorismo();
                        Utility.ValorizzaOggetti(calcoloContributivo, datiContributivi);
                        datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo.Add(datiContributivi);
                    }
                }

                if (contenitore.ListaDatiCalcoloVittimeTerrorismo.Exists(x => x.Tipo == 'I'))
                {
                    datiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo = new List<DatiImportoPensioneVittimeTerrorismo>();
                    foreach (GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo importoPensioneDB in contenitore.ListaDatiCalcoloVittimeTerrorismo.FindAll(x => x.Tipo == 'I'))
                    {
                        DatiImportoPensioneVittimeTerrorismo datiImportoPensione = new DatiImportoPensioneVittimeTerrorismo();
                        Utility.ValorizzaOggetti(importoPensioneDB, datiImportoPensione);
                        datiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo.Add(datiImportoPensione);
                    }
                }
            }
            //05/02/2016 mostriamo a video anche i datiCalcolo se presenti.
            if (lRetributivi != null)
            {
                if (datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo == null)
                    datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo = new List<DatiRetributiviVittimeTerrorismo>();
                datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo.InsertRange(0, lRetributivi.Select(x => new DatiRetributiviVittimeTerrorismo
                {
                    CodiceGestioneRetr = x.CodGestione,
                    Quota = x.RMSQuotaA.HasValue ? 'A' : 'B',
                    CodiceTipoQuota = x.CodiceTipoQuota,
                    RMS = x.RMSQuotaA.GetValueOrDefault() + x.RMSQuotaB.GetValueOrDefault(),
                    Settimane = Convert.ToInt32(x.SettimaneA.GetValueOrDefault() + x.SettimaneB.GetValueOrDefault()),
                    IsFromDatiCalcolo = true
                }));
            }
            if (lContributivi != null)
            {
                if (datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo == null)
                    datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo = new List<DatiContributiviVittimeTerrorismo>();

                datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo.InsertRange(0, lContributivi.Select(x => new DatiContributiviVittimeTerrorismo
                {
                    Ammontare = x.ImportoContributivo.GetValueOrDefault() + x.ImportoContributivoQuotaD.GetValueOrDefault(),
                    Montante = x.MontanteContributivo.GetValueOrDefault() + x.MontanteContributivoQuotaD.GetValueOrDefault(),
                    Settimane = Convert.ToInt32(x.Settimane.GetValueOrDefault() + x.SettimaneQuotaD.GetValueOrDefault()),
                    CodiceGestioneContr = x.CodGestione,
                    Quota = x.Quota,
                    IsFromDatiCalcolo = true
                }));
            }
        }

        public static void GetQuotePensioneByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiCalcoloQuotePensione datiCalcoloQuotePensione)
        {
            datiCalcoloQuotePensione = null;

            if (contenitore.ListaQuotePensione != null && contenitore.ListaQuotePensione.Count > 0)
            {
                datiCalcoloQuotePensione = new DatiCalcoloQuotePensione();
                datiCalcoloQuotePensione.LQuotePensione = new List<DatiQuotePensione>();

                foreach (GestioneCalcolo.QuotePensione quotePensione in contenitore.ListaQuotePensione)
                {
                    DatiQuotePensione datiQuotePensione = new DatiQuotePensione();
                    Utility.ValorizzaOggetti(quotePensione, datiQuotePensione);
                    if (contenitore.ListaTrattenuteQuotePensione != null && contenitore.ListaTrattenuteQuotePensione.Count > 0 &&
                        contenitore.ListaTrattenuteQuotePensione.Any(x => x.EnteGestioneFondoQuote == quotePensione.EnteGestioneFondo))
                    {
                        datiQuotePensione.ListaTrattenute = new List<DatiQuotePensione.DatiTrattenute>();
                        foreach (GestioneCalcolo.TrattenuteQuotePensione trattenute in contenitore.ListaTrattenuteQuotePensione.FindAll(x => x.EnteGestioneFondoQuote == quotePensione.EnteGestioneFondo))
                        {
                            DatiQuotePensione.DatiTrattenute datiTrattenute = new DatiQuotePensione.DatiTrattenute();
                            Utility.ValorizzaOggetti(trattenute, datiTrattenute);
                            datiQuotePensione.ListaTrattenute.Add(datiTrattenute);
                        }
                    }
                    datiCalcoloQuotePensione.LQuotePensione.Add(datiQuotePensione);
                }
            }
            //ENG - MEMO 74_2023
            if (contenitore.DatiPensioniDatiGenerici != null)
            {
                if (datiCalcoloQuotePensione != null && !datiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295.HasValue && contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295.HasValue)
                    datiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295 = contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295;
            }
        }

        public static void GetQuoteMiglioramentiContrattualiByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiCalcoloQuoteMiglioramentiContrattuali datiCalcoloQuoteMiglioramentiContrattuali)
        {
            datiCalcoloQuoteMiglioramentiContrattuali = null;

            if (contenitore.ListaQuoteMiglioramentiContrattuali != null && contenitore.ListaQuoteMiglioramentiContrattuali.Count > 0)
            {
                datiCalcoloQuoteMiglioramentiContrattuali = new DatiCalcoloQuoteMiglioramentiContrattuali();
                datiCalcoloQuoteMiglioramentiContrattuali.LQuoteMiglioramentiContrattuali = new List<DatiQuoteMiglioramentiContrattuali>();

                foreach (GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali quoteMiglioramentiContrattuali in contenitore.ListaQuoteMiglioramentiContrattuali)
                {
                    DatiQuoteMiglioramentiContrattuali datiQuoteMiglioramentiContrattuali = new DatiQuoteMiglioramentiContrattuali();
                    Utility.ValorizzaOggetti(quoteMiglioramentiContrattuali, datiQuoteMiglioramentiContrattuali);

                    datiCalcoloQuoteMiglioramentiContrattuali.LQuoteMiglioramentiContrattuali.Add(datiQuoteMiglioramentiContrattuali);
                }
            }
        }

        public static void GetQuotePensioneStoricoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiCalcoloQuotePensione datiCalcoloQuotePensioneStorico)
        {
            datiCalcoloQuotePensioneStorico = null;

            if (contenitore.ListaQuotePensioneStorico != null && contenitore.ListaQuotePensioneStorico.Count > 0)
            {
                datiCalcoloQuotePensioneStorico = new DatiCalcoloQuotePensione();
                datiCalcoloQuotePensioneStorico.LQuotePensione = new List<DatiQuotePensione>();

                foreach (GestioneCalcolo.QuotePensione quotePensione in contenitore.ListaQuotePensioneStorico)
                {
                    DatiQuotePensione datiQuotePensione = new DatiQuotePensione();
                    Utility.ValorizzaOggetti(quotePensione, datiQuotePensione);
                    if (contenitore.ListaTrattenuteQuotePensioneStorico != null && contenitore.ListaTrattenuteQuotePensioneStorico.Count > 0 &&
                        contenitore.ListaTrattenuteQuotePensioneStorico.Any(x => x.EnteGestioneFondoQuote == quotePensione.EnteGestioneFondo))
                    {
                        datiQuotePensione.ListaTrattenute = new List<DatiQuotePensione.DatiTrattenute>();
                        foreach (GestioneCalcolo.TrattenuteQuotePensione trattenute in contenitore.ListaTrattenuteQuotePensioneStorico.FindAll(x => x.EnteGestioneFondoQuote == quotePensione.EnteGestioneFondo))
                        {
                            DatiQuotePensione.DatiTrattenute datiTrattenute = new DatiQuotePensione.DatiTrattenute();
                            Utility.ValorizzaOggetti(trattenute, datiTrattenute);
                            datiQuotePensione.ListaTrattenute.Add(datiTrattenute);
                        }
                    }
                    datiCalcoloQuotePensioneStorico.LQuotePensione.Add(datiQuotePensione);
                }
            }
        }

        public static void GetQuoteMiglioramentiContrattualiStoricoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiCalcoloQuoteMiglioramentiContrattuali datiCalcoloQuoteMiglioramentiContrattualiStorico)
        {
            datiCalcoloQuoteMiglioramentiContrattualiStorico = null;

            if (contenitore.ListaQuoteMiglioramentiContrattualiStorico != null && contenitore.ListaQuoteMiglioramentiContrattualiStorico.Count > 0)
            {
                datiCalcoloQuoteMiglioramentiContrattualiStorico = new DatiCalcoloQuoteMiglioramentiContrattuali();
                datiCalcoloQuoteMiglioramentiContrattualiStorico.LQuoteMiglioramentiContrattuali = new List<DatiQuoteMiglioramentiContrattuali>();

                foreach (GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali QuoteMiglioramentiContrattuali in contenitore.ListaQuoteMiglioramentiContrattualiStorico)
                {
                    DatiQuoteMiglioramentiContrattuali datiQuoteMiglioramentiContrattuali = new DatiQuoteMiglioramentiContrattuali();
                    Utility.ValorizzaOggetti(QuoteMiglioramentiContrattuali, datiQuoteMiglioramentiContrattuali);
                    datiCalcoloQuoteMiglioramentiContrattualiStorico.LQuoteMiglioramentiContrattuali.Add(datiQuoteMiglioramentiContrattuali);
                }
            }
        }

        public static void StoreDatiCalcoloByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiCalcolo datiCalcolo,
            DatiExINPDAI datiInpdai, DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo, bool isSingleTab, out DatiCalcolo datiCalcoloOrdinati,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            //Aggiunto per gestire la personalizzazione del messaggio come da Appunto n. 36/2022 (JIRA L2_IST_LIQ-1857)
            string messaggioVideoApp = string.Empty;
            datiCalcoloOrdinati = new DatiCalcolo();
            List<GestioneCalcolo.DatiCalcoloContributivo> lContribuzione = null;
            List<GestioneCalcolo.DatiCalcoloRetributivo> lRetribuzione = null;
            #region RecuperoDati
            GestioneDanteCausa.DatiDanteCausa datiDA = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                datiDA = contenitore.DatiDanteCausa;

            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = null;
            if (isSingleTab)
                listaDatiCalcoloVittimeTerrorismo = contenitore.ListaDatiCalcoloVittimeTerrorismo;
            else
                listaDatiCalcoloVittimeTerrorismo = MapCalcoloVittimeTerrorismoFromViewToBL(datiCalcoloVittimeTerrorismo);

            //Per domande VESO92 con filtro L92 non saranno visibili nessuna delle griglie dei dati contributivi e retributivi ma solo il campo importoLordoAllaDecorenza
            //di conseguenza bypasso tutti controlli relativi a questi dati.
            bool isDomandaVESO92_L92 = Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null);
            bool isDomandaVOCRED_CRED27__DAP = Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione);
            bool isDomandaVAPE = Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria);
            bool isDomandaVESO29 = Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria);
            bool isDomandaVOESO_FS = (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") || Utility.IsDomandaVOESOWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null);
            bool isDomandaESOTEL = Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria);
            bool isDomandaESOAMB_L26 = Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione);
            bool isDomandaESPA_L26 = Utility.IsDomandaESPA_L26(contenitore.DatiPensione);
            bool isDomandaVOESO_Erariale_ESA = (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione));
            bool isDomandaRendita = Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione);
            bool isDomandaVESO33_DAP = Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione);
            bool isDomandaESOPMI = Utility.IsDomandaESOPMI(contenitore.DatiPensione.SiglaCategoria);
            #endregion RecuperoDati

            if (datiCalcolo.lDatiRetributivi != null && datiCalcolo.lDatiRetributivi.Count > 0)
            {
                var ret = datiCalcolo.lDatiRetributivi.Select(x => x.CodGestione.ToString()).ToList();
                char[] charsToRemove = new char[] { '-', '_', ',', '.', '/', '0', ' ' };
                string gestione = contenitore.DatiPensione.Gestione.TrimStart(charsToRemove);
                if (Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null && !ret.Contains(gestione))
                {
                    messaggioVideo = "Il codice gestione deve essere uguale alla gestione della pensione che si sta lavorando";
                    return;
                }
            }

            if (isDomandaVESO92_L92 || isDomandaVOCRED_CRED27__DAP || isDomandaVAPE || isDomandaVESO29 || isDomandaVOESO_FS || isDomandaESOTEL || isDomandaESOAMB_L26 || isDomandaVOESO_Erariale_ESA ||
                isDomandaESPA_L26 || isDomandaRendita || isDomandaVESO33_DAP || isDomandaESOPMI || contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault() || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione) || (ControlsDatiCalcolo(ref contenitore, datiCalcolo, ref contenitoreDecodifica, out messaggioVideoApp) &&
                ControlsDatiCalcoloINPDAI(ref contenitore, ref contenitoreDecodifica, datiCalcolo.lDatiRetributivi, datiCalcolo.lDatiContributivi, (datiInpdai != null) ? datiInpdai.AnzAl95 : null,
                (datiInpdai != null) ? datiInpdai.QuotaAl95 : null, contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null,
                out messaggioVideoApp)
                && GestioneControlli.VerificaDataPerfezionamentoPerTrasfAOI(contenitore.DatiPensione,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault() : 0,
                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0,
                contenitore.DatiEnpals, contenitore.DatiAreaTitolare.Anagrafica, Utility.DataSistemaAgo, datiCalcolo.FacoltaComputo, out messaggioVideo) &&
                ControlsDatiCalcoloAUT(ref contenitore, ref contenitoreDecodifica, datiCalcolo.lDatiContributivi, datiCalcolo.FacoltaComputo, out messaggioVideoApp)))
            {
                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI 
                if ((datiCalcolo.TipoCalcolo == TipoCalcolo.Retributivo || datiCalcolo.TipoCalcolo == TipoCalcolo.Misto || Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)) && datiCalcolo.lDatiRetributivi != null &&
                     datiCalcolo.lDatiRetributivi.Count > 0)
                {
                    InsertQuotaFittiziaA(ref contenitore, ref contenitoreDecodifica, datiCalcolo);

                    lRetribuzione = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
                    foreach (GestioneAggiornamentoPECO.DatiRetributivi calRetr in datiCalcolo.lDatiRetributivi)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                        datiRetr.IdPensione = datiCalcolo.IdPensione;
                        datiRetr.DecorrenzaOriginariaPensione = calRetr.Decorrenza;
                        datiRetr.CodiceGestione = calRetr.CodGestione;
                        datiRetr.QuotePrimeLiquidate = calRetr.Quota;
                        datiRetr.CodiceTipoQuota = calRetr.CodiceTipoQuota;
                        datiRetr.NSettimane707 = calRetr.NSettimane707;

                        if (calRetr.Quota.HasValue && calRetr.Quota.Value.ToString().ToUpperInvariant() == "A")
                        {
                            datiRetr.RMSQuotaA = calRetr.RMSQuotaA;
                            datiRetr.NSettimaneQuotaA = calRetr.SettimaneA;
                        }
                        else if (calRetr.Quota.HasValue && calRetr.Quota.Value.ToString().ToUpperInvariant() == "B")
                        {
                            datiRetr.RMSQuotaB = calRetr.RMSQuotaB;
                            datiRetr.NSettimaneQuotaB = calRetr.SettimaneB;
                        }
                        datiRetr.PL_Quotar = calRetr.PL_Quotar;
                        datiRetr.PL_Quotar707 = calRetr.PL_Quotar707;
                        if (Utility.IsDomandaSOMIN(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)))
                            datiRetr.RMS = calRetr.RMS;

                        //aggiunti per ripassarli per le ante96
                        datiRetr.NSettAnzianitaVV = calRetr.NSettAnzianitaVV;
                        datiRetr.NSettimaneExCombattente = calRetr.NSettimaneExCombattente;
                        datiRetr.RMSExCombattente = calRetr.RMSExCombattente;

                        if (ControlsDatiRetributivi(datiRetr, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : null, contenitore.DatiPensione, datiDA,
                            contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, contenitore.TipoCalcolo, contenitore.IsRiaperturaDomanda, datiCalcolo.lDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo, contenitore.ListaDatiContributivi, out messaggioVideo))
                            lRetribuzione.Add(datiRetr);
                        else
                            return;
                    }

                    if (!GestioneControlli.ControlsDatiRetributiviFinal(ref lRetribuzione, ref contenitore, ref contenitoreDecodifica, datiCalcolo.lDatiContributivi, contenitore.ListaDatiContributivi, out messaggioVideo))
                    {
                        datiCalcoloOrdinati.lDatiRetributivi = MappingDatiRetributiviFromBLToView(lRetribuzione);
                        return;
                    }
                    datiCalcoloOrdinati.lDatiRetributivi = MappingDatiRetributiviFromBLToView(lRetribuzione);
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (((datiCalcolo.TipoCalcolo == TipoCalcolo.Contributivo || datiCalcolo.TipoCalcolo == TipoCalcolo.Misto || Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)) &&
                    datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.Count > 0) ||
                    datiCalcolo.TipoCalcolo == TipoCalcolo.Retributivo &&
                    (contenitore.DatiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01)) ||
                        (Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione) && datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.Count > 0))
                    )
                {
                    lContribuzione = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                    GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneK = null;
                    if (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null && contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "K"))
                        decGestioneK = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "K");

                    GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneL = null;
                    if (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null && contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "L"))
                        decGestioneL = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "L");

                    var isAnte96 = Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null ? true : false;
                    if (datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.Count > 0)
                    {
                        foreach (GestioneAggiornamentoPECO.DatiContributivi calContr in datiCalcolo.lDatiContributivi)
                        {
                            GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();

                            datiContr.IdPensione = datiCalcolo.IdPensione;
                            datiContr.CodiceGestione = calContr.CodGestione;

                            if (calContr.Quota.HasValue && calContr.Quota.Value.ToString().ToUpperInvariant() == "C")
                            {
                                datiContr.ImportoContributivoTotale = calContr.ImportoContributivo;
                                datiContr.Montante = calContr.MontanteContributivo;
                                datiContr.NSettimane = calContr.Settimane;
                            }
                            else if (calContr.Quota.HasValue && calContr.Quota.Value.ToString().ToUpperInvariant() == "D")
                            {
                                datiContr.ImportoContribTotaleQuotaDL214 = calContr.ImportoContributivoQuotaD;
                                datiContr.MontanteQuotaDL214 = calContr.MontanteContributivoQuotaD;
                                datiContr.NSettimaneQuotaDL214 = calContr.SettimaneQuotaD;
                            }
                            else if (!calContr.Quota.HasValue && ((decGestioneK != null && calContr.CodGestione == decGestioneK.Id) || (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria)
                                && decGestioneL != null && calContr.CodGestione == decGestioneL.Id)))
                            {
                                datiContr.ImportoContributivoTotale = calContr.ImportoContributivo;
                                datiContr.Montante = calContr.MontanteContributivo;
                                datiContr.NSettimane = calContr.Settimane;
                            }
                            //alcune "ante" non hanno la quota
                            else if (!calContr.Quota.HasValue && (Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa) || isAnte96))
                            {
                                datiContr.Montante = calContr.MontanteContributivo;
                                if (isAnte96) datiContr.ImportoContributivoTotale = calContr.ImportoContributivo;
                            }

                            if (Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                            {
                                datiContr.DecorrenzaCalcoloContibutivo = contenitore.DatiContributivi.DecorrenzaCalcoloContibutivo;
                            }

                            if (Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null)
                                datiContr.DecorrenzaCalcoloContibutivo = calContr.DecorrenzaCalcoloContibutivo;

                            datiContr.PL_Quotac = calContr.PL_Quotac;
                            if (ControlsDatiContributivi(datiContr, contenitore.DatiPensione, datiDA, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo,
                                contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : string.Empty, contenitore.TipoCalcolo, datiCalcolo.lDatiContributivi, contenitore.ListaDatiContributivi, contenitore.DatiDanteCausa, out messaggioVideo))
                                lContribuzione.Add(datiContr);
                            else
                                return;
                        }
                    }

                    if (!GestioneControlli.ControlsDatiContributiviFinal(ref lContribuzione, contenitore.DatiPensione, datiDA, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo,
                        contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault() : 0,
                        contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio : null, contenitore.TipoCalcolo,
                        out messaggioVideo))
                    {
                        datiCalcoloOrdinati.lDatiContributivi = MappingDatiContributiviFromBLToView(ref contenitoreDecodifica, ref contenitore, lContribuzione, contenitore.DatiPensione);
                        return;
                    }
                    datiCalcoloOrdinati.lDatiContributivi = MappingDatiContributiviFromBLToView(ref contenitoreDecodifica, ref contenitore, lContribuzione, contenitore.DatiPensione);
                }

                if (!GestioneControlli.ControlsDatiContribRetribFinal(lContribuzione, lRetribuzione, contenitore.DatiPensione, listaDatiCalcoloVittimeTerrorismo, contenitore.DatiBeneficioVittimeTerrorismo,
                    contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI, contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo,
                    contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, contenitore.TipoCalcolo, contenitore.DatiDanteCausa, contenitore.DatiIntegrazioneArt11, out messaggioVideo))
                    return;

                if (!GestioneControlli.IsRiduzioneAssegnoAmmissibile(ref contenitore, ref contenitoreDecodifica, lContribuzione, out messaggioVideo))
                    return;

                if ((Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria)) && contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault())
                {
                    if (!datiCalcolo.ImportoLordoAllaDecorrenza.HasValue || datiCalcolo.ImportoLordoAllaDecorrenza.Value == 0)
                    {
                        messaggioVideo = "L' importo lordo alla decorrenza deve essere maggiore di 0";
                        return;
                    }
                }

                //ex-inpdai
                if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (datiInpdai != null && (datiInpdai.AnzAl95.HasValue || datiInpdai.QuotaAl95.HasValue))
                    {
                        if (contenitore.DatiPensioniDatiGenerici == null)
                            contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                        contenitore.DatiPensioniDatiGenerici.AnzAl95 = datiInpdai.AnzAl95;
                        contenitore.DatiPensioniDatiGenerici.QuotaAl95 = datiInpdai.QuotaAl95;
                    }
                }
                //domande aut
                if (Utility.IsDomandaAUT(contenitore.DatiPensione))
                {
                    if (contenitore.DatiPensioniDatiGenerici == null)
                        contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    contenitore.DatiPensioniDatiGenerici.FacoltaComputo = datiCalcolo.FacoltaComputo;
                }
                //domande VESO92 con filtro 'L92'
                if (isDomandaVESO92_L92 || isDomandaVOCRED_CRED27__DAP || isDomandaVESO29 || isDomandaVOESO_FS || isDomandaESOTEL || isDomandaESOAMB_L26 || isDomandaVOESO_Erariale_ESA || isDomandaESPA_L26 || isDomandaVESO33_DAP || contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault() ||
                    isDomandaESOPMI || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione))
                {
                    if (contenitore.DatiPensioniDatiGenerici == null)
                        contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza = datiCalcolo.ImportoLordoAllaDecorrenza;
                }

                //domande VAPE
                if (isDomandaVAPE)
                {
                    if (!ControlsDatiCalcoloVAPE(datiCalcolo != null ? datiCalcolo.ImportoLordo : (decimal?)null, contenitore.DatiPensione, out messaggioVideo))
                        return;
                    if (contenitore.DatiPensioniDatiGenerici == null)
                        contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    contenitore.DatiPensioniDatiGenerici.ImportoLordo = datiCalcolo.ImportoLordo;
                }

                if (datiCalcolo.PL_Coeftrasf != null)
                {
                    if (contenitore.DatiPensioniDatiGenerici == null)
                        contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf = datiCalcolo.PL_Coeftrasf;
                }

                if (isDomandaRendita)
                {
                    if (ControlsDatiCalcoloRendita(datiCalcolo, contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    {
                        if (contenitore.DatiPensioniDatiGenerici == null)
                            contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                        contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria = datiCalcolo.ImportoMensileAllaDecorrenzaOriginaria;
                        contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001 = datiCalcolo.ImportoMensileAlGennaio2001;
                    }
                    else
                        return;
                }

                if (isDomandaRendita)
                {
                    if (ControlsDatiCalcoloRendita(datiCalcolo, contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    {
                        if (contenitore.DatiPensioniDatiGenerici == null)
                            contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                        contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria = datiCalcolo.ImportoMensileAllaDecorrenzaOriginaria;
                        contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001 = datiCalcolo.ImportoMensileAlGennaio2001;
                    }
                    else
                        return;
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                {
                    if (contenitore.DatiPensioniDatiGenerici == null)
                        contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf = datiCalcolo.PL_Coeftrasf;
                }

                //ENG - Memo 116/2025
                if (Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione)
                    || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione)
                    || Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione))
                {
                    if (datiCalcolo.ContributiItalianiEdEsteriAl1295.HasValue)
                    {
                        if (contenitore.DatiPensioniDatiGenerici == null)
                            contenitore.DatiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                        contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 = datiCalcolo.ContributiItalianiEdEsteriAl1295;
                    }
                    else
                        contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 = null;
                }

                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = contenitore.DatiPensioniDatiGenerici;
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
                //----------------------------------------------------------------

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestioneCalcolo.EliminaCalcoloContributivoByIdPensione(datiPensione.Id, false);
                    GestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(datiPensione.Id, false);

                    if (lContribuzione != null && lContribuzione.Count > 0)
                    {
                        GestioneCalcolo.SalvaListCalcoloContributivoCI_AGO(lContribuzione);
                        if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                            datiQuadroDatiContributivi.TabDatiCalcoloINPDAI = 2;
                        else
                            datiQuadroDatiContributivi.TabDatiCalcolo = 2;
                    }
                    if (lRetribuzione != null && lRetribuzione.Count > 0)
                    {
                        GestioneCalcolo.SalvaListaCalcoloRetributivoCI_AGO(lRetribuzione);
                        if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                            datiQuadroDatiContributivi.TabDatiCalcoloINPDAI = 2;
                        else
                            datiQuadroDatiContributivi.TabDatiCalcolo = 2;
                    }

                    //ex-inpdai
                    //ENG - Aggiornamento Memo 68/2022 IOPGI
                    //ENG - Spacchettate SOPGI
                    if ((Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) || Utility.IsDomandaAUT(datiPensione) || datiCalcolo.PL_Coeftrasf != null ||
                        Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)) && datiPensioniDatiGenerici != null)
                    {
                        if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                        {
                            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                            datiPensioniDatiGenerici = null;
                        }
                        else
                            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioniDatiGenerici);
                        //per le VOPGI se inseriamo PL_Coeftrasf dobbiamo poter salvare il TabDatiCalcolo anche se non ci sono Retributivi e Contributivi
                        //ENG - Aggiornamento Memo 68/2022 IOPGI
                        //ENG - Spacchettate SOPGI
                        if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)) && datiCalcolo.PL_Coeftrasf != null)
                            datiQuadroDatiContributivi.TabDatiCalcolo = 2;
                    }
                    //veso92 filtro L92 vape
                    if ((isDomandaVESO92_L92 || isDomandaVOCRED_CRED27__DAP || isDomandaVAPE || isDomandaVESO29 || isDomandaVOESO_FS || isDomandaESOTEL || isDomandaESOAMB_L26 || isDomandaVOESO_Erariale_ESA || isDomandaESPA_L26 || isDomandaRendita || isDomandaVESO33_DAP || contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault() ||
                        isDomandaESOPMI || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione)) && datiPensioniDatiGenerici != null)
                    {
                        if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                        {
                            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                            datiPensioniDatiGenerici = null;
                        }
                        else
                            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioniDatiGenerici);

                        datiQuadroDatiContributivi.TabDatiCalcolo = 2;
                    }
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiCalcolo.IdPensione, datiQuadroDatiContributivi);
                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                //--------------------------------------------------------------------
                contenitore.DatiPensioniDatiGenerici = datiPensioniDatiGenerici;
                contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
                contenitore.ListaDatiRetributivi = lRetribuzione;
                contenitore.ListaDatiContributivi = lContribuzione;
                //--------------------------------------------------------------------
            }
            else if (messaggioVideo == string.Empty && messaggioVideoApp != string.Empty)
            {
                messaggioVideo = messaggioVideoApp;

                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) &&
                   (Utility.IsDomandaFPLD(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaGestioneAutonomi(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaAUT(contenitore.DatiPensione)))
                    messaggioVideo = messaggioVideo + "<br/>Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva";
            }
        }

        public static void StoreDatiCalcoloENPALSByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, DatiCalcoloENPALS datiCalcoloEnpals, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;

            if (!ControlsDatiCalcoloENPALS(datiCalcoloEnpals, contenitore.DatiPensione, out messaggioVideo))
                return;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneEnpals.DatiEnpals datiEnpals = contenitore.DatiEnpals;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            GestioneCalcolo.DatiCalcoloRetributivoENPAL datiRetributivi = null;
            List<GestioneCalcolo.DatiCalcoloContributivoENPAL> datiContributivi = null;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (Utility.IsEnpalsManualePL(true, Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda), datiPensione.IsDatiENPALSRecuperati))
                {
                    datiRetributivi = GetDatiRetributiviEnpalsOrdinati(datiCalcoloEnpals.LDatiRetributivi);
                    datiContributivi = GetDatiContributiviEnpalsOrdinati(datiCalcoloEnpals.LDatiContributivi);
                    if (datiRetributivi != null)
                    {
                        datiRetributivi.IdPensione = datiPensione.Id;
                        Utility.ValorizzaOggetti(datiCalcoloEnpals, datiRetributivi);
                        GestioneCalcolo.SalvaCalcoloRetributivoEnpals(datiRetributivi);
                    }
                    if (datiContributivi != null && datiContributivi.Count > 0)
                    {
                        foreach (var contr in datiContributivi)
                        {
                            contr.IdPensione = datiPensione.Id;
                            GestioneCalcolo.SalvaCalcoloContributivoEnpals(contr);
                        }
                    }

                    if (datiEnpals == null)
                    {
                        datiEnpals = new GestioneEnpals.DatiEnpals();
                        datiEnpals.IdPensione = datiPensione.Id;
                    }
                    Utility.ValorizzaOggetti(datiCalcoloEnpals, datiEnpals);
                    GestioneEnpals.SalvaDatiEnpalsEnpals(datiEnpals);
                }

                datiQuadroDatiContributivi.TabDatiCalcoloENPALS = 2;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiEnpals = datiEnpals;
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            if (Utility.IsEnpalsManualePL(true, Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda), datiPensione.IsDatiENPALSRecuperati))
            {
                contenitore.DatiCalcoloRetributivoENPALS = datiRetributivi;
                contenitore.DatiCalcoloContributivoENPALS = datiContributivi != null ? datiContributivi.FirstOrDefault() : null;
            }
            //--------------------------------------------------------------------
        }

        public static void StoreDatiCalcoloQuotePensioneByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiCalcoloQuotePensione datiCalcoloQuotePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<GestioneCalcolo.QuotePensione> lQuotePensione = null;
            List<GestioneCalcolo.TrattenuteQuotePensione> lTrattenuteQuotePensione = null;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            bool IsUpdateDatiGenerici = false;

            if (!ControlsDatiCalcoloQuotePensione(ref contenitore, ref contenitoreDecodifica, datiCalcoloQuotePensione, out messaggioVideo))
                return;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            //ENG - RIC Cumulo Progressiva: il tab Redditi deve essere opzionale se il campo "Cumulo Esterno" è pari ad "E"
            GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = contenitore.DatiQuadroRedditi;
            //----------------------------------------------------------------

            lQuotePensione = MappingQuotePensioneToDB(datiCalcoloQuotePensione.LQuotePensione);
            lQuotePensione.ForEach(x => x.IdPensione = datiPensione.Id);
            lTrattenuteQuotePensione = MappingTrattenuteQuotePensioneToDB(datiCalcoloQuotePensione.LQuotePensione);
            if (lTrattenuteQuotePensione != null && lTrattenuteQuotePensione.Count > 0)
                lTrattenuteQuotePensione.ForEach(x => x.IdPensione = datiPensione.Id);
            IsUpdateDatiGenerici = IsUpdateCumuloEsterno(ref contenitore, lQuotePensione, out datiPensioniDatiGenerici);

            //ENG - MEMO 74_2023
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);
            if (datiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295.HasValue)
            {
                if (datiGenerici == null)
                    datiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                datiGenerici.ContributiItalianiEdEsteriAl1295 = datiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295;
            }
            else
                datiGenerici.ContributiItalianiEdEsteriAl1295 = null;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //ENG - MEMO 74_2023
                if (datiGenerici != null)
                    GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);

                GestioneCalcolo.EliminaTrattenuteQuotePensioneByIdPensione(datiPensione.Id, false);
                GestioneCalcolo.EliminaQuotePensioneByIdPensione(datiPensione.Id, false);

                GestioneCalcolo.SalvaListaQuotePensione(lQuotePensione);
                if (lTrattenuteQuotePensione != null && lTrattenuteQuotePensione.Count > 0)
                    GestioneCalcolo.SalvaListaTrattenuteQuotePensione(lTrattenuteQuotePensione);

                if (IsUpdateDatiGenerici)
                {
                    GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioniDatiGenerici);
                    //ENG - RIC Cumulo Progressiva: il tab Redditi deve essere opzionale se il campo "Cumulo Esterno" è pari ad "E"
                    if (Utility.IsRicostituzioneCumuloProgressiva(contenitore.DatiPensione) && datiPensioniDatiGenerici != null && datiPensioniDatiGenerici.CumuloEsterno == 'E')
                    {
                        if (datiQuadroRedditi != null && datiQuadroRedditi.TabRedditi != 2)
                        {
                            datiQuadroRedditi.TabRedditi = 1;
                            datiQuadroRedditi.Tipo = 1;
                            GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                        }
                    }
                }

                datiQuadroDatiContributivi.TabQuotePensione = 2;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            contenitore.ListaQuotePensione = null;
            contenitore.ListaTrattenuteQuotePensione = null;
            //ENG - RIC Cumulo Progressiva: il tab Redditi deve essere opzionale se il campo "Cumulo Esterno" è pari ad "E"
            contenitore.DatiQuadroRedditi = datiQuadroRedditi;
            //--------------------------------------------------------------------
        }

        public static void StoreDatiCalcoloQuoteMiglioramentiContributiviByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiCalcoloQuoteMiglioramentiContrattuali datiCalcoloQuotePensione, out string messaggioVideo)
        {
            //PER IL MOMENTO AGGIORNO SOLO IL COLORE DEL SEMAFORO
            messaggioVideo = string.Empty;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            //----------------------------------------------------------------

            datiQuadroDatiContributivi.TabMiglioramentiContrattuali = 2;

            GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);


            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            //--------------------------------------------------------------------
        }



        private static bool IsUpdateCumuloEsterno(ref EntityBLCommon.ContenitoreObject contenitore, List<GestioneCalcolo.QuotePensione> lQuotePensione, out GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            datiPensioniDatiGenerici = null;
            if (Utility.IsRicostituzioneCumuloProgressiva(contenitore.DatiPensione))
            {
                datiPensioniDatiGenerici = contenitore.DatiPensioniDatiGenerici;
                if (datiPensioniDatiGenerici != null && lQuotePensione != null && lQuotePensione.Count > 0)
                {
                    if (lQuotePensione.Any(x => x.Decorrenza.Equals(new DateTime(9999, 1, 1)) && x.Importo != null && x.Importo <= 0.02m))
                    {
                        if (datiPensioniDatiGenerici.CumuloEsterno != 'M')
                        {
                            datiPensioniDatiGenerici.CumuloEsterno = 'M';
                            return true;
                        }
                    }
                    else
                    {
                        if (datiPensioniDatiGenerici.CumuloEsterno != 'E')
                        {
                            datiPensioniDatiGenerici.CumuloEsterno = 'E';
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void StoreDatiCalcoloVittimeTerrorismoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo, DatiCalcolo datiCalcolo, Utility.TipoCalcolo tipoCalcolo, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = MapCalcoloVittimeTerrorismoFromViewToBL(datiCalcoloVittimeTerrorismo);
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;

            if (isSingleTab)
            {
                listaDatiCalcoloRetributivo = contenitore.ListaDatiRetributivi;
                listaDatiCalcoloContributivo = contenitore.ListaDatiContributivi;
            }
            else
            {
                listaDatiCalcoloRetributivo = datiCalcolo != null ? MappingDatiRetributiviFromViewToBL(datiCalcolo.lDatiRetributivi) : null;
                listaDatiCalcoloContributivo = datiCalcolo != null ? MappingDatiContributiviFromViewToBL(ref contenitoreDecodifica, datiCalcolo.lDatiContributivi, contenitore.DatiPensione, contenitore.DatiDanteCausa) : null;
            }

            if (!ControlsDatiCalcoloVittimeTerrorismo(contenitore.DatiPensione, listaDatiCalcoloVittimeTerrorismo, contenitore.DatiBeneficioVittimeTerrorismo, listaDatiCalcoloRetributivo,
                listaDatiCalcoloContributivo, contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo,
                contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI, contenitore.DatiMaggiorazioniBenefici, tipoCalcolo, contenitore.DatiDanteCausa, out messaggioVideo))
                return;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneCalcoloVittimeTerrorismo.EliminaCalcoloVittimeTerrorismoByIdPensione(datiPensione.Id);

                foreach (GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcoloVittime in listaDatiCalcoloVittimeTerrorismo)
                    GestioneCalcoloVittimeTerrorismo.SalvaCalcoloVittimeTerrorismo(datiPensione.Id, datiCalcoloVittime);

                datiQuadroDatiContributivi.TabVittime = 2;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            contenitore.ListaDatiCalcoloVittimeTerrorismo = listaDatiCalcoloVittimeTerrorismo;
            //--------------------------------------------------------------------
        }

        private static List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> MapCalcoloVittimeTerrorismoFromViewToBL(DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo)
        {
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = new List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo>();

            if (datiCalcoloVittimeTerrorismo != null)
            {
                if (datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo != null && datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo.Count > 0)
                    foreach (DatiRetributiviVittimeTerrorismo datiRetributivi in datiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo)
                    {
                        GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcolo = new GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo();
                        Utility.ValorizzaOggetti(datiRetributivi, datiCalcolo);
                        datiCalcolo.Tipo = 'R';
                        listaDatiCalcoloVittimeTerrorismo.Add(datiCalcolo);
                    }

                if (datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo != null && datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo.Count > 0)
                    foreach (DatiContributiviVittimeTerrorismo datiContributivi in datiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo)
                    {
                        GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcolo = new GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo();
                        Utility.ValorizzaOggetti(datiContributivi, datiCalcolo);
                        datiCalcolo.Tipo = 'C';
                        listaDatiCalcoloVittimeTerrorismo.Add(datiCalcolo);
                    }

                if (datiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo != null && datiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo.Count > 0)
                    foreach (DatiImportoPensioneVittimeTerrorismo importoPensione in datiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo)
                    {
                        GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcolo = new GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo();
                        Utility.ValorizzaOggetti(importoPensione, datiCalcolo);
                        datiCalcolo.Tipo = 'I';
                        listaDatiCalcoloVittimeTerrorismo.Add(datiCalcolo);
                    }
            }
            return listaDatiCalcoloVittimeTerrorismo;
        }

        public static void DeleteDatiCalcoloENPALSByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneEnpals.DatiEnpals datiEnpals = contenitore.DatiEnpals;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (Utility.IsEnpalsManualePL(true, Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda), datiPensione.IsDatiENPALSRecuperati))
                {
                    GestioneCalcolo.EliminaCalcoloRetributivoEnpalsByIdPensione(datiPensione.Id, false);
                    GestioneCalcolo.EliminaCalcoloContributivoEnpalsByIdPensione(datiPensione.Id, false);

                    if (datiEnpals != null)
                    {
                        Utility.ValorizzaOggetti(new GestioneContrib.DatiCalcoloENPALS(), datiEnpals);
                        GestioneEnpals.SalvaDatiEnpalsEnpals(datiEnpals);
                    }
                }

                datiQuadroDatiContributivi.TabDatiCalcoloENPALS = 0;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiEnpals = datiEnpals;
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            //--------------------------------------------------------------------
        }

        public static void DeleteDatiCalcoloQuotePensioneByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
            try
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
                //----------------------------------------------------------------
                //ENG - MEMO 74_2023
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);
                if (datiGenerici != null)
                    datiGenerici.ContributiItalianiEdEsteriAl1295 = null;

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    //ENG - MEMO 74_2023
                    if (datiGenerici != null)
                    {
                        if (GestioneDatiGenericiAgoCi.IsDatiGenericiNull(datiGenerici))
                            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                        else
                            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);
                    }

                    GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);

                    GestioneCalcolo.EliminaTrattenuteQuotePensioneByIdPensione(datiPensione.Id, false);
                    GestioneCalcolo.EliminaQuotePensioneByIdPensione(datiPensione.Id, false);

                    datiQuadroDatiContributivi.TabQuotePensione = 0;

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                //--------------------------------------------------------------------
                contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
                //--------------------------------------------------------------------
            }
            catch (Exception Ex)
            {
                messaggioVideo = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }

        public static void DeleteDatiCalcoloVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            try
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
                //----------------------------------------------------------------

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestioneCalcoloVittimeTerrorismo.EliminaCalcoloVittimeTerrorismoByIdPensione(datiPensione.Id);

                    datiQuadroDatiContributivi.TabVittime = 0;

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                //--------------------------------------------------------------------
                contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
                //--------------------------------------------------------------------
            }
            catch (Exception Ex)
            {
                messaggioVideo = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }

        public static void GetDatiCalcoloQuotaFondoIntegrativoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
        out DatiQuotaFondoIntegrativo datiQuotaFondoIntegrativo, out string messaggioVideo)
        {
            datiQuotaFondoIntegrativo = new DatiQuotaFondoIntegrativo();
            messaggioVideo = string.Empty;

            if (contenitore.DatiPensione != null)
            {
                datiQuotaFondoIntegrativo.IdPensione = contenitore.DatiPensione.Id;
                if (contenitore != null && contenitore.ListaDatiQuotaFondoIntegrativo != null)
                {
                    datiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo = new List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo>();

                    foreach (GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo contrQuota in contenitore.ListaDatiQuotaFondoIntegrativo)
                    {
                        GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo datiContrQuota = new GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo();
                        datiContrQuota.CodiceGestione = contrQuota.CodiceGestione;
                        if (contrQuota.ImportoContributivoTotale.HasValue || contrQuota.Montante.HasValue)
                        {
                            datiContrQuota.Quota = 'C';
                            datiContrQuota.ImportoContributivoTotale = contrQuota.ImportoContributivoTotale;
                            datiContrQuota.Montante = contrQuota.Montante;
                            datiContrQuota.NSettimane = contrQuota.NSettimane;
                        }
                        else if (contrQuota.ImportoContribTotaleQuotaD.HasValue || contrQuota.MontanteQuotaD.HasValue || contrQuota.NSettimaneQuotaD.HasValue)
                        {
                            datiContrQuota.Quota = 'D';
                            datiContrQuota.ImportoContribTotaleQuotaD = contrQuota.ImportoContribTotaleQuotaD;
                            datiContrQuota.MontanteQuotaD = contrQuota.MontanteQuotaD;
                            datiContrQuota.NSettimaneQuotaD = contrQuota.NSettimaneQuotaD;
                        }
                        datiContrQuota.PL_Quotac = contrQuota.PL_Quotac;
                        datiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo.Add(datiContrQuota);
                    }
                }
                //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
                else if (contenitore != null && contenitore.ListaDatiQuotaFondoIntegrativoStorico != null)
                {
                    datiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo = new List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo>();

                    foreach (GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo contrQuota in contenitore.ListaDatiQuotaFondoIntegrativoStorico)
                    {
                        GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo datiContrQuota = new GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo();
                        datiContrQuota.CodiceGestione = contrQuota.CodiceGestione;
                        if (contrQuota.ImportoContributivoTotale.HasValue || contrQuota.Montante.HasValue)
                        {
                            datiContrQuota.Quota = 'C';
                            datiContrQuota.ImportoContributivoTotale = contrQuota.ImportoContributivoTotale;
                            datiContrQuota.Montante = contrQuota.Montante;
                            datiContrQuota.NSettimane = contrQuota.NSettimane;
                        }
                        else if (contrQuota.ImportoContribTotaleQuotaD.HasValue || contrQuota.MontanteQuotaD.HasValue || contrQuota.NSettimaneQuotaD.HasValue)
                        {
                            datiContrQuota.Quota = 'D';
                            datiContrQuota.ImportoContribTotaleQuotaD = contrQuota.ImportoContribTotaleQuotaD;
                            datiContrQuota.MontanteQuotaD = contrQuota.MontanteQuotaD;
                            datiContrQuota.NSettimaneQuotaD = contrQuota.NSettimaneQuotaD;
                        }
                        datiContrQuota.PL_Quotac = contrQuota.PL_Quotac;
                        datiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo.Add(datiContrQuota);
                    }
                }
            }
        }

        public static void StoreDatiCalcoloQuotaFondoIntegrativoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> datiQuotaFondoIntegrativo, DatiCalcolo datiCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> listaDatiCalcoloQuotaFondoIntegrativo = MappingDatiQuotaFondoIntegrativoFromViewToBL(ref contenitoreDecodifica, datiQuotaFondoIntegrativo);

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneQuotaFondoIntegrativo.EliminaQuotaFondoIntegrativoByIdPensione(datiPensione.Id, false);

                foreach (GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo datiQuota in listaDatiCalcoloQuotaFondoIntegrativo)
                    GestioneQuotaFondoIntegrativo.SalvaQuotaFondoIntegrativo(datiPensione.Id, datiQuota);

                datiQuadroDatiContributivi.TabQuotaFondoIntegrativo = 2;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
        }

        public static void DeleteDatiCalcoloQuotaFondoIntegrativo(ref EntityBLCommon.ContenitoreObject contenitore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            try
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
                //----------------------------------------------------------------

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestioneQuotaFondoIntegrativo.EliminaQuotaFondoIntegrativoByIdPensione(datiPensione.Id, false);

                    datiQuadroDatiContributivi.TabQuotaFondoIntegrativo = 0;

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            }
            catch (Exception Ex)
            {
                messaggioVideo = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }

        public static void GetDatiCalcoloQuotaFondoINPGIByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
        out DatiQuotaFondoINPGI datiQuotaFondoINPGI, out string messaggioVideo)
        {
            datiQuotaFondoINPGI = new DatiQuotaFondoINPGI();
            messaggioVideo = string.Empty;

            if (contenitore.DatiPensione != null)
            {
                datiQuotaFondoINPGI.IdPensione = contenitore.DatiPensione.Id;
                if (contenitore != null)
                {
                    if (contenitore.ListaDatiContributiviINPGI != null)
                    {
                        datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI>();
                        foreach (GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI contrQuota in contenitore.ListaDatiContributiviINPGI)
                        {
                            GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI datiContrQuota = new GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI();
                            datiContrQuota.CodiceGestione = contrQuota.CodiceGestione;
                            datiContrQuota.Montante = contrQuota.Montante;
                            datiContrQuota.Quota = contrQuota.Quota;
                            datiContrQuota.Settimane = contrQuota.Settimane;

                            datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI.Add(datiContrQuota);
                        }
                    }
                    if (contenitore.ListaDatiRetributiviINPGI != null)
                    {
                        datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI>();
                        foreach (GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI contrQuota in contenitore.ListaDatiRetributiviINPGI)
                        {
                            GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI datiContrQuota = new GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI();
                            datiContrQuota.CodiceGestione = contrQuota.CodiceGestione;
                            datiContrQuota.Settimane = contrQuota.Settimane;
                            datiContrQuota.ImportoCalcolato = contrQuota.ImportoCalcolato;
                            datiContrQuota.ImportoComma707 = contrQuota.ImportoComma707;
                            datiContrQuota.SettimaneComma707 = contrQuota.SettimaneComma707;
                            datiContrQuota.RetribuzioneMediaSettimanale = contrQuota.RetribuzioneMediaSettimanale;

                            datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI.Add(datiContrQuota);
                        }
                    }
                }
            }
        }

        public static void GetDatiCalcoloQuotaFondoINPGIStoricoByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
        out DatiQuotaFondoINPGI datiQuotaFondoINPGIStorico, out string messaggioVideo)
        {
            datiQuotaFondoINPGIStorico = new DatiQuotaFondoINPGI();
            messaggioVideo = string.Empty;

            if (contenitore.DatiPensione != null)
            {
                datiQuotaFondoINPGIStorico.IdPensione = contenitore.DatiPensione.Id;
                if (contenitore != null)
                {
                    if (contenitore.ListaDatiContributiviINPGIStorico != null)
                    {
                        datiQuotaFondoINPGIStorico.lDatiContributiviQuotaFondoINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI>();
                        foreach (GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI contrQuota in contenitore.ListaDatiContributiviINPGIStorico)
                        {
                            GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI datiContrQuota = new GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI();
                            datiContrQuota.CodiceGestione = contrQuota.CodiceGestione;
                            datiContrQuota.Montante = contrQuota.Montante;
                            datiContrQuota.Quota = contrQuota.Quota;
                            datiContrQuota.Settimane = contrQuota.Settimane;

                            datiQuotaFondoINPGIStorico.lDatiContributiviQuotaFondoINPGI.Add(datiContrQuota);
                        }
                    }
                    if (contenitore.ListaDatiRetributiviINPGIStorico != null)
                    {
                        datiQuotaFondoINPGIStorico.lDatiRetributiviQuotaFondoINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI>();
                        foreach (GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI contrQuota in contenitore.ListaDatiRetributiviINPGIStorico)
                        {
                            GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI datiContrQuota = new GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI();
                            datiContrQuota.CodiceGestione = contrQuota.CodiceGestione;
                            datiContrQuota.Settimane = contrQuota.Settimane;
                            datiContrQuota.ImportoCalcolato = contrQuota.ImportoCalcolato;
                            datiContrQuota.ImportoComma707 = contrQuota.ImportoComma707;
                            datiContrQuota.SettimaneComma707 = contrQuota.SettimaneComma707;
                            datiContrQuota.RetribuzioneMediaSettimanale = contrQuota.RetribuzioneMediaSettimanale;

                            datiQuotaFondoINPGIStorico.lDatiRetributiviQuotaFondoINPGI.Add(datiContrQuota);
                        }
                    }
                }
            }
        }

        public static void StoreDatiCalcoloQuotaFondoINPGIByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            DatiQuotaFondoINPGI datiQuotaFondoINPGI, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (ControlsDatiQuotaFondoINPGI(ref contenitore, ref contenitoreDecodifica, datiQuotaFondoINPGI, out messaggioVideo))
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
                //----------------------------------------------------------------
                List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> listaDatiContributiviQuotaFondoINPGI = MappingDatiContributiviQuotaFondoINPGIFromViewToBL(ref contenitoreDecodifica, datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI, datiPensione.Id);
                List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> listaDatiRetributiviQuotaFondoINPGI = MappingDatiRetributiviQuotaFondoINPGIFromViewToBL(ref contenitoreDecodifica, datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI, datiPensione.Id);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestioneQuotaFondoINPGI.EliminaCalcoloContributivoINPGIByIdPensione(datiPensione.Id, false);
                    GestioneQuotaFondoINPGI.EliminaCalcoloRetributivoINPGIByIdPensione(datiPensione.Id, false);

                    if (listaDatiContributiviQuotaFondoINPGI != null && listaDatiContributiviQuotaFondoINPGI.Count > 0)
                    {
                        foreach (GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI datiQuota in listaDatiContributiviQuotaFondoINPGI)
                            GestioneQuotaFondoINPGI.SalvaCalcoloContributivoINPGI(datiQuota);
                    }

                    if (listaDatiRetributiviQuotaFondoINPGI != null && listaDatiRetributiviQuotaFondoINPGI.Count > 0)
                    {
                        foreach (GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI datiQuota in listaDatiRetributiviQuotaFondoINPGI)
                            GestioneQuotaFondoINPGI.SalvaCalcoloRetributivoINPGI(datiQuota);
                    }

                    datiQuadroDatiContributivi.TabQuotaFondoINPGI = 2;

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            }
        }

        public static void DeleteDatiCalcoloQuotaFondoINPGI(ref EntityBLCommon.ContenitoreObject contenitore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            try
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
                //----------------------------------------------------------------

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestioneQuotaFondoINPGI.EliminaCalcoloContributivoINPGIByIdPensione(datiPensione.Id, false);
                    GestioneQuotaFondoINPGI.EliminaCalcoloRetributivoINPGIByIdPensione(datiPensione.Id, false);

                    datiQuadroDatiContributivi.TabQuotaFondoINPGI = 0;

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            }
            catch (Exception Ex)
            {
                messaggioVideo = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }

        private static bool ControlsDatiCalcolo(ref EntityBLCommon.ContenitoreObject contenitore, DatiCalcolo datiCalcolo, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoAppartenenza? tipo = Utility.GetTipoAppartenenza(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione);
            if (tipo != Utility.TipoAppartenenza.AGO)
                return false;

            if (BypassaControlliRicSdaiContributivoK(contenitore.DatiPensione, datiCalcolo.lDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo) ||
                BypassaControlliRic_VOCRED_CRED27_ContributivoL(contenitore.DatiPensione, datiCalcolo.lDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo) ||
                Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione) || //ENG - Memo 91/2026 
                Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)) != null)
                return true;

            //ENG- Memo 68/2022 aggiornato al 12/03/2025
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneModificheMemoINPGI_20250312 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20250312 ", out ctrlAbilitazioneModificheMemoINPGI_20250312);

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if ((datiCalcolo == null ||
                    ((datiCalcolo.lDatiContributivi == null || datiCalcolo.lDatiContributivi.Count == 0) &&
                    (datiCalcolo.lDatiRetributivi == null || datiCalcolo.lDatiRetributivi.Count == 0))) && !(Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && datiCalcolo.PL_Coeftrasf.HasValue)
                && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione) && datiCalcolo.PL_Coeftrasf.HasValue)
                && !(Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && datiCalcolo.PL_Coeftrasf.HasValue)
                && !((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2")
                && !(Utility.IsDomandaINPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                    contenitore.DatiPensione.FineAssicurazione.HasValue && !Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2022, 06, 30))))
            {
                if (ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI" &&
                    (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) ||
                    (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && (Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) || Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)))) &&
                    contenitore.DatiPensione.FineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2022, 06, 30)))
                    messaggioVideo = "Per data fine assicurazione maggiore al 30/06/2022 è necessaria la presenza di almeno una quota D.";
                else
                    messaggioVideo = "Non sono presenti dati della tab 'Dati Calcolo' da salvare.";
                return false;
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if ((!Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || !Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione)) && (!Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione)) && !Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa)
                && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)
                && !Utility.IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : null))
            {
                if (!contenitore.DatiPensione.FineAssicurazione.HasValue)
                {
                    messaggioVideo = "Data 'Fine Assicurazione' assente; verificare nella sezione 'Liquidazione Pensione'.";
                    return false;
                }
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (!Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
            {
                switch (datiCalcolo.TipoCalcolo)
                {
                    case TipoCalcolo.Contributivo:

                        if ((datiCalcolo.lDatiContributivi == null || datiCalcolo.lDatiContributivi.Count == 0 || (datiCalcolo.lDatiRetributivi != null && datiCalcolo.lDatiRetributivi.Count > 0)))
                        {
                            messaggioVideo = "'Tipo Calcolo' incongruente con i dati calcolo; verificare nella sezione 'Liquidazione Pensione'.";
                            return false;
                        }

                        if (contenitore.DatiPensione.FineAssicurazione.HasValue)
                        {
                            if (!Utility.DataSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01))) // ante 2012
                            {
                                if (datiCalcolo.lDatiContributivi.FindIndex(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) > -1)
                                {
                                    if (!Utility.IsPensioneInabilitaGenericaPost2012(contenitore.DatiPensione) && !Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                                    {
                                        messaggioVideo = string.Format("Quota 'D' non ammessa nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy}.",
                                            contenitore.DatiPensione.FineAssicurazione.Value);
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                // Modifica effettuata a seguito della mail di Sorrentino del 03-06-2014 con oggetto: segnalazione AGO
                                if (!datiCalcolo.IsUnicarpe
                                    && !Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione)
                                    && !Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione)
                                    && !Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione)
                                    && !Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                                {
                                    if (datiCalcolo.lDatiContributivi.FindIndex(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) == -1)
                                    {
                                        messaggioVideo = string.Format("Quota 'D' obbligatoria nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy}.",
                                            contenitore.DatiPensione.FineAssicurazione.Value);
                                        return false;
                                    }
                                }
                            }
                        }

                        break;
                    case TipoCalcolo.Retributivo:

                        if (contenitore.DatiPensione.FineAssicurazione.HasValue && !Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                        {
                            if (!Utility.DataSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01)))  // ante 2012
                            {
                                if ((datiCalcolo.lDatiRetributivi == null || datiCalcolo.lDatiRetributivi.Count == 0 || (datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.Count > 0)))
                                {
                                    if (!(Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione) && datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.Count > 0 &&
                                        datiCalcolo.lDatiContributivi.FindIndex(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) > -1 &&
                                        datiCalcolo.lDatiContributivi.FindIndex(x => (x.Settimane.HasValue || x.MontanteContributivo.HasValue || x.ImportoContributivo.HasValue)) == -1))
                                    {
                                        messaggioVideo = "'Tipo Calcolo' e/o data 'Fine assicurazione' incongruenti con i dati calcolo; verificare nella sezione 'Liquidazione Pensione'.";
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if ((datiCalcolo.lDatiRetributivi == null || datiCalcolo.lDatiRetributivi.Count == 0 ||
                                    (!datiCalcolo.IsUnicarpe && (datiCalcolo.lDatiContributivi == null || datiCalcolo.lDatiContributivi.Count == 0))))
                                {
                                    messaggioVideo = "'Tipo Calcolo' e/o data 'Fine assicurazione' incongruenti con i dati calcolo; verificare nella sezione 'Liquidazione Pensione'.";
                                    return false;
                                }
                                // Modifica effettuata a seguito della mail di Sorrentino del 03-06-2014 con oggetto: segnalazione AGO
                                if (!datiCalcolo.IsUnicarpe)
                                {
                                    if (!Utility.IsPensioneInabilitaPost2012(contenitore.DatiPensione)) //mail: Reeng Pensioni AGO - Modifiche applicative inabilità del 08/01/2014. Per le pensioni di inabilità (gruppo = 0002, prodotto = 0012) e decorrenzaPensione > 12/2011, occorre disabilitare il controllo tra la data fine assicurazione e la quota D dei dati calcolo contributivi.
                                    {
                                        if (datiCalcolo.lDatiContributivi.FindIndex(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) == -1)
                                        {
                                            messaggioVideo = string.Format("Quota 'D' obbligatoria nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy}",
                                                contenitore.DatiPensione.FineAssicurazione.Value);
                                            return false;
                                        }
                                    }
                                }
                                if (datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.FindIndex(x => (x.Settimane.HasValue || x.MontanteContributivo.HasValue || x.ImportoContributivo.HasValue)) > -1)
                                {
                                    messaggioVideo = string.Format("Quota 'C' non ammessa nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy}",
                                        contenitore.DatiPensione.FineAssicurazione.Value);
                                    return false;
                                }
                            }
                        }

                        // Modifica effettuata a seguito della mail di nuovaIVS del 01-12-2014 con oggetto: controlli dati retributivi e unicarpe
                        if (!datiCalcolo.IsUnicarpe &&
                            !(Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)) != null
                            && (contenitore.DatiPensione.SiglaCategoria.StartsWith("S") || contenitore.DatiPensione.SiglaCategoria.StartsWith("I")) && contenitore.DatiPensione.NaturaPensione != null
                            && (contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "3" || contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "4")))
                        {
                            if (contenitore.DatiPensione.FineAssicurazione.HasValue && !ControlsQuotaRetributivaB(datiCalcolo.lDatiRetributivi, contenitore.DatiPensione.FineAssicurazione, out messaggioVideo))
                                return false;
                        }

                        if (contenitore.DatiPensione.InizioAssicurazione.HasValue && !ControlsQuotaRetributivaA(datiCalcolo.lDatiRetributivi, contenitore.DatiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsPresenzaDatiRetributivi(datiCalcolo.lDatiRetributivi,contenitore.DatiPensione, out messaggioVideo))
                            return false;

                        break;
                    case TipoCalcolo.Misto:
                        if (!Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                        {
                            if (((datiCalcolo.lDatiRetributivi == null || datiCalcolo.lDatiRetributivi.Count == 0) || (datiCalcolo.lDatiContributivi == null || datiCalcolo.lDatiContributivi.Count == 0)))
                            {
                                messaggioVideo = "'Tipo Calcolo' e/o data 'Fine assicurazione' incongruenti con i dati calcolo; verificare nella sezione 'Liquidazione Pensione'.";
                                return false;
                            }

                            if (!Utility.DataSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01))) // ante 2012
                            {
                                if (datiCalcolo.lDatiContributivi.FindIndex(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) > -1)
                                {
                                    if (!Utility.IsPensioneInabilitaGenericaPost2012(contenitore.DatiPensione))
                                    {
                                        messaggioVideo = string.Format("Quota 'D' non ammessa nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy}",
                                            contenitore.DatiPensione.FineAssicurazione.Value);
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                // Modifica effettuata a seguito della mail di Sorrentino del 03-06-2014 con oggetto: segnalazione AGO
                                if (!datiCalcolo.IsUnicarpe)
                                {
                                    if (datiCalcolo.lDatiContributivi.FindIndex(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) == -1)
                                    {
                                        messaggioVideo = string.Format("Quota 'D' obbligatoria nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy}",
                                            contenitore.DatiPensione.FineAssicurazione.Value);
                                        return false;
                                    }
                                }
                            }
                        }

                        if (contenitore.DatiPensione.InizioAssicurazione.HasValue && !ControlsQuotaRetributivaA(datiCalcolo.lDatiRetributivi, contenitore.DatiPensione, out messaggioVideo))
                            return false;

                        if (!ControlsPresenzaDatiRetributivi(datiCalcolo.lDatiRetributivi,contenitore.DatiPensione, out messaggioVideo))
                            return false;

                        if (!Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                        {
                            if (!ControlsQuotaContributivaC(contenitore.DatiPensione, datiCalcolo.lDatiContributivi, datiCalcolo.lDatiRetributivi, out messaggioVideo))
                                return false;
                        }

                        break;
                    case TipoCalcolo.NonValido:
                        messaggioVideo = "E' necessario salvare il 'Tipo Calcolo' dal menu 'Liquidazione Pensione' prima di poter inserire i dati calcolo";
                        return false;
                }
            }

            //ENG- Memo 68/2022 aggiornato al 12/03/2025
            if (ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI")
            {
                if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && (Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) || Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)))) &&
                    !((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2") &&
                    contenitore.DatiPensione.FineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2022, 06, 30)))
                {
                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContr = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.ToList();
                    if (datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.Count() > 0)
                    {
                        foreach (GestioneAggiornamentoPECO.DatiContributivi datiContributivi in datiCalcolo.lDatiContributivi)
                        {
                            if (datiContributivi.Quota == 'C')
                            {
                                foreach (GestioneDecodifica.CodeGestioneCalcoloContributivo decCodeGestione in elencoCodeGestioneCalcoloContr)
                                {
                                    if (decCodeGestione.Id == datiContributivi.CodGestione && decCodeGestione.TraduzioneSuGP == "FB")
                                    {
                                        messaggioVideo = "Per il codice gestione FPLD - EC è ammessa solo la quota D.";
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    if (datiCalcolo.lDatiContributivi == null || !datiCalcolo.lDatiContributivi.Exists(x => x.Quota == 'D'))
                    {
                        messaggioVideo = "Per data fine assicurazione maggiore al 30/06/2022 è necessaria la presenza di almeno una quota D.";
                        return false;
                    }
                }
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if ((Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                && !((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2"))
            {
                if (((datiCalcolo.lDatiContributivi != null && datiCalcolo.lDatiContributivi.Count > 0) || (contenitore.ListaDatiContributiviINPGI != null && contenitore.ListaDatiContributiviINPGI.Count > 0)) &&
                    datiCalcolo.PL_Coeftrasf == null)
                {
                    messaggioVideo = "Coefficiente: campo obbligatorio";
                    return false;
                }
            }

            //ENG - Memo 116/2025: bloccare il salvataggio del tab DatiCalcolo se non è valorrizzato il campo Contributi Italiani ed Esteri al 31/12/95.
            if (Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione) ||
                Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione))
            {
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioniEstere);

                if ((!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "V") ||
                    (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0))
                {
                    if (!datiCalcolo.ContributiItalianiEdEsteriAl1295.HasValue)
                    {
                        messaggioVideo = "Contributi Italiani ed Esteri al 31/12/95 obbligatori";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsDatiCalcoloVAPE(decimal? importoLordo, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
            {
                if (importoLordo.GetValueOrDefault() > 1500)
                {
                    messaggioVideo = "L'importo lordo deve essere minore o uguale a 1500";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsDatiCalcoloINPDAIAlCalcolo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, decimal? anzAl95,
            decimal? quotaAl95, string tipoSettimaneBeneficio, Utility.TipoCalcolo tipoCalcolo, out string messaggioVideo)
        {
            List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetributivi = MappingDatiRetributiviFromBLToView(contenitore.ListaDatiRetributivi);
            List<GestioneAggiornamentoPECO.DatiContributivi> datiContributivi = MappingDatiContributiviFromBLToView(ref contenitoreDecodifica, ref contenitore, contenitore.ListaDatiContributivi, contenitore.DatiPensione);

            messaggioVideo = string.Empty;

            if (!ControlsDatiCalcoloINPDAI(ref contenitore, ref contenitoreDecodifica, datiRetributivi, datiContributivi, anzAl95, quotaAl95, tipoSettimaneBeneficio,
                out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsDatiCalcoloINPDAI(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            List<GestioneAggiornamentoPECO.DatiRetributivi> lDatiRetributivi, List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, decimal? anzAl95, decimal? quotaAl95,
            string tipoSettimaneBeneficio, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) || BypassaControlliRicSdaiContributivoK(contenitore.DatiPensione, lDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo))
                return true;

            decimal coefNormSettInGG = 6.9231M;
            decimal maxGGConsentiti = 14400M;

            DateTime decorrenzaOriginaria = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? contenitore.DatiPensione.DecorrenzaOriginaria.Value : DateTime.MinValue;

            #region Controlli Dati Retributivi
            if (lDatiRetributivi != null && lDatiRetributivi.Count > 0)
            {
                List<Liquidazione.BLCommon.CtrlDecorrenzaRetrExINPDAI> lstCtrlDecorrenza = contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI;
                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcoloRetributivo = contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo;
                List<DatiRetributiviExInpdai> lstDatiRetrExInpdai = lDatiRetributivi.Select(x => new DatiRetributiviExInpdai(x, lstCtrlDecorrenza, lstDecGestioneCalcoloRetributivo)).ToList();

                if (lstDatiRetrExInpdai.Exists(x => x.DecorrenzaExInpdai.GetValueOrDefault() == 0))
                {
                    DatiRetributiviExInpdai app = lstDatiRetrExInpdai.Find(x => x.DecorrenzaExInpdai.GetValueOrDefault() == 0);
                    messaggioVideo = "La terna Gestione '" + app.DecCodGestione + "', Quota '" + app.Quota + "' e Tipo Quota '" + app.CodiceTipoQuota + "' non è valida.";
                    return false;
                }

                //Controllo 1  
                //Il primo record presente nella griglia dei dati retributivi deve essere  bloccato con codice Gestione A e Quota A. (Valorizzazione decorrenza: MM/AAAA)
                DatiRetributiviExInpdai firstElem = lstDatiRetrExInpdai.FirstOrDefault();
                if (firstElem == null)
                {
                    messaggioVideo = "Il primo record deve essere valorizzato.";
                    return false;
                }
                else if (firstElem.DecCodGestione != "A" && firstElem.DecCodGestione != "S")
                {
                    messaggioVideo = "Codice gestione " + firstElem.DecCodGestione + " non ammesso per il primo record.";
                    return false;
                }
                //controllo che siano corretti i record successivi al primo 
                if (!Utility.IsDomandaDAIAnte2003(contenitore.DatiPensione, contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.HasValue ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.Value : DateTime.MaxValue))
                {
                    if (lstDatiRetrExInpdai.Count > 1 &&
                        lstDatiRetrExInpdai.GetRange(1, lstDatiRetrExInpdai.Count - 1).Exists(x => (x.DecCodGestione != "A" && x.DecCodGestione != "1" && x.DecCodGestione != "2" && x.DecCodGestione != "3" && x.DecCodGestione != "4")))
                    {
                        messaggioVideo = "Codice Gestione non ammesso.";
                        return false;
                    }
                }
                //Controllo 2 e 4 
                //Si deve verificare che non esistano 2 record con la stessa decorrenza (ovvero stessa gestione e stessa quota)
                List<int> lstOccurenceDecorrenza = (from e in lstDatiRetrExInpdai
                                                    group e by e.DecorrenzaExInpdai into d
                                                    select d.Count()).ToList();
                if (lstOccurenceDecorrenza.Exists(x => x > 1))
                {
                    messaggioVideo = "Non è possibile inserire due record con 'Gestione', 'Quota' e 'Tipo Quota' uguali.";
                    return false;
                }
                //Controllo 3 
                //Per tutti i codiceGestione i campi Quota, Giorni e Retribuzione sono obbligatori. Se la quota = B allora anche TipoQuota è obbligatorio
                foreach (var elem in lstDatiRetrExInpdai)
                {
                    if (elem.CodGestione == null || elem.Quota == null)
                    {
                        messaggioVideo = "Codice Gestioni e Quota per i dati retributivi sono campi obbligatori.";
                    }
                    switch (elem.Quota)
                    {
                        case 'A':
                            if (!elem.SettimaneA.HasValue || !elem.RMSQuotaA.HasValue)
                            {
                                messaggioVideo = "Per la quota 'A' la 'Reddito'/'Retribuzione media' e 'Settimane'/'Giorni' sono dati obbligatori.";
                                return false;
                            }
                            break;
                        case 'B':
                            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.HasValue && Utility.IsDomandaDAIAnte2003(contenitore.DatiPensione, contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.Value))
                            {
                                var config = lstCtrlDecorrenza.Find(x => x.Periodi == "Ante 2003" && x.Gestione.Trim() == elem.DecCodGestione && x.Quota == elem.Quota);
                                if (config != null && string.IsNullOrEmpty(config.TipoQuota))
                                {
                                    if (!elem.SettimaneB.HasValue || !elem.RMSQuotaB.HasValue)
                                    {
                                        messaggioVideo = "Per la quota 'B' la 'Reddito'/'Retribuzione media' e 'Settimane'/'Giorni' sono dati obbligatori.";
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!elem.SettimaneB.HasValue || !elem.RMSQuotaB.HasValue || string.IsNullOrEmpty(elem.CodiceTipoQuota))
                                    {
                                        messaggioVideo = "Per la quota 'B' la 'Reddito'/'Retribuzione media', 'Settimane'/'Giorni' e 'Tipo Quota' sono dati obbligatori.";
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!elem.SettimaneB.HasValue || !elem.RMSQuotaB.HasValue || string.IsNullOrEmpty(elem.CodiceTipoQuota))
                                {
                                    messaggioVideo = "Per la quota 'B' la 'Reddito'/'Retribuzione media', 'Settimane'/'Giorni' e 'Tipo Quota' sono dati obbligatori.";
                                    return false;
                                }
                            }
                            break;
                    }
                }
                //Controllo 5
                //Verificare che se ci stanno 2 record con Quota = A appartenti al periodo precedente 31.12.1992 (quota = A) allora tali quote dovranno avere lo steso RM
                // Ad eccezione della quota fittizia
                List<DatiRetributiviExInpdai> lstAnte93QuotaA = lstDatiRetrExInpdai.Where(x => x.DecCodGestione == "A" && x.Quota == 'A' && (x.SettimaneA.GetValueOrDefault() != 1
                        || x.RMSQuotaA.GetValueOrDefault() > 0.004M)).ToList();
                if (lstAnte93QuotaA.Count > 1 && (lstAnte93QuotaA[0].RMSQuotaA != lstAnte93QuotaA[1].RMSQuotaA))
                {
                    messaggioVideo = "Se presenti più record per quota 'A' gestione 'A' con periodo precedente al 31-12-1992 devono avere la stessa 'Retribuzione Media'.";
                    return false;
                }
                //Controllo 6
                //Verificare che se ci stanno 2 record con Quota = B appartenti al periodo precedente 31.12.2002 (quota = A) allora tali quote dovranno avere lo steso RM
                List<DatiRetributiviExInpdai> lstAnte2002QuotaBGestioneA = lstDatiRetrExInpdai.Where(x => x.DecCodGestione == "A" && x.Quota == 'B' && !new List<string> { "B4", "B9" }.Contains(x.CodiceTipoQuota)).ToList();
                if (lstAnte2002QuotaBGestioneA.Count > 1)
                {
                    decimal? rms = lstAnte2002QuotaBGestioneA[0].RMSQuotaB;
                    for (int i = 1; i < lstAnte2002QuotaBGestioneA.Count; i++)
                    {
                        if (rms != lstAnte2002QuotaBGestioneA[i].RMSQuotaB)
                        {
                            messaggioVideo = "Se presenti più record per quota 'B' gestione 'A' precedenti al 31-12-2002 devono avere la stessa 'Retribuzione Media Giornaliera'.";
                            return false;
                        }
                    }

                }
                //Controllo 7
                //Non possono essere presenti più di cinque registrazioni di quota B della gestione A (INPDAI) 
                List<DatiRetributiviExInpdai> lstQuotaBGestioneA = lstDatiRetrExInpdai.Where(x => x.DecCodGestione == "A" && x.Quota == 'B').ToList();
                if (lstQuotaBGestioneA.Count > 5)
                {
                    messaggioVideo = "Non possono essere presenti più di 5 registrazioni di quota B della gestione A.";
                    return false;
                }

                DatiRetributiviExInpdai quota76 = lstDatiRetrExInpdai.Where(x => x.DecCodGestione == "A" && x.Quota == 'A' && x.CodiceTipoQuota == "A1").FirstOrDefault();
                if (quota76 != null)
                {
                    if (quota76.SettimaneA > 1 && quota76.RMSQuotaA <= 0.004M)
                    {
                        messaggioVideo = "Attenzione per valori di “RMS / RMG” inferiori o uguali a 0,0040 il numero di “Giorni / Settimane” ammissibili è 1.";
                        return false;
                    }
                }

                var lstSenzaQuotaFittizia = lstDatiRetrExInpdai.Where(x => x.SettimaneA.GetValueOrDefault() + x.SettimaneB.GetValueOrDefault() != 1
                        || x.RMSQuotaA.GetValueOrDefault() + x.RMSQuotaB.GetValueOrDefault() > 0.004M).ToList();

                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO.CONTR_SOLIDARIETA_L_214_2011)
                    && !(Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && !Utility.DataSuccessivaA(decorrenzaOriginaria, new DateTime(1997, 1, 1)) && Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault()) &&
                    !(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && contenitore.DatiPensioniDatiGenerici != null && Utility.DataStrettamenteSuccessivaSenzaGiorno(new DateTime(2003, 12, 01), contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.Value)))
                {
                    if (!Utility.IsDomandaIDAI(contenitore.DatiPensione.SiglaCategoria) &&
                        !Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo : null) &&
                        !Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo != null ? contenitore.DatiBeneficioVittimeTerrorismo : null))
                    {
                        //Controlli max e min Anz95
                        //regola per calcolare il min : Anzianità al 95 deve essere uguale alla somma dei  giorni  delle gestioni MM, 76, 21 e 17 diviso 6,9231
                        decimal minAnz95 = 0;
                        decimal maxAnz95 = 0;
                        decimal minAnz95Comma707 = 0;
                        decimal maxAnz95Comma707 = 0;
                        //calcolo il massimo e minimo per Anz95
                        foreach (var r in lstSenzaQuotaFittizia)
                        {
                            switch (r.DecorrenzaExInpdai)
                            {
                                case 76:
                                case 21:
                                case 17:
                                    maxAnz95 += r.SettimaneA.GetValueOrDefault() + r.SettimaneB.GetValueOrDefault();
                                    maxAnz95Comma707 += r.NSettimane707.GetValueOrDefault();
                                    minAnz95 += r.SettimaneA.GetValueOrDefault() + r.SettimaneB.GetValueOrDefault();
                                    minAnz95Comma707 += r.NSettimane707.GetValueOrDefault();
                                    break;
                                case 31:
                                    if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio == "04")
                                    {
                                        maxAnz95 += (r.SettimaneB < 540) ? r.SettimaneB.Value : 540;
                                        maxAnz95Comma707 += (r.NSettimane707.GetValueOrDefault() < 540) ? r.NSettimane707.GetValueOrDefault() : 540;
                                    }
                                    else
                                    {
                                        maxAnz95 += (r.SettimaneB < 360) ? r.SettimaneB.Value : 360;
                                        maxAnz95Comma707 += (r.NSettimane707.GetValueOrDefault() < 360) ? r.NSettimane707.GetValueOrDefault() : 360;
                                    }
                                    break;
                                case 16:
                                    if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio == "04")
                                    {
                                        if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Misto)
                                        {
                                            maxAnz95 += (r.SettimaneB < 1620) ? r.SettimaneB.Value : 1620;
                                            maxAnz95Comma707 += (r.NSettimane707.GetValueOrDefault() < 1620) ? r.NSettimane707.GetValueOrDefault() : 1620;
                                        }
                                        else if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Retributivo)
                                        {
                                            maxAnz95 += (r.SettimaneB < 5400) ? r.SettimaneB.Value : 5400;
                                            maxAnz95Comma707 += (r.NSettimane707.GetValueOrDefault() < 5400) ? r.NSettimane707.GetValueOrDefault() : 5400;
                                        }
                                    }
                                    else
                                    {
                                        if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Misto)
                                        {
                                            maxAnz95 += (r.SettimaneB < 1080) ? r.SettimaneB.Value : 1080;
                                            maxAnz95Comma707 += (r.NSettimane707.GetValueOrDefault() < 1080) ? r.NSettimane707.GetValueOrDefault() : 1080;
                                        }
                                        else if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Retributivo)
                                        {
                                            maxAnz95 += (r.SettimaneB < 3600) ? r.SettimaneB.Value : 3600;
                                            maxAnz95Comma707 += (r.NSettimane707.GetValueOrDefault() < 3600) ? r.NSettimane707.GetValueOrDefault() : 3600;
                                        }
                                    }
                                    break;
                            }
                        }
                        maxAnz95 = Math.Floor((maxAnz95 / coefNormSettInGG) + 0.99M);
                        maxAnz95Comma707 = Math.Floor((maxAnz95Comma707 / coefNormSettInGG) + 0.99M);
                        minAnz95 = Math.Floor((minAnz95 / coefNormSettInGG) + 0.99M);
                        minAnz95Comma707 = Math.Floor((minAnz95Comma707 / coefNormSettInGG) + 0.99M);

                        //Controllo per il minimo numero di settimane Anz95
                        if (minAnz95 == 0 && minAnz95Comma707 == 0)
                        {
                            // Imposto i limiti al valore minimo consentito
                            minAnz95 = 0.9999M;
                            minAnz95Comma707 = 0.9999M;
                            //Siano in presenza di 1 o più quote fittizie
                            if (anzAl95.GetValueOrDefault() < 0.9999M)
                            {
                                messaggioVideo = "Il minimo valore consentito per 'Anzianità al 95' deve essere 0,9999";
                                return false;
                            }
                        }
                        //Controllo per max numero di settimane Anz95
                        if (maxAnz95 == 0 && maxAnz95Comma707 == 0)
                        {
                            // Imposto i limiti al valore massimo consentito
                            maxAnz95 = 0.9999M;
                            maxAnz95Comma707 = 0.9999M;
                            //siamo in presenza di 1 o più quote fittizie
                            if (anzAl95.GetValueOrDefault() > 0.9999M /*&& datiInpdai.QuotaAl95 != 0.999M*/)
                            {
                                messaggioVideo = "Il massimo valore consentito per 'Anzianità al 95' deve essere 0,9999";
                                return false;
                            }
                            //siamo in presenza di 1 o più quote fittizie
                            if (quotaAl95.GetValueOrDefault() != 0.9999M)
                            {
                                messaggioVideo = "La 'Quota al 95' deve essere 0,9999";
                                return false;
                            }
                        }

                        // Se anzAl95 non rientra in nessuno dei due range
                        if ((anzAl95.GetValueOrDefault() < minAnz95 || anzAl95.GetValueOrDefault() > maxAnz95) && (anzAl95.GetValueOrDefault() < minAnz95Comma707 || anzAl95.GetValueOrDefault() > maxAnz95Comma707))
                        {
                            if (anzAl95.GetValueOrDefault() < minAnz95 || anzAl95.GetValueOrDefault() > maxAnz95)
                            {
                                if (anzAl95.GetValueOrDefault() < minAnz95)
                                {
                                    messaggioVideo = string.Format("Il valore minimo dell’Anzianità al 95' con calcolo standard deve essere {0}", minAnz95);
                                    return false;
                                }
                                if (anzAl95.GetValueOrDefault() > maxAnz95)
                                {
                                    messaggioVideo = string.Format("Il valore massimo dell’Anzianità al 95' con calcolo standard deve essere {0}", maxAnz95);
                                    return false;
                                }
                            }

                            if (anzAl95.GetValueOrDefault() < minAnz95Comma707 || anzAl95.GetValueOrDefault() > maxAnz95Comma707)
                            {
                                if (anzAl95.GetValueOrDefault() < minAnz95Comma707)
                                {
                                    messaggioVideo = string.Format("Il valore minimo dell’Anzianità al 95' con calcolo 707 deve essere {0}", minAnz95Comma707);
                                    return false;
                                }
                                if (anzAl95.GetValueOrDefault() > maxAnz95Comma707)
                                {
                                    messaggioVideo = string.Format("Il valore massimo dell’Anzianità al 95' con calcolo 707 deve essere {0}", maxAnz95Comma707);
                                    return false;
                                }
                            }
                        }

                        //(Oggetto mail 20151118: LIQPENS - analisi DAI)
                        //Controllo minimo valore inseribile per 'Quota al 95' deve essere 0,01
                        if (quotaAl95 < 0.01M)
                        {
                            messaggioVideo = "Il minimo valore inseribile per 'Quote al 95' è 0,01";
                            return false;
                        }
                    }
                }

                //(Oggetto mail 20151118: LIQPENS - analisi DAI)
                //Controllo massimo inseribile per le differenti decorrenze: per 16,17,76,61,71 max=14400 per 21,31 max=720 per 41 max=2160 per 51,91 max=2080
                if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico != null && Utility.DataStrettamenteSuccessivaSenzaGiorno(new DateTime(2003, 12, 01), contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.Value)))
                {
                    foreach (var elem in lstDatiRetrExInpdai)
                    {
                        int maxValue = int.MaxValue;
                        int ggMMForRecord = elem.SettimaneA.GetValueOrDefault() + elem.SettimaneB.GetValueOrDefault();
                        switch (elem.DecorrenzaExInpdai)
                        {
                            case 16:
                                if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio == "04")
                                {
                                    if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Misto)
                                        maxValue = 1620;
                                    else if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Retributivo)
                                        maxValue = 5400;
                                }
                                else
                                {
                                    if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Misto)
                                        maxValue = 1080;
                                    else if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Retributivo)
                                        maxValue = 3600;
                                }
                                break;
                            case 17:
                            case 76:
                            case 61:
                            case 71:
                                maxValue = 14400;
                                break;
                            case 21:
                            case 31:
                                if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio == "04")
                                    maxValue = 1080;
                                else
                                    maxValue = 720;
                                break;
                            case 41:
                                if (!string.IsNullOrEmpty(tipoSettimaneBeneficio) && tipoSettimaneBeneficio == "04")
                                    maxValue = 3240;
                                else
                                    maxValue = 2160;
                                break;
                            case 51:
                            case 91:
                                maxValue = 2080;
                                break;
                        }
                        if (ggMMForRecord > maxValue)
                        {
                            messaggioVideo = string.Format("Il valore inserito ({0}) per la decorrenza {1}  supera il massimo valore consentito ({2}).", ggMMForRecord.ToString(), elem.DecorrenzaExInpdai.ToString(), maxValue.ToString());
                            return false;
                        }
                    }
                }
                //(Oggetto mail 20151118: LIQPENS - analisi DAI)
                //Controllo sulla somma dei gg inseribili per i retributivi calcolati nel seguente modo: somma = (76+21+31)/0.75 + [(51+91)*6.9231] + (41+16+17+71+61) 
                decimal somma = 0;
                foreach (var elem in lstSenzaQuotaFittizia)
                {
                    int ggMMForRecord = elem.SettimaneA.GetValueOrDefault() + elem.SettimaneB.GetValueOrDefault();
                    switch (elem.DecorrenzaExInpdai)
                    {
                        case 76:
                        case 21:
                            somma += Math.Round(ggMMForRecord / 0.75M);
                            break;
                        case 51:
                        case 91:
                            somma += Math.Round(ggMMForRecord * coefNormSettInGG);
                            break;
                        case 31:
                        case 41:
                        case 16:
                        case 17:
                        case 71:
                        case 61:
                            somma += ggMMForRecord;
                            break;
                    }
                }
                //ENG - creato bypass per il messaggio "La somma dei giorni gg supera il massimo valore consentito 14400. Controllare le informazioni memorizzate ed eventualmente variarle con una Ricostituzione contributiva"
                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO.SOMMA_GIORNI_SUP_14400) &&
                        !(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.HasValue && Utility.DataStrettamenteSuccessivaSenzaGiorno(new DateTime(2003, 12, 01), contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.Value)))
                {
                    if (somma > maxGGConsentiti)
                    {
                        messaggioVideo = string.Format("La somma dei giorni ({0}) supera il massimo valore consentito 14400.", somma.ToString("#.00"));
                        return false;
                    }
                }
            }
            #endregion Controlli Dati Retributivi

            #region Controlli Dati Contributivi
            if (lDatiContributivi != null && lDatiContributivi.Count > 0)
            {
                //Controllo 1
                //NSettimane non può essere inserito un  valore superiore a 2600 
                if (lDatiContributivi.Where(x => (x.Settimane > 2600 || x.SettimaneQuotaD > 2600)).Count() > 0)
                {
                    messaggioVideo = "Il numero di settimane per la quota contributiva non può essere superiore a 2600.";
                    return false;
                }

                if (contenitore.TipoCalcolo == Utility.TipoCalcolo.Contributivo)
                {
                    GestioneDecodifica.CodeGestioneCalcoloContributivo gestione = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim() == "A");

                    if (lDatiContributivi.FindAll(x => x.CodGestione == gestione.Id && x.Quota == 'C').Count == 0)
                    {
                        messaggioVideo = "E' obbligatorio inserire una registrazione con gestione 'A - INPDAI' e quota 'C'.";
                        return false;
                    }
                }
            }
            #endregion Controlli Dati Contributivi

            return true;
        }


        private static bool ControlsDatiCalcoloAUT(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, bool? facoltaComputo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!Utility.IsDomandaAUT(contenitore.DatiPensione))
                return true;

            if (lDatiContributivi == null || lDatiContributivi.Count == 0)
            {
                messaggioVideo = "Salvare almeno un record nella tabella dati contributivi.";
                return false;
            }

            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2021 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out ctrlMemo123_2021);

            var lstRecordCodGestione = from Record in lDatiContributivi
                                       join dec in contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo on Record.CodGestione equals dec.Id
                                       select new { Record, TraduzioneSuGP = dec.TraduzioneSuGP.Trim() };

            //1 Primo record deve essere G per le PL
            if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !Utility.IsDomandaRipristinoOrRiliquidazione(contenitore.DatiPensione) && lstRecordCodGestione.First().TraduzioneSuGP != "G")
            {
                messaggioVideo = "Per le categorie VOAUT, SOAUT, IOAUT il codice gestione del primo record deve essere 'G'.";
                return false;
            }

            //Deve essere presente almeno un record con G per le TFR/RIC + Ripristini/Riliq
            if ((Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaRipristinoOrRiliquidazione(contenitore.DatiPensione)) && lstRecordCodGestione.Where(x => x.TraduzioneSuGP == "G").Count() == 0)
            {
                messaggioVideo = "Per le categorie VOAUT, SOAUT, IOAUT il codice gestione 'G' deve essere presente.";
                return false;
            }

            if (lstRecordCodGestione.Any(x => x.Record.Settimane > 2600 || x.Record.SettimaneQuotaD > 2600))
            {
                messaggioVideo = "Per le categorie VOAUT, SOAUT, IOAUT il numero di settimane non può essere superiore a 2600.";
                return false;
            }

            if (!Utility.IsDomandaAutomatica(contenitore.DatiPensione) && Utility.IsDomandaVOAUT(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) &&
                !Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione) && !Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione))
            {
                if (contenitore.DatiPensione.InizioAssicurazione.HasValue && contenitore.DatiPensione.InizioAssicurazione.Value.Year <= 2011)
                {
                    if (!lstRecordCodGestione.Any(x => x.Record.Quota == 'C'))
                    {
                        messaggioVideo = "Per le domande VOAUT manuali con Inizio assicurazione minore uguale al 2011 deve essere presente almeno una registrazione con quota C";
                        return false;
                    }
                }
            }

            if (Utility.IsDomandaAnzianitaInComputo(contenitore.DatiPensione) || Utility.IsDomandaVecchiaiaInComputo(contenitore.DatiPensione))
            {
                if (!lstRecordCodGestione.Any(x => x.Record.Quota == 'C' && x.TraduzioneSuGP != "G"))
                {
                    messaggioVideo = "Per le domande VOAUT in computo deve essere presente almeno una registrazione con quota C con codice gestione diverso da 'G'";
                    return false;
                }
            }

            if (facoltaComputo.GetValueOrDefault())
            {
                if (!lstRecordCodGestione.Any(x => x.TraduzioneSuGP.Trim() != "G") && !Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione))
                {
                    messaggioVideo = "Per le categorie VOAUT, SOAUT, IOAUT è necessario che sia presente almeno una registrazione contributiva dei dati calcolo con codice gestione diverso da 'G'.";
                    return false;
                }

                //ENG - Il codice F0 deve essere abilitato per le domande AUT 
                List<string> lstCodiciAmmessi = new List<string> { "G", "C1", "C2", "C3", "C4", "C5", "D1", "E1", "E2", "A5", "A6", "A7", "A8", "A9", "B1", "B2", "B3", "B4", "1", "2", "3", "4" };
                //ENG - Aggiornamento Memo 123_2021
                if (ctrlMemo123_2021 != null && !String.IsNullOrEmpty(ctrlMemo123_2021.ValoreControllo) && !String.IsNullOrEmpty(ctrlMemo123_2021.ValoreControllo.Trim()) && ctrlMemo123_2021.ValoreControllo.Trim() == "SI")
                    lstCodiciAmmessi.Add("F0");

                //if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                //{
                //    lstCodiciAmmessi.Add("F0");
                //    lstCodiciAmmessi.Add("F1");
                //}
                List<string> records = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !lstCodiciAmmessi.Contains(x)).ToList();
                if (records != null && records.Count > 0)
                {
                    messaggioVideo = string.Format("Se il campo 'Facoltà di Computo' è uguale a SI i codici {0} non sono ammessi.", records.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;

                }

                int codiciCount = 0;
                //verifica coerenza codici Cx
                List<string> lstCodiciVerifyC = new List<string> { "C1", "C2", "C3", "C4", "C5" };
                var resC = lstRecordCodGestione.GroupBy(x => new { x.TraduzioneSuGP, x.Record.Quota }).Select(n => new { n.Key, Num = n.Count() }).Where(x => lstCodiciVerifyC.Contains(x.Key.TraduzioneSuGP)).ToList();

                if (resC.Any(c => c.Num > 1))
                {
                    messaggioVideo = string.Format("Non è possibile inserire quote duplicate per i codici {0}", lstCodiciVerifyC.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
                codiciCount = resC.Select(x => x.Key.TraduzioneSuGP).Intersect(lstCodiciVerifyC).Count();
                if (codiciCount > 1)
                {
                    messaggioVideo = string.Format("La doppia registrazione di tipo 'C' non è ammessa.");
                    return false;
                }

                //verifica coerenza codici Ex
                List<string> lstCodiciVerifyE = new List<string> { "E1", "E2" };
                var resE = lstRecordCodGestione.GroupBy(x => new { x.TraduzioneSuGP, x.Record.Quota }).Select(n => new { n.Key, Num = n.Count() }).Where(x => lstCodiciVerifyE.Contains(x.Key.TraduzioneSuGP)).ToList();

                if (resE.Any(c => c.Num > 1))
                {
                    messaggioVideo = string.Format("Non è possibile inserire quote duplicate per i codici {0}", lstCodiciVerifyE.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
                codiciCount = resE.Select(x => x.Key.TraduzioneSuGP).Intersect(lstCodiciVerifyE).Count();
                if (codiciCount > 1)
                {
                    messaggioVideo = string.Format("La doppia registrazione di tipo 'E' non è ammessa.");
                    return false;
                }

            }
            if (facoltaComputo.HasValue && !facoltaComputo.Value)
            {
                //2. Somma num settimane di tutti i record con gestione G deve essere maggiore o uguale a 260
                //int sommaNumSettPerCodGestG = (from elem in lstRecordCodGestione
                //                               where elem.TraduzioneSuGP == "G"
                //                               select (elem.Record.Settimane.GetValueOrDefault() + elem.Record.SettimaneQuotaD.GetValueOrDefault())).Aggregate((a, b) => a + b);
                //if (sommaNumSettPerCodGestG < 260)
                //{
                //    messaggioVideo = "Per le categorie VOAUT, SOAUT, IOAUT la somma del numero di settimane di tutti i record con codice gestione uguale G deve essere maggiore uguale a 260";
                //    return false;
                //}
                List<string> lstCodiciAmmessi = new List<string> { "G" };
                //if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                //{
                //    lstCodiciAmmessi.Add("F0");
                //    lstCodiciAmmessi.Add("F1");
                //}

                List<string> records = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !lstCodiciAmmessi.Contains(x)).ToList();
                if (records != null && records.Count > 0)
                {
                    messaggioVideo = string.Format("Se il campo 'Facoltà di Computo' è uguale a NO i codici {0} non sono ammessi.", records.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }

                //if (lstRecordCodGestione.Any(x => x.TraduzioneSuGP.Trim() != "G") )
                //{
                //    messaggioVideo = string.Format("Se il campo 'Facoltà di Computo' è uguale a NO i codici {0} non sono ammessi.", 
                //        lstRecordCodGestione.Where(x => x.TraduzioneSuGP.Trim() != "G").Select(x => x.TraduzioneSuGP.Trim()).Aggregate((a, b) => { return a + ", " + b; }));
                //    return false;
                //}
            }

            var lDatiContributiviBL = MappingDatiContributiviFromViewToBL(ref contenitoreDecodifica, lDatiContributivi, contenitore.DatiPensione, contenitore.DatiDanteCausa);
            if (!GestioneControlli.ControlsInizioAssicurazioneAUT(ref contenitoreDecodifica, contenitore.DatiPensione, lDatiContributiviBL, facoltaComputo, contenitore.DatiPensione.InizioAssicurazione,
                contenitore.DatiPensione.NaturaPensione, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : null, out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsDatiCalcolo_IOCUM_SOCUM(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
           List<GestioneCalcolo.QuotePensione> lQuotePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            var categoria = contenitore.DatiPensione.SiglaCategoria;

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo93 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo93", out ctrlAbilitazioneMemo93);

            if (!Utility.IsDomandaIOCUM(categoria) && !Utility.IsDomandaSOCUM(categoria))
                return true;

            if (contenitore.DatiPensioniDatiGenerici == null || contenitore.DatiPensioniDatiGenerici.TipoCumulo == null)
            {
                messaggioVideo = "Salvare il Tipo Cumulo nel quadro Liquidazione";
                return false;
            }

            //if (lDatiContributivi == null || lDatiContributivi.Count == 0)
            //{
            //    messaggioVideo = "Salvare almeno un record nella tabella dati contributivi.";
            //    return false;
            //}

            if (lQuotePensione.Sum(x => x.Settimane) > 5000)
            {
                messaggioVideo = "La somma delle settimane non può essere superiore a 5000.";
                return false;
            }

            var listaDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;
            if (listaDecEnteGestioneFondo == null)
                GestioneDecodifica.GetDecEnteGestioneFondo(out listaDecEnteGestioneFondo);

            var lstRecordCodGestione = from Record in lQuotePensione
                                       join dec in listaDecEnteGestioneFondo on Record.EnteGestioneFondo equals dec.Id
                                       select new { Record, TraduzioneSuGP = dec.Codice.Trim() };

            List<string> lstCodiciInterni = new List<string> { "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "B1", "B2", "B4", "B6", "C1", "C2", "C3", "C4", "C5", "D1", "E1", "E2" };
            List<string> lstCodiciEsterni = new List<string> { "F0", "F1", "G0", "H0", "I0", "J0", "K0", "L0", "N0", "O0", "P0", "Q0", "R0", "S0", "T0", "U0", "V0", "Z0", "Z1", "PR" };

            if (ctrlAbilitazioneMemo93 != null && !String.IsNullOrEmpty(ctrlAbilitazioneMemo93.ValoreControllo) && ctrlAbilitazioneMemo93.ValoreControllo.ToUpperInvariant().Trim() == "SI")
            {
                lstCodiciInterni.Add("F0");
                lstCodiciEsterni.Remove("F0");
                lstCodiciEsterni.Add("SI");
            }

            //Per le sole domande di categoria IOCUM e SOCUM se il tipo cumulo è esterno allora nei dati di calcolo dovrà essere presente almeno una delle gestioni seguenti: G0 H0 I0 J0 K0 L0 N0 O0 P0 Q0 R0 S0 T0 U0 V0 Z0 Z1.
            if (!contenitore.DatiPensioniDatiGenerici.TipoCumulo.GetValueOrDefault())
            {
                if (contenitore.DatiPensioniDatiGenerici.EnteCassa.HasValue)
                {
                    var idEnteCassa = contenitore.DatiPensioniDatiGenerici.EnteCassa.Value;
                    var enteCassa = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Where(x => x.Id == idEnteCassa).Select(x => x.TraduzioneSuGP).FirstOrDefault();
                    enteCassa = !string.IsNullOrEmpty(enteCassa) ? enteCassa.ToString().PadLeft(4, '0') : enteCassa;
                    //Per le sole domande di categoria IOCUM e SOCUM se il tipo cumulo è esterno e se l’ “Ente / Cassa” è 0801 allora nei dati di calcolo dovrà essere presente almeno una delle gestioni seguenti(interne): A1, A2, A3, A4, A5, A6, A7, A8, A9, B1, B2, B4, B6, C1, C2, C3, C4, C5, D1, E1, E2.
                    if (enteCassa == "0801")
                    {
                        List<string> records_1 = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciInterni.Contains(x)).ToList();
                        if (records_1 == null || records_1.Count == 0)
                        {
                            messaggioVideo = string.Format("Se il campo 'TipoCumulo' è uguale a ESTERNO ed 'Ente Cassa' è {1}, deve essere presente almeno una delle seguenti gestioni: {0} ", lstCodiciInterni.Aggregate((a, b) => { return a + ", " + b; }), enteCassa);
                            return false;
                        }
                    }
                    else
                    {
                        if (contenitoreDecodifica.ElencoCtrlEnteCassaCodiceGestione != null)
                        {
                            var codiciGestione = contenitoreDecodifica.ElencoCtrlEnteCassaCodiceGestione.Where(x => x.CodiceCategoria == categoria.Trim() && x.TraduzioneSuGP == enteCassa).FirstOrDefault();
                            if (codiciGestione != null)
                            {
                                var lstCodiciGestione = codiciGestione.CodiciGestione.Split(';').ToList();
                                List<string> records_2 = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciGestione.Contains(x)).ToList();
                                if (records_2 == null || records_2.Count == 0)
                                {
                                    messaggioVideo = string.Format("Se il campo 'TipoCumulo' è uguale a ESTERNO ed 'Ente Cassa' è {1} deve essere presente almeno una delle seguenti gestioni: {0} ", lstCodiciGestione.Aggregate((a, b) => { return a + ", " + b; }), enteCassa);
                                    return false;
                                }
                            }
                        }
                    }
                }

                List<string> records = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciEsterni.Contains(x)).ToList();
                if (records == null || records.Count == 0)
                {
                    messaggioVideo = string.Format("Se il campo 'TipoCumulo' è uguale a ESTERNO deve essere presente almeno una delle seguenti gestioni: {0} ", lstCodiciEsterni.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
                else
                {
                    //v.	Per le sole domande di categoria IOCUM se il tipo cumulo è esterno (in modo equivalente se è presente almeno una delle quote F0, F1, G0 H0 I0 J0 K0 L0 N0 O0 P0 Q0 R0 S0 T0 U0 V0 Z0 Z1 PR) allora occorre inserire un controllo che verifichi che la decorrenza pensione della domande sia strettamente successiva ad un certo valore.
                    if (Utility.IsDomandaIOCUM(categoria) && contenitore.DatiPensione != null &&
                        contenitore.DatiPensione.DecorrenzaOriginaria != null)
                    {
                        //Se esiste la gestione PR, deve essere successiva al  01/01/2013
                        if (lstRecordCodGestione.Where(x => x.TraduzioneSuGP == "PR").FirstOrDefault() != null && !Utility.DataSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, new DateTime(2013, 03, 01)))
                        {
                            messaggioVideo = "La decorrenza pensione deve essere successiva al 02/2013 se è presente la gestione PR";
                            return false;
                        }
                        //Se esiste almeno una gestione diversa da PR, deve essere successiva al  01/01/2017
                        if (lstRecordCodGestione.Where(x => x.TraduzioneSuGP != "PR" && lstCodiciEsterni.Contains(x.TraduzioneSuGP)).FirstOrDefault() != null && !Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 31)))
                        {
                            messaggioVideo = string.Format("La decorrenza pensione deve essere successiva al 01/2017 se è presente almeno una delle seguenti gestioni: {0} ", (lstCodiciEsterni.Where(x => x != "PR")).Aggregate((a, b) => { return a + ", " + b; }));
                            return false;
                        }
                    }
                }
            }
            //Per le sole domande di categoria IOCUM e SOCUM se il tipo cumulo è interno allora nei dati di calcolo dovrà essere presente almeno due delle gestioni seguenti: A1, A2, A3, A4, A5, A6, A7, A8, A9, B1, B2, B4, B6, C1, C2, C3, C4, C5, D1, E1, E2
            else
            {
                List<string> records = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciInterni.Contains(x)).ToList();
                if (records == null || records.Count < 2)
                {
                    messaggioVideo = string.Format("Se il campo 'TipoCumulo' è uguale a INTERNO devono essere presenti almeno due delle seguenti gestioni: {0}", lstCodiciInterni.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
                //Per le sole domande di categoria IOCUM e SOCUM se il tipo cumulo è interno allora nei dati di calcolo non dovrà essere presente nessuna delle gestioni seguenti(esterne): G0 H0 I0 J0 K0 L0 N0 O0 P0 Q0 R0 S0 T0 U0 V0 Z0 Z1
                List<string> recordsNonAmmessi = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciEsterni.Contains(x)).ToList();
                if (recordsNonAmmessi != null && recordsNonAmmessi.Count > 0)
                {
                    messaggioVideo = string.Format("Se il campo 'TipoCumulo' è uguale a INTERNO non possono essere presenti le seguenti gestioni: {0}", lstCodiciEsterni.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
            }

            //Se il tipo cumulo è interno allora non possono essere presenti solamente gestione AGO (A1, A2, A3, A4, A5 ed F0) -> Vale solo per  SOCUM
            if (Utility.IsDomandaSOCUM(categoria) && ctrlAbilitazioneMemo93 != null && !String.IsNullOrEmpty(ctrlAbilitazioneMemo93.ValoreControllo) && ctrlAbilitazioneMemo93.ValoreControllo.ToUpperInvariant() == "SI" && contenitore.DatiPensioniDatiGenerici.TipoCumulo.GetValueOrDefault() && contenitore.DatiPensione != null && contenitore.DatiPensione.NaturaPensione != null && contenitore.DatiPensione.NaturaPensione.Substring(0, 1) != "3" && contenitore.DatiPensione.NaturaPensione.Substring(0, 1) != "4")
            {
                List<string> listaCodiciFondoAgo = new List<string> { "A1", "A2", "A3", "A4", "A5", "F0" };
                List<string> listaCodiciFondoNoAgo = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !listaCodiciFondoAgo.Contains(x)).ToList();
                if (!(lstRecordCodGestione.Any(x => x.TraduzioneSuGP == "F0")) && (listaCodiciFondoNoAgo == null || listaCodiciFondoNoAgo.Count == 0))
                {
                    messaggioVideo = string.Format("Manca una gestione diversa da {0}", listaCodiciFondoAgo.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
            }

            if (Utility.IsDomandaIOCUM(categoria) && contenitore.DatiPensioniDatiGenerici.TipoCumulo.GetValueOrDefault() &&
                !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO.CONTROLLO_QUOTE_CUMULO) &&
                ((Utility.IsDomandaPensioneOrdinariaDiInabilita(contenitore.DatiPensione) && contenitore.DatiPensione.NaturaPensione != null && (contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "3" || contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "4"))
                || contenitore.DatiPensioniDatiGenerici.TipologiaCumulo == 'C'))
            {
                //interno
                if (contenitore.DatiPensioniDatiGenerici.TipoCumulo.GetValueOrDefault())
                {
                    List<string> listaCodiciFondoAgoGruppo1 = new List<string> { "A1", "A5", "F0" };
                    List<string> listaCodiciFondoNoAgo1 = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !listaCodiciFondoAgoGruppo1.Contains(x)).ToList();
                    if (listaCodiciFondoNoAgo1 == null || listaCodiciFondoNoAgo1.Count == 0)
                    {
                        messaggioVideo = string.Format("Attenzione è necessaria almeno una gestione interna diversa da {0} (FPLD, EX INPDAI, GIORNALISTI DIPENDENTI)", listaCodiciFondoAgoGruppo1.Aggregate((a, b) => { return a + ", " + b; }));
                        return false;
                    }

                    List<string> listaCodiciFondoAgoGruppo2 = new List<string> { "A2", "A3", "A4" };
                    List<string> listaCodiciFondoNoAgo2 = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !listaCodiciFondoAgoGruppo2.Contains(x)).ToList();
                    if (listaCodiciFondoNoAgo2 == null || listaCodiciFondoNoAgo2.Count == 0)
                    {
                        messaggioVideo = string.Format("Attenzione è necessaria almeno una gestione interna diversa da {0} ( CD/CM, ART,  COMM)", listaCodiciFondoAgoGruppo2.Aggregate((a, b) => { return a + ", " + b; }));
                        return false;
                    }
                }
                //esterno
                else
                {
                    List<string> listaCodiciFondoNoAgo = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !lstCodiciInterni.Contains(x)).ToList();
                    if (listaCodiciFondoNoAgo == null || listaCodiciFondoNoAgo.Count == 0)
                    {
                        messaggioVideo = "Attenzione è necessaria almeno una gestione esterna";
                        return false;
                    }
                }


            }

            //Per le sole domande di categoria IOCUM o SOCUM che abbiano il terzo byte del codice natura G è necessario verificare che sia presente almeno una fra le due quote A1 o A5.
            if (contenitore.DatiPensione != null && contenitore.DatiPensione.NaturaPensione != null && contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "G")
            {
                List<string> lstCodiciVerifyNatura = new List<string> { "A1", "A5" };
                List<string> recNat = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciVerifyNatura.Contains(x)).ToList();
                if (recNat == null || recNat.Count == 0)
                {
                    messaggioVideo = string.Format("Se terzo byte del codice natura è 'G' deve essere presente almeno una delle seguenti gestioni: {0} ", lstCodiciVerifyNatura.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
            }

            //Per le sole domande di categoria IOCUM e SOCUM valgono le regole di incompatibilità delle gestioni Cx / Ex (come per le AUT):
            int codiciCount = 0;
            //verifica coerenza codici Cx
            List<string> lstCodiciVerifyC = new List<string> { "C1", "C2", "C3", "C4", "C5" };
            var resC = lstRecordCodGestione.GroupBy(x => new { x.TraduzioneSuGP, x.Record.EnteGestioneFondo }).Select(n => new { n.Key, Num = n.Count() }).Where(x => lstCodiciVerifyC.Contains(x.Key.TraduzioneSuGP)).ToList();

            if (resC.Any(c => c.Num > 1))
            {
                messaggioVideo = string.Format("Non è possibile inserire quote duplicate per i codici {0}", lstCodiciVerifyC.Aggregate((a, b) => { return a + ", " + b; }));
                return false;
            }
            codiciCount = resC.Select(x => x.Key.TraduzioneSuGP).Intersect(lstCodiciVerifyC).Count();
            if (codiciCount > 1)
            {
                messaggioVideo = string.Format("La doppia registrazione di tipo 'C' non è ammessa.");
                return false;
            }

            //verifica coerenza codici Ex
            List<string> lstCodiciVerifyE = new List<string> { "E1", "E2" };
            var resE = lstRecordCodGestione.GroupBy(x => new { x.TraduzioneSuGP, x.Record.EnteGestioneFondo }).Select(n => new { n.Key, Num = n.Count() }).Where(x => lstCodiciVerifyE.Contains(x.Key.TraduzioneSuGP)).ToList();

            if (resE.Any(c => c.Num > 1))
            {
                messaggioVideo = string.Format("Non è possibile inserire quote duplicate per i codici {0}", lstCodiciVerifyE.Aggregate((a, b) => { return a + ", " + b; }));
                return false;
            }
            codiciCount = resE.Select(x => x.Key.TraduzioneSuGP).Intersect(lstCodiciVerifyE).Count();
            if (codiciCount > 1)
            {
                messaggioVideo = string.Format("La doppia registrazione di tipo 'E' non è ammessa.");
                return false;
            }

            return true;
        }

        private static bool ControlsDatiCalcolo_TOT(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
         List<GestioneCalcolo.QuotePensione> lQuotePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            var categoria = contenitore.DatiPensione.SiglaCategoria;

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo93 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo93", out ctrlAbilitazioneMemo93);

            if (!Utility.IsDomandaTotalizzazione(categoria))
                return true;

            if (lQuotePensione.Sum(x => x.Settimane) > 5000 && !Utility.IsDomandaVOTOT(categoria))
            {
                messaggioVideo = "La somma delle settimane non può essere superiore a 5000";
                return false;
            }

            var listaDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;
            if (listaDecEnteGestioneFondo == null)
                GestioneDecodifica.GetDecEnteGestioneFondo(out listaDecEnteGestioneFondo);

            var lstRecordCodGestione = from Record in lQuotePensione
                                       join dec in listaDecEnteGestioneFondo on Record.EnteGestioneFondo equals dec.Id
                                       select new { Record, TraduzioneSuGP = dec.Codice.Trim() };

            List<string> lstCodiciAmmessi = new List<string> { "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "B1", "B2", "B3", "B4", "B5", "B6", "C1", "C2", "C3", "C4", "C5", "D1", "E1", "E2", "F0", "F1", "G0", "H0", "I0", "J0", "K0", "L0", "N0", "O0", "P0", "Q0", "R0", "S0", "SP", "T0", "U0", "V0", "Z0", "Z1", "PR" };
            List<string> lstCodiciInterni = new List<string> { "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "B1", "B2", "B4", "B5", "B6", "C1", "C2", "C3", "C4", "C5", "D1", "E1", "E2", "SP" };
            if (ctrlAbilitazioneMemo93 != null && !String.IsNullOrEmpty(ctrlAbilitazioneMemo93.ValoreControllo) && ctrlAbilitazioneMemo93.ValoreControllo.ToUpperInvariant().Trim() == "SI")
                lstCodiciInterni.Add("F0");

            List<string> lstCodiciA = new List<string> { "A1", "A2", "A3", "A4", "A5" };
            if (ctrlAbilitazioneMemo93 != null && !String.IsNullOrEmpty(ctrlAbilitazioneMemo93.ValoreControllo) && ctrlAbilitazioneMemo93.ValoreControllo.ToUpperInvariant() == "SI")
                lstCodiciA.Add("F0");
            List<string> records = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !lstCodiciAmmessi.Contains(x)).ToList();
            if (records == null || records.Count > 0)
            {
                messaggioVideo = string.Format("Sono ammesse le seguenti gestioni: {0}", lstCodiciAmmessi.Aggregate((a, b) => { return a + ", " + b; }));
                return false;

            }
            List<string> records_A = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => !lstCodiciA.Contains(x)).ToList();


            if (!(lstRecordCodGestione.Any(x => x.TraduzioneSuGP == "F0")) && (records_A == null || records_A.Count == 0))
            {
                messaggioVideo = string.Format("Manca una gestione diversa da {0}", lstCodiciA.Aggregate((a, b) => { return a + ", " + b; }));
                return false;
            }

            if (Utility.IsDomandaIOTOT(categoria) || Utility.IsDomandaSOTOT(categoria))
            {
                if (contenitore.DatiPensioniDatiGenerici.EnteCassa.HasValue)
                {
                    var idEnteCassa = contenitore.DatiPensioniDatiGenerici.EnteCassa.Value;
                    var enteCassa = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Where(x => x.Id == idEnteCassa).Select(x => x.TraduzioneSuGP).FirstOrDefault();
                    enteCassa = !string.IsNullOrEmpty(enteCassa) ? enteCassa.ToString().PadLeft(4, '0') : enteCassa;
                    //Per le sole domande di categoria IOTOT e SOTOT se l’ “Ente / Cassa” è 0801 allora nei dati di calcolo dovrà essere presente almeno una delle gestioni seguenti(interne): A1, A2, A3, A4, A5, A6, A7, A8, A9, B1, B2, B4, B6, C1, C2, C3, C4, C5, D1, E1, E2.
                    if (enteCassa == "0801")
                    {
                        List<string> records_1 = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciInterni.Contains(x)).ToList();
                        if (records_1 == null || records_1.Count == 0)
                        {
                            messaggioVideo = string.Format("Se il campo 'Ente Cassa' è {1}, deve essere presente almeno una delle seguenti gestioni: {0} ", lstCodiciInterni.Aggregate((a, b) => { return a + ", " + b; }), enteCassa);
                            return false;
                        }
                    }
                    else
                    {
                        if (contenitoreDecodifica.ElencoCtrlEnteCassaCodiceGestione != null)
                        {
                            var codiciGestione = contenitoreDecodifica.ElencoCtrlEnteCassaCodiceGestione.Where(x => x.CodiceCategoria == categoria.Trim() && x.TraduzioneSuGP == enteCassa).FirstOrDefault();
                            if (codiciGestione != null)
                            {
                                var lstCodiciGestione = codiciGestione.CodiciGestione.Split(';').ToList();
                                List<string> records_2 = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciGestione.Contains(x)).ToList();
                                if (records_2 == null || records_2.Count == 0)
                                {
                                    messaggioVideo = string.Format("Se il campo 'Ente Cassa' è {1} deve essere presente almeno una delle seguenti gestioni: {0} ", lstCodiciGestione.Aggregate((a, b) => { return a + ", " + b; }), enteCassa);
                                    return false;
                                }
                            }
                        }
                    }
                }


                //Per le sole domande di categoria IOTOT o SOTOT che abbiano il terzo byte del codice natura G è necessario verificare che sia presente almeno una fra le due quote A1 o A5.
                if (contenitore.DatiPensione != null && contenitore.DatiPensione.NaturaPensione != null && contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "G")
                {
                    List<string> lstCodiciVerifyNatura = new List<string> { "A1", "A5" };
                    List<string> recNat = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciVerifyNatura.Contains(x)).ToList();
                    if (recNat == null || recNat.Count == 0)
                    {
                        messaggioVideo = string.Format("Se terzo byte del codice natura è 'G' deve essere presente almeno una delle seguenti gestioni: {0} ", lstCodiciVerifyNatura.Aggregate((a, b) => { return a + ", " + b; }));
                        return false;
                    }
                }

                //Per le sole domande di categoria IOTOT o SOTOT che abbiano la trattenuta INPDAP a SI è necessario che sia presente l’ente gestione A1
                if (contenitore.DatiPagamento != null && contenitore.DatiPagamento.TrattenutaInpdap.GetValueOrDefault() == true)
                {
                    List<string> lstCodiciVerifyNatura = new List<string> { "A1" };
                    List<string> recNat = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciVerifyNatura.Contains(x)).ToList();
                    if (recNat == null || recNat.Count == 0)
                    {
                        messaggioVideo = string.Format("Se la trattenuta Fondo Credito è uguale a SI, deve essere presente la gestione: {0} ", lstCodiciVerifyNatura.Aggregate((a, b) => { return a + ", " + b; }));
                        return false;
                    }
                }

                //Verifica doppie registrazioni C ed E
                int codiciCount = 0;
                //verifica coerenza codici Cx
                List<string> lstCodiciVerifyC = new List<string> { "C1", "C2", "C3", "C4", "C5" };
                var resC = lstRecordCodGestione.GroupBy(x => new { x.TraduzioneSuGP, x.Record.EnteGestioneFondo }).Select(n => new { n.Key, Num = n.Count() }).Where(x => lstCodiciVerifyC.Contains(x.Key.TraduzioneSuGP)).ToList();

                if (resC.Any(c => c.Num > 1))
                {
                    messaggioVideo = string.Format("Non è possibile inserire quote duplicate per i codici {0}", lstCodiciVerifyC.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
                codiciCount = resC.Select(x => x.Key.TraduzioneSuGP).Intersect(lstCodiciVerifyC).Count();
                if (codiciCount > 1)
                {
                    messaggioVideo = string.Format("La doppia registrazione di tipo 'C' non è ammessa.");
                    return false;
                }

                //verifica coerenza codici Ex
                List<string> lstCodiciVerifyE = new List<string> { "E1", "E2" };
                var resE = lstRecordCodGestione.GroupBy(x => new { x.TraduzioneSuGP, x.Record.EnteGestioneFondo }).Select(n => new { n.Key, Num = n.Count() }).Where(x => lstCodiciVerifyE.Contains(x.Key.TraduzioneSuGP)).ToList();

                if (resE.Any(c => c.Num > 1))
                {
                    messaggioVideo = string.Format("Non è possibile inserire quote duplicate per i codici {0}", lstCodiciVerifyE.Aggregate((a, b) => { return a + ", " + b; }));
                    return false;
                }
                codiciCount = resE.Select(x => x.Key.TraduzioneSuGP).Intersect(lstCodiciVerifyE).Count();
                if (codiciCount > 1)
                {
                    messaggioVideo = string.Format("La doppia registrazione di tipo 'E' non è ammessa.");
                    return false;
                }

            }
            return true;
        }


        private static bool ControlsDatiCalcoloGestioniVecchie_CUM(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
         List<GestioneCalcolo.QuotePensione> lQuotePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            var categoria = contenitore.DatiPensione.SiglaCategoria;
            if (!Utility.IsDomandaCumulo(categoria) && !Utility.IsDomandaTotalizzazione(categoria))
                return true;

            var listaDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;
            if (listaDecEnteGestioneFondo == null)
                GestioneDecodifica.GetDecEnteGestioneFondo(out listaDecEnteGestioneFondo);

            var lstRecordCodGestione = from Record in lQuotePensione
                                       join dec in listaDecEnteGestioneFondo on Record.EnteGestioneFondo equals dec.Id
                                       select new { Record, TraduzioneSuGP = dec.Codice.Trim() };

            List<string> lstCodiciNonAmmessi = new List<string> { "C0", "E0", "D0" };

            List<string> records = lstRecordCodGestione.Select(x => x.TraduzioneSuGP).Where(x => lstCodiciNonAmmessi.Contains(x)).ToList();
            if (records != null && records.Count > 0)
            {
                messaggioVideo = string.Format("Non sono ammesse le seguenti gestioni: {0}", lstCodiciNonAmmessi.Aggregate((a, b) => { return a + ", " + b; }));
                return false;
            }

            return true;
        }


        private static bool ControlsDatiCalcoloENPALS(DatiCalcoloENPALS datiCalcoloENPALS, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiCalcoloENPALS == null)
            {
                messaggioVideo = "Errore tecnico: Area assente";
                return false;
            }

            if (!datiCalcoloENPALS.ImportoPensione.HasValue)
            {
                messaggioVideo = "Importo Pensione obbligatorio.";
                return false;
            }

            if (!GestioneControlli.ControlsProRataTemporisWithNaturaPensione(datiPensione, datiPensione.NaturaPensione, datiCalcoloENPALS.ImportoProRataTemporis, out messaggioVideo))
                return false;

            GestioneCalcolo.DatiCalcoloRetributivoENPAL datiRetributivi = GetDatiRetributiviEnpalsOrdinati(datiCalcoloENPALS.LDatiRetributivi);
            List<GestioneCalcolo.DatiCalcoloContributivoENPAL> datiContributivi = GetDatiContributiviEnpalsOrdinati(datiCalcoloENPALS.LDatiContributivi);

            if (datiRetributivi != null)
            {
                if (!GestioneControlli.ControlsDatiRetributiviENPALS(datiRetributivi, datiCalcoloENPALS.ImportoPensione, out messaggioVideo))
                    return false;
            }

            if (datiContributivi != null && datiContributivi.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivoENPAL datiContr in datiContributivi)
                {
                    if (!GestioneControlli.ControlsDatiContributiviENPALS(datiContr, out messaggioVideo))
                        return false;
                }
            }

            return true;
        }

        public static bool ControlsDatiCalcoloQuotePensioneAlCalcolo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<GestioneCalcolo.QuotePensione> lQuotePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (lQuotePensione == null || lQuotePensione.Count == 0)
                return true;

            if (!GestioneControlli.ControlsQuotePensioneIsEnteIstruttoreFondoExINPDAP(lQuotePensione, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteIstruttoreExInpdap : null, contenitoreDecodifica.ElencoDecEnteGestioneFondo, out messaggioVideo))
                return false;

            foreach (GestioneCalcolo.QuotePensione quotePensione in lQuotePensione)
            {
                if (!GestioneControlli.ControlsQuotePensioneObbligatori(quotePensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsQuotePensionePerVecchiaia(contenitore.DatiPensione, quotePensione, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.TipoCumulo : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsQuotePensionePerRicostituzioneCumuloProgressiva(contenitore.DatiPensione, quotePensione, out messaggioVideo))
                    return false;
            }

            if (lQuotePensione.Count < 2)
            {
                messaggioVideo = "Devono essere presenti almeno 2 registrazioni.";
                return false;
            }

            List<int> lstOccurence = (from q in lQuotePensione
                                      group q by q.EnteGestioneFondo into e
                                      select e.Count()).ToList();
            if (lstOccurence.Exists(x => x > 1))
            {
                messaggioVideo = "Non è possibile inserire due record con 'Ente / Gestione - Fondo' uguale.";
                return false;
            }

            List<long> gestioni = new List<long> { 16, 15, 17, 18, 19, 6, 20 };
            if (contenitore.DatiPensione != null && contenitore.DatiPensione.Gruppo != null && contenitore.DatiPensione.Prodotto != null && contenitore.DatiPensione.Tipo != null &&
                Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0022" && contenitore.DatiPensione.Tipo == "0052" && !(lQuotePensione.Any(x => gestioni.Contains(x.EnteGestioneFondo))))
            {
                messaggioVideo = "Per questa tipologia di domanda deve essere presente almeno una gestione tra C1, C2, C3, C4, C5, A6, D1.";
                return false;
            }
            if (!GestioneControlli.ControlsQuotePensionePerInabilita(contenitore.DatiPensione, lQuotePensione, contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.EnteCassa : null,
                contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.TipoCumulo : null, out messaggioVideo,
                listaDecEnteGestioneFondo: contenitoreDecodifica.ElencoDecEnteGestioneFondo))
                return false;
            if (!(ControlsDatiCalcolo_IOCUM_SOCUM(ref contenitore, ref contenitoreDecodifica, lQuotePensione, out messaggioVideo)))
                return false;

            if (!(ControlsDatiCalcolo_TOT(ref contenitore, ref contenitoreDecodifica, lQuotePensione, out messaggioVideo)))
                return false;

            if (!(ControlsDatiCalcoloGestioniVecchie_CUM(ref contenitore, ref contenitoreDecodifica, lQuotePensione, out messaggioVideo)))
                return false;
            return true;
        }


        public static bool ControlsDatiCalcoloTrattenuteQuotePensioneAlCalcolo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<GestioneCalcolo.TrattenuteQuotePensione> lTrattenute, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (lTrattenute == null || lTrattenute.Count == 0)
                return true;

            if (lTrattenute.GroupBy(x => x.EnteGestioneFondoQuote).Count() > 6)
            {
                messaggioVideo = "Non è possibile inserire più di 6 Quote pensione con Trattenute";
                return false;
            }

            List<int> listaOccorrenze = (from t in lTrattenute
                                         group t by t.EnteGestioneFondoQuote into e
                                         select e.Count()).ToList();
            if (listaOccorrenze.Exists(x => x > 10))
            {
                messaggioVideo = "Non è possibile inserire più di 10 Trattenute per Quota";
                return false;
            }

            if (Utility.IsDomandaIOCUM(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOTOT(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOTOT(contenitore.DatiPensione.SiglaCategoria))
            {
                var res = lTrattenute.GroupBy(x => x.EnteGestioneFondoQuote).Select(cl => new
                {
                    ImportoEnteGestioneFondoQuote = cl.First().ImportoEnteGestioneFondoQuote,
                    ImportoTrattenute = cl.Sum(c => c.ImportoTrattenute)
                }).ToList();

                if (res != null && res.Exists(x => x.ImportoTrattenute > x.ImportoEnteGestioneFondoQuote))
                {
                    messaggioVideo = "La somma dei singoli importi della trattenuta deve essere inferiore o uguale al corrispondente importo quota.";
                    return false;
                }
            }

            foreach (GestioneCalcolo.TrattenuteQuotePensione trattenute in lTrattenute)
            {
                if (!GestioneControlli.ControlsTrattenuteQuotePensione(trattenute, contenitoreDecodifica.ElencoDecEnteGestioneFondo, contenitoreDecodifica.ElencoDecCodiceTrattenute, contenitore.DatiPensione, out messaggioVideo))
                    return false;
            }
            return true;
        }

        private static bool ControlsDatiCalcoloQuotePensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiCalcoloQuotePensione datiCalcoloQuotePensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiCalcoloQuotePensione == null || datiCalcoloQuotePensione.LQuotePensione == null || datiCalcoloQuotePensione.LQuotePensione.Count == 0)
                return false;

            List<GestioneCalcolo.QuotePensione> lQuotePensione = MappingQuotePensioneToDB(datiCalcoloQuotePensione.LQuotePensione);

            if (!ControlsDatiCalcoloQuotePensioneAlCalcolo(ref contenitore, ref contenitoreDecodifica, lQuotePensione, out messaggioVideo))
                return false;

            List<GestioneCalcolo.TrattenuteQuotePensione> lTrattenute = MappingTrattenuteQuotePensioneToDB(datiCalcoloQuotePensione.LQuotePensione);

            if (!ControlsDatiCalcoloTrattenuteQuotePensioneAlCalcolo(ref contenitore, ref contenitoreDecodifica, lTrattenute, out messaggioVideo))
                return false;

            //ENG - Memo74_2023: bloccare il salvataggio del tab QuotePensione se non è valorrizzato il campo Contributi Italiani ed Esteri al 31/12/95.
            GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo74_2023", out ctrlMemo74_2023);
            if ((ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI" && Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria)) ||
                //ENG - Memo 116/2025
                Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(contenitore.DatiPensione) ||
                Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(contenitore.DatiPensione))
            {
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioniEstere);

                if ((!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) && (contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "V" || contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "Z")) ||
                    (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0))
                {
                    if (!datiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295.HasValue)
                    {
                        messaggioVideo = "Contributi Italiani ed Esteri al 31/12/95 obbligatori";
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ControlsDatiCalcoloVittimeTerrorismo(GestionePensione.DatiPensione datiPensione,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo,
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo, List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcoloRetributivo,
            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> lstDecGestioneCalcoloContributivo, List<Liquidazione.BLCommon.CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExINPDAI,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, Utility.TipoCalcolo tipoCalcolo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //Bisogna inserire almeno un record aggiuntivo
            if (listaDatiCalcoloVittimeTerrorismo == null || listaDatiCalcoloVittimeTerrorismo.Count == 0)
            {
                messaggioVideo = "E' necessario acquisire almeno un record di Dati Retributivi e/o Contributivi Vittime.";
                return false;
            }


            #region Obbligatorietà
            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'R' || x.Tipo == 'C' || x.Tipo == 'I') && !x.DecorrenzaBeneficio.HasValue))
            {
                messaggioVideo = "La Decorrenza Beneficio è obbligatoria.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'R' || x.Tipo == 'I') && !x.CodiceGestioneRetr.HasValue))
            {
                messaggioVideo = "Il Codice Gestione è obbligatorio.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'C') && !x.CodiceGestioneContr.HasValue))
            {
                messaggioVideo = "Il Codice Gestione è obbligatorio.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'R' || x.Tipo == 'C') && !x.Quota.HasValue))
            {
                messaggioVideo = "La Quota è obbligatoria.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'R' || x.Tipo == 'C') && !x.Settimane.HasValue) ||
                (!IsSettimaneImportoPensioneLocked(datiPensione, datiBeneficioVittimeTerrorismo) && listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'I') && !x.Settimane.HasValue)))
            {
                messaggioVideo = "Le Settimane sono obbligatorie.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'R') && !x.RMS.HasValue))
            {
                messaggioVideo = "Il Reddito / Retribuzione Media è obbligatorio/a.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'R' || x.Tipo == 'C' || x.Tipo == 'I') && !x.Beneficio.HasValue))
            {
                messaggioVideo = "Il Beneficio è obbligatorio.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'C') && !x.Ammontare.HasValue))
            {
                messaggioVideo = "L'Ammontare è obbligatorio.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'C') && !x.Montante.HasValue))
            {
                messaggioVideo = "Il Montante è obbligatorio.";
                return false;
            }

            if (listaDatiCalcoloVittimeTerrorismo.Exists(x => (x.Tipo == 'I') && !x.ImportoPensione.HasValue))
            {
                messaggioVideo = "L'Importo Pensione è obbligatorio.";
                return false;
            }
            #endregion Obbligatorietà

            if (!GestioneControlli.ControlsCoerenzaDatiCalcoloVittimeTerrorismo(datiPensione, listaDatiCalcoloVittimeTerrorismo, datiBeneficioVittimeTerrorismo, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDecorrenzaBeneficioVittimeTerrorismo(datiPensione, listaDatiCalcoloVittimeTerrorismo, datiBeneficioVittimeTerrorismo, datiDanteCausa, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsBeneficioTerrorismo(datiPensione, listaDatiCalcoloVittimeTerrorismo, datiBeneficioVittimeTerrorismo, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDatiCalcoloWithBeneficioTerrorismo(datiPensione, listaDatiCalcoloVittimeTerrorismo, listaDatiCalcoloRetributivo, listaDatiCalcoloContributivo,
                lstDecGestioneCalcoloRetributivo, lstDecGestioneCalcoloContributivo, datiBeneficioVittimeTerrorismo, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDatiCalcoloVittimeTerrorismoWithVisibility(datiPensione, listaDatiCalcoloContributivo, listaDatiCalcoloVittimeTerrorismo, datiBeneficioVittimeTerrorismo,
                tipoCalcolo, datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.SoggettoBeneficiario : null,
                datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaPrestazione : null,
                datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaBeneficio : null, out messaggioVideo))
                return false;
            if (!GestioneControlli.ControlsDatiCalcoloVittimeTerrorismoINPDAI(datiPensione, listaDatiCalcoloVittimeTerrorismo, listaDatiCalcoloRetributivo, listaDatiCalcoloContributivo,
                datiBeneficioVittimeTerrorismo, lstDecGestioneCalcoloRetributivo, lstDecGestioneCalcoloContributivo, listaCtrlDecorrenzaRetrExINPDAI,
                datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.TipoSettimaneBeneficio : null, tipoCalcolo, out messaggioVideo))
                return false;
            return true;
        }

        internal static byte? GetDecorrenzaExInpdai(string gestione, char? quota, string tipoQuota, List<Liquidazione.BLCommon.CtrlDecorrenzaRetrExINPDAI> lstCtrl)
        {
            byte? decorrenza = lstCtrl.Where(x => x.Gestione.Trim() == gestione.Trim() && x.Quota == quota && x.TipoQuota == tipoQuota).Select(x => x.CodiceDecorrenza).FirstOrDefault();
            return decorrenza;
        }

        internal static string GetCodiceGestioneExInpdai(long? codGestione, List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalc)
        {
            return lstDecGestioneCalc.Where(x => x.Id == codGestione).Select(x => x.TraduzioneSuGP.Trim()).FirstOrDefault();
        }

        internal static string GetCodiceGestioneExInpdai(long? codGestione, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> lstDecGestioneCalc)
        {
            return lstDecGestioneCalc.Where(x => x.Id == codGestione).Select(x => x.TraduzioneSuGP.Trim()).FirstOrDefault();
        }

        internal static List<GestioneAggiornamentoPECO.DatiRetributivi> MappingDatiRetributiviFromBLToView(List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi)
        {
            List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetributivi = null;
            if (ldatiRetributivi != null && ldatiRetributivi.Count > 0)
            {
                datiRetributivi = new List<GestioneAggiornamentoPECO.DatiRetributivi>();
                foreach (GestioneCalcolo.DatiCalcoloRetributivo calcRetr in ldatiRetributivi)
                {
                    GestioneAggiornamentoPECO.DatiRetributivi datiRetr = new GestioneAggiornamentoPECO.DatiRetributivi();
                    datiRetr.Quota = calcRetr.QuotePrimeLiquidate;

                    if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                    {
                        datiRetr.SettimaneA = calcRetr.NSettimaneQuotaA;
                        datiRetr.RMSQuotaA = calcRetr.RMSQuotaA;
                    }
                    else if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                    {
                        datiRetr.SettimaneB = calcRetr.NSettimaneQuotaB;
                        datiRetr.RMSQuotaB = calcRetr.RMSQuotaB;
                    }
                    datiRetr.CodGestione = calcRetr.CodiceGestione;
                    datiRetr.Decorrenza = calcRetr.DecorrenzaOriginariaPensione;
                    datiRetr.CodiceTipoQuota = calcRetr.CodiceTipoQuota;
                    datiRetr.NSettimane707 = calcRetr.NSettimane707;
                    datiRetr.PL_Quotar = calcRetr.PL_Quotar;
                    datiRetr.PL_Quotar707 = calcRetr.PL_Quotar707;
                    datiRetr.RMS = calcRetr.RMS;
                    //aggiunti per ripassarli per le ante96
                    datiRetr.NSettAnzianitaVV = calcRetr.NSettAnzianitaVV;
                    datiRetr.NSettimaneExCombattente = calcRetr.NSettimaneExCombattente;
                    datiRetr.RMSExCombattente = calcRetr.RMSExCombattente;
                    datiRetributivi.Add(datiRetr);
                }
            }
            return datiRetributivi;
        }

        internal static List<GestioneAggiornamentoPECO.DatiContributivi> MappingDatiContributiviFromBLToView(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, ref EntityBLCommon.ContenitoreObject contenitore, List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi, GestionePensione.DatiPensione datiPensione)
        {
            List<GestioneAggiornamentoPECO.DatiContributivi> datiContributivi = null;
            if (ldatiContributivi != null && ldatiContributivi.Count > 0)
            {
                datiContributivi = new List<GestioneAggiornamentoPECO.DatiContributivi>();
                GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneK = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null ? (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "K") ? contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "K") : null) : null;
                GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneL = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null ? (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "L") ? contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "L") : null) : null;
                foreach (GestioneCalcolo.DatiCalcoloContributivo calContr in ldatiContributivi)
                {
                    GestioneAggiornamentoPECO.DatiContributivi datiContr = new GestioneAggiornamentoPECO.DatiContributivi();
                    datiContr.CodGestione = calContr.CodiceGestione;
                    var ctrlSettimane = Utility.IsDomandaAUT(datiPensione) ? (calContr.NSettimane.HasValue && calContr.NSettimane.Value != 0 ? true : false) : calContr.NSettimane.HasValue;
                    if ((Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) &&
                        decGestioneL != null && calContr.CodiceGestione == decGestioneL.Id)
                        || calContr.ImportoContributivoTotale.HasValue || calContr.Montante.HasValue || ctrlSettimane)
                    {
                        if ((decGestioneK == null || calContr.CodiceGestione != decGestioneK.Id) &&
                            (decGestioneL == null || !Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) || !Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || (Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) && calContr.CodiceGestione != decGestioneL.Id)))
                            datiContr.Quota = 'C';
                        datiContr.ImportoContributivo = calContr.ImportoContributivoTotale;
                        datiContr.MontanteContributivo = calContr.Montante;
                        datiContr.Settimane = calContr.NSettimane;
                    }
                    else if (calContr.ImportoContribTotaleQuotaDL214.HasValue || calContr.MontanteQuotaDL214.HasValue || calContr.NSettimaneQuotaDL214.HasValue)
                    {
                        datiContr.Quota = 'D';
                        datiContr.ImportoContributivoQuotaD = calContr.ImportoContribTotaleQuotaDL214;
                        datiContr.MontanteContributivoQuotaD = calContr.MontanteQuotaDL214;
                        datiContr.SettimaneQuotaD = calContr.NSettimaneQuotaDL214;
                    }
                    datiContr.DecorrenzaCalcoloContibutivo = calContr.DecorrenzaCalcoloContibutivo;
                    datiContr.PL_Quotac = calContr.PL_Quotac;
                    //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
                    if (!(Utility.IsRicostituzione_MotiviContributivi(datiPensione) && contenitore.ListaDatiQuotaFondoIntegrativoStorico != null && datiContr.CodGestione == null))
                        datiContributivi.Add(datiContr);

                }
            }
            return datiContributivi;
        }

        internal static List<GestioneCalcolo.DatiCalcoloRetributivo> MappingDatiRetributiviFromViewToBL(List<GestioneAggiornamentoPECO.DatiRetributivi> ldatiRetributivi)
        {
            List<GestioneCalcolo.DatiCalcoloRetributivo> datiRetributivi = null;
            if (ldatiRetributivi != null && ldatiRetributivi.Count > 0)
            {
                datiRetributivi = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
                foreach (GestioneAggiornamentoPECO.DatiRetributivi calcRetr in ldatiRetributivi)
                {
                    GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();
                    datiRetr.DecorrenzaOriginariaPensione = calcRetr.Decorrenza;
                    datiRetr.CodiceGestione = calcRetr.CodGestione;
                    datiRetr.QuotePrimeLiquidate = calcRetr.Quota;
                    datiRetr.CodiceTipoQuota = calcRetr.CodiceTipoQuota;
                    datiRetr.NSettimane707 = calcRetr.NSettimane707;

                    if (calcRetr.Quota.HasValue && calcRetr.Quota.Value.ToString().ToUpperInvariant() == "A")
                    {
                        datiRetr.RMSQuotaA = calcRetr.RMSQuotaA;
                        datiRetr.NSettimaneQuotaA = calcRetr.SettimaneA;
                    }
                    else if (calcRetr.Quota.HasValue && calcRetr.Quota.Value.ToString().ToUpperInvariant() == "B")
                    {
                        datiRetr.RMSQuotaB = calcRetr.RMSQuotaB;
                        datiRetr.NSettimaneQuotaB = calcRetr.SettimaneB;
                    }
                    datiRetr.PL_Quotar = calcRetr.PL_Quotar;
                    datiRetr.PL_Quotar707 = calcRetr.PL_Quotar707;
                    datiRetr.RMS = calcRetr.RMS;
                    //aggiunti per ripassarli per le ante96
                    datiRetr.NSettAnzianitaVV = calcRetr.NSettAnzianitaVV;
                    datiRetr.NSettimaneExCombattente = calcRetr.NSettimaneExCombattente;
                    datiRetr.RMSExCombattente = calcRetr.RMSExCombattente;
                    datiRetributivi.Add(datiRetr);
                }
            }
            return datiRetributivi;
        }

        internal static List<GestioneCalcolo.DatiCalcoloContributivo> MappingDatiContributiviFromViewToBL(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<GestioneAggiornamentoPECO.DatiContributivi> ldatiContributivi, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            List<GestioneCalcolo.DatiCalcoloContributivo> datiContributivi = null;
            var IsAnte96 = Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, Utility.IsRiaperturaDomanda(datiPensione.Id));
            if (ldatiContributivi != null && ldatiContributivi.Count > 0)
            {
                datiContributivi = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneK = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null ? (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "K") ? contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "K") : null) : null;
                GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneL = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null ? (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Exists(x => x.TraduzioneSuGP.Trim() == "L") ? contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.FirstOrDefault(x => x.TraduzioneSuGP.Trim() == "L") : null) : null;
                foreach (GestioneAggiornamentoPECO.DatiContributivi calcContr in ldatiContributivi)
                {
                    GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();

                    datiContr.CodiceGestione = calcContr.CodGestione;

                    if (calcContr.Quota.HasValue && calcContr.Quota.Value.ToString().ToUpperInvariant() == "C")
                    {
                        datiContr.ImportoContributivoTotale = calcContr.ImportoContributivo;
                        datiContr.Montante = calcContr.MontanteContributivo;
                        datiContr.NSettimane = calcContr.Settimane;
                    }
                    else if (calcContr.Quota.HasValue && calcContr.Quota.Value.ToString().ToUpperInvariant() == "D")
                    {
                        datiContr.ImportoContribTotaleQuotaDL214 = calcContr.ImportoContributivoQuotaD;
                        datiContr.MontanteQuotaDL214 = calcContr.MontanteContributivoQuotaD;
                        datiContr.NSettimaneQuotaDL214 = calcContr.SettimaneQuotaD;
                    }
                    else if (!calcContr.Quota.HasValue && ((decGestioneK != null && calcContr.CodGestione == decGestioneK.Id) || (decGestioneL != null && Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) &&
                        Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && calcContr.CodGestione == decGestioneL.Id) || IsAnte96 != null))
                    {
                        datiContr.ImportoContributivoTotale = calcContr.ImportoContributivo;
                        datiContr.Montante = calcContr.MontanteContributivo;
                        datiContr.NSettimane = calcContr.Settimane;
                    }
                    if (IsAnte96 != null) datiContr.DecorrenzaCalcoloContibutivo = calcContr.DecorrenzaCalcoloContibutivo;
                    datiContr.PL_Quotac = calcContr.PL_Quotac;
                    datiContributivi.Add(datiContr);
                }
            }
            return datiContributivi;
        }

        private static GestioneCalcolo.DatiCalcoloRetributivoENPAL GetDatiRetributiviEnpalsOrdinati(List<DatiRetributiviENPALS> ldatiRetributivi)
        {
            GestioneCalcolo.DatiCalcoloRetributivoENPAL datiRetributivi = null;
            if (ldatiRetributivi != null && ldatiRetributivi.Count > 0)
            {
                datiRetributivi = new GestioneCalcolo.DatiCalcoloRetributivoENPAL();
                foreach (DatiRetributiviENPALS calcRetr in ldatiRetributivi)
                {
                    if (calcRetr.Quota.Value.ToString().ToUpperInvariant() == "A")
                    {
                        datiRetributivi.PeriodiQuotaA = calcRetr.Periodi;
                        datiRetributivi.RMQuotaA = calcRetr.RM;
                        datiRetributivi.NTotaleContributiCalcoloQuotaA = (short?)calcRetr.NTotaleContributiCalcolo;
                        datiRetributivi.ImportoQuotaA = calcRetr.Importo;
                        datiRetributivi.GiorniQuotaA707 = calcRetr.Giorni707;
                        datiRetributivi.ImportoQuotaA707 = calcRetr.Importo707;
                        datiRetributivi.DecorrenzaQuotaA = calcRetr.Decorrenza;
                    }
                    else if (calcRetr.Quota.Value.ToString().ToUpperInvariant() == "B")
                    {
                        datiRetributivi.PeriodiQuotaB = calcRetr.Periodi;
                        datiRetributivi.RMQuotaB = calcRetr.RM;
                        datiRetributivi.NTotaleContributiCalcoloQuotaB = (short?)calcRetr.NTotaleContributiCalcolo;
                        datiRetributivi.ImportoQuotaB = calcRetr.Importo;
                        datiRetributivi.GiorniQuotaB707 = calcRetr.Giorni707;
                        datiRetributivi.ImportoQuotaB707 = calcRetr.Importo707;
                        datiRetributivi.DecorrenzaQuotaB = calcRetr.Decorrenza;
                    }
                }
            }
            return datiRetributivi;
        }

        private static List<GestioneCalcolo.DatiCalcoloContributivoENPAL> GetDatiContributiviEnpalsOrdinati(List<DatiContributiviENPALS> ldatiContributivi)
        {
            List<GestioneCalcolo.DatiCalcoloContributivoENPAL> datiContributivi = null;
            if (ldatiContributivi != null && ldatiContributivi.Count > 0)
            {
                datiContributivi = new List<GestioneCalcolo.DatiCalcoloContributivoENPAL>();
                foreach (DatiContributiviENPALS calcContr in ldatiContributivi)
                {
                    GestioneCalcolo.DatiCalcoloContributivoENPAL datiContr = new GestioneCalcolo.DatiCalcoloContributivoENPAL();
                    Utility.ValorizzaOggetti(calcContr, datiContr);

                    datiContributivi.Add(datiContr);
                }
            }

            return datiContributivi;
        }

        private static List<GestioneCalcolo.QuotePensione> MappingQuotePensioneToDB(List<DatiQuotePensione> lQuotePensione)
        {
            List<GestioneCalcolo.QuotePensione> lQuotePensioneDB = null;
            if (lQuotePensione != null && lQuotePensione.Count > 0)
            {
                lQuotePensioneDB = new List<GestioneCalcolo.QuotePensione>();
                foreach (DatiQuotePensione quotePensione in lQuotePensione)
                {
                    GestioneCalcolo.QuotePensione quotePensioneDB = new GestioneCalcolo.QuotePensione();
                    Utility.ValorizzaOggetti(quotePensione, quotePensioneDB);
                    lQuotePensioneDB.Add(quotePensioneDB);
                }
            }

            return lQuotePensioneDB;
        }

        private static List<GestioneCalcolo.TrattenuteQuotePensione> MappingTrattenuteQuotePensioneToDB(List<DatiQuotePensione> listaQuotePensione)
        {
            List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenuteDB = null;
            if (listaQuotePensione != null && listaQuotePensione.Count > 0)
            {
                foreach (DatiQuotePensione quotePensione in listaQuotePensione)
                {
                    if (quotePensione.ListaTrattenute != null && quotePensione.ListaTrattenute.Count > 0)
                    {
                        if (listaTrattenuteDB == null)
                            listaTrattenuteDB = new List<GestioneCalcolo.TrattenuteQuotePensione>();
                        foreach (DatiQuotePensione.DatiTrattenute trattenute in quotePensione.ListaTrattenute)
                        {
                            GestioneCalcolo.TrattenuteQuotePensione trattenuteDB = new GestioneCalcolo.TrattenuteQuotePensione();
                            Utility.ValorizzaOggetti(trattenute, trattenuteDB);
                            trattenuteDB.EnteGestioneFondoQuote = quotePensione.EnteGestioneFondo;
                            trattenuteDB.ImportoEnteGestioneFondoQuote = quotePensione.Importo;
                            listaTrattenuteDB.Add(trattenuteDB);
                        }
                    }
                }
            }
            return listaTrattenuteDB;
        }

        internal static List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> MappingDatiQuotaFondoIntegrativoFromViewToBL(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ldatiQuotaFondoIntegrativo)
        {
            List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> datiQuotaFondoIntegrativo = null;

            if (ldatiQuotaFondoIntegrativo != null && ldatiQuotaFondoIntegrativo.Count > 0)
            {
                datiQuotaFondoIntegrativo = new List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo>();
                foreach (GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo calcContr in ldatiQuotaFondoIntegrativo)
                {
                    GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo datiContr = new GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo();

                    datiContr.CodiceGestione = calcContr.CodiceGestione;

                    if (calcContr.Quota.HasValue && calcContr.Quota.Value.ToString().ToUpperInvariant() == "C")
                    {
                        datiContr.ImportoContributivoTotale = calcContr.ImportoContributivoTotale;
                        datiContr.Montante = calcContr.Montante;
                        datiContr.NSettimane = calcContr.NSettimane;
                    }
                    else if (calcContr.Quota.HasValue && calcContr.Quota.Value.ToString().ToUpperInvariant() == "D")
                    {
                        datiContr.ImportoContribTotaleQuotaD = calcContr.ImportoContribTotaleQuotaD;
                        datiContr.MontanteQuotaD = calcContr.MontanteQuotaD;
                        datiContr.NSettimaneQuotaD = calcContr.NSettimaneQuotaD;
                    }
                    datiContr.PL_Quotac = calcContr.PL_Quotac;
                    datiQuotaFondoIntegrativo.Add(datiContr);
                }
            }
            return datiQuotaFondoIntegrativo;
        }

        internal static List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> MappingDatiContributiviQuotaFondoINPGIFromViewToBL(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> ldatiQuotaFondoINPGI, long idPensione)
        {
            List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> datiContrQuotaFondoINPGI = null;

            if (ldatiQuotaFondoINPGI != null && ldatiQuotaFondoINPGI.Count > 0)
            {
                datiContrQuotaFondoINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI>();
                foreach (GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI calcContr in ldatiQuotaFondoINPGI)
                {
                    GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI datiContr = new GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI();

                    datiContr.IdPensione = idPensione;
                    datiContr.CodiceGestione = calcContr.CodiceGestione;
                    datiContr.Montante = calcContr.Montante;
                    datiContr.Quota = calcContr.Quota;
                    datiContr.Settimane = calcContr.Settimane;

                    datiContrQuotaFondoINPGI.Add(datiContr);
                }
            }
            return datiContrQuotaFondoINPGI;
        }

        internal static List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> MappingDatiRetributiviQuotaFondoINPGIFromViewToBL(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> ldatiQuotaFondoINPGI, long idPensione)
        {
            List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> datiRetrQuotaFondoINPGI = null;

            if (ldatiQuotaFondoINPGI != null && ldatiQuotaFondoINPGI.Count > 0)
            {
                datiRetrQuotaFondoINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI>();
                foreach (GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI calcContr in ldatiQuotaFondoINPGI)
                {
                    GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI datiContr = new GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI();

                    datiContr.IdPensione = idPensione;
                    datiContr.CodiceGestione = calcContr.CodiceGestione;
                    datiContr.Settimane = calcContr.Settimane;
                    datiContr.ImportoCalcolato = calcContr.ImportoCalcolato;
                    datiContr.ImportoComma707 = calcContr.ImportoComma707;
                    datiContr.SettimaneComma707 = calcContr.SettimaneComma707;
                    datiContr.RetribuzioneMediaSettimanale = calcContr.RetribuzioneMediaSettimanale;

                    datiRetrQuotaFondoINPGI.Add(datiContr);
                }
            }
            return datiRetrQuotaFondoINPGI;
        }

        public static bool ControlsDatiRetributivi(GestioneCalcolo.DatiCalcoloRetributivo datiRetr, char? codiceLiquidazione, GestionePensione.DatiPensione datiPensione,
            GestioneDanteCausa.DatiDanteCausa datiDA, List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetributivo, Utility.TipoCalcolo tipoCalcolo, bool isRiaperturaDomanda, List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo, List<GestioneCalcolo.DatiCalcoloContributivo> ListaDatiContributivi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (BypassaControlliRicSdaiContributivoK(datiPensione, lDatiContributivi, listaCodeGestioneCalcoloContributivo) || BypassaControlliRic_VOCRED_CRED27_ContributivoL(datiPensione, lDatiContributivi, listaCodeGestioneCalcoloContributivo) ||
                Utility.IsDomandaVOPGI_AGI(datiPensione))
                return true;

            if (!GestioneControlli.ControlsDatiRetributivi(datiRetr, codiceLiquidazione, datiPensione, listaCodeGestioneCalcoloRetributivo,
                 datiDA != null ? datiDA.DataMorte : (DateTime?)null, tipoCalcolo, isRiaperturaDomanda, ListaDatiContributivi, listaCodeGestioneCalcoloContributivo, datiDA, out messaggioVideo))
                return false;
            return true;
        }

        public static bool ControlsDatiContributivi(GestioneCalcolo.DatiCalcoloContributivo datiContr, GestionePensione.DatiPensione datiPensione,
            GestioneDanteCausa.DatiDanteCausa datiDA, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo, string tipoSettimaneBeneficio,
            Utility.TipoCalcolo tipoCalcolo, List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, GestioneDanteCausa.DatiDanteCausa danteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (BypassaControlliRicSdaiContributivoK(datiPensione, lDatiContributivi, listaCodeGestioneCalcoloContributivo) ||
                BypassaControlliRic_VOCRED_CRED27_ContributivoL(datiPensione, lDatiContributivi, listaCodeGestioneCalcoloContributivo) ||
                Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa))
                return true;

            if (!GestioneControlli.ControlsDatiContributivi(datiContr, datiPensione, listaCodeGestioneCalcoloContributivo,
                        datiDA != null ? datiDA.ProvenienzaPensione : (byte?)0,
                        datiDA != null ? datiDA.SiglaCategoria : string.Empty, tipoSettimaneBeneficio, tipoCalcolo, listaDatiContributivi, datiDA, out messaggioVideo))
                return false;
            return true;
        }

        public static void DeleteDatiCalcoloByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out string errori)
        {
            errori = string.Empty;
            //ENG- Memo 68/2022 aggiornato al 12/03/2025
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneModificheMemoINPGI_20250312 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20250312 ", out ctrlAbilitazioneModificheMemoINPGI_20250312);

            if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
            {
                if (contenitore.DatiPensioniDatiGenerici != null)
                {
                    contenitore.DatiPensioniDatiGenerici.QuotaAl95 = null;
                    contenitore.DatiPensioniDatiGenerici.AnzAl95 = null;
                }
            }
            if (Utility.IsDomandaAUT(contenitore.DatiPensione))
            {
                if (contenitore.DatiPensioniDatiGenerici != null)
                    contenitore.DatiPensioniDatiGenerici.FacoltaComputo = null;
            }
            if (Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) ||
                Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null) ||
                Utility.IsDomandaESPA_L26(contenitore.DatiPensione) || Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione))
            {
                if (contenitore.DatiPensioniDatiGenerici != null)
                    contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza = null;
            }
            if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
            {
                if (contenitore.DatiPensioniDatiGenerici != null)
                    contenitore.DatiPensioniDatiGenerici.ImportoLordo = null;
            }
            if (Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione))
            {
                if (contenitore.DatiPensioniDatiGenerici != null)
                {
                    contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria = null;
                    contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001 = null;
                }
            }
            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
            {
                if (contenitore.DatiPensioniDatiGenerici != null)
                    contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf = null;
            }

            //ENG - Memo 116/2025
            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295.HasValue)
                contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 = null;

            try
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = contenitore.DatiPensioniDatiGenerici;
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
                //----------------------------------------------------------------

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestioneCalcolo.EliminaCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, false);
                    GestioneCalcolo.EliminaCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, false);

                    datiQuadroDatiContributivi.Tipo = 2;

                    if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                        datiQuadroDatiContributivi.TabDatiCalcoloINPDAI = 0;
                    //ENG - Aggiornamento Memo 68/2022 IOPGI
                    //ENG - Spacchettate SOPGI 
                    else if (Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione))
                        || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, contenitore.DatiDanteCausa))
                    {
                        //ENG - VOPGI/IOPGI: RIC CONTRIBUTIVE/PL/TRF con fine assicurazione post 30/06/2022 il Tab Dati Calcolo deve essere obbligatorio 
                        if (ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI" &&
                            (Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) || (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) &&
                            contenitore.DatiPensione.FineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2022, 06, 30)) &&
                            (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || !Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || contenitore.IsRiaperturaDomanda))
                            datiQuadroDatiContributivi.TabDatiCalcolo = 0;
                        else

                            datiQuadroDatiContributivi.TabDatiCalcolo = 1;
                    }
                    else
                        datiQuadroDatiContributivi.TabDatiCalcolo = 0;

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    //ENG - Aggiornamento Memo 68/2022 IOPGI
                    //ENG - Spacchettate SOPGI
                    if ((Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) || Utility.IsDomandaAUT(datiPensione) || Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, contenitore.DatiDanteCausa)) && datiPensioniDatiGenerici != null)
                    {
                        if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                        {
                            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                            datiPensioniDatiGenerici = null;
                        }
                        else
                            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioniDatiGenerici);
                    }
                    //veso92 filtro L92
                    if (Utility.IsDomandaVESO92_L92(datiPensione) || Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) ||
                        Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) ||
                        Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null) ||
                        Utility.IsDomandaESPA_L26(contenitore.DatiPensione) || Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione) || Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione))
                    {
                        if (datiPensioniDatiGenerici.Equals(new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                        {
                            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                            datiPensioniDatiGenerici = null;
                        }
                        else
                            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiPensioniDatiGenerici);
                    }

                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                //--------------------------------------------------------------------
                contenitore.DatiPensioniDatiGenerici = datiPensioniDatiGenerici;
                contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
                contenitore.ListaDatiRetributivi = null;
                contenitore.ListaDatiContributivi = null;
                //--------------------------------------------------------------------
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }

        public static bool IsFineAssicurazionePost2012(DateTime? fineAssicurazione)
        {
            DateTime DuemilaDodici = new DateTime(2012, 01, 01);

            if (fineAssicurazione.HasValue)
                return Liquidazione.BLCommon.Utility.DataSuccessivaA(fineAssicurazione.Value, DuemilaDodici);

            return false;
        }

        public static void GetListaDecodificaGestioneCalcoloRetributivo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            out List<DecodificaGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivo)
        {
            listaDecodificaGestioneCalcoloRetributivo = null;
            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetrCommon = contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo.ToList();

            if (elencoCodeGestioneCalcoloRetrCommon != null)
            {
                GetCodeGestioneCalcoloRetributivoCustom(ref contenitore, ref elencoCodeGestioneCalcoloRetrCommon);

                listaDecodificaGestioneCalcoloRetributivo = new List<DecodificaGestioneCalcoloRetributivo>();

                foreach (GestioneDecodifica.CodeGestioneCalcoloRetributivo codGestioneCommon in elencoCodeGestioneCalcoloRetrCommon)
                {
                    DecodificaGestioneCalcoloRetributivo codGestione = new DecodificaGestioneCalcoloRetributivo();
                    codGestione.Id = codGestioneCommon.Id;
                    codGestione.Descrizione = codGestioneCommon.Descrizione;
                    codGestione.TraduzioneSuGP = codGestioneCommon.TraduzioneSuGP;
                    codGestione.IsFondo = codGestioneCommon.IsFondo;
                    listaDecodificaGestioneCalcoloRetributivo.Add(codGestione);
                }
            }
        }

        public static void GetCodeGestioneCalcoloRetributivoCustom(ref EntityBLCommon.ContenitoreObject contenitore, ref List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivoCustom)
        {
            if (contenitore.DatiPensione != null)
            {
                if (listaDecodificaGestioneCalcoloRetributivoCustom != null && listaDecodificaGestioneCalcoloRetributivoCustom.Count > 0)
                {
                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivoApp = listaDecodificaGestioneCalcoloRetributivoCustom.ToList();
                    string codCat = contenitore.DatiPensione.GetCodCategoria();
                    string filtro = contenitore.DatiPensione.GetFiltro();
                    char codNat1;
                    char codNat2;
                    char codNat3;
                    Utility.GetCodiciNatura(contenitore.DatiPensione.NaturaPensione, out codNat1, out codNat2, out codNat3);

                    foreach (GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestione in listaDecodificaGestioneCalcoloRetributivoApp)
                    {
                        switch (codeGestione.TraduzioneSuGP.Trim())
                        {
                            case "2":
                            case "3":
                            case "4":
                                if (codCat == "0001" || codCat == "0002" ||
                                   (codCat == "0003" && Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) != Utility.TipoUnicarpe.Automatica) ||
                                    Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) || // cod 29
                                    Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) || //cod 198
                                    Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || //cod 199
                                    Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) || //cod28 e 128
                                    Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) || //cod 27 e 127
                                    Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || //cod 197
                                    Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria) || //cod 196
                                    Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) || //cod 200
                                    Utility.IsDomandaVOMIN(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOMIN(contenitore.DatiPensione.SiglaCategoria) || //cod 13,14
                                    Utility.IsDomandaPescatori(contenitore.DatiPensione.SiglaCategoria) ||
                                    Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) ||
                                    ((codCat == "0082" || codCat == "0083" || codCat == "0084") && (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione))) ||
                                    Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione) || (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione) && Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria)))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "A":
                                if (codCat != "0082" && codCat != "0083" && codCat != "0084")
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "Q":
                                if (!(contenitore.DatiPensione.AttivitaEconomica == 4 && contenitore.DatiPensione.ProfessioneIndividuale == 350 && contenitore.DatiMaggiorazioniBenefici != null &&
                                    contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "09" && !Utility.DataSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, new DateTime(2004, 02, 01))))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "S":
                                if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                                {
                                    if (filtro != "BNS" && filtro != "BNX" && filtro != "SCO" && filtro != "B44" && filtro != "B45" &&
                                        !(Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione && codNat2 == 'Y'))
                                        listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                }
                                else if (filtro != "BNS" && filtro != "BNX" &&
                                    !((Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto) == Utility.TipoDomanda.Ripristino)
                                    && codNat2 == 'Y'))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "1H":
                                List<string> codCatRAL_1 = new List<string> { "0001", "0003", "0018", "0020", "0021", "0023", "0015", "0017", "0070", "0072", "0082", "0084" };
                                List<string> codCatR44R45_1 = new List<string> { "0002", "0019", "0022", "0016", "0071", "0083" };
                                if (((filtro != "RAL" || !codCatRAL_1.Contains(codCat)) && ((filtro != "R44" && filtro != "R45") || !codCatR44R45_1.Contains(codCat))) &&
                                    (!(Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && codNat2 == 'H')))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "2H":
                            case "3H":
                            case "4H":
                                List<string> codCatRAL_Other = new List<string> { "0018", "0020", "0021", "0023", "0015", "0017", "0070", "0072", "0082", "0084" };
                                List<string> codCatR44R45_Other = new List<string> { "0019", "0022", "0016", "0071", "0083" };
                                if ((filtro != "RAL" || !codCatRAL_Other.Contains(codCat)) && ((filtro != "R44" && filtro != "R45") || !codCatR44R45_Other.Contains(codCat)) &&
                                    (!(Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && codNat2 == 'H')))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "P":
                                if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) &&
                                    !Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria))
                                {
                                    GestioneAnagrafica.DatiAnagrafici soggetto = Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione) ? contenitore.DatiAnagraficiDanteCausa : contenitore.DatiAnagraficiTitolare;
                                    if (soggetto == null || soggetto.Sesso != 'F')
                                        listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                }
                                else
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "7":
                                if (!Utility.IsDomandaVOMIN(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaSOMIN(contenitore.DatiPensione.SiglaCategoria))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "H":
                                if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                                    codeGestione.Descrizione = "INPGI";
                                if (!Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "I":
                                if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                                    codeGestione.Descrizione = "CPDEL";
                                if ((!Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.SiglaCategoria.Trim() != "VR" && contenitore.DatiPensione.SiglaCategoria.Trim() != "VOCOM" && contenitore.DatiPensione.SiglaCategoria.Trim() != "VOART") || !Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "C":
                            case "G":
                            case "B":
                            case "D":
                            case "F":
                            case "L":
                            case "O":
                                if (!Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                            case "M":
                            case "N":
                                if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                                {
                                    if (codeGestione.TraduzioneSuGP.Trim() == "M") codeGestione.Descrizione = "TRASP";
                                    if (codeGestione.TraduzioneSuGP.Trim() == "N") codeGestione.Descrizione = "ASSICUR";
                                }
                                if ((contenitore.DatiPensione.SiglaCategoria.Trim() != "VR" && contenitore.DatiPensione.SiglaCategoria.Trim() != "VOCOM" && contenitore.DatiPensione.SiglaCategoria.Trim() != "VOART" && !Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria)) || !Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                break;
                        }
                    }
                }
            }
        }

        public static void GetListaDecodificaGestioneCalcoloContributivo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            out List<DecodificaGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivo, GestioneContrib.DatiCalcolo datiCalcolo)
        {
            listaDecodificaGestioneCalcoloContributivo = null;
            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContrCommon = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.ToList();

            if (elencoCodeGestioneCalcoloContrCommon != null)
            {
                GetCodeGestioneCalcoloContributivoCustom(ref contenitore, ref elencoCodeGestioneCalcoloContrCommon, datiCalcolo);

                listaDecodificaGestioneCalcoloContributivo = new List<DecodificaGestioneCalcoloContributivo>();

                foreach (GestioneDecodifica.CodeGestioneCalcoloContributivo codGestioneCommon in elencoCodeGestioneCalcoloContrCommon)
                {
                    DecodificaGestioneCalcoloContributivo codGestione = new DecodificaGestioneCalcoloContributivo();
                    codGestione.Id = codGestioneCommon.Id;
                    codGestione.Descrizione = codGestioneCommon.Descrizione;
                    codGestione.TraduzioneSuGP = codGestioneCommon.TraduzioneSuGP;
                    codGestione.IsFondo = codGestioneCommon.IsFondo;
                    listaDecodificaGestioneCalcoloContributivo.Add(codGestione);
                }
            }
        }

        public static void GetCodeGestioneCalcoloContributivoCustom(ref EntityBLCommon.ContenitoreObject contenitore, ref List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivoCustom, GestioneContrib.DatiCalcolo datiCalcolo)
        {
            if (contenitore.DatiPensione != null)
            {
                if (listaDecodificaGestioneCalcoloContributivoCustom != null && listaDecodificaGestioneCalcoloContributivoCustom.Count > 0)
                {
                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivoApp = listaDecodificaGestioneCalcoloContributivoCustom.ToList();
                    string codCat = contenitore.DatiPensione.GetCodCategoria();
                    string filtro = contenitore.DatiPensione.GetFiltro();

                    GestioneDecodifica.CodeGestioneCalcoloContributivo decGestioneL = listaDecodificaGestioneCalcoloContributivoApp.Find(x => x.TraduzioneSuGP.Trim() == "L");

                    //ENG - Aggiornamento Memo 123_2021 
                    GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2021 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out ctrlMemo123_2021);

                    //ENG- Memo 68/2022 aggiornato al 12/03/2025
                    GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneModificheMemoINPGI_20250312 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20250312 ", out ctrlAbilitazioneModificheMemoINPGI_20250312);

                    foreach (GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestione in listaDecodificaGestioneCalcoloContributivoApp)
                    {
                        switch (codeGestione.TraduzioneSuGP.Trim())
                        {
                            case "2":
                            case "3":
                            case "4":
                                if (codCat == "0001" || codCat == "0002" || (codCat == "0003" && Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) != Utility.TipoUnicarpe.Automatica) ||
                                    Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) || // cod 29
                                    Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) || //cod 198
                                    Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || //cod 199
                                    Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) || //cod28 e 128
                                    Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) || //cod 27 e 127
                                    Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || //cod 197
                                    Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria) || //cod 196
                                    Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) || //cod 200
                                    Utility.IsDomandaVOMIN(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOMIN(contenitore.DatiPensione.SiglaCategoria) || //cod 13,14
                                    Utility.IsDomandaPescatori(contenitore.DatiPensione.SiglaCategoria) ||
                                    Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) ||
                                    ((codCat == "0082" || codCat == "0083" || codCat == "0084") && (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione))) ||
                                    Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione) || (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione) && Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria)))
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "A":
                                if (codCat != "0082" && codCat != "0083" && codCat != "0084")
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "Q":
                                if (!(contenitore.DatiPensione.AttivitaEconomica == 4 && contenitore.DatiPensione.ProfessioneIndividuale == 350 && contenitore.DatiMaggiorazioniBenefici != null &&
                                    contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "09" && !Utility.DataSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, new DateTime(2004, 02, 01))))
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "S":
                                if (filtro != "BNS" && filtro != "BNX")
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "1H":
                                List<string> codCatRAL_1 = new List<string> { "0001", "0003", "0018", "0020", "0021", "0023", "0015", "0017", "0070", "0072", "0082", "0084" };
                                List<string> codCatR44R45_1 = new List<string> { "0002", "0019", "0022", "0016", "0071", "0083" };
                                if ((filtro != "RAL" || !codCatRAL_1.Contains(codCat)) &&
                                    ((filtro != "R44" && filtro != "R45") || !codCatR44R45_1.Contains(codCat)))
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "2H":
                            case "3H":
                            case "4H":
                                List<string> codCatRAL_Other = new List<string> { "0018", "0020", "0021", "0023", "0015", "0017", "0070", "0072", "0082", "0084" };
                                List<string> codCatR44R45_Other = new List<string> { "0019", "0022", "0016", "0071", "0083" };
                                if ((filtro != "RAL" || !codCatRAL_Other.Contains(codCat)) &&
                                    ((filtro != "R44" && filtro != "R45") || !codCatR44R45_Other.Contains(codCat)))
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            //COD GEST AUT
                            case "G":
                            case "C1":
                            case "C2":
                            case "C3":
                            case "C4":
                            case "C5":
                            case "E1":
                            case "E2":
                            case "F0":
                            case "F1":
                            case "D1":
                            case "A5":
                            case "A6":
                            case "A7":
                            case "A8":
                            case "A9":
                            case "B1":
                            case "B2":
                            case "B3":
                            case "B4":
                                //ENG - Aggiornamento Memo 123_2021
                                if (!Utility.IsDomandaAUT(contenitore.DatiPensione) || (codeGestione.TraduzioneSuGP == "F0" && (ctrlMemo123_2021 == null || String.IsNullOrEmpty(ctrlMemo123_2021.ValoreControllo) || ctrlMemo123_2021.ValoreControllo.Trim() == "NO")) || codeGestione.TraduzioneSuGP == "F1")
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "K":
                                if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && codCat == "0084" &&
                                    contenitore.DatiPensione.NaturaPensione.Substring(2, 1) == "T"))//SDAI
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "L":
                                if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)
                                    || !Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) ||
                                   datiCalcolo == null || decGestioneL == null || datiCalcolo.lDatiContributivi == null || !datiCalcolo.lDatiContributivi.Exists(x => x.CodGestione == decGestioneL.Id))
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "O":
                            case "P":
                                if (!Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) == null)
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                            case "FB":
                                if (!(ctrlAbilitazioneModificheMemoINPGI_20250312 != null && ctrlAbilitazioneModificheMemoINPGI_20250312.ValoreControllo == "SI" && Utility.IsDomandaINPGI(contenitore.DatiPensione.SiglaCategoria) &&
                                      contenitore.DatiPensione.FineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.FineAssicurazione.Value, new DateTime(2022, 06, 30))))
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                break;
                        }
                    }
                }
            }
        }

        public static void GetListaDecodificaGestioneQuotaFondoIntegrativo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            out List<DecodificaGestioneQuotaFondoIntegrativo> listaDecodificaGestioneQuotaFondoIntegrativo)
        {
            listaDecodificaGestioneQuotaFondoIntegrativo = null;
            List<GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo> elencoCodeGestioneQuotaFondoIntegrativoCommon = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoIntegrativo.ToList();

            if (elencoCodeGestioneQuotaFondoIntegrativoCommon != null)
            {
                listaDecodificaGestioneQuotaFondoIntegrativo = new List<DecodificaGestioneQuotaFondoIntegrativo>();

                foreach (GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo codGestioneCommon in elencoCodeGestioneQuotaFondoIntegrativoCommon)
                {
                    DecodificaGestioneQuotaFondoIntegrativo codGestione = new DecodificaGestioneQuotaFondoIntegrativo();
                    codGestione.Id = codGestioneCommon.Id;
                    codGestione.Descrizione = codGestioneCommon.Descrizione;
                    codGestione.TraduzioneSuGP = codGestioneCommon.TraduzioneSuGP;
                    listaDecodificaGestioneQuotaFondoIntegrativo.Add(codGestione);
                }
            }
        }

        public static void GetListaDecodificaGestioneQuotaFondoINPGI(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            out List<DecodificaGestioneQuotaFondoINPGI> listaDecodificaGestioneQuotaFondoINPGI)
        {
            listaDecodificaGestioneQuotaFondoINPGI = null;
            List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGICommon = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI.ToList();

            //ENG - Aggiornamento Memo INPGI 
            GestioneControlliDinamici.ControlloDinamico ctrlAggiornamentoMemoINPGI_20240307 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20240307", out ctrlAggiornamentoMemoINPGI_20240307);
            //Se la chiave non è attiva allora la nuova quota D (G1) non deve essere visibile
            if (ctrlAggiornamentoMemoINPGI_20240307 == null || String.IsNullOrEmpty(ctrlAggiornamentoMemoINPGI_20240307.ValoreControllo) || ctrlAggiornamentoMemoINPGI_20240307.ValoreControllo.Trim().ToUpperInvariant() == "NO")
            {
                if (elencoCodeGestioneQuotaFondoINPGICommon != null)
                    elencoCodeGestioneQuotaFondoINPGICommon.RemoveAll(x => x.TraduzioneSuGP == "G1");
            }

            if (ctrlAggiornamentoMemoINPGI_20240307 != null && !String.IsNullOrEmpty(ctrlAggiornamentoMemoINPGI_20240307.ValoreControllo) && ctrlAggiornamentoMemoINPGI_20240307.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if (Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione) &&
                    contenitore.DatiPensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 01)))
                {
                    if (elencoCodeGestioneQuotaFondoINPGICommon != null)
                        elencoCodeGestioneQuotaFondoINPGICommon.RemoveAll(x => x.TraduzioneSuGP != "F3" && x.TraduzioneSuGP != "F4" && x.TraduzioneSuGP != "F5" && x.TraduzioneSuGP != "G1" && x.TraduzioneSuGP != "F8");
                }
                else
                {
                    if (elencoCodeGestioneQuotaFondoINPGICommon != null)
                        elencoCodeGestioneQuotaFondoINPGICommon.RemoveAll(x => x.TraduzioneSuGP == "G1");
                }
            }

            //ENG - INPGI migrate
            if (!(((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || Utility.IsDomandaRipristino(contenitore.DatiPensione).Value) ||
                (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2"))
            {
                if (elencoCodeGestioneQuotaFondoINPGICommon != null)
                    elencoCodeGestioneQuotaFondoINPGICommon.RemoveAll(x => x.TraduzioneSuGP == "FA");
            }

            if (elencoCodeGestioneQuotaFondoINPGICommon != null)
            {
                listaDecodificaGestioneQuotaFondoINPGI = new List<DecodificaGestioneQuotaFondoINPGI>();

                foreach (GestioneDecodifica.CodeGestioneQuotaFondoINPGI codGestioneCommon in elencoCodeGestioneQuotaFondoINPGICommon)
                {
                    DecodificaGestioneQuotaFondoINPGI codGestione = new DecodificaGestioneQuotaFondoINPGI();
                    codGestione.Id = codGestioneCommon.Id;
                    codGestione.Descrizione = codGestioneCommon.Descrizione;
                    codGestione.TipoQuota = codGestioneCommon.TipoQuota;
                    codGestione.TraduzioneSuGP = codGestioneCommon.TraduzioneSuGP;
                    codGestione.PeriodoDal = codGestioneCommon.PeriodoDal;
                    codGestione.PeriodoAl = codGestioneCommon.PeriodoAl;
                    listaDecodificaGestioneQuotaFondoINPGI.Add(codGestione);
                }
            }
        }

        public static void GetListaDecEnteGestioneFondo(EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecEnteGestioneFondo> listaDecEnteGestioneFondo)
        {
            listaDecEnteGestioneFondo = null;
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            List<GestioneDecodifica.DecEnteGestioneFondo> listaDecEnteGestioneFondoDB = contenitoreDecodifica.ElencoDecEnteGestioneFondo;

            if (listaDecEnteGestioneFondoDB != null && listaDecEnteGestioneFondoDB.Count > 0)
            {
                GetListaDecEnteGestioneFondoCustom(contenitore, ref listaDecEnteGestioneFondoDB);

                listaDecEnteGestioneFondo = new List<DecEnteGestioneFondo>();

                foreach (GestioneDecodifica.DecEnteGestioneFondo decEnteGestioneFondoDB in listaDecEnteGestioneFondoDB)
                {
                    DecEnteGestioneFondo decEnteGestioneFondo = new DecEnteGestioneFondo();
                    Utility.ValorizzaOggetti(decEnteGestioneFondoDB, decEnteGestioneFondo);
                    listaDecEnteGestioneFondo.Add(decEnteGestioneFondo);
                }
            }
        }

        public static void GetListaDecCodiceTrattenute(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecCodiceTrattenute> listaDecCodiceTrattenute)
        {
            listaDecCodiceTrattenute = null;
            if (contenitoreDecodifica.ElencoDecCodiceTrattenute != null && contenitoreDecodifica.ElencoDecCodiceTrattenute.Count > 0)
            {
                listaDecCodiceTrattenute = new List<DecCodiceTrattenute>();
                foreach (GestioneDecodifica.DecCodiceTrattenute decCodiceTrattenuteDB in contenitoreDecodifica.ElencoDecCodiceTrattenute)
                {
                    DecCodiceTrattenute decCodiceTrattenute = new DecCodiceTrattenute();
                    Utility.ValorizzaOggetti(decCodiceTrattenuteDB, decCodiceTrattenute);
                    listaDecCodiceTrattenute.Add(decCodiceTrattenute);
                }
            }
        }
        public static bool ControlsQuotaRetributivaAAlCalcolo(List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi, DateTime? inizioAssicurazione, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, GestionePensione.DatiPensione datiPensione, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo, out string messaggioVideo)
        {
            List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetributivi = MappingDatiRetributiviFromBLToView(listaDatiRetributivi);

            messaggioVideo = string.Empty;

            if (GestioneControlli.BypassaControlliRicSdaiContributivoK(datiPensione, listaDatiContributivi, elencoCodeGestioneCalcoloContributivo) ||
                GestioneControlli.BypassaControlliRic_VOCRED_CRED27_ContributivoL(datiPensione, listaDatiContributivi, elencoCodeGestioneCalcoloContributivo))
                return true;

            if (datiPensione.InizioAssicurazione.HasValue && !ControlsQuotaRetributivaA(datiRetributivi, datiPensione, out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsQuotaRetributivaA(List<GestioneAggiornamentoPECO.DatiRetributivi> listaDatiRetributivi, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                if (!Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1993, 01, 01)))
                {
                    if (listaDatiRetributivi.FindIndex(x => x.Quota == 'A') == -1)
                    {
                        messaggioVideo = string.Format("Quota 'A' obbligatoria nella sezione dei dati calcolo per la seguente data di inizio assicurazione: {0:dd/MM/yyyy}.", datiPensione.InizioAssicurazione.Value);
                        return false;
                    }
                }
                else
                {
                    if (listaDatiRetributivi.FindIndex(x => x.Quota == 'A' && !IsQuotaFittiziaAPresente(x)) > -1)
                    {
                        messaggioVideo = string.Format("Quota 'A' non ammessa nella sezione dei dati calcolo per la seguente data di inizio assicurazione: {0:dd/MM/yyyy}.", datiPensione.InizioAssicurazione.Value);
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsQuotaRetributivaBAlCalcolo(List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi, DateTime? fineAssicurazione, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, GestionePensione.DatiPensione datiPensione, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo, out string messaggioVideo)
        {
            List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetributivi = MappingDatiRetributiviFromBLToView(listaDatiRetributivi);

            messaggioVideo = string.Empty;

            if (GestioneControlli.BypassaControlliRicSdaiContributivoK(datiPensione, listaDatiContributivi, elencoCodeGestioneCalcoloContributivo) ||
                GestioneControlli.BypassaControlliRic_VOCRED_CRED27_ContributivoL(datiPensione, listaDatiContributivi, elencoCodeGestioneCalcoloContributivo))
                return true;

            if (fineAssicurazione.HasValue && !ControlsQuotaRetributivaB(datiRetributivi, fineAssicurazione, out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsQuotaRetributivaB(List<GestioneAggiornamentoPECO.DatiRetributivi> listaDatiRetributivi, DateTime? fineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // Questo controllo vale solo per il tipo calcolo retributivo e non per il tipo calcolo misto
            if (Utility.DataStrettamenteSuccessivaA(fineAssicurazione.Value, new DateTime(1992, 12, 31)))
            {
                if (!Utility.DataSuccessivaA(fineAssicurazione.Value, new DateTime(2012, 01, 01)))
                {
                    if (listaDatiRetributivi.FindIndex(x => x.Quota == 'B') == -1)
                    {
                        messaggioVideo = string.Format("Quota 'B' obbligatoria nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy}.", fineAssicurazione.Value);
                        return false;
                    }
                }
            }
            else
            {
                if (listaDatiRetributivi.FindIndex(x => x.Quota == 'B') > -1)
                {
                    messaggioVideo = string.Format("Quota 'B' non ammessa nella sezione dei dati calcolo per la seguente data di fine assicurazione: {0:dd/MM/yyyy} (adeguare eventualmente la data fine assicurazione con quella dell'ultimo contributo utilizzato per il calcolo della pensione).", fineAssicurazione.Value);
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsPresenzaDatiRetributiviAlCalcolo(List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, GestionePensione.DatiPensione datiPensione, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo, out string messaggioVideo)
        {
            List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetributivi = MappingDatiRetributiviFromBLToView(listaDatiRetributivi);

            messaggioVideo = string.Empty;

            if (GestioneControlli.BypassaControlliRicSdaiContributivoK(datiPensione, listaDatiContributivi, elencoCodeGestioneCalcoloContributivo) ||
                GestioneControlli.BypassaControlliRic_VOCRED_CRED27_ContributivoL(datiPensione, listaDatiContributivi, elencoCodeGestioneCalcoloContributivo))
                return true;

            if (!ControlsPresenzaDatiRetributivi(datiRetributivi,datiPensione, out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsPresenzaDatiRetributivi(List<GestioneAggiornamentoPECO.DatiRetributivi> listaDatiRetributivi, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                if (!listaDatiRetributivi.Exists(x => !IsQuotaFittiziaAPresente(x)))
                {
                    messaggioVideo = "E' obbligatorio inserire almeno un record retributivo diverso dalla quota fittizia.";
                    return false;
                }
            }

            return true;
        }

        private static bool IsQuotaFittiziaAPresente(GestioneAggiornamentoPECO.DatiRetributivi datiRetributivi)
        {
            if (datiRetributivi.Quota == 'A' && datiRetributivi.SettimaneA == 1 &&
                (datiRetributivi.RMSQuotaA == 0.004M || datiRetributivi.RMSQuotaA == 0.001M || datiRetributivi.RMSQuotaA == 0.01M))
                return true;

            return false;
        }

        public static bool ControlsPresenzaQuotaCAlCalcolo(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloContributivo> lDatiContributivi, List<GestioneCalcolo.DatiCalcoloRetributivo> lDatiRetributivi, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, ref EntityBLCommon.ContenitoreObject contenitore, out string messaggioVideo)
        {
            List<GestioneAggiornamentoPECO.DatiContributivi> datiContributivi = MappingDatiContributiviFromBLToView(ref contenitoreDecodifica, ref contenitore, lDatiContributivi, datiPensione);
            List<GestioneAggiornamentoPECO.DatiRetributivi> datiRetributivi = MappingDatiRetributiviFromBLToView(lDatiRetributivi);

            messaggioVideo = string.Empty;

            if (GestioneControlli.BypassaControlliRicSdaiContributivoK(datiPensione, lDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo) ||
                GestioneControlli.BypassaControlliRic_VOCRED_CRED27_ContributivoL(datiPensione, lDatiContributivi, contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo))
                return true;
            if (!Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                if (!ControlsQuotaContributivaC(datiPensione, datiContributivi, datiRetributivi, out messaggioVideo))
                    return false;
            }

            return true;
        }

        public static bool ControlsQuotaContributivaC(GestionePensione.DatiPensione datiPensione, List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, List<GestioneAggiornamentoPECO.DatiRetributivi> lDatiRetributivi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //ENG - Bypassare controllo per le domande automatiche che hanno solo quota A e quota D
            //ENG - Aggiunto bypass CALCOLO_MISTO_NO_QUOTA_C
            if (!Utility.IsRiaperturaDomanda(datiPensione.Id) && !lDatiContributivi.Exists(x => (x.Settimane.HasValue || x.MontanteContributivo.HasValue || x.ImportoContributivo.HasValue))
                && !(Utility.IsDomandaAutomatica(datiPensione) && ((lDatiContributivi.Exists(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) &&
                lDatiRetributivi != null && lDatiRetributivi.Exists(x => (x.SettimaneA.HasValue || x.RMSQuotaA.HasValue)) && !lDatiRetributivi.Exists(x => (x.SettimaneB.HasValue || x.RMSQuotaB.HasValue))) ||
                (lDatiContributivi.Exists(x => (x.SettimaneQuotaD.HasValue || x.MontanteContributivoQuotaD.HasValue || x.ImportoContributivoQuotaD.HasValue)) &&
                lDatiRetributivi != null && lDatiRetributivi.Exists(x => (x.SettimaneA.HasValue || x.RMSQuotaA.HasValue)) && lDatiRetributivi.Exists(x => (x.SettimaneB.HasValue || x.RMSQuotaB.HasValue))))) &&
                !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_AGO.CALCOLO_MISTO_NO_QUOTA_C))
            {
                messaggioVideo = "Per il tipo calcolo Misto è necessario inserire la quota C";
                return false;
            }

            return true;
        }

        private static bool ControlsDatiQuotaFondoINPGI(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiQuotaFondoINPGI datiQuotaFondoINPGI, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!contenitore.DatiPensione.InizioAssicurazione.HasValue)
            {
                messaggioVideo = "Data 'Inizio Assicurazione' assente; verificare nella sezione 'Liquidazione Pensione'.";
                return false;
            }

            if ((datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI == null || datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI.Count == 0) &&
            (datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI == null || datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI.Count == 0))
            {
                messaggioVideo = "Quota GI obbligatoria.";
                return false;
            }

            if (datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI != null && datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI.Count > 0)
            {
                GestioneDecodifica.CodeGestioneQuotaFondoINPGI gestione;
                DateTime? startDate;
                DateTime? endDate;

                foreach (GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI contr in datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI)
                {
                    gestione = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI.Where(x => x.Id == contr.CodiceGestione).Select(x => x).First();
                    startDate = gestione.PeriodoDal.HasValue ? gestione.PeriodoDal : null;
                    endDate = gestione.PeriodoAl.HasValue ? gestione.PeriodoAl : null;
                    if (!startDate.HasValue && endDate.HasValue)
                    {
                        if (!Utility.DataSuccessivaA(endDate.GetValueOrDefault(), contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault()))
                        {
                            messaggioVideo = "Quota GI non ammessa per Inizio Assicurazione " + contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault().ToString("dd/MM/yyyy");
                            return false;
                        }
                    }
                    else if (startDate.HasValue && endDate.HasValue)
                    {
                        if (!Utility.DataSuccessivaA(endDate.GetValueOrDefault(), contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault()))
                        {
                            messaggioVideo = "Quota GI non ammessa per Inizio Assicurazione " + contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault().ToString("dd/MM/yyyy");
                            return false;
                        }
                    }
                }

                if ((contenitore.DatiPensioniDatiGenerici == null || contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf == null) &&
                    !((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || Utility.IsDomandaRipristino(contenitore.DatiPensione).Value ||
                    (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2"))
                {
                    messaggioVideo = "Coefficiente: campo obbligatorio";
                    return false;
                }
            }

            if (datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI != null && datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI.Count > 0)
            {
                GestioneDecodifica.CodeGestioneQuotaFondoINPGI gestione;
                DateTime? startDate;
                DateTime? endDate;
                foreach (GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI retr in datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI)
                {
                    gestione = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI.Where(x => x.Id == retr.CodiceGestione).Select(x => x).First();
                    startDate = gestione.PeriodoDal.HasValue ? gestione.PeriodoDal : null;
                    endDate = gestione.PeriodoAl.HasValue ? gestione.PeriodoAl : null;
                    if (!startDate.HasValue && endDate.HasValue)
                    {
                        if (!Utility.DataSuccessivaA(endDate.GetValueOrDefault(), contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault()))
                        {
                            messaggioVideo = "Quota GI non ammessa per Inizio Assicurazione " + contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault().ToString("dd/MM/yyyy");
                            return false;
                        }
                    }
                    else if (startDate.HasValue && endDate.HasValue)
                    {
                        if (!Utility.DataSuccessivaA(endDate.GetValueOrDefault(), contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault()))
                        {
                            messaggioVideo = "Quota GI non ammessa per Inizio Assicurazione " + contenitore.DatiPensione.InizioAssicurazione.GetValueOrDefault().ToString("dd/MM/yyyy");
                            return false;
                        }
                    }
                }
            }

            if (datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI != null && datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI.Count > 0 &&
                datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI != null && datiQuotaFondoINPGI.lDatiContributiviQuotaFondoINPGI.Count > 0 &&
                datiQuotaFondoINPGI.lDatiRetributiviQuotaFondoINPGI.Where(x => x.CodiceGestione == 7).Count() > 0)
            {
                messaggioVideo = "La quota F e la quota F1 devono essere alternative.";
                return false;
            }
            return true;
        }

        private static bool IsAnz95BloccatoForExInpdai(GestionePensione.DatiPensione datiPensione)
        {
            bool ret = false;
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                return true;
            if ((datiPensione.SiglaCategoria == "VDAI" || datiPensione.SiglaCategoria == "SDAI") &&
                (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || Utility.IsRiaperturaDomanda(datiPensione.Id)) &&
                datiPensione.DataAcquisizione.HasValue && !Utility.DataSuccessivaA(datiPensione.DataAcquisizione.Value, new DateTime(2003, 12, 01)))
                ret = true;
            return ret;
        }


        private static bool IsPrimoRecordRetrGestioneSForExInpdai(GestionePensione.DatiPensione datiPensione)
        {
            bool ret = false;
            string filtro = datiPensione.GetFiltro();
            if (filtro == "SCO" || filtro == "BNS" || filtro == "BNX" || filtro == "B44" || filtro == "B45")
                ret = true;
            return ret;
        }

        public static Utility.DifferenzaDateTime GetDecorrenzaCalcoloRetrExInpdai(GestioneDanteCausa.DatiDanteCausa datiDA, DateTime? decorrenzaOriginaria,
            GestioneDatiControlloFelpe.ControlloFelpe controlloFelpe, GestionePensione.DatiPensione datiPensione, Utility.DifferenzaDateTime decorrenzaDatiRetributivi)
        {
            Utility.DifferenzaDateTime dateRet = null;
            //Se il Campo DAU107 è = a “1” o “2” mettere il Campo DAU106AA in PAA
            if (datiDA != null && datiDA.ProvenienzaPensione.HasValue && (datiDA.ProvenienzaPensione.Value == 1 || datiDA.ProvenienzaPensione.Value == 2) && datiDA.DecorrenzaPensione.HasValue)
                dateRet = new Utility.DifferenzaDateTime(datiDA.DecorrenzaPensione.Value);
            //altrimenti mettere il Campo RAU104AA in PAA
            else if (decorrenzaOriginaria.HasValue)
                dateRet = new Utility.DifferenzaDateTime(decorrenzaOriginaria.Value);

            //posticipo
            if (datiPensione.GetFiltro() == "SCO" || datiPensione.GetFiltro() == "BNS" || datiPensione.GetFiltro() == "BNX" || datiPensione.GetFiltro() == "B44" || datiPensione.GetFiltro() == "B45")
            {
                if (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue)
                    dateRet = new Utility.DifferenzaDateTime(controlloFelpe.InizioBonus.Value);
            }

            if (decorrenzaDatiRetributivi != null)
                dateRet = decorrenzaDatiRetributivi;

            return dateRet;
        }

        private static void InsertQuotaFittiziaA(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiCalcolo datiCalcolo)
        {
            switch (contenitore.DatiPensione.SiglaCategoria.Trim())
            {
                case "VO":
                case "IO":
                case "SO":
                    if ((datiCalcolo.TipoCalcolo == TipoCalcolo.Retributivo || datiCalcolo.TipoCalcolo == TipoCalcolo.Misto) && contenitore.DatiPensione.InizioAssicurazione.HasValue &&
                        Utility.DataSuccessivaA(contenitore.DatiPensione.InizioAssicurazione.Value, new DateTime(1993, 01, 01)))
                    {
                        if ((datiCalcolo.lDatiRetributivi != null && datiCalcolo.lDatiRetributivi.Count > 0 && datiCalcolo.lDatiRetributivi.FindIndex(x => x.Quota == 'A') == -1) ||
                            (datiCalcolo.lDatiRetributivi == null || datiCalcolo.lDatiRetributivi.Count == 0))
                        {
                            GestioneAggiornamentoPECO.DatiRetributivi quotaAFittizia = new GestioneAggiornamentoPECO.DatiRetributivi();
                            quotaAFittizia.CodGestione = 1;
                            quotaAFittizia.Quota = 'A';
                            quotaAFittizia.RMSQuotaA = 0.004M;
                            quotaAFittizia.SettimaneA = 1;
                            if (IsSettimane707Visible(contenitore.DatiPensione, ref contenitoreDecodifica, datiCalcolo.lDatiRetributivi, datiCalcolo.lDatiContributivi, contenitore.DatiBeneficioVittimeTerrorismo, contenitore.TipoCalcolo, contenitore.DatiDanteCausa))
                                quotaAFittizia.NSettimane707 = 1;
                            if (datiCalcolo.lDatiRetributivi == null)
                                datiCalcolo.lDatiRetributivi = new List<GestioneAggiornamentoPECO.DatiRetributivi>();

                            datiCalcolo.lDatiRetributivi.Add(quotaAFittizia);
                        }
                    }
                    break;
                case "VDAI":
                case "IDAI":
                case "SDAI":
                    if (datiCalcolo.TipoCalcolo == TipoCalcolo.Retributivo || datiCalcolo.TipoCalcolo == TipoCalcolo.Misto)
                    {
                        List<DatiRetributiviExInpdai> lstDatiRetrExInpdai = null;

                        List<Liquidazione.BLCommon.CtrlDecorrenzaRetrExINPDAI> lstCtrlDecorrenza = contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI;
                        List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcoloRetributivo = contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo;
                        if (datiCalcolo.lDatiRetributivi != null && datiCalcolo.lDatiRetributivi.Count > 0)
                            lstDatiRetrExInpdai = datiCalcolo.lDatiRetributivi.Select(x => new DatiRetributiviExInpdai(x, lstCtrlDecorrenza, lstDecGestioneCalcoloRetributivo)).ToList();

                        if ((lstDatiRetrExInpdai != null && lstDatiRetrExInpdai.Count > 0 &&
                                lstDatiRetrExInpdai.FindIndex(x => (x.DecCodGestione == "A" || x.DecCodGestione == "S") && x.Quota == 'A' && x.CodiceTipoQuota == "A1") == -1) ||
                            (datiCalcolo.lDatiRetributivi == null || datiCalcolo.lDatiRetributivi.Count == 0))
                        {
                            GestioneAggiornamentoPECO.DatiRetributivi quotaAFittizia = new GestioneAggiornamentoPECO.DatiRetributivi();
                            quotaAFittizia.CodGestione = lstDecGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP.Trim() == "A").Id;
                            quotaAFittizia.Quota = 'A';
                            quotaAFittizia.CodiceTipoQuota = "A1";
                            quotaAFittizia.RMSQuotaA = 0.004M;
                            quotaAFittizia.SettimaneA = 1;
                            if (IsSettimane707Visible(contenitore.DatiPensione, ref contenitoreDecodifica, datiCalcolo.lDatiRetributivi, datiCalcolo.lDatiContributivi, contenitore.DatiBeneficioVittimeTerrorismo, contenitore.TipoCalcolo, contenitore.DatiDanteCausa))
                                quotaAFittizia.NSettimane707 = 1;
                            if (datiCalcolo.lDatiRetributivi == null)
                                datiCalcolo.lDatiRetributivi = new List<GestioneAggiornamentoPECO.DatiRetributivi>();

                            datiCalcolo.lDatiRetributivi.Insert(0, quotaAFittizia);
                        }
                    }
                    break;
            }
        }

        public static bool IsSettimane707Visible(GestionePensione.DatiPensione datiPensione, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<GestioneAggiornamentoPECO.DatiRetributivi> lDatiRetributivi,
            List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            Utility.TipoCalcolo tipoCalcolo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            List<GestioneCalcolo.DatiCalcoloRetributivo> datiRetributivi = MappingDatiRetributiviFromViewToBL(lDatiRetributivi);
            List<GestioneCalcolo.DatiCalcoloContributivo> datiContributivi = MappingDatiContributiviFromViewToBL(ref contenitoreDecodifica, lDatiContributivi, datiPensione, datiDanteCausa);

            return GestioneContrib.IsSettimane707Visible(datiPensione, datiRetributivi, datiContributivi, datiBeneficioVittimeTerrorismo, tipoCalcolo);
        }

        public static bool IsSettimane707INPGIVisible(Utility.TipoCalcolo tipoCalcolo, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa danteCausa)
        {
            return GestioneContrib.IsSettimane707INPGIVisible(datiPensione, tipoCalcolo, danteCausa);
        }

        public static bool IsSettimane707Visible(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloRetributivo> lDatiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> lDatiContributivi, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, Utility.TipoCalcolo tipoCalcolo)
        {
            DateTime dataCompare = new DateTime(2012, 1, 1);

            // Per le domande di vittime del terrorismo è stato inibito momentaneamente il comma 707
            if (Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo)
                || (datiPensione.NaturaPensione != null && datiPensione.NaturaPensione.Substring(0, 1) == "5"))
                return false;

            if (tipoCalcolo == Utility.TipoCalcolo.Retributivo && datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, dataCompare) &&
                (string.IsNullOrEmpty(datiPensione.NaturaPensione) || (datiPensione.NaturaPensione.Substring(0, 1) != "3" && datiPensione.NaturaPensione.Substring(0, 1) != "4")))
            {
                // Per le domande VESO33, VESO92, VOCOOP, VOESO, VOCRED la decorrenza deve essere maggiore o uguale al 01/01/2015 e deve essere presente la quota D
                if (Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria))
                    if (datiPensione.DecorrenzaOriginaria.HasValue && (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)) || (lDatiRetributivi != null && lDatiRetributivi.Count > 0 &&
                        (lDatiContributivi == null || lDatiContributivi.Count(x => x.IsQuotaDL214Presente()) == 0))))
                        return false;

                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                {
                    // Per le domande provenienti da Felpe, se non è presente la quota D, allora non si applica il comma 707
                    if (lDatiRetributivi != null && lDatiRetributivi.Count > 0 && (lDatiContributivi == null || lDatiContributivi.Count(x => x.IsQuotaDL214Presente()) == 0))
                        return false;

                    if (!Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) && !Utility.IsDomandaENPALS(datiPensione.Gestione) && lDatiRetributivi != null &&
                        lDatiRetributivi.Sum(x => x.NSettimaneQuotaA.GetValueOrDefault()) < 936 && !lDatiRetributivi.Any(x => x.NSettimane707.HasValue))
                        return false;
                }

                // Rimosso a seguito della mail del 31/03/2016 di Alessio con oggetto "RE: LiqPens AGO - Doppio calcolo segnalazione di produzione"
                //// Solo per le domande provenienti da Felpe non DAI, non si applica il comma 707 (e tutti i controlli collegati) se la somma delle settimane della quota A 
                //// di qualsiasi gestione è inferiore a 780 settimane
                //if (!Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Lettura_L &&
                //    lDatiRetributivi != null && lDatiRetributivi.Sum(x => x.NSettimaneQuotaA.GetValueOrDefault()) < 780)
                //    return false;

                return true;
            }

            return false;
        }

        public static bool IsSettimane707INPGIVisible(GestionePensione.DatiPensione datiPensione, Utility.TipoCalcolo tipoCalcolo, GestioneDanteCausa.DatiDanteCausa danteCausa)
        {
            DateTime dataCompare = new DateTime(2012, 1, 1);

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa))
                return false;

            if ((tipoCalcolo == Utility.TipoCalcolo.Retributivo && datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, dataCompare) &&
                (string.IsNullOrEmpty(datiPensione.NaturaPensione) || (datiPensione.NaturaPensione.Substring(0, 1) != "3" && datiPensione.NaturaPensione.Substring(0, 1) != "4"))))
            {
                return true;
            }

            return false;
        }

        public static bool IsBeneficioImportoPensioneX(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            if (Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo) && datiBeneficioVittimeTerrorismo != null && datiBeneficioVittimeTerrorismo.SoggettoBeneficiario.GetValueOrDefault() == 2 &&
                    datiBeneficioVittimeTerrorismo.TipologiaPrestazione.GetValueOrDefault() == 3 && datiBeneficioVittimeTerrorismo.TipologiaBeneficio.GetValueOrDefault() == 5)
                return true;

            return false;
        }

        public static bool IsSettimaneImportoPensioneLocked(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            if (Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) ||
                (Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo) && datiBeneficioVittimeTerrorismo != null && datiBeneficioVittimeTerrorismo.SoggettoBeneficiario.GetValueOrDefault() == 2 &&
                    datiBeneficioVittimeTerrorismo.TipologiaPrestazione.GetValueOrDefault() == 3 && datiBeneficioVittimeTerrorismo.TipologiaBeneficio.GetValueOrDefault() == 5))
                return true;

            return false;
        }

        #region private methods

        private static void GetListaDecEnteGestioneFondoCustom(EntityBLCommon.ContenitoreObject contenitore, ref List<GestioneDecodifica.DecEnteGestioneFondo> listaDecEnteGestioneFondo)
        {
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            if (Utility.IsDomandaAPEPrecoci(datiPensione))
                listaDecEnteGestioneFondo = listaDecEnteGestioneFondo.Where(x => new List<string> { "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "B1", "B2", "B3", "B4", "B6", "C1", "C2", "C3", "C4", "C5", "D1", "E1", "E2" }.Contains(x.Codice)).ToList();

            if ((Utility.IsDomandaIOCUM(datiPensione.SiglaCategoria) || Utility.IsDomandaSOCUM(datiPensione.SiglaCategoria)) && !Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id))
            {
                var listaDaRimuovere = new List<string> { "C0", "D0", "E0", "B3", "B5", "SP" };

                listaDecEnteGestioneFondo.RemoveAll(x => listaDaRimuovere.Contains(x.Codice));
            }
            else if ((Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria)) && !Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id))
            {
                var listaDaRimuovere = new List<string> { "C0", "D0", "E0" };

                listaDecEnteGestioneFondo.RemoveAll(x => listaDaRimuovere.Contains(x.Codice));
            }
        }

        private static bool ControlsDatiCalcoloRendita(DatiCalcolo datiCalcolo, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (Utility.IsRenditaCasalinghe(datiPensione) || Utility.IsRenditaFacoltativa(datiPensione))
            {
                if (datiCalcolo != null && datiCalcolo.ImportoMensileAlGennaio2001.HasValue && datiCalcolo.ImportoMensileAlGennaio2001.Value <= datiCalcolo.ImportoMensileAllaDecorrenzaOriginaria.GetValueOrDefault() && !Utility.IsRicostituzione(datiPensione.Gruppo))
                {
                    messaggioVideo = "L'Importo Mensile al Gennaio 2001 deve essere strettamente maggiore all'Importo Mensile alla Decorrenza Originaria";
                    return false;
                }
            }

            return true;
        }

        #endregion private methods

        #region nested class
        public class DatiCalcolo
        {
            public DatiCalcolo()
            { }

            public DatiCalcolo(GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec)
            {
                if (datiAggPec == null || datiAggPec.IsNull())
                    return;

                if (datiAggPec.lRetribuzione != null)
                    this._lDatiRetributivi = datiAggPec.lRetribuzione;

                if (datiAggPec.lContribuzione != null)
                    this._lDatiContributivi = datiAggPec.lContribuzione;

                if (datiAggPec.DatiInpdai != null)
                {
                    this._Anz95 = datiAggPec.DatiInpdai.Anz95 != 0 ? datiAggPec.DatiInpdai.Anz95 : (decimal?)null;
                    this._Quota95 = !Utility.IsDoubleEquals(datiAggPec.DatiInpdai.Quota95, 0) ? Convert.ToDecimal(datiAggPec.DatiInpdai.Quota95) : (decimal?)null;
                }

                if (datiAggPec.DatiFlat != null)
                {
                    this.ImportoLordo = datiAggPec.DatiFlat.ImportoLordo;
                    this.PL_Coeftrasf = datiAggPec.DatiFlat.PL_Coeftrasf;
                }

                if (datiAggPec.DatiControllo != null)
                {
                    switch (datiAggPec.DatiControllo.TipoCalcolo)
                    {
                        case GestioneAggiornamentoPECO.TipoCalcolo.Contributivo:
                            this._TipoCalcolo = TipoCalcolo.Contributivo;
                            break;
                        case GestioneAggiornamentoPECO.TipoCalcolo.Misto:
                            this._TipoCalcolo = TipoCalcolo.Misto;
                            break;
                        case GestioneAggiornamentoPECO.TipoCalcolo.Retributivo:
                            this._TipoCalcolo = TipoCalcolo.Retributivo;
                            break;
                        case GestioneAggiornamentoPECO.TipoCalcolo.NonValido:
                            this._TipoCalcolo = TipoCalcolo.NonValido;
                            break;
                    }
                    this._IsCalcoloValido = datiAggPec.DatiControllo.IsCalcoloValido;

                }
            }

            #region private properties

            private TipoCalcolo _TipoCalcolo;
            private bool _IsCalcoloValido;
            private bool _IsUnicarpe;
            private long _IdPensione;
            private bool? _FacoltaComputo;
            private List<GestioneAggiornamentoPECO.DatiContributivi> _lDatiContributivi;
            private List<GestioneAggiornamentoPECO.DatiRetributivi> _lDatiRetributivi;
            private decimal? _ImportoLordoAllaDecorrenza;
            private decimal? _ImportoLordo;
            private decimal? _Anz95;
            private decimal? _Quota95;
            private char? _TipoCalcoloVincenteUnicarpe;
            private decimal? _PL_Coeftrasf;
            private short? _CodiceP18PrecedentePensione;
            private decimal? _ImportoMensileAllaDecorrenzaOriginaria;
            private decimal? _ImportoMensileAlGennaio2001;
            private bool _IsPrimoRecordRetrGestioneS;
            private bool _SbloccaPannelliAnte96;
            private int? _ContributiItalianiEdEsteriAl1295;
            #endregion private properties

            #region public properties

            public TipoCalcolo TipoCalcolo { get { return _TipoCalcolo; } set { _TipoCalcolo = value; } }
            public bool IsCalcoloValido { get { return _IsCalcoloValido; } set { _IsCalcoloValido = value; } }
            public bool IsUnicarpe { get { return _IsUnicarpe; } set { _IsUnicarpe = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public bool? FacoltaComputo { get { return _FacoltaComputo; } set { _FacoltaComputo = value; } }
            public List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi { get { return _lDatiContributivi; } set { _lDatiContributivi = value; } }
            public List<GestioneAggiornamentoPECO.DatiRetributivi> lDatiRetributivi { get { return _lDatiRetributivi; } set { _lDatiRetributivi = value; } }
            public decimal? ImportoLordoAllaDecorrenza { get { return _ImportoLordoAllaDecorrenza; } set { _ImportoLordoAllaDecorrenza = value; } }
            public decimal? ImportoLordo { get { return _ImportoLordo; } set { _ImportoLordo = value; } }
            public decimal? Anz95 { get { return _Anz95; } set { _Anz95 = value; } }
            public decimal? Quota95 { get { return _Quota95; } set { _Quota95 = value; } }
            public char? TipoCalcoloVincenteUnicarpe { get { return _TipoCalcoloVincenteUnicarpe; } set { _TipoCalcoloVincenteUnicarpe = value; } }
            public decimal? PL_Coeftrasf { get { return _PL_Coeftrasf; } set { _PL_Coeftrasf = value; } }
            public short? CodiceP18PrecedentePensione { get { return _CodiceP18PrecedentePensione; } set { _CodiceP18PrecedentePensione = value; } }
            public decimal? ImportoMensileAllaDecorrenzaOriginaria { get { return _ImportoMensileAllaDecorrenzaOriginaria; } set { _ImportoMensileAllaDecorrenzaOriginaria = value; } }
            public decimal? ImportoMensileAlGennaio2001 { get { return _ImportoMensileAlGennaio2001; } set { _ImportoMensileAlGennaio2001 = value; } }
            public bool IsPrimoRecordRetrGestioneS { get { return _IsPrimoRecordRetrGestioneS; } set { _IsPrimoRecordRetrGestioneS = value; } }
            public bool SbloccaPannelliAnte96 { get { return _SbloccaPannelliAnte96; } set { _SbloccaPannelliAnte96 = value; } }
            public int? ContributiItalianiEdEsteriAl1295 { get { return _ContributiItalianiEdEsteriAl1295; } set { _ContributiItalianiEdEsteriAl1295 = value; } }
            #endregion public properties
        }

        public class DatiCalcoloENPALS
        {
            #region private properties
            private decimal? _ImportoQuotaRetributivaInMisto;
            private decimal? _ImportoProRataTemporis;
            private decimal? _ImportoPensione;
            private decimal? _ImportoPensione707;
            private List<DatiContributiviENPALS> _lDatiContributivi;
            private List<DatiRetributiviENPALS> _lDatiRetributivi;
            private string _DecorrenzaImportoPensione;
            private decimal? _ImportoIIS;
            private DateTime? _DecorrenzaImportoIIS;
            #endregion private properties

            #region public properties
            public decimal? ImportoQuotaRetributivaInMisto { get { return _ImportoQuotaRetributivaInMisto; } set { _ImportoQuotaRetributivaInMisto = value; } }
            public decimal? ImportoProRataTemporis { get { return _ImportoProRataTemporis; } set { _ImportoProRataTemporis = value; } }
            public decimal? ImportoPensione { get { return _ImportoPensione; } set { _ImportoPensione = value; } }
            public decimal? ImportoPensione707 { get { return _ImportoPensione707; } set { _ImportoPensione707 = value; } }
            public List<DatiContributiviENPALS> LDatiContributivi { get { return _lDatiContributivi; } set { _lDatiContributivi = value; } }
            public List<DatiRetributiviENPALS> LDatiRetributivi { get { return _lDatiRetributivi; } set { _lDatiRetributivi = value; } }
            public string DecorrenzaImportoPensione { get { return _DecorrenzaImportoPensione; } set { _DecorrenzaImportoPensione = value; } }
            public decimal? ImportoIIS { get { return _ImportoIIS; } set { _ImportoIIS = value; } }
            public DateTime? DecorrenzaImportoIIS { get { return _DecorrenzaImportoIIS; } set { _DecorrenzaImportoIIS = value; } }
            #endregion public properties
        }

        public class DatiContributiviENPALS
        {
            public DatiContributiviENPALS()
            { }
            public DatiContributiviENPALS(decimal? importoContributivoTotale, decimal? montante, decimal coefficienteTrasformazione, char? quota, string decorrenza, int? numeroContributiTotale)
            {
                this._ImportoContributivoTotale = importoContributivoTotale;
                this._Montante = montante;
                this._CoefficienteTrasformazione = coefficienteTrasformazione;
                this._Quota = quota;
                this._Decorrenza = decorrenza;
                this._NumeroContributiTotale = numeroContributiTotale;
            }

            #region private properties
            private decimal? _ImportoContributivoTotale;
            private decimal? _Montante;
            private decimal? _CoefficienteTrasformazione;
            private char? _Quota;
            private string _Decorrenza;
            private int? _NumeroContributiTotale;
            #endregion private properties

            #region public properties
            public decimal? ImportoContributivoTotale { get { return _ImportoContributivoTotale; } set { _ImportoContributivoTotale = value; } }
            public decimal? Montante { get { return _Montante; } set { _Montante = value; } }
            public decimal? CoefficienteTrasformazione { get { return _CoefficienteTrasformazione; } set { _CoefficienteTrasformazione = value; } }
            public char? Quota { get { return _Quota; } set { _Quota = value; } }
            public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public int? NumeroContributiTotale { get { return _NumeroContributiTotale; } set { _NumeroContributiTotale = value; } }
            #endregion public properties
        }

        public class DatiRetributiviENPALS
        {
            public DatiRetributiviENPALS()
            { }
            public DatiRetributiviENPALS(char quota, short Periodi, int NTotaleContributiCalcolo, decimal rm, decimal importo, decimal importoProRataTemporis, short? giorni707, decimal? importo707)
            {
                this._Quota = quota;
                this._Periodi = Periodi;
                this._NTotaleContributiCalcolo = NTotaleContributiCalcolo;
                this._RM = rm;
                this._Importo = importo;
                this._Giorni707 = giorni707;
                this._Importo707 = importo707;
            }
            #region private properties
            private char? _Quota;
            private short? _Periodi;
            private int? _NTotaleContributiCalcolo;
            private decimal? _RM;
            private decimal? _Importo;
            private short? _Giorni707;
            private decimal? _Importo707;
            private string _Decorrenza;
            #endregion private properties

            #region public properties
            public char? Quota { get { return _Quota; } set { _Quota = value; } }
            public short? Periodi { get { return _Periodi; } set { _Periodi = value; } }
            public int? NTotaleContributiCalcolo { get { return _NTotaleContributiCalcolo; } set { _NTotaleContributiCalcolo = value; } }
            public decimal? RM { get { return _RM; } set { _RM = value; } }
            public decimal? Importo { get { return _Importo; } set { _Importo = value; } }
            public short? Giorni707 { get { return _Giorni707; } set { _Giorni707 = value; } }
            public decimal? Importo707 { get { return _Importo707; } set { _Importo707 = value; } }
            public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            #endregion public properties
        }

        public class DatiExINPDAI
        {
            public decimal? AnzAl95 { get; set; }
            public decimal? QuotaAl95 { get; set; }
            public decimal? ImportoAl200312 { get; set; }
            public List<DecodificaTipoQuota> DecodificaTipoQuota { get; set; }
            public List<INPS.Pensioni.LiquidazioneAgo.Entity.CtrlDecorrenzaRetrExINPDAI> CtrlDecorrenzaRetrExINPDAI { get; set; }
            public bool IsDataAnzianitaAl95Bloccato { get; set; }
            public bool IsPrimoRecordRetrGestioneS { get; set; }
            public Utility.DifferenzaDateTime DecorrenzaCalcoloRetr { get; set; }
            public bool IsContribSolidarietaVisible { get; set; }
        }

        public class DatiCalcoloQuotePensione
        {
            #region private properties
            private int? _ContributiItalianiEdEsteriAl1295;
            #endregion private properties

            #region public properties
            public List<DatiQuotePensione> LQuotePensione { get; set; }
            public bool IsTrattenuteVisible { get; set; }
            public int? ContributiItalianiEdEsteriAl1295 { get { return _ContributiItalianiEdEsteriAl1295; } set { _ContributiItalianiEdEsteriAl1295 = value; } }
            #endregion public properties
        }

        public class DatiQuotePensione
        {
            #region public properties
            public long Id { get; set; }
            public long IdPensione { get; set; }
            public long EnteGestioneFondo { get; set; }
            public int? Settimane { get; set; }
            public decimal? Importo { get; set; }
            public DateTime? Decorrenza { get; set; }
            public bool IsQuotaProgressiva { get; set; }
            public List<DatiTrattenute> ListaTrattenute { get; set; }

            public class DatiTrattenute
            {
                public long Id { get; set; }
                public short AnnoCompetenza { get; set; }
                public string CodiceTrattenute { get; set; }
                public decimal ImportoTrattenute { get; set; }
            }
            #endregion public properties
        }

        public class DatiCalcoloQuoteMiglioramentiContrattuali
        {
            public List<DatiQuoteMiglioramentiContrattuali> LQuoteMiglioramentiContrattuali { get; set; }
        }

        public class DatiQuoteMiglioramentiContrattuali
        {
            public long Id { get; set; }
            public long? IdPensione { get; set; }
            public string Codice { get; set; }
            public string DataDecorrenza { get; set; }
            public string Quota { get; set; }
            public bool IsStorico { get; set; }
        }

        public class DatiCalcoloVittimeTerrorismo
        {
            #region public properties
            public List<DatiRetributiviVittimeTerrorismo> ListaDatiRetributiviVittimeTerrorismo { get; set; }
            public List<DatiContributiviVittimeTerrorismo> ListaDatiContributiviVittimeTerrorismo { get; set; }
            public List<DatiImportoPensioneVittimeTerrorismo> ListaDatiImportoPensioneVittimeTerrorismo { get; set; }
            #endregion public properties
        }

        public class DatiRetributiviVittimeTerrorismo
        {
            #region public properties
            public DateTime? DecorrenzaBeneficio { get; set; }
            public long? CodiceGestioneRetr { get; set; }
            public char? Quota { get; set; }
            public string CodiceTipoQuota { get; set; }
            public int? Settimane { get; set; }
            public decimal? RMS { get; set; }
            public char? Beneficio { get; set; }
            public bool? IsFromDatiCalcolo { get; set; }
            #endregion public properties
        }

        public class DatiContributiviVittimeTerrorismo
        {
            #region public properties
            public DateTime? DecorrenzaBeneficio { get; set; }
            public long? CodiceGestioneContr { get; set; }
            public char? Quota { get; set; }
            public int? Settimane { get; set; }
            public char? Beneficio { get; set; }
            public decimal? Ammontare { get; set; }
            public decimal? Montante { get; set; }
            public bool? IsFromDatiCalcolo { get; set; }
            #endregion public properties
        }

        public class DatiImportoPensioneVittimeTerrorismo
        {
            #region public properties
            public DateTime? DecorrenzaBeneficio { get; set; }
            public long? CodiceGestioneRetr { get; set; }
            public int? Settimane { get; set; }
            public char? Beneficio { get; set; }
            public decimal? ImportoPensione { get; set; }
            #endregion public properties
        }

        public class DatiQuotaFondoIntegrativo
        {
            public DatiQuotaFondoIntegrativo()
            { }

            public DatiQuotaFondoIntegrativo(List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> lDatiQuotaFondoIntegrativo)
            {
                if (lDatiQuotaFondoIntegrativo == null)
                    return;

                if (lDatiQuotaFondoIntegrativo != null)
                    this._lDatiQuotaFondoIntegrativo = lDatiQuotaFondoIntegrativo;
            }

            #region private properties

            private long _IdPensione;
            private List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> _lDatiQuotaFondoIntegrativo;

            #endregion private properties

            #region public properties

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> lDatiQuotaFondoIntegrativo { get { return _lDatiQuotaFondoIntegrativo; } set { _lDatiQuotaFondoIntegrativo = value; } }

            #endregion public properties
        }

        public class DatiQuotaFondoINPGI
        {
            public DatiQuotaFondoINPGI()
            { }

            #region private properties

            private long _IdPensione;
            private List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> _lDatiContributiviQuotaFondoINPGI;
            private List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> _lDatiRetributiviQuotaFondoINPGI;

            #endregion private properties

            #region public properties

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> lDatiContributiviQuotaFondoINPGI { get { return _lDatiContributiviQuotaFondoINPGI; } set { _lDatiContributiviQuotaFondoINPGI = value; } }
            public List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> lDatiRetributiviQuotaFondoINPGI { get { return _lDatiRetributiviQuotaFondoINPGI; } set { _lDatiRetributiviQuotaFondoINPGI = value; } }

            #endregion public properties
        }
        //ENG - MEMO 74_2023
        public class PrestazioneEsteraCumulo : GestioneDatiEsteriCumulo.PensioneEsteraCumulo
        {
            public PrestazioneEsteraCumulo()
            { }

            public PrestazioneEsteraCumulo(string codiceStatoIstituzione, string sigla, string citta, string nomeStato, string siglaStato, string matricolaIstituzione, string codiceConvenzione, bool confermato)
            {
                this.CodiceStato = codiceStatoIstituzione.Length == 6 ? codiceStatoIstituzione.Substring(0, 2) : "";
                this.CodiceIstituzione = codiceStatoIstituzione.Length == 6 ? codiceStatoIstituzione.Substring(2, 4) : "";
                this._Sigla = sigla;
                this._Citta = citta;
                this._NomeStato = nomeStato;
                this._SiglaStato = siglaStato;
                this._MatricolaIstituzione = matricolaIstituzione;
                this.CodiceConvenzione = Utility.StringToNullableByte(codiceConvenzione);
                this.Confermato = confermato;
            }
            #region private properties
            private string _Sigla;
            private string _Citta;
            private string _NomeStato;
            private string _SiglaStato;
            private string _MatricolaIstituzione;
            private bool _IsStorico;
            #endregion private properties

            #region public properties
            public string Sigla { get { return _Sigla; } set { _Sigla = value; } }
            public string Citta { get { return _Citta; } set { _Citta = value; } }
            public string NomeStato { get { return _NomeStato; } set { _NomeStato = value; } }
            public string SiglaStato { get { return _SiglaStato; } set { _SiglaStato = value; } }
            public string MatricolaIstituzione { get { return _MatricolaIstituzione; } set { _MatricolaIstituzione = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }
            #endregion public properties
        }

        //ENG - MEMO 74_2023
        public class StatoEsteroCumulo
        {
            #region private properties
            private PrestazioneEsteraCumulo _PrestazioneEsteraCumulo;
            private PrestazioneEsteraCumulo _PrestazioneEsteraStorico;
            private List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo> _ElencoImportiEsteriCumulo;
            #endregion private properties

            #region public properties
            public PrestazioneEsteraCumulo PrestazioneEsteraCumulo { get { return _PrestazioneEsteraCumulo; } set { _PrestazioneEsteraCumulo = value; } }
            public PrestazioneEsteraCumulo PrestazioneEsteraStorico { get { return _PrestazioneEsteraStorico; } set { _PrestazioneEsteraStorico = value; } }
            public List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo> ElencoImportiEsteriCumulo { get { return _ElencoImportiEsteriCumulo; } set { _ElencoImportiEsteriCumulo = value; } }

            #endregion public properties

        }
        //ENG - MEMO 74_2023
        public class ProRata
        {
            #region private properties
            private List<StatoEsteroCumulo> _ElencoStatiEsteri;
            #endregion private properties

            #region public properties
            public List<StatoEsteroCumulo> ElencoStatiEsteri { get { return _ElencoStatiEsteri; } set { _ElencoStatiEsteri = value; } }
            #endregion public properties
        }

        public enum TipoCalcolo
        {
            NonValido,
            Contributivo,
            Retributivo,
            Misto = 21,
            MistoL214 = 26,
            RetributivoComma707 = 27,
        };

        public enum TipoAppartenenza
        {
            FS,
            AGO,
            CI
        };

        internal class DatiRetributiviExInpdai : GestioneAggiornamentoPECO.DatiRetributivi
        {
            #region Private properties
            private byte? decorrenzaExInpdai;
            private string decCodGestione;
            #endregion Private properties

            #region Public properties
            public string DecCodGestione { get { return decCodGestione; } set { decCodGestione = value; } }
            public byte? DecorrenzaExInpdai { get { return decorrenzaExInpdai; } set { decorrenzaExInpdai = value; } }
            #endregion Public properties

            public DatiRetributiviExInpdai(GestioneAggiornamentoPECO.DatiRetributivi datiRetribPeco, List<Liquidazione.BLCommon.CtrlDecorrenzaRetrExINPDAI> listDecorrenza,
                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcRetrib)
            {
                base.CodGestione = datiRetribPeco.CodGestione;
                base.CodiceTipoQuota = datiRetribPeco.CodiceTipoQuota;
                base.Decorrenza = datiRetribPeco.Decorrenza;
                base.Quota = datiRetribPeco.Quota;
                base.RMSQuotaA = datiRetribPeco.RMSQuotaA;
                base.RMSQuotaB = datiRetribPeco.RMSQuotaB;
                base.SettimaneA = datiRetribPeco.SettimaneA;
                base.SettimaneB = datiRetribPeco.SettimaneB;
                base.NSettimane707 = datiRetribPeco.NSettimane707;
                //Proprietà esclusive Retributivi ExInpdai
                string sCodiceGestione = GetCodiceGestioneExInpdai(datiRetribPeco.CodGestione, lstDecGestioneCalcRetrib);
                this.DecCodGestione = sCodiceGestione;
                this.decorrenzaExInpdai = GetDecorrenzaExInpdai(sCodiceGestione, datiRetribPeco.Quota, datiRetribPeco.CodiceTipoQuota, listDecorrenza);
            }

            public DatiRetributiviExInpdai(GestioneCalcolo.DatiCalcoloRetributivo datiRetrib, List<Liquidazione.BLCommon.CtrlDecorrenzaRetrExINPDAI> listDecorrenza,
                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcRetrib)
            {
                base.CodGestione = datiRetrib.CodiceGestione;
                base.CodiceTipoQuota = datiRetrib.CodiceTipoQuota;
                base.Decorrenza = datiRetrib.DecorrenzaOriginariaPensione;
                base.Quota = datiRetrib.QuotePrimeLiquidate;
                base.RMSQuotaA = datiRetrib.RMSQuotaA;
                base.RMSQuotaB = datiRetrib.RMSQuotaB;
                base.SettimaneA = datiRetrib.NSettimaneQuotaA;
                base.SettimaneB = datiRetrib.NSettimaneQuotaB;
                base.NSettimane707 = datiRetrib.NSettimane707;
                //Proprietà esclusive Retributivi ExInpdai
                string sCodiceGestione = GetCodiceGestioneExInpdai(datiRetrib.CodiceGestione, lstDecGestioneCalcRetrib);
                this.DecCodGestione = sCodiceGestione;
                this.decorrenzaExInpdai = GetDecorrenzaExInpdai(sCodiceGestione, datiRetrib.QuotePrimeLiquidate, datiRetrib.CodiceTipoQuota, listDecorrenza);
            }

            public DatiRetributiviExInpdai(GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiRetribVittime, List<Liquidazione.BLCommon.CtrlDecorrenzaRetrExINPDAI> listDecorrenza,
                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lstDecGestioneCalcRetrib)
            {
                base.CodGestione = datiRetribVittime.CodiceGestioneRetr;
                base.CodiceTipoQuota = datiRetribVittime.CodiceTipoQuota;
                base.Decorrenza = datiRetribVittime.DecorrenzaBeneficio;
                base.Quota = datiRetribVittime.Quota;
                if (base.Quota == 'A')
                {
                    base.RMSQuotaA = datiRetribVittime.RMS;
                    base.SettimaneA = datiRetribVittime.Settimane;
                }
                else if (base.Quota == 'B')
                {
                    base.RMSQuotaB = datiRetribVittime.RMS;
                    base.SettimaneB = datiRetribVittime.Settimane;
                }
                //Proprietà esclusive Retributivi ExInpdai
                string sCodiceGestione = GetCodiceGestioneExInpdai(datiRetribVittime.CodiceGestioneRetr, lstDecGestioneCalcRetrib);
                this.DecCodGestione = sCodiceGestione;
                this.decorrenzaExInpdai = GetDecorrenzaExInpdai(sCodiceGestione, datiRetribVittime.Quota, datiRetribVittime.CodiceTipoQuota, listDecorrenza);
            }
        }

        internal class DatiContributiviExInpdai : GestioneAggiornamentoPECO.DatiContributivi
        {
            #region Private properties
            private string decCodGestione;
            #endregion Private properties

            #region Public properties
            public string DecCodGestione { get { return decCodGestione; } set { decCodGestione = value; } }
            #endregion Public properties

            public DatiContributiviExInpdai(GestioneCalcolo.DatiCalcoloContributivo datiContrib, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> lstDecGestioneCalcRetrib)
            {
                base.CodGestione = datiContrib.CodiceGestione;
                if (datiContrib.IsQuotaL335Presente())
                {
                    base.Quota = 'C';
                    base.Settimane = datiContrib.NSettimaneLegge335;
                }
                else if (datiContrib.IsQuotaDL214Presente())
                {
                    base.Quota = 'D';
                    base.SettimaneQuotaD = datiContrib.NSettimaneQuotaDL214;
                }
                //Proprietà esclusive Retributivi ExInpdai
                string sCodiceGestione = GetCodiceGestioneExInpdai(datiContrib.CodiceGestione, lstDecGestioneCalcRetrib);
                this.DecCodGestione = sCodiceGestione;
            }

            public DatiContributiviExInpdai(GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiRetribVittime, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> lstDecGestioneCalcRetrib)
            {
                base.CodGestione = datiRetribVittime.CodiceGestioneRetr;
                base.Quota = datiRetribVittime.Quota;
                if (base.Quota == 'C')
                {
                    base.Settimane = datiRetribVittime.Settimane;
                }
                else if (base.Quota == 'D')
                {
                    base.SettimaneQuotaD = datiRetribVittime.Settimane;
                }
                //Proprietà esclusive Retributivi ExInpdai
                string sCodiceGestione = GetCodiceGestioneExInpdai(datiRetribVittime.CodiceGestioneRetr, lstDecGestioneCalcRetrib);
                this.DecCodGestione = sCodiceGestione;
            }
        }
        #endregion nested class

        /// <summary>
        /// Verifica se la domanda è una RIC SDAI con codice K nei dati contributivi per bypassare eventualmente i controlli
        /// </summary>
        public static bool BypassaControlliRicSdaiContributivoK(GestionePensione.DatiPensione datiPensione, List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo)
        {
            bool bypassaControlli = false;

            if (datiPensione != null && Utility.IsRicostituzione(datiPensione.Gruppo) && !String.IsNullOrEmpty(datiPensione.SiglaCategoria)
                   && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SDAI")
            {
                if (listaCodeGestioneCalcoloContributivo != null && listaCodeGestioneCalcoloContributivo.Count > 0
                    && lDatiContributivi != null && lDatiContributivi.Count > 0)
                {
                    GestioneDecodifica.CodeGestioneCalcoloContributivo gestioneK = listaCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == "K");
                    if (gestioneK != null)
                    {
                        if (lDatiContributivi.Exists(x => x.CodGestione == gestioneK.Id))
                            bypassaControlli = true;
                    }
                }
            }

            return bypassaControlli;
        }

        /// <summary>
        /// Verifica se la domanda è RIC/TRF di tipo VOCRED/CRED27 con codice L nei dati contributivi per bypassare i controlli
        /// </summary>
        public static bool BypassaControlliRic_VOCRED_CRED27_ContributivoL(GestionePensione.DatiPensione datiPensione, List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo)
        {
            bool bypassaControlli = false;

            if (datiPensione != null && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) &&
                Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria))
            {
                if (listaCodeGestioneCalcoloContributivo != null && listaCodeGestioneCalcoloContributivo.Count > 0
                    && lDatiContributivi != null && lDatiContributivi.Count > 0)
                {
                    GestioneDecodifica.CodeGestioneCalcoloContributivo gestioneL = listaCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == "L");
                    if (gestioneL != null)
                    {
                        if (lDatiContributivi.Exists(x => x.CodGestione == gestioneL.Id))
                            bypassaControlli = true;
                    }
                }
            }

            return bypassaControlli;
        }
        //ENG - MEMO 74_2023
        public static void GetStatiEsteriFromService(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, long idPensione, out List<StatoEsteroCumulo> listaStatiEsteri, out string messaggioVideo)
        {
            listaStatiEsteri = null;
            messaggioVideo = "";

            // get data from Trans
            List<PrestazioneEsteraCumulo> listaPrestazioniEstere = null;
            List<PrestazioneEsteraCumulo> listaDatiEsteriCumulo = null;
            ServiceReferences.TotalIvs.clsDatiCumulo risposta = null;
            bool isNuovaProcedura = false;

            try
            {
                if (!GestioneNACI.VerificaProcedura(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out isNuovaProcedura, out messaggioVideo))
                    return;

                if (isNuovaProcedura)
                    GestioneNACI.GetListaStatiIstituzione(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out listaPrestazioniEstere, out messaggioVideo);
                else
                    GestioneAllegatiConvenzioni.GetPrestazioneEstereByNumeroDomanda(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out listaPrestazioniEstere, out messaggioVideo);
                //Recupero dati dal Cumulo
                GestioneTotalIvs.GetDatiCumulIVS(numeroDomanda, out risposta, out messaggioVideo);
            }
            catch (Exception)
            {
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                return;
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                return;
            }

            if (risposta != null && risposta.objDatiEsteri != null && risposta.objDatiEsteri.Length > 0)
            {
                for (int i = 1; i < 7; i++)
                {
                    listaDatiEsteriCumulo = new List<PrestazioneEsteraCumulo>();
                    foreach (ServiceReferences.TotalIvs.datiEsteriCumulo datiEsteriCumulo in risposta.objDatiEsteri)
                    {
                        if (datiEsteriCumulo != new ServiceReferences.TotalIvs.datiEsteriCumulo())
                        {
                            PrestazioneEsteraCumulo datiEsteri = new PrestazioneEsteraCumulo();
                            if (!string.IsNullOrEmpty(datiEsteriCumulo.CodiceStato))
                                datiEsteri.CodiceStato = datiEsteriCumulo.CodiceStato;

                            if (!string.IsNullOrEmpty(datiEsteriCumulo.CodiceIstituzione))
                                datiEsteri.CodiceIstituzione = datiEsteriCumulo.CodiceIstituzione;

                            if (datiEsteriCumulo.SettimaneDiritto != 0)
                                datiEsteri.ContributiDiritto = datiEsteriCumulo.SettimaneDiritto;

                            if (datiEsteriCumulo.SettimaneMisura != 0)
                                datiEsteri.SettimaneMisura = datiEsteriCumulo.SettimaneMisura;

                            listaDatiEsteriCumulo.Add(datiEsteri);
                        }
                    }
                }
            }

            listaStatiEsteri = new List<StatoEsteroCumulo>();
            //CASO IN CUI SOLO NACI O ALLEGATICONVENZIONI RESTITUISCONO I DATI ESTERI
            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0 && (listaDatiEsteriCumulo == null || listaDatiEsteriCumulo.Count() == 0))
            {
                foreach (PrestazioneEsteraCumulo prestazioneEstera in listaPrestazioniEstere)
                {
                    StatoEsteroCumulo statoEstero = new StatoEsteroCumulo();
                    statoEstero.PrestazioneEsteraCumulo = prestazioneEstera;
                    Data.aciistit descPrestazioneEstera = null;
                    Data.DAPrestazioniEstere.GetPrestazioneEstera(statoEstero.PrestazioneEsteraCumulo.CodiceStato + statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione, out descPrestazioneEstera);
                    if (descPrestazioneEstera != null)
                    {
                        statoEstero.PrestazioneEsteraCumulo.Sigla = descPrestazioneEstera.SIGLISTI;
                        statoEstero.PrestazioneEsteraCumulo.Citta = descPrestazioneEstera.CITTAIST;
                        statoEstero.PrestazioneEsteraCumulo.NomeStato = descPrestazioneEstera.NOMESTAT;
                        statoEstero.PrestazioneEsteraCumulo.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                        statoEstero.PrestazioneEsteraCumulo.CodiceConvenzione = Utility.StringToNullableByte(descPrestazioneEstera.CODICONV);
                        statoEstero.PrestazioneEsteraCumulo.Confermato = false;
                    }
                    statoEstero.ElencoImportiEsteriCumulo = new List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo>();
                    listaStatiEsteri.Add(statoEstero);
                }
            }
            //CASO IN CUI SOLO CUMULO RESTITUISCE I DATI ESTERI
            else if (listaDatiEsteriCumulo != null && listaDatiEsteriCumulo.Count() > 0 && (listaPrestazioniEstere == null || listaPrestazioniEstere.Count() == 0))
            {
                foreach (PrestazioneEsteraCumulo prestazioneEsteraCumulo in listaDatiEsteriCumulo)
                {
                    StatoEsteroCumulo statoEstero = new StatoEsteroCumulo();
                    statoEstero.PrestazioneEsteraCumulo = prestazioneEsteraCumulo;
                    Data.aciistit descPrestazioneEstera = null;
                    Data.DAPrestazioniEstere.GetPrestazioneEstera(statoEstero.PrestazioneEsteraCumulo.CodiceStato + statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione, out descPrestazioneEstera);
                    if (descPrestazioneEstera != null)
                    {
                        statoEstero.PrestazioneEsteraCumulo.Sigla = descPrestazioneEstera.SIGLISTI;
                        statoEstero.PrestazioneEsteraCumulo.Citta = descPrestazioneEstera.CITTAIST;
                        statoEstero.PrestazioneEsteraCumulo.NomeStato = descPrestazioneEstera.NOMESTAT;
                        statoEstero.PrestazioneEsteraCumulo.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                        statoEstero.PrestazioneEsteraCumulo.CodiceConvenzione = Utility.StringToNullableByte(descPrestazioneEstera.CODICONV);
                        statoEstero.PrestazioneEsteraCumulo.Confermato = false;
                    }
                    statoEstero.ElencoImportiEsteriCumulo = new List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo>();
                    listaStatiEsteri.Add(statoEstero);
                }
            }
            //CASO IN CUI SIA CUMULO CHE NACI O ALLEGATICONVENZIONI RESTITUISCONO I DATI ESTERI
            else if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0 && listaDatiEsteriCumulo != null && listaDatiEsteriCumulo.Count() > 0)
            {
                foreach (PrestazioneEsteraCumulo prestazioneEstera in listaPrestazioniEstere)
                {
                    foreach (PrestazioneEsteraCumulo prestazioneEsteraCumulo in listaDatiEsteriCumulo)
                    {
                        if (prestazioneEstera.CodiceStato != prestazioneEsteraCumulo.CodiceStato || prestazioneEstera.CodiceIstituzione != prestazioneEsteraCumulo.CodiceIstituzione)
                            messaggioVideo = "STATO E ISTITUZIONE NON COMPATIBILI CON QUELLI DEL SERVIZIO CUMUL";
                    }

                    StatoEsteroCumulo statoEstero = new StatoEsteroCumulo();
                    statoEstero.PrestazioneEsteraCumulo = prestazioneEstera;
                    Data.aciistit descPrestazioneEstera = null;
                    Data.DAPrestazioniEstere.GetPrestazioneEstera(statoEstero.PrestazioneEsteraCumulo.CodiceStato + statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione, out descPrestazioneEstera);
                    if (descPrestazioneEstera != null)
                    {
                        statoEstero.PrestazioneEsteraCumulo.Sigla = descPrestazioneEstera.SIGLISTI;
                        statoEstero.PrestazioneEsteraCumulo.Citta = descPrestazioneEstera.CITTAIST;
                        statoEstero.PrestazioneEsteraCumulo.NomeStato = descPrestazioneEstera.NOMESTAT;
                        statoEstero.PrestazioneEsteraCumulo.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                        statoEstero.PrestazioneEsteraCumulo.CodiceConvenzione = Utility.StringToNullableByte(descPrestazioneEstera.CODICONV);
                        statoEstero.PrestazioneEsteraCumulo.Confermato = false;
                    }
                    statoEstero.ElencoImportiEsteriCumulo = new List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo>();
                    listaStatiEsteri.Add(statoEstero);
                }
            }
        }
        //ENG - MEMO 74_2023
        public static void GetStatiEsteri(GestionePensione.DatiPensione datiPensione, List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere,
            string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out List<StatoEsteroCumulo> listaStatiEsteri, out string messaggioVideo)
        {
            listaStatiEsteri = null;
            messaggioVideo = "";

            // get data from DB
            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                GetStatiEEfromDBByIdPensione(datiPensione.Id, listaPrestazioniEstere, out listaStatiEsteri);
                return;
            }

            //if (Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(2, 1) == "V")
            //{
            GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo74_2023", out ctrlMemo74_2023);

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo74_2023", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023);

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_CUMUL_Memo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_CUMUL_Memo74_2023", out ctrlAbilitaChiamata_CUMUL_Memo74_2023);
            //ENG - Memo 116/2025
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo116_2025", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025);

            if ((ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI" && ((ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI") ||
                (ctrlAbilitaChiamata_CUMUL_Memo74_2023 != null && ctrlAbilitaChiamata_CUMUL_Memo74_2023.ValoreControllo == "SI"))) ||
                ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025.ValoreControllo == "SI")
                GetAndStoreStatiEsteri(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out listaStatiEsteri, out messaggioVideo);
            //}
        }

        internal static void GetStatiEEfromDBByIdPensione(long idPensione, List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere, out List<StatoEsteroCumulo> listaStatiEsteri)
        {
            listaStatiEsteri = new List<StatoEsteroCumulo>();
            List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo> listaImportiEsteri = null;
            GestioneDatiEsteriCumulo.GetImportiEsteriCumuloByIdPensione(idPensione, out listaImportiEsteri);
            List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEEStorico = null;
            GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloStoricoByIdPensione(idPensione, out listaPrestazioniEEStorico);

            foreach (GestioneDatiEsteriCumulo.PensioneEsteraCumulo prestazioneEE in listaPrestazioniEstere)
            {
                StatoEsteroCumulo statoEstero = new StatoEsteroCumulo();
                statoEstero.PrestazioneEsteraCumulo = new PrestazioneEsteraCumulo();
                Utility.ValorizzaOggetti(prestazioneEE, statoEstero.PrestazioneEsteraCumulo);
                Data.aciistit descPrestazioneEstera = null;
                Data.DAPrestazioniEstere.GetPrestazioneEstera(statoEstero.PrestazioneEsteraCumulo.CodiceStato + statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione, out descPrestazioneEstera);
                if (descPrestazioneEstera != null)
                {
                    statoEstero.PrestazioneEsteraCumulo.Sigla = descPrestazioneEstera.SIGLISTI;
                    statoEstero.PrestazioneEsteraCumulo.Citta = descPrestazioneEstera.CITTAIST;
                    statoEstero.PrestazioneEsteraCumulo.NomeStato = descPrestazioneEstera.NOMESTAT;
                    statoEstero.PrestazioneEsteraCumulo.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                    statoEstero.PrestazioneEsteraCumulo.MatricolaIstituzione = statoEstero.PrestazioneEsteraCumulo.MatricolaEstera;
                }
                if (listaPrestazioniEEStorico != null && listaPrestazioniEEStorico.Count > 0 &&
                    listaPrestazioniEEStorico.Any(x => x.CodiceStato == statoEstero.PrestazioneEsteraCumulo.CodiceStato && x.CodiceIstituzione == statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione))
                {
                    GestioneDatiEsteriCumulo.PensioneEsteraCumulo prestazioneStoricoDB = listaPrestazioniEEStorico.FirstOrDefault(x => x.CodiceStato == statoEstero.PrestazioneEsteraCumulo.CodiceStato && x.CodiceIstituzione == statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione);
                    PrestazioneEsteraCumulo prestazioneStorico = new PrestazioneEsteraCumulo();
                    Utility.ValorizzaOggetti(prestazioneStoricoDB, prestazioneStorico);
                    statoEstero.PrestazioneEsteraStorico = prestazioneStorico;
                }
                statoEstero.ElencoImportiEsteriCumulo = listaImportiEsteri.FindAll(x => x.IdPensioneEsteraCumulo == prestazioneEE.Id);
                listaStatiEsteri.Add(statoEstero);
            }
        }

        public static void GetAndStoreStatiEsteri(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out List<StatoEsteroCumulo> listaStatiEsteri, out string errori)
        {
            listaStatiEsteri = null;
            errori = string.Empty;
            try
            {
                GetStatiEsteriFromService(datiPensione.NDomus, Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)), datiPensione.CentroOperativo.HasValue ? datiPensione.CentroOperativo.Value : (byte)0,
                    matricolaOperatore, sedeOperatore, centroOperativoOperatore, datiPensione.Id, out listaStatiEsteri, out errori);
            }
            catch (INPS.DNA.DnaApplicationException)
            {
                throw new INPS.DNA.DnaApplicationException(errori);
            }
            if (!string.IsNullOrEmpty(errori))
                return;

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo74_2023", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023);
            //ENG - Memo 116/2025
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo116_2025", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025);
            if ((ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI") ||
                (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI"))
            {
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEE = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
                {
                    foreach (StatoEsteroCumulo statoEstero in listaStatiEsteri)
                    {
                        if (String.IsNullOrEmpty(statoEstero.PrestazioneEsteraCumulo.MatricolaEstera))
                            statoEstero.PrestazioneEsteraCumulo.MatricolaEstera = statoEstero.PrestazioneEsteraCumulo.MatricolaIstituzione;
                        listaPrestazioniEE.Add(statoEstero.PrestazioneEsteraCumulo);
                    }
                    GestioneDatiEsteriCumulo.SalvaListaPrestazioniEstereCumulo(datiPensione.Id, listaPrestazioniEE);
                }
            }
        }
        //ENG - MEMO 74_2023
        public static void RecuperaDescrizioneStatiEsteri(string codStato, string codIstituzione, out string descCodStato, out string descCodIstituzione, out string descCittà, out List<StatoEsteroCumulo> listaStatiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            descCodStato = string.Empty;
            descCodIstituzione = string.Empty;
            descCittà = string.Empty;
            listaStatiEsteri = new List<StatoEsteroCumulo>();

            string codStatoIstituzione = codStato + codIstituzione;
            if (!string.IsNullOrEmpty(codStatoIstituzione))
            {
                aciistit descPrestazioneEstera = null;
                DAPrestazioniEstere.GetPrestazioneEstera(codStatoIstituzione.PadLeft(6, '0'), out descPrestazioneEstera);
                if (descPrestazioneEstera != null)
                {
                    StatoEsteroCumulo statoEstero = new StatoEsteroCumulo();
                    if (statoEstero.PrestazioneEsteraCumulo == null)
                        statoEstero.PrestazioneEsteraCumulo = new PrestazioneEsteraCumulo();

                    descCodIstituzione = descPrestazioneEstera.SIGLISTI;
                    descCodStato = descPrestazioneEstera.NOMESTAT;
                    descCittà = descPrestazioneEstera.CITTAIST;
                    statoEstero.PrestazioneEsteraCumulo.CodiceStato = codStato;
                    statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione = codIstituzione;
                    statoEstero.PrestazioneEsteraCumulo.Sigla = descPrestazioneEstera.SIGLISTI;
                    statoEstero.PrestazioneEsteraCumulo.Citta = descPrestazioneEstera.CITTAIST;
                    statoEstero.PrestazioneEsteraCumulo.NomeStato = descPrestazioneEstera.NOMESTAT;
                    statoEstero.PrestazioneEsteraCumulo.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                    statoEstero.ElencoImportiEsteriCumulo = new List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo>();
                    listaStatiEsteri.Add(statoEstero);
                }
                else
                    messaggioVideo = "STATO O/E ISTITUZIONE NON ESITENTI";
            }
            else
                messaggioVideo = "INSERIRE SIA IL CODICE STATO CHE IL CODICE ISTITUZIONE";
        }

        public static void RecuperaCodiceConvenzioneStatiEsteri(string codStato, string codIstituzione, out byte? codiceConvenzione)
        {
            codiceConvenzione = null;
            string codStatoIstituzione = codStato + codIstituzione;
            if (!string.IsNullOrEmpty(codStatoIstituzione))
            {
                aciistit descPrestazioneEstera = null;
                DAPrestazioniEstere.GetPrestazioneEstera(codStatoIstituzione.PadLeft(6, '0'), out descPrestazioneEstera);
                if (descPrestazioneEstera != null)
                {
                    StatoEsteroCumulo statoEstero = new StatoEsteroCumulo();
                    if (statoEstero.PrestazioneEsteraCumulo == null)
                        statoEstero.PrestazioneEsteraCumulo = new PrestazioneEsteraCumulo();
                    codiceConvenzione = Utility.StringToNullableByte(descPrestazioneEstera.CODICONV);
                }
            }
        }
        //ENG - MEMO 74_2023
        public static void StoreStatiEsteri(ref EntityBLCommon.ContenitoreObject contenitore, List<StatoEsteroCumulo> listaStatiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listStati = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo74_2023", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023);

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_CUMUL_Memo74_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_CUMUL_Memo74_2023", out ctrlAbilitaChiamata_CUMUL_Memo74_2023);
            //ENG - Memo 116/2025
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaChiamata_Naci_AllegatiConv_Memo116_2025", out ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025);

            byte? codiceConvenzioneTabAssicurativi = contenitore.DatiPensioniDatiGenerici.CodiceConvenzioneAgo != null ? contenitore.DatiPensioniDatiGenerici.CodiceConvenzioneAgo : (byte?)null;
            if (ControlsStatiEsteri(datiPensione, listaStatiEsteri, codiceConvenzioneTabAssicurativi, out messaggioVideo))
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
                    {
                        if (Utility.IsRicostituzioneOrRiapertura(datiPensione, contenitore.IsRiaperturaDomanda) ||
                            (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo74_2023.ValoreControllo == "SI") ||
                            (ctrlAbilitaChiamata_CUMUL_Memo74_2023 != null && ctrlAbilitaChiamata_CUMUL_Memo74_2023.ValoreControllo == "SI") ||
                            (ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025 != null && ctrlAbilitaChiamata_Naci_AllegatiConv_Memo116_2025.ValoreControllo == "SI"))
                        {
                            foreach (StatoEsteroCumulo statoEstero in listaStatiEsteri)
                            {
                                statoEstero.PrestazioneEsteraCumulo.IdPensione = datiPensione.Id; ;
                                if (String.IsNullOrEmpty(statoEstero.PrestazioneEsteraCumulo.MatricolaEstera))
                                    statoEstero.PrestazioneEsteraCumulo.MatricolaEstera = statoEstero.PrestazioneEsteraCumulo.MatricolaIstituzione;
                                GestioneDatiEsteriCumulo.SalvaPrestazioneEsteraCumulo(statoEstero.PrestazioneEsteraCumulo);
                                GestioneDatiEsteriCumulo.EliminaImportiEsteriCumuloPerPrestazione(statoEstero.PrestazioneEsteraCumulo.Id);
                                if (statoEstero.ElencoImportiEsteriCumulo != null && statoEstero.ElencoImportiEsteriCumulo.Count > 0)
                                {
                                    foreach (GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo importoEstero in statoEstero.ElencoImportiEsteriCumulo)
                                    {
                                        importoEstero.IdPensioneEsteraCumulo = statoEstero.PrestazioneEsteraCumulo.Id;
                                        GestioneDatiEsteriCumulo.SalvaImportoEsteroCumulo(importoEstero);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int index = 0;
                            foreach (StatoEsteroCumulo statoEstero in listaStatiEsteri)
                            {
                                statoEstero.PrestazioneEsteraCumulo.IdPensione = datiPensione.Id;
                                byte? codiceConvenzione = null;
                                if (index == 0)
                                    codiceConvenzione = codiceConvenzioneTabAssicurativi;
                                else
                                    RecuperaCodiceConvenzioneStatiEsteri(statoEstero.PrestazioneEsteraCumulo.CodiceStato, statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione, out codiceConvenzione);

                                statoEstero.PrestazioneEsteraCumulo.CodiceConvenzione = codiceConvenzione;
                                listStati.Add(statoEstero.PrestazioneEsteraCumulo);
                                GestioneDatiEsteriCumulo.SalvaListaPrestazioniEstereCumulo(datiPensione.Id, listStati);
                                GestioneDatiEsteriCumulo.EliminaImportiEsteriCumuloPerPrestazione(statoEstero.PrestazioneEsteraCumulo.Id);
                                if (statoEstero.ElencoImportiEsteriCumulo != null && statoEstero.ElencoImportiEsteriCumulo.Count > 0)
                                {
                                    foreach (GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo importoEstero in statoEstero.ElencoImportiEsteriCumulo)
                                    {
                                        importoEstero.IdPensioneEsteraCumulo = statoEstero.PrestazioneEsteraCumulo.Id;
                                        GestioneDatiEsteriCumulo.SalvaImportoEsteroCumulo(importoEstero);
                                    }
                                }
                                index++;
                            }
                        }
                        datiQuadroDatiContributivi.TabDatiEsteri = 0;
                        if (!listaStatiEsteri.Any(x => x.PrestazioneEsteraCumulo == null) && !listaStatiEsteri.Any(x => !x.PrestazioneEsteraCumulo.Confermato.GetValueOrDefault()))
                            datiQuadroDatiContributivi.TabDatiEsteri = 2;

                        GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                    }
                    transactionScope.Complete();
                }
            }
        }
        //ENG - MEMO 74_2023
        public static void EliminaStatiEsteri(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore)
        {
            string messaggioVideo = string.Empty;

            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDatiEsteriCumulo.EliminaAllPrestazioniEstereCumulo(datiPensione.Id);
                datiQuadroDatiContributivi.TabDatiEsteri = 0;
                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        //ENG - MEMO 74_2023
        public static void EliminaStatoEsteroSingolo(long idPrestazione, GestionePensione.DatiPensione datiPensione)
        {
            string messaggioVideo = string.Empty;

            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo> listaImportiEsteri = null;
            GestioneDatiEsteriCumulo.GetImportiEsteriCumuloByIdPensione(datiPensione.Id, out listaImportiEsteri);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (idPrestazione != 0)
                {
                    if (listaImportiEsteri != null && listaImportiEsteri.Count() > 0)
                    {
                        if (listaImportiEsteri.Exists(x => x.IdPensioneEsteraCumulo == idPrestazione))
                        {
                            GestioneDatiEsteriCumulo.EliminaImportiEsteriCumuloPerPrestazione(idPrestazione);
                            GestioneDatiEsteriCumulo.EliminaPrestazioniEE(idPrestazione);
                        }
                    }
                    else
                        GestioneDatiEsteriCumulo.EliminaPrestazioniEE(idPrestazione);
                }
                transactionScope.Complete();
            }
        }

        private static bool ControlsStatiEsteri(GestionePensione.DatiPensione datiPensione, List<StatoEsteroCumulo> listaStatiEsteri, byte? codiceConvenzioneTabAssicurativi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            {
                //ENG - Memo 116/2025
                if (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione) ||
                    Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) || Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(datiPensione)))
                {
                    if (listaStatiEsteri.Exists(x => !x.PrestazioneEsteraCumulo.SettimaneMisura.HasValue || x.PrestazioneEsteraCumulo.SettimaneMisura.Value == 0))
                    {
                        messaggioVideo = "Il campo Settimane Misura deve essere valorizzato con un valore maggiore di 0";
                        return false;
                    }
                    if (listaStatiEsteri.Exists(x => !x.PrestazioneEsteraCumulo.ContributiDiritto.HasValue || x.PrestazioneEsteraCumulo.ContributiDiritto.Value == 0))
                    {
                        messaggioVideo = "Il campo Contributi Diritto deve essere valorizzato con un valore maggiore di 0";
                        return false;
                    }
                }

                foreach (StatoEsteroCumulo stato in listaStatiEsteri)
                {
                    stato.ElencoImportiEsteriCumulo.Sort(delegate
                        (GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo c1, GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo c2)
                    { return c1.DecorrenzaPrestazione.Value.CompareTo(c2.DecorrenzaPrestazione.Value); });

                    if (stato.ElencoImportiEsteriCumulo != null && stato.ElencoImportiEsteriCumulo.Count > 0)
                    {
                        GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo appImportoEstero = null;
                        foreach (GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo importiEsteri in stato.ElencoImportiEsteriCumulo)
                        {
                            importiEsteri.IdPensioneEsteraCumulo = stato.PrestazioneEsteraCumulo.Id;

                            if (appImportoEstero != null)
                            {
                                if (appImportoEstero.CessazionePrestazione.HasValue && !Utility.DataSuccessivaA(importiEsteri.DecorrenzaPrestazione.Value, appImportoEstero.CessazionePrestazione.Value))
                                {
                                    messaggioVideo = "Decorrenza Prestazione Estera non posteriore a Cessazione precedente";
                                    return false;
                                }
                            }

                            if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithDecorrenzaOriginaria(importiEsteri.DecorrenzaPrestazione, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaCoerenzaDecorrenzaCessazione(importiEsteri.DecorrenzaPrestazione, importiEsteri.CessazionePrestazione, out messaggioVideo))
                                return false;

                            appImportoEstero = importiEsteri;
                        }
                    }
                }
            }

            return true;
        }
        //

        //ENG - MEMO 74_2023
        //ENG - Memo 116/2025
        public static void ControlsCompatibilitàCodiceConvenzioneWithStatoEstero(long numeroDomanda, byte? progStorico, StatoEsteroCumulo stato, byte? codiceConvenzioneTabAssicurativi, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzioneTabAssicurativi.HasValue)
            {
                if (!GestioneControlli.VerificaCodiceConvenzioneWithStatoEstero(decorrenzaOriginaria, stato.PrestazioneEsteraCumulo.CodiceStato, codiceConvenzioneTabAssicurativi))
                    messaggioVideo = "Codice Convenzione errato o incompatibile con Stato ";

            }
        }
    }
}
