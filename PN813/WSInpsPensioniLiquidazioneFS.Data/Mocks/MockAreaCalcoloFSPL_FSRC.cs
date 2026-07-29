using System;
using System.Collections.Generic;

using INPS.DNA.Data.HostIntegration.HisMapper;

namespace INPS.Pensioni.LiquidazioneFs.Data.Mocks
{
    static public class MockAreaCalcoloFSPL_FSRC
    {
        static public void RitornaAreaCalcolo(FSPL_FSRC fspl_fsrc, List<String> ListaBlocchi)
        {
            fspl_fsrc.Request.LISTBLOCCO = new List<INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO>();
            INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO blocco = null;

            for (int i = 0; i < ListaBlocchi.Count; i++)
            {
                blocco = new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO();
                blocco.AR_ACCO = ListaBlocchi[i];
                fspl_fsrc.Request.LISTBLOCCO.Add(blocco);
            }

            //for (int i = fspl.Request.LISTBLOCCO.Count; i < 60; i++)
            //fspl_fsrc.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO());
        }

        /// <summary>
        /// Conversione area output
        /// </summary>
        static public void ConvertAreaDati(bool UtilizzaNuovoTracciato, ref byte[] areaSenzaDelimiter, ref List<INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO> blocchi, out Data.CMSGTRA.AreaVariabile FinalResponse)
        {

            FinalResponse = null;

            if (blocchi != null)
            {
                int offset = 0;
                int lunghezza = 0;
                Byte[] dest = null;
                FinalResponse = new Data.CMSGTRA.AreaVariabile();

                for (int i = 0; i < blocchi.Count; i++)
                {
                    if (String.IsNullOrEmpty(blocchi[i].AR_ACCO))
                        break;

                    switch (blocchi[i].AR_ACCO.ToUpperInvariant())
                    {
                        case "A":
                            lunghezza = 344;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.Anagrafica anagrafica = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Anagrafica();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.Anagrafica>(anagrafica, dest);
                            if (FinalResponse.ListaAnagrafica == null)
                                FinalResponse.ListaAnagrafica = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Anagrafica>();
                            FinalResponse.ListaAnagrafica.Add(anagrafica);

                            break;
                        case "B":
                            lunghezza = 150;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.DelegatoNew delegato = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DelegatoNew();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.DelegatoNew>(delegato, dest);
                            if (FinalResponse.ListaDelegato == null)
                                FinalResponse.ListaDelegato = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DelegatoNew>();
                            FinalResponse.ListaDelegato.Add(delegato);
                            break;
                        case "C":
                            lunghezza = 260;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.Familiare familiare = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.Familiare>(familiare, dest);
                            if (FinalResponse.ListaFamiliare == null)
                                FinalResponse.ListaFamiliare = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare>();
                            FinalResponse.ListaFamiliare.Add(familiare);
                            break;
                        case "D":
                            lunghezza = 150;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.DanteCausa danteCausa = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DanteCausa();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.DanteCausa>(danteCausa, dest);
                            if (FinalResponse.ListaDanteCausa == null)
                                FinalResponse.ListaDanteCausa = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DanteCausa>();
                            FinalResponse.ListaDanteCausa.Add(danteCausa);
                            break;
                        case "E":
                            lunghezza = 631;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.Supplementi supplementi = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Supplementi();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.Supplementi>(supplementi, dest);
                            if (FinalResponse.ListaSupplementi == null)
                                FinalResponse.ListaSupplementi = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Supplementi>();
                            FinalResponse.ListaSupplementi.Add(supplementi);
                            break;
                        case "F":
                            lunghezza = 763;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.TrattamentiFamiglia trattamentiFamiglia = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.TrattamentiFamiglia>(trattamentiFamiglia, dest);
                            if (FinalResponse.ListaTrattamentiFamiglia == null)
                                FinalResponse.ListaTrattamentiFamiglia = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia>();
                            FinalResponse.ListaTrattamentiFamiglia.Add(trattamentiFamiglia);
                            break;
                        case "G":
                            lunghezza = 872;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.Minimo_PensInv minimo_PensInv = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Minimo_PensInv();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.Minimo_PensInv>(minimo_PensInv, dest);
                            if (FinalResponse.ListaMinimo_PensInv == null)
                                FinalResponse.ListaMinimo_PensInv = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Minimo_PensInv>();
                            FinalResponse.ListaMinimo_PensInv.Add(minimo_PensInv);
                            break;
                        case "H":
                            lunghezza = 607;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.Residenza residenza = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.Residenza>(residenza, dest);
                            if (FinalResponse.ListaResidenza == null)
                                FinalResponse.ListaResidenza = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza>();
                            FinalResponse.ListaResidenza.Add(residenza);
                            break;
                        case "I":
                            lunghezza = 54;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.MaggiorazioneLegge maggiorazioneLegge = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.MaggiorazioneLegge();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.MaggiorazioneLegge>(maggiorazioneLegge, dest);
                            if (FinalResponse.ListaMaggiorazioneLegge == null)
                                FinalResponse.ListaMaggiorazioneLegge = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.MaggiorazioneLegge>();
                            FinalResponse.ListaMaggiorazioneLegge.Add(maggiorazioneLegge);
                            break;
                        case "K":
                            lunghezza = 2000;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.Deleghe_Tutele delegheTutele = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Deleghe_Tutele();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.Deleghe_Tutele>(delegheTutele, dest);
                            if (FinalResponse.ListaDelegheTutele == null)
                                FinalResponse.ListaDelegheTutele = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Deleghe_Tutele>();
                            FinalResponse.ListaDelegheTutele.Add(delegheTutele);
                            break;
                        case "L":
                            lunghezza = 450;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.RenditaINAIL renditaINAIL = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.RenditaINAIL();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.RenditaINAIL>(renditaINAIL, dest);
                            if (FinalResponse.ListaRenditaINAIL == null)
                                FinalResponse.ListaRenditaINAIL = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.RenditaINAIL>();
                            FinalResponse.ListaRenditaINAIL.Add(renditaINAIL);
                            break;
                        case "M":
                            lunghezza = 552;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.TrattenuteLavAutonomi trattenuteLavAutonomi = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattenuteLavAutonomi();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.TrattenuteLavAutonomi>(trattenuteLavAutonomi, dest);
                            if (FinalResponse.ListaTrattenuteLavAutonomi == null)
                                FinalResponse.ListaTrattenuteLavAutonomi = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattenuteLavAutonomi>();
                            FinalResponse.ListaTrattenuteLavAutonomi.Add(trattenuteLavAutonomi);
                            break;
                        case "N":
                            lunghezza = 208;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.AgoTeorico agoTeorico = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.AgoTeorico();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.AgoTeorico>(agoTeorico, dest);
                            if (FinalResponse.ListaAgoTeorico == null)
                                FinalResponse.ListaAgoTeorico = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.AgoTeorico>();
                            FinalResponse.ListaAgoTeorico.Add(agoTeorico);
                            break;
                        case "P":
                            lunghezza = 665;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.MaggiorazioneSociale maggiorazioneSociale = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.MaggiorazioneSociale();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.MaggiorazioneSociale>(maggiorazioneSociale, dest);
                            if (FinalResponse.ListaMaggiorazioneSociale == null)
                                FinalResponse.ListaMaggiorazioneSociale = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.MaggiorazioneSociale>();
                            FinalResponse.ListaMaggiorazioneSociale.Add(maggiorazioneSociale);
                            break;
                        case "R":
                            lunghezza = 2011;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.Redditi redditi = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Redditi();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.Redditi>(redditi, dest);
                            if (FinalResponse.ListaRedditi == null)
                                FinalResponse.ListaRedditi = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Redditi>();
                            FinalResponse.ListaRedditi.Add(redditi);
                            break;
                        case "W":
                            lunghezza = 176;
                            RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                            if (dest == null)
                                break;
                            Data.CMSGTRA.DatiNonCalcolo datiNonCalcolo = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DatiNonCalcolo();
                            HostTransactionManager.AreaFromHost<Data.CMSGTRA.DatiNonCalcolo>(datiNonCalcolo, dest);
                            if (FinalResponse.ListaDatiNonCalcolo == null)
                                FinalResponse.ListaDatiNonCalcolo = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DatiNonCalcolo>();
                            FinalResponse.ListaDatiNonCalcolo.Add(datiNonCalcolo);
                            break;
                        case "X":
                            switch (RecuperaFondo(offset, areaSenzaDelimiter))
                            {
                                case "PI":
                                case "PL":
                                    lunghezza = 204;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.PI pi = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PI();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.PI>(pi, dest);
                                    if (FinalResponse.ListaFondoPI == null)
                                        FinalResponse.ListaFondoPI = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PI>();
                                    FinalResponse.ListaFondoPI.Add(pi);
                                    break;
                                case "ES":
                                    lunghezza = 214;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.ES es = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.ES();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.ES>(es, dest);
                                    if (FinalResponse.ListaFondoES == null)
                                        FinalResponse.ListaFondoES = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.ES>();
                                    FinalResponse.ListaFondoES.Add(es);
                                    break;
                                case "GAS":
                                case "GA":
                                    lunghezza = 97;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.GAS gas = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.GAS();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.GAS>(gas, dest);
                                    if (FinalResponse.ListaFondoGAS == null)
                                        FinalResponse.ListaFondoGAS = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.GAS>();
                                    FinalResponse.ListaFondoGAS.Add(gas);
                                    break;
                                case "ET":
                                    lunghezza = 259;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.ET et = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.ET();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.ET>(et, dest);
                                    if (FinalResponse.ListaFondoET == null)
                                        FinalResponse.ListaFondoET = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.ET>();
                                    FinalResponse.ListaFondoET.Add(et);
                                    break;
                                case "PM":
                                case "PMS":
                                    lunghezza = 183;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.PM pm = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PM();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.PM>(pm, dest);
                                    if (FinalResponse.ListaFondoPM == null)
                                        FinalResponse.ListaFondoPM = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PM>();
                                    FinalResponse.ListaFondoPM.Add(pm);
                                    break;
                                case "TT":
                                    lunghezza = 231;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.TT tt = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.TT();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.TT>(tt, dest);
                                    if (FinalResponse.ListaFondoTT == null)
                                        FinalResponse.ListaFondoTT = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.TT>();
                                    FinalResponse.ListaFondoTT.Add(tt);
                                    break;
                                case "EL":
                                    lunghezza = 154;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.EL el = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.EL();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.EL>(el, dest);
                                    if (FinalResponse.ListaFondoEL == null)
                                        FinalResponse.ListaFondoEL = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.EL>();
                                    FinalResponse.ListaFondoEL.Add(el);
                                    break;
                                case "DZ":
                                    lunghezza = 149;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.DZ dz = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.DZ();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.DZ>(dz, dest);
                                    if (FinalResponse.ListaFondoDZ == null)
                                        FinalResponse.ListaFondoDZ = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.DZ>();
                                    FinalResponse.ListaFondoDZ.Add(dz);
                                    break;
                                case "VL":
                                    lunghezza = 202;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.VL vl = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.VL();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.VL>(vl, dest);
                                    if (FinalResponse.ListaFondoVL == null)
                                        FinalResponse.ListaFondoVL = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.VL>();
                                    FinalResponse.ListaFondoVL.Add(vl);
                                    break;
                                case "CL":
                                    lunghezza = 67;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.CL cl = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.CL();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.CL>(cl, dest);
                                    if (FinalResponse.ListaFondoCL == null)
                                        FinalResponse.ListaFondoCL = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.CL>();
                                    FinalResponse.ListaFondoCL.Add(cl);
                                    break;
                                case "FS":
                                    if (UtilizzaNuovoTracciato)
                                    {
                                        lunghezza = 220;
                                        RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                        if (dest == null)
                                            break;
                                        Data.CMSGTRA.Fondo.FS_New fs_New = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS_New();
                                        HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.FS_New>(fs_New, dest);
                                        if (FinalResponse.ListaFondoFS_New == null)
                                            FinalResponse.ListaFondoFS_New = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS_New>();
                                        FinalResponse.ListaFondoFS_New.Add(fs_New);
                                    }
                                    else
                                    {
                                        lunghezza = 208;
                                        RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                        if (dest == null)
                                            break;
                                        Data.CMSGTRA.Fondo.FS fs = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS();
                                        HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.FS>(fs, dest);
                                        if (FinalResponse.ListaFondoFS == null)
                                            FinalResponse.ListaFondoFS = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS>();
                                        FinalResponse.ListaFondoFS.Add(fs);
                                    }
                                    break;
                                case "PT":
                                    if (UtilizzaNuovoTracciato)
                                    {
                                        lunghezza = 251;
                                        RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                        if (dest == null)
                                            break;
                                        Data.CMSGTRA.Fondo.PT_New pt_New = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT_New();
                                        HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.PT_New>(pt_New, dest);
                                        if (FinalResponse.ListaFondoPT_New == null)
                                            FinalResponse.ListaFondoPT_New = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT_New>();
                                        FinalResponse.ListaFondoPT_New.Add(pt_New);
                                    }
                                    else
                                    {
                                        lunghezza = 239;
                                        RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                        if (dest == null)
                                            break;
                                        Data.CMSGTRA.Fondo.PT pt = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT();
                                        HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.PT>(pt, dest);
                                        if (FinalResponse.ListaFondoPT == null)
                                            FinalResponse.ListaFondoPT = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT>();
                                        FinalResponse.ListaFondoPT.Add(pt);
                                    }
                                    break;
                                case "GDP":
                                    lunghezza = 259;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Fondo.GDP gdp = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.GDP();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Fondo.GDP>(gdp, dest);
                                    if (FinalResponse.ListaFondoGDP == null)
                                        FinalResponse.ListaFondoGDP = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.GDP>();
                                    FinalResponse.ListaFondoGDP.Add(gdp);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Y":
                            switch (RecuperaFondo(offset, areaSenzaDelimiter))
                            {
                                case "PI":
                                case "PL":
                                    lunghezza = 326;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.PI pi = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.PI();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.PI>(pi, dest);
                                    if (FinalResponse.ListaAgoPI == null)
                                        FinalResponse.ListaAgoPI = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.PI>();
                                    FinalResponse.ListaAgoPI.Add(pi);
                                    break;
                                case "ES":
                                    lunghezza = 409;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.ES es = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.ES();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.ES>(es, dest);
                                    if (FinalResponse.ListaAgoES == null)
                                        FinalResponse.ListaAgoES = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.ES>();
                                    FinalResponse.ListaAgoES.Add(es);
                                    break;
                                case "GAS":
                                case "GA":
                                    lunghezza = 364;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.GAS gas = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.GAS();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.GAS>(gas, dest);
                                    if (FinalResponse.ListaAgoGAS == null)
                                        FinalResponse.ListaAgoGAS = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.GAS>();
                                    FinalResponse.ListaAgoGAS.Add(gas);
                                    break;
                                case "ET":
                                    lunghezza = 304;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.ET et = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.ET();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.ET>(et, dest);
                                    if (FinalResponse.ListaAgoET == null)
                                        FinalResponse.ListaAgoET = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.ET>();
                                    FinalResponse.ListaAgoET.Add(et);
                                    break;
                                case "PM":
                                case "PMS":
                                    lunghezza = 348;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.PM pm = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.PM();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.PM>(pm, dest);
                                    if (FinalResponse.ListaAgoPM == null)
                                        FinalResponse.ListaAgoPM = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.PM>();
                                    FinalResponse.ListaAgoPM.Add(pm);
                                    break;
                                case "EL":
                                    lunghezza = 216;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.EL el = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.EL();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.EL>(el, dest);
                                    if (FinalResponse.ListaAgoEL == null)
                                        FinalResponse.ListaAgoEL = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.EL>();
                                    FinalResponse.ListaAgoEL.Add(el);
                                    break;
                                case "TT":
                                    lunghezza = 218;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.TT tt = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.TT();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.TT>(tt, dest);
                                    if (FinalResponse.ListaAgoTT == null)
                                        FinalResponse.ListaAgoTT = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.TT>();
                                    FinalResponse.ListaAgoTT.Add(tt);
                                    break;
                                case "VL":
                                    lunghezza = 205;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.VL vl = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.VL();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.VL>(vl, dest);
                                    if (FinalResponse.ListaAgoVL == null)
                                        FinalResponse.ListaAgoVL = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.VL>();
                                    FinalResponse.ListaAgoVL.Add(vl);
                                    break;
                                case "DZ":
                                    lunghezza = 79;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.DZ dz = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.DZ();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.DZ>(dz, dest);
                                    if (FinalResponse.ListaAgoDZ == null)
                                        FinalResponse.ListaAgoDZ = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.DZ>();
                                    FinalResponse.ListaAgoDZ.Add(dz);
                                    break;
                                case "FS":
                                    lunghezza = 293;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.FS fs = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.FS();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.FS>(fs, dest);
                                    if (FinalResponse.ListaAgoFS == null)
                                        FinalResponse.ListaAgoFS = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.FS>();
                                    FinalResponse.ListaAgoFS.Add(fs);
                                    break;
                                case "PT":
                                    lunghezza = 293;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.PT pt = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.PT();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.PT>(pt, dest);
                                    if (FinalResponse.ListaAgoPT == null)
                                        FinalResponse.ListaAgoPT = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.PT>();
                                    FinalResponse.ListaAgoPT.Add(pt);
                                    break;
                                case "GDP":
                                    lunghezza = 400;
                                    RitornaAreaDaConvertire(ref lunghezza, ref offset, ref areaSenzaDelimiter, ref dest);
                                    if (dest == null)
                                        break;
                                    Data.CMSGTRA.Ago.GDP gdp = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.GDP();
                                    HostTransactionManager.AreaFromHost<Data.CMSGTRA.Ago.GDP>(gdp, dest);
                                    if (FinalResponse.ListaAgoGDP == null)
                                        FinalResponse.ListaAgoGDP = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.GDP>();
                                    FinalResponse.ListaAgoGDP.Add(gdp);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static private string RecuperaFondo(int offset, byte[] areaSenzaDelimiter)
        {
            string fondo = String.Empty;

            if (offset >= areaSenzaDelimiter.Length || offset + 4 >= areaSenzaDelimiter.Length)
                return fondo;
            byte[] dest = new byte[3];

            Buffer.BlockCopy(areaSenzaDelimiter, offset + 1, dest, 0, 3);

            Data.CMSGTRA.TipoFondo tipoFondo = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TipoFondo();
            HostTransactionManager.AreaFromHost<Data.CMSGTRA.TipoFondo>(tipoFondo, dest);

            if (tipoFondo.TIPO_FONDO != null)
                fondo = tipoFondo.TIPO_FONDO.Trim().ToUpperInvariant();

            return fondo;
        }

        static private void RitornaAreaDaConvertire(ref int lunghezza, ref int offset, ref byte[] areaSenzaDelimiter, ref byte[] dest)
        {
            if (offset >= areaSenzaDelimiter.Length)
                dest = null;
            else if (offset + lunghezza >= areaSenzaDelimiter.Length)
                lunghezza = areaSenzaDelimiter.Length - offset - 1;
            if (lunghezza == 0)
                dest = null;
            dest = new byte[lunghezza];
            Buffer.BlockCopy(areaSenzaDelimiter, offset, dest, 0, lunghezza);
            offset += lunghezza;
        }

    }
}
