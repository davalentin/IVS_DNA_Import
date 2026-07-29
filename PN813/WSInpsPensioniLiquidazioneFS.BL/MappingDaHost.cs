using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class MappingDaHost
    {
        #region public members
        public static void ValorizzaDatiPensione(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, Utility.TipoFondo? tipologiaFondo, out GestionePensione.DatiPensione datiPensione)
        {
            datiPensione = null;
            string siglaCategoria = string.Empty;

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                datiPensione = new GestionePensione.DatiPensione();
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];
                //causa carico sempre pari a 2
                //datiPensione.CausaCarico = Utility.StringToNullableByte(anagrafica.TRACAUSA.ToString());
                datiPensione.CausaCarico = 2;

                if (anagrafica.TRATIPCALC.Length >= 1)
                    datiPensione.TipoCalcolo = Utility.GetTipoCalcoloByTraduzioneSuGp(Utility.StringToNullableByte(anagrafica.TRATIPCALC.Substring(0, 1)), datiPensione, Utility.TipoAppartenenza.FS);

                if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    datiPensione.DataPerfezionamentoRequisiti = Utility.DataFromString(anagrafica.TRANRPAT.ToString(), Utility.FormatoData.AAAAmmGG);
                if (!IsCategoriaINPDAP(AreaPrelievo))
                    datiPensione.DecorrenzaCalcoloArretrati = Utility.DataFromInt(anagrafica.TRAAAPAG, anagrafica.TRAMMPAG, 1);

                if (!String.IsNullOrEmpty(anagrafica.TRAREQU1))
                {
                    switch (anagrafica.TRAREQU1)
                    {
                        case "1":
                            datiPensione.RequisitiAl1294 = false;
                            datiPensione.RequisitiAl996 = true;
                            break;
                        case "2":
                            datiPensione.RequisitiAl1294 = true;
                            datiPensione.RequisitiAl996 = true;
                            break;
                        case "4":
                            datiPensione.RequisitiAl1294 = true;
                            datiPensione.RequisitiAl996 = false;
                            break;
                        default:
                            datiPensione.RequisitiAl1294 = false;
                            datiPensione.RequisitiAl996 = false;
                            break;
                    }
                }

                if (anagrafica.TRAMATRI != 0)
                    datiPensione.MatricolaUtenteAcquisizione = anagrafica.TRAMATRI.ToString();

                datiPensione.DataInteressiLegali = Utility.DataFromInt(short.Parse(anagrafica.TRAINTLG.ToString().PadLeft(8, '0').Substring(0, 4)),
                    short.Parse(anagrafica.TRAINTLG.ToString().PadLeft(8, '0').Substring(4, 2)), short.Parse(anagrafica.TRAINTLG.ToString().PadLeft(8, '0').Substring(6, 2)));
                //codice arretrati sempre a 8
                //datiPensione.CodiceArretrati = Utility.StringToNullableByte(anagrafica.TRATRLAV.ToString());

                //ENG - PL Reversibilità 024: il codice arretrati non deve essere per default pari ad 8
                if (!(tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità && (tipologiaFondo == Utility.TipoFondo.FS || tipologiaFondo == Utility.TipoFondo.PT)))
                {
                    datiPensione.CodiceArretrati = 8;
                }

                datiPensione.CodiceSedeDestinazione = anagrafica.TRASECOM != 0 ? anagrafica.TRASECOM : anagrafica.TRASELIQ;

                siglaCategoria = GetCategoriaFromTRACATEG(anagrafica.TRACATEG.PadRight(8));
            }

            if (AreaPrelievo.FinalResponse.ListaFondoEL != null && AreaPrelievo.FinalResponse.ListaFondoEL.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.EL fondoEL = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoEL = AreaPrelievo.FinalResponse.ListaFondoEL.LastOrDefault();
                else
                    fondoEL = AreaPrelievo.FinalResponse.ListaFondoEL[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoEL.XELPVRAA, fondoEL.XELPVRMM, fondoEL.XELPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoEL.XELUVRAA, fondoEL.XELUVRMM, fondoEL.XELUVRGG);

                datiPensione.NaturaPensione = string.Concat(fondoEL.XELNATU1.ToString(), !string.IsNullOrEmpty(fondoEL.XELNATU2) ? fondoEL.XELNATU2 : " ", !string.IsNullOrEmpty(fondoEL.XELNATU3) ? fondoEL.XELNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoTT != null && AreaPrelievo.FinalResponse.ListaFondoTT.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.TT fondoTT = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoTT = AreaPrelievo.FinalResponse.ListaFondoTT.LastOrDefault();
                else
                    fondoTT = AreaPrelievo.FinalResponse.ListaFondoTT[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoTT.XTTPVRAA, fondoTT.XTTPVRMM, fondoTT.XTTPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoTT.XTTUVRAA, fondoTT.XTTUVRMM, fondoTT.XTTUVRGG);
                
                datiPensione.NaturaPensione = string.Concat(fondoTT.XTTNATU1.ToString(), !string.IsNullOrEmpty(fondoTT.XTTNATU2) ? fondoTT.XTTNATU2 : " ", !string.IsNullOrEmpty(fondoTT.XTTNATU3) ? fondoTT.XTTNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoET != null && AreaPrelievo.FinalResponse.ListaFondoET.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.ET fondoET = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoET = AreaPrelievo.FinalResponse.ListaFondoET.LastOrDefault();
                else
                    fondoET = AreaPrelievo.FinalResponse.ListaFondoET[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoET.XETPVRAA, fondoET.XETPVRMM, fondoET.XETPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoET.XETUVRAA, fondoET.XETUVRMM, fondoET.XETUVRGG);

                datiPensione.NaturaPensione = string.Concat(fondoET.XETNATU1.ToString(), !string.IsNullOrEmpty(fondoET.XETNATU2) ? fondoET.XETNATU2 : " ", !string.IsNullOrEmpty(fondoET.XETNATU3) ? fondoET.XETNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoVL != null && AreaPrelievo.FinalResponse.ListaFondoVL.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.VL fondoVL = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoVL = AreaPrelievo.FinalResponse.ListaFondoVL.LastOrDefault();
                else
                    fondoVL = AreaPrelievo.FinalResponse.ListaFondoVL[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoVL.XVLPVRAA, fondoVL.XVLPVRMM, fondoVL.XVLPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoVL.XVLUVRAA, fondoVL.XVLUVRMM, fondoVL.XVLUVRGG);

                datiPensione.NaturaPensione = string.Concat(fondoVL.XVLNATU1.ToString(), !string.IsNullOrEmpty(fondoVL.XVLNATU2) ? fondoVL.XVLNATU2 : " ", !string.IsNullOrEmpty(fondoVL.XVLNATU3) ? fondoVL.XVLNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPT != null && AreaPrelievo.FinalResponse.ListaFondoPT.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                Data.CMSGTRA.Fondo.PT fondoPT = AreaPrelievo.FinalResponse.ListaFondoPT[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoPT.XFSASSAA, fondoPT.XFSASSMM, fondoPT.XFSASSGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoPT.XFSCESAA, fondoPT.XFSCESMM, fondoPT.XFSCESGG);

                datiPensione.NaturaPensione = string.Concat(!string.IsNullOrEmpty(fondoPT.XFSNATU1) ? fondoPT.XFSNATU1 : " ", !string.IsNullOrEmpty(fondoPT.XFSNATU2) ? fondoPT.XFSNATU2 : " ", !string.IsNullOrEmpty(fondoPT.XFSNATU3) ? fondoPT.XFSNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoFS != null && AreaPrelievo.FinalResponse.ListaFondoFS.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                Data.CMSGTRA.Fondo.FS fondoFS = AreaPrelievo.FinalResponse.ListaFondoFS[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoFS.XFSASSAA, fondoFS.XFSASSMM, fondoFS.XFSASSGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoFS.XFSCESAA, fondoFS.XFSCESMM, fondoFS.XFSCESGG);

                datiPensione.NaturaPensione = string.Concat(!string.IsNullOrEmpty(fondoFS.XFSNATU1) ? fondoFS.XFSNATU1 : " ", !string.IsNullOrEmpty(fondoFS.XFSNATU2) ? fondoFS.XFSNATU2 : " ", !string.IsNullOrEmpty(fondoFS.XFSNATU3) ? fondoFS.XFSNATU3 : " ");
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoPT_New != null && AreaPrelievo.FinalResponse.ListaFondoPT_New.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                Data.CMSGTRA.Fondo.PT_New fondoPT = AreaPrelievo.FinalResponse.ListaFondoPT_New[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoPT.XFSASSAA, fondoPT.XFSASSMM, fondoPT.XFSASSGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoPT.XFSCESAA, fondoPT.XFSCESMM, fondoPT.XFSCESGG);

                datiPensione.NaturaPensione = string.Concat(!string.IsNullOrEmpty(fondoPT.XFSNATU1) ? fondoPT.XFSNATU1 : " ", !string.IsNullOrEmpty(fondoPT.XFSNATU2) ? fondoPT.XFSNATU2 : " ", !string.IsNullOrEmpty(fondoPT.XFSNATU3) ? fondoPT.XFSNATU3 : " ");
                // Isola utilizzato per taggare le migrate con XFSCSPEC = A. Impostiamo 1 per la gestione del TipoSpecifico
                if (!string.IsNullOrEmpty(fondoPT.XFSCSPEC))
                    datiPensione.Isola = fondoPT.XFSCSPEC == "A" ? (byte?)1 : null;
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoFS_New != null && AreaPrelievo.FinalResponse.ListaFondoFS_New.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                Data.CMSGTRA.Fondo.FS_New fondoFS = AreaPrelievo.FinalResponse.ListaFondoFS_New[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoFS.XFSASSAA, fondoFS.XFSASSMM, fondoFS.XFSASSGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoFS.XFSCESAA, fondoFS.XFSCESMM, fondoFS.XFSCESGG);

                datiPensione.NaturaPensione = string.Concat(!string.IsNullOrEmpty(fondoFS.XFSNATU1) ? fondoFS.XFSNATU1 : " ", !string.IsNullOrEmpty(fondoFS.XFSNATU2) ? fondoFS.XFSNATU2 : " ", !string.IsNullOrEmpty(fondoFS.XFSNATU3) ? fondoFS.XFSNATU3 : " ");
                // Isola utilizzato per taggare le migrate con XFSCSPEC = A. Impostiamo 1 per la gestione del TipoSpecifico
                if (!string.IsNullOrEmpty(fondoFS.XFSCSPEC))
                    datiPensione.Isola = fondoFS.XFSCSPEC == "A" ? (byte?)1 : null;
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPI != null && AreaPrelievo.FinalResponse.ListaFondoPI.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.PI fondoPI = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoPI = AreaPrelievo.FinalResponse.ListaFondoPI.LastOrDefault();
                else
                    fondoPI = AreaPrelievo.FinalResponse.ListaFondoPI[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoPI.XPIASSAA, fondoPI.XPIASSMM, fondoPI.XPIASSGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoPI.XPICESAA, fondoPI.XPICESMM, fondoPI.XPICESGG);

                datiPensione.NaturaPensione = string.Concat(!string.IsNullOrEmpty(fondoPI.XPINATU1) ? fondoPI.XPINATU1 : " ", fondoPI.XPINATU2.ToString(), !string.IsNullOrEmpty(fondoPI.XPINATU3) ? fondoPI.XPINATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGAS != null && AreaPrelievo.FinalResponse.ListaFondoGAS.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.GAS fondoGAS = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoGAS = AreaPrelievo.FinalResponse.ListaFondoGAS.LastOrDefault();
                else
                    fondoGAS = AreaPrelievo.FinalResponse.ListaFondoGAS[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoGAS.XGAPVRAA, fondoGAS.XGAPVRMM, fondoGAS.XGAPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoGAS.XGAUVRAA, fondoGAS.XGAUVRMM, fondoGAS.XGAUVRGG);

                datiPensione.NaturaPensione = string.Concat(fondoGAS.XGANATU1.ToString(), !string.IsNullOrEmpty(fondoGAS.XGANATU2) ? fondoGAS.XGANATU2 : " ", !string.IsNullOrEmpty(fondoGAS.XGANATU3) ? fondoGAS.XGANATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoCL != null && AreaPrelievo.FinalResponse.ListaFondoCL.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.CL fondoCL = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoCL = AreaPrelievo.FinalResponse.ListaFondoCL.LastOrDefault();
                else
                    fondoCL = AreaPrelievo.FinalResponse.ListaFondoCL[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoCL.XCLPVRAA, fondoCL.XCLPVRMM, fondoCL.XCLPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoCL.XCLUVRAA, fondoCL.XCLUVRMM, fondoCL.XCLUVRGG);

                datiPensione.NaturaPensione = string.Concat(fondoCL.XCLNATUR.ToString(), !string.IsNullOrEmpty(fondoCL.XCLNAFIL) ? fondoCL.XCLNAFIL : "  ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoDZ != null && AreaPrelievo.FinalResponse.ListaFondoDZ.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.DZ fondoDZ = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoDZ = AreaPrelievo.FinalResponse.ListaFondoDZ.LastOrDefault();
                else
                    fondoDZ = AreaPrelievo.FinalResponse.ListaFondoDZ[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoDZ.XDZPVRAA, fondoDZ.XDZPVRMM, fondoDZ.XDZPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoDZ.XDZUVRAA, fondoDZ.XDZUVRMM, fondoDZ.XDZUVRGG);

                datiPensione.NaturaPensione = string.Concat(fondoDZ.XDZNATU1.ToString(), !string.IsNullOrEmpty(fondoDZ.XDZNATU2) ? fondoDZ.XDZNATU2 : " ", !string.IsNullOrEmpty(fondoDZ.XDZNATU3) ? fondoDZ.XDZNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoES != null && AreaPrelievo.FinalResponse.ListaFondoES.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.ES fondoES = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoES = AreaPrelievo.FinalResponse.ListaFondoES.LastOrDefault();
                else
                    fondoES = AreaPrelievo.FinalResponse.ListaFondoES[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoES.XESPVRAA, fondoES.XESPVRMM, fondoES.XESPVRGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoES.XESUVRAA, fondoES.XESUVRMM, fondoES.XESUVRGG);
                datiPensione.NaturaPensione = string.Concat(fondoES.XESNATU1.ToString(), !string.IsNullOrEmpty(fondoES.XESNATU2) ? fondoES.XESNATU2 : " ", !string.IsNullOrEmpty(fondoES.XESNATU3) ? fondoES.XESNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPM != null && AreaPrelievo.FinalResponse.ListaFondoPM.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();

                Data.CMSGTRA.Fondo.PM fondoPM = null;
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S"))
                    fondoPM = AreaPrelievo.FinalResponse.ListaFondoPM.LastOrDefault();
                else
                    fondoPM = AreaPrelievo.FinalResponse.ListaFondoPM[0];

                datiPensione.InizioAssicurazione = Utility.DataFromInt(fondoPM.XPMPRIAA, fondoPM.XPMPRIMM, fondoPM.XPMPRIGG);
                datiPensione.FineAssicurazione = Utility.DataFromInt(fondoPM.XPMULTAA, fondoPM.XPMULTMM, fondoPM.XPMULTGG);
                datiPensione.NaturaPensione = string.Concat(fondoPM.XPMNATU1.ToString(), !string.IsNullOrEmpty(fondoPM.XPMNATU2) ? fondoPM.XPMNATU2 : " ", !string.IsNullOrEmpty(fondoPM.XPMNATU3) ? fondoPM.XPMNATU3 : " ");
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                Data.CMSGTRA.Fondo.GDP fondoGDP = AreaPrelievo.FinalResponse.ListaFondoGDP[0];

                datiPensione.InizioAssicurazione = Utility.DataFromString(fondoGDP.DATASS_GDP.ToString(), Utility.FormatoData.AAAAmmGG);
                datiPensione.FineAssicurazione = Utility.DataFromString(fondoGDP.DATACES_GDP.ToString(), Utility.FormatoData.AAAAmmGG);

                datiPensione.NaturaPensione = string.Concat(!string.IsNullOrEmpty(fondoGDP.NATPENS1_GDP) ? fondoGDP.NATPENS1_GDP : " ",
                    !string.IsNullOrEmpty(fondoGDP.NATPENS2_GDP) ? fondoGDP.NATPENS2_GDP : " ",
                    !string.IsNullOrEmpty(fondoGDP.NATPENS3_GDP) ? fondoGDP.NATPENS3_GDP : " ");
            }

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);
            if (tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.FS && tipoFondo.Value != Utility.TipoFondo.PT && tipoFondo.Value != Utility.TipoFondo.CL)
            {
                //verifica settaggio tipo calcolo retributivo per Monti
                if (datiPensione.TipoCalcolo.HasValue && (datiPensione.TipoCalcolo.Value == 18 || datiPensione.TipoCalcolo.Value == 25))
                {
                    if (datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 1, 1)))
                        datiPensione.TipoCalcolo = 25;
                    else
                        datiPensione.TipoCalcolo = 18;
                }
            }
            else if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.FS || tipoFondo.Value == Utility.TipoFondo.PT))
            {
                //verifica settaggio tipo calcolo retributivo per Monti
                if (datiPensione.TipoCalcolo.HasValue && (datiPensione.TipoCalcolo.Value == 18 || datiPensione.TipoCalcolo.Value == 25))
                {
                    if (datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 1, 1)))
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.FS:
                                if (AreaPrelievo.FinalResponse.ListaAgoFS != null && AreaPrelievo.FinalResponse.ListaAgoFS.Count > 0)
                                {
                                    if (AreaPrelievo.FinalResponse.ListaAgoFS.Exists(x => (x.YFSCONTR2012 != 0M || x.YFSMONTA2012 != 0M || x.YFSQUOTA2012 != 0M || x.YFSSETT2012 != 0)))
                                        datiPensione.TipoCalcolo = 25;
                                    else
                                        datiPensione.TipoCalcolo = 18;
                                }
                                break;
                            case Utility.TipoFondo.PT:
                                if (AreaPrelievo.FinalResponse.ListaAgoPT != null && AreaPrelievo.FinalResponse.ListaAgoPT.Count > 0)
                                {
                                    if (AreaPrelievo.FinalResponse.ListaAgoPT.Exists(x => (x.YFSCONTR2012 != 0M || x.YFSMONTA2012 != 0M || x.YFSQUOTA2012 != 0M || x.YFSSETT2012 != 0)))
                                        datiPensione.TipoCalcolo = 25;
                                    else
                                        datiPensione.TipoCalcolo = 18;
                                }
                                break;
                        }
                    }
                    else
                        datiPensione.TipoCalcolo = 18;
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoGDP != null && AreaPrelievo.FinalResponse.ListaAgoGDP.Count > 0)
            {
                //verifica settaggio tipo calcolo retributivo per Monti
                if (datiPensione.TipoCalcolo.HasValue && (datiPensione.TipoCalcolo.Value == 18 || datiPensione.TipoCalcolo.Value == 25))
                {
                    if (datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 1, 1)))
                    {
                        if (AreaPrelievo.FinalResponse.ListaAgoGDP.Exists(x => (x.YFSCONTR2012 != 0M || x.YFSMONTA2012 != 0M || x.YFSQUOTA2012 != 0M || x.YFSSETT2012 != 0)))
                            datiPensione.TipoCalcolo = 25;
                        else
                            datiPensione.TipoCalcolo = 18;
                    }
                    else
                        datiPensione.TipoCalcolo = 18;
                }
            }

            if (AreaPrelievo.FinalResponse.ListaResidenza != null && AreaPrelievo.FinalResponse.ListaResidenza.Count() > 0)
            {
                Data.CMSGTRA.Residenza residenza = AreaPrelievo.FinalResponse.ListaResidenza[0];
                if (residenza != null && residenza.LISTTRHONERE != null && residenza.LISTTRHONERE.Count > 0 && residenza.LISTTRHONERE.Any(x => (x.TRH_CODGRUP == "5300" || x.TRH_CODGRUP == "5800" || x.TRH_CODGRUP == "6000" || x.TRH_CODGRUP == "6100")))
                {
                    if ((residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "5300") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "5300").TRH_CODSGRUP == "5301") ||
                        (residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "5800") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "5800").TRH_CODSGRUP == "5801") ||
                        (residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "6000") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "6000").TRH_CODSGRUP == "6001") ||
                        (residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "6100") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "6100").TRH_CODSGRUP == "6101"))
                        datiPensione.LavoratorePubblico = false;
                    if ((residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "5300") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "5300").TRH_CODSGRUP == "5302") ||
                        (residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "5800") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "5800").TRH_CODSGRUP == "5802") ||
                        (residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "6000") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "6000").TRH_CODSGRUP == "6002") ||
                        (residenza.LISTTRHONERE.Any(x => x.TRH_CODGRUP == "6100") && residenza.LISTTRHONERE.FirstOrDefault(x => x.TRH_CODGRUP == "6100").TRH_CODSGRUP == "6102"))
                        datiPensione.LavoratorePubblico = true;
                }
                if (residenza != null)
                {
                    byte res = 0;
                    byte.TryParse(residenza.TRH_NUM_FIGLI, out res);
                    datiPensione.NumeroFigli = res > 0 ? res : (byte?)null;
                }
            }

            //ENG - Lettura nuovo campo FLAGSENT_R
            if (AreaPrelievo.FinalResponse.ListaRedditi != null && AreaPrelievo.FinalResponse.ListaRedditi.Count() > 0)
            {
                Data.CMSGTRA.Redditi redditi = AreaPrelievo.FinalResponse.ListaRedditi[0];
                if (redditi != null)
                {
                    short flgSent = 0;
                    short.TryParse(redditi.FLAGSENT_R.ToString(), out flgSent);
                    datiPensione.GP1AV91A = flgSent;

                    //ENG - Memo 28_2024 recupero GP1TPCLC_R (secondo byte = 1)
                    //if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                    //{
                    //    if (!String.IsNullOrEmpty(redditi.GP1TPCLC_R) && redditi.GP1TPCLC_R.Length >= 2)
                    //    {
                    //        if (redditi.GP1TPCLC_R.Substring(1, 1) == "1")
                    //            datiPensione.Caratterizzazione = " 1      ";
                    //    }
                    //}

                    datiPensione.GP1AV91B = redditi.GP1AV91B_R;

                    //sovrascrivo valore per GDP RIC REV SIN se ricevono la X
                    if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0 && tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                    {
                        Data.CMSGTRA.Fondo.GDP fondoGDP = AreaPrelievo.FinalResponse.ListaFondoGDP[0];
                        if (fondoGDP != null && fondoGDP.CSPEC_GDP == "X")
                        {
                            datiPensione.GP1AV91B = "3";
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiLavorazione(Data.FSPR AreaPrelievo, out GestioneLavorazione.DatiLavorazione datiLavorazione)
        {
            datiLavorazione = null;
            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                datiLavorazione = new GestioneLavorazione.DatiLavorazione();
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];
                datiLavorazione.TipoReversibilita = Utility.StringToNullableChar(anagrafica.TRATIPIR);
                datiLavorazione.TipoLiquidazione = anagrafica.TRATPLIQ;
            }
        }

        public static void ValorizzaDatiEliminazione(Data.FSPR AreaPrelievo, out GestionePensione.DatiEliminazione datiEliminazione)
        {
            datiEliminazione = null;
            List<GestioneDecodifica.CodiceEliminazione> lstDecCodiceEliminazione;
            GestioneDecodifica.GetCodiceEliminazioneByTipologia(out lstDecCodiceEliminazione, Utility.TipoAppartenenza.FS);

            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                datiEliminazione = new GestionePensione.DatiEliminazione();
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];
                if (!string.IsNullOrEmpty(anagrafica.TRACODEL) && anagrafica.TRACODEL != 0.ToString())
                {
                    GestioneDecodifica.CodiceEliminazione codiceEliminazione = lstDecCodiceEliminazione.Find(x => x.TraduzioneSuGP == anagrafica.TRACODEL[0]);
                    if (codiceEliminazione != null)
                        datiEliminazione.CodiceMotivo = Utility.StringToNullableByte(codiceEliminazione.Id);
                }
                datiEliminazione.DecorrenzaEliminazione = Utility.DataFromInt(short.Parse(anagrafica.TRADECEL.ToString().PadLeft(6, '0').Substring(0, 4)),
                    short.Parse(anagrafica.TRADECEL.ToString().PadLeft(6, '0').Substring(4, 2)), 1);
                datiEliminazione.DataEvento = Utility.DataFromInt(short.Parse(anagrafica.TRADATEV.ToString().PadLeft(8, '0').Substring(0, 4)),
                    short.Parse(anagrafica.TRADATEV.ToString().PadLeft(8, '0').Substring(4, 2)), short.Parse(anagrafica.TRADATEV.ToString().PadLeft(8, '0').Substring(6, 2)));
            }
        }

        public static void ValorizzaDatiSindacato(Data.FSPR AreaPrelievo, out GestionePensione.DatiSindacato datiSindacato)
        {
            datiSindacato = null;
            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                datiSindacato = new GestionePensione.DatiSindacato();
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                if (!String.IsNullOrEmpty(anagrafica.TRACODSI1))
                    datiSindacato.CodiceSindacato = anagrafica.TRACODSI1;
                datiSindacato.DecorrenzaSindacato = Utility.DataFromInt(short.Parse(anagrafica.TRADECSI.ToString().PadLeft(6, '0').Substring(2, 4)),
                    short.Parse(anagrafica.TRADECSI.ToString().PadLeft(6, '0').Substring(0, 2)), 1);
            }
        }

        public static void ValorizzaDatiDetrazioni(Data.FSPR AreaPrelievo, out GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni)
        {
            datiDetrazioni = null;
            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                datiDetrazioni = new GestioneDetrazioniImposta.DatiDetrazioni();
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                datiDetrazioni.DetrazioniReddito = Utility.StringToNullableByte(anagrafica.TRADETR1.ToString());
                datiDetrazioni.AgevolazionePensionati = Utility.StringToNullableByte(anagrafica.TRADETR2.ToString());
                datiDetrazioni.ConiugeOFiglio = Utility.StringToNullableByte(anagrafica.TRADETR3.ToString());
                datiDetrazioni.FigliMinori3AnniNoHandicap100 = Utility.StringToNullableByte(anagrafica.TRADETR4.ToString());
                datiDetrazioni.FigliMinori3AnniNoHandicap50 = Utility.StringToNullableByte(anagrafica.TRADETR5.ToString());
                datiDetrazioni.FigliMinori3AnniHandicap100 = Utility.StringToNullableByte(anagrafica.TRADETR6.ToString().PadLeft(2, '0').Substring(0, 1));
                datiDetrazioni.FigliMinori3AnniHandicap50 = Utility.StringToNullableByte(anagrafica.TRADETR6.ToString().PadLeft(2, '0').Substring(1, 1));
                datiDetrazioni.FigliMaggiori3AnniNoHandicap100 = Utility.StringToNullableByte(anagrafica.TRADETR7.ToString().PadLeft(2, '0').Substring(0, 1));
                datiDetrazioni.FigliMaggiori3AnniNoHandicap50 = Utility.StringToNullableByte(anagrafica.TRADETR7.ToString().PadLeft(2, '0').Substring(1, 1));
                datiDetrazioni.FigliMaggiori3AnniHandicap100 = Utility.StringToNullableByte(anagrafica.TRADETR8.ToString().PadLeft(2, '0').Substring(0, 1));
                datiDetrazioni.FigliMaggiori3AnniHandicap50 = Utility.StringToNullableByte(anagrafica.TRADETR8.ToString().PadLeft(2, '0').Substring(1, 1));
                datiDetrazioni.AltriFamiliari100 = Utility.StringToNullableByte(anagrafica.TRADETR9.ToString().PadLeft(2, '0').Substring(0, 1));
                datiDetrazioni.AltriFamiliari50 = Utility.StringToNullableByte(anagrafica.TRADETR9.ToString().PadLeft(2, '0').Substring(1, 1));
                datiDetrazioni.AddizionaleLombardiaVeneto = Utility.StringToNullableByte(anagrafica.TRADET10.ToString());
            }
        }

        public static void ValorizzaDatiPagamento(Data.FSPR AreaPrelievo, out GestionePagamento.DatiPagamento datiPagamento)
        {
            datiPagamento = null;
            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                datiPagamento = new GestionePagamento.DatiPagamento();
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];
                datiPagamento.TipoPagamento = anagrafica.TRAPGEST == 1 ? 'E' : (char?)null;
            }

            if (AreaPrelievo.FinalResponse.ListaDelegato != null && AreaPrelievo.FinalResponse.ListaDelegato.Count > 0)
            {
                if (datiPagamento == null)
                    datiPagamento = new GestionePagamento.DatiPagamento();
                Data.CMSGTRA.DelegatoNew delegato = AreaPrelievo.FinalResponse.ListaDelegato[0];

                datiPagamento.IBAN = delegato.TRBIBAN;
                datiPagamento.BIC = delegato.TRBBIC;
                datiPagamento.ABI = delegato.TRBCOABI;
                datiPagamento.CAB = delegato.TRBCOCAB;
                datiPagamento.ModalitaPagamento = Utility.StringToNullableChar(delegato.TRBCOPAG);
                if (datiPagamento.ABI.GetValueOrDefault() == 07601 && datiPagamento.ModalitaPagamento.GetValueOrDefault() == 'L')
                {
                    if (!(!string.IsNullOrEmpty(delegato.TRBIBAN) &&
                                delegato.TRBIBAN.Length == 27 &&
                                delegato.TRBIBAN.StartsWith("IT") &&
                                delegato.TRBIBAN.Substring(10, 5) == "03384"))
                    {
                        datiPagamento.Libretto = delegato.TRBIBAN;
                        datiPagamento.IBAN = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(delegato.TRBINPDAP))
                    datiPagamento.TrattenutaInpdap = delegato.TRBINPDAP == "SI" ? true : delegato.TRBINPDAP == "NO" ? false : (bool?)null;
                if (delegato.TRBMESEINPDAP > 0 && delegato.TRBANNOINPDAP > 0)
                    datiPagamento.DataRinunciaTrattenutaInpdap = Utility.DataFromInt(delegato.TRBANNOINPDAP, delegato.TRBMESEINPDAP, 1);
            }
        }

        public static void ValorizzaDatiFamiliare(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out List<Entity.DatiFamiliari> ListaFamiliari,
            out Dictionary<string, string> componentiFamiliari, out string messaggioVideo)
        {
            ListaFamiliari = null;
            componentiFamiliari = null;
            messaggioVideo = null;
            if (AreaPrelievo.FinalResponse.ListaFamiliare != null && AreaPrelievo.FinalResponse.ListaFamiliare.Count > 0)
            {
                ListaFamiliari = new List<Entity.DatiFamiliari>();
                componentiFamiliari = new Dictionary<string, string>();
                string siglaCategoria = GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG);
                Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, siglaCategoria);
                foreach (Data.CMSGTRA.Familiare familiare in AreaPrelievo.FinalResponse.ListaFamiliare)
                {
                    //if (string.IsNullOrEmpty(familiare.TRCCOFIS))
                    //{
                    //    messaggioVideo = "Sono presenti dei familiari senza codice fiscale sull'archivio pensione.";
                    //    return;
                    //}

                    Entity.DatiFamiliari fam = new Entity.DatiFamiliari();
                    fam.Familiare = new GestioneFamiliari.Familiare();
                    fam.ElencoCodMaggFamiliari = new List<GestioneFamiliari.CodMaggFamiliari>();

                    //gestione delel unioni civili
                    // se TRCCODFM = 7 = unito civile, imposta sigla familiare = C= coniuge e tipo unione = U = unione civile
                    if (familiare.TRCCODFM == "7")
                    {
                        fam.Familiare.SiglaFamiliare = 'C';
                        fam.Familiare.TipoUnione = "U";
                    }
                    //altrimenti se TRCCODFM =C = coniuge, imposta sigla familiare = C = coniuge e tipo unione = M = matrimonio
                    else if (familiare.TRCCODFM == "C")
                    {
                        fam.Familiare.SiglaFamiliare = 'C';
                        fam.Familiare.TipoUnione = "M";
                    }
                    else
                        // altrimenti sigla familiare = valore trovato
                        fam.Familiare.SiglaFamiliare = Utility.StringToNullableChar(familiare.TRCCODFM);

                    if (familiare.LISTTRCCONTI != null && familiare.LISTTRCCONTI.Count > 0)
                    {
                       
                        for (int i = 0; i < familiare.LISTTRCCONTI.Count; i++)
                        {
                            GestioneFamiliari.CodMaggFamiliari codMagg = new GestioneFamiliari.CodMaggFamiliari();
                            codMagg.Decorrenza = Utility.DataFromInt(familiare.LISTTRCCONTI[i].TRCDECAA, familiare.LISTTRCCONTI[i].TRCDECMM, 1);
                            codMagg.Cessazione = Utility.DataFromInt(familiare.LISTTRCCONTI[i].TRCSOSAA, familiare.LISTTRCCONTI[i].TRCSOSMM, 1);
                            codMagg.CodiceMaggiorazione = 0;
                            if (categoriaFondoPI != null)
                            {
                                codMagg.DirittoAF = familiare.LISTTRCCONTI[i].TRCDIRAF != null ? familiare.LISTTRCCONTI[i].TRCDIRAF : "";
                                codMagg.QuotaAF = familiare.LISTTRCCONTI[i].TRCQUOTA != null ? familiare.LISTTRCCONTI[i].TRCQUOTA : null;
                                codMagg.ContitolaritaFondo = familiare.LISTTRCCONTI[i].TRCCNFON != null ? familiare.LISTTRCCONTI[i].TRCCNFON : null;
                                codMagg.ContitolaritaAgo = familiare.LISTTRCCONTI[i].TRCCNAGO != null ? familiare.LISTTRCCONTI[i].TRCCNAGO : null;

                            }
                            if (codMagg.Decorrenza.HasValue || codMagg.Cessazione.HasValue)
                                fam.ElencoCodMaggFamiliari.Add(codMagg);
                        }

                      
                    }
                    fam.Familiare.CodiceFiscale = familiare.TRCCOFIS;
                    if (!string.IsNullOrEmpty(familiare.TRCPRFAM))
                    {
                        fam.Familiare.Progressivo = familiare.TRCPRFAM[0];
                        componentiFamiliari.Add(familiare.TRCCOFIS, familiare.TRCPRFAM);
                    }
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                        fam.Familiare.Confermato = true;

                    ListaFamiliari.Add(fam);
                }
            }
        }

        public static void ValorizzaDatiDetrazioniContitolare(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda,
            out List<GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato> listaDetrazioniContitolare)
        {
            listaDetrazioniContitolare = null;
            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && AreaPrelievo.FinalResponse.ListaFamiliare != null && AreaPrelievo.FinalResponse.ListaFamiliare.Count > 0)
            {
                listaDetrazioniContitolare = new List<GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato>();

                foreach (Data.CMSGTRA.Familiare familiare in AreaPrelievo.FinalResponse.ListaFamiliare)
                {
                    if (string.IsNullOrEmpty(familiare.TRCCOFIS) || string.IsNullOrEmpty(familiare.TRCCOFIS.Trim()) ||
                        (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0 &&
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACOFIS == familiare.TRCCOFIS.Trim()))
                        continue;

                    GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato datiDetrazioniContitolare = new GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato();
                    datiDetrazioniContitolare.DetrazioniReddito = (byte)familiare.TRCDETR1;
                    datiDetrazioniContitolare.AgevolazionePensionati = (byte)familiare.TRCDETR2;
                    datiDetrazioniContitolare.ConiugeOFiglio = (byte)familiare.TRCDETR3;
                    datiDetrazioniContitolare.FigliMinori3AnniNoHandicap100 = (byte)familiare.TRCDETR4;
                    datiDetrazioniContitolare.FigliMinori3AnniNoHandicap50 = (byte)familiare.TRCDETR5;
                    datiDetrazioniContitolare.FigliMinori3AnniHandicap100 = Utility.StringToNullableByte(familiare.TRCDETR6.ToString().PadLeft(2, '0').Substring(0, 1));
                    datiDetrazioniContitolare.FigliMinori3AnniHandicap50 = Utility.StringToNullableByte(familiare.TRCDETR6.ToString().PadLeft(2, '0').Substring(1, 1));
                    datiDetrazioniContitolare.FigliMaggiori3AnniNoHandicap100 = Utility.StringToNullableByte(familiare.TRCDETR7.ToString().PadLeft(2, '0').Substring(0, 1));
                    datiDetrazioniContitolare.FigliMaggiori3AnniNoHandicap50 = Utility.StringToNullableByte(familiare.TRCDETR7.ToString().PadLeft(2, '0').Substring(1, 1));
                    datiDetrazioniContitolare.FigliMaggiori3AnniHandicap100 = Utility.StringToNullableByte(familiare.TRCDETR8.ToString().PadLeft(2, '0').Substring(0, 1));
                    datiDetrazioniContitolare.FigliMaggiori3AnniHandicap50 = Utility.StringToNullableByte(familiare.TRCDETR8.ToString().PadLeft(2, '0').Substring(1, 1));
                    datiDetrazioniContitolare.AltriFamiliari100 = Utility.StringToNullableByte(familiare.TRCDETR9.ToString().PadLeft(2, '0').Substring(0, 1));
                    datiDetrazioniContitolare.AltriFamiliari50 = Utility.StringToNullableByte(familiare.TRCDETR9.ToString().PadLeft(2, '0').Substring(1, 1));
                    datiDetrazioniContitolare.AddizionaleLombardiaVeneto = (byte)familiare.TRCDET10;
                    datiDetrazioniContitolare.CodiceFiscale = familiare.TRCCOFIS.Trim();

                    listaDetrazioniContitolare.Add(datiDetrazioniContitolare);
                }

                if (listaDetrazioniContitolare.Count == 0)
                    listaDetrazioniContitolare = null;
            }
        }

        public static void ValorizzaRecordFondo(Data.FSPR AreaPrelievo, out List<GestioneRecordFondo.DatiRecordFondo> ListaRecordFondo)
        {
            ListaRecordFondo = null;
            if (AreaPrelievo.FinalResponse.ListaFondoEL != null && AreaPrelievo.FinalResponse.ListaFondoEL.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.EL fondoEL in AreaPrelievo.FinalResponse.ListaFondoEL)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoEL.XELDECAA, fondoEL.XELDECMM, 1).HasValue ? Utility.DataFromInt(fondoEL.XELDECAA, fondoEL.XELDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoEL.XELSOSAA, fondoEL.XELSOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoEL.XELNONCA == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoEL.XELNATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(fondoEL.XELNATU2);
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(fondoEL.XELNATU3);
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoTT != null && AreaPrelievo.FinalResponse.ListaFondoTT.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.TT fondoTT in AreaPrelievo.FinalResponse.ListaFondoTT)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoTT.XTTDECAA, fondoTT.XTTDECMM, 1).HasValue ? Utility.DataFromInt(fondoTT.XTTDECAA, fondoTT.XTTDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoTT.XTTSOSAA, fondoTT.XTTSOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoTT.XTTNOCAL == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoTT.XTTNATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(fondoTT.XTTNATU2);
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(fondoTT.XTTNATU3);
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoET != null && AreaPrelievo.FinalResponse.ListaFondoET.Count(x => x.XETFISSE != 7) > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.ET fondoET in AreaPrelievo.FinalResponse.ListaFondoET.FindAll(x => x.XETFISSE != 7))
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoET.XETDECAA, fondoET.XETDECMM, 1).HasValue ? Utility.DataFromInt(fondoET.XETDECAA, fondoET.XETDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoET.XETSOSAA, fondoET.XETSOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoET.XETNOCAL == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoET.XETNATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(fondoET.XETNATU2);
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(fondoET.XETNATU3);
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoVL != null && AreaPrelievo.FinalResponse.ListaFondoVL.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.VL fondoVL in AreaPrelievo.FinalResponse.ListaFondoVL)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoVL.XVLDECAA, fondoVL.XVLDECMM, 1).HasValue ? Utility.DataFromInt(fondoVL.XVLDECAA, fondoVL.XVLDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoVL.XVLSOSAA, fondoVL.XVLSOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoVL.XVLNONCA == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoVL.XVLNATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(fondoVL.XVLNATU2);
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(fondoVL.XVLNATU3);
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPT != null && AreaPrelievo.FinalResponse.ListaFondoPT.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.PT fondoPT in AreaPrelievo.FinalResponse.ListaFondoPT)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoPT.XFSDECAA, fondoPT.XFSDECMM, fondoPT.XFSDECGG).HasValue ? Utility.DataFromInt(fondoPT.XFSDECAA, fondoPT.XFSDECMM, fondoPT.XFSDECGG).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoPT.XFSSCAAA, fondoPT.XFSSCAMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoPT.XFSNCALC == "1" ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(!fondoPT.XFSNATU1.Equals(string.Empty) ? fondoPT.XFSNATU1 : " ");
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!fondoPT.XFSNATU2.Equals(string.Empty) ? fondoPT.XFSNATU2 : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!fondoPT.XFSNATU3.Equals(string.Empty) ? fondoPT.XFSNATU3 : " ");
                    recordFondo.Id = fondoPT.XFSPROGR;
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoFS != null && AreaPrelievo.FinalResponse.ListaFondoFS.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.FS fondoFS in AreaPrelievo.FinalResponse.ListaFondoFS)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoFS.XFSDECAA, fondoFS.XFSDECMM, fondoFS.XFSDECGG).HasValue ? Utility.DataFromInt(fondoFS.XFSDECAA, fondoFS.XFSDECMM, fondoFS.XFSDECGG).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoFS.XFSSCAAA, fondoFS.XFSSCAMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoFS.XFSNCALC == "1" ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(!fondoFS.XFSNATU1.Equals(string.Empty) ? fondoFS.XFSNATU1 : " ");
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!fondoFS.XFSNATU2.Equals(string.Empty) ? fondoFS.XFSNATU2 : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!fondoFS.XFSNATU3.Equals(string.Empty) ? fondoFS.XFSNATU3 : " ");
                    recordFondo.Id = fondoFS.XFSPROGR;
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoPT_New != null && AreaPrelievo.FinalResponse.ListaFondoPT_New.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.PT_New fondoPT in AreaPrelievo.FinalResponse.ListaFondoPT_New)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoPT.XFSDECAA, fondoPT.XFSDECMM, fondoPT.XFSDECGG).HasValue ? Utility.DataFromInt(fondoPT.XFSDECAA, fondoPT.XFSDECMM, fondoPT.XFSDECGG).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoPT.XFSSCAAA, fondoPT.XFSSCAMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoPT.XFSNCALC == "1" ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(!fondoPT.XFSNATU1.Equals(string.Empty) ? fondoPT.XFSNATU1 : " ");
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!fondoPT.XFSNATU2.Equals(string.Empty) ? fondoPT.XFSNATU2 : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!fondoPT.XFSNATU3.Equals(string.Empty) ? fondoPT.XFSNATU3 : " ");
                    recordFondo.Id = fondoPT.XFSPROGR;
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoFS_New != null && AreaPrelievo.FinalResponse.ListaFondoFS_New.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.FS_New fondoFS in AreaPrelievo.FinalResponse.ListaFondoFS_New)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoFS.XFSDECAA, fondoFS.XFSDECMM, fondoFS.XFSDECGG).HasValue ? Utility.DataFromInt(fondoFS.XFSDECAA, fondoFS.XFSDECMM, fondoFS.XFSDECGG).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoFS.XFSSCAAA, fondoFS.XFSSCAMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoFS.XFSNCALC == "1" ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(!fondoFS.XFSNATU1.Equals(string.Empty) ? fondoFS.XFSNATU1 : " ");
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!fondoFS.XFSNATU2.Equals(string.Empty) ? fondoFS.XFSNATU2 : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!fondoFS.XFSNATU3.Equals(string.Empty) ? fondoFS.XFSNATU3 : " ");
                    recordFondo.Id = fondoFS.XFSPROGR;
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPI != null && AreaPrelievo.FinalResponse.ListaFondoPI.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.PI fondoPI in AreaPrelievo.FinalResponse.ListaFondoPI)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoPI.XPIDECAA, fondoPI.XPIDECMM, fondoPI.XPIDECGG).HasValue ? Utility.DataFromInt(fondoPI.XPIDECAA, fondoPI.XPIDECMM, fondoPI.XPIDECGG).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoPI.XPISCAAA, fondoPI.XPISCAMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoPI.XPINCALC == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoPI.XPINATU1);
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(fondoPI.XPINATU2.ToString());
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(fondoPI.XPINATU3);
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGAS != null && AreaPrelievo.FinalResponse.ListaFondoGAS.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.GAS fondoGAS in AreaPrelievo.FinalResponse.ListaFondoGAS)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoGAS.XGADECAA, fondoGAS.XGADECMM, 1).HasValue ? Utility.DataFromInt(fondoGAS.XGADECAA, fondoGAS.XGADECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoGAS.XGASOSAA, fondoGAS.XGASOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoGAS.XGANOCAL == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoGAS.XGANATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(fondoGAS.XGANATU2);
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(fondoGAS.XGANATU3);
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoCL != null && AreaPrelievo.FinalResponse.ListaFondoCL.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.CL fondoCL in AreaPrelievo.FinalResponse.ListaFondoCL)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoCL.XCLDECAA, fondoCL.XCLDECMM, 1).HasValue ? Utility.DataFromInt(fondoCL.XCLDECAA, fondoCL.XCLDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoCL.XCLSCAAA, fondoCL.XCLSCAMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoCL.XCLNONCA == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoCL.XCLNATUR.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!string.IsNullOrEmpty(fondoCL.XCLNAFIL) ? fondoCL.XCLNAFIL.Substring(0, 1) : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!string.IsNullOrEmpty(fondoCL.XCLNAFIL) ? fondoCL.XCLNAFIL.Substring(1, 1) : " ");
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoDZ != null && AreaPrelievo.FinalResponse.ListaFondoDZ.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.DZ fondoDZ in AreaPrelievo.FinalResponse.ListaFondoDZ)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoDZ.XDZDECAA, fondoDZ.XDZDECMM, 1).HasValue ? Utility.DataFromInt(fondoDZ.XDZDECAA, fondoDZ.XDZDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoDZ.XDZSOSAA, fondoDZ.XDZSOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoDZ.XDZNOCAL == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoDZ.XDZNATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!string.IsNullOrEmpty(fondoDZ.XDZNATU2) && fondoDZ.XDZNATU2 != "0" ? fondoDZ.XDZNATU2.Substring(0, 1) : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!string.IsNullOrEmpty(fondoDZ.XDZNATU3) ? fondoDZ.XDZNATU3.Substring(1, 1) : " ");
                    recordFondo.Id = fondoDZ.XDZPROGR; 
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoES != null && AreaPrelievo.FinalResponse.ListaFondoES.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.ES fondoES in AreaPrelievo.FinalResponse.ListaFondoES)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoES.XESDECAA, fondoES.XESDECMM, 1).HasValue ? Utility.DataFromInt(fondoES.XESDECAA, fondoES.XESDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoES.XESSOSAA, fondoES.XESSOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoES.XESNOCAL == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoES.XESNATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!string.IsNullOrEmpty(fondoES.XESNATU2) ? fondoES.XESNATU2.Substring(0, 1) : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!string.IsNullOrEmpty(fondoES.XESNATU3) ? fondoES.XESNATU3.Substring(1, 1) : " ");
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPM != null && AreaPrelievo.FinalResponse.ListaFondoPM.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.PM fondoPM in AreaPrelievo.FinalResponse.ListaFondoPM)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromInt(fondoPM.XPMDECAA, fondoPM.XPMDECMM, 1).HasValue ?
                        Utility.DataFromInt(fondoPM.XPMDECAA, fondoPM.XPMDECMM, 1).Value : DateTime.MinValue;
                    recordFondo.DataSospensione = Utility.DataFromInt(fondoPM.XPMSOSAA, fondoPM.XPMSOSMM, 1);
                    recordFondo.CodiceNonCalcolo = fondoPM.XPMNCALC == 1 ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(fondoPM.XPMNATU1.ToString());
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(fondoPM.XPMNATU2);
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(fondoPM.XPMNATU3);
                    ListaRecordFondo.Add(recordFondo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0)
            {
                ListaRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                foreach (Data.CMSGTRA.Fondo.GDP fondoGDP in AreaPrelievo.FinalResponse.ListaFondoGDP)
                {
                    GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    recordFondo.DecorrenzaValiditaDati = Utility.DataFromString(fondoGDP.DECPENS_GDP.ToString(), Utility.FormatoData.AAAAmmGG);
                    recordFondo.DataSospensione = Utility.DataFromString(fondoGDP.SOSPENS_GDP.ToString() + "01", Utility.FormatoData.AAAAmmGG);
                    recordFondo.CodiceNonCalcolo = fondoGDP.NCALC_GDP == "1" ? 'S' : 'N';
                    recordFondo.CodiceNatura1 = Utility.StringToNullableChar(!fondoGDP.NATPENS1_GDP.Equals(string.Empty) ? fondoGDP.NATPENS1_GDP : " ");
                    recordFondo.CodiceNatura2 = Utility.StringToNullableChar(!fondoGDP.NATPENS2_GDP.Equals(string.Empty) ? fondoGDP.NATPENS2_GDP : " ");
                    recordFondo.CodiceNatura3 = Utility.StringToNullableChar(!fondoGDP.NATPENS3_GDP.Equals(string.Empty) ? fondoGDP.NATPENS3_GDP : " ");
                    recordFondo.Id = fondoGDP.PROGR_GDP;
                    ListaRecordFondo.Add(recordFondo);
                }
            }
        }

        public static void ValorizzaPensioneFondoDatiGenerici(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondo datiPensioneFondoDatiGenerici)
        {
            datiPensioneFondoDatiGenerici = null;
            datiPensioneFondoDatiGenerici.BypassDinamicoCodiceSpecifico = false;
            if (AreaPrelievo.FinalResponse.ListaFondoEL != null && AreaPrelievo.FinalResponse.ListaFondoEL.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.EL fondoEL = AreaPrelievo.FinalResponse.ListaFondoEL[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("EL", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoEL.XELATTIV.ToString());
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoEL.XELSPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoEL.XELSPECI) && x.Fondo == "EL");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }

                datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(fondoEL.XELREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(fondoEL.XELREQU2.ToString());
                if (fondoEL.XELTETTO != 0M)
                    datiPensioneFondoDatiGenerici.RetrPondAnnuaAGOLimite = fondoEL.XELTETTO;
                datiPensioneFondoDatiGenerici.CodiceDirittoQuoteFisse = Utility.StringToNullableByte(fondoEL.XELFISSE.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoTT != null && AreaPrelievo.FinalResponse.ListaFondoTT.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.TT fondoTT = AreaPrelievo.FinalResponse.ListaFondoTT[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("TT", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoTT.XTTATTIV.ToString());
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoTT.XTTSPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoTT.XTTSPECI) && x.Fondo == "TT");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }

                datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(fondoTT.XTTREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(fondoTT.XTTREQU2.ToString());

                if (fondoTT.XTTTETTO != 0M)
                    datiPensioneFondoDatiGenerici.RetrPondAnnuaAGOLimite = fondoTT.XTTTETTO;
                datiPensioneFondoDatiGenerici.CodiceDirittoQuoteFisse = Utility.StringToNullableByte(fondoTT.XTTFISSE.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoET != null && AreaPrelievo.FinalResponse.ListaFondoET.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.ET fondoET = AreaPrelievo.FinalResponse.ListaFondoET[0];

                if (!string.IsNullOrEmpty(fondoET.XETSPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoET.XETSPECI) && x.Fondo == "ET");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }

                datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(fondoET.XETREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(fondoET.XETREQU2.ToString());
                datiPensioneFondoDatiGenerici.CodiceDirittoQuoteFisse = Utility.StringToNullableByte(fondoET.XETFISSE.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoVL != null && AreaPrelievo.FinalResponse.ListaFondoVL.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.VL fondoVL = AreaPrelievo.FinalResponse.ListaFondoVL[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("VL", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp ==
                        fondoVL.XVLATTI1.ToString() + (fondoVL.XVLATTI2 != 0 ? fondoVL.XVLATTI2.ToString() : string.Empty));
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoVL.XVLSPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoVL.XVLSPECI) && x.Fondo == "VL");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }

                datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(fondoVL.XVLREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(fondoVL.XVLREQU2.ToString());
                datiPensioneFondoDatiGenerici.CodiceDirittoQuoteFisse = Utility.StringToNullableByte(fondoVL.XVLFISSE.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPT != null && AreaPrelievo.FinalResponse.ListaFondoPT.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.PT fondoPT = AreaPrelievo.FinalResponse.ListaFondoPT[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("PT", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoPT.XFSPROF);
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoPT.XFSCSPEC))
                {
                    if (fondoPT.XFSCSPEC == "A")
                    {
                        datiPensioneFondoDatiGenerici.BypassDinamicoCodiceSpecifico = true;
                        //datiPensioneFondoDatiGenerici.CodiceSpecificoTraduzione = 'A';
                    }
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoPT.XFSCSPEC) && x.Fondo == "PT");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoFS != null && AreaPrelievo.FinalResponse.ListaFondoFS.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.FS fondoFS = AreaPrelievo.FinalResponse.ListaFondoFS[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("FS", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoFS.XFSPROF);
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoFS.XFSCSPEC))
                {
                    if (fondoFS.XFSCSPEC == "A")
                    {
                        datiPensioneFondoDatiGenerici.BypassDinamicoCodiceSpecifico = true;
                        //datiPensioneFondoDatiGenerici.CodiceSpecificoTraduzione = 'A';
                    }
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        bool isCategoriaINPDAP = IsCategoriaINPDAP(AreaPrelievo);

                        if (isCategoriaINPDAP) // Categorie INPDAP
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoFS.XFSCSPEC) && x.Fondo == "DAP");
                            if (codiceSpecifico != null)
                                datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                        else
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoFS.XFSCSPEC) && x.Fondo == "FS");
                            if (codiceSpecifico != null)
                                datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoPT_New != null && AreaPrelievo.FinalResponse.ListaFondoPT_New.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.PT_New fondoPT = AreaPrelievo.FinalResponse.ListaFondoPT_New[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("PT", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoPT.XFSPROF);
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoPT.XFSCSPEC))
                {
                    if (fondoPT.XFSCSPEC == "A")
                    {
                        datiPensioneFondoDatiGenerici.BypassDinamicoCodiceSpecifico = true;
                        //datiPensioneFondoDatiGenerici.CodiceSpecificoTraduzione = 'A';
                    }
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoPT.XFSCSPEC) && x.Fondo == "PT");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoFS_New != null && AreaPrelievo.FinalResponse.ListaFondoFS_New.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.FS_New fondoFS = AreaPrelievo.FinalResponse.ListaFondoFS_New[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("FS", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoFS.XFSPROF);
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoFS.XFSCSPEC))
                {
                    if (fondoFS.XFSCSPEC == "A")
                    {
                        datiPensioneFondoDatiGenerici.BypassDinamicoCodiceSpecifico = true;
                        //datiPensioneFondoDatiGenerici.CodiceSpecificoTraduzione = 'A';
                    }
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoFS.XFSCSPEC) && x.Fondo == "FS");
                        if (codiceSpecifico != null)
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPI != null && AreaPrelievo.FinalResponse.ListaFondoPI.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.PI fondoPI = AreaPrelievo.FinalResponse.ListaFondoPI[0];

                // TODO - Da verificare se funziona - IMPORTANTE
                char? enteFondo = null;
                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    string siglaCategoria = GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG);
                    enteFondo = Utility.GetCharCategoriaFondoPI(Utility.TipoAppartenenza.FS, siglaCategoria);
                }

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("PI", enteFondo, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoPI.XPIATTIV.ToString());
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoPI.XPISPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoPI.XPISPECI) && x.Fondo == "PI");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGAS != null && AreaPrelievo.FinalResponse.ListaFondoGAS.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.GAS fondoGAS = AreaPrelievo.FinalResponse.ListaFondoGAS[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("GAS", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoGAS.XGAATTIV.ToString());
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                if (!string.IsNullOrEmpty(fondoGAS.XGASPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoGAS.XGASPECI) && x.Fondo == "GAS");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }

                datiPensioneFondoDatiGenerici.CodiceDirittoQuoteFisse = Utility.StringToNullableByte(fondoGAS.XGAFISSE.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoCL != null && AreaPrelievo.FinalResponse.ListaFondoCL.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.CL fondoCL = AreaPrelievo.FinalResponse.ListaFondoCL[0];

                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                GestioneDecodifica.GetAttivitaSvoltaByFondo("CL", null, out elencoAttivitaSvolte);
                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                {
                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.TraduzioneSuGp == fondoCL.XCLATTIV.ToString());
                    if (attSvolta != null)
                        datiPensioneFondoDatiGenerici.AttivitaSvolta = attSvolta.Id;
                }

                datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(fondoCL.XCLREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(fondoCL.XCLREQU2.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoDZ != null && AreaPrelievo.FinalResponse.ListaFondoDZ.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.DZ fondoDZ = AreaPrelievo.FinalResponse.ListaFondoDZ[0];

                if (!string.IsNullOrEmpty(fondoDZ.XDZSPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoDZ.XDZSPECI) && x.Fondo == "DZ");
                        if (codiceSpecifico != null)
                        {
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                        }
                    }
                }

                datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(fondoDZ.XDZREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(fondoDZ.XDZREQU2.ToString());
                datiPensioneFondoDatiGenerici.CodiceDirittoQuoteFisse = Utility.StringToNullableByte(fondoDZ.XDZFISSE.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoES != null && AreaPrelievo.FinalResponse.ListaFondoES.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.ES fondoES = AreaPrelievo.FinalResponse.ListaFondoES[0];

                if (fondoES.XESATTIV != 0)
                {
                    List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolta = null;
                    GestioneDecodifica.GetAttivitaSvoltaByFondo("ES", null, out elencoAttivitaSvolta);
                    if (elencoAttivitaSvolta != null && elencoAttivitaSvolta.Count > 0)
                    {
                        GestioneDecodifica.AttivitaSvolta attivitaSvolta = elencoAttivitaSvolta.Find(x => x.TraduzioneSuGp == fondoES.XESATTIV.ToString());
                        if (attivitaSvolta != null)
                            datiPensioneFondoDatiGenerici.AttivitaSvolta = attivitaSvolta.Id;
                    }
                }
                if (fondoES.XESFISSE != 0)
                    datiPensioneFondoDatiGenerici.CodiceDirittoQuoteFisse = (byte)fondoES.XESFISSE;
                if (!string.IsNullOrEmpty(fondoES.XESSPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoES.XESSPECI) && x.Fondo == "ES");
                        if (codiceSpecifico != null)
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                    }
                }

                //Comma 707
                if (fondoES.XESSETA_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = fondoES.XESSETA_707;
                if (fondoES.XESSETB_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = fondoES.XESSETB_707;
                // Al momento non verrà mappato perchè non abbiamo le specifiche
                //if(!String.IsNullOrEmpty(fondoES.XESCALC707)) 
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPM != null && AreaPrelievo.FinalResponse.ListaFondoPM.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.PM fondoPM = AreaPrelievo.FinalResponse.ListaFondoPM[0];

                if (!string.IsNullOrEmpty(fondoPM.XPMATTIV))
                {
                    List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolta = null;
                    GestioneDecodifica.GetAttivitaSvoltaByFondo("PM", null, out elencoAttivitaSvolta);
                    if (elencoAttivitaSvolta != null && elencoAttivitaSvolta.Count > 0)
                    {
                        GestioneDecodifica.AttivitaSvolta attivitaSvolta = elencoAttivitaSvolta.Find(x => x.TraduzioneSuGp == fondoPM.XPMATTIV.Substring(0, 1));
                        if (attivitaSvolta != null)
                            datiPensioneFondoDatiGenerici.AttivitaSvolta = attivitaSvolta.Id;
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0)
            {
                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Fondo.GDP fondoGDP = AreaPrelievo.FinalResponse.ListaFondoGDP[0];

                if (!string.IsNullOrEmpty(fondoGDP.CSPEC_GDP))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(fondoGDP.CSPEC_GDP) && x.Fondo == "DAP");
                        if (codiceSpecifico != null)
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                    }
                    datiPensioneFondoDatiGenerici.CodiceSpecificoGP = fondoGDP.CSPEC_GDP;
                }
            }

            if (AreaPrelievo.FinalResponse.ListaAgoEL != null && AreaPrelievo.FinalResponse.ListaAgoEL.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.EL agoEL = AreaPrelievo.FinalResponse.ListaAgoEL[0];

                if (!string.IsNullOrEmpty(agoEL.YELFLAG214))
                    datiPensioneFondoDatiGenerici.RiduzioneRetributiva = agoEL.YELFLAG214 == "S" ? true : false;
                if (agoEL.YELPERC214 != 0M)
                    datiPensioneFondoDatiGenerici.RiduzioneRetributivaPercentuale = agoEL.YELPERC214;
                //Comma 707
                if (agoEL.YELIMP707 != 0M)
                    datiPensioneFondoDatiGenerici.RetribuzionePonderataAGO707 = agoEL.YELIMP707;
                if (agoEL.YELSETA707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = agoEL.YELSETA707;
                if (agoEL.YELSETB707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = agoEL.YELSETB707;
                if (agoEL.YELSETC707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaC707 = agoEL.YELSETC707;
                if (agoEL.YELSETD707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaD707 = agoEL.YELSETD707;
                if (agoEL.YELSETDIR != 0)
                    datiPensioneFondoDatiGenerici.SettimaneUtiliDiritto = agoEL.YELSETDIR;
                // Al momento non verrà mappato perchè non abbiamo le specifiche
                //if(!String.IsNullOrEmpty(agoEL.YELCALC707)) 
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoTT != null && AreaPrelievo.FinalResponse.ListaAgoTT.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.TT agoTT = AreaPrelievo.FinalResponse.ListaAgoTT[0];

                if (!string.IsNullOrEmpty(agoTT.YTTFLAG214))
                    datiPensioneFondoDatiGenerici.RiduzioneRetributiva = agoTT.YTTFLAG214 == "S" ? true : false;
                if (agoTT.YTTPERC214 != 0M)
                    datiPensioneFondoDatiGenerici.RiduzioneRetributivaPercentuale = agoTT.YTTPERC214;
                //Comma 707
                if (agoTT.YTTIMP707 != 0M)
                    datiPensioneFondoDatiGenerici.RetribuzionePonderataAGO707 = agoTT.YTTIMP707;
                if (agoTT.YTTSETA707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = agoTT.YTTSETA707;
                if (agoTT.YTTSETB707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = agoTT.YTTSETB707;
                if (agoTT.YTTSETC707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaC707 = agoTT.YTTSETC707;
                if (agoTT.YTTSETD707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaD707 = agoTT.YTTSETD707;
                if (agoTT.YTTSETDIR != 0)
                    datiPensioneFondoDatiGenerici.SettimaneUtiliDiritto = agoTT.YTTSETDIR;
                // 2015-01-22 G.Arru - Al momento non verrà mappato perchè non abbiamo le specifiche
                //if(!String.IsNullOrEmpty(agoTT.YTTCALC707)) 
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoET != null && AreaPrelievo.FinalResponse.ListaAgoET.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.ET agoET = AreaPrelievo.FinalResponse.ListaAgoET[0];

                if (!string.IsNullOrEmpty(agoET.YETFLAG214))
                    datiPensioneFondoDatiGenerici.RiduzioneRetributiva = agoET.YETFLAG214 == "S" ? true : false;
                if (agoET.YETPERC214 != 0M)
                    datiPensioneFondoDatiGenerici.RiduzioneRetributivaPercentuale = agoET.YETPERC214;

                //Comma 707 ET
                if (agoET.YETSETAFAA707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707AA = (byte)agoET.YETSETAFAA707;
                if (agoET.YETSETAFMM707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707MM = (byte)agoET.YETSETAFMM707;
                if (agoET.YETSETAFGG707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707GG = (byte)agoET.YETSETAFGG707;
                if (agoET.YETSETBFAA707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707AA = (byte)agoET.YETSETBFAA707;
                if (agoET.YETSETBFMM707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707MM = (byte)agoET.YETSETBFMM707;
                if (agoET.YETSETBFGG707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707GG = (byte)agoET.YETSETBFGG707;
                if (agoET.YETSETCFAA707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaC707AA = (byte)agoET.YETSETCFAA707;
                if (agoET.YETSETCFMM707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaC707MM = (byte)agoET.YETSETCFMM707;
                if (agoET.YETSETCFGG707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaC707GG = (byte)agoET.YETSETCFGG707;
                if (agoET.YETSETAGOA707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = agoET.YETSETAGOA707;
                if (agoET.YETSETAGOB707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = agoET.YETSETAGOB707;
                if (agoET.YETIMP707 != 0M)
                    datiPensioneFondoDatiGenerici.RetribuzionePonderataAGO707 = agoET.YETIMP707;
                if (agoET.YETSETDIR != 0)
                    datiPensioneFondoDatiGenerici.SettimaneUtiliDiritto = agoET.YETSETDIR;
                // Al momento non verrà mappato perchè non abbiamo le specifiche
                //agoET.YETCALC707    
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoVL != null && AreaPrelievo.FinalResponse.ListaAgoVL.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.VL agoVL = AreaPrelievo.FinalResponse.ListaAgoVL[0];

                if (!string.IsNullOrEmpty(agoVL.YVLFLAG214))
                    datiPensioneFondoDatiGenerici.RiduzioneRetributiva = agoVL.YVLFLAG214 == "S" ? true : false;
                if (agoVL.YVLPERC214 != 0M)
                    datiPensioneFondoDatiGenerici.RiduzioneRetributivaPercentuale = agoVL.YVLPERC214;

                //Comma 707
                if (agoVL.YVLSETA1707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = agoVL.YVLSETA1707;
                if (agoVL.YVLSETA2707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA2707 = agoVL.YVLSETA2707;
                if (agoVL.YVLSETB707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = agoVL.YVLSETB707;
                if (agoVL.YVLSETC1707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaC707 = agoVL.YVLSETC1707;
                if (agoVL.YVLSETC2707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaC2707 = agoVL.YVLSETC2707;
                if (agoVL.YVLSETD707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaD707 = agoVL.YVLSETD707;
                if (agoVL.YVLSETDIR != 0)
                    datiPensioneFondoDatiGenerici.SettimaneUtiliDiritto = agoVL.YVLSETDIR;
                // 2015-01-22 G.Arru - Al momento non verrà mappato perchè non abbiamo le specifiche
                //if(!String.IsNullOrEmpty(agoVL.YVLCALC707))   
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoGAS != null && AreaPrelievo.FinalResponse.ListaAgoGAS.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.GAS agoGAS = AreaPrelievo.FinalResponse.ListaAgoGAS[0];

                if (!string.IsNullOrEmpty(agoGAS.YGAFLAG214))
                    datiPensioneFondoDatiGenerici.RiduzioneRetributiva = agoGAS.YGAFLAG214 == "S" ? true : false;
                if (agoGAS.YGAPERC214 != 0M)
                    datiPensioneFondoDatiGenerici.RiduzioneRetributivaPercentuale = agoGAS.YGAPERC214;

                datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(agoGAS.YGAREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(agoGAS.YGAREQU2.ToString());

                //Comma 707
                if (agoGAS.YGASETA_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = agoGAS.YGASETA_707;
                if (agoGAS.YGASETB_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = agoGAS.YGASETB_707;
                if (agoGAS.YGASETAES_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaAES707 = agoGAS.YGASETAES_707;
                if (agoGAS.YGASETBES_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaBES707 = agoGAS.YGASETBES_707;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoDZ != null && AreaPrelievo.FinalResponse.ListaAgoDZ.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.DZ agoDZ = AreaPrelievo.FinalResponse.ListaAgoDZ[0];

                if (!string.IsNullOrEmpty(agoDZ.YDZFLAG214))
                    datiPensioneFondoDatiGenerici.RiduzioneRetributiva = agoDZ.YDZFLAG214 == "S" ? true : false;
                if (agoDZ.YDZPERC214 != 0M)
                    datiPensioneFondoDatiGenerici.RiduzioneRetributivaPercentuale = agoDZ.YDZPERC214;
                if (agoDZ.YDZSETA_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = (short)agoDZ.YDZSETA_707;
                if (agoDZ.YDZSETB_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = (short)agoDZ.YDZSETB_707;

                //datiPensioneFondoDatiGenerici.CodiceRequisiti1 = Utility.StringToNullableChar(agoGAS.YGAREQU1);
                //datiPensioneFondoDatiGenerici.CodiceRequisiti2 = Utility.StringToNullableChar(agoGAS.YGAREQU2.ToString());
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoES != null && AreaPrelievo.FinalResponse.ListaAgoES.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.ES agoES = AreaPrelievo.FinalResponse.ListaAgoES[0];

                if (!string.IsNullOrEmpty(agoES.YESREQU1))
                    datiPensioneFondoDatiGenerici.CodiceRequisiti1 = char.Parse(agoES.YESREQU1);
                datiPensioneFondoDatiGenerici.CodiceRequisiti2 = char.Parse(agoES.YESREQU2.ToString());
                if (!string.IsNullOrEmpty(agoES.YESFLAG214))
                    datiPensioneFondoDatiGenerici.RiduzioneRetributiva = agoES.YESFLAG214 == "S" ? true : false;
                if (agoES.YESPERC214 != 0)
                    datiPensioneFondoDatiGenerici.RiduzioneRetributivaPercentuale = agoES.YESPERC214;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoPM != null && AreaPrelievo.FinalResponse.ListaAgoPM.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();
                Data.CMSGTRA.Ago.PM agoPM = AreaPrelievo.FinalResponse.ListaAgoPM[0];

                if (agoPM.YPM503AS != 0)
                    datiPensioneFondoDatiGenerici.CodiceRequisiti2 = char.Parse(agoPM.YPM503AS.ToString());
                if (!string.IsNullOrEmpty(agoPM.YPM503ET))
                    datiPensioneFondoDatiGenerici.CodiceRequisiti1 = char.Parse(agoPM.YPM503ET);
                if (!string.IsNullOrEmpty(agoPM.YPMSPECI))
                {
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.TraduzioneGp == Utility.StringToNullableChar(agoPM.YPMSPECI) && x.Fondo == "PM");
                        if (codiceSpecifico != null)
                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codiceSpecifico.Id;
                    }
                }

                if (agoPM.YPMSETA_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaA707 = agoPM.YPMSETA_707;
                if (agoPM.YPMSETB_707 != 0)
                    datiPensioneFondoDatiGenerici.QuotaB707 = agoPM.YPMSETB_707;
            }

            if (AreaPrelievo.FinalResponse.ListaDelegato != null && AreaPrelievo.FinalResponse.ListaDelegato.Count > 0)
            {
                if (datiPensioneFondoDatiGenerici == null)
                    datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();

                Data.CMSGTRA.DelegatoNew delegato = AreaPrelievo.FinalResponse.ListaDelegato[0];

                if (!String.IsNullOrEmpty(delegato.TRBBONUS))
                    datiPensioneFondoDatiGenerici.AttribuzioneBonus = delegato.TRBBONUS == "SI" ? true : false;
                datiPensioneFondoDatiGenerici.InizioBonus = Utility.DataFromInt(delegato.TRBANNODALBONUS, delegato.TRBMESEDALBONUS, 1);
                datiPensioneFondoDatiGenerici.FineBonus = Utility.DataFromInt(delegato.TRBANNOALBONUS, delegato.TRBMESEALBONUS, 1);
            }

            if (!datiPensioneFondoDatiGenerici.BypassDinamicoCodiceSpecifico.Value && (datiPensioneFondoDatiGenerici == null || !datiPensioneFondoDatiGenerici.CodiceSpecifico.HasValue))
            {
                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    bool isCategoriaINPDAP = IsCategoriaINPDAP(AreaPrelievo);
                    Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];
                    string siglaCategoria = GetCategoriaFromTRACATEG(anagrafica.TRACATEG);
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);

                    byte tipoSelezionabile = 0;
                    if (!string.IsNullOrEmpty(siglaCategoria))
                    {
                        switch (siglaCategoria[0])
                        {
                            case 'V':
                                tipoSelezionabile = 1;
                                break;
                            case 'I':
                                tipoSelezionabile = 2;
                                break;
                            case 'S':
                                tipoSelezionabile = 3;
                                break;
                        }

                        if (!String.IsNullOrEmpty(anagrafica.TRAUFPAG) && anagrafica.TRAUFPAG.Length > 2)
                        {
                            char? tipoPensione = Utility.StringToNullableChar(anagrafica.TRAUFPAG.Substring(2, 1));
                            if (tipoPensione.HasValue)
                            {
                                if (tipoFondo.HasValue)
                                {
                                    List<GestioneDecodifica.CodiceSpecifico> listaCodiciSpecifici = null;
                                    GestioneDecodifica.GetCodiceSpecifico(out listaCodiciSpecifici);
                                    if (listaCodiciSpecifici != null)
                                    {
                                        GestioneDecodifica.CodiceSpecifico codSpec = listaCodiciSpecifici.FirstOrDefault(x => x.TipoPensione == tipoPensione && x.Fondo == tipoFondo.Value.ToString() &&
                                            x.TipoSelezionabile.GetValueOrDefault() == tipoSelezionabile);
                                        if (codSpec != null)
                                        {
                                            if (datiPensioneFondoDatiGenerici == null)
                                                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();

                                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codSpec.Id;
                                        }
                                    }
                                }
                                else if (isCategoriaINPDAP)
                                {
                                    List<GestioneDecodifica.CodiceSpecifico> listaCodiciSpecifici = null;
                                    GestioneDecodifica.GetCodiceSpecifico(out listaCodiciSpecifici);
                                    if (listaCodiciSpecifici != null)
                                    {
                                        GestioneDecodifica.CodiceSpecifico codSpec = listaCodiciSpecifici.FirstOrDefault(x => x.TipoPensione == tipoPensione && x.Fondo == "DAP" &&
                                            x.TipoSelezionabile.GetValueOrDefault() == tipoSelezionabile);
                                        if (codSpec != null)
                                        {
                                            if (datiPensioneFondoDatiGenerici == null)
                                                datiPensioneFondoDatiGenerici = new GestioneFondo.DatiFondo();

                                            datiPensioneFondoDatiGenerici.CodiceSpecifico = codSpec.Id;
                                        }
                                    }

                                    //sovrascrivo valore per GDP sia RIC che PL REV SIN se ricevono la X
                                    if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0)
                                    {
                                        Data.CMSGTRA.Fondo.GDP fondoGDP = AreaPrelievo.FinalResponse.ListaFondoGDP[0];
                                        if (fondoGDP != null)
                                        {
                                            datiPensioneFondoDatiGenerici.CodiceSpecificoGP = fondoGDP.CSPEC_GDP;
                                            if (fondoGDP.CSPEC_GDP == "X")
                                            {
                                                datiPensioneFondoDatiGenerici.CodiceSpecifico = null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ValorizzaPensioneFondoDatiEL(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoEL datiPensioneFondoDatiEL)
        {
            datiPensioneFondoDatiEL = null;
            #region ListaFondoEL
            if (AreaPrelievo.FinalResponse.ListaFondoEL != null && AreaPrelievo.FinalResponse.ListaFondoEL.Count > 0)
            {
                datiPensioneFondoDatiEL = new GestioneFondo.DatiFondoEL();
                Data.CMSGTRA.Fondo.EL fondoEL = AreaPrelievo.FinalResponse.ListaFondoEL[0];

                datiPensioneFondoDatiEL.GradoInvalidita = fondoEL.XELGRADO != 0 ? Utility.StringToNullableByte(fondoEL.XELGRADO.ToString()) : null;
                datiPensioneFondoDatiEL.DecorrenzaTeorica = Utility.DataFromInt(fondoEL.XELTEOAA, fondoEL.XELTEOMM, 1);
                datiPensioneFondoDatiEL.AnnoRiscatti = fondoEL.XELRISAA != 0 ? Utility.StringToNullableByte(fondoEL.XELRISAA.ToString()) : null;
                datiPensioneFondoDatiEL.MeseRiscatti = fondoEL.XELRISMM != 0 ? Utility.StringToNullableByte(fondoEL.XELRISMM.ToString()) : null;
                datiPensioneFondoDatiEL.AnnoAnzianitaPregressa = fondoEL.XELPREAA != 0 ? Utility.StringToNullableByte(fondoEL.XELPREAA.ToString()) : null;
                datiPensioneFondoDatiEL.MeseAnzianitaPregressa = fondoEL.XELPREMM != 0 ? Utility.StringToNullableByte(fondoEL.XELPREMM.ToString()) : null;
                datiPensioneFondoDatiEL.AnnoServizioMilitare = fondoEL.XELMILAA != 0 ? Utility.StringToNullableByte(fondoEL.XELMILAA.ToString()) : null;
                datiPensioneFondoDatiEL.MeseServizioMilitare = fondoEL.XELMILMM != 0 ? Utility.StringToNullableByte(fondoEL.XELMILMM.ToString()) : null;
                datiPensioneFondoDatiEL.AnnoArt3Legge107971 = fondoEL.XELAR3AA != 0 ? Utility.StringToNullableByte(fondoEL.XELAR3AA.ToString()) : null;
                datiPensioneFondoDatiEL.MeseArt3Legge107971 = fondoEL.XELAR3MM != 0 ? Utility.StringToNullableByte(fondoEL.XELAR3MM.ToString()) : null;
                if (fondoEL.XELPRENE != 0)
                    datiPensioneFondoDatiEL.ProRataEnel = Utility.StringToNullableByte(fondoEL.XELPRENE.ToString());

                List<GestioneDecodifica.CodiceAzienda> listaCodiceAzienda = null;
                GestioneDecodifica.GetCodiceAzienda(out listaCodiceAzienda);
                GestioneDecodifica.CodiceAzienda codiceAzienda = listaCodiceAzienda.Find(x => x.TraduzioneGp == fondoEL.XELAZIEN.ToString() && x.Fondo == "EL");
                if (codiceAzienda != null)
                    datiPensioneFondoDatiEL.CodiceAzienda = codiceAzienda.Id;

                datiPensioneFondoDatiEL.ConvenzioneInternazionale = Utility.StringToNullableChar(fondoEL.XELCONVE);

                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    {
                        datiPensioneFondoDatiEL.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiEL.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiEL.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiEL.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }
                }
            }
            #endregion ListaFondoEL
        }

        public static void ValorizzaPensioneFondoDatiTT(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoTT datiPensioneFondoDatiTT)
        {
            datiPensioneFondoDatiTT = null;
            #region ListaFondoTT
            if (AreaPrelievo.FinalResponse.ListaFondoTT != null && AreaPrelievo.FinalResponse.ListaFondoTT.Count > 0)
            {
                datiPensioneFondoDatiTT = new GestioneFondo.DatiFondoTT();
                Data.CMSGTRA.Fondo.TT fondoTT = AreaPrelievo.FinalResponse.ListaFondoTT[0];

                datiPensioneFondoDatiTT.ConvenzioneInternazionale = Utility.StringToNullableChar(fondoTT.XTTCONVE);

                if (!string.IsNullOrEmpty(fondoTT.XTTDITTA) && fondoTT.XTTDITTA.Trim() != string.Empty)
                {
                    List<GestioneDecodifica.CodiceAzienda> listaCodiceAzienda = null;
                    GestioneDecodifica.GetCodiceAzienda(out listaCodiceAzienda);
                    GestioneDecodifica.CodiceAzienda codiceAzienda = listaCodiceAzienda.Find(x => x.TraduzioneGp == fondoTT.XTTDITTA.Trim() && x.Fondo == "TT");
                    if (codiceAzienda != null)
                        datiPensioneFondoDatiTT.Ditta = codiceAzienda.Id;
                }

                datiPensioneFondoDatiTT.RetribuzioneMensileInail = fondoTT.XTTINAEF;
                datiPensioneFondoDatiTT.RenditaInailAnnua = fondoTT.XTTINARE;
                if (!string.IsNullOrEmpty(fondoTT.XTTLEG58))
                    datiPensioneFondoDatiTT.CodiceArt5L58 = fondoTT.XTTLEG58 == "1" ? true : false;
                datiPensioneFondoDatiTT.PensioneDirettaGenitori = fondoTT.XTTPNGEN;
                datiPensioneFondoDatiTT.PeriodiFigurativiGiorni = fondoTT.XTTRISFGGG;
                datiPensioneFondoDatiTT.PeriodiFigurativiMesi = fondoTT.XTTRISFGMM;
                datiPensioneFondoDatiTT.PeriodiFigurativiAnni = fondoTT.XTTRISFGAA;
                datiPensioneFondoDatiTT.RiscattiContributiFissiGiorni = fondoTT.XTTRISFIGG;
                datiPensioneFondoDatiTT.RiscattiContributiFissiMesi = fondoTT.XTTRISFIMM;
                datiPensioneFondoDatiTT.RiscattiContributiFissiAnni = fondoTT.XTTRISFIAA;
                datiPensioneFondoDatiTT.RiscattiRiservaMatematicaGiorni = fondoTT.XTTRISMTGG;
                datiPensioneFondoDatiTT.RiscattiRiservaMatematicaMesi = fondoTT.XTTRISMTMM;
                datiPensioneFondoDatiTT.RiscattiRiservaMatematicaAnni = fondoTT.XTTRISMTAA;
                datiPensioneFondoDatiTT.SupplementoLegge58367 = fondoTT.XTTSPOBG;
                datiPensioneFondoDatiTT.DecorrenzaTeorica = Utility.DataFromInt(fondoTT.XTTTEOAA, fondoTT.XTTTEOMM, 1);
                if (fondoTT.XTTRTULT != 0M)
                    datiPensioneFondoDatiTT.RetribuzioneUltimoAnnoQuotaA = fondoTT.XTTRTULT;
                if (fondoTT.XTTRTBIE != 0M)
                    datiPensioneFondoDatiTT.RetribuzioneBiennio = fondoTT.XTTRTBIE;
                if (fondoTT.XTTACCES != 0M)
                    datiPensioneFondoDatiTT.ElementiAccessori = fondoTT.XTTACCES;
                if (fondoTT.XTTPEN53 != 0M)
                    datiPensioneFondoDatiTT.PensioneMensileAl53 = fondoTT.XTTPEN53;
                if (fondoTT.XTTRTSUP != 0M)
                    datiPensioneFondoDatiTT.RetribuzioneSupplementi = fondoTT.XTTRTSUP;

                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    {
                        datiPensioneFondoDatiTT.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiTT.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiTT.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiTT.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }

                    if (!string.IsNullOrEmpty(anagrafica.TRADIMISSIONI))
                        datiPensioneFondoDatiTT.DimissioniAnte97 = anagrafica.TRADIMISSIONI.Trim().ToUpperInvariant() == "SI" ? true : anagrafica.TRADIMISSIONI.Trim().ToUpperInvariant() == "NO" ? false : (bool?)null;
                }
            }
            #endregion ListaFondoTT
        }

        public static void ValorizzaPensioneFondoDatiET(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoET datiPensioneFondoDatiET)
        {
            datiPensioneFondoDatiET = null;
            #region ListaFondoET
            if (AreaPrelievo.FinalResponse.ListaFondoET != null && AreaPrelievo.FinalResponse.ListaFondoET.Count > 0)
            {
                datiPensioneFondoDatiET = new GestioneFondo.DatiFondoET();
                Data.CMSGTRA.Fondo.ET fondoET = AreaPrelievo.FinalResponse.ListaFondoET[0];

                datiPensioneFondoDatiET.Competenze40Percento = fondoET.XETCOM40;
                if (fondoET.XETEFFET != 0M)
                    datiPensioneFondoDatiET.RetribuzioneEffettiva = fondoET.XETEFFET;
                if (fondoET.XETGRADO != 0)
                    datiPensioneFondoDatiET.GradoInvalidita = Utility.StringToNullableByte(fondoET.XETGRADO.ToString());
                if (fondoET.XETINAIL != 0M)
                    datiPensioneFondoDatiET.ImportoRenditaInail = fondoET.XETINAIL;
                datiPensioneFondoDatiET.Importo13ma = fondoET.XETMES13;
                datiPensioneFondoDatiET.Importo14ma = fondoET.XETMES14;
                datiPensioneFondoDatiET.Stipendio = fondoET.XETPGTAB;
                if (fondoET.XETRETES != 0M)
                    datiPensioneFondoDatiET.RetribuzioneEsodo = fondoET.XETRETES;
                if (fondoET.XETSLEVA != 0)
                    datiPensioneFondoDatiET.NSettimaneLeva = fondoET.XETSLEVA;
                if (fondoET.XETSRICH != 0)
                    datiPensioneFondoDatiET.NSettimaneRichiamato = fondoET.XETSRICH;
                datiPensioneFondoDatiET.DecorrenzaTeorica = Utility.DataFromInt(fondoET.XETTEOAA, fondoET.XETTEOMM, 1);
                datiPensioneFondoDatiET.DataEsonero = Utility.DataFromInt(fondoET.XETAAESO, fondoET.XETMMESO, fondoET.XETGGESO);
                datiPensioneFondoDatiET.ElementiAccessori = fondoET.XETACCES;
                if (fondoET.XETAG402 != 0M)
                    datiPensioneFondoDatiET.ContributiAgoLegge40245 = fondoET.XETAG402;
                if (fondoET.XETAG140 != 0M)
                    datiPensioneFondoDatiET.ContributiAgoLegge140830 = fondoET.XETAG140;
                List<GestioneDecodifica.CodiceAzienda> listaCodiceAzienda = null;
                GestioneDecodifica.GetCodiceAzienda(out listaCodiceAzienda);
                GestioneDecodifica.CodiceAzienda codiceAzienda = listaCodiceAzienda.Find(x => x.TraduzioneGp == (fondoET.XETCODAZ + fondoET.XETNUMAZ.ToString().PadLeft(5, '0'))
                    && x.Fondo == "ET");
                if (codiceAzienda != null)
                    datiPensioneFondoDatiET.CodAzienda = codiceAzienda.Id;
                datiPensioneFondoDatiET.CodiceEsodo = fondoET.XETCODES == 0 ? false : fondoET.XETCODES == 1 ? true : (bool?)null;
                if (fondoET.XETINTAA != 0 || fondoET.XETINTMM != 0 || fondoET.XETINTGG != 0)
                {
                    datiPensioneFondoDatiET.AAInterruzione = fondoET.XETINTAA;
                    datiPensioneFondoDatiET.MMInterruzione = fondoET.XETINTMM;
                    datiPensioneFondoDatiET.GGInterruzione = fondoET.XETINTGG;
                }
                datiPensioneFondoDatiET.CodiceServizioMilitare = fondoET.XETMILIT == 0 ? false : fondoET.XETMILIT == 1 ? true : (bool?)null;
                datiPensioneFondoDatiET.PartTime = fondoET.XETPTCOD == 0 ? false : fondoET.XETPTCOD == 1 ? true : (bool?)null;

                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    {
                        datiPensioneFondoDatiET.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiET.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiET.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiET.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }
                }
                if (AreaPrelievo.FinalResponse.ListaAgoET != null && AreaPrelievo.FinalResponse.ListaAgoET.Count > 0)
                {
                    Data.CMSGTRA.Ago.ET agoET = AreaPrelievo.FinalResponse.ListaAgoET[0];
                    if (agoET.YETANZTO != 0)
                        datiPensioneFondoDatiET.SetAnzTotAltraPensione = agoET.YETANZTO;
                    if (agoET.YETBASEA != 0)
                        datiPensioneFondoDatiET.BaseAltraPensione = agoET.YETBASEA;
                    if (!string.IsNullOrEmpty(agoET.YETCATEG))
                        datiPensioneFondoDatiET.CategoriaAltraPensione = agoET.YETCATEG;
                    if (agoET.YETCERTI != 0)
                        datiPensioneFondoDatiET.CertificatoAltraPensione = agoET.YETCERTI;
                    if (agoET.YETMEDIM != 0)
                        datiPensioneFondoDatiET.RmsImpAltraPensione = agoET.YETMEDIM;
                    datiPensioneFondoDatiET.DecorrenzaAltraPensione = Utility.DataFromInt(agoET.YETORIAA, agoET.YETORIMM, 1);
                    if (agoET.YETRIVPR != 0)
                        datiPensioneFondoDatiET.RevAltraPensione = agoET.YETRIVPR;
                    datiPensioneFondoDatiET.DecorrenzaPrimoSupplemento = Utility.DataFromInt(agoET.YETSP1AA, agoET.YETSP1MM, 1);
                    if (agoET.YETSP1CT != 0)
                        datiPensioneFondoDatiET.ImpContribPrimoSupplemento = agoET.YETSP1CT;
                    datiPensioneFondoDatiET.DecorrenzaSecondoSupplemento = Utility.DataFromInt(agoET.YETSP2AA, agoET.YETSP2MM, 1);
                    if (agoET.YETSP2CT != 0)
                        datiPensioneFondoDatiET.ImpContribSecondoSupplemento = agoET.YETSP2CT;
                    if (agoET.YETTIPLQ != 0)
                        datiPensioneFondoDatiET.TipoLiquidazione = (byte)agoET.YETTIPLQ;
                }
            }
            #endregion ListaFondoET
        }

        public static void ValorizzaDatiINAIL(Data.FSPR AreaPrelievo, out List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaInail)
        {
            listaInail = null;

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.ListaRenditaINAIL != null)
            {
                listaInail = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();

                Data.CMSGTRA.RenditaINAIL dati = AreaPrelievo.FinalResponse.ListaRenditaINAIL[0];

                if (dati.LISTTRGELERD != null && dati.LISTTRGELERD.Count > 0)
                {
                    foreach (Data.CMSGTRA.RenditaINAIL.TRGELERD iN in dati.LISTTRGELERD)
                    {
                        if (iN.TRLDEC01 != 0 || !string.IsNullOrEmpty(iN.TRLEVE01) || iN.TRLIMP01 != 0)
                        {
                            GestionePensioneInailInabilita.DatiPensioniINAIL inail = new GestionePensioneInailInabilita.DatiPensioniINAIL();
                            if (iN.TRLDEC01 > 0)
                                inail.DecorrenzaRenditaInail = Utility.DataFromString(iN.TRLDEC01.ToString() + "01", Utility.FormatoData.AAAAmmGG);
                            inail.Evento = iN.TRLEVE01 == "1" ? true : false;
                            inail.ImportoMensileInail = iN.TRLIMP01;
                            listaInail.Add(inail);
                        }
                    }
                }
            }
        }

        public static void ValorizzaPensioneFondoDatiVL(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoVL datiPensioneFondoDatiVL)
        {
            datiPensioneFondoDatiVL = null;
            #region ListaFondoVL
            if (AreaPrelievo.FinalResponse.ListaFondoVL != null && AreaPrelievo.FinalResponse.ListaFondoVL.Count > 0)
            {
                datiPensioneFondoDatiVL = new GestioneFondo.DatiFondoVL();
                Data.CMSGTRA.Fondo.VL fondoVL = AreaPrelievo.FinalResponse.ListaFondoVL[0];

                if (fondoVL.XVLART22 != 0)
                    datiPensioneFondoDatiVL.CodiceArt22 = Utility.StringToNullableByte(fondoVL.XVLART22.ToString());
                if (fondoVL.XVLIMPCP != 0M)
                    datiPensioneFondoDatiVL.ImportoPercentualeCapitalizzazione = fondoVL.XVLIMPCP;
                if (fondoVL.XVLCODCP != 0)
                    datiPensioneFondoDatiVL.CodiceCapitalizzazione = Utility.StringToNullableByte(fondoVL.XVLCODCP.ToString());
                datiPensioneFondoDatiVL.DataInvalidita = Utility.DataFromInt(fondoVL.XVLINVAA, fondoVL.XVLINVMM, fondoVL.XVLINVGG);
                if (fondoVL.XVLIRINT != 0 || fondoVL.XVLIRDEC != 0)
                    datiPensioneFondoDatiVL.AliquotaIrpef = Utility.StringToNullableDecimal(fondoVL.XVLIRINT.ToString() + "," + fondoVL.XVLIRDEC.ToString());
                if (fondoVL.XVLRISAA != 0)
                    datiPensioneFondoDatiVL.RiscattiRicongiunzioniAA = fondoVL.XVLRISAA;
                if (fondoVL.XVLRISMM != 0)
                    datiPensioneFondoDatiVL.RiscattiRicongiunzioniMM = fondoVL.XVLRISMM;
                if (fondoVL.XVLRISGG != 0)
                    datiPensioneFondoDatiVL.RiscattiRicongiunzioniGG = fondoVL.XVLRISGG;
                if (fondoVL.XVLVOLAA != 0)
                    datiPensioneFondoDatiVL.ProsecuzioneVolontariaAA = fondoVL.XVLVOLAA;
                if (fondoVL.XVLVOLMM != 0)
                    datiPensioneFondoDatiVL.ProsecuzioneVolontariaMM = fondoVL.XVLVOLMM;
                if (fondoVL.XVLVOLGG != 0)
                    datiPensioneFondoDatiVL.ProsecuzioneVolontariaGG = fondoVL.XVLVOLGG;
                if (fondoVL.XVLSETT1 != 0M)
                    datiPensioneFondoDatiVL.RetribuzioneSettimanaleAgoQuotaA = fondoVL.XVLSETT1;
                if (fondoVL.XVLSETT2 != 0M)
                    datiPensioneFondoDatiVL.RetribuzioneSettimanaleAgoQuotaB = fondoVL.XVLSETT2;

                if (AreaPrelievo.FinalResponse.ListaAgoVL != null && AreaPrelievo.FinalResponse.ListaAgoVL.Count > 0)
                {
                    Data.CMSGTRA.Ago.VL agoVL = AreaPrelievo.FinalResponse.ListaAgoVL[0];

                    if (!string.IsNullOrEmpty(agoVL.YVLPRECO))
                        datiPensioneFondoDatiVL.LavoratorePrecoce = agoVL.YVLPRECO == "S" ? true : agoVL.YVLPRECO == "N" ? false : (bool?)null;
                }

                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    {
                        datiPensioneFondoDatiVL.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiVL.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiVL.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiVL.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }
                }
            }
            #endregion ListaFondoVL
        }

        public static void ValorizzaPensioneFondoDatiPT(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, ref GestioneFondo.DatiFondo datiFondo,
            out List<GestioneFondo.DatiFondoPT> listaDatiPensioneFondoDatiPT)
        {
            listaDatiPensioneFondoDatiPT = null;
            #region ListaFondoPT
            if (AreaPrelievo.UtilizzaNuovoTracciato)
            {
                if (AreaPrelievo.FinalResponse.ListaFondoPT_New != null && AreaPrelievo.FinalResponse.ListaFondoPT_New.Count > 0)
                {
                    listaDatiPensioneFondoDatiPT = new List<GestioneFondo.DatiFondoPT>();
                    foreach (Data.CMSGTRA.Fondo.PT_New fondoPT in AreaPrelievo.FinalResponse.ListaFondoPT_New)
                    {
                        GestioneFondo.DatiFondoPT datiPensioneFondoDatiPT = new GestioneFondo.DatiFondoPT();
                        datiPensioneFondoDatiPT.IdRecordFondo = fondoPT.XFSPROGR;

                        if (fondoPT.XFSCAUSA != 0)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            GestioneDecodifica.DecodificaCausaCessazione causaCess = null;
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                                causaCess = ListaCausaCess.Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == fondoPT.XFSCAUSA.ToString().Trim().ToUpperInvariant() && x.Fondo == "PT");
                            datiPensioneFondoDatiPT.CausaCessazione = causaCess != null ? causaCess.Id : (long?)null;
                        }
                        datiPensioneFondoDatiPT.DecorrenzaCalcolo = Utility.DataFromString(fondoPT.XFSDECAL.ToString().PadLeft(8, '0'), Utility.FormatoData.AAAAmmGG);
                        datiPensioneFondoDatiPT.DecorrenzaEconomica = Utility.DataFromInt(fondoPT.XFSDECECAA, fondoPT.XFSDECECMM, fondoPT.XFSDECECGG);
                        if (fondoPT.XFSF13ME != 0)
                            datiPensioneFondoDatiPT.TrediciMensilita = fondoPT.XFSF13ME == 1 ? true : false;
                        if (fondoPT.XFSPAL != 0M)
                        {
                            datiPensioneFondoDatiPT.PensioneAnnuaLorda = fondoPT.XFSPAL;
                            //ENG - PL Reversibilita 024
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiPT.IsPensioneAnnuaLordaDaPrelievo = true;
                            }
                        }
                        datiPensioneFondoDatiPT.TitolareAltraPensione = string.IsNullOrEmpty(fondoPT.XFSNATU1) ? (bool?)null : ((fondoPT.XFSNATU1 == "2" || fondoPT.XFSNATU1 == "6") ? true : false);


                        if (fondoPT.XFSPENS != 0)
                        {
                            datiPensioneFondoDatiPT.SiglaCategoria = int.Parse(fondoPT.XFSPENS.ToString().PadLeft(15, '0').Substring(0, 3));
                            datiPensioneFondoDatiPT.CodiceSede = short.Parse(fondoPT.XFSPENS.ToString().PadLeft(15, '0').Substring(3, 4));
                            datiPensioneFondoDatiPT.Ncertificato = int.Parse(fondoPT.XFSPENS.ToString().PadLeft(15, '0').Substring(7, 8));

                            datiPensioneFondoDatiPT.NMesiRiscattati = Utility.StringToNullableInt(fondoPT.XFSMESIRIS.ToString());
                            datiPensioneFondoDatiPT.NMesiTotali = Utility.StringToNullableInt(fondoPT.XFSMESITOT.ToString());

                            if (fondoPT.XFSDECAA != 0 && fondoPT.XFSDECMM != 0 && fondoPT.XFSDECGG != 0)
                                datiPensioneFondoDatiPT.DecorrenzaSecondaria = Utility.DataFromInt(fondoPT.XFSDECAA, fondoPT.XFSDECMM, fondoPT.XFSDECGG);
                        }

                        if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                        {

                            int? PrivilegiataSuperinvaliditaIndennita = null;
                            int? AssegnoIntegrativo = null;
                            int? IntegrazioneIndennitaAssistenza = null;
                            int? IndennitaAccompagnamentoAggiuntiva = null;
                            int? CumuloInfermita = null;
                            int? Categoria2aInfermita = null;
                            int? AssegnoCura = null;
                            int? IndennitaSpecialeAnnua = null;
                            DecodeASSACByFondo(fondoPT.XFSASSAC, "PT", out PrivilegiataSuperinvaliditaIndennita, out AssegnoIntegrativo,
                                out IntegrazioneIndennitaAssistenza, out IndennitaAccompagnamentoAggiuntiva, out CumuloInfermita,
                                out Categoria2aInfermita, out AssegnoCura, out IndennitaSpecialeAnnua);
                            datiPensioneFondoDatiPT.PrivilegiataSuperinvaliditaIndennita = PrivilegiataSuperinvaliditaIndennita;
                            datiPensioneFondoDatiPT.AssegnoIntegrativo = AssegnoIntegrativo;
                            datiPensioneFondoDatiPT.IntegrazioneIndennitaAssistenza = IntegrazioneIndennitaAssistenza;
                            datiPensioneFondoDatiPT.IndennitaAccompagnamentoAggiuntiva = IndennitaAccompagnamentoAggiuntiva;
                            datiPensioneFondoDatiPT.CumuloInfermita = CumuloInfermita;
                            datiPensioneFondoDatiPT.Categoria2aInfermita = Categoria2aInfermita;
                            datiPensioneFondoDatiPT.AssegnoCura = AssegnoCura;
                            datiPensioneFondoDatiPT.IndennitaSpecialeAnnua = IndennitaSpecialeAnnua;

                            if (fondoPT.XFSASSAC != 0)
                            {
                                if (datiFondo == null)
                                    datiFondo = new GestioneFondo.DatiFondo();
                                datiFondo.Privilegiate = true;
                            }
                        }

                        if (fondoPT.XFSSUAN != 0 || fondoPT.XFSSUANMM != 0 || fondoPT.XFSSUANGG != 0)
                        {
                            datiPensioneFondoDatiPT.ServizioUtileDirittoAA = fondoPT.XFSSUAN;
                            datiPensioneFondoDatiPT.ServizioUtileDirittoMM = fondoPT.XFSSUANMM;
                            datiPensioneFondoDatiPT.ServizioUtileDirittoGG = fondoPT.XFSSUANGG;

                        }

                        if (!string.IsNullOrEmpty(fondoPT.XFSCONG) && fondoPT.XFSCONG != "00")
                        {
                            datiPensioneFondoDatiPT.IndennitaIntegrativaSpecialeConglobata = fondoPT.XFSCONG.PadRight(2, '0').Substring(0, 1) == "1" ? true : false;
                            datiPensioneFondoDatiPT.IntegrazioneMinimo = fondoPT.XFSCONG.PadRight(2, '0').Substring(1, 1) == "1" ? true : false;
                        }

                        if (fondoPT.XFSDIIS >= 0)
                        {
                            switch (fondoPT.XFSDIIS)
                            {
                                case 0:
                                    datiPensioneFondoDatiPT.DirittoIndennitaIntegrativaSpeciale = false;
                                    break;
                                case 1:
                                    datiPensioneFondoDatiPT.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiPT.PagamentoIndennitaIntegrativaSpeciale = true;
                                    break;
                                case 2:
                                    datiPensioneFondoDatiPT.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiPT.PagamentoIndennitaIntegrativaSpeciale = false;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(fondoPT.XFSRID))
                        {
                            switch (fondoPT.XFSRID)
                            {
                                case "0":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = false;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = false;
                                    break;
                                case "1":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = true;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = false;
                                    break;
                                case "2":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = false;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = true;
                                    break;
                                case "3":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = true;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (fondoPT.XFSONEREMEF == 0)
                        {
                            datiPensioneFondoDatiPT.OnereMEF = false;
                        }
                        else if (fondoPT.XFSONEREMEF == 1)
                        {
                            datiPensioneFondoDatiPT.OnereMEF = true;
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                                datiPensioneFondoDatiPT.IsOnereMefFromGpUgualeSI = true;
                        }

                        datiPensioneFondoDatiPT.RipartizioneInpdap = fondoPT.XRIPINPDAP;

                        if (fondoPT.XFSDECECAA != 0 && fondoPT.XFSDECECMM != 0 && fondoPT.XFSDECECGG != 0)
                        {
                            if (fondoPT.XFSDECECAA == 9999 || fondoPT.XFSDECECMM == 99 || fondoPT.XFSDECECGG == 99)
                                datiPensioneFondoDatiPT.ScadenzaIllimitata = true;
                            else
                                datiPensioneFondoDatiPT.ScadenzaBenefici = Utility.DataFromInt(fondoPT.XFSDECECAA, fondoPT.XFSDECECMM, fondoPT.XFSDECECGG);
                        }
                        if (fondoPT.XFSIMPC != 0M)
                            datiPensioneFondoDatiPT.IncrementoContrattuale = fondoPT.XFSIMPC;

                        if (fondoPT.XFSPAL335 != 0M)
                            datiPensioneFondoDatiPT.PALConBenefici = fondoPT.XFSPAL335;
                        if (fondoPT.XFSSETDIR != 0)
                            datiPensioneFondoDatiPT.VVUtiliDiritto = fondoPT.XFSSETDIR;
                        if (fondoPT.XFSSETMIS != 0)
                            datiPensioneFondoDatiPT.VVUtiliMisura = fondoPT.XFSSETMIS;

                        if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                        {
                            if (datiPensioneFondoDatiPT == null)
                                datiPensioneFondoDatiPT = new GestioneFondo.DatiFondoPT();

                            Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                            if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiPT.RequisitiAnte247 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                                datiPensioneFondoDatiPT.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                                if (anagrafica.TRARECUP != 0)
                                {
                                    datiPensioneFondoDatiPT.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                                    datiPensioneFondoDatiPT.TrimesteRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                                }
                            }

                            string siglaCategoria = GetCategoriaFromTRACATEG(anagrafica.TRACATEG.PadRight(8));
                            string tipoReversibilita = AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRATIPIR;
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && !String.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant().StartsWith("S") &&
                                !String.IsNullOrEmpty(tipoReversibilita) && tipoReversibilita.Trim().ToUpperInvariant() == "R")
                            {
                                datiPensioneFondoDatiPT.TipologiaPensione = fondoPT.XFSTPENS;
                            }
                        }


                        listaDatiPensioneFondoDatiPT.Add(datiPensioneFondoDatiPT);
                    }
                }

                if (AreaPrelievo.FinalResponse.ListaAgoPT != null && AreaPrelievo.FinalResponse.ListaAgoPT.Count > 0)
                {
                    if (listaDatiPensioneFondoDatiPT == null)
                        listaDatiPensioneFondoDatiPT = new List<GestioneFondo.DatiFondoPT>();

                    foreach (Data.CMSGTRA.Ago.PT agoPt in AreaPrelievo.FinalResponse.ListaAgoPT)
                    {
                        if (!listaDatiPensioneFondoDatiPT.Exists(x => x.IdRecordFondo == agoPt.YFSPROGR))
                            listaDatiPensioneFondoDatiPT.Add(new GestioneFondo.DatiFondoPT { IdRecordFondo = agoPt.YFSPROGR });
                        GestioneFondo.DatiFondoPT datiPensioneFondoDatiPT = listaDatiPensioneFondoDatiPT.FirstOrDefault(x => x.IdRecordFondo == agoPt.YFSPROGR);

                        if (agoPt.YFSPAL707 != 0M)
                        {
                            datiPensioneFondoDatiPT.PensioneAnnuaLorda707 = agoPt.YFSPAL707;
                            //ENG - PL Reversibilita 024
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiPT.IsPensioneAnnuaLorda707DaPrelievo = true;
                            }
                        }
                        if (agoPt.YFSCOEFTRA != 0M)
                            datiPensioneFondoDatiPT.CoefficienteTrasformazione = agoPt.YFSCOEFTRA;
                        if (agoPt.YFSPAL214 != 0M)
                            datiPensioneFondoDatiPT.PensioneAnnuaLorda214 = agoPt.YFSPAL214;
                    }
                }
            }
            else
            {
                if (AreaPrelievo.FinalResponse.ListaFondoPT != null && AreaPrelievo.FinalResponse.ListaFondoPT.Count > 0)
                {
                    listaDatiPensioneFondoDatiPT = new List<GestioneFondo.DatiFondoPT>();
                    foreach (Data.CMSGTRA.Fondo.PT fondoPT in AreaPrelievo.FinalResponse.ListaFondoPT)
                    {
                        GestioneFondo.DatiFondoPT datiPensioneFondoDatiPT = new GestioneFondo.DatiFondoPT();
                        datiPensioneFondoDatiPT.IdRecordFondo = fondoPT.XFSPROGR;

                        if (fondoPT.XFSCAUSA != 0)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            GestioneDecodifica.DecodificaCausaCessazione causaCess = null;
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                                causaCess = ListaCausaCess.Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == fondoPT.XFSCAUSA.ToString().Trim().ToUpperInvariant() && x.Fondo == "PT");
                            datiPensioneFondoDatiPT.CausaCessazione = causaCess != null ? causaCess.Id : (long?)null;
                        }
                        datiPensioneFondoDatiPT.DecorrenzaCalcolo = Utility.DataFromString(fondoPT.XFSDECAL.ToString().PadLeft(8, '0'), Utility.FormatoData.AAAAmmGG);
                        datiPensioneFondoDatiPT.DecorrenzaEconomica = Utility.DataFromInt(fondoPT.XFSDECECAA, fondoPT.XFSDECECMM, fondoPT.XFSDECECGG);
                        if (fondoPT.XFSF13ME != 0)
                            datiPensioneFondoDatiPT.TrediciMensilita = fondoPT.XFSF13ME == 1 ? true : false;
                        if (fondoPT.XFSPAL != 0M)
                            datiPensioneFondoDatiPT.PensioneAnnuaLorda = fondoPT.XFSPAL;

                        datiPensioneFondoDatiPT.TitolareAltraPensione = string.IsNullOrEmpty(fondoPT.XFSNATU1) ? (bool?)null : ((fondoPT.XFSNATU1 == "2" || fondoPT.XFSNATU1 == "6") ? true : false);


                        if (fondoPT.XFSPENS != 0)
                        {
                            datiPensioneFondoDatiPT.SiglaCategoria = int.Parse(fondoPT.XFSPENS.ToString().PadLeft(15, '0').Substring(0, 3));
                            datiPensioneFondoDatiPT.CodiceSede = short.Parse(fondoPT.XFSPENS.ToString().PadLeft(15, '0').Substring(3, 4));
                            datiPensioneFondoDatiPT.Ncertificato = int.Parse(fondoPT.XFSPENS.ToString().PadLeft(15, '0').Substring(7, 8));

                            datiPensioneFondoDatiPT.NMesiRiscattati = Utility.StringToNullableInt(fondoPT.XFSMESIRIS.ToString());
                            datiPensioneFondoDatiPT.NMesiTotali = Utility.StringToNullableInt(fondoPT.XFSMESITOT.ToString());

                            if (fondoPT.XFSDECAA != 0 && fondoPT.XFSDECMM != 0 && fondoPT.XFSDECGG != 0)
                                datiPensioneFondoDatiPT.DecorrenzaSecondaria = Utility.DataFromInt(fondoPT.XFSDECAA, fondoPT.XFSDECMM, fondoPT.XFSDECGG);
                        }

                        if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                        {

                            int? PrivilegiataSuperinvaliditaIndennita = null;
                            int? AssegnoIntegrativo = null;
                            int? IntegrazioneIndennitaAssistenza = null;
                            int? IndennitaAccompagnamentoAggiuntiva = null;
                            int? CumuloInfermita = null;
                            int? Categoria2aInfermita = null;
                            int? AssegnoCura = null;
                            int? IndennitaSpecialeAnnua = null;
                            DecodeASSACByFondo(fondoPT.XFSASSAC, "PT", out PrivilegiataSuperinvaliditaIndennita, out AssegnoIntegrativo,
                                out IntegrazioneIndennitaAssistenza, out IndennitaAccompagnamentoAggiuntiva, out CumuloInfermita,
                                out Categoria2aInfermita, out AssegnoCura, out IndennitaSpecialeAnnua);
                            datiPensioneFondoDatiPT.PrivilegiataSuperinvaliditaIndennita = PrivilegiataSuperinvaliditaIndennita;
                            datiPensioneFondoDatiPT.AssegnoIntegrativo = AssegnoIntegrativo;
                            datiPensioneFondoDatiPT.IntegrazioneIndennitaAssistenza = IntegrazioneIndennitaAssistenza;
                            datiPensioneFondoDatiPT.IndennitaAccompagnamentoAggiuntiva = IndennitaAccompagnamentoAggiuntiva;
                            datiPensioneFondoDatiPT.CumuloInfermita = CumuloInfermita;
                            datiPensioneFondoDatiPT.Categoria2aInfermita = Categoria2aInfermita;
                            datiPensioneFondoDatiPT.AssegnoCura = AssegnoCura;
                            datiPensioneFondoDatiPT.IndennitaSpecialeAnnua = IndennitaSpecialeAnnua;

                            if (fondoPT.XFSASSAC != 0)
                            {
                                if (datiFondo == null)
                                    datiFondo = new GestioneFondo.DatiFondo();
                                datiFondo.Privilegiate = true;
                            }
                        }

                        if (fondoPT.XFSSUAN != 0)
                            datiPensioneFondoDatiPT.ServizioUtileDirittoAA = fondoPT.XFSSUAN;

                        if (!string.IsNullOrEmpty(fondoPT.XFSCONG) && fondoPT.XFSCONG != "00")
                        {
                            datiPensioneFondoDatiPT.IndennitaIntegrativaSpecialeConglobata = fondoPT.XFSCONG.PadRight(2, '0').Substring(0, 1) == "1" ? true : false;
                            datiPensioneFondoDatiPT.IntegrazioneMinimo = fondoPT.XFSCONG.PadRight(2, '0').Substring(1, 1) == "1" ? true : false;
                        }

                        if (fondoPT.XFSDIIS >= 0)
                        {
                            switch (fondoPT.XFSDIIS)
                            {
                                case 0:
                                    datiPensioneFondoDatiPT.DirittoIndennitaIntegrativaSpeciale = false;
                                    break;
                                case 1:
                                    datiPensioneFondoDatiPT.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiPT.PagamentoIndennitaIntegrativaSpeciale = true;
                                    break;
                                case 2:
                                    datiPensioneFondoDatiPT.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiPT.PagamentoIndennitaIntegrativaSpeciale = false;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(fondoPT.XFSRID))
                        {
                            switch (fondoPT.XFSRID)
                            {
                                case "0":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = false;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = false;
                                    break;
                                case "1":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = true;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = false;
                                    break;
                                case "2":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = false;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = true;
                                    break;
                                case "3":
                                    datiPensioneFondoDatiPT.RiduzioneL537 = true;
                                    datiPensioneFondoDatiPT.IISAbbattimentoAnni = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (fondoPT.XFSONEREMEF == 0)
                        {
                            datiPensioneFondoDatiPT.OnereMEF = false;
                        }
                        else if (fondoPT.XFSONEREMEF == 1)
                        {
                            datiPensioneFondoDatiPT.OnereMEF = true;
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                                datiPensioneFondoDatiPT.IsOnereMefFromGpUgualeSI = true;
                        }

                        datiPensioneFondoDatiPT.RipartizioneInpdap = fondoPT.XRIPINPDAP;

                        if (fondoPT.XFSDECECAA != 0 && fondoPT.XFSDECECMM != 0 && fondoPT.XFSDECECGG != 0)
                        {
                            if (fondoPT.XFSDECECAA == 9999 || fondoPT.XFSDECECMM == 99 || fondoPT.XFSDECECGG == 99)
                                datiPensioneFondoDatiPT.ScadenzaIllimitata = true;
                            else
                                datiPensioneFondoDatiPT.ScadenzaBenefici = Utility.DataFromInt(fondoPT.XFSDECECAA, fondoPT.XFSDECECMM, fondoPT.XFSDECECGG);
                        }
                        if (fondoPT.XFSIMPC != 0M)
                            datiPensioneFondoDatiPT.IncrementoContrattuale = fondoPT.XFSIMPC;

                        if (fondoPT.XFSPAL335 != 0M)
                            datiPensioneFondoDatiPT.PALConBenefici = fondoPT.XFSPAL335;

                        if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                        {
                            if (datiPensioneFondoDatiPT == null)
                                datiPensioneFondoDatiPT = new GestioneFondo.DatiFondoPT();

                            Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                            if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiPT.RequisitiAnte247 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                                datiPensioneFondoDatiPT.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                                if (anagrafica.TRARECUP != 0)
                                {
                                    datiPensioneFondoDatiPT.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                                    datiPensioneFondoDatiPT.TrimesteRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                                }
                            }
                        }

                        listaDatiPensioneFondoDatiPT.Add(datiPensioneFondoDatiPT);
                    }
                }
            }
            #endregion ListaFondoPT
        }

        public static void ValorizzaPensioneFondoDatiFS(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, ref GestioneFondo.DatiFondo datiFondo,
            out List<GestioneFondo.DatiFondoFST> listaDatiPensioneFondoDatiFS)
        {
            listaDatiPensioneFondoDatiFS = null;
            #region ListaFondoFS
            if (AreaPrelievo.UtilizzaNuovoTracciato)
            {
                if (AreaPrelievo.FinalResponse.ListaFondoFS_New != null && AreaPrelievo.FinalResponse.ListaFondoFS_New.Count > 0)
                {
                    listaDatiPensioneFondoDatiFS = new List<GestioneFondo.DatiFondoFST>();
                    foreach (Data.CMSGTRA.Fondo.FS_New fondoFS in AreaPrelievo.FinalResponse.ListaFondoFS_New)
                    {
                        GestioneFondo.DatiFondoFST datiPensioneFondoDatiFS = new GestioneFondo.DatiFondoFST();
                        datiPensioneFondoDatiFS.IdRecordFondo = fondoFS.XFSPROGR;

                        if (fondoFS.XFSCAUSA != 0)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            GestioneDecodifica.DecodificaCausaCessazione causaCess = null;
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                                causaCess = ListaCausaCess.Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == fondoFS.XFSCAUSA.ToString().Trim().ToUpperInvariant() && x.Fondo == "FS");
                            datiPensioneFondoDatiFS.CausaCessazione = causaCess != null ? causaCess.Id : 0;
                        }
                        datiPensioneFondoDatiFS.DecorrenzaCalcolo = Utility.DataFromString(fondoFS.XFSDECAL.ToString().PadLeft(8, '0'), Utility.FormatoData.AAAAmmGG);
                        datiPensioneFondoDatiFS.DecorrenzaEconomica = Utility.DataFromInt(fondoFS.XFSDECECAA, fondoFS.XFSDECECMM, fondoFS.XFSDECECGG);
                        if (fondoFS.XFSF13ME != 0)
                            datiPensioneFondoDatiFS.TrediciMensilita = fondoFS.XFSF13ME == 1 ? true : false;
                        if (fondoFS.XFSPAL != 0M)
                        {
                            datiPensioneFondoDatiFS.PensioneAnnuaLorda = fondoFS.XFSPAL;
                            //ENG - PL Reversibilita 024
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiFS.IsPensioneAnnuaLordaDaPrelievo = true;
                            }
                        }

                        if (fondoFS.XFSSUAN != 0 || fondoFS.XFSSUANMM != 0 || fondoFS.XFSSUANGG != 0)
                        {
                            datiPensioneFondoDatiFS.ServizioUtileDirittoAA = fondoFS.XFSSUAN;
                            datiPensioneFondoDatiFS.ServizioUtileDirittoMM = fondoFS.XFSSUANMM;
                            datiPensioneFondoDatiFS.ServizioUtileDirittoGG = fondoFS.XFSSUANGG;
                        }

                        datiPensioneFondoDatiFS.TitolareAltraPensione = String.IsNullOrEmpty(fondoFS.XFSNATU1) ? (bool?)null : ((fondoFS.XFSNATU1 == "2" || fondoFS.XFSNATU1 == "6") ? true : false);

                        if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                        {
                            int? PrivilegiataSuperinvaliditaIndennita = null;
                            int? AssegnoIntegrativo = null;
                            int? IntegrazioneIndennitaAssistenza = null;
                            int? IndennitaAccompagnamentoAggiuntiva = null;
                            int? CumuloInfermita = null;
                            int? Categoria2aInfermita = null;
                            int? AssegnoCura = null;
                            int? IndennitaSpecialeAnnua = null;
                            DecodeASSACByFondo(fondoFS.XFSASSAC, "FS", out PrivilegiataSuperinvaliditaIndennita, out AssegnoIntegrativo,
                                out IntegrazioneIndennitaAssistenza, out IndennitaAccompagnamentoAggiuntiva, out CumuloInfermita,
                                out Categoria2aInfermita, out AssegnoCura, out IndennitaSpecialeAnnua);
                            datiPensioneFondoDatiFS.PrivilegiataSuperinvaliditaIndennita = PrivilegiataSuperinvaliditaIndennita;
                            datiPensioneFondoDatiFS.AssegnoIntegrativo = AssegnoIntegrativo;
                            datiPensioneFondoDatiFS.IntegrazioneIndennitaAssistenza = IntegrazioneIndennitaAssistenza;
                            datiPensioneFondoDatiFS.IndennitaAccompagnamentoAggiuntiva = IndennitaAccompagnamentoAggiuntiva;
                            datiPensioneFondoDatiFS.CumuloInfermita = CumuloInfermita;
                            datiPensioneFondoDatiFS.Categoria2aInfermita = Categoria2aInfermita;
                            datiPensioneFondoDatiFS.AssegnoCura = AssegnoCura;
                            datiPensioneFondoDatiFS.IndennitaSpecialeAnnua = IndennitaSpecialeAnnua;

                            if (fondoFS.XFSASSAC != 0)
                            {
                                if (datiFondo == null)
                                    datiFondo = new GestioneFondo.DatiFondo();
                                datiFondo.Privilegiate = true;
                            }
                        }

                        if (!string.IsNullOrEmpty(fondoFS.XFSCONG) && fondoFS.XFSCONG != "00")
                        {
                            datiPensioneFondoDatiFS.IndennitaIntegrativaSpecialeConglobata = fondoFS.XFSCONG.PadRight(2, '0').Substring(0, 1) == "1" ? true : false;
                            datiPensioneFondoDatiFS.IntegrazioneMinimo = fondoFS.XFSCONG.PadRight(2, '0').Substring(1, 1) == "1" ? true : false;
                        }

                        if (fondoFS.XFSDIIS >= 0)
                        {
                            switch (fondoFS.XFSDIIS)
                            {
                                case 0:
                                    datiPensioneFondoDatiFS.DirittoIndennitaIntegrativaSpeciale = false;
                                    break;
                                case 1:
                                    datiPensioneFondoDatiFS.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiFS.PagamentoIndennitaIntegrativaSpeciale = true;
                                    break;
                                case 2:
                                    datiPensioneFondoDatiFS.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiFS.PagamentoIndennitaIntegrativaSpeciale = false;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(fondoFS.XFSRID))
                        {
                            switch (fondoFS.XFSRID)
                            {
                                case "0":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = false;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = false;
                                    break;
                                case "1":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = true;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = false;
                                    break;
                                case "2":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = false;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = true;
                                    break;
                                case "3":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = true;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (fondoFS.XFSNO336 != 0M)
                            datiPensioneFondoDatiFS.RMSSenzaLegge33670QA = fondoFS.XFSNO336;

                        if (fondoFS.XFSDECECAA != 0 && fondoFS.XFSDECECMM != 0 && fondoFS.XFSDECECGG != 0)
                        {
                            if (fondoFS.XFSDECECAA == 9999 || fondoFS.XFSDECECMM == 99 || fondoFS.XFSDECECGG == 99)
                                datiPensioneFondoDatiFS.ScadenzaIllimitata = true;
                            else
                                datiPensioneFondoDatiFS.ScadenzaBenefici = Utility.DataFromInt(fondoFS.XFSDECECAA, fondoFS.XFSDECECMM, fondoFS.XFSDECECGG);
                        }

                        if (fondoFS.XFSPAL335 != 0M)
                            datiPensioneFondoDatiFS.PALConBenefici = fondoFS.XFSPAL335;
                        if (fondoFS.XFSSETDIR != 0)
                            datiPensioneFondoDatiFS.VVUtiliDiritto = fondoFS.XFSSETDIR;
                        if (fondoFS.XFSSETMIS != 0)
                            datiPensioneFondoDatiFS.VVUtiliMisura = fondoFS.XFSSETMIS;

                        if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                        {
                            if (datiPensioneFondoDatiFS == null)
                                datiPensioneFondoDatiFS = new GestioneFondo.DatiFondoFST();

                            Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                            if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiFS.RequisitiAnte247 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                                datiPensioneFondoDatiFS.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                                if (anagrafica.TRARECUP != 0)
                                {
                                    datiPensioneFondoDatiFS.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                                    datiPensioneFondoDatiFS.TrimesteRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                                }
                            }

                            string siglaCategoria = GetCategoriaFromTRACATEG(anagrafica.TRACATEG.PadRight(8));
                            string tipoReversibilita = AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRATIPIR;
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && !String.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant().StartsWith("S") &&
                                 !String.IsNullOrEmpty(tipoReversibilita) && tipoReversibilita.Trim().ToUpperInvariant() == "R")
                            {
                                datiPensioneFondoDatiFS.TipologiaPensione = fondoFS.XFSTPENS;
                            }
                        }

                        listaDatiPensioneFondoDatiFS.Add(datiPensioneFondoDatiFS);
                    }
                }

                if (AreaPrelievo.FinalResponse.ListaAgoFS != null && AreaPrelievo.FinalResponse.ListaAgoFS.Count > 0)
                {
                    if (listaDatiPensioneFondoDatiFS == null)
                        listaDatiPensioneFondoDatiFS = new List<GestioneFondo.DatiFondoFST>();

                    foreach (Data.CMSGTRA.Ago.FS agoFs in AreaPrelievo.FinalResponse.ListaAgoFS)
                    {
                        if (!listaDatiPensioneFondoDatiFS.Exists(x => x.IdRecordFondo == agoFs.YFSPROGR))
                            listaDatiPensioneFondoDatiFS.Add(new GestioneFondo.DatiFondoFST { IdRecordFondo = agoFs.YFSPROGR });
                        GestioneFondo.DatiFondoFST datiPensioneFondoDatiFS = listaDatiPensioneFondoDatiFS.FirstOrDefault(x => x.IdRecordFondo == agoFs.YFSPROGR);

                        if (agoFs.YFSPAL707 != 0M)
                        {
                            datiPensioneFondoDatiFS.PensioneAnnuaLorda707 = agoFs.YFSPAL707;
                            //ENG - PL Reversibilita 024
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiFS.IsPensioneAnnuaLorda707DaPrelievo = true;
                            }
                        }
                        if (agoFs.YFSCOEFTRA != 0M)
                            datiPensioneFondoDatiFS.CoefficienteTrasformazione = agoFs.YFSCOEFTRA;
                        if (agoFs.YFSPAL214 != 0M)
                            datiPensioneFondoDatiFS.PensioneAnnuaLorda214 = agoFs.YFSPAL214;
                    }
                }
            }
            else
            {
                if (AreaPrelievo.FinalResponse.ListaFondoFS != null && AreaPrelievo.FinalResponse.ListaFondoFS.Count > 0)
                {
                    listaDatiPensioneFondoDatiFS = new List<GestioneFondo.DatiFondoFST>();
                    foreach (Data.CMSGTRA.Fondo.FS fondoFS in AreaPrelievo.FinalResponse.ListaFondoFS)
                    {
                        GestioneFondo.DatiFondoFST datiPensioneFondoDatiFS = new GestioneFondo.DatiFondoFST();
                        datiPensioneFondoDatiFS.IdRecordFondo = fondoFS.XFSPROGR;

                        if (fondoFS.XFSCAUSA != 0)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            GestioneDecodifica.DecodificaCausaCessazione causaCess = null;
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                                causaCess = ListaCausaCess.Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == fondoFS.XFSCAUSA.ToString().Trim().ToUpperInvariant() && x.Fondo == "FS");
                            datiPensioneFondoDatiFS.CausaCessazione = causaCess != null ? causaCess.Id : 0;
                        }
                        datiPensioneFondoDatiFS.DecorrenzaCalcolo = Utility.DataFromString(fondoFS.XFSDECAL.ToString().PadLeft(8, '0'), Utility.FormatoData.AAAAmmGG);
                        datiPensioneFondoDatiFS.DecorrenzaEconomica = Utility.DataFromInt(fondoFS.XFSDECECAA, fondoFS.XFSDECECMM, fondoFS.XFSDECECGG);
                        if (fondoFS.XFSF13ME != 0)
                            datiPensioneFondoDatiFS.TrediciMensilita = fondoFS.XFSF13ME == 1 ? true : false;
                        if (fondoFS.XFSPAL != 0M)
                            datiPensioneFondoDatiFS.PensioneAnnuaLorda = fondoFS.XFSPAL;

                        if (fondoFS.XFSSUAN != 0)
                            datiPensioneFondoDatiFS.ServizioUtileDirittoAA = fondoFS.XFSSUAN;

                        datiPensioneFondoDatiFS.TitolareAltraPensione = string.IsNullOrEmpty(fondoFS.XFSNATU1) ? (bool?)null : ((fondoFS.XFSNATU1 == "2" || fondoFS.XFSNATU1 == "6") ? true : false);

                        if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                        {
                            int? PrivilegiataSuperinvaliditaIndennita = null;
                            int? AssegnoIntegrativo = null;
                            int? IntegrazioneIndennitaAssistenza = null;
                            int? IndennitaAccompagnamentoAggiuntiva = null;
                            int? CumuloInfermita = null;
                            int? Categoria2aInfermita = null;
                            int? AssegnoCura = null;
                            int? IndennitaSpecialeAnnua = null;
                            DecodeASSACByFondo(fondoFS.XFSASSAC, "FS", out PrivilegiataSuperinvaliditaIndennita, out AssegnoIntegrativo,
                                out IntegrazioneIndennitaAssistenza, out IndennitaAccompagnamentoAggiuntiva, out CumuloInfermita,
                                out Categoria2aInfermita, out AssegnoCura, out IndennitaSpecialeAnnua);
                            datiPensioneFondoDatiFS.PrivilegiataSuperinvaliditaIndennita = PrivilegiataSuperinvaliditaIndennita;
                            datiPensioneFondoDatiFS.AssegnoIntegrativo = AssegnoIntegrativo;
                            datiPensioneFondoDatiFS.IntegrazioneIndennitaAssistenza = IntegrazioneIndennitaAssistenza;
                            datiPensioneFondoDatiFS.IndennitaAccompagnamentoAggiuntiva = IndennitaAccompagnamentoAggiuntiva;
                            datiPensioneFondoDatiFS.CumuloInfermita = CumuloInfermita;
                            datiPensioneFondoDatiFS.Categoria2aInfermita = Categoria2aInfermita;
                            datiPensioneFondoDatiFS.AssegnoCura = AssegnoCura;
                            datiPensioneFondoDatiFS.IndennitaSpecialeAnnua = IndennitaSpecialeAnnua;

                            if (fondoFS.XFSASSAC != 0)
                            {
                                if (datiFondo == null)
                                    datiFondo = new GestioneFondo.DatiFondo();
                                datiFondo.Privilegiate = true;
                            }
                        }

                        if (!string.IsNullOrEmpty(fondoFS.XFSCONG) && fondoFS.XFSCONG != "00")
                        {
                            datiPensioneFondoDatiFS.IndennitaIntegrativaSpecialeConglobata = fondoFS.XFSCONG.PadRight(2, '0').Substring(0, 1) == "1" ? true : false;
                            datiPensioneFondoDatiFS.IntegrazioneMinimo = fondoFS.XFSCONG.PadRight(2, '0').Substring(1, 1) == "1" ? true : false;
                        }

                        if (fondoFS.XFSDIIS >= 0)
                        {
                            switch (fondoFS.XFSDIIS)
                            {
                                case 0:
                                    datiPensioneFondoDatiFS.DirittoIndennitaIntegrativaSpeciale = false;
                                    break;
                                case 1:
                                    datiPensioneFondoDatiFS.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiFS.PagamentoIndennitaIntegrativaSpeciale = true;
                                    break;
                                case 2:
                                    datiPensioneFondoDatiFS.DirittoIndennitaIntegrativaSpeciale = true;
                                    datiPensioneFondoDatiFS.PagamentoIndennitaIntegrativaSpeciale = false;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(fondoFS.XFSRID))
                        {
                            switch (fondoFS.XFSRID)
                            {
                                case "0":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = false;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = false;
                                    break;
                                case "1":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = true;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = false;
                                    break;
                                case "2":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = false;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = true;
                                    break;
                                case "3":
                                    datiPensioneFondoDatiFS.RiduzioneL537 = true;
                                    datiPensioneFondoDatiFS.IISAbbattimentoAnni = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (fondoFS.XFSNO336 != 0M)
                            datiPensioneFondoDatiFS.RMSSenzaLegge33670QA = fondoFS.XFSNO336;

                        if (fondoFS.XFSDECECAA != 0 && fondoFS.XFSDECECMM != 0 && fondoFS.XFSDECECGG != 0)
                        {
                            if (fondoFS.XFSDECECAA == 9999 || fondoFS.XFSDECECMM == 99 || fondoFS.XFSDECECGG == 99)
                                datiPensioneFondoDatiFS.ScadenzaIllimitata = true;
                            else
                                datiPensioneFondoDatiFS.ScadenzaBenefici = Utility.DataFromInt(fondoFS.XFSDECECAA, fondoFS.XFSDECECMM, fondoFS.XFSDECECGG);
                        }

                        if (fondoFS.XFSPAL335 != 0M)
                            datiPensioneFondoDatiFS.PALConBenefici = fondoFS.XFSPAL335;

                        if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                        {
                            if (datiPensioneFondoDatiFS == null)
                                datiPensioneFondoDatiFS = new GestioneFondo.DatiFondoFST();

                            Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                            if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                            {
                                datiPensioneFondoDatiFS.RequisitiAnte247 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                                datiPensioneFondoDatiFS.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                                if (anagrafica.TRARECUP != 0)
                                {
                                    datiPensioneFondoDatiFS.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                                    datiPensioneFondoDatiFS.TrimesteRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                                }
                            }
                        }

                        listaDatiPensioneFondoDatiFS.Add(datiPensioneFondoDatiFS);
                    }
                }
            }
            #endregion ListaFondoFS
        }

        public static void ValorizzaPensioneFondoDatiPI(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoPI datiPensioneFondoDatiPI)
        {
            datiPensioneFondoDatiPI = null;
            #region ListaFondoPI
            if (AreaPrelievo.FinalResponse.ListaFondoPI != null && AreaPrelievo.FinalResponse.ListaFondoPI.Count > 0)
            {
                datiPensioneFondoDatiPI = new GestioneFondo.DatiFondoPI();
                Data.CMSGTRA.Fondo.PI fondoPI = AreaPrelievo.FinalResponse.ListaFondoPI[0];
                string siglaCategoria = AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0 ?
                    GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG) : string.Empty;

                Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, siglaCategoria);

                try
                {
                    if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
                    {
                        // 3 byte Livello
                        // 4 byte SettimaneMaggiorazione
                        // 3 byte SettimaneEsclusive
                        if (fondoPI.XPIONLEG != 0M)
                        {
                            string xpionleg = string.Format("{0:000000.0000}", fondoPI.XPIONLEG);
                            datiPensioneFondoDatiPI.Livello = byte.Parse(xpionleg.Substring(0, 3));
                            datiPensioneFondoDatiPI.SettimaneMaggiorazione = (short?)((short.Parse(xpionleg.Substring(3, 3)) * 10) + short.Parse(xpionleg.Substring(7, 1)));
                            datiPensioneFondoDatiPI.SettimaneEsclusive = (short?)(short.Parse(xpionleg.Substring(8, 3)) * 10);
                        }

                        // 1 byte SettimaneEsclusive
                        // 4 byte SettimaneINPDAI
                        // 5 byte 0
                        if (fondoPI.XPIDP346 != 0M)
                        {
                            string xpidp346 = string.Format("{0:000000.0000}", fondoPI.XPIDP346);
                            datiPensioneFondoDatiPI.SettimaneEsclusive = datiPensioneFondoDatiPI.SettimaneEsclusive.HasValue ? (short?)(datiPensioneFondoDatiPI.SettimaneEsclusive + short.Parse(xpidp346.Substring(0, 1))) : short.Parse(xpidp346.Substring(0, 1));
                            datiPensioneFondoDatiPI.SettimaneINPDAI = short.Parse(xpidp346.Substring(1, 4));
                        }
                    }
                    else
                        datiPensioneFondoDatiPI.NumeroMatricola = ((int)(fondoPI.XPIONLEG * 100)).ToString();
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }

                datiPensioneFondoDatiPI.Qualifica = fondoPI.XPIQUALI;
                if (categoriaFondoPI.HasValue && categoriaFondoPI.Value != Utility.CategoriaFondoPI.V)
                    datiPensioneFondoDatiPI.DecorrenzaPensioneEliminata = Utility.DataFromInt(fondoPI.XPIRDISD, fondoPI.XPIRDPNI, fondoPI.XPIRDISI);
                datiPensioneFondoDatiPI.RiscattiAA = fondoPI.XPIRISCA != 0 ? fondoPI.XPIRISCA : (short?)null;
                datiPensioneFondoDatiPI.RiscattiMM = fondoPI.XPIRISCM != 0 ? fondoPI.XPIRISCM : (short?)null;
                datiPensioneFondoDatiPI.RiscattiGG = fondoPI.XPIRISCG != 0 ? fondoPI.XPIRISCG : (short?)null;
                datiPensioneFondoDatiPI.StipendioAnnuo = fondoPI.XPISTIPE;
                datiPensioneFondoDatiPI.PensioneFacoltativaMensile = fondoPI.XPIFACOL;
                if (categoriaFondoPI.HasValue && categoriaFondoPI.Value != Utility.CategoriaFondoPI.U)
                    datiPensioneFondoDatiPI.DecorrenzaPrescrizione = Utility.DataFromInt(fondoPI.XPICAPDE, fondoPI.XPICAPIN, 1);
                else
                    datiPensioneFondoDatiPI.PercentualeCapitalizzazione = Utility.StringToNullableDecimal(fondoPI.XPICAPIN + "," + fondoPI.XPICAPDE);
                datiPensioneFondoDatiPI.ImportoIIS = fondoPI.XPIINTEG;
                if (fondoPI.XPI36BIS != 0M)
                    datiPensioneFondoDatiPI.StipendioBase = fondoPI.XPI36BIS;
                if (!string.IsNullOrEmpty(fondoPI.XPIOKIIS))
                    datiPensioneFondoDatiPI.AttCon = Utility.StringToNullableChar(fondoPI.XPIOKIIS);
                if (!string.IsNullOrEmpty(fondoPI.XPIMEDIC))
                    datiPensioneFondoDatiPI.CodiceMaggiorazione = Utility.StringToNullableChar(fondoPI.XPIMEDIC);
                if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
                {
                    if (fondoPI.XPIAS762 != 0M)
                        datiPensioneFondoDatiPI.PensComplRiv1_95 = fondoPI.XPIAS762;
                }
                else if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.V)
                {
                    if (fondoPI.XPIAS762 != 0M)
                        datiPensioneFondoDatiPI.RMSQuotaB = fondoPI.XPIAS762;

                    if (fondoPI.XPIRDISD != 0)
                        datiPensioneFondoDatiPI.NSettimaneQuotaA = fondoPI.XPIRDISD;
                }
                if (fondoPI.XPIINAIL != 0M)
                    datiPensioneFondoDatiPI.RMSQuotaA = fondoPI.XPIINAIL;
                if (fondoPI.XPIRDPND != 0)
                    datiPensioneFondoDatiPI.NSettimaneQuotaB = fondoPI.XPIRDPND;
                if (!(categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U))
                    datiPensioneFondoDatiPI.NonVedente = fondoPI.XPINONVE == 1 ? true : false;

                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    if (datiPensioneFondoDatiPI == null)
                        datiPensioneFondoDatiPI = new GestioneFondo.DatiFondoPI();

                    Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    {
                        datiPensioneFondoDatiPI.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiPI.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiPI.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiPI.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }
                }
            }
            #endregion ListaFondoPI
        }

        public static void ValorizzaPensioneFondoDatiGAS(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoGAS datiPensioneFondoDatiGAS)
        {
            datiPensioneFondoDatiGAS = null;
            #region ListaFondoGAS
            if (AreaPrelievo.FinalResponse.ListaFondoGAS != null && AreaPrelievo.FinalResponse.ListaFondoGAS.Count > 0)
            {
                datiPensioneFondoDatiGAS = new GestioneFondo.DatiFondoGAS();
                Data.CMSGTRA.Fondo.GAS fondoGAS = AreaPrelievo.FinalResponse.ListaFondoGAS[0];

                datiPensioneFondoDatiGAS.MesiUtiliIndennitaAggiuntiva = fondoGAS.XGARISCU;
                datiPensioneFondoDatiGAS.MesiNonUtiliIndennitaAggiuntiva = fondoGAS.XGARISCN;
                datiPensioneFondoDatiGAS.ServizioUtileIndennitaAggiuntiva = fondoGAS.XGAINDMM;
                datiPensioneFondoDatiGAS.Retribuzione = fondoGAS.XGAINDRT;
                datiPensioneFondoDatiGAS.CodicePensioneRidotta = Utility.StringToNullableBool(fondoGAS.XGAPNRID.ToString());
                datiPensioneFondoDatiGAS.Conguaglio = fondoGAS.XGACONGU;
                datiPensioneFondoDatiGAS.MesiAnte46 = fondoGAS.XGAANT46;
                datiPensioneFondoDatiGAS.AnzianitaUtileDal46 = fondoGAS.XGAPOS46;
                datiPensioneFondoDatiGAS.CodiceDimissioni = Utility.StringToNullableBool(fondoGAS.XGADIMIS.ToString());
                datiPensioneFondoDatiGAS.PercentualeRiduzione = fondoGAS.XGARIDUZ;
                datiPensioneFondoDatiGAS.Convenzione = fondoGAS.XGACONVE;
                datiPensioneFondoDatiGAS.Ditta = fondoGAS.XGADITTA;

                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                {
                    Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    {
                        datiPensioneFondoDatiGAS.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiGAS.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiGAS.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiGAS.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }
                }

                if (AreaPrelievo.FinalResponse.ListaAgoGAS != null && AreaPrelievo.FinalResponse.ListaAgoGAS.Count > 0)
                {
                    Data.CMSGTRA.Ago.GAS agoGAS = AreaPrelievo.FinalResponse.ListaAgoGAS[0];
                    datiPensioneFondoDatiGAS.DecorrenzaTeorica = Utility.DataFromInt(agoGAS.YGATEOAA, agoGAS.YGATEOMM, 1).HasValue ? Utility.DataFromInt(agoGAS.YGATEOAA, agoGAS.YGATEOMM, 1).Value : DateTime.MinValue;
                    if (agoGAS.YGASOSAA > 0 && agoGAS.YGASOSMM > 0)
                        datiPensioneFondoDatiGAS.SospensioneAGO = new DateTime(agoGAS.YGASOSAA, agoGAS.YGASOSMM, 01);
                    datiPensioneFondoDatiGAS.SettimaneAnzianitaEsclusiva = (short)agoGAS.YGAANZVV;
                    datiPensioneFondoDatiGAS.AnniDifferimento = agoGAS.YGADIFFE;
                    datiPensioneFondoDatiGAS.EtaMaturazioneRequisiti = (byte)agoGAS.YGAMATUR;
                    if (!string.IsNullOrEmpty(agoGAS.YGASPECI))
                        datiPensioneFondoDatiGAS.CodiceSpecificoAgo = char.Parse(agoGAS.YGASPECI);
                    datiPensioneFondoDatiGAS.ContributiTotaliSupplementoDPR143271 = agoGAS.YGACNTOT;
                    datiPensioneFondoDatiGAS.ContribuzioneEsclusivaDPR143271 = agoGAS.YGACNESC;
                    datiPensioneFondoDatiGAS.CCTotaliArt14 = agoGAS.YGACNT14;
                    datiPensioneFondoDatiGAS.ContribuzioneEsclusiva = agoGAS.YGACNE14;
                    if (agoGAS.YGACDCAA > 0 && agoGAS.YGACDCMM > 0)
                        datiPensioneFondoDatiGAS.DecDPCM = new DateTime(agoGAS.YGACDCAA, agoGAS.YGACDCMM, 01);
                    datiPensioneFondoDatiGAS.RMSArt14 = agoGAS.YGADPCRT;
                    datiPensioneFondoDatiGAS.RMSSent72 = agoGAS.YGAS72RT;
                    datiPensioneFondoDatiGAS.CCTotaliArt11 = agoGAS.YGACNT11;
                    datiPensioneFondoDatiGAS.CCEsclusivaArt11 = agoGAS.YGACNE11;
                    datiPensioneFondoDatiGAS.CodiceTipoLiquidazione = (byte?)agoGAS.YGATPLIQ;
                    if (agoGAS.YGADECAA > 0 && agoGAS.YGADECMM > 0)
                        datiPensioneFondoDatiGAS.DecorrenzaDatiAgo = new DateTime(agoGAS.YGADECAA, agoGAS.YGADECMM, 01);
                }
            }
            #endregion ListaFondoGAS
        }

        public static void ValorizzaPensioneFondoDatiES(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoES datiPensioneFondoDatiES)
        {
            datiPensioneFondoDatiES = null;
            #region ListaFondoES
            if (AreaPrelievo.FinalResponse.ListaFondoES != null && AreaPrelievo.FinalResponse.ListaFondoES.Count > 0)
            {
                datiPensioneFondoDatiES = new GestioneFondo.DatiFondoES();
                Data.CMSGTRA.Fondo.ES fondoES = AreaPrelievo.FinalResponse.ListaFondoES[0];

                if (fondoES.LISTXESCALCO != null && fondoES.LISTXESCALCO.Count > 0)
                {
                    datiPensioneFondoDatiES.Retribuzione = fondoES.LISTXESCALCO[0].XESCALRT;
                    datiPensioneFondoDatiES.MMServizioUtile = fondoES.LISTXESCALCO[0].XESCALMM;
                }
                if (fondoES.LISTXESCALCO != null && fondoES.LISTXESCALCO.Count > 1)
                {
                    datiPensioneFondoDatiES.Retribuzione2 = fondoES.LISTXESCALCO[1].XESCALRT;
                    datiPensioneFondoDatiES.MMServizioUtile2 = fondoES.LISTXESCALCO[1].XESCALMM;
                }
                if (fondoES.LISTXESCALCO != null && fondoES.LISTXESCALCO.Count > 2)
                {
                    datiPensioneFondoDatiES.Retribuzione3 = fondoES.LISTXESCALCO[2].XESCALRT;
                    datiPensioneFondoDatiES.MMServizioUtile3 = fondoES.LISTXESCALCO[2].XESCALMM;
                }
                if (fondoES.LISTXESCALCO != null && fondoES.LISTXESCALCO.Count > 3)
                {
                    datiPensioneFondoDatiES.Retribuzione4 = fondoES.LISTXESCALCO[3].XESCALRT;
                    datiPensioneFondoDatiES.MMServizioUtile4 = fondoES.LISTXESCALCO[3].XESCALMM;
                }
                datiPensioneFondoDatiES.AnnoUtile = (fondoES.XESANNUT == 1);
                datiPensioneFondoDatiES.Articolo58 = fondoES.XESART58 != 0 ? Convert.ToByte(fondoES.XESART58) : (byte?)null;
                //ATTENZIONE : verificare cosa mettere quando daHost arriva 0 (null||false)
                datiPensioneFondoDatiES.Articolo59 = (fondoES.XESART59 == 1);
                //ATTENZIONE : verificare cosa mettere quando daHost arriva 0 (null||false)
                datiPensioneFondoDatiES.CodiciRetributivi = Convert.ToByte(fondoES.XESCDRET);
                //ATTENZIONE : verificare cosa mettere quando daHost arriva 0 (null||0)
                datiPensioneFondoDatiES.ClassePensioneAnte50 = Convert.ToByte(fondoES.XESCLASS);
                //ATTENZIONE : verificare cosa mettere quando daHost arriva 0 (null||0)
                datiPensioneFondoDatiES.CodiceEsattoria = fondoES.XESCODES.ToString();
                datiPensioneFondoDatiES.CodiceDz = (fondoES.XESCODIC == 1);
                datiPensioneFondoDatiES.Optanti = (fondoES.XESOPTAN == 1);
                datiPensioneFondoDatiES.MaggiorazionePrivilegiata = (fondoES.XESPRIVI == 1);
                datiPensioneFondoDatiES.Promiscui = (fondoES.XESPROMI != 0) ? Convert.ToByte(fondoES.XESPROMI) : (byte?)0;
                datiPensioneFondoDatiES.Saltuari = (fondoES.XESSALTU == 1);
                if (!string.IsNullOrEmpty(fondoES.XESCONVE))
                    datiPensioneFondoDatiES.ConvenzioneInternazionale = char.Parse(fondoES.XESCONVE);
                datiPensioneFondoDatiES.AnniRiscatti = fondoES.XESRISAA != 0 ? fondoES.XESRISAA : (short?)null;
                datiPensioneFondoDatiES.MesiRiscatti = fondoES.XESRISMM != 0 ? fondoES.XESRISMM : (short?)null;
                //ANTE 67
                fondoES.LISTXES57ELE = new List<Data.CMSGTRA.Fondo.ES.XES57ELE>();
                if (fondoES.LISTXES57ELE != null && fondoES.LISTXES57ELE.Count > 0)
                {
                    datiPensioneFondoDatiES.ContributiLegge37758Art57Periodo1 = fondoES.LISTXES57ELE[0].XES57CTR;
                    datiPensioneFondoDatiES.DecorrenzaLegge37758Art57Pre67Periodo1 = new DateTime(fondoES.LISTXES57ELE[0].XES57DAA, fondoES.LISTXES57ELE[0].XES57DMM, 1);
                }
                if (fondoES.LISTXES57ELE != null && fondoES.LISTXES57ELE.Count > 1)
                {
                    datiPensioneFondoDatiES.ContributiLegge37758Art57Periodo2 = fondoES.LISTXES57ELE[1].XES57CTR;
                    datiPensioneFondoDatiES.DecorrenzaLegge37758Art57Pre67Periodo2 = new DateTime(fondoES.LISTXES57ELE[1].XES57DAA, fondoES.LISTXES57ELE[1].XES57DMM, 1);
                }
                if (fondoES.LISTXES57ELE != null && fondoES.LISTXES57ELE.Count > 2)
                {
                    datiPensioneFondoDatiES.ContributiLegge37758Art57Periodo3 = fondoES.LISTXES57ELE[2].XES57CTR;
                    datiPensioneFondoDatiES.DecorrenzaLegge37758Art57Pre67Periodo3 = new DateTime(fondoES.LISTXES57ELE[2].XES57DAA, fondoES.LISTXES57ELE[2].XES57DMM, 1);
                }
                datiPensioneFondoDatiES.ContributiLegge37758Art24 = fondoES.XES24CTR != 0 ? fondoES.XES24CTR : (decimal?)null;
                if (fondoES.XES24DAA != 0 && fondoES.XES24DMM != 0)
                    datiPensioneFondoDatiES.DecorrenzaArticolo24 = new DateTime(fondoES.XES24DAA, fondoES.XES24DMM, 1);
                datiPensioneFondoDatiES.CodicePensioneInPagamentoPre67 = !string.IsNullOrEmpty(fondoES.XESCODPN) ? char.Parse(fondoES.XESCODPN) : (char?)null;
                datiPensioneFondoDatiES.ImportoInPagamentoPre67 = fondoES.XESIMPAG != 0 ? fondoES.XESIMPAG : (decimal?)null;
                datiPensioneFondoDatiES.PensioneFondoAl67 = fondoES.XESPNFON != 0 ? fondoES.XESPNFON : (decimal?)null;
            }
            #endregion ListaFondoES

            #region ListaAgoES
            if (AreaPrelievo.FinalResponse.ListaAgoES != null && AreaPrelievo.FinalResponse.ListaAgoES.Count > 0)
            {
                Data.CMSGTRA.Ago.ES agoES = AreaPrelievo.FinalResponse.ListaAgoES[0];
                if (datiPensioneFondoDatiES == null)
                    datiPensioneFondoDatiES = new GestioneFondo.DatiFondoES();

                short resShort = 0;
                if (agoES.YESART11 != 0M)
                    datiPensioneFondoDatiES.IntegrazioneArticolo11 = agoES.YESART11;
                if (agoES.YESBALTR != 0M)
                    datiPensioneFondoDatiES.BaseAltraPensione = agoES.YESBALTR;
                if (!string.IsNullOrEmpty(agoES.YESCALTR))
                {
                    short.TryParse(agoES.YESCALTR, out resShort);
                    datiPensioneFondoDatiES.CategoriaAltraPensione = resShort;
                }
                if (agoES.YESCTR24 != 0M)
                    datiPensioneFondoDatiES.ImportoContributiLegge37758Art24 = agoES.YESCTR24;
                if (agoES.YESCTR57 != 0M)
                    datiPensioneFondoDatiES.ImportoContributiLegge37758Art57 = agoES.YESCTR57;
                if (agoES.YESDECAA != 0 && agoES.YESDECMM != 0)
                    datiPensioneFondoDatiES.Decorrenza = Utility.DataFromInt(agoES.YESDECAA, agoES.YESDECMM, 1);
                if (agoES.YESDIFFA != 0)
                    datiPensioneFondoDatiES.AnniDifferimento = agoES.YESDIFFA;
                if (agoES.YESMATUR != 0)
                    datiPensioneFondoDatiES.EtaMaturazioneRequisiti = (byte)agoES.YESMATUR;
                if (agoES.YESSA224 != 0)
                    datiPensioneFondoDatiES.SettimaneArt24QB = agoES.YESSA224;
                if (agoES.YESSAR24 != 0)
                    datiPensioneFondoDatiES.SettimaneArt24QA = agoES.YESSAR24;
                if (agoES.YESSAR57 != 0)
                    datiPensioneFondoDatiES.NSettimaneLegge37758Art57 = agoES.YESSAR57;
                if (agoES.YESSOSAA != 0 && agoES.YESSOSMM != 0)
                    datiPensioneFondoDatiES.Sospensione = Utility.DataFromInt(agoES.YESSOSAA, agoES.YESSOSMM, 1);
                if (!string.IsNullOrEmpty(agoES.YESSPECI))
                    datiPensioneFondoDatiES.CodiceSpecificoAgo = Utility.StringToNullableChar(agoES.YESSPECI);
                if (agoES.YESSUP14 != 0)
                    datiPensioneFondoDatiES.ImportoContributiLegge143271Art14 = agoES.YESSUP14;
                if (agoES.YESTEOAA != 0 && agoES.YESTEOMM != 0)
                    datiPensioneFondoDatiES.DecorrenzaTeorica = Utility.DataFromInt(agoES.YESTEOAA, agoES.YESTEOMM, 1);
                if (agoES.YESTPLIQ != 0)
                    datiPensioneFondoDatiES.CodiceTipoLiquidazione = (byte)agoES.YESTPLIQ;
                if (agoES.YESDIFFQ != 0)
                    datiPensioneFondoDatiES.ContributiDifferimentoQuota = agoES.YESDIFFQ;
                //Art 11 e 14
                if (agoES.YESCDCAA != 0 && agoES.YESCDCMM != 0)
                {
                    datiPensioneFondoDatiES.DecDPCM = Utility.DataFromInt(agoES.YESCDCAA, agoES.YESCDCMM, 1);
                }
                if (agoES.YESDPCRT != 0)
                    datiPensioneFondoDatiES.RmsDPCM = agoES.YESDPCRT;
                if (agoES.YESS72RT != 0)
                    datiPensioneFondoDatiES.RMSSent72 = agoES.YESS72RT;
                //Dati S.L 336
                if (agoES.YESZA14C != 0)
                    datiPensioneFondoDatiES.CCArt14SenzaLegge33670 = agoES.YESZA14C;
                if (agoES.YESZANZI != 0)
                    datiPensioneFondoDatiES.NSettimaneAnzianitaTotaliSenzaLegge33670 = agoES.YESZANZI;
                if (agoES.YESZRET2 != 0)
                    datiPensioneFondoDatiES.RMSSenzaLegge33670QB = agoES.YESZRET2;
                if (agoES.YESZRETS != 0)
                    datiPensioneFondoDatiES.RMSSenzaLegge33670QA = agoES.YESZRETS;
                if (agoES.YESZSPAG != 0)
                    datiPensioneFondoDatiES.ContributiSupplementoAgo = agoES.YESZSPAG;
                if (agoES.YESZSPFO != 0)
                    datiPensioneFondoDatiES.ContributiSupplementoFondo = agoES.YESZSPFO;
                if (agoES.YESZST24 != 0)
                    datiPensioneFondoDatiES.NSettimaneSenzaLegge33670Art24QuotaA = agoES.YESZST24;
                if (agoES.YESZST57 != 0)
                    datiPensioneFondoDatiES.NSettimaneSenzaLegge33670Art57QuotaA = agoES.YESZST57;
                if (agoES.YESZTOTC != 0)
                    datiPensioneFondoDatiES.ContributiTotaliSenzaLegge33670 = agoES.YESZTOTC;
            }
            #endregion ListaAgoES

            #region ListaAnagrafica
            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                {
                    if (!string.IsNullOrEmpty(anagrafica.TRACODSI) || !string.IsNullOrEmpty(anagrafica.TRAANZ247) || anagrafica.TRARECUP != 0)
                    {
                        if (datiPensioneFondoDatiES == null)
                            datiPensioneFondoDatiES = new GestioneFondo.DatiFondoES();
                        datiPensioneFondoDatiES.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiES.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiES.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiES.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }
                }
            }
            #endregion ListaAnagrafica
        }

        public static void ValorizzaPensioneFondoDatiCL(Data.FSPR AreaPrelievo, out GestioneFondo.DatiFondoCL datiPensioneFondoDatiCL)
        {
            datiPensioneFondoDatiCL = null;
            #region ListaFondoCL
            if (AreaPrelievo.FinalResponse.ListaFondoCL != null && AreaPrelievo.FinalResponse.ListaFondoCL.Count > 0)
            {
                datiPensioneFondoDatiCL = new GestioneFondo.DatiFondoCL();
                Data.CMSGTRA.Fondo.CL fondoCL = AreaPrelievo.FinalResponse.ListaFondoCL[0];

                datiPensioneFondoDatiCL.ImportoAltraPensione = fondoCL.XCLVITAL;
                if (fondoCL.XCLNOREQ == 1)
                    datiPensioneFondoDatiCL.CodicePensioneSenzaRequisiti = true;
                else
                    if (fondoCL.XCLNOREQ == 0)
                    datiPensioneFondoDatiCL.CodicePensioneSenzaRequisiti = false;
                datiPensioneFondoDatiCL.AnniDifferimento = fondoCL.XCLDIFFE;
                datiPensioneFondoDatiCL.EtaPerfezionamentoRequisiti = (byte)fondoCL.XCLPERFE;
                if (fondoCL.XCLMMREQ != 0 && fondoCL.XCLAAREQ != 0)
                    datiPensioneFondoDatiCL.DataPerfezionamentoRequisiti = new DateTime(fondoCL.XCLAAREQ, fondoCL.XCLMMREQ, 1);
                datiPensioneFondoDatiCL.ContrProvv = fondoCL.XCLCONTR_PROV[0];
            }
            #endregion ListaFondoCL
        }

        public static void ValorizzaPensioneFondoDatiDZ(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out List<GestioneFondo.DatiFondoDZ> listaDatiPensioneFondoDatiDZ)
        {
            listaDatiPensioneFondoDatiDZ = null;
            #region ListaFondoDZ
            if (AreaPrelievo.FinalResponse.ListaFondoDZ != null && AreaPrelievo.FinalResponse.ListaFondoDZ.Count > 0)
            {
                //Nuova Gestione Dazi Daniele
                listaDatiPensioneFondoDatiDZ = new List<GestioneFondo.DatiFondoDZ>();
                foreach (Data.CMSGTRA.Fondo.DZ fondoDZ in AreaPrelievo.FinalResponse.ListaFondoDZ)
                {
                    GestioneFondo.DatiFondoDZ datiPensioneFondoDatiDZ = new GestioneFondo.DatiFondoDZ();
                    datiPensioneFondoDatiDZ.IdRecordFondo = fondoDZ.XDZPROGR;
                    datiPensioneFondoDatiDZ.ClasseAnte50 = fondoDZ.XDZ50CLA;
                    datiPensioneFondoDatiDZ.PensioneBaseAnnua = fondoDZ.XDZANBAS;
                    datiPensioneFondoDatiDZ.MaggiorazioneAnzianitaEsodoAA = fondoDZ.XDZANZAA;
                    datiPensioneFondoDatiDZ.MaggiorazioneAnzianitaEsodoMM = fondoDZ.XDZANZMM;
                    if (fondoDZ.XDZCESAA != 0 && fondoDZ.XDZCESMM != 0 && fondoDZ.XDZCESGG != 0)
                        datiPensioneFondoDatiDZ.DataCessazioneServizio = new DateTime(fondoDZ.XDZCESAA, fondoDZ.XDZCESMM, fondoDZ.XDZCESGG);
                    datiPensioneFondoDatiDZ.CodiceDZ = fondoDZ.XDZCODDZ == 1 ? true : false;
                    datiPensioneFondoDatiDZ.CodiceEsodo = fondoDZ.XDZCODES == 1 ? true : false;
                    datiPensioneFondoDatiDZ.CodiceBenefici = fondoDZ.XDZCODIG;
                    datiPensioneFondoDatiDZ.Ditta = fondoDZ.XDZCODIT.ToString();
                    datiPensioneFondoDatiDZ.CodiceCaroPane = fondoDZ.XDZCPANE == 1 ? true : false;
                    datiPensioneFondoDatiDZ.PercentualeLiquidazionePensione = fondoDZ.XDZPERCE;
                    datiPensioneFondoDatiDZ.MaggiorazionePensionePrivilegiataAA = fondoDZ.XDZPRIVA;
                    datiPensioneFondoDatiDZ.MaggiorazionePensionePrivilegiataMM = fondoDZ.XDZPRIVM;
                    datiPensioneFondoDatiDZ.RetribuzioneAlNettoBeneficiEsodo = fondoDZ.XDZRETNO;
                    datiPensioneFondoDatiDZ.RiscattiAA = fondoDZ.XDZRISAA != 0 ? fondoDZ.XDZRISAA : (short?)null;
                    datiPensioneFondoDatiDZ.RiscattiMM = fondoDZ.XDZRISMM != 0 ? fondoDZ.XDZRISMM : (short?)null;
                    if (fondoDZ.XDZDECAA > 0 && fondoDZ.XDZDECMM > 0)
                        datiPensioneFondoDatiDZ.DecorrenzaSecondaria = new DateTime(fondoDZ.XDZDECAA, fondoDZ.XDZDECMM, 01);

                    #region ListaAnagrafica
                    if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
                    {
                        Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                        if (datiPensioneFondoDatiDZ == null)
                            datiPensioneFondoDatiDZ = new GestioneFondo.DatiFondoDZ();

                        if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                        {
                            if (!string.IsNullOrEmpty(anagrafica.TRACODSI) || !string.IsNullOrEmpty(anagrafica.TRAANZ247) || anagrafica.TRARECUP != 0 || !string.IsNullOrEmpty(anagrafica.TRA562))
                            {
                                if (datiPensioneFondoDatiDZ == null)
                                    datiPensioneFondoDatiDZ = new GestioneFondo.DatiFondoDZ();

                                datiPensioneFondoDatiDZ.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                                datiPensioneFondoDatiDZ.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                                if (anagrafica.TRARECUP != 0)
                                {
                                    datiPensioneFondoDatiDZ.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                                    datiPensioneFondoDatiDZ.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                                }

                                if (anagrafica.TRA562 == "SI")
                                    datiPensioneFondoDatiDZ.RaggiuntoRequisiti311297 = true;
                                else if (anagrafica.TRA562 == "NO")
                                    datiPensioneFondoDatiDZ.RaggiuntoRequisiti311297 = false;
                            }
                        }
                    }
                    #endregion ListaAnagrafica


                    listaDatiPensioneFondoDatiDZ.Add(datiPensioneFondoDatiDZ);
                }
            }
            #endregion ListaFondoDZ

            #region ListaAgoDZ
            if (AreaPrelievo.FinalResponse.ListaAgoDZ != null && AreaPrelievo.FinalResponse.ListaAgoDZ.Count > 0)
            {
                if (listaDatiPensioneFondoDatiDZ == null)
                    listaDatiPensioneFondoDatiDZ = new List<GestioneFondo.DatiFondoDZ>();

                foreach (Data.CMSGTRA.Ago.DZ agoDZ in AreaPrelievo.FinalResponse.ListaAgoDZ)
                {
                    if (!listaDatiPensioneFondoDatiDZ.Exists(x => x.IdRecordFondo == agoDZ.YDZPROGR))
                        listaDatiPensioneFondoDatiDZ.Add(new GestioneFondo.DatiFondoDZ { IdRecordFondo = agoDZ.YDZPROGR });
                    GestioneFondo.DatiFondoDZ datiPensioneFondoDatiDZ = listaDatiPensioneFondoDatiDZ.FirstOrDefault(x => x.IdRecordFondo == agoDZ.YDZPROGR);
                    if (agoDZ.YDZSCAAA != 0 && agoDZ.YDZSCASS != 0 && agoDZ.YDZSCAMM != 0)
                        datiPensioneFondoDatiDZ.Sospensione = new DateTime(agoDZ.YDZSCASS * 100 + agoDZ.YDZSCAAA, agoDZ.YDZSCAMM, 1);
                }
            }
            #endregion ListaAgoDZ


        }

        public static void ValorizzaPensioneFondoDatiPM(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out GestioneFondo.DatiFondoPM datiPensioneFondoDatiPM)
        {
            datiPensioneFondoDatiPM = null;
            #region ListaFondoPM
            if (AreaPrelievo.FinalResponse.ListaFondoPM != null && AreaPrelievo.FinalResponse.ListaFondoPM.Count > 0)
            {
                datiPensioneFondoDatiPM = new GestioneFondo.DatiFondoPM();
                Data.CMSGTRA.Fondo.PM fondoPM = AreaPrelievo.FinalResponse.ListaFondoPM[0];

                datiPensioneFondoDatiPM.AnnoUtileUltimoDecennio = fondoPM.XPMANULT == 1 ? true : false;
                if (!string.IsNullOrEmpty(fondoPM.XPMATTIV) && fondoPM.XPMATTIV.Length == 2)
                    datiPensioneFondoDatiPM.AttivitaSvolta2 = char.Parse(fondoPM.XPMATTIV.Substring(1, 1));
                if (fondoPM.XPMTILIQ != 0)
                    datiPensioneFondoDatiPM.TipoLiquidazione = byte.Parse(fondoPM.XPMTILIQ.ToString());
            }
            #endregion ListaFondoPM

            #region ListaAgoPM
            if (AreaPrelievo.FinalResponse.ListaAgoPM != null && AreaPrelievo.FinalResponse.ListaAgoPM.Count > 0)
            {
                if (datiPensioneFondoDatiPM == null)
                    datiPensioneFondoDatiPM = new GestioneFondo.DatiFondoPM();
                Data.CMSGTRA.Ago.PM agoPM = AreaPrelievo.FinalResponse.ListaAgoPM[0];

                if (agoPM.YPMTIPLQ != 0)
                    datiPensioneFondoDatiPM.CodiceTipoLiquidazione = byte.Parse(agoPM.YPMTIPLQ.ToString());
                if (!string.IsNullOrEmpty(agoPM.YPMTPCOD))
                    datiPensioneFondoDatiPM.CL413 = char.Parse(agoPM.YPMTPCOD);
            }
            #endregion ListaAgoPM

            #region ListaAnagrafica
            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                {
                    if (!string.IsNullOrEmpty(anagrafica.TRAANZ247) || anagrafica.TRARECUP != 0)
                    {
                        if (datiPensioneFondoDatiPM == null)
                            datiPensioneFondoDatiPM = new GestioneFondo.DatiFondoPM();

                        //datiPensioneFondoDatiPM.Requisiti247_243 = anagrafica.TRACODSI == "S" ? true : anagrafica.TRACODSI == "N" ? false : (bool?)null;
                        datiPensioneFondoDatiPM.AnzianitaAnni = Utility.StringToNullableInt(anagrafica.TRAANZ247);
                        if (anagrafica.TRARECUP != 0)
                        {
                            datiPensioneFondoDatiPM.AnnoRequisiti = Utility.StringToNullableShort(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(0, 4));
                            datiPensioneFondoDatiPM.NumeroTriSemRequisiti = Utility.StringToNullableByte(anagrafica.TRARECUP.ToString().PadLeft(5, '0').Substring(4, 1));
                        }
                    }
                }
            }
            #endregion ListaAnagrafica
        }

        public static void ValorizzaDatiCalcoloContributivo(Data.FSPR AreaPrelievo, GestionePensione.DatiPensione datiPensione, out List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo)
        {
            listaDatiCalcoloContributivo = null;
            if (AreaPrelievo.FinalResponse.ListaAgoEL != null && AreaPrelievo.FinalResponse.ListaAgoEL.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                Data.CMSGTRA.Ago.EL agoEL = AreaPrelievo.FinalResponse.ListaAgoEL[0];

                if (agoEL.YELCONTR != 0M)
                    datiCalcoloContributivo.ImportoContributivoTotale = agoEL.YELCONTR;
                if (agoEL.YELMONTA != 0M)
                    datiCalcoloContributivo.Montante = agoEL.YELMONTA;
                if (agoEL.YELSETTE != 0)
                    datiCalcoloContributivo.NSettimane = agoEL.YELSETTE;

                if (agoEL.YELIMPCRT != 0M)
                    datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoEL.YELIMPCRT;
                if (agoEL.YELMONTA2012 != 0M)
                    datiCalcoloContributivo.MontanteQuotaDL214 = agoEL.YELMONTA2012;
                if (agoEL.YELSETT2012 != 0)
                    datiCalcoloContributivo.NSettimaneQuotaDL214 = agoEL.YELSETT2012;

                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo> { datiCalcoloContributivo };
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoTT != null && AreaPrelievo.FinalResponse.ListaAgoTT.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                Data.CMSGTRA.Ago.TT agoTT = AreaPrelievo.FinalResponse.ListaAgoTT[0];

                if (agoTT.YTTCONTR != 0M)
                    datiCalcoloContributivo.ImportoContributivoTotale = agoTT.YTTCONTR;
                if (agoTT.YTTMONTA != 0M)
                    datiCalcoloContributivo.Montante = agoTT.YTTMONTA;
                if (agoTT.YTTSETTE != 0)
                    datiCalcoloContributivo.NSettimane = agoTT.YTTSETTE;

                if (agoTT.YTTIMPCRT != 0M)
                    datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoTT.YTTIMPCRT;
                if (agoTT.YTTMONTA2012 != 0M)
                    datiCalcoloContributivo.MontanteQuotaDL214 = agoTT.YTTMONTA2012;
                if (agoTT.YTTSETT2012 != 0)
                    datiCalcoloContributivo.NSettimaneQuotaDL214 = agoTT.YTTSETT2012;

                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo> { datiCalcoloContributivo };
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoET != null && AreaPrelievo.FinalResponse.ListaAgoET.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                Data.CMSGTRA.Ago.ET agoET = AreaPrelievo.FinalResponse.ListaAgoET[0];

                if (agoET.YETCONTR != 0M)
                    datiCalcoloContributivo.ImportoContributivoTotale = agoET.YETCONTR;
                if (agoET.YETMONTA != 0M)
                    datiCalcoloContributivo.Montante = agoET.YETMONTA;
                if (agoET.YETSETTE != 0)
                    datiCalcoloContributivo.NSettimane = agoET.YETSETTE;

                if (agoET.YETIMPCRT != 0M)
                    datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoET.YETIMPCRT;
                if (agoET.YETMONTA2012 != 0M)
                    datiCalcoloContributivo.MontanteQuotaDL214 = agoET.YETMONTA2012;
                if (agoET.YETSETT2012 != 0)
                    datiCalcoloContributivo.NSettimaneQuotaDL214 = agoET.YETSETT2012;

                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo> { datiCalcoloContributivo };
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoVL != null && AreaPrelievo.FinalResponse.ListaAgoVL.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                Data.CMSGTRA.Ago.VL agoVL = AreaPrelievo.FinalResponse.ListaAgoVL[0];

                if (agoVL.YVLCONTR != 0M)
                    datiCalcoloContributivo.ImportoContributivoTotale = agoVL.YVLCONTR;
                if (agoVL.YVLMONTA != 0M && agoVL.YVLTIPLQ == 3)
                {
                    datiCalcoloContributivo.MontanteAnte0697 = agoVL.YVLMONTA;
                }
                else if (agoVL.YVLMONTA != 0M)
                    datiCalcoloContributivo.Montante = agoVL.YVLMONTA;
                if (agoVL.YVLMONT2 != 0M && agoVL.YVLTIPLQ == 3)
                    datiCalcoloContributivo.Montante = agoVL.YVLMONT2;
                if (agoVL.YVLANZ1A != 0)
                    datiCalcoloContributivo.AnzianitaAnte0697AA = agoVL.YVLANZ1A;
                if (agoVL.YVLANZ1M != 0)
                    datiCalcoloContributivo.AnzianitaAnte0697MM = agoVL.YVLANZ1M;
                if (agoVL.YVLANZ1G != 0)
                    datiCalcoloContributivo.AnzianitaAnte0697GG = agoVL.YVLANZ1G;
                if (agoVL.YVLANZ2A != 0)
                    datiCalcoloContributivo.AnzianitaPost0697AA = agoVL.YVLANZ2A;
                if (agoVL.YVLANZ2M != 0)
                    datiCalcoloContributivo.AnzianitaPost0697MM = agoVL.YVLANZ2M;
                if (agoVL.YVLANZ2G != 0)
                    datiCalcoloContributivo.AnzianitaPost0697GG = agoVL.YVLANZ2G;

                if (agoVL.YVLIMPCRT != 0M)
                    datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoVL.YVLIMPCRT;
                if (agoVL.YVLMONTA2012 != 0M)
                    datiCalcoloContributivo.MontanteQuotaDL214 = agoVL.YVLMONTA2012;
                if (agoVL.YVLSETT2012 != 0)
                    datiCalcoloContributivo.NSettimaneQuotaDL214 = agoVL.YVLSETT2012;

                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo> { datiCalcoloContributivo };
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoGAS != null && AreaPrelievo.FinalResponse.ListaAgoGAS.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                Data.CMSGTRA.Ago.GAS agoGAS = AreaPrelievo.FinalResponse.ListaAgoGAS[0];

                if (agoGAS.YGAMONTA != 0M)
                    datiCalcoloContributivo.Montante = agoGAS.YGAMONTA;
                if (agoGAS.YGAESCLU != 0M)
                    datiCalcoloContributivo.MontanteEsclusivo = agoGAS.YGAESCLU;
                if (agoGAS.YGASETTE != 0)
                    datiCalcoloContributivo.NSettimane = agoGAS.YGASETTE;
                if (agoGAS.YGAMONTA2012 != 0M)
                    datiCalcoloContributivo.MontanteQuotaDL214 = agoGAS.YGAMONTA2012;
                if (agoGAS.YGASETT2012 != 0)
                    datiCalcoloContributivo.NSettimaneQuotaDL214 = agoGAS.YGASETT2012;
                if (agoGAS.YGAMONTAE2012 != 0M)
                    datiCalcoloContributivo.MontanteEsclusivoQuotaDL214 = agoGAS.YGAMONTAE2012;

                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo> { datiCalcoloContributivo };
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoDZ != null && AreaPrelievo.FinalResponse.ListaAgoDZ.Count > 0)
            {
                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                foreach (Data.CMSGTRA.Ago.DZ agoDZ in AreaPrelievo.FinalResponse.ListaAgoDZ)
                {
                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();

                    if (agoDZ.YDZMONTA2012 != 0M)
                        datiCalcoloContributivo.MontanteQuotaDL214 = agoDZ.YDZMONTA2012;
                    if (agoDZ.YDZSETT2012 != 0)
                        datiCalcoloContributivo.NSettimaneQuotaDL214 = agoDZ.YDZSETT2012;
                    if (agoDZ.YDZIMPCRT != 0M)
                        datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoDZ.YDZIMPCRT;
                    datiCalcoloContributivo.Id = agoDZ.YDZPROGR;
                    listaDatiCalcoloContributivo.Add(datiCalcoloContributivo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoES != null && AreaPrelievo.FinalResponse.ListaAgoES.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                Data.CMSGTRA.Ago.ES agoES = AreaPrelievo.FinalResponse.ListaAgoES[0];

                if (agoES.YESESCLU != 0)
                    datiCalcoloContributivo.MontanteEsclusivo = agoES.YESESCLU;
                if (agoES.YESMONTA != 0)
                    datiCalcoloContributivo.Montante = agoES.YESMONTA;
                if (agoES.YESMONTA2012 != 0)
                    datiCalcoloContributivo.MontanteQuotaDL214 = agoES.YESMONTA2012;
                if (agoES.YESSETT2012 != 0)
                    datiCalcoloContributivo.NSettimaneQuotaDL214 = agoES.YESSETT2012;
                if (agoES.YESSETTE != 0)
                    datiCalcoloContributivo.NSettimane = agoES.YESSETTE;
                if (agoES.YESTOTRT != 0)
                    datiCalcoloContributivo.ImportoContributivoTotale = agoES.YESTOTRT;
                if (agoES.YESIMPCRT != 0)
                    datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoES.YESIMPCRT * 10;

                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo> { datiCalcoloContributivo };
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoPM != null && AreaPrelievo.FinalResponse.ListaAgoPM.Count > 0)
            {
                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                Data.CMSGTRA.Ago.PM agoPM = AreaPrelievo.FinalResponse.ListaAgoPM[0];

                if (agoPM.YPMIMPCRT != 0M)
                    datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoPM.YPMIMPCRT;
                if (agoPM.YPMMONTA != 0M)
                    datiCalcoloContributivo.Montante = agoPM.YPMMONTA;
                if (agoPM.YPMMONTA2012 != 0M)
                    datiCalcoloContributivo.MontanteQuotaDL214 = agoPM.YPMMONTA2012;
                if (agoPM.YPMSETT2012 != 0)
                    datiCalcoloContributivo.NSettimaneQuotaDL214 = agoPM.YPMSETT2012;
                if (agoPM.YPMSETTE != 0)
                    datiCalcoloContributivo.NSettimane = agoPM.YPMSETTE;

                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo> { datiCalcoloContributivo };
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaAgoPT != null && AreaPrelievo.FinalResponse.ListaAgoPT.Count > 0)
            {
                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                foreach (Data.CMSGTRA.Ago.PT agoPT in AreaPrelievo.FinalResponse.ListaAgoPT)
                {
                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                    datiCalcoloContributivo.IdRecordFondo = agoPT.YFSPROGR;
                    if (agoPT.YFSQUOTAC != 0M)
                        datiCalcoloContributivo.MontanteContributivo = agoPT.YFSQUOTAC;
                    if (agoPT.YFSQUOTA2012 != 0M)
                        datiCalcoloContributivo.QuotaContributivaAnnua = agoPT.YFSQUOTA2012;
                    if (agoPT.YFSCONTR != 0M)
                        datiCalcoloContributivo.ImportoContributivoTotale = agoPT.YFSCONTR;
                    if (agoPT.YFSCONTR2012 != 0M)
                        datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoPT.YFSCONTR2012;
                    if (agoPT.YFSMONTA != 0M)
                        datiCalcoloContributivo.Montante = agoPT.YFSMONTA;
                    if (agoPT.YFSMONTA2012 != 0M)
                        datiCalcoloContributivo.MontanteQuotaDL214 = agoPT.YFSMONTA2012;
                    if (agoPT.YFSSETTC != 0)
                        datiCalcoloContributivo.NSettimane = agoPT.YFSSETTC;
                    if (agoPT.YFSSETT2012 != 0)
                        datiCalcoloContributivo.NSettimaneQuotaDL214 = agoPT.YFSSETT2012;

                    //Se il Tipo Calcolo è Retributivo e i campi precedenti sono tutti vuoti, il salvataggio dei dati contributivi non deve essere effettuato
                    if (!(datiPensione != null && datiPensione.TipoCalcolo == 18 && !datiCalcoloContributivo.MontanteContributivo.HasValue && !datiCalcoloContributivo.QuotaContributivaAnnua.HasValue && !datiCalcoloContributivo.ImportoContributivoTotale.HasValue
                        && !datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue && !datiCalcoloContributivo.Montante.HasValue && !datiCalcoloContributivo.MontanteQuotaDL214.HasValue && !datiCalcoloContributivo.NSettimane.HasValue && !datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue))
                        listaDatiCalcoloContributivo.Add(datiCalcoloContributivo);
                }
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaAgoFS != null && AreaPrelievo.FinalResponse.ListaAgoFS.Count > 0)
            {
                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                foreach (Data.CMSGTRA.Ago.FS agoFS in AreaPrelievo.FinalResponse.ListaAgoFS)
                {
                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                    datiCalcoloContributivo.IdRecordFondo = agoFS.YFSPROGR;
                    if (agoFS.YFSQUOTAC != 0M)
                        datiCalcoloContributivo.MontanteContributivo = agoFS.YFSQUOTAC;
                    if (agoFS.YFSQUOTA2012 != 0M)
                        datiCalcoloContributivo.QuotaContributivaAnnua = agoFS.YFSQUOTA2012;
                    if (agoFS.YFSCONTR != 0M)
                        datiCalcoloContributivo.ImportoContributivoTotale = agoFS.YFSCONTR;
                    if (agoFS.YFSCONTR2012 != 0M)
                        datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoFS.YFSCONTR2012;
                    if (agoFS.YFSMONTA != 0M)
                        datiCalcoloContributivo.Montante = agoFS.YFSMONTA;
                    if (agoFS.YFSMONTA2012 != 0M)
                        datiCalcoloContributivo.MontanteQuotaDL214 = agoFS.YFSMONTA2012;
                    if (agoFS.YFSSETTC != 0)
                        datiCalcoloContributivo.NSettimane = agoFS.YFSSETTC;
                    if (agoFS.YFSSETT2012 != 0)
                        datiCalcoloContributivo.NSettimaneQuotaDL214 = agoFS.YFSSETT2012;

                    //Se il Tipo Calcolo è Retributivo e i campi precedenti sono tutti vuoti, il salvataggio dei dati contributivi non deve essere effettuato
                    if (!(datiPensione != null && datiPensione.TipoCalcolo == 18 && !datiCalcoloContributivo.MontanteContributivo.HasValue && !datiCalcoloContributivo.QuotaContributivaAnnua.HasValue && !datiCalcoloContributivo.ImportoContributivoTotale.HasValue
                        && !datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue && !datiCalcoloContributivo.Montante.HasValue && !datiCalcoloContributivo.MontanteQuotaDL214.HasValue && !datiCalcoloContributivo.NSettimane.HasValue && !datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue))
                        listaDatiCalcoloContributivo.Add(datiCalcoloContributivo);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoGDP != null && AreaPrelievo.FinalResponse.ListaAgoGDP.Count > 0)
            {
                listaDatiCalcoloContributivo = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                foreach (Data.CMSGTRA.Ago.GDP agoGDP in AreaPrelievo.FinalResponse.ListaAgoGDP)
                {
                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = new GestioneCalcolo.DatiCalcoloContributivo();
                    datiCalcoloContributivo.IdRecordFondo = agoGDP.YFSPROGR;
                    if (agoGDP.YFSQUOTAC != 0M)
                        datiCalcoloContributivo.MontanteContributivo = agoGDP.YFSQUOTAC;
                    if (agoGDP.YFSQUOTA2012 != 0M)
                        datiCalcoloContributivo.QuotaContributivaAnnua = agoGDP.YFSQUOTA2012;
                    if (agoGDP.YFSCONTR != 0M)
                        datiCalcoloContributivo.ImportoContributivoTotale = agoGDP.YFSCONTR;
                    if (agoGDP.YFSCONTR2012 != 0M)
                        datiCalcoloContributivo.ImportoContribTotaleQuotaDL214 = agoGDP.YFSCONTR2012;
                    if (agoGDP.YFSMONTA != 0M)
                        datiCalcoloContributivo.Montante = agoGDP.YFSMONTA;
                    if (agoGDP.YFSMONTA2012 != 0M)
                        datiCalcoloContributivo.MontanteQuotaDL214 = agoGDP.YFSMONTA2012;
                    if (agoGDP.YFSSETTC != 0)
                        datiCalcoloContributivo.NSettimane = agoGDP.YFSSETTC;
                    if (agoGDP.YFSSETT2012 != 0)
                        datiCalcoloContributivo.NSettimaneQuotaDL214 = agoGDP.YFSSETT2012;

                    //Se il Tipo Calcolo è Retributivo e i campi precedenti sono tutti vuoti, il salvataggio dei dati contributivi non deve essere effettuato
                    if (!(datiPensione != null && datiPensione.TipoCalcolo == 18 && !datiCalcoloContributivo.MontanteContributivo.HasValue && !datiCalcoloContributivo.QuotaContributivaAnnua.HasValue && !datiCalcoloContributivo.ImportoContributivoTotale.HasValue
                    && !datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue && !datiCalcoloContributivo.Montante.HasValue && !datiCalcoloContributivo.MontanteQuotaDL214.HasValue && !datiCalcoloContributivo.NSettimane.HasValue && !datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue))
                        listaDatiCalcoloContributivo.Add(datiCalcoloContributivo);
                }
            }
        }

        public static void ValorizzaDatiCalcoloRetributivoDZ(Data.FSPR AreaPrelievo, out List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo)
        {
            listaDatiCalcoloRetributivo = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
            if (AreaPrelievo.FinalResponse.ListaAgoDZ != null && AreaPrelievo.FinalResponse.ListaAgoDZ.Count > 0)
            {
                foreach (Data.CMSGTRA.Ago.DZ agoDZ in AreaPrelievo.FinalResponse.ListaAgoDZ)
                {
                    GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                    if (agoDZ.YDZRSETA != 0M)
                        datiCalcoloRetributivo.RMSQuotaA = agoDZ.YDZRSETA;
                    if (agoDZ.YDZSETTA != 0)
                        datiCalcoloRetributivo.NSettimaneQuotaA = agoDZ.YDZSETTA;
                    if (agoDZ.YDZRSETB != 0M)
                        datiCalcoloRetributivo.RMSQuotaB = agoDZ.YDZRSETB;
                    if (agoDZ.YDZSETTB != 0)
                        datiCalcoloRetributivo.NSettimaneQuotaB = agoDZ.YDZSETTB;
                    datiCalcoloRetributivo.Id = agoDZ.YDZPROGR;
                    listaDatiCalcoloRetributivo.Add(datiCalcoloRetributivo);
                }
            }
        }

        public static void ValorizzaDatiCalcoloRetributivo(Data.FSPR AreaPrelievo, out GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo)
        {
            datiCalcoloRetributivo = null;
            if (AreaPrelievo.FinalResponse.ListaAgoEL != null && AreaPrelievo.FinalResponse.ListaAgoEL.Count > 0)
            {
                datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                Data.CMSGTRA.Ago.EL agoEL = AreaPrelievo.FinalResponse.ListaAgoEL[0];

                if (agoEL.YELRSETA != 0M)
                    datiCalcoloRetributivo.RMSQuotaA = agoEL.YELRSETA;
                if (agoEL.YELRSETB != 0M)
                    datiCalcoloRetributivo.RMSQuotaB = agoEL.YELRSETB;
                if (agoEL.YELRSETD != 0M)
                    datiCalcoloRetributivo.RMSQuotaD = agoEL.YELRSETD;
                if (agoEL.YELSETTA != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA = agoEL.YELSETTA;
                if (agoEL.YELSETTB != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaB = agoEL.YELSETTB;
                if (agoEL.YELSETTC != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaC = agoEL.YELSETTC;
                if (agoEL.YELSETTD != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaD = agoEL.YELSETTD;
                if (agoEL.YELTETTO != 0M)
                    datiCalcoloRetributivo.RetribuzionePonderataAnnua = agoEL.YELTETTO;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoTT != null && AreaPrelievo.FinalResponse.ListaAgoTT.Count > 0)
            {
                datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                Data.CMSGTRA.Ago.TT agoTT = AreaPrelievo.FinalResponse.ListaAgoTT[0];

                if (agoTT.YTTRSETA != 0M)
                    datiCalcoloRetributivo.RMSQuotaA = agoTT.YTTRSETA;
                if (agoTT.YTTRSETB != 0M)
                    datiCalcoloRetributivo.RMSQuotaB = agoTT.YTTRSETB;
                if (agoTT.YTTRSETD != 0M)
                    datiCalcoloRetributivo.RMSQuotaD = agoTT.YTTRSETD;
                if (agoTT.YTTSETTA != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA = agoTT.YTTSETTA;
                if (agoTT.YTTSETTB != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaB = agoTT.YTTSETTB;
                if (agoTT.YTTSETTC != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaC = agoTT.YTTSETTC;
                if (agoTT.YTTSETTD != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaD = agoTT.YTTSETTD;
                if (agoTT.YTTTETTO != 0M)
                    datiCalcoloRetributivo.RetribuzionePonderataAnnua = agoTT.YTTTETTO;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoET != null && AreaPrelievo.FinalResponse.ListaAgoET.Count > 0)
            {
                datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                Data.CMSGTRA.Ago.ET agoET = AreaPrelievo.FinalResponse.ListaAgoET[0];

                if (agoET.YETRSETA != 0M)
                    datiCalcoloRetributivo.RMSQuotaA = agoET.YETRSETA;
                if (agoET.YETRSETB != 0M)
                    datiCalcoloRetributivo.RMSQuotaB = agoET.YETRSETB;
                if (agoET.YETRSETD != 0M)
                    datiCalcoloRetributivo.RMSQuotaD = agoET.YETRSETD;
                if (agoET.YETSETTA != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA = agoET.YETSETTA;
                if (agoET.YETSETTB != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaB = agoET.YETSETTB;
                if (agoET.YETSETTC != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaC = agoET.YETSETTC;
                if (agoET.YETSETTD != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaD = agoET.YETSETTD;
                if (agoET.YETTETTO != 0M)
                    datiCalcoloRetributivo.RetribuzionePonderataAnnua = agoET.YETTETTO;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoVL != null && AreaPrelievo.FinalResponse.ListaAgoVL.Count > 0)
            {
                datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                Data.CMSGTRA.Ago.VL agoVL = AreaPrelievo.FinalResponse.ListaAgoVL[0];

                if (agoVL.YVLRSETA != 0M)
                    datiCalcoloRetributivo.RMSQuotaA = agoVL.YVLRSETA;
                if (agoVL.YVLRSETB != 0M)
                    datiCalcoloRetributivo.RMSQuotaB = agoVL.YVLRSETB;
                if (agoVL.YVLRSETD != 0M)
                    datiCalcoloRetributivo.RMSQuotaD = agoVL.YVLRSETD;
                if (agoVL.YVLSET1A != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA = agoVL.YVLSET1A;
                if (agoVL.YVLSET2A != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA2 = agoVL.YVLSET2A;
                if (agoVL.YVLSETTB != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaB = agoVL.YVLSETTB;
                if (agoVL.YVLSET1C != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaC = agoVL.YVLSET1C;
                if (agoVL.YVLSET2C != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaC2 = agoVL.YVLSET2C;
                if (agoVL.YVLSETTD != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaD = agoVL.YVLSETTD;
                if (agoVL.YVLTETTO != 0M)
                    datiCalcoloRetributivo.RetribuzionePonderataAnnua = agoVL.YVLTETTO;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoGAS != null && AreaPrelievo.FinalResponse.ListaAgoGAS.Count > 0)
            {
                datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                Data.CMSGTRA.Ago.GAS agoGas = AreaPrelievo.FinalResponse.ListaAgoGAS[0];

                if (agoGas.YGARETPN != 0M)
                    datiCalcoloRetributivo.RMSQuotaA = agoGas.YGARETPN;
                if (agoGas.YGAANZTO != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA = agoGas.YGAANZTO;
                if (agoGas.YGAANZES != 0)
                    datiCalcoloRetributivo.NSettimaneEsclusiveQuotaA = agoGas.YGAANZES;
                if (agoGas.YGARE2PN != 0M)
                    datiCalcoloRetributivo.RMSQuotaB = agoGas.YGARE2PN;
                if (agoGas.YGAANZT2 != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaB = agoGas.YGAANZT2;
                if (agoGas.YGAANZE2 != 0)
                    datiCalcoloRetributivo.NSettimaneEsclusiveQuotaB = agoGas.YGAANZE2;
            }
            //else if (AreaPrelievo.FinalResponse.ListaAgoDZ != null && AreaPrelievo.FinalResponse.ListaAgoDZ.Count > 0)
            //{
            //    datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
            //    Data.CMSGTRA.Ago.DZ agoDZ = AreaPrelievo.FinalResponse.ListaAgoDZ[0];

            //    if (agoDZ.YDZRSETA != 0M)
            //        datiCalcoloRetributivo.RMSQuotaA = agoDZ.YDZRSETA;
            //    if (agoDZ.YDZSETTA != 0)
            //        datiCalcoloRetributivo.NSettimaneQuotaA = agoDZ.YDZSETTA;
            //    if (agoDZ.YDZRSETB != 0M)
            //        datiCalcoloRetributivo.RMSQuotaB = agoDZ.YDZRSETB;
            //    if (agoDZ.YDZSETTB != 0)
            //        datiCalcoloRetributivo.NSettimaneQuotaB = agoDZ.YDZSETTB;
            //}
            else if (AreaPrelievo.FinalResponse.ListaAgoES != null && AreaPrelievo.FinalResponse.ListaAgoES.Count > 0)
            {
                datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                Data.CMSGTRA.Ago.ES agoES = AreaPrelievo.FinalResponse.ListaAgoES[0];

                if (agoES.YESANZT2 != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaB = agoES.YESANZT2;
                if (agoES.YESANZTO != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA = agoES.YESANZTO;
                if (agoES.YESRE2PN != 0)
                    datiCalcoloRetributivo.RMSQuotaB = agoES.YESRE2PN;
                if (agoES.YESRETPN != 0)
                    datiCalcoloRetributivo.RMSQuotaA = agoES.YESRETPN;
                if (agoES.YESVOLON != 0)
                    datiCalcoloRetributivo.NSettAnzianitaVV = agoES.YESVOLON;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoPM != null && AreaPrelievo.FinalResponse.ListaAgoPM.Count > 0)
            {
                datiCalcoloRetributivo = new GestioneCalcolo.DatiCalcoloRetributivo();
                Data.CMSGTRA.Ago.PM agoPM = AreaPrelievo.FinalResponse.ListaAgoPM[0];

                if (agoPM.YPMANZE1 != 0)
                    datiCalcoloRetributivo.NSettimaneEsclusiveQuotaB = agoPM.YPMANZE1;
                if (agoPM.YPMANZES != 0)
                    datiCalcoloRetributivo.NSettimaneEsclusiveQuotaA = agoPM.YPMANZES;
                if (agoPM.YPMANZT1 != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaB = agoPM.YPMANZT1;
                if (agoPM.YPMANZTO != 0)
                    datiCalcoloRetributivo.NSettimaneQuotaA = agoPM.YPMANZTO;
                if (agoPM.YPMRETP1 != 0M)
                    datiCalcoloRetributivo.RMSQuotaB = agoPM.YPMRETP1;
                if (agoPM.YPMRETPN != 0M)
                    datiCalcoloRetributivo.RMSQuotaA = agoPM.YPMRETPN;
            }
        }
        public static void ValorizzaDatiSupplementi(Data.FSPR AreaPrelievo, Utility.TipoFondo? tipoFondo, GestionePrelievo.TipoDomanda tipoDomanda, string prodotto, string tipo, out List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> ListaSupplementi)
        {
            ListaSupplementi = null;
            if (AreaPrelievo.FinalResponse.ListaSupplementi != null && AreaPrelievo.FinalResponse.ListaSupplementi.Count > 0)
            {
                string siglaCategoria = GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG);
                ListaSupplementi = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
                Data.CMSGTRA.Supplementi supplemento = AreaPrelievo.FinalResponse.ListaSupplementi[0];

                if (supplemento != null && supplemento.LISTTRE_SUP14 != null && supplemento.LISTTRE_SUP14.Count > 0)
                {
                    foreach (Data.CMSGTRA.Supplementi.TRE_SUP14 supp in supplemento.LISTTRE_SUP14)
                    {
                        INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupp = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
                        datiSupp.DecorrenzaSupplemento = Utility.DataFromInt(supp.TREDECAA, supp.TREDECMM, 1);

                        datiSupp.QuotaSupplemento = Utility.StringToNullableChar(supp.TREFLG01);
                        datiSupp.TipoSupplemento = Utility.StringToNullableChar(supp.TRENAT01);
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET)
                        {
                            if (datiSupp.TipoSupplemento.HasValue)
                            {
                                if (datiSupp.TipoSupplemento.Value == 'S')
                                {
                                    datiSupp.TipoSupplemento = 'R';
                                    datiSupp.QuotaSupplemento = 'B';
                                }
                                if (datiSupp.TipoSupplemento.Value == 'C' || datiSupp.TipoSupplemento.Value == 'D')
                                {
                                    datiSupp.MontanteSupplemento = supp.TRERMS01;
                                    if (datiSupp.MontanteSupplemento.Value > 0)
                                        datiSupp.QuotaSupplemento = 'B';
                                }
                                else if (datiSupp.TipoSupplemento == 'R')
                                    datiSupp.RMSSupplemento = supp.TRERMS01;

                                if (!datiSupp.QuotaSupplemento.HasValue)
                                    datiSupp.QuotaSupplemento = 'A';
                            }
                        }
                        else
                        {
                            if (datiSupp.TipoSupplemento.HasValue)
                            {

                                if (datiSupp.TipoSupplemento.Value == 'S')
                                {
                                    datiSupp.TipoSupplemento = 'R';
                                    if (!datiSupp.QuotaSupplemento.HasValue)
                                        datiSupp.QuotaSupplemento = 'B';
                                }
                                else if ((datiSupp.TipoSupplemento.Value == 'C' || datiSupp.TipoSupplemento.Value == 'D') && supp.TRERMS01 > 0)
                                {
                                    datiSupp.QuotaSupplemento = 'B';
                                }
                                else
                                    datiSupp.QuotaSupplemento = 'A';


                                if (datiSupp.TipoSupplemento.Value == 'C' || datiSupp.TipoSupplemento.Value == 'D')
                                    datiSupp.MontanteSupplemento = supp.TRERMS01;
                                else if (datiSupp.TipoSupplemento.Value == 'R')
                                    datiSupp.RMSSupplemento = supp.TRERMS01;
                            }
                        }
                        if (!string.IsNullOrEmpty(supp.TRETIP01))
                            datiSupp.CodGestioneSupplemento = supp.TRETIP01;
                        if (supp.TRETOT01 != 0)
                            datiSupp.NSettimaneSupplemento = supp.TRETOT01;
                        //ENG - MEMO 50/2023
                        if ((tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && 
                            ((prodotto.Trim() == "0107" || prodotto.Trim() == "0307" || prodotto.Trim() == "0407") && (tipo.Trim() == "0001" || tipo.Trim() == "0193")) || 
                            (prodotto.Trim() == "0102" || prodotto.Trim() == "0302" || prodotto.Trim() == "0402") || (prodotto.Trim() == "0114") && tipo.Trim() == "0001" ) 
                            && !IsCategoriaINPDAP(AreaPrelievo) && datiSupp.TipoSupplemento != null)
                            datiSupp.IsFromPrelievo = true;

                        ListaSupplementi.Add(datiSupp);
                    }
                }
            }
        }

        public static void ValorizzaDatiMaggiorazioni(Data.FSPR AreaPrelievo, ref GestionePensione.DatiPensione datiPensione, GestionePrelievo.TipoDomanda tipoDomanda, Utility.TipoFondo? fondo, string prodotto, string tipo,
            out INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            datiMaggiorazioniBenefici = null;
            Utility.TipoFondo? tipoFondo = null;
            if (AreaPrelievo.FinalResponse.ListaDelegato != null && AreaPrelievo.FinalResponse.ListaDelegato.Count > 0)
            {
                //ENG - per le Reversibilità prodotto = "0021" e tipo = "0001" (no 024 e INPDAP) precompilare il flag "EX-COMBATTENTE" mappato in TRBLG140 e ripassarlo al calcolo
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione ||
                    (tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità && prodotto.Trim() == "0021" && tipo.Trim() == "0001" && fondo.HasValue &&
                    (fondo.Value == Utility.TipoFondo.TT || fondo.Value == Utility.TipoFondo.EL || fondo.Value == Utility.TipoFondo.ET ||
                    fondo.Value == Utility.TipoFondo.DZ || fondo.Value == Utility.TipoFondo.ES || fondo.Value == Utility.TipoFondo.PM ||
                    fondo.Value == Utility.TipoFondo.PI || fondo.Value == Utility.TipoFondo.GAS || fondo.Value == Utility.TipoFondo.CL || fondo.Value == Utility.TipoFondo.VL || fondo.Value == Utility.TipoFondo.PL)))
                {
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                    Data.CMSGTRA.DelegatoNew delegato = AreaPrelievo.FinalResponse.ListaDelegato[0];

                    if (delegato.TRBSEN140 != 0)
                        datiMaggiorazioniBenefici.CodiceCieco = Utility.StringToNullableByte(delegato.TRBSEN140.ToString());
                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 = Utility.DataFromInt(short.Parse(delegato.TRBLG140.ToString().PadLeft(6, '0').Substring(2, 4)),
                        short.Parse(delegato.TRBLG140.ToString().PadLeft(6, '0').Substring(0, 2)), 1);
                }
            }
            if (AreaPrelievo.FinalResponse.ListaMaggiorazioneSociale != null && AreaPrelievo.FinalResponse.ListaMaggiorazioneSociale.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.MaggiorazioneSociale maggiorazioneSociale = AreaPrelievo.FinalResponse.ListaMaggiorazioneSociale[0];
                datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale = Utility.DataFromInt(short.Parse(maggiorazioneSociale.TRPDCMAG.ToString().PadLeft(8, '0').Substring(0, 4)),
                        short.Parse(maggiorazioneSociale.TRPDCMAG.ToString().PadLeft(8, '0').Substring(4, 2)), 1);
                datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale = Utility.DataFromInt(short.Parse(maggiorazioneSociale.TRPDATDO.ToString().PadLeft(8, '0').Substring(0, 4)),
                        short.Parse(maggiorazioneSociale.TRPDATDO.ToString().PadLeft(8, '0').Substring(4, 2)), 1);
            }

            List<short> elencoBeneficiAmmessiPerReversibilita = new List<short> { 1, 2, 3 };
            if (AreaPrelievo.FinalResponse.ListaFondoEL != null && AreaPrelievo.FinalResponse.ListaFondoEL.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.EL fondoEL = AreaPrelievo.FinalResponse.ListaFondoEL[0];
                if (fondoEL.XELNONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoEL.XELNONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoEL.XELNONVE.ToString().PadLeft(2, '0');
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                {
                    if (fondoEL.XELMAGGI != 0)
                        datiMaggiorazioniBenefici.PercentualeMaggiorazione = Utility.StringToNullableByte(fondoEL.XELMAGGI.ToString());
                    if (fondoEL.XELMG336 != 0)
                        datiMaggiorazioniBenefici.PercentualeMaggiorazioneSenzaLegge33670 = Utility.StringToNullableByte(fondoEL.XELMG336.ToString());
                    if (fondoEL.XELN2336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QB = fondoEL.XELN2336;
                    if (fondoEL.XELNO336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = fondoEL.XELNO336;

                    if (!string.IsNullOrEmpty(fondoEL.XELCOMBA) && fondoEL.XELCOMBA.Trim() != string.Empty)
                    {
                        List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                        GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                        GestioneDecodifica.CodiceMaggiorazioneExCombattenti codiceExComb = listaCodiciMaggExComb.Find(x => x.TraduzioneSuGP == fondoEL.XELCOMBA);
                        if (codiceExComb != null)
                            datiMaggiorazioniBenefici.ExCombattente = codiceExComb.Id;
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoTT != null && AreaPrelievo.FinalResponse.ListaFondoTT.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.TT fondoTT = AreaPrelievo.FinalResponse.ListaFondoTT[0];
                if (fondoTT.XTTNONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoTT.XTTNONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoTT.XTTNONVE.ToString().PadLeft(2, '0');
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoET != null && AreaPrelievo.FinalResponse.ListaFondoET.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.ET fondoET = AreaPrelievo.FinalResponse.ListaFondoET[0];
                if (fondoET.XETCDCIE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoET.XETCDCIE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoET.XETCDCIE.ToString().PadLeft(2, '0');
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                {
                    if (fondoET.XETN2336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QB = fondoET.XETN2336;
                    if (fondoET.XETNO336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = fondoET.XETNO336;

                    if (!string.IsNullOrEmpty(fondoET.XETEXCBT) && fondoET.XETEXCBT.Trim() != string.Empty)
                    {
                        List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                        GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                        GestioneDecodifica.CodiceMaggiorazioneExCombattenti codiceExComb = listaCodiciMaggExComb.Find(x => x.TraduzioneSuGP == fondoET.XETEXCBT);
                        if (codiceExComb != null)
                            datiMaggiorazioniBenefici.ExCombattente = codiceExComb.Id;
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoVL != null && AreaPrelievo.FinalResponse.ListaFondoVL.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.VL fondoVL = AreaPrelievo.FinalResponse.ListaFondoVL[0];
                if (fondoVL.XVLNONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoVL.XVLNONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoVL.XVLNONVE.ToString().PadLeft(2, '0');
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoFS != null && AreaPrelievo.FinalResponse.ListaFondoFS.Count > 0)
            {
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                {
                    tipoFondo = Utility.TipoFondo.FS;
                    if (datiMaggiorazioniBenefici == null)
                        datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                    Data.CMSGTRA.Fondo.FS fondoFS = AreaPrelievo.FinalResponse.ListaFondoFS[0];
                    if (fondoFS.XFSNO336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = fondoFS.XFSNO336;
                }
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoFS_New != null && AreaPrelievo.FinalResponse.ListaFondoFS_New.Count > 0)
            {
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                {
                    tipoFondo = Utility.TipoFondo.FS;
                    if (datiMaggiorazioniBenefici == null)
                        datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                    Data.CMSGTRA.Fondo.FS_New fondoFS = AreaPrelievo.FinalResponse.ListaFondoFS_New[0];
                    if (fondoFS.XFSNO336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = fondoFS.XFSNO336;
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGAS != null && AreaPrelievo.FinalResponse.ListaFondoGAS.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.GAS fondoGAS = AreaPrelievo.FinalResponse.ListaFondoGAS[0];
                if (fondoGAS.XGANONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoGAS.XGANONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoGAS.XGANONVE.ToString().PadLeft(2, '0');
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoCL != null && AreaPrelievo.FinalResponse.ListaFondoCL.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.CL fondoCL = AreaPrelievo.FinalResponse.ListaFondoCL[0];
                if (fondoCL.XCLNONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoCL.XCLNONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoCL.XCLNONVE.ToString().PadLeft(2, '0');
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoDZ != null && AreaPrelievo.FinalResponse.ListaFondoDZ.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.DZ fondoDZ = AreaPrelievo.FinalResponse.ListaFondoDZ[0];
                if (fondoDZ.XDZNONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoDZ.XDZNONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoDZ.XDZNONVE.ToString().PadLeft(2, '0');
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                {
                    if (fondoDZ.XDZNO336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = fondoDZ.XDZNO336;
                    if (fondoDZ.XDZN2336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QB = fondoDZ.XDZN2336;

                    if (!string.IsNullOrEmpty(fondoDZ.XDZCOMBA) && fondoDZ.XDZCOMBA.Trim() != string.Empty)
                    {
                        List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                        GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                        GestioneDecodifica.CodiceMaggiorazioneExCombattenti codiceExComb = listaCodiciMaggExComb.Find(x => x.TraduzioneSuGP == fondoDZ.XDZCOMBA);
                        if (codiceExComb != null)
                            datiMaggiorazioniBenefici.ExCombattente = codiceExComb.Id;
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoES != null && AreaPrelievo.FinalResponse.ListaFondoES.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.ES fondoES = AreaPrelievo.FinalResponse.ListaFondoES[0];
                if (fondoES.XESNONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoES.XESNONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoES.XESNONVE.ToString().PadLeft(2, '0');
                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                {
                    if (string.IsNullOrEmpty(fondoES.XESCOMBA))
                    {
                        List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                        GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                        if (listaCodiciMaggExComb != null && listaCodiciMaggExComb.Count > 0)
                        {
                            GestioneDecodifica.CodiceMaggiorazioneExCombattenti codMaggExComb = listaCodiciMaggExComb.Find(x => x.TraduzioneSuGP == fondoES.XESCOMBA.Trim());
                            if (codMaggExComb != null)
                                datiMaggiorazioniBenefici.ExCombattente = codMaggExComb.Id;
                        }
                    }
                    if (fondoES.XESNO336 != 0M)
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = fondoES.XESNO336;
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPM != null && AreaPrelievo.FinalResponse.ListaFondoPM.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.PM fondoPM = AreaPrelievo.FinalResponse.ListaFondoPM[0];
                if (fondoPM.XPMNONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoPM.XPMNONVE)))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoPM.XPMNONVE.ToString().PadLeft(2, '0');
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPI != null && AreaPrelievo.FinalResponse.ListaFondoPI.Count > 0)
            {
                if (datiMaggiorazioniBenefici == null)
                    datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                Data.CMSGTRA.Fondo.PI fondoPI = AreaPrelievo.FinalResponse.ListaFondoPI[0];
                string siglaCategoria = GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG);
                Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, siglaCategoria);

                if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
                {
                    if (fondoPI.XPINONVE != 0 && (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || elencoBeneficiAmmessiPerReversibilita.Contains(fondoPI.XPINONVE)))
                        datiMaggiorazioniBenefici.TipoSettimaneBeneficio = fondoPI.XPINONVE.ToString().PadLeft(2, '0');
                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione)
                    {
                        if (!string.IsNullOrEmpty(fondoPI.XPIEXCBT))
                            datiMaggiorazioniBenefici.CodiceCieco = Utility.StringToNullableByte(fondoPI.XPIEXCBT);
                        if (fondoPI.XPINO336 != 0M)
                            datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = fondoPI.XPINO336;
                    }
                }
            }

            if (AreaPrelievo.FinalResponse.ListaResidenza != null && AreaPrelievo.FinalResponse.ListaResidenza.Count > 0)
            {
                Data.CMSGTRA.Residenza residenza = AreaPrelievo.FinalResponse.ListaResidenza.FirstOrDefault();
                if (residenza != null)
                {
                    if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01")
                    {
                        if (residenza.THR_SET_NONVE != 0)
                            datiMaggiorazioniBenefici.NSettimaneBeneficio = residenza.THR_SET_NONVE;
                        if (residenza.THR_SET_NONVE_P95 != 0)
                            datiMaggiorazioniBenefici.SettAnzContribPost311295 = (short)residenza.THR_SET_NONVE_P95;
                    }

                    if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && residenza.LISTTRHONERE != null && residenza.LISTTRHONERE.Count > 0)
                    {
                        List<string> listaCodBenefici = new List<string> { "11", "12", "13", "14", "15", "18", "19", "24" };
                        foreach (Data.CMSGTRA.Residenza.TRHONERE on in residenza.LISTTRHONERE)
                        {
                            if (!string.IsNullOrEmpty(on.TRH_CODBENEF) && listaCodBenefici.Contains(on.TRH_CODBENEF))
                            {
                                datiMaggiorazioniBenefici.TipoSettimaneBeneficio = on.TRH_CODBENEF;
                                break;
                            }
                        }
                    }
                }
            }

            if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
            {
                switch (datiMaggiorazioniBenefici.TipoSettimaneBeneficio)
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

            if (datiMaggiorazioniBenefici != null && !datiMaggiorazioniBenefici.IsBeneficiFSNull())
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                datiPensione.Benefici = true;
            }

            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.FS)
            {
                if (datiMaggiorazioniBenefici != null && !datiMaggiorazioniBenefici.IsExCombattenteFSNull_FondoFS())
                {
                    if (datiPensione == null)
                        datiPensione = new GestionePensione.DatiPensione();
                    datiPensione.ExCombattente = true;
                }
            }
            else
            {
                if (datiMaggiorazioniBenefici != null && !datiMaggiorazioniBenefici.IsExCombattenteFSNull())
                {
                    if (datiPensione == null)
                        datiPensione = new GestionePensione.DatiPensione();
                    datiPensione.ExCombattente = true;
                }
            }
        }

        public static void ValorizzaDatiPatronato(Data.FSPR AreaPrelievo, out GestionePensione.DatiPatronato datiPatronato)
        {
            datiPatronato = null;
            if (AreaPrelievo.FinalResponse.ListaDelegato != null && AreaPrelievo.FinalResponse.ListaDelegato.Count > 0)
            {
                datiPatronato = new GestionePensione.DatiPatronato();
                Data.CMSGTRA.DelegatoNew delegato = AreaPrelievo.FinalResponse.ListaDelegato[0];

                datiPatronato.CodiceEnte = delegato.TRBTIPOENTEPAT;
                datiPatronato.CodiceUfficio = delegato.TRBUFFZONALE;
                datiPatronato.NPratica = delegato.TRBNUMPRATICA;
                datiPatronato.TipoUfficio = delegato.TRBTIPOUFFPAT == "01" ? "02" : delegato.TRBTIPOUFFPAT == "02" ? "23" : "";
            }
        }

        public static void ValorizzaDatiDanteCausa(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, Utility.TipoFondo? tipoFondo, out DatiAnagDanteCausa datiAnagDanteCausa, out GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            datiAnagDanteCausa = null;
            datiDanteCausa = null;
            if (AreaPrelievo.FinalResponse.ListaDanteCausa != null && AreaPrelievo.FinalResponse.ListaDanteCausa.Count > 0)
            {
                datiAnagDanteCausa = new DatiAnagDanteCausa();
                datiDanteCausa = new GestioneDanteCausa.DatiDanteCausa();
                Data.CMSGTRA.DanteCausa danteCausa = AreaPrelievo.FinalResponse.ListaDanteCausa[0];

                List<GestioneDecodifica.CodiceEliminazione> lstDecCodiceEliminazione;
                GestioneDecodifica.GetCodiceEliminazioneByTipologia(out lstDecCodiceEliminazione, Utility.TipoAppartenenza.FS);

                string cognNome = danteCausa.TRDCONOM;
                datiAnagDanteCausa.Cognome = !string.IsNullOrEmpty(cognNome) ? danteCausa.TRDCONOM.Trim().Substring(0, danteCausa.TRDCONOM.IndexOf('/')) : string.Empty;
                datiAnagDanteCausa.Nome = !string.IsNullOrEmpty(cognNome) ? danteCausa.TRDCONOM.Trim().Substring(danteCausa.TRDCONOM.IndexOf('/') + 1) : string.Empty;
                datiAnagDanteCausa.Sesso = Utility.StringToNullableChar(danteCausa.TRDSESSO);
                datiAnagDanteCausa.DataNascita = Utility.DataFromInt(danteCausa.TRDAANAS, danteCausa.TRDMMNAS, danteCausa.TRDGGNAS);
                datiAnagDanteCausa.CodiceComuneInps = danteCausa.TRDCONAS;
                if (danteCausa.TRDDTMATR != 0)
                {
                    datiAnagDanteCausa.DataMatrimonio = Utility.DataFromString(danteCausa.TRDDTMATR.ToString().PadLeft(8, '0'), Utility.FormatoData.GGmmAAAA);
                    // Se non riesco a recuperare la data, oppure se la data è in formato AAAAMMGG, quindi con anno recuperato pari al massimo a AAAA1231
                    if (!datiAnagDanteCausa.DataMatrimonio.HasValue || datiAnagDanteCausa.DataMatrimonio.Value.Year <= 1231)
                        datiAnagDanteCausa.DataMatrimonio = Utility.DataFromString(danteCausa.TRDDTMATR.ToString(), Utility.FormatoData.AAAAmmGG);
                }

                datiDanteCausa.DataMorte = Utility.DataFromInt(danteCausa.TRDMORAA, danteCausa.TRDMORMM, danteCausa.TRDMORGG);

                if (danteCausa.TRDCARIC != 0)
                    datiDanteCausa.Sede = danteCausa.TRDCARIC.ToString().PadLeft(4, '0');
                if (!String.IsNullOrEmpty(danteCausa.TRDCATEG))
                    datiDanteCausa.SiglaCategoria = GetCategoriaFromTRACATEG(danteCausa.TRDCATEG);
                if (danteCausa.TRDCERTI != 0)
                {
                    datiDanteCausa.Certificato = danteCausa.TRDCERTI;
                    if (IsCategoriaINPDAP(AreaPrelievo) && AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica[0] != null)
                    {
                        datiDanteCausa.DecorrenzaPensione = Utility.DataFromInt(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFMM,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFGG);
                    }
                }

                string siglaCategoria = AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0 ?
                GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG) : string.Empty;

                string tipoReversibilità = "";
                if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica[0] != null)
                    tipoReversibilità = AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRATIPIR;

                if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S") && !String.IsNullOrEmpty(tipoReversibilità) && tipoReversibilità == "R")
                {
                    if (danteCausa.TRDMORAA != 0 && danteCausa.TRDMORMM != 0 && danteCausa.TRDMORGG != 0)
                        datiDanteCausa.DecorrenzaEliminazione = Utility.DataFromInt(danteCausa.TRDMORAA, danteCausa.TRDMORMM, danteCausa.TRDMORGG).Value.AddMonths(1);

                    //ENG - RIC Reversibilita 024: la decorrenza della diretta deve essere letta dal prelievo e non dal GP7
                    if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count() > 0 && AreaPrelievo.FinalResponse.ListaAnagrafica[0] != null
                        && tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.PT || tipoFondo.Value == Utility.TipoFondo.FS))
                    {
                        if (!String.IsNullOrEmpty(datiAnagDanteCausa.Cognome) && !String.IsNullOrEmpty(datiAnagDanteCausa.Nome) && datiAnagDanteCausa.DataNascita.HasValue)
                        {
                            datiDanteCausa.DecorrenzaPensione = Utility.DataFromInt(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA,
                            AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFMM,
                            AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFGG);
                        }
                    }
                }
                else
                    datiDanteCausa.DecorrenzaEliminazione = Utility.DataFromInt(danteCausa.TRDCDEAA, danteCausa.TRDCDEMM, 1);

                datiDanteCausa.DecorrenzaEliminazioneContabile = Utility.DataFromInt(danteCausa.TRDCNNAA, danteCausa.TRDCNNMM, 1);
                if (danteCausa.TRDCODEL != 0)
                {
                    GestioneDecodifica.CodiceEliminazione codiceEliminazione = lstDecCodiceEliminazione.Find(x => x.TraduzioneSuGP == danteCausa.TRDCODEL.ToString()[0]);
                    if (codiceEliminazione != null)
                        datiDanteCausa.CodiceEliminazione = Utility.StringToNullableByte(codiceEliminazione.Id);
                }
            }
        }

        public static void ValorizzaDatiResidenzeEstere(Data.FSPR AreaPrelievo, out List<GestioneAnagrafica.DatiResidenzaEstero> ListaResidenzeEstere)
        {
            ListaResidenzeEstere = null;
            if (AreaPrelievo.FinalResponse.ListaResidenza != null && AreaPrelievo.FinalResponse.ListaResidenza.Count > 0)
            {
                ListaResidenzeEstere = new List<GestioneAnagrafica.DatiResidenzaEstero>();
                Data.CMSGTRA.Residenza residenza = AreaPrelievo.FinalResponse.ListaResidenza[0];

                if (residenza != null && residenza.LISTTRHELERD != null && residenza.LISTTRHELERD.Count > 0)
                {
                    foreach (Data.CMSGTRA.Residenza.TRHELERD res in residenza.LISTTRHELERD)
                    {
                        if (res.TRHAAR01 != 0 && res.TRHMMR01 != 0 && res.TRHSTA01 != "000")
                        {
                            GestioneAnagrafica.DatiResidenzaEstero datiRes = new GestioneAnagrafica.DatiResidenzaEstero();
                            datiRes.Decorrenza = Utility.DataFromInt(res.TRHAAR01, res.TRHMMR01, 1);
                            if (res.TRHSTA01 == "ITA")
                                datiRes.CodCatastaleStatoEE = "Z000";
                            else if (res.TRHSTA01 == "EE")
                                datiRes.CodCatastaleStatoEE = "";
                            else
                            {
                                List<GestioneDecodifica.StatoEstero> elencoStatiEsteri = null;
                                GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteri);
                                elencoStatiEsteri = elencoStatiEsteri.FindAll(x => x.Sigla == res.TRHSTA01).ToList<GestioneDecodifica.StatoEstero>();
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

        public static void ValorizzaDatiStatiCivili(Data.FSPR AreaPrelievo, out List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCivili)
        {
            ListaStatiCivili = null;
            if (AreaPrelievo.FinalResponse.ListaTrattamentiFamiglia != null && AreaPrelievo.FinalResponse.ListaTrattamentiFamiglia.Count > 0)
            {
                ListaStatiCivili = new List<GestioneAnagrafica.DatiStatoCivile>();
                Data.CMSGTRA.TrattamentiFamiglia trattamentiFamiglia = AreaPrelievo.FinalResponse.ListaTrattamentiFamiglia[0];

                if (trattamentiFamiglia != null && trattamentiFamiglia.LISTTRFELENU != null && trattamentiFamiglia.LISTTRFELENU.Count > 0)
                {
                    foreach (Data.CMSGTRA.TrattamentiFamiglia.TRFELENU stCiv in trattamentiFamiglia.LISTTRFELENU)
                    {
                        if (stCiv.TRFDECAA != 0 && stCiv.TRFDECMM != 0 && !string.IsNullOrEmpty(stCiv.TRFSTA01))
                        {
                            GestioneAnagrafica.DatiStatoCivile statoCivile = new GestioneAnagrafica.DatiStatoCivile();
                            statoCivile.Decorrenza = Utility.DataFromInt(stCiv.TRFDECAA, stCiv.TRFDECMM, 1);
                            if (!string.IsNullOrEmpty(stCiv.TRFSTA01))
                                statoCivile.Codice = stCiv.TRFSTA01.ToString()[0];

                            ListaStatiCivili.Add(statoCivile);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiDelegato(Data.FSPR AreaPrelievo, out DatiDelegato datiDelegato)
        {
            datiDelegato = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.ListaDelegheTutele != null)
            {
                datiDelegato = new DatiDelegato();
                Data.CMSGTRA.Deleghe_Tutele delegato = AreaPrelievo.FinalResponse.ListaDelegheTutele[0];

                datiDelegato.CodiceDelegato = delegato.CODDEL_GP1AP01;
                datiDelegato.CodiceFiscale = delegato.CODFIS_GP1AP26;
            }
        }

        public static void ValorizzaDatiTutore(Data.FSPR AreaPrelievo, out DatiTutore datiTutore)
        {
            datiTutore = null;
            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.ListaDelegheTutele != null)
            {
                datiTutore = new DatiTutore();
                Data.CMSGTRA.Deleghe_Tutele tutore = AreaPrelievo.FinalResponse.ListaDelegheTutele[0];

                datiTutore.CodiceTutore = tutore.CODDEL_GP1AP61;
                datiTutore.CodiceFiscale = tutore.CODFIS_GP1AP66;
                if (tutore.DATACES_GP1AP70A != 0 && tutore.DATACES_GP1AP70M != 0)
                {
                    datiTutore.CessValAmmSost = Utility.DataFromInt(tutore.DATACES_GP1AP70A, tutore.DATACES_GP1AP70M, 1);
                }
            }

            if (datiTutore != null && datiTutore.IsNull())
                datiTutore = null;
        }

        public static void ValorizzaDatiIstruttoria(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, ref GestionePensione.DatiPensione datiPensione, string gruppo, Utility.TipoFondo? tipoFondo, out GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            datiIstruttoria = null;
            if (AreaPrelievo.FinalResponse.ListaAnagrafica != null && AreaPrelievo.FinalResponse.ListaAnagrafica.Count > 0)
            {
                datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];

                List<GestioneDecodifica.CodiceParticolare> elencoCodiciParticolari = null;
                GestioneDecodifica.GetCodiciParticolari(out elencoCodiciParticolari);

                List<GestioneDecodifica.ComunicazioneCampo3> elencoDecodificaComunicazioneCampo3 = null;
                GestioneDecodifica.GetComunicazioneCampo3(out elencoDecodificaComunicazioneCampo3);

                if (elencoCodiciParticolari != null && elencoCodiciParticolari.Count > 0 && anagrafica.TRAESODAN.Length >= 1)
                {
                    if (!String.IsNullOrEmpty(anagrafica.TRACATEG))
                    {
                        string codCat = string.Empty;
                        GestioneDecodifica.GetCodCategoriaBySiglaCategoria(GetCategoriaFromTRACATEG(anagrafica.TRACATEG.Trim()), out codCat);
                        if (!String.IsNullOrEmpty(codCat))
                        {
                            char? traesodan = Utility.StringToNullableChar(anagrafica.TRAESODAN.Substring(0, 1));
                            if (traesodan == 'U')
                                traesodan = '3';

                            GestioneDecodifica.CodiceParticolare codPart = elencoCodiciParticolari.Find(x =>
                                x.TraduzioneSuGp == traesodan && x.CodCategoria.Trim().ToUpperInvariant() == codCat.Trim().ToUpperInvariant());
                            if (codPart != null)
                                datiIstruttoria.CodiceParticolareSoggettoDerogato = codPart.Id;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(anagrafica.TRACDCOM1))
                {
                    datiIstruttoria.CodiceComunicazioneCampo1 = anagrafica.TRACDCOM1.Length == 1 ? Utility.StringToNullableByte(anagrafica.TRACDCOM1) : (anagrafica.TRACDCOM1.Length == 2 ? Utility.StringToNullableByte(anagrafica.TRACDCOM1.Substring(0, 1)) : null);
                    datiIstruttoria.CodiceComunicazioneCampo2 = anagrafica.TRACDCOM1.Length == 1 ? null : (anagrafica.TRACDCOM1.Length == 2 ? Utility.StringToNullableChar(anagrafica.TRACDCOM1.Substring(1, 1)) : null);
                }

                if (!(tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.FS || tipoFondo.Value == Utility.TipoFondo.PT) && tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && gruppo != "0031") &&
                    !String.IsNullOrEmpty(anagrafica.TRACDCOM3) && !String.IsNullOrEmpty(anagrafica.TRACDCOM3.Trim()) && elencoDecodificaComunicazioneCampo3 != null && elencoDecodificaComunicazioneCampo3.Count() > 0 &&
                    elencoDecodificaComunicazioneCampo3.Exists(x => x.Id.Trim().ToUpperInvariant() == anagrafica.TRACDCOM3.Trim().ToUpperInvariant()))
                {
                    datiIstruttoria.CodiceComunicazioneCampo3 = Utility.StringToNullableChar(anagrafica.TRACDCOM3);
                    datiIstruttoria.Provvisoria = anagrafica.TRACDCOM3.Trim() != "Y"; //Y indica, in fase di prelievo, che la domanda è definitiva (ex codice Q)
                }
                else
                    datiIstruttoria.Provvisoria = false;

                if (!String.IsNullOrEmpty(anagrafica.TRACDCOM4))
                    datiIstruttoria.CodiceComunicazioneCampo4 = Utility.StringToNullableByte(anagrafica.TRACDCOM4);
            }

            if (AreaPrelievo.FinalResponse.ListaFondoEL != null && AreaPrelievo.FinalResponse.ListaFondoEL.Count > 0)
            {
                if (datiIstruttoria == null)
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                Data.CMSGTRA.Fondo.EL fondoEL = AreaPrelievo.FinalResponse.ListaFondoEL[0];

                if (fondoEL.XELSEDE != 0)
                    datiIstruttoria.SedePrecedentePensione = fondoEL.XELSEDE;
                if (fondoEL.XELCATEG != 0)
                    datiIstruttoria.CodiceP18PrecedentePensione = fondoEL.XELCATEG;
                if (fondoEL.XELCERTI != 0)
                    datiIstruttoria.CertificatoPrecedentePensione = fondoEL.XELCERTI;
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoTT != null && AreaPrelievo.FinalResponse.ListaFondoTT.Count > 0)
            {
                if (datiIstruttoria == null)
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                Data.CMSGTRA.Fondo.TT fondoTT = AreaPrelievo.FinalResponse.ListaFondoTT[0];

                if (fondoTT.XTTSEDE != 0)
                    datiIstruttoria.SedePrecedentePensione = fondoTT.XTTSEDE;
                if (fondoTT.XTTCATEG != 0)
                    datiIstruttoria.CodiceP18PrecedentePensione = fondoTT.XTTCATEG;
                if (fondoTT.XTTCERTI != 0)
                    datiIstruttoria.CertificatoPrecedentePensione = fondoTT.XTTCERTI;
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoET != null && AreaPrelievo.FinalResponse.ListaFondoET.Count > 0)
            {
                if (datiIstruttoria == null)
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                Data.CMSGTRA.Fondo.ET fondoET = AreaPrelievo.FinalResponse.ListaFondoET[0];

                if (fondoET.XETSEDE != 0)
                    datiIstruttoria.SedePrecedentePensione = fondoET.XETSEDE;
                if (fondoET.XETCATEG != 0)
                    datiIstruttoria.CodiceP18PrecedentePensione = fondoET.XETCATEG;
                if (fondoET.XETCERTI != 0)
                    datiIstruttoria.CertificatoPrecedentePensione = fondoET.XETCERTI;
            }

            if ((tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione || tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità) && datiIstruttoria != null &&
                (datiIstruttoria.SedePrecedentePensione.HasValue || datiIstruttoria.CodiceP18PrecedentePensione.HasValue || datiIstruttoria.CertificatoPrecedentePensione.HasValue))
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                datiPensione.TrasformazioneAOI = true;
            }
        }

        public static void ValorizzaDatiDL407(Data.FSPR AreaPrelievo, DateTime? decorrenzaOriginaria, ref GestioneFondo.DatiFondo datiFondo, ref GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out GestioneDL407.DatiDL407 datiDL407)
        {
            datiDL407 = null;

            if (AreaPrelievo.FinalResponse.ListaAgoEL != null && AreaPrelievo.FinalResponse.ListaAgoEL.Count > 0)
            {
                datiDL407 = new GestioneDL407.DatiDL407();
                Data.CMSGTRA.Ago.EL agoEL = AreaPrelievo.FinalResponse.ListaAgoEL[0];

                if (agoEL.YELRETRA != 0M)
                    datiDL407.RMSQuotaA = agoEL.YELRETRA;
                if (agoEL.YELRETRB != 0M)
                    datiDL407.RMSQuotaB = agoEL.YELRETRB;
                if (agoEL.YELRETRD != 0M)
                    datiDL407.RMSQuotaD = agoEL.YELRETRD;
                if (agoEL.YELRETTA != 0)
                    datiDL407.NSettimaneQuotaA = agoEL.YELRETTA;
                if (agoEL.YELRETTB != 0)
                    datiDL407.NSettimaneQuotaB = agoEL.YELRETTB;
                if (agoEL.YELRETTC != 0)
                    datiDL407.NSettimaneQuotaC = agoEL.YELRETTC;
                if (agoEL.YELRETTD != 0)
                    datiDL407.NSettimaneQuotaD = agoEL.YELRETTD;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoTT != null && AreaPrelievo.FinalResponse.ListaAgoTT.Count > 0)
            {
                datiDL407 = new GestioneDL407.DatiDL407();
                Data.CMSGTRA.Ago.TT agoTT = AreaPrelievo.FinalResponse.ListaAgoTT[0];

                if (agoTT.YTTRETRA != 0M)
                    datiDL407.RMSQuotaA = agoTT.YTTRETRA;
                if (agoTT.YTTRETRB != 0M)
                    datiDL407.RMSQuotaB = agoTT.YTTRETRB;
                if (agoTT.YTTRETRD != 0M)
                    datiDL407.RMSQuotaD = agoTT.YTTRETRD;
                if (agoTT.YTTRETTA != 0)
                    datiDL407.NSettimaneQuotaA = agoTT.YTTRETTA;
                if (agoTT.YTTRETTB != 0)
                    datiDL407.NSettimaneQuotaB = agoTT.YTTRETTB;
                if (agoTT.YTTRETTC != 0)
                    datiDL407.NSettimaneQuotaC = agoTT.YTTRETTC;
                if (agoTT.YTTRETTD != 0)
                    datiDL407.NSettimaneQuotaD = agoTT.YTTRETTD;
            }
            else if (AreaPrelievo.FinalResponse.ListaAgoET != null && AreaPrelievo.FinalResponse.ListaAgoET.Count > 0)
            {
                datiDL407 = new GestioneDL407.DatiDL407();
                Data.CMSGTRA.Ago.ET agoET = AreaPrelievo.FinalResponse.ListaAgoET[0];

                if (agoET.YETRETRA != 0M)
                    datiDL407.RMSQuotaA = agoET.YETRETRA;
                if (agoET.YETRETRB != 0M)
                    datiDL407.RMSQuotaB = agoET.YETRETRB;
                if (agoET.YETRETRD != 0M)
                    datiDL407.RMSQuotaD = agoET.YETRETRD;
                if (agoET.YETRETTA != 0)
                    datiDL407.NSettimaneQuotaA = agoET.YETRETTA;
                if (agoET.YETRETTB != 0)
                    datiDL407.NSettimaneQuotaB = agoET.YETRETTB;
                if (agoET.YETRETTC != 0)
                    datiDL407.NSettimaneQuotaC = agoET.YETRETTC;
                if (agoET.YETRETTD != 0)
                    datiDL407.NSettimaneQuotaD = agoET.YETRETTD;
            }

            if (AreaPrelievo.FinalResponse.ListaMaggiorazioneLegge != null && AreaPrelievo.FinalResponse.ListaMaggiorazioneLegge.Count > 0)
            {
                if (datiDL407 == null)
                    datiDL407 = new GestioneDL407.DatiDL407();

                Data.CMSGTRA.MaggiorazioneLegge maggiorazioneLegge = AreaPrelievo.FinalResponse.ListaMaggiorazioneLegge[0];

                if (maggiorazioneLegge.TRIRA336 != 0M && maggiorazioneLegge.TRIRA336 != 111111.1111M)
                    datiDL407.RetribPensSL336QuotaA = maggiorazioneLegge.TRIRA336;
                if (maggiorazioneLegge.TRIRB336 != 0M && maggiorazioneLegge.TRIRB336 != 111111.1111M)
                    datiDL407.RetribPensSL336QuotaB = maggiorazioneLegge.TRIRB336;
                if (maggiorazioneLegge.TRIRETQA != 0M)
                    datiDL407.RetribPensQuotaA = maggiorazioneLegge.TRIRETQA;
                if (maggiorazioneLegge.TRIRETQB != 0M)
                    datiDL407.RetribPensQuotaB = maggiorazioneLegge.TRIRETQB;
                if (maggiorazioneLegge.TRISEUTA != 0)
                    datiDL407.ServizioUtileAAQuotaA = Utility.StringToNullableByte(maggiorazioneLegge.TRISEUTA.ToString());
                if (maggiorazioneLegge.TRISEUTB != 0)
                    datiDL407.ServizioUtileAAQuotaB = Utility.StringToNullableByte(maggiorazioneLegge.TRISEUTB.ToString());
                if (maggiorazioneLegge.TRISEUTC != 0)
                    datiDL407.ServizioUtileAAQuotaC = Utility.StringToNullableByte(maggiorazioneLegge.TRISEUTC.ToString());
                if (maggiorazioneLegge.TRIUTIAA != 0)
                    datiDL407.ServizioUtileAAQuotaA = Utility.StringToNullableByte(maggiorazioneLegge.TRIUTIAA.ToString());
                if (maggiorazioneLegge.TRIUTIAB != 0)
                    datiDL407.ServizioUtileAAQuotaB = Utility.StringToNullableByte(maggiorazioneLegge.TRIUTIAB.ToString());
            }

            if (datiDL407 != null && !Utility.ConfrontaOggetti(datiDL407, new GestioneDL407.DatiDL407()))
            {
                if (datiFondo == null)
                    datiFondo = new GestioneFondo.DatiFondo();
                datiFondo.ChkDL407 = true;
            }
        }

        public static void ValorizzaDatiOneriTerrorismo(Data.FSPR AreaPrelievo, ref GestionePensione.DatiPensione datiPensione, out List<Entity.DatiBenefici.OneriTerrorismo> listaOneriTerrorismo)
        {
            listaOneriTerrorismo = null;
            if (AreaPrelievo.FinalResponse.ListaDelegato != null && AreaPrelievo.FinalResponse.ListaDelegato.Count > 0)
            {
                listaOneriTerrorismo = new List<Entity.DatiBenefici.OneriTerrorismo>();

                Data.CMSGTRA.DelegatoNew delegato = AreaPrelievo.FinalResponse.ListaDelegato[0];
                Entity.DatiBenefici.OneriTerrorismo onTer = null;
                if (delegato.TRBONERI1 != 0M)
                {
                    onTer = new Entity.DatiBenefici.OneriTerrorismo();
                    onTer.Importo = delegato.TRBONERI1;
                    onTer.Progressivo = 1;
                    onTer.CodiceAltroFondo = 548;
                    listaOneriTerrorismo.Add(onTer);
                }
                if (delegato.TRBONERI2 != 0M)
                {
                    onTer = new Entity.DatiBenefici.OneriTerrorismo();
                    onTer.Importo = delegato.TRBONERI2;
                    onTer.Progressivo = 2;
                    onTer.CodiceAltroFondo = 548;
                    listaOneriTerrorismo.Add(onTer);
                }
                if (delegato.TRBONERI3 != 0M)
                {
                    onTer = new Entity.DatiBenefici.OneriTerrorismo();
                    onTer.Importo = delegato.TRBONERI3;
                    onTer.Progressivo = 3;
                    onTer.CodiceAltroFondo = 548;
                    listaOneriTerrorismo.Add(onTer);
                }
            }
            if (listaOneriTerrorismo != null && listaOneriTerrorismo.Count > 0)
            {
                if (datiPensione == null)
                    datiPensione = new GestionePensione.DatiPensione();
                datiPensione.Benefici = true;
            }
        }

        public static void ValorizzaDatiOneri(Data.FSPR AreaPrelievo, out List<GestioneOneri.DatiOneri> ListaDatiOneri)
        {
            ListaDatiOneri = null;
            if (AreaPrelievo.FinalResponse.ListaResidenza != null && AreaPrelievo.FinalResponse.ListaResidenza.Count > 0)
            {
                ListaDatiOneri = new List<GestioneOneri.DatiOneri>();
                Data.CMSGTRA.Residenza residenza = AreaPrelievo.FinalResponse.ListaResidenza[0];

                if (residenza != null && residenza.LISTTRHONERE != null && residenza.LISTTRHONERE.Count > 0)
                {
                    foreach (Data.CMSGTRA.Residenza.TRHONERE on in residenza.LISTTRHONERE)
                    {
                        if (!string.IsNullOrEmpty(on.TRH_CODGRUP))
                        {
                            GestioneOneri.DatiOneri datiOneri = new GestioneOneri.DatiOneri();
                            List<GestioneDecodifica.GruppoOneri> elencoGruppoOneri = null;
                            GestioneDecodifica.GetGruppoOneri(out elencoGruppoOneri);
                            GestioneDecodifica.GruppoOneri gruppoOneri = elencoGruppoOneri.Find(x => x.Code == on.TRH_CODGRUP);
                            if (gruppoOneri == null)
                                continue;
                            datiOneri.IdCodeGruppo = gruppoOneri.Id;
                            List<GestioneDecodifica.SottoGruppoOneri> elencoSottoGruppoOneri = null;
                            GestioneDecodifica.GetSottoGruppoOneri(out elencoSottoGruppoOneri);
                            GestioneDecodifica.SottoGruppoOneri sottoGruppoOneri = elencoSottoGruppoOneri.Find(x => x.Code == on.TRH_CODSGRUP);
                            if (sottoGruppoOneri != null)
                                datiOneri.IdCodeSottoGruppo = sottoGruppoOneri.Id;
                            datiOneri.Decorrenza = Utility.DataFromString(on.TRH_DECONERE, Utility.FormatoData.AAAAmmGG);
                            datiOneri.Scadenza = Utility.DataFromString(on.TRH_SCADONERE, Utility.FormatoData.AAAAmmGG);
                            if (datiOneri.Scadenza == DateTime.MinValue)
                                datiOneri.Scadenza = null;

                            if (on.TRH_ONERE != 0M)
                                datiOneri.Onere = on.TRH_ONERE;
                            if (on.TRH_ANZCON != 0)
                                datiOneri.Settimane = Utility.StringToNullableShort(on.TRH_ANZCON.ToString());
                            if (on.TRH_CODGRUP == "5000" || on.TRH_CODGRUP == "5300" || on.TRH_CODGRUP == "5800" || on.TRH_CODGRUP == "6000" || on.TRH_CODGRUP == "6100")
                                datiOneri.ScadenzaBeneficio = Utility.DataFromString(residenza.TRH_CESINCUM + "01", Utility.FormatoData.AAAAmmGG);
                            ListaDatiOneri.Add(datiOneri);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiBeneficiParticolari(Data.FSPR AreaPrelievo, out List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaDatiBeneficiParticolari)
        {
            ListaDatiBeneficiParticolari = null;
            if (AreaPrelievo.FinalResponse.ListaResidenza != null && AreaPrelievo.FinalResponse.ListaResidenza.Count > 0)
            {
                ListaDatiBeneficiParticolari = new List<GestioneBeneficiParticolari.DatiBeneficiParticolari>();
                Data.CMSGTRA.Residenza residenza = AreaPrelievo.FinalResponse.ListaResidenza[0];

                if (residenza != null && residenza.LISTTRHONERE != null && residenza.LISTTRHONERE.Count > 0)
                {
                    foreach (Data.CMSGTRA.Residenza.TRHONERE on in residenza.LISTTRHONERE)
                    {
                        if (!string.IsNullOrEmpty(on.TRH_CODBENEF))
                        {
                            GestioneBeneficiParticolari.DatiBeneficiParticolari datiBeneficiParticolari = new GestioneBeneficiParticolari.DatiBeneficiParticolari();
                            datiBeneficiParticolari.CodiceBenefici = on.TRH_CODBENEF;
                            datiBeneficiParticolari.Settimane = Utility.StringToNullableShort(on.TRH_ANZBENEF.ToString());

                            ListaDatiBeneficiParticolari.Add(datiBeneficiParticolari);
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiServizioUtile(Data.FSPR AreaPrelievo, out List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile)
        {
            listaDatiServizioUtile = null;
            if (AreaPrelievo.FinalResponse.ListaFondoET != null && AreaPrelievo.FinalResponse.ListaFondoET.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.ET fondoET = AreaPrelievo.FinalResponse.ListaFondoET[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoET.XETUTIAA != 0 || fondoET.XETUTIMM != 0 || fondoET.XETUTIGG != 0 || fondoET.XETRETPN != 0M)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.Quota = "A";
                    datiServizioUtile.ServizioUtileAA = fondoET.XETUTIAA;
                    datiServizioUtile.ServizioUtileMM = fondoET.XETUTIMM;
                    datiServizioUtile.ServizioUtileGG = fondoET.XETUTIGG;
                    datiServizioUtile.RetribuzionePensionabile = fondoET.XETRETPN;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoET.XETPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

                if (fondoET.XETUT2AA != 0 || fondoET.XETUT2MM != 0 || fondoET.XETUT2GG != 0 || fondoET.XETRE2PN != 0M)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.Quota = "B";
                    datiServizioUtile.ServizioUtileAA = fondoET.XETUT2AA;
                    datiServizioUtile.ServizioUtileMM = fondoET.XETUT2MM;
                    datiServizioUtile.ServizioUtileGG = fondoET.XETUT2GG;
                    datiServizioUtile.RetribuzionePensionabile = fondoET.XETRE2PN;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoET.XETPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

                if (fondoET.XETUT3AA != 0 || fondoET.XETUT3MM != 0 || fondoET.XETUT3GG != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.Quota = "C";
                    datiServizioUtile.ServizioUtileAA = fondoET.XETUT3AA;
                    datiServizioUtile.ServizioUtileMM = fondoET.XETUT3MM;
                    datiServizioUtile.ServizioUtileGG = fondoET.XETUT3GG;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoET.XETPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoPT_New != null && AreaPrelievo.FinalResponse.ListaFondoPT_New.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (Data.CMSGTRA.Fondo.PT_New fondoPT in AreaPrelievo.FinalResponse.ListaFondoPT_New)
                {
                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                    int giorni;
                    int mesi;
                    int anni;
                    if (fondoPT.XFSIIS != 0M || fondoPT.XFSRETR != 0M || fondoPT.XFSQA14 != 0M || fondoPT.XFSSU92 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "A";
                        if (fondoPT.XFSSU92 != 0)
                        {
                            giorni = fondoPT.XFSSU92 % 30;
                            mesi = (fondoPT.XFSSU92 / 30) % 12;
                            anni = fondoPT.XFSSU92 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        datiServizioUtile.Retribuzione = fondoPT.XFSRETR;
                        datiServizioUtile.ImportoIndennitaIntegrativaSpeciale = fondoPT.XFSIIS;
                        datiServizioUtile.QuoteArt14 = fondoPT.XFSQA14;
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSRETRM != 0M || fondoPT.XFSSU94 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B1";
                        datiServizioUtile.Retribuzione = fondoPT.XFSRETRM;
                        if (fondoPT.XFSSU94 != 0)
                        {
                            giorni = fondoPT.XFSSU94 % 30;
                            mesi = (fondoPT.XFSSU94 / 30) % 12;
                            anni = fondoPT.XFSSU94 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSSU95 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B2";
                        giorni = fondoPT.XFSSU95 % 30;
                        mesi = (fondoPT.XFSSU95 / 30) % 12;
                        anni = fondoPT.XFSSU95 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSSU97 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B3";
                        giorni = fondoPT.XFSSU97 % 30;
                        mesi = (fondoPT.XFSSU97 / 30) % 12;
                        anni = fondoPT.XFSSU97 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSSUCE != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B4";
                        giorni = fondoPT.XFSSUCE % 30;
                        mesi = (fondoPT.XFSSUCE / 30) % 12;
                        anni = fondoPT.XFSSUCE / 30 / 12;
                        datiServizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }
                }
            }
            else if (AreaPrelievo.UtilizzaNuovoTracciato && AreaPrelievo.FinalResponse.ListaFondoFS_New != null && AreaPrelievo.FinalResponse.ListaFondoFS_New.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (Data.CMSGTRA.Fondo.FS_New fondoFS in AreaPrelievo.FinalResponse.ListaFondoFS_New)
                {
                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                    int giorni;
                    int mesi;
                    int anni;
                    if (fondoFS.XFSIIS != 0M || fondoFS.XFSRETR != 0M || fondoFS.XFSQA14 != 0M || fondoFS.XFSSU92 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "A";
                        if (fondoFS.XFSSU92 != 0)
                        {
                            giorni = fondoFS.XFSSU92 % 30;
                            mesi = (fondoFS.XFSSU92 / 30) % 12;
                            anni = fondoFS.XFSSU92 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        datiServizioUtile.Retribuzione = fondoFS.XFSRETR;
                        datiServizioUtile.ImportoIndennitaIntegrativaSpeciale = fondoFS.XFSIIS;
                        datiServizioUtile.QuoteArt14 = fondoFS.XFSQA14;
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSRETRM != 0M || fondoFS.XFSSU94 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B1";
                        datiServizioUtile.Retribuzione = fondoFS.XFSRETRM;
                        if (fondoFS.XFSSU94 != 0)
                        {
                            giorni = fondoFS.XFSSU94 % 30;
                            mesi = (fondoFS.XFSSU94 / 30) % 12;
                            anni = fondoFS.XFSSU94 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSSU95 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B2";
                        giorni = fondoFS.XFSSU95 % 30;
                        mesi = (fondoFS.XFSSU95 / 30) % 12;
                        anni = fondoFS.XFSSU95 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSSU97 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B3";
                        giorni = fondoFS.XFSSU97 % 30;
                        mesi = (fondoFS.XFSSU97 / 30) % 12;
                        anni = fondoFS.XFSSU97 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSSUCE != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B4";
                        giorni = fondoFS.XFSSUCE % 30;
                        mesi = (fondoFS.XFSSUCE / 30) % 12;
                        anni = fondoFS.XFSSUCE / 30 / 12;
                        datiServizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPT != null && AreaPrelievo.FinalResponse.ListaFondoPT.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (Data.CMSGTRA.Fondo.PT fondoPT in AreaPrelievo.FinalResponse.ListaFondoPT)
                {
                    //Data.CMSGTRA.Fondo.PT fondoPT = AreaPrelievo.FinalResponse.ListaFondoPT[0];

                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                    int giorni;
                    int mesi;
                    int anni;
                    if (fondoPT.XFSIIS != 0M || fondoPT.XFSRETR != 0M || fondoPT.XFSQA14 != 0M || fondoPT.XFSSU92 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "A";
                        if (fondoPT.XFSSU92 != 0)
                        {
                            giorni = fondoPT.XFSSU92 % 30;
                            mesi = (fondoPT.XFSSU92 / 30) % 12;
                            anni = fondoPT.XFSSU92 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        datiServizioUtile.Retribuzione = fondoPT.XFSRETR;
                        datiServizioUtile.ImportoIndennitaIntegrativaSpeciale = fondoPT.XFSIIS;
                        datiServizioUtile.QuoteArt14 = fondoPT.XFSQA14;
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSRETRM != 0M || fondoPT.XFSSU94 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B1";
                        datiServizioUtile.Retribuzione = fondoPT.XFSRETRM;
                        if (fondoPT.XFSSU94 != 0)
                        {
                            giorni = fondoPT.XFSSU94 % 30;
                            mesi = (fondoPT.XFSSU94 / 30) % 12;
                            anni = fondoPT.XFSSU94 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSSU95 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B2";
                        giorni = fondoPT.XFSSU95 % 30;
                        mesi = (fondoPT.XFSSU95 / 30) % 12;
                        anni = fondoPT.XFSSU95 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSSU97 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B3";
                        giorni = fondoPT.XFSSU97 % 30;
                        mesi = (fondoPT.XFSSU97 / 30) % 12;
                        anni = fondoPT.XFSSU97 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoPT.XFSSUCE != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoPT.XFSPROGR;
                        datiServizioUtile.Quota = "B4";
                        giorni = fondoPT.XFSSUCE % 30;
                        mesi = (fondoPT.XFSSUCE / 30) % 12;
                        anni = fondoPT.XFSSUCE / 30 / 12;
                        datiServizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoFS != null && AreaPrelievo.FinalResponse.ListaFondoFS.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (Data.CMSGTRA.Fondo.FS fondoFS in AreaPrelievo.FinalResponse.ListaFondoFS)
                {
                    //Data.CMSGTRA.Fondo.FS fondoFS = AreaPrelievo.FinalResponse.ListaFondoFS[0];

                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                    int giorni;
                    int mesi;
                    int anni;
                    if (fondoFS.XFSIIS != 0M || fondoFS.XFSRETR != 0M || fondoFS.XFSQA14 != 0M || fondoFS.XFSSU92 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "A";
                        if (fondoFS.XFSSU92 != 0)
                        {
                            giorni = fondoFS.XFSSU92 % 30;
                            mesi = (fondoFS.XFSSU92 / 30) % 12;
                            anni = fondoFS.XFSSU92 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        datiServizioUtile.Retribuzione = fondoFS.XFSRETR;
                        datiServizioUtile.ImportoIndennitaIntegrativaSpeciale = fondoFS.XFSIIS;
                        datiServizioUtile.QuoteArt14 = fondoFS.XFSQA14;
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSRETRM != 0M || fondoFS.XFSSU94 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B1";
                        datiServizioUtile.Retribuzione = fondoFS.XFSRETRM;
                        if (fondoFS.XFSSU94 != 0)
                        {
                            giorni = fondoFS.XFSSU94 % 30;
                            mesi = (fondoFS.XFSSU94 / 30) % 12;
                            anni = fondoFS.XFSSU94 / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSSU95 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B2";
                        giorni = fondoFS.XFSSU95 % 30;
                        mesi = (fondoFS.XFSSU95 / 30) % 12;
                        anni = fondoFS.XFSSU95 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSSU97 != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B3";
                        giorni = fondoFS.XFSSU97 % 30;
                        mesi = (fondoFS.XFSSU97 / 30) % 12;
                        anni = fondoFS.XFSSU97 / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoFS.XFSSUCE != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoFS.XFSPROGR;
                        datiServizioUtile.Quota = "B4";
                        giorni = fondoFS.XFSSUCE % 30;
                        mesi = (fondoFS.XFSSUCE / 30) % 12;
                        anni = fondoFS.XFSSUCE / 30 / 12;
                        datiServizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoPI != null && AreaPrelievo.FinalResponse.ListaFondoPI.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.PI fondoPI = AreaPrelievo.FinalResponse.ListaFondoPI[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoPI.XPISERVA != 0 || fondoPI.XPISERVM != 0 || fondoPI.XPISERVG != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoPI.XPISERVA;
                    datiServizioUtile.ServizioUtileMM = fondoPI.XPISERVM;
                    datiServizioUtile.ServizioUtileGG = fondoPI.XPISERVG;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoPI.XPIPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

            }
            else if (AreaPrelievo.FinalResponse.ListaFondoGAS != null && AreaPrelievo.FinalResponse.ListaFondoGAS.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.GAS fondoGAS = AreaPrelievo.FinalResponse.ListaFondoGAS[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoGAS.XGAUTIAA != 0 || fondoGAS.XGAUTIMM != 0 || fondoGAS.XGARETPN != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoGAS.XGAUTIAA;
                    datiServizioUtile.ServizioUtileMM = fondoGAS.XGAUTIMM;
                    datiServizioUtile.RetribuzionePensionabile = fondoGAS.XGARETPN;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoGAS.XGAPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

            }
            else if (AreaPrelievo.FinalResponse.ListaFondoDZ != null && AreaPrelievo.FinalResponse.ListaFondoDZ.Count > 0)
            {//Nuova Gestione Dazi Daniele
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                //Data.CMSGTRA.Fondo.DZ fondoDZ = AreaPrelievo.FinalResponse.ListaFondoDZ[0];
                foreach (Data.CMSGTRA.Fondo.DZ fondoDZ in AreaPrelievo.FinalResponse.ListaFondoDZ)
                {
                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                    if (fondoDZ.XDZUTIAA != 0 || fondoDZ.XDZUTIMM != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.Quota = "A";
                        datiServizioUtile.ServizioUtileAA = fondoDZ.XDZUTIAA;
                        datiServizioUtile.ServizioUtileMM = fondoDZ.XDZUTIMM;

                        datiServizioUtile.RetribuzionePensionabile = fondoDZ.XDZRETRI;
                        //necessario per il salvataggio dei DatiServizioUtile 
                        datiServizioUtile.IdRecordFondo = fondoDZ.XDZPROGR;
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }

                    if (fondoDZ.XDZUT2AA != 0 || fondoDZ.XDZUT2MM != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        datiServizioUtile.Quota = "B";
                        datiServizioUtile.ServizioUtileAA = fondoDZ.XDZUT2AA;
                        datiServizioUtile.ServizioUtileMM = fondoDZ.XDZUT2MM;

                        datiServizioUtile.RetribuzionePensionabile = fondoDZ.XDZRETR2;
                        //necessario per il salvataggio dei DatiServizioUtile 
                        datiServizioUtile.IdRecordFondo = fondoDZ.XDZPROGR;
                        listaDatiServizioUtile.Add(datiServizioUtile);
                    }
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoCL != null && AreaPrelievo.FinalResponse.ListaFondoCL.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.CL fondoCL = AreaPrelievo.FinalResponse.ListaFondoCL[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoCL.XCLUTIAA != 0 || fondoCL.XCLUTIMM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoCL.XCLUTIAA;
                    datiServizioUtile.ServizioUtileMM = fondoCL.XCLUTIMM;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoCL.XCLPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

            }
            else if (AreaPrelievo.FinalResponse.ListaFondoES != null && AreaPrelievo.FinalResponse.ListaFondoES.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.ES fondoES = AreaPrelievo.FinalResponse.ListaFondoES[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoES.XESUTIAA != 0 || fondoES.XESUTIMM != 0 || fondoES.XESRETPN != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoES.XESUTIAA;
                    datiServizioUtile.ServizioUtileMM = fondoES.XESUTIMM;
                    datiServizioUtile.RetribuzionePensionabile = fondoES.XESRETPN;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoES.XESPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

            }
            else if (AreaPrelievo.FinalResponse.ListaFondoEL != null && AreaPrelievo.FinalResponse.ListaFondoEL.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.EL fondoEL = AreaPrelievo.FinalResponse.ListaFondoEL[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoEL.XELRETPN != 0 || fondoEL.XELUTIAA != 0 || fondoEL.XELUTIMM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoEL.XELUTIAA;
                    datiServizioUtile.ServizioUtileMM = fondoEL.XELUTIMM;
                    datiServizioUtile.RetribuzionePensionabile = fondoEL.XELRETPN;
                    datiServizioUtile.Quota = "A";
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoEL.XELPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoEL.XELRE2PN != 0 || fondoEL.XELUT2AA != 0 || fondoEL.XELUT2MM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoEL.XELUT2AA;
                    datiServizioUtile.ServizioUtileMM = fondoEL.XELUT2MM;
                    datiServizioUtile.RetribuzionePensionabile = fondoEL.XELRE2PN;
                    datiServizioUtile.Quota = "B";
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoEL.XELPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoEL.XELUT3AA != 0 || fondoEL.XELUT3MM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoEL.XELUT3AA;
                    datiServizioUtile.ServizioUtileMM = fondoEL.XELUT3MM;
                    datiServizioUtile.Quota = "C";
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoEL.XELPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }

            }
            else if (AreaPrelievo.FinalResponse.ListaFondoVL != null && AreaPrelievo.FinalResponse.ListaFondoVL.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.VL fondoVL = AreaPrelievo.FinalResponse.ListaFondoVL[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoVL.XVLRETPN != 0M || fondoVL.XVLUT1AA != 0 || fondoVL.XVLUT1MM != 0 || fondoVL.XVLUT1GG != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoVL.XVLUT1AA;
                    datiServizioUtile.ServizioUtileMM = fondoVL.XVLUT1MM;
                    datiServizioUtile.ServizioUtileGG = fondoVL.XVLUT1GG;
                    datiServizioUtile.RetribuzionePensionabile = fondoVL.XVLRETPN;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoVL.XVLPROGR;
                    datiServizioUtile.Quota = "A";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoVL.XVLUTIAA != 0 || fondoVL.XVLUTIMM != 0 || fondoVL.XVLUTIGG != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoVL.XVLUTIAA;
                    datiServizioUtile.ServizioUtileMM = fondoVL.XVLUTIMM;
                    datiServizioUtile.ServizioUtileGG = fondoVL.XVLUTIGG;
                    datiServizioUtile.Quota = "A2";
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoVL.XVLPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoVL.XVLRE1PN != 0M || fondoVL.XVLUTBAA != 0 || fondoVL.XVLUTBMM != 0 || fondoVL.XVLUTBGG != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoVL.XVLUTBAA;
                    datiServizioUtile.ServizioUtileMM = fondoVL.XVLUTBMM;
                    datiServizioUtile.ServizioUtileGG = fondoVL.XVLUTBGG;
                    datiServizioUtile.RetribuzionePensionabile = fondoVL.XVLRE1PN;
                    datiServizioUtile.Quota = "B";
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoVL.XVLPROGR;
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoVL.XVLUTCAA != 0 || fondoVL.XVLUTCMM != 0 || fondoVL.XVLUTCGG != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoVL.XVLUTCAA;
                    datiServizioUtile.ServizioUtileMM = fondoVL.XVLUTCMM;
                    datiServizioUtile.ServizioUtileGG = fondoVL.XVLUTCGG;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoVL.XVLPROGR;
                    datiServizioUtile.Quota = "C";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
            }
            else if (AreaPrelievo.FinalResponse.ListaFondoTT != null && AreaPrelievo.FinalResponse.ListaFondoTT.Count > 0)
            {
                listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();
                Data.CMSGTRA.Fondo.TT fondoTT = AreaPrelievo.FinalResponse.ListaFondoTT[0];

                GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = null;
                if (fondoTT.XTTUTIAA != 0M || fondoTT.XTTUTIMM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUTIAA;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUTIMM;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "A";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoTT.XTTUTRAA != 0M || fondoTT.XTTUTRMM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUTRAA;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUTRMM;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "A2";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoTT.XTTRETPN != 0M || fondoTT.XTTUT2AA != 0 || fondoTT.XTTUT2MM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUT2AA;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUT2MM;
                    datiServizioUtile.RetribuzionePensionabile = fondoTT.XTTRETPN;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "B";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoTT.XTTUTR2A != 0 || fondoTT.XTTUTR2M != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUTR2A;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUTR2M;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "B2";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoTT.XTTUT3AA != 0 || fondoTT.XTTUT3MM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUT3AA;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUT3MM;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "C";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoTT.XTTUTR3A != 0 || fondoTT.XTTUTR3M != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUTR3A;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUTR3M;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "C2";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoTT.XTTRETPD != 0M || fondoTT.XTTUT4AA != 0 || fondoTT.XTTUT4MM != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUT4AA;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUT4MM;
                    datiServizioUtile.RetribuzionePensionabile = fondoTT.XTTRETPD;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "D";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
                if (fondoTT.XTTUTR4A != 0 || fondoTT.XTTUTR4M != 0)
                {
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    datiServizioUtile.ServizioUtileAA = fondoTT.XTTUTR4A;
                    datiServizioUtile.ServizioUtileMM = fondoTT.XTTUTR4M;
                    //necessario per il salvataggio dei DatiServizioUtile 
                    datiServizioUtile.IdRecordFondo = fondoTT.XTTPROGR;
                    datiServizioUtile.Quota = "D2";
                    listaDatiServizioUtile.Add(datiServizioUtile);
                }
            }

            if (AreaPrelievo.UtilizzaNuovoTracciato)
            {
                if (AreaPrelievo.FinalResponse.ListaAgoPT != null && AreaPrelievo.FinalResponse.ListaAgoPT.Count > 0)
                {
                    foreach (Data.CMSGTRA.Ago.PT agoPT in AreaPrelievo.FinalResponse.ListaAgoPT)
                    {
                        if (listaDatiServizioUtile == null)
                            listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();

                        if (agoPT.YFSQUOTA92 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "A"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoPT.YFSPROGR, Quota = "A" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "A");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA92;
                        }

                        if (agoPT.YFSQUOTA94 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B1"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoPT.YFSPROGR, Quota = "B1" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B1");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA94;
                        }

                        if (agoPT.YFSQUOTA95 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B2"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoPT.YFSPROGR, Quota = "B2" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B2");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA95;
                        }

                        if (agoPT.YFSQUOTA97 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B3"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoPT.YFSPROGR, Quota = "B3" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B3");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA97;
                        }

                        if (agoPT.YFSQUOTACE != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B4"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoPT.YFSPROGR, Quota = "B4" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B4");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTACE;
                        }
                    }
                }
                else if (AreaPrelievo.FinalResponse.ListaAgoFS != null && AreaPrelievo.FinalResponse.ListaAgoFS.Count > 0)
                {
                    foreach (Data.CMSGTRA.Ago.FS agoFS in AreaPrelievo.FinalResponse.ListaAgoFS)
                    {
                        if (listaDatiServizioUtile == null)
                            listaDatiServizioUtile = new List<GestioneDatiServizioUtile.ServizioUtile>();

                        if (agoFS.YFSQUOTA92 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "A"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoFS.YFSPROGR, Quota = "A" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "A");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA92;
                        }

                        if (agoFS.YFSQUOTA94 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B1"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoFS.YFSPROGR, Quota = "B1" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B1");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA94;
                        }

                        if (agoFS.YFSQUOTA95 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B2"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoFS.YFSPROGR, Quota = "B2" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B2");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA95;
                        }

                        if (agoFS.YFSQUOTA97 != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B3"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoFS.YFSPROGR, Quota = "B3" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B3");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA97;
                        }

                        if (agoFS.YFSQUOTACE != 0M)
                        {
                            if (!listaDatiServizioUtile.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B4"))
                                listaDatiServizioUtile.Add(new GestioneDatiServizioUtile.ServizioUtile { IdRecordFondo = agoFS.YFSPROGR, Quota = "B4" });
                            GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = listaDatiServizioUtile.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B4");
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTACE;
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiServizioUtile707(Data.FSPR AreaPrelievo, out List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707)
        {
            listaDatiServizioUtile707 = null;
            if (AreaPrelievo.UtilizzaNuovoTracciato)
            {
                if (AreaPrelievo.FinalResponse.ListaAgoPT != null && AreaPrelievo.FinalResponse.ListaAgoPT.Count > 0)
                {
                    foreach (Data.CMSGTRA.Ago.PT agoPT in AreaPrelievo.FinalResponse.ListaAgoPT)
                    {
                        listaDatiServizioUtile707 = new List<GestioneCalcolo.ServizioUtile707>();

                        if (agoPT.YFSSU92_707 != 0 || agoPT.YFSQUOTA92_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "A"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoPT.YFSPROGR, Quota = "A" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "A");
                            if (agoPT.YFSSU92_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoPT.YFSSU92_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoPT.YFSQUOTA92_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA92_707;
                        }

                        if (agoPT.YFSSU94_707 != 0 || agoPT.YFSQUOTA94_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B1"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoPT.YFSPROGR, Quota = "B1" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B1");
                            if (agoPT.YFSSU94_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoPT.YFSSU94_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoPT.YFSQUOTA94_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA94_707;
                        }

                        if (agoPT.YFSSU95_707 != 0 || agoPT.YFSQUOTA95_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B2"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoPT.YFSPROGR, Quota = "B2" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B2");
                            if (agoPT.YFSSU95_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoPT.YFSSU95_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoPT.YFSQUOTA95_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA95_707;
                        }

                        if (agoPT.YFSSU97_707 != 0 || agoPT.YFSQUOTA97_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B3"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoPT.YFSPROGR, Quota = "B3" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B3");
                            if (agoPT.YFSSU97_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoPT.YFSSU97_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoPT.YFSQUOTA97_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTA97_707;
                        }

                        if (agoPT.YFSSUCE_707 != 0 || agoPT.YFSQUOTACE_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B4"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoPT.YFSPROGR, Quota = "B4" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoPT.YFSPROGR && x.Quota == "B4");
                            if (agoPT.YFSSUCE_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoPT.YFSSUCE_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileCessazioneAA = anni;
                                datiServizioUtile.ServizioUtileCessazioneMM = mesi;
                                datiServizioUtile.ServizioUtileCessazioneGG = giorni;
                            }
                            if (agoPT.YFSQUOTACE_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoPT.YFSQUOTACE_707;
                        }
                    }
                }
                else if (AreaPrelievo.FinalResponse.ListaAgoFS != null && AreaPrelievo.FinalResponse.ListaAgoFS.Count > 0)
                {
                    foreach (Data.CMSGTRA.Ago.FS agoFS in AreaPrelievo.FinalResponse.ListaAgoFS)
                    {
                        listaDatiServizioUtile707 = new List<GestioneCalcolo.ServizioUtile707>();

                        if (agoFS.YFSSU92_707 != 0 || agoFS.YFSQUOTA92_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "A"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoFS.YFSPROGR, Quota = "A" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "A");
                            if (agoFS.YFSSU92_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoFS.YFSSU92_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoFS.YFSQUOTA92_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA92_707;
                        }

                        if (agoFS.YFSSU94_707 != 0 || agoFS.YFSQUOTA94_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B1"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoFS.YFSPROGR, Quota = "B1" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B1");
                            if (agoFS.YFSSU94_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoFS.YFSSU94_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoFS.YFSQUOTA94_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA94_707;
                        }

                        if (agoFS.YFSSU95_707 != 0 || agoFS.YFSQUOTA95_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B2"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoFS.YFSPROGR, Quota = "B2" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B2");
                            if (agoFS.YFSSU95_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoFS.YFSSU95_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoFS.YFSQUOTA95_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA95_707;
                        }

                        if (agoFS.YFSSU97_707 != 0 || agoFS.YFSQUOTA97_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B3"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoFS.YFSPROGR, Quota = "B3" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B3");
                            if (agoFS.YFSSU97_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoFS.YFSSU97_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileAA = anni;
                                datiServizioUtile.ServizioUtileMM = mesi;
                                datiServizioUtile.ServizioUtileGG = giorni;
                            }
                            if (agoFS.YFSQUOTA97_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTA97_707;
                        }

                        if (agoFS.YFSSUCE_707 != 0 || agoFS.YFSQUOTACE_707 != 0M)
                        {
                            if (!listaDatiServizioUtile707.Exists(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B4"))
                                listaDatiServizioUtile707.Add(new GestioneCalcolo.ServizioUtile707 { IdRecordFondo = agoFS.YFSPROGR, Quota = "B4" });
                            GestioneCalcolo.ServizioUtile707 datiServizioUtile = listaDatiServizioUtile707.FirstOrDefault(x => x.IdRecordFondo == agoFS.YFSPROGR && x.Quota == "B4");
                            if (agoFS.YFSSUCE_707 != 0)
                            {
                                short anni = 0;
                                short mesi = 0;
                                short giorni = 0;
                                Utility.GetAAMMGGFromSettimane(agoFS.YFSSUCE_707, out anni, out mesi, out giorni);
                                datiServizioUtile.ServizioUtileCessazioneAA = anni;
                                datiServizioUtile.ServizioUtileCessazioneMM = mesi;
                                datiServizioUtile.ServizioUtileCessazioneGG = giorni;
                            }
                            if (agoFS.YFSQUOTACE_707 != 0M)
                                datiServizioUtile.QuotaPensioneRetributivaAnnua = agoFS.YFSQUOTACE_707;
                        }
                    }
                }
            }
        }

        public static void ValorizzaDatiServizioUtileINPDAP(Data.FSPR AreaPrelievo, out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP)
        {
            listaDatiServizioUtileINPDAP = null;
            if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0)
            {
                listaDatiServizioUtileINPDAP = new List<GestioneDatiServizioUtileINPDAP.ServizioUtile>();
                foreach (Data.CMSGTRA.Fondo.GDP fondoGDP in AreaPrelievo.FinalResponse.ListaFondoGDP)
                {
                    GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = null;
                    int giorni;
                    int mesi;
                    int anni;
                    if (fondoGDP.QA14_GDP_EURO != 0M || fondoGDP.RETR_GDP_EURO != 0M || fondoGDP.SU92_GDP != 0M || fondoGDP.IIS_GDP_EURO != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoGDP.PROGR_GDP;
                        datiServizioUtile.Quota = "A";
                        if (fondoGDP.SU92_GDP != 0)
                        {
                            giorni = fondoGDP.SU92_GDP % 30;
                            mesi = (fondoGDP.SU92_GDP / 30) % 12;
                            anni = fondoGDP.SU92_GDP / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        datiServizioUtile.Retribuzione = fondoGDP.RETR_GDP_EURO;
                        datiServizioUtile.ImportoIndennitaIntegrativaSpeciale = fondoGDP.IIS_GDP_EURO;
                        datiServizioUtile.QuoteArt14 = fondoGDP.QA14_GDP_EURO;
                        listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                    }

                    if (fondoGDP.RETRM_GDP_EURO != 0M || fondoGDP.SU94_GDP != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoGDP.PROGR_GDP;
                        datiServizioUtile.Quota = "B1";
                        datiServizioUtile.Retribuzione = fondoGDP.RETRM_GDP_EURO;
                        if (fondoGDP.SU94_GDP != 0)
                        {
                            giorni = fondoGDP.SU94_GDP % 30;
                            mesi = (fondoGDP.SU94_GDP / 30) % 12;
                            anni = fondoGDP.SU94_GDP / 30 / 12;
                            datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                            datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                            datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        }
                        listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                    }

                    if (fondoGDP.SU95_GDP != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoGDP.PROGR_GDP;
                        datiServizioUtile.Quota = "B2";
                        giorni = fondoGDP.SU95_GDP % 30;
                        mesi = (fondoGDP.SU95_GDP / 30) % 12;
                        anni = fondoGDP.SU95_GDP / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                    }

                    if (fondoGDP.SU97_GDP != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoGDP.PROGR_GDP;
                        datiServizioUtile.Quota = "B3";
                        giorni = fondoGDP.SU97_GDP % 30;
                        mesi = (fondoGDP.SU97_GDP / 30) % 12;
                        anni = fondoGDP.SU97_GDP / 30 / 12;
                        datiServizioUtile.ServizioUtileAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                    }

                    if (fondoGDP.SUCE_GDP != 0)
                    {
                        datiServizioUtile = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                        datiServizioUtile.IdRecordFondo = fondoGDP.PROGR_GDP;
                        datiServizioUtile.Quota = "B4";
                        giorni = fondoGDP.SUCE_GDP % 30;
                        mesi = (fondoGDP.SUCE_GDP / 30) % 12;
                        anni = fondoGDP.SUCE_GDP / 30 / 12;
                        datiServizioUtile.ServizioUtileCessazioneAA = Utility.StringToNullableShort(anni.ToString());
                        datiServizioUtile.ServizioUtileCessazioneMM = Utility.StringToNullableShort(mesi.ToString());
                        datiServizioUtile.ServizioUtileCessazioneGG = Utility.StringToNullableShort(giorni.ToString());
                        listaDatiServizioUtileINPDAP.Add(datiServizioUtile);
                    }
                }
            }
            if (AreaPrelievo.FinalResponse.ListaAgoGDP != null && AreaPrelievo.FinalResponse.ListaAgoGDP.Count > 0)
            {
                foreach (Data.CMSGTRA.Ago.GDP agoGDP in AreaPrelievo.FinalResponse.ListaAgoGDP)
                {
                    if (listaDatiServizioUtileINPDAP == null)
                        listaDatiServizioUtileINPDAP = new List<GestioneDatiServizioUtileINPDAP.ServizioUtile>();

                    if (agoGDP.YFSQUOTA92 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "A"))
                            listaDatiServizioUtileINPDAP.Add(new GestioneDatiServizioUtileINPDAP.ServizioUtile { IdRecordFondo = agoGDP.YFSPROGR, Quota = "A" });
                        GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "A");
                        datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA92;
                    }

                    if (agoGDP.YFSQUOTA94 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B1"))
                            listaDatiServizioUtileINPDAP.Add(new GestioneDatiServizioUtileINPDAP.ServizioUtile { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B1" });
                        GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B1");
                        datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA94;
                    }

                    if (agoGDP.YFSQUOTA95 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B2"))
                            listaDatiServizioUtileINPDAP.Add(new GestioneDatiServizioUtileINPDAP.ServizioUtile { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B2" });
                        GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B2");
                        datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA95;
                    }

                    if (agoGDP.YFSQUOTA97 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B3"))
                            listaDatiServizioUtileINPDAP.Add(new GestioneDatiServizioUtileINPDAP.ServizioUtile { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B3" });
                        GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B3");
                        datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA97;
                    }

                    if (agoGDP.YFSQUOTACE != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B4"))
                            listaDatiServizioUtileINPDAP.Add(new GestioneDatiServizioUtileINPDAP.ServizioUtile { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B4" });
                        GestioneDatiServizioUtileINPDAP.ServizioUtile datiServizioUtile = listaDatiServizioUtileINPDAP.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B4");
                        datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTACE;
                    }
                }
            }
        }

        public static void ValorizzaDatiServizioUtileINPDAP707(Data.FSPR AreaPrelievo, out List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtileINPDAP707)
        {
            listaDatiServizioUtileINPDAP707 = null;
            if (AreaPrelievo.FinalResponse.ListaAgoGDP != null && AreaPrelievo.FinalResponse.ListaAgoGDP.Count > 0)
            {
                foreach (Data.CMSGTRA.Ago.GDP agoGDP in AreaPrelievo.FinalResponse.ListaAgoGDP)
                {
                    listaDatiServizioUtileINPDAP707 = new List<GestioneCalcolo.ServizioUtileINPDAP707>();

                    if (agoGDP.YFSSU92_707 != 0 || agoGDP.YFSQUOTA92_707 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP707.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "A"))
                            listaDatiServizioUtileINPDAP707.Add(new GestioneCalcolo.ServizioUtileINPDAP707 { IdRecordFondo = agoGDP.YFSPROGR, Quota = "A" });
                        GestioneCalcolo.ServizioUtileINPDAP707 datiServizioUtile = listaDatiServizioUtileINPDAP707.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "A");
                        if (agoGDP.YFSSU92_707 != 0)
                        {
                            short anni = 0;
                            short mesi = 0;
                            short giorni = 0;
                            Utility.GetAAMMGGFromSettimane(agoGDP.YFSSU92_707, out anni, out mesi, out giorni);
                            datiServizioUtile.ServizioUtileAA = anni;
                            datiServizioUtile.ServizioUtileMM = mesi;
                            datiServizioUtile.ServizioUtileGG = giorni;
                        }
                        if (agoGDP.YFSQUOTA92_707 != 0M)
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA92_707;
                    }

                    if (agoGDP.YFSSU94_707 != 0 || agoGDP.YFSQUOTA94_707 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP707.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B1"))
                            listaDatiServizioUtileINPDAP707.Add(new GestioneCalcolo.ServizioUtileINPDAP707 { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B1" });
                        GestioneCalcolo.ServizioUtileINPDAP707 datiServizioUtile = listaDatiServizioUtileINPDAP707.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B1");
                        if (agoGDP.YFSSU94_707 != 0)
                        {
                            short anni = 0;
                            short mesi = 0;
                            short giorni = 0;
                            Utility.GetAAMMGGFromSettimane(agoGDP.YFSSU94_707, out anni, out mesi, out giorni);
                            datiServizioUtile.ServizioUtileAA = anni;
                            datiServizioUtile.ServizioUtileMM = mesi;
                            datiServizioUtile.ServizioUtileGG = giorni;
                        }
                        if (agoGDP.YFSQUOTA94_707 != 0M)
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA94_707;
                    }

                    if (agoGDP.YFSSU95_707 != 0 || agoGDP.YFSQUOTA95_707 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP707.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B2"))
                            listaDatiServizioUtileINPDAP707.Add(new GestioneCalcolo.ServizioUtileINPDAP707 { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B2" });
                        GestioneCalcolo.ServizioUtileINPDAP707 datiServizioUtile = listaDatiServizioUtileINPDAP707.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B2");
                        if (agoGDP.YFSSU95_707 != 0)
                        {
                            short anni = 0;
                            short mesi = 0;
                            short giorni = 0;
                            Utility.GetAAMMGGFromSettimane(agoGDP.YFSSU95_707, out anni, out mesi, out giorni);
                            datiServizioUtile.ServizioUtileAA = anni;
                            datiServizioUtile.ServizioUtileMM = mesi;
                            datiServizioUtile.ServizioUtileGG = giorni;
                        }
                        if (agoGDP.YFSQUOTA95_707 != 0M)
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA95_707;
                    }

                    if (agoGDP.YFSSU97_707 != 0 || agoGDP.YFSQUOTA97_707 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP707.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B3"))
                            listaDatiServizioUtileINPDAP707.Add(new GestioneCalcolo.ServizioUtileINPDAP707 { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B3" });
                        GestioneCalcolo.ServizioUtileINPDAP707 datiServizioUtile = listaDatiServizioUtileINPDAP707.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B3");
                        if (agoGDP.YFSSU97_707 != 0)
                        {
                            short anni = 0;
                            short mesi = 0;
                            short giorni = 0;
                            Utility.GetAAMMGGFromSettimane(agoGDP.YFSSU97_707, out anni, out mesi, out giorni);
                            datiServizioUtile.ServizioUtileAA = anni;
                            datiServizioUtile.ServizioUtileMM = mesi;
                            datiServizioUtile.ServizioUtileGG = giorni;
                        }
                        if (agoGDP.YFSQUOTA97_707 != 0M)
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTA97_707;
                    }

                    if (agoGDP.YFSSUCE_707 != 0 || agoGDP.YFSQUOTACE_707 != 0M)
                    {
                        if (!listaDatiServizioUtileINPDAP707.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B4"))
                            listaDatiServizioUtileINPDAP707.Add(new GestioneCalcolo.ServizioUtileINPDAP707 { IdRecordFondo = agoGDP.YFSPROGR, Quota = "B4" });
                        GestioneCalcolo.ServizioUtileINPDAP707 datiServizioUtile = listaDatiServizioUtileINPDAP707.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR && x.Quota == "B4");
                        if (agoGDP.YFSSUCE_707 != 0)
                        {
                            short anni = 0;
                            short mesi = 0;
                            short giorni = 0;
                            Utility.GetAAMMGGFromSettimane(agoGDP.YFSSUCE_707, out anni, out mesi, out giorni);
                            datiServizioUtile.ServizioUtileCessazioneAA = anni;
                            datiServizioUtile.ServizioUtileCessazioneMM = mesi;
                            datiServizioUtile.ServizioUtileCessazioneGG = giorni;
                        }
                        if (agoGDP.YFSQUOTACE_707 != 0M)
                            datiServizioUtile.QuotaPensioneRetributivaAnnua = agoGDP.YFSQUOTACE_707;
                    }
                }
            }
        }

        public static void ValorizzaDatiNonCalcolo(Data.FSPR AreaPrelievo, Dictionary<string, string> componentiFamiliari, out List<Entity.DatiNoCalcolo> listaDatiNoCalcolo)
        {
            listaDatiNoCalcolo = null;

            if (AreaPrelievo.FinalResponse.ListaDatiNonCalcolo != null && AreaPrelievo.FinalResponse.ListaDatiNonCalcolo.Count > 0)
            {
                listaDatiNoCalcolo = new List<Entity.DatiNoCalcolo>();

                foreach (Data.CMSGTRA.DatiNonCalcolo recordDatiNoCalcolo in AreaPrelievo.FinalResponse.ListaDatiNonCalcolo)
                {
                    Entity.DatiNoCalcolo datiNoCalcolo = new Entity.DatiNoCalcolo();

                    if (recordDatiNoCalcolo.TRWDECAA != 0 && recordDatiNoCalcolo.TRWDECMM != 0 && recordDatiNoCalcolo.TRWDECGG != 0)
                        datiNoCalcolo.Decorrenza = recordDatiNoCalcolo.TRWDECGG.ToString() + "/" + recordDatiNoCalcolo.TRWDECMM.ToString() + "/" + recordDatiNoCalcolo.TRWDECAA.ToString();
                    if (recordDatiNoCalcolo.TRWCOL03 != 0M)
                        datiNoCalcolo.AdeguataAgo = recordDatiNoCalcolo.TRWCOL03;
                    if (recordDatiNoCalcolo.TRWCOL04 != 0M)
                        datiNoCalcolo.AdeguataFondo = recordDatiNoCalcolo.TRWCOL04;
                    if (recordDatiNoCalcolo.TRWCOL05 != 0M)
                        datiNoCalcolo.EccedenzaAgo = recordDatiNoCalcolo.TRWCOL05;
                    if (recordDatiNoCalcolo.TRWCOL06 != 0M)
                        datiNoCalcolo.QuotaAgoEsclusiva = recordDatiNoCalcolo.TRWCOL06;
                    if (recordDatiNoCalcolo.TRWCOL07 != 0M)
                        datiNoCalcolo.FacArt14 = recordDatiNoCalcolo.TRWCOL07;
                    if (recordDatiNoCalcolo.TRWCOL08 != 0M)
                        datiNoCalcolo.IndIntSpeciale = recordDatiNoCalcolo.TRWCOL08;
                    if (recordDatiNoCalcolo.TRWCOL09 != 0M)
                        datiNoCalcolo.AssegniFamiliari = recordDatiNoCalcolo.TRWCOL09;
                    if (recordDatiNoCalcolo.TRWCOL10 != 0M)
                        datiNoCalcolo.AggFamigliaFondo = recordDatiNoCalcolo.TRWCOL10;
                    if (recordDatiNoCalcolo.TRWCOL11 != 0M)
                        datiNoCalcolo.OnereCaricoAmm = recordDatiNoCalcolo.TRWCOL11;
                    if (recordDatiNoCalcolo.TRWCOL12 != 0M)
                        datiNoCalcolo.Art21 = recordDatiNoCalcolo.TRWCOL12;
                    if (recordDatiNoCalcolo.TRWCOL13 != 0M)
                        datiNoCalcolo.ImportoMensile = recordDatiNoCalcolo.TRWCOL13;
                    if (recordDatiNoCalcolo.TRWCOL14 != 0M)
                        datiNoCalcolo.Tredicesima = recordDatiNoCalcolo.TRWCOL14;

                    if (componentiFamiliari != null)
                    {
                        datiNoCalcolo.ListaComponentiFamiliari = new List<Entity.DatiNoCalcolo.ComponentiFamiliari>();
                        for (int i = 1; i < 16; i++)
                        {
                            Entity.DatiNoCalcolo.ComponentiFamiliari componente = new Entity.DatiNoCalcolo.ComponentiFamiliari();
                            string prog = (string)Utility.GetValueByNameProperty("TRWFAM" + i.ToString().PadLeft(2, '0'), recordDatiNoCalcolo);
                            if (!string.IsNullOrEmpty(prog))
                            {
                                componente.CodiceFiscale = componentiFamiliari.FirstOrDefault(x => x.Value == prog).Key;
                                datiNoCalcolo.ListaComponentiFamiliari.Add(componente);
                            }
                        }
                    }

                    if (!datiNoCalcolo.IsNull())
                        listaDatiNoCalcolo.Add(datiNoCalcolo);
                }
            }
        }

        public static void ValorizzaDatiPensioneINPDAP(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP)
        {
            listaDatiPensioneINPDAP = null;
            if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0)
            {
                listaDatiPensioneINPDAP = new List<GestionePensioneINPDAP.DatiPensioneINPDAP>();
                foreach (Data.CMSGTRA.Fondo.GDP fondoGDP in AreaPrelievo.FinalResponse.ListaFondoGDP)
                {
                    GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP = new GestionePensioneINPDAP.DatiPensioneINPDAP();
                    datiPensioneINPDAP.IdRecordFondo = fondoGDP.PROGR_GDP;

                    if (fondoGDP.CAUSA_GDP != 0)
                    {
                        string siglaCategoria = GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG);
                        List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                        GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                        GestioneDecodifica.DecodificaCausaCessazione causaCess = null;
                        if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                        {
                            string codNatura1 = !string.IsNullOrEmpty(fondoGDP.NATPENS1_GDP) ? fondoGDP.NATPENS1_GDP : " ";
                            var listaCodiceCausaCessazioneDB = ListaCausaCess.FindAll(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == fondoGDP.CAUSA_GDP.ToString().Trim().ToUpperInvariant() && x.Fondo == "DAP");
                            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("V") && (codNatura1 == "1" || codNatura1 == "2"))
                                causaCess = listaCodiceCausaCessazioneDB.Find(x => x.Fondo == "DAP" && x.TipoPensione == 'A');
                            else if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("V") && (codNatura1 == " " || codNatura1 == "6"))
                                causaCess = listaCodiceCausaCessazioneDB.Find(x => x.Fondo == "DAP" && x.TipoPensione == 'V');
                            else if ((tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("S")) || tipoDomanda == GestionePrelievo.TipoDomanda.Reversibilità)
                                causaCess = listaCodiceCausaCessazioneDB.Find(x => x.Fondo == "DAP" && x.TipoPensione == 'S');
                            else if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && siglaCategoria.StartsWith("I"))
                                causaCess = listaCodiceCausaCessazioneDB.Find(x => x.Fondo == "DAP" && x.TipoPensione == 'I');
                            else
                                causaCess = listaCodiceCausaCessazioneDB.Find(x => x.TraduzioneSuGP.Trim().ToUpperInvariant() == fondoGDP.CAUSA_GDP.ToString().Trim().ToUpperInvariant() && x.Fondo == "DAP");
                        }
                        datiPensioneINPDAP.CausaCessazione = causaCess != null ? causaCess.Id : (long?)null;
                    }

                    switch (fondoGDP.L537_ANNI_UT_GDP)
                    {
                        case 0:
                            datiPensioneINPDAP.RiduzioneL537 = null;
                            datiPensioneINPDAP.IISAbbattimentoAnni = null;
                            break;
                        case 1:
                            datiPensioneINPDAP.RiduzioneL537 = true;
                            datiPensioneINPDAP.IISAbbattimentoAnni = null;
                            break;
                        case 2:
                            datiPensioneINPDAP.RiduzioneL537 = null;
                            datiPensioneINPDAP.IISAbbattimentoAnni = true;
                            break;
                        case 3:
                            datiPensioneINPDAP.RiduzioneL537 = true;
                            datiPensioneINPDAP.IISAbbattimentoAnni = true;
                            break;
                    }

                    if (fondoGDP.DIIS_GDP == 2)
                        datiPensioneINPDAP.DirittoIndennitaIntegrativaSpeciale = true;
                    //**Revisione Campi INPDAP**
                    //if (fondoGDP.ANNI_MAX_GDP != 0)
                    //    datiPensioneINPDAP.AnniMax = (byte)fondoGDP.ANNI_MAX_GDP;

                    if (!string.IsNullOrEmpty(fondoGDP.PROF_GDP))
                    {
                        GestioneDecodifica.DecMicroqualificaINPDAP decMicroqualifica;
                        GestioneDecodifica.GetMicroqualificaByTraduzioneSuGP(fondoGDP.PROF_GDP.Trim().ToUpperInvariant(), out decMicroqualifica);
                        datiPensioneINPDAP.Microqualifica = decMicroqualifica != null ? decMicroqualifica.Id : (long?)null;
                    }



                    listaDatiPensioneINPDAP.Add(datiPensioneINPDAP);
                }
            }

            if (AreaPrelievo.FinalResponse.ListaAgoGDP != null && AreaPrelievo.FinalResponse.ListaAgoGDP.Count > 0)
            {
                if (listaDatiPensioneINPDAP == null)
                    listaDatiPensioneINPDAP = new List<GestionePensioneINPDAP.DatiPensioneINPDAP>();
                foreach (Data.CMSGTRA.Ago.GDP agoGDP in AreaPrelievo.FinalResponse.ListaAgoGDP)
                {
                    int comparto;
                    int.TryParse(agoGDP.YFSCOMPARTO, out comparto);
                    int settore;
                    int.TryParse(agoGDP.YFSSETTORE, out settore);
                    int ruolo;
                    int.TryParse(agoGDP.YFSRUOLO, out ruolo);

                    GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP = null;
                    if (listaDatiPensioneINPDAP.Count > 0)
                        datiPensioneINPDAP = listaDatiPensioneINPDAP.Find(x => x.IdRecordFondo == agoGDP.YFSPROGR);

                    if (datiPensioneINPDAP == null)
                    {
                        //aggiungi
                        datiPensioneINPDAP = new GestionePensioneINPDAP.DatiPensioneINPDAP();
                        datiPensioneINPDAP.IdRecordFondo = agoGDP.YFSPROGR;
                        datiPensioneINPDAP.Comparto = comparto > 0 ? comparto : (int?)null;
                        datiPensioneINPDAP.Settore = settore > 0 ? settore : (int?)null;
                        datiPensioneINPDAP.Ruolo = ruolo > 0 ? ruolo : (int?)null;
                        listaDatiPensioneINPDAP.Add(datiPensioneINPDAP);
                    }
                    else
                    {
                        datiPensioneINPDAP.Comparto = comparto > 0 ? comparto : (int?)null;
                        datiPensioneINPDAP.Settore = settore > 0 ? settore : (int?)null;
                        datiPensioneINPDAP.Ruolo = ruolo > 0 ? ruolo : (int?)null;
                    }
                }
            }
        }

        public static void ValorizzaRecordDatiFondoINPDAP(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, out List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP)
        {
            listaRecordDatiFondoINPDAP = null;
            if (AreaPrelievo.FinalResponse.ListaFondoGDP != null && AreaPrelievo.FinalResponse.ListaFondoGDP.Count > 0)
            {
                listaRecordDatiFondoINPDAP = new List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP>();
                foreach (Data.CMSGTRA.Fondo.GDP fondoGDP in AreaPrelievo.FinalResponse.ListaFondoGDP)
                {
                    GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                    recordDatiFondoINPDAP.IdRecordFondo = fondoGDP.PROGR_GDP;

                    switch (fondoGDP.IIS_CONG_DIR_MIN_GDP)
                    {
                        case 1:
                            recordDatiFondoINPDAP.IntegrazioneMinimo = true;
                            break;
                        case 10:
                            recordDatiFondoINPDAP.IndennitaIntegrativaSpecialeConglobata = true;
                            break;
                        case 11:
                            recordDatiFondoINPDAP.IntegrazioneMinimo = true;
                            recordDatiFondoINPDAP.IndennitaIntegrativaSpecialeConglobata = true;
                            break;
                    }

                    if (fondoGDP.PAL_GDP_EURO != 0)
                        recordDatiFondoINPDAP.PensioneAnnuaLorda = fondoGDP.PAL_GDP_EURO;

                    recordDatiFondoINPDAP.ServizioUtileDirittoAA = fondoGDP.SUAN_GDP;
                    recordDatiFondoINPDAP.ServizioUtileDirittoMM = fondoGDP.SUAN_MM_GDP;
                    recordDatiFondoINPDAP.ServizioUtileDirittoGG = fondoGDP.SUAN_GG_GDP;

                    if (fondoGDP.NO336_GDP_EURO != 0)
                        recordDatiFondoINPDAP.RMSSenzaLegge33670QA = fondoGDP.NO336_GDP_EURO;

                    string siglaCategoria = AreaPrelievo.FinalResponse.ListaAnagrafica != null ? GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG) : string.Empty;
                    if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria[0] == 'I')
                    {
                        if (fondoGDP.DECEC_GDP != 0)
                            recordDatiFondoINPDAP.ScadenzaBenefici = Utility.DataFromString(fondoGDP.DECEC_GDP.ToString(), Utility.FormatoData.AAAAmmGG);
                        else
                            recordDatiFondoINPDAP.ScadenzaIllimitata = true;
                    }
                    if (fondoGDP.PAL_A2C12L33595_GDP != 0)
                        recordDatiFondoINPDAP.PALConBenefici = fondoGDP.PAL_A2C12L33595_GDP;
                    if (fondoGDP.DECCALC_GDP != 0)
                        recordDatiFondoINPDAP.DecorrenzaCalcolo = Utility.DataFromString(fondoGDP.DECCALC_GDP.ToString(), Utility.FormatoData.AAAAmmGG);
                    if (fondoGDP.F13ME_GDP == 1)
                        recordDatiFondoINPDAP.TrediciMensilita = true;
                    if (fondoGDP.DIVISORE_GDP != 0)
                        recordDatiFondoINPDAP.Divisore = (byte)fondoGDP.DIVISORE_GDP;
                    if (!string.IsNullOrEmpty(fondoGDP.CAPITOLO_GDP))
                        recordDatiFondoINPDAP.Capitolo = fondoGDP.CAPITOLO_GDP;

                    recordDatiFondoINPDAP.TitolareAltraPensione = string.IsNullOrEmpty(fondoGDP.NATPENS1_GDP) ? (bool?)null : ((fondoGDP.NATPENS1_GDP == "2" || fondoGDP.NATPENS1_GDP == "6") ? true : false);

                    // TODO: Implementare il mapping dei campi della Legge 4/60
                    if (tipoDomanda != GestionePrelievo.TipoDomanda.Reversibilità)
                    {
                        int? PrivilegiataSuperinvaliditaIndennita = null;
                        int? AssegnoIntegrativo = null;
                        int? IntegrazioneIndennitaAssistenza = null;
                        int? IndennitaAccompagnamentoAggiuntiva = null;
                        int? CumuloInfermita = null;
                        int? Categoria2aInfermita = null;
                        int? AssegnoCura = null;
                        int? IndennitaSpecialeAnnua = null;
                        DecodeASSACByFondo(fondoGDP.ASSAC_GDP, "DAP", out PrivilegiataSuperinvaliditaIndennita, out AssegnoIntegrativo,
                            out IntegrazioneIndennitaAssistenza, out IndennitaAccompagnamentoAggiuntiva, out CumuloInfermita,
                            out Categoria2aInfermita, out AssegnoCura, out IndennitaSpecialeAnnua);
                        recordDatiFondoINPDAP.PrivilegiataSuperinvaliditaIndennita = PrivilegiataSuperinvaliditaIndennita;
                        recordDatiFondoINPDAP.AssegnoIntegrativo = AssegnoIntegrativo;
                        recordDatiFondoINPDAP.IntegrazioneIndennitaAssistenza = IntegrazioneIndennitaAssistenza;
                        recordDatiFondoINPDAP.IndennitaAccompagnamentoAggiuntiva = IndennitaAccompagnamentoAggiuntiva;
                        recordDatiFondoINPDAP.CumuloInfermita = CumuloInfermita;
                        recordDatiFondoINPDAP.Categoria2aInfermita = Categoria2aInfermita;
                        recordDatiFondoINPDAP.AssegnoCura = AssegnoCura;
                        recordDatiFondoINPDAP.IndennitaSpecialeAnnua = IndennitaSpecialeAnnua;

                        //if (fondoGDP.ASSAC_GDP != 0)
                        //{
                        //    if (recordDatiFondoINPDAP == null)
                        //        recordDatiFondoINPDAP = new GestioneFondo.DatiFondo();
                        //    recordDatiFondoINPDAP.Privilegiate = true;
                        //}
                    }

                    listaRecordDatiFondoINPDAP.Add(recordDatiFondoINPDAP);
                }
            }
            if (AreaPrelievo.FinalResponse.ListaAgoGDP != null && AreaPrelievo.FinalResponse.ListaAgoGDP.Count > 0)
            {
                if (listaRecordDatiFondoINPDAP == null)
                    listaRecordDatiFondoINPDAP = new List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP>();

                foreach (Data.CMSGTRA.Ago.GDP agoGDP in AreaPrelievo.FinalResponse.ListaAgoGDP)
                {
                    if (!listaRecordDatiFondoINPDAP.Exists(x => x.IdRecordFondo == agoGDP.YFSPROGR))
                        listaRecordDatiFondoINPDAP.Add(new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP { IdRecordFondo = agoGDP.YFSPROGR });
                    GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = listaRecordDatiFondoINPDAP.FirstOrDefault(x => x.IdRecordFondo == agoGDP.YFSPROGR);

                    if (agoGDP.YFSPAL707 != 0M)
                        recordDatiFondoINPDAP.PensioneAnnuaLorda707 = agoGDP.YFSPAL707;
                    if (agoGDP.YFSCOEFTRA != 0M)
                        recordDatiFondoINPDAP.CoefficienteTrasformazione = agoGDP.YFSCOEFTRA;
                }
            }
        }

        public static string GetCategoriaFromTRACATEG(string tracateg)
        {
            List<string> categorieINPDAP = new List<string> { "VCTPS", "VCPDEL", "VCPI", "VCPS", "VCPUG", "ICTPS", "ICPDEL", "ICPI", "ICPS", "ICPUG", "SCTPS", "SCPDEL", "SCPI", "SCPS", "SCPUG" };
            if (!string.IsNullOrEmpty(tracateg) && categorieINPDAP.Contains(tracateg.Trim()))
                return tracateg.Insert(1, "O");

            return tracateg;
        }
        #endregion public members

        #region private members
        private static void DecodeASSACByFondo(int assac, string fondo, out int? PrivilegiataSuperinvaliditaIndennita, out int? AssegnoIntegrativo,
            out int? IntegrazioneIndennitaAssistenza, out int? IndennitaAccompagnamentoAggiuntiva,
            out int? CumuloInfermita, out int? Categoria2aInfermita, out int? AssegnoCura,
            out int? IndennitaSpecialeAnnua)
        {
            PrivilegiataSuperinvaliditaIndennita = null;
            AssegnoIntegrativo = null;
            IntegrazioneIndennitaAssistenza = null;
            IndennitaAccompagnamentoAggiuntiva = null;
            CumuloInfermita = null;
            Categoria2aInfermita = null;
            AssegnoCura = null;
            IndennitaSpecialeAnnua = null;

            if (assac == 0)
                return;

            string strAssac = assac.ToString().PadLeft(8, '0');

            List<GestioneDecodifica.DecPensioniPrivilegiate> listaDecPensioniPrivilegiate = null;
            GestioneDecodifica.GetElencoPensioniPrivilegiate(out listaDecPensioniPrivilegiate);
            if (listaDecPensioniPrivilegiate != null && listaDecPensioniPrivilegiate.Count > 0)
            {
                int?[] listaValori = new int?[8];
                for (int i = 0; i < strAssac.Length; i++)
                {
                    GestioneDecodifica.DecPensioniPrivilegiate decPensioniPrivilegiate =
                        listaDecPensioniPrivilegiate.Find(x => x.TraduzioneSuGP == Utility.StringToNullableChar(strAssac.Substring(i, 1))
                            && x.Posizione == i + 1 && x.Fondo == fondo);
                    if (decPensioniPrivilegiate != null)
                        listaValori[i] = decPensioniPrivilegiate.Id;
                    else
                        listaValori[i] = null;
                }

                PrivilegiataSuperinvaliditaIndennita = listaValori[0];
                AssegnoIntegrativo = listaValori[1];
                IntegrazioneIndennitaAssistenza = listaValori[2];
                IndennitaAccompagnamentoAggiuntiva = listaValori[3];
                CumuloInfermita = listaValori[4];
                Categoria2aInfermita = listaValori[5];
                AssegnoCura = listaValori[6];
                IndennitaSpecialeAnnua = listaValori[7];
            }
        }

        private static bool IsCategoriaINPDAP(Data.FSPR AreaPrelievo)
        {
            bool isCatINPDAP = false;
            try
            {
                Data.CMSGTRA.Anagrafica anagrafica = AreaPrelievo.FinalResponse.ListaAnagrafica[0];
                string siglaCategoria = GetCategoriaFromTRACATEG(anagrafica.TRACATEG);
                string codCat = string.Empty;
                short codCatNum = 0;
                GestioneDecodifica.GetCodCategoriaBySiglaCategoria(siglaCategoria.Trim(), out codCat);
                short.TryParse(codCat, out codCatNum);

                if (codCatNum >= 213 && codCatNum <= 242) // Categorie INPDAP
                    isCatINPDAP = true;
            }
            catch (Exception)
            {
                //eccezione ignorata
            }
            return isCatINPDAP;
        }
        #endregion private members

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
            #endregion private properties

            #region public properties
            public int CodiceComuneInps { get { return _CodiceComuneInps; } set { _CodiceComuneInps = value; } }
            public string Cognome { get { return _Cognome; } set { _Cognome = value; } }
            public string Nome { get { return _Nome; } set { _Nome = value; } }
            public System.Nullable<char> Sesso { get { return _Sesso; } set { _Sesso = value; } }
            public System.Nullable<System.DateTime> DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }
            public System.Nullable<System.DateTime> DataMatrimonio { get { return _DataMatrimonio; } set { _DataMatrimonio = value; } }
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

        public static void ValorizzaScadenzaRevisioneSanitaria(Data.FSPR AreaPrelievo, GestionePrelievo.TipoDomanda tipoDomanda, GestionePensione.DatiPensione datiPensione, string gruppo, string prodotto, Utility.TipoFondo? tipoFondo, GestioneFondo.DatiFondo datiPensioneFondoDatiGenerici, DateTime? dataDecorrenza, out DateTime? scadenzaRevisioneSanitaria)
        {
            scadenzaRevisioneSanitaria = null;

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                //RIC MOTIVI CONTRIBUTIVI - cambiato in tutte le RIC
                if (!String.IsNullOrEmpty(gruppo) && gruppo.Trim() == "0031"
                    && datiPensioneFondoDatiGenerici != null && dataDecorrenza.HasValue && Utility.DataStrettamenteSuccessivaA(dataDecorrenza.Value, new DateTime(2024, 1, 1)))
                {
                    byte? idTipoPLPerRIC = (byte?)IsDomandaContributivo(tipoDomanda, datiPensione);
                    bool isCategoriaINPDAP = IsCategoriaINPDAP(AreaPrelievo);

                    //contributivo puro = 7
                    if (idTipoPLPerRIC == (byte?)Utility.TipoPLPerRIC.ContributivoPuro &&
                        ((tipoFondo == Utility.TipoFondo.FS && datiPensioneFondoDatiGenerici.CodiceSpecifico == 47) ||
                        (tipoFondo == Utility.TipoFondo.PT && datiPensioneFondoDatiGenerici.CodiceSpecifico == 41) ||
                        (tipoFondo == Utility.TipoFondo.TT && datiPensioneFondoDatiGenerici.CodiceSpecifico == 14) ||
                        (tipoFondo == Utility.TipoFondo.ET && datiPensioneFondoDatiGenerici.CodiceSpecifico == 22) ||
                        (isCategoriaINPDAP && (datiPensioneFondoDatiGenerici.CodiceSpecifico == 181 || datiPensioneFondoDatiGenerici.CodiceSpecifico == 182))))
                    {
                        if (AreaPrelievo != null && AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.ListaResidenza != null && AreaPrelievo.FinalResponse.ListaResidenza.Count > 0)
                        {
                            Data.CMSGTRA.Residenza residenza = AreaPrelievo.FinalResponse.ListaResidenza[0];

                            if (residenza != null && residenza.TRH_CESINCUM > 0)
                            {
                                scadenzaRevisioneSanitaria = Utility.DataFromString(residenza.TRH_CESINCUM + "01", Utility.FormatoData.AAAAmmGG);
                            }
                        }
                    }
                }
            }
        }

        public static Utility.TipoPLPerRIC IsDomandaContributivo(GestionePrelievo.TipoDomanda tipoDomanda, GestionePensione.DatiPensione datiPensione)
        {
            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione &&
                (!string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                datiPensione.NaturaPensione.PadRight(3, ' ').Substring(1, 1) == "J"))
            {
                return Utility.TipoPLPerRIC.ContributivoConOpzione;
            }
            if (tipoDomanda == GestionePrelievo.TipoDomanda.Ricostituzione && !String.IsNullOrEmpty(datiPensione.NaturaPensione) &&
               datiPensione.NaturaPensione.PadRight(3, ' ').Substring(1, 1) != "O" &&
               datiPensione.NaturaPensione.PadRight(3, ' ').Substring(1, 1) != "I" &&
                Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(1996, 1, 1)) &&
                Utility.GetTipoCalcoloById(datiPensione.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS) == Utility.TipoCalcolo.Contributivo)
            {
                return Utility.TipoPLPerRIC.ContributivoPuro;
            }

            return Utility.TipoPLPerRIC.Nessuno;
        }
    }
}

