using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class MappingDaHost
    {
        #region public members
        public static void ValorizzaDatiPensione(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, bool isRiaperturaDomanda, short categoria, out GestionePensione.DatiPensione datiPensione)
        {
            datiPensione = null;

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                (AreaPrelievo.FinalResponse.Gruppo1.AreaW1L != null || AreaPrelievo.FinalResponse.Gruppo1.AreaW2 != null ||
                AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null || AreaPrelievo.FinalResponse.Gruppo1.AreaTP11 != null ||
                AreaPrelievo.FinalResponse.Gruppo1.AreaTP12 != null || AreaPrelievo.FinalResponse.Gruppo1.AreaDati != null))
            {
                datiPensione = new GestionePensione.DatiPensione();
                DateTime dataSistema = Utility.DataSistemaCi;

                Data.PCIINPU7.AreaW1L areaW1L = AreaPrelievo.FinalResponse.Gruppo1.AreaW1L;
                Data.PCIINPU7.AreaW2 areaW2 = AreaPrelievo.FinalResponse.Gruppo1.AreaW2;
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;
                Data.PCIINPU7.AreaTP11 areaTP11 = AreaPrelievo.FinalResponse.Gruppo1.AreaTP11;
                Data.PCIINPU7.AreaTP12 areaTP12 = AreaPrelievo.FinalResponse.Gruppo1.AreaTP12;
                Data.PCIINPU7.AreaDati areaDati = AreaPrelievo.FinalResponse.Gruppo1.AreaDati;

                if (areaW1L != null)
                {
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        datiPensione.CausaCarico = 2;
                    else
                        datiPensione.CausaCarico = Utility.StringToNullableByte(areaW1L.IW1CARIC.ToString());
                    datiPensione.DataInizioCalcolo = Utility.DataFromInt(areaW1L.IW1DA1A, areaW1L.IW1DA1M, 1);
                    if (!String.IsNullOrEmpty(areaW1L.IW1TIPEL))
                    {
                        switch (areaW1L.IW1TIPEL.Trim().ToUpperInvariant())
                        {
                            case "V":
                            case "O":
                                datiPensione.FlagVerify = true;
                                break;
                            case "R":
                            case "L":
                                datiPensione.FlagVerify = false;
                                break;
                        }
                    }
                }
                if (areaW2 != null)
                {
                    if (!String.IsNullOrEmpty(areaW2.IABCONA2))
                        datiPensione.NaturaPensione = areaW2.IABCONA2;
                    else
                        datiPensione.NaturaPensione = " ";

                    if (!String.IsNullOrEmpty(areaW2.IABCONA3))
                        datiPensione.NaturaPensione += areaW2.IABCONA3;
                    else
                        datiPensione.NaturaPensione += " ";

                    if (!String.IsNullOrEmpty(areaW2.IABCONA4))
                        datiPensione.NaturaPensione += areaW2.IABCONA4;
                    else
                        datiPensione.NaturaPensione += " ";
                }
                if (areaVarie != null)
                {
                    datiPensione.DataPerfezionamentoRequisiti = Utility.DataFromInt(areaVarie.DECPERFREQ_A, areaVarie.DECPERFREQ_M, areaVarie.DECPERFREQ_G);

                    if (!String.IsNullOrEmpty(areaVarie.IREQ300996))
                    {
                        switch (areaVarie.IREQ300996)
                        {
                            case "1":
                                datiPensione.RequisitiAl996 = false;
                                break;
                            case "2":
                                datiPensione.RequisitiAl996 = true;
                                break;
                        }
                    }
                    if (!String.IsNullOrEmpty(areaVarie.IREQVE1294))
                        datiPensione.RequisitiVecchiaiaAl1294 = areaVarie.IREQVE1294.Trim().ToUpperInvariant() == "2" ? true : areaVarie.IREQVE1294.Trim().ToUpperInvariant() == "1" ? false : (bool?)null;

                    if (areaVarie.IDECARPENA != 0 && areaVarie.IDECARPENM != 0)
                        datiPensione.DecorrenzaCalcoloArretrati = Utility.DataFromInt(areaVarie.IDECARPENA, areaVarie.IDECARPENM, 1);

                    datiPensione.DataPrimaDomanda = Utility.DataFromInt(areaVarie.IPRIMADAAA, areaVarie.IPRIMADAMM, areaVarie.IPRIMADAGG);
                    datiPensione.CentroOperativoDestinazione = (byte)areaVarie.COD_C_OPERATIVO;

                    //ENG - Memo 28_2024 recupero GP1TPCLC (secondo byte = 1)
                    //if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                    //{
                    //    if (!String.IsNullOrEmpty(areaVarie.GP1TPCLC) && areaVarie.GP1TPCLC.Length >= 2 && areaVarie.GP1TPCLC.Substring(1, 1) == "1")
                    //    {
                    //        datiPensione.Caratterizzazione = " 1      ";
                    //    }
                    //}
                }

                if (areaTP11 != null)
                {
                    datiPensione.CodiceSedeDestinazione = (short)areaTP11.TP1SEDE;
                }

                if (areaTP12 != null)
                {
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        datiPensione.CodiceArretrati = 8;
                    else
                        datiPensione.CodiceArretrati = Utility.StringToNullableByte(areaTP12.TP1ACC.ToString());

                    datiPensione.AttivitaEconomica = areaTP12.TP1ATEC;
                    datiPensione.ProfessioneIndividuale = areaTP12.TP1PRIN;
                    datiPensione.DataInteressiLegali = Utility.DataFromInt(areaTP12.TP1ILEGA, areaTP12.TP1ILEGM, areaTP12.TP1ILEGG);
                }

                if (areaDati != null)
                {
                    if (areaDati.DAT4218 != 0 && areaDati.DAT4218.ToString().Length >= 6)
                    {
                        string data = areaDati.DAT4218.ToString();
                        if (int.Parse(data.Substring(4, 2)) > int.Parse(dataSistema.Year.ToString().PadLeft(4, '0').Substring(2)))
                            data = data.Substring(0, 4) + "20" + data.Substring(4);
                        else
                            data = data.Substring(0, 4) + "19" + data.Substring(4);
                        datiPensione.DataRicezionePrenotazioneCentrale = Utility.DataFromInt(short.Parse(data.Substring(4)), short.Parse(data.Substring(2, 2)), short.Parse(data.Substring(0, 2)));
                    }
                }
            }
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                (AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaAltriCampi != null || AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio != null || AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR != null))
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati = AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati;
                Data.PCIINPU7.AreaAltriCampi areaAltriCampi = AreaPrelievo.FinalResponse.Gruppo3.AreaAltriCampi;
                Data.PCIINPU7.AreaSpazio areaSpazio = AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio;
                Data.PCIINPU7.AreaW2CIR areaW2CIR = AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR;

                if (areaUlterioriDati != null)
                {
                    if (!String.IsNullOrEmpty(areaUlterioriDati.IREQ311294))
                    {
                        switch (areaUlterioriDati.IREQ311294)
                        {
                            case "1":
                                datiPensione.RequisitiAl1294 = false;
                                break;
                            case "2":
                                datiPensione.RequisitiAl1294 = true;
                                break;
                        }
                    }
                }

                if (areaAltriCampi != null)
                {
                    datiPensione.FineAssicurazione = Utility.DataFromInt(areaAltriCampi.FINASSA, areaAltriCampi.FINASSM, areaAltriCampi.FINASSG);
                    datiPensione.InizioAssicurazione = Utility.DataFromInt(areaAltriCampi.INIASSA, areaAltriCampi.INIASSM, areaAltriCampi.INIASSG);
                }

                if (areaSpazio != null)
                {
                    datiPensione.DataCompletezza = Utility.DataFromInt(areaSpazio.TP1COMPA, areaSpazio.TP1COMPM, areaSpazio.TP1COMPG);
                }

                //ENG - Implementata la gestione mancante per le Reversibilità
                //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                {
                    if (areaW2CIR != null)
                    {
                        if (areaW2CIR.ICI2DAPLIQA != 0 && areaW2CIR.ICI2DAPLIQM != 0)
                            datiPensione.DecorrenzaOriginariaPrima = Utility.DataFromInt(areaW2CIR.ICI2DAPLIQA, areaW2CIR.ICI2DAPLIQM, 1);
                    }
                }
            }
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo4 != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018 != null)
            {
                if (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Count > 0)
                {
                    if (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "5300") || AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "5800") ||
                        AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "6000") || AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "6100"))
                    {
                        if (datiPensione == null)
                            datiPensione = new GestionePensione.DatiPensione();

                        if ((AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "5300") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "5300").FELPE_CODSGRUP == "5301") ||
                            (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "5800") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "5800").FELPE_CODSGRUP == "5801") ||
                            (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "6000") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "6000").FELPE_CODSGRUP == "6001") ||
                            (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "6100") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "6100").FELPE_CODSGRUP == "6101"))
                            datiPensione.LavoratorePubblico = false;
                        else if ((AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "5300") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "5300").FELPE_CODSGRUP == "5302") ||
                            (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "5800") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "5800").FELPE_CODSGRUP == "5802") ||
                             (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "6000") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "6000").FELPE_CODSGRUP == "6002") ||
                             (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Any(x => x.FELPE_CODGRUP == "6100") && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault(x => x.FELPE_CODGRUP == "6100").FELPE_CODSGRUP == "6102"))
                            datiPensione.LavoratorePubblico = true;
                    }

                    if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault().FELPE_GP2PBNFGL))
                    {
                        byte res = 0;
                        byte.TryParse(AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.FirstOrDefault().FELPE_GP2PBNFGL, out res);
                        datiPensione.NumeroFigli = res > 0 ? res : (byte?)null;
                    }
                }

                if (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.AreaINAIL != null)
                {
                    if (AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.AreaINAIL.SENTENZA_IGP1AV91A != null)
                    {

                        datiPensione.GP1AV91A = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.AreaINAIL.SENTENZA_IGP1AV91A.IGP1AV91A;
                        if (datiPensione.GP1AV91A == null)
                            datiPensione.GP1AV91A = 0;
                    }
                }

                List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri> listaOneri = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI;
                if (listaOneri != null)
                {
                    var felpeCodBenef = listaOneri.Find(x => x.FELPE_CODBENEF == "12" || x.FELPE_CODBENEF == "15");
                    if (felpeCodBenef != null)
                    {
                        switch (felpeCodBenef.FELPE_CODBENEF)
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
        }

        public static void ValorizzaDatiSindacato(Data.GACI AreaPrelievo, out GestionePensione.DatiSindacato datiSindacato)
        {
            datiSindacato = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaW2 != null)
            {
                datiSindacato = new GestionePensione.DatiSindacato();
                Data.PCIINPU7.AreaW2 areaW2 = AreaPrelievo.FinalResponse.Gruppo1.AreaW2;

                if (!String.IsNullOrEmpty(areaW2.IABCOSIND))
                    datiSindacato.CodiceSindacato = areaW2.IABCOSIND;
            }
        }

        public static void ValorizzaDatiDetrazioni(Data.GACI AreaPrelievo, out GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni)
        {
            datiDetrazioni = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaTP12 != null)
            {
                datiDetrazioni = new GestioneDetrazioniImposta.DatiDetrazioni();
                Data.PCIINPU7.AreaTP12 areaTP12 = AreaPrelievo.FinalResponse.Gruppo1.AreaTP12;
                datiDetrazioni.DetrazioniReddito = Utility.StringToNullableByte(areaTP12.CO1N.ToString());
                datiDetrazioni.AgevolazionePensionati = Utility.StringToNullableByte(areaTP12.CO2N.ToString());
                datiDetrazioni.ConiugeOFiglio = Utility.StringToNullableByte(areaTP12.CO3N.ToString());
                datiDetrazioni.FigliMinori3AnniNoHandicap100 = Utility.StringToNullableByte(areaTP12.CO4N.ToString());
                datiDetrazioni.FigliMinori3AnniNoHandicap50 = Utility.StringToNullableByte(areaTP12.CO5N.ToString());
                datiDetrazioni.FigliMinori3AnniHandicap100 = Utility.StringToNullableByte(areaTP12.CO6N.ToString());
                datiDetrazioni.FigliMinori3AnniHandicap50 = Utility.StringToNullableByte(areaTP12.CO7N.ToString());
                datiDetrazioni.FigliMaggiori3AnniNoHandicap100 = Utility.StringToNullableByte(areaTP12.CO8N.ToString());
                datiDetrazioni.FigliMaggiori3AnniNoHandicap50 = Utility.StringToNullableByte(areaTP12.CO9N.ToString());
                datiDetrazioni.FigliMaggiori3AnniHandicap100 = Utility.StringToNullableByte(areaTP12.CO10N.ToString());
                datiDetrazioni.FigliMaggiori3AnniHandicap50 = Utility.StringToNullableByte(areaTP12.CO11N.ToString());
                datiDetrazioni.AltriFamiliari100 = Utility.StringToNullableByte(areaTP12.CO12N.ToString());
                datiDetrazioni.AltriFamiliari50 = Utility.StringToNullableByte(areaTP12.CO13N.ToString());
                datiDetrazioni.AddizionaleLombardiaVeneto = Utility.StringToNullableByte(areaTP12.CO14N.ToString());
            }
        }

        public static void ValorizzaDatiPagamento(Data.GACI AreaPrelievo, out GestionePagamento.DatiPagamento datiPagamento)
        {
            datiPagamento = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                (AreaPrelievo.FinalResponse.Gruppo1.AreaDati != null || AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null))
            {
                datiPagamento = new GestionePagamento.DatiPagamento();
                Data.PCIINPU7.AreaDati areaDati = AreaPrelievo.FinalResponse.Gruppo1.AreaDati;
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaDati != null)
                {
                    datiPagamento.ABI = areaDati.TP1ABI;
                    string cab = areaDati.TP1LIRE_EURO + areaDati.TP1SEDEUP.PadLeft(4, '0') + areaDati.TP1CIN + areaDati.TP1COSTA;
                    datiPagamento.CAB = Utility.StringToNullableInt(cab);
                    datiPagamento.ModalitaPagamento = GetModalitaPagamento(areaDati, areaVarie);
                }

                if (areaVarie != null)
                {
                    if (!String.IsNullOrEmpty(areaVarie.AN87A))
                        datiPagamento.TrattenutaInpdap = areaVarie.AN87A.Trim().ToUpperInvariant() == "SI" ? true : areaVarie.AN87A.Trim().ToUpperInvariant() == "NO" ? false : (bool?)null;
                    datiPagamento.DataRinunciaTrattenutaInpdap = Utility.DataFromInt(areaVarie.AN87DATAA, areaVarie.AN87DATAM, 1);
                    datiPagamento.BIC = areaVarie.BIC;
                    datiPagamento.IBAN = areaVarie.IBAN;
                    if (datiPagamento.ModalitaPagamento.GetValueOrDefault() == 'L' &&
                            datiPagamento.ABI.GetValueOrDefault() == 07601)
                    {
                        if (!(!string.IsNullOrEmpty(areaVarie.IBAN) &&
                                areaVarie.IBAN.Length == 27 &&
                                areaVarie.IBAN.StartsWith("IT") &&
                                areaVarie.IBAN.Substring(10, 5) == "03384"))
                        {
                            datiPagamento.Libretto = areaVarie.IBAN;
                            datiPagamento.IBAN = string.Empty;
                        }
                    }
                }
            }
        }

        private static char? GetModalitaPagamento(Data.PCIINPU7.AreaDati areaDati, Data.PCIINPU7.AreaVarie areaVarie)
        {
            char? modPag = null;
            if (areaDati == null)
                return modPag;

            string cab = areaDati.TP1LIRE_EURO.PadLeft(1, '0') + areaDati.TP1SEDEUP.PadLeft(4, '0') + areaDati.TP1CIN.PadLeft(1, '0') + areaDati.TP1COSTA.PadLeft(1, '0');

            modPag = Utility.StringToNullableChar(areaDati.TP1MODPAG);
            if (!modPag.HasValue)
            {
                if (areaDati.TP1ABI == 99999)
                    modPag = 'P';
                else if (areaDati.TP1ABI == 07601)
                {
                    if (areaVarie != null && !string.IsNullOrEmpty(areaVarie.IBAN))
                        modPag = 'L';
                    else
                        modPag = 'S';
                }
                else if (areaDati.TP1ABI == 36081 && cab == "05138")
                {
                    if (areaVarie != null && !string.IsNullOrEmpty(areaVarie.IBAN))
                        modPag = 'K';
                    else
                        modPag = 'S';
                }
                else if (cab != "0000000" && (cab.StartsWith("44") || cab.StartsWith("77")) && cab.Length >= 7)
                {
                    if (areaVarie != null && !string.IsNullOrEmpty(areaVarie.IBAN))
                        modPag = 'C';
                    else
                        modPag = 'S';
                }
                else if (areaDati.TP1ABI != 0)
                {
                    modPag = 'S';
                }
                else
                {
                    if (areaVarie != null && !string.IsNullOrEmpty(areaVarie.IBAN))
                    {
                        if (cab != "0000000" || !string.IsNullOrEmpty(areaVarie.BIC))
                            modPag = 'C';
                        else
                            modPag = 'L';
                    }
                }
            }

            return modPag;
        }

        public static void ValorizzaDatiFamiliare(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out List<Entity.DatiFamiliari> ListaFamiliari)
        {
            ListaFamiliari = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo2 != null &&
                AreaPrelievo.FinalResponse.Gruppo2.AreaW4 != null)
            {
                ListaFamiliari = new List<Entity.DatiFamiliari>();
                Data.PCIINPU7.AreaW4 areaW4 = AreaPrelievo.FinalResponse.Gruppo2.AreaW4;
                if (areaW4.CODICIFISCALIFAMILIARI != null && areaW4.CODICIFISCALIFAMILIARI.Count > 0 &&
                    areaW4.DATIFAMILIARI != null && areaW4.DATIFAMILIARI.Count > 0)
                {
                    for (int i = 0; i < areaW4.CODICIFISCALIFAMILIARI.Count; i++)
                    {
                        Entity.DatiFamiliari fam = new Entity.DatiFamiliari();
                        fam.Familiare = new GestioneFamiliari.Familiare();
                        fam.ElencoCodMaggFamiliari = new List<GestioneFamiliari.CodMaggFamiliari>();

                        if (areaW4.DATIFAMILIARI[i].LIST_GP3CK != null && areaW4.DATIFAMILIARI[i].LIST_GP3CK.Count > 0)
                        {
                            fam.Familiare.SiglaFamiliare = Utility.StringToNullableChar(areaW4.DATIFAMILIARI[i].LIST_GP3CK[0].IW4SIG);

                            if (Utility.StringToNullableChar(areaW4.DATIFAMILIARI[i].LIST_GP3CK[0].IW4SIG) == 'C')
                            {
                                if (areaW4.DATIFAMILIARI[i].LIST_GP3CK[0].GP3CH01B == "U")
                                    fam.Familiare.TipoUnione = "U";
                                else
                                    fam.Familiare.TipoUnione = "M";
                            }

                            foreach (Data.PCIINPU7.AreaW4.GP3CK cM in areaW4.DATIFAMILIARI[i].LIST_GP3CK)
                            {
                                GestioneFamiliari.CodMaggFamiliari codMagg = new GestioneFamiliari.CodMaggFamiliari();
                                codMagg.Decorrenza = Utility.DataFromInt(cM.IW4ACQA, cM.IW4ACQM, 1);
                                codMagg.Cessazione = Utility.DataFromInt(cM.IW4CESA, cM.IW4CESM, 1);
                                codMagg.CodiceMaggiorazione = Utility.StringToNullableByte(cM.IW4CMAG.ToString());
                                codMagg.SiglaFamiliare = Utility.StringToNullableChar(cM.IW4SIG);
                                if (Utility.StringToNullableChar(cM.IW4SIG) == 'C')
                                {
                                    if (cM.GP3CH01B == "U")
                                        codMagg.TipoUnione = "U";
                                    else
                                        codMagg.TipoUnione = "M";
                                }

                                if (codMagg.Decorrenza.HasValue || codMagg.Cessazione.HasValue)
                                    fam.ElencoCodMaggFamiliari.Add(codMagg);
                            }
                        }

                        if(fam.ElencoCodMaggFamiliari != null && fam.ElencoCodMaggFamiliari.Count > 0)
                        {
                            fam.Familiare.SiglaFamiliare = fam.ElencoCodMaggFamiliari.OrderByDescending(x => x.Decorrenza).FirstOrDefault().SiglaFamiliare;
                        }

                        fam.Familiare.CodiceFiscale = areaW4.CODICIFISCALIFAMILIARI[i].IW4COFI;

                        if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                            fam.Familiare.Confermato = true;

                        ListaFamiliari.Add(fam);
                    }
                }
            }

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
               AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio != null)
            {
                Data.PCIINPU7.AreaSpazio areaSpazio = AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio;

                if (ListaFamiliari != null && ListaFamiliari.Count > 0)
                {
                    for (int i = 0; i < ListaFamiliari.Count; i++)
                    {
                        if (areaSpazio.REVISIONISANITARIE != null && areaSpazio.REVISIONISANITARIE.Count > i && areaSpazio.REVISIONISANITARIE[i].TP1REVFA != 0 && areaSpazio.REVISIONISANITARIE[i].TP1REVFM != 0)
                            ListaFamiliari[i].Familiare.ScadenzaRevisioneSanitaria = new DateTime(areaSpazio.REVISIONISANITARIE[i].TP1REVFA, areaSpazio.REVISIONISANITARIE[i].TP1REVFM, 01);
                    }
                }
            }
        }

        public static void ValorizzaDatiCalcoloContributivo(Data.GACI AreaPrelievo, out List<GestioneCalcolo.DatiCalcoloContributivo> ListaCalcoloContributivo)
        {
            ListaCalcoloContributivo = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                AreaPrelievo.FinalResponse.Gruppo3.AreaContributi335 != null)
            {
                ListaCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                Data.PCIINPU7.AreaContributi335 areaContributi335 = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi335;

                if (areaContributi335.ICICONOBG335 != 0M || areaContributi335.ICIRETOBG335 != 0M ||
                    areaContributi335.ICISTOBG335 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContributivoTotale = areaContributi335.ICICONOBG335;
                    contr.Montante = areaContributi335.ICIRETOBG335;
                    contr.NSettimane = areaContributi335.ICISTOBG335;
                    contr.CodiceGestione = 1;
                    ListaCalcoloContributivo.Add(contr);
                }

                if (areaContributi335.ICICONCDM335 != 0M || areaContributi335.ICIRETCDM335 != 0M ||
                    areaContributi335.ICISTCDM335 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContributivoTotale = areaContributi335.ICICONCDM335;
                    contr.Montante = areaContributi335.ICIRETCDM335;
                    contr.NSettimane = areaContributi335.ICISTCDM335;
                    contr.CodiceGestione = 2;
                    ListaCalcoloContributivo.Add(contr);
                }

                if (areaContributi335.ICICONART335 != 0M || areaContributi335.ICIRETART335 != 0M ||
                    areaContributi335.ICISTART335 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContributivoTotale = areaContributi335.ICICONART335;
                    contr.Montante = areaContributi335.ICIRETART335;
                    contr.NSettimane = areaContributi335.ICISTART335;
                    contr.CodiceGestione = 3;
                    ListaCalcoloContributivo.Add(contr);
                }

                if (areaContributi335.ICICONCOM335 != 0M || areaContributi335.ICIRETCOM335 != 0M ||
                    areaContributi335.ICISTCOM335 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContributivoTotale = areaContributi335.ICICONCOM335;
                    contr.Montante = areaContributi335.ICIRETCOM335;
                    contr.NSettimane = areaContributi335.ICISTCOM335;
                    contr.CodiceGestione = 4;
                    ListaCalcoloContributivo.Add(contr);
                }
            }

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null)
            {
                if (ListaCalcoloContributivo == null)
                    ListaCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaVarie.ICICONOBG012 != 0M || areaVarie.ICIRETOBG012 != 0M ||
                    areaVarie.ICISTOBG012 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContribTotaleQuotaDL214 = areaVarie.ICICONOBG012;
                    contr.MontanteQuotaDL214 = areaVarie.ICIRETOBG012;
                    contr.NSettimaneQuotaDL214 = areaVarie.ICISTOBG012;
                    contr.CodiceGestione = 1;
                    ListaCalcoloContributivo.Add(contr);
                }

                if (areaVarie.ICICONCDM012 != 0M || areaVarie.ICIRETCDM012 != 0M ||
                    areaVarie.ICISTCDM012 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContribTotaleQuotaDL214 = areaVarie.ICICONCDM012;
                    contr.MontanteQuotaDL214 = areaVarie.ICIRETCDM012;
                    contr.NSettimaneQuotaDL214 = areaVarie.ICISTCDM012;
                    contr.CodiceGestione = 2;
                    ListaCalcoloContributivo.Add(contr);
                }

                if (areaVarie.ICICONART012 != 0M || areaVarie.ICIRETART012 != 0M ||
                    areaVarie.ICISTART012 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContribTotaleQuotaDL214 = areaVarie.ICICONART012;
                    contr.MontanteQuotaDL214 = areaVarie.ICIRETART012;
                    contr.NSettimaneQuotaDL214 = areaVarie.ICISTART012;
                    contr.CodiceGestione = 3;
                    ListaCalcoloContributivo.Add(contr);
                }

                if (areaVarie.ICICONCOM012 != 0M || areaVarie.ICIRETCOM012 != 0M ||
                    areaVarie.ICISTCOM012 != 0)
                {
                    GestioneCalcolo.DatiCalcoloContributivo contr = new GestioneCalcolo.DatiCalcoloContributivo();
                    contr.ImportoContribTotaleQuotaDL214 = areaVarie.ICICONCOM012;
                    contr.MontanteQuotaDL214 = areaVarie.ICIRETCOM012;
                    contr.NSettimaneQuotaDL214 = areaVarie.ICISTCOM012;
                    contr.CodiceGestione = 4;
                    ListaCalcoloContributivo.Add(contr);
                }
            }
        }

        public static void ValorizzaDatiCalcoloRetributivo(Data.GACI AreaPrelievo, out List<GestioneCalcolo.DatiCalcoloRetributivo> ListaCalcoloRetributivo)
        {
            ListaCalcoloRetributivo = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                (AreaPrelievo.FinalResponse.Gruppo3.AreaContributi != null || AreaPrelievo.FinalResponse.Gruppo3.AreaContributi233 != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaContributi503 != null))
            {
                ListaCalcoloRetributivo = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
                Data.PCIINPU7.AreaContributi areaContributi = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi;
                Data.PCIINPU7.AreaContributi233 areaContributi233 = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi233;
                Data.PCIINPU7.AreaContributi503 areaContributi503 = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi503;
                Data.PCIINPU7.AreaCampiVar areaCampiVar = AreaPrelievo.FinalResponse.Gruppo4.AreaCampiVar;

                if (areaContributi != null)
                {
                    if (areaContributi.IABREMSVV != 0M || areaContributi.IW1NSOBG != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaA = areaContributi.IABREMSVV;
                        retrib.NSettimaneQuotaA = areaContributi.IW1NSOBG;
                        retrib.QuotePrimeLiquidate = 'A';
                        retrib.CodiceGestione = 1;
                        ListaCalcoloRetributivo.Add(retrib);
                    }
                }

                if (areaContributi233 != null)
                {
                    if (areaContributi233.IW1RMSOBG != 0M || areaContributi233.IW1SAOBG != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaA = areaContributi233.IW1RMSOBG;
                        retrib.NSettimaneQuotaA = areaContributi233.IW1SAOBG;
                        retrib.QuotePrimeLiquidate = 'A';
                        retrib.CodiceGestione = 1;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10OBGA != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10OBGA;
                        ListaCalcoloRetributivo.Add(retrib);
                    }

                    if (areaContributi233.IW1RMSCDM != 0M || areaContributi233.IW1SACDM != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaA = areaContributi233.IW1RMSCDM;
                        retrib.NSettimaneQuotaA = areaContributi233.IW1SACDM;
                        retrib.QuotePrimeLiquidate = 'A';
                        retrib.CodiceGestione = 2;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10CDMA != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10CDMA;
                        ListaCalcoloRetributivo.Add(retrib);
                    }

                    if (areaContributi233.IW1RMSART != 0M || areaContributi233.IW1SAART != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaA = areaContributi233.IW1RMSART;
                        retrib.NSettimaneQuotaA = areaContributi233.IW1SAART;
                        retrib.QuotePrimeLiquidate = 'A';
                        retrib.CodiceGestione = 3;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10ARTA != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10ARTA;
                        ListaCalcoloRetributivo.Add(retrib);
                    }

                    if (areaContributi233.IW1RMSCOM != 0M || areaContributi233.IW1SACOM != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaA = areaContributi233.IW1RMSCOM;
                        retrib.NSettimaneQuotaA = areaContributi233.IW1SACOM;
                        retrib.QuotePrimeLiquidate = 'A';
                        retrib.CodiceGestione = 4;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10COMA != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10COMA;
                        ListaCalcoloRetributivo.Add(retrib);
                    }
                }

                if (areaContributi503 != null)
                {
                    if (areaContributi503.IW1RETOBG != 0M || areaContributi503.IW1STOBG != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaB = areaContributi503.IW1RETOBG;
                        retrib.NSettimaneQuotaB = areaContributi503.IW1STOBG;
                        retrib.QuotePrimeLiquidate = 'B';
                        retrib.CodiceGestione = 1;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10OBGB != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10OBGB;
                        ListaCalcoloRetributivo.Add(retrib);
                    }

                    if (areaContributi503.IW1RETCDM != 0M || areaContributi503.IW1STCDM != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaB = areaContributi503.IW1RETCDM;
                        retrib.NSettimaneQuotaB = areaContributi503.IW1STCDM;
                        retrib.QuotePrimeLiquidate = 'B';
                        retrib.CodiceGestione = 2;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10CDMB != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10CDMB;
                        ListaCalcoloRetributivo.Add(retrib);
                    }

                    if (areaContributi503.IW1RETART != 0M || areaContributi503.IW1START != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaB = areaContributi503.IW1RETART;
                        retrib.NSettimaneQuotaB = areaContributi503.IW1START;
                        retrib.QuotePrimeLiquidate = 'B';
                        retrib.CodiceGestione = 3;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10ARTB != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10ARTB;
                        ListaCalcoloRetributivo.Add(retrib);
                    }

                    if (areaContributi503.IW1RETCOM != 0M || areaContributi503.IW1STCOM != 0)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo retrib = new GestioneCalcolo.DatiCalcoloRetributivo();
                        retrib.RMSQuotaB = areaContributi503.IW1RETCOM;
                        retrib.NSettimaneQuotaB = areaContributi503.IW1STCOM;
                        retrib.QuotePrimeLiquidate = 'B';
                        retrib.CodiceGestione = 4;
                        if (areaCampiVar != null && areaCampiVar.Dati_2016 != null && areaCampiVar.Dati_2016.GP2BC10COMB != 0)
                            retrib.NSettimane707 = areaCampiVar.Dati_2016.GP2BC10COMB;
                        ListaCalcoloRetributivo.Add(retrib);
                    }
                }
            }
        }

        public static void ValorizzaDatiSupplementi(Data.GACI AreaPrelievo, out List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> ListaSupplementi)
        {
            ListaSupplementi = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo2 != null &&
                AreaPrelievo.FinalResponse.Gruppo2.AreaW3 != null)
            {
                ListaSupplementi = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
                Data.PCIINPU7.AreaW3 areaW3 = AreaPrelievo.FinalResponse.Gruppo2.AreaW3;

                if (areaW3.SUPPLEMENTI != null && areaW3.SUPPLEMENTI.Count > 0)
                {
                    foreach (Data.PCIINPU7.AreaW3.Supplemento supp in areaW3.SUPPLEMENTI)
                    {
                        INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupp = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
                        datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(supp.IW3DESUPA, supp.IW3DESUPM, 1);
                        if (supp.IW3IVS != 0M)
                            datiSupp.MontanteSupplemento = supp.IW3IVS;
                        if (supp.IW3RETSET != 0M)
                        {
                            datiSupp.RMSSupplemento = supp.IW3RETSET;
                            datiSupp.TipoSupplemento = 'R';

                            //ENG - Modifica Supplementi CI Memo 177/2012
                            if (!string.IsNullOrEmpty(supp.IW3TIPSUP))
                            {
                                if (supp.IW3TIPSUP == "0")
                                    datiSupp.QuotaSupplemento = 'A';
                                else if (supp.IW3TIPSUP == "1")
                                    datiSupp.QuotaSupplemento = 'B';
                            }
                        }
                        if (!string.IsNullOrEmpty(supp.IW3COGEST))
                            datiSupp.CodGestioneSupplemento = supp.IW3COGEST;
                        if (supp.IW3SETANZ != 0)
                            datiSupp.NSettimaneSupplemento = supp.IW3SETANZ;
                        if (supp.IW3IVSSOS != 0M)
                            datiSupp.AmmontareContributivo = supp.IW3IVSSOS;
                        if (datiSupp.MontanteSupplemento.HasValue && datiSupp.AmmontareContributivo.HasValue)
                        {
                            datiSupp.TipoSupplemento = 'C';

                            //ENG - Modifica Supplementi CI Memo 177/2012
                            if (!string.IsNullOrEmpty(supp.IW3TIPSUP))
                            {
                                if (supp.IW3TIPSUP == "3")
                                    datiSupp.QuotaSupplemento = 'C';
                                else if (supp.IW3TIPSUP == "4")
                                    datiSupp.QuotaSupplemento = 'D';
                            }
                        }

                        ListaSupplementi.Add(datiSupp);
                    }
                }
            }
        }

        public static void ValorizzaDatiDanteCausa(Data.GACI AreaPrelievo, out DatiAnagDanteCausa datiAnagDanteCausa, out GestioneDanteCausa.DatiDanteCausa datiDanteCausa, GestionePrelievo.TipoDomanda tipoDomanda, short categoria)
        {
            datiDanteCausa = null;
            datiAnagDanteCausa = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                (AreaPrelievo.FinalResponse.Gruppo1.AreaTP12 != null || AreaPrelievo.FinalResponse.Gruppo1.AreaW1L != null ||
                AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null))
            {
                datiAnagDanteCausa = new DatiAnagDanteCausa();
                datiDanteCausa = new GestioneDanteCausa.DatiDanteCausa();
                Data.PCIINPU7.AreaTP12 areaTP12 = AreaPrelievo.FinalResponse.Gruppo1.AreaTP12;
                Data.PCIINPU7.AreaW1L areaW1L = AreaPrelievo.FinalResponse.Gruppo1.AreaW1L;
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaTP12 != null)
                {
                    datiAnagDanteCausa.Cognome = areaTP12.TP1COGDC.Trim();
                    datiAnagDanteCausa.Nome = areaTP12.TP1NOMDC;
                    if (areaTP12.TP1SEDED != 0)
                        datiDanteCausa.Sede = areaTP12.TP1SEDED.ToString().PadLeft(4, '0');
                    if (areaTP12.TP1CATD != 0)
                    {
                        string siglaCategoria = "";
                        GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(areaTP12.TP1CATD.ToString().PadLeft(4, '0'), out siglaCategoria);
                        datiDanteCausa.SiglaCategoria = siglaCategoria;
                    }
                    if (areaTP12.TP1CERTD != 0)
                        datiDanteCausa.Certificato = areaTP12.TP1CERTD;
                    if (areaTP12.TP1COMDC != 0)
                        datiAnagDanteCausa.CodiceComuneInps = areaTP12.TP1COMDC;
                }

                if (areaW1L != null)
                {
                    datiAnagDanteCausa.Sesso = Utility.StringToNullableChar(areaW1L.IW1DSES);
                    datiAnagDanteCausa.DataNascita = Utility.DataFromInt(areaW1L.IW1DNASA, areaW1L.IW1DNASM, areaW1L.IW1DNASG);
                    datiDanteCausa.DataMorte = Utility.DataFromInt(areaW1L.IW1DMORA, areaW1L.IW1DMORM, areaW1L.IW1DMORG);
                    datiDanteCausa.DecorrenzaPensione = Utility.DataFromInt(areaW1L.IW1DEDIRA, areaW1L.IW1DEDIRM, 1);
                    if (areaW1L.IW1780CD != 0)
                        datiDanteCausa.Maggiorazione781Contributi = Utility.StringToNullableByte(areaW1L.IW1780CD.ToString());

                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                        datiDanteCausa.ProvenienzaPensione = Utility.StringToNullableByte(areaW1L.IW1DPROV.ToString());
                }

                if (areaVarie != null)
                {
                    datiDanteCausa.DecorrenzaResidenza = Utility.DataFromInt(areaVarie.DECRESDCA, areaVarie.DECRESDCM, 1);
                    if (!String.IsNullOrEmpty(areaVarie.IAPCATEG_DC))
                    {
                        short resShort = 0;
                        short enteAltraPensione = 0;
                        short.TryParse(areaVarie.IAPCATEG_DC, out resShort);
                        datiDanteCausa.CategoriaAltraPensione = resShort != 0 ? resShort.ToString() : areaVarie.IAPCATEG_DC.Trim();
                        datiDanteCausa.CessazioneAltraPensione = Utility.DataFromInt(areaVarie.IAPCESSAA_DC, areaVarie.IAPCESSAM_DC, 1);
                        if (areaVarie.IAPCODIMP_DC != 0)
                            datiDanteCausa.CodiceImportoAltraPensione = Utility.StringToNullableChar(areaVarie.IAPCODIMP_DC.ToString());

                        datiDanteCausa.DecorrenzaAltraPensione = Utility.DataFromInt(areaVarie.IAPDECORA_DC, areaVarie.IAPDECORM_DC, 1);
                        //datiDanteCausa.DecorrenzaAltraPensione = Utility.DataFromInt(areaVarie.IAPCESSAA_DC, areaVarie.IAPCESSAM_DC, 1);
                        datiDanteCausa.CodiceUCAltraPensione = Utility.StringToNullableChar(areaVarie.IAPUNIC_DC);
                        if (!String.IsNullOrEmpty(areaVarie.IAPENTE_DC) && short.TryParse(areaVarie.IAPENTE_DC, out enteAltraPensione))
                        {
                            datiDanteCausa.EnteAltraPensione = enteAltraPensione;
                        }
                    }
                    if (areaVarie.DATA_MATRIM_A != 0 && areaVarie.DATA_MATRIM_M != 0 && areaVarie.DATA_MATRIM_G != 0)
                        datiAnagDanteCausa.DataMatrimonio = new DateTime(areaVarie.DATA_MATRIM_A, areaVarie.DATA_MATRIM_M, areaVarie.DATA_MATRIM_G);

                    if (!String.IsNullOrEmpty(areaVarie.IW8NAT1_DC))
                        datiDanteCausa.NaturaPensione = areaVarie.IW8NAT1_DC;
                    if (!String.IsNullOrEmpty(areaVarie.IW8NAT2_DC))
                        if (!String.IsNullOrEmpty(datiDanteCausa.NaturaPensione) && datiDanteCausa.NaturaPensione.Length == 1)
                            datiDanteCausa.NaturaPensione += areaVarie.IW8NAT2_DC;
                        else
                            datiDanteCausa.NaturaPensione = " " + areaVarie.IW8NAT2_DC;
                    if (!String.IsNullOrEmpty(areaVarie.IW8NAT3_DC))
                        if (!String.IsNullOrEmpty(datiDanteCausa.NaturaPensione) && datiDanteCausa.NaturaPensione.Length == 2)
                            datiDanteCausa.NaturaPensione += areaVarie.IW8NAT3_DC;
                        else if (!String.IsNullOrEmpty(datiDanteCausa.NaturaPensione) && datiDanteCausa.NaturaPensione.Length == 1)
                            datiDanteCausa.NaturaPensione += " " + areaVarie.IW8NAT3_DC;
                        else
                            datiDanteCausa.NaturaPensione = "  " + areaVarie.IW8NAT3_DC;

                    //ENG - Implementata la gestione mancante per le Reversibilità
                    //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                    {
                        if (areaVarie.I_CRIRIL != 0)
                            datiDanteCausa.CodiceTipoPerequazione = Utility.StringToNullableByte(areaVarie.I_CRIRIL.ToString());

                        if (areaVarie.I_VINTERA != 0M)
                            datiDanteCausa.VirtualePura  = areaVarie.I_VINTERA;

                        if (areaVarie.I_VIRT != 0M)
                            datiDanteCausa.VirtualeIntegrata = areaVarie.I_VIRT;

                        if (areaVarie.I_ADEG != 0M)
                            datiDanteCausa.Adeguata = areaVarie.I_ADEG;
                    }
                    //
                }
            }

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                (AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati != null || AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R != null || AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR != null ||
                 AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R != null))
            {
                if (datiDanteCausa == null)
                    datiDanteCausa = new GestioneDanteCausa.DatiDanteCausa();

                Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati = AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati;
                Data.PCIINPU7.AreaWK1R areaWK1R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R;
                Data.PCIINPU7.AreaW2CIR areaW2CIR = AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR;
                Data.PCIINPU7.AreaWK2R areaWK2R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R;

                if (areaUlterioriDati != null)
                {
                    if (!string.IsNullOrEmpty(areaUlterioriDati.IRELPAR))
                    {
                        List<GestioneDecodifica.ParentelaDC> listaParentelaDC = null;
                        GestioneDecodifica.GetParentelaDC(out listaParentelaDC);
                        if (listaParentelaDC != null && listaParentelaDC.Count > 0)
                        {
                            GestioneDecodifica.ParentelaDC parentelaDC = listaParentelaDC.Find(x => x.Descrizione == areaUlterioriDati.IRELPAR);
                            if (parentelaDC != null)
                                datiDanteCausa.ParentelaDC = Utility.StringToNullableByte(parentelaDC.Id);
                        }
                    }
                }

                if (areaWK1R != null)
                {
                    if (areaWK1R.IW1DART5 != 0M)
                    {
                        datiDanteCausa.EccedenzaArt5 = areaWK1R.IW1DART5;
                    }
                }

                //ENG - Implementata la gestione mancante per le Reversibilità
                //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
                //Aggiunta gestione ImportoPagamentoDataMorte49593
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                {
                    if (areaWK2R != null)
                    {
                        if (areaWK2R.IABTQFI != 0M)
                            datiDanteCausa.TotaleQuoteFisse = areaWK2R.IABTQFI;
                        if (areaWK2R.IABML1Q != 0M)
                            datiDanteCausa.ImportoPagamentoDataMorte49593 = areaWK2R.IABML1Q;
                        if (areaW2CIR.ICI2VIRT != 0M)
                            datiDanteCausa.VirtualeIntegrata = areaW2CIR.ICI2VIRT;
                        if (areaW2CIR.ICI2VINTERA != 0M)
                            datiDanteCausa.VirtualePura = areaW2CIR.ICI2VINTERA;
                        if (areaWK2R.IABMCP != 0M)
                            datiDanteCausa.Adeguata = areaWK2R.IABMCP;
                    }
                }
            }
        }

        //ENG - Implementata la gestione mancante per le Reversibilità
        //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
        public static void ValorizzaDatiPensioniEstereDc(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, short categoria, out GestioneDanteCausa.PensioniEstereDcBL datiPensioniEstereDc)
        {
            datiPensioniEstereDc = null;
            if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
            {
                if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                    (AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R != null || AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R != null))
                {
                    datiPensioniEstereDc = new GestioneDanteCausa.PensioniEstereDcBL();
                    Data.PCIINPU7.AreaWK1R areaWK1R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R;
                    Data.PCIINPU7.AreaWK2R areaWK2R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R;

                    if (areaWK1R != null)
                    {
                        if (areaWK1R.IW1CM345 != 0)
                            datiPensioniEstereDc.CodiciVari = Utility.StringToNullableByte(areaWK1R.IW1CM345.ToString());
                    }

                    if (areaWK2R != null)
                    {
                        if (areaWK2R.IABMM345 != 0M)
                            datiPensioniEstereDc.Importo = areaWK2R.IABMM345;
                    }
                }
            }
        }
        //ENG - Implementata la gestione mancante per le Reversibilità
        //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
        public static void ValorizzaDatiPensioniEstereDcImportoTotaleSupplementi(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, short categoria, out GestioneDanteCausa.PensioniEstereDcBL importoTotSupplementi)
        {
            importoTotSupplementi = null;
            if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
            {
                if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null && AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR != null)
                {
                    importoTotSupplementi = new GestioneDanteCausa.PensioniEstereDcBL();
                    Data.PCIINPU7.AreaW2CIR areaW2CIR = AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR;

                    if (areaW2CIR != null)
                    {
                        if (areaW2CIR.ICI2SUP != 0M)
                            importoTotSupplementi.Importo = areaW2CIR.ICI2SUP;
                    }
                }
            }
        }
        //ENG - Implementata la gestione mancante per le Reversibilità
        //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
        public static void ValorizzaDatiPensioniEstereDcImportoArt6(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, short categoria, out GestioneDanteCausa.PensioniEstereDcBL importoArt6)
        {
            importoArt6 = null;
            if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
            {
                if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null && AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R != null)
                {
                    importoArt6 = new GestioneDanteCausa.PensioniEstereDcBL();
                    Data.PCIINPU7.AreaWK2R areaWK2R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R;

                    if (areaWK2R != null)
                    {
                        if (areaWK2R.IABMMEX6 != 0M)
                            importoArt6.Importo = areaWK2R.IABMMEX6;
                    }
                }
            }
        }

        public static void ValorizzaDatiResidenzeEstere(Data.GACI AreaPrelievo, out List<GestioneAnagrafica.DatiResidenzaEstero> ListaResidenzeEstere)
        {
            ListaResidenzeEstere = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null)
            {
                ListaResidenzeEstere = new List<GestioneAnagrafica.DatiResidenzaEstero>();
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaVarie.DATIRESIDENZA != null && areaVarie.DATIRESIDENZA.Count > 0)
                {
                    foreach (Data.PCIINPU7.AreaVarie.DatiResidenza res in areaVarie.DATIRESIDENZA)
                    {
                        if (res.IDECRESAA != 0 && res.IDECRESMM != 0 && res.ICODRES != "000")
                        {
                            GestioneAnagrafica.DatiResidenzaEstero datiRes = new GestioneAnagrafica.DatiResidenzaEstero();
                            datiRes.Decorrenza = Utility.DataFromInt((short)res.IDECRESAA, res.IDECRESMM, 1);
                            if (res.ICODRES == "ITA" || res.ICODRES == "I")
                                datiRes.CodCatastaleStatoEE = "Z000";
                            else if (res.ICODRES == "EE")
                                datiRes.CodCatastaleStatoEE = "";
                            else
                            {
                                List<GestioneDecodifica.StatoEstero> elencoStatiEsteri = null;
                                GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteri);
                                elencoStatiEsteri = elencoStatiEsteri.FindAll(x => x.Sigla == res.ICODRES).ToList<GestioneDecodifica.StatoEstero>();
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

        //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
        public static void ValorizzaDatiSentenza495_93(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, short categoria, out List<GestioneDanteCausa.DatiRedditoSentenza495_93> lDatiSentenze)
        {


            lDatiSentenze = null;
            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93))
            {
                if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                    AreaPrelievo.FinalResponse.Gruppo3.AreaSentenze != null && AreaPrelievo.FinalResponse.Gruppo3.AreaSentenze.SENTENZE != null && AreaPrelievo.FinalResponse.Gruppo3.AreaSentenze.SENTENZE.Count() > 0)
                {
                    lDatiSentenze = new List<GestioneDanteCausa.DatiRedditoSentenza495_93>();
                    foreach (Data.PCIINPU7.AreaSentenze.Sentenza sentenzaPrelievo in AreaPrelievo.FinalResponse.Gruppo3.AreaSentenze.SENTENZE)
                    {
                        GestioneDanteCausa.DatiRedditoSentenza495_93 sentenza = new GestioneDanteCausa.DatiRedditoSentenza495_93();
                        if (sentenzaPrelievo.ICISEN2 != 0 || sentenzaPrelievo.ICISEN2 != 0 || sentenzaPrelievo.ICISEN3M != 0)
                        {
                            if (sentenzaPrelievo.ICISEN1 == 1)
                                sentenza.FlagSentenza = true;
                            else
                                sentenza.FlagSentenza = false;
                            sentenza.CodiceSentenza = sentenzaPrelievo.ICISEN2;
                            sentenza.AnnoSentenza = sentenzaPrelievo.ICISEN3A;
                            sentenza.MeseSentenza = sentenzaPrelievo.ICISEN3M;

                            lDatiSentenze.Add(sentenza);
                        }
                    }
                }
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaVarie.IW8DEC_DC != 0 || areaVarie.IW8REDCON_DC != 0 || areaVarie.IW8RED_DC != 0)
                {
                    GestioneDanteCausa.DatiRedditoSentenza495_93 reddito = new GestioneDanteCausa.DatiRedditoSentenza495_93();
                    reddito.AnnoReddito = areaVarie.IW8DEC_DC != 0 ? areaVarie.IW8DEC_DC : (short?)null;
                    reddito.RedditoConiuge = areaVarie.IW8REDCON_DC != 0 ? areaVarie.IW8REDCON_DC : (decimal?)null;
                    reddito.RedditoTitolare = areaVarie.IW8RED_DC != 0 ? areaVarie.IW8RED_DC : (decimal?)null;

                    lDatiSentenze.Add(reddito);
                }
            }
        }

        public static void ValorizzaDatiIstruttoria(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, short categoria, out GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            datiIstruttoria = null;
            string codCat = string.Empty;

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                (AreaPrelievo.FinalResponse.Gruppo1.AreaTP12 != null || AreaPrelievo.FinalResponse.Gruppo1.AreaDati != null ||
                AreaPrelievo.FinalResponse.Gruppo1.AreaW1L != null || AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null))
            {
                datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();

                Data.PCIINPU7.AreaTP12 areaTP12 = AreaPrelievo.FinalResponse.Gruppo1.AreaTP12;
                Data.PCIINPU7.AreaDati areaDati = AreaPrelievo.FinalResponse.Gruppo1.AreaDati;
                Data.PCIINPU7.AreaW1L areaW1L = AreaPrelievo.FinalResponse.Gruppo1.AreaW1L;
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;


                if (areaTP12 != null)
                {
                    if (!String.IsNullOrEmpty(areaTP12.TP1CDCM))
                    {
                        List<GestioneDecodifica.CDCMMR> elencoCDCMMR = null;
                        GestioneDecodifica.GetCodiciCDCMMR(out elencoCDCMMR);
                        if (elencoCDCMMR != null && elencoCDCMMR.Count > 0)
                        {
                            GestioneDecodifica.CDCMMR codiceCDCMMR = elencoCDCMMR.Find(x => x.Descrizione.Trim().ToUpperInvariant() == areaTP12.TP1CDCM.Trim().ToUpperInvariant());
                            if (codiceCDCMMR != null)
                                datiIstruttoria.CodiceCdCmMr = codiceCDCMMR.Id;
                        }
                    }
                    if (areaTP12.TP1CORIC != 0)
                        datiIstruttoria.CodiceDomandaRicorso = Utility.StringToNullableByte(areaTP12.TP1CORIC.ToString());
                    datiIstruttoria.ScadenzaRevisioneSanitaria = Utility.DataFromInt(areaTP12.TP1REVA, areaTP12.TP1REVM, 1);

                    if (areaTP12.TP1CLIV1 != 0)
                        datiIstruttoria.ClasseInvalidita1Codice = areaTP12.TP1CLIV1;

                    if (areaTP12.TP1CLIV2 != 0)
                        datiIstruttoria.ClasseInvalidita2Codice = areaTP12.TP1CLIV2;
                }

                if (areaDati != null)
                {
                    if (areaDati.IW1CODOPZ != 0)
                        datiIstruttoria.CodiceOpzioneRiliquidazione = Utility.StringToNullableByte(areaDati.IW1CODOPZ.ToString());
                    datiIstruttoria.DataDomandaOpzione = Utility.DataFromInt(areaDati.IW1OPZAN, areaDati.IW1OPZMM, areaDati.IW1OPZGG);
                    if (areaDati.TP1CONTRATTO != 0)
                        datiIstruttoria.CodiceContrattoEquiparato = areaDati.TP1CONTRATTO;
                    if (areaDati.TP1LIVELLO != 0)
                        datiIstruttoria.CodiceLivelloEquip = areaDati.TP1LIVELLO;
                    if (areaDati.TP1MOBILITA != 0)
                        datiIstruttoria.CodiceMobilita = Utility.StringToNullableByte(areaDati.TP1MOBILITA.ToString());
                    if (areaDati.TP1REQRID != 0)
                        datiIstruttoria.Legge44997 = Utility.StringToNullableByte(areaDati.TP1REQRID.ToString());
                }

                if (areaW1L != null)
                {
                    datiIstruttoria.DecorrenzaOpzione = Utility.DataFromInt(areaW1L.IW1DEOPA, areaW1L.IW1DEOPM, 1);
                    GestioneDecodifica.GetCodCategoriaBySiglaCategoria(areaW1L.IW1CAT8, out codCat);
                    if (areaW1L.IW1CRIRIL != 0)
                        datiIstruttoria.RiliquidazionePostCristallizzazione = Utility.StringToNullableChar(areaW1L.IW1CRIRIL.ToString());
                }

                if (areaVarie != null)
                {
                    if (areaVarie.COD_C_OPERATIVO != 0)
                        datiIstruttoria.CodiceCentroOperativo = areaVarie.COD_C_OPERATIVO.ToString();
                    if (areaVarie.IREQPARD != 0)
                        datiIstruttoria.CodiceRequisitiParticolari = Utility.StringToNullableByte(areaVarie.IREQPARD.ToString());
                    if (areaVarie.IADASS != 0M)
                        datiIstruttoria.ImportoAdeguataAoi = areaVarie.IADASS;
                    datiIstruttoria.DecorrenzaOriginariaAltraPensione = Utility.DataFromInt(areaVarie.IDECASSA, areaVarie.IDECASSM, 1);
                    if (areaVarie.IIMPASS != 0M)
                        datiIstruttoria.ImportoPagamentoAoi = areaVarie.IIMPASS;

                    if (!String.IsNullOrEmpty(areaVarie.ESEFIS_EST))
                        datiIstruttoria.CodiceComunicazioneCampo4 = areaVarie.ESEFIS_EST.Trim().ToUpperInvariant() == "SI" ? 2 : (byte?)null;
                    if (!String.IsNullOrEmpty(areaVarie.ESEFIS_TERR))
                        datiIstruttoria.CodiceComunicazioneCampo4 = areaVarie.ESEFIS_TERR.Trim().ToUpperInvariant() == "SI" ? 1 : (byte?)null;

                    //ENG - Implementata la gestione mancante per le Reversibilità
                    //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                    {
                        if (areaVarie.I_AGGANCIO != null)
                            datiIstruttoria.I_AGGANCIO = Utility.StringToNullableChar(areaVarie.I_AGGANCIO);

                        if (areaVarie.I_SETTEST != 0)
                            datiIstruttoria.I_SETTEST = areaVarie.I_SETTEST;
                    }
                }
            }

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                (AreaPrelievo.FinalResponse.Gruppo3.AreaEX_W240 != null || AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaContributi != null || AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio != null || AreaPrelievo.FinalResponse.Gruppo3.AreaAltriCampi != null))
            {
                if (datiIstruttoria == null)
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();

                Data.PCIINPU7.AreaEX_W240 areaEX_W240 = AreaPrelievo.FinalResponse.Gruppo3.AreaEX_W240;
                Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati = AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati;
                Data.PCIINPU7.AreaContributi areaContributi = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi;
                Data.PCIINPU7.AreaSpazio areaSpazio = AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio;
                Data.PCIINPU7.AreaAltriCampi areaAltriCampi = AreaPrelievo.FinalResponse.Gruppo3.AreaAltriCampi;

                if (areaEX_W240 != null)
                {
                    if (!String.IsNullOrEmpty(areaEX_W240.IGP1AJ11))
                    {
                        List<GestioneDecodifica.CodiceParticolare> elencoCodiciParticolari = null;
                        GestioneDecodifica.GetCodiciParticolari(out elencoCodiciParticolari);
                        if (elencoCodiciParticolari != null && elencoCodiciParticolari.Count > 0)
                        {
                            if (!String.IsNullOrEmpty(codCat))
                            {
                                GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiciParticolari.Find(x =>
                                    x.TraduzioneSuGp == Utility.StringToNullableByte(areaEX_W240.IGP1AJ11) && x.CodCategoria.Trim().ToUpperInvariant() == codCat.Trim().ToUpperInvariant());
                                if (codiceParticolare != null)
                                    datiIstruttoria.CodiceParticolareSoggettoDerogato = codiceParticolare.Id;
                            }
                        }
                    }
                }

                if (areaUlterioriDati != null)
                {
                    datiIstruttoria.CodiceP18PrecedentePensione = Utility.StringToNullableShort(areaUlterioriDati.PRECCAT);
                    datiIstruttoria.CertificatoPrecedentePensione = Utility.StringToNullableInt(areaUlterioriDati.PRECCER);
                    datiIstruttoria.SedePrecedentePensione = Utility.StringToNullableShort(areaUlterioriDati.PRECSEDE);
                }

                if (areaContributi != null)
                {
                    datiIstruttoria.NContributiVolontari = areaContributi.TP1NUB;
                    datiIstruttoria.NSettGodimentoAssegno = areaContributi.IABNSASS;
                    if (!string.IsNullOrEmpty(codCat) && new List<string> { "0004", "0005", "0006" }.Contains(codCat.Trim()))
                        datiIstruttoria.NSettimaneOBG = areaContributi.TP1NUA;
                }

                if (areaSpazio != null)
                {
                    datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi = areaSpazio.ISETAUTVV_D;
                    datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi = areaSpazio.ISETAUTVV_M;
                }

                if (areaAltriCampi != null)
                {
                    if (areaAltriCampi.NRICONOSC != 0)
                        datiIstruttoria.NRiconoscimentiInvalidita = Utility.StringToNullableByte(areaAltriCampi.NRICONOSC.ToString());
                }
            }
        }

        public static void ValorizzaDatiDelegato(Data.GACI AreaPrelievo, out DatiDelegato datiDelegato)
        {
            datiDelegato = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaDelegato != null)
            {
                datiDelegato = new DatiDelegato();
                Data.PCIINPU7.AreaDelegato areaDelegato = AreaPrelievo.FinalResponse.Gruppo1.AreaDelegato;

                datiDelegato.CodiceDelegato = areaDelegato.D_TP1DTCOD;
                datiDelegato.CodiceFiscale = areaDelegato.D_TP1DTFISC;
            }
        }

        public static void ValorizzaDatiTutore(Data.GACI AreaPrelievo, out DatiTutore datiTutore)
        {
            datiTutore = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaTutore != null)
            {
                datiTutore = new DatiTutore();
                Data.PCIINPU7.AreaTutore areaTutore = AreaPrelievo.FinalResponse.Gruppo1.AreaTutore;

                datiTutore.CodiceTutore = areaTutore.T_TP1DTCOD;
                datiTutore.CodiceFiscale = areaTutore.T_TP1DTFISC;
            }
        }

        public static void ValorizzaDatiVittimeTerrorismo(Data.GACI AreaPrelievo, out GestioneVittimeTerrorismo.DatiVittimeTerrorismo datiVittimeTerrorismo)
        {
            datiVittimeTerrorismo = null;
        }

        public static void ValorizzaDatiStatiCivili(Data.GACI AreaPrelievo, out List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCivili)
        {
            ListaStatiCivili = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null)
            {
                ListaStatiCivili = new List<GestioneAnagrafica.DatiStatoCivile>();
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaVarie.VARSTATICIVILI != null && areaVarie.VARSTATICIVILI.Count > 0)
                {
                    foreach (Data.PCIINPU7.AreaVarie.VarStatiCivili stCiv in areaVarie.VARSTATICIVILI)
                    {
                        if (stCiv.DECSCIVA != 0 && stCiv.DECSCIVM != 0 && stCiv.CODSCIV != 0.ToString())
                        {
                            GestioneAnagrafica.DatiStatoCivile statoCivile = new GestioneAnagrafica.DatiStatoCivile();
                            statoCivile.Decorrenza = Utility.DataFromInt(stCiv.DECSCIVA, stCiv.DECSCIVM, 1);
                            if (stCiv.CODSCIV != 0.ToString())
                                statoCivile.Codice = stCiv.CODSCIV[0];

                            ListaStatiCivili.Add(statoCivile);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiPensioniCiDatiGenerici(Data.GACI AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, short categoria, out GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici)
        {
            datiGenerici = null;
            string codCat = string.Empty;

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                (AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null || AreaPrelievo.FinalResponse.Gruppo1.AreaW1L != null ||
                AreaPrelievo.FinalResponse.Gruppo1.AreaW2CL != null))
            {
                datiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;
                Data.PCIINPU7.AreaW1L areaW1L = AreaPrelievo.FinalResponse.Gruppo1.AreaW1L;
                Data.PCIINPU7.AreaW2CL areaW2CL = AreaPrelievo.FinalResponse.Gruppo1.AreaW2CL;

                if (areaW1L != null)
                {
                    datiGenerici.ConiugeSuperstite = (byte)areaW1L.IW1NOAF86;
                    GestioneDecodifica.GetCodCategoriaBySiglaCategoria(areaW1L.IW1CAT8, out codCat);
                }

                if (areaVarie != null)
                {
                    datiGenerici.CodiceVirtuale = Utility.StringToNullableChar(areaVarie.ICODVIRT);
                    if (!string.IsNullOrEmpty(areaVarie.IDEL126))
                        datiGenerici.DeliberaCee126 = areaVarie.IDEL126.Trim().ToUpperInvariant() == "S";
                    datiGenerici.DecorrenzaBonus = Utility.DataFromInt(areaVarie.IW1DEBONA, areaVarie.IW1DEBONM, 1);

                    if (areaVarie.IIMPASSEST != 0M)
                        datiGenerici.ImportoPensioneEEInvalido = areaVarie.IIMPASSEST;

                    datiGenerici.ApplicazioneSentenza49593 = Utility.StringToNullableChar(areaVarie.IW1C495);

                    if (!string.IsNullOrEmpty(areaVarie.COD_RIDUZIONE) && areaVarie.COD_RIDUZIONE.Trim() != string.Empty)
                    {
                        datiGenerici.RiduzioneRetributiva = areaVarie.COD_RIDUZIONE == "S";
                        datiGenerici.RiduzioneRetributivaPercentuale = areaVarie.PER_RIDUZIONE != 0 ? areaVarie.PER_RIDUZIONE : (decimal?)null;
                    }
                    //ENG - Implementata la gestione mancante per le Reversibilità
                    //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                    {
                        datiGenerici.ImportoMensilePensioneEstera = areaVarie.IAPIMPO_DC;
                    }
                }

                if (areaW2CL != null)
                {
                    datiGenerici.RegimeLiquidazione = Utility.StringToNullableChar(areaW2CL.ICI2REGLIQ);
                }
            }

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                (AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R != null || AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaAltriCampi != null || AreaPrelievo.FinalResponse.Gruppo3.AreaContributi != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaContributi503 != null || AreaPrelievo.FinalResponse.Gruppo3.AreaContributi335 != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio != null || AreaPrelievo.FinalResponse.Gruppo3.AreaCodiciStampa != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R != null))
            {
                if (datiGenerici == null)
                    datiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                Data.PCIINPU7.AreaWK1R areaWK1R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R;
                Data.PCIINPU7.AreaW2CIR areaW2CIR = AreaPrelievo.FinalResponse.Gruppo3.AreaW2CIR;
                Data.PCIINPU7.AreaAltriCampi areaAltriCampi = AreaPrelievo.FinalResponse.Gruppo3.AreaAltriCampi;
                Data.PCIINPU7.AreaContributi areaContributi = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi;
                Data.PCIINPU7.AreaContributi503 areaContributi503 = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi503;
                Data.PCIINPU7.AreaContributi335 areaContributi335 = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi335;
                Data.PCIINPU7.AreaSpazio areaSpazio = AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio;
                Data.PCIINPU7.AreaCodiciStampa areaCodiciStampa = AreaPrelievo.FinalResponse.Gruppo3.AreaCodiciStampa;
                Data.PCIINPU7.AreaWK2R areaWK2R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R;

                if (areaWK1R != null)
                {
                    datiGenerici.DecorrenzaArt2Dpcm = Utility.DataFromInt(areaWK1R.IW1DDPCMA, areaWK1R.IW1DDPCMM, 1);
                    datiGenerici.RMS9090 = areaWK1R.IW1RMSAR2;
                    datiGenerici.RMS8888 = areaWK1R.IW1RMSS72;
                }

                if (areaW2CIR != null)
                {
                    datiGenerici.ImportoCristallizzazione3481 = areaW2CIR.ICI2IMPCRIS34;
                }

                if (areaAltriCampi != null)
                {
                    datiGenerici.DecorrenzaCodiceVirtuale = Utility.DataFromInt(areaAltriCampi.IDECNAT3A, areaAltriCampi.IDECNAT3M, 1);
                    datiGenerici.CodiceBloccoArretratiEE = areaAltriCampi.TRESTERO == 1;
                    //if(areaAltriCampi.TRESTERO != 0)
                    //    datiGenerici.CodiceBloccoArretratiEE = Utility.StringToNullableShort(areaAltriCampi.TRESTERO.ToString());

                    //datiGenerici.UfficioPagatoreArretratiEE = areaAltriCampi.TRESTEROUP;
                    datiGenerici.UfficioPagatoreArretratiEE = Utility.GetIdFromUfficioPagatore(areaAltriCampi.TRESTEROUP);
                }

                if (areaContributi != null)
                {
                    datiGenerici.SettimanePerCalcoloContributivo = areaContributi.I1SETIVS;
                    datiGenerici.NSettFittiziePrepensionamento = areaContributi.ICI2SETFIT;
                    datiGenerici.NContributiItalia = areaContributi.IW1FFAA;
                    datiGenerici.ImportoIVS = areaContributi.IW1IVSTOT;
                    datiGenerici.VVMisuraAl1292 = areaContributi.IW1VVMISURA;
                    datiGenerici.AnniDifferimento = areaContributi.TP1DIFN != 0 ? areaContributi.TP1DIFN : (int?)null;
                    if (!string.IsNullOrEmpty(codCat) && !new List<string> { "0004", "0005", "0006" }.Contains(codCat.Trim()))
                        datiGenerici.SettimaneItalianeDiritto = areaContributi.TP1NUA;
                }

                if (areaContributi503 != null)
                {
                    datiGenerici.VVMisuraDL50392 = areaContributi503.ICI1VVOBG;
                }

                if (areaContributi335 != null)
                {
                    datiGenerici.CMSM = areaContributi335.ICIMMF;
                }

                if (areaSpazio != null)
                {
                    datiGenerici.ContributiItalianiEdEsteriAl1295 = areaSpazio.ITOT_EST_95;
                }

                if (areaCodiciStampa != null)
                {
                    if (!string.IsNullOrEmpty(areaCodiciStampa.CI281))
                    {
                        datiGenerici.CodiciMotivazioniCi281 = areaCodiciStampa.CI281.Trim();
                        //ENG - Gestione Nuovo Codice CI28
                        datiGenerici.CodiceCI28 = Utility.StringToNullableChar(areaCodiciStampa.CI281);
                    }
                    if (!string.IsNullOrEmpty(areaCodiciStampa.CI21) && areaCodiciStampa.CI21 != "0")
                        datiGenerici.CodiciCi21 = Utility.StringToNullableChar(areaCodiciStampa.CI21);
                }
            }
        }

        public static void ValorizzaDatiPensioniCIImportiValuta(Data.GACI AreaPrelievo, out List<GestioneDatiContributiviCi.PensioniCiImportiValuta> ListaImportiValuta)
        {
            ListaImportiValuta = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null)
            {
                ListaImportiValuta = new List<GestioneDatiContributiviCi.PensioniCiImportiValuta>();
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaVarie.IMPORTIESTERI != null && areaVarie.IMPORTIESTERI.Count > 0)
                {
                    foreach (Data.PCIINPU7.AreaVarie.ImportiEsteri impEst in areaVarie.IMPORTIESTERI)
                    {
                        if (impEst.IMPESTL != 0)
                        {
                            GestioneDatiContributiviCi.PensioniCiImportiValuta importoValuta = new GestioneDatiContributiviCi.PensioniCiImportiValuta();
                            importoValuta.DecorrenzaPrestazioneEE = Utility.DataFromInt(impEst.DECESTLA, impEst.DECESTLM, 1);
                            if (impEst.IMPESTL != 0)
                                importoValuta.ImportoPrestazioneEE = impEst.IMPESTL;

                            ListaImportiValuta.Add(importoValuta);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiIntegrazioneArt11(Data.GACI AreaPrelievo, out GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11)
        {
            datiIntegrazioneArt11 = null;


            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                (AreaPrelievo.FinalResponse.Gruppo3.AreaContributi != null || AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R != null))
            {
                datiIntegrazioneArt11 = new GestioneIntegrazioneArt11.IntegrazioneArt11();
                Data.PCIINPU7.AreaContributi areaContributi = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi;
                Data.PCIINPU7.AreaWK1R areaWK1R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R;

                if (areaContributi.IABAR11VV != 0M)
                    datiIntegrazioneArt11.ImportoIVS = areaContributi.IABAR11VV;

                if (areaWK1R.IW1A11S72 != 0M)
                    datiIntegrazioneArt11.ImportoIVS = areaWK1R.IW1A11S72;
            }
        }

        public static void ValorizzaDatiCalcoloContributivoEstero(Data.GACI AreaPrelievo, out List<GestioneCalcolo.DatiCalcoloContributivoEstero> ListaCalcoloContributivoEstero)
        {
            ListaCalcoloContributivoEstero = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                AreaPrelievo.FinalResponse.Gruppo3.AreaSettimaneEst != null)
            {
                ListaCalcoloContributivoEstero = new List<GestioneCalcolo.DatiCalcoloContributivoEstero>();
                Data.PCIINPU7.AreaSettimaneEst areaSettimaneEst = AreaPrelievo.FinalResponse.Gruppo3.AreaSettimaneEst;

                if (areaSettimaneEst.SETTIMANEESTERE != null && areaSettimaneEst.SETTIMANEESTERE.Count > 0)
                {
                    foreach (Data.PCIINPU7.AreaSettimaneEst.SettimaneEstere settEst in areaSettimaneEst.SETTIMANEESTERE)
                    {
                        if (settEst.SETRI233 != 0)
                        {
                            GestioneCalcolo.DatiCalcoloContributivoEstero settimaneEstere = new GestioneCalcolo.DatiCalcoloContributivoEstero();
                            settimaneEstere.Decorrenza = Utility.DataFromInt(settEst.DEC233A, settEst.DEC233M, 1);
                            if (settEst.SETRI233 != 0)
                                settimaneEstere.Settimane = settEst.SETRI233;
                            if (settEst.GEST233 != 0)
                            {
                                List<GestioneDecodifica.CodeGestione> listaCodiciGestione = null;
                                GestioneDecodifica.GetCodiceGestione(out listaCodiciGestione);
                                if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                                {
                                    GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.TraduzioneSuGP == settEst.GEST233);
                                    if (codeGestione != null)
                                        settimaneEstere.CodiceGestione = codeGestione.Id;
                                }
                            }
                            ListaCalcoloContributivoEstero.Add(settimaneEstere);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiPensioniCiMaternitaAcna(Data.GACI AreaPrelievo, out List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> ListaPensioniCiMaternitaAcna)
        {
            ListaPensioniCiMaternitaAcna = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio != null)
            {
                ListaPensioniCiMaternitaAcna = new List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna>();
                Data.PCIINPU7.AreaSpazio areaSpazio = AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio;

                if (areaSpazio.IIVSCEN1 != 0M || areaSpazio.ISETCEN1 != 0 || areaSpazio.ISETCEN2 != 0)
                {
                    GestioneDatiContributiviCi.PensioniCiMaternitaAcna acna = new GestioneDatiContributiviCi.PensioniCiMaternitaAcna();
                    acna.Tipo = 'A';
                    acna.ImportoIVS = areaSpazio.IIVSCEN1;
                    acna.SettimaneAl1292 = areaSpazio.ISETCEN1;
                    acna.SettimaneDL50392 = areaSpazio.ISETCEN2;
                    ListaPensioniCiMaternitaAcna.Add(acna);
                }
                if (areaSpazio.IIVSMAT1 != 0M || areaSpazio.ISETMAT1 != 0 || areaSpazio.ISETMAT2 != 0)
                {
                    GestioneDatiContributiviCi.PensioniCiMaternitaAcna maternita = new GestioneDatiContributiviCi.PensioniCiMaternitaAcna();
                    maternita.Tipo = 'M';
                    maternita.ImportoIVS = areaSpazio.IIVSMAT1;
                    maternita.SettimaneAl1292 = areaSpazio.ISETMAT1;
                    maternita.SettimaneDL50392 = areaSpazio.ISETMAT2;
                    ListaPensioniCiMaternitaAcna.Add(maternita);
                }

            }
        }

        public static void ValorizzaDatiNuoveLiquidate(Data.GACI AreaPrelievo, out GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate)
        {
            datiNuoveLiquidate = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio != null)
            {
                datiNuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();
                Data.PCIINPU7.AreaSpazio areaSpazio = AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio;

                if (!String.IsNullOrEmpty(areaSpazio.OPZIONE_CONTRIBUTIVA))
                {
                    datiNuoveLiquidate.FlagContributiva = areaSpazio.OPZIONE_CONTRIBUTIVA.Trim().ToUpperInvariant() == "S" ? true : areaSpazio.OPZIONE_CONTRIBUTIVA.Trim().ToUpperInvariant() == "N" ? false : (bool?)null;
                }
            }
        }

        public static void ValorizzaDatiStatiEsteri(Data.GACI AreaPrelievo, out List<GestioneContrib.StatoEstero> ListaStatiEsteri)
        {
            ListaStatiEsteri = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo4 != null &&
                AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2004 != null)
            {
                ListaStatiEsteri = new List<GestioneContrib.StatoEstero>();
                Data.PCIINPU7.AreaCampi2004 areaCampi2004 = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2004;

                if (areaCampi2004.STATIESTERI != null && areaCampi2004.STATIESTERI.Count > 0)
                {
                    foreach (Data.PCIINPU7.AreaCampi2004.StatoEstero stEst in areaCampi2004.STATIESTERI)
                    {
                        if (stEst.STATO != 0)
                        {
                            GestioneContrib.StatoEstero statoEstero = new GestioneContrib.StatoEstero();
                            statoEstero.PrestazioneEstera = new GestioneContrib.PrestazioneEstera();

                            //ENG - CodiceConvenzione valorizzato per tutte le prestazioni estere (non solo per il primo stato)
                            Data.PCIINPU7.AreaW2CL areaW2CL = AreaPrelievo.FinalResponse.Gruppo1.AreaW2CL;
                            statoEstero.PrestazioneEstera.CodiceConvenzione = (byte)areaW2CL.ICI2CONV;

                            statoEstero.PrestazioneEstera.CodiceArt48 = Utility.StringToNullableChar(stEst.ART48);
                            statoEstero.PrestazioneEstera.SospensioneCautelativaIntegrazione = Utility.StringToNullableChar(stEst.COD_SOSP_ESTERO);
                            statoEstero.PrestazioneEstera.DecorrenzaArt48 = Utility.DataFromInt(stEst.DECART48A, stEst.DECART48M, 1);
                            statoEstero.PrestazioneEstera.EtaSospensione = Utility.StringToNullableByte(stEst.ETA_SOSP_ESTERO.ToString());
                            statoEstero.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE = Utility.DataFromInt(stEst.IDAPLIQA, stEst.IDAPLIQM, 1);
                            if (stEst.ISTIT != 0)
                                statoEstero.PrestazioneEstera.CodiceIstituzione = stEst.ISTIT.ToString().PadLeft(4, '0');
                            statoEstero.PrestazioneEstera.MatricolaIstituzioneEE = stEst.MATRIC;
                            statoEstero.PrestazioneEstera.DecorrenzaRicalcolo = Utility.DataFromInt(stEst.RICALSTATOA, stEst.RICALSTATOM, 1);
                            statoEstero.PrestazioneEstera.ContributiEEDecorrenzaOriginaria = stEst.SETT1;
                            statoEstero.PrestazioneEstera.ContributiEERicalcolo = stEst.SETT2;
                            statoEstero.PrestazioneEstera.ContributiEEDiritto = stEst.SETTDIR;
                            if (stEst.STATO != 0)
                                statoEstero.PrestazioneEstera.CodiceStatoEE = stEst.STATO.ToString().PadLeft(2, '0');

                            if (stEst.IMPORTI != null && stEst.IMPORTI.Count > 0)
                            {
                                statoEstero.ElencoImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();
                                foreach (Data.PCIINPU7.AreaCampi2004.StatoEstero.Importo imp in stEst.IMPORTI)
                                {
                                    if (imp.IMPEST != 0M)
                                    {
                                        GestioneDatiContributiviCi.PensioniCiImportiEsteri importo = new GestioneDatiContributiviCi.PensioniCiImportiEsteri();
                                        importo.CessazionePrestazioneEE = Utility.DataFromInt(imp.CESAA, imp.CESMM, 1);
                                        importo.DecorrenzaPrestazioneEE = Utility.DataFromInt(imp.DECAA, imp.DECMM, 1);
                                        importo.ImportoPrestazioneEE = imp.IMPEST;

                                        if (importo.DecorrenzaPrestazioneEE.HasValue)
                                            statoEstero.ElencoImportiEsteri.Add(importo);
                                    }
                                }
                            }

                            ListaStatiEsteri.Add(statoEstero);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiEliminazione(Data.GACI AreaPrelievo, out GestionePensione.DatiEliminazione datiEliminazione)
        {
            datiEliminazione = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                (AreaPrelievo.FinalResponse.Gruppo1.AreaTP12 != null || AreaPrelievo.FinalResponse.Gruppo1.AreaDati != null || AreaPrelievo.FinalResponse.Gruppo1.AreaW1L != null))
            {
                datiEliminazione = new GestionePensione.DatiEliminazione();

                Data.PCIINPU7.AreaDati areaDati = AreaPrelievo.FinalResponse.Gruppo1.AreaDati;
                Data.PCIINPU7.AreaW1L areaW1L = AreaPrelievo.FinalResponse.Gruppo1.AreaW1L;
                Data.PCIINPU7.AreaTP12 areaTP12 = AreaPrelievo.FinalResponse.Gruppo1.AreaTP12;

                if (areaTP12 != null)
                {
                    datiEliminazione.DecorrenzaEliminazione = Utility.DataFromInt(areaTP12.TP1ELIMA, areaTP12.TP1ELIMM, 1);
                }

                if (areaDati != null)
                {
                    if (!string.IsNullOrEmpty(areaDati.TP1CODELIM))
                    {
                        List<GestioneDecodifica.CodiceEliminazione> lstDecCodElim;
                        GestioneDecodifica.GetCodiceEliminazioneByTipologia(out lstDecCodElim, Utility.TipoAppartenenza.CI);
                        datiEliminazione.CodiceMotivo = byte.Parse(lstDecCodElim.Find(x => x.TraduzioneSuGP == Utility.StringToNullableChar(areaDati.TP1CODELIM)).Id);
                    }
                }

                if (areaW1L != null)
                {
                    datiEliminazione.DataFineCalcoloArretrati = Utility.DataFromInt(areaW1L.IW1DA2A, areaW1L.IW1DA2M, 1);
                }
            }

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
                AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati != null)
            {
                if (datiEliminazione == null)
                    datiEliminazione = new GestionePensione.DatiEliminazione();

                Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati = AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati;

                if (areaUlterioriDati != null)
                {
                    datiEliminazione.DataEvento = Utility.DataFromInt(areaUlterioriDati.DECELIMA, areaUlterioriDati.DECELIMM, areaUlterioriDati.DECELIMG);
                }
            }
        }

        public static void ValorizzaDatiMaggiorazioni(Data.GACI AreaPrelievo, ref GestionePensione.DatiPensione datiPensione, GestionePrelievo.TipoDomanda tipoDomanda, short categoria, out Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            datiMaggiorazioniBenefici = null;
            List<GestioneDanteCausa.PensioniEstereDcBL> LpensioniEstereDcBL = null;
            GestioneDanteCausa.GetPensioniEstereDCByIdPensione(datiPensione.Id, out LpensioniEstereDcBL);

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                (AreaPrelievo.FinalResponse.Gruppo1.AreaDati != null || AreaPrelievo.FinalResponse.Gruppo1.AreaW1L != null || AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null))
            {

                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.PCIINPU7.AreaDati areaDati = AreaPrelievo.FinalResponse.Gruppo1.AreaDati;
                Data.PCIINPU7.AreaW1L areaW1L = AreaPrelievo.FinalResponse.Gruppo1.AreaW1L;
                Data.PCIINPU7.AreaVarie areaVarie = AreaPrelievo.FinalResponse.Gruppo1.AreaVarie;

                if (areaDati != null)
                {
                    if (areaDati.TP1USURA != 0)
                    {
                        datiMaggiorazioniBenefici.Attivitausuranti = true;
                    }
                }

                if (areaW1L != null)
                {
                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale = Utility.DataFromInt(areaW1L.IW1DEC544A, areaW1L.IW1DEC544M, 1);
                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 = Utility.DataFromInt(areaW1L.IW1DECEXA, areaW1L.IW1DECEXM, 1);
                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneLegge140 = Utility.DataFromInt(areaW1L.IW1DECMS1A, areaW1L.IW1DECMS1M, 1);
                    if (areaW1L.IW1TM59B != 0M)
                        datiMaggiorazioniBenefici.AumentoMensileLegge5991Comma9 = areaW1L.IW1TM59B;
                    //IW1CODEX
                    datiMaggiorazioniBenefici.Articolo6140 = (byte?)areaW1L.IW1CODEX;
                }

                if (areaVarie != null)
                {
                    datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale = Utility.DataFromInt(areaVarie.IW1CES544A, areaVarie.IW1CES544M, 1);
                }
            }

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null &&
               (AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R != null || AreaPrelievo.FinalResponse.Gruppo3.AreaContributi335 != null || AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio != null ||
                AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R != null || AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati != null))
            {
                Data.PCIINPU7.AreaWK1R areaWK1R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK1R;
                Data.PCIINPU7.AreaContributi335 areaContributi335 = AreaPrelievo.FinalResponse.Gruppo3.AreaContributi335;
                Data.PCIINPU7.AreaSpazio areaSpazio = AreaPrelievo.FinalResponse.Gruppo3.AreaSpazio;
                Data.PCIINPU7.AreaWK2R areaWK2R = AreaPrelievo.FinalResponse.Gruppo3.AreaWK2R;
                Data.PCIINPU7.AreaUlterioriDati areaUlterioriDati = AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati;

                if (areaWK1R != null)
                {
                    if (datiMaggiorazioniBenefici == null)
                        datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();

                    if (areaWK1R.IW1ADPCM != 0M)
                        datiMaggiorazioniBenefici.AumentoMensileLegge161289Art2 = areaWK1R.IW1ADPCM;

                    if (areaWK1R.IW1AS72A != 0M)
                        datiMaggiorazioniBenefici.Aumento7290 = areaWK1R.IW1AS72A;

                    if (areaWK1R.IW1TM409 != 0M)
                        datiMaggiorazioniBenefici.AumentoMensileLegge5991Comma2 = areaWK1R.IW1TM409;

                    //ENG - Implementata la gestione mancante per le Reversibilità
                    //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                    {
                        if (areaWK1R.IW1CM409 != 0)
                            datiMaggiorazioniBenefici.Articolo1Legge5991 = Convert.ToBoolean(areaWK1R.IW1CM409);
                    }

                    if (areaWK1R.IW1TM345 != 0M && LpensioniEstereDcBL != null)
                    {
                        if (LpensioniEstereDcBL.Exists(x => x.CodiciVari == 3))
                            datiMaggiorazioniBenefici.ImportoComplessivoArt3 = areaWK1R.IW1TM345;
                        else if (LpensioniEstereDcBL.Exists(x => x.CodiciVari == 4))
                            datiMaggiorazioniBenefici.ImportoComplessivoArt4 = areaWK1R.IW1TM345;
                        else if (LpensioniEstereDcBL.Exists(x => x.CodiciVari == 5))
                            datiMaggiorazioniBenefici.ImportoComplessivoArt5 = areaWK1R.IW1TM345;
                        else if (LpensioniEstereDcBL.Exists(x => x.CodiciVari == 8))
                            datiMaggiorazioniBenefici.ImportoComplessivoArt1 = areaWK1R.IW1TM345;
                    }
                    //            
                }
                //ENG - Implementata la gestione mancante per le Reversibilità
                //al momento vengono prese in considerazione anche le Ric indirette perchè non si hanno le specifiche su come distinguere, in fase di prelievo, le due tipologie(RIC REVERSIBILITA' E RIC INDIRETTE)
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità || (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && (categoria == 6 || categoria == 87 || categoria == 90 || categoria == 93)))
                {
                    if (areaWK2R != null)
                    {
                        if (areaWK2R.IABMM409 != 0M)
                            datiMaggiorazioniBenefici.MensileLegge5991 = areaWK2R.IABMM409;
                    }
                }

                if (areaUlterioriDati != null)
                {
                    if (areaUlterioriDati.IW1AS72B != 0M)
                        datiMaggiorazioniBenefici.Aumento7290DC = areaUlterioriDati.IW1AS72B;
                }
                //
                if (areaContributi335 != null)
                {
                    if (datiMaggiorazioniBenefici == null)
                        datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();

                    if (areaContributi335.ICISET1X100 != 0)
                        datiMaggiorazioniBenefici.NSettimaneIncremento1Percento = areaContributi335.ICISET1X100;
                    if (areaContributi335.ICISET05X100 != 0)
                        datiMaggiorazioniBenefici.NSettimaneIncremento05Percento = areaContributi335.ICISET05X100;
                }
                if (areaSpazio != null)
                {
                    if (datiMaggiorazioniBenefici == null)
                        datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();

                    if (areaSpazio.ANNI_ANTICIPO_544 != 0)
                        datiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02 = areaSpazio.ANNI_ANTICIPO_544;
                    if (!string.IsNullOrEmpty(areaSpazio.IREQA2C3_385))
                    {
                        List<GestioneDecodifica.CodiceRequisitiLegge50392> listaCodiciRequisitiLegge50392 = null;
                        GestioneDecodifica.GetCodiceRequisitiLegge50392(out listaCodiciRequisitiLegge50392);
                        if (listaCodiciRequisitiLegge50392 != null && listaCodiciRequisitiLegge50392.Count > 0)
                        {
                            GestioneDecodifica.CodiceRequisitiLegge50392 appCodiceRequisitiLegge50392 = listaCodiciRequisitiLegge50392.Find(x => x.TraduzioneSuGP.Equals(areaSpazio.IREQA2C3_385[0]));
                            datiMaggiorazioniBenefici.CodiceRequisitiLegge50392Art2 = appCodiceRequisitiLegge50392 != null ? Utility.StringToNullableByte(appCodiceRequisitiLegge50392.Id) : (byte?)null;
                        }
                    }
                }

                if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo4 != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018 != null &&
                    AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Count > 0)
                {
                    List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri> listaOneri = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI;
                    if (listaOneri.Exists(x => x.FELPE_CODBENEF == "11"))
                        datiMaggiorazioniBenefici.TipoSettimaneBeneficio = "11";
                }

                if (datiMaggiorazioniBenefici != null)
                {
                    if (!datiMaggiorazioniBenefici.IsBeneficiCINull())
                    {
                        if (datiPensione == null)
                            datiPensione = new GestionePensione.DatiPensione();
                        datiPensione.Benefici = true;
                    }

                    if (!datiMaggiorazioniBenefici.IsExCombattenteCINull())
                    {
                        if (datiPensione == null)
                            datiPensione = new GestionePensione.DatiPensione();
                        datiPensione.ExCombattente = true;
                    }

                    if (!datiMaggiorazioniBenefici.IsMaggiorazioniCINull())
                    {
                        if (datiPensione == null)
                            datiPensione = new GestionePensione.DatiPensione();
                        datiPensione.Maggiorazioni = true;
                    }
                }
            }
        }

        public static void ValorizzaDatiOneri(Data.GACI AreaPrelievo, ref GestionePensione.DatiPensione datiPensione, out List<GestioneOneri.DatiOneri> ListaDatiOneri)
        {
            ListaDatiOneri = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo4 != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018 != null &&
                AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Count > 0)
            {
                ListaDatiOneri = new List<GestioneOneri.DatiOneri>();
                List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri> listaOneri = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI;

                foreach (Data.PCIINPU7.AreaCampi2018.Felpe_Oneri on in listaOneri)
                {
                    if (on.FELPE_CODGRUP != string.Empty)
                    {
                        //ENG - Introdotto controllo per CodGup pari a 0
                        int felpeCodGrup = 0;
                        Int32.TryParse(on.FELPE_CODGRUP, out felpeCodGrup);
                        if (felpeCodGrup != 0)
                        {
                            GestioneOneri.DatiOneri datiOneri = new GestioneOneri.DatiOneri();
                            datiOneri.Decorrenza = Utility.DataFromString(on.FELPE_DECONERE, Utility.FormatoData.AAAAmmGG);
                            datiOneri.Scadenza = Utility.DataFromString(on.FELPE_SCADENZA, Utility.FormatoData.AAAAmmGG);

                            List<GestioneDecodifica.GruppoOneri> elencoGruppoOneri = null;
                            GestioneDecodifica.GetGruppoOneri(out elencoGruppoOneri);
                            GestioneDecodifica.GruppoOneri gruppoOneri = elencoGruppoOneri.Find(x => x.Code == on.FELPE_CODGRUP);
                            if (gruppoOneri != null)
                            {
                                datiOneri.IdCodeGruppo = gruppoOneri.Id;
                            }

                            List<GestioneDecodifica.SottoGruppoOneri> elencoSottoGruppoOneri = null;
                            GestioneDecodifica.GetSottoGruppoOneri(out elencoSottoGruppoOneri);
                            GestioneDecodifica.SottoGruppoOneri sottoGruppoOneri = elencoSottoGruppoOneri.Find(x => x.Code == on.FELPE_CODSGRUP);
                            if (sottoGruppoOneri != null)
                            {
                                datiOneri.IdCodeSottoGruppo = sottoGruppoOneri.Id;
                            }

                            if (on.FELPE_ONERE != 0M)
                                datiOneri.Onere = on.FELPE_ONERE;
                            if (on.FELPE_ANZCON != 0)
                                datiOneri.Settimane = (short)on.FELPE_ANZCON;

                            //Solo per Precoci, Quota 100 e Quota 102
                            if ((on.FELPE_CODGRUP == "5000" || on.FELPE_CODGRUP == "5300" || on.FELPE_CODGRUP == "5800" || on.FELPE_CODGRUP == "6000" || on.FELPE_CODGRUP == "6100") && AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
                                AreaPrelievo.FinalResponse.Gruppo1.AreaTP12 != null)
                            {
                                datiOneri.ScadenzaBeneficio = Utility.DataFromInt(AreaPrelievo.FinalResponse.Gruppo1.AreaTP12.TP1REVA, AreaPrelievo.FinalResponse.Gruppo1.AreaTP12.TP1REVM, 1);
                            }
                            ListaDatiOneri.Add(datiOneri);
                        }
                    }
                }

                Data.PCIINPU7.AreaCampi2018.Felpe_Oneri oneri = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI[0];
                if (oneri != null)
                {
                    byte res = 0;
                    byte.TryParse(oneri.FELPE_GP2PBNFGL, out res);
                    datiPensione.NumeroFigli = res > 0 ? res : (byte?)null;
                }
            }
        }

        public static void ValorizzaDatiBeneficiParticolari(Data.GACI AreaPrelievo, out List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaDatiBeneficiParticolari)
        {
            ListaDatiBeneficiParticolari = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null && AreaPrelievo.FinalResponse.Gruppo1.AreaVarie != null &&
                AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI.Count > 0)
            {
                ListaDatiBeneficiParticolari = new List<GestioneBeneficiParticolari.DatiBeneficiParticolari>();
                List<Data.PCIINPU7.AreaCampi2018.Felpe_Oneri> listaOneri = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.FELPE_ONERI;

                foreach (Data.PCIINPU7.AreaCampi2018.Felpe_Oneri on in listaOneri)
                {
                    if (on.FELPE_CODBENEF != string.Empty)
                    {
                        GestioneBeneficiParticolari.DatiBeneficiParticolari datiBeneficiParticolari = new GestioneBeneficiParticolari.DatiBeneficiParticolari();
                        datiBeneficiParticolari.CodiceBenefici = on.FELPE_CODBENEF;
                        if (on.FELPE_ANZBENEF != 0)
                            datiBeneficiParticolari.Settimane = (short)on.FELPE_ANZBENEF;

                        ListaDatiBeneficiParticolari.Add(datiBeneficiParticolari);
                    }
                }
            }
        }

        public static void ValorizzaDatiBititolarita(Data.GACI AreaPrelievo, out List<GestioneAltrePensioni.AltraPensione> ListaBititolarita)
        {
            ListaBititolarita = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null && AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati != null)
            {
                ListaBititolarita = new List<GestioneAltrePensioni.AltraPensione>();
                if (AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE != null && AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE.Count > 0)
                {
                    for (int i = 0; i < AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE.Count; i++)
                    {
                        GestioneAltrePensioni.AltraPensione altraPensione = new GestioneAltrePensioni.AltraPensione();
                        altraPensione.Categoria = AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPCATEG;
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPNUMP != 0)
                            altraPensione.Certificato = AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPNUMP;
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPCESSAA != 0 && AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPCESSAM != 0)
                            altraPensione.Cessazione = Utility.DataFromInt(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPCESSAA, AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPCESSAM, 1);
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPCODIMP != 0)
                            altraPensione.CodiceImporto = Utility.StringToNullableChar(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPCODIMP.ToString());
                        if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPUNIC))
                            altraPensione.CodiceUC = char.Parse(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPUNIC);
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPDECORA != 0 && AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPDECORM != 0)
                            altraPensione.Decorrenza = Utility.DataFromInt(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPDECORA, AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPDECORM, 1);
                        if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPENTE))
                            altraPensione.Ente = (Utility.StringToNullableByte(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPENTE) != 0 ? Utility.StringToNullableByte(AreaPrelievo.FinalResponse.Gruppo3.AreaUlterioriDati.ALTRAPENSIONE[i].IAPENTE) : null);

                        if (!altraPensione.IsNull())
                            ListaBititolarita.Add(altraPensione);
                    }
                }
            }
        }

        public static void ValorizzaDatiPostDecOriginaria(Data.GACI AreaPrelievo, out List<GestioneContrib.DatiPostDecOriginaria> ListaDatiPostDecOriginaria)
        {
            ListaDatiPostDecOriginaria = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo3 != null && AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec != null)
            {
                ListaDatiPostDecOriginaria = new List<GestioneContrib.DatiPostDecOriginaria>();
                if (AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI != null && AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI.Count > 0)
                {
                    for (int i = 0; i < AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI.Count; i++)
                    {
                        GestioneContrib.DatiPostDecOriginaria datiPostDecOriginaria = new GestioneContrib.DatiPostDecOriginaria();
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IDECRICA != 0 && AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IDECRICM != 0)
                            datiPostDecOriginaria.Decorrenza = new DateTime(AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IDECRICA, AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IDECRICM, 01);
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].INSIVSRIC != 0)
                            datiPostDecOriginaria.CTR = AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].INSIVSRIC;
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IIVSRIC != 0)
                            datiPostDecOriginaria.IVS = AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IIVSRIC;
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].INSOBGRIC != 0)
                            datiPostDecOriginaria.SettimaneRetributive = AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].INSOBGRIC;
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].INSVVRIC != 0)
                            datiPostDecOriginaria.SettimaneVV = AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].INSVVRIC;
                        if (AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IRMSRIC != 0)
                            datiPostDecOriginaria.RMS = AreaPrelievo.FinalResponse.Gruppo3.AreaContributiPostDec.CONTRIBUTI[i].IRMSRIC;

                        if (!datiPostDecOriginaria.IsNull())
                            ListaDatiPostDecOriginaria.Add(datiPostDecOriginaria);
                    }
                }
            }
        }

        //ENG - Reversibilità: campi Inail
        public static void ValorizzaDatiINAIL(Data.GACI AreaPrelievo, out List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaDatiInail)
        {
            listaDatiInail = null;

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo4 != null && AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018 != null &&
                AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.AreaINAIL != null)
            {
                listaDatiInail = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();

                Data.PCIINPU7.AreaINAIL areaInail = AreaPrelievo.FinalResponse.Gruppo4.AreaCampi2018.AreaINAIL;

                if (areaInail != null && areaInail.RENDITAINAIL != null && areaInail.RENDITAINAIL.Count > 0)
                {
                    foreach (Data.PCIINPU7.AreaINAIL.RenditaINAIL renditaInail in areaInail.RENDITAINAIL)
                    {
                        if ((renditaInail.N_IDECINAA != 0 && renditaInail.N_IDECINAM != 0) || renditaInail.N_IIMPINAIL != 0M ||
                            (!String.IsNullOrEmpty(renditaInail.N_ICODINAIL) && renditaInail.N_ICODINAIL != "0"))
                        {
                            GestionePensioneInailInabilita.DatiPensioniINAIL inail = new GestionePensioneInailInabilita.DatiPensioniINAIL();
                            inail.DecorrenzaRenditaInail = Utility.DataFromInt(renditaInail.N_IDECINAA, renditaInail.N_IDECINAM, 1);
                            inail.Evento = renditaInail.N_ICODINAIL == "1" ? true : false;
                            inail.ImportoMensileInail = renditaInail.N_IIMPINAIL;
                            listaDatiInail.Add(inail);
                        }
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
            private DateTime? _DataMatrimonio;
            #endregion private properties

            #region public properties
            public int CodiceComuneInps { get { return _CodiceComuneInps; } set { _CodiceComuneInps = value; } }
            public string Cognome { get { return _Cognome; } set { _Cognome = value; } }
            public string Nome { get { return _Nome; } set { _Nome = value; } }
            public System.Nullable<char> Sesso { get { return _Sesso; } set { _Sesso = value; } }
            public System.Nullable<System.DateTime> DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }
            public DateTime? DataMatrimonio { get { return _DataMatrimonio; } set { _DataMatrimonio = value; } }
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
            #endregion private properties

            #region public properties
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public string CodiceTutore { get { return _CodiceTutore; } set { _CodiceTutore = value; } }
            #endregion public properties
        }
        #endregion nested classes
    }
}



