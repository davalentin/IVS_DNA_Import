using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestionePrelievo
    {
        #region public members
        public static void PrelevaDomanda(RichiestaPrelievo richiesta, out RispostaPrelievo risposta, out string messaggioVideo)
        {
            risposta = null;
            messaggioVideo = "";


            DateTime dataSistema = Utility.DataSistemaFs;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoData = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioNuovoTracciato", out controlloDinamicoData);
            DateTime dataInizioNuovoTracciato = Utility.DataFromString(controlloDinamicoData.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            bool isNuovoTracciato = false;
            // Se è una Ric o TRF e la data sistema è compresa tra i due controlli dinamici(DataInizioInterregno e DataFineInterregno)
            // oppure se la data sistema è maggiore uguale al 01/12/2023 viene eseguito il nuovo tracciato
            if ((richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                   && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno)) ||
                Utility.DataSuccessivaA(dataSistema, dataInizioNuovoTracciato))
                isNuovoTracciato = true;

            Data.FSPR AreaPrelievo = null;
            Data.FSPRNew AreaPrelievoNew = null;
            Guid guid = Guid.NewGuid();
            Utility.TipoFondo? tipoFondo = null;

            if (!isNuovoTracciato)
            {
                ValorizzaAreaPrelievo(richiesta, "PRE", out AreaPrelievo, out messaggioVideo);

                Utility.MetodoServizio? metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaPrelievo.TransactionName);
                GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Request, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.IN, richiesta.NumDomanda, guid);

                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;
                EseguiPrelievo(AreaPrelievo);
                ControllaEsitoPrelievo(AreaPrelievo, out messaggioVideo);

                if (!string.IsNullOrEmpty(AreaPrelievo.MessaggioDaLoggare))
                {
                    long numeroDomanda = 0;
                    long.TryParse(richiesta.NumDomanda, out numeroDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaPrelievo.MessaggioDaLoggare, null, null);
                }

                if (AreaPrelievo.HasError)
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Messaggio, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievo.FinalResponse, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);

                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;

                GestioneDecodifica.FS_GetFondoByCategoriaNumerica(richiesta.Categoria.ToString(), richiesta.Certificato, out tipoFondo);
            }
            else
            {
                ValorizzaAreaPrelievoNew(richiesta, "PRE", out AreaPrelievoNew, out messaggioVideo);

                Utility.MetodoServizio? metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaPrelievoNew.TransactionName);
                GestioneLogSoap.SalvaLogSoap(AreaPrelievoNew.Request, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.IN, richiesta.NumDomanda, guid);

                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;
                EseguiPrelievoNew(AreaPrelievoNew);
                ControllaEsitoPrelievoNew(AreaPrelievoNew, out messaggioVideo);

                if (!string.IsNullOrEmpty(AreaPrelievoNew.MessaggioDaLoggare))
                {
                    long numeroDomanda = 0;
                    long.TryParse(richiesta.NumDomanda, out numeroDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaPrelievoNew.MessaggioDaLoggare, null, null);
                }

                if (AreaPrelievoNew.HasError)
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievoNew.Messaggio, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievoNew.FinalResponse, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);

                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;
               
                GestioneDecodifica.FS_GetFondoByCategoriaNumerica(richiesta.Categoria.ToString(), richiesta.Certificato, out tipoFondo);
            }

            if (!isNuovoTracciato)
                NormalizzaAreaToDB(AreaPrelievo, richiesta.TipoDomanda, tipoFondo, richiesta.Gruppo, richiesta.Prodotto, richiesta.Tipo, out risposta, out messaggioVideo);
            else
                NormalizzaAreaToDBNew(AreaPrelievoNew, richiesta.TipoDomanda, tipoFondo, richiesta.Gruppo, richiesta.Prodotto, richiesta.Tipo, out risposta, out messaggioVideo);
        }

        public static void EseguiSprenotazione(RichiestaPrelievo richiesta, out string messaggioVideo)
        {
            messaggioVideo = "";
            Data.FSPR AreaPrelievo = null;
            Guid guid = Guid.NewGuid();

            ValorizzaAreaPrelievo(richiesta, "SPR", out AreaPrelievo, out messaggioVideo);

            Utility.MetodoServizio? metodoServizio = (Utility.MetodoServizio)Utility.GetValueFromDescription<Utility.MetodoServizio>(AreaPrelievo.TransactionName);
            GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Request, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.IN, richiesta.NumDomanda, guid);

            if (!String.IsNullOrEmpty(messaggioVideo))
                return;
            EseguiPrelievo(AreaPrelievo);
            ControllaEsitoPrelievo(AreaPrelievo, out messaggioVideo);
            if (!string.IsNullOrEmpty(AreaPrelievo.MessaggioDaLoggare))
            {
                long numeroDomanda = 0;
                long.TryParse(richiesta.NumDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaPrelievo.MessaggioDaLoggare, null, null);
            }

            if (AreaPrelievo.HasError)
                GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Messaggio, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);
            else
                GestioneLogSoap.SalvaLogSoap(AreaPrelievo.FinalResponse, Utility.Servizio.SrvLiquidazioneFs, metodoServizio.Value, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);

            if (!String.IsNullOrEmpty(messaggioVideo))
                return;
        }
        #endregion public members

        #region private members
        private static void ValorizzaAreaPrelievo(RichiestaPrelievo richiesta, string tipoOperazione, out Data.FSPR AreaPrelievo, out string messaggioVideo)
        {
            AreaPrelievo = null;
            messaggioVideo = "";
            string tipoLiquidazione = string.Empty;

            if (richiesta.Sede == 0 || richiesta.SedeOperatore == 0 || richiesta.Categoria == 0 ||
                richiesta.Certificato == 0)
            {
                messaggioVideo = "Area richiesta non valorizzata correttamente";
                return;
            }
            if (richiesta.TipoDomanda == TipoDomanda.Reversibilità)
                tipoLiquidazione = "A2";
            else if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione)
            {
                tipoLiquidazione = "B1";
                if ((Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, richiesta.SiglaCategoria) == Utility.TipoFondo.PT || Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, richiesta.SiglaCategoria) == Utility.TipoFondo.FS) && tipoOperazione == "PRE")
                {
                    if (new List<string> { "0104", "0304", "0404", "0108", "0308", "0408" }.Contains(richiesta.Prodotto) ||
                       (new List<string> { "0301", "0401", "0101" }.Contains(richiesta.Prodotto) && !new List<string> { "0101", "0103", "0178" }.Contains(richiesta.Tipo)))
                        tipoLiquidazione = "B2";
                    else if (richiesta.Prodotto == "0101" || richiesta.Prodotto == "0301" || richiesta.Prodotto == "0401")
                        tipoLiquidazione = "B3";
                }
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(richiesta.SedeOperatore.ToString().PadLeft(4, '0') + richiesta.CentroOperativoOperatore.ToString().PadLeft(2, '0'));

            //RINNOVO
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.FS);
            string annoCompetenza = "";
            int annoComp = 0;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoFS", out ctrl);

            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out annoComp);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno di competenza
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo l'anno a 0
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                    annoCompetenza = Convert.ToString(dataSistema.Year + 1).Remove(0, 1);
                else
                    annoCompetenza = Convert.ToString(annoComp).Remove(0, 1);
            }

            AreaPrelievo = new INPS.Pensioni.LiquidazioneFs.Data.FSPR(richiesta.Sede, richiesta.SedeOperatore, richiesta.Categoria,
                richiesta.Certificato, tipoOperazione, tipoLiquidazione, annoCompetenza);

            AreaPrelievo.UtilizzaNuovoTracciato = GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.FS.UTILIZZANUOVOTRACCIATO_FSPT, dataSistema);
        }

        private static void EseguiPrelievo(Data.FSPR AreaPrelievo)
        {
            AreaPrelievo.Invoke();
        }

        private static void ControllaEsitoPrelievo(Data.FSPR AreaPrelievo, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (!String.IsNullOrEmpty(AreaPrelievo.Messaggio))
                messaggioVideo = AreaPrelievo.Messaggio;
        }

        private static void NormalizzaAreaToDB(Data.FSPR AreaPrelievo, TipoDomanda tipoDomanda, Utility.TipoFondo? tipoFondo, string gruppo, string prodotto, string tipo, out RispostaPrelievo risposta, out string errore)
        {
            errore = null;
            risposta = new RispostaPrelievo();
            risposta.CodiceFiscale = AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACOFIS;

            if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG) &&
                AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG.StartsWith("S"))
            {
                if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFAA != 0 &&
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFMM != 0)
                {
                    if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFGG == 0)
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFGG = 1;

                    risposta.DataDecorrenza = Utility.DataFromInt(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFAA,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFMM,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFGG);
                }
            }
            else
            {
                if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA == 1111)
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA = 2011;

                if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA != 0 &&
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFMM != 0 &&
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFGG != 0)
                    risposta.DataDecorrenza = Utility.DataFromInt(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFMM,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFGG);


            }
            if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG))
                risposta.SiglaCategoriaPensioneReversibilita = MappingDaHost.GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG);

            #region datiPensione
            GestionePensione.DatiPensione datiPensione = null;
            MappingDaHost.ValorizzaDatiPensione(AreaPrelievo, tipoDomanda, tipoFondo, out datiPensione);
            risposta.DatiPensione = datiPensione;
            #endregion datiPensione

            #region datiLavorazione
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            MappingDaHost.ValorizzaDatiLavorazione(AreaPrelievo, out datiLavorazione);
            risposta.DatiLavorazione = datiLavorazione;
            #endregion datiLavorazione

            #region datiEliminazione
            GestionePensione.DatiEliminazione datiEliminazione = null;
            MappingDaHost.ValorizzaDatiEliminazione(AreaPrelievo, out datiEliminazione);
            risposta.DatiEliminazione = datiEliminazione;
            #endregion datiEliminazione

            #region datiPensioneFondoDatiGenerici
            GestioneFondo.DatiFondo datiPensioneFondoDatiGenerici = null;
            MappingDaHost.ValorizzaPensioneFondoDatiGenerici(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiGenerici);
            risposta.DatiPensioneFondoDatiGenerici = datiPensioneFondoDatiGenerici;
            #endregion datiPensioneFondoDatiGenerici

            #region dati specifico fondo
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        #region datiPensioneFondoDatiEL
                        GestioneFondo.DatiFondoEL datiPensioneFondoDatiEL = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiEL(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiEL);
                        if (datiPensioneFondoDatiEL != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiEL = datiPensioneFondoDatiEL;
                        }
                        #endregion datiPensioneFondoDatiEL
                        break;
                    case Utility.TipoFondo.TT:
                        #region datiPensioneFondoDatiTT
                        GestioneFondo.DatiFondoTT datiPensioneFondoDatiTT = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiTT(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiTT);
                        if (datiPensioneFondoDatiTT != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiTT = datiPensioneFondoDatiTT;
                        }
                        #endregion datiPensioneFondoDatiTT
                        break;
                    case Utility.TipoFondo.ET:
                        #region datiPensioneFondoDatiET
                        GestioneFondo.DatiFondoET datiPensioneFondoDatiET = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiET(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiET);
                        if (datiPensioneFondoDatiET != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiET = datiPensioneFondoDatiET;
                        }
                        #endregion datiPensioneFondoDatiET

                        #region datiInail
                        List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaInail = null;
                        MappingDaHost.ValorizzaDatiINAIL(AreaPrelievo, out listaInail);
                        risposta.ListaInail = listaInail;
                        #endregion datiInail
                        break;
                    case Utility.TipoFondo.VL:
                        #region datiPensioneFondoDatiVL
                        GestioneFondo.DatiFondoVL datiPensioneFondoDatiVL = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiVL(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiVL);
                        if (datiPensioneFondoDatiVL != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiVL = datiPensioneFondoDatiVL;
                        }
                        #endregion datiPensioneFondoDatiVL
                        break;
                    case Utility.TipoFondo.PT:
                        #region datiPensioneFondoDatiPT
                        List<GestioneFondo.DatiFondoPT> datiPensioneFondoDatiPT = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiPT(AreaPrelievo, tipoDomanda, ref datiPensioneFondoDatiGenerici, out datiPensioneFondoDatiPT);
                        if (datiPensioneFondoDatiPT != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiPT = datiPensioneFondoDatiPT;
                        }
                        #endregion datiPensioneFondoDatiPT
                        break;
                    case Utility.TipoFondo.FS:
                        #region datiPensioneFondoDatiFS
                        List<GestioneFondo.DatiFondoFST> datiPensioneFondoDatiFS = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiFS(AreaPrelievo, tipoDomanda, ref datiPensioneFondoDatiGenerici, out datiPensioneFondoDatiFS);
                        if (datiPensioneFondoDatiFS != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiFS = datiPensioneFondoDatiFS;
                        }
                        #endregion datiPensioneFondoDatiFS
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        //TODO adeguare
                        #region datiPensioneFondoDatiPI
                        //GestioneFondo.DatiFondoPI datiPensioneFondoDatiPI = null;
                        //MappingDaHost.ValorizzaPensioneFondoDatiPI(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiPI);
                        //if (datiPensioneFondoDatiPI != null)
                        //{
                        //    risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                        //    risposta.DatiFondoSpecifico.DatiPensioneFondoDatiPI = datiPensioneFondoDatiPI;
                        //}
                        #endregion datiPensioneFondoDatiPI
                        break;
                    case Utility.TipoFondo.GAS:
                        #region datiPensioneFondoDatiGAS
                        GestioneFondo.DatiFondoGAS datiPensioneFondoDatiGAS = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiGAS(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiGAS);
                        if (datiPensioneFondoDatiGAS != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiGAS = datiPensioneFondoDatiGAS;
                        }
                        #endregion datiPensioneFondoDatiGAS
                        break;
                    case Utility.TipoFondo.CL:
                        #region datiPensioneFondoDatiCL
                        GestioneFondo.DatiFondoCL datiPensioneFondoDatiCL = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiCL(AreaPrelievo, out datiPensioneFondoDatiCL);
                        if (datiPensioneFondoDatiCL != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiCL = datiPensioneFondoDatiCL;
                        }
                        #endregion datiPensioneFondoDatiCL
                        break;
                    case Utility.TipoFondo.DZ: //Nuova Gestione Dazi Daniele
                        #region datiPensioneFondoDatiDZ
                        List<GestioneFondo.DatiFondoDZ> listaDatiPensioneFondoDatiDZ = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiDZ(AreaPrelievo, tipoDomanda, out listaDatiPensioneFondoDatiDZ);
                        if (listaDatiPensioneFondoDatiDZ != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiDZ = listaDatiPensioneFondoDatiDZ;
                        }
                        #endregion datiPensioneFondoDatiDZ
                        break;
                    case Utility.TipoFondo.ES:
                        #region datiPensioneFondoDatiES
                        GestioneFondo.DatiFondoES datiPensioneFondoDatiES = null;
                        MappingDaHost.ValorizzaPensioneFondoDatiES(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiES);
                        if (datiPensioneFondoDatiES != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiES = datiPensioneFondoDatiES;
                        }
                        #endregion datiPensioneFondoDatiES
                        break;
                    case Utility.TipoFondo.PM:
                        #region datiPensioneFondoDatiPM
                        //TODO adeguare
                        //GestioneFondo.DatiFondoPM datiPensioneFondoDatiPM = null;
                        //MappingDaHost.ValorizzaPensioneFondoDatiPM(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiPM);
                        //if (datiPensioneFondoDatiPM != null)
                        //{
                        //    risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                        //    risposta.DatiFondoSpecifico.DatiPensioneFondoDatiPM = datiPensioneFondoDatiPM;
                        //}
                        #endregion datiPensioneFondoDatiPM

                        #region datiInail
                        List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaINAIL = null;
                        MappingDaHost.ValorizzaDatiINAIL(AreaPrelievo, out listaINAIL);
                        risposta.ListaInail = listaINAIL;
                        #endregion datiInail
                        break;
                }
            }
            #endregion dati specifico fondo

            #region datiPensioneInpdap
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = null;
            MappingDaHost.ValorizzaDatiPensioneINPDAP(AreaPrelievo, tipoDomanda, out listaDatiPensioneINPDAP);
            if (listaDatiPensioneINPDAP != null && listaDatiPensioneINPDAP.Count > 0)
            {
                risposta.ListaDatiPensioneINPDAP = listaDatiPensioneINPDAP;
            }
            #endregion datiPensioneInpdap

            #region datiFondoInpdap
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            MappingDaHost.ValorizzaRecordDatiFondoINPDAP(AreaPrelievo, tipoDomanda, out listaRecordDatiFondoINPDAP);
            if (listaRecordDatiFondoINPDAP != null && listaRecordDatiFondoINPDAP.Count > 0)
            {
                risposta.ListaRecordDatiFondoINPDAP = listaRecordDatiFondoINPDAP;
            }
            #endregion datiFondoInpdap

            #region datiSindacato
            GestionePensione.DatiSindacato datiSindacato = null;
            MappingDaHost.ValorizzaDatiSindacato(AreaPrelievo, out datiSindacato);
            risposta.DatiSindacato = datiSindacato;
            #endregion datiSindacato

            #region datiDetrazioni
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            MappingDaHost.ValorizzaDatiDetrazioni(AreaPrelievo, out datiDetrazioni);
            risposta.DatiDetrazioni = datiDetrazioni;
            #endregion datiDetrazioni

            #region datiPagamento
            GestionePagamento.DatiPagamento datiPagamento = null;
            MappingDaHost.ValorizzaDatiPagamento(AreaPrelievo, out datiPagamento);
            risposta.DatiPagamento = datiPagamento;
            #endregion datiPagamento

            #region listaFamiliari
            List<Entity.DatiFamiliari> listaFamiliari = null;
            Dictionary<string, string> componentiFamiliari = null;
            MappingDaHost.ValorizzaDatiFamiliare(AreaPrelievo, tipoDomanda, out listaFamiliari, out componentiFamiliari, out errore);
            risposta.ListaFamiliari = listaFamiliari;

            if (!string.IsNullOrEmpty(errore))
                return;
            #endregion listaFamiliari

            #region listaDetrazioniContitolare

            List<GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato> listaDetrazioniContitolare = null;
            MappingDaHost.ValorizzaDatiDetrazioniContitolare(AreaPrelievo, tipoDomanda, out listaDetrazioniContitolare);
            risposta.ListaDetrazioniContitolare = listaDetrazioniContitolare;

            #endregion listaDetrazioniContitolare

            #region listaRecordFondo
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            MappingDaHost.ValorizzaRecordFondo(AreaPrelievo, out listaRecordFondo);
            risposta.ListaRecordFondo = listaRecordFondo;
            #endregion listaRecordFondo

            #region listaDatiCalcoloContributivo
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            MappingDaHost.ValorizzaDatiCalcoloContributivo(AreaPrelievo, datiPensione, out listaDatiCalcoloContributivo);
            risposta.ListaDatiCalcoloContributivo = listaDatiCalcoloContributivo;
            #endregion listaDatiCalcoloContributivo

            #region datiCalcoloRetributivo
            GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
            MappingDaHost.ValorizzaDatiCalcoloRetributivo(AreaPrelievo, out datiCalcoloRetributivo);
            risposta.DatiCalcoloRetributivo = datiCalcoloRetributivo;
            #endregion datiCalcoloRetributivo

            #region datiCalcoloRetributivoDZ
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivoDZ = null;
            MappingDaHost.ValorizzaDatiCalcoloRetributivoDZ(AreaPrelievo, out listaDatiCalcoloRetributivoDZ);
            risposta.DatiCalcoloRetributivoDZ = listaDatiCalcoloRetributivoDZ;
            #endregion datiCalcoloRetributivoDZ

            #region Controllo TipoCalcolo - legge Monti
            if (risposta.DatiPensione != null && risposta.DatiPensione.TipoCalcolo.HasValue && risposta.DatiPensione.TipoCalcolo.Value == 18 &&
                risposta.ListaDatiCalcoloContributivo != null && risposta.ListaDatiCalcoloContributivo.Exists(x => x.Montante.HasValue) &&
                risposta.DatiCalcoloRetributivo != null && risposta.DatiCalcoloRetributivo.RMSQuotaD.HasValue)
            {
                risposta.DatiPensione.TipoCalcolo = 25;
            }
            #endregion Controllo TipoCalcolo - legge Monti

            #region listaSupplementi
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            MappingDaHost.ValorizzaDatiSupplementi(AreaPrelievo, tipoFondo, tipoDomanda, prodotto, tipo, out listaSupplementi);
            risposta.ListaSupplementi = listaSupplementi;
            #endregion listaSupplementi

            #region datiPatronato
            GestionePensione.DatiPatronato datiPatronato = null;
            MappingDaHost.ValorizzaDatiPatronato(AreaPrelievo, out datiPatronato);
            risposta.DatiPatronato = datiPatronato;
            #endregion datiPatronato

            #region datiDanteCausa
            MappingDaHost.DatiAnagDanteCausa datiAnagDanteCausa = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            MappingDaHost.ValorizzaDatiDanteCausa(AreaPrelievo, tipoDomanda, tipoFondo, out datiAnagDanteCausa, out datiDanteCausa);
            risposta.DatiAnagDanteCausa = datiAnagDanteCausa;
            risposta.DatiDanteCausa = datiDanteCausa;
            #endregion datiDanteCausa

            #region listaResidenzeEstere
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            MappingDaHost.ValorizzaDatiResidenzeEstere(AreaPrelievo, out listaResidenzeEstere);
            risposta.ListaResidenzeEstere = listaResidenzeEstere;
            #endregion listaResidenzeEstere

            #region listaStatiCivili
            List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili = null;
            MappingDaHost.ValorizzaDatiStatiCivili(AreaPrelievo, out listaStatiCivili);
            risposta.ListaStatiCivili = listaStatiCivili;
            #endregion listaStatiCivili

            #region datiDelegato
            MappingDaHost.DatiDelegato datiDelegato = null;
            MappingDaHost.ValorizzaDatiDelegato(AreaPrelievo, out datiDelegato);
            risposta.DatiDelegato = datiDelegato;
            #endregion datiDelegato

            #region datiTutore
            MappingDaHost.DatiTutore datiTutore = null;
            MappingDaHost.ValorizzaDatiTutore(AreaPrelievo, out datiTutore);
            risposta.DatiTutore = datiTutore;
            #endregion datiTutore

            #region datiIstruttoria
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            MappingDaHost.ValorizzaDatiIstruttoria(AreaPrelievo, tipoDomanda, ref datiPensione, gruppo, tipoFondo, out datiIstruttoria);
            risposta.DatiIstruttoria = datiIstruttoria;
            #endregion datiIstruttoria

            #region datiMaggiorazioniBenefici
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            MappingDaHost.ValorizzaDatiMaggiorazioni(AreaPrelievo, ref datiPensione, tipoDomanda, tipoFondo, prodotto, tipo, out datiMaggiorazioniBenefici);
            risposta.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            #endregion datiMaggiorazioniBenefici

            #region datiDL407
            GestioneDL407.DatiDL407 datiDL407 = null;
            MappingDaHost.ValorizzaDatiDL407(AreaPrelievo, risposta.DataDecorrenza, ref datiPensioneFondoDatiGenerici, ref datiDanteCausa, out datiDL407);
            risposta.DatiDL407 = datiDL407;
            #endregion datiDL407

            #region datiOneriTerrorismo
            List<Entity.DatiBenefici.OneriTerrorismo> listaOneriTerrorismo = null;
            MappingDaHost.ValorizzaDatiOneriTerrorismo(AreaPrelievo, ref datiPensione, out listaOneriTerrorismo);
            risposta.ListaOneriTerrorismo = listaOneriTerrorismo;
            #endregion datiOneriTerrorismo

            #region listaDatiServizioUtile
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
            MappingDaHost.ValorizzaDatiServizioUtile(AreaPrelievo, out listaDatiServizioUtile);
            risposta.ListaDatiServizioUtile = listaDatiServizioUtile;
            #endregion listaDatiServizioUtile

            #region listaDatiServizioUtile707
            List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707 = null;
            MappingDaHost.ValorizzaDatiServizioUtile707(AreaPrelievo, out listaDatiServizioUtile707);
            risposta.ListaDatiServizioUtile707 = listaDatiServizioUtile707;
            #endregion listaDatiServizioUtile

            #region listaDatiServizioUtileINPDAP
            List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP = null;
            MappingDaHost.ValorizzaDatiServizioUtileINPDAP(AreaPrelievo, out listaDatiServizioUtileINPDAP);
            risposta.ListaDatiServizioUtileINPDAP = listaDatiServizioUtileINPDAP;
            #endregion listaDatiServizioUtileINPDAP

            #region listaDatiServizioUtileINPDAP707
            List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtileINPDAP707 = null;
            MappingDaHost.ValorizzaDatiServizioUtileINPDAP707(AreaPrelievo, out listaDatiServizioUtileINPDAP707);
            risposta.ListaDatiServizioUtileINPDAP707 = listaDatiServizioUtileINPDAP707;
            #endregion listaDatiServizioUtileINPDAP707

            #region datiOneri
            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            MappingDaHost.ValorizzaDatiOneri(AreaPrelievo, out listaDatiOneri);
            risposta.ListaDatiOneri = listaDatiOneri;
            #endregion datiOneri

            #region datiBeneficiParticolari
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolari = null;
            MappingDaHost.ValorizzaDatiBeneficiParticolari(AreaPrelievo, out listaDatiBeneficiParticolari);
            risposta.ListaDatiBeneficiParticolari = listaDatiBeneficiParticolari;
            #endregion datiBeneficiParticolari

            #region DatiNoCalcolo
            List<Entity.DatiNoCalcolo> listaDatiNoCalcolo = null;
            MappingDaHost.ValorizzaDatiNonCalcolo(AreaPrelievo, componentiFamiliari, out listaDatiNoCalcolo);
            risposta.ListaDatiNoCalcolo = listaDatiNoCalcolo;
            #endregion DatiNoCalcolo

            //ENG - Memo 28_2024
            #region ScadenzaRevisioneSanitaria
            DateTime? scadenzaRevisioneSanitaria = null;
            MappingDaHost.ValorizzaScadenzaRevisioneSanitaria(AreaPrelievo, tipoDomanda, datiPensione, gruppo, prodotto, tipoFondo, datiPensioneFondoDatiGenerici, risposta.DataDecorrenza, out scadenzaRevisioneSanitaria);
            if (scadenzaRevisioneSanitaria.HasValue)
            {
                if (risposta.DatiIstruttoria == null)
                    risposta.DatiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();

                risposta.DatiIstruttoria.ScadenzaRevisioneSanitaria = scadenzaRevisioneSanitaria;
            }
            #endregion ScadenzaRevisioneSanitaria

        }

        private static void ValorizzaAreaPrelievoNew(RichiestaPrelievo richiesta, string tipoOperazione, out Data.FSPRNew AreaPrelievo, out string messaggioVideo)
        {
            AreaPrelievo = null;
            messaggioVideo = "";
            string tipoLiquidazione = string.Empty;

            if (richiesta.Sede == 0 || richiesta.SedeOperatore == 0 || richiesta.Categoria == 0 ||
                richiesta.Certificato == 0)
            {
                messaggioVideo = "Area richiesta non valorizzata correttamente";
                return;
            }
            if (richiesta.TipoDomanda == TipoDomanda.Reversibilità)
                tipoLiquidazione = "A2";
            else if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione)
            {
                tipoLiquidazione = "B1";
                if ((Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, richiesta.SiglaCategoria) == Utility.TipoFondo.PT || Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, richiesta.SiglaCategoria) == Utility.TipoFondo.FS) && tipoOperazione == "PRE")
                {
                    if (new List<string> { "0104", "0304", "0404", "0108", "0308", "0408" }.Contains(richiesta.Prodotto) ||
                       (new List<string> { "0301", "0401", "0101" }.Contains(richiesta.Prodotto) && !new List<string> { "0101", "0103", "0178" }.Contains(richiesta.Tipo)))
                        tipoLiquidazione = "B2";
                    else if (richiesta.Prodotto == "0101" || richiesta.Prodotto == "0301" || richiesta.Prodotto == "0401")
                        tipoLiquidazione = "B3";
                }
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(richiesta.SedeOperatore.ToString().PadLeft(4, '0') + richiesta.CentroOperativoOperatore.ToString().PadLeft(2, '0'));

            //RINNOVO
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.FS);
            string annoCompetenza = "";
            int annoComp = 0;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoFS", out ctrl);

            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.FS, out annoComp);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno di competenza
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo l'anno a 0
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                    annoCompetenza = Convert.ToString(dataSistema.Year + 1).Remove(0, 1);
                else
                    annoCompetenza = Convert.ToString(annoComp).Remove(0, 1);
            }

            AreaPrelievo = new INPS.Pensioni.LiquidazioneFs.Data.FSPRNew(richiesta.Sede, richiesta.SedeOperatore, richiesta.Categoria,
                richiesta.Certificato, tipoOperazione, tipoLiquidazione, annoCompetenza);

            AreaPrelievo.UtilizzaNuovoTracciato = GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.FS.UTILIZZANUOVOTRACCIATO_FSPT, dataSistema);
        }

        private static void EseguiPrelievoNew(Data.FSPRNew AreaPrelievo)
        {
            AreaPrelievo.Invoke();
        }

        private static void ControllaEsitoPrelievoNew(Data.FSPRNew AreaPrelievo, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (!String.IsNullOrEmpty(AreaPrelievo.Messaggio))
                messaggioVideo = AreaPrelievo.Messaggio;
        }

        private static void NormalizzaAreaToDBNew(Data.FSPRNew AreaPrelievo, TipoDomanda tipoDomanda, Utility.TipoFondo? tipoFondo, string gruppo, string prodotto, string tipo, out RispostaPrelievo risposta, out string errore)
        {
            errore = null;
            risposta = new RispostaPrelievo();
            risposta.CodiceFiscale = AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACOFIS;

            if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG) &&
                AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG.StartsWith("S"))
            {
                if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFAA != 0 &&
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFMM != 0)
                {
                    if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFGG == 0)
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFGG = 1;

                    risposta.DataDecorrenza = Utility.DataFromInt(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFAA,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFMM,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRASPFGG);
                }
            }
            else
            {
                if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA == 1111)
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA = 2011;

                if (AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA != 0 &&
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFMM != 0 &&
                    AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFGG != 0)
                    risposta.DataDecorrenza = Utility.DataFromInt(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFAA,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFMM,
                        AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRADIFGG);


            }
            if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG))
                risposta.SiglaCategoriaPensioneReversibilita = MappingDaHostNew.GetCategoriaFromTRACATEG(AreaPrelievo.FinalResponse.ListaAnagrafica[0].TRACATEG);

            #region datiPensione
            GestionePensione.DatiPensione datiPensione = null;
            MappingDaHostNew.ValorizzaDatiPensione(AreaPrelievo, tipoDomanda, tipoFondo, out datiPensione);
            risposta.DatiPensione = datiPensione;
            #endregion datiPensione

            #region datiLavorazione
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            MappingDaHostNew.ValorizzaDatiLavorazione(AreaPrelievo, out datiLavorazione);
            risposta.DatiLavorazione = datiLavorazione;
            #endregion datiLavorazione

            #region datiEliminazione
            GestionePensione.DatiEliminazione datiEliminazione = null;
            MappingDaHostNew.ValorizzaDatiEliminazione(AreaPrelievo, out datiEliminazione);
            risposta.DatiEliminazione = datiEliminazione;
            #endregion datiEliminazione

            #region datiPensioneFondoDatiGenerici
            GestioneFondo.DatiFondo datiPensioneFondoDatiGenerici = null;
            MappingDaHostNew.ValorizzaPensioneFondoDatiGenerici(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiGenerici);
            risposta.DatiPensioneFondoDatiGenerici = datiPensioneFondoDatiGenerici;
            #endregion datiPensioneFondoDatiGenerici

            #region dati specifico fondo
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        #region datiPensioneFondoDatiEL
                        GestioneFondo.DatiFondoEL datiPensioneFondoDatiEL = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiEL(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiEL);
                        if (datiPensioneFondoDatiEL != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiEL = datiPensioneFondoDatiEL;
                        }
                        #endregion datiPensioneFondoDatiEL
                        break;
                    case Utility.TipoFondo.TT:
                        #region datiPensioneFondoDatiTT
                        GestioneFondo.DatiFondoTT datiPensioneFondoDatiTT = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiTT(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiTT);
                        if (datiPensioneFondoDatiTT != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiTT = datiPensioneFondoDatiTT;
                        }
                        #endregion datiPensioneFondoDatiTT
                        break;
                    case Utility.TipoFondo.ET:
                        #region datiPensioneFondoDatiET
                        GestioneFondo.DatiFondoET datiPensioneFondoDatiET = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiET(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiET);
                        if (datiPensioneFondoDatiET != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiET = datiPensioneFondoDatiET;
                        }
                        #endregion datiPensioneFondoDatiET

                        #region datiInail
                        List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaInail = null;
                        MappingDaHostNew.ValorizzaDatiINAIL(AreaPrelievo, out listaInail);
                        risposta.ListaInail = listaInail;
                        #endregion datiInail
                        break;
                    case Utility.TipoFondo.VL:
                        #region datiPensioneFondoDatiVL
                        GestioneFondo.DatiFondoVL datiPensioneFondoDatiVL = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiVL(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiVL);
                        if (datiPensioneFondoDatiVL != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiVL = datiPensioneFondoDatiVL;
                        }
                        #endregion datiPensioneFondoDatiVL
                        break;
                    case Utility.TipoFondo.PT:
                        #region datiPensioneFondoDatiPT
                        List<GestioneFondo.DatiFondoPT> datiPensioneFondoDatiPT = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiPT(AreaPrelievo, tipoDomanda, ref datiPensioneFondoDatiGenerici, out datiPensioneFondoDatiPT);
                        if (datiPensioneFondoDatiPT != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiPT = datiPensioneFondoDatiPT;
                        }
                        #endregion datiPensioneFondoDatiPT
                        break;
                    case Utility.TipoFondo.FS:
                        #region datiPensioneFondoDatiFS
                        List<GestioneFondo.DatiFondoFST> datiPensioneFondoDatiFS = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiFS(AreaPrelievo, tipoDomanda, ref datiPensioneFondoDatiGenerici, out datiPensioneFondoDatiFS);
                        if (datiPensioneFondoDatiFS != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiFS = datiPensioneFondoDatiFS;
                        }
                        #endregion datiPensioneFondoDatiFS
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        #region datiPensioneFondoDatiPI
                        List <GestioneFondo.DatiFondoPI> datiPensioneFondoDatiPI = null;
                        List<GestioneFondo.DatiAgoPI> lstDatiAgoPI;
                        List<GestioneFondo.DatiAgoTeoricoPI> lstDatiAgoTeoricoPI;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiPI(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiPI, out lstDatiAgoPI, out lstDatiAgoTeoricoPI);
                        if (datiPensioneFondoDatiPI != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiPI = datiPensioneFondoDatiPI;
                        }

                        if(lstDatiAgoPI != null)
                        {
                            if(risposta.DatiFondoSpecifico == null) risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneAgoPI = lstDatiAgoPI;
                        }

                        if (lstDatiAgoTeoricoPI != null)
                        {
                            if (risposta.DatiFondoSpecifico == null) risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneAgoTeoricoPI = lstDatiAgoTeoricoPI;
                        }
                        #endregion datiPensioneFondoDatiPI
                        break;
                    case Utility.TipoFondo.GAS:
                        #region datiPensioneFondoDatiGAS
                        GestioneFondo.DatiFondoGAS datiPensioneFondoDatiGAS = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiGAS(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiGAS);
                        if (datiPensioneFondoDatiGAS != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiGAS = datiPensioneFondoDatiGAS;
                        }
                        #endregion datiPensioneFondoDatiGAS
                        break;
                    case Utility.TipoFondo.CL:
                        #region datiPensioneFondoDatiCL
                        GestioneFondo.DatiFondoCL datiPensioneFondoDatiCL = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiCL(AreaPrelievo, out datiPensioneFondoDatiCL);
                        if (datiPensioneFondoDatiCL != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiCL = datiPensioneFondoDatiCL;
                        }
                        #endregion datiPensioneFondoDatiCL
                        break;
                    case Utility.TipoFondo.DZ: //Nuova Gestione Dazi Daniele
                        #region datiPensioneFondoDatiDZ
                        List<GestioneFondo.DatiFondoDZ> listaDatiPensioneFondoDatiDZ = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiDZ(AreaPrelievo, tipoDomanda, out listaDatiPensioneFondoDatiDZ);
                        if (listaDatiPensioneFondoDatiDZ != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiDZ = listaDatiPensioneFondoDatiDZ;
                        }
                        #endregion datiPensioneFondoDatiDZ
                        break;
                    case Utility.TipoFondo.ES:
                        #region datiPensioneFondoDatiES
                        GestioneFondo.DatiFondoES datiPensioneFondoDatiES = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiES(AreaPrelievo, tipoDomanda, out datiPensioneFondoDatiES);
                        if (datiPensioneFondoDatiES != null)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiES = datiPensioneFondoDatiES;
                        }
                        #endregion datiPensioneFondoDatiES
                        break;
                    case Utility.TipoFondo.PM:
                        #region datiPensioneFondoDatiPM
                        List<GestioneFondo.DatiFondoPM> listaDatiPensioneFondoDatiPM = null;
                        List<GestioneFondo.DatiAgoPM> lstDatiAgoPM = null;
                        MappingDaHostNew.ValorizzaPensioneFondoDatiPM(AreaPrelievo, tipoDomanda, out listaDatiPensioneFondoDatiPM, out lstDatiAgoPM);
                        if (listaDatiPensioneFondoDatiPM != null && listaDatiPensioneFondoDatiPM.Count > 0)
                        {
                            risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneFondoDatiPM = listaDatiPensioneFondoDatiPM;
                        }

                        if (lstDatiAgoPM != null)                     
                        {
                            if (risposta.DatiFondoSpecifico == null) 
                                risposta.DatiFondoSpecifico = new DatiFondoSpecifico();
                            risposta.DatiFondoSpecifico.DatiPensioneAgoPM = lstDatiAgoPM;
                          
                        }
                        #endregion datiPensioneFondoDatiPM

                        #region datiInail
                        List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaINAIL = null;
                        MappingDaHostNew.ValorizzaDatiINAIL(AreaPrelievo, out listaINAIL);
                        risposta.ListaInail = listaINAIL;
                        #endregion datiInail
                        break;
                }
            }
            #endregion dati specifico fondo

            #region datiPensioneInpdap
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = null;
            MappingDaHostNew.ValorizzaDatiPensioneINPDAP(AreaPrelievo, tipoDomanda, out listaDatiPensioneINPDAP);
            if (listaDatiPensioneINPDAP != null && listaDatiPensioneINPDAP.Count > 0)
            {
                risposta.ListaDatiPensioneINPDAP = listaDatiPensioneINPDAP;
            }
            #endregion datiPensioneInpdap

            #region datiFondoInpdap
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            MappingDaHostNew.ValorizzaRecordDatiFondoINPDAP(AreaPrelievo, tipoDomanda, out listaRecordDatiFondoINPDAP);
            if (listaRecordDatiFondoINPDAP != null && listaRecordDatiFondoINPDAP.Count > 0)
            {
                risposta.ListaRecordDatiFondoINPDAP = listaRecordDatiFondoINPDAP;
            }
            #endregion datiFondoInpdap

            #region datiSindacato
            GestionePensione.DatiSindacato datiSindacato = null;
            MappingDaHostNew.ValorizzaDatiSindacato(AreaPrelievo, out datiSindacato);
            risposta.DatiSindacato = datiSindacato;
            #endregion datiSindacato

            #region datiDetrazioni
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            MappingDaHostNew.ValorizzaDatiDetrazioni(AreaPrelievo, out datiDetrazioni);
            risposta.DatiDetrazioni = datiDetrazioni;
            #endregion datiDetrazioni

            #region datiPagamento
            GestionePagamento.DatiPagamento datiPagamento = null;
            MappingDaHostNew.ValorizzaDatiPagamento(AreaPrelievo, out datiPagamento);
            risposta.DatiPagamento = datiPagamento;
            #endregion datiPagamento

            #region listaFamiliari
            List<Entity.DatiFamiliari> listaFamiliari = null;
            Dictionary<string, string> componentiFamiliari = null;
            MappingDaHostNew.ValorizzaDatiFamiliare(AreaPrelievo, tipoDomanda, out listaFamiliari, out componentiFamiliari, out errore);
            risposta.ListaFamiliari = listaFamiliari;

            if (!string.IsNullOrEmpty(errore))
                return;
            #endregion listaFamiliari

            #region listaDetrazioniContitolare

            List<GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato> listaDetrazioniContitolare = null;
            MappingDaHostNew.ValorizzaDatiDetrazioniContitolare(AreaPrelievo, tipoDomanda, out listaDetrazioniContitolare);
            risposta.ListaDetrazioniContitolare = listaDetrazioniContitolare;

            #endregion listaDetrazioniContitolare

            #region listaRecordFondo
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            MappingDaHostNew.ValorizzaRecordFondo(AreaPrelievo, out listaRecordFondo);
            risposta.ListaRecordFondo = listaRecordFondo;
            #endregion listaRecordFondo

            #region listaDatiCalcoloContributivo
            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            MappingDaHostNew.ValorizzaDatiCalcoloContributivo(AreaPrelievo, datiPensione, out listaDatiCalcoloContributivo);
            risposta.ListaDatiCalcoloContributivo = listaDatiCalcoloContributivo;
            #endregion listaDatiCalcoloContributivo

            #region datiCalcoloRetributivo
            GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
            MappingDaHostNew.ValorizzaDatiCalcoloRetributivo(AreaPrelievo, out datiCalcoloRetributivo);
            risposta.DatiCalcoloRetributivo = datiCalcoloRetributivo;
            #endregion datiCalcoloRetributivo

            #region datiCalcoloRetributivoDZ
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivoDZ = null;
            MappingDaHostNew.ValorizzaDatiCalcoloRetributivoDZ(AreaPrelievo, out listaDatiCalcoloRetributivoDZ);
            risposta.DatiCalcoloRetributivoDZ = listaDatiCalcoloRetributivoDZ;
            #endregion datiCalcoloRetributivoDZ

            #region Controllo TipoCalcolo - legge Monti
            if (risposta.DatiPensione != null && risposta.DatiPensione.TipoCalcolo.HasValue && risposta.DatiPensione.TipoCalcolo.Value == 18 &&
                risposta.ListaDatiCalcoloContributivo != null && risposta.ListaDatiCalcoloContributivo.Exists(x => x.Montante.HasValue) &&
                risposta.DatiCalcoloRetributivo != null && risposta.DatiCalcoloRetributivo.RMSQuotaD.HasValue)
            {
                risposta.DatiPensione.TipoCalcolo = 25;
            }
            #endregion Controllo TipoCalcolo - legge Monti

            #region listaSupplementi
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            MappingDaHostNew.ValorizzaDatiSupplementi(AreaPrelievo, tipoFondo, tipoDomanda, prodotto, tipo, out listaSupplementi);
            risposta.ListaSupplementi = listaSupplementi;
            #endregion listaSupplementi

            #region datiPatronato
            GestionePensione.DatiPatronato datiPatronato = null;
            MappingDaHostNew.ValorizzaDatiPatronato(AreaPrelievo, out datiPatronato);
            risposta.DatiPatronato = datiPatronato;
            #endregion datiPatronato

            #region datiDanteCausa
            MappingDaHostNew.DatiAnagDanteCausa datiAnagDanteCausaNew = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            MappingDaHostNew.ValorizzaDatiDanteCausa(AreaPrelievo, tipoDomanda, tipoFondo, out datiAnagDanteCausaNew, out datiDanteCausa);
            MappingDaHost.DatiAnagDanteCausa datiAnagDanteCausa = new MappingDaHost.DatiAnagDanteCausa();
            Utility.ValorizzaOggetti(datiAnagDanteCausaNew, datiAnagDanteCausa);
            risposta.DatiDanteCausa = datiDanteCausa;
            risposta.DatiAnagDanteCausa = datiAnagDanteCausa;
            #endregion datiDanteCausa

            #region listaResidenzeEstere
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            MappingDaHostNew.ValorizzaDatiResidenzeEstere(AreaPrelievo, out listaResidenzeEstere);
            risposta.ListaResidenzeEstere = listaResidenzeEstere;
            #endregion listaResidenzeEstere

            #region listaStatiCivili
            List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili = null;
            MappingDaHostNew.ValorizzaDatiStatiCivili(AreaPrelievo, out listaStatiCivili);
            risposta.ListaStatiCivili = listaStatiCivili;
            #endregion listaStatiCivili

            #region datiDelegato
            MappingDaHostNew.DatiDelegato datiDelegatoNew = null;
            MappingDaHostNew.ValorizzaDatiDelegato(AreaPrelievo, out datiDelegatoNew);
            MappingDaHost.DatiDelegato datiDelegato = new MappingDaHost.DatiDelegato();
            Utility.ValorizzaOggetti(datiDelegatoNew, datiDelegato);
            risposta.DatiDelegato = datiDelegato;
            #endregion datiDelegato

            #region datiTutore
            MappingDaHostNew.DatiTutore datiTutoreNew = null;
            MappingDaHostNew.ValorizzaDatiTutore(AreaPrelievo, out datiTutoreNew);
            MappingDaHost.DatiTutore datiTutore = new MappingDaHost.DatiTutore();
            Utility.ValorizzaOggetti(datiTutoreNew, datiTutore);
            risposta.DatiTutore = datiTutore;
            #endregion datiTutore

            #region datiIstruttoria
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            MappingDaHostNew.ValorizzaDatiIstruttoria(AreaPrelievo, tipoDomanda, ref datiPensione, gruppo, tipoFondo, out datiIstruttoria);
            risposta.DatiIstruttoria = datiIstruttoria;
            #endregion datiIstruttoria

            #region datiMaggiorazioniBenefici
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            MappingDaHostNew.ValorizzaDatiMaggiorazioni(AreaPrelievo, ref datiPensione, tipoDomanda, tipoFondo, prodotto, tipo, out datiMaggiorazioniBenefici);
            risposta.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            #endregion datiMaggiorazioniBenefici

            #region datiDL407
            GestioneDL407.DatiDL407 datiDL407 = null;
            MappingDaHostNew.ValorizzaDatiDL407(AreaPrelievo, risposta.DataDecorrenza, ref datiPensioneFondoDatiGenerici, ref datiDanteCausa, out datiDL407);
            risposta.DatiDL407 = datiDL407;
            #endregion datiDL407

            #region datiOneriTerrorismo
            List<Entity.DatiBenefici.OneriTerrorismo> listaOneriTerrorismo = null;
            MappingDaHostNew.ValorizzaDatiOneriTerrorismo(AreaPrelievo, ref datiPensione, out listaOneriTerrorismo);
            risposta.ListaOneriTerrorismo = listaOneriTerrorismo;
            #endregion datiOneriTerrorismo

            #region listaDatiServizioUtile
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
            MappingDaHostNew.ValorizzaDatiServizioUtile(AreaPrelievo, out listaDatiServizioUtile);
            risposta.ListaDatiServizioUtile = listaDatiServizioUtile;
            #endregion listaDatiServizioUtile

            #region listaDatiServizioUtile707
            List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707 = null;
            MappingDaHostNew.ValorizzaDatiServizioUtile707(AreaPrelievo, out listaDatiServizioUtile707);
            risposta.ListaDatiServizioUtile707 = listaDatiServizioUtile707;
            #endregion listaDatiServizioUtile

            #region listaDatiServizioUtileINPDAP
            List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP = null;
            MappingDaHostNew.ValorizzaDatiServizioUtileINPDAP(AreaPrelievo, out listaDatiServizioUtileINPDAP);
            risposta.ListaDatiServizioUtileINPDAP = listaDatiServizioUtileINPDAP;
            #endregion listaDatiServizioUtileINPDAP

            #region listaDatiServizioUtileINPDAP707
            List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtileINPDAP707 = null;
            MappingDaHostNew.ValorizzaDatiServizioUtileINPDAP707(AreaPrelievo, out listaDatiServizioUtileINPDAP707);
            risposta.ListaDatiServizioUtileINPDAP707 = listaDatiServizioUtileINPDAP707;
            #endregion listaDatiServizioUtileINPDAP707

            #region datiOneri
            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            MappingDaHostNew.ValorizzaDatiOneri(AreaPrelievo, out listaDatiOneri);
            risposta.ListaDatiOneri = listaDatiOneri;
            #endregion datiOneri

            #region datiBeneficiParticolari
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolari = null;
            MappingDaHostNew.ValorizzaDatiBeneficiParticolari(AreaPrelievo, out listaDatiBeneficiParticolari);
            risposta.ListaDatiBeneficiParticolari = listaDatiBeneficiParticolari;
            #endregion datiBeneficiParticolari

            #region DatiNoCalcolo
            List<Entity.DatiNoCalcolo> listaDatiNoCalcolo = null;
            MappingDaHostNew.ValorizzaDatiNonCalcolo(AreaPrelievo, componentiFamiliari, out listaDatiNoCalcolo);
            risposta.ListaDatiNoCalcolo = listaDatiNoCalcolo;
            #endregion DatiNoCalcolo

            //ENG - Memo 28_2024
            #region ScadenzaRevisioneSanitaria
            DateTime? scadenzaRevisioneSanitaria = null;
            MappingDaHostNew.ValorizzaScadenzaRevisioneSanitaria(AreaPrelievo, tipoDomanda, datiPensione, gruppo, prodotto, tipoFondo, datiPensioneFondoDatiGenerici, risposta.DataDecorrenza, out scadenzaRevisioneSanitaria);
            if (scadenzaRevisioneSanitaria.HasValue)
            {
                if (risposta.DatiIstruttoria == null)
                    risposta.DatiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();

                risposta.DatiIstruttoria.ScadenzaRevisioneSanitaria = scadenzaRevisioneSanitaria;
            }
            #endregion ScadenzaRevisioneSanitaria

            List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> datiQuoteMiglioramentiContrattuali = null;
            MappingDaHostNew.ValorizzaDatiQuoteMiglioramentiContrattuali(AreaPrelievo, ref datiPensione, out datiQuoteMiglioramentiContrattuali);
            risposta.DatiQuoteMiglioramentiContrattuali = datiQuoteMiglioramentiContrattuali;

            GestionePensioneInailInabilita.DatiInabilita datiInabilita = null;
            MappingDaHostNew.ValorizzaMinimo_PensInv(AreaPrelievo, out datiInabilita);
            risposta.DatiInabilita = datiInabilita;
        }

        #endregion private members

        #region nested class
        [Serializable]
        public class RichiestaPrelievo
        {
            public RichiestaPrelievo(short sede, short categoria, int certificato, short sedeOperatore, short centroOperativoOperatore, TipoDomanda tipoDomanda, string numDomanda, string siglaCategoria, string gruppo, string prodotto, string tipo)
            {
                this._Sede = sede;
                this._Categoria = categoria;
                this._Certificato = certificato;
                this._SedeOperatore = sedeOperatore;
                this._CentroOperativoOperatore = sedeOperatore;
                this._TipoDomanda = tipoDomanda;
                this._NumDomanda = numDomanda;
                this._SiglaCategoria = siglaCategoria;
                this._Gruppo = gruppo;
                this._Prodotto = prodotto;
                this._Tipo = tipo;
            }

            #region public properties
            public short Sede { get { return _Sede; } set { _Sede = value; } }
            public short Categoria { get { return _Categoria; } set { _Categoria = value; } }
            public int Certificato { get { return _Certificato; } set { _Certificato = value; } }
            public short SedeOperatore { get { return _SedeOperatore; } set { _SedeOperatore = value; } }
            public short CentroOperativoOperatore { get { return _CentroOperativoOperatore; } set { _CentroOperativoOperatore = value; } }
            public TipoDomanda TipoDomanda { get { return _TipoDomanda; } set { _TipoDomanda = value; } }
            public string NumDomanda { get { return _NumDomanda; } set { _NumDomanda = value; } }
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            #endregion public properties

            #region private properties
            private short _Sede;
            private short _Categoria;
            private int _Certificato;
            private short _SedeOperatore;
            private short _CentroOperativoOperatore;
            private TipoDomanda _TipoDomanda;
            private string _NumDomanda;
            private string _SiglaCategoria;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;
            #endregion private properties
        }

        public class RispostaPrelievo
        {
            #region public properties
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public System.Nullable<DateTime> DataDecorrenza { get { return _DataDecorrenza; } set { _DataDecorrenza = value; } }
            public GestionePensione.DatiPensione DatiPensione { get { return _DatiPensione; } set { _DatiPensione = value; } }
            public GestioneLavorazione.DatiLavorazione DatiLavorazione { get { return _DatiLavorazione; } set { _DatiLavorazione = value; } }
            public GestionePensione.DatiEliminazione DatiEliminazione { get { return _DatiEliminazione; } set { _DatiEliminazione = value; } }
            public GestioneFondo.DatiFondo DatiPensioneFondoDatiGenerici { get { return _DatiPensioneFondoDatiGenerici; } set { _DatiPensioneFondoDatiGenerici = value; } }
            public DatiFondoSpecifico DatiFondoSpecifico { get { return _DatiFondoSpecifico; } set { _DatiFondoSpecifico = value; } }
            public GestionePensione.DatiSindacato DatiSindacato { get { return _DatiSindacato; } set { _DatiSindacato = value; } }
            public GestioneDetrazioniImposta.DatiDetrazioni DatiDetrazioni { get { return _DatiDetrazioni; } set { _DatiDetrazioni = value; } }
            public GestionePagamento.DatiPagamento DatiPagamento { get { return _DatiPagamento; } set { _DatiPagamento = value; } }
            public List<Entity.DatiFamiliari> ListaFamiliari { get { return _ListaFamiliari; } set { _ListaFamiliari = value; } }
            public List<GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato> ListaDetrazioniContitolare { get { return _ListaDetrazioniContitolare; } set { _ListaDetrazioniContitolare = value; } }
            public List<GestioneRecordFondo.DatiRecordFondo> ListaRecordFondo { get { return _ListaRecordFondo; } set { _ListaRecordFondo = value; } }
            public List<GestioneCalcolo.DatiCalcoloContributivo> ListaDatiCalcoloContributivo { get { return _ListaDatiCalcoloContributivo; } set { _ListaDatiCalcoloContributivo = value; } }
            public GestioneCalcolo.DatiCalcoloRetributivo DatiCalcoloRetributivo { get { return _DatiCalcoloRetributivo; } set { _DatiCalcoloRetributivo = value; } }
            public List<GestioneCalcolo.DatiCalcoloRetributivo> DatiCalcoloRetributivoDZ { get { return _ListDatiCalcoloRetributivoDZ; } set { _ListDatiCalcoloRetributivoDZ = value; } }
            public List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> ListaSupplementi { get { return _ListaSupplementi; } set { _ListaSupplementi = value; } }
            public GestionePensione.DatiPatronato DatiPatronato { get { return _DatiPatronato; } set { _DatiPatronato = value; } }
            public MappingDaHost.DatiAnagDanteCausa DatiAnagDanteCausa { get { return _DatiAnagDanteCausa; } set { _DatiAnagDanteCausa = value; } }
            public GestioneDanteCausa.DatiDanteCausa DatiDanteCausa { get { return _DatiDanteCausa; } set { _DatiDanteCausa = value; } }
            public List<GestioneAnagrafica.DatiResidenzaEstero> ListaResidenzeEstere { get { return _ListaResidenzeEstere; } set { _ListaResidenzeEstere = value; } }
            public List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCivili { get { return _ListaStatiCivili; } set { _ListaStatiCivili = value; } }
            public GestioneIstruttoria.DatiIstruttoria DatiIstruttoria { get { return _DatiIstruttoria; } set { _DatiIstruttoria = value; } }
            public INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici DatiMaggiorazioniBenefici { get { return _DatiMaggiorazioniBenefici; } set { _DatiMaggiorazioniBenefici = value; } }
            public GestioneDL407.DatiDL407 DatiDL407 { get { return _DatiDL407; } set { _DatiDL407 = value; } }
            public List<Entity.DatiBenefici.OneriTerrorismo> ListaOneriTerrorismo { get { return _ListaOneriTerrorismo; } set { _ListaOneriTerrorismo = value; } }
            public List<GestioneOneri.DatiOneri> ListaDatiOneri { get { return _ListaDatiOneri; } set { _ListaDatiOneri = value; } }
            public List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaDatiBeneficiParticolari { get { return _ListaDatiBeneficiParticolari; } set { _ListaDatiBeneficiParticolari = value; } }
            public List<GestioneDatiServizioUtile.ServizioUtile> ListaDatiServizioUtile { get { return _ListaDatiServizioUtile; } set { _ListaDatiServizioUtile = value; } }
            public List<GestioneDatiServizioUtileINPDAP.ServizioUtile> ListaDatiServizioUtileINPDAP { get { return _ListaDatiServizioUtileINPDAP; } set { _ListaDatiServizioUtileINPDAP = value; } }
            public List<Entity.DatiNoCalcolo> ListaDatiNoCalcolo { get { return _ListaDatiNoCalcolo; } set { _ListaDatiNoCalcolo = value; } }
            public List<GestioneCalcolo.ServizioUtile707> ListaDatiServizioUtile707 { get { return _ListaDatiServizioUtile707; } set { _ListaDatiServizioUtile707 = value; } }
            public List<GestioneCalcolo.ServizioUtileINPDAP707> ListaDatiServizioUtileINPDAP707 { get { return _ListaDatiServizioUtileINPDAP707; } set { _ListaDatiServizioUtileINPDAP707 = value; } }
            public List<GestionePensioneINPDAP.DatiPensioneINPDAP> ListaDatiPensioneINPDAP { get { return _ListaDatiPensioneINPDAP; } set { _ListaDatiPensioneINPDAP = value; } }
            public List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> ListaRecordDatiFondoINPDAP { get { return _ListaRecordDatiFondoINPDAP; } set { _ListaRecordDatiFondoINPDAP = value; } }
            public List<GestionePensioneInailInabilita.DatiPensioniINAIL> ListaInail { get { return _ListaInail; } set { _ListaInail = value; } }
            public string SiglaCategoriaPensioneReversibilita { get { return _SiglaCategoriaPensioneReversibilita; } set { _SiglaCategoriaPensioneReversibilita = value; } }

            public MappingDaHost.DatiDelegato DatiDelegato { get { return _DatiDelegato; } set { _DatiDelegato = value; } }
            public MappingDaHost.DatiTutore DatiTutore { get { return _DatiTutore; } set { _DatiTutore = value; } }

            public List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> DatiQuoteMiglioramentiContrattuali { get { return _DatiQuoteMiglioramentiContrattuali; } set { _DatiQuoteMiglioramentiContrattuali = value; } }

            public GestionePensioneInailInabilita.DatiInabilita DatiInabilita { get { return _DatiInabilita; } set { _DatiInabilita = value; } }
            #endregion public properties

            #region private properties
            private string _CodiceFiscale;
            private System.Nullable<DateTime> _DataDecorrenza;
            private GestionePensione.DatiPensione _DatiPensione;
            private GestioneLavorazione.DatiLavorazione _DatiLavorazione;
            private GestionePensione.DatiEliminazione _DatiEliminazione;
            private GestioneFondo.DatiFondo _DatiPensioneFondoDatiGenerici;
            private DatiFondoSpecifico _DatiFondoSpecifico;
            private GestionePensione.DatiSindacato _DatiSindacato;
            private GestioneDetrazioniImposta.DatiDetrazioni _DatiDetrazioni;
            private GestionePagamento.DatiPagamento _DatiPagamento;
            private List<Entity.DatiFamiliari> _ListaFamiliari;
            private List<GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato> _ListaDetrazioniContitolare;
            private List<GestioneRecordFondo.DatiRecordFondo> _ListaRecordFondo;
            private List<GestioneCalcolo.DatiCalcoloContributivo> _ListaDatiCalcoloContributivo;
            private GestioneCalcolo.DatiCalcoloRetributivo _DatiCalcoloRetributivo;
            private List<GestioneCalcolo.DatiCalcoloRetributivo> _ListDatiCalcoloRetributivoDZ;
            private List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> _ListaSupplementi;
            private GestionePensione.DatiPatronato _DatiPatronato;
            private MappingDaHost.DatiAnagDanteCausa _DatiAnagDanteCausa;
            private GestioneDanteCausa.DatiDanteCausa _DatiDanteCausa;
            private List<GestioneAnagrafica.DatiResidenzaEstero> _ListaResidenzeEstere;
            private List<GestioneAnagrafica.DatiStatoCivile> _ListaStatiCivili;
            private GestioneIstruttoria.DatiIstruttoria _DatiIstruttoria;
            private INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici _DatiMaggiorazioniBenefici;
            private GestioneDL407.DatiDL407 _DatiDL407;
            private List<Entity.DatiBenefici.OneriTerrorismo> _ListaOneriTerrorismo;
            private List<GestioneOneri.DatiOneri> _ListaDatiOneri;
            private List<GestioneBeneficiParticolari.DatiBeneficiParticolari> _ListaDatiBeneficiParticolari;
            private List<GestioneDatiServizioUtile.ServizioUtile> _ListaDatiServizioUtile;
            private List<GestioneDatiServizioUtileINPDAP.ServizioUtile> _ListaDatiServizioUtileINPDAP;
            private List<Entity.DatiNoCalcolo> _ListaDatiNoCalcolo;
            private List<GestioneCalcolo.ServizioUtile707> _ListaDatiServizioUtile707;
            private List<GestioneCalcolo.ServizioUtileINPDAP707> _ListaDatiServizioUtileINPDAP707;
            private List<GestionePensioneINPDAP.DatiPensioneINPDAP> _ListaDatiPensioneINPDAP;
            private List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> _ListaRecordDatiFondoINPDAP;
            private List<GestionePensioneInailInabilita.DatiPensioniINAIL> _ListaInail;
            private string _SiglaCategoriaPensioneReversibilita;
            private MappingDaHost.DatiDelegato _DatiDelegato;
            private MappingDaHost.DatiTutore _DatiTutore;
            private List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> _DatiQuoteMiglioramentiContrattuali;
            private GestionePensioneInailInabilita.DatiInabilita _DatiInabilita;
            #endregion private properties
        }

        [Serializable]
        public enum TipoDomanda
        {
            Reversibilità,
            Ricostituzione,
            Ripristino,
            RipristinoSuperstiti,
            Riliquidazione,
            RiliquidazioneSuperstiti
        };

        public class DatiFondoSpecifico
        {

            #region public members
            public GestioneFondo.DatiFondoEL DatiPensioneFondoDatiEL { get { return _DatiPensioneFondoDatiEL; } set { _DatiPensioneFondoDatiEL = value; } }
            public GestioneFondo.DatiFondoTT DatiPensioneFondoDatiTT { get { return _DatiPensioneFondoDatiTT; } set { _DatiPensioneFondoDatiTT = value; } }
            public GestioneFondo.DatiFondoET DatiPensioneFondoDatiET { get { return _DatiPensioneFondoDatiET; } set { _DatiPensioneFondoDatiET = value; } }
            public GestioneFondo.DatiFondoVL DatiPensioneFondoDatiVL { get { return _DatiPensioneFondoDatiVL; } set { _DatiPensioneFondoDatiVL = value; } }
            public List<GestioneFondo.DatiFondoPT> DatiPensioneFondoDatiPT { get { return _DatiPensioneFondoDatiPT; } set { _DatiPensioneFondoDatiPT = value; } }
            public List<GestioneFondo.DatiFondoFST> DatiPensioneFondoDatiFS { get { return _DatiPensioneFondoDatiFS; } set { _DatiPensioneFondoDatiFS = value; } }
            public List<GestioneFondo.DatiFondoPI> DatiPensioneFondoDatiPI { get { return _DatiPensioneFondoDatiPI; } set { _DatiPensioneFondoDatiPI = value; } }
            public GestioneFondo.DatiFondoGAS DatiPensioneFondoDatiGAS { get { return _DatiPensioneFondoDatiGAS; } set { _DatiPensioneFondoDatiGAS = value; } }
            public GestioneFondo.DatiFondoCL DatiPensioneFondoDatiCL { get { return _DatiPensioneFondoDatiCL; } set { _DatiPensioneFondoDatiCL = value; } }
            public List<GestioneFondo.DatiFondoDZ> DatiPensioneFondoDatiDZ { get; set; } //Nuova Gestione Dazi Daniele
            public GestioneFondo.DatiFondoES DatiPensioneFondoDatiES { get { return _DatiPensioneFondoDatiES; } set { _DatiPensioneFondoDatiES = value; } }
            public List<GestioneFondo.DatiFondoPM> DatiPensioneFondoDatiPM { get; set; }
            public List<GestioneFondo.DatiAgoPI> DatiPensioneAgoPI { get { return _DatiPensioneAgoPI; } set { _DatiPensioneAgoPI = value; } }

            public List<GestioneFondo.DatiAgoTeoricoPI> DatiPensioneAgoTeoricoPI { get { return _DatiPensioneAgoTeoricoPI; } set { _DatiPensioneAgoTeoricoPI = value; } }

            public List<GestioneFondo.DatiAgoPM> DatiPensioneAgoPM { get { return _DatiPensioneAgoPM; } set { _DatiPensioneAgoPM = value; } }

            #endregion public members

            #region private members
            private GestioneFondo.DatiFondoEL _DatiPensioneFondoDatiEL;
            private GestioneFondo.DatiFondoTT _DatiPensioneFondoDatiTT;
            private GestioneFondo.DatiFondoET _DatiPensioneFondoDatiET;
            private GestioneFondo.DatiFondoVL _DatiPensioneFondoDatiVL;
            private List<GestioneFondo.DatiFondoPT> _DatiPensioneFondoDatiPT;
            private List<GestioneFondo.DatiFondoFST> _DatiPensioneFondoDatiFS;
            private List<GestioneFondo.DatiFondoPI> _DatiPensioneFondoDatiPI;
            private GestioneFondo.DatiFondoGAS _DatiPensioneFondoDatiGAS;
            private GestioneFondo.DatiFondoCL _DatiPensioneFondoDatiCL;
            private GestioneFondo.DatiFondoES _DatiPensioneFondoDatiES;
            private List<GestioneFondo.DatiAgoPI> _DatiPensioneAgoPI;
            private List<GestioneFondo.DatiAgoTeoricoPI> _DatiPensioneAgoTeoricoPI;
            private List<GestioneFondo.DatiAgoPM> _DatiPensioneAgoPM;
            #endregion private members
        }

        //public class DatiAgoPI
        //{
        //    // Dati generali AGO
        //    public DateTime DecorrenzaDatiAgo { get; set; }
        //    public string CodiceSpecificoAgo { get; set; }
        //    public short TipoLiquidazione { get; set; }
        //    public DateTime SospensioneAGO { get; set; }
        //    public string CodiceNatura { get; set; }
        //    public short SettimaneVV { get; set; }

        //    // Quota A
        //    public decimal RMSQuotaA { get; set; }
        //    public int NSettimaneQuotaA { get; set; }
        //    public int NSettimaneEsclusiveQuotaA { get; set; }

        //    // Quota B
        //    public decimal RMSQuotaB { get; set; }
        //    public int NSettimaneQuotaB { get; set; }
        //    public int NSettimaneEsclusiveQuotaB { get; set; }

        //    // Totali
        //    public decimal Montante { get; set; }
        //    public decimal MontanteEsclusivo { get; set; }
        //    public int NSettimane { get; set; }
        //}

        #endregion nested class
    }
}



