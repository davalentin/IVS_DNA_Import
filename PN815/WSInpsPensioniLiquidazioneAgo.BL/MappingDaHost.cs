using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class MappingDaHost
    {
        #region private members
        //private static void GetReqAnzVecchByGP1AXE3(short gp1axe3, out bool? vecch94, out bool? anz94, out bool? anz96)
        //{
        //    vecch94 = null;
        //    anz94 = null;
        //    anz96 = null;

        //    switch (gp1axe3)
        //    {
        //        case 1:
        //            vecch94 = true;
        //            anz96 = true;
        //            break;
        //        case 2:
        //            vecch94 = true;
        //            anz94 = true;
        //            anz96 = true;
        //            break;
        //        case 3:
        //            vecch94 = true;
        //            break;
        //        case 4:
        //            vecch94 = true;
        //            anz94 = true;
        //            break;
        //        case 5:
        //            anz96 = true;
        //            break;
        //        case 6:
        //            anz94 = true;
        //            anz96 = true;
        //            break;
        //        case 8:
        //            anz94 = true;
        //            break;
        //        case 7:
        //        default:
        //            break;
        //    }
        //}

        private static char? GetModalitaPagamento(Data.CAREPET.Pagamento pagamento, Data.CAREPET.Coda coda)
        {
            char? modPag = null;
            if (pagamento == null)
                return modPag;

            modPag = Utility.StringToNullableChar(pagamento.T_GP1CTIPPAG_V);
            if (!modPag.HasValue)
            {
                if (pagamento.T_GP1CABI_V == 07601)
                {
                    if (coda != null && coda.AreaDati2007 != null && !string.IsNullOrEmpty(coda.AreaDati2007.T_GP1IBAN))
                        modPag = 'L';
                    else
                        modPag = 'S';
                }
                else if (pagamento.T_GP1CABI_V == 36081 && pagamento.T_GP1CCAB_V == 05138)
                {
                    if (coda != null && coda.AreaDati2007 != null && !string.IsNullOrEmpty(coda.AreaDati2007.T_GP1IBAN))
                        modPag = 'K';
                    else
                        modPag = 'S';
                }
                else if (pagamento.T_GP1CCAB_V != 0 && (pagamento.T_GP1CCAB_V.ToString().StartsWith("44") || pagamento.T_GP1CCAB_V.ToString().StartsWith("77")) && pagamento.T_GP1CCAB_V.ToString().Length >= 7)
                {
                    if (coda != null && coda.AreaDati2007 != null && !string.IsNullOrEmpty(coda.AreaDati2007.T_GP1IBAN))
                        modPag = 'C';
                    else
                        modPag = 'S';
                }
                else if (pagamento.T_GP1CABI_V != 0)
                {
                    modPag = 'S';
                }
                else
                {
                    if (coda != null && coda.AreaDati2007 != null && !string.IsNullOrEmpty(coda.AreaDati2007.T_GP1IBAN))
                    {
                        if ((pagamento.T_GP1CCAB_V != 0) || (coda.AreaDati2007 != null && !string.IsNullOrEmpty(coda.AreaDati2007.T_GP1BIC)))
                            modPag = 'C';
                        else
                            modPag = 'L';
                    }
                }
            }

            return modPag;
        }

        private static void GetQuotaByDecorrRetr(short meseDecRetr, out char? quota)
        {
            quota = null;

            if (meseDecRetr == 61 || meseDecRetr == 62 || meseDecRetr == 63 || meseDecRetr == 64 || meseDecRetr == 66 || meseDecRetr == 67 || meseDecRetr == 68 ||
                meseDecRetr == 16 || meseDecRetr == 21 || meseDecRetr == 31 || meseDecRetr == 41 || meseDecRetr == 51 ||
                meseDecRetr == 91 || meseDecRetr == 92 || meseDecRetr == 93 || meseDecRetr == 94 || meseDecRetr == 98)
                quota = 'B';
            else
                quota = 'A';
        }

        private static void GetQuotaByDecorrRetrDAI(short meseDecRetr, out char? quota)
        {
            quota = null;

            if (meseDecRetr == 61 || meseDecRetr == 62 || meseDecRetr == 63 || meseDecRetr == 64 || meseDecRetr == 66 || meseDecRetr == 67 || meseDecRetr == 68 ||
                meseDecRetr == 16 || meseDecRetr == 21 || meseDecRetr == 31 || meseDecRetr == 41 || meseDecRetr == 51 ||
                meseDecRetr == 91 || meseDecRetr == 92 || meseDecRetr == 93 || meseDecRetr == 94 || meseDecRetr == 98
                || meseDecRetr == 52 || meseDecRetr == 53 || meseDecRetr == 54 || meseDecRetr == 56 || meseDecRetr == 57 || meseDecRetr == 58
                || meseDecRetr == 59 || meseDecRetr == 60 || meseDecRetr == 70)
                quota = 'B';
            else
                quota = 'A';
        }
        private static void GetCodiceTipoQuotaByDecorrRetr(short meseDecRetr, List<CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExInpdai, out string codiceTipoQuota)
        {
            codiceTipoQuota = null;

            if (listaCtrlDecorrenzaRetrExInpdai == null || listaCtrlDecorrenzaRetrExInpdai.Count == 0)
                return;

            CtrlDecorrenzaRetrExINPDAI ctrl = null;

            if (meseDecRetr == 1 || meseDecRetr == 2 || meseDecRetr == 3 || meseDecRetr == 4 ||
                meseDecRetr == 5 || meseDecRetr == 6 || meseDecRetr == 7 || meseDecRetr == 8 ||
                meseDecRetr == 9 || meseDecRetr == 10 || meseDecRetr == 11 || meseDecRetr == 12)
                ctrl = listaCtrlDecorrenzaRetrExInpdai.Find(x => x.CodiceDecorrenza == 76);
            else
                ctrl = listaCtrlDecorrenzaRetrExInpdai.Find(x => x.CodiceDecorrenza == meseDecRetr);

            if (ctrl != null)
                codiceTipoQuota = ctrl.TipoQuota;
        }

        private static string GetCodiceCategoriaFromAreaPrelievo(Data.GAIN AreaPrelievo)
        {
            string categoriaFromHost = null;
            if (AreaPrelievo.Response.DatiGenerici != null && !string.IsNullOrEmpty(AreaPrelievo.Response.DatiGenerici.T_GP1AB01_V))
                categoriaFromHost = AreaPrelievo.Response.DatiGenerici.T_GP1AB01_V.Trim().ToUpperInvariant().PadLeft(4, '0');
            return categoriaFromHost;
        }

        private static long? GetGestioneFromQuotaDecorrenza(Data.GAIN AreaPrelievo, string codiceCategoria, int mese, char? quota,
            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo, int anno)
        {
            long? codiceGestione = null;
            short? meseDecorrenzaOpzione = null;
            short? meseDecorrenzaOriginaria = null;
            byte? provenienzaPensione = null;
            short? meseDecorrenzaPensioneDA = null;
            if (AreaPrelievo != null && AreaPrelievo.Response != null)
            {
                if (AreaPrelievo.Response.Istruttoria != null)
                {
                    var istruttoria = AreaPrelievo.Response.Istruttoria;
                    meseDecorrenzaOpzione = istruttoria.T_GP1AG03M;
                    meseDecorrenzaOriginaria = istruttoria.T_GP1AD01_OM_V;
                }
                if (AreaPrelievo.Response.DanteCausa != null)
                {
                    var areaDanteCausa = AreaPrelievo.Response.DanteCausa;
                    provenienzaPensione = (byte)areaDanteCausa.T_GP7LC04;
                    meseDecorrenzaPensioneDA = areaDanteCausa.T_GP7LC02M;
                }
            }

            string gestioneApp = null;
            switch (codiceCategoria)
            {
                case "0001": // VO
                case "0002": // IO
                case "0003": // SO
                case "0007": // VOP
                case "0008": // IOP
                case "0009": // SOP
                case "0013": // VOMIN
                case "0014": // SOMIN
                case "0035": // VMP
                case "0036": // IMP
                case "0079": // PMO
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
                    break;
                case "0032": // VOBANC
                case "0033": // IOBANC
                    if (quota == 'A')
                    {
                        if (mese == 75)
                            gestioneApp = "H";
                        else if (mese == meseDecorrenzaOpzione || mese == meseDecorrenzaOriginaria)
                            gestioneApp = "1";
                        else
                            gestioneApp = "S";
                    }
                    else
                    {
                        if (mese == 65)
                            gestioneApp = "H";
                        else
                            gestioneApp = (mese - 60).ToString();
                    }
                    break;
                case "0034": // SOBANC
                    if (quota == 'A')
                    {
                        if (mese == 75)
                            gestioneApp = "H";
                        else if (mese == meseDecorrenzaOpzione ||
                            (mese == meseDecorrenzaOriginaria && (!provenienzaPensione.HasValue || provenienzaPensione.Value == 0)) ||
                            (mese == meseDecorrenzaPensioneDA && (provenienzaPensione.HasValue && (provenienzaPensione.Value == 1 || provenienzaPensione.Value == 2))))
                            gestioneApp = "1";
                        else if (provenienzaPensione == 0)
                            gestioneApp = "S";
                    }
                    else
                    {
                        if (mese == 65)
                            gestioneApp = "H";
                        else
                            gestioneApp = (mese - 60).ToString();
                    }
                    break;
                case "0027": // VOCRED
                case "0028": // VOCOOP
                case "0029": // VOESO
                case "0127": // CRED27
                case "0128": // COOP28
                case "0129": // VESO29
                case "0196": // ESOAMB
                case "0197": // ESOTEL
                case "0198": // VESO33
                case "0199": // VESO92
                case "0200": // ESPA
                    if (quota == 'A')
                    {
                        if (mese == meseDecorrenzaOriginaria)
                            gestioneApp = "1";
                        else
                            gestioneApp = (mese - 70).ToString();
                    }
                    else
                    {
                        gestioneApp = (mese - 60).ToString();
                    }
                    break;
                case "0015": // VR
                case "0016": // IR
                case "0017": // SR
                case "0018": // VOART
                case "0019": // IOART
                case "0020": // SOART
                case "0021": // VOCOM
                case "0022": // IOCOM
                case "0023": // SOCOM
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
                    break;
                default:
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
                    break;
            }

            if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
            {
                GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == gestioneApp && !x.IsFondo);
                if (codeGestioneCalcoloRetributivo != null)
                    codiceGestione = codeGestioneCalcoloRetributivo.Id;
            }

            return codiceGestione;
        }
        #endregion private members

        #region public members
        public static void ValorizzaDatiPensione(Data.GAIN AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, List<string> categorieSpacchettamentoENPALS, List<string> categorieENPALS, out GestionePensione.DatiPensione datiPensione, out bool? enteIstruttoreExInpdap)
        {
            datiPensione = null;
            enteIstruttoreExInpdap = null;
            string categoriaFromHost = AreaPrelievo.Response.DatiGenerici != null ? AreaPrelievo.Response.DatiGenerici.T_GP1AB01_V.Trim().ToUpperInvariant().PadLeft(4, '0') : null;

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            //ENG - Memo 57_2023
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoAbilitazioneMemo57_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo57_2023", out controlloDinamicoAbilitazioneMemo57_2023);

            if (AreaPrelievo.Response != null &&
                (AreaPrelievo.Response.Istruttoria != null || AreaPrelievo.Response.Coda != null))
            {
                datiPensione = new GestionePensione.DatiPensione();
                Data.CAREPET.Istruttoria istruttoria = AreaPrelievo.Response.Istruttoria;
                Data.CAREPET.Coda coda = AreaPrelievo.Response.Coda;
                Data.CAREPET.Pagamento pagamento = AreaPrelievo.Response.Pagamento;
                bool isPensioneEliminata = AreaPrelievo.Response.Pagamento != null && !string.IsNullOrEmpty(AreaPrelievo.Response.Pagamento.T_GP1AM01_V) && AreaPrelievo.Response.Pagamento.T_GP1AM01_V != 0.ToString();

                if (istruttoria != null)
                {
                    string codiceCategoria = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);
                    string siglaCategoria = "";
                    GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(codiceCategoria, out siglaCategoria);
                    DateTime? decorrenzaPensione;
                    if (Utility.IsDomandaIOCUM(siglaCategoria) && istruttoria.T_GP1AD02_V != 0)
                    {
                        decorrenzaPensione = Utility.DataFromInt(istruttoria.T_GP1AD01_OA_V, istruttoria.T_GP1AD01_OM_V, istruttoria.T_GP1AD02_V);
                        datiPensione.DecorrenzaOriginaria = decorrenzaPensione;
                        enteIstruttoreExInpdap = true;
                    }
                    else if (Utility.IsDomandaESOPMI(siglaCategoria))
                    {
                        if (istruttoria.T_GP1AD01_OA_V == 2022 && istruttoria.T_GP1AD01_OM_V == 12 && istruttoria.T_GP1AD02_V != 0)
                            decorrenzaPensione = Utility.DataFromInt(istruttoria.T_GP1AD01_OA_V, istruttoria.T_GP1AD01_OM_V, istruttoria.T_GP1AD02_V);
                        else
                            decorrenzaPensione = Utility.DataFromInt(istruttoria.T_GP1AD01_OA_V, istruttoria.T_GP1AD01_OM_V, 1);
                        datiPensione.DecorrenzaOriginaria = decorrenzaPensione;
                    }
                    else
                        decorrenzaPensione = Utility.DataFromInt(istruttoria.T_GP1AD01_OA_V, istruttoria.T_GP1AD01_OM_V, 1);

                    datiPensione.NaturaPensione = istruttoria.T_GP1AF02.PadRight(3, ' ');
                    datiPensione.CausaCarico = Utility.StringToNullableByte(istruttoria.T_GP1AJ01_V.ToString());

                    if (AreaPrelievo.Request.Controllo.TIPO_RICHIESTA == "53")
                    {
                        if (Utility.IsDomandaVOESO(siglaCategoria) || Utility.IsDomandaVESO29(siglaCategoria) || Utility.IsDomandaVESO33(siglaCategoria) ||
                            ((Utility.IsDomandaCRED27(siglaCategoria) || Utility.IsDomandaCOOP28(siglaCategoria)) && !isPensioneEliminata))
                            datiPensione.CodiceArretrati = 1;
                        else
                            datiPensione.CodiceArretrati = 8;
                    }
                    else
                        datiPensione.CodiceArretrati = Utility.StringToNullableByte(istruttoria.T_GP1AJ05.ToString());

                    if (istruttoria.T_GP1ALA1_V != 0)
                        datiPensione.AliquotaTFREsodati = istruttoria.T_GP1ALA1_V;
                    if (istruttoria.T_GP1AV04 != 0)
                        datiPensione.AttivitaEconomica = istruttoria.T_GP1AV04;
                    if (istruttoria.T_GP1AV05 != 0)
                        datiPensione.ProfessioneIndividuale = istruttoria.T_GP1AV05;
                    datiPensione.DataInizioCalcolo = Utility.DataFromInt(istruttoria.T_GP1AXA4A_V, istruttoria.T_GP1AXA4M_V, 1);

                    if (istruttoria.T_GP1CENTCRD_V != 0 && !(categoriaFromHost == "0170" || categoriaFromHost == "0171" || categoriaFromHost == "0172" || categoriaFromHost == "0070" || categoriaFromHost == "0071" || categoriaFromHost == "0072" ||
                        categoriaFromHost == "0032" || categoriaFromHost == "0033" || categoriaFromHost == "0034"))
                    {
                        List<GestioneDecodificaAzienda.DecAzienda> elencoAziendaEditoria = null;

                        GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(!string.IsNullOrEmpty(siglaCategoria) ? siglaCategoria.Trim() : string.Empty, null, out elencoAziendaEditoria);
                        if (elencoAziendaEditoria != null)
                        {
                            GestioneDecodificaAzienda.DecAzienda decAziendaEditoria = elencoAziendaEditoria.Find(x => x.TraduzioneSuGP.Trim() == istruttoria.T_GP1CENTCRD_V.ToString().PadLeft(4, '0'));
                            if (decAziendaEditoria == null)
                                decAziendaEditoria = elencoAziendaEditoria.Find(x => x.TraduzioneSuGP.Trim() == istruttoria.T_GP1CENTCRD_V.ToString());
                            if (decAziendaEditoria != null)
                                datiPensione.CodiceBancaEsodati = decAziendaEditoria.Id;
                        }
                    }
                    else if (istruttoria.T_GP1CENTCRD_V != 0 && AreaPrelievo.Response.DatiGenerici != null && (categoriaFromHost == "0032" || categoriaFromHost == "0033" || categoriaFromHost == "0034"))
                    {
                        List<GestioneDecodifica.DecodificaBanchePerSede> elencoBanchePerSede = null;
                        GestioneDecodifica.GetDecodificaBanchePerSede(out elencoBanchePerSede);
                        if (elencoBanchePerSede != null)
                        {
                            var bancaPerSede = elencoBanchePerSede.Find(x => x.CodiceSede == AreaPrelievo.Response.DatiGenerici.T_GP1AB02_V.ToString().PadLeft(4, '0') && x.TraduzioneSuGP == istruttoria.T_GP1CENTCRD_V.ToString().PadLeft(4, '0'));
                            if (bancaPerSede != null)
                                datiPensione.CodiceBancaEsodati = Convert.ToInt16(bancaPerSede.Id);
                        }
                    }

                    datiPensione.InizioAssicurazione = Utility.DataFromInt(istruttoria.T_GP2BM01A, istruttoria.T_GP2BM01M, istruttoria.T_GP2BM01G);
                    datiPensione.FineAssicurazione = Utility.DataFromInt(istruttoria.T_GP2BM02A, istruttoria.T_GP2BM02M, istruttoria.T_GP2BM02G);
                    if ((decorrenzaPensione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensione.Value, new DateTime(2011, 1, 1))) || Utility.IsDomandaVOESO(siglaCategoria) || Utility.IsDomandaVOCRED(siglaCategoria) || Utility.IsDomandaVOCOOP(siglaCategoria) || Utility.IsDomandaVOAUT_IOAUT_SOAUT(siglaCategoria))
                        datiPensione.DataPerfezionamentoRequisiti = Utility.DataFromInt(istruttoria.T_GP2BM03A, istruttoria.T_GP2BM03M, istruttoria.T_GP2BM03G);
                    datiPensione.DataInteressiLegali = Utility.DataFromInt(istruttoria.T_TP1ILEGA, istruttoria.T_TP1ILEGM, istruttoria.T_TP1ILEGG);
                    if (!String.IsNullOrEmpty(istruttoria.T_TP1NOARC))
                        datiPensione.FlagVerify = istruttoria.T_TP1NOARC.Trim().ToUpperInvariant() == "1" ? true : istruttoria.T_TP1NOARC.Trim().ToUpperInvariant() == "0" ? false : (bool?)null;

                    datiPensione.Contributivo = !string.IsNullOrEmpty(istruttoria.T_GP1AF03_V) && istruttoria.T_GP1AF03_V != 0.ToString() ? istruttoria.T_GP1AF03_V[0] : (char?)null;

                    if (Utility.IsDomandaCRED27(siglaCategoria) || Utility.IsDomandaVESO33(siglaCategoria) || Utility.IsDomandaVESO92(siglaCategoria) || Utility.IsDomandaESPA(siglaCategoria))
                    {
                        datiPensione.IsRicExtracalcolo = !string.IsNullOrEmpty(istruttoria.T_GP1AF03_V) && !string.IsNullOrEmpty(istruttoria.T_GP1AF03_V.Trim()) && (istruttoria.T_GP1AF03_V.Trim() == "1" || istruttoria.T_GP1AF03_V.Trim() == "7") ? true : false;
                    }
                    if (categorieENPALS.Contains(categoriaFromHost) || (AreaPrelievo.Response.DatiGenerici.T_GP1AB02_V.ToString() == "9933" && (codiceCategoria.Trim() == "0801" || codiceCategoria.Trim() == "0802")))
                    {
                        if (!string.IsNullOrEmpty(istruttoria.T_GP1AF03_V))
                        {
                            switch (istruttoria.T_GP1AF03_V)
                            {
                                case "1":
                                case "8":
                                    datiPensione.TipoCalcolo = (byte)Utility.TipoCalcolo.Contributivo;
                                    break;
                                case "2":
                                case "3":
                                case "4":
                                    datiPensione.TipoCalcolo = (byte)Utility.TipoCalcolo.Retributivo;
                                    break;
                            }
                        }
                    }
                    else if (codiceCategoria.Trim() == "0243" || codiceCategoria.Trim() == "0244" || codiceCategoria.Trim() == "0245")
                    {
                        switch (istruttoria.T_GP1AF03_V)
                        {
                            case "8":
                                datiPensione.TipoCalcolo = (byte)Utility.TipoCalcolo.Contributivo;
                                break;
                            case "2":
                                datiPensione.TipoCalcolo = (byte)Utility.TipoCalcolo.Retributivo;
                                break;
                        }
                    }
                }
                if (coda != null)
                {
                    if (coda.AreaDati2012 != null && tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione &&
                        !string.IsNullOrEmpty(coda.AreaDati2012.T_GP1ALZ6) && coda.AreaDati2012.T_GP1ALZ6.Length == 6)
                    {
                        if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO))
                        {
                            datiPensione.CodiceSedeDestinazione = AreaPrelievo.Response.DatiGenerici.T_GP1AB02_V;
                            datiPensione.CentroOperativoDestinazione = 0;

                            short codiceSedeGP1ALZ6 = 0;
                            byte centroOperativoGP1ALZ6 = 0;
                            if (short.TryParse(coda.AreaDati2012.T_GP1ALZ6.Substring(0, 4), out codiceSedeGP1ALZ6))
                                datiPensione.CodiceSedeGP1ALZ6 = codiceSedeGP1ALZ6;
                            if (byte.TryParse(coda.AreaDati2012.T_GP1ALZ6.Substring(4, 2), out centroOperativoGP1ALZ6))
                                datiPensione.CentroOperativoGP1ALZ6 = centroOperativoGP1ALZ6;
                        }
                        else
                        {
                            short codiceSedeDestinazione = 0;
                            byte centroOperativoDestinazione = 0;
                            if (short.TryParse(coda.AreaDati2012.T_GP1ALZ6.Substring(0, 4), out codiceSedeDestinazione))
                                datiPensione.CodiceSedeDestinazione = codiceSedeDestinazione;
                            if (byte.TryParse(coda.AreaDati2012.T_GP1ALZ6.Substring(4, 2), out centroOperativoDestinazione))
                                datiPensione.CentroOperativoDestinazione = centroOperativoDestinazione;
                        }
                    }


                    if (coda.AreaDati2008 != null && coda.AreaDati2008.LISTT_ELTAB_GP2PB != null && coda.AreaDati2008.LISTT_ELTAB_GP2PB.Count() > 0 && coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 5300 || x.T_GP2PBPLEG == 5800 || x.T_GP2PBPLEG == 6000 || x.T_GP2PBPLEG == 6100))
                    {
                        if ((coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 5300) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 5300).T_GP2PBPLEG1 == 5301) ||
                            (coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 5800) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 5800).T_GP2PBPLEG1 == 5801) ||
                            (coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 6000) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 6000).T_GP2PBPLEG1 == 6001) ||
                            (coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 6100) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 6100).T_GP2PBPLEG1 == 6101))
                            datiPensione.LavoratorePubblico = false;
                        else if ((coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 5300) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 5300).T_GP2PBPLEG1 == 5302) ||
                            (coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 5800) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 5800).T_GP2PBPLEG1 == 5802) ||
                             (coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 6000) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 6000).T_GP2PBPLEG1 == 6002) ||
                             (coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBPLEG == 6100) && coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBPLEG == 6100).T_GP2PBPLEG1 == 6102))
                            datiPensione.LavoratorePubblico = true;
                    }
                    if (coda.AreaDati2008 != null && coda.AreaDati2008.LISTT_ELTAB_GP2PB != null && coda.AreaDati2008.LISTT_ELTAB_GP2PB.Count() > 0 &&
                        coda.AreaDati2008.LISTT_ELTAB_GP2PB.Exists(x => x.T_GP2PBNFGL > 0))
                    {
                        byte nFigli = 0;
                        byte.TryParse(coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBNFGL > 0).T_GP2PBNFGL.ToString(), out nFigli);
                        datiPensione.NumeroFigli = nFigli;
                    }
                    //Tipo contributivo optanti
                    if (!string.IsNullOrEmpty(istruttoria.T_GP1AV61))
                    {
                        switch (istruttoria.T_GP1AV61)
                        {
                            case "12":
                                datiPensione.SceltaLavMadri = 1;
                                break;
                            case "15":
                                datiPensione.SceltaLavMadri = 2;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Intestazione != null)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                if (!string.IsNullOrEmpty(AreaPrelievo.Response.Intestazione.T_WEBDOAS4))
                    datiPensione.DirittoAutonomo = AreaPrelievo.Response.Intestazione.T_WEBDOAS4;
            }
            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.NuoviDati2024 != null)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                if (AreaPrelievo.Response.NuoviDati2024.AreaDati2024 != null)
                {
                    if (!string.IsNullOrEmpty(AreaPrelievo.Response.NuoviDati2024.AreaDati2024.T_GP1AJ10ZD))
                        datiPensione.DataCondizioniPerComputo = Utility.DataFromString(AreaPrelievo.Response.NuoviDati2024.AreaDati2024.T_GP1AJ10ZD, Utility.FormatoData.AAAAmmGG);

                    datiPensione.GP1AV91A = AreaPrelievo.Response.NuoviDati2024.AreaDati2024.T_GP1AV91A;
                    if (datiPensione.GP1AV91A == null)
                        datiPensione.GP1AV91A = 0;
                }
            }

            //ENG - Memo 28_2024 recupero GP1TPCLC (secondo byte = 1)
            //if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            //{
            //    if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2018 != null &&
            //        !String.IsNullOrEmpty(AreaPrelievo.Response.Coda.AreaDati2018.T_GP1TPCLC))
            //    {
            //        if (AreaPrelievo.Response.Coda.AreaDati2018.T_GP1TPCLC.Length >= 2 && AreaPrelievo.Response.Coda.AreaDati2018.T_GP1TPCLC.Substring(1, 1) == "1")
            //            datiPensione.Caratterizzazione = " 1      ";
            //    }
            //}

            //ENG - Memo 57_2023
            if (controlloDinamicoAbilitazioneMemo57_2023 != null && !String.IsNullOrEmpty(controlloDinamicoAbilitazioneMemo57_2023.ValoreControllo) &&
                controlloDinamicoAbilitazioneMemo57_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if (AreaPrelievo != null)
                {
                    string codiceCategoria = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);
                    string siglaCategoria = "";
                    GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(codiceCategoria, out siglaCategoria);
                    if (Utility.IsDomandaAPESociale(siglaCategoria))
                    {
                        if (AreaPrelievo.Response != null && AreaPrelievo.Response.DatiGenerici != null)
                            datiPensione.AnnoMonitoraggio = AreaPrelievo.Response.DatiGenerici.T_GP1AT22;
                    }
                }
            }

            //ENG  - Memo 108_2024 (per la gestione del campo "CaratterizzazioneLegge" non serve la chiave "AbilitazioneMemo108_2024")
            if (categoriaFromHost == "0170") //VOCUM
            {
                if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2018 != null &&
                    !String.IsNullOrEmpty(AreaPrelievo.Response.Coda.AreaDati2018.T_GP1TPCLC) && !String.IsNullOrEmpty(AreaPrelievo.Response.Coda.AreaDati2018.T_GP1TPCLC.Trim()))
                {
                    if (AreaPrelievo.Response.Coda.AreaDati2018.T_GP1TPCLC.Length >= 2 && AreaPrelievo.Response.Coda.AreaDati2018.T_GP1TPCLC.Substring(1, 1) == "2")
                        datiPensione.Caratterizzazione = "1";
                }
            }

        }

        public static void ValorizzaDatiIstruttoria(Data.GAIN AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, GestionePrelievo.TipoRicostituzione tipoRicostituzione, List<string> categorieENPALS,
            ref GestionePensione.DatiPensione datiPensione, out GestioneEnpals.DatiEnpals datiEnpals, out GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            datiIstruttoria = null;
            datiEnpals = null;
            if (AreaPrelievo.Response != null && (
                AreaPrelievo.Response.Istruttoria != null || AreaPrelievo.Response.Coda != null))
            {
                datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                datiEnpals = new GestioneEnpals.DatiEnpals();
                Data.CAREPET.Istruttoria istruttoria = AreaPrelievo.Response.Istruttoria;
                Data.CAREPET.Coda coda = AreaPrelievo.Response.Coda;

                string categoriaFromHost = AreaPrelievo.Response.DatiGenerici != null ? AreaPrelievo.Response.DatiGenerici.T_GP1AB01_V.Trim().ToUpperInvariant().PadLeft(4, '0') : null;
                List<string> categorieAutonomi = new List<string> { "0015", "0016", "0017", "0018", "0019", "0020", "0021", "0022", "0023" };

                if (istruttoria != null)
                {
                    //NOTA: Il campo T_GP1AB01_V per VESO33 contiene la scadenzaAssegno
                    datiIstruttoria.GP1AF08 = (byte?)istruttoria.T_GP1AF08;
                    if (categoriaFromHost != "0198" && categoriaFromHost != "0199" && categoriaFromHost != "0127" && categoriaFromHost != "0128" && categoriaFromHost != "0028" && categoriaFromHost != "0027" &&
                        categoriaFromHost != "0029" && categoriaFromHost != "0143" && categoriaFromHost != "0129" && categoriaFromHost != "0200")
                        datiIstruttoria.ScadenzaRevisioneSanitaria = Utility.DataFromInt(istruttoria.T_GP1AF06A_V, istruttoria.T_GP1AF06M_V, 1);
                    if (!(categoriaFromHost == "0029" && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05 != null && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "L1"))
                        datiIstruttoria.DataDomandaOpzione = Utility.DataFromInt(istruttoria.T_GP1AG02A, istruttoria.T_GP1AG02M, istruttoria.T_GP1AG02G);
                    datiIstruttoria.DecorrenzaOpzione = Utility.DataFromInt(istruttoria.T_GP1AG03A, istruttoria.T_GP1AG03M, 1);
                    if (istruttoria.T_GP1AJ02 != 0)
                        datiIstruttoria.CodiceDomandaRicorso = Utility.StringToNullableByte(istruttoria.T_GP1AJ02.ToString());
                    if (istruttoria.T_GP1AJ08_V != 0)
                        datiIstruttoria.CodiceCdCmMr = Utility.StringToNullableByte(istruttoria.T_GP1AJ08_V.ToString());
                    if (istruttoria.T_GP1AP49 != 0)
                        datiIstruttoria.CodiceMobilita = Utility.StringToNullableByte(istruttoria.T_GP1AP49.ToString());

                    //Richiesta 20151221 (MAIL Pasquale Cozzolino oggetto: 'FW: LiqPens AGO - Segnalazioni')
                    if (categorieAutonomi.Contains(categoriaFromHost))
                        datiIstruttoria.NSettimaneOBG = istruttoria.T_GP2BN02;
                    else
                    {
                        if (categorieENPALS.Contains(categoriaFromHost))
                        {
                            if (istruttoria.T_GP1AV08 != 0)
                                datiEnpals.AnzianitaContributiva = (short)istruttoria.T_GP1AV08;
                        }
                        else
                            datiIstruttoria.NSettimaneOBG = istruttoria.T_GP1AV08;
                        datiIstruttoria.NContributiVolontari = istruttoria.T_GP1AV09;
                        datiIstruttoria.NContributiVVAnzianita = istruttoria.T_GP1AV10;
                    }

                    if (istruttoria.T_GP1AV11_V != 0)
                    {
                        datiIstruttoria.CodiceCentroOperativo = istruttoria.T_GP1AV11_V.ToString();

                        if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                        {
                            short Gp1av11 = 0;
                            short.TryParse(istruttoria.T_GP1AV11_V.ToString(), out Gp1av11);
                            datiPensione.GP1AV11 = Gp1av11;
                        }
                    }

                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                    {
                        if (istruttoria.T_GP1AV53 != 0)
                            datiIstruttoria.CodiceP18PrecedentePensione = istruttoria.T_GP1AV53;
                        if (istruttoria.T_GP1AV54 != 0)
                            datiIstruttoria.SedePrecedentePensione = istruttoria.T_GP1AV54;
                        if (istruttoria.T_GP1AV55 != 0)
                            datiIstruttoria.CertificatoPrecedentePensione = istruttoria.T_GP1AV55;

                        if (datiIstruttoria.CodiceP18PrecedentePensione.HasValue || datiIstruttoria.SedePrecedentePensione.HasValue || datiIstruttoria.CertificatoPrecedentePensione.HasValue)
                        {
                            if (datiPensione == null)
                                datiPensione = new GestionePensione.DatiPensione();

                            datiPensione.TrasformazioneAOI = true;
                        }
                    }

                    //TODO occorrerebbe discriminare per categoria094 (true/false)
                    if (istruttoria.T_GP1AV91I != 0)
                    {
                        List<GestioneDecodifica.PensioneExInpdai> elencoPensioniExIpdai = null;
                        GestioneDecodifica.GetPensioniExInpdai(out elencoPensioniExIpdai);
                        if (elencoPensioniExIpdai != null && elencoPensioniExIpdai.Count > 0)
                        {
                            GestioneDecodifica.PensioneExInpdai pensioneExInpdai = elencoPensioniExIpdai.Find(x => x.TraduzioneSuGp == istruttoria.T_GP1AV91I);
                            if (pensioneExInpdai != null)
                                datiIstruttoria.TipoPensioneExInpdai = pensioneExInpdai.Id;
                        }
                    }

                    datiIstruttoria.CodPosizioneLavoro = istruttoria.T_GP1CPOSLVR;
                    datiIstruttoria.DecorrenzaOriginariaAltraPensione = Utility.DataFromInt(istruttoria.T_GP1AV51A, istruttoria.T_GP1AV51M, 1);

                    List<GestioneDecodifica.CodiceParticolare> elencoCodiciParticolari = null;
                    GestioneDecodifica.GetCodiciParticolari(out elencoCodiciParticolari);
                    if (elencoCodiciParticolari != null && elencoCodiciParticolari.Count > 0 && !string.IsNullOrEmpty(istruttoria.T_GP1AJ11))
                    {
                        if (AreaPrelievo.Response.DatiGenerici != null && !String.IsNullOrEmpty(AreaPrelievo.Response.DatiGenerici.T_GP1AB01_V))
                        {
                            GestioneDecodifica.CodiceParticolare codPart = elencoCodiciParticolari.Find(x =>
                                x.TraduzioneSuGp == Utility.StringToNullableChar(istruttoria.T_GP1AJ11) && x.CodCategoria.Trim().ToUpperInvariant() == AreaPrelievo.Response.DatiGenerici.T_GP1AB01_V.Trim().ToUpperInvariant().PadLeft(4, '0'));
                            if (codPart != null)
                                datiIstruttoria.CodiceParticolareSoggettoDerogato = codPart.Id;
                        }
                    }

                    if (categoriaFromHost == "0069")
                    {
                        datiIstruttoria.CodiceEnte = Utility.StringToNullableShort(istruttoria.T_GP1AJ11);
                    }

                    if (istruttoria.T_GP1AP47 != 0)
                        datiIstruttoria.Legge44997 = Utility.StringToNullableByte(istruttoria.T_GP1AP47.ToString());

                    if (!string.IsNullOrEmpty(istruttoria.T_GP1AZ11E_V) && tipoDomanda != GestionePrelievo.TipoDomanda.Ripristino && tipoDomanda != GestionePrelievo.TipoDomanda.Riliquidazione)
                    {
                        if (categorieENPALS.Contains(categoriaFromHost))
                        {
                            datiEnpals.TipoLiquidazione = istruttoria.T_GP1AZ11E_V;
                            datiEnpals.TipoLiquidazioneProvvisoria = istruttoria.T_GP1AZ11F.ToString();
                        }
                        else
                        {
                            List<GestioneDecodifica.DecModalitaLiquidazione> elencoDecModalitaLiquidazione = null;
                            GestioneDecodifica.GetElencoDecModalitaLiquidazione(out elencoDecModalitaLiquidazione);
                            if (elencoDecModalitaLiquidazione != null && elencoDecModalitaLiquidazione.Count > 0)
                            {
                                GestioneDecodifica.DecModalitaLiquidazione modalitaLiquidazione = elencoDecModalitaLiquidazione.Find(x => x.TraduzioneGp.ToString().ToUpperInvariant() == istruttoria.T_GP1AZ11E_V.ToUpperInvariant());
                                if (modalitaLiquidazione != null)
                                    datiIstruttoria.ModalitaLiquidazione = modalitaLiquidazione.ValoreAggPeco;
                            }
                        }
                    }

                    datiIstruttoria.CodiceLiquidazione = Utility.StringToNullableChar(istruttoria.T_TP1COLIQ);
                    if (istruttoria.T_GP1AV72_V != 0)
                        datiIstruttoria.NRiconoscimentiInvalidita = Utility.StringToNullableByte(istruttoria.T_GP1AV72_V.ToString());

                    if (istruttoria.T_GP1CENTCRD_V != 0)
                    {
                        datiIstruttoria.CodiceAziendaEditoria = istruttoria.T_GP1CENTCRD_V;
                        datiIstruttoria.CodiceAziendaEditoriaPerTipo0171 = istruttoria.T_GP1CENTCRD_V;
                        datiIstruttoria.CodiceAziendaEditoriaPerTipo0179 = istruttoria.T_GP1CENTCRD_V;
                        datiIstruttoria.CodiceAziendaEditoriaLetteraB = istruttoria.T_GP1CENTCRD_V;
                    }

                    //ENG - Aggiornamento Memo INPGI
                    if (categoriaFromHost == "0243")
                    {
                        GestioneControlliDinamici.ControlloDinamico ctrlAggiornamentoMemo_INPGI = null;
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20240307", out ctrlAggiornamentoMemo_INPGI);
                        if (ctrlAggiornamentoMemo_INPGI != null && !String.IsNullOrEmpty(ctrlAggiornamentoMemo_INPGI.ValoreControllo) && ctrlAggiornamentoMemo_INPGI.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                        {
                            if (!String.IsNullOrEmpty(istruttoria.T_GP1AJ11) && istruttoria.T_GP1AJ11.Trim() == "1")
                            {
                                datiPensione.GP1AJ11 = "1";
                            }
                        }
                    }
                }

                if (coda != null)
                {

                    if (coda.AreaDati2007 != null)
                    {
                        datiIstruttoria.DecorrenzaCaricoPrecedentePensione = Utility.DataFromInt(coda.AreaDati2007.T_GP1AV56AA, coda.AreaDati2007.T_GP1AV56MM, 1);

                        //Richiesta 20151221 (MAIL Pasquale Cozzolino oggetto: 'FW: LiqPens AGO - Segnalazioni')
                        if (categorieAutonomi.Contains(categoriaFromHost))
                        {
                            datiIstruttoria.NContributiVolontari = coda.AreaDati2007.T_GP2BN03;
                            datiIstruttoria.NContributiVVAnzianita = coda.AreaDati2007.T_GP2BN04;
                        }

                        if (!string.IsNullOrEmpty(coda.AreaDati2007.T_GP1AN87B))
                            datiEnpals.GP1AN87B = coda.AreaDati2007.T_GP1AN87B;
                    }

                    if (coda.AreaDati2010 != null)
                    {
                        if (!String.IsNullOrEmpty(coda.AreaDati2010.T_ESENZESTERO))
                            datiIstruttoria.CodiceComunicazioneCampo4 = coda.AreaDati2010.T_ESENZESTERO.Trim().ToUpperInvariant() == "SI" ? 2 : (byte?)null;
                        if (!String.IsNullOrEmpty(coda.AreaDati2010.T_ESENZVITTIME))
                            datiIstruttoria.CodiceComunicazioneCampo4 = coda.AreaDati2010.T_ESENZVITTIME.Trim().ToUpperInvariant() == "SI" ? 1 : (byte?)null;
                    }

                    if (coda.AreaDati2013 != null)
                    {
                        if (coda.AreaDati2013.T_GP2BL10E == 8 || coda.AreaDati2013.T_GP2BL10E == 11)
                            datiIstruttoria.RiduzioneAssegno = coda.AreaDati2013.T_GP2BL10E;
                    }
                }
            }
        }

        public static void ValorizzaDatiPagamento(Data.GAIN AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestionePagamento.DatiPagamento datiPagamento)
        {
            datiPagamento = null;
            if (AreaPrelievo.Response != null &&
                (AreaPrelievo.Response.Pagamento != null || AreaPrelievo.Response.Coda != null))
            {
                datiPagamento = new GestionePagamento.DatiPagamento();
                Data.CAREPET.Pagamento pagamento = AreaPrelievo.Response.Pagamento;
                Data.CAREPET.Coda coda = AreaPrelievo.Response.Coda;

                if (tipoDomanda != GestionePrelievo.TipoDomanda.Ricostituzione)
                {
                    if (pagamento != null)
                    {
                        datiPagamento.ABI = pagamento.T_GP1CABI_V;
                        datiPagamento.CAB = pagamento.T_GP1CCAB_V;
                        datiPagamento.ModalitaPagamento = GetModalitaPagamento(pagamento, coda);
                        //datiPagamento.Libretto = pagamento.T_GP1CNCC_V;
                    }
                }

                if (coda != null && coda.AreaDati2007 != null)
                {
                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Ricostituzione)
                    {
                        datiPagamento.BIC = coda.AreaDati2007.T_GP1BIC;
                        datiPagamento.IBAN = coda.AreaDati2007.T_GP1IBAN;

                        if (datiPagamento.ModalitaPagamento.GetValueOrDefault() == 'L' &&
                            datiPagamento.ABI.GetValueOrDefault() == 07601)
                        {
                            if (!(!string.IsNullOrEmpty(coda.AreaDati2007.T_GP1IBAN) &&
                                    coda.AreaDati2007.T_GP1IBAN.Length == 27 &&
                                    coda.AreaDati2007.T_GP1IBAN.StartsWith("IT") &&
                                    coda.AreaDati2007.T_GP1IBAN.Substring(10, 5) == "03384"))
                            {
                                datiPagamento.Libretto = coda.AreaDati2007.T_GP1IBAN;
                                datiPagamento.IBAN = string.Empty;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(coda.AreaDati2007.T_GP1AN87A))
                        datiPagamento.TrattenutaInpdap = coda.AreaDati2007.T_GP1AN87A == "SI" ? true : coda.AreaDati2007.T_GP1AN87A == "NO" ? false : (bool?)null;
                    if (!string.IsNullOrEmpty(coda.AreaDati2007.T_GP1AN87D) && coda.AreaDati2007.T_GP1AN87D.Length >= 6)
                        datiPagamento.DataRinunciaTrattenutaInpdap = Utility.DataFromString((coda.AreaDati2007.T_GP1AN87D.Substring(0, 6) + "01"), Utility.FormatoData.AAAAmmGG);
                }
            }
        }

        public static void ValorizzaDatiFamiliare(Data.GAIN AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, List<string> categorieSpacchettamentoENPALS, string categoriaFromHost,
            out List<Entity.DatiFamiliari> ListaFamiliari)
        {
            ListaFamiliari = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Familiari != null &&
                AreaPrelievo.Response.Familiari.LISTT_GP3.Count > 0)
            {
                byte progressivo = 0;
                ListaFamiliari = new List<Entity.DatiFamiliari>();
                foreach (Data.CAREPET.Familiari.T_GP3 familiare in AreaPrelievo.Response.Familiari.LISTT_GP3)
                {
                    Entity.DatiFamiliari fam = new Entity.DatiFamiliari();
                    fam.Familiare = new GestioneFamiliari.Familiare();
                    fam.ElencoCodMaggFamiliari = new List<GestioneFamiliari.CodMaggFamiliari>();
                    //fam.Familiare.SiglaFamiliare = Utility.StringToNullableChar(familiare.T_GP3CH01);
                    fam.Familiare.Progressivo = (char)progressivo++;
                    fam.Familiare.DataMorte = Utility.DataFromInt(familiare.T_GP3CB12A_V, familiare.T_GP3CB12M_V, familiare.T_GP3CB12G_V);
                    fam.Familiare.ScadenzaRevisioneSanitaria = Utility.DataFromInt(familiare.T_GP3CK20A, familiare.T_GP3CK20M, 1);
                    if (!String.IsNullOrEmpty(familiare.T_GP3FTITPRN))
                        fam.Familiare.FlagTitolare = familiare.T_GP3FTITPRN.Trim().ToUpperInvariant() == "1" ? true : familiare.T_GP3FTITPRN.Trim().ToUpperInvariant() == "0" ? false : (bool?)null;
                    fam.Familiare.TipoComponente = Utility.StringToNullableChar(familiare.T_GP3CB09_V);
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || tipoDomanda == GestionePrelievo.TipoDomanda.Ripristino)
                        fam.Familiare.Confermato = true;
                    if (familiare.LISTT_GP3CK != null && familiare.LISTT_GP3CK.Count > 0)
                    {
                        //fam.Familiare.SiglaFamiliare = Utility.StringToNullableChar(familiare.LISTT_GP3CK[0].T_GP3CH01);
                        //if (Utility.StringToNullableChar(familiare.LISTT_GP3CK[0].T_GP3CH01) == 'C')
                        //{
                        //    if (familiare.LISTT_GP3CK[0].T_GP3CH01B == "U")
                        //        fam.Familiare.TipoUnione = "U";
                        //    else
                        //        fam.Familiare.TipoUnione = "M";
                        //}
                        //else if (Utility.StringToNullableChar(familiare.LISTT_GP3CK[0].T_GP3CH01) == 'R')
                        //{
                        //    if (familiare.LISTT_GP3CK[0].T_GP3CH01B == "U")
                        //        fam.Familiare.TipoUnione = "U";
                        //    else
                        //        fam.Familiare.TipoUnione = string.Empty;
                        //}

                        for (int i = 0; i < familiare.LISTT_GP3CK.Count; i++)
                        {
                            GestioneFamiliari.CodMaggFamiliari codMagg = new GestioneFamiliari.CodMaggFamiliari();
                            codMagg.Decorrenza = Utility.DataFromInt(familiare.LISTT_GP3CK[i].T_GP3CK01A, familiare.LISTT_GP3CK[i].T_GP3CK01M, 1);
                            codMagg.Cessazione = Utility.DataFromInt(familiare.LISTT_GP3CK[i].T_GP3CK02A, familiare.LISTT_GP3CK[i].T_GP3CK02M, 1);
                            codMagg.CodiceMaggiorazione = Utility.StringToNullableByte(familiare.LISTT_GP3CK[i].T_GP3CK04.ToString());
                            codMagg.SiglaFamiliare = Utility.StringToNullableChar(familiare.LISTT_GP3CK[i].T_GP3CH01);
                            if (Utility.StringToNullableChar(familiare.LISTT_GP3CK[i].T_GP3CH01) == 'C')
                            {
                                if (familiare.LISTT_GP3CK[i].T_GP3CH01B == "U")
                                    codMagg.TipoUnione = "U";
                                else
                                    codMagg.TipoUnione = "M";
                            }
                            else if (Utility.StringToNullableChar(familiare.LISTT_GP3CK[i].T_GP3CH01) == 'R')
                            {
                                if (familiare.LISTT_GP3CK[i].T_GP3CH01B == "U")
                                    codMagg.TipoUnione = "U";
                                else
                                    codMagg.TipoUnione = string.Empty;
                            }

                            if (codMagg.Decorrenza.HasValue || codMagg.Cessazione.HasValue)
                                fam.ElencoCodMaggFamiliari.Add(codMagg);
                        }

                        fam.ElencoCodMaggFamiliari.Sort(delegate(GestioneFamiliari.CodMaggFamiliari c1, GestioneFamiliari.CodMaggFamiliari c2) { return c1.Decorrenza.Value.CompareTo(c2.Decorrenza); });
                        if (fam.ElencoCodMaggFamiliari.Count > 0)
                        {
                            fam.Familiare.SiglaFamiliare = fam.ElencoCodMaggFamiliari.Last().SiglaFamiliare;
                            fam.Familiare.TipoUnione = fam.ElencoCodMaggFamiliari.Last().TipoUnione;
                        }
                    }
                    fam.Familiare.CodiceFiscale = familiare.T_GP3CB08;
                    ListaFamiliari.Add(fam);
                }
            }
        }


        //Rivista Gestione del ddlTipoPensione - 21/09/2020
        public static void ValorizzaDatiDanteCausa(Data.GAIN AreaPrelievo, ref List<GestioneDecodifica.StatoEstero> elencoStatiEsteri, List<GestioneDecodifica.TipoCalcolo> listaTipoCalcolo, out DatiAnagDanteCausa datiAnagDanteCausa, out GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            datiAnagDanteCausa = null;
            datiDanteCausa = null;
            short resShort = 0;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.DanteCausa != null)
            {
                datiAnagDanteCausa = new DatiAnagDanteCausa();
                datiDanteCausa = new GestioneDanteCausa.DatiDanteCausa();

                Data.CAREPET.DanteCausa areaDanteCausa = AreaPrelievo.Response.DanteCausa;

                datiAnagDanteCausa.Cognome = areaDanteCausa.T_GP7LC11;
                datiAnagDanteCausa.Nome = areaDanteCausa.T_GP7LC21;
                //caso di alcune ricRev indirette
                if (!String.IsNullOrEmpty(datiAnagDanteCausa.Cognome) && datiAnagDanteCausa.Cognome.Contains('/') &&
                    String.IsNullOrEmpty(datiAnagDanteCausa.Nome))
                {
                    string[] arrSplit = areaDanteCausa.T_GP7LC11.Split('/');
                    if (arrSplit != null && arrSplit.Length > 1)
                    {
                        if (!String.IsNullOrEmpty(arrSplit[0]))
                            datiAnagDanteCausa.Cognome = arrSplit[0];
                        if (!String.IsNullOrEmpty(arrSplit[1]))
                            datiAnagDanteCausa.Nome = arrSplit[1];
                    }
                }

                datiAnagDanteCausa.Sesso = Utility.StringToNullableChar(areaDanteCausa.T_GP7LC31);
                if (areaDanteCausa.T_GP7LC41A != 0 && areaDanteCausa.T_GP7LC41M != 0 &&
                    areaDanteCausa.T_GP7LC41G != 0)
                    datiAnagDanteCausa.DataNascita = Utility.DataFromInt(areaDanteCausa.T_GP7LC41A, areaDanteCausa.T_GP7LC41M, areaDanteCausa.T_GP7LC41G);
                else if (areaDanteCausa.T_GP7LC41A != 0 && (areaDanteCausa.T_GP7LC41M == 0 || areaDanteCausa.T_GP7LC41G == 0))
                {
                    datiAnagDanteCausa.DataNascita = Utility.DataFromInt(areaDanteCausa.T_GP7LC41A, areaDanteCausa.T_GP7LC41M == 0 ? 1 : areaDanteCausa.T_GP7LC41M, areaDanteCausa.T_GP7LC41G == 0 ? 1 : areaDanteCausa.T_GP7LC41G);
                    if (datiAnagDanteCausa.DataNascita.HasValue)
                    {
                        if (areaDanteCausa.T_GP7LC41G == 0)
                            datiAnagDanteCausa.DataNascita = datiAnagDanteCausa.DataNascita.Value.AddSeconds(1);
                        if (areaDanteCausa.T_GP7LC41M == 0)
                            datiAnagDanteCausa.DataNascita = datiAnagDanteCausa.DataNascita.Value.AddMinutes(1);
                    }
                }
                datiAnagDanteCausa.CodiceComuneInps = areaDanteCausa.T_GP7LC51;
                datiAnagDanteCausa.CodiceFiscale = areaDanteCausa.T_GP7LC01;

                if (!string.IsNullOrEmpty(areaDanteCausa.T_GP7LH01))
                {
                    if (elencoStatiEsteri == null)
                        GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteri);

                    List<GestioneDecodifica.StatoEstero> statoEstero;
                    if (!string.IsNullOrEmpty(areaDanteCausa.T_GP7LH01) && areaDanteCausa.T_GP7LH01.Trim() == "I")
                        statoEstero = elencoStatiEsteri.FindAll(x => x.Sigla == "ITA");
                    else
                        statoEstero = elencoStatiEsteri.FindAll(x => x.Sigla == areaDanteCausa.T_GP7LH01);
                    if (statoEstero != null && statoEstero.Count == 1)
                    {
                        datiAnagDanteCausa.Cittadinanza = statoEstero[0].CodCatastale;
                        datiDanteCausa.CittadinanzaByArca = true;
                    }
                }

                if (areaDanteCausa.T_GP7LC03A != 0 && areaDanteCausa.T_GP7LC03M != 0 && areaDanteCausa.T_GP7LC03G != 0)
                    datiDanteCausa.DataMorte = Utility.DataFromInt(areaDanteCausa.T_GP7LC03A, areaDanteCausa.T_GP7LC03M, areaDanteCausa.T_GP7LC03G);
                else if (areaDanteCausa.T_GP7LC03A != 0 && (areaDanteCausa.T_GP7LC03M == 0 || areaDanteCausa.T_GP7LC03G == 0))
                {
                    datiDanteCausa.DataMorte = Utility.DataFromInt(areaDanteCausa.T_GP7LC03A, areaDanteCausa.T_GP7LC03M == 0 ? 1 : areaDanteCausa.T_GP7LC03M, areaDanteCausa.T_GP7LC03G == 0 ? 1 : areaDanteCausa.T_GP7LC03G);
                    if (datiDanteCausa.DataMorte.HasValue)
                    {
                        if (areaDanteCausa.T_GP7LC03G == 0)
                            datiDanteCausa.DataMorte = datiDanteCausa.DataMorte.Value.AddSeconds(1);
                        if (areaDanteCausa.T_GP7LC03M == 0)
                            datiDanteCausa.DataMorte = datiDanteCausa.DataMorte.Value.AddMinutes(1);
                    }
                }
                if (areaDanteCausa.T_GP7LB02 != 0)
                    datiDanteCausa.Sede = areaDanteCausa.T_GP7LB02.ToString().PadLeft(4, '0');
                if (areaDanteCausa.T_GP7LB01 != 0)
                {
                    string siglaCategoria = "";
                    GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(areaDanteCausa.T_GP7LB01.ToString().PadLeft(4, '0'), out siglaCategoria);
                    datiDanteCausa.SiglaCategoria = siglaCategoria;
                }
                if (areaDanteCausa.T_GP7LB03 != 0)
                    datiDanteCausa.Certificato = areaDanteCausa.T_GP7LB03;
                if (areaDanteCausa.T_GP7LC02A != 0 && areaDanteCausa.T_GP7LC02M != 0)
                    datiDanteCausa.DecorrenzaPensione = Utility.DataFromInt(areaDanteCausa.T_GP7LC02A, areaDanteCausa.T_GP7LC02M, 1);
                if (areaDanteCausa.T_GP7LACQA != 0 && areaDanteCausa.T_GP7LACQM != 0)
                    datiDanteCausa.DecorrenzaAltraPensione = Utility.DataFromInt(areaDanteCausa.T_GP7LACQA, areaDanteCausa.T_GP7LACQM, 1);
                if (!string.IsNullOrEmpty(areaDanteCausa.T_GP7LC19) && areaDanteCausa.T_GP7LC19 != 0.ToString())
                {
                    if (listaTipoCalcolo != null && listaTipoCalcolo.Count > 0 && listaTipoCalcolo.Exists(x => x.TraduzioneSuGP == byte.Parse(areaDanteCausa.T_GP7LC19)))
                        datiDanteCausa.CodiceTipoPensione = byte.Parse(listaTipoCalcolo.FirstOrDefault(x => x.TraduzioneSuGP == byte.Parse(areaDanteCausa.T_GP7LC19)).Id);
                }
                if (areaDanteCausa.T_GP7LC29 != 0)
                    datiDanteCausa.CodiceBeneficiLegge = (byte)areaDanteCausa.T_GP7LC29;
                if (areaDanteCausa.T_GP7LC39 != 0)
                    datiDanteCausa.Maggiorazione781Contributi = (byte)areaDanteCausa.T_GP7LC39;
                if (!string.IsNullOrEmpty(areaDanteCausa.T_GP7LCAT))
                {
                    resShort = 0;
                    short.TryParse(areaDanteCausa.T_GP7LCAT, out resShort);
                    datiDanteCausa.CategoriaAltraPensione = resShort != 0 ? resShort.ToString() : areaDanteCausa.T_GP7LCAT.Trim();
                }
                if (areaDanteCausa.T_GP7LCESA != 0 && areaDanteCausa.T_GP7LCESM != 0)
                    datiDanteCausa.CessazioneAltraPensione = Utility.DataFromInt(areaDanteCausa.T_GP7LCESA, areaDanteCausa.T_GP7LCESM, 1);
                if (!string.IsNullOrEmpty(areaDanteCausa.T_GP7LCIM))
                    datiDanteCausa.CodiceImportoAltraPensione = Utility.StringToNullableChar(areaDanteCausa.T_GP7LCIM);
                if (!string.IsNullOrEmpty(areaDanteCausa.T_GP7LCUC))
                    datiDanteCausa.CodiceUCAltraPensione = Utility.StringToNullableChar(areaDanteCausa.T_GP7LCUC);
                if (areaDanteCausa.T_GP7LE01_V != 0M)
                    datiDanteCausa.ImportoPensione311284 = areaDanteCausa.T_GP7LE01_V;
                if (areaDanteCausa.T_GP7LE02_V != 0M)
                    datiDanteCausa.ImportoPensione1185 = areaDanteCausa.T_GP7LE02_V;
                if (areaDanteCausa.T_GP7LE03_V != 0M)
                    datiDanteCausa.ImportoPensione1190 = areaDanteCausa.T_GP7LE03_V;
                if (areaDanteCausa.T_GP7LE04 != 0)
                    datiDanteCausa.NContributiDiretta = areaDanteCausa.T_GP7LE04;
                if (areaDanteCausa.T_GP7LENT != 0)
                    datiDanteCausa.EnteAltraPensione = areaDanteCausa.T_GP7LENT;
                if (!string.IsNullOrEmpty(areaDanteCausa.T_GP7LNPE) && areaDanteCausa.T_GP7LNPE.Length == 3)
                    datiDanteCausa.NaturaPensioneAltraPensione = areaDanteCausa.T_GP7LNPE;
                if (!datiDanteCausa.IsNull())
                    datiDanteCausa.ProvenienzaPensione = (byte)areaDanteCausa.T_GP7LC04;
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null)
            {
                if (AreaPrelievo.Response.Coda.AreaDati2007 != null && AreaPrelievo.Response.Coda.AreaDati2007.LISTT_ELTAB_GP7LC != null && AreaPrelievo.Response.Coda.AreaDati2007.LISTT_ELTAB_GP7LC.Count > 0)
                {
                    List<Data.CAREPET.Coda.Dati2007.T_ELTAB_GP7LC> listaT_ELTAB_GP7LC = AreaPrelievo.Response.Coda.AreaDati2007.LISTT_ELTAB_GP7LC;
                    if (datiAnagDanteCausa == null)
                        datiAnagDanteCausa = new DatiAnagDanteCausa();
                    if (datiDanteCausa == null)
                        datiDanteCausa = new GestioneDanteCausa.DatiDanteCausa();
                    if (!string.IsNullOrEmpty(listaT_ELTAB_GP7LC[0].T_GP7LC61) && listaT_ELTAB_GP7LC[0].T_GP7LC61.Trim() != "EE")
                    {
                        if (elencoStatiEsteri == null)
                            GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteri);

                        GestioneDecodifica.StatoEstero statoEstero = elencoStatiEsteri.Find(x => x.Sigla == listaT_ELTAB_GP7LC[0].T_GP7LC61);
                        if (statoEstero != null)
                        {
                            datiDanteCausa.StatoEEResidenza = statoEstero.CodCatastale;
                            datiDanteCausa.StatoEEResidenzaByArca = true;
                        }
                    }
                    if (listaT_ELTAB_GP7LC[0].T_GP7LC62A != 0 && listaT_ELTAB_GP7LC[0].T_GP7LC62M != 0)
                        datiDanteCausa.DecorrenzaResidenza = Utility.DataFromInt(listaT_ELTAB_GP7LC[0].T_GP7LC62A, listaT_ELTAB_GP7LC[0].T_GP7LC62M, 1);
                }

                if (AreaPrelievo.Response.Coda.AreaDati2012 != null)
                {
                    Data.CAREPET.Coda.Dati2012 areaDati2012 = AreaPrelievo.Response.Coda.AreaDati2012;
                    if (areaDati2012.T_GP7LC42A != 0 && areaDati2012.T_GP7LC42M != 0 && areaDati2012.T_GP7LC42G != 0 && datiAnagDanteCausa != null)
                        datiAnagDanteCausa.DataMatrimonio = Utility.DataFromInt(areaDati2012.T_GP7LC42A, areaDati2012.T_GP7LC42M, areaDati2012.T_GP7LC42G);
                }
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.SPRDSC21 != null)
            {
                Data.CAREPET.SPRDSC21 areaSPRDSC21 = AreaPrelievo.Response.SPRDSC21;

                if (datiDanteCausa == null)
                    datiDanteCausa = new GestioneDanteCausa.DatiDanteCausa();

                if (areaSPRDSC21.T_GP4DAA1 != 0)
                    datiDanteCausa.CategoriaFascicolo = areaSPRDSC21.T_GP4DAA1;
                if (areaSPRDSC21.T_GP4DAA2_1 != 0)
                    datiDanteCausa.SedeFascicolo = areaSPRDSC21.T_GP4DAA2_1;
                if (areaSPRDSC21.T_GP4DAA2_2 != 0)
                    datiDanteCausa.NumeroFascicolo = areaSPRDSC21.T_GP4DAA2_2;
            }
        }

        public static void ValorizzaDatiResidenzeEstere(Data.GAIN AreaPrelievo, List<GestioneDecodifica.StatoEstero> elencoStatiEsteri, out List<GestioneAnagrafica.DatiResidenzaEstero> ListaResidenzeEstere)
        {
            ListaResidenzeEstere = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.ResidenzeEstero != null)
            {
                ListaResidenzeEstere = new List<GestioneAnagrafica.DatiResidenzaEstero>();
                Data.CAREPET.ResidenzeEstero residenzeEstero = AreaPrelievo.Response.ResidenzeEstero;

                if (residenzeEstero.LISTT_GP2BS00 != null && residenzeEstero.LISTT_GP2BS00.Count > 0)
                {
                    foreach (Data.CAREPET.ResidenzeEstero.T_GP2BS00 res in residenzeEstero.LISTT_GP2BS00)
                    {
                        if (res.T_GP2BS01A != 0 && res.T_GP2BS01M != 0 && res.T_GP2BS02 != "000")
                        {
                            GestioneAnagrafica.DatiResidenzaEstero datiRes = new GestioneAnagrafica.DatiResidenzaEstero();
                            datiRes.Decorrenza = Utility.DataFromInt(res.T_GP2BS01A, res.T_GP2BS01M, 1);
                            if (res.T_GP2BS02 == "ITA" || res.T_GP2BS02 == "I")
                                datiRes.CodCatastaleStatoEE = "Z000";
                            else if (res.T_GP2BS02 == "EE")
                                datiRes.CodCatastaleStatoEE = "";
                            else
                            {
                                if (elencoStatiEsteri == null)
                                    GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteri);
                                elencoStatiEsteri = elencoStatiEsteri.FindAll(x => x.Sigla == res.T_GP2BS02).ToList<GestioneDecodifica.StatoEstero>();
                                if (elencoStatiEsteri != null && elencoStatiEsteri.Count > 0)
                                {
                                    if (elencoStatiEsteri.Count == 1)
                                        datiRes.CodCatastaleStatoEE = elencoStatiEsteri[0].CodCatastale;
                                    else
                                    {
                                        bool isUguale = true;
                                        for (int i = elencoStatiEsteri.Count - 1; i > 0; i--)
                                        {
                                            if (elencoStatiEsteri[i].CodCatastale != elencoStatiEsteri[i - 1].CodCatastale)
                                            {
                                                isUguale = false;
                                                break;
                                            }
                                        }
                                        if (isUguale)
                                            datiRes.CodCatastaleStatoEE = elencoStatiEsteri[0].CodCatastale;
                                        else
                                            datiRes.CodCatastaleStatoEE = "";
                                    }
                                }
                                else
                                    datiRes.CodCatastaleStatoEE = "";
                            }
                            ListaResidenzeEstere.Add(datiRes);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiStatiCivili(Data.GAIN AreaPrelievo, out List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCivili)
        {
            ListaStatiCivili = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.StatoCivile != null)
            {
                ListaStatiCivili = new List<GestioneAnagrafica.DatiStatoCivile>();
                Data.CAREPET.StatoCivile statoCivile = AreaPrelievo.Response.StatoCivile;

                if (statoCivile.LISTT_GP2KM7A != null && statoCivile.LISTT_GP2KM7A.Count > 0)
                {
                    foreach (Data.CAREPET.StatoCivile.T_GP2KM7A stCiv in statoCivile.LISTT_GP2KM7A)
                    {
                        if (stCiv.T_GP2KM72A != 0 && stCiv.T_GP2KM72M != 0 && stCiv.T_GP2KM76 != 0.ToString())
                        {
                            GestioneAnagrafica.DatiStatoCivile statoCiv = new GestioneAnagrafica.DatiStatoCivile();
                            statoCiv.Decorrenza = Utility.DataFromInt(stCiv.T_GP2KM72A, stCiv.T_GP2KM72M, 1);
                            statoCiv.Codice = stCiv.T_GP2KM76[0];
                            ListaStatiCivili.Add(statoCiv);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiDelegato(Data.GAIN AreaPrelievo, out DatiDelegato datiDelegato)
        {
            datiDelegato = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Delegato != null)
            {
                datiDelegato = new DatiDelegato();
                Data.CAREPET.Delegato delegato = AreaPrelievo.Response.Delegato;

                datiDelegato.CodiceDelegato = delegato.T_GP1AP01_V;
                datiDelegato.CodiceFiscale = delegato.T_GP1AP26_V;
            }
        }

        public static void ValorizzaDatiTutore(Data.GAIN AreaPrelievo, out DatiTutore datiTutore)
        {
            datiTutore = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Tutore != null)
            {
                datiTutore = new DatiTutore();
                Data.CAREPET.Tutore tutore = AreaPrelievo.Response.Tutore;

                datiTutore.CodiceTutore = tutore.T_GP1AP61_V;
                datiTutore.CodiceFiscale = tutore.T_GP1AP66_V;
                if (!string.IsNullOrEmpty(tutore.T_GP1AP70A) && !string.IsNullOrEmpty(tutore.T_GP1AP70M))
                {
                    string mese = (tutore.T_GP1AP70A == "9999" && tutore.T_GP1AP70M == "99") ? "12" : tutore.T_GP1AP70M.PadLeft(2, '0');
                    datiTutore.CessValAmmSost = Utility.DataFromString(tutore.T_GP1AP70A.PadLeft(4, '0') + mese + "01", Utility.FormatoData.AAAAmmGG);
                }
            }

            if (datiTutore != null && datiTutore.IsNull())
                datiTutore = null;
        }

        public static void ValorizzaDatiCalcoloRetributivo(Data.GAIN AreaPrelievo, List<string> categorieENPALS, GestionePrelievo.TipoRicostituzione tipoRicostituzione, ref GestionePensione.DatiPensione datiPensione,
            out List<GestioneCalcolo.DatiCalcoloRetributivo> ListaCalcoloRetributivo, out GestioneCalcolo.DatiCalcoloRetributivoENPAL CalcoloRetributivoENPALS, out List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> ListaDatiRetributiviINPGI)
        {
            ListaCalcoloRetributivo = null;
            CalcoloRetributivoENPALS = null;
            ListaDatiRetributiviINPGI = null;

            string codiceCategoria = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);
            string siglaCategoria = "";
            GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(codiceCategoria, out siglaCategoria);

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.DatiRetributiviBIS != null)
            {
                //Necessario la valorizzazione della proprietà CodiceTipoQuota per ex-inpdai. 
                //Succeviamente verrà impostato a null per tutte le pensioni diverse da ex-inpdai. 
                List<CtrlDecorrenzaRetrExINPDAI> ctrlExInpdai = null;
                GestioneCtrlDecorrenzaRetrExINPDAI.GetCtrlDecorrenzaRetrExINPDAI(out ctrlExInpdai);
                /////////////////////////////////////////////////////////////////////////////////////

                ListaCalcoloRetributivo = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
                Data.CAREPET.DatiRetributiviBIS calcRetr_bis = AreaPrelievo.Response.DatiRetributiviBIS;

                if (calcRetr_bis.LISTT_GP2BC00_BIS != null && calcRetr_bis.LISTT_GP2BC00_BIS.Count > 0)
                {
                    int meseDecorrenza = 0;
                    Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS objMeseDecorrenza = calcRetr_bis.LISTT_GP2BC00_BIS.Find(x => x.T_GP2BC01M_BIS < 13);
                    if (objMeseDecorrenza != null)
                        meseDecorrenza = objMeseDecorrenza.T_GP2BC01M_BIS;

                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                    GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);

                    List<Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS> listaDatiRetributivi = null;

                    if (Utility.IsDomandaINPDAI(siglaCategoria))
                        listaDatiRetributivi = calcRetr_bis.LISTT_GP2BC00_BIS.FindAll(x => !(!string.IsNullOrEmpty(x.T_GP2BC0C_BIS) && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC0C_BIS)));
                    else
                        listaDatiRetributivi = calcRetr_bis.LISTT_GP2BC00_BIS.FindAll(x => !(!string.IsNullOrEmpty(x.T_GP2BC09_BIS) && x.T_GP2BC09_BIS.Length > 1 && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC09_BIS.Substring(1, 1))));

                    foreach (Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS retr_bis in listaDatiRetributivi)
                    {
                        if (retr_bis.T_GP2BC02_BIS != 0 || retr_bis.T_GP2BC03_BIS != 0M || !string.IsNullOrEmpty(retr_bis.T_GP2BC09_BIS))
                        {
                            if (categorieENPALS.Contains(codiceCategoria))
                            {
                                char? quota = Utility.StringToNullableChar(retr_bis.T_GP2BC0B_BIS);
                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString()))
                                {
                                    if(Utility.IsDomandaINPDAI(siglaCategoria)) GetQuotaByDecorrRetrDAI(retr_bis.T_GP2BC01M_BIS, out quota);
                                    else GetQuotaByDecorrRetr(retr_bis.T_GP2BC01M_BIS, out quota);
                                }

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    if (CalcoloRetributivoENPALS == null)
                                        CalcoloRetributivoENPALS = new GestioneCalcolo.DatiCalcoloRetributivoENPAL();

                                    CalcoloRetributivoENPALS.PeriodiQuotaA = (short)retr_bis.T_GP2BC02_BIS;
                                    CalcoloRetributivoENPALS.RMQuotaA = retr_bis.T_GP2BC03_BIS;
                                    if (retr_bis.T_GP2BC10_BIS != 0 && !(tipoRicostituzione == GestionePrelievo.TipoRicostituzione.MotiviContributivi && datiPensione.TipoCalcolo.GetValueOrDefault() != 26 && datiPensione.TipoCalcolo.GetValueOrDefault() != 27))
                                        CalcoloRetributivoENPALS.GiorniQuotaA707 = (short)retr_bis.T_GP2BC10_BIS;
                                    if (retr_bis.T_GP2BC01A_BIS != 0 && retr_bis.T_GP2BC01M_BIS != 0 && CalcoloRetributivoENPALS != null)
                                        CalcoloRetributivoENPALS.DecorrenzaQuotaA = string.Format("01/{0:00}/{1:0000}", retr_bis.T_GP2BC01M_BIS, retr_bis.T_GP2BC01A_BIS);
                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    if (CalcoloRetributivoENPALS == null)
                                        CalcoloRetributivoENPALS = new GestioneCalcolo.DatiCalcoloRetributivoENPAL();

                                    CalcoloRetributivoENPALS.PeriodiQuotaB = (short)retr_bis.T_GP2BC02_BIS;
                                    CalcoloRetributivoENPALS.RMQuotaB = retr_bis.T_GP2BC03_BIS;
                                    if (retr_bis.T_GP2BC10_BIS != 0 && !(tipoRicostituzione == GestionePrelievo.TipoRicostituzione.MotiviContributivi && datiPensione.TipoCalcolo.GetValueOrDefault() != 26 && datiPensione.TipoCalcolo.GetValueOrDefault() != 27))
                                        CalcoloRetributivoENPALS.GiorniQuotaB707 = (short)retr_bis.T_GP2BC10_BIS;
                                    if (retr_bis.T_GP2BC01A_BIS != 0 && retr_bis.T_GP2BC01M_BIS != 0 && CalcoloRetributivoENPALS != null)
                                        CalcoloRetributivoENPALS.DecorrenzaQuotaB = string.Format("01/{0:00}/{1:0000}", retr_bis.T_GP2BC01M_BIS, retr_bis.T_GP2BC01A_BIS);
                                }
                            }
                            else
                            {
                                GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                                char? quota = Utility.StringToNullableChar(retr_bis.T_GP2BC0B_BIS);
                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString()) ||
                                    (Utility.IsDomandaINPDAI(siglaCategoria) && !new List<char> { 'A', 'B' }.Contains(quota.GetValueOrDefault())))
                                {
                                    if (Utility.IsDomandaINPDAI(siglaCategoria)) GetQuotaByDecorrRetrDAI(retr_bis.T_GP2BC01M_BIS, out quota);
                                    else GetQuotaByDecorrRetr(retr_bis.T_GP2BC01M_BIS, out quota);
                                }
                                datiRetr.QuotePrimeLiquidate = quota;

                                //Necessario per ex-inpdai
                                string codiceTipoQuota;
                                GetCodiceTipoQuotaByDecorrRetr(retr_bis.T_GP2BC01M_BIS, ctrlExInpdai, out codiceTipoQuota);
                                datiRetr.CodiceTipoQuota = codiceTipoQuota;

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    datiRetr.NSettimaneQuotaA = retr_bis.T_GP2BC02_BIS;
                                    datiRetr.RMSQuotaA = retr_bis.T_GP2BC03_BIS;
                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    datiRetr.NSettimaneQuotaB = retr_bis.T_GP2BC02_BIS;
                                    datiRetr.RMSQuotaB = retr_bis.T_GP2BC03_BIS;
                                }

                                if (!string.IsNullOrEmpty(retr_bis.T_GP2BC09_BIS))
                                {
                                    retr_bis.T_GP2BC09_BIS = retr_bis.T_GP2BC09_BIS.Replace("0", " ");

                                    if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == retr_bis.T_GP2BC09_BIS.Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloRetributivo != null)
                                            datiRetr.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                                    }
                                }
                                if (!datiRetr.CodiceGestione.HasValue)
                                    datiRetr.CodiceGestione = GetGestioneFromQuotaDecorrenza(AreaPrelievo, codiceCategoria, meseDecorrenza, quota, elencoCodeGestioneCalcoloRetributivo, retr_bis.T_GP2BC01A_BIS);

                                if (retr_bis.T_GP2BC10_BIS != 0)
                                    datiRetr.NSettimane707 = retr_bis.T_GP2BC10_BIS;

                                if (retr_bis.T_GP2BC01A_BIS != 0)
                                    datiRetr.DecorrenzaOriginariaPensione = Utility.DataFromInt(retr_bis.T_GP2BC01A_BIS, meseDecorrenza, 1);

                                datiRetr.PL_Quotar = retr_bis.T_GP2BC0D_BIS;
                                datiRetr.PL_Quotar707 = retr_bis.T_GP2BC0F_BIS;

                                ListaCalcoloRetributivo.Add(datiRetr);
                            }
                        }
                    }
                }
            }
            else if (AreaPrelievo.Response != null && AreaPrelievo.Response.DatiRetributivi_Contributivi != null)
            {
                //Necessario la valorizzazione della proprietà CodiceTipoQuota per ex-inpdai. 
                //Succeviamente verrà impostato a null per tutte le pensioni diverse da ex-inpdai. 
                List<CtrlDecorrenzaRetrExINPDAI> ctrlExInpdai = null;
                GestioneCtrlDecorrenzaRetrExINPDAI.GetCtrlDecorrenzaRetrExINPDAI(out ctrlExInpdai);
                /////////////////////////////////////////////////////////////////////////////////////

                ListaCalcoloRetributivo = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
                ListaDatiRetributiviINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI>();
                Data.CAREPET.DatiRetributivi_Contributivi calcRetr = AreaPrelievo.Response.DatiRetributivi_Contributivi;

                if (calcRetr.LISTT_GP2BC00 != null && calcRetr.LISTT_GP2BC00.Count > 0)
                {
                    int meseDecorrenza = 0;
                    Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 objMeseDecorrenza = calcRetr.LISTT_GP2BC00.Find(x => x.T_GP2BC01M < 13);
                    if (objMeseDecorrenza != null && objMeseDecorrenza.T_GP2BC01M != 0)
                        meseDecorrenza = objMeseDecorrenza.T_GP2BC01M;
                    else
                        meseDecorrenza = 1;

                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                    GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);

                    List<Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00> listaDatiRetributivi = null;

                    List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGI = null;
                    GestioneDecodifica.GetCodeGestioneQuotaFondoINPGI(out elencoCodeGestioneQuotaFondoINPGI);

                    if (Utility.IsDomandaINPDAI(siglaCategoria))
                        listaDatiRetributivi = calcRetr.LISTT_GP2BC00.FindAll(x => !(!string.IsNullOrEmpty(x.T_GP2BC0C) && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC0C)));
                    else
                        listaDatiRetributivi = calcRetr.LISTT_GP2BC00.FindAll(x => !(!string.IsNullOrEmpty(x.T_GP2BC09) && x.T_GP2BC09.Length > 1 && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC09.Substring(1, 1))));

                    foreach (Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 retr in listaDatiRetributivi)
                    {
                        if (retr.T_GP2BC02 != 0 || retr.T_GP2BC03 != 0M || !string.IsNullOrEmpty(retr.T_GP2BC09))
                        {
                            if (categorieENPALS.Contains(codiceCategoria))
                            {
                                char? quota = Utility.StringToNullableChar(retr.T_GP2BC0B);
                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString()))
                                {
                                    if (Utility.IsDomandaINPDAI(siglaCategoria)) GetQuotaByDecorrRetrDAI(retr.T_GP2BC01M, out quota);
                                    else GetQuotaByDecorrRetr(retr.T_GP2BC01M, out quota);
                                }

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    if (CalcoloRetributivoENPALS == null)
                                        CalcoloRetributivoENPALS = new GestioneCalcolo.DatiCalcoloRetributivoENPAL();

                                    CalcoloRetributivoENPALS.PeriodiQuotaA = (short)retr.T_GP2BC02;
                                    CalcoloRetributivoENPALS.RMQuotaA = retr.T_GP2BC03;
                                    if (retr.T_GP2BC10 != 0 && !(tipoRicostituzione == GestionePrelievo.TipoRicostituzione.MotiviContributivi && datiPensione.TipoCalcolo.GetValueOrDefault() != 26 && datiPensione.TipoCalcolo.GetValueOrDefault() != 27))
                                        CalcoloRetributivoENPALS.GiorniQuotaA707 = (short)retr.T_GP2BC10;
                                    if (retr.T_GP2BC01A != 0 && retr.T_GP2BC01M != 0 && CalcoloRetributivoENPALS != null)
                                        CalcoloRetributivoENPALS.DecorrenzaQuotaA = string.Format("01/{0:00}/{1:0000}", retr.T_GP2BC01M, retr.T_GP2BC01A);
                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    if (CalcoloRetributivoENPALS == null)
                                        CalcoloRetributivoENPALS = new GestioneCalcolo.DatiCalcoloRetributivoENPAL();

                                    CalcoloRetributivoENPALS.PeriodiQuotaB = (short)retr.T_GP2BC02;
                                    CalcoloRetributivoENPALS.RMQuotaB = retr.T_GP2BC03;
                                    if (retr.T_GP2BC10 != 0 && !(tipoRicostituzione == GestionePrelievo.TipoRicostituzione.MotiviContributivi && datiPensione.TipoCalcolo.GetValueOrDefault() != 26 && datiPensione.TipoCalcolo.GetValueOrDefault() != 27))
                                        CalcoloRetributivoENPALS.GiorniQuotaB707 = (short)retr.T_GP2BC10;
                                    if (retr.T_GP2BC01A != 0 && retr.T_GP2BC01M != 0 && CalcoloRetributivoENPALS != null)
                                        CalcoloRetributivoENPALS.DecorrenzaQuotaB = string.Format("01/{0:00}/{1:0000}", retr.T_GP2BC01M, retr.T_GP2BC01A);
                                }
                            }
                            else if (elencoCodeGestioneQuotaFondoINPGI.Exists(x => x.TraduzioneSuGP == retr.T_GP2BC09 && x.TipoQuota == "R"))
                            {
                                GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI datiRetrINPGI = new GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI();

                                datiRetrINPGI.Settimane = retr.T_GP2BC02;
                                datiRetrINPGI.RetribuzioneMediaSettimanale = retr.T_GP2BC03;
                                datiRetrINPGI.ImportoCalcolato = retr.T_GP2BC0D;
                                datiRetrINPGI.CodiceGestione = elencoCodeGestioneQuotaFondoINPGI.Find(x => x.TraduzioneSuGP == retr.T_GP2BC09 && x.TipoQuota == "R").Id;

                                ListaDatiRetributiviINPGI.Add(datiRetrINPGI);
                            }
                            else
                            {
                                GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                                char? quota = Utility.StringToNullableChar(retr.T_GP2BC0B);
                                if (!quota.HasValue || string.IsNullOrEmpty(quota.Value.ToString()) ||
                                    (Utility.IsDomandaINPDAI(siglaCategoria) && !new List<char> { 'A', 'B' }.Contains(quota.GetValueOrDefault())))
                                {
                                    if (Utility.IsDomandaINPDAI(siglaCategoria)) GetQuotaByDecorrRetrDAI(retr.T_GP2BC01M, out quota);
                                    else GetQuotaByDecorrRetr(retr.T_GP2BC01M, out quota);
                                }
                                datiRetr.QuotePrimeLiquidate = quota;

                                //Necessario per ex-inpdai
                                string codiceTipoQuota;
                                GetCodiceTipoQuotaByDecorrRetr(retr.T_GP2BC01M, ctrlExInpdai, out codiceTipoQuota);
                                datiRetr.CodiceTipoQuota = codiceTipoQuota;

                                if (quota.HasValue && quota.Value == 'A')
                                {
                                    datiRetr.NSettimaneQuotaA = retr.T_GP2BC02;
                                    datiRetr.RMSQuotaA = retr.T_GP2BC03;
                                }
                                else if (quota.HasValue && quota.Value == 'B')
                                {
                                    datiRetr.NSettimaneQuotaB = retr.T_GP2BC02;
                                    datiRetr.RMSQuotaB = retr.T_GP2BC03;
                                }

                                if (!string.IsNullOrEmpty(retr.T_GP2BC09))
                                {
                                    if (codiceCategoria == "0014")
                                    {
                                        if (retr.T_GP2BC01M == 98 || retr.T_GP2BC01M == 99)
                                            if (retr.T_GP2BC09.Trim() != "1")
                                                retr.T_GP2BC09 = "7";
                                            else
                                                datiRetr.RMS = retr.T_GP2BC01M;
                                    }

                                    retr.T_GP2BC09 = retr.T_GP2BC09.Replace("0", " ");

                                    if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == retr.T_GP2BC09.Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloRetributivo != null)
                                            datiRetr.CodiceGestione = codeGestioneCalcoloRetributivo.Id;
                                    }
                                }
                                if (!datiRetr.CodiceGestione.HasValue)
                                    datiRetr.CodiceGestione = GetGestioneFromQuotaDecorrenza(AreaPrelievo, codiceCategoria, retr.T_GP2BC01M, quota, elencoCodeGestioneCalcoloRetributivo, retr.T_GP2BC01A);

                                if (retr.T_GP2BC10 != 0)
                                    datiRetr.NSettimane707 = retr.T_GP2BC10;

                                if (retr.T_GP2BC01A != 0)
                                {
                                    if (retr.T_GP2BC01A < 1996 && retr.T_GP2BC01M < 13)
                                        datiRetr.DecorrenzaOriginariaPensione = Utility.DataFromInt(retr.T_GP2BC01A, retr.T_GP2BC01M, 1);
                                    else
                                        datiRetr.DecorrenzaOriginariaPensione = Utility.DataFromInt(retr.T_GP2BC01A, meseDecorrenza, 1);
                                    if (retr.T_GP2BC01M == 88 || retr.T_GP2BC01M == 90)
                                        datiRetr.DecorrenzaOriginariaPensione = datiRetr.DecorrenzaOriginariaPensione.Value.AddSeconds(retr.T_GP2BC01M);
                                }


                                datiRetr.PL_Quotar = retr.T_GP2BC0D;
                                //datiRetr.PL_Quotar707 = retr.T_GP2BC0F;

                                //campi per ante96
                                datiRetr.NSettAnzianitaVV = retr.T_GP2BC08 != 0 ? retr.T_GP2BC08 : (int?)null;
                                datiRetr.NSettimaneExCombattente = retr.T_GP2BC04 != 0 ? retr.T_GP2BC04 : (int?)null;
                                datiRetr.RMSExCombattente = retr.T_GP2BC05 != 0 ? retr.T_GP2BC05 : (decimal?)null;

                                ListaCalcoloRetributivo.Add(datiRetr);
                            }
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiCalcoloContributivo(Data.GAIN AreaPrelievo, List<string> categorieENPALS, GestionePrelievo.TipoRicostituzione tipoRicostituzione, string tipo, ref GestioneEnpals.DatiEnpals datiENPALS, ref GestionePensione.DatiPensione datiPensione, out List<GestioneCalcolo.DatiCalcoloContributivo> ListaCalcoloContributivo,
            out GestioneCalcolo.DatiCalcoloContributivoENPAL CalcoloContributivoENPALS, out List<Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS> ListaSuppRecordENPALS, out List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ListaQuotaFondoIntegrativo, out List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> ListaDatiContributiviINPGI)
        {
            ListaCalcoloContributivo = null;
            CalcoloContributivoENPALS = null;
            ListaSuppRecordENPALS = null;
            ListaQuotaFondoIntegrativo = null;
            ListaDatiContributiviINPGI = null;

            string codiceCategoria = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);
            string siglaCategoria = "";
            GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(codiceCategoria, out siglaCategoria);

            List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGI = null;
            GestioneDecodifica.GetCodeGestioneQuotaFondoINPGI(out elencoCodeGestioneQuotaFondoINPGI);

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null)
            {
                ListaCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                Data.CAREPET.PannelloContributivo calcContr = AreaPrelievo.Response.PannelloContributivo;

                ListaDatiContributiviINPGI = new List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI>();
                ListaQuotaFondoIntegrativo = new List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo>();

                if (calcContr.LISTT_GP2BB03 != null && calcContr.LISTT_GP2BB03.Count > 0)
                {
                    if (codiceCategoria != "0143" && (codiceCategoria != "0199" || calcContr.LISTT_GP2BB03.First().T_GP2BB05 != "E") &&
                        calcContr.LISTT_GP2BB03.First().T_GP2BB05 != "L1" && calcContr.LISTT_GP2BB03.First().T_GP2BB05 != "E")
                    {
                        List<Data.CAREPET.PannelloContributivo.T_GP2BB03> listaDatiContributivi = null;

                        if (Utility.IsDomandaINPDAI(siglaCategoria))
                            listaDatiContributivi = calcContr.LISTT_GP2BB03.FindAll(x => !(!string.IsNullOrEmpty(x.T_GP2BB0C) && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BB0C)));
                        else
                            listaDatiContributivi = calcContr.LISTT_GP2BB03.FindAll(x => !(!string.IsNullOrEmpty(x.T_GP2BB05) && x.T_GP2BB05.Length == 2 && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BB05.Substring(1, 1))));

                        foreach (Data.CAREPET.PannelloContributivo.T_GP2BB03 contr in listaDatiContributivi)
                        {
                            if (contr.T_GP2BB06 != 0M || contr.T_GP2BB07 != 0M || contr.T_GP2BB08 != 0 || !string.IsNullOrEmpty(contr.T_GP2BB05) || contr.T_GP2BB09 != 0M)
                            {
                                if (categorieENPALS.Contains(codiceCategoria))
                                {
                                    if (contr.T_GP2BB05 == "M0")
                                    {
                                        if (datiENPALS == null)
                                            datiENPALS = new GestioneEnpals.DatiEnpals();
                                        datiENPALS.ImportoPensione = contr.T_GP2BB06;
                                        if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                            datiENPALS.DecorrenzaImportoPensione = string.Format("{0:00}/{1:00}/{2:0000}", contr.T_GP2BB04G, contr.T_GP2BB04M, contr.T_GP2BB04A);
                                    }
                                    else if (contr.T_GP2BB05 == "M2")
                                    {
                                        if (datiENPALS == null)
                                            datiENPALS = new GestioneEnpals.DatiEnpals();

                                        //Eng - ImportoPensione707 deve essere valorizzato per le RIC per motivi contributivi e TipoCalcolo != da 26 e 27 con tipo pensione "0169"
                                        if (!(tipoRicostituzione == GestionePrelievo.TipoRicostituzione.MotiviContributivi && datiPensione.TipoCalcolo.GetValueOrDefault() != 26 && datiPensione.TipoCalcolo.GetValueOrDefault() != 27 && tipo != "0169"))
                                            datiENPALS.ImportoPensione707 = contr.T_GP2BB06;
                                        if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                            datiENPALS.DecorrenzaImportoPensione707 = string.Format("{0:00}/{1:00}/{2:0000}", contr.T_GP2BB04G, contr.T_GP2BB04M, contr.T_GP2BB04A);
                                    }
                                    else if (contr.T_GP2BB05 == "M1")
                                    {
                                        if (ListaSuppRecordENPALS == null)
                                            ListaSuppRecordENPALS = new List<Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS>();

                                        Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS datiSuppRecordENPALS = new Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS();

                                        if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                            datiSuppRecordENPALS.Decorrenza = Utility.DataFromInt(contr.T_GP2BB04A, contr.T_GP2BB04M, contr.T_GP2BB04G);
                                        if (contr.T_GP2BB09 != 0M)
                                            datiSuppRecordENPALS.Importo = contr.T_GP2BB09;
                                        datiSuppRecordENPALS.IsFromGP = true;

                                        ListaSuppRecordENPALS.Add(datiSuppRecordENPALS);
                                    }
                                    else if (contr.T_GP2BB05 == "I1")
                                    {
                                        if (datiENPALS == null)
                                            datiENPALS = new GestioneEnpals.DatiEnpals();
                                        datiENPALS.ImportoIIS = contr.T_GP2BB09;
                                        if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                            datiENPALS.DecorrenzaImportoIIS = Utility.DataFromInt(contr.T_GP2BB04A, contr.T_GP2BB04M, contr.T_GP2BB04G);
                                    }
                                    else if (contr.T_GP2BB05 != "S1")// i dati con gestione S1 vengono recuperati nel metodo ValorizzaSentenzaArt4 poichè non fanno parte dei dati calcolo contributivo
                                    {
                                        if (!Utility.IsNullOrWhiteSpace(contr.T_GP2BB05) && contr.T_GP2BB05.Trim() == "1" && contr.T_GP2BB06 == 0 && contr.T_GP2BB07 == 0)
                                            continue;

                                        if (CalcoloContributivoENPALS == null)
                                            CalcoloContributivoENPALS = new GestioneCalcolo.DatiCalcoloContributivoENPAL();

                                        CalcoloContributivoENPALS.Montante = contr.T_GP2BB06;
                                        CalcoloContributivoENPALS.ImportoContributivoTotale = contr.T_GP2BB07;
                                        if (!string.IsNullOrEmpty(contr.T_GP2BB0B))
                                            CalcoloContributivoENPALS.Quota = contr.T_GP2BB0B[0];
                                        else if (!string.IsNullOrEmpty(contr.T_GP2BB0A))
                                        {
                                            if (contr.T_GP2BB0A == "3")
                                                CalcoloContributivoENPALS.Quota = 'C';
                                            else if (contr.T_GP2BB0A == "4")
                                                CalcoloContributivoENPALS.Quota = 'D';
                                        }
                                        else
                                            CalcoloContributivoENPALS.Quota = 'C';
                                        if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                            CalcoloContributivoENPALS.Decorrenza = string.Format("{0:00}/{1:00}/{2:0000}", contr.T_GP2BB04G, contr.T_GP2BB04M, contr.T_GP2BB04A);

                                        if (contr.T_GP2BB08 != 0)
                                            CalcoloContributivoENPALS.NumeroContributiTotale = contr.T_GP2BB08;
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(contr.T_GP2BB05) && contr.T_GP2BB05 == "ES")
                                    {
                                        GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo datiQuota = new GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo();
                                        if ((!(string.IsNullOrEmpty(contr.T_GP2BB0B)) && contr.T_GP2BB0B == "D") || (!(string.IsNullOrEmpty(contr.T_GP2BB0A)) && contr.T_GP2BB0A == "4"))
                                        {
                                            datiQuota.NSettimaneQuotaD = contr.T_GP2BB08;
                                            datiQuota.MontanteQuotaD = contr.T_GP2BB06;
                                            datiQuota.ImportoContribTotaleQuotaD = contr.T_GP2BB07;
                                        }
                                        else
                                        {
                                            datiQuota.NSettimane = contr.T_GP2BB08;
                                            datiQuota.Montante = contr.T_GP2BB06;
                                            datiQuota.ImportoContributivoTotale = contr.T_GP2BB07;
                                        }

                                        if (!string.IsNullOrEmpty(contr.T_GP2BB05))
                                        {
                                            List<GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo> elencoCodeGestioneQuotaFondoIntegrativo = null;
                                            GestioneDecodifica.GetCodeGestioneQuotaFondoIntegrativo(out elencoCodeGestioneQuotaFondoIntegrativo);
                                            if (elencoCodeGestioneQuotaFondoIntegrativo != null && elencoCodeGestioneQuotaFondoIntegrativo.Count > 0)
                                            {
                                                GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo codeGestioneQuotaFondoIntegrativo = elencoCodeGestioneQuotaFondoIntegrativo.Find(x => x.TraduzioneSuGP.Trim() == contr.T_GP2BB05.Trim());
                                                if (codeGestioneQuotaFondoIntegrativo != null)
                                                    datiQuota.CodiceGestione = codeGestioneQuotaFondoIntegrativo.Id;
                                            }
                                        }
                                        datiQuota.PL_Quotac = contr.T_GP2BB0D;

                                        ListaQuotaFondoIntegrativo.Add(datiQuota);
                                    }
                                    else if (elencoCodeGestioneQuotaFondoINPGI.Exists(x => x.TraduzioneSuGP == contr.T_GP2BB05 && x.TipoQuota == "C"))
                                    {
                                        GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI datiContrINPGI = new GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI();

                                        datiContrINPGI.Settimane = contr.T_GP2BB08;
                                        datiContrINPGI.Montante = contr.T_GP2BB06;
                                        datiContrINPGI.Quota = contr.T_GP2BB0D;
                                        datiContrINPGI.CodiceGestione = elencoCodeGestioneQuotaFondoINPGI.Find(x => x.TraduzioneSuGP == contr.T_GP2BB05 && x.TipoQuota == "C").Id;

                                        ListaDatiContributiviINPGI.Add(datiContrINPGI);

                                        //ENG - INPGI migrate
                                        if (contr.T_GP2BB05 == "FA")
                                        {
                                            datiPensione.GP1AV91B = "2";
                                        }
                                    }
                                    else
                                    {
                                        GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();
                                        if ((!(string.IsNullOrEmpty(contr.T_GP2BB0B)) && contr.T_GP2BB0B == "D") || (!(string.IsNullOrEmpty(contr.T_GP2BB0A)) && contr.T_GP2BB0A == "4"))
                                        {
                                            datiContr.NSettimaneQuotaDL214 = contr.T_GP2BB08;
                                            datiContr.MontanteQuotaDL214 = contr.T_GP2BB06;
                                            datiContr.ImportoContribTotaleQuotaDL214 = contr.T_GP2BB07;
                                        }
                                        else if (!string.IsNullOrEmpty(contr.T_GP2BB05) && contr.T_GP2BB05 == "K")
                                        {
                                            if (contr.T_GP2BB08 > 0)
                                                datiContr.NSettimane = contr.T_GP2BB08;
                                            if (contr.T_GP2BB06 > 0)
                                                datiContr.Montante = contr.T_GP2BB06;
                                            if (contr.T_GP2BB07 > 0)
                                                datiContr.ImportoContributivoTotale = contr.T_GP2BB07;
                                        }
                                        else
                                        {
                                            datiContr.NSettimane = contr.T_GP2BB08;
                                            datiContr.Montante = contr.T_GP2BB06;
                                            datiContr.ImportoContributivoTotale = contr.T_GP2BB07;
                                        }
                                        if (!string.IsNullOrEmpty(contr.T_GP2BB05))
                                        {
                                            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                                            GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);
                                            if (elencoCodeGestioneCalcoloContributivo != null && elencoCodeGestioneCalcoloContributivo.Count > 0)
                                            {
                                                GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim() == contr.T_GP2BB05.Trim() && !x.IsFondo);
                                                if (codeGestioneCalcoloContributivo != null)
                                                    datiContr.CodiceGestione = codeGestioneCalcoloContributivo.Id;
                                            }
                                        }

                                        datiContr.PL_Quotac = contr.T_GP2BB0D;

                                        if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                            datiContr.DecorrenzaCalcoloContibutivo = new DateTime(contr.T_GP2BB04A, contr.T_GP2BB04M, contr.T_GP2BB04G);

                                        ListaCalcoloContributivo.Add(datiContr);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiQuotePensione(Data.GAIN AreaPrelievo, out List<GestioneCalcolo.QuotePensione> ListaQuotePensione, out List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> ListaQuoteMiglioramentiContrattuali)
        {
            ListaQuotePensione = null;
            ListaQuoteMiglioramentiContrattuali = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null)
            {
                ListaQuotePensione = new List<GestioneCalcolo.QuotePensione>();
                ListaQuoteMiglioramentiContrattuali = new List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali>();
                Data.CAREPET.PannelloContributivo calcContr = AreaPrelievo.Response.PannelloContributivo;

                if (calcContr.LISTT_GP2BB03 != null && calcContr.LISTT_GP2BB03.Count > 0)
                {
                    foreach (Data.CAREPET.PannelloContributivo.T_GP2BB03 contr in calcContr.LISTT_GP2BB03)
                    {
                        if (contr.T_GP2BB05 == "R1" || contr.T_GP2BB05 == "R2" || contr.T_GP2BB05 == "R3")
                        {
                            GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali datiQuoteMiglioramentiContrattuali = new GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali();
                            datiQuoteMiglioramentiContrattuali.Codice = contr.T_GP2BB05;
                            //sistemare formato
                            datiQuoteMiglioramentiContrattuali.DataDecorrenza = contr.T_GP2BB04G.ToString().PadLeft(2, '0') + "/"+contr.T_GP2BB04M.ToString().PadLeft(2, '0') + "/"+ contr.T_GP2BB04A;
                            datiQuoteMiglioramentiContrattuali.Quota = contr.T_GP2BB06.ToString();
                            ListaQuoteMiglioramentiContrattuali.Add(datiQuoteMiglioramentiContrattuali);
                        }
                        else
                        {
                            if (contr.T_GP2BB06 != 0M || contr.T_GP2BB08 != 0 || !string.IsNullOrEmpty(contr.T_GP2BB05))
                            {
                                GestioneCalcolo.QuotePensione quotePensione = new GestioneCalcolo.QuotePensione();
                                quotePensione.Importo = contr.T_GP2BB06;
                                quotePensione.Settimane = (short)contr.T_GP2BB08;

                                if (!string.IsNullOrEmpty(contr.T_GP2BB05))
                                {
                                    List<GestioneDecodifica.DecEnteGestioneFondo> elencoDecEnteGestioneFondo = null;
                                    GestioneDecodifica.GetDecEnteGestioneFondo(out elencoDecEnteGestioneFondo);
                                    if (elencoDecEnteGestioneFondo != null && elencoDecEnteGestioneFondo.Count > 0)
                                    {
                                        GestioneDecodifica.DecEnteGestioneFondo decEnteGestioneFondo = elencoDecEnteGestioneFondo.Find(x => x.Codice.Trim() == contr.T_GP2BB05.Trim());
                                        if (decEnteGestioneFondo != null)
                                            quotePensione.EnteGestioneFondo = decEnteGestioneFondo.Id;
                                    }
                                }

                                if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                    quotePensione.Decorrenza = Utility.DataFromInt(contr.T_GP2BB04A, contr.T_GP2BB04M, contr.T_GP2BB04G);

                                ListaQuotePensione.Add(quotePensione);
                            }
                        }
                    }
                }
                //se non ha elementi lo rimetto null
                if (ListaQuoteMiglioramentiContrattuali != null && ListaQuoteMiglioramentiContrattuali.Count() == 0) ListaQuoteMiglioramentiContrattuali = null;
            }
        }

        public static void ValorizzaDatiTrattenuteQuotePensione(Data.GAIN AreaPrelievo, out List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenute)
        {
            listaTrattenute = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2014 != null &&
                AreaPrelievo.Response.Coda.AreaDati2014.LISTT_TABTRATTOT != null && AreaPrelievo.Response.Coda.AreaDati2014.LISTT_TABTRATTOT.Count > 0)
            {
                List<Data.CAREPET.Coda.Dati2014.T_TABTRATTOT> listaTabTratt = AreaPrelievo.Response.Coda.AreaDati2014.LISTT_TABTRATTOT;
                listaTrattenute = new List<GestioneCalcolo.TrattenuteQuotePensione>();

                List<GestioneDecodifica.DecEnteGestioneFondo> elencoDecEnteGestioneFondo = null;
                GestioneDecodifica.GetDecEnteGestioneFondo(out elencoDecEnteGestioneFondo);

                foreach (Data.CAREPET.Coda.Dati2014.T_TABTRATTOT tabTratt in listaTabTratt)
                {
                    if (!string.IsNullOrEmpty(tabTratt.T_GESTOT) && tabTratt.LISTT_CONTRIB != null && tabTratt.LISTT_CONTRIB.Count > 0)
                    {
                        if (elencoDecEnteGestioneFondo != null && elencoDecEnteGestioneFondo.Count > 0)
                        {
                            long IdEnteGestioneFondo = elencoDecEnteGestioneFondo.FirstOrDefault(x => x.Codice == tabTratt.T_GESTOT.Trim()).Id;
                            foreach (Data.CAREPET.Coda.Dati2014.T_TABTRATTOT.T_CONTRIB contrib in tabTratt.LISTT_CONTRIB)
                            {
                                if (contrib.T_ANNOTOT != 0 && !string.IsNullOrEmpty(contrib.T_CODTRAT) && contrib.T_TRATTOT != 0)
                                {
                                    GestioneCalcolo.TrattenuteQuotePensione trattenute = new GestioneCalcolo.TrattenuteQuotePensione();
                                    trattenute.EnteGestioneFondoQuote = IdEnteGestioneFondo;
                                    trattenute.AnnoCompetenza = contrib.T_ANNOTOT;
                                    trattenute.CodiceTrattenute = contrib.T_CODTRAT;
                                    trattenute.ImportoTrattenute = contrib.T_TRATTOT;
                                    listaTrattenute.Add(trattenute);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiDetrazioni(Data.GAIN AreaPrelievo, out GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni)
        {
            datiDetrazioni = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Istruttoria != null)
            {
                datiDetrazioni = new GestioneDetrazioniImposta.DatiDetrazioni();
                string detrazioni = AreaPrelievo.Response.Istruttoria.T_GP3CDTI_V.ToString().PadLeft(14, '0');

                datiDetrazioni.DetrazioniReddito = Utility.StringToNullableByte(detrazioni.Substring(0, 1));
                datiDetrazioni.AgevolazionePensionati = Utility.StringToNullableByte(detrazioni.Substring(1, 1));
                datiDetrazioni.ConiugeOFiglio = Utility.StringToNullableByte(detrazioni.Substring(2, 1));
                datiDetrazioni.FigliMinori3AnniNoHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(3, 1));
                datiDetrazioni.FigliMinori3AnniNoHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(4, 1));
                datiDetrazioni.FigliMinori3AnniHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(5, 1));
                datiDetrazioni.FigliMinori3AnniHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(6, 1));
                datiDetrazioni.FigliMaggiori3AnniNoHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(7, 1));
                datiDetrazioni.FigliMaggiori3AnniNoHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(8, 1));
                datiDetrazioni.FigliMaggiori3AnniHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(9, 1));
                datiDetrazioni.FigliMaggiori3AnniHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(10, 1));
                datiDetrazioni.AltriFamiliari100 = Utility.StringToNullableByte(detrazioni.Substring(11, 1));
                datiDetrazioni.AltriFamiliari50 = Utility.StringToNullableByte(detrazioni.Substring(12, 1));
                datiDetrazioni.AddizionaleLombardiaVeneto = Utility.StringToNullableByte(detrazioni.Substring(13, 1));

                datiDetrazioni.DecorrenzaDetrazioneImposte = Utility.DataFromInt(AreaPrelievo.Response.Istruttoria.T_GP3DDTIVRCA_V, AreaPrelievo.Response.Istruttoria.T_GP3DDTIVRCM_V, AreaPrelievo.Response.Istruttoria.T_GP3DDTIVRCG_V);
            }
        }

        public static void ValorizzaDatiSindacato(Data.GAIN AreaPrelievo, out GestionePensione.DatiSindacato datiSindacato)
        {
            datiSindacato = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Istruttoria != null && AreaPrelievo.Response.Istruttoria.LISTT_GP2BG10 != null
                && AreaPrelievo.Response.Istruttoria.LISTT_GP2BG10.Count > 0)
            {
                for (int i = AreaPrelievo.Response.Istruttoria.LISTT_GP2BG10.Count - 1; i >= 0; i--)
                {
                    Data.CAREPET.Istruttoria.T_GP2BG10 sindacato = AreaPrelievo.Response.Istruttoria.LISTT_GP2BG10[i];

                    if (sindacato != null)
                    {
                        if (sindacato.T_GP2BG13A_V == 9999 && sindacato.T_GP2BG13M_V == 99)
                        {
                            datiSindacato = new GestionePensione.DatiSindacato();
                            if (!String.IsNullOrEmpty(sindacato.T_GP2BG11_V))
                                datiSindacato.CodiceSindacato = sindacato.T_GP2BG11_V;
                            datiSindacato.DecorrenzaSindacato = Utility.DataFromInt(sindacato.T_GP2BG12A_V, sindacato.T_GP2BG12M_V, 1);
                            datiSindacato.CessazioneSindacato = Utility.DataFromInt(sindacato.T_GP2BG13A_V, sindacato.T_GP2BG13M_V, 1);
                            break;
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiSupplementi(Data.GAIN AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, List<string> categorieENPALS, List<string> categorieCumulo, List<string> categorieTot,
            GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            out List<Liquidazione.BLCommon.Entity.DatiSupplementi> ListaSupplementi, out List<Liquidazione.BLCommon.Entity.DatiSupplementiENPALS> ListaSupplementiENPALS,
            ref List<Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS> ListaSuppRecordENPALS, ref List<Liquidazione.BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiCumulo,
            GestionePrelievo.TipoRicostituzione tipoRicostituzione, string tipo, string prodotto, out string errore)
        {
            errore = null;
            ListaSupplementi = null;
            ListaSupplementiENPALS = null;
            listaSupplementiCumulo = null;

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);

            string categoriaFromHost = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Supplementi != null)
            {
                ListaSupplementi = new List<Liquidazione.BLCommon.Entity.DatiSupplementi>();
                ListaSupplementiENPALS = new List<Liquidazione.BLCommon.Entity.DatiSupplementiENPALS>();
                listaSupplementiCumulo = new List<Liquidazione.BLCommon.Entity.DatiSupplementiCumulo>();
                Data.CAREPET.Supplementi supplementi = AreaPrelievo.Response.Supplementi;

                if (supplementi.LISTT_GP2BE00 != null && supplementi.LISTT_GP2BE00.Count > 0)
                {
                    List<Data.CAREPET.Supplementi.T_GP2BE00> suppCodGestione2 = supplementi.LISTT_GP2BE00.FindAll(x => x.T_GP2BE02 == "2").ToList();
                    List<Data.CAREPET.Supplementi.T_GP2BE00> suppCodGestione3 = supplementi.LISTT_GP2BE00.FindAll(x => x.T_GP2BE02 == "3").ToList();
                    List<Data.CAREPET.Supplementi.T_GP2BE00> suppCodGestione4 = supplementi.LISTT_GP2BE00.FindAll(x => x.T_GP2BE02 == "4").ToList();

                    foreach (Data.CAREPET.Supplementi.T_GP2BE00 supp in supplementi.LISTT_GP2BE00)
                    {
                        if (categorieENPALS.Contains(categoriaFromHost))
                        {
                            Liquidazione.BLCommon.Entity.DatiSupplementiENPALS datiSuppEnpals = new Liquidazione.BLCommon.Entity.DatiSupplementiENPALS();
                            if (supp.T_GP2BE03 != 0M || supp.T_GP2BE04 != 0M)
                                datiSuppEnpals.TipoSupplemento = 'C';
                            if (supp.T_GP2BE03 != 0M)
                                datiSuppEnpals.Montante = supp.T_GP2BE03;
                            if (supp.T_GP2BE04 != 0M)
                                datiSuppEnpals.ImportoContributivoTotale = supp.T_GP2BE04;
                            if (supp.T_GP2BE07 != 0 && datiSuppEnpals.TipoSupplemento == 'C')
                            {
                                if (supp.T_GP2BE07 == 3)
                                    datiSuppEnpals.Quota = 'C';
                                else if (supp.T_GP2BE07 == 4)
                                    datiSuppEnpals.Quota = 'D';
                            }
                            if (supp.T_GP2BE05 != 0M)
                            {
                                datiSuppEnpals.TipoSupplemento = 'R';
                                datiSuppEnpals.RM = supp.T_GP2BE05;
                            }
                            if (supp.T_GP2BE06 != 0 && datiSuppEnpals.TipoSupplemento == 'R')
                                datiSuppEnpals.Periodi = (short)supp.T_GP2BE06;
                            if (supp.T_GP2BE01A != 0 && supp.T_GP2BE01M != 0)
                            {
                                if (!datiSuppEnpals.Quota.HasValue && datiSuppEnpals.TipoSupplemento.GetValueOrDefault() == 'R')
                                {
                                    if (supp.T_GP2BE01M < 13)
                                        datiSuppEnpals.Quota = 'A';
                                    else if (supp.T_GP2BE01M == 61)
                                        datiSuppEnpals.Quota = 'B';
                                }

                                datiSuppEnpals.Decorrenza = Utility.DataFromInt(supp.T_GP2BE01A, supp.T_GP2BE01M, 1);
                                if (!datiSuppEnpals.Decorrenza.HasValue)
                                {
                                    Data.CAREPET.Supplementi.T_GP2BE00 tempSupp = supplementi.LISTT_GP2BE00.FirstOrDefault(x => x.T_GP2BE01A == supp.T_GP2BE01A && x.T_GP2BE01M < 13);
                                    if (tempSupp != null)
                                        datiSuppEnpals.Decorrenza = Utility.DataFromInt(tempSupp.T_GP2BE01A, tempSupp.T_GP2BE01M, 1);
                                    else
                                    {
                                        Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS datiSuppRecordENPALS = ListaSuppRecordENPALS.Find(x => x.Decorrenza.Value.Year == supp.T_GP2BE01A);
                                        if (datiSuppRecordENPALS != null)
                                            datiSuppEnpals.Decorrenza = datiSuppRecordENPALS.Decorrenza;
                                    }
                                }
                            }
                            if (ListaSuppRecordENPALS != null && ListaSuppRecordENPALS.Count > 0)
                            {
                                Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS datiSuppRecordENPALS = null;
                                if (datiSuppEnpals.Quota == 'B')
                                    datiSuppRecordENPALS = ListaSuppRecordENPALS.Find(x => x.Decorrenza.Value.Year == supp.T_GP2BE01A);
                                else
                                    datiSuppRecordENPALS = ListaSuppRecordENPALS.Find(x => x.Decorrenza == Utility.DataFromInt(supp.T_GP2BE01A, supp.T_GP2BE01M, 1));

                                if (datiSuppRecordENPALS != null)
                                {
                                    if (supp.T_GP2BE11RZA != 0 && supp.T_GP2BE11RZM != 0 && supp.T_GP2BE11RZG != 0)
                                        datiSuppRecordENPALS.InizioSupplemento = Utility.DataFromInt(supp.T_GP2BE11RZA, supp.T_GP2BE11RZM, supp.T_GP2BE11RZG);
                                    if (datiSuppRecordENPALS.InizioSupplemento == DateTime.MinValue)
                                        datiSuppRecordENPALS.InizioSupplemento = null;
                                    if (supp.T_GP2BE12RZA != 0 && supp.T_GP2BE12RZM != 0 && supp.T_GP2BE12RZG != 0)
                                        datiSuppRecordENPALS.FineSupplemento = Utility.DataFromInt(supp.T_GP2BE12RZA, supp.T_GP2BE12RZM, supp.T_GP2BE12RZG);
                                    if (datiSuppRecordENPALS.FineSupplemento == DateTime.MinValue)
                                        datiSuppRecordENPALS.FineSupplemento = null;
                                }
                            }
                            if (!ListaSupplementiENPALS.Exists(x => x.Decorrenza == datiSuppEnpals.Decorrenza && x.Quota == datiSuppEnpals.Quota && x.Importo == datiSuppEnpals.Importo &&
                                x.Periodi == datiSuppEnpals.Periodi && x.RM == datiSuppEnpals.RM))
                                ListaSupplementiENPALS.Add(datiSuppEnpals);
                        }
                        else if (categorieCumulo.Contains(categoriaFromHost) || categorieTot.Contains(categoriaFromHost))
                        {
                            if (supp.T_GP2BE07 != 0 && categorieTot.Contains(categoriaFromHost))
                            {
                                supp.T_GP2BE07 = 0;
                            }

                            //ENG - per RIC prodotto= 0102 tipo= 0184 rimuovere blocco:
                            //ENG - Per tutte le RIC VOCUM (0170) bypassare il controllo
                            //"Attenzione! I dati supplementi presenti sull'archivio centrale sono incongruenti. Verificare i valori dei campi GP2BE07, GP2BE04, GP2BE05"
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (supp.T_GP2BE07 != 0 || supp.T_GP2BE04 != 0 || supp.T_GP2BE05 != 0) &&
                                !(prodotto == "0102" && tipo == "0184") && categoriaFromHost != "0170")
                            {
                                errore = "Attenzione! I dati supplementi presenti sull'archivio centrale sono incongruenti. Verificare i valori dei campi GP2BE07, GP2BE04, GP2BE05";
                                return;
                            }

                            Liquidazione.BLCommon.Entity.DatiSupplementiCumulo datiCumulo = new Liquidazione.BLCommon.Entity.DatiSupplementiCumulo();
                            datiCumulo.Decorrenza = Utility.DataFromInt(supp.T_GP2BE01A, supp.T_GP2BE01M, 1);
                            if (!string.IsNullOrEmpty(supp.T_GP2BE02))
                            {
                                List<GestioneDecodifica.DecEnteGestioneFondo> elencoDecEnteGestioneFondo = null;
                                GestioneDecodifica.GetDecEnteGestioneFondo(out elencoDecEnteGestioneFondo);
                                if (elencoDecEnteGestioneFondo != null && elencoDecEnteGestioneFondo.Count > 0)
                                {
                                    GestioneDecodifica.DecEnteGestioneFondo decEnteGestioneFondo = elencoDecEnteGestioneFondo.Find(x => x.Codice.Trim() == supp.T_GP2BE02.Trim());
                                    if (decEnteGestioneFondo != null)
                                        datiCumulo.EnteGestioneFondo = decEnteGestioneFondo.Id;
                                }
                            }
                            if (supp.T_GP2BE03 != 0M)
                                datiCumulo.Importo = supp.T_GP2BE03;
                            if (supp.T_GP2BE06 != 0)
                                datiCumulo.Settimane = supp.T_GP2BE06;

                            //ENG - RIC VOCUM ('0170')
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && categoriaFromHost == "0170")
                            {
                                if (supp.T_GP2BE07 == 5)
                                {
                                    datiCumulo.AdeguamentoProQuotaCasse = true;
                                }
                            }

                            if (!datiCumulo.IsSupplementiCumuloNull())
                                listaSupplementiCumulo.Add(datiCumulo);
                        }
                        else
                        {
                            INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupp = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
                            //TODO capire come fare con dati pensione che è incompleto
                            //Nota: durante il prelievo vengono marcate come ricostituzioni anche le riaperture (vedi common da cui proviene il dato)
                            //bool IsPannelloSupplementiAnte96 = Utility.IsPannelloSupplementiAnte96(datiPensione, datiPensione, datiDanteCausa, tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione? true : false);
                            if (supp.T_GP2BE05 != 0M)
                            {
                                datiSupp.RMSSupplemento = supp.T_GP2BE05;
                                datiSupp.TipoSupplemento = 'R';
                            }
                            if (supp.T_GP2BE03 != 0M)
                                datiSupp.MontanteSupplemento = supp.T_GP2BE03;
                            if (supp.T_GP2BE04 != 0M)
                                datiSupp.AmmontareContributivo = supp.T_GP2BE04;
                            if ((datiSupp.MontanteSupplemento.HasValue || datiSupp.AmmontareContributivo.HasValue) && !datiSupp.RMSSupplemento.HasValue)
                            {
                                datiSupp.TipoSupplemento = 'C';
                                if (supp.T_GP2BE07 != 0)
                                    datiSupp.CodiceLiquidazione = Convert.ToByte(supp.T_GP2BE07);
                            }
                            datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(supp.T_GP2BE01A, supp.T_GP2BE01M, 1);
                            if (!datiSupp.DecorrenzaSupplemento.HasValue)
                            {
                                Data.CAREPET.Supplementi.T_GP2BE00 tempSupp = supplementi.LISTT_GP2BE00.FirstOrDefault(x => x.T_GP2BE01A == supp.T_GP2BE01A && x.T_GP2BE02 == supp.T_GP2BE02 && x.T_GP2BE01M < 13);
                                if (tempSupp != null)
                                    datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(tempSupp.T_GP2BE01A, tempSupp.T_GP2BE01M, 1);
                                if (!datiSupp.DecorrenzaSupplemento.HasValue && (supp.T_GP2BE02 == "I" || supp.T_GP2BE02 == "M" || supp.T_GP2BE02 == "N"))
                                {
                                    datiSupp.QuotaSupplemento = 'B';
                                    var supplementoTemp = supp.T_GP2BE02 == "I" ? suppCodGestione2.Find(x => x.T_GP2BE01A == supp.T_GP2BE01A) :
                                                                  supp.T_GP2BE02 == "M" ? suppCodGestione3.Find(x => x.T_GP2BE01A == supp.T_GP2BE01A) :
                                                                  supp.T_GP2BE02 == "N" ? suppCodGestione4.Find(x => x.T_GP2BE01A == supp.T_GP2BE01A) :
                                                                  new Data.CAREPET.Supplementi.T_GP2BE00();
                                    datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(supplementoTemp.T_GP2BE01A, supplementoTemp.T_GP2BE01M, 1);
                                }

                                if (categoriaFromHost == "0082" || categoriaFromHost == "0083" || categoriaFromHost == "0084")
                                {
                                    MappingQuotaTipoQuotaINPDAI(supp, ref datiSupp);
                                }
                                //Banc
                                else if (categoriaFromHost == "0032" || categoriaFromHost == "0033" || categoriaFromHost == "0034")
                                {
                                    MappingQuotaTipoQuotaBanc(supp, ref datiSupp);
                                    if (supp.T_GP2BE02 == "H")
                                    {
                                        Data.CAREPET.Supplementi.T_GP2BE00 tempSuppBanc = supplementi.LISTT_GP2BE00.FirstOrDefault(x => x.T_GP2BE01A == supp.T_GP2BE01A && x.T_GP2BE02 == "1" && x.T_GP2BE01M < 13);
                                        if (tempSuppBanc != null)
                                            datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(tempSuppBanc.T_GP2BE01A, tempSuppBanc.T_GP2BE01M, 1);
                                    }
                                }
                                else
                                {
                                    if (datiSupp.DecorrenzaSupplemento.HasValue && datiSupp.TipoSupplemento == 'R')
                                        datiSupp.QuotaSupplemento = 'B';
                                }

                            }
                            else if (datiSupp.TipoSupplemento == 'R')
                            {
                                if (categoriaFromHost == "0082" || categoriaFromHost == "0083" || categoriaFromHost == "0084")
                                    datiSupp.CodTipoQuota = "";
                                datiSupp.QuotaSupplemento = 'A';
                            }
                            if (!string.IsNullOrEmpty(supp.T_GP2BE02))
                                datiSupp.CodGestioneSupplemento = supp.T_GP2BE02;
                            if (supp.T_GP2BE06 != 0)
                                datiSupp.NSettimaneSupplemento = supp.T_GP2BE06;
                            if (datiSupp.TipoSupplemento != null)
                                datiSupp.IsFromPrelievo = true;
                            //ENG - MEMO 50/2023
                            if (!(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" &&
                                tipoRicostituzione == GestionePrelievo.TipoRicostituzione.MotiviContributivi && tipo == "0001" && datiSupp.IsDatiSupplementiNull()))
                                ListaSupplementi.Add(datiSupp);
                        }
                    }
                }
            }
        }

        private static void MappingQuotaTipoQuotaINPDAI(Data.CAREPET.Supplementi.T_GP2BE00 supp, ref INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupp)
        {
            switch (supp.T_GP2BE01M)
            {
                case 16:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "B6";
                    break;
                case 17:
                case 71:
                case 72:
                case 73:
                case 74:
                    datiSupp.QuotaSupplemento = 'A';
                    datiSupp.CodTipoQuota = "";
                    break;
                case 21:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "B1";
                    break;
                case 31:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "B2";
                    break;
                case 41:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "B3";
                    break;
                case 51:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "B4";
                    break;
                case 61:
                case 62:
                case 63:
                case 64:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "B";
                    break;
                case 66:
                case 67:
                case 68:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "";
                    break;
                case 76:
                    datiSupp.QuotaSupplemento = 'A';
                    datiSupp.CodTipoQuota = "A1";
                    break;
                case 91:
                case 92:
                case 93:
                case 94:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "B9";
                    break;
            }

        }

        private static void MappingQuotaTipoQuotaBanc(Data.CAREPET.Supplementi.T_GP2BE00 supp, ref INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupp)
        {
            //Il mapping della quonta per le banc è uguale a quello inpdai con l'aggiunta dei codici sotto
            MappingQuotaTipoQuotaINPDAI(supp, ref datiSupp);

            switch (supp.T_GP2BE01M)
            {
                case 65:
                    datiSupp.QuotaSupplemento = 'B';
                    datiSupp.CodTipoQuota = "";
                    break;
                case 75:
                    datiSupp.QuotaSupplemento = 'A';
                    datiSupp.CodTipoQuota = "";
                    break;
            }

        }

        public static void ValorizzaDatiSupplementiBase(Data.GAIN AreaPrelievo, List<string> categorieENPALS, out INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase datiSupplementiBase)
        {
            datiSupplementiBase = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null && AreaPrelievo.Response.PannelloContributivo.T_GP1AF04 != 0M)
            {
                datiSupplementiBase = new INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase();
                datiSupplementiBase.RenditaFacoltativaOrdinaria = AreaPrelievo.Response.PannelloContributivo.T_GP1AF04;
            }
        }

        public static void ValorizzaDatiIntegrazioneArt11(Data.GAIN AreaPrelievo, out GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11)
        {
            datiIntegrazioneArt11 = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.IntegrazioneArticolo11 != null &&
                AreaPrelievo.Response.IntegrazioneArticolo11.LISTGPINTAR11 != null &&
                AreaPrelievo.Response.IntegrazioneArticolo11.LISTGPINTAR11.Count > 0)
            {
                datiIntegrazioneArt11 = new GestioneIntegrazioneArt11.IntegrazioneArt11();
                datiIntegrazioneArt11.Decorrenza = Utility.DataFromInt(AreaPrelievo.Response.IntegrazioneArticolo11.LISTGPINTAR11[0].T_GP2BC06A, AreaPrelievo.Response.IntegrazioneArticolo11.LISTGPINTAR11[0].T_GP2BC06M, 1);
                if (AreaPrelievo.Response.IntegrazioneArticolo11.LISTGPINTAR11[0].T_GP2BC07 != 0M)
                    datiIntegrazioneArt11.ImportoIVS = AreaPrelievo.Response.IntegrazioneArticolo11.LISTGPINTAR11[0].T_GP2BC07;
            }
        }

        public static void ValorizzaDatiEliminazione(Data.GAIN AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestionePensione.DatiEliminazione datiEliminazione)
        {
            List<GestioneDecodifica.CodiceEliminazione> lstDecCodiceEliminazione;
            GestioneDecodifica.GetCodiceEliminazioneByTipologia(out lstDecCodiceEliminazione, Utility.TipoAppartenenza.AGO);
            string categoriaFromHost = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);
            datiEliminazione = null;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo102", out ctrl);
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Pagamento != null)
            {
                datiEliminazione = new GestionePensione.DatiEliminazione();
                Data.CAREPET.Pagamento pag = AreaPrelievo.Response.Pagamento;
                if (!string.IsNullOrEmpty(pag.T_GP1AM01_V) && pag.T_GP1AM01_V != 0.ToString())
                {
                    GestioneDecodifica.CodiceEliminazione codiceEliminazione = lstDecCodiceEliminazione.Find(x => x.TraduzioneSuGP == pag.T_GP1AM01_V[0]);
                    if (codiceEliminazione != null)
                        datiEliminazione.CodiceMotivo = Utility.StringToNullableByte(codiceEliminazione.Id);
                }

                datiEliminazione.DecorrenzaEliminazione = Utility.DataFromInt(pag.T_GP1AM02A_V, pag.T_GP1AM02M_V, 1);
                datiEliminazione.DataEvento = Utility.DataFromInt(pag.T_GP1AM03A_V, pag.T_GP1AM03M_V, pag.T_GP1AM03G_V);
                if (ctrl != null && ctrl.ValoreControllo == "SI")
                {
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione &&
                       (categoriaFromHost == "0027" || categoriaFromHost == "0028" || categoriaFromHost == "0029" || categoriaFromHost == "0127" || categoriaFromHost == "0128" ||
                        categoriaFromHost == "0196" || categoriaFromHost == "0197" || categoriaFromHost == "0198" || categoriaFromHost == "0199" || categoriaFromHost == "0200" || categoriaFromHost == "0129") &&
                        !string.IsNullOrEmpty(pag.T_GP1AM01_V) && (pag.T_GP1AM01_V == "3" || pag.T_GP1AM01_V == "A") && pag.T_GP1AM02A_V != 0 && pag.T_GP1AM02M_V != 0)
                        datiEliminazione.DataFineCalcoloArretrati = Utility.DataFromInt(pag.T_GP1AM02A_V, pag.T_GP1AM02M_V, 1).Value.AddMonths(-1);
                    else if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione &&
                        (categoriaFromHost == "0027" || categoriaFromHost == "0028" || categoriaFromHost == "0029" || categoriaFromHost == "0043" || categoriaFromHost == "0127"
                        || categoriaFromHost == "0128" || categoriaFromHost == "0129" || categoriaFromHost == "0143" || categoriaFromHost == "0196" || categoriaFromHost == "0197"
                        || categoriaFromHost == "0198" || categoriaFromHost == "0199" || categoriaFromHost == "0200") &&
                        !string.IsNullOrEmpty(pag.T_GP1AM01_V) && pag.T_GP1AM01_V == "1")
                    {
                        datiEliminazione.DataFineCalcoloArretrati = Utility.DataFromInt(pag.T_GP1AM03A_V, pag.T_GP1AM03M_V, 1);
                    }
                    else if (pag.T_GP1AP2A != 0 && pag.T_GP1AP2M != 0)
                        datiEliminazione.DataFineCalcoloArretrati = Utility.DataFromInt(pag.T_GP1AP2A, pag.T_GP1AP2M, 1);
                }
                else
                {
                    if (pag.T_GP1AP2A != 0 && pag.T_GP1AP2M != 0)
                        datiEliminazione.DataFineCalcoloArretrati = Utility.DataFromInt(pag.T_GP1AP2A, pag.T_GP1AP2M, 1);
                }

                datiEliminazione.DataCessazioneDiritto = Utility.DataFromInt(pag.T_GP1AM05A_V, pag.T_GP1AM05M_V, pag.T_GP1AM05G_V);
                datiEliminazione.DataComunicazioneEliminazione = Utility.DataFromInt(pag.T_GP1AM04A_V, pag.T_GP1AM04M_V, 1);
            }
        }

        public static void ValorizzaDatiMaggiorazioni(Data.GAIN AreaPrelievo, ref GestionePensione.DatiPensione datiPensione, out INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            datiMaggiorazioniBenefici = null;
            if (AreaPrelievo.Response != null &&
                (AreaPrelievo.Response.Istruttoria != null ||
                (AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2007 != null) ||
                (AreaPrelievo.Response.Redditi != null && AreaPrelievo.Response.Redditi.RedditiMaggiorazione != null) ||
                AreaPrelievo.Response.DatiGenerici != null) &&
                //Escludo la valorizzazione per le reversibilità
                AreaPrelievo.Request.Controllo.TIPO_RICHIESTA != "51")
            {
                datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();

                if (AreaPrelievo.Response.Istruttoria != null)
                {
                    Data.CAREPET.Istruttoria istruttoria = AreaPrelievo.Response.Istruttoria;

                    if (!string.IsNullOrEmpty(istruttoria.T_GP1AXF3) && istruttoria.T_GP1AXF3.Trim() != string.Empty)
                        datiMaggiorazioniBenefici.Attivitausuranti = istruttoria.T_GP1AXF3 == "1" ? true : false;

                    //if (istruttoria.T_GP1AJ03 != 0)
                    //    datiMaggiorazioniBenefici.CodiceCieco = Utility.StringToNullableByte(istruttoria.T_GP1AJ03.ToString());

                    //datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 = Utility.DataFromInt(istruttoria.T_GP2BN53A, istruttoria.T_GP2BN53M, 1);
                    if (istruttoria.T_GP1AJ03 == 0 && istruttoria.T_GP2BN53A != 0 && istruttoria.T_GP2BN53M != 0)
                    {
                        datiMaggiorazioniBenefici.CodiceCieco = Utility.StringToNullableByte(istruttoria.T_GP1AJ03.ToString());
                    }
                    else if (istruttoria.T_GP1AJ03 != 0)
                        datiMaggiorazioniBenefici.CodiceCieco = Utility.StringToNullableByte(istruttoria.T_GP1AJ03.ToString());

                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 = Utility.DataFromInt(istruttoria.T_GP2BN53A, istruttoria.T_GP2BN53M, 1);

                    if (!string.IsNullOrEmpty(istruttoria.T_GP1AV61) && istruttoria.T_GP1AV61 != "00")
                    {
                        datiMaggiorazioniBenefici.TipoSettimaneBeneficio = istruttoria.T_GP1AV61;
                        // Se T_GP1AV61 ha valore, allora recupero T_GP1NSETBEN se != 0
                        if (istruttoria.T_GP1NSETBEN != 0)
                            datiMaggiorazioniBenefici.NSettimaneBeneficio = istruttoria.T_GP1NSETBEN;
                    }
                    if (istruttoria.T_GP1AXF1 != 0)
                        datiMaggiorazioniBenefici.NSettimaneIncremento1Percento = istruttoria.T_GP1AXF1;
                    if (istruttoria.T_GP1AXF2 != 0)
                        datiMaggiorazioniBenefici.NSettimaneIncremento05Percento = istruttoria.T_GP1AXF2;
                    if (istruttoria.T_TP1SENT != 0)
                        datiMaggiorazioniBenefici.Sentenza495240 = Utility.StringToNullableByte(istruttoria.T_TP1SENT.ToString());
                }

                if (AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2007 != null)
                {
                    Data.CAREPET.Coda.Dati2007 areaDati2007 = AreaPrelievo.Response.Coda.AreaDati2007;
                    datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale = Utility.DataFromInt(areaDati2007.T_GP1AF17AA, areaDati2007.T_GP1AF17MM, 1);
                }

                if (AreaPrelievo.Response.Redditi != null && AreaPrelievo.Response.Redditi.RedditiMaggiorazione != null)
                {
                    Data.CAREPET.Redditi.Maggiorazione maggiorazione = AreaPrelievo.Response.Redditi.RedditiMaggiorazione;
                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale = Utility.DataFromInt(maggiorazione.T_GP1AF07A, maggiorazione.T_GP1AF07M, 1);
                }

                if (AreaPrelievo.Response.DatiGenerici != null)
                {
                    Data.CAREPET.DatiGenerici datiGenerici = AreaPrelievo.Response.DatiGenerici;
                    if (datiGenerici.T_GP1ALA2 != 0)
                        datiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02 = datiGenerici.T_GP1ALA2;
                    else if (datiGenerici.T_GP1ALA3 != 0)
                        datiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02 = datiGenerici.T_GP1ALA3;
                }

                if (datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01")
                {
                    if (AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2008 != null && AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB != null)
                    {
                        Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB eltab = AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBBPAR == 1);
                        if (eltab != null && eltab.T_GP2PBBSET != 0)
                            datiMaggiorazioniBenefici.SettAnzContribPost311295 = eltab.T_GP2PBBSET;
                    }
                }
                else if (datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "02")
                {
                    if (AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2008 != null && AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB != null)
                    {
                        Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB eltab = AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBBPAR == 2);
                        if (eltab != null && eltab.T_GP2PBBSET != 0)
                            datiMaggiorazioniBenefici.NSettIntegrazioneContributivaConcessa = eltab.T_GP2PBBSET;
                    }
                }
                else if (datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "16")
                {
                    if (AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2008 != null && AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB != null)
                    {
                        Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB eltab = AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBBPAR == 16);
                        if (eltab != null && eltab.T_GP2PBBSET != 0)
                            datiMaggiorazioniBenefici.SettAnzContribPost311295 = eltab.T_GP2PBBSET;
                    }
                }
                else if (datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "17")
                {
                    if (AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2008 != null && AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB != null)
                    {
                        Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB eltab = AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB.FirstOrDefault(x => x.T_GP2PBBPAR == 17);
                        if (eltab != null && eltab.T_GP2PBBSET != 0)
                            datiMaggiorazioniBenefici.SettAnzContribPost311295 = eltab.T_GP2PBBSET;
                    }
                }

                if (!datiMaggiorazioniBenefici.IsBeneficiAGONull())
                {
                    if (datiPensione == null)
                        datiPensione = new GestionePensione.DatiPensione();
                    datiPensione.Benefici = true;
                }

                if (!datiMaggiorazioniBenefici.IsExCombattenteAGONull())
                {
                    if (datiPensione == null)
                        datiPensione = new GestionePensione.DatiPensione();
                    datiPensione.ExCombattente = true;
                }

                if (!datiMaggiorazioniBenefici.IsMaggiorazioniAGONull())
                {
                    if (datiPensione == null)
                        datiPensione = new GestionePensione.DatiPensione();
                    datiPensione.Maggiorazioni = true;
                }
            }
        }

        public static void ValorizzaPensioniDatiGenerici(Data.GAIN AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, bool? enteIstruttoreExInpdap, out GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici)
        {
            datiPensioniDatiGenerici = null;

            string categoriaFromHost = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null &&
                AreaPrelievo.Response.Coda.AreaDati2013 != null)
             
            {
                datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                Data.CAREPET.Coda.Dati2013 dati2013 = AreaPrelievo.Response.Coda.AreaDati2013;

                if (!string.IsNullOrEmpty(dati2013.T_GP2PCANT) && dati2013.T_GP2PCANT.Trim() != string.Empty)
                {
                    datiPensioniDatiGenerici.RiduzioneRetributiva = dati2013.T_GP2PCANT == "S" ? true : false;
                    datiPensioniDatiGenerici.RiduzioneRetributivaPercentuale = dati2013.T_GP2PCPER != 0 ? dati2013.T_GP2PCPER : (decimal?)null;
                }

                datiPensioniDatiGenerici.AnzAl95 = (dati2013.T_GP2BH01E != 0) ? dati2013.T_GP2BH01E : (decimal?)null;
                datiPensioniDatiGenerici.QuotaAl95 = (dati2013.T_GP2BL01E != 0) ? dati2013.T_GP2BL01E : (decimal?)null;

                if ((categoriaFromHost == "0170" || categoriaFromHost == "0171" || categoriaFromHost == "0172") &&
                    (!string.IsNullOrEmpty(dati2013.T_GP1AJ10) && dati2013.T_GP1AJ10.Trim() != string.Empty))
                {
                    switch (dati2013.T_GP1AJ10)
                    {
                        case "I":
                            datiPensioniDatiGenerici.TipoCumulo = true;
                            break;
                        case "E":
                            datiPensioniDatiGenerici.TipoCumulo = false;
                            datiPensioniDatiGenerici.CumuloEsterno = 'E';
                            break;
                        case "M":
                            datiPensioniDatiGenerici.TipoCumulo = false;
                            datiPensioniDatiGenerici.CumuloEsterno = 'M';
                            break;
                    }
                }
            }
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Istruttoria != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                Data.CAREPET.Istruttoria istruttoria = AreaPrelievo.Response.Istruttoria;

                if ((categoriaFromHost == "0170" || categoriaFromHost == "0171" || categoriaFromHost == "0172" || categoriaFromHost == "0070" || categoriaFromHost == "0071" || categoriaFromHost == "0072") && istruttoria.T_GP1CENTCRD_V != 0)
                {
                    List<GestioneDecodifica.DecodificaEnteCassaProfessionale> elencoDecodificaEnteCassaProfessionale = null;
                    GestioneDecodifica.GetDecodificaEnteCassaProfessionale(out elencoDecodificaEnteCassaProfessionale);
                    if (elencoDecodificaEnteCassaProfessionale != null && elencoDecodificaEnteCassaProfessionale.Count > 0)
                    {
                        GestioneDecodifica.DecodificaEnteCassaProfessionale decodificaEnteCassaProfessionale = elencoDecodificaEnteCassaProfessionale.Find(x => x.TraduzioneSuGP == istruttoria.T_GP1CENTCRD_V.ToString().PadLeft(4, '0'));
                        if (decodificaEnteCassaProfessionale != null)
                            datiPensioniDatiGenerici.EnteCassa = decodificaEnteCassaProfessionale.Id;
                    }
                }
                //NOTA: Il campo T_GP1AB01_V per VESO33 contiene la scadenzaAssegno anziche scadenzaRevisioneSanitaria
                if ((categoriaFromHost == "0029" || categoriaFromHost == "0129" || categoriaFromHost == "0199" || categoriaFromHost == "0200") && istruttoria.T_GP1AG02G > 0)
                {
                    datiPensioniDatiGenerici.ScadenzaAssegno = Utility.DataFromInt(istruttoria.T_GP1AG02A, istruttoria.T_GP1AG02M, istruttoria.T_GP1AG02G);
                }
                else if (categoriaFromHost == "0198" || categoriaFromHost == "0199" || categoriaFromHost == "0127" || categoriaFromHost == "0128" || categoriaFromHost == "0028" || categoriaFromHost == "0027" ||
                    categoriaFromHost == "0029" || categoriaFromHost == "0143" || categoriaFromHost == "0129" || categoriaFromHost == "0200")
                    datiPensioniDatiGenerici.ScadenzaAssegno = Utility.DataFromInt(istruttoria.T_GP1AF06A_V, istruttoria.T_GP1AF06M_V, 1);

                datiPensioniDatiGenerici.ImportoUltimaRetribuzione = istruttoria.T_GP1AXB8_V != 0 ? istruttoria.T_GP1AXB8_V : (decimal?)null;
            }

            if (categoriaFromHost == "0199" || categoriaFromHost == "0028" || categoriaFromHost == "0128" || categoriaFromHost == "0029" || categoriaFromHost == "0129" || categoriaFromHost == "0200"
                || categoriaFromHost == "0198")
            {
                if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null)
                {
                    if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03 != null && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.Count > 0)
                    {
                        if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05 != null &&
                           (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "E" ||
                            AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "L1"))
                        {
                            if (datiPensioniDatiGenerici == null)
                                datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                            datiPensioniDatiGenerici.ImportoLordoAllaDecorrenza = AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB06;
                        }
                    }
                }
            }
            else if (categoriaFromHost == "0027" || categoriaFromHost == "0127")
            {
                if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null)
                {
                    if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03 != null && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.Count > 0)
                    {
                        if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05 != null &&
                           AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "L")
                        {
                            if (datiPensioniDatiGenerici == null)
                                datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                            datiPensioniDatiGenerici.ImportoLordoAllaDecorrenza = AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB06;
                        }
                    }
                }
            }
            else if (categoriaFromHost == "0143")
            {
                if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03 != null && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.Count > 0)
                {
                    if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05 != null &&
                        AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "M0")
                    {
                        if (datiPensioniDatiGenerici == null)
                            datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                        datiPensioniDatiGenerici.ImportoLordo = AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB06;
                    }
                }

            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2008 != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                Data.CAREPET.Coda.Dati2008 dati2008 = AreaPrelievo.Response.Coda.AreaDati2008;

                datiPensioniDatiGenerici.InizioUltimoLavoro = Utility.DataFromInt(dati2008.T_GP2BM04A, dati2008.T_GP2BM04M, dati2008.T_GP2BM04G);
                datiPensioniDatiGenerici.FineUltimoLavoro = Utility.DataFromInt(dati2008.T_GP2BM05A, dati2008.T_GP2BM05M, dati2008.T_GP2BM05G);
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null &&
                AreaPrelievo.Response.Coda.AreaDati2014 != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                Data.CAREPET.Coda.Dati2014 dati2014 = AreaPrelievo.Response.Coda.AreaDati2014;

                if (categoriaFromHost == "0199" || categoriaFromHost == "0200")
                {
                    if (dati2014.T_GP1AAESO != 0)
                    {
                        datiPensioniDatiGenerici.AnnoBancaFideiussoria = dati2014.T_GP1AAESO;

                        datiPensioniDatiGenerici.ProgressivoBancaFideiussoria = (byte)dati2014.T_GP1PRESO;
                    }
                }
            }

            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Intestazione != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                if (AreaPrelievo.Response.Intestazione.T_GP1AF09Z != 0)
                    datiPensioniDatiGenerici.DataAssunzioneCarico = Utility.DataFromString(AreaPrelievo.Response.Intestazione.T_GP1AF09Z.ToString() + "01", Utility.FormatoData.AAAAmmGG);
            }

            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Sentenze != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                if (AreaPrelievo.Response.Sentenze.T_GP1AXE1_V != 0)
                    datiPensioniDatiGenerici.CodRicalcoloSentenza = Utility.StringToNullableByte(AreaPrelievo.Response.Sentenze.T_GP1AXE1_V.ToString());
            }

            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.DatiGenerici != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                datiPensioniDatiGenerici.ReqArt2DL503 = AreaPrelievo.Response.DatiGenerici.T_GP1AV91M;
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null &&
                AreaPrelievo.Response.Coda.AreaDati2020 != null && AreaPrelievo.Response.Coda.AreaDati2020.T_GP2BB10_UNICO > 0)
            {
                Data.CAREPET.Coda.Dati2020 dati2020 = AreaPrelievo.Response.Coda.AreaDati2020;

                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                datiPensioniDatiGenerici.PL_Coeftrasf = dati2020.T_GP2BB10_UNICO;
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2021 != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                Data.CAREPET.Coda.Dati2021 dati2021 = AreaPrelievo.Response.Coda.AreaDati2021;

                if (!string.IsNullOrEmpty(dati2021.T_GP1AJTIPCUM))
                    datiPensioniDatiGenerici.TipologiaCumulo = Utility.StringToNullableChar(dati2021.T_GP1AJTIPCUM);
            }

            if (categoriaFromHost == "0171" && tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && enteIstruttoreExInpdap.GetValueOrDefault())
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                datiPensioniDatiGenerici.EnteIstruttoreExInpdap = enteIstruttoreExInpdap;
            }

            if (categoriaFromHost == "0030" || categoriaFromHost == "0031" || categoriaFromHost == "0035" || categoriaFromHost == "0036")
            {
                if (AreaPrelievo.Response != null && AreaPrelievo.Response.Invciv != null)
                {
                    if (datiPensioniDatiGenerici == null)
                        datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    datiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria = AreaPrelievo.Response.Invciv.T_GP2BB061_V;
                    datiPensioniDatiGenerici.ImportoMensileAlGennaio2001 = AreaPrelievo.Response.Invciv.T_GP2BB062_V;
                }
            }
            //ENG - MEMO 74_2023
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.NuoviDati2024 != null && AreaPrelievo.Response.NuoviDati2024.AreaDatiGP2BO00 != null)
            {
                if (datiPensioniDatiGenerici == null)
                    datiPensioniDatiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                Data.CAREPET.NuoviDati2024.DatiGP2BO00 dati2024 = AreaPrelievo.Response.NuoviDati2024.AreaDatiGP2BO00;

                datiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 = Convert.ToInt32(dati2024.T_GP2BO05E);
                datiPensioniDatiGenerici.ContribuzioneEsteraTotale = dati2024.T_GP2BO08;
                datiPensioniDatiGenerici.TotaleSettimaneEstereUtiliPerDiritto = dati2024.T_GP2BO09;
                datiPensioniDatiGenerici.CodiceConvenzioneAgo = (byte)dati2024.T_GP2BO01;
            }
        }

        public static void ValorizzaDatiBititolarita(Data.GAIN AreaPrelievo, out List<GestioneAltrePensioni.AltraPensione> ListaBititolarita)
        {
            ListaBititolarita = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Bititolarieta != null)
            {
                ListaBititolarita = new List<GestioneAltrePensioni.AltraPensione>();
                Data.CAREPET.Bititolarieta bititolarieta = AreaPrelievo.Response.Bititolarieta;

                if (bititolarieta.LISTT_GP2A15 != null && bititolarieta.LISTT_GP2A15.Count > 0)
                {
                    foreach (Data.CAREPET.Bititolarieta.T_GP2A15 bit in bititolarieta.LISTT_GP2A15)
                    {
                        if (!string.IsNullOrEmpty(bit.T_GP2CAT))
                        {
                            GestioneAltrePensioni.AltraPensione datiBitit = new GestioneAltrePensioni.AltraPensione();
                            datiBitit.Categoria = bit.T_GP2CAT;
                            datiBitit.Certificato = Utility.StringToNullableInt(bit.T_GP2CER.ToString());
                            datiBitit.Cessazione = Utility.DataFromInt(bit.T_GP2CESA, bit.T_GP2CESM, 1);
                            datiBitit.CodiceUC = Utility.StringToNullableChar(bit.T_GP2CODU);
                            datiBitit.CodiceImporto = Utility.StringToNullableChar(bit.T_GP2CTM);
                            datiBitit.Decorrenza = Utility.DataFromInt(bit.T_GP2DECA, bit.T_GP2DECM, 1);
                            datiBitit.Ente = (bit.T_GP2ENTE != 0 ? Utility.StringToNullableByte(bit.T_GP2ENTE.ToString()) : null);
                            ListaBititolarita.Add(datiBitit);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiInabilitaINAIL(Data.GAIN AreaPrelievo, out List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaInail,
            out GestionePensioneInailInabilita.DatiInabilita datiInabilita)
        {
            listaInail = null;
            datiInabilita = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.INAIL_Accompagnamento != null)
            {
                listaInail = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();
                datiInabilita = new GestionePensioneInailInabilita.DatiInabilita();

                Data.CAREPET.INAIL_Accompagnamento inail_accomp = AreaPrelievo.Response.INAIL_Accompagnamento;

                if (inail_accomp.LISTT_GP2BINA != null && inail_accomp.LISTT_GP2BINA.Count > 0)
                {
                    foreach (Data.CAREPET.INAIL_Accompagnamento.T_GP2BINA iN in inail_accomp.LISTT_GP2BINA)
                    {
                        if (iN.T_GP2BIN1A != 0 || iN.T_GP2BIN2 != 0M || iN.T_GP2BIN3 != 0)
                        {
                            GestionePensioneInailInabilita.DatiPensioniINAIL inail = new GestionePensioneInailInabilita.DatiPensioniINAIL();
                            inail.DecorrenzaRenditaInail = Utility.DataFromInt(iN.T_GP2BIN1A, iN.T_GP2BIN1M, 1);
                            inail.Evento = iN.T_GP2BIN3 == 1 ? true : false;
                            inail.ImportoMensileInail = iN.T_GP2BIN2;
                            listaInail.Add(inail);
                        }
                    }
                }

                datiInabilita.DecorrenzaAssegnoAccompangamento = Utility.DataFromInt(inail_accomp.T_GP2BACCA, inail_accomp.T_GP2BACCM, 1);
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2007 != null)
            {
                if (datiInabilita == null)
                    datiInabilita = new GestionePensioneInailInabilita.DatiInabilita();
                datiInabilita.CessazioneAssegnoAccompangamento = Utility.DataFromInt(AreaPrelievo.Response.Coda.AreaDati2007.T_GP2BACFAA, AreaPrelievo.Response.Coda.AreaDati2007.T_GP2BACFMM, 1);
            }
        }

        public static void ValorizzaDatiOneri(Data.GAIN AreaPrelievo, out List<GestioneOneri.DatiOneri> ListaDatiOneri)
        {
            ListaDatiOneri = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2008 != null &&
                AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB != null && AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB.Count > 0)
            {
                ListaDatiOneri = new List<GestioneOneri.DatiOneri>();
                List<Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB> listaOneri = AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB;

                foreach (Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB on in listaOneri)
                {
                    if (on.T_GP2PBPLEG != 0)
                    {
                        GestioneOneri.DatiOneri datiOneri = new GestioneOneri.DatiOneri();
                        datiOneri.Decorrenza = Utility.DataFromInt(on.T_GP2PBPVARA, on.T_GP2PBPVARM, 1);
                        datiOneri.Scadenza = Utility.DataFromInt(on.T_GP2PBCESA, on.T_GP2PBCESM, on.T_GP2PBCESG);
                        if (datiOneri.Scadenza == DateTime.MinValue)
                            datiOneri.Scadenza = null;

                        List<GestioneDecodifica.GruppoOneri> elencoGruppoOneri = null;
                        GestioneDecodifica.GetGruppoOneri(out elencoGruppoOneri);
                        GestioneDecodifica.GruppoOneri gruppoOneri = elencoGruppoOneri.Find(x => x.Code == on.T_GP2PBPLEG.ToString().PadLeft(4, '0'));
                        if (gruppoOneri != null)
                        {
                            datiOneri.IdCodeGruppo = gruppoOneri.Id;
                        }

                        List<GestioneDecodifica.SottoGruppoOneri> elencoSottoGruppoOneri = null;
                        GestioneDecodifica.GetSottoGruppoOneri(out elencoSottoGruppoOneri);
                        GestioneDecodifica.SottoGruppoOneri sottoGruppoOneri = elencoSottoGruppoOneri.Find(x => x.Code == on.T_GP2PBPLEG1.ToString().PadLeft(4, '0'));
                        if (sottoGruppoOneri != null)
                        {
                            datiOneri.IdCodeSottoGruppo = sottoGruppoOneri.Id;
                        }

                        if (on.T_GP2PBPONR != 0M)
                            datiOneri.Onere = on.T_GP2PBPONR;
                        if (on.T_GP2PBBSET != 0)
                            datiOneri.Settimane = on.T_GP2PBBSET;

                        //Solo per Precoci, Quota 100, Quota 102 e Anticipata Flessibile
                        if ((on.T_GP2PBPLEG == 5000 || on.T_GP2PBPLEG == 5300 || on.T_GP2PBPLEG == 5800 || on.T_GP2PBPLEG == 6000 || on.T_GP2PBPLEG == 6100) && AreaPrelievo.Response != null && AreaPrelievo.Response.Istruttoria != null)
                        {
                            datiOneri.ScadenzaBeneficio = Utility.DataFromInt(AreaPrelievo.Response.Istruttoria.T_GP1AF06A_V, AreaPrelievo.Response.Istruttoria.T_GP1AF06M_V, 1);
                        }

                        ListaDatiOneri.Add(datiOneri);
                    }
                }
            }
        }

        public static void ValorizzaDatiBeneficiParticolari(Data.GAIN AreaPrelievo, out List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaDatiBeneficiParticolari)
        {
            ListaDatiBeneficiParticolari = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2008 != null &&
                AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB != null && AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB.Count > 0)
            {
                ListaDatiBeneficiParticolari = new List<GestioneBeneficiParticolari.DatiBeneficiParticolari>();
                List<Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB> listaOneri = AreaPrelievo.Response.Coda.AreaDati2008.LISTT_ELTAB_GP2PB;

                foreach (Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB on in listaOneri)
                {
                    if (on.T_GP2PBBPAR != 0)
                    {
                        GestioneBeneficiParticolari.DatiBeneficiParticolari datiBeneficiParticolari = new GestioneBeneficiParticolari.DatiBeneficiParticolari();
                        datiBeneficiParticolari.CodiceBenefici = on.T_GP2PBBPAR.ToString();
                        if (on.T_GP2PBPSET != 0)
                            datiBeneficiParticolari.Settimane = on.T_GP2PBPSET;

                        ListaDatiBeneficiParticolari.Add(datiBeneficiParticolari);
                    }
                }
            }
        }

        public static void ValorizzaRedditiSentenza495_93(Data.GAIN AreaPrelievo, out List<GestioneDanteCausa.DatiRedditoSentenza495_93> ListaDatiRedditiSentenza495_93)
        {
            ListaDatiRedditiSentenza495_93 = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Redditi != null && AreaPrelievo.Response.Redditi.RedditiSentenza495_93 != null &&
                AreaPrelievo.Response.Redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z != null && AreaPrelievo.Response.Redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z.Count > 0)
            {
                ListaDatiRedditiSentenza495_93 = new List<GestioneDanteCausa.DatiRedditoSentenza495_93>();
                List<Data.CAREPET.Redditi.Sentenza495_93.T_GP7LKE0Z> listaRedditiSentenza495_93 = AreaPrelievo.Response.Redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z;

                foreach (Data.CAREPET.Redditi.Sentenza495_93.T_GP7LKE0Z redditoSentenza495_93 in listaRedditiSentenza495_93)
                {
                    GestioneDanteCausa.DatiRedditoSentenza495_93 reddito = new GestioneDanteCausa.DatiRedditoSentenza495_93();
                    if (redditoSentenza495_93.T_GP7LKE1 != 0)
                    {
                        reddito.AnnoReddito = redditoSentenza495_93.T_GP7LKE1;
                        if (redditoSentenza495_93.T_GP7LKE1 < 2009)
                        {
                            reddito.RedditoTitolare = redditoSentenza495_93.T_GP7LKE2;
                            reddito.RedditoConiuge = redditoSentenza495_93.T_GP7LKE3;
                        }
                        else
                        {
                            reddito.RedditoTitolare = redditoSentenza495_93.T_GP7LKE2D;
                            reddito.RedditoDaPensioneDC = redditoSentenza495_93.T_GP7LKE2P;
                            reddito.RedditoConiuge = redditoSentenza495_93.T_GP7LKE3D;
                            reddito.RedditoDaPensioneConiuge = redditoSentenza495_93.T_GP7LKE3P;
                        }

                        reddito.CodiceDiReddito = redditoSentenza495_93.T_GP7LKE4A.ToString() + "-" + redditoSentenza495_93.T_GP7LKE4B.ToString() + "-" + redditoSentenza495_93.T_GP7LKE4C.ToString() + "-" + redditoSentenza495_93.T_GP7LKE4D.ToString();

                        if (reddito.RedditoConiuge.HasValue || reddito.RedditoDaPensioneConiuge.HasValue || reddito.RedditoDaPensioneDC.HasValue || reddito.RedditoTitolare.HasValue)
                            ListaDatiRedditiSentenza495_93.Add(reddito);
                    }
                }
            }
        }

        public static void ValorizzaDatiBeneficioVittimeTerrorismo(Data.GAIN AreaPrelievo, out GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, out bool isGP1AC01Valorizzato)
        {
            datiBeneficioVittimeTerrorismo = null;
            isGP1AC01Valorizzato = false;

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Pagamento != null)
            {
                if (datiBeneficioVittimeTerrorismo == null)
                    datiBeneficioVittimeTerrorismo = new GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo();

                if (!string.IsNullOrEmpty(AreaPrelievo.Response.Pagamento.T_GP1AC01_V))
                {
                    isGP1AC01Valorizzato = true;

                    if (AreaPrelievo.Response.Pagamento.T_GP1AC01_V == "4")
                    {
                        datiBeneficioVittimeTerrorismo.TipologiaPrestazione = Utility.StringToNullableInt64(AreaPrelievo.Response.Pagamento.T_GP1AC01_V.Substring(0, 1));
                        datiBeneficioVittimeTerrorismo.CodiceEvento = Utility.StringToNullableChar(AreaPrelievo.Response.Pagamento.T_GP1AC01_V.PadRight(3, ' ').Substring(1, 1));
                        datiBeneficioVittimeTerrorismo.TipologiaBeneficio = Utility.StringToNullableInt64(AreaPrelievo.Response.Pagamento.T_GP1AC01_V.PadRight(3, ' ').Substring(2, 1));
                    }
                    else
                    {
                        datiBeneficioVittimeTerrorismo.TipologiaPrestazione = Utility.StringToNullableInt64(AreaPrelievo.Response.Pagamento.T_GP1AC01_V.Substring(0, 1));
                        datiBeneficioVittimeTerrorismo.CodiceEvento = Utility.StringToNullableChar(AreaPrelievo.Response.Pagamento.T_GP1AC01_V.Substring(1, 1));
                        datiBeneficioVittimeTerrorismo.TipologiaBeneficio = Utility.StringToNullableInt64(AreaPrelievo.Response.Pagamento.T_GP1AC01_V.Substring(2, 1));
                    }
                }
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2009 != null)
            {
                if (datiBeneficioVittimeTerrorismo == null)
                    datiBeneficioVittimeTerrorismo = new GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo();

                string soggettoBeneficiarioTraduzioneSuGP = string.Empty;
                soggettoBeneficiarioTraduzioneSuGP += !string.IsNullOrEmpty(AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AC021) ? AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AC021.PadLeft(1, ' ') : " ";
                soggettoBeneficiarioTraduzioneSuGP += !string.IsNullOrEmpty(AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AC022) ? AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AC022.PadLeft(1, ' ') : " ";
                soggettoBeneficiarioTraduzioneSuGP += !string.IsNullOrEmpty(AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AC023) ? AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AC023.PadLeft(1, ' ') : " ";

                List<GestioneDecodifica.SoggettoBeneficiario> listaDecodificaSoggettoBeneficiario = null;
                GestioneDecodifica.GetDecodificaSoggettoBeneficiario(out listaDecodificaSoggettoBeneficiario);

                if (listaDecodificaSoggettoBeneficiario != null && listaDecodificaSoggettoBeneficiario.Count > 0)
                {
                    GestioneDecodifica.SoggettoBeneficiario soggettoBeneficiario = listaDecodificaSoggettoBeneficiario.Find(x => x.TraduzioneSuGP == soggettoBeneficiarioTraduzioneSuGP);

                    if (soggettoBeneficiario != null)
                        datiBeneficioVittimeTerrorismo.SoggettoBeneficiario = soggettoBeneficiario.Id;
                }

                datiBeneficioVittimeTerrorismo.DataEventoTerroristico = Utility.DataFromInt(AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AP35A, AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AP35M, AreaPrelievo.Response.Coda.AreaDati2009.T_GP1AP35G);
            }
        }

        public static void ValorizzaDatiCalcoloVittimeTerrorismo(Data.GAIN AreaPrelievo, out List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo)
        {
            listaDatiCalcoloVittimeTerrorismo = null;

            string codiceCategoria = GetCodiceCategoriaFromAreaPrelievo(AreaPrelievo);
            string siglaCategoria = "";
            GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(codiceCategoria, out siglaCategoria);

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.DatiRetributiviBIS != null)
            {
                listaDatiCalcoloVittimeTerrorismo = new List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo>();
                Data.CAREPET.DatiRetributiviBIS calcRetr_bis = AreaPrelievo.Response.DatiRetributiviBIS;

                if (calcRetr_bis.LISTT_GP2BC00_BIS != null && calcRetr_bis.LISTT_GP2BC00_BIS.Count > 0)
                {
                    List<Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS> listaDatiRetributiviVittime = null;

                    if (Utility.IsDomandaINPDAI(siglaCategoria))
                        listaDatiRetributiviVittime = calcRetr_bis.LISTT_GP2BC00_BIS.FindAll(x => !string.IsNullOrEmpty(x.T_GP2BC0C_BIS) && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC0C_BIS));
                    else
                        listaDatiRetributiviVittime = calcRetr_bis.LISTT_GP2BC00_BIS.FindAll(x => !string.IsNullOrEmpty(x.T_GP2BC09_BIS) && x.T_GP2BC09_BIS.Length > 1 && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC09_BIS.Substring(1, 1)));

                    foreach (Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS retr_bis in listaDatiRetributiviVittime)
                    {
                        if (retr_bis.T_GP2BC02_BIS != 0 || retr_bis.T_GP2BC03_BIS != 0M || !string.IsNullOrEmpty(retr_bis.T_GP2BC09_BIS))
                        {
                            GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo = new GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo();
                            if (!string.IsNullOrEmpty(retr_bis.T_GP2BC09_BIS))
                            {
                                retr_bis.T_GP2BC09_BIS = retr_bis.T_GP2BC09_BIS.Replace("0", " ");

                                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                                GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);
                                if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                {
                                    string codGestione = null;
                                    if (Utility.IsDomandaINPDAI(siglaCategoria))
                                        codGestione = retr_bis.T_GP2BC09_BIS.Trim();
                                    else
                                        codGestione = retr_bis.T_GP2BC09_BIS.Substring(0, 1).Trim();
                                    GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == codGestione && !x.IsFondo);
                                    if (codeGestioneCalcoloRetributivo != null)
                                        datiCalcoloVittimeTerrorismo.CodiceGestioneRetr = codeGestioneCalcoloRetributivo.Id;
                                }

                                if (!Utility.IsDomandaINPDAI(siglaCategoria))
                                    datiCalcoloVittimeTerrorismo.Beneficio = retr_bis.T_GP2BC09_BIS.Substring(1, 1)[0];
                            }

                            if (retr_bis.T_GP2BC01A_BIS != 0 && retr_bis.T_GP2BC01M_BIS != 0)
                                datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio = Utility.DataFromInt(retr_bis.T_GP2BC01A_BIS, retr_bis.T_GP2BC01M_BIS, 1);

                            if (retr_bis.T_GP2BC0A_BIS == 1)
                                datiCalcoloVittimeTerrorismo.Quota = 'A';
                            else if (retr_bis.T_GP2BC0A_BIS == 2)
                                datiCalcoloVittimeTerrorismo.Quota = 'B';

                            if (retr_bis.T_GP2BC03_BIS != 0M)
                                datiCalcoloVittimeTerrorismo.RMS = retr_bis.T_GP2BC03_BIS;

                            if (retr_bis.T_GP2BC02_BIS != 0)
                                datiCalcoloVittimeTerrorismo.Settimane = retr_bis.T_GP2BC02_BIS;

                            if (Utility.IsDomandaINPDAI(siglaCategoria))
                                datiCalcoloVittimeTerrorismo.Beneficio = !String.IsNullOrEmpty(retr_bis.T_GP2BC0C_BIS) ? Convert.ToChar(retr_bis.T_GP2BC0C_BIS) : (char?)null;

                            datiCalcoloVittimeTerrorismo.Tipo = 'R';

                            listaDatiCalcoloVittimeTerrorismo.Add(datiCalcoloVittimeTerrorismo);
                        }
                    }
                }
            }
            else if (AreaPrelievo.Response != null && AreaPrelievo.Response.DatiRetributivi_Contributivi != null)
            {
                listaDatiCalcoloVittimeTerrorismo = new List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo>();
                Data.CAREPET.DatiRetributivi_Contributivi calcRetr = AreaPrelievo.Response.DatiRetributivi_Contributivi;

                if (calcRetr.LISTT_GP2BC00 != null && calcRetr.LISTT_GP2BC00.Count > 0)
                {
                    List<Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00> listaDatiRetributiviVittime = null;

                    if (Utility.IsDomandaINPDAI(siglaCategoria))
                        listaDatiRetributiviVittime = calcRetr.LISTT_GP2BC00.FindAll(x => !string.IsNullOrEmpty(x.T_GP2BC0C) && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC0C));
                    else
                        listaDatiRetributiviVittime = calcRetr.LISTT_GP2BC00.FindAll(x => !string.IsNullOrEmpty(x.T_GP2BC09) && x.T_GP2BC09.Length > 1 && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BC09.Substring(1, 1)));

                    foreach (Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 retr in listaDatiRetributiviVittime)
                    {
                        if (retr.T_GP2BC02 != 0 || retr.T_GP2BC03 != 0M || !string.IsNullOrEmpty(retr.T_GP2BC09))
                        {
                            GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo = new GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo();
                            if (!string.IsNullOrEmpty(retr.T_GP2BC09))
                            {
                                retr.T_GP2BC09 = retr.T_GP2BC09.Replace("0", " ");

                                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                                GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);
                                if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                {
                                    string codGestione = null;
                                    if (Utility.IsDomandaINPDAI(siglaCategoria))
                                        codGestione = retr.T_GP2BC09.Trim();
                                    else
                                        codGestione = retr.T_GP2BC09.Substring(0, 1).Trim();
                                    GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP == codGestione && !x.IsFondo);
                                    if (codeGestioneCalcoloRetributivo != null)
                                        datiCalcoloVittimeTerrorismo.CodiceGestioneRetr = codeGestioneCalcoloRetributivo.Id;
                                }

                                if (!Utility.IsDomandaINPDAI(siglaCategoria))
                                    datiCalcoloVittimeTerrorismo.Beneficio = retr.T_GP2BC09.Substring(1, 1)[0];
                            }

                            if (retr.T_GP2BC01A != 0 && retr.T_GP2BC01M != 0)
                                datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio = Utility.DataFromInt(retr.T_GP2BC01A, retr.T_GP2BC01M, 1);

                            if (retr.T_GP2BC0A == 1)
                                datiCalcoloVittimeTerrorismo.Quota = 'A';
                            else if (retr.T_GP2BC0A == 2)
                                datiCalcoloVittimeTerrorismo.Quota = 'B';

                            if (retr.T_GP2BC03 != 0M)
                                datiCalcoloVittimeTerrorismo.RMS = retr.T_GP2BC03;

                            if (retr.T_GP2BC02 != 0)
                                datiCalcoloVittimeTerrorismo.Settimane = retr.T_GP2BC02;

                            if (Utility.IsDomandaINPDAI(siglaCategoria))
                                datiCalcoloVittimeTerrorismo.Beneficio = !String.IsNullOrEmpty(retr.T_GP2BC0C) ? Convert.ToChar(retr.T_GP2BC0C) : (char?)null;

                            datiCalcoloVittimeTerrorismo.Tipo = 'R';

                            listaDatiCalcoloVittimeTerrorismo.Add(datiCalcoloVittimeTerrorismo);
                        }
                    }
                }
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null)
            {
                if (listaDatiCalcoloVittimeTerrorismo == null)
                    listaDatiCalcoloVittimeTerrorismo = new List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo>();
                Data.CAREPET.PannelloContributivo calcContr = AreaPrelievo.Response.PannelloContributivo;

                if (calcContr.LISTT_GP2BB03 != null && calcContr.LISTT_GP2BB03.Count > 0)
                {
                    List<Data.CAREPET.PannelloContributivo.T_GP2BB03> listaDatiContributiviVittime = null;

                    if (Utility.IsDomandaINPDAI(siglaCategoria))
                        listaDatiContributiviVittime = calcContr.LISTT_GP2BB03.FindAll(x => !string.IsNullOrEmpty(x.T_GP2BB0C) && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BB0C));
                    else
                        listaDatiContributiviVittime = calcContr.LISTT_GP2BB03.FindAll(x => !string.IsNullOrEmpty(x.T_GP2BB05) && x.T_GP2BB05.Length == 2 && new List<string> { "X", "Y", "W", "Z" }.Contains(x.T_GP2BB05.Substring(1, 1)));

                    foreach (Data.CAREPET.PannelloContributivo.T_GP2BB03 contr in listaDatiContributiviVittime)
                    {
                        if (contr.T_GP2BB06 != 0M || contr.T_GP2BB07 != 0M || contr.T_GP2BB08 != 0 || !string.IsNullOrEmpty(contr.T_GP2BB05) || contr.T_GP2BB09 != 0M)
                        {
                            GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo = new GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo();
                            if (!string.IsNullOrEmpty(contr.T_GP2BB0A))
                            {
                                if (contr.T_GP2BB0A == "3")
                                    datiCalcoloVittimeTerrorismo.Quota = 'C';
                                else if (contr.T_GP2BB0A == "4")
                                    datiCalcoloVittimeTerrorismo.Quota = 'D';
                                datiCalcoloVittimeTerrorismo.Tipo = 'C';

                                if (contr.T_GP2BB06 != 0M)
                                    datiCalcoloVittimeTerrorismo.Montante = contr.T_GP2BB06;
                            }
                            else
                            {
                                datiCalcoloVittimeTerrorismo.Tipo = 'I';

                                if (contr.T_GP2BB06 != 0M)
                                    datiCalcoloVittimeTerrorismo.ImportoPensione = contr.T_GP2BB06;
                            }
                            if (datiCalcoloVittimeTerrorismo.Tipo == 'C')
                            {
                                if (!string.IsNullOrEmpty(contr.T_GP2BB05))
                                {
                                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                                    GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);
                                    if (elencoCodeGestioneCalcoloContributivo != null && elencoCodeGestioneCalcoloContributivo.Count > 0)
                                    {
                                        string codGestione = null;
                                        if (Utility.IsDomandaINPDAI(siglaCategoria))
                                            codGestione = contr.T_GP2BB05.Trim();
                                        else
                                            codGestione = contr.T_GP2BB05.Substring(0, 1).Trim();
                                        GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim() == codGestione && !x.IsFondo);
                                        if (codeGestioneCalcoloContributivo != null)
                                            datiCalcoloVittimeTerrorismo.CodiceGestioneContr = codeGestioneCalcoloContributivo.Id;
                                    }

                                    if (!Utility.IsDomandaINPDAI(siglaCategoria))
                                        datiCalcoloVittimeTerrorismo.Beneficio = contr.T_GP2BB05.Substring(1, 1)[0];
                                }
                            }
                            else if (datiCalcoloVittimeTerrorismo.Tipo == 'I')
                            {
                                if (!string.IsNullOrEmpty(contr.T_GP2BB05))
                                {
                                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                                    GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);
                                    if (elencoCodeGestioneCalcoloRetributivo != null && elencoCodeGestioneCalcoloRetributivo.Count > 0)
                                    {
                                        GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP.Trim() == contr.T_GP2BB05.Substring(0, 1).Trim() && !x.IsFondo);
                                        if (codeGestioneCalcoloRetributivo != null)
                                            datiCalcoloVittimeTerrorismo.CodiceGestioneRetr = codeGestioneCalcoloRetributivo.Id;
                                    }

                                    if (!Utility.IsDomandaINPDAI(siglaCategoria))
                                        datiCalcoloVittimeTerrorismo.Beneficio = contr.T_GP2BB05.Substring(1, 1)[0];
                                }
                            }

                            if (contr.T_GP2BB04A != 0 && contr.T_GP2BB04M != 0 && contr.T_GP2BB04G != 0)
                                datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio = Utility.DataFromInt(contr.T_GP2BB04A, contr.T_GP2BB04M, contr.T_GP2BB04G);

                            if (contr.T_GP2BB07 != 0M)
                                datiCalcoloVittimeTerrorismo.Ammontare = contr.T_GP2BB07;

                            if (contr.T_GP2BB08 != 0)
                                datiCalcoloVittimeTerrorismo.Settimane = contr.T_GP2BB08;

                            if (Utility.IsDomandaINPDAI(siglaCategoria))
                                datiCalcoloVittimeTerrorismo.Beneficio = !String.IsNullOrEmpty(contr.T_GP2BB0C) ? Convert.ToChar(contr.T_GP2BB0C) : (char?)null;

                            listaDatiCalcoloVittimeTerrorismo.Add(datiCalcoloVittimeTerrorismo);
                        }
                    }
                }
            }
        }

        public static void ValorizzaAventiDiritto_Periodi(Data.GAIN AreaPrelievo, out List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDiritto)
        {
            listaDatiAventiDiritto = null;
            string codiceFiscaleTitolare = string.Empty;
            string codiceNucleoTitolare = string.Empty;

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Pensionato != null)
            {
                codiceFiscaleTitolare = AreaPrelievo.Response.Pensionato.T_GP3CB08T_V;
            }

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.SPRDSC21 != null && AreaPrelievo.Response.SPRDSC21.LISTT_GP4DB00 != null && AreaPrelievo.Response.SPRDSC21.LISTT_GP4DB00.Count > 0)
            {
                listaDatiAventiDiritto = new List<GestioneAventiDiritto.AventeDirittoRecuperato>();
                List<Data.CAREPET.SPRDSC21.T_GP4DB00> listaAventiDiritto = AreaPrelievo.Response.SPRDSC21.LISTT_GP4DB00;

                foreach (Data.CAREPET.SPRDSC21.T_GP4DB00 aventeDirittoHost in listaAventiDiritto)
                {
                    GestioneAventiDiritto.AventeDirittoRecuperato aventeDiritto = new GestioneAventiDiritto.AventeDirittoRecuperato();
                    if (!string.IsNullOrEmpty(aventeDirittoHost.T_GP4DB09))
                        aventeDiritto.CodiceFiscale = aventeDirittoHost.T_GP4DB09;
                    else
                        // Se non è presente il codice fiscale, significa che l'elemento è vuoto e quindi passo al prossimo
                        continue;

                    if (!string.IsNullOrEmpty(aventeDirittoHost.T_GP4KA01))
                        aventeDiritto.CategoriaPensione = aventeDirittoHost.T_GP4KA01;
                    if (!string.IsNullOrEmpty(aventeDirittoHost.T_GP4KA02) && !string.IsNullOrEmpty(aventeDirittoHost.T_GP4KA03))
                    {
                        short sede = 0;
                        short.TryParse(aventeDirittoHost.T_GP4KA02 + aventeDirittoHost.T_GP4KA03, out sede);
                        if (sede != 0)
                            aventeDiritto.SedePensione = sede;
                    }
                    if (!string.IsNullOrEmpty(aventeDirittoHost.T_GP4KA04))
                    {
                        int certificato = 0;
                        int.TryParse(aventeDirittoHost.T_GP4KA04, out certificato);
                        if (certificato != 0)
                            aventeDiritto.CertificatoPensione = certificato;
                    }
                    if (aventeDirittoHost.T_GP4DB13 != 0)
                        aventeDiritto.CSog = aventeDirittoHost.T_GP4DB13;
                    if (aventeDirittoHost.T_GP4DB14 != 0)
                        aventeDiritto.DataMatrimonio = Utility.DataFromString(aventeDirittoHost.T_GP4DB14.ToString(), Utility.FormatoData.GGmmAAAA);
                    if (!string.IsNullOrEmpty(aventeDirittoHost.T_GP4DB15))
                    {
                        aventeDiritto.CodiceNucleo = aventeDirittoHost.T_GP4DB15;

                        if (aventeDiritto.CodiceFiscale == codiceFiscaleTitolare)
                            codiceNucleoTitolare = aventeDiritto.CodiceNucleo;
                    }

                    // Per il titolare setto IsTitolare a true e recupero la scadenza revisione sanitaria
                    if (aventeDiritto.CodiceFiscale == codiceFiscaleTitolare)
                    {
                        aventeDiritto.IsTitolare = true;

                        if (AreaPrelievo.Response != null && AreaPrelievo.Response.Familiari != null && AreaPrelievo.Response.Familiari.LISTT_GP3 != null && AreaPrelievo.Response.Familiari.LISTT_GP3.Count > 0)
                        {
                            Data.CAREPET.Familiari.T_GP3 familiare = AreaPrelievo.Response.Familiari.LISTT_GP3.Find(x => x.T_GP3CB08 == codiceFiscaleTitolare);
                            if (familiare != null)
                                aventeDiritto.ScadenzaRevisioneSanitaria = Utility.DataFromInt(familiare.T_GP3CK20A, familiare.T_GP3CK20M, 1);
                        }
                    }

                    aventeDiritto.PresenzaGP = true;

                    if (aventeDirittoHost.LISTT_GP4DC00 != null && aventeDirittoHost.LISTT_GP4DC00.Count > 0)
                    {
                        aventeDiritto.ListaPeriodi = new List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto>();

                        foreach (var periodoHost in aventeDirittoHost.LISTT_GP4DC00.Select((value, index) => new { value, index }))
                        {
                            GestionePeriodiAventiDiritto.PeriodoAventiDiritto periodo = new GestionePeriodiAventiDiritto.PeriodoAventiDiritto();
                            periodo.IsFromGP = true;
                            if (periodoHost.value.T_GP4DC01 != 0M)
                                periodo.PercSpettante = periodoHost.value.T_GP4DC01;
                            if (periodoHost.value.T_GP4DC02 != 0)
                                periodo.DecorrenzaPeriodo = Utility.DataFromString(periodoHost.value.T_GP4DC02.ToString() + "01", Utility.FormatoData.AAAAmmGG);
                            if (periodoHost.value.T_GP4DC03 != 0)
                                periodo.CessazionePeriodo = Utility.DataFromString(periodoHost.value.T_GP4DC03.ToString() + "01", Utility.FormatoData.AAAAmmGG);
                            if (!string.IsNullOrEmpty(periodoHost.value.T_GP4DC04))
                            {
                                periodo.GradoParentela = periodoHost.value.T_GP4DC04[0];

                                if (periodoHost.index == 0)
                                    //Aggiungo anche in decParentelaDa
                                    aventeDiritto.DecParentelaDA = periodoHost.value.T_GP4DC04[0];

                                if (periodoHost.value.T_GP4DC04 == "CU")
                                    periodo.TipoUnione = "U";
                                else if (periodo.GradoParentela == 'C')
                                {
                                    periodo.TipoUnione = "M";
                                    if (periodoHost.index == 0)
                                        aventeDiritto.TipoUnione = "M";
                                }
                            }
                            if (periodoHost.value.T_GP4DC05 != 0M)
                                periodo.CoeffRiduzione = periodoHost.value.T_GP4DC05;
                            if (periodoHost.value.T_GP4DC07 != 0M)
                                periodo.PercGiudice = periodoHost.value.T_GP4DC07;

                            aventeDiritto.ListaPeriodi.Add(periodo);
                        }
                    }

                    listaDatiAventiDiritto.Add(aventeDiritto);
                }

                if (listaDatiAventiDiritto != null && listaDatiAventiDiritto.Count > 0)
                {
                    listaDatiAventiDiritto.FindAll(x => x.CodiceNucleo == codiceNucleoTitolare).ForEach(x => x.NucleoTitolare = true);
                    listaDatiAventiDiritto.FindAll(x => x.CodiceNucleo != codiceNucleoTitolare).ForEach(x => x.NucleoTitolare = false);
                }
            }
        }

        public static void ValorizzaSentenzaArt4(Data.GAIN AreaPrelievo, out List<GestioneSentenzaArt4.DatiSentenzaArt4> listaDatiSentenzaArt4)
        {
            listaDatiSentenzaArt4 = null;

            if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03 != null && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.Count > 0)
            {
                listaDatiSentenzaArt4 = new List<GestioneSentenzaArt4.DatiSentenzaArt4>();
                List<Data.CAREPET.PannelloContributivo.T_GP2BB03> listaPannContr = AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03;

                foreach (Data.CAREPET.PannelloContributivo.T_GP2BB03 pc in listaPannContr)
                {
                    if (pc.T_GP2BB05 == "S1")
                    {
                        GestioneSentenzaArt4.DatiSentenzaArt4 datiSentenzaArt4 = new GestioneSentenzaArt4.DatiSentenzaArt4();
                        if (pc.T_GP2BB04A != 0 || pc.T_GP2BB04M != 0 || pc.T_GP2BB09 != 0M)
                            datiSentenzaArt4.DecorrenzaSentenza = Utility.DataFromInt(pc.T_GP2BB04A, pc.T_GP2BB04M, 1);
                        if (pc.T_GP2BB09 != 0)
                            datiSentenzaArt4.ImportoSentenza = pc.T_GP2BB09;

                        datiSentenzaArt4.IsFromGP = true;

                        listaDatiSentenzaArt4.Add(datiSentenzaArt4);
                    }
                }
            }
        }

        public static void ValorizzaSentenze(Data.GAIN AreaPrelievo, out List<GestioneSentenze.DatiSentenze> listaDatiSentenze)
        {
            listaDatiSentenze = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.Sentenze != null && AreaPrelievo.Response.Sentenze.LISTT_GP2SEN0 != null && AreaPrelievo.Response.Sentenze.LISTT_GP2SEN0.Count > 0)
            {
                listaDatiSentenze = new List<GestioneSentenze.DatiSentenze>();
                List<Data.CAREPET.Sentenze.T_GP2SEN0> listaSentenze = AreaPrelievo.Response.Sentenze.LISTT_GP2SEN0;

                foreach (Data.CAREPET.Sentenze.T_GP2SEN0 sen in listaSentenze)
                {
                    if (!Utility.IsNullOrWhiteSpace(sen.T_GP2SEN1) || !Utility.IsNullOrWhiteSpace(sen.T_GP2SEN2) ||
                        sen.T_GP2SEN3A != 0 || sen.T_GP2SEN3M != 0 || sen.T_GP2SEN4A != 0 || sen.T_GP2SEN4M != 0)
                    {
                        GestioneSentenze.DatiSentenze datiSentenze = new GestioneSentenze.DatiSentenze();
                        datiSentenze.CodSentenzaMerito = sen.T_GP2SEN1;
                        datiSentenze.CodSentenza = sen.T_GP2SEN2;
                        if (sen.T_GP2SEN3M != 0 && sen.T_GP2SEN3A != 0)
                            datiSentenze.DecorrenzaDal = Utility.DataFromInt(sen.T_GP2SEN3A, sen.T_GP2SEN3M, 1);
                        if (sen.T_GP2SEN4M != 0 && sen.T_GP2SEN4A != 0)
                            datiSentenze.DecorrenzaAl = Utility.DataFromInt(sen.T_GP2SEN4A, sen.T_GP2SEN4M, 1);
                        listaDatiSentenze.Add(datiSentenze);
                    }
                }
            }
        }

        //ENG - MEMO 74_2023 
        public static void ValorizzaDatiStatiEsteri(Data.GAIN AreaPrelievo, out List<GestioneContrib.StatoEsteroCumulo> listaStatiEsteriCumulo)
        {
            listaStatiEsteriCumulo = null;
            if (AreaPrelievo.Response != null && AreaPrelievo.Response.NuoviDati2024 != null && AreaPrelievo.Response.NuoviDati2024.LISTT_GP2BR00 != null && AreaPrelievo.Response.NuoviDati2024.LISTT_GP2BR00.Count() > 0)
            {
                listaStatiEsteriCumulo = new List<GestioneContrib.StatoEsteroCumulo>();
                List<Data.CAREPET.NuoviDati2024.DatiGP2BR00> listaDatiStatiEsteri = AreaPrelievo.Response.NuoviDati2024.LISTT_GP2BR00;

                foreach (Data.CAREPET.NuoviDati2024.DatiGP2BR00 stEst in listaDatiStatiEsteri)
                {
                    if (stEst.T_GP2BR02 != 0)
                    {
                        GestioneContrib.StatoEsteroCumulo statoEstero = new GestioneContrib.StatoEsteroCumulo();
                        statoEstero.PrestazioneEsteraCumulo = new GestioneContrib.PrestazioneEsteraCumulo();

                        if (AreaPrelievo.Response.NuoviDati2024.AreaDatiGP2BO00 != null)
                        {
                            Data.CAREPET.NuoviDati2024.DatiGP2BO00 dati2024 = AreaPrelievo.Response.NuoviDati2024.AreaDatiGP2BO00;
                            statoEstero.PrestazioneEsteraCumulo.CodiceConvenzione = (byte)dati2024.T_GP2BO01;
                        }

                        if (stEst.T_GP2BR02 != 0)
                            statoEstero.PrestazioneEsteraCumulo.CodiceStato = stEst.T_GP2BR02.ToString().PadLeft(2, '0');
                        if (stEst.T_GP2BR03 != 0)
                            statoEstero.PrestazioneEsteraCumulo.CodiceIstituzione = stEst.T_GP2BR03.ToString().PadLeft(4, '0');
                        if (!string.IsNullOrEmpty(stEst.T_GP2BR04))
                            statoEstero.PrestazioneEsteraCumulo.MatricolaEstera = stEst.T_GP2BR04;
                        if (stEst.T_GP2BR05 != 0)
                            statoEstero.PrestazioneEsteraCumulo.SettimaneMisura = stEst.T_GP2BR05;
                        if (stEst.T_GP2BR08 != 0)
                            statoEstero.PrestazioneEsteraCumulo.ContributiDiritto = stEst.T_GP2BR08;


                        if (stEst.LISTT_GP2BR10N != null && stEst.LISTT_GP2BR10N.Count > 0)
                        {
                            statoEstero.ElencoImportiEsteriCumulo = new List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo>();
                            foreach (Data.CAREPET.NuoviDati2024.DatiGP2BR00.T_GP2BR10N imp in stEst.LISTT_GP2BR10N)
                            {
                                if (imp.T_GP2BR14N != 0M)
                                {
                                    GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo importo = new GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo();
                                    importo.DecorrenzaPrestazione = Utility.DataFromInt(imp.T_GP2BR12SA, imp.T_GP2BR12M, 1);
                                    importo.CessazionePrestazione = Utility.DataFromInt(imp.T_GP2BR13SA, imp.T_GP2BR13M, 1);
                                    importo.ImportoPrestazione = imp.T_GP2BR14N;
                                    statoEstero.ElencoImportiEsteriCumulo.Add(importo);
                                }
                            }
                        }

                        listaStatiEsteriCumulo.Add(statoEstero);
                    }
                }
            }
        }
        #endregion public members

        #region nested classes
        public class DatiAnagDanteCausa
        {
            #region private properties
            private string _Cognome;
            private string _Nome;
            private System.Nullable<char> _Sesso;
            private System.Nullable<System.DateTime> _DataNascita;
            private int _CodiceComuneInps;
            private System.Nullable<System.DateTime> _DataMatrimonio;
            private string _CodiceFiscale;
            #endregion private properties

            #region public properties
            public int CodiceComuneInps { get { return _CodiceComuneInps; } set { _CodiceComuneInps = value; } }
            public string Cognome { get { return _Cognome; } set { _Cognome = value; } }
            public string Nome { get { return _Nome; } set { _Nome = value; } }
            public System.Nullable<char> Sesso { get { return _Sesso; } set { _Sesso = value; } }
            public System.Nullable<System.DateTime> DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }
            public System.Nullable<System.DateTime> DataMatrimonio { get { return _DataMatrimonio; } set { _DataMatrimonio = value; } }
            public string Cittadinanza { get; set; }
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            #endregion public properties

        }

        public class DatiDelegato
        {
            #region private properties
            private string _CodiceFiscale;
            private string _CodiceDelegato;
            #endregion private properties
            #region public properties
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public string CodiceDelegato { get { return _CodiceDelegato; } set { _CodiceDelegato = value; } }
            #endregion public properties
        }

        public class DatiTutore
        {
            #region private properties
            private string _CodiceFiscale;
            private string _CodiceTutore;
            private DateTime? _CessValAmmSost;
            #endregion private properties
            #region public properties
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public string CodiceTutore { get { return _CodiceTutore; } set { _CodiceTutore = value; } }
            public DateTime? CessValAmmSost { get { return _CessValAmmSost; } set { _CessValAmmSost = value; } }
            #endregion public properties

            #region public methods
            public bool IsNull()
            {
                if (!string.IsNullOrEmpty(_CodiceFiscale) || !string.IsNullOrEmpty(_CodiceTutore) || CessValAmmSost.HasValue)
                    return false;

                return true;
            }
            #endregion public methods
        }
        #endregion nested classes
    }
}
