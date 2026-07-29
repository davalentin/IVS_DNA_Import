using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.Entity.Oneri;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaOneri
    {

        #region Oneri Benefici Particolari

        public static void EliminaDatiOneriBeneficiPaticolari(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
            GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out datiQuadroOneri);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);

                datiQuadroOneri.TabOneri = 0;
                GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                transactionScope.Complete();
            }
        }

        public static void StoreDatiOneriBeneficiParticolari(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, Entity.Oneri.DatiOneriBenefParticolari datiOneriBenefParticolari)
        {
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
            GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = null;
            GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out datiQuadroOneri);
            bool aggiornaQuadroRedditi = ControlsAggiornamentoQuadroRedditi(datiPensione, datiQuadroOneri, datiOneriBenefParticolari.ListaDatiOneri, out datiQuadroRedditi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                StoreDatiOneri(datiOneriBenefParticolari.ListaDatiOneri, datiPensione);
                if (datiOneriBenefParticolari.ListaDatiOneri != null && datiOneriBenefParticolari.ListaDatiOneri.Count > 0 ||
                    datiOneriBenefParticolari.ListaDatiBeneficiParticolari != null && datiOneriBenefParticolari.ListaDatiBeneficiParticolari.Count > 0
                    || (Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaAutomatica(datiPensione)))
                    datiQuadroOneri.TabOneri = 2;
                else if (!(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaSO(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(2, 1) == "O"))
                    datiQuadroOneri.TabOneri = 0;
                GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);

                if (!Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda))
                {
                    if (aggiornaQuadroRedditi)
                    {
                        switch (datiQuadroRedditi.Tipo.Value)
                        {
                            case 1:
                                datiQuadroRedditi.Tipo = 2;
                                datiQuadroRedditi.TabRedditi = 0;
                                break;
                            case 2:
                                datiQuadroRedditi.TabRedditi = 0;
                                break;
                            default:
                                break;
                        }
                        GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                    }
                }
                transactionScope.Complete();
            }
        }

        private static bool ControlsAggiornamentoQuadroRedditi(GestionePensione.DatiPensione datiPensione, GestioneQuadri.DatiQuadroOneri datiQuadroOneri, List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> listaDatiOneri, out GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi)
        {
            datiQuadroRedditi = null;
            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out datiQuadroRedditi);
            //in caso di redditi già acquisiti
            if (Utility.IsDomandaAPEPrecoci(datiPensione) && datiQuadroRedditi != null && datiQuadroRedditi.TabRedditi.HasValue && datiQuadroRedditi.TabRedditi.Value == 2)
            {
                List<GestioneOneri.DatiOneri> listaDatiOneriDB = null;
                GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaDatiOneriDB);
                if (listaDatiOneriDB != null && listaDatiOneriDB.Count > 0 && listaDatiOneri != null && listaDatiOneri.Count > 0)
                {
                    foreach (var onere in listaDatiOneri)
                    {
                        GestioneOneri.DatiOneri onereDB = listaDatiOneriDB.FirstOrDefault(x => x.IdCodeGruppo == onere.IdCodeGruppo && x.IdCodeSottoGruppo == onere.IdCodeSottoGruppo);
                        if (onereDB == null || !onereDB.ScadenzaBeneficio.Equals(onere.ScadenzaBeneficio))
                            return true;
                    }
                }
            }
            return false;
        }
        #region Oneri

        public static void GetDatiOneri(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, out List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> lDatiOneri,
            out List<Entity.CodiciOneri.GruppoOneri> listaGruppoOneri, out List<Entity.CodiciOneri.SottoGruppoOneri> listaSottoGruppoOneri, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare)
        {
            lDatiOneri = null;
            lDatiOneri = new List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri>();

            listaGruppoOneri = null;
            listaSottoGruppoOneri = null;
            List<GestioneOneri.DatiOneri> lDatiOneriCommon = null;
            GetListaGruppoOneri(out listaGruppoOneri);
            GetListaSottoGruppoOneri(datiPensione, out listaSottoGruppoOneri);
            GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out lDatiOneriCommon);

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out ctrl);

            //ENG - Memo 121_2023
            GestioneControlliDinamici.ControlloDinamico ctrlMemo121_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("Abilitazione_Memo_121_2023", out ctrlMemo121_2023);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (lDatiOneriCommon != null && lDatiOneriCommon.Count > 0)
            {
                foreach (GestioneOneri.DatiOneri onere in lDatiOneriCommon)
                {
                    Entity.Oneri.DatiOneriBenefParticolari.DatiOneri DatiOnere = new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri();
                    Utility.ValorizzaOggetti(onere, DatiOnere);
                    if (!Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) && !Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) &&
                        !listaSottoGruppoOneri.Exists(x => (x.Code == "0901" || x.Code == "0903" || x.Code == "4404" || x.Code == "4405" || x.Code == "4406") && x.Id == onere.IdCodeSottoGruppo) && datiPensione != null)
                        DatiOnere.Decorrenza = datiPensione.DecorrenzaOriginaria;

                    lDatiOneri.Add(DatiOnere);
                }
            }
            // La seguente gestione non è più richiesta per le domande in salvaguardia perchè i dati Oneri dovranno sempre arrivare da Felpe anche per le manuali
            else if (!Utility.IsDomandaSalvaguardiaAutomatica(datiPensione))
            {
                long codGruppo = 0;
                long? codSottoGruppo = null;
                if (listaGruppoOneri != null && listaGruppoOneri.Count > 0)
                {
                    Entity.CodiciOneri.GruppoOneri gruppoOnere = null;
                    Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOneri = null;
                    //        if (Utility.IsDomandaUsuranti(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "3600");
                    //        else if (Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "3700");
                    //        else if (Utility.IsDomandaSalvaguardia228(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "3800");
                    //        else if (Utility.IsDomandaSalvaguardia124(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "3900");
                    //        else if (Utility.IsDomandaEsuberiPA(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4000");
                    //        else if (Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4100");
                    //        else if (Utility.IsDomandaSalvaguardia147(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4200");
                    //        else if (Utility.IsDomandaSalvaguardia147_2014(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4300");
                    //        else 
                    GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
                    GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);
                    if (Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo))
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4400");
                    //        else if (Utility.IsDomandaSalvaguardia135(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4500");
                    //        else if (Utility.IsDomandaSalvaguardia208_2015(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4600");
                    //        else if (Utility.IsDomandaSalvaguardia232_2016(datiPensione))
                    //            gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4800");
                    else if (Utility.IsDomandaQuota100(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "5300");
                        if (datiPensione.LavoratorePubblico.HasValue)
                        {
                            if (datiPensione.LavoratorePubblico.Value)
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5302");
                            else
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5301");
                        }
                    }
                    else if (Utility.IsDomandaQuota102(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "5800");
                        if (datiPensione.LavoratorePubblico.HasValue)
                        {
                            if (datiPensione.LavoratorePubblico.Value)
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5802");
                            else
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5801");
                        }
                    }
                    else if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "4900");
                        if (!string.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta))
                        {
                            switch (datiPensione.CodiceTipoRichiesta)
                            {
                                case "YB":
                                    sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "4901");
                                    break;
                                case "YD":
                                    sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "4902");
                                    break;
                                case "YF":
                                    sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "4903");
                                    break;
                                case "YH":
                                    sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "4904");
                                    break;
                            }
                        }
                    }
                    else if (Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) ||
                        Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "5200");
                        switch (datiPensione.CodiceTipoRichiesta)
                        {
                            case "XA":
                            case "XB":
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5201" || x.Code == "5221");
                                break;
                            case "XC":
                            case "XD":
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5202" || x.Code == "52222");
                                break;
                        }
                    }
                    else if (Utility.IsDomandaInabilitaAmianto(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "5100");
                        sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5101");
                    }
                    else if (Utility.IsDomandaSperimentaleDonna_DL_4_2019(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "5500");
                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                            sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5502");
                        else
                            sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5501");
                    }
                    else if (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) ||
                             Utility.IsDomandaAnticipataConOpzionePL(datiPensione) || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false)) ||
                             Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "5400");
                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                            sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5402");
                        else
                            sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5401");
                    }
                    else if (Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "2000");
                        sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "2011");
                    }
                    else if (Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "6000");
                        if (datiPensione.LavoratorePubblico.HasValue)
                        {
                            if (datiPensione.LavoratorePubblico.Value)
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "6002");
                            else
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "6001");
                        }
                    }
                    else if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                             Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "5900");
                        sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "5901");
                    }
                    //ENG - Memo 123/2024
                    else if ((!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                             (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) ||
                             (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))) ||
                             Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                    {
                        gruppoOnere = listaGruppoOneri.Find(x => x.Code == "6100");
                        if (datiPensione.LavoratorePubblico.HasValue)
                        {
                            if (datiPensione.LavoratorePubblico.Value)
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "6102");
                            else
                                sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Code == "6101");
                        }
                    }

                    if (gruppoOnere != null)
                    {
                        codGruppo = gruppoOnere.Id;
                        codSottoGruppo = sottoGruppoOneri != null ? sottoGruppoOneri.Id : (long?)null;
                        lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, datiPensione.DecorrenzaOriginaria,
                            (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                    }
                }
            }

            if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione))
            {
                GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                List<GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179> listaAccordiPerTipo0179 = null;
                GestioneAnagraficaAccordiPerTipo0179.GetDecAnagraficaAccordi(out listaAccordiPerTipo0179);

                List<GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179> listaAziendePerTipo0179 = null;
                GestioneAnagraficaAziendePerTipo0179.GetDecAnagraficaAziende(out listaAziendePerTipo0179);

                if (listaAziendePerTipo0179 != null && listaAziendePerTipo0179.Count > 0 && listaAccordiPerTipo0179 != null && listaAccordiPerTipo0179.Count > 0)
                {
                    GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179 accordoPerTipo0179 = listaAccordiPerTipo0179.FirstOrDefault(x => x.Codice == datiIstruttoria.CodiceAziendaEditoriaPerTipo0179);
                    if (accordoPerTipo0179 != null)
                    {
                        GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179 aziendaPerTipo0179 = listaAziendePerTipo0179.FirstOrDefault(x => x.Id == accordoPerTipo0179.DenominazioneAzienda);
                        if (aziendaPerTipo0179 != null)
                        {
                            long codGruppo = 0;
                            long codSottoGruppo = 0;

                            Entity.CodiciOneri.GruppoOneri gruppoOnere = listaGruppoOneri != null ? listaGruppoOneri.Find(x => x.Code == "0900") : null;
                            Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOnere = null;
                            if (gruppoOnere != null)
                                codGruppo = gruppoOnere.Id;

                            if (!string.IsNullOrEmpty(aziendaPerTipo0179.SottogruppoPrimoOnere))
                            {
                                if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                                {
                                    sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == aziendaPerTipo0179.SottogruppoPrimoOnere);

                                    if (sottoGruppoOnere != null)
                                        codSottoGruppo = sottoGruppoOnere.Id;

                                    if (!lDatiOneri.Exists(x => x.IdCodeSottoGruppo == codSottoGruppo))
                                        lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, datiPensione.DecorrenzaOriginaria,
                                            (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                                }
                            }

                            if (!string.IsNullOrEmpty(aziendaPerTipo0179.SottogruppoSecondoOnere))
                            {
                                if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                                {
                                    sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == aziendaPerTipo0179.SottogruppoSecondoOnere);

                                    if (sottoGruppoOnere != null)
                                        codSottoGruppo = sottoGruppoOnere.Id;

                                    if (!lDatiOneri.Exists(x => x.IdCodeSottoGruppo == codSottoGruppo))
                                        lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, null,
                                            (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                                }
                            }
                        }
                    }
                }
            }
            else if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione))
            {
                GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                List<GestioneAnagraficaAccordiPerTipo0171.DecodAnagraficaAccordiPerTipo0171> listaAccordiPerTipo0171 = null;
                GestioneAnagraficaAccordiPerTipo0171.GetDecAnagraficaAccordi(out listaAccordiPerTipo0171);

                List<GestioneAnagraficaAziendePerTipo0171.DecodAnagraficaAziendePerTipo0171> listaAziendePerTipo0171 = null;
                GestioneAnagraficaAziendePerTipo0171.GetDecAnagraficaAziende(out listaAziendePerTipo0171);

                if (listaAziendePerTipo0171 != null && listaAziendePerTipo0171.Count > 0 && listaAccordiPerTipo0171 != null && listaAccordiPerTipo0171.Count > 0)
                {
                    GestioneAnagraficaAccordiPerTipo0171.DecodAnagraficaAccordiPerTipo0171 accordoPerTipo0171 = listaAccordiPerTipo0171.FirstOrDefault(x => x.Codice == datiIstruttoria.CodiceAziendaEditoriaPerTipo0171);
                    if (accordoPerTipo0171 != null)
                    {
                        GestioneAnagraficaAziendePerTipo0171.DecodAnagraficaAziendePerTipo0171 aziendaPerTipo0171 = listaAziendePerTipo0171.FirstOrDefault(x => x.Id == accordoPerTipo0171.DenominazioneAzienda);
                        if (aziendaPerTipo0171 != null)
                        {
                            long codGruppo = 0;
                            long codSottoGruppo = 0;

                            Entity.CodiciOneri.GruppoOneri gruppoOnere = listaGruppoOneri != null ? listaGruppoOneri.Find(x => x.Code == "0900") : null;
                            Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOnere = null;
                            if (gruppoOnere != null)
                                codGruppo = gruppoOnere.Id;

                            if (!string.IsNullOrEmpty(aziendaPerTipo0171.SottogruppoPrimoOnere))
                            {
                                if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                                {
                                    sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == aziendaPerTipo0171.SottogruppoPrimoOnere);

                                    if (sottoGruppoOnere != null)
                                        codSottoGruppo = sottoGruppoOnere.Id;

                                    if (!lDatiOneri.Exists(x => x.IdCodeSottoGruppo == codSottoGruppo))
                                        lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, datiPensione.DecorrenzaOriginaria,
                                            (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                                }
                            }

                            if (!string.IsNullOrEmpty(aziendaPerTipo0171.SottogruppoSecondoOnere))
                            {
                                if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                                {
                                    sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == aziendaPerTipo0171.SottogruppoSecondoOnere);

                                    if (sottoGruppoOnere != null)
                                        codSottoGruppo = sottoGruppoOnere.Id;

                                    if (!lDatiOneri.Exists(x => x.IdCodeSottoGruppo == codSottoGruppo))
                                        lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, null,
                                            (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                                }
                            }
                        }
                    }
                }
            }
            else if (Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione))
            {
                GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                string codeTypeOneriEditoria = Utility.GetTypeOneriEditoria(datiPensione, datiIstruttoria, isRiaperturaDomanda);

                long codGruppo = 0;
                long codSottoGruppo = 0;

                Entity.CodiciOneri.GruppoOneri gruppoOnere = listaGruppoOneri != null ? listaGruppoOneri.Find(x => x.Code == "0900") : null;
                if (gruppoOnere != null)
                    codGruppo = gruppoOnere.Id;

                Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOnere = null;

                if (lDatiOneriCommon != null && lDatiOneriCommon.Count > 0)
                {
                    bool added903 = false;
                    foreach (GestioneOneri.DatiOneri onere in lDatiOneriCommon)
                    {
                        //solo oneri di Tipo Editoria
                        if (onere.IdCodeGruppo == codGruppo)
                        {
                            Entity.Oneri.DatiOneriBenefParticolari.DatiOneri DatiOnere = new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri();
                            Utility.ValorizzaOggetti(onere, DatiOnere);
                            if (!listaSottoGruppoOneri.Exists(x => (x.Code == "0901" || x.Code == "0903") && x.Id == onere.IdCodeSottoGruppo) && datiPensione != null)
                                DatiOnere.Decorrenza = datiPensione.DecorrenzaOriginaria;

                            sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == "0903");

                            Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOnereOld = null;
                            long codSottoGruppoOld = 0;
                            //Per le RIC verifichiamo anche il sottogruppo '0901' perchè le vecchie PL possono non avere sottogruppo '0903'
                            if (Utility.IsRicostituzione(datiPensione.Gruppo))
                            {
                                sottoGruppoOnereOld = listaSottoGruppoOneri.Find(x => x.Code == "0901");

                                if (sottoGruppoOnereOld != null)
                                    codSottoGruppoOld = sottoGruppoOnereOld.Id;
                            }

                            if (sottoGruppoOnere != null)
                                codSottoGruppo = sottoGruppoOnere.Id;

                            if (DatiOnere.IdCodeGruppo == codGruppo && (DatiOnere.IdCodeSottoGruppo == codSottoGruppo || (sottoGruppoOnereOld != null && DatiOnere.IdCodeSottoGruppo == codSottoGruppoOld)))
                            {
                                //ok è già aggiunto a DB
                            }
                            else
                            {
                                //non c'è corrispondenza, si deve eliminare
                                lDatiOneri.RemoveAll(x => x.Id == DatiOnere.Id);

                                //e poi riaggiungere se non già aggiunto
                                if (!added903)
                                {
                                    lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, datiPensione.DecorrenzaOriginaria,
                                    (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));

                                    added903 = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (listaGruppoOneri != null && listaGruppoOneri.Count > 0)
                    {
                        if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                        {
                            if (!Utility.IsRicostituzione(datiPensione.Gruppo))
                                sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == "0903");
                            else
                                sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == "0901");

                            if (sottoGruppoOnere != null)
                                codSottoGruppo = sottoGruppoOnere.Id;

                            lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, datiPensione.DecorrenzaOriginaria,
                                (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                        }
                    }
                }
            }
            if (Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione))
            {
                GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB> listaAccordiLetteraB = null;
                GestioneAnagraficaAccordiLetteraB.GetDecAnagraficaAccordi(out listaAccordiLetteraB);

                List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB> listaAziendeLetteraB = null;
                GestioneAnagraficaAziendeLetteraB.GetDecAnagraficaAziende(out listaAziendeLetteraB);

                if (listaAziendeLetteraB != null && listaAziendeLetteraB.Count > 0 && listaAccordiLetteraB != null && listaAccordiLetteraB.Count > 0)
                {
                    GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB accordoLetteraB = listaAccordiLetteraB.FirstOrDefault(x => x.Codice == datiIstruttoria.CodiceAziendaEditoriaLetteraB);
                    if (accordoLetteraB != null)
                    {
                        GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB aziendaLetteraB = listaAziendeLetteraB.FirstOrDefault(x => x.Id == accordoLetteraB.DenominazioneAzienda);
                        if (aziendaLetteraB != null)
                        {
                            long codGruppo = 0;
                            long codSottoGruppo = 0;

                            Entity.CodiciOneri.GruppoOneri gruppoOnere = listaGruppoOneri != null ? listaGruppoOneri.Find(x => x.Code == "0900") : null;
                            Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOnere = null;
                            if (gruppoOnere != null)
                                codGruppo = gruppoOnere.Id;

                            if (!string.IsNullOrEmpty(aziendaLetteraB.SottogruppoPrimoOnere))
                            {
                                if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                                {
                                    sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == aziendaLetteraB.SottogruppoPrimoOnere);

                                    if (sottoGruppoOnere != null)
                                        codSottoGruppo = sottoGruppoOnere.Id;

                                    if (!lDatiOneri.Exists(x => x.IdCodeSottoGruppo == codSottoGruppo))
                                        lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, datiPensione.DecorrenzaOriginaria,
                                            (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                                }
                            }

                            if (!string.IsNullOrEmpty(aziendaLetteraB.SottogruppoSecondoOnere))
                            {
                                if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                                {
                                    sottoGruppoOnere = listaSottoGruppoOneri.Find(x => x.Code == aziendaLetteraB.SottogruppoSecondoOnere);

                                    if (sottoGruppoOnere != null)
                                        codSottoGruppo = sottoGruppoOnere.Id;

                                    if (!lDatiOneri.Exists(x => x.IdCodeSottoGruppo == codSottoGruppo))
                                        lDatiOneri.Add(new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri((long?)null, datiPensione.Id, null,
                                            (DateTime?)null, (DateTime?)null, codGruppo, codSottoGruppo, (short?)null, (decimal?)null, false));
                                }
                            }
                        }
                    }
                }
            }

            if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) && Utility.IsRicostituzione(datiPensione.Gruppo))
            {
                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenericiAgoCi);

                if (lDatiOneri != null && lDatiOneri.Count > 0 && lDatiOneri.Last().Scadenza != datiGenericiAgoCi.ScadenzaAssegno.Value)
                    lDatiOneri.Last().Scadenza = datiGenericiAgoCi.ScadenzaAssegno.Value;
            }

            //ENG - Memo 121_2023  
            //ENG - Memo 123/2024
            if ((ctrlMemo121_2023 != null && !String.IsNullOrEmpty(ctrlMemo121_2023.ValoreControllo) && ctrlMemo121_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI"
                && (Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione))
                && (!Utility.IsDomandaAutomatica(datiPensione) || Utility.IsDomandaENPALS(datiPensione.Gestione)) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)) ||
                ((Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))
                && (!Utility.IsDomandaAutomatica(datiPensione) || Utility.IsDomandaENPALS(datiPensione.Gestione)) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)))
            {
                DateTime? cessazioneIncumulabilita = Utility.CalcolaCessazioneIncumulabilita(datiPensione, datiAnagraficiTitolare, datiPensione.DataPerfezionamentoRequisiti);

                if (cessazioneIncumulabilita.HasValue)
                {
                    if (lDatiOneri != null && lDatiOneri.Count > 0)
                    {
                        foreach (DatiOneriBenefParticolari.DatiOneri onere in lDatiOneri)
                        {
                            onere.ScadenzaBeneficio = cessazioneIncumulabilita.Value;
                        }
                    }
                }
            }
        }

        public static void GetDatiOneriStorico(long idPensione, out List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> lDatiOneriStorico)
        {
            lDatiOneriStorico = null;

            List<GestioneOneri.DatiOneri> listaOneriCommonStorico = null;
            GestioneOneri.GetOneriStoricoByIdPensione(idPensione, out listaOneriCommonStorico);

            if (listaOneriCommonStorico != null && listaOneriCommonStorico.Count > 0)
            {
                lDatiOneriStorico = new List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri>();
                foreach (GestioneOneri.DatiOneri onere in listaOneriCommonStorico)
                {
                    Entity.Oneri.DatiOneriBenefParticolari.DatiOneri DatiOnere = new Entity.Oneri.DatiOneriBenefParticolari.DatiOneri();
                    Utility.ValorizzaOggetti(onere, DatiOnere);
                    lDatiOneriStorico.Add(DatiOnere);
                }
            }
        }

        public static bool ControlsDatiOneri(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> lDatiOneri, char? derogaTraduzioneSuGP, bool isRiaperturaDomanda, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, Utility.TipoAppartenenza? tipoAppartenenza,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneControlliDinamici.ControlloDinamico ctrlEliminazioneScartoOneri0031_0105_0112 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("EliminazioneScartoOneri0031_0105_0112", out ctrlEliminazioneScartoOneri0031_0105_0112);

            if (lDatiOneri != null && lDatiOneri.Count > 0)
            {
                List<GestioneOneri.DatiOneri> elencoOneri = new List<GestioneOneri.DatiOneri>();

                List<GestioneDecodifica.GruppoOneri> elencoGruppoOneri = null;
                GestioneDecodifica.GetGruppoOneri(out elencoGruppoOneri);

                List<GestioneDecodifica.SottoGruppoOneri> elencoSottoGruppoOneri = null;
                GestioneDecodifica.GetSottoGruppoOneri(out elencoSottoGruppoOneri);
                DatiOneriBenefParticolari.DatiOneri onerePrecedente = null;

                foreach (Entity.Oneri.DatiOneriBenefParticolari.DatiOneri oN in lDatiOneri)
                {
                    if (!oN.IdCodeGruppo.HasValue)
                    {
                        messaggioVideo = "Codice Gruppo obbligatorio";
                        return false;
                    }
                    if (!oN.IdCodeSottoGruppo.HasValue)
                    {
                        messaggioVideo = "Codice Sottogruppo obbligatorio";
                        return false;
                    }
                    //Vittime terrorismo
                    //ENG - Per le RIC con GPT 0031/0105/0112 è stato richiesto di inserire più oneri. Al momento non ci sono vincoli particolari per il gruppo e sottogruppo
                    if (!(tipoAppartenenza == Utility.TipoAppartenenza.AGO && ctrlEliminazioneScartoOneri0031_0105_0112 != null && !String.IsNullOrEmpty(ctrlEliminazioneScartoOneri0031_0105_0112.ValoreControllo)
                        && ctrlEliminazioneScartoOneri0031_0105_0112.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)))
                    {
                        if (oN.IdCodeGruppo == 12 && lDatiOneri.Count() == 2 && oN == lDatiOneri.ElementAt(1))
                        {
                            string sottoGruppoPrimoOnere = elencoSottoGruppoOneri.Where(x => x.Id == lDatiOneri.ElementAt(0).IdCodeSottoGruppo).Select(x => x.Code).First();
                            switch (oN.IdCodeSottoGruppo)
                            {
                                case 10088:
                                    if (lDatiOneri.ElementAt(0).IdCodeSottoGruppo != 53)
                                        messaggioVideo = "Il Codice Sottogruppo 4404 non è compatibile con il Codice Sottogruppo " + sottoGruppoPrimoOnere;
                                    break;
                                case 10089:
                                    if (lDatiOneri.ElementAt(0).IdCodeSottoGruppo != 54)
                                        messaggioVideo = "Il Codice Sottogruppo 4405 non è compatibile con il Codice Sottogruppo " + sottoGruppoPrimoOnere;
                                    break;
                                case 10090:
                                    if (lDatiOneri.ElementAt(0).IdCodeSottoGruppo != 55)
                                        messaggioVideo = "Il Codice Sottogruppo 4406 non è compatibile con il Codice Sottogruppo " + sottoGruppoPrimoOnere;
                                    break;
                                default:
                                    break;
                            }
                            if (!string.IsNullOrEmpty(messaggioVideo))
                                return false;
                        }
                    }

                    GestioneDecodifica.GruppoOneri GruppoOnereDec = elencoGruppoOneri.Find(x => x.Id == oN.IdCodeGruppo);
                    GestioneDecodifica.SottoGruppoOneri SottoGruppoOnereDec = elencoSottoGruppoOneri.Find(x => x.Id == oN.IdCodeSottoGruppo);

                    if (tipoAppartenenza == Utility.TipoAppartenenza.AGO && ctrlEliminazioneScartoOneri0031_0105_0112 != null && !String.IsNullOrEmpty(ctrlEliminazioneScartoOneri0031_0105_0112.ValoreControllo)
                        && ctrlEliminazioneScartoOneri0031_0105_0112.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                    {
                        if (lDatiOneri.Count(x => x.IdCodeGruppo.GetValueOrDefault() == oN.IdCodeGruppo.GetValueOrDefault() && x.IdCodeSottoGruppo.GetValueOrDefault() == oN.IdCodeSottoGruppo.GetValueOrDefault()) > 1)
                        {
                            messaggioVideo = "Non è permesso inserire più Oneri aventi stesso Codice Gruppo Onere e Codice Sottogruppo Onere";
                            return false;
                        }

                    }

                    if (Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) && SottoGruppoOnereDec.Code == "0908")
                    {
                        oN.Decorrenza = datiPensione.DecorrenzaOriginaria.GetValueOrDefault();
                        oN.Scadenza = onerePrecedente.Scadenza;
                    }

                    if (GruppoOnereDec.Code != "2000" || SottoGruppoOnereDec.Code != "2010")
                        if (!GestioneCrossControls.ALL_VerificaDecorrenzeOneri(datiPensione, GruppoOnereDec.Code, oN.Decorrenza, oN.Scadenza, oN.ScadenzaBeneficio, derogaTraduzioneSuGP, isRiaperturaDomanda, SottoGruppoOnereDec.Code, onerePrecedente != null ? onerePrecedente.Scadenza : null, datiAnagraficiTitolare, tipoAppartenenza, ctrlEliminazioneScartoOneri0031_0105_0112, out messaggioVideo))
                            return false;

                    elencoOneri.Add(new GestioneOneri.DatiOneri(oN.Id, oN.IdPensione, oN.Decorrenza, oN.Scadenza, oN.ScadenzaBeneficio, oN.IdCodeGruppo, oN.IdCodeSottoGruppo, oN.Settimane, oN.Onere, oN.IsStorico));
                    onerePrecedente = oN;
                }

                //FG - Controllo che sia presente il codiceIstruttoria e che nel caso sia una azienda con oneri doppi:
                //la data decorrenza onere della seconda riga sia maggiore della scadenza onere della prima riga
                if (!GestioneCrossControls.AGO_ControlsOneriPrepensionamentoEditoria(datiPensione, datiIstruttoria, elencoOneri, isRiaperturaDomanda, out messaggioVideo))
                    return false;

            }
            return true;
        }

        private static void StoreDatiOneri(List<Entity.Oneri.DatiOneriBenefParticolari.DatiOneri> lDatiOneri, GestionePensione.DatiPensione datiPensione)
        {
            if (lDatiOneri != null && lDatiOneri.Count > 0)
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                    foreach (Entity.Oneri.DatiOneriBenefParticolari.DatiOneri onere in lDatiOneri)
                    {
                        if (!Entity.Oneri.DatiOneriBenefParticolari.DatiOneri.IsOneriNull(onere))
                        {
                            GestioneOneri.DatiOneri onereCommon = new GestioneOneri.DatiOneri();
                            Utility.ValorizzaOggetti(onere, onereCommon);
                            onereCommon.IdPensione = datiPensione.Id;
                            GestioneOneri.SalvaOneriOnere(onereCommon);
                        }
                    }
                    transactionScope.Complete();
                }
            }
        }

        #endregion Oneri

        #region Dati Benefici Particolari

        public static void GetDatiBeneficiParticolariByIdPensione(long idPensione, GestionePensione.DatiPensione datiPensione, out List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari> lDatiBeneficiParticolari)
        {
            lDatiBeneficiParticolari = null;
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listBeneficiParticolariCommon = null;

            GestioneBeneficiParticolari.GetBeneficiParticolariByIdPensione(idPensione, datiPensione, out listBeneficiParticolariCommon);
            if (listBeneficiParticolariCommon != null && listBeneficiParticolariCommon.Count > 0)
            {
                lDatiBeneficiParticolari = new List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari>();
                foreach (GestioneBeneficiParticolari.DatiBeneficiParticolari beneficiParticolari in listBeneficiParticolariCommon)
                {
                    Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari datiBeneficiParticolari = new Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari();
                    Utility.ValorizzaOggetti(beneficiParticolari, datiBeneficiParticolari);
                    lDatiBeneficiParticolari.Add(datiBeneficiParticolari);
                }
            }
        }

        public static void GetDatiBeneficiParticolariStoricoByIdPensione(long idPensione, out List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari> lDatiBeneficiParticolari)
        {
            lDatiBeneficiParticolari = null;
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listBeneficiParticolariCommon = null;

            GestioneBeneficiParticolari.GetBeneficiParticolariStoricoByIdPensione(idPensione, out listBeneficiParticolariCommon);
            if (listBeneficiParticolariCommon != null && listBeneficiParticolariCommon.Count > 0)
            {
                lDatiBeneficiParticolari = new List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari>();
                foreach (GestioneBeneficiParticolari.DatiBeneficiParticolari beneficiParticolari in listBeneficiParticolariCommon)
                {
                    Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari datiBeneficiParticolari = new Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari();
                    Utility.ValorizzaOggetti(beneficiParticolari, datiBeneficiParticolari);
                    lDatiBeneficiParticolari.Add(datiBeneficiParticolari);
                }
            }
        }

        public static bool ControlsDatiBeneficiParticolari(List<Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari> ldatiBeneficiParticolari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            return true;
        }

        public static void ValorizzaDatiBeneficiParticolariForPrepensionamento(GestionePensione.DatiPensione datiPensione, ref List<INPS.Pensioni.Liquidazione.Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari> datiBeneficiParticolari)
        {
            int codiceLegge = 0;
            string tipoSettimaneBeneficio = string.Empty;

            if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione, out codiceLegge, out tipoSettimaneBeneficio))
            {
                if (!string.IsNullOrEmpty(tipoSettimaneBeneficio))
                {
                    if (datiBeneficiParticolari == null)
                    {
                        datiBeneficiParticolari = new List<INPS.Pensioni.Liquidazione.Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari>();

                        INPS.Pensioni.Liquidazione.Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari beneficioParticolare = new INPS.Pensioni.Liquidazione.Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari();
                        beneficioParticolare.CodiceBenefici = tipoSettimaneBeneficio;

                        datiBeneficiParticolari.Add(beneficioParticolare);
                    }
                }
            }

            if (Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale))
            {
                if (datiBeneficiParticolari == null)
                {
                    datiBeneficiParticolari = new List<INPS.Pensioni.Liquidazione.Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari>();

                    INPS.Pensioni.Liquidazione.Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari beneficioParticolare = new INPS.Pensioni.Liquidazione.Entity.Oneri.DatiOneriBenefParticolari.DatiBeneficiParticolari();
                    beneficioParticolare.CodiceBenefici = "04";

                    datiBeneficiParticolari.Add(beneficioParticolare);
                }
            }
        }

        #endregion Dati Benefici Particolari

        #endregion Oneri Benefici Particolari

        #region Dati Prepensionamento

        public static void StoreDatiPrepensionamento(GestionePensione.DatiPensione datiPensione, Entity.Oneri.DatiPrepensionamento datiPrepensionamento)
        {
            if (datiPrepensionamento != null)
            {
                GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
                GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out datiQuadroOneri);

                GestionePrepensionamento.DatiPrepensionamento prepensionamento = new GestionePrepensionamento.DatiPrepensionamento();
                Utility.ValorizzaOggetti(datiPrepensionamento, prepensionamento);
                prepensionamento.IdPensione = datiPensione.Id;

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestionePrepensionamento.SalvaDatiPrepensionamento(prepensionamento);


                    datiQuadroOneri.TabPrepensionamento = 2;
                    GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaDatiPrepensionamento(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
            GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out datiQuadroOneri);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePrepensionamento.EliminaDatiPrepensionamentoByIdPensione(datiPensione.Id);

                datiQuadroOneri.TabPrepensionamento = 0;
                GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);

                transactionScope.Complete();
            }
        }

        public static bool ControlDatiPrepensionamento(GestionePensione.DatiPensione datiPensione, Entity.Oneri.DatiPrepensionamento datiPrepensionamento, bool IsCancelOperation, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiPrepensionamento == null)
            {
                messaggioVideo = "Dati Prepensionamento mancanti";
                return false;
            }

            if (!datiPrepensionamento.CodiceLegge.HasValue)
            {
                messaggioVideo = "Il 'Codice Legge' è obbligatorio";
                return false;
            }

            if (!datiPrepensionamento.SettimaneUtiliDiritto.HasValue)
            {
                messaggioVideo = "Le 'Settimane Utili Diritto' sono obbligatorie";
                return false;
            }

            if (!datiPrepensionamento.SettimaneUtiliMisura.HasValue)
            {
                messaggioVideo = "Le 'Settimane Utili Misura' sono obbligatorie";
                return false;
            }

            return true;
        }

        public static void ValorizzaDatiPrepensionamentoByDatiPensione(GestionePensione.DatiPensione datiPensione, GestionePrepensionamento.DatiPrepensionamento prepensionamento, out Entity.Oneri.DatiPrepensionamento datiPrepensionamento)
        {
            datiPrepensionamento = null;
            if (prepensionamento != null)
            {
                datiPrepensionamento = new Entity.Oneri.DatiPrepensionamento();
                Utility.ValorizzaOggetti(prepensionamento, datiPrepensionamento);

                if (datiPrepensionamento.IsDatiPrepensionamentoNull())
                    datiPrepensionamento = null;
            }
        }

        public static void ValorizzaDatiPrepensionamentoForPrepensionamento(GestionePensione.DatiPensione datiPensione, ref Entity.Oneri.DatiPrepensionamento datiPrepensionamento)
        {
            int codiceLegge = 0;
            string tipoSettimaneBeneficio = string.Empty;

            if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione, out codiceLegge, out tipoSettimaneBeneficio))
            {
                if (datiPrepensionamento == null)
                    datiPrepensionamento = new DatiPrepensionamento();

                if (!datiPrepensionamento.CodiceLegge.HasValue)
                    datiPrepensionamento.CodiceLegge = codiceLegge;
            }
        }

        #endregion Dati Prepensionamento

        #region Controls
        public static bool ControlsVisibleTabs(GestionePensione.DatiPensione datiPensione, bool? IsOneri, bool? IsPrepensionamento, bool isRiaperturaDomanda, bool isBeneficioVittimeTerrorismo, bool isBeneficioENAV, bool isBeneficioNonVedente)
        {
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out ctrl);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (IsOneri.HasValue && IsOneri.Value && (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) ||
                Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale) ||
                isBeneficioVittimeTerrorismo ||
                ((Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.AGO || Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.FS) &&
                 Utility.IsDomandaSperimentaleDonna(datiPensione)) ||
                Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) ||
                Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) ||
                Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) || Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione) ||
                Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione) || Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione) ||
                Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) || Utility.IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(datiPensione) ||
                (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.FS) ||
                Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || isBeneficioENAV ||
                isBeneficioNonVedente || (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, false)) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) ||
                Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione) || (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) ||
                (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)))
                return true;

            if (IsPrepensionamento.HasValue && IsPrepensionamento.Value &&
                Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione) &&
                !(Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) ||
                Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) ||
                Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione)))
                return true;

            return false;
        }

        #endregion Controls

        #region Decodifica
        public static void GetListaGruppoOneri(out List<Entity.CodiciOneri.GruppoOneri> listaGruppoOneri)
        {
            listaGruppoOneri = new List<Entity.CodiciOneri.GruppoOneri>();
            List<Liquidazione.BLCommon.GestioneDecodifica.GruppoOneri> listaGruppoOneriDB = null;
            GestioneDecodifica.GetGruppoOneri(out listaGruppoOneriDB);
            if (listaGruppoOneriDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.GruppoOneri gruppoOneriDB in listaGruppoOneriDB)
                {
                    Entity.CodiciOneri.GruppoOneri gruppoOneri = new Entity.CodiciOneri.GruppoOneri();
                    gruppoOneri.Id = gruppoOneriDB.Id;
                    gruppoOneri.Descrizione = gruppoOneriDB.Descrizione;
                    gruppoOneri.Code = gruppoOneriDB.Code;
                    listaGruppoOneri.Add(gruppoOneri);
                }
            }
        }

        public static void GetListaSottoGruppoOneri(GestionePensione.DatiPensione datiPensione, out List<Entity.CodiciOneri.SottoGruppoOneri> listaSottoGruppoOneri)
        {
            listaSottoGruppoOneri = new List<Entity.CodiciOneri.SottoGruppoOneri>();
            List<Liquidazione.BLCommon.GestioneDecodifica.SottoGruppoOneri> listaSottoGruppoOneriDB = null;
            GestioneDecodifica.GetSottoGruppoOneri(out listaSottoGruppoOneriDB);
            if (listaSottoGruppoOneriDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.SottoGruppoOneri sottoGruppoOneriDB in listaSottoGruppoOneriDB)
                {
                    Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOneri = new Entity.CodiciOneri.SottoGruppoOneri();
                    //lo inserisce sempre se Ispubblica è null
                    if (!sottoGruppoOneriDB.IsPubblica.HasValue)
                    {
                        sottoGruppoOneri.Id = sottoGruppoOneriDB.Id;
                        sottoGruppoOneri.Descrizione = sottoGruppoOneriDB.Descrizione;
                        sottoGruppoOneri.Code = sottoGruppoOneriDB.Code;
                        sottoGruppoOneri.IdOnere = sottoGruppoOneriDB.IdOnere;
                        sottoGruppoOneri.IsPubblica = sottoGruppoOneriDB.IsPubblica;
                        listaSottoGruppoOneri.Add(sottoGruppoOneri);
                    }
                    else if (!Utility.IsDomandaINPDAP(datiPensione.Gestione) && sottoGruppoOneriDB.IsPubblica == false)
                    {
                        sottoGruppoOneri.Id = sottoGruppoOneriDB.Id;
                        sottoGruppoOneri.Descrizione = sottoGruppoOneriDB.Descrizione;
                        sottoGruppoOneri.Code = sottoGruppoOneriDB.Code;
                        sottoGruppoOneri.IdOnere = sottoGruppoOneriDB.IdOnere;
                        sottoGruppoOneri.IsPubblica = sottoGruppoOneriDB.IsPubblica;
                        listaSottoGruppoOneri.Add(sottoGruppoOneri);
                    }
                    else if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && sottoGruppoOneriDB.IsPubblica == true)
                    {
                        sottoGruppoOneri.Id = sottoGruppoOneriDB.Id;
                        sottoGruppoOneri.Descrizione = sottoGruppoOneriDB.Descrizione;
                        sottoGruppoOneri.Code = sottoGruppoOneriDB.Code;
                        sottoGruppoOneri.IdOnere = sottoGruppoOneriDB.IdOnere;
                        sottoGruppoOneri.IsPubblica = sottoGruppoOneriDB.IsPubblica;
                        listaSottoGruppoOneri.Add(sottoGruppoOneri);
                    }

                }
            }
        }
        #endregion Decodifica

        #region Cross Properties
        public static Dictionary<string, bool> GetCrossProperties(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            bool isBeneficioAmianto;
            bool isOneriSperDonnaObbligatori;
            bool isBeneficioVittimeTerrorismo;
            bool isPrepensionamentoEditoriaArt1c154L205_2017;
            bool isPrepensionamentoEditoriaArt1c500L160_2019;
            bool isDomandaSperimentaleDonna_DL_4_2019OrRicostituzione;
            bool isDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione;
            bool isPrepensionamentoEditoria;
            bool isOpzioneDonna_Legge197_2022_Art1_Comma292;
            bool isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione;
            bool isPrepensionamentoEditoriaLetteraB;
            bool isOneriPresentiDaAzienda;
            bool isRicVOPGIMigrataFiltroEBA = false;

            Dictionary<string, bool> lReturn = new Dictionary<string, bool>();
            isBeneficioAmianto = IsBeneficioAmianto(datiPensione);
            isOneriSperDonnaObbligatori = Utility.IsOneriSperDonnaObbligatoriPerControlli(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione) ||
                Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione);

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);
            isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo);

            isPrepensionamentoEditoriaArt1c154L205_2017 = Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione);
            isPrepensionamentoEditoriaArt1c500L160_2019 = Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione);
            isDomandaSperimentaleDonna_DL_4_2019OrRicostituzione = Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione);
            isDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione = Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione);
            isPrepensionamentoEditoria = Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione);
            isOpzioneDonna_Legge197_2022_Art1_Comma292 = Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                                                         Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true);
            isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione = isOpzioneDonna_Legge197_2022_Art1_Comma292 || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) ||
                                                                         Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione);

            isPrepensionamentoEditoriaLetteraB = Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione);

            List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB> listaAccordiLetteraB = null;
            GestioneAnagraficaAccordiLetteraB.GetDecAnagraficaAccordi(out listaAccordiLetteraB);

            List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB> listaAziendeLetteraB = null;
            GestioneAnagraficaAziendeLetteraB.GetDecAnagraficaAziende(out listaAziendeLetteraB);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB accordoLetteraB = listaAccordiLetteraB.FirstOrDefault(x => datiIstruttoria != null && x.Codice == datiIstruttoria.CodiceAziendaEditoriaLetteraB);
            isOneriPresentiDaAzienda = accordoLetteraB != null;

            if (Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) && Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) && !String.IsNullOrEmpty(datiPensione.GP1AV91B) && datiPensione.GP1AV91B.Trim() == "2")
            {
                isRicVOPGIMigrataFiltroEBA = true;
            }

            lReturn.Add("IsBeneficioAmianto", isBeneficioAmianto);
            lReturn.Add("IsOneriSperDonnaObbligatori", isOneriSperDonnaObbligatori);
            lReturn.Add("IsBeneficioVittimeTerrorismo", isBeneficioVittimeTerrorismo);
            lReturn.Add("IsPrepensionamentoEditoriaArt1c154L205_2017", isPrepensionamentoEditoriaArt1c154L205_2017);
            lReturn.Add("IsPrepensionamentoEditoriaArt1c500L160_2019", isPrepensionamentoEditoriaArt1c500L160_2019);
            lReturn.Add("IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione", isDomandaSperimentaleDonna_DL_4_2019OrRicostituzione);
            lReturn.Add("IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione", isDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione);
            lReturn.Add("IsPrepensionamentoEditoria", isPrepensionamentoEditoria);
            lReturn.Add("IsOpzioneDonna_Legge197_2022_Art1_Comma292", isOpzioneDonna_Legge197_2022_Art1_Comma292);
            lReturn.Add("IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione", isOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione);
            lReturn.Add("IsPrepensionamentoEditoriaLetteraB", isPrepensionamentoEditoriaLetteraB);
            lReturn.Add("IsOneriPresentiDaAzienda", isOneriPresentiDaAzienda);
            lReturn.Add("IsRicVOPGIMigrataFiltroEBA", isRicVOPGIMigrataFiltroEBA);

            return lReturn;
        }

        private static bool IsBeneficioAmianto(GestionePensione.DatiPensione datiPensione)
        {
            string tipoBeneficio;
            int codiceLegge;
            Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione, out codiceLegge, out tipoBeneficio);
            if (tipoBeneficio == "04")
                return true;
            return false;
        }


        #endregion Cross Properties



    }
}
